/*{
  "DESCRIPTION": "Finding Color — a self-eroding feedback landscape keeps rewriting itself while dancing orbs, each tied to its own frequency band, bob across the field injecting fresh terrain. The relief is lit and painted from three editable colors; bass feeds the orbs, mids drive erosion, highs shimmer the palette.",
  "CREDIT": "Erosion feedback sim + shading from shadertoy Xsd3DB lineage, ShaderClaw audio port with dancing orbs",
  "CATEGORIES": [
    "Generator"
  ],
  "INPUTS": [
    {
      "NAME": "speed",
      "LABEL": "Speed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 3.0
    },
    {
      "NAME": "audioReact",
      "LABEL": "Audio React",
      "TYPE": "float",
      "GROUP": "Audio Reactivity",
      "DEFAULT": 0.6,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "colorA",
      "LABEL": "Color A",
      "TYPE": "color",
      "DEFAULT": [0.9, 0.3, 0.5, 1.0]
    },
    {
      "NAME": "colorB",
      "LABEL": "Color B",
      "TYPE": "color",
      "DEFAULT": [0.2, 0.7, 1.0, 1.0]
    },
    {
      "NAME": "colorC",
      "LABEL": "Color C",
      "TYPE": "color",
      "DEFAULT": [1.0, 0.85, 0.3, 1.0]
    }
  ],
  "PASSES": [
    {
      "TARGET": "valBuf",
      "PERSISTENT": true
    },
    {}
  ]
}*/

#define PI 3.1415927
#define N_ORBS 5
#define VAL_MAX 40.0

float knee(float x, float lo, float hi) { return smoothstep(lo, hi, x); }
float fftLog(float t) { return texture2D(audioFFT, vec2(pow(t, 2.2) * 0.5, 0.5)).r; }

// value field is unbounded in the original; pack 0..VAL_MAX 16-bit in rg
vec4 encV(float v) {
    float e = clamp(v / VAL_MAX, 0.0, 1.0) * 255.0;
    return vec4(floor(e) / 255.0, fract(e), 0.0, 1.0);
}
float decV(vec4 t) { return (t.r + t.g / 255.0) * VAL_MAX; }

float rnd(vec2 co) {
    return fract(sin(dot(co, vec2(12.9898, 78.233)) + fract(TIME) * 7.13) * 43758.5453);
}

float getVal(vec2 offsetPx) {
    vec2 uv = (gl_FragCoord.xy + offsetPx) / RENDERSIZE.xy;
    return decV(texture2D(valBuf, uv));
}

// each orb dances on its own frequency band
vec2 orbPos(int i, float T) {
    float fi = float(i);
    float band = pow(knee(fftLog(fi / float(N_ORBS)), 0.03, 0.7), 1.3);
    float a = T * (0.3 + 0.13 * fi) + fi * 2.4;
    float rad = 0.22 + 0.13 * fi * 0.25 + 0.18 * band * audioReact;
    vec2 c = vec2(0.5) + rad * vec2(cos(a), sin(a * (1.0 + 0.21 * fi)));
    c.y += 0.06 * band * sin(T * 6.0 + fi);   // bob on the beat
    return c * RENDERSIZE.xy;
}

vec4 passSim() {
    float T = TIME * speed;
    float ar = audioReact;
    float bassP = pow(knee(audioBass, 0.05, 0.85), 1.6);
    float midP  = pow(knee(audioMid,  0.08, 0.90), 1.3);

    float val = getVal(vec2(0.0));

    // erosion (mids stir it) — calmer than the original so audio reads through
    val += rnd(gl_FragCoord.xy) * val * 0.08 * mix(1.0, 0.5 + 1.6 * midP, ar);

    val = getVal(vec2(
        sin(getVal(vec2(val, 0.0)) - getVal(vec2(-val, 0.0)) + PI) * val * 0.4,
        cos(getVal(vec2(0.0, -val)) - getVal(vec2(0.0, val)) - PI * 0.5) * val * 0.4));

    val *= 1.0001;

    // dancing orbs inject terrain; bass feeds them
    for (int i = 0; i < N_ORBS; i++) {
        float d = length(orbPos(i, T) - gl_FragCoord.xy);
        val += smoothstep(RENDERSIZE.y / 10.0, 0.5, d)
             * (0.35 + ar * 2.4 * bassP);
    }

    if (FRAMEINDEX < 2) {
        val = rnd(gl_FragCoord.xy) * length(RENDERSIZE.xy) / 100.0
            + smoothstep(length(RENDERSIZE.xy) / 2.0, 0.5,
                         length(RENDERSIZE.xy * 0.5 - gl_FragCoord.xy)) * 25.0;
    }
    return encV(val);
}

vec4 passImage() {
    float T = TIME * speed;
    float ar = audioReact;
    float highP = pow(knee(audioHigh, 0.10, 0.90), 1.2);
    float levelP = knee(audioLevel, 0.05, 0.90);

    vec2 q = gl_FragCoord.xy / RENDERSIZE.xy;
    float val = decV(texture2D(valBuf, q));

    // three editable colors mixed by the terrain phase (highs spin the mix)
    float ph = val * 1.7 + T * 0.2 + ar * 1.5 * highP;
    float w1 = 0.5 + 0.5 * sin(ph);
    float w2 = 0.5 + 0.5 * sin(ph + 2.094);
    float w3 = 0.5 + 0.5 * sin(ph + 4.188);
    vec3 tint = (colorA.rgb * w1 + colorB.rgb * w2 + colorC.rgb * w3) / max(w1 + w2 + w3, 1e-3);
    vec4 color = vec4(tint, 1.0) * pow(clamp(vec4(cos(val), 0.8, sin(val), 1.0) * 0.5 + 0.5, 0.0, 1.0), vec4(0.5));

    vec3 e = vec3(1.0 / RENDERSIZE.xy, 0.0);
    float p10 = decV(texture2D(valBuf, q - e.zy));
    float p01 = decV(texture2D(valBuf, q - e.xz));
    float p21 = decV(texture2D(valBuf, q + e.xz));
    float p12 = decV(texture2D(valBuf, q + e.zy));
    vec3 grad = normalize(vec3(p21 - p01, p12 - p10, 1.0));
    vec3 light = normalize(vec3(0.2, -0.25, 0.7));
    float diffuse = dot(grad, light);
    float spec = pow(max(0.0, -reflect(light, grad).z), 32.0);

    vec4 col = (color * diffuse) + spec;

    // glowing dancing orbs on top
    for (int i = 0; i < N_ORBS; i++) {
        float band = pow(knee(fftLog(float(i) / float(N_ORBS)), 0.03, 0.7), 1.3);
        float d = length(orbPos(i, T) - gl_FragCoord.xy) / RENDERSIZE.y;
        vec3 oc = (i == 0 || i == 3) ? colorA.rgb : ((i == 1 || i == 4) ? colorB.rgb : colorC.rgb);
        col.rgb += oc * exp(-d * d * 900.0) * (0.5 + 1.6 * band * ar);
        col.rgb += oc * exp(-d * 14.0) * 0.10 * (0.5 + band);
    }

    col.rgb *= 1.0 + ar * 0.3 * levelP;
    return vec4(col.rgb, 1.0);
}

void main() {
    if (PASSINDEX == 0) gl_FragColor = passSim();
    else                gl_FragColor = passImage();
}
