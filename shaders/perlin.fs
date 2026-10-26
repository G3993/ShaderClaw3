/*{
  "DESCRIPTION": "Perlin — a living study of noise, texture and grain. Animated Perlin gradient noise is layered into fractal fields and folded back into itself (domain warping), so it boils, flows and curls like smoke in slow motion, then rendered through six textures that morph into each other: Smoke (soft warped clouds with deep shadows), Marble (turbulent veins cutting through polished stone), Topographic (the noise drawn as glowing contour lines, like a map of shifting terrain), Risograph (halftone dots sized by the noise, printed in two misregistered inks on paper), Paper Fibre (felted fibres and pulp flecks drifting through the light), and Film Stock (heavy silver-halide grain, scratches, dust, gate weave and flicker). Everything is finished with a tunable photographic grain — fine or coarse, mono or colour, frozen or crawling. Bass swells and darkens the fields, mids speed up the boiling, highs sharpen and sparkle the grain, beats push a ripple through the warp. At rest it drifts slowly with soft, sparse grain; during a build the noise tightens to finer detail, swirls faster and the grain gets hotter; on the drop the warp snaps outward in a bright shockwave and the grain flashes, then the peak boils at full contrast.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Texture", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "textureSel",  "LABEL": "Texture", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6], "LABELS": ["Auto Morph", "Smoke", "Marble", "Topographic", "Risograph", "Paper Fibre", "Film Stock"], "GROUP": "Noise" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 12.0, "GROUP": "Noise" },
    { "NAME": "noiseScale",  "LABEL": "Scale",            "TYPE": "float", "MIN": 0.4, "MAX": 4.0, "DEFAULT": 1.4, "GROUP": "Noise" },
    { "NAME": "octaves",     "LABEL": "Detail (octaves)", "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 7.0, "DEFAULT": 5.0, "GROUP": "Noise" },
    { "NAME": "warpAmt",     "LABEL": "Domain Warp",      "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.4, "GROUP": "Noise" },
    { "NAME": "flowSpeed",   "LABEL": "Boil Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Noise" },
    { "NAME": "drift",       "LABEL": "Drift",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "GROUP": "Noise" },
    { "NAME": "contrast",    "LABEL": "Contrast",         "TYPE": "float", "MIN": 0.5, "MAX": 2.5, "DEFAULT": 1.2, "GROUP": "Look" },
    { "NAME": "inkA",        "LABEL": "Ink / Shadow",     "TYPE": "color", "DEFAULT": [0.04, 0.03, 0.05, 1.0], "GROUP": "Look" },
    { "NAME": "inkB",        "LABEL": "Colour",           "TYPE": "color", "DEFAULT": [0.95, 0.22, 0.32, 1.0], "GROUP": "Look" },
    { "NAME": "inkC",        "LABEL": "Highlight / Paper","TYPE": "color", "DEFAULT": [0.96, 0.93, 0.86, 1.0], "GROUP": "Look" },
    { "NAME": "lineCount",   "LABEL": "Contour Lines",    "TYPE": "float", "MIN": 4.0, "MAX": 40.0, "DEFAULT": 16.0, "GROUP": "Look" },
    { "NAME": "dotSize",     "LABEL": "Halftone Size (px)","TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 20.0, "DEFAULT": 7.0, "GROUP": "Look" },
    { "NAME": "grainAmt",    "LABEL": "Grain",            "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.55, "GROUP": "Grain" },
    { "NAME": "grainSize",   "LABEL": "Grain Size",       "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 4.0, "DEFAULT": 1.3, "GROUP": "Grain" },
    { "NAME": "grainColour", "LABEL": "Colour Grain",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Grain" },
    { "NAME": "grainSpeed",  "LABEL": "Grain Crawl",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Grain" },
    { "NAME": "vignette",    "LABEL": "Vignette",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Grain" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.03, 0.04, 1.0], "GROUP": "Background" },
    { "NAME": "bgMix",       "LABEL": "Background Show-Through", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec3  h33(vec3 p)  { p = vec3(dot(p, vec3(127.1, 311.7, 74.7)), dot(p, vec3(269.5, 183.3, 246.1)), dot(p, vec3(113.5, 271.9, 124.6))); return fract(sin(p) * 43758.5453123) * 2.0 - 1.0; }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

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

// ── 3D Perlin gradient noise (quintic fade), range ≈ -1..1 ──────────────
float perlin(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    vec3 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    float n000 = dot(h33(i), f);
    float n100 = dot(h33(i + vec3(1, 0, 0)), f - vec3(1, 0, 0));
    float n010 = dot(h33(i + vec3(0, 1, 0)), f - vec3(0, 1, 0));
    float n110 = dot(h33(i + vec3(1, 1, 0)), f - vec3(1, 1, 0));
    float n001 = dot(h33(i + vec3(0, 0, 1)), f - vec3(0, 0, 1));
    float n101 = dot(h33(i + vec3(1, 0, 1)), f - vec3(1, 0, 1));
    float n011 = dot(h33(i + vec3(0, 1, 1)), f - vec3(0, 1, 1));
    float n111 = dot(h33(i + vec3(1, 1, 1)), f - vec3(1, 1, 1));
    return 1.6 * mix(mix(mix(n000, n100, u.x), mix(n010, n110, u.x), u.y),
                     mix(mix(n001, n101, u.x), mix(n011, n111, u.x), u.y), u.z);
}
// fractal sum; ridged = 1 gives turbulence |n|
float gOct, gOctCap, gWarpK;
float fbm(vec3 p, float ridged) {
    float s = 0.0, a = 0.5, norm = 0.0;
    for (int i = 0; i < 7; i++) {
        if (float(i) >= min(gOct, gOctCap)) break;
        float n = perlin(p);
        n = mix(n, 1.0 - 2.0 * abs(n), ridged);
        s += a * n; norm += a;
        p = vec3(rot(0.6) * p.xy, p.z) * 2.03 + vec3(1.7, -2.3, 0.9);
        a *= 0.5;
    }
    return s / max(norm, 1e-3);
}

float gT;
// the warped field: q (first warp) and r (second warp) give flowing structure
float field(vec2 p, float ridged, out vec2 warp) {
    float z = gT;
    vec2 q = vec2(fbm(vec3(p, z), 0.0), fbm(vec3(p + vec2(5.2, 1.3), z * 1.1), 0.0));
    float wk = warpAmt * gWarpK;
    vec2 r = vec2(fbm(vec3(p + 2.0 * q * wk + vec2(1.7, 9.2), z * 0.9), 0.0),
                  fbm(vec3(p + 2.0 * q * wk + vec2(8.3, 2.8), z * 1.2), 0.0));
    warp = r;
    return fbm(vec3(p + 2.0 * r * wk, z * 0.7), ridged);
}

vec3 ramp3(float t) {
    t = clamp(t, 0.0, 1.0);
    return t < 0.5 ? mix(inkA.rgb, inkB.rgb, smoothstep(0.0, 0.5, t)) : mix(inkB.rgb, inkC.rgb, smoothstep(0.5, 1.0, t));
}

// one texture at point p (noise space) and pixel fc.
// The warped field is evaluated from ONE call site (in a loop) to keep the shader small.
vec3 textureLook(float k, vec2 p, vec2 fc, float px) {
    // per-texture sampling setup
    float scl = k < 0.5 ? 1.0 : k < 1.5 ? 0.45 : k < 2.5 ? 0.38 : k < 3.5 ? 1.0 : k < 4.5 ? 0.6 : 0.9;
    float ridged = 0.0;
    gWarpK  = k < 0.5 ? 1.0 : k < 1.5 ? 0.45 : k < 2.5 ? 0.35 : k < 3.5 ? 0.8 : k < 4.5 ? 0.6 : 0.9;
    gOctCap = k < 0.5 ? 7.0 : k < 1.5 ? 4.0 : k < 2.5 ? 3.0 : k < 3.5 ? 4.0 : 7.0;
    bool riso = k > 2.5 && k < 3.5;
    bool topo = k > 1.5 && k < 2.5;
    float cell = max(dotSize, 2.0);
    vec2 P0 = p * scl, P1 = P0;
    vec2 gA0 = vec2(0.0), gA1 = vec2(0.0);
    if (topo) P1 = P0 + vec2(1.0, 1.0) * px * scl * 1.5;          // a neighbour sample for the field's slope
    if (riso) {
        for (int i = 0; i < 2; i++) {
            float fi = float(i);
            vec2 off = fi < 0.5 ? vec2(0.0) : vec2(2.5, -1.5) + 1.5 * vec2(sin(gT * 7.0), cos(gT * 5.0));   // misregistration
            float ang = fi < 0.5 ? 0.26 : 1.31;
            vec2 g = rot(ang) * (fc + off) / cell;
            vec2 id = floor(g) + 0.5;
            vec2 cpix = rot(-ang) * id * cell - off;
            vec2 cp = (cpix - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y * noiseScale + (p - (fc - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y * noiseScale);
            if (i == 0) { P0 = cp; gA0 = g; } else { P1 = cp + 0.37; gA1 = g; }
        }
    }
    float F0 = 0.0, F1 = 0.0;
    vec2 w = vec2(0.0), wtmp;
    for (int j = 0; j < 2; j++) {
        if (j == 1 && !riso && !topo) break;
        float fv = field(j == 0 ? P0 : P1, ridged, wtmp);
        if (j == 0) { F0 = fv; w = wtmp; } else F1 = fv;
    }
    float f = F0;
    if (k < 0.5) {                                                  // Smoke
        float v = smoothstep(0.05, 0.95, 0.5 + 0.5 * f * contrast);
        vec3 c = ramp3(v * 0.85 + 0.1 * length(w));
        return c * (0.75 + 0.45 * smoothstep(0.2, 0.9, v));
    }
    if (k < 1.5) {                                                  // Marble
        float veins = sin((p.x + p.y * 0.4) * 3.5 + f * 14.0 * contrast + w.x * 5.0);
        float v = pow(1.0 - abs(veins), 7.0);
        float v2 = pow(1.0 - abs(sin((p.y - p.x * 0.3) * 2.5 + f * 18.0 + w.y * 4.0)), 26.0);
        vec3 stone = mix(inkC.rgb * 0.82, inkC.rgb, smoothstep(-0.5, 0.6, f));
        stone *= 0.94 + 0.06 * h12(floor(p * 300.0));                // fine crystal sparkle
        vec3 c = mix(stone, inkB.rgb, v * 0.85);
        return mix(c, inkA.rgb, v2 * 0.9);
    }
    if (k < 2.5) {                                                  // Topographic
        float gainH = 1.8 * contrast * 0.8;
        float h = f * gainH + 0.5;                                  // stretch the field so many contours appear (unclamped: no flat plateaus)
        float n = floor(lineCount);
        float lv = h * n;
        float slope = max(abs(F1 - F0) * gainH * n / 2.12, 1e-4);   // contour units per pixel
        float dpx = (0.5 - abs(fract(lv) - 0.5)) / slope;           // distance to the nearest contour, in pixels
        float lw = 0.7 + 0.6 * gLift;
        float line = smoothstep(lw + 0.8, lw - 0.2, dpx);
        float major = mod(floor(lv + 0.5), 5.0) < 0.5 ? 1.0 : 0.0;
        vec3 base = inkA.rgb + 0.06 * clamp(h, 0.0, 1.0);
        return mix(base, mix(inkB.rgb, inkC.rgb, major), line * (0.55 + 0.45 * major));
    }
    if (riso) {                                                     // Risograph halftone, two inks multiplied
        vec3 c = inkC.rgb;
        for (int i = 0; i < 2; i++) {
            float fi = float(i);
            float fv = i == 0 ? F0 : F1;
            vec2 g = i == 0 ? gA0 : gA1;
            float tone = clamp(0.5 + 0.55 * fv * contrast + (fi - 0.5) * 0.15, 0.0, 1.0);
            float r = sqrt(tone) * 0.62;
            float dd = length(fract(g) - 0.5);
            float dotm = smoothstep(r + 0.06, r - 0.06, dd);
            vec3 ink = fi < 0.5 ? inkB.rgb : inkA.rgb + vec3(0.05, 0.1, 0.25);
            c = mix(c, c * ink * 1.05, dotm * 0.92);
        }
        return c;
    }
    if (k < 4.5) {                                                  // Paper fibre
        vec3 c = mix(inkC.rgb * 0.92, inkC.rgb, 0.5 + 0.5 * f);
        float fib = 0.0;
        for (int i = 0; i < 3; i++) {
            float fi = float(i);
            vec2 q = p * (6.0 + fi * 5.0) + w * 3.0 + fi * 13.1;
            vec2 id = floor(q), fq = fract(q) - 0.5;
            float a = h12(id) * TAU + w.x * 2.0;
            vec2 dir = vec2(cos(a), sin(a));
            float along = dot(fq, dir), across = dot(fq, vec2(-dir.y, dir.x));
            float sres = smoothstep(0.05, 0.0, abs(across + 0.15 * along * along)) * smoothstep(0.5, 0.2, abs(along)) * step(0.55, h12(id + 7.0));
            fib = max(fib, sres * (0.5 + 0.5 * h12(id + 3.0)));
        }
        c = mix(c, mix(inkB.rgb, inkA.rgb, 0.3), fib * 0.55);
        vec2 fp = p * 40.0 + w * 6.0;
        vec2 fid = floor(fp);
        float fl = step(0.97, h12(fid)) * smoothstep(0.3, 0.12, length(fract(fp) - 0.5 - (vec2(h12(fid + 1.0), h12(fid + 2.0)) - 0.5) * 0.4));
        c = mix(c, inkA.rgb * 1.2, fl * 0.55);
        return c * (0.92 + 0.12 * smoothstep(-0.4, 0.6, f));
    }
    // Film stock: silver-halide clumps, scratches, dust, flicker
    float v = smoothstep(-0.6, 0.7, f * contrast);
    vec3 c = mix(inkA.rgb, inkC.rgb, v);
    float fr = floor(TIME * 24.0);
    float clump = h12(floor(fc / (2.0 * grainSize)) + fr * 3.1);
    c *= 0.86 + 0.28 * clump;
    float sx = h11(fr * 1.7);
    float scr = smoothstep(1.5, 0.0, abs(fc.x - sx * RENDERSIZE.x)) * step(0.55, h11(fr * 2.3));
    c = mix(c, inkC.rgb, scr * 0.6);
    float dust = step(0.9993, h12(floor(fc / 3.0) + fr));
    c = mix(c, inkA.rgb, dust);
    return c * (0.92 + 0.08 * h11(fr));
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    gT = TIME * 0.12 * flowSpeed + (audioMidTime * 0.12 + audioBassTime * 0.06) * ar * flowSpeed + 0.6 * gBuild * gBuild;
    gOct = clamp(floor(octaves + 0.6 * gBuild + 0.8 * gPeak - 1.2 * gRest + 0.5), 1.0, 7.0);

    vec2 sp = (fc - 0.5 * R) / R.y;
    // film-gate weave + build tremor
    sp += vec2(sin(TIME * 7.3), cos(TIME * 5.9)) * 0.0015 * (0.3 + gBuild);
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.006 * smoothstep(0.7, 1.0, gBuild);
    // scale: finer detail as the build climbs, a bass swell, drop shockwave
    float sc = noiseScale * (1.0 + 0.45 * gBuild - 0.15 * gRest) * (1.0 - 0.06 * bassP() * ar);
    float rr = length(sp);
    float shock = exp(-pow((rr - (1.0 - gDrop) * 1.2) * 9.0, 2.0)) * gDrop;
    vec2 p = sp * sc + vec2(TIME * 0.03, -TIME * 0.02) * drift * 4.0;
    p += normalize(sp + 1e-4) * shock * 0.25 + sp * 0.12 * audioBeatPulse * ar * exp(-rr * 2.0);
    float px = sc / R.y;

    // texture morph
    float ph = TIME / morphPeriod;
    float ts = float(textureSel);
    float ka = ts > 0.5 ? ts - 1.0 : mod(floor(ph), 6.0);
    float kb = ts > 0.5 ? ka : mod(floor(ph) + 1.0, 6.0);
    float mf = ts > 0.5 ? 0.0 : smoothstep(0.75, 1.0, fract(ph));
    // grainy dissolve: the next texture eats through in noise-shaped patches (one textureLook call site)
    float nd = perlin(vec3(sp * 3.0, TIME * 0.3)) * 0.5 + 0.5;
    float m = mf > 0.001 ? smoothstep(nd - 0.12, nd + 0.12, mf * 1.24 - 0.12) : 0.0;
    vec3 col = vec3(0.0);
    for (int i = 0; i < 2; i++) {
        float wgt = i == 0 ? 1.0 - m : m;
        if (wgt < 0.001) continue;
        col += textureLook(i == 0 ? ka : kb, p, fc, px) * wgt;
    }

    // shape & energy
    col = mix(col, backdrop(uv), bgMix * (1.0 - lum(col)));
    col *= 0.78 + 0.32 * gLift + 0.1 * audioBeatPulse * ar;
    col += inkC.rgb * shock * 0.6;
    col = mix(col, vec3(lum(col)), 0.35 * gRest);
    col *= 1.0 - vignette * smoothstep(0.45, 1.25, length(sp * vec2(0.85, 1.0)));

    // photographic grain (resolution-independent size, crawling per frame)
    float gs = max(grainSize, 1.0);
    float fr = floor(TIME * 24.0 * grainSpeed);
    vec2 gp = fc / gs;
    float g1 = perlin(vec3(gp * 0.9, fr * 1.7)) ;
    float g2 = h12(floor(gp) + fr * 13.0) * 2.0 - 1.0;
    float gm = mix(g2, g1, 0.45);
    vec3 gc = vec3(gm);
    if (grainColour > 0.0) {
        vec3 gcol = vec3(h12(floor(gp) + fr * 7.0 + 1.3), h12(floor(gp) + fr * 7.0 + 5.1), h12(floor(gp) + fr * 7.0 + 9.7)) * 2.0 - 1.0;
        gc = mix(gc, gcol, grainColour);
    }
    float gA = grainAmt * (0.6 + 0.5 * gLift + 0.6 * gBuild + 0.8 * gDrop + 0.4 * highP() * ar) * (0.35 + 0.65 * (1.0 - abs(lum(col) - 0.5) * 1.4));
    col += gc * 0.09 * gA;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(lum(col) * 1.4, 0.0, 1.0) : 1.0);
}
