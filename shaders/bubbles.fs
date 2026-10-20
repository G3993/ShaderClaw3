/*{
  "DESCRIPTION": "Bubbles — effervescence seen from inside the liquid: gas nucleates on the floor of a vessel and along a submerged electrode wire, and streams of bubbles rise through a deep tinted liquid toward a rippling silver surface. Every bubble is a real raymarched gas pocket — it swells as the pressure drops, wobbles into an oblate wobbling shape, zig-zags sideways, and the fast big ones catch and swallow the slow small ones in a soft coalescence that flashes iridescent. Thin-film interference paints rainbow bands across each bubble that drift as the film drains with age; the liquid behind is bent through the gas, the rims flare with total internal reflection and a hot key-light glint, and a lens caustic focuses under each bubble onto the caustic-lit floor. Light shafts fall from the surface, the surface mirrors the rising bubbles and rings outward where each one pops, micro-bubble sparkle streams twinkle between the columns. Bass swells the bubbles, beats release a burst of fizz and flash the key light, highs twinkle the sparkles, level lifts the shafts.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-13 — original. CIDKID chemistry pack #6",
  "CATEGORIES": ["Generator", "3D", "Chemistry", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Rise Speed",          "TYPE": "float", "AUDIO": false, "MIN": 0.2,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "wobble",       "LABEL": "Wobble",              "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "camDrift",     "LABEL": "Camera Drift",        "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "density",      "LABEL": "Column Density",      "TYPE": "float", "AUDIO": false, "MIN": 0.1,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Bubbles" },
    { "NAME": "rate",         "LABEL": "Stream Rate",         "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Bubbles" },
    { "NAME": "size",         "LABEL": "Bubble Size",         "TYPE": "float", "AUDIO": false, "MIN": 0.4,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Bubbles" },
    { "NAME": "grow",         "LABEL": "Swell While Rising",  "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.7,  "GROUP": "Bubbles" },
    { "NAME": "merge",        "LABEL": "Coalescence",         "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Bubbles" },
    { "NAME": "spread",       "LABEL": "Spread (wire→floor)", "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.45, "GROUP": "Bubbles" },
    { "NAME": "electrode",    "LABEL": "Electrode Wire",      "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.4,  "GROUP": "Bubbles" },
    { "NAME": "shapeWobble",  "LABEL": "Shape Wobble",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Bubbles" },
    { "NAME": "depth",        "LABEL": "Vessel Depth",        "TYPE": "float", "AUDIO": false, "MIN": 1.8,  "MAX": 5.0,  "DEFAULT": 3.0,  "GROUP": "Liquid" },
    { "NAME": "clarity",      "LABEL": "Clarity",             "TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 1.0,  "DEFAULT": 0.7,  "GROUP": "Liquid" },
    { "NAME": "ripples",      "LABEL": "Surface Ripples",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Liquid" },
    { "NAME": "liquidColor",  "LABEL": "Liquid Color",        "TYPE": "color", "DEFAULT": [0.10, 0.62, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "backColor",    "LABEL": "Deep Color",          "TYPE": "color", "DEFAULT": [0.01, 0.035, 0.11, 1.0], "GROUP": "Color" },
    { "NAME": "backAmt",      "LABEL": "Deep Amount",         "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "film",         "LABEL": "Iridescence",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "hueShift",     "LABEL": "Hue Shift",           "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",   "LABEL": "Saturation",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",      "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" },
    { "NAME": "keyLight",     "LABEL": "Key Light",           "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "shafts",       "LABEL": "Light Shafts",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "caustics",     "LABEL": "Caustics",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "glint",        "LABEL": "Glints",              "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "sparkles",     "LABEL": "Micro Bubbles",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bloom",        "LABEL": "Bloom",               "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7,  "GROUP": "Finish" },
    { "NAME": "aberration",   "LABEL": "Chromatic Fringe",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "grain",        "LABEL": "Grain",               "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.2,  "GROUP": "Finish" },
    { "NAME": "marchRes",     "LABEL": "March Resolution",    "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 1.0,  "DEFAULT": 0.7,  "GROUP": "Finish" },
    { "NAME": "audioReact",   "LABEL": "Audio React",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" }
  ],
  "PASSES": [
    { "TARGET": "bbScene", "WIDTH": "floor($WIDTH*$marchRes)", "HEIGHT": "floor($HEIGHT*$marchRes)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Bubbles — CIDKID chemistry pack #6
//   pass 0  bbScene — camera INSIDE the liquid. The floor is tiled into
//                     columns (cell C); each active column owns a stream of
//                     NB bubbles on analytic paths (rise, swell, wobble,
//                     oblate shape wobble). The march evaluates only the
//                     current column (bubble state cached per column) and
//                     clamps every step to the column wall so nothing is
//                     missed. Bubbles: TIR rims, refraction through the gas,
//                     thin-film interference by age, key glints; floor:
//                     caustics + lens spot/shadow under each bubble + the
//                     electrode wire; surface: Fresnel/TIR mirror with pop
//                     rings, a 40-step reflection march of the bubbles;
//                     light shafts integrated along the ray. a = depth/FAR
//   pass 1  final   — micro-bubble sparkle streams (depth tested), bloom,
//                     chromatic fringe, ACES, grain, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define NB   4
#define FAR  13.0
#define CELL 1.0

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    if (h < 0.001) return c;
    float a = h * TAU, cs = cos(a), sn = sin(a);
    vec3 k = vec3(0.57735);
    return c * cs + cross(k, c) * sn + k * dot(k, c) * (1.0 - cs);
}

// ── state ────────────────────────────────────────────────────────────────
float g_t, g_ar, g_H, g_lens, g_merge, g_sizeK;
float g_bass, g_mid, g_high, g_beat, g_lev;
vec3  g_ro, g_uu, g_vv, g_ww, g_liq, g_deep, g_key, g_key2;
float g_wireZ, g_wireY, g_wireR;
// cached column
vec2  g_cell; float g_cellOn; vec2 g_nuc;
vec3  g_bc[NB]; vec3 g_brad[NB]; float g_bage[NB]; float g_bh[NB]; float g_bpop[NB]; float g_br[NB]; float g_bfilm[NB];
int   g_hitJ; float g_blend;

vec3 liqTop() { return g_liq; }

// caustic lines (two ridged noise layers multiplied → thin bright threads)
float caustic(vec2 p, float t) {
    vec2 q1 = p * 2.1 + vec2(t * 0.13, -t * 0.09);
    vec2 q2 = p * 2.7 + vec2(-t * 0.08, t * 0.11) + 5.3;
    float n1 = 1.0 - abs(2.0 * vnoise2(q1) - 1.0);
    float n2 = 1.0 - abs(2.0 * vnoise2(q2) - 1.0);
    float c = pow(n1 * n2, 3.0);
    vec2 q3 = p * 5.3 + vec2(t * 0.21, t * 0.17) + 11.0;
    float n3 = 1.0 - abs(2.0 * vnoise2(q3) - 1.0);
    return c * 2.6 + pow(n3, 7.0) * 0.35;
}

// liquid light field seen along dir d at height y: bright toward the surface,
// deep colour below, lamp glow above-front
vec3 envL(vec3 d, float y) {
    float up = smoothstep(-0.05, 0.85, d.y);
    vec3 c = mix(g_deep, g_liq * 1.05, up);
    c += g_liq * 0.3 * pow(max(d.y, 0.0), 3.0);
    float k = keyLight * (1.0 + 0.35 * g_ar * g_beat);
    c += mix(g_liq, vec3(1.0), 0.6) * pow(max(dot(d, g_key), 0.0), 26.0) * 1.5 * k;
    c += mix(g_liq, vec3(1.0), 0.3) * pow(max(dot(d, g_key2), 0.0), 14.0) * 0.35 * k;
    c *= mix(0.3, 1.15, clamp(y / g_H, 0.0, 1.0));
    return c;
}
// the same field with imagery: what a bent ray from p actually sees — the
// caustic skin of the surface above or the caustic-lit floor below
vec3 envI(vec3 d, vec3 p) {
    vec3 c = envL(d, p.y);
    if (d.y > 0.08) {
        float tt = (g_H - p.y) / d.y;
        vec2 xz = p.xz + d.xz * tt;
        float ca = caustic(xz * 0.7 + 3.0, g_t * speed) * caustics;
        c += mix(g_liq, vec3(1.0), 0.7) * ca * 0.9 * keyLight * exp(-tt * 0.12 / clarity) * smoothstep(0.08, 0.3, d.y);
    } else if (d.y < -0.08) {
        float tt = -p.y / d.y;
        vec2 xz = p.xz + d.xz * tt;
        float ca = caustic(xz * 0.9, g_t * speed) * caustics;
        vec3 fc = mix(g_deep, g_liq, 0.22);
        c = mix(c, fc * (0.3 + 1.2 * ca) * keyLight, exp(-tt * 0.14 / clarity) * smoothstep(-0.08, -0.3, d.y));
    }
    return c;
}

// thin-film interference (film of liquid n=1.33 around the gas), d = thickness
vec3 thinFilm(float cosI, float d) {
    float sinT2 = (1.0 - cosI * cosI) / (1.33 * 1.33);
    float cosT = sqrt(max(1.0 - sinT2, 0.0));
    vec3 lambda = vec3(650.0, 532.0, 450.0);
    vec3 ph = 4.0 * PI * 1.33 * d * cosT / lambda;
    vec3 c = 0.5 + 0.5 * cos(ph);
    c = c * c;
    return c;
}

// ── camera ───────────────────────────────────────────────────────────────
void setupCam() {
    float cd = camDrift;
    g_ro = vec3(0.45 * sin(g_t * 0.071) * cd, g_H * 0.2 + 0.1 * sin(g_t * 0.053) * cd, 0.3 * sin(g_t * 0.037) * cd);
    float yaw = 0.16 * sin(g_t * 0.047) * cd;
    float pitch = 0.06 + 0.05 * sin(g_t * 0.061) * cd;
    vec3 fwd = normalize(vec3(sin(yaw) * cos(pitch), sin(pitch), cos(yaw) * cos(pitch)));
    g_ww = fwd;
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    float roll = 0.03 * sin(g_t * 0.043) * cd;
    vec3 u2 = cos(roll) * g_uu + sin(roll) * g_vv;
    vec3 v2 = -sin(roll) * g_uu + cos(roll) * g_vv;
    g_uu = u2; g_vv = v2;
    g_lens = 1.35;
}
vec3 camRay(vec2 uv) { return normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_lens); }
vec3 project(vec3 p) {
    vec3 d = p - g_ro;
    float z = dot(d, g_ww);
    vec2 s = vec2(dot(d, g_uu), dot(d, g_vv)) / max(z, 0.05) * g_lens;
    return vec3(s, z);
}

// ── bubble columns ───────────────────────────────────────────────────────
// column activity: dense curtain along the electrode wire, sparser elsewhere
float columnWeight(vec2 c) {
    float cz = (c.y + 0.5) * CELL;
    if (cz < 0.95 && abs((c.x + 0.5) * CELL) < 1.3) return 0.0;
    float curtain = 0.1 + 0.9 * smoothstep(1.3, 0.0, abs(cz - g_wireZ));
    return density * mix(curtain, 1.0, spread) * 1.4;
}
void loadCell(vec2 c) {
    g_cell = c;
    vec2 hc = hash22(c + 0.37);
    g_cellOn = (hc.x < columnWeight(c)) ? 1.0 : 0.0;
    vec2 h2 = hash22(c * 1.7 + 9.1);
    vec2 base = (c + 0.5) * CELL;
    g_nuc = base + (h2 - 0.5) * 0.2 * CELL;
    if (g_cellOn < 0.5) return;
    float period = (1.0 / max(rate, 0.05)) * (0.7 + 0.6 * h2.x) * 1.2;
    float Lc = period * float(NB);
    float T = g_t * speed;
    float lateralMax = 0.1 * CELL + 0.1 * CELL * wobble;
    float rCap = 0.5 * CELL - lateralMax - 0.012;
    for (int j = 0; j < NB; j++) {
        float fj = float(j);
        vec2 hj = hash22(c * 3.7 + fj * 13.1 + 0.5);
        vec2 hk = hash22(c * 5.3 + fj * 7.7 + 2.5);
        float age = mod(T - fj * period - hc.y * Lc, Lc);
        vec2 hm = hash22(c * 2.1 + fj * 3.3 + 7.7);
        float hero = step(0.87, hm.x);                 // ~1 in 8 is a hero, 3-4x the fizz
        float r0 = g_sizeK * 0.07 * mix(0.42 + 0.55 * hj.x, 2.6 + 1.4 * hj.x, hero);
        // rise time as a fraction of the cycle: fizz is quick, heroes drift up slowly
        float rt = mix(0.42 + 0.28 * hj.x, 0.9, hero);
        float s = g_H / (Lc * rt);
        float y = 0.025 + s * age * (1.0 + 0.18 * grow * age / Lc);
        float emerge = smoothstep(0.0, 0.45 + 0.6 * hero, age);
        float r = r0 * (0.3 + 0.7 * emerge) * (1.0 + grow * 0.6 * clamp(y / g_H, 0.0, 1.0));
        r *= 1.0 + 0.16 * g_ar * g_bass;
        r = min(r, rCap);
        // film thickness (nm): wide spread per bubble, drains as it ages
        g_bfilm[j] = (130.0 + 780.0 * pow(hm.y, 1.4)) * (1.0 - 0.55 * clamp(age / (Lc * 0.85), 0.0, 1.0));
        // pop at the surface: quick shrink over the last bit, ring takes over
        float top = g_H - r * 1.1 - 0.03;
        float popK = 1.0 - smoothstep(top - 0.5 * r, top, y);
        g_bpop[j] = (y - top) / max(s, 0.1);          // seconds since the pop (<0 = still rising)
        r *= popK;
        // sideways zig-zag, stronger for the big ones
        float amp = 0.1 * CELL * wobble * smoothstep(0.0, 1.0, age) * (0.45 + 0.55 * hj.y) * (0.5 + 0.5 * smoothstep(0.04, 0.16, r));
        float wf = 2.0 + 1.6 * hk.x;
        float x = g_nuc.x + amp * sin(age * wf + hj.x * TAU);
        float z = g_nuc.y + amp * 0.8 * cos(age * wf * 0.83 + hj.y * TAU);
        g_bc[j] = vec3(x, y, z);
        // fade before the lens so a hero passing close never clips the near plane
        float dcam = length(vec3(x, y, z) - g_ro);
        r *= smoothstep(0.3, 0.75, dcam);
        // oblate + harmonic shape wobble
        float ob = 1.0 - 0.16 * smoothstep(0.06, 0.24, r);
        float e = 0.05 * shapeWobble * smoothstep(0.03, 0.12, r);
        float w1 = age * (5.0 + 3.0 * hk.y) + hk.x * TAU;
        g_brad[j] = r * vec3(1.0 + e * sin(w1), ob * (1.0 - 1.3 * e * sin(w1)), 1.0 + e * cos(w1 * 1.3 + 1.0));
        g_br[j] = r;
        g_bage[j] = age;
        g_bh[j] = hj.y;
    }
}
float sdEllip(vec3 p, vec3 r) {
    float k0 = length(p / r);
    float k1 = length(p / (r * r));
    return k0 * (k0 - 1.0) / max(k1, 1e-5);
}
float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}
// distance to the bubbles of the cached column (+ the wire); sets g_hitJ/g_blend
float sdColumn(vec3 p) {
    float d = 1e3;
    g_hitJ = -1; g_blend = 0.0;
    if (g_cellOn > 0.5) {
        float k = 0.012 + 0.07 * g_merge;
        for (int j = 0; j < NB; j++) {
            if (g_br[j] < 0.002) continue;
            float dj = sdEllip(p - g_bc[j], g_brad[j]);
            if (dj < d) {
                float h = clamp(0.5 + 0.5 * (d - dj) / k, 0.0, 1.0);
                g_blend = max(g_blend, 4.0 * h * (1.0 - h) * step(dj, 0.05));
                g_hitJ = j;
            } else {
                float h = clamp(0.5 + 0.5 * (dj - d) / k, 0.0, 1.0);
                g_blend = max(g_blend, 4.0 * h * (1.0 - h) * step(d, 0.05));
            }
            d = smin(d, dj, k);
        }
    }
    return d;
}
float sdWire(vec3 p) {
    return length(vec2(p.y - g_wireY, p.z - g_wireZ)) - g_wireR;
}
// march bubbles (+ wire) between t0 and t1; returns t or -1; mat 0 bubble, 1 wire
float marchBubbles(vec3 ro, vec3 rd, float t0, float t1, int steps, out int mat) {
    float t = t0; mat = 0;
    for (int i = 0; i < 150; i++) {
        if (i >= steps) break;
        vec3 p = ro + rd * t;
        vec2 c = floor(p.xz / CELL);
        if (c.x != g_cell.x || c.y != g_cell.y) loadCell(c);
        float d = sdColumn(p);
        float dw = (electrode > 0.001) ? sdWire(p) : 1e3;
        if (dw < d) { d = dw; if (d < 0.0012 * (1.0 + t * 0.15)) { mat = 1; return t; } }
        else if (d < 0.0012 * (1.0 + t * 0.15)) { mat = 0; return t; }
        // never cross into the next column without re-evaluating it
        vec2 q = p.xz - (c + 0.5) * CELL;
        float wall = min(0.5 * CELL - abs(q.x), 0.5 * CELL - abs(q.y));
        t += min(d, wall + 0.01);
        if (t > t1) break;
    }
    return -1.0;
}
vec3 bubbleNormal(vec3 p) {
    vec2 e = vec2(0.0012, -0.0012);
    float d1 = sdColumn(p + e.xyy), d2 = sdColumn(p + e.yyx), d3 = sdColumn(p + e.yxy), d4 = sdColumn(p + e.xxx);
    return normalize(e.xyy * d1 + e.yyx * d2 + e.yxy * d3 + e.xxx * d4);
}

// ── shading ──────────────────────────────────────────────────────────────
vec3 shadeBubble(vec3 p, vec3 rd, vec3 n, bool simple) {
    // per-bubble data for the nearest bubble (loop copy — no dynamic indexing)
    int hj = g_hitJ;
    float blend = g_blend;
    sdColumn(p);                // refresh g_hitJ at the exact hit
    hj = g_hitJ; blend = max(blend, g_blend);
    vec3 bc = vec3(0.0); float br = 0.1, bage = 0.0, bh = 0.0, bfilm = 400.0;
    for (int j = 0; j < NB; j++) { if (j != hj) continue; bc = g_bc[j]; br = g_br[j]; bage = g_bage[j]; bh = g_bh[j]; bfilm = g_bfilm[j]; }
    float cosv = clamp(dot(n, -rd), 0.0, 1.0);
    // liquid → gas Fresnel with total internal reflection past the critical angle
    float F0 = 0.02;
    float Fs = F0 + (1.0 - F0) * pow(1.0 - cosv, 5.0);
    float F = mix(Fs, 1.0, smoothstep(0.74, 0.64, cosv));
    vec3 rfl = envI(reflect(rd, n), p);
    // refraction through the gas pocket (sphere exit) with a little dispersion
    vec3 trans = vec3(0.0);
    if (!simple) {
        vec3 rin = refract(rd, n, 1.33);
        if (dot(rin, rin) < 0.5) rin = reflect(rd, n);
        vec3 nc = normalize(p - bc);
        float chord = max(-2.0 * dot(rin, nc) * br, 0.0);
        vec3 pe = p + rin * chord;
        vec3 ne = normalize(pe - bc);
        vec3 oR = refract(rin, -ne, 1.0 / 1.345); if (dot(oR, oR) < 0.5) oR = reflect(rin, -ne);
        vec3 oG = refract(rin, -ne, 1.0 / 1.33);  if (dot(oG, oG) < 0.5) oG = reflect(rin, -ne);
        vec3 oB = refract(rin, -ne, 1.0 / 1.315); if (dot(oB, oB) < 0.5) oB = reflect(rin, -ne);
        trans = vec3(envI(oR, pe).r, envI(oG, pe).g, envI(oB, pe).b);
    } else {
        trans = envI(rd, p);
    }
    // thin film: thickness drains with age, each bubble its own start
    float thick = bfilm * (1.0 + 0.08 * g_ar * g_mid);
    vec3 fl = thinFilm(cosv, thick);
    float edge = 0.3 + 0.7 * pow(1.0 - cosv, 1.2);
    vec3 filmC = fl * film * edge * (1.0 + 1.8 * blend * g_merge);
    vec3 col = rfl * F + trans * (1.0 - F);
    col = col * (1.0 + filmC * 2.2) + filmC * (0.3 + 0.35 * lum(g_liq));
    // key glints
    vec3 rv = reflect(rd, n);
    float k = keyLight * (1.0 + 0.5 * g_ar * g_beat);
    float gl = pow(max(dot(rv, g_key), 0.0), 220.0) * 4.0 + pow(max(dot(rv, g_key2), 0.0), 90.0) * 1.2;
    col += vec3(1.0, 0.98, 0.95) * gl * glint * k;
    // inner bright ring (light trapped at the rim) + merge flash
    float ring = exp(-pow((cosv - 0.40) * 9.0, 2.0)) * 0.22 * glint;
    col += mix(g_liq, vec3(1.0), 0.5) * ring;
    col += vec3(1.0) * blend * g_merge * 0.35;
    return col;
}
vec3 shadeWire(vec3 p, vec3 rd) {
    vec3 n = normalize(vec3(0.0, p.y - g_wireY, p.z - g_wireZ));
    vec3 steel = vec3(0.10, 0.11, 0.13);
    float dif = max(dot(n, g_key), 0.0);
    vec3 rv = reflect(rd, n);
    float fr = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);
    vec3 col = steel * (0.15 + 0.6 * dif) * keyLight + envL(rv, p.y) * (0.08 + 0.4 * fr);
    col += vec3(1.0, 0.95, 0.9) * pow(max(dot(rv, g_key), 0.0), 90.0) * 0.9 * keyLight;
    // faint fizz along the wire
    col += g_liq * (0.05 + 0.05 * sin(p.x * 9.0 + g_t * 7.0)) * electrode;
    return col;
}
// bubble lens spots + shadow rings on the floor from the column above
vec3 floorBubbleLight(vec3 p, vec3 col) {
    if (g_cellOn < 0.5) return col;
    for (int j = 0; j < NB; j++) {
        float r = g_br[j];
        if (r < 0.004) continue;
        vec3 b = g_bc[j];
        float dxz = length(p.xz - b.xz);
        float h = max(b.y - r, 0.0);
        float low = smoothstep(1.1, 0.3, h);                 // only bubbles near the floor mark it
        float blur = 0.35 * r + 0.12 * h;
        float spot = exp(-dxz * dxz / (blur * blur * 0.6)) * 2.2 * r / (0.08 + h * 0.9) * low;
        float sh = 0.6 * exp(-pow((dxz - r * 0.95) / (0.4 * r + 0.1 * h), 2.0)) / (1.0 + h * 1.2) * low;
        // fade before the column wall so neighbouring columns never show a seam
        vec2 q = p.xz - (g_cell + 0.5) * CELL;
        float wallF = smoothstep(0.5 * CELL, 0.5 * CELL - 0.12, max(abs(q.x), abs(q.y)));
        sh *= wallF; spot *= wallF;
        col *= 1.0 - sh;
        col += mix(g_liq, vec3(1.0), 0.45) * spot * caustics * keyLight;
    }
    return col;
}
vec3 shadeFloor(vec3 p, vec3 rd) {
    vec3 fc = mix(g_deep, g_liq, 0.22) * 0.9;
    float c = caustic(p.xz * 0.9, g_t * speed) * caustics;
    float att = exp(-g_H * 0.28) * 1.6;
    vec3 col = fc * (0.35 + 1.6 * c * att) * keyLight;
    col *= 0.94 + 0.06 * vnoise2(p.xz * 30.0);
    // nucleation site: a tiny bright cluster where the stream starts
    if (g_cellOn > 0.5) {
        float dn = length(p.xz - g_nuc);
        col += mix(g_liq, vec3(1.0), 0.6) * exp(-dn * dn * 900.0) * 1.2 * (0.7 + 0.3 * sin(g_t * 11.0 + g_nuc.x * 7.0));
        col += g_liq * exp(-dn * dn * 120.0) * 0.25;
    }
    col = floorBubbleLight(p, col);
    // wire glow spill
    if (electrode > 0.001) col *= 1.0 - 0.35 * exp(-abs(p.z - g_wireZ) * 14.0) * electrode;
    // sheen toward the key
    vec3 rv = reflect(rd, vec3(0.0, 1.0, 0.0));
    col += g_liq * pow(max(dot(rv, g_key), 0.0), 20.0) * 0.25 * keyLight;
    return col;
}
// surface ripple height + gradient (pop rings of the column under p, gentle swell)
vec3 surfaceH(vec2 xz) {
    float h = 0.0; vec2 g = vec2(0.0);
    // swell
    float s1 = sin(xz.x * 2.1 + g_t * 0.9), s2 = sin(xz.y * 1.7 - g_t * 0.7 + xz.x * 0.6);
    h += 0.006 * (s1 + s2) * ripples;
    g += 0.006 * ripples * (vec2(2.1 * cos(xz.x * 2.1 + g_t * 0.9), 0.0) + cos(xz.y * 1.7 - g_t * 0.7 + xz.x * 0.6) * vec2(0.6, 1.7));
    if (g_cellOn > 0.5) {
        for (int j = 0; j < NB; j++) {
            float pa = g_bpop[j];
            if (pa < 0.0 || pa > 2.2) continue;
            vec2 d = xz - g_bc[j].xz;
            float dist = length(d);
            float rr = pa * 0.85;
            float env = exp(-pa * 1.9) * smoothstep(0.0, 0.08, pa) * exp(-dist * 2.8) * 0.012 * ripples * (0.3 + 8.0 * g_br[j]);
            float ph = (dist - rr) * 26.0;
            h += env * sin(ph) * smoothstep(rr + 0.25, rr - 0.05, dist);
            g += env * cos(ph) * 26.0 * d / max(dist, 1e-3) * smoothstep(rr + 0.25, rr - 0.05, dist);
        }
    }
    return vec3(h, g);
}
vec3 aboveWorld(vec3 d, vec2 xz) {
    // the bright world over the surface: warm lamp + sky, caustic shimmer at the skin
    vec3 c = mix(g_liq, vec3(1.0), 0.75) * 1.7 * smoothstep(-0.2, 1.0, d.y);
    float k = keyLight * (1.0 + 0.4 * g_ar * g_beat);
    c += vec3(1.0, 0.97, 0.9) * pow(max(dot(d, g_key), 0.0), 40.0) * 5.0 * k;
    c += vec3(1.0, 0.95, 0.9) * pow(max(dot(d, g_key2), 0.0), 18.0) * 1.0 * k;
    c *= 0.6 + 0.9 * caustic(xz * 0.7 + 3.0, g_t * speed) * caustics;
    return c;
}
vec3 shadeSurface(vec3 p, vec3 rd) {
    vec3 hg = surfaceH(p.xz);
    vec3 n = normalize(vec3(-hg.y, 1.0, -hg.z));
    float cosv = clamp(dot(rd, n), 0.0, 1.0);
    float F0 = 0.02;
    float Fs = F0 + (1.0 - F0) * pow(1.0 - cosv, 5.0);
    float F = mix(Fs, 1.0, smoothstep(0.76, 0.66, cosv));
    vec3 rt = refract(rd, -n, 1.33);
    if (dot(rt, rt) < 0.5) rt = reflect(rd, n);
    vec3 above = aboveWorld(rt, p.xz);
    // mirror: march the bubbles below (short), else the liquid field
    vec3 rr = reflect(rd, n);
    vec3 refl = envL(rr, p.y);
    int mat;
    float tF = (rr.y < -0.001) ? (0.0 - p.y) / rr.y : 12.0;
    float t2 = marchBubbles(p + rr * 0.02, rr, 0.0, min(tF, 9.0), 40, mat);
    if (t2 > 0.0) {
        vec3 p2 = p + rr * (0.02 + t2);
        vec3 n2 = (mat == 1) ? normalize(vec3(0.0, p2.y - g_wireY, p2.z - g_wireZ)) : bubbleNormal(p2);
        vec3 c2 = (mat == 1) ? shadeWire(p2, rr) : shadeBubble(p2, rr, n2, true);
        refl = mix(c2, envL(rr, p.y), 1.0 - exp(-t2 * 0.16 / clarity));
    }
    vec3 col = refl * F + above * (1.0 - F);
    // ripple crests catch the light
    col += mix(g_liq, vec3(1.0), 0.7) * abs(hg.x) * 14.0 * keyLight;
    return col;
}

float shaftPat(vec3 p) {
    vec2 q = p.xz * 0.34 + vec2(0.1, 0.06) * (g_H - p.y) + 17.0;
    float t = g_t * speed * 0.6;
    float n1 = 1.0 - abs(2.0 * vnoise2(q * 2.1 + vec2(t * 0.13, -t * 0.09)) - 1.0);
    float n2 = 1.0 - abs(2.0 * vnoise2(q * 2.7 + vec2(-t * 0.08, t * 0.11) + 5.3) - 1.0);
    return pow(n1 * n2, 1.5) * 1.6;
}

vec4 renderScene(vec2 uv) {
    vec3 rd = camRay(uv);
    vec3 ro = g_ro;
    float tF = (rd.y < -0.001) ? (0.0 - ro.y) / rd.y : FAR;
    float tS = (rd.y > 0.001) ? (g_H - ro.y) / rd.y : FAR;
    float tEnd = min(min(tF, tS), FAR);
    loadCell(floor(ro.xz / CELL));
    int mat;
    float t = marchBubbles(ro, rd, 0.03, tEnd, 150, mat);
    vec3 col; float dep;
    if (t > 0.0) {
        vec3 p = ro + rd * t;
        if (mat == 1) col = shadeWire(p, rd);
        else { vec3 n = bubbleNormal(p); col = shadeBubble(p, rd, n, false); }
        dep = t;
    } else if (tF < tS && tF < FAR) {
        vec3 p = ro + rd * tF;
        loadCell(floor(p.xz / CELL));
        col = shadeFloor(p, rd);
        dep = tF;
    } else if (tS < FAR) {
        vec3 p = ro + rd * tS;
        loadCell(floor(p.xz / CELL));
        col = shadeSurface(p, rd);
        dep = tS;
    } else {
        col = envL(rd, ro.y + rd.y * 8.0);
        dep = FAR;
    }
    // liquid absorption toward the light field
    float fogK = 0.072 / clarity;
    col = mix(col, envL(rd, g_ro.y + rd.y * min(dep, 8.0) * 0.5), 1.0 - exp(-dep * fogK));
    // light shafts from the surface, integrated along the ray
    if (shafts > 0.001) {
        float tm = min(dep, 9.0);
        vec3 sh = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float tt = (float(i) + 0.5) / 8.0 * tm;
            vec3 p = ro + rd * tt;
            float a = shaftPat(p) * exp(-(g_H - p.y) * 0.42) * exp(-tt * 0.08);
            sh += vec3(a);
        }
        sh *= tm / 8.0;
        col += mix(g_liq, vec3(1.0), 0.35) * sh * shafts * 0.15 * keyLight * (1.0 + 0.5 * g_ar * g_lev);
    }
    return vec4(col, clamp(dep / FAR, 0.0, 1.0));
}

// ── pass 1: micro-bubble sparkle streams ─────────────────────────────────
vec3 microBubbles(vec2 uv, float sceneDepth) {
    vec3 col = vec3(0.0);
    float burst = g_ar * g_beat;
    for (int i = 0; i < 60; i++) {
        float fi = float(i);
        float st = floor(fi / 4.0);
        float h1 = hash11(st * 3.1 + 1.0), h2 = hash11(st * 7.3 + 2.0), h3 = hash11(fi * 5.7 + 3.0), h4 = hash11(fi * 2.9 + 4.0);
        vec2 nuc = vec2((h1 - 0.5) * 7.0, 1.2 + h2 * 6.0);
        float per = 2.5 + 2.0 * h3;
        float age = mod(g_t * speed * (0.8 + 0.5 * h4) + fi * 0.37, per);
        float y = 0.05 + age * g_H / per * 1.15;
        if (y > g_H - 0.05) continue;
        vec3 pw = vec3(nuc.x + 0.08 * sin(age * 3.0 + h4 * 6.0), y, nuc.y + 0.06 * cos(age * 2.6 + h3 * 6.0));
        vec3 pr = project(pw);
        if (pr.z < 0.4 || pr.z > sceneDepth) continue;
        float rad = clamp(0.012 * g_lens / pr.z, 0.0016, 0.012);
        vec2 d = uv - pr.xy;
        float dist = length(d);
        if (dist > rad * 1.4) continue;
        float tw = 0.5 + 0.5 * sin(g_t * (2.0 + 4.0 * h4) + h3 * TAU);
        tw = pow(tw, 2.0) * (1.0 + 1.6 * g_ar * g_high) + 0.25 + burst * 0.8;
        float disc = smoothstep(rad, rad * 0.7, dist);
        float rim = smoothstep(rad * 0.55, rad * 0.9, dist) * disc;
        float core = exp(-dist * dist / (rad * rad * 0.12));
        vec3 c = mix(g_liq, vec3(1.0), 0.6);
        col += c * (rim * 0.45 + core * 1.1 + disc * 0.3) * tw * exp(-pr.z * 0.12 / clarity) * sparkles;
    }
    return col;
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    g_ar = audioReact;
    g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_H = depth;
    g_merge = merge;
    g_sizeK = size;
    g_liq = hueRot(liquidColor.rgb, hueShift);
    g_deep = hueRot(backColor.rgb, hueShift) * backAmt;
    g_key = normalize(vec3(0.35, 1.0, -0.3));
    g_key2 = normalize(vec3(-0.7, 0.55, 0.5));
    g_wireZ = 3.1; g_wireY = 0.035; g_wireR = 0.008 + 0.008 * electrode;
    g_cell = vec2(1e5); g_cellOn = 0.0;
    setupCam();

    if (PASSINDEX == 0) {
        vec4 sc = renderScene(uv);
        gl_FragColor = vec4(max(sc.rgb, 0.0), sc.a);
    } else {
        vec2 st = fragCoord / R;
        vec2 asp = vec2(R.y / R.x, 1.0);
        vec4 c0 = texture2D(bbScene, st);
        float dep = c0.a * FAR;
        // chromatic fringe (radial, subtle)
        float ab = aberration * 0.0028 * (1.0 + 0.5 * g_ar * g_bass);
        vec2 dir = (st - 0.5) * 2.0;
        vec3 col = vec3(texture2D(bbScene, st + dir * ab).r, c0.g, texture2D(bbScene, st - dir * ab).b);
        col += microBubbles(uv, dep);
        // bloom from everything above white
        vec3 bl = vec3(0.0);
        float rad = 0.028;
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + hash12(fragCoord) * 6.28;
            float rr = sqrt((fi + 0.5) / 16.0) * rad;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            vec3 s = texture2D(bbScene, st + of).rgb;
            bl += max(s - 1.0, 0.0) * (1.0 - rr / rad);
        }
        bl /= 16.0;
        col += bl * bloom * 1.3;
        col *= mix(1.0, 0.9 + 0.22 * g_lev, g_ar);
        col = aces(col * 0.88);
        float l = lum(col);
        col = clamp(mix(vec3(l), col, saturation), 0.0, 1.0);
        float g1 = (hash12(fragCoord + fract(g_t) * 61.0) - 0.5) * 0.04;
        col += g1 * grain * (0.35 + 0.65 * l);
        col *= 1.0 - 0.1 * smoothstep(0.5, 1.5, length(uv));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.5);
        float lumHD = lum(col);
        col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
        float alphaOut = transparentBg ? max(1.0 - step(0.999, c0.a), smoothstep(0.02, 0.2, lum(bl))) : mix(1.0, backColor.a, backAmt * step(0.001, 1.0 - backColor.a));
        gl_FragColor = vec4(col, alphaOut);
    }
}
