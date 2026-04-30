/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Y2K vaporwave chaos — pink-and-teal sky over receding perspective grid floor, magenta sun with horizontal bars, scrolling katakana, then a swarm of bouncing screensaver primitives (hearts, stars, sparkles, WordArt blocks, lo-fi smileys) careening across the canvas. Procedural Giphy substitute: hashed shapes spawn, drift, scale, hue-rotate, dissolve. Late-night mall in a fever dream.",
  "INPUTS": [
    { "NAME": "horizonY", "LABEL": "Horizon", "TYPE": "float", "MIN": 0.40, "MAX": 0.75, "DEFAULT": 0.55 },
    { "NAME": "skyTopColor", "LABEL": "Sky Top", "TYPE": "color", "DEFAULT": [1.0, 0.42, 0.71, 1.0] },
    { "NAME": "skyHorizonColor", "LABEL": "Sky Horizon", "TYPE": "color", "DEFAULT": [0.36, 0.85, 0.76, 1.0] },
    { "NAME": "sunSize", "LABEL": "Sun Size", "TYPE": "float", "MIN": 0.05, "MAX": 0.40, "DEFAULT": 0.22 },
    { "NAME": "sunBars", "LABEL": "Sun Bars", "TYPE": "float", "MIN": 0.0, "MAX": 12.0, "DEFAULT": 6.0 },
    { "NAME": "gridDensity", "LABEL": "Grid Density", "TYPE": "float", "MIN": 4.0, "MAX": 24.0, "DEFAULT": 12.0 },
    { "NAME": "gridPersp", "LABEL": "Grid Perspective", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 1.8 },
    { "NAME": "gridSpeed", "LABEL": "Grid Speed", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25 },
    { "NAME": "y2kCount", "LABEL": "Y2K Object Count", "TYPE": "float", "MIN": 0.0, "MAX": 20.0, "DEFAULT": 12.0 },
    { "NAME": "y2kSpeed", "LABEL": "Y2K Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6 },
    { "NAME": "y2kSize", "LABEL": "Y2K Size", "TYPE": "float", "MIN": 0.02, "MAX": 0.20, "DEFAULT": 0.07 },
    { "NAME": "y2kChaos", "LABEL": "Chaos", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7 },
    { "NAME": "scanlineAmp", "LABEL": "Scanlines", "TYPE": "float", "MIN": 0.0, "MAX": 0.4, "DEFAULT": 0.18 },
    { "NAME": "katakanaIntensity", "LABEL": "Katakana", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6 },
    { "NAME": "chromaShift", "LABEL": "Chroma Shift", "TYPE": "float", "MIN": 0.0, "MAX": 0.02, "DEFAULT": 0.008 },
    { "NAME": "crtBloom", "LABEL": "CRT Bloom", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "inputTex", "LABEL": "Texture (optional GIF source)", "TYPE": "image" }
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

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Y2K shape SDFs — heart, 5-point star, sparkle plus, WordArt pill, smiley
float sdHeart(vec2 p) {
    p.x = abs(p.x);
    if (p.y + p.x > 1.0)
        return sqrt(dot(p - vec2(0.25, 0.75), p - vec2(0.25, 0.75))) - sqrt(2.0) / 4.0;
    return sqrt(min(dot(p - vec2(0.00, 1.00), p - vec2(0.00, 1.00)),
                    dot(p - 0.5 * max(p.x + p.y, 0.0), p - 0.5 * max(p.x + p.y, 0.0))))
         * sign(p.x - p.y);
}

float sdStar5(vec2 p, float r) {
    const vec2 k1 = vec2(0.809016994, -0.587785252);
    const vec2 k2 = vec2(-k1.x, k1.y);
    p.x = abs(p.x);
    p -= 2.0 * max(dot(k1, p), 0.0) * k1;
    p -= 2.0 * max(dot(k2, p), 0.0) * k2;
    p.x = abs(p.x);
    p.y -= r;
    vec2 ba = vec2(-0.309016994, 0.951056516) * 0.4;
    float h = clamp(dot(p, ba) / dot(ba, ba), 0.0, 1.0);
    return length(p - ba * h) * sign(p.y * ba.x - p.x * ba.y);
}

float sdSparkle(vec2 p) {
    // 4-point sparkle plus shape
    return min(max(abs(p.x) - 0.08, abs(p.y) - 0.30),
               max(abs(p.y) - 0.08, abs(p.x) - 0.30));
}

float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

float sdSmiley(vec2 p, float r) {
    float face  = length(p) - r;
    float eyeL  = length(p - vec2(-r * 0.35, r * 0.25)) - r * 0.10;
    float eyeR  = length(p - vec2( r * 0.35, r * 0.25)) - r * 0.10;
    // Mouth = annulus segment lower half
    float mr1 = abs(length(p - vec2(0.0, -r * 0.05)) - r * 0.45) - r * 0.06;
    float mouth = max(mr1, -p.y);
    float feat = min(min(eyeL, eyeR), mouth);
    return max(face, -feat);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Sky gradient
    vec3 sky = mix(skyHorizonColor.rgb, skyTopColor.rgb,
                   smoothstep(horizonY - 0.05, 1.0, uv.y));
    vec3 col = sky;

    // Sun
    vec2 sc = vec2(0.5, horizonY);
    vec2 sd = uv - sc; sd.x *= aspect;
    float sr = sunSize * (1.0 + audioBass * audioReact * 0.06);
    if (length(sd) < sr) {
        float ty = clamp((sd.y / sr + 1.0) * 0.5, 0.0, 1.0);
        vec3 sunC = mix(vec3(0.98, 0.45, 0.20),
                        vec3(1.0, 0.20, 0.62), ty);
        if (sunBars > 0.0) {
            float barY = sd.y / sr;
            float barMask = step(0.0,
                sin(barY * sunBars * 3.14159 + 0.4 + TIME * 0.5));
            sunC = mix(sunC, sky, barMask * 0.55);
        }
        col = sunC;
    }

    // Perspective grid floor
    if (uv.y < horizonY) {
        float dh = max(horizonY - uv.y, 0.001);
        vec2 gridUV = vec2((uv.x - 0.5) / (dh * gridPersp + 0.05),
                           1.0 / dh - TIME * gridSpeed
                              * (1.0 + audioMid * audioReact * 0.4));
        float gx = abs(fract(gridUV.x * gridDensity) - 0.5);
        float gy = abs(fract(gridUV.y) - 0.5);
        float lineW = 0.04 * dh;
        float line = smoothstep(0.5 - lineW, 0.5, max(gx, gy));
        vec3 floorBase = mix(vec3(0.10, 0.05, 0.18),
                             vec3(0.55, 0.10, 0.45), uv.y / horizonY);
        col = mix(floorBase, vec3(1.0, 0.42, 0.85), line);
        col = mix(col, sky, smoothstep(horizonY - 0.04, horizonY, uv.y));
    }

    // ============= Y2K CHAOS LAYER ============================================
    // N bouncing/spinning hashed primitives — hearts, stars, sparkles,
    // WordArt pills, lo-fi smileys — each with its own trajectory,
    // colour, scale pulse, and respawn cycle. Procedural Giphy
    // substitute: random shapes, random colours, random animations.
    int N = int(clamp(y2kCount, 0.0, 20.0));
    for (int i = 0; i < 20; i++) {
        if (i >= N) break;
        float fi = float(i);

        // Per-object respawn cycle — each respawns at a different rate
        // so the canvas constantly has new things appearing.
        float cycle = floor(TIME * y2kSpeed * (0.3 + hash11(fi * 1.3) * 0.7)
                            + fi * 0.7);
        float life  = fract(TIME * y2kSpeed * (0.3 + hash11(fi * 1.3) * 0.7)
                            + fi * 0.7);

        // Hashed start + direction per cycle
        float h1 = hash11(fi + cycle * 7.13);
        float h2 = hash11(fi + cycle * 13.7);
        float h3 = hash11(fi + cycle * 19.3);
        float h4 = hash11(fi + cycle * 23.1);

        // Position — start at hashed point, drift along hashed velocity.
        vec2 startP = vec2(h1, h2);
        vec2 vel    = (vec2(h3, h4) - 0.5) * 1.5;
        vec2 ctr    = startP + vel * life * y2kChaos;
        // Bounce-wrap so objects stay roughly on screen
        ctr = vec2(0.5 + sin(ctr.x * 3.14159) * 0.45,
                   0.5 + sin(ctr.y * 3.14159) * 0.45);

        // Per-object scale pulse + hue
        float sz   = y2kSize * (0.6 + h1 * 0.8)
                   * (0.7 + 0.3 * sin(TIME * 4.0 + fi))
                   * (1.0 + audioBass * audioReact * 0.4);
        float hue  = fract(h2 + TIME * 0.05);
        vec3 shapeCol = hsv2rgb(vec3(hue, 0.85, 0.95));

        // Visibility: fade in/out over cycle so respawn isn't a pop.
        float vis = smoothstep(0.0, 0.15, life)
                  * smoothstep(1.0, 0.85, life);

        // Per-object rotation
        float rot = TIME * (0.5 + h3 * 2.0)
                  + fi * 1.7;
        float ca  = cos(rot), sa = sin(rot);
        vec2 d    = uv - ctr;
        d.x *= aspect;
        vec2 lp   = vec2(ca * d.x - sa * d.y,
                         sa * d.x + ca * d.y) / max(sz, 1e-4);

        // Pick a shape per object (5 kinds)
        int kind = int(hash11(fi * 31.7) * 5.0);
        float sd = 1.0;
        if      (kind == 0) sd = sdHeart(lp + vec2(0.0, 0.5));
        else if (kind == 1) sd = sdStar5(lp, 0.85);
        else if (kind == 2) sd = sdSparkle(lp * 1.2);
        else if (kind == 3) sd = sdRoundBox(lp, vec2(0.85, 0.40), 0.20);
        else                sd = sdSmiley(lp, 0.85);

        // Fill + outline
        if (sd < 0.0) {
            col = mix(col, shapeCol, vis);
        }
        // Bright outline
        col = mix(col, vec3(1.0),
                  smoothstep(0.04, 0.0, abs(sd)) * vis * 0.5);
    }

    // Optional input texture as an animated overlay (where Giphy GIFs
    // would land if Easel grows an HTTPSource that fetches them).
    if (IMG_SIZE_inputTex.x > 0.0) {
        vec3 src = texture(inputTex, fract(uv + vec2(sin(TIME * 0.3) * 0.05, 0.0))).rgb;
        // Chroma-key the dark parts and overlay
        float sL = dot(src, vec3(0.299, 0.587, 0.114));
        col = mix(col, src, smoothstep(0.20, 0.40, sL) * 0.6);
    }

    // Chromatic aberration — luminance-modulated channel push (no input
    // texture required).
    if (chromaShift > 0.0) {
        col.r = mix(col.r,
                    col.r + col.g * 0.05 * sin(uv.y * 60.0 + TIME * 0.7),
                    chromaShift * 5.0);
        col.b = mix(col.b,
                    col.b - col.g * 0.05 * sin(uv.y * 60.0 + TIME * 0.7 + 1.57),
                    chromaShift * 5.0);
    }

    // Scanlines
    float scan = sin(gl_FragCoord.y * 1.4) * 0.5 + 0.5;
    col *= 1.0 - scanlineAmp * (1.0 - scan)
            * (0.8 + audioBass * audioReact * 0.3);

    // Katakana — ribbon at top edge
    {
        float total = 0.0;
        for (int g = 0; g < 6; g++) {
            float fg = float(g);
            vec2 origin = vec2(0.05 + fg * 0.15, 0.85);
            vec2 ld = (uv - origin) * vec2(60.0, 28.0);
            if (ld.x < 0.0 || ld.y < 0.0
             || ld.x > 8.0 || ld.y > 4.0) continue;
            vec2 ci = floor(ld);
            float h = hash21(ci + floor(TIME * (0.4 + audioHigh * audioReact * 1.2)));
            float vert = step(h, 0.55) * step(0.30, fract(ld.x))
                       * step(fract(ld.x), 0.55);
            float bar  = step(0.55, h) * step(h, 0.85)
                       * step(0.40, fract(ld.y))
                       * step(fract(ld.y), 0.62);
            total = max(total, max(vert, bar));
        }
        col = mix(col, vec3(0.7, 1.0, 0.85), total * katakanaIntensity);
    }

    // CRT bloom
    float L = dot(col, vec3(0.299, 0.587, 0.114));
    col += pow(L, 2.0) * crtBloom * 0.35
        * (0.7 + audioLevel * audioReact * 0.4);

    // VHS posterize
    col = floor(col * 16.0) / 16.0;

    gl_FragColor = vec4(col, 1.0);
}
