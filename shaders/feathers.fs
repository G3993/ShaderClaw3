/*{
  "DESCRIPTION": "Feathers — macro photography of real plumage in deep 3D. Every feather has a tapered glossy rachis, thousands of fine crisp barbs sweeping off the shaft, fuzzy barbules, split vanes and a fringed translucent edge; the vanes curve, catch a key light, glow with backlight through the barbs, streak with anisotropic sheen along the barbs and castS soft contact shadows on the feathers beneath, with structural thin-film colour that flips as the angle changes and a soft depth of field. Pick a pattern or let Auto Morph bloom from one into the next: Peacock Train (a fanned cascade of tail feathers with teardrop eyespots — indigo heart, turquoise, bronze and gold rings, an emerald halo and loose bronze barbs), Pheasant Scales (dense overlapping copper scale feathers with black crescent bars and iridescent green lacing), Argus Eyes (long mottled feathers with rows of ball-and-socket ocelli that look like polished spheres), Macaw Wing (layered flight feathers in bold scarlet, gold and blue bands), Mandala Fan (concentric rings of chevron-barred feathers rotating against each other like a kaleidoscope), Quetzal Stream (long ribbon tail feathers in rolling waves of emerald-gold iridescence), Owl Barring (soft velvety barred and mottled down with fine vermiculation) and Hummingbird Gorget (tiny scale feathers that flash from near-black to ruby, gold and green as the light sweeps). Bass ruffles the plumage and lifts the feathers off their shadows, the beat flashes the iridescence and the highs shimmer the barbs. At rest a few big feathers drift calmly in dim light; during a build they spread and fan open, the light sweeps faster and the plumage trembles; the drop is a burst — every feather flutters and flares in a flash of colour — and the peak is the full, dense, glittering display.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "3D", "Nature", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "patternSel",  "LABEL": "Pattern", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8], "LABELS": ["Auto Morph", "Peacock Train", "Pheasant Scales", "Argus Eyes", "Macaw Wing", "Mandala Fan", "Quetzal Stream", "Owl Barring", "Hummingbird Gorget"], "GROUP": "Feathers" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 14.0, "GROUP": "Feathers" },
    { "NAME": "featherCount","LABEL": "Feather Density",  "TYPE": "float", "AUDIO": false, "MIN": 8.0, "MAX": 34.0, "DEFAULT": 22.0, "GROUP": "Feathers" },
    { "NAME": "featherLen",  "LABEL": "Feather Length",   "TYPE": "float", "MIN": 1.2, "MAX": 3.4, "DEFAULT": 2.7, "GROUP": "Feathers" },
    { "NAME": "featherWidth","LABEL": "Feather Width",    "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.05, "GROUP": "Feathers" },
    { "NAME": "fluff",       "LABEL": "Fluff & Splits",   "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Feathers" },
    { "NAME": "iridescence", "LABEL": "Iridescence",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "prismPatches","LABEL": "Rainbow Patches",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Light" },
    { "NAME": "sheen",       "LABEL": "Sheen",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "backlight",   "LABEL": "Backlight",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "baseTone",    "LABEL": "Colour Variant", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Natural", "Pearl White", "Midnight Blue"], "GROUP": "Light" },
    { "NAME": "flow",        "LABEL": "Flow Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "ruffle",      "LABEL": "Ruffle",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "depthFade",   "LABEL": "Depth of Field & Haze", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.03, 0.025, 0.025, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vn2(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
// smooth step-edge of width aa centred on e
float sedge(float e, float x, float aa) { return smoothstep(e - aa, e + aa, x); }
// rotate a colour's hue about the grey axis (structural colour shift)
vec3 hueRot(vec3 c, float a) {
    vec3 k = vec3(0.57735);
    float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}
// thin-film interference tint for a film thickness (arbitrary units)
vec3 thinFilm(float d) {
    vec3 c = 0.5 - 0.5 * cos(TAU * d * vec3(2.0, 2.42, 2.9));
    return mix(vec3(0.85), c, 0.75);
}

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

float gT, gBass, gFan;
vec3 gKey;          // key light direction (screen space, toward light)

// ── per-pattern parameters (set by setPattern) ──────────────────────────
// 0 Peacock · 1 Pheasant · 2 Argus · 3 Macaw · 4 Mandala · 5 Quetzal · 6 Owl · 7 Hummingbird
float pBC, pk, pL, pW, pP, pB, pAsym, pNB, pSlope, pFl, pStag, pJN, pIN, pCup, pGloss, pShaftW, pShad, pCurv, pWrap;
void setPattern(float k) {
    pk = k;
    float lm = clamp(featherLen / 2.7, 0.55, 1.26);
    float wm = clamp(featherWidth / 1.05, 0.5, 1.9);
    pBC = 1.0; pAsym = 0.85; pStag = 0.5; pCup = 0.8; pGloss = 70.0; pShaftW = 1.0; pShad = 1.0; pCurv = 0.0; pWrap = 0.0;
    if (k < 0.5)      { pL = 3.3; pW = 1.25; pP = 1.55; pB = 0.62; pNB = 120.0; pSlope = 0.85; pFl = 1.0;  pGloss = 90.0; pShaftW = 0.65; }   // peacock: teardrop
    else if (k < 1.5) { pL = 1.75; pW = 1.45; pP = 1.25; pB = 0.42; pNB = 46.0;  pSlope = 0.95; pFl = 0.7;  pCurv = 0.7; pShaftW = 0.8; } // pheasant: rounded scale
    else if (k < 2.5) { pL = 5.2; pW = 1.12; pP = 0.9;  pB = 0.55; pNB = 210.0; pSlope = 0.7;  pFl = 0.8;  pAsym = 0.7; pStag = 0.0; }   // argus: long lanceolate
    else if (k < 3.5) { pL = 4.0; pW = 1.0;  pP = 0.85; pB = 0.6;  pNB = 170.0; pSlope = 0.6;  pFl = 0.6;  pAsym = 0.55; pGloss = 110.0; } // macaw: flight feather
    else if (k < 4.5) { pL = 2.5; pW = 1.3;  pP = 1.2;  pB = 0.55; pNB = 90.0;  pSlope = 0.9;  pFl = 0.8;  pAsym = 1.0; }   // mandala
    else if (k < 5.5) { pL = 5.0; pW = 1.05; pP = 0.55; pB = 0.42; pNB = 230.0; pSlope = 0.55; pFl = 0.5;  pGloss = 160.0; pCup = 1.1; pStag = 0.5; } // quetzal ribbon
    else if (k < 6.5) { pL = 1.9; pW = 1.5;  pP = 1.15; pB = 0.42; pNB = 120.0; pSlope = 1.1;  pFl = 1.3; pBC = 0.4;  pCurv = 0.6; pGloss = 30.0; pShad = 0.8; } // owl: soft round
    else              { pL = 1.55; pW = 1.4; pP = 1.3;  pB = 0.4;  pNB = 24.0;  pSlope = 1.0;  pFl = 0.4; pBC = 0.35;  pCurv = 1.3; pShaftW = 0.6; pGloss = 50.0; } // hummingbird scale
    pL *= lm; pW *= wm;
    pFl *= fluff;
    pL = min(pL, 5.4);
    pJN = min(ceil(pL * 1.2 + 0.45), 7.0);
    pIN = pW * 0.62 + 0.25 > 1.0 ? 2.0 : 1.0;
}

// row slide (rings rotating, wings breathing …)
float rowShift(float row) {
    if (pk > 3.5 && pk < 4.5) return (mod(row, 2.0) * 2.0 - 1.0) * gT * 0.22 + mod(row, 2.0) * 0.5;
    return pStag * mod(row, 2.0);
}

// plumage coordinates: +y points along the feathers, cells are ~isotropic.
// cellPx = pixels per cell, phi = screen angle of +y, N = wrap count (0 = none), blur = DOF footprint (px)
vec2 layoutCoords(vec2 sp, out float cellPx, out float phi, out float N, out float valid, out float blur) {
    float RY = RENDERSIZE.y;
    float dens = featherCount / 22.0;
    valid = 1.0; N = 0.0; blur = 0.0;
    float dof = depthFade;
    if (pk < 0.5 || (pk > 1.5 && pk < 4.5)) {
        // polar fans: Peacock (origin below), Argus (far below), Macaw (wrist top-left), Mandala (centre)
        vec2 O = vec2(0.0, -0.8); float n0 = 36.0; float a0 = 0.0; float fl = 0.09;
        if (pk > 1.5 && pk < 2.5) { O = vec2(0.0, -2.7); n0 = 64.0; fl = 0.035; }
        if (pk > 2.5 && pk < 3.5) { O = vec2(-1.25, 1.05); n0 = 48.0; a0 = 2.2; fl = 0.0; }
        if (pk > 3.5)             { O = vec2(0.0); n0 = 15.0; fl = 0.11; }
        vec2 c = sp - O;
        float r = max(length(c), 1e-4);
        float a = atan(c.x, c.y);                       // 0 = up, clockwise
        phi = PI * 0.5 - a;
        float ar = a - a0;
        ar = mod(ar + PI, TAU) - PI;
        N = floor(n0 * dens + 0.5);
        float sway = (pk > 2.5 && pk < 3.5) ? 0.06 * sin(gT * 0.35) : 0.02 * sin(gT * 0.3);
        float fan = pk > 3.5 ? 1.0 : gFan;
        float pv = (ar + sway) / fan * N / TAU;
        float pu = log(r) * N / TAU - gT * fl;
        cellPx = r * TAU / N * RY;
        if (pk > 3.5) { cellPx = max(cellPx, 0.5); valid = smoothstep(0.012, 0.03, r); }
        if (pk < 0.5) valid = smoothstep(PI * 0.82, PI * 0.72, abs(ar / fan));
        if (pk > 2.5 && pk < 3.5) { valid = smoothstep(0.95, 0.75, abs(ar / fan)) * smoothstep(0.35, 0.5, r); N = 0.0; }
        // depth: peacock / argus focus mid-frame, mandala focus at the centre ring
        float fd = pk > 3.5 ? abs(r - 0.28) : abs(sp.y + 0.02) * 0.9 + 0.25 * max(abs(sp.x) - 0.5, 0.0);
        blur = dof * 5.0 * smoothstep(0.12, 0.6, fd);
        return vec2(pv, pu);
    }
    if (pk > 4.5 && pk < 5.5) {
        // Quetzal: long ribbons streaming left in rolling waves
        float NQ = 8.5 * dens;
        vec2 p = sp * NQ;
        float w1 = 0.5 * p.x - gT * 0.9, w2 = 0.95 * p.x + gT * 0.55;
        float wv = 0.85 * sin(w1) + 0.3 * sin(w2 + p.y * 0.25);
        float dw = 0.85 * 0.5 * cos(w1) + 0.3 * 0.95 * cos(w2 + p.y * 0.25);
        p.y += wv;
        phi = atan(dw, -1.0);
        cellPx = RY / NQ;
        blur = dof * 4.0 * smoothstep(0.15, 0.55, abs(sp.y));
        return vec2(p.y, -p.x - gT * 1.1);
    }
    // scale grids: Pheasant / Owl / Hummingbird — a curved body seen in perspective, tips point down
    float NS = pk < 1.5 ? 6.5 : (pk < 6.5 ? 4.6 : 14.0);
    NS *= dens;
    float tilt = pk < 6.5 ? 0.5 : 0.25;
    float wgt = 1.0 - tilt * sp.y;
    vec2 p = sp / wgt;
    if (pk > 6.5) p *= 1.0 + 0.45 * dot(sp, sp);             // throat bulge
    float ra = pk < 1.5 ? -0.14 : 0.06;
    p = rot(ra) * p;
    p *= NS;
    p.x /= gFan;
    if (pk > 5.5 && pk < 6.5) p.x += 0.25 * sin(p.y * 0.6 + gT * 0.4);
    vec2 dir = rot(-ra) * vec2(0.0, -1.0);
    phi = atan(dir.y, dir.x);
    cellPx = RY / NS * wgt * gFan;
    blur = dof * 5.0 * smoothstep(0.1, 0.5, sp.y) + dof * 2.5 * smoothstep(0.25, 0.55, -sp.y);
    return vec2(p.x, -p.y + gT * (pk > 6.5 ? 0.12 : 0.28));
}

// ── markings: pigment albedo, structural (iridescent) weight, gloss … ────
void markings(float u, float v, float y, float L, float W, float rel, float h, float row, float aaU, float jag,
              out vec3 alb, out float iw, out vec3 shaftC, out float loose, out vec2 bump) {
    alb = vec3(0.5); iw = 0.0; shaftC = vec3(0.85, 0.8, 0.7); loose = 0.0; bump = vec2(0.0);
    float au = abs(u);
    if (pk < 0.5) {
        // Peacock eyespot: indigo heart, turquoise, bronze, gold, emerald halo
        float R = 0.33 * W;
        vec2 e = vec2(u, v - 0.73 * L) / R;
        e.y *= e.y < 0.0 ? 0.62 : 1.0;
        float r = length(e) + jag * 0.07;
        float aa = aaU / R + 0.02;
        vec2 eh = vec2(e.x * 1.08, (e.y + 0.05) * 1.18);
        float rh = length(eh) + 0.16 * smoothstep(0.32, 0.0, abs(e.x)) * smoothstep(-0.1, 0.35, e.y) + jag * 0.05;
        vec3 vane = mix(vec3(0.3, 0.17, 0.05), vec3(0.2, 0.3, 0.06), smoothstep(0.25, 0.6, y));
        alb = vane; iw = 0.55;
        alb = mix(alb, vec3(0.32, 0.12, 0.16), 1.0 - sedge(1.16, r, aa));            // thin violet-bronze line
        alb = mix(alb, vec3(0.05, 0.42, 0.14), 1.0 - sedge(1.07, r, aa));            // emerald halo
        alb = mix(alb, vec3(0.62, 0.48, 0.08), 1.0 - sedge(0.9, r, aa));             // gold
        alb = mix(alb, vec3(0.36, 0.17, 0.05), 1.0 - sedge(0.78, r, aa));            // bronze
        alb = mix(alb, vec3(0.0, 0.5, 0.56), 1.0 - sedge(0.6, r, aa));               // turquoise
        alb = mix(alb, vec3(0.02, 0.05, 0.3), 1.0 - sedge(0.42, rh, aa));            // indigo heart
        iw = mix(0.55, 0.92, 1.0 - sedge(1.1, r, aa));
        loose = smoothstep(0.62, 0.42, y) * smoothstep(1.2, 1.6, r);               // disconnected barbs below the eye
        shaftC = vec3(0.55, 0.42, 0.25);
    } else if (pk < 1.5) {
        // Pheasant scale: copper-gold with black crescent bars and iridescent green lacing
        float rho = length(vec2(u / (0.5 * W), (y - 0.3) / 0.7)) + jag * 0.03;
        float aa = aaU / (0.5 * W) + 0.015;
        float hv = h;
        alb = mix(vec3(0.62, 0.3, 0.05), vec3(0.85, 0.52, 0.1), smoothstep(0.2, 0.7, rho));
        float b1 = sedge(0.6 + 0.04 * hv, rho, aa) - sedge(0.7 + 0.04 * hv, rho, aa);
        float b2 = sedge(0.82, rho, aa) - sedge(0.89, rho, aa);
        alb = mix(alb, vec3(0.03, 0.02, 0.015), max(b1, b2));
        float lace = sedge(0.92, rho, aa);
        alb = mix(alb, vec3(0.05, 0.38, 0.22), lace);
        iw = 0.25 + 0.7 * lace + 0.2 * max(b1, b2);
        alb *= 0.8 + 0.25 * smoothstep(0.0, 0.5, y);
        shaftC = vec3(0.7, 0.45, 0.15);
    } else if (pk < 2.5) {
        // Argus: mottled tan with a row of 3D ball-and-socket ocelli along the shaft
        float bcA = (v - 0.7 * au) * 9.0 / L;
        float mot = vn2(vec2(bcA * 3.0, au * 7.0 / W) + h * 20.0) * 0.6 + vn2(vec2(bcA * 9.0, au * 20.0 / W)) * 0.4;
        alb = mix(vec3(0.42, 0.27, 0.13), vec3(0.66, 0.5, 0.3), mot);
        float sp = smoothstep(0.6, 0.75, vn2(vec2(bcA * 14.0, au * 26.0 / W) + 3.0));
        alb = mix(alb, vec3(0.12, 0.07, 0.04), sp * 0.8);
        alb = mix(alb, alb * vec3(0.55, 0.5, 0.48), smoothstep(0.3, 0.0, au / W));      // darker near shaft
        float nO = 6.0;
        float yo = clamp(floor(y * nO), 1.0, nO - 1.0);
        float Ro = 0.16 * W;
        vec2 oc = vec2(u - 0.19 * W, v - (yo + 0.5) * L / nO) / Ro;
        oc.y *= 0.92;
        float r = length(oc);
        float aa = aaU / Ro + 0.02;
        float inBall = 1.0 - sedge(0.78, r, aa);
        float ring = (1.0 - sedge(0.92, r, aa)) - inBall;
        float rim = (1.0 - sedge(1.04, r, aa)) - (1.0 - sedge(0.92, r, aa));
        // pigment-painted sphere + real bump
        vec2 s = oc / 0.78;
        float z = sqrt(max(1.0 - dot(s, s), 0.0));
        float sh = clamp(dot(normalize(vec3(s, z)), normalize(vec3(-0.5, 0.6, 0.65))), 0.0, 1.0);
        vec3 ball = mix(vec3(0.16, 0.07, 0.03), vec3(0.85, 0.6, 0.3), sh);
        ball += vec3(1.0, 0.95, 0.85) * smoothstep(0.25, 0.08, length(s - vec2(-0.32, 0.36))) * 0.9;
        alb = mix(alb, vec3(0.04, 0.02, 0.015), rim);
        alb = mix(alb, vec3(0.93, 0.88, 0.75), ring);
        alb = mix(alb, ball, inBall);
        bump = s * z * inBall * 0.9;
        iw = 0.12 + 0.35 * inBall;
        shaftC = vec3(0.86, 0.8, 0.66);
    } else if (pk < 3.5) {
        // Macaw: bands by row — scarlet coverts, gold, blue flight feathers
        float bt = row;
        vec3 red = vec3(0.72, 0.03, 0.03), yel = vec3(0.95, 0.68, 0.04), blu = vec3(0.04, 0.2, 0.72);
        vec3 c0 = bt < -1.5 ? red : (bt < 1.5 ? yel : blu);
        vec3 c1 = bt < -1.5 ? yel : (bt < 1.5 ? vec3(0.1, 0.5, 0.25) : vec3(0.02, 0.08, 0.35));
        alb = mix(c0, c1, smoothstep(0.55, 0.95, y) * (bt < -1.5 ? 0.25 : 0.85));
        if (bt < -1.5) alb = mix(alb, vec3(0.45, 0.02, 0.02), smoothstep(0.8, 1.0, y));
        alb *= u > 0.0 ? 1.0 : 0.78;                                                      // inner vane darker
        iw = bt < 1.5 ? 0.12 : 0.55;
        shaftC = bt < -1.5 ? vec3(0.4, 0.05, 0.04) : vec3(0.2, 0.18, 0.25);
    } else if (pk < 4.5) {
        // Mandala: chevron bars in ring colours with a small eye at the tip
        float ci = mod(row, 3.0);
        vec3 ca = ci < 0.5 ? vec3(0.0, 0.28, 0.34) : (ci < 1.5 ? vec3(0.36, 0.02, 0.14) : vec3(0.04, 0.06, 0.24));
        vec3 cb = ci < 0.5 ? vec3(0.8, 0.56, 0.14) : (ci < 1.5 ? vec3(0.85, 0.5, 0.36) : vec3(0.74, 0.7, 0.6));
        float ch = y - 0.55 * au / W + jag * 0.012;
        float aa = (aaU / L) * 4.0 + 0.03;
        float bars = smoothstep(0.62 - aa, 0.62 + aa, abs(fract(ch * 4.0) - 0.5) * 2.0);
        alb = mix(ca, cb, bars * smoothstep(0.15, 0.3, y));
        float re = length(vec2(u / (0.2 * W), (y - 0.84) / 0.09));
        float aE = aaU / (0.2 * W) + 0.03;
        alb = mix(alb, vec3(0.62, 0.42, 0.1), 1.0 - sedge(1.0, re, aE));
        alb = mix(alb, vec3(0.02, 0.08, 0.3), 1.0 - sedge(0.6, re, aE));
        iw = 0.55 + 0.35 * (1.0 - sedge(1.0, re, aE));
        shaftC = vec3(0.92, 0.88, 0.8);
    } else if (pk < 5.5) {
        // Quetzal: emerald ribbons flipping gold / blue
        alb = mix(vec3(0.02, 0.32, 0.3), vec3(0.06, 0.5, 0.16), smoothstep(0.0, 0.5, y));
        alb = mix(alb, vec3(0.45, 0.5, 0.06), smoothstep(0.7, 1.0, y) * 0.6);
        alb *= 0.75 + 0.25 * smoothstep(1.0, 0.6, rel);
        iw = 0.95;
        shaftC = vec3(0.15, 0.3, 0.18);
    } else if (pk < 6.5) {
        // Owl: buff with wavy brown bars, vermiculation, pale tip spots
        float m = vn2(vec2(u * 6.0 / W, v * 5.0) + h * 13.0);
        float bp = v / L * 7.0 + 0.9 * au / W + 0.7 * m;
        float aa = aaU / L * 7.0 + 0.05;
        float bar = smoothstep(0.62 - aa, 0.62 + aa, abs(fract(bp) - 0.5) * 2.0);
        alb = mix(vec3(0.6, 0.4, 0.2), vec3(0.85, 0.7, 0.48), m);
        alb = mix(alb, vec3(0.24, 0.14, 0.07), bar * 0.85);
        float fvA = 1.0 - smoothstep(0.1, 0.3, 30.0 / W * aaU);
        alb *= 1.0 - 0.22 * fvA * smoothstep(0.55, 0.8, vn2(vec2(u * 30.0 / W, v * 22.0 / L) + h * 5.0));
        alb = mix(alb, vec3(0.3, 0.18, 0.09), smoothstep(0.16, 0.02, au / W) * smoothstep(0.1, 0.4, y));  // shaft streak
        float tipS = 0.7 * (1.0 - sedge(0.05 * W, length(vec2(u, v - 0.86 * L)), aaU)) * step(0.55, h);
        alb = mix(alb, vec3(0.92, 0.88, 0.8), tipS);
        iw = 0.0;
        shaftC = vec3(0.72, 0.6, 0.45);
    } else {
        // Hummingbird gorget scale: dull grey down at the base, iridescent ruby plate at the tip
        alb = mix(vec3(0.12, 0.11, 0.1), vec3(0.6, 0.03, 0.2), smoothstep(0.3, 0.55, y));
        iw = smoothstep(0.3, 0.55, y);
        shaftC = vec3(0.3, 0.25, 0.25);
    }
}

// ── one feather. q = point relative to its cell corner (cell units) ─────
vec4 feather(vec2 q, vec2 id, float row, float cellPx, float blur, float phi, vec2 sp, out float shadow) {
    shadow = 0.0;
    float h  = h12(id * vec2(1.0, 1.37) + pk * 7.1);
    float h2 = h12(id.yx * 1.71 + 3.3 + pk);
    if (pk > 3.5 && pk < 4.5) { h = h12(vec2(0.0, id.y) + 4.0); h2 = h12(vec2(9.0, id.y)); }   // mandala: identical rings
    float L = pL * (0.88 + 0.24 * h2) * (1.0 + 0.06 * gPeak - 0.04 * gRest);
    float W = pW * (0.9 + 0.2 * h) * (1.0 + 0.12 * gDrop);
    vec2 base = vec2(0.5 + (h - 0.5) * 0.2, (h2 - 0.5) * 0.3);
    vec2 d = q - base;
    // ruffle: a wave of wind runs through the plumage; bass lifts, the drop flutters
    float wind = sin(dot(id, vec2(0.45, 0.6)) - gT * 1.7 + h * 6.0) * (0.6 + 0.4 * sin(id.x * 0.3 + gT * 0.4));
    float flut = gDrop * sin(TIME * 21.0 + h * 40.0);
    float lean = (h - 0.5) * 0.1 + 0.05 * ruffle * wind * (0.5 + gLift + gBass) + 0.12 * flut;
    if (pk > 3.5 && pk < 4.5) lean = 0.03 * ruffle * wind * (0.5 + gBass) + 0.1 * flut;
    d = rot(lean) * d;
    float y = d.y / L;
    float bend = (0.08 * (h - 0.5) + 0.06 * ruffle * wind * (0.6 + 0.8 * gLift + gBass)) * y * y * L;
    float u = d.x - bend;
    if (y < -0.2 || y > 1.15 || abs(u) > W * 0.62 + 0.3) return vec4(0.0);

    float pxC = cellPx / (1.0 + blur);                // pixels per cell, after depth-of-field
    float aaU = 1.0 / pxC;                            // one pixel in cell units
    float yc = clamp(y, 0.0, 1.0);
    float prof = pow(max(sin(PI * pow(yc, pP)), 0.0), pB);
    float side = u > 0.0 ? 1.0 : pAsym;
    float w = 0.5 * W * prof * side;
    float au = abs(u);
    float rel = au / max(w, 1e-4);

    // barbs: curved lines sweeping off the shaft toward the tip
    float spc = L / pNB;
    float slope = pSlope * (0.75 + 0.5 * y);
    float bcF = (d.y - slope * au - 0.65 * au * au / W) / spc;
    float bi = floor(bcF), ph = fract(bcF);
    float cyc = sqrt(1.0 + slope * slope) / (spc * pxC);  // barb cycles per pixel
    float bFade = 1.0 - smoothstep(0.16, 0.4, cyc);
    float bFadeC = bFade * pBC;
    float aaPh = min(cyc * 1.2, 0.5);
    float hb = h12(vec2(bi + sign(u) * 0.37, id.x * 3.1 + id.y * 7.7));
    float jag = (hb - 0.5) * bFade;
    // barb clumps: groups of barbs that separate, catch light differently and fray at the edge
    float cS = 3.5 + 3.0 * h;
    float cc = bcF / cS + 0.55 * vn2(vec2(au * 5.0 / W + h * 3.0, bcF / cS * 0.35 + h * 7.0));
    float ci = floor(cc + 0.5);
    float hc = h12(vec2(ci, id.x * 1.3 + id.y * 5.1 + sign(u) * 2.7));
    float cPx = cS / max(cyc, 1e-4);
    float cFade = smoothstep(2.5, 6.0, cPx);
    float cLine = (1.0 - smoothstep(0.35, 1.3, abs(cc - ci) * cPx)) * cFade * smoothstep(0.3, 0.85, hc);

    // fringed edge: each barb ends at its own length; outer barbs separate into strands
    float wb = w * (1.0 + pFl * 0.16 * jag * 2.0 + pFl * 0.1 * (hc - 0.5) * cFade - pFl * 0.05 * (1.0 - bFade) * (vn2(vec2(bcF * 0.15, id.x * 5.0)) - 0.5));
    float edge = smoothstep(wb + aaU, wb - aaU, au);
    float loose;
    vec3 alb, shaftC; float iw; vec2 bump;
    markings(u, d.y, y, L, W, rel, h, row, aaU, jag, alb, iw, shaftC, loose, bump);
    float sepZone = clamp(smoothstep(0.66 + 0.18 * hb, 1.02, rel) * pFl + loose, 0.0, 1.0);
    float rw = mix(1.0, 0.3 + 0.35 * hb, sepZone);                    // strand width (fraction of barb pitch)
    float pd = abs(ph - 0.5) * 2.0;
    float strand = smoothstep(rw + aaPh, rw - aaPh, pd);
    strand = mix(rw, strand, bFade);
    strand = mix(mix(1.0, rw, 0.5), strand, pBC);
    float cover = edge * mix(1.0, strand, sepZone);
    // splits: occasional barbs unzip from the edge inward in a widening V
    float splitOn = step(hb, 0.05 + 0.07 * pFl) * step(0.3, y);
    float gw = smoothstep(0.35 + 0.4 * h12(vec2(bi, id.y)), 1.0, rel) * 0.7;
    float gap = splitOn * smoothstep(1.0 - gw - aaPh, 1.0 - gw + aaPh, pd) * step(0.02, gw);
    cover *= 1.0 - gap * bFade;
    cover *= 1.0 - cLine * smoothstep(0.55, 1.0, rel) * min(pFl, 1.0) * 0.9;
    // downy afterfeather at the base
    float down = smoothstep(0.18, 0.0, y) * smoothstep(-0.12, 0.0, y);
    cover *= mix(1.0, 0.55 + 0.45 * vn2(vec2(au * 40.0 / W, y * 6.0) + id), down * min(pFl, 1.0));
    cover *= smoothstep(-0.02, 0.03, y);

    // rachis: tapered cylinder (calamus continues below the vane)
    float sw = W * 0.032 * pShaftW * (1.0 - 0.75 * clamp(y, 0.0, 1.0)) + 0.002;
    float swp = max(sw, 0.5 * aaU);
    float shaftCov = smoothstep(swp + aaU * 0.7, swp - aaU * 0.7, au) * min(sw / swp, 1.0)
                   * smoothstep(-0.16, -0.11, y) * smoothstep(0.995, 0.93, y);
    float alpha = max(cover, shaftCov);

    // contact + castS shadow onto the feathers beneath
    vec2 ud = vec2(sin(phi), -cos(phi)), vd = vec2(cos(phi), sin(phi));
    vec2 offS = -gKey.xy * (0.1 + 0.18 * gBass + 0.08 * gLift) * W;
    vec2 ds = vec2(u, d.y) - vec2(dot(offS, ud), dot(offS, vd));
    float ys = ds.y / L;
    float ws = 0.5 * W * pow(max(sin(PI * pow(clamp(ys, 0.0, 1.0), pP)), 0.0), pB);
    float sb = 0.1 * W + aaU;
    float castS = smoothstep(ws + sb, ws - sb, abs(ds.x)) * smoothstep(-0.05, 0.15, ys) * smoothstep(1.08, 0.85, ys);
    float contact = smoothstep(w + 0.1 * W + aaU, w, au) * smoothstep(-0.05, 0.1, y) * smoothstep(1.06, 0.9, y);
    shadow = clamp(0.5 * castS + 0.35 * contact, 0.0, 0.8) * pShad * (1.0 - alpha);
    if (alpha < 0.003) return vec4(0.0);

    // ── 3D shading ──────────────────────────────────────────────────────
    float cx = clamp(u / max(w, 1e-3), -1.3, 1.3);
    vec3 nf = vec3(0.7 * pCup * cx * (0.6 + 0.4 * yc), 0.15 - 0.55 * yc * yc, 1.0);
    vec2 tb = normalize(vec2(sign(u) + 1e-4, slope));            // barb tangent (feather frame)
    vec2 pb = vec2(-tb.y, tb.x);
    nf.xy += pb * (ph - 0.5) * 1.1 * bFade * sign(u);            // barb ridges
    nf.xy += bump;
    nf.xy += pb * (hc - 0.5) * 0.45 * cFade * sign(u);           // clumps tilt
    float twist = (h - 0.5) * 0.5 + 0.3 * ruffle * wind * (0.4 + gBass) + 0.9 * flut;
    nf.x += twist * 0.45;
    nf.y -= 0.6 * gBass * (0.3 + yc);                          // bass lifts the tips toward us
    vec3 n = normalize(vec3(ud * nf.x + vd * nf.y - sp * pCurv, nf.z));
    vec3 T = vec3(ud * tb.x + vd * tb.y, 0.0);
    T = normalize(T - n * dot(T, n));
    vec3 V = vec3(0.0, 0.0, 1.0);
    vec3 Lk = gKey;
    vec3 H = normalize(Lk + V);
    float ndl = dot(n, Lk);
    float dif = clamp((ndl + 0.25) / 1.25, 0.0, 1.0);
    float TH = dot(T, H);
    float aniso = pow(sqrt(max(1.0 - TH * TH, 0.0)), pGloss) * smoothstep(-0.1, 0.3, ndl);
    float ndh = max(dot(n, H), 0.0);
    float lobe = pow(ndh, 6.0);
    float fres = pow(1.0 - clamp(n.z, 0.0, 1.0), 3.0);

    // pigment
    float tone = float(baseTone);
    if (tone > 1.5) alb = mix(alb * vec3(0.35, 0.45, 0.8), vec3(0.04, 0.06, 0.18), 0.35);
    else if (tone > 0.5) alb = mix(alb, vec3(0.9, 0.88, 0.84) * (0.75 + 0.25 * lum(alb) / max(lum(alb), 0.05)), 0.72);
    vec3 fillC = vec3(0.24, 0.26, 0.3) * (0.5 + 0.5 * n.y);
    vec3 cPig = alb * (vec3(1.0, 0.95, 0.88) * dif * 1.05 + fillC * 0.55);

    // structural iridescence: hue flips with view & normal angle, beat flashes
    float film = vn2(sp * 2.0 + vec2(gT * 0.1, -gT * 0.07)) * 1.2 + 1.6 * (1.0 - n.z) + 0.25 * y + h * 0.2;
    float beat = audioBeatPulse * audioReact;
    float shift = (1.0 - n.z) * 2.2 + 0.9 * (ndh - 0.7) + 0.6 * beat + 1.4 * gDrop + 0.5 * prismPatches * sin(film * 2.0);
    if (pk > 6.5) shift = (1.0 - ndh) * 4.0 + 0.5 * beat + 1.4 * gDrop;
    float shiftK = pk > 6.5 ? 1.0 : (pk > 4.5 && pk < 5.5 ? 0.9 : 0.45);
    vec3 sAlb = hueRot(alb, shift * shiftK);
    sAlb = max(sAlb, 0.0) * (1.0 + 0.25 * gPeak);
    float sBright = pk > 6.5 ? (0.025 + 1.6 * pow(ndh, 18.0) + 2.5 * pow(ndh, 90.0)) : (0.3 + 0.55 * dif + 1.5 * lobe);
    vec3 cStr = sAlb * sBright * (1.0 + 0.8 * beat);
    float iA = clamp(iw * min(iridescence, 1.0), 0.0, 1.0);
    vec3 c = mix(cPig, cStr, iA) + cStr * iw * max(iridescence - 1.0, 0.0) * 0.6;
    // holographic rainbow patches
    float patchM = smoothstep(0.55, 0.9, vn2(sp * 1.4 + vec2(-gT * 0.08, gT * 0.06) + h)) * prismPatches * (0.25 + 0.75 * iw) + 0.4 * gDrop;
    c += thinFilm(film) * lobe * patchM * iridescence * 0.6;

    // barb grooves, barbules and shimmer
    float groove = mix(0.86, 0.7 + 0.3 * smoothstep(1.0, 0.4, pd) + 0.06 * (hb - 0.5), bFadeC);
    float bbF = 1.0 - smoothstep(0.08, 0.2, cyc);
    float barbule = vn2(vec2(bcF * 2.0, au * pNB * 0.9 / L + h * 9.0));
    c *= groove * (1.0 + 0.25 * bbF * (barbule - 0.5));
    c *= mix(1.0, 0.8 + 0.4 * hc, cFade) * (1.0 - 0.5 * cLine);
    // anisotropic sheen along the barbs, tinted by thin film
    vec3 specC = mix(vec3(1.0, 0.97, 0.92), thinFilm(film + 0.3) * 1.3, (pk > 6.5 ? 0.2 : 0.7) * iw);
    float shimmer = 1.0 + 1.2 * highP() * audioReact * vn2(vec2(bi * 0.7, TIME * 9.0 + h * 10.0)) * bFade;
    c += specC * aniso * sheen * 0.55 * (0.45 + 0.55 * mix(0.75, smoothstep(1.0, 0.2, pd), bFade)) * shimmer;
    c += vec3(1.0) * pow(ndh, 60.0) * sheen * 0.25 * (pk > 4.5 && pk < 5.5 ? 2.0 : 1.0);
    // backlight: translucency through thin barbs, fringe and edges + rim
    float thin = clamp(sepZone * 0.8 + down * 0.5 + 0.15 + 0.6 * (1.0 - edge * strand), 0.0, 1.0);
    vec3 backC = vec3(1.0, 0.82, 0.6) * backlight * (0.55 + 0.45 * gLift);
    c += alb * backC * thin * (0.35 + 0.5 * smoothstep(0.5, 1.0, rel)) * (0.6 + 0.6 * max(-Lk.y * 0.0 + 0.4, 0.0));
    c += backC * fres * 0.18 * (0.4 + iw);
    // ambient occlusion: tucked base, shaft groove, edges curl away
    float ao = mix(0.5, 1.0, smoothstep(0.0, 0.3, y)) * (0.85 + 0.15 * smoothstep(0.0, 0.25, rel));
    c *= ao;

    // rachis shading: glossy cylinder
    float sx = clamp(u / swp, -1.0, 1.0);
    vec3 ns = normalize(vec3(ud * sx, sqrt(max(1.0 - sx * sx, 0.0)) + 0.2));
    float sdif = clamp(dot(ns, Lk) * 0.7 + 0.35, 0.0, 1.0);
    vec3 sc = shaftC * sdif * mix(0.5, 1.0, smoothstep(0.0, 0.3, y)) + vec3(1.0, 0.97, 0.9) * pow(max(dot(ns, H), 0.0), 40.0) * 0.9 * sheen;
    sc += shaftC * backC * 0.25;
    c = mix(c, sc, shaftCov);

    return vec4(c, alpha);
}

vec4 plumage(float k, vec2 sp, vec3 bg) {
    setPattern(k);
    float cellPx, phi, N, valid, blur;
    vec2 pc = layoutCoords(sp, cellPx, phi, N, valid, blur);
    vec3 col = bg;
    float A = 0.0;
    float r0 = floor(pc.y);
    // painter's order: downstream rows first, upstream rows (on top) last
    for (int jj = 0; jj < 8; jj++) {
        float fj = float(jj);
        if (fj > pJN) break;
        float row = r0 + 1.0 - fj;
        float px = pc.x - rowShift(row);
        float ic = floor(px);
        for (int i = -2; i <= 2; i++) {
            float fi = float(i);
            if (abs(fi) > pIN) continue;
            vec2 id = vec2(ic + fi, row);
            vec2 idw = id;
            if (N > 0.5) idw.x = mod(idw.x, N);
            float sh;
            vec4 f = feather(vec2(px - id.x, pc.y - row), idw, row, cellPx, blur, phi, sp, sh);
            sh *= valid;
            col *= 1.0 - sh;
            A = max(A, sh * 0.6);
            float a = f.a * valid;
            col = mix(col, f.rgb, a);
            A = A + a * (1.0 - A);
        }
    }
    if (pk > 3.5 && pk < 4.5) {
        float r = length(sp) / 0.05;
        vec3 nG = vec3(sp / 0.05, sqrt(max(1.0 - r * r, 0.0)));
        vec3 gem = thinFilm(r * 0.6 - gT * 0.3 + 0.2) * (0.25 + 0.9 * pow(max(dot(normalize(nG), normalize(gKey + vec3(0.0, 0.0, 1.0))), 0.0), 12.0)) * vec3(0.7, 0.8, 1.0);
        gem += vec3(1.0, 0.9, 0.7) * pow(max(dot(normalize(nG), gKey), 0.0), 3.0) * 0.25;
        col = mix(col * (1.0 - 0.6 * smoothstep(1.6, 1.0, r)), gem, smoothstep(1.02, 0.95, r));
        A = max(A, smoothstep(1.02, 0.95, r));
    }
    // aerial depth: out-of-focus regions sink into a soft haze
    float hz = clamp(blur / 6.0, 0.0, 1.0) * 0.45;
    col = mix(col, col * 0.55 + bg * 0.3, hz);
    return vec4(col, A);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    gT = TIME * 0.4 * flow + (audioMidTime * 0.3 + audioBassTime * 0.25) * ar * flow + 1.2 * gBuild;
    gBass = bassP() * ar;
    gFan = clamp(0.84 + 0.1 * gLift + 0.1 * gBuild + 0.06 * gDrop + 0.04 * gBass - 0.06 * gRest, 0.72, 1.12);
    // rest: closer and calmer (fewer, bigger feathers); drop: burst outward; build: tremor near the top
    sp *= (1.0 - 0.24 * gRest) / (1.0 + 0.16 * gDrop * gDrop + 0.04 * gBass);
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.005 * smoothstep(0.65, 1.0, gBuild);

    float ps = float(patternSel);
    float ph = TIME / morphPeriod;
    float lightSwing = 0.35 + 0.25 * gBuild;
    float la = gT * (0.35 + 0.6 * gBuild);
    gKey = normalize(vec3(-0.5 + lightSwing * sin(la), 0.62 + 0.15 * cos(la * 0.7), 0.62));

    float ka = ps > 0.5 ? ps - 1.0 : mod(floor(ph), 8.0);
    float kb = mod(ka + 1.0, 8.0);
    float fr = ps > 0.5 ? 0.0 : smoothstep(0.7, 1.0, fract(ph));
    // morph: the next pattern blooms out from the centre behind a glowing front
    float wv = length(sp) * 0.85 + 0.3 * (vn2(sp * 3.5 + 7.0) - 0.5);
    float front = fr * 1.35 - 0.15;
    float k = wv < front ? kb : ka;
    float seam = (fr > 0.001 && fr < 0.999) ? exp(-abs(wv - front) * 45.0) : 0.0;

    vec3 bg = backdrop(uv);
    vec4 P = plumage(k, sp, bg);
    vec3 col = P.rgb;
    float alpha = P.a;

    // lighting falloff: rest is a dim hush with a heavy vignette; peak opens up
    float rr = dot(sp, sp);
    col *= mix(0.4, 1.0, exp(-rr * (2.0 - 1.3 * gLift))) * (0.62 + 0.5 * gLift);
    vec3 prism = 0.5 + 0.5 * cos(TAU * (atan(sp.y, sp.x) / TAU + gT * 0.05 + vec3(0.0, 0.33, 0.67)));
    col += prism * exp(-rr * 3.0) * alpha * (0.12 * gPeak + 0.3 * gBuild * gBuild) * iridescence;
    col += thinFilm(wv * 3.0 + gT) * seam * 0.6;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    col += vec3(1.0, 0.95, 0.9) * gDrop * gDrop * 0.18 * alpha;
    col = col / (1.0 + 0.25 * max(col - 0.7, 0.0));                     // gentle highlight shoulder
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? max(alpha, seam) : 1.0);
}
