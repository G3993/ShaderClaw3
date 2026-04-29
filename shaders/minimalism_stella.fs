/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Hard-edge minimalism after Frank Stella's Hyena Stomp (1962) and Marrakech (1964) — concentric Chebyshev rings marching outward from centre, hairline canvas gaps between bands, palette presets cycling from beat-synced rainbow to Black Paintings monochrome. What you see is what you see.",
  "INPUTS": [
    { "NAME": "bandCount", "LABEL": "Band Count", "TYPE": "float", "MIN": 4.0, "MAX": 22.0, "DEFAULT": 11.0 },
    { "NAME": "gapWidth", "LABEL": "Gap Width", "TYPE": "float", "MIN": 0.005, "MAX": 0.04, "DEFAULT": 0.014 },
    { "NAME": "rotation", "LABEL": "Rotation", "TYPE": "float", "MIN": 0.0, "MAX": 1.5708, "DEFAULT": 0.0 },
    { "NAME": "rotateSpeed", "LABEL": "Rotate Speed", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.05 },
    { "NAME": "marchSpeed", "LABEL": "March Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7 },
    { "NAME": "palettePreset", "LABEL": "Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Hyena Stomp", "Black Paintings", "Marrakech", "Synthetic Late"] },
    { "NAME": "paletteMarch", "LABEL": "Palette March", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4 },
    { "NAME": "shapeMode", "LABEL": "Ring Shape", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Square", "Diamond", "Hexagon"] },
    { "NAME": "saturation", "LABEL": "Saturation", "TYPE": "float", "MIN": 0.6, "MAX": 1.6, "DEFAULT": 1.0 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "useTex", "LABEL": "Sample Tex for Palette", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

// Stella's concentric square / diamond / polygon rings via the
// Chebyshev / L1 / hexagonal distance from canvas centre. Quantize the
// distance into bandCount rings; each ring picks a palette colour that
// MARCHES outward on bass beats.

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 stellaPalette(int preset, int bandIdx) {
    float fi = float(bandIdx);
    if (preset == 0) {
        // Hyena Stomp — saturated rainbow march
        return hsv2rgb(vec3(fract(fi / 11.0), 0.85, 0.95));
    } else if (preset == 1) {
        // Black Paintings — subtle pinstripe, not high-contrast.
        // Raw-canvas band tone matches actual Stella surfaces.
        return (bandIdx % 2 == 0) ? vec3(0.05) : vec3(0.42, 0.39, 0.34);
    } else if (preset == 2) {
        // Marrakech — metallic earth tones
        const vec3 mk[6] = vec3[6](
            vec3(0.78, 0.58, 0.20), vec3(0.62, 0.30, 0.18),
            vec3(0.36, 0.18, 0.10), vec3(0.86, 0.78, 0.62),
            vec3(0.92, 0.42, 0.20), vec3(0.45, 0.36, 0.22));
        return mk[bandIdx % 6];
    }
    // Synthetic late — fluorescent
    const vec3 sy[6] = vec3[6](
        vec3(0.95, 0.10, 0.78), vec3(0.20, 0.95, 0.45),
        vec3(0.18, 0.80, 0.95), vec3(0.95, 0.85, 0.20),
        vec3(0.65, 0.30, 0.95), vec3(0.95, 0.50, 0.20));
    return sy[bandIdx % 6];
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy - 0.5;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float r = rotation + TIME * rotateSpeed
            + audioMid * audioReact * 0.05;
    uv = mat2(cos(r), -sin(r), sin(r), cos(r)) * uv;

    float d;
    if (shapeMode == 0) {
        // Chebyshev (square rings)
        d = max(abs(uv.x), abs(uv.y));
    } else if (shapeMode == 1) {
        // L1 / Manhattan (diamond rings)
        d = abs(uv.x) + abs(uv.y);
    } else {
        // Hexagonal — distance to nearest hex face
        vec2 q = abs(uv);
        d = max(q.x * 0.866 + q.y * 0.5, q.y);
    }

    float bf = d * bandCount * 2.0;
    // Always-on march so rings shift outward even without audio.
    float bandIdx = floor(bf
                        + TIME * marchSpeed
                        + paletteMarch * audioBass * audioReact * 6.0);

    vec3 col;
    if (useTex && IMG_SIZE_inputTex.x > 0.0) {
        col = texture(inputTex, vec2(d * 1.4, 0.5)).rgb;
    } else {
        col = stellaPalette(int(palettePreset), int(bandIdx));
    }

    // Saturation control
    float L = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(L), col, saturation);

    // Hairline canvas-coloured gap between rings.
    float gap = smoothstep(0.5 - gapWidth, 0.5, fract(bf));
    vec3 raw = vec3(0.94, 0.91, 0.83);
    col = mix(col, raw, gap);

    col *= 0.86 + audioLevel * audioReact * 0.18;

    gl_FragColor = vec4(col + gap * audioHigh * audioReact * 0.08, 1.0);
}
