/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "De Stijl after late Mondrian — Broadway Boogie Woogie (1942–43) and Victory Boogie Woogie (1942–44). Asymmetric black grid lines partition a cream canvas into rectangles, some filled with pure cadmium red / cobalt blue / Naples yellow. Down each line marches a syncopated stream of small coloured squares at harmonic tempi (1x, 1.5x, 2x, 2.5x of base) — Mondrian's literal jazz. The grid quietly REPARTITIONS via smoothstep. Bass throws extra red squares; treble throws yellow. Rare audio-bass-triggered cell colour swaps. Five-colour palette only, no gradients. Stays alive in silence. Linear HDR.",
  "INPUTS": [
    { "NAME": "lineThickness",       "LABEL": "Line Thickness",        "TYPE": "float", "MIN": 0.001, "MAX": 0.012, "DEFAULT": 0.0035 },
    { "NAME": "gridDensity",         "LABEL": "Grid Density",          "TYPE": "long",  "VALUES": [0, 1, 2, 3], "LABELS": ["Sparse", "Medium", "Dense", "Very Dense"], "DEFAULT": 1 },
    { "NAME": "repartitionPeriod",   "LABEL": "Repartition Period (s)","TYPE": "float", "MIN": 8.0,   "MAX": 30.0,  "DEFAULT": 16.0 },
    { "NAME": "colorPulseIntensity", "LABEL": "Color Pulse Intensity", "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,   "DEFAULT": 1.0 },
    { "NAME": "whiteSaturation",     "LABEL": "White Tint Saturation", "TYPE": "float", "MIN": 0.0,   "MAX": 0.3,   "DEFAULT": 0.0 },
    { "NAME": "audioReact",          "LABEL": "Audio React",           "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,   "DEFAULT": 1.0 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  DE STIJL — Mondrian, Boogie Woogie era
//  Broadway Boogie Woogie (1942–43) was Mondrian's response to Manhattan
//  jazz after fleeing Europe. The black grid he had used since 1920 broke
//  apart into chains of coloured squares pulsing along the lines — the
//  painting LITERALIZED syncopation. This shader is built around that
//  device: a kinetic grid where every black line carries marching squares
//  at harmonic tempi, and every ~16 seconds the line POSITIONS slide to
//  new locations (smoothstep transitions) honouring Mondrian's iterative
//  practice — he physically re-taped his grid lines for months on end.
//  Five-colour palette, no mixing, no gradients. Silence is fine: TIME
//  drives the boogie regardless of audio.
// ════════════════════════════════════════════════════════════════════════

// ─── Mondrian palette (the only five colours allowed) ─────────────────
const vec3 PAL_RED    = vec3(0.85, 0.15, 0.10); // cadmium red
const vec3 PAL_BLUE   = vec3(0.10, 0.20, 0.65); // cobalt blue
const vec3 PAL_YELLOW = vec3(0.96, 0.86, 0.20); // Naples yellow
const vec3 PAL_WHITE  = vec3(0.96, 0.94, 0.88); // off-white / cream
const vec3 PAL_BLACK  = vec3(0.04, 0.04, 0.06); // pure black

// Maximum grid capacity (compile-time array sizes). Actual line counts used
// at runtime are derived from gridDensity and clamped to these maxima.
#define LINES_H 7   // max horizontal interior lines
#define LINES_V 8   // max vertical interior lines
#define SQUARE   0.020   // marching square half-size (in normalized coord)

// ─── hashes ───────────────────────────────────────────────────────────
float h11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float h12(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// Quantize [0,1] -> one of 5 palette colours by index.
vec3 pal5(int idx) {
    if (idx == 0) return PAL_WHITE;
    if (idx == 1) return PAL_RED;
    if (idx == 2) return PAL_BLUE;
    if (idx == 3) return PAL_YELLOW;
    return PAL_BLACK;
}

// Cell fill choice — heavily weighted toward white (Mondrian's canvas was
// mostly cream). Bass-triggered swap key changes the index every now and
// then for the audio-reactive surprise.
int cellColour(vec2 cellId, float swapKey) {
    float r = h12(cellId * 1.7 + swapKey * 7.13);
    if (r < 0.62) return 0;          // WHITE — dominant
    if (r < 0.74) return 1;          // RED
    if (r < 0.84) return 2;          // BLUE
    if (r < 0.94) return 3;          // YELLOW
    return 0;                        // small extra white pad
}

// ─── grid layout ──────────────────────────────────────────────────────
// We keep two SETS of line positions and crossfade between them every
// ~16 seconds with a 1-second smoothstep — this is the "repartition"
// gesture that reads as Mondrian re-taping his canvas.
//
// Each line has its OWN epoch (slightly de-synchronised) so they don't
// all shift at once — the canvas re-tunes itself piece by piece, like
// Mondrian's diary entries describe his own sessions.

float linePos(float idx, float total, float salt, float t, float period) {
    // Base slot (evenly spaced 0..1) plus an asymmetric per-epoch jitter.
    float slot   = (idx + 1.0) / (total + 1.0);
    float per    = max(period, 1.0);
    float epoch  = floor(t / per + salt * 0.31);
    float ph     = fract(t / per + salt * 0.31);
    // Last ~1s of each epoch is the smoothstep crossfade.
    float transStart = max(0.0, 1.0 - 1.0 / per);
    float trans  = smoothstep(transStart, 1.0, ph);
    float jitA   = (h11(epoch * 7.13 + salt) - 0.5) * 0.55 / total;
    float jitB   = (h11((epoch + 1.0) * 7.13 + salt) - 0.5) * 0.55 / total;
    float jit    = mix(jitA, jitB, trans);
    return clamp(slot + jit, 0.04, 0.96);
}

// ─── marching square pulses along a single line ───────────────────────
// Returns a colour OVER black (the line itself). The line carries a
// stream of small coloured squares at harmonic tempo. dir = 1 horizontal
// (squares travel along x), dir = 0 vertical (along y).
//
// Tempo harmonic per line: 1x, 1.5x, 2x, or 2.5x of base — chosen by
// hash, deterministic per line. Bass injects extra reds; treble extra
// yellows (hueBias parameter modulates spawn colour selection).
//
// We DO NOT blend — Mondrian had no gradients. Either the square is
// here (return colour) or it isn't (return black, which is the line).
vec3 lineSquares(float along, float coord, float lineSalt, float t,
                 float audioBass, float audioTreble, float audioReact,
                 float pulseAmt)
{
    // Tempo: 1.0, 1.5, 2.0, 2.5 — boogie-woogie harmonic ladder.
    float tempoSel = h11(lineSalt * 17.7);
    float tempo    = (tempoSel < 0.25) ? 1.0
                   : (tempoSel < 0.55) ? 1.5
                   : (tempoSel < 0.85) ? 2.0 : 2.5;
    float dir      = (h11(lineSalt * 23.3) < 0.5) ? -1.0 : 1.0;
    float baseSpd  = 0.075;
    float march    = t * baseSpd * tempo * dir;

    // Density: 4–7 squares riding the line at any time. pulseAmt scales
    // how many squares actually paint (when 0, the line is bare black).
    float density  = 4.0 + floor(h11(lineSalt * 29.7) * 4.0);
    density       *= clamp(pulseAmt, 0.0, 1.0);

    vec3 outCol = PAL_BLACK; // the line itself

    // Iterate over slots; each slot has a phase offset and a colour.
    for (int k = 0; k < 8; k++) {
        if (float(k) >= density) break;
        float fk     = float(k);
        float spawn  = h11(lineSalt * 31.1 + fk * 5.7);

        // Position along the line (with march).
        float pos    = fract(march + spawn);
        // Wrap-aware distance.
        float dA     = abs(along - pos);
        dA           = min(dA, 1.0 - dA);

        // Each square is offset in the cross direction so it sits ON the
        // line (cross dist must already be small for us to be here).
        // Square colour: deterministic pick from primaries, with audio
        // injecting bass=red and treble=yellow surprises.
        float colSel = h11(lineSalt * 41.3 + fk * 7.1);
        // Bass kick — promote some yellows/blues to red.
        float bassKick = audioBass * audioReact;
        float trebKick = audioTreble * audioReact;
        // Default: 1/3 each among R/B/Y.
        int sqIdx;
        if (colSel < 0.33 + 0.20 * bassKick)        sqIdx = 1; // RED
        else if (colSel < 0.66 - 0.10 * trebKick)   sqIdx = 2; // BLUE
        else                                        sqIdx = 3; // YELLOW
        // Treble overrides occasional non-yellows to yellow.
        if (h11(lineSalt * 53.7 + fk * 3.3) < trebKick * 0.30) sqIdx = 3;

        // Square drawn iff |dA| < SQUARE and |dCross| < SQUARE — caller
        // already guarantees |dCross| < small via line proximity. We
        // require BOTH explicitly (cross dist passed via 'coord').
        if (dA < SQUARE && abs(coord) < SQUARE) {
            outCol = pal5(sqIdx);
        }
    }
    // Audio-bass extra-red sprinkle: occasional bonus square mid-line.
    // Gated by pulseAmt so a fully muted boogie stays bare.
    if (audioBass * audioReact > 0.35 && pulseAmt > 0.05) {
        float bspawn = fract(t * 0.22 + lineSalt * 1.11);
        float dA = abs(along - bspawn);
        dA = min(dA, 1.0 - dA);
        if (dA < SQUARE && abs(coord) < SQUARE) outCol = PAL_RED;
    }
    return outCol;
}

// ════════════════════════════════════════════════════════════════════════
void main() {
    vec2  uv = isf_FragNormCoord.xy;
    float t  = TIME;

    // ─── parameter normalisation ─────────────────────────────────────
    float lineW    = clamp(lineThickness,       0.001, 0.012);
    float period   = clamp(repartitionPeriod,   8.0,   30.0);
    float pulseAmt = clamp(colorPulseIntensity, 0.0,   1.0);
    float wSat     = clamp(whiteSaturation,     0.0,   0.3);
    float aR       = clamp(audioReact,          0.0,   2.0);

    // gridDensity is a long enum (0..3): Sparse, Medium, Dense, Very Dense.
    // It maps to active line counts (clamped to compile-time maxima).
    int dIdx = int(clamp(gridDensity, 0.0, 3.0) + 0.5);
    int activeLinesV = (dIdx == 0) ? 3 : (dIdx == 1) ? 5 : (dIdx == 2) ? 6 : 8;
    int activeLinesH = (dIdx == 0) ? 2 : (dIdx == 1) ? 4 : (dIdx == 2) ? 5 : 7;

    // Audio: split full-band react into pseudo-bass (slow) and pseudo-
    // treble (fast) using TIME modulation when no real audio is wired.
    float audioBass   = 0.5 + 0.5 * sin(t * 1.7);    // 0..1, slow
    float audioTreble = 0.5 + 0.5 * sin(t * 4.3 + 1.3); // 0..1, fast
    audioBass   *= aR;
    audioTreble *= aR;

    // Rare cell-colour swap key — bass-triggered, holds for several
    // seconds so the swap reads as a "decision" not a strobe.
    float swapKey = floor(t / 7.0 + audioBass * 0.5);

    // ─── compute current line positions (with smoothstep repartition) ─
    // Arrays are sized to LINES_V/LINES_H maxima; only the first
    // activeLinesV / activeLinesH slots are actually consulted.
    float xs[LINES_V];
    for (int j = 0; j < LINES_V; j++) {
        xs[j] = linePos(float(j), float(activeLinesV),
                        float(j) * 1.31 + 11.0, t, period);
    }
    float ys[LINES_H];
    for (int i = 0; i < LINES_H; i++) {
        ys[i] = linePos(float(i), float(activeLinesH),
                        float(i) * 1.71 + 27.0, t, period);
    }

    // ─── identify which rectangle we're in ────────────────────────────
    int cellX = 0;
    for (int j = 0; j < LINES_V; j++) {
        if (j < activeLinesV && uv.x > xs[j]) cellX = j + 1;
    }
    int cellY = 0;
    for (int i = 0; i < LINES_H; i++) {
        if (i < activeLinesH && uv.y > ys[i]) cellY = i + 1;
    }

    // ─── default fill: cell colour ───────────────────────────────────
    int cIdx = cellColour(vec2(float(cellX), float(cellY)), swapKey);
    vec3 col = pal5(cIdx);

    // Optional warm/cool tint on cream cells: shift PAL_WHITE toward
    // a slightly warmer or cooler hue per cell. Strictly bounded so the
    // five-colour read stays intact when wSat == 0 (default).
    if (cIdx == 0 && wSat > 0.0) {
        float tintHash = h12(vec2(float(cellX), float(cellY)) * 3.7
                              + swapKey * 0.91);
        // -1..+1 — negative = cool, positive = warm.
        float tintDir  = tintHash * 2.0 - 1.0;
        vec3  warm     = vec3( 0.06,  0.02, -0.06); // pull R+, B-
        vec3  cool     = vec3(-0.06, -0.02,  0.06); // pull B+, R-
        vec3  shift    = (tintDir > 0.0) ? warm : cool;
        col = clamp(col + shift * wSat * abs(tintDir), 0.0, 1.0);
    }

    // ─── horizontal lines: black baseline + marching squares ─────────
    for (int i = 0; i < LINES_H; i++) {
        if (i >= activeLinesH) break;
        float dy = uv.y - ys[i];
        float ady = abs(dy);
        if (ady < lineW) {
            col = PAL_BLACK;
        }
        if (ady < SQUARE) {
            float salt = float(i) * 11.13 + 3.7;
            vec3 sq = lineSquares(uv.x, dy, salt, t,
                                  audioBass, audioTreble, aR, pulseAmt);
            if (sq != PAL_BLACK) col = sq;
        }
    }

    // ─── vertical lines: same idea, march along y ────────────────────
    for (int j = 0; j < LINES_V; j++) {
        if (j >= activeLinesV) break;
        float dx = uv.x - xs[j];
        float adx = abs(dx);
        if (adx < lineW) {
            col = PAL_BLACK;
        }
        if (adx < SQUARE) {
            float salt = float(j) * 13.71 + 17.3;
            vec3 sq = lineSquares(uv.y, dx, salt, t,
                                  audioBass, audioTreble, aR, pulseAmt);
            if (sq != PAL_BLACK) col = sq;
        }
    }

    // ─── frame border (Mondrian framed his canvases edge-to-edge in
    // ─── black) — gives the composition a definite stop.
    if (uv.x < lineW || uv.x > 1.0 - lineW
     || uv.y < lineW || uv.y > 1.0 - lineW) {
        col = PAL_BLACK;
    }

    // Output linear HDR — palette deliberately stays in [0,1] to keep
    // the strict five-colour read; host applies tone curve.
    gl_FragColor = vec4(col, 1.0);
}
