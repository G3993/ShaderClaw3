/*{
  "DESCRIPTION": "Petri Dish — a top-down, time-lapse view into a polished chrome petri dish where living colonies of neon glass grow, collide and bloom. The dish is a mirror-bright spun-chrome bowl reflecting a dark studio — white softbox bars curving across the floor, a cool horizon glowing around the walls, brushed concentric rings stretching the highlights into anisotropic bow-ties — while cyan and magenta light bars sweep slowly over it. On top, every organism is translucent 3D glass: domed colonies with soap-bubble iridescence sliding from cyan to violet to hot pink as their curvature turns from the eye, refracting the chrome beneath, glowing from inside with neon growth rings and blazing neon rims, throwing rippling caustics and a focused lens spot onto the metal and pooling their colour into it as reflections. The forms morph each growth cycle: Bacteria Colonies, glass domes, branching fractal tubes pushing feathery DLA-like fronts and an oil-slick swarming film spreading in terraced rings, stopping at fine grooves where they meet; Mould Bloom, fibre-optic hyphae radiating from powdery neon sporulating centres studded with glass beads; Antibiotic Assay, an iridescent lawn film streaked over the chrome with frosted glass disks punching clean mirror halos that widen as it grows; and Bioluminescent, where the chrome goes dark and electric colonies pulse with quorum-sensing waves. A Finish selector swaps the whole look between Chrome Neon, Chrome Ice, Black Chrome and Holographic foil. Real bloom, shallow depth of field toward the rim and sparkling glints on the highs finish it in HD; when the dish is full a sterilising wipe dissolves back to bare chrome and growth begins again. Bass swells every colony and speeds the time-lapse, mids drive the light sweep, highs sparkle across the glass, beats seed new colonies with a bright pop, and the drop fires a neon shockwave that bursts colonies everywhere at once. At rest the dish is sparse, dim and slow-lit; during a build it tightens, grows faster, the lights race and it starts to tremble; on the drop a burst of new colonies erupts across the chrome, and the peak runs dense, bright, fully lit and fully alive.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Organic", "Science", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Culture", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Bacteria Colonies", "Mould Bloom", "Antibiotic Assay", "Bioluminescent"], "GROUP": "Culture" },
    { "NAME": "morphPeriod", "LABEL": "Growth Cycle (s)", "TYPE": "float", "AUDIO": false, "MIN": 10.0, "MAX": 90.0, "DEFAULT": 28.0, "GROUP": "Culture" },
    { "NAME": "growthRate",  "LABEL": "Growth Speed",     "TYPE": "float", "MIN": 0.4, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Culture" },
    { "NAME": "colonyCount", "LABEL": "Colony Count",     "TYPE": "float", "MIN": 0.2, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Culture" },
    { "NAME": "beatSpawn",   "LABEL": "Beat Spawning",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Culture" },
    { "NAME": "finishSel",   "LABEL": "Finish", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Chrome Neon", "Chrome Ice", "Black Chrome", "Holographic"], "GROUP": "Dish" },
    { "NAME": "dishSize",    "LABEL": "Dish Size",        "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 0.6, "DEFAULT": 0.42, "GROUP": "Dish" },
    { "NAME": "spinSpeed",   "LABEL": "Dish Rotation",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Dish" },
    { "NAME": "rimShine",    "LABEL": "Chrome Shine",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Dish" },
    { "NAME": "gloss",       "LABEL": "Glass Gloss",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Dish" },
    { "NAME": "iridescence", "LABEL": "Iridescence",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Dish" },
    { "NAME": "lightAmt",    "LABEL": "Neon Lights",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Dish" },
    { "NAME": "bloomAmt",    "LABEL": "Bloom",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.7, "GROUP": "Finish" },
    { "NAME": "dofAmt",      "LABEL": "Depth of Field",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Finish" },
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
    { "TARGET": "spawnBuf", "PERSISTENT": true, "FLOAT": true },
    { "TARGET": "dishBuf", "FLOAT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718
#define NSLOT 16.0

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float h13(vec3 p)  { p = fract(p * 0.1031); p += dot(p, p.zyx + 31.32); return fract((p.x + p.y) * p.z); }
float vn2(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}
float vn3(vec3 x) {
    vec3 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(h13(i), h13(i + vec3(1.0, 0.0, 0.0)), f.x), mix(h13(i + vec3(0.0, 1.0, 0.0)), h13(i + vec3(1.0, 1.0, 0.0)), f.x), f.y),
               mix(mix(h13(i + vec3(0.0, 0.0, 1.0)), h13(i + vec3(1.0, 0.0, 1.0)), f.x), mix(h13(i + vec3(0.0, 1.0, 1.0)), h13(i + vec3(1.0, 1.0, 1.0)), f.x), f.y), f.z);
}
float ridge(float n) { return 1.0 - abs(2.0 * n - 1.0); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
float mx3(vec3 c) { return max(c.r, max(c.g, c.b)); }
float tri(float t) { return 1.0 - abs(fract(t * 0.5) * 2.0 - 1.0); }

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

// ── growth clock: one cycle = one dish from bare chrome to full, then a wipe ──
float gP, gCyc, gFp, gA, gForm, gNext, gT;
void clocks() {
    float ar = audioReact;
    gP = morphPeriod;
    float clk = TIME + audioBassTime * 0.35 * ar + audioMidTime * 0.08 * ar + 1.5 * gBuild * gBuild;
    float ph = clk / gP;
    gCyc = floor(ph);
    gFp = fract(ph);
    gA = gFp * gP;
    float fs = float(formSel);
    gForm = fs > 0.5 ? fs - 1.0 : mod(gCyc, 4.0);
    gNext = fs > 0.5 ? fs - 1.0 : mod(gCyc + 1.0, 4.0);
    gT = TIME * 0.8 + audioMidTime * 0.3 * ar + audioHighTime * 0.15 * ar + 2.0 * gBuild * gBuild;
}

// ── finish (scheme) + neon palette — cool hues only: acid lime → cyan → blue → violet → magenta → hot pink ──
float gSch, gFilmK, gLI, gLa;
vec3 gLc1, gLc2;
vec3 neonCol(float s) {
    s = tri(s) * 5.0;
    vec3 a = vec3(0.3, 1.0, 0.26), b = vec3(0.0, 0.92, 1.0), c = vec3(0.08, 0.32, 1.0);
    vec3 d = vec3(0.5, 0.12, 1.0), e = vec3(0.95, 0.08, 1.0), f = vec3(1.0, 0.1, 0.52);
    if (s < 1.0) return mix(a, b, s);
    if (s < 2.0) return mix(b, c, s - 1.0);
    if (s < 3.0) return mix(c, d, s - 2.0);
    if (s < 4.0) return mix(d, e, s - 3.0);
    return mix(e, f, s - 4.0);
}
// palette position for a colony hash, per finish
float schemeS(float h) {
    if (gSch < 0.5) return 0.08 + 0.92 * h;                       // Chrome Neon: full cool spectrum
    if (gSch < 1.5) return 0.16 + 0.48 * h;                       // Chrome Ice: cyan → blue → violet
    if (gSch < 2.5) return h < 0.22 ? 0.2 : 0.55 + 0.45 * (h - 0.22) / 0.78; // Black Chrome: violet/magenta/pink + cyan
    return h;                                                     // Holographic
}
void finishSetup() {
    float ar = audioReact;
    gSch = float(finishSel);
    gFilmK = (gSch < 0.5 ? 0.55 : gSch < 1.5 ? 0.3 : gSch < 2.5 ? 0.45 : 1.1) * iridescence;
    gLa = TIME * 0.16 + audioMidTime * 0.12 * ar + 1.2 * gBuild * gBuild;
    gLI = lightAmt * (0.35 + 0.55 * gLift + 0.45 * gPeak + 0.35 * audioBeatPulse * ar - 0.15 * gRest);
    gLc1 = gSch < 1.5 ? vec3(0.0, 0.85, 1.0) : gSch < 2.5 ? vec3(0.55, 0.1, 1.0) : vec3(0.0, 0.95, 0.9);
    gLc2 = gSch < 0.5 ? vec3(1.0, 0.08, 0.8) : gSch < 1.5 ? vec3(0.15, 0.3, 1.0) : gSch < 2.5 ? vec3(1.0, 0.06, 0.6) : vec3(0.85, 0.15, 1.0);
}

// studio environment seen in reflections: dark ceiling, white softboxes, cool horizon, sweeping neon bars
vec3 envMap(vec3 r) {
    vec2 rx = r.xy;
    float rl = length(rx);
    vec3 c = mix(vec3(0.012, 0.014, 0.02), vec3(0.05, 0.058, 0.075), smoothstep(0.0, 0.5, rl));
    c += vec3(0.11, 0.115, 0.125) * smoothstep(-0.32, 0.32, rx.y - 0.6 * rx.x) + vec3(0.055, 0.058, 0.065) * (0.5 + 0.5 * sin(rx.x * 9.0 + 1.0));
    c += vec3(0.47, 0.5, 0.55) * smoothstep(0.5, 0.92, rl) * smoothstep(1.05, 0.9, rl);
    c *= 1.0 - 0.75 * smoothstep(0.97, 1.05, rl) * step(r.z, 0.05);
    vec2 s1 = rot(0.42) * rx;
    float strip = smoothstep(0.012, 0.007, abs(s1.x - 0.06)) * smoothstep(0.5, 0.3, abs(s1.y));
    strip += 0.6 * smoothstep(0.007, 0.004, abs(s1.x + 0.1)) * smoothstep(0.45, 0.28, abs(s1.y));
    float key = smoothstep(0.1, 0.07, max(abs(rx.x + 0.36) * 1.4, abs(rx.y - 0.42) * 0.7));
    c += vec3(0.93, 0.96, 1.0) * (strip * 1.5 + key * 2.4) * rimShine;
    // neon light bars sweeping through the studio
    vec2 a1 = vec2(cos(gLa), sin(gLa)), a2 = vec2(cos(-gLa * 0.83 + 2.1), sin(-gLa * 0.83 + 2.1));
    float b1 = dot(rx, a1) - 0.07 * sin(gLa * 1.7);
    float b2 = dot(rx, a2) - 0.09 * sin(gLa * 1.3 + 1.0);
    float m1 = exp(-b1 * b1 / 0.00008) + 0.05 * exp(-b1 * b1 / 0.002);
    float m2 = exp(-b2 * b2 / 0.00008) + 0.05 * exp(-b2 * b2 / 0.002);
    c += (gLc1 * m1 + gLc2 * m2) * 1.3 * gLI;
    return c;
}

// polished spun-chrome bowl + rounded lip; p = screen-aligned dish coords
vec3 chromeAt(vec2 p, float px, float dimK, out float brush) {
    float r = length(p);
    vec2 dir = p / max(r, 1e-4);
    float ang = atan(p.y, p.x);
    float s = 0.15 * r + 1.4 * pow(smoothstep(0.84, 1.0, r), 2.0);
    vec2 nxy = -dir * s;
    float t = clamp((r - 1.0425) / 0.0425, -1.0, 1.0);
    nxy = mix(nxy, dir * t * 1.5, smoothstep(0.996, 1.004, r));
    // brushed concentric grooves (fade out when below pixel size)
    brush = vn2(vec2(r * 240.0, ang * 2.0 + 7.0)) * 0.6 + vn2(vec2(r * 90.0, ang * 1.3)) * 0.4;
    nxy += dir * (brush - 0.5) * 0.006 * (1.0 - smoothstep(0.004, 0.008, px));
    vec3 n = normalize(vec3(nxy, 1.0));
    vec3 rf = reflect(vec3(0.0, 0.0, -1.0), n);
    vec3 e = envMap(rf);
    vec3 tint = gSch < 0.5 ? vec3(0.9, 0.91, 0.93) : gSch < 1.5 ? vec3(0.72, 0.86, 1.0) : gSch < 2.5 ? vec3(0.3, 0.31, 0.34) : vec3(0.9);
    if (gSch > 2.5) tint = mix(tint, neonCol(r * 1.6 + rf.x * 2.0 - rf.y * 1.5 + gT * 0.05), 0.6);
    vec3 c = e * tint;
    // anisotropic bow-tie highlights from the moving neon lights
    vec2 l1 = vec2(cos(gLa), sin(gLa)), l2 = vec2(cos(-gLa * 0.83 + 2.1), sin(-gLa * 0.83 + 2.1));
    float fl = smoothstep(0.05, 0.4, r) * smoothstep(1.0, 0.85, r) * (0.7 + 0.6 * brush);
    c += (gLc1 * pow(abs(dot(dir, l1)), 50.0) + gLc2 * pow(abs(dot(dir, l2)), 50.0)) * fl * 0.12 * gLI;
    // studio key bow-tie (white)
    c += vec3(0.8, 0.85, 0.95) * pow(abs(dot(dir, vec2(-0.64, 0.77))), 70.0) * fl * 0.12 * rimShine;
    return c * dimK;
}

// colony types: 0 glass dome colony, 1 branching front, 2 mould, 3 small glass bead, 4 swarming film
float colonyType(float form, float h, float isEvent) {
    if (form < 0.5) return isEvent > 0.5 ? (h < 0.6 ? 3.0 : 0.0) : (h < 0.32 ? 0.0 : h < 0.58 ? 1.0 : 3.0);
    if (form < 1.5) return isEvent > 0.5 ? (h < 0.7 ? 2.0 : 3.0) : (h < 0.66 ? 2.0 : h < 0.82 ? 0.0 : 3.0);
    if (form < 2.5) return 3.0;
    return isEvent > 0.5 ? (h < 0.6 ? 3.0 : 1.0) : (h < 0.45 ? 1.0 : h < 0.78 ? 0.0 : 3.0);
}
float colonyRmax(float typ, float h) {
    if (typ < 0.5) return 0.09 + 0.11 * h;
    if (typ < 1.5) return 0.17 + 0.17 * h;
    if (typ < 2.5) return 0.15 + 0.18 * h;
    if (typ < 3.5) return 0.025 + 0.04 * h;
    return 0.65 + 0.3 * h;
}
float colonyTau(float typ) {
    if (typ < 0.5) return 0.3;
    if (typ < 1.5) return 0.38;
    if (typ < 2.5) return 0.32;
    if (typ < 3.5) return 0.16;
    return 0.5;
}
float colonyR(float typ, float rmax, float age) {
    if (age <= 0.0) return 0.0;
    float r = rmax * (1.0 - exp(-age * growthRate / (0.75 * colonyTau(typ) * gP)));
    r *= 1.0 + (0.07 * bassP() * audioReact + 0.12 * gDrop) * (typ > 3.5 ? 0.4 : 1.0);
    return r;
}

// dendritic (DLA-like) colony: branches split in two at fixed radii, curling and thinning outward
float wrapA(float a) { return a - TAU * floor(a / TAU + 0.5); }
float dendrite(vec2 v, float d, float seed, float px, out float tw, out vec2 nb) {
    float th = mod(atan(v.y, v.x) + seed * TAU, TAU);
    float lv = d < 0.035 ? 0.0 : d < 0.08 ? 1.0 : d < 0.14 ? 2.0 : d < 0.21 ? 3.0 : 4.0;
    float rlo = lv < 0.5 ? 0.0 : lv < 1.5 ? 0.035 : lv < 2.5 ? 0.08 : lv < 3.5 ? 0.14 : 0.21;
    float rhi = lv < 0.5 ? 0.035 : lv < 1.5 ? 0.08 : lv < 2.5 ? 0.14 : lv < 3.5 ? 0.21 : 0.3;
    float n = 4.0 * pow(2.0, lv);
    float sw = TAU / n;
    float idx = floor(th / sw);
    float pidx = floor(idx * 0.5);
    float oc = (idx + 0.5 + 0.55 * (h12(vec2(idx, lv + seed * 17.0)) - 0.5)) * sw;
    float pc = lv < 0.5 ? oc : (pidx + 0.5 + 0.55 * (h12(vec2(pidx, lv - 1.0 + seed * 17.0)) - 0.5)) * sw * 2.0;
    float tb = mix(pc, oc, smoothstep(rlo, rlo + 0.75 * (rhi - rlo), d));
    tb += (vn2(vec2(d * 30.0, idx * 3.1 + lv * 17.0 + seed * 5.0)) - 0.5) * 0.45 * sw * smoothstep(rlo, rhi, d);
    float sg = wrapA(th - tb);
    float arc = d * abs(sg);
    tw = vn2(vec2(d * 160.0, idx + seed * 9.0));
    float w = 0.0105 * pow(0.76, lv) * (0.75 + 0.5 * tw) + px * 0.6;
    float alive = step(0.1 * lv, h12(vec2(idx * 1.7, lv * 3.3 + seed)) + 0.15) * step(0.1 * max(lv - 1.0, 0.0), h12(vec2(pidx * 1.7, lv * 3.3 - 3.3 + seed)) + 0.15);
    float f = smoothstep(w + px, w - px, arc) * alive;
    // glass tube cross-section normal
    vec2 tg = vec2(-v.y, v.x) / max(d, 1e-4);
    nb = tg * clamp(d * sg / w, -1.0, 1.0) * 0.9;
    // hairy fringe along the branches
    float fz = vn2(vec2(th * 60.0, d * 90.0) + seed * 40.0);
    f = max(f, smoothstep(w * 2.2, w, arc) * smoothstep(0.55, 0.8, fz) * alive);
    float core = smoothstep(0.03, 0.022, d);
    if (core > f) nb = v / 0.03 * 0.85;
    f = max(f, core);
    return f;
}

void main() {
    showDynamics();
    clocks();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;

    // ── pass 0: spawn events (beats / drop) in a persistent slot buffer ──
    // layout by normalised coords: bottom half = 16 slot columns (x, y, birth, seed); top half = control
    if (PASSINDEX == 0) {
        vec4 ctrl = texture2D(spawnBuf, vec2(0.5, 0.75));        // counter, armed, cycle id, drop latch
        float slot = floor(clamp(uv.x, 0.0, 0.9999) * NSLOT);
        vec4 me = texture2D(spawnBuf, vec2((slot + 0.5) / NSLOT, 0.25));
        float cycId = mod(gCyc, 512.0);
        bool fresh = FRAMEINDEX < 2 || abs(ctrl.z - cycId) > 0.5;
        if (fresh) { ctrl = vec4(0.0, 1.0, cycId, 0.0); me = vec4(0.0, 0.0, -1.0, 0.0); }
        bool canSpawn = gFp < 0.86 && beatSpawn > 0.01;
        bool beat = canSpawn && ctrl.y > 0.5 && audioBeatPulse * ar > 0.35 && h11(floor(TIME * 3.0) + cycId) < 0.35 + 0.65 * beatSpawn;
        bool burst = canSpawn && gDrop > 0.45 && ctrl.w < 0.5;
        if (burst || (beat && abs(slot - ctrl.x) < 0.5)) {
            vec2 hh = h22(vec2(slot * 7.13 + fract(TIME * 0.731) * 113.0, cycId + TIME * 0.37));
            float an = hh.x * TAU, rr = sqrt(hh.y) * 0.84;
            me = vec4(rr * cos(an), rr * sin(an), gA - (burst ? h11(slot + 3.1) * 0.6 : 0.0), h11(slot * 3.7 + TIME));
        }
        if (beat) ctrl.x = mod(ctrl.x + 1.0, NSLOT);
        if (beat) ctrl.y = 0.0;
        if (audioBeatPulse * ar < 0.2) ctrl.y = 1.0;
        if (burst) ctrl.w = 1.0;
        if (gDrop < 0.15) ctrl.w = 0.0;
        gl_FragColor = uv.y < 0.5 ? me : ctrl;
        return;
    }

    vec2 sp = (fc - 0.5 * R) / R.y;
    float trem = smoothstep(0.55, 1.0, gBuild);
    sp += trem * 0.004 * (h22(vec2(floor(TIME * 24.0), 3.0)) - 0.5);
    float DR = dishSize * (1.0 + 0.05 * gPeak - 0.08 * gBuild + 0.04 * gDrop);
    vec2 qs = sp / DR;                       // screen-aligned dish coords (light, chrome)
    float L = length(qs);

    // ── pass 2: depth of field + bloom + tone map + finish ──────────────
    if (PASSINDEX == 2) {
        vec4 c0 = texture2D(dishBuf, uv);
        vec3 col = c0.rgb;
        float dm = c0.a;
        float rpx = R.y / 1080.0;
        float jit = h12(fc) * TAU;
        // shallow depth of field: focus on the culture, softening toward the rim and beyond
        float rd = dofAmt * rpx * 7.0 * smoothstep(0.62, 1.35, L);
        if (rd > 0.35) {
            vec4 acc = c0;
            for (int i = 0; i < 12; i++) {
                float fi = float(i);
                float a = fi * 2.39996 + jit;
                vec2 o = vec2(cos(a), sin(a)) * sqrt((fi + 0.5) / 12.0) * rd / R;
                acc += texture2D(dishBuf, uv + o);
            }
            acc /= 13.0;
            col = acc.rgb; dm = acc.a;
        }
        // bloom: hue-preserving bright pass gathered over a jittered disc
        float bR = rpx * 26.0;
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 16; i++) {
            float fi = float(i);
            float a = fi * 2.39996 + jit;
            float rr = sqrt((fi + 0.5) / 16.0);
            vec3 s = texture2D(dishBuf, uv + vec2(cos(a), sin(a)) * rr * bR / R).rgb;
            float m = mx3(s);
            bl += s * (max(m - 0.95, 0.0) / max(m, 1e-4)) * (1.0 - 0.7 * rr);
        }
        col += bl / 16.0 * bloomAmt * (1.1 + 0.5 * gPeak + 0.6 * gDrop);
        // hue-preserving filmic shoulder on the dish, white roll-off on hot highlights
        float m = mx3(col);
        vec3 tm = col * ((1.0 - exp(-1.2 * m)) / max(m, 1e-4)) * 1.05;
        tm = mix(tm, vec3(mx3(tm)), 0.75 * smoothstep(1.3, 4.5, m));
        col = mix(col, tm, clamp(dm, 0.0, 1.0));
        // peak: richer colour; rest: softer
        col = mix(vec3(lum(col)), col, 1.0 + 0.2 * gPeak - 0.15 * gRest);
        col += vec3(0.85, 0.95, 1.0) * gDrop * gDrop * 0.07;
        col *= 1.0 + 0.1 * audioBeatPulse * ar;
        col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
        col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
        float alpha = clamp(dm + lum(col), 0.0, 1.0);
        col = studioPalette(clamp(col, 0.0, 1.0));
        gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
        return;
    }

    // ── pass 1: the dish (HDR) ──────────────────────────────────────────
    finishSetup();
    float px = 1.0 / (R.y * DR);             // dish units per pixel
    float spin = TIME * 0.012 * spinSpeed + audioMidTime * 0.01 * ar * spinSpeed;
    vec2 q = rot(spin) * qs;                 // culture frame (turns with the dish)
    float form = gForm;
    bool bio = form > 2.5;

    vec3 bg = backdrop(uv);
    // soft drop shadow of the dish on the bench
    float shL = length(qs - vec2(0.035, -0.05));
    bg *= 1.0 - 0.45 * smoothstep(1.25, 0.95, shL);

    vec3 col = bg;
    float alpha = 0.0;
    float inAgar = smoothstep(1.0 + px, 1.0 - px, L);
    float inDish = smoothstep(1.085 + px, 1.085 - px, L);

    if (L < 1.12) {
        vec3 light = normalize(vec3(-0.5, 0.6, 0.62));
        float pulse = audioBeatPulse * ar;

        // ── sterilising wipe → bare chrome of the next culture ──
        float wp = smoothstep(0.86, 0.985, gFp);
        float clean = 0.0, front = 0.0;
        if (wp > 0.0) {
            vec2 wd = vec2(cos(gCyc * 2.4 + 0.7), sin(gCyc * 2.4 + 0.7));
            float nf = 0.62 * (dot(qs, wd) * 0.5 + 0.5) + 0.38 * (vn2(qs * 3.0 + gCyc) * 0.65 + vn2(qs * 9.0) * 0.35) + 0.07 * (vn2(qs * 45.0) - 0.5);
            float edge = wp * 1.3 - 0.15;
            clean = smoothstep(edge + 0.02, edge - 0.02, nf);
            front = exp(-pow((nf - edge) / 0.03, 2.0)) * smoothstep(0.0, 0.08, wp) * smoothstep(1.0, 0.9, wp);
        }
        float lay = (1.0 - clean) * smoothstep(0.0, 0.02, gFp) * inAgar;
        float bioK = mix(bio ? 1.0 : 0.0, gNext > 2.5 ? 1.0 : 0.0, clean);

        // ── antibiotic assay masks: lawn + glass disks + inhibition halos ──
        float lawnGrow = smoothstep(0.02 * gP, 0.32 * gP, gA * growthRate);
        bool assay = form > 1.5 && form < 2.5;
        float lawn = 0.0, zoneRim = 0.0, diskM = 0.0, diskShadow = 0.0, lg = 0.0;
        vec2 diskV = vec2(0.0);
        if (assay) {
            float zoneMask = 1.0;
            for (int k = 0; k < 6; k++) {
                float fk = float(k);
                float an = fk * TAU / 5.0 + gCyc * 1.7 + 0.4;
                vec2 dc = fk < 4.5 ? 0.56 * vec2(cos(an), sin(an)) : vec2(0.0);
                float zr = 0.085 + (0.1 + 0.2 * h11(fk * 5.3 + gCyc * 2.1)) * smoothstep(0.04 * gP, 0.5 * gP, gA * growthRate);
                zr *= 1.0 + 0.04 * bassP() * ar + 0.06 * gDrop;
                float dz = length(q - dc);
                zoneMask = min(zoneMask, smoothstep(zr - 0.006, zr + 0.02, dz));
                zoneRim = max(zoneRim, exp(-pow((dz - zr - 0.025) / 0.018, 2.0)));
                float dmk = smoothstep(0.085 + px, 0.085 - px, dz);
                if (dmk > diskM) { diskM = dmk; diskV = q - dc; }
                diskShadow = max(diskShadow, smoothstep(0.11, 0.08, length(q - dc - vec2(0.012, -0.018))));
            }
            vec2 sd = rot(gCyc * 0.9) * q;
            float streak = 0.88 + 0.12 * sin(sd.y * 70.0 + vn2(sd * 6.0) * 4.0);
            lg = vn2(q * 60.0) * 0.5 + vn2(q * 140.0) * 0.5;
            lawn = lawnGrow * zoneMask * streak * (0.72 + 0.28 * lg) * (1.0 + 0.25 * zoneRim * lawnGrow);
            zoneRim *= lawnGrow;
        }

        // ── swarming film underneath (type 4, slot 0 only) ──
        float s0 = h11(gCyc * 7.31 + 0.5);
        bool swarm = (form < 0.5 || bio) && h11(s0 * 41.0 + 9.0) < 0.6;
        float film = 0.0, terr = 0.0, tEdge = 0.0, d0 = 0.0;
        if (swarm) {
            vec2 c0 = 0.35 * vec2(cos(s0 * TAU), sin(s0 * TAU)) * h11(s0 * 3.3);
            float r0 = colonyR(4.0, colonyRmax(4.0, h11(s0 * 5.7)), gA);
            vec2 v0 = q - c0;
            d0 = length(v0);
            vec2 dir0 = v0 / max(d0, 1e-4);
            float wob = vn3(vec3(dir0 * 5.0, s0 * 9.0)) - 0.5;
            float u0 = d0 / max(r0 * (1.0 + 0.12 * wob), 1e-4);
            film = smoothstep(1.0, 0.97, u0);
            terr = fract(d0 * 8.0 + wob * 0.6 * smoothstep(0.0, 0.25, d0) + s0);
            tEdge = smoothstep(0.82, 0.97, terr) * smoothstep(1.0, 0.97, terr);
        }

        // ── colonies: owner = nearest front (colonies stop where they meet) ──
        float e1 = 1e3, e2 = 1e3;
        vec2 bc = vec2(0.0);
        float br = 0.0, bt = 0.0, bs = 0.0, ba = 0.0;
        float keepFrac = colonyCount * (1.0 - 0.55 * gRest) * (assay ? 0.3 : 1.0);
        for (int k = 0; k < 18; k++) {
            float fk = float(k);
            float seed = h11(fk * 13.17 + gCyc * 7.31 + 0.5);
            if (k > 2 && h11(seed * 17.9 + 4.4) > keepFrac) continue;
            float typ = colonyType(form, h11(seed * 29.3 + 1.1), 0.0);
            float an = h11(seed * 91.7 + 1.3) * TAU;
            float rr = sqrt(h11(seed * 53.1 + 2.9)) * 0.8;
            vec2 c = rr * vec2(cos(an), sin(an));
            float birth = (k < 3 ? h11(seed * 7.7) * 0.06 : h11(seed * 7.7) * 0.55) * gP;
            float age = gA - birth;
            float r = colonyR(typ, colonyRmax(typ, h11(seed * 61.3 + 0.7)), age);
            if (r <= 0.0) continue;
            float e = length(q - c) - r * (typ > 1.5 && typ < 2.5 ? 1.15 : 1.0);
            if (e < e1) { e2 = e1; e1 = e; bc = c; br = r; bt = typ; bs = seed; ba = age; }
            else if (e < e2) { e2 = e; }
        }
        for (int k = 0; k < 16; k++) {
            vec4 ev = texture2D(spawnBuf, vec2((float(k) + 0.5) / NSLOT, 0.25));
            if (ev.z < 0.0) continue;
            float age = gA - ev.z;
            float seed = fract(ev.w * 7.13 + float(k) * 0.371);
            float typ = colonyType(form, ev.w, 1.0);
            float r = colonyR(typ, colonyRmax(typ, h11(seed * 61.3)) * (typ < 2.5 ? 0.55 : 1.0), age);
            if (r <= 0.0) continue;
            float e = length(q - ev.xy) - r * (typ > 1.5 && typ < 2.5 ? 1.15 : 1.0);
            if (e < e1) { e2 = e1; e1 = e; bc = ev.xy; br = r; bt = typ; bs = seed; ba = age; }
            else if (e < e2) { e2 = e; }
        }

        // ── owner colony as a neon glass body: coverage, normal, thickness, emission ──
        float hs = schemeS(h11(bs * 3.7 + 0.2));
        vec3 hc = neonCol(hs);
        float opa = 0.0, hgt = 0.0, emi = 0.0, tip = 0.0, bead = 0.0;
        vec3 nrm = vec3(0.0, 0.0, 1.0);
        vec2 v = q - bc;
        float d = length(v);
        vec2 dir = v / max(d, 1e-4);
        if (br > 0.0 && e1 < 0.09) {
            if (bt < 0.5 || bt > 2.5) {
                // glass dome with neon growth rings (or a small glass bead)
                float rr = br * (1.0 + 0.09 * (vn3(vec3(dir * 4.0, bs * 9.0)) - 0.5) * (bt < 0.5 ? 1.0 : 0.5));
                float u = d / max(rr, 1e-4);
                opa = smoothstep(1.0, 1.0 - 1.5 * px / max(rr, 1e-4), u);
                float sq = sqrt(max(1.0 - u * u, 0.0));
                nrm = normalize(vec3(dir * 0.9 * u / max(sq, 0.16), 1.0));
                hgt = sq;
                float ring = bt < 0.5 ? 0.5 + 0.5 * cos(d * 120.0 + bs * 6.0 + (vn2(v * 30.0) - 0.5) * 2.0) : 0.5;
                float ringL = bt < 0.5 ? pow(ring, 5.0) * smoothstep(0.12, 0.4, u) : 0.0;
                emi = 0.22 + 0.55 * smoothstep(0.65, 0.0, u) + 0.7 * ringL;
                tip = exp(-pow((u - 1.0) * rr / 0.012, 2.0));
            } else if (bt < 1.5) {
                // branching fractal front of glass tubes
                float tw = 0.0;
                vec2 nb = vec2(0.0);
                float fill = dendrite(v, d, bs, px, tw, nb);
                float ue = d + 0.05 * (vn2(dir * 6.0 + bs * 19.0) - 0.5) + 0.03 * (tw - 0.5);
                fill *= smoothstep(br, br - 0.012, ue);
                opa = fill * 0.95;
                nrm = normalize(vec3(nb, sqrt(max(1.0 - dot(nb, nb), 0.04))));
                hgt = nrm.z * 0.6;
                tip = fill * smoothstep(br - 0.05, br - 0.008, ue);
                emi = 0.35 + 0.35 * smoothstep(0.4, 0.0, d / br) + 0.4 * tw;
            } else {
                // mould: fibre-optic hyphae radiating from a powdery neon sporulating centre with glass beads
                float sh = gT * (0.25 + 0.6 * gBuild);
                float fb = vn3(vec3(dir * 95.0, d * 14.0 + bs * 5.0 - sh));
                float fb2 = vn3(vec3(dir * 36.0, d * 5.0 + bs * 11.0));
                float u = d / br;
                float ue = u - 0.24 * (fb * 0.6 + fb2 * 0.4);
                opa = smoothstep(1.0, 0.72, ue);
                float zone = smoothstep(0.78, 0.45, u + 0.1 * (fb2 - 0.5));
                float powder = vn2(v * 140.0) * 0.5 + vn2(v * 55.0) * 0.5;
                float rings = 0.5 + 0.5 * sin(d * 75.0 + fb2 * 3.0);
                vec3 hyph = mix(vec3(0.8, 0.94, 1.0), hc, 0.35);
                hc = mix(hyph, neonCol(schemeS(h11(bs * 8.3 + 0.9))), zone);
                nrm = normalize(vec3(dir * (1.0 - zone) * 0.55 + (fb - 0.5) * 1.3 * vec2(-dir.y, dir.x) + (powder - 0.5) * 0.5 * zone, 1.0));
                hgt = 0.3 + 0.5 * zone;
                emi = (0.25 + 0.75 * fb * fb) * (1.0 - zone) + zone * (0.3 + 0.5 * powder) * (0.75 + 0.35 * rings);
                // glass exudate beads on the sporulating zone
                vec2 g = v * 55.0;
                vec2 gi = floor(g);
                vec2 bv = fract(g) - 0.2 - 0.6 * h22(gi + bs * 17.0);
                bead = smoothstep(0.16, 0.08, length(bv)) * step(0.7, h12(gi + 3.0 + bs)) * zone;
                nrm = normalize(mix(nrm, normalize(vec3(bv / 0.16 * 0.85, 1.0)), bead));
                hgt = mix(hgt, 1.0, bead);
                opa *= 0.97;
            }
        }
        opa *= lay;
        float groove = (1.0 - smoothstep(0.0, 0.014, e2 - e1)) * step(e2, 0.004);
        float born = exp(-ba * 1.6);

        // ── chrome seen through the glass (refracted) ──
        vec2 off = nrm.xy * (0.012 + 0.035 * hgt) * opa;
        float brush = 0.0;
        vec3 cc = chromeAt(qs + off, px, mix(1.0, 0.16, bioK), brush);

        // caustics thrown by the glass bodies onto the chrome + a focused lens spot
        float prox = smoothstep(0.05, -0.03, e1) * lay * step(0.0, br - 1e-5);
        if (prox > 0.0) {
            vec2 cq = q * 36.0 + nrm.xy * 2.5;
            float caus = pow(ridge(vn2(cq + vec2(gT * 0.5, 0.0))) * ridge(vn2(cq * 1.6 - vec2(0.0, gT * 0.42) + 5.0)), 5.0);
            vec2 lo = rot(-spin) * vec2(0.45, -0.5);
            float spot = exp(-dot(v - lo * br, v - lo * br) / max(0.04 * br * br, 1e-6)) * step(bt, 0.5);
            cc += hc * (caus * 1.6 + spot * 1.3) * prox * (0.5 + 0.7 * gLift + 0.4 * highP() * ar);
        }
        // neon colonies pooling into the brushed chrome as reflections
        float pool = exp(-max(e1, 0.0) * 26.0) * step(0.0, br - 1e-5) * lay;
        cc += hc * pool * (0.12 + 0.2 * gLift + 0.15 * pulse) * (0.5 + 0.9 * brush) * (1.0 + 1.2 * bioK);
        // contact shadow just outside each body
        cc *= 1.0 - 0.3 * smoothstep(0.025, 0.0, e1) * step(0.0, e1) * lay;

        // ── swarming film: oil-slick soap film with neon terrace lines ──
        if (swarm) {
            float fa = film * lay;
            vec3 fcol = neonCol(schemeS(s0) + gFilmK * (terr * 0.6 + d0 * 1.5) + 0.1 * sin(gT * 0.4));
            if (bioK > 0.5) {
                float wave = 0.5 + 0.5 * sin(d0 * 30.0 - gT * 2.0 * (1.0 + 2.0 * gBuild));
                cc += fa * fcol * ((0.12 + 0.35 * wave) + tEdge * 0.6) * (0.5 + 0.6 * gLift);
            } else {
                cc = mix(cc, cc * 0.8 + fcol * 0.08, fa * 0.6);
                cc += fa * fcol * (0.05 + 0.1 * terr + 0.55 * tEdge) * (0.6 + 0.5 * gLift);
            }
        }

        // ── assay: iridescent lawn film, frosted glass disks, clean mirror halos ──
        if (assay) {
            float la = clamp(lawn * 0.85, 0.0, 1.0) * lay;
            vec3 lcol = neonCol(schemeS(0.35) + gFilmK * (lg * 0.9 + q.x * 0.4) + 0.08 * sin(gT * 0.5));
            cc = mix(cc, cc * 0.5 + lcol * (0.16 + 0.12 * lg), la);
            cc += lcol * zoneRim * 0.5 * lay * (0.6 + 0.5 * gLift);
            cc *= 1.0 - 0.3 * diskShadow * (1.0 - diskM) * lay;
            float fib = vn2(diskV * 260.0) * 0.6 + vn2(diskV * 90.0) * 0.4;
            float dl = length(diskV);
            float code = smoothstep(0.004, 0.0, abs(dl - 0.055)) * step(0.5, fract(atan(diskV.y, diskV.x) * 2.0));
            vec3 dn = normalize(vec3(diskV * 4.0, 1.0));
            vec3 frost = vec3(0.82, 0.9, 1.0) * (0.75 + 0.3 * fib) * (0.7 + 0.45 * dot(dn, light));
            vec3 dRing = neonCol(schemeS(0.8)) * smoothstep(0.012, 0.0, abs(dl - 0.079)) * 1.4;
            frost += dRing + neonCol(schemeS(0.55)) * code * 1.2;
            cc = mix(cc, frost, diskM * lay);
        }

        // ── colony glass: transmission + emission + thin-film iridescence + env reflection + neon rim ──
        if (opa > 0.001) {
            float nzc = clamp(nrm.z, 0.0, 1.0);
            float fres = pow(1.0 - nzc, 2.0);
            vec3 filmC = neonCol(hs + gFilmK * ((1.0 - nzc) * 1.8 + hgt * 0.7) + 0.12 * sin(gT * 0.6 + bs * 6.0));
            vec3 envG = envMap(reflect(vec3(0.0, 0.0, -1.0), nrm));
            float eK = (0.3 + 0.6 * gLift) * (1.0 + 0.3 * pulse) * (1.0 + 1.4 * bioK);
            vec3 g = cc * (hc * 0.7 + 0.12) * (1.0 - 0.35 * hgt);
            g += hc * emi * eK * 0.55;
            g += filmC * (0.06 + 1.1 * fres) * min(iridescence, 1.2) * (1.0 - 0.5 * bioK);
            g += (hc * 0.9 + filmC * 0.5) * tip * (0.9 + 0.8 * gLift);
            g += envG * (0.05 + 0.85 * fres + 0.2 * bead) * gloss * (1.0 - 0.4 * bioK);
            // glints on the highs
            float gl = step(0.975, h12(floor(q * 170.0) + floor(TIME * 9.0) * 1.37)) * smoothstep(0.6, 1.5, lum(envG));
            g += vec3(1.0) * gl * (0.4 + 2.5 * highP() * ar) * gloss;
            if (bioK > 0.5) {
                float wave = 0.5 + 0.5 * sin(d * 48.0 - gT * 2.6 * (1.0 + 1.5 * gBuild + 0.6 * gPeak) + bs * 6.0);
                g += hc * (0.2 + 0.7 * wave) * eK * 0.5;
            }
            g *= 1.0 - 0.6 * groove;
            g += hc * born * 1.4;
            cc = mix(cc, g, clamp(opa, 0.0, 1.0));
        }
        if (bioK > 0.5) {
            float halo = exp(-max(e1, 0.0) * 22.0) * step(e1, 0.5) * lay;
            cc += hc * halo * (0.1 + 0.18 * gLift + 0.2 * pulse);
        }

        // ── drop shockwave: neon ring across the dish ──
        float swr = (1.0 - gDrop) * 1.25;
        float wave = exp(-pow((length(q) - swr) * 10.0, 2.0)) * gDrop;
        cc += mix(gLc1, gLc2, 0.5 + 0.5 * sin(atan(q.y, q.x) * 2.0 + gT)) * wave * 0.9 * inAgar;
        // wipe front
        cc += vec3(0.7, 0.88, 1.0) * front * 0.7 * inAgar;

        // ── glass lid: faint cool band + thin edge lines with sweeping highlights ──
        float band = smoothstep(0.32, 0.0, abs(dot(qs, normalize(vec2(1.0, -1.0))) + 0.25)) * smoothstep(1.0, 0.5, L);
        cc += vec3(0.85, 0.93, 1.0) * band * 0.035 * rimShine * (1.0 - 0.6 * bioK);
        float ang = atan(qs.y, qs.x);
        vec2 l1 = vec2(cos(gLa), sin(gLa)), l2 = vec2(cos(-gLa * 0.83 + 2.1), sin(-gLa * 0.83 + 2.1));
        vec2 ad = vec2(cos(ang), sin(ang));
        float lines = exp(-pow((L - 1.004) / 0.004, 2.0)) * 0.5 + exp(-pow((L - 1.083) / 0.004, 2.0)) * 0.6;
        float hiW = pow(max(dot(ad, vec2(-0.64, 0.77)), 0.0), 14.0);
        cc += (vec3(0.8, 0.9, 1.0) * (0.15 + hiW) * rimShine + (gLc1 * pow(max(dot(ad, l1), 0.0), 20.0) + gLc2 * pow(max(dot(ad, l2), 0.0), 20.0)) * gLI) * lines;

        col = mix(col, cc, inDish);
        alpha = inDish;
    }
    gl_FragColor = vec4(col, alpha);
}
