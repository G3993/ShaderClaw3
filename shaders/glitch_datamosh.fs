/*{
  "CATEGORIES": ["Filter", "Glitch", "Audio Reactive"],
  "DESCRIPTION": "Glitch / Datamosh — one continuous compound state, all glitch vocabulary layered: heavy chromatic aberration, drifting pixel-sort bands, broken P-frame motion smears, bass-triggered macroblock corruption, CRT scanlines + barrel curvature, a wandering VHS tape-roll slash, and periodic posterize/hue-rotate cataclysms. After Rosa Menkman's DCT:SYPHONING (2018), Takeshi Murata's Monster Movie (2005), JODI's %80%80~404~ (2001), Cory Arcangel's Super Mario Clouds, and Bill Etra video-synth tape decay. Asymmetric VHS magenta cast in shadows + cyan-lime in highlights, channel-clipped corruptions pushed past 1.0 linear so they GLOW under bloom. Operates on inputTex, falls back to Etra-pattern color bars + bouncing rectangle. Output linear HDR.",
  "INPUTS": [
    { "NAME": "intensity",     "LABEL": "Intensity",         "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.92 },
    { "NAME": "tearRate",      "LABEL": "Tear Rate",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "posterizeKick", "LABEL": "Posterize Kick",    "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0 },
    { "NAME": "audioReact",    "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "inputTex",      "LABEL": "Texture",           "TYPE": "image" }
  ]
}*/

// =====================================================================
//  Glitch / Datamosh — ONE STATE, all vocabulary layered, always-on.
//  Glitch art is not a button. It is a condition the signal lives in.
//
//  Layers (every frame, no switching):
//    - Heavy chromatic aberration (treble-shift)
//    - Drifting pixel-sort bands (3-6 horizontal, vertical drift)
//    - Datamosh smearing (slow time-shifted UV pull, broken P-frames)
//    - Bass-kick macroblock corruption (8x8 hue-rotate / channel invert)
//    - CRT barrel curvature + scanlines
//    - VHS tape-roll line (sliding luma slash + comet trail)
//    - Cursed cataclysms (~5-10s posterize-3 OR hue-rotate-90)
//    - VHS asymmetric tint (magenta shadow / cyan-lime highlight)
//    - HDR boost on hot pixels for bloom pickup
// =====================================================================

// ─── hash / noise ────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2  hash22(vec2 p)  {
    return fract(sin(vec2(dot(p, vec2(127.1, 311.7)),
                          dot(p, vec2(269.5, 183.3)))) * 43758.5453);
}

// ─── color helpers ───────────────────────────────────────────────────
vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}
float luma(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }

// ─── Etra fallback — color bars + bouncing rectangle + grain ─────────
vec3 etraPattern(vec2 uv, float t) {
    vec2 q = fract(uv);
    // SMPTE-ish 7-bar color field
    float bar = floor(q.x * 7.0);
    vec3 bars[7];
    bars[0] = vec3(0.75, 0.75, 0.75); // gray
    bars[1] = vec3(0.75, 0.75, 0.00); // yellow
    bars[2] = vec3(0.00, 0.75, 0.75); // cyan
    bars[3] = vec3(0.00, 0.75, 0.00); // green
    bars[4] = vec3(0.75, 0.00, 0.75); // magenta
    bars[5] = vec3(0.75, 0.00, 0.00); // red
    bars[6] = vec3(0.00, 0.00, 0.75); // blue
    int bi = int(clamp(bar, 0.0, 6.0));
    vec3 col = bars[bi];

    // Lower band: pluge-ish black-white step
    if (q.y < 0.25) {
        col = (q.x < 0.5) ? vec3(0.04) : vec3(0.92);
    }

    // Bouncing rectangle — Etra video-synth signature, slow ricochet
    vec2 bp = vec2(0.5 + 0.4 * sin(t * 1.30),
                   0.5 + 0.35 * cos(t * 0.97 + 1.0));
    vec2 d  = abs(q - bp);
    if (d.x < 0.07 && d.y < 0.05) {
        col = vec3(1.0, 0.55, 0.10) + 0.4 * sin(vec3(t, t + 1.0, t + 2.0) * 3.0);
    }

    // Salt-pepper grain
    float g = step(0.985, hash21(floor(q * 320.0) + floor(t * 8.0)));
    col += vec3(g) * 0.6;
    return col;
}

vec3 sourceSample(vec2 uv, float t) {
    if (IMG_SIZE_inputTex.x > 0.0) {
        return texture(inputTex, fract(uv)).rgb;
    }
    return etraPattern(uv, t);
}

// ─── Datamosh source — sample with slow time-shifted UV pull. The
//     vector survives across many frames so motion gets dragged like a
//     broken P-frame. Used as a "feedback" replacement everywhere.
vec3 moshSample(vec2 uv, float t, float aBass) {
    float bs    = 22.0;
    vec2  bId   = floor(uv * bs);
    float tB    = floor(t * 0.65);
    vec2  mv    = (hash22(bId + tB * 13.0) - 0.5) * 2.0;
    float stick = step(0.82, hash21(bId + tB * 7.0));
    float amp   = 0.022 * (1.0 + aBass * 1.6) * (1.0 + stick * 5.0);
    vec2  pull  = mv * amp;
    vec3  c1    = sourceSample(uv - pull, t);
    vec3  c2    = sourceSample(uv - pull * 0.5, t);
    // Screen-blend the half-pull so smear reads as motion-trail
    return mix(c1, vec3(1.0) - (vec3(1.0) - c1) * (vec3(1.0) - c2), 0.32);
}

// ─── Pixel-sort band — Asendorf horizontal smear within a band ───────
//     Returns (color, mask). Mask is 0 outside any active band.
vec4 pixelSortLayer(vec2 uv, float t, float aMid) {
    // 5-10 bands, drifting vertically. Each band has its own anchor row.
    float bandCount = 5.0 + floor(hash11(floor(t * 0.28)) * 6.0); // 5..10
    float coverage  = 0.0;
    vec3  acc       = vec3(0.0);
    for (int i = 0; i < 10; i++) {
        if (float(i) >= bandCount) break;
        float fi    = float(i);
        // Drift band center vertically over time
        float driftSpeed = 0.04 + 0.06 * hash11(fi * 7.13);
        float yC    = fract(hash11(fi * 11.7) + t * driftSpeed * (1.0 + aMid));
        float thick = 0.035 + 0.06 * hash11(fi * 3.7 + 1.3);
        float dy    = abs(uv.y - yC);
        float m     = 1.0 - smoothstep(thick * 0.6, thick, dy);
        if (m <= 0.0) continue;

        // Anchor — brightest sample drawn from a deterministic x
        float seed   = hash11(fi * 17.3 + floor(t * (1.4 + aMid * 3.5)));
        float dir    = (seed > 0.5) ? 1.0 : -1.0;
        float anchorX = fract(seed * 1.7 + t * 0.05);
        vec3  anchor = sourceSample(vec2(anchorX, yC), t);
        float anchorL = luma(anchor);

        vec3  here   = sourceSample(uv, t);
        float L      = luma(here);
        float thresh = 0.30 + 0.40 * hash11(fi * 5.1);
        float reach  = 0.18 + aMid * 0.18;
        float dx     = (uv.x - anchorX) * dir;
        float fall   = smoothstep(reach, 0.0, abs(dx));
        float doSmear = step(L, thresh) * step(L, anchorL) * fall;

        vec3  bandCol = mix(here, anchor, doSmear);
        acc += bandCol * m;
        coverage += m;
    }
    if (coverage <= 0.0) return vec4(0.0);
    return vec4(acc / coverage, clamp(coverage, 0.0, 1.0));
}

// ─── Macroblock corruption — bass kick triggers 8x8 hue-rotate +
//     channel invert across ~20% of canvas. Returns (color, mask).
vec4 macroblockLayer(vec2 uv, float t, float aBass) {
    float bs    = 8.0;
    vec2  px    = uv * RENDERSIZE.xy;
    vec2  blkPx = floor(px / bs) * bs;
    vec2  blkUV = (blkPx + bs * 0.5) / RENDERSIZE.xy;

    // Bass kick — sharp rising edge, decays over ~0.3s
    float kickSeed = hash21(blkPx + floor(t * 14.0));
    float kickEnv  = clamp(aBass * 1.6 + 0.18, 0.0, 1.7);
    // Baseline ~28% coverage even in silence — bass pushes to ~55%
    float thresh   = 0.28 + kickEnv * 0.32;
    if (kickSeed > thresh) return vec4(0.0);

    vec3 src = sourceSample(blkUV, t);
    vec3 hsv = rgb2hsv(src);
    hsv.x    = fract(hsv.x + kickSeed * 1.7 + 0.30);
    hsv.y    = clamp(hsv.y * (0.7 + kickSeed * 1.3), 0.0, 1.0);
    hsv.z    = clamp(hsv.z * (0.85 + kickSeed * 0.6), 0.0, 1.6);
    vec3 col = hsv2rgb(hsv);

    // Channel inversion on a sub-fraction of the corrupted blocks
    if (kickSeed < thresh * 0.30) col = vec3(1.0) - col;

    // Push hot pixels above 1.0 — these are the bloom feeders
    col *= 1.0 + kickEnv * 0.6;

    return vec4(col, 1.0);
}

// ─── VHS rolling tape line — thin sharp white scanline that flickers,
//     replacing the old fat rolling band. ~1-2px equivalent, intermittent.
vec3 vhsRoll(vec3 col, vec2 uv, float t, float tear) {
    // Slow vertical sweep, occasional fast snap-back.
    float phase = fract(t * 0.22 * (1.0 + tear * 0.3));
    float yLine = phase;
    // Sharp 1-2px line — pixel-thin via RENDERSIZE
    float pxY   = 1.0 / max(RENDERSIZE.y, 1.0);
    float dy    = uv.y - yLine;
    float thinPx = abs(dy) / pxY; // distance in pixels
    float line   = exp(-pow(thinPx * 0.8, 2.0));
    // Flicker — gate the line with random on/off so it stutters in/out
    float flick  = step(0.55, hash11(floor(t * 12.0)));
    float roll   = line * flick;
    // Cold sharp white, hot enough to feed bloom — but no fat trail.
    return col + vec3(1.30, 1.30, 1.35) * roll * 1.2;
}

// ─── Sync-loss tear — lateral row displacement, treble-driven ─────────
vec2 syncTearUV(vec2 uv, float t, float tear, float aHigh) {
    float row1 = floor(uv.y * 200.0);
    float row2 = floor(uv.y * 38.0);
    float tBkt = floor(t * (4.0 + aHigh * 14.0));
    float h1   = hash21(vec2(row1, tBkt));
    float h2   = hash21(vec2(row2, tBkt * 0.5));
    float disp = (h1 - 0.5) * tear * 0.10
               + (h2 - 0.5) * tear * (0.20 + aHigh * 0.30);
    return vec2(fract(uv.x + disp), uv.y);
}

// ─── CRT barrel curvature ─────────────────────────────────────────────
vec2 barrel(vec2 uv) {
    vec2 c = uv - 0.5;
    float r2 = dot(c, c);
    c *= 1.0 + 0.06 * r2 + 0.04 * r2 * r2;
    return c + 0.5;
}

// ─── main ─────────────────────────────────────────────────────────────
void main() {
    vec2  uvRaw = isf_FragNormCoord.xy;
    float t     = TIME;
    float aR    = clamp(audioReact, 0.0, 2.0);
    float aBass = clamp(audioBass * aR, 0.0, 2.0);
    float aMid  = clamp(audioMid  * aR, 0.0, 2.0);
    float aHigh = clamp(audioHigh * aR, 0.0, 2.0);
    float intens = clamp(intensity, 0.0, 1.5);

    // CRT barrel + outside-bezel mask. Glitch art on a curved tube.
    vec2 uv = barrel(uvRaw);
    float bezel = step(0.0, uv.x) * step(uv.x, 1.0)
                * step(0.0, uv.y) * step(uv.y, 1.0);

    // Sync-tear UV displacement (always on, treble-modulated)
    float tearAmt = (0.04 + 0.06 * intens) * (0.6 + tearRate);
    vec2 uvT = syncTearUV(uv, t, tearAmt, aHigh);

    // ---- Base: datamosh-smeared source (broken P-frame baseline) ----
    vec3 base = moshSample(uvT, t, aBass);

    // ---- Heavy chromatic aberration: 4-10px scaling with treble -----
    // RENDERSIZE in pixels — convert to UV. Treble pushes shift.
    float pxToUV = 1.0 / max(RENDERSIZE.x, 1.0);
    float chr    = (8.0 + 12.0 * (0.5 + aHigh * 1.4)) * pxToUV * (0.8 + intens * 0.8);
    // Direction wobbles slowly so split isn't pure horizontal
    float ang = t * 0.23;
    vec2  dR  = vec2( cos(ang),  sin(ang)) * chr;
    vec2  dB  = vec2(-cos(ang * 1.13), -sin(ang * 1.13)) * chr;
    float r   = moshSample(uvT + dR, t, aBass).r;
    float g   = base.g;
    float b   = moshSample(uvT + dB, t, aBass).b;
    vec3  col = vec3(r, g, b);

    // ---- Pixel-sort bands (3-6, drifting) ---------------------------
    vec4 sortL = pixelSortLayer(uvT, t, aMid);
    col = mix(col, sortL.rgb, sortL.a * (0.85 + 0.15 * intens));

    // ---- Macroblock corruption (bass-kick events) -------------------
    vec4 mb = macroblockLayer(uvT, t, aBass);
    col = mix(col, mb.rgb, mb.a);

    // ---- VHS rolling tape-line --------------------------------------
    col = vhsRoll(col, uv, t, tearRate);

    // ---- Cursed cataclysms: every 2.5-5s, 0.3-0.6s posterize-3 OR
    //      hue-rotate-90. Audio + hash gating, smooth envelope.
    float period = mix(2.5, 5.0, hash11(floor(t / 3.7)));
    float bkt    = floor(t / period);
    float seed   = hash11(bkt * 19.07);
    float dur    = 0.30 + 0.30 * hash11(bkt * 3.1);
    float phaseS = fract(t / period) * period; // seconds into current bucket
    float env    = smoothstep(0.0, 0.05, phaseS) * smoothstep(dur, dur - 0.08, phaseS);
    float gate   = step(0.20, seed) * step(0.05, aBass + aMid * 0.5 + 0.45);
    float evt    = env * gate * posterizeKick;
    if (evt > 0.001) {
        // Two flavors, picked by seed
        if (seed > 0.72) {
            // Hue-rotate 90deg + saturation lift
            vec3 hsv = rgb2hsv(col);
            hsv.x    = fract(hsv.x + 0.25);
            hsv.y    = clamp(hsv.y * 1.4, 0.0, 1.0);
            col      = mix(col, hsv2rgb(hsv), evt);
        } else {
            // Posterize-to-3 colors per channel
            vec3 p = floor(col * 3.0) / 3.0;
            // Push posterize highlights past 1.0 so bloom tags them
            p *= 1.0 + 0.45 * step(0.66, p);
            col = mix(col, p, evt);
        }
    }

    // ---- Scanline pattern (CRT) — soft horizontal modulation --------
    float scan = 0.92 + 0.08 * sin(uv.y * RENDERSIZE.y * 3.14159);
    col *= scan;
    // Bright scanline highlights pushed past 1.0 (bloom feeders)
    float scanHi = smoothstep(0.99, 1.00, scan);
    col += vec3(0.15, 0.18, 0.14) * scanHi;

    // ---- VHS asymmetric tint (THE key cast) -------------------------
    //      Magenta-purple in shadows, cyan-lime on highlights.
    float L      = luma(col);
    float shadow = 1.0 - smoothstep(0.0, 0.42, L);
    float hi     = smoothstep(0.55, 1.10, L);
    vec3  tShadow = mix(vec3(1.0), vec3(1.05, 0.92, 0.98), shadow);
    vec3  tHigh   = mix(vec3(1.0), vec3(0.96, 1.04, 0.94), hi);
    col *= tShadow * tHigh;

    // ---- Baseline corruption (alive in silence) --------------------
    float baseline = 0.05 * (hash21(floor(uvRaw * RENDERSIZE.xy * 0.5)
                                    + floor(t * 5.0)) - 0.5);
    col += vec3(baseline);
    // Treble-driven VHS dust — sparse bright specks
    float dust = step(0.997 - aHigh * 0.012,
                      hash21(floor(uvRaw * RENDERSIZE.xy * 0.7) + floor(t * 26.0)));
    col += vec3(1.15, 1.05, 0.95) * dust * 1.4; // hot specks for bloom

    // ---- HDR boost — push hot pixels into bloom range --------------
    float Lf = luma(col);
    float boost = smoothstep(0.78, 1.10, Lf);
    col += col * boost * 1.0;
    // Channel-clip residue: any single channel above 1.0 gets extra heat
    vec3  over = max(col - vec3(1.0), vec3(0.0));
    col += over * 0.85;
    // Macroblock corruption bursts get an additional bloom kick
    if (mb.a > 0.5) {
        float mbL = luma(mb.rgb);
        col += mb.rgb * smoothstep(0.7, 1.2, mbL) * 0.6;
    }

    // ---- Bezel: outside curved CRT goes black ----------------------
    col *= bezel;

    gl_FragColor = vec4(max(col, vec3(0.0)), 1.0);
}
