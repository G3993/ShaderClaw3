/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Bricks - grid with animated displacement on a solar plasma background",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "preset", "LABEL": "Style", "TYPE": "long", "VALUES": [0,1,2], "LABELS": ["Bricks","Bricks Harlequin","Bricks Zebra"], "DEFAULT": 0 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.5 },
    { "NAME": "intensity", "LABEL": "Displacement", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "density", "LABEL": "Grid Density", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "textScale", "LABEL": "Size", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "textColor", "LABEL": "Text Color", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "plasmaScale", "LABEL": "Plasma Scale", "TYPE": "float", "MIN": 0.5, "MAX": 8.0, "DEFAULT": 3.0 },
    { "NAME": "plasmaSpeed", "LABEL": "Plasma Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.3 },
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

// =======================================================================
// SOLAR PLASMA BACKGROUND
// =======================================================================

float hash21(vec2 p) {
    p = fract(p * vec2(234.34, 435.346));
    p += dot(p, p + 34.23);
    return fract(p.x * p.y);
}

float smoothNoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * smoothNoise(p);
        p = p * 2.1 + vec2(3.7, 1.53);
        a *= 0.5;
    }
    return v;
}

vec3 solarPlasma(vec2 uv) {
    float t = TIME * plasmaSpeed;
    vec2 q = vec2(fbm(uv * plasmaScale + vec2(t * 0.31, t * 0.13)),
                  fbm(uv * plasmaScale + vec2(t * 0.19, t * 0.43 + 5.2)));
    float f = fbm(uv * plasmaScale + q * 2.1 + vec2(t * 0.11, -t * 0.17));

    float audioBoost = 1.0 + audioLevel * audioMod * 0.5;
    f = clamp(f * audioBoost, 0.0, 1.0);

    // Solar palette: black → deep crimson → orange → gold → white-hot HDR
    vec3 col;
    float hot = hdrPeak;
    if (f < 0.25)      col = mix(vec3(0.0),            vec3(0.9, 0.04, 0.0),            f / 0.25);
    else if (f < 0.50) col = mix(vec3(0.9, 0.04, 0.0), vec3(hot * 0.7, hot * 0.25, 0.0), (f - 0.25) / 0.25);
    else if (f < 0.75) col = mix(vec3(hot * 0.7, hot * 0.25, 0.0), vec3(hot, hot * 0.6, 0.0),  (f - 0.50) / 0.25);
    else               col = mix(vec3(hot, hot * 0.6, 0.0),         vec3(hot, hot * 0.95, hot * 0.55), (f - 0.75) / 0.25);

    return col;
}

// =======================================================================
// EFFECT: BRICKS — returns textHit in alpha when plasmaBg active
// =======================================================================

vec4 effectBricks(vec2 uv, int sub) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    int numChars = charCount();
    float waveAmount = intensity;
    float cols = floor(mix(5.0, 40.0, density));

    float wX=0.0, wY=0.0, fX=3.0, fY=3.0, pm=0.0;
    bool brick = false;
    if (sub == 0) { wX=0.3; fX=2.5; brick=true; }
    else if (sub == 1) { wX=0.6; wY=0.6; pm=2.0; }
    else { wX=1.0; fX=4.0; pm=1.0; }

    float rws = floor(cols*(7.0/5.0)/aspect);
    float cellW = 1.0/cols, cellH = 1.0/rws;

    float ci = clamp(floor(uv.x/cellW), 0.0, cols-1.0);
    float ri = clamp(floor(uv.y/cellH), 0.0, rws-1.0);
    float lx = fract(uv.x/cellW), ly = fract(uv.y/cellH);

    if (brick && mod(ri, 2.0) > 0.5) {
        float sx = uv.x + cellW*0.5;
        ci = mod(floor(sx/cellW), cols);
        lx = fract(sx/cellW);
    }

    float t = TIME*speed*2.5;
    float phase = ci + ri;
    if (pm > 0.5 && pm < 1.5) phase = ri;
    else if (pm > 1.5) phase = (ci + ri)*PI;

    lx = fract(lx + sin(phase*fX+t)*waveAmount*wX*0.3);
    ly = fract(ly + sin(phase*fY+t*1.1)*waveAmount*wY*0.3);

    int charIdx = int(mod(ci + ri*cols, float(numChars)));
    float cWR = 5.0/7.0;
    float sX = textScale*cWR, sY = textScale;
    float mX = (1.0-sX)*0.5, mY = (1.0-sY)*0.5;

    float textHit = 0.0;
    if (lx >= mX && lx < 1.0-mX && ly >= mY && ly < 1.0-mY) {
        float gc = ((lx-mX)/sX)*5.0, gr = ((ly-mY)/sY)*7.0;
        if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0) {
            int ci2 = int(mod(float(charIdx), float(numChars)));
            int ch = getChar(ci2);
            if (ch >= 0 && ch <= 36 && ch != 26) textHit = charPixel(ch, gc, gr);
        }
    }

    if (transparentBg) {
        return vec4(textColor.rgb, textHit);
    }

    // Solar plasma bg: text = user textColor, bg = plasma
    return vec4(textColor.rgb, textHit);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    int p = int(preset);
    vec4 textLayer = effectBricks(uv, p);

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
            vec4 cR = effectBricks(uv + vec2(shift + chromaAmt, 0.0), p);
            vec4 cG = effectBricks(uv + vec2(shift, chromaAmt * 0.5), p);
            vec4 cB = effectBricks(uv + vec2(shift - chromaAmt, 0.0), p);
            gl_FragColor = vec4(cR.r, cG.g, cB.b, max(max(cR.a, cG.a), cB.a));
        }
        return;
    }

    // Solar plasma composite: text (black ink) over living plasma background
    vec3 plasma = solarPlasma(uv);
    vec3 finalCol = mix(plasma, textColor.rgb, textLayer.a);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt = g * 0.015;
        vec3 pR = solarPlasma(uv + vec2(shift + chromaAmt, 0.0));
        vec3 pG = solarPlasma(uv + vec2(shift, chromaAmt * 0.5));
        vec3 pB = solarPlasma(uv + vec2(shift - chromaAmt, 0.0));
        vec4 tR = effectBricks(uv + vec2(shift + chromaAmt, 0.0), p);
        vec4 tG = effectBricks(uv + vec2(shift, chromaAmt * 0.5), p);
        vec4 tB = effectBricks(uv + vec2(shift - chromaAmt, 0.0), p);
        float scanline = 0.95 + 0.05 * sin(uv.y * RENDERSIZE.y * 1.5 + t * 40.0);
        finalCol = vec3(
            mix(pR.r, textColor.r, tR.a),
            mix(pG.g, textColor.g, tG.a),
            mix(pB.b, textColor.b, tB.a)
        ) * scanline;
    }

    gl_FragColor = vec4(finalCol, 1.0);
}
