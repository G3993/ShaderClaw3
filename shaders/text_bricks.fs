/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Bricks - grid with animated displacement",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "preset", "LABEL": "Style", "TYPE": "long", "VALUES": [0,1,2], "LABELS": ["Bricks","Bricks Harlequin","Bricks Zebra"], "DEFAULT": 0 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.5 },
    { "NAME": "intensity", "LABEL": "Displacement", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "density", "LABEL": "Grid Density", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "textScale", "LABEL": "Size", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "textColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.0, 0.9, 1.0, 1.0] },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "hdrGlow", "LABEL": "HDR Glow", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 2.2 }
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

// =======================================================================
// BACKGROUND: FORGE FOUNDRY — hot metal plate
// =======================================================================

float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    vec2 shift = vec2(100.0);
    for (int i = 0; i < 5; i++) {
        v += a * (fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453) * 2.0 - 1.0);
        p = p * 2.0 + shift + vec2(sin(float(i) * 1.7) * 3.7, cos(float(i) * 2.3) * 2.9);
        a *= 0.5;
    }
    return v;
}

float fbmSmooth(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    for (int i = 0; i < 5; i++) {
        float angle = float(i) * 0.7 + TIME * 0.01;
        vec2 q = p + vec2(cos(angle) * float(i) * 1.3, sin(angle * 1.5) * float(i) * 0.9);
        v += a * sin(q.x * 3.7 + sin(q.y * 2.3 + float(i))) * cos(q.y * 2.9 + cos(q.x * 1.7));
        p *= 2.0;
        a *= 0.5;
    }
    return v * 0.5 + 0.5;
}

vec3 forgeBg(vec2 uv) {
    // Radiating heat glow from center
    float heat = exp(-length(uv - 0.5) * 3.0);

    // Slowly drifting hot metal surface noise
    float metal = fbmSmooth(uv * 8.0 + TIME * 0.05);

    // Combine heat and metal texture
    float intensity = clamp(heat * 1.5 + metal * 0.4, 0.0, 1.0);

    // Color ramp: black -> deep ember -> orange -> gold
    vec3 col = vec3(0.0);
    col = mix(col, vec3(0.6, 0.05, 0.0), smoothstep(0.0, 0.25, intensity));
    col = mix(col, vec3(1.0, 0.4, 0.0), smoothstep(0.25, 0.6, intensity));
    col = mix(col, vec3(1.0, 0.75, 0.1), smoothstep(0.6, 1.0, intensity));

    // HDR push for center glow — gold peaks at 2.0
    col *= 1.0 + heat * 1.0;

    // Glowing cinder sparks — 12 scattered embers
    vec2 sparkPositions[12];
    sparkPositions[0]  = vec2(0.12, 0.78);
    sparkPositions[1]  = vec2(0.85, 0.15);
    sparkPositions[2]  = vec2(0.33, 0.92);
    sparkPositions[3]  = vec2(0.67, 0.08);
    sparkPositions[4]  = vec2(0.91, 0.61);
    sparkPositions[5]  = vec2(0.08, 0.45);
    sparkPositions[6]  = vec2(0.55, 0.88);
    sparkPositions[7]  = vec2(0.22, 0.22);
    sparkPositions[8]  = vec2(0.78, 0.77);
    sparkPositions[9]  = vec2(0.44, 0.05);
    sparkPositions[10] = vec2(0.06, 0.62);
    sparkPositions[11] = vec2(0.96, 0.38);

    for (int i = 0; i < 12; i++) {
        // Sparks drift slowly using per-spark hash offsets
        float phase = hash(float(i) * 7.3 + 1.1);
        float spd   = hash(float(i) * 3.1 + 0.5) * 0.3 + 0.05;
        vec2 drift  = vec2(sin(TIME * spd + phase * 6.28) * 0.03,
                           cos(TIME * spd * 0.8 + phase * 5.1) * 0.04);
        vec2 sPos   = fract(sparkPositions[i] + drift);
        float d     = length(uv - sPos);
        float spark = exp(-d * 150.0);
        // Orange-to-white-hot spark color, HDR peak 2.5
        col += spark * vec3(2.5, 1.2, 0.2);
    }

    return col;
}

// =======================================================================
// EFFECT: BRICKS - grid with animated displacement
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

    vec3 bg = transparentBg ? bgColor.rgb : forgeBg(uv);
    vec3 boostedText = textColor.rgb * hdrGlow;

    bool inv = mod(ri, 2.0) < 1.0;
    vec3 fg = inv ? bg : boostedText;
    vec3 bgc = inv ? boostedText : bg;
    vec3 fc = mix(bgc, fg, textHit);
    float a = 1.0;
    if (transparentBg) { a = textHit; fc = boostedText; }
    return vec4(fc, a);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    int p = int(preset);
    vec4 col = effectBricks(uv, p);

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
        vec4 cR = effectBricks(uvR, p);
        vec4 cG = effectBricks(uvG, p);
        vec4 cB = effectBricks(uvB, p);
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
