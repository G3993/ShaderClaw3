/*{
  "DESCRIPTION": "Plasma Cell Growth — raymarched 3D organic metaball plasma cells with division animation. Crimson/orange/white-hot HDR palette.",
  "CREDIT": "ShaderClaw auto-improve v15",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "cellCount",  "LABEL": "Cell Count",  "TYPE": "float", "DEFAULT": 6.0,  "MIN": 2.0, "MAX": 10.0 },
    { "NAME": "blobSize",   "LABEL": "Blob Size",   "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.1, "MAX": 0.6 },
    { "NAME": "divSpeed",   "LABEL": "Div Speed",   "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "camSpeed",   "LABEL": "Cam Speed",   "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioMod",   "LABEL": "Audio Mod",   "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Smooth min for metaball blending
float smin(float a, float b, float k) {
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * k * 0.25;
}

// Sphere SDF
float sdSphere(vec3 p, vec3 c, float r) {
    return length(p - c) - r;
}

// Cell center with division animation
vec3 cellCenter(float id, float t) {
    float h1 = hash11(id * 1.37);
    float h2 = hash11(id * 2.71);
    float h3 = hash11(id * 5.13);
    float phase = id * 1.047; // 60 degree offsets

    // Orbit around origin with slight oscillation
    float orbitR = 0.5 + h1 * 0.6;
    float orbitA = phase + t * (0.2 + h3 * 0.3) * divSpeed;
    float orbitEl = 0.3 * sin(t * (0.1 + h2 * 0.2) + h1 * 6.28);

    return vec3(orbitR * cos(orbitA) * cos(orbitEl),
                orbitR * sin(orbitEl) + h2 * 0.4 - 0.2,
                orbitR * sin(orbitA) * cos(orbitEl));
}

float sceneSDF(vec3 p, float t) {
    int N = int(clamp(cellCount, 2.0, 10.0));
    float d = 1e10;
    float audioS = 1.0 + audioLevel * audioMod * 0.2;
    float sz = blobSize * audioS;

    // Add cells with smooth blending
    for (int i = 0; i < 10; i++) {
        if (i >= N) break;
        float fi = float(i);

        // Division: cell splits in two with a period
        float divPhase = fract(t * divSpeed * 0.3 + hash11(fi * 7.3));
        float splitting = smoothstep(0.4, 0.9, divPhase);
        float merging   = smoothstep(0.9, 1.0, divPhase);

        vec3 center = cellCenter(fi, t);
        float r = sz * (1.0 - splitting * 0.25 + merging * 0.25);

        if (splitting > 0.01) {
            // Elongate into two lobes
            float h4 = hash11(fi * 11.7);
            vec3 axis = normalize(vec3(cos(h4 * 6.28), sin(h4 * 6.28), 0.2));
            float sep = splitting * sz * 0.7;
            float d1 = sdSphere(p, center + axis * sep, r * (1.0 - splitting * 0.15));
            float d2 = sdSphere(p, center - axis * sep, r * (1.0 - splitting * 0.15));
            d = smin(d, smin(d1, d2, sz * 0.6), sz * 0.3);
        } else {
            d = smin(d, sdSphere(p, center, r), sz * 0.4);
        }
    }
    return d;
}

void main() {
    vec2 uv = isf_FragNormCoord.xy * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float t = TIME;
    float audio = 1.0 + (audioLevel + audioBass * 0.4) * audioMod;

    // Orbiting camera
    float camA = t * camSpeed * 0.5;
    float camEl = 0.4 + 0.2 * sin(t * camSpeed * 0.3);
    vec3 ro = vec3(cos(camA) * 3.5, sin(camEl) * 1.5, sin(camA) * 3.5);
    vec3 ta = vec3(0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 2.2 * ww);

    // Raymarch
    float tRay = 0.1;
    bool hit = false;
    vec3 hitP = vec3(0.0);
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * tRay;
        float d = sceneSDF(p, t);
        if (d < 0.002) { hitP = p; hit = true; break; }
        if (tRay > 8.0) break;
        tRay += d * 0.85;
    }

    // Deep crimson background (like microscope slide)
    vec3 col = vec3(0.04, 0.0, 0.01);

    if (hit) {
        float e = 0.002;
        vec3 n = normalize(vec3(
            sceneSDF(hitP + vec3(e,0,0), t) - sceneSDF(hitP - vec3(e,0,0), t),
            sceneSDF(hitP + vec3(0,e,0), t) - sceneSDF(hitP - vec3(0,e,0), t),
            sceneSDF(hitP + vec3(0,0,e), t) - sceneSDF(hitP - vec3(0,0,e), t)
        ));

        // Plasma cell palette: crimson to orange to white-hot specular
        // Hue 0.0-0.07 range (red-orange)
        float posHue = fract(dot(hitP, vec3(0.3, 0.5, 0.2)) * 0.4);
        float hue = 0.0 + posHue * 0.07;
        vec3 baseCol = hsv2rgb(vec3(hue, 1.0, 1.0));

        vec3 L = normalize(vec3(0.8, 1.2, 0.5));
        float diff = max(dot(n, L), 0.0);
        float spec = pow(max(dot(reflect(-L, n), -rd), 0.0), 16.0);
        float rim  = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);
        float occ  = clamp(sceneSDF(hitP + n * 0.15, t) / 0.15, 0.0, 1.0);

        col = baseCol * (0.1 + diff * 0.7) * hdrPeak * audio * occ;
        col += vec3(1.0, 0.9, 0.7) * spec * hdrPeak * 1.2; // white-hot spec
        col += baseCol * rim * hdrPeak * 0.6;               // rim = HDR edge

        // fwidth edge darkening for ink silhouette
        float fw = fwidth(tRay);
        col *= 1.0 - smoothstep(0.0, fw * 30.0, fw * 0.1);
    }

    // Glow aura around cells
    {
        vec3 gp = ro + rd * min(tRay, 6.0);
        float near = sceneSDF(gp, t);
        float glow = exp(-near * 8.0) * 0.35;
        col += vec3(0.9, 0.15, 0.02) * glow * hdrPeak * audio;
    }

    gl_FragColor = vec4(col, 1.0);
}
