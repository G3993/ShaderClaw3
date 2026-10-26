/*{
  "DESCRIPTION": "Polarized — chemistry crystals seen through a polarizing light microscope. Between crossed polarizers the slide is black, and every birefringent crystal glows in Michel-Lévy interference colours: first-order greys and straw gold, then magenta, indigo, teal, lime and rose bands as the crystal thickens. Crystals nucleate and grow on the slide, impinging on each other along curved grain boundaries, then slowly dissolve and regrow somewhere new. Four species: Spherulites, round fibrous radial crystals with concentric colour bands and the dark Maltese-cross extinction that stays locked to the polarizer; Crystal Grains, faceted hexagonal plates and laths with sharp edges, rainbow wedge fringes on their rims and striped twins; Needle Sprays, bow-tie sheaves of acicular needles fanning out from a point, each needle its own colour; and Fern Dendrites, four- and six-armed fern crystals with combs of side branches, each arm glowing its own interference colour. Inside every crystal sits its own micrograph texture at its own orientation (Crystal Pattern, Mixed by default): banded spherulites whose twisted lamellae ring the Maltese cross with extinction bands, feathery fern dendrites with barbs and fine barbules, polysynthetic twin lamellae and spindle tartan, oscillatory zoning in nested polygons with dark growth lines, hourglass sector zoning, undulose extinction sweeping through deformation bands, radiating sheaves and bow-ties of needles, cleavage crack sets, fluid-inclusion trails of dark high-relief bubbles, and thin-film contour fringes like Newton's rings. Every pattern is drawn as retardation and slow-axis changes, so it glows through the true interference-colour ramp and its extinction bands swing as the polarizer and stage turn; all fine detail is filtered to the pixel for crisp, moiré-free lines, and a glassy relief catches a soft sheen and specular glints on domed rims, ridges and facets. Auto Morph lets each new generation of crystals nucleate as the next species, so the slide mixes and changes over. The analyser rotates over time so colours shift and grains blink in and out of extinction, while the stage turns slowly. Bass rotates the polarizer, every beat nucleates a new crystal with a white seed flash, highs throw glints along grain edges and growth fronts, and the drop snaps the polarizer round and floods the field with a full high-order rainbow. At rest the slide is sparse, slow and soft first-order gold and grey; during a build crystals grow faster, the polarizer spins up and the stage tremors; at peak the slide is packed with vivid high-order colour.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Science", "Organic", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Crystal Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Spherulites", "Crystal Grains", "Needle Sprays", "Fern Dendrites"], "GROUP": "Crystals" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 6.0, "MAX": 60.0, "DEFAULT": 16.0, "GROUP": "Crystals" },
    { "NAME": "crystalSize", "LABEL": "Crystal Size",     "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Crystals" },
    { "NAME": "density",     "LABEL": "Density",          "TYPE": "float", "MIN": 0.1, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Crystals" },
    { "NAME": "growSpeed",   "LABEL": "Growth Speed",     "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Crystals" },
    { "NAME": "patternSel",  "LABEL": "Crystal Pattern", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], "LABELS": ["Mixed", "Banded Spherulite", "Feathery Dendrite", "Twin Lamellae", "Oscillatory Zoning", "Sector Zoning", "Undulose Extinction", "Radiating Sheaf", "Cleavage Sets", "Fluid Inclusions", "Thin-Film Fringes"], "GROUP": "Crystals" },
    { "NAME": "patternAmt",  "LABEL": "Pattern Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Crystals" },
    { "NAME": "patternDetail","LABEL": "Pattern Detail",  "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Crystals" },
    { "NAME": "biref",       "LABEL": "Birefringence",   "TYPE": "float", "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "polSpin",     "LABEL": "Analyser Rotation","TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "stageSpin",   "LABEL": "Stage Rotation",   "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "saturation",  "LABEL": "Colour Intensity", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "fieldStop",   "LABEL": "Microscope Vignette","TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Optics" },
    { "NAME": "beatSeeds",   "LABEL": "Beat Nucleation",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Audio Reactivity" },
    { "NAME": "sparkle",     "LABEL": "Edge Sparkle",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Audio Reactivity" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 3, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.03, 0.04, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
    { "TARGET": "nucState", "PERSISTENT": true, "FLOAT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718
#define NSLOT 6.0

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vn2(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}
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

// ── Michel-Lévy interference colour ─────────────────────────────────────
// Light through polarizer (angle pa) → birefringent plate (slow axis psi,
// retardation G nm) → analyser (angle pb). Integrated over the visible band.
vec3 polColor(float G, float psi, float pa, float pb) {
    float A = cos(pa - pb); A *= A;
    float B = sin(2.0 * (psi - pa)) * sin(2.0 * (psi - pb));
    vec3 acc = vec3(0.0), wsum = vec3(0.0);
    for (int i = 0; i < 10; i++) {
        float lam = 400.0 + 33.3 * float(i);
        vec3 w = vec3(exp(-pow((lam - 602.0) / 42.0, 2.0)) + 0.28 * exp(-pow((lam - 438.0) / 22.0, 2.0)),
                      exp(-pow((lam - 545.0) / 42.0, 2.0)),
                      exp(-pow((lam - 452.0) / 30.0, 2.0)));
        float s = sin(PI * G / lam);
        acc += w * max(A - B * s * s, 0.0);
        wsum += w;
    }
    return acc / wsum;
}

// growth metric of a crystal (cell units, crystal frame): circle / facets / needles
float crystalMetric(float form, vec2 l, float h) {
    float d = length(l);
    if (form < 0.5 || form > 2.5) return d;
    if (form < 1.5) {                                     // faceted plates & laths
        vec2 k = vec2(l.x * mix(0.5, 1.0, h), l.y);
        float m = abs(k.y);
        m = max(m, abs(dot(k, vec2(0.866, 0.5))));
        m = max(m, abs(dot(k, vec2(0.866, -0.5))));
        return m * 1.12;
    }
    float a = atan(l.y, l.x);                             // needle sprays: jagged sheaf
    float bi = floor((a / TAU + 0.5) * 64.0);
    float len = 0.5 + 0.5 * h12(vec2(bi, h * 91.0));
    float sheaf = 0.45 + 0.55 * pow(abs(cos(a)), 0.7);
    return d / (len * sheaf);
}

// interior look: retardation G (nm, before thickness taper), slow axis psi (crystal frame), mask
void crystalLook(float form, vec2 l, float h, float Rc, float pxc, float T, out float G, out float psi, out float mask) {
    float d = length(l);
    float a = atan(l.y, l.x);
    vec2 dir = d > 1e-4 ? l / d : vec2(1.0, 0.0);
    mask = 1.0;
    if (form < 0.5) {                                     // Spherulite: radial fibres → Maltese cross
        float fib  = vn2(dir * 22.0 + h * 50.0);
        float fib2 = vn2(dir * 70.0 + vec2(d * 1.5, h * 13.0));
        float band = cos(d * TAU * (1.6 + 1.6 * h) - h * 6.0);
        G = 330.0 + 850.0 * h + 160.0 * band + 140.0 * (fib - 0.5) + 50.0 * (fib2 - 0.5);
        psi = a + 0.08 * (fib - 0.5);
        mask = 0.88 + 0.2 * fib2;
    } else if (form < 1.5) {                              // Crystal grain: single orientation, twins, cleavage
        float zm = crystalMetric(1.0, l, h);
        G = 380.0 + 1100.0 * h12(vec2(h, 7.1)) + 90.0 * l.y + 55.0 * sin(zm * TAU * (2.0 + 2.5 * h)) * step(0.35, h);
        psi = 0.18 * l.x;                                 // undulose extinction
        if (h > 0.72) {
            float tw = step(0.5, fract(l.y * mix(2.0, 6.0, h) + 0.2 * vn2(l * 4.0 + h * 20.0)));
            psi += tw * (0.5 + 0.4 * h);
            G += tw * 140.0;
        }
        float clv = smoothstep(0.96, 1.0, abs(sin(l.x * 26.0 + 3.0 * vn2(l * 5.0 + h * 9.0))));
        mask = 1.0 - 0.45 * clv * step(0.6, h);
    } else if (form < 2.5) {                              // Needle spray
        float N = 64.0;
        float u = (a / TAU + 0.5) * N;
        float bi = floor(u);
        float hb = h12(vec2(bi, h * 91.0));
        float len = (0.5 + 0.5 * hb) * (0.45 + 0.55 * pow(abs(cos(a)), 0.7)) * Rc;
        float off = (fract(u) - 0.5 - (h12(vec2(bi, 3.3 + h)) - 0.5) * 0.5) * TAU / N * d;
        float w = 0.028 * (1.0 - 0.75 * clamp(d / max(len, 1e-3), 0.0, 1.0)) + pxc;
        float nd = smoothstep(w, w - 1.5 * pxc, abs(off));
        float core = smoothstep(0.14, 0.05, d);
        mask = max(nd, core);
        psi = ((bi + 0.5) / N - 0.5) * TAU + 0.1 * (hb - 0.5);
        G = 320.0 + 1350.0 * hb + 260.0 * d;
    } else {                                              // Fern dendrite
        float S = h > 0.5 ? 6.0 : 4.0;
        float sec = TAU / S;
        float sa = mod(a + 0.5 * sec, sec) - 0.5 * sec;
        float armA = a - sa;
        float x = d * cos(sa), y = abs(d * sin(sa));
        float wA = 0.055 * (1.0 - 0.7 * x / max(Rc, 0.05)) + 0.01;
        float spine = smoothstep(wA, wA - 1.5 * pxc, y) * step(x, Rc);
        float be = S > 5.0 ? PI / 3.0 : PI / 4.0;
        float sb = sin(be), cb = cos(be);
        float sp = 0.085 + 0.03 * h;
        float x0 = x - y * cb / sb;                       // where this point's side branch leaves the spine
        float bl = y / sb;                                // distance along the branch
        float kk = floor(x0 / sp + 0.5);
        float dist = abs(x0 - kk * sp) * sb;
        float Lb = max(Rc - kk * sp, 0.0) * (0.5 + 0.2 * h12(vec2(kk, h * 17.0)));
        float wB = 0.032 * (1.0 - 0.8 * clamp(bl / max(Lb, 1e-3), 0.0, 1.0)) + 0.005;
        float br = smoothstep(wB, wB - 1.5 * pxc, dist) * step(bl, Lb) * step(0.5, kk);
        mask = max(spine, br);
        float thick = spine > br ? 1.0 - y / wA : 1.0 - dist / wB;
        float armId = floor((a + 0.5 * sec) / sec);
        G = 420.0 + 850.0 * h12(vec2(armId, h * 31.0)) + 160.0 * clamp(thick, 0.0, 1.0) + (spine > br ? 120.0 : 0.0);
        psi = armA + (spine > br ? 0.0 : be * 0.35);
    }
}

// ── internal crystal patterns (micrograph textures) ─────────────────────
// All periodic detail is filtered against the pixel footprint px (cell units):
// stripes/lines fade to their mean before they can alias.
float aaF(float f, float px) { return 1.0 - smoothstep(0.16, 0.42, f * px); }
// box-filtered square wave (±1), u in periods, f = periods per cell unit
float sqF(float u, float f, float px) {
    float w = max(f * px, 1e-4);
    float i1 = 0.5 - abs(fract(u + 0.5 * w) - 0.5), i0 = 0.5 - abs(fract(u - 0.5 * w) - 0.5);
    return clamp((i1 - i0) / w, -1.0, 1.0);
}
// filtered line coverage: dist = distance to the nearest line, hw = half width, f = lines per unit
float lineF(float dist, float hw, float f, float px) {
    float hw2 = max(hw, 0.5 * px);
    float cov = clamp((hw2 - dist) / px + 0.5, 0.0, 1.0) * (hw / hw2);
    return mix(min(2.0 * hw * f, 1.0), cov, aaF(f, px));
}
// blend two optic axes (axes are mod PI → mix in doubled-angle space)
float mixAx(float a, float b, float t) {
    vec2 v = mix(vec2(cos(2.0 * a), sin(2.0 * a)), vec2(cos(2.0 * b), sin(2.0 * b)), t);
    return 0.5 * atan(v.y, v.x + 1e-6);
}
// polygon (N-gon) metric and signed angle inside its sector
float polyM(vec2 q, float N, out float sa, out float k) {
    float sec = TAU / N;
    float a = atan(q.y, q.x) + PI;
    k = floor(a / sec);
    sa = a - (k + 0.5) * sec;
    return length(q) * cos(sa) / cos(0.5 * sec);
}

// pt: 0 banded spherulite, 1 feathery dendrite, 2 twin lamellae, 3 oscillatory zoning,
//     4 sector zoning, 5 undulose extinction, 6 radiating sheaf, 7 cleavage sets,
//     8 fluid-inclusion trails, 9 thin-film (Newton) fringes
void crystalPattern(float pt, vec2 l, float h, float Rc, float px, float T,
                    inout float G, inout float psi, inout float mask, out float rel) {
    float D  = patternDetail;
    float h2 = h12(vec2(h * 53.1, 2.7)), h3 = h12(vec2(h * 17.9, 8.1));
    vec2 s = rot(TAU * h2) * l;
    float d = length(l);
    vec2 dir = d > 1e-4 ? l / d : vec2(1.0, 0.0);
    float a = atan(l.y, l.x);
    float dG = 0.0, dpsi = 0.0, wR = 0.0, psiR = 0.0, mm = 1.0;
    rel = 0.0;
    if (pt < 0.5) {                                       // banded spherulite: twisted radial lamellae
        float f = (4.0 + 5.0 * h3) * D;
        float fa = aaF(f, px);
        float ph = TAU * d * f + 1.2 * vn2(dir * 3.0 + h * 20.0);
        float tw = sin(ph);
        psiR = a + 0.6 * tw * fa; wR = 1.0;               // radial axes → Maltese cross, twist → extinction rings
        dG = (190.0 * tw + 70.0 * sin(2.0 * ph)) * fa;
        float K = 140.0 * D;
        dG += 110.0 * (vn2(dir * K + h * 37.0) - 0.5) * aaF(0.6 * K / max(d, 1e-3), px) * smoothstep(0.0, 0.08, d);
        rel = 0.05 / f * tw * fa;
    } else if (pt < 1.5) {                                // feathery fern dendrite
        float S = 3.0 + floor(h3 * 4.0);
        float sec = TAU / S;
        float a2 = a + TAU * h2;
        float sa = mod(a2 + 0.5 * sec, sec) - 0.5 * sec;
        float armA = a - sa;
        float x = d * cos(sa), y = d * sin(sa), yy = abs(y);
        float be = 0.62 + 0.25 * h3;
        float sb = sin(be), cbe = cos(be);
        float sp = 0.055 / D;
        float x0 = x - yy * cbe / sb;
        float kk = floor(x0 / sp + 0.5);
        float dist = abs(x0 - kk * sp) * sb;
        float bl = yy / sb;
        float Lb = max(0.95 - kk * sp, 0.0) * (0.42 + 0.25 * h12(vec2(kk, h * 17.0 + floor((a2 + 0.5 * sec) / sec))));
        float tb = clamp(bl / max(Lb, 1e-3), 0.0, 1.0);
        float barb = lineF(dist, 0.011 / D * (1.0 - 0.75 * tb), 1.0 / (sp * sb), px) * step(bl, Lb) * step(0.5, kk);
        float spine = lineF(yy, 0.018 / D * (1.0 - 0.6 * clamp(x, 0.0, 1.0)), 0.5, px) * step(0.0, x);
        // barbules: fine hairs along each barb
        float f2 = 3.2 / sp;
        float hair = (0.5 + 0.5 * sin(TAU * (bl + 0.4 * dist) * f2)) * exp(-dist / (0.22 * sp)) * step(bl, Lb * 1.15) * step(0.5, kk);
        hair = mix(0.25 * exp(-dist / (0.22 * sp)), hair, aaF(f2, px)) * step(bl, Lb * 1.15) * step(0.5, kk);
        float cov = max(max(spine, barb), 0.55 * hair);
        dG = 420.0 * cov - 150.0 + 140.0 * h12(vec2(kk, h));
        psiR = armA + (spine > barb ? 0.0 : sign(y) * be); wR = clamp(cov * 1.4, 0.0, 1.0);
        mm = mix(0.3, 1.0, cov);
        rel = 0.012 / D * (spine + 0.7 * barb + 0.3 * hair);
    } else if (pt < 2.5) {                                // polysynthetic twin lamellae (+ tartan cross-hatch)
        float f = (5.0 + 7.0 * h2) * D;
        float u = s.y * f + 0.45 * vn2(vec2(s.x * 1.5, s.y * 2.0) + h * 30.0) + 0.9 * vn2(vec2(s.y * f * 0.21, h * 9.0));
        float q = sqF(u, f * 1.2, px);
        float q2 = 0.0;
        if (h3 > 0.55) {                                  // pericline set → microcline tartan, spindle shaped
            float f2 = f * 0.9;
            float u2 = s.x * f2 + 0.4 * vn2(s * 2.3 - h * 11.0);
            float lens = 0.5 + 0.5 * cos(TAU * (s.y * f * 0.5 + 0.3 * vn2(s * 4.0 + h)));   // spindle-shaped pericline lenses
            q2 = sin(TAU * u2) * lens * smoothstep(0.3, 0.75, vn2(s * 1.7 + h * 5.0)) * aaF(2.0 * f2, px);
        }
        dpsi = (0.3 + 0.25 * h2) * (q + 0.45 * q2);
        dG = 95.0 * q + 45.0 * q2 + 30.0;
        mm = 1.0 - 0.28 * (1.0 - abs(q)) * aaF(f, px);
        rel = 0.004 / D * (q + 0.5 * q2);
    } else if (pt < 3.5) {                                // oscillatory zoning: nested polygons
        float sa, kx;
        float N = 4.0 + floor(h3 * 4.0);
        float zm = polyM(s, N, sa, kx);
        float f = (6.0 + 6.0 * h2) * D;
        float u = zm * f + 0.5 * vn2(vec2(zm * 3.0, h * 40.0));
        float w = sin(TAU * u) * aaF(f, px) + 0.5 * sin(TAU * u * 2.31 + 1.3) * aaF(2.31 * f, px)
                + 0.35 * sin(TAU * u * 5.7 + h * 7.0) * aaF(5.7 * f, px);
        float gl = lineF(abs(fract(u + 0.5) - 0.5) / f, 0.0035, f, px);   // growth lines
        dG = 140.0 * w - 60.0 * gl;
        dpsi = 0.06 * w;
        mm = 1.0 - 0.45 * gl;
        rel = 0.05 / f * w - 0.004 * gl;
    } else if (pt < 4.5) {                                // sector (hourglass) zoning
        float sa, kx;
        float N = h3 > 0.5 ? 4.0 : 6.0;
        float zm = polyM(s, N, sa, kx);
        float sec = TAU / N;
        float distb = d * sin(0.5 * sec - abs(sa));
        float v = (mod(kx, 2.0) * 2.0 - 1.0) * clamp(distb / px * 1.5, 0.0, 1.0);
        float f = (10.0 + 8.0 * h2) * D;
        float rings = sin(TAU * zm * f + v * 1.6) * aaF(f, px);
        dpsi = 0.24 * v + 0.04 * rings;
        dG = 210.0 * v + 70.0 * rings + 40.0 * (h12(vec2(kx, h)) - 0.5);
        mm = 1.0 - 0.3 * lineF(distb, 0.003, 0.5, px);
        rel = -0.11 * zm + 0.025 / f * rings;              // low pyramid: each sector catches light differently
    } else if (pt < 5.5) {                                // undulose extinction + deformation bands
        float sw = 0.75 * sin(s.x * (1.2 + h3) + 1.6 * vn2(s * 1.3 + h * 11.0) + 0.12 * T);
        float f = (5.0 + 5.0 * h2) * D;
        float u = s.x * f + 0.9 * vn2(s * vec2(1.0, 3.0) * 2.0 + h * 3.0);
        float bid = floor(u), t = fract(u);
        float wb = clamp(f * px * 1.5, 0.04, 0.5);
        float o0 = h12(vec2(bid, h * 7.0)) - 0.5, o1 = h12(vec2(bid + 1.0, h * 7.0)) - 0.5;
        float off = mix(o0, o1, smoothstep(1.0 - wb, 1.0, t));
        dpsi = sw + 0.4 * off;
        dG = 120.0 * off + 40.0 * sw;
        rel = 0.012 * off;
    } else if (pt < 6.5) {                                // radiating sheaf / bow-tie of needles
        bool bow = h3 > 0.5;
        vec2 v = bow ? s : s - vec2(-0.8, 0.0);
        float ang = atan(v.y, bow ? abs(v.x) : v.x);
        float r = length(v);
        float Nr = (45.0 + 45.0 * h2) * D;                // needles per radian
        float u = ang * Nr;
        float f = Nr / max(r, 1e-3);
        float fa = aaF(f, px);
        float hn = h12(vec2(floor(u), h * 13.0));
        float gd = (0.5 - abs(fract(u) - 0.5)) / f;
        float gap = lineF(gd, 0.14 / f, f, px);
        float lenN = 0.35 + 0.9 * h12(vec2(floor(u), h * 3.0));
        float gen = step(lenN, r);                        // younger needles continue the sheaf
        psiR = (bow && v.x < 0.0) ? -ang : ang; wR = 1.0;
        dG = (300.0 * (hn - 0.5) + 160.0 * gen * (h12(vec2(floor(u), h * 5.0 + 2.0)) - 0.5)) * fa + 160.0 * r - 80.0;
        mm = 1.0 - 0.6 * gap;
        float cyl = 1.0 - pow(2.0 * fract(u) - 1.0, 2.0);
        rel = 0.35 / f * sqrt(max(cyl, 0.0)) * fa;
    } else if (pt < 7.5) {                                // cleavage line sets
        float ang2 = h3 < 0.33 ? 0.977 : (h3 < 0.66 ? 1.5708 : 1.309);
        float nset = h3 > 0.85 ? 3.0 : 2.0;
        if (h3 > 0.85) ang2 = 1.0472;
        float lines = 0.0, groove = 0.0;
        for (int i = 0; i < 3; i++) {
            float fi = float(i);
            if (fi >= nset) break;
            float th = fi * ang2;
            vec2 n = vec2(cos(th), sin(th));
            float f = (6.0 + 5.0 * h2) * D * (0.8 + 0.4 * h12(vec2(fi, h)));
            float u = dot(s, n) * f + 0.25 * vn2(s * 2.0 + fi * 7.0 + h * 9.0);
            float ln = floor(u + 0.5);
            float dist = abs(u - ln) / f;
            float along = dot(s, vec2(-n.y, n.x));
            float seg = smoothstep(0.35, 0.55, vn2(vec2(along * 2.5, ln * 3.7 + fi * 11.0 + h * 30.0)));
            lines = max(lines, lineF(dist, 0.0045, f, px) * seg);
            groove += exp(-pow(dist / 0.018, 2.0)) * seg * aaF(f, px);
            dG += 45.0 * (h12(vec2(ln, fi + h * 3.0)) - 0.5);   // tiny thickness steps between cracks
        }
        dG -= 160.0 * lines;
        mm = 1.0 - 0.8 * lines;
        rel = -0.012 * groove;
    } else if (pt < 8.5) {                                // fluid-inclusion trails (isotropic → dark, high relief)
        float inc = 0.0, dome = 0.0, trail = 0.0;
        for (int i = 0; i < 3; i++) {
            float fi = float(i);
            float th = TAU * h12(vec2(fi, h * 21.0));
            vec2 n = vec2(cos(th), sin(th)), pn = vec2(-n.y, n.x);
            float x = dot(s, n);
            float y = dot(s, pn) - (h12(vec2(fi, h * 9.0)) - 0.5) * 1.0 - 0.12 * sin(x * 3.0 + fi * 2.0 + h * 6.0);
            float spc = 0.065 / D;
            float k = floor(x / spc);
            float hk = h12(vec2(k, fi + h * 77.0));
            float xc = (k + 0.5 + 0.35 * (hk - 0.5)) * spc;
            float yc = (h12(vec2(k + 3.1, fi + h)) - 0.5) * 0.02;
            float r = spc * (0.1 + 0.32 * h12(vec2(k, fi * 3.0 + h))) * step(0.3, hk);
            vec2 q = vec2(x - xc, y - yc) * vec2(0.7, 1.0);
            float qd = length(q);
            float cov = clamp((r - qd) / px + 0.5, 0.0, 1.0);
            float vis = smoothstep(0.4 * px, 1.4 * px, r);
            cov = mix(min(r / px, 1.0) * exp(-qd * qd / (px * px)), cov, vis);   // sub-pixel dots → soft splat
            inc = max(inc, cov);
            dome += cov * sqrt(max(1.0 - qd * qd / max(r * r, 1e-6), 0.0)) * r * vis;
            trail = max(trail, lineF(abs(y), 0.0015, 0.5, px));
        }
        mm = 1.0 - 0.9 * clamp(inc, 0.0, 1.0) - 0.15 * trail;
        dG = -40.0 * trail;
        rel = -1.2 * dome;
    } else {                                              // thin-film / Newton contour fringes
        float tf = 0.7 * (1.0 - min(d * d / max(Rc * Rc, 0.01), 1.0)) + 0.55 * vn2(s * 1.6 + h * 20.0)
                 + 0.3 * vn2(s * 3.7 - h * 9.0) + 0.08 * sin(T * 0.4 + h * 6.0);
        float amp = 1150.0 * D;
        amp *= 1.0 - 0.8 * smoothstep(120.0, 320.0, 3.5 * amp * px);   // keep fringes ≥ ~3 px apart
        dG = amp * tf - 300.0;
        rel = 0.06 * tf;
    }
    float amt = patternAmt;
    float psiT = mixAx(psi + dpsi, psiR, wR);
    psi = mixAx(psi, psiT, amt);
    G = max(G + amt * dG, 0.0);
    mask *= mix(1.0, mm, amt);
    rel *= amt;
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    float Tg = (TIME * 0.8 + audioMidTime * 0.3 * ar + audioBassTime * 0.2 * ar) * growSpeed + 2.0 * gBuild * gBuild;

    // ── pass 0: nucleation state (6 beat-seeded crystal slots, one per column band)
    if (PASSINDEX == 0) {
        vec4 prev = texture2D(nucState, uv);
        float slot = floor(uv.x * NSLOT);
        float dt = clamp(TIMEDELTA, 0.0, 0.25);
        float cnt = prev.r, since = prev.g + dt, age = prev.b + dt * (0.8 + 0.6 * gLift), bp = prev.a;
        if (FRAMEINDEX < 2) {
            cnt = NSLOT - 1.0; since = 0.0; bp = 0.0;
            age = (NSLOT - 1.0 - slot) * 1.6 + 0.6;
        }
        float beat = audioBeatPulse * ar * beatSeeds;
        float trig = 0.0;
        if (beat > 0.55 && bp <= 0.55 && since > 0.22) trig = 1.0;
        float gap = mix(2.4, 5.5, gRest) - 1.0 * gPeak;
        if (since > gap && beatSeeds > 0.01) trig = 1.0;
        if (trig > 0.5) {
            cnt = mod(cnt + 1.0, 60.0);
            since = 0.0;
            if (abs(mod(cnt, NSLOT) - slot) < 0.5) age = 0.0;
        }
        gl_FragColor = vec4(cnt, min(since, 60.0), min(age, 60.0), beat);
        return;
    }

    // ── pass 1: the slide under crossed polarizers
    vec2 sp = (fc - 0.5 * R) / R.y;
    // build: field contracts and trembles near the top
    float trem = smoothstep(0.7, 1.0, gBuild);
    sp += 0.004 * trem * vec2(sin(TIME * 61.0), cos(TIME * 53.0));
    sp *= 1.0 + 0.25 * gBuild - 0.08 * gRest;
    float stageAng = (TIME * 0.025 + audioBassTime * 0.01 * ar) * stageSpin;
    vec2 st = rot(stageAng) * sp + vec2(0.13 * sin(TIME * 0.031), 0.09 * cos(TIME * 0.023));

    float cs = 0.27 * crystalSize;                        // crystal cell size (screen heights)
    float pxc = 1.0 / (R.y * cs);
    vec2 p = st / cs;
    float vg = 0.16;                                      // growth speed, cells per unit Tg
    float fsel = float(formSel);
    float dens = clamp(density * mix(1.0, 0.45, gRest) + 0.25 * gPeak, 0.0, 1.0);

    float best = -1e5, second = -1e5;
    float oForm = 0.0, oH = 0.0, oPhi = 0.0, oRc = 0.0, oFront = 0.0, oBeat = 0.0, oFade = 1.0;
    vec2 oL = vec2(0.0);
    vec2 cb = floor(p);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 c = cb + vec2(float(i), float(j));
        float hc = h12(c * 1.37 + 0.5);
        float P = (14.0 + 8.0 * hc);
        float ph = Tg + hc * P;
        float age = mod(ph, P);
        float cyc = floor(ph / P);
        float hh = h12(c + cyc * vec2(17.31, 9.17));
        if (hh > dens) continue;
        vec2 seed = c + 0.15 + 0.7 * vec2(h12(c * 3.1 + cyc * 1.7), h12(c * 2.3 - cyc * 2.9 + 4.0));
        float phi = TAU * h12(c * 5.7 + cyc * 0.37);
        float hk = h12(c * 7.9 + cyc * 3.3 + 1.0);
        float birth = Tg - age;
        float form = fsel > 0.5 ? fsel - 1.0 : mod(floor(birth / morphPeriod + 0.35 * hk), 4.0);
        float f = age / P;
        float Rc = min(vg * age, 1.0) * (1.0 - smoothstep(0.8, 0.96, f));
        vec2 l = rot(phi) * (p - seed);
        float m = crystalMetric(form, l, hk);
        if (m >= Rc) continue;
        float sc = age - m / vg;
        if (sc > best) {
            second = best; best = sc;
            oForm = form; oH = hk; oPhi = phi; oRc = Rc; oFront = Rc - m; oL = l; oBeat = 0.0;
            oFade = smoothstep(0.0, 0.06, f) * (1.0 - smoothstep(0.86, 0.96, f));
        } else if (sc > second) second = sc;
    }
    // beat-nucleated crystals sit on top (youngest wins)
    float flash = 0.0;
    float cnt = texture2D(nucState, vec2(0.5 / NSLOT, 0.5)).r;
    float bAge = 1e3;
    for (int k = 0; k < 6; k++) {
        float fk = float(k);
        float age = texture2D(nucState, vec2((fk + 0.5) / NSLOT, 0.5)).b;
        float id = cnt - mod(cnt - fk + 60.0, NSLOT);
        vec2 sd = (vec2(h11(id * 1.93 + 0.3) - 0.5, h11(id * 3.71 + 1.7) - 0.5) * vec2(1.5 * R.x / R.y, 0.85)) / cs;
        sd = rot(stageAng) * sd;                          // seeds land in view, then turn with the stage
        float phi = TAU * h11(id * 5.13 + 2.1);
        float hk = h11(id * 7.77 + 0.9);
        float form = fsel > 0.5 ? fsel - 1.0 : mod(floor((TIME * 0.8 - age) / morphPeriod + 0.35 * hk), 4.0);
        float life = 10.0;
        float Rc = 1.15 * (1.0 - exp(-age * 0.45)) * (1.0 - smoothstep(0.75 * life, life, age));
        vec2 rel = p - sd;
        flash += exp(-dot(rel, rel) * cs * cs * 1600.0) * exp(-age * 5.0) * 1.6
               + exp(-pow(length(rel) - age * 1.8, 2.0) * 220.0) * exp(-age * 4.0) * 0.5;
        vec2 l = rot(phi) * rel;
        float m = crystalMetric(form, l, hk);
        if (m < Rc && age < bAge && age < life) {
            bAge = age;
            oForm = form; oH = hk; oPhi = phi; oRc = Rc; oFront = Rc - m; oL = l; oBeat = 1.0;
            oFade = 1.0;
            best = 1e4; second = -1e5;
        }
    }

    vec3 bg = backdrop(uv);
    vec3 col = vec3(0.0);
    float alpha = 0.0;
    float edge = 0.0;
    if (best > -1e4) {
        float bdist = second > -1e4 ? (best - second) * vg * 0.5 : 1e3;
        float front = min(oFront, bdist);
        float G, psi, mask;
        crystalLook(oForm, oL, oH, oRc, pxc, Tg, G, psi, mask);
        // micrograph pattern: one call site, 3 taps (centre + 2 offsets give the relief gradient)
        float psel = float(patternSel);
        float ptype = psel > 0.5 ? psel - 1.0 : floor(h12(vec2(oH * 61.7, 3.9)) * 10.0);
        float eR = max(pxc, 0.004);
        float G0 = G, psi0 = psi, mask0 = mask, relC = 0.0;
        vec2 rg = vec2(0.0);
        for (int k = 0; k < 3; k++) {
            vec2 off = k == 1 ? vec2(eR, 0.0) : (k == 2 ? vec2(0.0, eR) : vec2(0.0));
            float g1 = G0, s1 = psi0, m1 = mask0, r1;
            crystalPattern(ptype, oL + off, oH, oRc, pxc, Tg, g1, s1, m1, r1);
            if (k == 0) { G = g1; psi = s1; mask = m1; relC = r1; }
            else if (k == 1) rg.x = (r1 - relC) / eR;
            else rg.y = (r1 - relC) / eR;
        }
        // thin edges: thickness tapers to zero at rims → rainbow wedge fringes
        float tap = smoothstep(0.0, oForm > 1.5 ? 0.12 : 0.22, front);
        G *= biref * mix(1.0, 0.5, gRest) * (1.0 + 0.15 * gPeak) * (1.0 + 0.8 * gDrop);
        G *= mix(0.12, 1.0, tap);
        // polarizer: steady rotation + bass, spins up in the build, snaps on the drop
        float pa = (TIME * 0.11 + audioBassTime * 0.22 * ar) * polSpin + 1.6 * gBuild * gBuild + 0.785 * gDrop;
        float eps = 0.06 * sin(TIME * 0.17) + 0.05 * midP() * ar + 0.35 * gDrop * gDrop;   // analyser near-crossed
        float pb = pa + 0.5 * PI + eps;
        float psiS = psi + oPhi - stageAng;               // slow axis in screen frame
        vec3 c = polColor(G, psiS, pa, pb);
        // drop: extinction lifts so the whole slide floods with rainbow
        vec3 cAll = polColor(G, pa + 0.785, pa, pa + 0.5 * PI);
        c = mix(c, max(c, cAll), 0.85 * gDrop);
        float sat = saturation * (1.05 + 0.25 * gPeak - 0.3 * gRest);
        c = max(mix(vec3(lum(c)), c, sat), 0.0);
        float aa = smoothstep(0.0, 1.5 * pxc, front);
        float m = mask * aa * oFade;
        col = c * m * (0.85 + 0.35 * gLift) * 1.15;
        // dark grain boundaries
        col *= smoothstep(0.0, 2.2 * pxc, bdist) * 0.7 + 0.3;
        // glassy relief: domed rims + pattern relief, lit from upper-left (screen frame)
        float ld = length(oL);
        vec2 dome = -(ld > 1e-4 ? oL / ld : vec2(0.0)) * 0.55 * (1.0 - smoothstep(0.0, 0.1, front));
        vec2 gs = rot(-(oPhi + stageAng)) * (rg + dome);
        vec3 nrm = normalize(vec3(-gs, 1.0));
        vec3 Ld = normalize(vec3(-0.5, 0.6, 0.65));
        vec3 Hv = normalize(Ld + vec3(0.0, 0.0, 1.0));
        float shade = clamp(1.0 + 0.7 * (dot(nrm, Ld) - Ld.z), 0.45, 1.6);
        col *= shade;
        float spec = pow(max(dot(nrm, Hv), 0.0), 48.0);
        float fres = pow(1.0 - nrm.z, 2.0);
        col += (vec3(1.0, 0.98, 0.95) * spec * 0.55 + (0.25 + c) * fres * 0.35) * aa * oFade * (0.7 + 0.3 * gLift);
        edge = (1.0 - smoothstep(0.0, 0.05, front)) * m;
        alpha = clamp(m * 1.5, 0.0, 1.0);
    }
    // highs: glints along growth fronts and grain edges
    vec2 gcell = floor(fc / 3.0);
    float glint = step(0.975 - 0.02 * gPeak, h12(gcell + floor(TIME * 14.0) * 7.31));
    col += vec3(1.0, 0.97, 0.92) * glint * edge * sparkle * (0.15 * gPeak + 0.9 * highP() * ar) * 1.2;
    // beat seed flash
    col += vec3(1.0, 0.96, 0.9) * flash * beatSeeds;

    // microscope field stop
    float vig = smoothstep(1.1, 0.55, length(sp * vec2(0.62, 1.0)));
    col *= mix(1.0, vig, fieldStop);

    col = bg * (1.0 - alpha) + col;
    col += vec3(1.0) * gDrop * gDrop * 0.06;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(alpha + lum(col), 0.0, 1.0) : 1.0);
}
