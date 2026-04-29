/*{
  "CATEGORIES": ["Generator", "Audio Reactive", "Geometric"],
  "DESCRIPTION": "Sand on a vibrating plate. Audio frequencies sculpt classic Chladni nodal lines into living constellations. 1787 plate physics rendered as light.",
  "INPUTS": [
    {"NAME":"baseN","TYPE":"float","MIN":1.0,"MAX":12.0,"DEFAULT":3.0},
    {"NAME":"baseM","TYPE":"float","MIN":1.0,"MAX":12.0,"DEFAULT":5.0},
    {"NAME":"audioModeRange","TYPE":"float","MIN":0.0,"MAX":8.0,"DEFAULT":4.0},
    {"NAME":"lineSharpness","TYPE":"float","MIN":0.001,"MAX":0.1,"DEFAULT":0.02},
    {"NAME":"jitter","TYPE":"float","MIN":0.0,"MAX":0.05,"DEFAULT":0.005},
    {"NAME":"sandColor","TYPE":"color","DEFAULT":[0.95,0.88,0.7,1.0]},
    {"NAME":"plateColor","TYPE":"color","DEFAULT":[0.06,0.05,0.05,1.0]},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

#define PI 3.14159265

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Smooth audio-driven mode evolution. We avoid integer-only floor() jumps
    // by using float modes — the Chladni equation is well-defined for floats
    // and produces smooth pattern transitions instead of strobing mode-flips.
    // Slight TIME drift so the pattern keeps evolving even when audio is flat.
    float n = baseN + audioBass * audioModeRange + sin(TIME * 0.05) * 0.5;
    float m = baseM + audioHigh * audioModeRange + cos(TIME * 0.07) * 0.5;
    // Degenerate guard: when n == m, the equation collapses to zero everywhere.
    if (abs(n - m) < 0.5) m += 1.0;

    // Per-pixel jitter coupled to audioMid — sand vibrates more at louder mids.
    vec2 q = uv + (vec2(hash(uv), hash(uv + 1.3)) - 0.5) * jitter * (audioMid + 0.1);

    // The Chladni equation for a square plate clamped at edges:
    //   f(x,y) = sin(nπx)sin(mπy) − sin(mπx)sin(nπy)
    // Nodal lines are where f ≈ 0 — sand settles there.
    float f = sin(n * PI * q.x) * sin(m * PI * q.y)
            - sin(m * PI * q.x) * sin(n * PI * q.y);

    // Lines: sharp where |f| is small.
    float line = smoothstep(lineSharpness, 0.0, abs(f));

    // Plate texture — optional inputTex underneath the sand.
    vec3 plate = plateColor.rgb;
    if (IMG_SIZE_inputTex.x > 0.0) {
        plate = mix(plateColor.rgb, texture(inputTex, uv).rgb * 0.4, 0.5);
    }

    // Sand brightness pulses with audioLevel — louder = brighter sand.
    float bright = 0.5 + audioLevel * 1.2;
    vec3 col = mix(plate, sandColor.rgb * bright, line);

    // Soft halo around the nodal lines so they feel like piles, not strokes.
    float halo = smoothstep(lineSharpness * 4.0, 0.0, abs(f)) - line;
    col += sandColor.rgb * halo * 0.25 * bright;

    gl_FragColor = vec4(col, 1.0);
}
