/*
{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Lazer Claw — three razor-sharp claw scar marks that tear across the screen as you drag",
  "INPUTS": [
    { "NAME": "clawWidth", "LABEL": "Width", "TYPE": "float", "DEFAULT": 0.008, "MIN": 0.002, "MAX": 0.03 },
    { "NAME": "spread", "LABEL": "Spread", "TYPE": "float", "DEFAULT": 0.04, "MIN": 0.01, "MAX": 0.15 },
    { "NAME": "glowWidth", "LABEL": "Glow", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0 },
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

// Distance from point to line segment
float sdSeg(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

// Claw scratch — tapered line (thick at start, thin at tip)
float clawMark(vec2 p, vec2 a, vec2 b, float w, float curve) {
    vec2 ba = b - a;
    float segLen = length(ba);
    if (segLen < 0.0001) return length(p - a) - w;

    vec2 dir = ba / segLen;
    vec2 perp = vec2(-dir.y, dir.x);
    vec2 pa = p - a;
    float along = dot(pa, dir);
    float t = clamp(along / segLen, 0.0, 1.0);

    // Curve offset
    float curveOff = curve * sin(t * 3.14159) * segLen * 0.12;
    vec2 closest = a + dir * (t * segLen) + perp * curveOff;
    float dist = length(p - closest);

    // Taper: thick at entry, razor at tip
    float taper = w * (1.0 - t * t) * (0.2 + 0.8 * smoothstep(0.0, 0.1, t));
    return dist - taper;
}

vec4 passFeedback() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    vec3 col = vec3(0.0);
    if (FRAMEINDEX > 0) {
        col = texture2D(fbBuf, uv).rgb * feedback;
        col.r *= 1.001;
        col.gb *= 0.998;
    }

    vec2 mNow = mousePos;
    vec2 delta = mouseDelta;

    // Amplify delta for longer visible strokes
    float moveLen = length(delta);
    vec2 mPrev = mNow - delta * 3.0;

    // Draw when mouse moves OR mouse is down (for touch drag)
    if (moveLen > 0.0003 || mouseDown > 0.5) {
        vec2 uvA = vec2(uv.x * aspect, uv.y);
        vec2 a = vec2(mPrev.x * aspect, mPrev.y);
        vec2 b = vec2(mNow.x * aspect, mNow.y);

        vec2 moveDir = length(b - a) > 0.0001 ? normalize(b - a) : vec2(0.707, 0.707);
        vec2 spreadDir = vec2(-moveDir.y, moveDir.x);

        float w = clawWidth;
        float sp = spread;

        float d1 = clawMark(uvA, a - spreadDir * sp, b - spreadDir * sp, w * 0.7, 1.3);
        float d2 = clawMark(uvA, a, b, w, 0.0);
        float d3 = clawMark(uvA, a + spreadDir * sp, b + spreadDir * sp, w * 0.7, -1.3);

        float intensity = smoothstep(0.0002, 0.008, moveLen) * 0.8 + 0.2;

        float glowR = max(0.0003, glowWidth * 0.008);

        float core1 = smoothstep(0.002, -0.002, d1);
        float core2 = smoothstep(0.002, -0.002, d2);
        float core3 = smoothstep(0.002, -0.002, d3);

        float glow1 = d1 > 0.0 ? glowR / (d1 * d1 + glowR) * 0.15 : 0.0;
        float glow2 = d2 > 0.0 ? glowR / (d2 * d2 + glowR) * 0.15 : 0.0;
        float glow3 = d3 > 0.0 ? glowR / (d3 * d3 + glowR) * 0.15 : 0.0;

        col += color1.rgb * (core1 + glow1) * intensity;
        col += color2.rgb * (core2 + glow2) * intensity;
        col += color3.rgb * (core3 + glow3) * intensity;

        col *= 1.0 + audioBass * 2.0;
    }

    return vec4(clamp(col, 0.0, 1.0), 1.0);
}

vec4 passRender() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = texture2D(fbBuf, uv).rgb;

    float bright = max(col.r, max(col.g, col.b));
    float shimmer = sin(uv.x * 60.0 + TIME * 5.0) * sin(uv.y * 40.0 - TIME * 3.5);
    col *= 1.0 + shimmer * bright * 0.03;

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
