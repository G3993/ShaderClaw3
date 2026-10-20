/*{
  "DESCRIPTION": "Vortex Street — a real Lattice-Boltzmann fluid (D2Q9, nine particle distributions streaming and colliding on a grid, four solver steps every frame) flowing past bobbing cylinders and pitching plates in a dark water tunnel. Every body sheds a genuine Kármán vortex street: alternating whirlpools peel off, roll up and march downstream, and the wakes of staggered bodies collide, braid and break into turbulence. Each body bleeds its own ink — hot magenta, electric cyan, molten gold, acid lime — and where two wakes shear together the pigments blend into vermilion, indigo and green along the layers. Dye has real range: hair-fine wisps fade into black, translucent veils glow faintly, dense ink rises into glossy relief and the vortex eyes burn white-hot. A rake of fine streaklines enters from the left, bending around the bodies and curling into every vortex, over a dark tank floor with a faint caustic web and schlieren shimmer. Bass pushes the current harder, beats squeeze puffs of ink from every body, mids swing the plates, highs sparkle the gloss.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #5",
  "CATEGORIES": ["Generator", "Fluid", "Simulation", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "flowSpeed",   "LABEL": "Current",           "TYPE": "float", "MIN": 0.0,  "MAX": 1.6,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "drift",       "LABEL": "Body Motion",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7,  "GROUP": "Speed" },
    { "NAME": "colorCycle",  "LABEL": "Color Cycle",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Speed" },
    { "NAME": "viscosity",   "LABEL": "Viscosity",         "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Flow" },
    { "NAME": "bodies",      "LABEL": "Bodies",            "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 6.0, "DEFAULT": 4.0,  "GROUP": "Flow" },
    { "NAME": "bodySize",    "LABEL": "Body Size",         "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.25, "GROUP": "Flow" },
    { "NAME": "plateMix",    "LABEL": "Plates vs Cylinders","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5,  "GROUP": "Flow" },
    { "NAME": "simScale",    "LABEL": "Sim Resolution",    "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0,  "GROUP": "Flow" },
    { "NAME": "injection",   "LABEL": "Ink Flow",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Ink" },
    { "NAME": "rake",        "LABEL": "Streaklines",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.6,  "GROUP": "Ink" },
    { "NAME": "rakeSpacing", "LABEL": "Streakline Spacing","TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Ink" },
    { "NAME": "striation",   "LABEL": "Ink Banding",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.75, "GROUP": "Ink" },
    { "NAME": "fade",        "LABEL": "Ink Fade",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.08, "GROUP": "Ink" },
    { "NAME": "dyeRes",      "LABEL": "Ink Resolution",    "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Ink" },
    { "NAME": "ink1",        "LABEL": "Ink 1",             "TYPE": "color", "DEFAULT": [1.0, 0.1, 0.42, 1.0],  "GROUP": "Color" },
    { "NAME": "ink2",        "LABEL": "Ink 2",             "TYPE": "color", "DEFAULT": [0.04, 0.72, 1.0, 1.0], "GROUP": "Color" },
    { "NAME": "ink3",        "LABEL": "Ink 3",             "TYPE": "color", "DEFAULT": [1.0, 0.6, 0.06, 1.0],  "GROUP": "Color" },
    { "NAME": "ink4",        "LABEL": "Ink 4",             "TYPE": "color", "DEFAULT": [0.5, 1.0, 0.08, 1.0],  "GROUP": "Color" },
    { "NAME": "rakeColor",   "LABEL": "Streakline Color",  "TYPE": "color", "DEFAULT": [0.6, 0.74, 1.0, 1.0], "GROUP": "Color" },
    { "NAME": "backColor",   "LABEL": "Water Color",       "TYPE": "color", "DEFAULT": [0.004, 0.007, 0.016, 1.0], "GROUP": "Color" },
    { "NAME": "vortTint",    "LABEL": "Spin Tint",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "relief",      "LABEL": "Ink Relief",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "specAmt",     "LABEL": "Gloss",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "edgeAmt",     "LABEL": "Filament Edges",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Light" },
    { "NAME": "vortGlow",    "LABEL": "Vortex Glow",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Light" },
    { "NAME": "schlieren",   "LABEL": "Water Shimmer",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35, "GROUP": "Light" },
    { "NAME": "caustics",    "LABEL": "Caustics",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7,  "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "exposure",    "LABEL": "Exposure",          "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0,  "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "lb0", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lb1", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lb2", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lb3", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lb4", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lb5", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lb6", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lb7", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lb8", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lb9", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lbA", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "lbB", "PERSISTENT": true, "WIDTH": "floor(480*$simScale)", "HEIGHT": "floor(480*$simScale*$HEIGHT/$WIDTH)" },
    { "TARGET": "dyeBuf", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$dyeRes),2560)", "HEIGHT": "min(floor($HEIGHT*$dyeRes),1440)" },
    { "TARGET": "dyeAux", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$dyeRes),2560)", "HEIGHT": "min(floor($HEIGHT*$dyeRes),1440)" },
    { "TARGET": "glowBuf", "WIDTH": "floor($WIDTH/4)", "HEIGHT": "floor($HEIGHT/4)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Vortex Street — FRONTIER pack #5
//   passes 0-11  lb0..lbB : Lattice-Boltzmann D2Q9, BGK collision with a
//                Smagorinsky eddy viscosity. 4 solver steps per frame, each
//                step = 3 buffers holding the 9 post-collision distributions
//                (stored as f - w for half-float precision):
//                  X = (f0 f1 f2 f3)   Y = (f4 f5 f6 f7)   Z = (f8 ux uy 0)
//                pull streaming, moving-wall bounce-back on analytic bodies,
//                equilibrium inflow on the left, sponge outflow on the right,
//                periodic top/bottom. Every pass of a step recomputes the full
//                step (cheap at 480 cells) and writes its third.
//   pass 12      dyeBuf : 4 pigment densities, RK2 back-trace through the
//                LBM velocity, clamped Catmull-Rom resample (crisp),
//                injection at body surfaces (one pigment per body)
//   pass 13      dyeAux : streakline ink from the inlet rake (same advection)
//   pass 14      glowBuf : quarter-res emission of the thick ink for bloom
//   pass 15      final : density curve (wisp → veil → glossy core), pigment
//                mixing along shear layers, relief + gloss on thick ink,
//                vortex glow, caustic/schlieren water, glass bodies, bloom,
//                ACES, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define SPF  4.0

float knee(float x, float lo, float hi) { return smoothstep(lo, hi, x); }
float bassP()  { return pow(knee(audioBass, 0.05, 0.85), 1.4); }
float midP()   { return pow(knee(audioMid,  0.06, 0.85), 1.2); }
float highP()  { return pow(knee(audioHigh, 0.10, 0.90), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

// ── shared ───────────────────────────────────────────────────────────────
float g_t, g_ar, g_hy, g_U;

float bodyTime() { return TIME * (0.15 + 0.85 * drift); }

// inflow speed in lattice units / step
float inflowU() {
    float u = 0.085 * flowSpeed * (1.0 + 0.4 * bassP() * audioReact);
    return min(u, 0.16);
}

// body k frame: centre (width units, y in [0, hy]), angle, radius, half length
void bodyFrame(float kf, float t, out vec2 c, out float ang, out float r, out float hl) {
    float h0 = hash11(kf * 7.31 + 1.3);
    float h1 = hash11(kf * 3.17 + 8.9);
    c.x = 0.13 + 0.088 * kf + 0.012 * sin(t * 0.21 + kf * 1.7);
    c.y = g_hy * (0.2 + 0.6 * fract(0.37 + kf * 0.618) + 0.13 * sin(t * (0.11 + 0.03 * kf) + kf * 2.1) + 0.03 * sin(t * (0.37 + 0.05 * kf) + kf));
    r = 0.019 * bodySize * (0.9 + 0.3 * h0);
    float plate = step(h1, plateMix);
    hl = plate * r * 1.7;
    r *= mix(1.0, 0.42, plate);
    ang = plate * (0.35 * sin(kf * 1.9) + 0.55 * sin(t * (0.42 + 0.09 * kf) + kf) * (1.0 + 0.6 * midP() * audioReact));
}

vec2 rot2(vec2 p, float a) { float s = sin(a), co = cos(a); return vec2(co * p.x - s * p.y, s * p.x + co * p.y); }

float bodySD(vec2 p, vec2 c, float ang, float r, float hl) {
    vec2 q = rot2(p - c, -ang);
    q.x -= clamp(q.x, -hl, hl);
    return length(q) - r;
}

// nearest body: x = signed distance, y = body index
vec2 bodiesSD(vec2 p, float t) {
    vec2 best = vec2(1e3, 0.0);
    for (int k = 0; k < 6; k++) {
        float kf = float(k);
        if (kf >= floor(bodies + 0.5)) break;
        vec2 c; float ang, r, hl;
        bodyFrame(kf, t, c, ang, r, hl);
        float d = bodySD(p, c, ang, r, hl);
        if (d < best.x) best = vec2(d, kf);
    }
    return best;
}

// velocity of the body surface at p (width units / second)
vec2 bodyVel(vec2 p, float kf, float t) {
    vec2 c0; float a0, r0, h0;
    vec2 c1; float a1, r1, h1;
    const float eps = 0.02;
    bodyFrame(kf, t, c0, a0, r0, h0);
    bodyFrame(kf, t + eps * (0.15 + 0.85 * drift), c1, a1, r1, h1);
    vec2 q = rot2(p - c0, -a0);
    vec2 p1 = c1 + rot2(q, a1);
    return (p1 - p) / eps;
}

vec4 pigSel(float kf) {
    float ch = mod(kf, 4.0);
    return ch < 0.5 ? vec4(1, 0, 0, 0) : (ch < 1.5 ? vec4(0, 1, 0, 0) : (ch < 2.5 ? vec4(0, 0, 1, 0) : vec4(0, 0, 0, 1)));
}

// ── seeded vortex street (frame 0) ───────────────────────────────────────
// irregular: each vortex jittered in spacing, offset, size and strength,
// growing and weakening downstream, some missing
void vortexSeed(float kf, float mf, vec2 c, out vec2 v, out float sz, out float str, out float up) {
    float h1 = hash11(kf * 17.3 + mf * 5.1), h2 = hash11(kf * 9.7 + mf * 11.3 + 2.0), h3 = hash11(kf * 4.1 + mf * 7.9 + 5.0);
    up = mod(mf + step(0.5, hash11(kf * 2.3)), 2.0) < 0.5 ? 1.0 : -1.0;
    float x = 0.045 + 0.06 * mf * (0.85 + 0.3 * hash11(kf * 3.3 + 1.0)) + 0.018 * (h1 - 0.5);
    v = c + vec2(x, (up * 0.017 + 0.012 * (h2 - 0.5) + 0.006 * mf * (h3 - 0.5)) * bodySize);
    sz = (0.75 + 0.18 * mf + 0.35 * h3) * bodySize;
    str = (0.65 + 0.6 * h2) * (1.0 - 0.1 * mf) * step(0.18, h1 + 0.25 * (4.0 - mf) * 0.25);
}
vec2 seedVel(vec2 p, float t, float U) {
    vec2 u = vec2(U, 0.0);
    for (int k = 0; k < 6; k++) {
        float kf = float(k);
        if (kf >= floor(bodies + 0.5)) break;
        vec2 c; float ang, r, hl;
        bodyFrame(kf, t, c, ang, r, hl);
        for (int m = 0; m < 5; m++) {
            float mf = float(m);
            vec2 v; float sz, str, up;
            vortexSeed(kf, mf, c, v, sz, str, up);
            vec2 d = p - v;
            float rr = length(d) + 1e-5;
            float rc = 0.011 * sz;
            float s = -up;
            float mag = str * 0.11 * U / 0.085 * (1.0 - exp(-rr * rr / (rc * rc))) / (rr / rc);
            mag *= exp(-rr * rr / (16.0 * rc * rc)) * smoothstep(0.03, 0.12, p.x);
            u += s * mag * vec2(-d.y, d.x) / rr;
        }
    }
    float m = length(u);
    return m > 0.18 ? u * 0.18 / m : u;
}

vec4 seedDye(vec2 p, float t) {
    vec4 dye = vec4(0.0);
    for (int k = 0; k < 6; k++) {
        float kf = float(k);
        if (kf >= floor(bodies + 0.5)) break;
        vec2 c; float ang, r, hl;
        bodyFrame(kf, t, c, ang, r, hl);
        vec4 sel = pigSel(kf);
        for (int m = 0; m < 5; m++) {
            float mf = float(m);
            vec2 v; float vsz, str, up;
            vortexSeed(kf, mf, c, v, vsz, str, up);
            float hv = hash11(kf * 13.1 + mf * 3.7);
            float sk = 0.75 + 0.4 * hv;
            vec2 d = (p - v) * vec2(sk, 1.0 / sk);
            float rr = length(d);
            float th = atan(d.y, d.x);
            float sz = 0.028 * vsz;
            float env = exp(-pow(rr / sz, 2.0));
            float arms = 0.5 + 0.5 * cos((1.0 + step(0.5, hv)) * th - up * rr * (220.0 + 200.0 * hv) / vsz + hv * 6.28);
            // break the arms into fragments so it reads as torn ink, not a printed spiral
            float frag = smoothstep(-0.2, 0.6, sin(th * (2.0 + floor(hv * 3.0)) + rr * 90.0 * (0.5 + hv) + hv * 11.0));
            dye += sel * env * (smoothstep(0.55, 0.98, arms) * mix(0.35, 1.0, frag) + 0.2 * smoothstep(0.2, 0.5, arms)) * 1.3 * min(str * 1.3, 1.0);
        }
        // the band from the body to its first vortex
        vec2 q = p - c;
        float band = exp(-pow(q.y / (0.6 * r + 0.004), 2.0)) * smoothstep(0.0, 0.01, q.x) * smoothstep(0.08, 0.03, q.x);
        dye += sel * band;
    }
    return dye;
}

float seedRake(vec2 p, float t) {
    float sp = 0.0175 * rakeSpacing;
    float line = abs(fract(p.y / sp) - 0.5) * sp * R.x;
    return (1.0 - smoothstep(0.45, 1.1, line)) * rake * smoothstep(0.0, 0.02, bodiesSD(p, t).x);
}

// ═════════════════════════════════════════════════════════════════════════
//  LATTICE BOLTZMANN  (passes 0..11)
// ═════════════════════════════════════════════════════════════════════════
const float W0 = 0.444444444;
const float W1 = 0.111111111;
const float W2 = 0.027777778;

// previous step's buffer j (0 = X, 1 = Y, 2 = Z) at integer cell c
vec4 prevBuf(int j, vec2 c) {
    c.x = clamp(c.x, 0.0, R.x - 1.0);
    c.y = mod(c.y, R.y);
    vec2 uv = (c + 0.5) / R;
    int s = PASSINDEX / 3;
    if (s == 0) { if (j == 0) return texture2D(lb9, uv); if (j == 1) return texture2D(lbA, uv); return texture2D(lbB, uv); }
    if (s == 1) { if (j == 0) return texture2D(lb0, uv); if (j == 1) return texture2D(lb1, uv); return texture2D(lb2, uv); }
    if (s == 2) { if (j == 0) return texture2D(lb3, uv); if (j == 1) return texture2D(lb4, uv); return texture2D(lb5, uv); }
    if (j == 0) return texture2D(lb6, uv); if (j == 1) return texture2D(lb7, uv); return texture2D(lb8, uv);
}

float feq(float w, vec2 e, vec2 u, float rho) {
    float cu = dot(e, u);
    return w * rho * (1.0 + 3.0 * cu + 4.5 * cu * cu - 1.5 * dot(u, u));
}

vec4 lbmPass() {
    vec2 cell = floor(gl_FragCoord.xy);
    vec2 p = (cell + 0.5) / R.x;        // width units
    int comp = PASSINDEX - 3 * (PASSINDEX / 3);
    float U = g_U;
    // body time for this sub-step (sub-steps share the frame's TIME)
    float t = bodyTime();

    float f0, f1, f2, f3, f4, f5, f6, f7, f8;

    if (FRAMEINDEX < 1) {
        vec2 u = seedVel(p, t, U);
        f0 = feq(W0, vec2( 0, 0), u, 1.0);
        f1 = feq(W1, vec2( 1, 0), u, 1.0); f2 = feq(W1, vec2(0, 1), u, 1.0);
        f3 = feq(W1, vec2(-1, 0), u, 1.0); f4 = feq(W1, vec2(0,-1), u, 1.0);
        f5 = feq(W2, vec2( 1, 1), u, 1.0); f6 = feq(W2, vec2(-1, 1), u, 1.0);
        f7 = feq(W2, vec2(-1,-1), u, 1.0); f8 = feq(W2, vec2( 1,-1), u, 1.0);
        vec2 sd0 = bodiesSD(p, t);
        if (sd0.x < 0.0) u = vec2(0.0);
        if (comp == 0) return vec4(f0 - W0, f1 - W1, f2 - W1, f3 - W1);
        if (comp == 1) return vec4(f4 - W1, f5 - W2, f6 - W2, f7 - W2);
        return vec4(f8 - W2, u, 0.0);
    }

    // ── pull streaming
    f0 = prevBuf(0, cell).x                  + W0;
    f1 = prevBuf(0, cell - vec2( 1.0, 0.0)).y + W1;
    f2 = prevBuf(0, cell - vec2( 0.0, 1.0)).z + W1;
    f3 = prevBuf(0, cell + vec2( 1.0, 0.0)).w + W1;
    f4 = prevBuf(1, cell + vec2( 0.0, 1.0)).x + W1;
    f5 = prevBuf(1, cell - vec2( 1.0, 1.0)).y + W2;
    f6 = prevBuf(1, cell + vec2( 1.0,-1.0)).z + W2;
    f7 = prevBuf(1, cell + vec2( 1.0, 1.0)).w + W2;
    f8 = prevBuf(2, cell + vec2(-1.0, 1.0)).x + W2;

    // ── solid bodies: inside → rest state; next to → moving bounce-back
    vec2 sd = bodiesSD(p, t);
    float cellW = 1.0 / R.x;
    if (sd.x < 0.0) {
        if (comp == 0) return vec4(0.0);
        if (comp == 1) return vec4(0.0);
        vec2 bv = bodyVel(p, sd.y, t) * R.x / (60.0 * SPF);
        return vec4(0.0, bv, 0.0);
    }
    if (sd.x < 2.0 * cellW) {
        vec4 A = prevBuf(0, cell), B = prevBuf(1, cell), C = prevBuf(2, cell);
        float p0 = A.x + W0, p1 = A.y + W1, p2 = A.z + W1, p3 = A.w + W1;
        float p4 = B.x + W1, p5 = B.y + W2, p6 = B.z + W2, p7 = B.w + W2, p8 = C.x + W2;
        vec2 uw = bodyVel(p, sd.y, t) * R.x / (60.0 * SPF);
        vec2 e;
        e = vec2( 1, 0); if (bodiesSD(p - e * cellW, t).x < 0.0) f1 = p3 + 6.0 * W1 * dot(e, uw);
        e = vec2( 0, 1); if (bodiesSD(p - e * cellW, t).x < 0.0) f2 = p4 + 6.0 * W1 * dot(e, uw);
        e = vec2(-1, 0); if (bodiesSD(p - e * cellW, t).x < 0.0) f3 = p1 + 6.0 * W1 * dot(e, uw);
        e = vec2( 0,-1); if (bodiesSD(p - e * cellW, t).x < 0.0) f4 = p2 + 6.0 * W1 * dot(e, uw);
        e = vec2( 1, 1); if (bodiesSD(p - e * cellW, t).x < 0.0) f5 = p7 + 6.0 * W2 * dot(e, uw);
        e = vec2(-1, 1); if (bodiesSD(p - e * cellW, t).x < 0.0) f6 = p8 + 6.0 * W2 * dot(e, uw);
        e = vec2(-1,-1); if (bodiesSD(p - e * cellW, t).x < 0.0) f7 = p5 + 6.0 * W2 * dot(e, uw);
        e = vec2( 1,-1); if (bodiesSD(p - e * cellW, t).x < 0.0) f8 = p6 + 6.0 * W2 * dot(e, uw);
    }

    // ── macroscopic
    float rho = f0 + f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8;
    vec2 u = vec2(f1 - f3 + f5 - f6 - f7 + f8, f2 - f4 + f5 + f6 - f7 - f8) / max(rho, 0.2);
    bool bad = !(rho > 0.4 && rho < 2.5) || !(abs(u.x) < 0.5 && abs(u.y) < 0.5);
    float um = length(u);
    if (um > 0.22) u *= 0.22 / um;

    // inflow column + gentle transverse wobble so wakes never lock symmetric
    float xs = cell.x / R.x;
    vec2 uIn = vec2(U, U * 0.04 * sin(TIME * 0.7 + p.y * 9.0));
    float sponge = smoothstep(0.86, 1.0, xs) * 0.25 + (cell.x < 1.5 ? 1.0 : 0.0);
    // warm-up: the seeded street is not pressure-consistent; damp the startup
    // acoustic wave (pressure relaxes, velocity kept) and widen the inlet sponge
    float warm = 1.0 - smoothstep(0.0, 1.6, TIME);
    sponge += warm * 0.35 * (1.0 - smoothstep(0.0, 0.1, xs));
    if (warm > 0.0) rho = mix(rho, 1.0, 0.25 * warm);
    if (bad) { rho = 1.0; u = uIn; sponge = 1.0; }

    // ── collision: BGK + Smagorinsky
    float e0 = feq(W0, vec2( 0, 0), u, rho);
    float e1 = feq(W1, vec2( 1, 0), u, rho), e2 = feq(W1, vec2(0, 1), u, rho);
    float e3 = feq(W1, vec2(-1, 0), u, rho), e4 = feq(W1, vec2(0,-1), u, rho);
    float e5 = feq(W2, vec2( 1, 1), u, rho), e6 = feq(W2, vec2(-1, 1), u, rho);
    float e7 = feq(W2, vec2(-1,-1), u, rho), e8 = feq(W2, vec2( 1,-1), u, rho);
    float n1 = f1 - e1, n2 = f2 - e2, n3 = f3 - e3, n4 = f4 - e4;
    float n5 = f5 - e5, n6 = f6 - e6, n7 = f7 - e7, n8 = f8 - e8;
    float pxx = n1 + n3 + n5 + n6 + n7 + n8;
    float pyy = n2 + n4 + n5 + n6 + n7 + n8;
    float pxy = n5 - n6 + n7 - n8;
    float Q = sqrt(pxx * pxx + pyy * pyy + 2.0 * pxy * pxy);
    float nu = mix(0.0035, 0.03, viscosity * viscosity);
    float tau0 = 0.5 + 3.0 * nu;
    float tau = 0.5 * (tau0 + sqrt(tau0 * tau0 + 18.0 * 1.41421 * 0.0225 * Q / rho));
    float om = 1.0 / tau;
    f0 += (e0 - f0) * om; f1 += (e1 - f1) * om; f2 += (e2 - f2) * om;
    f3 += (e3 - f3) * om; f4 += (e4 - f4) * om; f5 += (e5 - f5) * om;
    f6 += (e6 - f6) * om; f7 += (e7 - f7) * om; f8 += (e8 - f8) * om;

    // ── inflow / sponge: relax toward the free stream
    if (sponge > 0.0) {
        float s = clamp(sponge, 0.0, 1.0);
        f0 = mix(f0, feq(W0, vec2( 0, 0), uIn, 1.0), s);
        f1 = mix(f1, feq(W1, vec2( 1, 0), uIn, 1.0), s);
        f2 = mix(f2, feq(W1, vec2( 0, 1), uIn, 1.0), s);
        f3 = mix(f3, feq(W1, vec2(-1, 0), uIn, 1.0), s);
        f4 = mix(f4, feq(W1, vec2( 0,-1), uIn, 1.0), s);
        f5 = mix(f5, feq(W2, vec2( 1, 1), uIn, 1.0), s);
        f6 = mix(f6, feq(W2, vec2(-1, 1), uIn, 1.0), s);
        f7 = mix(f7, feq(W2, vec2(-1,-1), uIn, 1.0), s);
        f8 = mix(f8, feq(W2, vec2( 1,-1), uIn, 1.0), s);
        u = mix(u, uIn, s);
    }

    if (comp == 0) return vec4(f0 - W0, f1 - W1, f2 - W1, f3 - W1);
    if (comp == 1) return vec4(f4 - W1, f5 - W2, f6 - W2, f7 - W2);
    return vec4(f8 - W2, u, 0.0);
}

// ═════════════════════════════════════════════════════════════════════════
//  INK  (pass 12)
// ═════════════════════════════════════════════════════════════════════════
vec2 simRes() {
    float w = floor(480.0 * simScale);
    return vec2(w, floor(w * R.y / R.x));
}
// lattice velocity at uv (bilinear over the last solver step)
vec2 latVel(vec2 uv) { return texture2D(lbB, uv).yz; }

// clamped Catmull-Rom: crisp resample without ringing
vec4 fetchDye(vec2 uv) { return PASSINDEX == 12 ? texture2D(dyeBuf, uv) : texture2D(dyeAux, uv); }

vec4 sampleDye(vec2 uv) {
    vec2 ts = R;
    vec2 sp = uv * ts - 0.5;
    vec2 tc = floor(sp) + 0.5;
    vec2 f = sp - (tc - 0.5);
    vec2 w0 = f * (-0.5 + f * (1.0 - 0.5 * f));
    vec2 w1 = 1.0 + f * f * (-2.5 + 1.5 * f);
    vec2 w2 = f * (0.5 + f * (2.0 - 1.5 * f));
    vec2 w3 = f * f * (-0.5 + 0.5 * f);
    vec2 w12 = w1 + w2;
    vec2 o12 = w2 / w12;
    vec2 t0 = (tc - 1.0) / ts, t3 = (tc + 2.0) / ts, t12 = (tc + o12) / ts;
    vec4 r = vec4(0.0);
    r += fetchDye(vec2(t12.x, t0.y))  * w12.x * w0.y;
    r += fetchDye(vec2(t0.x,  t12.y)) * w0.x  * w12.y;
    r += fetchDye(vec2(t12.x, t12.y)) * w12.x * w12.y;
    r += fetchDye(vec2(t3.x,  t12.y)) * w3.x  * w12.y;
    r += fetchDye(vec2(t12.x, t3.y))  * w12.x * w3.y;
    float wsum = w12.x * w0.y + w0.x * w12.y + w12.x * w12.y + w3.x * w12.y + w12.x * w3.y;
    r /= wsum;
    // neighbourhood clamp
    vec4 a = fetchDye((tc + vec2(0.0, 0.0)) / ts);
    vec4 b = fetchDye((tc + vec2(1.0, 0.0)) / ts);
    vec4 c = fetchDye((tc + vec2(0.0, 1.0)) / ts);
    vec4 d = fetchDye((tc + vec2(1.0, 1.0)) / ts);
    return clamp(r, min(min(a, b), min(c, d)), max(max(a, b), max(c, d)));
}

vec4 dyePass() {
    vec2 uv = gl_FragCoord.xy / R;
    vec2 p = uv * vec2(1.0, g_hy);
    float t = bodyTime();

    if (FRAMEINDEX < 1) return PASSINDEX == 12 ? seedDye(p, t) : vec4(seedRake(p, t), 0.0, 0.0, 0.0);

    vec2 sr = simRes();
    // displacement per frame in uv = lattice vel * steps / sim cells
    vec2 k1 = latVel(uv) * SPF / sr;
    vec2 k2 = latVel(uv - 0.5 * k1) * SPF / sr;
    vec2 src = uv - k2;
    vec4 dye = sampleDye(src);
    if (src.x < 0.0) dye = vec4(0.0);

    float dt = 1.0 / 60.0;
    // ink lingers longer downstream so the wakes fill the tank
    dye *= 1.0 - dt * mix(0.01, 0.35, fade) * mix(1.0, 0.35, uv.x);

    // inject at body surfaces (each body its own pigment), banded in time
    vec2 sd = bodiesSD(p, t);
    float px = 1.0 / R.x;
    float ar = audioReact;
    float kf = sd.y;
    float ring = smoothstep(6.0 * px, 0.0, sd.x) * step(0.0, sd.x);
    float band = 0.5 + 0.5 * sin(TIME * 8.0 + kf * 2.1);
    band = mix(1.0, smoothstep(0.25, 0.75, band) * 1.6, striation);
    float puff = 1.0 + 1.6 * beatP() * ar + 0.4 * bassP() * ar;
    if (PASSINDEX == 12) dye += pigSel(kf) * ring * injection * 0.4 * band * puff;

    // streakline rake at the inlet
    float sp = 0.0175 * rakeSpacing;
    float yy = p.y / sp;
    float line = abs(fract(yy) - 0.5) * sp * R.x;   // distance in dye px
    float ln = 1.0 - smoothstep(0.45, 1.1, line);
    float inlet = 1.0 - smoothstep(0.004, 0.012, p.x);
    float pulse = mix(1.0, 0.35 + 0.65 * smoothstep(0.1, 0.3, fract(TIME * 0.9 + floor(yy) * 0.37)), 0.5);
    if (PASSINDEX == 13) dye.r = max(dye.r, ln * inlet * rake * pulse);

    if (sd.x < 0.0) dye = vec4(0.0);
    return clamp(dye, 0.0, 3.0);
}

// ═════════════════════════════════════════════════════════════════════════
//  SHADING (passes 14, 15)
// ═════════════════════════════════════════════════════════════════════════
vec3 g_c1, g_c2, g_c3, g_c4, g_cr;
vec3 g_l1, g_l2, g_l3, g_l4;

vec3 satAdj(vec3 c) { return max(mix(vec3(lum(c)), c, saturation), 0.0); }

void setupColors() {
    float h = hueShift + colorCycle * TIME * 0.03;
    g_c1 = satAdj(hueRot(ink1.rgb, h)); g_c2 = satAdj(hueRot(ink2.rgb, h));
    g_c3 = satAdj(hueRot(ink3.rgb, h)); g_c4 = satAdj(hueRot(ink4.rgb, h));
    g_l1 = log(g_c1 + 0.03); g_l2 = log(g_c2 + 0.03); g_l3 = log(g_c3 + 0.03); g_l4 = log(g_c4 + 0.03);
    g_cr = rakeColor.rgb;
}

// pigment-ish mixing: weighted geometric mean (subtractive feel: gold into
// magenta → vermilion, cyan into magenta → indigo, gold into cyan → green),
// renormalised to the arithmetic brightness so blends stay luminous
vec3 mixPig(vec4 d) {
    float D = dot(d, vec4(1.0)) + 1e-4;
    vec4 w = d / D;
    vec3 ar = w.x * g_c1 + w.y * g_c2 + w.z * g_c3 + w.w * g_c4;
    vec3 geo = exp(w.x * g_l1 + w.y * g_l2 + w.z * g_l3 + w.w * g_l4);
    float mA = max(ar.r, max(ar.g, ar.b));
    float mG = max(geo.r, max(geo.g, geo.b));
    geo *= mA / max(mG, 1e-3);
    return mix(ar, geo, 0.7);
}

// density curve: hair-fine wisps (dim, tinted, translucent) → veils →
// glowing glossy bodies → white-hot vortex eyes. thick = 0..1 weight of the
// glossy regime, cov = how much the ink hides the water
vec3 inkCol(vec4 d, out float thick, out float cov) {
    float D = max(dot(d, vec4(1.0)) - 0.012, 0.0);
    vec3 cm = mixPig(d);
    cov = 1.0 - exp(-2.6 * D);
    thick = smoothstep(0.3, 1.5, D);
    vec3 c = cm * (0.46 * cov * (1.0 - 0.5 * thick) + 1.0 * thick);
    float core = smoothstep(1.4, 2.9, D);
    c += mix(cm, vec3(1.0, 0.96, 0.9), 0.3) * core * 0.5;
    return c;
}

vec3 bodyColor(float kf) {
    float ch = mod(kf, 4.0);
    return ch < 0.5 ? g_c1 : (ch < 1.5 ? g_c2 : (ch < 2.5 ? g_c3 : g_c4));
}

// faint caustic web on the tank floor (two drifting warped-sine ridge layers)
float caust(vec2 q, float t) {
    vec2 a = q * 21.0;
    float v1 = abs(sin(a.x + 1.3 * sin(a.y * 0.7 + t * 0.6)) + sin(a.y + 1.2 * sin(a.x * 0.8 - t * 0.5)));
    vec2 b = q * 33.0 + 3.1;
    float v2 = abs(sin(b.x + 1.1 * sin(b.y * 0.9 - t * 0.45)) + sin(b.y + 1.3 * sin(b.x * 0.6 + t * 0.55)));
    return pow(1.0 - clamp(v1 * 0.5, 0.0, 1.0), 9.0) + 0.6 * pow(1.0 - clamp(v2 * 0.5, 0.0, 1.0), 9.0);
}

vec4 glowPass() {
    vec2 uv = gl_FragCoord.xy / R;
    vec2 px = 1.0 / R;
    vec3 acc = vec3(0.0);
    float wsum = 0.0;
    for (int i = 0; i < 12; i++) {
        float fi = float(i);
        float a = fi * 2.399963;
        float rr = sqrt((fi + 0.5) / 12.0) * 1.6;
        vec2 o = vec2(cos(a), sin(a)) * rr * px;
        float th, cv;
        vec3 e = inkCol(texture2D(dyeBuf, uv + o), th, cv) * th;
        float w = 1.0 - rr / 2.0;
        acc += e * e * w;   // squared: only the dense, hot ink blooms
        wsum += w;
    }
    acc /= wsum;
    // body rims bloom too
    vec2 p = uv * vec2(1.0, g_hy);
    vec2 sd = bodiesSD(p, bodyTime());
    acc += bodyColor(sd.y) * exp(-abs(sd.x) * 260.0) * 0.6;
    return vec4(acc, 1.0);
}

vec4 finalPass() {
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 px = 1.0 / R;
    vec2 p = uv * vec2(1.0, g_hy);
    float ar = audioReact;
    float t = bodyTime();

    // ink + relief
    vec4 d  = texture2D(dyeBuf, uv);
    vec4 dx1 = texture2D(dyeBuf, uv + vec2(px.x, 0.0)), dx0 = texture2D(dyeBuf, uv - vec2(px.x, 0.0));
    vec4 dy1 = texture2D(dyeBuf, uv + vec2(0.0, px.y)), dy0 = texture2D(dyeBuf, uv - vec2(0.0, px.y));
    float rk = texture2D(dyeAux, uv).r;
    float rkx = texture2D(dyeAux, uv + vec2(px.x, 0.0)).r - texture2D(dyeAux, uv - vec2(px.x, 0.0)).r;
    float rky = texture2D(dyeAux, uv + vec2(0.0, px.y)).r - texture2D(dyeAux, uv - vec2(0.0, px.y)).r;
    vec4 one = vec4(1.0);
    float D = dot(d, one);
    vec2 gi = vec2(dot(dx1 - dx0, one), dot(dy1 - dy0, one)) * 0.5;
    vec2 g = gi + 0.35 * vec2(rkx, rky);
    float thick, cov;
    vec3 ink = inkCol(d, thick, cov);
    float rcov = 1.0 - exp(-2.2 * rk);
    float hs = max(cov, rcov * 0.6);

    vec3 n = normalize(vec3(-g * 6.0 * relief, 1.0));
    vec3 L = normalize(vec3(-0.55, 0.6, 0.62));
    vec3 V = vec3(0.0, 0.0, 1.0);
    float diff = 0.62 + 0.38 * max(dot(n, L), 0.0) / L.z;
    ink *= mix(1.0, diff, clamp(relief, 0.0, 1.0) * thick);
    vec3 rakeC = g_cr * rcov * 0.5 * mix(1.0, diff, clamp(relief, 0.0, 1.0));

    // vorticity from the lattice velocity
    vec2 sr = simRes();
    vec2 sp = 1.0 / sr;
    float w = (latVel(uv + vec2(sp.x, 0.0)).y - latVel(uv - vec2(sp.x, 0.0)).y)
            - (latVel(uv + vec2(0.0, sp.y)).x - latVel(uv - vec2(0.0, sp.y)).x);
    w *= 0.5 * (sr.x / 480.0) / 0.02;
    float aw = abs(w);
    // spin tint: clockwise cools, counter-clockwise warms
    ink = mix(ink, hueRot(ink, 0.06 * sign(w) * smoothstep(0.1, 1.0, aw)), vortTint);
    // hot vortex cores
    float core = smoothstep(0.35, 1.4, aw) * thick;
    ink += ink * core * vortGlow * 0.9;

    // filament edges: crisp rims where ink ends — faint on wisps, bright on bodies
    float gm = length(gi);
    float edge = smoothstep(0.03, 0.2, gm) * (1.0 - smoothstep(0.9, 2.2, D));
    ink += mixPig(d) * edge * edgeAmt * mix(0.18, 0.5, thick);

    // gloss: only the thick ink (and faintly the streaklines) is wet and raised
    vec3 Hh = normalize(L + V);
    float spec = pow(max(dot(n, Hh), 0.0), 56.0) * max(thick, 0.35 * rcov);
    spec *= specAmt * (1.0 + 0.6 * highP() * ar);
    ink += vec3(1.0, 0.97, 0.93) * spec * step(0.002, length(g));

    // water: deep, caustic web + schlieren shimmer, refracted by the flow
    vec3 back = backColor.rgb;
    vec2 vel = latVel(uv);
    float sh = smoothstep(0.08, 1.2, aw) * 0.05 + smoothstep(0.0, 0.1, length(vel - vec2(g_U, 0.0))) * 0.012;
    back += vec3(0.25, 0.45, 0.9) * sh * schlieren;
    back += vec3(0.16, 0.36, 0.8) * caust(p + vel * 0.9, TIME * 0.5) * 0.05 * caustics;
    back *= 0.85 + 0.15 * hash12(floor(fc * 0.5) + 3.0);

    vec3 ug = texture2D(glowBuf, uv + vec2(0.006, -0.011)).rgb + texture2D(glowBuf, uv + vec2(0.012, -0.022)).rgb * 0.6;
    back += sqrt(max(ug, 0.0)) * 0.12;
    // thin ink sits slightly behind: it darkens the water and the streaklines before glowing
    vec3 col = (back + rakeC) * (1.0 - 0.85 * cov) + ink;

    // bodies: polished black glass with coloured rims
    vec2 sd = bodiesSD(p, t);
    float apx = px.x;
    vec3 bc = bodyColor(sd.y);
    // contact shadow on the ink around each body
    col *= mix(1.0, 0.45 + 0.55 * smoothstep(0.0, 0.02, sd.x), 0.8);
    if (sd.x < 2.0 * apx) {
        float e2 = 0.0015;
        vec2 gg = vec2(bodiesSD(p + vec2(e2, 0.0), t).x - bodiesSD(p - vec2(e2, 0.0), t).x,
                       bodiesSD(p + vec2(0.0, e2), t).x - bodiesSD(p - vec2(0.0, e2), t).x) / (2.0 * e2);
        float depth = clamp(-sd.x / 0.012, 0.0, 1.0);
        vec3 bn = normalize(vec3(gg * (1.0 - depth) * 1.8, 0.35 + depth));
        float bdiff = max(dot(bn, L), 0.0);
        float bspec = pow(max(dot(bn, Hh), 0.0), 90.0);
        float fres = pow(1.0 - bn.z, 3.0);
        vec3 bcol = vec3(0.006, 0.007, 0.01) + bdiff * 0.03 + bspec * vec3(1.0) * 1.3 + fres * bc * 0.5;
        // softbox crescent on the upper-left shoulder
        float sh2 = dot(gg, normalize(vec2(-0.6, 0.7)));
        float cres = smoothstep(0.35, 0.6, sh2) * smoothstep(0.0, 0.25, depth) * (1.0 - smoothstep(0.45, 0.8, depth));
        bcol += vec3(0.95, 0.97, 1.0) * cres * 0.85;
        // inner glow line + outer rim
        bcol += bc * exp(-abs(sd.x + 0.0022) / (0.7 * apx)) * 0.9;
        float m = smoothstep(apx, -apx, sd.x);
        col = mix(col, bcol, m);
    }
    col += bc * exp(-max(sd.x, 0.0) / (1.2 * apx)) * step(0.0, sd.x) * 0.6;

    // bloom (fringed)
    vec2 uvc = uv - 0.5;
    vec2 ab = uvc * aberration * 0.006;
    vec3 bl = vec3(0.0);
    for (int i = 0; i < 9; i++) {
        float fi = float(i);
        float a = fi * 2.399963 + 0.7;
        float rr = sqrt((fi + 0.5) / 9.0) * 2.5;
        vec2 o = vec2(cos(a), sin(a)) * rr / (R * 0.25);
        bl.r += texture2D(glowBuf, uv + o + ab).r;
        bl.g += texture2D(glowBuf, uv + o).g;
        bl.b += texture2D(glowBuf, uv + o - ab).b;
    }
    bl /= 9.0;
    col += bl * bloom * 1.1;

    // music lift (linear, gentle)
    col *= 1.0 + ar * (0.22 * bassP() + 0.1 * midP());
    col = aces(col * 1.15 * exposure);

    float g1 = (hash12(fc + fract(TIME) * 61.0) - 0.5) * 0.045;
    col += g1 * grain;
    col *= 1.0 - 0.1 * smoothstep(0.35, 0.85, length(uvc * vec2(1.0, g_hy) * 1.4));
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float l = lum(col);
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);

    float alpha = transparentBg ? clamp(max(hs * 1.4, smoothstep(apx, -apx, sd.x)) + lum(bl) * 2.0, 0.0, 1.0) : 1.0;
    return vec4(col, alpha);
}

void main() {
    g_hy = R.y / R.x;
    g_ar = audioReact;
    g_U = inflowU();
    if (PASSINDEX < 12) { gl_FragColor = lbmPass(); return; }
    setupColors();
    if (PASSINDEX == 12 || PASSINDEX == 13) { gl_FragColor = dyePass(); return; }
    if (PASSINDEX == 14) { gl_FragColor = glowPass(); return; }
    gl_FragColor = finalPass();
}
