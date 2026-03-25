/*{
  "DESCRIPTION": "Liquid Chrome — single-pass rotational CFD with specular lighting, flockaroo's original technique",
  "CREDIT": "Based on flockaroo's single pass CFD (Shadertoy), ported to ISF",
  "CATEGORIES": ["Generator", "Simulation"],
  "INPUTS": [
    { "NAME": "fluidSpeed", "LABEL": "Fluid Speed", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 10.0 },
    { "NAME": "bumpHeight", "LABEL": "Bump", "TYPE": "float", "DEFAULT": 150.0, "MIN": 10.0, "MAX": 500.0 },
    { "NAME": "specAmount", "LABEL": "Specular", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.0, "MAX": 8.0 },
    { "NAME": "specPow", "LABEL": "Spec Power", "TYPE": "float", "DEFAULT": 36.0, "MIN": 4.0, "MAX": 128.0 },
    { "NAME": "diffMin", "LABEL": "Shadow", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "motorStrength", "LABEL": "Motor", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "texBlend", "LABEL": "Tex Blend", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "texWarp", "LABEL": "Tex Warp", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "texFeed", "LABEL": "Tex Feed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorMix", "LABEL": "Color Mix", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Overlay","Multiply","Screen","Replace"], "DEFAULT": 0 },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ],
  "PASSES": [
    { "TARGET": "simBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Flockaroo's original single-pass CFD — RotNum=5, pure rotational self-advection
// No divergence-free field needed — stochastic rotation sampling gives proper mean values

#define RotNum 5
#define PI2 6.283185

float _ang = PI2 / float(RotNum);

float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec2 rot2(vec2 v, float a) {
    float c = cos(a), s = sin(a);
    return vec2(c * v.x - s * v.y, s * v.x + c * v.y);
}

float getRot(vec2 pos, vec2 b, vec2 Res) {
    vec2 p = b;
    float rotSum = 0.0;
    for (int i = 0; i < RotNum; i++) {
        rotSum += dot(texture2D(simBuf, fract((pos + p) / Res)).xy - vec2(0.5), p.yx * vec2(1.0, -1.0));
        p = rot2(p, _ang);
    }
    return rotSum / float(RotNum) / dot(b, b);
}

void main() {
    vec2 Res = RENDERSIZE;
    vec2 pos = gl_FragCoord.xy;
    vec2 uv = isf_FragNormCoord;

    // ===== PASS 0: Fluid Simulation =====
    if (PASSINDEX == 0) {
        float rnd = hash21(vec2(float(FRAMEINDEX) / Res.x, 0.5));

        vec2 b = vec2(cos(_ang * rnd), sin(_ang * rnd));
        vec2 v = vec2(0.0);
        float bbMax = 0.7 * Res.y;
        bbMax *= bbMax;

        for (int l = 0; l < 20; l++) {
            if (dot(b, b) > bbMax) break;
            vec2 p = b;
            for (int i = 0; i < RotNum; i++) {
                // Odd RotNum optimization (no half-angle needed)
                v += p.yx * getRot(pos + p, b, Res);
                p = rot2(p, _ang);
            }
            b *= 2.0;
        }

        // Self-advect
        gl_FragColor = texture2D(simBuf, fract((pos + v * vec2(-1.0, 1.0) * fluidSpeed) / Res));

        // Central motor — slow rotating current
        vec2 scr = (pos / Res) * 2.0 - vec2(1.0);
        gl_FragColor.xy += motorStrength * scr.xy / (dot(scr, scr) / 0.1 + 0.3);

        // Mouse interaction
        float interacting = max(mouseDown, pinchHold);
        if (interacting > 0.3) {
            vec2 mScr = fract((pos - mousePos * Res) / Res.x + 0.5) - 0.5;
            float falloff = 1.0 / (dot(mScr, mScr) / 0.05 + 0.05);
            gl_FragColor.xy += 0.0003 * mouseDelta * Res * interacting * falloff;
        }

        // Audio — bass pushes from center
        if (audioBass > 0.2) {
            vec2 aScr = (pos / Res) * 2.0 - vec2(1.0);
            gl_FragColor.xy += audioBass * 0.005 * aScr / (dot(aScr, aScr) / 0.1 + 0.3);
        }

        // Init with texture or default
        if (FRAMEINDEX <= 4) {
            vec4 initTex = texture2D(inputTex, uv);
            if (initTex.a > 0.01) {
                gl_FragColor = initTex;
            } else {
                // Default: subtle gradient seed
                gl_FragColor = vec4(uv.x * 0.5, uv.y * 0.3, 0.2, 1.0);
            }
        }

        // Continuous texture feeding — keeps video colors alive in the fluid
        vec4 tex = texture2D(inputTex, uv);
        if (tex.a > 0.01 && texFeed > 0.001) {
            // Warp the texture UV by the fluid velocity for organic morphing
            vec2 warpedUV = fract(uv + v * vec2(-1.0, 1.0) * texWarp * 0.01);
            vec4 warpedTex = texture2D(inputTex, warpedUV);
            // Feed warped texture color back into the simulation
            gl_FragColor.rgb = mix(gl_FragColor.rgb, warpedTex.rgb, texFeed * 0.1);
        }

        return;
    }

    // ===== PASS 1: Specular Lighting =====
    float delta = 1.0 / Res.y;
    float valL = length(texture2D(simBuf, uv + vec2(-delta, 0.0)).xyz);
    float valR = length(texture2D(simBuf, uv + vec2(delta, 0.0)).xyz);
    float valU = length(texture2D(simBuf, uv + vec2(0.0, delta)).xyz);
    float valD = length(texture2D(simBuf, uv + vec2(0.0, -delta)).xyz);

    vec3 n = normalize(vec3(valR - valL, valU - valD, 1.0 / bumpHeight * Res.y));

    vec3 light = normalize(vec3(1.0, 1.0, 2.0));
    float diff = clamp(dot(n, light), diffMin, 1.0);
    float spec = pow(clamp(dot(reflect(light, n), vec3(0.0, 0.0, -1.0)), 0.0, 1.0), specPow) * specAmount;

    vec4 col = texture2D(simBuf, uv);

    // Warp texture UV by fluid velocity for organic morphing
    vec2 simVel = col.xy - vec2(0.5);
    vec2 warpedUV = fract(uv + simVel * vec2(-1.0, 1.0) * texWarp * 0.05);

    vec3 final = col.rgb * diff + vec3(spec);

    // Texture color mix with blend modes
    vec4 texSample = texture2D(inputTex, warpedUV);
    if (texSample.a > 0.01 && texBlend > 0.001) {
        vec3 texCol = texSample.rgb;
        vec3 blended;
        int cm = int(colorMix);
        if (cm == 0) { // Overlay
            blended = final * (1.0 - texBlend) + texCol * final * texBlend * 2.0;
        } else if (cm == 1) { // Multiply
            blended = mix(final, final * texCol, texBlend);
        } else if (cm == 2) { // Screen
            blended = mix(final, 1.0 - (1.0 - final) * (1.0 - texCol), texBlend);
        } else { // Replace
            blended = mix(final, texCol, texBlend);
        }
        final = blended;
    }

    float alpha = 1.0;
    if (transparentBg) {
        float lum = dot(final, vec3(0.299, 0.587, 0.114));
        alpha = smoothstep(0.02, 0.15, lum);
    }

    gl_FragColor = vec4(final, alpha);
}
