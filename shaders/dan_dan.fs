/*{
  "CATEGORIES": ["Effect", "Glitch", "Audio Reactive"],
  "DESCRIPTION": "DAN DAN — Tachyon+ inspired circuit-bend signal mangler. Treats the input image as a video signal and corrupts it: RGB desync, horizontal sync tear, sample-and-hold blocks, bit-crush, color FM, luma frequency banding, dropout streaks, datamosh smear, vertical roll, bass-kicked channel inversion. GLITCH SEED selects which subset of bends fires (1024 distinct configs). PALETTE recolors with gradient mapping: Original / Purple / Mono / Cyberpunk / Corporate / Nightclub Red / Random. NEW GLITCH tap button jumps to a random seed. GLITCH HIT (momentary) slams Corruption + Circuit Bend to max while held — release returns to your slider values.",
  "INPUTS": [
    { "NAME": "intensity", "LABEL": "Corruption",   "TYPE": "float", "MIN": 0.0, "MAX": 1.5,    "DEFAULT": 0.6 },
    { "NAME": "frequency", "LABEL": "Signal Freq",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0,    "DEFAULT": 0.4 },
    { "NAME": "bend",      "LABEL": "Circuit Bend", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,    "DEFAULT": 0.5 },
    { "NAME": "seed",      "LABEL": "Glitch Seed",  "TYPE": "float", "MIN": 0.0, "MAX": 1024.0, "DEFAULT": 111.0 },
    { "NAME": "palette",   "LABEL": "Palette",      "TYPE": "long",
      "VALUES": [0, 1, 2, 3, 4, 5, 6],
      "LABELS": ["Original", "Purple", "Mono", "Cyberpunk", "Corporate", "Nightclub Red", "Random"],
      "DEFAULT": 0 },
    { "NAME": "newGlitch", "LABEL": "NEW GLITCH",   "TYPE": "event", "TARGET": "seed" },
    { "NAME": "glitchHit", "LABEL": "GLITCH HIT",   "TYPE": "event", "MOMENTARY": true },
    { "NAME": "audioReact","LABEL": "Audio React",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0,    "DEFAULT": 1.0 },
    { "NAME": "inputTex",  "LABEL": "Texture",      "TYPE": "image" }
  ]
}*/

// ---- hash / color helpers ----
float h11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float h21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2  h22(vec2 p)  {
    return fract(sin(vec2(dot(p, vec2(127.1, 311.7)),
                          dot(p, vec2(269.5, 183.3)))) * 43758.5453);
}
vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y); float e = 1e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}
float luma(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }

// ---- procedural fallback (cheap signal pattern) ----
vec3 testSignal(vec2 uv, float t) {
    vec2 q = fract(uv);
    vec3 col = 0.5 + 0.5 * cos(6.28318 * (vec3(0.0, 0.33, 0.67)
               + (q.x + q.y) * 0.9 + t * 0.07));
    col *= 0.72 + 0.28 * step(0.5, fract(q.x * 7.0));
    vec2 p1 = vec2(0.5 + 0.35 * sin(t * 0.7), 0.5 + 0.3 * cos(t * 0.9));
    col += vec3(1.0, 0.9, 0.7) * smoothstep(0.16, 0.0, length(q - p1)) * 0.8;
    return col;
}
vec3 src(vec2 p) {
    if (IMG_SIZE_inputTex.x > 0.5) return texture(inputTex, fract(p)).rgb;
    return testSignal(p, TIME);
}

// ---- seed-driven recipe gate ----
float onMod(float effSeed, float salt, float rarity, float bendK) {
    float g = h21(vec2(effSeed * 0.61803 + 11.0, salt * 1.7 + 3.7));
    float bar = mix(0.25, 0.85, rarity) - bendK * 0.45;
    return step(clamp(bar, 0.05, 0.95), g);
}
float par(float effSeed, float salt) {
    return h21(vec2(effSeed * 0.61803 + 91.0, salt * 1.7 + 19.0));
}

// ---- palette gradient map (luma -> 3-stop ramp), Random derived from seed ----
vec3 applyPalette(vec3 col, int mode, float seedF) {
    if (mode == 0) return col;                                         // Original
    float L = luma(col);
    vec3 a, b, c2;
    if      (mode == 1) { a = vec3(0.05, 0.0, 0.10); b = vec3(0.55, 0.10, 0.78); c2 = vec3(1.0, 0.75, 1.0); } // Purple
    else if (mode == 2) { a = vec3(0.0);             b = vec3(0.5);              c2 = vec3(1.0); }            // Mono
    else if (mode == 3) { a = vec3(0.05, 0.0, 0.20); b = vec3(0.90, 0.0, 0.62);  c2 = vec3(0.05, 0.95, 1.0);} // Cyberpunk
    else if (mode == 4) { a = vec3(0.0, 0.02, 0.08); b = vec3(0.0, 0.45, 0.82);  c2 = vec3(0.92, 0.98, 1.0);} // Corporate
    else if (mode == 5) { a = vec3(0.06, 0.0, 0.0);  b = vec3(0.85, 0.05, 0.12); c2 = vec3(1.0, 0.85, 0.55);} // Nightclub Red
    else {                                                                                                    // Random (seed-derived)
        a  = 0.5 + 0.5 * cos(6.28318 * (vec3(0.0, 0.33, 0.67) + seedF * 0.013));
        b  = 0.5 + 0.5 * cos(6.28318 * (vec3(0.0, 0.33, 0.67) + seedF * 0.027 + 2.0));
        c2 = 0.5 + 0.5 * cos(6.28318 * (vec3(0.0, 0.33, 0.67) + seedF * 0.041 + 4.0));
    }
    vec3 g = L < 0.5 ? mix(a, b, L * 2.0) : mix(b, c2, (L - 0.5) * 2.0);
    return mix(g, col, 0.15);                                          // 15% original chroma kept
}

// =====================================================================
void main() {
    float aR = clamp(audioReact, 0.0, 2.0);
    float aB = clamp(audioBass * aR, 0.0, 2.0);
    float aM = clamp(audioMid  * aR, 0.0, 2.0);
    float aH = clamp(audioHigh * aR, 0.0, 2.0);

    // GLITCH HIT (momentary): slam Corruption + Circuit Bend to max while held
    float hit = glitchHit ? 1.0 : 0.0;
    float I   = clamp(mix(intensity, 1.5, hit), 0.0, 1.5);
    float B   = clamp(mix(bend, 1.0, hit), 0.0, 1.0);
    float F   = clamp(frequency, 0.0, 1.0);
    float t   = TIME * (0.5 + F * 2.5);                                // signal time scaled by freq
    float effSeed = floor(seed + 0.5);

    vec2 uv0 = isf_FragNormCoord.xy;
    vec2 uv  = uv0;
    vec2 RES = RENDERSIZE.xy;
    vec2 px  = 1.0 / max(RES, vec2(1.0));

    // ===== UV-DOMAIN signal bends ===================================
    // [1] Horizontal sync tear — per-row x jitter
    if (onMod(effSeed, 1.0, 0.2, B) > 0.5) {
        float row = floor(uv.y * RES.y * 0.5);
        float d   = (h21(vec2(row, floor(t * (8.0 + aH * 22.0)))) - 0.5)
                  * (0.05 + 0.25 * par(effSeed, 1.0)) * I;
        uv.x = fract(uv.x + d);
    }
    // [2] Sample-and-hold blocks — freeze horizontal runs at quantized x
    if (onMod(effSeed, 2.0, 0.3, B) > 0.5) {
        float hold = mix(40.0, 8.0, par(effSeed, 2.0));
        float tb   = floor(t * mix(3.0, 16.0, par(effSeed, 2.0)));
        uv.x = (floor(uv.x * hold) + h21(vec2(floor(uv.x * hold) + tb,
                                             floor(uv.y * hold * 0.4)))) / hold;
    }
    // [3] Datamosh smear (broken P-frame pull)
    if (onMod(effSeed, 3.0, 0.4, B) > 0.5) {
        float bs = 22.0;
        vec2  id = floor(uv * bs);
        float tb = floor(t * 0.7);
        vec2  mv = (h22(id + tb * 13.0) - 0.5) * 2.0;
        uv -= mv * (0.012 + 0.045 * par(effSeed, 3.0)) * I;
    }
    // [4] Wave warp (signal distortion)
    if (onMod(effSeed, 4.0, 0.35, B) > 0.5) {
        float fr = mix(8.0, 36.0, par(effSeed, 4.0)) * (0.6 + F);
        uv.x += (0.006 + 0.04  * par(effSeed, 4.0)) * I * sin(uv.y * fr + t * 2.0);
        uv.y += (0.004 + 0.025 * par(effSeed, 4.0)) * I * cos(uv.x * fr * 1.2 + t * 1.7);
    }
    // [5] Vertical roll / jitter
    if (onMod(effSeed, 5.0, 0.55, B) > 0.5) {
        float r = (h11(floor(t * (2.0 + 8.0 * par(effSeed, 5.0)))) - 0.5) * 0.20 * I;
        uv.y = fract(uv.y + r);
    }

    // ===== SAMPLE with RGB channel desync ===========================
    float chr = mix(3.0, 18.0, par(effSeed, 10.0)) * px.x * I * (0.8 + aH * 1.2);
    float ang = par(effSeed, 10.0) * 6.28 + t * 0.23;
    vec2  dR  = vec2( cos(ang),         sin(ang)) * chr;
    vec2  dB  = vec2(-cos(ang * 1.13), -sin(ang * 1.13)) * chr;
    vec3 col = vec3(src(uv + dR).r, src(uv).g, src(uv + dB).b);

    // ===== COLOR-DOMAIN bends =======================================
    // [6] Bit-crush / posterize
    if (onMod(effSeed, 6.0, 0.35, B) > 0.5) {
        float lv = mix(16.0, 2.0, par(effSeed, 6.0));
        col = mix(col, floor(col * lv + 0.5) / lv, 0.6 + 0.4 * B);
    }
    // [7] Color FM — hue modulated by a carrier at Signal Freq
    if (onMod(effSeed, 7.0, 0.4, B) > 0.5) {
        vec3 hsv = rgb2hsv(col);
        float carrier = sin(t * mix(0.5, 9.0, F) + uv.x * mix(2.0, 20.0, par(effSeed, 7.0)));
        hsv.x = fract(hsv.x + carrier * mix(0.05, 0.4, par(effSeed, 7.0)) * I);
        hsv.y = clamp(hsv.y * (0.8 + 0.6 * B), 0.0, 1.0);
        col = hsv2rgb(hsv);
    }
    // [8] Frequency luma banding (FM brightness stripes)
    if (onMod(effSeed, 8.0, 0.45, B) > 0.5) {
        float band = 0.5 + 0.5 * sin(uv.y * mix(40.0, 220.0, par(effSeed, 8.0))
                                     + t * mix(2.0, 12.0, F));
        col *= mix(1.0, band, 0.35 * I);
    }
    // [9] Dropout — sparse white/black scanlines
    if (onMod(effSeed, 9.0, 0.5, B) > 0.5) {
        float ln = floor(uv0.y * RES.y);
        float fr = floor(t * 32.0);
        if (h21(vec2(ln * 0.137, fr)) > 0.985 - 0.02 * I) {
            col = (h21(vec2(ln, fr + 1.0)) > 0.5) ? vec3(0.95) : vec3(0.0);
        }
    }
    // [10] Bass-kicked channel invert burst
    if (onMod(effSeed, 11.0, 0.65, B) > 0.5) {
        float kick = aB * 1.5;
        if (h21(floor(uv0 * RES * 0.0625) + vec2(floor(t * 8.0))) < (0.05 + 0.10 * kick)) {
            col = vec3(1.0) - col;
        }
    }
    // Baseline static / signal noise (always alive)
    col += vec3(h21(floor(uv0 * RES) + vec2(floor(t * 40.0))) - 0.5) * 0.07 * I * (0.5 + aH);

    // ===== PALETTE gradient map =====================================
    col = applyPalette(col, palette, effSeed);

    // ===== HDR pop on glitch hit ====================================
    col += col * smoothstep(0.78, 1.1, luma(col)) * (0.4 + 0.5 * hit);

    // newGlitch uniform is UI-only (PropertyPanel uses the press to randomize 'seed');
    // reference it once so the compiler keeps it bound.
    if (newGlitch) col *= 1.0;

    gl_FragColor = vec4(max(col, vec3(0.0)), 1.0);
}
