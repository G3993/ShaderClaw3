/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Cosmic Mosaic — galaxy-hued mosaic tiles with animated text displacement",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed",      "LABEL": "Speed",        "TYPE": "float", "MIN": 0.1, "MAX": 3.0,  "DEFAULT": 0.5  },
    { "NAME": "intensity",  "LABEL": "Displacement", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.5  },
    { "NAME": "density",    "LABEL": "Grid Density", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.5  },
    { "NAME": "textScale",  "LABEL": "Text Size",    "TYPE": "float", "MIN": 0.3, "MAX": 2.0,  "DEFAULT": 0.85 },
    { "NAME": "hdrText",    "LABEL": "Text HDR",     "TYPE": "float", "MIN": 1.0, "MAX": 4.0,  "DEFAULT": 2.8  },
    { "NAME": "hdrTile",    "LABEL": "Tile HDR",     "TYPE": "float", "MIN": 0.5, "MAX": 3.0,  "DEFAULT": 2.0  },
    { "NAME": "colorDrift", "LABEL": "Color Drift",  "TYPE": "float", "MIN": 0.0, "MAX": 0.15, "DEFAULT": 0.04 },
    { "NAME": "groutWidth", "LABEL": "Grout Width",  "TYPE": "float", "MIN": 0.0, "MAX": 0.12, "DEFAULT": 0.04 },
    { "NAME": "pulse",      "LABEL": "Audio Pulse",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.7  }
  ]
}*/

// Atlas-only font engine
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

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// =======================================================================
// EFFECT: COSMIC MOSAIC — galaxy-hued tiles with displaced text
// Each grid cell is a distinct mosaic tile with its own galaxy color.
// 6 hues, hash-assigned per cell, all slowly cycling together.
// Black grout lines provide ink-contrast separation.
// Text renders white-hot (HDR) over the tiles.
// =======================================================================

vec4 effectMosaic(vec2 uv) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    int numChars = charCount();
    float cols = floor(mix(5.0, 40.0, density));
    float rws  = floor(cols * (7.0 / 5.0) / aspect);
    float cellW = 1.0 / cols;
    float cellH = 1.0 / rws;

    float ci = clamp(floor(uv.x / cellW), 0.0, cols - 1.0);
    float ri = clamp(floor(uv.y / cellH), 0.0, rws  - 1.0);
    float lx = fract(uv.x / cellW);
    float ly = fract(uv.y / cellH);

    // Animated displacement
    float t     = TIME * speed * 2.5;
    float phase = ci + ri;
    lx = fract(lx + sin(phase * 2.5 + t)       * intensity * 0.3);
    ly = fract(ly + sin(phase * 3.0 + t * 1.1) * intensity * 0.3);

    // Galaxy tile color — 6 hues assigned by hash, slow global drift
    float hueN   = floor(hash(ci * 7.31 + ri * 3.17) * 6.0);
    float hue    = fract(hueN / 6.0 + TIME * colorDrift);
    float audio  = 1.0 + audioBass * pulse;
    vec3 tileCol = hsv2rgb(vec3(hue, 1.0, 1.0)) * hdrTile * audio;

    // Black grout lines at cell edges
    float grout     = min(min(lx, 1.0 - lx), min(ly, 1.0 - ly));
    float groutMask = smoothstep(0.0, groutWidth, grout);
    tileCol *= groutMask;

    // Text rendering
    int charIdx = int(mod(ci + ri * cols, float(numChars)));
    int ch      = getChar(charIdx);

    float textHit = 0.0;
    float cWR = 5.0 / 7.0;
    float sX  = textScale * cWR, sY = textScale;
    float mX  = (1.0 - sX) * 0.5,  mY = (1.0 - sY) * 0.5;

    if (lx >= mX && lx < 1.0 - mX && ly >= mY && ly < 1.0 - mY) {
        float gc = ((lx - mX) / sX) * 5.0;
        float gr = ((ly - mY) / sY) * 7.0;
        if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0 &&
                ch >= 0 && ch <= 36 && ch != 26)
            textHit = charPixel(ch, gc, gr);
    }

    // White-hot text over tile, warm white peaks
    vec3 textCol = vec3(1.0, 0.95, 0.82) * hdrText * audio;

    vec3 col = tileCol;
    col = mix(col, textCol, textHit);

    return vec4(col, 1.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 col = effectMosaic(uv);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band      = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift      = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt  = g * 0.015;
        vec4 cR = effectMosaic(uv + vec2(shift + chromaAmt, 0.0));
        vec4 cG = effectMosaic(uv + vec2(shift, chromaAmt * 0.5));
        vec4 cB = effectMosaic(uv + vec2(shift - chromaAmt, 0.0));
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
