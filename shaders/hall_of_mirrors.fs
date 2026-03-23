/*{
  "DESCRIPTION": "Hall of Mirrors — infinite recursive zoom with rotation, creates fractal tunnel from any input",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "zoomRate", "LABEL": "Zoom Rate", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "rotRate", "LABEL": "Rotation", "TYPE": "float", "DEFAULT": 0.2, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "layers", "LABEL": "Layers", "TYPE": "float", "DEFAULT": 5.0, "MIN": 2.0, "MAX": 10.0 },
    { "NAME": "layerOpacity", "LABEL": "Layer Opacity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 1.0 },
    { "NAME": "tintShift", "LABEL": "Color Shift", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float bass = audioBass;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    vec2 center = mousePos;
    float t = TIME;

    vec3 col = vec3(0.0);
    float totalAlpha = 0.0;

    for (int i = 0; i < 10; i++) {
        float fi = float(i);
        float active = step(fi, layers - 0.5);
        float depth = fi / layers;

        // Each layer is progressively zoomed and rotated
        float sc = pow(zoomRate + 0.3, fi) * (1.0 + bass * 0.2 * fi);
        float rot = fi * rotRate * (1.0 + bass * 0.5) + t * rotRate * 0.3;
        float c = cos(rot), s = sin(rot);

        vec2 p = uv - center;
        p.x *= aspect;
        p = mat2(c, -s, s, c) * p;
        p /= sc;
        p.x /= aspect;
        p += center;

        vec2 layerUV = fract(p);

        vec3 layerCol;
        if (hasInput) {
            layerCol = texture2D(inputTex, layerUV).rgb;
        } else {
            float check = mod(floor(layerUV.x * 8.0) + floor(layerUV.y * 8.0), 2.0);
            layerCol = mix(vec3(0.15), vec3(0.6), check);
        }

        // Tint shift per layer
        if (tintShift > 0.001) {
            float hueRot = fi * tintShift * 0.5;
            float ch = cos(hueRot), sh = sin(hueRot);
            layerCol.rg = mat2(ch, -sh, sh, ch) * layerCol.rg;
        }

        float w = pow(layerOpacity, fi) * active;
        col += layerCol * w;
        totalAlpha += w;
    }

    col /= max(totalAlpha, 0.001);

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
