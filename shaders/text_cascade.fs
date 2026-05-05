/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Magma Cascade — cascading text rows flowing over a FBM lava-flow background",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed",     "LABEL": "Speed",        "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.5 },
    { "NAME": "intensity", "LABEL": "Wave Height",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "density",   "LABEL": "Row Count",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "textScale", "LABEL": "Size",         "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "lavaScale", "LABEL": "Lava Scale",   "TYPE": "float", "MIN": 0.5, "MAX": 6.0, "DEFAULT": 3.0 },
    { "NAME": "hdrText",   "LABEL": "Text HDR",     "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 3.0 },
    { "NAME": "hdrLava",   "LABEL": "Lava HDR",     "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 2.5 },
    { "NAME": "pulse",     "LABEL": "Audio Pulse",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7 }
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
        mix(hash21(i + vec2(0.0,1.0)), hash21(i + vec2(1.0,1.0)), f.x),
        f.y
    );
}

// =======================================================================
// LAVA BACKGROUND — FBM heat ramp flowing downward
// Palette: rock black → deep crimson → orange → gold → white-hot
// =======================================================================

vec3 magmaBg(vec2 uv, float audio) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = vec2(uv.x * aspect, uv.y) * lavaScale;
    float tFlow = TIME * speed * 0.35;

    // Downward flow + horizontal turbulence
    p.y -= tFlow;
    p.x += sin(p.y * 1.4 + tFlow * 0.6) * 0.25;

    float v = 0.0, amp = 0.5;
    for (int i = 0; i < 5; i++) {
        v += amp * vnoise(p);
        p = p * 2.1 + vec2(1.7, 9.2);
        amp *= 0.48;
    }
    v = clamp(v * 2.1 - 0.35, 0.0, 1.0);

    // Heat ramp — fully saturated palette, no white-mixing until peaks
    vec3 ROCK    = vec3(0.04, 0.01, 0.0);
    vec3 CRIMSON = vec3(0.65, 0.0,  0.0);
    vec3 ORANGE  = vec3(1.0,  0.35, 0.0);
    vec3 GOLD    = vec3(1.0,  0.80, 0.0);
    vec3 WHITE   = vec3(1.0,  0.95, 0.80);

    float b = hdrLava * audio;
    vec3 col = ROCK;
    col = mix(col, CRIMSON * b,        smoothstep(0.08, 0.28, v));
    col = mix(col, ORANGE  * b * 1.1,  smoothstep(0.28, 0.58, v));
    col = mix(col, GOLD    * b * 1.2,  smoothstep(0.58, 0.78, v));
    col = mix(col, WHITE   * b * 1.5,  smoothstep(0.78, 1.0,  v));

    return col;
}

// =======================================================================
// EFFECT: MAGMA CASCADE — text rows riding lava waves
// =======================================================================

vec4 effectMagma(vec2 uv) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    int numChars = charCount();
    float rows = floor(mix(5.0, 30.0, density));
    float t = TIME * speed;

    // Y warp — lava-like vertical undulation
    float warpedY = uv.y + sin(uv.y * PI * 3.0 + t * 1.8) * intensity * 0.06;
    float rowH    = 1.0 / rows;
    float rowIdx  = clamp(floor(warpedY / rowH), 0.0, rows - 1.0);
    float localY  = fract(warpedY / rowH);

    float cH  = rowH;
    float cW  = cH * (5.0 / 7.0) * (1.0 / aspect) * textScale;
    float gW  = cW * 0.15;
    float wordW = float(numChars) * (cW + gW);

    // Cascade: each row flows at a lava-crawl pace, waves vary per row
    float xOff = sin(rowIdx * 0.65 + t * 1.6) * intensity * wordW * 1.4
               + t * 0.06;
    float px = mod(uv.x + xOff - 0.5 + wordW * 0.5, wordW);
    if (px < 0.0) px += wordW;

    float cs  = cW + gW;
    float csF = px / cs;
    int slot  = int(floor(csF));
    float clx = fract(csF);
    float cf  = cW / cs;

    float textHit = 0.0;
    if (clx < cf && slot >= 0 && slot < numChars) {
        float gc = (clx / cf) * 5.0;
        float gr = localY * 7.0;
        if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0) {
            int ch = getChar(slot);
            if (ch >= 0 && ch <= 36 && ch != 26)
                textHit = charPixel(ch, gc, gr);
        }
    }

    float audio = 1.0 + audioBass * pulse;
    vec3 bg      = magmaBg(uv, audio);
    // White-hot text — bright enough to read over any lava brightness
    vec3 textCol = vec3(1.0, 0.95, 0.82) * hdrText * audio;

    vec3 col = mix(bg, textCol, textHit);
    return vec4(col, 1.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 col = effectMagma(uv);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band      = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift      = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt  = g * 0.015;
        vec4 cR = effectMagma(uv + vec2(shift + chromaAmt, 0.0));
        vec4 cG = effectMagma(uv + vec2(shift, chromaAmt * 0.5));
        vec4 cB = effectMagma(uv + vec2(shift - chromaAmt, 0.0));
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
