/*{
  "CATEGORIES": ["Generator", "Glitch", "Atmospheric", "Audio Reactive"],
  "DESCRIPTION": "Combined holographic / glitch shader. Four modes stack the best techniques from datamosh (persistent moshBuf frame-feedback + I-frame stutter + per-row tear + 8x8 DCT block corruption + RGB chroma split + bass burst garbage), hologram (scan-bar + cyan tint + parallax tilt + horizontal interlace), and foil (diffraction-grating rainbow palette + sparkle cells + paper grain + four foil patterns: linear / radial / crystal / glitter). Mode 3 stacks all three at lower intensity for a Pokemon-card-on-fire look.",
  "INPUTS": [
    { "NAME": "mode",             "LABEL": "Mode",              "TYPE": "long",
      "VALUES": [0,1,2,3], "LABELS": ["Datamosh","Hologram","Foil","Combined"], "DEFAULT": 3 },
    { "NAME": "foilPattern",      "LABEL": "Foil Pattern",      "TYPE": "long",
      "VALUES": [0,1,2,3], "LABELS": ["Linear Stripes","Radial","Crystal Embossed","Glitter Spray"], "DEFAULT": 2 },
    { "NAME": "stripeFreq",       "LABEL": "Pattern Frequency", "TYPE": "float", "MIN": 1.0,  "MAX": 60.0, "DEFAULT": 16.0 },
    { "NAME": "tiltSpeed",        "LABEL": "Tilt Speed",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.35 },
    { "NAME": "tiltAmount",       "LABEL": "Tilt Amount",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "sparkleDensity",   "LABEL": "Sparkle Density",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "sparkleSize",      "LABEL": "Sparkle Size",      "TYPE": "float", "MIN": 0.2,  "MAX": 3.0,  "DEFAULT": 1.0 },
    { "NAME": "paperGrainAmount", "LABEL": "Paper Grain",       "TYPE": "float", "MIN": 0.0,  "MAX": 0.6,  "DEFAULT": 0.18 },
    { "NAME": "hueRotateSpeed",   "LABEL": "Hue Rotate Speed",  "TYPE": "float", "MIN": -1.0, "MAX": 1.0,  "DEFAULT": 0.08 },
    { "NAME": "saturation",       "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.2 },

    { "NAME": "moshDirection",    "LABEL": "Mosh Direction",    "TYPE": "float", "MIN": 0.0,  "MAX": 6.2832, "DEFAULT": 0.0 },
    { "NAME": "moshStrength",     "LABEL": "Mosh Strength",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "moshPersistence",  "LABEL": "Mosh Persistence",  "TYPE": "float", "MIN": 0.85, "MAX": 0.999, "DEFAULT": 0.94 },
    { "NAME": "tearAmp",          "LABEL": "Tear Amount",       "TYPE": "float", "MIN": 0.0,  "MAX": 0.20, "DEFAULT": 0.06 },
    { "NAME": "rowDensity",       "LABEL": "Row Density",       "TYPE": "float", "MIN": 4.0,  "MAX": 60.0, "DEFAULT": 24.0 },
    { "NAME": "chroma",           "LABEL": "Chroma Split",      "TYPE": "float", "MIN": 0.0,  "MAX": 0.04, "DEFAULT": 0.014 },
    { "NAME": "blockSize",        "LABEL": "DCT Block Size",    "TYPE": "float", "MIN": 4.0,  "MAX": 32.0, "DEFAULT": 8.0 },
    { "NAME": "blockCorruption",  "LABEL": "Block Corruption",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.40 },
    { "NAME": "burstProb",        "LABEL": "Burst Probability", "TYPE": "float", "MIN": 0.0,  "MAX": 0.6,  "DEFAULT": 0.10 },
    { "NAME": "freezeChance",     "LABEL": "Freeze Chance",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.5,  "DEFAULT": 0.05 },

    { "NAME": "scanFreq",         "LABEL": "Scan Frequency",    "TYPE": "float", "MIN": 1.0,  "MAX": 4.0,  "DEFAULT": 2.0 },
    { "NAME": "scanBarSpeed",     "LABEL": "Scan Bar Speed",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6 },
    { "NAME": "interlaceAmount",  "LABEL": "Interlace",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "hologramTint",     "LABEL": "Hologram Tint",     "TYPE": "color", "DEFAULT": [0.4, 1.0, 0.95, 1.0] },
    { "NAME": "glow",             "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7 },

    { "NAME": "audioReact",       "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "bgColor",          "LABEL": "Background",        "TYPE": "color", "DEFAULT": [0.04, 0.05, 0.07, 1.0] },
    { "NAME": "resetField",       "LABEL": "Reset",             "TYPE": "bool",  "DEFAULT": false },
    { "NAME": "inputTex",         "LABEL": "Texture",           "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "moshBuf", "PERSISTENT": true },
    {}
  ]
}*/

// ============================================================
// Holographic Foil — combined glitch / hologram / foil shader
// Three lineages folded into one void main:
//   * Datamosh (Menkman / Murata / JODI) — persistent moshBuf frame-feedback,
//     I-frame stutter, per-row tear, RGB chroma split, 8x8 DCT block
//     corruption, bass-driven burst rainbow garbage.
//   * Hologram (Blade Runner 2049 / Nam June Paik) — sliding scan-bar,
//     cyan tint, horizontal interlace, parallax tilt, edge bloom.
//   * Foil (Pokemon-holo / chrome-vinyl / Milky-Way) — diffraction-grating
//     rainbow palette swept by tilt, four selectable patterns
//     (linear / radial / crystal / glitter), sparkle highlights, paper grain.
// Mode enum picks one lineage; mode 3 stacks all three at lower intensity.
// ============================================================

float hash11(float n)  { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)   { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2  hash22(vec2 p) {
    float a = hash21(p);
    float b = hash21(p + 17.13);
    return vec2(a, b);
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

vec3 quantize(vec3 c, float steps) { return floor(c * steps) / steps; }

vec3 hsv2rgb(vec3 c) {
    vec3 p = abs(fract(c.xxx + vec3(0.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0);
    return c.z * mix(vec3(1.0), clamp(p - 1.0, 0.0, 1.0), c.y);
}

vec3 satAdjust(vec3 c, float s) {
    float l = dot(c, vec3(0.299, 0.587, 0.114));
    return mix(vec3(l), c, s);
}

// -------- Foil pattern primitives --------
vec3 patternCrystal(vec2 uv, float freq, float tilt, float hueOff, float sat) {
    vec2 g = uv * freq;
    vec2 cell = floor(g);
    vec2 fr = fract(g);
    vec3 acc = vec3(0.0);
    float wsum = 0.0;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            vec2 c = cell + vec2(float(i), float(j));
            vec2 n2 = hash22(c) * 2.0 - 1.0;
            vec3 N = normalize(vec3(n2, 0.6));
            vec3 V = normalize(vec3(cos(tilt * 6.283), sin(tilt * 6.283), 0.85));
            float d = dot(N, V) * 0.5 + 0.5;
            float hue = fract(d + hueOff);
            vec3 col = hsv2rgb(vec3(hue, sat, 1.0));
            float spec = pow(d, 14.0);
            col += vec3(spec);
            float dist = length(fr - (vec2(float(i), float(j)) + 0.5));
            float w = exp(-dist * 1.6);
            acc += col * w;
            wsum += w;
        }
    }
    return acc / max(wsum, 1e-4);
}

vec3 patternGlitter(vec2 uv, float freq, float tilt, float hueOff, float sat, float size) {
    vec2 g = uv * freq;
    vec2 cell = floor(g);
    vec2 fr = fract(g) - 0.5;
    vec2 jitter = hash22(cell) - 0.5;
    float dist = length(fr - jitter * 0.7);
    float pref = hash21(cell + 7.31);
    float align = cos((tilt - pref) * 6.283) * 0.5 + 0.5;
    float dot_ = smoothstep(0.18 * size, 0.0, dist) * pow(align, 6.0);
    float hue = fract(pref + hueOff + tilt * 0.3);
    vec3 base = hsv2rgb(vec3(hue, sat * 0.7, 0.55));
    return base + vec3(dot_) * (0.9 + align * 1.3);
}

float sparkleLayer(vec2 uv, float density, float size, float tilt) {
    float N = 8.0 + density * 6.0;
    vec2 g = uv * N;
    vec2 cell = floor(g);
    vec2 fr = fract(g) - 0.5;
    vec2 j = hash22(cell + 91.7) - 0.5;
    float d = length(fr - j * 0.8);
    float r = mix(0.012, 0.04, size * 0.4);
    float phase = hash21(cell + 3.7);
    float prob  = hash21(cell + 19.4);
    if (prob > density) return 0.0;
    float align = pow(cos((tilt - phase) * 6.2831) * 0.5 + 0.5, 16.0);
    return smoothstep(r, 0.0, d) * align;
}

float paperGrain(vec2 uv) {
    float n = vnoise(uv * 220.0) * 0.5 + vnoise(uv * 90.0) * 0.5;
    return n - 0.5;
}

// -------- Procedural source for moshBuf when no inputTex bound --------
vec3 proceduralFresh(vec2 uv) {
    // Rotates through three glitch-art canon styles every ~8 sec.
    float era = mod(floor(TIME * 0.125), 3.0);
    if (era < 0.5) {
        // Murata Pink Dot (2007) — pulsing pink biomorphic blob.
        vec2 dC = vec2(0.5, 0.5)
                + 0.10 * vec2(sin(TIME * 0.40), cos(TIME * 0.32));
        vec2 dD = uv - dC;
        float angP = atan(dD.y, dD.x);
        float dR = 0.18 + 0.05 * sin(TIME * 0.70)
                        + 0.04 * sin(angP * 5.0 + TIME * 1.30);
        float blob = length(dD * vec2(1.4, 1.0)) - dR;
        float blobMask = smoothstep(0.02, -0.02, blob);
        return mix(vec3(0.04, 0.02, 0.06), vec3(0.95, 0.42, 0.78), blobMask);
    } else if (era < 1.5) {
        // Cory Arcangel "I Shot Andy Warhol" (2002) — 8-bit Warhol checker.
        vec2 q = floor(uv * vec2(28.0, 18.0) + vec2(floor(TIME * 0.6), 0.0));
        float chk = mod(q.x + q.y, 2.0);
        vec3 a = vec3(0.95, 0.55, 0.20);
        vec3 b = vec3(0.18, 0.08, 0.30);
        float flw = step(0.86, hash21(q + floor(TIME * 1.0)));
        return mix(chk < 0.5 ? a : b, vec3(0.95, 0.30, 0.60), flw);
    } else {
        // Rosa Menkman "Vernacular of File Formats" (2010) — codec bands.
        float bandY = floor(uv.y * 18.0 + TIME * 0.5);
        float bH = hash21(vec2(bandY, 0.0));
        float fx = fract(uv.x * (8.0 + bH * 16.0) + TIME * (0.2 + bH));
        return vec3(fract(bH * 3.7),
                    fract(bH * 5.3 + fx),
                    fract(bH * 7.1 + fx * 2.0));
    }
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // ============================================================
    // PASS 0 — moshBuf accumulation (datamosh persistent feedback)
    // Only meaningful for modes 0 (Datamosh) and 3 (Combined). For
    // modes 1/2 we still write something sensible so the buffer can
    // be sampled without producing a black frame.
    // ============================================================
    if (PASSINDEX == 0) {

        if (FRAMEINDEX < 2 || resetField) {
            vec3 init = (IMG_SIZE_inputTex.x > 0.0)
                      ? texture(inputTex, uv).rgb
                      : proceduralFresh(uv);
            gl_FragColor = vec4(init, 1.0);
            return;
        }

        // I-frame stutter — every ~2 sec, briefly hard-reset to a fresh
        // frame so the buffer doesn't saturate into mush.
        if (fract(TIME * 0.5) < 0.02) {
            vec3 fresh0 = (IMG_SIZE_inputTex.x > 0.0)
                        ? texture(inputTex, uv).rgb
                        : proceduralFresh(uv);
            gl_FragColor = vec4(fresh0, 1.0);
            return;
        }

        // Datamosh proper: previous frame pulled along synthesized motion.
        vec2 mDir = vec2(cos(moshDirection), sin(moshDirection))
                  * moshStrength
                  * (1.0 + audioBass * audioReact * 1.8);
        vec3 prev = texture(moshBuf, uv - mDir).rgb;

        float fr = hash21(floor(uv * vec2(rowDensity * 2.0, rowDensity))
                          + floor(TIME * 8.0));
        bool freeze = fr < freezeChance * (0.6 + audioMid * audioReact);

        vec3 fresh = (IMG_SIZE_inputTex.x > 0.0)
                   ? texture(inputTex, uv).rgb
                   : proceduralFresh(uv);

        vec3 outC = freeze ? mix(prev, fresh, 0.05)
                           : mix(fresh, prev, moshPersistence);
        gl_FragColor = vec4(outC, 1.0);
        return;
    }

    // ============================================================
    // PASS 1 — final output. Mode enum branches the stack.
    // ============================================================

    int M = int(mode);
    bool useDatamosh = (M == 0 || M == 3);
    bool useHologram = (M == 1 || M == 3);
    bool useFoil     = (M == 2 || M == 3);
    // Combined mode lowers intensities so the layers can coexist.
    float mix3 = (M == 3) ? 0.6 : 1.0;

    vec2 p = uv - 0.5;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;

    // -------- Tilt drives diffraction sweep + parallax.
    // TIME-only base; audioMid pushes the sweep on bass-light tracks.
    float tilt = TIME * tiltSpeed
               + audioMid * audioReact * 0.7
               + p.x * 0.4 + p.y * 0.18;
    tilt *= tiltAmount;
    float hueOff = TIME * hueRotateSpeed + audioBass * audioReact * 0.15;

    // ============================================================
    // FOIL BASELINE — diffraction palette + sparkle + paper grain.
    // ============================================================
    vec3 foil = bgColor.rgb;
    if (useFoil) {
        if (foilPattern == 0) {
            float h = uv.x * stripeFreq + tilt * 4.0;
            foil = hsv2rgb(vec3(fract(h * 0.06 + hueOff), saturation, 1.0));
            float stripe = 0.5 + 0.5 * cos(h * 6.2831 / 12.0);
            foil *= 0.7 + 0.3 * stripe;
        } else if (foilPattern == 1) {
            float h = length(uv - 0.5) * stripeFreq * 0.6 - tilt * 4.0;
            foil = hsv2rgb(vec3(fract(h * 0.08 + hueOff), saturation, 1.0));
            float ring = 0.5 + 0.5 * cos(h * 6.2831 / 8.0);
            foil *= 0.65 + 0.35 * ring;
        } else if (foilPattern == 2) {
            foil = patternCrystal(uv, stripeFreq * 0.4 + 4.0, tilt, hueOff, saturation);
        } else {
            foil = patternGlitter(uv, stripeFreq * 0.8 + 6.0, tilt, hueOff, saturation, sparkleSize);
        }
        // Sparkle cells — density modulated by audioHigh.
        float spkD = clamp(sparkleDensity * (0.7 + audioHigh * audioReact * 0.9), 0.0, 1.0);
        float spk = sparkleLayer(uv, spkD, sparkleSize, tilt);
        foil += vec3(spk) * vec3(1.0, 0.98, 0.92) * 1.6;
        foil += vec3(paperGrain(uv) * paperGrainAmount);
    }

    // ============================================================
    // DATAMOSH LAYER — pulled from persistent moshBuf with per-row
    // tearing, RGB chroma split, 8x8 DCT block corruption, burst.
    // ============================================================
    vec3 mosh = vec3(0.0);
    float moshAlpha = 0.0;
    if (useDatamosh) {
        // Per-row tearing, refreshed at 8 Hz, bass amplifies.
        float rowH = 1.0 / max(rowDensity, 1.0);
        float rowId = floor(uv.y / rowH);
        float tBucket = floor(TIME * (4.0 + audioBass * audioReact * 12.0));
        float tear = (hash21(vec2(rowId, tBucket)) - 0.5)
                   * tearAmp * (1.0 + audioMid * audioReact * 1.2);
        vec2 uvT = uv;
        uvT.x = fract(uvT.x + tear);

        // RGB chroma split — width grows with audioHigh.
        float chr = chroma * (1.0 + audioHigh * audioReact * 2.0);
        float r = texture(moshBuf, uvT + vec2( chr, 0.0)).r;
        float g = texture(moshBuf, uvT).g;
        float b = texture(moshBuf, uvT - vec2( chr, 0.0)).b;
        mosh = vec3(r, g, b);

        // 8x8 DCT-block corruption — random blocks averaged + quantized.
        float bs = max(blockSize, 1.0);
        vec2 blkPx = floor(gl_FragCoord.xy / bs) * bs;
        vec2 blkUV = blkPx / RENDERSIZE.xy;
        float blkRoll = hash21(blkPx + floor(TIME * 6.0));
        if (blkRoll < blockCorruption * (0.4 + audioLevel * audioReact)) {
            vec3 avg = texture(moshBuf, blkUV + vec2(bs, bs) / RENDERSIZE.xy * 0.5).rgb;
            mosh = quantize(avg, 4.0 + floor(blkRoll * 6.0));
        }

        // Burst rainbow garbage on bass — catastrophic-failure mode.
        float burstRoll = hash21(blkPx * 1.3 + floor(TIME * 12.0));
        if (burstRoll < burstProb * (0.30 + audioBass * audioReact * 0.7)) {
            mosh = vec3(hash11(burstRoll * 1.7),
                        hash11(burstRoll * 3.7),
                        hash11(burstRoll * 7.3));
        }
        moshAlpha = 1.0;
    }

    // ============================================================
    // BASE COMPOSITE — pick / blend foil and mosh per mode.
    // ============================================================
    vec3 col;
    if (M == 0) {
        col = mosh;
    } else if (M == 1) {
        // Hologram alone uses moshBuf as a clean source so the test
        // pattern still appears when no inputTex is bound.
        col = texture(moshBuf, uv).rgb;
    } else if (M == 2) {
        col = foil;
    } else {
        // Combined — foil base with mosh stamped on top at lowered alpha.
        col = mix(foil, mosh, 0.55 * mix3 * moshAlpha);
    }

    // ============================================================
    // HOLOGRAM TOP-PASS — scan-bar, interlace, cyan tint, bloom.
    // ============================================================
    if (useHologram) {
        // Vertical tear bands — independent of datamosh tear so it still
        // shows in pure hologram mode.
        float bandH = 0.04;
        float bandY = floor(uv.y / bandH) * bandH;
        float tearTrig = step(1.0 - 0.06 * (1.0 + audioBass * audioReact),
                              hash21(vec2(bandY, floor(TIME * 8.0))));
        vec2 uvH = uv;
        uvH.x += tearTrig * (hash21(vec2(bandY, TIME)) - 0.5) * 0.08;

        // Cyan tint with hologramTint user color.
        col *= mix(vec3(1.0), hologramTint.rgb, 0.55 * mix3);

        // Sliding scan-bar — bright moving stripe driven by audioMid.
        float barPos = fract(TIME * scanBarSpeed * (1.0 + audioMid * audioReact * 0.5));
        float bar = exp(-pow((uvH.y - barPos) * 8.0, 2.0));
        col += hologramTint.rgb * bar * 0.25 * mix3;

        // Static scanlines pinned to pixel grid.
        col *= 0.85 + 0.15 * sin(gl_FragCoord.y * scanFreq * 0.5);

        // Horizontal interlace — every other row dimmed slightly.
        float odd = mod(floor(gl_FragCoord.y), 2.0);
        col *= mix(1.0, mix(1.0, 0.78, odd), interlaceAmount);

        // Mid-band flicker.
        float flicker = 0.92 + 0.08 * sin(TIME * 60.0
                          + hash21(vec2(floor(TIME * 30.0), 0.0)) * 6.28);
        col *= mix(1.0, flicker, audioMid * audioReact * 0.4);

        // Edge bloom — high-luminance pixels glow beyond their position.
        float lum = dot(col, vec3(0.299, 0.587, 0.114));
        col += hologramTint.rgb * pow(lum, 1.4) * glow * 0.25 * mix3;
    }

    // ============================================================
    // FINISH — saturation, vignette, soft tonemap, audioLevel gain.
    // ============================================================
    col = satAdjust(col, 1.0 + (saturation - 1.0) * 0.3);

    float vig = smoothstep(1.15, 0.25, length(p));
    col = mix(bgColor.rgb, col, 0.85 + 0.15 * vig);

    // Soft Reinhard so sparkle / burst highlights don't clip ugly.
    col = col / (1.0 + col * 0.25);

    // Transmission strength — global brightness driven by audioLevel.
    col *= 0.6 + audioLevel * audioReact * 0.5;

    gl_FragColor = vec4(col, 1.0);
}
