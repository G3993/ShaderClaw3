/*{
  "DESCRIPTION": "Amber Preservation — 2D overhead view of specimens suspended in warm amber resin with interference ring halos. HDR amber/crimson/gold palette.",
  "CREDIT": "ShaderClaw auto-improve v14",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "specimenCount", "LABEL": "Specimens",   "TYPE": "float", "DEFAULT": 7.0,  "MIN": 2.0, "MAX": 14.0 },
    { "NAME": "ringFreq",      "LABEL": "Ring Freq",   "TYPE": "float", "DEFAULT": 24.0, "MIN": 8.0, "MAX": 48.0 },
    { "NAME": "hdrPeak",       "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "amberDepth",    "LABEL": "Amber Depth", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioMod",      "LABEL": "Audio Mod",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
vec2  hash12(float n) { return vec2(hash11(n), hash11(n + 17.3)); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 pos = vec2(uv.x * aspect, uv.y);
    float t = TIME * 0.3;
    float audio = 1.0 + (audioLevel + audioBass * 0.4) * audioMod;

    // Amber resin background: deep warm brown-orange
    vec3 amberBase = vec3(0.28, 0.09, 0.01) * amberDepth;
    vec3 col = amberBase;

    // Accumulated interference from each specimen
    float totalRing = 0.0;
    vec3 totalCol = vec3(0.0);

    int N = int(clamp(specimenCount, 2.0, 14.0));
    for (int i = 0; i < 14; i++) {
        if (i >= N) break;
        float fi = float(i);

        vec2 center = (hash12(fi * 1.37) * 0.8 + 0.1) * vec2(aspect, 1.0);
        float dist = length(pos - center);

        // Slow pulse: each specimen pulses independently
        float phase = t * (0.4 + hash11(fi * 3.71) * 0.6) + fi * 2.1;
        float pulse = 0.5 + 0.5 * sin(phase);

        // Concentric interference rings emanating from inclusion
        float ring = sin(dist * ringFreq - t * 2.0 + fi * 1.7)
                   * exp(-dist * 8.0)
                   * (0.5 + 0.5 * pulse);

        // Specimen core: tiny black inclusion with warm amber halo
        float coreDist = smoothstep(0.04, 0.0, dist);
        float haloMask = exp(-dist * 20.0) * (1.0 - coreDist);

        // Hue: warm spectrum — deep amber (0.06) to crimson (0.02)
        float hue = 0.02 + hash11(fi * 7.73) * 0.10; // 0.02-0.12 red-orange
        vec3 specCol = hsv2rgb(vec3(hue, 1.0, 1.0));

        totalRing += ring;
        totalCol  += specCol * (haloMask + max(ring, 0.0) * 0.8);

        // Black inclusion core
        col = mix(col, vec3(0.0), coreDist * 0.95);
    }

    // Gold/amber ring highlights
    vec3 ringTint = mix(vec3(0.9, 0.45, 0.02), vec3(1.0, 0.72, 0.05), clamp(totalRing, 0.0, 1.0));
    float ringLum = clamp(totalRing, -1.0, 1.0);

    col += totalCol * 0.4;
    col += ringTint * max(ringLum, 0.0) * hdrPeak * audio;
    // White-hot at ring crests
    col += vec3(1.0, 0.85, 0.4) * max(ringLum * ringLum, 0.0) * hdrPeak * 0.6 * audio;
    // Dark trough: deep amber shadow
    col *= 1.0 - max(-ringLum, 0.0) * 0.6;

    gl_FragColor = vec4(col, 1.0);
}
