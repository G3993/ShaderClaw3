/*{
  "CATEGORIES": ["Generator", "Text", "3D"],
  "DESCRIPTION": "Neon Corridor — text floating as HDR signage inside a 3D raymarched infinite corridor with glowing neon-brick walls.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "INPUTS": [
    { "NAME": "msg",       "TYPE": "text",  "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily","LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "speed",     "LABEL": "Speed",      "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.6 },
    { "NAME": "textScale", "LABEL": "Text Size",  "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "hdrPeak",   "LABEL": "HDR Peak",   "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.5 },
    { "NAME": "textColor", "LABEL": "Sign Color", "TYPE": "color", "DEFAULT": [0.0, 1.0, 1.0, 1.0] },
    { "NAME": "wallColor", "LABEL": "Wall Tint",  "TYPE": "color", "DEFAULT": [0.8, 0.2, 1.0, 1.0] },
    { "NAME": "audioMod",  "LABEL": "Audio Mod",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7 }
  ]
}*/

// ── Atlas font helpers ──────────────────────────────────────────────────
float charPixel(int ch, float col, float row) {
    if (ch < 0 || ch > 36) return 0.0;
    vec2 uv = vec2(col / 5.0, row / 7.0);
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    return smoothstep(0.1, 0.55, texture2D(fontAtlasTex, vec2((float(ch) + uv.x) / 37.0, uv.y)).r);
}
int getChar(int slot) {
    if (slot == 0)  return int(msg_0);  if (slot == 1)  return int(msg_1);
    if (slot == 2)  return int(msg_2);  if (slot == 3)  return int(msg_3);
    if (slot == 4)  return int(msg_4);  if (slot == 5)  return int(msg_5);
    if (slot == 6)  return int(msg_6);  if (slot == 7)  return int(msg_7);
    if (slot == 8)  return int(msg_8);  if (slot == 9)  return int(msg_9);
    if (slot == 10) return int(msg_10); if (slot == 11) return int(msg_11);
    if (slot == 12) return int(msg_12); if (slot == 13) return int(msg_13);
    if (slot == 14) return int(msg_14); if (slot == 15) return int(msg_15);
    if (slot == 16) return int(msg_16); if (slot == 17) return int(msg_17);
    if (slot == 18) return int(msg_18); if (slot == 19) return int(msg_19);
    if (slot == 20) return int(msg_20); if (slot == 21) return int(msg_21);
    if (slot == 22) return int(msg_22); if (slot == 23) return int(msg_23);
    if (slot == 24) return int(msg_24); if (slot == 25) return int(msg_25);
    if (slot == 26) return int(msg_26); if (slot == 27) return int(msg_27);
    if (slot == 28) return int(msg_28); if (slot == 29) return int(msg_29);
    if (slot == 30) return int(msg_30); if (slot == 31) return int(msg_31);
    if (slot == 32) return int(msg_32); if (slot == 33) return int(msg_33);
    if (slot == 34) return int(msg_34); if (slot == 35) return int(msg_35);
    if (slot == 36) return int(msg_36); if (slot == 37) return int(msg_37);
    if (slot == 38) return int(msg_38); if (slot == 39) return int(msg_39);
    if (slot == 40) return int(msg_40); if (slot == 41) return int(msg_41);
    if (slot == 42) return int(msg_42); if (slot == 43) return int(msg_43);
    if (slot == 44) return int(msg_44); if (slot == 45) return int(msg_45);
    if (slot == 46) return int(msg_46); return int(msg_47);
}
int charCount() { int n = int(msg_len); return n > 0 ? n : 1; }

// ── Corridor 3D raymarch ────────────────────────────────────────────────
float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// Brick pattern SDF in wall-space (u=horizontal, v=vertical)
float brickPattern(vec2 wuv) {
    float row = floor(wuv.y * 6.0);
    float offset = mod(row, 2.0) * 0.5;
    vec2 bCell = vec2(fract(wuv.x * 8.0 + offset), fract(wuv.y * 6.0));
    float mortar = smoothstep(0.04, 0.06, bCell.x) * smoothstep(0.04, 0.06, bCell.y)
                 * smoothstep(0.04, 0.06, 1.0 - bCell.x) * smoothstep(0.04, 0.06, 1.0 - bCell.y);
    return mortar;
}

vec3 corridorColor(vec3 p, vec3 rd) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    // Corridor: 2 wide, 1.5 tall, extends in Z
    float halfW = 1.0, halfH = 0.75;

    // Determine which wall was hit
    float dLeft   = p.x - (-halfW);
    float dRight  = halfW - p.x;
    float dFloor  = p.y - (-halfH);
    float dCeil   = halfH - p.y;
    float minD    = min(min(dLeft, dRight), min(dFloor, dCeil));

    vec2 wuv;
    vec3 wallN;
    if (minD == dLeft)  { wuv = p.zy * vec2(0.25, 0.33); wallN = vec3( 1.0, 0.0, 0.0); }
    else if (minD == dRight) { wuv = p.zy * vec2(0.25, 0.33); wallN = vec3(-1.0, 0.0, 0.0); }
    else if (minD == dFloor) { wuv = p.xz * vec2(0.5, 0.3);   wallN = vec3( 0.0, 1.0, 0.0); }
    else                 { wuv = p.xz * vec2(0.5, 0.3);   wallN = vec3( 0.0,-1.0, 0.0); }

    float brick = brickPattern(fract(wuv + 0.5));
    float depth = exp(-max(0.0, p.z - 0.5) * 0.18); // fade to dark at depth

    // Neon strip lights along ceiling edges
    float stripLeft  = smoothstep(0.05, 0.0, abs(p.x + halfW * 0.9)) * smoothstep(0.05, 0.0, abs(p.y - halfH));
    float stripRight = smoothstep(0.05, 0.0, abs(p.x - halfW * 0.9)) * smoothstep(0.05, 0.0, abs(p.y - halfH));
    float neonStrip  = max(stripLeft, stripRight);

    // Wall color: wallColor tint on brick faces, dark mortar
    vec3 brickCol = wallColor.rgb * brick * depth * 0.6;
    vec3 neonCol  = textColor.rgb * neonStrip * hdrPeak * (1.0 + audioBass * audioMod * 0.4);
    vec3 floorCol = vec3(0.03, 0.02, 0.06) * depth;

    return brickCol + neonCol + floorCol;
}

float sdCorridor(vec3 p) {
    float halfW = 1.0, halfH = 0.75;
    vec2 d = abs(p.xy) - vec2(halfW, halfH);
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

// ── Text rendering ─────────────────────────────────────────────────────
float sampleText(vec2 uv) {
    int numChars = charCount();
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float charW = textScale * 0.7 / float(numChars) / aspect;
    float charH = textScale * 0.12;
    float startX = 0.5 - charW * float(numChars) * 0.5;
    float startY = 0.5 - charH * 0.5;

    for (int i = 0; i < 48; i++) {
        if (i >= numChars) break;
        float fi = float(i);
        float x0 = startX + fi * charW;
        float x1 = x0 + charW;
        if (uv.x < x0 || uv.x > x1 || uv.y < startY || uv.y > startY + charH) continue;
        float gc = ((uv.x - x0) / charW) * 5.0;
        float gr = ((uv.y - startY) / charH) * 7.0;
        int ch = getChar(i);
        if (ch >= 0 && ch <= 36) return charPixel(ch, gc, gr);
    }
    return 0.0;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 uvC = uv * 2.0 - 1.0;
    uvC.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.4;
    float t = TIME * speed;

    // Camera moving forward through corridor
    vec3 ro = vec3(0.0, 0.0, -fract(t * 0.25) * 4.0);
    vec3 rd = normalize(vec3(uvC * vec2(1.0, 1.0), 1.5));

    // Raymarch corridor (box interior)
    vec3 col = vec3(0.0, 0.0, 0.01);
    float dist = 0.05;
    for (int i = 0; i < 60; i++) {
        vec3 p = ro + rd * dist;
        // Repeat corridor in Z
        p.z = mod(p.z + 4.0, 4.0);
        float d = -sdCorridor(p); // inside: negative SDF
        if (d > -0.02) {
            // Hit corridor wall
            col = corridorColor(p, rd);
            break;
        }
        if (dist > 30.0) break;
        dist += max(abs(d), 0.05);
    }

    // HDR neon text sign overlay (2D, screen space)
    float textHit = sampleText(uv);
    if (textHit > 0.01) {
        vec3 signCol = textColor.rgb * hdrPeak * audio * textHit;
        // Glow halo
        col = mix(col, signCol, min(textHit * 2.0, 1.0));
        col += signCol * 0.3; // additive glow
    }

    gl_FragColor = vec4(col, 1.0);
}
