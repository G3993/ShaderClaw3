/*{
    "DESCRIPTION": "Voronoi Cell Organism — Haeckel radiolaria Voronoi cells pulsing with audio",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        { "NAME": "audioReact",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.9 },
        { "NAME": "cellScale",   "TYPE": "float", "MIN": 1.0, "MAX": 20.0, "DEFAULT": 6.0 },
        { "NAME": "pulseSpeed",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 0.5 },
        { "NAME": "inkWidth",    "TYPE": "float", "MIN": 0.003, "MAX": 0.04, "DEFAULT": 0.012 }
    ]
}*/

precision highp float;

// ---------- palette ----------
const vec3 AMBER    = vec3(0.7,  0.28, 0.0);
const vec3 GOLD     = vec3(1.0,  0.72, 0.0);
const vec3 HOT_GOLD = vec3(2.8,  1.8,  0.0);
const vec3 IVORY    = vec3(1.8,  1.6,  1.0);
const vec3 INK      = vec3(0.0,  0.0,  0.0);

// ---------- hash utilities ----------
float hash(float n) {
    return fract(sin(n) * 43758.5453123);
}

vec2 hash2(vec2 p) {
    float x = hash(p.x * 127.1 + p.y * 311.7);
    float y = hash(p.x * 269.5 + p.y * 183.3);
    return vec2(x, y);
}

// ---------- Voronoi ----------
// Returns (dist1, dist2) — distance to nearest and 2nd-nearest cell centre.
// nearestCell is set to the integer grid coords of the nearest cell.
vec2 voronoi(vec2 p, out vec2 nearestCell, out vec2 nearestCentre) {
    vec2 ip = floor(p);
    vec2 fp = fract(p);

    float d1 = 8.0;
    float d2 = 8.0;
    vec2 nearest = vec2(0.0);
    vec2 nCentre  = vec2(0.0);

    float audio = 0.5 + 0.5 * audioBass * audioReact;

    for (int x = -2; x <= 2; x++) {
        for (int y = -2; y <= 2; y++) {
            vec2 neighbor = vec2(float(x), float(y));
            vec2 cellId   = ip + neighbor;

            vec2 jitter = (hash2(cellId) * 2.0 - 1.0) * 0.4;
            float pulse = sin(TIME * pulseSpeed + hash(cellId.x + cellId.y * 100.0) * 6.28318) * 0.1;
            jitter *= (1.0 + pulse * audio);

            vec2 cellCentre = neighbor + 0.5 + jitter;
            float d = length(cellCentre - fp);

            if (d < d1) {
                d2 = d1;
                d1 = d;
                nearest  = cellId;
                nCentre  = cellCentre;
            } else if (d < d2) {
                d2 = d;
            }
        }
    }

    nearestCell   = nearest;
    nearestCentre = nCentre;
    return vec2(d1, d2);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Audio modulator
    float audio = 0.5 + 0.5 * audioBass * audioReact;

    // Scale to Voronoi space
    vec2 p = uv * vec2(aspect, 1.0) * cellScale;

    vec2 nearestCell;
    vec2 nearestCentre;
    vec2 dists = voronoi(p, nearestCell, nearestCentre);
    float d1 = dists.x;
    float d2 = dists.y;

    // ---- cell interior colour ----
    float cellHash = hash(nearestCell.x * 7.3 + nearestCell.y * 13.7);
    vec3 cellCol;
    if (cellHash < 0.33) {
        cellCol = AMBER;
    } else if (cellHash < 0.66) {
        cellCol = GOLD * 0.65;
    } else {
        cellCol = AMBER * 0.8 + GOLD * 0.2;
    }
    // Subtle brightness variation driven by audio
    cellCol *= (0.85 + 0.3 * audio * hash(nearestCell.x * 3.1 + nearestCell.y * 5.7));

    // ---- ink cell walls ----
    // Edge strength: how close are we to the boundary between two cells?
    float edgeDist = d2 - d1;
    float inkMask = smoothstep(inkWidth, 0.0, edgeDist);
    vec3 col = mix(cellCol, INK, inkMask);

    // ---- cell-centre nuclei ----
    // Distance to the nearest Voronoi centre in cell space
    vec2 fp = fract(p);
    float centDist = length(nearestCentre - fp);
    float nucleusHash = hash(nearestCell.x * 11.3 + nearestCell.y * 17.1);

    // Two types: HOT_GOLD nucleus or IVORY nucleus
    vec3 nucleusCol = (nucleusHash > 0.5) ? HOT_GOLD : IVORY;
    // Exponential falloff glow from centre
    float nucleusRadius = 0.06 + 0.04 * audio;
    float nucleusGlow   = exp(-centDist * 18.0) * (1.5 + 0.8 * audio);
    float nucleusMask   = smoothstep(nucleusRadius, 0.0, centDist);

    // Blend: nucleus hard disk first, then glow halo on top
    col = mix(col, nucleusCol, nucleusMask);
    col += nucleusCol * nucleusGlow * 0.4 * (1.0 - inkMask);

    // ---- background (very dark amber, visible only in gaps) ----
    col = mix(AMBER * 0.18, col, 1.0); // already fully covered but preserves dark floor

    // ---- output LINEAR HDR — no clamp, no tonemapping ----
    gl_FragColor = vec4(col, 1.0);
}
