/*{
  "DESCRIPTION": "Synthwave Cityscape — noir cyberpunk night city with neon signs, building silhouettes, and wet pavement reflections",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "CREDIT": "ShaderClaw auto-improve",
  "INPUTS": [
    { "NAME": "buildingCount", "TYPE": "float", "DEFAULT": 10.0, "MIN": 5.0, "MAX": 16.0, "LABEL": "Buildings" },
    { "NAME": "neonIntensity", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 4.0, "LABEL": "Neon Glow" },
    { "NAME": "windowFlicker", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Window Flicker" },
    { "NAME": "hazeDensity", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Atmospheric Haze" },
    { "NAME": "audioReact", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio" }
  ],
  "PASSES": [
    { "TARGET": "city" },
    {}
  ]
}*/

// ──────────────────────────────────────────────────────────────────────
// Shared utilities
// ──────────────────────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// ──────────────────────────────────────────────────────────────────────
// PASS 0 — City scene rendered to "city" buffer
// ──────────────────────────────────────────────────────────────────────
vec4 passCity(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Street/horizon level — buildings sit on this line
    float streetY = 0.35;

    // ── Night sky gradient ──────────────────────────────────────────
    // Deep navy at top, deep purple-black near horizon
    vec3 skyTop     = vec3(0.01, 0.02, 0.08);
    vec3 skyHorizon = vec3(0.05, 0.01, 0.12);
    // Amber light pollution near horizon (city glow)
    vec3 lightPollution = vec3(0.18, 0.06, 0.02);
    float pollutionFade = smoothstep(streetY + 0.25, streetY, uv.y);
    vec3 sky = mix(skyTop, skyHorizon, smoothstep(1.0, streetY, uv.y));
    sky = mix(sky, lightPollution, pollutionFade * 0.45);

    vec3 col = sky;

    // ── Moon ────────────────────────────────────────────────────────
    vec2 moonCenter = vec2(0.82, 0.78);
    float moonR = 0.045;
    vec2 moonD = uv - moonCenter;
    moonD.x *= aspect;
    float moonDist = length(moonD);
    // Soft glow halo
    float moonGlow = exp(-moonDist * 12.0) * 0.25;
    col += vec3(0.7, 0.75, 0.9) * moonGlow;
    // Moon disc
    float moonMask = smoothstep(moonR, moonR - 0.003, moonDist);
    col = mix(col, vec3(0.82, 0.85, 0.92), moonMask);

    // ── Audio reactive values ───────────────────────────────────────
    float bassPulse   = 1.0 + audioBass * audioReact * 0.5;
    float midFlicker  = 1.0 + audioMid  * audioReact * windowFlicker * 0.6;

    // ── Building silhouettes ────────────────────────────────────────
    int N = int(clamp(buildingCount, 5.0, 16.0));
    vec3 buildingColor = vec3(0.03, 0.02, 0.05);
    float totalBuildingMask = 0.0;
    vec3 neonAccum = vec3(0.0);

    for (int i = 0; i < 16; i++) {
        if (i >= N) break;
        float fi = float(i);

        float h1 = hash11(fi * 3.71 + 1.0);
        float h2 = hash11(fi * 7.13 + 2.0);
        float h3 = hash11(fi * 13.7 + 3.0);
        float h4 = hash11(fi * 19.3 + 4.0);

        // X position spread across screen with slight variation
        float bx = (fi + 0.5 + (h1 - 0.5) * 0.6) / float(N);
        float bw = 0.03 + h2 * 0.04;
        float bh = 0.12 + h3 * 0.38;
        float btop = streetY + bh;

        float left  = bx - bw;
        float right = bx + bw;

        float inX = step(left, uv.x) * step(uv.x, right);
        float inY = step(0.0,  uv.y) * step(uv.y, btop);
        float bMask = inX * inY;

        if (bMask > 0.5) {
            col = buildingColor;
            totalBuildingMask = 1.0;

            // Windows grid in local building UV coords
            vec2 localUV = vec2(
                (uv.x - left) / (2.0 * bw),
                (uv.y - 0.0)  / bh
            );
            vec2 winGrid = localUV * vec2(5.0, bh * 20.0);
            vec2 winCell = floor(winGrid);
            vec2 winFrac = fract(winGrid);

            float winInner = step(0.2, winFrac.x) * step(winFrac.x, 0.75)
                           * step(0.15, winFrac.y) * step(winFrac.y, 0.80);

            // Window on/off flicker
            float flickerRate = 0.5 + windowFlicker * midFlicker * 1.5;
            float winHash = hash21(winCell + vec2(fi * 17.3, floor(TIME * flickerRate)));
            float winOn = step(0.35, winHash);

            // Amber windows: HDR 1.5
            vec3 winColor = vec3(1.0, 0.7, 0.1) * 1.5;
            col = mix(col, winColor, winOn * winInner * 0.9);
        }

        // ── Neon signs on building rooftops ─────────────────────
        float neonChance = hash11(fi * 41.3 + 5.0);
        if (neonChance > 0.55) {
            float neonH   = 0.012;
            float neonY   = btop - neonH * (1.5 + h4 * 3.0);
            float nLeft   = bx - bw * 0.85;
            float nRight  = bx + bw * 0.85;

            float nInX = step(nLeft, uv.x) * step(uv.x, nRight);
            float nInY = step(neonY - neonH * 0.5, uv.y) * step(uv.y, neonY + neonH * 0.5);
            float neonMask = nInX * nInY;

            // Alternate magenta / cyan by building index
            float neonType = step(0.5, hash11(fi * 29.7));
            vec3 magenta = vec3(1.0, 0.0, 0.8) * neonIntensity * bassPulse;
            vec3 cyan    = vec3(0.0, 0.8, 1.0) * neonIntensity * bassPulse;
            vec3 neonCol = mix(magenta, cyan, neonType);

            if (neonMask > 0.5) {
                col = neonCol;
            }

            // Neon bloom glow
            float cx = clamp(uv.x, nLeft, nRight);
            float neonDist = length(vec2((uv.x - cx) * aspect, uv.y - neonY));
            float glow = exp(-neonDist * 28.0) * 0.6;
            neonAccum += neonCol * glow;
        }
    }

    // Add neon glow (attenuated inside buildings)
    col += neonAccum * (1.0 - totalBuildingMask * 0.7);

    // ── Street / wet pavement ───────────────────────────────────────
    if (uv.y < streetY) {
        float groundT = uv.y / streetY;
        vec3 groundColor = mix(vec3(0.005, 0.003, 0.008),
                               vec3(0.02, 0.01, 0.03), groundT);
        col = groundColor;

        float reflFade = smoothstep(0.0, streetY, uv.y) * 0.55;

        // Wet pavement ripple
        float ripple = sin(uv.x * 80.0 + TIME * 2.0) * 0.002;
        float reflX = uv.x + ripple;

        // Reflect neon glow across streetY
        vec3 rNeonAccum = vec3(0.0);
        for (int ri = 0; ri < 16; ri++) {
            if (ri >= N) break;
            float rfi = float(ri);
            float rh1 = hash11(rfi * 3.71 + 1.0);
            float rh2 = hash11(rfi * 7.13 + 2.0);
            float rh3 = hash11(rfi * 13.7 + 3.0);
            float rh4 = hash11(rfi * 19.3 + 4.0);
            float rbx  = (rfi + 0.5 + (rh1 - 0.5) * 0.6) / float(N);
            float rbw  = 0.03 + rh2 * 0.04;
            float rbh  = 0.12 + rh3 * 0.38;
            float rbtop = streetY + rbh;
            float rneonChance = hash11(rfi * 41.3 + 5.0);
            if (rneonChance > 0.55) {
                float rneonH  = 0.012;
                float rneonY  = rbtop - rneonH * (1.5 + rh4 * 3.0);
                float rnLeft  = rbx - rbw * 0.85;
                float rnRight = rbx + rbw * 0.85;
                // Reflected Y across streetY
                float rneonYrefl = streetY - (rneonY - streetY);
                float rcx = clamp(reflX, rnLeft, rnRight);
                float rneonDist = length(vec2((reflX - rcx) * aspect, uv.y - rneonYrefl));
                float rneonType = step(0.5, hash11(rfi * 29.7));
                vec3 rMagenta = vec3(1.0, 0.0, 0.8) * neonIntensity * bassPulse;
                vec3 rCyan    = vec3(0.0, 0.8, 1.0) * neonIntensity * bassPulse;
                vec3 rGlowColor = mix(rMagenta, rCyan, rneonType);
                float rGlow = exp(-rneonDist * 22.0) * 0.45;
                rNeonAccum += rGlowColor * rGlow;
            }
        }

        col = groundColor + rNeonAccum * reflFade;
    }

    return vec4(col, 1.0);
}

// ──────────────────────────────────────────────────────────────────────
// PASS 1 — Atmospheric composite: haze + chromatic aberration + bloom
// ──────────────────────────────────────────────────────────────────────
vec4 passAtmosphere(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;
    float streetY = 0.35;

    // Base city read
    vec3 col = texture(city, uv).rgb;

    // ── Chromatic aberration on bright neon areas ───────────────────
    float lumC = dot(col, vec3(0.299, 0.587, 0.114));
    float aberStr = 0.003 * clamp(lumC * 2.0, 0.0, 1.0);
    float rC = texture(city, clamp(uv + vec2( aberStr, 0.0), 0.0, 1.0)).r;
    float gC = texture(city, uv).g;
    float bC = texture(city, clamp(uv - vec2( aberStr, 0.0), 0.0, 1.0)).b;
    col = vec3(rC, gC, bC);

    // ── Atmospheric haze ────────────────────────────────────────────
    // Blue fog increases with height in sky
    float hazeHeight  = smoothstep(streetY, 1.0, uv.y);
    vec3  hazeColor   = vec3(0.04, 0.05, 0.12);
    // Horizon haze to blend building tops into sky
    float horizonHaze = smoothstep(streetY + 0.18, streetY, uv.y)
                      * smoothstep(0.0, streetY, uv.y);
    vec3  horizonHazeColor = vec3(0.08, 0.03, 0.14);
    col = mix(col, hazeColor,        hazeHeight  * hazeDensity * 0.4);
    col = mix(col, horizonHazeColor, horizonHaze * hazeDensity * 0.3);

    // ── Subtle vignette ─────────────────────────────────────────────
    vec2 vcUV = uv * 2.0 - 1.0;
    float vignette = 1.0 - dot(vcUV, vcUV) * 0.25;
    col *= vignette;

    // ── Linear HDR output, no clamping ─────────────────────────────
    return vec4(col, 1.0);
}

// ──────────────────────────────────────────────────────────────────────
void main() {
    if (PASSINDEX == 0) FragColor = passCity(gl_FragCoord.xy);
    else                FragColor = passAtmosphere(gl_FragCoord.xy);
}
