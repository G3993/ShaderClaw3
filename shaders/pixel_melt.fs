/*{
  "DESCRIPTION": "Pixel Melt — pixels drip downward based on brightness, audio-reactive meltdown",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "meltAmount", "LABEL": "Melt", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "meltSpeed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "pixelSize", "LABEL": "Pixel Size", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "heatColor", "LABEL": "Heat Tint", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float bass = audioBass;
    float t = TIME * meltSpeed;
    float melt = meltAmount * (1.0 + bass * 3.0);

    // Optional pixelation
    vec2 sampleUV = uv;
    if (pixelSize > 0.01) {
        float ps = mix(RENDERSIZE.y, 20.0, pixelSize);
        sampleUV = floor(uv * ps) / ps;
    }

    // Sample brightness above current pixel to determine drip
    vec2 drip = vec2(0.0);
    float totalWeight = 0.0;
    for (int i = 1; i <= 8; i++) {
        float fi = float(i);
        vec2 above = sampleUV + vec2(0.0, fi * 0.02);
        float valid = step(above.y, 1.0);
        vec3 aboveCol = hasInput ? texture2D(inputTex, clamp(above, 0.0, 1.0)).rgb : vec3(0.5);
        float lum = dot(aboveCol, vec3(0.299, 0.587, 0.114));
        float h = hash(floor(above * 200.0) + floor(t * 2.0));
        float w = lum * (1.0 - fi / 8.0) * valid;
        drip.y += w * melt * (0.5 + h * 0.5);
        totalWeight += w;
    }
    if (totalWeight > 0.0) drip /= totalWeight;

    // Add horizontal wobble
    drip.x = sin(sampleUV.y * 30.0 + t * 3.0) * melt * 0.2;

    vec2 finalUV = clamp(sampleUV + drip, 0.0, 1.0);

    vec3 col;
    if (hasInput) {
        col = texture2D(inputTex, finalUV).rgb;
    } else {
        col = vec3(hash(finalUV * 50.0 + t), hash(finalUV * 60.0 + t + 1.0), hash(finalUV * 70.0 + t + 2.0));
    }

    // Heat tint on melted areas
    float meltStr = length(drip) * 10.0;
    col = mix(col, col * heatColor.rgb, clamp(meltStr, 0.0, 0.5));

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
