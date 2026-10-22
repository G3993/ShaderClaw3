/*{
  "DESCRIPTION": "Cells — a gallery of living specimens under a fluorescence microscope. Each tile is a raymarched block of tissue whose surface is grown from an animated 3D Voronoi foam: honeycomb pits, drooping scales, ring cells, plated domes, pebble clusters, budding blobs, fibrous mats, smooth caverns and a thermal-imaged well. Twelve species, each slowly morphing into the next on its own clock, so the grid never repeats in lockstep. Lighting is emissive and false-coloured like confocal imaging — deep navy cavities, royal blue and cyan walls, acid-green rims, red-orange glow pooled in the pits — and the whole image is resolved as a fine point-cloud stipple. Bass swells the cells, beats send a fluorescent wave through every specimen, mids drift the hue, highs make the stipple sparkle.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "3D", "Nature", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "layoutMode",  "LABEL": "Layout", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1], "LABELS": ["Specimen Grid", "Single Specimen (HD morph)"], "GROUP": "Layout" },
    { "NAME": "gridCols",    "LABEL": "Columns",          "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 6.0, "DEFAULT": 4.0, "GROUP": "Layout" },
    { "NAME": "gridRows",    "LABEL": "Rows",             "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 4.0, "DEFAULT": 3.0, "GROUP": "Layout" },
    { "NAME": "gap",         "LABEL": "Spacing",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 0.4, "DEFAULT": 0.1, "GROUP": "Layout" },
    { "NAME": "species",     "LABEL": "Species (single)", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2,3,4,5,6,7,8,9,10,11],
      "LABELS": ["Foam", "Scales", "Red Cells", "Rings", "Plated Dome", "Pebbles", "Ember Dome", "Buds", "Thermal Well", "Fibres", "Caverns", "Eye Cells"], "GROUP": "Layout" },
    { "NAME": "autoMorph",   "LABEL": "Auto Morph",       "TYPE": "bool",  "DEFAULT": true, "GROUP": "Motion" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 9.0, "GROUP": "Motion" },
    { "NAME": "morphSmooth", "LABEL": "Morph Share (single)", "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Motion" },
    { "NAME": "spin",        "LABEL": "Spin",             "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Motion" },
    { "NAME": "flow",        "LABEL": "Cell Flow",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Motion" },
    { "NAME": "elevation",   "LABEL": "Camera Elevation", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Motion" },
    { "NAME": "cellScale",   "LABEL": "Cell Scale",       "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Tissue" },
    { "NAME": "depth",       "LABEL": "Relief Depth",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Tissue" },
    { "NAME": "stipple",     "LABEL": "Stipple",          "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Finish" },
    { "NAME": "dotSize",     "LABEL": "Dot Size (px)",    "TYPE": "float", "AUDIO": false, "MIN": 1.5, "MAX": 6.0, "DEFAULT": 2.0, "GROUP": "Finish" },
    { "NAME": "colDeep",     "LABEL": "Deep",             "TYPE": "color", "DEFAULT": [0.01, 0.015, 0.12, 1.0], "GROUP": "Color" },
    { "NAME": "colBlue",     "LABEL": "Blue",             "TYPE": "color", "DEFAULT": [0.04, 0.22, 0.95, 1.0], "GROUP": "Color" },
    { "NAME": "colCyan",     "LABEL": "Cyan",             "TYPE": "color", "DEFAULT": [0.18, 0.78, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "colGreen",    "LABEL": "Green",            "TYPE": "color", "DEFAULT": [0.5, 1.0, 0.12, 1.0],   "GROUP": "Color" },
    { "NAME": "colHot",      "LABEL": "Hot",              "TYPE": "color", "DEFAULT": [1.0, 0.2, 0.04, 1.0],   "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "glow",        "LABEL": "Fluorescence",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "rimLight",    "LABEL": "Rim Light",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "renderScale", "LABEL": "Render Scale",     "TYPE": "float", "AUDIO": false, "MIN": 0.35, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.015, 0.015, 0.02, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
    { "TARGET": "fScene", "WIDTH": "floor($WIDTH*$renderScale)", "HEIGHT": "floor($HEIGHT*$renderScale)" },
    { "TARGET": "fBloom", "WIDTH": "$WIDTH/4", "HEIGHT": "$HEIGHT/4" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Cells
//   pass 0  fScene : per-tile raymarch of one specimen (each pixel marches
//           only the specimen of the tile it sits in, so a 12-tile grid
//           costs the same as one). Surface = rounded block / dome whose
//           relief is an animated 3D Voronoi foam. Species are 5 vec4 param
//           sets, linearly blended → morphs are continuous.
//   pass 1  fBloom : quarter-res bright prefilter
//   pass 2  final  : bloom, point-cloud stipple at native res, tonemap
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

// ── Studio Palette: remap to black → accent → white (red or blue) ─────────
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

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
vec3  h33(vec3 p)  { p = fract(p * vec3(0.1031, 0.1030, 0.0973)); p += dot(p, p.yxz + 33.33); return fract((p.xxy + p.yxx) * p.zyx); }

float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

vec3 hueRot(vec3 c, float h) {
    if (h <= 0.0) return c;
    float a = h * TAU;
    vec3 k = vec3(0.57735);
    float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}

mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// ── species table ────────────────────────────────────────────────────────
//  A = (roundness 0 box→1 sphere, cell freq, vertical squash, relief depth)
//  B = (pit, bulge, ring, cave)            relief modes
//  C = (fibre, well, crack, size jitter)
//  D = (blue, green, red, thermal)         palette weights
//  E = (pit glow, wall width, dome, 0)
void speciesParams(float i, out vec4 A, out vec4 B, out vec4 C, out vec4 D, out vec4 E) {
    if (i < 0.5)       { A = vec4(0.22, 3.0, 1.0, 0.17); B = vec4(1.0, 0.0, 0.0, 0.0); C = vec4(0.0, 0.0, 0.0, 0.5); D = vec4(1.0, 0.0, 0.0, 0.0); E = vec4(0.0, 0.08, 0.0, 0.0); } // foam
    else if (i < 1.5)  { A = vec4(0.35, 2.6, 0.5, 0.2);  B = vec4(1.0, 0.0, 0.0, 0.2); C = vec4(0.25, 0.0, 0.0, 0.3); D = vec4(0.0, 1.0, 0.0, 0.0); E = vec4(0.0, 0.05, 0.0, 0.0); } // scales
    else if (i < 2.5)  { A = vec4(0.2, 3.4, 1.0, 0.12);  B = vec4(1.0, 0.0, 0.0, 0.0); C = vec4(0.0, 0.0, 0.0, 0.4); D = vec4(1.0, 0.0, 0.0, 0.0); E = vec4(1.0, 0.07, 0.0, 0.0); } // red cells
    else if (i < 3.5)  { A = vec4(0.12, 4.6, 1.0, 0.07); B = vec4(0.0, 0.0, 1.0, 0.0); C = vec4(0.0, 0.0, 0.0, 0.3); D = vec4(0.45, 0.55, 0.0, 0.0); E = vec4(0.0, 0.05, 0.0, 0.0); } // rings
    else if (i < 4.5)  { A = vec4(0.9, 2.3, 1.0, 0.08);  B = vec4(0.25, 0.7, 0.0, 0.0); C = vec4(0.0, 0.0, 0.7, 0.3); D = vec4(0.7, 0.3, 0.0, 0.0); E = vec4(0.0, 0.04, 0.6, 0.0); } // plated dome
    else if (i < 5.5)  { A = vec4(0.32, 1.9, 1.0, 0.17); B = vec4(0.0, 1.0, 0.0, 0.0); C = vec4(0.12, 0.0, 0.0, 0.5); D = vec4(1.0, 0.0, 0.0, 0.0); E = vec4(0.0, 0.0, 0.0, 0.0); } // pebbles
    else if (i < 6.5)  { A = vec4(1.0, 4.2, 1.0, 0.09);  B = vec4(0.0, 0.35, 0.0, 0.0); C = vec4(0.45, 0.0, 1.0, 0.3); D = vec4(0.0, 0.0, 1.0, 0.0); E = vec4(0.25, 0.05, 1.0, 0.0); } // ember dome
    else if (i < 7.5)  { A = vec4(0.55, 1.45, 1.0, 0.24); B = vec4(0.0, 1.0, 0.0, 0.0); C = vec4(0.0, 0.0, 0.0, 0.7); D = vec4(1.0, 0.0, 0.0, 0.0); E = vec4(0.0, 0.0, 0.0, 0.0); } // buds
    else if (i < 8.5)  { A = vec4(0.18, 1.2, 1.0, 0.04); B = vec4(0.0, 0.0, 0.0, 0.3); C = vec4(0.0, 1.0, 0.0, 0.0); D = vec4(0.0, 0.0, 0.0, 1.0); E = vec4(0.0, 0.0, 0.0, 0.0); } // thermal well
    else if (i < 9.5)  { A = vec4(0.22, 6.0, 0.3, 0.06); B = vec4(0.3, 0.0, 0.0, 0.0); C = vec4(1.0, 0.0, 0.0, 0.3); D = vec4(0.55, 0.45, 0.0, 0.0); E = vec4(0.9, 0.05, 0.0, 0.0); } // fibres
    else if (i < 10.5) { A = vec4(0.25, 1.8, 1.0, 0.22); B = vec4(0.0, 0.0, 0.0, 1.0); C = vec4(0.0, 0.0, 0.0, 0.6); D = vec4(1.0, 0.0, 0.0, 0.0); E = vec4(0.0, 0.0, 0.0, 0.0); } // caverns
    else               { A = vec4(0.15, 2.9, 0.8, 0.14); B = vec4(1.0, 0.0, 0.0, 0.0); C = vec4(0.0, 0.0, 0.0, 0.3); D = vec4(0.15, 0.85, 0.0, 0.0); E = vec4(0.0, 0.05, 0.0, 0.0); } // eye cells
}

// current specimen state
vec4 gA, gB, gC, gD, gE;
float gT;        // cell flow time
float gSeed;
float gDepth;
float gSpinA;
// surface attributes written by the last full map() evaluation
float gH;        // relief height (−1 deep … +1 proud)
float gPit;      // 0..1 how far inside a pit
float gWell;     // 0..1 depth into the thermal well
float gEdge;     // 0..1 proximity to a cell border

// ── animated 3D Voronoi: (F1, F2, id) ────────────────────────────────────
vec3 voronoi(vec3 q) {
    vec3 ip = floor(q), fp = fract(q);
    float f1 = 8.0, f2 = 8.0, id = 0.0;
    for (int k = -1; k <= 1; k++)
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec3 o = vec3(float(i), float(j), float(k));
        vec3 h = h33(ip + o + gSeed * 17.0);
        vec3 pt = o + 0.5 + 0.38 * sin(gT + TAU * h);
        vec3 d = pt - fp;
        float dd = dot(d, d);
        if (dd < f1) { f2 = f1; f1 = dd; id = h.x; }
        else if (dd < f2) { f2 = dd; }
    }
    return vec3(sqrt(f1), sqrt(f2), id);
}

float vnoise3(vec3 x) {
    vec3 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    float n000 = h33(p).x, n100 = h33(p + vec3(1, 0, 0)).x, n010 = h33(p + vec3(0, 1, 0)).x, n110 = h33(p + vec3(1, 1, 0)).x;
    float n001 = h33(p + vec3(0, 0, 1)).x, n101 = h33(p + vec3(1, 0, 1)).x, n011 = h33(p + vec3(0, 1, 1)).x, n111 = h33(p + vec3(1, 1, 1)).x;
    return mix(mix(mix(n000, n100, f.x), mix(n010, n110, f.x), f.y), mix(mix(n001, n101, f.x), mix(n011, n111, f.x), f.y), f.z);
}

float sdRoundBox(vec3 p, vec3 b, float r) {
    vec3 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - r;
}

float baseShape(vec3 p) {
    float rr = mix(0.1, 0.8, gA.x);
    vec3 b = vec3(0.8, 0.8 + 0.15 * gE.z, 0.8);
    vec3 pp = p;
    pp.y -= 0.12 * gE.z;
    float d = sdRoundBox(pp, b, rr);
    // domes sit on a flat cut
    d = max(d, -(p.y + mix(2.0, 0.62, gE.z)));
    return d;
}

float relief(vec3 p) {
    vec3 q = p * gA.y * cellScale;
    q.y *= gA.z;
    vec3 v = voronoi(q);
    float F1 = v.x, F2 = v.y, id = v.z;
    float e = F2 - F1;
    float jit = 1.0 - gC.w * 0.6 * id;
    float w = gE.y;

    float hPit  = -smoothstep(w, w + 0.32, e) * jit;
    float hBul  = sqrt(clamp(e * 1.7, 0.0, 1.0)) * jit - 0.55;
    float hRing = exp(-pow((F1 - 0.3) / 0.075, 2.0)) - 0.7 * smoothstep(0.24, 0.0, F1) - 0.25;
    float hCave = -smoothstep(0.6, 0.05, F1) * (0.6 + 0.4 * id);
    float hFib  = 0.0;
    if (gC.x > 0.01) hFib = (vnoise3(p * vec3(16.0, 2.2, 16.0) * cellScale + gT * 0.15) - 0.5) * 1.6;
    float hCrk  = -(1.0 - smoothstep(0.0, 0.09, e)) * smoothstep(0.35, -0.45, p.y);

    float h = gB.x * hPit + gB.y * hBul + gB.z * hRing + gB.w * hCave + gC.x * hFib + gC.z * hCrk * 1.4;
    gPit  = clamp(-(gB.x * hPit + gB.w * hCave + gC.z * hCrk) / max(gB.x + gB.w + gC.z, 0.001), 0.0, 1.0);
    gEdge = 1.0 - smoothstep(0.0, 0.18, e);
    return h;
}

float map(vec3 p) {
    float b = baseShape(p);
    // thermal well: deep funnel in the top face
    float well = 0.0;
    if (gC.y > 0.01) {
        float rr = length(p.xz - vec2(0.0, 0.1));
        well = gC.y * pow(smoothstep(0.78, 0.0, rr), 1.4) * smoothstep(-0.5, 0.75, p.y);
        b += well * 0.95;
    }
    if (b > 0.6) { gH = 0.0; return b - 0.5; }
    float h = relief(p);
    gH = h;
    gWell = clamp(well * 1.2, 0.0, 1.0);
    // beat wave swells the surface as it passes
    float pulse = audioBeatPulse * audioReact;
    float wr = (1.0 - audioBeatPulse) * 1.6;
    float wave = exp(-pow((length(p) - wr) * 6.0, 2.0)) * pulse;
    return b - gDepth * gA.w * h - 0.04 * wave;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.0014, -0.0014);
    return normalize(e.xyy * map(p + e.xyy) + e.yyx * map(p + e.yyx) + e.yxy * map(p + e.yxy) + e.xxx * map(p + e.xxx));
}

float calcAO(vec3 p, vec3 n) {
    float occ = 0.0, sca = 1.0;
    for (int i = 0; i < 4; i++) {
        float hh = 0.02 + 0.07 * float(i);
        float d = map(p + n * hh);
        occ += (hh - d) * sca;
        sca *= 0.75;
    }
    return clamp(1.0 - 3.2 * occ, 0.0, 1.0);
}

// ── palettes ─────────────────────────────────────────────────────────────
vec3 ramp4(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    t = clamp(t, 0.0, 1.2);
    vec3 col = mix(a, b, smoothstep(0.0, 0.38, t));
    col = mix(col, c, smoothstep(0.38, 0.78, t));
    col = mix(col, d, smoothstep(0.78, 1.15, t));
    return col;
}
vec3 thermal(float t) {
    // top surface hot → down the well cold
    vec3 c = colHot.rgb * 1.1;
    c = mix(c, vec3(1.0, 0.55, 0.08), smoothstep(0.1, 0.3, t));
    c = mix(c, mix(colGreen.rgb, colCyan.rgb, 0.3), smoothstep(0.3, 0.48, t));
    c = mix(c, colCyan.rgb, smoothstep(0.48, 0.62, t));
    c = mix(c, colBlue.rgb, smoothstep(0.62, 0.8, t));
    c = mix(c, colDeep.rgb * 0.6, smoothstep(0.8, 1.0, t));
    return c;
}

vec4 renderSpecimen(vec2 uv, float tileIdx) {
    // camera
    float el = mix(0.05, 0.85, elevation) + 0.14 * sin(TIME * 0.11 + tileIdx);
    vec3 ro = vec3(0.0, sin(el), cos(el)) * 4.6;
    vec3 ta = vec3(0.0, -0.05, 0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 3.3 * ww);

    // bounding sphere
    float bR = 1.6;
    float bb = dot(ro, rd);
    float cc = dot(ro, ro) - bR * bR;
    float disc = bb * bb - cc;
    if (disc < 0.0) return vec4(0.0);
    float sq = sqrt(disc);
    float t = max(-bb - sq, 0.0);
    float tFar = -bb + sq;

    // object rotation (spin about y)
    mat2 R = rot(gSpinA);
    vec3 roO = ro; roO.xz = R * roO.xz;
    vec3 rdO = rd; rdO.xz = R * rdO.xz;

    bool hit = false;
    for (int i = 0; i < 128; i++) {
        vec3 p = roO + rdO * t;
        float d = map(p);
        if (d < 0.0006 * t) { hit = true; break; }
        t += d * 0.62;
        if (t > tFar) break;
    }
    if (!hit) return vec4(0.0);

    vec3 p = roO + rdO * t;
    vec3 n = calcNormal(p);
    map(p);                         // refresh surface attributes at the hit
    float h = gH, pit = gPit, well = gWell, edge = gEdge;
    float ao = calcAO(p, n);

    float ar = audioReact;
    float facing = clamp(dot(n, -rdO), 0.0, 1.0);
    float fres = pow(1.0 - facing, 3.0);
    vec3 L = normalize(vec3(-0.35, 0.85, 0.45)); L.xz = R * L.xz;
    float key = 0.5 + 0.5 * dot(n, L);
    float ridge = clamp(1.0 - pit, 0.0, 1.0);
    // exposure: confocal look — proud tissue glows, cavities fall to navy
    float ex = ao * (0.3 + 0.7 * key) * (0.45 + 0.55 * ridge) + 0.12 * clamp(h, 0.0, 1.0);
    ex *= mix(1.0, 0.45 + 0.55 * smoothstep(-0.95, 0.35, p.y), 0.8);   // undersides fade to black
    ex = clamp(pow(ex, 1.35) * (0.62 + 0.25 * glow), 0.0, 1.1);

    vec3 cBlue  = ramp4(ex, colDeep.rgb, colBlue.rgb, colCyan.rgb, mix(colCyan.rgb, vec3(0.85, 0.97, 1.0), 0.3));
    vec3 cGreen = ramp4(ex, colDeep.rgb, mix(colBlue.rgb, colCyan.rgb, 0.4), mix(colCyan.rgb, colGreen.rgb, 0.8), mix(colGreen.rgb, vec3(0.95, 1.0, 0.6), 0.25));
    vec3 cRed   = ramp4(ex + 0.15 * (1.0 - pit), mix(colDeep.rgb, colCyan.rgb, 0.3), colHot.rgb * 0.55, colHot.rgb, mix(colHot.rgb, vec3(1.0, 0.62, 0.3), 0.6));
    // cracks in the ember dome stay teal
    cRed = mix(cRed, mix(colDeep.rgb, colCyan.rgb, 0.55) * (0.4 + 0.6 * ao), pit * gC.z);
    float heat = max(well, smoothstep(0.35, -0.85, p.y) * 0.75);
    vec3 cTherm = thermal(heat) * (0.35 + 0.75 * ao) * (0.75 + 0.35 * key);

    float wsum = max(gD.x + gD.y + gD.z + gD.w, 0.001);
    vec3 col = (cBlue * gD.x + cGreen * gD.y + cRed * gD.z + cTherm * gD.w) / wsum;

    // red glow pooled in the pits (and in fibre roots)
    float pg = gE.x * smoothstep(0.25, 0.85, pit) * (0.35 + 0.65 * ao + 0.3 * facing);
    col = mix(col, colHot.rgb * (0.9 + 0.4 * glow) * (1.0 + 0.4 * bassP() * ar), clamp(pg, 0.0, 1.0));

    // fluorescent cell walls / rims
    col += mix(colCyan.rgb, colGreen.rgb, gD.y) * edge * ridge * ao * 0.22 * glow;
    col += mix(colCyan.rgb, colGreen.rgb, gD.y * 0.7) * fres * 0.4 * rimLight * (0.6 + 0.4 * ao);

    // beat wave: a fluorescent shell racing through the specimen
    float wr = (1.0 - audioBeatPulse) * 1.6;
    float wave = exp(-pow((length(p) - wr) * 5.0, 2.0)) * audioBeatPulse * ar;
    col += mix(colCyan.rgb, vec3(1.0), 0.3) * wave * 0.9;

    col *= 1.0 + 0.25 * audioLevel * ar;
    return vec4(max(col, 0.0), 1.0);
}

vec4 scenePass() {
    vec2 Rs = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    bool single = float(layoutMode) > 0.5;
    float cols = single ? 1.0 : floor(gridCols + 0.5);
    float rows = single ? 1.0 : floor(gridRows + 0.5);
    float cell = min(Rs.x / cols, Rs.y / rows);
    vec2 origin = 0.5 * (Rs - cell * vec2(cols, rows));
    vec2 g = (fc - origin) / cell;
    if (g.x < 0.0 || g.y < 0.0 || g.x >= cols || g.y >= rows) return vec4(0.0);
    vec2 ti = floor(g);
    vec2 uv = (fract(g) * 2.0 - 1.0) / (1.0 - (single ? 0.0 : gap));
    if (single) uv *= 0.92;
    float tileIdx = ti.x + (rows - 1.0 - ti.y) * cols;

    // which species, and how far through the morph to the next one
    float base, f = 0.0;
    if (single) {
        float ph = autoMorph ? TIME / morphPeriod : 0.0;
        base = float(species) + floor(ph);
        // one specimen keeps growing from form to form: most of each period is morph
        f = smoothstep(1.0 - morphSmooth, 1.0, fract(ph));
        f = f * f * (3.0 - 2.0 * f);
    } else {
        float ph = autoMorph ? TIME / morphPeriod + h11(tileIdx * 3.7 + 0.4) * 0.999 : 0.0;
        base = tileIdx + floor(ph);
        f = smoothstep(0.72, 1.0, fract(ph));
    }
    float sa = mod(base, 12.0), sb = mod(base + 1.0, 12.0);
    vec4 A0, B0, C0, D0, E0, A1, B1, C1, D1, E1;
    speciesParams(sa, A0, B0, C0, D0, E0);
    speciesParams(sb, A1, B1, C1, D1, E1);
    gA = mix(A0, A1, f); gB = mix(B0, B1, f); gC = mix(C0, C1, f); gD = mix(D0, D1, f); gE = mix(E0, E1, f);

    float ar = audioReact;
    gSeed = h11(tileIdx * 1.31 + 0.7);
    gT = TIME * flow * 0.7 + 0.45 * audioBassTime * ar * flow + gSeed * 20.0;
    gDepth = depth * (1.0 + 0.3 * bassP() * ar);
    gSpinA = TIME * spin * 0.35 + tileIdx * 1.7 + gSeed * 3.0;

    vec4 c = renderSpecimen(uv, tileIdx);
    c.rgb = hueRot(c.rgb, fract(hueShift + 0.04 * midP() * ar));
    return c;
}

// ═════════════════════════════════════════════════════════════════════════
vec4 bloomPass() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 px = 1.0 / RENDERSIZE.xy;
    vec3 c = vec3(0.0);
    c += texture2D(fScene, uv + px * vec2(-0.25, -0.25)).rgb;
    c += texture2D(fScene, uv + px * vec2(0.25, -0.25)).rgb;
    c += texture2D(fScene, uv + px * vec2(-0.25, 0.25)).rgb;
    c += texture2D(fScene, uv + px * vec2(0.25, 0.25)).rgb;
    c *= 0.25;
    c *= smoothstep(0.45, 1.1, lum(c));
    return vec4(c, 1.0);
}

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 d = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(d)));
}

vec4 finalPass() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec4 sc = texture2D(fScene, uv);
    vec3 col = sc.rgb;

    // point-cloud stipple: every surface resolves into fine jittered dots
    float ds = dotSize;
    vec2 gp = fc / ds;
    vec2 gi = floor(gp);
    vec2 gf = fract(gp) - 0.5;
    vec2 jit = (h22(gi) - 0.5) * 0.45;
    float L = lum(col);
    float rad = 0.62 * sqrt(clamp(L * 1.3, 0.0, 1.0)) * (0.8 + 0.4 * h12(gi + 3.1));
    float dm = smoothstep(rad + 0.14, rad - 0.14, length(gf - jit));
    float tw = step(0.992 - 0.03 * highP() * audioReact, h12(gi + floor(TIME * 14.0) * 0.37));
    vec3 dotted = col * (0.18 + dm * (1.05 + 1.4 * tw));
    col = mix(col, dotted, stipple);

    // bloom
    vec2 bpx = 4.0 / R;
    vec3 b = vec3(0.0);
    float wsum = 0.0;
    for (int i = 0; i < 24; i++) {
        float fi = float(i);
        float ang = fi * 2.39996323;
        float rr = (fi + 0.5) / 24.0;
        vec2 o = vec2(cos(ang), sin(ang)) * rr * rr * 12.0 * bpx;
        float w = exp(-rr * 2.5);
        b += texture2D(fBloom, uv + o).rgb * w;
        wsum += w;
    }
    b = (b / wsum) * bloom * 0.9;

    // tissue: bloom + shoulder that keeps neon saturated
    col = (col + b) * (1.0 + 0.18 * bassP() * audioReact);
    col = 1.0 - exp(-col * 1.05);
    float l = lum(col);
    col = mix(vec3(l), col, saturation * 1.08);
    col = clamp(col, 0.0, 1.0);
    // composite over the user background (untouched by the tonemap)
    vec3 glowOut = 1.0 - exp(-b * 1.15);
    col = mix(backdrop(uv) + glowOut, col, sc.a);
    // vignette
    vec2 dc = uv - 0.5;
    col *= 1.0 - 0.1 * smoothstep(0.4, 1.0, length(dc * vec2(R.x / R.y, 1.0)));
    // grain + dither
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    float a = transparentBg ? clamp(sc.a + lum(glowOut), 0.0, 1.0) : 1.0;
    col = studioPalette(clamp(col, 0.0, 1.0));
    return vec4(clamp(col, 0.0, 1.0), a);
}

void main() {
    if (PASSINDEX == 0)      gl_FragColor = scenePass();
    else if (PASSINDEX == 1) gl_FragColor = bloomPass();
    else                     gl_FragColor = finalPass();
}
