/*{
  "CATEGORIES": ["Generator", "Pattern", "Audio Reactive"],
  "DESCRIPTION": "Memphis Group, Milano 1981 — quiet, calculated. ONE composition per mood after a real piece: Sottsass's 'Carlton' (1981) staggered rectangle stack with a circle on top; du Pasquier's 'Bacterio' (1981) black amoeba squiggles on cream; the 'Treetops' lamp silhouette (single tall triangle, horizontal stripes, sun); de Lucchi's 'First' chair (1983) angular figure on cream. Two or three elements, never more. Six-colour hard rule. LINEAR HDR.",
  "INPUTS": [
    { "NAME": "mood",             "LABEL": "Composition",       "TYPE": "long",
      "VALUES": [0, 1, 2, 3], "LABELS": ["Carlton", "Bacterio", "Treetops", "First Chair"], "DEFAULT": 0 },
    { "NAME": "audioReact",       "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "paletteShift",     "LABEL": "Palette Shift",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.0 },
    { "NAME": "compositionDrift", "LABEL": "Composition Drift", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "bloomLift",        "LABEL": "Bloom Lift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.15 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//   MEMPHIS PRIMITIVES — 1981 Milano, restrained.
//   Memphis is calculated CLASH between simple shapes, not chaos.
//   One composition. Two or three elements. Six colours. Cream ground.
// ════════════════════════════════════════════════════════════════════════

// ─── six-colour palette (HARD RULE) ────────────────────────────────────
const vec3 CREAM = vec3(0.95, 0.90, 0.82);
const vec3 PINK  = vec3(0.95, 0.55, 0.72);
const vec3 TEAL  = vec3(0.18, 0.62, 0.65);
const vec3 YEL   = vec3(0.95, 0.78, 0.20);
const vec3 BLK   = vec3(0.10, 0.08, 0.10);
const vec3 RED   = vec3(0.85, 0.20, 0.18);

// Per-mood palette rotation (a tonal nudge, never breaks the six).
vec3 paletteRotate(vec3 c, float shift) {
    // Cycle PINK→TEAL→YEL→RED, leave CREAM/BLK alone.
    float s = clamp(shift, 0.0, 1.0);
    if (s < 0.001) return c;
    if (all(lessThan(abs(c - PINK), vec3(0.01)))) return mix(PINK, TEAL, s);
    if (all(lessThan(abs(c - TEAL), vec3(0.01)))) return mix(TEAL, YEL,  s);
    if (all(lessThan(abs(c - YEL),  vec3(0.01)))) return mix(YEL,  RED,  s);
    if (all(lessThan(abs(c - RED),  vec3(0.01)))) return mix(RED,  PINK, s);
    return c;
}

// ─── hashes / tiny noise (used only by Bacterio) ───────────────────────
float h11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float h21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    float a = h21(i);
    float b = h21(i + vec2(1.0, 0.0));
    float c = h21(i + vec2(0.0, 1.0));
    float d = h21(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

// ─── audio reactivity (gentle, single channel) ─────────────────────────
// Audio is a light tonal nudge — Memphis pieces don't pulse.
float readPulse(float t) {
    float r = clamp(audioReact, 0.0, 2.0);
    float base = 0.5 + 0.5 * sin(t * 0.41);
    return clamp(0.25 + r * 0.45 * base, 0.0, 1.0);
}

// Bass-triggered SCALE PULSE — each kick the central element grows ~5% then settles.
// Decays smoothly so we don't get strobing. Gated by audioReact.
float readKick(float t) {
    float r = clamp(audioReact, 0.0, 2.0);
    float bass = clamp(audioBass, 0.0, 4.0);
    // Synthetic kick fallback when audio is silent: a sharp envelope every ~1.6s.
    float phase = fract(t * 0.62);
    float synth = pow(1.0 - phase, 6.0);
    float k = max(bass * 0.9, synth * 0.55);
    return clamp(k, 0.0, 1.0) * r;
}

// 2D rotation helper — used for slow rotation of colored shapes.
vec2 rot2(vec2 p, float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c) * p;
}

// ─── primitives (SDF helpers, alpha = inside) ──────────────────────────
float sdRect(vec2 p, vec2 c, vec2 halfSize) {
    vec2 d = abs(p - c) - halfSize;
    return max(d.x, d.y);
}
float sdCircle(vec2 p, vec2 c, float r) { return length(p - c) - r; }

float sdTriangle(vec2 p, vec2 c, float halfW, float h) {
    // Isoceles: tip up at c + (0, h/2), base at c - (0, h/2).
    vec2 q = p - c;
    q.x = abs(q.x);
    // edges: from (halfW, -h/2) to (0, h/2)
    vec2 e = vec2(-h, 2.0 * halfW);
    e = normalize(e);
    float side = dot(q - vec2(halfW, -h * 0.5), vec2(e.y, -e.x));
    float bottom = -(q.y + h * 0.5);
    return max(side, bottom);
}

// Crisp anti-aliased fill (no smear).
float fillAA(float d, float px) {
    return 1.0 - smoothstep(-px, px, d);
}

// Layer paint: dst stays where src is empty. src is (rgb, coverage).
vec3 paint(vec3 dst, vec3 src, float cov) { return mix(dst, src, clamp(cov, 0.0, 1.0)); }

// ════════════════════════════════════════════════════════════════════════
//   MOOD 0 — "Carlton" (Sottsass, 1981)
//   Staggered colored rectangles. ONE big circle on top. Black accent.
// ════════════════════════════════════════════════════════════════════════
vec3 moodCarlton(vec2 uv, float ar, float t, float pulse, float drift, float pShift, float kick) {
    // Aspect-aware coords centred on canvas (uv in 0..1).
    vec2 p = (uv - 0.5) * vec2(ar, 1.0);
    float px = 1.5 / RENDERSIZE.y;

    // Slow side-to-side breath (drift). Memphis pieces don't translate
    // so we orbit the composition by a few pixels only.
    float dx = sin(t * 0.18) * 0.012 * drift;
    float dy = cos(t * 0.22) * 0.008 * drift;
    p -= vec2(dx, dy);

    // Central scale pulse — kick grows the whole piece by ~5%, then settles.
    float scl = 1.0 / (1.0 + 0.05 * kick);
    p *= scl;

    vec3 col = CREAM;

    // 1) Bottom slab — wide pink rectangle. Slow rotate.
    {
        vec3 c = paletteRotate(PINK, pShift);
        vec2 ctr = vec2(-0.05, -0.22);
        vec2 q = rot2(p - ctr, sin(t * 0.05) * 0.06) + ctr;
        float d = sdRect(q, ctr, vec2(0.34, 0.10));
        col = paint(col, c, fillAA(d, px));
        col = paint(col, BLK, fillAA(abs(d) - 0.006, px));
    }

    // 2) Tall teal column — staggered to the right. Counter-rotate.
    {
        vec3 c = paletteRotate(TEAL, pShift);
        vec2 ctr = vec2(0.16, 0.02);
        vec2 q = rot2(p - ctr, -sin(t * 0.05) * 0.05) + ctr;
        float d = sdRect(q, ctr, vec2(0.07, 0.30));
        col = paint(col, c, fillAA(d, px));
        col = paint(col, BLK, fillAA(abs(d) - 0.006, px));
    }

    // 3) Yellow square — offset upper-left. Slow tumble.
    {
        vec3 c = paletteRotate(YEL, pShift);
        vec2 ctr = vec2(-0.22, 0.10);
        vec2 q = rot2(p - ctr, t * 0.05) + ctr;
        float d = sdRect(q, ctr, vec2(0.10, 0.10));
        col = paint(col, c, fillAA(d, px));
        col = paint(col, BLK, fillAA(abs(d) - 0.006, px));
    }

    // 4) The signature CIRCLE on top — red, the resolution of the composition.
    //    Bass-triggered scale pulse: grows 5% on kick, plus a gentle bob.
    {
        vec3 c = paletteRotate(RED, pShift);
        float bob = sin(t * 1.1) * 0.004;
        float r = (0.085 + 0.005 * pulse) * (1.0 + 0.05 * kick);
        float d = sdCircle(p, vec2(0.04, 0.30 + bob), r);
        col = paint(col, c, fillAA(d, px));
        col = paint(col, BLK, fillAA(abs(d) - 0.006, px));
    }
    return col;
}

// ════════════════════════════════════════════════════════════════════════
//   MOOD 1 — "Bacterio" (du Pasquier, 1981)
//   Black amoeba squiggles on cream. Full canvas. NOTHING else.
// ════════════════════════════════════════════════════════════════════════
vec3 moodBacterio(vec2 uv, float ar, float t, float pulse, float drift, float kick) {
    vec2 p = (uv - 0.5) * vec2(ar, 1.0) * 5.5;
    // Very slow drift only.
    p += vec2(t * 0.03, -t * 0.018) * (0.4 + drift * 0.5);

    // FBM warp — amoebas wobble/morph slowly. Two evolving noise layers,
    // each panning at a different rate, give the squiggles real motion.
    float w1 = vnoise(p * 0.9 + vec2(t * 0.07, 3.1)) * 1.6;
    float w2 = vnoise(p * 1.7 + vec2(-t * 0.05, 7.3)) * 0.6;
    vec2  q  = p + vec2(w1 + w2, -w1 * 0.6 + w2 * 0.4);
    float n  = vnoise(q * 0.8 + vec2(t * 0.04, -t * 0.03));

    // Iso-band → squiggle. Tighter band = thinner ink.
    float band = abs(n - 0.5);
    // Bass kick thickens the ink ~5% briefly.
    float thick = 0.075 + 0.012 * pulse + 0.006 * kick;
    float ink = 1.0 - smoothstep(thick - 0.012, thick, band);

    // Pure black on cream — the original Bacterio.
    return mix(CREAM, BLK, ink);
}

// ════════════════════════════════════════════════════════════════════════
//   MOOD 2 — "Treetops"
//   Single tall triangle with horizontal stripes + a sun-circle behind.
// ════════════════════════════════════════════════════════════════════════
vec3 moodTreetops(vec2 uv, float ar, float t, float pulse, float drift, float pShift, float kick) {
    vec2 p = (uv - 0.5) * vec2(ar, 1.0);
    float px = 1.5 / RENDERSIZE.y;

    // Subtle composition orbit.
    float dx = sin(t * 0.14) * 0.010 * drift;
    p.x -= dx;

    vec3 col = CREAM;

    // 1) Sun — yellow disc behind/right of the triangle.
    //    Gentle vertical bounce, plus a 5% scale pulse on kicks.
    {
        vec3 c = paletteRotate(YEL, pShift);
        float bob = sin(t * 0.9) * 0.008;
        float r = (0.16 + 0.006 * pulse) * (1.0 + 0.05 * kick);
        float d = sdCircle(p, vec2(0.20, 0.18 + bob), r);
        col = paint(col, c, fillAA(d, px));
        col = paint(col, BLK, fillAA(abs(d) - 0.006, px));
    }

    // 2) Tall triangle — teal field, horizontal pink stripes inside.
    //    Slow rotation about base; stripes drift downward over time.
    {
        vec3 baseTri = paletteRotate(TEAL, pShift);
        vec3 stripe  = paletteRotate(PINK, pShift);
        vec2 triC = vec2(-0.04, -0.02);
        float halfW = 0.20;
        float h     = 0.62;
        // Slow rotation pivot at triangle center.
        float ang = sin(t * 0.05) * 0.05;
        vec2 pr = rot2(p - triC, ang) + triC;
        float d = sdTriangle(pr, triC, halfW, h);
        float inside = fillAA(d, px);

        // Horizontal stripes drift slowly (Treetops mood). Six bands.
        float yLocal = (pr.y - (triC.y - h * 0.5)) / h;
        float slide = t * 0.04;
        float stripeMask = step(0.5, fract(yLocal * 6.0 + slide));
        vec3 triCol = mix(baseTri, stripe, stripeMask);

        col = paint(col, triCol, inside);
        col = paint(col, BLK, fillAA(abs(d) - 0.007, px));
    }

    // 3) A small black square — the de-resolution mark, lower right. Pulses on kicks.
    {
        float s = 0.045 * (1.0 + 0.10 * kick);
        float d = sdRect(p, vec2(0.30, -0.30), vec2(s, s));
        col = paint(col, BLK, fillAA(d, px));
    }
    return col;
}

// ════════════════════════════════════════════════════════════════════════
//   MOOD 3 — "First Chair" (de Lucchi, 1983)
//   Stylized angular figure: thin stick, circular seat, one accent ball.
// ════════════════════════════════════════════════════════════════════════
vec3 moodFirstChair(vec2 uv, float ar, float t, float pulse, float drift, float pShift, float kick) {
    vec2 p = (uv - 0.5) * vec2(ar, 1.0);
    float px = 1.5 / RENDERSIZE.y;

    // Gentle composition rock.
    float dx = sin(t * 0.20) * 0.010 * drift;
    p.x -= dx;

    // Whole figure tilts ever so slightly.
    float tilt = sin(t * 0.05) * 0.04;
    vec2 pr = rot2(p, tilt);

    vec3 col = CREAM;

    // 1) Vertical black stick (the spine).
    {
        float d = sdRect(pr, vec2(0.0, 0.05), vec2(0.012, 0.30));
        col = paint(col, BLK, fillAA(d, px));
    }

    // 2) Horizontal black bar (the back-rest crossbar).
    {
        float d = sdRect(pr, vec2(0.0, 0.24), vec2(0.16, 0.014));
        col = paint(col, BLK, fillAA(d, px));
    }

    // 3) Round teal seat — disc on the stick. Bass kick grows it 5%.
    {
        vec3 c = paletteRotate(TEAL, pShift);
        float r = (0.13 + 0.005 * pulse) * (1.0 + 0.05 * kick);
        float d = sdCircle(pr, vec2(0.0, -0.06), r);
        col = paint(col, c, fillAA(d, px));
        col = paint(col, BLK, fillAA(abs(d) - 0.007, px));
    }

    // 4) Two pink accent balls at the crossbar tips — bouncing/pulsing.
    {
        vec3 c = paletteRotate(PINK, pShift);
        float bounceL = sin(t * 1.7) * 0.012;
        float bounceR = sin(t * 1.7 + 3.14159) * 0.012;
        float rr = 0.030 * (1.0 + 0.18 * kick + 0.06 * sin(t * 1.7));
        float dL = sdCircle(pr, vec2(-0.16, 0.24 + bounceL), rr);
        float dR = sdCircle(pr, vec2( 0.16, 0.24 + bounceR), rr);
        col = paint(col, c, fillAA(dL, px));
        col = paint(col, BLK, fillAA(abs(dL) - 0.005, px));
        col = paint(col, c, fillAA(dR, px));
        col = paint(col, BLK, fillAA(abs(dR) - 0.005, px));
    }
    return col;
}

// ════════════════════════════════════════════════════════════════════════
//   MAIN
// ════════════════════════════════════════════════════════════════════════
void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float ar = RENDERSIZE.x / RENDERSIZE.y;

    float t      = TIME;
    float pulse  = readPulse(t);
    float kick   = readKick(t);
    float drift  = clamp(compositionDrift, 0.0, 1.0);
    float pShift = clamp(paletteShift, 0.0, 1.0);

    int m = int(mood + 0.5);
    vec3 col;
    if      (m == 1) col = moodBacterio  (uv, ar, t, pulse, drift, kick);
    else if (m == 2) col = moodTreetops  (uv, ar, t, pulse, drift, pShift, kick);
    else if (m == 3) col = moodFirstChair(uv, ar, t, pulse, drift, pShift, kick);
    else             col = moodCarlton   (uv, ar, t, pulse, drift, pShift, kick);

    // Bloom lift — gentle linear-space lift to give print colour a hint of glow.
    // No tonemap. Output is LINEAR HDR for the host pipeline.
    float lift = clamp(bloomLift, 0.0, 1.0) * 0.18;
    col += col * lift;

    gl_FragColor = vec4(col, 1.0);
}
