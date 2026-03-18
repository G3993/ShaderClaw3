/*{
  "DESCRIPTION": "La Bloom — a love letter on vintage paper. Ink reveals char by char, bleeds softly, lingers, fades. Aged parchment with grain and worn edges.",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator", "Text"],
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": "MY DEAREST I HAVE LOVED YOU SINCE", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "DEFAULT": 2, "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"] },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 20.0 },
    { "NAME": "fadeTime", "LABEL": "Fade", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "bloom", "LABEL": "Ink Bleed", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "wobble", "LABEL": "Wobble", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "textScale", "LABEL": "Size", "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.01, "MAX": 1.0 },
    { "NAME": "kerning", "LABEL": "Spacing", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "paperGrain", "LABEL": "Paper Grain", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "edgeBurn", "LABEL": "Edge Burn", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "foxing", "LABEL": "Foxing", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "oscSpeed", "LABEL": "Osc Speed", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 0.0 },
    { "NAME": "oscAmount", "LABEL": "Osc Amount", "TYPE": "float", "MIN": 0.0, "MAX": 0.2, "DEFAULT": 0.0 },
    { "NAME": "oscSpread", "LABEL": "Osc Spread", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5 },
    { "NAME": "textColor", "LABEL": "Ink", "TYPE": "color", "DEFAULT": [0.14, 0.07, 0.04, 1.0] },
    { "NAME": "bgColor", "LABEL": "Paper", "TYPE": "color", "DEFAULT": [0.93, 0.88, 0.78, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
  ]
}*/

// ==========================================
// Font atlas sampling
// ==========================================

float sampleChar(int ch, vec2 uv) {
    if (ch < 0 || ch > 36) return 0.0;
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    return texture2D(fontAtlasTex, vec2((float(ch) + uv.x) / 37.0, uv.y)).r;
}

// ==========================================
// Character lookup (msg_0..msg_47)
// ==========================================

int getChar(int slot) {
    if (slot == 0) return int(msg_0);
    if (slot == 1) return int(msg_1);
    if (slot == 2) return int(msg_2);
    if (slot == 3) return int(msg_3);
    if (slot == 4) return int(msg_4);
    if (slot == 5) return int(msg_5);
    if (slot == 6) return int(msg_6);
    if (slot == 7) return int(msg_7);
    if (slot == 8) return int(msg_8);
    if (slot == 9) return int(msg_9);
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
    return 26;
}

int charCount() {
    int n = int(msg_len);
    if (n <= 0) return 48;
    if (n > 48) return 48;
    return n;
}

// ==========================================
// Procedural noise for paper texture
// ==========================================

float hash(float n) {
    return fract(sin(n * 127.1) * 43758.5453);
}

vec2 hash2(vec2 p) {
    return fract(sin(vec2(
        dot(p, vec2(127.1, 311.7)),
        dot(p, vec2(269.5, 183.3))
    )) * 43758.5453);
}

// Value noise
float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f); // smoothstep

    float a = hash(dot(i, vec2(1.0, 157.0)));
    float b = hash(dot(i + vec2(1.0, 0.0), vec2(1.0, 157.0)));
    float c = hash(dot(i + vec2(0.0, 1.0), vec2(1.0, 157.0)));
    float d = hash(dot(i + vec2(1.0, 1.0), vec2(1.0, 157.0)));

    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// Fractal Brownian motion — layered noise for organic texture
float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    vec2 shift = vec2(100.0);
    for (int i = 0; i < 4; i++) {
        v += a * vnoise(p);
        p = p * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}

// ==========================================
// Main
// ==========================================

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Aspect-corrected coordinates centered at 0.5
    vec2 p;
    p.x = (uv.x - 0.5) * aspect + 0.5;
    p.y = uv.y;

    // ======================================
    // PAPER BACKGROUND
    // ======================================

    vec3 paper = bgColor.rgb;

    // Fine grain noise (paper fiber)
    float grain = vnoise(uv * RENDERSIZE * 0.15);
    float fineGrain = vnoise(uv * RENDERSIZE * 0.4);
    grain = mix(grain, fineGrain, 0.3);
    paper += (grain - 0.5) * 0.08 * paperGrain;

    // Larger mottled variation (aged paper discoloration)
    float mottle = fbm(uv * 6.0 + 3.7);
    paper -= mottle * 0.06 * paperGrain;

    // Subtle warm-cool variation across the page
    float warmShift = fbm(uv * 3.0 + 17.0);
    paper.r += warmShift * 0.025 * paperGrain;
    paper.b -= warmShift * 0.02 * paperGrain;

    // Edge burn / vignette — darkened and yellowed edges
    vec2 edgeUV = uv * 2.0 - 1.0;
    float edgeDist = max(abs(edgeUV.x), abs(edgeUV.y));
    float vignette = smoothstep(0.5, 1.1, edgeDist);
    // Burnt edges are darker and slightly more amber
    vec3 burnColor = bgColor.rgb * vec3(0.55, 0.42, 0.28);
    paper = mix(paper, burnColor, vignette * edgeBurn);

    // Corner wear — extra darkening at corners
    float cornerDist = length(edgeUV);
    float cornerBurn = smoothstep(0.9, 1.5, cornerDist);
    paper = mix(paper, burnColor * 0.7, cornerBurn * edgeBurn * 0.5);

    // Foxing — brown age spots scattered on paper
    if (foxing > 0.001) {
        for (int i = 0; i < 8; i++) {
            vec2 spot = hash2(vec2(float(i) * 13.7, float(i) * 7.3));
            spot = spot * 0.7 + 0.15; // keep spots within page
            float d = length(uv - spot);
            float radius = 0.015 + hash(float(i) * 31.1) * 0.025;
            float spotMask = smoothstep(radius, radius * 0.3, d);
            // Spots are brownish-amber
            vec3 spotColor = vec3(0.65, 0.5, 0.3) * (0.7 + hash(float(i) * 51.3) * 0.3);
            paper = mix(paper, spotColor, spotMask * foxing * 0.4);
        }
    }

    // Faint horizontal ruled lines (like stationery)
    float lineSpacing = 0.045;
    float lineY = mod(uv.y + lineSpacing * 0.5, lineSpacing);
    float lineMask = 1.0 - smoothstep(0.0005, 0.001, abs(lineY - lineSpacing * 0.5));
    vec3 lineColor = bgColor.rgb * vec3(0.75, 0.78, 0.85);
    paper = mix(paper, lineColor, lineMask * 0.15 * paperGrain);

    // ======================================
    // TEXT RENDERING
    // ======================================

    int total = charCount();
    float ftotal = float(total);

    // Character cell sizing
    float charH = 0.18 * textScale;
    float charW = charH * (5.0 / 7.0);
    float gap = charW * 0.25 * kerning;
    float cellStep = charW + gap;

    // Multi-line layout: how many chars fit per row
    float maxW = aspect * 0.85;
    int charsPerRow = int(maxW / cellStep);
    if (charsPerRow < 1) charsPerRow = 1;
    int numRows = (total + charsPerRow - 1) / charsPerRow;

    // Total line height
    float lineH = charH * 1.5;
    float blockH = float(numRows) * lineH;

    // Looping time — full cycle = reveal all + linger + dissolve
    float revealDur = ftotal / speed;
    float totalDur = revealDur + fadeTime + 2.5;
    float t = mod(TIME, totalDur);

    // Accumulate sharp + soft (ink bleed) masks
    float textMask = 0.0;
    float bleedMask = 0.0;

    // Vertical centering
    float blockTop = 0.5 + blockH * 0.5;

    for (int i = 0; i < 64; i++) {
        if (i >= total) break;

        int ch = getChar(i);
        if (ch < 0 || ch > 36) continue; // skip spaces

        float fi = float(i);
        int row = i / charsPerRow;
        int col = i - row * charsPerRow;

        // Chars in this row (last row may be partial)
        int rowStart = row * charsPerRow;
        int rowEnd = rowStart + charsPerRow;
        if (rowEnd > total) rowEnd = total;
        int rowLen = rowEnd - rowStart;
        float rowW = float(rowLen) * cellStep - gap;

        // Position: centered per row, top-aligned block
        float ox = 0.5 - rowW * 0.5 + float(col) * cellStep;
        float oy = blockTop - float(row) * lineH - charH;

        // Per-char age
        float age = t - fi / speed;

        // Fade envelope: ink soaks in, lingers, fades
        float fadeIn = smoothstep(0.0, 0.5, age);
        float fadeOut = 1.0 - smoothstep(fadeTime, fadeTime + 2.0, age);
        float env = fadeIn * fadeOut;
        if (env <= 0.001) continue;

        // Wobble: deterministic per-char baseline shift + size variation
        float h = hash(fi * 3.17);
        float baseShift = wobble * charH * 0.15 * (h - 0.5) * 2.0;
        float sizeVar = 1.0 + wobble * 0.1 * (hash(fi * 7.31) - 0.5) * 2.0;
        // Slight rotation feel via horizontal micro-offset
        float hShift = wobble * charW * 0.05 * (hash(fi * 11.13) - 0.5) * 2.0;

        // Ink bleed on reveal (characters spread slightly as ink soaks in)
        float revealSpread = 1.0 + (1.0 - smoothstep(0.0, 0.8, age)) * 0.1 * bloom;

        float finalScale = sizeVar * revealSpread;
        float adjCharH = charH * finalScale;
        float adjCharW = charW * finalScale;

        // Per-character oscillation
        float oscY = oscAmount * sin(TIME * oscSpeed * 6.2832 + fi * oscSpread * 3.14159);

        // Cell UV for sharp sample
        vec2 cellUV;
        cellUV.x = (p.x - ox - hShift) / adjCharW;
        cellUV.y = (p.y - oy - baseShift - oscY) / adjCharH;

        float s = sampleChar(ch, cellUV);
        s = smoothstep(0.1, 0.5, s);
        textMask += s * env;

        // Ink bleed: softer, slightly larger sample (ink soaking into paper fibers)
        if (bloom > 0.001) {
            float bleedScale = 1.25;
            vec2 bleedUV;
            bleedUV.x = (p.x - ox - hShift) / (adjCharW * bleedScale);
            bleedUV.y = (p.y - oy - baseShift - oscY) / (adjCharH * bleedScale);
            bleedUV += (1.0 - 1.0 / bleedScale) * 0.5;

            float bs = sampleChar(ch, bleedUV);
            bs = smoothstep(0.05, 0.35, bs);
            // Modulate bleed with paper grain (ink bleeds more along fibers)
            float bleedGrain = vnoise(gl_FragCoord.xy * 0.08 + fi * 7.0);
            bs *= 0.7 + bleedGrain * 0.3;
            bleedMask += bs * env;
        }
    }

    // ======================================
    // COMPOSITING
    // ======================================

    // Combine sharp text + ink bleed
    float combined = textMask + bleedMask * bloom * 0.4;
    combined = clamp(combined, 0.0, 1.0);

    // Ink color darkens the paper (subtractive, like real ink)
    vec3 inkColor = textColor.rgb;
    vec3 col = mix(paper, inkColor, combined);

    // Subtle ink bleed tint — slightly warmer/browner than base ink
    float bleedAmount = bleedMask * bloom * 0.2;
    vec3 bleedTint = inkColor * vec3(1.1, 0.9, 0.7); // warmer bleed
    col = mix(col, bleedTint, bleedAmount * (1.0 - combined));
    col = clamp(col, 0.0, 1.0);

    float alpha = 1.0;
    if (transparentBg) {
        alpha = combined + bleedAmount;
        alpha = clamp(alpha, 0.0, 1.0);
        col = inkColor;
    }

    gl_FragColor = vec4(col, alpha);
}
