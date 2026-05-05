/*{
  "DESCRIPTION": "Blade Hologram — cyberpunk noir city at dusk transmitted through a degrading holographic channel. Pass 0 renders neon rain city (moon, skyline silhouette, rain reflections, neon signs). Pass 1 layers hologram glitch: vertical tear, RGB shift, EMI bursts.",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "CREDIT": "Easel — cyberpunk noir rework of vapor_hologram",
  "INPUTS": [
    { "NAME": "horizonY",         "LABEL": "Horizon",         "TYPE": "float", "MIN": 0.30, "MAX": 0.70, "DEFAULT": 0.48 },
    { "NAME": "skyTopColor",      "LABEL": "Sky Top",         "TYPE": "color", "DEFAULT": [0.01, 0.0,  0.03, 1.0] },
    { "NAME": "skyHorizonColor",  "LABEL": "Sky Horizon",     "TYPE": "color", "DEFAULT": [0.05, 0.0,  0.12, 1.0] },
    { "NAME": "moonSize",         "LABEL": "Moon Size",       "TYPE": "float", "MIN": 0.04, "MAX": 0.30, "DEFAULT": 0.14 },
    { "NAME": "signCount",        "LABEL": "Neon Signs",      "TYPE": "float", "MIN": 0.0,  "MAX": 20.0, "DEFAULT": 14.0 },
    { "NAME": "signSpeed",        "LABEL": "Sign Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.4 },
    { "NAME": "signSize",         "LABEL": "Sign Size",       "TYPE": "float", "MIN": 0.01, "MAX": 0.15, "DEFAULT": 0.06 },
    { "NAME": "rainDensity",      "LABEL": "Rain Lines",      "TYPE": "float", "MIN": 4.0,  "MAX": 40.0, "DEFAULT": 18.0 },
    { "NAME": "rainSpeed",        "LABEL": "Rain Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.5 },
    { "NAME": "katakanaIntensity","LABEL": "Katakana",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "holoChroma",       "LABEL": "Holo Chroma",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.04, "DEFAULT": 0.014 },
    { "NAME": "holoScanFreq",     "LABEL": "Holo Scanlines",  "TYPE": "float", "MIN": 1.0,  "MAX": 4.0,  "DEFAULT": 2.0 },
    { "NAME": "holoTear",         "LABEL": "Tear Prob",       "TYPE": "float", "MIN": 0.0,  "MAX": 0.3,  "DEFAULT": 0.07 },
    { "NAME": "holoBreak",        "LABEL": "EMI Break",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.3 },
    { "NAME": "holoGlow",         "LABEL": "Holo Glow",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.5,  "DEFAULT": 1.6 },
    { "NAME": "holoTint",         "LABEL": "Hologram Tint",   "TYPE": "color", "DEFAULT": [0.0,  1.0, 0.55, 1.0] },
    { "NAME": "holoMix",          "LABEL": "Hologram Mix",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.88 },
    { "NAME": "audioReact",       "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ],
  "PASSES": [
    { "TARGET": "city" },
    {}
  ]
}*/

// ──────────────────────────────────────────────────────────────────────
// Shared
// ──────────────────────────────────────────────────────────────────────
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float sdRoundBox2(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

// ──────────────────────────────────────────────────────────────────────
// PASS 0 — Cyberpunk noir city scene
// ──────────────────────────────────────────────────────────────────────
vec4 passCity(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Sky gradient — void black top to deep violet horizon
    vec3 sky = mix(skyHorizonColor.rgb, skyTopColor.rgb,
                   smoothstep(horizonY - 0.05, 1.0, uv.y));
    vec3 col = sky;

    // Moon — large silver-white circle, upper center-left
    vec2 moonPos = vec2(0.35, horizonY + 0.25);
    vec2 md = uv - moonPos; md.x *= aspect;
    float moonR = moonSize * (1.0 + audioBass * audioReact * 0.04);
    float moonDist = length(md);
    if (moonDist < moonR) {
        // Subtle crater noise
        float noise = hash21(floor(md * 80.0)) * 0.06;
        vec3 moonCol = vec3(0.85 + noise, 0.88 + noise, 0.9 + noise) * 2.2; // HDR white
        col = moonCol;
    }
    // Moon haze glow
    float moonHaze = 1.0 - smoothstep(moonR, moonR * 2.5, moonDist);
    col = mix(col, vec3(0.4, 0.5, 0.65) * 0.5, moonHaze * 0.35);

    // Building skyline silhouettes at horizon
    {
        float buildingY = horizonY;
        float bx = uv.x;
        // Tiled building profile using hash
        float bSlice = floor(bx * 22.0);
        float bH = hash11(bSlice) * 0.18 + 0.04;
        float bTop = buildingY + bH;
        if (uv.y > buildingY && uv.y < bTop) {
            // Solid black silhouette
            col = vec3(0.0, 0.0, 0.005);
            // Occasional lit window on the silhouette
            float wx = fract(bx * 22.0) * 8.0;
            float wy = (uv.y - buildingY) / bH * 6.0;
            float wCell = hash21(vec2(floor(wx), floor(wy)) + bSlice);
            if (wCell > 0.72) {
                float hue = hash11(wCell * 31.7);
                col = hsv2rgb(vec3(hue, 0.9, 1.0)) * 1.5;
            }
        }
    }

    // Rain reflection floor — below horizon
    if (uv.y < horizonY) {
        float dh = max(horizonY - uv.y, 0.001);
        float t = TIME * rainSpeed;
        // Horizontal rain streaks
        float rainV = uv.y * rainDensity + t * 8.0;
        float rainCell = floor(rainV);
        float rainLocal = fract(rainV);
        // Break probability for each rain streak row
        float rainH = hash11(rainCell + floor(uv.x * 40.0));
        float rainStreak = step(rainH, 0.45) * smoothstep(0.0, 0.06, rainLocal) * smoothstep(0.14, 0.08, rainLocal);
        // Puddle reflection: mix in sky color dimmed
        vec3 reflectedSky = sky * 0.4;
        col = mix(reflectedSky * 0.3, reflectedSky, smoothstep(horizonY - 0.04, horizonY, uv.y));
        col += vec3(0.6, 0.8, 1.0) * rainStreak * 0.5;
    }

    // Neon signs — glowing rectangles floating in the sky
    int N = int(clamp(signCount, 0.0, 20.0));
    for (int i = 0; i < 20; i++) {
        if (i >= N) break;
        float fi = float(i);
        float cycle = floor(TIME * signSpeed * 0.15 + fi * 0.5);
        float h1 = hash11(fi + cycle * 7.1);
        float h2 = hash11(fi + cycle * 13.3);
        float h3 = hash11(fi + cycle * 19.7);
        // Signs float in the sky, scattered across the scene
        vec2 ctr = vec2(h1 * 0.9 + 0.05, horizonY + h2 * 0.38 + 0.05);
        float signW = signSize * (1.0 + h3 * 1.2);
        float signH = signSize * (0.4 + hash11(fi * 31.7) * 0.6);
        // Slight oscillation
        ctr.y += sin(TIME * (0.3 + h1 * 0.4) + fi * 2.1) * 0.012;

        vec2 d = uv - ctr;
        d.x *= aspect;
        float dist = sdRoundBox2(d, vec2(signW, signH), signH * 0.15);

        // Neon hue: fully saturated, 6 distinct colors
        float hue = fract(h3 + float(i) / 6.0);
        vec3 neonCol = hsv2rgb(vec3(hue, 1.0, 1.0));

        // Sign fill (semi-transparent edge)
        float fillAlpha = smoothstep(0.005, -0.005, dist);
        float glowAlpha = 1.0 - smoothstep(0.0, signH * 0.8, dist);

        float vis = 0.9 + 0.1 * sin(TIME * (1.5 + h1 * 2.0) + fi);
        // Audio: bass pulses sign brightness
        float audioBoost = 1.0 + audioBass * audioReact * 0.5;

        // Neon outline (inner face, very bright HDR)
        float outline = smoothstep(0.008, 0.0, abs(dist));
        col = mix(col, neonCol * (2.2 * audioBoost), fillAlpha * vis * 0.7);
        col += neonCol * outline * 2.5 * vis * audioBoost;
        // Soft glow spread
        col += neonCol * glowAlpha * 0.08 * vis;
    }

    // Katakana ribbon (upper strip, kept from original)
    {
        float total = 0.0;
        for (int g = 0; g < 6; g++) {
            float fg = float(g);
            vec2 origin = vec2(0.05 + fg * 0.15, 0.80);
            vec2 ld = (uv - origin) * vec2(60.0, 28.0);
            if (ld.x < 0.0 || ld.y < 0.0 || ld.x > 8.0 || ld.y > 4.0) continue;
            vec2 ci = floor(ld);
            float h = hash21(ci + floor(TIME * (0.4 + audioHigh * audioReact * 1.2)));
            float vert = step(h, 0.55) * step(0.30, fract(ld.x)) * step(fract(ld.x), 0.55);
            float bar  = step(0.55, h) * step(h, 0.85) * step(0.40, fract(ld.y)) * step(fract(ld.y), 0.62);
            total = max(total, max(vert, bar));
        }
        // Green katakana (Matrix-style) instead of teal
        col = mix(col, vec3(0.0, 1.0, 0.35) * 1.6, total * katakanaIntensity);
    }

    return vec4(col, 1.0);
}

// ──────────────────────────────────────────────────────────────────────
// PASS 1 — Hologram glitch over city buffer
// ──────────────────────────────────────────────────────────────────────
vec4 passHologram(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;

    // Vertical tear
    float bandH = 0.04;
    float bandY = floor(uv.y / bandH) * bandH;
    float tearTrig = step(1.0 - holoTear * (1.0 + audioBass * audioReact),
                          hash21(vec2(bandY, floor(TIME * 8.0))));
    uv.x += tearTrig * (hash21(vec2(bandY, TIME)) - 0.5) * 0.15;

    // RGB chroma shift
    float ch = holoChroma * (1.0 + audioHigh * audioReact);
    float r = texture(city, clamp(uv + vec2( ch, 0.0), 0.0, 1.0)).r;
    float g = texture(city, clamp(uv,               0.0, 1.0)).g;
    float b = texture(city, clamp(uv - vec2( ch, 0.0), 0.0, 1.0)).b;
    vec3 holo = vec3(r, g, b) * holoTint.rgb;

    // Scanlines
    holo *= 0.85 + 0.15 * sin(gl_FragCoord.y * holoScanFreq * 0.5);

    // EMI break
    float breakTrig = step(0.9, hash21(vec2(floor(TIME * 4.0), 0.0)));
    holo = mix(holo, vec3(hash21(uv * TIME)),
               holoBreak * audioBass * audioReact * 0.4 * breakTrig);

    // Mid-band flicker
    float flicker = 0.92 + 0.08 * sin(TIME * 60.0 + hash21(vec2(floor(TIME * 30.0))) * 6.28);
    holo *= mix(1.0, flicker, audioMid * audioReact * 0.5);

    // Edge bloom
    float lum = dot(holo, vec3(0.299, 0.587, 0.114));
    holo += holoTint.rgb * pow(lum, 1.4) * holoGlow * 0.4;

    // Transmission: NEVER drops below 82% — fixes the audio-dependency bug
    holo *= max(0.82, 0.65 + audioLevel * audioReact * 0.4);

    // Mix: 0=pure city, 1=full hologram
    vec3 city_ = texture(city, fragCoord / RENDERSIZE.xy).rgb;
    return vec4(mix(city_, holo, holoMix), 1.0);
}

// ──────────────────────────────────────────────────────────────────────
void main() {
    if (PASSINDEX == 0) gl_FragColor = passCity(gl_FragCoord.xy);
    else                gl_FragColor = passHologram(gl_FragCoord.xy);
}
