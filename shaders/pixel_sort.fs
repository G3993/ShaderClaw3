/*{
  "DESCRIPTION": "Pixel Sort — glitchy datamosh-style streaking with threshold control and directional sorting",
  "CATEGORIES": ["Effect"],
  "INPUTS": [
    { "NAME": "inputImage", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "sortStrength", "LABEL": "Strength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "threshold", "LABEL": "Threshold", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "sortDirection", "LABEL": "Direction", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Down", "Right", "Up", "Diagonal"] },
    { "NAME": "streakLength", "LABEL": "Streak Length", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.01, "MAX": 0.5 },
    { "NAME": "glitchRate", "LABEL": "Glitch Rate", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorBleed", "LABEL": "Color Bleed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioDrive", "LABEL": "Audio Drive", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "accentColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

float hash(float n) { return fract(sin(n) * 43758.5453); }
float hash2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float luma(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }

// Fallback pattern when no texture is bound
vec3 fallbackColor(vec2 uv) {
    float g = hash2(floor(uv * 80.0)) * 0.5 + 0.25;
    float stripe = step(0.5, fract(uv.y * 40.0 + TIME * 0.5));
    return accentColor.rgb * g * (0.6 + stripe * 0.4);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 texel = 1.0 / RENDERSIZE;
    bool hasInput = IMG_SIZE_inputImage.x > 0.0;

    float bass = audioBass * audioDrive;
    float high = audioHigh * audioDrive;

    // Sort direction vector
    int sDir = int(sortDirection);
    vec2 dir;
    if (sDir == 0) dir = vec2(0.0, -1.0);       // Down
    else if (sDir == 1) dir = vec2(1.0, 0.0);    // Right
    else if (sDir == 2) dir = vec2(0.0, 1.0);    // Up
    else dir = normalize(vec2(1.0, -1.0));       // Diagonal

    // Glitch: randomize sort bands per-frame-group
    float timeSlice = floor(TIME * (2.0 + glitchRate * 10.0));
    float bandHash = hash2(vec2(floor(uv.y * 80.0), timeSlice));
    float bandActive = step(1.0 - sortStrength - bass * 0.3, bandHash);

    // Threshold: only sort pixels above brightness threshold
    vec3 origCol = hasInput ? texture2D(inputImage, uv).rgb : fallbackColor(uv);
    float origLuma = luma(origCol);

    float thresh = threshold - bass * 0.15;
    bool aboveThreshold = origLuma > thresh;

    vec3 col = origCol;

    if (bandActive > 0.5 && aboveThreshold) {
        // Walk along sort direction, find the brightest/darkest pixel in the streak
        float len = streakLength * (1.0 + bass * 0.5);
        int steps = int(min(len * max(RENDERSIZE.x, RENDERSIZE.y), 64.0));

        vec3 sortedCol = origCol;
        float sortedLuma = origLuma;

        // Accumulate: smear toward brightest pixel in the streak direction
        float weight = 1.0;
        float totalWeight = 1.0;

        for (int i = 1; i < 64; i++) {
            if (i >= steps) break;
            float fi = float(i);
            vec2 sampleUV = uv + dir * texel * fi;

            // Stay in bounds
            if (sampleUV.x < 0.0 || sampleUV.x > 1.0 || sampleUV.y < 0.0 || sampleUV.y > 1.0) break;

            vec3 s = hasInput ? texture2D(inputImage, sampleUV).rgb : fallbackColor(sampleUV);
            float sLuma = luma(s);

            // Only continue streak if above threshold
            if (sLuma < thresh) break;

            // Weight falls off with distance
            float w = 1.0 - fi / float(steps);
            w *= w;
            sortedCol += s * w;
            totalWeight += w;
        }

        sortedCol /= totalWeight;

        // Color bleed: shift toward accent on bright streaks
        if (colorBleed > 0.001) {
            float bleedAmt = smoothstep(0.5, 0.9, sortedLuma) * colorBleed;
            sortedCol = mix(sortedCol, sortedCol * accentColor.rgb * 2.0, bleedAmt * 0.5);
        }

        col = sortedCol;
    }

    // Chromatic aberration on glitch bands
    if (bandActive > 0.5 && high > 0.1) {
        float aberr = high * 0.003;
        vec3 _ar = hasInput ? texture2D(inputImage, uv + dir * aberr).rgb : fallbackColor(uv + dir * aberr);
        vec3 _ab = hasInput ? texture2D(inputImage, uv - dir * aberr).rgb : fallbackColor(uv - dir * aberr);
        col.r = _ar.r * 0.3 + col.r * 0.7;
        col.b = _ab.b * 0.3 + col.b * 0.7;
    }

    // Scanline noise on active bands
    float scanline = hash2(vec2(uv.y * RENDERSIZE.y, timeSlice));
    if (bandActive > 0.5 && scanline > 0.95) {
        col = mix(col, vec3(luma(col)), 0.5);
    }

    float alpha = 1.0;
    if (transparentBg) {
        alpha = smoothstep(0.02, 0.15, luma(col));
    }

    gl_FragColor = vec4(col, alpha);
}
