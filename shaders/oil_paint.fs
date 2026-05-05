/*{
    "DESCRIPTION": "Sumi-e Ink Wash — converts input to Japanese ink painting with Sobel edge strokes and cinnabar seals",
    "CREDIT": "ShaderClaw auto-improve v2",
    "ISFVSN": "2",
    "CATEGORIES": ["Effect"],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "inkStrength",
            "TYPE": "float",
            "DEFAULT": 2.2,
            "MIN": 0.5,
            "MAX": 5.0
        },
        {
            "NAME": "washBlur",
            "TYPE": "float",
            "DEFAULT": 3.5,
            "MIN": 1.0,
            "MAX": 8.0
        },
        {
            "NAME": "sealChance",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "audioReact",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MIN": 0.0,
            "MAX": 2.0
        }
    ],
    "PASSES": [
        { "TARGET": "blurBuf" },
        {}
    ]
}*/

precision highp float;

// ---- Palette ----
const vec3 PAPER      = vec3(2.0,  1.85, 1.60);   // HDR warm paper
const vec3 LIGHT_INK  = vec3(0.45, 0.40, 0.35);   // pale wash
const vec3 DARK_INK   = vec3(0.03, 0.025,0.02);   // deep sumi
const vec3 CINNABAR   = vec3(1.8,  0.05, 0.0);    // HDR red seal accent

void main() {
    vec2 uv = isf_FragNormCoord;

#if defined(PASSINDEX) && PASSINDEX == 0
    // ---- Pass 0: 9x9 Gaussian blur of inputImage ----
    vec2 texel = 1.0 / RENDERSIZE;
    float radius = washBlur;

    // Gaussian kernel weights for offsets -4..+4
    float weights[9];
    weights[0] = 0.0162;
    weights[1] = 0.0540;
    weights[2] = 0.1216;
    weights[3] = 0.1945;
    weights[4] = 0.2270;
    weights[5] = 0.1945;
    weights[6] = 0.1216;
    weights[7] = 0.0540;
    weights[8] = 0.0162;

    vec4 blurred = vec4(0.0);
    for (int i = 0; i < 9; i++) {
        float offset = float(i - 4) * radius;
        vec2 uvX = uv + vec2(offset * texel.x, 0.0);
        vec2 uvY = uv + vec2(0.0, offset * texel.y);
        blurred += IMG_NORM_PIXEL(inputImage, clamp(uvX, 0.0, 1.0)) * weights[i] * 0.5;
        blurred += IMG_NORM_PIXEL(inputImage, clamp(uvY, 0.0, 1.0)) * weights[i] * 0.5;
    }
    gl_FragColor = blurred;

#else
    // ---- Pass 1: Sumi-e tone mapping + Sobel edges + cinnabar seals ----

    // Audio modulator
    float audio = 0.5 + 0.5 * audioBass * audioReact;

    // Sample blurred buffer
    vec4 blurred = IMG_NORM_PIXEL(blurBuf, uv);
    float lum = dot(blurred.rgb, vec3(0.299, 0.587, 0.114));

    // ---- Sobel edge detection on blurBuf (4-neighbor) ----
    vec2 texel = 1.0 / RENDERSIZE;
    float lumR  = dot(IMG_NORM_PIXEL(blurBuf, uv + vec2( texel.x, 0.0)).rgb, vec3(0.299, 0.587, 0.114));
    float lumL  = dot(IMG_NORM_PIXEL(blurBuf, uv + vec2(-texel.x, 0.0)).rgb, vec3(0.299, 0.587, 0.114));
    float lumU  = dot(IMG_NORM_PIXEL(blurBuf, uv + vec2(0.0,  texel.y)).rgb, vec3(0.299, 0.587, 0.114));
    float lumD  = dot(IMG_NORM_PIXEL(blurBuf, uv + vec2(0.0, -texel.y)).rgb, vec3(0.299, 0.587, 0.114));
    float gx = lumR - lumL;
    float gy = lumU - lumD;
    float edge = clamp(sqrt(gx * gx + gy * gy) * inkStrength * 3.0, 0.0, 1.0);

    // ---- Ink tone mapping ----
    float inkConc = pow(1.0 - lum, inkStrength * 0.5 * audio);
    inkConc = clamp(inkConc, 0.0, 1.0);

    vec3 tonedColor;
    if (inkConc < 0.35) {
        tonedColor = mix(PAPER, LIGHT_INK, inkConc / 0.35);
    } else {
        tonedColor = mix(LIGHT_INK, DARK_INK, (inkConc - 0.35) / 0.65);
    }

    // ---- Stroke edges with dark ink ----
    tonedColor = mix(tonedColor, DARK_INK, edge * 0.85 * inkStrength * 0.4);

    // ---- Cinnabar seals: on brightest regions ----
    // Use a simple pseudo-random pattern based on fragment position
    vec2 seed = floor(gl_FragCoord.xy / 8.0);
    float rand = fract(sin(dot(seed, vec2(127.1, 311.7))) * 43758.5453);
    float sealMask = step(0.9, lum) * step(rand, sealChance * 0.35);
    tonedColor = mix(tonedColor, CINNABAR, sealMask);

    // Output LINEAR HDR — no clamp, no ACES, no gamma
    gl_FragColor = vec4(tonedColor, 1.0);

#endif
}
