/*{
  "DESCRIPTION": "Aurora Borealis — animated polar light curtains above dark arctic sky",
  "CREDIT": "ShaderClaw v2 (aurora borealis rewrite of random_freeze)",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "audioReact",    "LABEL": "Audio React",    "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "auroraSpeed",   "LABEL": "Aurora Speed",   "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "intensity",     "LABEL": "Intensity",      "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.2, "MAX": 3.0 },
    { "NAME": "curtainCount",  "LABEL": "Curtain Count",  "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 12.0 }
  ]
}*/

// ── Palette ────────────────────────────────────────────────────────────
const vec3 MIDNIGHT    = vec3(0.0,  0.0,  0.02);  // near-black polar sky
const vec3 POLAR_TEAL  = vec3(0.0,  0.8,  0.6);   // teal aurora
const vec3 ELEC_GREEN  = vec3(0.1,  2.5,  0.3);   // HDR electric green
const vec3 DEEP_VIOLET = vec3(0.4,  0.0,  1.2);   // HDR deep violet
const vec3 PALE_AQUA   = vec3(0.5,  1.8,  1.5);   // HDR pale aqua highlight

// ── Hash helpers ────────────────────────────────────────────────────────
float hash(float n) {
    return fract(sin(n * 127.1) * 43758.5453);
}

float hash2(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// ── Main ───────────────────────────────────────────────────────────────
void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME * auroraSpeed;

    // ── Sky background: vertical gradient MIDNIGHT (top) → dark teal horizon ──
    float skyT = 1.0 - uv.y;  // 0 at top, 1 at horizon
    vec3 horizonColor = vec3(0.0, 0.04, 0.06);
    vec3 sky = mix(MIDNIGHT, horizonColor, pow(skyT, 2.5));

    // ── Stars: sparse bright dots in upper 70% ───────────────────────────
    vec3 col = sky;
    if (uv.y > 0.30) {
        vec2 starGrid = floor(uv * vec2(80.0 * aspect, 60.0));
        float starHash = hash2(starGrid);
        if (starHash > 0.94) {
            // sub-pixel position within cell for falloff
            vec2 cellUV = fract(uv * vec2(80.0 * aspect, 60.0)) - 0.5;
            float dist = length(cellUV);
            float starBright = exp(-dist * 18.0);
            // twinkle
            float twinkle = 0.7 + 0.3 * sin(TIME * (3.0 + starHash * 7.0) + starHash * 100.0);
            vec3 starColor = vec3(2.0, 2.0, 2.2) * starBright * twinkle;
            col += starColor;
        }
    }

    // ── Aurora curtains ─────────────────────────────────────────────────
    // height envelope: curtains live in mid-sky, fade near ground and top
    float heightEnv = smoothstep(0.18, 0.55, uv.y) * smoothstep(1.0, 0.7, uv.y);

    // audio modulator: intensify curtains up to ×1.5
    float audioMod = 0.5 + 0.5 * audioBass * audioReact;

    vec3 aurora_acc = vec3(0.0);
    int N = int(clamp(curtainCount, 2.0, 12.0));

    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);

        // random horizontal center for this curtain
        float cx = hash(fi * 3.13 + 1.0);

        // sinusoidal x-wobble: curtain sways with height and time
        float wobbleFreq  = 2.0 + hash(fi * 1.77) * 4.0;
        float wobbleAmp   = 0.04 + hash(fi * 2.39) * 0.06;
        float wobblePhase = hash(fi * 5.91) * 6.28318;
        float xWobble = wobbleAmp * sin(uv.y * wobbleFreq + t * (0.7 + hash(fi * 0.53)) + wobblePhase);

        float dx = uv.x - cx + xWobble;

        // Gaussian profile in x — width varies per curtain
        float curtainWidth = 0.04 + hash(fi * 4.17) * 0.10;
        float gaussX = exp(-(dx * dx) / (curtainWidth * curtainWidth));

        // vertical ripple in brightness
        float rippleFreq  = 4.0 + hash(fi * 3.71) * 8.0;
        float ripplePhase = hash(fi * 7.23) * 6.28318;
        float ripple = 0.5 + 0.5 * sin(uv.y * rippleFreq + t * 1.5 + ripplePhase);

        float curtainBright = gaussX * ripple * heightEnv * audioMod;

        // pick one of the three aurora colors by hash
        float colorSeed = hash(fi * 9.17 + 2.0);
        vec3 aColor;
        if (colorSeed < 0.33) {
            aColor = POLAR_TEAL;
        } else if (colorSeed < 0.66) {
            aColor = ELEC_GREEN;
        } else {
            aColor = DEEP_VIOLET;
        }

        aurora_acc += aColor * curtainBright;
    }

    // Scale by intensity
    aurora_acc *= intensity;

    col += aurora_acc;

    // PALE_AQUA glow at very bright stacked regions
    float stacked = smoothstep(1.5, 3.0, length(aurora_acc));
    col += PALE_AQUA * stacked * intensity;

    // ── Ground silhouette: dark landscape at bottom 20% ─────────────────
    float groundMask = smoothstep(0.20, 0.18, uv.y);
    col = mix(col, MIDNIGHT, groundMask);

    // Output linear HDR — no clamp, no ACES, no gamma
    gl_FragColor = vec4(col, 1.0);
}
