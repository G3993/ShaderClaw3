/*{
  "CATEGORIES": ["Generator", "Text", "3D"],
  "DESCRIPTION": "Digital Waterfall — holographic text planes cascade downward through a 3D void. Camera looks up from below at falling light sheets.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "INPUTS": [
    { "NAME": "msg",         "TYPE": "text",  "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily",  "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "fallSpeed",   "LABEL": "Fall Speed",  "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 0.7 },
    { "NAME": "planeCount",  "LABEL": "Planes",      "TYPE": "float", "MIN": 2.0, "MAX": 8.0, "DEFAULT": 5.0 },
    { "NAME": "textScale",   "LABEL": "Text Size",   "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",    "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.5 },
    { "NAME": "textColor",   "LABEL": "Text Color",  "TYPE": "color", "DEFAULT": [0.0, 1.0, 0.8, 1.0] },
    { "NAME": "accentColor", "LABEL": "Accent",      "TYPE": "color", "DEFAULT": [1.0, 0.2, 0.7, 1.0] },
    { "NAME": "audioMod",    "LABEL": "Audio Mod",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7 }
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

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// Sample text characters tiled horizontally centered on a plane
float sampleTextRow(vec2 planeUV) {
    int numChars = charCount();
    float charAR = 5.0 / 7.0;
    float totalW = textScale * charAR * float(numChars);
    float startX = (1.0 - totalW) * 0.5;
    float charW = totalW / float(numChars);
    float charH = textScale;
    float startY = (1.0 - charH) * 0.5;

    if (planeUV.y < startY || planeUV.y > startY + charH) return 0.0;

    int ci = int(clamp((planeUV.x - startX) / charW, 0.0, float(numChars) - 1.0));
    if (planeUV.x < startX || planeUV.x > startX + totalW) return 0.0;

    float lx = (planeUV.x - (startX + float(ci) * charW)) / charW;
    float ly = (planeUV.y - startY) / charH;
    int ch = getChar(ci);
    return charPixel(ch, lx * 5.0, ly * 7.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.4;
    float t = TIME;

    // Camera: looking up from below, slightly angled
    vec3 ro = vec3(sin(t * 0.05) * 0.3, -3.5, 0.0);
    vec3 target = vec3(0.0, 2.0, 0.5);
    vec3 fwd = normalize(target - ro);
    vec3 rgt = normalize(cross(vec3(0.0, 0.0, 1.0), fwd));
    vec3 upV = cross(fwd, rgt);
    vec3 rd  = normalize(fwd + uv.x * rgt * 0.75 + uv.y * upV * 0.75);

    // Void background with subtle nebula glow
    vec3 col = vec3(0.0, 0.0, 0.01);
    float nebula = exp(-length(uv) * 1.2);
    col += mix(textColor.rgb, accentColor.rgb, sin(t * 0.07) * 0.5 + 0.5) * nebula * 0.06;

    float nb = floor(clamp(planeCount, 2.0, 8.0));

    for (int pi = 0; pi < 8; pi++) {
        if (float(pi) >= nb) break;
        float fi = float(pi);

        // Each plane falls at its own speed offset, looping every 6 units
        float yOffset = hash11(fi * 3.7) * 6.0;
        float fallPhase = fi / nb;
        float planeY = mod(8.0 - t * fallSpeed * (0.7 + fi * 0.1) + yOffset, 8.0) - 2.0;

        // Ray–plane intersection at y = planeY
        if (abs(rd.y) < 0.001) continue;
        float rayT = (planeY - ro.y) / rd.y;
        if (rayT < 0.1 || rayT > 40.0) continue;
        vec3 p = ro + rd * rayT;

        // Plane UV: centered at origin, x goes ±2, z goes ±1.5
        vec2 planeUV = vec2(p.x / 3.5 + 0.5, p.z / 2.0 + 0.5);
        if (planeUV.x < 0.0 || planeUV.x > 1.0 || planeUV.y < 0.0 || planeUV.y > 1.0) continue;

        float textHit = sampleTextRow(planeUV);
        float depth = 1.0 - clamp(rayT / 30.0, 0.0, 1.0);

        // Alternating textColor / accentColor per plane
        vec3 planeCol = (mod(fi, 2.0) < 1.0) ? textColor.rgb : accentColor.rgb;
        float pHue = fi / nb;

        // HDR glow: text pixels shine brightly
        vec3 glow = planeCol * textHit * hdrPeak * audio * depth;
        // Soft translucent plane body (background haze of this holographic sheet)
        vec3 haze = planeCol * 0.04 * depth * smoothstep(0.0, 0.05, 1.0 - abs(textHit - 0.0));

        // Additive compositing: further planes blend in, nearer dominate
        col += glow + haze;
    }

    // Particle sparks falling with the planes
    for (int si = 0; si < 30; si++) {
        float fs = float(si);
        float sx  = hash11(fs * 7.3 + 1.0) * 4.0 - 2.0;
        float sz  = hash11(fs * 3.1 + 2.0) * 3.0 - 1.5;
        float sy  = mod(8.0 - t * fallSpeed * (0.5 + hash11(fs * 11.7) * 0.8)
                    + hash11(fs * 5.9) * 8.0, 8.0) - 2.0;
        if (abs(rd.y) < 0.001) continue;
        float rt = (sy - ro.y) / rd.y;
        if (rt < 0.0 || rt > 40.0) continue;
        vec3 sp = ro + rd * rt;
        float sdist = length(sp.xz - vec2(sx, sz)) * 12.0;
        float spark = exp(-sdist * sdist) * 0.5 * (1.0 - rt / 40.0);
        vec3 sparkCol = mix(textColor.rgb, accentColor.rgb, hash11(fs * 2.3));
        col += sparkCol * spark * hdrPeak * 0.4 * audio;
    }

    gl_FragColor = vec4(col, 1.0);
}
