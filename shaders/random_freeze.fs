/*{
    "DESCRIPTION": "Voronoi Frost — 2D procedural ice crystallization. Voronoi cells with crack edges, animated slow growth. Ice palette: glacier blue, deep navy, crystal teal, violet. fwidth AA on cracks.",
    "CATEGORIES": ["Generator", "Abstract", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "cellScale",  "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0,  "MAX": 12.0, "LABEL": "Cell Scale" },
        { "NAME": "crackWidth", "TYPE": "float", "DEFAULT": 0.025,"MIN": 0.005,"MAX": 0.1,  "LABEL": "Crack Width" },
        { "NAME": "hdrPeak",    "TYPE": "float", "DEFAULT": 2.2,  "MIN": 1.0,  "MAX": 4.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2  hash22(vec2 p)  { return fract(sin(vec2(dot(p,vec2(127.1,311.7)), dot(p,vec2(269.5,183.3)))) * 43758.5453); }

// Ice palette — 4 fully saturated cool hues
vec3 iceColor(float t) {
    t = fract(t);
    if (t < 0.25) return mix(vec3(0.0,  0.35, 0.85), vec3(0.05, 0.12, 0.55), t * 4.0);    // glacier→navy
    if (t < 0.50) return mix(vec3(0.05, 0.12, 0.55), vec3(0.0,  0.7,  0.75), (t-0.25)*4.0); // navy→teal
    if (t < 0.75) return mix(vec3(0.0,  0.7,  0.75), vec3(0.45, 0.1,  0.85), (t-0.50)*4.0); // teal→violet
    return             mix(vec3(0.45, 0.1,  0.85), vec3(0.0,  0.35, 0.85), (t-0.75)*4.0); // violet→glacier
}

// Voronoi: returns (nearest cell dist, 2nd nearest cell dist, cell ID)
vec3 voronoi(vec2 p) {
    vec2  ip = floor(p);
    vec2  fp = fract(p);
    float d1 = 8.0, d2 = 8.0;
    float bestID = 0.0;
    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            vec2 neighbor = vec2(float(x), float(y));
            vec2 cell     = ip + neighbor;
            vec2 jitter   = hash22(cell);
            vec2 pt       = neighbor + jitter - fp;
            float d       = dot(pt, pt);
            if (d < d1) { d2 = d1; d1 = d; bestID = hash21(cell); }
            else if (d < d2) { d2 = d; }
        }
    }
    return vec3(sqrt(d1), sqrt(d2), bestID);
}

void main() {
    float asp = RENDERSIZE.x / RENDERSIZE.y;
    vec2 uv = isf_FragNormCoord * vec2(asp, 1.0);
    float t = TIME * 0.06;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;

    // Slow growth: scale pulses slightly
    float growPulse = 1.0 + sin(t * 0.8) * 0.04;
    vec2 p = uv * cellScale * growPulse;

    // Slow crystallization drift
    p += vec2(sin(t * 0.3) * 0.2, cos(t * 0.23) * 0.2);

    vec3 vor = voronoi(p);
    float d1 = vor.x;
    float d2 = vor.y;
    float cellID = vor.z;

    // Crack distance = 2nd nearest - nearest (thin at boundary)
    float crack = d2 - d1;

    // fwidth AA on crack edges
    float crackAA = fwidth(crack);
    float crackMask = smoothstep(crackWidth + crackAA, crackWidth - crackAA, crack);

    // Per-cell color
    vec3 cellCol = iceColor(cellID + t * 0.04);

    // Interior shimmer: brightness falloff from cell center
    float shimmer = 1.0 - d1 * 0.5;
    shimmer += sin(d1 * 18.0 - t * 1.5) * 0.08 * (1.0 - d1); // refraction rings

    // HDR: crystal face highlights
    float highlight = pow(max(0.0, 1.0 - d1 * 3.0), 4.0);

    vec3 col = cellCol * shimmer * hdrPeak * audio;
    col += vec3(0.85, 0.95, 1.0) * highlight * hdrPeak; // ice-white highlight
    col  = mix(col, vec3(0.0, 0.0, 0.01), crackMask);   // black ink cracks

    gl_FragColor = vec4(col, 1.0);
}
