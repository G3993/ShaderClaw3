/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Cascade - tiled rows with wave offsets on a deep-sea bioluminescent background",
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
    { "NAME": "textColor", "LABEL": "Text Color", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.02, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "bioScale", "LABEL": "Bio Scale", "TYPE": "float", "MIN": 0.5, "MAX": 8.0, "DEFAULT": 2.5 },
    { "NAME": "bioSpeed", "LABEL": "Bio Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.25 },
    { "NAME": "hdrPeak", "LABEL": "HDR Peak", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 2.5 },
    { "NAME": "audioMod", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
  ]
}*/

const float PI = 3.14159265;
const float TWO_PI = 6.28318530;

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

float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21b(vec2 p) {
    p = fract(p * vec2(234.34, 435.346));
    p += dot(p, p + 34.23);
    return fract(p.x * p.y);
}

// =======================================================================
// DEEP-SEA BIOLUMINESCENT BACKGROUND
// =======================================================================

float smoothNoiseBio(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash21b(i);
    float b = hash21b(i + vec2(1.0, 0.0));
    float c = hash21b(i + vec2(0.0, 1.0));
    float d = hash21b(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbmBio(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * smoothNoiseBio(p);
        p = p * 2.1 + vec2(5.3, 2.71);
        a *= 0.5;
    }
    return v;
}

vec3 bioBg(vec2 uv) {
    float t = TIME * bioSpeed;
    vec2 q = vec2(fbmBio(uv * bioScale + t * 0.09),
                  fbmBio(uv * bioScale + vec2(3.1, 7.4) + t * 0.07));
    float f = fbmBio(uv * bioScale + q * 1.8 + vec2(t * 0.06, -t * 0.04));

    float audioBoost = 1.0 + audioLevel * audioMod * 0.6;
    f = clamp(f * audioBoost, 0.0, 1.0);

    float h = hdrPeak;
    // Deep sea bioluminescent palette: near-black → deep teal → electric cyan → lime → white-hot
    vec3 col;
    if (f < 0.30)      col = mix(vec3(0.0, 0.005, 0.04),       vec3(0.0, 0.20, 0.28),              f / 0.30);
    else if (f < 0.55) col = mix(vec3(0.0, 0.20, 0.28),        vec3(0.0, h * 0.38, h * 0.34),      (f - 0.30) / 0.25);
    else if (f < 0.75) col = mix(vec3(0.0, h * 0.38, h * 0.34), vec3(h * 0.12, h, h * 0.18),      (f - 0.55) / 0.20);
    else               col = mix(vec3(h * 0.12, h, h * 0.18),   vec3(h * 0.8, h, h * 0.9),         (f - 0.75) / 0.25);

    return col;
}

// =======================================================================
// EFFECT: CASCADE — returns textHit in alpha
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

    return vec4(textColor.rgb, textHit);
}

// =======================================================================
// MAIN
// =======================================================================

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 textLayer = effectCascade(uv);

    if (transparentBg) {
        gl_FragColor = vec4(textLayer.rgb, textLayer.a);

        if (_voiceGlitch > 0.01) {
            float g = _voiceGlitch;
            float t = TIME * 17.0;
            float band = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
            float bandNoise = fract(sin(band * 91.7 + t) * 43758.5);
            float bandActive = step(1.0 - g * 0.6, bandNoise);
            float shift = (bandNoise - 0.5) * 0.08 * g * bandActive;
            float chromaAmt = g * 0.015;
            vec4 cR = effectCascade(uv + vec2(shift + chromaAmt, 0.0));
            vec4 cG = effectCascade(uv + vec2(shift, chromaAmt * 0.5));
            vec4 cB = effectCascade(uv + vec2(shift - chromaAmt, 0.0));
            gl_FragColor = vec4(cR.r, cG.g, cB.b, max(max(cR.a, cG.a), cB.a));
        }
        return;
    }

    // Bioluminescent composite: dark ink text over living deep-sea background
    vec3 bio = bioBg(uv);
    vec3 finalCol = mix(bio, textColor.rgb, textLayer.a);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt = g * 0.015;
        vec3 bR = bioBg(uv + vec2(shift + chromaAmt, 0.0));
        vec3 bG = bioBg(uv + vec2(shift, chromaAmt * 0.5));
        vec3 bB = bioBg(uv + vec2(shift - chromaAmt, 0.0));
        vec4 tR = effectCascade(uv + vec2(shift + chromaAmt, 0.0));
        vec4 tG = effectCascade(uv + vec2(shift, chromaAmt * 0.5));
        vec4 tB = effectCascade(uv + vec2(shift - chromaAmt, 0.0));
        float scanline = 0.95 + 0.05 * sin(uv.y * RENDERSIZE.y * 1.5 + t * 40.0);
        finalCol = vec3(
            mix(bR.r, textColor.r, tR.a),
            mix(bG.g, textColor.g, tG.a),
            mix(bB.b, textColor.b, tB.a)
        ) * scanline;
    }

    gl_FragColor = vec4(finalCol, 1.0);
}
