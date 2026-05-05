/*{
    "DESCRIPTION": "Cyberpunk Rain Reflection — neon signs reflected in rain-wet midnight asphalt",
    "CATEGORIES": ["Generator", "Audio Reactive"],
    "PASSES": [
        { "TARGET": "sceneBuf" },
        {}
    ],
    "INPUTS": [
        { "NAME": "audioReact",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0  },
        { "NAME": "rainSpeed",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8  },
        { "NAME": "lightningChance","TYPE": "float", "MIN": 0.0,  "MAX": 0.5,  "DEFAULT": 0.08 },
        { "NAME": "neonGlow",       "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.4  },
        { "NAME": "fogDensity",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.4  }
    ]
}*/

precision highp float;

// ---------- palette ----------
const vec3 MIDNIGHT    = vec3(0.02, 0.02, 0.04);
const vec3 NEON_RED    = vec3(2.5,  0.0,  0.05);
const vec3 NEON_CYAN   = vec3(0.0,  1.8,  2.5);
const vec3 NEON_AMBER  = vec3(2.2,  0.8,  0.0);
const vec3 NEON_PURPLE = vec3(0.8,  0.0,  2.0);
const vec3 RAIN_STREAK = vec3(0.3,  0.5,  0.7);

// ---------- hash ----------
float hash(float n) { return fract(sin(n) * 43758.5453123); }
float hash2d(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123); }

// ---------- SDF helpers ----------
float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

// ---------- PASS 0 — full scene ----------
#ifdef PASSINDEX
#if PASSINDEX == 0

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Audio modulator
    float audio = 0.5 + 0.5 * audioBass * audioReact;

    vec3 col = MIDNIGHT;

    // ---- Sky / asphalt split ----
    float skyLine = 0.55; // uv.y above this = sky, below = asphalt
    float isAsphalt = step(uv.y, skyLine);

    // Base asphalt colour (slightly lighter near horizon)
    vec3 asphaltBase = mix(MIDNIGHT * 1.8, MIDNIGHT, smoothstep(0.3, skyLine, uv.y));
    col = mix(MIDNIGHT, asphaltBase, isAsphalt);

    // ---- Neon signs (upper scene) ----
    // Sign 0 — NEON_RED
    {
        vec2 signPos = vec2(0.22, 0.75);
        vec2 signSize = vec2(0.10, 0.04);
        float d = sdRoundBox(uv - signPos, signSize, 0.008);
        float glow = exp(-max(d, 0.0) * 6.0) * neonGlow * audio;
        float face = step(d, 0.0);
        col += NEON_RED * (face * 0.6 + glow);
    }
    // Sign 1 — NEON_CYAN
    {
        vec2 signPos = vec2(0.65, 0.80);
        vec2 signSize = vec2(0.12, 0.035);
        float d = sdRoundBox(uv - signPos, signSize, 0.008);
        float glow = exp(-max(d, 0.0) * 5.0) * neonGlow * audio;
        float face = step(d, 0.0);
        col += NEON_CYAN * (face * 0.6 + glow);
    }
    // Sign 2 — NEON_AMBER
    {
        vec2 signPos = vec2(0.45, 0.70);
        vec2 signSize = vec2(0.08, 0.03);
        float d = sdRoundBox(uv - signPos, signSize, 0.006);
        float glow = exp(-max(d, 0.0) * 7.0) * neonGlow * audio;
        float face = step(d, 0.0);
        col += NEON_AMBER * (face * 0.6 + glow);
    }
    // Sign 3 — NEON_PURPLE
    {
        vec2 signPos = vec2(0.82, 0.72);
        vec2 signSize = vec2(0.09, 0.032);
        float d = sdRoundBox(uv - signPos, signSize, 0.007);
        float glow = exp(-max(d, 0.0) * 6.0) * neonGlow * audio;
        float face = step(d, 0.0);
        col += NEON_PURPLE * (face * 0.6 + glow);
    }

    // ---- Reflections on asphalt ----
    if (uv.y < skyLine) {
        // Mirror the above-horizon UV and add waviness
        float waveAmp = 0.008 + 0.005 * audio;
        float waveFreq = 60.0;
        float waviness = sin(uv.x * waveFreq * aspect + TIME * rainSpeed * 3.0) * waveAmp;
        vec2 reflUV = vec2(uv.x + waviness, 1.0 - uv.y * 0.42 + 0.5);
        reflUV = clamp(reflUV, vec2(0.0), vec2(1.0));

        // Build reflected neon by recomputing sign SDFs at reflected UV
        vec3 reflCol = vec3(0.0);

        vec2 ruv = reflUV;
        {
            vec2 sp = vec2(0.22, 0.75); vec2 ss = vec2(0.10, 0.04);
            float d = sdRoundBox(ruv - sp, ss, 0.008);
            reflCol += NEON_RED * exp(-max(d,0.0) * 6.0) * neonGlow * audio;
        }
        {
            vec2 sp = vec2(0.65, 0.80); vec2 ss = vec2(0.12, 0.035);
            float d = sdRoundBox(ruv - sp, ss, 0.008);
            reflCol += NEON_CYAN * exp(-max(d,0.0) * 5.0) * neonGlow * audio;
        }
        {
            vec2 sp = vec2(0.45, 0.70); vec2 ss = vec2(0.08, 0.03);
            float d = sdRoundBox(ruv - sp, ss, 0.006);
            reflCol += NEON_AMBER * exp(-max(d,0.0) * 7.0) * neonGlow * audio;
        }
        {
            vec2 sp = vec2(0.82, 0.72); vec2 ss = vec2(0.09, 0.032);
            float d = sdRoundBox(ruv - sp, ss, 0.007);
            reflCol += NEON_PURPLE * exp(-max(d,0.0) * 6.0) * neonGlow * audio;
        }

        // Reflection fades with distance from skyLine
        float reflFade = smoothstep(0.0, skyLine, uv.y) * 0.7;
        col += reflCol * reflFade;
    }

    // ---- Rain streaks ----
    {
        float cols = 120.0 * aspect;
        float col_idx = floor(uv.x * cols);
        // Per-column phase offset
        float phase = hash(col_idx) * 10.0;
        float speed = (0.8 + hash(col_idx * 0.3) * 0.4) * rainSpeed;
        // Streak position scrolls downward
        float streakY = fract(uv.y + TIME * speed + phase);
        float streakLen = 0.04 + hash(col_idx * 7.3) * 0.06;
        float streakMask = smoothstep(0.0, streakLen * 0.3, streakY)
                         * smoothstep(streakLen, streakLen * 0.6, streakY);
        // Width mask — only show in thin vertical band
        float colFrac = fract(uv.x * cols);
        float widthMask = step(0.88, colFrac);
        float rainMask = streakMask * widthMask * (0.25 + 0.15 * audio);
        col += RAIN_STREAK * rainMask;
    }

    // ---- Rain ripples on asphalt ----
    if (uv.y < skyLine) {
        for (int i = 0; i < 6; i++) {
            float fi = float(i);
            // Pseudo-random drop position
            vec2 dropPos = vec2(hash(fi * 3.7 + 0.1), hash(fi * 5.3 + 0.2) * skyLine);
            float spawnT = hash(fi * 11.1 + floor(TIME * 2.0));
            float rippleT = fract(TIME * 1.5 + spawnT);
            float dist = length((uv - dropPos) * vec2(aspect, 1.0));
            float maxR = 0.12 * rippleT;
            float ring = cos(dist * 40.0 - TIME * 6.0) * exp(-dist * 8.0)
                       * exp(-rippleT * 3.0)
                       * smoothstep(0.0, 0.01, maxR - dist);
            col += RAIN_STREAK * ring * 0.3;
        }
    }

    // ---- Lightning flash ----
    {
        float flashSeed = floor(TIME * 3.0);
        float flashRand = hash(flashSeed * 7.77 + 3.14);
        float flash = step(1.0 - lightningChance, flashRand);
        float flashDecay = fract(TIME * 3.0); // 0→1 in one frame slot
        float flashBright = flash * exp(-flashDecay * 8.0);
        vec3 flashCol = vec3(1.5, 1.6, 2.0);
        col = mix(col, flashCol, flashBright * 0.85);
    }

    // ---- Atmospheric fog (distance toward horizon) ----
    {
        vec3 fogCol = vec3(0.04, 0.04, 0.08);
        float fogAmt = fogDensity * smoothstep(0.0, skyLine, uv.y);
        col = mix(col, fogCol, fogAmt);
    }

    gl_FragColor = vec4(col, 1.0);
}

#endif // PASSINDEX == 0

// ---------- PASS 1 — composite + scanlines + chromatic aberration ----------
#if PASSINDEX == 1

void main() {
    vec2 uv = isf_FragNormCoord;

    float aberr = 0.003;
    vec3 col;
    col.r = texture2D(sceneBuf, uv + vec2( aberr, 0.0)).r;
    col.g = texture2D(sceneBuf, uv).g;
    col.b = texture2D(sceneBuf, uv + vec2(-aberr, 0.0)).b;

    // Scanline modulation
    float scanline = 0.85 + 0.15 * sin(gl_FragCoord.y * 2.0);
    col *= scanline;

    gl_FragColor = vec4(col, 1.0);
}

#endif // PASSINDEX == 1
#endif // PASSINDEX defined
