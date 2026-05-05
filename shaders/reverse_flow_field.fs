/*{
    "DESCRIPTION": "Plasma River — 3D raymarched volumetric plasma ribbons twisting through space. FBM domain-warped sinusoidal ribbons with neon cyan/magenta/lime palette. Audio drives ribbon amplitude.",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "CREDIT": "ShaderClaw",
    "INPUTS": [
        { "NAME": "ribbonCount", "LABEL": "Ribbons",    "TYPE": "float", "DEFAULT": 5.0,  "MIN": 2.0,  "MAX": 10.0 },
        { "NAME": "flowSpeed",   "LABEL": "Flow Speed", "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0  },
        { "NAME": "hdrPeak",     "LABEL": "HDR Peak",   "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0,  "MAX": 5.0  },
        { "NAME": "thickness",   "LABEL": "Thickness",  "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.02, "MAX": 0.2  },
        { "NAME": "audioReact",  "LABEL": "Audio React","TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0  }
    ]
}*/

// 4-color plasma palette: cyan / magenta / electric lime / violet
vec3 plasmaPal(float t) {
    t = fract(t);
    if (t < 0.25) return mix(vec3(0.0,1.0,1.0), vec3(1.0,0.0,1.0), t*4.0);
    if (t < 0.50) return mix(vec3(1.0,0.0,1.0), vec3(0.5,1.0,0.0), (t-0.25)*4.0);
    if (t < 0.75) return mix(vec3(0.5,1.0,0.0), vec3(0.4,0.0,1.0), (t-0.50)*4.0);
    return mix(vec3(0.4,0.0,1.0), vec3(0.0,1.0,1.0), (t-0.75)*4.0);
}

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }
float hash12(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// FBM for ribbon warping
float fbm(vec3 p) {
    float v = 0.0, a = 0.5;
    vec3 shift = vec3(100.0);
    for (int i = 0; i < 4; i++) {
        v += a * (sin(p.x)*cos(p.y) + sin(p.y)*cos(p.z) + sin(p.z)*cos(p.x));
        p = p * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}

// Ribbon SDF: a sine-wave tube twisted through 3D
float ribbonSDF(vec3 p, float t, int idx) {
    float fi = float(idx);
    float s1 = hash11(fi * 1.37);
    float s2 = hash11(fi * 2.91);
    float s3 = hash11(fi * 4.17);

    float phase = fi * 1.2 + t * flowSpeed * (0.5 + s1 * 0.5);
    float freq = 0.8 + s2 * 0.8;
    float amp = 0.6 + s3 * 0.4;

    float audio = 1.0 + audioBass * audioReact * 0.4 + audioMid * audioReact * 0.2;

    // Ribbon center curve
    float cx = amp * audio * sin(p.z * freq + phase);
    float cy = amp * audio * cos(p.z * freq * 0.7 + phase * 1.3) * 0.6
             + (fi - float(int(ribbonCount))/2.0) * 0.5;

    // FBM warp
    vec3 warpPt = vec3(p.z * 0.3 + phase * 0.1, fi * 0.7, t * 0.2);
    cx += fbm(warpPt) * 0.15;
    cy += fbm(warpPt + vec3(3.7)) * 0.15;

    vec2 dist2d = p.xy - vec2(cx, cy);
    return length(dist2d) - thickness;
}

float map(vec3 p, float t) {
    float d = 1e8;
    int N = int(clamp(ribbonCount, 2.0, 10.0));
    for (int i = 0; i < 10; i++) {
        if (i >= N) break;
        d = min(d, ribbonSDF(p, t, i));
    }
    return d;
}

vec3 calcNormal(vec3 p, float t) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p+e.xyy,t)-map(p-e.xyy,t),
        map(p+e.yxy,t)-map(p-e.yxy,t),
        map(p+e.yyx,t)-map(p-e.yyx,t)));
}

// Find nearest ribbon for color
int nearestRibbon(vec3 p, float t) {
    float dMin = 1e8;
    int idx = 0;
    int N = int(clamp(ribbonCount, 2.0, 10.0));
    for (int i = 0; i < 10; i++) {
        if (i >= N) break;
        float d = ribbonSDF(p, t, i);
        if (d < dMin) { dMin = d; idx = i; }
    }
    return idx;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;
    vec3 ro = vec3(0.0, 0.0, -3.0 + t * flowSpeed * 0.5); // forward flight
    vec3 rd = normalize(vec3(uv, 1.5));

    vec3 col = vec3(0.0, 0.0, 0.012); // deep space background
    float dm = 0.01;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dm;
        float d = map(p, t);
        if (d < 0.002) {
            vec3 N = calcNormal(p, t);
            int ridx = nearestRibbon(p, t);
            float hue = float(ridx) / ribbonCount + t * 0.03;
            vec3 plasma = plasmaPal(hue) * hdrPeak;

            vec3 light = normalize(vec3(0.5, 1.0, -0.5));
            float diff = max(dot(N, light), 0.0);
            float spec = pow(max(dot(reflect(-light, N), -rd), 0.0), 16.0);

            // fwidth edge AA
            float dotNV = dot(N, -rd);
            float edgeW = fwidth(dotNV);
            float edge = 1.0 - smoothstep(-edgeW, 0.15+edgeW, dotNV);

            col = plasma * (0.15 + diff * 0.85) + vec3(1.0)*spec*2.5;
            col *= 1.0 - edge * 0.88;
            break;
        }
        if (dm > 12.0) break;
        dm += d * 0.85;
    }

    gl_FragColor = vec4(col, 1.0);
}
