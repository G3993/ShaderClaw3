/*{
  "DESCRIPTION": "Diatoms — a living microscope slide of glass skeletons, each one a little lens filled with the bold graphic patterns of Objectos. Centric diatoms are round silica discs whose interiors hold concentric patterns in perfect radial symmetry: rings of polka dots that double in number toward the rim, polar checkerboards whose rings counter-rotate, stripes that ripple outward from the centre, glowing centre-to-rim gradients, glossy ink domes, dark-rimmed matte lenses and oily iridescent film. Pennate diatoms are long boat shapes with a sinuous raphe line and nodules, their fill mirrored along the long axis so bands, chevrons, dots and checks flow outward toward both tips. Triangular and square Triceratium valves carry the same fills around glowing ocelli, and radiolaria are spiky lattice spheres whose patterns are painted on a true 3D sphere that turns behind the lattice. Every fill sits under a domed glass valve: it is lit as a 3D lens with a sharp specular highlight and a refractive bulge toward the rim, while the glowing glass edges, raphe, ridges and spines stay crisp on top. Patterns use the Objectos palette (signal red, cyan, maroon, pink, lavender, ink) and each specimen slowly trades pattern and colours on its own clock (or lock one with Pattern). Four views: Darkfield, iridescent glass on black; Brightfield, coloured glass on a pale cream field with a circular field stop; Mandala Slide, a Victorian arranged slide of concentric rings of species around one great disc, clicking round a step at a time; and Radiolaria, a drifting field of spined spheres. Auto Morph changes slides with an iris wipe. Bass pulses the specimens and squeezes their patterns, mids twist the fills into swirls and push the stripes, beats step the mandala rings, highs flare the lens highlights and make the glass edges glint; a drop blows the mandala apart before it reassembles. At rest the slide is sparse, slow and softly lit with muted fills; during a build it contracts, spins faster and the highlights shimmer quicker with a tremor at the top; the drop is a flash, a shockwave and an explosion of the arrangement, and the peak is dense, saturated and in full motion.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Organic", "Science", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Slide", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Darkfield", "Brightfield", "Mandala Slide", "Radiolaria"], "GROUP": "Form" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 6.0, "MAX": 90.0, "DEFAULT": 20.0, "GROUP": "Form" },
    { "NAME": "density",     "LABEL": "Specimen Density", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Form" },
    { "NAME": "drift",       "LABEL": "Drift",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "spinSpeed",   "LABEL": "Rotation",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "pulseAmt",    "LABEL": "Breathing",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "poreShimmer", "LABEL": "Pore Shimmer",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "patternSel",  "LABEL": "Pattern", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7], "LABELS": ["Auto · Mixed", "Glossy", "Glowing Gradient", "Dark-Rim Matte", "Sliding Stripes", "Polka Dots", "Checks", "Iridescent"], "GROUP": "Patterns" },
    { "NAME": "patternAmt",  "LABEL": "Pattern Amount",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Patterns" },
    { "NAME": "patternScale","LABEL": "Pattern Scale",    "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Patterns" },
    { "NAME": "iridescence", "LABEL": "Iridescence",      "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.9, "GROUP": "Look" },
    { "NAME": "glow",        "LABEL": "Glass Glow",       "TYPE": "float", "MIN": 0.2, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 3, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.02, 0.03, 0.05, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec3  h33(vec3 p)  { p = fract(p * vec3(0.1031, 0.1030, 0.0973)); p += dot(p, p.yxz + 33.33); return fract((p.xxy + p.yxx) * p.zyx); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

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

float gT, gSpin, gShim, gBr;
// interior (valve face) of the current specimen, set by the specimen functions:
// signed distance to the inner outline (<0 inside), lens radius (elliptical, 0 centre … 1 rim) + its direction,
// radial symmetry order, base pattern frequency, sphere direction (radiolaria)
float gInD, gInW, gDome, gSym, gK0, gPoreK, gSlide, gSpecG;
vec2 gInG;
vec3 gSphN;

float cov(float d, float aa) { return smoothstep(aa, -aa, d); }
float ridge(float d, float w) { float x = d / w; return exp(-x * x); }

// hex lattice (neighbour spacing 1) → offset from nearest cell centre
vec2 hexLocal(vec2 p) {
    const vec2 s = vec2(1.0, 1.7320508);
    vec2 a = mod(p, s) - s * 0.5;
    vec2 b = mod(p - s * 0.5, s) - s * 0.5;
    return dot(a, a) < dot(b, b) ? a : b;
}
float hexD(vec2 h) { h = abs(h); return max(h.x, dot(h, vec2(0.5, 0.8660254))); }   // 0.5 at the cell wall

vec2 vor3(vec3 x) {
    vec3 ip = floor(x), fp = fract(x);
    float d1 = 8.0, d2 = 8.0;
    for (int k = -1; k <= 1; k++)
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec3 g = vec3(float(i), float(j), float(k));
        vec3 rr = g + h33(ip + g) * 0.7 + 0.15 - fp;
        float d = dot(rr, rr);
        if (d < d1) { d2 = d1; d1 = d; } else if (d < d2) { d2 = d; }
    }
    return sqrt(vec2(d1, d2));
}
mat3 rotM(float a, float b) {
    float ca = cos(a), sa = sin(a), cb = cos(b), sb = sin(b);
    mat3 ry = mat3(ca, 0.0, -sa, 0.0, 1.0, 0.0, sa, 0.0, ca);
    mat3 rx = mat3(1.0, 0.0, 0.0, 0.0, cb, sb, 0.0, -sb, cb);
    return ry * rx;
}

// Every specimen returns vec4(silica body, glowing edges/ridges, pore holes, footprint),
// in local units (radius / half-length 1). aa = one pixel in local units.

// ── centric diatom: round disc ──────────────────────────────────────────
vec4 centric(vec2 q, float sd, float aa) {
    float r = length(q);
    if (r > 1.12) return vec4(0.0);
    float a = atan(q.y, q.x);
    float foot = cov(r - 1.0, aa);
    float edge = ridge(r - 1.0, aa + 0.022) * 1.3 + ridge(r - 0.915, aa + 0.012) * 0.6 * foot;
    float hole = 0.0, pe = 0.0;
    float style = floor(fract(sd * 7.31) * 3.0);
    gInD = r - 0.9; gInW = 0.9; gDome = r / 0.9; gInG = q / max(r, 1e-3); gK0 = 5.5;
    gSym = 6.0 + 2.0 * step(0.5, fract(sd * 2.9));
    if (style < 0.5) {
        // radiating rows of pores, doubling toward the rim (Coscinodiscus / Arachnoidiscus feel)
        float N = 18.0 + 6.0 * floor(fract(sd * 13.7) * 3.0);
        if (r > 0.54) N *= 2.0;
        float K = 11.0 + 3.0 * fract(sd * 3.3);
        float rr = r * K;
        float ring = floor(rr);
        float u = a / TAU * N + 0.5 * mod(ring, 2.0);
        vec2 c = vec2(fract(u) - 0.5, fract(rr) - 0.5);
        float cw = max(r, 1e-3) * TAU / N, ch = 1.0 / K;
        float cell = min(cw, ch);
        float d = length(c * vec2(cw, ch)) - 0.33 * cell * gBr;
        float lod = smoothstep(0.55, 0.22, aa / cell);
        float inside = smoothstep(0.21, 0.26, r) * (1.0 - smoothstep(0.84, 0.88, r));
        hole = cov(d, aa) * inside * lod;
        pe = ridge(d, aa + 0.09 * cell) * inside * lod;
        edge += ridge((0.5 - abs(c.x)) * cw, aa + 0.004) * 0.3 * inside * lod * gPoreK;     // radial costae
        edge += ridge(r - 0.205, aa + 0.01) * 0.7;
        // central rosette of big pores
        float ra = mod(a + PI / 6.0, TAU / 6.0) - PI / 6.0;
        float dr = length(vec2(cos(ra), sin(ra)) * r - vec2(0.115, 0.0)) - 0.042 * gBr;
        hole = max(hole, cov(dr, aa));
        pe += ridge(dr, aa + 0.01);
    } else {
        // honeycomb of hexagonal areolae, finer toward the rim; style 2 = sectored wheel
        float K = 8.0 + 5.0 * fract(sd * 5.9);
        float warp = 1.0 + 0.35 * r * r;
        vec2 h = hexLocal(q * K * warp);
        float cell = 1.0 / (K * warp);
        float lod = smoothstep(0.5, 0.2, aa / cell);
        float fillA = 0.72 * gBr;
        float inside = 1.0 - smoothstep(0.84, 0.89, r);
        float lift = 1.0;
        if (style > 1.5) {
            float Ns = 6.0 + 2.0 * step(0.5, fract(sd * 2.9));
            float sa = TAU / Ns;
            float ia = floor(a / sa);
            float raised = mod(ia, 2.0);
            float fa = a - ia * sa;
            float bd = r * sin(min(fa, sa - fa));
            edge += ridge(bd, aa + 0.008) * 0.7 * smoothstep(0.12, 0.2, r) * foot;
            lift = mix(0.55, 1.35, raised);
            fillA *= mix(0.85, 0.6, raised);
            inside *= smoothstep(0.14, 0.2, r);
            edge += ridge(r - 0.14, aa + 0.01) * 0.8;
        }
        float hd = (hexD(h) - 0.5 * fillA) * cell;
        hole = cov(hd, aa) * inside * lod;
        pe = ridge(hd, aa + 0.07 * cell) * inside * lod * lift;
        edge += ridge(r - 0.06, aa + 0.008) * 0.5;
    }
    pe *= (0.7 + 0.3 * sin(a * 3.0 + r * 9.0 - gShim)) * gPoreK;
    return vec4(foot * (1.0 - hole) * (0.6 + 0.4 * r), edge + pe * 0.85, hole, foot);
}

// ── pennate diatom: long boat with raphe and striae ─────────────────────
vec4 pennate(vec2 q, float sd, float aa) {
    float x = q.x, ax = abs(x), ay = abs(q.y);
    if (ax > 1.04) return vec4(0.0);
    float W = 0.15 + 0.1 * fract(sd * 5.3);
    float sk = fract(sd * 3.1);
    float hw = W * pow(max(1.0 - x * x, 0.0), 0.45 + 0.5 * sk);
    if (sk > 0.62) {                                          // capitate ends
        hw *= 1.0 - 0.45 * exp(-pow((ax - 0.7) / 0.09, 2.0));
        hw = max(hw, W * 0.5 * exp(-pow((ax - 0.87) / 0.07, 2.0)));
    }
    float d = (ay - hw) * 0.85;
    if (d > 0.08) return vec4(0.0);
    float foot = cov(d, aa);
    float edge = ridge(d, aa + 0.012) * 1.3 + ridge(d + 0.03, aa + 0.008) * 0.45 * foot;
    gInD = d + 0.035; gInW = W; gK0 = 2.4 / W; gSym = 2.0;
    vec2 de = vec2(x / 0.95, q.y / (W * 0.85));
    gDome = length(de); gInG = de / max(gDome, 1e-3);
    // raphe: sinuous central slit with central + terminal nodules
    float rl = smoothstep(0.05, 0.08, ax) * (1.0 - smoothstep(0.86, 0.9, ax));
    edge += ridge(q.y + 0.005 * sin(x * 9.0), aa + 0.004) * rl * 0.95;
    float cn = length(vec2(x / 0.075, q.y / max(W * 0.55, 0.05))) - 1.0;
    edge += ridge(cn * 0.06, aa + 0.006) * 0.55;
    edge += ridge(length(vec2(ax - 0.895, q.y)) - 0.02, aa + 0.006) * 0.8;
    // striae: rows of pores, radiating toward the centre
    float F = 24.0 + 12.0 * fract(sd * 9.1);
    float s = (x - sign(x) * ay * 0.45) * F;
    float axial = 0.03 + 0.025 * fract(sd * 2.3);
    float inStr = smoothstep(axial, axial + 0.02, ay) * smoothstep(-0.0, -0.03, d) * smoothstep(0.06, 0.1, ax);
    float G = F * 1.3;
    vec2 pc = vec2((fract(s) - 0.5) / F, (fract(ay * G) - 0.5) / G);
    float dp = length(pc) - 0.3 / F * gBr;
    float lod = smoothstep(0.5, 0.22, aa * G);
    float hole = cov(dp, aa) * inStr * lod;
    float pe = ridge(dp, aa + 0.06 / F) * inStr * lod;
    edge += ridge((0.5 - abs(fract(s) - 0.5)) / F, aa + 0.1 / F) * inStr * 0.4 * smoothstep(0.5, 0.22, aa * F) * gPoreK;
    pe *= (0.5 + 0.5 * sin(x * 7.0 + ay * 20.0 - gShim)) * gPoreK;
    return vec4(foot * (1.0 - hole) * 0.8, edge + pe * 0.85, hole, foot);
}

// ── Triceratium-like polygon valve with ocelli ──────────────────────────
vec4 polyD(vec2 q, float sd, float aa) {
    float r = length(q);
    if (r > 1.1) return vec4(0.0);
    float n = 3.0 + step(0.5, fract(sd * 4.7));
    float a = atan(q.y, q.x);
    float an = PI / n;
    float bn = mod(a + an, 2.0 * an) - an;
    float conc = (fract(sd * 8.3) - 0.4) * 0.12;
    float dpoly = r * cos(bn) - 0.98 * cos(an) - conc * (1.0 - pow(bn / an, 2.0));
    float d = max(dpoly, r - 0.86);
    if (d > 0.08) return vec4(0.0);
    float foot = cov(d, aa);
    float edge = ridge(d, aa + 0.02) * 1.3 + ridge(d + 0.075, aa + 0.01) * 0.55 * foot;
    float va = a - bn + an * (bn < 0.0 ? -1.0 : 1.0);
    vec2 oc = 0.66 * vec2(cos(va), sin(va));
    float od = length(q - oc) - 0.1;
    float ocIn = cov(od, aa);
    gInD = max(d + 0.085, 0.012 - od); gInW = 0.72; gDome = r / 0.8; gInG = q / max(r, 1e-3); gK0 = 6.0; gSym = n;
    edge += ridge(od, aa + 0.01) * 0.9;
    float K = 6.0 + 3.0 * fract(sd * 6.1);
    vec2 h = hexLocal(q * K);
    float cell = 1.0 / K;
    float lod = smoothstep(0.5, 0.2, aa / cell);
    float hd = (hexD(h) - 0.5 * 0.74 * gBr) * cell;
    float inside = smoothstep(-0.07, -0.11, d) * (1.0 - ocIn);
    float hole = cov(hd, aa) * inside * lod;
    float pe = ridge(hd, aa + 0.07 * cell) * inside * lod;
    edge += ocIn * 0.4 * (0.5 + 0.5 * sin(length(q - oc) * 110.0 - gShim)) * smoothstep(0.5, 0.2, aa * 18.0);
    pe *= (0.5 + 0.5 * sin(a * n + r * 10.0 - gShim)) * gPoreK;
    edge += ridge(r - 0.1, aa + 0.008) * 0.5;
    return vec4(foot * (1.0 - hole) * 0.85, edge + pe * 0.85, hole, foot);
}

// ── radiolarian: lattice sphere with spines ─────────────────────────────
vec4 radiolarian(vec2 q, float sd, float aa) {
    float r = length(q);
    if (r > 1.08) return vec4(0.0);
    float R = 0.4 + 0.08 * fract(sd * 3.7);
    mat3 M = rotM(sd * 40.0 + gSpin * 0.35, sd * 23.0 + gSpin * 0.22);
    float body = 0.0, edge = 0.0, hole = 0.0;
    float foot = cov(r - R, aa);
    if (r < R + 0.05) {
        vec2 u = q / R;
        float z = sqrt(max(1.0 - dot(u, u), 0.0));
        float K = 3.0 + 1.8 * fract(sd * 9.3);
        float aaK = aa / R * K / (z + 0.2);
        gInD = r - R * 0.985; gInW = R; gDome = r / R; gInG = q / max(r, 1e-3); gK0 = 3.2; gSym = 5.0;
        for (int hh = 0; hh < 2; hh++) {
            float sg = hh == 0 ? 1.0 : -1.0;
            vec3 nn = M * vec3(u, z * sg);
            if (hh == 0) gSphN = nn;
            vec2 F = vor3(nn * K + sd * 31.0);
            float wd = F.y - F.x;
            float ww = 0.09 * (2.0 - gBr);
            float wall = 1.0 - smoothstep(ww, ww + 0.1 + aaK, wd);
            float wt = hh == 0 ? 1.0 : 0.38;
            edge += wall * wt * (0.45 + 0.55 * z) * foot;
            body += wall * wt * 0.6 * foot;
            if (hh == 0) hole = (1.0 - wall) * foot;
        }
        edge += pow(1.0 - z, 3.0) * 0.8 * foot + ridge(r - R, aa + 0.015) * 1.1;
        edge += ridge(r - R * 0.42, aa + 0.01) * 0.5;
    }
    float nSp = 7.0 + floor(fract(sd * 5.1) * 8.0);
    for (int i = 0; i < 14; i++) {
        float fi = float(i);
        if (fi >= nSp) break;
        vec3 dv = normalize(h33(vec3(sd * 91.0, fi * 1.7, 3.1)) * 2.0 - 1.0 + 1e-3);
        dv = M * dv;
        vec2 dp = dv.xy;
        float L2 = dot(dp, dp);
        if (L2 < 2e-3) continue;
        float t = clamp(dot(q, dp) / L2, 0.0, 1.0);
        float dist = length(q - dp * t);
        float w = 0.03 * pow(1.0 - t, 0.8) + 0.002;
        float on = smoothstep(R * 0.8, R * 1.0, t);
        float bk = dv.z < 0.0 ? 0.45 : 1.0;
        float spn = cov(dist - w, aa) * on;
        edge += (spn * 0.7 + ridge(dist - w, aa + 0.006) * on * 0.6) * bk;
        body += spn * 0.5 * bk;
    }
    return vec4(body, edge, hole, max(foot, min(body, 1.0)));
}

// ── Objectos patterns, ported onto the valve face ───────────────────────
// Objectos palette, ordered dark / light alternately (red, cyan, maroon, pink, ink, lavender)
// so an odd index offset always pairs a dark with a light colour (reads in every Studio Palette)
vec3 objC(float k) {
    k = mod(k, 6.0);
    if (k < 0.5) return vec3(1.0, 0.02, 0.02);
    if (k < 1.5) return vec3(0.0, 0.9, 0.95);
    if (k < 2.5) return vec3(0.42, 0.0, 0.12);
    if (k < 3.5) return vec3(1.0, 0.55, 0.92);
    if (k < 4.5) return vec3(0.03, 0.03, 0.04);
    return vec3(0.6, 0.5, 1.0);
}
// anti-aliased square wave (edges at .25/.75), w = filter width in cell units
float sqw(float x, float w) { return clamp((0.25 - abs(fract(x) - 0.5)) / w, -1.0, 1.0); }
// m: 0 glossy, 1 glowing gradient, 2 dark-rim matte, 3 sliding stripes, 4 polka dots,
// 5 checks, 6 iridescent.  g = (radial/along cell coord, angular/across cell coord),
// cap = centre disc of a radial layout, aaC = pixel footprint in cells, n = lens normal,
// rl = 0 centre … 1 rim
vec3 patMat(float m, float ia, float ib, vec2 g, float cap, float aaC, vec3 n, float rl, float sd) {
    vec3 A = objC(ia), B = objC(ib);
    vec3 L = normalize(vec3(-0.42, 0.55, 0.72));
    float ndl = dot(n, L);
    float diff = clamp(ndl, 0.0, 1.0), wrap = clamp(ndl * 0.5 + 0.5, 0.0, 1.0);
    float nv = clamp(n.z, 0.0, 1.0);
    float fres = pow(1.0 - nv, 2.0);
    float nh = clamp(dot(n, normalize(L + vec3(0.0, 0.0, 1.0))), 0.0, 1.0);
    float spec = pow(nh, 90.0) * gSpecG;
    float lod = 1.0 - smoothstep(0.25, 0.6, aaC);
    if (m < 0.5) {
        vec3 env = mix(vec3(0.03, 0.03, 0.05), vec3(0.45, 0.5, 0.6), n.y * 0.5 + 0.5);
        return A * (0.15 + 0.75 * diff) + env * 0.3 * fres + vec3(spec * 0.9 + 0.08 * pow(nh, 12.0));
    }
    if (m < 1.5) {
        vec3 c = mix(A, B, smoothstep(0.0, 1.0, rl)) * 1.05 * (0.75 + 0.25 * wrap);
        return c + mix(B, vec3(1.0), 0.3) * fres * 0.35;
    }
    if (m < 2.5) {
        vec3 c = mix(B, A, pow(nv, 1.5));
        c = mix(c, vec3(0.015) + B * 0.1, smoothstep(0.55, 1.0, fres) * 0.8);
        return c * (0.65 + 0.45 * wrap) + vec3(spec * 0.25);
    }
    float s;
    if (m < 3.5) {
        float w = max(TAU * aaC * 0.9, 0.04);
        s = mix(0.5, smoothstep(-w, w, sin(TAU * g.x - gSlide)), lod);
    } else if (m < 4.5) {
        vec2 cc = cap > 0.5 ? vec2(g.x, 0.0) : fract(g) - 0.5;
        s = cov(length(cc) - 0.31 * gBr, max(aaC, 0.01));
        lod = mix(lod, 1.0, cap * step(g.x, 0.45));
        s = mix(0.3, s, lod);
    } else if (m < 5.5) {
        float w = max(aaC * 0.75, 0.015);
        s = mix(0.5, 0.5 + 0.5 * sqw(g.x, w) * mix(sqw(g.y, w), 1.0, cap), lod);
    } else {
        float t = nv * 1.8 + rl * 0.8 + TIME * 0.15 + sd * 3.0;
        vec3 ir = 0.5 + 0.5 * cos(TAU * (t + vec3(0.0, 0.33, 0.67)));
        return mix(A, ir, 0.65) * (0.35 + 0.75 * wrap) + vec3(spec * 0.8);
    }
    return mix(A, B, s) * (0.3 + 0.8 * wrap) + vec3(spec * 0.5);
}

vec4 specimen(vec2 q, float type, float sd, float aa) {
    if (type < 0.5) return centric(q, sd, aa);
    if (type < 1.5) return pennate(q, sd, aa);
    if (type < 2.5) return polyD(q, sd, aa);
    return radiolarian(q, sd, aa);
}

// mandala ring layout: radius, slot count, specimen size
void ringP(float i, out float Rr, out float N, out float S) {
    if (i < 0.5)      { Rr = 0.0;   N = 1.0;  S = 0.16; }
    else if (i < 1.5) { Rr = 0.29;  N = 16.0; S = 0.075; }
    else if (i < 2.5) { Rr = 0.445; N = 14.0; S = 0.065; }
    else if (i < 3.5) { Rr = 0.58;  N = 28.0; S = 0.048; }
    else if (i < 4.5) { Rr = 0.705; N = 24.0; S = 0.058; }
    else { Rr = 0.83 + (i - 5.0) * 0.12; N = 40.0 + (i - 5.0) * 10.0; S = 0.042; }
}

// form: 0 Darkfield field, 1 Brightfield field, 2 Mandala slide, 3 Radiolaria field
vec4 renderForm(float form, vec2 sp, vec2 uv) {
    float ar = audioReact;
    float px = 1.0 / RENDERSIZE.y;
    float bp = bassP() * ar * pulseAmt;
    vec2 q = vec2(9.0);
    float type = 0.0, sd = 0.0, aa = 0.01, present = 1.0, dim = 1.0;
    bool bright = form > 0.5 && form < 1.5;
    if (form > 1.5 && form < 2.5) {
        // ── Victorian arranged slide ──
        float mz = 1.4 * (1.0 + 0.3 * gBuild - 0.1 * gRest);
        vec2 p = sp * mz;
        float r = length(p), a = atan(p.y, p.x);
        float best = 1e3, bR = 0.0, bN = 1.0, bS = 0.1, bI = 0.0;
        for (int i = 0; i < 14; i++) {
            float fi = float(i);
            float Ri, Ni, Si;
            ringP(fi, Ri, Ni, Si);
            Ri *= 1.0 + gDrop * (0.3 + 0.16 * fi);
            Si *= 1.0 + pulseAmt * (0.035 * sin(gT * 1.3 - fi * 0.8)) + 0.07 * bp * exp(-fi * 0.15) + 0.15 * gDrop;
            float dd = i == 0 ? r - Si : abs(r - Ri) - Si;
            if (dd < best) { best = dd; bR = Ri; bN = Ni; bS = Si; bI = fi; }
        }
        float dir = mod(bI, 2.0) < 0.5 ? 1.0 : -1.0;
        float slotA = TAU / bN;
        float bs = TIME / 5.0 + audioBassTime * 0.35 * ar + bI * 0.13;
        float stp = floor(bs) + smoothstep(0.82, 1.0, fract(bs));
        float rotA = dir * (gT * 0.03 * spinSpeed + stp * slotA + 0.6 * gBuild * gBuild + 1.2 * gDrop) + bI * 0.37;
        float k = floor((a - rotA) / slotA + 0.5);
        float ca = k * slotA + rotA;
        vec2 lp = rot(ca) * (p - bR * vec2(cos(ca), sin(ca)));
        float par = mod(k, 2.0);
        float S = bS;
        if (bI < 0.5) {
            lp = rot(gSpin * 0.12) * p; type = 0.0; sd = 0.71;
        } else if (bI < 1.5) {
            if (par < 0.5) { type = 1.0; sd = 0.12; } else { type = 0.0; sd = 0.38; S *= 0.45; }
        } else if (bI < 2.5) {
            type = 2.0; sd = 0.05; lp = rot(PI / 3.0) * lp;
        } else if (bI < 3.5) {
            type = 0.0; sd = par < 0.5 ? 0.33 : 0.52;
        } else if (bI < 4.5) {
            if (par < 0.5) { type = 1.0; sd = 0.71; lp = vec2(lp.y, -lp.x); } else { type = 0.0; sd = 0.18; S *= 0.5; }
        } else {
            float cyc = mod(bI, 3.0);
            if (cyc < 0.5)      { type = par < 0.5 ? 0.0 : 2.0; sd = par < 0.5 ? 0.43 : 0.14; S *= par < 0.5 ? 1.0 : 0.85; }
            else if (cyc < 1.5) { type = par < 0.5 ? 1.0 : 0.0; sd = par < 0.5 ? 0.27 : 0.61; S *= par < 0.5 ? 1.0 : 0.6; }
            else                { type = 0.0; sd = par < 0.5 ? 0.09 : 0.88; S *= par < 0.5 ? 1.0 : 0.7; }
        }
        // rest thins the outer rings; build/peak fill them back
        float vis = 1.0 - gRest * smoothstep(2.5, 5.5, bI) * (0.55 + 0.45 * par);
        present = step(0.05, vis);
        dim = mix(1.0, 0.9 - 0.04 * bI, 0.35) * vis;
        q = lp / S;
        aa = px * mz / S;
    } else {
        // ── drifting field of specimens on a slide ──
        float isR = form > 2.5 ? 1.0 : 0.0;
        float cs = mix(0.56, 0.3, density) * (1.0 + 0.1 * isR);
        vec2 p = sp * (1.0 + 0.35 * gBuild - 0.12 * gRest) * (1.0 - 0.22 * gDrop);
        p = rot(0.05 * sin(gT * 0.11)) * p;
        p += vec2(gT * 0.035, -gT * 0.014) * drift + vec2(17.3, 5.1) * form;
        vec2 g = p / cs;
        g.x += 0.5 * mod(floor(g.y), 2.0);
        vec2 id = floor(g);
        vec2 f = g - id - 0.5;
        float h0 = h12(id + form * 13.1), h2 = h12(id * 1.31 + 7.7), h3 = h12(id * 0.73 + 3.3);
        float h4 = h12(id + 11.9), h5 = h12(id + 23.1), h6 = h12(id * 1.7 + 5.5), h7 = h12(id * 2.3 + 1.9);
        float occ = mix(0.9, 0.55, gRest) * mix(0.8, 1.0, gPeak);
        present = step(h0, occ);
        if (isR > 0.5) type = h2 < 0.82 ? 3.0 : 0.0;
        else type = h2 < 0.42 ? 0.0 : h2 < 0.78 ? 1.0 : h2 < 0.93 ? 2.0 : 3.0;
        float size = type > 0.5 && type < 1.5 ? 0.4 : type > 2.5 ? 0.42 : 0.25 + 0.15 * h3;
        size *= 1.0 + pulseAmt * 0.035 * sin(gT * 0.8 + h3 * TAU) + 0.07 * bp + 0.08 * gDrop;
        float room = max(0.46 - size, 0.0);
        vec2 off = (vec2(h4, h5) - 0.5) * 1.2 * room + room * 0.4 * (0.5 + 0.5 * gPeak) * vec2(sin(gT * 0.5 + h6 * TAU), cos(gT * 0.43 + h7 * TAU));
        float ang = h6 * TAU + gSpin * 0.25 * (h6 > 0.5 ? 1.0 : -1.0) * (0.5 + h3);
        q = rot(ang) * (f - off) / size;
        float dof = 1.0 + 3.0 * pow(h7, 6.0) * (1.0 - 0.5 * gPeak);
        dim = 1.0 - 0.45 * (dof - 1.0) / 3.0;
        aa = px / (cs * size) * dof;
        sd = fract(h3 * 7.13 + h4 * 3.1);
    }
    vec4 S = vec4(0.0);
    gInD = 1.0; gInW = 1.0; gDome = 1.0; gInG = vec2(0.0); gSym = 6.0; gK0 = 5.0; gSphN = vec3(0.0, 0.0, 1.0);
    if (present > 0.5) S = specimen(q, type, sd, aa);
    S.xy *= dim;

    // ── Objectos pattern inside the valve, under a domed glass lens ──
    float pm = 0.0;
    vec3 pc = vec3(0.0), pn = vec3(0.0, 0.0, 1.0);
    if (S.w > 0.0 && patternAmt > 0.001 && gInD < aa) {
        float rho = min(gDome, 1.0);
        float zz = sqrt(1.0 - rho * rho);
        pn = normalize(vec3(gInG * rho * 0.85, zz + 0.12));   // softened spherical lens
        pm = cov(gInD, aa) * patternAmt;
        float K = gK0 * patternScale * (1.0 + 0.1 * bp);          // bass squeezes the fill
        vec2 qp = q * (0.94 + 0.06 * zz) - pn.xy * 0.05 * gInW;  // lens magnification + refraction
        float twA = 0.6 * sin(TIME * 1.3 + sd * 6.0) * mix(0.5, 1.3, gLift) + 0.8 * midP() * ar + 0.9 * gBuild * gBuild;
        vec2 g;
        float cap = 0.0, rl, aaC;
        if (type > 0.5 && type < 1.5) {
            // pennate: mirrored across the raphe and about the centre, flowing to both tips
            float ax = abs(qp.x), ay = abs(qp.y);
            g = vec2((ax + 0.25 * twA * ay) * K - 0.35 * gT, ay * K);
            rl = ax;
            aaC = aa * K;
        } else {
            float r, ang, sc = 1.0;
            if (type > 2.5) {                                     // radiolaria: on the rotating sphere
                r = acos(clamp(gSphN.z, -1.0, 1.0)) / (PI * 0.5);
                ang = atan(gSphN.y, gSphN.x);
                sc = 1.0 / (gInW * max(zz, 0.2) * PI * 0.5);
            } else {
                r = length(qp);
                ang = atan(qp.y, qp.x);
            }
            float rk = r * K;
            float ri = floor(rk);
            float rc = (ri + 0.5) / K;
            float circ = type > 2.5 ? sin(min(rc, 2.0) * PI * 0.5) / (PI * 0.5) : rc;
            float N = gSym * max(1.0, floor(TAU * circ * K / gSym + 0.5));
            ang += twA * r * 0.8 + (mod(ri, 2.0) * 2.0 - 1.0) * 0.2 * gSpin;   // swirl + counter-rotating rings
            g = vec2(rk, ang / TAU * N);
            cap = 1.0 - step(0.5, ri);
            rl = clamp(r, 0.0, 1.0);
            float circR = type > 2.5 ? sin(clamp(r, 0.02, 1.98) * PI * 0.5) / (PI * 0.5) : max(r, 1e-3);
            aaC = aa * sc * max(K, N / (TAU * circR) * (1.0 - cap));
        }
        float ph = TIME / 9.0 + h11(sd * 73.1) * 0.999;
        float kq = floor(ph);
        float fq = smoothstep(0.75, 1.0, fract(ph));
        float psel = float(patternSel);
        for (int k = 0; k < 2; k++) {
            float w = k == 0 ? 1.0 - fq : fq;
            if (w < 0.001) continue;
            float kk = kq + float(k);
            float mm = psel > 0.5 ? psel - 1.0 : floor(h11(sd * 23.3 + kk * 0.913) * 7.0);
            float p1 = floor(h11(sd * 41.1 + kk * 1.37) * 6.0);
            float p2 = p1 + 1.0 + 2.0 * floor(h11(sd * 83.7 + kk) * 3.0);
            pc += w * patMat(mm, p1, p2, g, cap, aaC, pn, rl, sd);
        }
        // glass lens on top: sharp highlight + cool fresnel rim
        float gnh = clamp(dot(pn, normalize(vec3(-0.42, 0.55, 0.72) + vec3(0.0, 0.0, 1.0))), 0.0, 1.0);
        pc += vec3(pow(gnh, 220.0) * 0.45 * gSpecG) + vec3(0.55, 0.72, 0.9) * pow(1.0 - pn.z, 3.0) * 0.3;
        if (type > 2.5) pc *= mix(0.6, 1.0, S.z);                 // lattice walls shade the fill
        pc *= (1.0 + 0.15 * audioLevel * ar) * mix(0.85, 1.12, gLift) * dim;
        pc = mix(vec3(lum(pc)), pc, 1.0 - 0.3 * gRest + 0.25 * gPeak + 0.2 * gBuild);
        pc = max(pc, 0.0);
    }

    // shading
    float irA = iridescence * mix(0.5, 1.0, gLift) * (1.0 - 0.45 * gRest) * (1.0 + 0.25 * gPeak);
    float phase = sd * 2.7 + length(q) * 0.3 - gT * 0.05 + S.y * 0.45 + gShim * 0.015;
    vec3 ir = 0.5 + 0.5 * cos(TAU * (phase + vec3(0.0, 0.33, 0.67)));
    float hiG = 1.0 + 0.9 * highP() * ar + 0.25 * gPeak - 0.2 * gRest;
    float gl = pow(clamp(S.y, 0.0, 1.0), 3.0) * step(0.982 - 0.02 * highP() * ar, h12(floor(q * 22.0) + floor(TIME * 14.0) + sd * 97.0));
    gl *= 0.35 + 2.0 * highP() * ar + 0.6 * gPeak;
    // drifting debris
    vec2 dg = (sp + vec2(gT * 0.02, gT * 0.009) * drift) * 26.0 + form * 7.0;
    vec2 di = floor(dg);
    vec2 dfp = fract(dg) - 0.5 - (vec2(h12(di + 1.3), h12(di + 2.7)) - 0.5) * 0.6;
    float speck = step(0.93, h12(di + 3.1)) * exp(-dot(dfp, dfp) * 90.0) * (1.0 - S.w);
    vec3 col;
    float alpha;
    float rr = length(sp * vec2(0.82, 1.0));
    if (bright) {
        float stop = 1.0 - smoothstep(0.62, 1.0, rr) * 0.55;
        vec3 base = vec3(0.96, 0.91, 0.79) * stop * (0.94 + 0.06 * gLift);
        vec3 absorb = vec3(0.22, 0.52, 1.05);
        float T = S.x * 0.45 * (1.0 - 0.7 * pm) + S.y * 0.85 + S.w * 0.18 * (1.0 - pm);
        vec3 under = mix(base, pc * mix(1.0, stop, 0.5) + base * 0.06, pm);
        col = under * exp(-absorb * T * 1.7);
        col *= mix(vec3(1.0), 0.8 + 0.4 * ir, 0.3 * irA * S.w * (1.0 - pm));
        col = mix(col, vec3(1.0, 0.98, 0.92) * stop, S.z * 0.35 * (1.0 - pm));   // pores let light through
        col -= vec3(0.35, 0.42, 0.5) * gl * 0.4;
        col *= 1.0 - speck * 0.5;
        alpha = clamp(max(T, pm), 0.0, 1.0);
    } else {
        vec3 edgeC = mix(vec3(0.85, 0.94, 1.0), ir * 1.15, irA * 0.6);
        if (form > 2.5) edgeC = mix(edgeC, vec3(0.6, 0.86, 1.0), 0.25);
        vec3 bodyC = mix(vec3(0.3, 0.42, 0.52), ir, irA * 0.45);
        vec3 add = edgeC * S.y * glow * hiG * 0.85 + bodyC * S.x * 0.14 * glow * (1.0 - pm);
        add = add / (1.0 + 0.35 * add);
        add += vec3(1.0, 0.97, 0.9) * gl;
        add += vec3(0.5, 0.6, 0.7) * speck * 0.35;
        vec3 bg = backdrop(uv) + vec3(0.008, 0.016, 0.03) * (1.0 - smoothstep(0.0, 1.0, rr)) * (1.0 + gLift);
        if (form > 1.5 && form < 2.5) add += vec3(0.25, 0.32, 0.4) * exp(-dot(sp, sp) * 12.0) * 0.08 * (1.0 + gPeak);
        col = mix(bg, pc, pm) + add;
        alpha = clamp(max(lum(add) * 2.0 + S.w * 0.3, pm), 0.0, 1.0);
    }
    return vec4(col, alpha);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    gT = TIME * 0.5 + (audioMidTime * 0.25 + audioBassTime * 0.15) * ar + 1.5 * gBuild * gBuild;
    gSpin = gT * spinSpeed + 2.0 * gBuild * gBuild;
    gShim = TIME * 1.6 * poreShimmer + audioHighTime * 1.2 * ar + 4.0 * gBuild * gBuild;
    gPoreK = 1.0 - 0.75 * patternAmt;
    gSlide = TIME * 2.0 + audioMidTime * 0.8 * ar + 3.0 * gBuild * gBuild;
    gSpecG = (1.0 + 1.3 * highP() * ar + 0.3 * gPeak - 0.3 * gRest) * (1.0 + gBuild * gBuild * (0.5 + 0.5 * sin(TIME * 9.0 + gBuild * gBuild * 70.0)));
    gBr = 1.0 + pulseAmt * (0.05 * sin(TIME * 0.6) + 0.07 * bassP() * ar) + 0.08 * gDrop;
    // build tremor near the top
    float trem = smoothstep(0.7, 1.0, gBuild);
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.006 * trem;

    float fsel = float(formSel);
    float f0, f1, wt;
    if (fsel > 0.5) { f0 = fsel - 1.0; f1 = f0; wt = 0.0; }
    else {
        float ph = TIME / morphPeriod;
        f0 = mod(floor(ph), 4.0);
        f1 = mod(f0 + 1.0, 4.0);
        wt = smoothstep(0.8, 1.0, fract(ph));
    }
    // iris wipe: the next slide opens from the centre
    float rad = length(sp);
    float edgeR = wt * 1.25;
    float m = wt > 0.0 ? 1.0 - smoothstep(edgeR - 0.07, edgeR, rad) : 0.0;
    vec4 acc = vec4(0.0);
    for (int k = 0; k < 2; k++) {
        float wk = k == 0 ? 1.0 - m : m;
        if (wk < 0.001) continue;
        acc += renderForm(k == 0 ? f0 : f1, sp, uv) * wk;
    }
    vec3 col = acc.rgb;
    float alpha = acc.a;
    if (wt > 0.0) col += vec3(0.55, 0.75, 1.0) * ridge(rad - edgeR + 0.035, 0.02) * 0.45 * sin(PI * wt);
    // drop: flash + shockwave ring
    col += vec3(0.7, 0.85, 1.0) * ridge(rad - (1.0 - gDrop) * 1.3, 0.03 + 0.05 * (1.0 - gDrop)) * gDrop * 0.6;
    col += vec3(1.0) * gDrop * gDrop * 0.12;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
