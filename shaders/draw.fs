/*{
  "DESCRIPTION": "Draw — interactive painting with emissive glow and 2D light bleeding",
  "CREDIT": "ShaderClaw (inspired by Radiance Cascades by Alexander Sannikov)",
  "CATEGORIES": ["Generator", "Interactive"],
  "INPUTS": [
    { "NAME": "brushSize", "LABEL": "Brush Size", "TYPE": "float", "DEFAULT": 12.0, "MIN": 2.0, "MAX": 60.0 },
    { "NAME": "brushSoft", "LABEL": "Brush Softness", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "brushColor", "LABEL": "Brush Color", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
    { "NAME": "emissive", "LABEL": "Emissive", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "glowRadius", "LABEL": "Glow Radius", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "glowFalloff", "LABEL": "Glow Falloff", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 6.0 },
    { "NAME": "decay", "LABEL": "Fade Speed", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.04, 1.0] },
    { "NAME": "tonemap", "LABEL": "Tonemap", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 5.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
  ],
  "PASSES": [
    { "TARGET": "drawBuf", "PERSISTENT": true },
    { "TARGET": "blurH", "PERSISTENT": true },
    { "TARGET": "blurV", "PERSISTENT": true },
    {}
  ]
}*/

// ---- Pass 0: Drawing accumulation ----

void drawPass(vec2 uv, vec2 px) {
    vec4 prev = texture2D(drawBuf, uv);

    // Fade existing content
    prev.rgb *= 1.0 - decay;
    prev.a = max(prev.a - decay * 0.5, 0.0);

    // Draw when mouse is down
    if (mouseDown > 0.5) {
        vec2 mpos = mousePos * RENDERSIZE;
        float d = length(px - mpos);
        float hardEdge = brushSize * (1.0 - brushSoft * 0.8);
        float softEdge = brushSize;
        float mask = 1.0 - smoothstep(hardEdge, softEdge, d);

        if (mask > 0.0) {
            vec3 col = brushColor.rgb;
            // Store color with emissive multiplier in alpha
            float emAlpha = clamp(emissive / 5.0, 0.0, 1.0);
            prev.rgb = mix(prev.rgb, col, mask);
            prev.a = mix(prev.a, emAlpha, mask);
        }
    }

    gl_FragColor = prev;
}

// ---- Pass 1 & 2: Separable Gaussian blur for glow ----

void blurPassH(vec2 uv) {
    vec2 texel = 1.0 / RENDERSIZE;
    float radius = glowRadius * 20.0;
    vec3 col = vec3(0.0);
    float total = 0.0;

    for (float i = -10.0; i <= 10.0; i += 1.0) {
        float offset = i * texel.x * radius / 10.0;
        vec4 samp = texture2D(drawBuf, uv + vec2(offset, 0.0));
        // Only blur emissive content
        float em = samp.a * emissive;
        float w = exp(-0.5 * (i * i) / 16.0);
        col += samp.rgb * em * w;
        total += w;
    }

    gl_FragColor = vec4(col / total, 1.0);
}

void blurPassV(vec2 uv) {
    vec2 texel = 1.0 / RENDERSIZE;
    float radius = glowRadius * 20.0;
    vec3 col = vec3(0.0);
    float total = 0.0;

    for (float i = -10.0; i <= 10.0; i += 1.0) {
        float offset = i * texel.y * radius / 10.0;
        vec4 samp = texture2D(blurH, uv + vec2(0.0, offset));
        float w = exp(-0.5 * (i * i) / 16.0);
        col += samp.rgb * w;
        total += w;
    }

    gl_FragColor = vec4(col / total, 1.0);
}

// ---- Pass 3: Composite ----

void compositePass(vec2 uv) {
    vec4 draw = texture2D(drawBuf, uv);
    vec3 glow = texture2D(blurV, uv).rgb;

    // Base drawing
    vec3 col = bgColor.rgb;

    // Add the drawn color
    float drawAlpha = length(draw.rgb) > 0.001 ? 1.0 : 0.0;
    col = mix(col, draw.rgb, smoothstep(0.0, 0.01, length(draw.rgb)));

    // Add emissive glow on top
    float glowStr = draw.a * emissive;
    col += glow * glowRadius;

    // Second wider glow pass (reuse blur with offset sampling for extra spread)
    vec2 texel = 1.0 / RENDERSIZE;
    vec3 wideGlow = vec3(0.0);
    float wTotal = 0.0;
    for (float i = -4.0; i <= 4.0; i += 1.0) {
        for (float j = -4.0; j <= 4.0; j += 1.0) {
            float w = exp(-(i * i + j * j) / (glowFalloff * 8.0));
            wideGlow += texture2D(blurV, uv + vec2(i, j) * texel * glowRadius * 8.0).rgb * w;
            wTotal += w;
        }
    }
    col += (wideGlow / wTotal) * glowRadius * 0.5;

    // Tonemap: 1 - 1/(1+x)^n
    col = 1.0 - 1.0 / pow(1.0 + col, vec3(tonemap));

    float alpha = 1.0;
    if (transparentBg) {
        alpha = clamp(dot(col, vec3(0.299, 0.587, 0.114)) * 2.0, 0.0, 1.0);
    }

    gl_FragColor = vec4(col, alpha);
}

// ---- Main dispatcher ----

void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 px = gl_FragCoord.xy;

    if (PASSINDEX == 0) {
        drawPass(uv, px);
    } else if (PASSINDEX == 1) {
        blurPassH(uv);
    } else if (PASSINDEX == 2) {
        blurPassV(uv);
    } else {
        compositePass(uv);
    }
}
