/*{
  "DESCRIPTION": "Ink Drop Diffusion — 2D simulation of colored ink drops expanding in still water. Domain-warped ripple rings with deep cobalt/violet palette and black ink silhouettes.",
  "CREDIT": "ShaderClaw auto-improve v13",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "dropCount",  "LABEL": "Drop Count",  "TYPE": "float", "DEFAULT": 6.0,  "MIN": 1.0, "MAX": 12.0 },
    { "NAME": "ringSpeed",  "LABEL": "Ring Speed",  "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "inkDark",    "LABEL": "Ink Darkness","TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "rippleFreq", "LABEL": "Ripple Freq", "TYPE": "float", "DEFAULT": 18.0, "MIN": 4.0, "MAX": 40.0 },
    { "NAME": "audioMod",   "LABEL": "Audio Mod",   "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
vec2  hash21(float n) { return vec2(hash11(n), hash11(n + 17.3)); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Smooth noise for domain warp
float hash21f(vec2 p) {
    p = fract(p * vec2(127.1, 311.7));
    p += dot(p, p + 33.31);
    return fract(p.x * p.y);
}
float snoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21f(i), hash21f(i+vec2(1,0)), u.x),
               mix(hash21f(i+vec2(0,1)), hash21f(i+vec2(1,1)), u.x), u.y) * 2.0 - 1.0;
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 pos = vec2(uv.x * aspect, uv.y);
    float t = TIME * ringSpeed;
    float audio = 1.0 + (audioLevel + audioBass * 0.4) * audioMod;

    // Accumulate ripple contributions
    float ripple = 0.0;
    float totalInk = 0.0;
    vec3 inkColor = vec3(0.0);

    int N = int(clamp(dropCount, 1.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);

        vec2 center = vec2(hash11(fi * 1.37) * aspect, hash11(fi * 2.71));
        float birthT = hash11(fi * 5.13) * 8.0;
        float age = mod(t + birthT, 10.0);
        float radius = age * 0.08 * (0.5 + hash11(fi * 3.1) * 0.5);

        // Domain warp for organic non-circular ripples
        float warpAmt = 0.015 * snoise(pos * 6.0 + t * 0.3);
        vec2 p = pos + warpAmt;
        float dist = length(p - center);

        // Ripple ring: peaks at radius, decays outward
        float ring = sin((dist - radius) * rippleFreq - t * 1.5)
                   * exp(-max(dist - radius, 0.0) * 12.0)
                   * exp(-max(radius - dist, 0.0) * 8.0)
                   * exp(-age * 0.2);

        // Ink hue: deep blue to violet spectrum
        float hue = 0.55 + hash11(fi * 7.7) * 0.25;
        vec3 dropCol = hsv2rgb(vec3(hue, 1.0, 1.0));

        float inkMask = exp(-dist * 4.0) * exp(-age * 0.3);
        ripple    += ring * 0.5;
        inkColor  += dropCol * inkMask;
        totalInk  += inkMask;
    }

    // Normalize ink color
    vec3 baseInk = totalInk > 0.001 ? inkColor / totalInk : vec3(0.02, 0.03, 0.15);

    // Background: still black water
    vec3 col = vec3(0.01, 0.005, 0.02);

    // White-hot crest of ripple wave
    float crestMask = max(ripple, 0.0);
    col += baseInk * crestMask * hdrPeak * audio;
    col += vec3(1.0) * crestMask * crestMask * hdrPeak * 0.5 * audio;

    // Ink darkness in trough (negative ripple)
    float troughMask = max(-ripple * inkDark, 0.0);
    col = mix(col, vec3(0.0), troughMask * 0.9);

    // Diffuse ink haze
    float haze = totalInk * 0.15;
    col += baseInk * haze;

    gl_FragColor = vec4(col, 1.0);
}
