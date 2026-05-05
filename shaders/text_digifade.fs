/*{
  "CATEGORIES": ["Generator", "Text", "3D"],
  "DESCRIPTION": "Ember Scatter — text shatters into glowing amber embers that drift outward in 3D space. Painterly warm-light close-up portrait.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "INPUTS": [
    { "NAME": "msg",        "TYPE": "text",  "DEFAULT": " ETHEREA", "MAX_LENGTH": 48 },
    { "NAME": "fontFamily", "LABEL": "Font", "TYPE": "long", "VALUES": [0,1,2,3], "LABELS": ["Inter","Times New Roman","Libre Caslon","Outfit"], "DEFAULT": 0 },
    { "NAME": "fadeSpeed",  "LABEL": "Fade Speed",  "TYPE": "float", "MIN": 0.1, "MAX": 2.0,  "DEFAULT": 0.5 },
    { "NAME": "textScale",  "LABEL": "Text Size",   "TYPE": "float", "MIN": 0.3, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "MIN": 1.0, "MAX": 4.0,  "DEFAULT": 2.8 },
    { "NAME": "emberColor", "LABEL": "Ember Color", "TYPE": "color", "DEFAULT": [1.0, 0.5, 0.0, 1.0] },
    { "NAME": "coreColor",  "LABEL": "Core Color",  "TYPE": "color", "DEFAULT": [1.0, 0.9, 0.3, 1.0] },
    { "NAME": "audioMod",   "LABEL": "Audio Mod",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.7 }
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

// Sample the text at a given [0,1]² UV on a 2D text plane
float sampleText(vec2 p) {
    int numChars = charCount();
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float charW = textScale * 0.65 / float(numChars);
    float charH = textScale * 0.14;
    float startX = 0.5 - charW * float(numChars) * 0.5;
    float startY = 0.5 - charH * 0.5;
    for (int i = 0; i < 48; i++) {
        if (i >= numChars) break;
        float fi = float(i);
        float x0 = startX + fi * charW;
        if (p.x < x0 || p.x > x0 + charW || p.y < startY || p.y > startY + charH) continue;
        int ch = getChar(i);
        if (ch < 0 || ch > 36) return 0.0;
        return charPixel(ch, ((p.x - x0) / charW) * 5.0, ((p.y - startY) / charH) * 7.0);
    }
    return 0.0;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 uvC = uv * 2.0 - 1.0;
    uvC.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;
    float t = TIME * fadeSpeed;

    // Phase: 0=solid text, 1=dissolving into embers, loop period ~4s
    float phase = fract(t * 0.25);
    float dissolve = smoothstep(0.2, 0.8, phase); // 0=solid, 1=fully scattered

    // Black background with warm coal glow at center
    float coalGlow = exp(-length(uvC) * 2.5) * dissolve;
    vec3 col = vec3(0.01, 0.005, 0.0) + emberColor.rgb * 0.04 * coalGlow;

    // ── Static text base (solid phase) ──
    float textHit = sampleText(uv);
    vec3 solidText = mix(coreColor.rgb * hdrPeak * audio,
                         emberColor.rgb * hdrPeak * audio * 0.7, textHit * textHit);
    col += solidText * textHit * (1.0 - dissolve);

    // ── Ember particles: text pixels that have detached and drift in 3D ──
    // 3D: each ember starts at text-pixel position and drifts outward + upward
    // We approximate with a dense grid of embers seeded from text positions
    int numChars = charCount();
    float charW = textScale * 0.65 / float(numChars);
    float charH = textScale * 0.14;

    for (int ci = 0; ci < 48; ci++) {
        if (ci >= numChars) break;
        // Sample a grid of points within this character's bounding box
        for (int gi = 0; gi < 12; gi++) {
            float fi = float(ci), fg = float(gi);
            float seed = fi * 13.7 + fg * 3.9;
            float startX = 0.5 - charW * float(numChars) * 0.5 + fi * charW;

            // Sample point within character bounding box
            float px = startX + hash11(seed) * charW;
            float py = 0.5 - charH * 0.5 + hash11(seed + 7.3) * charH;

            // Check if this grid point lands on a text pixel
            float glyph = sampleText(vec2(px, py));
            if (glyph < 0.3) continue;

            // Drift direction: outward + upward, slight turbulence
            float angle = atan(py - 0.5, px - 0.5) + hash11(seed + 1.1) * 0.8 - 0.4;
            float speed2 = 0.2 + hash11(seed + 2.3) * 0.4;
            float delay  = hash11(seed + 5.7) * 0.3;
            float life   = max(0.0, dissolve - delay) / (1.0 - delay + 0.001);

            // Ember world position (2D screen space, with Z depth illusion)
            float zDrift = life * 0.6 * hash11(seed + 8.1);
            float scale  = 1.0 + zDrift; // grows as it comes "toward" camera
            vec2 ePos = vec2(px + cos(angle) * speed2 * life,
                             py + sin(angle) * speed2 * life + life * life * 0.08);
            ePos = vec2(ePos.x - 0.5, ePos.y - 0.5) * scale + 0.5;

            // Fade: embers brighten briefly then fade
            float brightness = smoothstep(0.0, 0.15, life) * smoothstep(1.0, 0.6, life);
            // Size grows with depth illusion
            float radius = (0.004 + zDrift * 0.008) * (1.0 + audioBass * audioMod * 0.3);

            float d = length(uv - ePos);
            float ember = exp(-d * d / (radius * radius)) * brightness;
            if (ember < 0.001) continue;

            // Color: white-hot core → amber → dark ember edge
            float heatRamp = ember;
            vec3 eCol = mix(emberColor.rgb, coreColor.rgb, heatRamp * heatRamp);
            col += eCol * hdrPeak * audio * ember;
        }
    }

    // ── Reformation phase: embers converge back ──
    float reForm = smoothstep(0.8, 1.0, phase);
    col += solidText * textHit * reForm * hdrPeak * audio * 0.5;

    gl_FragColor = vec4(col, 1.0);
}
