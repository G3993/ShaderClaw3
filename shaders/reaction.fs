/*{
  "DESCRIPTION": "Reaction — a living Gray-Scott reaction-diffusion chemistry rendered as 3D relief: two chemicals feed and kill each other in a real simulation (six solver steps every frame), and the growing concentration is raised into a landscape of coral ridges, dividing cells, wandering worms, fingerprint ridges, pulsing solitons or drifting spots that keep spreading, splitting and healing while a low camera drifts and orbits over them. Ridges glow hot, valleys hold a deep subsurface tint, a key light throws soft shadows across the relief with crisp speculars, rim light and a haze toward the horizon. Fresh seeds keep landing so the chemistry never dies. Bass pulses the growth and lifts the relief, beats splash new seeds, mids stir the field, highs sparkle the ridges.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-13 — original. CIDKID chemistry pack #5",
  "CATEGORIES": ["Generator", "3D", "Chemistry", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Reaction Speed",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.9,  "GROUP": "Speed" },
    { "NAME": "orbitSpeed", "LABEL": "Camera Orbit",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "travel",     "LABEL": "Travel",           "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "colorCycle", "LABEL": "Color Cycle",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.0,  "GROUP": "Speed" },
    { "NAME": "pattern",    "LABEL": "Pattern (coral→mitosis→worms→fingerprint→solitons→spots)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 5.0, "DEFAULT": 0.0, "GROUP": "Chemistry" },
    { "NAME": "feedTrim",   "LABEL": "Feed Trim",        "TYPE": "float", "AUDIO": false, "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.0,  "GROUP": "Chemistry" },
    { "NAME": "killTrim",   "LABEL": "Kill Trim",        "TYPE": "float", "AUDIO": false, "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.0,  "GROUP": "Chemistry" },
    { "NAME": "seeding",    "LABEL": "Seeding",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Chemistry" },
    { "NAME": "stir",       "LABEL": "Stir",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Chemistry" },
    { "NAME": "simRes",     "LABEL": "Sim Resolution",   "TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 1.0, "DEFAULT": 0.5,  "GROUP": "Chemistry" },
    { "NAME": "scale",      "LABEL": "Pattern Scale",    "TYPE": "float", "AUDIO": false, "MIN": 0.5,  "MAX": 2.5, "DEFAULT": 1.0,  "GROUP": "Relief" },
    { "NAME": "height",     "LABEL": "Relief Height",    "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0, "DEFAULT": 1.0,  "GROUP": "Relief" },
    { "NAME": "sharpness",  "LABEL": "Ridge Sharpness",  "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0, "DEFAULT": 0.5,  "GROUP": "Relief" },
    { "NAME": "camHeight",  "LABEL": "Camera Height",    "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 3.0, "DEFAULT": 1.2,  "GROUP": "Relief" },
    { "NAME": "camPitch",   "LABEL": "Camera Pitch",     "TYPE": "float", "AUDIO": false, "MIN": 0.15, "MAX": 1.3, "DEFAULT": 0.55, "GROUP": "Relief" },
    { "NAME": "deepColor",  "LABEL": "Valley Color",     "TYPE": "color", "DEFAULT": [0.01, 0.02, 0.06, 1.0], "GROUP": "Color" },
    { "NAME": "midColor",   "LABEL": "Slope Color",      "TYPE": "color", "DEFAULT": [0.3, 0.08, 0.95, 1.0],   "GROUP": "Color" },
    { "NAME": "ridgeColor", "LABEL": "Ridge Color",      "TYPE": "color", "DEFAULT": [0.25, 0.95, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "glowAmt",    "LABEL": "Subsurface Glow",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Color" },
    { "NAME": "backColor",  "LABEL": "Background Color", "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.05, 1.0], "GROUP": "Color" },
    { "NAME": "backAmt",    "LABEL": "Background Amount","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "hueShift",   "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation", "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "keyLight",   "LABEL": "Key Light",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "lightAz",    "LABEL": "Light Direction",  "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 360.0, "DEFAULT": 215.0, "GROUP": "Light" },
    { "NAME": "lightEl",    "LABEL": "Light Height",     "TYPE": "float", "AUDIO": false, "MIN": 8.0, "MAX": 80.0,  "DEFAULT": 34.0,  "GROUP": "Light" },
    { "NAME": "shadowAmt",  "LABEL": "Shadows",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.85, "GROUP": "Light" },
    { "NAME": "specAmt",    "LABEL": "Specular",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "iridescence","LABEL": "Iridescence",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Light" },
    { "NAME": "rimAmt",     "LABEL": "Rim Light",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "fog",        "LABEL": "Haze",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bloom",      "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7,  "GROUP": "Finish" },
    { "NAME": "aberration", "LABEL": "Chromatic Fringe", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.2,  "GROUP": "Finish" },
    { "NAME": "grain",      "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Finish" },
    { "NAME": "marchRes",   "LABEL": "March Resolution", "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Finish" },
    { "NAME": "audioReact", "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG","TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "rdA", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "rdB", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "rdC", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "rdD", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "rdE", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "rdF", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "rdScene", "WIDTH": "floor($WIDTH*$marchRes)", "HEIGHT": "floor($HEIGHT*$marchRes)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Reaction — CIDKID chemistry pack #5
//   passes 0-5  rdA → … → rdF : six Gray-Scott solver steps per frame
//               (each persistent pass reads the previous one's output, pass 0
//               reads rdF from the last frame). state = hi/lo packed (U, V)
//   pass 6      rdScene : the V concentration becomes a heightfield that is
//               raymarched (plane-bounded, bisection refine) and lit:
//               key light + soft shadows + curvature AO + subsurface + rim +
//               speculars + haze. a = hit coverage
//   pass 7      final : bloom, fringe, grain, ACES, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define FAR  60.0

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm2(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 4; i++) { s += a * vnoise2(p); p = p * 2.03 + 17.1; a *= 0.5; }
    return s;
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

// ── shared state ─────────────────────────────────────────────────────────
float g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev;
vec2  g_tile;      // world size of one sim tile (x, z)
vec2  g_simR;      // sim buffer resolution
vec3  g_ro, g_uu, g_vv, g_ww, g_L, g_back;
float g_hScale, g_lens;

// ═════════════════════════════════════════════════════════════════════════
//  CHEMISTRY  (passes 0..2)
// ═════════════════════════════════════════════════════════════════════════
vec2 feedKill(float p) {
    // Pearson parameter pairs, blended continuously along the dial
    // (verified alive in long runs: each keeps growing/dividing, none saturate the plane)
    vec2 c0 = vec2(0.0545, 0.0620);   // coral — fingers that branch into a labyrinth
    vec2 c1 = vec2(0.0367, 0.0649);   // mitosis — cells that keep dividing
    vec2 c2 = vec2(0.0580, 0.0650);   // worms — long ridges that wander
    vec2 c3 = vec2(0.0400, 0.0600);   // fingerprint — dense parallel ridges
    vec2 c4 = vec2(0.0250, 0.0600);   // solitons — pulsing spots and rings
    vec2 c5 = vec2(0.0140, 0.0540);   // moving spots — drifting cells
    p = clamp(p, 0.0, 5.0);
    vec2 r = c0;
    r = mix(r, c1, clamp(p - 0.0, 0.0, 1.0));
    r = mix(r, c2, clamp(p - 1.0, 0.0, 1.0));
    r = mix(r, c3, clamp(p - 2.0, 0.0, 1.0));
    r = mix(r, c4, clamp(p - 3.0, 0.0, 1.0));
    r = mix(r, c5, clamp(p - 4.0, 0.0, 1.0));
    return r;
}

// read the previous solver state (pass 0 reads last frame's rdF)
vec4 prevState(vec2 uv) {
    uv = fract(uv);
    if (PASSINDEX == 0) return texture2D(rdF, uv);
    if (PASSINDEX == 1) return texture2D(rdA, uv);
    if (PASSINDEX == 2) return texture2D(rdB, uv);
    if (PASSINDEX == 3) return texture2D(rdC, uv);
    if (PASSINDEX == 4) return texture2D(rdD, uv);
    return texture2D(rdE, uv);
}

// half-float buffers quantise a 0..1 value to ~5e-4 near 1.0 — too coarse for the
// solver's per-step increments (the relief terraced). Each chemical is stored as
// hi = floor(x·256)/256 (exact in f16) + lo = fract(x·256), i.e. ~2e-6 precision.
vec4 packState(float U, float V) {
    float uh = floor(U * 256.0), vh = floor(V * 256.0);
    return vec4(uh / 256.0, U * 256.0 - uh, vh / 256.0, V * 256.0 - vh);
}
vec2 unpackState(vec4 s) { return vec2(s.r + s.g / 256.0, s.b + s.a / 256.0); }
vec2 prevUV(vec2 uv) { return unpackState(prevState(uv)); }

vec4 seedField(vec2 uv) {
    // spawn-visible initial condition: a few soft blobs + scattered dots
    // (small features, so the chemistry's own scale shows from frame 1)
    vec2 q = uv * vec2(R.x / R.y, 1.0);
    float n = fbm2(q * 26.0 + 3.7);
    float blobs = smoothstep(0.64, 0.70, n);
    float dots = step(0.965, hash12(floor(uv * R / 4.0)));      // 4x4-texel seed dots
    float v = clamp(blobs * 0.9 + dots * 0.9, 0.0, 1.0);
    return packState(1.0 - v * 0.5, v);
}

vec4 simStep(vec2 fragCoord) {
    vec2 ts = 1.0 / R;
    vec2 uv = fragCoord * ts;
    vec2 asp = vec2(R.x / R.y, 1.0);
    // slow stir: a tiny curl advection + a wandering feed/kill "weather"
    float st = stir * (1.0 + 0.8 * g_ar * g_mid);
    vec2 q = uv * asp * 2.0 + g_t * 0.03;
    vec2 flow = vec2(vnoise2(q + 11.0) - 0.5, vnoise2(q + 47.0) - 0.5) * 0.22 * st;   // ≤ 0.11 texel / step
    vec2 suv = uv - flow * ts;
    vec2 c = prevUV(suv);
    vec2 l  = prevUV(suv + vec2(-ts.x, 0.0)), r  = prevUV(suv + vec2(ts.x, 0.0));
    vec2 d  = prevUV(suv + vec2(0.0, -ts.y)), u  = prevUV(suv + vec2(0.0, ts.y));
    vec2 dl = prevUV(suv + vec2(-ts.x, -ts.y)), dr = prevUV(suv + vec2(ts.x, -ts.y));
    vec2 ul = prevUV(suv + vec2(-ts.x, ts.y)),  ur = prevUV(suv + vec2(ts.x, ts.y));
    vec2 lap = 0.2 * (l + r + d + u) + 0.05 * (dl + dr + ul + ur) - c;
    vec2 fk = feedKill(pattern);
    float weather = (vnoise2(uv * asp * 1.6 + g_t * 0.02 + 5.0) - 0.5) * 0.0025 * st;
    float f = fk.x + feedTrim * 0.012 + weather;
    float k = fk.y + killTrim * 0.004 - 0.0012 * g_ar * g_bass;   // bass lowers kill = growth surge
    float U = c.x, V = c.y;
    float uvv = U * V * V;
    float dt = 0.25 + 0.75 * speed;
    U += (1.0 * lap.x - uvv + f * (1.0 - U)) * dt;
    V += (0.5 * lap.y + uvv - (f + k) * V) * dt;
    // seeding: a fresh seed lands every so often at a hashed point
    float rate = 0.4 * seeding;
    float sn = floor(g_t * rate);
    float sage = fract(g_t * rate) / max(rate, 1e-3);
    vec2 sp = hash22(vec2(sn, 3.0 + sn * 0.37));
    vec2 dpx = (uv - sp) * asp * R.y;             // distance in sim texels
    float sd = length(dpx);
    float splash = smoothstep(5.0, 2.0, sd) * smoothstep(0.35, 0.0, sage) * step(0.01, seeding);
    // beat splash: a ring that opens at a hashed spot while the pulse is high
    float bn = floor(g_t * 2.0);
    vec2 bp = hash22(vec2(bn * 1.7, 9.0 + bn));
    float bd = length((uv - bp) * asp * R.y);
    float ring = smoothstep(2.0, 0.0, abs(bd - 4.0 - 6.0 * (1.0 - g_beat))) * g_beat * g_ar * 0.9;
    float inj = max(splash, ring);
    V = max(V, inj * 0.85);
    U = min(U, 1.0 - inj * 0.4);
    return packState(clamp(U, 0.0, 1.0), clamp(V, 0.0, 1.0));
}

// ═════════════════════════════════════════════════════════════════════════
//  RELIEF  (pass 3)
// ═════════════════════════════════════════════════════════════════════════
// concentration → height, tricubic-smooth sampled, mirror-tiled across the plane
float vSample(vec2 uv) {
    vec4 s = texture2D(rdF, uv);
    return s.b + s.a / 256.0;
}
float vCubic(vec2 uv) {
    // B-spline bicubic from 4 bilinear taps (smooth normals when magnified)
    vec2 ts = 1.0 / g_simR;
    vec2 st = uv * g_simR - 0.5;
    vec2 i = floor(st), f = fract(st);
    vec2 w0 = (1.0 / 6.0) * (1.0 - f) * (1.0 - f) * (1.0 - f);
    vec2 w1 = (1.0 / 6.0) * (4.0 - 3.0 * f * f * (2.0 - f));   // (3f³-6f²+4)/6
    vec2 w2 = (1.0 / 6.0) * (4.0 - 3.0 * (1.0 - f) * (1.0 - f) * (1.0 + f));
    vec2 w3 = (1.0 / 6.0) * f * f * f;
    vec2 g0 = w0 + w1, g1 = w2 + w3;
    vec2 h0 = (w1 / g0) - 1.0 + i;
    vec2 h1 = (w3 / g1) + 1.0 + i;
    vec2 p00 = (h0 + 0.5) * ts, p11 = (h1 + 0.5) * ts;
    float a = vSample(vec2(p00.x, p00.y)), b = vSample(vec2(p11.x, p00.y));
    float c = vSample(vec2(p00.x, p11.y)), d = vSample(vec2(p11.x, p11.y));
    return g0.y * (g0.x * a + g1.x * b) + g1.y * (g0.x * c + g1.x * d);
}
vec2 worldToUV(vec2 wp) {
    vec2 uv = wp / g_tile;
    // mirror tiling: no seams at tile borders
    vec2 m = fract(uv * 0.5) * 2.0;
    return 1.0 - abs(m - 1.0);
}
float shapeH(float v) {
    float h = clamp(v * 2.4, 0.0, 1.0);
    h = pow(h, 0.9);
    h = mix(h, smoothstep(0.1, 0.85, h), sharpness);
    return h;
}
float H(vec2 wp) {           // world height
    return shapeH(vCubic(worldToUV(wp))) * g_hScale;
}
float Hraw(vec2 wp) {        // shaped 0..1 height, no scale
    return shapeH(vCubic(worldToUV(wp)));
}

vec3 normalAt(vec2 wp, float e) {
    float hl = H(wp - vec2(e, 0.0)), hr = H(wp + vec2(e, 0.0));
    float hd = H(wp - vec2(0.0, e)), hu = H(wp + vec2(0.0, e));
    return normalize(vec3(hl - hr, 2.0 * e, hd - hu));
}

float marchRelief(vec3 ro, vec3 rd, out float hitH) {
    hitH = 0.0;
    float maxH = g_hScale * 1.02 + 0.001;
    float t = 0.0;
    if (ro.y > maxH) {
        if (rd.y >= 0.0) return -1.0;
        t = (maxH - ro.y) / rd.y;
    }
    float tEnd = FAR;
    if (rd.y < 0.0) tEnd = min(tEnd, (0.0 - ro.y) / rd.y + 0.001);
    float dPrev = 0.0, tPrev = t;
    bool hit = false;
    for (int i = 0; i < 110; i++) {
        vec3 p = ro + rd * t;
        float h = H(p.xz);
        float d = p.y - h;
        if (d < 0.0005) { hit = true; break; }
        dPrev = d; tPrev = t;
        t += max(d * 0.5, 0.012 + t * 0.006);
        if (t > tEnd) break;
    }
    if (!hit) {
        // check the floor plane itself (valleys at height 0)
        if (rd.y < 0.0 && t > tEnd - 0.01) { t = tEnd; hitH = 0.0; return t; }
        return -1.0;
    }
    // bisection refine between tPrev (outside) and t (inside)
    float a = tPrev, b = t;
    for (int i = 0; i < 5; i++) {
        float m = 0.5 * (a + b);
        vec3 p = ro + rd * m;
        if (p.y - H(p.xz) < 0.0) b = m; else a = m;
    }
    t = 0.5 * (a + b);
    hitH = H((ro + rd * t).xz);
    return t;
}

float softShadow(vec3 p, vec3 L) {
    // march toward the light over the heightfield, penumbra from the closest miss
    float res = 1.0;
    float t = 0.03;
    for (int i = 0; i < 18; i++) {
        vec3 q = p + L * t;
        float d = q.y - H(q.xz);
        if (q.y > g_hScale * 1.05) break;
        res = min(res, 12.0 * d / t);
        if (res < 0.01) break;
        t += clamp(d * 0.8, 0.02, 0.25);
    }
    return clamp(res, 0.0, 1.0);
}

float curvatureAO(vec2 wp, float h, float e) {
    float s = 0.0;
    s += H(wp + vec2(e, 0.0)) + H(wp - vec2(e, 0.0)) + H(wp + vec2(0.0, e)) + H(wp - vec2(0.0, e));
    s += H(wp + vec2(e, e)) + H(wp - vec2(e, e)) + H(wp + vec2(e, -e)) + H(wp - vec2(e, -e));
    float avg = s / 8.0;
    return clamp(1.0 - (avg - h) * 3.2 / max(g_hScale, 0.05), 0.25, 1.0);
}

vec3 pal(float h) {
    vec3 a = deepColor.rgb, b = midColor.rgb, c = ridgeColor.rgb;
    vec3 col = mix(a, b, smoothstep(0.0, 0.55, h));
    col = mix(col, c, smoothstep(0.5, 1.0, h));
    col = hueRot(col, hueShift + g_t * 0.02 * colorCycle);
    float l = lum(col);
    return max(mix(vec3(l), col, saturation), 0.0);
}

vec3 skyCol(vec3 rd) {
    vec3 b = g_back;
    float hor = pow(1.0 - clamp(rd.y, 0.0, 1.0), 6.0);
    vec3 glow = pal(0.6) * 0.18 * hor;
    float sun = pow(max(dot(rd, g_L), 0.0), 64.0);
    return b * (1.0 + 0.8 * hor) + glow + pal(0.9) * sun * 0.6 * keyLight;
}

vec4 renderScene(vec2 uv) {
    vec3 rd = normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_lens);
    float hh;
    float t = marchRelief(g_ro, rd, hh);
    vec3 sky = skyCol(rd);
    if (t < 0.0) return vec4(sky, 0.0);
    vec3 p = g_ro + rd * t;
    float e = g_tile.y / g_simR.y;                  // one sim texel in world units
    float en = max(e * 1.2, t * 0.0015);            // normal eps grows with distance (anti-alias)
    vec3 n = normalAt(p.xz, en);
    float hN = clamp(hh / max(g_hScale, 1e-3), 0.0, 1.0);
    // micro grit on the normal: fine mineral texture so close-ups never look smooth-plastic
    // (3D-ish: y is folded in so cliff faces don't get stretched into stripes)
    vec2 gq = vec2(p.x + p.y * 0.9, p.z - p.y * 0.7);
    float gr = vnoise2(gq * 70.0) - 0.5 + 0.5 * (vnoise2(gq * 160.0 + 7.0) - 0.5);
    float gr2 = vnoise2(gq * 70.0 + 31.0) - 0.5 + 0.5 * (vnoise2(gq * 160.0 + 53.0) - 0.5);
    vec3 ng = normalize(n + vec3(gr, 0.0, gr2) * 0.26 * (0.35 + 0.65 * hN) * smoothstep(16.0, 1.0, t));
    float ao = curvatureAO(p.xz, hh, e * 1.5);
    float sh = mix(1.0, softShadow(p + n * 0.01, g_L), shadowAmt);
    vec3 V = -rd;
    vec3 Hv = normalize(g_L + V);
    float ndl = max(dot(ng, g_L), 0.0);
    float ndv = max(dot(ng, V), 0.0);
    float fres = pow(1.0 - ndv, 3.0);
    vec3 albedo = pal(hN);
    // lighting
    vec3 key = vec3(1.0, 0.96, 0.9) * keyLight * (1.0 + 0.12 * g_ar * g_beat);
    vec3 skyFill = mix(g_back * 1.5 + 0.02, pal(0.55) * 0.35, 0.3) * (0.45 + 0.55 * ng.y);
    vec3 col = albedo * (key * ndl * sh * 1.6 + skyFill * ao * 1.2);
    // subsurface: the structures glow from inside, strongest at grazing; valleys stay deep
    float sss = glowAmt * (0.35 + 0.65 * fres) * (1.0 - hN * 0.5) * ao * (0.12 + 0.88 * smoothstep(0.02, 0.35, hN));
    col += pal(hN * 0.6 + 0.3) * sss * 0.55 * (0.4 + 0.6 * sh);
    // speculars: crisp hot + wide sheen, plus high-frequency glints on the ridges
    float sg = (0.12 + 0.88 * smoothstep(0.02, 0.3, hN));          // the valley floor is matte
    float spec = pow(max(dot(ng, Hv), 0.0), 56.0) * specAmt * sh * sg;
    float sheen = pow(max(dot(ng, Hv), 0.0), 7.0) * 0.12 * specAmt * sh * sg;
    float glint = pow(max(dot(ng, Hv), 0.0), 140.0) * step(0.86, hash12(floor(p.xz * 90.0))) * 3.0 * (0.4 + 0.6 * g_high * g_ar) * sh;
    col += key * (spec * 0.85 + sheen + glint * hN * 0.7);
    // thin-film sheen: an oil-slick hue sweep across the ridges at grazing angles
    float film = fres * iridescence * (0.3 + 0.7 * hN) * sh * sg;
    vec3 filmC = 0.5 + 0.5 * cos(TAU * (ndv * 1.6 + hN * 0.7 + vec3(0.0, 0.33, 0.67)));
    col += filmC * film * 0.28 * key;
    // rim
    vec3 rimC = pal(min(hN + 0.35, 1.0));
    col += rimC * fres * rimAmt * 0.5 * (0.3 + 0.7 * ndl) * ao;
    // haze
    float fz = 1.0 - exp(-t * 0.05 * fog);
    col = mix(col, sky, fz);
    return vec4(max(col, 0.0), 1.0);
}

void setupCam() {
    float d = camHeight / max(sin(camPitch), 0.1);
    float ang = g_t * 0.08 * orbitSpeed + 0.6;
    vec2 c = vec2(g_t * travel * 0.28, 1.3 * sin(g_t * 0.045 * travel + 1.0) + 2.5);
    vec3 ta = vec3(c.x, 0.08 * g_hScale, c.y);
    vec3 off = vec3(cos(ang), 0.0, sin(ang)) * d * cos(camPitch);
    g_ro = vec3(ta.x - off.x, camHeight + 0.015 * sin(g_t * 0.5), ta.z - off.z);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    g_lens = 1.55;
    float az = radians(lightAz), el = radians(lightEl);
    g_L = normalize(vec3(cos(el) * cos(az), sin(el), cos(el) * sin(az)));
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    g_ar = audioReact;
    g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_back = backColor.rgb * backAmt;

    if (PASSINDEX <= 5) {
        if (FRAMEINDEX < 2) { gl_FragColor = seedField(fragCoord / R); return; }
        gl_FragColor = simStep(fragCoord);
        return;
    }

    if (PASSINDEX == 6) {
        vec2 uv = (fragCoord - 0.5 * R) / R.y;
        // sim buffer size: the scene pass is marchRes·canvas, the sim is simRes·canvas
        g_simR = floor(R / marchRes * simRes);
        // 50 sim texels per world unit: feature size is the same at any canvas resolution
        g_tile = g_simR / 50.0 * scale;
        g_hScale = 0.15 * height * (1.0 + 0.18 * g_ar * g_bass);
        setupCam();
        gl_FragColor = renderScene(uv);
        return;
    }

    // ── final ────────────────────────────────────────────────────────────
    vec2 st = fragCoord / R;
    vec2 uvc = (fragCoord - 0.5 * R) / R.y;
    vec2 asp = vec2(R.y / R.x, 1.0);
    vec4 c0 = texture2D(rdScene, st);
    float ab = aberration * 0.002 * length(uvc);
    vec2 dir = normalize(uvc + 1e-4);
    vec3 col = vec3(texture2D(rdScene, st + dir * ab * asp).r, c0.g, texture2D(rdScene, st - dir * ab * asp).b);
    // bloom: bright ridges/speculars only
    float thr = 0.85 + lum(g_back) * 2.0;
    vec3 bl = vec3(0.0);
    for (int i = 0; i < 14; i++) {
        float fi = float(i);
        float a = fi * 2.399963 + hash12(fragCoord) * 6.28;
        float rr = sqrt((fi + 0.5) / 14.0) * 0.028;
        vec2 of = vec2(cos(a), sin(a)) * rr * asp;
        vec3 s = texture2D(rdScene, st + of).rgb;
        bl += max(s - thr, 0.0) * (1.0 - rr / 0.028);
    }
    bl /= 14.0;
    col += bl * bloom * 1.6;
    col *= mix(1.0, 0.92 + 0.16 * g_lev, g_ar);
    col = aces(col * 0.98);
    // grain
    float g1 = (hash12(fragCoord + fract(g_t) * 61.0) - 0.5) * 0.05;
    col += g1 * grain;
    col *= 1.0 - 0.11 * smoothstep(0.45, 1.4, length(uvc));
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.5);
    float lumHD = lum(col);
    col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
    float alphaOut = transparentBg ? c0.a : mix(1.0, backColor.a, backAmt * step(0.001, 1.0 - backColor.a));
    gl_FragColor = vec4(col, alphaOut);
}
