/*{
  "DESCRIPTION": "Anime — a sky full of hand-inked anime explosions that keep bursting, billowing, morphing and flowing. Every blast lives a whole cartoon life: a white flash frame and a spiky starburst, a shock ring racing out flat across the ground, then cauliflower smoke puffs swelling up in cel-shaded layers (crisp shadow crescents, light facets and a hot glowing core), splash blades and spikes shooting out, teardrop debris flying on real arcs, and finally the smoke breaking apart into holes and drifting blobs before the next one goes off. By default every blast bursts out of the centre of the frame and flies outward, growing as it goes, with the biggest one going off right in the middle (switch Origin to Scattered to spread them across the sky). Four kinds of blast mix together — Billow, Mushroom, Spike Burst and Splash Crown. The ink is drawn like a real animator's: bold outer outlines that get heavier on the shadow side, lighter broken lines where puffs overlap, and a gentle line boil so it always feels hand-drawn. Five looks: Cel Color (bold candy colours on warm grey paper), Manga Ink (black and white with screentone dots), Sketch (white and lavender with construction lines, like an effects animator's model sheet), Pop Poster (pink-and-blue airbrushed clouds, yellow and black starbursts, flowing ribbons) and Night Glow (glowing violet blasts on a dark sky). An endless bank of inked smoke clouds keeps billowing outward behind them, swelling and breaking apart. Manga focus lines rush in around the biggest blast as it goes off. At rest only a few small blasts drift by; during a build they come faster and the focus lines close in; on the drop everything flashes and bursts at once.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Anime", "Illustration", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "styleSel",    "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Cel Color", "Manga Ink", "Sketch", "Pop Poster", "Night Glow"], "GROUP": "Look" },
    { "NAME": "typeSel",     "LABEL": "Blast Type", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Mixed", "Billow", "Mushroom", "Spike Burst", "Splash Crown"], "GROUP": "Explosions" },
    { "NAME": "count",       "LABEL": "Blasts",           "TYPE": "float", "AUDIO": false, "MIN": 2.0, "MAX": 10.0, "DEFAULT": 8.0, "GROUP": "Explosions" },
    { "NAME": "cloudBank",   "LABEL": "Cloud Bank",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Explosions" },
    { "NAME": "sizeScale",   "LABEL": "Size",             "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Explosions" },
    { "NAME": "speed",       "LABEL": "Speed",            "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Explosions" },
    { "NAME": "flow",        "LABEL": "Morph & Flow",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Explosions" },
    { "NAME": "originSel",   "LABEL": "Origin", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["From Center", "Scattered"], "GROUP": "Explosions" },
    { "NAME": "tilt",        "LABEL": "Tilt",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Explosions" },
    { "NAME": "lineWidth",   "LABEL": "Ink Weight",       "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Ink" },
    { "NAME": "innerLines",  "LABEL": "Inner Lines",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Ink" },
    { "NAME": "lineBoil",    "LABEL": "Line Boil",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Ink" },
    { "NAME": "construct",   "LABEL": "Construction Lines","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Ink" },
    { "NAME": "starAmt",     "LABEL": "Starbursts",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Effects" },
    { "NAME": "focusLines",  "LABEL": "Focus Lines",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Effects" },
    { "NAME": "debrisAmt",   "LABEL": "Debris",           "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Effects" },
    { "NAME": "ribbonAmt",   "LABEL": "Ribbons",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Effects" },
    { "NAME": "speckAmt",    "LABEL": "Floating Specks",  "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Effects" },
    { "NAME": "frameStep",   "LABEL": "Frame Feel", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Smooth", "On 2s (12 fps)", "On 3s (8 fps)"], "GROUP": "Effects" },
    { "NAME": "glowAmt",     "LABEL": "Glow",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "paperAmt",    "LABEL": "Paper Texture",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgSel",       "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["Look's Paper", "Custom Colour"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "Custom BG Colour", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vn(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm(vec2 p) { return 0.62 * vn(p) + 0.38 * vn(p * 2.13 + 5.2); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
float easeOut(float x) { x = clamp(x, 0.0, 1.0); return 1.0 - (1.0 - x) * (1.0 - x) * (1.0 - x); }

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

// ── Painter: every shape lays down its fill, then its ink line ──────────
float gStyle, PX, LW, RES, gT, gClk;
vec3 INK;
vec3 gCol;  float gA;  float gMinD;  float gECov;  float gStarD;  float gXCov;
vec3 C_SH, C_BASE, C_LIT, C_HOT, C_STAR;

void paintFill(vec3 f, float d) {
    float c = 1.0 - smoothstep(-PX, PX, d);
    gCol = mix(gCol, f, c);
    gA = max(gA, c);
    gECov = max(gECov, c);
    gXCov = max(gXCov, c);
}
void paintLine(float d, float w, float a) {
    float l = (1.0 - smoothstep(w * 0.5 - PX * 0.7, w * 0.5 + PX * 0.7, abs(d))) * a;
    gCol = mix(gCol, INK, l);
    gA = max(gA, l);
}
void paintBoth(vec3 f, float d, float w, float a) { paintFill(f, d); paintLine(d, w, a); }

bool isStyle(float s) { return abs(gStyle - s) < 0.5; }

void setPal(float hidx) {
    if (isStyle(0.0)) {                       // Cel Color — the candy palette on warm grey
        if (hidx < 0.5)      { C_BASE = vec3(0.11, 0.37, 0.72); C_SH = vec3(0.05, 0.17, 0.46); C_LIT = vec3(0.42, 0.76, 0.98); }
        else if (hidx < 1.5) { C_BASE = vec3(0.96, 0.64, 0.79); C_SH = vec3(0.84, 0.38, 0.60); C_LIT = vec3(1.0, 0.87, 0.93); }
        else if (hidx < 2.5) { C_BASE = vec3(0.98, 0.84, 0.10); C_SH = vec3(0.95, 0.50, 0.10); C_LIT = vec3(1.0, 0.97, 0.62); }
        else if (hidx < 3.5) { C_BASE = vec3(0.95, 0.25, 0.12); C_SH = vec3(0.66, 0.07, 0.13); C_LIT = vec3(1.0, 0.60, 0.38); }
        else                 { C_BASE = vec3(0.95, 0.93, 0.90); C_SH = vec3(0.68, 0.70, 0.84); C_LIT = vec3(1.0); }
        C_HOT = vec3(1.0, 0.96, 0.78);
        C_STAR = (hidx > 1.5 && hidx < 2.5) ? vec3(1.0, 0.98, 0.92) : vec3(1.0, 0.86, 0.16);
    } else if (isStyle(1.0)) {                // Manga Ink
        C_BASE = vec3(0.98, 0.97, 0.95); C_SH = vec3(0.98, 0.97, 0.95); C_LIT = vec3(1.0); C_HOT = vec3(1.0); C_STAR = vec3(1.0);
    } else if (isStyle(2.0)) {                // Sketch — white + lavender model sheet
        C_BASE = vec3(0.98, 0.98, 1.0); C_SH = vec3(0.66, 0.66, 0.97); C_LIT = vec3(1.0);
        C_HOT = vec3(0.97, 0.68, 0.94); C_STAR = vec3(0.98, 0.80, 0.97);
    } else if (isStyle(3.0)) {                // Pop Poster — airbrushed pink→blue clouds
        C_BASE = vec3(0.97, 0.72, 0.85); C_SH = vec3(0.92, 0.52, 0.72); C_LIT = vec3(0.70, 0.89, 0.98);
        C_HOT = vec3(1.0, 0.99, 0.94);
        C_STAR = hidx < 2.5 ? vec3(0.99, 0.95, 0.08) : vec3(0.86, 1.0, 0.0);
    } else {                                  // Night Glow
        if (hidx < 2.5) { C_BASE = vec3(0.20, 0.25, 0.60); C_SH = vec3(0.09, 0.10, 0.30); C_LIT = vec3(0.66, 0.74, 1.0); }
        else            { C_BASE = vec3(0.38, 0.23, 0.62); C_SH = vec3(0.15, 0.08, 0.30); C_LIT = vec3(0.86, 0.70, 1.0); }
        C_HOT = vec3(1.0, 0.96, 1.0); C_STAR = vec3(0.95, 0.93, 1.0);
    }
}

float screentone(vec2 fc, float lev) {
    vec2 g = rot(0.785398) * fc / (5.5 * RES);
    vec2 f = fract(g) - 0.5;
    float r = sqrt(lev / PI);
    return 1.0 - smoothstep(r - 0.1, r + 0.1, length(f));
}

// cel shading of one smoke puff (q = unit-disc coords, Ld = light direction in the same frame)
// clu = how far this point sits on the shadow side of the whole cloud (-1 lit … +1 shadow)
vec3 puffShade(vec2 q, float seed, float heat, vec2 Ld, float qpx, vec2 fc, float clu) {
    float n1 = vn(q * 1.6 + seed * 13.1 + gT * 0.3) - 0.5;
    float n2 = vn(q * 3.8 + seed * 7.7 - gT * 0.25) - 0.5;
    float jag = n1 * 0.32 + n2 * 0.14;
    float sh = length(q - Ld * 0.42) - 0.84 + jag;              // > 0: shadow crescent away from the light
    sh = max(sh, clu - 0.3 + jag * 0.8);                         // the cloud's own shadow side
    float hl = length(q + Ld * 0.3) - 0.9 + n2 * 0.18;           // > 0: lit rim crescent
    hl = min(hl, 0.12 - clu * 0.4);
    float ht = heat * 1.25 - 0.3 - length(q + Ld * 0.05) + jag * 0.9;  // > 0: hot core (only while it is really hot)
    float e = max(qpx, 0.012);
    float mS = smoothstep(-e, e, sh), mH = smoothstep(-e, e, hl), mT = smoothstep(-e, e, ht);
    vec3 c;
    if (isStyle(3.0)) {                                          // airbrushed, grainy gradient
        float g = clamp(0.5 + 0.55 * dot(q, -Ld) + jag * 0.5, 0.0, 1.0);
        g += (h12(fc) - 0.5) * 0.3;
        c = mix(C_BASE, C_LIT, smoothstep(0.35, 0.75, g));
        return mix(c, C_HOT, mT * 0.85);
    }
    c = mix(C_BASE, C_LIT, mH);
    if (isStyle(1.0)) {
        float tone = screentone(fc, 0.3 + 0.3 * smoothstep(0.0, 0.3, sh));
        c = mix(c, INK, mS * tone);
        c = mix(c, INK, smoothstep(-e, e, min(sh - 0.34, clu - 0.6 + jag)));
    } else {
        c = mix(c, C_SH, mS);
        if (isStyle(0.0) || isStyle(4.0)) c = mix(c, C_SH * 0.7, smoothstep(-e, e, sh - 0.36) * 0.85);
    }
    c = mix(c, C_HOT, mT);
    if ((isStyle(2.0) || isStyle(1.0)) && construct > 0.0) {     // effects-sheet construction grid
        float r2 = dot(q, q);
        if (r2 < 1.0) {
            float z = sqrt(1.0 - r2);
            float lat = asin(clamp(q.y, -1.0, 1.0));
            float lon = atan(q.x, z);
            float gl = min(abs(fract(lat * 1.6 + 0.5) - 0.5) / 1.6 * cos(lat),
                           abs(fract(lon * 1.6 + 0.5) - 0.5) / 1.6 * z);
            float ln = 1.0 - smoothstep(0.0, qpx * 1.3, gl);
            c = mix(c, isStyle(1.0) ? vec3(0.55) : C_SH * 0.85, ln * 0.45 * construct * (1.0 - mT));
        }
    }
    return c;
}

// curved, tapered splash blade; returns (distance, side ratio across the blade)
vec2 blade(vec2 q, vec2 b, float ang, float L, float w, float bend) {
    vec2 dir = vec2(sin(ang), cos(ang));
    vec2 nrm = vec2(dir.y, -dir.x);
    vec2 e = q - b;
    float u = dot(e, dir), v = dot(e, nrm);
    float tt = clamp(u / L, 0.0, 1.0);
    float dv = v - bend * L * tt * tt;
    float wp = w * (1.0 - tt) * (0.7 + 0.3 * smoothstep(0.0, 0.2, tt));
    float side = dv > 0.0 ? 1.0 : 0.45;                          // asymmetric fin
    float d = abs(dv) - wp * side;
    d = max(d, max(-u, u - L));
    d /= sqrt(1.0 + 4.0 * bend * bend * tt * tt);
    return vec2(d * 0.85, dv / max(wp, 1e-4));
}

// spiky starburst with uneven spikes (distance corrected by the radial slope)
float starD(vec2 q, float R, float N, float rp, float inner, float seed, float sharp) {
    float rho = length(q);
    float x = atan(q.y, q.x) / TAU * N + rp;
    float id = mod(floor(x), N);
    float f = fract(x);
    float tip = 1.0 - abs(f - 0.5) * 2.0;
    float Ls = 0.5 + 0.5 * h11(id * 3.17 + seed);
    float r = R * (inner + (1.0 - inner) * pow(tip, sharp) * Ls);
    float drda = R * (1.0 - inner) * Ls * sharp * pow(max(tip, 1e-4), sharp - 1.0) * 2.0 * N / TAU;
    float k = drda / max(rho, 1e-4);
    return (rho - r) / sqrt(1.0 + k * k);
}

// flat perspective shock ring; returns (distance, height relative to its centre)
vec2 ringD(vec2 q, vec2 rc, float rad, float th, float sq) {
    vec2 e = q - rc;
    vec2 ee = vec2(e.x, e.y * sq);
    float dl = max(length(ee), 1e-4);
    float gm = length(vec2(ee.x, ee.y * sq)) / dl;
    return vec2(abs((dl - rad) / gm) - th, e.y);
}

void drawRibbons(vec2 p, float A) {
    float n = floor(ribbonAmt * 3.0 + 0.5);
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        if (fi >= n) break;
        float h = h11(fi * 5.3 + 1.0);
        vec2 q = rot((h - 0.5) * 1.1) * p;
        float len = 1.2 + 0.7 * h11(fi + 3.0);
        float span = 2.0 * A + 2.0 + len;
        float xs = -A - len - 0.4 + fract(gT * 0.05 * (1.0 + fi * 0.35) + h) * span;
        float tt = (q.x - xs) / len;
        if (tt < 0.0 || tt > 1.0) continue;
        float y0 = (h11(fi + 7.0) - 0.5) * 0.9;
        float amp = 0.1 + 0.1 * h;
        float fr = 2.2 + 1.5 * h11(fi + 9.0);
        float ph = gT * 0.7 + h * 6.0;
        float fy = y0 + amp * sin(fr * q.x + ph);
        float dfy = amp * fr * cos(fr * q.x + ph);
        float tw = q.x * 2.6 - gT * 0.9 + fi * 2.0;              // the ribbon twists as it flows
        float w = (0.014 + 0.012 * h) * pow(sin(PI * tt), 0.7) * (0.3 + 0.7 * abs(cos(tw)));
        float d = abs(q.y - fy) / sqrt(1.0 + dfy * dfy) - w;
        vec3 c;
        if (isStyle(3.0))      c = fi < 0.5 ? vec3(0.27, 0.66, 0.95) : fi < 1.5 ? vec3(0.17, 0.66, 0.45) : vec3(0.96, 0.30, 0.14);
        else if (isStyle(0.0)) c = fi < 0.5 ? vec3(0.11, 0.37, 0.72) : fi < 1.5 ? vec3(0.95, 0.25, 0.12) : vec3(0.98, 0.84, 0.10);
        else if (isStyle(4.0)) c = vec3(0.7, 0.75, 1.0);
        else if (isStyle(2.0)) c = vec3(0.66, 0.66, 0.97);
        else                   c = INK;
        c *= cos(tw) > 0.0 ? 1.0 : 0.78;
        gMinD = min(gMinD, d + 0.01);
        paintBoth(c, d, LW * 0.7, 1.0);
    }
}

void drawSpecks(vec2 p) {
    if (speckAmt <= 0.0) return;
    vec2 g = p * 6.0 + vec2(gT * 0.04, gT * 0.12);
    vec2 id = floor(g);
    vec2 f = fract(g) - 0.5;
    float h = h12(id + 3.3);
    if (h > 0.22 * speckAmt) return;
    vec2 o = (vec2(h12(id + 1.1), h12(id + 2.2)) - 0.5) * 0.5;
    float r = 0.05 + 0.08 * h12(id + 4.4);
    float d = (length(f - o) - r) / 6.0;
    float hc = h12(id + 5.5);
    setPal(floor(hc * 5.0));
    vec3 c = hc < 0.3 ? INK : (hc < 0.65 ? C_BASE : C_STAR);
    paintBoth(c, d, LW * 0.6, 1.0);
}

// endless bank of inked smoke clusters flowing outward from the middle (from-centre mode) or drifting
// across the sky (scattered). Two zoom layers, half a cycle apart: each cluster is born small, swells
// as its layer zooms out, then breaks apart into holes just before the layer wraps.
void drawCloudBank(vec2 p, vec2 fc) {
    if (cloudBank <= 0.0) return;
    bool fromC = float(originSel) < 0.5;
    vec2 Lw = normalize(vec2(-0.55, 0.83));
    float C = 0.42;
    for (int layer = 0; layer < 2; layer++) {
        float fl = float(layer);
        float ph = gT * 0.09 + fl * 0.5;
        float f = fract(ph);
        float lcyc = floor(ph) * 2.0 + fl;
        float z = fromC ? exp2(f * 1.3) : 1.0;
        vec2 g = fromC ? p / z : p + vec2(-(f + floor(ph)) * 0.6, 0.0) * (1.0 + fl * 0.3);
        float life = smoothstep(0.0, 0.25, f);
        float diss = smoothstep(0.62, 0.97, f);
        vec2 gid = floor(g / C);
        for (int dy = 1; dy >= -1; dy--) {                    // top rows first so lower clusters overlap them
            for (int dx = -1; dx <= 1; dx++) {
                vec2 id = gid + vec2(float(dx), float(dy));
                float cs = h12(id + lcyc * 7.31);
                if (cs > cloudBank * 0.85) continue;
                vec2 cc = (id + 0.5 + (vec2(h12(id + 1.7 + lcyc), h12(id + 2.9 + lcyc)) - 0.5) * 0.5) * C;
                vec2 sc = fromC ? cc * z : cc - (g - p);      // cluster centre on screen
                float keep = fromC ? smoothstep(0.12, 0.42, length(sc / vec2(1.3, 1.0))) : 1.0;
                float sz = (0.6 + 0.6 * h12(id + 4.1 + lcyc)) * life * keep * (1.0 - 0.45 * diss);
                if (sz < 0.05) continue;
                setPal(floor(h12(id + 5.3 + lcyc) * 5.0));
                vec3 tint = isStyle(0.0) ? mix(C_BASE, vec3(0.97, 0.95, 0.93), 0.35) : C_BASE;
                vec3 tS = C_SH, tB = C_BASE, tL = C_LIT;
                C_BASE = tint; C_SH = mix(tS, tint, 0.15); C_LIT = mix(tL, vec3(1.0), 0.2);
                gECov = 0.0;
                float cr = 0.27 * C * sz;
                for (int k = 0; k < 6; k++) {
                    float fk = float(k);
                    float a = fk * 1.1 + h12(id + fk) * 0.6;
                    float rr = fk < 0.5 ? 0.0 : 0.75 + 0.35 * h12(id + fk * 3.1);
                    vec2 pc = cc + vec2(cos(a) * 1.25, sin(a) * 0.75 - 0.25 * step(0.5, fk)) * rr * cr
                            + 0.25 * cr * vec2(sin(gT * 0.8 + fk + cs * 9.0), cos(gT * 0.7 + fk * 1.3)) * flow;
                    float r = cr * (fk < 0.5 ? 1.15 : 0.62 + 0.3 * h12(id + fk * 5.7));
                    vec2 q = (g - pc) / r;
                    float zz = fromC ? z : 1.0;
                    float d = (length(q) - 1.0) * r * zz;
                    if (d > LW * 2.0 + 2.0 * PX) continue;
                    if (diss > 0.0) d = max(d, (diss * 0.95 - fbm(g * 9.0 + id)) * 0.5 * r * zz * 3.0);
                    float clu = dot(g - cc, -Lw) / (cr * 1.6);
                    float prev = gECov;
                    vec3 fill = puffShade(q, cs * 31.0 + fk, 0.0, Lw, PX / (r * zz), fc, clu);
                    paintFill(fill, d);
                    float an = atan(q.y, q.x);
                    float side = dot(q / max(length(q), 1e-4), -Lw);
                    float w = LW * 0.85 * (0.7 + 0.6 * smoothstep(-0.4, 0.7, side)) * mix(1.0, 0.55, prev);
                    paintLine(d, w, mix(1.0, innerLines * smoothstep(0.25, 0.5, vn(vec2(an * 1.6 + fk, cs * 9.0))), prev));
                }
                C_BASE = tB; C_SH = tS; C_LIT = tL;
            }
        }
    }
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 p = (fc - 0.5 * R) / R.y;
    float A = R.x / R.y;
    RES = R.y / 1080.0;
    PX = 1.0 / R.y;
    float ar = audioReact;
    float fsel = float(frameStep);
    float tq = fsel < 0.5 ? TIME : (fsel < 1.5 ? floor(TIME * 12.0) / 12.0 : floor(TIME * 8.0) / 8.0);
    gT   = tq * 0.6 * speed + audioMidTime * 0.25 * ar;
    gClk = tq * speed + audioBassTime * 0.18 * ar;
    gStyle = float(styleSel);

    vec3 bg;
    float lwS;
    if (isStyle(0.0))      { bg = vec3(0.80, 0.79, 0.78); INK = vec3(0.08, 0.06, 0.10); lwS = 1.0; }
    else if (isStyle(1.0)) { bg = vec3(0.96, 0.95, 0.92); INK = vec3(0.05, 0.045, 0.06); lwS = 1.15; }
    else if (isStyle(2.0)) { bg = vec3(0.995, 0.995, 1.0); INK = vec3(0.33, 0.30, 0.74); lwS = 0.7; }
    else if (isStyle(3.0)) { bg = vec3(0.95, 0.93, 0.86); INK = vec3(0.04); lwS = 0.8; }
    else                   { bg = mix(vec3(0.10, 0.13, 0.30), vec3(0.02, 0.03, 0.09), smoothstep(-0.2, 1.0, uv.y)); INK = vec3(0.84, 0.84, 1.0); lwS = 0.6; }
    if (float(bgSel) > 0.5) bg = bgColor.rgb;
    LW = max(lineWidth * lwS * 2.4 / 1080.0, PX * 0.9) * (1.0 + 0.18 * audioBeatPulse * ar);

    gCol = bg; gA = 0.0; gMinD = 1e3; gStarD = 1e3; gXCov = 0.0;

    // shared warp: slow flowing morph + a gentle hand-drawn line boil (8 drawings a second)
    float bs = floor(TIME * 8.0) * 17.31;
    vec2 wp = p + lineBoil * PX * 3.2 * (vec2(vn(p * 38.0 + bs), vn(p * 38.0 - bs + 3.7)) - 0.5);
    wp += flow * 0.07 * (vec2(fbm(p * 1.6 + vec2(0.0, gT * 0.2)), fbm(p * 1.6 + vec2(5.2, -gT * 0.17))) - 0.5);

    drawRibbons(wp, A);
    drawSpecks(wp);
    gXCov = 0.0;
    drawCloudBank(wp, fc);

    float cntF = clamp(count - 2.0 * gRest + gPeak, 2.0, 10.0);
    float heroFlash = 0.0;
    vec2 heroC = vec2(0.0);
    float heroCyc = 0.0;
    vec2 Lw = normalize(vec2(-0.55, 0.83));

    for (int i = 0; i < 10; i++) {
        float fi = float(i);
        float vis = clamp(cntF - fi, 0.0, 1.0);
        if (vis <= 0.0) break;
        float per = 5.2 + 2.6 * h11(fi * 7.13 + 1.7);
        float cc = gClk / per + h11(fi * 3.71 + 0.3);
        float cyc = floor(cc);
        float l = fract(cc);
        float sd = fi * 17.31 + cyc * 3.917;
        float ty = float(typeSel) > 0.5 ? float(typeSel) - 1.0 : floor(h11(sd + 0.5) * 4.0);
        bool fromC = float(originSel) < 0.5;
        if (fromC && i > 0 && ty > 0.5 && ty < 1.5) ty = 0.0;
        float S = (fromC ? (i == 0 ? 0.36 / 0.5 : 0.75 * mix(0.6, 1.0, easeOut(l / 0.5))) : 1.0) * (i == 0 ? 0.5 : 0.27 + 0.15 * h11(sd + 1.1)) * (ty > 0.5 && ty < 1.5 ? 0.8 : 1.0) * sizeScale * vis * (1.0 + 0.12 * gDrop);
        float hx = h11(sd + 2.3), hy = h11(sd + 3.1);
        vec2 base;
        if (i == 0) base = vec2((hx - 0.5) * 0.5 * A, -0.44 + hy * 0.12);
        else base = vec2((((fi - 0.5) / max(cntF - 1.0, 1.0)) * 2.0 - 1.0) * A * 0.44 + (hx - 0.5) * 0.25, -0.47 + hy * 0.55);
        float grounded = (ty > 0.5 && ty < 1.5) || ty > 2.5 ? 0.3 : 1.0;
        float er = (h11(sd + 4.0) - 0.5) * 1.2 * tilt * grounded;
        if (fromC) {                             // every blast bursts out of the middle, pointing outward
            float th = i == 0 ? (hx - 0.5) * 0.8 * tilt
                              : (fi - 1.0) / max(cntF - 1.0, 1.0) * TAU + cyc * 2.39996 + (hx - 0.5) * 0.9;
            vec2 dir = vec2(sin(th), cos(th));
            float trav = i == 0 ? 0.0 : 0.04 + (0.24 + 0.1 * hy) * easeOut(l / 0.75);
            base = dir * trav * vec2(min(A, 2.0) * 0.85, 0.7) - (i == 0 ? dir * 0.45 * S : vec2(0.0));
            er = -th;
        }
        float ce = cos(er), se = sin(er);

        // life phases
        float grow  = easeOut(l / 0.42);
        float fast  = easeOut(l / 0.16);
        float diss  = smoothstep(0.55, 1.0, l);
        float fade  = 1.0 - smoothstep(0.86, 1.0, l);
        float heatG = 1.0 - smoothstep(0.04, 0.5, l);
        float flashF = 1.0 - smoothstep(0.02, 0.08, l);
        float star  = smoothstep(0.0, 0.03, l) * (1.0 - smoothstep(0.1, 0.26, l));
        star = max(star, gDrop * 0.8);
        float rise  = l * (ty < 1.5 ? 0.3 : 0.1);
        vec2 cs = vec2(0.0, ty < 0.5 ? 0.55 : (ty < 1.5 ? 0.6 : 0.2));
        if (i == 0) { heroFlash = star * starAmt; heroC = base + vec2(-sin(er), cos(er)) * cs.y * S; heroCyc = cyc; }

        vec2 dp = p - base;
        vec2 q0 = vec2(ce * dp.x + se * dp.y, -se * dp.x + ce * dp.y) / max(S, 1e-3);
        if (abs(q0.x) > 2.0 || q0.y < -1.0 || q0.y > 2.4) continue;

        setPal(floor(h11(sd + 4.4) * 5.0));
        vec2 dw = wp - base;
        vec2 wq = vec2(ce * dw.x + se * dw.y, -se * dw.x + ce * dw.y) / S;
        wq += flow * 0.14 * (vec2(vn(wq * 2.4 + vec2(gT * 0.35, sd)), vn(wq * 2.4 + vec2(sd + 9.1, -gT * 0.3))) - 0.5);
        vec2 Ld = vec2(ce * Lw.x + se * Lw.y, -se * Lw.x + ce * Lw.y);
        float qpx = PX / S;
        gECov = 0.0;

        // black spiky burst behind everything (Manga / Pop)
        if ((isStyle(1.0) || isStyle(3.0)) && star * starAmt > 0.01) {
            float d = starD(wq - cs, 1.05 * star * starAmt, floor(6.0 + h11(sd + 8.0) * 5.0), h11(sd + 9.0), 0.22, sd + 1.0, 3.0) * S;
            paintBoth(INK, d, LW, 1.0);
        }

        // shock rings: flat on the ground, and a "Saturn" ring around a mushroom cap
        vec2 rg = ringD(wq, vec2(0.0, 0.03), 0.25 + 1.05 * easeOut(l / 0.35), 0.035 * (1.0 - smoothstep(0.15, 0.42, l)) - 0.006, 5.0);
        float capY = (0.95 * grow + rise);
        vec2 rc = ringD(wq, vec2(0.0, capY), 0.3 + 0.75 * easeOut(l / 0.3), 0.028 * (1.0 - smoothstep(0.25, 0.55, l)) - 0.006, 5.5);
        bool hasCap = ty > 0.5 && ty < 1.5;
        vec3 ringCol = isStyle(4.0) ? C_LIT : (isStyle(1.0) ? vec3(1.0) : C_LIT);
        if (grounded < 0.5 && l < 0.45 && rg.y > 0.0) paintBoth(ringCol, rg.x * S, LW * 0.8, 1.0);
        if (hasCap && l < 0.55 && rc.y > 0.0) paintBoth(ringCol, rc.x * S, LW * 0.8, 1.0);

        // splash blades and spikes
        for (int j = 0; j < 7; j++) {
            float fj = float(j);
            float g1 = h11(sd + fj * 2.71 + 0.6), g2 = h11(sd + fj * 1.93 + 0.9), g3 = h11(sd + fj * 3.33 + 0.2);
            vec2 b = vec2(0.0);
            float ang = 0.0, L = 0.0, w = 0.0, bend = 0.0;
            if (ty < 0.5) {
                if (fj < 3.0) { b = vec2(0.0, 0.55 * grow + rise); ang = g1 * TAU; L = (0.55 + 0.3 * g2) * fast; w = 0.07; bend = (g3 - 0.5) * 0.7; }
            } else if (ty < 1.5) {
                if (fj < 4.0) { float sg = fj < 2.0 ? -1.0 : 1.0; b = vec2((fj - 1.5) * 0.12, 0.02); ang = sg * (1.05 + 0.35 * g1); L = (0.32 + 0.2 * g2) * fast; w = 0.06; bend = sg * 0.5; }
            } else if (ty < 2.5) {
                b = vec2((g1 - 0.5) * 0.4, 0.08); ang = (fj / 6.0 - 0.5) * 1.5 + (g2 - 0.5) * 0.3;
                L = (0.75 + 0.75 * g3) * fast * (j == 3 ? 1.3 : 1.0); w = 0.075 + 0.03 * g2; bend = (g1 - 0.5) * 0.25;
            } else {
                b = vec2((fj / 6.0 - 0.5) * 0.55, 0.03); ang = (fj / 6.0 - 0.5) * 2.4 + (g2 - 0.5) * 0.3;
                L = (0.55 + 0.45 * g3) * fast; w = 0.08 + 0.04 * g2; bend = (ang < 0.0 ? -1.0 : 1.0) * (0.35 + 0.4 * g1);
            }
            w *= 1.35 * (1.0 - smoothstep(0.22, 0.55, l));
            L = min(L, 0.95) * (1.0 + 0.15 * smoothstep(0.1, 0.5, l));
            if (L < 0.01 || w < 0.02) continue;
            vec2 bd = blade(wq, b, ang, L, w, bend);
            float d = bd.x * S;
            gMinD = min(gMinD, d);
            if (d > LW * 2.0 + 2.0 * PX) continue;
            vec3 f = mix(C_BASE, C_LIT, smoothstep(-0.25, -0.35, bd.y));
            f = isStyle(1.0) ? mix(f, INK, smoothstep(0.3, 0.4, bd.y)) : mix(f, C_SH, smoothstep(0.2, 0.3, bd.y));
            f = mix(f, C_HOT, max(flashF, heatG * 0.5));
            paintBoth(f, d, LW * 0.85, 1.0);
        }

        // teardrop debris flying on arcs
        if (debrisAmt > 0.0 && l > 0.03) {
            vec2 gv = vec2(-se, -ce);                             // world-down in the local frame
            for (int m = 0; m < 10; m++) {
                float fm = float(m);
                if (fm >= 10.0 * debrisAmt) break;
                float k1 = h11(sd + fm * 4.13 + 0.7), k2 = h11(sd + fm * 2.57 + 0.3), k3 = h11(sd + fm * 1.31 + 0.9);
                float a = (k1 - 0.5) * 2.8;
                vec2 vel = vec2(sin(a), cos(a)) * (1.3 + 1.1 * k2);
                float tau = l * 1.4;
                vec2 vnow = vel + gv * 1.6 * tau;
                vec2 pos = cs * 0.6 + vel * tau + gv * 0.8 * tau * tau;
                float rad = (0.025 + 0.035 * k3) * (1.0 - smoothstep(0.45, 0.95, l));
                if (rad < 0.004) continue;
                vec2 vd = normalize(vnow);
                vec2 e = wq - pos;
                float ea = dot(e, vd), eb = dot(e, vec2(vd.y, -vd.x));
                ea = ea < 0.0 ? ea / 2.2 : ea;                    // tail trails behind
                float d = (length(vec2(ea, eb)) - rad) * S;
                if (d > LW * 2.0 + 2.0 * PX) continue;
                vec3 f = k3 < 0.25 ? INK : (k3 < 0.7 ? C_BASE : C_STAR);
                paintBoth(f, d, LW * 0.7, 1.0);
            }
        }
        vec2 cloudC = vec2(0.0, (ty < 1.5 ? 0.65 * grow : 0.3) + rise);
        float cloudR = ty < 1.5 ? 0.55 : 0.45;
        // smoke puffs — painted back to front so each one inks over the ones behind it
        for (int k = 0; k < 14; k++) {
            float fk = float(k);
            float h1 = h11(sd + fk * 1.37 + 0.11), h2 = h11(sd + fk * 2.11 + 0.37), h3 = h11(sd + fk * 0.73 + 0.59);
            vec2 c = vec2(0.0);
            float r = 0.0;
            float grow2 = easeOut((l - 0.06) / 0.5);
            if (ty < 0.5) {                                       // Billow: a cauliflower ball
                float y = 0.66 + (0.5 - fk / 13.0) * 0.95 + (h2 - 0.5) * 0.12;
                float wx = sqrt(max(0.26 - (y - 0.64) * (y - 0.64) * 0.9, 0.004));
                c = vec2((h1 * 2.0 - 1.0) * wx, y);
                r = 0.2 + 0.13 * h3;
                c = vec2(c.x * grow, mix(0.25, c.y, grow) + rise);
                r *= mix(0.3, 1.0, grow);
            } else if (ty < 1.5) {                                // Mushroom: cap, stem, skirt
                if (fk < 3.0)       { c = vec2((fk - 1.0) * 0.24, 1.2 + h2 * 0.06); r = 0.23 + 0.04 * h3; }
                else if (fk < 8.0)  { float x = ((fk - 3.0) / 4.0 * 2.0 - 1.0) * 0.52; c = vec2(x, 0.98 + 0.08 * h2 - 0.08 * abs(x)); r = 0.22 + 0.06 * h3; }
                else if (fk < 11.0) { c = vec2((h1 - 0.5) * 0.1, 0.78 - (fk - 8.0) * 0.24); r = 0.13 + 0.03 * h3; }
                else                { c = vec2((fk - 12.0) * 0.34, 0.08); r = 0.17 + 0.05 * h3; }
                c = vec2(c.x * mix(0.4, 1.0, grow), c.y * grow + rise * step(fk, 10.5));
                r *= mix(0.35, 1.0, grow);
            } else if (ty < 2.5) {                                // Spike Burst: smoke behind a low dome
                if (fk < 6.0)       { float a = (fk / 5.0 - 0.5) * 2.2; c = vec2(sin(a) * 0.42, 0.42 + cos(a) * 0.32 + h2 * 0.1); r = (0.2 + 0.08 * h3) * grow2; }
                else if (fk < 7.0)  { c = vec2(0.0, 0.04); r = 0.44 * grow; }
                else                { float x = ((fk - 7.0) / 6.0 * 2.0 - 1.0) * 0.55; c = vec2(x, 0.05 + 0.06 * h2); r = (0.14 + 0.06 * h3) * grow; }
                c.y += rise;
            } else {                                              // Splash Crown: a rising plume over foam
                if (fk < 5.0) { c = vec2((fk / 4.0 - 0.5) * 0.55 + (h1 - 0.5) * 0.1, 0.22 + 0.16 * h2 + 0.12 * (1.0 - abs(fk / 4.0 - 0.5) * 2.0)) * vec2(grow2, 1.0); r = (0.16 + 0.06 * h3) * grow2; }
                else if (fk < 7.0) { c = vec2((fk - 5.5) * 0.36, 0.55 + 0.1 * h2); r = (0.17 + 0.06 * h3) * grow2; }
                else          { c = vec2(((fk - 7.0) / 6.0 - 0.5) * 1.0 + (h1 - 0.5) * 0.1, 0.05 + h2 * 0.08); r = (0.12 + 0.06 * h3) * grow; }
            }
            r *= 1.2 * (1.0 - 0.5 * diss) * fade * (1.0 + 0.05 * audioBeatPulse * ar);
            if (r < 0.006) continue;
            c += 0.035 * vec2(sin(gT * 0.9 + fk * 2.1 + sd), cos(gT * 0.75 + fk * 1.7)) * flow;
            vec2 q = (wq - c) / r;
            if (ty > 0.5 && ty < 1.5 && k == 8) {                 // mushroom stem: one tapered column
                float top = 1.0 * grow + rise;
                float yy = clamp(wq.y, 0.0, top);
                float sr = mix(0.17, 0.11, yy / max(top, 1e-3)) * mix(0.35, 1.0, grow) * (1.0 - 0.5 * diss) * fade;
                r = sr;
                q = vec2(wq.x - c.x * 0.3, (wq.y - yy) * 1.0) / max(sr, 1e-3);
            }
            float d = (length(q) - 1.0) * r;
            float dropD = d * S;
            gMinD = min(gMinD, dropD);
            if (dropD > LW * 2.0 + 2.0 * PX) continue;
            if (diss > 0.0) {                                     // smoke breaking up into holes
                float hn = fbm(wq * 3.2 + vec2(sd, sd * 0.37));
                d = max(d, (diss * 0.9 - hn) * 0.5);
            }
            d *= S;
            float heat = heatG * clamp(1.2 - length(c - cs) * 1.4, 0.0, 1.0);
            float prev = gECov;
            float clu = dot(wq - cloudC, -Ld) / cloudR;
            vec3 fill = puffShade(q, sd + fk, heat, Ld, qpx / r, fc, clu);
            fill = mix(fill, C_HOT, flashF);
            paintFill(fill, d);
            float an = atan(q.y, q.x);
            float side = dot(q / max(length(q), 1e-4), -Ld);
            float w = LW * (0.75 + 0.6 * smoothstep(-0.4, 0.7, side)) * mix(1.0, 0.55, prev) * (0.8 + 0.4 * vn(vec2(an * 2.0, sd + fk)));
            float la = mix(1.0, innerLines * smoothstep(0.25, 0.5, vn(vec2(an * 1.6 + fk, sd * 0.3))), prev);
            paintLine(d, w, la);
        }

        if (grounded < 0.5 && l < 0.45 && rg.y <= 0.0) paintBoth(ringCol, rg.x * S, LW * 0.8, 1.0);
        if (hasCap && l < 0.55 && rc.y <= 0.0) paintBoth(ringCol, rc.x * S, LW * 0.8, 1.0);

        // starburst flash
        float sR = (0.5 + 0.25 * h11(sd + 5.5)) * star * starAmt * (i == 0 ? 1.2 : 1.0);
        if (sR > 0.01) {
            float N = floor(8.0 + h11(sd + 6.0) * 7.0);
            float rp = h11(sd + 7.0) + l * 0.6;
            float d = starD(wq - cs, sR, N, rp, 0.36, sd, 2.4) * S;
            gMinD = min(gMinD, d);
            gStarD = min(gStarD, d);
            paintBoth(C_STAR, d, LW, 1.0);
            float d2 = starD(wq - cs, sR * 0.5, N, rp + 0.5, 0.5, sd + 3.0, 2.0) * S;
            paintFill(isStyle(4.0) ? vec3(1.0) : C_HOT, d2);
        }

    }

    // manga focus lines rushing in on the biggest blast
    float fI = focusLines * clamp(heroFlash * 1.4 + gDrop + 0.35 * gBuild, 0.0, 1.0);
    if (fI > 0.01) {
        vec2 dp = p - heroC;
        float rho = length(dp);
        float N = 150.0;
        float x = atan(dp.y, dp.x) / TAU * N;
        float id = mod(floor(x), N);
        float f = fract(x);
        float on = step(h11(id * 1.7 + heroCyc * 0.37 + 3.0), 0.62);
        float r0 = (0.5 + 0.5 * h11(id + 11.0 + heroCyc)) * (1.35 - 0.55 * fI) * max(A, 1.0) * 0.6;
        float wF = (0.15 + 0.35 * h11(id + 5.0)) * smoothstep(r0, r0 + 0.7, rho);
        float pxA = PX / max(rho, 1e-3) * N / TAU;
        float ln = 1.0 - smoothstep(wF * 0.5 - pxA, wF * 0.5 + pxA, abs(f - 0.5));
        float a = ln * on * fI * (1.0 - gXCov);           // the lines stay behind the blasts
        gCol = mix(gCol, INK, a);
        gA = max(gA, a);
    }

    vec3 col = gCol;
    if (!isStyle(4.0)) {
        float pap = 0.035 * vn(fc * 0.6 / RES) + 0.025 * vn(fc * vec2(0.04, 0.9) / RES);
        col *= 1.0 - paperAmt * pap;
    } else {
        col += vec3(0.5, 0.55, 1.0) * exp(-max(gMinD, 0.0) / 0.03) * 0.3 * glowAmt;
    }
    if (!isStyle(1.0)) col += mix(vec3(1.0, 0.92, 0.6), vec3(1.0), 0.4) * exp(-max(gStarD, 0.0) / 0.02) * 0.22 * glowAmt;
    col += vec3(1.0) * gDrop * gDrop * 0.08;
    col *= 1.0 + 0.08 * audioBeatPulse * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(gA, 0.0, 1.0) : 1.0);
}
