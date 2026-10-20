/*{
  "DESCRIPTION": "Watercolor — a real wet-on-wet watercolor simulation that paints a whole painting on every sheet of cold-press paper. Water lives only on wet paper; the rim of each wash dries first and drifts the pigment outward into crisp tide lines, charges dropped into a wet wash feather and bleed along a divergence-free flow, clean water dropped into a drying wash grows cauliflower back-runs, and the finest pigment settles into the valleys of the paper tooth as granulation. The autonomous painter alternates Frankenthaler-scale gestural abstracts — three or four huge sweeping washes or poured pools per sheet, each in a harmonious triad (indanthrone · crimson · turquoise, orange · crimson · violet, turquoise · indanthrone · orange…), charged wet-in-wet, bloomed with clean water, finished with a few calligraphic dark accents and spatter, white paper breathing between them — with a loose atmospheric landscape: a graded sky with a lifted glow, sky blooms and soft clouds, mountain ranges dropped into the still-wet wash so they feather, and a warm ground. Pigments mix subtractively (Beer-Lambert) with a density cap so layered glazes stay luminous, the sheet cockles and its tooth catches a raking light, wet passages carry a moving gloss, and when a painting is finished a fresh sheet slides in over it with a drop shadow. Bass deepens the glaze, mids stir the flow, beats throw bigger spatter, highs sparkle the wet gloss.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #6",
  "CATEGORIES": ["Generator", "Simulation", "Painterly", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Painting Speed",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "sheetTime",   "LABEL": "Seconds per Sheet", "TYPE": "float", "AUDIO": false, "MIN": 30.0, "MAX": 120.0, "DEFAULT": 48.0, "GROUP": "Speed" },
    { "NAME": "composition", "LABEL": "Composition (cycle · landscape · gestural)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.0, "GROUP": "Speed" },
    { "NAME": "waterLoad",   "LABEL": "Water Load",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Water" },
    { "NAME": "flow",        "LABEL": "Flow / Feathering", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Water" },
    { "NAME": "dryRate",     "LABEL": "Drying",            "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Water" },
    { "NAME": "blooms",      "LABEL": "Blooms / Back-runs","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Water" },
    { "NAME": "spatter",     "LABEL": "Spatter",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Water" },
    { "NAME": "pigment",     "LABEL": "Pigment Load",      "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Pigment" },
    { "NAME": "edgeDark",    "LABEL": "Edge Darkening",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Pigment" },
    { "NAME": "granulation", "LABEL": "Granulation",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Pigment" },
    { "NAME": "pig1",        "LABEL": "Pigment 1 (Indanthrone)", "TYPE": "color", "DEFAULT": [0.04, 0.07, 0.4, 1.0], "GROUP": "Color" },
    { "NAME": "pig2",        "LABEL": "Pigment 2 (Perylene Crimson)", "TYPE": "color", "DEFAULT": [0.66, 0.02, 0.14, 1.0], "GROUP": "Color" },
    { "NAME": "pig3",        "LABEL": "Pigment 3 (Phthalo Turquoise)", "TYPE": "color", "DEFAULT": [0.0, 0.4, 0.5, 1.0], "GROUP": "Color" },
    { "NAME": "pig4",        "LABEL": "Pigment 4 (Pyrrol Orange)", "TYPE": "color", "DEFAULT": [0.97, 0.36, 0.02, 1.0], "GROUP": "Color" },
    { "NAME": "pig5",        "LABEL": "Pigment 5 (Neutral Tint)", "TYPE": "color", "DEFAULT": [0.1, 0.1, 0.14, 1.0], "GROUP": "Color" },
    { "NAME": "paperColor",  "LABEL": "Paper",             "TYPE": "color", "DEFAULT": [0.975, 0.95, 0.895, 1.0], "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",   "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" },
    { "NAME": "paperTexture","LABEL": "Paper Tooth",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "cockling",    "LABEL": "Paper Cockling",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "lightAz",     "LABEL": "Light Direction",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 360.0, "DEFAULT": 135.0, "GROUP": "Light" },
    { "NAME": "wetSheen",    "LABEL": "Wet Sheen",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.05, "GROUP": "Finish" },
    { "NAME": "simRes",      "LABEL": "Sim Resolution",    "TYPE": "float", "AUDIO": false, "MIN": 0.35, "MAX": 1.0, "DEFAULT": 0.75, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" }
  ],
  "PASSES": [
    { "TARGET": "paintBuf", "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "wetBuf", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "pigBuf", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "depBuf", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Watercolor — FRONTIER pack #6
//   pass 0  paintBuf: what the autonomous painter lays this frame (rgb pigment, a water; wet = water > 0)
//   pass 1  wetBuf : r water depth · gb flow (texels/frame) · a paper wetness
//   pass 2  pigBuf : rgb SUSPENDED pigment (optical density) riding the water
//   pass 3  depBuf : rgb DEPOSITED pigment (optical density) settled in the paper
//   pass 4  final  : Beer-Lambert glaze over cold-press paper (tooth + cockling
//                    under a raking light), granulation, wet gloss, sheet-change
//                    shadow, bloom → ACES → HD finisher
//   Physics (after Curtis et al. 1997, simplified to one pass per layer):
//   water lives only on wet paper and diffuses there; the rim of a wash dries
//   first and the wetness gradient drifts pigment outward (edge darkening /
//   tide lines); a divergence-free curl flow + fractal diffusivity feathers the
//   pigment; water entering DAMP paper grows a stochastic cauliflower front
//   (back-runs) and lifts settled colour; deposits prefer the tooth's valleys.
//   The painter paints a PAINTING per sheet — landscape (graded wet-in-wet sky,
//   lifted sun, cloud charges, mountains dropped into the damp wash, dark
//   conifers), botanical (negative-painted background, charged flower heads,
//   calligraphic stems, leaves) or gestural (huge charged sweeping washes,
//   dark accents, spatter) — then a fresh sheet slides in over it.
// ─────────────────────────────────────────────────────────────────────────

#define R     RENDERSIZE.xy
#define TAU   6.28318530718
#define PI    3.14159265359
#define SLIDE 2.6
#define START 11.5     // the first sheet opens mid-painting: earlier layers are seeded already dry

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 5; i++) { s += a * vnoise(p); p = mat2(1.6, 1.2, -1.2, 1.6) * p + 7.3; a *= 0.5; }
    return s;
}
float fbm3(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 3; i++) { s += a * vnoise(p); p = mat2(1.6, 1.2, -1.2, 1.6) * p + 7.3; a *= 0.5; }
    return s;
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

// ── audio (smooth by default) ────────────────────────────────────────────
float g_ar;
float bassP() { return g_ar * pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()  { return g_ar * smoothstep(0.06, 0.85, audioMid); }
float highP() { return g_ar * smoothstep(0.10, 0.90, audioHigh); }
float beatP() { return g_ar * clamp(audioBeatPulse, 0.0, 1.0); }

// ── paper: cold-press tooth → 0..1 height (offset per sheet) ─────────────
vec2 g_paperOff = vec2(0.0);
float paperH(vec2 p) {
    vec2 q = (p + g_paperOff) * 46.0;
    float b = vnoise(q) * 0.55 + vnoise(q * 2.1 + 3.1) * 0.28 + vnoise(q * 4.3 - 1.7) * 0.17;
    float f = vnoise(vec2(q.x * 3.5 + q.y * 0.7, q.y * 0.35) * 1.3) * 0.5 + vnoise(vec2(q.y * 3.2 - q.x * 0.9, q.x * 0.3) * 1.2) * 0.5;
    return clamp(b * 0.78 + f * 0.22, 0.0, 1.0);
}
// fine tooth (1–3 px): where granulating pigment settles
float paperFine(vec2 p) { vec2 q = (p + g_paperOff) * 46.0; return vnoise(q * 4.3 - 1.7) * 0.55 + vnoise(q * 9.1 + 4.4) * 0.45; }
// low-passed tooth for the half-res sim (the full tooth aliases into streaks at sim resolution)
float paperLo(vec2 p) { vec2 q = (p + g_paperOff) * 46.0; return vnoise(q) * 0.7 + vnoise(q * 0.5 + 9.1) * 0.3; }
// gentle large-scale waviness of a wetted sheet
float cockle(vec2 p) { return fbm3((p + g_paperOff * 0.37) * 1.6); }

// ── pigments as optical density (Beer-Lambert): K = -ln(transmittance) ────
vec3 pigCol(float idx) {
    vec3 c = pig1.rgb;
    if (idx > 0.5) c = pig2.rgb;
    if (idx > 1.5) c = pig3.rgb;
    if (idx > 2.5) c = pig4.rgb;
    if (idx > 3.5) c = pig5.rgb;
    return c;
}
vec3 pigK(float idx) { return -log(clamp(pigCol(idx), 0.012, 0.985)); }

// ── sheet clock ──────────────────────────────────────────────────────────
float g_t, g_dt, g_asp, g_n, g_u, g_tp, g_edge, g_sliding, g_seed;
void setup() {
    g_ar = audioReact;
    g_asp = R.x / R.y;
    float L = max(sheetTime, 30.0);
    // the first sheet opens with its first wash already laid (seeded at frame 0)
    float T = TIME * speed + SLIDE + START;
    g_t = T;
    g_dt = clamp(TIMEDELTA, 0.0, 0.05) * speed;
    g_n = floor(T / L);
    g_u = T - g_n * L;
    g_tp = g_u - SLIDE;                      // the painter's clock on this sheet
    float k = clamp(g_u / SLIDE, 0.0, 1.0);
    k = k * k * (3.0 - 2.0 * k);
    g_sliding = (g_n > 0.5 && g_u < SLIDE) ? 1.0 : 0.0;
    g_edge = mix(g_asp + 0.08, -0.08, k);    // leading edge of the incoming sheet
    g_seed = 0.0;
}
// the incoming sheet's deckled, slightly skewed leading edge
float edgeAt(vec2 p) { return g_edge + 0.035 * (p.y - 0.5) + 0.004 * (vnoise(vec2(p.y * 40.0, g_n)) - 0.5); }
vec2 sheetOff(float n) { return vec2(hash11(n * 1.7 + 0.3), hash11(n * 2.9 + 0.7)) * 40.0; }
// 0 = landscape, 1 = gestural. The cycle is gestural · landscape · gestural (gestural varies every sheet)
float planOf(float n) {
    if (composition < 0.5) return mod(n, 3.0) == 1.0 ? 0.0 : 1.0;
    return composition < 1.5 ? 0.0 : 1.0;
}

// ── painter primitives ───────────────────────────────────────────────────
vec2 P_pos;
struct Paint { float water; vec3 pig; float lift; float wet; vec3 dry; };

float rag(vec2 p, float s) { return (fbm3(p * 16.0 + s) - 0.5) * 0.028 + (vnoise(p * 95.0 + s) - 0.5) * 0.007; }
float edgeM(float sd) { return smoothstep(-0.0028, 0.0028, sd); }        // sd > 0 inside

// a wash dragged across its region: param s (0..1) says when the loaded brush passes this point
void sweepIn(inout Paint P, float m, float s, float t0, float dur, vec3 K, float pa, float wa) {
    if (m < 0.001) return;
    float sc = 1.0;
    if (g_seed > 0.5) {
        if (t0 > 0.01) { if (t0 + dur < START) P.dry += K * pa * m * pigment * 14.0; return; }
        sc = 22.0;
    }
    else {
        s += 0.16 * (fbm3(P_pos * 2.2 + t0) - 0.5) + 0.05 * (fbm3(P_pos * 9.0 - t0) - 0.5);      // a ragged wet front, never a straight line
        float a = g_tp - (t0 + s * dur);
        if (a < 0.0 || a > 0.42) return;
    }
    P.water += wa * m * waterLoad * (1.0 + 0.25 * bassP()) * sc;
    P.pig += K * pa * m * pigment * sc;
    P.wet = max(P.wet, m);
}
// a loaded brush touched into one spot (wet-in-wet charge)
void chargeIn(inout Paint P, float m, float t0, float len, vec3 K, float pa, float wa) {
    if (m < 0.001) return;
    if (g_seed > 0.5) { if (t0 + len < START) P.dry += K * pa * m * pigment * 9.0; return; }
    float a = g_tp - t0;
    if (a < 0.0 || a > len) return;
    P.water += wa * m * waterLoad;
    P.pig += K * pa * m * pigment;
    P.wet = max(P.wet, m);
}
// clean water + tissue: lifts pigment back to paper white
void liftIn(inout Paint P, float m, float t0, float len) {
    if (m < 0.001 || g_seed > 0.5) return;
    float a = g_tp - t0;
    if (a < 0.0 || a > len) return;
    P.water += 0.008 * m;
    P.lift += 0.16 * m;
    P.wet = max(P.wet, m);
}
// domain-warped organic blob, <1 inside
float blob(vec2 p, vec2 c, float r, float seed, float rough) {
    vec2 d = p - c;
    float sq = 1.0 + 0.35 * hash11(seed * 1.7);
    float an = TAU * hash11(seed * 3.3);
    vec2 dr = mat2(cos(an), sin(an), -sin(an), cos(an)) * d;
    vec2 q = d / r * 1.3 + vec2(hash11(seed * 5.1), hash11(seed * 7.9)) * 37.0;
    vec2 warp = vec2(fbm3(q), fbm3(q + vec2(5.2, 1.3))) - 0.5;
    warp = warp * 1.25 + (vec2(fbm3(q * 2.9 + 3.3), fbm3(q * 2.9 + 8.1)) - 0.5) * 0.45;
    warp += rough * 0.18 * (vec2(fbm3(p * 22.0 + seed), fbm3(p * 22.0 - seed)) - 0.5);
    return length(dr * vec2(1.0, sq) / r + warp * 0.9);
}
// arc around O (radius Rr) starting at angle a0, signed span → (distance to arc line, param f)
vec2 arcDF(vec2 p, vec2 O, float Rr, float a0, float span) {
    vec2 d = p - O;
    float a = atan(d.y, d.x) - a0;
    a = mod(a + PI, TAU) - PI;
    return vec2(abs(length(d) - Rr), a / span);
}
void spatterIn(inout Paint P, vec2 p, float t0, float seed, vec3 K, float size) {
    if (g_seed > 0.5) return;
    float a = g_tp - t0;
    if (a < 0.0 || a > 0.12 || spatter < 0.01) return;
    vec2 h = hash22(vec2(seed, 4.2));
    vec2 c0 = vec2(0.15 + h.x * (g_asp - 0.3), 0.15 + h.y * 0.7);
    vec2 dir = normalize(hash22(vec2(seed, 8.8)) - 0.5);
    float big = (0.005 + 0.01 * hash11(seed * 2.7)) * size * (0.8 + 1.4 * beatP()) * (0.5 + 0.5 * spatter);
    float acc = 0.0;
    for (int j = 0; j < 12; j++) {
        float fj = float(j);
        vec2 hj = hash22(vec2(seed, fj + 1.0));
        vec2 o = dir * (hj.x * 0.32) + vec2(-dir.y, dir.x) * (hj.y - 0.5) * 0.06 * (1.0 + hj.x * 2.5);
        float r = big * (1.5 - hj.x) * (0.35 + 0.65 * hash11(seed + fj * 3.3));
        float d = length(p - c0 - o * step(0.5, fj));
        acc = max(acc, 1.0 - smoothstep(r * 0.85, r, d));
    }
    P.pig += K * acc * 0.2 * pigment;
    P.water += acc * 0.03;
    P.wet = max(P.wet, acc);
}

// ═════════════════════════════════════════════════════════════════════════
//  PLAN A — LANDSCAPE
// ═════════════════════════════════════════════════════════════════════════
void planLandscape(vec2 p, float n, inout Paint P) {
    float tp = g_tp;
    float hs = hash11(n * 3.3 + 1.0);
    float hz = 0.3 + 0.08 * hash11(n * 5.1 + 2.0);
    vec3 Ktop = pigK(0.0), Kmid = pigK(1.0), Klow = pigK(3.0);                  // sunset
    if (hs > 0.5) { Klow = mix(pigK(1.0), pigK(3.0), 0.4); Kmid = mix(pigK(1.0), pigK(0.0), 0.45); Ktop = mix(pigK(0.0), pigK(2.0), 0.3); }   // violet dusk
    vec2 sun = vec2(g_asp * (0.25 + 0.5 * hash11(n * 7.7)), hz + 0.12 + 0.12 * hash11(n * 8.8));
    float sunD = length((p - sun) * vec2(0.8, 1.15)) / 0.1;
    // 1 · graded wet-in-wet sky, the light source only thinned (a lifted glow, no hard disk)
    if (tp < 6.0 || g_seed > 0.5) {
        float m = 1.0;
        float ty = clamp((p.y - hz) / (1.0 - hz), 0.0, 1.0) + 0.12 * (fbm3(p * 2.5 + n) - 0.5);
        vec3 K = mix(Klow, Kmid, smoothstep(0.0, 0.45, ty));
        K = mix(K, Ktop, smoothstep(0.4, 0.95, ty));
        K *= mix(0.1, 1.0, smoothstep(0.25, 2.6, sunD + 0.6 * (fbm3(p * 6.0 + n) - 0.5)));
        sweepIn(P, m, clamp(p.x / g_asp * 0.8 + (1.0 - p.y) * 0.25, 0.0, 1.0), 0.0, 4.5, K, 0.045, 0.011);
    }
    // 2 · sky blooms: clean water dropped into the damp sky → soft cauliflower back-runs; a few soft cloud charges
    if (tp > 5.0 && tp < 10.0 || g_seed > 0.5) {
        for (int j = 0; j < 6; j++) {
            float fj = float(j);
            vec2 h = hash22(vec2(n * 9.1 + fj, 3.7));
            vec2 c = vec2(g_asp * (0.05 + 0.9 * h.x), hz + 0.25 + 0.65 * (1.0 - hz) * h.y);
            float r = 0.06 + 0.07 * hash11(n + fj * 4.3);
            float m = 1.0 - smoothstep(0.85, 1.0, blob(c + (p - c) * vec2(0.6, 1.4), c, r, n * 5.0 + fj, 1.0));
            if (mod(fj, 2.0) < 0.5) chargeIn(P, m, 5.4 + fj * 0.6, 0.2, pigK(0.0), 0.0, 0.018);
            else chargeIn(P, m * smoothstep(0.8, 1.6, sunD), 5.4 + fj * 0.6, 0.2, mix(Kmid, Ktop, 0.6), 0.05, 0.014);
        }
    }
    // 3 · far range dropped into the still-wet lower sky → it feathers up into it
    if (tp > 4.4 && tp < 7.5 || g_seed > 0.5) {
        float top = hz + 0.03 + 0.3 * (fbm(vec2(p.x * 1.3 + n * 3.1, 0.5)) - 0.38);
        float m = edgeM(top - p.y);
        sweepIn(P, m, p.x / g_asp, 4.6, 2.2, mix(Ktop, Kmid, 0.35), 0.035, 0.014);
    }
    // 4 · middle range while the sky is damp: softer edge than wet-on-dry
    if (tp > 7.5 && tp < 11.5 || g_seed > 0.5) {
        float top = hz - 0.06 + 0.2 * (fbm(vec2(p.x * 2.2 + n * 1.3, 4.2)) - 0.42);
        float m = edgeM(top - p.y + rag(p, n + 2.0) * 0.3);
        sweepIn(P, m, 1.0 - p.x / g_asp, 7.8, 3.0, mix(pigK(0.0), Kmid, 0.3), 0.035, 0.013);
    }
    // 5 · near ground, warm glow into indigo, wet-on-dry
    if (tp > 14.0 && tp < 19.0 || g_seed > 0.5) {
        float top = hz - 0.15 + 0.07 * (fbm(vec2(p.x * 3.6 + n, 8.0)) - 0.5);
        float m = edgeM(top - p.y + rag(p, n + 3.0) * 0.5);
        vec3 K = mix(mix(Klow, pigK(1.0), 0.5), pigK(0.0), smoothstep(top - 0.02, top - 0.2, p.y) * 0.8 + 0.2 * (fbm3(p * 4.0 + n) - 0.5));
        sweepIn(P, m, p.x / g_asp, 14.5, 3.5, K, 0.05, 0.013);
    }
    spatterIn(P, p, 30.0, n * 3.0 + 1.0, mix(Kmid, pigK(0.0), 0.5), 0.7);
}

// ═════════════════════════════════════════════════════════════════════════
//  PLAN B — GESTURAL (Frankenthaler-scale washes, the star)
// ═════════════════════════════════════════════════════════════════════════
// harmonious triads (never more than three pigments per sheet; neutral tint only for accents)
void triad(float h, out vec3 Ka, out vec3 Kb, out vec3 Kc) {
    // Kb is the bridge: Ka and Kc only ever meet through it (no complementary mud)
    if (h < 0.34)      { Ka = pigK(2.0); Kb = pigK(0.0); Kc = pigK(1.0); }   // phthalo turquoise · indanthrone · crimson-rose
    else if (h < 0.67) { Ka = pigK(3.0); Kb = pigK(1.0); Kc = pigK(0.0); }   // pyrrol orange · crimson · indanthrone
    else               { Ka = pigK(0.0); Kb = pigK(2.0); Kc = mix(pigK(2.0), pigK(3.0), 0.35); }   // indanthrone · turquoise · phthalo green
}
vec3 triPick(float i, vec3 Ka, vec3 Kb, vec3 Kc) {
    float k = mod(i, 3.0);
    return k < 0.5 ? Ka : (k < 1.5 ? Kb : Kc);
}
void gestureArc(float n, float i, out vec2 O, out float Rr, out float a0, out float span, out float w) {
    vec2 h = hash22(vec2(n * 3.7 + i * 1.9, 2.2));
    // stratified across the sheet so the washes compose instead of piling up
    float cnt = 3.0 + floor(hash11(n * 2.9) * 1.99);
    float k = mod(i + floor(hash11(n * 4.4) * cnt), cnt);
    vec2 T = vec2(g_asp * ((k + 0.5 + 0.5 * (h.x - 0.5)) / cnt), 0.25 + 0.5 * h.y);
    Rr = 0.45 + 0.8 * hash11(n * 4.1 + i);
    float dirA = TAU * hash11(n * 9.3 + i * 2.7);
    O = T - Rr * vec2(cos(dirA), sin(dirA));
    span = (1.1 + 0.9 * hash11(n + i * 5.0)) / Rr * (hash11(i + n * 0.7) > 0.5 ? 1.0 : -1.0);
    a0 = dirA - span * 0.5;
    w = 0.1 + 0.11 * hash11(n * 6.1 + i);
}
void planGestural(vec2 p, float n, inout Paint P) {
    float tp = g_tp;
    vec3 Ka, Kb, Kc;
    triad(hash11(n * 1.37 + 0.2), Ka, Kb, Kc);
    float count = 3.0 + floor(hash11(n * 2.9) * 1.99);
    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        if (fi >= count) break;
        float t0 = fi * 6.5;
        if (g_seed < 0.5 && (tp < t0 || tp > t0 + 6.4)) continue;
        float rot = floor(hash11(n * 6.7) * 3.0);
        float ki = mod(fi + rot, 3.0);
        vec3 K1 = triPick(ki, Ka, Kb, Kc);
        vec3 K2 = triPick(ki < 0.5 ? 1.0 : (ki < 1.5 ? (hash11(n + fi) > 0.5 ? 0.0 : 2.0) : 1.0), Ka, Kb, Kc);   // Kb sits between Ka and Kc
        vec2 O; float Rr, a0, span, w;
        gestureArc(n, fi, O, Rr, a0, span, w);
        float kind = hash11(n * 5.3 + fi * 7.1);          // < .2 pour with runs · < .38 thin wet calligraphy · else big gesture
        if (fi < 0.5) kind = 0.6;                        // the first mark is always a big gesture
        float m = 0.0, f = 0.0;
        float am = a0 + span * 0.5;
        vec2 cc0 = O + Rr * vec2(cos(am), sin(am));
        if (kind < 0.2) {
            // a poured puddle, then gravity runs dripping down from its lower rim
            float bs = blob(p, cc0, w * 1.6, n * 3.0 + fi, 1.0);
            m = 1.0 - smoothstep(0.96, 1.0, bs);
            f = clamp(dot(p - cc0, vec2(0.8, 0.6)) / (w * 3.2) + 0.5, 0.0, 1.0);
            for (int k = 0; k < 4; k++) {
                float fk = float(k);
                float x0 = cc0.x + (hash11(fk + fi * 3.0 + n) - 0.5) * w * 2.2;
                float y0 = cc0.y - w * 0.9;
                float L = 0.08 + 0.22 * hash11(fk * 5.0 + n + fi);
                float s = (y0 - p.y) / L;
                if (s < 0.0 || s > 1.0) continue;
                float dw = (0.004 + 0.007 * hash11(fk + n)) * (1.0 - 0.6 * s) + 0.0015;
                float bead = 0.006 * smoothstep(0.85, 1.0, s);   // the bead at the end of the run
                float sd = dw + bead - abs(p.x - x0 - 0.004 * sin(s * 9.0 + fk));
                float md = edgeM(sd);
                if (g_seed > 0.5) md *= 0.0;
                sweepIn(P, md, s, t0 + 1.4 + fk * 0.3, 2.2, K1, 0.06, 0.012);
            }
        } else {
            vec2 df = arcDF(p, O, Rr, a0, span);
            f = df.y;
            if (f > -0.02 && f < 1.02) {
                float fc = clamp(f, 0.0, 1.0);
                // loaded start → pressure swell → dry, broken finish; width wanders along the path
                float prof = smoothstep(0.0, 0.06, fc) * (0.7 + 0.45 * sin(PI * pow(fc, 0.75))) * (1.0 - 0.6 * smoothstep(0.55, 1.0, fc));
                prof *= 0.75 + 0.5 * fbm3(vec2(fc * 3.0, fi + n * 1.7));
                float ww = w * prof * (kind < 0.38 ? 0.16 : 1.0);
                float signedD = length(p - O) - Rr;
                m = edgeM(ww - df.x + rag(p, fi + n) * (kind < 0.38 ? 0.25 : 1.0));
                // dry-brush streaks: toward the end the brush only catches the tooth, in streaks along the stroke
                float across = signedD / max(ww, 1e-3);
                float streak = vnoise(vec2(across * 7.0, fi * 13.0 + n)) * 0.6 + paperH(p) * 0.4;
                float dryness = smoothstep(0.6, 1.0, fc);
                m *= step(dryness * 0.75, streak);
            }
        }
        if (m < 0.001) continue;
        vec3 K = mix(K1, K2, smoothstep(0.3, 0.8, clamp(f, 0.0, 1.0) + 0.35 * (fbm3(p * 2.2 + fi) - 0.5)) * 0.85);
        sweepIn(P, m, clamp(f, 0.0, 1.0), t0, kind < 0.38 && kind >= 0.2 ? 1.2 : 2.8, K, kind < 0.38 && kind >= 0.2 ? 0.09 : 0.05, 0.013);
        if (g_seed > 0.5 || (kind < 0.38 && kind >= 0.2)) continue;
        // charge the wet wash with its neighbour pigment, then clean-water drops for back-runs
        for (int j = 0; j < 3; j++) {
            float fj = float(j);
            float aC = a0 + span * (0.2 + 0.3 * fj);
            vec2 cc = mix(O + Rr * vec2(cos(aC), sin(aC)), cc0, 0.25) + (hash22(vec2(fi, fj + n)) - 0.5) * w;
            float mC = (1.0 - smoothstep(0.88, 1.0, blob(p, cc, w * (0.4 + 0.35 * hash11(fj + fi + n)), n + fi * 4.0 + fj, 1.2))) * m;
            chargeIn(P, mC, t0 + 1.5 + fj * 0.5, 0.2, mix(K2, K1, 0.25), 0.09, 0.013);
            vec2 cw = cc + (hash22(vec2(fj + 3.0, fi + n)) - 0.5) * w * 1.5;
            float mW = (1.0 - smoothstep(0.85, 1.0, blob(p, cw, w * (0.3 + 0.3 * hash11(fj * 2.0 + fi)), n + fi * 9.0 + fj, 1.5))) * m;
            chargeIn(P, mW, t0 + 3.6 + fj * 0.45, 0.2, K1, 0.0, 0.02 * blooms);
        }
    }
    if (g_seed > 0.5) return;
    // a few calligraphic dark accents
    float accents = 1.0 + floor(hash11(n * 8.1) * 2.99);
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        if (fi >= accents) break;
        float t0 = count * 6.5 + 1.0 + fi * 1.6;
        if (tp < t0 || tp > t0 + 1.5) continue;
        vec2 O; float Rr, a0, span, w;
        gestureArc(n + 17.0, fi, O, Rr, a0, span, w);
        span *= 0.6;
        vec2 df = arcDF(p, O, Rr, a0, span);
        if (df.y < 0.0 || df.y > 1.0) continue;
        float ww = (0.003 + 0.008 * hash11(fi + n)) * (1.0 - 0.7 * df.y) * (0.6 + 0.8 * vnoise(vec2(df.y * 12.0, fi))) + 0.0012;
        sweepIn(P, edgeM(ww - df.x), df.y, t0, 1.1, mix(pigK(4.0), Kb, 0.35), 0.13, 0.01);
    }
    float ts = count * 6.5 + 4.0;
    spatterIn(P, p, ts, n * 7.0 + 1.0, Ka, 1.1);
    spatterIn(P, p, ts + 0.7, n * 7.0 + 2.0, Kb, 1.0);
    spatterIn(P, p, ts + 1.5, n * 7.0 + 3.0, Kc, 0.8);
}

Paint painter(vec2 p) {
    P_pos = p;
    Paint P; P.water = 0.0; P.pig = vec3(0.0); P.lift = 0.0; P.wet = 0.0; P.dry = vec3(0.0);
    if (g_sliding > 0.5 || g_tp < 0.0) return P;
    if (planOf(g_n) < 0.5) planLandscape(p, g_n, P);
    else planGestural(p, g_n, P);
    return P;
}

// isotropic 8-tap ring of paper wetness (wash boundary detector)
float wetRing(vec2 uv, vec2 e, float rad) {
    float acc = 0.0;
    for (int i = 0; i < 8; i++) {
        float an = float(i) * 0.785398 + 0.3927;
        acc += texture2D(wetBuf, uv + vec2(cos(an), sin(an)) * e * rad).a;
    }
    return acc * 0.125;
}
float rimOf(vec2 uv, vec2 e, float a) { return smoothstep(0.08, 0.4, (1.0 - wetRing(uv, e, 2.5)) * a); }
float depositRate(vec2 uv, vec2 e, float h, float a, float tooth) {
    float rate = 0.0065
               + 0.07 * edgeDark * (1.0 - smoothstep(0.0, 0.05, h)) * smoothstep(0.0, 0.004, h)
               + 0.30 * edgeDark * rimOf(uv, e, a);
    if (h < 0.0015) rate = 0.5;
    return clamp(rate, 0.0, 0.95);
}

// ═════════════════════════════════════════════════════════════════════════
void main() {
    setup();
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 e = 1.0 / R;
    vec2 p = vec2(uv.x * g_asp, uv.y);
    float fI = float(FRAMEINDEX);
    bool oldSheet = g_sliding > 0.5 && p.x < edgeAt(p);
    g_paperOff = sheetOff(oldSheet ? g_n - 1.0 : g_n);
    bool cleared = g_sliding > 0.5 && p.x >= edgeAt(p);     // under the incoming blank sheet

    // ─── frame 0: the sheet's first wash is already laid, still wet ─────
    if (PASSINDEX >= 1 && PASSINDEX < 4 && FRAMEINDEX < 2) {
        g_seed = 1.0;
        Paint S = painter(p);
        if (PASSINDEX == 1) gl_FragColor = vec4(min(S.water, 0.22), 0.0, 0.0, S.wet);
        else if (PASSINDEX == 2) gl_FragColor = vec4(S.pig, 1.0);
        else gl_FragColor = vec4(S.dry, 1.0);
        return;
    }

    // ─── pass 0 : the painter, evaluated once per frame ─────────────────
    if (PASSINDEX == 0) {
        Paint P = painter(p);
        gl_FragColor = vec4(P.pig, P.water + 2.0 * step(0.5, P.wet));   // a: water + 2·(wet mask)
        return;
    }

    // ─── pass 1 : water · flow · paper wetness ──────────────────────────
    if (PASSINDEX == 1) {
        if (cleared) { gl_FragColor = vec4(0.0); return; }
        vec4 c  = texture2D(wetBuf, uv);
        vec4 nN = texture2D(wetBuf, uv + vec2(0.0, e.y)), nS = texture2D(wetBuf, uv - vec2(0.0, e.y));
        vec4 nE = texture2D(wetBuf, uv + vec2(e.x, 0.0)), nW = texture2D(wetBuf, uv - vec2(e.x, 0.0));
        vec4 n2N = texture2D(wetBuf, uv + vec2(0.0, 3.0 * e.y)), n2S = texture2D(wetBuf, uv - vec2(0.0, 3.0 * e.y));
        vec4 n2E = texture2D(wetBuf, uv + vec2(3.0 * e.x, 0.0)), n2W = texture2D(wetBuf, uv - vec2(3.0 * e.x, 0.0));
        float w = c.a;
        vec2 grad = vec2(n2E.r - n2W.r, n2N.r - n2S.r) / 6.0;
        float rim = clamp((1.0 - wetRing(uv, e, 3.0)) * w * 2.0, 0.0, 1.0);
        // flow: outward drift on the (smooth) wetness field + a wide depth gradient + curl wander
        vec2 gradW = vec2(n2E.a - n2W.a, n2N.a - n2S.a) / 6.0;
        vec2 grad6 = vec2(texture2D(wetBuf, uv + vec2(7.0 * e.x, 0.0)).r - texture2D(wetBuf, uv - vec2(7.0 * e.x, 0.0)).r,
                          texture2D(wetBuf, uv + vec2(0.0, 7.0 * e.y)).r - texture2D(wetBuf, uv - vec2(0.0, 7.0 * e.y)).r) / 14.0;
        vec2 vT = -gradW * (30.0 * (0.3 + 0.7 * flow)) * smoothstep(0.0, 0.03, c.r)
                  - (grad * 0.4 + grad6 * 0.6) * (10.0 * (0.3 + 0.7 * flow));
        vec2 q = (p + g_paperOff) * 6.0 + vec2(0.0, g_t * 0.03);
        float ce = 0.02;
        float pn = fbm3(q + vec2(0.0, ce)), ps = fbm3(q - vec2(0.0, ce));
        float pe = fbm3(q + vec2(ce, 0.0)), pw = fbm3(q - vec2(ce, 0.0));
        vec2 curl = vec2(pn - ps, -(pe - pw)) / (2.0 * ce);
        vT += curl * 0.07 * flow * (1.0 + 0.6 * midP()) * smoothstep(0.0, 0.04, c.r);
        vec2 v = mix(c.gb, vT, 0.2) * w;
        float vl = length(v); if (vl > 0.7) v *= 0.7 / vl;
        float h = c.r + 0.24 * (nN.r + nS.r + nE.r + nW.r - 4.0 * c.r) * w;
        // back-run front: stochastic (Eden) growth into DAMP paper gated by fibre noise → round cauliflower edges
        float tooth = paperLo(p);
        float fr = fbm((p + g_paperOff) * 40.0 + 3.0) * 0.65 + tooth * 0.35;
        float pd = texture2D(wetBuf, uv + e).r + texture2D(wetBuf, uv - e).r
                 + texture2D(wetBuf, uv + vec2(e.x, -e.y)).r + texture2D(wetBuf, uv + vec2(-e.x, e.y)).r;
        float pushAvg = (nN.r + nS.r + nE.r + nW.r + pd * 0.7) / 6.8;
        float damp = smoothstep(0.04, 0.35, w);
        float prob = damp * smoothstep(0.5, 0.75, fr) * smoothstep(0.004, 0.03, pushAvg) * 0.35 * blooms;
        float front = step(hash12(fc * 1.37 + fract(fI * 0.6180339) * 917.0), prob);
        if (w < 0.5) h = max(c.r, pushAvg * 0.9 * front);
        // evaporation: the rim and thin water go first (drives edge darkening)
        float hBl = (n2N.r + n2S.r + n2E.r + n2W.r + nN.r + nS.r + nE.r + nW.r) * 0.125;
        h -= (0.0007 + 0.0014 * rim + 0.0004 * (1.0 - smoothstep(0.0, 0.05, hBl))) * dryRate;
        vec4 pb = texture2D(paintBuf, uv);
        float pWet = step(1.5, pb.a);
        h = clamp(h + pb.a - 2.0 * pWet, 0.0, 0.5);
        w = max(w * (1.0 - 0.006 * dryRate), smoothstep(0.0, 0.02, h));
        w = max(w, pWet);
        if (h < 0.0005) w *= (1.0 - 0.02 * dryRate);
        gl_FragColor = vec4(h, v, clamp(w, 0.0, 1.0));
        return;
    }

    // ─── pass 2 : suspended pigment ─────────────────────────────────────
    if (PASSINDEX == 2) {
        if (cleared) { gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0); return; }
        vec4 wb = texture2D(wetBuf, uv);
        vec2 v = wb.gb;
        vec3 s  = texture2D(pigBuf, uv - v * e).rgb;
        vec3 s0 = texture2D(pigBuf, uv).rgb;
        vec3 sN = texture2D(pigBuf, uv + vec2(0.0, e.y)).rgb, sS = texture2D(pigBuf, uv - vec2(0.0, e.y)).rgb;
        vec3 sE = texture2D(pigBuf, uv + vec2(e.x, 0.0)).rgb, sW = texture2D(pigBuf, uv - vec2(e.x, 0.0)).rgb;
        // fractal diffusivity → feathered, fern-like fringes instead of gaussian blur
        float Dk = 0.16 * (0.5 + 0.6 * smoothstep(0.35, 0.75, fbm((p + g_paperOff) * 34.0 + 11.0))) * wb.a * smoothstep(0.0, 0.03, wb.r) * (0.4 + 0.6 * flow);
        s += Dk * (sN + sS + sE + sW - 4.0 * s0);
        vec3 Ppig = texture2D(paintBuf, uv).rgb;
        vec3 dep = texture2D(depBuf, uv).rgb;
        float sat = dot(dep + s0, vec3(0.333));
        s += Ppig / (1.0 + 0.6 * sat * sat);    // a loaded sheet takes a little less (fights mud, keeps glazes)
        float lift = clamp(max(0.0, wb.r - 0.1) * 0.02 * blooms, 0.0, 0.25);
        s += dep * lift;
        s -= s * depositRate(uv, e, wb.r, wb.a, paperH(p));
        if (wb.a < 0.02) s *= 0.5;
        float sd2 = dot(s, vec3(0.333));
        if (sd2 > 1.6) s *= 1.6 / sd2;
        gl_FragColor = vec4(clamp(s, 0.0, 6.0), 1.0);
        return;
    }

    // ─── pass 3 : deposited pigment ─────────────────────────────────────
    if (PASSINDEX == 3) {
        if (cleared) { gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0); return; }
        vec4 wb = texture2D(wetBuf, uv);
        vec3 s = texture2D(pigBuf, uv).rgb;            // this frame's suspension (after its own deposit)
        vec3 dep = texture2D(depBuf, uv).rgb;
        float rate = depositRate(uv, e, wb.r, wb.a, paperH(p));
        dep += s * rate / (1.0 - rate);
        float lift = clamp(max(0.0, wb.r - 0.1) * 0.02 * blooms, 0.0, 0.25);
        dep -= dep * lift;
        float dd = dot(dep, vec3(0.333));
        if (dd > 1.9) dep *= 1.9 / dd;          // density cap: layered glazes stay luminous, never mud
        gl_FragColor = vec4(clamp(dep, 0.0, 9.0), 1.0);
        return;
    }

    // ─── pass 4 : paint it ──────────────────────────────────────────────
    vec2 simR = floor(R * simRes);
    vec2 se = 1.0 / simR;
    float tooth = paperH(p);
    // paper fibres fray the sampled position → torn, crisp micro-edges at full res
    vec2 jit = (vec2(vnoise(p * 210.0), vnoise(p * 210.0 + 5.3)) - 0.5) * se * 1.8;
    vec2 suv = uv + jit;
    vec3 dep = texture2D(depBuf, suv).rgb;
    vec3 sus = texture2D(pigBuf, suv).rgb;
    vec4 wb  = texture2D(wetBuf, suv);
    if (g_sliding > 0.5 && p.x >= edgeAt(p)) { dep = vec3(0.0); sus = vec3(0.0); wb = vec4(0.0); }

    vec3 A = dep + sus * 0.95;
    A = 2.7 * (1.0 - exp(-A / 2.7));
    float dens = dot(A, vec3(0.333));
    // granulation: pigment pools in the tooth's valleys; flocculation speckle at full res
    // granulation is pigment-specific: indanthrone / neutral tint (absorb red more than blue) settle,
    // crimson / orange / turquoise stay clean. It collects in the tooth's VALLEYS only (peaks stay smooth)
    // and it darkens all channels equally (same hue, no chroma noise).
    float granP = smoothstep(0.15, 1.1, A.r - 0.7 * A.b) * smoothstep(0.05, 0.5, dens);
    float valley = smoothstep(0.42, 0.22, paperFine(p)) * smoothstep(0.62, 0.4, tooth);
    A *= 1.0 + granulation * granP * valley * 0.28;
    // music: the glaze breathes — pigment deepens on the bass, mids add body (smooth followers)
    A *= 0.92 + 0.36 * bassP() + 0.14 * midP();
    vec3 col = paperColor.rgb * exp(-A * 1.1);
    col = hueRot(col, hueShift);
    col = mix(vec3(lum(col)), col, saturation);

    // raking light: deep tooth shadows + broad cockling of the wetted sheet
    float ang = radians(lightAz);
    vec3 L = normalize(vec3(cos(ang), sin(ang), 0.5));
    float tx = 1.0 / R.y;
    float hx = (paperH(p + vec2(tx, 0.0)) - paperH(p - vec2(tx, 0.0))) * 0.45 + (paperFine(p + vec2(tx, 0.0)) - paperFine(p - vec2(tx, 0.0))) * 0.35;
    float hy = (paperH(p + vec2(0.0, tx)) - paperH(p - vec2(0.0, tx))) * 0.45 + (paperFine(p + vec2(0.0, tx)) - paperFine(p - vec2(0.0, tx))) * 0.35;
    vec3 n = normalize(vec3(-hx, -hy, 0.085 / max(paperTexture, 0.05)));
    float diff = clamp(dot(n, L), 0.0, 1.0);
    col *= mix(1.0, 0.84 + 0.26 * diff, clamp(paperTexture, 0.0, 1.5));
    float ck = 0.03;
    vec2 cg = vec2(cockle(p + vec2(ck, 0.0)) - cockle(p - vec2(ck, 0.0)), cockle(p + vec2(0.0, ck)) - cockle(p - vec2(0.0, ck))) / (2.0 * ck);
    vec3 cn = normalize(vec3(-cg * 0.06 * cockling, 1.0));
    col *= 0.93 + 0.2 * clamp(dot(cn, L) - 0.9, -0.5, 0.5) * 2.0;

    // wet passages: deeper colour + a glossy film that rides the cockles and the water's own relief
    float h = wb.r;
    float film = smoothstep(0.004, 0.06, h);
    col *= mix(vec3(1.0), col * 0.35 + 0.62, 0.35 * film);
    float gx = texture2D(wetBuf, suv + vec2(2.0 * se.x, 0.0)).r - texture2D(wetBuf, suv - vec2(2.0 * se.x, 0.0)).r;
    float gy = texture2D(wetBuf, suv + vec2(0.0, 2.0 * se.y)).r - texture2D(wetBuf, suv - vec2(0.0, 2.0 * se.y)).r;
    vec3 wn = normalize(vec3(-gx * 9.0 - hx * 0.1 - cg.x * 0.02 * cockling, -gy * 9.0 - hy * 0.1 - cg.y * 0.02 * cockling, 0.1));
    vec3 Hh = normalize(L + vec3(0.0, 0.0, 1.0));
    float nh = clamp(dot(wn, Hh), 0.0, 1.0);
    float spec = pow(nh, 60.0) * 0.16 + pow(nh, 600.0) * 0.4 * (1.0 + highP()) * smoothstep(0.03, 0.08, h);
    col += spec * film * wetSheen;

    // sheet change: the incoming sheet's edge + its drop shadow on the old painting
    if (g_sliding > 0.5) {
        float dx = edgeAt(p) - p.x;
        if (dx > 0.0) col *= 1.0 - 0.42 * exp(-dx * 38.0) - 0.1 * exp(-dx * 7.0);
        else col *= 1.0 - 0.18 * exp(dx * 900.0);          // the deckled edge catches a hairline shadow
        col *= 1.0 + 0.04 * step(dx, 0.0) * exp(dx * 60.0);
    }

    col *= 1.72;    // exposure into ACES: paper lands near white, pigment keeps its depth

    // soft bloom off the wet gloss
    float bl = 0.0;
    for (int i = 0; i < 6; i++) {
        float a = float(i) * 1.047 + 0.4;
        bl += smoothstep(0.04, 0.25, texture2D(wetBuf, suv + vec2(cos(a), sin(a)) * se * 4.0).r);
    }
    col += vec3(1.0, 0.97, 0.92) * bl / 6.0 * 0.06 * bloom;

    col = aces(col);
    col += (hash12(fc + fract(TIME) * 91.0) - 0.5) * 0.03 * grain;
    // HD finisher
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float l = dot(col, vec3(0.299, 0.587, 0.114));
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);

    float alpha = 1.0;
    if (transparentBg) {
        alpha = clamp(1.0 - exp(-dens * 1.6), 0.0, 1.0);
        col *= alpha;
    }
    gl_FragColor = vec4(col, alpha);
}
