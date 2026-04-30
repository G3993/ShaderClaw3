/*{
  "DESCRIPTION": "ASCII art filter — converts any image/video to colored ASCII characters with ANSI palette matching",
  "CATEGORIES": ["Effect", "Text"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Source", "TYPE": "image" },
    { "NAME": "cellSize", "LABEL": "Cell Size", "TYPE": "float", "DEFAULT": 8.0, "MIN": 4.0, "MAX": 32.0 },
    { "NAME": "charSet", "LABEL": "Character Set", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Simple", "Extended", "Blocks"] },
    { "NAME": "colorMode", "LABEL": "Color Mode", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["ANSI 16", "Mono", "Custom Duo", "Full Color"] },
    { "NAME": "fgColor", "LABEL": "Foreground", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "contrast", "LABEL": "Contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 3.0 },
    { "NAME": "brightness", "LABEL": "Brightness", "TYPE": "float", "DEFAULT": 0.0, "MIN": -0.5, "MAX": 0.5 },
    { "NAME": "invert", "LABEL": "Invert", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "scanlines", "LABEL": "Scanlines", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

// ============================================================
// ASCII Filter — Shadertoy tlfXzB port to ISF
// Single-pass: cell analysis + procedural bitmap rendering
// ============================================================

// --- 16-color CGA/ANSI palette ---
vec3 ansiColor(int i) {
    if (i ==  0) return vec3(0.000, 0.000, 0.000);
    if (i ==  1) return vec3(0.667, 0.000, 0.000);
    if (i ==  2) return vec3(0.000, 0.667, 0.000);
    if (i ==  3) return vec3(0.667, 0.333, 0.000);
    if (i ==  4) return vec3(0.000, 0.000, 0.667);
    if (i ==  5) return vec3(0.667, 0.000, 0.667);
    if (i ==  6) return vec3(0.000, 0.667, 0.667);
    if (i ==  7) return vec3(0.667, 0.667, 0.667);
    if (i ==  8) return vec3(0.333, 0.333, 0.333);
    if (i ==  9) return vec3(1.000, 0.333, 0.333);
    if (i == 10) return vec3(0.333, 1.000, 0.333);
    if (i == 11) return vec3(1.000, 1.000, 0.333);
    if (i == 12) return vec3(0.333, 0.333, 1.000);
    if (i == 13) return vec3(1.000, 0.333, 1.000);
    if (i == 14) return vec3(0.333, 1.000, 1.000);
    return vec3(1.000, 1.000, 1.000);
}

// --- 5x7 procedural bitmap font ---
// Characters encoded as 35-bit bitmaps packed into vec2 (5 cols x 7 rows)
// Simple set: ' .:-=+*#%@'  (10 chars, density ramp)
vec2 charBitmap_simple(int idx) {
    if (idx == 0) return vec2(0.0, 0.0);                    // space
    if (idx == 1) return vec2(0.0, 4096.0);                 // .
    if (idx == 2) return vec2(4096.0, 4096.0);              // :
    if (idx == 3) return vec2(0.0, 14336.0);                // -
    if (idx == 4) return vec2(14336.0, 14336.0);            // =
    if (idx == 5) return vec2(4100.0, 14724.0);             // +
    if (idx == 6) return vec2(141873.0, 4564.0);            // *
    if (idx == 7) return vec2(718609.0, 23213.0);           // #
    if (idx == 8) return vec2(575022.0, 14897.0);           // %
    return vec2(1033777.0, 14897.0);                        // @
}

// Extended set: adds more ASCII density levels
vec2 charBitmap_ext(int idx) {
    if (idx == 0) return vec2(0.0, 0.0);                    // space
    if (idx == 1) return vec2(0.0, 4096.0);                 // .
    if (idx == 2) return vec2(0.0, 4.0);                    // '
    if (idx == 3) return vec2(4096.0, 4096.0);              // :
    if (idx == 4) return vec2(0.0, 14336.0);                // -
    if (idx == 5) return vec2(14336.0, 14336.0);            // =
    if (idx == 6) return vec2(4100.0, 14724.0);             // +
    if (idx == 7) return vec2(4100.0, 31460.0);             // t
    if (idx == 8) return vec2(236912.0, 14897.0);           // x
    if (idx == 9) return vec2(349601.0, 14897.0);           // o
    if (idx == 10) return vec2(141873.0, 4564.0);           // *
    if (idx == 11) return vec2(476561.0, 14897.0);          // d
    if (idx == 12) return vec2(718609.0, 23213.0);          // #
    if (idx == 13) return vec2(575022.0, 14897.0);          // %
    return vec2(1033777.0, 14897.0);                        // @
}

// Block set: uses box-drawing density (Unicode-inspired but as bitmaps)
vec2 charBitmap_block(int idx) {
    if (idx == 0) return vec2(0.0, 0.0);                    // empty
    if (idx == 1) return vec2(0.0, 28672.0);                // lower bar
    if (idx == 2) return vec2(0.0, 30576.0);                // lower half
    if (idx == 3) return vec2(28672.0, 28672.0);            // upper + lower bar
    if (idx == 4) return vec2(30576.0, 30576.0);            // bars
    if (idx == 5) return vec2(30576.0, 31744.0);            // lower heavy
    if (idx == 6) return vec2(31744.0, 30576.0);            // upper heavy
    if (idx == 7) return vec2(31744.0, 31744.0);            // heavy bars
    if (idx == 8) return vec2(1048575.0, 31744.0);          // almost full
    return vec2(1048575.0, 32767.0);                        // full block
}

float sampleBitmap(vec2 data, float col, float row) {
    float ri = floor(row);
    float rv;
    if (ri < 4.0) rv = mod(floor(data.x / pow(32.0, ri)), 32.0);
    else rv = mod(floor(data.y / pow(32.0, ri - 4.0)), 32.0);
    return mod(floor(rv / pow(2.0, 4.0 - floor(col))), 2.0);
}

// --- Palette matching ---
// Find the two closest ANSI colors to the input color
void findClosestPair(vec3 col, out int foreIdx, out int backIdx, out float mix_t) {
    float bestDist = 99.0;
    float secondDist = 99.0;
    foreIdx = 0;
    backIdx = 15;

    // Find closest color
    for (int i = 0; i < 16; i++) {
        float d = distance(col, ansiColor(i));
        if (d < bestDist) {
            bestDist = d;
            foreIdx = i;
        }
    }

    // Find second closest on the axis toward the target
    vec3 delta = col - ansiColor(foreIdx);
    float dLen = length(delta);
    if (dLen < 0.001) {
        backIdx = foreIdx;
        mix_t = 0.0;
        return;
    }
    vec3 dir = delta / dLen;
    bestDist = 99.0;

    for (int i = 0; i < 16; i++) {
        vec3 axis = ansiColor(i) - ansiColor(foreIdx);
        float aLen = length(axis);
        if (aLen < 0.001) continue;
        float alignment = dot(axis / aLen, dir);
        float score = (1.0 - alignment) + distance(col, ansiColor(i)) * 0.5;
        if (score < bestDist && i != foreIdx) {
            bestDist = score;
            backIdx = i;
        }
    }

    float fd = distance(col, ansiColor(foreIdx));
    float bd = distance(col, ansiColor(backIdx));
    mix_t = fd / max(fd + bd, 0.001);
}

// --- Main ---
void main() {
    vec2 Res = RENDERSIZE;
    vec2 px = gl_FragCoord.xy;
    vec2 uv = isf_FragNormCoord;

    // Cell grid
    float cW = cellSize;
    float cH = cellSize * 1.4; // 5:7 aspect ratio
    vec2 cell = floor(px / vec2(cW, cH));
    vec2 cellUV = mod(px, vec2(cW, cH)) / vec2(cW, cH);

    // Sample input image at cell center (2x2 supersample)
    vec2 cellBase = cell * vec2(cW, cH) / Res;
    vec2 cellStep = vec2(cW, cH) / Res;

    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    vec3 sampleCol = vec3(0.5);

    if (hasInput) {
        vec2 st = cellBase + cellStep * 0.25;
        vec2 dt = cellStep * 0.5;
        sampleCol  = texture2D(inputTex, st).rgb;
        sampleCol += texture2D(inputTex, st + vec2(dt.x, 0.0)).rgb;
        sampleCol += texture2D(inputTex, st + vec2(0.0, dt.y)).rgb;
        sampleCol += texture2D(inputTex, st + dt).rgb;
        sampleCol *= 0.25;
    }

    // Apply brightness/contrast
    sampleCol = (sampleCol - 0.5) * contrast + 0.5 + brightness;
    sampleCol = clamp(sampleCol, 0.0, 1.0);

    // Luminance for character selection
    float lum = dot(sampleCol, vec3(0.299, 0.587, 0.114));
    if (invert) lum = 1.0 - lum;

    // Select character by luminance
    int numChars;
    if (charSet < 0.5)      numChars = 10; // Simple
    else if (charSet < 1.5) numChars = 15; // Extended
    else                    numChars = 10; // Blocks

    int charIdx = int(clamp(lum * float(numChars - 1) + 0.5, 0.0, float(numChars - 1)));

    // Get bitmap
    vec2 bitmap;
    if (charSet < 0.5)      bitmap = charBitmap_simple(charIdx);
    else if (charSet < 1.5) bitmap = charBitmap_ext(charIdx);
    else                    bitmap = charBitmap_block(charIdx);

    // Sample the character glyph at this pixel's position within the cell
    // Map cellUV to 5x7 grid with slight padding
    float cropFactor = 0.7;
    vec2 glyphUV = 0.5 + (cellUV - 0.5) * cropFactor;
    float col_f = glyphUV.x * 5.0;
    float row_f = (1.0 - glyphUV.y) * 7.0; // flip Y — row 0 is top
    float charAlpha = 0.0;

    if (glyphUV.x >= 0.0 && glyphUV.x <= 1.0 && glyphUV.y >= 0.0 && glyphUV.y <= 1.0) {
        charAlpha = sampleBitmap(bitmap, col_f, row_f);
    }

    // Determine foreground and background colors
    vec3 fg, bg;

    if (colorMode < 0.5) {
        // ANSI 16 — palette match
        int foreIdx, backIdx;
        float mix_t;
        findClosestPair(sampleCol, foreIdx, backIdx, mix_t);
        fg = ansiColor(foreIdx);
        bg = ansiColor(backIdx);
    } else if (colorMode < 1.5) {
        // Mono
        fg = fgColor.rgb;
        bg = bgColor.rgb;
    } else if (colorMode < 2.5) {
        // Custom Duo — fg/bg by brightness
        fg = fgColor.rgb;
        bg = bgColor.rgb;
        // Swap based on lum to maintain contrast
        if (lum < 0.5) {
            vec3 tmp = fg;
            fg = bg;
            bg = tmp;
        }
    } else {
        // Full Color — use actual image color as foreground
        fg = sampleCol;
        bg = sampleCol * 0.15;
    }

    // Mix character foreground/background
    vec3 result = mix(bg, fg, charAlpha);

    // Scanline effect
    if (scanlines > 0.001) {
        float scanline = sin(px.y * 3.14159 / (cH * 0.5)) * 0.5 + 0.5;
        result *= 1.0 - scanlines * 0.3 * (1.0 - scanline);
    }

    // Alpha
    float alpha = 1.0;
    if (transparentBg) {
        alpha = charAlpha * 0.9 + 0.1 * step(0.05, lum);
    }

    // Surprise: every ~13s the entire field briefly translates to one
    // line of katakana that scrolls horizontally for ~0.6s — the
    // matrix ghost crossing through the ASCII.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 13.0);
        float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.20, 0.10, _ph);
        float _band = exp(-pow((_suv.y - 0.5) * 30.0, 2.0));
        result = mix(result, vec3(0.20, 0.95, 0.40), _f * _band * 0.5);
    }

    gl_FragColor = vec4(result, alpha);
}
