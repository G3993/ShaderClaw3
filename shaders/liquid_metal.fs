/*{
  "DESCRIPTION": "Liquid Metal — rotational CFD with environmental reflection, bismuth-like surface",
  "CREDIT": "Based on flockaroo's 'Spilled' (Shadertoy), ported to ISF by ShaderClaw",
  "CATEGORIES": ["Generator", "Simulation"],
  "INPUTS": [
    { "NAME": "fluidSpeed", "LABEL": "Fluid Speed", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.5, "MAX": 20.0 },
    { "NAME": "viscosity", "LABEL": "Viscosity", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.0, "MAX": 0.1 },
    { "NAME": "metalColor", "LABEL": "Metal Tint", "TYPE": "color", "DEFAULT": [0.85, 0.88, 0.95, 1.0] },
    { "NAME": "envBright", "LABEL": "Reflection", "TYPE": "float", "DEFAULT": 1.41, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "bumpHeight", "LABEL": "Bump Height", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.001, "MAX": 0.08 },
    { "NAME": "envShift", "LABEL": "Env Hue", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "crunch", "LABEL": "Surface Grain", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.01 },
    { "NAME": "moveMode", "LABEL": "Movement", "TYPE": "long", "VALUES": [0, 1, 2, 3], "LABELS": ["None", "Slow Swirl", "Pulse", "Chaos"], "DEFAULT": 1 },
    { "NAME": "moveSpeed", "LABEL": "Move Speed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.05, "MAX": 2.0 },
    { "NAME": "texBlend", "LABEL": "Tex Blend", "TYPE": "float", "DEFAULT": 0.78, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "texWarp", "LABEL": "Tex Warp", "TYPE": "float", "DEFAULT": 0.51, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "texFeed", "LABEL": "Tex Feed", "TYPE": "float", "DEFAULT": 0.37, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorMix", "LABEL": "Color Mix", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Overlay","Multiply","Screen","Replace"], "DEFAULT": 0 },
    { "NAME": "texScale", "LABEL": "Tex Scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ],
  "PASSES": [
    { "TARGET": "simBuf", "PERSISTENT": true },
    {}
  ]
}*/

#define PI2 6.283185
#define RotNum 5

// ---- Helpers ----
float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// ---- Rotation matrices (precomputed for RotNum=5) ----
float _ang = PI2 / float(RotNum);

vec2 rot2(vec2 v, float a) {
    float c = cos(a), s = sin(a);
    return vec2(c * v.x - s * v.y, s * v.x + c * v.y);
}

// Aspect-correct texture UV -- "contain" mode (native aspect, no squeeze)
vec2 texUV(vec2 coord, float canvasAspect) {
    vec2 st = coord - 0.5;
    float texAspect = IMG_SIZE_inputTex.x / max(IMG_SIZE_inputTex.y, 1.0);
    float ratio = canvasAspect / max(texAspect, 0.001);
    if (ratio > 1.0) st.x *= ratio;
    else             st.y /= ratio;
    st /= texScale;
    st += 0.5;
    if (_flip_inputTex) st.y = 1.0 - st.y;
    return fract(st);
}

vec4 sampleTex(vec2 coord, float canvasAspect) {
    return texture2D(inputTex, texUV(coord, canvasAspect));
}

// ---- Multi-scale rotational curl measurement ----
float getRot(vec2 pos, vec2 b, vec2 Res) {
    vec2 p = b;
    float rotSum = 0.0;
    for (int i = 0; i < RotNum; i++) {
        vec2 samp = texture2D(simBuf, fract((pos + p) / Res)).xy - vec2(0.5);
        rotSum += dot(samp, p.yx * vec2(1.0, -1.0));
        p = rot2(p, _ang);
    }
    return rotSum / float(RotNum) / dot(b, b);
}

void main() {
    vec2 Res = RENDERSIZE;
    vec2 pos = gl_FragCoord.xy;
    vec2 uv = isf_FragNormCoord;
    int mMode = int(moveMode);

    // ===== PASS 0: Fluid Simulation =====
    if (PASSINDEX == 0) {
        // Vary curl evaluation points in time for stochastic smoothing
        vec2 b = cos(float(FRAMEINDEX) * 0.3 - vec2(0.0, 1.57));

        vec2 v = vec2(0.0);
        float bbMax = 0.5 * Res.y;
        bbMax *= bbMax;

        // Multi-scale curl computation (up to 20 octaves)
        for (int l = 0; l < 20; l++) {
            if (dot(b, b) > bbMax) break;
            vec2 p = b;
            for (int i = 0; i < RotNum; i++) {
                v += p.yx * getRot(pos + p, -rot2(b, _ang * 0.5), Res);
                p = rot2(p, _ang);
            }
            b *= 2.0;
        }

        // Self-advection
        float speedScale = fluidSpeed * sqrt(Res.x / 600.0);
        vec2 advUV = fract((pos - v * vec2(-1.0, 1.0) * speedScale) / Res);
        vec4 col = texture2D(simBuf, advUV);

        // Self-consistency: feed computed velocity back into stored field
        col.xy = mix(col.xy, v * vec2(-1.0, 1.0) * sqrt(0.125) * 0.9, viscosity);

        // ---- Mouse interaction ----
        float interacting = max(mouseDown, pinchHold);
        if (interacting > 0.3) {
            vec2 scr = fract((pos - mousePos * Res) / Res.x + 0.5) - 0.5;
            float falloff = 1.0 / (dot(scr, scr) / 0.05 + 0.05);
            col.xy += 0.0003 * mouseDelta * Res * interacting * falloff;
        }

        // ---- Movement modes ----
        float t = TIME * moveSpeed;

        if (mMode == 1) {
            // Slow Swirl — single gentle rotating current in center
            vec2 scr = fract((pos / Res) - 0.5 + 0.5) - 0.5;
            col.xy += 0.003 * cos(t * 0.3 - vec2(0.0, 1.57)) / (dot(scr, scr) / 0.05 + 0.05);
        } else if (mMode == 2) {
            // Pulse — radial breathing from center
            vec2 scr = fract((pos / Res) - 0.5 + 0.5) - 0.5;
            float pulse = sin(t * 0.8) * 0.004;
            col.xy += normalize(scr + 0.001) * pulse / (dot(scr, scr) / 0.03 + 0.1);
        } else if (mMode == 3) {
            // Chaos — multiple random splats
            for (int s = 0; s < 3; s++) {
                float fs = float(s);
                float phase = t * (0.7 + fs * 0.3) + fs * 2.094;
                vec2 splatPos = vec2(
                    0.5 + 0.35 * sin(phase) * cos(phase * 0.7 + fs),
                    0.5 + 0.35 * cos(phase * 0.8) * sin(phase * 0.5 + fs * 1.5)
                ) * Res;
                vec2 scr = fract((pos - splatPos) / Res.x + 0.5) - 0.5;
                vec2 splatVel = vec2(cos(phase * 1.3 + fs), sin(phase * 0.9 + fs * 2.0));
                col.xy += 0.002 * splatVel / (dot(scr, scr) / 0.03 + 0.08);
            }
        }

        // Audio — bass drives radial push
        if (audioBass > 0.2) {
            vec2 scr = fract((pos / Res) - 0.5 + 0.5) - 0.5;
            float pushAngle = hash21(vec2(float(FRAMEINDEX) * 0.1, 1.0)) * PI2;
            col.xy += vec2(cos(pushAngle), sin(pushAngle)) * audioBass * 0.003 / (dot(scr, scr) / 0.04 + 0.06);
        }

        // Surface grain — "crunchy drops" from high-freq noise
        if (crunch > 0.0) {
            float n1 = hash21(pos * 0.35 + TIME * 0.1) - 0.5;
            float n2 = hash21(pos * 0.7 + TIME * 0.07 + 100.0) - 0.5;
            col.zw += vec2(n1, n2) * crunch;
        }

        // Initialization
        if (FRAMEINDEX < 4) {
            col = vec4(0.0);
            // Seed with texture if available
            float aspect = Res.x / Res.y;
            vec4 initTex = sampleTex(uv, aspect);
            if (initTex.a > 0.01) {
                col = (initTex - 0.5) * 0.7;
            }
        }

        // Continuous texture feeding — keeps video colors alive in the fluid
        float aspect = Res.x / Res.y;
        vec4 tex = sampleTex(uv, aspect);
        if (tex.a > 0.01 && texFeed > 0.001) {
            // Warp the texture UV by the fluid velocity for organic morphing
            vec2 warpedUV = fract(uv + v * vec2(-1.0, 1.0) * texWarp * 0.01);
            vec4 warpedTex = sampleTex(warpedUV, aspect);
            // Feed warped texture color back into the simulation
            col.rgb = mix(col.rgb, warpedTex.rgb, texFeed * 0.1);
        }

        gl_FragColor = col;
        return;
    }

    // ===== PASS 1: Liquid Metal Rendering =====

    // Get simulation color
    vec4 simCol = texture2D(simBuf, uv);

    // Calculate surface normal from gradient
    float delta = 1.4 / Res.x;
    float valC = length(texture2D(simBuf, uv).xyz);
    float valL = length(texture2D(simBuf, uv + vec2(-delta, 0.0)).xyz);
    float valR = length(texture2D(simBuf, uv + vec2(delta, 0.0)).xyz);
    float valU = length(texture2D(simBuf, uv + vec2(0.0, delta)).xyz);
    float valD = length(texture2D(simBuf, uv + vec2(0.0, -delta)).xyz);

    vec3 n = normalize(vec3(
        -(valR - valL) * bumpHeight * Res.x,
        -(valU - valD) * bumpHeight * Res.y,
        1.0
    ));

    // Screen-space ray direction
    vec2 sc = (gl_FragCoord.xy - Res * 0.5) / Res.x;
    vec3 viewDir = normalize(vec3(sc, -1.0));

    // Reflection direction
    vec3 R = reflect(viewDir, n);

    // Procedural environment map — gradient sky with metallic color
    float envY = R.y * 0.5 + 0.5; // 0=ground, 1=sky
    float envAngle = atan(R.z, R.x) / PI2 + 0.5;

    // Rich gradient environment
    vec3 skyHigh = hsv2rgb(vec3(0.6 + envShift, 0.3, 1.2));     // bright blue-white sky
    vec3 skyLow = hsv2rgb(vec3(0.55 + envShift, 0.5, 0.8));     // deeper blue at horizon
    vec3 ground = hsv2rgb(vec3(0.08 + envShift, 0.6, 0.15));    // warm dark ground
    vec3 horizon = hsv2rgb(vec3(0.1 + envShift, 0.3, 0.9));     // bright warm horizon line

    vec3 envColor;
    if (envY > 0.52) {
        // Sky — gradient from horizon to zenith
        float t = smoothstep(0.52, 1.0, envY);
        envColor = mix(horizon, skyHigh, t);
        // Add subtle cloud-like variation
        float cloud = sin(envAngle * 12.0 + R.y * 8.0) * 0.5 + 0.5;
        envColor = mix(envColor, skyLow, cloud * 0.2);
    } else if (envY > 0.48) {
        // Horizon band — bright line
        envColor = horizon * 1.3;
    } else {
        // Ground reflection
        float t = smoothstep(0.48, 0.0, envY);
        envColor = mix(horizon * 0.5, ground, t);
    }

    // Apply reflection
    vec3 refl = envColor * envBright;

    // Fluid color contribution — gives the bismuth/oil-slick look
    vec3 fluidCol = simCol.rgb + 0.5;
    fluidCol = mix(vec3(1.0), fluidCol, 0.35);
    fluidCol *= 0.95 + 0.05 * n; // subtle normal-based color shift

    // Final: fluid color modulated by environment reflection
    vec3 finalCol = fluidCol * refl;

    // Tint with metal color
    finalCol *= metalColor.rgb;

    // Texture input — warp UV by fluid velocity for organic morphing
    float aspect = Res.x / Res.y;
    vec2 simVel = simCol.xy;
    vec2 warpedUV = fract(uv + simVel * vec2(-1.0, 1.0) * texWarp * 0.05);
    vec4 texSample = sampleTex(warpedUV, aspect);
    if (texSample.a > 0.01 && texBlend > 0.001) {
        vec3 texCol = texSample.rgb;
        vec3 blended;
        int cm = int(colorMix);
        if (cm == 0) { // Overlay
            blended = finalCol * (1.0 - texBlend) + texCol * finalCol * texBlend * 2.0;
        } else if (cm == 1) { // Multiply
            blended = mix(finalCol, finalCol * texCol, texBlend);
        } else if (cm == 2) { // Screen
            blended = mix(finalCol, 1.0 - (1.0 - finalCol) * (1.0 - texCol), texBlend);
        } else { // Replace
            blended = mix(finalCol, texCol, texBlend);
        }
        finalCol = blended;
    }

    // Add specular highlight
    vec3 lightDir = normalize(vec3(0.5, 0.8, 1.0));
    vec3 halfVec = normalize(lightDir - viewDir);
    float spec = pow(max(dot(n, halfVec), 0.0), 64.0) * 1.5;
    finalCol += vec3(spec) * metalColor.rgb;

    // Audio-reactive brightness pulse
    finalCol += metalColor.rgb * audioBass * 0.1;

    float alpha = 1.0;
    if (transparentBg) {
        float lum = dot(finalCol, vec3(0.299, 0.587, 0.114));
        alpha = smoothstep(0.02, 0.2, lum);
    }

    gl_FragColor = vec4(finalCol, alpha);
}
