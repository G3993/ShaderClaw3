/*{
  "DESCRIPTION": "Orbitals — the electron clouds of a hydrogen atom rendered as real quantum probability: 1s, 2s, 2p, 3d, 4f and sp³ hybrid wavefunctions (exact Laguerre radial × spherical harmonic forms) raymarched as glowing volumetric density, the two phase signs of the wavefunction painted in two colours. The atom cycles through orbitals with quantum jumps: the cloud collapses toward the nucleus, a photon flash bursts out as an expanding ring of light, and the next orbital blooms open. Nested probability shells flow outward through the cloud as thin crisp iso-lines, the nucleus burns white at the centre, electron dust specks orbit through the field, and a faint lab-floor grid grounds it all. Or dial any orbital by hand and morph between neighbours. Bass swells the cloud, mids spin the phase, highs twinkle the dust, beats fire a photon flash.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-13 — original. CIDKID chemistry pack #7",
  "CATEGORIES": ["Generator", "3D", "Chemistry", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Spin Speed",          "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "cycleSpeed",   "LABEL": "Jump Rate",           "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "orbitSpeed",   "LABEL": "Camera Orbit",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Speed" },
    { "NAME": "waveSpeed",    "LABEL": "Shell Flow",          "TYPE": "float", "MIN": -2.0, "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Speed" },
    { "NAME": "orbital",      "LABEL": "Orbital (0 = auto cycle)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 8.0, "DEFAULT": 0.0, "GROUP": "Orbital" },
    { "NAME": "size",         "LABEL": "Cloud Size",          "TYPE": "float", "AUDIO": false, "MIN": 0.5,  "MAX": 1.6,  "DEFAULT": 1.0,  "GROUP": "Orbital" },
    { "NAME": "density",      "LABEL": "Density",             "TYPE": "float", "AUDIO": false, "MIN": 0.2,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Orbital" },
    { "NAME": "absorb",       "LABEL": "Opacity",             "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Orbital" },
    { "NAME": "shells",       "LABEL": "Probability Shells",  "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Orbital" },
    { "NAME": "flash",        "LABEL": "Photon Flash",        "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Orbital" },
    { "NAME": "dust",         "LABEL": "Electron Dust",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Orbital" },
    { "NAME": "nucleus",      "LABEL": "Nucleus Glow",        "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Orbital" },
    { "NAME": "tilt",         "LABEL": "Tilt",                "TYPE": "float", "AUDIO": false, "MIN": -1.2, "MAX": 1.2,  "DEFAULT": 0.35, "GROUP": "Camera" },
    { "NAME": "zoom",         "LABEL": "Zoom",                "TYPE": "float", "AUDIO": false, "MIN": 0.5,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Camera" },
    { "NAME": "floorAmt",     "LABEL": "Floor Grid",          "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Camera" },
    { "NAME": "colorPlus",    "LABEL": "Phase + Color",       "TYPE": "color", "DEFAULT": [0.15, 0.62, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "colorMinus",   "LABEL": "Phase − Color",       "TYPE": "color", "DEFAULT": [1.0, 0.18, 0.55, 1.0],  "GROUP": "Color" },
    { "NAME": "backColor",    "LABEL": "Background Color",    "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0],    "GROUP": "Color" },
    { "NAME": "backAmt",      "LABEL": "Background Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "hueShift",     "LABEL": "Hue Shift",           "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",   "LABEL": "Saturation",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.15,  "GROUP": "Color" },
    { "NAME": "exposure",     "LABEL": "Exposure",            "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 2.5,  "DEFAULT": 1.15,  "GROUP": "Light" },
    { "NAME": "coreHeat",     "LABEL": "Core Heat",           "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bloom",        "LABEL": "Bloom",               "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "aberration",   "LABEL": "Chromatic Fringe",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.3,  "GROUP": "Finish" },
    { "NAME": "grain",        "LABEL": "Grain",               "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Finish" },
    { "NAME": "marchSteps",   "LABEL": "March Steps",         "TYPE": "float", "AUDIO": false, "MIN": 40.0, "MAX": 128.0, "DEFAULT": 80.0, "GROUP": "Finish" },
    { "NAME": "marchRes",     "LABEL": "March Resolution",    "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 1.0,  "DEFAULT": 0.7,  "GROUP": "Finish" },
    { "NAME": "audioReact",   "LABEL": "Audio React",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",      "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "obScene", "WIDTH": "floor($WIDTH*$marchRes)", "HEIGHT": "floor($HEIGHT*$marchRes)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Orbitals — CIDKID chemistry pack #7
//   pass 0  obScene — volumetric raymarch of |ψ|² inside a bounding sphere:
//                     exact hydrogen wavefunctions (two orbitals blended
//                     during a quantum jump), phase-sign colouring, flowing
//                     iso-shells (one-step-wide crossings), nucleus, photon
//                     ring, lab floor grid with a glow pool. a = coverage
//   pass 1  final   — dust specks, nucleus star, bloom, fringe, ACES, grain,
//                     HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define NORB 8

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float hash13(vec3 p3) { p3 = fract(p3 * 0.1031); p3 += dot(p3, p3.zyx + 31.32); return fract((p3.x + p3.y) * p3.z); }
float vnoise3(vec3 x) {
    vec3 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(hash13(p), hash13(p + vec3(1.0, 0.0, 0.0)), f.x), mix(hash13(p + vec3(0.0, 1.0, 0.0)), hash13(p + vec3(1.0, 1.0, 0.0)), f.x), f.y),
               mix(mix(hash13(p + vec3(0.0, 0.0, 1.0)), hash13(p + vec3(1.0, 0.0, 1.0)), f.x), mix(hash13(p + vec3(0.0, 1.0, 1.0)), hash13(p + vec3(1.0, 1.0, 1.0)), f.x), f.y), f.z);
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
mat3 rotX(float a) { float c = cos(a), s = sin(a); return mat3(1.0, 0.0, 0.0, 0.0, c, s, 0.0, -s, c); }
mat3 rotY(float a) { float c = cos(a), s = sin(a); return mat3(c, 0.0, -s, 0.0, 1.0, 0.0, s, 0.0, c); }
vec3 hueRot(vec3 c, float h) {
    if (h < 0.001) return c;
    float a = h * TAU; float cs = cos(a), sn = sin(a);
    vec3 k = vec3(0.57735);
    return c * cs + cross(k, c) * sn + k * dot(k, c) * (1.0 - cs);
}

// ── state ────────────────────────────────────────────────────────────────
float g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev, g_lens, g_size, g_floorY;
int   g_idA, g_idB;
float g_u, g_flash, g_bflash, g_ring, g_collapse, g_bgl, g_frame;
vec3  g_ro, g_uu, g_vv, g_ww, g_back, g_cP, g_cM;
mat3  g_rot, g_rotI;

vec3 camRay(vec2 uv) { return normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_lens); }
vec3 project(vec3 p) {
    vec3 d = p - g_ro;
    float z = dot(d, g_ww);
    vec2 s = vec2(dot(d, g_uu), dot(d, g_vv)) / max(z, 0.05) * g_lens;
    return vec3(s, z);
}

// ── hydrogen wavefunctions (unnormalised real forms, Bohr units) ─────────
// id: 0 1s · 1 2p_z · 2 2s · 3 3d_z² · 4 3d_xy · 5 4f_z³ · 6 sp³ hybrid · 7 4f_xyz
float orbExtent(int id) {   // radius (a0) where the density has died away
    if (id == 0) return 2.4;
    if (id == 1) return 11.0;
    if (id == 2) return 9.0;
    if (id == 3) return 22.0;
    if (id == 4) return 22.0;
    if (id == 5) return 46.0;
    if (id == 6) return 10.5;
    return 46.0;
}
float orbFrame(int id) {    // camera distance multiplier → every orbital fills ~65 % of the frame
    if (id == 0) return 0.96;
    if (id == 1) return 0.74;
    if (id == 2) return 1.36;
    if (id == 3) return 0.95;
    if (id == 4) return 1.17;
    if (id == 5) return 0.72;
    if (id == 6) return 0.90;
    return 0.85;
}
float orbNorm(int id) {     // 1 / visual peak of |ψ|² (outer lobe for s-type)
    if (id == 0) return 1.0;
    if (id == 1) return 1.0 / 0.5413;
    if (id == 2) return 1.0 / 0.32;
    if (id == 3) return 1.0 / 23.74;
    if (id == 4) return 1.0 / 5.934;
    if (id == 5) return 1.0 / 7401.5;
    if (id == 6) return 1.0 / 0.24;
    return 1.0 / 4100.0;
}
// signed wavefunction value at p (a0 units)
float psi(vec3 p, int id) {
    float r = max(length(p), 1e-4);
    vec3 u = p / r;
    if (id == 0) return exp(-r);
    if (id == 1) return r * exp(-r * 0.5) * u.z;
    if (id == 2) return (2.0 - r) * exp(-r * 0.5);
    if (id == 3) return r * r * exp(-r / 3.0) * (1.5 * u.z * u.z - 0.5);
    if (id == 4) return r * r * exp(-r / 3.0) * u.x * u.y;
    if (id == 5) return r * r * r * exp(-r * 0.25) * u.z * (2.5 * u.z * u.z - 1.5);
    if (id == 6) {
        // four sp³ lobes on the tetrahedron; alternate phase so the pair reads
        float s2 = (2.0 - r) * exp(-r * 0.5);
        float p2 = r * exp(-r * 0.5) * 1.7320508;
        float ca = max(dot(u, vec3( 0.57735,  0.57735,  0.57735)), 0.0);
        float cb = max(dot(u, vec3(-0.57735, -0.57735,  0.57735)), 0.0);
        float cc = max(dot(u, vec3(-0.57735,  0.57735, -0.57735)), 0.0);
        float cd = max(dot(u, vec3( 0.57735, -0.57735, -0.57735)), 0.0);
        float a = 0.5 * (s2 + p2 * ca) * ca * ca;
        float b = 0.5 * (s2 + p2 * cb) * cb * cb;
        float c = 0.5 * (s2 + p2 * cc) * cc * cc;
        float d = 0.5 * (s2 + p2 * cd) * cd * cd;
        // the four hybrids summed are a sphere (closed shell) — show each
        // lobe on its own: the strongest wins, phases alternate per lobe
        float m = max(max(a * a, b * b), max(c * c, d * d));
        float sgn = (max(a * a, d * d) >= max(b * b, c * c)) ? 1.0 : -1.0;
        return sgn * sqrt(m);
    }
    return r * r * r * exp(-r * 0.25) * u.x * u.y * u.z * 3.873;
}

// ── camera ───────────────────────────────────────────────────────────────
void setupCam() {
    float oa = g_t * 0.08 * orbitSpeed + 0.6;
    float dist = 4.3 * g_frame / zoom;
    float el = tilt + 0.08 * sin(g_t * 0.13 * orbitSpeed + 1.0);
    g_ro = vec3(sin(oa) * cos(el), sin(el), cos(oa) * cos(el)) * dist;
    vec3 ta = vec3(0.0, -0.05, 0.0);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    g_lens = 1.9;
}

// ── orbital timeline ─────────────────────────────────────────────────────
void setupOrbital() {
    // orbital 0 = auto cycle; 1..8 = pick (fractional part morphs to the next)
    float ph;
    if (orbital < 0.5) {
        float period = 9.0 / max(cycleSpeed, 0.02);
        float cyc = g_t / period;
        float idx = floor(cyc);
        float f = fract(cyc);
        // rest for 70 % of the period, then a 30 % quantum jump
        float u = smoothstep(0.70, 1.0, f);
        g_idA = int(mod(idx, float(NORB)));
        g_idB = int(mod(idx + 1.0, float(NORB)));
        g_u = u;
        ph = f;
    } else {
        float o = clamp(orbital - 1.0, 0.0, float(NORB) - 1.0);
        float idx = floor(o);
        g_idA = int(idx);
        g_idB = int(mod(idx + 1.0, float(NORB)));
        g_u = smoothstep(0.0, 1.0, fract(o));
        ph = 0.0;
    }
    // jump envelope: collapse toward the nucleus mid-transition, flash at the
    // core, ring expanding outward afterwards
    float w = sin(g_u * PI);
    g_collapse = 1.0 - 0.55 * w * w;
    g_flash = exp(-pow((g_u - 0.5) / 0.09, 2.0)) * flash;
    g_ring = (g_u > 0.5 && g_u < 0.999) ? (g_u - 0.5) * 2.0 : -1.0;
    // beats: a gentle volumetric glow only (≤ 0.25 of the jump flash, softened
    // envelope, no nucleus star / screen burst — those belong to the jumps)
    float bp = smoothstep(0.0, 1.0, g_beat);
    g_bflash = 0.12 * flash * g_ar * bp * bp;
    // the cloud spins about its axis; mids push the phase
    float spin = g_t * 0.25 * speed + 0.12 * g_ar * g_mid;
    g_rot = rotY(spin) * rotX(0.35 * sin(g_t * 0.07 * speed));
    g_rotI = rotX(-0.35 * sin(g_t * 0.07 * speed)) * rotY(-spin);
}

// density + phase (0 = minus, 1 = plus) at world p
vec2 cloud(vec3 pw) {
    vec3 p = g_rot * pw;
    p /= g_collapse;
    float kA = orbExtent(g_idA) * 0.78 / g_size;
    float kB = orbExtent(g_idB) * 0.78 / g_size;
    float a = psi(p * kA, g_idA);
    float b = psi(p * kB, g_idB);
    float dA = a * a * orbNorm(g_idA);
    float dB = b * b * orbNorm(g_idB);
    float d = mix(dA, dB, g_u);
    float ph = mix(step(0.0, a), step(0.0, b), g_u);
    return vec2(d, ph);
}

// visual density: gamma, tail suppression (deep black between lobes)
float shapeD(float x, float dens) {
    float d = pow(max(x, 0.0), 0.8) * dens;
    return d * d / (d + 0.12) * 1.12;
}
bool sphereBound(vec3 ro, vec3 rd, float rb, out float t0, out float t1) {
    float b = dot(ro, rd);
    float c = dot(ro, ro) - rb * rb;
    float h = b * b - c;
    if (h < 0.0) return false;
    h = sqrt(h);
    t0 = max(-b - h, 0.0); t1 = -b + h;
    return t1 > t0;
}

vec3 toneCol(float ph) { return mix(g_cM, g_cP, ph); }

vec3 floorCol(vec3 p, vec3 rd, float lightPool) {
    // faint lab grid, perspective fade, cloud glow pooling under the atom
    vec2 g = p.xz / (0.5 * g_size);
    vec2 f = abs(fract(g) - 0.5);
    float lineW = 0.02 + 0.06 * length(p.xz) * 0.15;
    float line = 1.0 - smoothstep(0.0, lineW, min(f.x, f.y));
    float fade = exp(-length(p.xz) * 0.55 / g_size);
    vec3 base = g_back;
    vec3 c = base;
    float lineAmt = 0.28 * floorAmt * fade * line;
    c += mix(vec3(0.5, 0.6, 0.75), vec3(1.0), 0.3) * lineAmt * mix(1.0, -0.6, g_bgl);
    // glow pool: colour of the cloud, hottest right under the nucleus
    vec3 pool = (0.5 * g_cP + 0.5 * g_cM) * lightPool * exp(-dot(p.xz, p.xz) * 1.3 / (g_size * g_size)) * 0.35 * floorAmt;
    c += pool * (1.0 - g_bgl);
    return c;
}

vec4 renderScene(vec2 uv, vec2 fragCoord) {
    vec3 rd = camRay(uv);
    vec3 ro = g_ro;
    float rb = g_size * 1.28 + 0.1;
    // backdrop
    vec3 bg = g_back * mix(1.0, 0.75 + 0.35 * smoothstep(-0.3, 0.6, rd.y), g_bgl);
    float tFloor = 1e9;
    if (floorAmt > 0.001 && rd.y < -0.0005) tFloor = (g_floorY - ro.y) / rd.y;
    float t0, t1;
    vec3 acc = vec3(0.0);
    vec3 T = vec3(1.0);
    float cover = 0.0;
    float poolL = 0.4 + 0.6 * density;
    if (sphereBound(ro, rd, rb, t0, t1)) {
        t1 = min(t1, tFloor);
        int steps = int(marchSteps);
        float dt = (t1 - t0) / float(steps);
        float jit = fract(52.9829189 * fract(dot(fragCoord, vec2(0.06711056, 0.00583715))));
        float swell = 1.0 + 0.22 * g_ar * g_bass;
        float dens = density * swell;
        // flowing iso-levels: four shells sliding outward (density falls with r)
        float fl = g_t * 0.12 * waveSpeed;
        float L0 = 0.05 + 0.80 * fract(0.00 - fl);
        float L1 = 0.05 + 0.80 * fract(0.25 - fl);
        float L2 = 0.05 + 0.80 * fract(0.50 - fl);
        float L3 = 0.05 + 0.80 * fract(0.75 - fl);
        float prevD = -1.0;
        float ringR = g_ring * rb * 1.05;
        float ringW = 0.05 * g_size;
        float ringI = (g_ring >= 0.0) ? (1.0 - g_ring) * (1.0 - g_ring) * flash * 2.5 : 0.0;
        for (int i = 0; i < 128; i++) {
            if (i >= steps) break;
            float t = t0 + (float(i) + jit) * dt;
            if (t > t1) break;
            vec3 p = ro + rd * t;
            vec2 cd = cloud(p);
            float d = shapeD(cd.x, dens);
            float r = length(p);
            // quantum speckle: fine drifting grain through the cloud
            float spk = vnoise3(p * (15.0 / g_size) + vec3(0.0, g_t * 0.35, g_t * 0.2));
            d *= 0.84 + 0.32 * spk;
            // self-shadow toward the key light → lobes read as lit volumes
            vec3 Ld = normalize(vec3(-0.55, 0.8, 0.3));
            vec2 cs = cloud(p + Ld * 0.11 * g_size);
            float shd = exp(-shapeD(cs.x, dens) * 2.6);
            float lit = 0.28 + 0.72 * shd;
            // probability shells: one-step-wide crossings of the iso levels
            float sh = 0.0;
            float dS = d / swell;
            if (prevD >= 0.0 && shells > 0.001) {
                float slope = max(abs(dS - prevD), 0.004);
                float k0 = abs(dS - L0) / slope, k1 = abs(dS - L1) / slope, k2 = abs(dS - L2) / slope, k3 = abs(dS - L3) / slope;
                sh = smoothstep(0.9, 0.0, k0) * (0.6 + 0.4 * L0) + smoothstep(0.9, 0.0, k1) * (0.6 + 0.4 * L1)
                   + smoothstep(0.9, 0.0, k2) * (0.6 + 0.4 * L2) + smoothstep(0.9, 0.0, k3) * (0.6 + 0.4 * L3);
                sh *= smoothstep(0.02, 0.08, dS) * shells;
            }
            prevD = dS;
            vec3 tone = toneCol(cd.y);
            // hot core where the density is highest
            vec3 em = tone * d * 1.4 * lit;
            // hot saturated core down each lobe's axis (density² term), going white at the peak
            vec3 toneSat = clamp(tone * 1.25 - 0.1, 0.0, 1.0);
            em += mix(toneSat, vec3(1.0, 0.98, 0.95), smoothstep(0.9, 1.6, d)) * d * d * 1.0 * coreHeat * (0.6 + 0.4 * lit);
            em += mix(tone, vec3(1.0), 0.3) * sh * 3.4 * (0.55 + 0.45 * lit);
            // nucleus
            float nuc = exp(-r * r * 900.0 / (g_size * g_size)) * 40.0 * nucleus * (1.0 + 3.0 * g_flash);
            em += vec3(1.0, 0.95, 0.85) * nuc;
            // photon flash: a burst at the centre + expanding ring shell
            float fla = (g_flash + g_bflash) * exp(-r * r * 5.0 / (g_size * g_size)) * 5.0;
            em += mix(tone, vec3(1.0), 0.7) * fla;
            if (ringI > 0.0) {
                float rg = exp(-pow((r - ringR) / ringW, 2.0)) * ringI;
                em += mix(tone, vec3(1.0, 0.95, 0.9), 0.6) * rg;
            }
            float sigma = (d * absorb * 2.3 + nuc * 0.02) * dt;
            // coloured absorption on pale backdrops: the cloud behaves like dyed glass
            vec3 sigRGB = vec3(sigma) * mix(vec3(1.0), (1.0 - tone * 0.85), g_bgl);
            vec3 Ts = exp(-sigRGB);
            acc += T * em * dt * (1.0 - g_bgl * 0.45);
            T *= Ts;
            cover += (1.0 - exp(-sigma)) * (1.0 - cover);
            if (T.g < 0.01 && T.r < 0.01 && T.b < 0.01) break;
        }
    }
    vec3 behind = bg;
    if (tFloor < 1e8) {
        vec3 pf = ro + rd * tFloor;
        behind = floorCol(pf, rd, poolL * (1.0 + 2.0 * g_flash));
        // distance fog toward the backdrop
        behind = mix(behind, bg, smoothstep(2.0, 9.0, tFloor - 3.0 / zoom));
    }
    vec3 col = behind * T + acc;
    float a = max(cover, smoothstep(0.02, 0.3, lum(acc)));
    return vec4(col, clamp(a, 0.0, 1.0));
}

// ── pass 1 helpers: electron dust specks ─────────────────────────────────
vec3 dustPass(vec2 uv) {
    vec3 col = vec3(0.0);
    float ext = g_size * 1.05 * g_frame;
    float zAtom = length(g_ro);
    for (int i = 0; i < 44; i++) {
        float fi = float(i);
        float h1 = hash11(fi * 3.1 + 1.0), h2 = hash11(fi * 7.3 + 2.0), h3 = hash11(fi * 5.7 + 3.0), h4 = hash11(fi * 2.9 + 4.0);
        // tilted orbits at random radii, each its own inclination and rate
        float rr = ext * (0.35 + 0.75 * h1);
        float rate = (0.25 + 0.5 * h2) * speed * (h3 > 0.5 ? 1.0 : -1.0);
        float ang = h4 * TAU + g_t * rate + 0.15 * g_ar * g_mid;
        vec3 q = vec3(cos(ang) * rr, 0.0, sin(ang) * rr);
        q = rotX(h2 * 2.4 - 1.2) * rotY(h3 * TAU) * q;
        q *= g_collapse;
        vec3 pr = project(q);
        if (pr.z < 0.3) continue;
        vec2 d = uv - pr.xy;
        float dd = dot(d, d);
        float sz = 0.0024 * g_lens / pr.z * 3.0;
        if (dd > sz * sz * 60.0) continue;
        float tw = 0.4 + 0.6 * pow(0.5 + 0.5 * sin(g_t * (1.5 + 3.0 * h1) + h4 * TAU), 2.0) * (1.0 + 0.8 * g_ar * g_high);
        float spot = exp(-dd / (sz * sz)) * 1.4 + exp(-dd / (sz * sz * 9.0)) * 0.12;
        vec3 c = mix(toneCol(step(0.5, h2)), vec3(1.0), 0.5);
        // rough depth sort: a speck behind the atom is dimmed by the cloud in front of it
        vec2 sst = pr.xy * vec2(R.y / R.x, 1.0) + 0.5;
        float cov = texture2D(obScene, clamp(sst, 0.0, 1.0)).a;
        float behind = smoothstep(-0.15 * g_size, 0.15 * g_size, pr.z - zAtom);
        float occl = 1.0 - 0.92 * cov * behind;
        col += c * spot * tw * dust * 1.7 * occl;
    }
    return col;
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    g_ar = audioReact;
    g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_back = backColor.rgb * backAmt;
    g_bgl = smoothstep(0.2, 0.55, lum(g_back));
    g_cP = hueRot(colorPlus.rgb, hueShift);
    g_cM = hueRot(colorMinus.rgb, hueShift);
    g_size = size;
    g_floorY = -g_size * 1.22;
    setupOrbital();
    g_frame = mix(orbFrame(g_idA), orbFrame(g_idB), smoothstep(0.0, 1.0, g_u));
    setupCam();

    if (PASSINDEX == 0) {
        vec4 sc = renderScene(uv, fragCoord);
        gl_FragColor = vec4(max(sc.rgb, 0.0), sc.a);
    } else {
        vec2 st = fragCoord / R;
        vec2 asp = vec2(R.y / R.x, 1.0);
        vec2 dir = (st - 0.5) * 2.0;
        float ab = aberration * 0.0028 * (1.0 + 0.8 * g_flash);
        vec2 o = dir * ab;
        vec4 c0 = texture2D(obScene, st);
        vec3 col = vec3(texture2D(obScene, st + o).r, c0.g, texture2D(obScene, st - o).b);
        // dust specks + nucleus star
        vec3 dustC = dustPass(uv);
        col += dustC;
        vec3 pn = project(vec3(0.0));
        vec2 dn = uv - pn.xy;
        float star = exp(-dot(dn, dn) * 9000.0) * 2.0 + exp(-dot(dn, dn) * 500.0) * 0.25;
        float spikes = (exp(-abs(dn.x) * 120.0) * exp(-abs(dn.y) * 1800.0) + exp(-abs(dn.y) * 120.0) * exp(-abs(dn.x) * 1800.0)) * 0.6;
        col += vec3(1.0, 0.96, 0.88) * (star * (1.0 + 3.0 * g_flash) + spikes * (0.5 + 2.5 * g_flash)) * nucleus * (1.0 - 0.7 * g_bgl);
        // photon burst: a soft radial flash over the whole atom
        col += mix(0.5 * (g_cP + g_cM), vec3(1.0), 0.6) * g_flash * exp(-dot(dn, dn) * 6.0) * 0.5 * (1.0 - g_bgl);
        // bloom (threshold above the backdrop luma)
        vec3 bl = vec3(0.0);
        float rad = 0.035;
        float thr = 0.9 + 0.5 * g_bgl;
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + hash12(fragCoord) * 6.28;
            float rr = sqrt((fi + 0.5) / 16.0) * rad;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            vec3 s = texture2D(obScene, st + of).rgb;
            bl += max(s - thr, 0.0) * (1.0 - rr / rad);
        }
        bl /= 16.0;
        col += bl * bloom * 1.3;
        col *= exposure * mix(1.0, 0.94 + 0.12 * g_lev, g_ar);
        col = aces(col);
        // saturation + S-curve + HD finisher
        float l = lum(col);
        col = clamp(mix(vec3(l), col, saturation), 0.0, 1.0);
        float gl = lum(col);
        float g1 = (hash12(fragCoord + fract(g_t) * 61.0) - 0.5) * 0.04;
        col += g1 * grain * (0.3 + 0.7 * gl);
        col *= 1.0 - 0.10 * smoothstep(0.5, 1.5, length(uv));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.5);
        float l2 = lum(col);
        col = clamp(mix(vec3(l2), col, 1.22), 0.0, 1.0);
        float alphaOut = transparentBg ? max(c0.a, smoothstep(0.02, 0.2, lum(dustC + bl))) : mix(1.0, backColor.a, backAmt * step(0.001, 1.0 - backColor.a));
        gl_FragColor = vec4(col, alphaOut);
    }
}
