/*{
  "DESCRIPTION": "Cyberpunk Rainy Alley — dark rain-soaked alley with neon signs and wet reflections transmitted through a degrading holographic channel. Pass 0 renders the alley scene. Pass 1 layers hologram glitch on top: vertical tear, RGB shift, EMI bursts, hologram tint, scanlines.",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "CREDIT": "Easel — cyberpunk alley + hologram_glitch",
  "INPUTS": [
    { "NAME": "horizonY",         "LABEL": "Horizon",         "TYPE": "float", "MIN": 0.40, "MAX": 0.75, "DEFAULT": 0.55 },
    { "NAME": "skyTopColor",      "LABEL": "Sky Top",         "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.03, 1.0] },
    { "NAME": "skyHorizonColor",  "LABEL": "Sky Horizon",     "TYPE": "color", "DEFAULT": [0.0, 0.02, 0.06, 1.0] },
    { "NAME": "sunSize",          "LABEL": "Sun Size",        "TYPE": "float", "MIN": 0.05, "MAX": 0.40, "DEFAULT": 0.22 },
    { "NAME": "sunBars",          "LABEL": "Sun Bars",        "TYPE": "float", "MIN": 0.0,  "MAX": 12.0, "DEFAULT": 6.0 },
    { "NAME": "gridDensity",      "LABEL": "Grid Density",    "TYPE": "float", "MIN": 4.0,  "MAX": 24.0, "DEFAULT": 12.0 },
    { "NAME": "gridPersp",        "LABEL": "Grid Perspective","TYPE": "float", "MIN": 0.5,  "MAX": 4.0,  "DEFAULT": 1.8 },
    { "NAME": "gridSpeed",        "LABEL": "Grid Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25 },
    { "NAME": "y2kCount",         "LABEL": "Rain Density",    "TYPE": "float", "MIN": 0.0,  "MAX": 20.0, "DEFAULT": 12.0 },
    { "NAME": "y2kSpeed",         "LABEL": "Rain Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6 },
    { "NAME": "y2kSize",          "LABEL": "Rain Width",      "TYPE": "float", "MIN": 0.02, "MAX": 0.20, "DEFAULT": 0.07 },
    { "NAME": "y2kChaos",         "LABEL": "Chaos",           "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.7 },
    { "NAME": "katakanaIntensity","LABEL": "Billboard",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6 },
    { "NAME": "vaporPosterize",   "LABEL": "Posterize",       "TYPE": "float", "MIN": 1.0,  "MAX": 32.0, "DEFAULT": 16.0 },
    { "NAME": "holoChroma",       "LABEL": "Holo Chroma",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "holoScanFreq",     "LABEL": "Holo Scanlines",  "TYPE": "float", "MIN": 1.0,  "MAX": 4.0,  "DEFAULT": 2.0 },
    { "NAME": "holoTear",         "LABEL": "Tear Probability","TYPE": "float", "MIN": 0.0,  "MAX": 0.3,  "DEFAULT": 0.06 },
    { "NAME": "holoBreak",        "LABEL": "EMI Break",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.3 },
    { "NAME": "holoGlow",         "LABEL": "Holo Glow",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7 },
    { "NAME": "holoTint",         "LABEL": "Hologram Tint",   "TYPE": "color", "DEFAULT": [0.55, 1.0, 0.95, 1.0] },
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
// Shared utilities
// ──────────────────────────────────────────────────────────────────────
float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

// Signed distance: rounded rectangle (2D)
float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

// ──────────────────────────────────────────────────────────────────────
// PASS 0 — Render cyberpunk rainy alley to "vapor" buffer
// ──────────────────────────────────────────────────────────────────────
vec4 passAlley(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;

    // ── Sky / void background ──────────────────────────────────
    vec3 sky = mix(skyHorizonColor.rgb, skyTopColor.rgb,
                   smoothstep(0.3, 1.0, uv.y));
    vec3 col = sky;

    // ── Alley walls (left & right bands) ────────────────────────
    vec3 wallColor = vec3(0.04, 0.04, 0.06);
    float wallL = smoothstep(0.27, 0.22, uv.x);
    float wallR = smoothstep(0.73, 0.78, uv.x);
    col = mix(col, wallColor, wallL + wallR);

    // Subtle brick texture on walls
    if (uv.x < 0.27 || uv.x > 0.73) {
        vec2 brickUV;
        if (uv.x < 0.27) {
            brickUV = vec2((0.25 - uv.x) * 10.0, uv.y * 12.0);
        } else {
            brickUV = vec2((uv.x - 0.75) * 10.0, uv.y * 12.0);
        }
        float rowShift = floor(brickUV.y) * 0.5;
        vec2 brickCell = floor(vec2(brickUV.x + rowShift, brickUV.y));
        float mortar = smoothstep(0.05, 0.12, fract(brickUV.x + rowShift)) *
                       smoothstep(0.05, 0.12, fract(brickUV.y));
        col = mix(col, wallColor * 0.6, (1.0 - mortar) * (wallL + wallR));
    }

    // ── Neon sign — LEFT WALL: hot magenta rectangle ─────────────────
    {
        float signFlicker = sin(TIME * 2.3 + 0.0) * 0.3 + 0.7;
        // Sign region: x in [0.02,0.22], y in [0.50,0.65]
        vec2 signCenter = vec2(0.12, 0.575);
        vec2 signHalf   = vec2(0.09, 0.07);
        float r = 0.008;
        vec2  sp = uv - signCenter;
        float outline = sdRoundBox(sp, signHalf, r);
        // Outer glow
        float glow = exp(-max(outline, 0.0) * 80.0);
        // Inner bright edge
        float edge = smoothstep(0.003, 0.0, abs(outline));
        vec3  magenta = vec3(1.0, 0.0, 0.8) * 2.0 * signFlicker;
        col += magenta * (edge * 1.8 + glow * 0.4) * wallL;

        // Horizontal bars inside sign (fake text rows)
        float inside = step(outline, 0.0);
        float bars = step(0.5, sin((sp.y / signHalf.y * 3.0 + TIME * 1.5) * 6.28));
        col = mix(col, vec3(0.04, 0.01, 0.03), inside * bars * wallL * 0.8);
    }

    // ── Neon sign — RIGHT WALL: electric cyan rectangle ──────────────
    {
        float signFlicker = sin(TIME * 1.7 + 1.4) * 0.3 + 0.7;
        vec2 signCenter = vec2(0.88, 0.525);
        vec2 signHalf   = vec2(0.08, 0.065);
        float r = 0.008;
        vec2  sp = uv - signCenter;
        float outline = sdRoundBox(sp, signHalf, r);
        float glow = exp(-max(outline, 0.0) * 80.0);
        float edge = smoothstep(0.003, 0.0, abs(outline));
        vec3  cyan = vec3(0.0, 0.9, 1.0) * 2.0 * signFlicker;
        col += cyan * (edge * 1.8 + glow * 0.4) * wallR;

        float inside = step(outline, 0.0);
        float bars = step(0.5, sin((sp.y / signHalf.y * 3.0 - TIME * 1.8) * 6.28));
        col = mix(col, vec3(0.0, 0.02, 0.04), inside * bars * wallR * 0.8);
    }

    // ── Holographic ad billboard — upper center ────────────────────
    {
        // Billboard spans x=[0.28,0.72], y=[0.60,0.88]
        float inBB = step(0.28, uv.x) * step(uv.x, 0.72) *
                     step(0.60, uv.y) * step(uv.y, 0.88);
        if (inBB > 0.5) {
            vec2 bbUV = (uv - vec2(0.28, 0.60)) / vec2(0.44, 0.28);
            // Horizontal scan bars of shifting cyan/violet
            float scanLine = floor(bbUV.y * 14.0);
            float phase = scanLine * 0.37 + TIME * 0.8;
            float hue = fract(phase * 0.15);
            // Alternate cyan and violet bands
            vec3 bandCol;
            if (mod(scanLine, 2.0) < 1.0) {
                bandCol = vec3(0.0, 0.9, 1.0); // cyan
            } else {
                bandCol = vec3(0.6, 0.1, 1.0); // violet
            }
            bandCol *= 1.5 + 0.5 * sin(TIME * 3.0 + scanLine * 0.9);
            // Fake character pattern inside each bar
            float charX = fract(bbUV.x * 20.0 + floor(scanLine) * 0.5);
            float charPat = step(0.4, charX) * step(charX, 0.85);
            col = mix(col, bandCol * charPat, katakanaIntensity * 0.9 * inBB);
            // Billboard frame glow
            vec2 bbCenter = vec2(0.5, 0.5);
            float frame = sdRoundBox(bbUV - bbCenter, vec2(0.5, 0.5), 0.02);
            float frameGlow = exp(-max(frame, 0.0) * 30.0);
            float frameEdge = smoothstep(0.004, 0.0, abs(frame));
            col += vec3(0.0, 0.9, 1.0) * (frameEdge * 1.2 + frameGlow * 0.3) * inBB;
        }
    }

    // ── 96 rain streaks ───────────────────────────────────────────────
    // Three palette colors: hot magenta, electric cyan, amber
    vec3 rainPalette[3];
    rainPalette[0] = vec3(1.0, 0.0,  0.8);  // hot magenta
    rainPalette[1] = vec3(0.0, 0.9,  1.0);  // electric cyan
    rainPalette[2] = vec3(1.0, 0.6,  0.0);  // amber

    for (int i = 0; i < 96; i++) {
        float fi = float(i);
        float speed = 0.4 + hash11(fi * 3.17) * 0.9
                    + audioLevel * audioReact * 0.3;
        float xPos  = hash11(fi * 7.13 + floor(TIME * speed * 0.1 + fi * 0.07));
        float yOff  = fract(TIME * speed * 0.25 + hash11(fi * 11.3));
        // y goes from 1 to 0 (top to bottom), reset via fract
        float yCenter = 1.0 - yOff;
        float length_  = 0.04 + hash11(fi * 5.71) * 0.06;
        float width_   = 0.0015 + hash11(fi * 2.37) * 0.002;

        // Capsule / vertical streak SDF
        float dx = abs(uv.x - xPos);
        float dy = uv.y - yCenter;
        float streak = max(dx - width_, abs(dy - length_ * 0.5) - length_ * 0.5);
        float alpha = smoothstep(0.002, 0.0, streak);

        int palIdx = int(mod(fi, 3.0));
        vec3 rainCol;
        if (palIdx == 0) rainCol = rainPalette[0];
        else if (palIdx == 1) rainCol = rainPalette[1];
        else rainCol = rainPalette[2];

        // Skip rain inside wall bands only slightly (let some bleed through for atmosphere)
        float inAlley = smoothstep(0.20, 0.26, uv.x) * smoothstep(0.80, 0.74, uv.x);
        col += rainCol * alpha * 2.5 * (0.3 + inAlley * 0.7);
    }

    // ── Wet ground reflections (bottom quarter) ──────────────────────
    if (uv.y < 0.28) {
        float reflY = uv.y / 0.28; // 0 at bottom, 1 at ground horizon

        // Shimmer distortion
        float shimmer = sin(uv.x * 60.0 + TIME * 4.0) * 0.01
                      + sin(uv.x * 37.0 - TIME * 2.7) * 0.006;
        vec2 reflUV = vec2(uv.x + shimmer, 0.28 + (0.28 - uv.y)); // flip above

        // Sample reflected scene approximately with procedural colors
        // Left neon magenta reflection
        float reflMag = exp(-abs(uv.x - 0.12) * 12.0);
        // Right neon cyan reflection
        float reflCyan = exp(-abs(uv.x - 0.88) * 12.0);

        float flickerM = sin(TIME * 2.3) * 0.3 + 0.7;
        float flickerC = sin(TIME * 1.7 + 1.4) * 0.3 + 0.7;

        vec3 reflMagCol  = vec3(1.0, 0.0, 0.8) * reflMag * flickerM;
        vec3 reflCyanCol = vec3(0.0, 0.9, 1.0) * reflCyan * flickerC;

        float puddleFade = reflY; // attenuate near bottom edge
        col += (reflMagCol + reflCyanCol) * 1.8 * (1.0 - puddleFade * 0.7);

        // Ground surface tint — very dark wet concrete
        float groundMix = smoothstep(0.28, 0.0, uv.y) * 0.7;
        col = mix(col, vec3(0.02, 0.02, 0.03), groundMix * 0.5);
    }

    // Optional input texture overlay
    if (IMG_SIZE_inputTex.x > 0.0) {
        vec3 src = texture2D(inputTex, fract(uv + vec2(sin(TIME * 0.3) * 0.05, 0.0))).rgb;
        float sL = dot(src, vec3(0.299, 0.587, 0.114));
        col = mix(col, src, smoothstep(0.20, 0.40, sL) * 0.6);
    }

    // Posterize (gives holo something quantized to glitch)
    if (vaporPosterize > 1.0) col = floor(col * vaporPosterize) / vaporPosterize;

    return vec4(col, 1.0);
}

// ──────────────────────────────────────────────────────────────────────
// PASS 1 — Hologram glitch over alley buffer
// ──────────────────────────────────────────────────────────────────────
vec4 passHologram(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;

    // Vertical tear — band-shifted bands of the buffer
    float bandH = 0.04;
    float bandY = floor(uv.y / bandH) * bandH;
    float tearTrig = step(1.0 - holoTear * (1.0 + audioBass * audioReact),
                          hash21(vec2(bandY, floor(TIME * 8.0))));
    uv.x += tearTrig * (hash21(vec2(bandY, TIME)) - 0.5) * 0.15;

    // RGB chromatic shift on the vapor buffer
    float ch = holoChroma * (1.0 + audioHigh * audioReact);
    float r = texture2D(vapor, clamp(uv + vec2( ch, 0.0), 0.0, 1.0)).r;
    float g = texture2D(vapor, clamp(uv,                  0.0, 1.0)).g;
    float b = texture2D(vapor, clamp(uv - vec2( ch, 0.0), 0.0, 1.0)).b;
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

    // Mix: 0 = pure alley, 1 = full hologram
    vec3 alley = texture2D(vapor, fragCoord / RENDERSIZE.xy).rgb;
    return vec4(mix(alley, holo, holoMix), 1.0);
}

// ──────────────────────────────────────────────────────────────────────
void main() {
    if (PASSINDEX == 0) gl_FragColor = passAlley(gl_FragCoord.xy);
    else                gl_FragColor = passHologram(gl_FragCoord.xy);
}
