/*
{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Lazer Claw — three T-Rex claw slash marks that follow mouse movement with neon trails",
  "INPUTS": [
    { "NAME": "clawSize", "LABEL": "Size", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.05, "MAX": 0.8 },
    { "NAME": "spread", "LABEL": "Spread", "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.01, "MAX": 0.2 },
    { "NAME": "thickness", "LABEL": "Thickness", "TYPE": "float", "DEFAULT": 0.008, "MIN": 0.002, "MAX": 0.03 },
    { "NAME": "glowWidth", "LABEL": "Glow", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "curvature", "LABEL": "Curve", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "feedback", "LABEL": "Trail", "TYPE": "float", "DEFAULT": 0.93, "MIN": 0.5, "MAX": 0.99 },
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

// T-Rex claw slash SDF — curved tapered scratch mark
// Starts thick at top, curves and tapers to a sharp point at bottom
float clawSlash(vec2 p, float len, float w, float curve) {
    // Parameterize along the slash (t=0 top, t=1 tip)
    float t = clamp((p.y + len) / (2.0 * len), 0.0, 1.0);

    // Curved path — claw arcs outward
    float xOff = curve * sin(t * 3.14159) * len;
    float dx = p.x - xOff;

    // Taper: thick at entry, razor-thin at tip (T-Rex claw shape)
    float taper = w * (1.0 - t * t * t) * (0.3 + 0.7 * smoothstep(0.0, 0.15, t));

    // Distance
    float dy = abs(p.y) - len;
    float distX = abs(dx) - taper;
    if (dy < 0.0 && distX < 0.0) return max(distX, -0.001);
    if (dy < 0.0) return distX;
    return length(max(vec2(distX, dy), 0.0));
}

vec2 rot2d(vec2 p, float a) {
    float c = cos(a), s = sin(a);
    return vec2(p.x * c - p.y * s, p.x * s + p.y * c);
}

// ═══════════════════════════════════════════════════════════════════════
// PASS 0: Feedback buffer — decay + paint claw slashes at mouse
// ═══════════════════════════════════════════════════════════════════════

vec4 passFeedback() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Decay previous frame
    vec3 col = vec3(0.0);
    if (FRAMEINDEX > 0) {
        col = texture2D(fbBuf, uv).rgb * feedback;
        // Ember fade — red persists longer
        col.r *= 1.002;
        col.gb *= 0.997;
    }

    vec2 mPos = mousePos;
    vec2 delta = mouseDelta;

    // Slash direction from mouse movement
    float moveLen = length(delta);
    float slashAngle = moveLen > 0.001 ? atan(delta.y, delta.x * aspect) : -0.7854; // default diagonal

    vec2 toMouse = uv - mPos;
    toMouse.x *= aspect;
    float mouseDist = length(toMouse);

    float drawRadius = clawSize * 1.5;
    if (mouseDist < drawRadius && (mouseDown > 0.5 || moveLen > 0.002)) {
        // Rotate into slash-aligned space
        // Claws slash perpendicular to movement — rotated 90 degrees
        float ang = slashAngle + 1.5708;
        vec2 local = rot2d(toMouse, -ang);

        float len = clawSize * 0.5;
        float w = thickness;
        float crv = curvature * 0.3;

        // Three parallel claw marks — center one longest, outer ones shorter and curved more
        float sp = spread;
        float d1 = clawSlash(local + vec2(-sp, 0.0), len * 0.85, w * 0.7, crv * 1.4);
        float d2 = clawSlash(local, len, w, crv);
        float d3 = clawSlash(local + vec2(sp, 0.0), len * 0.85, w * 0.7, -crv * 1.4);

        // Proximity fade — soft edge
        float proxFade = smoothstep(drawRadius, drawRadius * 0.3, mouseDist);

        // Movement intensity — faster = brighter
        float intensity = smoothstep(0.001, 0.02, moveLen) * 0.7 + 0.3;

        // Neon core + glow
        float glowR = max(0.001, glowWidth * 0.015);

        float core1 = smoothstep(0.001, -0.002, d1);
        float core2 = smoothstep(0.001, -0.002, d2);
        float core3 = smoothstep(0.001, -0.002, d3);

        float glow1 = glowR / (d1 * d1 + glowR) * 0.25;
        float glow2 = glowR / (d2 * d2 + glowR) * 0.25;
        float glow3 = glowR / (d3 * d3 + glowR) * 0.25;

        col += color1.rgb * (core1 + glow1) * proxFade * intensity;
        col += color2.rgb * (core2 + glow2) * proxFade * intensity;
        col += color3.rgb * (core3 + glow3) * proxFade * intensity;

        // Audio bass makes marks flare
        col *= 1.0 + audioBass * 2.0;
    }

    return vec4(clamp(col, 0.0, 1.0), 1.0);
}

// ═══════════════════════════════════════════════════════════════════════
// PASS 1: Final output — shimmer + spark effects
// ═══════════════════════════════════════════════════════════════════════

vec4 passRender() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = texture2D(fbBuf, uv).rgb;

    // Heat shimmer along claw marks
    float shimmer = 0.5 + 0.5 * sin(uv.x * 60.0 + TIME * 5.0) * sin(uv.y * 40.0 - TIME * 3.0);
    col *= 0.97 + 0.03 * shimmer;

    // Edge spark effect — bright pixels get extra sparkle
    float bright = max(col.r, max(col.g, col.b));
    float spark = sin(TIME * 15.0 + uv.x * 50.0) * sin(TIME * 12.0 + uv.y * 50.0);
    col += vec3(1.0, 0.8, 0.6) * bright * bright * spark * 0.08;

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
