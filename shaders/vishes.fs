/*{
  "DESCRIPTION": "Clifford Attractor Nebula — strange attractor painted as an additive HDR nebula with audio pulse and slow parameter morphing",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw — Clifford attractor chaos game in ISF persistent pass",
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Speed",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 4.0  },
    { "NAME": "fadeRate",   "LABEL": "Trail Fade", "TYPE": "float", "DEFAULT": 0.001,"MIN": 0.0, "MAX": 0.02 },
    { "NAME": "brightness", "LABEL": "Brightness", "TYPE": "float", "DEFAULT": 1.4,  "MIN": 0.3, "MAX": 4.0  },
    { "NAME": "morphSpeed", "LABEL": "Morph",      "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.0, "MAX": 0.5  }
  ],
  "PASSES": [
    { "TARGET": "canvas", "PERSISTENT": true },
    {}
  ]
}*/

float h1(float n) { return fract(sin(n * 127.1) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Clifford attractor parameters — slow morph keeps it interesting
    float mt = TIME * morphSpeed;
    float a_p = -1.7 + sin(mt * 0.31) * 0.22;
    float b_p =  1.8 + cos(mt * 0.27) * 0.22;
    float c_p = -1.9 + sin(mt * 0.19) * 0.22;
    float d_p = -0.4 + cos(mt * 0.23) * 0.22;

    // ─────────────────────────────────────────────────────────
    // PASS 0 — Accumulate attractor point density onto canvas
    // Each pixel fires 4 independent trajectories and adds
    // Gaussian splashes wherever the orbit passes nearby.
    // ─────────────────────────────────────────────────────────
    if (PASSINDEX == 0) {
        // Pixel-space Gaussian: σ ≈ 2.5 pixels
        float sigSq = 2.0 * 6.25;

        vec3 contrib = vec3(0.0);

        for (int seed = 0; seed < 4; seed++) {
            float fs = float(seed);
            // New random starting point each frame (seeded by TIME + seed index)
            float st = TIME * 2.91 + fs * 17.3;
            vec2 p = vec2(h1(st) * 4.0 - 2.0, h1(st + 1.7) * 4.0 - 2.0);

            // Each seed carries a distinct hue, slowly rotating
            float hue = fract(fs * 0.25 + TIME * 0.025);

            for (int step = 0; step < 150; step++) {
                // One Clifford map iteration
                float xn = sin(a_p * p.y) + c_p * cos(a_p * p.x);
                float yn = sin(b_p * p.x) + d_p * cos(b_p * p.y);
                p = vec2(xn, yn);

                if (step < 25) continue;  // skip warmup (not yet on attractor)

                // Map attractor coords to screen (height = full 0→1, centered in width)
                // Attractor range ≈ [-3, 3]; divide by 6, then center
                vec2 sc = vec2(0.5 + p.x / (6.0 * aspect), 0.5 + p.y / 6.0);

                // Squared pixel-distance from this screen position to our pixel
                vec2 delta = (uv - sc) * RENDERSIZE;
                float dist2 = dot(delta, delta);

                // Gaussian splash (contributes within ~6px radius)
                float splash = exp(-dist2 / sigSq);
                if (splash < 0.005) continue;

                // Color: seed hue + small angular variation for natural gradients
                float localHue = fract(hue + atan(p.y, p.x) * 0.04);
                contrib += hsv2rgb(vec3(localHue, 1.0, 1.0)) * splash;
            }
        }

        // Scale contribution: target canvas ≈ 1.0 for average attractor density
        float audioMod = 1.0 + audioBass * 0.45 + audioLevel * 0.15;
        float scale = 0.025 * speed * audioMod;

        vec4 prev = texture2D(canvas, uv);
        gl_FragColor = prev * (1.0 - fadeRate) + vec4(contrib * scale, 0.0);
        return;
    }

    // ─────────────────────────────────────────────────────────
    // PASS 1 — HDR display: sqrt tone-curve + nebula background
    // ─────────────────────────────────────────────────────────
    vec3 nebula = texture2D(canvas, uv).rgb;

    // sqrt tone curve: compresses wide HDR range into visible space
    // Dense cores (canvas ≈ 60) → sqrt(60)*b ≈ 7.7b (white-hot HDR)
    // Typical arms (canvas ≈ 1)  → sqrt(1)*b  ≈ b   (colored nebula)
    // Faint wisps (canvas ≈ 0.05) → sqrt(0.05)*b ≈ 0.22b (faint glow)
    vec3 col = sqrt(max(nebula, vec3(0.0))) * brightness;

    // Deep-space background: near-black indigo void between arms
    col += vec3(0.003, 0.001, 0.012) * (1.0 - min(length(col), 1.0));

    // Audio: gentle global pulse
    col *= 1.0 + audioLevel * 0.2;

    gl_FragColor = vec4(col, 1.0);
}
