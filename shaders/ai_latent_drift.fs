/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "AI hallucination after Refik Anadol's Machine Hallucinations (2019), Mario Klingemann's Memories of Passersby (2018), Tom White's Perception Engines — multi-octave fbm walked by three slow LFOs through a curl-noise domain warp, directionally blurred to that diffusion-model softness, palette-mapped through a wide muted hue arc with phantom feature pulses on bass. The latent space drifting between near-recognisable forms.",
  "INPUTS": [
    { "NAME": "latentSpeed", "LABEL": "Latent Walk Speed", "TYPE": "float", "MIN": 0.005, "MAX": 0.30, "DEFAULT": 0.06 },
    { "NAME": "lowScale", "LABEL": "Macro Scale", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 1.6 },
    { "NAME": "highScale", "LABEL": "Detail Scale", "TYPE": "float", "MIN": 2.0, "MAX": 14.0, "DEFAULT": 6.5 },
    { "NAME": "warpAmount", "LABEL": "Warp", "TYPE": "float", "MIN": 0.0, "MAX": 0.40, "DEFAULT": 0.16 },
    { "NAME": "warpScale", "LABEL": "Warp Scale", "TYPE": "float", "MIN": 1.0, "MAX": 8.0, "DEFAULT": 3.5 },
    { "NAME": "blurTaps", "LABEL": "Directional Blur Taps", "TYPE": "float", "MIN": 0.0, "MAX": 14.0, "DEFAULT": 5.0 },
    { "NAME": "blurStrength", "LABEL": "Blur Strength", "TYPE": "float", "MIN": 0.0, "MAX": 0.05, "DEFAULT": 0.012 },
    { "NAME": "saturation", "LABEL": "Saturation", "TYPE": "float", "MIN": 0.20, "MAX": 0.95, "DEFAULT": 0.52 },
    { "NAME": "phantomPulse", "LABEL": "Phantom Pulse", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35 },
    { "NAME": "phantomScale", "LABEL": "Phantom Scale", "TYPE": "float", "MIN": 4.0, "MAX": 30.0, "DEFAULT": 14.0 },
    { "NAME": "promptInfluence", "LABEL": "Tex Prompt Influence", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.10 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "paletteShift", "LABEL": "Palette Shift", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

float fbm(vec2 p) {
    float a = 0.5, s = 0.0;
    // 4 octaves — visual softness is dominated by the curl-noise warp,
    // not octave depth. Halving from 6 saves ~33% per fbm call.
    for (int i = 0; i < 4; i++) {
        s += a * vnoise(p);
        p = mat2(1.6, 1.2, -1.2, 1.6) * p;
        a *= 0.5;
    }
    return s;
}

vec2 curl(vec2 p) {
    float e = 0.01;
    float a = vnoise(p + vec2(0.0, e)) - vnoise(p - vec2(0.0, e));
    float b = vnoise(p + vec2(e, 0.0)) - vnoise(p - vec2(e, 0.0));
    return vec2(a, -b);
}

// Wide muted hue arc — cream → mauve → teal → soft orange. Anadol's
// Machine Hallucinations colour space lives here.
vec3 latentPalette(float t, float shift) {
    t = fract(t + shift);
    const vec3 P[5] = vec3[5](
        vec3(0.92, 0.86, 0.74),  // cream
        vec3(0.66, 0.48, 0.74),  // mauve
        vec3(0.34, 0.62, 0.66),  // teal
        vec3(0.92, 0.62, 0.42),  // soft orange
        vec3(0.86, 0.78, 0.60)   // back to cream
    );
    float ft = t * 4.0;
    int i  = int(floor(ft));
    int j  = (i + 1) % 5;
    float k  = smoothstep(0.0, 1.0, fract(ft));
    return mix(P[i], P[j], k);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Three LFO phases — give the latent walk three independent rates so
    // the macro form, mid detail, and curl direction don't synchronise.
    float ws = latentSpeed * (1.0 + audioBass * audioReact * 1.2);
    float L1 = TIME * ws;
    float L2 = TIME * ws * 1.31 + 17.3;
    float L3 = TIME * ws * 0.71 + 5.7;

    // Curl-noise domain warp
    vec2 P = vec2(uv.x * aspect, uv.y);
    vec2 cl = curl(P * warpScale + L3);
    vec2 wP = P + cl * warpAmount;

    // Directional blur — sample fbm field along the curl direction, N
    // taps. The diffusion-model softness comes from this — sharp edges
    // cannot survive the directional smear.
    int taps = int(clamp(blurTaps, 1.0, 14.0));
    vec2 bDir = normalize(cl + vec2(1e-5)) * blurStrength;
    float lowSum = 0.0, highSum = 0.0, wSum = 0.0;
    for (int i = 0; i < 14; i++) {
        if (i >= taps) break;
        float t = (float(i) - float(taps - 1) * 0.5) / float(taps);
        vec2 sP = wP + bDir * t * 12.0;
        float w = exp(-pow(t * 2.0, 2.0));
        lowSum  += fbm(sP * lowScale  + vec2(L1, -L1)) * w;
        highSum += fbm(sP * highScale + vec2(L2, L2 * 0.7)) * w;
        wSum    += w;
    }
    float lowField  = lowSum  / max(wSum, 1e-4);
    float highField = highSum / max(wSum, 1e-4);
    float field = lowField * 0.7 + highField * 0.3;

    // Palette mapping — colour gradient direction rotates over time so
    // the bias isn't permanently left-to-right; this is the slow
    // rotating colour band Anadol's installations exhibit.
    float ang = TIME * 0.05;
    float bias = uv.x * cos(ang) + uv.y * sin(ang);
    float t = field + bias * 0.20 + L1 * 0.5 + paletteShift;
    vec3 col = latentPalette(t, 0.0);

    // Saturation control — Anadol stays muted.
    float L = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(L), col,
              clamp(saturation * (0.85 + audioMid * audioReact * 0.2),
                    0.0, 1.0));

    // Phantom features — finer fbm at varying scale so phantom forms
    // grow and dissolve like emerging GAN concepts. Colour is taken
    // from the same palette so they integrate, not blot.
    if (phantomPulse > 0.0) {
        float scaleNow = phantomScale * (0.7 + 0.3 * sin(TIME * 0.10));
        float ph = fbm(P * scaleNow + L2 * 1.7) - 0.5;
        ph *= smoothstep(0.4, 0.0, abs(ph));
        vec3 phCol = latentPalette(t + 0.3, 0.0);
        col += phCol * ph
             * phantomPulse * (0.4 + audioHigh * audioReact * 1.3);
    }

    // Tex "prompt" — input bleeds in at low opacity, blurred along curl
    // so it loses recognisability the way a diffusion model abstracts
    // the initial latent.
    if (promptInfluence > 0.0 && IMG_SIZE_inputTex.x > 0.0) {
        vec3 src = vec3(0.0);
        for (int i = 0; i < 5; i++) {
            float t2 = (float(i) - 2.0) * 0.5;
            src += texture(inputTex, uv + cl * 0.06 * t2).rgb;
        }
        src /= 5.0;
        col = mix(col, src, promptInfluence * 0.9);
    }

    // Subtle film grain
    col += (hash21(uv * RENDERSIZE) - 0.5) * 0.012;

    // Audio luminance breath
    col *= 0.88 + audioLevel * audioReact * 0.18;

    gl_FragColor = vec4(col, 1.0);
}
