/*{
  "CATEGORIES": ["Generator", "Text"],
  "DESCRIPTION": "Coral Depths — bioluminescent underwater perspective text, aquatic palette",
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
    { "NAME": "audioMod", "LABEL": "Audio Mod", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6 }
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
// EFFECT: CORAL DEPTHS — bioluminescent underwater perspective text
// =======================================================================

vec4 effectSpacy(vec2 uv, int sub) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    int numChars = charCount();
    float rws = floor(mix(3.0, 20.0, density));
    float sR = mix(0.5, 1.5, intensity);

    float minS=0.3, maxS=2.5, track=0.15, scM=1.0;
    bool mirror = false;

    if (sub == 0) { minS=0.3/sR; maxS=2.5*sR; }
    else if (sub == 1) { minS=0.2/sR; maxS=3.0*sR; track=0.05; scM=1.4; }
    else if (sub == 2) { minS=0.4/sR; maxS=2.0*sR; track=0.2; scM=0.9; mirror=true; }
    else { minS=0.15/sR; maxS=2.0*sR; track=0.12; }

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

    // ── Coral Depths: bioluminescent color logic ──────────────────────

    // Depth-based color: near rows (large rs) = bright cyan, far rows = magenta
    float depth = clamp(rs / (maxS * textScale), 0.0, 1.0);  // 0=far, 1=near
    vec3 nearColor = vec3(0.0, 2.5, 2.5);   // bio cyan (near/large)
    vec3 farColor  = vec3(2.5, 0.0, 1.5);   // bio magenta (far/small)
    vec3 rowColor  = mix(farColor, nearColor, depth);

    // Bioluminescent pulse: text brightness pulses with TIME per row
    float pulse = 0.8 + 0.2 * sin(TIME * speed * 2.0 + ri * 0.9);

    // Volumetric water scatter: faint blue-green glow between rows
    vec3 bg = vec3(0.0, 0.0, 0.0);
    float waterGlow = (1.0 - depth) * 0.04 * (sin(TIME * 0.3 + rn * PI) * 0.5 + 0.5);
    bg += vec3(0.0, 0.04, 0.06) * waterGlow;

    // Floating sparkle particles (bioluminescent plankton)
    float sparkX = floor(uv.x * 40.0 + TIME * 0.15);
    float sparkY = floor(uv.y * 25.0 + TIME * speed * 0.3);
    float sparkSeed = hash(sparkX * 7.3 + sparkY * 13.1);
    float sparkBright = step(0.97, sparkSeed) * (0.5 + 0.5 * sin(TIME * 3.0 + sparkSeed * TWO_PI));
    bg += vec3(0.5, 2.5, 1.5) * sparkBright * 0.15;

    // Slow current — gentle horizontal undulation in background
    float currentPhase = sin(uv.y * 8.0 + TIME * 0.4) * 0.5 + 0.5;
    bg += vec3(0.0, 0.02, 0.05) * currentPhase * (1.0 - depth) * 0.3;

    // Audio: pulse modulates glow intensity (modulates, does not gate)
    float audioBoost = 1.0 + audioLevel * audioMod * 0.35;

    // Text glow + background
    float glow = textHit * pulse * audioBoost;
    vec3 fc = bg + rowColor * glow;

    return vec4(fc, 1.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    int p = int(preset);
    vec4 col = effectSpacy(uv, p);

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
        vec4 cR = effectSpacy(uvR, p);
        vec4 cG = effectSpacy(uvG, p);
        vec4 cB = effectSpacy(uvB, p);
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
