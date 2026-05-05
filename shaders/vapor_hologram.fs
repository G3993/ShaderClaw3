/*{
  "DESCRIPTION": "Vaporwave Hologram — Synthwave Night Edition. Deep violet sky, neon teal grid, electric cyan sun, Y2K shapes in fully-saturated HDR. Pass 0 renders the night vaporwave scene. Pass 1 layers hologram glitch on top.",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "CREDIT": "ShaderClaw auto-improve — Synthwave Night Edition",
  "INPUTS": [
    { "NAME": "horizonY",         "LABEL": "Horizon",         "TYPE": "float", "MIN": 0.40, "MAX": 0.75, "DEFAULT": 0.55 },
    { "NAME": "skyTopColor",      "LABEL": "Sky Top",         "TYPE": "color", "DEFAULT": [0.05, 0.0, 0.25, 1.0] },
    { "NAME": "skyHorizonColor",  "LABEL": "Sky Horizon",     "TYPE": "color", "DEFAULT": [0.0, 0.5, 0.5, 1.0] },
    { "NAME": "sunSize",          "LABEL": "Sun Size",        "TYPE": "float", "MIN": 0.05, "MAX": 0.40, "DEFAULT": 0.22 },
    { "NAME": "sunBars",          "LABEL": "Sun Bars",        "TYPE": "float", "MIN": 0.0,  "MAX": 12.0, "DEFAULT": 6.0 },
    { "NAME": "gridDensity",      "LABEL": "Grid Density",    "TYPE": "float", "MIN": 4.0,  "MAX": 24.0, "DEFAULT": 12.0 },
    { "NAME": "gridPersp",        "LABEL": "Grid Perspective","TYPE": "float", "MIN": 0.5,  "MAX": 4.0,  "DEFAULT": 1.8 },
    { "NAME": "gridSpeed",        "LABEL": "Grid Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25 },
    { "NAME": "y2kCount",         "LABEL": "Y2K Object Count","TYPE": "float", "MIN": 0.0,  "MAX": 20.0, "DEFAULT": 12.0 },
    { "NAME": "y2kSpeed",         "LABEL": "Y2K Speed",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6 },
    { "NAME": "y2kSize",          "LABEL": "Y2K Size",        "TYPE": "float", "MIN": 0.02, "MAX": 0.20, "DEFAULT": 0.07 },
    { "NAME": "y2kChaos",         "LABEL": "Chaos",           "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.7 },
    { "NAME": "katakanaIntensity","LABEL": "Katakana",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6 },
    { "NAME": "vaporPosterize",   "LABEL": "Vapor Posterize", "TYPE": "float", "MIN": 1.0,  "MAX": 32.0, "DEFAULT": 16.0 },
    { "NAME": "holoChroma",       "LABEL": "Holo Chroma",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "holoScanFreq",     "LABEL": "Holo Scanlines",  "TYPE": "float", "MIN": 1.0,  "MAX": 4.0,  "DEFAULT": 2.0 },
    { "NAME": "holoTear",         "LABEL": "Tear Probability","TYPE": "float", "MIN": 0.0,  "MAX": 0.3,  "DEFAULT": 0.06 },
    { "NAME": "holoBreak",        "LABEL": "EMI Break",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.3 },
    { "NAME": "holoGlow",         "LABEL": "Holo Glow",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.4 },
    { "NAME": "holoTint",         "LABEL": "Hologram Tint",   "TYPE": "color", "DEFAULT": [0.0, 1.0, 0.9, 1.0] },
    { "NAME": "holoMix",          "LABEL": "Hologram Mix",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.85 },
    { "NAME": "audioReact",       "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "inputTex",         "LABEL": "Texture (optional GIF source)", "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "vapor" },
    {}
  ]
}*/

// ──────────────────────────────────────────────────────────────────────
// Shared
// ──────────────────────────────────────────────────────────────────────
float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// ──────────────────────────────────────────────────────────────────────
// Y2K SDF shapes
// ──────────────────────────────────────────────────────────────────────
float sdHeart(vec2 p) {
    p.x = abs(p.x);
    if (p.y + p.x > 1.0)
        return sqrt(dot(p - vec2(0.25, 0.75), p - vec2(0.25, 0.75))) - sqrt(2.0) / 4.0;
    return sqrt(min(dot(p - vec2(0.0, 1.0),  p - vec2(0.0, 1.0)),
                    dot(p - 0.5 * max(p.x + p.y, 0.0), p - 0.5 * max(p.x + p.y, 0.0))))
         * sign(p.x - p.y);
}
float sdStar5(vec2 p, float r) {
    const vec2 k1 = vec2(0.809016994, -0.587785252);
    const vec2 k2 = vec2(-k1.x, k1.y);
    p.x = abs(p.x);
    p -= 2.0 * max(dot(k1, p), 0.0) * k1;
    p -= 2.0 * max(dot(k2, p), 0.0) * k2;
    p.x = abs(p.x);
    p.y -= r;
    vec2 ba = vec2(-0.309016994, 0.951056516) * 0.4;
    float h = clamp(dot(p, ba) / dot(ba, ba), 0.0, 1.0);
    return length(p - ba * h) * sign(p.y * ba.x - p.x * ba.y);
}
float sdSparkle(vec2 p) {
    return min(max(abs(p.x) - 0.08, abs(p.y) - 0.30),
               max(abs(p.y) - 0.08, abs(p.x) - 0.30));
}
float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}
float sdSmiley(vec2 p, float r) {
    float face  = length(p) - r;
    float eyeL  = length(p - vec2(-r * 0.35, r * 0.25)) - r * 0.10;
    float eyeR  = length(p - vec2( r * 0.35, r * 0.25)) - r * 0.10;
    float mr1   = abs(length(p - vec2(0.0, -r * 0.05)) - r * 0.45) - r * 0.06;
    float mouth = max(mr1, -p.y);
    float feat  = min(min(eyeL, eyeR), mouth);
    return max(face, -feat);
}

// ──────────────────────────────────────────────────────────────────────
// PASS 0 — Render Synthwave Night scene to "vapor" buffer
// ──────────────────────────────────────────────────────────────────────
vec4 passVapor(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Sky gradient — deep violet top to teal horizon
    vec3 sky = mix(skyHorizonColor.rgb, skyTopColor.rgb,
                   smoothstep(horizonY - 0.05, 1.0, uv.y));
    vec3 col = sky;

    // Electric cyan sun — HDR 2.5
    vec2 sc = vec2(0.5, horizonY);
    vec2 sd = uv - sc; sd.x *= aspect;
    float sr = sunSize * (1.0 + audioBass * audioReact * 0.06);
    if (length(sd) < sr) {
        float ty = clamp((sd.y / sr + 1.0) * 0.5, 0.0, 1.0);
        // Electric cyan to electric blue gradient — no warm colors
        vec3 sunC = mix(vec3(0.0, 0.5, 1.0), vec3(0.0, 0.9, 1.0), ty) * 2.5;
        if (sunBars > 0.0) {
            float barY = sd.y / sr;
            float barMask = step(0.0, sin(barY * sunBars * 3.14159 + 0.4 + TIME * 0.5));
            // Black bars for contrast
            sunC = mix(sunC, vec3(0.0), barMask * 0.7);
        }
        col = sunC;
    }

    // Perspective grid floor — neon teal HDR 2.0
    if (uv.y < horizonY) {
        float dh = max(horizonY - uv.y, 0.001);
        vec2 gridUV = vec2((uv.x - 0.5) / (dh * gridPersp + 0.05),
                           1.0 / dh - TIME * gridSpeed
                              * (1.0 + audioMid * audioReact * 0.4));
        float gx = abs(fract(gridUV.x * gridDensity) - 0.5);
        float gy = abs(fract(gridUV.y) - 0.5);
        float lineW = 0.04 * dh;
        float fw = fwidth(max(gx, gy));
        float line = smoothstep(0.5 - lineW - fw, 0.5 - lineW + fw, max(gx, gy));
        // Deep navy floor base
        vec3 floorBase = mix(vec3(0.0, 0.0, 0.08),
                             vec3(0.0, 0.15, 0.25), uv.y / horizonY);
        // Neon teal grid lines at HDR 2.0
        col = mix(floorBase, vec3(0.0, 1.0, 0.8) * 2.0, line);
        col = mix(col, sky, smoothstep(horizonY - 0.04, horizonY, uv.y));
    }

    // Y2K chaos layer — fully saturated HDR 2.2 shapes
    int N = int(clamp(y2kCount, 0.0, 20.0));
    for (int i = 0; i < 20; i++) {
        if (i >= N) break;
        float fi = float(i);
        float cycle = floor(TIME * y2kSpeed * (0.3 + hash11(fi * 1.3) * 0.7) + fi * 0.7);
        float life  = fract(TIME * y2kSpeed * (0.3 + hash11(fi * 1.3) * 0.7) + fi * 0.7);
        float h1 = hash11(fi + cycle * 7.13);
        float h2 = hash11(fi + cycle * 13.7);
        float h3 = hash11(fi + cycle * 19.3);
        float h4 = hash11(fi + cycle * 23.1);
        vec2 startP = vec2(h1, h2);
        vec2 vel    = (vec2(h3, h4) - 0.5) * 1.5;
        vec2 ctr    = startP + vel * life * y2kChaos;
        ctr = vec2(0.5 + sin(ctr.x * 3.14159) * 0.45,
                   0.5 + sin(ctr.y * 3.14159) * 0.45);
        float sz   = y2kSize * (0.6 + h1 * 0.8)
                   * (0.7 + 0.3 * sin(TIME * 4.0 + fi))
                   * (1.0 + audioBass * audioReact * 0.4);
        float hue  = fract(h2 + TIME * 0.05);
        vec3 shapeCol = hsv2rgb(vec3(hue, 0.85, 0.95));
        float vis = smoothstep(0.0, 0.15, life) * smoothstep(1.0, 0.85, life);
        float rot = TIME * (0.5 + h3 * 2.0) + fi * 1.7;
        float ca  = cos(rot), sa = sin(rot);
        vec2 d    = uv - ctr; d.x *= aspect;
        vec2 lp   = vec2(ca * d.x - sa * d.y, sa * d.x + ca * d.y) / max(sz, 1e-4);
        int kind = int(hash11(fi * 31.7) * 5.0);
        float dist;
        if      (kind == 0) dist = sdHeart(lp + vec2(0.0, 0.5));
        else if (kind == 1) dist = sdStar5(lp, 0.85);
        else if (kind == 2) dist = sdSparkle(lp * 1.2);
        else if (kind == 3) dist = sdRoundBox(lp, vec2(0.85, 0.40), 0.20);
        else                dist = sdSmiley(lp, 0.85);
        if (dist < 0.0) col = mix(col, shapeCol, vis);
        col = mix(col, vec3(1.0), smoothstep(0.04, 0.0, abs(dist)) * vis * 0.5);
    }

    // Optional input texture overlay
    if (IMG_SIZE_inputTex.x > 0.0) {
        vec3 src = texture(inputTex, fract(uv + vec2(sin(TIME * 0.3) * 0.05, 0.0))).rgb;
        float sL = dot(src, vec3(0.299, 0.587, 0.114));
        col = mix(col, src, smoothstep(0.20, 0.40, sL) * 0.6);
    }

    // Katakana ribbon (top)
    {
        float total = 0.0;
        for (int g = 0; g < 6; g++) {
            float fg = float(g);
            vec2 origin = vec2(0.05 + fg * 0.15, 0.85);
            vec2 ld = (uv - origin) * vec2(60.0, 28.0);
            if (ld.x < 0.0 || ld.y < 0.0 || ld.x > 8.0 || ld.y > 4.0) continue;
            vec2 ci = floor(ld);
            float h = hash21(ci + floor(TIME * (0.4 + audioHigh * audioReact * 1.2)));
            float vert = step(h, 0.55) * step(0.30, fract(ld.x)) * step(fract(ld.x), 0.55);
            float bar  = step(0.55, h) * step(h, 0.85) * step(0.40, fract(ld.y)) * step(fract(ld.y), 0.62);
            total = max(total, max(vert, bar));
        }
        col = mix(col, vec3(0.7, 1.0, 0.85), total * katakanaIntensity);
    }

    // VHS posterize before hologram (gives the holo something quantized to glitch)
    if (vaporPosterize > 1.0) col = floor(col * vaporPosterize) / vaporPosterize;

    return vec4(col, 1.0);
}

// ──────────────────────────────────────────────────────────────────────
// PASS 1 — Hologram glitch over vapor buffer
// ──────────────────────────────────────────────────────────────────────
vec4 passHologram(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;

    // Vertical tear — band-shifted bands of vapor.
    float bandH = 0.04;
    float bandY = floor(uv.y / bandH) * bandH;
    float tearTrig = step(1.0 - holoTear * (1.0 + audioBass * audioReact),
                          hash21(vec2(bandY, floor(TIME * 8.0))));
    uv.x += tearTrig * (hash21(vec2(bandY, TIME)) - 0.5) * 0.15;

    // RGB chromatic shift on the vapor buffer
    float ch = holoChroma * (1.0 + audioHigh * audioReact);
    float r = texture(vapor, clamp(uv + vec2( ch, 0.0), 0.0, 1.0)).r;
    float g = texture(vapor, clamp(uv,                 0.0, 1.0)).g;
    float b = texture(vapor, clamp(uv - vec2( ch, 0.0), 0.0, 1.0)).b;
    vec3 holo = vec3(r, g, b) * holoTint.rgb;

    // Scanlines (resolution-aware)
    holo *= 0.85 + 0.15 * sin(gl_FragCoord.y * holoScanFreq * 0.5);

    // EMI break: rare bursts replace fragments with hash noise
    float breakTrig = step(0.9, hash21(vec2(floor(TIME * 4.0), 0.0)));
    holo = mix(holo, vec3(hash21(uv * TIME)),
               holoBreak * audioBass * audioReact * 0.4 * breakTrig);

    // Mid-band flicker
    float flicker = 0.92 + 0.08 * sin(TIME * 60.0
                  + hash21(vec2(floor(TIME * 30.0))) * 6.28);
    holo *= mix(1.0, flicker, audioMid * audioReact * 0.5);

    // Edge bloom — bright pixels glow beyond their position
    float lum = dot(holo, vec3(0.299, 0.587, 0.114));
    holo += holoTint.rgb * pow(lum, 1.4) * holoGlow * 0.3;

    // Transmission strength — low audio dims the hologram (signal weakens)
    holo *= 0.5 + audioLevel * 0.6;

    // Mix: 0 = pure vapor, 1 = full hologram
    vec3 vapor_ = texture(vapor, fragCoord / RENDERSIZE.xy).rgb;
    return vec4(mix(vapor_, holo, holoMix), 1.0);
}

// ──────────────────────────────────────────────────────────────────────
void main() {
    if (PASSINDEX == 0) FragColor = passVapor(gl_FragCoord.xy);
    else                FragColor = passHologram(gl_FragCoord.xy);
}
