/*{
  "DESCRIPTION": "Kelp — a giant-kelp forest where every strand is a real rope simulation: thirty-six stalks of linked nodes, each node buoyed upward by its gas bladders and dragged by a living current (a long ocean swell surging back and forth, a cross-swell, slow turbulence), integrated every frame and held to length node by node, so the forest leans, surges and recoils like the real thing. Long translucent golden blades with ruffled, rippling margins and a faint midrib stream off the stalks and glow as the sun comes through them, with little gas bladders at their bases. Sharp-edged sunbeams fan down through the gaps in the canopy, flickering with the surface waves and glittering with particles; overhead the surface shimmers with Snell's window and a rippling sun, caustics dance over up-facing blades and the sand, a milling bait ball of over a hundred silver fish circles and turns together, flashing as their flanks catch the light, and the camera glides slowly in, up and down through the forest while the water falls away to deep navy. Bass deepens the swell and the glow, beats push a surge through the forest and flash the caustics, mids stir the turbulence, highs sparkle the fish and the drifting particles.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #9",
  "CATEGORIES": ["Generator", "3D", "Nature", "Simulation", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Speed",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "camDrift",    "LABEL": "Camera Drift",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "swell",       "LABEL": "Swell",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Current" },
    { "NAME": "current",     "LABEL": "Current",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.45,  "GROUP": "Current" },
    { "NAME": "turbulence",  "LABEL": "Turbulence",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Current" },
    { "NAME": "buoyancy",    "LABEL": "Buoyancy",         "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.3, "GROUP": "Kelp" },
    { "NAME": "kelpLength",  "LABEL": "Kelp Length",      "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 1.3, "DEFAULT": 1.0, "GROUP": "Kelp" },
    { "NAME": "bladeSize",   "LABEL": "Blade Size",       "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Kelp" },
    { "NAME": "flutter",     "LABEL": "Blade Flutter",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Kelp" },
    { "NAME": "fish",        "LABEL": "Fish School",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Life" },
    { "NAME": "snow",        "LABEL": "Marine Snow",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Life" },
    { "NAME": "camPitch",    "LABEL": "Look Up",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.72, "GROUP": "Camera" },
    { "NAME": "pitchSway",   "LABEL": "Tilt Sway",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Camera" },
    { "NAME": "sunEl",       "LABEL": "Sun Height",       "TYPE": "float", "AUDIO": false, "MIN": 15.0, "MAX": 85.0, "DEFAULT": 34.0, "GROUP": "Light" },
    { "NAME": "godRays",     "LABEL": "God Rays",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "caustics",    "LABEL": "Caustics",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "glow",        "LABEL": "Frond Glow",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "fog",         "LABEL": "Water Depth Haze", "TYPE": "float", "MIN": 0.2,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "waterDeep",   "LABEL": "Deep Water",       "TYPE": "color", "DEFAULT": [0.0, 0.018, 0.04, 1.0],  "GROUP": "Color" },
    { "NAME": "waterShallow","LABEL": "Shallow Water",    "TYPE": "color", "DEFAULT": [0.01, 0.24, 0.32, 1.0], "GROUP": "Color" },
    { "NAME": "kelpColor",   "LABEL": "Kelp",             "TYPE": "color", "DEFAULT": [0.36, 0.22, 0.04, 1.0], "GROUP": "Color" },
    { "NAME": "kelpGlow",    "LABEL": "Kelp Backlight",   "TYPE": "color", "DEFAULT": [1.0, 0.68, 0.16, 1.0],  "GROUP": "Color" },
    { "NAME": "sunColor",    "LABEL": "Sunlight",         "TYPE": "color", "DEFAULT": [0.85, 1.0, 0.92, 1.0],  "GROUP": "Color" },
    { "NAME": "sandColor",   "LABEL": "Sand",             "TYPE": "color", "DEFAULT": [0.55, 0.47, 0.32, 1.0], "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "bloom",       "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG", "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "ropeBuf", "PERSISTENT": true },
    { "TARGET": "metaBuf" },
    { "TARGET": "sceneBuf" },
    { "TARGET": "rayBuf", "WIDTH": "$WIDTH/4", "HEIGHT": "$HEIGHT/4" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Kelp — CURVEBALL pack #9
//   pass 0  ropeBuf (persistent): Verlet / follow-the-leader rope physics.
//           texel (node j, row 3s+c) of strand s: c0 = position hi, c1 = lo,
//           c2 = velocity. positions are stored relative to the straight rest
//           pose and split hi/lo so half-float buffers keep sub-mm precision.
//           every texel re-walks its strand from the holdfast, so each node is
//           constrained against its parent's NEW position (consistent chain).
//   pass 1  metaBuf: per-strand + per-chunk screen bounding boxes (culling),
//           a 32-px screen-tile strand bitmask (36 bits as 4 x 9-bit integers,
//           exact in half float) and
//           and a top-down canopy shade map for god rays / caustic shadows.
//   pass 2  sceneBuf: projected kelp ribbons (stalks, ruffled blades, gas
//           bladders) composited front-to-back, water column, Snell's window,
//           sand, volumetric sun shafts, fish school, marine snow.
//   pass 3  rayBuf (quarter res): volumetric sun shafts through the canopy,
//           marched to the kelp / background depth stored in sceneBuf.a
//   pass 4  final: depth-aware ray upsample, bloom, fringe, grain, ACES, HD
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define NS   36
#define NN   24
#define NSF  36.0
#define NNF  24.0
#define SURF 14.0
#define TILE 32.0
#define TILE_Y 170.0
#define NODE_Y 10.0

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float hash13(vec3 p3) { p3 = fract(p3 * 0.1031); p3 += dot(p3, p3.zyx + 31.32); return fract((p3.x + p3.y) * p3.z); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float vn3(vec3 x) {
    vec3 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    float a = mix(mix(hash13(p), hash13(p + vec3(1,0,0)), f.x), mix(hash13(p + vec3(0,1,0)), hash13(p + vec3(1,1,0)), f.x), f.y);
    float b = mix(mix(hash13(p + vec3(0,0,1)), hash13(p + vec3(1,0,1)), f.x), mix(hash13(p + vec3(0,1,1)), hash13(p + vec3(1,1,1)), f.x), f.y);
    return mix(a, b, f.z);
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

// ── shared state ─────────────────────────────────────────────────────────
float g_t, g_st, g_ar, g_bass, g_mid, g_high, g_beat, g_lev;
vec3  g_ro, g_uu, g_vv, g_ww, g_Ld, g_sunU;
float g_F;

// texel-centre fetches into the region-addressed buffers (full canvas size)
vec2 g_R;   // full canvas size (region-addressed buffers live at canvas size)
vec4 ropeAt(float j, float row) { return texture2D(ropeBuf, (vec2(j, row) + 0.5) / g_R); }
vec4 metaAt(float x, float y)   { return texture2D(metaBuf, (vec2(x, y) + 0.5) / g_R); }

// ── forest layout ────────────────────────────────────────────────────────
// strand s: holdfast position on the sea floor and segment length
vec4 strandInfo(float s) {
    float h1 = hash11(s * 12.91 + 1.7), h2 = hash11(s * 7.37 + 5.1), h3 = hash11(s * 3.13 + 9.4);
    // depth from the camera rail: a few hero strands close, most spread back
    float u = (s + 0.5) / NSF;
    float d = 7.0 + 38.0 * pow(fract(u + 0.37 * h3 * 0.0), 1.25);
    d += (h3 - 0.5) * 3.0;
    // lateral spread follows the frustum so the forest fills the frame
    float side = (h1 * 2.0 - 1.0);
    side = sign(side) * pow(abs(side), 0.8);
    float x = side * 0.62 * d + (h2 - 0.5) * 2.0;
    float segLen = (SURF + 2.5) / (NNF - 1.0) * kelpLength * mix(0.82, 1.18, h2);
    return vec4(x, 0.0, -6.0 + d, segLen);
}
vec3 restPos(vec4 info, float j) { return vec3(info.x, j * info.w, info.z); }

// decode a node's world position from the rope buffer
vec3 nodePos(vec4 info, float s, float j) {
    vec3 hi = ropeAt(j, s * 3.0).xyz;
    vec3 lo = ropeAt(j, s * 3.0 + 1.0).xyz;
    return restPos(info, j) + hi + lo / 16.0;
}

// ── the living current ───────────────────────────────────────────────────
vec3 flowAt(vec3 p, float t) {
    vec2 cd = vec2(0.9701, 0.2425);
    float depthF = exp(-(SURF - p.y) * 0.06);
    float sw = swell * (0.9 + 0.6 * g_ar * g_bass);
    float ph  = dot(p.xz, cd) * 0.21 - t * 0.85;
    float ph2 = dot(p.xz, vec2(-0.287, 0.958)) * 0.16 - t * 0.57 + 1.7;
    vec3 u = vec3(cd.x, 0.0, cd.y) * (current * 0.25 + sw * 1.45 * depthF * cos(ph));
    u.y += sw * 0.5 * depthF * sin(ph);
    u += vec3(-0.287, 0.0, 0.958) * sw * 0.45 * depthF * cos(ph2);
    float tb = turbulence * (0.32 + 0.35 * g_ar * g_mid);
    vec3 q = p * 0.33 + vec3(0.0, 0.0, t * 0.13);
    u += tb * 1.3 * vec3(vn3(q) - 0.5, (vn3(q + vec3(5.2, 1.3, 2.7)) - 0.5) * 0.5, vn3(q + vec3(2.1, 7.7, 4.4)) - 0.5);
    u += vec3(cd.x, 0.0, cd.y) * g_beat * g_ar * 0.45;
    return u;
}

// ═════════════════════════════════════════════════════════════════════════
//  PASS 0 — rope physics
// ═════════════════════════════════════════════════════════════════════════
vec4 ropePass(vec2 fc) {
    float ix = floor(fc.x), iy = floor(fc.y);
    if (ix >= NNF || iy >= NSF * 3.0) return vec4(0.0);
    float s = floor(iy / 3.0);
    float c = iy - s * 3.0;
    float k = ix;
    vec4 info = strandInfo(s);
    float segLen = info.w;
    vec3 anchor = restPos(info, 0.0);
    float lean = 0.55 + 0.45 * current;
    float surfY = SURF - 0.22 - 0.18 * hash11(s * 5.3);

    float dt = clamp(TIMEDELTA, 0.0, 1.0 / 30.0) * speed;
    bool init = FRAMEINDEX < 3;

    vec3 parent = anchor;
    vec3 outP = anchor, outV = vec3(0.0);
    bool bad = false;
    for (int jj = 1; jj < NN; jj++) {
        float j = float(jj);
        if (j > k) break;
        vec3 pn, vn;
        if (init || bad) {
            // a believable pose: leaning down-current, canopy lying on the surface
            float hgt = j * segLen;
            vec3 target = anchor + vec3(0.0, hgt, 0.0) + vec3(0.97, 0.0, 0.24) * lean * pow(hgt / 9.0, 1.6) * 1.6;
            target.y = min(target.y, surfY);
            vec3 d = target - parent;
            pn = parent + d / max(length(d), 1e-4) * segLen;
            vn = vec3(0.0);
        } else {
            vec3 hi = ropeAt(j, s * 3.0).xyz;
            vec3 lo = ropeAt(j, s * 3.0 + 1.0).xyz;
            vec3 v  = ropeAt(j, s * 3.0 + 2.0).xyz;
            vec3 p = restPos(info, j) + hi + lo / 16.0;
            float chk = dot(hi, hi) + dot(v, v);
            if (!(chk < 4000.0)) { bad = true; }
            if (bad) {
                float hgt = j * segLen;
                vec3 target = anchor + vec3(0.0, hgt, 0.0);
                target.y = min(target.y, surfY);
                vec3 d = target - parent;
                pn = parent + d / max(length(d), 1e-4) * segLen;
                vn = vec3(0.0);
            } else if (dt < 1e-5) {
                pn = p; vn = v;
            } else {
                vec3 rel = flowAt(p, g_st) - v;
                float buoy = buoyancy * (0.55 + 1.6 * j / NNF);
                vec3 acc = 1.9 * rel * (0.3 + length(rel)) + vec3(0.0, buoy, 0.0);
                v += acc * dt;
                v *= exp(-0.35 * dt);
                pn = p + v * dt;
                pn.y = clamp(pn.y, 0.08, surfY);
                // follow-the-leader inextensibility against the parent's new position
                vec3 d = pn - parent;
                pn = parent + d / max(length(d), 1e-4) * segLen;
                vn = (pn - p) / dt * 0.985;
                float vl = length(vn);
                if (vl > 7.0) vn *= 7.0 / vl;
            }
        }
        parent = pn;
        if (j == k) { outP = pn; outV = vn; }
    }

    vec3 rel = outP - restPos(info, k);
    vec3 hi = floor(rel * 16.0) / 16.0;
    if (c < 0.5) return vec4(hi, 1.0);
    if (c < 1.5) return vec4((rel - hi) * 16.0, 1.0);
    return vec4(outV, 1.0);
}

// ═════════════════════════════════════════════════════════════════════════
//  camera
// ═════════════════════════════════════════════════════════════════════════
void setupCam() {
    float t = g_t * camDrift;
    float trav = 0.5 - 0.5 * cos(t * 0.021);           // slow glide in and out of the forest
    g_ro = vec3(2.6 * sin(t * 0.031), 1.6 + 4.2 * trav + 0.6 * sin(t * 0.043), -7.0 + 10.0 * trav + 1.2 * sin(t * 0.017 + 0.6));
    float yaw = 0.18 * sin(t * 0.027 + 0.4) + 0.06;
    float look = sin(t * 0.033 + 2.1);                  // sometimes tilts down to the sand
    float pitch = mix(0.08, 0.62, camPitch) + pitchSway * (0.16 * look - 0.34 * smoothstep(0.55, 1.0, -look));
    vec3 fw = normalize(vec3(sin(yaw) * cos(pitch), sin(pitch), cos(yaw) * cos(pitch)));
    g_ww = fw;
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    g_F = 1.12;
    // sun: refracted into the water (Snell), slightly ahead-left of the camera
    float elA = radians(sunEl);
    float elW = PI * 0.5 - asin(clamp(cos(elA) / 1.333, -1.0, 1.0));
    vec2 az = normalize(vec2(-0.18, 1.0));
    g_sunU = normalize(vec3(az.x * cos(elW), sin(elW), az.y * cos(elW)));
    g_Ld = -g_sunU;
}
vec3 project(vec3 p) {
    vec3 q = p - g_ro;
    float z = max(dot(q, g_ww), 0.05);
    return vec3(dot(q, g_uu) * g_F / z, dot(q, g_vv) * g_F / z, z);
}

// ═════════════════════════════════════════════════════════════════════════
//  PASS 1 — culling boxes + canopy shade map
// ═════════════════════════════════════════════════════════════════════════
// shade map covers world x in [-28,28], z in [-6,50]
vec2 shadeUV(vec2 xz) { return (vec2(64.0, 0.0) + clamp((xz - vec2(-28.0, -6.0)) / 56.0, 0.0, 1.0) * 159.0 + 0.5) / g_R; }

vec4 metaPass(vec2 fc) {
    float ix = floor(fc.x), iy = floor(fc.y);
    if (ix < NSF && iy < 5.0) {
        float s = ix;
        vec4 info = strandInfo(s);
        float j0 = 0.0, j1 = NNF - 1.0;
        if (iy > 0.5) { j0 = (iy - 1.0) * 6.0; j1 = min(j0 + 6.0, NNF - 1.0); }
        vec2 mn = vec2(1e5), mx = vec2(-1e5);
        float reach = bladeSize * 3.7 + 0.15;
        for (int jj = 0; jj < NN; jj++) {
            float j = float(jj);
            if (j < j0 || j > j1) continue;
            vec3 pr = project(nodePos(info, s, j));
            float m = reach * g_F / pr.z + 0.004;
            mn = min(mn, pr.xy - m);
            mx = max(mx, pr.xy + m);
        }
        return vec4(mn, mx);
    }
    // per-node projected geometry: texels (j, NODE_Y + 2s) and (j, NODE_Y + 2s + 1)
    if (ix < NNF && iy >= NODE_Y && iy < NODE_Y + NSF * 2.0) {
        float s = floor((iy - NODE_Y) * 0.5);
        float row = iy - NODE_Y - s * 2.0;
        float j = ix;
        vec4 info = strandInfo(s);
        vec3 pa = project(nodePos(info, s, j));
        if (row < 0.5) return vec4(pa, 1.0);
        vec3 pb = project(nodePos(info, s, min(j + 1.0, NNF - 1.0)));
        float tt = g_t * speed;
        float bladeLen = 2.7 * bladeSize;
        vec2 ab = pb.xy - pa.xy;
        vec2 tdir = ab / max(length(ab), 1e-6);
        float side = mod(j, 2.0) < 0.5 ? 1.0 : -1.0;
        float fl = flutter * (0.28 * sin(tt * 2.1 + j * 1.37 + s * 2.1) + 0.12 * sin(tt * 3.7 + j * 0.7));
        float ang = side * (0.3 + 0.16 * hash11(s + j * 7.0)) + fl;
        vec2 bdir = vec2(tdir.x * cos(ang) - tdir.y * sin(ang), tdir.x * sin(ang) + tdir.y * cos(ang));
        // blades stream with the swell surge (screen-projected surge direction)
        float surge = cos(dot(vec2(info.x, info.z), vec2(0.9701, 0.2425)) * 0.21 - tt * 0.85);
        vec2 sdir = normalize(vec2(g_uu.x * 0.97 + g_uu.z * 0.24, 0.35));
        bdir = normalize(bdir + sdir * vec2(surge * 0.55 * swell, 0.0) + vec2(0.0, 0.25));
        float bl = bladeLen * mix(0.5, 1.3, hash11(s * 1.3 + j * 5.1)) * smoothstep(1.0, 5.0, j) * g_F / pa.z;
        float bend = (hash11(s * 3.7 + j * 1.9) - 0.5) * 0.5 + 0.18 * sin(tt * 1.3 + j * 0.9 + s);
        return vec4(bdir, bl, bend);
    }
    // screen tiles: texels (x, TILE_Y + y)
    if (iy >= TILE_Y && ix < ceil(g_R.x / TILE) && iy < TILE_Y + ceil(g_R.y / TILE)) {
        vec2 p0 = vec2(ix, iy - TILE_Y) * TILE;
        vec2 t0 = (p0 - 0.5 * g_R) / g_R.y;
        vec2 t1 = (p0 + TILE - 0.5 * g_R) / g_R.y;
        float reach = bladeSize * 3.7 + 0.15;
        vec4 m = vec4(0.0);
        for (int si = 0; si < NS; si++) {
            float s = float(si);
            vec4 info = strandInfo(s);
            bool hit = false;
            vec2 mn = vec2(1e5), mx = vec2(-1e5);
            for (int jj = 0; jj < NN; jj++) {
                float j = float(jj);
                vec3 pr = project(nodePos(info, s, j));
                float mg = reach * g_F / pr.z + 0.004;
                mn = min(mn, pr.xy - mg);
                mx = max(mx, pr.xy + mg);
                // close the box every 6 nodes (same chunks the scene uses)
                if (mod(j, 6.0) > 4.5 || jj == NN - 1) {
                    if (!(t1.x < mn.x || t1.y < mn.y || t0.x > mx.x || t0.y > mx.y)) hit = true;
                    mn = pr.xy - mg; mx = pr.xy + mg;
                }
            }
            if (hit) {
                float ch = floor(s / 9.0);
                float b = exp2(s - ch * 9.0);
                if (ch < 0.5) m.x += b; else if (ch < 1.5) m.y += b; else if (ch < 2.5) m.z += b; else m.w += b;
            }
        }
        return m;
    }
    if (ix >= 64.0 && ix < 224.0 && iy < 160.0) {
        vec2 xz = vec2(-28.0, -6.0) + (vec2(ix - 64.0, iy) / 159.0) * 56.0;
        float occ = 0.0;
        for (int si = 0; si < NS; si++) {
            float s = float(si);
            vec4 info = strandInfo(s);
            if (abs(info.x - xz.x) > 14.0 || abs(info.z - xz.y) > 14.0) continue;
            for (int jj = 8; jj < NN; jj++) {
                float j = float(jj);
                vec3 p = nodePos(info, s, j);
                // shade lands where the refracted sun projects the node down onto xz
                vec2 sh = p.xz + g_Ld.xz / max(-g_Ld.y, 0.2) * (p.y - 4.0) * 0.5;
                vec2 dd = xz - sh;
                float w = 0.25 + 0.75 * smoothstep(6.0, 13.0, p.y);
                occ += w * exp(-dot(dd, dd) * 2.2 / (bladeSize * bladeSize));
            }
        }
        return vec4(exp(-occ * 0.55), 0.0, 0.0, 1.0);
    }
    return vec4(0.0);
}
float canopyLight(vec3 p) {
    vec2 xz = p.xz + g_Ld.xz / max(-g_Ld.y, 0.2) * (4.0 - p.y) * 0.5;
    float sh = texture2D(metaBuf, shadeUV(xz)).r;
    return mix(1.0, sh, smoothstep(SURF - 0.5, SURF - 3.5, p.y));
}

// ═════════════════════════════════════════════════════════════════════════
//  light helpers
// ═════════════════════════════════════════════════════════════════════════
// sharp caustic web: two drifting voronoi layers, edges = bright focus lines
float causticLayer(vec2 p, float t) {
    vec2 i = floor(p), f = fract(p);
    float d1 = 8.0, d2 = 8.0;
    for (int y = -1; y <= 1; y++)
    for (int x = -1; x <= 1; x++) {
        vec2 g = vec2(float(x), float(y));
        vec2 o = hash22(i + g);
        o = 0.5 + 0.42 * sin(t * (0.6 + 0.5 * o) + TAU * o);
        float d = length(g + o - f);
        if (d < d1) { d2 = d1; d1 = d; } else if (d < d2) d2 = d;
    }
    return d2 - d1;
}
float caustic(vec2 p, float t) {
    p += 0.35 * vec2(sin(p.y * 0.7 + t * 0.4), cos(p.x * 0.6 - t * 0.35));
    float a = causticLayer(p * 1.1, t * 1.3);
    float b = causticLayer(p * 1.7 + 3.1, t * 1.7);
    float c = exp(-a * 9.0) + 0.7 * exp(-b * 11.0);
    return c * c * (1.0 + 0.8 * g_ar * g_beat);
}

vec3 fogColor(vec3 rd) {
    float up = smoothstep(-0.2, 0.85, rd.y);
    vec3 c = mix(waterDeep.rgb * 0.6, waterShallow.rgb * 0.75, up * up * up * up);
    // brighter only toward the sun's side of the column
    c += waterShallow.rgb * 0.35 * pow(max(dot(rd, g_sunU), 0.0), 12.0);
    return c;
}

// surface from below + sand floor + water column
vec3 background(vec3 rd, out float tBg) {
    vec3 col;
    if (rd.y > 0.0) {
        float ts = (SURF - g_ro.y) / rd.y;
        tBg = ts;
        vec2 q = g_ro.xz + rd.xz * ts;
        // ripple normal: directional wave trains
        float tt = g_t * speed;
        vec2 rn = vec2(0.0);
        rn += vec2(0.97, 0.24) * cos(dot(q, vec2(0.97, 0.24)) * 1.3 - tt * 1.6) * 0.5;
        rn += vec2(-0.3, 0.95) * cos(dot(q, vec2(-0.3, 0.95)) * 2.1 - tt * 2.0 + 1.0) * 0.35;
        rn += vec2(0.6, -0.8) * cos(dot(q, vec2(0.6, -0.8)) * 3.7 - tt * 2.7 + 2.0) * 0.2;
        rn += (vec2(vnoise2(q * 1.6 + tt * 0.3), vnoise2(q * 1.6 - tt * 0.3 + 7.0)) - 0.5) * 0.6;
        vec3 rdp = normalize(rd + vec3(rn.x, 0.0, rn.y) * 0.05);
        vec3 rds = normalize(rd + vec3(rn.x, 0.0, rn.y) * 0.012);
        float win = smoothstep(0.62, 0.69, rdp.y);
        // inside Snell's window: the sky, and the sun broken by the ripples
        float sunA = acos(clamp(dot(rds, g_sunU), -1.0, 1.0));
        vec3 sky = sunColor.rgb * mix(0.18, 0.55, smoothstep(0.66, 1.0, rdp.y));
        sky += sunColor.rgb * (5.0 * smoothstep(0.022, 0.016, sunA) + 1.2 * exp(-sunA * sunA * 2500.0) + 0.45 * exp(-sunA * sunA * 60.0));
        // outside: total internal reflection of the dark water below
        vec3 tir = waterDeep.rgb * 1.6 + waterShallow.rgb * 0.18 * (0.5 + 0.5 * rn.x);
        col = mix(tir, sky, win);
        // rim of the window shimmers
        col += sunColor.rgb * 0.05 * exp(-abs(rdp.y - 0.655) * 140.0);
        float att = exp(-ts * 0.045 * fog);
        col = mix(fogColor(rd), col, att);
    } else {
        float tf = (g_ro.y - 0.0) / max(-rd.y, 1e-3);
        tBg = tf;
        vec3 p = g_ro + rd * tf;
        // footprint-aware detail: fade high frequencies with distance (no moire)
        float fp = tf * 0.004 / max(-rd.y, 0.05);
        float ripW = 1.0 - smoothstep(0.08, 0.3, fp * 2.2);
        float rip = sin(dot(p.xz, vec2(1.9, 0.7)) * 2.2 + vnoise2(p.xz * 0.7) * 5.0) * ripW;
        float grainS = (hash12(floor(p.xz * 60.0)) - 0.5) * (1.0 - smoothstep(0.004, 0.02, fp));
        vec3 sand = sandColor.rgb * (0.7 + 0.18 * rip + 0.12 * grainS);
        float lightF = canopyLight(p);
        float ca = caustic(p.xz * 0.9, g_t * speed) * (1.0 - smoothstep(0.05, 0.25, fp * 3.0)) + 0.12 * smoothstep(0.05, 0.25, fp * 3.0);
        vec3 lit = sand * (0.08 + 0.45 * lightF) * sunColor.rgb + sunColor.rgb * ca * 1.6 * caustics * lightF * sandColor.rgb * 1.6;
        float att = exp(-tf * 0.05 * fog);
        col = mix(fogColor(rd), lit * 0.85, att);
        tBg = min(tBg, 70.0);
    }
    return col;
}

// ═════════════════════════════════════════════════════════════════════════
//  PASS 2 — scene
// ═════════════════════════════════════════════════════════════════════════
// kelp hit record: x = depth z, y = coverage, z = type (0 stalk, 1 blade, 2 bladder),
// w = packed across coordinate; extra: u along, strand id, node id
vec4  h1 = vec4(1e5, 0.0, 0.0, 0.0), h2 = vec4(1e5, 0.0, 0.0, 0.0);
vec3  e1 = vec3(0.0), e2 = vec3(0.0);

void consider(float z, float a, float type, float across, float along, float s, float j) {
    if (a <= 0.003) return;
    if (z < h1.x) {
        h2 = h1; e2 = e1;
        h1 = vec4(z, a, type, across); e1 = vec3(along, s, j);
    } else if (z < h2.x) {
        h2 = vec4(z, a, type, across); e2 = vec3(along, s, j);
    }
}

void bladeTest(vec2 p, vec2 A, vec2 dir, float len, float wid, float z, float s, float j, float px, float bend) {
    vec2 rel = p - A;
    float u = dot(rel, dir) / len;
    if (u < -0.05 || u > 1.05) return;
    vec2 pr = vec2(-dir.y, dir.x);
    if (abs(dot(rel, pr)) > len * 0.24 + 2.0 * px) return;
    float cl = len * (bend * sin(u * PI) * 0.3 + 0.035 * sin(u * 9.0 + g_t * speed * 2.2 + j));
    float off = dot(rel, pr) - cl;
    float uu = clamp(u, 0.0, 1.0);
    // strap: short stipe taper, long near-parallel blade, pointed tip
    float prof = smoothstep(0.0, 0.1, uu) * (1.0 - smoothstep(0.62, 1.0, uu) * 0.92) * (1.0 - 0.25 * uu);
    float w = wid * prof;
    // ruffled margins: each edge waves on its own (no symmetric lobes)
    float tw = g_t * speed * 3.0;
    float side = off >= 0.0 ? 1.0 : -1.0;
    float ph = side > 0.0 ? 0.0 : 2.1;
    float edge = w * (1.0 + 0.13 * sin(uu * 52.0 + tw + ph + j) + 0.05 * sin(uu * 117.0 - tw * 1.3 + ph * 2.0 + s));
    float d = abs(off) - edge;
    float a = clamp(0.5 - d / px, 0.0, 1.0) * step(0.0, u) * step(u, 1.0);
    consider(z + uu * 0.02, a, 1.0, off / max(edge, 1e-4), uu, s, j);
}

vec3 shadeKelp(vec4 h, vec3 e, vec3 rd) {
    float z = h.x, type = h.z, ac = clamp(h.w, -1.0, 1.0), u = e.x, s = e.y, j = e.z;
    float t = z / max(dot(rd, g_ww), 0.05);
    vec3 P = g_ro + rd * t;
    float lightF = canopyLight(P);
    float depthL = exp(-(SURF - P.y) * 0.05);
    float var = hash11(s * 9.1 + 0.3);
    vec3 base = kelpColor.rgb * mix(0.75, 1.25, var) * mix(vec3(1.0), vec3(1.15, 0.95, 0.7), hash11(j * 3.1 + s));
    // forward scattering: looking toward the sun the blades light up from behind
    float back = pow(max(dot(rd, g_sunU), 0.0), 2.0);
    float thin = type < 0.5 ? 0.25 : (type > 1.5 ? 1.6 : 1.0);
    vec3 trans = kelpGlow.rgb * glow * (1.0 + 0.4 * g_ar * audioBass) * (0.08 + 1.6 * back * back + 0.5 * back) * thin * lightF * depthL;
    // corrugation ridges along the blade + cross-section curvature
    float ridge = 1.0;
    float veins = 1.0;
    if (type > 0.5 && type < 1.5) {
        ridge = 1.0 - 0.35 * exp(-ac * ac * 220.0);                       // slight midrib
        veins = 0.86 + 0.14 * (0.5 + 0.5 * sin(ac * 23.0 + 3.0 * sin(u * 6.0 + j)));  // fine veins along the strap
    }
    float curv = sqrt(max(1.0 - ac * ac, 0.0));
    float top = 0.5 + 0.5 * ac;
    vec3 col = base * (0.05 + 0.4 * lightF * depthL * (0.35 + 0.65 * top)) * sunColor.rgb * ridge * (0.5 + 0.5 * curv);
    col += trans * mix(0.75, 1.0, abs(ac)) * ridge * veins * base * 2.4;
    // glowing, light-catching ruffled margins
    if (type > 0.5 && type < 1.5) col += kelpGlow.rgb * 0.5 * glow * smoothstep(0.72, 0.98, abs(ac)) * lightF * depthL * (0.4 + back);
    // gas bladder: a little lens of golden light with a specular pip
    if (type > 1.5) col += kelpGlow.rgb * (0.25 + 0.6 * back) * lightF * glow + vec3(1.0) * 0.6 * smoothstep(0.55, 0.95, curv) * step(0.2, ac) * lightF;
    // caustics crawling over the fronds
    col += sunColor.rgb * caustic(P.xz * 0.9 + P.y * 0.15, g_t * speed) * 1.1 * caustics * lightF * depthL * (0.25 + 0.75 * top) * (0.5 + 0.5 * curv);
    col += waterShallow.rgb * 0.03;
    float fogA = 1.0 - exp(-z * 0.075 * fog);
    return mix(col, fogColor(rd), fogA);
}

vec4 scenePass(vec2 fc) {
    vec2 uv = (fc - 0.5 * R) / R.y;
    float px = 1.0 / R.y;
    vec3 rd = normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_F);
    float tt = g_t * speed;
    float bladeLen = 2.7 * bladeSize;
    float swPh = -tt * 0.85;

    // ── kelp ribbons ─────────────────────────────────────────────────────
    // tiny canvases can't hold the tile rows: then every strand is a candidate
    vec4 tmask = R.y >= TILE_Y + ceil(R.y / TILE) ? metaAt(floor(fc.x / TILE), TILE_Y + floor(fc.y / TILE)) : vec4(511.0);
    for (int si = 0; si < NS; si++) {
        float s = float(si);
        float tch = floor(s / 9.0);
        float tv = tch < 0.5 ? tmask.x : (tch < 1.5 ? tmask.y : (tch < 2.5 ? tmask.z : tmask.w));
        if (mod(floor(tv / exp2(s - tch * 9.0) + 0.001), 2.0) < 0.5) continue;
        vec4 bb = metaAt(s, 0.0);
        if (uv.x < bb.x || uv.y < bb.y || uv.x > bb.z || uv.y > bb.w) continue;
        float nrow = NODE_Y + s * 2.0;
        for (int ci = 0; ci < 4; ci++) {
            float ch = float(ci);
            vec4 cb = metaAt(s, 1.0 + ch);
            if (uv.x < cb.x || uv.y < cb.y || uv.x > cb.z || uv.y > cb.w) continue;
            float j0 = ch * 6.0;
            vec3 pa = metaAt(j0, nrow).xyz;
            for (int k = 0; k < 6; k++) {
                float j = j0 + float(k);
                if (j > NNF - 2.0) break;
                vec3 pb = metaAt(j + 1.0, nrow).xyz;
                // fully hidden behind two opaque nearer layers, or out of reach: skip
                float nodeReach = (bladeLen * 1.35 + 0.12) * g_F / pa.z;
                vec2 ab = pb.xy - pa.xy;
                if (pa.z > 34.0 / fog || (h2.y > 0.99 && pa.z > h2.x) || length(uv - pa.xy) > nodeReach + length(ab)) { pa = pb; continue; }
                // stalk: tapered capsule between projected nodes
                float hh = clamp(dot(uv - pa.xy, ab) / max(dot(ab, ab), 1e-8), 0.0, 1.0);
                float zz = mix(pa.z, pb.z, hh);
                float rw = (0.075 - 0.035 * (j + hh) / NNF) * g_F / zz;
                vec2 dv = uv - pa.xy - ab * hh;
                float dd = length(dv) - rw;
                if (dd < 2.0 * px) {
                    float sgn = dot(dv, vec2(-ab.y, ab.x)) >= 0.0 ? 1.0 : -1.0;
                    consider(zz, clamp(0.5 - dd / px, 0.0, 1.0), 0.0, sgn * length(dv) / max(rw, 1e-5), hh, s, j);
                }
                if (j >= 2.0) {
                    vec4 bi = metaAt(j, nrow + 1.0);
                    vec2 bdir = bi.xy;
                    float bl = bi.z;
                    vec2 A = pa.xy + bdir * 0.08 * g_F / pa.z;
                    bladeTest(uv, A, bdir, bl, bl * 0.06, pa.z - 0.01, s, j, px, bi.w);
                    // gas bladder at the blade's base
                    vec2 bc = pa.xy + bdir * 0.05 * g_F / pa.z;
                    float br = 0.038 * g_F / pa.z;
                    vec2 bq = uv - bc;
                    float bd = length(bq) - br;
                    if (bd < 2.0 * px && hash11(s * 2.9 + j * 1.7) > 0.35)
                        consider(pa.z - 0.02, clamp(0.5 - bd / px, 0.0, 1.0), 2.0, clamp(dot(bq, vec2(0.0, 1.0)) / br, -1.0, 1.0), 0.5, s, j);
                }
                pa = pb;
            }
        }
    }

    // ── background, kelp composite ───────────────────────────────────────
    float tBg;
    vec3 bg = vec3(0.0); tBg = 70.0;
    if (h1.y < 0.995) bg = background(rd, tBg);
    float cov = h1.y + (1.0 - h1.y) * h2.y;
    vec3 col = bg;
    if (h2.y > 0.0 && h1.y < 0.995) col = mix(col, shadeKelp(h2, e2, rd), h2.y);
    if (h1.y > 0.0) col = mix(col, shadeKelp(h1, e1, rd), h1.y);

    float tEnd = tBg;
    float cosV = max(dot(rd, g_ww), 0.05);
    if (h1.y > 0.5) tEnd = min(tEnd, h1.x / cosV);
    else if (h2.y > 0.5) tEnd = min(tEnd, h2.x / cosV);
    tEnd = min(tEnd, 48.0);

    // ── fish school: a milling bait ball (torus of circling fish) ───────
    if (fish > 0.01) {
        vec3 sc = vec3(g_ro.x + 3.5 * sin(tt * 0.043), max(g_ro.y + 1.5, 4.0) + 1.5 * sin(tt * 0.031), g_ro.z + 13.0 + 2.5 * cos(tt * 0.037));
        vec3 pc = project(sc);
        float rad = 3.4;
        if (pc.z > 1.5 && length(uv - pc.xy) < (rad + 1.2) * g_F / pc.z) {
            // the whole school turns together: milling direction eases between +1 and -1
            float dirS = sin(tt * 0.09 + 0.6 * sin(tt * 0.031));
            float mill = sign(dirS) * smoothstep(0.0, 0.35, abs(dirS));
            float turn = 1.0 - smoothstep(0.0, 0.35, abs(dirS));     // 1 while the school is turning
            float tilt = 0.35 * sin(tt * 0.07);
            vec3 ax = normalize(vec3(sin(tilt), cos(tilt), 0.25 * sin(tt * 0.05)));
            vec3 e1 = normalize(cross(ax, vec3(0.0, 0.0, 1.0)));
            vec3 e2 = cross(ax, e1);
            float nF = 130.0 * fish;
            for (int i = 0; i < 260; i++) {
                float fi = float(i);
                if (fi >= nF) break;
                vec3 h = vec3(hash11(fi * 1.71), hash11(fi * 3.13 + 2.0), hash11(fi * 5.37 + 4.0));
                float r = rad * (0.45 + 0.55 * sqrt(h.x));
                float ang = h.y * TAU + mill * tt * (0.55 + 0.25 * h.z) * (2.6 / r) + 0.2 * sin(tt * 0.5 + fi);
                float hgt = (h.z - 0.5) * 2.4 + 0.25 * sin(tt * 0.7 + fi * 0.3);
                vec3 fp = sc + (e1 * cos(ang) + e2 * sin(ang)) * r + ax * hgt;
                vec3 pr = project(fp);
                if (pr.z < 0.6) continue;
                if (h1.y > 0.5 && pr.z > h1.x) continue;
                vec2 q = uv - pr.xy;
                float sz = 0.3 * g_F / pr.z;
                if (dot(q, q) > sz * sz * 2.2) continue;
                vec3 tang = normalize(-e1 * sin(ang) + e2 * cos(ang)) * (mill >= 0.0 ? 1.0 : -1.0);
                vec3 pr2 = project(fp + tang * 0.25);
                vec2 fdir = pr2.xy - pr.xy;
                float fl = length(fdir);
                if (fl < 1e-6) continue;
                fdir /= fl;
                // foreshortening: fish heading toward/away from camera look short
                float fore = clamp(fl / (0.25 * g_F / pr.z), 0.25, 1.0);
                vec2 lq = vec2(dot(q, fdir), dot(q, vec2(-fdir.y, fdir.x))) / sz;
                lq.x /= fore;
                float x = lq.x;
                float hw = 0.2 * sqrt(max(1.0 - x * x, 0.0)) * (x > 0.0 ? 1.0 : 0.75);
                // forked tail: two lobes spreading behind the peduncle
                float tx = -0.85 - x;
                float tl = (tx > 0.0 && tx < 0.5) ? tx * 0.75 : 0.0;
                float fork = (tx > 0.18 && tx < 0.5) ? (tx - 0.18) * 0.55 : 0.0;
                float ay = abs(lq.y);
                float dB = min(ay - hw, max(ay - tl, fork - ay));
                float fa = clamp(0.5 - dB * sz / px, 0.0, 1.0) * step(-1.35, x) * step(x, 1.0);
                if (fa <= 0.0) continue;
                // which body side faces world-up on screen: dark dorsal, silver belly
                vec2 sideV = vec2(-fdir.y, fdir.x);
                float upS = dot(sideV, vec2(0.0, 1.0)) >= 0.0 ? 1.0 : -1.0;
                float yn = clamp(lq.y * upS / max(hw, 0.05), -1.0, 1.0);   // +1 dorsal, -1 belly
                vec3 dors = vec3(0.02, 0.06, 0.09);
                vec3 belly = vec3(0.55, 0.66, 0.72);
                vec3 fc3 = mix(belly, dors, smoothstep(-0.25, 0.45, yn));
                // silver flank: mirror the light when the flank faces between sun and camera
                vec3 side = normalize(cross(tang, ax));
                float glint = pow(max(dot(reflect(-g_sunU, side), -rd), 0.0), 12.0) + pow(max(dot(reflect(-g_sunU, -side), -rd), 0.0), 12.0);
                glint *= (0.6 + 1.6 * turn) * (1.0 + 1.2 * g_ar * g_high);
                // crisp specular band along the lateral line
                float band = smoothstep(0.22, 0.0, abs(yn + 0.05)) * step(-0.85, x);
                fc3 *= 0.4 + 0.6 * canopyLight(fp);
                fc3 += vec3(0.9, 0.97, 1.0) * glint * (0.5 + 2.2 * band);
                fc3 += vec3(0.7, 0.85, 0.95) * band * 0.12;
                // eye
                fc3 = mix(fc3, vec3(0.0), smoothstep(0.06, 0.03, length(lq - vec2(0.72, 0.03 * upS))));
                float fogA = 1.0 - exp(-pr.z * 0.075 * fog);
                col = mix(col, mix(fc3, fogColor(rd), fogA), fa);
            }
        }
    }

    // ── marine snow: three depth layers of drifting particles ────────────
    if (snow > 0.01) {
        for (int L = 0; L < 3; L++) {
            float fl = float(L);
            float scl = 9.0 + fl * 8.0;
            vec2 drift = vec2(0.012 * sin(tt * 0.4 + fl) + g_ro.x * (0.02 + 0.015 * fl), -tt * (0.006 + 0.004 * fl));
            vec2 q = uv * scl + drift * scl;
            vec2 id = floor(q), f = fract(q);
            vec2 o = hash22(id + fl * 17.0);
            float pres = step(1.0 - 0.55 * snow, hash12(id * 1.3 + fl));
            float rr = (0.035 + 0.05 * o.x) * (L == 0 ? 2.2 : 1.0);
            float d = length(f - (0.2 + 0.6 * o));
            float soft = L == 0 ? 0.6 : 0.25;
            float m = smoothstep(rr, rr * soft, d) * pres;
            float tw = 0.6 + 0.4 * sin(tt * (1.0 + o.y * 2.0) + o.x * 30.0);
            col += mix(waterShallow.rgb, sunColor.rgb, 0.6) * m * (0.12 + 0.12 * fl) * tw * (1.0 + 0.8 * g_ar * g_high) * (0.6 + 0.6 * smoothstep(-0.3, 0.6, rd.y));
        }
    }

    // alpha packs coverage (3 bits) and the shaft march end distance
    return vec4(col, floor(cov * 7.0 + 0.5) * 64.0 + tEnd);
}


// ═════════════════════════════════════════════════════════════════════════
//  PASS 3 — volumetric sun shafts (half res)
// ═════════════════════════════════════════════════════════════════════════
vec4 raysPass(vec2 fc) {
    vec2 fcF = fc * 4.0 + 1.5;
    vec2 uv = (fcF - 0.5 * g_R) / g_R.y;
    vec3 rd = normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_F);
    float tt = g_t * speed;
    float a = texture2D(sceneBuf, fc / R).a;
    float tEnd = a - floor(a / 64.0) * 64.0;
    float ign = hash12(fc + 0.37);
    float acc = 0.0;
    const int RSTEPS = 16;
    float dtR = tEnd / float(RSTEPS);
    for (int i = 0; i < RSTEPS; i++) {
        float tr = (float(i) + ign) * dtR;
        vec3 p = g_ro + rd * tr;
        vec2 S = p.xz + g_Ld.xz / max(-g_Ld.y, 0.2) * (SURF - p.y);
        float sh = vnoise2(S * 0.32 + vec2(tt * 0.05, -tt * 0.03));
        sh = 0.75 * sh + 0.25 * vnoise2(S * 1.3 - vec2(tt * 0.22, tt * 0.09));   // surface flicker
        float shaft = smoothstep(0.66, 0.72, sh);
        float depthA = exp(-(SURF - p.y) * 0.075);
        acc += shaft * canopyLight(p) * depthA * exp(-tr * 0.035 * fog);
    }
    acc *= dtR;
    float g = 0.6, cth = dot(rd, g_sunU);
    float phase = min((1.0 - g * g) / pow(1.0 + g * g - 2.0 * g * cth, 1.5), 4.0) * 0.06;
    float x = acc * (0.014 + phase) * godRays * 1.7 * (1.0 + 0.3 * g_ar * g_lev);
    // soft shoulder: beams stay distinct instead of fusing into a white blob toward the sun
    vec3 rays = sunColor.rgb * min(x, 2.5) * 0.3;
    return vec4(rays, tEnd);
}

// ═════════════════════════════════════════════════════════════════════════
void main() {
    vec2 fc = gl_FragCoord.xy;
    g_ar = audioReact;
    g_t = TIME;
    g_st = TIME * speed;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_R = PASSINDEX == 3 ? R * 4.0 : R;

    if (PASSINDEX == 0) { gl_FragColor = ropePass(fc); return; }
    setupCam();
    if (PASSINDEX == 1) { gl_FragColor = metaPass(fc); return; }
    if (PASSINDEX == 2) { gl_FragColor = scenePass(fc); return; }
    if (PASSINDEX == 3) { gl_FragColor = raysPass(fc); return; }

    // ── final ────────────────────────────────────────────────────────────
    vec2 st = fc / R;
    vec2 uvc = (fc - 0.5 * R) / R.y;
    vec2 asp = vec2(R.y / R.x, 1.0);
    vec4 c0 = texture2D(sceneBuf, st);
    float ab = aberration * 0.0018 * length(uvc);
    vec2 dir = normalize(uvc + 1e-4);
    vec3 col = vec3(texture2D(sceneBuf, st + dir * ab * asp).r, c0.g, texture2D(sceneBuf, st - dir * ab * asp).b);
    // shafts: 12-tap depth-aware blur of the half-res ray buffer
    float covQ = floor(c0.a / 64.0);
    float tMe = c0.a - covQ * 64.0;
    vec3 ray = vec3(0.0); float wsum = 0.0;
    for (int i = 0; i < 16; i++) {
        float fi = float(i);
        float an = fi * 2.399963 + 0.7;
        float rr = sqrt((fi + 0.5) / 16.0) * 14.0;
        vec4 rs = texture2D(rayBuf, st + vec2(cos(an), sin(an)) * rr / R);
        float w = exp(-abs(rs.a - tMe) * 0.35) * (1.0 - 0.6 * rr / 14.0);
        ray += rs.rgb * w; wsum += w;
    }
    ray /= max(wsum, 1e-4);
    col += ray;
    // particulate glittering inside the beams (only where shaft light passes)
    float rl = lum(ray);
    if (rl > 0.01) {
        float tt = g_t * speed;
        for (int L = 0; L < 2; L++) {
            float fl = float(L);
            vec2 q = fc / (5.0 + fl * 4.0) + vec2(0.0, -tt * (3.0 + fl * 2.0)) + vec2(sin(tt * 0.3 + fl) * 4.0, 0.0);
            vec2 id = floor(q), f = fract(q);
            vec2 o = hash22(id + fl * 31.0);
            float m = smoothstep(0.16, 0.0, length(f - 0.2 - 0.6 * o)) * step(0.94, hash12(id * 1.7 + fl));
            float tw = 0.5 + 0.5 * sin(tt * (3.0 + 4.0 * o.x) + o.y * 40.0);
            col += sunColor.rgb * m * tw * min(rl * 7.0, 1.4) * (1.0 + g_ar * g_high);
        }
    }
    float thr = 0.75;
    vec3 bl = vec3(0.0);
    for (int i = 0; i < 16; i++) {
        float fi = float(i);
        float a = fi * 2.399963 + 1.3;
        float rr = sqrt((fi + 0.5) / 16.0) * 0.03;
        vec2 of = vec2(cos(a), sin(a)) * rr * asp;
        vec3 s = texture2D(sceneBuf, st + of).rgb;
        bl += max(s - thr, 0.0) * (1.0 - rr / 0.03);
    }
    bl /= 16.0;
    col += bl * bloom * 1.8;
    col = hueRot(col, hueShift);
    float l0 = lum(col);
    col = max(mix(vec3(l0), col, saturation), 0.0);
    col *= mix(1.0, 0.86 + 0.2 * audioLevel + 0.18 * audioBass, g_ar);
    col = aces(col * 1.05);
    float g1 = (hash12(fc + fract(g_t) * 61.0) - 0.5) * 0.05;
    col += g1 * grain;
    col *= 1.0 - 0.12 * smoothstep(0.5, 1.4, length(uvc));
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float lumHD = lum(col);
    col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
    gl_FragColor = vec4(col, transparentBg ? covQ / 7.0 : 1.0);
}
