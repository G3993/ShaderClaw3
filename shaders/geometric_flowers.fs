/*{
  "DESCRIPTION": "Geometric Flowers — an endless wall of thick glossy tiles, each one a small jewelled object, flipping and turning in 3D like a giant split-flap board, and no two neighbours alike. Twenty sculpted blooms live on the faces: the overlapping-circle Flower of Life designs (even-odd rosette, seven-circle Seed of Life, two-ring star lotus, four-circle mandala, trefoil tulip, fourteen-petal daisy wheel) and a botanical petal engine that grows poppies with four cupped petals, layered chrysanthemums, toothed cosmos, swirling hibiscus with a pollen column, six-point star lilies, spiky proteas, spiralling dahlias, three-up three-down irises, camellias, art-deco sunburst fans, star-throated morning glories, heart-leaved four-leaf clovers, notched sakura and an interlaced tessellated rose. Every face draws a fresh combination: petal count, petal shape (round, pointed, notched, spoon, teardrop, lanceolate), twist and layer offset; a single bloom, a cluster of three to five, an offset cropped bloom, a flower on a stem with leaves, or a quarter-flower that joins with its three neighbours into one big flower across four tiles; a ground of guilloché, sunburst rays, concentric rings, checker, chevron, raised pearls, crackle glaze, terrazzo chips, basket weave, gradient or a twinkling night sky; a frame that is absent, thin gold, thick bevelled, double-lined, scalloped, beaded, octagonal or circular inset, corner ornaments or a medallion; relief depth, domed or recessed-panel faces; enamel, gold, silver, rose gold, glass, ceramic glaze, matte clay, iridescent film or velvet; and a colour harmony that is complementary, analogous, monochrome, triadic or inverse (dark flower on a light ground). The petals are raised in stepped bevelled rings with fine striations, real parallax, soft shadows and contact occlusion, lit by a warm key, a cool rim and a studio softbox. The grid morphs between Card Flip, Cube Roll, Prism Turn, Ripple Lift and Scatter, and every flip reveals a new design. Bass pushes tiles toward you, every beat sends a ring of tilting tiles out from the centre, mids hurry the flips, highs throw glints along the bevels and make the night-sky stars twinkle. At rest the wall lies calm and flat with a single block turning now and then; during a build the flip waves accelerate and the tiles tremble and lift; on the drop the whole wall explodes into tumbling tiles that fall back into place, and the peak is a constant cascade of flips in full colour.",
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
    { "NAME": "flowerSel",   "LABEL": "Flower", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21], "LABELS": ["Auto / Mixed", "Rosette", "Seed of Life", "Star Lotus", "Mandala", "Tulip Block", "Daisy Wheel", "Mixed (neighbours differ)", "Poppy", "Chrysanthemum", "Cosmos", "Hibiscus", "Star Lily", "Protea", "Dahlia", "Iris", "Camellia", "Art-Deco Sunburst", "Morning Glory", "Four-Leaf Clover", "Sakura", "Tessellated Rose"], "GROUP": "Flowers" },
    { "NAME": "variety",     "LABEL": "Tile Variety",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Flowers" },
    { "NAME": "layoutSel",   "LABEL": "Tile Layout", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Random", "Single Flower", "Cluster (3-5)", "Quarter (joins 4 tiles)", "Offset / Cropped", "Flower & Leaves"], "GROUP": "Flowers" },
    { "NAME": "groundSel",   "LABEL": "Tile Ground", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12], "LABELS": ["Random", "Plain", "Guilloché", "Sunburst Rays", "Concentric Rings", "Checker", "Chevron", "Dotted", "Crackle Glaze", "Terrazzo", "Woven", "Gradient", "Starry Night"], "GROUP": "Flowers" },
    { "NAME": "frameSel",    "LABEL": "Tile Frame", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], "LABELS": ["Random", "No Frame", "Thin Gold", "Thick Bevelled", "Double Line", "Scalloped", "Beaded", "Octagonal Inset", "Circular Inset", "Corner Ornaments", "Inset Medallion"], "GROUP": "Flowers" },
    { "NAME": "depthAmt",    "LABEL": "Depth / Extrusion","TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Flowers" },
    { "NAME": "bevel",       "LABEL": "Bevel",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Flowers" },
    { "NAME": "materialSel", "LABEL": "Material", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7], "LABELS": ["Jewel Mix (varies per tile)", "Enamel", "Metal", "Glass", "Ceramic Glaze", "Matte Clay", "Iridescent", "Velvet"], "GROUP": "Look" },
    { "NAME": "colourSel",   "LABEL": "Colours", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Bold Mix", "Ink & Vermilion", "Ivory & Ruby", "Jade & Saffron", "Cobalt & Pink", "Vermilion & Ivory"], "GROUP": "Look" },
    { "NAME": "shine",       "LABEL": "Shine",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
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

// ── per-face design parameters (all hash-driven, set once per pixel before the relief march) ──
float gType, gFrame, gLayout, gGround, gInset, gDome, gH2, gH3;
float gShapeOv, gNAdj, gTwist, gOffL, gOneL, gClus, gMS, gDepthK, gGemV;
vec2 gQC, gLay, gGC, gFQ;
float gQS, gGS, gFS;
vec3 gG, gG2, gA, gB, gC, gL, gFM, gFM2;

// ── geometric flower height field (Flower-of-Life vocabulary + a botanical petal engine, raised in layers) ──
// q in tile units (-0.5..0.5). returns height in [-1, 0] (0 = frame top, -1 = ground).
// sets gLayer (0 ground, 1 frame, 2 outer petals, 3 inner petals, 4 gem, 5 leaves & stem) and gAO.
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
float sdLens(vec2 p, float b, float w) {        // lens of half-length b, half-width w
    float R = 0.5 * (w + b * b / w);
    return sdVesica(p, R, R - w);
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
// one petal along +y from the base (0,0) to length L, max half-width w — a single profile-driven code path
// shapes: 0 round, 1 pointed, 2 notched, 3 spoon, 4 teardrop, 5 lanceolate, 6 heart
float petalD(vec2 f, float L, float w, float shp) {
    float a = 0.55, b = 0.38, nr = 0.0;
    if (shp > 0.5 && shp < 1.5)      { a = 0.65; b = 0.85; }
    else if (shp < 2.5 && shp > 1.5) { nr = 0.4; }
    else if (shp < 3.5 && shp > 2.5) { a = 2.6; b = 0.42; }
    else if (shp < 4.5 && shp > 3.5) { a = 1.25; b = 0.45; }
    else if (shp < 5.5 && shp > 4.5) { a = 0.9; b = 0.9; w *= 0.62; }
    else if (shp > 5.5)              { a = 0.75; b = 0.3; nr = 0.62; }
    float tm = a / (a + b);
    float nrm = 1.0 / (pow(tm, a) * pow(1.0 - tm, b));
    float t = clamp(f.y / L, 0.0, 1.0);
    float wd = w * nrm * pow(t, a) * pow(1.0 - t, b);
    float d = max((abs(f.x) - wd) * 0.75, max(-f.y, f.y - L));
    if (nr > 0.0) d = max(d, -(length(f - vec2(0.0, L + w * 0.1)) - w * nr));
    return d;
}
// a ring of n petals. P = (n, r0, r1, halfWidth), S = (shape, twist, angle offset, seam between petals)
float petalRing(vec2 q, vec4 P, vec4 S, float spir) {
    float r = length(q);
    vec2 f = pfold(rot(S.z + spir * r) * q, P.x);
    float sct = PI / P.x;
    float a = atan(f.x, f.y);
    vec2 g = rot(S.y) * (f - vec2(0.0, P.y));
    float d = petalD(g, P.z - P.y, P.w, S.x);
    if (S.w > 0.0) d = max(d, S.w - r * sin(sct - abs(a)));               // each petal keeps its own bevel
    return d;
}

float flowerH(vec2 q) {
    vec2 aq = abs(q);
    if (max(aq.x, aq.y) > 0.5) { gLayer = 1.0; gAO = 1.0; return 0.0; }    // tile body beyond the face
    float w = gBev;
    // ── layout: tile coords → flower space ──
    vec2 fc = vec2(0.0);
    float fs = 1.0, frot = 0.0;
    if (gLayout > 0.5 && gLayout < 1.5) {                // cluster of 3–5 small blooms (nearest one is drawn)
        float best = 1e3;
        for (int k = 0; k < 5; k++) {
            float fk = float(k);
            if (fk > gClus - 0.5) break;
            vec2 ck; float sk;
            if (gClus < 3.5)      { float a = fk * TAU / 3.0; ck = 0.235 * vec2(sin(a), cos(a)); sk = 0.42; }
            else if (gClus < 4.5) { ck = (vec2(mod(fk, 2.0), floor(fk * 0.5)) - 0.5) * 0.47; sk = 0.44; }
            else { float a = PI * 0.25 + (fk - 1.0) * PI * 0.5; ck = fk < 0.5 ? vec2(0.0) : 0.465 * vec2(sin(a), cos(a)); sk = fk < 0.5 ? 0.52 : 0.28; }
            float dk = length(q - ck) - 0.45 * sk;
            if (dk < best) { best = dk; fc = ck; fs = sk; frot = fk * 1.13; }
        }
    } else if (gLayout < 2.5 && gLayout > 1.5) { fc = gQC; fs = gQS; }    // quarter: one big bloom across 4 tiles
    else if (gLayout > 2.5 && gLayout < 3.5) { fc = gLay; fs = 1.32; }  // offset & cropped
    else if (gLayout > 3.5) { fc = vec2(0.0, 0.13); fs = 0.66; }         // flower on a stem with leaves
    vec2 fq = rot(frot) * (q - fc) / fs;
    gFQ = fq; gFS = fs;
    float d2 = 1e3, d3 = 1e3, d4 = 1e3;
    float gemR = 0.065, gemN = 8.0, facet = 1.0;
    vec2 gq = fq;
    float type = gType;
    vec4 P2 = vec4(8.0, 0.0, 0.44, 0.1), S2 = vec4(0.0), P3 = vec4(0.0), S3 = vec4(0.0);
    vec4 R0 = vec4(0.0), R1 = vec4(0.0);            // legacy even-odd circle rings (n, ring r, circle r, offset)
    float spir = 0.0;
    vec2 pq = fq, rq = fq;
    if (type < 5.5) {
        if (type < 0.5) {                                   // Rosette: 8 circles through the centre, even-odd + inner lens ring
            R0 = vec4(8.0, 0.19, 0.19, 0.0);
            vec2 f = pfold(rot(PI / 8.0) * fq, 8.0);
            d3 = sdVesica(f - vec2(0.0, 0.135), 0.12, 0.085);
            gemR = 0.07;
        } else if (type < 1.5) {                            // Seed of Life: 7 circles even-odd, with a halo ring
            R0 = vec4(6.0, 0.15, 0.15, 0.0);
            d2 = abs(length(fq) - 0.385) - 0.035;
            vec2 f = pfold(rot(PI / 6.0) * fq, 6.0);
            d2 = min(d2, length(f - vec2(0.0, 0.385)) - 0.06);
            gemR = 0.05; facet = 0.0;
        } else if (type < 2.5) {                            // Star Lotus: two rings of pointed petals
            vec2 f = pfold(fq, 8.0);
            d2 = sdVesica(f - vec2(0.0, 0.225), 0.3, 0.255);
            vec2 g = pfold(rot(PI / 8.0) * fq, 8.0);
            d3 = sdVesica(g - vec2(0.0, 0.14), 0.2, 0.162);
            gemR = 0.07; gemN = 6.0;
        } else if (type < 3.5) {                            // Mandala: four-circle flower, halo of beads, inner four-circle
            R0 = vec4(4.0, 0.175, 0.175, 0.0);
            R1 = vec4(4.0, 0.075, 0.075, PI / 4.0);
            vec2 f = pfold(rot(PI / 8.0) * fq, 8.0);
            d2 = length(f - vec2(0.0, 0.405)) - 0.04;
            gemR = 0.04; facet = 0.0;
        } else if (type < 4.5) {                            // Tulip: trefoil even-odd head, lens leaves, stem
            vec2 h = fq - vec2(0.0, 0.1);
            R0 = vec4(3.0, 0.095, 0.13, PI / 3.0);
            rq = h;
            float st = sdRoundSeg(fq, vec2(0.0, -0.4), vec2(0.0, 0.02), 0.024);
            vec2 l = vec2(abs(fq.x), fq.y) - vec2(0.105, -0.21);
            l = rot(0.7) * l;
            d2 = min(st, sdVesica(l, 0.16, 0.105));
            gq = h; gemR = 0.04; facet = 0.0;
        } else {                                            // Daisy Wheel: 14 long petals + 14 short, domed centre
            vec2 f = pfold(fq, 14.0);
            d2 = sdRoundSeg(f, vec2(0.0, 0.16), vec2(0.0, 0.37), 0.04);
            vec2 g = pfold(rot(PI / 14.0) * fq, 14.0);
            d3 = sdRoundSeg(g, vec2(0.0, 0.15), vec2(0.0, 0.25), 0.03);
            gemR = 0.12; facet = 0.0;
        }
    } else {
        // botanical petal engine: two petal rings (outer, inner) + heart, all parameterised per flower
        facet = 0.0;
        if (type < 6.5) {        // Poppy: four broad cupped petals, crossed inner four, seed-pod dome
            P2 = vec4(4.0, 0.0, 0.445, 0.205); S2 = vec4(0.0, 0.0, 0.0, 0.014);
            P3 = vec4(4.0, 0.0, 0.3, 0.15);    S3 = vec4(0.0, 0.0, PI / 4.0, 0.012);
            gemR = 0.085;
        } else if (type < 7.5) { // Chrysanthemum: many thin layered petals
            P2 = vec4(22.0, 0.1, 0.45, 0.03);  S2 = vec4(5.0, 0.0, 0.0, 0.0);
            P3 = vec4(16.0, 0.04, 0.31, 0.036); S3 = vec4(5.0, 0.0, PI / 16.0, 0.0);
            gemR = 0.06;
        } else if (type < 8.5) { // Cosmos: eight toothed petals, ring of stamens
            P2 = vec4(8.0, 0.04, 0.445, 0.11); S2 = vec4(2.0, 0.0, 0.0, 0.01);
            P3 = vec4(14.0, 0.07, 0.14, 0.018); S3 = vec4(0.0, 0.0, 0.0, 0.0);
            gemR = 0.07;
        } else if (type < 9.5) { // Hibiscus: five swirling petals, staminal column
            P2 = vec4(5.0, 0.0, 0.445, 0.2);   S2 = vec4(0.0, 0.38, 0.0, 0.014);
            P3 = vec4(5.0, 0.02, 0.17, 0.045); S3 = vec4(1.0, -0.3, PI / 5.0, 0.0);
            gemR = 0.035;
        } else if (type < 10.5) { // Star Lily: six-point star of pointed petals over a second star
            P2 = vec4(6.0, 0.0, 0.455, 0.078); S2 = vec4(1.0, 0.0, 0.0, 0.0);
            P3 = vec4(6.0, 0.0, 0.33, 0.055);  S3 = vec4(1.0, 0.0, PI / 6.0, 0.0);
            gemR = 0.05; facet = 1.0; gemN = 6.0;
        } else if (type < 11.5) { // Protea: crown of spiky bracts around a great domed head
            P2 = vec4(14.0, 0.13, 0.445, 0.048); S2 = vec4(1.0, 0.0, 0.0, 0.0);
            P3 = vec4(10.0, 0.06, 0.3, 0.06);  S3 = vec4(1.0, 0.0, PI / 10.0, 0.006);
            gemR = 0.12;
        } else if (type < 12.5) { // Dahlia: spiralling rings of spoon petals
            P2 = vec4(16.0, 0.14, 0.45, 0.045); S2 = vec4(3.0, 0.0, 0.0, 0.006);
            P3 = vec4(12.0, 0.04, 0.29, 0.042); S3 = vec4(4.0, 0.0, PI / 12.0, 0.005);
            spir = 2.4; gemR = 0.06;
        } else if (type < 13.5) { // Iris: three falls down, three standards up
            P2 = vec4(3.0, 0.0, 0.445, 0.17);  S2 = vec4(0.0, 0.0, PI, 0.014);
            P3 = vec4(3.0, 0.0, 0.38, 0.095);  S3 = vec4(1.0, 0.0, 0.0, 0.012);
            gemR = 0.04;
        } else if (type < 14.5) { // Camellia: layered rounded petals, offset rows
            P2 = vec4(7.0, 0.0, 0.445, 0.16);  S2 = vec4(0.0, 0.0, 0.0, 0.013);
            P3 = vec4(7.0, 0.0, 0.29, 0.115);  S3 = vec4(0.0, 0.0, PI / 7.0, 0.011);
            gemR = 0.075; facet = 1.0; gemN = 7.0;
        } else if (type < 15.5) { // Art-Deco Sunburst: fan of blades over stepped arcs
            pq = fq + vec2(0.0, 0.17);
            P2 = vec4(18.0, 0.15, 0.5, 0.05);  S2 = vec4(4.0, 0.0, 0.0, 0.0);
            gq = pq; gemR = 0.075; facet = 1.0; gemN = 10.0;
        } else if (type < 16.5) { // Morning Glory: scalloped funnel with a raised five-point star
            P2 = vec4(5.0, 0.06, 0.39, 0.024); S2 = vec4(5.0, 0.0, 0.0, 0.0);
            gemR = 0.07;
        } else if (type < 17.5) { // Four-Leaf Clover: heart leaves with raised midribs
            P2 = vec4(4.0, 0.02, 0.44, 0.135); S2 = vec4(6.0, 0.0, PI / 4.0, 0.012);
            P3 = vec4(4.0, 0.03, 0.3, 0.012);  S3 = vec4(5.0, 0.0, PI / 4.0, 0.0);
            gemR = 0.035;
        } else if (type < 18.5) { // Sakura: five notched petals, spoon stamens
            P2 = vec4(5.0, 0.0, 0.435, 0.16);  S2 = vec4(2.0, 0.0, 0.0, 0.014);
            P3 = vec4(10.0, 0.05, 0.18, 0.02); S3 = vec4(3.0, 0.0, PI / 10.0, 0.0);
            gemR = 0.04;
        } else {                  // Tessellated Rose: interlaced squares (handled below)
            gemR = 0.075; facet = 1.0; gemN = 8.0;
        }
        // per-tile variation of the petal engine: count, petal shape, twist, layer offset, layer count
        if (type < 14.5 || type > 16.5) {
            if (P2.x > 7.5) P2.x += floor(gNAdj * 3.0) * 2.0 - 2.0;
            if (P3.x > 7.5) P3.x += floor(gNAdj * 3.0) * 2.0 - 2.0;
            if (gShapeOv > -0.5 && type > 6.5) S2.x = gShapeOv;
            S2.y += gTwist;
            S3.y -= gTwist * 0.6;
            if (gOffL > 0.5 && P3.x > 0.5) S3.z += PI / P3.x;
            if (gOneL > 0.5) P3.x = 0.0;
        }
    }
    // the two rings of the flower, evaluated in one loop (one call site each for the compiler)
    float rv0 = 1e3, rv1 = 1e3;
    for (int k = 0; k < 2; k++) {
        float d = 1e3;
        if (type < 5.5) {
            vec4 Rk = k == 0 ? R0 : R1;
            if (Rk.x > 0.5) d = ringXor(rq, Rk.x, Rk.y, Rk.z, Rk.w);
        } else {
            vec4 Pk = k == 0 ? P2 : P3;
            if (Pk.x > 0.5) d = petalRing(pq, Pk, k == 0 ? S2 : S3, k == 0 ? spir : spir * 0.6);
        }
        if (k == 0) rv0 = d; else rv1 = d;
    }
    if (type < 5.5) {
        if (type < 0.5) d2 = rv0;
        else if (type < 1.5) d3 = xorD(length(fq) - 0.15, rv0);
        else if (type > 2.5 && type < 3.5) { d2 = min(d2, rv0); d3 = max(rv1, length(fq) - 0.15); }
        else if (type > 3.5 && type < 4.5) d3 = max(rv0, length(rq) - 0.225);
    } else {
        d2 = rv0; d3 = rv1;
        if (type > 8.5 && type < 9.5) {            // hibiscus column with a pollen tip
            d3 = min(d3, min(sdRoundSeg(fq, vec2(0.0), vec2(0.1, 0.27), 0.016), length(fq - vec2(0.1, 0.27)) - 0.035));
        } else if (type > 14.5 && type < 15.5) {   // sunburst: upper half only, arcs below the fan
            d2 = max(d2, -pq.y);
            float r = length(pq);
            d3 = max(min(abs(r - 0.115) - 0.022, abs(r - 0.43) - 0.018), -pq.y);
            d2 = min(d2, max(abs(pq.y - 0.012) - 0.012, abs(pq.x) - 0.47));
        } else if (type > 15.5 && type < 16.5) {   // morning glory funnel
            float a = atan(fq.x, fq.y);
            d3 = d2;
            d2 = length(fq) - 0.405 - 0.028 * cos(5.0 * a);
            d3 = min(d3, abs(length(fq) - 0.13) - 0.025);
        } else if (type > 18.5) {                  // tessellated rose: two interlaced squares, inner pair turned 22.5°
            vec2 q1 = rot(PI / 4.0) * fq;
            d2 = xorD(max(abs(fq.x), abs(fq.y)) - 0.33, max(abs(q1.x), abs(q1.y)) - 0.33);
            d2 = min(d2, abs(length(fq) - 0.44) - 0.016);
            vec2 q2 = rot(PI / 8.0) * fq, q3 = rot(3.0 * PI / 8.0) * fq;
            d3 = xorD(max(abs(q2.x), abs(q2.y)) - 0.2, max(abs(q3.x), abs(q3.y)) - 0.2);
        }
    }
    gemR *= gGemV;
    d2 *= fs; d3 *= fs;
    // engraved ground (the fine pattern itself is added as a bump at shading time)
    float hc = -1.0;
    if (gInset > 0.5) {                                   // recessed panel: raised surround
        float dP = gFrame > 5.5 && gFrame < 7.5 ? length(q) - 0.36 : max(aq.x, aq.y) - 0.34;
        hc = max(hc, bevH(-dP, -0.74, 0.26, w));
    }
    float ly = 0.0;
    // frame variants
    float dB = max(aq.x, aq.y);
    float edge = dB - 0.5;
    float dR = 1e3, fTop = 0.0, fTh = 0.55, fW = w * 0.7;
    float fr = gFrame;
    if (fr < 0.5) { }                                                                 // no frame
    else if (fr < 1.5) { dR = max(0.478 - dB, edge); fTh = 0.4; }                     // thin gold
    else if (fr < 2.5) { dR = max(0.425 - dB, edge); fW = w * 1.6 + 0.02; }           // thick bevelled
    else if (fr < 3.5) { dR = min(abs(dB - 0.485) - 0.012, abs(dB - 0.447) - 0.009); fTh = 0.35; }   // double line
    else if (fr < 4.5) {                                                              // scalloped
        float t = aq.x > aq.y ? q.y : q.x;
        dR = max(0.452 - 0.032 * abs(cos(t * PI * 6.0)) - dB, edge);
    } else if (fr < 5.5) {                                                            // beaded
        float t = aq.x > aq.y ? q.y : q.x;
        vec2 bp = vec2(dB - 0.452, mod(t + 0.5, 1.0 / 13.0) - 0.5 / 13.0);
        dR = min(max(0.485 - dB, edge), length(bp) - 0.016);
        fTh = 0.45;
    } else if (fr < 6.5) {                                                            // octagonal inset
        float dO = max(dB, (aq.x + aq.y) * 0.7071) - 0.455;
        dR = max(-dO, edge);
    } else if (fr < 7.5) { dR = max(0.465 - length(q), edge); }                       // circular inset
    else if (fr < 8.5) {                                                              // corner ornaments only
        vec2 cq = aq - 0.5;
        float rc = length(cq);
        dR = min(abs(rc - 0.15) - 0.016, rc - 0.075);
        dR = min(dR, max(abs(min(-cq.x, -cq.y) - 0.012) - 0.009, rc - 0.29));
        fTh = 0.42;
    } else {                                                                          // inset medallion
        dR = min(max(0.484 - dB, edge), abs(length(q) - 0.44) - 0.014);
        fTh = 0.42;
    }
    float hh = bevH(dR, fTop, fTh, fW);
    if (hh > hc) { hc = hh; ly = 1.0; }
    // stem & leaves
    float dl = 1e3;
    if (gLayout > 3.5) {
        vec2 qb = vec2(q.x - 0.035 * sin((q.y + 0.5) * 4.0), q.y);
        dl = sdRoundSeg(qb, vec2(0.0, -0.5), vec2(0.0, 0.05), 0.017);
        vec2 lq = vec2(abs(qb.x), qb.y) - vec2(0.0, -0.3);
        lq = rot(-0.95) * lq;
        dl = min(dl, sdLens(lq - vec2(0.0, 0.13), 0.13, 0.055));
        hh = bevH(dl, -0.62, 0.3, w * 0.8);
        if (hh > hc) { hc = hh; ly = 5.0; }
    }
    hh = bevH(d2, gH2, 0.48, w);
    if (hh > hc) { hc = hh; ly = 2.0; }
    hh = bevH(d3, gH3, 0.45, w * 0.85);
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
    d4 *= fs;
    // pillowed (domed) faces
    hc = max(hc - gDome * dot(q, q) * 0.9, -1.0);
    // contact occlusion: lower layers darken where they meet a higher one
    float ao = 1.0;
    if (ly < 0.5) ao *= 1.0 - 0.6 * exp(-max(dR, 0.0) / 0.03);
    if (ly < 0.5 || ly > 4.5) ao *= 1.0 - 0.6 * exp(-max(d2, 0.0) / 0.03);
    if (ly < 0.5) ao *= 1.0 - 0.5 * exp(-max(dl, 0.0) / 0.025);
    if (ly < 2.5 || ly > 4.5) ao *= 1.0 - 0.5 * exp(-max(d3, 0.0) / 0.025);
    if (ly < 3.5 || ly > 4.5) ao *= 1.0 - 0.45 * exp(-max(d4, 0.0) / 0.02);
    gLayer = ly; gAO = ao;
    return hc;
}

// ── surface detail at the hit point: ground patterns & petal striations (bump height, tone, glow, chip) ──
vec4 surfDetail(vec2 g, vec2 fq, float lyr, float pf) {
    float hgt = 0.0, tone = 0.0, em = 0.0, chip = 0.0;
    if (lyr > 1.5 && lyr < 3.5) {                       // fine radial striations on petals
        float r = length(fq), a = atan(fq.x, fq.y);
        float k = 70.0;
        float fd = 1.0 - smoothstep(0.25, 0.6, pf / gFS * k / (TAU * max(r, 0.02)));
        hgt = (0.5 + 0.5 * cos(a * k)) * fd * smoothstep(0.03, 0.12, r) * 0.35;
        return vec4(hgt, 0.0, 0.0, 0.0);
    }
    if (lyr > 0.5) return vec4(0.0);
    float t = gGround;
    float r = length(g), a = atan(g.x, g.y);
    float pg = pf * gGS;
    if (t < 0.5) {                                      // plain, with a soft sheen gradient
        tone = 0.25 * smoothstep(0.6, -0.4, g.y);
    } else if (t < 1.5) {                               // guilloché
        float wv = sin(r * 150.0 + 2.2 * sin(a * 12.0 + r * 20.0) * smoothstep(0.02, 0.12, r));
        hgt = (0.5 + 0.5 * wv) * (1.0 - smoothstep(0.25, 0.6, pg * 28.0));
        tone = 0.3 * hgt;
    } else if (t < 2.5) {                               // sunburst rays
        float wv = cos(a * 40.0);
        float fd = 1.0 - smoothstep(0.25, 0.6, pg * 40.0 / (TAU * max(r, 0.02)));
        hgt = smoothstep(-0.5, 0.5, wv) * fd * smoothstep(0.02, 0.07, r);
        tone = 0.55 * hgt;
    } else if (t < 3.5) {                               // concentric rings
        float wv = sin(r * TAU * 12.0);
        hgt = smoothstep(-0.35, 0.35, wv) * (1.0 - smoothstep(0.3, 0.7, pg * 12.0));
        tone = 0.6 * hgt;
    } else if (t < 4.5) {                               // checker (diamond)
        vec2 gg = rot(PI / 4.0) * g * 6.5;
        float s = sin(gg.x * PI) * sin(gg.y * PI);
        float fd = 1.0 - smoothstep(0.3, 0.7, pg * 6.5);
        hgt = smoothstep(-0.12, 0.12, s) * fd;
        tone = hgt;
    } else if (t < 5.5) {                               // chevron
        float yy = g.y * 7.0 + abs(fract(g.x * 3.5) - 0.5) * 3.0;
        hgt = smoothstep(-0.3, 0.3, sin(yy * TAU)) * (1.0 - smoothstep(0.3, 0.7, pg * 9.0));
        tone = 0.75 * hgt;
    } else if (t < 6.5) {                               // dotted (raised pearls)
        vec2 gg = g * 11.0;
        gg.x += 0.5 * mod(floor(gg.y), 2.0);
        vec2 cc = fract(gg) - 0.5;
        float rr = length(cc) / 0.24;
        hgt = sqrt(max(1.0 - rr * rr, 0.0)) * (1.0 - smoothstep(0.3, 0.7, pg * 11.0));
        tone = smoothstep(0.0, 0.2, hgt);
    } else if (t < 7.5) {                               // crackle glaze
        vec2 gg = g * 7.0;
        vec2 ip = floor(gg) + step(0.5, fract(gg)) - 1.0;
        float d1 = 9.0, d2 = 9.0, ch = 0.0;
        for (int j = 0; j < 2; j++)
        for (int i = 0; i < 2; i++) {
            vec2 o = vec2(float(i), float(j));
            vec2 pt = ip + o + 0.25 + 0.5 * vec2(h12(ip + o), h12(ip + o + 31.7)) - gg;
            float dd = dot(pt, pt);
            if (dd < d1) { d2 = d1; d1 = dd; ch = h12(ip + o + 7.3); }
            else d2 = min(d2, dd);
        }
        float e = sqrt(d2) - sqrt(d1);
        float crack = 1.0 - smoothstep(0.0, 0.05 + pg * 9.0, e);
        hgt = 1.0 - crack;
        tone = -0.75 * crack + 0.18 * (ch - 0.5);
    } else if (t < 8.5) {                               // terrazzo speckle
        vec2 gg = g * 9.0, ip = floor(gg);
        vec2 lp = fract(gg) - 0.5 - (vec2(h12(ip), h12(ip + 13.1)) - 0.5) * 0.4;
        float hh = h12(ip + 5.7);
        lp = rot(hh * 6.0) * lp;
        float d = max(abs(lp.x) * 1.15 + abs(lp.y) * 0.35, abs(lp.y) * 1.2) - (0.08 + 0.16 * hh);
        float c = smoothstep(0.02 + pg * 9.0, -0.02, d);
        hgt = 0.3 * c;
        chip = c * (1.0 + floor(h12(ip + 9.9) * 2.99));
        tone = 0.12 * (h12(gg * 3.1) - 0.5);
    } else if (t < 9.5) {                               // woven basket
        vec2 gg = g * 7.0, ip = floor(gg), fp = fract(gg);
        float par = mod(ip.x + ip.y, 2.0);
        float acr = par > 0.5 ? fp.x : fp.y, alo = par > 0.5 ? fp.y : fp.x;
        float strand = abs(sin(acr * 3.0 * PI));
        hgt = sqrt(strand) * (0.55 + 0.45 * sin(alo * PI)) * (1.0 - smoothstep(0.3, 0.7, pg * 21.0));
        tone = 0.35 * par + 0.25 * hgt;
    } else if (t < 10.5) {                              // gradient
        tone = smoothstep(-0.55, 0.55, (g.x + g.y) * 0.7071);
    } else {                                            // starry night sky
        vec2 gg = g * 14.0, ip = floor(gg);
        vec2 sp = fract(gg) - 0.5 - (vec2(h12(ip + 3.3), h12(ip + 8.1)) - 0.5) * 0.7;
        float hs = h12(ip + 1.9);
        float sz = 0.03 + 0.07 * hs * hs;
        float star = exp(-dot(sp, sp) / (sz * sz)) * step(0.35, hs);
        float spark = max(0.0, 1.0 - abs(sp.x) * 40.0) * max(0.0, 1.0 - abs(sp.y) * 4.0) + max(0.0, 1.0 - abs(sp.y) * 40.0) * max(0.0, 1.0 - abs(sp.x) * 4.0);
        star += spark * step(0.88, hs) * 0.6;
        em = star * (0.6 + 0.4 * sin(TIME * (1.0 + 3.0 * hs) + hs * 40.0));
        float band = (g.x * 0.6 - g.y * 0.8);
        tone = 0.5 * exp(-band * band * 14.0) * (0.6 + 0.4 * h12(floor(g * 60.0)));
    }
    return vec4(hgt, tone, em, chip);
}

// ── colours ──────────────────────────────────────────────────────────────
vec3 hsv(float h, float s, float v) {
    vec3 k = clamp(abs(fract(h + vec3(0.0, 2.0 / 3.0, 1.0 / 3.0)) * 6.0 - 3.0) - 1.0, 0.0, 1.0);
    return v * mix(vec3(1.0), k, s);
}
// scheme: ground, outer petals, inner petals, gem
void scheme(float s, out vec3 G, out vec3 A, out vec3 B, out vec3 C) {
    if (s < 0.5)      { G = vec3(0.022, 0.026, 0.045); A = vec3(0.93, 0.13, 0.07); B = vec3(0.96, 0.92, 0.84); C = vec3(1.0, 0.68, 0.14); }
    else if (s < 1.5) { G = vec3(0.92, 0.88, 0.79);    A = vec3(0.70, 0.03, 0.11); B = vec3(0.035, 0.03, 0.045); C = vec3(1.0, 0.62, 0.1); }
    else if (s < 2.5) { G = vec3(0.012, 0.15, 0.14);   A = vec3(1.0, 0.58, 0.05);  B = vec3(0.96, 0.92, 0.84); C = vec3(0.9, 0.05, 0.2); }
    else if (s < 3.5) { G = vec3(0.025, 0.07, 0.36);   A = vec3(1.0, 0.40, 0.56);  B = vec3(0.97, 0.94, 0.88); C = vec3(1.0, 0.72, 0.2); }
    else              { G = vec3(0.80, 0.09, 0.05);    A = vec3(0.97, 0.93, 0.85); B = vec3(0.03, 0.03, 0.04); C = vec3(1.0, 0.75, 0.2); }
}
// colour harmonies built from one hue: complementary, analogous, monochrome tints, inverse, triad
void harmony(float h0, float ht, float hj) {
    h0 = fract(h0 * 0.74 + 0.44);                      // jewel hues: teal, sapphire, amethyst, magenta, ruby, vermilion, amber
    float h1 = fract(h0 + 0.5);
    if (h1 > 0.16 && h1 < 0.44) h1 = h1 < 0.3 ? 0.12 : 0.47;    // complements never land on acid yellow-green
    if (ht < 0.5)      { gG = hsv(h1, 0.7, 0.11);         gA = hsv(h0, 0.8, 0.86);  gB = hsv(h1, 0.3, 0.96);        gC = hsv(h0 + 0.08, 0.85, 1.0); }
    else if (ht < 1.5) { gG = hsv(h0 - 0.07, 0.8, 0.13);  gA = hsv(h0, 0.8, 0.88);  gB = hsv(h0 + 0.07, 0.45, 0.97); gC = vec3(1.0, 0.72, 0.22); }
    else if (ht < 2.5) { gG = hsv(h0, 0.8, 0.17);         gA = hsv(h0, 0.68, 0.7);  gB = hsv(h0, 0.16, 0.97);       gC = hsv(h0, 0.5, 1.0); }
    else if (ht < 3.5) { gG = hsv(h0 + 0.04, 0.09, 0.92); gA = hsv(h0, 0.85, 0.5);  gB = hsv(h1, 0.6, 0.09);        gC = vec3(1.0, 0.7, 0.18); }
    else               { gG = hsv(h0 + 0.3, 0.5, 0.09);   gA = hsv(h0, 0.8, 0.86);  gB = hsv(h0 - 0.3, 0.45, 0.95);  gC = vec3(0.98, 0.95, 0.88); }
    gG2 = ht > 2.5 && ht < 3.5 ? gG * vec3(0.84, 0.82, 0.78) : mix(gG, gA, 0.35) * 1.1 + 0.015;
    gL = ht > 1.5 && ht < 2.5 ? hsv(h0, 0.55, 0.38) : hsv(0.36 + 0.08 * hj, 0.7, 0.32);
}

// flower design & colour scheme for one tile face (cell c, flip number n)
float faceScheme(vec2 c, float n) {
    float cs = float(colourSel);
    if (cs > 0.5) return cs - 1.0;
    float fs = float(flowerSel);
    if (fs > 6.5 || mod(n, 3.0) > 1.5) return floor(h12(c * 1.37 + n * 0.71) * 5.0);
    return mod(floor(h11(n * 3.1 + 1.7) * 5.0) + mod(c.x + c.y, 2.0) * 2.0, 5.0);
}
vec4 h4(vec2 p) { return vec4(h12(p), h12(p + 17.3), h12(p + 41.7), h12(p + 73.1)); }

// every face draws a fresh combination: flower, petal shape/count/twist, layout, ground, frame, depth, material, colours
void faceParams(vec2 c, float n, float s2) {
    float v = variety;
    vec2 Bk = floor(c * 0.5);
    float lsel = float(layoutSel);
    float hq = h12(Bk * 3.17 + vec2(n * 0.613, n * 0.271) + 4.1);
    bool quarter = (lsel > 2.5 && lsel < 3.5) || (lsel < 0.5 && hq < 0.17 * v);
    vec2 sd = quarter ? Bk * 2.0 + 97.5 : c;           // a quarter-flower block shares one design
    vec2 p = sd * 1.37 + vec2(n * 0.71, n * 1.13);
    vec4 hA = h4(p), hB = h4(p + 3.7), hC = h4(p + 9.1), hD = h4(p + 15.3), hE = h4(p + 21.9), hF = h4(p + 27.1);
    // flower
    float fsel = float(flowerSel);
    if (fsel > 0.5 && fsel < 6.5) gType = fsel - 1.0;
    else if (fsel > 7.5) gType = fsel - 2.0;
    else if (fsel > 6.5) gType = v > 0.3 ? mod(sd.x * 3.0 + sd.y * 8.0 + n * 7.0, 20.0) : mod(sd.x + 2.0 * sd.y + 5.0 * n, 6.0);
    else {
        bool sweep = mod(n, 3.0) < 1.5 && h11(n * 5.31 + 0.7) > v * 0.75;
        if (sweep) gType = floor(h11(n * 1.73 + 0.31) * (v > 0.3 ? 20.0 : 6.0));
        else gType = hB.x < v ? floor(hA.x * 20.0) : mod(sd.x + 2.0 * sd.y + 5.0 * n, 6.0);
    }
    // ground, frame, layout
    float gsel = float(groundSel), frs = float(frameSel);
    gGround = gsel > 0.5 ? gsel - 1.0 : (hA.y < v ? floor(hB.y * 12.0) : 1.0);
    gFrame = frs > 0.5 ? frs - 1.0 : (hA.z < v ? floor(hB.z * 10.0) : 2.0);
    if (quarter && frs < 0.5) gFrame = hB.z < 0.55 ? 0.0 : 1.0;
    gLayout = 0.0;
    if (quarter) gLayout = 2.0;
    else if (lsel > 0.5) gLayout = lsel - 1.0;
    else if (hA.w < v * 0.62) gLayout = hC.x < 0.42 ? 1.0 : hC.x < 0.68 ? 3.0 : 4.0;
    gClus = 3.0 + floor(hC.y * 2.99);
    gLay = vec2(hC.z < 0.5 ? -1.0 : 1.0, hC.w < 0.5 ? -1.0 : 1.0) * (0.13 + 0.09 * hD.x);
    vec2 o = Bk * 2.0 + 0.5 - c;                        // shared corner of the 2×2 block (world units)
    gQC = o / s2; gQS = 2.0 / s2;
    gGC = quarter ? gQC : vec2(0.0);
    gGS = quarter ? 0.5 * s2 : 1.0;
    // petal engine variation
    gNAdj = hD.y;
    gShapeOv = hD.z < 0.45 * v ? floor(hD.w * 5.99) : -1.0;
    gTwist = (hE.x - 0.5) * 0.7 * v * step(0.5, hE.y);
    gOffL = step(0.5, hE.z) * step(0.3, v);
    gOneL = step(hE.w, 0.14 * v);
    gGemV = mix(1.0, 0.75 + 0.6 * hF.x, v);
    // relief depth, layer heights, domed / recessed faces
    gDepthK = quarter ? 1.0 : mix(1.0, 0.6 + 0.9 * hF.y, v);
    gH2 = -0.5 + (hF.z - 0.5) * 0.3 * v;
    gH3 = gH2 + 0.23 + (hF.w - 0.5) * 0.12 * v;
    vec4 hG = h4(p + 33.3), hH = h4(p + 39.9);
    gDome = (!quarter && hG.x < 0.3 * v) ? 0.35 + 0.45 * hG.y : 0.0;
    gInset = (!quarter && hG.z < 0.28 * v && gFrame > 0.5) ? 1.0 : 0.0;
    // material set & metal tones
    float msel = float(materialSel);
    gMS = msel > 0.5 ? -1.0 : (hG.w < v ? floor(hH.x * 7.99) : 0.0);
    float mt = hH.y;
    gFM = vec3(0.95, 0.72, 0.32);                                  // gold
    if (v > 0.01 && mt > 0.45) gFM = mt < 0.65 ? vec3(0.88, 0.9, 0.94) : mt < 0.8 ? vec3(0.93, 0.6, 0.5) : mt < 0.92 ? vec3(0.72, 0.48, 0.26) : vec3(0.42, 0.42, 0.46);   // silver, rose gold, bronze, gunmetal
    gFM2 = mt < 0.5 ? vec3(0.88, 0.9, 0.94) : vec3(0.95, 0.72, 0.32);
    // colour harmony
    float cs = float(colourSel);
    if (cs < 0.5 && hH.z < v) harmony(hH.w, floor(h12(p + 51.3) * 4.99), h12(p + 57.1));
    else {
        scheme(faceScheme(sd, n), gG, gA, gB, gC);
        gG2 = mix(gG, gB, 0.3);
        gL = vec3(0.05, 0.3, 0.15);
    }
    if (gGround > 10.5) { gG = mix(gG, vec3(0.008, 0.012, 0.05), 0.85); gG2 = vec3(0.16, 0.12, 0.3); }
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
    // (rest flips scatter per 2×2 block so a quarter-flower motif always turns as one)
    float f = gClock - mix(dly * gWaveK, h12(floor(c * 0.5) * 1.31 + 0.7), gRest);
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
        vec3 emis = vec3(0.0);
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
            float s2 = 2.0 * bHs;
            faceParams(bC, n, s2);
            vec2 q0 = vec2(dot(lp, uR), dot(lp, vR)) / s2;
            vec3 dF = vec3(dot(ld, uR), dot(ld, vR), dot(ld, nF));
            float Hq = 0.16 * depthAmt * 0.5 * gDepthK;          // relief depth in tile units
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
            vec2 qh = q0, fqh = vec2(0.0);
            float fsh = 1.0;
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
                float hv = flowerH(qq);
                if (i < 16) {
                    bool below = dF.z * tt <= Hq * hv;
                    if (below) { t1 = tt; found = 1.0; } else t0 = tt;
                } else if (i == 16) { hc = hv; lyr = gLayer; ao = gAO; fqh = gFQ; fsh = gFS; }
                else if (i == 17) hx = hv;
                else if (i == 18) hy = hv;
                else if (i == 19) hs1 = hv;
                else hs2 = hv;
            }
            vec3 nf = normalize(vec3(-Hq * (hx - hc) / eps, -Hq * (hy - hc) / eps, 1.0));
            // fine surface detail (ground pattern / petal striations) as a bump, sampled 3× in one loop
            vec4 sd0 = vec4(0.0);
            float sdx = 0.0, sdy = 0.0;
            if (lyr < 3.5) {
                gFS = fsh;
                float ee = max(0.0015, gPF * 0.7);
                vec2 gh = (qh - gGC) * gGS;
                for (int k = 0; k < 3; k++) {
                    vec2 dq = k == 1 ? vec2(ee, 0.0) : k == 2 ? vec2(0.0, ee) : vec2(0.0);
                    vec4 sv = surfDetail(gh + dq * gGS, fqh + dq / fsh, lyr, gPF);
                    if (k == 0) sd0 = sv; else if (k == 1) sdx = sv.x; else sdy = sv.x;
                }
                float bA = (lyr < 0.5 ? 0.035 : 0.012) * Hq;
                nf = normalize(nf + vec3(-(sdx - sd0.x), -(sdy - sd0.x), 0.0) * bA / ee);
                ao *= 1.0 - 0.18 * (1.0 - sd0.x) * step(lyr, 0.5) * step(0.5, gGround) * step(gGround, 9.5);
            }
            steep = 1.0 - nf.z;
            // soft shadows from the higher layers
            if (lF.z > 0.0) {
                float e1 = Hq * hc + 0.03 * lSlope, e2 = Hq * hc + 0.085 * lSlope;
                sh *= 1.0 - 0.8 * clamp((Hq * hs1 - e1) / (0.25 * Hq), 0.0, 1.0);
                sh *= 1.0 - 0.6 * clamp((Hq * hs2 - e2) / (0.25 * Hq), 0.0, 1.0);
            } else sh = 0.0;
            nW = bM * (uR * nf.x + vR * nf.y + nF * nf.z);
            // per-layer colour
            vec3 gnd = sd0.y >= 0.0 ? mix(gG, gG2, clamp(sd0.y, 0.0, 1.0)) : gG * (1.0 + 0.8 * sd0.y);
            if (sd0.w > 0.5) gnd = sd0.w < 1.5 ? gA : sd0.w < 2.5 ? gB : gC;      // terrazzo chips
            alb = lyr < 0.5 ? gnd : lyr < 1.5 ? gFM : lyr < 2.5 ? gA : lyr < 3.5 ? gB : lyr < 4.5 ? gC : gL;
            emis = sd0.z * vec3(0.95, 0.93, 1.0) * step(lyr, 0.5);
            // per-layer material: 0 enamel, 1 metal, 2 glass, 3 ceramic glaze, 4 matte clay, 5 iridescent, 6 velvet
            if (msel > 0.5) {
                mat = msel - 1.0;
                if (msel > 0.5 && msel < 1.5 && lyr > 0.5 && lyr < 1.5) alb = mix(gG, gB, 0.5) * 0.4 + vec3(0.5, 0.4, 0.25);
            } else {
                float ms = gMS;
                vec4 mL = vec4(0.0, 1.0, 0.0, 0.0);                 // ground, frame, outer, inner
                if (ms < 0.5) { }                                    // jewel
                else if (ms < 1.5) { mL = vec4(3.0, 1.0, 3.0, 0.0); } // glazed tile, metal frame
                else if (ms < 2.5) { mL = vec4(6.0, 1.0, 1.0, 0.0); } // gilded petals on velvet
                else if (ms < 3.5) { mL = vec4(0.0, 1.0, 2.0, 2.0); } // glass petals
                else if (ms < 4.5) { mL = vec4(3.0, 3.0, 3.0, 3.0); } // all ceramic glaze
                else if (ms < 5.5) { mL = vec4(4.0, 1.0, 4.0, 4.0); } // matte clay, metal frame
                else if (ms < 6.5) { mL = vec4(0.0, 1.0, 5.0, 5.0); } // iridescent petals
                else { mL = vec4(6.0, 1.0, 0.0, 1.0); }              // velvet ground, enamel + metal heart
                mat = lyr < 0.5 ? mL.x : lyr < 1.5 ? mL.y : lyr < 2.5 ? mL.z : lyr < 3.5 ? mL.w : lyr < 4.5 ? 2.0 : (mL.z > 0.5 && mL.z < 1.5 ? 0.0 : mL.z);
                if (mat > 0.5 && mat < 1.5 && lyr > 1.5 && lyr < 3.5) alb = lyr < 2.5 ? gFM2 : gFM;
                if (ms > 3.5 && ms < 4.5 && lyr > 0.5 && lyr < 1.5) alb = mix(gB, vec3(0.9, 0.86, 0.78), 0.5);
            }
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
        if (mat < 0.5 || (mat > 2.5 && mat < 3.5)) {   // glossy enamel / ceramic glaze with a clear coat
            float gz = step(2.5, mat);
            vec3 ab = alb * (1.0 - 0.35 * gz * smoothstep(0.1, 0.5, steep));    // glaze pools darker in the recesses
            float F = 0.04 + 0.96 * fres;
            cl = ab * (dif * keyC + hemi * vec3(0.16, 0.17, 0.2) + rimD * rimC * 0.12) * ao;
            cl += (en * F * (0.9 + 0.5 * gz) + keyC * spK * (1.6 + 1.2 * gz) + rimC * spR * 0.5 * (0.3 + fres)) * shn * mix(0.5, 1.0, ao);
        } else if (mat < 1.5) {              // polished metal
            vec3 F = alb + (1.0 - alb) * fres;
            cl = F * (en * 1.1 + keyC * spK * 2.2 + rimC * spR * 0.8) * shn * ao;
            cl += alb * (dif * 0.28 + 0.06) * ao;
        } else if (mat < 2.5) {              // glass / gem: tinted body glow + strong reflections
            float F = 0.06 + 0.94 * fres;
            float body = 0.35 + 0.65 * pow(ndv, 1.5);
            cl = alb * (0.22 * dif * keyC + 0.5 * body * (0.4 + 0.6 * sh)) * ao;
            cl += alb * alb * 0.6 * pow(clamp(dot(-nW, gKey) * 0.5 + 0.5, 0.0, 1.0), 2.0);
            cl += (en * F * 1.3 + keyC * spK * 2.5 + rimC * spR * 0.9) * shn;
        } else if (mat < 4.5) {              // matte clay: soft wrapped diffuse, a faint broad sheen
            float wr = clamp((dot(nW, gKey) + 0.3) / 1.3, 0.0, 1.0) * sh;
            cl = alb * (wr * keyC * 1.05 + hemi * vec3(0.2, 0.2, 0.22) + rimD * rimC * 0.08) * ao;
            cl += keyC * pow(clamp(dot(rf, gKey), 0.0, 1.0), 6.0) * 0.06 * sh * shn;
        } else if (mat < 5.5) {              // iridescent thin film over a dark base
            vec3 film = 0.5 + 0.5 * cos(TAU * (vec3(0.0, 0.33, 0.67) + ndv * 1.3 + lum(alb) * 0.8 + 0.05 * TIME));
            float F = 0.25 + 0.75 * fres;
            cl = alb * 0.25 * (dif * keyC + hemi * 0.2) * ao;
            cl += film * (en * F * 1.4 + keyC * spK * 2.2 + 0.25 * dif) * shn * ao;
        } else {                             // velvet / flocked: no hard highlight, glowing grazing sheen
            float sheen = pow(1.0 - ndv, 2.2);
            cl = alb * (dif * keyC * 0.75 + hemi * vec3(0.12, 0.12, 0.14)) * ao;
            cl += mix(alb, vec3(1.0), 0.25) * sheen * (0.35 + 0.65 * dif) * 0.9 * ao;
        }
        cl += emis * (1.0 + 0.6 * highP() * ar);
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
