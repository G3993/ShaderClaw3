/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Vaporwave after Macintosh Plus' Floral Shoppe (2011) — pink-and-teal vertical sky over a perspective-warped grid floor receding to a horizon, magenta sun, classical SDF bust silhouette, scanlines, mistranslated katakana, CRT bloom. Late-night mall in a dream.",
  "INPUTS": [
    { "NAME": "horizonY", "LABEL": "Horizon", "TYPE": "float", "MIN": 0.40, "MAX": 0.75, "DEFAULT": 0.55 },
    { "NAME": "skyTopColor", "LABEL": "Sky Top", "TYPE": "color", "DEFAULT": [1.0, 0.42, 0.71, 1.0] },
    { "NAME": "skyHorizonColor", "LABEL": "Sky Horizon", "TYPE": "color", "DEFAULT": [0.36, 0.85, 0.76, 1.0] },
    { "NAME": "sunSize", "LABEL": "Sun Size", "TYPE": "float", "MIN": 0.05, "MAX": 0.40, "DEFAULT": 0.22 },
    { "NAME": "sunBars", "LABEL": "Sun Bars", "TYPE": "float", "MIN": 0.0, "MAX": 12.0, "DEFAULT": 6.0 },
    { "NAME": "gridDensity", "LABEL": "Grid Density", "TYPE": "float", "MIN": 4.0, "MAX": 24.0, "DEFAULT": 12.0 },
    { "NAME": "gridPersp", "LABEL": "Grid Perspective", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 1.8 },
    { "NAME": "gridSpeed", "LABEL": "Grid Speed", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25 },
    { "NAME": "bustSize", "LABEL": "Bust Size", "TYPE": "float", "MIN": 0.10, "MAX": 0.45, "DEFAULT": 0.26 },
    { "NAME": "bustHover", "LABEL": "Bust Hover", "TYPE": "float", "MIN": 0.0, "MAX": 0.05, "DEFAULT": 0.012 },
    { "NAME": "scanlineAmp", "LABEL": "Scanlines", "TYPE": "float", "MIN": 0.0, "MAX": 0.4, "DEFAULT": 0.18 },
    { "NAME": "katakanaIntensity", "LABEL": "Katakana", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6 },
    { "NAME": "chromaShift", "LABEL": "Chroma Shift", "TYPE": "float", "MIN": 0.0, "MAX": 0.02, "DEFAULT": 0.004 },
    { "NAME": "crtBloom", "LABEL": "CRT Bloom", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "useTex", "LABEL": "Tex on Bust", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

// Marble noise — perlin-ish bands of swirled grain, slow-drifting so the
// bust shimmers like real polished marble under stage lights.
vec3 marble(vec2 uv) {
    float n = sin(uv.x * 6.0 + vnoise(uv * 8.0 + TIME * 0.08) * 4.0);
    n = n * 0.5 + 0.5;
    return mix(vec3(0.85, 0.83, 0.80), vec3(0.95, 0.92, 0.86), n);
}

// Greek-bust silhouette SDF — head + neck + base. Cartoonish enough that
// the silhouette reads at any size.
float bustSDF(vec2 p, float s) {
    // Head — circle
    float head = length(p - vec2(0.0, s * 0.18)) - s * 0.30;
    // Neck — narrowing rectangle
    float neck = max(abs(p.x) - s * 0.13,
                     abs(p.y + s * 0.08) - s * 0.18);
    // Shoulders — wide rounded rectangle
    float sh = max(abs(p.x) - s * 0.55,
                   abs(p.y + s * 0.40) - s * 0.16);
    sh = length(max(vec2(abs(p.x) - s * 0.4,
                          abs(p.y + s * 0.40) - s * 0.10), 0.0)) - s * 0.16;
    // Profile cut — diagonal slice on the right (gives Greek-bust profile feel)
    float profile = p.x - p.y * 0.18 - s * 0.35;
    float body = min(min(head, neck), sh);
    return max(body, profile);
}

// Sparse procedural katakana — stacked vertical strokes with horizontal
// crossbars at hashed positions. Reads as Japanese-ish at distance.
float katakana(vec2 uv, float t) {
    float total = 0.0;
    for (int g = 0; g < 6; g++) {
        float fg = float(g);
        vec2 origin = vec2(0.05 + fg * 0.15, 0.85);
        vec2 ld = (uv - origin) * vec2(60.0, 28.0);
        if (ld.x < 0.0 || ld.y < 0.0 || ld.x > 8.0 || ld.y > 4.0) continue;
        vec2 ci = floor(ld);
        float h = hash21(ci + floor(t * 3.0));
        float vert = step(h, 0.55) * step(0.30, fract(ld.x))
                   * step(fract(ld.x), 0.55);
        float bar  = step(0.55, h) * step(h, 0.85)
                   * step(0.40, fract(ld.y))
                   * step(fract(ld.y), 0.62);
        total = max(total, max(vert, bar));
    }
    return total;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Sky gradient
    vec3 sky = mix(skyHorizonColor.rgb, skyTopColor.rgb,
                   smoothstep(horizonY - 0.05, 1.0, uv.y));
    vec3 col = sky;

    // Sun — flat magenta circle behind bust, with horizontal bars
    // (vaporwave 80s aesthetic — sun cut by horizontal stripes).
    vec2 sc = vec2(0.5, horizonY);
    vec2 sd = uv - sc; sd.x *= aspect;
    float sr = sunSize * (1.0 + audioBass * audioReact * 0.06);
    if (length(sd) < sr) {
        // Vertical gradient inside sun, magenta to orange.
        float ty = clamp((sd.y / sr + 1.0) * 0.5, 0.0, 1.0);
        vec3 sunC = mix(vec3(0.98, 0.45, 0.20),
                        vec3(1.0, 0.20, 0.62), ty);
        // Horizontal bars cutting the sun — scroll upward over time so
        // the sun reads as alive, not a static gradient.
        if (sunBars > 0.0) {
            float barY = sd.y / sr;
            float barMask = step(0.0,
                sin(barY * sunBars * 3.14159 + 0.4 + TIME * 0.5));
            sunC = mix(sunC, sky, barMask * 0.55);
        }
        col = sunC;
    }

    // Perspective grid floor — only below horizon.
    if (uv.y < horizonY) {
        // Distance from horizon (smaller = closer to vanish point)
        float dh = max(horizonY - uv.y, 0.001);
        // Perspective transform: x scales with depth, y advances with TIME.
        vec2 gridUV = vec2((uv.x - 0.5) / (dh * gridPersp + 0.05),
                           1.0 / dh - TIME * gridSpeed
                              * (1.0 + audioMid * audioReact * 0.4));
        // Grid line distance — both axes
        float gx = abs(fract(gridUV.x * gridDensity) - 0.5);
        float gy = abs(fract(gridUV.y) - 0.5);
        float lineW = 0.04 * dh; // lines thin at horizon, thick close
        float line = smoothstep(0.5 - lineW, 0.5, max(gx, gy));
        // Floor base colour — purple to teal vertical fade
        vec3 floorBase = mix(vec3(0.10, 0.05, 0.18),
                             vec3(0.55, 0.10, 0.45), uv.y / horizonY);
        col = mix(floorBase, vec3(1.0, 0.42, 0.85), line);
        // Fade floor toward horizon
        col = mix(col, sky, smoothstep(0.0, 0.04, dh) * 0.0
                          + smoothstep(horizonY - 0.04, horizonY, uv.y));
    }

    // Bust SDF — silhouette in front of sun
    vec2 bC = vec2(0.5, horizonY + 0.04
                  + sin(TIME * 0.4) * bustHover
                    * (1.0 + audioLevel * audioReact));
    vec2 bD = uv - bC; bD.x *= aspect;
    float bsd = bustSDF(bD, bustSize);
    if (bsd < 0.0) {
        vec3 bustCol;
        if (useTex && IMG_SIZE_inputTex.x > 0.0) {
            bustCol = texture(inputTex,
                              (uv - bC) / bustSize * 0.5 + 0.5).rgb;
            bustCol = mix(bustCol, vec3(0.95, 0.85, 0.92), 0.4);
        } else {
            bustCol = marble(bD * 4.0);
        }
        col = bustCol;
    }
    // Hairline outline
    col = mix(col, vec3(0.18, 0.10, 0.18),
              smoothstep(0.003, 0.0, abs(bsd)));

    // Chromatic aberration — works in procedural mode too. Uses a
    // luminance-modulated channel push so the shift breathes with
    // brightness instead of needing a real texture sample.
    if (chromaShift > 0.0) {
        col.r = mix(col.r,
                    col.r + col.g * 0.05 * sin(uv.y * 60.0 + TIME * 0.7),
                    chromaShift * 5.0);
        col.b = mix(col.b,
                    col.b - col.g * 0.05 * sin(uv.y * 60.0 + TIME * 0.7 + 1.57),
                    chromaShift * 5.0);
    }

    // Scanlines — sinusoidal modulation along screen-y in pixel space
    float scan = sin(gl_FragCoord.y * 1.4) * 0.5 + 0.5;
    col *= 1.0 - scanlineAmp * (1.0 - scan)
            * (0.8 + audioBass * audioReact * 0.3);

    // Katakana glyph clusters in mint top-edge band
    float kg = katakana(uv, TIME * (0.4 + audioHigh * audioReact * 1.2));
    col = mix(col, vec3(0.7, 1.0, 0.85), kg * katakanaIntensity);

    // CRT bloom — additive based on luminance
    float L = dot(col, vec3(0.299, 0.587, 0.114));
    col += pow(L, 2.0) * crtBloom * 0.35
        * (0.7 + audioLevel * audioReact * 0.4);

    // VHS posterize — quantize colour to a small bit depth for the
    // characteristic vaporwave colour banding.
    col = floor(col * 16.0) / 16.0;

    gl_FragColor = vec4(col, 1.0);
}
