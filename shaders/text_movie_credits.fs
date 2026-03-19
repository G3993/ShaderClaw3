/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Movie credits scroll — text scrolls upward like cinema end credits with live transcript support",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": "ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "oscSpeed", "LABEL": "Osc Speed", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 0.0 },
    { "NAME": "oscAmount", "LABEL": "Osc Amount", "TYPE": "float", "MIN": 0.0, "MAX": 0.2, "DEFAULT": 0.0 },
    { "NAME": "oscSpread", "LABEL": "Osc Spread", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5 },
    { "NAME": "textColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.85, 0.78, 0.62, 1.0] },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "textScale", "LABEL": "Size", "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "condensed", "LABEL": "Condensed", "TYPE": "float", "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "speed", "LABEL": "Scroll Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.3 },
    { "NAME": "lineCount", "LABEL": "Lines", "TYPE": "float", "MIN": 2.0, "MAX": 12.0, "DEFAULT": 7.0 },
    { "NAME": "lineSpacing", "LABEL": "Line Spacing", "TYPE": "float", "MIN": 1.0, "MAX": 3.0, "DEFAULT": 1.5 },
    { "NAME": "shimmer", "LABEL": "Shimmer", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": true }
  ]
}*/

// Movie end credits — all lines show the user's text (live transcript).
// Scrolls upward continuously like cinema credits.

// Crisp character sampling — tight smoothstep for sharp edges
float sampleCharAA(int ch, vec2 uv, float pxSize) {
    if (ch < 0 || ch > 36) return 0.0;
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    float raw = texture2D(fontAtlasTex, vec2((float(ch) + uv.x) / 37.0, uv.y)).r;
    // Tight anti-aliasing: just enough to avoid jaggies, not enough to blur
    float edge = clamp(pxSize * 0.8, 0.01, 0.08);
    return smoothstep(0.45 - edge, 0.45 + edge, raw);
}

int getChar(int slot) {
    if (slot == 0)  return int(msg_0);
    if (slot == 1)  return int(msg_1);
    if (slot == 2)  return int(msg_2);
    if (slot == 3)  return int(msg_3);
    if (slot == 4)  return int(msg_4);
    if (slot == 5)  return int(msg_5);
    if (slot == 6)  return int(msg_6);
    if (slot == 7)  return int(msg_7);
    if (slot == 8)  return int(msg_8);
    if (slot == 9)  return int(msg_9);
    if (slot == 10) return int(msg_10);
    if (slot == 11) return int(msg_11);
    if (slot == 12) return int(msg_12);
    if (slot == 13) return int(msg_13);
    if (slot == 14) return int(msg_14);
    if (slot == 15) return int(msg_15);
    if (slot == 16) return int(msg_16);
    if (slot == 17) return int(msg_17);
    if (slot == 18) return int(msg_18);
    if (slot == 19) return int(msg_19);
    if (slot == 20) return int(msg_20);
    if (slot == 21) return int(msg_21);
    if (slot == 22) return int(msg_22);
    if (slot == 23) return int(msg_23);
    if (slot == 24) return int(msg_24);
    if (slot == 25) return int(msg_25);
    if (slot == 26) return int(msg_26);
    if (slot == 27) return int(msg_27);
    if (slot == 28) return int(msg_28);
    if (slot == 29) return int(msg_29);
    if (slot == 30) return int(msg_30);
    if (slot == 31) return int(msg_31);
    if (slot == 32) return int(msg_32);
    if (slot == 33) return int(msg_33);
    if (slot == 34) return int(msg_34);
    if (slot == 35) return int(msg_35);
    if (slot == 36) return int(msg_36);
    if (slot == 37) return int(msg_37);
    if (slot == 38) return int(msg_38);
    if (slot == 39) return int(msg_39);
    if (slot == 40) return int(msg_40);
    if (slot == 41) return int(msg_41);
    if (slot == 42) return int(msg_42);
    if (slot == 43) return int(msg_43);
    if (slot == 44) return int(msg_44);
    if (slot == 45) return int(msg_45);
    if (slot == 46) return int(msg_46);
    return int(msg_47);
}

int charCount() {
    int n = int(msg_len);
    return n > 0 ? n : 1;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Aspect-correct x coordinate (standard for text shaders)
    vec2 p = vec2((uv.x - 0.5) * aspect + 0.5, uv.y);

    vec3 col = bgColor.rgb;
    float alpha = transparentBg ? 0.0 : 1.0;
    float textMask = 0.0;

    int numLines = int(lineCount + 0.5);
    int numChars = charCount();

    // Character dimensions in aspect-corrected space — scale down on portrait
    float portraitScale = aspect < 1.0 ? aspect : 1.0;
    float cW = 0.045 * textScale * condensed * portraitScale;
    float cH = 0.045 * textScale * 1.4 * portraitScale;
    float gap = cW * 0.18;
    float cellStep = cW + gap;

    // Line height with spacing control
    float lineH = cH * lineSpacing;

    // Total block height
    float blockH = lineH * float(numLines);

    // Scrolling: text falls from top to bottom, loops seamlessly
    float totalH = blockH + 1.0; // block height + full screen gap for clean loop
    float scrollOffset = mod(TIME * speed * 0.2, totalH);

    // Pixel size in character UV space (for anti-aliasing)
    float pxSize = 1.0 / (cH * RENDERSIZE.y);

    // Total text width
    float totalW = float(numChars) * cellStep - gap;
    float startX = 0.5 - totalW * 0.5;

    // Render each line — all lines show user text
    for (int line = 0; line < 12; line++) {
        if (line >= numLines) break;

        // Line position: start above screen, fall downward
        float lineY = 1.0 + float(line) * lineH - scrollOffset;

        // Skip if off screen
        if (lineY + cH < -0.05 || lineY > 1.05) continue;

        // Fade at top and bottom edges
        float edgeFade = smoothstep(-0.02, 0.08, lineY) * smoothstep(1.02, 0.92, lineY + cH);

        // Center vertically in line slot
        float charY = lineY + (lineH - cH) * 0.5;

        // Render each character
        for (int ci = 0; ci < 48; ci++) {
            if (ci >= numChars) break;

            int ch = getChar(ci);
            if (ch == 26) continue; // space
            if (ch < 0 || ch > 36) continue;

            float cx = startX + float(ci) * cellStep;
            float oscY = oscAmount * sin(TIME * oscSpeed * 6.2832 + float(ci) * oscSpread * 3.14159);
            vec2 cellUV = vec2(
                (p.x - cx) / cW,
                (uv.y - charY - oscY) / cH
            );

            if (cellUV.x < -0.05 || cellUV.x > 1.05 || cellUV.y < -0.05 || cellUV.y > 1.05) continue;

            float s = sampleCharAA(ch, cellUV, pxSize);
            textMask = max(textMask, s * edgeFade);
        }
    }

    // Shimmer — subtle metallic gleam sweep
    if (shimmer > 0.01 && textMask > 0.01) {
        float sweep = fract(TIME * 0.08);
        float sweepPos = sweep * 1.4 - 0.2;
        float dist = abs(uv.x - sweepPos);
        float gleam = exp(-dist * dist * 80.0) * shimmer;
        textMask = min(1.0, textMask * (1.0 + gleam * 2.0));
    }

    // Composite
    col = mix(col, textColor.rgb, textMask);
    if (transparentBg) {
        alpha = textMask;
    }

    gl_FragColor = vec4(col, alpha);
}
