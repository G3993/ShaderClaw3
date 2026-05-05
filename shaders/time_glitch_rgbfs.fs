/*{
  "DESCRIPTION": "Plasma Storm — domain-warped plasma clouds split by fork lightning",
  "CREDIT": "ShaderClaw — plasma storm generator",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "turbulence",   "LABEL": "Turbulence",   "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.1, "MAX": 3.0 },
    { "NAME": "cloudScale",   "LABEL": "Cloud Scale",  "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 6.0 },
    { "NAME": "lightningRate","LABEL": "Lightning",    "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "boltCount",    "LABEL": "Bolt Count",   "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 6.0 },
    { "NAME": "hdrBoost",     "LABEL": "HDR Boost",    "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "pulse",        "LABEL": "Audio Pulse",  "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "speed",        "LABEL": "Speed",        "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.5 }
  ]
}*/

#define MAX_BOLTS 6

float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(hash21(i),                   hash21(i + vec2(1.0, 0.0)), f.x),
        mix(hash21(i + vec2(0.0, 1.0)), hash21(i + vec2(1.0, 1.0)), f.x),
        f.y
    );
}

float fbm(vec2 p) {
    float v = 0.0, amp = 0.5;
    for (int i = 0; i < 6; i++) {
        v += amp * vnoise(p);
        p = p * 2.1 + vec2(1.7, 9.2);
        amp *= 0.48;
    }
    return v;
}

float sdSeg(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

float boltDist(vec2 uv, float seed) {
    float tSlot = floor(TIME * 3.0 + seed * 0.17);
    float minD = 1e9;

    // Main bolt: 16 segments, top (y=1) to bottom (y=0)
    float x = 0.2 + hash11(seed * 3.7 + tSlot * 0.11) * 0.6;
    vec2 A = vec2(x, 1.0);

    float branchX = 0.5, branchY = 0.5;
    int branchAt = int(hash11(seed * 5.1 + tSlot * 0.3) * 10.0) + 3;

    for (int i = 0; i < 16; i++) {
        float fi = 1.0 - float(i + 1) * 0.0625;
        float dx = (hash11(seed + float(i) * 7.31 + tSlot * 13.7) - 0.5) * 0.1;
        x = clamp(x + dx, 0.05, 0.95);
        vec2 B = vec2(x, fi);
        minD = min(minD, sdSeg(uv, A, B));
        if (i == branchAt) { branchX = x; branchY = fi; }
        A = B;
    }

    // Branch bolt: 8 segments continuing downward from branch point
    float bDir = (hash11(seed * 9.3 + tSlot * 0.7) > 0.5) ? 1.0 : -1.0;
    float bx = branchX;
    vec2 BA = vec2(bx, branchY);
    for (int i = 0; i < 8; i++) {
        float fi = max(branchY - float(i + 1) * 0.0625, 0.0);
        float dx = (hash11(seed * 2.3 + float(i) * 5.17 + tSlot * 9.1) - 0.3) * 0.1 * bDir;
        bx = clamp(bx + dx, 0.02, 0.98);
        vec2 BB = vec2(bx, fi);
        minD = min(minD, sdSeg(uv, BA, BB));
        BA = BB;
        if (fi <= 0.0) break;
    }

    return minD;
}

void main() {
    vec2 uv    = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float audio  = 1.0 + audioBass * pulse;
    float audioM = 1.0 + audioMid  * pulse * 0.6;

    // Plasma: double domain-warp FBM, aspect-corrected, time-scrolling
    vec2 p = vec2(uv.x * aspect, uv.y) * cloudScale;
    float tFlow = TIME * speed;

    vec2 q = vec2(
        fbm(p + vec2(tFlow * 0.4, 0.0)),
        fbm(p + vec2(0.0, tFlow * 0.3) + 5.2)
    );
    vec2 r = vec2(
        fbm(p + turbulence * audio * q + vec2(1.7, 9.2) + tFlow * 0.15),
        fbm(p + turbulence * audio * q + vec2(8.3, 2.8) + tFlow * 0.12)
    );
    float plasma = clamp(fbm(p + turbulence * audio * r) * 2.1 - 0.35, 0.0, 1.0);

    // Palette: void black / electric violet / hot cyan / gold / white-hot
    vec3 VIOLET = vec3(0.42, 0.0,  1.0);
    vec3 CYAN   = vec3(0.0,  0.88, 1.0);
    vec3 GOLD   = vec3(1.0,  0.70, 0.0);
    vec3 WHITE  = vec3(1.0,  0.94, 0.82);

    vec3 col = vec3(0.0);
    col = mix(col, VIOLET * hdrBoost,        smoothstep(0.0,  0.32, plasma));
    col = mix(col, CYAN   * hdrBoost * 1.1,  smoothstep(0.32, 0.60, plasma));
    col = mix(col, GOLD   * hdrBoost * 1.25, smoothstep(0.60, 0.80, plasma));
    col = mix(col, WHITE  * hdrBoost * 1.45, smoothstep(0.80, 1.0,  plasma));

    // Void-black ink crush on lowest plasma values
    col *= smoothstep(0.04, 0.14, plasma);

    // Lightning — each bolt flashes at 3 Hz, audio boosts appearance rate
    int nBolts = int(clamp(boltCount, 1.0, 6.0));
    float minBoltD = 1e9;

    for (int i = 0; i < MAX_BOLTS; i++) {
        if (i >= nBolts) break;
        float seed = float(i) * 31.7;
        float tSlot = floor(TIME * 3.0 + seed * 0.17);
        float effRate = min(lightningRate + audioMid * 0.3, 1.0);
        if (hash11(seed * 0.47 + tSlot * 0.29) > effRate) continue;
        minBoltD = min(minBoltD, boltDist(uv, seed));
    }

    float px    = 1.0 / min(RENDERSIZE.x, RENDERSIZE.y);
    float core  = 1.0 - smoothstep(0.0, px * 2.0,  minBoltD);
    float inner = 1.0 - smoothstep(0.0, px * 7.0,  minBoltD);
    float outer = 1.0 - smoothstep(0.0, px * 22.0, minBoltD);

    col += WHITE  * core  * 4.5 * audioM
         + VIOLET * inner * 3.5 * audioM
         + CYAN   * outer * 2.0 * audioM;

    gl_FragColor = vec4(col, 1.0);
}
