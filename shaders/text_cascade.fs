/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Cascade - tiled rows with wave offsets",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.5 },
    { "NAME": "intensity", "LABEL": "Wave Height", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "density", "LABEL": "Row Count", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "textScale", "LABEL": "Size", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "oscSpeed", "LABEL": "Osc Speed", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 0.0 },
    { "NAME": "oscAmount", "LABEL": "Osc Amount", "TYPE": "float", "MIN": 0.0, "MAX": 0.2, "DEFAULT": 0.0 },
    { "NAME": "oscSpread", "LABEL": "Osc Spread", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5 },
    { "NAME": "textColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "hdrGlow", "LABEL": "HDR Glow", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 2.0 },
    { "NAME": "audioMod", "LABEL": "Audio Mod", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
  ]
}*/

const float PI = 3.14159265;
const float TWO_PI = 6.28318530;

// Atlas-only font engine (no bitmap fallback — faster ANGLE compile)
float charPixel(int ch, float col, float row) {
    if (ch < 0 || ch > 36) return 0.0;
    vec2 uv = vec2(col / 5.0, row / 7.0);
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    return smoothstep(0.1, 0.55, texture2D(fontAtlasTex, vec2((float(ch) + uv.x) / 37.0, uv.y)).r);
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

float sampleChar(int ch, vec2 uv) {
    if (ch < 0 || ch > 36) return 0.0;
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    return texture2D(fontAtlasTex, vec2((float(ch) + uv.x) / 37.0, uv.y)).r;
}

float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// =======================================================================
// BACKGROUND: CYBERPUNK RAIN
// Monochromatic electric-blue rain streaks over deep midnight cityscape.
// Completely different from aurora (no colorful bands) — vertical blue rain only.
// =======================================================================

vec3 generateBackground(vec2 uv) {
    // --- Sky gradient: deep midnight navy at top, near-black at mid ---
    vec3 skyTop    = vec3(0.02, 0.02, 0.08);   // deep midnight navy
    vec3 skyBottom = vec3(0.0,  0.0,  0.02);   // near void black at horizon
    vec3 sky = mix(skyBottom, skyTop, uv.y);

    // --- Rain streaks: many vertical lines moving downward ---
    // Each "column" of rain is defined by its x-lane index.
    float rainCol = 0.0;
    // Use several bands of rain at different densities/speeds
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        float laneCount = mix(18.0, 60.0, fi / 2.0);
        float fallSpeed = mix(0.55, 1.3, fi / 2.0);

        float lane = floor(uv.x * laneCount);
        float laneRand = hash(lane + fi * 73.1);
        // Offset each lane's x slightly for sub-lane jitter
        float laneX = (lane + 0.5 + (laneRand - 0.5) * 0.4) / laneCount;

        // Streak: thin vertical smear moving downward
        float streakY = fract(uv.y + TIME * fallSpeed * (0.7 + laneRand * 0.6) + laneRand * 4.7);
        // Streak length: short bright head, longer dim tail
        float head   = smoothstep(0.0, 0.01, streakY) * smoothstep(0.06, 0.0, streakY);
        float tail   = smoothstep(0.0, 0.005, streakY) * smoothstep(0.22, 0.0, streakY) * 0.25;
        float streak = head + tail;

        // Horizontal width of each rain streak (thin)
        float dx = abs(uv.x - laneX) * laneCount;
        float xMask = smoothstep(0.35, 0.0, dx);

        // Only active lanes (not every lane has rain at all times)
        float activeThresh = 0.55;
        float active = step(activeThresh, laneRand);

        rainCol += streak * xMask * active;
    }

    // Electric blue rain color — HDR peak
    vec3 rainColor = vec3(0.1, 0.4, 2.5);
    vec3 rain = rainColor * clamp(rainCol, 0.0, 1.0);

    // --- Neon city glow at bottom: puddle reflections ---
    // Distorted horizontal bands simulating wet pavement reflections
    float puddleY = smoothstep(0.28, 0.0, uv.y);   // only near bottom
    float puddleDistort = sin(uv.x * 14.0 + TIME * 1.2) * 0.012
                        + sin(uv.x * 31.0 - TIME * 0.7) * 0.006;
    float puddleUvY = uv.y + puddleDistort;
    float puddle = puddleY * max(0.0, 1.0 - puddleUvY * 8.0);

    // Warm orange neon reflection in puddle
    vec3 puddleGlow = vec3(2.2, 0.6, 0.05) * puddle * 0.6;

    // --- Neon sign flicker bands: horizontal warm bands near bottom ---
    // 3 sign bands at fixed y-positions
    float signGlow = 0.0;
    float sign1Y = abs(uv.y - 0.12);
    float sign2Y = abs(uv.y - 0.20);
    float sign3Y = abs(uv.y - 0.07);

    float flicker1 = step(0.5, hash(floor(TIME * 13.0) + 1.0));
    float flicker2 = step(0.4, hash(floor(TIME * 13.0) + 2.0));
    float flicker3 = step(0.6, hash(floor(TIME * 13.0) + 3.0));

    signGlow += smoothstep(0.025, 0.0, sign1Y) * flicker1;
    signGlow += smoothstep(0.018, 0.0, sign2Y) * flicker2 * 0.7;
    signGlow += smoothstep(0.012, 0.0, sign3Y) * flicker3 * 0.5;

    // Neon orange/red sign color — warm, HDR
    vec3 signColor = vec3(2.5, 0.35, 0.05);
    vec3 signs = signColor * signGlow * 0.5;

    // --- Composite ---
    vec3 finalBg = sky + rain + puddleGlow + signs;
    return finalBg;
}

// =======================================================================
// EFFECT: CASCADE - tiled rows with wave offsets
// =======================================================================

vec4 effectCascade(vec2 uv) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    int numChars = charCount();
    float waveAmount = intensity;
    float rows = floor(mix(5.0, 30.0, density));

    float warpedY = uv.y + sin(uv.y * TWO_PI * 1.5 + TIME * speed * 1.5) * waveAmount * 0.06;
    float rowH = 1.0 / rows;
    float rowIdx = clamp(floor(warpedY / rowH), 0.0, rows - 1.0);
    float localY = fract(warpedY / rowH);

    float cH = rowH;
    float cW = cH * (5.0/7.0) * (1.0/aspect) * textScale;
    float gW = cW * 0.15;
    float wordW = float(numChars) * (cW + gW);

    float xOff = sin(rowIdx*0.6 + TIME*speed*2.0) * waveAmount * wordW * 1.5 + TIME*speed*0.08;
    float px = mod(uv.x + xOff - 0.5 + wordW * 0.5, wordW);
    if (px < 0.0) px += wordW;

    float cs = cW + gW;
    float csF = px / cs;
    int slot = int(floor(csF));
    float clx = fract(csF);
    float cf = cW / cs;

    float textHit = 0.0;
    if (clx < cf && slot >= 0 && slot < numChars) {
        float gc = (clx/cf) * 5.0, gr = localY * 7.0;
        if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0) {
            int ch = getChar(slot);
            if (ch >= 0 && ch <= 36 && ch != 26) textHit = charPixel(ch, gc, gr);
        }
    }

    bool inv = mod(rowIdx, 2.0) < 1.0;
    vec3 fg = inv ? bgColor.rgb : textColor.rgb;
    vec3 bg = inv ? textColor.rgb : bgColor.rgb;
    vec3 fc = mix(bg, fg, textHit);
    float a = 1.0;
    if (transparentBg) { a = textHit; fc = textColor.rgb; }
    return vec4(fc, a);
}

// =======================================================================
// MAIN
// =======================================================================

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    if (!transparentBg) {
        // Compute textHit directly for HDR composite
        float aspect = RENDERSIZE.x / RENDERSIZE.y;
        int numChars = charCount();
        float waveAmount = intensity;
        float rows = floor(mix(5.0, 30.0, density));

        float warpedY = uv.y + sin(uv.y * TWO_PI * 1.5 + TIME * speed * 1.5) * waveAmount * 0.06;
        float rowH = 1.0 / rows;
        float rowIdx = clamp(floor(warpedY / rowH), 0.0, rows - 1.0);
        float localY = fract(warpedY / rowH);

        float cH = rowH;
        float cW = cH * (5.0/7.0) * (1.0/aspect) * textScale;
        float gW = cW * 0.15;
        float wordW = float(numChars) * (cW + gW);

        float xOff = sin(rowIdx*0.6 + TIME*speed*2.0) * waveAmount * wordW * 1.5 + TIME*speed*0.08;
        float px = mod(uv.x + xOff - 0.5 + wordW * 0.5, wordW);
        if (px < 0.0) px += wordW;

        float cs = cW + gW;
        float csF = px / cs;
        int slot = int(floor(csF));
        float clx = fract(csF);
        float cf = cW / cs;

        float textHit = 0.0;
        if (clx < cf && slot >= 0 && slot < numChars) {
            float gc = (clx/cf) * 5.0, gr = localY * 7.0;
            if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0) {
                int ch = getChar(slot);
                if (ch >= 0 && ch <= 36 && ch != 26) textHit = charPixel(ch, gc, gr);
            }
        }

        vec3 bg = generateBackground(uv);
        vec3 finalColor = bg + textHit * textColor.rgb * hdrGlow * audioMod;
        gl_FragColor = vec4(finalColor, 1.0);
        return;
    }

    vec4 col = effectCascade(uv);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt = g * 0.015;
        vec2 uvR = uv + vec2(shift + chromaAmt, 0.0);
        vec2 uvB = uv + vec2(shift - chromaAmt, 0.0);
        vec2 uvG = uv + vec2(shift, chromaAmt * 0.5);
        vec4 cR = effectCascade(uvR);
        vec4 cG = effectCascade(uvG);
        vec4 cB = effectCascade(uvB);
        vec4 glitched = vec4(cR.r, cG.g, cB.b, max(max(cR.a, cG.a), cB.a));
        float scanline = 0.95 + 0.05 * sin(uv.y * RENDERSIZE.y * 1.5 + t * 40.0);
        float blockX = floor(uv.x * 6.0);
        float blockY = floor(uv.y * 4.0);
        float blockNoise = fract(sin((blockX + blockY * 7.0) * 113.1 + floor(t * 8.0)) * 43758.5);
        float dropout = step(1.0 - g * 0.15, blockNoise);
        glitched.rgb *= scanline;
        glitched.rgb *= 1.0 - dropout;
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = col;
}
