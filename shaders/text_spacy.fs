/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Aurora Tunnel — perspective text rows flying through curtains of aurora borealis",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed",      "LABEL": "Speed",         "TYPE": "float", "MIN": 0.1, "MAX": 3.0,  "DEFAULT": 0.5 },
    { "NAME": "intensity",  "LABEL": "Perspective",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.5 },
    { "NAME": "density",    "LABEL": "Row Count",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.5 },
    { "NAME": "textScale",  "LABEL": "Size",          "TYPE": "float", "MIN": 0.3, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "hdrText",    "LABEL": "Text HDR",      "TYPE": "float", "MIN": 1.0, "MAX": 4.0,  "DEFAULT": 3.0 },
    { "NAME": "hdrAurora",  "LABEL": "Aurora HDR",    "TYPE": "float", "MIN": 0.5, "MAX": 3.0,  "DEFAULT": 2.2 },
    { "NAME": "curtainFreq","LABEL": "Curtain Freq",  "TYPE": "float", "MIN": 1.0, "MAX": 20.0, "DEFAULT": 7.0 },
    { "NAME": "pulse",      "LABEL": "Audio Pulse",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.7 }
  ]
}*/

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

// =======================================================================
// AURORA BACKGROUND — sinusoidal curtain bands cycling green/teal/violet/pink
// Void-black gaps between curtains give strong ink contrast.
// =======================================================================

vec3 auroraHue(float phase) {
    // Aurora palette: green → teal → violet → pink (stays in aurora range)
    vec3 GREEN  = vec3(0.0,  1.0,  0.25);
    vec3 TEAL   = vec3(0.0,  0.88, 1.0);
    vec3 VIOLET = vec3(0.45, 0.0,  1.0);
    vec3 PINK   = vec3(1.0,  0.08, 0.6);
    float h = fract(phase);
    vec3 c = GREEN;
    c = mix(c, TEAL,   smoothstep(0.0,  0.33, h));
    c = mix(c, VIOLET, smoothstep(0.33, 0.67, h));
    c = mix(c, PINK,   smoothstep(0.67, 1.0,  h));
    return c;
}

vec3 auroraBg(vec2 uv, float audio) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float x = uv.x * aspect;
    float y = uv.y;
    float t = TIME * 0.12;

    // Curtain undulation: x-position warps sinusoidally with y
    float warp = sin(y * 4.7 + t * 0.9) * 0.18 + sin(y * 11.3 - t * 0.55) * 0.08;
    float cx = x + warp;

    // Band intensity: sharp sinusoidal curtains (void-black gaps between)
    float b1 = pow(max(0.0, sin(cx * curtainFreq + t * 0.5)), 3.0);
    float b2 = pow(max(0.0, sin(cx * curtainFreq * 1.6 - t * 0.4 + 2.1)), 2.5) * 0.65;
    float b3 = pow(max(0.0, sin(cx * curtainFreq * 2.3 + t * 0.3 + 4.3)), 2.0) * 0.35;
    float bands = (b1 + b2 + b3) / 2.0;

    // Hue varies across x-position and drifts over time
    float huePhase = cx * 0.2 + y * 0.08 + t * 0.06;
    vec3 col = auroraHue(huePhase) * bands * hdrAurora * audio;

    return col;
}

// =======================================================================
// EFFECT: AURORA TUNNEL — perspective rows scrolling through aurora curtains
// Text inherits aurora color at its screen position; void-black bg between rows.
// =======================================================================

vec4 effectAurora(vec2 uv) {
    float aspect  = RENDERSIZE.x / RENDERSIZE.y;
    int numChars  = charCount();
    float rws = floor(mix(3.0, 20.0, density));
    float sR  = mix(0.5, 1.5, intensity);

    float minS = 0.3 / sR, maxS = 2.5 * sR;

    float rH = 1.0 / rws;
    float sY = mod(uv.y + TIME * speed, 1.0);
    float ri = clamp(floor(sY / rH), 0.0, rws - 1.0);
    float ly = fract(sY / rH);

    float rn = (ri + 0.5) / rws;
    float dc = abs(rn - 0.5) * 2.0;
    float rs = mix(minS, maxS, dc * dc) * textScale;

    float cH = rH * rs;
    float cW = cH * (5.0 / 7.0) * (1.0 / aspect);
    float gW = cW * 0.15;
    float wordW = max(float(numChars) * (cW + gW), 0.001);

    float piw = mod(uv.x - 0.5 + wordW * 0.5, wordW);
    if (piw < 0.0) piw += wordW;
    float cs = cW + gW, csF = piw / cs;
    int slot = int(floor(csF));
    float clx = fract(csF), cf = cW / cs;
    float tsy = 0.5 - rs * 0.5;
    float gy  = (ly - tsy) / rs;

    float textHit = 0.0;
    if (clx < cf && slot >= 0 && slot < numChars && gy >= 0.0 && gy <= 1.0) {
        float gc = (clx / cf) * 5.0, gr = gy * 7.0;
        if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0) {
            int ch = getChar(slot);
            if (ch >= 0 && ch <= 36 && ch != 26) textHit = charPixel(ch, gc, gr);
        }
    }

    float audio = 1.0 + audioBass * pulse;
    vec3 bg     = auroraBg(uv, audio);

    // Text: aurora color at this row position boosted to white-hot HDR
    // Row aurora hue matches surrounding curtain for immersion
    float rowHue = uv.x * RENDERSIZE.x / RENDERSIZE.y * 0.2
                 + mod(uv.y + TIME * speed, 1.0) * 0.08
                 + TIME * 0.06 * 0.12;
    vec3 textCol = auroraHue(rowHue) * hdrText * audio;

    vec3 col = mix(bg, textCol, textHit);
    return vec4(col, 1.0);
}

void main() {
    vec2 uv  = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 col = effectAurora(uv);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band       = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise  = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift      = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt  = g * 0.015;
        vec4 cR = effectAurora(uv + vec2(shift + chromaAmt, 0.0));
        vec4 cG = effectAurora(uv + vec2(shift, chromaAmt * 0.5));
        vec4 cB = effectAurora(uv + vec2(shift - chromaAmt, 0.0));
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
