/*{
  "DESCRIPTION": "Proteins — a living protein drawn like a molecular-graphics render (PyMOL / ChimeraX), raymarched in 3D with glossy studio lighting and soft ambient occlusion on a dark stage. A single chain wriggles across the screen as a loose string of beads, then collapses and folds: alpha helices coil up into springy ribbons, beta strands snap flat into a twisted sheet of arrows, loops curl into tubes, and the whole compact fold turns slowly before it unravels and the next form appears. Four forms morph in turn: Ribbon Fold (the classic cartoon, rainbow-coloured blue at the N-terminus to red at the C-terminus), Space-Fill (every atom a CPK sphere: carbons tinted by chain, oxygen red, nitrogen blue, sulphur yellow, wrapped in a translucent glass surface), Molecular Surface (one blobby iridescent skin with breathing binding pockets and glowing ligands nestled inside) and Virus Capsid (120 copies of the protein assembling into an icosahedral shell). Colour by rainbow N→C, by secondary structure or by chain. The bass makes the structure breathe, beats snap the folding forward in steps, and a light pulse runs down the chain at the peak; the drop bursts the protein apart into a cloud of half-unravelled pieces that then fold back together. At rest it is a calm, pale fold breathing slowly; during a build it contracts, spins up and trembles; at the peak it glows in full colour inside a translucent shell.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "3D", "Science", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Ribbon Fold", "Space-Fill", "Molecular Surface", "Virus Capsid"], "GROUP": "Structure" },
    { "NAME": "morphPeriod", "LABEL": "Fold Cycle (s)",   "TYPE": "float", "AUDIO": false, "MIN": 6.0, "MAX": 60.0, "DEFAULT": 16.0, "GROUP": "Structure" },
    { "NAME": "colourSel",   "LABEL": "Colour Scheme", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Rainbow N→C", "Secondary Structure", "By Chain"], "GROUP": "Look" },
    { "NAME": "shellAmt",    "LABEL": "Translucent Surface", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "gloss",       "LABEL": "Gloss",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "aoAmt",       "LABEL": "Ambient Occlusion","TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "pulseAmt",    "LABEL": "Chain Pulse",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "spin",        "LABEL": "Rotation",         "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "wriggle",     "LABEL": "String Wriggle",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "zoom",        "LABEL": "Zoom",             "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "renderScale", "LABEL": "Render Scale",     "TYPE": "float", "AUDIO": false, "MIN": 0.35, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 2, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.035, 0.04, 0.06, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
    { "TARGET": "sceneBuf", "WIDTH": "floor($WIDTH*$renderScale)", "HEIGHT": "floor($HEIGHT*$renderScale)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Proteins — single pass. One 7-element chain (H S S H S S H) whose element
// frames are computed once per pixel; the SDF is built from helical ribbons,
// arrow strands and Bezier loops (or atom spheres along the same backbone).
// Capsid = icosahedral fold of space + one scaled copy of the protein.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }
float smax(float a, float b, float k) { return -smin(-a, -b, k); }
vec3 rotA(vec3 v, vec3 k, float a) { float c = cos(a), s = sin(a); return v * c + cross(k, v) * s + k * dot(k, v) * (1.0 - c); }
vec3 hdir(float h) { return normalize(vec3(h11(h + 0.31), h11(h + 4.17), h11(h + 9.73)) - 0.5 + 1e-3); }

vec3 studioPalette(vec3 c) {
    float m = float(paletteMode);
    if (m < 0.5 || palAmount <= 0.0) return c;
    vec3 acc = m < 1.5 ? palRed.rgb : palBlue.rgb;
    float L = clamp(dot(c, vec3(0.2126, 0.7152, 0.0722)), 0.0, 1.0);
    float mx = max(c.r, max(c.g, c.b)), mn = min(c.r, min(c.g, c.b));
    float sat = mx > 1e-4 ? (mx - mn) / mx : 0.0;
    float t = pow(L, 0.8);
    vec3 ramp = t < 0.5 ? mix(vec3(0.0), acc, t * 2.0) : mix(acc, vec3(1.0), (t - 0.5) * 2.0);
    vec3 mono = vec3(smoothstep(0.0, 1.0, L));
    vec3 o = mix(mono, ramp, smoothstep(0.06, 0.4, sat));
    return mix(c, o, palAmount);
}
vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

// ── Show dynamics: rest → build → peak / drop ───────────────────────────
float gRest, gBuild, gPeak, gDrop, gLift;
void showDynamics() {
    float m = float(showState);
    float e = clamp(audioEnergy, 0.0, 1.0);
    float b = clamp(audioBuildup, 0.0, 1.0);
    float d = clamp(audioDrop, 0.0, 1.0);
    if (m > 0.5) {
        if (m < 1.5)      { e = 0.04; b = 0.0; d = 0.0; }
        else if (m < 2.5) { float r = fract(TIME / 8.0); e = 0.3 + 0.35 * r; b = r * r; d = 0.0; }
        else              { e = 1.0; b = 0.0; d = exp(-fract(TIME / 4.0) * 5.0); }
    }
    float k = dynamics;
    gBuild = b * k;
    gDrop  = d * k;
    gPeak  = smoothstep(0.5, 0.9, e) * k;
    gRest  = (1.0 - smoothstep(0.08, 0.4, e)) * (1.0 - b) * k;
    gLift  = mix(0.5, clamp(0.15 + e * 0.7 + gBuild * 0.25 + d * 0.3, 0.0, 1.0), k);
}

// ── chain state (computed once per pixel in main) ───────────────────────
float gT, gF, gScale, gFoldW;
vec3 C0, C1, C2, C3, C4, C5, C6;
vec3 D0, D1, D2, D3, D4, D5, D6;
vec3 U0, U1, U2, U3, U4, U5, U6;
vec3 V0, V1, V2, V3, V4, V5, V6;
float L0, L1, L2, L3, L4, L5, L6;
float u0, u1, u2, u3, u4, u5, u6;
vec3 S0, S1, S2, S3, S4, S5, S6;     // element start points (N side)
vec3 E0, E1, E2, E3, E4, E5, E6;     // element end points (C side)
vec3 B0, B1, B2, B3, B4, B5;         // loop control points
vec3 tNa, tNb, tCa, tCb;             // terminal tails

// ── SDF accumulator ─────────────────────────────────────────────────────
float gD, gRaw, gK, gBR, gMode;
float gRes, gMat, gAtom, gUnit;
void put(float d, float res, float mat, float atom) {
    if (d < gRaw) { gRaw = d; gRes = res; gMat = mat; gAtom = atom; }
    gD = gK > 0.0 ? smin(gD, d, gK) : min(gD, d);
}
float sideType(float h) { return h < 0.55 ? 0.0 : h < 0.77 ? 2.0 : h < 0.93 ? 1.0 : 3.0; }

// iq's exact distance to a quadratic Bezier — returns (distance, t)
vec2 sdBezier(vec3 pos, vec3 A, vec3 B, vec3 C) {
    vec3 a = B - A;
    vec3 b = A - 2.0 * B + C;
    vec3 c = a * 2.0;
    vec3 d = A - pos;
    float kk = 1.0 / max(dot(b, b), 1e-6);
    float kx = kk * dot(a, b);
    float ky = kk * (2.0 * dot(a, a) + dot(d, b)) / 3.0;
    float kz = kk * dot(d, a);
    float p = ky - kx * kx;
    float p3 = p * p * p;
    float q = kx * (2.0 * kx * kx - 3.0 * ky) + kz;
    float h = q * q + 4.0 * p3;
    vec2 res;
    if (h >= 0.0) {
        h = sqrt(h);
        vec2 x = (vec2(h, -h) - q) / 2.0;
        vec2 uv = sign(x) * pow(abs(x), vec2(1.0 / 3.0));
        float t = clamp(uv.x + uv.y - kx, 0.0, 1.0);
        vec3 qq = d + (c + b * t) * t;
        res = vec2(dot(qq, qq), t);
    } else {
        float z = sqrt(-p);
        float v = acos(clamp(q / (p * z * 2.0), -1.0, 1.0)) / 3.0;
        float m = cos(v);
        float n = sin(v) * 1.732050808;
        vec3 t = clamp(vec3(m + m, -n - m, n - m) * z - kx, 0.0, 1.0);
        vec3 qx = d + (c + b * t.x) * t.x; float dx = dot(qx, qx);
        vec3 qy = d + (c + b * t.y) * t.y; float dy = dot(qy, qy);
        res = dx < dy ? vec2(dx, t.x) : vec2(dy, t.y);
    }
    return vec2(sqrt(res.x), res.y);
}
vec3 bez(vec3 A, vec3 B, vec3 C, float t) { float s = 1.0 - t; return s * s * A + 2.0 * s * t * B + t * t * C; }

// alpha helix as a coiled ribbon; u = 0 → stretched into a plain tube
float helixRibbon(vec3 q, float L, float u) {
    float r = mix(0.025, 0.24, u);
    float pitch = L / 3.5;
    float a = atan(q.y, q.x);
    float zz = q.z + 0.5 * L;
    float dz = zz - pitch * a / TAU;
    dz = mod(dz + 0.5 * pitch, pitch) - 0.5 * pitch;
    float sl = TAU * r / sqrt(TAU * TAU * r * r + pitch * pitch);
    float w = mix(0.5 * pitch * sl + 0.03, 0.12, u);
    float th = mix(0.035, 0.03, u);
    float rr = mix(0.03, 0.02, u);
    vec2 cs = abs(vec2(length(q.xy) - r, dz * sl)) - vec2(th, w) + rr;
    float d = length(max(cs, 0.0)) + min(max(cs.x, cs.y), 0.0) - rr;
    d = max(d, abs(q.z) - 0.5 * L);
    return d * 0.7;
}
// beta strand as a flat pleated arrow; u = 0 → tube
float strandArrow(vec3 q, float L, float u) {
    float hz = 0.5 * L;
    float ah = 0.36 * u + 0.001;
    float wB = mix(0.045, 0.14, u), wH = mix(0.045, 0.24, u);
    float wz = q.z < hz - ah ? wB : max(wH * clamp((hz - q.z) / ah, 0.0, 1.0), 0.045 * (1.0 - u));
    float pl = 0.012 * u * sin(q.z * TAU / 0.34 + 1.0);
    float th = mix(0.045, 0.032, u);
    float rr = mix(0.04, 0.016, u);
    vec3 b = vec3((abs(q.x) - wz) * 0.8, abs(q.y - pl) - th, abs(q.z) - hz) + rr;
    return (length(max(b, 0.0)) + min(max(b.x, max(b.y, b.z)), 0.0) - rr) * 0.85;
}

void helixAtoms(vec3 q, float L, float u, float idx, float ra, float rs) {
    float r = mix(0.025, 0.24, u);
    float pitch = L / 3.5;
    float a0 = TAU / 10.8;                     // 3 backbone atoms × 3.6 residues per turn
    float ang = atan(q.y, q.x);
    float zz = q.z + 0.5 * L;
    float turn = floor((TAU * zz / pitch - ang) / TAU + 0.5);
    float k = floor((ang + TAU * turn) / a0 + 0.5);
    for (int j = -1; j <= 1; j++) {
        float kk = clamp(k + float(j), 0.0, 37.0);
        float ak = kk * a0;
        vec3 c = vec3(cos(ak) * r, sin(ak) * r, pitch * kk / 10.8 - 0.5 * L);
        put(length(q - c) - ra, (idx + 0.15 + 0.7 * kk / 37.0) / 7.0, 0.0, mod(kk, 3.0) < 0.5 ? 1.0 : 0.0);
    }
    float m = floor((k - 1.0) / 3.0 + 0.5);
    for (int j = -1; j <= 1; j++) {
        float mm = clamp(m + float(j), 0.0, 12.0);
        float kk = 3.0 * mm + 1.0;
        float ak = kk * a0;
        float hs = h11(mm * 3.7 + idx * 17.0);
        float sr = (0.1 + 0.09 * hs) * rs;
        vec3 c = vec3(vec2(cos(ak), sin(ak)) * (r + 0.13 + sr * 0.7), pitch * kk / 10.8 - 0.5 * L);
        put(length(q - c) - sr, (idx + 0.15 + 0.7 * kk / 37.0) / 7.0, 0.0, sideType(h11(mm * 1.3 + idx * 5.0 + 0.5)));
    }
}
void strandAtoms(vec3 q, float L, float u, float idx, float ra, float rs) {
    float n = floor(L / 0.125);
    float sp = L / n;
    float k = floor((q.z + 0.5 * L) / sp + 0.5);
    for (int j = -1; j <= 1; j++) {
        float kk = clamp(k + float(j), 0.0, n);
        vec3 c = vec3(0.0, 0.035 * (mod(kk, 2.0) * 2.0 - 1.0) * u, kk * sp - 0.5 * L);
        put(length(q - c) - ra, (idx + 0.15 + 0.7 * kk / n) / 7.0, 1.0, mod(kk, 3.0) < 0.5 ? 1.0 : 0.0);
    }
    float m = floor((k - 1.0) / 3.0 + 0.5);
    float mMax = floor((n - 1.0) / 3.0);
    for (int j = -1; j <= 1; j++) {
        float mm = clamp(m + float(j), 0.0, mMax);
        float kk = 3.0 * mm + 1.0;
        float hs = h11(mm * 3.7 + idx * 17.0);
        float sr = (0.1 + 0.09 * hs) * rs;
        float sd = mod(mm, 2.0) * 2.0 - 1.0;
        vec3 c = vec3(0.0, sd * (0.13 + sr * 0.7), kk * sp - 0.5 * L);
        put(length(q - c) - sr, (idx + 0.15 + 0.7 * kk / n) / 7.0, 1.0, sideType(h11(mm * 1.3 + idx * 5.0 + 0.5)));
    }
}

void elem(vec3 p, vec3 C, vec3 D, vec3 U, vec3 V, float L, float u, float isH, float idx) {
    vec3 pc = p - C;
    float az = dot(pc, D);
    float db = length(pc - D * clamp(az, -0.5 * L, 0.5 * L)) - gBR;
    if (db > gD + gK + 0.05) return;
    vec3 q = vec3(dot(pc, U), dot(pc, V), az);
    if (gMode < 0.5 || gMode > 2.5) {
        float d = isH > 0.5 ? helixRibbon(q, L, u) : strandArrow(q, L, u);
        if (gMode > 2.5) d -= 0.07;
        put(d, (idx + 0.15 + 0.7 * clamp(az / L + 0.5, 0.0, 1.0)) / 7.0, isH > 0.5 ? 0.0 : 1.0, 0.0);
    } else {
        float ra = gMode < 1.5 ? 0.13 : 0.2;
        float rs = gMode < 1.5 ? 1.0 : 1.35;
        if (isH > 0.5) helixAtoms(q, L, u, idx, ra, rs);
        else strandAtoms(q, L, u, idx, ra, rs);
    }
}

void loopSeg(vec3 p, vec3 A, vec3 B, vec3 Cc, float idx) {
    vec3 m = (A + 2.0 * B + Cc) * 0.25;
    float br = max(length(A - m), length(Cc - m)) + gBR;
    if (length(p - m) - br > gD + gK + 0.05) return;
    float r0 = max(idx + 0.85, 0.0) / 7.0, r1 = min(idx + 1.15, 7.0) / 7.0;
    vec2 bz = sdBezier(p, A, B, Cc);
    if (gMode < 0.5 || gMode > 2.5) {
        float d = bz.x - 0.05;
        if (gMode > 2.5) d -= 0.07;
        put(d, mix(r0, r1, bz.y), 2.0, 0.0);
    } else {
        float ra = gMode < 1.5 ? 0.13 : 0.2;
        float rs = gMode < 1.5 ? 1.0 : 1.35;
        float n = max(floor((length(B - A) + length(Cc - B)) / 0.125), 3.0);
        float k = floor(bz.y * n + 0.5);
        for (int j = -1; j <= 1; j++) {
            float kk = clamp(k + float(j), 0.0, n);
            float t = kk / n;
            put(length(p - bez(A, B, Cc, t)) - ra, mix(r0, r1, t), 2.0, mod(kk, 3.0) < 0.5 ? 1.0 : 0.0);
        }
        float mres = floor((k - 1.0) / 3.0 + 0.5);
        float mMax = floor((n - 1.0) / 3.0);
        for (int j = -1; j <= 1; j++) {
            float mm = clamp(mres + float(j), 0.0, mMax);
            float t = (3.0 * mm + 1.0) / n;
            float hs = h11(mm * 3.7 + idx * 17.0 + 3.0);
            float sr = (0.1 + 0.09 * hs) * rs;
            vec3 c = bez(A, B, Cc, t) + hdir(mm * 2.3 + idx * 11.0) * (0.13 + sr * 0.7);
            put(length(p - c) - sr, mix(r0, r1, t), 2.0, sideType(h11(mm * 1.3 + idx * 5.0 + 7.5)));
        }
    }
}

// element / loop lookups by index (keeps one compiled copy of elem / loopSeg)
void elemAt(float i, out vec3 C, out vec3 D, out vec3 U, out vec3 V, out float L, out float u) {
    if (i < 0.5)      { C = C0; D = D0; U = U0; V = V0; L = L0; u = u0; }
    else if (i < 1.5) { C = C1; D = D1; U = U1; V = V1; L = L1; u = u1; }
    else if (i < 2.5) { C = C2; D = D2; U = U2; V = V2; L = L2; u = u2; }
    else if (i < 3.5) { C = C3; D = D3; U = U3; V = V3; L = L3; u = u3; }
    else if (i < 4.5) { C = C4; D = D4; U = U4; V = V4; L = L4; u = u4; }
    else if (i < 5.5) { C = C5; D = D5; U = U5; V = V5; L = L5; u = u5; }
    else              { C = C6; D = D6; U = U6; V = V6; L = L6; u = u6; }
}
void loopAt(float k, out vec3 A, out vec3 B, out vec3 Cc) {
    if (k < 0.5)      { A = tNa; B = tNb; Cc = S0; }
    else if (k < 1.5) { A = E0; B = B0; Cc = S1; }
    else if (k < 2.5) { A = E1; B = B1; Cc = S2; }
    else if (k < 3.5) { A = E2; B = B2; Cc = S3; }
    else if (k < 4.5) { A = E3; B = B3; Cc = S4; }
    else if (k < 5.5) { A = E4; B = B4; Cc = S5; }
    else if (k < 6.5) { A = E5; B = B5; Cc = S6; }
    else              { A = E6; B = tCa; Cc = tCb; }
}
void protein(vec3 p) {
    vec3 C, D, U, V; float L, u;
    for (int i = 0; i < 7; i++) {
        float fi = float(i);
        elemAt(fi, C, D, U, V, L, u);
        float isH = (i == 0 || i == 3 || i == 6) ? 1.0 : 0.0;
        elem(p, C, D, U, V, L, u, isH, fi);
    }
    vec3 A, B, Cc;
    for (int k = 0; k < 8; k++) {
        float fk = float(k);
        loopAt(fk, A, B, Cc);
        loopSeg(p, A, B, Cc, fk - 1.0);
    }
}

// binding pockets (folded frame) for the molecular surface
vec3 pocketPos(float i) {
    return i < 0.5 ? vec3(0.0, 0.15, 1.2) : i < 1.5 ? vec3(1.0, -0.25, -0.35) : vec3(-0.95, 0.45, -0.55);
}

float capsidForm(vec3 p) {
    {
        // ── Virus capsid: fold space into the icosahedral fundamental domain ──
        gMode = 3.0; gK = 0.1; gBR = 0.5;
        vec3 nc = vec3(-0.5, -0.809017, 0.309017);
        float id = 0.0;
        vec3 q = p;
        for (int i = 0; i < 5; i++) {
            vec2 sg = step(q.xy, vec2(0.0));
            q.xy = abs(q.xy);
            float dn = dot(q, nc);
            float rf = step(dn, 0.0);
            q -= 2.0 * min(0.0, dn) * nc;
            id = id * 8.0 + sg.x + 2.0 * sg.y + 4.0 * rf;
        }
        float hu = h11(id * 0.0137 + 0.5);
        gUnit = hu;
        vec3 Z = vec3(0.18406, 0.12492, 0.97494);
        vec3 X = normalize(vec3(0.0, 0.0, 1.0) - Z * Z.z);
        vec3 Y = cross(Z, X);
        float asmb = smoothstep(0.1 + 0.35 * hu, 0.55 + 0.35 * hu, gF);
        float Rc = 2.05 * (1.0 + 0.035 * bassP() * audioReact) * (1.0 - 0.08 * gBuild)
                 + (1.0 - asmb) * 1.3 + gDrop * (0.8 + 1.0 * hu);
        vec3 lp = q - Z * Rc;
        float sc = 0.33;
        vec3 uq = vec3(dot(lp, X), dot(lp, Y), dot(lp, Z)) / sc;
        uq.xy = rot(0.6) * uq.xy;
        protein(uq);
        return gD * sc * 0.75;
    }
    return 0.0;
}
float mapForm(vec3 p, float f) {
    gD = 1e3; gRaw = 1e3; gUnit = -1.0; gRes = 0.0; gMat = 2.0; gAtom = 0.0;
    if (f > 2.5) return capsidForm(p);
    gMode = f;
    gK = f > 1.5 ? 0.26 : 0.0;
    gBR = f < 0.5 ? 0.42 : 0.8;
    vec3 pp = p / gScale;
    protein(pp);
    float d = gD;
    if (f > 1.5) {
        d -= 0.03;
        d += 0.012 * sin(9.0 * pp.x + gT) * sin(8.0 * pp.y - gT * 0.7) * sin(7.0 * pp.z + gT * 0.5);
        float pr = (0.34 + 0.06 * bassP() * audioReact) * gFoldW;
        for (int i = 0; i < 3; i++) {
            vec3 pk = pocketPos(float(i));
            d = smax(d, -(length(pp - pk) - pr), 0.09);
            float dl = length(pp - pk * 0.9 - 0.04 * vec3(sin(gT * 1.3 + float(i)), cos(gT * 1.1 + float(i) * 2.0), 0.0)) - 0.12 * gFoldW;
            if (dl < d) { gMat = 4.0; gRes = float(i) / 3.0; gRaw = dl; }
            d = min(d, dl);
        }
    }
    return d * gScale;
}

float gFA, gFB, gBl, gHitForm;
float map(vec3 p) {
    float best = 1e3, r = 0.0, m = 2.0, a = 0.0, un = -1.0, hf = gFA;
    for (int k = 0; k < 2; k++) {
        if (k == 1 && gBl < 0.001) break;
        float f = k == 0 ? gFA : gFB;
        float d = mapForm(p, f);
        d += k == 0 ? smoothstep(0.35, 1.0, gBl) * 0.3 : (1.0 - smoothstep(0.0, 0.65, gBl)) * 0.3;
        if (d < best) { best = d; r = gRes; m = gMat; a = gAtom; un = gUnit; hf = f; }
    }
    gRes = r; gMat = m; gAtom = a; gUnit = un; gHitForm = hf;
    return best;
}

vec3 calcNormal(vec3 p) {
    vec3 n = vec3(0.0);
    for (int i = 0; i < 4; i++) {
        vec3 e = 0.5773 * (2.0 * vec3(float(((i + 3) / 2) - 2 * ((i + 3) / 4)), float((i / 2) - 2 * (i / 4)), float(i - 2 * (i / 2))) - 1.0);
        n += e * map(p + e * 0.0015);
    }
    return normalize(n);
}
vec3 chainPath(float s) {
    float t = gT * 0.35;
    float wg = wriggle * mix(1.0, 0.45, gRest);
    return vec3((s / 17.9 - 0.5) * 5.6 + 0.25 * sin(s * 0.9 + t),
                (0.62 * sin(s * 1.05 + t * 1.3) + 0.25 * sin(s * 2.2 - t * 0.9)) * wg,
                (0.6 * cos(s * 0.85 - t) + 0.2 * sin(s * 1.9 + t * 0.7)) * wg);
}

void elemState(float i, vec3 fC, vec3 fD, vec3 fU, float fL, float s0, float s1, float isH,
               out vec3 C, out vec3 D, out vec3 U, out vec3 V, out float L, out float u) {
    vec3 a = chainPath(s0), b = chainPath(s1);
    vec3 ab = b - a;
    float uL = length(ab);
    vec3 uD = ab / uL;
    vec3 uU = normalize(cross(uD, vec3(0.28, 0.19, 0.94)));
    float hi = h11(i * 7.13 + 1.7);
    float w = smoothstep(0.12 + 0.12 * hi, 0.74 + 0.14 * hi, gF);
    u = smoothstep(0.02 + 0.05 * i, 0.4 + 0.05 * i, gF) * (1.0 - 0.75 * gDrop);
    vec3 ax = hdir(i * 3.3 + 2.0);
    float tumble = sin(PI * w) * (1.3 + 0.7 * hi);
    vec3 fD2 = rotA(fD, ax, tumble);
    vec3 fU2 = rotA(fU, ax, tumble);
    D = normalize(mix(uD, fD2, w) + fU * 0.003);
    vec3 Ur = mix(uU, fU2, w);
    U = normalize(Ur - D * dot(Ur, D) + 1e-4);
    V = cross(D, U);
    L = isH > 0.5 ? mix(uL, fL, u) : mix(uL, fL, w);
    vec3 bdir = normalize(fC + ax * 0.7);
    C = mix(0.5 * (a + b), fC, w) + ax * sin(PI * w) * 0.6 + bdir * gDrop * (0.8 + 0.6 * hi)
        + gBuild * gBuild * gBuild * 0.05 * vec3(sin(TIME * 43.0 + i * 5.1), sin(TIME * 37.0 + i * 2.3), sin(TIME * 47.0 + i * 7.7));
}
void ends(vec3 C, vec3 D, vec3 U, float L, float u, float isH, out vec3 S, out vec3 E) {
    float r = isH > 0.5 ? mix(0.025, 0.24, u) : 0.0;
    S = C - D * L * 0.5 + U * r;
    E = C + D * L * 0.5 - U * r;
}
vec3 loopCtl(vec3 E, vec3 Dp, vec3 S, vec3 Dn, vec3 cen) {
    vec3 mid = 0.5 * (E + S);
    vec3 o = mid - cen;
    return mid + (Dp - Dn) * 0.17 + normalize(o + 1e-3) * mix(0.1, 0.24, gFoldW);
}
vec3 strandDir(float x, float sg) { float a = 0.4 * x; return vec3(0.0, sg * cos(a), sg * sin(a)); }

vec3 rainbow(float t) {
    float h = 0.7 * (1.0 - clamp(t, 0.0, 1.0));
    vec3 c = clamp(abs(mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    return mix(vec3(1.0), c, 0.88);
}
vec3 schemeCol(float res, float mat, float unit) {
    float cs = float(colourSel);
    if (cs < 0.5) return rainbow(res);
    if (cs < 1.5) return mat < 0.5 ? vec3(0.95, 0.22, 0.42) : mat < 1.5 ? vec3(1.0, 0.8, 0.22) : vec3(0.35, 0.85, 0.55);
    if (unit >= 0.0) {
        float k = floor(unit * 3.0);
        return k < 0.5 ? vec3(0.3, 0.55, 1.0) : k < 1.5 ? vec3(1.0, 0.42, 0.32) : vec3(0.98, 0.85, 0.45);
    }
    return res < 0.43 ? vec3(0.3, 0.6, 1.0) : vec3(1.0, 0.55, 0.25);
}
float shellOf(float f) { return f < 0.5 ? 0.0 : f < 1.5 ? 1.0 : 0.0; }

void renderScene() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    gT = TIME * 0.6 + (audioMidTime * 0.4 + audioBassTime * 0.3) * ar + 1.5 * gBuild * gBuild;

    // ── fold cycle & form morph ──
    float ph = TIME / morphPeriod;
    float fp = fract(ph);
    float fsel = float(formSel);
    if (fsel < 0.5) { gFA = mod(floor(ph), 4.0); gFB = mod(floor(ph) + 1.0, 4.0); gBl = smoothstep(0.9, 1.0, fp); }
    else { gFA = fsel - 1.0; gFB = gFA; gBl = 0.0; }
    float fold = smoothstep(0.05, 0.46, fp) * (1.0 - smoothstep(0.76, 0.92, fp));
    fold = mix(fold, 0.55 + 0.45 * fold, gRest);
    // beats snap the folding forward in steps
    float st = (floor(fold * 6.0) + smoothstep(0.55, 1.0, fract(fold * 6.0))) / 6.0;
    fold = mix(fold, st, 0.6 * ar * smoothstep(0.15, 0.7, audioEnergy));
    fold = clamp(fold + 0.05 * audioBeatPulse * ar * (1.0 - fold), 0.0, 1.0);
    gF = fold;
    gFoldW = smoothstep(0.75, 1.0, gF) * (1.0 - gDrop);
    gScale = (1.0 + 0.055 * bassP() * ar * (0.5 + gLift)) * (1.0 - 0.13 * gBuild);

    // ── chain: H1 S1 S2 H2 S3 S4 H3 (a small α/β fold around a twisted 4-strand sheet) ──
    vec3 hz = vec3(0.0, 0.0, 1.0);
    elemState(0.0, vec3(-0.45, 0.05, 0.82), normalize(vec3(-0.3, -1.0, 0.1)), hz, 1.9, 0.5, 3.1, 1.0, C0, D0, U0, V0, L0, u0);
    elemState(1.0, vec3(-0.57, 0.0, 0.0), strandDir(-0.57, 1.0), vec3(1.0, 0.0, 0.0), 1.45, 3.55, 5.15, 0.0, C1, D1, U1, V1, L1, u1);
    elemState(2.0, vec3(-0.19, 0.0, 0.0), strandDir(-0.19, -1.0), vec3(1.0, 0.0, 0.0), 1.75, 5.6, 7.2, 0.0, C2, D2, U2, V2, L2, u2);
    elemState(3.0, vec3(0.45, -0.05, 0.82), normalize(vec3(-0.3, 1.0, -0.1)), hz, 1.9, 7.65, 10.25, 1.0, C3, D3, U3, V3, L3, u3);
    elemState(4.0, vec3(0.19, 0.0, 0.0), strandDir(0.19, -1.0), vec3(1.0, 0.0, 0.0), 1.75, 10.7, 12.3, 0.0, C4, D4, U4, V4, L4, u4);
    elemState(5.0, vec3(0.57, 0.0, 0.0), strandDir(0.57, 1.0), vec3(1.0, 0.0, 0.0), 1.45, 12.75, 14.35, 0.0, C5, D5, U5, V5, L5, u5);
    elemState(6.0, vec3(0.05, 0.0, -0.85), normalize(vec3(0.25, -1.0, 0.0)), vec3(1.0, 0.0, 0.0), 2.1, 14.8, 17.4, 1.0, C6, D6, U6, V6, L6, u6);
    ends(C0, D0, U0, L0, u0, 1.0, S0, E0);
    ends(C1, D1, U1, L1, u1, 0.0, S1, E1);
    ends(C2, D2, U2, L2, u2, 0.0, S2, E2);
    ends(C3, D3, U3, L3, u3, 1.0, S3, E3);
    ends(C4, D4, U4, L4, u4, 0.0, S4, E4);
    ends(C5, D5, U5, L5, u5, 0.0, S5, E5);
    ends(C6, D6, U6, L6, u6, 1.0, S6, E6);
    vec3 cen = (C0 + C1 + C2 + C3 + C4 + C5 + C6) / 7.0;
    B0 = loopCtl(E0, D0, S1, D1, cen);
    B1 = loopCtl(E1, D1, S2, D2, cen);
    B2 = loopCtl(E2, D2, S3, D3, cen);
    B3 = loopCtl(E3, D3, S4, D4, cen);
    B4 = loopCtl(E4, D4, S5, D5, cen);
    B5 = loopCtl(E5, D5, S6, D6, cen);
    float wT = smoothstep(0.2, 0.8, gF);
    tNa = mix(chainPath(0.0), S0 - D0 * 0.55 + V0 * 0.35, wT);
    tNb = mix(chainPath(0.25), S0 - D0 * 0.25 + V0 * 0.05, wT) + 0.02;
    tCa = mix(chainPath(17.65), E6 + D6 * 0.3, wT) + 0.02;
    tCb = mix(chainPath(17.9), E6 + D6 * 0.6 - V6 * 0.35, wT);

    // ── camera: slow studio orbit ──
    float capW = (gFA > 2.5 ? 1.0 - gBl : 0.0) + (gFB > 2.5 ? gBl : 0.0);
    float camD = mix(7.6, 11.0, capW) * (1.0 - 0.08 * gPeak + 0.06 * gRest) / zoom;
    float az = (TIME * 0.11 + audioMidTime * 0.05 * ar) * spin + 0.6 * gBuild * gBuild;
    float el = 0.32 + 0.16 * sin(TIME * 0.05);
    vec3 ro = vec3(sin(az) * cos(el), sin(el), cos(az) * cos(el)) * camD;
    vec3 ww = normalize(-ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(sp.x * uu + sp.y * vv + 1.9 * ww);
    // studio lights in camera space
    vec3 lKey  = normalize(-uu * 0.55 + vv * 0.75 - ww * 0.45);
    vec3 lFill = normalize(uu * 0.8 + vv * 0.1 - ww * 0.3);
    vec3 lRim  = normalize(uu * 0.2 + vv * 0.5 + ww * 0.9);

    // ── bounding sphere ──
    float Rb = mix(3.7, 3.9, capW) + 1.9 * gDrop;
    float bb = dot(ro, rd);
    float cc = dot(ro, ro) - Rb * Rb;
    float hh = bb * bb - cc;
    vec3 bg = backdrop(uv);
    bg += vec3(0.35, 0.42, 0.62) * exp(-dot(sp, sp) * 3.0) * (0.05 + 0.08 * gPeak) * (transparentBg ? 0.0 : 1.0);
    vec3 col = bg;
    float alpha = 0.0;
    float shellK = mix(shellOf(gFA), shellOf(gFB), gBl) * shellAmt;
    float shellR = gFA < 0.5 ? 0.24 : 0.13;
    float glowAcc = 0.0;
    float hitT = -1.0, shellT = -1.0;
    float hRes = 0.0, hMat = 2.0, hAtom = 0.0, hUnit = -1.0, hForm = 0.0;
    if (hh > 0.0) {
        float tq = sqrt(hh);
        float t = max(-bb - tq, 0.0);
        float tEnd = -bb + tq;
        for (int i = 0; i < 80; i++) {
            vec3 p = ro + rd * t;
            float d = map(p);
            if (shellT < 0.0 && d < shellR) shellT = t;
            if (d < 0.0012 * t) { hitT = t; hRes = gRes; hMat = gMat; hAtom = gAtom; hUnit = gUnit; hForm = gHitForm; break; }
            glowAcc += exp(-d * 9.0) * 0.012;
            t += d * 0.92;
            if (t > tEnd) break;
        }
    }
    float frs = 0.0;
    // both normals (surface hit + glass shell) through ONE calcNormal copy
    vec3 nHit = vec3(0.0, 0.0, 1.0), nShell = vec3(0.0, 0.0, 1.0);
    for (int k = 0; k < 2; k++) {
        bool want = k == 0 ? hitT > 0.0 : (shellK > 0.01 && shellT > 0.0);
        if (!want) continue;
        vec3 nn = calcNormal(ro + rd * (k == 0 ? hitT : shellT));
        if (k == 0) nHit = nn; else nShell = nn;
    }
    if (hitT > 0.0) {
        vec3 p = ro + rd * hitT;
        vec3 n = nHit;
        float res = hRes, mat = hMat, atm = hAtom, unit = hUnit, form = hForm;
        // cheap occlusion & shadow (no extra scene evaluations → a much smaller shader):
        // crevices facing the molecule's centre are darker, light from the key falls off on the far side
        float cdist = length(p) / max(gScale, 0.5);
        float ao = mix(1.0, clamp(0.35 + 0.65 * (0.5 + 0.5 * dot(n, normalize(p + 1e-4))) * smoothstep(0.2, 1.8, cdist), 0.0, 1.0), aoAmt);
        float sh = clamp(0.45 + 0.55 * smoothstep(-0.4, 0.6, dot(normalize(p + 1e-4), lKey)) + 0.3 * smoothstep(1.2, 2.2, cdist), 0.0, 1.0);
        vec3 base = schemeCol(res, mat, unit);
        float spE = 55.0, spK = 0.75;
        if (form > 0.5 && form < 1.5) {               // space-fill: CPK heteroatoms, carbons tinted by scheme
            base = atm < 0.5 ? mix(base, vec3(0.82), 0.3) : atm < 1.5 ? vec3(0.22, 0.38, 1.0) : atm < 2.5 ? vec3(1.0, 0.16, 0.12) : vec3(1.0, 0.85, 0.22);
            spE = 32.0; spK = 0.55;
        }
        frs = pow(1.0 - clamp(dot(n, -rd), 0.0, 1.0), 3.0);
        if (form > 1.5 && form < 2.5) {               // molecular surface: thin-film iridescence
            vec3 ir = 0.5 + 0.5 * cos(TAU * (frs * 1.3 + dot(n, vec3(0.25, 0.4, 0.15)) + gT * 0.04 + vec3(0.0, 0.33, 0.67)));
            base = mix(base, ir, 0.45 + 0.4 * frs);
            spE = 90.0; spK = 1.0;
        }
        if (form > 2.5) { spE = 40.0; spK = 0.6; }
        // saturation & brightness follow the show state
        base = mix(vec3(lum(base)), base, mix(0.5, 1.05, gLift));
        base = mix(base, vec3(0.9), 0.15 * gRest);
        float dif = clamp(dot(n, lKey), 0.0, 1.0) * sh;
        float fil = clamp(0.5 + 0.5 * dot(n, lFill), 0.0, 1.0);
        float rimL = pow(clamp(dot(n, lRim), 0.0, 1.0), 2.0) * frs;
        vec3 hv = normalize(lKey - rd);
        float spec = pow(clamp(dot(n, hv), 0.0, 1.0), spE) * sh * spK * gloss;
        // shimmering sweep highlight (faster during the build)
        vec3 lSw = normalize(uu * cos(gT * 0.7) + vv * 0.6 + -ww * sin(gT * 0.7));
        spec += pow(clamp(dot(reflect(rd, n), lSw), 0.0, 1.0), 40.0) * 0.35 * gloss * (0.3 + gLift);
        col = base * (0.16 * ao + 0.95 * dif + 0.32 * fil * ao) + vec3(1.0) * spec + vec3(0.7, 0.8, 1.0) * rimL * (0.4 + 0.6 * gLift) * ao;
        col *= mix(0.85, 1.05, gLift);
        if (mat > 3.5) col = mix(col, vec3(0.4, 1.0, 0.75) * (0.75 + 0.6 * audioBeatPulse * ar) + spec, 0.7);   // glowing ligand
        // light pulse running N→C along the chain
        float pz = fract(gT * 0.16);
        float pul = exp(-pow((res - pz) * 16.0, 2.0)) * (0.1 + 1.1 * gPeak + 0.5 * audioBeatPulse * ar) * pulseAmt;
        col += (base * 1.2 + 0.25) * pul;
        // depth fog
        col = mix(col, bg, 1.0 - exp(-max(hitT - camD + 1.2, 0.0) * 0.14));
        alpha = 1.0;
    }
    // translucent glass shell around atoms / ribbons
    if (shellK > 0.01 && shellT > 0.0) {
        vec3 ps = ro + rd * shellT;
        vec3 ns = nShell;
        float fs = pow(1.0 - clamp(dot(ns, -rd), 0.0, 1.0), 2.5);
        vec3 tint = mix(vec3(0.55, 0.75, 1.0), schemeCol(hRes, hMat, hUnit), 0.35);
        float sspec = pow(clamp(dot(ns, normalize(lKey - rd)), 0.0, 1.0), 70.0);
        float sa = clamp(shellK * (0.1 + 0.75 * fs), 0.0, 0.85);
        col = col * (1.0 - 0.3 * sa) + tint * (0.15 + 0.85 * fs) * sa + vec3(1.0) * sspec * 0.5 * shellK;
        alpha = max(alpha, sa);
    }
    // soft halo
    col += vec3(0.4, 0.55, 1.0) * glowAcc * (0.06 + 0.35 * gPeak + 0.6 * gDrop);
    col += vec3(1.0) * gDrop * gDrop * 0.12;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    float vig = 1.0 - smoothstep(0.6, 1.3, length(sp * vec2(0.8, 1.0)));
    col *= mix(1.0, vig, 0.5);
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), clamp(alpha + lum(col), 0.0, 1.0));
}

void main() {
    if (PASSINDEX == 0) { renderScene(); return; }
    // final: upscale the scene buffer, then grain + palette at full resolution
    vec2 fc = gl_FragCoord.xy;
    vec4 sc = texture2D(sceneBuf, fc / RENDERSIZE.xy);
    vec3 col = sc.rgb;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(col, transparentBg ? sc.a : 1.0);
}
