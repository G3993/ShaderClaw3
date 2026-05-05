/*{
    "DESCRIPTION": "Signal Weave — 2D interwoven neon scanline strips with per-strip color and glitch displacement. Fully saturated HDR palette: hot magenta, data green, electric blue, signal red, gold, cyan. fwidth AA on edges.",
    "CATEGORIES": ["Generator", "Glitch", "Neon", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "stripCount",   "TYPE": "float", "DEFAULT": 28.0, "MIN": 6.0,  "MAX": 60.0, "LABEL": "Strip Count" },
        { "NAME": "glitchAmt",    "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 1.5,  "LABEL": "Glitch Amount" },
        { "NAME": "weaveDepth",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0,  "LABEL": "Weave Depth" },
        { "NAME": "hdrPeak",      "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",     "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash11(float n)  { return fract(sin(n*127.1)*43758.5453); }
float hash21(vec2 p)   { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// 6-color neon palette — fully saturated, no white mixing
vec3 neonPalette(int i) {
    if (i == 0) return vec3(1.0,  0.05, 0.8);  // hot magenta
    if (i == 1) return vec3(0.0,  1.0,  0.1);  // data green
    if (i == 2) return vec3(0.0,  0.4,  1.0);  // electric blue
    if (i == 3) return vec3(1.0,  0.05, 0.0);  // signal red
    if (i == 4) return vec3(1.0,  0.75, 0.0);  // gold
    return             vec3(0.0,  0.9,  1.0);  // cyan
}

void main() {
    float asp = RENDERSIZE.x / RENDERSIZE.y;
    vec2 uv = isf_FragNormCoord * vec2(asp, 1.0);
    float t = TIME;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;

    // -- Horizontal strips --
    float stripH = 1.0 / stripCount;
    float stripIdx = floor(uv.y / stripH);
    float stripY   = fract(uv.y / stripH); // 0..1 within strip

    // Per-strip color
    int ci = int(mod(stripIdx + floor(t * 0.8), 6.0));
    vec3 baseCol = neonPalette(ci);

    // Glitch: each strip horizontally displaced by hash + slow drift
    float glitchSeed = hash11(stripIdx * 3.17 + floor(t * 3.0) * 0.13);
    float glitchOffset = (glitchSeed - 0.5) * glitchAmt;
    // Rare big glitch bursts
    float bigGlitch = step(0.93, hash11(stripIdx * 7.3 + floor(t * 12.0) * 0.37)) * 0.4;
    glitchOffset += (hash11(stripIdx * 13.7 + t) - 0.5) * bigGlitch;

    float glitchedX = uv.x + glitchOffset;

    // -- Vertical weave strips --
    float wStripH = asp / stripCount;
    float wStripIdx = floor(glitchedX / wStripH);
    float wStripX   = fract(glitchedX / wStripH);

    // Weave over/under logic: alternating dominance
    float weavePhase = mod(stripIdx + wStripIdx, 2.0);
    float weaveBoost = mix(1.0, 0.3, weavePhase * weaveDepth);

    // fwidth AA on strip edges
    float hEdge = fwidth(stripY);
    float hLine = smoothstep(0.0, hEdge*2.0, stripY) * smoothstep(1.0, 1.0-hEdge*2.0, stripY);

    float vEdge = fwidth(wStripX);
    float vLine = smoothstep(0.0, vEdge*2.0, wStripX) * smoothstep(1.0, 1.0-vEdge*2.0, wStripX);

    // Black ink gap between strips
    float inkGap = hLine * vLine;

    // Vertical strip color (orthogonal palette shift)
    int wci = int(mod(wStripIdx + floor(t * 0.5 + 3.0), 6.0));
    vec3 wCol = neonPalette(wci);

    // Mix H and V strip colors based on weave
    vec3 col = mix(baseCol, wCol, weavePhase * 0.5) * weaveBoost;

    // HDR: strip centers bright, edges slightly darker
    float centerBright = (1.0 - abs(stripY * 2.0 - 1.0)) * 0.3 + 0.7;
    col *= centerBright * hdrPeak * audio;

    // Black ink gap between strips
    col *= inkGap;

    // Scanline modulation: subtle brightness variation
    col *= 0.85 + 0.15 * sin(uv.y * stripCount * 6.2832 * 0.5);

    gl_FragColor = vec4(col, 1.0);
}
