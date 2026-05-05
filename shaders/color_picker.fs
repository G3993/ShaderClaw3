/*{
  "DESCRIPTION": "Acrylic Pour Canvas — domain-warped fluid acrylic bands with 5-color saturated palette. Standalone generator, no input required.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "pourSpeed",  "LABEL": "Flow Speed",  "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "warpAmt",    "LABEL": "Warp Amount", "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.0,  "MAX": 6.0 },
    { "NAME": "bandFreq",   "LABEL": "Band Count",  "TYPE": "float", "DEFAULT": 8.0,  "MIN": 2.0,  "MAX": 24.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0,  "MAX": 4.0 },
    { "NAME": "audioReact", "LABEL": "Audio",       "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * noise(p);
        p = p * 2.1 + vec2(1.7, 9.2);
        a *= 0.5;
    }
    return v;
}

// 5-color fully-saturated acrylic palette: ultramarine, crimson, chrome yellow, sap green, white-hot
vec3 pourColor(float t) {
    t = fract(t);
    float s = t * 5.0;
    float f = fract(s);
    int i = int(floor(s));
    if (i == 0) return mix(vec3(0.04, 0.06, 1.0),  vec3(1.0,  0.02, 0.08), f);
    if (i == 1) return mix(vec3(1.0,  0.02, 0.08), vec3(1.0,  0.85, 0.0),  f);
    if (i == 2) return mix(vec3(1.0,  0.85, 0.0),  vec3(0.0,  0.9,  0.12), f);
    if (i == 3) return mix(vec3(0.0,  0.9,  0.12), vec3(1.0,  1.0,  1.0),  f);
    return           mix(vec3(1.0,  1.0,  1.0),  vec3(0.04, 0.06, 1.0),  f);
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioReact + audioBass * audioReact * 0.5;
    float t = TIME * pourSpeed;

    // Double domain warp (fluid pour simulation)
    float w = warpAmt * audio;
    vec2 q = vec2(fbm(uv              + t * 0.31),
                  fbm(uv + vec2(5.2, 1.3) + t * 0.23));
    vec2 r = vec2(fbm(uv + w * q + vec2(1.7, 9.2) + t * 0.17),
                  fbm(uv + w * q + vec2(8.3, 2.8) + t * 0.13));
    float f = fbm(uv + w * r);

    // Concentric band distance + warp offset
    float dist = length(uv) * 0.6 + f * 0.5;
    float band = fract(dist * bandFreq * 0.4 + t * 0.3);

    vec3 col = pourColor(band) * hdrPeak;

    // Black ink veins at band boundaries (fwidth AA)
    float fw = fwidth(band);
    float ink = 1.0 - smoothstep(0.0, 3.5 * fw, min(band, 1.0 - band));
    col = mix(col, vec3(0.0), ink * 0.92);

    // Subtle HDR hot-spot at canvas center
    float ctr = exp(-length(uv) * 1.4);
    col += pourColor(t * 0.3) * ctr * hdrPeak * 0.4;

    gl_FragColor = vec4(col, 1.0);
}
