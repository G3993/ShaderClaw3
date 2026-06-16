/*{
  "CATEGORIES": ["Generator", "Simulation"],
  "DESCRIPTION": "Red Carpet — heavy velvet drape rendered as a procedurally folded surface. Catenary vertical folds, micro-wrinkles, audio-reactive billowing, anisotropic velvet specular with rim glow and deep crease shadows.",
  "INPUTS": [
    { "NAME": "clothColor",      "LABEL": "Cloth Color",       "TYPE": "color", "DEFAULT": [0.62, 0.04, 0.06, 1.0] },
    { "NAME": "highlightColor",  "LABEL": "Highlight",         "TYPE": "color", "DEFAULT": [1.00, 0.50, 0.42, 1.0] },
    { "NAME": "shadowColor",     "LABEL": "Shadow",            "TYPE": "color", "DEFAULT": [0.07, 0.00, 0.01, 1.0] },
    { "NAME": "foldCount",       "LABEL": "Major Folds",       "TYPE": "float", "MIN": 3.0,  "MAX": 14.0, "DEFAULT": 7.0 },
    { "NAME": "foldDepth",       "LABEL": "Fold Depth",        "TYPE": "float", "MIN": 0.05, "MAX": 0.65, "DEFAULT": 0.30 },
    { "NAME": "foldVariance",    "LABEL": "Fold Variance",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "wrinkleAmount",   "LABEL": "Wrinkles",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.45 },
    { "NAME": "wrinkleScale",    "LABEL": "Wrinkle Scale",     "TYPE": "float", "MIN": 4.0,  "MAX": 32.0, "DEFAULT": 14.0 },
    { "NAME": "drapeCurve",      "LABEL": "Drape Curve",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "lightAngle",      "LABEL": "Light Angle",       "TYPE": "float", "MIN": 0.0,  "MAX": 6.2832, "DEFAULT": 0.85 },
    { "NAME": "velvetSheen",     "LABEL": "Velvet Sheen",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.7 },
    { "NAME": "rimGlow",         "LABEL": "Rim Glow",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "specular",        "LABEL": "Specular",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "billowAmount",    "LABEL": "Billow",            "TYPE": "float", "MIN": 0.0,  "MAX": 0.20, "DEFAULT": 0.05 },
    { "NAME": "billowSpeed",     "LABEL": "Billow Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.18 },
    { "NAME": "audioReact",      "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "filmGrain",       "LABEL": "Film Grain",        "TYPE": "float", "MIN": 0.0,  "MAX": 0.05, "DEFAULT": 0.012 },
    { "NAME": "transparentBg",   "LABEL": "Transparent",       "TYPE": "bool",  "DEFAULT": false }
  ]
}*/

// We model the drape as a height field h(uv) summing:
//   1. Major catenary folds: bumpy cosine waves whose amplitude bows
//      down toward the floor (gravity pull) — gives the classic pleated
//      curtain.
//   2. Phased secondary harmonic for fold-within-fold complexity.
//   3. Micro-wrinkles via fbm — small surface noise that catches light.
//   4. Audio-driven billow via low-frequency fbm warp on the input UV.
//
// Then differentiate the height to get a normal, run a velvet-style
// anisotropic shading with rim glow, deep crease shadow, and specular
// highlight.

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

float fbm(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 5; i++) {
        s += a * vnoise(p);
        p = mat2(1.6, 1.2, -1.2, 1.6) * p;
        a *= 0.5;
    }
    return s;
}

// Drape height — major folds + minor harmonic + wrinkle fbm.
// uv.y = 0 at floor (top of canvas in screen-space terms, but here we
// treat 0 as bottom of frame and 1 as top hung anchor).
float drapeHeight(vec2 uv) {
    // Per-column fold variance — varies fold pitch across width.
    float laneJit = (vnoise(vec2(uv.x * 1.7, 0.0)) - 0.5) * foldVariance * 0.4;
    // Catenary amplitude: bows away from anchor (top) toward floor
    // (bottom) so folds get deeper downward.
    float drape = pow(1.0 - uv.y, mix(1.0, 2.6, drapeCurve));

    // Major folds — vertical pleats.
    float major = sin((uv.x + laneJit) * foldCount * 6.2832
                    + uv.y * 0.7) * drape * foldDepth;

    // Phased secondary harmonic — fold-within-fold.
    float minor = sin((uv.x + laneJit) * foldCount * 12.5664
                    + uv.y * 1.2 + 1.3) * drape * foldDepth * 0.32;

    // Audio-driven billow: low-freq fbm gently warps fold position so
    // the curtain breathes.
    float billow = (fbm(vec2(uv.x * 1.4, uv.y * 0.8 + TIME * billowSpeed))
                  - 0.5) * billowAmount * (1.0 + audioBass * audioReact);

    // Micro wrinkles — fine surface noise.
    float wrinkle = (fbm(uv * wrinkleScale) - 0.5)
                  * wrinkleAmount * 0.06;

    return major + minor + billow + wrinkle;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Height field + finite-difference normal.
    float e = 0.002;
    float h  = drapeHeight(uv);
    float hx = drapeHeight(uv + vec2(e, 0.0));
    float hy = drapeHeight(uv + vec2(0.0, e));
    vec3 N = normalize(vec3((h - hx) / e, (h - hy) / e, 1.0));

    // Light vector
    float la = lightAngle;
    vec3  L  = normalize(vec3(cos(la), sin(la), 0.7));
    vec3  V  = vec3(0.0, 0.0, 1.0);
    vec3  H  = normalize(L + V);

    // Diffuse — gentle Lambert
    float diff = max(dot(N, L), 0.0);

    // Velvet anisotropic — uses 1 - dot(N,V) to brighten grazing angles.
    // This is what makes velvet read as velvet: bright at the edges,
    // duller in the middle.
    float NoV = max(dot(N, V), 0.0);
    float velvet = pow(1.0 - NoV, 4.0) * velvetSheen;

    // Rim glow — extra brightness at silhouette of folds.
    float rim = pow(1.0 - NoV, 3.0) * rimGlow;

    // Specular highlight — sharp glint along fold tops.
    float NoH = max(dot(N, H), 0.0);
    float spec = pow(NoH, 28.0) * specular;

    // Crease occlusion — deepest folds (h far from local mean) darken.
    // Approximate by clamping the second derivative.
    float ao = clamp(0.5 + (h - (drapeHeight(uv + vec2(0.0, e * 6.0))
                              + drapeHeight(uv - vec2(0.0, e * 6.0))) * 0.5)
                 * 8.0, 0.0, 1.0);
    ao = mix(0.55, 1.0, ao);

    // Compose colour. Diffuse layer gets clothColor; velvet adds
    // highlight tint; specular adds white-ish bright dot.
    vec3 base = clothColor.rgb;
    vec3 hi   = highlightColor.rgb;
    vec3 sh   = shadowColor.rgb;

    vec3 col = mix(sh, base, diff);
    col = mix(col, hi, velvet * 0.6);
    col += hi * rim * 0.4;
    col += vec3(1.0, 0.92, 0.86) * spec;
    col *= ao;

    // Floor pooling — deep shadow at very bottom where the carpet pools.
    float pool = smoothstep(0.18, 0.0, uv.y);
    col = mix(col, sh, pool * 0.5);

    // Subtle vertical gradient brightness — top of curtain catches more
    // ambient light than the floor.
    col *= 0.85 + smoothstep(0.0, 1.0, uv.y) * 0.25;

    // Audio-driven luminance breath
    col *= 1.0 + audioLevel * audioReact * 0.06;

    // Film grain
    col += (hash21(uv * RENDERSIZE) - 0.5) * filmGrain;

    float a = 1.0;
    if (transparentBg) {
        // Make outside-cloth transparent — but our drape covers the full
        // canvas, so this is mostly a flag for downstream compositing.
        a = 1.0;
    }

    gl_FragColor = vec4(col, a);
}
