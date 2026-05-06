/*{
  "CATEGORIES": ["Generator", "Minimalism", "Audio Reactive"],
  "DESCRIPTION": "Minimalism — Stella + the canon. Five disciplined moods, each with its own working controls. Every parameter has a visible per-frame effect. A small debug strip at the bottom edge prints the active per-mood param values so you can SEE knobs change. Returns LINEAR HDR.",
  "INPUTS": [
    { "NAME": "mood",          "LABEL": "Mood",            "TYPE": "long",  "DEFAULT": 0,
      "VALUES": [0,1,2,3,4],
      "LABELS": ["Black Paintings","Protractor","Agnes Martin","Ellsworth Kelly","LeWitt #260"] },

    { "NAME": "hairline",      "LABEL": "Hairline Gap",    "TYPE": "float", "MIN": 0.4,  "MAX": 3.0,  "DEFAULT": 0.85 },
    { "NAME": "breathDepth",   "LABEL": "Breath Depth",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "canvasTooth",   "LABEL": "Canvas Tooth",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5 },
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "showDebug",     "LABEL": "Show Debug Strip","TYPE": "bool",  "DEFAULT": 1 },

    { "NAME": "stripeCount",   "LABEL": "BP — Stripe Count",  "TYPE": "long",  "DEFAULT": 1,
      "VALUES": [0,1,2,3], "LABELS": ["4","6","8","12"] },
    { "NAME": "stripeSpacing", "LABEL": "BP — Stripe Spacing","TYPE": "float", "MIN": 0.5, "MAX": 2.5, "DEFAULT": 1.0 },

    { "NAME": "arcCount",      "LABEL": "PR — Arc Count",     "TYPE": "long",  "DEFAULT": 1,
      "VALUES": [0,1,2,3], "LABELS": ["3","5","7","9"] },
    { "NAME": "arcRotation",   "LABEL": "PR — Arc Rotation",  "TYPE": "float", "MIN": -3.1416, "MAX": 3.1416, "DEFAULT": 0.0 },

    { "NAME": "gridDensity",   "LABEL": "AM — Grid Density",  "TYPE": "long",  "DEFAULT": 1,
      "VALUES": [0,1,2,3,4], "LABELS": ["8","12","18","26","40"] },
    { "NAME": "pencilWeight",  "LABEL": "AM — Pencil Weight", "TYPE": "float", "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.0 },

    { "NAME": "colorA",        "LABEL": "EK — Color A",       "TYPE": "color", "DEFAULT": [0.870, 0.110, 0.090, 1.0] },
    { "NAME": "colorB",        "LABEL": "EK — Color B",       "TYPE": "color", "DEFAULT": [0.060, 0.180, 0.560, 1.0] },
    { "NAME": "colorC",        "LABEL": "EK — Color C",       "TYPE": "color", "DEFAULT": [0.965, 0.815, 0.140, 1.0] },

    { "NAME": "lineDirection", "LABEL": "LW — Line Directions","TYPE": "long",  "DEFAULT": 0,
      "VALUES": [0,1,2], "LABELS": ["4 directions","6 directions","8 directions"] }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
//  MINIMALISM — Stella and the canon. Each mood has its OWN working
//  controls and EVERY parameter is amplified to be unmistakably visible.
//  A small debug strip on the bottom edge prints the active params as
//  count-ticks + color swatches so a user can verify knob → render.
// ─────────────────────────────────────────────────────────────────────────

const vec3 CREAM   = vec3(0.945, 0.918, 0.838);
const vec3 PIGBLK  = vec3(0.020, 0.018, 0.020);

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float aaStep(float edge, float x) {
    float w = max(fwidth(x), 1e-5);
    return smoothstep(edge - w, edge + w, x);
}
float aaBand(float lo, float hi, float x) {
    return aaStep(lo, x) - aaStep(hi, x);
}

float decodeStripeCount(int idx) {
    if (idx <= 0) return 4.0;
    if (idx == 1) return 6.0;
    if (idx == 2) return 8.0;
    return 12.0;
}
float decodeArcCount(int idx) {
    if (idx <= 0) return 3.0;
    if (idx == 1) return 5.0;
    if (idx == 2) return 7.0;
    return 9.0;
}
float decodeGridDensity(int idx) {
    if (idx <= 0) return 8.0;
    if (idx == 1) return 12.0;
    if (idx == 2) return 18.0;
    if (idx == 3) return 26.0;
    return 40.0;
}

float canvasGrain(vec2 p, float treble) {
    vec2  q = p * RENDERSIZE.xy;
    float n = hash21(floor(q));
    return (n - 0.5) * (0.012 + 0.008 * treble);
}

// ═══════════════════════════════════════════════════════════════════════
//  MOOD 0 — BLACK PAINTINGS
//  bands  → 4/6/8/12 concentric rectangles (DRAMATICALLY different).
//  stripeMul → multiplies radial step (0.5 → 2.5x).
//  gapPx  → cream gap between bands.
// ═══════════════════════════════════════════════════════════════════════
vec3 blackPaintings(vec2 uv, float bands, float stripeMul, float gapPx,
                    float bass, float mid) {
    vec2  c = uv - 0.5;
    float dx = abs(c.x) / 0.5;
    float dy = abs(c.y) / 0.5;
    float d  = max(dx, dy);

    float breath  = 1.0 + 0.02 * bass;
    // bands directly = number of visible rectangles within d∈[0..1].
    float spacing = bands * stripeMul * (1.0 + 0.015 * mid);
    float bf      = d * spacing / breath;

    float fb     = fract(bf);
    // Gap thickness scales with stripeMul so denser stripes still show gaps.
    float gapU   = (gapPx / RENDERSIZE.y) * spacing;
    float inGap  = 1.0 - aaBand(0.5 - gapU, 0.5 + gapU, fb);
    float margin = aaStep(0.985, d);

    vec3 col = mix(PIGBLK, CREAM, inGap);
    col      = mix(col, CREAM, margin);
    return col;
}

// ═══════════════════════════════════════════════════════════════════════
//  MOOD 1 — PROTRACTOR SERIES
//  arcs        → 3/5/7/9 visible arcs FILLING the half-disc.
//  arcRotation → rotates palette index around arcs (-π…π).
//  gapPx       → cream between arcs.
// ═══════════════════════════════════════════════════════════════════════
vec3 protractor(vec2 uv, float arcs, float rotation, float gapPx,
                float bass, float mid) {
    vec2 p = uv - vec2(0.5, 0.0);
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    p.x *= aspect;

    float r       = length(p);
    float breath  = 1.0 + 0.02 * bass;

    // Normalise so the outermost arc lands near the canvas edge regardless
    // of arcCount: r_max ≈ 0.95 → bf goes 0..arcs across the visible disc.
    float rNorm   = r / 0.95;
    float spacing = arcs * (1.0 + 0.015 * mid);
    float bf      = rNorm * spacing / breath;

    float rotShift = rotation / 6.2831853 * 6.0;
    float idxF     = floor(bf) + rotShift;
    int   idx      = int(floor(mod(idxF, 6.0) + 0.5));
    if (idx < 0) idx += 6;
    if (idx >= 6) idx -= 6;

    vec3 pal[6];
    pal[0] = vec3(0.880, 0.090, 0.085);
    pal[1] = vec3(0.040, 0.420, 0.330);
    pal[2] = vec3(0.040, 0.110, 0.430);
    pal[3] = vec3(0.960, 0.500, 0.080);
    pal[4] = vec3(0.970, 0.870, 0.380);
    pal[5] = vec3(0.820, 0.140, 0.520);
    vec3 pigment = pal[idx];

    float fb    = fract(bf);
    float gapU  = (gapPx / RENDERSIZE.y) * spacing;
    float inGap = 1.0 - aaBand(0.5 - gapU, 0.5 + gapU, fb);

    float maxBand = aaStep(arcs, bf);
    vec3 col      = mix(pigment, CREAM, inGap);
    col           = mix(col, CREAM, maxBand);
    col           = mix(col, CREAM, 1.0 - aaStep(0.0, p.y));
    return col;
}

// ═══════════════════════════════════════════════════════════════════════
//  MOOD 2 — AGNES MARTIN
//  bands → 8/12/18/26/40 lines per axis.
//  pencilMul → AMPLIFIED line thickness (0.2…3.0) — clearly thickens.
// ═══════════════════════════════════════════════════════════════════════
vec3 agnesMartin(vec2 uv, float bands, float pencilMul,
                 float bass, float mid) {
    float breath  = 1.0 + 0.02 * bass;
    float spacing = bands * (1.0 + 0.015 * mid);
    vec2  g       = uv * spacing / breath;
    vec2  fg      = fract(g);

    float dx = min(fg.x, 1.0 - fg.x);
    float dy = min(fg.y, 1.0 - fg.y);

    // AMPLIFIED: base width 1.2px, scaled by pencilMul² for dramatic spread.
    float lw = (1.2 / RENDERSIZE.y) * spacing * pencilMul * pencilMul;
    float lineX = 1.0 - aaStep(lw, dx);
    float lineY = 1.0 - aaStep(lw, dy);
    float line  = max(lineX, lineY);

    // Heavier pencil → noticeably darker graphite.
    float graphiteAmt = clamp(0.22 + 0.18 * mid, 0.0, 0.65)
                      * line
                      * clamp(0.4 + 0.5 * pencilMul, 0.4, 1.9);
    vec3  graphite    = vec3(0.28, 0.26, 0.24);
    return mix(CREAM, graphite, graphiteAmt);
}

// ═══════════════════════════════════════════════════════════════════════
//  MOOD 3 — ELLSWORTH KELLY — three user-supplied colour fields.
// ═══════════════════════════════════════════════════════════════════════
vec3 ellsworthKelly(vec2 uv, vec3 cA, vec3 cB, vec3 cC,
                    float bass, float mid) {
    float br   = 0.01 * (bass - 0.5) + 0.005 * (mid - 0.5);
    float seam = 0.42 + br;
    float horz = 0.55 + br;

    float leftMask  = 1.0 - aaStep(seam, uv.x);
    float topRight  = aaStep(seam, uv.x) * aaStep(horz, uv.y);
    float botRight  = aaStep(seam, uv.x) * (1.0 - aaStep(horz, uv.y));

    float tilt    = (1.0 - uv.y) * 0.06;
    float trapCut = aaStep(seam + tilt, uv.x);
    botRight *= trapCut;
    float creamWedge = aaStep(seam, uv.x) * (1.0 - aaStep(horz, uv.y))
                     * (1.0 - trapCut);

    vec3 col = cA * leftMask
             + cB * topRight
             + cC * botRight
             + CREAM * creamWedge;
    return col;
}

// ═══════════════════════════════════════════════════════════════════════
//  MOOD 4 — SOL LEWITT, WALL DRAWING #260
//  dirMode → 4 / 6 / 8 directions of pencil hatching (AMPLIFIED contrast).
// ═══════════════════════════════════════════════════════════════════════
vec3 lewitt260(vec2 uv, float bands, int dirMode,
               float bass, float mid) {
    float breath  = 1.0 + 0.02 * bass;
    // More directions → tighter spacing, so the layered ink gets visibly
    // darker and the field reads denser. Big jumps between modes.
    float dirBoost = (dirMode == 0) ? 1.0 : (dirMode == 1) ? 1.6 : 2.3;
    float spacing = bands * 1.4 * dirBoost * (1.0 + 0.015 * mid);
    float lineW   = 1.0 / RENDERSIZE.y * spacing;

    int   nDirs   = (dirMode == 0) ? 4 : (dirMode == 1) ? 6 : 8;
    // Per-pass darkening AMPLIFIED so 4 vs 8 dirs is unmistakable.
    float pass    = (dirMode == 0) ? 0.22 : (dirMode == 1) ? 0.20 : 0.18;

    float keep = 1.0;
    for (int i = 0; i < 8; i++) {
        if (i >= nDirs) break;
        float ang = 3.1415926 * float(i) / float(nDirs);
        float ax  = uv.x * cos(ang) + uv.y * sin(ang);
        float fr  = fract(ax * spacing / breath);
        float dl  = min(fr, 1.0 - fr);
        float ln  = 1.0 - aaStep(lineW, dl);
        keep *= (1.0 - pass * ln);
    }
    float ink = 1.0 - keep;
    vec3 graphite = vec3(0.28, 0.26, 0.24);
    return mix(CREAM, graphite, ink);
}

// ═══════════════════════════════════════════════════════════════════════
//  DEBUG STRIP — bottom 2.5% of frame.
//  Prints, for the active mood, the params actually consumed:
//    Mood 0: stripeCount ticks + stripeSpacing bar
//    Mood 1: arcCount ticks + arcRotation bar (centered)
//    Mood 2: gridDensity ticks + pencilWeight bar
//    Mood 3: 3 colour swatches (A / B / C)
//    Mood 4: lineDirection ticks (4/6/8)
//  Returns (color, mask). mask=1 inside the strip, 0 elsewhere.
// ═══════════════════════════════════════════════════════════════════════
vec3 debugStrip(vec2 uv, int m, float spCount, float spMul,
                float arcN, float arcRot, float gridN, float pencilM,
                vec3 cA, vec3 cB, vec3 cC, int dirMode,
                out float mask) {
    float stripH = 0.025;
    mask = 1.0 - aaStep(stripH, uv.y);
    if (mask < 0.001) return vec3(0.0);

    // Local coords inside the strip: sx in [0..1] across, sy in [0..1] up.
    float sx = uv.x;
    float sy = uv.y / stripH;

    // Background: dark slate so cream readouts pop.
    vec3 bg = vec3(0.06, 0.06, 0.07);
    vec3 col = bg;

    // Mood label colour band (left 8%) — colour-codes the mood.
    vec3 moodColor;
    if      (m == 0) moodColor = vec3(0.95, 0.95, 0.92);  // BP cream
    else if (m == 1) moodColor = vec3(0.88, 0.09, 0.09);  // PR red
    else if (m == 2) moodColor = vec3(0.55, 0.52, 0.48);  // AM graphite
    else if (m == 3) moodColor = cA;                       // EK A
    else             moodColor = vec3(0.30, 0.28, 0.26);  // LW graphite
    float moodBand = 1.0 - aaStep(0.07, sx);
    col = mix(col, moodColor, moodBand);

    // Region 1: count ticks in sx ∈ [0.10 .. 0.55].
    // Region 2: bar/rotation in sx ∈ [0.60 .. 0.98].
    float r1Lo = 0.10, r1Hi = 0.55;
    float r2Lo = 0.60, r2Hi = 0.98;

    // ── ticks: render N evenly spaced bright bars in region 1.
    float nTicks = 0.0;
    if      (m == 0) nTicks = spCount;
    else if (m == 1) nTicks = arcN;
    else if (m == 2) nTicks = gridN;
    else if (m == 4) nTicks = (dirMode == 0) ? 4.0 : (dirMode == 1) ? 6.0 : 8.0;

    if (nTicks > 0.5 && sx > r1Lo && sx < r1Hi) {
        float t = (sx - r1Lo) / (r1Hi - r1Lo);
        float pos = t * nTicks;
        float fp  = fract(pos);
        // tick = bright when near integer
        float tick = 1.0 - aaStep(0.18, abs(fp - 0.5) * 2.0);
        // Confine vertically to mid 60%.
        float vBand = aaBand(0.2, 0.8, sy);
        col = mix(col, vec3(0.96, 0.94, 0.86), tick * vBand * 0.95);
    }

    // ── region 2: per-mood readout.
    if (sx > r2Lo && sx < r2Hi) {
        float t = (sx - r2Lo) / (r2Hi - r2Lo);
        if (m == 0) {
            // stripeSpacing bar: fills 0..1 mapped from 0.5..2.5.
            float v = clamp((spMul - 0.5) / 2.0, 0.0, 1.0);
            float bar = 1.0 - aaStep(v, t);
            float vBand = aaBand(0.25, 0.75, sy);
            col = mix(col, vec3(0.96, 0.94, 0.86), bar * vBand);
        } else if (m == 1) {
            // arcRotation: centred bar, signed.
            float v = clamp(arcRot / 3.1416, -1.0, 1.0);
            float center = 0.5;
            float lo = min(center, center + v * 0.5);
            float hi = max(center, center + v * 0.5);
            float bar = aaBand(lo, hi, t);
            float vBand = aaBand(0.3, 0.7, sy);
            vec3 c = (v >= 0.0) ? vec3(0.4, 0.85, 0.95) : vec3(0.95, 0.6, 0.4);
            col = mix(col, c, bar * vBand);
            // centre tick
            float ctick = 1.0 - aaStep(0.004, abs(t - 0.5));
            col = mix(col, vec3(0.5), ctick);
        } else if (m == 2) {
            // pencilWeight bar 0.2..3.0.
            float v = clamp((pencilM - 0.2) / 2.8, 0.0, 1.0);
            float bar = 1.0 - aaStep(v, t);
            float vBand = aaBand(0.25, 0.75, sy);
            col = mix(col, vec3(0.30, 0.28, 0.26) * 1.5 + vec3(0.4), bar * vBand);
        } else if (m == 3) {
            // 3 colour swatches A / B / C.
            float seg = floor(t * 3.0);
            vec3 sw = (seg < 0.5) ? cA : (seg < 1.5) ? cB : cC;
            float vBand = aaBand(0.15, 0.85, sy);
            col = mix(col, sw, vBand);
        } else if (m == 4) {
            // dirMode label dots: 4 / 6 / 8 — show all 3 with active highlighted.
            float seg = floor(t * 3.0);
            int activeIdx = (dirMode == 0) ? 0 : (dirMode == 1) ? 1 : 2;
            float on = (abs(seg - float(activeIdx)) < 0.5) ? 1.0 : 0.25;
            float dotMask = aaBand(0.3, 0.7, sy)
                          * (1.0 - aaStep(0.12, abs(fract(t * 3.0) - 0.5)));
            col = mix(col, vec3(0.96, 0.94, 0.86) * on, dotMask);
        }
    }

    return col;
}

// ─── main ────────────────────────────────────────────────────────────────
void main() {
    vec2  uv = isf_FragNormCoord.xy;
    float t  = TIME;

    float aR     = clamp(audioReact, 0.0, 2.0);
    float silent = 0.5 + 0.5 * sin(t * 6.2831853 * 0.05);
    float bass   = mix(silent, clamp(audioBass, 0.0, 1.0), 0.55) * aR;
    float mid    = mix(silent * 0.5, clamp(audioMid, 0.0, 1.0), 0.55) * aR;
    float treb   = clamp(audioHigh, 0.0, 1.0) * aR;

    bass *= clamp(breathDepth, 0.0, 1.0);
    mid  *= clamp(breathDepth, 0.0, 1.0);

    int   m  = int(mood + 0.5);
    float gp = clamp(hairline, 0.4, 3.0);

    float spCount = decodeStripeCount(int(stripeCount + 0.5));
    float spMul   = clamp(stripeSpacing, 0.5, 2.5);
    float arcN    = decodeArcCount(int(arcCount + 0.5));
    float arcRot  = clamp(arcRotation, -3.1416, 3.1416);
    float gridN   = decodeGridDensity(int(gridDensity + 0.5));
    float pencilM = clamp(pencilWeight, 0.2, 3.0);
    int   dirMode = int(lineDirection + 0.5);

    vec3 col;
    if      (m == 0) col = blackPaintings(uv, spCount, spMul, gp, bass, mid);
    else if (m == 1) col = protractor    (uv, arcN, arcRot, gp, bass, mid);
    else if (m == 2) col = agnesMartin   (uv, gridN, pencilM, bass, mid);
    else if (m == 3) col = ellsworthKelly(uv, colorA.rgb, colorB.rgb, colorC.rgb, bass, mid);
    else             col = lewitt260     (uv, gridN, dirMode, bass, mid);

    col += canvasGrain(uv, treb) * clamp(canvasTooth, 0.0, 1.0);

    // Debug strip overlay (bottom 2.5%).
    if (showDebug) {
        float dmask;
        vec3 dcol = debugStrip(uv, m, spCount, spMul, arcN, arcRot,
                               gridN, pencilM,
                               colorA.rgb, colorB.rgb, colorC.rgb,
                               dirMode, dmask);
        col = mix(col, dcol, dmask);
    }

    gl_FragColor = vec4(col, 1.0);
}
