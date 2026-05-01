/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Fauvism after Matisse — pure complementary fields painted in unmodulated Fauvist primaries. Animated Voronoi-cell regions, each filled with a single primary, soft brushwork inside, painterly edges between cells. After Open Window Collioure (1905), Green Line Portrait (1905), and Joy of Life (1906).",
  "INPUTS": [
    { "NAME": "cellCount",      "LABEL": "Cells",         "TYPE": "float", "MIN": 4.0,  "MAX": 32.0, "DEFAULT": 14.0 },
    { "NAME": "drift",          "LABEL": "Drift Speed",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.30 },
    { "NAME": "swirl",          "LABEL": "Swirl",         "TYPE": "float", "MIN": 0.0,  "MAX": 0.50, "DEFAULT": 0.18 },
    { "NAME": "swirlScale",     "LABEL": "Swirl Scale",   "TYPE": "float", "MIN": 0.5,  "MAX": 6.0,  "DEFAULT": 2.4 },
    { "NAME": "edgeBlur",       "LABEL": "Edge Blur",     "TYPE": "float", "MIN": 0.0,  "MAX": 0.30, "DEFAULT": 0.08 },
    { "NAME": "brushStrength",  "LABEL": "Brush Strength","TYPE": "float", "MIN": 0.0,  "MAX": 0.80, "DEFAULT": 0.32 },
    { "NAME": "brushScale",     "LABEL": "Brush Scale",   "TYPE": "float", "MIN": 4.0,  "MAX": 80.0, "DEFAULT": 22.0 },
    { "NAME": "complementary",  "LABEL": "Complementary", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.45 },
    { "NAME": "saturation",     "LABEL": "Saturation",    "TYPE": "float", "MIN": 0.4,  "MAX": 2.0,  "DEFAULT": 1.18 },
    { "NAME": "paletteShift",   "LABEL": "Palette Shift", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0 },
    { "NAME": "paperShow",      "LABEL": "Paper Show",    "TYPE": "float", "MIN": 0.0,  "MAX": 0.40, "DEFAULT": 0.10 },
    { "NAME": "audioReact",     "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "inputBleed",     "LABEL": "Input Bleed",   "TYPE": "float", "MIN": 0.0,  "MAX": 0.7,  "DEFAULT": 0.0 },
    { "NAME": "inputTex",       "LABEL": "Texture",       "TYPE": "image" }
  ]
}*/

// 9-stop Fauvist palette — Matisse + Derain + Vlaminck unmodulated primaries.
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

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
vec2 hash22(vec2 p) {
    return vec2(hash21(p), hash21(p + 17.3));
}
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
    int i = int(mod(idx, 9.0));
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

// Voronoi: returns the two closest seed distances + closest-cell color.
// Animated by adding a curl-noise displacement to seed positions so the
// painted regions migrate organically (TouchDesigner-style flow).
vec3 fauveVoronoi(vec2 uv, float aspect, float t) {
    // Choose grid density based on cellCount; nearest-9 search keeps cost stable.
    float n = clamp(cellCount, 4.0, 32.0);
    vec2 p = uv * vec2(aspect, 1.0) * sqrt(n);
    vec2 ip = floor(p);
    vec2 fp = fract(p);

    float bestD1 = 1e9, bestD2 = 1e9;
    vec3 bestColor = FAUVE0;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            vec2 cell = ip + vec2(float(i), float(j));
            vec2 jitter = hash22(cell);
            // Drift: each seed orbits inside its cell over time.
            jitter += swirl * vec2(sin(t * drift + cell.x * 1.7),
                                   cos(t * drift + cell.y * 2.3));
            vec2 seed = vec2(float(i), float(j)) + jitter - fp;
            float d = length(seed);
            if (d < bestD1) {
                bestD2 = bestD1;
                bestD1 = d;
                float idx = mod(cell.x * 3.7 + cell.y * 7.13 + paletteShift * 9.0, 9.0);
                bestColor = fauvePalette(idx);
            } else if (d < bestD2) {
                bestD2 = d;
            }
        }
    }
    // Pack edge distance into the color's alpha-equivalent via length encoding.
    // We return rgb only; caller derives edge from a separate call if needed.
    return bestColor;
}

// Edge metric: distance to nearest cell boundary (bestD2 - bestD1) — used
// for painterly edge softening.
float fauveVoronoiEdge(vec2 uv, float aspect, float t) {
    float n = clamp(cellCount, 4.0, 32.0);
    vec2 p = uv * vec2(aspect, 1.0) * sqrt(n);
    vec2 ip = floor(p);
    vec2 fp = fract(p);

    float bestD1 = 1e9, bestD2 = 1e9;
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            vec2 cell = ip + vec2(float(i), float(j));
            vec2 jitter = hash22(cell);
            jitter += swirl * vec2(sin(t * drift + cell.x * 1.7),
                                   cos(t * drift + cell.y * 2.3));
            vec2 seed = vec2(float(i), float(j)) + jitter - fp;
            float d = length(seed);
            if (d < bestD1)      { bestD2 = bestD1; bestD1 = d; }
            else if (d < bestD2) { bestD2 = d; }
        }
    }
    return bestD2 - bestD1;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    float t = TIME * (1.0 + audioMid * audioReact * 0.4);

    // Curl-noise displacement on the read coordinate — pigment swirls.
    float fb = fbm(uv * swirlScale + t * 0.10);
    vec2 disp = (vec2(fb, fbm(uv.yx * swirlScale + t * 0.13 + 5.0)) - 0.5)
              * swirl * 0.20;

    vec2 sUV = uv + disp;
    vec3 col = fauveVoronoi(sUV, aspect, t);
    float edge = fauveVoronoiEdge(sUV, aspect, t);

    // Painterly brushwork inside cells — anisotropic noise tilted by a
    // slow rotation. Adds visible brush direction without crossing the
    // cell boundaries.
    float brushAngle = sin(t * 0.05) * 1.2;
    float ca = cos(brushAngle), sa = sin(brushAngle);
    vec2 q = vec2(ca * uv.x + sa * uv.y, -sa * uv.x + ca * uv.y);
    q.y *= 3.5;
    float brush = fbm(q * brushScale);
    col *= 1.0 - brushStrength * 0.35 + brushStrength * 0.7 * brush;

    // Edge softening — within edgeBlur of a cell boundary, blend toward
    // the average of neighbours by sampling a slightly displaced point.
    if (edge < edgeBlur) {
        vec2 nudge = (hash22(floor(uv * 200.0)) - 0.5) * 0.005;
        vec3 neighbor = fauveVoronoi(sUV + nudge * 6.0, aspect, t);
        float blend = 1.0 - edge / max(edgeBlur, 1e-4);
        col = mix(col, neighbor, blend * 0.55);
    }

    // Black contour line where edge is very thin — Matisse "Green Line".
    float contour = smoothstep(0.0, 0.012, edge);
    col *= mix(0.20, 1.0, contour);

    // Optional input texture bleed — snaps to the closest Fauvist primary.
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

    // Complementary edge zing — bright pixels get pulled toward inverted
    // hue along boundaries, like the green line in the Matisse portrait.
    if (complementary > 0.0) {
        float compMix = (1.0 - smoothstep(0.0, 0.04, edge)) * complementary;
        col = mix(col, 1.0 - col, compMix * 0.4);
    }

    // Global saturation push — Fauvism wants pure unmodulated colour.
    col = saturateCol(col, saturation * (0.92 + audioBass * audioReact * 0.18));

    // Paper grain peeking through where pigment thins.
    float grain = (hash21(uv * RENDERSIZE) - 0.5) * 0.04;
    col = mix(col, col + grain, paperShow);
    col = mix(col, PAPER, paperShow * (1.0 - smoothstep(0.0, 0.02, edge)) * 0.10);

    // Audio breath
    col *= 0.90 + audioLevel * audioReact * 0.20;

    gl_FragColor = vec4(col, 1.0);
}
