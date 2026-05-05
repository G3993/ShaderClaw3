/*{
    "DESCRIPTION": "Paint Suspension — 3D raymarched smooth metaball paint drops floating in liquid. Painter's primary palette: cobalt blue, cadmium yellow, alizarin crimson, viridian. Slow drifting motion. Cinematic lighting.",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "CREDIT": "ShaderClaw",
    "INPUTS": [
        { "NAME": "blobCount",   "LABEL": "Blob Count",  "TYPE": "float", "DEFAULT": 8.0,  "MIN": 3.0,  "MAX": 14.0 },
        { "NAME": "blobSize",    "LABEL": "Blob Size",   "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.1,  "MAX": 0.8  },
        { "NAME": "hdrPeak",     "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0  },
        { "NAME": "driftSpeed",  "LABEL": "Drift Speed", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0,  "MAX": 1.0  },
        { "NAME": "audioReact",  "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0  }
    ]
}*/

// 4-color painter's primaries (fully saturated, no white mixing)
vec3 paintPal(int idx) {
    if (idx == 0) return vec3(0.0, 0.25, 0.85);  // cobalt blue
    if (idx == 1) return vec3(1.0, 0.80, 0.0);   // cadmium yellow
    if (idx == 2) return vec3(0.85, 0.04, 0.12); // alizarin crimson
    if (idx == 3) return vec3(0.04, 0.55, 0.22); // viridian green
    if (idx == 4) return vec3(0.80, 0.30, 0.0);  // burnt sienna (accent)
    return vec3(0.5, 0.0, 0.8);                  // violet (accent)
}

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }

// Smooth minimum (metaball blending)
float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5*(b-a)/k, 0.0, 1.0);
    return mix(b, a, h) - k*h*(1.0-h);
}

// Blob center positions (animated)
vec3 blobCenter(int idx, float t) {
    float fi = float(idx);
    float s1 = hash11(fi * 1.37);
    float s2 = hash11(fi * 2.91);
    float s3 = hash11(fi * 4.17);
    float s4 = hash11(fi * 7.53);
    float sp  = hash11(fi * 11.7);
    float audio = 1.0 + audioBass * audioReact * 0.3;

    float ox = (s1 - 0.5) * 3.0;
    float oy = (s2 - 0.5) * 2.0;
    float oz = (s3 - 0.5) * 2.0;

    // Slow drift orbits
    float freq = 0.3 + s4 * 0.4;
    ox += sin(t * driftSpeed * freq + s1 * 6.28) * 0.5;
    oy += cos(t * driftSpeed * freq * 0.73 + s2 * 6.28) * 0.4 * audio;
    oz += sin(t * driftSpeed * freq * 0.57 + s3 * 6.28) * 0.4;

    return vec3(ox, oy, oz);
}

float blobRadius(int idx, float t) {
    float fi = float(idx);
    float s = hash11(fi * 3.31);
    float audio = 1.0 + audioMid * audioReact * 0.2;
    return blobSize * (0.6 + s * 0.8) * audio;
}

float map(vec3 p, float t) {
    int N = int(clamp(blobCount, 3.0, 14.0));
    float d = 1e8;
    for (int i = 0; i < 14; i++) {
        if (i >= N) break;
        vec3 center = blobCenter(i, t);
        float r = blobRadius(i, t);
        float di = length(p - center) - r;
        d = smin(d, di, 0.4);
    }
    return d;
}

// Nearest blob for color attribution
int nearestBlob(vec3 p, float t) {
    int N = int(clamp(blobCount, 3.0, 14.0));
    float dMin = 1e8;
    int idx = 0;
    for (int i = 0; i < 14; i++) {
        if (i >= N) break;
        float d = length(p - blobCenter(i, t)) - blobRadius(i, t);
        if (d < dMin) { dMin = d; idx = i; }
    }
    return idx;
}

vec3 calcNormal(vec3 p, float t) {
    vec2 e = vec2(0.003, 0.0);
    return normalize(vec3(
        map(p+e.xyy,t)-map(p-e.xyy,t),
        map(p+e.yxy,t)-map(p-e.yxy,t),
        map(p+e.yyx,t)-map(p-e.yyx,t)));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;

    // Slow studio camera, gentle up-tilt
    vec3 ro = vec3(0.0, 0.5, 5.0);
    vec3 rd = normalize(vec3(uv * 0.45, -1.0));

    // Liquid medium background (deep teal-black)
    vec3 col = vec3(0.0, 0.02, 0.03);
    float dm = 0.01;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dm;
        float d = map(p, t);
        if (d < 0.005) {
            vec3 N = calcNormal(p, t);
            int cidx = nearestBlob(p, t);
            vec3 paint = paintPal(cidx % 6) * hdrPeak;

            // Studio lighting: key (warm top-right) + fill (cool left)
            vec3 key = normalize(vec3(0.8, 1.0, 0.5));
            vec3 fill = normalize(vec3(-0.6, 0.2, 0.8));
            float dKey  = max(dot(N, key), 0.0);
            float dFill = max(dot(N, fill), 0.0);
            float spec  = pow(max(dot(reflect(-key, N), -rd), 0.0), 24.0);

            // fwidth ink edge
            float dotNV = dot(N, -rd);
            float edgeW = fwidth(dotNV);
            float edge = 1.0 - smoothstep(-edgeW, 0.12+edgeW, dotNV);

            col = paint * (0.1 + dKey*0.7 + dFill*0.2) + vec3(1.0)*spec*3.0;
            col *= 1.0 - edge * 0.9;
            break;
        }
        if (dm > 12.0) break;
        dm += max(d * 0.85, 0.005);
    }

    gl_FragColor = vec4(col, 1.0);
}
