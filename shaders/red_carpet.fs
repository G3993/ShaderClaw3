/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Red Carpet — award-show paparazzi aesthetic with depth lighting, soft shadows, and a slow parallax drift. Deep red velvet stretching to vanishing point with chrome stanchion poles, velvet pile-direction shading, occasional HDR camera-flash bursts, and a drifting warm key light that casts directional shadows across the carpet. Outputs LINEAR HDR.",
  "INPUTS": [
    { "NAME": "flashRate",        "LABEL": "Flash Rate",        "TYPE": "float", "MIN": 0.0, "MAX": 8.0,   "DEFAULT": 2.5 },
    { "NAME": "pileDirection",    "LABEL": "Pile Direction",    "TYPE": "float", "MIN": 0.0, "MAX": 6.2832,"DEFAULT": 1.5708 },
    { "NAME": "velvetSaturation", "LABEL": "Velvet Saturation", "TYPE": "float", "MIN": 0.0, "MAX": 2.0,   "DEFAULT": 1.0 },
    { "NAME": "audioReact",       "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0,   "DEFAULT": 1.0 },
    { "NAME": "driftSpeed",       "LABEL": "Drift Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.18 },
    { "NAME": "lightDepth",       "LABEL": "Light & Shadow",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0,   "DEFAULT": 1.0 },
    { "NAME": "shadowSoftness",   "LABEL": "Shadow Softness",   "TYPE": "float", "MIN": 0.1, "MAX": 4.0,   "DEFAULT": 1.4 }
  ]
}*/

// =====================================================================
// RED CARPET — paparazzi flash, deep red velvet, chrome stanchions
// + depth lighting / shadow pass, slow parallax drift
// =====================================================================

float hash11(float x) { return fract(sin(x * 91.345) * 47453.123); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise2(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

// 3-octave fbm for shadow/light variation on the carpet
float fbm2(vec2 p) {
    float s = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        s += a * vnoise2(p);
        p = p * 1.97 + vec2(3.7, 1.3);
        a *= 0.5;
    }
    return s;
}

// Rotation helper
mat2 rot2(float a) { float c=cos(a), s=sin(a); return mat2(c,-s,s,c); }

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p  = uv * 2.0 - 1.0;
    p.x    *= RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    float t     = TIME;
    float audio = audioBass * audioReact;

    // ── Slow parallax drift ──────────────────────────────────────────
    // Camera sways gently left/right and bobs slightly up/down.
    float driftAmt = driftSpeed;
    float driftX   =  sin(t * 0.31 * driftAmt + 0.7) * 0.055
                    + sin(t * 0.17 * driftAmt + 2.1) * 0.025;
    float driftY   =  sin(t * 0.23 * driftAmt + 1.3) * 0.030
                    + sin(t * 0.11 * driftAmt + 0.4) * 0.012;

    // Apply drift to view — shift p slightly (parallax on the carpet)
    vec2 pd = p + vec2(driftX, driftY);

    // ── Perspective projection ───────────────────────────────────────
    float horizonY  = 0.15 + driftY * 0.4;   // horizon shifts with drift
    float carpetMask = smoothstep(horizonY + 0.02, horizonY - 0.02, pd.y);

    float yFromHorizon = max(horizonY - pd.y, 0.001);
    float z            = 0.5 / yFromHorizon;
    float carpetU      = (pd.x - driftX * 0.6) * z * 1.2;
    float carpetV      = z;

    // ── Drifting key light direction ─────────────────────────────────
    // A warm overhead light slowly swings left/right, forward/back.
    float lightSwingX  = sin(t * 0.19 * driftAmt + 1.1) * 0.55;
    float lightSwingZ  = cos(t * 0.13 * driftAmt + 0.6) * 0.35 + 0.7;
    vec3  keyLightDir  = normalize(vec3(lightSwingX, 1.6, lightSwingZ));

    // Carpet surface normal in world space (roughly flat, tilted toward
    // camera). We approximate the pile micro-normal for shading.
    vec2  pileDir2D    = vec2(cos(pileDirection), sin(pileDirection));
    vec3  carpetNormal = normalize(vec3(-pileDir2D.x * 0.12,
                                        1.0,
                                       -pileDir2D.y * 0.12));

    // Diffuse key hit on carpet surface.
    float keyDiff = max(0.0, dot(carpetNormal, keyLightDir));

    // ── Soft shadow stripes ──────────────────────────────────────────
    // Low-frequency fbm in carpet-space, drifting with the light.
    float shadowOff  = sin(t * 0.09 * driftAmt + 2.4) * 0.7;
    float shadowSamp = fbm2(vec2(carpetU * 0.55 + shadowOff,
                                  carpetV * 0.30 + t * 0.06 * driftAmt));
    // Remap fbm into a soft dark band pattern (2–3 bands across depth).
    float shadowBand = sin(shadowSamp * 6.28 * 1.8 + shadowOff * 2.0) * 0.5 + 0.5;
    shadowBand       = mix(1.0, shadowBand, 0.35 * lightDepth);

    // Secondary fine shadow noise (stanchion/crowd silhouettes suggestion).
    float shadowDetail = fbm2(vec2(carpetU * 1.8 + shadowOff * 0.4,
                                    carpetV * 0.9 - t * 0.04 * driftAmt));
    shadowDetail = mix(0.82, 1.0, shadowDetail);
    shadowDetail = mix(1.0, shadowDetail, 0.5 * lightDepth);

    float totalShadow = shadowBand * shadowDetail;

    // ── Velvet base color ────────────────────────────────────────────
    vec3 deepRed = vec3(0.18, 0.012, 0.012);
    vec3 midRed  = vec3(0.62, 0.04,  0.035);
    vec3 hotRed  = vec3(1.10, 0.16,  0.10);

    // Warm key light color & ambient fill
    vec3 keyLightCol = vec3(1.05, 0.80, 0.60);
    vec3 ambientCol  = vec3(0.22, 0.08, 0.08);

    // ── Pile direction shading ───────────────────────────────────────
    vec2 viewDir  = normalize(vec2(carpetU * 0.05, 1.0));
    float pileDot = dot(pileDir2D, viewDir);
    float pileShade = 0.55 + 0.4 * (pileDot * 0.5 + 0.5) * 1.6;

    // ── Pile micro-texture ───────────────────────────────────────────
    float depthFade = clamp(1.0 / (1.0 + z * 0.6), 0.0, 1.0);
    float pile = vnoise2(vec2(carpetU * 80.0, carpetV * 220.0));
    pile += 0.5 * vnoise2(vec2(carpetU * 220.0, carpetV * 600.0));
    pile /= 1.5;
    float pileMod = mix(0.92, 0.65 + pile * 0.7, depthFade);

    float tufts = hash21(floor(vec2(carpetU * 140.0, carpetV * 380.0)));
    tufts = pow(tufts, 18.0) * depthFade;

    // ── Assemble velvet color ────────────────────────────────────────
    vec3 velvet = mix(deepRed, midRed, clamp(pileShade, 0.0, 1.2));
    velvet = mix(velvet, hotRed, tufts * 0.85);
    velvet *= pileMod;

    // Apply directional key light + ambient (depth-attenuated).
    float keyAtten  = mix(0.0, 1.0, depthFade * 0.9 + 0.1);
    vec3  litVelvet = velvet * (ambientCol
                              + keyLightCol * keyDiff * keyAtten * lightDepth * 0.7);
    // Blend raw velvet (original look) with fully lit version tastefully.
    velvet = mix(velvet, litVelvet, 0.55 * lightDepth);

    // Apply soft shadow.
    velvet *= totalShadow;

    // Saturation control.
    float vlum = dot(velvet, vec3(0.299, 0.587, 0.114));
    velvet = mix(vec3(vlum), velvet, velvetSaturation);

    // Distance fall-off.
    velvet *= mix(1.0, 0.55, smoothstep(1.0, 6.0, z));
    velvet *= mix(1.0, 0.18, smoothstep(2.0, 8.0, z));

    // ── Specular highlight from the drifting key light ───────────────
    // Approximate specular on the carpet surface (pile-sheen).
    vec3  halfV   = normalize(keyLightDir + vec3(0.0, 0.0, -1.0)); // toward camera approx
    float spec    = pow(max(0.0, dot(carpetNormal, halfV)), 32.0);
    spec         *= depthFade * keyAtten * lightDepth;
    velvet       += keyLightCol * spec * 0.18;

    // ── Spotlight pools ──────────────────────────────────────────────
    vec2 driftedP = vec2(pd.x, pd.y);
    float spot1 = exp(-pow(length(vec2(driftedP.x - 0.0,
                            (horizonY - driftedP.y) - 0.45)), 2.0) * 6.0);
    float spot2 = exp(-pow(length(vec2(driftedP.x * 0.9,
                            (horizonY - driftedP.y) - 0.85)), 2.0) * 2.5);
    // Drift the pools slightly with the light.
    float spot3 = exp(-pow(length(vec2(driftedP.x - lightSwingX * 0.3,
                            (horizonY - driftedP.y) - 0.62)), 2.0) * 4.0);
    float spotPool = spot1 * 0.45 + spot2 * 0.25 + spot3 * 0.30 * lightDepth;
    velvet += vec3(1.10, 0.45, 0.32) * spotPool * carpetMask;

    // ── Stanchion poles ──────────────────────────────────────────────
    float stanchion = 0.0;
    vec3  chromeCol = vec3(0.0);

    // Chrome key light color drifts with the key.
    vec3 chromeKey = vec3(1.0, 0.90, 0.78) * (0.5 + 0.5 * lightDepth);

    for (int i = 0; i < 6; i++) {
        float fi    = float(i) + 1.0;
        float poleZ = fi * 1.1;
        float poleY = horizonY - 0.5 / poleZ;

        // Poles drift laterally with camera parallax (closer poles move more).
        float poleDrift = driftX * (1.0 / poleZ) * 0.7;

        for (int s = 0; s < 2; s++) {
            float sgn   = s == 0 ? -1.0 : 1.0;
            float poleU = sgn * 1.6;
            float poleX = poleU / (poleZ * 1.2) + poleDrift;

            float poleHeight = 0.55 / poleZ;
            float poleHalfW  = 0.012 / poleZ;

            float dx    = abs(pd.x - poleX);
            float dyTop = (poleY + poleHeight) - pd.y;
            float dyBot = pd.y - poleY;
            float inX   = smoothstep(poleHalfW * 1.4, poleHalfW * 0.6, dx);
            float inY   = step(0.0, dyTop) * step(0.0, dyBot);
            float pmask = inX * inY;

            float across     = (pd.x - poleX) / max(poleHalfW, 1e-4);
            float chromeShade = 1.0 - 0.55 * abs(across);

            // Glint shifts with the drifting key light.
            float glintOffset = across - (0.3 + lightSwingX * 0.4);
            float glint        = exp(-pow(glintOffset, 2.0) * 18.0);

            // Soft shadow on the chrome from depth lighting.
            float chromeShadow = mix(1.0, totalShadow,
                                     0.4 * lightDepth / (fi * 0.5 + 0.5));

            // Finial cap.
            float capR  = poleHalfW * 2.4;
            vec2  capC  = vec2(poleX, poleY + poleHeight);
            float capD  = length(vec2(pd.x - capC.x, pd.y - capC.y));
            float cap   = smoothstep(capR, capR * 0.6, capD);
            float capGlint = smoothstep(capR * 0.5, 0.0,
                             length(vec2(pd.x - capC.x - capR * 0.25,
                                         pd.y - capC.y - capR * 0.2)));

            stanchion = max(stanchion, max(pmask, cap));
            vec3 chrome = (vec3(0.55, 0.58, 0.62) * chromeShade
                         + chromeKey * glint * 0.35
                         + vec3(2.4, 2.2, 1.9) * capGlint * cap)
                         * chromeShadow;
            chromeCol = mix(chromeCol, chrome, max(pmask, cap));
        }
    }

    // ── Background above horizon ─────────────────────────────────────
    vec3  venueBg  = vec3(0.012, 0.008, 0.012);
    float backGlow = exp(-pow((pd.y - horizonY) * 4.0, 2.0))
                   * exp(-pow(pd.x * 0.6, 2.0));
    venueBg += vec3(0.45, 0.06, 0.05) * backGlow * 0.6;

    // Subtle light shaft from drifting key (above horizon).
    float shaftX  = pd.x - lightSwingX * 0.5;
    float shaft   = exp(-shaftX * shaftX * 3.0)
                  * smoothstep(horizonY + 0.55, horizonY + 0.02, pd.y)
                  * smoothstep(horizonY - 0.02, horizonY + 0.1,  pd.y);
    venueBg += vec3(0.35, 0.12, 0.06) * shaft * 0.18 * lightDepth;

    vec3 col = mix(venueBg, velvet, carpetMask);
    col = mix(col, chromeCol, stanchion);

    // ── Paparazzi flash bursts ───────────────────────────────────────
    if (flashRate > 0.001) {
        float slotLen = 1.0 / max(flashRate, 0.001);
        float slot    = floor(t / slotLen);
        for (int k = 0; k < 3; k++) {
            float s     = slot - float(k);
            float seed  = s * 17.13;
            float startT = s * slotLen + hash11(seed) * slotLen * 0.7;
            float age    = t - startT;
            if (age > 0.0 && age < 0.18) {
                vec2 fp = vec2(
                    (hash11(seed + 1.7) - 0.5) * 2.0
                        * (RENDERSIZE.x / max(RENDERSIZE.y, 1.0)) * 0.95,
                    mix(horizonY + 0.02, horizonY + 0.45, hash11(seed + 3.1))
                );
                float rise   = smoothstep(0.0, 0.012, age);
                float decay  = 1.0 - smoothstep(0.012, 0.10, age);
                float flashI = rise * decay;

                float r    = length(pd - fp);
                float core = exp(-r * r * 800.0);
                float halo = exp(-r * r * 18.0) * 0.35;
                float glow = exp(-r * r * 2.0)  * 0.10;
                float flashAmt = (core * 3.0 + halo + glow) * flashI;
                col += vec3(2.6, 2.7, 3.0) * flashAmt;
                col += velvet * flashI * exp(-r * r * 4.0) * 1.2 * carpetMask;
            }
        }
    }

    // ── Depth-based edge vignette (tasteful darkening at carpet sides) ──
    float edgeDist  = 1.0 - abs(pd.x) / (RENDERSIZE.x / max(RENDERSIZE.y, 1.0));
    float sideVig   = smoothstep(0.0, 0.35, edgeDist);
    col *= mix(1.0, sideVig, 0.38 * lightDepth);

    // ── Audio react ──────────────────────────────────────────────────
    col *= 1.0 + audio * 0.08;
    col += vec3(2.0, 2.1, 2.4) * audioHigh * audioReact * 0.05;

    // Ambient floor.
    col += vec3(0.01, 0.003, 0.003);

    gl_FragColor = vec4(col, 1.0);
}