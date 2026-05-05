/*{
  "DESCRIPTION": "RGB Time Glitch — standalone glitch art generator. Procedural scan-line bands, RGB channel split with time offsets, block corruption, VHS noise and CRT scanlines. Audio-reactive. HDR linear output.",
  "CREDIT": "ShaderClaw auto-improve 2026-05-05 (concept from VIDVOX time-buffer glitch)",
  "CATEGORIES": ["Generator", "Glitch"],
  "INPUTS": [
    {"NAME":"glitchAmount","LABEL":"Glitch Amount","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.35},
    {"NAME":"chromaShift","LABEL":"Chroma Shift","TYPE":"float","MIN":0.0,"MAX":0.08,"DEFAULT":0.018},
    {"NAME":"blockSize","LABEL":"Block Size","TYPE":"float","MIN":0.01,"MAX":0.25,"DEFAULT":0.08},
    {"NAME":"scanDensity","LABEL":"Scan Density","TYPE":"float","MIN":4.0,"MAX":40.0,"DEFAULT":18.0},
    {"NAME":"colorA","LABEL":"Color A","TYPE":"color","DEFAULT":[0.0,1.0,0.85,1.0]},
    {"NAME":"colorB","LABEL":"Color B","TYPE":"color","DEFAULT":[1.0,0.1,0.5,1.0]},
    {"NAME":"colorC","LABEL":"Color C","TYPE":"color","DEFAULT":[0.9,0.95,1.0,1.0]},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0},
    {"NAME":"inputTex","LABEL":"Input","TYPE":"image"}
  ]
}*/

float rand(vec2 co) { return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453); }
float rand1(float x) { return fract(sin(x * 12.9898) * 43758.5453); }

// Procedural VHS signal — what the shader generates when no inputTex is given.
// Three horizontal frequency bands with color noise + bright scan pulses.
vec3 vhsSignal(vec2 uv, float t) {
    float band  = floor(uv.y * 8.0 + t * 0.3);
    float noise = rand(vec2(uv.x * 0.5, band + t));
    float bCol  = rand1(band);

    // 3-color palette: colorA / colorB / colorC
    vec3 c = (bCol < 0.33) ? colorA.rgb : ((bCol < 0.66) ? colorB.rgb : colorC.rgb);
    c *= 0.6 + 0.4 * noise;

    // Horizontal scan bright lines — HDR
    float scanY = fract(uv.y * scanDensity - t * 0.8);
    float scan  = smoothstep(0.0, 0.04, scanY) * smoothstep(0.12, 0.04, scanY);
    c += colorC.rgb * scan * 1.8;

    // Vertical bar pattern shifting with time
    float bar = step(0.85, fract(uv.x * 3.0 + t * 0.15));
    c *= 1.0 - bar * 0.4;

    return c;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float t  = TIME;
    float audioMod = 0.5 + 0.5 * audioLevel * audioReact;
    float bassGate = audioBass * audioReact;

    // Audio pushes glitch intensity
    float g = glitchAmount * (1.0 + bassGate * 0.8);

    // ── Band glitch: random horizontal strips shift in X ──
    float bandH     = max(blockSize, 0.01);
    float bandIdx   = floor(uv.y / bandH);
    float bandNoise = rand(vec2(bandIdx, floor(t * 6.0)));
    float bandActive = step(1.0 - g, bandNoise);
    float shift     = (rand(vec2(bandIdx, floor(t * 8.0) + 1.0)) - 0.5) * g * 0.12 * bandActive;

    // ── Block corruption: larger chunks teleport ──
    float bigBlockH = bandH * 4.0;
    float bigIdx    = floor(uv.y / bigBlockH);
    float bigNoise  = rand(vec2(bigIdx + 17.3, floor(t * 3.0)));
    float bigActive = step(1.0 - g * 0.5, bigNoise);
    vec2 bigShift   = (vec2(rand(vec2(bigIdx, 33.1)), rand(vec2(bigIdx + 1.0, 33.1))) - 0.5) * g * 0.08 * bigActive;

    // ── Chromatic aberration — R/G/B shifted differently ──
    float cShift = chromaShift * (1.0 + bassGate * 0.5);
    vec2 uvR = uv + vec2(shift + bigShift.x + cShift,  bigShift.y);
    vec2 uvG = uv + vec2(shift + bigShift.x,            bigShift.y);
    vec2 uvB = uv + vec2(shift + bigShift.x - cShift,  bigShift.y);

    // Clamp coords to [0,1]
    uvR = clamp(uvR, 0.0, 1.0);
    uvG = clamp(uvG, 0.0, 1.0);
    uvB = clamp(uvB, 0.0, 1.0);

    // ── Source signal ──
    vec3 sigR, sigG, sigB;
    if (IMG_SIZE_inputTex.x > 0.0) {
        sigR = texture(inputTex, uvR).rgb;
        sigG = texture(inputTex, uvG).rgb;
        sigB = texture(inputTex, uvB).rgb;
    } else {
        sigR = vhsSignal(uvR, t);
        sigG = vhsSignal(uvG, t + 0.07);
        sigB = vhsSignal(uvB, t + 0.14);
    }

    // Reassemble with channel split
    vec3 col = vec3(sigR.r, sigG.g, sigB.b);

    // ── CRT scanline overlay ──
    float scanY = fract(uv.y * RENDERSIZE.y * 0.5);
    float crt   = 0.85 + 0.15 * smoothstep(0.0, 0.4, scanY);
    col *= crt;

    // ── Pixel drop-out (block noise) ──
    float dX = floor(uv.x * mix(4.0, 24.0, g));
    float dY = floor(uv.y * mix(3.0, 16.0, g));
    float dropout = rand(vec2(dX + dY * 7.0, floor(t * 10.0)));
    float dropMask = step(1.0 - g * 0.3, dropout);
    // Dropped pixels flash to colorA or black
    vec3 dropColor = (dropout > 0.5) ? colorA.rgb * 1.5 : vec3(0.0);
    col = mix(col, dropColor, dropMask);

    // ── HDR bright scan spikes driven by audio ──
    {
        float spikeY = fract(uv.y * scanDensity * 0.5 - t * 1.2 + audioBass * audioReact * 2.0);
        float spike = smoothstep(0.0, 0.015, spikeY) * smoothstep(0.04, 0.015, spikeY);
        // HDR spike: peaks at ~1.8
        col += colorC.rgb * spike * 1.8 * audioMod;
    }

    // ── Color cast drift ──
    vec3 cast = mix(colorA.rgb, colorB.rgb, 0.5 + 0.5 * sin(t * 0.4)) * 0.08;
    col += cast * g;

    // ── Bass-driven full-frame flash ──
    col += colorA.rgb * max(0.0, bassGate - 0.7) * 2.0;

    // ── Subtle vignette ──
    col *= 1.0 - 0.35 * dot(uv - 0.5, uv - 0.5) * 4.0;

    // Linear HDR output — host applies ACES
    gl_FragColor = vec4(col, 1.0);
}
