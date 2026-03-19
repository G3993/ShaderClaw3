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

// Simplified claw mark — distance to line segment with taper
float clawMark(vec2 p, vec2 a, vec2 b, float w) {
    vec2 ba = b - a;
    float l2 = dot(ba, ba);
    if (l2 < 0.00001) return length(p - a) - w;
    float t = clamp(dot(p - a, ba) / l2, 0.0, 1.0);
    float dist = length(p - a - ba * t);
    // Taper: thick at start, thin at end
    float taper = w * (1.0 - t * t);
    return dist - taper;
}

vec4 passFeedback() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Decay previous
    vec3 col = vec3(0.0);
    if (FRAMEINDEX > 0) {
        col = texture2D(fbBuf, uv).rgb * feedback;
    }

    vec2 mNow = mousePos;
    vec2 delta = mouseDelta;
    float moveLen = length(delta);
    vec2 mPrev = mNow - delta * 3.0;

    if (moveLen > 0.0003 || mouseDown > 0.5) {
        vec2 uvA = vec2(uv.x * aspect, uv.y);
        vec2 a = vec2(mPrev.x * aspect, mPrev.y);
        vec2 b = vec2(mNow.x * aspect, mNow.y);

        // Quick distance check — skip pixels far from the stroke
        vec2 mid = (a + b) * 0.5;
        float reach = length(b - a) * 0.5 + spread + clawWidth * 2.0 + glowWidth * 0.05;
        if (length(uvA - mid) < reach) {
            vec2 moveDir = length(b - a) > 0.0001 ? normalize(b - a) : vec2(0.707, 0.707);
            vec2 sp = vec2(-moveDir.y, moveDir.x) * spread;

            float d1 = clawMark(uvA, a - sp, b - sp, clawWidth * 0.7);
            float d2 = clawMark(uvA, a, b, clawWidth);
            float d3 = clawMark(uvA, a + sp, b + sp, clawWidth * 0.7);

            float intensity = smoothstep(0.0002, 0.008, moveLen) * 0.8 + 0.2;
            float glowR = max(0.0003, glowWidth * 0.008);

            float c1 = smoothstep(0.002, -0.002, d1);
            float c2 = smoothstep(0.002, -0.002, d2);
            float c3 = smoothstep(0.002, -0.002, d3);

            float g1 = d1 > 0.0 ? glowR / (d1 * d1 + glowR) * 0.15 : 0.0;
            float g2 = d2 > 0.0 ? glowR / (d2 * d2 + glowR) * 0.15 : 0.0;
            float g3 = d3 > 0.0 ? glowR / (d3 * d3 + glowR) * 0.15 : 0.0;

            col += color1.rgb * (c1 + g1) * intensity;
            col += color2.rgb * (c2 + g2) * intensity;
            col += color3.rgb * (c3 + g3) * intensity;

            col *= 1.0 + audioBass * 2.0;
        }
    }

    return vec4(clamp(col, 0.0, 1.0), 1.0);
}

vec4 passRender() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec3 col = texture2D(fbBuf, uv).rgb;

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
