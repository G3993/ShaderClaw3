/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Dada after Hannah Höch's Cut with the Kitchen Knife (1919) and Hausmann's ABCD (1923) — persistent collage buffer where new chaotic strips of input video get glued on at random angles and scales every beat, with sliding chromatic shift, halftone print noise, scattered red ink stamps and stuttering letterforms. Pure anti-art collapse, multi-pass frame feedback.",
  "INPUTS": [
    { "NAME": "stripsPerBeat", "LABEL": "Strips Per Beat", "TYPE": "float", "MIN": 1.0, "MAX": 20.0, "DEFAULT": 8.0 },
    { "NAME": "beatRate", "LABEL": "Beat Rate", "TYPE": "float", "MIN": 0.2, "MAX": 8.0, "DEFAULT": 2.5 },
    { "NAME": "stripScale", "LABEL": "Strip Scale", "TYPE": "float", "MIN": 0.05, "MAX": 0.4, "DEFAULT": 0.18 },
    { "NAME": "rotateRange", "LABEL": "Rotation Range", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.7 },
    { "NAME": "paperFade", "LABEL": "Paper Fade", "TYPE": "float", "MIN": 0.95, "MAX": 1.0, "DEFAULT": 0.992 },
    { "NAME": "chromaSlide", "LABEL": "Chroma Slide", "TYPE": "float", "MIN": 0.0, "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "halftoneAmount", "LABEL": "Halftone", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "halftoneScale", "LABEL": "Halftone Scale", "TYPE": "float", "MIN": 50.0, "MAX": 400.0, "DEFAULT": 180.0 },
    { "NAME": "stampDensity", "LABEL": "Stamp Density", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45 },
    { "NAME": "letterStutter", "LABEL": "Letter Stutter", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "paperTone", "LABEL": "Paper Tone", "TYPE": "color", "DEFAULT": [0.93, 0.88, 0.78, 1.0] },
    { "NAME": "resetField", "LABEL": "Reset", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "collageBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Pass 0 keeps a persistent collage that ACCUMULATES — every "beat" tick
// a handful of new strips get pasted on top at random rotations, scales,
// and source-region offsets. Old strips fade slowly so the collage is
// always evolving. Pass 1 adds halftone, chromatic slide, stamps, and
// stuttering letterforms.

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Beat tick — every 1/beatRate seconds we re-roll a fresh strip set.
    // audioBass adds a tick on top, so live beats trigger collage updates.
    float bt = floor(TIME * beatRate)
             + floor(audioBass * audioReact * 6.0);

    // ============= PASS 0 — collage accumulation =============
    if (PASSINDEX == 0) {

        if (FRAMEINDEX < 2 || resetField) {
            gl_FragColor = vec4(paperTone.rgb, 1.0);
            return;
        }

        vec3 prev = texture(collageBuf, uv).rgb;
        prev = mix(paperTone.rgb, prev, paperFade);

        // Lay down N strips this frame. Each frame paints all SPB strips
        // for the current beat — once the beat ticks, a new set appears.
        int N = int(clamp(stripsPerBeat, 1.0, 20.0));
        for (int i = 0; i < 20; i++) {
            if (i >= N) break;
            float fi = float(i) + bt * 31.7;

            vec2 c  = vec2(hash11(fi * 1.31),
                           hash11(fi * 2.97 + 4.7));
            float r = (hash11(fi * 5.71) - 0.5) * 2.0 * rotateRange;
            float s = stripScale * (0.45 + hash11(fi * 7.13) * 1.1);
            float aR = 0.35 + hash11(fi * 9.71) * 0.7;

            vec2 q = uv - c;
            q.x *= aspect;
            q = mat2(cos(r), -sin(r), sin(r), cos(r)) * q;

            // SDF inside-rectangle test
            if (abs(q.x) > s || abs(q.y) > s * aR) continue;

            // Sample input from a hashed offset region — different
            // viewpoint per strip (this is the photomontage core).
            vec2 sampleUV = (q / vec2(s, s * aR)) * 0.4 + 0.5
                          + vec2(hash11(fi * 11.7) * 1.7,
                                 hash11(fi * 13.3) * 1.7);
            vec3 patch;
            if (IMG_SIZE_inputTex.x > 0.0) {
                patch = texture(inputTex, fract(sampleUV)).rgb;
            } else {
                // Fallback: hashed colour swatches mimicking newsprint cuts
                float hh = hash11(fi * 17.1);
                patch = (hh < 0.3) ? vec3(0.85, 0.78, 0.62)
                       : (hh < 0.55) ? vec3(0.45, 0.40, 0.32)
                       : (hh < 0.78) ? vec3(0.18, 0.16, 0.14)
                       : vec3(0.65, 0.20, 0.18);
            }

            // Random desaturation on some strips — Höch's sources were
            // halftone-grey magazines.
            if (hash11(fi * 19.3) < 0.45) {
                patch = vec3(dot(patch, vec3(0.33)));
            }
            // Random invert on a few — gives Dada wrongness.
            if (hash11(fi * 23.7) < 0.10) {
                patch = vec3(1.0) - patch;
            }

            // Torn-paper irregularity — hash-jagged edge instead of
            // a clean smoothstep so strips read as cut-and-glued, not
            // perfect rectangles.
            float jag = hash21(q * 40.0 + bt) * 0.04;
            float edgeX = (s - abs(q.x)) / s;
            float edgeY = (s * aR - abs(q.y)) / (s * aR);
            float edgeMask = smoothstep(0.0, 0.06,
                                       min(edgeX, edgeY) - jag);

            prev = mix(prev, patch, edgeMask);
        }

        gl_FragColor = vec4(prev, 1.0);
        return;
    }

    // ============= PASS 1 — output ============================================

    // Chromatic slide — RGB channels pulled in different directions; the
    // amount drifts so it never settles. Treble pushes harder.
    float cs = chromaSlide * (1.0 + audioHigh * audioReact * 1.5);
    vec2 dirR = vec2(sin(TIME * 0.7),  cos(TIME * 0.91));
    vec2 dirB = vec2(cos(TIME * 0.83), -sin(TIME * 1.07));
    float r = texture(collageBuf, uv + dirR * cs).r;
    float g = texture(collageBuf, uv).g;
    float b = texture(collageBuf, uv + dirB * cs).b;
    vec3 col = vec3(r, g, b);

    // Halftone overlay — luma-modulated dot grid so highlights have small
    // dots and shadows have big dots.
    if (halftoneAmount > 0.0) {
        float L = dot(col, vec3(0.299, 0.587, 0.114));
        vec2 hg = vec2(uv.x * aspect, uv.y) * halftoneScale;
        // Rotate the dot grid 15° (newspaper convention)
        float c15 = cos(0.262), s15 = sin(0.262);
        hg = mat2(c15, -s15, s15, c15) * hg;
        vec2 hf = fract(hg) - 0.5;
        float dotR = (1.0 - L) * 0.45;
        float dotMask = step(length(hf), dotR);
        vec3 ht = mix(vec3(0.93, 0.90, 0.82), vec3(0.10), dotMask);
        col = mix(col, ht, halftoneAmount);
    }

    // Red ink stamps — circles, crosses at hashed positions.
    if (stampDensity > 0.0) {
        for (int g_ = 0; g_ < 6; g_++) {
            // Stamps slide between beats so they never freeze.
            vec2 c = vec2(hash11(float(g_) * 13.7 + bt * 0.71),
                          hash11(float(g_) * 17.1 + bt * 0.29))
                   + 0.05 * vec2(sin(TIME * 0.4 + float(g_)),
                                 cos(TIME * 0.6 + float(g_)));
            vec2 d = uv - c;
            d.x *= aspect;
            float rr = 0.025 + hash11(float(g_) * 3.7) * 0.025;
            int kind = int(hash11(float(g_) * 9.7) * 3.0);
            float m = 0.0;
            if (kind == 0) {
                float l = length(d);
                m = smoothstep(rr + 0.003, rr, l)
                  - smoothstep(rr - 0.003, rr - 0.006, l);
            } else if (kind == 1) {
                m = max(step(abs(d.x), rr) * step(abs(d.y), 0.003),
                        step(abs(d.y), rr) * step(abs(d.x), 0.003));
            } else {
                m = step(abs(d.x), rr * 0.6) * step(abs(d.y), rr)
                  * step(0.5, hash21(floor(d * 80.0)));
            }
            col = mix(col, vec3(0.78, 0.10, 0.10),
                      m * stampDensity
                       * (0.4 + audioHigh * audioReact * 0.8));
        }
    }

    // Stuttering letter row — a band of glitching letterforms across the
    // canvas, position resets every beat.
    if (letterStutter > 0.0) {
        // Continuous scroll instead of teleport-per-beat.
        float bandY = fract(TIME * 0.05 + sin(bt * 0.71) * 0.2);
        float dy = abs(uv.y - bandY);
        if (dy < 0.04) {
            vec2 ld = vec2(uv.x * 80.0,
                           (uv.y - bandY + 0.04) * 60.0);
            vec2 ci = floor(ld);
            float h = hash11(ci.x * 1.7 + ci.y * 13.0 + bt);
            float vert = step(h, 0.55) * step(0.18, fract(ld.x))
                       * step(fract(ld.x), 0.55)
                       * step(0.10, fract(ld.y))
                       * step(fract(ld.y), 0.92);
            float bar  = step(0.55, h) * step(h, 0.85)
                       * step(0.40, fract(ld.y))
                       * step(fract(ld.y), 0.62);
            float letterMask = max(vert, bar)
                             * smoothstep(0.04, 0.0, dy);
            col = mix(col, vec3(0.05),
                      letterMask * letterStutter
                       * (0.6 + audioMid * audioReact * 0.8));
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
