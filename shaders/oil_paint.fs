/*{
  "DESCRIPTION": "Waterlily Pool — impressionist Monet-palette water surface with caustics and floating lily pads",
  "CREDIT": "ShaderClaw — waterlily v2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "waveSpeed",  "LABEL": "Wave Speed",  "TYPE": "float", "DEFAULT": 0.40, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "waveScale",  "LABEL": "Wave Scale",  "TYPE": "float", "DEFAULT": 3.0,  "MIN": 0.5, "MAX": 8.0 },
    { "NAME": "causticStr", "LABEL": "Caustics",    "TYPE": "float", "DEFAULT": 1.8,  "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "lilyCount",  "LABEL": "Lily Pads",   "TYPE": "float", "DEFAULT": 6.0,  "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 4.0 },
    { "NAME": "audioMod",   "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

float hash21w(vec2 p) {
    p = fract(p * vec2(234.34, 435.346));
    p += dot(p, p + 34.23);
    return fract(p.x * p.y);
}

float hash1w(float n) { return fract(sin(n * 127.1) * 43758.5); }

float smoothNoiseW(vec2 p) {
    vec2 i = floor(p); vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash21w(i), b = hash21w(i + vec2(1.0, 0.0));
    float c = hash21w(i + vec2(0.0, 1.0)), d = hash21w(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbmWater(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * smoothNoiseW(p);
        p = p * 2.07 + vec2(3.2, 1.83);
        a *= 0.52;
    }
    return v;
}

// Monet palette: deep blue → aqua → lavender HDR → white-hot HDR
vec3 monetPalette(float f) {
    float h = hdrPeak;
    if (f < 0.25)      return mix(vec3(0.0,  0.05, 0.38),        vec3(0.10, 0.55, 0.68),        f / 0.25);
    else if (f < 0.50) return mix(vec3(0.10, 0.55, 0.68),        vec3(0.52, 0.32, 0.80),        (f - 0.25) / 0.25);
    else if (f < 0.75) return mix(vec3(0.52, 0.32, 0.80),        vec3(h*0.45, h*0.65, h),       (f - 0.50) / 0.25);
    else               return mix(vec3(h*0.45, h*0.65, h),        vec3(h, h, h*0.88),            (f - 0.75) / 0.25);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 uvA = vec2(uv.x * aspect, uv.y);

    float t = TIME * waveSpeed;
    float audioBoost = 1.0 + audioLevel * audioMod * 0.5;

    // ── Water FBM (domain warped) ──────────────────────────────────────────
    vec2 q = vec2(fbmWater(uvA * waveScale + t * 0.08),
                  fbmWater(uvA * waveScale + vec2(5.1, 2.9) + t * 0.06));
    float water = fbmWater(uvA * waveScale * 0.85 + q * 1.6 + t * 0.04);

    // ── Caustic sparkles: constructive interference at wave peaks ──────────
    float cA = sin(uvA.x * 24.0 * waveScale + water * 9.0 + t * 2.8)
             * sin(uvA.y * 19.0 * waveScale + water * 7.0 - t * 2.3);
    float cB = sin(uvA.x * 11.0 * waveScale - uvA.y * 7.0 * waveScale + t * 1.9)
             * sin(uvA.x * 6.0  * waveScale + uvA.y * 14.0 * waveScale - t * 1.5);
    float caustic = pow(max(cA * 0.6 + cB * 0.4, 0.0), 3.5);

    // ── Palette index ──────────────────────────────────────────────────────
    float f = clamp(water * 0.62 + caustic * causticStr * 0.38, 0.0, 1.0);
    f = clamp(f * (0.85 + audioBoost * 0.15), 0.0, 1.0);
    vec3 col = monetPalette(f);

    // ── Lily pads ──────────────────────────────────────────────────────────
    int maxPads = 10;
    float lc = min(lilyCount, 10.0);
    for (int i = 0; i < 10; i++) {
        if (float(i) >= lc) break;
        float fi = float(i);
        // Deterministic position with gentle drift
        vec2 center = vec2(
            (0.12 + fract(fi * 0.7374 + 0.13) * 0.76) * aspect,
             0.10 + fract(fi * 0.3819 + 0.57) * 0.80
        );
        center.x += 0.035 * sin(t * 0.28 + fi * 2.37);
        center.y += 0.025 * cos(t * 0.21 + fi * 1.93);

        float radius = 0.038 + fract(fi * 0.5137) * 0.052;
        float dist   = length(uvA - center);

        // Lily pad: dark green fill
        float padEdge = dist - radius;
        float padMask = smoothstep(0.006, -0.008, padEdge);
        vec3 lilyGreen = vec3(0.02, 0.14 + fract(fi * 0.31) * 0.06, 0.03);
        col = mix(col, lilyGreen, padMask * 0.92);

        // Notch (water gap in pad): wedge cutout
        float angle = atan(uvA.y - center.y, uvA.x - center.x);
        float notch = smoothstep(0.18, 0.0, abs(angle - (fi * 1.37 + 0.5))) * float(dist < radius);
        col = mix(col, col * 0.0, notch * padMask);

        // Flower bloom at pad center: hot pink/white HDR
        float flowerD = dist - radius * 0.28;
        float flowerMask = smoothstep(0.012, 0.001, abs(flowerD));
        vec3 bloom = vec3(hdrPeak * 0.95, hdrPeak * 0.45, hdrPeak * 0.55);
        col += bloom * flowerMask * 0.7;

        // Rim highlight: aqua/white HDR at pad edge
        float rimMask = smoothstep(0.012, 0.0, abs(padEdge) - 0.002);
        col += vec3(hdrPeak * 0.35, hdrPeak * 0.7, hdrPeak * 0.5) * rimMask * 0.5;
    }

    gl_FragColor = vec4(col, 1.0);
}
