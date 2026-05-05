/*{
  "DESCRIPTION": "Databend Mosaic — 2D horizontal strip databending with saturated RGB color blocks, glitch displacement, and white-hot scan line flashes. No input required.",
  "CREDIT": "ShaderClaw auto-improve v15",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "Glitch"],
  "INPUTS": [
    { "NAME": "stripCount",  "LABEL": "Strip Count",  "TYPE": "float", "DEFAULT": 48.0,  "MIN": 8.0,  "MAX": 120.0 },
    { "NAME": "blockW",      "LABEL": "Block Width",  "TYPE": "float", "DEFAULT": 0.05,  "MIN": 0.01, "MAX": 0.2 },
    { "NAME": "glitchAmt",   "LABEL": "Glitch",       "TYPE": "float", "DEFAULT": 0.7,   "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "flashRate",   "LABEL": "Flash Rate",   "TYPE": "float", "DEFAULT": 0.15,  "MIN": 0.0,  "MAX": 0.5 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,   "MIN": 1.0,  "MAX": 4.0 },
    { "NAME": "audioMod",    "LABEL": "Audio Mod",    "TYPE": "float", "DEFAULT": 0.6,   "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash12(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float t = TIME;
    float audio = 1.0 + (audioLevel + audioBass * 0.4) * audioMod;

    // Strip index (horizontal bands)
    float stripH = 1.0 / stripCount;
    float stripIdx = floor(uv.y / stripH);
    float stripFrac = fract(uv.y / stripH);

    // Per-strip hash values
    float sh1 = hash11(stripIdx * 1.37 + floor(t * 2.0));
    float sh2 = hash11(stripIdx * 7.31 + floor(t * 3.0));
    float sh3 = hash11(stripIdx * 13.7 + floor(t * 1.5));

    // Glitch: horizontal displacement per strip
    float disp = 0.0;
    float isGlitch = step(1.0 - glitchAmt * 0.6, sh1);
    disp = (sh2 - 0.5) * 0.3 * isGlitch;

    // Chromatic aberration: R/G/B channels offset independently
    float chromaR = uv.x + disp + sh3 * 0.02 * isGlitch;
    float chromaG = uv.x + disp;
    float chromaB = uv.x + disp - sh3 * 0.02 * isGlitch;

    // Block color: per-horizontal-block hash
    float blockIdxR = floor(mod(chromaR, 1.0) / blockW);
    float blockIdxG = floor(mod(chromaG, 1.0) / blockW);
    float blockIdxB = floor(mod(chromaB, 1.0) / blockW);

    float hueR = hash12(vec2(blockIdxR, stripIdx + floor(t * 4.0)));
    float hueG = hash12(vec2(blockIdxG, stripIdx + floor(t * 3.0) + 17.0));
    float hueB = hash12(vec2(blockIdxB, stripIdx + floor(t * 5.0) + 37.0));

    vec3 colR = hsv2rgb(vec3(hueR, 1.0, 1.0));
    vec3 colG = hsv2rgb(vec3(hueG, 1.0, 1.0));
    vec3 colB = hsv2rgb(vec3(hueB, 1.0, 1.0));

    // Recombine channels (databend: each channel from its own displaced position)
    vec3 col = vec3(colR.r, colG.g, colB.b) * hdrPeak * audio;

    // Scanline flash: occasional bright white lines
    float flashLine = step(1.0 - flashRate, hash11(stripIdx * 0.73 + floor(t * 12.0)));
    col += vec3(1.0) * flashLine * hdrPeak * 1.5; // white-hot flash

    // Strip boundary: thin black line between strips
    col *= smoothstep(0.0, 0.04, stripFrac) * smoothstep(1.0, 0.96, stripFrac);

    // Dark strips (intentional null blocks)
    float isDark = step(0.88, hash11(stripIdx * 3.17 + floor(t * 1.0)));
    col *= 1.0 - isDark * 0.95;

    gl_FragColor = vec4(col, 1.0);
}
