/*{
  "DESCRIPTION": "Voronoi Cell Organism — Haeckel radiolaria Voronoi cells pulsing with audio",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "audioReact",  "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,  "DEFAULT": 0.9  },
    { "NAME": "cellScale",   "TYPE": "float", "MIN": 1.0,   "MAX": 20.0, "DEFAULT": 6.0  },
    { "NAME": "pulseSpeed",  "TYPE": "float", "MIN": 0.0,   "MAX": 3.0,  "DEFAULT": 0.5  },
    { "NAME": "inkWidth",    "TYPE": "float", "MIN": 0.003, "MAX": 0.04, "DEFAULT": 0.012 }
  ]
}*/

precision highp float;

// ── Palette ──────────────────────────────────────────────────────────────────
const vec3 AMBER    = vec3(0.70, 0.28, 0.00);
const vec3 GOLD     = vec3(1.00, 0.72, 0.00);
const vec3 HOT_GOLD = vec3(2.80, 1.80, 0.00);
const vec3 IVORY    = vec3(1.80, 1.60, 1.00);
const vec3 INK      = vec3(0.00, 0.00, 0.00);

// ── Hash helpers ─────────────────────────────────────────────────────────────
float hash(float n) {
    return fract(sin(n) * 43758.5453123);
}

vec2 hash2(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)),
             dot(p, vec2(269.5, 183.3)));
    return fract(sin(p) * 43758.5453123);
}

// ── Voronoi ───────────────────────────────────────────────────────────────────
// Returns (dist-to-nearest, dist-to-2nd-nearest) and writes nearestCell.
vec2 voronoi(vec2 p, out vec2 nearestCell, float audio) {
    vec2 ip = floor(p);
    vec2 fp = fract(p);

    float d1 = 8.0, d2 = 8.0;
    vec2  nearest = vec2(0.0);

    for (int x = -2; x <= 2; x++) {
        for (int y = -2; y <= 2; y++) {
            vec2 neighbor = vec2(float(x), float(y));
            vec2 cellId   = ip + neighbor;

            vec2  jitter  = (hash2(cellId) * 2.0 - 1.0) * 0.4;
            float pulse   = sin(TIME * pulseSpeed + hash(cellId.x + cellId.y * 100.0) * 6.28318) * 0.1;
            jitter *= (1.0 + pulse * audio);

            vec2  cellCenter = neighbor + 0.5 + jitter;
            float d          = length(cellCenter - fp);

            if (d < d1) {
                d2      = d1;
                d1      = d;
                nearest = cellId;
            } else if (d < d2) {
                d2 = d;
            }
        }
    }

    nearestCell = nearest;
    return vec2(d1, d2);
}

void main() {
    // Audio modulator
    float audio = 0.5 + 0.5 * audioBass * audioReact;

    // UV
    vec2  uv     = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2  p      = uv * vec2(aspect, 1.0) * cellScale;

    // Voronoi query
    vec2  nearestCell;
    vec2  dists = voronoi(p, nearestCell, audio);
    float dist1 = dists.x;
    float dist2 = dists.y;

    // ── Cell interior color ───────────────────────────────────────────────────
    float cellHash = hash(nearestCell.x * 7.3 + nearestCell.y * 13.7);
    vec3  interior;
    if (cellHash < 0.4) {
        interior = AMBER;
    } else if (cellHash < 0.75) {
        interior = GOLD * 0.85;
    } else {
        interior = mix(AMBER, GOLD, cellHash);
    }
    // Subtle radial darkening toward edges (cytoplasm depth)
    interior *= (0.6 + 0.4 * (1.0 - dist1));
    // Audio-driven brightness boost
    interior *= (1.0 + (audio - 0.5) * 0.4);

    // ── Cell edge — INK walls ─────────────────────────────────────────────────
    float edgeMask = 1.0 - smoothstep(0.0, inkWidth, dist2 - dist1);
    vec3  col      = mix(interior, INK, edgeMask);

    // ── Cell-center nuclei ────────────────────────────────────────────────────
    // Find the jittered center for the nearest cell in cell-space
    vec2  ip2        = floor(p);
    vec2  fp2        = fract(p);
    // Re-derive nearest center position (offset from fragment)
    vec2  nJitter    = (hash2(nearestCell) * 2.0 - 1.0) * 0.4;
    float nPulse     = sin(TIME * pulseSpeed + hash(nearestCell.x + nearestCell.y * 100.0) * 6.28318) * 0.1;
    nJitter *= (1.0 + nPulse * audio);
    vec2  centerOff  = nearestCell - ip2;          // integer offset of nearest cell
    vec2  centerFrac = centerOff + 0.5 + nJitter;  // position of center in fract-space
    float nucDist    = length(centerFrac - fp2);

    // Nucleus glow — exponential falloff
    float nucHash    = hash(nearestCell.x * 3.1 + nearestCell.y * 17.9);
    vec3  nucColor   = (nucHash > 0.5) ? HOT_GOLD : IVORY;
    float nucGlow    = exp(-nucDist * 18.0) * (1.2 + audio * 0.6);
    col = mix(col, nucColor, clamp(nucGlow, 0.0, 1.0));

    // ── Background fill (very dark amber for cells outside range) ─────────────
    // Already handled by interior, but darken overall background via ambient
    col = max(col, AMBER * 0.3 * (1.0 - dist1 * 0.5));

    gl_FragColor = vec4(col, 1.0);
}
