/*{
  "DESCRIPTION": "Biology — a living psychedelic specimen plate. Organisms drift across a field of hand-drawn magenta contour lines: beaded chains with lime rings, ringed targets on wavy stalks, black tadpoles with neon outlines, ferns with dotted fruiting tips, teal worms, black combs and dark lobes full of black-and-white tongues. The line field is the distance to every organism drawn as isolines, so each creature echoes outward in rings that crease into seams where they meet, while a web of hidden cells adds cross-stitched seams in the open space. Organisms swim, undulate, sway, pulse and are born and fade on their own clocks; the lines flow outward from them. Bass swells the bodies, beats pulse the rings and push a wave through the line field, mids stir the wiggle, highs flicker the bead centres.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "Nature", "Lines", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "glyphCount",  "LABEL": "Organisms",        "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 12.0, "DEFAULT": 10.0, "GROUP": "Organisms" },
    { "NAME": "glyphSize",   "LABEL": "Organism Size",    "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Organisms" },
    { "NAME": "lifeCycle",   "LABEL": "Life Cycle (s)",   "TYPE": "float", "AUDIO": false, "MIN": 6.0, "MAX": 90.0, "DEFAULT": 24.0, "GROUP": "Organisms" },
    { "NAME": "swim",        "LABEL": "Swim / Drift",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Motion" },
    { "NAME": "wiggle",      "LABEL": "Wiggle",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "lineFlow",    "LABEL": "Line Flow",        "TYPE": "float", "MIN": -2.0, "MAX": 2.0, "DEFAULT": 0.5, "GROUP": "Motion" },
    { "NAME": "lineDensity", "LABEL": "Line Density",     "TYPE": "float", "MIN": 30.0, "MAX": 200.0, "DEFAULT": 110.0, "GROUP": "Line Field" },
    { "NAME": "lineWidth",   "LABEL": "Line Weight",      "TYPE": "float", "MIN": 0.1, "MAX": 0.8, "DEFAULT": 0.34, "GROUP": "Line Field" },
    { "NAME": "halo",        "LABEL": "Magenta Halo",     "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.75, "GROUP": "Line Field" },
    { "NAME": "web",         "LABEL": "Web Seams",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Line Field" },
    { "NAME": "webScale",    "LABEL": "Web Scale",        "TYPE": "float", "MIN": 2.0, "MAX": 12.0, "DEFAULT": 5.0, "GROUP": "Line Field" },
    { "NAME": "cMagenta",    "LABEL": "Magenta",          "TYPE": "color", "DEFAULT": [0.86, 0.04, 0.46, 1.0], "GROUP": "Palette" },
    { "NAME": "cInk",        "LABEL": "Line Ink",         "TYPE": "color", "DEFAULT": [0.55, 0.0, 0.25, 1.0],  "GROUP": "Palette" },
    { "NAME": "cLime",       "LABEL": "Lime",             "TYPE": "color", "DEFAULT": [0.66, 1.0, 0.22, 1.0],  "GROUP": "Palette" },
    { "NAME": "cCyan",       "LABEL": "Cyan",             "TYPE": "color", "DEFAULT": [0.45, 0.9, 0.95, 1.0],  "GROUP": "Palette" },
    { "NAME": "cRed",        "LABEL": "Red",              "TYPE": "color", "DEFAULT": [1.0, 0.22, 0.12, 1.0],  "GROUP": "Palette" },
    { "NAME": "cYellow",     "LABEL": "Yellow",           "TYPE": "color", "DEFAULT": [1.0, 0.86, 0.35, 1.0],  "GROUP": "Palette" },
    { "NAME": "cBlack",      "LABEL": "Body Black",       "TYPE": "color", "DEFAULT": [0.03, 0.01, 0.03, 1.0], "GROUP": "Palette" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Palette" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Palette" },
    { "NAME": "zoom",        "LABEL": "Zoom",             "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "Paper / Top / Center", "TYPE": "color", "DEFAULT": [1.0, 0.88, 0.93, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "Paper Bottom / Edge", "TYPE": "color", "DEFAULT": [0.98, 0.76, 0.86, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Biology — single pass, 2D. Every organism is a signed distance shape.
// D = soft-min of all organism distances (+ a Voronoi seed field) is drawn
// as isolines: that is the magenta line field. Organisms are then painted
// over it with their own fills and neon outlines.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718
#define MAXG 12

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
vec3 hueRot(vec3 c, float h) {
    if (h <= 0.0) return c;
    float a = h * TAU; vec3 k = vec3(0.57735); float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }

float gPx;      // one screen pixel in glyph-local units (set per glyph)
float gT;
float gWig;

// distance to a wavy line y = A sin(k x + ph) for x in [x0, x1]
float wavy(vec2 q, float A, float k, float ph, float x0, float x1) {
    float x = clamp(q.x, x0, x1);
    float y = A * sin(k * x + ph);
    float dy = A * k * cos(k * x + ph);
    float d = abs(q.y - y) / sqrt(1.0 + dy * dy);
    return length(vec2(q.x - x, 0.0)) + d;
}
float waveY(float x, float A, float k, float ph) { return A * sin(k * x + ph); }

// paint helpers: fill + outline with AA
vec4 over(vec4 dst, vec3 c, float a) { return vec4(mix(dst.rgb, c, a), max(dst.a, a)); }
float fillA(float d) { return clamp(0.5 - d / gPx, 0.0, 1.0); }
float bandA(float d, float w) { return clamp(0.5 - (abs(d) - w) / gPx, 0.0, 1.0); }

// ── organisms (local units: roughly x in [-1.3, 1.3]) ────────────────────
// each returns rgba painted, and writes its signed distance to dOut

vec4 beadChain(vec2 q, float sd, out float dOut) {
    float A = 0.13 * gWig, k = 2.6, ph = gT * 1.6 + sd * 9.0;
    float dS = wavy(q, A, k, ph, -1.15, 1.15) - 0.055;
    float dB = 1e5; float bi = 0.0; vec2 bc = vec2(0.0);
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        float bx = -1.0 + fi * 0.4;
        vec2 c = vec2(bx, waveY(bx, A, k, ph));
        float r = 0.115 * (1.0 + 0.12 * sin(gT * 2.0 + fi) + 0.18 * bassP() * audioReact);
        float d = length(q - c) - r;
        if (d < dB) { dB = d; bi = fi; bc = q - c; }
    }
    float d = smin(dS, dB, 0.06);
    // the zig-zag outline of the reference: a crinkled offset contour
    float crink = 0.012 * sin(atan(q.y, q.x) * 40.0 + q.x * 60.0);
    dOut = d;
    vec4 c = vec4(0.0);
    c = over(c, cLime.rgb, bandA(d - 0.03 + crink, 0.012));
    c = over(c, cBlack.rgb, fillA(d));
    float rb = length(bc);
    float inB = fillA(dB + 0.012);
    vec3 bead = mix(cLime.rgb, cLime.rgb * 0.75, smoothstep(0.05, 0.1, rb));
    bead = mix(cBlack.rgb, bead, smoothstep(0.035, 0.045, rb));
    vec3 core = mod(bi, 3.0) < 0.5 ? vec3(0.45, 0.35, 0.9) : cLime.rgb * 0.6;
    bead = mix(core * (1.0 + 0.6 * highP() * audioReact), bead, smoothstep(0.02, 0.03, rb));
    c = over(c, bead, inB);
    return c;
}

vec4 target(vec2 q, float sd, out float dOut) {
    vec2 dc = vec2(0.75, 0.0);
    float R = 0.36 * (1.0 + 0.1 * audioBeatPulse * audioReact);
    float r = length(q - dc);
    float dD = r - R;
    float ph = gT * 2.2 + sd * 7.0;
    float dS = wavy(q, 0.09 * gWig, 7.0, ph, -1.3, 0.6) - 0.032;
    float d = smin(dD, dS, 0.04);
    dOut = d;
    vec4 c = vec4(0.0);
    c = over(c, cYellow.rgb, bandA(dS - 0.03, 0.011));
    c = over(c, cBlack.rgb, fillA(dS));
    // rings (pulse outward on beats)
    float rr = r / R + 0.08 * audioBeatPulse * audioReact;
    vec3 rc = cBlack.rgb;
    rc = mix(rc, vec3(0.95, 0.95, 0.85), step(0.16, rr) * step(rr, 0.3));
    rc = mix(rc, cLime.rgb, step(rr, 0.16));
    rc = mix(rc, cRed.rgb, step(0.3, rr) * step(rr, 0.44));
    rc = mix(rc, cBlack.rgb, step(0.44, rr) * step(rr, 0.56));
    rc = mix(rc, vec3(0.97, 0.9, 0.92), step(0.56, rr) * step(rr, 0.68));
    rc = mix(rc, cBlack.rgb, step(0.68, rr) * step(rr, 0.86));
    rc = mix(rc, cRed.rgb, step(0.86, rr) * step(rr, 0.92));
    c = over(c, vec3(0.97, 0.92, 0.9), bandA(dD - 0.025, 0.012));
    c = over(c, rc, fillA(dD));
    return c;
}

vec4 tadpole(vec2 q, float sd, out float dOut, vec3 rim) {
    vec2 hc = vec2(0.7, 0.0);
    float R = 0.27 * (1.0 + 0.15 * bassP() * audioReact);
    float dH = length(q - hc) - R;
    float ph = gT * 3.0 + sd * 5.0;
    float x = clamp(q.x, -1.25, 0.6);
    float taper = mix(0.018, 0.05, smoothstep(-1.25, 0.6, x));
    float dT = wavy(q, 0.12 * gWig * smoothstep(0.7, -0.6, q.x), 4.5, ph, -1.25, 0.6) - taper;
    float d = smin(dH, dT, 0.08);
    dOut = d;
    vec4 c = vec4(0.0);
    c = over(c, rim, bandA(d - 0.03, 0.012));
    c = over(c, cBlack.rgb, fillA(d));
    return c;
}

vec4 fern(vec2 q, float sd, out float dOut) {
    float sway = 0.15 * gWig * sin(gT * 0.9 + sd * 4.0);
    float dS = wavy(q, 0.12 + sway, 1.6, sd * 3.0, -1.2, 1.2) - 0.024;
    float d = dS;
    vec4 c = vec4(0.0);
    float dTip = 1e5; vec2 tipq = vec2(0.0); float tipi = 0.0;
    float dBr = 1e5;
    for (int i = 0; i < 12; i++) {
        float fi = float(i);
        float bx = -1.05 + fi * 0.19;
        float by = waveY(bx, 0.12 + sway, 1.6, sd * 3.0);
        float side = mod(fi, 2.0) < 0.5 ? 1.0 : -1.0;
        float ang = side * (0.75 + 0.3 * sin(fi * 1.7 + sd * 3.0)) + 0.18 * gWig * sin(gT * 1.4 + fi * 0.6);
        float L = (0.42 + 0.16 * sin(fi * 2.3 + sd)) * (1.0 - abs(bx) * 0.35);
        vec2 b = q - vec2(bx, by);
        b = rot(-ang + PI * 0.5) * b;          // branch along +x
        float bend = 0.18 * b.x * b.x * side;
        vec2 bq = vec2(b.x, b.y - bend);
        float dB = length(vec2(bq.x - clamp(bq.x, 0.0, L), bq.y)) - 0.012;
        dBr = min(dBr, dB);
        vec2 tp = bq - vec2(L, 0.0);
        float tr = 0.065 * (0.75 + 0.5 * h11(fi + sd * 13.0));
        float dt = length(tp) - tr;
        if (dt < dTip) { dTip = dt; tipq = tp; tipi = fi; }
    }
    d = min(min(d, dBr), dTip);
    dOut = d;
    c = over(c, cBlack.rgb, bandA(dBr - 0.004, 0.006));
    c = over(c, cRed.rgb * 0.95, fillA(dBr));
    c = over(c, cBlack.rgb, bandA(dS - 0.006, 0.008));
    c = over(c, mix(cRed.rgb, cYellow.rgb, 0.3), fillA(dS));
    // fruiting tips: ringed dots
    float tr = length(tipq);
    float k = mod(tipi, 3.0);
    vec3 ringC = k < 0.5 ? cCyan.rgb : (k < 1.5 ? cLime.rgb : cRed.rgb);
    vec3 tipC = mix(cBlack.rgb, ringC, smoothstep(0.012, 0.018, tr));
    tipC = mix(vec3(0.95, 0.9, 0.85) * (0.8 + 0.6 * highP() * audioReact), tipC, smoothstep(0.006, 0.01, tr));
    c = over(c, cBlack.rgb, bandA(dTip - 0.008, 0.008));
    c = over(c, tipC, fillA(dTip));
    return c;
}

vec4 worm(vec2 q, float sd, out float dOut) {
    float ph = gT * 2.5 + sd * 6.0;
    float d = wavy(q, 0.16 * gWig, 4.0, ph, -0.9, 0.9) - 0.06;
    dOut = d;
    vec4 c = vec4(0.0);
    c = over(c, cCyan.rgb, bandA(d - 0.022, 0.012));
    c = over(c, mix(cCyan.rgb, cBlack.rgb, 0.65), fillA(d));
    c = over(c, mix(cCyan.rgb, vec3(1.0), 0.3), fillA(abs(d + 0.035) - 0.008));
    return c;
}

vec4 comb(vec2 q, float sd, out float dOut) {
    float A = 0.14, k = 1.8, ph = sd * 4.0 + 0.4 * gWig * sin(gT * 0.8);
    float y = waveY(clamp(q.x, -1.2, 1.2), A, k, ph);
    float dS = wavy(q, A, k, ph, -1.2, 1.2) - 0.045;
    // teeth: perpendicular capsules repeated along x
    float sp = 0.13;
    float xc = floor(q.x / sp + 0.5);
    float ty = q.y - y;
    float dTooth = 1e5;
    for (int j = -1; j <= 1; j++) {
        float xi = clamp(xc + float(j), -9.0, 9.0);
        float tx = q.x - xi * sp;
        float tl = 0.2 + 0.05 * sin(xi * 1.3 + gT * 1.5 * gWig);
        dTooth = min(dTooth, length(vec2(tx, ty - clamp(ty, -tl, tl))) - 0.028);
    }
    float d = min(dS, dTooth);
    dOut = d;
    vec4 c = vec4(0.0);
    c = over(c, mix(cCyan.rgb, vec3(0.9), 0.3), bandA(d - 0.02, 0.01));
    c = over(c, cBlack.rgb, fillA(d));
    return c;
}

// dark lobe: its inside shows black-and-white tongue contours
vec4 lobe(vec2 q, float sd, out float dOut) {
    vec2 qq = q;
    qq.y += 0.18 * gWig * sin(q.x * 2.5 + gT * 1.2 + sd * 5.0);
    qq.x *= 0.62;
    float d = length(qq) - 0.42;
    d = smin(d, length(qq - vec2(0.35, 0.2)) - 0.22, 0.12);
    dOut = d;
    vec4 c = vec4(0.0);
    float inner = -d;
    float lines = smoothstep(0.35, 0.25, abs(fract(inner * 22.0 - gT * 0.8) - 0.5));
    vec3 tongue = mix(cBlack.rgb, vec3(0.97, 0.9, 0.95), lines * smoothstep(0.0, 0.03, inner));
    c = over(c, tongue, fillA(d));
    return c;
}

vec3 paper(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    vec2 p = (fc - 0.5 * R) / R.y / zoom;
    float aspect = R.x / R.y;
    gT = TIME * (0.6 + 0.4 * swim) + 0.3 * audioBassTime * ar;
    gWig = wiggle * (1.0 + 0.6 * midP() * ar);

    float D = 1e5;
    vec4 ink = vec4(0.0);
    float n = floor(clamp(glyphCount, 3.0, 12.0) + 0.5);

    for (int i = 0; i < MAXG; i++) {
        float fi = float(i);
        if (fi > n - 0.5) break;
        float sd = h11(fi * 3.17 + 0.5);
        // jittered grid home (4 × 3), wandering
        vec2 home = vec2((mod(fi, 4.0) + 0.5) / 4.0 - 0.5, (floor(fi / 4.0) + 0.5) / 3.0 - 0.5);
        home *= vec2(aspect, 1.0) / zoom;
        home += (h22(vec2(fi, 2.0)) - 0.5) * vec2(0.22 * aspect, 0.18);
        float st = TIME * swim * 0.08;
        vec2 c = home + 0.12 * vec2(sin(st * (1.0 + sd) + fi * 2.1), cos(st * (0.8 + sd) + fi * 1.3));
        // life: grow in, live, shrink out (each organism on its own clock)
        float lp = TIME / lifeCycle + sd;
        float lf = fract(lp);
        float life = smoothstep(0.0, 0.15, lf) * smoothstep(1.0, 0.85, lf);
        float typ = floor(h11(fi * 7.31 + floor(lp) * 3.7) * 7.0);
        float ang = h11(fi * 5.1 + floor(lp)) * TAU + 0.15 * sin(TIME * 0.2 * swim + fi);
        float sc = (0.21 + 0.08 * sd) * glyphSize * max(life, 0.001);
        vec2 q = rot(-ang) * (p - c) / sc;
        gPx = 1.0 / (R.y * zoom * sc);
        float bound = length(q) - 1.45;
        float d;
        vec4 g = vec4(0.0);
        if (bound > 0.9) {
            d = bound;                                 // far away: the bound is enough for the line field
        } else if (typ < 0.5) g = beadChain(q, sd, d);
        else if (typ < 1.5) g = target(q, sd, d);
        else if (typ < 2.5) g = tadpole(q, sd, d, cCyan.rgb);
        else if (typ < 3.5) g = fern(q, sd, d);
        else if (typ < 4.5) g = worm(q, sd, d);
        else if (typ < 5.5) g = comb(q, sd, d);
        else g = lobe(q, sd, d);
        g.a *= smoothstep(0.0, 0.25, life);
        // ease from the true shape distance to its round bound, so far contours stay seamless
        d = mix(d, bound, smoothstep(0.15, 0.9, bound));
        ink = over(ink, g.rgb, g.a);
        // contribute to the line field in screen units
        D = smin(D, d * sc + 0.012, 0.03);
    }

    // hidden web cells: rings around many seeds meet in cross-stitched seams
    vec2 wp = p * webScale + vec2(0.0, TIME * 0.02);
    vec2 wi = floor(wp), wf = fract(wp);
    float f1 = 9.0;
    for (int j = -1; j <= 1; j++)
    for (int k = -1; k <= 1; k++) {
        vec2 o = vec2(float(k), float(j));
        vec2 pt = o + 0.5 + 0.38 * sin(TIME * 0.15 + TAU * h22(wi + o));
        f1 = min(f1, length(pt - wf));
    }
    float Dw = f1 / webScale + 0.06;
    float Dall = mix(D, smin(D, Dw, 0.02), web);

    // isolines flowing outward from every organism
    float beatW = audioBeatPulse * ar;
    float ph = Dall * lineDensity - TIME * lineFlow * 2.0 - beatW * 1.5;
    float fw = lineDensity / (R.y * zoom);                 // phase change per pixel
    float lw = lineWidth * 0.5;
    float line = clamp((lw - abs(fract(ph) - 0.5)) / max(fw, 1e-4) + 0.5, 0.0, 1.0);
    // halo: lines and paper deepen toward the organisms
    float hal = exp(-max(D, 0.0) * 14.0) * halo;
    vec3 pap = paper(uv);
    vec3 ground = mix(pap, mix(pap, cMagenta.rgb, 0.85), clamp(hal, 0.0, 1.0));
    // lines are magenta on the open paper and deepen to ink inside the halos
    vec3 lineC = mix(cMagenta.rgb, cInk.rgb, clamp(hal * 1.1, 0.0, 1.0));
    vec3 col = mix(ground, lineC, line);
    // a faint magenta wash drifting through the open field
    col = mix(col, cMagenta.rgb, 0.06 * (0.5 + 0.5 * sin(p.x * 2.0 + p.y * 1.3 + TIME * 0.1)));

    col = mix(col, ink.rgb, ink.a);
    col *= 1.0 + 0.1 * audioLevel * ar;
    col = hueRot(col, hueShift);
    col = mix(vec3(lum(col)), col, saturation);
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.08;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    float alpha = transparentBg ? max(ink.a, line * 0.9) : 1.0;
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), alpha);
}
