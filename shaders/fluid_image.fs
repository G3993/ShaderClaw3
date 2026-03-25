/*{
  "DESCRIPTION": "UV-advection fluid sim — warp any image/video with fluid dynamics, image stays crisp and returns when fluid settles",
  "CREDIT": "Based on Paketa12/Bruno Imbrizi UV-advection technique, ported to ISF by ShaderClaw",
  "CATEGORIES": ["VFX", "Simulation"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Image/Video", "TYPE": "image" },
    { "NAME": "fluidSpeed", "LABEL": "Fluid Speed", "TYPE": "float", "DEFAULT": 5.0, "MIN": 0.5, "MAX": 20.0 },
    { "NAME": "advectSpeed", "LABEL": "Advect Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "returnRate", "LABEL": "Return Rate", "TYPE": "float", "DEFAULT": 0.005, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "vorticity", "LABEL": "Vorticity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "viscosity", "LABEL": "Viscosity", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "splatForce", "LABEL": "Splat Force", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "splatRadius", "LABEL": "Splat Radius", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.01, "MAX": 0.2 },
    { "NAME": "bumpHeight", "LABEL": "Surface Depth", "TYPE": "float", "DEFAULT": 80.0, "MIN": 0.0, "MAX": 300.0 },
    { "NAME": "specAmount", "LABEL": "Specular", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "specPow", "LABEL": "Spec Power", "TYPE": "float", "DEFAULT": 36.0, "MIN": 4.0, "MAX": 128.0 },
    { "NAME": "showUV", "LABEL": "Show UV", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "moveMode", "LABEL": "Movement", "TYPE": "long", "VALUES": [0,1,2], "LABELS": ["None","Swirl","Pulse"], "DEFAULT": 0 },
    { "NAME": "moveSpeed", "LABEL": "Move Speed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.05, "MAX": 2.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ],
  "PASSES": [
    { "TARGET": "velBuf", "PERSISTENT": true },
    { "TARGET": "uvBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Fluid Image — UV-advection fluid simulation
// Pass 0: Velocity field (rotational self-advection, RotNum=5)
// Pass 1: UV advection buffer (stores displaced UV coordinates)
// Pass 2: Final render (sample original image at warped UVs + surface lighting)

#define PI2 6.283185
#define RotNum 5

// ---- Helpers ----
float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

// ---- Rotation ----
float _ang = PI2 / float(RotNum);

vec2 rot2(vec2 v, float a) {
    float c = cos(a), s = sin(a);
    return vec2(c * v.x - s * v.y, s * v.x + c * v.y);
}

// ---- Multi-scale rotational curl measurement ----
float getRot(vec2 pos, vec2 b, vec2 Res) {
    vec2 p = b;
    float rotSum = 0.0;
    for (int i = 0; i < RotNum; i++) {
        vec2 samp = texture2D(velBuf, fract((pos + p) / Res)).xy - vec2(0.5);
        rotSum += dot(samp, p.yx * vec2(1.0, -1.0));
        p = rot2(p, _ang);
    }
    return rotSum / float(RotNum) / dot(b, b);
}

void main() {
    vec2 Res = RENDERSIZE;
    vec2 pos = gl_FragCoord.xy;
    vec2 uv = isf_FragNormCoord;
    float aspect = Res.x / Res.y;
    int mMode = int(moveMode);

    // ===== PASS 0: Velocity Field =====
    if (PASSINDEX == 0) {
        // Stochastic rotation offset per frame
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

        // Vorticity amplification — scale the curl-derived velocity
        v *= mix(0.5, 2.0, vorticity / 5.0);

        // Self-advection
        float speedScale = fluidSpeed * sqrt(Res.x / 600.0);
        vec2 advUV = fract((pos - v * vec2(-1.0, 1.0) * speedScale) / Res);
        vec4 col = texture2D(velBuf, advUV);

        // Viscosity damping — blend stored velocity toward neutral
        col.xy = mix(col.xy, vec2(0.5), viscosity * 0.02);

        // Boundary collision — reflect velocity near edges to create spirals
        float edgeDist = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y));
        float edgeForce = smoothstep(0.05, 0.0, edgeDist);
        if (edgeForce > 0.0) {
            vec2 edgeNormal = vec2(0.0);
            if (uv.x < 0.05) edgeNormal.x = 1.0;
            if (uv.x > 0.95) edgeNormal.x = -1.0;
            if (uv.y < 0.05) edgeNormal.y = 1.0;
            if (uv.y > 0.95) edgeNormal.y = -1.0;
            edgeNormal = normalize(edgeNormal + 0.001);
            // Reflect velocity off boundary
            vec2 vel = (col.xy - 0.5) * 2.0;
            float into = dot(vel, -edgeNormal);
            if (into > 0.0) {
                vel += edgeNormal * into * 2.0 * edgeForce;
                col.xy = vel * 0.5 + 0.5;
            }
        }

        // ---- Mouse interaction ----
        float interacting = max(mouseDown, pinchHold);
        if (interacting > 0.3) {
            vec2 mDiff = uv - mousePos;
            mDiff.x *= aspect;
            float dist2 = dot(mDiff, mDiff);
            float r2 = splatRadius * splatRadius;
            if (dist2 < r2 * 12.0) {
                float falloff = exp(-dist2 / r2);
                vec2 force = mouseDelta * Res * splatForce * 0.0003 * interacting;
                if (dot(force, force) < 0.000001) {
                    // No delta — push radially
                    force = normalize(mDiff + 0.001) * 0.02 * interacting * splatForce;
                }
                force = clamp(force, vec2(-0.3), vec2(0.3));
                col.xy += force * falloff;
                col.xy = clamp(col.xy, 0.0, 1.0);
            }
        }

        // ---- Movement modes ----
        float t = TIME * moveSpeed;

        if (mMode == 1) {
            // Swirl — rotating current in center
            vec2 scr = fract((pos / Res) - 0.5 + 0.5) - 0.5;
            float swirlStr = 0.003 * splatForce;
            col.xy += swirlStr * cos(t * 0.3 - vec2(0.0, 1.57)) / (dot(scr, scr) / 0.05 + 0.05);
        } else if (mMode == 2) {
            // Pulse — radial breathing from center
            vec2 scr = fract((pos / Res) - 0.5 + 0.5) - 0.5;
            float pulse = sin(t * 0.8) * 0.004 * splatForce;
            col.xy += normalize(scr + 0.001) * pulse / (dot(scr, scr) / 0.03 + 0.1);
        }

        // Audio — bass creates force splats
        if (audioBass > 0.2) {
            float at = float(FRAMEINDEX) * 0.1;
            vec2 splatPos = vec2(hash21(vec2(at, 1.0)), hash21(vec2(at, 2.0)));
            vec2 mDiff = uv - splatPos;
            mDiff.x *= aspect;
            float dist2 = dot(mDiff, mDiff);
            float splatR = splatRadius * (1.0 + audioBass * 3.0);
            if (dist2 < splatR * splatR * 12.0) {
                float falloff = exp(-dist2 / (splatR * splatR));
                float splatAngle = hash21(vec2(at, 3.0)) * PI2;
                col.xy += vec2(cos(splatAngle), sin(splatAngle)) * audioBass * 0.3 * splatForce * falloff;
                col.xy = clamp(col.xy, 0.0, 1.0);
            }
        }

        // Initialization
        if (FRAMEINDEX < 4) {
            col = vec4(0.5, 0.5, 0.0, 1.0);
            // Seed with gentle initial swirl
            vec2 d = uv - 0.5;
            col.xy = vec2(0.5 - d.y * 0.2, 0.5 + d.x * 0.2);
        }

        gl_FragColor = col;
        return;
    }

    // ===== PASS 1: UV Advection =====
    if (PASSINDEX == 1) {
        // Initialize UV buffer to identity on first frames
        if (FRAMEINDEX < 4) {
            gl_FragColor = vec4(uv, 0.0, 1.0);
            return;
        }

        // Read velocity at this pixel (stored as 0-1, decode to -1..1)
        vec2 vel = (texture2D(velBuf, uv).xy - 0.5) * 2.0;

        // Advect: sample the UV buffer from where the fluid came from
        // This is the core of the Paketa12 technique — pull UVs along velocity
        vec2 advUV = fract(uv - vel * advectSpeed * 0.003);
        vec2 storedUV = texture2D(uvBuf, advUV).rg;

        // Pull back toward identity (the "return home" force)
        // When returnRate=0, UVs never return; when high, image snaps back quickly
        storedUV = mix(storedUV, uv, returnRate);

        gl_FragColor = vec4(storedUV, 0.0, 1.0);
        return;
    }

    // ===== PASS 2: Final Render =====

    // Read warped UVs from the advection buffer
    vec2 warpedUV = texture2D(uvBuf, uv).rg;

    // UV displacement for lighting computation
    vec2 uvDisp = warpedUV - uv;

    bool hasInput = IMG_SIZE_inputTex.x > 0.0;

    // Sample the original image at warped UVs — this is why it stays crisp
    vec3 col;
    if (hasInput && !showUV) {
        col = texture2D(inputTex, warpedUV).rgb;
    } else {
        // No texture or showUV mode: visualize the UV field as colors
        // Red = X displacement, Green = Y displacement (the "UV aquarium")
        col = vec3(warpedUV.x, warpedUV.y, 0.3 + 0.3 * sin(TIME * 0.5));
    }

    // ---- Surface lighting from UV displacement gradient ----
    if (bumpHeight > 0.0) {
        float delta = max(1.0 / Res.x, 1.0 / Res.y);

        // Compute gradient of UV displacement for surface normal
        vec2 uvL = texture2D(uvBuf, uv + vec2(-delta, 0.0)).rg;
        vec2 uvR = texture2D(uvBuf, uv + vec2( delta, 0.0)).rg;
        vec2 uvU = texture2D(uvBuf, uv + vec2(0.0,  delta)).rg;
        vec2 uvD = texture2D(uvBuf, uv + vec2(0.0, -delta)).rg;

        // Use the magnitude of UV displacement as the height field
        float hL = length(uvL - (uv + vec2(-delta, 0.0)));
        float hR = length(uvR - (uv + vec2( delta, 0.0)));
        float hU = length(uvU - (uv + vec2(0.0,  delta)));
        float hD = length(uvD - (uv + vec2(0.0, -delta)));

        vec3 n = normalize(vec3(
            (hR - hL) * bumpHeight,
            (hU - hD) * bumpHeight,
            1.0
        ));

        // Diffuse lighting
        vec3 lightDir = normalize(vec3(0.5, 0.8, 1.0));
        float diff = clamp(dot(n, lightDir), 0.4, 1.0);

        // Specular highlight — Blinn-Phong
        vec2 sc = (gl_FragCoord.xy - Res * 0.5) / Res.x;
        vec3 viewDir = normalize(vec3(sc, -1.0));
        vec3 halfVec = normalize(lightDir - viewDir);
        float spec = pow(max(dot(n, halfVec), 0.0), specPow) * specAmount;

        col = col * diff + vec3(spec);
    }

    // Transparent background
    float alpha = 1.0;
    if (transparentBg) {
        float lum = dot(col, vec3(0.299, 0.587, 0.114));
        alpha = smoothstep(0.02, 0.15, lum);
    }

    gl_FragColor = vec4(col, alpha);
}
