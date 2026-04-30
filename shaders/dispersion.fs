/*{
  "DESCRIPTION": "Chromatic dispersion — refractive color separation driven by animated flow noise",
  "CREDIT": "ShaderClaw (dispersion model inspired by Shadertoy)",
  "CATEGORIES": ["Effect"],
  "INPUTS": [
    { "NAME": "inputImage", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "dispScale", "LABEL": "Dispersion", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 0.3 },
    { "NAME": "noiseScale", "LABEL": "Noise Scale", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.5, "MAX": 12.0 },
    { "NAME": "flowSpeed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrastAmt", "LABEL": "Contrast", "TYPE": "float", "DEFAULT": 12.0, "MIN": 1.0, "MAX": 30.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
  ],
  "PASSES": [
    { "TARGET": "dispBuf", "PERSISTENT": true },
    {}
  ]
}*/

// ---- Simplex 2D noise (Ashima Arts) ----
vec3 mod289(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec2 mod289(vec2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x * 34.0) + 1.0) * x); }

float snoise(vec2 v) {
    const vec4 C = vec4(0.211324865405187, 0.366025403784439,
                       -0.577350269189626, 0.024390243902439);
    vec2 i  = floor(v + dot(v, C.yy));
    vec2 x0 = v - i + dot(i, C.xx);
    vec2 i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
    vec4 x12 = x0.xyxy + C.xxzz;
    x12.xy -= i1;
    i = mod289(i);
    vec3 p = permute(permute(i.y + vec3(0.0, i1.y, 1.0))
                             + i.x + vec3(0.0, i1.x, 1.0));
    vec3 m = max(0.5 - vec3(dot(x0, x0), dot(x12.xy, x12.xy),
                             dot(x12.zw, x12.zw)), 0.0);
    m = m * m;
    m = m * m;
    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;
    m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
    vec3 g;
    g.x = a0.x * x0.x + h.x * x0.y;
    g.yz = a0.yz * x12.xz + h.yz * x12.yw;
    return 130.0 * dot(m, g);
}

// Curl noise — divergence-free 2D flow from scalar noise
vec2 curlNoise(vec2 p) {
    float eps = 0.001;
    float n1 = snoise(vec2(p.x, p.y + eps));
    float n2 = snoise(vec2(p.x, p.y - eps));
    float a = (n1 - n2) / (2.0 * eps);
    n1 = snoise(vec2(p.x + eps, p.y));
    n2 = snoise(vec2(p.x - eps, p.y));
    float b = (n1 - n2) / (2.0 * eps);
    return vec2(a, -b);
}

// ---- Dispersion helpers ----

vec3 sigmoidContrast(vec3 x, float k) {
    return 1.0 / (1.0 + exp(-k * (x - 0.5)));
}

vec2 normz(vec2 x) {
    return length(x) < 0.0001 ? vec2(0.0) : normalize(x);
}

// Spectral weights — models human cone response curves
vec3 sampleWeights(float i) {
    return vec3(i * i, 46.6666 * pow((1.0 - i) * i, 3.0), (1.0 - i) * (1.0 - i));
}

#define SAMPLES 16

vec3 sampleDisp(vec2 uv, vec2 dispNorm, float disp) {
    vec3 col = vec3(0.0);
    vec3 denom = vec3(0.0);
    float sd = 1.0 / float(SAMPLES);
    float wl = 0.0;
    for (int i = 0; i < SAMPLES; i++) {
        vec3 sw = sampleWeights(wl);
        denom += sw;
        col += sw * texture2D(inputImage, uv + dispNorm * disp * wl).rgb;
        wl += sd;
    }
    return col / denom;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 texel = 1.0 / RENDERSIZE;

    // ---- Pass 0: generate animated displacement field ----
    if (PASSINDEX == 0) {
        float t = TIME * flowSpeed;
        vec2 p = uv * noiseScale;
        // Multi-octave curl noise for organic flow
        vec2 d = curlNoise(p + t) * 0.6
               + curlNoise(p * 2.1 - t * 0.7) * 0.3
               + curlNoise(p * 4.3 + t * 0.4) * 0.1;
        gl_FragColor = vec4(d, 0.0, 1.0);
        return;
    }

    // ---- Pass 1: chromatic dispersion ----
    vec2 n = vec2(0.0, texel.y);
    vec2 e = vec2(texel.x, 0.0);
    vec2 s = vec2(0.0, -texel.y);
    vec2 w = vec2(-texel.x, 0.0);

    vec2 d   = texture2D(dispBuf, uv).xy;
    vec2 d_n = texture2D(dispBuf, fract(uv + n)).xy;
    vec2 d_e = texture2D(dispBuf, fract(uv + e)).xy;
    vec2 d_s = texture2D(dispBuf, fract(uv + s)).xy;
    vec2 d_w = texture2D(dispBuf, fract(uv + w)).xy;

    // Antialias the vector field
    vec2 db = 0.4 * d + 0.15 * (d_n + d_e + d_s + d_w);

    float ld = length(db);
    vec2 ln = normz(db);

    vec3 col = sampleDisp(uv, ln, dispScale * ld);
    col = sigmoidContrast(col, contrastAmt);

    float alpha = 1.0;
    if (transparentBg) {
        alpha = dot(col, vec3(0.299, 0.587, 0.114));
    }

    // Surprise: every ~22s a complete spectrum splits dramatically —
    // for ~0.5s the dispersion blows wide and rejoins. Newton's prism.
    {
        float _ph = fract(TIME / 22.0);
        float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.18, 0.10, _ph);
        // Push channels apart based on luminance
        float _l = dot(col, vec3(0.299, 0.587, 0.114));
        col = mix(col, vec3(col.r * 1.4, col.g, col.b * 0.8) * (0.5 + _l), _f * 0.4);
    }

    gl_FragColor = vec4(col, alpha);
}
