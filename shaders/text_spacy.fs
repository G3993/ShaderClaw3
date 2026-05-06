/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Spacy - perspective tunnel rows over an electric storm background",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "preset", "LABEL": "Style", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Spacy","Spacy Bridge","Spacy Whitney","Spacy Recede"], "DEFAULT": 0 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.5 },
    { "NAME": "intensity", "LABEL": "Perspective", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "density", "LABEL": "Depth", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "textScale", "LABEL": "Size", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "oscSpeed", "LABEL": "Osc Speed", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 0.0 },
    { "NAME": "oscAmount", "LABEL": "Osc Amount", "TYPE": "float", "MIN": 0.0, "MAX": 0.2, "DEFAULT": 0.0 },
    { "NAME": "oscSpread", "LABEL": "Osc Spread", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5 },
    { "NAME": "textColor", "LABEL": "Text Color", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "stormScale", "LABEL": "Storm Scale", "TYPE": "float", "MIN": 0.5, "MAX": 8.0, "DEFAULT": 2.5 },
    { "NAME": "stormSpeed", "LABEL": "Storm Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.3 },
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
float hash21s(vec2 p) {
    p = fract(p * vec2(234.34, 435.346));
    p += dot(p, p + 34.23);
    return fract(p.x * p.y);
}

// =======================================================================
// ELECTRIC STORM BACKGROUND
// =======================================================================

float smoothNoiseS(vec2 p) {
    vec2 i = floor(p); vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash21s(i), b = hash21s(i + vec2(1.0, 0.0));
    float c = hash21s(i + vec2(0.0, 1.0)), d = hash21s(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbmS(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * smoothNoiseS(p);
        p = p * 2.1 + vec2(1.7, 9.2);
        a *= 0.5;
    }
    return v;
}

vec3 stormBg(vec2 uv) {
    float t = TIME * stormSpeed;
    float h = hdrPeak;

    // Turbulent storm cloud FBM
    vec2 q = vec2(fbmS(uv * stormScale + t * 0.09),
                  fbmS(uv * stormScale + vec2(4.8, 2.3) + t * 0.07));
    float cloud = fbmS(uv * stormScale * 0.8 + q * 1.6 + t * 0.04);

    // Dark storm cloud palette: near-black → deep violet boil
    vec3 col = mix(vec3(0.008, 0.008, 0.02), vec3(0.16, 0.04, 0.28), cloud * cloud);

    // Lightning bolt 1: periodic strike
    float fire1 = smoothstep(0.82, 1.0, fract(hash(31.7) * 5.3 + t * 0.38));
    float xc1 = hash(11.0 + floor(t * 0.4)) * 0.65 + 0.18;
    float path1 = sin(uv.y * 23.0 + 2.9) * 0.022 + sin(uv.y * 9.7 + 7.1) * 0.014;
    float dist1 = abs(uv.x - xc1 - path1);
    float bolt1 = smoothstep(0.018, 0.001, dist1) * fire1;
    float glow1 = smoothstep(0.12, 0.0, dist1) * fire1 * 0.35;

    // Lightning bolt 2: offset timing
    float fire2 = smoothstep(0.87, 1.0, fract(hash(73.1) * 4.1 + t * 0.29));
    float xc2 = hash(22.0 + floor(t * 0.35)) * 0.55 + 0.22;
    float path2 = sin(uv.y * 17.0 + 5.3) * 0.019 + sin(uv.y * 6.8 + 2.7) * 0.016;
    float dist2 = abs(uv.x - xc2 - path2);
    float bolt2 = smoothstep(0.015, 0.001, dist2) * fire2;
    float glow2 = smoothstep(0.10, 0.0, dist2) * fire2 * 0.30;

    float boltTotal = bolt1 + bolt2;
    float glowTotal = glow1 + glow2;

    // Lightning: violet core → white-hot electric yellow at tip
    vec3 boltCol = mix(vec3(h * 0.45, 0.0, h), vec3(h, h * 0.92, h * 0.35), clamp(boltTotal, 0.0, 1.0));
    // Cyan atmospheric halo glow
    vec3 glowCol = vec3(0.0, glowTotal * 0.45, glowTotal) * h;

    col += boltCol * min(boltTotal, 1.5);
    col += glowCol;

    float audioBoost = 1.0 + audioLevel * audioMod * 0.5;
    col *= audioBoost;

    return col;
}

// =======================================================================
// EFFECT: SPACY — returns textHit in alpha
// =======================================================================

vec4 effectSpacy(vec2 uv, int sub) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    int numChars = charCount();
    float rws = floor(mix(3.0, 20.0, density));
    float sR = mix(0.5, 1.5, intensity);

    float minS=0.3, maxS=2.5, track=0.15, scM=1.0;
    bool mirror = false;

    if (sub == 0)      { minS=0.3/sR; maxS=2.5*sR; }
    else if (sub == 1) { minS=0.2/sR; maxS=3.0*sR; track=0.05; scM=1.4; }
    else if (sub == 2) { minS=0.4/sR; maxS=2.0*sR; track=0.2; scM=0.9; mirror=true; }
    else               { minS=0.15/sR; maxS=2.0*sR; track=0.12; }

    float rH = 1.0/rws;
    float sY = mod(uv.y + TIME*speed*scM, 1.0);
    float ri = clamp(floor(sY/rH), 0.0, rws-1.0);
    float ly = fract(sY/rH);

    float rn = (ri+0.5)/rws;
    float dc = abs(rn-0.5)*2.0;
    float rs = mix(minS, maxS, dc*dc)*textScale;

    float cH = rH*rs;
    float cW = cH*(5.0/7.0)*(1.0/aspect);
    float gW = cW*track;
    float wordW = max(float(numChars)*(cW+gW), 0.001);

    float px = uv.x;
    if (mirror && rn < 0.5) px = 1.0 - px;

    float piw = mod(px - 0.5 + wordW * 0.5, wordW);
    if (piw < 0.0) piw += wordW;
    float cs = cW+gW, csF = piw/cs;
    int slot = int(floor(csF));
    float clx = fract(csF), cf = cW/cs;
    float tsy = 0.5-rs*0.5;
    float gy = (ly-tsy)/rs;

    float textHit = 0.0;
    if (clx < cf && slot >= 0 && slot < numChars && gy >= 0.0 && gy <= 1.0) {
        float gc = (clx/cf)*5.0, gr = gy*7.0;
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
    int p = int(preset);
    vec4 textLayer = effectSpacy(uv, p);

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
            vec4 cR = effectSpacy(uv + vec2(shift + chromaAmt, 0.0), p);
            vec4 cG = effectSpacy(uv + vec2(shift, chromaAmt * 0.5), p);
            vec4 cB = effectSpacy(uv + vec2(shift - chromaAmt, 0.0), p);
            gl_FragColor = vec4(cR.r, cG.g, cB.b, max(max(cR.a, cG.a), cB.a));
        }
        return;
    }

    // Storm composite: white-hot text over electric storm
    vec3 storm = stormBg(uv);
    // Text rendered at hdrPeak brightness for white-hot silhouette against dark clouds
    vec3 finalCol = mix(storm, textColor.rgb * hdrPeak, textLayer.a);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt = g * 0.015;
        vec3 sR = stormBg(uv + vec2(shift + chromaAmt, 0.0));
        vec3 sG = stormBg(uv + vec2(shift, chromaAmt * 0.5));
        vec3 sB = stormBg(uv + vec2(shift - chromaAmt, 0.0));
        vec4 tR = effectSpacy(uv + vec2(shift + chromaAmt, 0.0), p);
        vec4 tG = effectSpacy(uv + vec2(shift, chromaAmt * 0.5), p);
        vec4 tB = effectSpacy(uv + vec2(shift - chromaAmt, 0.0), p);
        float scanline = 0.95 + 0.05 * sin(uv.y * RENDERSIZE.y * 1.5 + t * 40.0);
        vec3 tc = textColor.rgb * hdrPeak;
        finalCol = vec3(
            mix(sR.r, tc.r, tR.a),
            mix(sG.g, tc.g, tG.a),
            mix(sB.b, tc.b, tB.a)
        ) * scanline;
    }

    gl_FragColor = vec4(finalCol, 1.0);
}
