/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Fauvism after Matisse — living pigment. Persistent paint buffer advected by a curl-noise velocity field; new drops of pure Fauvist primary colour erupt continuously. TouchDesigner-style fluid feedback as wild colour liberation, no cells, no outlines.",
  "INPUTS": [
    { "NAME": "swirlStrength", "LABEL": "Swirl Strength", "TYPE": "float", "MIN": 0.0, "MAX": 0.05, "DEFAULT": 0.012 },
    { "NAME": "swirlScale", "LABEL": "Swirl Scale", "TYPE": "float", "MIN": 1.0, "MAX": 8.0, "DEFAULT": 3.0 },
    { "NAME": "flowSpeed", "LABEL": "Flow Speed", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25 },
    { "NAME": "paintFade", "LABEL": "Paint Persistence", "TYPE": "float", "MIN": 0.92, "MAX": 1.0, "DEFAULT": 0.988 },
    { "NAME": "dropRate", "LABEL": "Drop Rate", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "dropSize", "LABEL": "Drop Size", "TYPE": "float", "MIN": 0.005, "MAX": 0.08, "DEFAULT": 0.028 },
    { "NAME": "saturationBoost", "LABEL": "Saturation", "TYPE": "float", "MIN": 0.6, "MAX": 1.8, "DEFAULT": 1.15 },
    { "NAME": "paperShow", "LABEL": "Paper Show-through", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.18 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "paletteShift", "LABEL": "Palette Shift", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "inputBleed", "LABEL": "Input Bleed", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.0 },
    { "NAME": "resetField", "LABEL": "Reset", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "paintBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Hand-tuned Fauvist palette — Matisse "Open Window, Collioure" 1905;
// Derain "Charing Cross Bridge" 1906. Pure unmodulated complementaries.
const vec3 FAUVE[6] = vec3[6](
    vec3(0.94, 0.30, 0.55),  // hot pink
    vec3(0.96, 0.32, 0.16),  // vermilion
    vec3(0.97, 0.83, 0.20),  // lemon
    vec3(0.30, 0.66, 0.32),  // sap green
    vec3(0.10, 0.32, 0.78),  // cobalt
    vec3(0.62, 0.34, 0.68)   // mauve
);
const vec3 PAPER = vec3(0.96, 0.93, 0.85);

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

// Curl noise → divergence-free 2D velocity field. Advecting a buffer by
// curl(fbm) gives the smoke-like swirl that TD's "FluidStream" macro
// uses, without solving Navier-Stokes.
vec2 curl(vec2 p) {
    float e = 0.01;
    float n1 = vnoise(p + vec2(0.0,  e)) - vnoise(p - vec2(0.0,  e));
    float n2 = vnoise(p + vec2( e, 0.0)) - vnoise(p - vec2( e, 0.0));
    return vec2(n1, -n2) / (2.0 * e);
}

vec3 saturateColor(vec3 c, float amt) {
    float l = dot(c, vec3(0.299, 0.587, 0.114));
    return mix(vec3(l), c, amt);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // ============= PASS 0 — paintBuf advection + drop deposition =============
    if (PASSINDEX == 0) {

        // Reset / first frame → paper.
        if (FRAMEINDEX < 2 || resetField) {
            gl_FragColor = vec4(PAPER, 1.0);
            return;
        }

        // Sample previous frame at a UV displaced by curl noise. This is
        // the advection step — pigment flows along the velocity field.
        vec2 vel = curl(vec2(uv.x * aspect, uv.y) * swirlScale
                      + TIME * flowSpeed)
                 * swirlStrength * (1.0 + audioBass * audioReact * 1.5);
        vec3 prev = texture(paintBuf, uv - vel).rgb;

        // Slow fade toward paper — pigment evaporates so old strokes die
        // gradually instead of saturating the buffer.
        prev = mix(PAPER, prev, paintFade);

        // Drop deposition: a sparse, audio-driven grid of new pigment
        // drops in random Fauvist primaries. Each pixel decides whether
        // a drop centre is near it and tints toward that drop's colour.
        float gridN = 16.0;
        vec3 deposit = prev;
        for (int j = -1; j <= 1; j++) {
            for (int i = -1; i <= 1; i++) {
                vec2 g = floor(uv * gridN)
                       + vec2(floor(TIME * 0.13), floor(TIME * 0.17))
                       + vec2(float(i), float(j));
                // Time bucket so drops emerge in waves, not constantly.
                float bucket = floor(TIME * (1.0 + audioMid * 2.0) * 1.5);
                vec2 seed = g + bucket * 7.31;
                float roll = hash21(seed);
                if (roll > (1.0 - dropRate * (0.7 + audioBass * 0.6))) continue;

                // Drop centre — jittered inside its grid cell.
                vec2 c = (g + vec2(hash21(seed + 13.7),
                                   hash21(seed + 19.3))) / gridN;
                vec2 d = uv - c;
                d.x *= aspect / gridN * gridN; // approx square space
                float r = dropSize * (0.5 + hash21(seed + 31.1));
                float falloff = smoothstep(r, 0.0, length(d));
                if (falloff < 0.001) continue;

                int ci = int(mod(hash21(seed + 47.9) * 6.0
                              + paletteShift * 6.0, 6.0));
                vec3 dropCol = FAUVE[ci];
                deposit = mix(deposit, dropCol, falloff * 0.85);
            }
        }

        // Optional bleed from input texture — uses live video as a paint
        // mask, low opacity so the field is still mostly self-driven.
        if (IMG_SIZE_inputTex.x > 0.0 && inputBleed > 0.0) {
            vec3 src = texture(inputTex, uv).rgb;
            // Snap source to nearest Fauvist primary so it doesn't read
            // photographic — the painter disagrees with reality.
            vec3 best = FAUVE[0];
            float bd = 1e9;
            for (int k = 0; k < 6; k++) {
                float dd = dot(src - FAUVE[k], src - FAUVE[k]);
                if (dd < bd) { bd = dd; best = FAUVE[k]; }
            }
            deposit = mix(deposit, best, inputBleed);
        }

        gl_FragColor = vec4(deposit, 1.0);
        return;
    }

    // ============= PASS 1 — output ============================================

    vec3 col = texture(paintBuf, uv).rgb;
    col = saturateColor(col,
              saturationBoost * (0.85 + audioLevel * audioReact * 0.25));

    // Snap to nearest Fauvist primary — pulls washy paint values toward
    // unmodulated colour like Matisse's palette discipline. The 0.35
    // mix keeps brushwork variance, but the dominant hue snaps clean.
    vec3 best = FAUVE[0];
    float bd = 1e9;
    for (int k = 0; k < 6; k++) {
        float dd = dot(col - FAUVE[k], col - FAUVE[k]);
        if (dd < bd) { bd = dd; best = FAUVE[k]; }
    }
    col = mix(col, best, 0.35);

    // Complementary edge boost — Matisse "Green Line" portrait vibrate.
    float Lc = dot(col, vec3(0.299, 0.587, 0.114));
    float ex = abs(dFdx(Lc)) + abs(dFdy(Lc));
    col = mix(col, vec3(1.0) - col,
              smoothstep(0.05, 0.20, ex) * 0.40);

    // Paper grain showing through where pigment is thin.
    float grain = (hash21(uv * RENDERSIZE) - 0.5) * 0.04;
    col += grain * paperShow;

    gl_FragColor = vec4(col, 1.0);
}
