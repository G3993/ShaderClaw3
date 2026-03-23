/*{
  "DESCRIPTION": "Feedback Trails — ghostly echo trails with color drift, rotation, and zoom feedback",
  "CATEGORIES": ["Effect"],
  "INPUTS": [
    { "NAME": "inputImage", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "trailDecay", "LABEL": "Trail Length", "TYPE": "float", "DEFAULT": 0.92, "MIN": 0.5, "MAX": 0.99 },
    { "NAME": "feedbackZoom", "LABEL": "Zoom", "TYPE": "float", "DEFAULT": 0.005, "MIN": -0.02, "MAX": 0.03 },
    { "NAME": "feedbackRotation", "LABEL": "Spin", "TYPE": "float", "DEFAULT": 0.005, "MIN": -0.05, "MAX": 0.05 },
    { "NAME": "hueDrift", "LABEL": "Hue Drift", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 0.2 },
    { "NAME": "inputMix", "LABEL": "Input Mix", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioDrive", "LABEL": "Audio Drive", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "mirrorMode", "LABEL": "Mirror", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "accentColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ],
  "PASSES": [
    { "TARGET": "trailBuf", "PERSISTENT": true },
    {}
  ]
}*/

#define TAU 6.28318530718

vec3 hueShift(vec3 col, float shift) {
    float angle = shift * TAU;
    float s = sin(angle), c = cos(angle);
    vec3 k = vec3(0.577350269);
    return col * c + cross(k, col) * s + k * dot(k, col) * (1.0 - c);
}

void main() {
    vec2 uv = isf_FragNormCoord;

    float bass = audioBass * audioDrive;
    float level = audioLevel * audioDrive;

    if (PASSINDEX == 0) {
        // --- Feedback accumulation pass ---

        // Feedback UV: zoom toward center + rotate
        vec2 fbUV = uv - 0.5;

        // Zoom — shrink or grow
        float z = 1.0 - feedbackZoom - bass * 0.005;
        fbUV *= z;

        // Rotate
        float rot = feedbackRotation + bass * 0.003;
        float c = cos(rot), s = sin(rot);
        fbUV = mat2(c, -s, s, c) * fbUV;

        fbUV += 0.5;

        // Mirror mode: fold at edges instead of wrapping
        if (mirrorMode) {
            fbUV = abs(mod(fbUV, 2.0) - 1.0);
        } else {
            fbUV = fract(fbUV);
        }

        // Sample previous frame with decay
        vec3 trail = texture2D(trailBuf, fbUV).rgb * trailDecay;

        // Hue drift on the trail
        if (hueDrift > 0.001) {
            trail = hueShift(trail, hueDrift);
        }

        // Mix in fresh input
        vec3 fresh = texture2D(inputImage, uv).rgb;
        vec3 result = trail + fresh * inputMix;

        // Soft clamp to prevent blowout
        result = result / (result + 0.1) * 1.1;

        // On first frames, just show input
        if (FRAMEINDEX < 3) {
            result = fresh;
        }

        gl_FragColor = vec4(result, 1.0);
    }
    else {
        // --- Output pass: read trail buffer + apply final touches ---
        vec3 col = texture2D(trailBuf, uv).rgb;

        // Subtle accent color tint at bright spots
        float lum = dot(col, vec3(0.299, 0.587, 0.114));
        col = mix(col, col * accentColor.rgb, smoothstep(0.3, 0.8, lum) * 0.2);

        float alpha = 1.0;
        if (transparentBg) {
            alpha = smoothstep(0.02, 0.15, lum);
        }

        gl_FragColor = vec4(col, alpha);
    }
}
