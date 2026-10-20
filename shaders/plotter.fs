/*{
  "DESCRIPTION": "Plotter — a pen plotter drawing 3D scenes as line art, live. Raymarched scenes (stacked blocks, the classic sphere-and-cube CSG with drilled holes, a rippling function plot, a striped skyscraper city, a torus and a sphere lattice) are rendered the way a vector plotter would see them: silhouette and crease edges, contour and grid lines laid on every surface, three levels of hatching where the light falls off, hidden lines removed. The pen plots each sheet stroke by stroke with a glowing pen head and wet-ink highlight, holds the finished drawing, lifts the page and starts the next scene from a new angle — a continuous plotting session. Hand-shake squiggle, pen pressure, ink bleed, paper grain, two pens (edges and hatching) on dark or cream paper. Bass thickens the pen, mids push the plot along, highs shake the hand, the level brightens the ink.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-13 — original, after the look of fogleman/ln and vpype (MIT; no code reused)",
  "CATEGORIES": ["Generator", "3D", "Line Art", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Plot Speed",        "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "orbitSpeed",  "LABEL": "Camera Orbit",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.35, "GROUP": "Speed" },
    { "NAME": "scene",       "LABEL": "Scene (0 = cycle)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 6.0,  "DEFAULT": 0.0,  "GROUP": "Scene" },
    { "NAME": "detail",      "LABEL": "Line Density",      "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.5,  "DEFAULT": 1.0,  "GROUP": "Scene" },
    { "NAME": "outlineAmt",  "LABEL": "Outlines",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 1.0,  "GROUP": "Scene" },
    { "NAME": "contourAmt",  "LABEL": "Contour Lines",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.8,  "GROUP": "Scene" },
    { "NAME": "hatchAmt",    "LABEL": "Hatching",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Scene" },
    { "NAME": "floorOn",     "LABEL": "Ground Grid",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Scene" },
    { "NAME": "lightAz",     "LABEL": "Light Direction",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 360.0,"DEFAULT": 230.0,"GROUP": "Scene" },
    { "NAME": "penWidth",    "LABEL": "Pen Width (px)",    "TYPE": "float", "MIN": 0.6,  "MAX": 4.0,  "DEFAULT": 1.4,  "GROUP": "Pen" },
    { "NAME": "squiggle",    "LABEL": "Hand Shake",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Pen" },
    { "NAME": "pressure",    "LABEL": "Pen Pressure",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Pen" },
    { "NAME": "bleed",       "LABEL": "Ink Bleed",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35, "GROUP": "Pen" },
    { "NAME": "plotMode",    "LABEL": "Live Plotting",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 1.0,  "GROUP": "Plot" },
    { "NAME": "plotTime",    "LABEL": "Plot Time (s)",     "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 60.0, "DEFAULT": 14.0, "GROUP": "Plot" },
    { "NAME": "holdTime",    "LABEL": "Hold Time (s)",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 30.0, "DEFAULT": 5.0,  "GROUP": "Plot" },
    { "NAME": "penGlow",     "LABEL": "Pen Head Glow",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Plot" },
    { "NAME": "paperColor",  "LABEL": "Paper",             "TYPE": "color", "DEFAULT": [0.035, 0.035, 0.05, 1.0], "GROUP": "Color" },
    { "NAME": "inkColor",    "LABEL": "Pen 1 (edges)",     "TYPE": "color", "DEFAULT": [0.93, 0.96, 1.0, 1.0],    "GROUP": "Color" },
    { "NAME": "inkColor2",   "LABEL": "Pen 2 (hatching)",  "TYPE": "color", "DEFAULT": [0.35, 0.8, 1.0, 1.0],     "GROUP": "Color" },
    { "NAME": "inkAmt",      "LABEL": "Ink Strength",      "TYPE": "float", "MIN": 0.2,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "grain",       "LABEL": "Paper Grain",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.3,  "GROUP": "Finish" },
    { "NAME": "vignette",    "LABEL": "Vignette",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 0.3,  "DEFAULT": 0.08, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",   "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "plGeo" },
    { "TARGET": "plInk" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Plotter — 3D line art, plotted live
//   pass 0  plGeo  — raymarched G-buffer (full res):
//                    r = depth, g = surface u, b = surface v, a = id + shade
//   pass 1  plInk  — line extraction with hidden lines already removed:
//                    silhouettes / creases from depth + id discontinuities,
//                    contour + grid iso-lines of (u,v) with analytic AA from
//                    neighbouring taps, 3-level hatching from the shade,
//                    plot-order reveal mask (pen sweep) + pen head
//   pass 2  final  — two pens on paper, ink bleed, grain, vignette
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define FAR  40.0

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
mat3 rotY(float a) { float c = cos(a), s = sin(a); return mat3(c, 0.0, -s, 0.0, 1.0, 0.0, s, 0.0, c); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }

// ── state ────────────────────────────────────────────────────────────────
float g_t, g_ar, g_bass, g_mid, g_high, g_lev;
float g_sheet, g_scene, g_prog, g_lift, g_seed, g_lens;
vec3  g_ro, g_uu, g_vv, g_ww, g_key;

// ── plotting session clock ───────────────────────────────────────────────
void setupClock() {
    float sp = max(speed, 0.001);
    float lift = 1.6;
    float L = (plotTime + holdTime + lift) / sp;
    float tt = g_t + (0.35 * g_ar * g_mid) * 0.0;     // (mid push applied below)
    float sheet = floor(tt / L);
    float u = (tt - sheet * L) * sp;                     // seconds into this sheet (at speed 1)
    float push = 0.9 * g_ar * g_mid;                     // mids nudge the pen along, never backwards
    g_prog = clamp((u + push) / plotTime, 0.0, 1.0);
    g_lift = smoothstep(plotTime + holdTime, plotTime + holdTime + lift, u);
    g_sheet = sheet;
    g_seed = hash11(sheet * 3.1 + 7.0);
    float sc = (scene < 0.5) ? mod(sheet, 6.0) : (floor(scene + 0.5) - 1.0);
    g_scene = sc;
    if (plotMode < 0.5) { g_prog = 1.0; g_lift = 0.0; }
}

// ── scenes ───────────────────────────────────────────────────────────────
// Every object returns distance, a surface (u,v) parametrisation for the
// vector texturing (contours / grids), an id (faces of a cube get their own
// ids so creases show), and a plot order key.

float sdBox(vec3 p, vec3 b) { vec3 q = abs(p) - b; return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0); }
float sdCylY(vec3 p, float r, float h) { vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(r, h); return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)); }
float sdTorus(vec3 p, vec2 t) { vec2 q = vec2(length(p.xz) - t.x, p.y); return length(q) - t.y; }

// cube face parametrisation: which face, and the two in-face coords
void cubeUV(vec3 pl, vec3 b, out float face, out vec2 uv) {
    vec3 a = abs(pl) / b;
    if (a.x >= a.y && a.x >= a.z) { face = (pl.x > 0.0) ? 0.0 : 1.0; uv = pl.zy; }
    else if (a.y >= a.z)          { face = (pl.y > 0.0) ? 2.0 : 3.0; uv = pl.xz; }
    else                          { face = (pl.z > 0.0) ? 4.0 : 5.0; uv = pl.xy; }
}
void sphereUV(vec3 pl, out vec2 uv) {
    vec3 n = normalize(pl);
    uv = vec2(atan(n.z, n.x) / TAU * 4.0, asin(clamp(n.y, -1.0, 1.0)) / PI * 4.0);
}

// function plot height
float plotF(vec2 xz) {
    float s = g_seed;
    return 0.45 * sin(xz.x * (1.1 + s) + 0.5) * cos(xz.y * (0.9 + 0.6 * s) - 0.3) + 0.25 * sin(length(xz) * 2.2 - s * 4.0);
}

#define MAXOBJ 12
// scene evaluation: returns min distance; out params describe the nearest object
float sceneDist(vec3 p, out float id, out vec2 uv, out float order) {
    float d = 1e9; id = 0.0; uv = vec2(0.0); order = 0.0;
    float sc = g_scene;
    if (sc < 0.5) {
        // 0  Blocks — a stack of boxes, ln "hello world" style
        for (int i = 0; i < 7; i++) {
            float fi = float(i);
            float h1 = hash11(fi * 1.7 + g_seed * 9.0), h2 = hash11(fi * 2.9 + g_seed * 5.0), h3 = hash11(fi * 4.3 + g_seed * 3.0);
            vec3 b = vec3(0.35 + 0.5 * h1, 0.3 + 0.9 * h2, 0.35 + 0.5 * h3);
            vec3 c = vec3((h2 - 0.5) * 3.2, b.y, (h3 - 0.5) * 3.2);
            mat3 rt = rotY(h1 * 1.2);
            vec3 pl = rt * (p - c);
            float dd = sdBox(pl, b) - 0.02;
            if (dd < d) { d = dd; float f; vec2 t; cubeUV(pl, b, f, t); id = 1.0 + fi * 6.0 + f; uv = t; order = fi / 7.0 + 0.5 * (pl.y / b.y + 1.0) / 7.0; }
        }
    } else if (sc < 1.5) {
        // 1  CSG — (sphere ∩ cube) − 3 cylinders, ln's example
        vec3 pl = rotY(0.4 + g_seed) * (p - vec3(0.0, 1.0, 0.0));
        float bs = 1.0;
        float dS = length(pl) - bs * 1.3;
        float dB = sdBox(pl, vec3(bs));
        float dC1 = sdCylY(pl, 0.6, 2.0);
        float dC2 = sdCylY(pl.yzx, 0.6, 2.0);
        float dC3 = sdCylY(pl.zxy, 0.6, 2.0);
        float dHole = min(dC1, min(dC2, dC3));
        d = max(max(dS, dB), -dHole);
        // which surface are we on? pick the dominant term for the param
        float mS = dS, mB = dB, mH = -dHole;
        if (mH >= mS && mH >= mB) {
            // inside a drilled hole: angle around the hole + depth along it
            vec3 q = (dC1 <= dC2 && dC1 <= dC3) ? pl : ((dC2 <= dC3) ? pl.yzx : pl.zxy);
            uv = vec2(atan(q.z, q.x) / TAU * 4.0, q.y * 2.0);
            id = 20.0 + ((dC1 <= dC2 && dC1 <= dC3) ? 0.0 : ((dC2 <= dC3) ? 1.0 : 2.0));
        } else if (mS >= mB) { vec2 t; sphereUV(pl, t); uv = t; id = 10.0; }
        else { float f; vec2 t; cubeUV(pl, vec3(bs), f, t); uv = t; id = 11.0 + f; }
        order = 0.5 * (pl.y / 1.3 + 1.0);
        // a small cube beside it for scale
        vec3 p2 = p - vec3(2.6, 0.4, -0.8);
        float d2 = sdBox(p2, vec3(0.4));
        if (d2 < d) { d = d2; float f; vec2 t; cubeUV(p2, vec3(0.4), f, t); id = 30.0 + f; uv = t; order = 0.8 + 0.2 * (p2.y / 0.4 + 1.0) * 0.5; }
    } else if (sc < 2.5) {
        // 2  Plot — a function surface over a square, grid in x and z
        vec2 xz = p.xz;
        float h = plotF(xz) + 0.9;
        float dPlate = sdBox(p - vec3(0.0, 0.9, 0.0), vec3(2.6, 1.4, 2.6));
        float dSurf = (p.y - h) * 0.55;
        d = max(dSurf, dPlate);
        uv = xz; id = 40.0; order = (xz.y + 2.6) / 5.2;
        // base slab
        float dBase = sdBox(p - vec3(0.0, -0.05, 0.0), vec3(2.7, 0.08, 2.7));
        if (dBase < d) { d = dBase; float f; vec2 t; cubeUV(p - vec3(0.0, -0.05, 0.0), vec3(2.7, 0.08, 2.7), f, t); id = 41.0 + f; uv = t; order = 0.0; }
    } else if (sc < 3.5) {
        // 3  City — striped skyscrapers on a grid
        for (int i = 0; i < MAXOBJ; i++) {
            float fi = float(i);
            float gx = mod(fi, 4.0) - 1.5, gz = floor(fi / 4.0) - 1.0;
            float h1 = hash11(fi * 3.3 + g_seed * 11.0), h2 = hash11(fi * 5.1 + g_seed * 13.0);
            float hgt = 0.4 + 1.9 * h1 * h1;
            vec3 b = vec3(0.28 + 0.12 * h2, hgt, 0.28 + 0.12 * h1);
            vec3 c = vec3(gx * 1.15 + (h2 - 0.5) * 0.2, hgt, gz * 1.15 + (h1 - 0.5) * 0.2);
            vec3 pl = p - c;
            float dd = sdBox(pl, b);
            if (dd < d) { d = dd; float f; vec2 t; cubeUV(pl, b, f, t); id = 50.0 + fi * 6.0 + f; uv = (f < 1.5) ? vec2(t.x, pl.y) : ((f < 3.5) ? t : vec2(t.x, pl.y)); order = fi / 12.0 + 0.5 * (pl.y / b.y + 1.0) / 12.0; }
        }
    } else if (sc < 4.5) {
        // 4  Torus with a sphere through it
        vec3 pl = rotY(g_seed * 2.0) * (p - vec3(0.0, 1.1, 0.0));
        pl = vec3(pl.x, pl.y * cos(0.5) - pl.z * sin(0.5), pl.y * sin(0.5) + pl.z * cos(0.5));
        float dT = sdTorus(pl, vec2(1.5, 0.5));
        float dS = length(pl - vec3(1.5, 0.0, 0.0)) - 0.75;
        if (dT < dS) { d = dT; float a1 = atan(pl.z, pl.x); vec2 q = vec2(length(pl.xz) - 1.5, pl.y); float a2 = atan(q.y, q.x); uv = vec2(a1 / TAU * 12.0, a2 / TAU * 4.0); id = 60.0; order = (a1 + PI) / TAU; }
        else { d = dS; vec2 t; sphereUV(pl - vec3(1.5, 0.0, 0.0), t); uv = t; id = 61.0; order = 0.9 + 0.1 * (t.y * 0.25 + 0.5); }
    } else {
        // 5  Sphere lattice 3x3x3
        for (int i = 0; i < 27; i++) {
            float fi = float(i);
            vec3 c = vec3(mod(fi, 3.0) - 1.0, floor(mod(fi, 9.0) / 3.0) - 1.0, floor(fi / 9.0) - 1.0) * 1.3 + vec3(0.0, 1.85, 0.0);
            float r = 0.3 + 0.2 * hash11(fi * 2.1 + g_seed * 7.0);
            float dd = length(p - c) - r;
            if (dd < d) { d = dd; vec2 t; sphereUV(p - c, t); uv = t; id = 70.0 + fi; order = fi / 27.0 + 0.5 * (t.y * 0.25 + 0.5) / 27.0; }
        }
        // rods between neighbours along x
        for (int i = 0; i < 18; i++) {
            float fi = float(i);
            vec3 c = vec3(mod(fi, 2.0) - 0.5, floor(mod(fi, 6.0) / 2.0) - 1.0, floor(fi / 6.0) - 1.0) * 1.3 + vec3(0.0, 1.85, 0.0);
            vec3 q = p - c;
            float dd = length(vec2(length(q.yz), max(abs(q.x) - 0.65, 0.0))) - 0.05;
            if (dd < d) { d = dd; uv = vec2(q.x * 3.0, atan(q.z, q.y) / TAU * 2.0); id = 100.0 + fi; order = 0.97; }
        }
    }
    return d;
}

float mapD(vec3 p) { float id; vec2 uv; float o; return sceneDist(p, id, uv, o); }

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.0015, 0.0);
    return normalize(vec3(mapD(p + e.xyy) - mapD(p - e.xyy), mapD(p + e.yxy) - mapD(p - e.yxy), mapD(p + e.yyx) - mapD(p - e.yyx)));
}

void setupCam() {
    float sa = (g_seed - 0.5) * 3.0 + 0.6;
    float oa = sa + g_t * 0.05 * orbitSpeed;
    float el = 0.45 + 0.35 * hash11(g_sheet * 5.7 + 1.0);
    float dist = 10.5;
    if (g_scene > 1.5 && g_scene < 2.5) dist = 11.5;
    if (g_scene > 4.5) dist = 11.0;
    vec3 ta = vec3(0.0, 1.0, 0.0);
    if (g_scene > 2.5 && g_scene < 3.5) { ta.y = 1.2; dist = 12.5; }
    if (g_scene > 4.5) ta.y = 1.7;
    g_ro = ta + vec3(sin(oa) * cos(el) * dist, sin(el) * dist, cos(oa) * cos(el) * dist);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    g_lens = 2.2;
}

// ── pass 0: G-buffer ─────────────────────────────────────────────────────
vec4 gbuffer(vec2 uv) {
    vec3 rd = normalize(g_uu * uv.x + g_vv * uv.y + g_ww * g_lens);
    vec3 ro = g_ro;
    float tF = (rd.y < -0.0005) ? (-ro.y / rd.y) : FAR;
    float t = 0.0, id = 0.0, order = 0.0; vec2 suv = vec2(0.0);
    bool hit = false;
    for (int i = 0; i < 110; i++) {
        vec3 p = ro + rd * t;
        float d = sceneDist(p, id, suv, order);
        if (d < 0.0008 * (1.0 + t)) { hit = true; break; }
        t += d * 0.9;
        if (t > min(tF, FAR)) break;
    }
    if (hit) {
        vec3 p = ro + rd * t;
        vec3 n = calcNormal(p);
        float shade = clamp(dot(n, g_key) * 0.5 + 0.5, 0.0, 1.0);
        shade = shade * shade;
        // order key: object order, encoded in b channel's fractional... keep it separate: we pack order into the shade slot's upper range
        return vec4(t, suv.x, suv.y, floor(id) + clamp(shade, 0.0, 0.98) * 0.5 + 0.5 * clamp(order, 0.0, 0.98) * 0.98 * 0.0 + 0.0);
    }
    if (tF < FAR && floorOn > 0.001) {
        vec3 p = ro + rd * tF;
        return vec4(tF, p.x, p.z, 200.0 + 0.5 * 0.5);     // ground plane, mid shade
    }
    return vec4(FAR, 0.0, 0.0, 0.0);
}

// Order (plot sequence) is recomputed in pass 1 from id + v so we do not
// need a 5th channel: each object is plotted from one end to the other.
float plotKey(float id, float v, float depth) {
    if (id < 0.5) return 2.0;
    if (id > 199.5) return -0.05;                       // ground grid first
    float obj = floor(id);
    float o = hash11(obj * 0.37 + 2.0);                  // object order
    float along = fract(v * 0.25 + 0.5);                 // sweep along the object
    return clamp(0.08 + 0.85 * (o * 0.75 + along * 0.25), 0.0, 1.0);
}

// ── pass 1: line extraction ──────────────────────────────────────────────
float isoLine(float x, float spacing, float grad, float w) {
    float d = abs(fract(x / spacing + 0.5) - 0.5) * spacing;   // distance to the nearest iso value
    float pxd = d / max(grad, 1e-6);                             // in pixels
    return 1.0 - smoothstep(w * 0.5, w * 0.5 + 1.0, pxd);
}

vec4 inkPass(vec2 fragCoord) {
    vec2 px = 1.0 / R;
    // hand shake: a slow, low-frequency wander of the sample position (whole drawing shakes coherently, like a shaky pen arm)
    vec2 shake = (vec2(vnoise2(fragCoord * 0.006 + g_t * 0.7), vnoise2(fragCoord * 0.006 + 31.0 - g_t * 0.6)) - 0.5) * (2.0 + 10.0 * g_ar * g_high) * squiggle;
    vec2 st = (fragCoord + shake) * px;
    vec4 c  = texture2D(plGeo, st);
    vec4 cx = texture2D(plGeo, st + vec2(px.x, 0.0));
    vec4 cy = texture2D(plGeo, st + vec2(0.0, px.y));
    vec4 cxm = texture2D(plGeo, st - vec2(px.x, 0.0));
    vec4 cym = texture2D(plGeo, st - vec2(0.0, px.y));
    float depth = c.r;
    float id = floor(c.a);
    float shade = fract(c.a) * 2.0;
    bool ground = id > 199.5;
    bool empty = id < 0.5;

    float w = penWidth * (1.0 + 0.5 * g_ar * g_bass);
    // pressure: thickness wanders along the stroke
    w *= 1.0 + (vnoise2(fragCoord * 0.05 + 5.0) - 0.5) * 0.8 * pressure;

    // ── silhouettes / creases: depth or id discontinuity against any neighbour
    float dref = max(depth, 0.5);
    float eD = max(max(abs(cx.r - depth), abs(cxm.r - depth)), max(abs(cy.r - depth), abs(cym.r - depth)));
    float idj = 0.0;
    idj += (floor(cx.a) != id) ? 1.0 : 0.0; idj += (floor(cxm.a) != id) ? 1.0 : 0.0;
    idj += (floor(cy.a) != id) ? 1.0 : 0.0; idj += (floor(cym.a) != id) ? 1.0 : 0.0;
    float edge = smoothstep(0.012 * dref, 0.03 * dref, eD) + step(0.5, idj);
    edge = clamp(edge, 0.0, 1.0) * outlineAmt;
    // only draw the edge on the NEAR side (so the line hugs the object, not the background)
    float nearer = step(depth, min(min(cx.r, cxm.r), min(cy.r, cym.r)) + 0.02 * dref);
    edge *= mix(0.35, 1.0, nearer);
    // widen the outline to the pen width: a second ring of taps
    float eD2 = 0.0;
    for (int i = 0; i < 4; i++) {
        float a = float(i) * 1.5708 + 0.785;
        vec4 s = texture2D(plGeo, st + vec2(cos(a), sin(a)) * px * max(w - 1.0, 0.0));
        eD2 = max(eD2, (floor(s.a) != id) ? 1.0 : smoothstep(0.012 * dref, 0.03 * dref, abs(s.r - depth)));
    }
    edge = max(edge, eD2 * outlineAmt * 0.9 * mix(0.35, 1.0, nearer));

    // ── contour / grid lines on the surface (hidden lines are gone already)
    float gu = max(abs(cx.g - c.g), abs(cy.g - c.g));
    float gv = max(abs(cx.b - c.b), abs(cy.b - c.b));
    // discontinuities in u/v (face seams) must not paint lines
    float seamU = step(0.5 * abs(c.g) + 0.2, abs(cx.g - c.g) + abs(cy.g - c.g)) ;
    float spacing = (ground ? 1.0 : 0.5) / detail;
    float lu = isoLine(c.g, spacing, gu, w * 0.8);
    float lv = isoLine(c.b, spacing, gv, w * 0.8);
    float contour = max(lu, lv) * (1.0 - seamU);
    if (ground) contour *= floorOn * 0.8 * exp(-depth * 0.09);
    else contour *= contourAmt;
    if (empty) contour = 0.0;

    // ── hatching: parallel lines in surface space, three tones
    float hatch = 0.0;
    if (!empty && !ground && hatchAmt > 0.001) {
        float hs = 0.2 / detail;
        float gd = max(gu, gv) * 0.75 + 1e-6;
        float h1 = isoLine(c.g * 0.8 + c.b * 0.6, hs, gd, w * 0.7);
        float h2 = isoLine(c.g * 0.8 - c.b * 0.6, hs, gd, w * 0.7);
        float h3 = isoLine(c.b, hs * 0.5, gv * 0.5 + 1e-6, w * 0.6);
        float t1 = smoothstep(0.5, 0.42, shade);
        float t2 = smoothstep(0.3, 0.24, shade);
        float t3 = smoothstep(0.14, 0.09, shade);
        hatch = max(max(h1 * t1, h2 * t2), h3 * t3) * hatchAmt;
    }

    // ── pen sweep reveal
    float key = plotKey(id, c.b, depth);
    float band = 0.01;
    float reveal = 1.0 - smoothstep(g_prog - band, g_prog, key);
    float frontier = exp(-abs(key - g_prog) / (band * 4.0)) * step(g_prog, 0.999) * 3.0;
    float ink1 = edge * reveal;
    float ink2 = max(contour, hatch) * reveal;
    // the drawing is lifted off at the end of the sheet
    float keep = 1.0 - g_lift;
    return vec4(ink1 * keep, ink2 * keep, frontier * max(ink1 + ink2, 0.0) * keep, (empty ? 0.0 : 1.0));
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    g_ar = audioReact; g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_lev = levP();
    setupClock();
    setupCam();
    float az = radians(lightAz);
    g_key = normalize(vec3(sin(az) * 0.75, 0.65, cos(az) * 0.75));

    if (PASSINDEX == 0) {
        gl_FragColor = gbuffer(uv);
    } else if (PASSINDEX == 1) {
        gl_FragColor = inkPass(fragCoord);
    } else {
        vec2 px = 1.0 / R;
        vec2 st = fragCoord * px;
        vec4 k = texture2D(plInk, st);
        // ink bleed: soft halo around the strokes (ink soaking into the paper)
        vec2 bl = vec2(0.0); float fg = 0.0;
        for (int i = 0; i < 8; i++) {
            float a = float(i) * 0.785;
            vec4 s = texture2D(plInk, st + vec2(cos(a), sin(a)) * px * 1.5);
            bl += s.rg;
            fg += texture2D(plInk, st + vec2(cos(a + 0.4), sin(a + 0.4)) * px * 5.0).b;
        }
        bl /= 8.0; fg = fg / 8.0 + k.b;
        float ink1 = clamp(k.r + bl.x * 0.5 * bleed, 0.0, 1.0);
        float ink2 = clamp(k.g + bl.y * 0.5 * bleed, 0.0, 1.0);
        // paper
        vec3 paper = paperColor.rgb;
        float fib = vnoise2(fragCoord * 0.35) * 0.6 + vnoise2(fragCoord * 1.7) * 0.4;
        paper *= 1.0 + (fib - 0.5) * 0.25 * grain;
        bool dark = lum(paperColor.rgb) < 0.4;
        vec3 c1 = inkColor.rgb, c2 = inkColor2.rgb;
        float amp = inkAmt * (1.0 + 0.15 * g_ar * g_lev);
        vec3 col;
        if (dark) {
            // luminous ink on dark paper
            col = paper;
            col = mix(col, c2, clamp(ink2 * amp, 0.0, 1.0));
            col = mix(col, c1, clamp(ink1 * amp, 0.0, 1.0));
            col += (c1 * 0.6 + c2 * 0.4) * fg * penGlow * 0.9;
        } else {
            // real ink: multiply onto the paper
            col = paper;
            col *= mix(vec3(1.0), c2, clamp(ink2 * amp, 0.0, 1.0));
            col *= mix(vec3(1.0), c1, clamp(ink1 * amp, 0.0, 1.0));
            col = mix(col, col * 0.7 + vec3(0.3, 0.2, 0.1) * 0.0, k.b * penGlow * 0.5);
            col = mix(col, vec3(1.0, 0.55, 0.15), clamp(fg * penGlow * 0.35, 0.0, 1.0));
        }
        // pen head: a small glowing dot where the frontier is densest around this pixel (cheap: use the local frontier value)
        float g1 = (hash12(fragCoord + fract(g_t) * 61.0) - 0.5) * 0.04;
        col += g1 * grain;
        col *= 1.0 - vignette * smoothstep(0.5, 1.5, length(uv));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.35);
        float alphaOut = transparentBg ? clamp(max(ink1, ink2) * amp, 0.0, 1.0) : 1.0;
        gl_FragColor = vec4(col, alphaOut);
    }
}
