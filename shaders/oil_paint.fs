/*{
  "DESCRIPTION": "Sumi-e Ink Wash — converts input to Japanese ink painting with Sobel edge strokes and cinnabar seals",
  "CATEGORIES": ["Effect"],
  "INPUTS": [
    { "NAME": "inputImage",  "TYPE": "image" },
    { "NAME": "inkStrength", "TYPE": "float", "MIN": 0.5, "MAX": 5.0,  "DEFAULT": 2.2 },
    { "NAME": "washBlur",    "TYPE": "float", "MIN": 1.0, "MAX": 8.0,  "DEFAULT": 3.5 },
    { "NAME": "sealChance",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.5 },
    { "NAME": "audioReact",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.6 }
  ],
  "PASSES": [
    { "TARGET": "blurBuf" },
    {}
  ]
}*/

precision highp float;

// ---------------------------------------------------------------------------
// PALETTE — exactly 4 + cinnabar accent, no others
// ---------------------------------------------------------------------------
#define PAPER      vec3(2.00, 1.85, 1.60)   // HDR warm paper
#define LIGHT_INK  vec3(0.45, 0.40, 0.35)   // pale wash
#define DARK_INK   vec3(0.03, 0.025, 0.02)  // deep sumi
#define CINNABAR   vec3(1.80, 0.05, 0.00)   // HDR red seal accent

// ---------------------------------------------------------------------------
// PASS 0 — 9x9 Gaussian blur of inputImage → blurBuf
// ---------------------------------------------------------------------------
#ifdef PASSINDEX
#if PASSINDEX == 0

// Gaussian weights for radius-4 kernel (sigma ≈ 1.7)
// Computed as 1-D separable; we do a single 2-D 9x9 pass here for simplicity.
float gauss9[9];

void buildGauss() {
    // unnormalised 1-D weights: sigma = (washBlur / 8.0) * 4.0 + 0.5
    // For simplicity use fixed weights that give soft wash look
    gauss9[0] = 0.0625;
    gauss9[1] = 0.1250;
    gauss9[2] = 0.1875;
    gauss9[3] = 0.2500;
    gauss9[4] = 0.3125;  // not normalised — we'll divide
    gauss9[5] = 0.2500;
    gauss9[6] = 0.1875;
    gauss9[7] = 0.1250;
    gauss9[8] = 0.0625;
}

void main() {
    vec2 uv      = isf_FragNormCoord;
    vec2 texel   = 1.0 / RENDERSIZE;
    float radius = washBlur;                 // blur radius in pixels
    vec4 sum     = vec4(0.0);
    float wTotal = 0.0;

    for (int x = -4; x <= 4; x++) {
        for (int y = -4; y <= 4; y++) {
            float wx = exp(-float(x * x) / (2.0 * radius * radius));
            float wy = exp(-float(y * y) / (2.0 * radius * radius));
            float w  = wx * wy;
            vec2 offset = vec2(float(x), float(y)) * texel;
            sum    += IMG_NORM_PIXEL(inputImage, uv + offset) * w;
            wTotal += w;
        }
    }
    gl_FragColor = sum / wTotal;
}

#endif // PASSINDEX == 0

// ---------------------------------------------------------------------------
// PASS 1 — Sumi-e tone mapping + Sobel edges + cinnabar seals
// ---------------------------------------------------------------------------
#if PASSINDEX == 1

void main() {
    vec2 uv    = isf_FragNormCoord;
    vec2 texel = 1.0 / RENDERSIZE;

    // Audio modulator
    float audio = 0.5 + 0.5 * audioBass * audioReact;

    // Sample blurred image
    vec4 blurred = IMG_NORM_PIXEL(blurBuf, uv);

    // Luminance
    float lum = dot(blurred.rgb, vec3(0.299, 0.587, 0.114));

    // ---------------------------------------------------------------------------
    // Sobel edge detection on blurBuf (4-neighbour)
    float lumL = dot(IMG_NORM_PIXEL(blurBuf, uv + vec2(-texel.x, 0.0)).rgb, vec3(0.299, 0.587, 0.114));
    float lumR = dot(IMG_NORM_PIXEL(blurBuf, uv + vec2( texel.x, 0.0)).rgb, vec3(0.299, 0.587, 0.114));
    float lumD = dot(IMG_NORM_PIXEL(blurBuf, uv + vec2(0.0, -texel.y)).rgb, vec3(0.299, 0.587, 0.114));
    float lumU = dot(IMG_NORM_PIXEL(blurBuf, uv + vec2(0.0,  texel.y)).rgb, vec3(0.299, 0.587, 0.114));

    float gx   = lumR - lumL;
    float gy   = lumU - lumD;
    float edge = clamp(sqrt(gx * gx + gy * gy) * inkStrength * 3.0, 0.0, 1.0);

    // ---------------------------------------------------------------------------
    // Tone mapping — ink concentration
    float inkConc = pow(1.0 - lum, inkStrength * 0.5 * audio);

    // Piecewise lerp: low inkConc → PAPER, mid → LIGHT_INK, high → DARK_INK
    vec3 tonedColor;
    if (inkConc < 0.35) {
        tonedColor = mix(PAPER, LIGHT_INK, smoothstep(0.0, 0.35, inkConc));
    } else {
        tonedColor = mix(LIGHT_INK, DARK_INK, smoothstep(0.35, 1.0, inkConc));
    }

    // ---------------------------------------------------------------------------
    // Stroke edges with dark ink
    tonedColor = mix(tonedColor, DARK_INK, edge * 0.85 * inkStrength * 0.4);

    // ---------------------------------------------------------------------------
    // Cinnabar seals — replace brightest regions with HDR red accent
    // Uses a stable hash derived from UV to scatter seal blobs
    if (lum > 0.9) {
        // Small scattered blobs: use quantised uv grid
        vec2 cell  = floor(uv * 18.0);
        float h    = fract(sin(dot(cell, vec2(127.1, 311.7))) * 43758.5453);
        // Only paint seal where hash < sealChance threshold
        float sealMask = step(h, sealChance * 0.35);
        tonedColor = mix(tonedColor, CINNABAR, sealMask);
    }

    // ---------------------------------------------------------------------------
    // Output — linear HDR, no clamp, no ACES, no gamma
    gl_FragColor = vec4(tonedColor, 1.0);
}

#endif // PASSINDEX == 1
#endif // PASSINDEX defined
