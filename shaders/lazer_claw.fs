/*
{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Lazer Claw — sharp neon claw slash marks with persistent trails",
  "INPUTS": [
    { "NAME": "clawSize", "LABEL": "Size", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.02, "MAX": 0.5 },
    { "NAME": "spread", "LABEL": "Spread", "TYPE": "float", "DEFAULT": 0.035, "MIN": 0.005, "MAX": 0.15 },
    { "NAME": "thickness", "LABEL": "Thickness", "TYPE": "float", "DEFAULT": 0.012, "MIN": 0.003, "MAX": 0.05 },
    { "NAME": "glowWidth", "LABEL": "Glow", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "feedback", "LABEL": "Trail", "TYPE": "float", "DEFAULT": 0.92, "MIN": 0.5, "MAX": 0.99 },
    { "NAME": "color1", "LABEL": "Claw 1", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
    { "NAME": "color2", "LABEL": "Claw 2", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "color3", "LABEL": "Claw 3", "TYPE": "color", "DEFAULT": [1.0, 0.0, 0.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": true }
  ],
  "PASSES": [
    { "TARGET": "fbBuf", "PERSISTENT": true },
    {}
  ]
}
*/

vec2 rot2d(vec2 p, float a) {
    float c = cos(a), s = sin(a);
    return vec2(p.x * c - p.y * s, p.x * s + p.y * c);
}

// Curved tapered claw mark — thin scratch shape
float clawSDF(vec2 p, float len, float w, float curve) {
    p.y -= curve * p.x * p.x;
    // Taper: thickest at center, sharp points at tips
    float t = clamp(p.x / len, -1.0, 1.0);
    float taper = w * (1.0 - t * t);
    float dy = abs(p.y) - taper;
    float dx = abs(p.x) - len;
    return length(max(vec2(dx, dy), 0.0)) + min(max(dx, dy), 0.0);
}

// ═══════════════════════════════════════════════════════════════════════
// PASS 0: Feedback buffer — decay + paint claw scratches at mouse
// ═══════════════════════════════════════════════════════════════════════

vec4 passFeedback() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Decay previous frame
    vec3 col = vec3(0.0);
    if (FRAMEINDEX > 0) {
        col = texture2D(fbBuf, uv).rgb * feedback;
        // Cool fade — claw marks dim toward embers
        col.r *= 1.001;
        col.gb *= 0.998;
    }

    vec2 mPos = mousePos;
    vec2 toMouse = uv - mPos;
    toMouse.x *= aspect;
    float mouseDist = length(toMouse);

    // Only draw near mouse
    float drawRadius = clawSize * 2.0;
    if (mouseDist < drawRadius) {
        float angle = atan(toMouse.y, toMouse.x);

        // Local claw space
        vec2 local = rot2d(toMouse, -angle + 1.5708);
        local /= clawSize;

        float w = thickness / clawSize;
        float len = 0.6;
        float crv = 0.3;

        // 3 parallel claw scratches
        float sp = spread / clawSize;
        float d1 = clawSDF(local + vec2(0.0, sp), len, w * 0.8, crv * 1.2);
        float d2 = clawSDF(local, len, w * 1.0, crv);
        float d3 = clawSDF(local - vec2(0.0, sp), len, w * 0.8, crv * 0.8);

        // Proximity fade
        float proxFade = smoothstep(drawRadius, 0.0, mouseDist);

        // Sharp neon edge — bright core with controlled glow falloff
        float glowR = max(0.002, glowWidth * 0.02);

        // Core: hard bright line where SDF < 0 (inside the mark)
        float core1 = smoothstep(0.002, -0.001, d1);
        float core2 = smoothstep(0.002, -0.001, d2);
        float core3 = smoothstep(0.002, -0.001, d3);

        // Glow: soft falloff outside the mark
        float glow1 = glowR / (d1 * d1 + glowR) * 0.3;
        float glow2 = glowR / (d2 * d2 + glowR) * 0.3;
        float glow3 = glowR / (d3 * d3 + glowR) * 0.3;

        // Combine: solid core + soft glow halo
        col += color1.rgb * (core1 + glow1) * proxFade;
        col += color2.rgb * (core2 + glow2) * proxFade;
        col += color3.rgb * (core3 + glow3) * proxFade;

        // Audio bass makes marks flare
        col *= 1.0 + audioBass * 1.5;
    }

    return vec4(clamp(col, 0.0, 1.0), 1.0);
}

// ═══════════════════════════════════════════════════════════════════════
// PASS 1: Final output — read feedback, subtle shimmer
// ═══════════════════════════════════════════════════════════════════════

vec4 passRender() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = texture2D(fbBuf, uv).rgb;

    // Subtle neon shimmer
    float shimmer = 0.5 + 0.5 * sin(uv.x * 80.0 + TIME * 4.0) * sin(uv.y * 60.0 - TIME * 3.0);
    col *= 0.96 + 0.04 * shimmer;

    // Audio sparkle on high frequencies
    col *= 1.0 + audioHigh * 0.1 * sin(TIME * 10.0 + uv.x * 30.0);

    col = clamp(col, 0.0, 1.0);

    if (transparentBg) {
        float a = max(col.r, max(col.g, col.b));
        return vec4(col, a);
    }
    return vec4(col, 1.0);
}

void main() {
    if (PASSINDEX == 0) gl_FragColor = passFeedback();
    else gl_FragColor = passRender();
}
