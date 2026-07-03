/*{
  "DESCRIPTION": "Pulse Field — a frequency-decomposed cell grid: every cell listens to its own slice of the spectrum, kicks ripple shockwaves across the field, highs sparkle the edges. World-class audio reactivity per the house playbook.",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "CREDIT": "Etherea",
  "INPUTS": [
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "speed",      "LABEL": "Speed",       "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "cellsX",     "LABEL": "Columns",     "TYPE": "float", "DEFAULT": 16.0, "MIN": 6.0, "MAX": 28.0 },
    { "NAME": "glow",       "LABEL": "Glow",        "TYPE": "float", "DEFAULT": 0.9,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "ringPower",  "LABEL": "Kick Rings",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// ── Pulse Field ──────────────────────────────────────────────
// Playbook techniques: per-band spatial separation (each cell owns
// a stable FFT band + phase lag), kick-transient shockwave rings
// (audioBeatPulse as inverse clock), palette anchors, idle floor.

float hash11(float p) { return fract(sin(p * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float knee(float x, float lo, float hi) { return smoothstep(lo, hi, x); }

// log-frequency FFT lookup — musical energy lives in the low bins
float fftLog(float t) {
    return texture2D(audioFFT, vec2(pow(clamp(t, 0.0, 1.0), 2.2) * 0.5, 0.5)).r;
}

void main() {
    vec2 res = RENDERSIZE.xy;
    vec2 uv = gl_FragCoord.xy / res;
    vec2 p = (uv - 0.5) * vec2(res.x / res.y, 1.0);

    float amt   = clamp(audioReact, 0.0, 2.0);
    float t     = TIME * speed;

    // Conditioned drivers (soft knees + floors — alive in silence)
    float bassP  = pow(knee(audioBass, 0.05, 0.85), 1.6) * amt;
    float highP  = pow(knee(audioHigh, 0.10, 0.90), 1.2) * amt;
    float beatP  = audioBeatPulse * audioBeatPulse * amt;   // decaying accent
    float drive  = 0.25 + 0.75 * knee(audioEnergy, 0.05, 0.9) * min(amt, 1.0);

    // ---- cell grid: frequency -> space -------------------------------------
    float nx = floor(cellsX);
    float ny = floor(nx * res.y / res.x + 0.5);
    // slow autonomous drift — the field breathes even in silence
    vec2 drift = 0.35 * vec2(sin(t * 0.26 + uv.y * 2.2), cos(t * 0.19 + uv.x * 2.6));
    vec2 gridUv = uv * vec2(nx, ny) + drift;
    vec2 cellId = floor(gridUv);
    vec2 f = fract(gridUv) - 0.5;

    float h  = hash21(cellId);                 // stable identity
    float h2 = hash21(cellId + 71.3);

    // Each cell owns a band; center cells listen low, edge cells high
    float centerBias = length((cellId + 0.5) / vec2(nx, ny) - 0.5) * 1.4;
    float band = fftLog(clamp(h * 0.6 + centerBias * 0.5, 0.0, 1.0));

    // Per-cell phase lag — responses cascade, never in lockstep
    float lag = 0.5 + 0.5 * sin(t * 2.0 + h * 6.2831);
    float e = pow(band, 1.4) * mix(0.7, 1.0, lag) * amt;

    // Idle breath so the field lives in silence (independent of audio)
    float breath = 0.5 + 0.5 * sin(t * 0.8 + h * 6.2831 + cellId.x * 0.35);
    e = max(e, 0.16 + 0.18 * breath * (0.5 + 0.5 * drive));

    // ---- kick shockwave ring (event with finite life) -----------------------
    float d = length(p);
    float age = 1.0 - audioBeatPulse;                 // 0 at beat -> 1 as it fades
    float radius = mix(0.06, 1.35, age);
    float width  = mix(0.03, 0.16, age);
    float ring = exp(-pow((d - radius) / width, 2.0)) * beatP * ringPower;
    ring *= mix(0.4, 1.0, pow(knee(audioPunch, 0.05, 0.9), 1.5));

    // ring lifts the cells it passes through
    e += ring * 1.6;

    // ---- cell shape ---------------------------------------------------------
    float cellSize = 0.26 + 0.30 * e + 0.06 * bassP;   // bass = small global lift
    float shape = smoothstep(cellSize, cellSize - 0.14, length(f));
    // soft halo doubles the coverage without flattening the grid
    float halo = smoothstep(cellSize + 0.22, cellSize, length(f)) * 0.35;

    // sparse sparkle on highs — only "bright" cells
    float spark = step(0.82, h2) * highP * (0.5 + 0.5 * sin(t * 9.0 + h2 * 40.0));

    // ---- palette (anchors steered by spectral character; h adds variety) ----
    float tone = clamp(0.18 + e * 1.15 + 0.15 * audioBrightness + 0.22 * (h - 0.5), 0.0, 1.0);
    vec3 col = (tone < 0.5)
        ? mix(audioPalShadow, audioPalMid, tone * 2.0)
        : mix(audioPalMid, audioPalHigh, tone * 2.0 - 1.0);
    col = mix(col, audioPalAccent, clamp(ring + spark + 0.25 * step(0.9, h2), 0.0, 1.0) * 0.7);

    // subtle vignette + background drift so black isn't dead
    float bgGlow = 0.09 * (0.4 + 0.6 * drive) * (1.0 - d * 0.8)
                 * (0.8 + 0.2 * sin(t * 0.5 + p.x * 2.0) * sin(t * 0.37 + p.y * 2.4));
    vec3 outCol = col * (shape + halo) * (0.85 + glow * (e + spark))
                + mix(audioPalShadow, audioPalMid, 0.35) * bgGlow * 6.0;
    outCol += audioPalAccent * ring * 0.5;

    gl_FragColor = vec4(outCol, 1.0);
}
