/*{
  "CATEGORIES": ["Effect", "Glitch", "Audio Reactive", "Art Movement"],
  "DESCRIPTION": "DAN DAN × DE STIJL — Mondrian grid overlaid on a glitched signal. The grid partitions, repartitions, and marches coloured squares along its lines while the underlying image is corrupted with RGB desync, sync tears, datamosh, wave warp, bit-crush, luma banding, dropouts, and bass-kicked inversions. Grid lines and cells themselves glitch and drift with pixel displacement keyed to the circuit-bend controls. GLITCH HIT slams all corruption to max. PALETTE recolors the non-grid zones.",
  "INPUTS": [
    { "NAME": "intensity",           "LABEL": "Corruption",            "TYPE": "float", "MIN": 0.0, "MAX": 1.5,    "DEFAULT": 0.6 },
    { "NAME": "frequency",           "LABEL": "Signal Freq",           "TYPE": "float", "MIN": 0.0, "MAX": 1.0,    "DEFAULT": 0.4 },
    { "NAME": "bend",                "LABEL": "Circuit Bend",          "TYPE": "float", "MIN": 0.0, "MAX": 1.0,    "DEFAULT": 0.5 },
    { "NAME": "seed",                "LABEL": "Glitch Seed",           "TYPE": "float", "MIN": 0.0, "MAX": 1024.0, "DEFAULT": 111.0 },
    { "NAME": "palette",             "LABEL": "Palette",               "TYPE": "long",
      "VALUES": [0, 1, 2, 3, 4, 5, 6],
      "LABELS": ["Original", "Purple", "Mono", "Cyberpunk", "Corporate", "Nightclub Red", "Random"],
      "DEFAULT": 0 },
    { "NAME": "newGlitch",           "LABEL": "NEW GLITCH",            "TYPE": "event", "TARGET": "seed" },
    { "NAME": "glitchHit",           "LABEL": "GLITCH HIT",            "TYPE": "event", "MOMENTARY": true },
    { "NAME": "audioReact",          "LABEL": "Audio React",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0,    "DEFAULT": 1.0 },
    { "NAME": "inputTex",            "LABEL": "Texture",               "TYPE": "image" },
    { "NAME": "gridOpacity",         "LABEL": "Grid Opacity",          "TYPE": "float", "MIN": 0.0, "MAX": 1.0,    "DEFAULT": 0.72 },
    { "NAME": "lineThickness",       "LABEL": "Line Thickness",        "TYPE": "float", "MIN": 0.001, "MAX": 0.012, "DEFAULT": 0.0035 },
    { "NAME": "gridDensity",         "LABEL": "Grid Density",          "TYPE": "long",  "VALUES": [0, 1, 2, 3], "LABELS": ["Sparse", "Medium", "Dense", "Very Dense"], "DEFAULT": 1 },
    { "NAME": "repartitionPeriod",   "LABEL": "Repartition Period (s)","TYPE": "float", "MIN": 4.0,   "MAX": 30.0,  "DEFAULT": 12.0 },
    { "NAME": "colorPulseIntensity", "LABEL": "Color Pulse Intensity", "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 1.0 },
    { "NAME": "gridGlitch",          "LABEL": "Grid Glitch",           "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.45 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  SHARED HELPERS
// ════════════════════════════════════════════════════════════════════════
float h11(float n)  { return fract(sin(n * 12.9898) * 43758.5453); }
float h21(vec2 p)   { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float h12(vec2 p)   { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2  h22(vec2 p)   {
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

// ════════════════════════════════════════════════════════════════════════
//  DAN DAN — glitch helpers
// ════════════════════════════════════════════════════════════════════════
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
float onMod(float effSeed, float salt, float rarity, float bendK) {
    float g = h21(vec2(effSeed * 0.61803 + 11.0, salt * 1.7 + 3.7));
    float bar = mix(0.25, 0.85, rarity) - bendK * 0.45;
    return step(clamp(bar, 0.05, 0.95), g);
}
float par(float effSeed, float salt) {
    return h21(vec2(effSeed * 0.61803 + 91.0, salt * 1.7 + 19.0));
}
vec3 applyPalette(vec3 col, int mode, float seedF) {
    if (mode == 0) return col;
    float L = luma(col);
    vec3 a, b, c2;
    if      (mode == 1) { a = vec3(0.05, 0.0, 0.10); b = vec3(0.55, 0.10, 0.78); c2 = vec3(1.0, 0.75, 1.0); }
    else if (mode == 2) { a = vec3(0.0);              b = vec3(0.5);              c2 = vec3(1.0); }
    else if (mode == 3) { a = vec3(0.05, 0.0, 0.20);  b = vec3(0.90, 0.0, 0.62);  c2 = vec3(0.05, 0.95, 1.0); }
    else if (mode == 4) { a = vec3(0.0, 0.02, 0.08);  b = vec3(0.0, 0.45, 0.82);  c2 = vec3(0.92, 0.98, 1.0); }
    else if (mode == 5) { a = vec3(0.06, 0.0, 0.0);   b = vec3(0.85, 0.05, 0.12); c2 = vec3(1.0, 0.85, 0.55); }
    else {
        a  = 0.5 + 0.5 * cos(6.28318 * (vec3(0.0, 0.33, 0.67) + seedF * 0.013));
        b  = 0.5 + 0.5 * cos(6.28318 * (vec3(0.0, 0.33, 0.67) + seedF * 0.027 + 2.0));
        c2 = 0.5 + 0.5 * cos(6.28318 * (vec3(0.0, 0.33, 0.67) + seedF * 0.041 + 4.0));
    }
    vec3 g = L < 0.5 ? mix(a, b, L * 2.0) : mix(b, c2, (L - 0.5) * 2.0);
    return mix(g, col, 0.15);
}

// ════════════════════════════════════════════════════════════════════════
//  DE STIJL — Mondrian palette & grid helpers
// ════════════════════════════════════════════════════════════════════════
const vec3 PAL_RED    = vec3(0.85, 0.15, 0.10);
const vec3 PAL_BLUE   = vec3(0.10, 0.20, 0.65);
const vec3 PAL_YELLOW = vec3(0.96, 0.86, 0.20);
const vec3 PAL_WHITE  = vec3(0.96, 0.94, 0.88);
const vec3 PAL_BLACK  = vec3(0.04, 0.04, 0.06);

#define LINES_H 7
#define LINES_V 8
#define SQUARE 0.020

vec3 pal5(int idx) {
    if (idx == 0) return PAL_WHITE;
    if (idx == 1) return PAL_RED;
    if (idx == 2) return PAL_BLUE;
    if (idx == 3) return PAL_YELLOW;
    return PAL_BLACK;
}

int cellColour(vec2 cellId, float swapKey) {
    float r = h12(cellId * 1.7 + swapKey * 7.13);
    if (r < 0.62) return 0;
    if (r < 0.74) return 1;
    if (r < 0.84) return 2;
    if (r < 0.94) return 3;
    return 0;
}

float linePos(float idx, float total, float salt, float t, float period) {
    float slot   = (idx + 1.0) / (total + 1.0);
    float per    = max(period, 1.0);
    float epoch  = floor(t / per + salt * 0.31);
    float ph     = fract(t / per + salt * 0.31);
    float transStart = max(0.0, 1.0 - 1.0 / per);
    float trans  = smoothstep(transStart, 1.0, ph);
    float jitA   = (h11(epoch * 7.13 + salt) - 0.5) * 0.55 / total;
    float jitB   = (h11((epoch + 1.0) * 7.13 + salt) - 0.5) * 0.55 / total;
    float jit    = mix(jitA, jitB, trans);
    return clamp(slot + jit, 0.04, 0.96);
}

vec3 lineSquares(float along, float coord, float lineSalt, float t,
                 float mBass, float mTreble, float mAR, float pulseAmt,
                 float gGlitch)
{
    float tempoSel = h11(lineSalt * 17.7);
    float tempo    = (tempoSel < 0.25) ? 1.0
                   : (tempoSel < 0.55) ? 1.5
                   : (tempoSel < 0.85) ? 2.0 : 2.5;
    float dir      = (h11(lineSalt * 23.3) < 0.5) ? -1.0 : 1.0;
    // Glitch: tempo randomly stutters / doubles when gridGlitch is high
    float stutterT = floor(t * (2.0 + gGlitch * 12.0) * tempo);
    float stutter  = 1.0 + gGlitch * 2.0 * step(0.82, h11(lineSalt * 61.3 + stutterT));
    float baseSpd  = 0.075 + gGlitch * 0.22;
    float march    = t * baseSpd * tempo * stutter * dir;

    // Extra glitch: random phase snap
    float snapKey  = floor(t * (1.0 + gGlitch * 6.0) + lineSalt * 5.5);
    float snapAmt  = gGlitch * (h11(lineSalt * 79.3 + snapKey) - 0.5) * 0.4;
    march += snapAmt;

    float density  = 4.0 + floor(h11(lineSalt * 29.7) * 4.0);
    density       += gGlitch * 5.0 * mBass;
    density       *= clamp(pulseAmt, 0.0, 1.0);

    vec3 outCol = PAL_BLACK;

    for (int k = 0; k < 12; k++) {
        if (float(k) >= density) break;
        float fk    = float(k);
        float spawn = h11(lineSalt * 31.1 + fk * 5.7);
        // Glitch: individual square can jump position
        float jmpKey = floor(t * (3.0 + gGlitch * 9.0) + fk * 2.3 + lineSalt * 4.1);
        float jmp    = gGlitch * (h11(lineSalt * 47.9 + fk * 11.3 + jmpKey) - 0.5) * 0.35;
        float pos    = fract(march + spawn + jmp);
        float dA     = abs(along - pos);
        dA           = min(dA, 1.0 - dA);

        // Square size glitches
        float sqSize = SQUARE * (1.0 + gGlitch * 2.5 * h11(lineSalt * 93.1 + fk * 4.7 + floor(t * 4.0)));

        float colSel   = h11(lineSalt * 41.3 + fk * 7.1);
        float bassKick = mBass * mAR;
        float trebKick = mTreble * mAR;
        int sqIdx;
        if (colSel < 0.33 + 0.20 * bassKick)      sqIdx = 1;
        else if (colSel < 0.66 - 0.10 * trebKick) sqIdx = 2;
        else                                       sqIdx = 3;
        if (h11(lineSalt * 53.7 + fk * 3.3) < trebKick * 0.30) sqIdx = 3;
        // Glitch colour scramble
        if (gGlitch > 0.3 && h11(lineSalt * 71.1 + fk * 9.7 + floor(t * 8.0)) < gGlitch * 0.35) {
            sqIdx = int(floor(h11(lineSalt * 83.3 + fk * 6.6 + floor(t * 12.0)) * 4.0)) + 1;
            if (sqIdx > 3) sqIdx = 0;
        }

        if (dA < sqSize && abs(coord) < sqSize) {
            outCol = pal5(sqIdx);
        }
    }

    if (mBass * mAR > 0.35 && pulseAmt > 0.05) {
        float bspawn = fract(t * 0.22 + lineSalt * 1.11);
        float dA = abs(along - bspawn);
        dA = min(dA, 1.0 - dA);
        float sqSize2 = SQUARE * (1.0 + gGlitch * 1.5);
        if (dA < sqSize2 && abs(coord) < sqSize2) outCol = PAL_RED;
    }
    return outCol;
}

// ════════════════════════════════════════════════════════════════════════
//  MAIN
// ════════════════════════════════════════════════════════════════════════
void main() {
    float aR = clamp(audioReact, 0.0, 2.0);
    float aB = clamp(audioBass  * aR, 0.0, 2.0);
    float aM = clamp(audioMid   * aR, 0.0, 2.0);
    float aH = clamp(audioHigh  * aR, 0.0, 2.0);

    float hit = glitchHit ? 1.0 : 0.0;
    float I   = clamp(mix(intensity, 1.5, hit), 0.0, 1.5);
    float B   = clamp(mix(bend, 1.0, hit), 0.0, 1.0);
    float F   = clamp(frequency, 0.0, 1.0);
    float t   = TIME * (0.5 + F * 2.5);
    float tRaw= TIME;
    float effSeed = floor(seed + 0.5);
    float gG  = clamp(gridGlitch + hit * 0.6, 0.0, 1.0);

    vec2 uv0  = isf_FragNormCoord.xy;
    vec2 uv   = uv0;
    vec2 RES  = RENDERSIZE.xy;
    vec2 px   = 1.0 / max(RES, vec2(1.0));

    // ── EXTRA pixel-level displacement (new) ──────────────────────────
    // Pixel-level scanline jitter driven by audio high
    {
        float row = floor(uv.y * RES.y);
        float tFast = tRaw * (4.0 + aH * 18.0);
        float jLine = (h21(vec2(row * 0.073, floor(tFast))) - 0.5)
                      * 0.003 * (1.0 + aH * 3.0) * I;
        uv.x = fract(uv.x + jLine);
    }
    // Turbulent micro-warp keyed to audio mid
    {
        float mwAmp = 0.004 * I * (0.5 + aM * 1.5);
        uv.x += mwAmp * sin(uv.y * 210.0 + tRaw * 7.3 + aM * 4.0);
        uv.y += mwAmp * 0.7 * cos(uv.x * 190.0 + tRaw * 5.9 + aM * 3.2);
    }

    // ── DAN DAN UV bends ──────────────────────────────────────────────
    if (onMod(effSeed, 1.0, 0.2, B) > 0.5) {
        float row = floor(uv.y * RES.y * 0.5);
        float d   = (h21(vec2(row, floor(t * (8.0 + aH * 22.0)))) - 0.5)
                  * (0.05 + 0.25 * par(effSeed, 1.0)) * I;
        uv.x = fract(uv.x + d);
    }
    if (onMod(effSeed, 2.0, 0.3, B) > 0.5) {
        float hold = mix(40.0, 8.0, par(effSeed, 2.0));
        float tb   = floor(t * mix(3.0, 16.0, par(effSeed, 2.0)));
        uv.x = (floor(uv.x * hold) + h21(vec2(floor(uv.x * hold) + tb,
                                             floor(uv.y * hold * 0.4)))) / hold;
    }
    if (onMod(effSeed, 3.0, 0.4, B) > 0.5) {
        float bs = 22.0;
        vec2  id = floor(uv * bs);
        float tb = floor(t * 0.7);
        vec2  mv = (h22(id + tb * 13.0) - 0.5) * 2.0;
        uv -= mv * (0.012 + 0.045 * par(effSeed, 3.0)) * I;
    }
    if (onMod(effSeed, 4.0, 0.35, B) > 0.5) {
        float fr = mix(8.0, 36.0, par(effSeed, 4.0)) * (0.6 + F);
        uv.x += (0.006 + 0.04  * par(effSeed, 4.0)) * I * sin(uv.y * fr + t * 2.0);
        uv.y += (0.004 + 0.025 * par(effSeed, 4.0)) * I * cos(uv.x * fr * 1.2 + t * 1.7);
    }
    if (onMod(effSeed, 5.0, 0.55, B) > 0.5) {
        float r = (h11(floor(t * (2.0 + 8.0 * par(effSeed, 5.0)))) - 0.5) * 0.20 * I;
        uv.y = fract(uv.y + r);
    }

    // ── NEW: large-block glitch displacement ──────────────────────────
    {
        float blockSz = mix(0.08, 0.25, par(effSeed, 20.0));
        vec2  bid     = floor(uv / blockSz);
        float bT      = floor(tRaw * (2.0 + I * 5.0));
        float bTrig   = step(0.88 - I * 0.3, h21(bid + bT * 7.3));
        vec2  bOff    = (h22(bid + bT * 3.7) - 0.5) * blockSz * 2.5 * I * bTrig;
        uv = fract(uv + bOff);
    }

    // ── NEW: column-level vertical smear ─────────────────────────────
    {
        float col2  = floor(uv.x * RES.x * 0.12);
        float colT  = floor(tRaw * (1.5 + aB * 4.0));
        float smear = (h21(vec2(col2, colT)) - 0.5) * 0.18 * I * aB;
        uv.y = fract(uv.y + smear);
    }

    // ── RGB desync sample ─────────────────────────────────────────────
    float chr = mix(3.0, 18.0, par(effSeed, 10.0)) * px.x * I * (0.8 + aH * 1.2);
    float ang = par(effSeed, 10.0) * 6.28 + t * 0.23;
    vec2  dR  = vec2( cos(ang),          sin(ang)) * chr;
    vec2  dB  = vec2(-cos(ang * 1.13),  -sin(ang * 1.13)) * chr;
    // NEW: extra mid-channel desync drift
    vec2  dG  = vec2(sin(ang * 0.77), -cos(ang * 0.91)) * chr * 0.5 * aM;
    vec3 col  = vec3(src(uv + dR).r, src(uv + dG).g, src(uv + dB).b);

    // ── DAN DAN color bends ───────────────────────────────────────────
    if (onMod(effSeed, 6.0, 0.35, B) > 0.5) {
        float lv = mix(16.0, 2.0, par(effSeed, 6.0));
        col = mix(col, floor(col * lv + 0.5) / lv, 0.6 + 0.4 * B);
    }
    if (onMod(effSeed, 7.0, 0.4, B) > 0.5) {
        vec3 hsv = rgb2hsv(col);
        float carrier = sin(t * mix(0.5, 9.0, F) + uv.x * mix(2.0, 20.0, par(effSeed, 7.0)));
        hsv.x = fract(hsv.x + carrier * mix(0.05, 0.4, par(effSeed, 7.0)) * I);
        hsv.y = clamp(hsv.y * (0.8 + 0.6 * B), 0.0, 1.0);
        col = hsv2rgb(hsv);
    }
    if (onMod(effSeed, 8.0, 0.45, B) > 0.5) {
        float band = 0.5 + 0.5 * sin(uv.y * mix(40.0, 220.0, par(effSeed, 8.0))
                                     + t * mix(2.0, 12.0, F));
        col *= mix(1.0, band, 0.35 * I);
    }
    if (onMod(effSeed, 9.0, 0.5, B) > 0.5) {
        float ln = floor(uv0.y * RES.y);
        float fr = floor(t * 32.0);
        if (h21(vec2(ln * 0.137, fr)) > 0.985 - 0.02 * I) {
            col = (h21(vec2(ln, fr + 1.0)) > 0.5) ? vec3(0.95) : vec3(0.0);
        }
    }
    if (onMod(effSeed, 11.0, 0.65, B) > 0.5) {
        float kick = aB * 1.5;
        if (h21(floor(uv0 * RES * 0.0625) + vec2(floor(t * 8.0))) < (0.05 + 0.10 * kick)) {
            col = vec3(1.0) - col;
        }
    }
    // NEW: pixel-shimmer — high-frequency per-pixel color sparks
    {
        float sparkT = floor(tRaw * (15.0 + aH * 35.0));
        float spark  = h21(floor(uv0 * RES) + vec2(sparkT * 7.3, sparkT * 3.7));
        float sparkMask = step(1.0 - 0.04 * I * (0.5 + aH), spark);
        vec3 sparkCol   = hsv2rgb(vec3(h21(vec2(spark, sparkT)), 0.9, 1.0));
        col = mix(col, sparkCol, sparkMask);
    }
    col += vec3(h21(floor(uv0 * RES) + vec2(floor(t * 40.0))) - 0.5) * 0.07 * I * (0.5 + aH);

    // ── Palette map ───────────────────────────────────────────────────
    col = applyPalette(col, palette, effSeed);
    col += col * smoothstep(0.78, 1.1, luma(col)) * (0.4 + 0.5 * hit);

    // ════════════════════════════════════════════════════════════════
    //  DE STIJL GRID OVERLAY
    // ════════════════════════════════════════════════════════════════
    float lineW    = clamp(lineThickness, 0.001, 0.012);
    float period   = clamp(repartitionPeriod, 4.0, 30.0);
    float pulseAmt = clamp(colorPulseIntensity, 0.0, 1.0);
    float gOp      = clamp(gridOpacity, 0.0, 1.0);

    // Grid UV — apply its own glitch displacement so the grid itself shakes
    vec2 gUV = uv0;
    {
        // Grid-level horizontal sync jitter
        float grow = floor(gUV.y * RES.y * 0.25);
        float gT   = floor(tRaw * (3.0 + gG * 14.0));
        float gShift = (h21(vec2(grow * 0.23, gT)) - 0.5) * 0.012 * gG;
        gUV.x = fract(gUV.x + gShift);
        // Vertical roll
        float vroll = (h11(floor(tRaw * (1.0 + gG * 5.0) + 27.3)) - 0.5) * 0.06 * gG;
        gUV.y = fract(gUV.y + vroll);
        // Wave warp
        gUV.x += 0.005 * gG * sin(gUV.y * 28.0 + tRaw * 3.7);
        gUV.y += 0.004 * gG * cos(gUV.x * 24.0 + tRaw * 2.9);
    }

    int dIdx = int(clamp(float(gridDensity), 0.0, 3.0) + 0.5);
    int activeLinesV = (dIdx == 0) ? 3 : (dIdx == 1) ? 5 : (dIdx == 2) ? 6 : 8;
    int activeLinesH = (dIdx == 0) ? 2 : (dIdx == 1) ? 4 : (dIdx == 2) ? 5 : 7;

    // Mondrian-style audio: pseudo-bass/treble or real audio
    float mBass   = clamp(audioBass   * aR, 0.0, 2.0);
    float mTreble = clamp(audioHigh   * aR, 0.0, 2.0);
    // Supplement with TIME when audio is quiet
    mBass   = max(mBass,   0.25 + 0.25 * sin(tRaw * 1.7));
    mTreble = max(mTreble, 0.25 + 0.25 * sin(tRaw * 4.3 + 1.3));

    float swapKey = floor(tRaw / 7.0 + mBass * 0.5);

    float xs[LINES_V];
    for (int j = 0; j < LINES_V; j++) {
        xs[j] = linePos(float(j), float(activeLinesV),
                        float(j) * 1.31 + 11.0, tRaw, period);
        // Glitch: occasional snap to random position
        float snapT = floor(tRaw * (0.5 + gG * 4.0) + float(j) * 3.7);
        float snap  = step(1.0 - gG * 0.3, h11(float(j) * 17.3 + snapT));
        xs[j] = mix(xs[j], h11(float(j) * 31.7 + snapT * 2.1), snap);
    }
    float ys[LINES_H];
    for (int i = 0; i < LINES_H; i++) {
        ys[i] = linePos(float(i), float(activeLinesH),
                        float(i) * 1.71 + 27.0, tRaw, period);
        float snapT = floor(tRaw * (0.5 + gG * 4.0) + float(i) * 5.1);
        float snap  = step(1.0 - gG * 0.3, h11(float(i) * 19.7 + snapT));
        ys[i] = mix(ys[i], h11(float(i) * 37.3 + snapT * 2.1), snap);
    }

    // Cell identification using glitched UV
    int cellX = 0;
    for (int j = 0; j < LINES_V; j++) {
        if (j < activeLinesV && gUV.x > xs[j]) cellX = j + 1;
    }
    int cellY = 0;
    for (int i = 0; i < LINES_H; i++) {
        if (i < activeLinesH && gUV.y > ys[i]) cellY = i + 1;
    }

    // Cell fill
    int cIdx = cellColour(vec2(float(cellX), float(cellY)), swapKey);
    vec3 gridCol = pal5(cIdx);

    // Horizontal lines
    for (int i = 0; i < LINES_H; i++) {
        if (i >= activeLinesH) break;
        float dy  = gUV.y - ys[i];
        float ady = abs(dy);
        // Glitch: line thickness pulses
        float lw2 = lineW * (1.0 + gG * 3.0 * aB * h11(float(i) * 23.1 + floor(tRaw * 6.0)));
        if (ady < lw2) gridCol = PAL_BLACK;
        if (ady < SQUARE * (1.0 + gG)) {
            float salt = float(i) * 11.13 + 3.7;
            vec3 sq = lineSquares(gUV.x, dy, salt, tRaw,
                                  mBass, mTreble, aR, pulseAmt, gG);
            if (sq != PAL_BLACK) gridCol = sq;
        }
    }

    // Vertical lines
    for (int j = 0; j < LINES_V; j++) {
        if (j >= activeLinesV) break;
        float dx  = gUV.x - xs[j];
        float adx = abs(dx);
        float lw2 = lineW * (1.0 + gG * 3.0 * aB * h11(float(j) * 29.3 + floor(tRaw * 6.0)));
        if (adx < lw2) gridCol = PAL_BLACK;
        if (adx < SQUARE * (1.0 + gG)) {
            float salt = float(j) * 13.71 + 17.3;
            vec3 sq = lineSquares(gUV.y, dx, salt, tRaw,
                                  mBass, mTreble, aR, pulseAmt, gG);
            if (sq != PAL_BLACK) gridCol = sq;
        }
    }

    // Frame border
    if (gUV.x < lineW || gUV.x > 1.0 - lineW
     || gUV.y < lineW || gUV.y > 1.0 - lineW) {
        gridCol = PAL_BLACK;
    }

    // Glitch: invert grid cell colour on beat
    if (gG > 0.2 && mBass > 0.8) {
        float invKey = floor(tRaw * (2.0 + gG * 6.0));
        if (h21(vec2(float(cellX) + invKey, float(cellY) * 3.7)) < gG * mBass * 0.25) {
            gridCol = vec3(1.0) - gridCol;
        }
    }

    // Composite: grid over glitch signal
    // On grid lines (black) use full opacity; on colored cells use gridOpacity
    float isLine = step(gridCol.r + gridCol.g + gridCol.b, 0.25);
    float blendW = mix(gOp, 1.0, isLine * 0.7);
    col = mix(col, gridCol, blendW);

    // HDR pop on glitch hit
    col += col * smoothstep(0.78, 1.1, luma(col)) * hit * 0.6;

    if (newGlitch) col *= 1.0;

    gl_FragColor = vec4(max(col, vec3(0.0)), 1.0);
}