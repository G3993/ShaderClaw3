/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Carbon Fiber — woven fabric texture with silk sheen, parallax background plane, soft bloom glow, smoother motion, mouse deformation, and audio reactivity",
  "CREDIT": "Based on Giorgi Azmaipharashvili's silk shader (MIT License)",
  "INPUTS": [
    { "NAME": "fabricColor",   "LABEL": "Color",           "TYPE": "color", "DEFAULT": [0.08, 0.08, 0.12, 1.0] },
    { "NAME": "sheenColor",    "LABEL": "Sheen",           "TYPE": "color", "DEFAULT": [1.0, 0.83, 0.6, 1.0] },
    { "NAME": "bgPlaneColor",  "LABEL": "BG Plane Color",  "TYPE": "color", "DEFAULT": [0.04, 0.05, 0.09, 1.0] },
    { "NAME": "weaveScale",    "LABEL": "Weave Scale",     "TYPE": "float", "MIN": 0.3,  "MAX": 3.0,  "DEFAULT": 1.0 },
    { "NAME": "sheenStrength", "LABEL": "Sheen Strength",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7 },
    { "NAME": "waveAmount",    "LABEL": "Wave",            "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.3 },
    { "NAME": "speed",         "LABEL": "Speed",           "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 0.6 },
    { "NAME": "depthBump",     "LABEL": "Weave Depth",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "highlight",     "LABEL": "Highlights",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.85 },
    { "NAME": "scrollSpeed",   "LABEL": "Scroll Speed",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25 },
    { "NAME": "mouseDeform",   "LABEL": "Mouse Deform",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5 },
    { "NAME": "parallaxAmt",   "LABEL": "Parallax Depth",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "bgWeaveScale",  "LABEL": "BG Weave Scale",  "TYPE": "float", "MIN": 0.1,  "MAX": 2.0,  "DEFAULT": 0.45 },
    { "NAME": "bloomStrength", "LABEL": "Bloom Strength",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.55 },
    { "NAME": "bloomRadius",   "LABEL": "Bloom Radius",    "TYPE": "float", "MIN": 0.001,"MAX": 0.12, "DEFAULT": 0.032 },
    { "NAME": "invert",        "LABEL": "Invert",          "TYPE": "bool",  "DEFAULT": true },
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.3 },
    { "NAME": "transparentBg", "LABEL": "Transparent",     "TYPE": "bool",  "DEFAULT": true }
  ]
}*/

// ═══════════════════════════════════════════════════════════════════════
// CARBON FIBER — parallax BG plane + soft bloom + smoother motion
// ═══════════════════════════════════════════════════════════════════════

// ── Smooth hash helpers ──────────────────────────────────────────────
float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// ── Fabric noise / weave ─────────────────────────────────────────────
float fabricNoise(vec2 p) {
    return smoothstep(-0.5, 0.9, sin((p.x - p.y) * 555.0) * sin(p.y * 1444.0)) - 0.4;
}

float fabricWeave(vec2 p) {
    const mat2 m = mat2(1.6, 1.2, -1.2, 1.6);
    float f = 0.4 * fabricNoise(p);
    f += 0.3 * fabricNoise(p = m * p);
    f += 0.2 * fabricNoise(p = m * p);
    return f + 0.1 * fabricNoise(m * p);
}

// ── Silk surface value ────────────────────────────────────────────────
float silk(vec2 uv, float t, float mr, float wScale) {
    float s = sin(5.0 * (uv.x + uv.y + cos(2.0 * uv.x + 5.0 * uv.y))
                  + sin(12.0 * (uv.x + uv.y)) - t);
    s = 0.7 + 0.3 * (s * s * 0.5 + s);
    s *= 0.9 + 0.6 * fabricWeave(uv * mr * 0.0006 * wScale);
    return s * 0.9 + 0.1;
}

// ── Silk surface derivative (sheen) ──────────────────────────────────
float silkDerivative(vec2 uv, float t) {
    float xy = uv.x + uv.y;
    float d = (5.0 * (1.0 - 2.0 * sin(2.0 * uv.x + 5.0 * uv.y))
               + 12.0 * cos(12.0 * xy))
            * cos(5.0 * (cos(2.0 * uv.x + 5.0 * uv.y) + xy)
                  + sin(12.0 * xy) - t);
    return 0.005 * d * (sign(d) + 3.0);
}

// ── Smooth exponential easing for time-based events ──────────────────
float smoothPulse(float ph, float rise, float fall) {
    return smoothstep(0.0, rise, ph) * smoothstep(fall, fall - rise, ph);
}

// ── Tone-map a single UV coordinate into a carbon colour ─────────────
vec3 carbonSurface(vec2 uv, float t, float mr, float wScale) {
    float s = sqrt(silk(uv, t, mr, wScale));
    float d = silkDerivative(uv, t);

    vec3 c = vec3(s);
    c += sheenStrength * sheenColor.rgb * d;
    c += sheenColor.rgb * pow(max(d, 0.0), 6.0) * highlight * 0.6;
    c *= 1.0 - max(0.0, 0.8 * d);

    if (invert) {
        c = pow(c, 0.3 / vec3(0.52, 0.5, 0.4));
        c = 1.0 - c;
        float maxC = max(max(fabricColor.r, fabricColor.g), fabricColor.b + 0.01);
        c *= fabricColor.rgb / maxC;
        float sheenMask = max(0.0, d) * sheenStrength;
        c += sheenColor.rgb * sheenMask * 0.3;
    } else {
        c = pow(c, vec3(0.52, 0.5, 0.4));
        c *= fabricColor.rgb * 4.0;
    }
    return clamp(c, 0.0, 1.0);
}

// ── Background parallax plane ─────────────────────────────────────────
vec3 bgPlaneSurface(vec2 uv, float t) {
    // Coarser, slower weave for the receding plane
    float s = fabricWeave(uv * 0.0003 * bgWeaveScale) * 0.5 + 0.5;
    float glow = pow(s, 2.2);
    // Mix between a very dark base and the bg plane tint
    vec3 c = mix(bgPlaneColor.rgb * 0.3, bgPlaneColor.rgb, glow);
    // Subtle slow shimmer
    float sh = 0.5 + 0.5 * sin(uv.x * 2.1 + t * 0.22)
                       * cos(uv.y * 1.7 + t * 0.17);
    c += bgPlaneColor.rgb * sh * 0.08;
    return clamp(c, 0.0, 1.0);
}

// ── 9-tap Gaussian blur approximation for bloom ───────────────────────
//    We compute it analytically (no extra passes) using nearby UV samples.
vec3 gaussianBloomSample(vec2 uv, float t, float mr) {
    float r = bloomRadius;
    // Offsets for a 3×3 Gaussian kernel (weights: center=4, edge=2, corner=1, /16)
    vec3 acc = vec3(0.0);
    float wTotal = 0.0;

    // 3×3 kernel
    for (int dy = -1; dy <= 1; dy++) {
        for (int dx = -1; dx <= 1; dx++) {
            float w = (dx == 0 && dy == 0) ? 4.0
                    : (dx == 0 || dy == 0) ? 2.0 : 1.0;
            vec2 off = vec2(float(dx), float(dy)) * r;
            vec2 sUV = uv + off;
            acc += w * carbonSurface(sUV, t, mr, weaveScale);
            wTotal += w;
        }
    }
    return acc / wTotal;
}

void main() {
    float mr = min(RENDERSIZE.x, RENDERSIZE.y);
    vec2 fragNorm = gl_FragCoord.xy / RENDERSIZE.xy; // 0..1 in screen space

    // ── Smooth time with a gentle exponential envelope ─────────────────
    float t = TIME * speed;

    // ── Audio reactivity values ─────────────────────────────────────────
    float audioWave    = audioBass * audioReact * 0.08;
    float audioShimmer = audioHigh * audioReact * 3.0;

    // ── Mouse deformation in normalised UV space ────────────────────────
    vec2 mNorm = mousePos; // already 0..1

    // ════════════════════════════════════════════════════════════════════
    // LAYER 1 — Background parallax plane (deeper, slower, coarser)
    // ════════════════════════════════════════════════════════════════════
    // Parallax offset: mouse shifts the BG plane in the opposite direction
    // at a reduced rate, giving depth.
    vec2 parallaxOff = (mNorm - 0.5) * parallaxAmt * 0.06;
    vec2 bgUV = gl_FragCoord.xy;
    // Slow independent scroll for the BG
    bgUV += vec2(t * scrollSpeed * 0.025, t * scrollSpeed * 0.022);
    bgUV -= parallaxOff * mr;          // parallax shift (opposite to fg)

    vec3 bgCol = bgPlaneSurface(bgUV, t);

    // ════════════════════════════════════════════════════════════════════
    // LAYER 2 — Foreground carbon weave
    // ════════════════════════════════════════════════════════════════════
    vec2 uv = gl_FragCoord.xy / mr;

    // Smooth scroll (diagonal drift)
    uv += vec2(t * scrollSpeed * 0.05, t * scrollSpeed * 0.04);

    // Wave distortion — eased with smoothstep envelope at audio peaks
    float waveBoost = 1.0 + audioWave * 8.0;
    uv.y += waveAmount * 0.03 * sin(8.0 * uv.x - t) * waveBoost;
    uv.x += waveAmount * 0.015 * sin(6.0 * uv.y - t * 0.7) * waveBoost;

    // Depth drape — slow large-scale undulation
    if (depthBump > 0.0) {
        float drape = sin(uv.x * 3.0 + t * 0.4) * cos(uv.y * 2.5 + t * 0.5);
        uv.x += drape * depthBump * 0.012;
        uv.y += drape * depthBump * 0.008;
    }

    // Audio ripple
    uv.y += audioWave * sin(12.0 * uv.x + t * 3.0);
    uv.x += audioWave * 0.5 * cos(10.0 * uv.y + t * 2.5);

    // Mouse deformation — fabric pushes away from cursor (FG layer)
    // FG parallax: shift in same direction as mouse, more strongly
    vec2 fgParallax = (mNorm - 0.5) * parallaxAmt * 0.03;
    uv += fgParallax;

    vec2 mUV = mNorm * RENDERSIZE.xy / mr;
    float mDist = distance(mUV, uv);
    float mPush  = smoothstep(0.5, 0.0, mDist) * mouseDeform * 0.08;
    uv += normalize(uv - mUV + 0.001) * mPush;

    // ── Core carbon surface (foreground) ─────────────────────────────
    vec3 fgCol = carbonSurface(uv, t, mr, weaveScale);

    // Audio shimmer on highlights
    float d = silkDerivative(uv, t);
    fgCol += sheenColor.rgb * audioShimmer * 0.15 * max(0.0, d) * 2.0 + sheenColor.rgb * audioShimmer * 0.06;
    fgCol = clamp(fgCol, 0.0, 1.0);

    // ════════════════════════════════════════════════════════════════════
    // SOFT GLOW BLOOM — analytically sampled neighbourhood
    // ════════════════════════════════════════════════════════════════════
    // Detect bright highlights (sheen peaks) and spread them softly.
    vec3 blurredFg = gaussianBloomSample(uv, t, mr);
    // Luminance-threshold — only very bright regions bloom
    float lum = dot(blurredFg, vec3(0.299, 0.587, 0.114));
    float threshold = 0.55;
    vec3  bloomContrib = max(blurredFg - threshold, vec3(0.0)) / (1.0 - threshold);
    // Tint bloom toward sheen colour for that hot-glint look
    bloomContrib = mix(bloomContrib, bloomContrib * sheenColor.rgb * 1.4, 0.5);
    fgCol += bloomContrib * bloomStrength;
    fgCol = clamp(fgCol, 0.0, 1.0);

    // ════════════════════════════════════════════════════════════════════
    // COMPOSITE — blend BG plane behind FG weave
    // ════════════════════════════════════════════════════════════════════
    // Use luminance of FG as approximate opacity for compositing.
    // Dark carbon lets a little BG show through the weave valleys.
    float fgLum    = dot(fgCol, vec3(0.299, 0.587, 0.114));
    float bgBlend  = clamp(1.0 - fgLum * 2.0, 0.0, 1.0) * parallaxAmt * 0.6;
    vec3  c = mix(fgCol, bgCol, bgBlend);

    // ── Periodic heat-shimmer surprise ───────────────────────────────
    {
        float _ph   = fract(TIME / 30.0);
        float _f    = smoothPulse(_ph, 0.06, 0.30);
        float _diag = fragNorm.x + fragNorm.y;
        float _wave = sin((_diag - _ph * 4.0) * 14.0) * 0.5 + 0.5;
        c = mix(c, c * (0.8 + 0.4 * _wave), _f * 0.5);
    }

    // ── Audio pulse — sheen brightens with high-band energy ────────────
    float audioKnee = pow(smoothstep(0.05, 0.85, audioHigh), 1.2) * audioReact;
    c += sheenColor.rgb * audioKnee * 0.5;
    c *= 1.0 + audioKnee * 0.35;

    // ── Output ───────────────────────────────────────────────────────
    c = clamp(c, 0.0, 1.0);

    if (transparentBg) {
        float alpha = dot(c, vec3(0.299, 0.587, 0.114));
        gl_FragColor = vec4(c, alpha);
    } else {
        gl_FragColor = vec4(c, 1.0);
    }
}