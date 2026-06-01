/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Rothko color-field — luminous stacked bands that breathe, melt, and bleed into one another like pigment dissolving in warm light. Mood-first, movement-second.",
  "INPUTS": [
    { "NAME": "rothkoWork",    "LABEL": "Painting",          "TYPE": "long",  "DEFAULT": 0, "VALUES": [0,1,2,3,4], "LABELS": ["Orange Red Yellow","No.61 Rust+Blue","White Center","Seagram Maroon","Black on Maroon"] },
    { "NAME": "topColor",      "LABEL": "Top Color",         "TYPE": "color", "DEFAULT": [0.92,0.50,0.22,1.0] },
    { "NAME": "midColor",      "LABEL": "Mid Color",         "TYPE": "color", "DEFAULT": [0.85,0.20,0.14,1.0] },
    { "NAME": "botColor",      "LABEL": "Bot Color",         "TYPE": "color", "DEFAULT": [0.95,0.78,0.30,1.0] },
    { "NAME": "groundColor",   "LABEL": "Ground",            "TYPE": "color", "DEFAULT": [0.28,0.08,0.08,1.0] },
    { "NAME": "breathSpeed",   "LABEL": "Breath Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 0.6,  "DEFAULT": 0.09 },
    { "NAME": "meltDepth",     "LABEL": "Melt / Bleed",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.72 },
    { "NAME": "feather",       "LABEL": "Edge Feather",      "TYPE": "float", "MIN": 0.05, "MAX": 0.55, "DEFAULT": 0.28 },
    { "NAME": "innerInset",    "LABEL": "Rectangle Inset",   "TYPE": "float", "MIN": 0.0,  "MAX": 0.18, "DEFAULT": 0.05 },
    { "NAME": "bandCount",     "LABEL": "Bands",             "TYPE": "float", "MIN": 2.0,  "MAX": 4.0,  "DEFAULT": 3.0 },
    { "NAME": "waveAmount",    "LABEL": "Edge Waviness",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.04, "DEFAULT": 0.010 },
    { "NAME": "shimmer",       "LABEL": "Surface Shimmer",   "TYPE": "float", "MIN": 0.0,  "MAX": 0.18, "DEFAULT": 0.05 },
    { "NAME": "shimmerScale",  "LABEL": "Shimmer Scale",     "TYPE": "float", "MIN": 0.5,  "MAX": 8.0,  "DEFAULT": 2.2 },
    { "NAME": "paintTexture",  "LABEL": "Paint Texture",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.38 },
    { "NAME": "textureScale",  "LABEL": "Texture Scale",     "TYPE": "float", "MIN": 1.0,  "MAX": 16.0, "DEFAULT": 4.5 },
    { "NAME": "chrShimmer",    "LABEL": "Chromatic Edge",    "TYPE": "float", "MIN": 0.0,  "MAX": 0.025,"DEFAULT": 0.008 },
    { "NAME": "vignette",      "LABEL": "Vignette",          "TYPE": "float", "MIN": 0.0,  "MAX": 0.8,  "DEFAULT": 0.30 },
    { "NAME": "grain",         "LABEL": "Film Grain",        "TYPE": "float", "MIN": 0.0,  "MAX": 0.05, "DEFAULT": 0.014 },
    { "NAME": "audioInfluence","LABEL": "Audio Influence",   "TYPE": "float", "MIN": 0.0,  "MAX": 0.12, "DEFAULT": 0.04 },
    { "NAME": "useTex",        "LABEL": "Sample Texture",    "TYPE": "bool",  "DEFAULT": false },
    { "NAME": "inputTex",      "LABEL": "Texture",           "TYPE": "image" }
  ]
}*/

// ── low-level noise ───────────────────────────────────────────────────────────

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float vnoise(vec2 p) {
    vec2 ip = floor(p);
    vec2 fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

// 5-octave fBm — used for paint texture & melt fields
float fbm(vec2 p) {
    float v = 0.0, a = 0.52;
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p  = p * 2.03 + vec2(3.7, 1.9);
        a *= 0.5;
    }
    return v;
}

// Smooth colour ramp across three anchors
vec3 triGrad(float t, vec3 cA, vec3 cB, vec3 cC) {
    t = clamp(t, 0.0, 1.0);
    return (t < 0.5) ? mix(cA, cB, t * 2.0) : mix(cB, cC, (t - 0.5) * 2.0);
}

// ── band mask (feathered rect with wavy edges) ────────────────────────────────

float bandMask(vec2 uv, float yLo, float yHi, float xIn, float fth, float wave) {
    float wx = wave * sin(uv.x * 11.0 + TIME * 0.07);
    float wy = wave * cos(uv.x *  7.3 - TIME * 0.05);
    float ym = smoothstep(yLo - fth + wx, yLo + fth + wx, uv.y)
             * (1.0 - smoothstep(yHi - fth + wy, yHi + fth + wy, uv.y));
    float xm = smoothstep(xIn, xIn + fth * 0.5, uv.x)
             * (1.0 - smoothstep(1.0 - xIn - fth * 0.5, 1.0 - xIn, uv.x));
    return ym * xm;
}

// ── main ──────────────────────────────────────────────────────────────────────

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // ── resolve palette ───────────────────────────────────────────────────────
    int rw = int(rothkoWork);

    vec3 cTop = topColor.rgb;
    vec3 cMid = midColor.rgb;
    vec3 cBot = botColor.rgb;
    vec3 cGnd = groundColor.rgb;

    if      (rw == 1) { cTop = vec3(0.52,0.16,0.16); cMid = vec3(0.18,0.20,0.44); cBot = vec3(0.38,0.14,0.20); cGnd = vec3(0.06,0.04,0.12); }
    else if (rw == 2) { cTop = vec3(0.93,0.87,0.79); cMid = vec3(0.84,0.30,0.18); cBot = vec3(0.52,0.16,0.38); cGnd = vec3(0.18,0.09,0.14); }
    else if (rw == 3) { cTop = vec3(0.28,0.04,0.05); cMid = vec3(0.16,0.03,0.04); cBot = vec3(0.08,0.02,0.03); cGnd = vec3(0.05,0.01,0.02); }
    else if (rw == 4) { cTop = vec3(0.04,0.01,0.02); cMid = vec3(0.26,0.04,0.05); cBot = vec3(0.08,0.02,0.03); cGnd = vec3(0.02,0.01,0.01); }

    if (useTex) {
        cTop = IMG_NORM_PIXEL(inputTex, vec2(0.5, 0.85)).rgb;
        cMid = IMG_NORM_PIXEL(inputTex, vec2(0.5, 0.50)).rgb;
        cBot = IMG_NORM_PIXEL(inputTex, vec2(0.5, 0.15)).rgb;
    }

    // ── breathing / melting ───────────────────────────────────────────────────
    // Three phase-offset sinusoids drive slow cross-colour cycling.
    // Each band inhales from its neighbour, exhales back — never mechanical.
    float bt = TIME * breathSpeed;

    float ph0 = sin(bt * 0.53)              * 0.5 + 0.5;
    float ph1 = sin(bt * 0.41 + 1.8)        * 0.5 + 0.5;
    float ph2 = sin(bt * 0.37 + 3.5)        * 0.5 + 0.5;
    float ph3 = sin(bt * 0.29 + 5.1)        * 0.5 + 0.5;
    float ph4 = sin(bt * 0.61 + 2.4)        * 0.5 + 0.5;
    float ph5 = sin(bt * 0.47 + 4.7)        * 0.5 + 0.5;

    // Each colour breathes toward its neighbour band *and* toward ground
    vec3 bTop = mix(cTop, mix(cMid, cGnd, 0.25), ph0);
    vec3 bMid = mix(cMid, mix(cBot, cTop, ph1),  ph2);
    vec3 bBot = mix(cBot, mix(cTop, cGnd, 0.15), ph3);

    cTop = mix(cTop, bTop, meltDepth);
    cMid = mix(cMid, bMid, meltDepth);
    cBot = mix(cBot, bBot, meltDepth);

    // Secondary slow "tide" — overall hue drift all three bands together
    float tide = sin(bt * 0.19) * 0.5 + 0.5;
    cTop = mix(cTop, mix(cTop, cBot, 0.30), tide * meltDepth * 0.5);
    cMid = mix(cMid, mix(cMid, cTop, 0.30), (1.0 - tide) * meltDepth * 0.5);
    cBot = mix(cBot, mix(cBot, cMid, 0.30), ph4 * meltDepth * 0.5);

    // ── ground with vertical luminosity gradient ──────────────────────────────
    // Ground subtly transitions from the base tone to a slightly warmer top.
    vec3 col = mix(cGnd * 0.82, cGnd * 1.08, uv.y);

    // Soft large-scale noise wash that bleeds ground into bands
    float gBleed = fbm(uv * 1.4 + vec2(bt * 0.07, bt * 0.04));
    col = mix(col, mix(cGnd, cMid, 0.35), gBleed * meltDepth * 0.28);

    // ── parameters ───────────────────────────────────────────────────────────
    int N    = int(clamp(bandCount, 2.0, 4.0));
    float xI = clamp(innerInset, 0.0, 0.38);
    // Feather expands with meltDepth for more painterly bleed
    float fth = feather * (1.0 + meltDepth * 1.8);
    float wav = waveAmount;

    // Slow vertical drift of band positions — bands float up/down gently
    float drift = sin(bt * 0.17) * 0.025 * meltDepth;

    // ── chromatic edge offset ─────────────────────────────────────────────────
    float cOff = chrShimmer * (0.6 + 0.4 * sin(TIME * 0.41));

    // ── band painting ─────────────────────────────────────────────────────────
    // Bands are blended per-channel with a tiny chromatic offset for spectral
    // fringing at the edges — one of Rothko's signature optical effects.

    if (N >= 3) {
        // Top band
        float y1L = 0.60 + drift;
        float y1H = 0.92 + drift * 0.5;
        float mR1 = bandMask(uv + vec2(cOff, 0.0), y1L, y1H, xI, fth, wav);
        float mG1 = bandMask(uv,                   y1L, y1H, xI, fth, wav);
        float mB1 = bandMask(uv - vec2(cOff, 0.0), y1L, y1H, xI, fth, wav);

        // Mid band
        float y2L = 0.32 - drift;
        float y2H = 0.58 - drift * 0.5;
        float mR2 = bandMask(uv + vec2(cOff, 0.0), y2L, y2H, xI, fth, wav);
        float mG2 = bandMask(uv,                   y2L, y2H, xI, fth, wav);
        float mB2 = bandMask(uv - vec2(cOff, 0.0), y2L, y2H, xI, fth, wav);

        // Bot band
        float y3L = 0.06 + drift * 0.3;
        float y3H = 0.28 + drift * 0.3;
        float mR3 = bandMask(uv + vec2(cOff, 0.0), y3L, y3H, xI, fth, wav);
        float mG3 = bandMask(uv,                   y3L, y3H, xI, fth, wav);
        float mB3 = bandMask(uv - vec2(cOff, 0.0), y3L, y3H, xI, fth, wav);

        col.r = mix(col.r, cTop.r, mR1); col.g = mix(col.g, cTop.g, mG1); col.b = mix(col.b, cTop.b, mB1);
        col.r = mix(col.r, cMid.r, mR2); col.g = mix(col.g, cMid.g, mG2); col.b = mix(col.b, cMid.b, mB2);
        col.r = mix(col.r, cBot.r, mR3); col.g = mix(col.g, cBot.g, mG3); col.b = mix(col.b, cBot.b, mB3);

    } else {
        float y1L = 0.53 + drift;
        float y1H = 0.92 + drift * 0.3;
        float mR1 = bandMask(uv + vec2(cOff, 0.0), y1L, y1H, xI, fth, wav);
        float mG1 = bandMask(uv,                   y1L, y1H, xI, fth, wav);
        float mB1 = bandMask(uv - vec2(cOff, 0.0), y1L, y1H, xI, fth, wav);

        float y2L = 0.08 - drift;
        float y2H = 0.46 - drift * 0.3;
        float mR2 = bandMask(uv + vec2(cOff, 0.0), y2L, y2H, xI, fth, wav);
        float mG2 = bandMask(uv,                   y2L, y2H, xI, fth, wav);
        float mB2 = bandMask(uv - vec2(cOff, 0.0), y2L, y2H, xI, fth, wav);

        col.r = mix(col.r, cTop.r, mR1); col.g = mix(col.g, cTop.g, mG1); col.b = mix(col.b, cTop.b, mB1);
        col.r = mix(col.r, cBot.r, mR2); col.g = mix(col.g, cBot.g, mG2); col.b = mix(col.b, cBot.b, mB2);
    }

    // ── paint texture ─────────────────────────────────────────────────────────
    // Two fBm layers at different scales and drift speeds create the sense of
    // impasto — thick paint with micro-ridges catching light differently.
    if (paintTexture > 0.001) {
        float tScale = textureScale;
        float tT     = TIME * breathSpeed * 0.12;

        float tex1 = fbm(uv * tScale            + vec2(tT * 0.23,  tT * 0.17));
        float tex2 = fbm(uv * tScale * 1.8      + vec2(-tT * 0.19, tT * 0.31) + vec2(4.1, 2.3));
        float tex3 = fbm(uv * tScale * 0.45     + vec2(tT * 0.11, -tT * 0.14) + vec2(1.7, 6.1));

        float tex  = tex1 * 0.55 + tex2 * 0.28 + tex3 * 0.17;

        // Modulate brightness — brighter at peaks, slightly darker in troughs
        float modulate = 1.0 + (tex - 0.48) * paintTexture * 1.6;
        col = clamp(col * modulate, 0.0, 1.0);

        // Colour-tinted lift at luminous peaks (warm/cool depending on phase)
        float tintPhase = sin(bt * 0.31) * 0.5 + 0.5;
        vec3  tint      = mix(cTop * 1.1, cBot * 0.9, tintPhase);
        float lift      = max(0.0, tex - 0.60) * paintTexture * 0.22;
        col = clamp(col + tint * lift, 0.0, 1.0);
    }

    // ── surface shimmer ───────────────────────────────────────────────────────
    // A two-octave noise rides on top as a fine luminous tremor.
    if (shimmer > 0.001) {
        float sc = shimmerScale;
        float n1 = vnoise(uv * sc           + vec2(TIME * breathSpeed * 0.55, TIME * breathSpeed * 0.40));
        float n2 = vnoise(uv * sc * 2.1     - vec2(TIME * breathSpeed * 0.42, TIME * breathSpeed * 0.31));
        float shm = (n1 * 0.65 + n2 * 0.35);
        col *= 1.0 + (shm - 0.5) * shimmer * 2.0;
    }

    // ── inner luminous centre glow ────────────────────────────────────────────
    // Each Rothko painting has a warm inner luminosity — slightly brighter
    // near the vertical centre of the canvas, as if lit from within.
    float centreGlow = exp(-pow((uv.x - 0.5) * 2.8, 2.0))
                     * exp(-pow((uv.y - 0.5) * 1.4, 2.0));
    centreGlow *= 0.18 * (0.7 + 0.3 * sin(bt * 0.23));
    col += col * centreGlow;

    // ── vignette ──────────────────────────────────────────────────────────────
    float d   = length((uv - 0.5) * vec2(1.0, 1.15));
    float vig = pow(d * 1.35, 3.0) * vignette;
    col *= 1.0 - vig;

    // ── film grain ────────────────────────────────────────────────────────────
    float g = hash21(uv * RENDERSIZE + vec2(float(FRAMEINDEX) * 0.317, 0.0));
    col += (g - 0.5) * grain;

    // ── audio ─────────────────────────────────────────────────────────────────
    float audioMod = 1.0 + audioBass  * audioInfluence * 0.80
                        + audioMid   * audioInfluence * 0.40
                        + audioLevel * audioInfluence * 0.30;
    col *= audioMod;

    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}