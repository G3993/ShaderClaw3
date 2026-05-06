/*{
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "DESCRIPTION": "Mondrian's Broadway Boogie Woogie — orthogonal lanes carrying marching coloured pulses, bright primary squares glowing at intersections.",
  "INPUTS": [
    { "NAME": "audioReact",    "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "pulseSpeed",    "LABEL": "Pulse Speed",    "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "gridDensity",   "LABEL": "Grid Density",   "TYPE": "long",  "VALUES": [0, 1, 2], "LABELS": ["8", "12", "16"], "DEFAULT": 1 },
    { "NAME": "glowIntensity", "LABEL": "Glow Intensity", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  SQUARES — Mondrian's Broadway Boogie Woogie (1942–43)
//  Orthogonal H+V lanes on cream canvas, small coloured squares pulsing
//  along each lane at different speeds, bright primary squares glowing
//  at intersections. LINEAR HDR output (HDR peaks ~1.6 for bloom).
//  Five-colour palette only: cream, black, red, blue, yellow.
// ════════════════════════════════════════════════════════════════════════

// ─── Mondrian palette ────────────────────────────────────────────────
const vec3 PAL_CREAM  = vec3(0.96, 0.94, 0.88);
const vec3 PAL_BLACK  = vec3(0.04, 0.04, 0.06);
const vec3 PAL_RED    = vec3(0.85, 0.15, 0.10);
const vec3 PAL_BLUE   = vec3(0.10, 0.20, 0.65);
const vec3 PAL_YELLOW = vec3(0.96, 0.86, 0.20);

// ─── hashes ──────────────────────────────────────────────────────────
float h11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float h12(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// pick a primary palette colour (red/blue/yellow) from an index
vec3 pickPrimary(float k) {
    float r = fract(k);
    if (r < 0.34) return PAL_RED;
    if (r < 0.67) return PAL_BLUE;
    return PAL_YELLOW;
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float t = TIME * clamp(pulseSpeed, 0.1, 3.0);

    float aR = clamp(audioReact, 0.0, 2.0);
    float gI = clamp(glowIntensity, 0.0, 2.0);

    // gridDensity enum: 0->8, 1->12, 2->16 lanes per axis
    int dIdx = int(clamp(gridDensity, 0.0, 2.0) + 0.5);
    float N = (dIdx == 0) ? 8.0 : (dIdx == 1) ? 12.0 : 16.0;

    // pseudo-bass driver (works in silence too) — audioBass is a uniform
    float bassDrive = clamp(audioBass + 0.35 + 0.35 * sin(TIME * 1.7), 0.0, 1.5);
    float bass = bassDrive * aR;

    // ─── canvas: cream ────────────────────────────────────────────────
    vec3 col = PAL_CREAM;

    // ─── lane geometry ────────────────────────────────────────────────
    // Black lane lines (fixed grid). Lane half-width in normalized coords.
    float laneW = 0.012;          // black lane stripe width
    float pulseR = 0.018;         // marching square half-size
    float glowR  = 0.040;         // intersection glow radius

    vec2 g = uv * N;              // grid coords
    vec2 gi = floor(g);           // cell id
    vec2 gf = fract(g);           // 0..1 within cell

    // distance to nearest vertical lane line (between cells)
    float dV = min(gf.x, 1.0 - gf.x) / N;
    // distance to nearest horizontal lane line
    float dH = min(gf.y, 1.0 - gf.y) / N;

    // black lane stripes (V + H)
    float laneV = 1.0 - smoothstep(laneW * 0.5, laneW, dV);
    float laneH = 1.0 - smoothstep(laneW * 0.5, laneW, dH);
    float laneMask = max(laneV, laneH);
    col = mix(col, PAL_BLACK, laneMask);

    // ─── marching pulses along VERTICAL lanes (move in Y) ────────────
    // Each vertical lane sits at x = k/N for k = 1..N-1.
    // We pick the nearest lane to the current pixel.
    float kV = floor(uv.x * N + 0.5);          // nearest vertical lane index
    float laneXV = kV / N;
    float distXV = abs(uv.x - laneXV);
    if (distXV < laneW * 1.6 && kV > 0.5 && kV < N - 0.5) {
        float salt = h11(kV * 1.31 + 3.0);
        float spd  = 0.10 + 0.20 * salt;       // per-lane speed
        // pulse spawn period along the lane in [0,1] space
        float per  = 0.16 + 0.10 * h11(kV * 2.7);
        float along = mod(uv.y - t * spd, per);
        // distance to nearest pulse center along lane
        float dAlong = min(along, per - along);
        float dPerp  = distXV;
        float d = max(dAlong, dPerp);
        if (d < pulseR) {
            float kCol = h11(kV * 5.13 + floor((uv.y - t * spd) / per) * 0.71);
            vec3 pc = pickPrimary(kCol);
            col = pc;
        }
    }

    // ─── marching pulses along HORIZONTAL lanes (move in X) ──────────
    float kH = floor(uv.y * N + 0.5);
    float laneYH = kH / N;
    float distYH = abs(uv.y - laneYH);
    if (distYH < laneW * 1.6 && kH > 0.5 && kH < N - 0.5) {
        float salt = h11(kH * 1.91 + 17.0);
        float spd  = 0.10 + 0.20 * salt;
        float per  = 0.16 + 0.10 * h11(kH * 3.3);
        float along = mod(uv.x + t * spd, per);
        float dAlong = min(along, per - along);
        float dPerp  = distYH;
        float d = max(dAlong, dPerp);
        if (d < pulseR) {
            float kCol = h11(kH * 7.19 + floor((uv.x + t * spd) / per) * 0.83);
            vec3 pc = pickPrimary(kCol);
            col = pc;
        }
    }

    // ─── intersections: bright primary squares + HDR glow ────────────
    // Nearest intersection point in normalized coords.
    vec2 ix = vec2(floor(uv.x * N + 0.5), floor(uv.y * N + 0.5)) / N;
    // skip border (first/last) so it doesn't fight the canvas edge
    bool interior = (ix.x > 0.5 / N) && (ix.x < 1.0 - 0.5 / N)
                 && (ix.y > 0.5 / N) && (ix.y < 1.0 - 0.5 / N);
    if (interior) {
        float dI = length(uv - ix);
        // pulsing brightness — different phase per intersection
        float ph = h12(ix * 13.7) * 6.2831 + t * 1.7;
        float pulse = 0.55 + 0.45 * sin(ph);
        // pick a primary colour for this intersection
        float kc = h12(ix * 19.3);
        vec3 pc = pickPrimary(kc);

        // solid bright square at intersection center
        float core = 1.0 - smoothstep(pulseR * 0.6, pulseR * 1.1, dI);
        // HDR core peak ~1.6 linear for bloom
        float corePeakHDR = 1.6;
        col = mix(col, pc * corePeakHDR * (0.85 + 0.15 * pulse), core);

        // soft outer glow — additive HDR halo
        float halo = exp(-dI * dI / (glowR * glowR));
        vec3 glow = pc * halo * (0.55 + 0.45 * pulse) * gI;
        col += glow * 0.9;
    }

    // ─── audio bass: throw extra glowing squares ─────────────────────
    // Sample a handful of randomised positions; activate when bass is high.
    if (bass > 0.05) {
        for (int i = 0; i < 6; i++) {
            float fi = float(i);
            // slow-changing seeds so squares persist for a beat
            float seed = floor(TIME * 2.0 + fi * 11.0) + fi * 3.7;
            vec2 p = vec2(h11(seed * 1.13), h11(seed * 2.27));
            // snap to grid intersection
            p = (floor(p * N + 0.5)) / N;
            float dI = length(uv - p);
            float kc = h11(seed * 4.41);
            vec3 pc = pickPrimary(kc);
            float r = pulseR * (1.2 + 0.6 * bass);
            float core = 1.0 - smoothstep(r * 0.6, r * 1.1, dI);
            float corePeakHDR = 1.6;
            col = mix(col, pc * corePeakHDR, core * bass);
            float halo = exp(-dI * dI / (glowR * glowR * 1.6));
            col += pc * halo * bass * 0.8 * gI;
        }
    }

    // LINEAR HDR output, NO internal tonemap.
    gl_FragColor = vec4(col, 1.0);
}
