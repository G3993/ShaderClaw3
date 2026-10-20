/*{
  "DESCRIPTION": "Molecules — ball-and-stick chemistry in a lit 3D studio. Real molecules tumble in from the fog, meet over a glossy floor and REACT: bonds shiver harder and harder, dissolve in a flash, and the atoms re-pair into the products (CO2, water, ammonia, ethane…) which drift apart and leave, then the next reaction rolls in — hydrogen burning (2 H2 + O2 → 2 H2O), methane combustion, the Haber process, ethene hydrogenation, carbon monoxide oxidation and benzene's Kekulé resonance flip. Atoms are glossy CPK-coloured spheres (carbon dark, hydrogen white, oxygen red, nitrogen blue) with Fresnel rims, crisp speculars, soft key-light shadows and contact occlusion; bonds are two-tone rods, double and triple bonds as twin and triple rods; an optional electron cloud glows around the atoms. Bass shakes the bonds, beats flash the rods white, highs sparkle the highlights, the level lifts the key light.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-13 — original. CIDKID chemistry pack #4",
  "CATEGORIES": ["Generator", "3D", "Chemistry", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Reaction Speed",     "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "tumble",       "LABEL": "Tumble",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "orbitSpeed",   "LABEL": "Camera Orbit",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "molecule",     "LABEL": "Reaction (0 H2+O2, 1 CH4, 2 Haber, 3 Ethene, 4 CO, 5 Benzene)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 5.0, "DEFAULT": 1.0, "GROUP": "Molecules" },
    { "NAME": "autoCycle",    "LABEL": "Cycle Reactions",    "TYPE": "bool",  "DEFAULT": true, "GROUP": "Molecules" },
    { "NAME": "atomSize",     "LABEL": "Atom Size",          "TYPE": "float", "AUDIO": false, "MIN": 0.5,  "MAX": 1.6,  "DEFAULT": 1.0,  "GROUP": "Molecules" },
    { "NAME": "bondSize",     "LABEL": "Bond Thickness",     "TYPE": "float", "AUDIO": false, "MIN": 0.4,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Molecules" },
    { "NAME": "spread",       "LABEL": "Spread",             "TYPE": "float", "AUDIO": false, "MIN": 0.6,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Molecules" },
    { "NAME": "vibration",    "LABEL": "Bond Vibration",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Molecules" },
    { "NAME": "flash",        "LABEL": "Reaction Flash",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Molecules" },
    { "NAME": "cloud",        "LABEL": "Electron Cloud",     "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.45, "GROUP": "Molecules" },
    { "NAME": "zoom",         "LABEL": "Zoom",               "TYPE": "float", "AUDIO": false, "MIN": 0.5,  "MAX": 2.2,  "DEFAULT": 1.0,  "GROUP": "Camera" },
    { "NAME": "camHeight",    "LABEL": "Camera Height",      "TYPE": "float", "AUDIO": false, "MIN": -0.2, "MAX": 1.0,  "DEFAULT": 0.3,  "GROUP": "Camera" },
    { "NAME": "floorOn",      "LABEL": "Floor",              "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 1.0,  "GROUP": "Scene" },
    { "NAME": "floorReflect", "LABEL": "Floor Reflection",   "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55, "GROUP": "Scene" },
    { "NAME": "fog",          "LABEL": "Fog",                "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Scene" },
    { "NAME": "backColor",    "LABEL": "Background Color",   "TYPE": "color", "DEFAULT": [0.05, 0.06, 0.085, 1.0], "GROUP": "Color" },
    { "NAME": "backAmt",      "LABEL": "Background Amount",  "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "floorColor",   "LABEL": "Floor Color",        "TYPE": "color", "DEFAULT": [0.22, 0.23, 0.27, 1.0], "GROUP": "Color" },
    { "NAME": "candy",        "LABEL": "Candy Colors",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",   "LABEL": "Saturation",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "keyLight",     "LABEL": "Key Light",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "lightAz",      "LABEL": "Light Direction",    "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 360.0,"DEFAULT": 215.0,"GROUP": "Light" },
    { "NAME": "lightEl",      "LABEL": "Light Height",       "TYPE": "float", "AUDIO": false, "MIN": 10.0, "MAX": 80.0, "DEFAULT": 48.0, "GROUP": "Light" },
    { "NAME": "fill",         "LABEL": "Fill Light",         "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "glint",        "LABEL": "Glints",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "rimLight",     "LABEL": "Colored Rim Light",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "haze",         "LABEL": "Light Haze",         "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "dust",         "LABEL": "Dust",               "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bloom",        "LABEL": "Bloom",              "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7,  "GROUP": "Finish" },
    { "NAME": "exposure",     "LABEL": "Exposure",           "TYPE": "float", "AUDIO": false, "MIN": 0.5,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Finish" },
    { "NAME": "aberration",   "LABEL": "Chromatic Fringe",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.2,  "GROUP": "Finish" },
    { "NAME": "grain",        "LABEL": "Grain",              "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.2,  "GROUP": "Finish" },
    { "NAME": "marchRes",     "LABEL": "March Resolution",   "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 1.0,  "DEFAULT": 0.7,  "GROUP": "Finish" },
    { "NAME": "audioReact",   "LABEL": "Audio React",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",     "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "moScene", "WIDTH": "floor($WIDTH*$marchRes)", "HEIGHT": "floor($HEIGHT*$marchRes)" },
    { "TARGET": "moHaze",  "WIDTH": "floor($WIDTH*0.5)",       "HEIGHT": "floor($HEIGHT*0.5)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Molecules — CIDKID chemistry pack #4
//   pass 0  moScene — raymarched ball-and-stick molecules over a glossy floor:
//                     atoms (spheres) + bonds (capsules, twin/triple rods),
//                     key light with iq soft shadows, contact AO, Fresnel env,
//                     floor reflection march, electron-cloud glow accumulated
//                     along the ray, reaction flash. a = 1 geometry / 0.9 sky
//   pass 1  moHaze  — half-res volumetric key-light beam, shadowed by the atoms
//   pass 2  final   — chromatic fringe, HDR bloom, exposure, ACES, S-curve,
//                     saturation, grain, vignette, HD finisher
//
//   The simulation: a cycle of length 16/speed s. u = phase in [0,1).
//     u 0.00–0.45  reactant molecules tumble in from the fog toward the origin
//     u 0.40–0.50  bonds shiver, thin out and dissolve; flash at 0.5
//     u 0.45–0.55  atoms travel from their reactant to their product slots
//     u 0.50–0.60  product bonds grow in
//     u 0.55–1.00  product molecules drift apart into the fog and fade
//   At the cycle boundary everything is tiny and far away, so switching to the
//   next reaction there is invisible.
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define MAXA 12
#define MAXB 12
#define MAXF 8
#define SKY  60.0

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
mat3 rotX(float a) { float c = cos(a), s = sin(a); return mat3(1.0, 0.0, 0.0, 0.0, c, s, 0.0, -s, c); }
mat3 rotY(float a) { float c = cos(a), s = sin(a); return mat3(c, 0.0, -s, 0.0, 1.0, 0.0, s, 0.0, c); }
mat3 rotZ(float a) { float c = cos(a), s = sin(a); return mat3(c, s, 0.0, -s, c, 0.0, 0.0, 0.0, 1.0); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }

// ── state ────────────────────────────────────────────────────────────────
float g_t, g_ar, g_lens, g_u, g_s, g_flash, g_beatE, g_cycle;
float g_bass, g_mid, g_high, g_beat, g_lev;
vec3  g_ro, g_uu, g_vv, g_ww, g_back, g_sun, g_sunC, g_bc;
float g_br, g_floorY, g_sunI, g_ring, g_camAz;
vec2  g_fc;
int   g_rx, g_na, g_nb;

vec3  g_pos[MAXA]; float g_rad[MAXA]; vec3 g_col[MAXA]; float g_asc[MAXA];
vec3  g_bA[2 * MAXB]; vec3 g_bB[2 * MAXB]; vec3 g_bN[2 * MAXB];
float g_bR[2 * MAXB]; float g_bO[2 * MAXB]; vec3 g_bCa[2 * MAXB]; vec3 g_bCb[2 * MAXB];
vec3  g_fC[MAXF]; mat3 g_fR[MAXF];

// hit info from the last sdf evaluation
vec3 s_col; int s_mat; float s_bh;

// ── element look ─────────────────────────────────────────────────────────
vec3 elemCol(int e) {
    vec3 c = vec3(0.95, 0.95, 0.97);                  // H
    if (e == 1) c = vec3(0.17, 0.17, 0.19);           // C
    if (e == 2) c = vec3(0.98, 0.12, 0.07);           // O
    if (e == 3) c = vec3(0.14, 0.32, 0.98);           // N
    if (candy > 0.001) {
        vec3 k = vec3(0.98, 0.98, 1.0);
        if (e == 1) k = vec3(0.12, 0.10, 0.16);
        if (e == 2) k = vec3(1.0, 0.25, 0.55);
        if (e == 3) k = vec3(0.2, 0.9, 1.0);
        if (e == 0) k = vec3(1.0, 0.92, 0.35);
        c = mix(c, k, candy);
    }
    return c;
}
float elemRad(int e) {
    float r = 0.30;                                    // H
    if (e == 1) r = 0.46;
    if (e == 2) r = 0.43;
    if (e == 3) r = 0.44;
    return r;
}

// ── reaction data ────────────────────────────────────────────────────────
// atom k of reaction rx: element, reactant frame + local pos, product frame + local pos
// frames 0..3 = reactant molecules, 4..7 = product molecules
void atomDef(int rx, int k, out int e, out int fr, out vec3 pr, out int fp, out vec3 pp) {
    e = 0; fr = 0; pr = vec3(0.0); fp = 4; pp = vec3(0.0);
    vec3 wA = vec3(0.759, -0.47, 0.0), wB = vec3(-0.759, -0.47, 0.0), wO = vec3(0.0, 0.12, 0.0);
    if (rx == 0) {           // 2 H2 + O2 → 2 H2O
        if (k == 0) { e = 0; fr = 0; pr = vec3(-0.37, 0.0, 0.0); fp = 4; pp = wA; }
        if (k == 1) { e = 0; fr = 0; pr = vec3( 0.37, 0.0, 0.0); fp = 5; pp = wA; }
        if (k == 2) { e = 0; fr = 1; pr = vec3(-0.37, 0.0, 0.0); fp = 4; pp = wB; }
        if (k == 3) { e = 0; fr = 1; pr = vec3( 0.37, 0.0, 0.0); fp = 5; pp = wB; }
        if (k == 4) { e = 2; fr = 2; pr = vec3(-0.60, 0.0, 0.0); fp = 4; pp = wO; }
        if (k == 5) { e = 2; fr = 2; pr = vec3( 0.60, 0.0, 0.0); fp = 5; pp = wO; }
    } else if (rx == 1) {    // CH4 + 2 O2 → CO2 + 2 H2O
        float h = 0.629;
        if (k == 0) { e = 1; fr = 0; pr = vec3(0.0); fp = 4; pp = vec3(0.0); }
        if (k == 1) { e = 0; fr = 0; pr = vec3( h,  h,  h); fp = 5; pp = wA; }
        if (k == 2) { e = 0; fr = 0; pr = vec3( h, -h, -h); fp = 5; pp = wB; }
        if (k == 3) { e = 0; fr = 0; pr = vec3(-h,  h, -h); fp = 6; pp = wA; }
        if (k == 4) { e = 0; fr = 0; pr = vec3(-h, -h,  h); fp = 6; pp = wB; }
        if (k == 5) { e = 2; fr = 1; pr = vec3(-0.60, 0.0, 0.0); fp = 4; pp = vec3(-1.16, 0.0, 0.0); }
        if (k == 6) { e = 2; fr = 1; pr = vec3( 0.60, 0.0, 0.0); fp = 5; pp = wO; }
        if (k == 7) { e = 2; fr = 2; pr = vec3(-0.60, 0.0, 0.0); fp = 4; pp = vec3( 1.16, 0.0, 0.0); }
        if (k == 8) { e = 2; fr = 2; pr = vec3( 0.60, 0.0, 0.0); fp = 6; pp = wO; }
    } else if (rx == 2) {    // N2 + 3 H2 → 2 NH3
        vec3 n0 = vec3(0.936, -0.278, 0.0), n1 = vec3(-0.468, -0.278, 0.811), n2 = vec3(-0.468, -0.278, -0.811), nN = vec3(0.0, 0.1, 0.0);
        if (k == 0) { e = 3; fr = 0; pr = vec3(-0.55, 0.0, 0.0); fp = 4; pp = nN; }
        if (k == 1) { e = 3; fr = 0; pr = vec3( 0.55, 0.0, 0.0); fp = 5; pp = nN; }
        if (k == 2) { e = 0; fr = 1; pr = vec3(-0.37, 0.0, 0.0); fp = 4; pp = n0; }
        if (k == 3) { e = 0; fr = 1; pr = vec3( 0.37, 0.0, 0.0); fp = 4; pp = n1; }
        if (k == 4) { e = 0; fr = 2; pr = vec3(-0.37, 0.0, 0.0); fp = 4; pp = n2; }
        if (k == 5) { e = 0; fr = 2; pr = vec3( 0.37, 0.0, 0.0); fp = 5; pp = n0; }
        if (k == 6) { e = 0; fr = 3; pr = vec3(-0.37, 0.0, 0.0); fp = 5; pp = n1; }
        if (k == 7) { e = 0; fr = 3; pr = vec3( 0.37, 0.0, 0.0); fp = 5; pp = n2; }
    } else if (rx == 3) {    // C2H4 + H2 → C2H6
        if (k == 0) { e = 1; fr = 0; pr = vec3(-0.67, 0.0, 0.0); fp = 4; pp = vec3(-0.77, 0.0, 0.0); }
        if (k == 1) { e = 1; fr = 0; pr = vec3( 0.67, 0.0, 0.0); fp = 4; pp = vec3( 0.77, 0.0, 0.0); }
        if (k == 2) { e = 0; fr = 0; pr = vec3(-1.22,  0.94, 0.0); fp = 4; pp = vec3(-1.13,  1.03, 0.0); }
        if (k == 3) { e = 0; fr = 0; pr = vec3(-1.22, -0.94, 0.0); fp = 4; pp = vec3(-1.13, -0.515, 0.892); }
        if (k == 4) { e = 0; fr = 0; pr = vec3( 1.22,  0.94, 0.0); fp = 4; pp = vec3( 1.13,  0.515, 0.892); }
        if (k == 5) { e = 0; fr = 0; pr = vec3( 1.22, -0.94, 0.0); fp = 4; pp = vec3( 1.13, -1.03, 0.0); }
        if (k == 6) { e = 0; fr = 1; pr = vec3(-0.37, 0.0, 0.0); fp = 4; pp = vec3(-1.13, -0.515, -0.892); }
        if (k == 7) { e = 0; fr = 1; pr = vec3( 0.37, 0.0, 0.0); fp = 4; pp = vec3( 1.13,  0.515, -0.892); }
    } else if (rx == 4) {    // 2 CO + O2 → 2 CO2
        if (k == 0) { e = 1; fr = 0; pr = vec3(-0.56, 0.0, 0.0); fp = 4; pp = vec3(0.0); }
        if (k == 1) { e = 2; fr = 0; pr = vec3( 0.56, 0.0, 0.0); fp = 4; pp = vec3(-1.16, 0.0, 0.0); }
        if (k == 2) { e = 1; fr = 1; pr = vec3(-0.56, 0.0, 0.0); fp = 5; pp = vec3(0.0); }
        if (k == 3) { e = 2; fr = 1; pr = vec3( 0.56, 0.0, 0.0); fp = 5; pp = vec3(-1.16, 0.0, 0.0); }
        if (k == 4) { e = 2; fr = 2; pr = vec3(-0.60, 0.0, 0.0); fp = 4; pp = vec3( 1.16, 0.0, 0.0); }
        if (k == 5) { e = 2; fr = 2; pr = vec3( 0.60, 0.0, 0.0); fp = 5; pp = vec3( 1.16, 0.0, 0.0); }
    } else {                 // benzene: Kekulé resonance flip
        float a = float(k) * TAU / 6.0;
        if (k < 6) { e = 1; pr = vec3(cos(a), sin(a), 0.0) * 1.12; }
        else       { e = 0; pr = vec3(cos(a), sin(a), 0.0) * 1.98; }
        fr = 0; fp = 4; pp = pr;
    }
}
// bond b of phase ph (0 reactant, 1 product) → atom indices + order; ia = -1 = none
void bondDef(int rx, int ph, int b, out int ia, out int ib, out float od) {
    ia = -1; ib = -1; od = 1.0;
    if (rx == 0) {
        if (ph == 0) {
            if (b == 0) { ia = 0; ib = 1; } if (b == 1) { ia = 2; ib = 3; } if (b == 2) { ia = 4; ib = 5; od = 2.0; }
        } else {
            if (b == 0) { ia = 4; ib = 0; } if (b == 1) { ia = 4; ib = 2; } if (b == 2) { ia = 5; ib = 1; } if (b == 3) { ia = 5; ib = 3; }
        }
    } else if (rx == 1) {
        if (ph == 0) {
            if (b == 0) { ia = 0; ib = 1; } if (b == 1) { ia = 0; ib = 2; } if (b == 2) { ia = 0; ib = 3; } if (b == 3) { ia = 0; ib = 4; }
            if (b == 4) { ia = 5; ib = 6; od = 2.0; } if (b == 5) { ia = 7; ib = 8; od = 2.0; }
        } else {
            if (b == 0) { ia = 0; ib = 5; od = 2.0; } if (b == 1) { ia = 0; ib = 7; od = 2.0; }
            if (b == 2) { ia = 6; ib = 1; } if (b == 3) { ia = 6; ib = 2; } if (b == 4) { ia = 8; ib = 3; } if (b == 5) { ia = 8; ib = 4; }
        }
    } else if (rx == 2) {
        if (ph == 0) {
            if (b == 0) { ia = 0; ib = 1; od = 3.0; } if (b == 1) { ia = 2; ib = 3; } if (b == 2) { ia = 4; ib = 5; } if (b == 3) { ia = 6; ib = 7; }
        } else {
            if (b == 0) { ia = 0; ib = 2; } if (b == 1) { ia = 0; ib = 3; } if (b == 2) { ia = 0; ib = 4; }
            if (b == 3) { ia = 1; ib = 5; } if (b == 4) { ia = 1; ib = 6; } if (b == 5) { ia = 1; ib = 7; }
        }
    } else if (rx == 3) {
        if (ph == 0) {
            if (b == 0) { ia = 0; ib = 1; od = 2.0; } if (b == 1) { ia = 0; ib = 2; } if (b == 2) { ia = 0; ib = 3; }
            if (b == 3) { ia = 1; ib = 4; } if (b == 4) { ia = 1; ib = 5; } if (b == 5) { ia = 6; ib = 7; }
        } else {
            if (b == 0) { ia = 0; ib = 1; } if (b == 1) { ia = 0; ib = 2; } if (b == 2) { ia = 0; ib = 3; } if (b == 3) { ia = 0; ib = 6; }
            if (b == 4) { ia = 1; ib = 4; } if (b == 5) { ia = 1; ib = 5; } if (b == 6) { ia = 1; ib = 7; }
        }
    } else if (rx == 4) {
        if (ph == 0) {
            if (b == 0) { ia = 0; ib = 1; od = 3.0; } if (b == 1) { ia = 2; ib = 3; od = 3.0; } if (b == 2) { ia = 4; ib = 5; od = 2.0; }
        } else {
            if (b == 0) { ia = 0; ib = 1; od = 2.0; } if (b == 1) { ia = 0; ib = 4; od = 2.0; }
            if (b == 2) { ia = 2; ib = 3; od = 2.0; } if (b == 3) { ia = 2; ib = 5; od = 2.0; }
        }
    } else {
        if (b < 6) {
            ia = b; ib = b + 1; if (b == 5) ib = 0;
            float even = (mod(float(b), 2.0) < 0.5) ? 1.0 : 0.0;
            od = (ph == 0) ? 1.0 + even : 2.0 - even;
        } else if (b < 12) { ia = b - 6; ib = b; }
    }
}
void reactionInfo(int rx, out int na, out int nb, out int nr, out int np) {
    na = 6; nb = 4; nr = 3; np = 2;
    if (rx == 1) { na = 9; nb = 6; nr = 3; np = 3; }
    if (rx == 2) { na = 8; nb = 6; nr = 4; np = 2; }
    if (rx == 3) { na = 8; nb = 7; nr = 2; np = 1; }
    if (rx == 4) { na = 6; nb = 4; nr = 3; np = 2; }
    if (rx == 5) { na = 12; nb = 12; nr = 1; np = 1; }
}

// ── timeline + frames ────────────────────────────────────────────────────
void setupScene() {
    float T = 16.0 / max(speed, 0.05);
    float tt = g_t * 1.0;
    g_cycle = floor(tt / T);
    g_u = fract(tt / T);
    float rsel = autoCycle ? mod(molecule + g_cycle, 6.0) : molecule;
    g_rx = int(floor(rsel + 0.5));
    if (g_rx < 0) g_rx = 0; if (g_rx > 5) g_rx = 5;
    int nr, np;
    reactionInfo(g_rx, g_na, g_nb, nr, np);
    float u = g_u;
    float sIn = smoothstep(0.45, 0.55, u);
    g_s = sIn * sIn * (3.0 - 2.0 * sIn);
    g_flash = flash * exp(-pow((u - 0.5) / 0.03, 2.0));
    g_ring = flash * smoothstep(0.0, 0.01, u - 0.5) * exp(-(u - 0.5) * 22.0);
    g_beatE = g_ar * g_beat;
    float seed = g_cycle * 1.7 + float(g_rx) * 0.37;
    float near = (1.0 + 0.22 * float(nr)) * spread;
    // reactant frames: come in from the fog toward the origin
    for (int f = 0; f < MAXF; f++) {
        float fi = float(f);
        if (f < 4) {
            float a = g_camAz + PI + ((fi + 0.5) / float(nr) - 0.5) * 2.2 + 0.5 * (hash11(seed + fi * 3.3) - 0.5);
            float el = 0.22 * (hash11(seed + fi * 7.1 + 2.0) - 0.5);
            vec3 dir = normalize(vec3(sin(a), el, cos(a)));   // camera sits at azimuth g_camAz
            float su = clamp(u / 0.45, 0.0, 1.0);
            float d = 4.8 - (4.8 - near) * pow(su, 0.8);
            if (u > 0.45) d = near - 0.5 * (u - 0.45);
            g_fC[f] = dir * d + vec3(0.0, 0.25 * sin(g_t * 0.7 + fi * 2.0), 0.0);
            float rate = tumble * (0.35 + 0.35 * hash11(seed + fi * 5.9 + 9.0));
            float ph0 = hash11(seed + fi * 2.2 + 4.0) * TAU;
            g_fR[f] = rotY(g_t * rate + ph0) * rotX(g_t * rate * 0.63 + ph0 * 1.7) * rotZ(ph0 * 0.5 + g_t * rate * 0.21);
        } else {
            float pj = fi - 4.0;
            float a = g_camAz + PI + ((pj + 0.5) / float(np) - 0.5) * 2.3 * ((mod(g_cycle, 2.0) < 0.5) ? 1.0 : -1.0) + 0.5 * (hash11(seed + fi * 1.9) - 0.5);
            float el = 0.3 * (hash11(seed + fi * 6.3 + 1.0) - 0.3);
            vec3 dir = normalize(vec3(sin(a), el, cos(a)));
            float so = smoothstep(0.5, 1.0, u);
            float d = 0.85 * spread + 4.2 * pow(so, 1.4);
            g_fC[f] = dir * d + vec3(0.0, 0.2 * sin(g_t * 0.6 + fi * 1.3), 0.0);
            float rate = tumble * (0.3 + 0.3 * hash11(seed + fi * 4.4 + 8.0));
            float ph0 = hash11(seed + fi * 3.7 + 6.0) * TAU;
            g_fR[f] = rotY(g_t * rate + ph0) * rotX(g_t * rate * 0.57 + ph0 * 1.3) * rotZ(ph0 * 0.8 + g_t * rate * 0.17);
        }
    }
    // atoms
    float scR = smoothstep(0.0, 0.05, u);
    float scP = 1.0 - smoothstep(0.93, 0.995, u);
    float vibA = vibration * (0.012 + 0.07 * smoothstep(0.2, 0.46, u) * (1.0 - smoothstep(0.5, 0.56, u)) + 0.045 * g_ar * g_bass);
    float vibP = vibration * (0.01 + 0.03 * g_ar * g_bass);
    vec3 csum = vec3(0.0); float wsum = 0.0;
    for (int k = 0; k < MAXA; k++) {
        if (k >= g_na) { g_asc[k] = 0.0; g_rad[k] = 0.0; g_pos[k] = vec3(0.0, -50.0, 0.0); g_col[k] = vec3(0.0); continue; }
        int e, fr, fp; vec3 pr, pp;
        atomDef(g_rx, k, e, fr, pr, fp, pp);
        float fk = float(k);
        float vr = 1.0 + vibA * sin(g_t * 19.0 + fk * 2.1) + vibA * 0.5 * sin(g_t * 31.0 + fk * 5.3);
        float vp = 1.0 + vibP * sin(g_t * 17.0 + fk * 1.7);
        vec3 wr = vec3(0.0), wp = vec3(0.0);
        for (int f = 0; f < MAXF; f++) {
            if (f == fr) wr = g_fC[f] + g_fR[f] * (pr * vr * scR);
            if (f == fp) wp = g_fC[f] + g_fR[f] * (pp * vp * scP);
        }
        // a little arc on the way over so atoms don't slide in straight lines
        vec3 arc = vec3(0.0, 0.5, 0.0) * sin(g_s * PI) * hash11(fk * 9.1 + seed);
        g_pos[k] = mix(wr, wp, g_s) + arc;
        float sc = mix(scR, scP, g_s);
        g_asc[k] = sc;
        g_rad[k] = elemRad(e) * atomSize * sc * (1.0 + 0.25 * g_flash);
        g_col[k] = elemCol(e);
        float w = sc * sc;
        csum += g_pos[k] * w; wsum += w;
    }
    g_bc = csum / max(wsum, 1e-4);
    float rr = 0.5;
    for (int k = 0; k < MAXA; k++) {
        if (k >= g_na) break;
        rr = max(rr, (length(g_pos[k] - g_bc) + g_rad[k] + 0.25) * step(0.02, g_asc[k]));
    }
    g_br = rr + 0.2;
    // bonds: reactant list then product list
    float envR = (1.0 - smoothstep(0.40, 0.49, u)) * scR;
    float envP = smoothstep(0.51, 0.60, u) * scP;
    float br = 0.105 * bondSize;
    for (int b = 0; b < 2 * MAXB; b++) {
        int ph = (b < MAXB) ? 0 : 1;
        int bi = (b < MAXB) ? b : b - MAXB;
        int ia, ib; float od;
        bondDef(g_rx, ph, bi, ia, ib, od);
        g_bR[b] = 0.0; g_bO[b] = 1.0; g_bA[b] = vec3(0.0); g_bB[b] = vec3(0.0, 1.0, 0.0); g_bN[b] = vec3(0.0, 0.0, 1.0);
        g_bCa[b] = vec3(0.5); g_bCb[b] = vec3(0.5);
        if (ia < 0) continue;
        vec3 A = vec3(0.0), B = vec3(0.0), ca = vec3(0.0), cb = vec3(0.0); int fa = 0;
        for (int i = 0; i < MAXA; i++) {
            if (i == ia) { A = g_pos[i]; ca = g_col[i]; }
            if (i == ib) { B = g_pos[i]; cb = g_col[i]; }
        }
        int e, fr, fp; vec3 pr, pp;
        atomDef(g_rx, ia, e, fr, pr, fp, pp);
        fa = (ph == 0) ? fr : fp;
        vec3 up = vec3(0.0, 0.0, 1.0);
        for (int f = 0; f < MAXF; f++) { if (f == fa) up = g_fR[f] * vec3(0.0, 0.0, 1.0); }
        vec3 ax = normalize(B - A + 1e-5);
        vec3 nn = cross(ax, up);
        if (dot(nn, nn) < 1e-4) nn = cross(ax, vec3(0.0, 1.0, 0.0));
        g_bN[b] = normalize(nn);
        g_bA[b] = A; g_bB[b] = B; g_bO[b] = od;
        g_bR[b] = br * ((ph == 0) ? envR : envP) * (od > 1.5 ? 0.78 : 1.0);
        g_bCa[b] = ca; g_bCb[b] = cb;
    }
}

void setupCam() {
    float oa = g_t * 0.08 * orbitSpeed + 0.6;
    g_camAz = oa;
    float dist = 10.5 / zoom;
    g_ro = vec3(sin(oa) * dist, 0.6 + camHeight * dist * 0.55 + 0.25 * sin(g_t * 0.1), cos(oa) * dist);
    vec3 ta = vec3(0.0, -0.25, 0.0);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    g_lens = 1.9;
    float az = radians(lightAz), el = radians(lightEl);
    g_sun = normalize(vec3(cos(el) * sin(az), sin(el), cos(el) * cos(az)));
    g_sunC = vec3(1.0, 0.96, 0.9);
    g_sunI = keyLight * 1.7 * (1.0 + 0.25 * g_ar * g_lev);
    g_floorY = -2.1;
}
vec3 camRay(vec2 uv) { return normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_lens); }
vec3 project(vec3 p) {
    vec3 d = p - g_ro;
    float z = dot(d, g_ww);
    vec2 s = vec2(dot(d, g_uu), dot(d, g_vv)) / max(z, 0.05) * g_lens;
    return vec3(s, z);
}

// studio backdrop: graded dark cyclorama + a soft pool of light behind the
// molecules + three panel lights that show up in the reflections
vec3 env(vec3 d, bool panels) {
    float up = d.y * 0.5 + 0.5;
    vec3 c = g_back * mix(0.4, 1.3, smoothstep(0.0, 0.8, up));
    float pool = exp(-pow(length(d.xy - vec2(0.0, 0.12)) * 2.2, 2.0));
    c += (g_back * 3.0 + vec3(0.07, 0.08, 0.11)) * pool * 0.8 * fill;
    if (panels) {
        float key = 1.0 + 0.4 * g_ar * g_beat;
        c += vec3(1.0, 0.97, 0.92) * smoothstep(0.86, 0.985, dot(d, g_sun)) * 2.2 * key * keyLight;
        c += vec3(0.85, 0.92, 1.0) * smoothstep(0.90, 0.99, dot(d, normalize(vec3(-0.6, 0.35, 0.6)))) * 1.2 * fill;
        c += vec3(1.0, 0.85, 0.75) * smoothstep(0.92, 0.995, dot(d, normalize(vec3(0.5, 0.2, -0.75)))) * 0.9 * fill;
    }
    return c;
}

// ── SDF ──────────────────────────────────────────────────────────────────
float sdCap(vec3 p, vec3 a, vec3 b, float r, out float h) {
    vec3 pa = p - a, ba = b - a;
    h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}
float sceneSDF(vec3 p) {
    float d = 1e9;
    s_col = vec3(0.5); s_mat = 0; s_bh = 0.0;
    for (int i = 0; i < MAXA; i++) {
        if (i >= g_na) break;
        float di = length(p - g_pos[i]) - g_rad[i];
        if (di < d) { d = di; s_col = g_col[i]; s_mat = 0; }
    }
    for (int b = 0; b < 2 * MAXB; b++) {
        float r = g_bR[b];
        if (r < 0.003) continue;
        float od = g_bO[b];
        float db = 1e9, hb = 0.0;
        for (int k = 0; k < 3; k++) {
            if (float(k) >= od - 0.5) break;
            float off = (float(k) - (od - 1.0) * 0.5) * 0.175 * bondSize;
            float h;
            float dk = sdCap(p - g_bN[b] * off, g_bA[b], g_bB[b], r, h);
            if (dk < db) { db = dk; hb = h; }
        }
        if (db < d) { d = db; s_mat = 1; s_bh = hb; s_col = (hb < 0.5) ? g_bCa[b] : g_bCb[b]; }
    }
    return d;
}
float sceneDist(vec3 p) { return sceneSDF(p); }
vec3 sceneNormal(vec3 p) {
    vec2 e = vec2(0.0012, -0.0012);
    return normalize(e.xyy * sceneDist(p + e.xyy) + e.yyx * sceneDist(p + e.yyx) +
                     e.yxy * sceneDist(p + e.yxy) + e.xxx * sceneDist(p + e.xxx));
}
bool boundHit(vec3 ro, vec3 rd, out float t0, out float t1) {
    vec3 oc = ro - g_bc;
    float b = dot(oc, rd);
    float c = dot(oc, oc) - g_br * g_br;
    float h = b * b - c;
    if (h < 0.0) return false;
    h = sqrt(h);
    t0 = max(-b - h, 0.0); t1 = -b + h;
    return t1 > t0;
}
// march; accumulates electron-cloud glow along the way. returns t or -1
float marchScene(vec3 ro, vec3 rd, float t0, float t1, int steps, inout vec3 glow) {
    float t = t0;
    for (int i = 0; i < 90; i++) {
        if (i >= steps) break;
        vec3 p = ro + rd * t;
        float d = sceneSDF(p);
        if (d < 0.0018 * (1.0 + t * 0.08)) return t;
        float st = max(d, 0.004);
        if (cloud > 0.001 && s_mat == 0) {
            glow += s_col * exp(-max(d, 0.0) * 9.0) * min(st, 0.35) * 0.55;
        }
        t += st;
        if (t > t1) break;
    }
    return -1.0;
}
// iq penumbra soft shadow, clipped to the bounding sphere
float softShadow(vec3 ro, vec3 rd, float k, int steps) {
    float t0, t1;
    if (!boundHit(ro, rd, t0, t1)) return 1.0;
    float res = 1.0, t = max(t0, 0.015), ph = 1e10;
    for (int i = 0; i < 24; i++) {
        if (i >= steps) break;
        float d = sceneSDF(ro + rd * t);
        float y = d * d / (2.0 * ph);
        float dd = sqrt(max(d * d - y * y, 0.0));
        res = min(res, k * dd / max(t - y, 0.001));
        ph = d;
        if (res < 0.004) return 0.0;
        t += clamp(d, 0.02, 0.3);
        if (t > t1) break;
    }
    return clamp(res, 0.0, 1.0);
}
float aoScene(vec3 p, vec3 n, float scale) {
    if (length(p - g_bc) > g_br + 0.8 * scale) return 1.0;
    float occ = 0.0, sca = 1.0;
    for (int i = 0; i < 5; i++) {
        float h = (0.03 + 0.12 * float(i)) * scale;
        float d = sceneSDF(p + n * h);
        occ += (h - d) * sca;
        sca *= 0.7;
    }
    return clamp(1.0 - 1.6 * occ / scale, 0.0, 1.0);
}

// ── shading ──────────────────────────────────────────────────────────────
vec3 shadeSurf(vec3 p, vec3 rd, vec3 n, vec3 base, int mat, float bh, bool simple) {
    float sh = simple ? 1.0 : softShadow(p + n * 0.02, g_sun, 18.0, 20);
    float ao = simple ? 1.0 : aoScene(p, n, 0.6);
    float dif = max(dot(n, g_sun), 0.0);
    float cosv = max(dot(n, -rd), 0.0);
    float F = 0.04 + 0.96 * pow(1.0 - cosv, 5.0);
    // plastic: glossy coat over a coloured body
    vec3 hv = normalize(g_sun - rd);
    float spec = pow(max(dot(n, hv), 0.0), 72.0) * (1.0 + 1.6 * g_ar * g_high * glint);
    float spec2 = pow(max(dot(n, hv), 0.0), 14.0) * 0.12;
    vec3 sky = env(n, false) * 2.2 * fill + vec3(0.03, 0.035, 0.05) * fill;
    vec3 rfl = env(reflect(rd, n), true);
    vec3 col = base * (sky * ao * 0.9 + g_sunC * g_sunI * dif * sh * 1.25);
    col += g_sunC * g_sunI * (spec * 1.6 + spec2) * sh * glint;
    col += rfl * F * (0.6 + 0.4 * ao);
    // rim from the fill side + a cool back light for separation from the dark
    col += base * pow(1.0 - cosv, 3.0) * 0.25 * fill * ao;
    float rimL = pow(1.0 - cosv, 3.5) * max(dot(n, normalize(-g_ww + vec3(0.0, 0.5, 0.0)) * -1.0), 0.0);
    col += vec3(0.55, 0.75, 1.0) * rimL * 0.9 * fill * ao;
    // coloured rims: cool cyan from the left/back, warm amber from the right
    vec3 rimA = normalize(-g_uu * 0.8 + g_ww * 0.5 + vec3(0.0, 0.35, 0.0));
    vec3 rimB = normalize(g_uu * 0.8 + g_ww * 0.3 + vec3(0.0, 0.15, 0.0));
    float eR = pow(1.0 - cosv, 2.6);
    col += vec3(0.25, 0.85, 1.0) * eR * max(dot(n, rimA), 0.0) * 0.9 * rimLight * ao;
    col += vec3(1.0, 0.45, 0.2) * eR * max(dot(n, rimB), 0.0) * 0.7 * rimLight * ao;
    // bonds: a pale highlight band along the rod + beat flash
    if (mat == 1) {
        col = mix(col, col * 1.15 + vec3(0.06), 0.3);
        float seam = smoothstep(0.03, 0.0, abs(bh - 0.5)) * 0.5;
        col += vec3(1.0) * seam * 0.15;
        col += vec3(1.0, 0.98, 0.9) * g_beatE * 1.6;
    }
    // reaction flash: the atoms glow from inside
    col += mix(base, vec3(1.0), 0.6) * g_flash * 0.9 * (0.6 + 0.4 * cosv);
    // flash as a point light at the cluster centre
    vec3 lv = g_bc - p; float ld = length(lv); lv /= max(ld, 1e-3);
    col += mix(base, vec3(1.0), 0.5) * max(dot(n, lv), 0.0) * g_flash * 1.4 / (1.0 + ld * ld);
    return col;
}
vec3 shadeFloor(vec3 p, vec3 rd) {
    vec3 n = vec3(0.0, 1.0, 0.0);
    vec3 fc = floorColor.rgb;
    float dif = max(dot(n, g_sun), 0.0);
    float sh = softShadow(p + n * 0.02, g_sun, 12.0, 22);
    float ao = aoScene(p, n, 2.0);
    vec3 sky = env(n, false) * 1.8 * fill;
    float pool = exp(-dot(p.xz, p.xz) * 0.035);
    vec3 col = fc * (sky * ao * (0.5 + 0.8 * pool) + g_sunC * g_sunI * dif * sh * (0.7 + 0.5 * pool));
    // flash light pooling on the floor
    vec3 lv = g_bc - p; float ld = length(lv); lv /= max(ld, 1e-3);
    col += fc * vec3(1.0, 0.95, 0.9) * max(dot(n, lv), 0.0) * g_flash * 2.5 / (1.0 + ld * ld * 0.5);
    // shockwave ring racing out across the floor after the flash
    float rr0 = length(p.xz - g_bc.xz);
    float ringR = (g_u - 0.5) * 34.0;
    col += vec3(0.8, 0.9, 1.0) * g_ring * exp(-pow((rr0 - ringR) / 0.35, 2.0)) * 1.2 * floorReflect;
    // sun sheen
    vec3 rr = reflect(rd, n);
    col += g_sunC * pow(max(dot(rr, g_sun), 0.0), 40.0) * 0.25 * g_sunI * floorReflect;
    // mirror the molecules
    if (floorReflect > 0.001) {
        vec3 refl = env(rr, true) * 0.5;
        float t0, t1; vec3 g2 = vec3(0.0);
        if (boundHit(p + n * 0.01, rr, t0, t1)) {
            float t2 = marchScene(p + n * 0.01, rr, t0, t1, 36, g2);
            if (t2 > 0.0) {
                vec3 p2 = p + n * 0.01 + rr * t2;
                vec3 n2 = sceneNormal(p2);
                vec3 c2 = s_col; int m2 = s_mat; float h2 = s_bh;
                refl = shadeSurf(p2, rr, n2, c2, m2, h2, true) * exp(-t2 * 0.08);
            }
        }
        refl += g2 * cloud * 0.6;
        float fr = pow(1.0 - max(dot(n, -rd), 0.0), 2.5);
        col = mix(col, refl, floorReflect * (0.45 + 0.55 * fr));
    }
    // fine brushed texture so the floor is not a flat slab
    col *= 0.95 + 0.05 * vnoise2(p.xz * 7.0) + 0.03 * vnoise2(vec2(p.x * 60.0, p.z * 3.0));
    return col;
}

vec4 renderScene(vec3 ro, vec3 rd) {
    vec3 glow = vec3(0.0);
    float tF = 1e9;
    if (floorOn > 0.001 && rd.y < -0.001) tF = (g_floorY - ro.y) / rd.y;
    float t0, t1, t = -1.0;
    if (boundHit(ro, rd, t0, t1)) t = marchScene(ro, rd, t0, min(t1, tF), 90, glow);
    vec3 col; float depth;
    if (t > 0.0) {
        vec3 p = ro + rd * t;
        vec3 n = sceneNormal(p);
        sceneSDF(p);
        col = shadeSurf(p, rd, n, s_col, s_mat, s_bh, false);
        col = mix(col, env(rd, false), 1.0 - exp(-t * 0.02 * fog));
        depth = t;
    } else if (tF < 40.0) {
        vec3 p = ro + rd * tF;
        vec3 fl = shadeFloor(p, rd);
        float ff = 1.0 - exp(-tF * 0.05 * fog);
        fl = mix(fl, env(rd, false), ff);
        col = mix(env(rd, false), fl, floorOn);
        depth = tF;
    } else {
        col = env(rd, false);
        depth = SKY;
    }
    // flash bloom core at the cluster centre
    vec3 oc = g_bc - ro; float tc = max(dot(oc, rd), 0.0);
    float dc = length(oc - rd * tc);
    if (tc < depth) col += vec3(1.0, 0.97, 0.9) * g_flash * 0.8 * exp(-dc * dc * 2.0);
    col += glow * cloud * (1.0 + 0.6 * g_ar * g_mid);
    return vec4(col, clamp(depth / SKY, 0.0, 1.0));
}

// ── pass 1: volumetric key-light haze (half res) ─────────────────────────
// a beam down the key light through the molecules, shadowed by them with a
// short sun-ward march per sample; the final pass blurs this buffer
vec3 hazePass(vec2 uv, vec2 st) {
    float depth = texture2D(moScene, st).a * SKY;
    vec3 rd = camRay(uv);
    vec3 ro = g_ro;
    float tEnd = min(depth, 28.0);
    float j = hash12(g_fc + fract(g_t * 7.0) * 13.0);
    float tS = 0.3;
    float dtS = (tEnd - tS) / 12.0;
    float acc = 0.0;
    for (int s = 0; s < 12; s++) {
        float tt = tS + (float(s) + j) * dtS;
        vec3 p = ro + rd * tt;
        vec3 rel = p - vec3(0.0, -0.3, 0.0);
        float along = dot(rel, g_sun);
        vec3 perp = rel - g_sun * along;
        float pr2 = dot(perp, perp);
        float cone = exp(-pr2 / (2.0 + 0.8 * max(along, 0.0)));
        float dens = cone * (0.65 + 0.35 * exp(-max(p.y - g_floorY, 0.0) * 0.4));
        if (dens < 0.004) continue;
        float vis = 1.0, b0, b1;
        if (boundHit(p, g_sun, b0, b1)) {
            float tq = max(b0, 0.05);
            for (int k = 0; k < 3; k++) {
                float dq = sceneSDF(p + g_sun * tq);
                vis = min(vis, smoothstep(-0.05, 0.3, dq));
                tq += max(dq, 0.35);
                if (tq > b1) break;
            }
        }
        acc += dens * vis * dtS;
    }
    return g_sunC * acc * haze * 0.03 * g_sunI * (1.0 + 0.5 * g_flash);
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    g_ar = audioReact;
    g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_back = backColor.rgb * backAmt;
    setupCam();

    g_fc = fragCoord;
    if (PASSINDEX == 0) {
        setupScene();
        vec3 rd = camRay(uv);
        vec4 sc = renderScene(g_ro, rd);
        gl_FragColor = vec4(max(sc.rgb, 0.0), sc.a);
    } else if (PASSINDEX == 1) {
        setupScene();
        vec2 st = fragCoord / R;
        gl_FragColor = vec4(max(hazePass(uv, st), 0.0), 1.0);
    } else {
        vec2 st = fragCoord / R;
        vec2 asp = vec2(R.y / R.x, 1.0);
        vec2 dir = (st - 0.5) * 2.0;
        float ab = aberration * 0.0028;
        vec2 o = dir * ab;
        vec4 c0 = texture2D(moScene, st);
        float depth = c0.a * SKY;
        float isSky = step(0.995, c0.a);
        vec3 col = vec3(texture2D(moScene, st + o).r, c0.g, texture2D(moScene, st - o).b);
        // haze: 12-tap disc blur — the beam is low frequency, the march noise is not
        vec3 hz = texture2D(moHaze, st).rgb;
        for (int i = 0; i < 12; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + 0.7;
            float rr = sqrt((fi + 0.5) / 12.0) * 0.006;
            hz += texture2D(moHaze, st + vec2(cos(a), sin(a)) * rr * asp).rgb;
        }
        col += hz / 13.0;
        // fine dust drifting through the key light (occluded by the scene)
        if (dust > 0.001) {
            vec3 dc = vec3(0.0);
            for (int i = 0; i < 40; i++) {
                float fi = float(i);
                float h1 = hash11(fi * 3.1 + 1.0), h2 = hash11(fi * 7.3 + 2.0), h3 = hash11(fi * 5.7 + 3.0), h4 = hash11(fi * 2.9 + 4.0);
                vec3 pd = vec3((h1 - 0.5) * 9.0, -1.8 + 4.2 * fract(h2 + g_t * 0.012 * (0.5 + h4)), (h3 - 0.5) * 9.0);
                pd.x += 0.6 * sin(g_t * 0.21 + fi); pd.z += 0.6 * cos(g_t * 0.17 + fi * 1.3);
                vec3 pr = project(pd);
                if (pr.z < 0.5 || pr.z > depth) continue;
                vec2 d = uv - pr.xy;
                float rad = 0.0035 * (1.0 + 0.5 * h4) * 9.0 / pr.z;
                float dd = dot(d, d);
                if (dd > rad * rad * 9.0) continue;
                vec3 rel = pd - vec3(0.0, -0.3, 0.0);
                float along = dot(rel, g_sun);
                float cone = exp(-dot(rel - g_sun * along, rel - g_sun * along) / 3.0);
                float tw = 0.4 + 0.6 * pow(0.5 + 0.5 * sin(g_t * (0.7 + 1.5 * h3) + h1 * 6.28), 2.0) * (1.0 + 1.0 * g_ar * g_high);
                dc += g_sunC * exp(-dd / (rad * rad)) * (0.15 + 0.85 * cone) * tw * 0.9;
            }
            col += dc * dust * g_sunI * 0.6;
        }
        // HDR bloom from everything above white
        vec3 bl = vec3(0.0);
        float rad = 0.032;
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + hash12(fragCoord) * 6.28;
            float rr = sqrt((fi + 0.5) / 16.0) * rad;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            vec3 s = texture2D(moScene, st + of).rgb;
            bl += min(max(s - 1.0, 0.0), 2.5) * pow(1.0 - rr / rad, 1.5);
        }
        bl /= 16.0;
        col += bl * bloom * 1.3;
        col *= exposure * mix(1.0, 0.9 + 0.2 * g_lev, g_ar);
        col = aces(col);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.5);
        float l0 = lum(col);
        col = clamp(mix(vec3(l0), col, 1.22 * saturation), 0.0, 1.0);
        float gl = lum(col);
        float g1 = (hash12(fragCoord + fract(g_t) * 61.0) - 0.5) * 0.04;
        col += g1 * grain * (0.3 + 0.7 * gl);
        col *= 1.0 - 0.12 * smoothstep(0.5, 1.5, length(uv));
        col = clamp(col, 0.0, 1.0);
        float alphaOut = transparentBg ? max(1.0 - isSky, smoothstep(0.02, 0.2, lum(bl))) : mix(1.0, backColor.a, backAmt * step(0.001, 1.0 - backColor.a));
        gl_FragColor = vec4(col, alphaOut);
    }
}
