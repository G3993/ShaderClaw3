/*{
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Standalone abstract expressionist oil-paint generator — after Cy Twombly's gestural calligraphy and Franz Kline's bold ink/paint slashes. Domain-warped FBM blob fields churn like thick paint strokes on raw linen. Deep black ground, warm ivory gestures, cadmium red accent, cobalt blue, golden ochre — classic oil-paint palette. TIME-driven liveness even at audio=0. Linear HDR output; host applies ACES.",
  "CREDIT": "ShaderClaw",
  "INPUTS": [
    { "NAME": "strokeScale",   "LABEL": "Stroke Scale",    "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3, "MAX": 3.0 },
    { "NAME": "churnSpeed",    "LABEL": "Churn Speed",     "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "strokeWidth",   "LABEL": "Stroke Width",    "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.1, "MAX": 1.0 },
    { "NAME": "impastoLift",   "LABEL": "Impasto Lift",    "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "paletteMode",   "LABEL": "Palette",         "TYPE": "long",  "DEFAULT": 0,
      "VALUES": [0, 1, 2, 3],
      "LABELS": ["Kline Black+Ivory", "Twombly Ochre+Red", "Cobalt+Cream", "Night Palette"] },
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// ─── Palette ──────────────────────────────────────────────────────────────────
// Five canonical oil-paint pigments. Franz Kline: black + ivory on raw canvas.
// Cy Twombly: ochre, cadmium red scrawl on warm ivory ground.
// Cobalt palette: deep cobalt + lead white, cool and luminous.
// Night palette: near-black ground, cobalt accent, muted ochre.

const vec3 C_BLACK  = vec3(0.040, 0.035, 0.032);   // Ivory Black (deep)
const vec3 C_IVORY  = vec3(0.940, 0.920, 0.870);   // Cremnitz White / raw linen
const vec3 C_RED    = vec3(0.820, 0.165, 0.110);   // Cadmium Red
const vec3 C_COBALT = vec3(0.095, 0.195, 0.620);   // Cobalt Blue
const vec3 C_OCHRE  = vec3(0.680, 0.490, 0.140);   // Yellow Ochre / Gold

// ─── Hashing / noise primitives ───────────────────────────────────────────────

float hash12(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// Smooth value noise
float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash12(i);
    float b = hash12(i + vec2(1.0, 0.0));
    float c = hash12(i + vec2(0.0, 1.0));
    float d = hash12(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// 5-octave FBM — lacunarity 2.1, gain 0.48 give slightly irregular fractal
// character avoiding the "computer noise" look. A per-octave rotation breaks
// axis-aligned banding — crucial for gestural, directional strokes.
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    vec2  shift = vec2(100.0);
    mat2  rot   = mat2(cos(0.5), sin(0.5), -sin(0.5), cos(0.5));
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p  = rot * p * 2.1 + shift;
        a *= 0.48;
    }
    return v;
}

// Domain-warped FBM — q is computed once and reused so the second FBM call
// uses the first as a spatial offset. This folds the field back on itself,
// producing the layered, impacted topology of wet paint-on-paint.
float warpedFbm(vec2 p, float warpAmt) {
    vec2 q = vec2(fbm(p + vec2(0.00, 0.00)),
                  fbm(p + vec2(5.20, 1.30)));
    vec2 r = vec2(fbm(p + warpAmt * q + vec2(1.70, 9.20)),
                  fbm(p + warpAmt * q + vec2(8.30, 2.80)));
    return fbm(p + warpAmt * r);
}

// ─── Stroke coverage field ────────────────────────────────────────────────────
// Returns [0,1] coverage for one "gesture layer".
// Uses fwidth-based AA on the threshold boundary so the stroke edge is never
// a hard 1-pixel alias, regardless of zoom level or display DPI.
float strokeField(vec2 p, float seed, float threshold, float edgeSharp) {
    float n   = warpedFbm(p + seed, 0.9);
    float raw = n - threshold;
    float fw  = fwidth(raw) * edgeSharp;
    return smoothstep(-fw, fw, raw);
}

// ─── Directional stroke bias ──────────────────────────────────────────────────
// Kline and de Kooning paint in one dominant direction — horizontal or diagonal.
// Compress the sampling UV by different factors on X vs Y to create anisotropic
// strokes (wide but not tall, or the reverse). dirBias in [-1,1].
vec2 strikeBias(vec2 p, float dirBias) {
    // dirBias > 0 → horizontal strokes (X compressed); < 0 → vertical
    float bx = 1.0 + dirBias * 0.55;
    float by = 1.0 - dirBias * 0.55;
    return vec2(p.x * bx, p.y * by);
}

// ─── Surface relief gradient (impasto normal estimation) ──────────────────────
// Central-difference of the warped FBM field gives a height gradient.
// The caller builds a pseudo-normal from (grad, 1.0) for raking-light shading.
vec2 heightGrad(vec2 uv, float warpSeed, float scale) {
    float ep = 1.5 / max(RENDERSIZE.x, RENDERSIZE.y);
    vec2 ex  = vec2(ep, 0.0);
    float hL = warpedFbm((uv - ex) * scale + warpSeed, 0.85);
    float hR = warpedFbm((uv + ex) * scale + warpSeed, 0.85);
    float hD = warpedFbm((uv - ex.yx) * scale + warpSeed, 0.85);
    float hU = warpedFbm((uv + ex.yx) * scale + warpSeed, 0.85);
    return vec2(hR - hL, hU - hD);
}

// ─── Main ─────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Aspect-correct coordinate centred at 0.5 so strokes don't stretch.
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 p = uv;
    p.x *= aspect;

    // ── Audio modulator ──────────────────────────────────────────────────
    // Pattern: (0.5 + 0.5 * audioBass * audioReact).
    // At silence (audioBass=0) → 0.5 → all layers still alive and beautiful.
    // At full bass → up to 1.0 + audioReact → strokes expand, highlights peak.
    float audioMod = 0.5 + 0.5 * audioBass * audioReact;

    // ── TIME-driven churn (alive at audio=0) ─────────────────────────────
    // Two independent phase offsets drift X and Y at different frequencies.
    // Result: a slow breathing, churning motion that never settles, even when
    // the audio feed is silent.
    float t = TIME * churnSpeed;
    vec2 drift = vec2(
        0.14 * sin(t * 0.71) + 0.08 * cos(t * 1.17),
        0.11 * cos(t * 0.63) + 0.09 * sin(t * 0.93)
    );
    // Secondary slow rotation: entire composition slowly pivots, like a
    // canvas being turned by the artist to examine from a fresh angle.
    float angle  = sin(t * 0.19) * 0.06;
    float ca = cos(angle), sa = sin(angle);
    mat2  slowRot = mat2(ca, -sa, sa, ca);

    // Base sampling coordinate — scaled, biased for anisotropy, drifting.
    float sc   = strokeScale * 1.6;
    vec2  base = slowRot * (p * sc) + drift;

    // Stroke width modulated by audio and user slider.
    float sw = strokeWidth * audioMod;

    // ── Palette selection ────────────────────────────────────────────────
    vec3 ground, c1, c2, c3, c4, c5;
    int pm = int(paletteMode);
    if (pm == 1) {
        // Twombly: warm ivory ground, golden ochre, cadmium red scrawl
        ground = vec3(0.900, 0.870, 0.780);
        c1 = C_OCHRE;
        c2 = C_RED;
        c3 = C_BLACK;
        c4 = vec3(0.760, 0.580, 0.210);  // raw sienna
        c5 = vec3(0.550, 0.330, 0.090);  // burnt umber
    } else if (pm == 2) {
        // Cobalt + cream: cool luminous palette, de Kooning Woman series
        ground = vec3(0.880, 0.860, 0.820);
        c1 = C_COBALT;
        c2 = C_IVORY;
        c3 = vec3(0.120, 0.100, 0.380);  // ultramarine shadow
        c4 = C_OCHRE;
        c5 = C_RED * 0.75;
    } else if (pm == 3) {
        // Night: near-black ground, cobalt + muted ochre — Kline's late work
        ground = vec3(0.025, 0.022, 0.020);
        c1 = C_COBALT;
        c2 = C_OCHRE * 0.80;
        c3 = C_IVORY * 0.55;
        c4 = C_RED * 0.60;
        c5 = vec3(0.160, 0.080, 0.290);  // dioxazine violet
    } else {
        // Kline (default): deep black ground, bold ivory gestures,
        // ochre and red counter-accents — after "Mahoning" (1956).
        ground = C_BLACK;
        c1 = C_IVORY;
        c2 = C_OCHRE;
        c3 = C_RED;
        c4 = C_COBALT;
        c5 = vec3(0.820, 0.790, 0.730);  // aged lead white
    }

    // ── Stroke layers ────────────────────────────────────────────────────
    // Each layer samples domain-warped FBM at a different scale and phase,
    // thresholded to a coverage mask. Overlapping them produces the all-over
    // compositional density of abstract expressionism.
    // The threshold is audio-modulated so louder bass = broader strokes.

    float baseTh = 0.50 - sw * 0.18;

    // Layer 1 — primary gesture (c1): large, slow horizontal sweeps.
    // Anisotropic bias (dirBias=0.7) produces wide flat strokes like Kline's
    // house-painting brush.
    float l1 = strokeField(strikeBias(base * 0.68, 0.70),
                            0.00, baseTh, 1.6);

    // Layer 2 — secondary gesture (c2): mid-scale diagonal marks.
    float l2 = strokeField(strikeBias(base * 1.05 + vec2(3.70, 1.10), -0.30),
                            17.3, baseTh + 0.04, 1.5);

    // Layer 3 — accent (c3): tight energetic marks at higher frequency.
    // Kline's counterpoint strokes — short, abrupt, decisive.
    float l3 = strokeField(base * 1.60 + vec2(7.20, 5.80),
                            33.1, baseTh + 0.10, 1.4);

    // Layer 4 — deep underpaint (c4): broad slow zones, partially submerged.
    // The paint scraped back or the first rough lay-in that shows through.
    float l4 = strokeField(strikeBias(base * 0.42 + vec2(1.50, 8.90), 0.55),
                            51.7, baseTh - 0.07, 1.8);

    // Layer 5 — fifth color, small turbulent gestures (c5): Twombly-style
    // graffiti marks — the shortest, most energetic, last applied.
    float l5 = strokeField(base * 2.30 + vec2(12.4, 3.10),
                            74.9, baseTh + 0.16, 1.3);

    // ── Composite: back to front ─────────────────────────────────────────
    // Blending with mix() keeps colours in oil-paint gamut at this stage.
    // HDR values come later from impasto and gloss — not from additive blends.
    vec3 col = ground;
    col = mix(col, c4, l4 * 0.70);            // deep underpaint, semi-opaque
    col = mix(col, c1, l1);                   // primary gesture, fully opaque
    col = mix(col, c2, l2 * 0.85);           // secondary gesture
    col = mix(col, c3, l3 * 0.68);           // accent marks
    col = mix(col, c5, l5 * 0.50);           // fifth color, light touch

    // ── Impasto relief shading (raking light) ────────────────────────────
    // The height gradient of the FBM field acts as a canvas-relief normal.
    // A directional key light from upper-left rakes across it, as in a
    // gallery lit from a high oblique source. This creates the 3D illusion
    // of thick paint ridges (raised) and deep channels between strokes.
    if (impastoLift > 0.001) {
        vec2  g     = heightGrad(p, 0.0, sc * 0.68);
        vec3  norm  = normalize(vec3(g * 6.0, 1.0));
        vec3  light = normalize(vec3(-0.55, 0.70, 0.45));

        float diff  = clamp(dot(norm, light), 0.0, 1.0);

        // Specular: Blinn-Phong tight highlight on paint ridges.
        vec3  halfv = normalize(light + vec3(0.0, 0.0, 1.0));
        float spec  = pow(clamp(dot(norm, halfv), 0.0, 1.0), 32.0);

        // Modulate diffuse: shadows deepen channels, highlights lift ridges.
        col *= 0.80 + 0.30 * diff;

        // HDR specular highlight — lifted to 1.5–2.0 so ACES tonemapper
        // produces convincing wet-paint glisten. Never clamped here.
        float specIntensity = impastoLift * (1.0 + audioMod * 0.55);
        col += vec3(1.10, 1.05, 0.95) * spec * specIntensity * 1.60;
    }

    // ── Wet-paint gloss ───────────────────────────────────────────────────
    // Where primary strokes are densest and brightest, add a broad HDR
    // wet gloss — as if varnish is still wet. Values lifted above 1.0
    // feed the ACES bloom/highlight roll-off for a convincing oil-paint sheen.
    float strokeLum = dot(col, vec3(0.299, 0.587, 0.114));
    float wetGloss  = smoothstep(0.52, 0.90, strokeLum)
                    * l1                            // only on primary layer
                    * (0.75 + audioMod * 0.50);    // audio lifts gloss
    col += C_IVORY * wetGloss * 1.85;              // HDR: values go to ~2.0

    // ── Gesture velocity trails (Twombly scribble energy) ─────────────────
    // High-frequency FBM at a diagonal adds calligraphic energy — thin,
    // near-random marks that read as the painter's hand moving fast across
    // the canvas. TIME-animated so they shift even at silence.
    float scribN = warpedFbm(base * 3.90 + vec2(sin(t * 0.38) * 0.35,
                                                  cos(t * 0.52) * 0.28), 0.55);
    // fwidth-based AA on the scribble edge — no aliasing at any DPI.
    float scThresh = 0.645;
    float scFw     = fwidth(scribN - scThresh) * 1.3;
    float scribMask = smoothstep(scThresh - scFw, scThresh + scFw, scribN);
    vec3  scribColor = mix(c3, C_IVORY, 0.42);
    col = mix(col, scribColor * 1.35, scribMask * 0.36);

    // ── Canvas weave texture ──────────────────────────────────────────────
    // Two orthogonal sine waves simulate coarse linen canvas threads. The
    // weave reads only where paint is thin — absent under thick impasto.
    float canvasX = sin(gl_FragCoord.x * 2.30) * 0.5 + 0.5;
    float canvasY = sin(gl_FragCoord.y * 2.30) * 0.5 + 0.5;
    float weave   = canvasX * canvasY * 0.026;
    col += weave * (1.0 - l1 * 0.72);             // canvas shows through thin paint

    // ── Vignette ──────────────────────────────────────────────────────────
    // Darkens corners and edges to frame the composition — standard for
    // gallery-lit paintings viewed at distance.
    vec2 vigUV = uv - 0.5;
    float vig  = 1.0 - dot(vigUV * vec2(1.0, 1.2), vigUV * vec2(1.0, 1.2)) * 2.1;
    vig = clamp(vig, 0.0, 1.0);
    vig = pow(vig, 0.50);
    col *= vig;

    // ── Linear HDR output ─────────────────────────────────────────────────
    // NO gamma encode. NO ACES. NO clamp.
    // Specular and gloss peaks above 1.0 are intentional — they feed the
    // host pipeline's ACES tonemapper and bloom pass.
    gl_FragColor = vec4(col, 1.0);
}
