/*{
  "CATEGORIES": ["Generator", "Audio Reactive", "Geometric"],
  "DESCRIPTION": "Perfect rectangular constellation; every cell bound to one FFT bin, bass at left, treble at right, breathing in unison. Kraftwerk + Ryoji Ikeda data.matrix.",
  "INPUTS": [
    {"NAME":"cols","TYPE":"float","MIN":8.0,"MAX":96.0,"DEFAULT":48.0},
    {"NAME":"rows","TYPE":"float","MIN":4.0,"MAX":48.0,"DEFAULT":24.0},
    {"NAME":"cellRadius","TYPE":"float","MIN":0.05,"MAX":0.5,"DEFAULT":0.32},
    {"NAME":"jitter","TYPE":"float","MIN":0.0,"MAX":0.4,"DEFAULT":0.05},
    {"NAME":"decay","TYPE":"float","MIN":0.5,"MAX":0.99,"DEFAULT":0.9},
    {"NAME":"useTex","TYPE":"bool","DEFAULT":false},
    {"NAME":"lowColor","TYPE":"color","DEFAULT":[1.0,0.2,0.3,1.0]},
    {"NAME":"highColor","TYPE":"color","DEFAULT":[0.2,0.8,1.0,1.0]},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 grid = vec2(max(8.0, cols), max(4.0, rows));
    vec2 cId = floor(uv * grid);
    vec2 cUV = fract(uv * grid) - 0.5;

    // Each cell column = one FFT bin slice. Skip top 5% (mostly noise).
    float bin = (cId.x + 0.5) / grid.x * 0.95;
    float amp = texture(audioFFT, vec2(bin, 0.5)).r;

    // Approximate the temporal-decay smoothing without persistent buffers:
    // we mix the raw FFT value with a subtle per-cell idle wobble so quiet
    // bins still breathe rather than going dead. The `decay` knob here
    // governs how much of the wobble is mixed in — high decay → smooth.
    float wobble = 0.5 + 0.5 * sin(TIME * 1.5 + hash(cId) * 6.2832);
    amp = mix(amp, max(amp, wobble * 0.15), 1.0 - decay);

    // Per-cell phase jitter so dots don't all pulse on the same frame.
    vec2 jit = (vec2(hash(cId), hash(cId + 1.7)) - 0.5) * jitter * (amp + 0.2);

    // Cell radius modulated by bin amplitude — the dot grows with sound.
    float r = cellRadius * (0.3 + amp * 1.5 + audioLevel * 0.2);
    float dotMask = smoothstep(r, r * 0.85, length(cUV - jit));

    // Colour: spectrum gradient unless a texture mosaic is requested.
    vec3 base;
    if (useTex && IMG_SIZE_inputTex.x > 0.0) {
        // Each cell samples live video at its center → frequency-aware mosaic.
        base = texture(inputTex, (cId + 0.5) / grid).rgb;
    } else {
        base = mix(lowColor.rgb, highColor.rgb, cId.x / grid.x);
    }

    // Per-cell rotation hash for slight visual life on tall columns.
    float rotPhase = hash(cId + 7.7) * 6.2832;
    float twinkle = 0.85 + 0.15 * sin(TIME * 2.0 + rotPhase);

    vec3 col = base * dotMask * (amp + 0.1 + audioLevel * 0.05) * twinkle;

    // Base bloom — bass column 0 gets an audioBass boost, treble end gets audioHigh.
    if (cId.x < 1.0) col += lowColor.rgb * audioBass * 0.4 * dotMask;
    if (cId.x > grid.x - 2.0) col += highColor.rgb * audioHigh * 0.4 * dotMask;

    gl_FragColor = vec4(col, 1.0);
}
