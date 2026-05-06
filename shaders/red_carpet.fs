/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Red Carpet — award-show paparazzi aesthetic. Deep red velvet stretching to vanishing point with chrome stanchion poles silhouetted along the sides and occasional bright HDR camera-flash bursts. Velvet has visible pile-direction shading. Outputs LINEAR HDR.",
  "INPUTS": [
    { "NAME": "flashRate",        "LABEL": "Flash Rate",        "TYPE": "float", "MIN": 0.0, "MAX": 8.0, "DEFAULT": 2.5 },
    { "NAME": "pileDirection",    "LABEL": "Pile Direction",    "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 1.5708 },
    { "NAME": "velvetSaturation", "LABEL": "Velvet Saturation", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "audioReact",       "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
  ]
}*/

// =====================================================================
// RED CARPET — paparazzi flash, deep red velvet, chrome stanchions
// =====================================================================

float hash11(float x) { return fract(sin(x * 91.345) * 47453.123); }
float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash22(vec2 p) {
    return fract(sin(dot(p, vec2(269.5, 183.3))) * 39172.731);
}

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = uv * 2.0 - 1.0;
    p.x *= RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    float t = TIME;
    float audio = audioBass * audioReact;

    // === Perspective: carpet stretches into a vanishing point ===
    // Horizon at y = horizonY (upper portion), carpet below.
    float horizonY = 0.15;
    float carpetMask = smoothstep(horizonY + 0.02, horizonY - 0.02, p.y);

    // Project floor: depth z increases as we approach horizon.
    // y' = horizonY - depth_factor / z  ->  z = depth_factor / (horizonY - y)
    float yFromHorizon = max(horizonY - p.y, 0.001);
    float z = 0.5 / yFromHorizon;          // depth (1 near, big far)
    // Carpet-space U coordinate: x widens with distance to camera (perspective).
    float carpetU = p.x * z * 1.2;
    float carpetV = z;

    // === Velvet base color ===
    vec3 deepRed   = vec3(0.18, 0.012, 0.012);   // shadow oxblood
    vec3 midRed    = vec3(0.62, 0.04, 0.035);    // body crimson
    vec3 hotRed    = vec3(1.10, 0.16, 0.10);     // lit pile

    // === Pile direction shading ===
    // pileDirection is an angle; pile fibers point that way in carpet-space.
    // Light comes from above-camera; brighter when looking *with* the pile,
    // darker when looking *against* it.
    vec2 pileDir = vec2(cos(pileDirection), sin(pileDirection));
    // Camera view direction, projected onto carpet plane, points from
    // viewer into scene (toward +V in carpet-space).
    vec2 viewDir = normalize(vec2(carpetU * 0.05, 1.0));
    float pileDot = dot(pileDir, viewDir);
    // Map [-1,1] -> [0.55, 1.35]: with-pile brighter, against-pile darker.
    float pileShade = 0.55 + 0.4 * (pileDot * 0.5 + 0.5) * 1.6;

    // === Velvet pile micro-texture ===
    // High-frequency tufts with depth attenuation (far away the tufts
    // smear into a uniform red — important for perspective).
    float depthFade = clamp(1.0 / (1.0 + z * 0.6), 0.0, 1.0);
    float pile = vnoise(vec2(carpetU * 80.0, carpetV * 220.0));
    pile += 0.5 * vnoise(vec2(carpetU * 220.0, carpetV * 600.0));
    pile = pile / 1.5;
    float pileMod = mix(0.92, 0.65 + pile * 0.7, depthFade);

    // Sparse bright tufts catching the light (HDR-ish micro-highlights).
    float tufts = hash21(floor(vec2(carpetU * 140.0, carpetV * 380.0)));
    tufts = pow(tufts, 18.0) * depthFade;

    // Color: lerp deep -> mid by pile shade, lerp toward hot for tufts.
    vec3 velvet = mix(deepRed, midRed, clamp(pileShade, 0.0, 1.2));
    velvet = mix(velvet, hotRed, tufts * 0.85);
    velvet *= pileMod;

    // Saturation control around luminance.
    float vlum = dot(velvet, vec3(0.299, 0.587, 0.114));
    velvet = mix(vec3(vlum), velvet, velvetSaturation);

    // Slight red-channel boost as carpet recedes (atmospheric warmth).
    velvet *= mix(1.0, 0.55, smoothstep(1.0, 6.0, z));
    // Add deep-shadow vignette at the very far end.
    velvet *= mix(1.0, 0.18, smoothstep(2.0, 8.0, z));

    // === Spotlight pools on the carpet ===
    // Two soft warm pools where photographers would aim their lights.
    float spot1 = exp(-pow(length(vec2(p.x - 0.0, (horizonY - p.y) - 0.45)), 2.0) * 6.0);
    float spot2 = exp(-pow(length(vec2(p.x * 0.9, (horizonY - p.y) - 0.85)), 2.0) * 2.5);
    float spotPool = spot1 * 0.55 + spot2 * 0.30;
    velvet += vec3(1.10, 0.45, 0.32) * spotPool * carpetMask;

    // === Stanchion poles (chrome silhouettes along carpet edges) ===
    // Two rows of poles receding into the distance. In carpet-space they
    // sit at fixed |U| values and at integer V intervals.
    float stanchion = 0.0;
    vec3 chromeCol = vec3(0.0);
    for (int i = 0; i < 6; i++) {
        float fi = float(i) + 1.0;
        float poleZ = fi * 1.1;                      // depth in carpet-space
        // Convert pole world position back into screen p.
        float poleY = horizonY - 0.5 / poleZ;
        // Two sides
        for (int s = 0; s < 2; s++) {
            float sgn = s == 0 ? -1.0 : 1.0;
            float poleU = sgn * 1.6;                 // fixed offset in carpet-U
            // Convert (U, z) -> screen x:  x = U / (z * 1.2)
            float poleX = poleU / (poleZ * 1.2);
            // Pole height shrinks with distance.
            float poleHeight = 0.55 / poleZ;
            float poleHalfW = 0.012 / poleZ;
            // Vertical pole rectangle from poleY upward.
            float dx = abs(p.x - poleX);
            float dyTop = (poleY + poleHeight) - p.y;     // above pole top
            float dyBot = p.y - poleY;                     // below pole base
            float inX = smoothstep(poleHalfW * 1.4, poleHalfW * 0.6, dx);
            float inY = step(0.0, dyTop) * step(0.0, dyBot);
            float pmask = inX * inY;
            // Chrome shading: vertical streak — bright center, darker edges,
            // with a hot specular glint slightly off-axis.
            float across = (p.x - poleX) / max(poleHalfW, 1e-4);
            float chromeShade = 1.0 - 0.55 * abs(across);
            float glint = exp(-pow(across - 0.3, 2.0) * 18.0);
            // Hot ball cap on top of pole (the classic stanchion finial).
            float capR = poleHalfW * 2.4;
            vec2 capC = vec2(poleX, poleY + poleHeight);
            float capD = length(vec2((p.x - capC.x), (p.y - capC.y)));
            float cap = smoothstep(capR, capR * 0.6, capD);
            float capGlint = smoothstep(capR * 0.5, 0.0, length(vec2(p.x - capC.x - capR * 0.25, p.y - capC.y - capR * 0.2)));
            // Velvet-rope swag between adjacent poles (just a soft red arc).
            // skipped for clarity — poles + caps read as stanchions already.

            stanchion = max(stanchion, max(pmask, cap));
            vec3 chrome = vec3(0.55, 0.58, 0.62) * chromeShade
                        + vec3(2.2, 2.0, 1.7) * glint * 0.35
                        + vec3(2.4, 2.2, 1.9) * capGlint * cap;
            chromeCol = mix(chromeCol, chrome, max(pmask, cap));
        }
    }

    // === Background above horizon: dark venue + soft red glow ===
    vec3 venueBg = vec3(0.012, 0.008, 0.012);
    // Faint red back-glow over the carpet area.
    float backGlow = exp(-pow((p.y - horizonY) * 4.0, 2.0)) * exp(-pow(p.x * 0.6, 2.0));
    venueBg += vec3(0.45, 0.06, 0.05) * backGlow * 0.6;

    vec3 col = mix(venueBg, velvet, carpetMask);
    col = mix(col, chromeCol, stanchion);

    // === Paparazzi flash bursts ===
    // Each "slot" of duration (1/flashRate) seconds spawns one flash at a
    // random screen position. Flash duration ~0.1s, peak ~3.0 linear.
    if (flashRate > 0.001) {
        float slotLen = 1.0 / max(flashRate, 0.001);
        float slot = floor(t / slotLen);
        // Up to 3 overlapping recent slots so flashes can stack briefly.
        for (int k = 0; k < 3; k++) {
            float s = slot - float(k);
            float seed = s * 17.13;
            float startT = s * slotLen + hash11(seed) * slotLen * 0.7;
            float age = t - startT;
            if (age > 0.0 && age < 0.18) {
                vec2 fp = vec2(
                    (hash11(seed + 1.7) - 0.5) * 2.0 * (RENDERSIZE.x / max(RENDERSIZE.y, 1.0)) * 0.95,
                    mix(horizonY + 0.02, horizonY + 0.45, hash11(seed + 3.1))
                );
                // Flash temporal: sharp rise (~10ms), short hold, decay to 0 by 100ms.
                float rise = smoothstep(0.0, 0.012, age);
                float decay = 1.0 - smoothstep(0.012, 0.10, age);
                float flashI = rise * decay;
                // Spatial: tight hot core + wide soft halo.
                float r = length(p - fp);
                float core = exp(-r * r * 800.0);
                float halo = exp(-r * r * 18.0) * 0.35;
                float glow = exp(-r * r * 2.0) * 0.10;
                float flashAmt = (core * 3.0 + halo + glow) * flashI;
                // Slight bluish-white HDR pop.
                col += vec3(2.6, 2.7, 3.0) * flashAmt;
                // Bonus: flash also briefly lights the carpet near it.
                col += velvet * flashI * exp(-r * r * 4.0) * 1.2 * carpetMask;
            }
        }
    }

    // Audio: bass pulses overall luminance; treble adds a small flash kick.
    col *= 1.0 + audio * 0.08;
    col += vec3(2.0, 2.1, 2.4) * audioHigh * audioReact * 0.05;

    // Ambient floor — keep deep shadows from clipping.
    col += vec3(0.01, 0.003, 0.003);

    gl_FragColor = vec4(col, 1.0);
}
