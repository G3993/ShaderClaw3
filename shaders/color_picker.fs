/*{
  "DESCRIPTION": "Fauvism Color Fields — Matisse-inspired biomorphic shapes with black ink outlines",
  "CATEGORIES": ["Generator", "Art"],
  "INPUTS": [
    { "NAME": "audioReact",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8 },
    { "NAME": "motionSpeed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.4 },
    { "NAME": "inkWidth",    "TYPE": "float", "MIN": 0.003, "MAX": 0.05, "DEFAULT": 0.014 }
  ]
}*/

precision highp float;

// ---------------------------------------------------------------------------
// PALETTE — exactly 5 hand-chosen Fauvist colors, no others
// ---------------------------------------------------------------------------
#define COL_COBALT  vec3(0.0,  0.28, 0.67)   // deep cobalt blue
#define COL_VERMIL  vec3(1.0,  0.14, 0.0)    // pure vermillion
#define COL_YELLOW  vec3(1.0,  0.87, 0.0)    // cadmium yellow
#define COL_GREEN   vec3(0.13, 0.55, 0.13)   // forest green
#define COL_BLACK   vec3(0.0,  0.0,  0.0)    // ink black

// HDR fill scale — never clamp
#define HDR 2.5

// ---------------------------------------------------------------------------
// SDF primitives
// ---------------------------------------------------------------------------
float sdEllipse(vec2 p, vec2 ab) {
    // exact ellipse SDF approximation (Quilez)
    p = abs(p);
    if (p.x > p.y) { p = p.yx; ab = ab.yx; }
    float l = ab.y * ab.y - ab.x * ab.x;
    float m = ab.x * p.x / l;
    float n = ab.y * p.y / l;
    float m2 = m * m, n2 = n * n;
    float c = (m2 + n2 - 1.0) / 3.0;
    float c3 = c * c * c;
    float d = c3 + m2 * n2;
    float q = d < 0.0
        ? 2.0 * cos(atan(sqrt(-d / c3)) / 3.0) * sqrt(-c) - 1.0 / 3.0 / c
        : pow(sqrt(d) + sqrt(m2 * n2), 1.0 / 3.0)
          + pow(abs(sqrt(d) - sqrt(m2 * n2)), 1.0 / 3.0) * sign(d - sqrt(m2 * n2))
          - 1.0 / 3.0 / c;
    vec2 uv2 = ab * vec2(m, n) / q;
    return length(uv2 - p) * sign(p.y - uv2.y);
}

float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

// ---------------------------------------------------------------------------
// Audio modulator — ±10% scale variation
// ---------------------------------------------------------------------------
float audioMod() {
    return 0.5 + 0.5 * audioBass * audioReact;
}

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------
void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);

    float t   = TIME * motionSpeed;
    float mod = audioMod();   // 0.0 .. 1.0, centres at 0.5

    // Scale modulator: shapes grow/shrink ±10%
    float scl = 1.0 + 0.1 * (mod - 0.5) * 2.0;   // 0.9 .. 1.1

    // ---------------------------------------------------------------------------
    // Background — cobalt blue at HDR
    // ---------------------------------------------------------------------------
    vec3 col = COL_COBALT * HDR;

    // ---------------------------------------------------------------------------
    // Helper macro: draw one shape back-to-front.
    // Each shape has its own time offset, drift frequency and fill color.
    // ---------------------------------------------------------------------------

    // Shape 1 — large vermillion rounded box, drifts slowly top-left area
    {
        float ox = sin(t * 0.31 + 0.0) * 0.18;
        float oy = cos(t * 0.27 + 1.1) * 0.14;
        vec2  c  = vec2(-0.32 + ox, 0.18 + oy);
        float d  = sdRoundBox(p - c, vec2(0.26, 0.19) * scl, 0.07 * scl);
        float fw = fwidth(d);
        // fill
        float fill = smoothstep(fw, -fw, d);
        col = mix(col, COL_VERMIL * HDR, fill);
        // ink border
        float ink = smoothstep(fw * 2.0, 0.0, abs(d) - inkWidth);
        col = mix(col, COL_BLACK, ink);
    }

    // Shape 2 — cadmium yellow ellipse, right-centre
    {
        float ox = sin(t * 0.19 + 2.3) * 0.15;
        float oy = cos(t * 0.23 + 0.7) * 0.16;
        vec2  c  = vec2(0.28 + ox, -0.10 + oy);
        float d  = sdEllipse(p - c, vec2(0.30, 0.18) * scl);
        float fw = fwidth(d);
        float fill = smoothstep(fw, -fw, d);
        col = mix(col, COL_YELLOW * HDR, fill);
        float ink = smoothstep(fw * 2.0, 0.0, abs(d) - inkWidth);
        col = mix(col, COL_BLACK, ink);
    }

    // Shape 3 — forest green rounded box, lower-left
    {
        float ox = cos(t * 0.37 + 3.5) * 0.13;
        float oy = sin(t * 0.29 + 1.9) * 0.12;
        vec2  c  = vec2(-0.20 + ox, -0.25 + oy);
        float d  = sdRoundBox(p - c, vec2(0.22, 0.16) * scl, 0.09 * scl);
        float fw = fwidth(d);
        float fill = smoothstep(fw, -fw, d);
        col = mix(col, COL_GREEN * HDR, fill);
        float ink = smoothstep(fw * 2.0, 0.0, abs(d) - inkWidth);
        col = mix(col, COL_BLACK, ink);
    }

    // Shape 4 — vermillion ellipse, upper-right
    {
        float ox = sin(t * 0.43 + 4.1) * 0.12;
        float oy = cos(t * 0.17 + 2.8) * 0.17;
        vec2  c  = vec2(0.10 + ox, 0.27 + oy);
        float d  = sdEllipse(p - c, vec2(0.18, 0.28) * scl);
        float fw = fwidth(d);
        float fill = smoothstep(fw, -fw, d);
        col = mix(col, COL_VERMIL * HDR, fill);
        float ink = smoothstep(fw * 2.0, 0.0, abs(d) - inkWidth);
        col = mix(col, COL_BLACK, ink);
    }

    // Shape 5 — cadmium yellow rounded box, bottom-right / centre overlap
    {
        float ox = cos(t * 0.25 + 5.7) * 0.14;
        float oy = sin(t * 0.33 + 0.3) * 0.11;
        vec2  c  = vec2(0.05 + ox, -0.08 + oy);
        float d  = sdRoundBox(p - c, vec2(0.19, 0.24) * scl, 0.06 * scl);
        float fw = fwidth(d);
        float fill = smoothstep(fw, -fw, d);
        col = mix(col, COL_YELLOW * HDR, fill);
        float ink = smoothstep(fw * 2.0, 0.0, abs(d) - inkWidth);
        col = mix(col, COL_BLACK, ink);
    }

    // Shape 6 — forest green ellipse, top-centre, overlapping others
    {
        float ox = sin(t * 0.21 + 6.2) * 0.10;
        float oy = cos(t * 0.38 + 3.3) * 0.09;
        vec2  c  = vec2(-0.04 + ox, 0.20 + oy);
        float d  = sdEllipse(p - c, vec2(0.26, 0.13) * scl);
        float fw = fwidth(d);
        float fill = smoothstep(fw, -fw, d);
        col = mix(col, COL_GREEN * HDR, fill);
        float ink = smoothstep(fw * 2.0, 0.0, abs(d) - inkWidth);
        col = mix(col, COL_BLACK, ink);
    }

    // ---------------------------------------------------------------------------
    // Output — linear HDR, no clamp, no ACES, no gamma
    // ---------------------------------------------------------------------------
    gl_FragColor = vec4(col, 1.0);
}
