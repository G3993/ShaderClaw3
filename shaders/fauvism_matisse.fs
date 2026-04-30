/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Fauvism after Matisse — living pigment. Persistent paint buffer advected by a curl-noise velocity field; new drops of pure Fauvist primary colour erupt continuously. TouchDesigner-style fluid feedback as wild colour liberation, no cells, no outlines.",
  "INPUTS": [
    { "NAME": "swirlStrength", "LABEL": "Swirl Strength", "TYPE": "float", "MIN": 0.0, "MAX": 0.05, "DEFAULT": 0.012 },
    { "NAME": "swirlScale", "LABEL": "Swirl Scale", "TYPE": "float", "MIN": 1.0, "MAX": 8.0, "DEFAULT": 3.0 },
    { "NAME": "flowSpeed", "LABEL": "Flow Speed", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25 },
    { "NAME": "paintFade", "LABEL": "Paint Persistence", "TYPE": "float", "MIN": 0.92, "MAX": 0.995, "DEFAULT": 0.992 },
    { "NAME": "dropRate", "LABEL": "Drop Rate", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.75 },
    { "NAME": "dropSize", "LABEL": "Drop Size", "TYPE": "float", "MIN": 0.005, "MAX": 0.10, "DEFAULT": 0.040 },
    { "NAME": "saturationBoost", "LABEL": "Saturation", "TYPE": "float", "MIN": 0.6, "MAX": 1.8, "DEFAULT": 1.15 },
    { "NAME": "paperShow", "LABEL": "Paper Show-through", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.18 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "paletteShift", "LABEL": "Palette Shift", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "inputBleed", "LABEL": "Input Bleed", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.0 },
    { "NAME": "resetField", "LABEL": "Reset", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "paintA", "PERSISTENT": true },
    { "TARGET": "paintB", "PERSISTENT": true },
    { "TARGET": "paintC", "PERSISTENT": true },
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

// 2-octave value-noise — adds higher-frequency detail so curl produces
// real vortex structure instead of broad smooth sweeps.
float vnoise2(vec2 p) {
    return vnoise(p) * 0.65 + vnoise(p * 2.31 + 5.7) * 0.35;
}

// Curl noise → divergence-free 2D velocity field. 2-octave gives the
// smoke-like vortex shedding that single-octave curl misses.
vec2 curl(vec2 p) {
    float e = 0.01;
    float n1 = vnoise2(p + vec2(0.0,  e)) - vnoise2(p - vec2(0.0,  e));
    float n2 = vnoise2(p + vec2( e, 0.0)) - vnoise2(p - vec2( e, 0.0));
    return vec2(n1, -n2) / (2.0 * e);
}

vec3 saturateColor(vec3 c, float amt) {
    float l = dot(c, vec3(0.299, 0.587, 0.114));
    return mix(vec3(l), c, amt);
}

// Run one paint-sim step on a chosen buffer with a per-buffer time
// offset + palette shift. Three buffers (A, B, C) loop independently so
// the canvas is always layering 3 paint snapshots at different ages.
vec3 simulatePaintStep(int bufIdx, vec2 uv, float aspect, vec3 prev) {
    float bufTime  = TIME + float(bufIdx) * 3.7;
    float bufPalSh = paletteShift + float(bufIdx) * 0.33;

    float gridN = 16.0;
    vec3 deposit = prev;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            vec2 g = floor(uv * gridN)
                   + vec2(floor(bufTime * 0.13), floor(bufTime * 0.17))
                   + vec2(float(i), float(j));
            float bucket = floor(bufTime * (1.0 + audioMid * 2.0) * 4.0);
            vec2 seed = g + bucket * 7.31 + float(bufIdx) * 19.7;
            float roll = hash21(seed);
            if (roll > (1.0 - dropRate * (0.85 + audioBass * 0.4))) continue;
            vec2 c = (g + vec2(hash21(seed + 13.7),
                               hash21(seed + 19.3))) / gridN;
            vec2 d = uv - c;
            d.x *= aspect;
            float r = dropSize * (0.5 + hash21(seed + 31.1));
            float falloff = smoothstep(r * 1.4, 0.0, length(d));
            falloff = falloff * falloff * (3.0 - 2.0 * falloff);
            if (falloff < 0.001) continue;
            int ci = int(mod(hash21(seed + 47.9) * 6.0
                          + bufPalSh * 6.0, 6.0));
            deposit = mix(deposit, FAUVE[ci], falloff);
        }
    }
    return deposit;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Curl velocity (shared by all paint passes)
    vec2 vel = curl(vec2(uv.x * aspect, uv.y) * swirlScale
                  + TIME * flowSpeed)
             * swirlStrength * (1.0 + audioBass * audioReact * 1.5);

    // ============= PASS 0/1/2 — three independent paint buffers =============
    if (PASSINDEX < 3) {
        if (FRAMEINDEX < 2 || resetField) {
            gl_FragColor = vec4(PAPER, 1.0);
            return;
        }
        vec3 prev;
        if (PASSINDEX == 0)      prev = texture(paintA, uv - vel * 0.4).rgb;
        else if (PASSINDEX == 1) prev = texture(paintB, uv - vel * 0.7).rgb;
        else                     prev = texture(paintC, uv - vel * 1.1).rgb;
        vec3 step = simulatePaintStep(PASSINDEX, uv, aspect, prev);
        gl_FragColor = vec4(step, 1.0);
        return;
    }

    // ============= PASS 3 — output: looping composite of all 3 =============
    vec3 colA = texture(paintA, uv).rgb;
    vec3 colB = texture(paintB, uv).rgb;
    vec3 colC = texture(paintC, uv).rgb;
    // Looping weights cycle A→B→C→A every ~6 sec via three phase-shifted
    // sinusoids that always sum to 1.
    float t = TIME * 0.18;
    float wA = 0.5 + 0.5 * sin(t);
    float wB = 0.5 + 0.5 * sin(t + 2.094);  // 120° offset
    float wC = 0.5 + 0.5 * sin(t + 4.189);  // 240° offset
    float wsum = wA + wB + wC;
    vec3 col = (colA * wA + colB * wB + colC * wC) / wsum;
    col = saturateColor(col,
              saturationBoost * (0.85 + audioLevel * audioReact * 0.25));
    float grain = (hash21(uv * RENDERSIZE) - 0.5) * 0.04;
    col += grain * paperShow;
    gl_FragColor = vec4(col, 1.0);
    return;

    // (Old single-buffer code below kept for reference — unreached.)
    vec3 prevDead = vec3(0.0);
        // No fade — drops alone refresh the canvas. Reduced advection
        // strength (0.4×) inside the sample so curl smears gently
        // without averaging neighbours toward grey over time.

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
                // Time bucket — faster cycle so different cells fire each
                // ~0.25s, keeping coverage continuous instead of leaving
                // empty regions to fade to paper.
                float bucket = floor(TIME * (1.0 + audioMid * 2.0) * 4.0);
                vec2 seed = g + bucket * 7.31;
                float roll = hash21(seed);
                // Always-on floor: at silence the threshold is
                // 1 - dropRate*0.85, so 65% of cells fire at default.
                if (roll > (1.0 - dropRate * (0.85 + audioBass * 0.4))) continue;

                // Drop centre — jittered inside its grid cell.
                vec2 c = (g + vec2(hash21(seed + 13.7),
                                   hash21(seed + 19.3))) / gridN;
                vec2 d = uv - c;
                d.x *= aspect; // aspect-corrected so drops are circles in screen space
                float r = dropSize * (0.5 + hash21(seed + 31.1));
                // Wider/softer falloff — drops blend into each other
                // smoothly instead of leaving hard pixelated edges.
                float falloff = smoothstep(r * 1.4, 0.0, length(d));
                falloff = falloff * falloff * (3.0 - 2.0 * falloff);
                if (falloff < 0.001) continue;

                int ci = int(mod(hash21(seed + 47.9) * 6.0
                              + paletteShift * 6.0, 6.0));
                vec3 dropCol = FAUVE[ci];
                // Full overwrite at drop centre — guarantees fresh
                // pure colour each frame instead of progressive blend.
                deposit = mix(deposit, dropCol, falloff);
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

    // Output — just the paint buffer + saturation + grain. No depth
    // pass, no snap, no chroma lift. The early-iterations look.
    vec3 col = texture(paintBuf, uv).rgb;
    col = saturateColor(col,
              saturationBoost * (0.85 + audioLevel * audioReact * 0.25));
    float grain = (hash21(uv * RENDERSIZE) - 0.5) * 0.04;
    col += grain * paperShow;

    gl_FragColor = vec4(col, 1.0);
}
