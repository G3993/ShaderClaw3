/*{
    "DESCRIPTION": "Fauvism Color Fields — Matisse-inspired biomorphic shapes with black ink outlines",
    "CREDIT": "ShaderClaw auto-improve v2",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "Art"],
    "INPUTS": [
        {
            "NAME": "audioReact",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "motionSpeed",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "inkWidth",
            "TYPE": "float",
            "DEFAULT": 0.014,
            "MIN": 0.003,
            "MAX": 0.05
        }
    ]
}*/

precision highp float;

// ---- Palette (exactly 5 chosen colors) ----
const vec3 COL_COBALT = vec3(0.0,  0.28, 0.67);   // deep cobalt blue
const vec3 COL_VERMIL = vec3(1.0,  0.14, 0.0);    // pure vermillion
const vec3 COL_YELLOW = vec3(1.0,  0.87, 0.0);    // cadmium yellow
const vec3 COL_GREEN  = vec3(0.13, 0.55, 0.13);   // forest green
const vec3 COL_BLACK  = vec3(0.0,  0.0,  0.0);    // ink black

// ---- SDF helpers ----
float sdEllipse(vec2 p, vec2 ab) {
    // approximate SDF for ellipse
    p = abs(p);
    if (p.x > p.y) { p = p.yx; ab = ab.yx; }
    float l = ab.y * ab.y - ab.x * ab.x;
    float m = ab.x * p.x / l;
    float n = ab.y * p.y / l;
    float m2 = m * m;
    float n2 = n * n;
    float c = (m2 + n2 - 1.0) / 3.0;
    float c3 = c * c * c;
    float q = c3 + m2 * n2 * 2.0;
    float d = c3 + m2 * n2;
    float g = m + m * n2;
    float co;
    if (d < 0.0) {
        float h = acos(q / c3) / 3.0;
        float s = cos(h);
        float t2 = sin(h) * sqrt(3.0);
        float rx = sqrt(-c * (s + t2 + 2.0) + m2);
        float ry = sqrt(-c * (s - t2 + 2.0) + m2);
        co = (ry + sign(l) * rx + abs(g) / (rx * ry) - m) / 2.0;
    } else {
        float h = 2.0 * m * n * sqrt(d);
        float s = sign(q + h) * pow(abs(q + h), 1.0 / 3.0);
        float u = sign(q - h) * pow(abs(q - h), 1.0 / 3.0);
        float rx = -s - u - c * 4.0 + 2.0 * m2;
        float ry = (s - u) * sqrt(3.0);
        float rm = sqrt(rx * rx + ry * ry);
        co = (ry / sqrt(rm - rx) + 2.0 * g / rm - m) / 2.0;
    }
    vec2 r2 = ab * vec2(co, sqrt(1.0 - co * co));
    return length(r2 - p) * sign(p.y - r2.y);
}

float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}

// ---- Main ----
void main() {
    vec2 uv = isf_FragNormCoord;
    // aspect-corrected canvas coords, centered
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);

    // Audio modulator
    float audio = 0.5 + 0.5 * audioBass * audioReact;

    float t = TIME * motionSpeed;

    // ---- Define 6 biomorphic shapes (back to front) ----
    // Each shape: center drift, dimensions, color

    // Shape 1 — large cobalt ellipse (background fill, back layer)
    vec2 c1 = vec2(sin(t * 0.31) * 0.15, cos(t * 0.27) * 0.08);
    vec2 ab1 = vec2(0.55, 0.38) * (1.0 + 0.1 * (audio - 0.5));
    float d1 = sdEllipse(p - c1, ab1);

    // Shape 2 — vermillion rounded box
    vec2 c2 = vec2(-0.25 + sin(t * 0.19) * 0.12, 0.05 + cos(t * 0.23) * 0.10);
    vec2 ab2 = vec2(0.28, 0.20) * (1.0 + 0.1 * (audio - 0.5));
    float r2 = 0.07;
    float d2 = sdRoundBox(p - c2, ab2, r2);

    // Shape 3 — yellow ellipse
    vec2 c3 = vec2(0.22 + cos(t * 0.17) * 0.10, -0.08 + sin(t * 0.29) * 0.09);
    vec2 ab3 = vec2(0.22, 0.32) * (1.0 + 0.1 * (audio - 0.5));
    float d3 = sdEllipse(p - c3, ab3);

    // Shape 4 — green rounded box
    vec2 c4 = vec2(0.10 + sin(t * 0.13) * 0.08, 0.18 + cos(t * 0.37) * 0.07);
    vec2 ab4 = vec2(0.18, 0.14) * (1.0 + 0.1 * (audio - 0.5));
    float r4 = 0.09;
    float d4 = sdRoundBox(p - c4, ab4, r4);

    // Shape 5 — vermillion ellipse (smaller accent)
    vec2 c5 = vec2(-0.28 + cos(t * 0.41) * 0.06, -0.15 + sin(t * 0.22) * 0.11);
    vec2 ab5 = vec2(0.14, 0.20) * (1.0 + 0.1 * (audio - 0.5));
    float d5 = sdEllipse(p - c5, ab5);

    // Shape 6 — yellow rounded box (top-right)
    vec2 c6 = vec2(0.32 + sin(t * 0.26) * 0.07, 0.15 + cos(t * 0.34) * 0.06);
    vec2 ab6 = vec2(0.16, 0.10) * (1.0 + 0.1 * (audio - 0.5));
    float r6 = 0.05;
    float d6 = sdRoundBox(p - c6, ab6, r6);

    // ---- Composite back-to-front ----
    // Start with background
    vec3 col = COL_COBALT * 2.5;

    // Helper: ink border mask
    // shape fill + ink outline
    float eps = 0.001;

    // Shape 1 fill (cobalt — same as BG but slightly brighter border visible)
    float fill1 = 1.0 - smoothstep(-eps, eps, d1);
    col = mix(col, COL_COBALT * 2.5, fill1);
    float ink1 = smoothstep(fwidth(d1) * 2.0, 0.0, abs(d1) - inkWidth);
    col = mix(col, COL_BLACK, ink1);

    // Shape 2 fill (vermillion)
    float fill2 = 1.0 - smoothstep(-eps, eps, d2);
    col = mix(col, COL_VERMIL * 2.5, fill2);
    float ink2 = smoothstep(fwidth(d2) * 2.0, 0.0, abs(d2) - inkWidth);
    col = mix(col, COL_BLACK, ink2);

    // Shape 3 fill (yellow)
    float fill3 = 1.0 - smoothstep(-eps, eps, d3);
    col = mix(col, COL_YELLOW * 2.5, fill3);
    float ink3 = smoothstep(fwidth(d3) * 2.0, 0.0, abs(d3) - inkWidth);
    col = mix(col, COL_BLACK, ink3);

    // Shape 4 fill (green)
    float fill4 = 1.0 - smoothstep(-eps, eps, d4);
    col = mix(col, COL_GREEN * 2.5, fill4);
    float ink4 = smoothstep(fwidth(d4) * 2.0, 0.0, abs(d4) - inkWidth);
    col = mix(col, COL_BLACK, ink4);

    // Shape 5 fill (vermillion accent)
    float fill5 = 1.0 - smoothstep(-eps, eps, d5);
    col = mix(col, COL_VERMIL * 2.5, fill5);
    float ink5 = smoothstep(fwidth(d5) * 2.0, 0.0, abs(d5) - inkWidth);
    col = mix(col, COL_BLACK, ink5);

    // Shape 6 fill (yellow accent)
    float fill6 = 1.0 - smoothstep(-eps, eps, d6);
    col = mix(col, COL_YELLOW * 2.5, fill6);
    float ink6 = smoothstep(fwidth(d6) * 2.0, 0.0, abs(d6) - inkWidth);
    col = mix(col, COL_BLACK, ink6);

    // Output LINEAR HDR — no clamp, no ACES, no gamma
    gl_FragColor = vec4(col, 1.0);
}
