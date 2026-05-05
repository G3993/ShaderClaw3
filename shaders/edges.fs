/*{
    "DESCRIPTION": "Molten Lava — triple domain-warp FBM fluid with hot lava palette",
    "CREDIT": "ShaderClaw auto-improve v2",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        {
            "NAME": "audioReact",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "flowSpeed",
            "TYPE": "float",
            "DEFAULT": 0.35,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "warpStrength",
            "TYPE": "float",
            "DEFAULT": 1.3,
            "MIN": 0.0,
            "MAX": 3.0
        }
    ]
}*/

precision highp float;

// ---- Palette (exactly 5 chosen lava colors) ----
const vec3 BLACK_CRUST = vec3(0.02, 0.01, 0.0);
const vec3 CRIMSON     = vec3(0.6,  0.0,  0.0);
const vec3 ORANGE      = vec3(1.0,  0.35, 0.0);
const vec3 YELLOW      = vec3(1.0,  0.85, 0.0);
const vec3 WHITE_HOT   = vec3(2.5,  2.2,  1.8);  // HDR peak

// ---- Hash noise ----
float hash(vec2 p) {
    p = fract(p * vec2(127.1, 311.7));
    p += dot(p, p + 19.43);
    return fract(p.x * p.y);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(hash(i + vec2(0,0)), hash(i + vec2(1,0)), u.x),
        mix(hash(i + vec2(0,1)), hash(i + vec2(1,1)), u.x),
        u.y
    );
}

// ---- 5-octave FBM ----
float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    vec2  shift = vec2(100.0);
    mat2  rot = mat2(cos(0.5), sin(0.5), -sin(0.5), cos(0.5));
    for (int i = 0; i < 5; i++) {
        v += a * noise(p);
        p  = rot * p * 2.1 + shift;
        a *= 0.5;
    }
    return v;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = uv * vec2(aspect, 1.0) * 2.5;

    float t = TIME * flowSpeed;

    // Audio modulator drives warp amplitude
    float audio = 0.5 + 0.5 * audioBass * audioReact;
    float warp = warpStrength * audio;

    // ---- Triple domain warp ----
    // Layer q
    vec2 q = vec2(
        fbm(p + t * vec2(0.13, 0.07)),
        fbm(p + vec2(5.2, 1.3) + t * vec2(0.11, 0.09))
    );

    // Layer r — warped by q
    vec2 r = vec2(
        fbm(p + warp * q + vec2(1.7,  9.2) + t * vec2(0.15, 0.05)),
        fbm(p + warp * q + vec2(8.3,  2.8) + t * vec2(0.08, 0.13))
    );

    // Final FBM value warped by r
    float f = fbm(p + warp * r);
    f = clamp(f, 0.0, 1.0);

    // ---- Piecewise lava color mapping ----
    vec3 col;
    if (f < 0.20) {
        col = mix(BLACK_CRUST, CRIMSON, smoothstep(0.0, 0.20, f));
    } else if (f < 0.45) {
        col = mix(CRIMSON, ORANGE, smoothstep(0.20, 0.45, f));
    } else if (f < 0.68) {
        col = mix(ORANGE, YELLOW, smoothstep(0.45, 0.68, f));
    } else if (f < 0.82) {
        col = mix(YELLOW, WHITE_HOT, smoothstep(0.68, 0.82, f));
    } else {
        col = WHITE_HOT;
    }

    // ---- Crack veins: iso-contours where f > 0.72 ----
    float crackMask = step(0.72, f);
    float fwidthF = max(fwidth(f) * 6.0, 0.001);
    float crackLine = 1.0 - smoothstep(0.5, 2.0, abs(fract(f * 6.0) - 0.5) / fwidthF);
    col += WHITE_HOT * crackLine * crackMask * 0.9;

    // Output LINEAR HDR — no clamp, no ACES, no gamma
    gl_FragColor = vec4(col, 1.0);
}
