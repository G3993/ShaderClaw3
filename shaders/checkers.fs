/*{
  "DESCRIPTION": "Checkers — audio-reactive 3D checkerboard with metallic or gradient palettes, bevel lighting, brushed noise, and optional texture on tiles",
  "CREDIT": "ShaderClaw — fusion of Bars time-sweeps + Squares cellular structure",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "palette", "LABEL": "Palette", "TYPE": "long", "DEFAULT": 0,
      "VALUES": [0, 1, 2],
      "LABELS": ["B&W Metallic", "Fantasy Gradient", "Both (Split)"] },
    { "NAME": "tileCount", "LABEL": "Tiles", "TYPE": "float", "DEFAULT": 8.0, "MIN": 2.0, "MAX": 40.0 },
    { "NAME": "scrollSpeed", "LABEL": "Scroll Speed", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "scrollAngle", "LABEL": "Scroll Angle", "TYPE": "float", "DEFAULT": 0.78, "MIN": 0.0, "MAX": 6.28 },
    { "NAME": "perspective", "LABEL": "Perspective", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 0.9 },
    { "NAME": "bevel", "LABEL": "Bevel", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "reliefHeight", "LABEL": "Relief Height", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "glossiness", "LABEL": "Glossiness", "TYPE": "float", "DEFAULT": 32.0, "MIN": 2.0, "MAX": 128.0 },
    { "NAME": "specAmount", "LABEL": "Specular", "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "metalness", "LABEL": "Metalness", "TYPE": "float", "DEFAULT": 0.75, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "brushAmt", "LABEL": "Brushed Noise", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "hueShift", "LABEL": "Hue Shift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "hueSpread", "LABEL": "Hue Spread", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "gradientFlow", "LABEL": "Gradient Flow", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "rimGlow", "LABEL": "Rim Glow", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "lightAngle", "LABEL": "Light Angle", "TYPE": "float", "DEFAULT": 2.3, "MIN": 0.0, "MAX": 6.28 },
    { "NAME": "lightPitch", "LABEL": "Light Pitch", "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.1, "MAX": 1.0 },
    { "NAME": "audioBassLift", "LABEL": "Bass Lift", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "audioMidWarp", "LABEL": "Mid Warp", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.5 },
    { "NAME": "audioHighShimmer", "LABEL": "High Shimmer", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "audioTileFlip", "LABEL": "Beat Flip", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "texBlend", "LABEL": "Texture Blend", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "texTileMode", "LABEL": "Tex on Tiles", "TYPE": "long", "DEFAULT": 0,
      "VALUES": [0, 1, 2],
      "LABELS": ["Light tiles", "Dark tiles", "All tiles"] },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "backgroundColor", "LABEL": "BG Color", "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.04, 1.0] }
  ]
}*/

#ifdef GL_ES
precision highp float;
#endif

#define PI 3.14159265358979
#define TAU 6.28318530718

// ---------- hashing / noise ----------
float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float valueNoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash12(i);
    float b = hash12(i + vec2(1.0, 0.0));
    float c = hash12(i + vec2(0.0, 1.0));
    float d = hash12(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// ---------- cosine palette ----------
vec3 cosPalette(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(TAU * (c * t + d));
}

// ---------- tile height field ----------
// Returns height (0..1) for a given "board space" coord.
// Checker parity raises alternate tiles. Bass pumps height.
float tileHeight(vec2 bp, float bassPump, float beatFlip) {
    vec2 cellIdx = floor(bp);
    vec2 f = fract(bp);

    float parity = mod(cellIdx.x + cellIdx.y, 2.0);
    // Beat-flip: periodically swap parity with smooth transition
    float flipPhase = fract(TIME * 0.5 + hash12(cellIdx) * 0.3);
    float flipAmt = beatFlip * smoothstep(0.45, 0.55, flipPhase) * (1.0 - smoothstep(0.9, 1.0, flipPhase));
    parity = mix(parity, 1.0 - parity, flipAmt);

    // Distance from tile edge (0 at edge, 0.5 at center)
    vec2 d2 = min(f, 1.0 - f);
    float edge = min(d2.x, d2.y); // 0..0.5
    // Smooth top: plateau with rounded edges
    float topMask = smoothstep(0.0, max(bevel, 0.001), edge);

    // Height: raised tiles = 1 * topMask, sunken tiles = 0
    float h = mix(0.0, topMask, parity);
    // Bass adds extra bounce, alternates sign so it pumps the board
    h += (parity * 2.0 - 1.0) * bassPump * 0.25 * topMask;
    return h;
}

// Normal from finite differences on the height field
vec3 tileNormal(vec2 bp, float eps, float bassPump, float beatFlip) {
    float hL = tileHeight(bp - vec2(eps, 0.0), bassPump, beatFlip);
    float hR = tileHeight(bp + vec2(eps, 0.0), bassPump, beatFlip);
    float hD = tileHeight(bp - vec2(0.0, eps), bassPump, beatFlip);
    float hU = tileHeight(bp + vec2(0.0, eps), bassPump, beatFlip);
    vec3 n = normalize(vec3(hL - hR, hD - hU, 2.0 * eps / max(reliefHeight, 0.001)));
    return n;
}

// ---------- parity & cell id (non-displaced, for coloring) ----------
void sampleTile(vec2 bp, out float parity, out vec2 cellIdx, out vec2 localF) {
    cellIdx = floor(bp);
    localF = fract(bp);
    parity = mod(cellIdx.x + cellIdx.y, 2.0);
}

void main() {
    vec2 Res = RENDERSIZE;
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = Res.x / Res.y;

    // ===== Perspective warp (pseudo-3D tilt) =====
    vec2 p = uv - 0.5;
    // Non-linear vertical squeeze so the top recedes
    float tilt = perspective;
    float persp = 1.0 + tilt * (0.5 - uv.y) * 2.0;
    p.x *= persp;
    p.y = (uv.y - 0.5) * (1.0 - tilt * 0.4);
    p += 0.5;

    // ===== Audio signals =====
    float bass = audioBass;
    float mid = audioMid;
    float high = audioHigh;
    float level = audioLevel;

    // ===== Mid-frequency warp on the board UV =====
    float warpT = TIME * 0.7;
    vec2 warp = vec2(
        sin(p.y * 6.0 + warpT) * 0.02,
        cos(p.x * 6.0 + warpT * 1.3) * 0.02
    ) * audioMidWarp * (0.4 + mid * 2.0);
    p += warp;

    // ===== Scroll direction =====
    vec2 scrollDir = vec2(cos(scrollAngle), sin(scrollAngle));
    vec2 scroll = scrollDir * TIME * scrollSpeed * (1.0 + bass * 0.8);

    // Board-space coordinate: aspect-corrected tile grid, with scroll applied
    vec2 boardP = (p - 0.5);
    boardP.x *= aspect;
    boardP *= tileCount;
    boardP += scroll * tileCount;

    // ===== Height + normal =====
    float bassPump = bass * audioBassLift;
    float h = tileHeight(boardP, bassPump, audioTileFlip);

    float eps = 0.01;
    vec3 N = tileNormal(boardP, eps, bassPump, audioTileFlip);

    // Parity / cell for coloring
    float parity; vec2 cellIdx; vec2 localF;
    sampleTile(boardP, parity, cellIdx, localF);

    // ===== Lighting =====
    float ca = cos(lightAngle), sa = sin(lightAngle);
    vec3 L = normalize(vec3(ca * cos(lightPitch), sa * cos(lightPitch), sin(lightPitch)));
    vec3 V = vec3(0.0, 0.0, 1.0);
    vec3 H = normalize(L + V);

    float ndl = clamp(dot(N, L), 0.0, 1.0);
    float ndh = clamp(dot(N, H), 0.0, 1.0);
    float spec = pow(ndh, glossiness) * specAmount;

    // Rim / fresnel for glow on edges of raised tiles
    float ndv = clamp(dot(N, V), 0.0, 1.0);
    float fresnel = pow(1.0 - ndv, 3.0);

    // ===== Base color per palette mode =====
    vec3 lightTone = vec3(0.92);
    vec3 darkTone = vec3(0.08);

    // Brushed-metal noise aligned with light direction
    vec2 brushCoord = vec2(
        dot(localF - 0.5, vec2(ca, sa)) * 20.0,
        dot(localF - 0.5, vec2(-sa, ca)) * 3.0
    );
    float brush = valueNoise(brushCoord + cellIdx * 13.7);
    brush = (brush - 0.5) * brushAmt;

    // Fantasy gradient: cosine palette driven by cell + time + distance-from-center
    float cellHash = hash12(cellIdx);
    float gradT = hueShift
        + cellHash * hueSpread
        + TIME * gradientFlow * 0.1
        + length(localF - 0.5) * 0.5;
    vec3 palA = vec3(0.5);
    vec3 palB = vec3(0.5);
    vec3 palC = vec3(1.0, 1.0, 1.0);
    vec3 palD = vec3(0.00, 0.33, 0.67);
    vec3 gradColor = cosPalette(gradT, palA, palB, palC, palD);

    // Dark tiles in gradient mode get a deeper, desaturated variant
    vec3 gradColorDark = gradColor * 0.25 + vec3(0.01, 0.005, 0.03);

    vec3 lightTile, darkTile;
    if (palette < 0.5) {
        // B&W metallic
        lightTile = lightTone + brush;
        darkTile = darkTone - brush * 0.4;
    } else if (palette < 1.5) {
        // Fantasy gradient
        lightTile = gradColor;
        darkTile = gradColorDark;
    } else {
        // Split: light tiles metallic, dark tiles gradient (or vice versa based on cell y)
        float side = step(0.0, cellIdx.y);
        lightTile = mix(gradColor, lightTone + brush, side);
        darkTile = mix(gradColorDark, darkTone - brush * 0.4, side);
    }

    vec3 tileColor = mix(darkTile, lightTile, parity);

    // ===== Texture overlay =====
    bool hasTex = IMG_SIZE_inputTex.x > 0.0;
    if (hasTex && texBlend > 0.001) {
        vec2 tuv = isf_FragNormCoord;
        // Tile the texture across the board, slight movement for life
        vec2 texCoord = localF + cellIdx * 0.13 + vec2(TIME * 0.02, 0.0);
        vec4 tx = IMG_NORM_PIXEL(inputTex, fract(texCoord));
        bool applyLight = (texTileMode > 1.5) || (texTileMode < 0.5 && parity > 0.5) || (texTileMode >= 0.5 && texTileMode < 1.5 && parity < 0.5);
        if (applyLight) {
            tileColor = mix(tileColor, tx.rgb, texBlend);
        }
    }

    // ===== Shading: diffuse + specular + fresnel rim =====
    // Metallic: tint specular by base color; Non-metallic: white specular
    float metalMix = (palette > 0.5 && palette < 1.5) ? metalness * 0.3 : metalness;
    vec3 specTint = mix(vec3(1.0), tileColor, metalMix);

    // Sunken tiles are in shadow — darker ambient
    float ambient = mix(0.18, 0.45, parity);
    vec3 shaded = tileColor * (ambient + ndl * 0.85);
    shaded += specTint * spec;

    // Rim glow on raised tiles
    vec3 rimColor = (palette > 0.5 && palette < 1.5) ? gradColor : vec3(0.7, 0.85, 1.0);
    shaded += rimColor * fresnel * rimGlow * parity;

    // ===== High-frequency shimmer on metal tiles =====
    if (audioHighShimmer > 0.001) {
        float sp = hash12(localF * 40.0 + cellIdx * 7.0 + vec2(TIME * 23.0, TIME * 17.0));
        float sparkle = smoothstep(0.985 - high * 0.15, 1.0, sp);
        shaded += specTint * sparkle * audioHighShimmer * (1.0 + high * 3.0);
    }

    // ===== Gutters between tiles (slight dark gap) =====
    vec2 edgeDist = min(localF, 1.0 - localF);
    float gutter = smoothstep(0.0, 0.015, min(edgeDist.x, edgeDist.y));
    shaded *= mix(0.25, 1.0, gutter);

    // ===== Audio level overall gain =====
    shaded *= (1.0 + level * 0.3);

    // ===== Sunken tile floor: blend toward background =====
    float sunken = 1.0 - parity;
    shaded = mix(shaded, shaded * 0.4 + backgroundColor.rgb * 0.6, sunken * (1.0 - bassPump * 0.3));

    // Vignette-style edge darkening for depth
    float vig = smoothstep(1.2, 0.3, length(uv - 0.5) * 1.8);
    shaded *= mix(0.55, 1.0, vig);

    // Surprise: every ~24s the entire board briefly tilts forward as if
    // a knight just leapt — a fleeting parallax shear plus a single
    // checker square going incandescent gold.
    {
        float _ph = fract(TIME / 24.0);
        float _f  = smoothstep(0.0, 0.05, _ph) * smoothstep(0.22, 0.12, _ph);
        // Tint the brightest checker
        float _b = dot(shaded, vec3(0.299, 0.587, 0.114));
        shaded = mix(shaded, vec3(1.0, 0.78, 0.20), _f * smoothstep(0.65, 0.85, _b) * 0.7);
    }

    gl_FragColor = vec4(shaded, 1.0);
}
