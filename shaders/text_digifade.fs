/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Stellar Nebula Fade — digifade sweep dissolving text into cosmic nebula",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed",      "LABEL": "Speed",       "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.5 },
    { "NAME": "intensity",  "LABEL": "Dissolve",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "density",    "LABEL": "Slices",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "textScale",  "LABEL": "Size",        "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "nebulaScale","LABEL": "Nebula Scale","TYPE": "float", "MIN": 0.5, "MAX": 5.0, "DEFAULT": 2.0 },
    { "NAME": "hdrText",    "LABEL": "Text HDR",    "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.8 },
    { "NAME": "hdrNebula",  "LABEL": "Nebula HDR",  "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 2.2 },
    { "NAME": "pulse",      "LABEL": "Audio Pulse", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7 }
  ]
}*/

const float PI = 3.14159265;

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

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(hash21(i),              hash21(i + vec2(1.0, 0.0)), f.x),
        mix(hash21(i + vec2(0.0, 1.0)), hash21(i + vec2(1.0, 1.0)), f.x),
        f.y
    );
}

float fbm4(vec2 p) {
    float v = 0.0, amp = 0.5;
    for (int i = 0; i < 4; i++) {
        v += amp * vnoise(p);
        p = p * 2.1 + vec2(1.7, 9.2);
        amp *= 0.5;
    }
    return v;
}

// =======================================================================
// NEBULA BACKGROUND — two FBM gas clouds + multi-scale star field
// Palette: void navy / blue-violet gas / hot cyan veins / amber dust / white stars
// =======================================================================

vec3 nebulaBg(vec2 uv, float audio) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = vec2(uv.x * aspect, uv.y) * nebulaScale;
    float tDrift = TIME * 0.04;

    // Gas cloud 1 — blue-violet primary
    float gas1 = fbm4(p + vec2(tDrift, 0.0));
    // Gas cloud 2 — cyan secondary (different scale + offset)
    float gas2 = fbm4(p * 1.6 + vec2(0.0, tDrift * 0.8) + 4.7);

    vec3 VOID   = vec3(0.0,  0.01, 0.06);
    vec3 BLUE   = vec3(0.1,  0.25, 0.9);
    vec3 VIOLET = vec3(0.5,  0.0,  1.0);
    vec3 CYAN   = vec3(0.0,  0.85, 1.0);
    vec3 AMBER  = vec3(1.0,  0.65, 0.0);

    float b = hdrNebula * audio;
    vec3 col = VOID;
    col = mix(col, BLUE   * b,        smoothstep(0.25, 0.55, gas1));
    col = mix(col, VIOLET * b * 1.1,  smoothstep(0.50, 0.75, gas1));
    col = mix(col, CYAN   * b * 0.9,  smoothstep(0.35, 0.65, gas2) * 0.7);
    col = mix(col, AMBER  * b * 1.3,  smoothstep(0.65, 0.82, gas2) * 0.4);

    // Star field — 3 density layers
    for (int layer = 0; layer < 3; layer++) {
        float scale = 40.0 + float(layer) * 35.0;
        float seed  = float(layer) * 17.3;
        vec2 sCell  = floor(p * scale);
        float sh    = hash21(sCell + seed);
        float sh2   = hash21(sCell * 7.31 + seed);
        if (sh > 0.04) continue; // only 4% density per layer
        vec2 pos    = fract(p * scale) - vec2(hash21(sCell + 0.7), hash21(sCell + 1.3));
        float sr    = length(pos);
        float sSize = 0.04 + sh2 * 0.08;
        float star  = (1.0 - smoothstep(0.0, sSize, sr)) * (0.5 + sh2 * 0.5);
        col += vec3(0.85, 0.92, 1.0) * star * (2.5 + audio * 0.5);
    }

    return col;
}

// =======================================================================
// EFFECT: STELLAR NEBULA FADE — digifade sweep dissolves text into nebula
// =======================================================================

vec4 effectNebula(vec2 uv) {
    float aspect  = RENDERSIZE.x / RENDERSIZE.y;
    int numChars  = charCount();
    float sliceCount = mix(5.0, 100.0, density);
    float t = TIME * speed;

    vec2 p = vec2((uv.x - 0.5) * aspect + 0.5, uv.y);

    // Single-line layout
    float cH = 0.18 * textScale;
    if (aspect < 1.0) cH *= aspect;
    float cW = cH * (5.0 / 7.0);
    float gW = cW * 0.2;

    float totalTextW = float(numChars) * cW + float(numChars - 1) * gW;
    float maxW = 0.9 * aspect;
    float fitScale = totalTextW > maxW ? maxW / totalTextW : 1.0;
    cH *= fitScale; cW *= fitScale; gW *= fitScale;

    float rowW  = float(numChars) * cW + float(numChars - 1) * gW;
    float startX = 0.5 - rowW * 0.5;
    float startY = 0.5 - cH * 0.5;

    float si = floor(uv.y * sliceCount);
    float n1 = hash11(si + floor(t * 2.0));
    float n2 = hash11(si * 3.7 + floor(t * 3.0));

    // Sweep dissolve: sweeps left-to-right, displacing text into nebula static
    float sw = sin(t * 0.7) * 0.5 + 0.5;
    float ps = smoothstep(sw - 0.15, sw + 0.1, (p.x - startX) / max(rowW, 0.001));

    float dx = abs(ps * n1 * intensity * 0.3 + ps * sin(si * 0.3 + t) * intensity * 0.09);
    float dy = ps * (n2 - 0.5) * intensity * 0.04;

    vec2 samp = vec2(p.x - dx, p.y - dy);
    float rx  = samp.x - startX;
    float ry  = samp.y - startY;

    float textHit = 0.0;
    if (rx >= 0.0 && rx <= rowW && ry >= 0.0 && ry <= cH) {
        float cs   = cW + gW;
        float csF  = rx / cs;
        int slot   = int(floor(csF));
        float clx  = fract(csF), cf = cW / cs;
        if (clx < cf && slot >= 0 && slot < numChars) {
            float gc = (clx / cf) * 5.0, gr = (ry / cH) * 7.0;
            if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0) {
                int ch = getChar(slot);
                if (ch >= 0 && ch <= 36 && ch != 26)
                    textHit = max(textHit, charPixel(ch, gc, gr));
            }
        }
    }

    float audio  = 1.0 + audioBass * pulse;
    vec3 bg      = nebulaBg(uv, audio);
    // Warm star-white text — solid before sweep front, dissolving after
    vec3 textCol = vec3(1.0, 0.92, 0.65) * hdrText * audio;

    vec3 col = mix(bg, textCol, textHit);
    return vec4(col, 1.0);
}

void main() {
    vec2 uv  = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 col = effectNebula(uv);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band       = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise  = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift      = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt  = g * 0.015;
        vec4 cR = effectNebula(uv + vec2(shift + chromaAmt, 0.0));
        vec4 cG = effectNebula(uv + vec2(shift, chromaAmt * 0.5));
        vec4 cB = effectNebula(uv + vec2(shift - chromaAmt, 0.0));
        vec4 glitched = vec4(cR.r, cG.g, cB.b, 1.0);
        float scanline  = 0.95 + 0.05 * sin(uv.y * RENDERSIZE.y * 1.5 + t * 40.0);
        float blockX    = floor(uv.x * 6.0);
        float blockY    = floor(uv.y * 4.0);
        float blockNoise = fract(sin((blockX + blockY * 7.0) * 113.1 + floor(t * 8.0)) * 43758.5);
        float dropout   = step(1.0 - g * 0.15, blockNoise);
        glitched.rgb   *= scanline * (1.0 - dropout);
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = col;
}
