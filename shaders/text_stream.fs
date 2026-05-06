/*{
  "DESCRIPTION": "Stream — horizontal lanes of text flowing across the screen at different speeds, like a river of words. Each lane loops the message endlessly with a slight vertical sway, edge-fades at the L/R borders, and parallax-scaled brightness (closer lanes brighter). Audio bass nudges flow speed; treble adds sparkle on character edges. Pair with an Easel Background Layer for backdrop.",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator", "Text", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": "STREAM OF CONSCIOUSNESS FLOWING ACROSS THE SCREEN", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Inter","Times","Caslon","Outfit"] },
    { "NAME": "laneCount", "LABEL": "Lanes", "TYPE": "long", "DEFAULT": 4, "VALUES": [1,2,3,4,5,6,7,8], "LABELS": ["1","2","3","4","5","6","7","8"] },
    { "NAME": "flowSpeed", "LABEL": "Flow Speed", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "direction", "LABEL": "Direction", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2], "LABELS": ["Left","Right","Alternate"] },
    { "NAME": "speedSpread", "LABEL": "Speed Spread", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "swayAmp", "LABEL": "Sway", "TYPE": "float", "DEFAULT": 0.012, "MIN": 0.0, "MAX": 0.06 },
    { "NAME": "swayFreq", "LABEL": "Sway Frequency", "TYPE": "float", "DEFAULT": 1.4, "MIN": 0.0, "MAX": 6.0 },
    { "NAME": "textScale", "LABEL": "Text Size", "TYPE": "float", "DEFAULT": 0.075, "MIN": 0.025, "MAX": 0.18 },
    { "NAME": "kerning", "LABEL": "Kerning", "TYPE": "float", "DEFAULT": 1.05, "MIN": 0.7, "MAX": 1.6 },
    { "NAME": "edgeFade", "LABEL": "Edge Fade", "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.0, "MAX": 0.4 },
    { "NAME": "haloStrength", "LABEL": "Halo", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "hdrBoost", "LABEL": "HDR Boost", "TYPE": "float", "DEFAULT": 1.6, "MIN": 1.0, "MAX": 3.5 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "inputTex", "LABEL": "Background Layer", "TYPE": "image" },
    { "NAME": "bgOpacity", "LABEL": "BG Layer Opacity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.02, 0.03, 0.06, 1.0] },
    { "NAME": "textA", "LABEL": "Text Hot", "TYPE": "color", "DEFAULT": [0.20, 0.95, 1.00, 1.0] },
    { "NAME": "textB", "LABEL": "Text Cool", "TYPE": "color", "DEFAULT": [0.95, 0.30, 0.85, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent BG", "TYPE": "bool", "DEFAULT": 0.0 }
  ]
}*/

// =====================================================================
// Stream — multi-lane scrolling text. Each lane shows the message
// repeating endlessly. Lanes scroll at different speeds (parallax),
// alternate direction in mode 2, sway vertically, and edge-fade at
// L/R borders. LINEAR HDR.
// =====================================================================

#define MAX_LANES 8

// ─── Font atlas ─────────────────────────────────────────────────────
float sampleChar(int ch, vec2 uv) {
    if (ch < 0 || ch > 36) return 0.0;
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    return texture2D(fontAtlasTex, vec2((float(ch) + uv.x) / 37.0, uv.y)).r;
}

int getChar(int slot) {
    if (slot ==  0) return int(msg_0);
    if (slot ==  1) return int(msg_1);
    if (slot ==  2) return int(msg_2);
    if (slot ==  3) return int(msg_3);
    if (slot ==  4) return int(msg_4);
    if (slot ==  5) return int(msg_5);
    if (slot ==  6) return int(msg_6);
    if (slot ==  7) return int(msg_7);
    if (slot ==  8) return int(msg_8);
    if (slot ==  9) return int(msg_9);
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
    if (slot == 47) return int(msg_47);
    return -1;
}

int charCount() {
    int n = int(msg_len);
    if (n <= 0) return 8;
    if (n > 48) return 48;
    return n;
}

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

void main() {
    vec2 res = RENDERSIZE;
    vec2 uv  = gl_FragCoord.xy / res;
    float aspect = res.x / res.y;

    // Centered, aspect-corrected. Y goes 0(bottom) → 1(top).
    vec2 p;
    p.x = (uv.x - 0.5) * aspect;
    p.y = uv.y;

    float audio = clamp(audioReact, 0.0, 2.0);
    float bass  = audioBass;
    float treb  = audioHigh;

    // ─── Background ────────────────────────────────────────────────
    vec3 layerBG = IMG_NORM_PIXEL(inputTex, uv).rgb;
    vec3 col = mix(bgColor.rgb, layerBG, bgOpacity);

    int total      = charCount();
    int lanes      = int(laneCount);
    if (lanes > MAX_LANES) lanes = MAX_LANES;
    float charH    = textScale;
    float charW    = charH * (5.0 / 7.0);
    float kern     = charW * kerning;
    float ftotal   = float(total);
    int   dirMode  = int(direction);

    // Lane layout: distribute lanes evenly across vertical space
    // [0.15, 0.85] with one lane height of margin so chars don't
    // touch the screen edges.
    float laneTop    = 0.85;
    float laneBottom = 0.15;
    float laneSpan   = laneTop - laneBottom;
    float laneStep   = (lanes > 1) ? laneSpan / float(lanes - 1) : 0.0;

    float textMask = 0.0;
    vec3  textCol  = vec3(0.0);
    float halo     = 0.0;

    // Bass micro-boost on flow speed.
    float effSpeed = flowSpeed * (1.0 + 0.4 * bass * audio);

    for (int li = 0; li < MAX_LANES; li++) {
        if (li >= lanes) break;
        float fli = float(li);
        // Each lane has its own deterministic seed for speed/direction.
        float seed = hash11(fli * 7.13 + 0.5);

        // Speed varies per lane within ±speedSpread fraction.
        float laneSpd = effSpeed * (1.0 + (seed - 0.5) * 2.0 * speedSpread);
        // Direction: 0=left, 1=right, 2=alternate (even=left, odd=right).
        float dir = (dirMode == 1) ? 1.0
                  : (dirMode == 2) ? (mod(fli, 2.0) < 0.5 ? -1.0 : 1.0)
                  : -1.0;
        float scroll = TIME * laneSpd * dir;

        // Lane vertical center + sway.
        float laneY = (lanes > 1)
            ? laneBottom + laneStep * fli
            : 0.5;
        laneY += swayAmp * sin(TIME * swayFreq + fli * 1.7 + seed * 6.0);

        // Skip if pixel is well outside this lane's vertical range.
        float dy = p.y - laneY;
        if (abs(dy) > charH * 0.9) continue;

        // World-x of the pixel relative to scroll. Each repeated copy of
        // the message is cellTotal wide. Use mod() for seamless wrap.
        float cellTotal = ftotal * kern;
        float wx = p.x + scroll + aspect * 0.5;
        float modX = mod(wx, cellTotal);
        if (modX < 0.0) modX += cellTotal;

        // Char index in the message.
        float colF   = modX / kern;
        int   colIdx = int(floor(colF));
        if (colIdx < 0 || colIdx >= total) continue;

        int ch = getChar(colIdx);
        // Cell-local UV.
        vec2 cellUV;
        cellUV.x = fract(colF);
        cellUV.y = (dy / charH) + 0.5;

        float s = sampleChar(ch, cellUV);
        s = smoothstep(0.18, 0.55, s);
        if (s < 0.001) continue;

        // Edge fade — dissolve characters near L/R borders so they
        // appear/disappear softly rather than popping at the screen edge.
        float ux = uv.x;
        float fadeL = smoothstep(0.0, edgeFade, ux);
        float fadeR = 1.0 - smoothstep(1.0 - edgeFade, 1.0, ux);
        float edge  = fadeL * fadeR;

        // Lane parallax: faster lanes (closer) read brighter; slower
        // lanes recede.
        float parallax = clamp(abs(laneSpd) / max(effSpeed, 0.001), 0.4, 1.4);
        // Color: gradient between the two text colors based on the lane.
        float tMix = (lanes > 1) ? fli / float(lanes - 1) : 0.5;
        vec3  baseColor = mix(textA.rgb, textB.rgb, tMix);
        // Treble shimmer adds a per-char sparkle.
        float shimmer = 1.0 + 0.45 * treb * audio
                      * smoothstep(0.6, 1.0, hash11(floor(modX * 13.0) + fli));

        float w = s * edge * parallax * shimmer;
        textMask = max(textMask, w);
        textCol  = mix(textCol, baseColor, w);

        // Soft halo around each char. Half-tone falloff.
        float haloR = charW * 1.2;
        float dlen  = length(vec2((cellUV.x - 0.5) * charW, dy));
        halo += exp(-pow(dlen / haloR, 2.0)) * w * 0.25;
    }

    halo = clamp(halo, 0.0, 4.0);

    // Compose: background → halo glow → ink chars.
    col += textCol * halo * haloStrength;
    // HDR ring on bright halo zones.
    float ring = clamp(halo - 0.4, 0.0, 1.5);
    col += textCol * pow(ring, 2.0) * hdrBoost * 0.4;

    // Solid ink characters.
    col = mix(col, textCol, textMask);

    float alpha = 1.0;
    if (transparentBg) {
        alpha = clamp(textMask + halo * 0.3, 0.0, 1.0);
        col   = textCol;
    }

    gl_FragColor = vec4(col, alpha);
}
