/*{
  "DESCRIPTION": "DNA — a glass double helix turning through soft studio light: two strands (round tubes or a flat wound ribbon) joined by base-pair rungs, rendered as real refracting glass with Fresnel reflections, chromatic dispersion that paints rainbow fringes along every edge, thin-film iridescence, specular glints, light bending through to the strand behind. Bokeh sparkles drift in depth with a shallow depth of field, soft bloom, film grain. Background colour is yours to set — pale studio grey, white paper or black glass. Bass breathes and spins the helix, mids light the rungs, highs twinkle the bokeh, beats flash the key light.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-12 — original. CIDKID chemistry pack #2",
  "CATEGORIES": ["Generator", "3D", "Chemistry", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Spin Speed",        "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "travel",       "LABEL": "Travel",            "TYPE": "float", "MIN": -2.0, "MAX": 2.0,  "DEFAULT": 0.4,  "GROUP": "Speed" },
    { "NAME": "orbitSpeed",   "LABEL": "Camera Orbit",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "colorCycle",   "LABEL": "Color Cycle",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "radius",       "LABEL": "Helix Radius",      "TYPE": "float", "AUDIO": false, "MIN": 0.4,  "MAX": 2.0,  "DEFAULT": 1.1,  "GROUP": "Helix" },
    { "NAME": "pitch",        "LABEL": "Pitch (turn length)","TYPE": "float", "AUDIO": false, "MIN": 1.5, "MAX": 8.0,  "DEFAULT": 3.4,  "GROUP": "Helix" },
    { "NAME": "thickness",    "LABEL": "Strand Thickness",  "TYPE": "float", "AUDIO": false, "MIN": 0.03, "MAX": 0.4,  "DEFAULT": 0.14, "GROUP": "Helix" },
    { "NAME": "ribbon",       "LABEL": "Ribbon (tube→band)","TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35, "GROUP": "Helix" },
    { "NAME": "groove",       "LABEL": "Groove Offset",     "TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 0.5,  "DEFAULT": 0.42, "GROUP": "Helix" },
    { "NAME": "rungs",        "LABEL": "Rungs Per Turn",    "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 16.0, "DEFAULT": 10.0, "GROUP": "Helix" },
    { "NAME": "rungThick",    "LABEL": "Rung Thickness",    "TYPE": "float", "AUDIO": false, "MIN": 0.02, "MAX": 0.2,  "DEFAULT": 0.06, "GROUP": "Helix" },
    { "NAME": "rungGap",      "LABEL": "Base-Pair Gap",     "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 0.3,  "DEFAULT": 0.08, "GROUP": "Helix" },
    { "NAME": "rungWave",     "LABEL": "Rung Motion",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Helix" },
    { "NAME": "bend",         "LABEL": "Bend",              "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Helix" },
    { "NAME": "tilt",         "LABEL": "Tilt (screen roll)","TYPE": "float", "AUDIO": false, "MIN": -1.57,"MAX": 1.57, "DEFAULT": 1.35, "GROUP": "Helix" },
    { "NAME": "zoom",         "LABEL": "Zoom",              "TYPE": "float", "AUDIO": false, "MIN": 0.4,  "MAX": 2.5,  "DEFAULT": 0.9,  "GROUP": "Helix" },
    { "NAME": "ior",          "LABEL": "Refraction",        "TYPE": "float", "AUDIO": false, "MIN": 1.0,  "MAX": 2.0,  "DEFAULT": 1.45, "GROUP": "Glass" },
    { "NAME": "dispersion",   "LABEL": "Dispersion",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Glass" },
    { "NAME": "iridescence",  "LABEL": "Iridescence",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Glass" },
    { "NAME": "reflectAmt",   "LABEL": "Reflection",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Glass" },
    { "NAME": "clarity",      "LABEL": "Clarity",           "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.85, "GROUP": "Glass" },
    { "NAME": "glassColor",   "LABEL": "Glass Tint",        "TYPE": "color", "DEFAULT": [0.96, 0.98, 1.0, 1.0], "GROUP": "Glass" },
    { "NAME": "rungColor",    "LABEL": "Rung Tint",         "TYPE": "color", "DEFAULT": [1.0, 0.85, 0.9, 1.0],  "GROUP": "Glass" },
    { "NAME": "rungGlow",     "LABEL": "Rung Glow",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Glass" },
    { "NAME": "keyLight",     "LABEL": "Key Light",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "glint",        "LABEL": "Glints",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bokeh",        "LABEL": "Bokeh Sparkles",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bokehSize",    "LABEL": "Bokeh Size",        "TYPE": "float", "MIN": 0.2,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "dof",          "LABEL": "Depth Of Field",    "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.35,  "GROUP": "Light" },
    { "NAME": "focus",        "LABEL": "Focus Distance",    "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "backColor",    "LABEL": "Background Color",  "TYPE": "color", "DEFAULT": [0.86, 0.86, 0.89, 1.0], "GROUP": "Color" },
    { "NAME": "backAmt",      "LABEL": "Background Amount", "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "hueShift",     "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",   "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "bloom",        "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "aberration",   "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "grain",        "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "marchRes",     "LABEL": "March Resolution",  "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 1.0,  "DEFAULT": 0.75, "GROUP": "Finish" },
    { "NAME": "audioReact",   "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "dnScene", "WIDTH": "floor($WIDTH*$marchRes)", "HEIGHT": "floor($HEIGHT*$marchRes)" },
    { "TARGET": "dnBokeh", "WIDTH": "floor($WIDTH*0.5)",       "HEIGHT": "floor($HEIGHT*0.5)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// DNA — CIDKID chemistry pack #2
//   pass 0  dnScene — raymarched glass double helix in a twisted domain
//                     (strands + repeated base-pair rungs), bounded by a
//                     cylinder; glass = Fresnel reflection of a studio env,
//                     per-channel dispersive refraction with an exit march,
//                     a secondary march for the strand seen THROUGH the glass,
//                     thin-film iridescence, glints. a = depth
//   pass 1  dnBokeh — 56 iridescent bokeh sparkles with depth-sized discs
//   pass 2  final   — depth-of-field gather, bokeh, bloom, chromatic fringe,
//                     grain, vignette, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359

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
mat3 tr(mat3 m) { return mat3(m[0][0], m[1][0], m[2][0], m[0][1], m[1][1], m[2][1], m[0][2], m[1][2], m[2][2]); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }

// ── state ────────────────────────────────────────────────────────────────
float g_t, g_ar, g_lens, g_twist, g_rot, g_lip, g_R, g_th, g_cyc, g_spacing, g_rungT, g_shift;
float g_bass, g_mid, g_high, g_beat, g_lev;
vec3  g_ro, g_uu, g_vv, g_ww, g_back;
mat3  g_tilt, g_tiltI;

vec3 rainbow(float t) {
    t += hueShift + g_cyc;
    vec3 c = 0.5 + 0.5 * cos(TAU * (t + vec3(0.0, 0.33, 0.67)));
    float l = lum(c);
    return clamp(mix(vec3(l), c, saturation), 0.0, 1.0);
}

void setupCam() {
    float oa = g_t * 0.07 * orbitSpeed + 0.4;
    float dist = 6.2 / zoom + radius * 1.2;
    g_ro = vec3(sin(oa) * dist, 0.9 + 0.5 * sin(g_t * 0.11 * orbitSpeed + 2.0), cos(oa) * dist);
    vec3 ta = vec3(0.0, 0.1, 0.0);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    g_lens = 1.7;
    // Tilt is a screen roll: the camera keeps orbiting AROUND the helix axis
    // (always side-on, never down the bore) and the frame is rolled so the
    // helix lies across the 16:9 picture edge to edge.
    float roll = tilt + 0.05 * sin(g_t * 0.13);
    vec3 u2 = cos(roll) * g_uu + sin(roll) * g_vv;
    vec3 v2 = -sin(roll) * g_uu + cos(roll) * g_vv;
    g_uu = u2; g_vv = v2;
    // a slow nod of the axis toward and away from the camera
    g_tilt = rotX(0.16 * sin(g_t * 0.09) + 0.1);
    g_tiltI = tr(g_tilt);
}
vec3 camRay(vec2 uv) { return normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_lens); }
vec3 project(vec3 p) {
    vec3 d = p - g_ro;
    float z = dot(d, g_ww);
    vec2 s = vec2(dot(d, g_uu), dot(d, g_vv)) / max(z, 0.05) * g_lens;
    return vec3(s, z);
}

// studio environment: soft graded backdrop + three panel lights
vec3 env(vec3 d) {
    float up = d.y * 0.5 + 0.5;
    vec3 c = g_back * mix(0.42, 1.12, smoothstep(0.1, 0.75, up));
    // a dark gobo band low on the horizon → crisp dark cores in the tubes
    c *= 1.0 - 0.45 * smoothstep(0.35, 0.0, abs(d.y + 0.35)) ;
    float key = keyLight * (1.0 + 0.5 * g_ar * g_beat);
    c += vec3(1.0, 0.98, 0.95) * smoothstep(0.80, 0.985, dot(d, normalize(vec3(-0.45, 0.8, 0.35)))) * 2.2 * key;
    c += vec3(1.0, 0.90, 0.80) * smoothstep(0.86, 0.99, dot(d, normalize(vec3(0.7, 0.35, -0.5)))) * 1.3 * key;
    c += vec3(0.80, 0.90, 1.0) * smoothstep(0.90, 0.995, dot(d, normalize(vec3(0.1, -0.5, 0.85)))) * 1.1 * key;
    return c;
}

// ── helix SDF (twisted domain, exact in the horizontal cut) ──────────────
// returns the in-plane distance (an upper bound of the true distance; the
// march scales steps by g_lip to stay conservative). mat: 0 strand A, 1 B, 2 rung
float sdStrand(vec2 q, float phi) {
    float c = cos(phi), s = sin(phi);
    vec2 ql = vec2(c * q.x + s * q.y, -s * q.x + c * q.y);   // strand frame
    vec2 cc = vec2(ql.x - g_R, ql.y);
    float tr_ = mix(g_th, g_th * 0.42, ribbon);              // radial half
    float tt = g_th * (1.0 + 3.2 * ribbon);                 // tangential half
    float rnd = mix(g_th, g_th * 0.38, ribbon);
    vec2 b = vec2(tr_, tt) - rnd;
    vec2 dd = abs(cc) - b;
    return length(max(dd, 0.0)) + min(max(dd.x, dd.y), 0.0) - rnd;
}
float sdDNA(vec3 p, out int mat) {
    p.x += bend * 0.7 * sin(p.y * 0.45 + g_t * 0.25 * speed);
    p.z += bend * 0.5 * cos(p.y * 0.33 - g_t * 0.17 * speed);
    float a = p.y * g_twist + g_rot;
    float ca = cos(a), sa = sin(a);
    vec2 q = vec2(ca * p.x + sa * p.z, -sa * p.x + ca * p.z);
    float dA = sdStrand(q, 0.0);
    float dB = sdStrand(q, groove * TAU);
    float d = dA; mat = 0;
    if (dB < d) { d = dB; mat = 1; }
    if (rungs > 0.5) {
        // rungs ride along the axis with Travel; each one breathes open and
        // shut on a wave running up the helix (base pairs unzipping)
        float ys = p.y - g_shift;
        float idx = floor(ys / g_spacing + 0.5);
        float yr = ys - idx * g_spacing;
        float wave = 0.5 + 0.5 * sin(idx * 0.9 - g_t * 1.3 * speed * rungWave);
        float g = groove * TAU;
        vec3 pr = vec3(q.x, yr, q.y);
        vec3 a3 = vec3(g_R, 0.0, 0.0);
        vec3 b3 = vec3(g_R * cos(g), 0.0, g_R * sin(g));
        vec3 pa = pr - a3, ba = b3 - a3;
        float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
        float dr = length(pa - ba * h) - g_rungT;
        // base-pair gap: carve a thin slab at the middle of the rung
        float gapW = rungGap * (0.5 + 1.6 * wave * rungWave) * (1.0 + 0.8 * g_ar * g_bass);
        float gap = gapW * 0.5 - abs(h - 0.5) * length(ba);
        dr = max(dr, gap);
        if (dr < d) { d = dr; mat = 2; }
    }
    return d;
}
float sdDNAp(vec3 p) { int m; return sdDNA(p, m); }
vec3 dnaNormal(vec3 p) {
    vec2 e = vec2(0.0015, -0.0015);
    return normalize(e.xyy * sdDNAp(p + e.xyy) + e.yyx * sdDNAp(p + e.yyx) +
                     e.yxy * sdDNAp(p + e.yxy) + e.xxx * sdDNAp(p + e.xxx));
}
// bounding cylinder around the helix axis (helix space)
bool boundCyl(vec3 ro, vec3 rd, out float t0, out float t1) {
    float rb = g_R + g_th * 4.5 + bend * 1.3 + 0.1;
    vec2 o = ro.xz, d = rd.xz;
    float a = dot(d, d), b = dot(o, d), c = dot(o, o) - rb * rb;
    if (a < 1e-6) { if (c > 0.0) return false; t0 = 0.0; t1 = 40.0; return true; }
    float h = b * b - a * c;
    if (h < 0.0) return false;
    h = sqrt(h);
    t0 = max((-b - h) / a, 0.0); t1 = min((-b + h) / a, 40.0);
    return t1 > t0;
}
// march from ro along rd within [t0,t1]; returns t or -1
float marchDNA(vec3 ro, vec3 rd, float t0, float t1, int steps, out int mat) {
    float t = t0; mat = 0;
    for (int i = 0; i < 110; i++) {
        if (i >= steps) break;
        vec3 p = ro + rd * t;
        int m; float d = sdDNA(p, m);
        if (d < 0.0018 * (1.0 + t * 0.1)) { mat = m; return t; }
        t += d * g_lip;
        if (t > t1) break;
    }
    return -1.0;
}

// shared bits: reflection, glints, iridescence, rung glow
void glassCommon(vec3 p, vec3 rd, vec3 n, int mat, out float F, out vec3 rfl, out float gl, out vec3 irid, out vec3 rungAdd, out float emisAdd) {
    float cosv = max(dot(n, -rd), 0.0);
    F = 0.04 + 0.96 * pow(1.0 - cosv, 5.0);
    rfl = env(g_tiltI * reflect(rd, n)) * reflectAmt;
    vec3 rv = g_tiltI * reflect(rd, n);
    gl = pow(max(dot(rv, normalize(vec3(-0.45, 0.8, 0.35))), 0.0), 240.0) * 3.0
       + pow(max(dot(rv, normalize(vec3(0.7, 0.35, -0.5))), 0.0), 160.0) * 1.6;
    gl *= glint * (0.7 + 0.6 * vnoise2(p.xy * 40.0 + p.z * 30.0));
    float edge = pow(1.0 - cosv, 1.7);
    irid = rainbow(cosv * 2.2 + p.y * 0.08 + float(mat) * 0.15 + 0.3 * g_ar * g_high) * edge * iridescence * 1.6;
    rungAdd = vec3(0.0); emisAdd = gl + lum(irid) * 0.5;
    if (mat == 2) {
        // a light pulse travelling across each rung (A → B), mids drive it
        float a = p.y * g_twist + g_rot;
        vec2 q = vec2(cos(a) * p.x + sin(a) * p.z, -sin(a) * p.x + cos(a) * p.z);
        float along = atan(q.y, q.x) / (groove * TAU);            // 0 at A, 1 at B
        float idx = floor((p.y - g_shift) / g_spacing + 0.5);
        float pulse = exp(-pow(fract(along - g_t * 0.7 * speed * rungWave - idx * 0.13) - 0.5, 2.0) * 60.0);
        float glow = rungGlow * (0.35 + 1.1 * g_ar * g_mid) * (0.5 + 1.5 * pulse * rungWave);
        rungAdd = rungColor.rgb * glow * 0.6 + vec3(1.0) * pulse * 0.25 * rungWave * rungGlow;
        emisAdd += glow * 0.5;
    }
}
// the strand seen THROUGH the glass: reflection + tinted backdrop, no second refraction
vec3 shadeThrough(vec3 p, vec3 rd, vec3 n, int mat) {
    float F, gl, e; vec3 rfl, irid, rungAdd;
    glassCommon(p, rd, n, mat, F, rfl, gl, irid, rungAdd, e);
    vec3 tint = (mat == 2) ? rungColor.rgb : glassColor.rgb;
    vec3 behind = env(g_tiltI * rd) * tint * mix(0.7, 1.0, clarity);
    return rfl * F + behind * (1.0 - F) + irid * (0.6 + 0.4 * F) * 1.3 + vec3(gl) + rungAdd;
}
// glass surface shading at p (helix space) seen along rd
vec3 shadeGlass(vec3 p, vec3 rd, vec3 n, int mat, out float emisOut) {
    float F, gl, emis; vec3 rfl, irid, rungAdd;
    glassCommon(p, rd, n, mat, F, rfl, gl, irid, rungAdd, emis);
    vec3 tint = (mat == 2) ? rungColor.rgb : glassColor.rgb;
    // dispersive refraction: three entry rays, exit march on green
    float dsp = dispersion * 0.08;
    vec3 rR = refract(rd, n, 1.0 / max(ior - dsp, 1.0));
    vec3 rG = refract(rd, n, 1.0 / ior);
    vec3 rB = refract(rd, n, 1.0 / (ior + dsp));
    float ti = 0.02; vec3 ex = p + rG * ti;
    for (int i = 0; i < 20; i++) {
        float d = sdDNAp(ex);
        if (d > 0.0015) break;
        ti += max(-d, 0.01);
        ex = p + rG * ti;
    }
    float thick = ti;
    vec3 nx = -dnaNormal(ex);
    vec3 oR = refract(rR, nx, max(ior - dsp, 1.0)); if (dot(oR, oR) < 0.5) oR = reflect(rR, nx);
    vec3 oG = refract(rG, nx, ior);                 if (dot(oG, oG) < 0.5) oG = reflect(rG, nx);
    vec3 oB = refract(rB, nx, ior + dsp);           if (dot(oB, oB) < 0.5) oB = reflect(rB, nx);
    vec3 trans = vec3(env(g_tiltI * oR).r, env(g_tiltI * oG).g, env(g_tiltI * oB).b);
    float s0, s1; int m2;
    vec3 ex2 = ex + oG * 0.01;
    if (boundCyl(ex2, oG, s0, s1)) {
        float t2 = marchDNA(ex2, oG, s0, s1, 56, m2);
        if (t2 > 0.0) {
            vec3 p2 = ex2 + oG * t2;
            vec3 n2 = dnaNormal(p2);
            trans = mix(trans, shadeThrough(p2, oG, n2, m2), 0.85);
        }
    }
    vec3 absorb = exp(-thick * (1.0 - tint) * 2.5) * mix(0.6, 1.0, clarity);
    vec3 col = rfl * F + trans * (1.0 - F) * absorb;
    col = mix(col, col * tint, 0.5);
    col += irid * (0.6 + 0.4 * F) * 1.3 + vec3(gl) + rungAdd;
    emisOut = emis;
    return col;
}

vec4 renderScene(vec2 uv) {
    vec3 rdw = camRay(uv);
    vec3 ro = g_tilt * g_ro;
    vec3 rd = g_tilt * rdw;
    float t0, t1;
    vec3 col = env(rdw);
    float depth = 50.0;
    float emis = 0.0;
    if (boundCyl(ro, rd, t0, t1)) {
        int mat;
        float t = marchDNA(ro, rd, t0, t1, 110, mat);
        if (t > 0.0) {
            vec3 p = ro + rd * t;
            vec3 n = dnaNormal(p);
            col = shadeGlass(p, rd, n, mat, emis);
            depth = t;
            // depth haze toward the backdrop
            col = mix(col, g_back, smoothstep(6.0, 22.0, t) * 0.5);
        }
    }
    return vec4(col, depth + emis * 0.0);
}

// ── pass 1: bokeh sparkles ───────────────────────────────────────────────
vec3 bokehPass(vec2 uv) {
    vec3 col = vec3(0.0);
    float focusD = focus * (6.2 / zoom + radius * 1.2);
    for (int i = 0; i < 56; i++) {
        float fi = float(i);
        float h1 = hash11(fi * 3.1 + 1.0), h2 = hash11(fi * 7.3 + 2.0), h3 = hash11(fi * 5.7 + 3.0), h4 = hash11(fi * 2.9 + 4.0);
        // drift upward along the helix axis, wrap, slow sideways sway
        float y = mod(h2 * 14.0 + g_t * (0.08 + 0.1 * h3) * speed, 14.0) - 7.0;
        float ang = h1 * TAU + g_t * 0.05 * (h4 - 0.5);
        float rr = radius * (0.4 + 2.6 * h3);
        vec3 ph = vec3(cos(ang) * rr, y, sin(ang) * rr);
        vec3 pw = g_tiltI * ph;
        vec3 pr = project(pw);
        if (pr.z < 0.5) continue;
        float coc = dof * abs(pr.z - focusD) / focusD;
        float rad = bokehSize * (0.004 + 0.03 * coc) * 1.6 / pr.z * 3.0;
        rad = clamp(rad, 0.003, 0.09);
        vec2 d = uv - pr.xy;
        float dist = length(d);
        if (dist > rad * 1.3) continue;
        float tw = pow(0.5 + 0.5 * sin(g_t * (0.8 + 2.5 * h4) + h1 * TAU), 3.0) * (1.0 + 1.4 * g_ar * g_high) + 0.15;
        float disc = smoothstep(rad, rad * 0.82, dist);
        float ring = smoothstep(rad * 0.7, rad * 0.95, dist) * disc * 0.8;   // bright bokeh rim
        float core = exp(-dist * dist / (rad * rad * 0.08)) * (1.0 - smoothstep(0.0, 0.4, coc));
        float energy = 0.00012 / (rad * rad);
        vec3 c = mix(vec3(1.0), rainbow(h1 + h2 * 0.3), 0.55);
        col += c * (disc * 0.6 + ring + core * 1.5) * min(energy, 1.4) * tw * bokeh;
    }
    return col;
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    g_ar = audioReact;
    g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_cyc = g_t * 0.04 * colorCycle;
    g_back = backColor.rgb * backAmt;
    g_R = radius * (1.0 + 0.08 * g_ar * g_bass);
    g_th = thickness;
    g_twist = TAU / pitch;
    g_rot = g_t * 0.5 * speed + 0.9 * g_ar * g_bass - g_t * travel * g_twist;
    g_lip = 0.92 / sqrt(1.0 + g_R * g_R * g_twist * g_twist);
    g_spacing = pitch / max(rungs, 1.0);
    g_rungT = rungThick;
    g_shift = g_t * travel;
    setupCam();

    if (PASSINDEX == 0) {
        vec4 sc = renderScene(uv);
        gl_FragColor = vec4(max(sc.rgb, 0.0), sc.a);
    } else if (PASSINDEX == 1) {
        gl_FragColor = vec4(max(bokehPass(uv), 0.0), 1.0);
    } else {
        vec2 st = fragCoord / R;
        vec2 asp = vec2(R.y / R.x, 1.0);
        float focusD = focus * (6.2 / zoom + radius * 1.2);
        vec4 c0 = texture2D(dnScene, st);
        float depth = c0.a;
        float coc = dof * abs(depth - focusD) / focusD;
        float rad = clamp(coc * 0.012, 0.0, 0.022);
        // depth-of-field gather (poisson disc), far/near both blur
        vec3 col = vec3(0.0);
        float wsum = 0.0;
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + hash12(fragCoord) * 6.28;
            float rr = sqrt((fi + 0.5) / 16.0) * rad;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            vec4 s = texture2D(dnScene, st + of);
            float sc = dof * abs(s.a - focusD) / focusD;
            float w = 0.3 + 0.7 * smoothstep(rr * 30.0, rr * 60.0, sc * 0.018 + 0.001);
            col += s.rgb * w; wsum += w;
        }
        col /= max(wsum, 1e-3);
        // chromatic fringe on the glass only (radial, subtle)
        float ab = aberration * 0.0025;
        vec2 dir = (st - 0.5) * 2.0;
        vec3 fr = vec3(texture2D(dnScene, st + dir * ab).r, c0.g, texture2D(dnScene, st - dir * ab).b);
        col = mix(col, fr, 0.5 * step(depth, 49.0) * (1.0 - smoothstep(0.0, 0.01, rad)));
        // bokeh + bloom
        vec3 bk = texture2D(dnBokeh, st).rgb;
        col += bk;
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 12; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + hash12(fragCoord + 7.0) * 6.28;
            float rr = sqrt((fi + 0.5) / 12.0) * 0.03;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            vec3 s = texture2D(dnScene, st + of).rgb;
            bl += max(s - 1.25, 0.0) * (1.0 - rr / 0.03);
        }
        bl /= 12.0;
        col += bl * bloom * 1.4;
        col *= mix(1.0, 0.9 + 0.2 * g_lev, g_ar);
        col = col / (1.0 + max(col - 1.0, 0.0) * 0.6);   // soft shoulder, keeps the pale backdrop
        // film grain, slightly heavier in the mids
        float gl = lum(col);
        float g1 = (hash12(fragCoord + fract(g_t) * 61.0) - 0.5) * 0.045;
        col += g1 * grain * (0.5 + 0.5 * (1.0 - abs(gl - 0.5) * 2.0));
        col *= 1.0 - 0.10 * smoothstep(0.5, 1.5, length(uv));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.45);
        float lumHD = lum(col);
        col = clamp(mix(vec3(lumHD), col, 1.25), 0.0, 1.0);
        float alphaOut = transparentBg ? max(step(depth, 49.0), smoothstep(0.02, 0.2, lum(bk))) : mix(1.0, backColor.a, backAmt * step(0.001, 1.0 - backColor.a));
        gl_FragColor = vec4(col, alphaOut);
    }
}
