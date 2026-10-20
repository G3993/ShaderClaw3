/*{
  "DESCRIPTION": "Bismuth — an alien hopper-crystal megastructure floating in deep space. A vast square funnel of stair-stepped terraces (the way real bismuth grows, edges outrunning faces) in dark iridescent metal: every terrace carries a different oxide thickness so the steps shift violet, teal, gold and magenta as the camera orbits. A plasma core burns at the bottom of the pit and lights the funnel walls from below; circuit seams glow along every terrace edge with pulses running round them; a scanner plane sweeps up the structure; newly grown terraces come in white-hot and cool to colour; smaller shard crystals drift around the mother structure. Volumetric fog fills the pit and a column of light climbs out of it; stars and nebula behind. Bass drives the core and the seams, mids shift the oxide colours, highs sparkle the edges, beats fire a pulse ring out through the terraces.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-14 — original (v2, science-fiction rebuild). CIDKID chemistry pack #3",
  "CATEGORIES": ["Generator", "3D", "Chemistry", "Sci-Fi", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Growth Speed",      "TYPE": "float", "MIN": 0.0,   "MAX": 3.0,   "DEFAULT": 1.0,   "GROUP": "Speed" },
    { "NAME": "orbitSpeed",  "LABEL": "Camera Orbit",      "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 0.5,   "GROUP": "Speed" },
    { "NAME": "pulseSpeed",  "LABEL": "Pulse Speed",       "TYPE": "float", "MIN": 0.0,   "MAX": 3.0,   "DEFAULT": 1.0,   "GROUP": "Speed" },
    { "NAME": "colorCycle",  "LABEL": "Oxide Drift",       "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 0.4,   "GROUP": "Speed" },
    { "NAME": "ringSpin",    "LABEL": "Ring Spin",         "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 0.4,   "GROUP": "Speed" },
    { "NAME": "shards",      "LABEL": "Shards",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 5.0,   "DEFAULT": 4.0,   "GROUP": "Crystal" },
    { "NAME": "size",        "LABEL": "Size",              "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Crystal" },
    { "NAME": "terraces",    "LABEL": "Terraces",          "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 16.0,  "DEFAULT": 11.0,  "GROUP": "Crystal" },
    { "NAME": "stepSize",    "LABEL": "Terrace Width",     "TYPE": "float", "AUDIO": false, "MIN": 0.06,"MAX": 0.3,   "DEFAULT": 0.1,   "GROUP": "Crystal" },
    { "NAME": "stepHeight",  "LABEL": "Terrace Height",    "TYPE": "float", "AUDIO": false, "MIN": 0.03,"MAX": 0.25,  "DEFAULT": 0.13,  "GROUP": "Crystal" },
    { "NAME": "twist",       "LABEL": "Twist",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.5,   "GROUP": "Crystal" },
    { "NAME": "tiltAmt",     "LABEL": "Shard Tilt",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.7,   "GROUP": "Crystal" },
    { "NAME": "spread",      "LABEL": "Shard Spread",      "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Crystal" },
    { "NAME": "cycle",       "LABEL": "Life Cycle (s)",    "TYPE": "float", "AUDIO": false, "MIN": 10.0,"MAX": 120.0, "DEFAULT": 45.0,  "GROUP": "Crystal" },
    { "NAME": "oxide",       "LABEL": "Iridescence",       "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.85,  "GROUP": "Color" },
    { "NAME": "thickness",   "LABEL": "Oxide Thickness",   "TYPE": "float", "MIN": 120.0, "MAX": 700.0, "DEFAULT": 330.0, "GROUP": "Color" },
    { "NAME": "thickDrift",  "LABEL": "Thickness Per Step","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.5,   "GROUP": "Color" },
    { "NAME": "metalColor",  "LABEL": "Metal Tint",        "TYPE": "color", "DEFAULT": [0.42, 0.43, 0.5, 1.0], "GROUP": "Color" },
    { "NAME": "glowColor",   "LABEL": "Seam Glow",         "TYPE": "color", "DEFAULT": [0.25, 0.9, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "coreColor",   "LABEL": "Core Colour",       "TYPE": "color", "DEFAULT": [1.0, 0.78, 0.5, 1.0],  "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.0,   "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0,   "MAX": 1.5,   "DEFAULT": 1.0,   "GROUP": "Color" },
    { "NAME": "backColor",   "LABEL": "Background Color",  "TYPE": "color", "DEFAULT": [0.004, 0.005, 0.012, 1.0], "GROUP": "Color" },
    { "NAME": "backAmt",     "LABEL": "Background Amount", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 1.0,   "GROUP": "Color" },
    { "NAME": "stars",       "LABEL": "Stars",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.6,   "GROUP": "Color" },
    { "NAME": "seams",       "LABEL": "Circuit Seams",     "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Light" },
    { "NAME": "coreLight",   "LABEL": "Core Light",        "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Light" },
    { "NAME": "keyLight",    "LABEL": "Key Light",         "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Light" },
    { "NAME": "lightAz",     "LABEL": "Light Direction",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 360.0, "DEFAULT": 150.0, "GROUP": "Light" },
    { "NAME": "lightEl",     "LABEL": "Light Height",      "TYPE": "float", "AUDIO": false, "MIN": 10.0,"MAX": 80.0,  "DEFAULT": 56.0,  "GROUP": "Light" },
    { "NAME": "fog",         "LABEL": "Fog",               "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Light" },
    { "NAME": "scan",        "LABEL": "Scanner Sweep",     "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0,   "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 0.9,   "GROUP": "Finish" },
    { "NAME": "streak",      "LABEL": "Anamorphic Streak", "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 0.7,   "GROUP": "Finish" },
    { "NAME": "motes",       "LABEL": "Dust Motes",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.6,   "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.25,  "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.2,   "GROUP": "Finish" },
    { "NAME": "marchRes",    "LABEL": "March Resolution",  "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0,   "DEFAULT": 0.75,  "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 0.6,   "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",   "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "bmScene", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Bismuth v2 — CIDKID chemistry pack #3 (science-fiction rebuild)
//   pass 0  bmScene — raymarched hopper megastructure + shard crystals in a
//                     void: dark iridescent metal (thin-film oxide), circuit
//                     seams with travelling pulses, plasma core lighting the
//                     pit, hot growth frontier, scanner sweep, volumetric fog
//                     + light column, stars/nebula.   rgb = HDR, a = hit
//   pass 1  final   — bloom + anamorphic streak, dust motes, ACES, fringe,
//                     grain, vignette, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define MAXC 6
#define FAR  60.0

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
float fbm2(vec2 p) { float a = 0.5, s = 0.0; for (int i = 0; i < 4; i++) { s += a * vnoise2(p); p = p * 2.03 + 11.0; a *= 0.5; } return s; }
mat3 rotX(float a) { float c = cos(a), s = sin(a); return mat3(1.0, 0.0, 0.0, 0.0, c, s, 0.0, -s, c); }
mat3 rotY(float a) { float c = cos(a), s = sin(a); return mat3(c, 0.0, -s, 0.0, 1.0, 0.0, s, 0.0, c); }
mat3 rotZ(float a) { float c = cos(a), s = sin(a); return mat3(c, s, 0.0, -s, c, 0.0, 0.0, 0.0, 1.0); }
mat3 tr(mat3 m) { return mat3(m[0][0], m[1][0], m[2][0], m[0][1], m[1][1], m[2][1], m[0][2], m[1][2], m[2][2]); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU; float s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

// ── state ────────────────────────────────────────────────────────────────
float g_t, g_ar, g_lens, g_bass, g_mid, g_high, g_beat, g_lev, g_n;
vec3  g_ro, g_uu, g_vv, g_ww, g_back, g_key, g_keyC, g_glow, g_coreC, g_corePos;
float g_keyI, g_coreI, g_scanY, g_pulseR;

// per crystal (0 = mother structure, 1.. = shards)
vec3  g_pos[MAXC];
mat3  g_rotI[MAXC];
float g_sz[MAXC], g_asp[MAXC], g_K[MAXC], g_bnd[MAXC], g_thk[MAXC], g_ramp[MAXC], g_life[MAXC], g_hit[MAXC], g_full[MAXC], g_heat[MAXC];
float g_ext, g_top, g_rimW;
vec3 c_pos; mat3 c_rotI; float c_sz, c_asp, c_K, c_thk, c_ramp, c_heat;
#define MAXK 16

// life curve: grow → hold → quick retract
float growthOf(float u, out float dir) {
    float g = smoothstep(0.0, 0.6, u);
    g = g * g * (3.0 - 2.0 * g);
    float ret = smoothstep(0.86, 0.985, u);
    dir = (u < 0.6) ? 1.0 : ((u > 0.86) ? -1.0 : 0.0);
    return g * (1.0 - ret);
}

void setupCrystals() {
    float gs = max(speed, 0.0001);
    float per = cycle / gs;
    for (int i = 0; i < MAXC; i++) {
        float fi = float(i);
        bool mother = (i == 0);
        float ph = mother ? 0.0 : fract(fi * 0.37 + 0.12 * hash11(fi * 7.31 + 1.7));
        float lt = g_t / (mother ? per : per * 0.55) + ph;
        float u = fract(lt);
        float gen = floor(lt);
        float h1 = hash11(gen * 3.17 + fi * 11.3 + 0.3);
        float h2 = hash11(gen * 5.71 + fi * 13.7 + 0.6);
        float h3 = hash11(gen * 9.13 + fi * 17.1 + 0.9);
        float h4 = hash11(gen * 2.39 + fi * 19.3 + 0.2);
        float dir;
        float g = growthOf(u, dir);
        g_life[i] = g;
        float szFull = mother ? size * 1.0 : size * (0.3 + 0.22 * h3);
        float sz = mother ? szFull : szFull * (0.5 + 0.5 * smoothstep(0.0, 0.25, g));
        float asp = mother ? 0.86 : 0.7 + 0.3 * h4;
        float Kmax = mother ? terraces : 3.0 + floor(h2 * 3.0);
        float K = 1.0 + (Kmax - 1.0) * g;
        g_heat[i] = abs(dir);                                  // frontier is hot while growing / retracting
        float tilt = mother ? 0.0 : tiltAmt * (0.35 + 0.75 * h2);
        float yaw = mother ? g_t * 0.02 : h1 * TAU + g_t * 0.12 * (h3 > 0.5 ? 1.0 : -1.0);
        mat3 rot = rotY(yaw) * rotX(tilt) * rotZ(mother ? 0.0 : (h3 - 0.5) * tiltAmt * 0.9);
        g_rotI[i] = tr(rot);
        g_sz[i] = sz; g_asp[i] = asp; g_K[i] = K;
        g_ramp[i] = (mother ? 1.0 : 0.6 + 0.4 * h4) * (h2 > 0.5 ? 1.0 : -1.0);
        g_thk[i] = thickness * (0.78 + 0.44 * h4) + 70.0 * sin(g_t * 0.21 * colorCycle + fi * 1.7);
        float sTop = (stepHeight + stepHeight * (K - 1.0)) * sz;
        float wOut = (stepSize * 0.9 + stepSize * (K - 1.0) + 2.5 * stepSize) * sz;
        if (mother) {
            g_pos[i] = vec3(0.0);
            g_top = sTop; g_rimW = (stepSize * 0.9 + stepSize * (K - 1.0)) * sz;
        } else {
            // shards drift around the mother structure, bobbing
            float ang = (fi + 0.5) / 5.0 * TAU + h1 * 1.2 + g_t * 0.04 * orbitSpeed * (h2 > 0.5 ? 1.0 : -1.0);
            float rad = spread * g_full[0] * (1.15 + 0.55 * h2);
            float hgt = size * (0.1 + 1.3 * h3) + 0.12 * sin(g_t * 0.5 + fi * 2.1);
            g_pos[i] = vec3(cos(ang) * rad, hgt, sin(ang) * rad);
        }
        g_bnd[i] = sqrt(wOut * wOut * 2.0 + sTop * sTop) * 1.12 + 0.06;
        g_full[i] = (stepSize * 0.9 + stepSize * (Kmax - 1.0) + 2.5 * stepSize) * szFull * 1.2;
        g_hit[i] = (fi < g_n) ? 1.0 : 0.0;
    }
    g_ext = g_full[0] * 0.95 + 0.25 * size;
    g_corePos = vec3(0.0, stepHeight * 0.9 * size + 0.02, 0.0);
}

void loadCrystal(int which) {
    for (int i = 0; i < MAXC; i++) {
        if (i != which) continue;
        c_pos = g_pos[i]; c_rotI = g_rotI[i]; c_sz = g_sz[i]; c_asp = g_asp[i];
        c_K = g_K[i]; c_thk = g_thk[i]; c_ramp = g_ramp[i]; c_heat = g_heat[i];
    }
}

float sdBox(vec3 p, vec3 b) { vec3 q = abs(p) - b; return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0); }

// Hopper crystal = block minus a stack of square pits: solid = U_j skirtBox_j
// minus U_L pitBox_L.  Every ring sits in its own rotated frame (twist per
// level + slow counter-rotation), and each box is an exact SDF, so the
// min/max combination stays a valid bound without any shear.
float rotAngle(float L, float ramp) {
    // bounded: each ring sways a few degrees instead of winding up forever
    return ramp * (twist * 0.028 * L + ringSpin * 0.05 * sin(g_t * (0.07 + 0.013 * L) * (mod(L, 2.0) < 1.0 ? 1.0 : -1.0) + L * 1.7));
}
float box2(vec2 q, vec2 b) { vec2 d = abs(q) - b; return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0); }
vec2 rot2(vec2 q, float a) { float c = cos(a), s = sin(a); return vec2(c * q.x - s * q.y, s * q.x + c * q.y); }

float hopper(vec3 p, float sz, float asp, float K, float ramp, out float level) {
    float s  = stepSize * sz;
    float sh = stepHeight * sz * (1.0 + 0.05 * g_ar * g_bass);
    float w0 = s * 1.7, h0 = sh * 0.9;
    float Ki = floor(K), fr = K - Ki;
    float Kcap = Ki - 1.0 + fr;
    float yRim = h0 + sh * Kcap;
    float wOut = w0 + max(Ki - 1.0, 0.0) * s + fr * s;
    float so = s * 0.8, sho = sh * 0.8;
    // pits
    float dPit = 1e9; level = 0.0;
    for (int L = 0; L < MAXK; L++) {
        float fL = float(L);
        if (fL > Ki - 0.5) break;
        float wL = w0 + fL * s;
        float yL = h0 + sh * fL;
        vec2 q = rot2(p.xz, rotAngle(fL, ramp));
        float d2 = box2(q, vec2(wL, wL * asp));
        float dP = max(d2, yL - p.y);
        if (dP < dPit) { dPit = dP; level = fL; }
    }
    // skirt: 3 stacked boxes, wider and a little lower outward; the widest is the hull down to the base
    float dSk = 1e9;
    for (int j = 0; j < 3; j++) {
        float fj = float(j);
        float wj = wOut + fj * so;
        float yj = yRim - fj * sho;
        vec2 q = rot2(p.xz, rotAngle(Ki + fj, ramp));
        float d2 = box2(q, vec2(wj, wj * asp));
        float dB = max(d2, max(p.y - yj, -0.15 * sz - p.y));
        if (dB < dSk) { dSk = dB; }
    }
    return max(dSk, -dPit) * 0.9;
}

// shading features for a surface point: terrace index, distance to the
// nearest convex edge, coordinate along that edge, position across the
// terrace (0..1), ring radius in the ring's frame, skirt flag
void terraceInfo(vec3 p, float sz, float asp, float K, float ramp,
                 out float level, out float edgeD, out float along, out float across, out float r, out float isSkirt) {
    float s  = stepSize * sz;
    float sh = stepHeight * sz * (1.0 + 0.05 * g_ar * g_bass);
    float w0 = s * 1.7, h0 = sh * 0.9;
    float Ki = floor(K), fr = K - Ki;
    float Kcap = Ki - 1.0 + fr;
    float yRim = h0 + sh * Kcap;
    float wOut = w0 + max(Ki - 1.0, 0.0) * s + fr * s;
    float so = s * 0.8, sho = sh * 0.8;
    // nearest pit box surface
    float best = 1e9; level = 0.0; isSkirt = 0.0;
    for (int L = 0; L < MAXK; L++) {
        float fL = float(L);
        if (fL > Ki - 0.5) break;
        float wL = w0 + fL * s, yL = h0 + sh * fL;
        vec2 q = rot2(p.xz, rotAngle(fL, ramp));
        float dP = abs(max(box2(q, vec2(wL, wL * asp)), yL - p.y));
        if (dP < best) { best = dP; level = fL; }
    }
    for (int j = 0; j < 3; j++) {
        float fj = float(j);
        float wj = wOut + fj * so, yj = yRim - fj * sho;
        vec2 q = rot2(p.xz, rotAngle(Ki + fj, ramp));
        float dB = abs(max(box2(q, vec2(wj, wj * asp)), max(p.y - yj, -0.15 * sz - p.y)));
        if (dB < best - 1e-4) { best = dB; level = Ki + 1.0 + fj; isSkirt = 1.0; }
    }
    float L = level;
    if (isSkirt < 0.5) {
        // floor L spans w_{L-1}..w_L at height y_L; its convex edges: the lip at
        // (w_{L-1}, y_L) in frame L-1 and the riser top at (w_L, y_{L+1}) in frame L
        float wL = w0 + L * s, yL = h0 + sh * L;
        float yUp = (L > Ki - 1.5) ? yRim : h0 + sh * (L + 1.0);
        vec2 qL = rot2(p.xz, rotAngle(L, ramp));
        r = max(abs(qL.x), abs(qL.y) / asp);
        along = (abs(qL.x) > abs(qL.y) / asp) ? qL.y / asp + 7.0 * sign(qL.x) : qL.x + 3.0 * sign(qL.y);
        float dEdgeUp = length(vec2(r - wL, p.y - yUp));
        edgeD = dEdgeUp;
        across = clamp((p.y - yL) / max(yUp - yL, 1e-3), 0.0, 1.0);   // riser default
        if (L > 0.5) {
            float wP = w0 + (L - 1.0) * s;
            vec2 qP = rot2(p.xz, rotAngle(L - 1.0, ramp));
            float rP = max(abs(qP.x), abs(qP.y) / asp);
            float dLip = length(vec2(rP - wP, p.y - yL));
            if (dLip < edgeD) { edgeD = dLip; }
            if (abs(p.y - yL) < 0.02 * sz) {                          // on the floor
                r = rP; along = (abs(qP.x) > abs(qP.y) / asp) ? qP.y / asp + 7.0 * sign(qP.x) : qP.x + 3.0 * sign(qP.y);
                across = clamp((rP - wP) / s, 0.0, 1.0);
            }
        } else {
            across = clamp(r / w0, 0.0, 1.0);
        }
        // outer lip of the rim
        if (L > Ki - 1.5) {
            vec2 qR = rot2(p.xz, rotAngle(Ki, ramp));
            float rR = max(abs(qR.x), abs(qR.y) / asp);
            edgeD = min(edgeD, length(vec2(rR - wOut, p.y - yRim)));
        }
    } else {
        float j = L - Ki - 1.0;
        float wj = wOut + j * so, yj = yRim - j * sho;
        vec2 q = rot2(p.xz, rotAngle(Ki + j, ramp));
        r = max(abs(q.x), abs(q.y) / asp);
        along = (abs(q.x) > abs(q.y) / asp) ? q.y / asp + 7.0 * sign(q.x) : q.x + 3.0 * sign(q.y);
        edgeD = length(vec2(r - wj, p.y - yj));
        across = clamp((r - wj) / so, 0.0, 1.0);
        // the inner lip of this step is the previous box's outer edge at this height
        if (j > 0.5) {
            float wq = wOut + (j - 1.0) * so;
            vec2 q2 = rot2(p.xz, rotAngle(Ki + j - 1.0, ramp));
            float r2 = max(abs(q2.x), abs(q2.y) / asp);
            edgeD = min(edgeD, length(vec2(r2 - wq, p.y - (yRim - (j - 1.0) * sho))));
        }
    }
}

float crystalDist(int i, vec3 pw, out float level) {
    vec3 pl = c_rotI * (pw - c_pos);
    return hopper(pl, c_sz, c_asp, c_K, c_ramp, level);
}

bool crystalBound(int i, vec3 ro, vec3 rd, out float t0, out float t1) {
    vec3 cc; float rr;
    for (int k = 0; k < MAXC; k++) { if (k != i) continue; cc = g_pos[k]; rr = g_bnd[k]; }
    vec3 oc = ro - cc;
    float b = dot(oc, rd);
    float c = dot(oc, oc) - rr * rr;
    float h = b * b - c;
    if (h < 0.0) return false;
    h = sqrt(h);
    t0 = -b - h; t1 = -b + h;
    return t1 > 0.0;
}

float sceneDist(vec3 p, out int hitC) {
    float dm = 1e9; hitC = -1;
    for (int i = 0; i < MAXC; i++) {
        if (g_hit[i] < 0.5) continue;
        c_pos = g_pos[i]; c_rotI = g_rotI[i]; c_sz = g_sz[i]; c_asp = g_asp[i]; c_K = g_K[i]; c_ramp = g_ramp[i];
        float lv;
        float d = crystalDist(i, p, lv);
        if (d < dm) { dm = d; hitC = i; }
    }
    return dm;
}

vec3 sceneNormal(vec3 p, int hc) {
    loadCrystal(hc);
    float lv;
    vec2 e = vec2(0.0015 * c_sz, 0.0);
    return normalize(vec3(crystalDist(hc, p + e.xyy, lv) - crystalDist(hc, p - e.xyy, lv),
                          crystalDist(hc, p + e.yxy, lv) - crystalDist(hc, p - e.yxy, lv),
                          crystalDist(hc, p + e.yyx, lv) - crystalDist(hc, p - e.yyx, lv)));
}

void selectHits(vec3 ro, vec3 rd, float maxT, out float tStart, out float tEnd) {
    tStart = maxT; tEnd = 0.0;
    for (int i = 0; i < MAXC; i++) {
        g_hit[i] = (float(i) < g_n) ? 1.0 : 0.0;
        if (g_hit[i] < 0.5) continue;
        float t0, t1;
        if (!crystalBound(i, ro, rd, t0, t1) || t0 > maxT) { g_hit[i] = 0.0; continue; }
        tStart = min(tStart, max(t0, 0.0)); tEnd = max(tEnd, min(t1, maxT));
    }
}

float marchCrystals(vec3 ro, vec3 rd, float maxT, int steps, out int hitC) {
    float tStart, tEnd;
    selectHits(ro, rd, maxT, tStart, tEnd);
    hitC = -1;
    if (tEnd <= tStart) return -1.0;
    float t = tStart;
    for (int s = 0; s < 110; s++) {
        if (s >= steps) break;
        int hc;
        float d = sceneDist(ro + rd * t, hc);
        if (d < 0.0012 * (1.0 + t * 0.5)) { hitC = hc; return t; }
        t += d;
        if (t > tEnd) break;
    }
    return -1.0;
}

float visibility(vec3 ro, vec3 rd, float maxT, float k) {
    float res = 1.0, t = 0.02, ph = 1e10;
    float tStart, tEnd;
    selectHits(ro, rd, maxT, tStart, tEnd);
    if (tEnd <= 0.0) return 1.0;
    for (int s = 0; s < 30; s++) {
        int hc;
        float d = sceneDist(ro + rd * t, hc);
        if (hc < 0) return res;
        float y = d * d / (2.0 * ph);
        float dd = sqrt(max(d * d - y * y, 0.0));
        res = min(res, k * dd / max(t - y, 0.001));
        ph = d;
        if (res < 0.004) return 0.0;
        t += clamp(d, 0.012, 0.3);
        if (t > maxT) break;
    }
    return clamp(res, 0.0, 1.0);
}

float ambientOcc(vec3 p, vec3 n, float scale) {
    float occ = 0.0, sca = 1.0;
    for (int i = 0; i < MAXC; i++) g_hit[i] = (float(i) < g_n) ? 1.0 : 0.0;
    for (int i = 1; i <= 5; i++) {
        float h = 0.012 + 0.09 * float(i) * scale;
        int hc;
        float d = sceneDist(p + n * h, hc);
        occ += (h - d) * sca;
        sca *= 0.75;
    }
    return clamp(1.0 - 1.6 * occ, 0.0, 1.0);
}

// ── sky: deep space, a cold nebula, stars ────────────────────────────────
vec3 skyBase(vec3 rd) {
    vec3 c = g_back * (0.9 + 0.4 * smoothstep(-0.3, 0.9, rd.y));
    vec2 q = rd.xz / (abs(rd.y) + 0.35);
    float neb = fbm2(q * 1.3 + vec2(0.0, g_t * 0.004));
    neb = smoothstep(0.35, 0.85, neb);
    c += mix(vec3(0.03, 0.015, 0.08), vec3(0.01, 0.05, 0.09), smoothstep(-0.2, 0.6, rd.x)) * neb * 0.22 * backAmt;
    c += vec3(0.04, 0.02, 0.07) * backAmt * pow(max(dot(rd, normalize(vec3(-0.4, 0.3, -1.0))), 0.0), 4.0);
    return c;
}
vec3 skyStars(vec3 rd) {
    vec3 ad = abs(rd);
    vec2 q = (ad.y > ad.x && ad.y > ad.z) ? rd.xz / ad.y + 2.0 : ((ad.x > ad.z) ? rd.yz / ad.x + 5.0 : rd.xy / ad.z + 9.0);
    q *= 90.0;
    vec2 cell = floor(q), f = fract(q);
    float h = hash12(cell);
    vec2 o = vec2(hash12(cell + 7.1), hash12(cell + 3.3)) * 0.8 + 0.1;
    float d = length(f - o);
    float br = smoothstep(0.993, 1.0, h);
    float tw = 0.65 + 0.35 * sin(g_t * (1.5 + 3.0 * hash12(cell + 1.0)) + h * 60.0);
    vec3 tint = mix(vec3(0.7, 0.8, 1.0), vec3(1.0, 0.85, 0.7), hash12(cell + 9.0));
    return tint * br * tw * smoothstep(0.07, 0.0, d) * 2.2;
}
vec3 envCol(vec3 rd) {
    vec3 c = skyBase(rd);
    // one cold panel high behind the camera side + the key as a hot spot
    c += vec3(0.45, 0.6, 0.9) * 0.22 * smoothstep(0.2, 0.9, rd.y) * keyLight;
    c += vec3(1.0, 0.3, 0.7) * 0.5 * pow(max(dot(rd, normalize(vec3(1.0, -0.1, 0.5))), 0.0), 4.0);
    c += vec3(1.0, 0.7, 0.35) * 0.3 * pow(max(dot(rd, normalize(vec3(-0.8, 0.2, 0.4))), 0.0), 5.0);
    float kd = max(dot(rd, g_key), 0.0);
    c += g_keyC * g_keyI * (pow(kd, 200.0) * 1.2 + pow(kd, 10.0) * 0.05);
    return c;
}

// thin-film interference for an oxide skin of d nanometres (n ≈ 2.4)
vec3 thinFilm(float d, float cosT) {
    float n = 2.4;
    float cosT2 = sqrt(max(1.0 - (1.0 - cosT * cosT) / (n * n), 0.0));
    vec3 lam = vec3(640.0, 545.0, 460.0);
    vec3 ph = (4.0 * PI * n * d * cosT2) / lam;
    vec3 r = 0.5 + 0.5 * cos(ph + PI);
    return r * (0.55 + 0.45 * r);
}

// how much of the core a point in free space can see (inside the pit, or in the cone above it)
float pitWeight(vec3 pw) {
    vec3 pl = g_rotI[0] * (pw - g_pos[0]);
    vec2 q = rot2(pl.xz, rotAngle(floor(g_K[0]), g_ramp[0]));
    float r = max(abs(q.x), abs(q.y) / g_asp[0]);
    float allow = g_rimW + max(pl.y - g_top, 0.0) * 1.4;
    float w = smoothstep(allow * 1.05, allow * 0.8, r);
    w *= smoothstep(-0.05, 0.02, pl.y);
    return w;
}

vec3 shadeCrystal(int hc, vec3 p, vec3 rd, vec3 n, bool simple) {
    loadCrystal(hc);
    vec3 pl = c_rotI * (p - c_pos);
    float lv, edgeD, along, across, r, isSkirt;
    terraceInfo(pl, c_sz, c_asp, c_K, c_ramp, lv, edgeD, along, across, r, isSkirt);
    vec3 v = -rd;
    float ndv = max(dot(n, v), 0.0);
    float s = stepSize * c_sz;
    vec3 nl = c_rotI * n;                          // normal in local space
    float isFloor = smoothstep(0.6, 0.95, nl.y);
    float isWall  = 1.0 - smoothstep(0.25, 0.6, abs(nl.y));

    // ── iridescent dark metal ──
    float d = c_thk * (1.0 + thickDrift * (0.16 * lv + 0.25 * (hash11(lv * 3.7 + float(hc) * 5.1) - 0.5))) + 55.0 * g_ar * g_mid;
    float nz = (vnoise2(pl.xz * 5.0 / c_sz + lv * 3.1) + vnoise2(pl.xy * 5.0 / c_sz + 7.0) + vnoise2(pl.zy * 5.0 / c_sz + 3.0)) / 3.0;
    d += (nz - 0.5) * 8.0 + thickDrift * 30.0 * pl.y / c_sz;
    vec3 film = thinFilm(d, ndv);
    vec3 F0 = metalColor.rgb;
    vec3 albedo = F0 * mix(vec3(1.0), film * 1.25 + 0.2, oxide);
    albedo = hueRot(albedo, hueShift);
    // brushed micro-structure: fine lines running round each terrace
    float brush = 0.9 + 0.1 * vnoise2(vec2(along * 90.0 / c_sz, across * 4.0 + lv));
    albedo *= brush;
    vec3 Fr = albedo + (1.0 - albedo) * pow(1.0 - ndv, 5.0) * 0.8;

    // ── lighting ──
    vec3 rr = reflect(rd, n);
    vec3 refl = envCol(rr);
    float ao = simple ? 1.0 : ambientOcc(p, n, 1.0);
    if (!simple) {
        int hc2;
        float t2 = marchCrystals(p + n * 0.004, rr, 10.0, 36, hc2);
        if (t2 > 0.0) {
            vec3 p2 = p + n * 0.004 + rr * t2;
            vec3 n2 = sceneNormal(p2, hc2);
            loadCrystal(hc2);
            vec3 pl2 = c_rotI * (p2 - c_pos);
            float lv2, e2, al2, ac2, r2, sk2;
            terraceInfo(pl2, c_sz, c_asp, c_K, c_ramp, lv2, e2, al2, ac2, r2, sk2);
            float d2 = c_thk * (1.0 + thickDrift * 0.18 * lv2);
            vec3 alb2 = hueRot(F0 * mix(vec3(1.0), thinFilm(d2, max(dot(n2, -rr), 0.0)) * 1.25 + 0.2, oxide), hueShift);
            vec3 h2 = normalize(g_key - rr);
            vec3 c2 = alb2 * envCol(reflect(rr, n2)) * 0.6 + alb2 * g_keyC * g_keyI * pow(max(dot(n2, h2), 0.0), 60.0) * 1.0;
            // seams seen in the reflection
            c2 += g_glow * seams * smoothstep(0.012 * c_sz, 0.0, e2) * 1.5;
            refl = mix(refl, c2, 0.5);
            loadCrystal(hc);
        }
    }
    float sh = simple ? 1.0 : visibility(p + n * 0.006, g_key, 12.0, 5.0);
    vec3 h = normalize(g_key + v);
    float ndl = max(dot(n, g_key), 0.0);
    vec3 col = Fr * refl * (0.35 + 0.65 * ao);
    col += albedo * g_keyC * g_keyI * ndl * 0.35 * (0.3 + 0.7 * sh);
    float specN = pow(max(dot(n, h), 0.0), 120.0);
    float specW = pow(max(dot(n, h), 0.0), 12.0);
    col += g_keyC * g_keyI * (specN * 3.0 + specW * 0.22) * sh * mix(albedo, vec3(1.0), 0.4);

    // plasma core as a point light inside the pit (lights the funnel walls from below)
    vec3 toC = g_corePos - p;
    float dc = length(toC); toC /= max(dc, 1e-4);
    float ndc = max(dot(n, toC), 0.0);
    if (ndc > 0.001 && g_coreI > 0.001) {
        float vis = simple ? 1.0 : visibility(p + n * 0.006, toC, dc - 0.02, 14.0);
        float atten = g_coreI * 0.16 / (0.1 + dc * dc);
        vec3 hc3 = normalize(toC + v);
        col += albedo * g_coreC * ndc * atten * vis;
        col += g_coreC * pow(max(dot(n, hc3), 0.0), 50.0) * atten * vis * 0.8;
    }

    // ── emission: circuit seams, traces, frontier heat, scan, pulse ring ──
    float Ki = floor(c_K);
    float seamW = 0.011 * c_sz;
    float seam = smoothstep(seamW, seamW * 0.25, edgeD);
    // travelling glow round the terraces (outward wave per level, bass lifts it)
    float wave = 0.55 + 0.45 * sin(g_t * 2.0 * pulseSpeed - lv * 0.9 + along * 3.0 / c_sz);
    vec3 filmN = thinFilm(c_thk * (1.0 + thickDrift * 0.2 * lv), 0.85); filmN /= max(max(filmN.r, filmN.g), max(filmN.b, 0.2));
    vec3 seamC = mix(g_glow, filmN * (0.6 + 0.4 * g_glow), 0.45);
    vec3 emis = seamC * seam * seams * (0.7 + 1.3 * wave + 2.0 * g_ar * g_bass);
    // circuit traces across the terrace floors
    float cellA = floor(along / (0.55 * s) + lv * 7.0);
    float gate = step(0.42, hash11(cellA * 3.7 + float(hc) * 13.0 + lv));
    float tr1 = smoothstep(0.03, 0.0, abs(across - 0.5)) * gate;
    float tr2 = smoothstep(0.03, 0.0, abs(across - 0.78)) * step(0.6, hash11(cellA * 1.9 + lv * 2.0 + 4.0));
    float gap = smoothstep(0.08, 0.14, abs(fract(along / (0.55 * s)) - 0.5));
    float trace = (tr1 + tr2) * gap * isFloor * (1.0 - isSkirt);
    float pulse = smoothstep(0.08, 0.0, abs(fract(along / (2.6 * s) - g_t * 0.22 * pulseSpeed * (mod(lv, 2.0) < 1.0 ? 1.0 : -1.0)) - 0.5));
    emis += g_glow * trace * seams * (0.25 + 1.6 * pulse + 0.8 * g_ar * g_high);
    // riser slits
    float cw = floor(along / (0.9 * s) + lv * 0.3), ch = floor(p.y / (0.55 * sh));
    float wh = hash11(cw * 2.1 + ch * 7.3 + lv * 5.0 + float(hc));
    float slit = smoothstep(0.2, 0.12, abs(fract(along / (0.9 * s) + lv * 0.3) - 0.5)) * smoothstep(0.32, 0.22, abs(fract(p.y / (0.55 * sh)) - 0.5)) * step(0.74, wh);
    emis += mix(g_glow, vec3(1.0, 0.8, 0.5), step(0.9, wh)) * slit * isWall * seams * 0.6 * (0.6 + 0.4 * sin(g_t * 2.0 * pulseSpeed + wh * 20.0));
    // hull base: a seam along the bottom edge and a dim vent stripe above it
    if (isSkirt > 0.5) {
        float baseD = abs(p.y + 0.15 * c_sz);
        emis += g_glow * smoothstep(0.012 * c_sz, 0.003 * c_sz, baseD) * seams * 1.2 * isWall;
        float vent = smoothstep(0.03, 0.015, abs(p.y + 0.15 * c_sz - 0.09 * c_sz)) * smoothstep(0.1, 0.3, abs(fract(along / (0.9 * s)) - 0.5));
        emis += g_coreC * vent * isWall * seams * 0.5 * (0.7 + 0.3 * sin(g_t * 1.3 + along * 4.0)) * g_coreI;
    }
    // hull plating: panel grooves on every wall
    float pg = smoothstep(0.035, 0.0, abs(fract(along / (0.9 * s) + lv * 0.3) - 0.5)) + smoothstep(0.035, 0.0, abs(fract(p.y / (0.55 * sh)) - 0.5));
    col *= 1.0 - 0.35 * clamp(pg, 0.0, 1.0) * isWall;
    // growth frontier: the newest terrace comes in white-hot and cools to colour
    float age = (c_K - 1.0 - lv);
    float hot = smoothstep(1.0, 0.0, age) * c_heat * (1.0 - isSkirt) * step(0.5, lv);
    emis += vec3(1.0, 0.7, 0.4) * hot * hot * 2.2 * (0.4 + 0.6 * seam) + g_glow * hot * 0.5;
    // scanner plane sweeping up through everything
    float scanB = smoothstep(0.012, 0.0, abs(p.y - g_scanY)) * scan;
    emis += vec3(0.7, 1.0, 1.0) * scanB * 4.0;
    emis += vec3(0.3, 0.9, 1.0) * smoothstep(0.12, 0.0, g_scanY - p.y) * step(p.y, g_scanY) * scan * 0.35;
    // beat pulse ring running out through the mother structure's terraces
    if (hc == 0) {
        float ring = smoothstep(0.045 * c_sz, 0.0, abs(r - g_pulseR)) * isFloor;
        emis += g_glow * ring * (0.6 + 2.5 * g_ar * g_beat) * seams * 1.5;
    }
    // the pit floor IS the plasma core
    if (hc == 0 && lv < 0.5 && isSkirt < 0.5) {
        float pr = r / (stepSize * c_sz * 1.7);
        float plasma = fbm2(pl.xz * 14.0 / c_sz + vec2(g_t * 0.7, -g_t * 0.5)) * 0.7 + fbm2(pl.xz * 28.0 / c_sz - g_t * 0.9) * 0.3;
        vec3 coreE = g_coreC * (2.2 + 3.0 * plasma) + vec3(1.0) * smoothstep(0.75, 1.0, plasma) * 3.0;
        coreE *= (1.0 - 0.55 * smoothstep(0.55, 1.0, pr)) * (1.0 + 1.2 * g_ar * g_bass + 0.6 * g_ar * g_beat);
        emis += coreE * g_coreI;
    }
    // edge glints that twinkle with the highs
    float edge = 1.0 - smoothstep(0.0, 0.35, abs(nl.y));
    float tw = 0.6 + 0.4 * sin(g_t * 9.0 + lv * 2.0 + along * 30.0 + hash11(lv * 3.0 + float(hc)) * 6.0);
    col += g_keyC * specN * edge * tw * (0.5 + 1.2 * g_ar * g_high) * sh;
    return col + emis;
}

// volumetric fog: analytic height haze + a short march through the core's
// light (inside the pit and the column above it) + the scanner slice
vec3 fogMarch(vec3 ro, vec3 rd, float tMax, vec2 fc, inout float transm) {
    vec3 acc = vec3(0.0);
    // ambient haze (analytic): thin, dark blue, denser low down
    float hMid = ro.y + rd.y * min(tMax, 8.0) * 0.5;
    float amb = fog * (1.0 - exp(-min(tMax, 8.0) * 0.06)) * (0.25 + 0.75 * exp(-max(hMid, 0.0) * 1.2));
    acc += vec3(0.05, 0.09, 0.16) * amb * 0.35 * (0.6 + 0.4 * keyLight);
    acc += g_glow * amb * 0.08 * seams;
    transm *= exp(-amb * 0.35);
    // scanner slice (a glowing plane crossing the ray)
    if (scan > 0.001 && abs(rd.y) > 1e-4) {
        float tS = (g_scanY - ro.y) / rd.y;
        if (tS > 0.0 && tS < tMax) {
            vec3 ps = ro + rd * tS;
            float near = exp(-length(ps.xz) * 0.9);
            acc += vec3(0.5, 1.0, 1.0) * scan * 0.05 * fog * near / (0.15 + abs(rd.y)) * transm;
        }
    }
    // core light: march only the segment inside its influence sphere
    float rad = 1.1 * size + 0.5 * g_top;
    vec3 oc = ro - g_corePos;
    float b = dot(oc, rd), c = dot(oc, oc) - rad * rad;
    float h = b * b - c;
    if (h > 0.0 && g_coreI > 0.001) {
        h = sqrt(h);
        float t0 = max(-b - h, 0.0), t1 = min(-b + h, tMax);
        if (t1 > t0) {
            const int N = 24;
            float dt = (t1 - t0) / float(N);
            float ign = fract(52.9829189 * fract(0.06711056 * fc.x + 0.00583715 * fc.y) + fract(g_t) * 0.618);
            float t = t0 + dt * ign;
            float tr = 1.0;
            for (int i = 0; i < N; i++) {
                vec3 p = ro + rd * t;
                float dens = fog * (0.45 + 0.35 * vnoise2(p.xz * 2.2 + p.y * 1.7 + g_t * 0.2));
                dens *= 0.5 + 0.5 * exp(-max(p.y - g_top, 0.0) * 1.5);
                vec3 toC = g_corePos - p;
                float dc2 = dot(toC, toC);
                float pw = pitWeight(p);
                vec3 inS = g_coreC * g_coreI * pw * 0.2 / (0.06 + dc2);
                float a = dens * dt;
                acc += inS * a * tr * transm;
                tr *= exp(-a * 0.35 * pw);
                t += dt;
            }
            transm *= tr;
        }
    }
    return acc;
}

void setupCam() {
    float oa = g_t * 0.065 * orbitSpeed + 2.2;
    float dist = (1.75 * g_ext + 0.3) * (1.0 + 0.12 * sin(g_t * 0.045 * orbitSpeed + 1.0));
    float el = 0.42 + 0.18 * sin(g_t * 0.052 * orbitSpeed + 0.6);
    vec3 ta = vec3(0.0, 0.7 * g_top + 0.05 * size, 0.0);
    g_ro = ta + vec3(sin(oa) * cos(el) * dist, sin(el) * dist, cos(oa) * cos(el) * dist);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    g_lens = 1.3;
}
vec3 camRay(vec2 uv) { return normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_lens); }

vec4 renderScene(vec3 ro, vec3 rd, vec2 fc) {
    int hc;
    float t = marchCrystals(ro, rd, 30.0, 110, hc);
    vec3 col; float depth; float hit = 1.0;
    if (t > 0.0) {
        vec3 p = ro + rd * t;
        vec3 n = sceneNormal(p, hc);
        col = shadeCrystal(hc, p, rd, n, false);
        depth = t;
    } else {
        col = skyBase(rd) + skyStars(rd) * stars;
        depth = FAR;
        hit = 0.0;
    }
    float transm = 1.0;
    vec3 fg = fogMarch(ro, rd, depth, fc, transm);
    col = col * transm + fg;
    hit = max(hit, 1.0 - transm);
    return vec4(col, hit);
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    g_ar = audioReact;
    g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_n = clamp(floor(shards + 0.5) + 1.0, 1.0, float(MAXC));
    g_back = backColor.rgb * backAmt;
    float az = radians(lightAz), el = radians(lightEl);
    g_key = normalize(vec3(sin(az) * cos(el), sin(el), cos(az) * cos(el)));
    g_keyC = vec3(1.0, 0.93, 0.84);
    g_keyI = keyLight * (1.0 + 0.3 * g_ar * g_lev + 0.2 * g_ar * g_beat);
    g_glow = hueRot(glowColor.rgb, hueShift);
    g_coreC = hueRot(coreColor.rgb, hueShift);
    g_coreI = coreLight * (1.0 + 0.9 * g_ar * g_bass + 0.5 * g_ar * g_beat) * (0.85 + 0.15 * sin(g_t * 7.0) * sin(g_t * 3.1));
    setupCrystals();
    setupCam();
    float sweepPer = 9.0 / max(pulseSpeed, 0.05);
    g_scanY = -0.3 + fract(g_t / sweepPer) * (g_top + 1.9 * size);
    g_pulseR = fract(g_t * 0.28 * pulseSpeed) * (g_rimW + 0.35 * size);

    if (PASSINDEX == 0) {
        vec3 rd = camRay(uv);
        vec4 sc = renderScene(g_ro, rd, fragCoord);
        gl_FragColor = vec4(max(sc.rgb, 0.0), sc.a);
    } else {
        vec2 st = fragCoord / R;
        vec2 asp = vec2(R.y / R.x, 1.0);
        vec4 c0 = texture2D(bmScene, st);
        vec3 col = c0.rgb;
        float ignR = fract(52.9829189 * fract(0.06711056 * fragCoord.x + 0.00583715 * fragCoord.y)) * 6.28;
        // chromatic fringe (radial)
        float ab = aberration * 0.0035;
        vec2 dir = (st - 0.5) * 2.0;
        vec3 fr = vec3(texture2D(bmScene, st + dir * ab).r, c0.g, texture2D(bmScene, st - dir * ab).b);
        col = mix(col, fr, 0.7 * smoothstep(0.2, 0.9, c0.a) + 0.2);
        // bloom: two radii from everything over 1.0
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + ignR;
            float rr = sqrt((fi + 0.5) / 16.0) * 0.05;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            vec3 s = texture2D(bmScene, st + of).rgb;
            bl += max(s - 1.0, 0.0) * (1.0 - rr / 0.05);
        }
        bl /= 16.0;
        vec3 bl2 = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + ignR + 1.0;
            float rr = sqrt((fi + 0.5) / 8.0) * 0.014;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            bl2 += max(texture2D(bmScene, st + of).rgb - 0.9, 0.0);
        }
        bl2 /= 8.0;
        col += (bl * 1.4 + bl2 * 0.6) * bloom;
        // anamorphic streak: horizontal, blue-tinted
        if (streak > 0.001) {
            vec3 sk = vec3(0.0);
            for (int i = 0; i < 12; i++) {
                float fi = float(i) - 5.5;
                float w = 1.0 - abs(fi) / 6.5;
                vec2 of = vec2(fi * 0.018 * asp.x * (1.0 + 0.5 * hash12(fragCoord + fi)), 0.0);
                sk += max(texture2D(bmScene, st + of).rgb - 1.1, 0.0) * w;
            }
            sk /= 6.0;
            col += sk * vec3(0.5, 0.8, 1.2) * streak * 0.6;
        }
        // dust motes drifting up in three parallax layers
        if (motes > 0.001) {
            vec3 mo = vec3(0.0);
            for (int L = 0; L < 3; L++) {
                float fl = float(L);
                float sc = 18.0 + fl * 14.0;
                vec2 q = uv * sc + vec2(g_t * 0.02 * (fl + 1.0) * (mod(fl, 2.0) < 1.0 ? 1.0 : -1.0), -g_t * 0.05 * (1.0 + fl * 0.6));
                vec2 cell = floor(q), f = fract(q);
                float h = hash12(cell + fl * 31.0);
                vec2 o = vec2(hash12(cell + 2.0 + fl), hash12(cell + 5.0 + fl)) * 0.7 + 0.15;
                float dd = length(f - o);
                float br = step(0.72, h) * (0.5 + 0.5 * sin(g_t * (1.0 + 2.0 * h) + h * 40.0));
                mo += mix(vec3(0.6, 0.85, 1.0), g_glow, 0.5) * br * smoothstep(0.05 - fl * 0.01, 0.0, dd) * (1.0 - 0.25 * fl);
            }
            col += mo * motes * 1.1 * (0.6 + 0.4 * smoothstep(0.9, 0.2, c0.a));
        }
        col *= 1.0 + 0.08 * g_ar * g_lev;
        col = aces(col * 1.05);
        float l0 = lum(col);
        col = clamp(mix(vec3(l0), col, saturation), 0.0, 1.0);
        float g1 = (hash12(fragCoord + fract(g_t) * 61.0) - 0.5) * 0.05;
        col += g1 * grain * (0.5 + 0.5 * (1.0 - abs(l0 - 0.5) * 2.0));
        col *= 1.0 - 0.18 * smoothstep(0.45, 1.5, length(uv));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.5);
        float lumHD = lum(col);
        col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
        float alphaOut = transparentBg ? smoothstep(0.1, 0.9, c0.a) : mix(1.0, backColor.a, backAmt * step(0.001, 1.0 - backColor.a));
        gl_FragColor = vec4(col, alphaOut);
    }
}
