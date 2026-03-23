/*{
  "DESCRIPTION": "Wave Distort — animated wave displacement with multiple modes, great for webcam",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "waveX", "LABEL": "Horizontal", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.0, "MAX": 0.15 },
    { "NAME": "waveY", "LABEL": "Vertical", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.0, "MAX": 0.15 },
    { "NAME": "freq", "LABEL": "Frequency", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 30.0 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "mode", "LABEL": "Mode", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Sine","Square","Sawtooth","Radial"] },
    { "NAME": "rgbPhase", "LABEL": "RGB Phase", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

float wave(float x, int m) {
    if (m == 1) return sign(sin(x)); // square
    if (m == 2) return fract(x / 6.2832) * 2.0 - 1.0; // sawtooth
    return sin(x); // sine
}

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float bass = audioBass;
    float t = TIME * speed;
    int m = int(mode);

    float wx = waveX * (1.0 + bass * 3.0);
    float wy = waveY * (1.0 + bass * 3.0);

    vec2 d = vec2(0.0);
    if (m == 3) {
        // Radial mode
        vec2 center = mousePos;
        vec2 p = uv - center;
        float r = length(p);
        float radWave = sin(r * freq * 10.0 - t * 5.0) * (wx + wy) * 0.5;
        radWave *= exp(-r * 3.0);
        d = normalize(p + 0.001) * radWave;
    } else {
        d.x = wave(uv.y * freq + t, m) * wx;
        d.y = wave(uv.x * freq * 1.3 + t * 0.7, m) * wy;
    }

    vec3 col;
    if (hasInput) {
        if (rgbPhase > 0.001) {
            float phase = rgbPhase * 0.02;
            float r = texture2D(inputTex, clamp(uv + d * 1.2, 0.0, 1.0)).r;
            float g = texture2D(inputTex, clamp(uv + d, 0.0, 1.0)).g;
            float b = texture2D(inputTex, clamp(uv + d * 0.8, 0.0, 1.0)).b;
            col = vec3(r, g, b);
        } else {
            col = texture2D(inputTex, clamp(uv + d, 0.0, 1.0)).rgb;
        }
    } else {
        vec2 wuv = uv + d;
        float c = sin(wuv.x * 20.0) * cos(wuv.y * 20.0);
        col = vec3(0.5 + 0.5 * c) * vec3(0.8, 0.9, 1.0);
    }

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
