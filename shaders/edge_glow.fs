/*{
  "DESCRIPTION": "Edge Glow — neon edge detection with customizable glow color, audio-reactive",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "edgeStr", "LABEL": "Edge Strength", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 8.0 },
    { "NAME": "glowStr", "LABEL": "Glow", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "mixOriginal", "LABEL": "Original Mix", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glowColor", "LABEL": "Glow Color", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "invert", "LABEL": "Invert", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

float luma(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    vec2 px = 1.0 / RENDERSIZE;
    float bass = audioBass;

    // Sobel edge detection
    float tl = luma(texture2D(inputTex, uv + vec2(-px.x, px.y)).rgb);
    float t_ = luma(texture2D(inputTex, uv + vec2(0.0, px.y)).rgb);
    float tr = luma(texture2D(inputTex, uv + vec2(px.x, px.y)).rgb);
    float ml = luma(texture2D(inputTex, uv + vec2(-px.x, 0.0)).rgb);
    float mr = luma(texture2D(inputTex, uv + vec2(px.x, 0.0)).rgb);
    float bl = luma(texture2D(inputTex, uv + vec2(-px.x, -px.y)).rgb);
    float b_ = luma(texture2D(inputTex, uv + vec2(0.0, -px.y)).rgb);
    float br = luma(texture2D(inputTex, uv + vec2(px.x, -px.y)).rgb);

    float gx = -tl - 2.0*ml - bl + tr + 2.0*mr + br;
    float gy = -tl - 2.0*t_ - tr + bl + 2.0*b_ + br;
    float edge = sqrt(gx*gx + gy*gy) * edgeStr * (1.0 + bass * 2.0);
    edge = clamp(edge, 0.0, 1.0);

    vec3 original = texture2D(inputTex, uv).rgb;
    vec3 edgeCol = glowColor.rgb * edge;

    // Glow: cheap 3x3 blur of edge intensity
    if (glowStr > 0.001) {
        float glow = 0.0;
        vec2 gOff = px * 2.0 * glowStr;
        glow += luma(texture2D(inputTex, uv + vec2(-gOff.x, gOff.y)).rgb);
        glow += luma(texture2D(inputTex, uv + vec2(0.0, gOff.y)).rgb);
        glow += luma(texture2D(inputTex, uv + vec2(gOff.x, gOff.y)).rgb);
        glow += luma(texture2D(inputTex, uv + vec2(-gOff.x, 0.0)).rgb);
        glow += luma(texture2D(inputTex, uv + vec2(gOff.x, 0.0)).rgb);
        glow += luma(texture2D(inputTex, uv + vec2(-gOff.x, -gOff.y)).rgb);
        glow += luma(texture2D(inputTex, uv + vec2(0.0, -gOff.y)).rgb);
        glow += luma(texture2D(inputTex, uv + vec2(gOff.x, -gOff.y)).rgb);
        float centerLum = luma(original);
        glow = abs(glow / 8.0 - centerLum) * edgeStr * 4.0;
        glow = clamp(glow, 0.0, 1.0);
        edgeCol += glowColor.rgb * glow * glowStr;
    }

    vec3 col;
    if (invert) {
        col = mix(edgeCol, original, mixOriginal);
    } else {
        col = mix(original * mixOriginal, original + edgeCol, edge);
    }

    if (!hasInput) col = edgeCol;

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
