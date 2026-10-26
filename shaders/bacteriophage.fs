/*{
  "DESCRIPTION": "Bacteriophage — glossy 3D T4 viruses in the style of a scientific animation. Each phage is built like the real thing: a faceted, elongated icosahedral gold head with crisp facet edges, a collar and neck, a ribbed blue tail sheath, a hexagonal baseplate and six long jointed tail fibres that bend at the knee like spider legs. Landing Party: phages drop out of the dark onto the curved, translucent membrane of a rod-shaped bacterium, reach down with their legs, plant their feet, walk a few steps, then the sheath contracts, the hollow tail tube punches through the membrane and a glowing double strand of DNA pours into the cell while ripples spread across the surface; others tumble through space overhead. Hero Phage: one huge phage standing on the cell, close-up and slowly orbiting, flexing its legs and firing its tail. Electron Micrograph: a whole bacterium covered in attached phages, rendered as a grey scanning-electron image with edge glow, depth-of-field softness and heavy grain. Neon Swarm: a lattice of small tumbling phages floating in the dark with glowing neon outlines. Auto Morph dissolves between the forms through a glowing seam. Bass bobs the phages on their knees, beats make sheaths contract and inject, and the drop makes every phage contract at once with a burst of DNA and a shockwave across the membrane. At rest the phages drift calmly with soft dim glows; during a build the legs twitch faster, sheaths tense, the camera closes in and the image trembles; on the drop everything injects together in a bright DNA flash and the peak runs at full density, glow and motion. Physics: every phage is a simulated body with momentum, drag and Brownian jitter that sinks back toward the host under gravity and lands on spring-damped tail-fibre landing gear that squats and rebounds; beats slam them onto their knees, firing the sheath recoils them, and the drop's shockwave blasts them off the membrane into tumbling flight before they drift back down and re-dock.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "3D", "Science", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Landing Party", "Hero Phage", "Electron Micrograph", "Neon Swarm"], "GROUP": "Form" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 6.0, "MAX": 60.0, "DEFAULT": 18.0, "GROUP": "Form" },
    { "NAME": "motion",      "LABEL": "Motion",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "orbit",       "LABEL": "Camera Orbit",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "zoom",        "LABEL": "Zoom",             "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "dnaGlow",     "LABEL": "DNA Glow",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "rimLight",    "LABEL": "Rim Light",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "membraneSee", "LABEL": "Membrane Translucency", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 2, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.03, 0.04, 0.09, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.01, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" },
    { "NAME": "physicsAmt",  "LABEL": "Physics Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "gravity",     "LABEL": "Gravity / Host Pull", "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "bounce",      "LABEL": "Leg Bounce",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Physics" },
    { "NAME": "kickForce",   "LABEL": "Kick Force",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "resetPhysics","LABEL": "Reset Physics",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Physics" }
  ],
  "PASSES": [
    { "TARGET": "phState", "PERSISTENT": true, "FLOAT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}

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

// ── scene state ─────────────────────────────────────────────────────────
float gF;                 // form: 0 landing, 1 hero, 2 SEM, 3 swarm
float gCount;             // phage instances evaluated by map()
float gT, gWalk, gBeatC;
vec3  gMA, gMB; float gMR; // the bacterium: a capsule
// map() results (taken from the last sample, i.e. the march hit)
float gMat, gAux, gHue, gInjH, gGlow, gGlowI;
// phage() local results
float pMat, pAux, pGlow;
// inst() side outputs: analytic hover height (world) and floater flag
float gH, gFloat;

float sdCap(vec3 p, vec3 a, vec3 b, float ra, float rb) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - mix(ra, rb, h);
}

// One T4 phage in its own frame: baseplate at the origin, tail along +y.
// c = sheath contraction, inj = DNA injection, gy = ground height (local),
// L = legs planted (0 flight pose → 1 standing), wa/wp = gait amount/phase.
float phage(vec3 q, float c, float inj, float gy, float L, float wa, float wp) {
    float r2 = length(q.xz);
    float an = atan(q.z, q.x);
    // fold into one of six 60° sectors: one fibre, one baseplate face
    float sec = floor(an / 1.0471976 + 0.5);
    float sa = sec * 1.0471976;
    float cs = cos(sa), sn = sin(sa);
    vec3 qs = vec3(cs * q.x + sn * q.z, q.y, -sn * q.x + cs * q.z);

    float Lsh = 0.95 * (1.0 - 0.5 * c);
    float y0 = 0.075, y1 = y0 + Lsh;
    // ribbed tail sheath (rings pack tighter and the sheath fattens as it contracts)
    float rs = 0.1 + 0.06 * c;
    float ny = (q.y - y0) / Lsh * 20.0;
    float ring = 0.5 + 0.5 * cos(TAU * ny);
    float rsm = rs + 0.016 * ring + 0.006 * cos(6.0 * an + PI * ny);
    float d = max(r2 - rsm, abs(q.y - 0.5 * (y0 + y1)) - 0.5 * Lsh) * 0.8;
    pMat = 3.0; pAux = ring;
    // inner tail tube: pokes out below the baseplate when the sheath contracts
    float ext = 0.95 * 0.5 * c + 0.05;
    float dC = max(r2 - 0.042, abs(q.y - 0.5 * (y1 - ext)) - 0.5 * (y1 + ext));
    if (dC < d) { d = dC; pMat = 4.0; pAux = q.y; }
    // collar + neck
    float yc = y1 + 0.035;
    float dK = length(vec2(max(r2 - 0.16, 0.0), q.y - yc)) - 0.022;
    dK = min(dK, max(r2 - 0.07, abs(q.y - yc - 0.06) - 0.07));
    if (dK < d) { d = dK; pMat = 2.0; pAux = 0.0; }
    // prolate icosahedral head, five-fold axis along the tail
    float hE = 0.13, rr = 0.31;
    float yh = yc + 0.06 + hE + rr * 1.2584 * 0.96;
    vec3 h = q - vec3(0.0, yh, 0.0);
    h.y -= clamp(h.y, -hE, hE);
    vec3 pi = abs(vec3(h.x, 0.5257 * h.y - 0.8507 * h.z, 0.8507 * h.y + 0.5257 * h.z));
    float d1 = dot(pi, vec3(0.57735));
    float d2 = dot(pi, vec3(0.0, 0.93417, 0.35682));
    float d3 = dot(pi, vec3(0.93417, 0.35682, 0.0));
    float d4 = dot(pi, vec3(0.35682, 0.0, 0.93417));
    float m1 = max(max(d1, d2), max(d3, d4));
    float m2 = max(max(min(d1, d2), min(d3, d4)), min(max(d1, d2), max(d3, d4)));
    float dH = m1 - rr;
    if (dH < d) { d = dH; pMat = 1.0; pAux = m1 - m2; }
    // hexagonal baseplate + short pins
    vec2 bq = vec2(qs.x - 0.17, abs(q.y - 0.03) - 0.02);
    float dB = length(max(bq, 0.0)) + min(max(bq.x, bq.y), 0.0) - 0.022;
    dB = min(dB, sdCap(qs, vec3(0.12, 0.0, 0.0), vec3(0.2, -0.12, 0.0), 0.02, 0.014));
    // long tail fibre with a knee (two-bone IK towards the foot target)
    vec3 A = vec3(0.17, 0.045, 0.0);
    float ph = wp + sec * PI + 0.4 * sec;
    float lift = max(sin(ph), 0.0) * 0.2 * wa;
    vec3 Fl = vec3(1.16 + 0.05 * cos(ph) * wa, gy + 0.025 + lift, 0.14 * cos(ph) * wa);
    vec3 Ff = vec3(0.6 + 0.09 * sin(wp * 0.7 + sec * 1.7), -0.68 + 0.08 * cos(wp * 0.9 + sec), 0.1 * sin(wp * 0.5 + sec * 2.3));
    vec3 F = mix(Ff, Fl, L);
    vec3 AF = F - A;
    float dl = length(AF);
    AF *= min(1.0, 1.38 / dl);
    dl = min(dl, 1.38);
    float kh = sqrt(max(0.49 - 0.25 * dl * dl, 0.0));
    vec3 K = A + 0.5 * AF + kh * normalize(vec3(-AF.y, AF.x, 0.0) + 1e-4);
    float dFb = min(sdCap(qs, A, K, 0.03, 0.026), sdCap(qs, K, A + AF, 0.024, 0.011));
    dFb = min(dFb, length(qs - K) - 0.036);
    if (dB < d) { d = dB; pMat = 5.0; pAux = 0.0; }
    if (dFb < d) { d = dFb; pMat = 6.0; pAux = 0.0; }
    // DNA: glowing double helix leaving the tube tip (glow only, never solid)
    pGlow = 1e3;
    if (inj > 0.01) {
        float tip = -ext;
        float len = inj * 0.85 + 0.1 * gDrop;
        float hr = 0.035 + 0.07 * gDrop + 0.02 * smoothstep(tip, tip - len, q.y);
        float ha = q.y * 15.0 + gT * 5.0;
        vec2 hx = hr * vec2(cos(ha), sin(ha));
        float dxz = min(length(q.xz - hx), length(q.xz + hx));
        float dy = max(max(q.y - tip, tip - len - q.y), 0.0);
        pGlow = min(length(vec2(dxz, dy)), max(dC, -0.0) + 0.01);
    }
    return d;
}

// Instance i of the current form: frame (C,U,X), scale s and its life cycle.
void inst(float i, vec3 p, out vec3 C, out vec3 U, out vec3 X, out float s,
          out float c, out float inj, out float gy, out float L, out float wa, out float hs) {
    float ar = audioReact;
    hs = h11(i * 7.13 + gF * 3.1 + 1.7);
    float hb = h11(hs * 17.0 + floor(TIME * 0.5 + hs));          // which phages answer this beat
    float beatC = gBeatC * step(0.4, hb);
    float spin = hs * TAU + gT * 0.12 * (hs - 0.5);
    float h = 0.0;
    U = vec3(0.0, 1.0, 0.0);
    vec3 P = vec3(0.0);
    s = 0.8; c = 0.0; inj = 0.0; L = 1.0; wa = 0.0;
    bool floater = false;
    gH = 0.0;
    if (gF > 2.5) {
        // Neon Swarm: lattice cell around p
        float cs = 3.6;
        vec3 id = floor(p / cs + 0.5);
        hs = h11(dot(id, vec3(1.0, 57.0, 113.0)) * 0.137 + 3.1);
        float dens = mix(0.4, 0.85, 1.0 - gRest);
        s = h11(hs * 31.0) < dens ? 0.72 : 0.0;
        P = id * cs + (vec3(h11(hs * 3.3), h11(hs * 5.7), h11(hs * 9.1)) - 0.5) * 0.4;
        floater = true;
        spin = hs * TAU + gT * 0.3;
    } else if (gF > 1.5) {
        // Electron Micrograph: phages attached all over a rod-shaped cell
        s = 0.42;
        float x = -2.3 + i * 0.66 + (hs - 0.5) * 0.25;
        float th = (h11(hs * 9.1) - 0.5) * 2.6;
        P = vec3(x, 0.0, 0.0) + gMR * vec3(0.0, cos(th), sin(th));
        U = vec3(0.0, cos(th), sin(th));
    } else if (gF > 0.5) {
        // Hero: one big phage standing on the cell, two far floaters
        if (i < 0.5) {
            s = 1.45;
            P = vec3(0.0, gMA.y + gMR, 0.0);
            spin = gT * 0.1;
        } else {
            s = 0.55;
            P = vec3((i - 1.5) * 7.0, 3.2 + 0.4 * sin(gT * 0.2 + i), -7.5 - i * 1.5);
            floater = true;
        }
    } else {
        // Landing Party: four landers along the cell + two tumbling floaters
        if (i < 3.5) {
            s = 0.82;
            float x = (i - 1.5) * 2.2 + (hs - 0.5) * 0.5;
            float th = (h11(hs * 9.1) - 0.5) * 0.4;
            P = vec3(x, gMA.y, 0.0) + gMR * vec3(0.0, cos(th), sin(th));
            U = vec3(0.0, cos(th), sin(th));
        } else {
            s = 0.62;
            P = vec3((i - 4.5) * 5.2 + 0.6 * sin(gT * 0.13 + hs * 6.0), 3.3 + 0.35 * sin(gT * 0.21 + hs * 4.0), -2.8 - (i - 4.0) * 2.0);
            floater = true;
        }
    }
    if (floater) {
        U = normalize(vec3(sin(gT * 0.23 + hs * 6.3), cos(gT * 0.19 + hs * 4.1) + 0.4, sin(gT * 0.17 + hs * 9.7)));
        L = 0.0; wa = 1.0;
        c = beatC;
        inj = max(beatC * 0.7, gDrop);
        if (gF > 2.5) inj = max(inj, 0.35 * gPeak * step(0.7, hb));
        h = 0.0;
    } else if (gF > 0.5 && gF < 1.5) {
        // hero cycle: flex, contract, inject, relax
        float ph = fract(TIME * 0.06 + audioBassTime * 0.01);
        c = smoothstep(0.45, 0.52, ph) * (1.0 - smoothstep(0.85, 0.97, ph));
        inj = smoothstep(0.5, 0.58, ph) * (1.0 - smoothstep(0.8, 0.92, ph));
        wa = (0.35 + 0.5 * gPeak + 0.3 * gBuild) * (1.0 - c) * (1.0 - gRest * 0.7);
        c = max(c, gBeatC * 0.8);
        inj = max(inj, gBeatC * 0.8);
    } else {
        // lander cycle: descend → plant legs → walk → contract + inject → lift away
        float rate = gF > 1.5 ? 0.025 : 0.045;
        float ph = fract(TIME * rate + audioBassTime * 0.006 + hs * 0.83);
        if (gF > 1.5) ph = 0.3 + 0.62 * ph;                       // micrograph: mostly attached
        float desc = 1.0 - smoothstep(0.0, 0.3, ph);
        float up = smoothstep(0.9, 1.0, ph);
        h = (desc * desc * 5.5 + up * up * 7.0) * s;
        wa = smoothstep(0.28, 0.33, ph) * (1.0 - smoothstep(0.46, 0.52, ph));
        P.x += 0.5 * smoothstep(0.3, 0.5, ph) * (hs > 0.5 ? 1.0 : -1.0) * (gF > 1.5 ? 0.0 : 1.0);
        c = smoothstep(0.52, 0.58, ph) * (1.0 - up);
        inj = smoothstep(0.55, 0.62, ph) * (1.0 - smoothstep(0.8, 0.9, ph));
        L = 1.0 - smoothstep(0.1, 1.3, h / s);
        c = max(c, beatC * L);
        inj = max(inj, beatC * 0.8 * L);
    }
    if (!floater) {
        wa = max(wa, (0.25 * gPeak + 0.4 * gBuild) * L * (1.0 - c));
        c = max(c, 0.28 * gBuild * L * (0.8 + 0.2 * sin(TIME * 23.0 + hs * 9.0)));
        h += (bassP() * ar * 0.12 + 0.03 * sin(gT * 1.3 + hs * 6.0)) * s * L * (1.0 - 0.6 * gRest);
    }
    c = max(c, gDrop);
    inj = max(inj, gDrop) * (1.0 - 0.45 * gRest);
    gy = -h / s - 0.7 * s / gMR;
    gH = h; gFloat = floater ? 1.0 : 0.0;
    C = P + U * h;
    vec3 ref = abs(U.z) < 0.9 ? vec3(0.0, 0.0, 1.0) : vec3(1.0, 0.0, 0.0);
    vec3 X0 = normalize(cross(U, ref));
    vec3 Z0 = cross(X0, U);
    X = X0 * cos(spin) + Z0 * sin(spin);
}

// ── physics state: 8 bodies × 3 rows (pos+tilt, vel+spin, edge memory) ──
#define NB 8.0
#define H0 0.22
vec3 rotAx(vec3 v, vec3 k, float a) {
    float c = cos(a), s = sin(a);
    return v * c + cross(k, v) * s + k * dot(k, v) * (1.0 - c);
}
// inst() + the simulated body: lateral drift, physical height on the legs, tumble
void instP(float i, vec3 p, out vec3 C, out vec3 U, out vec3 X, out float s,
           out float c, out float inj, out float gy, out float L, out float wa, out float hs) {
    inst(i, p, C, U, X, s, c, inj, gy, L, wa, hs);
    float pa = physicsAmt;
    if (pa < 0.001 || s <= 0.0) return;
    bool sw = gF > 2.5;
    float bi = sw ? floor(hs * 7.99) : i;
    vec4 st = texture2D(phState, vec2((bi + 0.5) / NB, 0.5 / 3.0));
    float kF = sw ? 0.35 : (gF > 0.5 && gF < 1.5 && i < 0.5) ? 0.35 : 1.0;
    vec3 Z = cross(X, U);
    vec3 P = C - U * gH;
    vec3 lat = (X * st.x + Z * st.z) * s * kF * pa;
    if (sw) lat = clamp(lat, -0.35, 0.35);
    float H;
    if (gFloat > 0.5) H = gH / s + clamp(st.y * kF, -1.2, 1.2) * pa;
    else H = mix(gH / s, H0 + (st.y - H0) * kF, pa);
    float ang = st.w * pa * (kF < 0.5 && !sw ? 0.45 : 1.0);   // the close-up hero tilts less
    vec3 ka = normalize(vec3(h11(bi * 3.17 + 0.3) - 0.5, 0.3 * (h11(bi * 5.3 + 0.1) - 0.5), h11(bi * 8.9 + 0.7) - 0.5));
    ka = normalize(X * ka.x + U * ka.y + Z * ka.z);
    vec3 U2 = rotAx(U, ka, ang), X2 = rotAx(X, ka, ang);
    C = P + U * (H * s) + lat + (U - U2) * (0.9 * s);
    if (gFloat < 0.5) {
        float Lp = (1.0 - smoothstep(0.12, 1.0, H - H0)) * smoothstep(0.55, 0.95, cos(ang));
        L = mix(L, Lp, pa);
        gy = -H - 0.7 * s / gMR;
    }
    U = U2; X = X2;
}

float map(vec3 p) {
    float d = 1e3;
    gGlow = 1e3; gGlowI = 0.0;
    gMat = 0.0; gAux = 0.0; gHue = 0.0; gInjH = 0.0;
    for (int k = 0; k < 8; k++) {
        float i = float(k);
        if (i > gCount - 0.5) break;
        vec3 C, U, X; float s, c, inj, gy, L, wa, hs;
        instP(i, p, C, U, X, s, c, inj, gy, L, wa, hs);
        if (s <= 0.0) continue;
        float db = length(p - C - U * (0.55 * s)) - 2.0 * s;
        if (db > d) continue;
        if (db > 0.3 * s) { d = min(d, db + 0.15 * s); gGlow = min(gGlow, db + 0.15 * s); continue; }
        vec3 r = p - C;
        vec3 Z = cross(X, U);
        vec3 q = vec3(dot(r, X), dot(r, U), dot(r, Z)) / s;
        float dl = phage(q, c, inj, gy, L, wa, gWalk + hs * 5.0) * s;
        if (dl < d) { d = dl; gMat = pMat; gAux = pAux; gHue = hs; gInjH = inj; }
        if (pGlow * s < gGlow) { gGlow = pGlow * s; gGlowI = inj; }
    }
    if (gF > 2.5) d = min(d, 0.45);
    return d;
}

// analytic ray / capsule (the bacterium)
float capHit(vec3 ro, vec3 rd, vec3 pa, vec3 pb, float r) {
    vec3 ba = pb - pa, oa = ro - pa;
    float baba = dot(ba, ba), bard = dot(ba, rd), baoa = dot(ba, oa), rdoa = dot(rd, oa), oaoa = dot(oa, oa);
    float a = baba - bard * bard;
    float b = baba * rdoa - baoa * bard;
    float c = baba * oaoa - baoa * baoa - r * r * baba;
    float h = b * b - a * c;
    if (h >= 0.0) {
        float t = (-b - sqrt(h)) / a;
        float y = baoa + t * bard;
        if (y > 0.0 && y < baba) return t;
        vec3 oc = (y <= 0.0) ? oa : ro - pb;
        b = dot(rd, oc); c = dot(oc, oc) - r * r; h = b * b - c;
        if (h > 0.0) return -b - sqrt(h);
    }
    return -1.0;
}

vec3 neon(float h) {
    h = fract(h) * 4.0;
    if (h < 1.0) return vec3(1.0, 0.15, 0.75);
    if (h < 2.0) return vec3(0.1, 0.9, 1.0);
    if (h < 3.0) return vec3(0.55, 1.0, 0.2);
    return vec3(0.6, 0.3, 1.0);
}
vec3 phageCol(float m, float aux) {
    if (m < 1.5) return vec3(1.0, 0.68, 0.22);                   // head: gold capsid
    if (m < 2.5) return vec3(0.92, 0.88, 0.8);                  // collar
    if (m < 3.5) return mix(vec3(0.12, 0.22, 0.7), vec3(0.35, 0.55, 1.0), aux); // ribbed sheath
    if (m < 4.5) return vec3(0.8, 0.85, 0.9);                   // tube
    if (m < 5.5) return vec3(0.95, 0.72, 0.38);                 // baseplate
    return vec3(1.0, 0.82, 0.5);                                 // fibres
}

void setForm(float form) {
    gF = form;
    gCount = form < 0.5 ? 6.0 : form < 1.5 ? 3.0 : form < 2.5 ? 8.0 : 1.0;
    // the bacterium
    if (form < 0.5) { gMR = 7.0; gMA = vec3(-60.0, -7.0, 0.0); gMB = vec3(60.0, -7.0, 0.0); }
    else if (form < 1.5) { gMR = 7.0; gMA = vec3(0.0, -7.0, 0.0); gMB = vec3(0.0, -7.0, 0.001); }
    else            { gMR = 1.5; gMA = vec3(-2.5, 0.0, 0.0);   gMB = vec3(2.5, 0.0, 0.0); }
}

vec4 renderScene(float form, vec2 sp, vec2 uv) {
    float ar = audioReact;
    setForm(form);

    // ── camera ──
    float tr = smoothstep(0.7, 1.0, gBuild);
    vec3 shake = (vec3(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0), 0.0) - 0.5) * 0.06 * tr;
    vec3 ro, ta;
    float fl = 1.7;
    float az, el, dist;
    float zm = (1.0 - 0.14 * gBuild + 0.12 * gRest - 0.05 * gPeak) / zoom;
    if (form < 0.5) {
        az = 0.45 * sin(TIME * 0.045 + audioMidTime * 0.02 * ar) * orbit;
        el = 0.3 + 0.06 * sin(TIME * 0.07);
        dist = 9.5 * zm;
        ta = vec3(0.0, 0.75, 0.0);
        fl = 1.3;
    } else if (form < 1.5) {
        az = (TIME * 0.09 + audioMidTime * 0.03 * ar) * orbit + 0.4;
        el = 0.12 + 0.1 * sin(TIME * 0.06);
        dist = 6.8 * zm;
        ta = vec3(0.0, 1.5, 0.0);
        fl = 1.55;
    } else if (form < 2.5) {
        az = 0.35 * sin(TIME * 0.03) * orbit + 0.15;
        el = 0.45;
        dist = 10.0 * zm;
        ta = vec3(0.0, 0.25, 0.0);
    } else {
        az = 0.0; el = 0.0; dist = 1.0;
    }
    if (form < 2.5) {
        ro = ta + dist * vec3(sin(az) * cos(el), sin(el), cos(az) * cos(el));
    } else {
        float zt = TIME * 0.35 * motion + audioBassTime * 0.12 * ar;
        ro = vec3(1.8 + 0.5 * sin(TIME * 0.11), 1.8 + 0.4 * cos(TIME * 0.09), -zt);
        ta = ro + vec3(0.6 * sin(TIME * 0.07) * orbit, 0.3 * sin(TIME * 0.05), -2.0);
        fl = 1.4 / zm;
    }
    ta += shake;
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    if (form > 2.5) { float rl = 0.25 * sin(TIME * 0.04); vec3 u2 = uu * cos(rl) + vv * sin(rl); vv = cross(u2, ww); uu = u2; }
    vec3 rd = normalize(sp.x * uu + sp.y * vv + fl * ww);
    vec3 lKey = normalize(-uu * 0.5 + vv * 0.8 - ww * 0.35);
    vec3 lRim = normalize(uu * 0.3 + vv * 0.45 + ww * 0.85);

    // ── membrane (analytic) + march ──
    float tM = form < 2.5 ? capHit(ro, rd, gMA, gMB, gMR) : -1.0;
    bool sem = form > 1.5 && form < 2.5;
    float tEnd = tM > 0.0 ? (sem ? tM : tM + 2.2) : (form > 2.5 ? 22.0 : 40.0);
    float tMm = tM > 0.0 ? tM : 1e9;
    float t = 0.05, hitT = -1.0;
    float accA = 0.0, accB = 0.0;
    vec3 accN = vec3(0.0);
    float dmin = 1e3, tmin = 0.0;
    float steps = 0.0;
    for (int i = 0; i < 100; i++) {
        vec3 p = ro + rd * t;
        float d = map(p);
        float gw = exp(-gGlow * 22.0) * gGlowI;
        float stp = max(min(d * 0.9, gGlow + 0.035), 0.004);
        if (t < tMm) accA += gw * stp; else accB += gw * stp;
        if (form > 2.5) accN += neon(gHue) * exp(-max(d, 0.0) * 30.0) * stp * 0.9 * exp(-t * 0.16);
        if (d < dmin && t < tMm) { dmin = d; tmin = t; }
        if (d < 0.0012 * t) { hitT = t; break; }
        t += stp;
        steps += 1.0;
        if (t > tEnd) break;
    }
    float hMat = gMat, hAux = gAux, hHue = gHue, hInj = gInjH;
    vec3 n = vec3(0.0, 1.0, 0.0);
    vec3 hp = ro + rd * hitT;
    if (hitT > 0.0) {
        n = vec3(0.0);
        float eps = 0.0007 * hitT + 0.0004;
        for (int i = 0; i < 4; i++) {
            vec3 e = i == 0 ? vec3(1.0, -1.0, -1.0) : i == 1 ? vec3(-1.0, -1.0, 1.0) : i == 2 ? vec3(-1.0, 1.0, -1.0) : vec3(1.0);
            n += e * map(hp + e * eps);
        }
        n = normalize(n);
    }

    // ── shading ──
    vec3 bg = backdrop(uv);
    vec3 dnaC = vec3(0.35, 1.0, 0.85);
    float glowK = dnaGlow * (0.75 + 0.6 * gPeak - 0.35 * gRest + 2.5 * gDrop) * (1.0 + 0.35 * sin(TIME * 21.0) * gBuild);
    if (sem) {
        // grey substrate with speckle
        float sub = 0.1 + 0.08 * vnoise(sp * 9.0) + 0.05 * vnoise(sp * 37.0);
        bg = vec3(sub);
    } else if (!transparentBg) {
        // drifting debris for depth
        vec2 g = sp * 7.0 + vec2(TIME * 0.05, TIME * 0.02);
        vec2 id = floor(g);
        vec2 o = vec2(h12(id), h12(id + 3.1));
        float dd = length(fract(g) - o);
        bg += vec3(0.25, 0.4, 0.7) * smoothstep(0.08, 0.0, dd) * step(0.75, h12(id + 7.7)) * 0.25;
        if (form < 1.5) bg += vec3(0.25, 0.1, 0.35) * 0.18 * smoothstep(0.35, -0.3, sp.y);
        else bg += (neon(0.3 + 0.1 * sin(TIME * 0.05)) * 0.05 + vec3(0.04, 0.0, 0.06)) * vnoise(sp * 2.0 + TIME * 0.03) * (0.6 + 0.6 * gPeak);
    }
    vec3 col = bg;
    float alpha = 0.0;
    float ao = 1.0 - 0.5 * smoothstep(20.0, 95.0, steps);
    vec3 phC = vec3(0.0);
    if (hitT > 0.0) {
        vec3 base = phageCol(hMat, hAux);
        float dif = max(dot(n, lKey), 0.0);
        float wrap = 0.5 + 0.5 * dot(n, lKey);
        float vdn = max(dot(n, -rd), 0.0);
        float fres = pow(1.0 - vdn, 3.0);
        float spec = pow(max(dot(n, normalize(lKey - rd)), 0.0), 60.0);
        float sp2  = pow(max(dot(reflect(rd, n), lRim), 0.0), 12.0);
        float occ = ao;
        if (hMat > 2.5 && hMat < 3.5) occ *= 0.65 + 0.35 * hAux;      // sheath grooves
        if (form < 2.5) {
            // analytic AO: crevice where legs/baseplate meet the membrane, head self-shade on the tail
            vec3 ba0 = gMB - gMA;
            vec3 ax0 = gMA + ba0 * clamp(dot(hp - gMA, ba0) / dot(ba0, ba0), 0.0, 1.0);
            float dMem = length(hp - ax0) - gMR;
            float aoR = sem ? 0.25 : 0.7;
            occ *= 0.35 + 0.65 * smoothstep(-0.05 * aoR, aoR, dMem) + 0.2 * max(dot(n, normalize(hp - ax0)), 0.0) * (1.0 - smoothstep(0.0, aoR, dMem));
            if (hMat > 1.5 && hMat < 4.5) dif *= 0.55 + 0.45 * max(n.y, 0.0);
        }
        if (hMat < 1.5) {
            float edge = smoothstep(0.0, 0.012, hAux);
            base *= 0.75 + 0.25 * edge;
            spec *= 0.6 + 0.8 * edge;
        }
        if (sem) {
            // secondary-electron look: edges glow, flat faces darker
            float v = 0.12 + 0.75 * pow(1.0 - vdn, 1.6) + 0.3 * dif;
            v *= occ * (hMat < 1.5 ? 1.1 : hMat > 5.5 ? 0.95 : 1.0);
            float foc = smoothstep(0.0, 2.5, abs(hitT - 9.8 * zm));
            v = mix(v, 0.38, foc * 0.45);
            phC = vec3(v);
        } else if (form > 2.5) {
            vec3 nc = neon(hHue);
            phC = base * 0.12 * (0.2 + dif) + nc * (0.12 + 0.3 * pow(1.0 - vdn, 1.5) + 1.6 * pow(1.0 - vdn, 3.5)) * (1.1 + 0.5 * gPeak) * rimLight;
            phC += vec3(1.0) * spec * 0.5;
            phC *= occ;
        } else {
            phC = base * (0.08 + 0.95 * dif * (0.55 + 0.45 * wrap)) * occ;
            phC += base * vec3(0.55, 0.45, 0.6) * 0.5 * (0.4 + 0.6 * max(dot(n, -lKey * vec3(-1.0, 1.0, 1.0)), 0.0) + 0.3 * n.y) * occ;
            vec3 rf = reflect(rd, n);
            vec3 env = mix(vec3(0.5, 0.2, 0.45), vec3(1.0, 0.95, 0.9), smoothstep(-0.3, 0.6, dot(rf, vv)));
            phC += env * base * (0.15 + 0.5 * fres) * occ;
            phC += vec3(1.0, 0.97, 0.92) * (spec * 1.4 + 0.5 * pow(max(dot(rf, lKey), 0.0), 8.0) * 0.4) * occ;
            phC += vec3(0.5, 0.75, 1.0) * (fres * 1.1 + sp2 * 0.5) * rimLight;
        }
        if (hMat > 3.5 && hMat < 4.5) phC += (sem ? vec3(0.6) : dnaC) * hInj * glowK * 0.9;   // tube lights up while injecting
        // fog
        // aerial perspective: distant phages sink into a cool haze
        float fogK = form > 2.5 ? 0.12 : 0.025;
        vec3 haze = sem ? bg : mix(bg, vec3(0.16, 0.2, 0.34), 0.35);
        phC = mix(phC, haze, 1.0 - exp(-max(hitT - (form > 2.5 ? 2.5 : 4.0), 0.0) * fogK));
    }
    if (tM > 0.0 && (hitT < 0.0 || hitT > tM)) {
        // the bacterial cell envelope
        vec3 mp = ro + rd * tM;
        vec3 ba = gMB - gMA;
        vec3 ax = gMA + ba * clamp(dot(mp - gMA, ba) / dot(ba, ba), 0.0, 1.0);
        vec3 mn = normalize(mp - ax);
        vec2 suv = vec2(mp.x, atan(mn.z, mn.y) * gMR);
        // lipid bead texture + slow undulation, as an analytic bump
        float kb = sem ? 9.0 : 7.0;
        vec2 bead = vec2(cos(suv.x * kb) * sin(suv.y * kb), sin(suv.x * kb) * cos(suv.y * kb)) * 0.06;
        float wv = vnoise(suv * 0.9 + gT * 0.05);
        vec2 grad = bead + vec2(vnoise(suv * 0.9 + vec2(0.1, 0.0) + gT * 0.05) - wv, vnoise(suv * 0.9 + vec2(0.0, 0.1) + gT * 0.05) - wv) * (sem ? 6.0 : 3.0);
        vec3 tX = vec3(1.0, 0.0, 0.0);
        vec3 tY = normalize(cross(mn, tX));
        vec3 bn = normalize(mn - tX * grad.x - tY * grad.y);
        // contact shadows under phages, injection ripples, drop shockwave
        float shad = 1.0, rip = 0.0;
        for (int k = 0; k < 8; k++) {
            float i = float(k);
            if (i > gCount - 0.5) break;
            vec3 C, U, X; float s, c, inj, gy, L, wa, hs;
            instP(i, mp, C, U, X, s, c, inj, gy, L, wa, hs);
            if (L < 0.01 || s <= 0.0) continue;
            vec3 P = C + U * (gy * s);
            float dd = length(mp - P) / s;
            shad *= 1.0 - 0.55 * L * exp(-dd * dd * 1.6);
            // soft key-light shadow of the capsid and sheath (analytic sphere occluders)
            for (int j = 0; j < 2; j++) {
                vec3 oc = mp - (C + U * ((j == 0 ? 1.6 : 0.6) * s));
                float rS = (j == 0 ? 0.38 : 0.16) * s;
                float al = -dot(oc, lKey);
                float pr = length(oc + lKey * al);
                if (al > 0.0) shad *= mix(1.0, smoothstep(rS * 0.5, rS + 0.12 * al, pr), 0.7);
            }
            float rr = fract(TIME * 0.5 + hs);
            rip += inj * exp(-pow((dd - rr * 2.4) * 5.0, 2.0)) * (1.0 - rr);
            rip += gDrop * exp(-pow((dd - (1.0 - gDrop) * 6.0) * 2.5, 2.0)) * 1.5;
            rip += inj * 0.6 * exp(-dd * dd * 18.0);
        }
        float vdn = max(dot(bn, -rd), 0.0);
        float fres = pow(1.0 - vdn, 2.5);
        float dif = max(dot(bn, lKey), 0.0);
        float spec = pow(max(dot(bn, normalize(lKey - rd)), 0.0), 40.0);
        vec3 mc;
        float mAlpha;
        if (sem) {
            mc = vec3((0.1 + 0.6 * pow(1.0 - vdn, 1.4) + 0.35 * dif) * shad);
            mc += vec3(0.5) * rip * glowK * 0.3;
            float foc = smoothstep(0.0, 3.0, abs(tM - 9.8 * zm));
            mc = mix(mc, vec3(0.32), foc * 0.4);
            mAlpha = 1.0;
        } else {
            vec3 memA = vec3(0.6, 0.18, 0.55), memB = vec3(0.95, 0.5, 0.75);
            float bh = sin(suv.x * kb) * sin(suv.y * kb);
            mc = mix(memA, memB, 0.35 + 0.35 * sin(suv.x * 0.6 + wv * 3.0) + 0.3 * bh) * (0.08 + 0.6 * dif) * shad;
            mc += vec3(1.0, 0.85, 0.95) * spec * 0.9 * shad;
            mc += vec3(0.95, 0.6, 1.0) * fres * 0.55 * rimLight;
            // subsurface glow: light bleeding through the soft envelope past the terminator
            mc += memB * vec3(1.0, 0.55, 0.7) * 0.2 * smoothstep(-0.5, 0.25, dot(bn, lKey)) * (1.0 - dif) * (0.5 + 0.5 * shad);
            mc += dnaC * rip * glowK * 0.35;
            mc = mix(mc, bg, 1.0 - exp(-max(tM - 6.0, 0.0) * 0.03));
            mAlpha = mix(1.0, 0.45, membraneSee * (1.0 - fres));
        }
        // seen through the envelope: cytoplasm, the tail tubes, the DNA
        vec3 inside = vec3(0.12, 0.03, 0.14) + vec3(0.25, 0.05, 0.2) * vnoise(suv * 3.0);
        if (hitT > 0.0) inside = mix(inside, phC, 0.6);
        inside += dnaC * accB * glowK * 3.0;
        col = mix(inside, mc, mAlpha);
        alpha = 1.0;
    } else if (hitT > 0.0) {
        col = phC;
        alpha = 1.0;
    }
    // near-miss halo: depth of field softness (SEM) / crisp silhouettes
    // + pixel-footprint coverage AA on every silhouette, and a shallow depth of field
    if (hitT < 0.0 && form < 2.5 && tmin < tMm) {
        float pix = 1.4 / (RENDERSIZE.y * fl);
        float coc = sem ? 0.004 + 0.03 * smoothstep(0.0, 3.0, abs(tmin - 9.8 * zm))
                        : pix + 0.006 * smoothstep(1.5, 9.0, abs(tmin - dist));
        float cov = (1.0 - smoothstep(0.0, coc * tmin, dmin)) * (sem ? 0.6 : 0.7);
        col = mix(col, sem ? vec3(0.4) : vec3(0.5, 0.62, 0.95) * 0.55 * rimLight, cov);
        alpha = max(alpha, cov);
    }
    // DNA glow in front of everything
    vec3 gcol = sem ? vec3(0.85) : dnaC;
    col += gcol * accA * glowK * (sem ? 0.9 : 2.4);
    col += accN * (0.7 + 0.5 * gPeak - 0.1 * gRest + 1.2 * gDrop) * rimLight;
    alpha = max(alpha, clamp(accA * glowK + lum(accN), 0.0, 1.0));
    if (sem) {
        col = vec3(lum(col));
        col += (h12(sp * 911.0 + fract(TIME * 7.3) * 37.0) - 0.5) * 0.12;
        col *= 0.92 + 0.08 * sin(uv.y * RENDERSIZE.y * 1.4);
        col = pow(max(col, 0.0), vec3(0.9)) * 1.1;
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
    gT = (TIME * 0.6 + audioMidTime * 0.25 * ar) * motion + 1.5 * gBuild * gBuild;
    gWalk = (TIME * 2.2 + audioBassTime * 1.0 * ar) * motion + 3.0 * gBuild * gBuild;
    gBeatC = smoothstep(0.1, 0.8, audioBeatPulse) * ar;

    if (PASSINDEX == 0) {
        // ── physics update: one body per cell, computed only near cell centres ──
        vec2 cid = floor(uv * vec2(NB, 3.0));
        vec2 cc = (cid + 0.5) / vec2(NB, 3.0) * R;
        if (max(abs(fc.x - cc.x), abs(fc.y - cc.y)) > 3.0) { gl_FragColor = vec4(0.0); return; }
        float bi = cid.x;
        vec4 s0 = texture2D(phState, vec2((bi + 0.5) / NB, 0.5 / 3.0));
        vec4 s1 = texture2D(phState, vec2((bi + 0.5) / NB, 1.5 / 3.0));
        vec4 s2 = texture2D(phState, vec2((bi + 0.5) / NB, 2.5 / 3.0));
        // dominant form (the morph seam is per pixel; physics follows the majority)
        float fs = float(formSel), fm = fs - 1.0;
        if (fs < 0.5) {
            float ph = TIME / morphPeriod;
            float fA = mod(floor(ph), 4.0);
            float w = smoothstep(1.0 - 2.5 / morphPeriod, 1.0, fract(ph));
            fm = w > 0.5 ? mod(fA + 1.0, 4.0) : fA;
        }
        setForm(fm);
        vec3 C, U, X; float s, c, inj, gy, L, wa, hs;
        inst(bi, vec3(0.0), C, U, X, s, c, inj, gy, L, wa, hs);
        bool flo = gFloat > 0.5 || s <= 0.0;
        float hA = s > 0.0 ? gH / s : 0.0;
        vec3 o = s0.xyz, v = s1.xyz;
        float a = s0.w, w = s1.w;
        float beat = gBeatC, drop = gDrop;
        float chk = abs(o.x) + abs(o.y) + abs(o.z) + abs(a) + abs(v.x) + abs(v.y) + abs(v.z) + abs(w);
        bool bad = FRAMEINDEX < 2 || resetPhysics || s2.w < 0.5 || !(chk < 1e3);
        if (bad) {
            o = vec3(0.0, flo ? 0.0 : H0 + hA, 0.0); v = vec3(0.0); a = 0.0; w = 0.0;
        } else {
            float dt = clamp(TIMEDELTA, 0.0, 0.1);
            float kf = kickForce;
            float hb = h11(hs * 17.0 + floor(TIME * 0.5 + hs));
            float sel = step(0.4, hb);
            vec3 kr = vec3(h11(hb * 13.1 + bi), h11(hb * 29.7 + bi), h11(hb * 41.3 + bi)) - 0.5;
            float hd = h11(bi * 4.7 + floor(TIME * 0.25) * 1.3);
            vec3 kd = vec3(h11(hd * 7.1), h11(hd * 3.9), h11(hd * 5.3)) - 0.5;
            float dB = max(beat - s2.y, 0.0) * sel;
            float dD = max(drop - s2.z, 0.0);
            float dC = max(c - s2.x, 0.0);
            float fi = mod(float(FRAMEINDEX), 977.0);
            vec3 nz = vec3(h11(fi * 1.31 + bi * 7.7), h11(fi * 2.17 + bi * 3.3), h11(fi * 0.73 + bi * 5.9)) - 0.5;
            float brown = (0.5 + 1.1 * gBuild + 0.7 * gPeak) * (flo ? 2.0 : 1.0);
            float drag = 1.15 - 0.35 * gPeak;
            vec3 F = vec3(0.0);
            float ground = 0.0;
            if (flo) {
                // floaters: soft tether to their analytic drift path
                v += (kr * 3.5 * dB + kd * 6.0 * dD) * kf;
                w += (kr.y * 3.0 * dB + kd.y * 10.0 * dD) * kf;
            } else {
                // docked: gravity/host pull, guided lift-off, spring-damped tail-fibre legs
                ground = 1.0 - smoothstep(H0, H0 + 1.5, o.y);
                v.y += (-2.6 * dB * ground + dC * 2.4) * kf + dC * 0.8;   // kick slams down, firing recoils up
                v.xz += kr.xz * 1.2 * dB * kf;
                w += kr.y * 4.0 * dB * kf;
                // drop shockwave: blast off the host, with spin
                v.y += dD * (4.0 + 2.5 * hd) * kf;
                v.xz += kd.xz * 6.0 * dD * kf;
                w += (kd.y > 0.0 ? 1.0 : -1.0) * (9.0 + 8.0 * abs(kd.y)) * dD * kf;
            }
            v += nz * brown * 1.6 * sqrt(dt);
            // 4 semi-implicit substeps (forces re-evaluated) — stable up to 0.1 s frames
            float h4 = dt * 0.25;
            float gg = 2.5 * gravity * (1.0 - 0.4 * gRest + 0.35 * gBuild);
            float kL = 55.0 + 45.0 * gBuild, cL = mix(14.0, 2.2, bounce);
            for (int k = 0; k < 4; k++) {
                if (flo) F = -2.6 * o - drag * v;
                else {
                    F.xz = -1.4 * o.xz - drag * v.xz;
                    F.y = -gg - drag * v.y + 8.0 * max(hA + H0 - o.y, 0.0);
                    if (o.y < H0) { F.y += kL * (H0 - o.y) - cL * v.y; F.xz -= 5.0 * v.xz; }
                    ground = 1.0 - smoothstep(H0, H0 + 1.5, o.y);
                }
                w += (-(0.5 + 9.0 * ground) * sin(a) - (0.4 + 2.6 * ground) * w) * h4;
                v += F * h4;
                o += v * h4;
                a += w * h4;
                if (!flo && o.y < 0.03) { o.y = 0.03; v.y = max(v.y, 0.0); }
            }
            float vl = length(v);
            if (vl > 14.0) v *= 14.0 / vl;
            w = clamp(w, -25.0, 25.0);
            if (flo) o = clamp(o, vec3(-3.0), vec3(3.0));
            else {
                if (o.y < 0.03) { o.y = 0.03; v.y = max(v.y, 0.0); }
                o = clamp(o, vec3(-5.0, 0.03, -5.0), vec3(5.0, 9.0, 5.0));
            }
            a = mod(a + PI, TAU) - PI;
        }
        float row = cid.y;
        gl_FragColor = row < 0.5 ? vec4(o, a) : row < 1.5 ? vec4(v, w) : vec4(c, beat, drop, 1.0);
        return;
    }

    // form morph: per-pixel dissolve through a glowing seam
    float fsel = float(formSel);
    float form = fsel - 1.0;
    float seam = 0.0;
    if (fsel < 0.5) {
        float ph = TIME / morphPeriod;
        float fA = mod(floor(ph), 4.0);
        float fB = mod(fA + 1.0, 4.0);
        float w = smoothstep(1.0 - 2.5 / morphPeriod, 1.0, fract(ph));
        float nz = 0.65 * vnoise(sp * 3.5 + 1.7) + 0.35 * vnoise(sp * 9.0) + 0.25 * length(sp);
        float thr = w * 1.45 - 0.1;
        form = nz < thr ? fB : fA;
        seam = exp(-abs(nz - thr) * 40.0) * step(0.001, w) * step(w, 0.999);
    }
    vec4 S = renderScene(form, sp, uv);
    vec3 col = S.rgb;
    float alpha = S.a;
    col += vec3(0.35, 1.0, 0.85) * seam * 1.2;
    alpha = max(alpha, seam);
    col += vec3(0.6, 1.0, 0.9) * gDrop * gDrop * 0.12;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    col *= 1.0 - 0.25 * dot(sp, sp);
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
