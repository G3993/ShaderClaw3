/*{
  "DESCRIPTION": "Geometric Flowers — an endless wall of thick glossy tiles, each carrying a sculpted geometric flower, flipping and turning in 3D like a giant split-flap board. Every flower is built from the overlapping-circle vocabulary of the Flower of Life: even-odd circle rosettes, a seven-circle Seed of Life bloom, a two-ring star lotus, a four-circle mandala with a halo of dots, a trefoil tulip with lens leaves, and a fourteen-petal daisy wheel. They are not decals: the petals are raised in stepped rings with rounded bevels, sitting inside a gilded frame on an engraved ground, with real parallax, soft shadows and contact occlusion between layers and faceted or domed glass gems at their hearts, lit by a warm key, a cool rim light and a studio softbox sweeping across the enamel, metal or glass. The grid morphs between Card Flip (tiles turning over in waves), Cube Roll (each cell a cube rolling to its next face), Prism Turn (triangular prisms like a trivision billboard), Ripple Lift (tiles rising and tilting on a travelling swell) and Scatter (tiles flying up, tumbling and settling back), and every flip reveals a new flower on the back face, sometimes a whole new pattern sweeping across the wall, sometimes a mix where neighbours always differ. The grid tilts and rolls in perspective and always fills the frame edge to edge. Bass pushes tiles toward you, every beat sends a ring of tilting tiles out from the centre, mids hurry the flips along and highs throw glints along the bevels. At rest the wall lies calm and flat with a single tile turning now and then; during a build the flip waves accelerate and the tiles tremble and lift; on the drop the whole wall explodes into a burst of tumbling tiles that falls back into place, and the peak is a constant cascade of flips in full colour.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "3D", "Geometric", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "gridSel",     "LABEL": "Grid Motion", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Auto Morph", "Card Flip", "Cube Roll", "Prism Turn", "Ripple Lift", "Scatter"], "GROUP": "Grid" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 6.0, "MAX": 60.0, "DEFAULT": 18.0, "GROUP": "Grid" },
    { "NAME": "gridSize",    "LABEL": "Tiles Across",     "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 14.0, "DEFAULT": 6.5, "GROUP": "Grid" },
    { "NAME": "tileGap",     "LABEL": "Tile Gap",         "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 0.3, "DEFAULT": 0.07, "GROUP": "Grid" },
    { "NAME": "flipSpeed",   "LABEL": "Flip Speed",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "waveSpeed",   "LABEL": "Wave Speed",       "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "waveDir",     "LABEL": "Wave Direction", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Auto", "Left → Right", "Top → Bottom", "Diagonal", "Centre Out", "Checker"], "GROUP": "Motion" },
    { "NAME": "camTilt",     "LABEL": "Camera Tilt",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Motion" },
    { "NAME": "camMotion",   "LABEL": "Camera Roll & Sway","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "flowerSel",   "LABEL": "Flower", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7], "LABELS": ["Auto / Mixed", "Rosette", "Seed of Life", "Star Lotus", "Mandala", "Tulip Block", "Daisy Wheel", "Mixed (neighbours differ)"], "GROUP": "Flowers" },
    { "NAME": "depthAmt",    "LABEL": "Depth / Extrusion","TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Flowers" },
    { "NAME": "bevel",       "LABEL": "Bevel",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Flowers" },
    { "NAME": "materialSel", "LABEL": "Material", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Jewel Mix (enamel, gold, glass)", "Enamel", "Metal", "Glass"], "GROUP": "Look" },
    { "NAME": "colourSel",   "LABEL": "Colours", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Bold Mix", "Ink & Vermilion", "Ivory & Ruby", "Jade & Saffron", "Cobalt & Pink", "Vermilion & Ivory"], "GROUP": "Look" },
    { "NAME": "shine",       "LABEL": "Shine",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
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

// ── globals ──────────────────────────────────────────────────────────────
float gClock, gFlipDur, gWaveK, gGrid, gHS;
vec3 gKey, gRimL;

// ── geometric flower height field (Flower-of-Life vocabulary, raised in layers) ──
// q in tile units (-0.5..0.5). returns height in [-1, 0] (0 = frame top, -1 = ground).
// sets gLayer (0 ground, 1 gilded frame, 2 outer petals, 3 inner petals, 4 gem) and gAO.
float gLayer, gAO, gBev, gPF;
float xorD(float a, float b) { return max(min(a, b), -max(a, b)); }       // even-odd (XOR) of two shapes
vec2 pfold(vec2 q, float n) {                                              // fold into one of n sectors around +y
    float s = TAU / n;
    float a = atan(q.x, q.y);
    a = mod(a + 0.5 * s, s) - 0.5 * s;
    return length(q) * vec2(sin(a), cos(a));
}
float sdVesica(vec2 p, float r, float d) {      // lens (two-circle intersection), axis along y
    p = abs(p);
    float b = sqrt(max(r * r - d * d, 0.0));
    return ((p.y - b) * d > p.x * b) ? length(p - vec2(0.0, b)) : length(p - vec2(-d, 0.0)) - r;
}
float sdRoundSeg(vec2 p, vec2 a, vec2 b, float r) { vec2 pa = p - a, ba = b - a; float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0); return length(pa - ba * h) - r; }
float ringXor(vec2 q, float n, float rr, float R, float off) {             // n circles on a ring, even-odd filled
    float d = 1e3;
    for (int k = 0; k < 8; k++) {
        if (float(k) > n - 0.5) break;
        float a = off + float(k) * TAU / n;
        d = xorD(d, length(q - rr * vec2(sin(a), cos(a))) - R);
    }
    return d;
}
float bevH(float d, float top, float thick, float w) {                     // rounded-bevel extrusion
    if (d > 0.0) return -9.0;
    float s = clamp(-d / w, 0.0, 1.0);
    return top - thick * (1.0 - sqrt(1.0 - (1.0 - s) * (1.0 - s)));
}

float flowerH(vec2 q, float type) {
    vec2 aq = abs(q);
    if (max(aq.x, aq.y) > 0.5) { gLayer = 1.0; gAO = 1.0; return 0.0; }    // tile body beyond the face
    float w = gBev;
    float d2 = 1e3, d3 = 1e3, d4 = 1e3;
    float gemR = 0.065, gemN = 8.0, facet = 1.0;
    vec2 gq = q;
    if (type < 0.5) {                                   // Rosette: 8 circles through the centre, even-odd + inner lens ring
        d2 = ringXor(q, 8.0, 0.19, 0.19, 0.0);
        vec2 f = pfold(rot(PI / 8.0) * q, 8.0);
        d3 = sdVesica(f - vec2(0.0, 0.135), 0.12, 0.085);
        gemR = 0.07;
    } else if (type < 1.5) {                            // Seed of Life: 7 circles even-odd, with a halo ring
        float d = length(q) - 0.15;
        d = xorD(d, ringXor(q, 6.0, 0.15, 0.15, 0.0));
        d3 = d;
        d2 = abs(length(q) - 0.385) - 0.035;
        vec2 f = pfold(rot(PI / 6.0) * q, 6.0);
        d2 = min(d2, length(f - vec2(0.0, 0.385)) - 0.06);
        gemR = 0.05; facet = 0.0;
    } else if (type < 2.5) {                            // Star Lotus: two rings of pointed petals
        vec2 f = pfold(q, 8.0);
        d2 = sdVesica(f - vec2(0.0, 0.225), 0.3, 0.255);
        vec2 g = pfold(rot(PI / 8.0) * q, 8.0);
        d3 = sdVesica(g - vec2(0.0, 0.14), 0.2, 0.162);
        gemR = 0.07; gemN = 6.0;
    } else if (type < 3.5) {                            // Mandala: four-circle flower, halo of beads, inner four-circle
        d2 = ringXor(q, 4.0, 0.175, 0.175, 0.0);
        vec2 f = pfold(rot(PI / 8.0) * q, 8.0);
        d2 = min(d2, length(f - vec2(0.0, 0.405)) - 0.04);
        d3 = ringXor(rot(PI / 4.0) * q, 4.0, 0.075, 0.075, 0.0);
        d3 = max(d3, length(q) - 0.15);
        gemR = 0.04; facet = 0.0;
    } else if (type < 4.5) {                            // Tulip: trefoil even-odd head, lens leaves, stem
        vec2 h = q - vec2(0.0, 0.1);
        d3 = ringXor(h, 3.0, 0.095, 0.13, PI / 3.0);
        d3 = max(d3, length(h) - 0.225);
        float st = sdRoundSeg(q, vec2(0.0, -0.4), vec2(0.0, 0.02), 0.024);
        vec2 l = vec2(abs(q.x), q.y) - vec2(0.105, -0.21);
        l = rot(0.7) * l;
        d2 = min(st, sdVesica(l, 0.16, 0.105));
        gq = h; gemR = 0.04; facet = 0.0;
    } else {                                            // Daisy Wheel: 14 long petals + 14 short, domed centre
        vec2 f = pfold(q, 14.0);
        d2 = sdRoundSeg(f, vec2(0.0, 0.16), vec2(0.0, 0.37), 0.04);
        vec2 g = pfold(rot(PI / 14.0) * q, 14.0);
        d3 = sdRoundSeg(g, vec2(0.0, 0.15), vec2(0.0, 0.25), 0.03);
        gemR = 0.12; facet = 0.0;
    }
    // engraved guilloché ground
    float r = length(q);
    float hc = -1.0 + 0.035 * (0.5 + 0.5 * sin(r * 150.0)) * smoothstep(0.02, 0.006, gPF);
    float ly = 0.0;
    // gilded frame
    float dB = max(aq.x, aq.y);
    float dR = max(0.455 - dB, dB - 0.5);
    float hh = bevH(dR, 0.0, 0.55, w * 0.7);
    if (hh > hc) { hc = hh; ly = 1.0; }
    hh = bevH(d2, -0.5, 0.48, w);
    if (hh > hc) { hc = hh; ly = 2.0; }
    hh = bevH(d3, -0.27, 0.45, w * 0.85);
    if (hh > hc) { hc = hh; ly = 3.0; }
    // gem: faceted brilliant or polished dome
    d4 = length(gq) - gemR;
    if (d4 < 0.0) {
        float gr = length(gq) / gemR;
        float hg;
        if (facet > 0.5) {
            vec2 pf = pfold(gq, gemN);
            hg = -0.03 - 0.5 * clamp((pf.y / gemR - 0.4) / 0.6, 0.0, 1.0);
        } else {
            hg = -0.04 - 0.55 * (1.0 - sqrt(max(1.0 - gr * gr, 0.0)));
        }
        if (hg > hc) { hc = hg; ly = 4.0; }
    }
    // contact occlusion: lower layers darken where they meet a higher one
    float ao = 1.0;
    if (ly < 0.5) ao *= 1.0 - 0.6 * exp(-max(dR, 0.0) / 0.03);
    if (ly < 1.5) ao *= 1.0 - 0.6 * exp(-max(d2, 0.0) / 0.03);
    if (ly < 2.5) ao *= 1.0 - 0.5 * exp(-max(d3, 0.0) / 0.025);
    if (ly < 3.5) ao *= 1.0 - 0.45 * exp(-max(d4, 0.0) / 0.02);
    gLayer = ly; gAO = ao;
    return hc;
}

// ── colours ──────────────────────────────────────────────────────────────
// scheme: ground, outer petals, inner petals, gem
void scheme(float s, out vec3 G, out vec3 A, out vec3 B, out vec3 C) {
    if (s < 0.5)      { G = vec3(0.022, 0.026, 0.045); A = vec3(0.93, 0.13, 0.07); B = vec3(0.96, 0.92, 0.84); C = vec3(1.0, 0.68, 0.14); }
    else if (s < 1.5) { G = vec3(0.92, 0.88, 0.79);    A = vec3(0.70, 0.03, 0.11); B = vec3(0.035, 0.03, 0.045); C = vec3(1.0, 0.62, 0.1); }
    else if (s < 2.5) { G = vec3(0.012, 0.15, 0.14);   A = vec3(1.0, 0.58, 0.05);  B = vec3(0.96, 0.92, 0.84); C = vec3(0.9, 0.05, 0.2); }
    else if (s < 3.5) { G = vec3(0.025, 0.07, 0.36);   A = vec3(1.0, 0.40, 0.56);  B = vec3(0.97, 0.94, 0.88); C = vec3(1.0, 0.72, 0.2); }
    else              { G = vec3(0.80, 0.09, 0.05);    A = vec3(0.97, 0.93, 0.85); B = vec3(0.03, 0.03, 0.04); C = vec3(1.0, 0.75, 0.2); }
}

// flower design & colour scheme for one tile face (cell c, flip number n)
float faceType(vec2 c, float n) {
    float fs = float(flowerSel);
    if (fs > 0.5 && fs < 6.5) return fs - 1.0;
    if (fs > 6.5 || mod(n, 3.0) > 1.5) return mod(c.x + 2.0 * c.y + 5.0 * n, 6.0);   // neighbours always differ
    return floor(h11(n * 1.73 + 0.31) * 6.0);                                         // one design sweeps the wall
}
float faceScheme(vec2 c, float n) {
    float cs = float(colourSel);
    if (cs > 0.5) return cs - 1.0;
    float fs = float(flowerSel);
    if (fs > 6.5 || mod(n, 3.0) > 1.5) return floor(h12(c * 1.37 + n * 0.71) * 5.0);
    return mod(floor(h11(n * 3.1 + 1.7) * 5.0) + mod(c.x + c.y, 2.0) * 2.0, 5.0);
}

// studio environment for reflections
vec3 env(vec3 d) {
    vec3 c = mix(vec3(0.03, 0.03, 0.04), vec3(0.16, 0.16, 0.19), smoothstep(-0.5, 1.0, d.z));
    c += vec3(1.0, 0.9, 0.76) * 1.5 * smoothstep(0.94, 0.99, dot(d, gKey));
    c += vec3(1.0, 0.93, 0.85) * 0.18 * smoothstep(0.6, 0.95, dot(d, gKey));
    c += vec3(0.6, 0.75, 1.0) * 1.3 * smoothstep(0.86, 0.97, dot(d, gRimL));
    c += vec3(0.5, 0.55, 0.65) * 0.25 * smoothstep(0.05, 0.0, abs(d.y + 0.15)) * step(0.0, d.z);
    return c;
}

mat3 axisRot(vec3 k, float a) {
    float c = cos(a), s = sin(a), t = 1.0 - c;
    return mat3(c + t * k.x * k.x,       t * k.x * k.y + s * k.z, t * k.x * k.z - s * k.y,
                t * k.x * k.y - s * k.z, c + t * k.y * k.y,       t * k.y * k.z + s * k.x,
                t * k.x * k.z + s * k.y, t * k.y * k.z - s * k.x, c + t * k.z * k.z);
}

float waveDelay(vec2 cc, vec2 c, float dsel) {
    if (dsel < 0.5) return cc.x;
    if (dsel < 1.5) return -cc.y;
    if (dsel < 2.5) return (cc.x - cc.y) * 0.707;
    if (dsel < 3.5) return length(cc);
    return mod(c.x + c.y, 2.0) * 2.0 + length(cc) * 0.4;
}
float waveAxis(vec2 cc, vec2 c, float dsel) {
    if (dsel < 0.5) return PI * 0.5;
    if (dsel < 1.5) return 0.0;
    if (dsel < 2.5) return PI * 0.25;
    if (dsel < 3.5) return atan(cc.y, cc.x) + PI * 0.5;
    return mod(c.x + c.y, 2.0) * PI * 0.5;
}
float autoDir(float i) { return floor(h11(i * 7.31 + 2.0) * 4.99); }

// tile animation for cell c: world-from-local matrix, centre, shape (N faces in the turn), flip count, size
void tileAnim(vec2 c, vec2 cc, out mat3 Mw, out vec3 ctr, out float N, out float K, out float hsT, out float fOff, out float phi) {
    float ar = audioReact;
    float h = h12(c * 1.31 + 0.7), h2 = h12(c * 2.17 + 5.3), h3 = h12(c * 0.73 + 9.1);
    // morph cycle (each tile switches shape at its own moment, rippling out from the centre)
    float tm = TIME / morphPeriod - length(cc) * 0.012 - h3 * 0.12;
    float mi = floor(tm), mf = fract(tm);
    float mode = gGrid > 0.5 ? gGrid - 1.0 : mod(mi, 5.0);
    float trans = gGrid > 0.5 ? 0.0 : smoothstep(0.965, 1.0, mf) + 1.0 - smoothstep(0.0, 0.035, mf);
    // wave direction (blends smoothly to the next one after each switch)
    float ds = float(waveDir);
    float dA = ds > 0.5 ? ds - 1.0 : autoDir(mi - 1.0);
    float dB = ds > 0.5 ? ds - 1.0 : autoDir(mi);
    float wb = smoothstep(0.0, 0.15, mf);
    float dly = mix(waveDelay(cc, c, dA), waveDelay(cc, c, dB), wb);
    phi = mix(waveAxis(cc, c, dA), waveAxis(cc, c, dB), wb);
    // squares only map onto themselves for 90° (cubes, prisms) or 45° (cards) axes
    float snapA = (mode > 0.5 && mode < 2.5) ? PI * 0.5 : PI * 0.25;
    phi = floor(phi / snapA + 0.5) * snapA;
    // flip phase: a wave across the grid; at rest the flips scatter randomly and rarely
    float f = gClock - mix(dly * gWaveK, h * 1.0, gRest);
    K = floor(f);
    float fr = fract(f);
    float dur = gFlipDur;
    if (mode > 2.5 && mode < 3.5) dur = max(dur, 0.45);
    if (mode > 3.5) dur = max(dur, 0.5);
    float e = smoothstep(1.0 - dur, 1.0, fr);                    // hold, then turn at the end of each cycle
    float x = clamp(fr / 0.12, 0.0, 1.0);
    e += 0.07 * sin(x * PI) * (1.0 - x);                          // mechanical overshoot & settle
    N = mode < 0.5 ? 2.0 : mode < 1.5 ? 4.0 : mode < 2.5 ? 3.0 : 2.0;
    float a = e * TAU / N;
    float flipPop = sin(PI * clamp(e, 0.0, 1.0));                 // tiles pop forward as they turn
    float lift = 0.0;
    vec2 lat = vec2(0.0);
    vec3 tax = normalize(vec3(h2 - 0.5, h3 - 0.5, 0.35 * (h - 0.5)));
    float tang = 0.0;
    if (mode > 2.5 && mode < 3.5) {                               // Ripple Lift: rise and tilt on a swell, turning at the crest
        float sw = fr - (1.0 - 0.5 * dur);
        lift = 0.45 * pow(0.5 + 0.5 * cos(TAU * sw), 3.0);
        a += 0.45 * sin(TAU * sw) * (1.0 - smoothstep(0.0, 0.2, abs(sw) - dur * 0.3));
    } else if (mode > 3.5) {                                     // Scatter: fly up, tumble, settle back
        float s = sin(PI * clamp((fr - 0.38) / 0.62, 0.0, 1.0));
        s *= s;
        lift = s * (0.55 + 0.75 * h);
        lat = s * 0.3 * (vec2(h2, h3) - 0.5);
        tang = s * (1.5 + 2.0 * h) * (h3 > 0.5 ? 1.0 : -1.0);
        a = smoothstep(0.55, 0.85, fr) * PI;
    }
    // audio + show dynamics
    lift += flipPop * (N < 2.5 ? 0.22 : 0.12);
    float dc = length(cc);
    float ringR = (1.0 - audioBeatPulse) * gridSize * 0.55;
    float kick = audioBeatPulse * ar * exp(-pow(dc - ringR, 2.0) * 0.8);
    lift += 0.3 * kick + bassP() * ar * 0.35 * (0.35 + 0.65 * h) * depthAmt;
    a += 0.6 * kick;
    lift += gBuild * 0.28 * h + gBuild * gBuild * 0.05 * sin(TIME * 37.0 + h * 40.0);
    tang += gBuild * gBuild * 0.09 * sin(TIME * 29.0 + h2 * 30.0);
    float dX = gDrop * gDrop;                                      // explosion snaps back quickly
    lift += dX * (0.25 + 0.7 * h);
    tang += dX * (1.0 + 1.6 * h2) * (h > 0.5 ? 1.0 : -1.0);
    lat += dX * 0.22 * (dc > 0.01 ? cc / dc : vec2(0.0)) * h3;
    lift += gPeak * 0.08 * sin(TIME * 2.3 + dc * 1.4);
    lift *= 1.0 - 0.75 * gRest;
    lift = min(lift, 1.4);
    // shape
    float sc = 1.0 - 0.8 * trans;
    tang += trans * 2.2;
    lift += 0.25 * trans;
    hsT = gHS * sc;
    float Hm = hsT * 0.16 * depthAmt;
    fOff = N < 2.5 ? Hm + hsT * 0.07 : N < 3.5 ? hsT / 1.7320508 : hsT;
    float cp = cos(phi), sp = sin(phi), ca = cos(a), sa = sin(a);
    mat3 Rz = mat3(cp, sp, 0.0, -sp, cp, 0.0, 0.0, 0.0, 1.0);
    mat3 Rx = mat3(1.0, 0.0, 0.0, 0.0, ca, sa, 0.0, -sa, ca);
    // cards turn about any in-plane axis and stay square to the grid; cubes & prisms use 90° axes
    float psi = N < 2.5 ? phi : 0.0;
    mat3 Rb = mat3(cos(psi), -sin(psi), 0.0, sin(psi), cos(psi), 0.0, 0.0, 0.0, 1.0);
    Mw = axisRot(tax, tang) * Rz * Rx * Rb;
    ctr = vec3(c + lat, lift - fOff);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    float aspect = R.x / R.y;

    gGrid = float(gridSel);
    gHS = 0.5 * (1.0 - tileGap);
    gClock = TIME * 0.11 * flipSpeed + audioMidTime * 0.09 * ar + 2.5 * gBuild * gBuild;
    gFlipDur = clamp(0.2 + 0.28 * gPeak - 0.13 * gRest + 0.1 * gBuild, 0.06, 0.55);
    gWaveK = 0.1 / waveSpeed;
    gKey = normalize(vec3(-0.42, 0.5, 0.76));
    gRimL = normalize(vec3(0.62, 0.72, 0.3));

    // build tremor (screen)
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.004 * smoothstep(0.6, 1.0, gBuild);

    // camera: tilted over the endless wall, slowly swaying and rolling
    float focal = 1.8;
    float D = gridSize * focal / aspect + 0.6;
    float pitch = clamp(camTilt * (0.62 + 0.18 * sin(TIME * 0.071)) * (1.0 - 0.45 * gRest) + 0.12 * gBuild + 0.08 * gPeak, 0.0, 0.72);
    float yaw = camMotion * (0.22 * sin(TIME * 0.043) + 0.06 * gPeak * sin(TIME * 0.31));
    float roll = camMotion * (0.07 * sin(TIME * 0.089 + 1.0) + 0.04 * gBuild * sin(TIME * 0.7)) * (1.0 - 0.5 * gRest);
    vec2 tgt = camMotion * vec2(0.35 * sin(TIME * 0.05), 0.25 * sin(TIME * 0.037 + 2.0));
    vec3 cdir = vec3(0.0, -sin(pitch), cos(pitch));
    cdir.xy = rot(-yaw) * cdir.xy;
    vec3 ro = vec3(tgt, 0.0) + D * cdir;
    vec3 ww = -cdir;
    vec3 upv = vec3(0.0, cos(pitch), sin(pitch));
    upv.xy = rot(-yaw) * upv.xy;
    vec3 uu = normalize(cross(ww, upv));
    vec3 vv = cross(uu, ww);
    vec2 rsp = rot(roll) * sp;
    float zoomK = 1.0 + 0.12 * gDrop;
    vec3 rd = normalize(rsp.x * uu + rsp.y * vv + focal * zoomK * ww);

    // find the nearest tile: test the cell under the ray (at mid height) and its 8 neighbours
    float zMid = 0.15 + 0.55 * gDrop + (gGrid > 4.5 ? 0.35 : 0.0) + 0.15 * gBuild;
    vec3 pm = ro + rd * ((zMid - ro.z) / rd.z);
    vec2 cell0 = floor(pm.xy + 0.5);
    vec2 camCell = tgt;
    float bestT = 1e9, best2 = 1e9;
    vec2 bC = vec2(0.0);
    mat3 bM = mat3(1.0);
    vec3 bCtr = vec3(0.0);
    float bN = 2.0, bK = 0.0, bHs = 0.4, bOff = 0.05, bPhi = 0.0, bPlane = 0.0;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 c = cell0 + vec2(float(i), float(j));
        mat3 Mw; vec3 ctr; float N, K, hsT, fOff, phi;
        tileAnim(c, c - camCell, Mw, ctr, N, K, hsT, fOff, phi);
        if (hsT < 0.004) continue;
        vec3 lo = (ro - ctr) * Mw;
        vec3 ld = rd * Mw;
        float tN = -1e9, tF = 1e9, kN = -1.0;
        for (int k = 0; k < 6; k++) {
            float fk = float(k);
            vec3 pn; float pd;
            if (fk < 1.5) { pn = vec3(fk < 0.5 ? 1.0 : -1.0, 0.0, 0.0); pd = hsT; }
            else {
                float jj = fk - 2.0;
                if (jj > N - 0.5) {
                    if (N > 2.5) break;
                    pn = vec3(0.0, jj < 2.5 ? 1.0 : -1.0, 0.0); pd = hsT;
                } else {
                    float th = jj * TAU / N;
                    pn = vec3(0.0, sin(th), cos(th)); pd = fOff;
                }
            }
            float dn = dot(pn, ld), num = pd - dot(pn, lo);
            if (abs(dn) < 1e-6) { if (num < 0.0) tN = 1e9; }
            else {
                float tt = num / dn;
                if (dn < 0.0) { if (tt > tN) { tN = tt; kN = fk; } }
                else tF = min(tF, tt);
            }
        }
        if (tN <= tF && tN > 0.0) {
            if (tN < bestT) {
                best2 = bestT;
                bestT = tN; bC = c; bM = Mw; bCtr = ctr; bN = N; bK = K; bHs = hsT; bOff = fOff; bPhi = phi; bPlane = kN;
            } else best2 = min(best2, tN);
        }
    }

    vec3 bg = backdrop(uv);
    // gaps: the backdrop, with soft contact shadow under the tiles
    vec3 gapCol = bg;
    {
        float tb = (-0.55 - ro.z) / rd.z;
        vec2 pb = (ro + rd * tb).xy;
        vec2 dq = abs(pb - floor(pb + 0.5));
        float e = max(dq.x, dq.y) - gHS;
        gapCol *= 0.45 + 0.55 * smoothstep(-0.05, 0.12, e);
    }
    vec3 col = gapCol;
    float alpha = 0.0;

    if (bestT < 1e8) {
        vec3 lo = (ro - bCtr) * bM;
        vec3 ld = rd * bM;
        vec3 lp = lo + ld * bestT;
        float pxW = bestT / (focal * R.y);
        // silhouette coverage (distance from entry point to the entry face's edges)
        float margin = 1e3;
        for (int k = 0; k < 6; k++) {
            float fk = float(k);
            if (abs(fk - bPlane) < 0.5) continue;
            vec3 pn; float pd;
            if (fk < 1.5) { pn = vec3(fk < 0.5 ? 1.0 : -1.0, 0.0, 0.0); pd = bHs; }
            else {
                float jj = fk - 2.0;
                if (jj > bN - 0.5) {
                    if (bN > 2.5) break;
                    pn = vec3(0.0, jj < 2.5 ? 1.0 : -1.0, 0.0); pd = bHs;
                } else {
                    float th = jj * TAU / bN;
                    pn = vec3(0.0, sin(th), cos(th)); pd = bOff;
                }
            }
            margin = min(margin, pd - dot(pn, lp));
        }
        float cover = clamp(margin / pxW * 0.8 + 0.15, 0.0, 1.0);

        vec3 nW;            // world normal
        vec3 alb;
        float mat;          // 0 enamel, 1 metal, 2 glass
        float ao = 1.0, sh = 1.0, steep = 0.0, lyr = 5.0;
        float msel = float(materialSel);
        float fsch;
        vec3 G, A, B, C;
        if (bPlane > 1.5 && bPlane - 2.0 < bN - 0.5) {
            // ── a flower face: relief-march the layered height field ──
            float jr = bPlane - 2.0;
            float th = jr * TAU / bN;
            vec3 nF = vec3(0.0, sin(th), cos(th));
            vec3 xL = vec3(1.0, 0.0, 0.0);
            vec3 vF = cross(nF, xL);
            vec3 uR = cos(bPhi) * xL - sin(bPhi) * vF;
            if (bN < 2.5) uR = jr < 0.5 ? xL : vec3(cos(2.0 * bPhi), sin(2.0 * bPhi), 0.0);   // keep the card's back face upright
            vec3 vR = cross(nF, uR);
            float n = bK + (jr < bN * 0.5 + 0.01 ? jr : jr - bN);
            float type = faceType(bC, n);
            fsch = faceScheme(bC, n);
            float s2 = 2.0 * bHs;
            vec2 q0 = vec2(dot(lp, uR), dot(lp, vR)) / s2;
            vec3 dF = vec3(dot(ld, uR), dot(ld, vR), dot(ld, nF));
            float Hq = 0.16 * depthAmt * 0.5;                    // relief depth in tile units
            gPF = pxW / s2 / max(-dF.z, 0.3);
            gBev = max(0.016 + 0.03 * bevel, 1.2 * gPF);
            float lxy = max(length(dF.xy), 1e-4);
            float tMax = min(Hq / max(-dF.z, 1e-3), 0.3 / lxy);
            vec3 lL = gKey * bM;
            vec3 lF = vec3(dot(lL, uR), dot(lL, vR), dot(lL, nF));
            vec2 lDir = lF.xy / max(length(lF.xy), 1e-4);
            float lSlope = lF.z / max(length(lF.xy), 1e-4);
            float eps = max(0.003, gPF);
            float t0 = 0.0, t1 = tMax, found = 0.0;
            float hc = 0.0, hx = 0.0, hy = 0.0, hs1 = -1.0, hs2 = -1.0;
            vec2 qh = q0;
            for (int i = 0; i < 21; i++) {
                vec2 qq;
                float tt = 0.0;
                if (i < 12) {
                    if (found > 0.5) continue;
                    tt = tMax * float(i + 1) / 12.0;
                    qq = q0 + dF.xy * tt;
                } else if (i < 16) {
                    tt = 0.5 * (t0 + t1);
                    qq = q0 + dF.xy * tt;
                } else {
                    qh = q0 + dF.xy * t1;
                    if (i == 16) qq = qh;
                    else if (i == 17) qq = qh + vec2(eps, 0.0);
                    else if (i == 18) qq = qh + vec2(0.0, eps);
                    else if (i == 19) qq = qh + lDir * 0.03;
                    else qq = qh + lDir * 0.085;
                }
                float hv = flowerH(qq, type);
                if (i < 16) {
                    bool below = dF.z * tt <= Hq * hv;
                    if (below) { t1 = tt; found = 1.0; } else t0 = tt;
                } else if (i == 16) { hc = hv; lyr = gLayer; ao = gAO; }
                else if (i == 17) hx = hv;
                else if (i == 18) hy = hv;
                else if (i == 19) hs1 = hv;
                else hs2 = hv;
            }
            vec3 nf = normalize(vec3(-Hq * (hx - hc) / eps, -Hq * (hy - hc) / eps, 1.0));
            steep = 1.0 - nf.z;
            // soft shadows from the higher layers
            if (lF.z > 0.0) {
                float e1 = Hq * hc + 0.03 * lSlope, e2 = Hq * hc + 0.085 * lSlope;
                sh *= 1.0 - 0.8 * clamp((Hq * hs1 - e1) / (0.25 * Hq), 0.0, 1.0);
                sh *= 1.0 - 0.6 * clamp((Hq * hs2 - e2) / (0.25 * Hq), 0.0, 1.0);
            } else sh = 0.0;
            nW = bM * (uR * nf.x + vR * nf.y + nF * nf.z);
            scheme(fsch, G, A, B, C);
            alb = lyr < 0.5 ? G : lyr < 1.5 ? vec3(0.95, 0.72, 0.32) : lyr < 2.5 ? A : lyr < 3.5 ? B : C;
            mat = msel < 0.5 ? (lyr > 0.5 && lyr < 1.5 ? 1.0 : lyr > 3.5 ? 2.0 : 0.0) : msel - 1.0;
            if (msel > 0.5 && msel < 1.5 && lyr > 0.5 && lyr < 1.5) alb = mix(G, B, 0.5) * 0.4 + vec3(0.5, 0.4, 0.25);
            ao *= 0.75 + 0.25 * smoothstep(-1.0, -0.4, hc);         // deep ground sits in shade
        } else {
            // ── tile body sides ──
            vec3 pn;
            float fk = bPlane;
            if (fk < 1.5) pn = vec3(fk < 0.5 ? 1.0 : -1.0, 0.0, 0.0);
            else pn = vec3(0.0, fk < 4.5 ? 1.0 : -1.0, 0.0);
            nW = bM * pn;
            fsch = faceScheme(bC, bK);
            scheme(fsch, G, A, B, C);
            alb = msel < 0.5 ? vec3(0.95, 0.72, 0.32) : mix(G, A, 0.3);
            mat = msel < 0.5 ? 1.0 : msel - 1.0;
            ao = 0.85;
            steep = 0.3;
        }
        // rest: a quieter palette; peak: full colour
        alb = mix(vec3(lum(alb)), alb, 0.72 + 0.28 * (1.0 - gRest) + 0.1 * gPeak);

        // ── lighting ──
        vec3 V = -rd;
        float ndv = clamp(dot(nW, V), 0.0, 1.0);
        vec3 rf = reflect(rd, nW);
        float dif = clamp(dot(nW, gKey), 0.0, 1.0) * sh;
        float rimD = clamp(dot(nW, gRimL), 0.0, 1.0);
        float hemi = 0.5 + 0.5 * nW.z;
        vec3 keyC = vec3(1.0, 0.93, 0.82) * 1.25;
        vec3 rimC = vec3(0.55, 0.7, 1.0);
        float fres = pow(1.0 - ndv, 5.0);
        vec3 en = env(rf);
        float spK = pow(clamp(dot(rf, gKey), 0.0, 1.0), 90.0) * sh;
        float spR = pow(clamp(dot(rf, gRimL), 0.0, 1.0), 40.0);
        float shn = shine;
        vec3 cl;
        if (mat < 0.5) {                     // glossy enamel / ceramic with a clear coat
            float F = 0.04 + 0.96 * fres;
            cl = alb * (dif * keyC + hemi * vec3(0.16, 0.17, 0.2) + rimD * rimC * 0.12) * ao;
            cl += (en * F * 0.9 + keyC * spK * 1.6 + rimC * spR * 0.5 * (0.3 + fres)) * shn * mix(0.5, 1.0, ao);
        } else if (mat < 1.5) {              // polished metal
            vec3 F = alb + (1.0 - alb) * fres;
            cl = F * (en * 1.1 + keyC * spK * 2.2 + rimC * spR * 0.8) * shn * ao;
            cl += alb * (dif * 0.28 + 0.06) * ao;
        } else {                             // glass / gem: tinted body glow + strong reflections
            float F = 0.06 + 0.94 * fres;
            float body = 0.35 + 0.65 * pow(ndv, 1.5);
            cl = alb * (0.22 * dif * keyC + 0.5 * body * (0.4 + 0.6 * sh)) * ao;
            cl += alb * alb * 0.6 * pow(clamp(dot(-nW, gKey) * 0.5 + 0.5, 0.0, 1.0), 2.0);
            cl += (en * F * 1.3 + keyC * spK * 2.5 + rimC * spR * 0.9) * shn;
        }
        // glints along the bevels on the highs
        float gl = steep * smoothstep(0.15, 0.6, steep) * (0.25 + 0.75 * highP() * ar) * (0.6 + 0.4 * sin(TIME * 9.0 + dot(bC, vec2(3.1, 1.7))));
        cl += keyC * gl * pow(clamp(dot(rf, gKey) * 0.5 + 0.5, 0.0, 1.0), 6.0) * 1.2 * shn;
        // distance haze toward the backdrop so the far wall recedes
        float haze = 1.0 - exp(-max(bestT - D, 0.0) * 0.08);
        cl = mix(cl, bg * 0.6 + cl * 0.25, haze * 0.6);
        vec3 behind = best2 < 1e8 ? cl * 0.35 : gapCol;
        col = mix(behind, cl, cover);
        alpha = mix(best2 < 1e8 ? 1.0 : 0.0, 1.0, cover);
    }

    // finish
    col *= 1.0 - 0.28 * smoothstep(0.55, 1.25, length(sp * vec2(0.8, 1.0)));
    col += vec3(1.0, 0.95, 0.9) * gDrop * gDrop * 0.12;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    col = col / (1.0 + 0.15 * max(col - 0.8, 0.0));                  // gentle highlight shoulder
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(col, transparentBg ? alpha : 1.0);
}
