/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Cascade - tiled rows with wave offsets, ember storm background. Rising glowing embers on void black — each row alternates text color between hot orange and deep crimson. HDR ember cores 2.0+, text 2.5+ glow.",
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
    { "NAME": "textColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [1.0, 0.55, 0.0, 1.0] },
    { "NAME": "hdrGlow", "LABEL": "HDR Glow", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 2.5 },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
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
float hash2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// ── Ember Storm Background ───────────────────────────────────────────────
// Rising embers: small bright sparks drifting upward on void black.
// Palette: deep crimson → orange → gold → white-hot HDR core.
vec3 emberBg(vec2 uv, float t) {
    vec3 col = vec3(0.004, 0.001, 0.0); // near-void black with warm tint

    // 40 ember particles distributed across UV space, rising upward
    for (int i = 0; i < 40; i++) {
        float fi   = float(i);
        float seed = hash(fi * 7.31 + 1.0);
        float seed2= hash(fi * 3.17 + 2.0);
        float seed3= hash(fi * 5.43 + 0.5);

        // Ember horizontal start position
        float ex = seed;
        // Vertical drift: rising with time, looping
        float riseSpeed = 0.04 + seed2 * 0.06;
        float ey = fract(1.0 - (t * riseSpeed + seed3));

        // Small horizontal sway
        float swayAmp  = 0.015 + seed * 0.025;
        float swayFreq = 2.0 + seed2 * 4.0;
        ex = fract(ex + swayAmp * sin(t * swayFreq + fi * 2.3));

        // Distance from current UV to ember center
        vec2 delta = uv - vec2(ex, ey);
        float d = length(delta);

        // Ember size varies: 0.003–0.012 in UV space
        float r = 0.003 + seed * 0.009;
        if (d < r * 6.0) {
            float falloff = exp(-d * d / (r * r * 0.5));
            // Color: white-hot core → gold → orange → crimson at edges
            float heat = falloff;
            vec3 coreCol  = vec3(2.0, 1.4, 0.8);  // white-hot HDR
            vec3 midCol   = vec3(1.8, 0.5, 0.0);  // orange
            vec3 outerCol = vec3(0.8, 0.05, 0.0); // deep crimson
            vec3 ec = heat > 0.7 ? mix(midCol,  coreCol,  (heat - 0.7) / 0.3) :
                      heat > 0.3 ? mix(outerCol, midCol,  (heat - 0.3) / 0.4) :
                                   outerCol * (heat / 0.3);
            col += ec * falloff;
        }
    }
    return col;
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

    // Ember storm bg per row position
    vec3 rowBg = transparentBg ? bgColor.rgb : emberBg(uv, TIME);

    bool inv = mod(rowIdx, 2.0) < 1.0;
    // Alternating rows: orange text vs crimson text, both on ember bg
    vec3 altTextCol = inv ? textColor.rgb * hdrGlow
                          : vec3(1.0, 0.15, 0.0) * hdrGlow;
    vec3 fg = inv ? rowBg : altTextCol;
    vec3 bg = inv ? altTextCol : rowBg;
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
