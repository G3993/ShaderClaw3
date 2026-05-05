/*{
  "DESCRIPTION": "Acid Rain Noir — neon rain streaks over a black city skyline silhouette. Acid green / hot magenta / electric cyan on void black.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "rainSpeed",  "LABEL": "Rain Speed",    "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "neonGlow",   "LABEL": "Neon Glow",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "skylineH",   "LABEL": "Skyline Height","TYPE": "float", "DEFAULT": 0.38, "MIN": 0.1, "MAX": 0.7 },
    { "NAME": "audioReact", "LABEL": "Audio",         "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

float hash11(float n) {
    return fract(sin(n * 127.1) * 43758.5453);
}

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// Building height at x (normalized 0..1 screen x), returns 0..1
float buildH(float x) {
    float cell = floor(x * 22.0);
    return 0.12 + hash11(cell * 7.31) * 0.88;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Audio: boosts streak length and brightness
    float audio = 1.0 + audioLevel * audioReact + audioBass * audioReact * 0.4;

    // ── Sky gradient: near-black blue-violet ───────────────────────
    float skyT = uv.y;
    vec3 col = mix(vec3(0.0, 0.0, 0.03), vec3(0.05, 0.0, 0.10), skyT);

    // ── Rain streaks (128) ────────────────────────────────────
    for (int i = 0; i < 128; i++) {
        float fi = float(i);

        float sx   = hash11(fi * 1.313);
        float spd  = rainSpeed * (0.4 + hash11(fi * 2.73) * 0.8) * audio;
        float yPos = fract(hash11(fi * 5.17) + TIME * spd);
        float len  = (0.07 + hash11(fi * 3.91) * 0.12) * audio;

        // Color cycle based on hash
        float hc = hash11(fi * 6.19);
        vec3 streakCol;
        if (hc < 0.33) {
            streakCol = vec3(0.2, 1.0, 0.0);   // acid green
        } else if (hc < 0.66) {
            streakCol = vec3(1.0, 0.0, 0.8);   // hot magenta
        } else {
            streakCol = vec3(0.0, 0.8, 1.0);   // electric cyan
        }

        // X distance: aspect-corrected, sub-pixel AA
        float dx = (uv.x - sx) * aspect;
        float xMask = smoothstep(0.0014, 0.0, abs(dx));

        // Vertical: streak falls downward. yPos=0 → top, increases to bottom
        float topY = 1.0 - yPos;
        float botY = topY - len;

        // Only the pixels inside the streak band
        float inStreak = step(botY, uv.y) * step(uv.y, topY);

        // Fade envelope: sharp at top (tip), full brightness near bottom
        float fadeEnv = 1.0 - clamp((uv.y - botY) / max(len, 0.001), 0.0, 1.0);
        fadeEnv = fadeEnv * fadeEnv;  // sharper tip

        float intensity = xMask * inStreak * fadeEnv * neonGlow;

        // Only draw streaks in the sky (above building silhouette)
        float bh = buildH(uv.x) * skylineH;
        float skyMask = step(bh, uv.y);

        col += streakCol * intensity * skyMask;
    }

    // ── Wet ground neon reflections ──────────────────────────────
    float groundLine = skylineH * 0.28;
    if (uv.y < groundLine) {
        float shimmer = sin(uv.x * 70.0 + TIME * 7.0) * 0.5 + 0.5;
        float groundT = 1.0 - uv.y / max(groundLine, 0.001);
        groundT = groundT * groundT;
        float magGlow  = shimmer * groundT * 0.6;
        float cyanGlow = (1.0 - shimmer) * groundT * 0.5;
        col += vec3(1.0, 0.0, 0.8) * magGlow  * neonGlow * 0.6;
        col += vec3(0.0, 0.8, 1.0) * cyanGlow * neonGlow * 0.6;
    }

    // ── Building silhouettes ───────────────────────────────────
    float bh = buildH(uv.x) * skylineH;
    if (uv.y < bh) {
        // Solid black building fill
        col = vec3(0.0);

        // Dim amber window glow via 2D grid hash
        float winX = floor(uv.x * 80.0);
        float winY = floor(uv.y * 60.0);
        float winH = hash21(vec2(winX, winY));
        if (winH > 0.78) {
            float winBright = (winH - 0.78) / 0.22;
            col += vec3(0.9, 0.65, 0.1) * winBright * winBright * 0.25;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
