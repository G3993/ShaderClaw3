/*
{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Text with moving highlight sweep — chrome-like shine effect",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": "ETHEREA", "MAX_LENGTH": 12 },
    { "NAME": "speed", "TYPE": "float", "MIN": 0.2, "MAX": 5.0, "DEFAULT": 1.0 },
    { "NAME": "textColor", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "shineColor", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "bgColor", "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.05, 1.0] },
    { "NAME": "textScale", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "shineWidth", "TYPE": "float", "MIN": 0.05, "MAX": 0.5, "DEFAULT": 0.15 },
    { "NAME": "transparentBg", "TYPE": "bool", "DEFAULT": true }
  ]
}
*/

// --- Text uniforms (ISF text type auto-generates these) ---
// uniform float msg_0 .. msg_11, msg_len

// --- Atlas-based font engine (A-Z + digits + space) ---
// Replaces the legacy hardcoded 5x7 packed-bit charData() bitmap with a
// sample from the shared, high-resolution fontAtlasTex — same
// charPixel/sampleChar helper used by the migrated text_*.fs shaders.

float charPixel(int ch, float col, float row) {
    if (ch < 0 || ch > 36) return 0.0;
    vec2 uv = vec2(col / 5.0, row / 7.0);
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    return smoothstep(0.1, 0.55, texture2D(fontAtlasTex, vec2((float(ch) + uv.x) / 37.0, uv.y)).r);
}

// Accept both atlas indices (0-36, fed by the app) and raw ASCII codes
// (fed by some hosts): map ASCII letters/digits/space onto atlas indices.
int normChar(int c) {
    if (c >= 65 && c <= 90) return c - 65;      // ASCII 'A'-'Z'
    if (c >= 97 && c <= 122) return c - 97;     // ASCII 'a'-'z'
    if (c == 32) return 26;                     // ASCII space
    if (c >= 48 && c <= 57) return c - 48 + 27; // ASCII '0'-'9'
    return c;                                    // already an atlas index
}

int getChar(int slot) {
    int c;
    if (slot == 0) c = int(msg_0); else if (slot == 1) c = int(msg_1);
    else if (slot == 2) c = int(msg_2); else if (slot == 3) c = int(msg_3);
    else if (slot == 4) c = int(msg_4); else if (slot == 5) c = int(msg_5);
    else if (slot == 6) c = int(msg_6); else if (slot == 7) c = int(msg_7);
    else if (slot == 8) c = int(msg_8); else if (slot == 9) c = int(msg_9);
    else if (slot == 10) c = int(msg_10); else c = int(msg_11);
    return normChar(c);
}

int charCount() { int n = int(msg_len); return n > 0 ? n : 1; }

// --- Main ---

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Aspect-corrected coordinates (centered)
    vec2 p = vec2((uv.x - 0.5) * aspect + 0.5, uv.y);

    // Character layout dimensions
    float charW = 0.09 * textScale;
    float charH = charW * 1.5;
    float gap = charW * 0.25;
    int numChars = charCount();
    float totalW = float(numChars) * charW + float(numChars - 1) * gap;

    // Center text horizontally, vertically at 0.5
    float startX = 0.5 - totalW * 0.5;
    float startY = 0.5 - charH * 0.5;

    // Determine if this pixel is on a text character and its x position in the text block
    float textMask = 0.0;
    float pixelX = 0.0;
    float pixelY = 0.0;

    for (int i = 0; i < 12; i++) {
        if (i >= numChars) break;

        int ch = getChar(i);

        // Skip space characters (26 = space)
        if (ch == 26) continue;

        float cx = startX + float(i) * (charW + gap);
        float cy = startY;

        // Check if pixel is within this character's bounding box
        if (p.x >= cx && p.x < cx + charW && p.y >= cy && p.y < cy + charH) {
            // Map to character grid (5 cols x 7 rows)
            float localX = (p.x - cx) / charW;
            float localY = (p.y - cy) / charH;

            float col = localX * 5.0;
            float row = localY * 7.0;

            if (col >= 0.0 && col < 5.0 && row >= 0.0 && row < 7.0) {
                float px = charPixel(ch, col, row);
                if (px > 0.5) {
                    textMask = 1.0;
                    pixelX = p.x - startX;
                    pixelY = (p.y - startY) / charH;
                }
            }
        }
    }

    // --- Shine calculation ---
    // The shine sweeps diagonally from left to right across the text, wrapping around
    float sweepRange = totalW + shineWidth * 2.0;
    float shinePos = mod(TIME * speed * 0.3, sweepRange) - shineWidth;

    // Diagonal shine: offset x position by y to create an angled sweep line
    float diagonalOffset = pixelY * 0.3 * totalW;
    float distToShine = abs(pixelX - shinePos + diagonalOffset);

    // Broad shine glow (falls off smoothly from the shine center)
    float shineAmount = smoothstep(shineWidth, 0.0, distToShine);

    // Tight specular highlight at the very center of the shine band
    float specWidth = shineWidth * 0.12;
    float specular = smoothstep(specWidth, 0.0, distToShine);
    specular = specular * specular; // sharpen the highlight

    // --- Compose final color ---
    vec4 finalColor;

    if (textMask > 0.5) {
        // Base text color
        vec3 col = textColor.rgb;

        // Mix in shine glow — boost above 1.0 for bloom
        col = mix(col, shineColor.rgb * 1.5, shineAmount);

        // Add specular highlight — HDR peak for bloom pipeline
        col += shineColor.rgb * specular * 4.0;

        // Black ink shadow side: pixels far from shine are deep-dark for contrast
        float darkSide = smoothstep(shineWidth * 2.0, shineWidth * 5.0, distToShine);
        col = mix(col, col * 0.15, darkSide * 0.6);

        finalColor = vec4(col, textColor.a);
    } else {
        // Background
        if (transparentBg) {
            finalColor = vec4(0.0, 0.0, 0.0, 0.0);
        } else {
            finalColor = bgColor;
        }
    }

    gl_FragColor = finalColor;
}
