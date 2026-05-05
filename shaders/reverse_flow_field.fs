/*{
  "DESCRIPTION": "Plasma Ribbons — raymarched 3D plasma tendrils coiling through space. Electric magenta/violet/cyan palette. Cinematic lighting.",
  "CREDIT": "ShaderClaw auto-improve v14",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "ribbonCount", "LABEL": "Ribbons",    "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0, "MAX": 8.0 },
    { "NAME": "coilFreq",    "LABEL": "Coil Freq",  "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 6.0 },
    { "NAME": "thickness",   "LABEL": "Thickness",  "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.01, "MAX": 0.2 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",   "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "camSpeed",    "LABEL": "Cam Speed",  "TYPE": "float", "DEFAULT": 0.2,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioMod",    "LABEL": "Audio Mod",  "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// SDF capsule
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b - a, ap = p - a;
    float h = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
    return length(ap - ab * h) - r;
}

// Ribbon SDF: piecewise capsule spine following a coil
float ribbonSDF(vec3 p, float idx, float t) {
    float s1 = hash11(idx * 1.37);
    float s2 = hash11(idx * 2.71);
    float s3 = hash11(idx * 5.13);
    float phase = idx * 2.094; // 120° offset per ribbon

    float minD = 1e10;
    int SEGS = 8;
    vec3 prevPt = vec3(0.0);
    for (int s = 0; s <= SEGS; s++) {
        float fS = float(s) / float(SEGS);
        float y = (fS - 0.5) * 3.0;
        float angle = fS * coilFreq * 6.28318 + phase + t * (0.3 + s1 * 0.4);
        float r = 0.4 + 0.3 * sin(fS * 3.1 + t * 0.5 + s2 * 6.28);
        vec3 pt = vec3(r * cos(angle) + s2 * 0.4 - 0.2,
                       y + 0.3 * sin(t * 0.7 + s3 * 6.28),
                       r * sin(angle) + s3 * 0.4 - 0.2);
        if (s > 0) minD = min(minD, sdCapsule(p, prevPt, pt, thickness));
        prevPt = pt;
    }
    return minD;
}

vec2 sceneSDF(vec3 p, float t) {
    float d = 1e10;
    float id = 0.0;
    int N = int(clamp(ribbonCount, 1.0, 8.0));
    for (int i = 0; i < 8; i++) {
        if (i >= N) break;
        float di = ribbonSDF(p, float(i), t);
        if (di < d) { d = di; id = float(i); }
    }
    return vec2(d, id);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float t = TIME;
    float audio = 1.0 + (audioLevel + audioBass * 0.4) * audioMod;

    // Orbiting camera
    float camA = t * camSpeed * 0.4;
    float camEl = 0.3 + 0.2 * sin(t * camSpeed * 0.3);
    vec3 ro = vec3(cos(camA) * 3.0, sin(camEl) * 1.5, sin(camA) * 3.0);
    vec3 ta = vec3(0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 2.0 * ww);

    // Raymarch
    float tRay = 0.0;
    float id = -1.0;
    vec3 hit = vec3(0.0);
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * tRay;
        vec2 res = sceneSDF(p, t);
        if (res.x < 0.001) { hit = p; id = res.y; break; }
        if (tRay > 10.0) break;
        tRay += res.x * 0.9;
    }

    vec3 col = vec3(0.005, 0.0, 0.02); // deep space void

    if (id >= 0.0) {
        // Normal via finite difference
        float e = 0.001;
        vec3 n = normalize(vec3(
            sceneSDF(hit + vec3(e,0,0), t).x - sceneSDF(hit - vec3(e,0,0), t).x,
            sceneSDF(hit + vec3(0,e,0), t).x - sceneSDF(hit - vec3(0,e,0), t).x,
            sceneSDF(hit + vec3(0,0,e), t).x - sceneSDF(hit - vec3(0,0,e), t).x
        ));

        // Ribbon hue: electric magenta→violet→cyan
        float hue = fract(id / ribbonCount + 0.75); // 0.75-1.0 range = magenta→violet
        vec3 ribbonCol = hsv2rgb(vec3(hue, 1.0, 1.0));

        // Lighting: rim + specular
        vec3 L = normalize(vec3(1.0, 1.2, 0.5));
        float diff = max(dot(n, L), 0.0);
        float spec = pow(max(dot(reflect(-L, n), -rd), 0.0), 12.0);
        float rim  = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);

        col = ribbonCol * (0.2 + diff * 0.6) * hdrPeak * audio;
        col += vec3(1.0, 0.8, 1.0) * spec * hdrPeak; // white specular
        col += ribbonCol * rim * hdrPeak * 0.8;       // rim glow (HDR)

        // fwidth AA on SDF edge
        float fw = fwidth(tRay);
        col *= 1.0 - smoothstep(0.0, fw * 20.0, 0.001);
    }

    // Glow around ribbons (volumetric-like)
    {
        vec3 p = ro + rd * min(tRay, 6.0);
        vec2 near = sceneSDF(p, t);
        float glow = exp(-near.x * 15.0) * 0.4;
        float hue2 = fract(near.y / ribbonCount + 0.75);
        col += hsv2rgb(vec3(hue2, 1.0, 1.0)) * glow * hdrPeak * audio;
    }

    gl_FragColor = vec4(col, 1.0);
}
