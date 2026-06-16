/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Dada — single-pass collage anti-art. Hannah Höch / Hausmann strip composition with a Magritte False-Mirror eye at center, sky-and-clouds visible through the iris and a hard black pupil, blinking every few seconds. Halftone print, scattered stamps, stuttering letterforms, paper tone. Drifting strips reposition continuously.",
  "INPUTS": [
    { "NAME": "stripCount",     "LABEL": "Strips",          "TYPE":"float","MIN":2.0, "MAX":24.0, "DEFAULT":12.0 },
    { "NAME": "stripScale",     "LABEL": "Strip Scale",     "TYPE":"float","MIN":0.05,"MAX":0.40, "DEFAULT":0.16 },
    { "NAME": "rotateRange",    "LABEL": "Rotation Range",  "TYPE":"float","MIN":0.0, "MAX":1.5,  "DEFAULT":0.65 },
    { "NAME": "drift",          "LABEL": "Drift",           "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.30 },
    { "NAME": "eyeShow",        "LABEL": "Magritte Eye",    "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":1.0 },
    { "NAME": "eyeBlinkRate",   "LABEL": "Blink Rate",      "TYPE":"float","MIN":2.0, "MAX":30.0, "DEFAULT":11.0 },
    { "NAME": "eyeSize",        "LABEL": "Eye Size",        "TYPE":"float","MIN":0.10,"MAX":0.45, "DEFAULT":0.28 },
    { "NAME": "cloudDrift",     "LABEL": "Cloud Drift",     "TYPE":"float","MIN":0.0, "MAX":2.0,  "DEFAULT":0.45 },
    { "NAME": "halftone",       "LABEL": "Halftone",        "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.55 },
    { "NAME": "halftoneScale",  "LABEL": "Halftone Scale",  "TYPE":"float","MIN":80.0,"MAX":400.0,"DEFAULT":220.0 },
    { "NAME": "stampDensity",   "LABEL": "Stamp Density",   "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.50 },
    { "NAME": "letterStutter",  "LABEL": "Letter Stutter",  "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.50 },
    { "NAME": "chromaSlide",    "LABEL": "Chroma Slide",    "TYPE":"float","MIN":0.0, "MAX":0.04, "DEFAULT":0.012 },
    { "NAME": "audioReact",     "LABEL": "Audio React",     "TYPE":"float","MIN":0.0, "MAX":2.0,  "DEFAULT":1.0 },
    { "NAME": "paperTone",      "LABEL": "Paper",           "TYPE":"color","DEFAULT":[0.93, 0.88, 0.78, 1.0] },
    { "NAME": "inputTex",       "LABEL": "Texture",         "TYPE":"image" }
  ]
}*/

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) { v += vnoise(p) * a; p *= 2.07; a *= 0.5; }
    return v;
}

// ──────────────────────────────────────────────────────────────────────
// Magritte False-Mirror eye SDF
//   - eyelid envelope (almond)
//   - iris (circle) filled with sky-and-clouds
//   - black pupil
//   - blink: eyelid envelope shrinks vertically every blinkRate seconds
// ──────────────────────────────────────────────────────────────────────
struct EyeMask {
    float eyelid;   // <0 inside, >0 outside
    float iris;
    float pupil;
};

EyeMask magrittEye(vec2 uv, float aspect) {
    EyeMask m;
    vec2 c = vec2(0.5, 0.55);
    vec2 p = (uv - c) * vec2(aspect, 1.0) / max(eyeSize, 0.01);

    // Blink — the eyelid closes briefly, every ~blinkRate seconds.
    float blinkPhase = fract(TIME / max(eyeBlinkRate, 0.5));
    float blink = smoothstep(0.00, 0.04, blinkPhase) * smoothstep(0.10, 0.05, blinkPhase);
    // 1.0 = eye open, 0.05 = nearly closed
    float openness = mix(1.0, 0.05, blink);

    // Almond eyelid: |y| envelope tapered with x
    vec2 eP = p;
    eP.y /= openness;
    float xCurve = sqrt(max(0.0, 1.0 - eP.x * eP.x * 1.05));
    float topLid = eP.y - 0.40 * xCurve;
    float botLid = -eP.y - 0.40 * xCurve;
    m.eyelid = max(topLid, botLid);

    // Iris circle (slightly inset)
    m.iris = length(eP) - 0.32;

    // Pupil
    m.pupil = length(eP) - 0.08;
    return m;
}

vec3 skyWithClouds(vec2 uv) {
    // The famous False-Mirror sky: ultramarine lower, paler upper, with
    // soft cumulus drifting horizontally.
    vec3 lo = vec3(0.45, 0.65, 0.92);
    vec3 hi = vec3(0.78, 0.88, 0.98);
    vec3 sky = mix(lo, hi, smoothstep(0.0, 1.0, uv.y));
    float cd = TIME * cloudDrift * 0.04;
    float c1 = fbm(uv * 4.0 + vec2(cd, 0.0));
    float c2 = fbm(uv * 2.0 + vec2(cd * 0.7, 0.3));
    float clouds = smoothstep(0.55, 0.75, c1 * 0.6 + c2 * 0.4);
    sky = mix(sky, vec3(0.99, 0.98, 0.95), clouds * 0.85);
    return sky;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    float t = TIME;

    // Paper background
    vec3 col = paperTone.rgb;
    // Paper grain
    col *= 0.94 + 0.06 * vnoise(uv * RENDERSIZE.x * 0.4);

    // ── Drifting collage strips ───────────────────────────────────────
    int N = int(clamp(stripCount, 0.0, 24.0));
    for (int i = 0; i < 24; i++) {
        if (i >= N) break;
        float fi = float(i);
        // Position drifts slowly, angle wobbles
        vec2 home = vec2(hash11(fi * 7.13), hash11(fi * 11.7));
        vec2 wobble = vec2(sin(t * drift + fi * 1.3), cos(t * drift * 0.8 + fi * 1.7)) * 0.04;
        vec2 ctr = home + wobble;

        float angle = (hash11(fi * 17.9) - 0.5) * rotateRange + sin(t * 0.1 + fi) * 0.05;
        float ca = cos(angle), sa = sin(angle);
        vec2 d = uv - ctr;
        d.x *= aspect;
        vec2 lp = mat2(ca, -sa, sa, ca) * d;

        // Strip half-extents — wide and thin
        vec2 hs = vec2(stripScale * (0.6 + hash11(fi * 23.1) * 0.9),
                       stripScale * (0.10 + hash11(fi * 29.3) * 0.20));
        if (abs(lp.x) > hs.x || abs(lp.y) > hs.y) continue;

        // Strip content: either input texture sample (with chroma slide)
        // or procedural pattern (newspaper-print stripes, hashes, blots).
        vec3 stripCol;
        if (IMG_SIZE_inputTex.x > 0.0) {
            vec2 sUV = vec2((lp.x / hs.x) * 0.5 + 0.5, (lp.y / hs.y) * 0.5 + 0.5);
            float ch = chromaSlide;
            float r = texture(inputTex, sUV + vec2( ch, 0.0)).r;
            float g = texture(inputTex, sUV).g;
            float b = texture(inputTex, sUV - vec2( ch, 0.0)).b;
            stripCol = vec3(r, g, b);
        } else {
            // Procedural newsprint — alternating dark/light bands plus noise
            float band = step(0.5, fract(lp.x * 14.0 + fi));
            stripCol = mix(vec3(0.20, 0.18, 0.16), vec3(0.85, 0.82, 0.74), band);
            stripCol *= 0.8 + 0.2 * vnoise(lp * 80.0);
        }

        col = stripCol;

        // Black ink edge
        float edge = max(abs(lp.x) - hs.x, abs(lp.y) - hs.y);
        col = mix(col, vec3(0.05), smoothstep(0.0, -0.005, edge) * 0.0
                                  + smoothstep(-0.005, -0.001, edge) * 0.6);
    }

    // ── Magritte False-Mirror eye over the collage ────────────────────
    if (eyeShow > 0.001) {
        EyeMask em = magrittEye(uv, aspect);
        // Iris filled with sky-and-clouds (the False Mirror)
        if (em.iris < 0.0 && em.eyelid < 0.0) {
            // Local UV inside the iris for the sky
            vec2 c = vec2(0.5, 0.55);
            vec2 p = (uv - c) * vec2(aspect, 1.0) / max(eyeSize, 0.01);
            vec2 skyUV = p * 0.5 + 0.5;
            vec3 sky = skyWithClouds(skyUV);
            col = mix(col, sky, eyeShow);
        }
        // Pupil — solid black
        if (em.pupil < 0.0 && em.eyelid < 0.0) {
            col = mix(col, vec3(0.02), eyeShow);
        }
        // Eyelid skin around the eye — warm flesh tone
        if (em.eyelid < 0.0 && em.iris > 0.0) {
            // Outside iris but inside eyelid → eyelid skin band
            // Realistic skin: warm tan with subtle gradient
            float depth = clamp(-em.eyelid * 6.0, 0.0, 1.0);
            vec3 skin = mix(vec3(0.78, 0.55, 0.35), vec3(0.95, 0.78, 0.58), depth);
            col = mix(col, skin, eyeShow * 0.95);
        }
        // Eyelid contour line
        col = mix(col, vec3(0.20, 0.10, 0.06),
                  smoothstep(0.012, 0.0, abs(em.eyelid)) * eyeShow * 0.8);
        // Iris contour line
        if (em.eyelid < -0.005) {
            col = mix(col, vec3(0.05),
                      smoothstep(0.008, 0.0, abs(em.iris)) * eyeShow * 0.6);
        }
    }

    // ── Stamps (red ink rubber-stamp blots) ───────────────────────────
    if (stampDensity > 0.001) {
        for (int s = 0; s < 6; s++) {
            float fs = float(s);
            vec2 sp = vec2(hash11(fs * 31.7), hash11(fs * 37.1));
            sp += vec2(sin(t * 0.15 + fs), cos(t * 0.18 + fs)) * 0.02;
            float r = 0.04 + hash11(fs * 41.3) * 0.06;
            float d = length(uv - sp) - r;
            float blot = smoothstep(0.005, -0.005, d);
            // Inner ring (rubber stamp circle)
            float ring = smoothstep(0.005, 0.0, abs(d - 0.005));
            col = mix(col, vec3(0.78, 0.10, 0.08), max(blot * 0.4, ring) * stampDensity);
        }
    }

    // ── Stuttering letterforms ────────────────────────────────────────
    if (letterStutter > 0.001) {
        float steps = 8.0;
        vec2 g = floor(uv * steps);
        float h = hash21(g + floor(t * 4.0));
        float h2 = hash21(g + floor(t * 4.0) * 7.13);
        if (h > 0.86) {
            float ink = step(0.45, fract((uv.x + uv.y) * 35.0 + h2 * 6.28));
            col = mix(col, vec3(0.05), ink * letterStutter * 0.6);
        }
    }

    // ── Halftone print effect (newspaper dots) ────────────────────────
    if (halftone > 0.001) {
        float L = dot(col, vec3(0.299, 0.587, 0.114));
        vec2 hp = uv * halftoneScale;
        vec2 hi = floor(hp), hf = fract(hp);
        float dotR = sqrt(1.0 - L) * 0.5;
        float dotMask = smoothstep(dotR + 0.05, dotR - 0.05, length(hf - 0.5));
        col = mix(col, vec3(0.05), dotMask * halftone * 0.40);
    }

    // Surprise: anti-art glitch — every ~17s the entire canvas inverts
    // for one frame (~0.05s).
    {
        float _ph = fract(TIME / 17.0);
        float _flash = step(_ph, 0.04);
        col = mix(col, 1.0 - col, _flash);
    }

    // Audio breath
    col *= 0.95 + audioLevel * audioReact * 0.10;

    gl_FragColor = vec4(col, 1.0);
}
