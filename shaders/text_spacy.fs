/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Spacy — perspective tunnel rows over a solar corona background: chromosphere, prominences, and ejection plumes. Deep orange/crimson/white-hot palette.",
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
    { "NAME": "textColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.04, 0.0, 0.0, 1.0] },
    { "NAME": "hdrGlow", "LABEL": "HDR Glow", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 2.0 },
    { "NAME": "coronaScale", "LABEL": "Corona Scale", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "coronaSpeed", "LABEL": "Corona Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.3 },
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

float hash(float n)  { return fract(sin(n * 127.1) * 43758.5453); }
float hash2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float noise(vec2 p){
    vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
    return mix(mix(hash2(i),hash2(i+vec2(1,0)),f.x),
               mix(hash2(i+vec2(0,1)),hash2(i+vec2(1,1)),f.x),f.y);
}
float fbm(vec2 p){ float v=0.0,a=0.5; for(int i=0;i<4;i++){v+=a*noise(p);p*=2.1;a*=0.5;} return v; }

// ──────────────────────────────────────────────────────────────────────
// Solar corona background — chromosphere gradient + prominence arcs + ejection plumes
// Deep orange / deep crimson / white-hot palette — fully saturated
// ──────────────────────────────────────────────────────────────────────
vec3 solarCoronaBg(vec2 uv){
    float audio = 1.0 + (audioLevel + audioBass * 0.8) * audioMod;
    float t = TIME * coronaSpeed;
    vec2 center = vec2(0.5, 0.5);
    vec2 rel = (uv - center) / coronaScale;
    float r = length(rel);
    float ang = atan(rel.y, rel.x);

    // Solar disk
    float diskR = 0.22;
    float inDisk = step(r, diskR);

    // Chromosphere gradient (deep orange to white-hot at core)
    vec3 diskCol;
    float diskT = clamp(1.0 - r / diskR, 0.0, 1.0);
    if(diskT < 0.5) diskCol = mix(vec3(0.8, 0.2, 0.0), vec3(1.0, 0.55, 0.0), diskT*2.0);
    else             diskCol = mix(vec3(1.0, 0.55, 0.0), vec3(1.0, 0.95, 0.7), (diskT-0.5)*2.0);
    diskCol *= hdrGlow * audio * diskT;

    // Corona glow halo
    float corona = exp(-max(r - diskR, 0.0) * 4.0 / coronaScale);
    vec3 coronaCol = mix(vec3(0.8, 0.3, 0.0), vec3(0.3, 0.0, 0.0), clamp(r - diskR, 0.0, 1.0));
    coronaCol *= corona * hdrGlow * 0.8 * audio;

    // Prominence arcs: 3 FBM-shaped arcs around disk edge
    vec3 prom = vec3(0.0);
    for(float pi2 = 0.0; pi2 < 3.0; pi2++){
        float pAng = pi2 * TWO_PI / 3.0 + t * 0.2 + pi2 * 1.7;
        float arcR = diskR + 0.06 + 0.03 * sin(t * 0.7 + pi2 * 2.3);
        float arcW = 0.03;
        // FBM wavy arc
        float warp = fbm(rel * 5.0 + vec2(t * 0.3 + pi2 * 7.1, 0.0)) * 0.08;
        float dArc = abs(r - arcR - warp);
        float dAng = abs(mod(ang - pAng + PI, TWO_PI) - PI);
        float arcMask = smoothstep(arcW, 0.0, dArc) * smoothstep(1.2, 0.0, dAng);
        prom += vec3(1.0, 0.4, 0.0) * arcMask * hdrGlow * 0.7;
    }

    // Solar wind streaks (radial, faint)
    float streakAng = fract((ang / TWO_PI + t * 0.03) * 24.0);
    float streak = exp(-abs(streakAng - 0.5) * 20.0) * exp(-max(r - diskR - 0.02, 0.0) * 8.0);
    vec3 windCol = vec3(0.9, 0.5, 0.1) * streak * hdrGlow * 0.3 * audio;

    // Void bg
    vec3 voidCol = bgColor.rgb;
    float bgFade = exp(-r * 3.0) * 0.4;
    voidCol += vec3(0.15, 0.02, 0.0) * bgFade;

    vec3 result = voidCol;
    result += coronaCol;
    result += prom;
    result += windCol;
    result = mix(result, diskCol, inDisk);

    return result;
}

// ──────────────────────────────────────────────────────────────────────
// Spacy text effect
// ──────────────────────────────────────────────────────────────────────
float effectSpacyHit(vec2 uv, int sub) {
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
    return textHit;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    int p = int(preset);

    float textHit = effectSpacyHit(uv, p);

    vec3 bg = transparentBg ? bgColor.rgb : solarCoronaBg(uv);
    float audio = 1.0 + audioLevel * audioMod * 0.4;
    // Depth-based brightness: close rows brighter
    float rowIdx = floor(uv.y * floor(mix(3.0, 20.0, density)));
    float rowCenter = (rowIdx + 0.5) / floor(mix(3.0, 20.0, density));
    float depthBright = 0.5 + abs(rowCenter - 0.5) * 2.0;
    vec3 textCol = textColor.rgb * hdrGlow * audio * depthBright;

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
        float tR = effectSpacyHit(uv + vec2(shift + chromaAmt, 0.0), p);
        float tG = effectSpacyHit(uv + vec2(shift, chromaAmt * 0.5), p);
        float tB = effectSpacyHit(uv + vec2(shift - chromaAmt, 0.0), p);
        vec3 glitched = mix(bg, textCol, (tR + tG + tB) / 3.0);
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = vec4(col, a);
}
