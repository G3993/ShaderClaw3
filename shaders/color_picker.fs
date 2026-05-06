/*{
    "DESCRIPTION": "Neon Prism — standalone HDR generator. Animated rainbow caustic dispersion from a dark prism silhouette. No input required.",
    "CREDIT": "ShaderClaw auto-improve 2026-05-06",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        {"NAME": "disperseAmt", "LABEL": "Dispersion",    "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.1, "MAX": 2.0},
        {"NAME": "glowStr",    "LABEL": "Glow",           "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 4.0},
        {"NAME": "rotSpeed",   "LABEL": "Rotation",       "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0},
        {"NAME": "ripples",    "LABEL": "Caustic Ripples","TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0, "MAX": 8.0},
        {"NAME": "audioReact", "LABEL": "Audio React",    "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0}
    ]
}*/

// Full spectrum: violet→blue→cyan→green→yellow→orange→red
vec3 specColor(float t) {
    t = clamp(t, 0.0, 1.0) * 0.833; // stop before wrap-around back to violet
    vec3 p = abs(fract(vec3(t, t + 0.333, t + 0.667)) * 6.0 - 3.0) - 1.0;
    return clamp(p, 0.0, 1.0);
}

mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float t = TIME;
    float audio = 1.0 + (audioLevel + audioBass * 0.5) * audioReact * 0.5;

    // Polar coords
    float r = length(uv);
    float a = atan(uv.y, uv.x);

    // Prism: dark equilateral triangle silhouette
    // Triangle SDF in polar: compare angle to nearest vertex
    float prismSize = 0.18 * audio;
    float prismRot = t * rotSpeed;
    float ta = fract((a - prismRot) / (2.0 * 3.14159)); // normalized angle 0..1
    // sdTriangle via 3-fold symmetry
    float ta3 = fract(ta * 3.0) / 3.0 * 2.0 * 3.14159; // angle within one sector
    float prismD = r - prismSize / cos(ta3 - 3.14159 / 3.0);
    float prismMask = smoothstep(0.01, -0.01, prismD);
    float prismEdge = smoothstep(0.05, 0.0, abs(prismD)) * (1.0 - prismMask * 0.5);

    // Dispersion: 7 spectral bands radiating from prism edge
    vec3 col = vec3(0.0, 0.0, 0.008); // void black

    // Each band is a narrow angular stripe with a fixed hue
    int nBands = 7;
    for (int bi = 0; bi < 7; bi++) {
        float hue = float(bi) / 7.0; // fixed spectrum position
        vec3 bandColor = specColor(hue);

        // This band's angle: dispersed from a single source ray
        float sourceAngle = prismRot + 3.14159 * 0.5; // top vertex of prism
        float spread = disperseAmt * 0.55;
        float bandA = sourceAngle + (hue - 0.5) * spread;

        // Angular distance from this band's center
        float da = a - bandA;
        // Wrap to [-pi, pi]
        da = da - floor((da + 3.14159) / (2.0 * 3.14159)) * 2.0 * 3.14159;

        // Band width decreases with radius (perspective dispersion)
        float bw = 0.06 * disperseAmt / (1.0 + r * 0.8);
        float bandEnv = exp(-da * da / (bw * bw));

        // Radial envelope: starts at prism edge, fades outward
        float rStart = prismSize + 0.02;
        float rFade = exp(-(r - rStart) * (2.0 - disperseAmt * 0.3));
        rFade *= smoothstep(rStart - 0.01, rStart + 0.04, r);

        // Caustic ripples along the beam
        float ripplePhase = r * ripples * 8.0 - t * 2.5 + float(bi) * 0.9;
        float caustic = 0.65 + 0.35 * sin(ripplePhase);

        float intensity = bandEnv * rFade * caustic * glowStr * audio;
        col += bandColor * intensity;
    }

    // Second fan: a reflected beam going downward (like double-slit)
    for (int bi = 0; bi < 7; bi++) {
        float hue = float(bi) / 7.0;
        vec3 bandColor = specColor(hue);
        float sourceAngle = prismRot - 3.14159 * 0.5;
        float spread = disperseAmt * 0.55;
        float bandA = sourceAngle + (hue - 0.5) * spread;
        float da = a - bandA;
        da = da - floor((da + 3.14159) / (2.0 * 3.14159)) * 2.0 * 3.14159;
        float bw = 0.06 * disperseAmt / (1.0 + r * 0.8);
        float bandEnv = exp(-da * da / (bw * bw));
        float rStart = prismSize + 0.02;
        float rFade = exp(-(r - rStart) * 1.5) * smoothstep(rStart - 0.01, rStart + 0.04, r);
        float caustic = 0.65 + 0.35 * sin(r * ripples * 8.0 - t * 2.5 - float(bi) * 1.1);
        col += bandColor * bandEnv * rFade * caustic * glowStr * 0.7 * audio;
    }

    // Prism silhouette: darken interior, glow at edge
    col *= 1.0 - prismMask * 0.92;
    // White-hot edge glow (ink contrast)
    col += prismEdge * vec3(0.85, 0.92, 1.0) * 2.8;

    gl_FragColor = vec4(col, 1.0);
}
