/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Fauvism after Matisse — gaussian color-splat field, no cells/no Voronoi. N overlapping soft blobs of unmodulated Fauvist primaries blend organically; anisotropic brush strokes tilted along the painter's wrist direction; black contour lines pop where two strong colors meet (the Green Line). After Open Window Collioure (1905), Joy of Life (1906), Derain's Charing Cross Bridge.",
  "INPUTS": [
    { "NAME": "splatCount",     "LABEL": "Splat Count",   "TYPE":"float","MIN":4.0, "MAX":40.0, "DEFAULT":18.0 },
    { "NAME": "splatSize",      "LABEL": "Splat Size",    "TYPE":"float","MIN":0.05,"MAX":0.40, "DEFAULT":0.16 },
    { "NAME": "splatVariance",  "LABEL": "Size Variance", "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.65 },
    { "NAME": "drift",          "LABEL": "Drift Speed",   "TYPE":"float","MIN":0.0, "MAX":1.5,  "DEFAULT":0.30 },
    { "NAME": "swirlAmt",       "LABEL": "Swirl Amount",  "TYPE":"float","MIN":0.0, "MAX":0.40, "DEFAULT":0.12 },
    { "NAME": "swirlScale",     "LABEL": "Swirl Scale",   "TYPE":"float","MIN":0.5, "MAX":6.0,  "DEFAULT":2.4 },
    { "NAME": "brushStrokes",   "LABEL": "Brush Strokes", "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.42 },
    { "NAME": "brushScale",     "LABEL": "Brush Scale",   "TYPE":"float","MIN":4.0, "MAX":80.0, "DEFAULT":24.0 },
    { "NAME": "brushAnisotropy","LABEL": "Brush Aniso",   "TYPE":"float","MIN":1.0, "MAX":8.0,  "DEFAULT":3.5 },
    { "NAME": "wristAngle",     "LABEL": "Wrist Angle",   "TYPE":"float","MIN":0.0, "MAX":6.2832,"DEFAULT":0.7 },
    { "NAME": "contourStrength","LABEL": "Green Line",    "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.55 },
    { "NAME": "contourWidth",   "LABEL": "Line Width",    "TYPE":"float","MIN":0.001,"MAX":0.020,"DEFAULT":0.006 },
    { "NAME": "saturation",     "LABEL": "Saturation",    "TYPE":"float","MIN":0.4, "MAX":2.0,  "DEFAULT":1.20 },
    { "NAME": "complementary",  "LABEL": "Complementary", "TYPE":"float","MIN":0.0, "MAX":0.40, "DEFAULT":0.10 },
    { "NAME": "paletteShift",   "LABEL": "Palette Shift", "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.0 },
    { "NAME": "paperShow",      "LABEL": "Paper Show",    "TYPE":"float","MIN":0.0, "MAX":0.50, "DEFAULT":0.12 },
    { "NAME": "audioReact",     "LABEL": "Audio React",   "TYPE":"float","MIN":0.0, "MAX":2.0,  "DEFAULT":1.0 },
    { "NAME": "inputBleed",     "LABEL": "Input Bleed",   "TYPE":"float","MIN":0.0, "MAX":0.7,  "DEFAULT":0.0 },
    { "NAME": "inputTex",       "LABEL": "Texture",       "TYPE":"image" }
  ]
}*/

// ── Fauvist palette: 9 unmodulated primaries ──────────────────────────
const vec3 FAUVE0 = vec3(0.96, 0.32, 0.16);  // vermilion
const vec3 FAUVE1 = vec3(0.97, 0.83, 0.20);  // lemon
const vec3 FAUVE2 = vec3(0.30, 0.66, 0.32);  // sap green
const vec3 FAUVE3 = vec3(0.10, 0.32, 0.78);  // cobalt
const vec3 FAUVE4 = vec3(0.94, 0.30, 0.55);  // hot pink
const vec3 FAUVE5 = vec3(0.62, 0.34, 0.68);  // mauve
const vec3 FAUVE6 = vec3(0.95, 0.55, 0.10);  // cadmium orange
const vec3 FAUVE7 = vec3(0.18, 0.55, 0.65);  // viridian
const vec3 FAUVE8 = vec3(0.85, 0.10, 0.25);  // alizarin
const vec3 PAPER  = vec3(0.96, 0.93, 0.85);

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) { v += vnoise(p) * a; p *= 2.07; a *= 0.5; }
    return v;
}
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

// ──────────────────────────────────────────────────────────────────────
// Anisotropic brush stroke noise — fbm in a sheared, rotated frame so
// the result reads as paint strokes pulled along the wrist direction.
// ──────────────────────────────────────────────────────────────────────
float strokeNoise(vec2 p, float angle, float aniso) {
    float ca = cos(angle), sa = sin(angle);
    vec2 q = vec2(ca * p.x + sa * p.y, -sa * p.x + ca * p.y);
    q.y *= aniso;
    return fbm(q);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 cuv = uv * vec2(aspect, 1.0);
    float t = TIME * (1.0 + audioMid * audioReact * 0.3);

    // Background paper — slight grain so it never reads as flat fill
    vec3 col = PAPER * (0.93 + 0.07 * vnoise(uv * 320.0));

    // ── Splat field accumulation ─────────────────────────────────────
    // Gaussian blobs of saturated color with weighted blending. Unlike
    // Voronoi, splats overlap softly and fight for dominance based on
    // proximity, with no abrupt cell boundaries.
    int N = int(clamp(splatCount, 1.0, 40.0));
    vec3 accumColor = vec3(0.0);
    float accumWeight = 0.0;
    // Track second-best splat so we can compute the contour where two
    // strong colors meet (Matisse's "Green Line")
    vec3 bestColor = FAUVE0;
    vec3 secondBestColor = FAUVE1;
    float bestWeight = 0.0;
    float secondBestWeight = 0.0;

    for (int i = 0; i < 40; i++) {
        if (i >= N) break;
        float fi = float(i);
        // Splat home position with slow drift
        vec2 home = vec2(hash11(fi * 7.13), hash11(fi * 11.7)) * vec2(aspect, 1.0);
        vec2 wobble = vec2(sin(t * drift + fi * 1.7), cos(t * drift * 0.8 + fi * 1.3)) * 0.06;
        vec2 ctr = home + wobble;

        // Per-splat radius variance
        float sz = splatSize * mix(0.5, 1.7, hash11(fi * 13.3) * splatVariance + (1.0 - splatVariance) * 0.5);
        sz *= 1.0 + audioBass * audioReact * 0.10;

        // Curl-noise displacement of the SAMPLE coord — splats become
        // organic blobs instead of perfect disks (no Voronoi here, the
        // blobs warp with curl noise).
        vec2 disp = (vec2(fbm(cuv * swirlScale + fi),
                          fbm(cuv.yx * swirlScale + fi + 5.0)) - 0.5) * swirlAmt;
        vec2 d = (cuv + disp) - ctr;

        // Gaussian falloff
        float r2 = dot(d, d);
        float w = exp(-r2 / (sz * sz));

        // Color
        float pidx = mod(fi * 2.31 + floor(t * 0.04), 9.0);
        vec3 splatCol = fauvePalette(pidx);

        accumColor += splatCol * w;
        accumWeight += w;

        // Track best/second-best for contour line detection
        if (w > bestWeight) {
            secondBestWeight = bestWeight;
            secondBestColor  = bestColor;
            bestWeight = w;
            bestColor  = splatCol;
        } else if (w > secondBestWeight) {
            secondBestWeight = w;
            secondBestColor  = splatCol;
        }
    }

    // Composite splat color on paper, weighted by total presence
    vec3 splatField = accumColor / max(accumWeight, 1e-4);
    float coverage = smoothstep(0.0, 1.0, accumWeight);
    col = mix(col, splatField, coverage);

    // ── Anisotropic brush strokes inside the splat field ─────────────
    if (brushStrokes > 0.0) {
        float bn = strokeNoise(cuv * brushScale, wristAngle, brushAnisotropy);
        float bn2 = strokeNoise(cuv * brushScale * 1.7, wristAngle + 0.3, brushAnisotropy * 0.7);
        float stroke = mix(bn, bn2, 0.4);
        // Strokes lighten / darken the paint asymmetrically — the brush
        // catches more pigment on one side of each stroke.
        col *= 1.0 - brushStrokes * 0.30 + brushStrokes * 0.55 * stroke;
    }

    // ── Green Line contour where two strong splats meet ──────────────
    if (contourStrength > 0.0) {
        // Boundary metric: how close the two top weights are. When
        // they're equal, we're on a color boundary.
        float bDiff = abs(bestWeight - secondBestWeight) / max(bestWeight + secondBestWeight, 1e-4);
        float onLine = smoothstep(contourWidth * 80.0, 0.0, bDiff)
                     * smoothstep(0.10, 0.40, accumWeight);
        col = mix(col, vec3(0.06, 0.18, 0.10), onLine * contourStrength);
    }

    // ── Optional input texture bleed (snapped to nearest Fauvist) ────
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

    // ── Complementary edge zing (subtle — Matisse's vibrate) ─────────
    if (complementary > 0.0) {
        // Detect chromatic edges via local fbm derivative
        float Lc = dot(col, vec3(0.299, 0.587, 0.114));
        float ex = abs(dFdx(Lc)) + abs(dFdy(Lc));
        col = mix(col, 1.0 - col, smoothstep(0.05, 0.20, ex) * complementary);
    }

    // Saturation boost + paper peek-through
    col = saturateCol(col, saturation * (0.92 + audioBass * audioReact * 0.18));
    if (paperShow > 0.0) {
        float papMask = (1.0 - smoothstep(0.05, 0.30, accumWeight));
        col = mix(col, PAPER, papMask * paperShow);
    }

    // Audio breath
    col *= 0.92 + audioLevel * audioReact * 0.15;

    gl_FragColor = vec4(col, 1.0);
}
