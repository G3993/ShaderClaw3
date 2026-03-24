/*{
  "DESCRIPTION": "Computational Fluid Dynamics — multi-buffer rotational self-advection with mouse/touch painting",
  "CATEGORIES": ["Generator", "Simulation"],
  "INPUTS": [
    { "NAME": "fluidSpeed", "LABEL": "Fluid Speed", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.1, "MAX": 8.0 },
    { "NAME": "viscosity", "LABEL": "Viscosity", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "brushSize", "LABEL": "Brush Size", "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.01, "MAX": 0.2 },
    { "NAME": "brushForce", "LABEL": "Brush Force", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.5, "MAX": 10.0 },
    { "NAME": "specAmount", "LABEL": "Specular", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.0, "MAX": 8.0 },
    { "NAME": "specPow", "LABEL": "Spec Sharpness", "TYPE": "float", "DEFAULT": 36.0, "MIN": 4.0, "MAX": 128.0 },
    { "NAME": "fluidHeight", "LABEL": "Surface Height", "TYPE": "float", "DEFAULT": 150.0, "MIN": 1.0, "MAX": 500.0 },
    { "NAME": "diffMin", "LABEL": "Shadow", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorSat", "LABEL": "Saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "colorCycle", "LABEL": "Color Cycle", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorFloor", "LABEL": "Color Floor", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "movement", "LABEL": "Movement", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "moveSpeed", "LABEL": "Move Speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ],
  "PASSES": [
    { "TARGET": "velBuf", "PERSISTENT": true },
    { "TARGET": "dyeBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Rotational fluid simulation based on Wyatt's technique
// Pass 0: Velocity field (self-advecting rotational curl)
// Pass 1: Dye/color field (advected by velocity)
// Pass 2: Final output with specular lighting

#define ROTNUM 3
#define PI 3.14159265

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

void main() {
    vec2 Res = RENDERSIZE;
    vec2 pos = gl_FragCoord.xy;
    vec2 uv = isf_FragNormCoord;

    // ===== PASS 0: Velocity field =====
    if (PASSINDEX == 0) {
        float ang = 2.0 * PI / float(ROTNUM);
        float ca = cos(ang), sa = sin(ang);
        float cah = cos(ang * 0.5), sah = sin(ang * 0.5);

        // Per-frame random start angle for the sampling pattern
        float rnd = hash21(vec2(float(FRAMEINDEX), 0.37));
        vec2 b = vec2(cos(ang * rnd), sin(ang * rnd));

        vec2 v = vec2(0.0);
        float maxR2 = Res.y * Res.y * 0.25;

        // Multi-scale curl computation (8 octaves)
        for (int level = 0; level < 8; level++) {
            float bb = dot(b, b);
            if (bb > maxR2) break;

            // 3 rotated sample directions
            vec2 d0 = b;
            vec2 d1 = vec2(ca * b.x - sa * b.y, sa * b.x + ca * b.y);
            vec2 d2 = vec2(ca * b.x + sa * b.y, -sa * b.x + ca * b.y);

            // Half-step rotated probes
            vec2 h0 = vec2(cah * b.x - sah * b.y, sah * b.x + cah * b.y);
            vec2 h1 = vec2(ca * h0.x - sa * h0.y, sa * h0.x + ca * h0.y);
            vec2 h2 = vec2(ca * h0.x + sa * h0.y, -sa * h0.x + ca * h0.y);

            // Sample velocity at 3x3 grid of offset+direction combos
            vec2 s0a = texture2D(velBuf, fract((pos + d0 + d0) / Res)).xy - 0.5;
            vec2 s0b = texture2D(velBuf, fract((pos + d0 + d1) / Res)).xy - 0.5;
            vec2 s0c = texture2D(velBuf, fract((pos + d0 + d2) / Res)).xy - 0.5;
            v += d0.yx * (dot(s0a, h0) + dot(s0b, h1) + dot(s0c, h2)) / bb;

            vec2 s1a = texture2D(velBuf, fract((pos + d1 + d0) / Res)).xy - 0.5;
            vec2 s1b = texture2D(velBuf, fract((pos + d1 + d1) / Res)).xy - 0.5;
            vec2 s1c = texture2D(velBuf, fract((pos + d1 + d2) / Res)).xy - 0.5;
            v += d1.yx * (dot(s1a, h0) + dot(s1b, h1) + dot(s1c, h2)) / bb;

            vec2 s2a = texture2D(velBuf, fract((pos + d2 + d0) / Res)).xy - 0.5;
            vec2 s2b = texture2D(velBuf, fract((pos + d2 + d1) / Res)).xy - 0.5;
            vec2 s2c = texture2D(velBuf, fract((pos + d2 + d2) / Res)).xy - 0.5;
            v += d2.yx * (dot(s2a, h0) + dot(s2b, h1) + dot(s2c, h2)) / bb;

            b *= 2.0;
        }

        // Self-advect velocity
        vec2 advUV = fract((pos + v * vec2(-1.0, 1.0) * fluidSpeed) / Res);
        vec4 vel = texture2D(velBuf, advUV);

        // Viscosity damping — pull velocity toward 0.5 (neutral)
        vel.xy = mix(vel.xy, vec2(0.5), viscosity * 0.01);

        // Mouse/touch/hand injection — add velocity where user interacts
        // Accept mouseDown (click/touch) OR pinchHold (hand-as-mouse pinch gesture)
        float interacting = max(mouseDown, pinchHold);
        if (interacting > 0.3) {
            vec2 mDiff = uv - mousePos;
            mDiff.x *= Res.x / Res.y; // aspect correct
            float dist = length(mDiff);
            float falloff = exp(-dist * dist / (brushSize * brushSize));
            // mouseDelta gives drag direction → inject as velocity
            vec2 force = mouseDelta * brushForce * interacting;
            // If delta is tiny (hand hovering), add radial push
            if (length(force) < 0.001) {
                force = normalize(mDiff + 0.001) * 0.02 * interacting;
            }
            // Clamp force to prevent simulation blowup
            force = clamp(force, vec2(-0.3), vec2(0.3));
            vel.xy += force * falloff;
            vel.xy = clamp(vel.xy, 0.0, 1.0);
        }

        // Autonomous movement — random wandering splats
        if (movement) {
            float t = TIME * moveSpeed;
            // 3 wandering points that orbit and create continuous flow
            for (int s = 0; s < 3; s++) {
                float fs = float(s);
                float phase = t * (0.7 + fs * 0.3) + fs * 2.094;
                vec2 splatPos = vec2(
                    0.5 + 0.35 * sin(phase) * cos(phase * 0.7 + fs),
                    0.5 + 0.35 * cos(phase * 0.8) * sin(phase * 0.5 + fs * 1.5)
                );
                vec2 splatVel = vec2(
                    cos(phase * 1.3 + fs) * 0.15,
                    sin(phase * 0.9 + fs * 2.0) * 0.15
                ) * moveSpeed;

                vec2 mDiff = uv - splatPos;
                mDiff.x *= Res.x / Res.y;
                float dist = length(mDiff);
                float splatR = brushSize * 2.0;
                float falloff = exp(-dist * dist / (splatR * splatR));
                vel.xy += splatVel * falloff;
            }
            vel.xy = clamp(vel.xy, 0.0, 1.0);
        }

        // Audio injection — bass creates random splats
        if (audioBass > 0.3) {
            float t = float(FRAMEINDEX) * 0.1;
            vec2 splatPos = vec2(hash21(vec2(t, 1.0)), hash21(vec2(t, 2.0)));
            vec2 mDiff = uv - splatPos;
            mDiff.x *= Res.x / Res.y;
            float dist = length(mDiff);
            float splatR = brushSize * (1.0 + audioBass * 3.0);
            float falloff = exp(-dist * dist / (splatR * splatR));
            float splatAngle = hash21(vec2(t, 3.0)) * 6.283;
            vel.xy += vec2(cos(splatAngle), sin(splatAngle)) * audioBass * 0.3 * falloff;
            vel.xy = clamp(vel.xy, 0.0, 1.0);
        }

        // Seed on first frames
        if (FRAMEINDEX < 3) {
            vel = vec4(0.5, 0.5, 0.0, 1.0);
            // Add some initial swirl
            vec2 d = uv - 0.5;
            vel.xy = vec2(0.5 - d.y * 0.3, 0.5 + d.x * 0.3);
        }

        gl_FragColor = vel;
        return;
    }

    // ===== PASS 1: Dye/color field =====
    if (PASSINDEX == 1) {
        // Read velocity to advect dye
        vec2 vel = (texture2D(velBuf, uv).xy - 0.5) * 2.0;

        // Advect: sample dye from where the fluid came from
        vec2 advUV = fract(uv - vel * fluidSpeed * 0.01);
        vec4 dye = texture2D(dyeBuf, advUV);

        // Slight dissipation
        dye.rgb *= 0.999;

        // Subtle color cycle — gently shifts hue across the fluid, audio speeds it up
        if (colorCycle > 0.001) {
            float cyc = colorCycle * 0.003 * (1.0 + audioBass * 1.5);
            float hue = fract(TIME * colorCycle * 0.08 + uv.x * 0.15 + uv.y * 0.1);
            vec3 tint = hsv2rgb(vec3(hue, 0.6, 1.0));
            dye.rgb = mix(dye.rgb, dye.rgb * tint, cyc);
        }

        // Color floor — prevent going fully dark
        if (colorFloor > 0.001) {
            float lum = dot(dye.rgb, vec3(0.299, 0.587, 0.114));
            float boost = smoothstep(0.0, colorFloor, colorFloor - lum);
            dye.rgb += dye.rgb * boost * 0.5 + vec3(boost * colorFloor * 0.3);
        }

        bool hasInput = IMG_SIZE_inputTex.x > 0.0;

        // Mouse/touch/hand painting — inject color
        float dyeInteracting = max(mouseDown, pinchHold);
        if (dyeInteracting > 0.3) {
            vec2 mDiff = uv - mousePos;
            mDiff.x *= Res.x / Res.y;
            float dist = length(mDiff);
            float falloff = exp(-dist * dist / (brushSize * brushSize)) * dyeInteracting;

            if (hasInput) {
                vec3 texCol = texture2D(inputTex, mousePos).rgb;
                dye.rgb = mix(dye.rgb, texCol, falloff * 0.8);
            } else {
                float hue = fract(TIME * 0.1 + hash21(mousePos * 100.0));
                vec3 paintCol = hsv2rgb(vec3(hue, 0.8, 1.0));
                dye.rgb = mix(dye.rgb, paintCol, falloff * 0.8);
            }
        }

        // Autonomous movement — inject color at wandering points
        if (movement) {
            float t = TIME * moveSpeed;
            for (int s = 0; s < 3; s++) {
                float fs = float(s);
                float phase = t * (0.7 + fs * 0.3) + fs * 2.094;
                vec2 splatPos = vec2(
                    0.5 + 0.35 * sin(phase) * cos(phase * 0.7 + fs),
                    0.5 + 0.35 * cos(phase * 0.8) * sin(phase * 0.5 + fs * 1.5)
                );
                vec2 mDiff = uv - splatPos;
                mDiff.x *= Res.x / Res.y;
                float dist = length(mDiff);
                float splatR = brushSize * 2.5;
                float falloff = exp(-dist * dist / (splatR * splatR)) * 0.15;

                vec3 splatCol;
                if (hasInput) {
                    splatCol = texture2D(inputTex, splatPos).rgb;
                } else {
                    float hue = fract(t * 0.08 + fs * 0.333);
                    splatCol = hsv2rgb(vec3(hue, 0.85, 1.0));
                }
                dye.rgb = mix(dye.rgb, splatCol, falloff);
            }
        }

        // Audio color injection
        if (audioBass > 0.3) {
            float t = float(FRAMEINDEX) * 0.1;
            vec2 splatPos = vec2(hash21(vec2(t, 1.0)), hash21(vec2(t, 2.0)));
            vec2 mDiff = uv - splatPos;
            mDiff.x *= Res.x / Res.y;
            float dist = length(mDiff);
            float splatR = brushSize * (2.0 + audioBass * 5.0);
            float falloff = exp(-dist * dist / (splatR * splatR));

            vec3 splatCol;
            if (hasInput) {
                splatCol = texture2D(inputTex, splatPos).rgb;
            } else {
                float hue = fract(TIME * 0.15 + hash21(vec2(t, 4.0)));
                splatCol = hsv2rgb(vec3(hue, 0.9, 1.0));
            }
            dye.rgb = mix(dye.rgb, splatCol, falloff * audioBass * 0.6);
        }

        // Seed on first frames
        if (FRAMEINDEX < 3) {
            if (hasInput) {
                dye = texture2D(inputTex, uv);
            } else {
                float a = atan(uv.y - 0.5, uv.x - 0.5);
                float d = length(uv - 0.5);
                float hue = fract(a / 6.283 + d * 2.0);
                dye = vec4(hsv2rgb(vec3(hue, 0.7, 0.9)), 1.0);
            }
        }

        dye.a = 1.0;
        gl_FragColor = dye;
        return;
    }

    // ===== PASS 2: Final output with specular lighting =====
    vec4 col = texture2D(dyeBuf, uv);

    // Saturation
    float gray = dot(col.rgb, vec3(0.299, 0.587, 0.114));
    col.rgb = mix(vec3(gray), col.rgb, colorSat);

    // Surface normal from dye luminance gradient
    float delta = 1.0 / Res.y;
    float lL = dot(texture2D(dyeBuf, uv + vec2(-delta, 0.0)).rgb, vec3(0.3, 0.6, 0.1));
    float lR = dot(texture2D(dyeBuf, uv + vec2( delta, 0.0)).rgb, vec3(0.3, 0.6, 0.1));
    float lU = dot(texture2D(dyeBuf, uv + vec2(0.0,  delta)).rgb, vec3(0.3, 0.6, 0.1));
    float lD = dot(texture2D(dyeBuf, uv + vec2(0.0, -delta)).rgb, vec3(0.3, 0.6, 0.1));
    vec3 n = normalize(vec3((lR - lL) * fluidHeight, (lU - lD) * fluidHeight, 1.0));

    // Diffuse + specular
    vec3 lightDir = normalize(vec3(1.0, 1.0, 2.0));
    float diff = clamp(dot(n, lightDir), diffMin, 1.0);
    float spec = pow(clamp(dot(reflect(-lightDir, n), vec3(0.0, 0.0, 1.0)), 0.0, 1.0), specPow) * specAmount;

    vec3 final = col.rgb * diff + vec3(spec);

    float alpha = 1.0;
    if (transparentBg) {
        float lum = dot(final, vec3(0.299, 0.587, 0.114));
        alpha = smoothstep(0.02, 0.15, lum);
    }

    gl_FragColor = vec4(final, alpha);
}
