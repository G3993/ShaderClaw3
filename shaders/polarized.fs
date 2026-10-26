/*{
  "DESCRIPTION": "Polarized — chemistry crystals seen through a polarizing light microscope. Between crossed polarizers the slide is black, and every birefringent crystal glows in Michel-Lévy interference colours: first-order greys and straw gold, then magenta, indigo, teal, lime and rose bands as the crystal thickens. Crystals nucleate and grow on the slide, impinging on each other along curved grain boundaries, then slowly dissolve and regrow somewhere new. Four species: Spherulites, round fibrous radial crystals with concentric colour bands and the dark Maltese-cross extinction that stays locked to the polarizer; Crystal Grains, faceted hexagonal plates and laths with sharp edges, rainbow wedge fringes on their rims and striped twins; Needle Sprays, bow-tie sheaves of acicular needles fanning out from a point, each needle its own colour; and Fern Dendrites, four- and six-armed fern crystals with combs of side branches, each arm glowing its own interference colour. Auto Morph lets each new generation of crystals nucleate as the next species, so the slide mixes and changes over. The analyser rotates over time so colours shift and grains blink in and out of extinction, while the stage turns slowly. Bass rotates the polarizer, every beat nucleates a new crystal with a white seed flash, highs throw glints along grain edges and growth fronts, and the drop snaps the polarizer round and floods the field with a full high-order rainbow. At rest the slide is sparse, slow and soft first-order gold and grey; during a build crystals grow faster, the polarizer spins up and the stage tremors; at peak the slide is packed with vivid high-order colour.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Science", "Organic", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Crystal Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Spherulites", "Crystal Grains", "Needle Sprays", "Fern Dendrites"], "GROUP": "Crystals" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 6.0, "MAX": 60.0, "DEFAULT": 16.0, "GROUP": "Crystals" },
    { "NAME": "crystalSize", "LABEL": "Crystal Size",     "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Crystals" },
    { "NAME": "density",     "LABEL": "Density",          "TYPE": "float", "MIN": 0.1, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Crystals" },
    { "NAME": "growSpeed",   "LABEL": "Growth Speed",     "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Crystals" },
    { "NAME": "biref",       "LABEL": "Birefringence",    "TYPE": "float", "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "polSpin",     "LABEL": "Analyser Rotation","TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "stageSpin",   "LABEL": "Stage Rotation",   "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "saturation",  "LABEL": "Colour Intensity", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "fieldStop",   "LABEL": "Microscope Vignette","TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Optics" },
    { "NAME": "beatSeeds",   "LABEL": "Beat Nucleation",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Audio Reactivity" },
    { "NAME": "sparkle",     "LABEL": "Edge Sparkle",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Audio Reactivity" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
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
