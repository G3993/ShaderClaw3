/*{
  "CATEGORIES": ["Generator", "Text", "Audio Reactive"],
  "DESCRIPTION": "Volumetric typography haunted by audio-driven aurora ribbons that trail glyph-like silhouettes — Pipilotti Rist saturation meets the northern lights. Type as cold-light apparition.",
  "INPUTS": [
    {"NAME":"msg","TYPE":"text","DEFAULT":"AURORA","MAX_LENGTH":48},
    {"NAME":"extrudeDepth","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.6},
    {"NAME":"ribbonAmp","TYPE":"float","MIN":0.0,"MAX":0.5,"DEFAULT":0.18},
    {"NAME":"ribbonFreq","TYPE":"float","MIN":1.0,"MAX":20.0,"DEFAULT":6.0},
    {"NAME":"driftSpeed","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":0.4},
    {"NAME":"glow","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.6},
    {"NAME":"audioReact","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

// Note: Easel's text engine isn't accessible from a generic ISF generator,
// so we render *abstract glyph silhouettes* — vertical block letters built
// from rectangles arranged across screen — that read as type-shaped masses
// while the aurora ribbons handle all the visual interest behind them.

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    float a = hash(i), b = hash(i + vec2(1, 0));
    float c = hash(i + vec2(0, 1)), d = hash(i + vec2(1, 1));
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Procedural block-glyph mask. We approximate type as a row of vertical
// bars with internal cross-strokes — abstract enough to read as glyphs
// without a font atlas. Returns 1 inside character, 0 outside.
float glyphMask(vec2 uv, float slice) {
    // 6 glyph slots across center band of screen
    float gw = 0.12;
    float gx = (uv.x - 0.18) / gw;
    int idx = int(floor(gx));
    if (idx < 0 || idx > 5) return 0.0;
    vec2 g = vec2(fract(gx), (uv.y - 0.4) / 0.2);
    if (g.y < 0.0 || g.y > 1.0) return 0.0;
    // Slight perspective offset per depth slice — fakes 3D extrusion.
    g.x += slice * 0.012;
    g.y += slice * 0.008;
    // Hash-driven glyph shape: each slot picks one of three skeletal forms
    float h = hash(vec2(float(idx), 0.0));
    // Vertical bars + mid-bar variation
    float verts = step(0.18, g.x) * step(g.x, 0.32) + step(0.68, g.x) * step(g.x, 0.82);
    float mid = step(abs(g.y - 0.5), 0.08) * step(0.18, g.x) * step(g.x, 0.82);
    float top = step(0.85, g.y) * step(0.18, g.x) * step(g.x, 0.82) * step(h, 0.7);
    float bot = step(g.y, 0.15) * step(0.18, g.x) * step(g.x, 0.82) * step(h, 0.5);
    return clamp(verts + mid + top + bot, 0.0, 1.0);
}

// Aurora ribbon field — stacked sin curves with FFT-driven amplitude.
float ribbonField(vec2 uv, float freq, float amp, float t, out float ribbonY) {
    float total = 0.0;
    ribbonY = 0.0;
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        float phase = fi * 1.37;
        float yCenter = 0.3 + fi * 0.12;
        // Each ribbon's Y modulates with audioMid via amp; freq slightly varies.
        float y = yCenter + sin(uv.x * (freq + fi * 0.5) + t + phase) * amp * (0.6 + 0.4 * sin(fi));
        // Thickness driven by audioHigh + per-ribbon hash variation.
        float thick = 0.025 + audioHigh * 0.04 + 0.01 * hash(vec2(fi));
        float band = smoothstep(thick, 0.0, abs(uv.y - y));
        total += band;
        ribbonY += band * (y - 0.5);
    }
    return total;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = vec3(0.0);

    // 1. Render extruded-text silhouette (back to front). Each slice is offset
    //    and tinted darker as we recede to suggest depth.
    int SLICES = int(8.0 + extrudeDepth * 8.0);
    for (int i = 16; i >= 0; i--) {
        if (i >= SLICES) continue;
        float slice = float(i) - float(SLICES) * 0.5;
        float depth = float(i) / float(SLICES);
        float m = glyphMask(uv, slice * (extrudeDepth + 0.05) * (1.0 + audioLevel * audioReact * 0.3));
        vec3 sliceCol = mix(vec3(0.06, 0.04, 0.12), vec3(0.7, 0.85, 1.0), depth);
        col = mix(col, sliceCol, m * 0.7);
    }

    // 2. Aurora ribbons drifting over the glyphs.
    float ribbonY;
    float r = ribbonField(uv, ribbonFreq * (1.0 + audioHigh * 0.3),
                          ribbonAmp * (1.0 + audioMid * audioReact),
                          TIME * driftSpeed, ribbonY);

    // Perceptual hue sweep along x + slow drift over time.
    vec3 ribbonCol = hsv2rgb(vec3(uv.x * 0.7 + TIME * 0.04, 0.85, 1.0));

    // Optional: live video sampled along the ribbon path tints the aurora.
    if (IMG_SIZE_inputTex.x > 0.0) {
        vec3 t = texture(inputTex, vec2(uv.x, 0.5 + ribbonY * 0.5)).rgb;
        ribbonCol = mix(ribbonCol, ribbonCol * t * 1.5, 0.5);
    }

    float flare = 0.5 + audioBass * audioReact;
    col += ribbonCol * r * flare;

    // 3. Soft gaussian-ish glow halo around bright pixels (cheap fake)
    float halo = vnoise(uv * 8.0 + TIME * 0.1) * glow;
    col += ribbonCol * r * halo * 0.3;

    // Background gradient — deep night sky behind it all.
    vec3 sky = mix(vec3(0.02, 0.03, 0.08), vec3(0.0, 0.01, 0.02), uv.y);
    col += sky * (1.0 - clamp(r, 0.0, 1.0));

    gl_FragColor = vec4(col, 1.0);
}
