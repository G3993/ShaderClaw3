/*{
  "DESCRIPTION": "Chat — speech-bubble note taker. Splits the message into chunks and emits each chunk as a small chat bubble that floats up the screen. Each bubble cycles through a 6-slot palette so consecutive messages get wildly different colors. Bubbles alternate left/right sides like a chaotic group chat. Drop a long sentence in MSG and watch it self-organize. Tighter kerning so chunks read like dense one-liners.",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator", "Text"],
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": "HEY DID YOU SEE THIS LOOKS CRAZY RIGHT YEAH ITS WILD", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Inter","Times","Caslon","Outfit"] },
    { "NAME": "bubbleCount", "LABEL": "Bubbles On Screen", "TYPE": "long", "DEFAULT": 5, "VALUES": [2,3,4,5,6,7], "LABELS": ["2","3","4","5","6","7"] },
    { "NAME": "spawnRate", "LABEL": "Spawn Rate", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.1, "MAX": 1.5 },
    { "NAME": "floatSpeed", "LABEL": "Float Speed", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.05, "MAX": 0.6 },
    { "NAME": "wobble", "LABEL": "Wobble", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "textScale", "LABEL": "Text Size", "TYPE": "float", "DEFAULT": 0.038, "MIN": 0.018, "MAX": 0.07 },
    { "NAME": "kerning", "LABEL": "Kerning", "TYPE": "float", "DEFAULT": 0.92, "MIN": 0.7, "MAX": 1.4 },
    { "NAME": "bubblePadding", "LABEL": "Bubble Padding", "TYPE": "float", "DEFAULT": 0.022, "MIN": 0.005, "MAX": 0.05 },
    { "NAME": "cornerRadius", "LABEL": "Corner Radius", "TYPE": "float", "DEFAULT": 0.025, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "autoTextColor", "LABEL": "Auto Text Color", "TYPE": "bool", "DEFAULT": 1.0 },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.06, 0.07, 0.10, 1.0] },
    { "NAME": "color1", "LABEL": "Bubble 1", "TYPE": "color", "DEFAULT": [0.10, 0.55, 1.00, 1.0] },
    { "NAME": "color2", "LABEL": "Bubble 2", "TYPE": "color", "DEFAULT": [1.00, 0.20, 0.55, 1.0] },
    { "NAME": "color3", "LABEL": "Bubble 3", "TYPE": "color", "DEFAULT": [0.30, 1.00, 0.45, 1.0] },
    { "NAME": "color4", "LABEL": "Bubble 4", "TYPE": "color", "DEFAULT": [1.00, 0.75, 0.10, 1.0] },
    { "NAME": "color5", "LABEL": "Bubble 5", "TYPE": "color", "DEFAULT": [0.65, 0.20, 1.00, 1.0] },
    { "NAME": "color6", "LABEL": "Bubble 6", "TYPE": "color", "DEFAULT": [0.10, 0.95, 0.95, 1.0] },
    { "NAME": "manualTextColor", "LABEL": "Manual Text", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent BG", "TYPE": "bool", "DEFAULT": 0.0 }
  ]
}*/

// ===========================================================
// Chat — chunks the message into bubbles that float upward
// alternating sides. Each bubble has a lifetime; new ones
// spawn at the bottom while old ones drift off the top.
// LINEAR HDR.
// ===========================================================

#define MAX_BUBBLES 7

// ─── Font atlas ────────────────────────────────────────────
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
    if (n <= 0) return 12;
    if (n > 48) return 48;
    return n;
}

// ─── SDF rounded rectangle with tail ────────────────────────
// p in bubble-local coords (center at 0). hb = box half-extent.
// r = corner radius. (`half` is a GLSL reserved word, hence `hb`.)
float sdRoundedRectTail(vec2 p, vec2 hb, float r, float tailX, float tailY) {
    vec2 q = abs(p) - (hb - vec2(r));
    float box = min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r;

    // Tail: small triangle pointing away from box bottom corner.
    if (tailX != 0.0) {
        // Tail tip at (tailX, tailY), base at the bubble's bottom edge.
        vec2 tipP   = vec2(tailX, tailY);
        // Distance to the tail-shape — approx a circular notch.
        float td = length(p - tipP * 0.55) - 0.012;
        return min(box, td);
    }
    return box;
}

// ─── hash for per-bubble jitter ─────────────────────────────
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

    int total       = charCount();
    int bubbles     = int(bubbleCount);
    if (bubbles > MAX_BUBBLES) bubbles = MAX_BUBBLES;
    float charH     = textScale;
    float charW     = charH * (5.0 / 7.0);
    // Audio bass micro-boost the spawn rate.
    float effSpawn  = spawnRate * (1.0 + 0.3 * bass * audio);
    float lifetime  = float(bubbles) / max(effSpawn, 0.05);
    // Each bubble's "global age" in seconds.
    // Bubble k spawned at time T - (k / effSpawn) (approx).

    // Chunk the message: each bubble holds chars [k*chunk, (k+1)*chunk).
    int chunkLen = (total + bubbles - 1) / bubbles;
    if (chunkLen < 1) chunkLen = 1;

    // ─── Background ────────────────────────────────────────
    vec3 col = bgColor.rgb;
    // Mild radial vignette so bubbles read well.
    vec2 vignP = vec2((uv.x - 0.5) * aspect, uv.y - 0.5);
    float vd = length(vignP);
    col *= mix(1.0, 0.7, smoothstep(0.45, 0.95, vd));

    float bubbleAlpha = 0.0;          // accumulated bubble fill mask
    vec3  bubbleCol   = vec3(0.0);    // accumulated bubble color
    float charMask    = 0.0;          // accumulated text mask
    vec3  charCol     = vec3(0.0);    // accumulated text color

    for (int k = 0; k < MAX_BUBBLES; k++) {
        if (k >= bubbles) break;
        float fk = float(k);

        // Phase in [0,1): 0 = just spawned, 1 = expired.
        float age = mod(TIME * effSpawn + fk * 0.27, 1.0) * lifetime * effSpawn;
        float phase = age / lifetime;
        if (phase >= 1.0) continue;

        // Pop-in: short scale-up at the start.
        float popIn  = smoothstep(0.0, 0.06, phase);
        // Fade-out near end.
        float fadeOut = 1.0 - smoothstep(0.85, 1.0, phase);
        float env = popIn * fadeOut;
        if (env < 0.01) continue;

        // Vertical position: rises from bottom (y=0.18) to top (y=0.92).
        float by = mix(0.18, 0.92, phase);
        // Horizontal: sender on right, receiver on left, alternating per bubble.
        bool senderSide = (mod(fk, 2.0) < 0.5);
        float seed = hash11(fk * 11.31);
        float wob  = wobble * 0.06 * (seed - 0.5) * 2.0
                   + wobble * 0.04 * sin(TIME * 0.7 + fk * 1.7);
        float bx;
        if (senderSide) bx =  aspect * 0.25 + wob;
        else            bx = -aspect * 0.25 + wob;

        vec2 bubbleC = vec2(bx, by);
        vec2 d = p - bubbleC;

        // Bubble box size — fits chunkLen chars across with padding.
        float boxW = float(chunkLen) * charW * 1.15 + bubblePadding * 2.0;
        float boxH = charH + bubblePadding * 2.0;
        vec2  halfBox = vec2(boxW, boxH) * 0.5;

        // Pop-in scales the bubble.
        float scale = mix(0.6, 1.0, popIn);
        d /= scale;

        // Tail: sender → tail bottom-right; receiver → tail bottom-left.
        float tailX = senderSide ?  halfBox.x * 0.72 : -halfBox.x * 0.72;
        float tailY = -halfBox.y * 1.05;

        float sdf = sdRoundedRectTail(d, halfBox, cornerRadius, tailX, tailY);

        // Anti-aliased bubble fill.
        float fw   = fwidth(sdf);
        float fill = 1.0 - smoothstep(-fw, fw, sdf);
        if (fill < 0.001) continue;

        // Per-bubble color from the 6-slot palette. Cycle by index so
        // consecutive bubbles never share a color.
        int paletteIdx = int(mod(fk, 6.0));
        vec3 bubColor = color1.rgb;
        if (paletteIdx == 1) bubColor = color2.rgb;
        else if (paletteIdx == 2) bubColor = color3.rgb;
        else if (paletteIdx == 3) bubColor = color4.rgb;
        else if (paletteIdx == 4) bubColor = color5.rgb;
        else if (paletteIdx == 5) bubColor = color6.rgb;

        // Vertical sheen for that glossy iMessage feel.
        float sheen = smoothstep(-halfBox.y, halfBox.y * 0.7, d.y);
        bubColor = mix(bubColor * 0.92, bubColor * 1.12, sheen);
        // Bass pulse on the freshest bubble (just spawned).
        if (phase < 0.2) bubColor *= 1.0 + 0.15 * bass * audio * (1.0 - phase / 0.2);

        // Composite — newest bubble wins overlap.
        bubbleAlpha = max(bubbleAlpha, fill * env);
        bubbleCol   = mix(bubbleCol, bubColor, fill * env);

        // ─── Text inside bubble ───
        // Bubble-local coords with text origin at top-left of inner area.
        float innerL = -halfBox.x + bubblePadding;
        float innerB = -halfBox.y + bubblePadding;
        // Char advance — `kerning` uniform controls letter spacing
        // multiplier (1.0 = touching, <1.0 = overlapping, >1.0 = airy).
        float kern = charW * kerning;

        // Position relative to inner top-left of text row.
        float lx = (d.x - innerL);
        float ly = (d.y - innerB);
        if (lx < 0.0 || ly < 0.0 || ly > charH) continue;

        // Char index inside bubble.
        int colIdx = int(floor(lx / kern));
        if (colIdx < 0 || colIdx >= chunkLen) continue;
        int globalIdx = k * chunkLen + colIdx;
        if (globalIdx >= total) continue;

        int ch = getChar(globalIdx);
        vec2 cellLocal = vec2((lx - float(colIdx) * kern) / charW,
                              ly / charH);
        float s = sampleChar(ch, cellLocal);
        s = smoothstep(0.18, 0.55, s);
        if (s > 0.001) {
            // Auto contrast: pick black/white based on bubble luminance,
            // so any "crazy" palette entry stays readable. Manual override
            // with autoTextColor=false uses manualTextColor instead.
            vec3 txtColor;
            if (autoTextColor) {
                float lum = dot(bubColor, vec3(0.299, 0.587, 0.114));
                txtColor = (lum > 0.55) ? vec3(0.04) : vec3(1.0);
            } else {
                txtColor = manualTextColor.rgb;
            }
            charMask = max(charMask, s * env * fill);
            charCol  = mix(charCol, txtColor, s * env * fill);
        }
    }

    // Compose background ← bubble ← text.
    col = mix(col, bubbleCol, bubbleAlpha);
    col = mix(col, charCol,   charMask);

    // Subtle drop shadow for newest bubble approximation: faint dark
    // ring just below where any bubble edge sits. Cheap & cheerful.

    float alpha = 1.0;
    if (transparentBg) {
        alpha = max(bubbleAlpha, charMask);
        col   = mix(bubbleCol, charCol, charMask);
    }

    gl_FragColor = vec4(col, alpha);
}
