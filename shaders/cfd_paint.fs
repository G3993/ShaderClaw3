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
    { "NAME": "movePattern", "LABEL": "Move Pattern", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Freeform", "Center", "Wave", "Vortex", "Pulse"] },
    { "NAME": "moveSpeed", "LABEL": "Move Speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "moveSpread", "LABEL": "Move Spread", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "moveIntensity", "LABEL": "Move Intensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "sourceBlend", "LABEL": "Source Blend", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "blendMode", "LABEL": "Blend Mode", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Warp", "Dissolve", "Luma Map", "Edge Reveal", "Chromatic"] },
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
// Pass 2: Final output with specular lighting + source blending

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

// Inject a velocity splat at splatPos with given direction
void injectSplat(vec2 uv, vec2 Res, vec2 splatPos, vec2 splatVel, float radius, inout vec4 vel) {
    vec2 mDiff = uv - splatPos;
    mDiff.x *= Res.x / Res.y;
    float dist = length(mDiff);
    float falloff = exp(-dist * dist / (radius * radius));
    vel.xy += splatVel * falloff;
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

        float rnd = hash21(vec2(float(FRAMEINDEX), 0.37));
        vec2 b = vec2(cos(ang * rnd), sin(ang * rnd));

        vec2 v = vec2(0.0);
        float maxR2 = Res.y * Res.y * 0.25;

        for (int level = 0; level < 8; level++) {
            float bb = dot(b, b);
            if (bb > maxR2) break;

            vec2 d0 = b;
            vec2 d1 = vec2(ca * b.x - sa * b.y, sa * b.x + ca * b.y);
            vec2 d2 = vec2(ca * b.x + sa * b.y, -sa * b.x + ca * b.y);

            vec2 h0 = vec2(cah * b.x - sah * b.y, sah * b.x + cah * b.y);
            vec2 h1 = vec2(ca * h0.x - sa * h0.y, sa * h0.x + ca * h0.y);
            vec2 h2 = vec2(ca * h0.x + sa * h0.y, -sa * h0.x + ca * h0.y);

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

        // Viscosity damping
        vel.xy = mix(vel.xy, vec2(0.5), viscosity * 0.01);

        // Mouse/touch/hand injection
        float interacting = max(mouseDown, pinchHold);
        if (interacting > 0.3) {
            vec2 mDiff = uv - mousePos;
            mDiff.x *= Res.x / Res.y;
            float dist = length(mDiff);
            float falloff = exp(-dist * dist / (brushSize * brushSize));
            vec2 force = mouseDelta * brushForce * interacting;
            if (length(force) < 0.001) {
                force = normalize(mDiff + 0.001) * 0.02 * interacting;
            }
            force = clamp(force, vec2(-0.3), vec2(0.3));
            vel.xy += force * falloff;
            vel.xy = clamp(vel.xy, 0.0, 1.0);
        }

        // ---- Movement patterns ----
        float t = TIME * moveSpeed;
        float spread = mix(0.05, 0.42, moveSpread);
        float intensity = moveIntensity * 0.2;
        int splatCount = 5;

        if (movePattern < 0.5) {
            // Freeform — wandering orbits (original but more splats)
            for (int s = 0; s < 5; s++) {
                float fs = float(s);
                float phase = t * (0.5 + fs * 0.25) + fs * 1.257;
                vec2 splatPos = vec2(
                    0.5 + spread * sin(phase) * cos(phase * 0.7 + fs),
                    0.5 + spread * cos(phase * 0.8) * sin(phase * 0.5 + fs * 1.5)
                );
                vec2 splatVel = vec2(
                    cos(phase * 1.3 + fs) * intensity,
                    sin(phase * 0.9 + fs * 2.0) * intensity
                );
                injectSplat(uv, Res, splatPos, splatVel, brushSize * 2.0, vel);
            }
        } else if (movePattern < 1.5) {
            // Center — concentrated radial pulses from center
            for (int s = 0; s < 5; s++) {
                float fs = float(s);
                float phase = t * (0.8 + fs * 0.2) + fs * 1.257;
                float r = spread * 0.5 * (0.3 + 0.7 * abs(sin(phase * 0.5)));
                float a = phase * 0.7 + fs * 1.257;
                vec2 splatPos = vec2(0.5 + cos(a) * r, 0.5 + sin(a) * r);
                // Radial push outward from center
                vec2 dir = normalize(splatPos - 0.5 + 0.001);
                float pushPull = sin(phase * 1.5); // oscillate between push and pull
                vec2 splatVel = dir * pushPull * intensity * 1.5;
                injectSplat(uv, Res, splatPos, splatVel, brushSize * 2.5, vel);
            }
            // Central vortex
            vec2 cDiff = uv - 0.5;
            cDiff.x *= Res.x / Res.y;
            float cDist = length(cDiff);
            float centralFalloff = exp(-cDist * cDist / (spread * spread * 0.3));
            float spin = sin(t * 0.6) * intensity * 0.5;
            vel.xy += vec2(-cDiff.y, cDiff.x) * spin * centralFalloff;
        } else if (movePattern < 2.5) {
            // Wave — horizontal sweeps with vertical sine modulation
            for (int s = 0; s < 6; s++) {
                float fs = float(s);
                float wavePhase = t * 0.8 + fs * 0.5;
                // Sweep x position across canvas in a sine wave
                float x = 0.5 + spread * sin(wavePhase * 0.6 + fs * 0.8);
                // Y follows a different wave
                float y = 0.5 + spread * 0.7 * sin(wavePhase * 1.1 + fs * 1.7);
                vec2 splatPos = vec2(x, y);
                // Velocity follows the wave direction
                float waveAngle = wavePhase * 0.6 + fs * 0.8;
                vec2 splatVel = vec2(
                    cos(waveAngle) * intensity * 1.2,
                    sin(wavePhase * 2.2 + fs) * intensity * 0.6
                );
                injectSplat(uv, Res, splatPos, splatVel, brushSize * 2.2, vel);
            }
            // Global wave field — gentle sine distortion across entire surface
            float waveField = sin(uv.x * 8.0 + t * 1.5) * sin(uv.y * 6.0 - t * 0.8);
            vel.xy += vec2(waveField, cos(uv.x * 5.0 - t)) * intensity * 0.15;
        } else if (movePattern < 3.5) {
            // Vortex — spinning tornado patterns
            for (int s = 0; s < 4; s++) {
                float fs = float(s);
                float vortexPhase = t * (0.6 + fs * 0.15);
                float r = spread * (0.15 + fs * 0.12);
                vec2 center = vec2(
                    0.5 + spread * 0.3 * sin(t * 0.3 + fs),
                    0.5 + spread * 0.3 * cos(t * 0.25 + fs * 1.5)
                );
                vec2 splatPos = center + vec2(cos(vortexPhase), sin(vortexPhase)) * r;
                // Tangential velocity (perpendicular to radius)
                vec2 radial = splatPos - center;
                vec2 tangent = vec2(-radial.y, radial.x);
                vec2 splatVel = tangent * intensity * 2.0 / (r + 0.05);
                injectSplat(uv, Res, splatPos, splatVel, brushSize * 1.8, vel);
            }
            // Global rotation field
            vec2 gDiff = uv - 0.5;
            gDiff.x *= Res.x / Res.y;
            float gDist = length(gDiff);
            float rotFalloff = exp(-gDist * gDist / (spread * spread));
            float rotDir = sin(t * 0.4) > 0.0 ? 1.0 : -1.0;
            vel.xy += vec2(-gDiff.y, gDiff.x) * rotDir * intensity * 0.3 * rotFalloff;
        } else if (movePattern < 4.5) {
            // Pulse — rhythmic expanding rings from shifting centers
            float pulseRate = 2.0 * moveSpeed;
            for (int s = 0; s < 3; s++) {
                float fs = float(s);
                float pulse = fract(t * pulseRate * (0.3 + fs * 0.15) + fs * 0.333);
                float pulseR = pulse * spread * 1.5;
                vec2 center = vec2(
                    0.5 + spread * 0.4 * sin(t * 0.2 + fs * 2.094),
                    0.5 + spread * 0.4 * cos(t * 0.15 + fs * 2.094)
                );
                // Ring of splats at pulse radius
                for (int a = 0; a < 6; a++) {
                    float fa = float(a);
                    float angle = fa * 1.047 + fs * 0.5;
                    vec2 splatPos = center + vec2(cos(angle), sin(angle)) * pulseR;
                    vec2 dir = normalize(splatPos - center + 0.001);
                    float strength = (1.0 - pulse) * intensity * 1.5; // stronger at start
                    injectSplat(uv, Res, splatPos, dir * strength, brushSize * (1.5 + pulse * 2.0), vel);
                }
            }
        }

        vel.xy = clamp(vel.xy, 0.0, 1.0);

        // Audio injection
        if (audioBass > 0.3) {
            float at = float(FRAMEINDEX) * 0.1;
            vec2 splatPos = vec2(hash21(vec2(at, 1.0)), hash21(vec2(at, 2.0)));
            vec2 mDiff = uv - splatPos;
            mDiff.x *= Res.x / Res.y;
            float dist = length(mDiff);
            float splatR = brushSize * (1.0 + audioBass * 3.0);
            float falloff = exp(-dist * dist / (splatR * splatR));
            float splatAngle = hash21(vec2(at, 3.0)) * 6.283;
            vel.xy += vec2(cos(splatAngle), sin(splatAngle)) * audioBass * 0.3 * falloff;
            vel.xy = clamp(vel.xy, 0.0, 1.0);
        }

        // Seed
        if (FRAMEINDEX < 3) {
            vel = vec4(0.5, 0.5, 0.0, 1.0);
            vec2 d = uv - 0.5;
            vel.xy = vec2(0.5 - d.y * 0.3, 0.5 + d.x * 0.3);
        }

        gl_FragColor = vel;
        return;
    }

    // ===== PASS 1: Dye/color field =====
    if (PASSINDEX == 1) {
        vec2 vel = (texture2D(velBuf, uv).xy - 0.5) * 2.0;
        vec2 advUV = fract(uv - vel * fluidSpeed * 0.01);
        vec4 dye = texture2D(dyeBuf, advUV);

        // Dissipation — stronger when sourceBlend is high to let fresh source bleed in
        float dissipation = mix(0.999, 0.98, sourceBlend * sourceBlend);
        dye.rgb *= dissipation;

        // Color cycle
        if (colorCycle > 0.001) {
            float cyc = colorCycle * 0.003 * (1.0 + audioBass * 1.5);
            float hue = fract(TIME * colorCycle * 0.08 + uv.x * 0.15 + uv.y * 0.1);
            vec3 tint = hsv2rgb(vec3(hue, 0.6, 1.0));
            dye.rgb = mix(dye.rgb, dye.rgb * tint, cyc);
        }

        // Color floor
        if (colorFloor > 0.001) {
            float lum = dot(dye.rgb, vec3(0.299, 0.587, 0.114));
            float boost = smoothstep(0.0, colorFloor, colorFloor - lum);
            dye.rgb += dye.rgb * boost * 0.5 + vec3(boost * colorFloor * 0.3);
        }

        bool hasInput = IMG_SIZE_inputTex.x > 0.0;

        // When sourceBlend is active, continuously re-inject source color
        // weighted by velocity — calm areas get more source, active areas stay fluid
        if (hasInput && sourceBlend > 0.001) {
            float velMag = length(vel);
            // Calm areas re-absorb source faster
            float reinjection = sourceBlend * 0.08 * (1.0 - smoothstep(0.0, 0.8, velMag));
            vec3 srcCol = texture2D(inputTex, uv).rgb;
            dye.rgb = mix(dye.rgb, srcCol, reinjection);
        }

        // Mouse/touch painting
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

        // Movement color injection (uses same pattern logic via splat positions)
        float t = TIME * moveSpeed;
        float spread = mix(0.05, 0.42, moveSpread);

        // Generate splat positions matching velocity pass patterns
        for (int s = 0; s < 5; s++) {
            float fs = float(s);
            vec2 splatPos;

            if (movePattern < 0.5) {
                float phase = t * (0.5 + fs * 0.25) + fs * 1.257;
                splatPos = vec2(
                    0.5 + spread * sin(phase) * cos(phase * 0.7 + fs),
                    0.5 + spread * cos(phase * 0.8) * sin(phase * 0.5 + fs * 1.5)
                );
            } else if (movePattern < 1.5) {
                float phase = t * (0.8 + fs * 0.2) + fs * 1.257;
                float r = spread * 0.5 * (0.3 + 0.7 * abs(sin(phase * 0.5)));
                float a = phase * 0.7 + fs * 1.257;
                splatPos = vec2(0.5 + cos(a) * r, 0.5 + sin(a) * r);
            } else if (movePattern < 2.5) {
                float wavePhase = t * 0.8 + fs * 0.5;
                splatPos = vec2(
                    0.5 + spread * sin(wavePhase * 0.6 + fs * 0.8),
                    0.5 + spread * 0.7 * sin(wavePhase * 1.1 + fs * 1.7)
                );
            } else if (movePattern < 3.5) {
                float vortexPhase = t * (0.6 + fs * 0.15);
                float r = spread * (0.15 + fs * 0.12);
                vec2 center = vec2(
                    0.5 + spread * 0.3 * sin(t * 0.3 + fs),
                    0.5 + spread * 0.3 * cos(t * 0.25 + fs * 1.5)
                );
                splatPos = center + vec2(cos(vortexPhase), sin(vortexPhase)) * r;
            } else {
                // Pulse — use center positions
                splatPos = vec2(
                    0.5 + spread * 0.4 * sin(t * 0.2 + fs * 2.094),
                    0.5 + spread * 0.4 * cos(t * 0.15 + fs * 2.094)
                );
            }

            vec2 mDiff = uv - splatPos;
            mDiff.x *= Res.x / Res.y;
            float dist = length(mDiff);
            float splatR = brushSize * 2.5;
            float falloff = exp(-dist * dist / (splatR * splatR)) * 0.15 * moveIntensity;

            vec3 splatCol;
            if (hasInput) {
                splatCol = texture2D(inputTex, splatPos).rgb;
            } else {
                float hue = fract(t * 0.08 + fs * 0.2);
                splatCol = hsv2rgb(vec3(hue, 0.85, 1.0));
            }
            dye.rgb = mix(dye.rgb, splatCol, falloff);
        }

        // Audio color injection
        if (audioBass > 0.3) {
            float at = float(FRAMEINDEX) * 0.1;
            vec2 splatPos = vec2(hash21(vec2(at, 1.0)), hash21(vec2(at, 2.0)));
            vec2 mDiff = uv - splatPos;
            mDiff.x *= Res.x / Res.y;
            float dist = length(mDiff);
            float splatR = brushSize * (2.0 + audioBass * 5.0);
            float falloff = exp(-dist * dist / (splatR * splatR));

            vec3 splatCol;
            if (hasInput) {
                splatCol = texture2D(inputTex, splatPos).rgb;
            } else {
                float hue = fract(TIME * 0.15 + hash21(vec2(at, 4.0)));
                splatCol = hsv2rgb(vec3(hue, 0.9, 1.0));
            }
            dye.rgb = mix(dye.rgb, splatCol, falloff * audioBass * 0.6);
        }

        // Seed
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

    // ===== PASS 2: Final output with specular lighting + source blending =====
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

    vec3 fluid = col.rgb * diff + vec3(spec);

    // ---- Source blending ----
    if (sourceBlend > 0.001 && IMG_SIZE_inputTex.x > 0.0) {
        vec2 vel = (texture2D(velBuf, uv).xy - 0.5) * 2.0;
        float velMag = length(vel);

        // Warp UVs by velocity for all modes that need it
        float warpAmt = (1.0 - sourceBlend * 0.7) * fluidSpeed * 0.12;
        vec2 warpedUV = fract(uv + vel * warpAmt);

        vec3 src = texture2D(inputTex, warpedUV).rgb;
        float srcLum = dot(src, vec3(0.299, 0.587, 0.114));
        float fluidLum = dot(fluid, vec3(0.299, 0.587, 0.114));

        // Velocity gradient (edge detection)
        vec2 velL = (texture2D(velBuf, uv + vec2(-delta, 0.0)).xy - 0.5) * 2.0;
        vec2 velR = (texture2D(velBuf, uv + vec2( delta, 0.0)).xy - 0.5) * 2.0;
        vec2 velU = (texture2D(velBuf, uv + vec2(0.0,  delta)).xy - 0.5) * 2.0;
        vec2 velD = (texture2D(velBuf, uv + vec2(0.0, -delta)).xy - 0.5) * 2.0;
        float velEdge = length(velR - velL) + length(velU - velD);

        vec3 blended;

        if (blendMode < 0.5) {
            // WARP — source warped by fluid, mixed with velocity-aware mask
            // More motion = more fluid visible, calm = more source
            float motionMask = smoothstep(0.0, 0.6, velMag);
            float blend = sourceBlend * (1.0 - motionMask * 0.7);
            // Apply fluid lighting to source for cohesion
            vec3 litSrc = src * mix(1.0, diff, 0.6) + vec3(spec * 0.3);
            blended = mix(fluid, litSrc, blend);

        } else if (blendMode < 1.5) {
            // DISSOLVE — noise-based dissolve driven by velocity
            float noise = hash21(pos * 0.01 + vel * 5.0 + TIME * 0.1);
            // Threshold shifts with sourceBlend and local velocity
            float threshold = sourceBlend - velMag * 0.5;
            float dissolveMask = smoothstep(threshold - 0.1, threshold + 0.1, noise);
            vec3 litSrc = src * mix(1.0, diff, 0.4) + vec3(spec * 0.2);
            blended = mix(litSrc, fluid, dissolveMask);
            // Glow at dissolve edges
            float edgeGlow = smoothstep(0.0, 0.15, abs(noise - threshold));
            blended += (1.0 - edgeGlow) * mix(src, fluid, 0.5) * 0.3;

        } else if (blendMode < 2.5) {
            // LUMA MAP — fluid luminance reveals source, dark fluid areas show source through
            float lumMask = smoothstep(0.1, 0.6, fluidLum);
            float blend = sourceBlend * (1.0 - lumMask);
            // Tint the source with fluid color for organic feel
            vec3 tintedSrc = mix(src, src * (col.rgb / (fluidLum + 0.1)), 0.3);
            blended = mix(fluid, tintedSrc * diff + vec3(spec * 0.2), blend);

        } else if (blendMode < 3.5) {
            // EDGE REVEAL — source bleeds through at velocity boundaries
            float edgeMask = smoothstep(0.1, 0.8, velEdge * 3.0);
            // Combine with calm-area reveal
            float calmMask = 1.0 - smoothstep(0.0, 0.3, velMag);
            float revealMask = max(edgeMask, calmMask * 0.5) * sourceBlend;
            vec3 litSrc = src * diff + vec3(spec * 0.15);
            blended = mix(fluid, litSrc, revealMask);
            // Subtle color bleed at edges
            blended += edgeMask * mix(src, fluid, 0.5) * 0.15 * sourceBlend;

        } else {
            // CHROMATIC — RGB channels warped at different amounts
            float chromaSpread = (1.0 - sourceBlend * 0.5) * fluidSpeed * 0.08;
            vec2 uvR = fract(uv + vel * (warpAmt + chromaSpread));
            vec2 uvG = fract(uv + vel * warpAmt);
            vec2 uvB = fract(uv + vel * (warpAmt - chromaSpread));
            vec3 chromaSrc = vec3(
                texture2D(inputTex, uvR).r,
                texture2D(inputTex, uvG).g,
                texture2D(inputTex, uvB).b
            );
            // Motion mask — more chromatic separation in active areas
            float motionMask = smoothstep(0.0, 0.5, velMag);
            vec3 litSrc = chromaSrc * mix(1.0, diff, 0.5) + vec3(spec * 0.25);
            blended = mix(fluid, litSrc, sourceBlend);
            // Extra chromatic fringing at high velocity
            blended += (chromaSrc - src) * motionMask * 0.3;
        }

        fluid = blended;
    }

    float alpha = 1.0;
    if (transparentBg) {
        float lum = dot(fluid, vec3(0.299, 0.587, 0.114));
        alpha = smoothstep(0.02, 0.15, lum);
    }

    gl_FragColor = vec4(fluid, alpha);
}
