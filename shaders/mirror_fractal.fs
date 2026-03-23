/*{
  "DESCRIPTION": "Mirror Fractal — recursive mirror reflections that fold space, audio-reactive",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "folds", "LABEL": "Folds", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 8.0 },
    { "NAME": "foldScale", "LABEL": "Scale", "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "rotation", "LABEL": "Rotation", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14, "MAX": 3.14 },
    { "NAME": "drift", "LABEL": "Drift", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float bass = audioBass;
    float t = TIME * drift;

    vec2 p = uv - 0.5;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    p.x *= aspect;

    // Rotate
    float rot = rotation + t * 0.3 + bass * 1.0;
    float c = cos(rot), s = sin(rot);
    p = mat2(c, -s, s, c) * p;

    // Iterative mirror folds
    float maxFolds = folds + bass * 2.0;
    for (int i = 0; i < 8; i++) {
        float fi = float(i);
        float active = step(fi, maxFolds - 0.5);

        // Fold along different axes
        float foldAngle = fi * 1.618 + t * 0.2;
        vec2 axis = vec2(cos(foldAngle), sin(foldAngle));

        // Mirror reflection across axis
        float d = dot(p, axis);
        vec2 folded = p - 2.0 * d * axis;
        p = mix(p, folded, active * step(d, 0.0));

        // Scale down + offset (only when active)
        p = mix(p, p * foldScale, active);
        vec2 off = vec2(0.5 + sin(t + fi) * 0.2, 0.3 + cos(t * 1.3 + fi) * 0.2);
        p = mix(p, p - off, active);
    }

    // Map back to UV
    p.x /= aspect;
    vec2 warpUV = fract(p + 0.5);

    vec3 col;
    if (hasInput) {
        col = texture2D(inputTex, warpUV).rgb;
    } else {
        col = vec3(
            0.5 + 0.5 * sin(p.x * 5.0),
            0.5 + 0.5 * sin(p.y * 7.0 + 2.0),
            0.5 + 0.5 * sin((p.x + p.y) * 3.0 + 4.0)
        );
    }

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
