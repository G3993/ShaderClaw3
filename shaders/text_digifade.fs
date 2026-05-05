/*{
    "DESCRIPTION": "Data Cubes Dissolve — 3D grid of raymarched box SDFs that fragment and scatter over time. Deep void background. Fully saturated palette: electric blue, crimson, gold, violet. 64-step raymarch.",
    "CATEGORIES": ["Generator", "3D", "Glitch", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "gridSize",   "TYPE": "float", "DEFAULT": 4.0,  "MIN": 2.0, "MAX": 8.0,  "LABEL": "Grid Size" },
        { "NAME": "dissolve",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 1.0,  "LABEL": "Dissolve Wave" },
        { "NAME": "hdrPeak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",   "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash11(float n)  { return fract(sin(n*127.1)*43758.5453); }
float hash21(vec2 p)   { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

// Per-cube color from palette
vec3 cubeColor(vec2 ci) {
    float h = hash21(ci);
    if (h < 0.25) return vec3(0.05, 0.25, 1.0);  // electric blue
    if (h < 0.50) return vec3(0.95, 0.08, 0.05); // crimson
    if (h < 0.75) return vec3(1.0,  0.72, 0.0);  // gold
    return             vec3(0.6,  0.05, 1.0);    // violet
}

// Grid scene SDF
float sceneSDF(vec3 p) {
    float N = gridSize;
    float halfN = N * 0.5;
    float dmin = 1e6;
    float wavePhase = TIME * 0.4;

    for (int ix = 0; ix < 8; ix++) {
        if (float(ix) >= N) break;
        for (int iy = 0; iy < 8; iy++) {
            if (float(iy) >= N) break;
            vec2 ci = vec2(float(ix), float(iy));
            float seed = hash21(ci);

            // Cube position: dissolve wave scatters cubes outward
            float dissolveT = fract(wavePhase + seed * 1.3) * dissolve;
            float scatter = dissolveT * dissolveT;
            vec3 offset = vec3(
                sin(seed * 6.28 + wavePhase * 1.1) * scatter * 2.0,
                cos(seed * 9.42 + wavePhase * 0.9) * scatter * 2.0,
                sin(seed * 3.14 + wavePhase * 0.7) * scatter * 1.5
            );
            vec3 basePos = vec3(float(ix) - halfN + 0.5, float(iy) - halfN + 0.5, 0.0) * 0.55;
            vec3 cubeCenter = basePos + offset;

            // Scale: cubes shrink as they dissolve
            float scale = max(0.01, 1.0 - scatter * 1.2);
            float halfBox = 0.22 * scale;

            dmin = min(dmin, sdBox(p - cubeCenter, vec3(halfBox)));
        }
    }
    return dmin;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneSDF(p+e.xyy)-sceneSDF(p-e.xyy),
        sceneSDF(p+e.yxy)-sceneSDF(p-e.yxy),
        sceneSDF(p+e.yyx)-sceneSDF(p-e.yyx)
    ));
}

// ID: which cube was hit
vec3 hitColor(vec3 p) {
    float N = gridSize;
    float halfN = N * 0.5;
    float wavePhase = TIME * 0.4;
    float dmin = 1e6;
    vec3 bestCol = vec3(1.0);
    for (int ix = 0; ix < 8; ix++) {
        if (float(ix) >= N) break;
        for (int iy = 0; iy < 8; iy++) {
            if (float(iy) >= N) break;
            vec2 ci = vec2(float(ix), float(iy));
            float seed = hash21(ci);
            float dissolveT = fract(wavePhase + seed * 1.3) * dissolve;
            float scatter = dissolveT * dissolveT;
            vec3 offset = vec3(
                sin(seed*6.28 + wavePhase*1.1)*scatter*2.0,
                cos(seed*9.42 + wavePhase*0.9)*scatter*2.0,
                sin(seed*3.14 + wavePhase*0.7)*scatter*1.5
            );
            vec3 basePos = vec3(float(ix)-halfN+0.5, float(iy)-halfN+0.5, 0.0)*0.55;
            float scale = max(0.01, 1.0 - scatter*1.2);
            float d = sdBox(p-(basePos+offset), vec3(0.22*scale));
            if (d < dmin) { dmin = d; bestCol = cubeColor(ci); }
        }
    }
    return bestCol;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;

    // Slow orbiting camera
    float camT = TIME * 0.18;
    vec3 ro = vec3(sin(camT)*3.5, cos(camT*0.7)*1.5, cos(camT)*3.5);
    vec3 fw  = normalize(-ro);
    vec3 rgt = normalize(cross(fw, vec3(0.0,1.0,0.0)));
    vec3 up_ = cross(rgt, fw);
    vec3 rd  = normalize(fw + uv.x*rgt*0.7 + uv.y*up_*0.7);

    float dist = 0.0;
    bool hit = false;
    for (int i = 0; i < 64; i++) {
        float d = sceneSDF(ro + rd * dist);
        if (d < 0.003) { hit = true; break; }
        dist += d;
        if (dist > 12.0) break;
    }

    vec3 col = vec3(0.0, 0.0, 0.01);

    if (hit) {
        vec3 p    = ro + rd * dist;
        vec3 N    = calcNormal(p);
        vec3 base = hitColor(p);

        vec3 key  = normalize(vec3(-0.6, 1.0, -0.8));
        float kD  = max(dot(N, key), 0.0);
        float sp  = pow(max(dot(reflect(-key,N),-rd),0.0), 32.0);

        // fwidth edge darkening for cube silhouettes
        float edgeAA = fwidth(sceneSDF(p));

        col  = base * (kD + 0.1) * hdrPeak * audio;
        col += vec3(1.0) * sp * hdrPeak * 0.7;
    }

    gl_FragColor = vec4(col, 1.0);
}
