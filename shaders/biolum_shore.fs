/*{
  "DESCRIPTION": "Bioluminescent Shore — a night beach where the sea is alive with dinoflagellates. The plankton is a real excitable medium (a Barkley / FitzHugh–Nagumo model running two solver steps every frame on the water surface): mechanical shear excites it, it flashes electric blue, then goes refractory and has to recover before it can fire again. Breaking waves are the trigger — the curl of every breaker ignites and the light races along the lip and through the whitewater as a travelling excitation front, foam lace sparkles grain by grain, the swash glows as it runs up the sand and slides back, and an invisible walker leaves a trail of glowing footprints in the wet sand that fade behind them. Fish strike the surface and send rings of light across the calm water. Shoaling swell sets refract around the curved bay, crests steepen and break where the water gets too shallow, wet sand mirrors the glow, and above it all a dark headland with a sweeping lighthouse beam, the Milky Way and a sky full of pinpoint stars. Bass charges the plankton and lifts the sets, beats fire extra splashes and brighter flashes, mids feed the sparkle, highs twinkle the stars.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #8",
  "CATEGORIES": ["Generator", "3D", "Nature", "Simulation", "Water", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Speed",              "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "panSpeed",     "LABEL": "Camera Pan",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Speed" },
    { "NAME": "waveHeight",   "LABEL": "Wave Height",        "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.8, "DEFAULT": 1.0, "GROUP": "Sea" },
    { "NAME": "setStrength",  "LABEL": "Swell Sets",         "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Sea" },
    { "NAME": "swash",        "LABEL": "Swash Run-up",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Sea" },
    { "NAME": "glow",         "LABEL": "Glow",               "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.3, "GROUP": "Bioluminescence" },
    { "NAME": "excitability", "LABEL": "Excitability",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Bioluminescence" },
    { "NAME": "afterglow",    "LABEL": "Afterglow",          "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Bioluminescence" },
    { "NAME": "sparkle",      "LABEL": "Plankton Sparkle",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Bioluminescence" },
    { "NAME": "footprints",   "LABEL": "Glowing Footprints", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Bioluminescence" },
    { "NAME": "splashes",     "LABEL": "Fish Splashes",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Bioluminescence" },
    { "NAME": "glowColor",    "LABEL": "Glow Color",         "TYPE": "color", "DEFAULT": [0.02, 0.45, 1.0, 1.0], "GROUP": "Color" },
    { "NAME": "coreColor",    "LABEL": "Glow Core",          "TYPE": "color", "DEFAULT": [0.55, 0.95, 1.0, 1.0], "GROUP": "Color" },
    { "NAME": "skyTop",       "LABEL": "Sky Zenith",         "TYPE": "color", "DEFAULT": [0.005, 0.009, 0.03, 1.0], "GROUP": "Color" },
    { "NAME": "skyHorizon",   "LABEL": "Sky Horizon",        "TYPE": "color", "DEFAULT": [0.03, 0.045, 0.09, 1.0], "GROUP": "Color" },
    { "NAME": "townGlow",     "LABEL": "Horizon Warmth",     "TYPE": "color", "DEFAULT": [0.09, 0.04, 0.05, 1.0], "GROUP": "Color" },
    { "NAME": "sandColor",    "LABEL": "Sand Color",         "TYPE": "color", "DEFAULT": [0.42, 0.37, 0.31, 1.0], "GROUP": "Color" },
    { "NAME": "milkyWay",     "LABEL": "Milky Way",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "lighthouse",   "LABEL": "Lighthouse",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Color" },
    { "NAME": "saturation",   "LABEL": "Saturation",         "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "exposure",     "LABEL": "Exposure",           "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "starlight",    "LABEL": "Starlight Fill",     "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "reflections",  "LABEL": "Glow Reflections",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "bloom",        "LABEL": "Bloom",              "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.9, "GROUP": "Finish" },
    { "NAME": "aberration",   "LABEL": "Chromatic Fringe",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "grain",        "LABEL": "Grain",              "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "simRes",       "LABEL": "Sim Resolution",     "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Finish" },
    { "NAME": "marchRes",     "LABEL": "March Resolution",   "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Finish" },
    { "NAME": "audioReact",   "LABEL": "Audio React",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",     "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "simA", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1280)", "HEIGHT": "min(floor($HEIGHT*$simRes*1.25),900)" },
    { "TARGET": "simB", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1280)", "HEIGHT": "min(floor($HEIGHT*$simRes*1.25),900)" },
    { "TARGET": "shScene", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { "TARGET": "shBloom", "WIDTH": "floor($WIDTH/4)", "HEIGHT": "floor($HEIGHT/4)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Bioluminescent Shore — CURVEBALL pack #8
//   pass 0/1  simA → simB : two Barkley excitable-medium steps per frame on
//             the sea surface (world-space grid, sqrt-spaced in depth so the
//             near shore gets the resolution). r = excitation u, g = recovery
//             v, b = afterglow, a = sand wetness. Stimulus = wave shear
//             (breaking curl, whitewater, swash front), footsteps, fish strikes.
//   pass 2    shScene : heightfield raymarch of sand + shoaling/breaking swell,
//             night sky (Milky Way, stars, headland, lighthouse), glow
//             emission + spill + reflections, plankton sparkle
//   pass 3    shBloom : quarter-res wide bloom prefilter
//   pass 4    final   : bloom, fringe, ACES, HD finisher, grain
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define PI   3.14159265359
#define TAU  6.28318530718

const float XW = 23.0;   // sim covers x in [-XW, XW]
const float Z0 = 0.5;    // and z in [Z0, Z1] (sqrt spaced)
const float Z1 = 34.0;

float g_faceT;
float g_t, g_wt, g_ar, g_bass, g_mid, g_high, g_beat, g_lev, g_set;

float bassP() { return pow(smoothstep(0.05, 0.85, audioBass), 1.3); }
float midP()  { return pow(smoothstep(0.06, 0.85, audioMid), 1.2); }
float highP() { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }

float bsH11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float bsH12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  bsH22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float bsNoise(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(bsH12(p), bsH12(p + vec2(1.0, 0.0)), f.x), mix(bsH12(p + vec2(0.0, 1.0)), bsH12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float bsFbm(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 4; i++) { s += a * bsNoise(p); p = p * 2.03 + 17.1; a *= 0.5; }
    return s;
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { return clamp((x * (2.51 * x + 0.03)) / (x * (2.43 * x + 0.59) + 0.14), 0.0, 1.0); }

// foam lace: thin threads around cells (F2 - F1 of a jittered grid)
float lace(vec2 c) {
    vec2 ip = floor(c), fp = fract(c);
    float f1 = 8.0, f2 = 8.0;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 g = vec2(float(i), float(j));
        vec2 o = bsH22(ip + g);
        vec2 r = g + o - fp;
        float d = dot(r, r);
        if (d < f1) { f2 = f1; f1 = d; } else if (d < f2) { f2 = d; }
    }
    float e = sqrt(f2) - sqrt(f1);
    // foam = the material BETWEEN round holes: thick at junctions, holes vary in size
    float hole = smoothstep(0.10, 0.34, e) * smoothstep(0.15, 0.55, bsH12(ip + 3.7) + 0.35);
    return 1.0 - hole;
}

// one voronoi layer: returns hole mask (1 inside a hole), holes of very different sizes
float foamHoles(vec2 c, float seed) {
    vec2 ip = floor(c), fp = fract(c);
    float f1 = 8.0, f2 = 8.0; vec2 id1 = ip;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 g = vec2(float(i), float(j));
        vec2 o = bsH22(ip + g + seed);
        vec2 r = g + o - fp;
        float d = dot(r, r);
        if (d < f1) { f2 = f1; f1 = d; id1 = ip + g; } else if (d < f2) { f2 = d; }
    }
    float e = sqrt(f2) - sqrt(f1);
    float hr = bsH12(id1 + seed * 1.7);
    float th = mix(0.04, 0.55, hr * hr);           // hole size varies wildly per cell
    float keep = step(0.18, hr);                     // some cells stay solid foam
    return smoothstep(th * 0.6, th, e) * keep;
}
// real sea foam: warped, stretched along the flow, big torn holes + fine perforation + filaments
float seaFoam(vec2 p, float stretch) {
    vec2 c = vec2(p.x, p.y * stretch);
    vec2 w = vec2(bsNoise(c * 0.55), bsNoise(c * 0.55 + 11.3)) - 0.5;
    c += w * 1.6;
    float big = foamHoles(c * 1.1, 3.0);
    float mid = foamHoles(c * 2.9 + 5.1, 7.0);
    float fine = foamHoles(c * 7.3 + 1.7, 13.0);
    float f = (1.0 - big) * (1.0 - 0.85 * mid) * (1.0 - 0.6 * fine);
    // filaments: thin threads bridging the holes
    float fil = 1.0 - smoothstep(0.0, 0.06, abs(bsNoise(c * vec2(5.0, 1.4)) - 0.5));
    f = max(f, fil * big * 0.55);
    // thickness variation
    return clamp(f * (0.55 + 0.6 * bsNoise(c * 0.9 + 4.0)), 0.0, 1.0);
}

// ── world mapping ────────────────────────────────────────────────────────
vec2 simToWorld(vec2 uv) { return vec2((uv.x - 0.5) * 2.0 * XW, Z0 + (Z1 - Z0) * uv.y * uv.y); }
vec2 worldToSim(vec2 p)  { return vec2(p.x / (2.0 * XW) + 0.5, sqrt(max(p.y - Z0, 0.0) / (Z1 - Z0))); }

// ── beach ────────────────────────────────────────────────────────────────
const float SLOPE = 0.12;
float shoreZ(float x) { return 6.2 + 1.4 * sin(x * 0.095 + 0.6) + 0.6 * sin(x * 0.27 + 2.1) + 0.22 * sin(x * 0.71 + 4.0); }
float sandBase(vec2 p) {
    float s = shoreZ(p.x);
    float y = SLOPE * (s - p.y);
    // berm + soft dunes above the waterline
    float up = smoothstep(s - 1.0, s - 7.0, p.y);
    y += up * (0.10 * (bsNoise(p * vec2(0.25, 0.4)) - 0.5) + 0.05 * sin(p.x * 0.4 + p.y * 0.3));
    return y;
}
float sandH(vec2 p) { return sandBase(p) + 0.006 * (bsNoise(p * 4.1) - 0.5); }

// ── swell: three shoaling, breaking crest trains ─────────────────────────
// returns height; out: brk (breaking strength), ww (whitewater), curl (lip), q (phase of dominant)
float trainH(vec2 p, float k, float w, float tilt, float ph, float A0, float d,
             out float brk, out float ww, out float curl, out float qq, out float wid) {
    float s = p.y - 0.8 * shoreZ(p.x);
    float phi = k * (s + p.x * tilt) + w * g_wt + ph;
    float q = fract(phi / TAU);
    wid = floor(phi / TAU);
    // set envelope: groups of bigger waves, per wave id
    float env = 1.0 - setStrength * 0.55 * (0.5 + 0.5 * sin(wid * 1.37 + 0.8 * sin(wid * 0.53)));
    float A = A0 * env * waveHeight * g_set;
    float shoal = clamp(pow(3.0 / max(d, 0.08), 0.25), 0.8, 2.2);
    float As = A * shoal;
    float ratio = 2.0 * As / (0.78 * d + 0.02);
    brk = smoothstep(0.8, 1.15, ratio);
    float Ae = mix(As, 0.42 * d + 0.015, brk);
    float fr = mix(5.0, 34.0, smoothstep(0.35, 1.0, ratio));
    float bk = mix(2.6, 4.0, brk);
    float h = exp(-q * bk) + exp(-(1.0 - q) * fr) - 0.32;
    // pitching lip: a ledge thrown forward of the crest just before it plunges
    float lipZone = smoothstep(0.75, 1.05, ratio) * (1.0 - smoothstep(1.1, 1.6, ratio));
    h += lipZone * 0.35 * exp(-pow((1.0 - q - 0.035) / 0.022, 2.0));
    ww = brk * exp(-q * 3.2);                         // whitewater trailing the bore
    g_faceT = max(g_faceT, smoothstep(0.55, 0.95, ratio) * smoothstep(0.78, 0.97, q) * (A0 > 0.3 ? 1.0 : 0.5));
    curl = smoothstep(0.55, 1.0, ratio) * exp(-(1.0 - q) * fr * 0.7) * (0.4 + 0.6 * brk);
    qq = q;
    return Ae * h;
}

// full surface info for water
float seaSurface(vec2 p, out float brk, out float ww, out float curl, out float q0, out float wid0) {
    float d = max(-sandBase(p), 0.0) + 0.02;
    float b0, w0, c0, b1, w1, c1, b2, w2, c2, q1, q2, i1, i2;
    float h = trainH(p, 0.52, 1.05, 0.13, 0.0, 0.40, d, b0, w0, c0, q0, wid0);
    h += trainH(p, 0.81, 1.30, -0.10, 1.7, 0.20, d, b1, w1, c1, q1, i1);
    h += trainH(p, 1.37, 1.70, 0.17, 4.1, 0.08, d, b2, w2, c2, q2, i2);
    brk = max(b0, max(b1 * 0.8, b2 * 0.5));
    ww = clamp(w0 + 0.7 * w1 + 0.4 * w2, 0.0, 1.0);
    curl = clamp(c0 + 0.7 * c1 + 0.4 * c2, 0.0, 1.0);
    // gentle small chop
    h += 0.025 * sin(p.x * 2.1 + p.y * 1.3 + g_wt * 2.3) + 0.018 * sin(p.x * -1.4 + p.y * 2.7 + g_wt * 2.9);
    return h;
}

// swash: run-up front driven by the dominant train arriving at the shore
float g_unused;
void swashInfo(float x, out float front, out float runR, out float rdot) {
    float s0 = shoreZ(x);
    float s = s0 - 0.8 * s0;
    float phi = 0.52 * (s + x * 0.13) + 1.05 * g_wt;
    float q = fract(phi / TAU);
    float wid = floor(phi / TAU);
    float env = 1.0 - setStrength * 0.55 * (0.5 + 0.5 * sin(wid * 1.37 + 0.8 * sin(wid * 0.53)));
    float Rm = 2.6 * swash * waveHeight * env * g_set;
    // fast uprush (q 0..0.22), slow backwash
    float up = smoothstep(0.0, 0.22, q);
    float back = 1.0 - smoothstep(0.22, 0.9, q);
    float r = Rm * up * back * (0.85 + 0.15 * sin(x * 1.3 + wid));
    runR = r;
    front = s0 - r;
    rdot = (q < 0.22) ? 1.0 : -0.35;
}

// combined terrain height (for the march)
float surfH(vec2 p) {
    float sb = sandH(p);
    float s0 = shoreZ(p.x);
    if (p.y < s0 - 0.3) {
        float fr, rr, rd; swashInfo(p.x, fr, rr, rd);
        float sheet = (p.y > fr) ? 0.025 * smoothstep(fr, fr + 0.6, p.y) : 0.0;
        return sb + sheet;
    }
    float b, w, c, q, id;
    float hs = seaSurface(p, b, w, c, q, id);
    return max(sb, hs);
}

// ── stimulus & agitation for the plankton ────────────────────────────────
float g_footGlow;
float footStep(vec2 p, out float wetMark) {
    g_footGlow = 0.0;
    // invisible walker along the wet sand, left → right, loops every P seconds
    const float P = 40.0, stride = 0.74, x0 = -28.0;
    float spd = 56.0 / P;
    float pathT = mod(g_t, P);
    float n = floor((p.x - x0) / stride);
    float stim = 0.0;
    wetMark = 0.0;
    for (int k = 0; k < 2; k++) {
        float nn = n + float(k);
        float xf = x0 + (nn - 0.5) * stride;
        float side = mod(nn, 2.0) * 2.0 - 1.0;
        float zf = shoreZ(xf) - 3.0 + side * 0.13;
        float ts = (xf - x0) / spd;
        float age = pathT - ts;
        vec2 dl = p - vec2(xf, zf);
        // foot shape: heel + sole ellipse + toes (mirror by side)
        vec2 e = dl / vec2(0.15, 0.062);
        float sole = 1.0 - smoothstep(0.75, 1.0, length(e - vec2(0.15, 0.0)));
        float heel = 1.0 - smoothstep(0.7, 1.0, length((dl - vec2(-0.08, 0.0)) / vec2(0.06, 0.042)));
        float toes = 0.0;
        for (int t = 0; t < 4; t++) {
            float ft = float(t);
            vec2 tp = vec2(0.13 + 0.008 * ft, side * (-0.03 + 0.02 * ft));
            toes = max(toes, 1.0 - smoothstep(0.009, 0.016 - 0.002 * ft, length(dl - tp)));
        }
        float foot = max(max(sole, heel), toes);
        if (age > 0.0 && age < 0.16) stim = max(stim, foot);
        if (age > 0.0 && age < 30.0) wetMark = max(wetMark, foot * (1.0 - age / 30.0));
        if (age > 0.0) {
            // analytic afterglow of the print: bright flash, slow fade, rim glows longest
            float rimG = foot * (1.0 - foot) * 4.0;
            float gl = foot * (exp(-age * 2.0) * 1.3 + 0.5 * exp(-age / (6.0 * afterglow))) + rimG * 0.6 * exp(-age / (10.0 * afterglow));
            g_footGlow = max(g_footGlow, gl);
        }
    }
    return stim;
}

float splashRing(vec2 p, float slot, out float ripple) {
    float per = 2.9 + slot * 0.7;
    float tt = g_t + slot * 1.37;
    float c = floor(tt / per);
    float age = mod(tt, per);
    vec2 h = bsH22(vec2(c, slot * 7.1));
    vec2 cp = vec2((h.x - 0.5) * 30.0, 0.0);
    cp.y = shoreZ(cp.x) + 4.0 + h.y * 18.0;
    float r = 0.15 + 2.6 * age;
    float dd = length(p - cp);
    float life = 1.0 - smoothstep(0.0, 1.8, age);
    float exist = step(0.25, bsH11(c * 3.7 + slot));
    ripple = exist * life * sin((dd - r) * 9.0) * exp(-abs(dd - r) * 2.5) * smoothstep(r + 0.2, 0.0, dd);
    return exist * life * exp(-pow((dd - r) / 0.045, 2.0)) * smoothstep(0.0, 0.1, age) * (0.35 + 0.65 * life);
}

void plankton(vec2 p, out float stim, out float agit, out vec2 vel, out float sheet, out float foot) {
    float s0 = shoreZ(p.x);
    float b, ww, cu, q, id;
    float hs = seaSurface(p, b, ww, cu, q, id);
    float sea = smoothstep(s0 - 0.4, s0 + 0.3, p.y);
    // lace that travels with each wave
    // stimulus detail only on the first solver step and only where something can fire
    bool detail = (PASSINDEX == 0);
    float waveAct = sea * (cu + ww);
    float L = 0.0;
    if (detail && waveAct > 0.01)
        L = seaFoam(vec2(p.x * 1.3 + id * 5.3, q * 9.0 + id * 3.1), 0.9);
    float fr, rr, rd; swashInfo(p.x, fr, rr, rd);
    sheet = (p.y > fr && p.y < s0 + 0.3) ? 1.0 : 0.0;
    float frontBand = exp(-pow((p.y - fr) / 0.18, 2.0)) * step(0.15, rr);
    float wm;
    foot = 0.0;
    if (detail && abs(p.y - (s0 - 3.0)) < 0.6) foot = footStep(p, wm) * footprints;
    float rip0, rip1;
    float spl = (splashRing(p, 0.0, rip0) + splashRing(p, 1.0, rip1)) * splashes;
    float L2 = 0.0;
    if (detail && frontBand + spl > 0.01) L2 = seaFoam(p * 2.2 + vec2(0.0, g_wt * 0.4), 0.6);
    float kick = 1.0 + 0.8 * g_ar * (g_bass + 0.6 * g_beat);
    float fresh = ww * exp(-q * 9.0);
    float patchy = smoothstep(0.35, 0.7, bsNoise(p * vec2(0.45, 0.9) + vec2(id * 2.3, 0.0)));
    stim = sea * (cu * (0.35 + 0.65 * L) * (0.55 + 0.45 * patchy) + fresh * L * 0.7) * kick;
    stim += frontBand * (0.25 + 0.75 * L2) * (rd > 0.0 ? 0.6 : 0.3) * kick;
    stim += spl * L2 * 0.8;
    stim += foot;
    if (!detail) stim = 0.0;
    agit = clamp(sea * (ww * 0.8 + cu) + frontBand + sheet * 0.3 + spl * 0.8, 0.0, 1.0);
    // surface drift: the bore and the swash carry the light
    vel = vec2(0.15 * sin(p.y * 0.3 + g_wt * 0.2), -2.6 * ww * sea - 2.2 * cu * sea);
    if (sheet > 0.5) vel.y += -2.8 * rd * smoothstep(fr, fr + 1.5, p.y);
}

// ── simulation pass ──────────────────────────────────────────────────────
vec4 simPrev(vec2 uv) {
    if (PASSINDEX == 0) return texture2D(simB, uv);
    return texture2D(simA, uv);
}

vec4 simStep(vec2 fc) {
    vec2 uv = fc / R;
    vec2 px = 1.0 / R;
    vec2 p = simToWorld(uv);
    float stim, agit, sheet, foot; vec2 vel;
    plankton(p, stim, agit, vel, sheet, foot);
    float dt = clamp(TIMEDELTA, 0.0, 0.05) * speed * 0.5;
    if (FRAMEINDEX < 2 && PASSINDEX == 1) return texture2D(simA, uv);
    if (FRAMEINDEX < 2) {
        // frame 0: the shore is already alight
        float u0 = clamp(stim * 0.8, 0.0, 1.0);
        float wet = smoothstep(shoreZ(p.x) - 3.2, shoreZ(p.x) - 0.5, p.y);
        float wm; float f = footStep(p, wm);
        return vec4(u0, 0.25 * agit, clamp(u0 * 0.8 + 0.2 * agit * bsNoise(p * 3.0) + wm * 0.7 * footprints, 0.0, 1.0), wet);
    }
    vec2 duv = vec2(vel.x / (2.0 * XW), vel.y / (2.0 * (Z1 - Z0) * max(uv.y, 0.03)));
    vec2 su = uv - duv * dt;
    vec4 c  = simPrev(su);
    vec4 cn = simPrev(su + vec2(0.0, px.y));
    vec4 cs = simPrev(su - vec2(0.0, px.y));
    vec4 ce = simPrev(su + vec2(px.x, 0.0));
    vec4 cw = simPrev(su - vec2(px.x, 0.0));
    float u = c.r, v = c.g;
    float lap = cn.r + cs.r + ce.r + cw.r - 4.0 * u;
    // Barkley: du = u(1-u)(u - (v+b)/a)/eps + D lap ; dv = u - v
    float a = 0.78, eps = 0.028;
    float bth = mix(0.34, 0.035, clamp(agit * (0.4 + excitability), 0.0, 1.0));
    float h = 0.03 * speed;
    float D = 5.0 * (0.25 + 0.75 * agit);
    float du = u * (1.0 - u) * (u - (v + bth) / a) / eps + D * lap;
    float dv = u - v;
    u = clamp(u + h * du, 0.0, 1.0);
    v = clamp(v + h * dv * 1.15, 0.0, 1.0);
    // mechanical stimulation fires wherever the cells have recovered
    float ready = 1.0 - smoothstep(0.25, 0.7, v);
    u = max(u, clamp(stim, 0.0, 1.0) * ready);
    // afterglow (longer on the sand so footprints linger)
    float onSand = 1.0 - smoothstep(shoreZ(p.x) - 0.6, shoreZ(p.x) + 0.2, p.y);
    float tau = mix(0.9, 3.2, onSand) * afterglow;
    float ag = max(c.b * exp(-dt / tau), u);
    float wet = max(c.a * exp(-dt / 30.0), sheet);
    return vec4(u, v, ag, wet);
}

// ── sky ──────────────────────────────────────────────────────────────────
float g_pixAng;
vec3 g_lhPos; vec3 g_lhDir;

float headland(float az) {
    // dark headland on the right of the bay
    float x = az - 0.36;
    float m = smoothstep(0.0, 0.28, x) * (1.0 - smoothstep(0.95, 1.3, x));
    float prof = 0.05 * sqrt(max(sin(clamp(x / 1.25, 0.0, 1.0) * PI), 0.0));
    float hgt = m * (prof + 0.014 * bsFbm(vec2(az * 7.0, 1.3)) + 0.004 * bsNoise(vec2(az * 60.0, 2.0)));
    return hgt;
}

vec3 sky(vec3 rd, bool primary) {
    float el = rd.y;
    float az = atan(rd.x, rd.z);
    float up = max(el, 0.0);
    vec3 col = mix(skyHorizon.rgb, skyTop.rgb, pow(up, 0.45));
    col += townGlow.rgb * 0.6 * exp(-abs(el) * 22.0) * (0.5 + 0.5 * smoothstep(0.6, -0.4, az));
    if (el < 0.0) col *= 0.7;
    // Milky Way
    vec3 nb = normalize(vec3(0.78, 0.56, -0.2));
    float bd = dot(rd, nb);
    float band = exp(-bd * bd * 16.0);
    vec2 mu = vec2(az * 5.0, el * 5.0);
    float dust = 0.5, lanes = 0.0;
    if (primary && band > 0.02) {
        dust = bsFbm(mu * 2.2 + 3.0);
        lanes = smoothstep(0.48, 0.66, bsFbm(mu * 5.5 + vec2(bd * 6.0, 1.7)));
    }
    vec3 cf = normalize(vec3(-0.35, 0.16, 0.92));
    vec3 cdir = normalize(cf - nb * dot(cf, nb));
    float core = pow(max(dot(rd, cdir), 0.0), 6.0);
    vec3 mwc = mix(vec3(0.55, 0.64, 0.9), vec3(0.95, 0.8, 0.66), core * 0.8);
    float mw = band * (0.35 + 0.65 * dust) * (1.0 - 0.55 * lanes * band) * (0.6 + 1.6 * core);
    col += mwc * mw * 0.11 * milkyWay * smoothstep(-0.02, 0.15, el);
    // stars: two layers of pinpoints, steep brightness distribution
    if (el > -0.05 && primary) {
        for (int L = 0; L < 2; L++) {
            float sc = (L == 0) ? 160.0 : 330.0;
            vec2 sp = vec2(az * sc / TAU * 6.0, el * sc);
            vec2 ip = floor(sp);
            vec2 hh = bsH22(ip + float(L) * 31.0);
            vec2 sc2 = ip + 0.15 + 0.7 * hh;
            float dd = length(sp - sc2) / sc;                  // angular distance (approx)
            float ex = step((L == 0) ? 0.86 : 0.72, bsH12(ip * 0.37 + float(L) * 9.0));
            float br = ex * pow(bsH12(ip * 1.7 + float(L)), (L == 0) ? 5.0 : 7.0) * ((L == 0) ? 2.2 : 0.7);
            br *= 1.0 + band * 1.5;
            float tw = 0.75 + 0.25 * sin(g_t * (3.0 + 5.0 * hh.x) + hh.y * 40.0) * (0.5 + g_high * g_ar);
            float pa = primary ? g_pixAng : g_pixAng * 3.0;
            float st = br * tw * exp(-dd * dd / (pa * pa * 0.5));
            vec3 tint = mix(vec3(0.75, 0.85, 1.0), vec3(1.0, 0.85, 0.65), hh.x);
            col += tint * st * smoothstep(-0.01, 0.06, el);
        }
    }
    // shooting star every ~14 s
    {
        float per = 14.0;
        float c = floor(g_t / per);
        float age = mod(g_t, per);
        vec2 h = bsH22(vec2(c, 9.0));
        float life = smoothstep(0.0, 0.1, age) * (1.0 - smoothstep(0.5, 0.9, age));
        if (life > 0.0 && primary) {
            vec2 a0 = vec2(-0.5 + h.x, 0.25 + 0.2 * h.y);
            vec2 dir = normalize(vec2(0.8, -0.35 - 0.2 * h.y));
            vec2 pnow = a0 + dir * age * 0.45;
            vec2 pp = vec2(az, el);
            vec2 rel = pp - pnow;
            float along = dot(rel, -dir);
            float perp = length(rel + dir * along);
            float trail = smoothstep(0.12, 0.0, along) * step(0.0, along);
            col += vec3(0.8, 0.9, 1.0) * life * trail * exp(-perp * perp / (g_pixAng * g_pixAng * 1.2)) * 1.2;
        }
    }
    // headland silhouette with village lights
    float hh = headland(az);
    if (el < hh && el > -0.02 && (primary || el > 0.006)) {
        float rim = smoothstep(hh - 0.004, hh, el);
        col = vec3(0.003, 0.004, 0.008) + col * 0.12 + skyHorizon.rgb * 0.35 * rim;
        vec2 lp = vec2(az * 260.0, el * 260.0);
        vec2 li = floor(lp);
        float on = step(0.93, bsH12(li)) * step(el, hh * 0.55) * (primary ? 1.0 : 0.0);
        float lt = exp(-dot(fract(lp) - 0.5, fract(lp) - 0.5) * 60.0) * on;
        col += vec3(1.0, 0.62, 0.3) * lt * 0.8;
    }
    // lighthouse beam (volumetric, sweeping)
    if (lighthouse > 0.0) {
        vec3 w = g_lhPos;
        vec3 rel = w;                 // camera at origin for sky purposes
        vec3 n = cross(rd, g_lhDir);
        float nl = length(n);
        float dist = abs(dot(rel, n)) / max(nl, 1e-4);
        // only the forward half of the beam
        float tb = dot(cross(rel, rd), n) / max(nl * nl, 1e-4);
        float bw = 1.5 + 0.06 * max(tb, 0.0);
        float beam = exp(-dist * dist / (bw * bw)) * step(0.0, tb) * exp(-max(tb, 0.0) * 0.0035);
        col += vec3(1.0, 0.86, 0.62) * beam * 0.13 * lighthouse;
        // the lamp itself
        vec3 ld = normalize(w);
        float lamp = exp(-(1.0 - dot(rd, ld)) / (g_pixAng * g_pixAng * 2.0));
        float facing = 0.25 + 0.75 * pow(max(dot(g_lhDir, -ld), 0.0), 8.0);
        col += vec3(1.0, 0.9, 0.7) * lamp * facing * lighthouse * 2.5;
    }
    return col;
}

// ── scene ────────────────────────────────────────────────────────────────
vec3 g_ro;

float g_agShare;
vec3 glowRamp(float u, float ag) {
    vec3 deep = vec3(0.10, 0.12, 0.85) * 0.8;
    vec3 c = mix(glowColor.rgb, coreColor.rgb, smoothstep(0.35, 0.95, u));
    return mix(c, deep, ag * (1.0 - smoothstep(0.3, 0.8, u)));
}
float glowAt(vec2 p, out float uu) {
    vec2 s = worldToSim(p);
    float inD = step(Z0, p.y) * step(abs(p.x), XW - 0.5) * step(p.y, Z1);
    vec4 c = texture2D(simB, s);
    float fade = smoothstep(Z1, Z1 - 6.0, p.y) * smoothstep(XW, XW - 8.0, abs(p.x));
    uu = c.r * inD * fade;
    g_agShare = clamp(c.b * 0.55 / (c.r + c.b * 0.55 + 1e-3), 0.0, 1.0);
    return (c.r + 0.55 * c.b) * inD * fade;
}

vec3 sparkles(vec2 p, float act, float ps) {
    if (act < 0.01) return vec3(0.0);
    vec3 acc = vec3(0.0);
    for (int L = 0; L < 2; L++) {
        float cs = (L == 0) ? 0.05 : 0.11;
        vec2 cp = p / cs;
        vec2 ip = floor(cp);
        vec2 h = bsH22(ip + float(L) * 17.0);
        vec2 c = ip + 0.2 + 0.6 * h;
        float d = length(cp - c) * cs;
        float pr = max(ps * 0.8, 0.004);
        float w = pow(bsH12(ip * 1.3 + 4.0), 3.0);
        float flick = 0.5 + 0.5 * sin(g_t * (9.0 + 14.0 * h.x) + h.y * 30.0);
        float sp = w * flick * exp(-d * d / (pr * pr));
        float far = smoothstep(cs * 0.25, cs * 0.6, ps);
        sp = mix(sp, w * 0.12, far);
        acc += sp * mix(glowColor.rgb, coreColor.rgb, 0.6);
    }
    return acc * act * sparkle * (0.7 + 0.6 * g_mid * g_ar);
}

vec4 renderScene(vec2 uv) {
    float yaw = 0.2 * sin(g_t * panSpeed * 0.045) + 0.06 * sin(g_t * panSpeed * 0.11);
    float pitch = -0.125 + 0.01 * sin(g_t * 0.21);
    float camZ = -0.4;
    vec3 ro = vec3(0.6 * sin(g_t * 0.05), sandBase(vec2(0.0, camZ)) + 1.72 + 0.015 * sin(g_t * 0.7), camZ);
    g_ro = ro;
    vec3 fw = vec3(sin(yaw) * cos(pitch), sin(pitch), cos(yaw) * cos(pitch));
    vec3 rt = normalize(cross(vec3(0.0, 1.0, 0.0), fw));
    vec3 upv = cross(fw, rt);
    float focal = 1.5;
    vec3 rd = normalize(fw * focal + rt * uv.x + upv * uv.y);
    g_pixAng = 1.0 / (R.y * focal);

    // lighthouse (on the headland, right side, far)
    float la = g_t * 0.55;
    float lhAz = 0.6;
    float lhD = 230.0;
    g_lhPos = vec3(sin(lhAz) * lhD, (headland(lhAz) + 0.004) * lhD, cos(lhAz) * lhD);
    g_lhDir = normalize(vec3(cos(la), -0.03, sin(la)));

    vec3 col;
    float cover = 1.0;
    // march the heightfield
    float t = 0.4, tPrev = 0.4;
    bool hit = false;
    float dPrev = 1.0;
    for (int i = 0; i < 90; i++) {
        vec3 pos = ro + rd * t;
        float dh = pos.y - surfH(pos.xz);
        if (dh < 0.0) { hit = true; break; }
        tPrev = t; dPrev = dh;
        t += max(dh * 0.7, 0.015 + t * 0.006);
        if (t > 75.0) break;
    }
    if (hit) {
        float ta = tPrev, tb = t;
        for (int k = 0; k < 6; k++) {
            float tm = 0.5 * (ta + tb);
            vec3 pm = ro + rd * tm;
            if (pm.y - surfH(pm.xz) < 0.0) tb = tm; else ta = tm;
        }
        t = 0.5 * (ta + tb);
    }
    bool farSea = false;
    if (!hit && rd.y < 0.0) {
        t = -ro.y / rd.y;
        farSea = true;
    }

    if (!hit && !farSea) {
        col = sky(rd, true);
        cover = 0.0;
    } else {
        vec3 pos = ro + rd * t;
        vec2 p = pos.xz;
        float ps = t * g_pixAng;                 // world size of one pixel
        float s0 = shoreZ(p.x);
        float sb = sandH(p);
        float b, ww, cu, q, id;
        g_faceT = 0.0;
        float hs = farSea ? 0.0 : seaSurface(p, b, ww, cu, q, id);
        float faceSave = g_faceT;
        if (farSea) { b = 0.0; ww = 0.0; cu = 0.0; q = 0.0; id = 0.0; }
        float fr, rr, rdt; swashInfo(p.x, fr, rr, rdt);
        bool isWater = farSea || (p.y >= s0 - 0.3 && hs > sb) || (p.y < s0 - 0.3 && p.y > fr);
        // normal
        float e = 0.012 + 0.0025 * t;
        vec3 n;
        if (farSea) n = vec3(0.0, 1.0, 0.0);
        else {
            float h0 = surfH(p);
            float hx = surfH(p + vec2(e, 0.0)) - h0;
            float hz = surfH(p + vec2(0.0, e)) - h0;
            n = normalize(vec3(-hx, e, -hz));
        }
        float uu;
        float gl = glowAt(p, uu);
        gl = gl / (1.0 + 0.45 * gl);          // soft shoulder: dense glow stays blue, never a white slab
        float agS = g_agShare;
        // glow light spilling onto nearby surfaces
        float tmp;
        float spill = 0.25 * (glowAt(p + vec2(0.7, 0.3), tmp) + glowAt(p + vec2(-0.6, 0.5), tmp)
                            + glowAt(p + vec2(0.2, 1.1), tmp) + glowAt(p + vec2(-0.3, -0.6), tmp));
        vec3 gcol = glowRamp(uu, agS);
        float gGain = glow * (1.0 + g_ar * (0.3 * g_bass + 0.45 * clamp(audioLevel, 0.0, 1.0) + 0.2 * clamp(audioMid, 0.0, 1.0)));
        vec3 amb = vec3(0.04, 0.05, 0.085) * starlight;

        if (isWater) {
            // directional ripple trains (anisotropic), fading with pixel footprint
            vec2 slope = vec2(0.0);
            for (int i = 0; i < 8; i++) {
                float fi = float(i);
                float ang = 1.6 + (bsH11(fi * 3.1) - 0.5) * 1.3;
                vec2 dir = vec2(cos(ang), sin(ang));
                float kk = 5.0 + fi * 3.6;
                float amp = 0.11 / kk;
                float fade = 1.0 - smoothstep(0.25, 0.9, kk * ps);
                float ph = dot(p, dir) * kk + g_wt * sqrt(9.8 * kk) * 0.6 + fi * 1.7;
                slope += dir * amp * kk * cos(ph) * fade;
            }
            float rp0, rp1;
            splashRing(p, 0.0, rp0); splashRing(p, 1.0, rp1);
            slope *= (1.0 - 0.6 * ww);
            n = normalize(n + vec3(-slope.x, 0.0, -slope.y) * 0.9 + vec3(0.0, 0.0, (rp0 + rp1) * 0.15));
            float cosv = max(dot(n, -rd), 0.0);
            float F = 0.02 + 0.98 * pow(1.0 - cosv, 5.0);
            vec3 rf = reflect(rd, n);
            if (rf.y < 0.0) rf.y = -rf.y * 0.3;
            vec3 refl = sky(normalize(rf), false);
            // reflected glow of crests further out
            vec2 rdir = normalize(rf.xz + 1e-4);
            float gr = 0.0;
            gr += glowAt(p + rdir * 0.8, tmp) * 0.5;
            gr += glowAt(p + rdir * 2.0, tmp) * 0.35;
            gr += glowAt(p + rdir * 3.8, tmp) * 0.2;
            vec3 body = vec3(0.002, 0.008, 0.013) + spill * gcol * 0.06 * gGain;
            // breaking face: dark glassy water lit from inside by the plunging lip behind it
            float face = farSea ? 0.0 : faceSave;
            if (face > 0.01) {
                float lipD = (1.0 - q) * TAU / 0.52;           // distance to the crest (seaward)
                float lu;
                float lu1, lu2;
                float lipG = (glowAt(p + vec2(0.0, lipD + 0.15), lu) + glowAt(p + vec2(0.45, lipD + 0.3), lu1) + glowAt(p + vec2(-0.45, lipD + 0.3), lu2)) / 3.0;
                lu = max(lu, max(lu1, lu2));
                vec3 lc = glowRamp(max(lu, 0.6), 0.0);
                float thick = exp(-lipD * 1.4);
                body = mix(body, vec3(0.0, 0.006, 0.012), face * 0.6) + lc * lipG * gGain * face * (0.25 + 0.9 * thick);
            }
            // swash sheet: a thin film over wet sand — the sand shows through
            float film = farSea ? 0.0 : (1.0 - smoothstep(s0 - 0.6, s0 + 0.4, p.y));
            vec3 wetSand = sandColor.rgb * 0.42 * (0.85 + 0.3 * bsNoise(p * 9.0)) * (amb * 4.0 + spill * gcol * 0.7 * gGain);
            body = mix(body, wetSand, film);
            col = mix(body, refl + gr * gcol * gGain * 0.7 * reflections, F);
            // foam
            float L = seaFoam(vec2(p.x * 1.3 + id * 5.3, q * 9.0 + id * 3.1), 0.9);
            float Lf = seaFoam(p * 3.0 + vec2(0.0, g_wt * 0.3), 0.6);
            float foam = clamp(ww * L * (0.4 + 0.6 * exp(-q * 4.0)) + cu * 0.55 * (0.4 + 0.6 * L), 0.0, 1.0) * (1.0 - 0.7 * face);
            if (p.y < s0 + 0.3 && !farSea) foam = max(foam, exp(-pow((p.y - fr) / 0.3, 2.0)) * (0.5 + 0.5 * Lf) * step(0.15, rr));
            foam *= 1.0 - smoothstep(0.6, 1.4, ps * 30.0) * 0.4;
            vec3 foamCol = amb * 2.2 + spill * gcol * 0.5 * gGain;
            col = mix(col, foamCol, foam * 0.85);
            // emission: the plankton
            vec3 em = gcol * gl * gGain * 0.8 * (0.8 + 0.4 * (1.0 - F)) * (0.3 + 0.7 * max(foam, 0.25)) * (1.0 - 0.5 * face);
            // swash sheet: a torn glowing film, never banded ribbons, never white
            if (film > 0.01) {
                float torn = seaFoam(vec2(p.x * 1.6, (p.y - fr) * 2.4 + g_wt * 0.35), 0.5);
                float along = 0.55 + 0.45 * bsNoise(vec2(p.x * 0.6, p.y * 3.5 - g_wt * 0.5));
                em *= mix(1.0, (0.2 + 0.95 * torn) * along, film);
                em = min(em, gcol * 0.95);
            }
            col += em * (1.0 - 0.3 * film);
            col += sparkles(p, uu * (0.4 + foam), ps) * gGain;
            // far sea: faint whitecap flecks
            if (farSea) {
                float fl = step(0.985, bsH12(floor(p * vec2(0.6, 0.25)))) * (0.5 + 0.5 * sin(g_t * 2.0 + p.x));
                col += gcol * fl * 0.04 * gGain * exp(-t * 0.004);
            }
        } else {
            // sand: dry → wet, footprints, stranded sparkle
            float wm;
            float fstim = footStep(p, wm);
            vec4 sc = texture2D(simB, worldToSim(p));
            float wet = clamp(max(sc.a * 1.15, 0.75 * smoothstep(s0 - 4.5, s0 - 0.3, p.y)), 0.0, 1.0);
            // footprint depression: shading via normal tilt toward the print centre
            float dep = wm * footprints;
            vec3 alb = sandColor.rgb * mix(1.0, 0.45, wet) * (0.85 + 0.3 * bsNoise(p * 9.0));
            float rph = p.x * 6.5 + p.y * 1.7 + 1.3 * bsNoise(p * 0.7);
            float rip = sin(rph) * (1.0 - smoothstep(0.02, 0.06, ps)) * (1.0 - wet * 0.7);
            n = normalize(n + vec3(cos(rph) * 0.18 * (1.0 - smoothstep(0.02, 0.06, ps)), 0.0, 0.0));
            float ndl = 0.6 + 0.4 * n.y + 0.12 * rip;
            col = alb * (amb * ndl * 4.0 + spill * gcol * 0.7 * gGain);
            col *= 1.0 - 0.35 * dep;
            // wet sheen reflects sky + glow
            float cosv = max(dot(n, -rd), 0.0);
            float F = 0.02 + 0.98 * pow(1.0 - cosv, 5.0);
            // roughness grows as the sand drains: fresh = mirror, drying = blurred
            float rough = mix(0.22, 0.015, smoothstep(0.35, 0.95, wet));
            vec3 nm = normalize(vec3(n.x * mix(1.0, 0.25, wet), n.y, n.z * mix(1.0, 0.25, wet)));
            vec3 rf = reflect(rd, nm);
            rf = normalize(vec3(rf.x, abs(rf.y), rf.z));
            // mirror of the glowing breakers: march the reflected ray over the sea plane and take the
            // first strong glow (crests, swash, footprints), blurred by roughness
            vec2 rdir = normalize(rf.xz + 1e-4);
            float slopeR = rf.y / max(length(rf.xz), 1e-3);
            vec3 grc = vec3(0.0); float acc = 0.0;
            for (int k = 0; k < 10; k++) {
                float dk = 0.6 + float(k) * float(k) * 0.22;
                float hgt = dk * slopeR;                          // height of the reflected ray above the sand here
                vec2 sp = p + rdir * dk;
                float ju;
                float g = glowAt(sp + vec2(rough * dk * 0.5 * sin(float(k) * 2.3), 0.0), ju);
                // breakers stand ~0.5–1 m tall: the ray sees them until it climbs above them
                float vis = 1.0 - smoothstep(0.6, 1.4, hgt);
                float a = clamp(g * 1.4, 0.0, 1.0) * vis * (1.0 - acc);
                grc += glowRamp(ju, 0.3) * g * vis * (1.0 - acc);
                acc += a;
            }
            vec3 rsky = sky(rf, rough < 0.05);
            vec3 rcol = rsky + grc * gGain * reflections * 1.2;
            float Fw = mix(F, max(F, 0.55), smoothstep(0.3, 0.95, wet));
            col = mix(col, col * 0.35, wet * 0.6);                // wet sand is darker
            col += rcol * Fw * wet * (0.6 + 0.4 * dep);
            // backwash sheets: thin glowing films sliding back down the slope
            float sheetZone = smoothstep(fr - 0.2, fr + 0.8, p.y) * (1.0 - smoothstep(s0 - 0.6, s0, p.y)) * step(0.15, rr);
            if (sheetZone > 0.0) {
                float lines = seaFoam(vec2(p.x * 2.2, (p.y - fr) * 6.0 + g_wt * 0.8), 0.35);
                col += glowRamp(0.5, 0.2) * sheetZone * lines * gGain * 0.35 * (rdt < 0.0 ? 1.0 : 0.5);
            }
            // plankton stranded where the wave retreated: grains still flickering in the wet sand
            float strand = smoothstep(0.35, 0.9, sc.a) * (1.0 - smoothstep(s0 - 0.5, s0, p.y));
            col += sparkles(p * 1.3, strand * (0.35 + 0.8 * sc.b), ps) * gGain * 1.2;
            // glowing prints / stranded plankton
            col += gcol * gl * gGain * (0.5 + 0.5 * wet) * 0.9;
            col += mix(glowColor.rgb, coreColor.rgb, clamp(g_footGlow - 0.6, 0.0, 1.0)) * g_footGlow * footprints * gGain * 1.1;
            col += sparkles(p, (uu + 0.3 * sc.b) * wet, ps) * gGain * 0.7;
        }
        // distance haze toward the horizon
        float haze = 1.0 - exp(-t * 0.012);
        vec3 hzc = skyHorizon.rgb * 0.7 + townGlow.rgb * 0.35;
        col = mix(col, hzc, haze * 0.85);
    }
    return vec4(col * exposure, cover);
}

// ── main ─────────────────────────────────────────────────────────────────
void main() {
    vec2 fc = gl_FragCoord.xy;
    g_t = TIME * speed;
    g_wt = g_t * 0.95 + 13.3;   // start with a big set mid-break
    g_ar = audioReact;
    g_bass = bassP(); g_mid = midP(); g_high = highP();
    g_beat = clamp(audioBeatPulse, 0.0, 1.0);
    g_lev = clamp(audioLevel, 0.0, 1.0);
    g_set = 1.0 + 0.22 * g_ar * g_bass;

    if (PASSINDEX <= 1) { gl_FragColor = simStep(fc); return; }

    if (PASSINDEX == 2) {
        vec2 uv = (fc - 0.5 * R) / R.y;
        gl_FragColor = renderScene(uv);
        return;
    }

    if (PASSINDEX == 3) {
        // wide bloom prefilter at quarter res
        vec2 st = fc / R;
        vec2 px = 1.0 / R;
        vec3 acc = vec3(0.0); float wsum = 0.0;
        for (int j = -2; j <= 2; j++)
        for (int i = -2; i <= 2; i++) {
            vec2 o = vec2(float(i), float(j));
            float w = exp(-dot(o, o) * 0.35);
            vec2 sp = st + o * px * 1.5;
            float inb = step(0.0, sp.x) * step(sp.x, 1.0) * step(0.0, sp.y) * step(sp.y, 1.0);
            vec3 s = texture2D(shScene, sp).rgb;
            acc += max(s - 0.35, 0.0) * w * inb; wsum += w;
        }
        gl_FragColor = vec4(acc / wsum, 1.0);
        return;
    }

    // final
    vec2 st = fc / R;
    vec2 uvc = (fc - 0.5 * R) / R.y;
    vec2 asp = vec2(R.y / R.x, 1.0);
    vec4 c0 = texture2D(shScene, st);
    float ab = aberration * 0.0018 * length(uvc);
    vec2 dir = normalize(uvc + 1e-4);
    vec3 col = vec3(texture2D(shScene, st + dir * ab * asp).r, c0.g, texture2D(shScene, st - dir * ab * asp).b);
    // tight bloom (fixed golden-angle taps — no per-pixel random = no sand)
    vec3 bl = vec3(0.0);
    for (int i = 0; i < 12; i++) {
        float fi = float(i);
        float a = fi * 2.399963;
        float rr = sqrt((fi + 0.5) / 12.0) * 0.012;
        vec2 of = vec2(cos(a), sin(a)) * rr * asp;
        vec2 sp = st + of;
        float inb = step(0.0, sp.x) * step(sp.x, 1.0) * step(0.0, sp.y) * step(sp.y, 1.0);
        vec3 s = texture2D(shScene, sp).rgb;
        bl += max(s - 0.25, 0.0) * (1.0 - rr / 0.012) * inb;
    }
    bl /= 6.0;
    // wide bloom from the quarter-res buffer
    vec3 wb = vec3(0.0);
    for (int i = 0; i < 9; i++) {
        float fi = float(i);
        float a = fi * 2.399963 + 0.7;
        float rr = sqrt((fi + 0.5) / 9.0) * 0.05;
        vec2 sp = st + vec2(cos(a), sin(a)) * rr * asp;
        float inb = step(0.0, sp.x) * step(sp.x, 1.0) * step(0.0, sp.y) * step(sp.y, 1.0);
        wb += texture2D(shBloom, sp).rgb * inb;
    }
    wb /= 9.0;
    col += (bl * 0.55 + wb * 1.3) * bloom;
    col = aces(col * 1.15);
    float l = lum(col);
    col = mix(vec3(l), col, saturation);
    // vignette (gentle)
    col *= 1.0 - 0.12 * smoothstep(0.55, 1.2, length(uvc * vec2(0.85, 1.0)));
    // HD finisher
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    l = dot(col, vec3(0.299, 0.587, 0.114));
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    // grain
    float gn = bsH12(fc + fract(TIME * 13.7) * 431.0) - 0.5;
    col += gn * grain * 0.035;
    float alpha = transparentBg ? clamp(max(c0.a, lum(col) * 2.0), 0.0, 1.0) : 1.0;
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), alpha);
}
