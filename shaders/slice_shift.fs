/*{
  "DESCRIPTION": "Slice Shift — horizontal slices that offset on beat, glitch datamosh style",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "sliceCount", "LABEL": "Slices", "TYPE": "float", "DEFAULT": 15.0, "MIN": 3.0, "MAX": 60.0 },
    { "NAME": "shiftAmount", "LABEL": "Shift", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "glitchRate", "LABEL": "Glitch Rate", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.5, "MAX": 15.0 },
    { "NAME": "rgbSplit", "LABEL": "RGB Split", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "vertical", "LABEL": "Vertical", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

float hash(float n) { return fract(sin(n) * 43758.5453); }

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float bass = smoothstep(0.0, 0.3, audioBass);
    float mid = smoothstep(0.0, 0.3, audioMid);
    float high = smoothstep(0.0, 0.3, audioHigh);
    // Mid drives glitch rate
    float t = floor(TIME * glitchRate * (1.0 + mid * 3.0));

    float primary = vertical ? uv.x : uv.y;
    float sliceIdx = floor(primary * sliceCount);
    float h = hash(sliceIdx + t * 7.13);
    float h2 = hash(sliceIdx + t * 13.37 + 100.0);

    // Only shift some slices — bass drives shift amount
    float active = step(0.5 - bass * 0.3, h2);
    float shift = (h - 0.5) * 2.0 * shiftAmount * active * (1.0 + bass * 3.0);

    vec2 shiftUV = uv;
    if (vertical) {
        shiftUV.y = fract(uv.y + shift);
    } else {
        shiftUV.x = fract(uv.x + shift);
    }

    vec3 col;
    if (hasInput) {
        if (rgbSplit > 0.001 && active > 0.5) {
            float rs = rgbSplit * 0.02 * (1.0 + high * 4.0);
            vec2 rOff = vertical ? vec2(0.0, rs) : vec2(rs, 0.0);
            float r = texture2D(inputTex, fract(shiftUV + rOff)).r;
            float g = texture2D(inputTex, shiftUV).g;
            float b = texture2D(inputTex, fract(shiftUV - rOff)).b;
            col = vec3(r, g, b);
        } else {
            col = texture2D(inputTex, shiftUV).rgb;
        }
    } else {
        col = vec3(hash(shiftUV.x * 50.0 + shiftUV.y * 70.0 + t));
        col *= vec3(0.8 + 0.2 * sin(sliceIdx), 0.8 + 0.2 * cos(sliceIdx * 1.3), 1.0);
    }

    // Scanline on slice borders
    float border = abs(fract(primary * sliceCount) - 0.5) * 2.0;
    if (border > 0.95 && active > 0.5) col *= 1.3;

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
