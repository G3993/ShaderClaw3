/*{
  "DESCRIPTION": "Light Spectrum — a sheet of thunder and a crystal line of light, mirrored like a Rorschach of glass. Branching orange lightning webs crawl and strike across the frame through drifting bokeh orbs. A column of translucent crystal sheets stands in the centre, their edges burning white and their faces filled with fine vertical fibres; light bursts out of a glowing core in radial filaments, and everything is split into a real spectrum — the scene is sampled at seven wavelength scales and recombined, so every edge throws violet-to-red prism fringes. Thin-film iridescence slides over the sheets, a lavender-and-peach haze breathes behind, and a feedback buffer smears the light outward into trails. The light keeps morphing through six forms: Thunder Sheet (a branching electric web), Crystal Column (vertical sheets), Prism Lens (mirrored lobes on a burning horizon), Facet Star (kaleidoscopic shards), Starburst (pure radial filaments) and Obsidian Mirror (dark mirrored glass relief with chromatic edge glints). Bass swells the core and spreads the sheets, beats flash the light and tear the image, mids wobble the sheets, highs sparkle the fibres. Rest: a few slim sheets glow softly in lavender haze with a dim, slow-crawling web and rare distant flickers; Build: the sheets draw together as the camera pushes in, strikes flicker faster and faster, dispersion and core glow climb and the image starts to tear and tremble; Drop: one massive white-hot strike — the whole web ignites, a spectral shockwave bursts from the core and the trails blast outward — settling into a wide, fully charged storm of sheets, filaments and lightning.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "Light", "Glitch", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Crystal Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6], "LABELS": ["Auto Morph", "Crystal Column", "Prism Lens", "Facet Star", "Starburst", "Thunder Sheet", "Obsidian Mirror"], "GROUP": "Crystal" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 11.0, "GROUP": "Crystal" },
    { "NAME": "symmetry",    "LABEL": "Symmetry", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["None", "Mirror Left/Right", "Mirror Four Ways"], "GROUP": "Crystal" },
    { "NAME": "sheetCount",  "LABEL": "Crystal Sheets",   "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 7.0, "DEFAULT": 5.0, "GROUP": "Crystal" },
    { "NAME": "sheetMorph",  "LABEL": "Sheet Morph",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Crystal" },
    { "NAME": "speed",       "LABEL": "Speed",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Crystal" },
    { "NAME": "thunder",     "LABEL": "Thunder (all forms)", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.6, "GROUP": "Thunder" },
    { "NAME": "thunderScale","LABEL": "Web Scale",        "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.2, "GROUP": "Thunder" },
    { "NAME": "strikeRate",  "LABEL": "Strike Flicker",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Thunder" },
    { "NAME": "thunderColor","LABEL": "Lightning Colour", "TYPE": "color", "DEFAULT": [1.0, 0.38, 0.08, 1.0], "GROUP": "Thunder" },
    { "NAME": "bokeh",       "LABEL": "Bokeh Orbs",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.6, "GROUP": "Thunder" },
    { "NAME": "dispersion",  "LABEL": "Prism Dispersion", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "iridescence", "LABEL": "Iridescence",      "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.7, "GROUP": "Light" },
    { "NAME": "streaks",     "LABEL": "Light Filaments",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "fibers",      "LABEL": "Fibre Detail",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "coreGlow",    "LABEL": "Core Glow",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "exposure",    "LABEL": "Exposure",         "TYPE": "float", "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "trails",      "LABEL": "Light Trails",     "TYPE": "float", "MIN": 0.0, "MAX": 0.97, "DEFAULT": 0.8, "GROUP": "Motion" },
    { "NAME": "trailZoom",   "LABEL": "Trail Expansion",  "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Motion" },
    { "NAME": "glitch",      "LABEL": "Glitch Tears",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Motion" },
    { "NAME": "haze",        "LABEL": "Haze",             "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.7, "GROUP": "Atmosphere" },
    { "NAME": "hazeA",       "LABEL": "Haze Lavender",    "TYPE": "color", "DEFAULT": [0.72, 0.5, 0.95, 1.0], "GROUP": "Atmosphere" },
    { "NAME": "hazeB",       "LABEL": "Haze Peach",       "TYPE": "color", "DEFAULT": [1.0, 0.62, 0.45, 1.0], "GROUP": "Atmosphere" },
    { "NAME": "crystalTint", "LABEL": "Crystal Tint",     "TYPE": "color", "DEFAULT": [0.82, 0.95, 1.0, 1.0], "GROUP": "Atmosphere" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Atmosphere" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Atmosphere" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.02, 0.01, 0.04, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
    { "TARGET": "trail", "PERSISTENT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Light Spectrum
//   pass 0  trail (persistent): light intensity field I(p) sampled at seven
//           wavelength scales → spectral colour; merged with the previous
//           frame pushed outward (light trails). Background is NOT in here,
//           only emitted light, so the user background composites cleanly.
//   pass 1  final: glow, haze, tonemap, background, grain.
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

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vn(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1, 0)), f.x), mix(h12(i + vec2(0, 1)), h12(i + vec2(1, 1)), f.x), f.y);
}
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
vec3 hueRot(vec3 c, float h) {
    if (h <= 0.0) return c;
    float a = h * TAU; vec3 k = vec3(0.57735); float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}

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

float gT;
float gW0, gW1, gW2, gW3, gW4, gW5;   // form weights: column, lens, facet, starburst, thunder, obsidian
float gSym;

// ── the light: a scalar intensity field (plus a film phase for iridescence) ──
float lightField(vec2 p, out float phase) {
    float x = p.x, y = p.y;
    if (gSym > 0.5) x = abs(x);
    if (gSym > 1.5) y = abs(y);
    vec2 q = vec2(x, y);
    float r = length(q);
    float a = atan(q.y, q.x);
    float I = 0.0;
    phase = 0.0;
    float ar = audioReact;
    float beat = audioBeatPulse * ar;

    // ── crystal column: translucent vertical sheets with burning edges ──
    if (gW0 + gW1 > 0.001) {
        // rest: fewer, slimmer sheets; build: they draw together; drop: they blast apart
        float n = max(floor(clamp(sheetCount, 1.0, 7.0) + 0.5) * mix(0.45, 1.0, smoothstep(0.15, 0.85, gLift)), 1.0);
        float wob = sheetMorph * (1.0 + 0.7 * midP() * ar) * mix(0.6, 1.2, gLift);
        float spread = (1.0 + 0.25 * bassP() * ar) * mix(0.8, 1.25, gLift) * (1.0 - 0.4 * gBuild) + 0.6 * gDrop;
        float acc = 0.0;
        for (int i = 0; i < 7; i++) {
            float fi = float(i);
            if (fi > ceil(n) - 0.5) break;
            float sw = clamp(n - fi, 0.0, 1.0);
            float hs = h11(fi * 3.7 + 1.0);
            float xc = (0.03 + fi * 0.055) * spread + 0.03 * wob * sin(y * (3.0 + 4.0 * hs) + gT * (0.8 + hs) + fi * 2.0);
            // lens form: sheets bow outward into mirrored lobes
            xc += gW1 * 0.28 * (1.0 - fi / 7.0) * smoothstep(0.75, 0.0, abs(abs(y) - 0.25)) * (0.6 + 0.4 * sin(gT * 0.5 + fi));
            float w = 0.012 + 0.02 * hs * (0.5 + 0.5 * sin(gT * 0.7 + fi * 1.3)) * wob;
            float d = abs(x - xc);
            float edge = exp(-max(d - w, 0.0) * 260.0) * (0.45 + 0.55 * hs);
            float face = smoothstep(w + 0.025, w, d) * 0.12;
            float len = smoothstep(0.7 + 0.25 * hs + 0.1 * sin(gT * 0.4 + fi), 0.1, abs(y));
            acc += (edge + face) * len * sw;
            phase += (face + edge * 0.5) * (fi * 0.17 + y * 0.6);
        }
        // fine vertical fibres inside the crystal
        float fib = pow(vn(vec2(x * 180.0, y * 2.5 - gT * 0.6)), 3.0) * fibers * (1.0 + 1.5 * highP() * ar);
        acc += fib * 0.3 * smoothstep(0.42, 0.0, x) * smoothstep(0.85, 0.2, abs(y));
        I += acc * (gW0 + gW1);
    }
    // ── prism lens: burning horizon line and lobe rims ──
    if (gW1 > 0.001) {
        float hz = exp(-abs(y) * 38.0) * smoothstep(0.9, 0.0, x) * 1.3;
        vec2 lq = vec2(x / 0.42, (abs(y) - 0.33) / 0.27);
        float rim = exp(-abs(length(lq) - 1.0) * 22.0) * 0.8;
        float shell = smoothstep(1.0, 0.6, length(lq)) * 0.18 * (0.6 + 0.4 * sin(abs(y) * 30.0 - gT * 2.0));
        I += (hz + rim + shell) * gW1;
        phase += (rim + shell) * (length(lq) * 1.3 + gT * 0.1) * gW1;
    }
    // ── facet star: kaleidoscopic shards ──
    if (gW2 > 0.001) {
        float N = 6.0;
        float seg = TAU / N;
        float af = abs(mod(a + gT * 0.05, seg) - 0.5 * seg);
        vec2 kq = r * vec2(cos(af), sin(af));
        float shard = 0.0;
        for (int i = 0; i < 4; i++) {
            float fi = float(i);
            float R0 = 0.12 + fi * 0.12 + 0.03 * sin(gT * 0.8 + fi * 1.7) * sheetMorph;
            float ang = 0.35 + 0.15 * fi + 0.1 * sin(gT * 0.5 + fi);
            float dl = abs(dot(kq, vec2(cos(ang), sin(ang))) - R0);
            shard += exp(-dl * 160.0) * (0.7 - fi * 0.12);
            phase += exp(-dl * 40.0) * (fi * 0.25 + r);
        }
        float facetFill = smoothstep(0.55, 0.0, r) * 0.12 * (0.5 + 0.5 * sin(af * 40.0 + r * 20.0 - gT));
        I += (shard + facetFill) * gW2;
    }
    // ── obsidian mirror: dark glassy cells, light only on their edges ──
    if (gW5 > 0.001) {
        vec2 op = q * 6.5 + vec2(0.0, gT * 0.12);
        vec2 oi = floor(op), of = fract(op);
        float f1 = 9.0, f2 = 9.0;
        for (int j = -1; j <= 1; j++)
        for (int k = -1; k <= 1; k++) {
            vec2 o = vec2(float(k), float(j));
            vec2 pt = o + 0.5 + 0.4 * sin(gT * 0.4 + TAU * vec2(h12(oi + o), h12(oi + o + 3.1)));
            float dd = length(pt - of);
            if (dd < f1) { f2 = f1; f1 = dd; } else if (dd < f2) f2 = dd;
        }
        float e = f2 - f1;
        float glint = exp(-e * 30.0) * (0.45 + 0.55 * sin(e * 24.0 + gT + f1 * 6.0));
        // glassy cell bodies: smoky grey domes, brighter toward one side
        float dome = sqrt(max(1.0 - f1 * 1.6, 0.0));
        float relief = 0.12 * dome + 0.08 * smoothstep(0.2, 0.0, abs(f1 - 0.35));
        I += (glint * 0.9 + relief) * gW5;
        phase += e * 4.0 * gW5;
    }
    // ── radial filaments from the core (all forms, strongest in starburst) ──
    float fil = pow(vn(vec2(a * 55.0 / PI, r * 1.8 - gT * 0.9)), 4.0) + 0.6 * pow(vn(vec2(a * 140.0 / PI, r * 3.0 - gT * 1.3)), 6.0);
    float filEnv = smoothstep(0.02, 0.25, r) * exp(-r * mix(2.6, 1.3, gLift));
    I += fil * filEnv * streaks * ((0.25 * (1.0 - gW5 - gW4 * 0.6) + 0.8 * gW3) * mix(0.6, 1.3, gLift) + 0.4 * beat + 0.5 * gBuild + 1.6 * gDrop);
    // drop: a spectral shockwave ring bursting out of the core (gets prism-split like everything else)
    float shR = (1.0 - gDrop) * 1.3;
    I += gDrop * 1.6 * exp(-pow((r - shR) / (0.015 + 0.05 * (1.0 - gDrop)), 2.0));
    // ── core ──
    float core = exp(-r * r * 90.0) * 0.9 + exp(-r * 12.0) * 0.18;
    I += core * coreGlow * (1.0 - 0.7 * gW5) * (1.0 + 0.8 * bassP() * ar + 1.2 * beat) * (mix(0.55, 1.0, gLift) + 0.7 * gBuild * gBuild + 1.0 * gDrop);
    phase += r * 2.0;
    return I;
}

// spectral colour of wavelength index k in [0,1]: violet → red
vec3 spectrum(float k) {
    // violet, blue, cyan, green, yellow, orange, red
    vec3 c = clamp(vec3(abs(k * 6.0 - 4.5) - 1.0, 2.0 - abs(k * 6.0 - 3.0), 2.0 - abs(k * 6.0 - 1.5)), 0.0, 1.0);
    c.r += 0.5 * smoothstep(0.25, 0.0, k);      // violet end carries some red
    return c;
}
vec3 film(float t) { return 0.5 + 0.5 * cos(TAU * (t + vec3(0.0, 0.33, 0.67))); }

// ── thunder: a ridged fractal web of electric veins ──────────────────────
float ridgeN(vec2 p) { return 1.0 - abs(vn(p) * 2.0 - 1.0); }
float thunderWeb(vec2 p, float t) {
    vec2 x = p;
    if (gSym > 0.5) x.x = abs(x.x);
    if (gSym > 1.5) x.y = abs(x.y);
    vec2 w = x * 2.2 * thunderScale;
    w += 0.55 * vec2(vn(w * 0.9 + t * 0.25), vn(w * 0.9 - t * 0.21 + 5.0)) - 0.27;
    float v = 0.0, amp = 0.65;
    for (int i = 0; i < 5; i++) {
        v += amp * pow(ridgeN(w), 7.0);
        w = w * 2.07 + vec2(1.7, -2.3) + t * 0.07;
        amp *= 0.6;
    }
    // where the sheet is lit: a drifting band of storm
    float sheet = smoothstep(0.12, 0.55, vn(x * 0.9 + vec2(t * 0.06, -t * 0.04)) * 0.7 + 0.3 * vn(x * 2.3 - t * 0.1));
    return v * sheet;
}

vec4 trailPass() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    float beat = audioBeatPulse * ar;

    // glitch: horizontal tears + vertical slip, more on beats
    float gl = glitch * (0.2 + 1.6 * beat) * mix(0.25, 1.2, gLift) + 0.6 * smoothstep(0.7, 1.0, gBuild) + 1.2 * gDrop;
    float row = floor(uv.y * 60.0);
    float tear = step(1.0 - 0.15 * gl, h12(vec2(row, floor(TIME * 10.0))));
    vec2 fcg = fc + vec2(tear * (h12(vec2(row * 1.7, floor(TIME * 10.0))) - 0.5) * R.x * 0.08 * gl, 0.0);

    vec2 p = (fcg - 0.5 * R) / R.y;
    // build: camera pushes in with a tremor near the top; drop: a scale punch
    p += (vec2(h11(floor(TIME * 24.0)), h11(floor(TIME * 24.0) + 7.3)) - 0.5) * 0.008 * smoothstep(0.65, 1.0, gBuild);
    p *= (1.0 - 0.2 * gBuild) * (1.0 - 0.12 * gDrop);
    float r = length(p);

    // spectral sampling: the same light field at seven wavelength scales
    float disp = dispersion * (0.03 + 0.09 * r) * (1.0 + 0.6 * beat) * mix(0.6, 1.2, gLift) * (1.0 + 0.7 * gBuild + 2.0 * gDrop);
    vec3 col = vec3(0.0);
    vec3 wsum = vec3(0.0);
    float phase = 0.0;
    for (int k = 0; k < 7; k++) {
        float fk = float(k) / 6.0;
        float s = 1.0 + (fk - 0.5) * disp * 2.0;
        float ph;
        float I = lightField(p * s, ph);
        vec3 sc = spectrum(fk);
        col += I * sc;
        wsum += sc;
        if (k == 3) phase = ph;
    }
    col /= wsum;
    // crystal tint + thin-film iridescence riding on the structure
    col *= crystalTint.rgb;
    col = mix(col, col * film(phase * 0.8 + gT * 0.05) * 1.5, iridescence * 0.5);

    // ── thunder sheet: three samples for a chromatic fringe ──
    float th = thunder * ((0.5 + 1.1 * gW4) * mix(0.15, 1.0, gLift) + 0.4 * gBuild);
    if (th > 0.001) {
        float tt = gT * 0.8;
        float cd = 0.006 * (dispersion + 0.3);
        float tr = thunderWeb(p * (1.0 + cd), tt);
        float tg = thunderWeb(p, tt);
        float tb = thunderWeb(p * (1.0 - cd), tt);
        // strikes: random flashes + every beat
        // strike clock: additive, and it races ahead (bounded) as the build climbs
        float sclk = TIME * (6.0 + 10.0 * strikeRate) + 60.0 * gBuild * gBuild * gBuild;
        float fl = h11(floor(sclk));
        float sp = clamp(0.35 * strikeRate * mix(0.35, 1.4, gLift) + 0.35 * gBuild, 0.0, 0.9);
        float strike = mix(0.35, 0.6, gLift) + 0.9 * step(1.0 - sp, fl) * fl + 1.6 * audioBeatPulse * ar * mix(0.4, 1.0, gLift) + 0.4 * bassP() * ar + 2.5 * gDrop;
        vec3 tc = vec3(tr, tg, tb) * strike;
        vec3 bolt = thunderColor.rgb * tc * 3.2 + vec3(1.0, 0.92, 0.8) * pow(max(tg * strike - 0.35, 0.0), 1.3) * 2.5;
        col = col * (1.0 - 0.35 * gW4) + bolt * th;
    }
    // ── bokeh orbs drifting in front ──
    if (bokeh > 0.001) {
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            vec2 c = vec2(h11(fi * 3.3) - 0.5, h11(fi * 7.1) - 0.5) * vec2(R.x / R.y, 1.0) * 1.1;
            c += 0.08 * vec2(sin(TIME * 0.2 + fi * 2.0), cos(TIME * 0.17 + fi * 1.3));
            c.y = mod(c.y + TIME * 0.015 * (0.5 + h11(fi)) + 0.55, 1.1) - 0.55;
            float rad = 0.012 + 0.04 * h11(fi * 5.7);
            float d = length(p - c) / rad;
            float orb = smoothstep(1.0, 0.85, d) * (0.35 + 0.65 * smoothstep(0.6, 0.95, d));
            vec3 oc = mix(vec3(0.85, 0.85, 0.95), thunderColor.rgb * 1.2 + 0.2, step(0.7, h11(fi * 9.1)));
            col += oc * orb * bokeh * (0.35 + 0.4 * h11(fi * 2.2)) * (1.0 + 0.6 * highP() * ar) * mix(0.5, 1.2, gLift);
        }
    }

    // feedback: previous light pushed outward (trails / bloom smear)
    float z = 1.0 - 0.003 * trailZoom * (1.0 + 0.8 * beat) - 0.02 * gDrop;   // drop blasts the trails outward
    vec2 puv = (uv - 0.5) * z + 0.5;
    vec3 prev = texture2D(trail, puv).rgb;
    if (FRAMEINDEX < 2) prev = vec3(0.0);
    vec3 outc = max(col, prev * min(trails * (1.0 + 0.08 * gRest), 0.97) * 0.92);
    return vec4(min(outc, vec3(8.0)), 1.0);
}

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

vec4 finalPass() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    vec3 L = texture2D(trail, uv).rgb;
    // soft glow: two rings of taps
    vec3 g = vec3(0.0);
    for (int i = 0; i < 8; i++) {
        float a = float(i) * TAU / 8.0 + 0.3;
        vec2 o = vec2(cos(a), sin(a));
        g += texture2D(trail, uv + o * 6.0 / R).rgb + texture2D(trail, uv + o * 18.0 / R).rgb * 0.6;
    }
    g /= 12.8;
    vec3 light = L + g * 0.45;

    // haze: lavender / peach clouds that the light sits in
    vec2 p = (fc - 0.5 * R) / R.y;
    vec2 hp = vec2(gSym > 0.5 ? abs(p.x) : p.x, p.y);
    float cl = vn(hp * 3.0 + vec2(0.0, TIME * 0.05)) * 0.6 + vn(hp * 7.0 - TIME * 0.08) * 0.4;
    vec3 hz = mix(hazeA.rgb, hazeB.rgb, smoothstep(0.35, 0.75, vn(hp * 2.0 + 5.0 + TIME * 0.03)));
    float hazeAmt = haze * smoothstep(0.45, 0.9, cl) * smoothstep(1.2, 0.2, length(p)) * 0.4 * (1.0 - 0.7 * gW5) * mix(1.35, 0.85, gLift);
    light += hz * hazeAmt * (1.0 + 0.3 * bassP() * ar);

    light *= exposure * (1.0 + 0.15 * audioLevel * ar) * (mix(0.85, 1.0, gLift) + 0.1 * gBuild + 0.25 * gDrop);
    vec3 col = 1.0 - exp(-light * 0.95);
    col = hueRot(col, hueShift);
    col = mix(vec3(lum(col)), col, saturation * 1.1 * mix(0.8, 1.15, gLift));
    float cov = clamp(lum(col) * 1.6, 0.0, 1.0);
    vec3 bg = backdrop(uv);
    col = bg * (1.0 - cov) + col;
    col *= 1.0 - 0.12 * smoothstep(0.45, 1.0, length((uv - 0.5) * vec2(R.x / R.y, 1.0)));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.07;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    return vec4(clamp(col, 0.0, 1.0), transparentBg ? cov : 1.0);
}

void main() {
    showDynamics();
    // shared state for both passes
    gT = TIME * speed + 0.4 * audioBassTime * audioReact * speed + 0.25 * audioMidTime * audioReact * speed
       + 2.0 * gBuild * gBuild * speed;
    gSym = float(symmetry);
    float sel = float(formSel);
    gW0 = 0.0; gW1 = 0.0; gW2 = 0.0; gW3 = 0.0; gW4 = 0.0; gW5 = 0.0;
    float fa, fb, f;
    if (sel < 0.5) {
        float ph = TIME / morphPeriod;
        fa = mod(floor(ph) + 4.0, 6.0); fb = mod(fa + 1.0, 6.0);
        f = smoothstep(0.55, 1.0, fract(ph));
    } else { fa = sel - 1.0; fb = fa; f = 0.0; }
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        float w = (abs(fa - fi) < 0.5 ? 1.0 - f : 0.0) + (abs(fb - fi) < 0.5 ? f : 0.0);
        if (i == 0) gW0 = w; else if (i == 1) gW1 = w; else if (i == 2) gW2 = w; else if (i == 3) gW3 = w; else if (i == 4) gW4 = w; else gW5 = w;
    }
    if (gW5 > 0.5) gSym = max(gSym, 2.0);
    // the lens form mirrors four ways on its own
    if (gW1 > 0.5) gSym = max(gSym, 2.0);

    if (PASSINDEX == 0) gl_FragColor = trailPass();
    else                gl_FragColor = finalPass();
}
