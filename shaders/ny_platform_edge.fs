/*{
  "DESCRIPTION": "Platform Edge — Shades of New York 002. The yellow tactile warning strip at the edge of every subway platform, the last thing under your feet before the train doors: square tiles of truncated domes in staggered rows, rendered as glossy moulded resin under the station's fluorescent tubes. Every dome is lit in 3D, with a flat worn top, a sloped shoulder, a specular crescent from the overhead lights, a reflection of the tube strips, a soft shadow and contact occlusion on the tile, and real platform wear: scuffed tops, chipped shoulders, grime, gum spots, scratches through the coating and dark grout between tiles. The yellow is pushed more saturated and contrasty than the real thing. The domes move: they breathe, pump with the bass and ripple in waves that change form (Ripples, Train Wave, Rain Pops, Checker Pulse, Spiral, or Auto). Each dome is its own little piston, so the strip reads like a tactile display. The colour story drifts too: the strip holds its yellow, then fades into subway line colours (orange, red, green, blue, lime, purple, brown) as sweeping bands, a per-tile mosaic or a whole-floor fade, before returning to yellow. Domes Only keeps the tile yellow while the domes change colour, and Dome Glow lights them from inside like LEDs. Every so often a train passes: a headlight sweeps across the floor and the station rumbles. Camera Domes turns the strip into a tactile pixel screen, with each dome taking the colour of the camera image beneath it and rising with its brightness. Your mouse, or your hand in ShaderClaw, is a flashlight; press or pinch to step on the domes and send a ripple out from your foot. Camera Tilt goes from straight down to a low view along the platform. Audio: bass pumps the domes, beats send a ripple ring through the floor, mids drive the waves and the colour drift, highs sparkle the gloss. Show dynamics: at Rest the floor barely breathes in pure yellow; the Build raises the domes row by row as a train approaches, its headlight growing while the station shakes; the Drop punches every dome up in a shockwave that flips the floor to a line colour with a glowing flash; the Peak keeps everything pumping.",
  "CREDIT": "Easel / ShaderClaw house shader — Shades of New York series, 2026-11-11 — original.",
  "CATEGORIES": ["Generator", "Shades of New York", "Texture", "3D", "Interactive", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "domeRows",    "LABEL": "Dome Rows On Screen", "TYPE": "float", "AUDIO": false, "MIN": 5.0, "MAX": 40.0, "DEFAULT": 14.0, "GROUP": "Floor" },
    { "NAME": "tilt",        "LABEL": "Camera Tilt",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Floor" },
    { "NAME": "drift",       "LABEL": "Walk / Drift",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Floor" },
    { "NAME": "sway",        "LABEL": "Sway",             "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Floor" },
    { "NAME": "domeHeight",  "LABEL": "Dome Height",      "TYPE": "float", "MIN": 0.2, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Floor" },
    { "NAME": "waveMode",    "LABEL": "Wave", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Auto", "Ripples", "Train Wave", "Rain Pops", "Checker Pulse", "Spiral"], "GROUP": "Pump" },
    { "NAME": "pumpAmt",     "LABEL": "Pump Amount",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Pump" },
    { "NAME": "waveSpeed",   "LABEL": "Wave Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Pump" },
    { "NAME": "breathe",     "LABEL": "Breathing",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Pump" },
    { "NAME": "dimples",     "LABEL": "Sink Below Floor", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Pump" },
    { "NAME": "morphPeriod", "LABEL": "Auto Wave Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 14.0, "GROUP": "Pump" },
    { "NAME": "colorMode",   "LABEL": "Colour Story", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Auto Story", "Platform Yellow", "Line Colour Fade", "Line Mosaic", "Line Waves", "Camera Domes"], "GROUP": "Colour" },
    { "NAME": "storyPeriod", "LABEL": "Story Length (s)", "TYPE": "float", "AUDIO": false, "MIN": 8.0, "MAX": 120.0, "DEFAULT": 32.0, "GROUP": "Colour" },
    { "NAME": "yellowCol",   "LABEL": "Warning Yellow",   "TYPE": "color", "DEFAULT": [1.0, 0.70, 0.02, 1.0], "GROUP": "Colour" },
    { "NAME": "yellowSat",   "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.6, "DEFAULT": 1.15, "GROUP": "Colour" },
    { "NAME": "contrast",    "LABEL": "Contrast",         "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.25, "GROUP": "Colour" },
    { "NAME": "domeOnly",    "LABEL": "Domes Only (tile stays yellow)", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Colour" },
    { "NAME": "domeGlow",    "LABEL": "Dome Glow",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Colour" },
    { "NAME": "gloss",       "LABEL": "Gloss",            "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Light" },
    { "NAME": "fluoros",     "LABEL": "Fluorescent Tubes","TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.75, "GROUP": "Light" },
    { "NAME": "trainEvery",  "LABEL": "Train Every (s, 0 = off)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 90.0, "DEFAULT": 22.0, "GROUP": "Light" },
    { "NAME": "headlight",   "LABEL": "Headlight",        "TYPE": "color", "DEFAULT": [1.0, 0.93, 0.8, 1.0], "GROUP": "Light" },
    { "NAME": "wear",        "LABEL": "Wear & Grime",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Wear" },
    { "NAME": "gum",         "LABEL": "Gum Spots",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Wear" },
    { "NAME": "interact",    "LABEL": "Hand", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Off", "Step (press / pinch)", "Flashlight + Step"], "GROUP": "Hand" },
    { "NAME": "stepSize",    "LABEL": "Foot Size",        "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 4.0, "DEFAULT": 1.6, "GROUP": "Hand" },
    { "NAME": "camTex",      "LABEL": "Image / Camera",   "TYPE": "image", "GROUP": "Camera" },
    { "NAME": "camHeight",   "LABEL": "Brightness Lifts Domes", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.8, "GROUP": "Camera" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 4, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White", "NYC Reference"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background (far platform)", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.025, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.08, 0.07, 0.05, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG (domes only)", "TYPE": "bool", "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Platform Edge — Shades of New York 002
//   One pass. A pinhole camera looks at the floor plane (top down → along
//   the platform). The floor is a lattice of truncated-cone domes in
//   staggered rows on 8×8 tiles. The dome field is evaluated from ONE call
//   site inside a 3-step parallax loop; each dome's height (the pump) and
//   colour are functions of its lattice id, so every dome moves as a unit.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718
#define TILE 8.0
#define RB 0.355
#define RT 0.205
#define BASEH 0.14

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h21(float p) { vec3 p3 = fract(vec3(p) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

vec3 studioPalette(vec3 c) {
    float m = float(paletteMode);
    if (palAmount <= 0.0 || m > 3.5) return c;                     // NYC Reference = the photo's own colours
    float L = clamp(lum(c), 0.0, 1.0);
    float mx = max(c.r, max(c.g, c.b)), mn = min(c.r, min(c.g, c.b));
    float sat = mx > 1e-4 ? (mx - mn) / mx : 0.0;
    vec3 o;
    if (m < 0.5) {
        vec3 hueC = pow((c - mn) / max(mx - mn, 1e-4), vec3(1.4));
        float s2 = 1.0 - pow(1.0 - sat, 4.0);
        float v = clamp(pow(mx, 0.85) * 1.12, 0.0, 1.0);
        o = mix(vec3(1.0), hueC, s2) * v;
        float hx = L * 1.7 + 0.04 * TIME;
        vec3 neon = pow(clamp(abs(fract(hx + vec3(0.0, 0.6667, 0.3333)) * 6.0 - 3.0) - 1.0, 0.0, 1.0), vec3(1.4));
        float wn = (1.0 - smoothstep(0.02, 0.1, sat)) * smoothstep(0.06, 0.25, L) * (1.0 - smoothstep(0.55, 0.85, L));
        o = mix(o, neon * v, wn);
        o = mix(o, vec3(1.0), smoothstep(0.85, 1.0, L) * 0.5);
    } else if (m > 2.5) {
        o = vec3(smoothstep(0.02, 0.95, pow(L, 0.9)));
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


vec2 h22(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1, 0)), f.x), mix(h12(i + vec2(0, 1)), h12(i + vec2(1, 1)), f.x), f.y);
}

// MTA trunk-line colours (no marks, just the colours you see on the platform signs)
vec3 lineCol(float i) {
    i = mod(floor(i), 8.0);
    if (i < 0.5) return vec3(0.99, 0.80, 0.04);    // yellow
    if (i < 1.5) return vec3(1.0, 0.39, 0.10);     // orange
    if (i < 2.5) return vec3(0.93, 0.21, 0.18);    // red
    if (i < 3.5) return vec3(0.0, 0.58, 0.24);     // green
    if (i < 4.5) return vec3(0.0, 0.22, 0.65);     // blue
    if (i < 5.5) return vec3(0.42, 0.75, 0.27);    // lime
    if (i < 6.5) return vec3(0.73, 0.20, 0.68);    // purple
    return vec3(0.60, 0.40, 0.20);                 // brown
}

// ── camera / globals ────────────────────────────────────────────────────
float gT, gAr, gDens, gFl, gH;
vec3 gCamPos, gCf, gCr, gCu;
vec2 gCenterQ, gMouseQ;
float gStepOn, gMouseOk;
float gRipR, gShock, gRise;
float gW1, gW2, gW3, gW4, gW5;
float gCamMode;

vec2 screenUV(vec3 w) {
    vec3 v = w - gCamPos;
    float z = max(dot(v, gCf), 1e-3);
    vec2 s = vec2(dot(v, gCr), dot(v, gCu)) / z * gFl;
    return s * vec2(RENDERSIZE.y / RENDERSIZE.x, 1.0) + 0.5;
}

// ── per-dome pump: the dome's height as a function of its lattice id ────
float gGlowK;
vec3 gCamC;
float domePump(vec2 did, float hsh) {
    vec2 dc = did - gCenterQ;
    float d = length(dc);
    float ang = atan(dc.y, dc.x);
    float T = gT;
    // wave forms (audio-free baseline, audio adds on top)
    float w1 = pow(0.5 + 0.5 * sin(d * 0.55 - T * 3.0), 3.0);                         // ripples
    float w2 = pow(0.5 + 0.5 * sin(dc.x * 0.42 - dc.y * 0.12 - T * 3.4), 4.0);        // train wave
    float pk = T * 0.45 + hsh * 17.3;
    float w3 = exp(-fract(pk) * 7.0) * step(h11(hsh * 91.7 + floor(pk)), 0.4);        // rain pops
    float w4 = pow(0.5 + 0.5 * sin(T * 2.6 + mod(did.y + floor(did.x * 0.5), 2.0) * PI), 3.0);   // checker
    float w5 = pow(0.5 + 0.5 * sin(ang * 3.0 + d * 0.32 - T * 2.4), 4.0);             // spiral
    float wave = gW1 * w1 + gW2 * w2 + gW3 * w3 + gW4 * w4 + gW5 * w5;
    // beat ripple ring, drop shockwave, build rise, step dent
    float rip = exp(-pow((d - gRipR) / 1.8, 2.0)) * audioBeatPulse * gAr;
    float shock = exp(-pow((d - gShock) / 2.6, 2.0)) * gDrop;
    float rise = gRise * smoothstep(1.5, -1.5, -dc.y - (gBuild * 50.0 - 25.0));
    float dm = length(did - gMouseQ);
    float fs = stepSize;
    float dent = gStepOn * gMouseOk * exp(-dm * dm / (fs * fs));
    float stepRing = gStepOn * gMouseOk * sin(dm * 1.1 - TIME * 9.0) * exp(-dm / (4.0 * fs)) * smoothstep(fs * 0.6, fs * 1.4, dm);
    float rest = mix(0.45, 1.0, gLift);
    float pump = 1.0
        + breathe * 0.14 * sin(TIME * 1.2 + hsh * TAU)
        + pumpAmt * rest * (wave * 0.55 + 0.4 * bassP() * gAr * (0.4 + 0.6 * wave))
        + pumpAmt * (0.9 * rip + 1.4 * shock) + 0.8 * rise
        - 1.6 * dent + 0.35 * stepRing;
    // some domes sink below the floor between waves (dimples) for contrast
    pump -= dimples * (1.0 - wave) * smoothstep(0.55, 0.95, h11(hsh * 31.1 + floor(T * 0.25))) * 1.4;
    gGlowK = clamp(wave * 0.5 + rip * 1.2 + shock * 1.6 + 0.4 * rise + 0.3 * stepRing, 0.0, 2.0);
    // camera domes: colour from the camera image under the dome, brightness lifts it
    gCamC = vec3(0.0);
    if (gCamMode > 0.001) {
        vec2 su = screenUV(vec3(did / gDens, 0.0));
        gCamC = texture2D(camTex, clamp(su, 0.0, 1.0)).rgb;
        pump = mix(pump, 0.25 + camHeight * 1.6 * lum(gCamC) + 0.3 * (pump - 1.0), gCamMode);
    }
    return pump;
}

// ── the dome field (ONE call site) ──────────────────────────────────────
vec2 gL, gDid, gTid;
float gRr, gHd, gDp, gGe, gHsh, gPump;
float field(vec2 q) {
    vec2 tid = floor(q / TILE);
    vec2 tq = q - tid * TILE;
    float row = floor(tq.y);
    float off = mod(row, 2.0);
    float cx = 2.0 * floor((tq.x - 1.0 + off) * 0.5 + 0.5) + 1.0 - off;
    vec2 c = vec2(cx, row + 0.5);
    vec2 l = tq - c;
    vec2 did = tid * TILE + c;
    float hsh = h12(did * 0.731 + 17.0);
    float pump = domePump(did, hsh);
    float Hd = BASEH * domeHeight * pump;
    float rr = length(l);
    // truncated cone, rounded shoulder, slightly crowned top
    float s = clamp((RB - rr) / (RB - RT), 0.0, 1.0);
    float prof = s * s * (3.0 - 2.0 * s);
    float dprof = -6.0 * s * (1.0 - s) / (RB - RT);                 // d prof / d r
    float crown = rr < RT ? 0.06 * (1.0 - rr * rr / (RT * RT)) : 0.0;
    float dcrown = rr < RT ? -0.12 * rr / (RT * RT) : 0.0;
    // grout: domes at a tile edge are cut
    vec2 e = min(tq, TILE - tq);
    float ge = min(e.x, e.y);
    float cut = smoothstep(0.05, 0.12, ge);
    gL = l; gDid = did; gTid = tid; gRr = rr; gHsh = hsh; gGe = ge; gPump = pump;
    gHd = Hd * cut;
    gDp = (dprof + dcrown) * Hd * cut;                               // dh/dr
    return (prof + crown) * Hd * cut;
}

// ── colour story ────────────────────────────────────────────────────────
float gCY, gCF, gCM, gCW, gStory;
vec3 storyColour(vec2 sp, vec2 tid, vec3 yel) {
    float clk = gStory;
    // whole-floor fade through the line colours
    float k1 = clk * 7.0;
    vec3 cF = mix(lineCol(floor(k1)), lineCol(floor(k1) + 1.0), smoothstep(0.7, 1.0, fract(k1)));
    // per-tile mosaic, each tile changing on its own beat
    float th = h12(tid + 3.1);
    float k2 = clk * 6.0 + th * 2.0;
    vec3 cM = mix(lineCol(floor(th * 8.0 + floor(k2))), lineCol(floor(th * 8.0 + floor(k2)) + 1.0), smoothstep(0.82, 1.0, fract(k2)));
    // sweeping bands of colour across the strip
    float k3 = dot(sp, vec2(0.86, 0.5)) * 0.07 - clk * 9.0;
    vec3 cW = mix(lineCol(floor(k3)), lineCol(floor(k3) + 1.0), smoothstep(0.8, 1.0, fract(k3)));
    return yel * gCY + cF * gCF + cM * gCM + cW * gCW;
}

// ── wear: grime, speckles, gum, scratches ───────────────────────────────
float grime(vec2 q, out float gumM, out float scr) {
    float g = vnoise(q * 0.55) * 0.6 + vnoise(q * 2.3) * 0.4;
    float spk = step(0.992, h12(floor(q * 34.0)));
    // gum: sparse dark blobs, flattened
    vec2 gc = floor(q * 0.5);
    vec2 go = (h22(gc) - 0.5) * 1.2;
    vec2 gq = (q * 0.5 - gc - 0.5 - go * 0.3) * vec2(1.0, 1.3);
    float gr = 0.08 + 0.12 * h12(gc + 5.0);
    gumM = step(h12(gc + 9.0), gum * 0.18) * smoothstep(gr, gr * 0.55, length(gq) * (0.85 + 0.3 * vnoise(gq * 9.0 + gc * 3.0)));
    // scratches through the coating: thin hashed lines
    vec2 sc = floor(q / 3.0);
    vec2 sp = q / 3.0 - sc;
    float a = h12(sc + 2.0) * PI;
    vec2 dir = vec2(cos(a), sin(a));
    vec2 rel = sp - h22(sc + 4.0);
    float along = dot(rel, dir);
    scr = step(h12(sc + 7.0), wear * 0.7) * smoothstep(0.012, 0.0, abs(dot(rel, vec2(-dir.y, dir.x)))) * smoothstep(0.45, 0.2, abs(along));
    return wear * (0.28 * smoothstep(0.35, 0.95, g) + 0.45 * spk);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy, fc = gl_FragCoord.xy, uv = fc / R;
    vec2 p = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    gAr = ar;

    // ── trains: a pass every N seconds; the build brings one in from far away ──
    float passT = trainEvery > 0.5 ? mod(TIME, trainEvery) / 5.0 : 9.0;
    float passOn = step(passT, 1.0) * smoothstep(0.0, 0.1, passT) * smoothstep(1.0, 0.85, passT);
    float rumble = max(passOn * 0.7, gBuild * gBuild);
    p += (h21(floor(TIME * 40.0)) - 0.5) * 0.006 * rumble + vec2(0.0, 0.004 * sin(TIME * 47.0) * rumble);

    // ── camera: pitch from straight down (tilt 0) to a low look along the platform ──
    float th = mix(1.5707963, 0.33, tilt);
    gCf = vec3(0.0, cos(th), -sin(th));
    gCr = vec3(1.0, 0.0, 0.0);
    gCu = vec3(0.0, sin(th), cos(th));
    gH = 1.0;
    gFl = 1.6;
    gDens = domeRows * gFl / gH;
    float walk = TIME * drift * 0.05 + audioBassTime * 0.015 * ar * drift;
    gCamPos = vec3(0.05 * sway * sin(TIME * 0.11) + 0.02 * sway * sin(TIME * 0.37), walk, gH);
    float tc = gH / sin(th);
    gCenterQ = (gCamPos.xy + gCf.xy * tc) * gDens;

    // clocks
    gT = TIME * waveSpeed * (0.55 + 0.45 * gLift) + audioMidTime * 0.5 * ar * waveSpeed + 2.0 * gBuild * gBuild;
    gStory = TIME / storyPeriod + audioMidTime * 0.015 * ar;
    gRipR = (1.0 - audioBeatPulse) * 16.0;
    gShock = (1.0 - gDrop) * 30.0;
    gRise = gBuild;

    // wave weights (Auto crossfades through the five)
    float wm = float(waveMode);
    gW1 = 0.0; gW2 = 0.0; gW3 = 0.0; gW4 = 0.0; gW5 = 0.0;
    if (wm < 0.5) {
        float ph = TIME / morphPeriod;
        float i0 = mod(floor(ph), 5.0), f = smoothstep(0.75, 1.0, fract(ph));
        float i1 = mod(i0 + 1.0, 5.0);
        gW1 = (1.0 - f) * step(abs(i0 - 0.0), 0.1) + f * step(abs(i1 - 0.0), 0.1);
        gW2 = (1.0 - f) * step(abs(i0 - 1.0), 0.1) + f * step(abs(i1 - 1.0), 0.1);
        gW3 = (1.0 - f) * step(abs(i0 - 2.0), 0.1) + f * step(abs(i1 - 2.0), 0.1);
        gW4 = (1.0 - f) * step(abs(i0 - 3.0), 0.1) + f * step(abs(i1 - 3.0), 0.1);
        gW5 = (1.0 - f) * step(abs(i0 - 4.0), 0.1) + f * step(abs(i1 - 4.0), 0.1);
    } else if (wm < 1.5) gW1 = 1.0;
    else if (wm < 2.5) gW2 = 1.0;
    else if (wm < 3.5) gW3 = 1.0;
    else if (wm < 4.5) gW4 = 1.0;
    else gW5 = 1.0;

    // colour story weights (Auto: yellow → bands → mosaic → fade → yellow)
    float cmode = float(colorMode);
    gCY = 0.0; gCF = 0.0; gCM = 0.0; gCW = 0.0; gCamMode = 0.0;
    if (cmode < 0.5) {
        float ph = fract(gStory);
        gCW = smoothstep(0.36, 0.45, ph) - smoothstep(0.56, 0.63, ph);
        gCM = smoothstep(0.56, 0.63, ph) - smoothstep(0.72, 0.79, ph);
        gCF = smoothstep(0.72, 0.79, ph) - smoothstep(0.88, 0.98, ph);
        gCY = 1.0 - gCW - gCM - gCF;
    } else if (cmode < 1.5) gCY = 1.0;
    else if (cmode < 2.5) gCF = 1.0;
    else if (cmode < 3.5) gCM = 1.0;
    else if (cmode < 4.5) gCW = 1.0;
    else { gCY = 1.0; gCamMode = 1.0; }

    // the hand: mouse ray → floor
    float im = float(interact);
    vec2 mp = (mousePos - 0.5) * vec2(R.x / R.y, 1.0);
    vec3 rdm = normalize(mp.x * gCr + mp.y * gCu + gFl * gCf);
    gMouseOk = step(rdm.z, -0.02) * step(0.5, im);
    vec2 mW = gCamPos.xy + rdm.xy * (gH / max(-rdm.z, 0.02));
    gMouseQ = mW * gDens;
    gStepOn = step(0.5, mouseDown);

    // ── view ray → floor ──
    vec3 rd = normalize(p.x * gCr + p.y * gCu + gFl * gCf);
    vec3 col;
    float alpha = 1.0;
    vec3 bg = backdrop(uv);
    if (rd.z > -0.012) {
        col = bg;
        alpha = transparentBg ? 0.0 : 1.0;
    } else {
        float t = gH / -rd.z;
        vec2 W = gCamPos.xy + rd.xy * t;
        vec2 q0 = W * gDens;
        float fp = t * gDens / (gFl * R.y) / max(-rd.z, 0.18);          // floor units per pixel
        float relK = 1.0 - smoothstep(0.08, 0.35, fp);                   // far away: flatten the relief

        // parallax: walk up the ray to the dome surface (one call site, three evaluations)
        vec2 q = q0;
        vec2 slopeXY = rd.xy / -rd.z;
        int iters = tilt > 0.02 ? 3 : 1;
        float h = 0.0;
        for (int i = 0; i < 3; i++) {
            h = field(q);
            if (i >= iters - 1) break;
            q = q0 - slopeXY * h * relK;
        }
        vec2 l = gL;
        float rr = gRr;
        float aa = max(fp * 1.2, 0.004);
        float onDome = smoothstep(RB + aa, RB - aa, rr) * smoothstep(0.05, 0.12, gGe) * step(0.01, abs(gHd));
        float topM = smoothstep(RT + aa, RT - aa - 0.02, rr) * onDome;

        // normal (floor space, z up)
        vec2 grad = gDp * (rr > 1e-4 ? l / rr : vec2(0.0)) * relK;
        // tile bevel at the grout
        float bevel = 1.0 - smoothstep(0.03, 0.1, gGe);
        vec3 N = normalize(vec3(-grad, 1.0));

        // ── colour ──
        vec3 yel = yellowCol.rgb;
        yel = clamp(mix(vec3(lum(yel)), yel, yellowSat), 0.0, 1.0);
        vec2 sp = mix(q, gDid, onDome);                                  // domes take one colour each
        vec3 sc = storyColour(sp, gTid, yel);
        // drop: a line-colour burst ring and a brief whole-floor flip
        float dC = length(gDid - gCenterQ);
        float burst = exp(-pow((dC - gShock) / 4.0, 2.0)) * gDrop;
        vec3 flipC = lineCol(floor(gStory * 7.0) + 2.0 + floor(TIME / 4.0));
        sc = mix(sc, flipC, clamp(burst * 1.2 + gDrop * gDrop * 0.6, 0.0, 1.0));
        vec3 domeC = gCamMode > 0.5 ? mix(sc, gCamC * 1.15, onDome) : sc;
        vec3 base = mix(domeC, yel, domeOnly * (1.0 - onDome) * (1.0 - gCamMode));
        if (gCamMode > 0.5) base = mix(texture2D(camTex, uv).rgb * 0.35 + yel * 0.18, domeC, onDome);

        // worn dome tops are lighter and matte, shoulders keep the colour
        float scuff = vnoise(l * 18.0 + gHsh * 40.0);
        base = mix(base, mix(base, vec3(1.0, 0.93, 0.75), 0.35) * (0.92 + 0.12 * scuff), topM * wear * 0.75);
        // chipped shoulders: dark crescents on some domes
        float ca = gHsh * TAU * 3.0;
        float chip = step(h11(gHsh * 77.0), wear * 0.35) * smoothstep(0.07, 0.02, length(l - vec2(cos(ca), sin(ca)) * RB * 0.8)) * onDome;
        base = mix(base, vec3(0.16, 0.12, 0.08), chip * 0.85);
        // grime, gum, scratches on the tile surface
        float gumM, scr;
        float gr = grime(q, gumM, scr);
        base *= 1.0 - gr * (1.0 - 0.5 * onDome);
        base = mix(base, mix(base, vec3(1.0, 0.95, 0.8), 0.5), scr * (1.0 - onDome) * 0.6);
        base = mix(base, vec3(0.05, 0.045, 0.04), gumM * 0.9);
        // grout
        float grout = 1.0 - smoothstep(0.022, 0.034 + aa, gGe);
        base = mix(base, vec3(0.13, 0.115, 0.1), grout);
        // contrast around the colour's own mid
        base = clamp((base - 0.45) * contrast + 0.45 + (contrast - 1.0) * 0.08 * (lum(base) - 0.4), 0.0, 1.0);

        // ── light ──
        vec3 V = -rd;
        vec3 L = normalize(vec3(-0.5, 0.42, 0.78));
        float dif = clamp(dot(N, L) * 1.0 + 0.2, 0.05, 1.25);
        // dome shadow on the tile (toward lower right) + contact occlusion ring
        float hn = gHd / (BASEH * domeHeight + 1e-4);
        vec2 shOff = L.xy / L.z * gHd * 2.2;
        float shadow = smoothstep(RB * 1.05, RB * 0.55, length(l + shOff)) * (1.0 - onDome) * relK;
        float ao = 1.0 - 0.32 * exp(-max(rr - RB, 0.0) / 0.06) * step(RB, rr) * clamp(hn, 0.0, 1.5) * relK;
        ao *= 1.0 - 0.45 * grout - 0.2 * bevel;
        // gloss: Blinn-Phong key, the fluorescent tube strips in reflection, fresnel
        float gl = gloss * (1.0 - 0.65 * topM * wear) * (1.0 - grout);
        vec3 Hh = normalize(L + V);
        float spec = pow(max(dot(N, Hh), 0.0), mix(18.0, 140.0, gloss)) * gl * 1.2;
        vec3 Rf = reflect(-V, N);
        float tubeU = Rf.x / max(Rf.z, 0.15) * 1.3 + gCamPos.x * 2.0;
        float tubes = smoothstep(0.82, 0.98, 0.5 + 0.5 * cos(tubeU * PI)) * smoothstep(0.0, 0.3, Rf.z) * fluoros;
        float fres = 0.04 + 0.96 * pow(1.0 - max(dot(N, V), 0.0), 5.0);
        vec3 lit = base * (dif * ao * (1.0 - 0.5 * shadow) + 0.08);
        lit += vec3(1.0, 0.98, 0.94) * (spec + tubes * gl * (0.12 + 0.9 * fres) * 0.8);
        // highs sparkle the gloss
        lit += vec3(1.0) * spec * 1.5 * highP() * ar * step(0.6, h11(gHsh * 13.0 + floor(TIME * 12.0)));

        // train headlight sweeping across the floor (or approaching during the build)
        vec2 cW = gCenterQ / gDens;
        float span = 0.9 * R.x / R.y / gFl * gH;
        float lx = passOn > 0.0 ? mix(-1.6, 1.6, passT) * span : -1.6 * span * (1.0 - gBuild);
        vec3 Lp = vec3(cW.x + lx, cW.y + 0.15, 0.28);
        float lightOn = max(passOn, gBuild * gBuild);
        vec3 fw = vec3(q / gDens, h / gDens);
        vec3 Ld = Lp - fw;
        float att = 1.0 / (1.0 + dot(Ld, Ld) * 9.0);
        vec3 Ln = normalize(Ld);
        float hl = max(dot(N, Ln), 0.0) * att * lightOn * 2.2;
        float hsp = pow(max(dot(N, normalize(Ln + V)), 0.0), 90.0) * att * lightOn * 6.0 * gloss;
        lit += headlight.rgb * (base * hl + hsp);

        // flashlight in your hand
        if (im > 1.5 && gMouseOk > 0.5) {
            vec3 Lm = vec3(mW, 0.16) - fw;
            float am = exp(-dot(Lm.xy, Lm.xy) * 90.0) * 0.45;
            vec3 lm = normalize(Lm);
            lit += vec3(1.0, 0.97, 0.9) * (base * max(dot(N, lm), 0.0) * am * 0.9 + pow(max(dot(N, normalize(lm + V)), 0.0), 80.0) * am * 1.4 * gloss);
        }

        // domes glow from inside like LEDs
        float glowK = domeGlow * (gGlowK * 0.45 + 0.6 * gPeak * bassP() * ar + gDrop * 1.2) * onDome;
        lit += domeC * glowK * (0.45 + 0.55 * topM);
        lit += domeC * 0.12 * domeGlow * gDrop * (1.0 - onDome);       // the whole tile flashes on the drop

        // show lift + far fog toward the dark end of the platform
        lit *= 0.82 + 0.3 * gLift;
        float fog = smoothstep(2.5, 9.0, t) * step(0.02, tilt);
        col = mix(lit, bg, fog);
        alpha = transparentBg ? clamp(onDome + lum(domeC) * glowK, 0.0, 1.0) * (1.0 - fog) : 1.0;
    }

    // drop flash, vignette, grain
    col += vec3(1.0, 0.95, 0.8) * gDrop * gDrop * 0.1;
    vec2 dc = uv - 0.5;
    col *= 1.0 - 0.22 * smoothstep(0.4, 1.0, length(dc * vec2(R.x / R.y, 1.0)));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * 0.018;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), alpha);
}
