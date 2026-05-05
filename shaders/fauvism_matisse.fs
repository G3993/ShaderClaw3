/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Fauvism after Matisse — accumulating brush-stroke field. N curved colored capsules layer on top of each other, each pulled along the painter's wrist direction. No cells, no Voronoi, no closest-wins. Just impasto paint laid on impasto paint. Pure Fauvist primaries.",
  "INPUTS": [
    { "NAME": "strokeCount",    "LABEL": "Stroke Count",  "TYPE":"float","MIN":10.0,"MAX":80.0, "DEFAULT":48.0 },
    { "NAME": "strokeLength",   "LABEL": "Stroke Length", "TYPE":"float","MIN":0.05,"MAX":0.50, "DEFAULT":0.18 },
    { "NAME": "strokeWidth",    "LABEL": "Stroke Width",  "TYPE":"float","MIN":0.005,"MAX":0.06,"DEFAULT":0.020 },
    { "NAME": "strokeCurve",    "LABEL": "Stroke Curve",  "TYPE":"float","MIN":0.0, "MAX":0.30, "DEFAULT":0.10 },
    { "NAME": "lengthVariance","LABEL": "Length Variance","TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.55 },
    { "NAME": "drift",          "LABEL": "Drift Speed",   "TYPE":"float","MIN":0.0, "MAX":1.5,  "DEFAULT":0.20 },
    { "NAME": "wristAngle",     "LABEL": "Wrist Angle",   "TYPE":"float","MIN":0.0, "MAX":6.2832,"DEFAULT":0.7 },
    { "NAME": "wristSpread",    "LABEL": "Wrist Spread",  "TYPE":"float","MIN":0.0, "MAX":3.14, "DEFAULT":0.9 },
    { "NAME": "strokeOpacity",  "LABEL": "Stroke Opacity","TYPE":"float","MIN":0.30,"MAX":1.0,  "DEFAULT":0.85 },
    { "NAME": "edgeSoftness",   "LABEL": "Edge Softness", "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.45 },
    { "NAME": "canvasGrain",    "LABEL": "Canvas Grain",  "TYPE":"float","MIN":0.0, "MAX":0.30, "DEFAULT":0.10 },
    { "NAME": "saturation",     "LABEL": "Saturation",    "TYPE":"float","MIN":0.4, "MAX":2.0,  "DEFAULT":1.20 },
    { "NAME": "paletteShift",   "LABEL": "Palette Shift", "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.0 },
    { "NAME": "audioReact",     "LABEL": "Audio React",   "TYPE":"float","MIN":0.0, "MAX":2.0,  "DEFAULT":1.0 },
    { "NAME": "inputBleed",     "LABEL": "Input Bleed",   "TYPE":"float","MIN":0.0, "MAX":0.7,  "DEFAULT":0.0 },
    { "NAME": "inputTex",       "LABEL": "Texture",       "TYPE":"image" }
  ]
}*/

// 9-stop Fauvist palette
const vec3 FAUVE0 = vec3(0.96, 0.32, 0.16);  // vermilion
const vec3 FAUVE1 = vec3(0.97, 0.83, 0.20);  // lemon
const vec3 FAUVE2 = vec3(0.30, 0.66, 0.32);  // sap green
const vec3 FAUVE3 = vec3(0.10, 0.32, 0.78);  // cobalt
const vec3 FAUVE4 = vec3(0.94, 0.30, 0.55);  // hot pink
const vec3 FAUVE5 = vec3(0.62, 0.34, 0.68);  // mauve
const vec3 FAUVE6 = vec3(0.95, 0.55, 0.10);  // cadmium orange
const vec3 FAUVE7 = vec3(0.18, 0.55, 0.65);  // viridian
const vec3 FAUVE8 = vec3(0.85, 0.10, 0.25);  // alizarin
const vec3 CANVAS = vec3(0.94, 0.90, 0.78);

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

vec3 fauvePalette(float idx) {
    int i = int(mod(idx + paletteShift * 9.0, 9.0));
    if (i == 0) return FAUVE0;
    if (i == 1) return FAUVE1;
    if (i == 2) return FAUVE2;
    if (i == 3) return FAUVE3;
    if (i == 4) return FAUVE4;
    if (i == 5) return FAUVE5;
    if (i == 6) return FAUVE6;
    if (i == 7) return FAUVE7;
    return FAUVE8;
}
vec3 saturateCol(vec3 c, float a) {
    float L = dot(c, vec3(0.299, 0.587, 0.114));
    return mix(vec3(L), c, a);
}

// Distance to a quadratic bezier — used to give strokes a gentle curve.
// Approximated by sampling 5 points along the curve, returning min dist.
float distToCurvedStroke(vec2 p, vec2 a, vec2 b, vec2 c) {
    float minD = 1e9;
    // Sample 6 points along bezier from a to c, with control point b
    for (int i = 0; i <= 5; i++) {
        float t = float(i) / 5.0;
        vec2 q = mix(mix(a, b, t), mix(b, c, t), t);
        float d = length(p - q);
        if (d < minD) minD = d;
    }
    return minD;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 cuv = uv * vec2(aspect, 1.0);
    float t = TIME * (1.0 + audioMid * audioReact * 0.3);

    // Start with raw canvas — strokes paint ON TOP.
    vec3 col = CANVAS * (0.92 + 0.08 * hash21(uv * RENDERSIZE.x * 0.3) * canvasGrain * 10.0);

    // ── Stroke accumulation pass ─────────────────────────────────────
    // Each stroke is a curved bezier capsule. We layer strokes onto the
    // canvas in deterministic order — later strokes paint over earlier
    // ones (no closest-wins; this is impasto, not Voronoi).
    int N = int(clamp(strokeCount, 1.0, 80.0));
    for (int i = 0; i < 80; i++) {
        if (i >= N) break;
        float fi = float(i);

        // Stroke center — random position with slow drift
        vec2 home = vec2(hash11(fi * 7.13), hash11(fi * 11.7));
        home += vec2(sin(t * drift + fi * 1.7),
                     cos(t * drift * 0.7 + fi * 2.3)) * 0.06;
        vec2 center = home * vec2(aspect, 1.0);

        // Stroke direction — wrist angle + per-stroke spread
        float angle = wristAngle + (hash11(fi * 13.3) - 0.5) * wristSpread;
        // Slight angle jitter over time
        angle += sin(t * 0.05 + fi * 0.7) * 0.15;
        vec2 dir = vec2(cos(angle), sin(angle));
        vec2 perp = vec2(-dir.y, dir.x);

        // Stroke length
        float len = strokeLength * mix(0.5, 1.5, hash11(fi * 17.9) * lengthVariance + (1.0 - lengthVariance) * 0.5);
        len *= 1.0 + audioBass * audioReact * 0.10;

        // Bezier control points — start, curved-mid, end
        vec2 a = center - dir * len * 0.5;
        vec2 c = center + dir * len * 0.5;
        // Curved control point — pushes off the line by strokeCurve
        float bend = (hash11(fi * 19.7) - 0.5) * 2.0 * strokeCurve;
        vec2 b = center + perp * bend * len;

        // Distance from this fragment to the stroke
        float d = distToCurvedStroke(cuv, a, b, c);

        // Stroke width with per-stroke variance + audio modulation
        float w = strokeWidth * (0.7 + hash11(fi * 23.1) * 0.6);

        // Mask — soft tapered edges with edgeSoftness slider
        float core = 1.0 - smoothstep(w * 0.4, w, d);
        float fringe = 1.0 - smoothstep(w, w * (1.0 + edgeSoftness * 1.5), d);
        float mask = mix(core, fringe, edgeSoftness);

        if (mask < 0.001) continue;

        // Stroke color
        float pidx = mod(fi * 2.31 + floor(t * 0.03), 9.0);
        vec3 strokeCol = fauvePalette(pidx);

        // Per-stroke pigment density variation — paint loaded on the
        // brush isn't uniform; some patches are denser
        float density = 0.65 + 0.35 * hash21(floor(cuv * 80.0) + fi);
        strokeCol *= density;

        // Composite — straight alpha over (impasto stacking)
        col = mix(col, strokeCol, mask * strokeOpacity);
    }

    // ── Optional input texture bleed ─────────────────────────────────
    if (IMG_SIZE_inputTex.x > 0.0 && inputBleed > 0.0) {
        vec3 src = texture(inputTex, uv).rgb;
        vec3 best = FAUVE0; float bd = 1e9;
        for (int k = 0; k < 9; k++) {
            vec3 cand = fauvePalette(float(k));
            float dd = dot(src - cand, src - cand);
            if (dd < bd) { bd = dd; best = cand; }
        }
        col = mix(col, best, inputBleed);
    }

    // Saturation push — Fauvism wants pure unmodulated color
    col = saturateCol(col, saturation * (0.92 + audioBass * audioReact * 0.18));

    // Audio breath
    col *= 0.92 + audioLevel * audioReact * 0.15;

    gl_FragColor = vec4(col, 1.0);
}
