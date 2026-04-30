/*{
  "DESCRIPTION": "American Flag — 13 stripes + 50-star canton with billowing wind, fabric shading, and audio-reactive gusts",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "windStrength", "LABEL": "Wind", "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.0, "MAX": 0.25 },
    { "NAME": "windSpeed", "LABEL": "Wind Speed", "TYPE": "float", "DEFAULT": 1.6, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "windScale", "LABEL": "Wind Scale", "TYPE": "float", "DEFAULT": 4.5, "MIN": 0.5, "MAX": 12.0 },
    { "NAME": "flagFill", "LABEL": "Flag Fill", "TYPE": "float", "DEFAULT": 0.92, "MIN": 0.5, "MAX": 1.0 },
    { "NAME": "shadeStrength", "LABEL": "Fabric Shading", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "fabricNoise", "LABEL": "Fabric Noise", "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.0, "MAX": 0.4 },
    { "NAME": "audioGust", "LABEL": "Audio Gust", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "audioFlap", "LABEL": "Audio Flap", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "starGlow", "LABEL": "Star Glow", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "starSize", "LABEL": "Star Size", "TYPE": "float", "DEFAULT": 0.42, "MIN": 0.15, "MAX": 0.7 },
    { "NAME": "redColor", "LABEL": "Red", "TYPE": "color", "DEFAULT": [0.698, 0.133, 0.203, 1.0] },
    { "NAME": "whiteColor", "LABEL": "White", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "blueColor", "LABEL": "Blue (Canton)", "TYPE": "color", "DEFAULT": [0.235, 0.234, 0.431, 1.0] },
    { "NAME": "backgroundColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.04, 1.0] }
  ]
}*/

#ifdef GL_ES
precision highp float;
#endif

#define PI 3.14159265358979
#define TAU 6.28318530718

// Official US flag proportions
#define FLAG_RATIO 1.9       // fly / hoist
#define CANTON_W   (2.0/5.0) // canton width  / fly
#define CANTON_H   (7.0/13.0)// canton height / hoist
#define STRIPE_H   (1.0/13.0)

float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash12(i);
    float b = hash12(i + vec2(1.0, 0.0));
    float c = hash12(i + vec2(0.0, 1.0));
    float d = hash12(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// 5-pointed star SDF in flag-local star cell coords (-1..1).
// Returns signed distance: <0 inside the star.
float starSDF(vec2 p, float r) {
    // 5-pointed star via folding into 1/10th sector
    const float k1x = 0.809016994; // cos(PI/5)
    const float k1y = 0.587785252; // sin(PI/5)
    const float k2x = 0.309016994; // cos(2PI/5)
    const float k2y = 0.951056516; // sin(2PI/5)
    p.x = abs(p.x);
    p -= 2.0 * max(dot(vec2(-k1x, k1y), p), 0.0) * vec2(-k1x, k1y);
    p -= 2.0 * max(dot(vec2( k1x, k1y), p), 0.0) * vec2( k1x, k1y);
    p.x = abs(p.x);
    p.y -= r;
    vec2 ba = vec2(k2x, -k2y) * r * 0.5;  // inner radius factor
    float h = clamp(dot(p, ba) / dot(ba, ba), 0.0, r);
    return length(p - ba * h) * sign(p.y * ba.x - p.x * ba.y);
}

void main() {
    vec2 Res = RENDERSIZE;
    vec2 uv  = isf_FragNormCoord.xy;
    float aspect = Res.x / Res.y;

    // ===== Audio =====
    float bass  = audioBass;
    float midF  = audioMid;
    float high  = audioHigh;
    float level = audioLevel;

    // ===== Place flag rectangle centered, fitted to viewport with FLAG_RATIO =====
    float vpRatio = aspect;
    float fillW, fillH;
    if (vpRatio > FLAG_RATIO) {
        fillH = flagFill;
        fillW = fillH * FLAG_RATIO / vpRatio;
    } else {
        fillW = flagFill;
        fillH = fillW * vpRatio / FLAG_RATIO;
    }
    vec2 flagMin = vec2(0.5 - fillW * 0.5, 0.5 - fillH * 0.5);
    vec2 flagMax = vec2(0.5 + fillW * 0.5, 0.5 + fillH * 0.5);

    vec2 flagUV = (uv - flagMin) / (flagMax - flagMin); // 0..1 inside flag
    bool inside = flagUV.x >= 0.0 && flagUV.x <= 1.0 && flagUV.y >= 0.0 && flagUV.y <= 1.0;

    // ===== Wind: sample-time displacement of flag-local UV =====
    // Hoist (left edge) is anchored, fly (right edge) waves more.
    float anchor = clamp(flagUV.x, 0.0, 1.0);
    float gust = (1.0 + audioGust * (0.5 + bass * 1.8));
    float flap = audioFlap * (0.6 + high * 1.5);

    float t = TIME * windSpeed;
    float wx = flagUV.x * windScale;
    float wy = flagUV.y * windScale * 0.6;

    // Layered traveling waves left -> right
    float wave  = sin(wx * 1.5 - t * 1.2 + flagUV.y * 2.0);
    wave       += 0.55 * sin(wx * 3.1 - t * 1.9 + flagUV.y * 4.3);
    wave       += 0.30 * sin(wx * 5.7 - t * 2.7 + flagUV.y * 7.1 + flap * 6.0);

    float crossWave = sin(wy * 2.3 + t * 0.9) * 0.5;

    float disp = wave + crossWave;
    // Anchor at hoist (left), grow to fly (right)
    float anchorMask = pow(anchor, 1.4);

    vec2 warped = flagUV;
    warped.y += disp * windStrength * anchorMask * gust;
    warped.x += sin(wy * 1.7 + t * 0.7) * windStrength * 0.35 * anchorMask * gust;

    // Brightness from wave slope (fabric shading)
    float slope = cos(wx * 1.5 - t * 1.2 + flagUV.y * 2.0)
                + 0.55 * cos(wx * 3.1 - t * 1.9 + flagUV.y * 4.3) * 1.5;
    float shade = 1.0 + slope * 0.18 * shadeStrength * anchorMask;

    // Soft micro folds
    float folds = vnoise(vec2(warped.x * 18.0, warped.y * 26.0) + t * 0.3);
    shade *= 1.0 + (folds - 0.5) * fabricNoise;

    // ===== Inside warped flag: stripes + canton =====
    vec3 col = backgroundColor.rgb;

    if (inside) {
        // Use warped coords for color sampling, but clamp so we don't sample outside
        vec2 fc = clamp(warped, vec2(0.0), vec2(1.0));

        // Stripes: row index 0 (top) .. 12 (bottom). Top stripe is red.
        // flagUV.y in 0..1 with 0 at bottom by ISF convention; remap so 0 = top.
        float yTop = 1.0 - fc.y;
        float row  = floor(yTop / STRIPE_H);
        float redStripe = mod(row, 2.0) < 0.5 ? 1.0 : 0.0;
        vec3 stripeCol = mix(whiteColor.rgb, redColor.rgb, redStripe);

        // Canton in upper-left: x in [0, CANTON_W], yTop in [0, CANTON_H]
        bool inCanton = fc.x < CANTON_W && yTop < CANTON_H;

        vec3 surface = stripeCol;

        if (inCanton) {
            surface = blueColor.rgb;

            // Star grid: 9 rows tall, alternating 6 and 5 stars per row (offset).
            // Rows 0,2,4,6,8 -> 6 stars; rows 1,3,5,7 -> 5 stars (shifted by 0.5 cell)
            // Cell size: width = CANTON_W / 6, height = CANTON_H / 9 (using 6-cell base)
            // Standard layout uses 12x12 sub-grid; we approximate with even spacing.
            vec2 cantonUV = vec2(fc.x / CANTON_W, yTop / CANTON_H); // 0..1
            // 11 columns x 9 rows of star slots, stars on alternating slots
            float colsBase = 6.0;
            float rows     = 9.0;

            // Map to a fine grid where rows alternate offset
            float ry = cantonUV.y * rows;
            float rIdx = floor(ry);
            float rFrac = ry - rIdx;
            bool oddRow = mod(rIdx, 2.0) > 0.5;
            float colCount = oddRow ? 5.0 : 6.0;
            float xOffset  = oddRow ? 0.5 / 6.0 : 0.0;

            float rx = (cantonUV.x - xOffset) * colsBase;
            float cIdx = floor(rx);
            float cFrac = rx - cIdx;

            // Only draw a star if cIdx is in range for this row
            bool validCol = cIdx >= 0.0 && cIdx < colCount;

            // Local star coord in -1..1 inside cell
            vec2 sp = vec2(cFrac - 0.5, rFrac - 0.5) * 2.0;
            // Account for cell aspect (canton is wider than tall per cell)
            float cellW = CANTON_W / colsBase * fillW;
            float cellH = CANTON_H / rows    * fillH;
            sp.x *= (cellW / cellH) / vpRatio;

            float starR = starSize;
            float d = starSDF(sp, starR);

            if (validCol) {
                float starMask = smoothstep(0.02, -0.01, d);
                surface = mix(surface, whiteColor.rgb, starMask);
                // Soft glow
                float glow = exp(-max(d, 0.0) * 22.0) * starGlow * (0.6 + high * 1.5);
                surface += vec3(0.6, 0.7, 1.0) * glow;
            }
        }

        col = surface * shade;

        // Edge highlight along fold crests
        float crest = smoothstep(0.6, 1.0, slope * anchorMask);
        col += vec3(0.04) * crest;

        // Audio level lift
        col *= (1.0 + level * 0.15);

        // Subtle drop shadow at flag edge (inside)
        float edgeDist = min(min(flagUV.x, 1.0 - flagUV.x), min(flagUV.y, 1.0 - flagUV.y));
        float edgeShade = smoothstep(0.0, 0.02, edgeDist);
        col *= mix(0.75, 1.0, edgeShade);
    } else {
        // Background with vignette
        float vig = smoothstep(1.1, 0.3, length(uv - 0.5) * 1.6);
        col = backgroundColor.rgb * mix(0.5, 1.0, vig);

        // Soft drop shadow under the flag
        vec2 sOff = vec2(0.012, -0.014);
        vec2 sUV  = uv - sOff;
        bool inShadow = sUV.x > flagMin.x && sUV.x < flagMax.x
                     && sUV.y > flagMin.y && sUV.y < flagMax.y;
        if (inShadow) {
            float dx = min(sUV.x - flagMin.x, flagMax.x - sUV.x);
            float dy = min(sUV.y - flagMin.y, flagMax.y - sUV.y);
            float sd = min(dx, dy);
            float shadow = smoothstep(0.0, 0.03, sd) * 0.45;
            col = mix(col, vec3(0.0), shadow);
        }
    }

    // Surprise: every ~40s, a single star in the canton briefly fans
    // through the spectrum (Jasper Johns "Three Flags" subversion).
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 40.0);
        float _f  = smoothstep(0.0, 0.06, _ph) * smoothstep(0.30, 0.18, _ph);
        // Rough canton zone (top-left ~38% x 54%)
        if (_suv.x < 0.38 && _suv.y > 0.46) {
            float _h = fract(TIME * 0.6);
            vec3 _rainbow = 0.5 + 0.5 * cos(6.28318 * _h + vec3(0.0, 2.094, 4.188));
            col = mix(col, _rainbow, _f * 0.35 * step(0.86, dot(col, vec3(0.299, 0.587, 0.114))));
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
