/*{
  "DESCRIPTION": "Thermal Vision — false-color heat map with adjustable palette, audio-reactive hotspots",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "contrast", "LABEL": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "palette", "LABEL": "Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Heat","Night Vision","Cyberpunk","Ice"] },
    { "NAME": "scanlines", "LABEL": "Scanlines", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "noise", "LABEL": "Noise", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "mixOriginal", "LABEL": "Original Mix", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

vec3 heatPalette(float t) {
    // Black → blue → red → yellow → white
    vec3 a = vec3(0.0, 0.0, 0.2);
    vec3 b = vec3(0.8, 0.0, 0.0);
    vec3 c = vec3(1.0, 0.8, 0.0);
    vec3 d = vec3(1.0, 1.0, 1.0);
    if (t < 0.33) return mix(a, b, t / 0.33);
    if (t < 0.66) return mix(b, c, (t - 0.33) / 0.33);
    return mix(c, d, (t - 0.66) / 0.34);
}

vec3 nightVision(float t) {
    return vec3(t * 0.1, t * 1.0, t * 0.15);
}

vec3 cyberPalette(float t) {
    vec3 a = vec3(0.05, 0.0, 0.1);
    vec3 b = vec3(0.8, 0.0, 0.4);
    vec3 c = vec3(0.0, 0.8, 1.0);
    if (t < 0.5) return mix(a, b, t / 0.5);
    return mix(b, c, (t - 0.5) / 0.5);
}

vec3 icePalette(float t) {
    vec3 a = vec3(0.0, 0.0, 0.15);
    vec3 b = vec3(0.2, 0.5, 0.9);
    vec3 c = vec3(0.8, 0.95, 1.0);
    if (t < 0.5) return mix(a, b, t / 0.5);
    return mix(b, c, (t - 0.5) / 0.5);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float bass = audioBass;

    vec3 original = hasInput ? texture2D(inputTex, uv).rgb : vec3(0.5);
    float lum = dot(original, vec3(0.299, 0.587, 0.114));

    // Contrast
    lum = clamp(pow(lum, 1.0 / contrast) * contrast, 0.0, 1.0);

    // Audio boost
    lum = clamp(lum + bass * 0.2, 0.0, 1.0);

    // Apply palette
    int pal = int(palette);
    vec3 col;
    if (pal == 0) col = heatPalette(lum);
    else if (pal == 1) col = nightVision(lum);
    else if (pal == 2) col = cyberPalette(lum);
    else col = icePalette(lum);

    // Mix original
    col = mix(col, original, mixOriginal);

    // Scanlines
    if (scanlines > 0.001) {
        float scan = sin(gl_FragCoord.y * 2.0) * 0.5 + 0.5;
        col *= 1.0 - scan * scanlines * 0.3;
    }

    // Noise
    if (noise > 0.001) {
        float n = hash(uv * RENDERSIZE + TIME * 100.0) - 0.5;
        col += n * noise;
    }

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
