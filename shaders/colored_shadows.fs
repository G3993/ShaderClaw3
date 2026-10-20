/*{
  "DESCRIPTION": "Colored Shadows — an installation study in additive light, after Olafur Eliasson and Goethe's coloured shadows. Three coloured spotlights — red, green and blue — wash a tall white gallery wall, and where they overlap the light adds up to white. Every form in front of the wall blocks a different light from a different angle, so its shadows are never grey: block the red and the shadow glows cyan, block the green and it turns magenta, block the blue and it burns yellow, and where two shadows overlap only one pure primary is left — until all three meet in a core of true black. A slowly turning Calder-style mobile of hanging discs, a white ceramic sphere carved into a gyroid lattice on a plinth, and a pair of interlocked rings tumbling in the air throw crisp umbras ringed by soft rainbow penumbrae that slide, stretch and cross as the lights drift and take turns fading. A dark polished floor mirrors the wall, faint haze glows around each lamp and dust motes catch the beams. Bass swells the lamps, beats breathe the light, mids warm the haze, highs sparkle the dust.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #7",
  "CATEGORIES": ["Generator", "3D", "Light", "Abstract", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Speed",              "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "mobileSpeed", "LABEL": "Mobile Turn",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "formSpeed",   "LABEL": "Sculpture Turn",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "lightDrift",  "LABEL": "Light Drift",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "camSway",     "LABEL": "Camera Sway",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "light1",      "LABEL": "Lamp 1",             "TYPE": "color", "DEFAULT": [1.0, 0.06, 0.035, 1.0], "GROUP": "Lights" },
    { "NAME": "light2",      "LABEL": "Lamp 2",             "TYPE": "color", "DEFAULT": [0.04, 1.0, 0.13, 1.0], "GROUP": "Lights" },
    { "NAME": "light3",      "LABEL": "Lamp 3",             "TYPE": "color", "DEFAULT": [0.07, 0.2, 1.0, 1.0],  "GROUP": "Lights" },
    { "NAME": "lightPower",  "LABEL": "Lamp Power",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Lights" },
    { "NAME": "lampSize",    "LABEL": "Lamp Size (penumbra)","TYPE": "float", "AUDIO": false, "MIN": 0.005, "MAX": 0.3, "DEFAULT": 0.06, "GROUP": "Lights" },
    { "NAME": "spotWidth",   "LABEL": "Spot Width",         "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Lights" },
    { "NAME": "turns",       "LABEL": "Lamps Take Turns",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.75, "GROUP": "Lights" },
    { "NAME": "whiteBal",    "LABEL": "Auto White Balance", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Lights" },
    { "NAME": "hero",        "LABEL": "Form (0 cycle, 1 dancer, 2 lattice sphere, 3 mobile, 4 rings)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 4.0, "DEFAULT": 0.0, "GROUP": "Forms" },
    { "NAME": "heroTime",    "LABEL": "Seconds per Form",   "TYPE": "float", "AUDIO": false, "MIN": 15.0, "MAX": 120.0, "DEFAULT": 45.0, "GROUP": "Forms" },
    { "NAME": "lattice",     "LABEL": "Lattice Density",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.75, "GROUP": "Forms" },
    { "NAME": "wallTone",    "LABEL": "Wall Colour",        "TYPE": "color", "DEFAULT": [0.86, 0.85, 0.83, 1.0], "GROUP": "Room" },
    { "NAME": "floorGloss",  "LABEL": "Floor Gloss",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Room" },
    { "NAME": "haze",        "LABEL": "Haze",               "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.3, "GROUP": "Room" },
    { "NAME": "dust",        "LABEL": "Dust Motes",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.3, "GROUP": "Room" },
    { "NAME": "ambient",     "LABEL": "Bounce Light",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Room" },
    { "NAME": "exposure",    "LABEL": "Exposure",           "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "bloom",       "LABEL": "Bloom",              "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",              "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.18, "GROUP": "Finish" },
    { "NAME": "marchRes",    "LABEL": "March Resolution",   "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Room" }
  ],
  "PASSES": [
    { "TARGET": "csScene", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Colored Shadows — CURVEBALL pack #7
//   pass 0 csScene : white gallery wall + dark gloss floor + three forms,
//                    lit by three coloured spotlights, each with its own
//                    penumbra soft shadow (objects only, box-clipped).
//                    floor = glossy reflection of the shaded room.
//   pass 1 final   : bloom, fringe, ACES, HD finisher, grain
// ─────────────────────────────────────────────────────────────────────────

#define TAU 6.28318530718
#define WALLZ 0.0
#define FLOORY -1.6

float g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev;
vec3  g_lp0, g_lp1, g_lp2;     // lamp positions
vec3  g_ld0, g_ld1, g_ld2;     // lamp aim
vec3  g_lc0, g_lc1, g_lc2;     // lamp colour * power
vec3  g_S;                     // sphere centre
vec3  g_RC;                    // ring group centre
vec3  g_M;                     // mobile pivot
mat3  g_sphRot, g_ringRot;
float g_a1, g_a2, g_bA, g_bB1, g_bB2, g_gs;
float g_cosO, g_cosI;
float g_scene;                 // 0 dancer, 1 lattice sphere, 2 mobile, 3 rings
vec3  g_D;                     // dancer base (feet)
float g_dYaw;
vec3  j_hip, j_chest, j_neck, j_head, j_shL, j_shR, j_elL, j_elR, j_haL, j_haR, j_hiL, j_hiR, j_knL, j_knR, j_ftL, j_ftR;

float hash12(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
mat3 rotY(float a) { float c = cos(a), s = sin(a); return mat3(c, 0.0, s, 0.0, 1.0, 0.0, -s, 0.0, c); }
mat3 rotX(float a) { float c = cos(a), s = sin(a); return mat3(1.0, 0.0, 0.0, 0.0, c, -s, 0.0, s, c); }
mat3 rotZ(float a) { float c = cos(a), s = sin(a); return mat3(c, -s, 0.0, s, c, 0.0, 0.0, 0.0, 1.0); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }

// ── SDF primitives ───────────────────────────────────────────────────────
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}
float sdDiscZ(vec3 q, float r, float th) {          // disc in the local xy plane
    vec2 d = vec2(length(q.xy) - r, abs(q.z) - th);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - 0.004;
}
float sdTorus(vec3 p, vec2 t) { vec2 q = vec2(length(p.xz) - t.x, p.y); return length(q) - t.y; }
float sdBox(vec3 p, vec3 b) { vec3 q = abs(p) - b; return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0); }

// ── the three forms ──────────────────────────────────────────────────────
float sphereForm(vec3 p) {
    vec3 q = p - g_S;
    float bound = length(q) - 0.66;
    if (bound > 0.08) return bound;
    q = g_sphRot * q;
    float shell = abs(length(q) - 0.6) - 0.042;
    vec3 g = q * g_gs;
    float gy = (abs(dot(sin(g), cos(g.yzx))) - 0.16) / g_gs;
    // smooth intersection rounds the strut rims so they read as clean bevels, not 1-px edges
    float a = shell, b = gy * 0.42, k = 0.025;
    float h = clamp(0.5 - 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) + k * h * (1.0 - h);
}
float plinth(vec3 p) {
    return sdBox(p - vec3(g_S.x, -1.31, g_S.z), vec3(0.4, 0.29, 0.4)) - 0.01;
}
float ringForm(vec3 p) {
    vec3 q = p - g_RC;
    float bound = length(q) - 1.12;
    if (bound > 0.08) return bound;
    q = g_ringRot * q;
    float a = sdTorus(q + vec3(0.27, 0.0, 0.0), vec2(0.5, 0.05));
    vec3 q2 = q - vec3(0.27, 0.0, 0.0);
    q2 = vec3(q2.x, -q2.z, q2.y);                    // ring B stands up through ring A
    float b = sdTorus(q2, vec2(0.42, 0.045));
    return min(a, b);
}
float mobileForm(vec3 p) {
    float wire0 = sdCapsule(p, g_M, g_M + vec3(0.0, 3.0, 0.0), 0.006);
    vec3 q = p - g_M;
    float bound = length(q - vec3(0.25, -0.45, 0.0)) - 1.6;
    if (bound > 0.08) return min(wire0, bound);
    q.xz = rot(g_a1) * q.xz;
    float d = wire0;
    d = min(d, sdCapsule(q, vec3(-0.55, 0.0, 0.0), vec3(0.95, 0.03, 0.0), 0.011));
    // disc A — big, hangs from the left end
    d = min(d, sdCapsule(q, vec3(-0.55, 0.0, 0.0), vec3(-0.55, -0.34, 0.0), 0.005));
    vec3 qa = q - vec3(-0.55, -0.62, 0.0);
    qa.xz = rot(g_bA) * qa.xz;
    d = min(d, sdDiscZ(qa, 0.28, 0.008));
    // sub-arm on the right end
    d = min(d, sdCapsule(q, vec3(0.95, 0.03, 0.0), vec3(0.95, -0.24, 0.0), 0.005));
    vec3 q2 = q - vec3(0.95, -0.24, 0.0);
    q2.xz = rot(g_a2) * q2.xz;
    d = min(d, sdCapsule(q2, vec3(-0.5, 0.0, 0.0), vec3(0.55, -0.02, 0.0), 0.009));
    d = min(d, sdCapsule(q2, vec3(-0.5, 0.0, 0.0), vec3(-0.5, -0.22, 0.0), 0.005));
    vec3 qb1 = q2 - vec3(-0.5, -0.4, 0.0);
    qb1.xz = rot(g_bB1) * qb1.xz;
    d = min(d, sdDiscZ(qb1, 0.18, 0.007));
    d = min(d, sdCapsule(q2, vec3(0.55, -0.02, 0.0), vec3(0.55, -0.3, 0.0), 0.005));
    // the last element is a crescent: disc minus an offset disc
    vec3 qb2 = q2 - vec3(0.55, -0.52, 0.0);
    qb2.xz = rot(g_bB2) * qb2.xz;
    float cres = max(sdDiscZ(qb2, 0.22, 0.007), -(length(qb2.xy - vec2(0.1, 0.07)) - 0.17));
    d = min(d, cres);
    return d;
}

float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }
float sdCapT(vec3 p, vec3 a, vec3 b, float ra, float rb) {      // tapered limb
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - mix(ra, rb, h);
}
// a dancer: joints are animated in setupScene, here only the flesh
float dancerForm(vec3 p) {
    vec3 q = p - g_D;
    float bound = length(q - vec3(0.0, 0.95, 0.0)) - 1.25;
    if (bound > 0.1) return bound;
    q.xz = rot(g_dYaw) * q.xz;
    float d = sdCapT(q, j_hip, j_chest, 0.13, 0.15);
    d = smin(d, sdCapT(q, j_chest, j_neck, 0.15, 0.06), 0.06);
    d = smin(d, length((q - j_head) * vec3(1.0, 0.86, 1.0)) - 0.105, 0.04);
    d = smin(d, sdCapT(q, j_shL, j_shR, 0.06, 0.06), 0.06);
    d = smin(d, sdCapT(q, j_shL, j_elL, 0.055, 0.042), 0.03);
    d = smin(d, sdCapT(q, j_elL, j_haL, 0.042, 0.03), 0.02);
    d = smin(d, sdCapT(q, j_shR, j_elR, 0.055, 0.042), 0.03);
    d = smin(d, sdCapT(q, j_elR, j_haR, 0.042, 0.03), 0.02);
    d = smin(d, sdCapT(q, j_hiL, j_hiR, 0.11, 0.11), 0.06);
    d = smin(d, sdCapT(q, j_hiL, j_knL, 0.085, 0.055), 0.04);
    d = smin(d, sdCapT(q, j_knL, j_ftL, 0.055, 0.035), 0.02);
    d = smin(d, sdCapT(q, j_hiR, j_knR, 0.085, 0.055), 0.04);
    d = smin(d, sdCapT(q, j_knR, j_ftR, 0.055, 0.035), 0.02);
    return d;
}

float objSDF(vec3 p) {
    if (g_scene < 0.5) return dancerForm(p);
    if (g_scene < 1.5) return min(sphereForm(p), plinth(p));
    if (g_scene < 2.5) return mobileForm(p);
    return ringForm(p);
}

// object AABB (generous) for clipping primary / shadow / reflection marches
const vec3 BMIN = vec3(-3.4, -1.62, 0.2);
const vec3 BMAX = vec3( 3.3,  4.8,  2.75);
vec2 boxHit(vec3 ro, vec3 rd) {
    vec3 inv = 1.0 / rd;
    vec3 t0 = (BMIN - ro) * inv, t1 = (BMAX - ro) * inv;
    vec3 tn = min(t0, t1), tf = max(t0, t1);
    return vec2(max(max(tn.x, tn.y), tn.z), min(min(tf.x, tf.y), tf.z));
}

vec3 objNormal(vec3 p) {
    const vec2 k = vec2(1.0, -1.0);
    float e = 0.0011;
    return normalize(k.xyy * objSDF(p + k.xyy * e) + k.yyx * objSDF(p + k.yyx * e) +
                     k.yxy * objSDF(p + k.yxy * e) + k.xxx * objSDF(p + k.xxx * e));
}

// march objects along ro+rd*t for t in [ta, tb]; returns hit t or -1
float marchObj(vec3 ro, vec3 rd, float ta, float tb, int maxSteps) {
    float t = ta;
    for (int i = 0; i < 110; i++) {
        if (i >= maxSteps || t > tb) break;
        float h = objSDF(ro + rd * t);
        if (h < 0.0009 * (1.0 + t)) return t;
        t += h * 0.92;
    }
    return -1.0;
}

// penumbra shadow toward a lamp of radius lampSize (k = 1 / angular radius of the lamp)
float softShadow(vec3 p, vec3 l, float dist, int maxSteps) {
    vec2 bh = boxHit(p, l);
    float ta = max(bh.x, 0.012), tb = min(bh.y, dist - 0.05);
    if (tb <= ta) return 1.0;
    float k = dist / max(lampSize, 0.004);
    float res = 1.0, t = ta;
    for (int i = 0; i < 72; i++) {
        if (i >= maxSteps || t > tb) break;
        float h = objSDF(p + l * t);
        if (h < 0.0005) return 0.0;
        // classic penumbra estimate (the "improved" y-correction misfires where the
        // bound-sphere distance jumps to the true distance and blackens whole discs)
        res = min(res, k * h / t);
        t += clamp(h, 0.004, 0.1);
    }
    res = clamp(res, 0.0, 1.0);
    return res * res * (3.0 - 2.0 * res);
}

// ── lamps ────────────────────────────────────────────────────────────────
vec3 limbDir(float raise, float swing, float side) {   // raise 0 = hanging down, pi = overhead; side -1 left / +1 right
    return normalize(vec3(side * sin(raise), -cos(raise), swing));
}
void setupScene() {
    float T = g_t;
    float lt = T * lightDrift;
    // three lamps spread wide and roughly level, so one form throws a FAN of three shadows side by side
    g_lp0 = vec3(-3.5 + 0.35 * sin(lt * 0.19),        0.75 + 0.3 * sin(lt * 0.23 + 1.0), 5.4);
    g_lp1 = vec3( 0.0 + 0.45 * sin(lt * 0.15 + 2.0),  1.05 + 0.25 * sin(lt * 0.21),       5.6);
    g_lp2 = vec3( 3.5 + 0.35 * sin(lt * 0.17 + 4.0),  0.7 + 0.3 * sin(lt * 0.27 + 2.5),   5.4);
    vec3 aim = vec3(0.0, -0.25, 0.0);
    g_ld0 = normalize(aim + vec3( 0.5, 0.0, 0.0) - g_lp0);
    g_ld1 = normalize(aim - g_lp1);
    g_ld2 = normalize(aim + vec3(-0.5, 0.0, 0.0) - g_lp2);
    g_cosO = cos(0.6 * spotWidth);
    g_cosI = cos(0.16 * spotWidth);

    // auto white balance: per-lamp gains so the three lamps sum to a warm white at the wall centre
    vec3 C = vec3(0.0, -0.1, 0.0);
    vec3 L0 = g_lp0 - C, L1 = g_lp1 - C, L2 = g_lp2 - C;
    float E0 = max(normalize(L0).z, 0.0) * pow(smoothstep(g_cosO, g_cosI, dot(-normalize(L0), g_ld0)), 2.0) / dot(L0, L0);
    float E1 = max(normalize(L1).z, 0.0) * pow(smoothstep(g_cosO, g_cosI, dot(-normalize(L1), g_ld1)), 2.0) / dot(L1, L1);
    float E2 = max(normalize(L2).z, 0.0) * pow(smoothstep(g_cosO, g_cosI, dot(-normalize(L2), g_ld2)), 2.0) / dot(L2, L2);
    vec3 A0 = light1.rgb * E0, A1 = light2.rgb * E1, A2 = light3.rgb * E2;
    vec3 W = vec3(1.0, 0.94, 0.84) * 0.98;
    float det = dot(A0, cross(A1, A2));
    vec3 gain = vec3(1.0) / max(vec3(E0, E1, E2) * 3.0, 1e-4);            // fallback: equal irradiance
    if (abs(det) > 1e-7) {
        vec3 gs = vec3(dot(W, cross(A1, A2)), dot(A0, cross(W, A2)), dot(A0, cross(A1, W))) / det;
        gain = mix(gain * 1.25, clamp(gs, vec3(0.0), vec3(1e4)), whiteBal);
    } else gain *= 1.25;

    // lamps take turns dimming (each for ~12% of a ~3 min cycle) — the other two lift to keep the wall bright
    float ph = T * 0.0352 * lightDrift - 0.19;
    float f0 = 1.0 - 0.85 * turns * smoothstep(0.9, 0.995, sin(ph));
    float f1 = 1.0 - 0.85 * turns * smoothstep(0.9, 0.995, sin(ph + TAU / 3.0));
    float f2 = 1.0 - 0.85 * turns * smoothstep(0.9, 0.995, sin(ph + 2.0 * TAU / 3.0));
    float comp = clamp(3.0 / (f0 + f1 + f2), 1.0, 1.45);
    f0 *= 0.93 + 0.07 * sin(T * 0.41);
    f1 *= 0.93 + 0.07 * sin(T * 0.37 + 2.0);
    f2 *= 0.93 + 0.07 * sin(T * 0.43 + 4.0);

    // one hero form at a time; the lamps black out briefly for each change, like a gallery cue
    float hp = T / heroTime + 0.06;
    float cue = smoothstep(0.0, 0.035, fract(hp)) * smoothstep(1.0, 0.965, fract(hp));
    g_scene = hero > 0.5 ? floor(hero - 0.5 + 0.001) : mod(floor(hp), 4.0);
    if (hero > 0.5) cue = 1.0;

    float lift = 1.0 + g_ar * (0.4 * g_bass + 0.15 * g_lev + 0.2 * g_beat);
    float P = lightPower * lift * comp * cue;
    g_lc0 = light1.rgb * gain.x * P * f0;
    g_lc1 = light2.rgb * gain.y * P * f1;
    g_lc2 = light3.rgb * gain.z * P * f2;

    float ft = T * formSpeed;
    g_S = vec3(0.0, -0.39, 1.75);
    g_sphRot = rotX(0.35 * sin(ft * 0.13)) * rotY(ft * 0.19);
    g_gs = mix(4.2, 7.5, clamp(lattice + 0.1 * sin(T * 0.045), 0.0, 1.0));
    g_RC = vec3(0.0, 0.15, 1.6);
    g_ringRot = rotZ(ft * 0.11 + 0.6) * rotY(ft * 0.17) * rotX(0.5 * sin(ft * 0.09));
    float mt = T * mobileSpeed;
    g_M = vec3(-0.22, 0.95, 1.2);
    g_a1 = mt * 0.12 + 0.75 * sin(mt * 0.21);
    g_a2 = -mt * 0.2 + 0.9 * sin(mt * 0.16 + 1.0);
    g_bA = 0.55 * sin(mt * 0.33);
    g_bB1 = 0.8 * sin(mt * 0.29 + 2.0);
    g_bB2 = mt * 0.24;

    // ── the dancer: a slow contemporary phrase (plié, sway, arm arcs, side extension) ──
    float w = ft * 0.55;
    g_D = vec3(0.35 * sin(ft * 0.05), FLOORY, 1.65);
    g_dYaw = 0.5 * sin(ft * 0.07) + 0.25 * sin(w * 0.31);
    float plie = 0.06 * (0.5 + 0.5 * sin(w * 2.0));
    j_hip = vec3(0.1 * sin(w), 0.97 - plie, 0.0);
    float lean = 0.22 * sin(w + 0.5), pitch = 0.12 * sin(w * 0.7);
    vec3 up = normalize(vec3(sin(lean), cos(lean) * cos(pitch), sin(pitch)));
    j_chest = j_hip + up * 0.42;
    j_neck  = j_chest + up * 0.17;
    j_head  = j_neck + normalize(up + vec3(0.15 * sin(w * 0.9), 0.0, 0.1)) * 0.13;
    vec3 sideAx = normalize(vec3(cos(lean), -sin(lean), 0.0));
    j_shL = j_chest + up * 0.08 - sideAx * 0.19;
    j_shR = j_chest + up * 0.08 + sideAx * 0.19;
    float rL = 1.25 + 1.15 * sin(w * 0.8), rR = 1.0 + 1.25 * sin(w * 0.8 + 2.2);
    vec3 uL = limbDir(rL, 0.35 * sin(w * 0.6), -1.0), uR = limbDir(rR, 0.35 * sin(w * 0.6 + 1.5), 1.0);
    j_elL = j_shL + uL * 0.29;
    j_elR = j_shR + uR * 0.29;
    j_haL = j_elL + normalize(uL + vec3(0.0, 0.55 + 0.4 * sin(w * 1.1 + 1.0), 0.35)) * 0.27;
    j_haR = j_elR + normalize(uR + vec3(0.0, 0.55 + 0.4 * sin(w * 1.1 + 3.0), 0.35)) * 0.27;
    j_hiL = j_hip - sideAx * 0.1;
    j_hiR = j_hip + sideAx * 0.1;
    // standing leg absorbs the plié; the free leg sweeps out to the side and back
    float bend = plie * 3.0;
    j_knL = j_hiL + normalize(vec3(-0.02, -1.0, 0.12 + bend)) * 0.46;
    j_ftL = vec3(j_hiL.x - 0.03, 0.04, 0.0);
    float ext = 0.75 * smoothstep(-0.4, 1.0, sin(w * 0.5));
    vec3 lr = normalize(vec3(sin(ext), -cos(ext), -0.35 * sin(ext)));
    j_knR = j_hiR + lr * 0.46;
    j_ftR = j_knR + normalize(lr + vec3(0.0, -0.15, 0.0)) * 0.46;
    j_ftR.y = max(j_ftR.y, 0.04);
}

vec3 lampTerm(vec3 P, vec3 N, vec3 V, vec3 lp, vec3 ld, vec3 lc, float spec, float shininess, int sSteps) {
    vec3 L = lp - P;
    float d = length(L);
    vec3 l = L / d;
    float ndl = dot(N, l);
    if (ndl <= 0.0) return vec3(0.0);
    float spot = smoothstep(g_cosO, g_cosI, dot(-l, ld)); spot *= spot;
    if (spot <= 0.0) return vec3(0.0);
    float vis = softShadow(P + N * 0.012, l, d, sSteps);
    vec3 h = normalize(l + V);
    float sp = spec * pow(max(dot(N, h), 0.0), shininess) * (shininess + 8.0) / 25.0;
    return lc * spot * vis * (ndl + sp) / (d * d);
}

vec3 lampsAt(vec3 P, vec3 N, vec3 V, float spec, float shin, int sSteps) {
    return lampTerm(P, N, V, g_lp0, g_ld0, g_lc0, spec, shin, sSteps)
         + lampTerm(P, N, V, g_lp1, g_ld1, g_lc1, spec, shin, sSteps)
         + lampTerm(P, N, V, g_lp2, g_ld2, g_lc2, spec, shin, sSteps);
}

// unshadowed light reaching a point in the air (for haze/dust)
vec3 airLight(vec3 P) {
    vec3 c = vec3(0.0);
    vec3 L = g_lp0 - P; float d = length(L); c += g_lc0 * smoothstep(g_cosO, g_cosI, dot(-L / d, g_ld0)) / (d * d);
    L = g_lp1 - P; d = length(L); c += g_lc1 * smoothstep(g_cosO, g_cosI, dot(-L / d, g_ld1)) / (d * d);
    L = g_lp2 - P; d = length(L); c += g_lc2 * smoothstep(g_cosO, g_cosI, dot(-L / d, g_ld2)) / (d * d);
    return c;
}

// analytic in-scatter of a point lamp along a ray (glow around each lamp)
vec3 lampGlow(vec3 ro, vec3 rd, float tmax, vec3 lp, vec3 lc) {
    float b = dot(lp - ro, rd);
    float h = max(length(ro + rd * b - lp), 0.02);
    float I = (atan((tmax - b) / h) + atan(b / h)) / h;
    return lc * I;
}

// ── surface shading ──────────────────────────────────────────────────────
vec3 shadeWall(vec3 P, vec3 V, int sSteps) {
    // plaster: low-amplitude bump + faint albedo mottling
    vec2 u = P.xy;
    float n1 = vnoise(u * 38.0), n2 = vnoise(u * 9.0 + 7.0), n3 = vnoise(u * 140.0);
    vec3 N = normalize(vec3((n1 - 0.5) * 0.03 + (n3 - 0.5) * 0.015, (vnoise(u * 38.0 + 3.1) - 0.5) * 0.03, 1.0));
    vec3 alb = wallTone.rgb * (0.95 + 0.06 * n2 + 0.02 * n3);
    vec3 lit = lampsAt(P, N, V, 0.03, 6.0, sSteps);
    // contact darkening near forms + skirting gradient near the floor
    float dO = objSDF(P + vec3(0.0, 0.0, 0.12));
    float ao = clamp(0.55 + 0.45 * smoothstep(0.0, 0.5, dO), 0.0, 1.0);
    ao *= 0.8 + 0.2 * smoothstep(FLOORY, FLOORY + 0.6, P.y);
    vec3 amb = vec3(lum(airLight(vec3(P.xy, 0.0)))) * 0.02 * ambient;
    return alb * (lit + amb * ao);
}
vec3 shadeObj(vec3 P, vec3 V, int sSteps) {
    vec3 N = objNormal(P);
    // ceramic glaze on the lattice sphere, satin plaster elsewhere
    // everything is matte white plaster / bisque: colour lives in the shadows, not on the forms
    vec3 alb = vec3(0.84, 0.83, 0.81);
    float spec = 0.05;
    float shin = 8.0;
    vec3 lit = lampsAt(P, N, V, spec, shin, sSteps);
    // 3-tap AO
    float ao = 0.0;
    ao += (0.03 - objSDF(P + N * 0.03)) * 1.0;
    ao += (0.09 - objSDF(P + N * 0.09)) * 0.5;
    ao += (0.2  - objSDF(P + N * 0.2 )) * 0.25;
    ao = clamp(1.0 - 2.2 * ao, 0.0, 1.0);
    // neutral room bounce (the lit white wall fills the forms gently with white)
    vec3 wallLit = airLight(vec3(P.xy * 0.6, 0.05));
    vec3 amb = vec3(lum(wallLit)) * 0.22 * ambient * (0.55 + 0.45 * abs(N.z));
    float back = clamp(-N.z, 0.0, 1.0);
    vec3 wallBounce = wallLit * 0.18 * back;
    return alb * (lit + (amb + wallBounce) * ao);
}

// trace a ray into the room: returns colour, t of the hit in .a; floor handled by caller
vec4 traceRoom(vec3 ro, vec3 rd, int steps, int sSteps, out int kind) {
    float tW = (WALLZ - ro.z) / rd.z;
    if (rd.z >= 0.0) tW = 1e4;
    float tF = rd.y < 0.0 ? (FLOORY - ro.y) / rd.y : 1e4;
    float tEnd = min(tW, tF);
    vec2 bh = boxHit(ro, rd);
    float tO = -1.0;
    if (bh.y > max(bh.x, 0.0)) tO = marchObj(ro, rd, max(bh.x, 0.0), min(bh.y, tEnd), steps);
    vec3 V = -rd;
    if (tO > 0.0) { kind = 2; return vec4(shadeObj(ro + rd * tO, V, sSteps), tO); }
    if (tW < tF)  { kind = 0; return vec4(shadeWall(ro + rd * tW, V, sSteps), tW); }
    kind = 1; return vec4(0.0, 0.0, 0.0, tF);
}

vec3 dustMotes(vec3 ro, vec3 rd, float tmax) {
    vec3 c = vec3(0.0);
    for (int k = 0; k < 3; k++) {
        float zc = 3.4 - float(k) * 0.55;
        float t = (zc - ro.z) / rd.z;
        if (t <= 0.0 || t > tmax) continue;
        vec3 P = ro + rd * t;
        vec2 drift = vec2(g_t * 0.03, -g_t * 0.018) * (1.0 + float(k) * 0.4);
        vec2 g = P.xy * 9.0 + drift + float(k) * 13.7;
        vec2 cell = floor(g);
        vec2 j = hash22(cell + float(k) * 31.0);
        if (j.x > 0.32) continue;                          // sparse
        vec2 cp = cell + 0.2 + 0.6 * hash22(cell * 1.7 + 3.0);
        float px = t * 1.2 / (RENDERSIZE.y * 1.0) * 9.0;  // one pixel in cell units
        float r = length(g - cp) / max(px, 1e-4);
        float m = exp(-r * r * 0.35);
        float tw = 0.55 + 0.45 * sin(g_t * (1.5 + 3.0 * j.y) + j.x * 40.0);
        c += airLight(P) * m * tw * (0.05 + 0.12 * g_high * g_ar);
    }
    return c * dust;
}

vec3 shadeFloor(vec3 ro, vec3 rd, vec3 P) {
    // dark polished concrete with a broken glossy reflection
    vec2 u = P.xz;
    float n = vnoise(u * 6.0), n2 = vnoise(u * 31.0);
    vec3 N = normalize(vec3((vnoise(u * 9.0) - 0.5) * 0.018, 1.0, (vnoise(u * 9.0 + 5.3) - 0.5) * 0.018));
    vec3 alb = vec3(0.1, 0.098, 0.104) * (0.8 + 0.4 * n) * (0.92 + 0.16 * n2);
    vec3 V = -rd;
    vec3 dl = lampsAt(P, N, V, 0.0, 1.0, 40);
    // the floor sees the lamps at grazing angles where the auto white balance does not hold —
    // keep half of it neutral so the floor stays charcoal, not green
    vec3 dif = alb * mix(vec3(lum(dl)), dl, 0.5);
    float dO = objSDF(P);
    dif *= 0.45 + 0.55 * smoothstep(0.0, 0.35, dO);   // contact shadow under the plinth / feet
    vec3 rr = reflect(rd, N);
    int k2;
    vec4 rh = traceRoom(P + N * 0.004, rr, 60, 30, k2);
    vec3 refl = k2 == 1 ? vec3(0.0) : rh.rgb;
    float cosv = clamp(dot(N, V), 0.0, 1.0);
    float F = 0.04 + 0.96 * pow(1.0 - cosv, 5.0);
    float gloss = floorGloss * (0.75 + 0.25 * n2);
    return dif + refl * gloss * (0.22 + 0.78 * F) * (0.85 + 0.15 * n);
}

void main() {
    g_t    = TIME * speed;
    g_ar   = audioReact;
    g_bass = smoothstep(0.05, 0.85, audioBass);
    g_mid  = smoothstep(0.06, 0.85, audioMid);
    g_high = smoothstep(0.10, 0.90, audioHigh);
    g_beat = clamp(audioBeatPulse, 0.0, 1.0) * smoothstep(0.0, 0.25, clamp(audioBeatPhase, 0.0, 1.0));
    g_lev  = clamp(audioLevel, 0.0, 1.0);

    if (PASSINDEX == 0) {
        setupScene();
        vec2 res = RENDERSIZE.xy;
        vec2 uv = (gl_FragCoord.xy - 0.5 * res) / res.y;
        float cs = camSway * g_t;
        vec3 ro = vec3(0.55 * sin(cs * 0.07), 0.12 + 0.1 * sin(cs * 0.05 + 1.0), 7.3);
        vec3 ta = vec3(0.12 * sin(cs * 0.06), -0.05, 0.9);
        vec3 ww = normalize(ta - ro), uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0))), vv = cross(uu, ww);
        vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.75 * ww);

        int kind;
        vec4 hit = traceRoom(ro, rd, 100, 56, kind);
        vec3 col = hit.rgb;
        float tHit = hit.a;
        vec3 Ph = ro + rd * tHit;
        float pxw = tHit / (RENDERSIZE.y * 1.75);              // one pixel in world units at the hit
        if (kind == 1) {
            col = shadeFloor(ro, rd, Ph);
            // analytic AA of the wall/floor seam: blend in the wall within a pixel of it
            float cov = clamp(0.5 - Ph.z / (pxw / max(abs(rd.y), 0.05)), 0.0, 1.0);
            if (cov > 0.0) col = mix(col, shadeWall(vec3(Ph.x, FLOORY + 0.0005, 0.0), -rd, 40), cov);
        } else if (kind == 0) {
            float cov = clamp(0.5 - (Ph.y - FLOORY) / pxw, 0.0, 1.0);
            if (cov > 0.0) col = mix(col, shadeFloor(ro, rd, vec3(Ph.x, FLOORY, 0.0005)), cov);
        }
        // haze glow around the lamps + dust in the beams
        float hz = haze * (1.0 + 0.5 * g_mid * g_ar);
        col += (lampGlow(ro, rd, tHit, g_lp0, g_lc0) + lampGlow(ro, rd, tHit, g_lp1, g_lc1) + lampGlow(ro, rd, tHit, g_lp2, g_lc2)) * 0.0045 * hz;
        col += dustMotes(ro, rd, tHit);
        col *= exposure;
        float a = transparentBg ? clamp(lum(col) * 1.6, 0.0, 1.0) : 1.0;
        gl_FragColor = vec4(col, a);
        return;
    }

    // ── final ────────────────────────────────────────────────────────────
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 px = 1.0 / RENDERSIZE.xy;
    vec2 dir = uv - 0.5;
    float ab = aberration * 0.0022;
    vec3 col;
    col.r = texture2D(csScene, uv + dir * ab).r;
    col.g = texture2D(csScene, uv).g;
    col.b = texture2D(csScene, uv - dir * ab).b;
    float alpha = texture2D(csScene, uv).a;
    // two-ring bloom on highlights (fixed golden-angle taps — no per-pixel randomness)
    vec3 bl = vec3(0.0);
    float wsum = 0.0;
    for (int i = 0; i < 16; i++) {
        float fi = float(i);
        float ang = fi * 2.39996;
        float r = (fi < 8.0 ? 5.0 : 15.0) * (0.6 + 0.4 * fract(fi * 0.618));
        vec2 o = vec2(cos(ang), sin(ang)) * r * px * (RENDERSIZE.y / 1080.0);
        vec3 s = texture2D(csScene, uv + o).rgb;
        float w = fi < 8.0 ? 1.0 : 0.6;
        bl += max(s - 0.98, 0.0) * w;
        wsum += w;
    }
    col += bl / wsum * bloom * 1.4;
    col = aces(col * 1.05);
    // HD finisher
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float l = dot(col, vec3(0.299, 0.587, 0.114));
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    float vig = 1.0 - 0.12 * dot(dir * vec2(1.2, 1.0), dir * vec2(1.2, 1.0)) * 2.2;
    col *= vig;
    float gr = hash12(gl_FragCoord.xy + fract(TIME * 7.13) * 917.0) - 0.5;
    col += gr * grain * 0.045;
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), alpha);
}
