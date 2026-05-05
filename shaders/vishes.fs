/*{
    "DESCRIPTION": "Lichtenberg Discharge — procedural fractal branching lightning tree growing from center, 3D-lit with electric blue glow and black ink silhouette. Contrasts prior 2D cellular walker with structured arboreal geometry.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Cinematic"],
    "INPUTS": [
        { "NAME": "branchDepth", "LABEL": "Branch Depth", "TYPE": "float", "DEFAULT": 5.0,  "MIN": 2.0, "MAX": 7.0 },
        { "NAME": "growSpeed",   "LABEL": "Grow Speed",   "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "glowRad",     "LABEL": "Glow Radius",  "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.005, "MAX": 0.15 },
        { "NAME": "hdrBoost",    "LABEL": "HDR Boost",    "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0, "MAX": 4.0 },
        { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0 }
    ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// 2D distance to line segment
float sdSeg(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

// Recursive Lichtenberg branch contribution at pixel p
// Returns minimum distance to any branch segment
float lichtenberg(vec2 p, vec2 start, vec2 dir, float len, float t, int depth, float seed) {
    if (depth <= 0 || len < 0.005) return 1e8;

    float phase = t * growSpeed + seed * 0.73;
    float grow  = clamp(fract(phase) * float(depth), 0.0, 1.0);

    vec2 end = start + dir * len * grow;
    float d  = sdSeg(p, start, end);

    if (grow < 0.99) return d;

    // Branch 1: fork left
    float h1 = hash11(seed * 3.17);
    float h2 = hash11(seed * 5.37 + 0.5);
    float ang1 = atan(dir.y, dir.x) + (0.35 + h1 * 0.5);
    float ang2 = atan(dir.y, dir.x) - (0.35 + h2 * 0.5);
    vec2 d1 = vec2(cos(ang1), sin(ang1));
    vec2 d2 = vec2(cos(ang2), sin(ang2));

    float branchLen = len * (0.55 + hash11(seed * 7.53) * 0.25);

    float b1 = lichtenberg(p, end, d1, branchLen, t, depth - 1, seed + 11.3);
    float b2 = lichtenberg(p, end, d2, branchLen, t, depth - 1, seed + 23.7);
    return min(d, min(b1, b2));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t     = TIME;
    float audio = 1.0 + audioLevel * audioReact * 0.3;

    // 4-color palette: electric blue, violet, cyan, white-hot
    vec3 elecBlue  = vec3(0.05, 0.25, 1.00);
    vec3 violet    = vec3(0.45, 0.00, 1.00);
    vec3 elecCyan  = vec3(0.00, 0.85, 1.00);
    vec3 whiteHot  = vec3(0.80, 0.90, 1.00);

    // Black void background
    vec3 col = vec3(0.002, 0.001, 0.005);

    int depth = int(clamp(branchDepth, 2.0, 7.0));

    // Multiple primary branches radiating from center
    float bestDist = 1e8;
    for (int bi = 0; bi < 5; bi++) {
        float fi   = float(bi);
        float ang  = fi / 5.0 * 6.28318 + t * 0.04;
        float seed = fi * 17.3 + floor(t * growSpeed) * 7.1;
        vec2  dir  = vec2(cos(ang), sin(ang));

        float d = lichtenberg(uv, vec2(0.0), dir, 0.7, t, depth, seed);
        bestDist = min(bestDist, d);
    }

    // Glow and core rendering
    float rad = glowRad * audio;
    float core = smoothstep(rad * 0.3, 0.0, bestDist);
    float halo = exp(-bestDist / rad);
    float mid  = smoothstep(rad, 0.0, bestDist);

    // Color: white-hot core → cyan mid → electric blue halo → violet outer
    vec3 glowCol = mix(elecBlue, elecCyan, halo);
    glowCol = mix(glowCol, whiteHot, mid);

    col += glowCol * halo * hdrBoost * 0.7;
    col += whiteHot * core * hdrBoost;

    // Ambient electric violet shimmer
    float shimmer = 0.5 + 0.5 * sin(t * 8.0 + hash21(floor(uv * 20.0)) * 6.28);
    col += violet * shimmer * 0.04 * audio;

    // Black ink: strong silhouette where dist is just above core threshold
    float inkMask = smoothstep(rad * 1.5, rad * 0.6, bestDist);
    col = mix(col, vec3(0.0), inkMask * (1.0 - core) * 0.7);

    gl_FragColor = vec4(col, 1.0);
}
