/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Digifade — glitch dissolve text over a live laser arena: sweeping spotlight beams and grid floor. Hot red/gold palette.",
  "INPUTS": [
    { "NAME": "msg", "TYPE": "text", "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "preset", "LABEL": "Style", "TYPE": "long", "VALUES": [0,1], "LABELS": ["Digifade","Digifade Glitch"], "DEFAULT": 0 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.5 },
    { "NAME": "intensity", "LABEL": "Glitch", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "density", "LABEL": "Dissolve", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "textScale", "LABEL": "Size", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "textColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [1.0, 0.7, 0.0, 1.0] },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.02, 0.0, 0.0, 1.0] },
    { "NAME": "hdrGlow", "LABEL": "HDR Glow", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 2.5 },
    { "NAME": "beamCount", "LABEL": "Laser Beams", "TYPE": "float", "MIN": 2.0, "MAX": 12.0, "DEFAULT": 6.0 },
    { "NAME": "beamSpeed", "LABEL": "Beam Speed", "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.8 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "audioMod", "LABEL": "Audio Mod", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
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

// ──────────────────────────────────────────────────────────────────────
// Laser arena background — sweeping spotlight beams from top corners
// Grid floor perspective for stage feel. Hot red/gold palette.
// ──────────────────────────────────────────────────────────────────────
vec3 laserArenaBg(vec2 uv){
    float audio = 1.0 + (audioLevel + audioBass * 0.8) * audioMod;
    float t = TIME * beamSpeed;
    vec3 col = bgColor.rgb;

    // Perspective grid floor (lower half)
    if(uv.y < 0.48){
        float dh = max(0.48 - uv.y, 0.002);
        vec2 gridUV = vec2((uv.x - 0.5) / (dh * 2.5), 1.0 / dh - t * 0.3);
        float gx = abs(fract(gridUV.x * 10.0) - 0.5);
        float gy = abs(fract(gridUV.y) - 0.5);
        float lineW = 0.04 * dh;
        float line = smoothstep(0.5 - lineW, 0.5, max(gx, gy));
        vec3 floorBase = mix(vec3(0.06, 0.0, 0.0), vec3(0.25, 0.0, 0.0), uv.y / 0.48);
        col = mix(floorBase, vec3(0.9, 0.3, 0.0), line * 0.6);
    }

    // Spotlight beams from top — each sweeps sinusoidally
    int N = int(clamp(beamCount, 2.0, 12.0));
    for(int i = 0; i < 12; i++){
        if(i >= N) break;
        float fi = float(i);
        // Beam origin at top: evenly spaced, some at left/right edges
        float origX = (fi + 0.5) / float(N);
        float origY = 1.02;

        // Sweep angle driven by sin at per-beam phase
        float phase = fi * 1.3 + t;
        float dirX = sin(phase * 0.7) * 0.5;
        float targetX = 0.5 + dirX;
        float targetY = 0.0;

        vec2 beamDir = normalize(vec2(targetX - origX, targetY - origY));
        vec2 pixRel = uv - vec2(origX, origY);

        // Distance from beam axis
        float projLen = dot(pixRel, beamDir);
        float perpDist = abs(length(pixRel - beamDir * projLen));
        float beamWidth = 0.02 + audio * 0.005;
        float beamGlow = exp(-perpDist * perpDist / (beamWidth * beamWidth));
        beamGlow *= step(0.0, projLen);  // only below origin
        beamGlow *= (1.0 - clamp(projLen / 1.5, 0.0, 1.0));  // fade with distance

        // Alternating hues: red vs gold
        vec3 beamCol = (mod(fi, 2.0) < 1.0) ? vec3(1.0, 0.1, 0.0) : vec3(1.0, 0.75, 0.0);
        col += beamCol * beamGlow * hdrGlow * audio;
    }

    // Ambient haze at bottom (stage smoke)
    float haze = exp(-uv.y * 8.0) * 0.3;
    col += vec3(0.5, 0.1, 0.0) * haze;

    return col;
}

// ──────────────────────────────────────────────────────────────────────
// Digifade text effect
// ──────────────────────────────────────────────────────────────────────
float effectDigifadeHit(vec2 uv, int sub) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    int numChars = charCount();
    float glitchAmount = intensity;
    float sliceCount = mix(5.0, 100.0, density);

    float complexity = 1.0, sweepSpeed = 1.0, vertGlitch = 0.0, maxDisp = 0.3;
    if (sub == 1) { complexity = 2.0; sweepSpeed = 1.3; maxDisp = 0.5; vertGlitch = 0.4; }

    float t = TIME * speed * sweepSpeed;
    vec2 p = vec2((uv.x - 0.5) * aspect + 0.5, uv.y);

    float cH = 0.18 * textScale;
    if (aspect < 1.0) cH *= aspect;
    float cW = cH * (5.0/7.0);
    float gW = cW * 0.2;
    float totalTextW = float(numChars) * cW + float(numChars - 1) * gW;
    float maxW = 0.9 * aspect;
    float fitScale = totalTextW > maxW ? maxW / totalTextW : 1.0;
    cH *= fitScale; cW *= fitScale; gW *= fitScale;

    float rowW = float(numChars) * cW + float(numChars - 1) * gW;
    float startX = 0.5 - rowW * 0.5;
    float startY = 0.5 - cH * 0.5;

    float si = floor(uv.y * sliceCount);
    float n1 = hash(si + floor(t*2.0));
    float n2 = hash(si*3.7 + floor(t*3.0));

    float sw = sin(t*0.7)*0.5+0.5;
    float ps = smoothstep(sw-0.15, sw+0.1, (p.x-startX)/max(rowW, 0.001));
    float dx = abs(ps*n1*glitchAmount*maxDisp + ps*sin(si*0.3*complexity+t)*glitchAmount*maxDisp*0.3);
    float dy = vertGlitch > 0.01 ? ps*(n2-0.5)*vertGlitch*glitchAmount*0.06 : 0.0;

    vec2 samp = vec2(p.x - dx, p.y - dy);
    float rx = samp.x - startX, ry = samp.y - startY;
    float textHit = 0.0;

    if (rx >= 0.0 && rx <= rowW && ry >= 0.0 && ry <= cH) {
        float cs = cW + gW;
        float csF = rx / cs;
        int slot = int(floor(csF));
        float clx = fract(csF), cf = cW/cs;
        if (clx < cf && slot >= 0 && slot < numChars) {
            float gc = (clx/cf)*5.0, gr = (ry/cH)*7.0;
            if (gc >= 0.0 && gc < 5.0 && gr >= 0.0 && gr < 7.0) {
                int ch = getChar(slot);
                if (ch >= 0 && ch <= 36 && ch != 26) textHit = max(textHit, charPixel(ch, gc, gr));
            }
        }
    }
    return textHit;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    int p = int(preset);

    float textHit = effectDigifadeHit(uv, p);

    vec3 bg = transparentBg ? bgColor.rgb : laserArenaBg(uv);
    float audio = 1.0 + audioLevel * audioMod * 0.4;
    vec3 textCol = textColor.rgb * hdrGlow * audio;

    vec3 col = mix(bg, textCol, textHit);
    float a = transparentBg ? textHit : 1.0;

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t2 = TIME * 17.0;
        float band = floor(uv.y * mix(8.0, 40.0, g) + t2 * 3.0);
        float bandNoise = fract(sin(band * 91.7 + t2) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt = g * 0.015;
        float tR = effectDigifadeHit(uv + vec2(shift + chromaAmt, 0.0), p);
        float tG = effectDigifadeHit(uv + vec2(shift, chromaAmt * 0.5), p);
        float tB = effectDigifadeHit(uv + vec2(shift - chromaAmt, 0.0), p);
        vec3 glitched = mix(bg, textCol, (tR + tG + tB) / 3.0);
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = vec4(col, a);
}
