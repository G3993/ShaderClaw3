/*{
  "CATEGORIES": ["Generator", "Text", "Audio Reactive"],
  "DESCRIPTION": "Solari split-flap departures board — grid of mechanical flap cells animating between glyphs with the iconic mid-flip blur. Cells stagger their flips so the board 'wakes' row-by-row when a new message arrives. Audio-bass triggers a fresh flip cycle; the board feels alive and busy. Yellow-on-charcoal is the canonical Solari di Udine palette.",
  "INPUTS": [
    { "NAME": "cellsX",         "LABEL": "Cells X",         "TYPE": "float", "MIN": 6.0,  "MAX": 48.0, "DEFAULT": 22.0 },
    { "NAME": "cellsY",         "LABEL": "Cells Y",         "TYPE": "float", "MIN": 2.0,  "MAX": 18.0, "DEFAULT": 6.0 },
    { "NAME": "flipPeriod",     "LABEL": "Flip Period (s)", "TYPE": "float", "MIN": 0.6,  "MAX": 8.0,  "DEFAULT": 2.5 },
    { "NAME": "flipDuration",   "LABEL": "Flip Duration",   "TYPE": "float", "MIN": 0.05, "MAX": 0.6,  "DEFAULT": 0.18 },
    { "NAME": "rowStagger",     "LABEL": "Row Stagger",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.8,  "DEFAULT": 0.12 },
    { "NAME": "cellGap",        "LABEL": "Cell Gap",        "TYPE": "float", "MIN": 0.0,  "MAX": 0.40, "DEFAULT": 0.12 },
    { "NAME": "cellRound",      "LABEL": "Cell Corner",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.30, "DEFAULT": 0.08 },
    { "NAME": "midFlipShadow",  "LABEL": "Mid-Flip Shadow", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "hingeLine",      "LABEL": "Hinge Line",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.65 },
    { "NAME": "boardColor",     "LABEL": "Board Color",     "TYPE": "color", "DEFAULT": [0.08, 0.08, 0.10, 1.0] },
    { "NAME": "cellColor",      "LABEL": "Cell Color",      "TYPE": "color", "DEFAULT": [0.05, 0.05, 0.06, 1.0] },
    { "NAME": "glyphColor",     "LABEL": "Glyph Color",     "TYPE": "color", "DEFAULT": [0.97, 0.85, 0.10, 1.0] },
    { "NAME": "audioReact",     "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

// Pseudo-glyph — 5×7 hash-driven bitmap. Each cell shows a different
// "letter" by indexing into a deterministic pattern from its glyph seed.
// Looks like a real flap-board character without needing a font atlas.
float drawGlyph(vec2 cuv, float seed) {
    // Centre and scale so glyphs leave margin from cell edges.
    cuv = (cuv - 0.5) / 0.78 + 0.5;
    if (cuv.x < 0.0 || cuv.x > 1.0 || cuv.y < 0.0 || cuv.y > 1.0) return 0.0;
    vec2 g = floor(cuv * vec2(5.0, 7.0));
    // Deterministic glyph "shape" via two hash channels — gives slight
    // structure (top/bottom bars, vertical stems) per glyph.
    float h1 = hash21(g + vec2(seed * 7.13, 0.0));
    float h2 = hash21(g + vec2(0.0, seed * 11.1));
    // Bias toward central columns + edges so glyphs look letter-like.
    float colBias = (g.x == 0.0 || g.x == 4.0) ? 0.42 : 0.55;
    return step(colBias, mix(h1, h2, 0.5));
}

float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = boardColor.rgb;

    int NX = int(clamp(cellsX, 1.0, 48.0));
    int NY = int(clamp(cellsY, 1.0, 18.0));
    float fNX = float(NX), fNY = float(NY);

    // Cell coords
    vec2 gp = vec2(uv.x * fNX, uv.y * fNY);
    vec2 gi = floor(gp);
    vec2 gf = fract(gp);
    // Recentre to [-0.5, 0.5] for SDF.
    vec2 cp = gf - 0.5;

    // Cell aspect — make cells taller than wide (split-flap proportion).
    float cellAspect = (fNY * RENDERSIZE.x) / (fNX * RENDERSIZE.y);
    cp.x *= cellAspect;

    // Cell rounded-rect mask. Avoid `half` (reserved word in some
    // GLSL profiles).
    vec2  halfSz = vec2(0.5 - cellGap * 0.5) * vec2(cellAspect, 1.0);
    float sd     = sdRoundBox(cp, halfSz, cellRound);
    float cellMask = smoothstep(0.005, -0.005, sd);
    if (cellMask < 0.001) {
        gl_FragColor = vec4(col, 1.0);
        return;
    }

    // Cell base
    col = mix(boardColor.rgb, cellColor.rgb, cellMask);

    // Per-cell flip cycle. Each cell has its own phase offset so the
    // board doesn't all flip in sync. Row stagger so the wave reads as
    // top-down or bottom-up cascade. Audio bass tightens cycles.
    float cellSeed = hash11(gi.x * 13.7 + gi.y * 41.3);
    float period   = flipPeriod / max(1.0 + audioBass * audioReact * 0.6, 0.05);
    float phase    = cellSeed * 5.0
                   + (fNY - 1.0 - gi.y) * rowStagger
                   + gi.x * 0.013;
    float t = mod(TIME, period * 9.7) / period;
    float era = floor(t + phase);
    float frac = fract(t + phase);

    // Glyph IDs — current and next.
    float seedNow  = hash11(cellSeed * 19.3 + era);
    float seedNext = hash11(cellSeed * 19.3 + era + 1.0);

    // Flip phase: 0..flipDuration is the "in motion" window. Outside that
    // window the cell shows seedNow steady. During the window we draw the
    // upper half = old glyph clipped, lower half = new glyph clipped, plus
    // a moving hinge bar simulating the falling flap.
    float flipPhase = frac / flipDuration;
    bool  flipping  = flipPhase < 1.0;

    // Hinge horizontal line in cell-local Y.
    float hingeY = 0.0;            // centre line
    float upperMask = step(cp.y, hingeY);
    float lowerMask = 1.0 - upperMask;

    // Local glyph UV — un-aspect-correct so glyph reads square.
    vec2 glyphUV = vec2(cp.x / cellAspect, cp.y) / (1.0 - cellGap) + 0.5;

    float glyphAlpha = 0.0;
    float darkenFlip = 0.0;
    if (!flipping) {
        // Steady state — show seedNow over whole cell.
        glyphAlpha = drawGlyph(glyphUV, seedNow);
    } else {
        // Upper half — top of new glyph appearing.
        // Lower half — bottom of old glyph slightly darkened by
        // motion shadow.
        // First half of flipPhase: top flap of old falls into hinge,
        // exposes top of new. Second half: bottom flap of new rotates
        // into place.
        float half1 = step(flipPhase, 0.5);
        float half2 = 1.0 - half1;

        // Old upper mask shrinks down toward hingeY in first half.
        float oldUpperShrink = mix(1.0, 0.0, smoothstep(0.0, 0.5, flipPhase));
        float newLowerGrow   = mix(0.0, 1.0, smoothstep(0.5, 1.0, flipPhase));

        // Build composite mask for the moving flap zone.
        // Old glyph above hinge for half1 (shrinking), new glyph above
        // hinge for half2.
        float topGlyph = mix(drawGlyph(glyphUV, seedNow),
                             drawGlyph(glyphUV, seedNext),
                             smoothstep(0.45, 0.55, flipPhase));
        // Bottom (lower half) glyph: shows old until midway, new after.
        float botGlyph = mix(drawGlyph(glyphUV, seedNow),
                             drawGlyph(glyphUV, seedNext),
                             smoothstep(0.45, 0.55, flipPhase));

        glyphAlpha = topGlyph * upperMask + botGlyph * lowerMask;

        // Mid-flip darkening shadow — simulates the falling flap blocking
        // light from the back of the cell.
        float fallY = mix(0.0, -0.5, smoothstep(0.0, 0.5, flipPhase))
                    + mix(-0.5, 0.0, smoothstep(0.5, 1.0, flipPhase));
        float shadowBand = smoothstep(0.10, 0.0, abs(cp.y - fallY));
        darkenFlip = shadowBand * midFlipShadow * half1
                   + smoothstep(0.04, 0.0, abs(cp.y - fallY)) * 0.4 * half2;
    }

    col = mix(col, glyphColor.rgb, glyphAlpha * cellMask);
    col *= 1.0 - darkenFlip * cellMask;

    // Hinge horizontal line — thin dark divider.
    float hingeBand = smoothstep(0.012, 0.000, abs(cp.y));
    col = mix(col, boardColor.rgb * 0.45, hingeBand * cellMask * hingeLine);

    gl_FragColor = vec4(col, 1.0);
}
