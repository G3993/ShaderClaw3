/*{
  "DESCRIPTION": "Molten Lava — triple domain-warp FBM fluid with hot lava palette",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "audioReact",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8 },
    { "NAME": "flowSpeed",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.35 },
    { "NAME": "warpStrength", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.3 }
  ]
}*/

precision highp float;

// ---------------------------------------------------------------------------
// PALETTE — exactly 5 hand-chosen lava colors, no others
// ---------------------------------------------------------------------------
#define BLACK_CRUST vec3(0.02, 0.01, 0.0)
#define CRIMSON     vec3(0.6,  0.0,  0.0)
#define ORANGE      vec3(1.0,  0.35, 0.0)
#define YELLOW      vec3(1.0,  0.85, 0.0)
#define WHITE_HOT   vec3(2.5,  2.2,  1.8)   // HDR peak

// ---------------------------------------------------------------------------
// Hash & FBM
// ---------------------------------------------------------------------------
vec2 hash2(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)),
             dot(p, vec2(269.5, 183.3)));
    return fract(sin(p) * 43758.5453123);
}

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash(i + vec2(0.0, 0.0)),
                   hash(i + vec2(1.0, 0.0)), u.x),
               mix(hash(i + vec2(0.0, 1.0)),
                   hash(i + vec2(1.0, 1.0)), u.x), u.y);
}

float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    vec2  shift = vec2(100.0);
    mat2  rot   = mat2(cos(0.5), sin(0.5), -sin(0.5), cos(0.5));
    for (int i = 0; i < 5; i++) {
        v += a * noise(p);
        p  = rot * p * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}

// ---------------------------------------------------------------------------
// Piecewise lava color map  (f in 0..1)
// ---------------------------------------------------------------------------
vec3 lavaColor(float f) {
    // 0.00 – 0.25 : BLACK_CRUST → CRIMSON
    // 0.25 – 0.50 : CRIMSON     → ORANGE
    // 0.50 – 0.72 : ORANGE      → YELLOW
    // 0.72 – 1.00 : YELLOW      → WHITE_HOT
    vec3 c;
    if (f < 0.25) {
        c = mix(BLACK_CRUST, CRIMSON, smoothstep(0.0, 0.25, f));
    } else if (f < 0.50) {
        c = mix(CRIMSON, ORANGE, smoothstep(0.25, 0.50, f));
    } else if (f < 0.72) {
        c = mix(ORANGE, YELLOW, smoothstep(0.50, 0.72, f));
    } else {
        c = mix(YELLOW, WHITE_HOT, smoothstep(0.72, 1.0, f));
    }
    return c;
}

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------
void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Scale to aspect-correct coords, lava flows horizontally
    vec2 p = uv * vec2(aspect, 1.0) * 3.5;

    float t    = TIME * flowSpeed;
    float audio = 0.5 + 0.5 * audioBass * audioReact;  // modulator pattern
    float ws    = warpStrength * audio;                  // audio drives warp amplitude

    // ---------------------------------------------------------------------------
    // Triple domain warp  (classic Quilez / Inigo technique)
    // q = first warp layer
    vec2 q = vec2(
        fbm(p + t * vec2(0.13, 0.07)),
        fbm(p + vec2(5.2, 1.3) + t * vec2(0.09, 0.11))
    );
    // r = second warp layer driven by q
    vec2 r = vec2(
        fbm(p + ws * q + vec2(1.7, 9.2) + t * vec2(0.07, 0.05)),
        fbm(p + ws * q + vec2(8.3, 2.8) + t * vec2(0.11, 0.09))
    );
    // f = final warped FBM
    float f = fbm(p + ws * r);

    // Remap 0..1 softly
    f = clamp(f, 0.0, 1.0);

    // ---------------------------------------------------------------------------
    // Base lava color
    vec3 col = lavaColor(f);

    // ---------------------------------------------------------------------------
    // Crack veins: iso-contours at high f values
    // Only where f > 0.72 (the hot zone)
    if (f > 0.72) {
        float crackF   = fract(f * 6.0) - 0.5;
        float fw       = max(fwidth(f) * 6.0, 0.001);
        float crackMask = 1.0 - smoothstep(0.5, 2.0, abs(crackF) / fw);
        // Only blend crack veins in the hot zone
        float hotMask  = smoothstep(0.72, 0.85, f);
        col = mix(col, WHITE_HOT, crackMask * hotMask);
    }

    // ---------------------------------------------------------------------------
    // Output — linear HDR, no clamp, no ACES, no gamma
    gl_FragColor = vec4(col, 1.0);
}
