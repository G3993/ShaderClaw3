/*{
  "DESCRIPTION": "Brushstroke Canyon — 3D raymarched height-field of massive frozen paint brushstrokes. de Kooning palette: cadmium red, ochre, cerulean blue, chalk white.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "camHeight",   "LABEL": "Camera Height","TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.3, "MAX": 3.0 },
    { "NAME": "flySpeed",    "LABEL": "Fly Speed",    "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "reliefAmt",   "LABEL": "Relief",       "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.1, "MAX": 1.2 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioReact",  "LABEL": "Audio",        "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// ── Value noise & FBM ──────────────────────────────────────────────────────

float hash2(vec2 p) {
    p = fract(p * vec2(127.1, 311.7));
    p += dot(p, p + 19.17);
    return fract(p.x * p.y);
}

float valueNoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    float a = hash2(i + vec2(0.0, 0.0));
    float b = hash2(i + vec2(1.0, 0.0));
    float c = hash2(i + vec2(0.0, 1.0));
    float d = hash2(i + vec2(1.0, 1.0));
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm(vec2 p) {
    float v = 0.0;
    float amp = 0.5;
    float freq = 1.0;
    for (int k = 0; k < 5; k++) {
        v    += valueNoise(p * freq) * amp;
        freq *= 2.1;
        amp  *= 0.48;
    }
    return v;
}

// Domain-warped terrain height
float terrainH(vec2 xz, float t, float relief) {
    vec2 q = vec2(fbm(xz + t * 0.1), fbm(xz + vec2(5.2, 1.3) + t * 0.07));
    return fbm(xz + 1.5 * q) * relief;
}

// Finite-difference surface normal
vec3 terrainNormal(vec2 xz, float t, float relief) {
    float eps = 0.003;
    float hC = terrainH(xz, t, relief);
    float hR = terrainH(xz + vec2(eps, 0.0), t, relief);
    float hU = terrainH(xz + vec2(0.0, eps), t, relief);
    return normalize(vec3(hC - hR, eps, hC - hU));
}

// de Kooning paint color from FBM layer value
vec3 paintColor(vec2 xz, float t) {
    float layer = fbm(xz * 1.7 + vec2(3.3, 7.1) + t * 0.05);
    vec3 cadRed     = vec3(1.0,  0.04, 0.08);
    vec3 ochre      = vec3(0.95, 0.60, 0.0);
    vec3 cerulean   = vec3(0.0,  0.50, 1.0);
    vec3 chalkWhite = vec3(0.92, 0.92, 0.88);
    vec3 c;
    if (layer < 0.25) {
        c = cadRed;
    } else if (layer < 0.50) {
        c = mix(cadRed, ochre, (layer - 0.25) / 0.25);
    } else if (layer < 0.75) {
        c = mix(ochre, cerulean, (layer - 0.50) / 0.25);
    } else {
        c = mix(cerulean, chalkWhite, (layer - 0.75) / 0.25);
    }
    return c;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    float t = TIME;
    float relief = reliefAmt * (1.0 + audioLevel * audioReact * 0.4);

    // ── Camera ───────────────────────────────────────────────────────────
    vec3 ro = vec3(t * flySpeed, camHeight, t * flySpeed * 0.3);
    vec3 target = ro + vec3(0.3, -0.6, 1.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up2   = cross(right, fwd);
    vec3 rd    = normalize(fwd + uv.x * right * 0.6 + uv.y * up2 * 0.6);

    // ── Sky ──────────────────────────────────────────────────────────────
    vec3 skyCol = vec3(0.1, 0.07, 0.02);
    vec3 col    = skyCol;

    // ── Ray march (64 steps, adaptive step size) ─────────────────────────
    float dist  = 0.0;
    bool  hit   = false;
    float tStep = 0.02;

    for (int step = 0; step < 64; step++) {
        dist += tStep;
        vec3 pos = ro + rd * dist;
        float h  = terrainH(pos.xz, t, relief);
        if (pos.y < h) {
            // Binary search refinement (6 halvings)
            float stepBack = tStep * 0.5;
            dist -= stepBack;
            for (int b = 0; b < 6; b++) {
                stepBack *= 0.5;
                vec3 bp = ro + rd * dist;
                float bh = terrainH(bp.xz, t, relief);
                if (bp.y < bh) {
                    dist -= stepBack;
                } else {
                    dist += stepBack;
                }
            }
            hit = true;
            break;
        }
        tStep = 0.015 + dist * 0.04;
    }

    if (hit) {
        vec3 pos = ro + rd * dist;
        vec3 N   = terrainNormal(pos.xz, t, relief);

        // Base paint color
        vec3 baseCol = paintColor(pos.xz, t);

        // ── Lighting ─────────────────────────────────────────────────────
        vec3 sunDir   = normalize(vec3(-0.4, 1.0, 0.6));
        vec3 sunColor = vec3(1.0, 0.95, 0.85);
        vec3 ambColor = vec3(0.6, 0.7, 1.0);

        float diff = max(dot(N, sunDir), 0.0);

        // Specular — HDR peak on chalk white
        vec3 H     = normalize(sunDir - rd);
        float spec = pow(max(dot(N, H), 0.0), 32.0);

        // Whiteness drives specular HDR multiplier
        float whiteness = dot(baseCol, vec3(0.333));
        float specHDR   = spec * 3.0 * whiteness;

        // Slope-based black ink (stroke boundaries)
        float slopeX = abs(terrainH(pos.xz + vec2(0.01, 0.0), t, relief) -
                           terrainH(pos.xz - vec2(0.01, 0.0), t, relief)) / 0.02;
        float slopeZ = abs(terrainH(pos.xz + vec2(0.0, 0.01), t, relief) -
                           terrainH(pos.xz - vec2(0.0, 0.01), t, relief)) / 0.02;
        float slope   = clamp((slopeX + slopeZ) * 1.5, 0.0, 1.0);
        float inkMask = smoothstep(0.5, 1.0, slope);

        col  = baseCol * (diff * sunColor + 0.1 * ambColor);
        col += vec3(specHDR);
        col  = mix(col, vec3(0.0), inkMask);
        col *= hdrPeak / 2.5;

        // ── Fog / mist ────────────────────────────────────────────────────
        float fog = exp(-dist * 0.05);
        col = mix(skyCol, col, fog);
    }

    gl_FragColor = vec4(col, 1.0);
}
