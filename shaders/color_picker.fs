/*{
  "DESCRIPTION": "Neon Mandala — 8-fold radial kaleidoscope, fully saturated 4-color HDR petals on void black",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "folds",      "LABEL": "Symmetry",      "TYPE": "float", "DEFAULT": 8.0,  "MIN": 3.0,  "MAX": 16.0 },
    { "NAME": "zoom",       "LABEL": "Zoom",          "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3,  "MAX": 3.0 },
    { "NAME": "rotSpeed",   "LABEL": "Rotate Speed",  "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "petalDepth", "LABEL": "Petal Depth",   "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0,  "MAX": 8.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",      "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0 },
    { "NAME": "audioReact", "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

const float PI  = 3.14159265359;
const float TAU = 6.28318530718;

// 4 fully-saturated hues: magenta → cyan → yellow → violet
vec3 neonPalette(float t) {
    t = fract(t) * 4.0;
    vec3 c0 = vec3(1.0, 0.0, 1.0);
    vec3 c1 = vec3(0.0, 1.0, 1.0);
    vec3 c2 = vec3(1.0, 1.0, 0.0);
    vec3 c3 = vec3(0.5, 0.0, 1.0);
    if (t < 1.0) return mix(c0, c1, t);
    if (t < 2.0) return mix(c1, c2, t - 1.0);
    if (t < 3.0) return mix(c2, c3, t - 2.0);
    return mix(c3, c0, t - 3.0);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv /= max(zoom, 0.01);

    float t     = TIME * rotSpeed;
    float audio = 1.0 + (audioLevel * 0.5 + audioBass * 0.8) * audioReact;

    float r = length(uv);
    float a = atan(uv.y, uv.x);

    // Kaleidoscope fold: mirror angle into one segment, then mirror again
    float seg   = TAU / folds;
    float aFold = mod(a + t, seg);
    if (aFold > seg * 0.5) aFold = seg - aFold;

    vec3 col = vec3(0.0);

    int N = int(clamp(petalDepth, 1.0, 8.0));
    for (int layer = 0; layer < 8; layer++) {
        if (layer >= N) break;
        float fl  = float(layer);
        float lr  = r * pow(1.4, fl);
        float hue = fract(fl * 0.16 + t * 0.05 + r * 0.1);

        // Radial oscillator x angular oscillator = petal mask
        float radOsc = 0.5 + 0.5 * cos(lr * (5.0 + fl * 2.0) - t * (1.0 + fl * 0.25));
        float angOsc = 0.5 + 0.5 * cos(aFold * folds * (1.0 + fl * 0.4) * 2.0);
        float petal  = radOsc * angOsc;

        // fwidth AA on the petal edge
        float fw   = max(fwidth(petal), 0.001);
        float mask = smoothstep(0.28 - fw, 0.28 + fw, petal);

        float boost = hdrPeak * audio * pow(0.80, fl);
        col += mask * neonPalette(hue) * boost;
    }

    // Black ink center — anchors contrast
    float ink = 1.0 - smoothstep(0.04, 0.14, r);
    col *= (1.0 - ink * 0.95);

    // White-hot core spark
    col += vec3(3.0, 2.5, 2.0) * exp(-r * 20.0) * audio;

    gl_FragColor = vec4(col, 1.0);
}
