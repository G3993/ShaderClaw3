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
// BACKGROUND: LAVA FLOW
// Domain-warped FBM lava channels — pure warm fire palette, no blue/purple
// =======================================================================

vec3 generateBackground(vec2 uv) {
    // Slow time drift
    float t = TIME * 0.05;

    // Domain warp pass 1: offset uv by low-freq sin noise
    vec2 q;
    q.x = sin(uv.x * 3.1 + uv.y * 2.7 + t * 1.3) * 0.18
        + sin(uv.x * 1.5 - uv.y * 3.9 + t * 0.9) * 0.12;
    q.y = sin(uv.y * 2.8 + uv.x * 3.3 - t * 1.1) * 0.18
        + sin(uv.y * 4.1 - uv.x * 1.7 + t * 0.7) * 0.10;

    // Domain warp pass 2: offset again for richer channels
    vec2 r;
    r.x = sin((uv.x + q.x) * 4.2 + (uv.y + q.y) * 1.9 + t * 1.7) * 0.14
        + sin((uv.x + q.x) * 2.1 - (uv.y + q.y) * 4.4 + t * 0.6) * 0.09;
    r.y = sin((uv.y + q.y) * 3.7 + (uv.x + q.x) * 2.5 - t * 1.4) * 0.14
        + sin((uv.y + q.y) * 1.8 - (uv.x + q.x) * 3.2 + t * 1.0) * 0.08;

    // Warped sample position
    vec2 wp = uv + q + r;

    // 3-octave FBM lava field from sin-based noise
    float lava = 0.0;
    float amp = 1.0;
    float freq = 1.0;
    // Octave 1
    lava += amp * (sin(wp.x * freq * 5.3 + wp.y * freq * 3.7 + t * 2.1) * 0.5 + 0.5);
    amp *= 0.55; freq *= 2.1;
    // Octave 2
    lava += amp * (sin(wp.x * freq * 5.3 + wp.y * freq * 3.7 - t * 1.7 + 1.3) * 0.5 + 0.5);
    amp *= 0.55; freq *= 2.1;
    // Octave 3
    lava += amp * (sin(wp.x * freq * 5.3 + wp.y * freq * 3.7 + t * 2.5 + 2.6) * 0.5 + 0.5);

    // Normalize to 0..1
    float total = 1.0 + 0.55 + 0.55 * 0.55;
    lava /= total;

    // Sharpen: create narrow bright channels (crevice = dark, flow = bright)
    float channel = pow(lava, 2.2);

    // Secondary crackling detail layer
    float crack = sin(wp.x * 18.0 + t * 3.0) * sin(wp.y * 14.0 - t * 2.3);
    crack = crack * 0.5 + 0.5;
    crack = pow(crack, 4.0);

    // Combine: base channel + hot crack highlights
    float heat = clamp(channel + crack * 0.35, 0.0, 1.0);

    // 3-stop color ramp: black crevice -> lava orange -> gold-white HDR peak
    vec3 colBlack  = vec3(0.0, 0.0, 0.0);
    vec3 colOrange = vec3(2.0, 0.5, 0.0);   // HDR lava orange
    vec3 colGold   = vec3(2.5, 2.0, 0.2);   // HDR gold-white

    vec3 bgCol;
    if (heat < 0.45) {
        bgCol = mix(colBlack, colOrange, heat / 0.45);
    } else {
        bgCol = mix(colOrange, colGold, (heat - 0.45) / 0.55);
    }

    // Subtle pulsing ember glow driven by TIME
    float pulse = sin(TIME * 0.8 + lava * TWO_PI) * 0.5 + 0.5;
    bgCol += colOrange * pulse * 0.15 * channel;

    return bgCol;
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

    bool inv = mod(ri, 2.0) < 1.0;
    vec3 fg = inv ? bgColor.rgb : textColor.rgb;
    vec3 bg = inv ? textColor.rgb : bgColor.rgb;
    vec3 fc = mix(bg, fg, textHit);
    float a = 1.0;
    if (transparentBg) { a = textHit; fc = textColor.rgb; }
    return vec4(fc, a);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    int p = int(preset);

    if (!transparentBg) {
        // Compute text hit by sampling effectBricks for alpha info
        vec4 brickResult = effectBricks(uv, p);
        // In opaque (non-transparent) mode effectBricks returns alpha=1 always,
        // so we need to re-derive textHit. We do this by comparing result to pure bg.
        // Instead, refactor: compute textHit directly then composite.
        float aspect = RENDERSIZE.x / RENDERSIZE.y;
        int numChars = charCount();
        float waveAmount = intensity;
        float cols = floor(mix(5.0, 40.0, density));

        float wX=0.0, wY=0.0, fX=3.0, fY=3.0, pm=0.0;
        bool brick = false;
        if (p == 0) { wX=0.3; fX=2.5; brick=true; }
        else if (p == 1) { wX=0.6; wY=0.6; pm=2.0; }
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

        float t2 = TIME*speed*2.5;
        float phase = ci + ri;
        if (pm > 0.5 && pm < 1.5) phase = ri;
        else if (pm > 1.5) phase = (ci + ri)*PI;

        lx = fract(lx + sin(phase*fX+t2)*waveAmount*wX*0.3);
        ly = fract(ly + sin(phase*fY+t2*1.1)*waveAmount*wY*0.3);

        int charIdx = int(mod(ci + ri*cols, float(numChars)));
        float cWR = 5.0/7.0;
        float sX = textScale*cWR, sY = textScale;
        float mX2 = (1.0-sX)*0.5, mY2 = (1.0-sY)*0.5;

        float textHit = 0.0;
        if (lx >= mX2 && lx < 1.0-mX2 && ly >= mY2 && ly < 1.0-mY2) {
            float gc = ((lx-mX2)/sX)*5.0, gr = ((ly-mY2)/sY)*7.0;
            if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0) {
                int ci2 = int(mod(float(charIdx), float(numChars)));
                int ch = getChar(ci2);
                if (ch >= 0 && ch <= 36 && ch != 26) textHit = charPixel(ch, gc, gr);
            }
        }

        vec3 bg = generateBackground(uv);
        vec3 finalColor = bg + textHit * textColor.rgb * hdrGlow * audioMod;
        gl_FragColor = vec4(finalColor, 1.0);
        return;
    }

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
