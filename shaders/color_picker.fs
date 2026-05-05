/*{
  "DESCRIPTION": "Neon Ink Clouds — domain-warped FBM turbulence with fully saturated neon palette and black ink edges",
  "CREDIT": "ShaderClaw auto-improve v15",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "cloudScale",   "LABEL": "Cloud Scale",  "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 8.0 },
    { "NAME": "warpStrength", "LABEL": "Warp",         "TYPE": "float", "DEFAULT": 1.4,  "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "hueShift",     "LABEL": "Hue Rotate",   "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "hdrPeak",      "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "inkEdge",      "LABEL": "Ink Edges",    "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "audioMod",     "LABEL": "Audio Mod",    "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

float hash21(vec2 p) {
    p = fract(p * vec2(127.1, 311.7));
    p += dot(p, p + 33.31);
    return fract(p.x * p.y);
}

float noise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21(i),           hash21(i + vec2(1,0)), u.x),
               mix(hash21(i + vec2(0,1)), hash21(i + vec2(1,1)), u.x), u.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * noise(p);
        p = p * 2.03 + vec2(5.31, 1.73);
        a *= 0.5;
    }
    return v;
}

vec3 neonPalette(float t) {
    // Cosine palette: magenta → orange → gold → lime → cyan → violet, fully saturated
    vec3 a = vec3(0.5);
    vec3 b = vec3(0.5);
    vec3 c = vec3(1.0, 1.0, 1.0);
    vec3 d = vec3(0.0, 0.33, 0.67);
    return a + b * cos(6.28318 * (c * t + d));
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0) * cloudScale;

    float t = TIME * 0.10;
    float audio = 1.0 + (audioLevel + audioBass * 0.3) * audioMod;

    // 3-layer nested domain warp
    vec2 q = vec2(fbm(p + t * 0.13),
                  fbm(p + vec2(5.2, 1.3) + t * 0.09));
    vec2 r = vec2(fbm(p + warpStrength * q + vec2(1.7, 9.2) + t * 0.11),
                  fbm(p + warpStrength * q + vec2(8.3, 2.8) + t * 0.07));
    vec2 s = vec2(fbm(p + warpStrength * 1.6 * r + vec2(3.1, 6.4) + t * 0.08),
                  fbm(p + warpStrength * 1.6 * r + vec2(7.1, 1.9) + t * 0.06));

    float f = fbm(p + warpStrength * 2.0 * s + t * 0.05);

    // Iso-contour ink edges via fwidth
    float fw = fwidth(f);
    float bandF = fract(f * 5.0);
    float edge = smoothstep(fw * inkEdge * 20.0, 0.0, abs(bandF - 0.5) - 0.15);

    // Neon color from palette
    float hue = fract(f * 1.3 + hueShift + t * 0.05);
    vec3 col = neonPalette(hue) * hdrPeak * audio;

    // Black ink at contour edges
    col *= (1.0 - edge * 0.98);

    // Deep void at near-zero f
    float bgMask = smoothstep(0.0, 0.25, f);
    col = mix(vec3(0.01, 0.0, 0.05), col, bgMask);

    gl_FragColor = vec4(col, 1.0);
}
