/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "ASCII Rain — falling columns using your custom message text",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": "ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.66 },
    { "NAME": "intensity", "LABEL": "Trail Length", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "density", "LABEL": "Columns", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.75 },
    { "NAME": "textScale", "LABEL": "Size", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 0.3 },
    { "NAME": "textColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.0, 1.0, 0.2, 1.0] },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": true }
  ]
}*/

// Atlas-only font sampling (row 0=top, 7=bottom → invert V for WebGL atlas)
float charPixel(int ch, float col, float row) {
    if (ch < 0 || ch > 36) return 0.0;
    vec2 uv = vec2((float(ch) + col / 5.0) / 37.0, 1.0 - row / 7.0);
    if (uv.x < 0.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    return smoothstep(0.1, 0.55, texture2D(fontAtlasTex, uv).r);
}

int getChar(int slot) {
    if (slot == 0)  return int(msg_0);  if (slot == 1)  return int(msg_1);
    if (slot == 2)  return int(msg_2);  if (slot == 3)  return int(msg_3);
    if (slot == 4)  return int(msg_4);  if (slot == 5)  return int(msg_5);
    if (slot == 6)  return int(msg_6);  if (slot == 7)  return int(msg_7);
    if (slot == 8)  return int(msg_8);  if (slot == 9)  return int(msg_9);
    if (slot == 10) return int(msg_10); if (slot == 11) return int(msg_11);
    if (slot == 12) return int(msg_12); if (slot == 13) return int(msg_13);
    if (slot == 14) return int(msg_14); if (slot == 15) return int(msg_15);
    if (slot == 16) return int(msg_16); if (slot == 17) return int(msg_17);
    if (slot == 18) return int(msg_18); if (slot == 19) return int(msg_19);
    if (slot == 20) return int(msg_20); if (slot == 21) return int(msg_21);
    if (slot == 22) return int(msg_22); if (slot == 23) return int(msg_23);
    if (slot == 24) return int(msg_24); if (slot == 25) return int(msg_25);
    if (slot == 26) return int(msg_26); if (slot == 27) return int(msg_27);
    if (slot == 28) return int(msg_28); if (slot == 29) return int(msg_29);
    if (slot == 30) return int(msg_30); if (slot == 31) return int(msg_31);
    if (slot == 32) return int(msg_32); if (slot == 33) return int(msg_33);
    if (slot == 34) return int(msg_34); if (slot == 35) return int(msg_35);
    if (slot == 36) return int(msg_36); if (slot == 37) return int(msg_37);
    if (slot == 38) return int(msg_38); if (slot == 39) return int(msg_39);
    if (slot == 40) return int(msg_40); if (slot == 41) return int(msg_41);
    if (slot == 42) return int(msg_42); if (slot == 43) return int(msg_43);
    if (slot == 44) return int(msg_44); if (slot == 45) return int(msg_45);
    if (slot == 46) return int(msg_46); return int(msg_47);
}

int charCount() {
    int n = int(msg_len);
    return n > 0 ? n : 1;
}

float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// ═══════════════════════════════════════════════════════════════════════
// ASCII RAIN — falling columns of your custom message text
// ═══════════════════════════════════════════════════════════════════════

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    int numChars = charCount();

    // Grid layout
    float cols = floor(mix(12.0, 50.0, density) / textScale);
    float cellW = 1.0 / cols;
    float cellH = cellW * (7.0 / 5.0) * aspect;
    float rows = ceil(1.0 / cellH);

    // Cell coordinates (top-down)
    float flippedY = 1.0 - uv.y;
    float ci = floor(uv.x / cellW);
    float ri = floor(flippedY / cellH);
    float lx = fract(uv.x / cellW);
    float ly = fract(flippedY / cellH);

    float trailLen = mix(5.0, 30.0, intensity);

    // Rain drops — 3 staggered drops per column for coverage
    float brightness = 0.0;
    float headGlow = 0.0;
    float bestDist = 999.0;  // track which drop is closest (for char sequencing)
    float bestDropHead = 0.0;

    for (int d = 0; d < 3; d++) {
        float dSeed = hash(ci * 13.7 + float(d) * 91.3);
        float dSpeed = (0.3 + dSeed * 1.0) * speed * 3.0;
        float dPhase = hash(ci * 7.3 + float(d) * 43.1) * 100.0;
        float period = rows + trailLen + 10.0;
        float dropPos = mod(TIME * dSpeed + dPhase, period);
        float dist = dropPos - ri;
        if (dist >= 0.0 && dist < trailLen) {
            float t = dist / trailLen;
            float b = 1.0 - t;
            if (b * b > brightness) {
                brightness = b * b;
                bestDist = dist;
                bestDropHead = dropPos;
            }
            if (dist < 1.5) {
                headGlow = max(headGlow, 1.0 - dist / 1.5);
            }
        }
    }

    // Dim background characters
    brightness = max(brightness, 0.04);

    // Character selection: spell the message sequentially down each rain streak
    // The character index is based on distance from the drop head, so the
    // message reads top-to-bottom within each falling trail
    int ch = 26; // space
    if (bestDist < 999.0) {
        // Within a rain streak: invert so message reads top-to-bottom
        int charIdx = numChars - 1 - int(mod(bestDist, float(numChars)));
        if (charIdx < 0) charIdx = 0;
        ch = getChar(charIdx);
    } else {
        // Background: pick a static character from msg based on grid position
        int charIdx = numChars - 1 - int(mod(ci + ri, float(numChars)));
        if (charIdx < 0) charIdx = 0;
        ch = getChar(charIdx);
    }

    // Render glyph
    float pixel = 0.0;
    float gc = lx * 5.0, gr = ly * 7.0;
    if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0) {
        pixel = charPixel(ch, gc, gr);
    }

    // Per-column rainbow: each stream gets a distinct hue, multiplied by textColor
    float colHue = fract(ci * 0.08);
    vec4 _K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 _hp = abs(fract(colHue + _K.xyz) * 6.0 - _K.www);
    vec3 colTint = clamp(_hp - _K.xxx, 0.0, 1.0);
    vec3 baseRainbow = textColor.rgb * colTint;
    // HDR trail: peak at 2.5× for bloom pipeline
    vec3 charCol = baseRainbow * brightness * 2.5;
    // HDR head burst: warm white spike at 3.5× to clearly mark the leading character
    vec3 headFlash = mix(colTint, vec3(1.0), 0.3) * 3.5;
    charCol = mix(charCol, headFlash, headGlow * 0.85);

    vec3 fc = transparentBg ? vec3(0.0) : bgColor.rgb;
    float alpha = transparentBg ? 0.0 : 1.0;

    if (pixel > 0.5) {
        fc = charCol;
        if (transparentBg) alpha = clamp(brightness, 0.0, 1.0);
    }

    gl_FragColor = vec4(fc, alpha);
}
