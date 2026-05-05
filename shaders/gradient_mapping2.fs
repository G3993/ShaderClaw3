/*{
  "DESCRIPTION": "Gradient Mapper — maps input luminosity through a 3-stop vivid cosine palette with TIME-driven hue drift and HDR highlights.",
  "CREDIT": "",
  "ISFVSN": "2",
  "CATEGORIES": ["Effect", "Color"],
  "INPUTS": [
    { "NAME": "inputImage",     "TYPE": "image" },
    { "NAME": "baseColor",      "TYPE": "color",  "DEFAULT": [0.04, 0.02, 0.55, 1.0] },
    { "NAME": "targetColor",    "TYPE": "color",  "DEFAULT": [1.0,  0.05, 0.35, 1.0] },
    { "NAME": "highlightColor", "LABEL": "Highlight", "TYPE": "color", "DEFAULT": [1.0, 0.85, 0.10, 1.0] },
    { "NAME": "hueDrift",       "LABEL": "Hue Drift",    "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "contrast",       "LABEL": "Contrast",     "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "hdrBoost",       "LABEL": "HDR Boost",    "TYPE": "float", "DEFAULT": 1.5,  "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "audioReact",     "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "showGradient",   "LABEL": "Show Preview", "TYPE": "bool",  "DEFAULT": true }
  ]
}*/

// ─── Gradient Mapper ──────────────────────────────────────────────────────────
// Maps input luminosity to a 3-stop palette via cosine interpolation.
// Shadows → baseColor, midtones → targetColor, highlights → highlightColor.
// TIME-driven hue drift rotates the whole palette slowly.
// HDR boost lifts bright areas above 1.0 for bloom pipeline.
// ─────────────────────────────────────────────────────────────────────────────

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    return vec3(abs(q.z + (q.w - q.y) / (6.0*d + 1e-10)), d / (q.x + 1e-10), q.x);
}

// 3-stop gradient: 0=shadow (baseColor), 0.5=midtone (targetColor), 1=highlight (highlightColor)
vec3 gradientMap(float t, float drift) {
    // Apply hue drift to all three stops
    vec3 c0 = rgb2hsv(baseColor.rgb);
    vec3 c1 = rgb2hsv(targetColor.rgb);
    vec3 c2 = rgb2hsv(highlightColor.rgb);
    c0.x = fract(c0.x + drift);
    c1.x = fract(c1.x + drift);
    c2.x = fract(c2.x + drift);
    // Force full saturation for vivid output
    c0.y = max(c0.y, 0.85);
    c1.y = max(c1.y, 0.85);
    c2.y = max(c2.y, 0.85);

    // Piecewise linear interpolation: shadow→mid, mid→highlight
    vec3 col;
    if (t < 0.5) {
        col = mix(hsv2rgb(c0), hsv2rgb(c1), t * 2.0);
    } else {
        col = mix(hsv2rgb(c1), hsv2rgb(c2), (t - 0.5) * 2.0);
    }
    return col;
}

void main() {
    vec2 uv = isf_FragNormCoord;

    // Show gradient preview strip at top 10% of frame
    float isPreview = showGradient ? step(0.9, uv.y) : 0.0;
    vec2 sampleUV = isPreview > 0.5 ? vec2(uv.x, 0.5) : uv;

    vec4 src = IMG_NORM_PIXEL(inputImage, sampleUV);
    float lum = isPreview > 0.5
        ? uv.x  // gradient preview: sweep luminosity left→right
        : dot(src.rgb, vec3(0.299, 0.587, 0.114));

    // Contrast S-curve
    float lumC = lum - 0.5;
    lumC = lumC * (1.0 + contrast * 0.5) * (1.0 + abs(lumC) * contrast * 0.5);
    lum = clamp(lumC + 0.5, 0.0, 1.0);

    // Audio drives luminosity shift — bass lifts midtones
    lum = clamp(lum + audioBass * audioReact * 0.08, 0.0, 1.0);

    // TIME-driven hue drift
    float drift = hueDrift * sin(TIME * 0.15) * 0.5;

    vec3 col = gradientMap(lum, drift);

    // HDR boost for highlights — bright areas punch above 1.0
    float hiMask = smoothstep(0.6, 1.0, lum);
    col += col * hiMask * hdrBoost * 1.5;   // highlights reach 2.5× at lum=1

    // Audio-reactive shimmer on bright areas
    col += col * hiMask * audioBass * audioReact * 0.4;

    // Preview strip: add a white tick mark at the midpoint
    if (isPreview > 0.5) {
        float tick = smoothstep(0.008, 0.0, abs(uv.x - 0.5)) * 2.0;
        col += vec3(2.0) * tick;
    }

    // Surprise: every ~17s gradient inverts for ~0.5s — solarize pop
    {
        float _ph = fract(TIME / 17.0);
        float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.20, 0.12, _ph);
        vec3 inverted = gradientMap(1.0 - lum, drift + 0.5);
        col = mix(col, inverted * 2.2, _f);
    }

    gl_FragColor = vec4(col, src.a);
}
