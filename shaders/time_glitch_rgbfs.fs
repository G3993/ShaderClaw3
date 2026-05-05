/*{
  "DESCRIPTION": "CRT Meltdown — dying CRT monitor with hard horizontal glitch bars, RGB channel split, phosphor burn-in, sweeping sync bars, and audio-reactive glitch frequency. Fully self-contained generator, no input image required.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"glitchRate",    "LABEL":"Glitch Rate",    "TYPE":"float","DEFAULT":0.5, "MIN":0.1,"MAX":2.0},
    {"NAME":"rgbSplit",      "LABEL":"RGB Split",      "TYPE":"float","DEFAULT":0.008,"MIN":0.0,"MAX":0.03},
    {"NAME":"scanlineDepth", "LABEL":"Scanline Depth", "TYPE":"float","DEFAULT":0.5, "MIN":0.0,"MAX":1.0},
    {"NAME":"hdrPeak",       "LABEL":"HDR Peak",       "TYPE":"float","DEFAULT":2.0, "MIN":1.0,"MAX":3.0},
    {"NAME":"audioReact",    "LABEL":"Audio React",    "TYPE":"float","DEFAULT":0.7, "MIN":0.0,"MAX":2.0}
  ]
}*/

// ── hash helpers ───────────────────────────────────────────────────────────
float hash1(float n) {
    return fract(sin(n * 127.1) * 43758.5453);
}
float hash2(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// ── square-wave signal — hard horizontal bars at a given frequency ─────────
float signal(vec2 uv, float freq, float phase) {
    return step(0.5, fract(uv.x * freq + phase));
}

// ── generated CRT picture — three slightly-desynchronised square-wave grids ─
vec3 crtSignal(vec2 uv) {
    float t  = TIME * 2.0;
    // horizontal square-wave channels
    float rH = signal(uv, 40.0, t * 0.97);
    float gH = signal(uv, 37.0, t * 1.03 + 0.15);
    float bH = signal(uv, 43.0, t * 0.91 + 0.31);
    // cross them with vertical square waves (swap axes)
    float rV = signal(vec2(uv.y, uv.x), 22.0, t * 0.80);
    float gV = signal(vec2(uv.y, uv.x), 19.0, t * 0.85 + 0.20);
    float bV = signal(vec2(uv.y, uv.x), 25.0, t * 0.75 + 0.40);
    return vec3(rH * rV, gH * gV, bH * bV);
}

// ── main ───────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = isf_FragNormCoord; // 0..1

    float audio     = 1.0 + audioLevel * audioReact;
    float bassAudio = 1.0 + audioBass  * audioReact;
    float highAudio = 1.0 + audioHigh  * audioReact;

    // ── glitch bar logic ──────────────────────────────────────────────────
    // Quantise time to 8 fps * glitchRate (audio-reactive)
    float glitchTime   = floor(TIME * 8.0 * glitchRate * bassAudio);
    float barID        = floor(uv.y * 24.0);

    float glitchRand   = hash1(barID + glitchTime * 137.0);
    float isGlitchBar  = step(0.85, glitchRand); // ~15% of bars glitch

    // random horizontal offset for glitch bars
    float glitchOffset = (hash1(barID * 7.3 + glitchTime) - 0.5) * 0.15 * isGlitchBar;

    // RGB split amount boosted inside glitch bars
    float splitAmt     = rgbSplit * (1.0 + isGlitchBar * 4.0) * audio;

    // ── sample three RGB channels with independent horizontal offsets ─────
    vec2 uvBase = vec2(uv.x + glitchOffset, uv.y);
    vec2 uvR    = uvBase + vec2( splitAmt, 0.0);
    vec2 uvG    = uvBase;
    vec2 uvB    = uvBase - vec2( splitAmt, 0.0);

    vec3 sigR = crtSignal(uvR);
    vec3 sigG = crtSignal(uvG);
    vec3 sigB = crtSignal(uvB);

    // compose: R from right-shifted, G from centre, B from left-shifted
    vec3 col = vec3(sigR.r, sigG.g, sigB.b);

    // HDR-tinted channels — pure saturated primaries
    vec3 rTint = vec3(1.0, 0.0, 0.0) * 2.0;
    vec3 gTint = vec3(0.0, 1.0, 0.0) * 2.0;
    vec3 bTint = vec3(0.0, 0.3, 1.0) * 2.0;
    vec3 tinted = col.r * rTint + col.g * gTint + col.b * bTint;

    // phosphor white (HDR warm white) for non-glitch bars
    vec3  phosphorWhite = vec3(1.0, 1.0, 0.95) * 2.5;
    float whiteMix      = dot(col, vec3(0.333));
    vec3  phosphor      = phosphorWhite * whiteMix;

    // blend: non-glitch bars = phosphor, glitch bars = full RGB tinted HDR
    col = mix(phosphor, tinted * hdrPeak, isGlitchBar);

    // ── scanlines ─────────────────────────────────────────────────────────
    float lineF    = uv.y * RENDERSIZE.y * 0.5;
    float lineEdge = fract(lineF);
    float fwLine   = fwidth(lineF);
    float scanMask = smoothstep(0.3 - fwLine, 0.3 + fwLine, lineEdge);
    col *= mix(1.0, 1.0 - scanlineDepth * 0.72, 1.0 - scanMask);

    // ── phosphor burn-in — hot white HDR oval at centre ──────────────────
    vec2  burnUV = (uv - 0.5) * vec2(3.0, 5.0);
    float burnIn = exp(-dot(burnUV, burnUV) * 0.25) * 2.5 * hdrPeak;
    col += phosphorWhite * burnIn * 0.14;

    // ── sweeping sync bars (3 bars, full black, AA edges) ─────────────────
    float syncPos    = fract(TIME * 0.3 * glitchRate);
    float syncGap    = 1.0 / 3.0;
    float halfThick  = 0.012;
    float fwSync     = fwidth(uv.y);
    for (int i = 0; i < 3; i++) {
        float barCentre = fract(syncPos + float(i) * syncGap);
        float d        = abs(uv.y - barCentre);
        float syncMask = 1.0 - smoothstep(halfThick - fwSync, halfThick + fwSync, d);
        col *= 1.0 - syncMask; // zero = black sync bar
    }

    // ── hard black ink separator lines between glitch-bar zones ───────────
    float barEdgeF  = uv.y * 24.0;
    float barEdge   = fract(barEdgeF);
    float fwBar     = fwidth(barEdgeF);
    float inkMaskLo = smoothstep(0.05 - fwBar, 0.05 + fwBar, barEdge);
    float inkMaskHi = smoothstep(0.95 + fwBar, 0.95 - fwBar, barEdge);
    float inkLine   = inkMaskLo * inkMaskHi;
    // darken the separator only inside glitch bars (hard black edges)
    col *= mix(1.0, mix(0.0, 1.0, inkLine), isGlitchBar * 0.6);

    gl_FragColor = vec4(col, 1.0);
}
