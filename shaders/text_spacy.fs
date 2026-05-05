/*{
  "CATEGORIES": ["Generator", "Text", "3D"],
  "DESCRIPTION": "Accretion Text — text orbits a 3D raymarched black hole with glowing gold accretion disk. Gravitational lensing warps the star field.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "INPUTS": [
    { "NAME": "msg",         "TYPE": "text",  "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily",  "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "orbitSpeed",  "LABEL": "Orbit Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.35 },
    { "NAME": "textScale",   "LABEL": "Text Size",    "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.8 },
    { "NAME": "diskColor",   "LABEL": "Disk Color",   "TYPE": "color", "DEFAULT": [1.0, 0.6, 0.05, 1.0] },
    { "NAME": "textColor",   "LABEL": "Text Color",   "TYPE": "color", "DEFAULT": [0.9, 0.95, 1.0, 1.0] },
    { "NAME": "lensStrength","LABEL": "Lens Strength","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8 },
    { "NAME": "audioMod",    "LABEL": "Audio Mod",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7 }
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
float hash12(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// ── Accretion disk: torus SDF rendered as emissive ring ─────────────────
float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

// Procedural star field (lensed)
float stars(vec2 lensedDir) {
    vec2 d = lensedDir * 60.0;
    vec2 cell = floor(d);
    float h = hash12(cell);
    float b = step(0.985, h);
    float b2 = hash12(cell + vec2(1.0, 0.0));
    return b * (0.4 + b2 * 0.6);
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.4;
    float t = TIME;

    // Slightly above equatorial plane, looking at black hole
    vec3 ro = vec3(0.0, 0.8, -5.0);
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 upV = cross(fwd, rgt);
    vec3 rd   = normalize(fwd + uv.x * rgt * 0.85 + uv.y * upV * 0.85);

    // ── Star background with gravitational lensing ──
    float bhR   = length(uv);
    float lensFactor = lensStrength / max(bhR * bhR * 8.0, 0.3);
    vec2 lensedUV = uv + normalize(uv) * lensFactor * 0.1;
    float starB = stars(lensedUV);
    vec3 col = vec3(0.0, 0.0, 0.005) + vec3(0.85, 0.9, 1.0) * starB * 0.6;

    // ── Black hole disk (event horizon) ──
    float bhShadow = smoothstep(0.18, 0.12, bhR);
    col *= (1.0 - bhShadow);

    // ── Accretion disk: 3D torus marched from camera ──
    float diskR = 1.2, diskr = 0.18;
    float tDisk = -1.0;
    float dist = 0.1;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dist;
        float d = sdTorus(p, diskR, diskr);
        if (d < 0.005) { tDisk = dist; break; }
        if (dist > 15.0) break;
        dist += max(d * 0.6, 0.005);
    }

    if (tDisk > 0.0) {
        vec3 p = ro + rd * tDisk;
        float diskAngle = atan(p.z, p.x);
        float diskR2 = length(p.xz);
        // Rotation: inner disk spins faster
        float spin = diskAngle + t * orbitSpeed * (1.8 / max(diskR2, 0.5));
        // Turbulent brightness variation in disk
        float turbulence = sin(spin * 6.0) * 0.3 + sin(diskAngle * 14.0 + t * 2.0) * 0.2 + 0.8;
        vec3 dCol = diskColor.rgb * hdrPeak * audio * turbulence;
        // Depth fade on far side
        float shadowFade = 1.0 - smoothstep(3.0, 6.0, tDisk);
        col += dCol * shadowFade;
    }

    // ── Photon ring: bright arc just outside event horizon ──
    float ringR = 0.16;
    float photonRing = exp(-abs(bhR - ringR) * 40.0);
    col += diskColor.rgb * hdrPeak * audio * photonRing * 1.5;

    // ── Orbiting text: characters on a circular orbit ──
    int numChars = charCount();
    float orbitRadius = 2.2;
    float charAng = (2.0 * 3.14159265 / float(numChars));
    float baseAngle = t * orbitSpeed;

    for (int ci = 0; ci < 48; ci++) {
        if (ci >= numChars) break;
        float fi = float(ci);
        float angle = baseAngle + fi * charAng;
        // 3D position on circular orbit in XZ plane
        vec3 charPos = vec3(cos(angle) * orbitRadius, 0.0, sin(angle) * orbitRadius);
        // Project to screen
        vec3 toChar = charPos - ro;
        float dotFwd = dot(toChar, fwd);
        if (dotFwd < 0.1) continue;
        vec2 projXY = vec2(dot(toChar, rgt), dot(toChar, upV)) / dotFwd;
        // Character size in screen space
        float charSize = textScale * 0.08 / max(dotFwd * 0.2, 0.5);
        float charHgt = charSize;
        float charWdt = charSize * 0.7;
        vec2 delta = uv - projXY;
        if (abs(delta.x) > charWdt || abs(delta.y) > charHgt) continue;
        float gc = (delta.x / charWdt + 1.0) * 2.5;
        float gr = (delta.y / charHgt + 1.0) * 3.5;
        int ch = getChar(ci);
        float hit = charPixel(ch, gc, gr);
        if (hit < 0.01) continue;
        // Dim on the far/back side of orbit
        float facing = cos(angle - baseAngle + 3.14159265 * 0.5);
        float vis = smoothstep(-0.5, 0.5, facing);
        // Doppler-like color shift: approaching=blue, receding=red
        float doppler = (sin(angle) * 0.5 + 0.5);
        vec3 charCol = mix(vec3(0.8, 0.4, 1.0), textColor.rgb, doppler);
        col += charCol * hdrPeak * audio * hit * vis;
    }

    gl_FragColor = vec4(col, 1.0);
}
