/*{
  "DESCRIPTION": "Cyberpunk Rain Reflection — neon signs reflected in rain-wet midnight asphalt",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "audioReact",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "rainSpeed",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8 },
    { "NAME": "lightningChance", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.08 },
    { "NAME": "neonGlow",        "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.4 },
    { "NAME": "fogDensity",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4 }
  ],
  "PASSES": [
    { "TARGET": "sceneBuf" },
    {}
  ]
}*/

precision highp float;

// ── Palette ──────────────────────────────────────────────────────────────────
const vec3 MIDNIGHT    = vec3(0.02, 0.02, 0.04);
const vec3 NEON_RED    = vec3(2.50, 0.00, 0.05);
const vec3 NEON_CYAN   = vec3(0.00, 1.80, 2.50);
const vec3 NEON_AMBER  = vec3(2.20, 0.80, 0.00);
const vec3 NEON_PURPLE = vec3(0.80, 0.00, 2.00);
const vec3 RAIN_STREAK = vec3(0.30, 0.50, 0.70);

// ── Hash helpers ─────────────────────────────────────────────────────────────
float hash(float n) {
    return fract(sin(n) * 43758.5453123);
}
float hash2f(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

// ── SDF round-box (2D) ────────────────────────────────────────────────────────
float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

// ── Pass 0 ────────────────────────────────────────────────────────────────────
#ifdef PASSINDEX
#if PASSINDEX == 0

void main() {
    vec2  uv     = isf_FragNormCoord;           // [0,1] bottom-left origin
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float audio  = 0.5 + 0.5 * audioBass * audioReact;

    // Flip Y so top of screen = sky
    vec2 suv = vec2(uv.x, 1.0 - uv.y);

    // ── Background split: sky (top 40%) / asphalt (bottom 60%) ───────────────
    vec3 col;
    float horizonY = 0.42;
    if (suv.y < horizonY) {
        // Asphalt — dark with subtle wet sheen gradient
        float wetness = smoothstep(0.0, horizonY, suv.y);
        col = mix(MIDNIGHT * 1.8, MIDNIGHT, wetness);
    } else {
        // Sky — very dark midnight gradient
        float skyGrad = (suv.y - horizonY) / (1.0 - horizonY);
        col = mix(MIDNIGHT * 1.4, MIDNIGHT * 0.6, skyGrad);
    }

    // ── Neon signs (sky region) ───────────────────────────────────────────────
    // Sign 1 — Red (left)
    {
        vec2  spos  = vec2(0.18, 0.70);
        vec2  shalf = vec2(0.09, 0.035);
        vec2  sp    = (suv - spos) * vec2(aspect, 1.0);
        float d     = sdRoundBox(sp, shalf * vec2(aspect, 1.0), 0.005);
        float glow  = exp(-max(d, 0.0) * 6.0) * neonGlow * audio;
        float body  = smoothstep(0.004, -0.001, d);
        col += NEON_RED * (glow + body * 0.6);
        // Inner text bars
        vec2  tp  = sp;
        float tb1 = abs(tp.y + 0.005) - 0.002;
        float tb2 = abs(tp.y - 0.010) - 0.002;
        float textMask = smoothstep(0.002, 0.0, min(tb1, tb2)) *
                         step(abs(tp.x), shalf.x * aspect * 0.8);
        col += NEON_RED * textMask * 0.8;
    }

    // Sign 2 — Cyan (center)
    {
        vec2  spos  = vec2(0.50, 0.65);
        vec2  shalf = vec2(0.11, 0.04);
        vec2  sp    = (suv - spos) * vec2(aspect, 1.0);
        float d     = sdRoundBox(sp, shalf * vec2(aspect, 1.0), 0.006);
        float glow  = exp(-max(d, 0.0) * 5.0) * neonGlow * audio;
        float body  = smoothstep(0.004, -0.001, d);
        col += NEON_CYAN * (glow + body * 0.5);
    }

    // Sign 3 — Amber (upper right)
    {
        vec2  spos  = vec2(0.80, 0.76);
        vec2  shalf = vec2(0.075, 0.030);
        vec2  sp    = (suv - spos) * vec2(aspect, 1.0);
        float d     = sdRoundBox(sp, shalf * vec2(aspect, 1.0), 0.005);
        float glow  = exp(-max(d, 0.0) * 7.0) * neonGlow * audio;
        float body  = smoothstep(0.003, -0.001, d);
        col += NEON_AMBER * (glow + body * 0.6);
    }

    // Sign 4 — Purple (far right, lower)
    {
        vec2  spos  = vec2(0.88, 0.56);
        vec2  shalf = vec2(0.06, 0.025);
        vec2  sp    = (suv - spos) * vec2(aspect, 1.0);
        float d     = sdRoundBox(sp, shalf * vec2(aspect, 1.0), 0.004);
        float glow  = exp(-max(d, 0.0) * 8.0) * neonGlow * audio;
        float body  = smoothstep(0.003, -0.001, d);
        col += NEON_PURPLE * (glow + body * 0.5);
    }

    // ── Reflections on asphalt ────────────────────────────────────────────────
    if (suv.y < horizonY) {
        float reflY = suv.y / horizonY;   // 0=bottom, 1=horizon
        float waveX = sin(suv.x * 80.0 * aspect + TIME * rainSpeed * 3.0) * 0.004
                    + sin(suv.x * 30.0 * aspect - TIME * 2.1) * 0.006;
        vec2  rUV   = vec2(suv.x + waveX, horizonY + (horizonY - suv.y) * 0.35);

        // Sign 1
        {
            vec2 spos = vec2(0.18, 0.70); vec2 shalf = vec2(0.09, 0.035);
            vec2 sp = (rUV - spos) * vec2(aspect, 1.0);
            float d = sdRoundBox(sp, shalf * vec2(aspect, 1.0), 0.005);
            col += NEON_RED * exp(-max(d, 0.0) * 5.0) * neonGlow * audio * (0.5 + 0.5 * reflY);
        }
        // Sign 2
        {
            vec2 spos = vec2(0.50, 0.65); vec2 shalf = vec2(0.11, 0.04);
            vec2 sp = (rUV - spos) * vec2(aspect, 1.0);
            float d = sdRoundBox(sp, shalf * vec2(aspect, 1.0), 0.006);
            col += NEON_CYAN * exp(-max(d, 0.0) * 4.5) * neonGlow * audio * (0.5 + 0.5 * reflY);
        }
        // Sign 3
        {
            vec2 spos = vec2(0.80, 0.76); vec2 shalf = vec2(0.075, 0.030);
            vec2 sp = (rUV - spos) * vec2(aspect, 1.0);
            float d = sdRoundBox(sp, shalf * vec2(aspect, 1.0), 0.005);
            col += NEON_AMBER * exp(-max(d, 0.0) * 6.0) * neonGlow * audio * (0.5 + 0.5 * reflY);
        }
        // Sign 4
        {
            vec2 spos = vec2(0.88, 0.56); vec2 shalf = vec2(0.06, 0.025);
            vec2 sp = (rUV - spos) * vec2(aspect, 1.0);
            float d = sdRoundBox(sp, shalf * vec2(aspect, 1.0), 0.004);
            col += NEON_PURPLE * exp(-max(d, 0.0) * 7.0) * neonGlow * audio * (0.5 + 0.5 * reflY);
        }
    }

    // ── Rain puddle ripples (asphalt only) ────────────────────────────────────
    if (suv.y < horizonY) {
        for (int i = 0; i < 8; i++) {
            float fi   = float(i);
            vec2  seed = vec2(hash(fi * 1.73), hash(fi * 3.17));
            vec2  dropP= vec2(seed.x, seed.y * horizonY);
            float phase= TIME * 2.5 + fi * 1.3;
            vec2  d2   = (suv - dropP) * vec2(aspect, 1.0);
            float dist = length(d2);
            float ripple = cos(dist * 40.0 - phase) * exp(-dist * 8.0) * 0.012;
            col += RAIN_STREAK * ripple;
        }
    }

    // ── Rain streaks (full canvas) ────────────────────────────────────────────
    {
        float colIdx  = floor(suv.x * 120.0 * aspect);
        float colFrac = fract(suv.x * 120.0 * aspect);
        float streakOffset = hash(colIdx) * 1.0;
        float streakY = fract(suv.y * 2.0 + TIME * rainSpeed * 0.8 + streakOffset);
        float streakMask = step(0.93, colFrac) *
                           smoothstep(0.0, 0.05, streakY) *
                           smoothstep(1.0, 0.85, streakY);
        float brightness = 0.12 + hash(colIdx * 7.3) * 0.1;
        col += RAIN_STREAK * streakMask * brightness;
    }

    // ── Lightning flash ───────────────────────────────────────────────────────
    {
        float lightningTrigger = step(1.0 - lightningChance, hash(floor(TIME * 3.0)));
        float flashAlpha       = lightningTrigger * (0.3 + 0.4 * fract(TIME * 3.0));
        col = mix(col, vec3(1.5, 1.6, 2.0), flashAlpha * 0.35);
    }

    // ── Fog ───────────────────────────────────────────────────────────────────
    {
        vec3  fogColor = vec3(0.04, 0.04, 0.08);
        float fogFactor = suv.y > horizonY
            ? fogDensity * smoothstep(horizonY, 1.0, suv.y) * 0.7
            : fogDensity * (1.0 - suv.y / horizonY) * 0.3;
        col = mix(col, fogColor, fogFactor);
    }

    gl_FragColor = vec4(col, 1.0);
}

#endif // PASSINDEX == 0

// ── Pass 1 — chromatic aberration + scanlines ─────────────────────────────────
#if PASSINDEX == 1

void main() {
    vec2 uv = isf_FragNormCoord;
    float aberr = 0.003;
    vec3 col;
    col.r = texture2D(sceneBuf, uv + vec2( aberr, 0.0)).r;
    col.g = texture2D(sceneBuf, uv                   ).g;
    col.b = texture2D(sceneBuf, uv - vec2( aberr, 0.0)).b;

    float scanline = 0.85 + 0.15 * sin(gl_FragCoord.y * 2.0);
    col *= scanline;

    gl_FragColor = vec4(col, 1.0);
}

#endif // PASSINDEX == 1
#endif // PASSINDEX defined
