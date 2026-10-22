/*{
  "DESCRIPTION": "Chess — a living pixel grid in datamosh. Everything snaps to a crisp grid of big pixels: 1-bit moiré dither fields, checkerboard columns at several scales, blurry rainbow mosaic blocks, and Tetris-shaped patches filled with cross-stitch motifs (dots, crosses, X's, hex rings, stars, stripes) in red, pink, yellow, blue, green, orange and lavender. Pixel-art flowers and green-and-yellow pixel worms crawl through it like organisms under a microscope. The patterns slide through the grid band by band, reshuffle and morph, and the whole picture is datamoshed: macroblocks drag the previous frame along motion vectors so the image smears and bleeds like corrupted video, while only scattered blocks refresh with the new frame. At rest the pixels are big, the mosh is gentle and only a few flowers wander; during a build the grid gets finer, the smear runs longer and the slices tear faster; on the drop a clean keyframe snaps everything into place with an inverted flash, then the peak moshes at full speed with every pattern alive.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "pixelSize",   "LABEL": "Pixel Size",       "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 24.0, "DEFAULT": 7.0, "GROUP": "Grid" },
    { "NAME": "tileSize",    "LABEL": "Pattern Tile (px)","TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 12.0, "DEFAULT": 6.0, "GROUP": "Grid" },
    { "NAME": "patternMix",  "LABEL": "Colour Patches",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Grid" },
    { "NAME": "checkerAmt",  "LABEL": "Checkerboards",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Grid" },
    { "NAME": "flowers",     "LABEL": "Pixel Flowers",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Creatures" },
    { "NAME": "vines",       "LABEL": "Red Vines",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Creatures" },
    { "NAME": "worms",       "LABEL": "Pixel Worms",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Creatures" },
    { "NAME": "moshAmt",     "LABEL": "Datamosh",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Glitch" },
    { "NAME": "moshBlock",   "LABEL": "Macroblock (px)",  "TYPE": "float", "AUDIO": false, "MIN": 2.0, "MAX": 12.0, "DEFAULT": 4.0, "GROUP": "Glitch" },
    { "NAME": "tearAmt",     "LABEL": "Slice Tears",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Glitch" },
    { "NAME": "stepRate",    "LABEL": "Digital Frame Rate","TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 30.0, "DEFAULT": 12.0, "GROUP": "Glitch" },
    { "NAME": "speed",       "LABEL": "Speed",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Glitch" },
    { "NAME": "inkColor",    "LABEL": "1-Bit Light",      "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0], "GROUP": "Look" },
    { "NAME": "fabric",      "LABEL": "Felt Texture",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Look" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.04, 0.04, 0.05, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
    { "TARGET": "moshBuf", "PERSISTENT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vn2(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

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

float gT;      // continuous clock
float gStep;   // digital (quantized) clock
float gPx;     // screen pixels per grid cell

// the reference palette: red, magenta/pink, yellow, blue, green, orange, lavender, mint, cream
vec3 swatch(float k) {
    k = mod(floor(k), 9.0);
    if (k < 0.5) return vec3(0.95, 0.06, 0.06);
    if (k < 1.5) return vec3(0.95, 0.55, 0.65);
    if (k < 2.5) return vec3(1.0, 0.88, 0.05);
    if (k < 3.5) return vec3(0.07, 0.3, 0.95);
    if (k < 4.5) return vec3(0.15, 0.85, 0.2);
    if (k < 5.5) return vec3(1.0, 0.5, 0.08);
    if (k < 6.5) return vec3(0.45, 0.15, 0.5);
    if (k < 7.5) return vec3(0.68, 0.88, 0.7);
    return vec3(0.95, 0.93, 0.85);
}

// motif masks inside a tile; l = local cell (0..T-1)
float motif(float k, vec2 l, float T) {
    vec2 c = l - floor(T * 0.5);           // centred
    float ax = abs(c.x), ay = abs(c.y);
    float r = floor(T * 0.5) - 1.0;
    if (k < 0.5) return step(ax, 0.5) * step(ay, 0.5) + step(max(ax, ay), 0.0);              // dot
    if (k < 1.5) return step(min(ax, ay), 0.0) * step(max(ax, ay), r);                        // plus
    if (k < 2.5) return step(abs(ax - ay), 0.0) * step(ax, r);                                // X
    if (k < 3.5) return step(abs(ax + ay - r), 0.0);                                          // diamond / hex ring
    if (k < 4.5) return step(min(ax, ay), 0.0) * step(max(ax, ay), r) + step(abs(ax - ay), 0.0) * step(ax, r - 1.0); // star
    if (k < 5.5) return step(mod(l.y, 2.0), 0.5);                                             // stripes
    return step(length(c), r + 0.2);                                                          // filled circle
}

// ordered dither 4x4 (recursive bayer)
float bayer2(vec2 a) { a = floor(a); return fract(dot(a, vec2(0.5, a.y * 0.75))); }
float bayer4(vec2 a) { return bayer2(0.5 * a) * 0.25 + bayer2(a); }

// ── the grid scene at an integer cell ────────────────────────────────────
vec3 scene(vec2 c, vec2 screenCell) {
    float T = floor(tileSize);
    float lift = gLift;
    // bands slide through the grid at their own speed, one cell per digital step
    float band = floor(c.y / (T * 2.0));
    float bandSpeed = (h11(band * 3.7) - 0.5) * 2.0;
    vec2 g = c;
    g.x += floor(gStep * bandSpeed * (0.5 + 0.8 * lift) * speed);
    // pattern regions: polyomino blobs on the tile grid
    vec2 tile = floor(g / T);
    vec2 l = g - tile * T;
    float shuffle = floor(gStep / (24.0 - 14.0 * lift));            // regions reshuffle in steps
    float blob = vn2(tile * 0.33 + vec2(shuffle * 1.7, shuffle * 0.9));
    float group = floor(vn2(tile * 0.17 + 5.0 + shuffle * 0.4) * 9.0);
    // 1-bit moiré dither field underneath everything (ref: wavy hatch)
    float wave = 0.5 + 0.5 * sin(g.x * 0.55 + g.y * 0.2 + 3.0 * sin(g.y * 0.07 + gT * 0.4) + gT * 0.6);
    float field = smoothstep(0.0, 1.0, vn2(g * 0.03 + vec2(gT * 0.05, 0.0)));
    float vstripe = 0.5 + 0.5 * sin(g.x * 1.7 + 2.0 * sin(g.y * 0.15 + gT * 0.5));
    float hatch = step(bayer4(g + vec2(0.0, floor(wave * 3.0))), mix(wave, vstripe, field) * (0.15 + 0.95 * field));
    vec3 col = mix(vec3(0.0), inkColor.rgb, hatch);
    // void areas: black with sparse white dashes (ref: right side)
    float voidMask = smoothstep(0.62, 0.7, vn2(g * 0.025 + vec2(-gT * 0.04, 3.0)));
    float dash = step(0.93, h12(vec2(floor(g.x / 2.0), g.y) + floor(gStep * 0.5))) * step(mod(g.y, 3.0), 0.5);
    dash = max(dash, step(0.88, h12(g + floor(gStep * 0.7) * 1.3)));        // white speckle
    col = mix(col, mix(vec3(0.0), inkColor.rgb, dash), voidMask);

    // checkerboard columns and blocks at several scales (ref: poster)
    float cz = floor(c.x / (T * 3.0));
    float cOn = step(1.0 - checkerAmt * (0.5 + 0.5 * lift), h11(cz * 7.1 + floor(shuffle * 0.5)));
    float cs = pow(2.0, floor(h11(cz * 3.3 + 1.0) * 3.0));                // 1, 2 or 4 cells
    vec2 cg = floor((g + vec2(0.0, floor(gStep * 0.5 * (h11(cz) - 0.5) * 2.0))) / cs);
    float check = mod(cg.x + cg.y, 2.0);
    col = mix(col, vec3(check), cOn * step(0.45, vn2(vec2(cz, g.y * 0.05 + shuffle))) * step(mod(c.x, T * 3.0), T * 1.5));

    // rainbow mosaic blocks: a big blurry gradient seen through coarse pixels
    vec2 mg = floor(g / 3.0) * 3.0;
    float mm = smoothstep(0.66, 0.7, vn2(mg * 0.02 + vec2(4.0, gT * 0.03))) * (0.4 + 0.6 * gLift);
    vec3 rainbow = 0.55 + 0.45 * cos(TAU * (vn2(mg * 0.04 + gT * 0.05) * 1.5 + mg.y * 0.004 + vec3(0.0, 0.33, 0.67)));
    rainbow = mix(rainbow, vec3(1.0), 0.15);
    col = mix(col, rainbow, mm * 0.95);

    // cross-stitch patches: flat colour + motif in a contrasting colour
    float pOn = step(1.0 - patternMix * (0.45 + 0.6 * lift), blob) * step(0.45, vn2(tile * 0.12 + 20.0 + shuffle * 0.3));
    if (pOn > 0.5) {
        vec3 base = swatch(group);
        float mk = floor(h12(tile * 1.3 + group + shuffle) * 7.0);
        vec3 ink = swatch(group + 2.0 + floor(h12(tile + 9.0) * 6.0));
        float mt = motif(mk, l, T);
        // some patches are just motifs on black (dots grid etc.)
        bool bare = h12(vec2(group, shuffle)) > 0.7;
        vec3 pc = bare ? mix(vec3(0.0), ink, mt) : mix(base, ink, mt);
        // patch outlines are blocky (polyomino); felt texture
        float felt = 1.0 - fabric * 0.18 * h12(c * 1.7);
        col = pc * felt;
    }
    return col;
}

// pixel flowers (7x7 sprites) and pixel worms crawling over the grid
vec3 creatures(vec2 c, vec3 col) {
    float lift = gLift;
    // flowers
    float nF = floor(4.0 + 12.0 * flowers * (0.35 + 0.65 * lift));
    for (int i = 0; i < 16; i++) {
        float fi = float(i);
        if (fi >= nF) break;
        vec2 hh = h22(vec2(fi, 3.1));
        vec2 gridN = RENDERSIZE / gPx;
        // wander: smooth path, snapped to the grid each digital step
        vec2 pos = floor(gridN * (hh + 0.2 * vec2(sin(gStep * 0.05 * (0.5 + hh.y) + fi), cos(gStep * 0.043 * (0.5 + hh.x) + fi * 2.0))));
        pos = mod(pos, gridN);
        vec2 d = c - pos;
        float ax = abs(d.x), ay = abs(d.y);
        if (max(ax, ay) > 3.0) continue;
        float bloom = floor(1.0 + 1.5 * (0.5 + 0.5 * sin(gStep * 0.2 + fi)) + gDrop * 1.5);
        // petals: plus-shaped rings of red/pink with a magenta + yellow heart
        float petal = step(ax + ay, bloom + 1.0) * step(1.0, ax + ay);
        float heart = step(ax + ay, 0.5);
        float corner = step(abs(ax - ay), 0.0) * step(ax, bloom) * step(1.0, ax);
        vec3 pcol = swatch(h11(fi * 1.7) > 0.4 ? 0.0 : 1.0);
        if (petal > 0.5 && corner < 0.5) col = pcol;
        if (corner > 0.5) col = vec3(0.95, 0.15, 0.85);                 // magenta corners
        if (heart > 0.5) col = vec3(0.0);
        if (d.y < 0.0 && ax < 0.5 && ay <= 3.0 && petal < 0.5) col = vec3(0.15, 0.85, 0.2); // stem
    }
    // red vines: jagged branching paths that crawl and grow across the grid (ref: red lightning-vine)
    vec2 gridV = RENDERSIZE / gPx;
    float nV = floor(1.0 + 4.0 * vines * (0.4 + 0.6 * lift));
    for (int v = 0; v < 5; v++) {
        float fv = float(v);
        if (fv >= nV) break;
        float epoch = floor(gStep / 90.0 + h11(fv) );                    // each vine regrows every so often
        float grow = fract(gStep / 90.0 + h11(fv)) * (1.0 + gBuild) + 0.15 * gDrop;
        vec2 p0 = floor(gridV * h22(vec2(fv, epoch + 2.0)));
        vec2 dir = normalize(h22(vec2(epoch, fv + 7.0)) - 0.5 + 1e-3);
        for (int sgi = 0; sgi < 10; sgi++) {
            float fs = float(sgi);
            if (fs / 10.0 > grow) break;
            vec2 turn = (h22(vec2(fs + fv * 13.0, epoch)) - 0.5) * 1.4;
            dir = normalize(dir + turn);
            float len = 4.0 + floor(h11(fs + fv * 7.0 + epoch) * 6.0);
            vec2 p1 = floor(p0 + dir * len);
            // distance from this cell to the segment, in cells
            vec2 pa = c - p0, ba = p1 - p0;
            float hseg = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-3), 0.0, 1.0);
            float dseg = length(pa - ba * hseg);
            float thick = 1.2 + 0.8 * h11(fs + fv);
            if (dseg < thick) {
                col = vec3(0.98, 0.04, 0.04);
                if (h12(c + floor(gStep * 0.3)) > 0.92) col = vec3(0.95, 0.15, 0.85);   // pink buds
            } else if (dseg < thick + 1.2 && h12(c * 1.3 + epoch) > 0.86) {
                col = h12(c + 5.0) > 0.5 ? vec3(0.1, 0.9, 0.15) : vec3(1.0, 0.9, 0.05); // green / yellow leaves
            }
            // a branch every few segments
            if (mod(fs, 3.0) > 1.5) {
                vec2 bd = vec2(-dir.y, dir.x) * (h11(fs + epoch) > 0.5 ? 1.0 : -1.0);
                vec2 b1 = floor(p1 + bd * (3.0 + 3.0 * h11(fs + fv)));
                vec2 qa = c - p1, qb = b1 - p1;
                float hq = clamp(dot(qa, qb) / max(dot(qb, qb), 1e-3), 0.0, 1.0);
                if (length(qa - qb * hq) < 0.9) col = vec3(0.98, 0.04, 0.04);
            }
            p0 = p1;
        }
    }
    // worms: chains of green/yellow pixels following a curve
    float nW = floor(2.0 + 8.0 * worms * (0.3 + 0.7 * lift));
    for (int i = 0; i < 10; i++) {
        float fi = float(i);
        if (fi >= nW) break;
        vec2 hh = h22(vec2(fi, 11.7));
        vec2 gridN = RENDERSIZE / gPx;
        float t = gStep * (0.15 + 0.1 * hh.x) * (0.5 + lift);
        // head position and the body as a run of segments behind it
        for (int s = 0; s < 9; s++) {
            float fs = float(s);
            float u = t - fs * 0.35;
            vec2 p = gridN * hh + vec2(u * 2.0 * (hh.y - 0.5) * 3.0, 6.0 * sin(u * 0.6 + fi)) + vec2(4.0 * sin(u * 0.25), 0.0);
            p = mod(floor(p), gridN);
            vec2 d = abs(c - p);
            if (max(d.x, d.y) < 0.5) col = mod(fs + fi, 3.0) < 1.0 ? vec3(1.0, 0.9, 0.05) : vec3(0.1, 0.9, 0.15);
            else if (d.x + d.y < 1.5 && mod(fs, 2.0) < 0.5) col = mix(col, vec3(0.1, 0.9, 0.15), 0.85);
        }
    }
    return col;
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    float ar = audioReact;
    gT = TIME * 0.6 * speed + (audioMidTime * 0.4 + audioBassTime * 0.3) * ar * speed + 1.5 * gBuild;
    gStep = floor((TIME * 0.4 + audioMidTime * 0.5 * ar + audioBassTime * 0.3 * ar) * stepRate * speed + 6.0 * gBuild * gBuild);
    // pixel size: big at rest, finer as the build climbs (snaps to whole pixels)
    gPx = floor(max(pixelSize * (1.25 - 0.5 * gLift - 0.25 * gBuild + 0.3 * gRest), 2.0));

    vec2 cell = floor(gl_FragCoord.xy / gPx);
    vec2 cc = (cell + 0.5) * gPx;

    if (PASSINDEX == 0) {
        // ── datamosh buffer ──
        // slice tears: rows of cells shift sideways for a step
        float rowBand = floor(cell.y / (2.0 + floor(h11(floor(gStep * 0.5)) * 6.0)));
        float tear = step(1.0 - tearAmt * (0.12 + 0.35 * gLift + 0.4 * gBuild), h12(vec2(rowBand, floor(gStep))));
        vec2 c = cell + vec2(floor((h12(vec2(rowBand, floor(gStep) + 3.0)) - 0.5) * 24.0) * tear, 0.0);
        vec3 fresh = scene(c, cell);
        fresh = creatures(cell, fresh);

        // macroblock motion vectors (whole cells, so the grid stays crisp)
        float MB = floor(moshBlock);
        vec2 mb = floor(cell / MB);
        vec2 flow = vec2(vn2(mb * 0.15 + vec2(gT * 0.15, 0.0)), vn2(mb * 0.15 + vec2(7.0, gT * 0.12))) - 0.5;
        vec2 mv = floor(flow * (2.0 + 4.0 * gLift + 6.0 * gBuild) + 0.5);
        // vertical drip zones: blocks smear straight down like corrupted video
        float drip = step(0.6, vn2(vec2(mb.x * 0.25, gT * 0.08) + 9.0));
        if (drip > 0.5) mv = vec2(0.0, 1.0 + floor(2.0 * h12(mb + floor(gStep * 0.25))));
        vec2 prevUV = (cc - mv * gPx) / R;
        vec3 prev = texture2D(moshBuf, prevUV).rgb;
        // which blocks refresh this frame (P-frame residuals); keyframe on the drop
        float mosh = moshAmt * (0.55 + 0.35 * gLift + 0.35 * gBuild) * (1.0 - smoothstep(0.3, 0.8, gDrop));
        float refresh = step(mosh, h12(mb + vec2(floor(gStep), 17.0)));
        if (FRAMEINDEX < 3) refresh = 1.0;
        // smeared blocks bleed a little colour (chroma drift) as they travel
        vec3 smear = prev;
        smear.r = mix(smear.r, texture2D(moshBuf, prevUV + vec2(gPx / R.x, 0.0)).r, 0.06 * mosh * step(0.8, h12(mb + floor(gStep * 0.5))));
        vec3 col = mix(smear, fresh, refresh);
        gl_FragColor = vec4(col, 1.0);
        return;
    }

    // ── output ──
    vec3 col = texture2D(moshBuf, cc / R).rgb;
    vec2 uv = gl_FragCoord.xy / R;
    // void cells take the background colour
    float black = step(max(col.r, max(col.g, col.b)), 0.02);
    vec3 bg = backdrop(uv);
    col = mix(col, bg, black);
    // drop: inverted flash for a moment
    col = mix(col, 1.0 - col, step(0.75, gDrop) * 0.9);
    col *= 0.7 + 0.4 * gLift + 0.12 * audioBeatPulse * ar;
    // thin grid seams between pixels (subtle)
    vec2 fp = fract(gl_FragCoord.xy / gPx);
    col *= 1.0 - 0.1 * step(1.0 - 1.0 / gPx, max(fp.x, fp.y)) * step(5.0, gPx);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? 1.0 - black : 1.0);
}
