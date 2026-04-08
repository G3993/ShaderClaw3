/*{
  "DESCRIPTION": "Multi-Layer Particles — N particles in shared coordinate space, split across layers for multi-GPU projection mapping. Set layerIndex differently on each ShaderClaw layer (0, 1, 2...) to split particles across GPU streams.",
  "CREDIT": "Etherea / ShaderClaw",
  "CATEGORIES": ["Generator", "Projection"],
  "INPUTS": [
    { "NAME": "layerIndex", "LABEL": "Layer Index", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0 },
    { "NAME": "layerCount", "LABEL": "Layer Count", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 8.0 },
    { "NAME": "particleCount", "LABEL": "Particles", "TYPE": "float", "DEFAULT": 30.0, "MIN": 3.0, "MAX": 100.0 },
    { "NAME": "particleSize", "LABEL": "Size", "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.01, "MAX": 0.2 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "trailLength", "LABEL": "Trail", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioDrive", "LABEL": "Audio Drive", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "colorMode", "LABEL": "Color Mode", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "accentColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [1.0, 0.7, 0.3, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 1.0 }
  ],
  "PASSES": [
    { "TARGET": "trail", "PERSISTENT": true },
    {}
  ]
}*/

// ============================================================
// Deterministic hash — identical output on every GPU/instance
// ============================================================

float hash1(float n) { return fract(sin(n) * 43758.5453); }

vec2 hash2(float n) {
    return vec2(hash1(n), hash1(n + 127.1));
}

vec3 hash3(float n) {
    return vec3(hash1(n), hash1(n + 127.1), hash1(n + 269.5));
}

// ============================================================
// Particle position — deterministic from ID and time
// ============================================================

vec2 particlePos(float id, float t) {
    // Each particle has a unique orbit: Lissajous curves with hash-driven frequencies
    vec2 h = hash2(id * 7.13);
    float fx = 0.3 + h.x * 1.7;  // x frequency
    float fy = 0.5 + h.y * 1.3;  // y frequency
    float px = hash1(id * 3.91);  // x phase
    float py = hash1(id * 5.17);  // y phase

    // Amplitude varies per particle (keep inside 0-1 with margin)
    float ax = 0.15 + hash1(id * 11.3) * 0.3;
    float ay = 0.15 + hash1(id * 13.7) * 0.3;

    return vec2(
        0.5 + ax * sin(t * fx + px * 6.2832),
        0.5 + ay * sin(t * fy + py * 6.2832)
    );
}

// ============================================================
// Particle color — three modes
// ============================================================

vec3 particleColor(float id, float count) {
    float mode = floor(colorMode + 0.5);

    if (mode < 0.5) {
        // Mode 0: accent color with per-particle hue shift
        vec3 base = accentColor.rgb;
        float hueShift = (hash1(id * 13.37) - 0.5) * 0.4;
        float angle = hueShift * 6.2832;
        float cs = cos(angle), sn = sin(angle);
        return vec3(
            base.r * (0.667 + cs * 0.333) + base.g * (0.333 - cs * 0.333 + sn * 0.577) + base.b * (0.333 - cs * 0.333 - sn * 0.577),
            base.r * (0.333 - cs * 0.333 - sn * 0.577) + base.g * (0.667 + cs * 0.333) + base.b * (0.333 - cs * 0.333 + sn * 0.577),
            base.r * (0.333 - cs * 0.333 + sn * 0.577) + base.g * (0.333 - cs * 0.333 - sn * 0.577) + base.b * (0.667 + cs * 0.333)
        ) * (0.7 + 0.3 * hash1(id * 2.37));
    } else if (mode < 1.5) {
        // Mode 1: per-layer distinct color (layers get different hues)
        float layerHue = floor(layerIndex + 0.5) / max(floor(layerCount + 0.5), 1.0);
        float hue = layerHue + hash1(id * 13.37) * 0.1;
        // HSV to RGB
        vec3 rgb = clamp(abs(mod(hue * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
        return rgb * (0.7 + 0.3 * hash1(id * 2.37));
    } else {
        // Mode 2: rainbow per particle
        float hue = hash1(id * 17.31);
        vec3 rgb = clamp(abs(mod(hue * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
        return rgb;
    }
}

// ============================================================
// Pass 0: Trail buffer (persistent feedback)
// ============================================================

vec4 passTrail(vec2 uv) {
    // Fade previous frame
    vec4 prev = texture2D(trail, uv);
    float fade = 0.9 + trailLength * 0.09; // 0.9 to 0.99

    vec3 col = prev.rgb * fade;
    float alpha = prev.a * fade;

    float t = TIME * speed;
    float count = floor(particleCount);
    float layers = max(floor(layerCount + 0.5), 1.0);
    float myLayer = floor(layerIndex + 0.5);

    // Audio-reactive size pulse
    float audioPulse = 1.0 + audioBass * audioDrive * 0.5;

    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 uvAspect = vec2(uv.x * aspect, uv.y);

    for (float i = 0.0; i < 100.0; i++) {
        if (i >= count) break;

        // Layer assignment: this particle belongs to layer (i mod layerCount)
        float assignedLayer = mod(i, layers);
        if (abs(assignedLayer - myLayer) > 0.5) continue;

        vec2 pos = particlePos(i, t);
        vec2 posAspect = vec2(pos.x * aspect, pos.y);

        float dist = length(uvAspect - posAspect);
        float sz = particleSize * audioPulse * (0.7 + hash1(i * 2.37) * 0.6);

        // Soft glow falloff
        float glow = sz / (dist + 0.001);
        glow = pow(glow, 2.5) * 0.015;

        // Hard core
        float core = smoothstep(sz, sz * 0.3, dist);

        vec3 pCol = particleColor(i, count);

        col += pCol * (core + glow);
        alpha += core + glow * 0.5;
    }

    return vec4(col, clamp(alpha, 0.0, 1.0));
}

// ============================================================
// Pass 1: Final output
// ============================================================

vec4 passFinal(vec2 uv) {
    vec4 trailCol = texture2D(trail, uv);

    // Tone mapping — prevent blowout from accumulated trails
    vec3 col = trailCol.rgb / (1.0 + trailCol.rgb);

    if (transparentBg) {
        float lum = dot(col, vec3(0.299, 0.587, 0.114));
        float alpha = clamp(lum * 3.0, 0.0, 1.0);
        if (trailCol.a < 0.01) alpha = 0.0;
        return vec4(col, alpha);
    }

    return vec4(col, 1.0);
}

// ============================================================
// Dispatch
// ============================================================

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE;
    if (PASSINDEX == 0) {
        gl_FragColor = passTrail(uv);
    } else {
        gl_FragColor = passFinal(uv);
    }
}
