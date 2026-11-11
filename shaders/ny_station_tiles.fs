/*{
  "DESCRIPTION": "Station Tiles — Shades of New York 003. The tiled walls of the subway, built tile by tile in glossy, handmade HD ceramic. Four walls flow into each other: White Subway Squares (pillowed glossy squares with crisp tube-light highlights on every top edge and thin dark grout), Black Glass Mosaic (small hand-glazed black tiles, wavy and liquid, with cream grout and silvery reflections), Greek Key Band (the classic station frieze of brick-coursed tesserae: cream, yellow and tan rows, a maroon stripe, a green meander with white-framed maroon boxes), and Ornament Panels (a maroon field of upright tesserae around a white frame and a central motif whose white contour band follows the shape tile by tile, filled with blue, yellow and tan). The motif keeps morphing: diamond, circle, star, octagon, flower and cross, with tesserae changing colour one by one as the outline sweeps through them. The walls don't fade into each other, they rebuild: every tile of the next wall flips over on its own like a split-flap, so big subway squares break into small glass tiles, which then turn into the mosaic and the ornaments, as a diagonal wipe, a ripple from the centre, a random sparkle or a row cascade. The grid between the tiles is alive: light runs along the grout lines and the whole grid flashes on the beat, tiles lift off the wall in waves and cast shadows into the grout, and tiles tilt so glints race across the glaze. Every tile has its own colour variance, glaze waviness, pillowed edge, chips and craquelure, lit by a reflection of the station's fluorescent tubes plus a passing train light. Camera Mosaic turns any wall into a mosaic portrait from the camera image. Your mouse, or your hand in ShaderClaw, lifts and tilts the tiles under it like a magnet; press or pinch to spin them over. Audio: bass lifts the tiles, beats flash the grout grid, mids speed up the tile flips and the motif morph, highs make the glaze glint. Show dynamics: at Rest the wall is still with soft glints; the Build makes the grout light run faster and the tiles rattle on their beds; the Drop spins every tile in a shockwave ring from the centre with a flash; the Peak lifts and glints tiles with the music.",
  "CREDIT": "Easel / ShaderClaw house shader — Shades of New York series, 2026-11-11 — original.",
  "CATEGORIES": ["Generator", "Shades of New York", "Texture", "Mosaic", "Interactive", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "sequence",    "LABEL": "Wall", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Story (all four)", "White Subway Squares", "Black Glass Mosaic", "Greek Key Band", "Ornament Panels"], "GROUP": "Wall" },
    { "NAME": "chapterLen",  "LABEL": "Wall Length (s)",  "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 12.0, "GROUP": "Wall" },
    { "NAME": "flipStyle",   "LABEL": "Tile Flip", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto", "Diagonal Wipe", "Ripple From Centre", "Random Sparkle", "Row Cascade"], "GROUP": "Wall" },
    { "NAME": "zoom",        "LABEL": "Tiles On Screen",  "TYPE": "float", "AUDIO": false, "MIN": 2.0, "MAX": 14.0, "DEFAULT": 5.5, "GROUP": "Wall" },
    { "NAME": "drift",       "LABEL": "Drift",            "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Wall" },
    { "NAME": "shapeMode",   "LABEL": "Ornament", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6], "LABELS": ["Morph All", "Diamond", "Circle", "Star", "Octagon", "Flower", "Cross"], "GROUP": "Ornament" },
    { "NAME": "morphSpeed",  "LABEL": "Morph Speed",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Ornament" },
    { "NAME": "fillColor",   "LABEL": "Motif Fill",       "TYPE": "color", "DEFAULT": [0.14, 0.36, 0.52, 1.0], "GROUP": "Ornament" },
    { "NAME": "gridGlow",    "LABEL": "Grout Light",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Grid" },
    { "NAME": "glowColor",   "LABEL": "Grout Light Colour", "TYPE": "color", "DEFAULT": [1.0, 0.82, 0.45, 1.0], "GROUP": "Grid" },
    { "NAME": "liftAmt",     "LABEL": "Tile Lift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.6, "GROUP": "Grid" },
    { "NAME": "tiltAmt",     "LABEL": "Tile Tilt / Glints", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.5, "GROUP": "Grid" },
    { "NAME": "groutW",      "LABEL": "Grout Width",      "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Grid" },
    { "NAME": "gloss",       "LABEL": "Glaze Gloss",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Glaze" },
    { "NAME": "waviness",    "LABEL": "Handmade Waviness","TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.7, "GROUP": "Glaze" },
    { "NAME": "pillow",      "LABEL": "Pillowed Edges",   "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.8, "GROUP": "Glaze" },
    { "NAME": "wear",        "LABEL": "Chips & Grime",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Glaze" },
    { "NAME": "tubes",       "LABEL": "Fluorescent Tubes","TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.9, "GROUP": "Light" },
    { "NAME": "trainEvery",  "LABEL": "Train Light Every (s, 0 = off)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 90.0, "DEFAULT": 18.0, "GROUP": "Light" },
    { "NAME": "trainColor",  "LABEL": "Train Light",      "TYPE": "color", "DEFAULT": [0.75, 0.85, 1.0, 1.0], "GROUP": "Light" },
    { "NAME": "interact",    "LABEL": "Hand", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1], "LABELS": ["Off", "Magnet (press / pinch spins)"], "GROUP": "Hand" },
    { "NAME": "handSize",    "LABEL": "Hand Size",        "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Hand" },
    { "NAME": "camTex",      "LABEL": "Image / Camera",   "TYPE": "image", "GROUP": "Camera" },
    { "NAME": "camMix",      "LABEL": "Camera Mosaic",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Camera" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 4, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White", "NYC Reference"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Behind Flipping Tiles", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.05, 0.045, 0.04, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG (tiles only)", "TYPE": "bool", "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Station Tiles — Shades of New York 003
//   One pass. A "layout" maps a wall point to the tile it lies in: the tile's
//   centre, half size, corner radius, grout width, local coordinates and
//   base colour (from the design: frieze, panel, motif). Transitions flip the
//   NEXT layout's tiles one by one; a flipping tile shows the old wall until
//   it turns edge-on, then the new one. evalLayout() has ONE call site, run
//   at most twice per pixel (find the flipping tile, then look through it).
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

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
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// station palette, from the four reference photos (a touch richer)
const vec3 C_WHITE  = vec3(0.93, 0.92, 0.88);
const vec3 C_CREAM  = vec3(0.95, 0.89, 0.74);
const vec3 C_YELLOW = vec3(0.96, 0.84, 0.36);
const vec3 C_TAN    = vec3(0.85, 0.63, 0.36);
const vec3 C_MAROON = vec3(0.44, 0.12, 0.09);
const vec3 C_GREEN  = vec3(0.07, 0.26, 0.21);
const vec3 C_BLACK  = vec3(0.022, 0.022, 0.027);

// ── layout output (one tile) ────────────────────────────────────────────
vec3  tCol;      // base colour
float tMat;      // 0 glossy subway glaze, 1 black hand glaze, 2 matte mosaic
vec2  tC;        // tile centre (wall units)
vec2  tB;        // tile half size including half the grout
vec2  tLc;       // pixel relative to the tile centre, in the tile's frame
float tR;        // corner radius
float tGw;       // grout half width
float tId;       // per-tile hash
float tEdge;     // extra grout: distance to a design boundary that cuts tiles (large = none)
vec3  tGrout;    // grout colour

void squareCell(vec2 p, float s, float jit, float seed) {
    vec2 id = floor(p / s);
    vec2 c = (id + 0.5) * s;
    vec2 j = (h22(id + seed) - 0.5) * jit * s;
    float a = (h12(id + seed + 9.0) - 0.5) * jit * 0.6;
    tC = c + j;
    tLc = rot(a) * (p - tC);
    tB = vec2(0.5 * s) * (1.0 - jit * 0.35 * h12(id + seed + 4.0));
    tId = h12(id * 1.37 + seed);
}
// brick-coursed tesserae; vertical = 1 lays them upright (rows become columns)
void brickCell(vec2 p, float th, float tw, float vertical, float seed, float aligned) {
    vec2 q = vertical > 0.5 ? p.yx : p;
    float row = floor(q.y / th);
    float off = aligned > 0.5 ? 0.0 : h11(row * 7.13 + seed) * tw;
    float col = floor((q.x - off) / tw);
    vec2 id = vec2(col, row);
    float hh = h12(id + seed * 3.1);
    vec2 c = vec2((col + 0.5) * tw + off, (row + 0.5) * th);
    float wsh = aligned > 0.5 ? 1.0 : 0.82 + 0.18 * h12(id + seed + 2.0);    // tesserae differ in width
    c.x += (hh - 0.5) * tw * (1.0 - wsh) * 0.9;
    vec2 lc = rot((h12(id + seed + 7.0) - 0.5) * 0.06) * (q - c);
    vec2 b = vec2(0.5 * tw * wsh, 0.5 * th * (0.94 + 0.06 * h12(id + seed + 5.0)));
    if (vertical > 0.5) { c = c.yx; lc = lc.yx; b = b.yx; }
    tC = c; tLc = lc; tB = b; tId = hh;
}

// ── ornament motif: signed distance, morphing through six shapes ────────
float gShapeA, gShapeB, gShapeF;
float shapeSD(vec2 P, float k) {
    float r = length(P);
    float a = atan(P.y, P.x);
    vec2 A = abs(P);
    if (k < 0.5) return (A.x / 0.62 + A.y / 0.95 - 1.0) * 0.5;                            // diamond (tall)
    if (k < 1.5) return r - 0.62;                                                         // circle
    if (k < 2.5) return r - 0.66 * (0.72 + 0.28 * cos(5.0 * a + PI * 0.5)) - 0.04;        // star
    if (k < 3.5) return max(max(A.x, A.y), (A.x + A.y) * 0.7071) - 0.56;                  // octagon
    if (k < 4.5) return r - 0.58 * (0.78 + 0.22 * abs(cos(3.0 * a))) - 0.05;              // flower
    vec2 bx = A - vec2(0.62, 0.2), by = A - vec2(0.2, 0.62);                              // cross
    return min(length(max(bx, 0.0)) + min(max(bx.x, bx.y), 0.0), length(max(by, 0.0)) + min(max(by.x, by.y), 0.0));
}
float motifSD(vec2 P) { return mix(shapeSD(P, gShapeA), shapeSD(P, gShapeB), gShapeF); }

// ── Greek key: design grid, 14 columns × 8 rows per period ──────────────
// 0 = background (yellow/tan), 1 = green stroke, 2 = white frame, 3 = maroon
float keyCell(float ix, float iy) {
    ix = mod(ix, 14.0);
    if (iy < 0.5 || iy > 6.5) return 1.0;                             // top + bottom borders
    if (ix < 0.5 || (ix > 7.5 && ix < 8.5)) return 1.0;               // the uprights
    if (ix < 7.5) {                                                   // the hook
        if (iy > 4.5 && iy < 5.5 && ix < 6.5) return 1.0;
        if (ix > 5.5 && ix < 6.5 && iy > 1.5 && iy < 5.5) return 1.0;
        if (iy > 1.5 && iy < 2.5 && ix > 2.5 && ix < 6.5) return 1.0;
        if (ix > 2.5 && ix < 3.5 && iy > 1.5 && iy < 3.5) return 1.0;
        return 0.0;
    }
    if (ix > 9.5 && ix < 12.5 && iy > 1.5 && iy < 5.5) return 3.0;    // maroon heart
    if (ix > 8.5) return 2.0;                                         // white frame
    return 0.0;
}

vec3 mixYT(float h) { return h < 0.5 ? mix(C_YELLOW, C_CREAM, 0.25 * h) : mix(C_TAN, C_YELLOW, 0.4 * (h - 0.5)); }

// ── layouts ─────────────────────────────────────────────────────────────
void evalLayout(float L, vec2 p) {
    tEdge = 9.0;
    tMat = 2.0; tR = 0.02; tGw = 0.011 * groutW;
    tGrout = vec3(0.47, 0.43, 0.39);
    if (L < 0.5) {                                                    // White Subway Squares
        squareCell(p, 1.0, 0.012, 1.0);
        float v = tId;
        tCol = mix(vec3(0.97, 0.965, 0.95), vec3(0.94, 0.95, 0.96), v);
        tMat = 0.0; tR = 0.045; tGw = 0.012 * groutW;
        tGrout = vec3(0.4, 0.39, 0.37);
    } else if (L < 1.5) {                                             // Black Glass Mosaic
        squareCell(p, 0.5, 0.06, 3.0);
        tCol = C_BLACK * (0.8 + 0.5 * tId) + vec3(0.0, 0.004, 0.012) * step(0.8, tId);
        tMat = 1.0; tR = 0.03; tGw = 0.018 * groutW;
        tGrout = vec3(0.86, 0.8, 0.68);
    } else if (L < 2.5) {                                             // Greek Key Band
        float th = 0.15;
        float rowF = floor(p.y / th);
        float band = 10.0;                                            // band rows: -10 .. 9
        if (rowF < -band || rowF > band - 1.0) {                      // the white wall around it
            squareCell(p + vec2(0.0, 0.5), 1.0, 0.012, 1.0);
            tC -= vec2(0.0, 0.5);
            tCol = mix(C_WHITE, vec3(0.9, 0.92, 0.93), tId);
            tMat = 0.0; tR = 0.07; tGw = 0.012 * groutW;
            tGrout = vec3(0.32, 0.31, 0.29);
            tEdge = abs(p.y - (rowF < 0.0 ? -band : band) * th);
        } else {
            float r = rowF + band;                                    // 0 .. 19 (bottom → top)
            bool key = r > 5.5 && r < 13.5;
            brickCell(p, th, key ? 0.2 : 0.21, 0.0, 11.0 + (key ? 0.0 : r), key ? 1.0 : 0.0);
            float h = tId;
            vec3 c;
            if (r < 0.5 || r > 18.5) c = C_WHITE;
            else if (r > 2.5 && r < 3.5 || r > 15.5 && r < 16.5) c = C_MAROON * (0.9 + 0.2 * h);
            else if (!key) c = mixYT(h);
            else {
                float k = keyCell(floor(tC.x / 0.2 + 3.0), r - 6.0);
                c = k < 0.5 ? mixYT(h) : k < 1.5 ? C_GREEN * (0.85 + 0.3 * h) : k < 2.5 ? C_WHITE : C_MAROON * (0.9 + 0.2 * h);
            }
            tCol = c;
            tEdge = min(abs(p.y + band * th), abs(p.y - band * th));
        }
    } else {                                                          // Ornament Panels
        float Pw = 2.5, Ph = 3.3;
        vec2 pid = vec2(floor(p.x / Pw + 0.5), 0.0);
        vec2 P = p - pid * Pw;
        P.y = mod(P.y + Ph * 0.5, Ph + 0.6) - Ph * 0.5;               // panels stack too
        vec2 A = abs(P);
        float fx = 0.92, fy = 1.38, fw = 0.12;                        // inner frame (half size) and band width
        float outer = max(A.x - fx, A.y - fy);
        if (outer > 0.0) {                                            // maroon field of upright tesserae
            brickCell(p, 0.115, 0.27, 1.0, 21.0, 0.0);
            tCol = C_MAROON * (0.82 + 0.3 * tId) + vec3(0.04, 0.0, 0.0) * step(0.7, tId);
            tEdge = abs(outer);
        } else if (outer > -fw) {                                     // white frame: square tesserae along it
            vec2 Q = P;
            bool vert = A.x - fx > A.y - fy;
            float sgn = vert ? sign(P.x) : sign(P.y);
            vec2 along = vert ? vec2(P.y, (A.x - fx + fw * 0.5)) : vec2(P.x, (A.y - fy + fw * 0.5));
            squareCell(vec2(along.x, along.y) + 0.5 * fw, fw, 0.05, 31.0 + sgn);
            tC = vert ? vec2(sgn * (fx - fw * 0.5), tC.x - 0.5 * fw) + pid * Pw : vec2(tC.x - 0.5 * fw, sgn * (fy - fw * 0.5)) + pid * Pw;
            tCol = C_WHITE * (0.95 + 0.05 * tId);
            tEdge = min(abs(outer), abs(outer + fw));
        } else {
            vec2 M = P / 1.3;                                         // motif space
            float s = motifSD(M) * 1.3;
            float bw = 0.12;
            if (abs(s) < bw * 0.5) {                                  // white contour band, tiles follow the outline
                float ang = atan(P.y, P.x);
                float n = 34.0;
                float u = (ang / TAU + 0.5) * n;
                vec2 cid = vec2(floor(u), 0.0);
                float uc = (cid.x + 0.5) / n * TAU - PI;
                float rr = length(P);
                vec2 dirc = vec2(cos(uc), sin(uc));
                tC = pid * Pw + dirc * rr;                            // approx centre on the band
                vec2 tang = vec2(-dirc.y, dirc.x);
                float arcW = TAU * max(rr, 0.2) / n;
                tLc = vec2((fract(u) - 0.5) * arcW, s);
                tB = vec2(0.5 * arcW, 0.5 * bw);
                tId = h12(cid + pid * 7.0 + 41.0);
                tCol = C_WHITE * (0.94 + 0.06 * tId);
                tEdge = 9.0;
            } else if (s < 0.0) {                                     // the motif fill, brick coursed
                brickCell(p, 0.13, 0.19, 0.0, 51.0, 0.0);
                vec3 fc = fillColor.rgb;
                tCol = fc * (0.8 + 0.35 * tId) + vec3(0.0, 0.03, 0.03) * step(0.6, h12(vec2(tId, 3.0)));
                tEdge = abs(s + bw * 0.5);
            } else {                                                  // yellow / tan corners
                brickCell(p, 0.13, 0.2, 0.0, 61.0, 0.0);
                tCol = mixYT(tId);
                tEdge = min(abs(s - bw * 0.5), abs(outer + fw));
            }
        }
    }
}

// ── grime / chips on a tile ─────────────────────────────────────────────
float chipMask(vec2 lc, vec2 b, float id) {
    float hit = step(h11(id * 91.3), wear * 0.55);
    vec2 cp = (h22(vec2(id * 17.0, 3.0)) * 2.0 - 1.0) * b * 0.9;
    float rr = (0.12 + 0.25 * h11(id * 7.7)) * min(b.x, b.y);
    float n = vnoise(lc * 60.0 + id * 9.0);
    return hit * smoothstep(rr, rr * 0.4, length(lc - cp) * (0.7 + 0.6 * n));
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy, fc = gl_FragCoord.xy, uv = fc / R;
    vec2 p0 = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    float sc = zoom;
    vec2 cam = vec2(TIME * 0.035 * drift + 0.02 * sin(TIME * 0.17), 0.025 * sin(TIME * 0.11) * drift);
    // build: the wall rattles
    p0 += (h21(floor(TIME * 30.0)) - 0.5) * 0.004 * smoothstep(0.55, 1.0, gBuild);
    vec2 p = p0 * sc + cam;
    float pxW = sc / R.y;                                             // wall units per pixel

    // ── the story: which wall, which next, how far into the flip ──
    float sq = float(sequence);
    float seq = TIME / chapterLen + audioMidTime * 0.02 * ar + 0.3 * gBuild * gBuild;
    float chap = floor(seq);
    float u = fract(seq);
    float LA, LB;
    if (sq < 0.5) { LA = mod(chap, 4.0); LB = mod(chap + 1.0, 4.0); }
    else { LA = sq - 1.0; LB = LA; }
    float tp = LA == LB ? 0.0 : clamp((u - 0.6) / 0.36, 0.0, 1.0);
    float fstyle = float(flipStyle);
    if (fstyle < 0.5) fstyle = 1.0 + mod(chap, 4.0);

    // ornament morph clock
    float sm = float(shapeMode);
    float mclk = TIME * 0.12 * morphSpeed + audioMidTime * 0.05 * ar * morphSpeed;
    if (sm < 0.5) { gShapeA = mod(floor(mclk), 6.0); gShapeB = mod(floor(mclk) + 1.0, 6.0); gShapeF = smoothstep(0.55, 1.0, fract(mclk)); }
    else { gShapeA = sm - 1.0; gShapeB = gShapeA; gShapeF = 0.0; }

    // hand in wall space
    vec2 mW = ((mousePos - 0.5) * vec2(R.x / R.y, 1.0)) * sc + cam;
    float handOn = step(0.5, float(interact));

    // ── find the tile, flip it, look through it (ONE evalLayout call site) ──
    float Lsel = tp > 0.0 ? LB : LA;
    vec2 pe = p;
    float sx = 1.0, back = 0.0, flipShade = 1.0;
    vec2 cB = vec2(0.0), bB = vec2(0.5);
    for (int k = 0; k < 2; k++) {
        evalLayout(Lsel, pe);
        if (k == 1) break;
        cB = tC; bB = tB;
        float idh = tId;
        // flip progress of this tile toward the next wall
        vec2 cr = (cB - cam) / sc;
        float thr;
        if (fstyle < 1.5)      thr = clamp(0.5 + dot(cr, normalize(vec2(1.0, -0.6))) * 0.55, 0.0, 1.0) * 0.8 + idh * 0.2;
        else if (fstyle < 2.5) thr = clamp(length(cr) * 0.95, 0.0, 1.0) * 0.85 + idh * 0.15;
        else if (fstyle < 3.5) thr = idh;
        else                   thr = clamp(0.5 - cr.y * 0.95, 0.0, 1.0) * 0.85 + fract(cr.x * 3.1) * 0.03 + idh * 0.12;
        float f = clamp((tp * 1.3 - thr) / 0.3, 0.0, 1.0);
        f = f * f * (3.0 - 2.0 * f);
        float ang = f * PI;
        // drop: a shockwave ring spins every tile once around
        float dR = length(cr);
        float ring = clamp(1.0 - abs(dR - (1.0 - gDrop) * 1.6) / 0.35, 0.0, 1.0) * step(0.02, gDrop);
        ang += ring * ring * TAU;
        // hand: press / pinch spins the tiles under it
        float dh = length(cB - mW) / (sc * 0.12 * handSize);
        ang += handOn * step(0.5, mouseDown) * TAU * exp(-dh * dh) * fract(TIME * 0.9);
        sx = cos(ang);
        float showB = step(0.5, f);
        flipShade = 0.55 + 0.45 * abs(sx);
        // first pass already shows the right wall, unturned: done
        bool needSecond = abs(sx) < 0.999 || (tp > 0.0 && showB < 0.5);
        if (!needSecond) break;
        // look through the turning tile
        vec2 lcl = p - cB;
        float lx = lcl.x / max(abs(sx), 1e-3);
        if (abs(lx) > bB.x) { back = 1.0; break; }
        pe = cB + vec2(sign(sx) * lx, lcl.y);
        Lsel = (tp > 0.0 && showB > 0.5) ? LB : LA;
    }

    // ── shade the tile ──
    vec2 lc = tLc;
    vec2 b = tB - tGw;
    float aa = pxW * 1.2;
    vec2 qd = abs(lc) - (b - tR);
    float d = length(max(qd, 0.0)) + min(max(qd.x, qd.y), 0.0) - tR;   // rounded box SDF
    float edgeCut = tEdge - tGw;                                     // design boundaries cut tiles too
    float inside = smoothstep(aa, -aa, d) * smoothstep(-aa, aa, edgeCut);
    vec2 g = max(qd.x, qd.y) > 0.0 ? normalize(max(qd, 0.0)) * sign(lc) : (qd.x > qd.y ? vec2(sign(lc.x), 0.0) : vec2(0.0, sign(lc.y)));

    // per-tile motion: lift waves, tilt glints, magnet hand
    float id = tId;
    vec2 cw = (tC - cam) / sc;
    float T = TIME * (0.6 + 0.4 * gLift) + audioBassTime * 0.3 * ar;
    float wave = pow(0.5 + 0.5 * sin(dot(cw, vec2(2.3, 1.1)) * 3.0 - T * 2.2), 4.0);
    float lift = liftAmt * (0.25 * wave + 0.6 * bassP() * ar * (0.3 + 0.7 * wave) + 0.5 * audioBeatPulse * ar * step(0.82, h11(id * 31.0 + floor(TIME * 2.0))));
    vec2 tilt = tiltAmt * 0.18 * vec2(sin(T * 1.7 + id * 40.0 + cw.x * 4.0), cos(T * 1.3 + id * 23.0 + cw.y * 4.0));
    tilt *= 0.35 + 0.65 * gLift + 1.5 * gBuild * gBuild;
    float dHand = length(tC - mW) / (sc * 0.16 * handSize);
    float mag = handOn * exp(-dHand * dHand);
    lift += mag * 0.9;
    tilt += mag * normalize(mW - tC + 1e-4) * 0.35;

    // surface normal: pillowed edge + handmade glaze waviness + tilt
    float tsz = min(b.x, b.y);
    float ew = tsz * (tMat < 0.5 ? 0.055 : tMat < 1.5 ? 0.22 : 0.12);
    float e = max(-d, 0.0);
    float slope = pillow * exp(-e / ew) * 0.9 * (tMat > 1.5 ? 0.6 : 1.0);
    vec2 nxy = g * slope + tilt;
    float wv = waviness * (tMat < 0.5 ? 0.04 : tMat < 1.5 ? 0.16 : 0.06);
    vec2 wl = lc / max(tsz, 0.05);
    nxy += wv * vec2(sin(wl.x * 5.1 + id * 30.0) * cos(wl.y * 3.7 + id * 11.0) + 0.5 * sin(wl.y * 9.0 + id * 7.0),
                     cos(wl.y * 4.3 + id * 19.0) * sin(wl.x * 3.1 + id * 5.0) + 0.5 * cos(wl.x * 8.0 + id * 13.0));
    nxy += (tMat > 1.5 ? 0.05 : 0.0) * (vec2(vnoise(lc * 120.0), vnoise(lc * 120.0 + 7.0)) - 0.5);   // matte tesserae: bisque texture
    vec3 N = normalize(vec3(nxy, 1.0));

    // base colour, camera mosaic, chips, grime
    vec3 base = tCol;
    if (camMix > 0.001) {
        vec2 cu = ((tC - cam) / sc) * vec2(R.y / R.x, 1.0) + 0.5;
        vec3 cc = texture2D(camTex, clamp(cu, 0.0, 1.0)).rgb;
        base = mix(base, cc * (0.92 + 0.12 * id), camMix);
    }
    float chip = chipMask(lc, b, id) * (tMat > 1.5 ? 1.0 : tMat > 0.5 ? 0.4 : 0.0);
    base = mix(base, vec3(0.78, 0.68, 0.52), chip);                 // exposed bisque
    float grime = wear * 0.12 * smoothstep(0.4, 1.0, vnoise(p * 1.7)) + wear * 0.25 * exp(-e / (tsz * 0.1)) * step(0.5, tMat) * step(tMat, 1.5) * 0.0;
    base *= 1.0 - grime;

    // ── light: soft top light, fluorescent tubes in reflection, train light, hand glints ──
    vec3 V = normalize(vec3(-p0 * 0.35, 1.0));
    vec3 Lk = normalize(vec3(-0.25, 0.65, 0.72));
    float dif = clamp(dot(N, Lk) * 0.55 + 0.55, 0.0, 1.2);
    float topLight = 0.9 + 0.18 * uv.y;
    vec3 Rf = reflect(-V, N);
    float ry = Rf.y / max(Rf.z, 0.25);
    float rx = Rf.x / max(Rf.z, 0.25);
    float tubeA = smoothstep(0.07, 0.0, abs(ry - 0.42 - 0.04 * sin(cam.x * 0.5))) * smoothstep(1.4, 0.3, abs(rx));
    float tubeB = smoothstep(0.05, 0.0, abs(ry - 0.95)) * smoothstep(1.6, 0.2, abs(rx + 0.3)) * 0.7;
    float ceilG = smoothstep(-0.2, 1.2, ry) * 0.25;
    float fres = 0.05 + 0.95 * pow(1.0 - max(dot(N, V), 0.0), 5.0);
    float glossK = gloss * (tMat < 0.5 ? 1.0 : tMat < 1.5 ? 1.15 : 0.22) * (1.0 - 0.8 * chip);
    float sharp = tMat > 1.5 ? 0.3 : 1.0;
    vec3 env = vec3(1.0, 0.98, 0.94) * ((tubeA + tubeB) * tubes * mix(0.35, 1.0, sharp) + ceilG);
    // train light: a bright streak sweeping along the wall
    float passT = trainEvery > 0.5 ? mod(TIME, trainEvery) / 4.0 : 9.0;
    float trainOn = step(passT, 1.0) * smoothstep(0.0, 0.15, passT) * smoothstep(1.0, 0.8, passT);
    float tx = mix(2.2, -2.2, passT) * (R.x / R.y);
    float trainSp = smoothstep(0.35, 0.0, abs(rx - tx)) * smoothstep(-0.6, 0.6, ry) * trainOn;
    env += trainColor.rgb * trainSp * 1.6;
    // highs: glints jump between tiles
    float glint = highP() * ar * step(0.9, h11(id * 51.0 + floor(TIME * 8.0))) * pow(max(dot(N, normalize(Lk + V)), 0.0), 60.0);
    vec3 lit = base * dif * topLight * (0.9 + 0.35 * lift) * (tMat < 0.5 ? 1.16 : 1.0);
    lit = mix(lit, lit * 0.25 + env, clamp(glossK * (0.06 + 0.9 * fres) * (tMat > 0.5 && tMat < 1.5 ? 2.2 : 1.0), 0.0, 1.0) * 0.85);
    lit += env * glossK * 0.18 * sharp + vec3(1.0) * glint * 1.5 * gloss;
    // hand: a soft lamp under the palm
    float hl = mag * 0.25;
    lit += vec3(1.0, 0.96, 0.9) * hl * (base * 0.6 + pow(max(dot(N, normalize(vec3(mW - tC, 0.4))), 0.0), 40.0) * glossK);
    lit *= flipShade;

    // ── grout: the grid between the tiles, with light running through it ──
    float gd = max(d, -edgeCut);                                     // >0 in the grout
    vec3 groutC = tGrout * (0.85 + 0.15 * vnoise(p * 9.0)) * (1.0 - 0.3 * wear * vnoise(p * 2.3));
    // shadow of a lifted tile falls into the grout (down-right)
    float sh = smoothstep(0.0, tsz * 0.35 * (0.3 + lift), gd) ;
    groutC *= mix(0.55, 1.0, sh) * (1.0 - 0.35 * lift);
    float runPos = abs(lc.x) > abs(lc.y) * (b.x / max(b.y, 1e-3)) ? p.y : p.x;
    float runner = pow(0.5 + 0.5 * sin(runPos * 2.2 - TIME * (1.5 + 5.0 * gBuild) * (0.6 + 0.4 * gLift) + id * 2.0), 12.0);
    float gGlow = gridGlow * (0.35 * runner + 0.9 * audioBeatPulse * ar + 1.2 * gDrop + 0.5 * gBuild * runner);
    vec3 groutLit = groutC * (0.75 + 0.3 * topLight) + glowColor.rgb * gGlow;

    vec3 col = mix(groutLit, lit, inside);
    vec3 bg = backdrop(uv);
    if (back > 0.5) col = bg + glowColor.rgb * gridGlow * 0.15 * gBuild;   // behind a turning tile
    float alpha = transparentBg ? (back > 0.5 ? 0.0 : inside) : 1.0;

    // show lift, drop flash, vignette, grain
    col *= 0.86 + 0.24 * gLift;
    col += vec3(1.0, 0.95, 0.85) * gDrop * gDrop * 0.12;
    vec2 dc = uv - 0.5;
    col *= 1.0 - 0.25 * smoothstep(0.4, 1.0, length(dc * vec2(R.x / R.y, 1.0)));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * 0.015;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), alpha);
}
