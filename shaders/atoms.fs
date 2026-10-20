/*{
  "DESCRIPTION": "Atoms — a lit 3D scene: an atom (or a cloud of them) hangs over a reflective floor under a sun. The nucleus is a packed cluster of proton and neutron spheres around a white-hot core that lights everything near it; three tilted neon orbit rings carry racing electrons that leave rainbow trails. Real volumetric light: sun shafts through haze, soft shadows, and god rays cast outward from the core through the spinning rings. Electron and core light pools onto the floor, the floor mirrors the atom, fog eats the distance. HDR bloom, exposure, ACES tone mapping, film grain. Palette morphs Prism → Classic → Candy, hues cycle; Background Color tints the void (a light colour turns the frame into airbrushed paper). Bass swells the core and the shafts, mids push the electrons and ring glow, highs twinkle the sparkles, beats flash the sun.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-12 — original. CIDKID chemistry pack #1 (v2: lit scene, floor, volumetric rays)",
  "CATEGORIES": ["Generator", "3D", "Chemistry", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",         "LABEL": "Speed",             "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "electronSpeed", "LABEL": "Electron Speed",    "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "orbitSpeed",    "LABEL": "Camera Orbit",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "colorCycle",    "LABEL": "Color Cycle",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "count",         "LABEL": "Atoms",             "TYPE": "float", "AUDIO": false, "MIN": 1.0,  "MAX": 5.0,  "DEFAULT": 1.0,  "GROUP": "Atoms" },
    { "NAME": "size",          "LABEL": "Size",              "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Atoms" },
    { "NAME": "spread",        "LABEL": "Spread",            "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 4.0,  "DEFAULT": 1.6,  "GROUP": "Atoms" },
    { "NAME": "drift",         "LABEL": "Drift",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Atoms" },
    { "NAME": "spin",          "LABEL": "Spin",              "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Atoms" },
    { "NAME": "rings",         "LABEL": "Rings",             "TYPE": "float", "AUDIO": false, "MIN": 1.0,  "MAX": 3.0,  "DEFAULT": 3.0,  "GROUP": "Atoms" },
    { "NAME": "ringTilt",      "LABEL": "Ring Tilt",         "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.05, "GROUP": "Atoms" },
    { "NAME": "electrons",     "LABEL": "Electrons / Ring",  "TYPE": "float", "AUDIO": false, "MIN": 1.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Atoms" },
    { "NAME": "nucleusSize",   "LABEL": "Nucleus Size",      "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 1.6,  "DEFAULT": 1.0,  "GROUP": "Atoms" },
    { "NAME": "floorOn",       "LABEL": "Floor",             "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 1.0,  "GROUP": "Scene" },
    { "NAME": "floorY",        "LABEL": "Floor Height",      "TYPE": "float", "AUDIO": false, "MIN": -6.0, "MAX": -1.0, "DEFAULT": -2.3, "GROUP": "Scene" },
    { "NAME": "floorReflect",  "LABEL": "Floor Reflection",  "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55, "GROUP": "Scene" },
    { "NAME": "floorColor",    "LABEL": "Floor Color",       "TYPE": "color", "DEFAULT": [0.26, 0.26, 0.32, 1.0], "GROUP": "Scene" },
    { "NAME": "fog",           "LABEL": "Fog",               "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Scene" },
    { "NAME": "haze",          "LABEL": "Haze Density",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Scene" },
    { "NAME": "sunAz",         "LABEL": "Sun Azimuth",       "TYPE": "float", "AUDIO": false, "MIN": -180.0,"MAX": 180.0,"DEFAULT": 35.0, "GROUP": "Light" },
    { "NAME": "sunEl",         "LABEL": "Sun Elevation",     "TYPE": "float", "AUDIO": false, "MIN": 5.0,  "MAX": 85.0, "DEFAULT": 38.0, "GROUP": "Light" },
    { "NAME": "sunInt",        "LABEL": "Sun Intensity",     "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.3,  "GROUP": "Light" },
    { "NAME": "sunColor",      "LABEL": "Sun Color",         "TYPE": "color", "DEFAULT": [1.0, 0.93, 0.82, 1.0], "GROUP": "Light" },
    { "NAME": "shafts",        "LABEL": "Light Rays",        "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "coreLight",     "LABEL": "Core Light",        "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "ringGlow",      "LABEL": "Ring Glow",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "glint",         "LABEL": "Star Glint",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.4,  "GROUP": "Light" },
    { "NAME": "sparkle",       "LABEL": "Sparkles",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Light" },
    { "NAME": "trails",        "LABEL": "Electron Trails",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Light" },
    { "NAME": "trailFade",     "LABEL": "Trail Fade",        "TYPE": "float", "AUDIO": false, "MIN": 0.1,  "MAX": 3.0,  "DEFAULT": 0.8,  "GROUP": "Light" },
    { "NAME": "palette",       "LABEL": "Palette (Prism→Classic→Candy)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "hueShift",      "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",    "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "nebula",        "LABEL": "Nebula",            "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35, "GROUP": "Color" },
    { "NAME": "paper",         "LABEL": "Paper Light",       "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "backColor",     "LABEL": "Background Color",  "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Color" },
    { "NAME": "backAmt",       "LABEL": "Background Amount", "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "exposure",      "LABEL": "Exposure",          "TYPE": "float", "AUDIO": false, "MIN": 0.2,  "MAX": 3.0,  "DEFAULT": 1.15,  "GROUP": "Finish" },
    { "NAME": "bloom",         "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Finish" },
    { "NAME": "contrast",      "LABEL": "Contrast",          "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35, "GROUP": "Finish" },
    { "NAME": "aberration",    "LABEL": "Chromatic Fringe",  "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "grain",         "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.3,  "GROUP": "Finish" },
    { "NAME": "marchRes",      "LABEL": "March Resolution",  "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 1.0,  "DEFAULT": 0.7,  "GROUP": "Finish" },
    { "NAME": "volQuality",    "LABEL": "Ray Quality",       "TYPE": "float", "AUDIO": false, "MIN": 8.0,  "MAX": 48.0, "DEFAULT": 24.0, "GROUP": "Finish" },
    { "NAME": "audioReact",    "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "atScene", "WIDTH": "floor($WIDTH*$marchRes)", "HEIGHT": "floor($HEIGHT*$marchRes)" },
    { "TARGET": "atVol",   "WIDTH": "floor($WIDTH*0.4)",       "HEIGHT": "floor($HEIGHT*0.4)" },
    { "TARGET": "atTrail", "PERSISTENT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// ATOMS v2 — CIDKID chemistry pack #1
//   pass 0  atScene — raymarched atoms + floor, sun with soft shadows,
//                     core/electron point lights, floor reflections, fog.
//                     a = depth (60 = sky)
//   pass 1  atVol   — volumetric light (half res): sun shafts through haze
//                     shadowed by the atoms, god rays from each white-hot
//                     core cast out through its rings; sparkles + glint
//   pass 2  atTrail — persistent electron trails
//   pass 3  final   — composite, HDR bloom, exposure, ACES, contrast,
//                     fringe, grain, paper mode
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define MAXA 5
#define MAXE 6
#define NUCN 9
#define SKY  60.0

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float hash13(vec3 p3) { p3 = fract(p3 * 0.1031); p3 += dot(p3, p3.zyx + 31.32); return fract((p3.x + p3.y) * p3.z); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float vnoise3(vec3 x) {
    vec3 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(hash13(p), hash13(p + vec3(1, 0, 0)), f.x), mix(hash13(p + vec3(0, 1, 0)), hash13(p + vec3(1, 1, 0)), f.x), f.y),
               mix(mix(hash13(p + vec3(0, 0, 1)), hash13(p + vec3(1, 0, 1)), f.x), mix(hash13(p + vec3(0, 1, 1)), hash13(p + vec3(1, 1, 1)), f.x), f.y), f.z);
}
float fbm2(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 5; i++) { s += a * vnoise2(p); p = p * 2.03 + vec2(1.7, 9.2); a *= 0.5; }
    return s;
}
mat3 rotX(float a) { float c = cos(a), s = sin(a); return mat3(1.0, 0.0, 0.0, 0.0, c, s, 0.0, -s, c); }
mat3 rotY(float a) { float c = cos(a), s = sin(a); return mat3(c, 0.0, -s, 0.0, 1.0, 0.0, s, 0.0, c); }
mat3 rotZ(float a) { float c = cos(a), s = sin(a); return mat3(c, s, 0.0, -s, c, 0.0, 0.0, 0.0, 1.0); }
mat3 tr(mat3 m) { return mat3(m[0][0], m[1][0], m[2][0], m[0][1], m[1][1], m[2][1], m[0][2], m[1][2], m[2][2]); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }

// ── palette ──────────────────────────────────────────────────────────────
float g_cyc;
vec3 g3(float t, vec3 a, vec3 b, vec3 c) {
    float s = fract(t) * 3.0;
    if (s < 1.0) return mix(a, b, s);
    if (s < 2.0) return mix(b, c, s - 1.0);
    return mix(c, a, s - 2.0);
}
vec3 palCol(float t) {
    t += hueShift + g_cyc;
    vec3 prism   = pow(0.5 + 0.5 * cos(TAU * (t + vec3(0.0, 0.33, 0.67))), vec3(0.8));
    vec3 classic = g3(t, vec3(1.0, 0.12, 0.06), vec3(1.0, 0.62, 0.06), vec3(0.10, 0.45, 1.0));
    vec3 candy   = g3(t, vec3(1.0, 0.16, 0.60), vec3(0.62, 0.42, 1.0), vec3(0.42, 0.85, 1.0));
    vec3 c = mix(mix(prism, classic, clamp(palette, 0.0, 1.0)), candy, clamp(palette - 1.0, 0.0, 1.0));
    float l = lum(c);
    return clamp(mix(vec3(l), c, saturation), 0.0, 1.0);
}

// ── per-frame state ──────────────────────────────────────────────────────
float g_t, g_ar, g_lens, g_nPulse, g_ringLift, g_bass, g_mid, g_high, g_beat, g_lev, g_floorY, g_sunI;
int   g_n, g_nr, g_ne;
vec3  g_ro, g_uu, g_vv, g_ww, g_sun, g_sunC, g_back;
vec3  g_apos[MAXA];
mat3  g_arot[MAXA];
mat3  g_arotI[MAXA];
float g_ahue[MAXA];
float g_asz[MAXA];
float g_aseed[MAXA];
mat3  g_ringM[MAXA * 3];
mat3  g_ringMI[MAXA * 3];
vec3  g_rcol[MAXA * 3];
vec3  g_epos[MAXA * MAXE];
vec3  g_ecol[MAXA * MAXE];

vec3 nucOff(int k) {
    if (k == 0) return vec3(0.0, 0.0, 0.0);
    if (k == 1) return vec3(0.30, 0.10, 0.16);
    if (k == 2) return vec3(-0.26, 0.20, -0.10);
    if (k == 3) return vec3(0.10, -0.30, 0.16);
    if (k == 4) return vec3(-0.10, -0.16, -0.30);
    if (k == 5) return vec3(0.16, 0.30, -0.20);
    if (k == 6) return vec3(-0.30, -0.08, 0.24);
    if (k == 7) return vec3(0.26, -0.22, -0.24);
    return vec3(-0.06, 0.34, 0.20);
}

void setupAtoms() {
    g_nr = int(clamp(rings + 0.5, 1.0, 3.0));
    g_ne = int(clamp(electrons + 0.5, 1.0, 2.0));
    float t = g_t * speed;
    g_nPulse = 1.0 + 0.05 * sin(g_t * 2.1) + g_ar * 0.18 * g_bass;
    g_ringLift = 1.0 + 0.9 * g_ar * g_mid;
    float dScale = (g_n == 1) ? 0.35 : 1.0;
    for (int i = 0; i < MAXA; i++) {
        if (i >= g_n) break;
        float sd = hash11(float(i) * 5.17 + 2.0);
        g_aseed[i] = sd;
        float ang = float(i) * TAU / float(g_n) + sd * 0.7;
        float rad = (g_n == 1) ? 0.0 : spread * (1.1 + 0.4 * sd);
        vec3 home = vec3(cos(ang) * rad, (hash11(sd + 1.0) - 0.5) * spread * 0.6, sin(ang) * rad * 0.7);
        vec3 dr = vec3(sin(t * 0.13 + sd * 9.0) + 0.4 * sin(t * 0.07 + sd * 3.0),
                       0.7 * sin(t * 0.11 + sd * 5.0) + 0.3 * sin(t * 0.05 + sd),
                       sin(t * 0.09 + sd * 7.0) + 0.4 * cos(t * 0.06 + sd * 2.0)) * 0.5 * drift * (0.4 + 0.5 * spread) * dScale;
        g_apos[i] = home + dr;
        g_asz[i] = size * ((g_n == 1) ? 1.0 : (0.72 + 0.5 * hash11(sd + 4.0)));
        float sa = t * spin * (0.15 + 0.1 * sd) + sd * TAU;
        float sb = t * spin * (0.11 + 0.08 * hash11(sd + 8.0)) + sd * 3.0;
        mat3 Ra = rotY(sa) * rotX(sb * 0.7) * rotZ(0.3 * sin(t * 0.1 + sd));
        g_arot[i] = Ra;
        g_arotI[i] = tr(Ra);
        g_ahue[i] = float(i) / float(g_n) * 0.6 + sd * 0.2;
        for (int j = 0; j < 3; j++) {
            float sgn = (j == 0) ? 0.0 : ((j == 1) ? 1.0 : -1.0);
            mat3 M = rotY(float(j) * 1.1 + 0.4) * rotZ(ringTilt * sgn) * rotX((j == 0) ? 0.18 : 0.0);
            g_ringM[i * 3 + j] = M;
            g_ringMI[i * 3 + j] = tr(M);
            g_rcol[i * 3 + j] = palCol(g_ahue[i] + float(j) * 0.21);
            for (int k = 0; k < 2; k++) {
                float ph = t * electronSpeed * (1.2 + 0.4 * float(j) + 0.3 * sd) * 1.1 + g_ar * 0.5 * g_mid + float(k) * PI + sd * TAU + float(j) * 2.0;
                ph *= ((j == 1) ? -1.0 : 1.0);
                vec3 lp = M * vec3(cos(ph), 0.0, sin(ph));
                g_epos[i * MAXE + j * 2 + k] = g_apos[i] + Ra * lp * g_asz[i];
                g_ecol[i * MAXE + j * 2 + k] = mix(palCol(g_ahue[i] + float(j) * 0.21 + 0.06), vec3(1.0), 0.35);
            }
        }
    }
}

void setupCam() {
    float oa = g_t * 0.09 * orbitSpeed + 0.6;
    float dist = 4.3 + size * 1.1 + ((g_n > 1) ? spread * 1.25 : 0.0);
    g_ro = vec3(sin(oa) * dist, 1.1 + 0.5 * sin(g_t * 0.13 * orbitSpeed + 1.0), cos(oa) * dist);
    vec3 ta = vec3(0.0, -0.15, 0.0);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    g_lens = 1.6;
    float az = radians(sunAz), el = radians(sunEl);
    g_sun = normalize(vec3(cos(el) * sin(az), sin(el), cos(el) * cos(az)));
    g_sunC = sunColor.rgb;
    g_sunI = sunInt * (1.0 + 0.35 * g_ar * g_beat);
    g_floorY = floorY;
    g_back = backColor.rgb * backAmt;
}
vec3 camRay(vec2 uv) { return normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_lens); }
vec3 project(vec3 p) {
    vec3 d = p - g_ro;
    float z = dot(d, g_ww);
    vec2 s = vec2(dot(d, g_uu), dot(d, g_vv)) / max(z, 0.05) * g_lens;
    return vec3(s, z);
}
// sky: background colour graded by elevation + sun halo
vec3 skyCol(vec3 rd) {
    vec3 c = g_back * (0.75 + 0.45 * smoothstep(-0.3, 0.6, rd.y));
    c += g_sunC * g_sunI * 0.05 * haze * exp(-abs(rd.y + 0.05) * 9.0) * (0.5 + 0.5 * max(dot(rd.xz, g_sun.xz), 0.0));
    float sd = max(dot(rd, g_sun), 0.0);
    c += g_sunC * g_sunI * (pow(sd, 48.0) * 0.6 + pow(sd, 8.0) * 0.06) * haze;
    return c;
}

// ── current atom (copied with a loop index — GLSL ES 1.0 subscripts) ──────
vec3  c_apos; mat3 c_arotI; float c_asz; float c_ahue;
mat3  c_ringMI[3]; vec3 c_rcol[3];
vec3  c_epos[MAXE]; vec3 c_ecol[MAXE];
void loadAtom(int i) {
    for (int n = 0; n < MAXA; n++) {
        if (n != i) continue;
        c_apos = g_apos[n]; c_arotI = g_arotI[n]; c_asz = g_asz[n]; c_ahue = g_ahue[n];
        for (int j = 0; j < 3; j++) {
            c_ringMI[j] = g_ringMI[n * 3 + j];
            c_rcol[j] = g_rcol[n * 3 + j];
            for (int k = 0; k < 2; k++) {
                c_epos[j * 2 + k] = g_epos[n * MAXE + j * 2 + k];
                c_ecol[j * 2 + k] = g_ecol[n * MAXE + j * 2 + k];
            }
        }
    }
}

// ── atom distance fields ─────────────────────────────────────────────────
void atomEval(vec3 pw, float kR, out float dN, out float dR, out float dE, out int nSub, out vec3 glow) {
    float s = c_asz;
    vec3 p = c_arotI * (pw - c_apos) / s;
    float nr = 0.19 * nucleusSize * g_nPulse;
    dN = 1e9; nSub = 0;
    for (int k = 0; k < NUCN; k++) {
        float dk = length(p - nucOff(k) * nucleusSize * g_nPulse) - nr * ((k == 0) ? 1.15 : 1.0);
        if (dk < dN) { dN = dk; nSub = k; }
    }
    dR = 1e9; dE = 1e9;
    glow = vec3(0.0);
    for (int j = 0; j < 3; j++) {
        if (j >= g_nr) break;
        vec3 q = c_ringMI[j] * p;
        float d = (length(vec2(length(q.xz) - 1.0, q.y)) - 0.018) * s;
        dR = min(dR, d);
        glow += c_rcol[j] * exp(-max(d, 0.0) * kR) * 0.35 * ringGlow * g_ringLift;
        for (int k = 0; k < 2; k++) {
            if (k >= g_ne) break;
            vec3 el = c_arotI * (c_epos[j * 2 + k] - c_apos) / s;
            float de = (length(p - el) - 0.07) * s;
            dE = min(dE, de);
            glow += c_ecol[j * 2 + k] * exp(-max(de, 0.0) * kR * 1.3) * 0.9;
        }
    }
    dN *= s;
}
float atomDist(vec3 pw) {
    float dN, dR, dE; int a; vec3 g;
    atomEval(pw, 1.0, dN, dR, dE, a, g);
    return min(dN, min(dR, dE));
}
vec3 atomNormal(vec3 p) {
    vec2 e = vec2(0.0012, -0.0012);
    return normalize(e.xyy * atomDist(p + e.xyy) + e.yyx * atomDist(p + e.yyx) +
                     e.yxy * atomDist(p + e.yxy) + e.xxx * atomDist(p + e.xxx));
}
// what the core light has to get through: the proton/neutron spheres around
// it (not the core itself) and the rings, thickened so they throw real beams
float ringDist(vec3 pw) {
    float s = c_asz;
    vec3 p = c_arotI * (pw - c_apos) / s;
    float nr = 0.19 * nucleusSize * g_nPulse;
    float d = 1e9;
    for (int k = 1; k < NUCN; k++) d = min(d, length(p - nucOff(k) * nucleusSize * g_nPulse) - nr * 0.92);
    for (int j = 0; j < 3; j++) {
        if (j >= g_nr) break;
        vec3 q = c_ringMI[j] * p;
        d = min(d, length(vec2(length(q.xz) - 1.0, q.y)) - 0.05);
    }
    return d * s;
}
// soft visibility from p toward the loaded atom's core, rings only
float coreVis(vec3 p, vec3 lv, float ld, float k) {
    float res = 1.0, t = 0.02;
    for (int s = 0; s < 10; s++) {
        float d = ringDist(p + lv * t);
        res = min(res, k * d / t);
        if (res < 0.01) return 0.0;
        t += clamp(d, 0.025, 0.35);
        if (t > ld) break;
    }
    return clamp(res, 0.0, 1.0);
}
bool atomBound(int i, vec3 ro, vec3 rd, out float t0, out float t1) {
    float s = 1.0; vec3 ap = vec3(0.0);
    for (int n = 0; n < MAXA; n++) { if (n == i) { s = g_asz[n]; ap = g_apos[n]; } }
    float br = s * (1.06 + 0.1 * max(nucleusSize - 1.0, 0.0));
    vec3 oc = ro - ap;
    float b = dot(oc, rd);
    float c = dot(oc, oc) - br * br;
    float h = b * b - c;
    if (h < 0.0) return false;
    h = sqrt(h);
    t0 = max(-b - h, 0.0); t1 = -b + h;
    return t1 > t0;
}

// soft shadow / visibility along rd through every atom. skipNuc: ignore the
// nucleus (the core is the light itself; only rings + electrons cast)
float visibility(vec3 ro, vec3 rd, float maxT, float k, int steps, bool skipNuc) {
    float res = 1.0;
    for (int i = 0; i < MAXA; i++) {
        if (i >= g_n) break;
        float t0, t1;
        if (!atomBound(i, ro, rd, t0, t1)) continue;
        t0 = max(t0, 0.01); t1 = min(t1, maxT);
        if (t1 <= t0) continue;
        loadAtom(i);
        float t = t0, ph = 1e10;
        for (int s = 0; s < 32; s++) {
            if (s >= steps) break;
            float dN, dR, dE; int nS; vec3 g;
            atomEval(ro + rd * t, 1.0, dN, dR, dE, nS, g);
            float d = skipNuc ? min(dR, dE) : min(dN, min(dR, dE));
            // penumbra from the last two samples (iq) — smooth, no banding
            float y = d * d / (2.0 * ph);
            float dd = sqrt(max(d * d - y * y, 0.0));
            res = min(res, k * dd / max(t - y, 0.001));
            ph = d;
            if (res < 0.003) return 0.0;
            t += clamp(d, 0.01, 0.3);
            if (t > t1) break;
        }
    }
    return clamp(res, 0.0, 1.0);
}

float allAtomsDist(vec3 p) {
    float d = 1e9;
    for (int i = 0; i < MAXA; i++) {
        if (i >= g_n) break;
        loadAtom(i);
        d = min(d, atomDist(p));
    }
    return d;
}
// ambient occlusion: contact darkening between spheres and under the atom
float aoAtoms(vec3 p, vec3 n, float scale) {
    float occ = 0.0, sca = 1.0;
    for (int i = 0; i < 5; i++) {
        float h = (0.03 + 0.11 * float(i)) * scale;
        float d = allAtomsDist(p + n * h);
        occ += (h - d) * sca;
        sca *= 0.72;
    }
    return clamp(1.0 - 1.8 * occ / scale, 0.0, 1.0);
}

// point lights of the loaded atom onto a surface point
vec3 atomLights(vec3 p, vec3 n, vec3 rd, float spec) {
    vec3 col = vec3(0.0);
    float s = c_asz;
    // the white-hot core
    vec3 lv = c_apos - p; float ld = length(lv); lv /= ld;
    vec3 cc = mix(palCol(c_ahue + 0.5), vec3(1.0), 0.7);
    float att = coreLight * 2.4 / (0.4 + ld * ld * 0.8 / (s * s));
    col += cc * (max(dot(n, lv), 0.0) + pow(max(dot(reflect(-lv, n), -rd), 0.0), 40.0) * spec) * att;
    for (int j = 0; j < 3; j++) {
        if (j >= g_nr) break;
        for (int k = 0; k < 2; k++) {
            if (k >= g_ne) break;
            vec3 ev = c_epos[j * 2 + k] - p; float ed = length(ev); ev /= ed;
            float ea = 0.9 / (0.3 + ed * ed * 2.2 / (s * s));
            col += c_ecol[j * 2 + k] * (max(dot(n, ev), 0.0) * 0.8 + pow(max(dot(reflect(-ev, n), -rd), 0.0), 26.0) * spec) * ea;
        }
    }
    return col;
}

// shade an atom surface (atom loaded). simple = reflection-quality shading
vec3 shadeAtom(int ai, vec3 p, vec3 rd, vec3 n, int mat, int sub, bool simple) {
    loadAtom(ai);
    float s = c_asz; float hue = c_ahue;
    vec3 col = vec3(0.0);
    if (mat == 0) {
        if (sub == 0) {
            // the core: white-hot, palette-tinted, breathing with the bass
            vec3 cc = mix(palCol(hue + 0.5), vec3(1.0), 0.75);
            float fr = pow(1.0 - max(dot(n, -rd), 0.0), 2.0);
            col = cc * (3.0 + 2.5 * g_ar * g_bass) * coreLight * (0.7 + 0.5 * fr) + cc * 0.6;
        } else {
            vec3 base = (mod(float(sub), 2.0) < 0.5) ? palCol(hue) : palCol(hue + 0.5);
            base = mix(base, vec3(1.0), 0.06);
            float sh = simple ? 1.0 : visibility(p + n * 0.02, g_sun, 12.0, 16.0, 24, false);
            float ao = simple ? 1.0 : aoAtoms(p, n, s);
            loadAtom(ai);
            float dif = max(dot(n, g_sun), 0.0);
            float spc = pow(max(dot(reflect(-g_sun, n), -rd), 0.0), 60.0);
            vec3 sky = g_back * 0.35 + vec3(0.03, 0.035, 0.05);
            col = base * (sky * 0.8 * ao + g_sunC * g_sunI * (dif * 0.95 + 0.05 * ao) * sh) + g_sunC * spc * g_sunI * sh * 0.9;
            col += base * atomLights(p, n, rd, 1.0) * (0.35 + 0.65 * ao);
            float fr = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);
            col += palCol(hue + 0.25) * fr * 0.35;
            col *= 0.94 + 0.06 * vnoise2(p.xy * 90.0 / s + p.z * 40.0);
        }
    } else if (mat == 1) {
        vec3 pl = c_arotI * (p - c_apos) / s;
        float best = 1e9; vec3 rc = vec3(1.0); vec3 hot = vec3(0.0);
        for (int j = 0; j < 3; j++) {
            if (j >= g_nr) break;
            vec3 q = c_ringMI[j] * pl;
            float d = length(vec2(length(q.xz) - 1.0, q.y));
            if (d < best) {
                best = d; rc = c_rcol[j]; hot = vec3(0.0);
                for (int k = 0; k < 2; k++) {
                    if (k >= g_ne) break;
                    float de = length(c_epos[j * 2 + k] - p);
                    hot += c_ecol[j * 2 + k] * exp(-de * de * 14.0 / (s * s)) * 2.2;
                }
            }
        }
        float spc = pow(max(dot(reflect(-g_sun, n), -rd), 0.0), 50.0);
        col = rc * (1.3 + 0.6 * g_ar * g_mid) * ringGlow + vec3(0.25) + hot + g_sunC * spc * g_sunI * 0.8;
        col += vec3(0.5) * pow(1.0 - max(dot(n, -rd), 0.0), 2.0);
    } else {
        float best = 1e9; vec3 ec = vec3(1.0);
        for (int j = 0; j < 3; j++) {
            if (j >= g_nr) break;
            for (int k = 0; k < 2; k++) {
                if (k >= g_ne) break;
                float de = length(c_epos[j * 2 + k] - p);
                if (de < best) { best = de; ec = c_ecol[j * 2 + k]; }
            }
        }
        col = mix(ec, vec3(1.0), 0.65) * 2.8;
    }
    return col;
}

// march all atoms; returns nearest hit t (or -1); glow accumulated
float marchAtoms(vec3 ro, vec3 rd, float maxT, int steps, out int hitA, out int hitM, out int hitSub, inout vec3 glow) {
    float hitT = maxT; hitA = -1; hitM = 0; hitSub = 0;
    for (int i = 0; i < MAXA; i++) {
        if (i >= g_n) break;
        float t0, t1;
        if (!atomBound(i, ro, rd, t0, t1)) continue;
        if (t0 > hitT) continue;
        t1 = min(t1, hitT);
        loadAtom(i);
        float s = c_asz;
        float t = t0;
        float kR = 20.0 / s;
        for (int st = 0; st < 80; st++) {
            if (st >= steps) break;
            vec3 p = ro + rd * t;
            float dN, dR, dE; int nS; vec3 g;
            atomEval(p, kR, dN, dR, dE, nS, g);
            float d = min(dN, min(dR, dE));
            float stp = clamp(d * 0.8, 0.003 * s, 0.22 * s);
            glow += g * stp / s;
            if (d < 0.0025 * (1.0 + t * 0.08)) {
                hitT = t; hitA = i;
                hitM = (dN <= dR && dN <= dE) ? 0 : ((dR <= dE) ? 1 : 2);
                hitSub = nS;
                break;
            }
            t += stp;
            if (t > t1) break;
        }
    }
    return (hitA >= 0) ? hitT : -1.0;
}

vec3 shadeFloor(vec3 p, vec3 rd) {
    vec3 n = vec3(0.0, 1.0, 0.0);
    vec3 fc = floorColor.rgb;
    float dif = max(dot(n, g_sun), 0.0);
    float sh = visibility(p + n * 0.02, g_sun, 16.0, 14.0, 24, false);
    float ao = aoAtoms(p, n, 2.2);
    vec3 sky = g_back * 0.4 + vec3(0.025, 0.03, 0.045);
    vec3 col = fc * (sky * ao + g_sunC * g_sunI * dif * sh * 1.1);
    // the atoms pool light onto the floor
    for (int i = 0; i < MAXA; i++) {
        if (i >= g_n) break;
        loadAtom(i);
        col += fc * atomLights(p, n, rd, 0.6) * 1.6 * (0.4 + 0.6 * ao);
    }
    // sun sheen
    vec3 rr = reflect(rd, n);
    col += g_sunC * pow(max(dot(rr, g_sun), 0.0), 32.0) * 0.18 * g_sunI * floorReflect;
    // mirror the atoms
    if (floorReflect > 0.001) {
        vec3 refl = skyCol(rr) * 0.6;
        int hA, hM, hS; vec3 g2 = vec3(0.0);
        float t2 = marchAtoms(p + n * 0.01, rr, 25.0, 32, hA, hM, hS, g2);
        if (t2 > 0.0) {
            vec3 p2 = p + n * 0.01 + rr * t2;
            vec3 n2 = atomNormal(p2);
            refl = shadeAtom(hA, p2, rr, n2, hM, hS, true) * exp(-t2 * 0.06);
        }
        refl += g2 * 0.6;
        float fr = pow(1.0 - max(dot(n, -rd), 0.0), 2.0);
        col = mix(col, refl, floorReflect * (0.3 + 0.7 * fr));
    }
    // soft fine texture so the floor is not a flat slab
    col *= 0.96 + 0.04 * vnoise2(p.xz * 6.0) + 0.02 * vnoise2(p.xz * 40.0);
    return col;
}

vec4 renderScene(vec3 ro, vec3 rd) {
    vec3 glow = vec3(0.0);
    int hA, hM, hS;
    float tF = 1e9;
    if (floorOn > 0.001 && rd.y < -0.001) tF = (g_floorY - ro.y) / rd.y;
    float t = marchAtoms(ro, rd, min(tF, 40.0), 80, hA, hM, hS, glow);
    vec3 col; float depth;
    if (t > 0.0) {
        loadAtom(hA);
        vec3 p = ro + rd * t;
        vec3 n = atomNormal(p);
        col = shadeAtom(hA, p, rd, n, hM, hS, false);
        depth = t;
    } else if (tF < 40.0) {
        vec3 p = ro + rd * tF;
        vec3 fl = shadeFloor(p, rd);
        float ff = 1.0 - exp(-tF * 0.045 * fog);
        fl = mix(fl, skyCol(rd), ff);
        col = mix(skyCol(rd), fl, floorOn);
        depth = tF;
    } else {
        col = skyCol(rd);
        depth = SKY;
    }
    col += glow;
    return vec4(col, (depth >= SKY - 0.5) ? 0.9 : 1.0);
}

// ── pass 1: volumetric light + sparkles ──────────────────────────────────
vec3 volPass(vec2 uv, vec2 fragCoord) {
    vec3 rd = camRay(uv);
    vec3 ro = g_ro;
    // where the ray ends: the floor or the nucleus ball (rings are thin)
    float tmax = 26.0;
    if (floorOn > 0.001 && rd.y < -0.001) tmax = min(tmax, (g_floorY - ro.y) / rd.y);
    // march only the haze around the atoms (that is where the rays live)
    float ta = 1e9, tb = -1e9;
    for (int i = 0; i < MAXA; i++) {
        if (i >= g_n) break;
        float s_ = g_asz[i];
        vec3 oc = ro - g_apos[i];
        float b = dot(oc, rd);
        float c = dot(oc, oc) - s_ * s_ * 0.3 * nucleusSize * nucleusSize;
        float h = b * b - c;
        if (h > 0.0) { float tn = -b - sqrt(h); if (tn > 0.0) tmax = min(tmax, tn); }
        float br = s_ * 2.6;
        float c2 = dot(oc, oc) - br * br;
        float h2 = b * b - c2;
        if (h2 > 0.0) { h2 = sqrt(h2); ta = min(ta, -b - h2); tb = max(tb, -b + h2); }
    }
    ta = max(ta, 0.0); tb = min(tb, tmax);
    if (tb <= ta) return vec3(0.0);
    int steps = int(volQuality);
    float dt = (tb - ta) / float(steps);
    float jit = fract(52.9829189 * fract(dot(fragCoord, vec2(0.06711056, 0.00583715))));
    vec3 acc = vec3(0.0);
    float T = 1.0;
    float phaseSun = 0.12 + 0.88 * pow(max(dot(rd, g_sun), 0.0), 10.0);
    float hz = haze * (1.0 + 0.5 * g_ar * g_bass);
    for (int s = 0; s < 48; s++) {
        if (s >= steps) break;
        float t = ta + (float(s) + jit) * dt;
        vec3 p = ro + rd * t;
        // haze: thicker low, slowly drifting
        float dens = hz * (0.55 + 0.45 * vnoise3(p * 0.45 + vec3(g_t * 0.03, 0.0, g_t * 0.02))) * (0.35 + 0.65 * exp(-max(p.y - g_floorY, 0.0) * 0.12));
        dens *= 0.055;
        // the haze gathers around the atoms: rays live near them, the void stays black
        float near = 0.0;
        for (int i = 0; i < MAXA; i++) {
            if (i >= g_n) break;
            float ld0 = length(g_apos[i] - p);
            near += exp(-max(ld0 - g_asz[i] * 0.8, 0.0) * 0.75);
        }
        dens *= 0.1 + 2.6 * near;
        vec3 inscat = vec3(0.0);
        // sun shafts, shadowed by the atoms
        float vs = visibility(p, g_sun, 10.0, 4.0, 6, false);
        inscat += g_sunC * g_sunI * vs * phaseSun * 0.9;
        // god rays from each white-hot core out through its rings
        for (int i = 0; i < MAXA; i++) {
            if (i >= g_n) break;
            loadAtom(i);
            vec3 lv = c_apos - p; float ld = length(lv); lv /= ld;
            float s_ = c_asz;
            float vc = coreVis(p, lv, ld - 0.22 * s_ * nucleusSize, 10.0);
            vec3 cc = mix(palCol(g_ahue[i] + 0.5), vec3(1.0), 0.6);
            float att = coreLight * (1.0 + 0.8 * g_ar * g_bass) * 6.5 / (0.8 + ld * ld * 0.9 / (s_ * s_));
            inscat += cc * att * vc;
        }
        float a = dens * dt;
        acc += inscat * a * T * shafts;
        T *= exp(-a * 0.6);
    }
    // sparkles + a small star glint riding with each core
    vec3 col = acc;
    for (int i = 0; i < MAXA; i++) {
        if (i >= g_n) break;
        vec3 pr = project(g_apos[i]);
        if (pr.z < 0.3) continue;
        float ss = g_asz[i] * g_lens / pr.z;
        float sd = g_aseed[i];
        vec2 v = uv - pr.xy;
        float r = length(v);
        if (r > ss * 2.2) continue;
        float gw = 1.0 / (ss * 0.02);
        float card = exp(-min(abs(v.x), abs(v.y)) * gw) * exp(-r / (ss * 0.45)) * smoothstep(ss * 1.5, ss * 0.4, r);
        float diag = exp(-min(abs(v.x + v.y), abs(v.x - v.y)) * 0.707 * gw) * exp(-r / (ss * 0.25));
        vec3 gcol = mix(vec3(1.0), palCol(sd + 0.5), 0.4);
        col += gcol * (card + diag * 0.7) * glint * (1.0 + 0.35 * g_beat * g_ar) * 1.5;
        mat3 rz = rotZ(g_t * 0.03 * speed + sd);
        vec2 sv = (rz * vec3(v, 0.0)).xy;
        float cs = ss * 0.2;
        vec2 g = sv / cs;
        vec2 gi = floor(g), gf = fract(g);
        for (int dy = -1; dy <= 1; dy++) {
            for (int dx = -1; dx <= 1; dx++) {
                vec2 cell = gi + vec2(float(dx), float(dy));
                float h = hash12(cell + sd * 77.0);
                if (h < 0.6) continue;
                vec2 pp = vec2(hash12(cell + 1.3), hash12(cell + 2.7));
                vec2 dv = (gf - vec2(float(dx), float(dy)) - pp) * cs;
                float dd = dot(dv, dv);
                float tw = pow(0.5 + 0.5 * sin(g_t * (1.2 + 3.0 * h) * speed + h * TAU), 5.0) * (1.0 + 1.6 * g_ar * g_high);
                float w = ss * 0.011;
                float star = exp(-dd / (w * w)) * 1.4
                           + 0.4 * (exp(-abs(dv.x) / (w * 0.35)) * exp(-abs(dv.y) / (w * 3.0)) + exp(-abs(dv.y) / (w * 0.35)) * exp(-abs(dv.x) / (w * 3.0)));
                col += mix(vec3(1.0), palCol(h + sd), 0.55) * star * tw * exp(-r / (ss * 1.4)) * sparkle;
            }
        }
    }
    return col;
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    g_ar = audioReact;
    g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_cyc = g_t * 0.045 * colorCycle;
    g_n = int(clamp(count + 0.5, 1.0, float(MAXA)));
    setupCam();
    setupAtoms();

    if (PASSINDEX == 0) {
        vec3 rd = camRay(uv);
        vec4 sc = renderScene(g_ro, rd);
        gl_FragColor = vec4(max(sc.rgb, 0.0), sc.a);
    } else if (PASSINDEX == 1) {
        vec2 st = fragCoord / R;
        gl_FragColor = vec4(max(volPass(uv, fragCoord), 0.0), 1.0);
    } else if (PASSINDEX == 2) {
        vec2 st = fragCoord / R;
        vec3 prev = (FRAMEINDEX < 2) ? vec3(0.0) : texture2D(atTrail, st).rgb;
        float dt = clamp(TIMEDELTA, 1.0 / 240.0, 0.1);
        float decay = exp(-dt * 2.4 / max(trailFade, 0.05));
        vec3 cur = vec3(0.0);
        for (int i = 0; i < MAXA; i++) {
            if (i >= g_n) break;
            for (int j = 0; j < 3; j++) {
                if (j >= g_nr) break;
                for (int k = 0; k < 2; k++) {
                    if (k >= g_ne) break;
                    vec3 pr = project(g_epos[i * MAXE + j * 2 + k]);
                    if (pr.z < 0.3) continue;
                    float se = 0.075 * g_asz[i] * g_lens / pr.z;
                    vec2 d = uv - pr.xy;
                    cur += g_ecol[i * MAXE + j * 2 + k] * exp(-dot(d, d) / (se * se * 1.1)) * (1.1 + 0.4 * g_ar * g_lev);
                }
            }
        }
        vec3 trl = max(prev * decay, cur);
        if (trails <= 0.001) trl = vec3(0.0);
        gl_FragColor = vec4(trl, 1.0);
    } else {
        vec2 st = fragCoord / R;
        vec2 asp = vec2(R.y / R.x, 1.0);
        vec2 dir = (st - 0.5) * 2.0;
        float ab = aberration * 0.003 * (1.0 + 0.6 * g_ar * g_bass);
        vec2 o = dir * ab;
        vec4 c0 = texture2D(atScene, st);
        float isSky = step(c0.a, 0.95);
        vec3 sc = vec3(texture2D(atScene, st + o).r, c0.g, texture2D(atScene, st - o).b);
        // volumetric: 12-tap disc blur — light shafts are low frequency, the
        // march noise is not
        vec3 vol = texture2D(atVol, st).rgb;
        for (int i = 0; i < 12; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + 0.7;
            float rr = sqrt((fi + 0.5) / 12.0) * 0.0042;
            vol += texture2D(atVol, st + vec2(cos(a), sin(a)) * rr * asp).rgb;
        }
        vol /= 13.0;
        vec3 trl = vec3(texture2D(atTrail, st + o).r, texture2D(atTrail, st).g, texture2D(atTrail, st - o).b);

        // nebula only in the sky
        vec3 col = sc;
        if (isSky > 0.5 && nebula > 0.001) {
            vec2 nu = uv * 1.3 + vec2(g_t * 0.018, -g_t * 0.012) * speed;
            float f1 = fbm2(nu * 1.7 + 3.0 + g_t * 0.01);
            float f = fbm2(nu + 0.7 * vec2(f1, fbm2(nu * 1.3 - 7.0)));
            vec3 neb = palCol(f * 0.5 + 0.55) * pow(f, 2.2) * 0.5 * nebula;
            float dust = pow(hash12(floor(uv * 260.0 + 0.5)), 90.0) * (0.5 + 0.5 * sin(g_t * 3.0 + hash12(floor(uv * 260.0)) * 40.0));
            neb += vec3(dust) * 0.3 * nebula;
            col += neb;
        }
        col += vol + trl * trails * (1.0 + 0.4 * g_ar * g_lev);

        // HDR bloom from everything above white
        vec3 bl = vec3(0.0);
        float rad = 0.03;
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + hash12(fragCoord) * 6.28;
            float rr = sqrt((fi + 0.5) / 16.0) * rad;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            vec3 s = texture2D(atScene, st + of).rgb + texture2D(atVol, st + of).rgb + texture2D(atTrail, st + of).rgb * trails;
            bl += max(s - 1.0, 0.0) * (1.0 - rr / rad);
        }
        bl /= 16.0;
        col += bl * bloom * 1.2;

        col *= exposure * mix(1.0, 0.86 + 0.28 * g_lev, g_ar);
        col = aces(col);
        col = mix(col, col * col * (3.0 - 2.0 * col), contrast);
        float l0 = lum(col);
        col = clamp(mix(vec3(l0), col, 1.15), 0.0, 1.0);

        // paper mode: light background colour flips the frame
        float bgl = lum(g_back);
        float papAmt = max(paper, smoothstep(0.22, 0.5, bgl));
        if (papAmt > 0.001) {
            float l = lum(col);
            vec3 tint = col / max(l, 1e-3);
            tint = clamp(mix(vec3(lum(tint)), tint, 1.6) * 0.78, 0.0, 1.0);
            vec3 pap = mix(vec3(1.0, 0.95, 0.955), g_back, smoothstep(0.22, 0.5, bgl));
            vec3 light = mix(pap, tint, smoothstep(0.0, 0.42, l));
            col = mix(col, light, papAmt);
        }
        float gl = lum(col);
        float g1 = (hash12(fragCoord + fract(g_t) * 61.0) - 0.5) * 0.04;
        col += g1 * grain * (0.3 + 0.7 * gl);
        col *= 1.0 - 0.12 * smoothstep(0.45, 1.45, length(uv));
        col = clamp(col, 0.0, 1.0);
        float alphaOut = transparentBg ? max(1.0 - isSky, smoothstep(0.02, 0.2, lum(vol + trl * trails + bl))) : mix(1.0, backColor.a, backAmt * step(0.001, 1.0 - backColor.a));
        gl_FragColor = vec4(col, alphaOut);
    }
}
