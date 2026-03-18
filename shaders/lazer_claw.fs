/*
{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Lazer Claw — three razor-sharp claw scar marks that tear across the screen as you drag",
  "INPUTS": [
    { "NAME": "clawWidth", "LABEL": "Width", "TYPE": "float", "DEFAULT": 0.006, "MIN": 0.002, "MAX": 0.025 },
    { "NAME": "spread", "LABEL": "Spread", "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.01, "MAX": 0.15 },
    { "NAME": "glowWidth", "LABEL": "Glow", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "feedback", "LABEL": "Trail", "TYPE": "float", "DEFAULT": 0.96, "MIN": 0.5, "MAX": 0.995 },
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

// Distance from point to line segment (a→b), returns distance and t (0-1 along segment)
float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

// Claw scratch: line segment with taper (thick at start, razor tip at end)
// and slight curve offset for organic feel
float clawScratch(vec2 p, vec2 a, vec2 b, float w, float curveAmt) {
    vec2 ba = b - a;
    float segLen = length(ba);
    if (segLen < 0.001) return 999.0;

    vec2 dir = ba / segLen;
    vec2 perp = vec2(-dir.y, dir.x);

    // Project point onto segment
    vec2 pa = p - a;
    float along = dot(pa, dir);
    float t = clamp(along / segLen, 0.0, 1.0);

    // Curve: offset perpendicular based on position along scratch
    float curveOff = curveAmt * sin(t * 3.14159) * segLen * 0.15;
    vec2 closest = a + dir * (t * segLen) + perp * curveOff;

    float dist = length(p - closest);

    // Taper: thick at entry (t=0), thin at tip (t=1)
    // Shape like a real claw: widest at 15%, tapers to nothing
    float taper = w * smoothstep(0.0, 0.15, t) * (1.0 - t * t);

    return dist - taper;
}

// ═══════════════════════════════════════════════════════════════════════
// PASS 0: Feedback — persistent scars + new scratch segments
// ═══════════════════════════════════════════════════════════════════════

vec4 passFeedback() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Read + decay
    vec3 col = vec3(0.0);
    if (FRAMEINDEX > 0) {
        col = texture2D(fbBuf, uv).rgb * feedback;
        // Scars cool from white → red → dark
        col.r *= 1.001;
        col.gb *= 0.998;
    }

    // Current mouse segment: from (mousePos - mouseDelta) to mousePos
    vec2 mNow = mousePos;
    vec2 mPrev = mNow - mouseDelta;
    float moveLen = length(mouseDelta);

    // Only draw when mouse is moving (dragging creates scars)
    if (moveLen > 0.001) {
        // Aspect-correct coordinates
        vec2 uvA = vec2(uv.x * aspect, uv.y);
        vec2 a = vec2(mPrev.x * aspect, mPrev.y);
        vec2 b = vec2(mNow.x * aspect, mNow.y);

        // Movement direction perpendicular = spread direction
        vec2 moveDir = normalize(b - a);
        vec2 spreadDir = vec2(-moveDir.y, moveDir.x);

        float w = clawWidth;
        float sp = spread;

        // Three parallel claw scratches offset perpendicular to movement
        float d1 = clawScratch(uvA, a - spreadDir * sp, b - spreadDir * sp, w * 0.7, 1.2);
        float d2 = clawScratch(uvA, a, b, w, 0.0);
        float d3 = clawScratch(uvA, a + spreadDir * sp, b + spreadDir * sp, w * 0.7, -1.2);

        // Intensity scales with speed
        float intensity = smoothstep(0.001, 0.015, moveLen);

        // Neon glow
        float glowR = max(0.0005, glowWidth * 0.01);

        // Hard bright core where inside the scratch
        float core1 = smoothstep(0.001, -0.001, d1);
        float core2 = smoothstep(0.001, -0.001, d2);
        float core3 = smoothstep(0.001, -0.001, d3);

        // Soft glow falloff outside
        float glow1 = d1 > 0.0 ? glowR / (d1 * d1 + glowR) * 0.2 : 0.0;
        float glow2 = d2 > 0.0 ? glowR / (d2 * d2 + glowR) * 0.2 : 0.0;
        float glow3 = d3 > 0.0 ? glowR / (d3 * d3 + glowR) * 0.2 : 0.0;

        col += color1.rgb * (core1 + glow1) * intensity;
        col += color2.rgb * (core2 + glow2) * intensity;
        col += color3.rgb * (core3 + glow3) * intensity;

        // Audio bass flare
        col *= 1.0 + audioBass * 2.0;
    }

    return vec4(clamp(col, 0.0, 1.0), 1.0);
}

// ═══════════════════════════════════════════════════════════════════════
// PASS 1: Final output — heat distortion + sparks
// ═══════════════════════════════════════════════════════════════════════

vec4 passRender() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = texture2D(fbBuf, uv).rgb;

    // Subtle heat shimmer on bright areas
    float bright = max(col.r, max(col.g, col.b));
    float shimmer = sin(uv.x * 80.0 + TIME * 6.0) * sin(uv.y * 60.0 - TIME * 4.0);
    col *= 1.0 + shimmer * bright * 0.04;

    // Micro sparks along scar edges
    float spark = sin(TIME * 20.0 + uv.x * 100.0) * sin(TIME * 17.0 + uv.y * 80.0);
    col += vec3(1.0, 0.7, 0.5) * bright * bright * max(spark, 0.0) * 0.1;

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
