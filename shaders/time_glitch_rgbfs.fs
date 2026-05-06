/*{
  "DESCRIPTION": "RGB Prism Rings 2D — concentric rings with per-channel glitch offsets creating chromatic aberration moiré patterns",
  "CREDIT": "ShaderClaw auto-improve 2026-05-06",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Speed",        "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0  },
    { "NAME": "ringDensity", "LABEL": "Ring Density", "TYPE": "float", "DEFAULT": 8.0, "MIN": 2.0, "MAX": 20.0 },
    { "NAME": "glitchAmt",   "LABEL": "Glitch Amount","TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0  },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.0, "MIN": 1.0, "MAX": 4.0  },
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0  }
  ]
}*/

// ── Palette ────────────────────────────────────────────────────────────────
// Signal red:    vec3(2.0, 0.0, 0.0)  — HDR saturated red
// Data green:    vec3(0.0, 2.0, 0.0)  — HDR saturated green
// Electric blue: vec3(0.0, 0.0, 2.5)  — HDR saturated blue
// Void black:    vec3(0.0)            — pure dark

// ── Per-band glitch displacement ───────────────────────────────────────────
float glitchOffset(float d, float t, float ch, float amt) {
    // Quantize distance into bands to get block-glitch steps
    float band = floor(d * 6.0 + t * 0.5);
    float noise = fract(sin(band * 91.7 + ch * 137.5 + t * 0.1) * 43758.5453);
    return (noise - 0.5) * amt;
}

// ── Ring mask with fwidth AA ───────────────────────────────────────────────
float ringMask(float dist, float freq, float phase, float ringWidth) {
    float wave = sin(dist * freq + phase);
    float threshold = 1.0 - ringWidth * 2.0;
    float fw = fwidth(wave) * 1.5;
    return smoothstep(threshold - fw, threshold + fw, wave);
}

void main() {
    float tm = TIME * speed;

    // Audio modulation
    float audioMod = audioReact * (audioBass * 0.5 + audioLevel * 0.15);
    float glitch = glitchAmt + audioMod * 0.4;
    float ringW  = 0.25 + audioMod * 0.15;

    // ── UV centered, aspect-corrected ─────────────────────────────────────
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float freq = ringDensity * 2.5;

    // ── Red channel: no rotation ──────────────────────────────────────────
    float distR = length(uv);
    distR += glitchOffset(distR, tm, 0.0, glitch);
    float maskR = ringMask(distR, freq, tm * 1.0, ringW);

    // ── Green channel: slight rotation to create moiré with R ─────────────
    float cosG = cos(0.05), sinG = sin(0.05);
    vec2 uvG = vec2(uv.x * cosG - uv.y * sinG, uv.x * sinG + uv.y * cosG);
    float distG = length(uvG);
    distG += glitchOffset(distG, tm + 1.7, 1.0, glitch);
    float maskG = ringMask(distG, freq * 1.02, tm * 1.1, ringW);

    // ── Blue channel: slight counter-rotation for tri-channel moiré ────────
    float cosB = cos(-0.07), sinB = sin(-0.07);
    vec2 uvB = vec2(uv.x * cosB - uv.y * sinB, uv.x * sinB + uv.y * cosB);
    float distB = length(uvB);
    distB += glitchOffset(distB, tm + 3.1, 2.0, glitch);
    float maskB = ringMask(distB, freq * 0.98, tm * 0.9, ringW);

    // ── HDR color assembly — additive blend ──────────────────────────────
    vec3 colR = maskR * vec3(2.0, 0.0, 0.0) * hdrPeak; // signal red
    vec3 colG = maskG * vec3(0.0, 2.0, 0.0) * hdrPeak; // data green
    vec3 colB = maskB * vec3(0.0, 0.0, 2.5) * hdrPeak; // electric blue

    // Additive: overlapping rings of different channels sum to white-hot
    vec3 col = colR + colG + colB;

    // HDR output — no clamp, no tonemapping, no ACES
    gl_FragColor = vec4(col, 1.0);
}
