/*{
    "DESCRIPTION": "RGB Voxel Grid — raymarched 3D grid of glowing cubes, each channel (R/G/B) pulsing independently to audio. Per-cube HDR emission with black gaps. Contrasts prior flat RGB plane geometry.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "INPUTS": [
        { "NAME": "gridSize",   "LABEL": "Grid Size",   "TYPE": "float", "DEFAULT": 4.0,  "MIN": 2.0, "MAX": 8.0 },
        { "NAME": "cubeGap",    "LABEL": "Gap",         "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.02, "MAX": 0.5 },
        { "NAME": "pulseSpeed", "LABEL": "Pulse Speed", "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.0, "MAX": 4.0 },
        { "NAME": "hdrBoost",   "LABEL": "HDR Boost",   "TYPE": "float", "DEFAULT": 2.6,  "MIN": 1.0, "MAX": 4.0 },
        { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 }
    ]
}*/

float hash31(vec3 p) { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453); }

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Grid SDF: repeated cubes with gap
float scene(vec3 p, out vec3 cellId) {
    float cell = 1.0;
    vec3 rep = mod(p + 0.5, cell) - 0.5;
    cellId   = floor(p + 0.5);

    float half_ = (cell - cubeGap) * 0.5;
    return sdBox(rep, vec3(half_));
}

vec3 calcNormal(vec3 p) {
    vec3 ci;
    const vec2 e = vec2(0.003, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, ci) - scene(p - e.xyy, ci),
        scene(p + e.yxy, ci) - scene(p - e.yxy, ci),
        scene(p + e.yyx, ci) - scene(p - e.yyx, ci)
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;

    // Isometric-ish drifting orbit camera
    float camA = t * 0.09;
    float camH = 1.0 + sin(t * 0.05) * 0.4;
    float camD = gridSize * 1.4;
    vec3 ro = vec3(cos(camA) * camD, camH * 2.0, sin(camA) * camD);
    vec3 target = vec3(0.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd * 1.4 + uv.x * right + uv.y * up);

    // Clip march to grid bounds
    float halfG = gridSize * 0.5;
    float dt    = 0.0;
    bool  hit   = false;
    float dSurf = 1.0;
    vec3  cellId = vec3(0.0);

    for (int i = 0; i < 100; i++) {
        vec3 p = ro + rd * dt;
        // Stop outside grid bounding box
        vec3 bq = abs(p) - vec3(halfG + 1.0);
        if (max(bq.x, max(bq.y, bq.z)) > 0.0) break;

        vec3 ci;
        dSurf = scene(p, ci);
        if (dSurf < 0.002) { hit = true; cellId = ci; break; }
        dt += max(dSurf * 0.85, 0.008);
    }

    // Pure black background = maximum HDR contrast
    vec3 col = vec3(0.0);

    if (hit) {
        // Skip cells outside grid radius
        float maxG = gridSize * 0.5 - 0.5;
        if (max(abs(cellId.x), max(abs(cellId.y), abs(cellId.z))) > maxG) {
            gl_FragColor = vec4(col, 1.0);
            return;
        }

        vec3 p   = ro + rd * dt;
        vec3 nor = calcNormal(p);
        vec3 vd  = -rd;

        // Per-cell RGB channel assignment from cell hash
        float h1 = hash31(cellId);
        float h2 = hash31(cellId + vec3(0.5, 0.0, 0.0));
        float h3 = hash31(cellId + vec3(0.0, 0.5, 0.0));

        // Independent R/G/B pulse per cell
        float audioBoost = 1.0 + audioLevel * audioReact;
        float pR = 0.5 + 0.5 * sin(t * pulseSpeed * (0.6 + h1) + h1 * 6.28) * audioBoost;
        float pG = 0.5 + 0.5 * sin(t * pulseSpeed * (0.7 + h2) + h2 * 6.28 + 2.1) * audioBoost;
        float pB = 0.5 + 0.5 * sin(t * pulseSpeed * (0.5 + h3) + h3 * 6.28 + 4.2) * audioBoost;

        // Cell color: predominantly one RGB channel based on dominant hash
        vec3 cellCol;
        if (h1 > h2 && h1 > h3)
            cellCol = vec3(pR, pG * 0.15, pB * 0.05);  // Red cell
        else if (h2 > h3)
            cellCol = vec3(pR * 0.05, pG, pB * 0.15);  // Green cell
        else
            cellCol = vec3(pR * 0.05, pG * 0.15, pB);  // Blue cell

        // Electric light: no key needed — cube is emissive
        vec3 keyDir = normalize(vec3(1.0, 1.5, 0.8));
        float spec  = pow(max(0.0, dot(reflect(-keyDir, nor), vd)), 32.0);
        float diff  = max(0.0, dot(nor, keyDir));

        col  = cellCol * (diff * 0.5 + 0.3) * hdrBoost;
        col += vec3(1.0) * spec * hdrBoost * 0.4;  // white-hot specular

        // Black gap ink edge
        float ew   = fwidth(dSurf) * 4.0;
        float edge = smoothstep(0.0, ew, abs(dSurf));
        col = mix(vec3(0.0), col, edge);
    }

    gl_FragColor = vec4(col, 1.0);
}
