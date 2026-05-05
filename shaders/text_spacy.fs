/*{
    "DESCRIPTION": "Warp Singularity — 3D hyperspace jump. Star streaks race from vanishing point; tunnel aperture contracts. Colors: blue-white streaks, gold, violet, deep black void.",
    "CATEGORIES": ["Generator", "3D", "Space", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "warpSpeed",    "TYPE": "float", "DEFAULT": 1.5,  "MIN": 0.1, "MAX": 5.0,  "LABEL": "Warp Speed" },
        { "NAME": "starCount",    "TYPE": "float", "DEFAULT": 80.0, "MIN": 20.0,"MAX": 200.0,"LABEL": "Star Count" },
        { "NAME": "hdrPeak",      "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",     "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// Star streak color from palette
vec3 starColor(float seed) {
    float h = hash11(seed * 7.13);
    if (h < 0.5)  return vec3(0.85, 0.92, 1.0);  // blue-white
    if (h < 0.75) return vec3(1.0,  0.75, 0.0);  // gold
    return             vec3(0.7,  0.1,  1.0);    // violet
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t   = TIME;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.6;

    vec3 col = vec3(0.0, 0.0, 0.0);

    int N = int(min(starCount, 200.0));

    for (int i = 0; i < 200; i++) {
        if (i >= N) break;
        float fi = float(i);

        // Random 3D star position in a cylinder
        float seed  = fi * 1.37;
        float ang   = hash11(seed * 1.17) * 6.2832;
        float rInit = hash11(seed * 2.31) * 0.8 + 0.05; // cylindrical radius
        float zInit = hash11(seed * 3.57); // normalized Z [0,1]

        // Z scrolls over time (warp jump: stars fly toward viewer)
        float zPhase = fract(zInit + t * warpSpeed * 0.25);
        float zWorld = zPhase; // 0 = far, 1 = close

        // Perspective projection
        float zProj  = 0.1 + zWorld * 0.9; // avoid divide-by-zero
        float screenR = rInit / zProj;
        vec2 starPt = vec2(cos(ang), sin(ang)) * screenR;

        // Streak tail: star appears as a line from previous position
        float zPrev = max(zWorld - warpSpeed * 0.015, 0.001);
        float rPrev  = rInit / (0.1 + zPrev * 0.9);
        vec2 prevPt  = vec2(cos(ang), sin(ang)) * rPrev;

        // Distance from uv to the streak segment
        vec2 ab = starPt - prevPt;
        vec2 ap = uv - prevPt;
        float denom = max(dot(ab,ab), 1e-6);
        float h_t   = clamp(dot(ap,ab)/denom, 0.0, 1.0);
        float d     = length(ap - ab * h_t);

        // Streak width proportional to proximity
        float width = 0.003 * (1.0 + zWorld * 2.0);
        float glow  = exp(-d * d / (width * width));

        // HDR brightness scales with proximity
        float bright = hdrPeak * zWorld * zWorld * audio;

        vec3 sc = starColor(fi);
        col += sc * glow * bright;
    }

    // Central singularity glow
    float singR = length(uv);
    float sing  = exp(-singR * singR * 6.0) * 1.5 * audio;
    col += vec3(0.6, 0.8, 1.0) * sing * hdrPeak;

    // Tunnel vignette ring
    float ring = abs(singR - 0.65 - sin(t*warpSpeed)*0.03);
    col += vec3(0.0, 0.5, 1.0) * exp(-ring * ring * 800.0) * hdrPeak * 0.5;

    gl_FragColor = vec4(col, 1.0);
}
