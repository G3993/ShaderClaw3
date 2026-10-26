/*{
  "DESCRIPTION": "Biology — a living psychedelic specimen plate. Seventeen kinds of abstract organism drift through oil-slick acid pools of soap-film iridescence and bubble foam, across a field of hand-drawn magenta contour lines: beaded chains, ringed targets on wavy stalks, tadpoles, ferns with dotted fruiting tips, worms, combs, dark lobes full of black-and-white tongues, ammonite spirals, spiny urchins, amoebas with nuclei, radiolarian lattices, jellyfish, dividing cells, branching coral, spore clusters, eyes and striped ribbons — each with its own accent colours, proportions and counts. The line field is the distance to every organism drawn as isolines, so each creature echoes outward in rings that crease into seams where they meet, while a web of hidden cells adds cross-stitched seams in the open space. Organisms swim, undulate, sway, pulse and are born and fade on their own clocks; the lines flow outward from them. Bass swells the bodies, beats pulse the rings and push a wave through the line field, mids stir the wiggle, highs flicker the bead centres. Rest: a few small organisms drift slowly in sparse, soft contour lines with shallow acid pools; Build: the organisms coil inward and shrink while the lines tighten and race outward, the oil-slick spreads and shimmers faster, with a tremor at the top; Drop: a rainbow shockwave rips out through the line field and film as every organism punches up and scatters into a dense, saturated, full-acid plate.",
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
    { "NAME": "cViolet",     "LABEL": "Violet",           "TYPE": "color", "DEFAULT": [0.58, 0.42, 1.0, 1.0],  "GROUP": "Palette" },
    { "NAME": "cBlack",      "LABEL": "Body Black",       "TYPE": "color", "DEFAULT": [0.03, 0.01, 0.03, 1.0], "GROUP": "Palette" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Palette" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Palette" },
    { "NAME": "acid",        "LABEL": "Acid (oil slick)", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Acid" },
    { "NAME": "acidPools",   "LABEL": "Pool Coverage",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Acid" },
    { "NAME": "acidScale",   "LABEL": "Pool Scale",       "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 0.9, "GROUP": "Acid" },
    { "NAME": "foam",        "LABEL": "Bubble Foam",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Acid" },
    { "NAME": "foamSize",    "LABEL": "Bubble Size",      "TYPE": "float", "MIN": 10.0, "MAX": 80.0, "DEFAULT": 34.0, "GROUP": "Acid" },
    { "NAME": "filmSpeed",   "LABEL": "Film Swirl",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Acid" },
    { "NAME": "bodySheen",   "LABEL": "Iridescent Bodies","TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Acid" },
    { "NAME": "zoom",        "LABEL": "Zoom",             "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 3, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
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

// ── Studio Palette: remap to black → accent → white (red or blue) ─────────
vec3 studioPalette(vec3 c) {
    float m = float(paletteMode);
    if (palAmount <= 0.0) return c;
    float L = clamp(dot(c, vec3(0.2126, 0.7152, 0.0722)), 0.0, 1.0);
    float mx = max(c.r, max(c.g, c.b)), mn = min(c.r, min(c.g, c.b));
    float sat = mx > 1e-4 ? (mx - mn) / mx : 0.0;
    vec3 o;
    if (m < 0.5) {
        // Colorful: saturation pushed hard toward neon / primary / poppy colour.
        vec3 hueC = pow((c - mn) / max(mx - mn, 1e-4), vec3(1.4));   // pure hue, nudged toward primaries
        float s2 = 1.0 - pow(1.0 - sat, 4.0);                         // strong but curved saturation boost
        float v = clamp(pow(mx, 0.85) * 1.12, 0.0, 1.0);
        o = mix(vec3(1.0), hueC, s2) * v;
        // flat mid-greys pick up a drifting neon spectrum keyed to brightness
        float hx = L * 1.7 + 0.04 * TIME;
        vec3 neon = pow(clamp(abs(fract(hx + vec3(0.0, 0.6667, 0.3333)) * 6.0 - 3.0) - 1.0, 0.0, 1.0), vec3(1.4));
        float wn = (1.0 - smoothstep(0.02, 0.1, sat)) * smoothstep(0.06, 0.25, L) * (1.0 - smoothstep(0.55, 0.85, L));
        o = mix(o, neon * v, wn);
        o = mix(o, vec3(1.0), smoothstep(0.85, 1.0, L) * 0.5);       // white-hot cores
    } else if (m > 2.5) {
        o = vec3(smoothstep(0.02, 0.95, pow(L, 0.9)));             // Black & White
    } else {
        vec3 acc = m < 1.5 ? palRed.rgb : palBlue.rgb;
        float t = pow(L, 0.8);
        vec3 ramp = t < 0.5 ? mix(vec3(0.0), acc, t * 2.0) : mix(acc, vec3(1.0), (t - 0.5) * 2.0);
        vec3 mono = vec3(smoothstep(0.0, 1.0, L));
        o = mix(mono, ramp, smoothstep(0.06, 0.4, sat));
    }
    return mix(c, o, palAmount);
}
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

// ── Show dynamics: rest → build → peak / drop ───────────────────────────
float gRest, gBuild, gPeak, gDrop, gLift;
void showDynamics() {
    float m = float(showState);
    float e = clamp(audioEnergy, 0.0, 1.0);
    float b = clamp(audioBuildup, 0.0, 1.0);
    float d = clamp(audioDrop, 0.0, 1.0);
    if (m > 0.5) {                                   // manual override for the VJ
        if (m < 1.5)      { e = 0.04; b = 0.0; d = 0.0; }                                      // Rest
        else if (m < 2.5) { float r = fract(TIME / 8.0); e = 0.3 + 0.35 * r; b = r * r; d = 0.0; } // Build: 8 s riser, loops
        else              { e = 1.0; b = 0.0; d = exp(-fract(TIME / 4.0) * 5.0); } // Peak: hits every 4 s + beats
    }
    float k = dynamics;
    gBuild = b * k;                                                  // tension 0..1
    gDrop  = d * k;                                                  // impact impulse 0..1
    gPeak  = smoothstep(0.5, 0.9, e) * k;                            // sustained high section
    gRest  = (1.0 - smoothstep(0.08, 0.4, e)) * (1.0 - b) * k;       // quiet / breakdown
    gLift  = mix(0.5, clamp(0.15 + e * 0.7 + gBuild * 0.25 + d * 0.3, 0.0, 1.0), k);  // 0 rest … 1 peak; 0.5 when dynamics = 0
}

float gPx;      // one screen pixel in glyph-local units (set per glyph)
float gT;
float gWig;
vec3 gAcc, gAcc2;   // per-organism accent colours

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

// ── more abstract organisms ─────────────────────────────────────────────
float segD(vec2 p, vec2 a, vec2 b) { vec2 pa = p - a, ba = b - a; float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0); return length(pa - ba * h); }

vec4 spiralShell(vec2 q, float sd, out float dOut) {
    q = rot(gT * 0.25 * (sd > 0.5 ? 1.0 : -1.0)) * q;
    float r = length(q), a = atan(q.y, q.x) + PI;
    float c = 0.045 + 0.02 * sd;
    float n0 = floor((r / c - a) / TAU + 0.5);
    float nMax = floor(1.05 / (c * TAU));
    float dr = 1e5;
    for (int j = -1; j <= 1; j++) {
        float n = clamp(n0 + float(j), 0.0, nMax);
        dr = min(dr, abs(r - c * (a + TAU * n)));
    }
    float d = max(dr - c * 1.1 * (0.6 + 0.4 * r), r - 1.1);
    float chamber = smoothstep(0.3, 0.1, abs(fract(a * 3.0 / PI + r * 4.0) - 0.5));
    dOut = d;
    vec4 o = vec4(0.0);
    o = over(o, gAcc, bandA(d - 0.025, 0.012));
    vec3 fill = mix(cBlack.rgb, gAcc2, 0.25 + 0.5 * chamber * smoothstep(0.0, 0.03, -d));
    o = over(o, fill, fillA(d));
    return o;
}

vec4 urchin(vec2 q, float sd, out float dOut) {
    float N = 7.0 + floor(sd * 9.0);
    float a = atan(q.y, q.x) + gT * 0.15;
    float seg = TAU / N;
    float k = floor(a / seg + 0.5);
    float aa = a - k * seg;
    float r = length(q);
    vec2 lq = vec2(r * cos(aa), r * sin(aa));
    float L = 0.95 + 0.15 * sin(k * 2.3 + gT * 1.2 * gWig);
    lq.y -= 0.06 * gWig * sin(lq.x * 6.0 - gT * 3.0 + k);
    float dSp = segD(lq, vec2(0.0), vec2(L, 0.0)) - 0.03;
    float dTip = length(lq - vec2(L, 0.0)) - 0.075;
    float dC = r - 0.3 * (1.0 + 0.12 * bassP() * audioReact);
    float d = min(min(dSp, dTip), dC);
    dOut = d;
    vec4 o = vec4(0.0);
    o = over(o, gAcc, bandA(min(dSp, dC) - 0.02, 0.01));
    o = over(o, cBlack.rgb, fillA(min(dSp, dC)));
    vec3 cc = mix(gAcc2, cBlack.rgb, smoothstep(0.08, 0.14, r));
    o = over(o, cc, fillA(dC + 0.05));
    o = over(o, cBlack.rgb, bandA(dTip, 0.012));
    o = over(o, gAcc2, fillA(dTip));
    return o;
}

vec4 amoeba(vec2 q, float sd, out float dOut) {
    float a = atan(q.y, q.x);
    float R = 0.75 + 0.12 * sin(a * 3.0 + gT * 1.1 + sd * 6.0) * gWig + 0.08 * sin(a * 5.0 - gT * 1.7) * gWig + 0.06 * sin(a * 2.0 + gT * 0.6);
    float d = (length(q) - R) * 0.8;
    vec2 nc = 0.25 * vec2(sin(gT * 0.5 + sd * 4.0), cos(gT * 0.4 + sd * 2.0));
    float dN = length(q - nc) - 0.2;
    dOut = d;
    vec4 o = vec4(0.0);
    o = over(o, cBlack.rgb, bandA(d - 0.03, 0.03));
    // cytoplasm: inner contour lines echoing the membrane
    float inner = -d;
    float lines = smoothstep(0.3, 0.15, abs(fract(inner * 18.0 + gT * 0.4) - 0.5));
    vec3 cyto = mix(mix(vec3(0.98, 0.93, 0.96), gAcc, 0.35), gAcc * 0.75, lines);
    o = over(o, cyto, fillA(d));
    // vacuoles
    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        vec2 vc = 0.45 * vec2(cos(fi * 1.9 + sd * 7.0 + gT * 0.2), sin(fi * 2.7 + sd * 3.0 + gT * 0.25));
        float dv = length(q - vc) - (0.06 + 0.04 * h11(fi + sd * 9.0));
        o = over(o, cBlack.rgb, bandA(dv, 0.01));
        o = over(o, gAcc2, fillA(dv));
    }
    o = over(o, gAcc2, bandA(dN - 0.03, 0.015));
    o = over(o, cBlack.rgb, fillA(dN));
    o = over(o, gAcc, fillA(length(q - nc) - 0.06));
    return o;
}

vec4 radiolarian(vec2 q, float sd, out float dOut) {
    q = rot(gT * 0.1) * q;
    float r = length(q), a = atan(q.y, q.x);
    float R = 0.8 * (1.0 + 0.06 * audioBeatPulse * audioReact);
    float dOuter = r - R;
    // spikes
    float N = 10.0 + floor(sd * 10.0);
    float seg = TAU / N;
    float aa = a - floor(a / seg + 0.5) * seg;
    vec2 lq = vec2(r * cos(aa), r * sin(aa));
    float dSpk = segD(lq, vec2(R, 0.0), vec2(R + 0.32, 0.0)) - 0.02 * (1.0 - smoothstep(R, R + 0.32, lq.x));
    float d = min(dOuter, dSpk);
    dOut = d;
    // lattice holes on rings
    float ring = floor(r / 0.16);
    float rn = max(ring, 1.0) * 6.0;
    float sa = TAU / rn;
    float ka = floor((a + ring) / sa + 0.5) * sa - ring;
    vec2 hc = (ring + 0.5) * 0.16 * vec2(cos(ka), sin(ka));
    float dHole = length(q - hc) - 0.05;
    vec4 o = vec4(0.0);
    o = over(o, cBlack.rgb, fillA(dSpk));
    o = over(o, gAcc, bandA(dOuter - 0.02, 0.014));
    vec3 body = mix(cBlack.rgb, gAcc2, 0.85 * fillA(dHole) * step(r, R - 0.05));
    body = mix(body, gAcc, 0.9 * fillA(abs(dHole) - 0.008) * step(r, R - 0.05));
    o = over(o, body, fillA(dOuter));
    return o;
}

vec4 jelly(vec2 q, float sd, out float dOut) {
    q.y -= 0.3;
    float pulse = 1.0 + 0.1 * sin(gT * 2.0 + sd * 6.0) + 0.1 * audioBeatPulse * audioReact;
    vec2 dq = q / vec2(0.75 * pulse, 0.55 / pulse);
    float dDome = (length(dq) - 1.0) * 0.55;
    float rim = -q.y + 0.04 * sin(q.x * 18.0);
    dDome = max(dDome, rim);
    float dT = 1e5;
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        float x0 = -0.5 + fi * 0.25;
        vec2 tq = vec2(q.x - x0 - 0.12 * gWig * sin(q.y * 5.0 - gT * 2.5 + fi), q.y);
        dT = min(dT, length(vec2(tq.x, tq.y - clamp(tq.y, -1.35 + 0.15 * h11(fi + sd * 5.0), 0.0))) - 0.022);
    }
    float d = min(dDome, dT);
    dOut = d;
    vec4 o = vec4(0.0);
    o = over(o, gAcc, bandA(dT - 0.02, 0.01));
    o = over(o, cBlack.rgb, fillA(dT));
    float bands = smoothstep(0.35, 0.2, abs(fract(length(dq) * 5.0 - gT * 0.5) - 0.5));
    vec3 dome = mix(gAcc2, mix(gAcc, vec3(1.0), 0.4), bands);
    o = over(o, cBlack.rgb, bandA(dDome - 0.02, 0.018));
    o = over(o, dome, fillA(dDome));
    return o;
}

vec4 dividing(vec2 q, float sd, out float dOut) {
    float split = 0.18 + 0.2 * (0.5 + 0.5 * sin(gT * 0.6 + sd * 6.0));
    float d1 = length(q - vec2(-split, 0.0)) - 0.5;
    float d2 = length(q - vec2(split, 0.0)) - 0.5;
    float d = smin(d1, d2, 0.3 - split * 0.5);
    dOut = d;
    vec4 o = vec4(0.0);
    o = over(o, gAcc, bandA(d - 0.035, 0.018));
    float inner = -d;
    float lines = smoothstep(0.3, 0.12, abs(fract(inner * 14.0) - 0.5));
    o = over(o, mix(cBlack.rgb, gAcc * 0.6, lines * 0.6), fillA(d));
    // chromosomes: little X marks gathering at each pole
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        float side = fi < 2.5 ? -1.0 : 1.0;
        vec2 c = vec2(side * split * 1.2, 0.0) + 0.2 * vec2(cos(fi * 2.1 + sd * 5.0), sin(fi * 2.9 + gT * 0.3));
        vec2 xq = rot(fi + gT * 0.4) * (q - c);
        float dx = min(segD(xq, vec2(-0.07, -0.07), vec2(0.07, 0.07)), segD(xq, vec2(-0.07, 0.07), vec2(0.07, -0.07))) - 0.018;
        o = over(o, gAcc2, fillA(dx));
    }
    return o;
}

vec4 coral(vec2 q, float sd, out float dOut) {
    vec2 p = q - vec2(0.0, -1.15);
    float L = 0.55, w = 0.07, dd = 1e5;
    float spread = 0.42 + 0.15 * sd;
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        dd = min(dd, segD(p, vec2(0.0), vec2(0.0, L)) - w);
        p.y -= L;
        p.x = abs(p.x);
        p = rot(spread + 0.08 * gWig * sin(gT * 1.1 + fi * 1.7)) * p;
        L *= 0.74; w *= 0.72;
    }
    float dTip = length(p) - 0.05;
    float d = min(dd, dTip);
    dOut = d;
    vec4 o = vec4(0.0);
    o = over(o, gAcc, bandA(dd - 0.02, 0.012));
    o = over(o, cBlack.rgb, fillA(dd));
    o = over(o, gAcc2, fillA(dTip));
    return o;
}

vec4 spores(vec2 q, float sd, out float dOut) {
    float d = 1e5;
    vec4 o = vec4(0.0);
    for (int i = 0; i < 9; i++) {
        float fi = float(i);
        vec2 c = (vec2(h11(fi * 3.1 + sd * 17.0), h11(fi * 5.7 + sd * 11.0)) - 0.5) * 1.7;
        c += 0.05 * vec2(sin(gT + fi), cos(gT * 1.3 + fi));
        float r = 0.08 + 0.17 * h11(fi * 9.3 + sd * 3.0);
        float di = length(q - c) - r;
        d = min(d, di);
        float rr = length(q - c) / r;
        vec3 sc = mix(cBlack.rgb, mix(vec3(0.97, 0.93, 0.95), gAcc, 0.3), step(0.45, rr) * step(rr, 0.7));
        sc = mix(sc, gAcc2, step(rr, 0.25));
        o = over(o, gAcc, bandA(di - 0.015, 0.01));
        o = over(o, sc, fillA(di));
    }
    dOut = d;
    return o;
}

vec4 eye(vec2 q, float sd, out float dOut) {
    float blink = 1.0 - 0.92 * pow(max(0.0, sin(gT * 0.7 + sd * 9.0)), 30.0);
    vec2 eq = vec2(q.x, q.y / max(blink, 0.08));
    float d = max(length(eq - vec2(0.0, -0.7)) - 1.05, length(eq - vec2(0.0, 0.7)) - 1.05) * max(blink, 0.08);
    dOut = d;
    vec4 o = vec4(0.0);
    o = over(o, cBlack.rgb, bandA(d - 0.03, 0.02));
    vec2 look = 0.15 * vec2(sin(gT * 0.5 + sd * 4.0), 0.4 * cos(gT * 0.37));
    float ri = length(q - look);
    vec3 iris = mix(gAcc, gAcc2, smoothstep(0.1, 0.32, ri));
    iris = mix(iris, cBlack.rgb, smoothstep(0.3, 0.33, ri));
    iris = mix(cBlack.rgb, iris, smoothstep(0.08, 0.1, ri));
    iris = mix(iris, vec3(1.0), smoothstep(0.035, 0.0, length(q - look - vec2(0.06, 0.06))));
    vec3 white = mix(vec3(0.98, 0.94, 0.96), iris, step(ri, 0.36));
    o = over(o, white, fillA(d));
    return o;
}

vec4 ribbon(vec2 q, float sd, out float dOut) {
    float A = 0.3 * gWig, k = 2.2, ph = gT * 1.2 + sd * 6.0;
    float x = clamp(q.x, -1.2, 1.2);
    float w = 0.16 * (1.0 - 0.6 * abs(x) / 1.2);
    float y = A * sin(k * x + ph);
    float dy = A * k * cos(k * x + ph);
    float off = (q.y - y) / sqrt(1.0 + dy * dy);
    float d = length(vec2(q.x - x, 0.0)) + abs(off) - w;
    dOut = d;
    // stripes running along the ribbon, like the black-and-white tongues
    float st = smoothstep(0.3, 0.2, abs(fract(off / max(w, 0.01) * 2.5 + 0.5) - 0.5));
    vec4 o = vec4(0.0);
    o = over(o, gAcc, bandA(d - 0.02, 0.01));
    o = over(o, mix(cBlack.rgb, vec3(0.97, 0.92, 0.94), st), fillA(d));
    return o;
}

vec3 palPick(float k) {
    k = mod(floor(k), 6.0);
    if (k < 0.5) return cLime.rgb;
    if (k < 1.5) return cCyan.rgb;
    if (k < 2.5) return cRed.rgb;
    if (k < 3.5) return cYellow.rgb;
    if (k < 4.5) return cViolet.rgb;
    return vec3(0.97, 0.93, 0.95);
}

// ── acid: soap-film / oil-slick interference ─────────────────────────────
vec3 thinFilm(float th) {
    // interference colours of a thin film: each channel cycles at its own wavelength
    vec3 c = 0.5 + 0.5 * cos(TAU * (th * vec3(4.4, 5.3, 6.5) + vec3(0.0, 0.18, 0.42)));
    c = pow(c, vec3(2.0)) * 1.25;
    return c * 1.15;
}
float filmThickness(vec2 p, float t) {
    vec2 q = p * 1.6;
    q += 0.6 * vec2(sin(q.y * 1.7 + t), cos(q.x * 1.3 - t * 0.8));
    q += 0.35 * vec2(sin(q.y * 3.1 - t * 1.3 + 1.0), cos(q.x * 2.7 + t * 1.1));
    return 0.5 + 0.25 * sin(q.x * 1.4 + q.y * 0.7) + 0.18 * sin(q.y * 2.6 - q.x * 1.9 + t * 0.7) + 0.08 * sin(length(q) * 6.0 - t * 2.0);
}
float vn2(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1, 0)), f.x), mix(h12(i + vec2(0, 1)), h12(i + vec2(1, 1)), f.x), f.y);
}

float fwPool(float n, float thr, float pxScale) {
    // anti-aliased pool coverage
    float w = 1.5 / pxScale * 2.0;
    return smoothstep(thr - w, thr + w, n);
}

vec3 paper(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    vec2 p = (fc - 0.5 * R) / R.y / zoom;
    // build tremor near the top of the riser
    float trem = smoothstep(0.65, 1.0, gBuild);
    p += (h22(vec2(floor(TIME * 24.0), 3.7)) - 0.5) * 0.007 * trem;
    float aspect = R.x / R.y;
    float rD = length(p);
    // drop shockwave: a ring racing out from the centre as the impulse decays
    float shockR = (1.0 - gDrop) * 1.4;
    float shock = gDrop * exp(-pow((rD - shockR) / (0.05 + 0.1 * (1.0 - gDrop)), 2.0));
    // additive clocks only: music clocks + bounded build offsets (no phase jumps)
    gT = TIME * (0.6 + 0.4 * swim) + 0.3 * audioBassTime * ar + 0.25 * audioMidTime * ar
       + 2.5 * gBuild * gBuild;
    gWig = wiggle * (1.0 + 0.6 * midP() * ar) * mix(0.55, 1.3, gLift);

    float D = 1e5;
    vec4 ink = vec4(0.0);
    float n = floor(clamp(glyphCount, 3.0, 12.0) + 0.5);
    // population: rest keeps ~40 % of the organisms, peak all of them
    float popF = mix(0.5, 1.08, smoothstep(0.15, 0.85, gLift));

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
        // build: the colony coils in toward the centre; drop: it scatters back out
        c *= 1.0 - 0.22 * gBuild + 0.12 * gDrop;
        c = rot(0.35 * gBuild * gBuild) * c;
        float rank = h11(fi * 11.37 + 0.71);
        float vis = clamp((popF - rank) * 7.0, 0.0, 1.0);
        if (vis < 0.001) continue;
        // life: grow in, live, shrink out (each organism on its own clock)
        float lp = TIME / lifeCycle + sd;
        float lf = fract(lp);
        float life = smoothstep(0.0, 0.15, lf) * smoothstep(1.0, 0.85, lf);
        float gen = floor(lp);
        float typ = floor(h11(fi * 7.31 + gen * 3.7) * 17.0);
        gAcc = palPick(h11(fi * 2.9 + gen * 1.3) * 6.0);
        gAcc2 = palPick(h11(fi * 4.3 + gen * 2.1) * 6.0 + 1.0);
        float ang = h11(fi * 5.1 + floor(lp)) * TAU + 0.15 * sin(TIME * 0.2 * swim + fi);
        float sc = (0.17 + 0.1 * h11(fi * 6.1 + gen)) * glyphSize * max(life * vis, 0.001)
                 * mix(0.78, 1.12, gLift) * (1.0 - 0.18 * gBuild) * (1.0 + 0.28 * gDrop);
        vec2 q = rot(-ang) * (p - c) / sc;
        q *= vec2(mix(0.85, 1.2, h11(fi * 8.7 + gen)), mix(0.85, 1.2, h11(fi * 3.3 + gen)));
        gPx = 1.0 / (R.y * zoom * sc);
        float bound = length(q) - 1.45;
        float d;
        vec4 g = vec4(0.0);
        if (bound > 0.9) {
            d = bound;                                 // far away: the bound is enough for the line field
        } else if (typ < 0.5) g = beadChain(q, sd, d);
        else if (typ < 1.5) g = target(q, sd, d);
        else if (typ < 2.5) g = tadpole(q, sd, d, gAcc);
        else if (typ < 3.5) g = fern(q, sd, d);
        else if (typ < 4.5) g = worm(q, sd, d);
        else if (typ < 5.5) g = comb(q, sd, d);
        else if (typ < 6.5) g = lobe(q, sd, d);
        else if (typ < 7.5) g = spiralShell(q, sd, d);
        else if (typ < 8.5) g = urchin(q, sd, d);
        else if (typ < 9.5) g = amoeba(q, sd, d);
        else if (typ < 10.5) g = radiolarian(q, sd, d);
        else if (typ < 11.5) g = jelly(q, sd, d);
        else if (typ < 12.5) g = dividing(q, sd, d);
        else if (typ < 13.5) g = coral(q, sd, d);
        else if (typ < 14.5) g = spores(q, sd, d);
        else if (typ < 15.5) g = eye(q, sd, d);
        else g = ribbon(q, sd, d);
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
    // rest: sparse lines; build: they tighten and race outward; drop: the shockwave kinks them
    float dens = lineDensity * mix(0.7, 1.08, gLift) * (1.0 + 0.35 * gBuild);
    float ph = Dall * dens - TIME * lineFlow * 2.0 - audioMidTime * lineFlow * 0.6 * ar - beatW * 1.5
             - lineFlow * 8.0 * gBuild * gBuild * gBuild - shock * 2.5;
    float fw = dens / (R.y * zoom);                        // phase change per pixel
    float lw = lineWidth * 0.5;
    float line = clamp((lw - abs(fract(ph) - 0.5)) / max(fw, 1e-4) + 0.5, 0.0, 1.0);
    // halo: lines and paper deepen toward the organisms
    float hal = exp(-max(D, 0.0) * 14.0) * halo * mix(0.55, 1.15, gLift);
    vec3 pap = paper(uv);
    vec3 ground = mix(pap, mix(pap, cMagenta.rgb, 0.85), clamp(hal, 0.0, 1.0));
    // lines are magenta on the open paper and deepen to ink inside the halos
    vec3 lineC = mix(cMagenta.rgb, cInk.rgb, clamp(hal * 1.1, 0.0, 1.0));
    vec3 col = mix(ground, lineC, line);
    // a faint magenta wash drifting through the open field
    col = mix(col, cMagenta.rgb, 0.06 * (0.5 + 0.5 * sin(p.x * 2.0 + p.y * 1.3 + TIME * 0.1)));

    // ── acid pools: oil-slick film with bubble foam and black voids ──
    float ft = TIME * filmSpeed * 0.5 + 0.3 * audioBassTime * ar * filmSpeed + 0.2 * audioHighTime * ar * filmSpeed
             + 3.0 * gBuild * gBuild * filmSpeed;
    float acidK = acid * mix(0.7, 1.12, gLift);
    if (acidK > 0.001) {
        vec2 ap = p * acidScale + vec2(0.07, -0.05) * ft;
        float pn = vn2(ap * 1.3 + 0.4 * vec2(sin(ap.y + ft * 0.3), cos(ap.x - ft * 0.2))) * 0.65 + vn2(ap * 3.1 - ft * 0.15) * 0.35;
        float thr = 1.0 - clamp(acidPools * mix(0.7, 1.2, gLift) + 0.3 * gDrop, 0.0, 1.2) * 0.75;
        float pw = fwPool(pn, thr, R.y * zoom / acidScale);
        float poolEdge = exp(-abs(pn - thr) * 90.0);
        float th = filmThickness(p * acidScale, ft) + 0.15 * midP() * ar + 0.5 * gDrop * (1.0 - rD) + 0.08 * gBuild;
        vec3 film = thinFilm(th);
        // pale film between bands (like the cream soap areas of the reference)
        film = mix(film, vec3(0.98, 0.95, 0.88), smoothstep(0.6, 0.85, fract(th * 2.0)) * 0.55);
        // bubble foam: varied little bubbles, iridescent rings, some black voids
        vec2 bp = p * foamSize + vec2(0.0, ft * 0.6);
        vec2 bi = floor(bp), bf = fract(bp);
        vec3 foamC = film; float foamA = 0.0;
        for (int j = -1; j <= 1; j++)
        for (int k = -1; k <= 1; k++) {
            vec2 o = vec2(float(k), float(j));
            vec2 hh = h22(bi + o);
            vec2 bc = o + 0.5 + 0.3 * (hh - 0.5);
            float br = 0.18 + 0.3 * h12(bi + o + 7.7) * (0.6 + 0.4 * sin(ft * 2.0 + hh.x * 20.0));
            float bd = length(bf - bc) / br;
            if (bd < 1.0) {
                float kind = h12(bi + o + 3.3);
                vec3 bcCol = kind < 0.3 ? cBlack.rgb
                           : thinFilm(th + hh.y * 0.6 + bd * 0.35) * (kind < 0.6 ? 1.0 : 0.6);
                bcCol = mix(bcCol, vec3(1.0), 0.6 * smoothstep(0.35, 0.0, length(bf - bc - br * vec2(-0.35, 0.35))) * step(0.3, kind));
                float ringd = smoothstep(0.78, 0.86, bd) * smoothstep(1.0, 0.92, bd);
                bcCol = mix(bcCol, mix(cBlack.rgb, thinFilm(th + 0.3), 0.4), ringd);
                foamC = bcCol; foamA = smoothstep(1.0, 0.9, bd);
            }
        }
        float foamMask = foam * smoothstep(0.0, 0.08, pn - thr) * (0.35 + 0.65 * smoothstep(0.35, 0.6, vn2(ap * 2.2 + 4.0)));
        vec3 pool = mix(film, foamC, foamA * foamMask);
        // big black voids drifting in the film
        float vd = vn2(ap * 4.0 + 9.0 + ft * 0.2);
        pool = mix(pool, cBlack.rgb, smoothstep(0.66, 0.68, vd) * (1.0 - foamMask * 0.5));
        // crisp dark seams where film bands meet (the sharp oil-slick edges)
        pool = mix(pool, cBlack.rgb, 0.7 * smoothstep(0.03, 0.0, abs(fract(th * 2.2) - 0.5)) * (1.0 - foamA * foamMask));
        col = mix(col, pool, pw * acidK);
        // pool shoreline: a black line with a rainbow rim
        col = mix(col, cBlack.rgb, poolEdge * 0.9 * acidK);
        col = mix(col, thinFilm(th * 1.7 + 0.2), smoothstep(0.04, 0.0, abs(pn - thr - 0.03)) * 0.8 * acidK);
    }


    // organisms; their dark bodies carry an iridescent sheen when the acid is up
    vec3 inkC = ink.rgb;
    if (acidK > 0.001 && bodySheen > 0.001) {
        float dark = smoothstep(0.12, 0.02, lum(inkC));
        float th2 = filmThickness(p * 2.3 + 3.0, ft * 1.3);
        inkC = mix(inkC, thinFilm(th2) * 0.9, dark * bodySheen * min(acidK, 1.0) * 0.75);
    }
    col = mix(col, inkC, ink.a);
    // drop: the shockwave front is a band of raw oil-slick rainbow over everything
    col = mix(col, thinFilm(rD * 2.0 + TIME * 0.1), clamp(shock * 0.8, 0.0, 1.0));
    float shockEdge = gDrop * exp(-pow((rD - shockR - 0.04 - 0.08 * (1.0 - gDrop)) / 0.014, 2.0));
    col = mix(col, vec3(1.0, 0.97, 0.99), clamp(shockEdge, 0.0, 1.0));
    col = mix(col, cBlack.rgb, 0.5 * gDrop * exp(-pow((rD - shockR - 0.07 - 0.08 * (1.0 - gDrop)) / 0.02, 2.0)));
    col *= 1.0 + 0.1 * audioLevel * ar;
    // build shimmer: a sweep whose rate climbs with the riser (phase grows ~ b^3, no jumps)
    float shim = 0.5 + 0.5 * sin(gBuild * gBuild * gBuild * 60.0 + p.x * 4.0 - p.y * 2.0);
    col *= 1.0 + 0.14 * gBuild * shim + 0.18 * gDrop * exp(-rD * 2.0);
    col = hueRot(col, hueShift);
    col = mix(vec3(lum(col)), col, saturation * mix(0.72, 1.18, gLift));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.08;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    float alpha = transparentBg ? max(ink.a, line * 0.9) : 1.0;
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), alpha);
}
