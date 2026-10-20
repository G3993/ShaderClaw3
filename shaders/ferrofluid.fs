/*{
  "DESCRIPTION": "Ferrofluid — a dish of black magnetic liquid under three wandering magnets, raymarched in 3D. Above a critical field the mirror-smooth pool breaks into the Rosensweig instability: hedgehogs of sharp concave spikes grow in exact hexagonal shells over each magnet, the liquid mounds up beneath them, and where two magnets meet the spikes part along a seam. As the magnets drift, merge and separate, the spike fields rise, shrink, swirl and re-form, and the whole field slowly breathes so the dish keeps changing. Black-mirror shading with one inter-reflection bounce (spikes mirror their neighbours, the pool mirrors the hedgehogs), crisp studio softbox and coloured strip-light reflections sliding across the flanks as the camera orbits, an oily thin-film sheen at the spike bases, a gunmetal dish and an out-of-focus studio backdrop. Bass pumps the magnets so the spikes stand up with the low end, beats send a capillary ring across the pool, mids swirl the hedgehogs, levels brighten the strips, highs shiver the surface.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #3",
  "CATEGORIES": ["Generator", "3D", "Simulation", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Speed",             "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "magnetSpeed",  "LABEL": "Magnet Wander",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Speed" },
    { "NAME": "swirl",        "LABEL": "Hedgehog Swirl",    "TYPE": "float", "MIN": -2.0, "MAX": 2.0,  "DEFAULT": 0.35, "GROUP": "Speed" },
    { "NAME": "orbitSpeed",   "LABEL": "Camera Orbit",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "fieldStrength","LABEL": "Field Strength",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Magnetic Field" },
    { "NAME": "fieldCycle",   "LABEL": "Field Breathing",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Magnetic Field" },
    { "NAME": "magnets",      "LABEL": "Magnets",           "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 3.0, "DEFAULT": 3.0, "GROUP": "Magnetic Field" },
    { "NAME": "spikeHeight",  "LABEL": "Spike Height",      "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Magnetic Field" },
    { "NAME": "spikeSpacing", "LABEL": "Spike Spacing",     "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Magnetic Field" },
    { "NAME": "sharpness",    "LABEL": "Spike Sharpness",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Magnetic Field" },
    { "NAME": "fluidTint",    "LABEL": "Fluid Tint",        "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.025, 1.0], "GROUP": "Color" },
    { "NAME": "keyColor",     "LABEL": "Softbox",           "TYPE": "color", "DEFAULT": [1.0, 0.93, 0.85, 1.0],  "GROUP": "Color" },
    { "NAME": "light1",       "LABEL": "Strip Light 1",     "TYPE": "color", "DEFAULT": [0.1, 0.7, 1.0, 1.0],    "GROUP": "Color" },
    { "NAME": "light2",       "LABEL": "Strip Light 2",     "TYPE": "color", "DEFAULT": [1.0, 0.18, 0.45, 1.0],  "GROUP": "Color" },
    { "NAME": "light3",       "LABEL": "Rim Light",         "TYPE": "color", "DEFAULT": [1.0, 0.55, 0.12, 1.0],  "GROUP": "Color" },
    { "NAME": "sheen",        "LABEL": "Oil Sheen",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.45, "GROUP": "Color" },
    { "NAME": "hueShift",     "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",   "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "backColor",    "LABEL": "Background Color",  "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Color" },
    { "NAME": "backAmt",      "LABEL": "Background Amount", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "backdrop",     "LABEL": "Backdrop Glow",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "keyLight",     "LABEL": "Softbox Intensity", "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "stripLights",  "LABEL": "Strip Lights",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "reflections",  "LABEL": "Inter-Reflections", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "camHeight",    "LABEL": "Camera Height",     "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 3.0, "DEFAULT": 0.85, "GROUP": "Light" },
    { "NAME": "camDist",      "LABEL": "Camera Distance (wide)",   "TYPE": "float", "AUDIO": false, "MIN": 1.5, "MAX": 5.0, "DEFAULT": 2.7, "GROUP": "Light" },
    { "NAME": "bloom",        "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "aberration",   "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.2,  "GROUP": "Finish" },
    { "NAME": "dof",          "LABEL": "Depth of Field",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "grain",        "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "marchRes",     "LABEL": "March Resolution",  "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Finish" },
    { "NAME": "audioReact",   "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "ffScene", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Ferrofluid — FRONTIER pack #3
//   The Rosensweig instability: above a critical field a magnetic liquid
//   breaks into a hexagonal lattice of spikes. Each magnet under the dish
//   owns a "hedgehog": shells of 6·i spikes on ring i (exact hex shell
//   counts), spike height = f(field at the spike), the liquid mounds over
//   the magnet, cones are smooth-unioned into the pool (capillary fillets).
//   Where two magnets meet, ownership splits and the spikes part.
//   pass 0  ffScene — raymarch (cones + pool + dish + table), black-mirror
//                     liquid with one inter-reflection bounce, studio strips
//   pass 1  final   — bloom, fringe, ACES, grain, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define FAR  14.0
#define DISH 1.65

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU; float s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}
float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }
vec3 thinFilm(float d, float cosT) {
    float n = 1.45;
    float cosT2 = sqrt(max(1.0 - (1.0 - cosT * cosT) / (n * n), 0.0));
    vec3 lam = vec3(640.0, 545.0, 460.0);
    vec3 ph = (4.0 * PI * n * d * cosT2) / lam;
    return 0.5 + 0.5 * cos(ph + PI);
}

// ── state ────────────────────────────────────────────────────────────────
float g_focus, g_stripI, g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev, g_swirl, g_lam, g_hMax, g_fmul;
vec2  g_m0, g_m1, g_m2;
float g_s0, g_s1, g_s2;
vec3  g_bk, g_bu, g_ro, g_uu, g_vv, g_ww, g_back, g_kc, g_L1, g_L2, g_L3;
float m_mat, m_spk, m_H;   // material id of last map(); spike-ness (0 pool .. 1 tip)

// ── magnetic field ───────────────────────────────────────────────────────
float bOne(vec2 p, vec2 m, float s) { vec2 d = (p - m) * 2.1; float q = 1.0 + dot(d, d); return s / (q * sqrt(q)); }
vec3 fieldParts(vec2 p) { return vec3(bOne(p, g_m0, g_s0), bOne(p, g_m1, g_s1), bOne(p, g_m2, g_s2)); }
float fieldTot(vec2 p) { vec3 b = fieldParts(p); return b.x + b.y + b.z; }

// pool surface: flat liquid that mounds over the magnets and climbs the dish wall
float moundH(float B) { return 0.15 * (1.0 - exp(-B * B * 1.1)); }
float poolH(vec2 xz, float B) {
    float r = length(xz);
    float h = moundH(B);
    h += 0.045 * exp(-max(DISH - r, 0.0) * 22.0);
    // beat: a capillary ring runs out from the main magnet; highs: fine surface shiver
    float rm = length(xz - g_m0);
    float ringR = 0.25 + (1.0 - g_beat) * 1.4;
    h += 0.0045 * g_ar * g_beat * sin((rm - ringR) * 70.0) * exp(-abs(rm - ringR) * 9.0);
    h += 0.0012 * g_ar * g_high * (vnoise2(xz * 38.0 + g_t * 1.5) - 0.5);
    return h;
}

// signed distance to an infinite downward cone, apex at height H above the base point,
// base radius rb at y=0 (local coords: q radial, y vertical)
float coneSD(float q, float y, float H, float rb) {
    vec2 w = vec2(q, y - H);
    vec2 n = vec2(H, rb) / length(vec2(H, rb));
    vec2 down = vec2(rb, -H) / length(vec2(H, rb));
    return dot(w, down) < 0.0 ? length(w) : dot(w, n);
}

// one magnet's hedgehog: rings of 6·i spikes, three rings around the sample
float spikeAt(vec3 p, vec2 sp, float id, float which, inout float d) {
    if (length(sp) > DISH - 0.12) return d;
    vec3 b = fieldParts(sp);
    float B = b.x + b.y + b.z;
    float bw = which < 0.5 ? b.x : (which < 1.5 ? b.y : b.z);
    float own = smoothstep(0.38, 0.7, bw * bw / max(dot(b, b), 1e-5));
    float hj = hash11(id * 1.37 + 0.5);
    // heights fall off smoothly toward the mound edge — the outer ring are low bumps, not teardrops
    float hs = smoothstep(0.3, 1.05, B);
    float H = g_hMax * hs * hs * (0.6 + 0.4 * min(B, 1.6)) * own;
    H *= 0.88 + 0.24 * hj;
    H *= 1.0 + 0.05 * sin(g_t * (1.3 + hj) + hj * 6.28);
    if (H < 0.004) return d;
    float base = moundH(B) - 0.2 * H;
    // base radius and fillet shrink with the spike so a fading spike vanishes continuously
    float rb = g_lam * mix(0.78, 0.6, sharpness) * (0.35 + 0.65 * smoothstep(0.0, 0.18, H));
    float sh = clamp((p.y - base) / H, 0.0, 1.0);
    // capillary profile: concave flanks, needle tip, fat base (the fat base comes from the fillets)
    float c = coneSD(length(p.xz - sp), p.y - base, H, rb) + rb * 0.85 * sh * (1.0 - sh) * (1.0 - sh) - 0.002;
    if (c < d) { m_spk = sh; m_H = H; }
    d = smin(d, c, g_lam * 0.28 * smoothstep(0.0, 0.1, H));
    return d;
}

// one magnet's hedgehog: rings of 6·i spikes (exact hex shell counts); for each of the
// two bracketing rings the two bracketing slots are blended (smooth-min); a dropped
// neighbour is >= one spacing away, beyond the fillet radius
float hedgehog(vec3 p, vec2 m, float which, float dir) {
    vec2 rel = p.xz - m;
    float r = length(rel);
    float a = atan(rel.y, rel.x);
    float rr = r / g_lam;
    float ri = floor(rr);
    float d = 1e5;
    for (int k = 0; k <= 1; k++) {
        float i = ri + float(k);
        if (i < 0.0 || i > 15.0) continue;
        if (i < 0.5) { d = spikeAt(p, m, which * 31.0, which, d); continue; }
        float n = 6.0 * i;
        float ph = g_swirl * dir * 1.6 / (i + 1.5) + hash11(i + which * 7.0);
        float s0 = floor(a / TAU * n - ph);
        for (int j = 0; j < 2; j++) {
            float slot = s0 + float(j);
            float ang = (slot + ph) / n * TAU;
            vec2 sp = m + i * g_lam * vec2(cos(ang), sin(ang));
            d = spikeAt(p, sp, which * 31.0 + i * 97.0 + mod(slot, n), which, d);
        }
    }
    return d;
}

float map(vec3 p) {
    float r = length(p.xz);
    vec3 b = fieldParts(p.xz);
    float B = b.x + b.y + b.z;
    // liquid
    float ph = poolH(p.xz, B);
    float dl = (p.y - ph) * 0.75;
    m_spk = 0.0; m_H = 0.0;
    if (B > 0.2 && r < DISH) {
        // height bound: nothing in the neighbourhood rises above the local max spike tip
        float Bn = B * 1.25 + 0.05;
        float hn = smoothstep(0.3, 1.05, Bn);
        float Hloc = g_hMax * hn * hn * (0.6 + 0.4 * min(Bn, 1.6)) * 1.25 + 0.02;
        float above = p.y - ph - Hloc;
        if (above > 0.03) dl = min(dl, above);
        else {
            float ds = 1e5;
            float bb = max(dot(b, b), 1e-6);
            if (b.x > 0.12 && b.x * b.x > 0.1 * bb) ds = min(ds, hedgehog(p, g_m0, 0.0,  1.0));
            if (b.y > 0.12 && b.y * b.y > 0.1 * bb) ds = min(ds, hedgehog(p, g_m1, 1.0, -1.0));
            if (b.z > 0.12 && b.z * b.z > 0.1 * bb) ds = min(ds, hedgehog(p, g_m2, 2.0,  1.0));
            dl = smin(dl, ds * 0.8, g_lam * 0.42 * smoothstep(0.0, 0.12, m_H));
        }
    }
    dl = max(dl, r - DISH);
    // dish wall: rounded annulus, lip above the liquid
    vec2 q = vec2(r - DISH - 0.05, p.y + 0.02);
    vec2 e = abs(q) - vec2(0.035, 0.085);
    float dw = length(max(e, 0.0)) + min(max(e.x, e.y), 0.0) - 0.04;
    // dish floor under the wall + table
    float dt = p.y + 0.14;
    float d = dl; m_mat = 0.0;
    if (dw < d) { d = dw; m_mat = 1.0; }
    if (dt < d) { d = dt; m_mat = 2.0; }
    return d;
}

float march(vec3 ro, vec3 rd, float tmax, int steps) {
    float t = 0.0;
    // skip empty space above the scene slab (camera rays start high)
    float yTop = g_hMax * 1.35 + 0.3;
    if (ro.y > yTop) {
        if (rd.y >= -1e-4) return -1.0;
        t = (ro.y - yTop) / -rd.y;
    }
    for (int i = 0; i < 140; i++) {
        if (i >= steps) break;
        float d = map(ro + rd * t);
        if (d < 0.0006 * (1.0 + t)) return t;
        t += d;
        if (t > tmax) break;
    }
    return -1.0;
}

vec3 normalAt(vec3 p, float t) {
    float e = 0.0009 * (1.0 + 0.5 * t);
    const vec2 k = vec2(1.0, -1.0);
    return normalize(k.xyy * map(p + k.xyy * e) + k.yyx * map(p + k.yyx * e) +
                     k.yxy * map(p + k.yxy * e) + k.xxx * map(p + k.xxx * e));
}

// ── studio environment: black room, crisp softboxes and strip lights ─────
float rectLight(vec3 rd, vec3 L, vec3 U, vec2 hs, float soft) {
    float c = dot(rd, L);
    if (c <= 0.0) return 0.0;
    vec3 V = cross(L, U);
    vec2 uv = vec2(dot(rd, U), dot(rd, V)) / c;
    vec2 e = abs(uv) - hs;
    float m = smoothstep(soft, -soft, max(e.x, e.y));
    // softbox falloff toward edges + fine diffuser grid
    float core = 0.75 + 0.25 * smoothstep(0.0, -0.5 * min(hs.x, hs.y), max(e.x, e.y));
    return m * core;
}
vec3 env(vec3 rd) {
    vec3 c = vec3(0.0);
    vec3 up = vec3(0.0, 1.0, 0.0);
    // camera-relative product rig: small overhead softbox just behind the subject + a wide low
    // backlight panel the pool mirrors
    vec3 sbL = normalize(g_bk * 0.45 + up);
    c += g_kc * 1.0 * keyLight * rectLight(rd, sbL, g_bu, vec2(0.3, 0.16), 0.008);
    vec3 blL = normalize(g_bk + vec3(0.0, 0.32, 0.0));
    float bl = rectLight(rd, blL, g_bu, vec2(1.25, 0.2), 0.09);
    vec3 V = cross(blL, g_bu);
    float vv = dot(rd, V) / max(dot(rd, blL), 1e-3);
    c += g_kc * 0.4 * keyLight * bl * (0.35 + 0.65 * smoothstep(0.2, -0.2, vv));
    // tall white softbox strips around the set (fixed in the world): every flank carries a
    // full-length white line, and they slide over the spikes as the camera orbits
    c += vec3(1.0, 0.97, 0.94) * 3.2 * keyLight * rectLight(rd, normalize(vec3(0.75, 0.35, 0.66)), up, vec2(1.5, 0.040), 0.012);
    c += g_L2 * 2.6 * g_stripI * rectLight(rd, normalize(vec3(-0.95, 0.35, 0.3)), up, vec2(1.5, 0.040), 0.012);
    // camera-relative white strips: a front-side key (full-length lines on the flanks facing us)
    // and a back strip just off-axis behind the subject (long crisp streak in the pool + rims)
    c += vec3(1.0, 0.97, 0.94) * 3.0 * keyLight * rectLight(rd, normalize(-g_bk * 0.55 - g_bu * 0.85 + up * 0.08), up, vec2(1.6, 0.050), 0.012);
    c += vec3(1.0, 0.97, 0.94) * 3.4 * keyLight * rectLight(rd, normalize(g_bk + g_bu * 0.42 + up * 0.12), up, vec2(1.4, 0.035), 0.012);
    c += vec3(1.0, 0.97, 0.94) * 2.8 * keyLight * rectLight(rd, normalize(vec3(0.2, 0.35, -0.98)), up, vec2(1.5, 0.036), 0.012);
    // coloured strips
    c += g_L1 * 3.2 * g_stripI * rectLight(rd, normalize(vec3(1.0, 0.2, -0.4)), up, vec2(1.2, 0.060), 0.012);
    c += g_L2 * 3.2 * g_stripI * rectLight(rd, normalize(vec3(-0.7, 0.2, -0.75)), up, vec2(1.2, 0.060), 0.012);
    // low horizontal rim strip
    c += g_L3 * 1.6 * g_stripI * rectLight(rd, normalize(vec3(0.1, 0.05, 1.0)), vec3(1.0, 0.0, 0.0), vec2(0.8, 0.012), 0.003);
    c += vec3(0.002, 0.0025, 0.004) * smoothstep(-0.2, 0.8, rd.y);
    return c;
}

// out-of-focus studio backdrop behind the dish (what misses see)
vec3 backdropCol(vec3 rd) {
    float side = dot(rd, g_bu);
    vec3 c = mix(g_L2, g_L1, smoothstep(-0.7, 0.7, side)) * 0.07;
    c += g_kc * 0.09 * exp(-side * side * 6.0);
    float h = rd.y - 0.03;
    c *= exp(-max(h, 0.0) * 4.5) * smoothstep(-0.25, 0.0, h);
    return c * backdrop;
}

// cheap secondary shading (inside reflections)
vec3 shadeSimple(vec3 p, vec3 rd, vec3 n, float mat) {
    vec3 rr = reflect(rd, n);
    float ndv = clamp(dot(n, -rd), 0.0, 1.0);
    float F = mat < 0.5 ? 0.32 + 0.68 * pow(1.0 - ndv, 5.0) : (mat < 1.5 ? 0.55 : 0.03 + 0.5 * pow(1.0 - ndv, 5.0));
    return env(rr) * F;
}

float aoAt(vec3 p, vec3 n) {
    float o = 0.0, s = 1.0;
    for (int i = 1; i <= 2; i++) {
        float h = 0.03 * float(i * i);
        o += (h - map(p + n * h)) * s;
        s *= 0.6;
    }
    return clamp(1.0 - 3.2 * o, 0.0, 1.0);
}

vec3 shade(vec3 p, vec3 rd, vec3 n, float mat, float spk) {
    float ndv = clamp(dot(n, -rd), 0.0, 1.0);
    vec3 rr = reflect(rd, n);
    float ao = aoAt(p, n);
    // one inter-reflection bounce (spikes mirror their neighbours, the table mirrors the dish)
    vec3 refl;
    // the bounce only on the pool / mound (mirror image of the hedgehogs); a per-pixel gate on the
    // flanks flickered between occluded and open and broke the strip lines into dashes
    float Fq = 0.32 + 0.68 * pow(1.0 - ndv, 5.0);
    bool doB = reflections > 0.01 && mat < 0.5 && spk < 0.02;
    float t2 = doB ? march(p + n * 0.006, rr, 4.0, 24) : -1.0;
    if (t2 > 0.0) {
        vec3 p2 = p + n * 0.006 + rr * t2;
        float matB = m_mat;
        vec3 n2 = normalAt(p2, t2);
        refl = mix(env(rr), shadeSimple(p2, rr, n2, matB), reflections);
    } else refl = env(rr);
    refl *= mix(1.0, ao, 0.85);

    vec3 col;
    if (mat < 0.5) {
        // ferrofluid: near-black body, mirror-smooth surface
        float F = (0.32 + 0.68 * pow(1.0 - ndv, 5.0)) * (0.5 + 0.5 * smoothstep(0.02, 0.16, ndv));
        vec3 film = thinFilm(300.0 + 260.0 * vnoise2(p.xz * 1.3 + g_t * 0.04) + 160.0 * spk, ndv);
        float sheenM = sheen * 0.5 * (1.0 - smoothstep(0.0, 0.3, spk)) * smoothstep(0.15, 0.6, fieldTot(p.xz));
        vec3 tint = mix(vec3(1.0), 0.45 + film * 1.0, sheenM);
        col = refl * F * tint;
        // body: faint diffuse from the softbox
        float dif = max(dot(n, normalize(vec3(-0.25, 1.0, 0.15))), 0.0);
        col += fluidTint.rgb * dif * 0.25 * ao;
    } else if (mat < 1.5) {
        // dish: dark gunmetal, brushed
        float brush = 0.85 + 0.15 * vnoise2(vec2(atan(p.z, p.x) * 260.0, p.y * 6.0));
        col = refl * vec3(0.42, 0.44, 0.48) * brush * (0.6 + 0.4 * pow(1.0 - ndv, 3.0));
    } else {
        // table: black lacquer with faint micro-scratches
        float r = length(p.xz);
        float scratch = vnoise2(p.xz * vec2(90.0, 6.0)) * vnoise2(p.xz * vec2(7.0, 70.0));
        float F = 0.035 + 0.2 * pow(1.0 - ndv, 5.0);
        col = refl * F * (0.85 + 0.2 * scratch) * 0.6;
        // contact shadow ring around the dish
        col *= smoothstep(DISH + 0.05, DISH + 0.45, r) * 0.7 + 0.3;
        col += vec3(0.0015) * scratch;
    }
    return col;
}

// FXAA-style edge resolve: the spike rims are sub-pixel mirror highlights, so the march pass
// aliases into stair-steps; blend along the local edge direction (luma gradient)
float lumT(vec2 st) { return lum(min(texture2D(ffScene, st).rgb, 2.0)); }
vec4 sceneAA(vec2 st) {
    vec2 px = 1.0 / min(floor(R * marchRes), vec2(2304.0, 1296.0));
    vec4 cM = texture2D(ffScene, st);
    float lM = lum(min(cM.rgb, 2.0));
    float lN = lumT(st + vec2(0.0, px.y)), lS = lumT(st - vec2(0.0, px.y));
    float lE = lumT(st + vec2(px.x, 0.0)), lW = lumT(st - vec2(px.x, 0.0));
    float lmax = max(lM, max(max(lN, lS), max(lE, lW)));
    float lmin = min(lM, min(min(lN, lS), min(lE, lW)));
    float range = lmax - lmin;
    if (range < max(0.04, lmax * 0.12)) return cM;
    vec2 dir = vec2(-(lN - lS), (lE - lW));
    dir = dir / (min(abs(dir.x), abs(dir.y)) + max(range * 0.25, 0.01));
    dir = clamp(dir, -3.0, 3.0) * px;
    vec3 a = 0.5 * (texture2D(ffScene, st + dir * (1.0 / 3.0 - 0.5)).rgb + texture2D(ffScene, st + dir * (2.0 / 3.0 - 0.5)).rgb);
    vec3 b = a * 0.5 + 0.25 * (texture2D(ffScene, st - dir * 0.5).rgb + texture2D(ffScene, st + dir * 0.5).rgb);
    float lb = lum(min(b, 2.0));
    return vec4((lb < lmin || lb > lmax) ? a : b, cM.a);
}

void setupScene() {
    g_ar = audioReact;
    g_t = TIME * speed;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_lam = 0.118 * spikeSpacing;
    g_stripI = stripLights * (1.0 + g_ar * (0.3 * g_lev + 0.15 * g_mid));
    g_swirl = TIME * speed * swirl * 0.35 + 0.35 * g_ar * g_mid;
    // bass pumps the magnets: spikes rise with the low end, smoothly
    g_fmul = fieldStrength * (1.0 + 0.42 * g_ar * g_bass + 0.12 * g_ar * g_beat);
    float br = 1.0 - fieldCycle * 0.38 * (0.5 + 0.5 * sin(g_t * 0.21 + 1.3));
    g_hMax = 0.36 * spikeHeight;
    float mt = TIME * speed * magnetSpeed * 0.3;
    g_m0 = vec2(0.42 * sin(mt * 0.71 + 0.4), 0.30 * sin(mt * 0.53 + 2.0));
    g_m1 = vec2(0.95 * cos(mt * 0.43 + 2.4), 0.70 * sin(mt * 0.61 + 0.9));
    g_m2 = vec2(0.85 * cos(mt * 0.37 + 4.6), 0.80 * sin(mt * 0.47 + 4.1));
    g_s0 = 1.25 * g_fmul * br;
    g_s1 = magnets >= 2.0 ? 0.95 * g_fmul * (1.0 - fieldCycle * 0.4 * (0.5 + 0.5 * sin(g_t * 0.17 + 3.0))) : 0.0;
    g_s2 = magnets >= 3.0 ? 0.80 * g_fmul * (1.0 - fieldCycle * 0.45 * (0.5 + 0.5 * sin(g_t * 0.13 + 5.0))) : 0.0;
    g_back = backColor.rgb * backAmt;
    g_kc = keyColor.rgb;
    g_L1 = hueRot(light1.rgb, hueShift); g_L2 = hueRot(light2.rgb, hueShift); g_L3 = hueRot(light3.rgb, hueShift);
    // slow dolly between a low hero macro on the main hedgehog and the full dish
    float wide = smoothstep(0.0, 1.0, 0.5 - 0.5 * cos(TIME * 0.045));
    float oa = TIME * orbitSpeed * 0.06 + 0.6;
    float cd = camDist * mix(0.5, 1.0, wide);
    vec3 tm = vec3(g_m0.x, 0.0, g_m0.y);
    vec3 ta = mix(tm * 0.8 + vec3(0.0, 0.2, 0.0), vec3(0.1 * sin(TIME * 0.09), 0.14, 0.1 * cos(TIME * 0.08)), wide);
    g_ro = vec3(ta.x + cd * sin(oa), camHeight * mix(0.42, 1.0, wide) + 0.06 * sin(TIME * 0.07), ta.z + cd * cos(oa));
    g_focus = length(ta - g_ro);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    g_bk = normalize(vec3(g_ww.x, 0.0, g_ww.z));
    g_bu = normalize(cross(vec3(0.0, 1.0, 0.0), g_bk));
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    setupScene();

    if (PASSINDEX == 0) {
        vec3 rd = normalize(g_uu * uv.x + g_vv * uv.y + g_ww * 1.55);
        float t = march(g_ro, rd, FAR, 100);
        vec3 col; float hit;
        if (t > 0.0) {
            vec3 p = g_ro + rd * t;
            float mat = m_mat, spk = m_spk;
            vec3 n = normalAt(p, t);
            col = shade(p, rd, n, mat, spk);
            // selective supersampling: silhouettes and the thin strip highlights get a second,
            // diagonally offset ray (the lines are ~1-2 px wide and stair-step otherwise)
            float ndv0 = dot(n, -rd);
            if (mat < 0.5 && (ndv0 < 0.25 || lum(col) > 0.5)) {
                vec2 sres = min(floor(R * marchRes), vec2(2304.0, 1296.0));
                vec2 uv2 = uv + vec2(0.5, -0.5) / sres.y;
                vec3 rd2 = normalize(g_uu * uv2.x + g_vv * uv2.y + g_ww * 1.55);
                float tB = march(g_ro + rd2 * max(t - 0.25, 0.0), rd2, 1.0, 40);
                vec3 c2 = g_back + backdropCol(rd2) * backAmt;
                if (tB > 0.0) {
                    vec3 pB = g_ro + rd2 * (max(t - 0.25, 0.0) + tB);
                    float matS = m_mat, spkS = m_spk;
                    vec3 nB = normalAt(pB, t);
                    c2 = matS < 0.5 ? shade(pB, rd2, nB, matS, spkS) : col;
                }
                col = 0.5 * (col + c2);
            }
            vec3 bd = g_back + backdropCol(rd) * backAmt;
            if (mat > 1.5) col = mix(bd, col, smoothstep(3.6, 2.1, length(p.xz)));
            col = mix(col, bd, smoothstep(5.0, 12.0, t));
            hit = mat < 1.5 ? 1.0 : smoothstep(5.0, 3.0, length(p.xz));
        } else { col = g_back + backdropCol(rd) * backAmt; hit = 0.0; }
        gl_FragColor = vec4(max(col, 0.0), hit > 0.01 ? max(t, 0.01) * hit : 0.0);
    } else {
        vec2 st = fragCoord / R;
        vec2 asp = vec2(R.y / R.x, 1.0);
        vec4 c0 = sceneAA(st);
        vec3 col = c0.rgb;
        // depth of field: thin-lens circle of confusion from the depth stored in alpha
        float tc = c0.a > 0.0 ? c0.a : 30.0;
        float coc = clamp(dof * 0.016 * abs(1.0 - g_focus / tc), 0.0, 0.022);
        if (coc > 0.0012) {
            vec3 acc = col; float wsum = 1.0;
            float ig = fract(52.9829189 * fract(0.06711056 * fragCoord.x + 0.00583715 * fragCoord.y)) * 6.28;
            for (int i = 0; i < 14; i++) {
                float fi = float(i);
                float a = fi * 2.399963 + ig;
                float rr = sqrt((fi + 0.5) / 14.0) * coc;
                vec4 sm = texture2D(ffScene, st + vec2(cos(a), sin(a)) * rr * asp);
                float ts = sm.a > 0.0 ? sm.a : 30.0;
                float cs = dof * 0.016 * abs(1.0 - g_focus / ts);
                // a sample only spreads as far as its own blur (no sharp-into-blur halos)
                float w = smoothstep(rr * 0.6, rr, cs + 0.001);
                acc += min(sm.rgb, 6.0) * w; wsum += w;
            }
            col = acc / wsum;
        }
        float ignR = fract(52.9829189 * fract(0.06711056 * fragCoord.x + 0.00583715 * fragCoord.y)) * 6.28;
        float ab = aberration * 0.0035;
        vec2 dir = (st - 0.5) * 2.0;
        vec3 fr = vec3(texture2D(ffScene, st + dir * ab).r, c0.g, texture2D(ffScene, st - dir * ab).b);
        col = mix(col, fr, 0.5 * smoothstep(0.2, 0.8, length(dir)));
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + ignR;
            float rr = sqrt((fi + 0.5) / 16.0) * 0.04;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            bl += max(texture2D(ffScene, st + of).rgb - 0.8, 0.0) * (1.0 - rr / 0.04);
        }
        bl /= 16.0;
        vec3 bl2 = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + ignR + 1.0;
            float rr = sqrt((fi + 0.5) / 8.0) * 0.01;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            bl2 += max(texture2D(ffScene, st + of).rgb - 0.7, 0.0);
        }
        bl2 /= 8.0;
        col += (bl * 1.4 + bl2 * 0.45) * bloom;
        col *= 1.0 + g_ar * (0.12 * g_lev + 0.08 * g_bass);
        col = aces(col * 1.1);
        float l0 = lum(col);
        col = clamp(mix(vec3(l0), col, saturation), 0.0, 1.0);
        col += (hash12(fragCoord + fract(TIME) * 61.0) - 0.5) * 0.04 * grain;
        col *= 1.0 - 0.14 * smoothstep(0.5, 1.4, length(uv));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
        float lumHD = lum(col);
        col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
        float alphaOut = transparentBg ? smoothstep(0.05, 0.6, step(0.001, c0.a) + lum(c0.rgb)) : 1.0;
        gl_FragColor = vec4(col, alphaOut);
    }
}
