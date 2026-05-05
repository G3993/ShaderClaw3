/*{
  "DESCRIPTION": "Abstract Turbulence — standalone 2D domain-warped FBM colour field in an expressionist paint style. 4-colour saturated palette. NEW ANGLE: 2D fluid swirl vs prior 3D lava-impasto surface.",
  "CATEGORIES": ["Generator", "Abstract"],
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "INPUTS": [
    {"NAME":"flowSpeed",  "LABEL":"Flow Speed",  "TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":0.35},
    {"NAME":"warpScale",  "LABEL":"Warp Scale",  "TYPE":"float","MIN":1.0,"MAX":8.0,"DEFAULT":3.5},
    {"NAME":"warpDepth",  "LABEL":"Warp Depth",  "TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":0.8},
    {"NAME":"hdrPeak",    "LABEL":"HDR Peak",    "TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.2},
    {"NAME":"edgeInk",    "LABEL":"Ink Edges",   "TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0},
    {"NAME":"audioReact", "LABEL":"Audio",       "TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

vec3 paintPal(float t) {
    t = clamp(t, 0.0, 1.0);
    vec3 c0 = vec3(2.2, 0.05, 0.0);   // cadmium red
    vec3 c1 = vec3(0.0, 0.2,  2.4);   // cobalt blue
    vec3 c2 = vec3(0.0, 1.8,  0.5);   // viridian
    vec3 c3 = vec3(2.5, 2.0,  0.0);   // chrome yellow
    float s = t * 4.0;
    if (s < 1.0) return mix(c0, c1, s);
    if (s < 2.0) return mix(c1, c2, s - 1.0);
    if (s < 3.0) return mix(c2, c3, s - 2.0);
    return         mix(c3, c0, s - 3.0);
}

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash(i),           hash(i + vec2(1,0)), u.x),
               mix(hash(i + vec2(0,1)), hash(i + vec2(1,1)), u.x), u.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) { v += a * vnoise(p); p *= 2.03; a *= 0.52; }
    return v;
}

vec2 curlWarp(vec2 p, float t) {
    vec2 q = vec2(fbm(p + vec2(0.0, t)),
                  fbm(p + vec2(5.2, t + 1.3)));
    return vec2(fbm(p + warpDepth * q + vec2(1.7, 9.2) + t * 0.15),
                fbm(p + warpDepth * q + vec2(8.3, 2.8) + t * 0.12));
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = vec2(uv.x * aspect, uv.y) * warpScale;

    float t = TIME * flowSpeed;
    float audio = 1.0 + audioLevel * audioReact * 0.3 + audioBass * audioReact * 0.25;

    vec2 w = curlWarp(p, t);
    float field = fbm(p + w * 2.0 + t * 0.08);

    float colIdx = field + TIME * 0.04 + audioMid * audioReact * 0.1;
    vec3 col = paintPal(fract(colIdx)) * hdrPeak * audio;

    float fw = fwidth(field) * edgeInk * 22.0;
    float inkMask = smoothstep(0.0, fw, abs(fract(field * 3.5) - 0.5) * 2.0 - 0.6);
    col *= 0.08 + 0.92 * inkMask;

    float fw2 = fwidth(field) * edgeInk * 10.0;
    float inkMask2 = smoothstep(0.0, fw2, abs(fract(field * 7.0) - 0.5) * 2.0 - 0.7);
    col *= 0.3 + 0.7 * inkMask2;

    gl_FragColor = vec4(col, 1.0);
}
