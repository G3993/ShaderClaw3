/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Color-field after Rothko's Orange Red Yellow (1961) and the Chapel paintings (1971) — stacked Gaussian-blurred rectangles floating on a coloured ground, edges deeply feathered, very slow shimmer. Meditative, never crisp. The breath is gentle even when audio is loud — the painting refuses to be hurried.",
  "INPUTS": [
    { "NAME": "rothkoWork", "LABEL": "Painting", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Orange Red Yellow (1961)", "No.61 Rust+Blue (1953)", "White Center (1950)", "Seagram Maroon (1958)", "Black on Maroon (1959)"] },
    { "NAME": "bandCount", "LABEL": "Bands", "TYPE": "float", "MIN": 2.0, "MAX": 4.0, "DEFAULT": 3.0 },
    { "NAME": "feather", "LABEL": "Feather", "TYPE": "float", "MIN": 0.04, "MAX": 0.30, "DEFAULT": 0.16 },
    { "NAME": "innerInset", "LABEL": "Rectangle Inset", "TYPE": "float", "MIN": 0.0, "MAX": 0.18, "DEFAULT": 0.06 },
    { "NAME": "groundColor", "LABEL": "Ground Color", "TYPE": "color", "DEFAULT": [0.32, 0.10, 0.10, 1.0] },
    { "NAME": "topColor", "LABEL": "Top Band", "TYPE": "color", "DEFAULT": [0.92, 0.50, 0.22, 1.0] },
    { "NAME": "midColor", "LABEL": "Middle Band", "TYPE": "color", "DEFAULT": [0.85, 0.20, 0.14, 1.0] },
    { "NAME": "botColor", "LABEL": "Bottom Band", "TYPE": "color", "DEFAULT": [0.95, 0.78, 0.30, 1.0] },
    { "NAME": "shimmer", "LABEL": "Shimmer", "TYPE": "float", "MIN": 0.0, "MAX": 0.12, "DEFAULT": 0.04 },
    { "NAME": "shimmerSpeed", "LABEL": "Shimmer Speed", "TYPE": "float", "MIN": 0.0, "MAX": 0.2, "DEFAULT": 0.04 },
    { "NAME": "bandBleed",   "LABEL": "Band Bleed",      "TYPE": "float", "MIN": 0.0, "MAX": 0.50, "DEFAULT": 0.18 },
    { "NAME": "groundMix",   "LABEL": "Ground Mix",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.20 },
    { "NAME": "colorBreath", "LABEL": "Color Breath",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.30 },
    { "NAME": "breathSpeed", "LABEL": "Breath Speed",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.10 },
    { "NAME": "rotation",    "LABEL": "Rotation",        "TYPE": "float", "MIN": -0.5,"MAX": 0.5,  "DEFAULT": 0.0 },
    { "NAME": "vignette", "LABEL": "Vignette", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.22 },
    { "NAME": "grain", "LABEL": "Film Grain", "TYPE": "float", "MIN": 0.0, "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "audioInfluence", "LABEL": "Audio Influence (capped)", "TYPE": "float", "MIN": 0.0, "MAX": 0.10, "DEFAULT": 0.04 },
    { "NAME": "useTex", "LABEL": "Sample Tex for Bands", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

// Rothko's chapel surfaces look luminous because the bands are stacked
// over a chromatic GROUND (not white) and their edges are *much* more
// feathered than they look — the eye reads the soft transition as glow.
// We model each band as a smoothstep-bounded vertical slab with a small
// horizontal inset, low-amplitude noise breathing it, and a very slow
// shimmer ripple. Audio influence is hard-capped because Rothko's whole
// argument is patience.

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

float bandShape(vec2 uv, float yLo, float yHi, float xInset, float feath) {
    // Slow horizontal undulation on band edges — Rothko's edges aren't
    // axis-aligned; they have organic micro-wobble.
    float edgeULo = 0.005 * sin(uv.x * 8.0 + TIME * 0.03);
    float edgeUHi = 0.005 * cos(uv.x * 7.0 + TIME * 0.04);
    yLo += edgeULo;
    yHi += edgeUHi;
    float yMask = smoothstep(yLo - feath, yLo + feath, uv.y)
                * (1.0 - smoothstep(yHi - feath, yHi + feath, uv.y));
    float xMask = smoothstep(xInset, xInset + feath * 0.6, uv.x)
                * (1.0 - smoothstep(1.0 - xInset - feath * 0.6,
                                    1.0 - xInset, uv.x));
    // Painterly inner glow — band centres slightly brighter than edges
    // so each rectangle reads as having internal radiance, not flat fill.
    // The chapel-painting "light from within" Rothko quality.
    float bandCenterY  = (yLo + yHi) * 0.5;
    float dyFromCenter = abs(uv.y - bandCenterY)
                       / max((yHi - yLo) * 0.5, 1e-4);
    yMask *= 1.0 + 0.08 * (1.0 - dyFromCenter * dyFromCenter);
    return yMask * xMask;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Ground — chromatic, never neutral. Subtle vertical gradient so it
    // doesn't read as flat digital fill.
    vec3 col = groundColor.rgb
             * (0.95 + 0.10 * smoothstep(0.0, 1.0, uv.y));

    int N = int(clamp(bandCount, 2.0, 4.0));

    // Per-painting band palette — five canonical Rothko colour worlds.
    // User colour pickers (topColor/midColor/botColor) override only
    // when rothkoWork == 0 (default), preserving manual control.
    vec3 cTop = topColor.rgb;
    vec3 cMid = midColor.rgb;
    vec3 cBot = botColor.rgb;
    int rw = int(rothkoWork);
    if      (rw == 1) { cTop = vec3(0.55, 0.18, 0.18); cMid = vec3(0.20, 0.22, 0.45); cBot = vec3(0.40, 0.16, 0.20); }
    else if (rw == 2) { cTop = vec3(0.92, 0.86, 0.78); cMid = vec3(0.85, 0.32, 0.20); cBot = vec3(0.55, 0.18, 0.40); }
    else if (rw == 3) { cTop = vec3(0.30, 0.05, 0.06); cMid = vec3(0.18, 0.04, 0.05); cBot = vec3(0.10, 0.03, 0.04); }
    else if (rw == 4) { cTop = vec3(0.05, 0.02, 0.03); cMid = vec3(0.28, 0.05, 0.06); cBot = vec3(0.10, 0.03, 0.04); }
    if (useTex && IMG_SIZE_inputTex.x > 0.0) {
        cTop = texture(inputTex, vec2(0.5, 0.85)).rgb;
        cMid = texture(inputTex, vec2(0.5, 0.50)).rgb;
        cBot = texture(inputTex, vec2(0.5, 0.15)).rgb;
    }

    // Color breath — slowly cross-fade band colors among themselves so
    // the painting is never literally still. Three independent phases.
    if (colorBreath > 0.001) {
        float bt = TIME * breathSpeed;
        vec3 bTop = mix(cTop, mix(cMid, cBot, 0.5), 0.5 + 0.5 * sin(bt * 0.7));
        vec3 bMid = mix(cMid, cTop, 0.5 + 0.5 * sin(bt * 0.5 + 1.7));
        vec3 bBot = mix(cBot, cMid, 0.5 + 0.5 * sin(bt * 0.6 + 3.1));
        cTop = mix(cTop, bTop, colorBreath);
        cMid = mix(cMid, bMid, colorBreath);
        cBot = mix(cBot, bBot, colorBreath);
    }

    // Blend each band with ground for that subdued chapel feel
    cTop = mix(cTop, groundColor.rgb, groundMix);
    cMid = mix(cMid, groundColor.rgb, groundMix);
    cBot = mix(cBot, groundColor.rgb, groundMix);

    // Slow rotation — a gentle drift keyed off rotation slider
    if (rotation != 0.0) {
        vec2 c = uv - 0.5;
        float a = TIME * rotation * 0.05;
        float ca = cos(a), sa = sin(a);
        uv = 0.5 + vec2(ca * c.x - sa * c.y, sa * c.x + ca * c.y);
    }

    // Band layout — top band tallest, middle smaller, bottom small.
    // Insets give the floating-rectangle feel. Feather is large.
    float xIn = clamp(innerInset, 0.0, 0.4);
    float fth = feather;

    // bandBleed widens the feather for cross-band color bleed
    float fthB = fth * (1.0 + bandBleed * 4.0);

    if (N >= 3) {
        // 3-band Orange/Red/Yellow layout
        float t1 = bandShape(uv, 0.62, 0.92, xIn, fthB);
        float t2 = bandShape(uv, 0.34, 0.58, xIn, fthB);
        float t3 = bandShape(uv, 0.08, 0.30, xIn, fthB);
        col = mix(col, cTop, t1);
        col = mix(col, cMid, t2);
        col = mix(col, cBot, t3);
    } else {
        // 2-band layout
        float t1 = bandShape(uv, 0.55, 0.92, xIn, fthB);
        float t2 = bandShape(uv, 0.10, 0.46, xIn, fthB);
        col = mix(col, cTop, t1);
        col = mix(col, cBot, t2);
    }

    // Slow noise breath — symmetric around 1.0 so bands brighten and
    // darken equally rather than always trending toward darker.
    float n = vnoise(uv * 2.6 + TIME * shimmerSpeed)
            + 0.5 * vnoise(uv * 5.3 - TIME * shimmerSpeed * 0.7);
    col *= 1.0 + (n - 0.5) * shimmer;

    // Vignette — corners darker, the painting is centred.
    float vig = pow(length(uv - 0.5) * 1.4, 3.0) * vignette;
    col *= 1.0 - vig;

    // Film grain so the surface doesn't read as digital.
    col += (hash21(uv * RENDERSIZE) - 0.5) * grain;

    // Audio capped low — bass nudges luminance ≤ audioInfluence.
    col *= 1.0 + audioBass * audioInfluence * 0.7
              + audioLevel * audioInfluence;

    // Surprise: once every ~47 seconds, a single horizontal "memory line"
    // ghosts across the canvas — the trace of a previous painting on the
    // same canvas. Visible for ~3 seconds, very faint.
    float gPhase = fract(TIME / 47.0);
    float gFade = smoothstep(0.0, 0.05, gPhase) * smoothstep(0.20, 0.10, gPhase);
    float gY = 0.30 + 0.40 * hash21(vec2(floor(TIME / 47.0), 0.0));
    float gLine = exp(-pow((uv.y - gY) * 80.0, 2.0));
    col += vec3(0.20, 0.15, 0.10) * gLine * gFade * 0.25;

    gl_FragColor = vec4(col, 1.0);
}
