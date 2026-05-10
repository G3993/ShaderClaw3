/*{
  "DESCRIPTION": "Diagonal Film Splice Glitch — the frame is cut into diagonal bands; each band gets an independent per-channel color grade and horizontal displacement, simulating a physically spliced film print with chromatic aberration. Standalone generator (no inputImage needed). Palette: signal red, data green, electric blue — fully saturated HDR 2.0+. Completely different from prior 3D-plane and ring-glitch angles.",
  "CREDIT": "ShaderClaw auto-improve — diagonal film splice",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "bandCount",  "LABEL": "Band Count",   "TYPE": "float", "DEFAULT": 8.0,  "MIN": 3.0,  "MAX": 20.0 },
    { "NAME": "bandAngle",  "LABEL": "Band Angle",   "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.2 },
    { "NAME": "dispAmt",    "LABEL": "Displacement", "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.0,  "MAX": 0.15 },
    { "NAME": "chromaAmt",  "LABEL": "Chroma Split", "TYPE": "float", "DEFAULT": 0.012,"MIN": 0.0,  "MAX": 0.05 },
    { "NAME": "glitchRate", "LABEL": "Glitch Rate",  "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0,  "MAX": 1.5 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.5,  "MAX": 4.0 },
    { "NAME": "audioReact", "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

#define PI 3.14159265

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash12(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// Smooth noise
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash12(i);
    float b = hash12(i + vec2(1.0, 0.0));
    float c = hash12(i + vec2(0.0, 1.0));
    float d = hash12(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// Procedural film frame content: abstract color bars + noise grain
// Each color channel has a slightly different spatial frequency
vec3 filmContent(vec2 uv, float bandIdx) {
    float h = hash11(bandIdx * 3.17 + 0.5);
    float h2 = hash11(bandIdx * 7.43 + 1.1);

    // Vertical color bars (different per band)
    float barW  = 0.06 + h * 0.12;
    float barX  = mod(uv.x / barW, 1.0);
    float barIdx= floor(uv.x / barW) + bandIdx * 37.0;

    // Fully saturated bar colors cycling: signal red, data green, electric blue, gold, magenta, cyan
    float barHue = hash11(barIdx) * 5.0;
    vec3 barCol;
    int bi = int(mod(floor(barHue), 6.0));
    if      (bi == 0) barCol = vec3(1.0, 0.03, 0.10); // signal red
    else if (bi == 1) barCol = vec3(0.05, 1.0, 0.20); // data green
    else if (bi == 2) barCol = vec3(0.08, 0.18, 1.0); // electric blue
    else if (bi == 3) barCol = vec3(1.0,  0.72, 0.0);  // gold
    else if (bi == 4) barCol = vec3(1.0,  0.05, 0.90); // magenta
    else               barCol = vec3(0.0,  1.0,  0.85); // cyan

    // Add film grain
    float grain = vnoise(uv * vec2(180.0, 90.0) + bandIdx * 47.0) * 0.18;
    barCol = barCol * (0.85 + grain);

    // Scanline overlay
    float scan = 0.92 + 0.08 * sin(uv.y * 240.0);
    barCol *= scan;

    return barCol;
}

void main() {
    vec2 res = RENDERSIZE;
    vec2 uv  = isf_FragNormCoord.xy;
    float t  = TIME;

    // Audio — K ≤ 1.2 per motion rules §2
    float audio = clamp(audioLevel * audioReact, 0.0, 1.5);
    float aBoost = 1.0 + audio * min(audioReact * 0.4, 1.2);

    // Diagonal band coordinate
    float angle  = bandAngle;
    float diagUV = uv.x + uv.y * tan(angle); // diagonal axis

    // Epoch: which glitch snapshot is each band showing
    // rate ≤ 0.2 per motion rules §4 (min ~5s period at audio=0)
    float epochRate = glitchRate * 0.15; // keep ≤ 0.2
    float epoch     = floor(t * epochRate);

    // Band index from diagonal coordinate
    float bandF  = diagUV * bandCount;
    float bandIdx= floor(bandF);
    float bandFrac = fract(bandF);

    // Per-band glitch parameters (change each epoch)
    float gSeed  = hash12(vec2(bandIdx, epoch));
    float gSeed2 = hash12(vec2(bandIdx * 3.7, epoch + 0.5));
    float gSeed3 = hash12(vec2(bandIdx * 5.1, epoch + 1.2));

    // Horizontal displacement for this band
    float disp = (gSeed - 0.5) * 2.0 * dispAmt;
    // Some bands glitch hard, most are stable
    float isGlitch = step(0.75, gSeed3);
    disp *= isGlitch;

    // Per-band UV with displacement and chromatic aberration
    vec2 uvR = vec2(uv.x + disp + chromaAmt,  uv.y);
    vec2 uvG = vec2(uv.x + disp,               uv.y);
    vec2 uvB = vec2(uv.x + disp - chromaAmt,  uv.y);

    // Diagonal band for R/G/B channels (slight offset so they desync)
    float bIdxR = floor((uvR.x + uvR.y * tan(angle)) * bandCount);
    float bIdxG = floor((uvG.x + uvG.y * tan(angle)) * bandCount);
    float bIdxB = floor((uvB.x + uvB.y * tan(angle)) * bandCount);

    float epochR = floor(t * epochRate + hash11(bIdxR * 0.3) * 0.5);
    float epochG = floor(t * epochRate + hash11(bIdxG * 0.5) * 0.5);
    float epochB = floor(t * epochRate + hash11(bIdxB * 0.7) * 0.5);

    float dispR = (hash12(vec2(bIdxR, epochR)) - 0.5) * 2.0 * dispAmt *
                   step(0.72, hash12(vec2(bIdxR * 3.1, epochR + 1.7)));
    float dispG = (hash12(vec2(bIdxG, epochG)) - 0.5) * 2.0 * dispAmt *
                   step(0.72, hash12(vec2(bIdxG * 2.9, epochG + 0.8)));
    float dispB = (hash12(vec2(bIdxB, epochB)) - 0.5) * 2.0 * dispAmt *
                   step(0.72, hash12(vec2(bIdxB * 3.7, epochB + 2.1)));

    vec2 sampR = clamp(vec2(uv.x + dispR + chromaAmt,  uv.y), vec2(0.0), vec2(1.0));
    vec2 sampG = clamp(vec2(uv.x + dispG,               uv.y), vec2(0.0), vec2(1.0));
    vec2 sampB = clamp(vec2(uv.x + dispB - chromaAmt,   uv.y), vec2(0.0), vec2(1.0));

    float r = filmContent(sampR, bIdxR).r;
    float g = filmContent(sampG, bIdxG).g;
    float b = filmContent(sampB, bIdxB).b;

    vec3 col = vec3(r, g, b) * hdrPeak * aBoost;

    // Band splice lines: thin bright white-flash at band boundaries
    float bandEdge = fwidth(bandF) * 0.5;
    float edgeMask = exp(-abs(bandFrac - 0.0) / (bandEdge + 0.002));
    edgeMask      += exp(-abs(bandFrac - 1.0) / (bandEdge + 0.002));
    edgeMask       = clamp(edgeMask * 0.4, 0.0, 0.4);
    col += vec3(2.5, 2.3, 2.0) * edgeMask; // bright splice flash

    // Dropout: occasional black bands (physical splice tape)
    float dropout = step(0.94, hash12(vec2(bandIdx, epoch * 2.7)));
    col *= (1.0 - dropout * 0.9);

    // Vignette
    vec2 vc = uv - 0.5;
    col *= 1.0 - 0.5 * dot(vc, vc) * 2.0;

    gl_FragColor = vec4(max(col, vec3(0.0)), 1.0);
}
