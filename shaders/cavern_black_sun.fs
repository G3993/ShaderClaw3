/*{
  "DESCRIPTION": "Cavern Black Sun — falling through Deep Cavern's ring tunnel toward a Black Hole Sun at the vanishing point. Ring palette receding outward, fbm-warped rays burning at the core. Eliasson + Soundgarden.",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "CREDIT": "Easel — combines deep_cavern + black_hole_sun (Andrea Bovo)",
  "INPUTS": [
    { "NAME": "pullSpeed",    "LABEL": "Pull Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 4.0,  "DEFAULT": 0.7 },
    { "NAME": "ringDensity",  "LABEL": "Ring Density",     "TYPE": "float", "MIN": 4.0, "MAX": 60.0, "DEFAULT": 20.0 },
    { "NAME": "fogDensity",   "LABEL": "Fog",              "TYPE": "float", "MIN": 0.5, "MAX": 4.0,  "DEFAULT": 1.5 },
    { "NAME": "breathe",      "LABEL": "Breathe",          "TYPE": "float", "MIN": 0.0, "MAX": 0.3,  "DEFAULT": 0.12 },
    { "NAME": "mouseTilt",    "LABEL": "Mouse Tilt",       "TYPE": "float", "MIN": 0.0, "MAX": 0.5,  "DEFAULT": 0.2 },
    { "NAME": "ringEdgeGlow", "LABEL": "Ring Edge Glow",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.8 },
    { "NAME": "texMix",       "LABEL": "Wall Texture Mix", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.4 },
    { "NAME": "sunRadius",    "LABEL": "Sun Radius",       "TYPE": "float", "MIN": 0.05,"MAX": 1.2,  "DEFAULT": 0.45 },
    { "NAME": "sunFeather",   "LABEL": "Sun Feather",      "TYPE": "float", "MIN": 0.05,"MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "sunBrightness","LABEL": "Sun Brightness",   "TYPE": "float", "MIN": 0.0, "MAX": 5.0,  "DEFAULT": 2.5 },
    { "NAME": "rayBrightness","LABEL": "Ray Brightness",   "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 2.5 },
    { "NAME": "rayDensity",   "LABEL": "Ray Density",      "TYPE": "float", "MIN": 0.0, "MAX": 60.0, "DEFAULT": 12.0 },
    { "NAME": "sunCurvature", "LABEL": "Sun Curvature",    "TYPE": "float", "MIN": 50.0,"MAX": 1080.0,"DEFAULT": 300.0 },
    { "NAME": "sunAngle",     "LABEL": "Sun Angle",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.5 },
    { "NAME": "sunFreq",      "LABEL": "Sun Freq",         "TYPE": "float", "MIN": 1.0, "MAX": 10.0, "DEFAULT": 5.0 },
    { "NAME": "sunWarp",      "LABEL": "Sun Warp",         "TYPE": "bool",  "DEFAULT": true },
    { "NAME": "sunTint",      "LABEL": "Sun Tint",         "TYPE": "color", "DEFAULT": [1.0, 0.85, 0.55, 1.0] },
    { "NAME": "audioReact",   "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "inputTex",     "LABEL": "Wall Texture",     "TYPE": "image" }
  ]
}*/

// ──────────────────────────────────────────────────────────────────────
// Shared helpers
// ──────────────────────────────────────────────────────────────────────
float hash1(float x) { return fract(sin(x * 127.1) * 43758.5453); }

vec2 hash2(vec2 x) {
    const vec2 k = vec2(0.3183099, 0.3678794);
    x = x * k + k.yx;
    return -1.0 + 2.0 * fract(64.0 * k * fract(x.x * x.y * (x.x + x.y)));
}

float gradNoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(dot(hash2(i + vec2(0.0, 0.0)), f - vec2(0.0, 0.0)),
                   dot(hash2(i + vec2(1.0, 0.0)), f - vec2(1.0, 0.0)), u.x),
               mix(dot(hash2(i + vec2(0.0, 1.0)), f - vec2(0.0, 1.0)),
                   dot(hash2(i + vec2(1.0, 1.0)), f - vec2(1.0, 1.0)), u.x), u.y);
}

mat2 rot2d(float a) { float s = sin(a), c = cos(a); return mat2(c, -s, s, c); }

float sunFbm(vec2 p, float freq) {
    float z = 1.0;
    float rz = 0.0;
    p *= 0.25;
    mat2 mrot = rot2d(sunAngle);
    for (int i = 1; i < 6; i++) {
        rz += (sin(gradNoise(p) * freq) * 0.5 + 0.5) / (z + TIME * 0.001);
        z *= 1.75;
        p *= 2.0;
        p *= mrot;
    }
    return rz;
}

// 4-stop ring palette
vec3 ringPalette(float band) {
    float t = fract(band * 0.157);
    vec3 a = vec3(0.85, 0.35, 0.65);
    vec3 b = vec3(0.25, 0.45, 0.95);
    vec3 c = vec3(0.95, 0.78, 0.35);
    vec3 d = vec3(0.5,  0.85, 0.65);
    if (t < 0.25) return mix(a, b, t / 0.25);
    if (t < 0.5)  return mix(b, c, (t - 0.25) / 0.25);
    if (t < 0.75) return mix(c, d, (t - 0.50) / 0.25);
    return mix(d, a, (t - 0.75) / 0.25);
}

// ──────────────────────────────────────────────────────────────────────
void main() {
    // Polar coords centered, with mouse tilt & breathing — shared by both effects
    vec2 p = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    p -= (mousePos - 0.5) * mouseTilt;

    float r  = length(p) * (1.0 + audioBass * audioReact * breathe);
    float th = atan(p.y, p.x);

    // ─── Deep Cavern: receding rings ─────────────────────────────────
    float v = 1.0 / max(r, 1e-3) + TIME * pullSpeed * (1.0 + audioLevel * audioReact * 0.5);
    float u = th / 6.2832 + 0.5;

    float band    = floor(v * ringDensity);
    vec3  ringCol = ringPalette(band + audioMid * audioReact * 2.0);
    float fillStripe = step(0.5, fract(v * ringDensity));
    ringCol *= mix(0.6, 1.0, fillStripe);

    float fog  = exp(-r * fogDensity);
    float edge = smoothstep(0.06, 0.0, abs(fract(v * ringDensity) - 0.5))
               * ringEdgeGlow * (0.4 + audioHigh * audioReact * 1.5);

    vec3 cavernCol = ringCol;
    if (IMG_SIZE_inputTex.x > 0.0 && texMix > 0.001) {
        vec3 tex = texture(inputTex, vec2(u, fract(v * 0.5))).rgb;
        cavernCol = mix(ringCol, tex, texMix);
    }
    cavernCol = cavernCol * fog + edge * ringCol;
    cavernCol *= smoothstep(0.0, 0.15, r); // outer vignette only

    // ─── Black Hole Sun: warped fbm rays at the core ─────────────────
    float t  = TIME * 0.025;
    vec2  uv = (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
    uv *= sunCurvature * 5e-2;

    float rs = sqrt(dot(uv, uv));
    float xs = dot(normalize(uv), vec2(0.5, 0.0)) + t;
    float ys = dot(normalize(uv), vec2(0.0, 0.5)) + t;
    if (sunWarp) {
        float d = rayDensity * 0.5;
        xs = sunFbm(vec2(ys * d, rs + xs * d), sunFreq);
        ys = sunFbm(vec2(rs + ys * d, xs * d), sunFreq);
    }
    float val = sunFbm(vec2(rs + ys * rayDensity, rs + xs * rayDensity - ys), sunFreq);
    val = smoothstep(0.0, rayBrightness, val);
    vec3 sunCol = clamp(1.0 - vec3(val), 0.0, 1.0);
    // Original black-hole-sun spot tinting — bright pulse pinned to centre
    sunCol = mix(sunCol, vec3(1.0),
                 (sunBrightness - 1.5) - 10.0 * rs / sunCurvature * (200.0 / max(sunBrightness, 0.01)));
    sunCol *= sunTint.rgb;
    // Audio adds a bass-driven pulse to the rays
    sunCol *= 1.0 + audioBass * audioReact * 0.5;

    // ─── Composite — sun dominates the core, rings dominate the periphery
    float sunMix = 1.0 - smoothstep(sunRadius, sunRadius + sunFeather, r);
    vec3 col = mix(cavernCol, sunCol, sunMix);
    // Halo: even outside the sun radius, leak some ray brightness into the rings
    float halo = (1.0 - val) * smoothstep(sunRadius * 1.6, sunRadius, r);
    col += sunTint.rgb * halo * 0.35;

    gl_FragColor = vec4(col, 1.0);
}
