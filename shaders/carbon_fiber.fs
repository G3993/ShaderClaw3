/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Carbon Fiber — woven fabric texture with silk sheen, mouse deformation, and audio reactivity",
  "CREDIT": "Based on Giorgi Azmaipharashvili's silk shader (MIT License)",
  "INPUTS": [
    { "NAME": "fabricColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.08, 0.08, 0.12, 1.0] },
    { "NAME": "sheenColor", "LABEL": "Sheen", "TYPE": "color", "DEFAULT": [1.0, 0.83, 0.6, 1.0] },
    { "NAME": "weaveScale", "LABEL": "Weave Scale", "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "sheenStrength", "LABEL": "Sheen Strength", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7 },
    { "NAME": "waveAmount", "LABEL": "Wave", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.6 },
    { "NAME": "depthBump",   "LABEL": "Weave Depth",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "highlight",   "LABEL": "Highlights",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.85 },
    { "NAME": "scrollSpeed", "LABEL": "Scroll Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25 },
    { "NAME": "mouseDeform", "LABEL": "Mouse Deform", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "invert", "LABEL": "Invert", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

// ═══════════════════════════════════════════════════════════════════════
// CARBON FIBER — Woven fabric with silk lighting model
// ═══════════════════════════════════════════════════════════════════════

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

float silk(vec2 uv, float t, float mr) {
    float s = sin(5.0 * (uv.x + uv.y + cos(2.0 * uv.x + 5.0 * uv.y)) + sin(12.0 * (uv.x + uv.y)) - t);
    s = 0.7 + 0.3 * (s * s * 0.5 + s);
    s *= 0.9 + 0.6 * fabricWeave(uv * mr * 0.0006 * weaveScale);
    return s * 0.9 + 0.1;
}

float silkDerivative(vec2 uv, float t) {
    float xy = uv.x + uv.y;
    float d = (5.0 * (1.0 - 2.0 * sin(2.0 * uv.x + 5.0 * uv.y)) + 12.0 * cos(12.0 * xy))
            * cos(5.0 * (cos(2.0 * uv.x + 5.0 * uv.y) + xy) + sin(12.0 * xy) - t);
    return 0.005 * d * (sign(d) + 3.0);
}

void main() {
    float mr = min(RENDERSIZE.x, RENDERSIZE.y);
    vec2 uv = gl_FragCoord.xy / mr;

    float t = TIME * speed;

    // Continuous fabric scroll — the weave drifts diagonally so even at
    // rest the surface is alive.
    uv += vec2(t * scrollSpeed * 0.05, t * scrollSpeed * 0.04);

    // Audio-driven wave boost
    float audioWave = audioBass * audioReact * 0.02;
    float audioShimmer = audioHigh * audioReact;

    // Gentle wave distortion
    uv.y += waveAmount * 0.03 * sin(8.0 * uv.x - t);
    uv.x += waveAmount * 0.015 * sin(6.0 * uv.y - t * 0.7);

    // Depth bump — slow large-scale undulation that gives the fabric
    // a "draped over something" feel rather than perfectly flat.
    if (depthBump > 0.0) {
        float drape = sin(uv.x * 3.0 + t * 0.4) * cos(uv.y * 2.5 + t * 0.5);
        uv.x += drape * depthBump * 0.012;
        uv.y += drape * depthBump * 0.008;
    }

    // Audio ripple
    uv.y += audioWave * sin(12.0 * uv.x + t * 3.0);
    uv.x += audioWave * 0.5 * cos(10.0 * uv.y + t * 2.5);

    // Mouse deformation — fabric pushes away from cursor
    vec2 mUV = mousePos * RENDERSIZE.xy / mr;
    float mDist = distance(mUV, uv);
    float mPush = smoothstep(0.5, 0.0, mDist) * mouseDeform * 0.08;
    uv += normalize(uv - mUV + 0.001) * mPush;

    // Silk surface
    float s = sqrt(silk(uv, t, mr));

    // Surface derivative (sheen/lighting)
    float d = silkDerivative(uv, t);

    // Base grayscale fabric
    vec3 c = vec3(s);

    // Directional sheen highlight + extra raking light from upper-left
    c += sheenStrength * sheenColor.rgb * d;
    c += sheenColor.rgb * pow(max(d, 0.0), 6.0) * highlight * 0.6;

    // Shadow in the folds
    c *= 1.0 - max(0.0, 0.8 * d);

    // Audio shimmer on highlights
    c += sheenColor.rgb * audioShimmer * 0.15 * max(0.0, d) * 2.0;

    // Tone mapping — invert for dark carbon look or keep bright silk
    if (invert) {
        c = pow(c, 0.3 / vec3(0.52, 0.5, 0.4));
        c = 1.0 - c;
        // Tint with fabric color
        c *= fabricColor.rgb / max(max(fabricColor.r, fabricColor.g), fabricColor.b + 0.01);
        // Re-add sheen on top for that carbon fiber glint
        float sheenMask = max(0.0, d) * sheenStrength;
        c += sheenColor.rgb * sheenMask * 0.3;
    } else {
        c = pow(c, vec3(0.52, 0.5, 0.4));
        c *= fabricColor.rgb * 4.0;
    }

    c = clamp(c, 0.0, 1.0);

    // Surprise: every ~30s a heat-shimmer ripples across one diagonal —
    // the carbon weave catches a thermal flux for ~1.5s.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 30.0);
        float _f  = smoothstep(0.0, 0.06, _ph) * smoothstep(0.30, 0.18, _ph);
        float _diag = _suv.x + _suv.y;
        float _wave = sin((_diag - _ph * 4.0) * 14.0) * 0.5 + 0.5;
        c = mix(c, c * (0.8 + 0.4 * _wave), _f * 0.5);
    }

    if (transparentBg) {
        float lum = dot(c, vec3(0.299, 0.587, 0.114));
        gl_FragColor = vec4(c, lum);
    } else {
        gl_FragColor = vec4(c, 1.0);
    }
}
