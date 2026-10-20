/*{
  "DESCRIPTION": "Dunes — a living desert grown by the Werner sand-slab model: every grain-packet on the field is a slab that the wind lifts, hops downwind and drops (more readily on sand than on bare clay, always in the wind-shadow behind a crest), and wherever a slope gets steeper than the angle of repose the sand avalanches, so real barchans and transverse dunes form, migrate, collide and calve by themselves. The field is raised into a raymarched desert at golden hour: razor slip faces falling into deep violet shadow, long sun shadows raking across the stoss slopes, wind ripples combed across every gentle face, grain-flow streaks on the avalanche faces, cracked clay flats between the dunes, quartz glints, sand blowing off the brinks as glowing plumes, giant draa swells under it all and heat shimmer on the horizon while a low camera glides across the sea of sand. Bass gusts the wind (more transport and saltation), beats send a wave of blowing sand across the field, the level warms the light, highs sparkle the quartz.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #6",
  "CATEGORIES": ["Generator", "3D", "Simulation", "Nature", "Landscape", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Dune Migration",   "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "glide",       "LABEL": "Camera Glide",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Speed" },
    { "NAME": "orbitSpeed",  "LABEL": "Camera Turn",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "dayCycle",    "LABEL": "Sun Drift",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "windVeer",    "LABEL": "Wind Veer",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.3, "GROUP": "Sand" },
    { "NAME": "hopLength",   "LABEL": "Hop Length",       "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.0, "GROUP": "Sand" },
    { "NAME": "duneHeight",  "LABEL": "Dune Height",      "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Sand" },
    { "NAME": "draa",        "LABEL": "Draa Swells",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Sand" },
    { "NAME": "blowing",     "LABEL": "Blowing Sand",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Sand" },
    { "NAME": "ripples",     "LABEL": "Wind Ripples",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Sand" },
    { "NAME": "camHeight",   "LABEL": "Camera Height",    "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 4.0, "DEFAULT": 1.0, "GROUP": "Camera" },
    { "NAME": "camPitch",    "LABEL": "Camera Pitch",     "TYPE": "float", "AUDIO": false, "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Camera" },
    { "NAME": "sandColor",   "LABEL": "Sand",             "TYPE": "color", "DEFAULT": [0.86, 0.5, 0.26, 1.0],  "GROUP": "Color" },
    { "NAME": "flatColor",   "LABEL": "Clay Flats",       "TYPE": "color", "DEFAULT": [0.74, 0.47, 0.3, 1.0],  "GROUP": "Color" },
    { "NAME": "shadeColor",  "LABEL": "Shadow Sky Light", "TYPE": "color", "DEFAULT": [0.14, 0.2, 0.7, 1.0], "GROUP": "Color" },
    { "NAME": "sunColor",    "LABEL": "Sun",              "TYPE": "color", "DEFAULT": [1.0, 0.62, 0.36, 1.0],  "GROUP": "Color" },
    { "NAME": "skyTop",      "LABEL": "Sky Zenith",       "TYPE": "color", "DEFAULT": [0.1, 0.2, 0.5, 1.0],  "GROUP": "Color" },
    { "NAME": "skyHorizon",  "LABEL": "Sky Horizon",      "TYPE": "color", "DEFAULT": [0.96, 0.44, 0.2, 1.0],  "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "keyLight",    "LABEL": "Sun Strength",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "sunEl",       "LABEL": "Sun Height",       "TYPE": "float", "AUDIO": false, "MIN": 2.0, "MAX": 40.0, "DEFAULT": 11.0, "GROUP": "Light" },
    { "NAME": "sunAz",       "LABEL": "Sun Direction",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 360.0, "DEFAULT": 150.0, "GROUP": "Light" },
    { "NAME": "shadowAmt",   "LABEL": "Shadows",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "sparkle",     "LABEL": "Quartz Glints",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "haze",        "LABEL": "Haze",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "shimmer",     "LABEL": "Heat Shimmer",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7,  "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.1,  "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "marchRes",    "LABEL": "March Resolution", "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.68, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG", "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "prepA", "PERSISTENT": true, "WIDTH": "256", "HEIGHT": "256" },
    { "TARGET": "trnA",  "PERSISTENT": true, "WIDTH": "256", "HEIGHT": "256" },
    { "TARGET": "prepB", "PERSISTENT": true, "WIDTH": "256", "HEIGHT": "256" },
    { "TARGET": "trnB",  "PERSISTENT": true, "WIDTH": "256", "HEIGHT": "256" },
    { "TARGET": "avl1",  "PERSISTENT": true, "WIDTH": "256", "HEIGHT": "256" },
    { "TARGET": "avl2",  "PERSISTENT": true, "WIDTH": "256", "HEIGHT": "256" },
    { "TARGET": "avl3",  "PERSISTENT": true, "WIDTH": "256", "HEIGHT": "256" },
    { "TARGET": "disp",  "PERSISTENT": true, "WIDTH": "256", "HEIGHT": "256" },
    { "TARGET": "scene", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Dunes — CURVEBALL pack #6
//   The Werner (1995) sand-slab dune model, made gather-only so every texel
//   can run in parallel and sand is conserved EXACTLY:
//   pass 0/2 prep : per cell — height h (integer slabs), wind-shadow flag
//                   (an upwind cell rises above the 15° shadow line), erode
//                   flag (random pick, not shadowed, has sand)
//   pass 1/3 hop  : every eroded slab hops downwind by the hop vector v and
//                   tries to land (p = 0.6 on sand, 0.4 on bare, 1 in shadow,
//                   forced on the last hop). A cell gathers the slabs from
//                   the sources s = c − k·v whose chain of landing tests
//                   (hashed on the source, so every cell agrees) first
//                   succeeds at c.
//   pass 4-6 slide: angle of repose — each cell sends half its excess to its
//                   steepest neighbour beyond 2 slabs/cell (8-neighbour);
//                   a cell gathers what its neighbours sent to it.
//   pass 7 scene  : the slab field (B-spline) × draa swells, raymarched at
//                   golden hour; rgb + depth in alpha
//   pass 8 final  : heat shimmer, bloom, fringe, ACES, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define SIMN 256.0
#define FAR  170.0

const float CS    = 0.16;     // world units per sim cell
const float SLAB0 = 0.052;    // slab height: 2 slabs per cell ≈ 33° repose
const float TREP  = 2.0;      // avalanche threshold, slabs per cell
const float TSHD  = 0.82;     // 15° wind-shadow line, slabs per cell

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.3); }
float midP()   { return smoothstep(0.06, 0.85, audioMid); }
float highP()  { return smoothstep(0.10, 0.90, audioHigh); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float hash13(vec3 p3) { p3 = fract(p3 * 0.1031); p3 += dot(p3, p3.zyx + 31.32); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise(vec2 x) {
    vec2 p = floor(x), f = fract(x);
    f = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm3(vec2 p) { float a = 0.5, s = 0.0; for (int i = 0; i < 3; i++) { s += a * vnoise(p); p = p * 2.07 + 13.7; a *= 0.5; } return s; }
float fbm5(vec2 p) { float a = 0.5, s = 0.0; for (int i = 0; i < 5; i++) { s += a * vnoise(p); p = p * 2.03 + 7.3;  a *= 0.5; } return s; }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}
vec3 grade(vec3 c) {
    c = hueRot(c, hueShift);
    return max(mix(vec3(lum(c)), c, saturation), 0.0);
}

// ── shared state ─────────────────────────────────────────────────────────
float g_ar, g_bass, g_mid, g_high, g_beat, g_lev, g_gust, g_fr;
float g_slab;
float g_twil, g_isSun, g_storm;

// sandstorm clock, tied to the sun cycle: a ~15 s dust storm at every dawn
// (cyc 0.84–0.89, sun ≈ 5° up). It veils the sunrise while the field is rebuilt,
// so each golden hour opens on a fresh dense crescent sea.
// x = storm intensity 0..1, y = storm index (which fresh dune population to grow)
vec2 stormState() {
    float cr = TIME * 0.0034 * max(dayCycle, 0.25);
    float k = floor(cr);
    float u = cr - k;
    float I = smoothstep(0.84, 0.851, u) * smoothstep(0.89, 0.877, u);
    return vec2(I, k);
}
vec3  g_L, g_sunC, g_shade, g_skyT, g_skyH, g_sand, g_flat;

// ═════════════════════════════════════════════════════════════════════════
//  SIMULATION
// ═════════════════════════════════════════════════════════════════════════
vec4 prevTex(vec2 uv) {
    if (PASSINDEX == 0) return texture2D(avl3, uv);
    if (PASSINDEX == 1) return texture2D(prepA, uv);
    if (PASSINDEX == 2) return texture2D(trnA, uv);
    if (PASSINDEX == 3) return texture2D(prepB, uv);
    if (PASSINDEX == 4) return texture2D(trnB, uv);
    if (PASSINDEX == 5) return texture2D(avl1, uv);
    return texture2D(avl2, uv);
}
vec2 wrapC(vec2 c) { return mod(c, R); }
vec4 P(vec2 c) { return prevTex((wrapC(c) + 0.5) / R); }

// initial field: transverse ridges that break into barchans where sand is short.
// Built only from integer frequencies over the periodic domain so it tiles.
float barchan(vec2 d, float A, float W) {
    // d: offset from the dune centre in cells (x downwind). Crescent with horns
    // pointing downwind; stoss rises gently, slip face at the repose slope.
    float v = d.y / W;
    if (abs(v) >= 1.0) return 0.0;
    float side = 1.0 - v * v;
    float Ah = A * pow(side, 0.7);                   // lower toward the horns
    float L = 2.0 * Ah + 3.0;                        // stoss length
    float u = d.x - 0.55 * W * v * v;                // horns swept downwind
    float h = u < 0.0 ? Ah * (1.0 - pow(clamp(-u / L, 0.0, 1.0), 1.6))
                      : Ah - u * TREP;               // slip face
    return max(h, 0.0);
}

// initial field: a jittered, periodic population of barchans of many sizes on
// a thin sand sheet (built on the periodic domain so it tiles)
float seedH(vec2 c) {
    c = wrapC(c);
    vec2 G = R / vec2(7.0, 6.0);
    vec2 cell = floor(c / G);
    float h = 0.0;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 k = cell + vec2(float(i), float(j));
        vec2 kw = mod(k, vec2(7.0, 6.0));
        vec2 r = hash22(kw * 7.31 + 2.0);
        float A = 7.0 + 9.0 * hash12(kw * 3.17 + 9.0);
        vec2 ctr = (k + 0.3 + 0.4 * r) * G;
        h = max(h, barchan(c - ctr, A, 0.95 * A + 4.0));
    }
    float sheet = 1.5 * smoothstep(0.45, 0.8, hash12(floor(c / 9.0)) * 0.6 + 0.4 * sin(TAU * c.y / R.y * 2.0));
    return floor(max(h, sheet) + 0.5);
}

float hPrev(vec2 c) {
    if (PASSINDEX == 0 && FRAMEINDEX < 2) return seedH(c);
    return P(c).r;
}

// prevailing wind slowly swings ±25° so dunes realign, calve and stay crescents
vec2 windDir() { float th = 0.42 * sin(TIME * 0.0045 + 0.7) + 0.12 * sin(TIME * 0.011); return vec2(cos(th), sin(th)); }

float pDep(float h, float sh) { return sh > 0.5 ? 1.0 : (h > 0.5 ? 0.6 : 0.4); }

vec4 prepStep(vec2 c, float salt) {
    float h = hPrev(c);
    // wind-shadow: some upwind cell rises above the 15° line through this cell
    float sh = 0.0;
    vec2 wd = windDir();
    for (int d = 1; d <= 10; d++) {
        float fd = float(d);
        if (hPrev(floor(c - wd * fd + 0.5)) - h > fd * TSHD) sh = 1.0;
    }
    for (int d = 0; d < 13; d++) {
        float fd = 12.0 + 2.0 * float(d);
        if (hPrev(floor(c - wd * fd + 0.5)) - h > fd * TSHD) sh = 1.0;
    }
    // open system regulated toward a target sand budget: a 16-point sample of
    // the field estimates the mean depth; below target, wind drops new sand on
    // bare ground in drifting supply bands; above it, sand deflates anywhere.
    // Keeps a barchan field with open flats forever (no starving, no flooding).
    float meanH = 0.0;
    for (int k = 0; k < 16; k++) {
        vec2 sp = floor(fract(vec2(float(k) * 0.618034, float(k) * 0.381966 + 0.13) + TIME * 0.0007) * R);
        meanH += (FRAMEINDEX < 2) ? seedH(sp) : P(sp).r;
    }
    meanH /= 16.0;
    float target = 2.9;
    float rS = hash13(vec3(wrapC(c), g_fr * 0.913 + salt * 5.0));
    float band = 0.5 + 0.5 * sin(TAU * (wrapC(c).y / R.y * 3.0 + TIME * 0.004));
    float addP = clamp((target - meanH) * 0.004, 0.0, 0.006) * speed * (0.4 + band);
    float remP = clamp((meanH - target) * 0.003, 0.0, 0.006) * speed;
    // under cover of the storm the field is rebuilt toward a fresh dense
    // barchan population (redistributing slabs: removals ≈ additions because the
    // seeded population carries about the same sand budget)
    vec2 stS = stormState();
    if (stS.x > 0.05 && FRAMEINDEX > 2) {
        vec2 off = floor(hash22(vec2(stS.y * 3.7 + 1.3, 5.1)) * R);
        float tgt = seedH(c + off);
        float rr = hash13(vec3(wrapC(c), g_fr * 0.377 + salt * 9.0));
        float pS = 0.07 * stS.x;
        if (tgt > h + 0.5 && rr < pS) h += 1.0;
        else if (tgt < h - 0.5 && rr < pS) h -= 1.0;
    }
    if (h < 0.5 && rS < addP) h += 1.0;
    else if (h > 0.5 && rS > 1.0 - remP) h -= 1.0;
    float pE = 0.11 * speed * (1.0 + 0.7 * g_gust);
    float er = (h > 0.5 && sh < 0.5 && hash13(vec3(wrapC(c), g_fr * 0.731 + salt)) < pE) ? 1.0 : 0.0;
    return vec4(h, sh, er, 1.0);
}

vec2 hopVec(float salt) {
    float L = floor(hopLength + 0.5);
    vec2 wd = windDir();
    float r = hash13(vec3(g_fr * 0.37, salt, 7.7));
    float r2 = hash13(vec3(g_fr * 0.71, salt, 3.3));
    // stochastic rounding of the cross-wind component → the mean hop follows the wind
    float base = L * wd.y / max(wd.x, 0.5);
    float dy = floor(base + r2);
    float pl = 0.5 * windVeer, pr = 0.5 * windVeer;
    dy += r < pl ? -1.0 : (r > 1.0 - pr ? 1.0 : 0.0);
    return vec2(L, dy);
}

vec4 hopStep(vec2 c, float salt) {
    vec4 me = P(c);
    vec2 v = hopVec(salt);
    float arr = 0.0;
    for (int k = 1; k <= 5; k++) {
        float fk = float(k);
        vec2 s = wrapC(c - fk * v);
        vec4 S = P(s);
        if (S.b < 0.5) continue;
        bool alive = true;
        for (int i = 1; i < 5; i++) {
            if (i >= k) break;
            vec4 M = P(s + float(i) * v);
            if (hash13(vec3(s, g_fr * 0.519 + salt * 3.1 + float(i) * 0.137)) < pDep(M.r, M.g)) { alive = false; break; }
        }
        if (!alive) continue;
        if (k == 5 || hash13(vec3(s, g_fr * 0.519 + salt * 3.1 + fk * 0.137)) < pDep(me.r, me.g)) arr += 1.0;
    }
    return vec4(me.r - me.b + arr, 0.0, 0.0, 1.0);
}

vec2 nOff(int i) {
    if (i == 0) return vec2( 1.0,  0.0);
    if (i == 1) return vec2(-1.0,  0.0);
    if (i == 2) return vec2( 0.0,  1.0);
    if (i == 3) return vec2( 0.0, -1.0);
    if (i == 4) return vec2( 1.0,  1.0);
    if (i == 5) return vec2(-1.0, -1.0);
    if (i == 6) return vec2( 1.0, -1.0);
    return vec2(-1.0, 1.0);
}

// steepest over-repose neighbour of c and how many slabs slide there
vec3 slideOf(vec2 c, float salt) {
    c = wrapC(c);
    float h = P(c).r;
    float best = 0.0, bd = 0.0;
    vec2 bo = vec2(0.0);
    for (int i = 0; i < 8; i++) {
        vec2 o = nOff(i);
        float dist = (i < 4) ? 1.0 : 1.41421356;
        float d = h - P(c + o).r;
        float ex = d - TREP * dist;
        float sc = ex / dist + 0.002 * hash13(vec3(c + o * 0.37, g_fr * 0.11 + salt));
        if (ex > 0.0 && sc > best) { best = sc; bo = o; bd = ex; }
    }
    float amt = best > 0.0 ? max(1.0, floor(bd * 0.5)) : 0.0;
    return vec3(bo, amt);
}

vec4 slideStep(vec2 c, float salt) {
    float h = P(c).r;
    vec3 me = slideOf(c, salt);
    h -= me.z;
    for (int i = 0; i < 8; i++) {
        vec2 o = nOff(i);
        vec3 nb = slideOf(c + o, salt);
        if (nb.z > 0.0 && nb.x == -o.x && nb.y == -o.y) h += nb.z;
    }
    return vec4(h, 0.0, 0.0, 1.0);
}

// ═════════════════════════════════════════════════════════════════════════
//  TERRAIN
// ═════════════════════════════════════════════════════════════════════════
float simT(vec2 tc) { return texture2D(disp, fract(tc / SIMN)).r; }

// cubic B-spline over the slab field (4 bilinear taps) — smooth slopes, no steps
float simH(vec2 c) {
    vec2 st = c - 0.5;
    vec2 i = floor(st), f = st - i;
    vec2 f2 = f * f, f3 = f2 * f;
    vec2 w0 = (1.0 - 3.0 * f + 3.0 * f2 - f3) / 6.0;
    vec2 w1 = (4.0 - 6.0 * f2 + 3.0 * f3) / 6.0;
    vec2 w2 = (1.0 + 3.0 * f + 3.0 * f2 - 3.0 * f3) / 6.0;
    vec2 w3 = f3 / 6.0;
    vec2 g0 = w0 + w1, g1 = w2 + w3;
    vec2 h0 = i - 0.5 + w1 / g0;
    vec2 h1 = i + 1.5 + w3 / g1;
    return g0.y * (g0.x * simT(vec2(h0.x, h0.y)) + g1.x * simT(vec2(h1.x, h0.y)))
         + g1.y * (g0.x * simT(vec2(h0.x, h1.y)) + g1.x * simT(vec2(h1.x, h1.y)));
}

float duneMod(vec2 xz) { return 0.78 + 0.44 * vnoise(xz * 0.013 + 3.1); }
float draaH(vec2 xz)   { float n = vnoise(xz * 0.011 + 1.7); return draa * (2.4 * n * n * (3.0 - 2.0 * n) + 1.1 * vnoise(xz * vec2(0.03, 0.05) + 6.1)); }

float Hw(vec2 xz) { return simH(xz / CS) * g_slab * duneMod(xz) + draaH(xz); }
float Hs(vec2 xz) { return simH(xz / CS); }   // raw slabs (sand depth)

// Catmull-Rom over the slab field (16 exact taps): keeps the brinks razor sharp
// for shading normals while the march uses the smooth B-spline
float simT0(vec2 i) { return texture2D(avl3, fract((i + 0.5) / SIMN)).r; }
vec4 crW(float f) {
    float f2 = f * f, f3 = f2 * f;
    return vec4(-0.5 * f3 + f2 - 0.5 * f, 1.5 * f3 - 2.5 * f2 + 1.0, -1.5 * f3 + 2.0 * f2 + 0.5 * f, 0.5 * f3 - 0.5 * f2);
}
float simCR(vec2 c) {
    vec2 st = c - 0.5;
    vec2 i = floor(st), f = st - i;
    vec4 wx = crW(f.x), wy = crW(f.y);
    float acc = 0.0;
    for (int y = 0; y < 4; y++) {
        float fy = float(y) - 1.0;
        vec4 row = vec4(simT0(i + vec2(-1.0, fy)), simT0(i + vec2(0.0, fy)), simT0(i + vec2(1.0, fy)), simT0(i + vec2(2.0, fy)));
        float wyv = y == 0 ? wy.x : (y == 1 ? wy.y : (y == 2 ? wy.z : wy.w));
        acc += wyv * dot(row, wx);
    }
    return acc;
}
float HwS(vec2 xz) { return simCR(xz / CS) * g_slab * duneMod(xz) + draaH(xz); }
vec3 terrainNS(vec2 xz, float e) {
    float hx = HwS(xz + vec2(e, 0.0)) - HwS(xz - vec2(e, 0.0));
    float hz = HwS(xz + vec2(0.0, e)) - HwS(xz - vec2(0.0, e));
    return normalize(vec3(-hx, 2.0 * e, -hz));
}

vec3 terrainN(vec2 xz, float e) {
    float hx = Hw(xz + vec2(e, 0.0)) - Hw(xz - vec2(e, 0.0));
    float hz = Hw(xz + vec2(0.0, e)) - Hw(xz - vec2(0.0, e));
    return normalize(vec3(-hx, 2.0 * e, -hz));
}

// ═════════════════════════════════════════════════════════════════════════
//  SKY
// ═════════════════════════════════════════════════════════════════════════
vec3 sky(vec3 rd, bool withSun) {
    float y = max(rd.y, 0.0);
    // three-stop golden-hour gradient: amber horizon → pale gold band → clean blue
    vec3 band = mix(g_skyH, g_skyH * 1.12 + vec3(0.06, 0.04, 0.0) * g_isSun, 0.6);
    vec3 c = mix(g_skyH, band, smoothstep(0.0, 0.025, y));
    c = mix(c, g_skyT, smoothstep(0.0, 1.0, pow(clamp(y / 0.32, 0.0, 1.0), 0.7)));
    // cool band just above the horizon on the anti-sun side (earth-shadow belt)
    float anti = max(-dot(normalize(vec3(rd.x, 0.0, rd.z)), normalize(vec3(g_L.x, 0.0, g_L.z))), 0.0);
    c = mix(c, c * vec3(0.78, 0.7, 1.05), anti * exp(-y * 9.0) * 0.6);
    float sd = max(dot(rd, g_L), 0.0);
    c += g_sunC * (0.12 * pow(sd, 8.0) + 0.55 * pow(sd, 48.0));
    if (rd.y > 0.0) {
        vec2 uv = rd.xz / (rd.y + 0.06);
        float cl = fbm5(uv * vec2(0.35, 1.6) + vec2(TIME * 0.006, 0.0));
        cl = smoothstep(0.6, 0.9, cl) * smoothstep(0.0, 0.15, rd.y) * (1.0 - smoothstep(0.5, 0.9, rd.y));
        vec3 clc = mix(g_skyT * 1.4, g_sunC * 1.6, 0.35 + 0.65 * pow(sd, 3.0));
        c = mix(c, clc, cl * 0.55);
    }
    // far mesas on the horizon: violet silhouettes, hazed into the sky
    if (withSun) {
        vec2 hz = normalize(rd.xz);                 // seamless around the horizon
        float b1 = fbm3(hz * 2.2 + 3.0), b2 = fbm3(hz * 9.0 + 11.0);
        float mesa = smoothstep(0.52, 0.56, b1);    // flat-topped buttes with steep sides
        float mh = 0.011 * mesa * (0.8 + 0.4 * b2);
        float m = smoothstep(mh + 0.0008, mh - 0.0008, rd.y) * step(-0.02, rd.y) * step(0.001, mh);
        vec3 mc = mix(g_skyH * 0.7, g_shade * 0.9, 0.35);
        mc += g_sunC * 0.18 * pow(max(dot(rd, g_L), 0.0), 4.0);
        c = mix(c, mc, m * 0.85);
    }
    if (withSun) {
        float disk = smoothstep(0.99955, 0.99975, dot(rd, g_L));
        c += g_sunC * vec3(1.0, 0.9, 0.75) * disk * 7.0 * g_isSun;
        // night: stars + crisp moon disk
        float nightK = 1.0 - g_twil;
        if (nightK > 0.01 && rd.y > 0.0) {
            vec2 sc = vec2(atan(rd.z, rd.x) * 260.0, asin(rd.y) * 260.0);
            vec2 ci = floor(sc);
            float hs = hash12(ci);
            vec2 fo = fract(sc) - 0.5 - (hash22(ci) - 0.5) * 0.6;
            float st = step(0.985, hs) * smoothstep(0.32, 0.0, length(fo)) * (0.4 + 3.0 * pow(hash12(ci + 7.0), 6.0));
            st *= 0.75 + 0.25 * sin(TIME * 2.0 + hs * 50.0);
            c += vec3(0.85, 0.9, 1.0) * st * nightK * smoothstep(0.0, 0.12, rd.y);
            c += vec3(0.22, 0.2, 0.35) * 0.06 * nightK * smoothstep(0.55, 0.85, fbm3(sc * 0.004 + 2.0));  // faint milky band
        }
    }
    return c;
}

// ═════════════════════════════════════════════════════════════════════════
//  SCENE
// ═════════════════════════════════════════════════════════════════════════
float sunShadow(vec3 p) {
    float res = 1.0, t = 0.04;
    for (int i = 0; i < 22; i++) {
        vec3 q = p + g_L * t;
        float h = q.y - Hw(q.xz);
        res = min(res, 26.0 * h / t);
        if (res < 0.002 || t > 45.0) break;
        t += clamp(h * 0.9, 0.08, 2.2);
    }
    return clamp(res, 0.0, 1.0);
}

// rounded pebbles: x = coverage (anti-aliased by footprint aa), y = stone id, z = top light
vec3 pebbles(vec2 p, float aa) {
    vec2 n = floor(p), f = fract(p);
    float best = 8.0, id = 0.0, rr = 0.3;
    vec2 rel = vec2(0.0);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 g = vec2(float(i), float(j));
        vec2 o = hash22(n + g);
        vec2 r = g + o - f;
        float hp = hash12(n + g + 4.1);
        float rad = hp > 0.62 ? 0.1 + 0.3 * (hp - 0.62) : -1.0;   // only ~1/3 of cells hold a stone
        float d = length(r) - rad;
        if (d < best) { best = d; id = hash12(n + g + 9.7); rr = rad; rel = r; }
    }
    float cov = smoothstep(aa + 0.02, -aa - 0.02, best);
    float top = clamp(0.5 + 0.9 * dot(-rel / max(rr, 0.05), normalize(vec2(-0.6, 0.4))), 0.0, 1.0);
    return vec3(cov, id, top);
}

float voronoiEdge(vec2 p) {
    vec2 n = floor(p), f = fract(p);
    float d1 = 8.0, d2 = 8.0;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 g = vec2(float(i), float(j));
        vec2 o = hash22(n + g);
        vec2 r = g + o - f;
        float d = dot(r, r);
        if (d < d1) { d2 = d1; d1 = d; } else if (d < d2) { d2 = d; }
    }
    return sqrt(d2) - sqrt(d1);
}

vec4 renderScene(vec2 fc) {
    vec2 uv = (fc - 0.5 * R) / R.y;

    // ── camera: low glide across the field, slowly turning ───────────────
    float T = TIME;
    // swings between cross-wind views (slip faces in profile), never straight into the sun
    float ang = 1.75 + 0.5 * sin(T * 0.017 * orbitSpeed) + 0.12 * sin(T * 0.041 * orbitSpeed + 1.0);
    vec3 fwd = normalize(vec3(cos(ang), 0.0, sin(ang)));
    vec2 cxz = vec2(13.0, 7.0) + vec2(T * 0.55, T * 0.24) * glide;
    float gh = 0.0;
    gh = max(gh, Hw(cxz));
    gh = max(gh, Hw(cxz + fwd.xz * 2.5));
    gh = max(gh, Hw(cxz + vec2(fwd.z, -fwd.x) * 2.0));
    gh = max(gh, Hw(cxz - vec2(fwd.z, -fwd.x) * 2.0));
    vec3 ro = vec3(cxz.x, gh + 2.2 * camHeight + 0.2, cxz.y);
    float pitch = -0.14 + 0.25 * camPitch;
    vec3 ww = normalize(fwd + vec3(0.0, pitch, 0.0));
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.55 * ww);

    // ── march the heightfield ────────────────────────────────────────────
    float t = 0.05, hitT = -1.0;
    float lastT = t, lastD = ro.y - Hw(ro.xz);
    for (int i = 0; i < 120; i++) {
        vec3 p = ro + rd * t;
        float d = p.y - Hw(p.xz);
        if (d < 0.0) {
            // bisection refine between lastT and t
            float a = lastT, b = t;
            for (int k = 0; k < 6; k++) {
                float m = 0.5 * (a + b);
                vec3 q = ro + rd * m;
                if (q.y - Hw(q.xz) < 0.0) b = m; else a = m;
            }
            hitT = 0.5 * (a + b);
            break;
        }
        lastT = t; lastD = d;
        t += clamp(d * 0.6, 0.015 + t * 0.004, 3.0);
        if (t > FAR || p.y > 14.0 && rd.y > 0.0) break;
    }

    vec3 col;
    float depth = 1.0;
    vec2 windD = windDir();
    float pixW = 2.0 / (R.y * 1.55);   // world size of one pixel per unit distance

    if (hitT < 0.0) {
        col = sky(rd, true);
        // below the far edge of the terrain: the same haze the terrain fades into
        vec3 fogM = sky(normalize(vec3(rd.x, 0.0, rd.z)), false) * 0.92;
        col = mix(col, fogM, smoothstep(0.012, -0.004, rd.y));
    } else {
        vec3 p = ro + rd * hitT;
        float ft = pixW * hitT;                         // pixel footprint
        vec3 n = terrainN(p.xz, max(0.02, ft * 1.2));
        ft /= sqrt(max(abs(dot(n, rd)), 0.1));         // grazing views stretch the footprint
        float slabs = Hs(p.xz);
        float sandAmt = smoothstep(0.15, 1.4, slabs);
        float slope = 1.0 - n.y;
        float slip = smoothstep(0.32, 0.5, length(n.xz) / max(n.y, 0.05) * 0.55);  // ~ >25°
        float lee = smoothstep(0.0, 0.3, dot(n.xz, windD));   // faces the downwind side
        slip *= lee;

        // wind ripples: crests across the wind, sinuous, gone on slip faces & flats
        float rf = 52.0;
        float wob = 2.4 * vnoise(p.xz * vec2(0.8, 2.1)) + 0.9 * vnoise(p.xz * 3.7);
        float ph = p.x * rf + wob * 2.6 + 0.5 * vnoise(p.xz * 9.0);
        float dprof = cos(ph) + 0.62 * cos(2.0 * ph);
        // fine secondary ripples + grain, only right under the camera
        float ph2 = dot(p.xz, vec2(windD.x, windD.y)) * 150.0 + 3.0 * vnoise(p.xz * 6.0);
        dprof += 0.45 * cos(ph2) * (1.0 - smoothstep(0.003, 0.009, ft));
        float rFade = (1.0 - smoothstep(0.012, 0.035, ft)) * (1.0 - slip) * mix(0.85, 1.0, sandAmt) * ripples;
        vec3 nr = normalize(n + vec3(-0.055 * dprof * rFade, 0.0, -0.03 * dprof * rFade * (vnoise(p.xz * 1.3) - 0.5)));

        // grain-flow streaks on avalanche faces (run down-slope)
        vec2 dn = normalize(n.xz + 1e-4);
        vec2 across = vec2(-dn.y, dn.x);
        float gfl = vnoise(vec2(dot(p.xz, across) * 26.0, dot(p.xz, dn) * 1.5));
        float gflF = slip * (1.0 - smoothstep(0.01, 0.04, ft));
        nr = normalize(nr + vec3(across.x, 0.0, across.y) * (gfl - 0.5) * 0.1 * gflF);

        // albedo
        vec3 alb = g_sand * (0.92 + 0.08 * fbm3(p.xz * 0.6) + 0.05 * (hash12(floor(p.xz * 160.0)) - 0.5) * (1.0 - smoothstep(0.002, 0.006, ft)));
        alb *= mix(1.0, 0.86 + 0.14 * gfl, gflF);
        alb = mix(alb, alb * vec3(1.08, 1.05, 1.0), smoothstep(0.5, 1.0, slabs / 26.0));   // sun-bleached crests
        // gravel plain (serir) between the dunes: darker pebble speckle, wind streaks
        vec3 pebC = pebbles(p.xz * 13.0, ft * 13.0);
        float peb = pebC.x * (1.0 - smoothstep(0.012, 0.04, ft));
        float strk = vnoise(vec2(p.x * 1.2, p.z * 9.0)) * (1.0 - smoothstep(0.01, 0.04, ft));
        vec3 clay = g_flat * (0.88 + 0.1 * vnoise(p.xz * 1.9) + 0.08 * strk);
        vec3 stone = mix(vec3(0.1, 0.065, 0.05), vec3(0.42, 0.18, 0.09), step(0.6, pebC.y));
        stone *= 0.75 + 0.5 * pebC.z;
        clay = mix(clay, stone, peb * (1.0 - smoothstep(0.3, 1.2, slabs)));
        alb = mix(clay, alb, sandAmt);

        // lighting
        float dif = max(dot(nr, g_L), 0.0);
        float shd = mix(1.0, sunShadow(p + n * 0.02), shadowAmt);
        float ao = clamp(0.55 + 0.45 * (p.y - 0.5 * (Hw(p.xz + vec2(0.64, 0.64)) + Hw(p.xz - vec2(0.64, 0.64)))) * 2.2 + 0.45, 0.0, 1.0);
        vec3 sunL = g_sunC * keyLight * 3.5 * pow(dif, 0.85) * shd;
        // sand back-scatter: brighter looking down-sun
        sunL *= 1.0 + 0.35 * pow(max(dot(-rd, g_L), 0.0), 3.0);
        vec3 skyL = g_shade * (0.35 + 0.65 * (0.5 + 0.5 * nr.y)) * ao * 0.75;
        vec3 bnc = g_sunC * g_sand * 0.22 * max(dot(nr, -normalize(vec3(g_L.x, 0.0, g_L.z))), 0.0) * ao;
        col = alb * (sunL + skyL + bnc);

        // quartz glints: sparse crisp sparkles on lit sand
        vec2 gc = floor(p.xz * 140.0);
        float gh2 = hash12(gc);
        float gl = step(0.9935 - 0.004 * g_high * g_ar, gh2) * dif * shd * sandAmt;
        gl *= 0.5 + 0.5 * sin(TIME * 3.0 + gh2 * 60.0 + dot(rd.xz, vec2(40.0)));
        gl *= 1.0 - smoothstep(0.002, 0.005, ft);
        col += g_sunC * gl * 2.5 * sparkle * (1.0 + 1.2 * g_high * g_ar);

        // aerial haze, brighter toward the sun
        float fogA = 1.0 - exp(-pow(hitT * 0.0085 * haze * (1.0 + 3.2 * g_storm), 1.35));
        fogA = max(fogA, smoothstep(0.8 * FAR, FAR, hitT));
        vec3 fogC = sky(normalize(vec3(rd.x, 0.0, rd.z)), false) * 0.92;
        col = mix(col, fogC, fogA);
        depth = hitT / FAR;
    }

    // ── blowing sand: plumes off the brinks + a saltation sheet in gusts ─
    if (blowing > 0.001 || g_storm > 0.0) {
        float tEnd = hitT < 0.0 ? 30.0 : min(hitT, 30.0);
        float ign = fract(52.9829189 * fract(dot(fc, vec2(0.06711056, 0.00583715))));
        float dens = 0.0;
        float dt = tEnd / 10.0;
        float beatBand = g_beat * g_ar;
        for (int i = 0; i < 10; i++) {
            float tt = (float(i) + 0.5 + 0.35 * (ign - 0.5)) * dt;
            vec3 q = ro + rd * tt;
            float hs = Hw(q.xz);
            float above = q.y - hs;
            if (above < 0.0) continue;
            float brink = smoothstep(0.08, 0.3, Hw(q.xz - windD * 0.35) - hs);
            vec2 adv = q.xz * 1.1 - vec2(TIME * 1.9 * (0.7 + 0.6 * g_gust), 0.0);
            float tex = fbm3(adv + vec2(0.0, q.y * 2.0));
            float plume = brink * exp(-above / 0.16) * smoothstep(0.45, 0.8, tex);
            float sheet = exp(-above / 0.07) * (0.08 + 1.6 * g_gust) * smoothstep(0.45, 0.75, tex);
            float wave = exp(-pow(fract(q.x / 34.0 - TIME * 0.35) - 0.5, 2.0) * 120.0) * beatBand * 2.0;
            dens += (plume * (1.0 + 1.5 * g_gust) + sheet + wave * exp(-above / 0.3)) * dt;
        }
        dens *= 0.16 * blowing * (1.0 + 3.5 * g_storm);
        dens += g_storm * 0.5 * (0.5 + 0.5 * fbm3(fc / R.y * vec2(3.0, 9.0) - vec2(TIME * 0.9, 0.0)));
        float sd = dot(rd, g_L);
        float hg = (1.0 - 0.36) / pow(1.0 + 0.36 - 1.2 * sd, 1.5) * 0.35 + 0.15;
        vec3 dustC = g_sunC * g_sand * 1.9 * hg * keyLight + g_shade * 0.12;
        float a = 1.0 - exp(-dens);
        col = mix(col, dustC, a);
    }

    col *= 1.0 + g_ar * (0.3 * g_bass + 0.15 * g_mid + 0.1 * g_lev);
    return vec4(col, depth);
}

// ═════════════════════════════════════════════════════════════════════════
void main() {
    vec2 fc = gl_FragCoord.xy;
    g_ar = audioReact;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_gust = g_ar * (0.65 * g_bass + 0.35 * g_lev);
    g_fr = mod(float(FRAMEINDEX), 8192.0);

    if (PASSINDEX == 0) { gl_FragColor = prepStep(floor(fc), 1.0); return; }
    if (PASSINDEX == 1) { gl_FragColor = hopStep(floor(fc), 1.0);  return; }
    if (PASSINDEX == 2) { gl_FragColor = prepStep(floor(fc), 2.0); return; }
    if (PASSINDEX == 3) { gl_FragColor = hopStep(floor(fc), 2.0);  return; }
    if (PASSINDEX <= 6) { gl_FragColor = slideStep(floor(fc), float(PASSINDEX)); return; }
    if (PASSINDEX == 7) {
        // display height: 3×3 binomial blur of the slab field, eased over ~8 frames —
        // removes the 1-slab quantisation steps and the per-frame hop flicker
        vec2 c = floor(fc);
        float acc = 0.0;
        for (int j = -1; j <= 1; j++)
        for (int i = -1; i <= 1; i++) {
            float w = (i == 0 ? 2.0 : 1.0) * (j == 0 ? 2.0 : 1.0);
            acc += w * texture2D(avl3, (mod(c + vec2(float(i), float(j)), R) + 0.5) / R).r;
        }
        acc /= 16.0;
        // brink sharpening: unsharp-mask against a wider ring so slip-face crests
        // come out as knife lines instead of rounded humps
        float wide = 0.0;
        for (int k = 0; k < 8; k++) {
            vec2 o = nOff(k) * 2.0;
            wide += texture2D(avl3, (mod(c + o, R) + 0.5) / R).r;
        }
        wide /= 8.0;
        acc = acc + 1.1 * max(acc - wide - 0.55, 0.0);   // crests only — slope noise stays untouched
        // thin sand sheets (scattered fresh slabs) read as a flat veneer, not lumps
        acc = acc < 2.4 ? acc * 0.1 : acc - 2.16;
        float prev = texture2D(disp, (c + 0.5) / R).r;
        gl_FragColor = vec4(FRAMEINDEX < 3 ? acc : mix(prev, acc, 0.12), 0.0, 0.0, 1.0);
        return;
    }

    // lights / palette (scene + final)
    // ~5-minute cycle at dayCycle 1: golden hour → sunset → blue hour → night → dawn
    float cyc = fract(TIME * 0.0034 * dayCycle);
    float elD = mix(-13.0, sunEl, 0.5 + 0.5 * cos(TAU * cyc));
    float el = radians(elD);
    float az = radians(sunAz) + 0.5 * sin(TAU * cyc);
    vec3 sunL = normalize(vec3(cos(el) * cos(az), sin(el), cos(el) * sin(az)));
    float dayF = smoothstep(-0.015, 0.04, sunL.y);
    g_twil = smoothstep(-0.2, -0.02, sunL.y);                  // 1 = day / blue hour, 0 = deep night
    vec3 moonL = normalize(vec3(-cos(az + 0.9) * 0.8, 0.42, -sin(az + 0.9) * 0.8));
    g_isSun = dayF;
    g_L = dayF > 0.001 ? sunL : moonL;
    float warm = smoothstep(0.03, 0.35, el);
    g_sunC = grade(sunColor.rgb * mix(vec3(1.05, 0.78, 0.55), vec3(1.0), warm));
    vec3 moonC = vec3(0.42, 0.52, 0.8) * 0.4 * (1.0 - g_twil * 0.7);
    g_sunC = dayF > 0.001 ? g_sunC * smoothstep(-0.015, 0.06, sunL.y) : moonC;
    // palette through the cycle: day colours → violet/indigo blue hour → night navy
    vec3 blueT = vec3(0.04, 0.07, 0.24), blueH = vec3(0.44, 0.32, 0.6);
    vec3 nightT = vec3(0.004, 0.007, 0.022), nightH = vec3(0.03, 0.045, 0.1);
    g_skyT = grade(mix(mix(nightT, blueT, g_twil), skyTop.rgb, dayF));
    g_skyH = grade(mix(mix(nightH, blueH, g_twil), skyHorizon.rgb, dayF));
    // skylight fill: strong cool blue in the blue hour (the light IS the sky then)
    g_shade = grade(mix(mix(vec3(0.012, 0.02, 0.06), vec3(0.22, 0.26, 0.7) * 1.25, g_twil), shadeColor.rgb, dayF));
    g_sand = grade(sandColor.rgb);
    g_flat = grade(flatColor.rgb);
    g_slab = SLAB0 * duneHeight;
    g_storm = stormState().x;
    // storm: sun dims and reddens, skylight turns to dusty ochre
    g_sunC *= 1.0 - 0.65 * g_storm;
    g_shade = mix(g_shade, g_sand * 0.55 * (0.3 + 0.7 * g_twil), 0.7 * g_storm);
    g_skyH = mix(g_skyH, g_sand * 0.8 * (0.25 + 0.75 * g_twil), 0.75 * g_storm);
    g_skyT = mix(g_skyT, g_sand * 0.5 * (0.25 + 0.75 * g_twil), 0.6 * g_storm);

    if (PASSINDEX == 8) { gl_FragColor = renderScene(fc); return; }

    // ── final ────────────────────────────────────────────────────────────
    vec2 st = fc / R;
    vec2 uvc = (fc - 0.5 * R) / R.y;
    vec2 asp = vec2(R.y / R.x, 1.0);
    float dep = texture2D(scene, st).a;
    // heat shimmer: far ground only, rising wobble
    float shm = shimmer * smoothstep(0.18, 0.9, dep) * step(dep, 0.999);
    vec2 wob = vec2(vnoise(vec2(st.x * 60.0, st.y * 160.0 - TIME * 3.0)) - 0.5,
                    vnoise(vec2(st.x * 70.0 + 9.0, st.y * 190.0 - TIME * 3.6)) - 0.5) * 0.0022 * shm;
    vec2 sst = st + wob;
    float ab = aberration * 0.0016 * length(uvc);
    vec2 dir = normalize(uvc + 1e-4);
    vec4 c0 = texture2D(scene, sst);
    vec3 col = vec3(texture2D(scene, sst + dir * ab * asp).r, c0.g, texture2D(scene, sst - dir * ab * asp).b);
    vec3 bl = vec3(0.0);
    for (int i = 0; i < 16; i++) {
        float fi = float(i);
        float a = fi * 2.399963;
        float rr = sqrt((fi + 0.5) / 16.0) * 0.035;
        vec3 s = texture2D(scene, st + vec2(cos(a), sin(a)) * rr * asp).rgb;
        bl += max(s - 1.1, 0.0) * (1.0 - rr / 0.035);
    }
    col += bl / 16.0 * 2.2 * bloom;

    col = aces(col * 0.95);
    col = pow(col, vec3(0.94));
    // HD finisher
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float l = dot(col, vec3(0.299, 0.587, 0.114));
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    float vig = 1.0 - 0.12 * dot(uvc * 0.9, uvc * 0.9);
    col *= vig;
    col += (hash12(fc + fract(TIME) * 61.0) - 0.5) * 0.035 * grain;
    float alpha = transparentBg ? (dep < 0.999 ? 1.0 : 0.0) : 1.0;
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), alpha);
}
