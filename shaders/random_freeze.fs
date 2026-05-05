/*{
  "DESCRIPTION": "Aurora Borealis — animated polar light curtains above dark arctic sky",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "audioReact",    "LABEL": "Audio React",    "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "auroraSpeed",   "LABEL": "Aurora Speed",   "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "intensity",     "LABEL": "Intensity",      "TYPE": "float", "DEFAULT": 1.1, "MIN": 0.2, "MAX": 3.0 },
    { "NAME": "curtainCount",  "LABEL": "Curtain Count",  "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 12.0 }
  ]
}*/

// ── Palette ──────────────────────────────────────────────────────────
#define MIDNIGHT    vec3(0.0,  0.0,  0.02)
#define POLAR_TEAL  vec3(0.0,  0.8,  0.6)
#define ELEC_GREEN  vec3(0.1,  2.5,  0.3)
#define DEEP_VIOLET vec3(0.4,  0.0,  1.2)
#define PALE_AQUA   vec3(0.5,  1.8,  1.5)

// ── Hashes ───────────────────────────────────────────────────────────
float hash(float n) {
    return fract(sin(n * 127.1) * 43758.5453);
}

vec2 hash2(vec2 p) {
    float h = dot(p, vec2(127.1, 311.7));
    return fract(sin(vec2(h, h + 19.19)) * 43758.5453);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // ── Sky background: vertical gradient MIDNIGHT top → dark teal horizon ──
    vec3 horizonTeal = vec3(0.0, 0.04, 0.06);
    vec3 sky = mix(horizonTeal, MIDNIGHT, smoothstep(0.0, 0.6, uv.y));
    vec3 col = sky;

    // ── Stars: sparse bright dots in upper 70% ──────────────────────
    {
        vec2 starCell = floor(uv * vec2(80.0 * aspect, 60.0));
        vec2 h2 = hash2(starCell);
        float starHash = h2.x;
        float twinkle  = h2.y;
        if (uv.y > 0.30 && starHash > 0.94) {
            vec2 cellUV  = fract(uv * vec2(80.0 * aspect, 60.0));
            vec2 starPos = vec2(0.5);
            float dist   = length(cellUV - starPos);
            float glow   = exp(-dist * 14.0);
            float twink  = 0.7 + 0.3 * sin(TIME * (3.0 + twinkle * 5.0) + twinkle * 6.28);
            col += vec3(2.0, 2.0, 2.2) * glow * twink * 0.6;
        }
    }

    // ── Aurora curtains ──────────────────────────────────────────────
    // Curtain height envelope: curtains glow in mid-sky
    float heightEnv = smoothstep(0.18, 0.55, uv.y) * smoothstep(1.0, 0.7, uv.y);

    // Audio modulator: intensify curtains up to x1.5
    float audioMod = 0.5 + 0.5 * audioBass * audioReact;

    vec3 aurora_acc = vec3(0.0);

    int N = int(clamp(curtainCount, 2.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);

        // Random horizontal center for this curtain
        float cx = hash(fi * 3.13 + 1.0);

        // Sinusoidal x-wobble driven by uv.y and TIME
        float wobbleFreq  = 2.0 + hash(fi * 1.77) * 3.0;
        float wobbleAmp   = 0.04 + hash(fi * 2.31) * 0.06;
        float wobbleSpeed = auroraSpeed * (0.5 + hash(fi * 0.91) * 1.0);
        float xWobble     = wobbleAmp * sin(uv.y * wobbleFreq * 6.2832 + TIME * wobbleSpeed + fi * 1.23);

        // Gaussian profile in x
        float dx      = uv.x - (cx + xWobble);
        float width   = 0.04 + hash(fi * 4.19) * 0.08;
        float profile = exp(-dx * dx / (width * width));

        // Vertical ripple in brightness
        float rippleFreq  = 4.0 + hash(fi * 5.55) * 6.0;
        float rippleSpeed = auroraSpeed * (1.0 + hash(fi * 3.33) * 1.5);
        float ripple      = 0.6 + 0.4 * sin(uv.y * rippleFreq * 6.2832 + TIME * rippleSpeed + fi * 2.71);

        // One of the three aurora colors selected by hash
        float colorSel = hash(fi * 7.07 + 0.5);
        vec3 curtainColor;
        if (colorSel < 0.33)       curtainColor = POLAR_TEAL;
        else if (colorSel < 0.66)  curtainColor = ELEC_GREEN;
        else                       curtainColor = DEEP_VIOLET;

        float brightness = profile * ripple * heightEnv * intensity * (1.0 + 0.5 * audioMod);
        aurora_acc += curtainColor * brightness;
    }

    col += aurora_acc;

    // ── PALE_AQUA glow at very bright stacked regions ────────────────
    col += PALE_AQUA * smoothstep(1.5, 3.0, length(aurora_acc)) * 0.6;

    // ── Ground silhouette: dark landscape bottom 20% ─────────────────
    float ground = smoothstep(0.20, 0.18, uv.y);
    col = mix(col, MIDNIGHT, ground);

    // Output linear HDR — no clamp, no ACES, no gamma
    gl_FragColor = vec4(col, 1.0);
}
