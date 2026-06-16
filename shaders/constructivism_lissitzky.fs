/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Russian Constructivism / Suprematism — Malevich, Lissitzky, Kassák, Bauhaus poster geometry. N animated primary-color shapes (squares, triangles, circles, arcs, rectangles, half-rounds) drift across cream paper in diagonal composition; black accent forms; subtle paper grain. Lithograph palette discipline. Beat the Whites with the Red Wedge.",
  "INPUTS": [
    {"NAME":"shapeCount",      "LABEL":"Shape Count",   "TYPE":"float","MIN":4.0, "MAX":24.0, "DEFAULT":14.0},
    {"NAME":"motion",          "LABEL":"Motion",        "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.30},
    {"NAME":"compositionTilt", "LABEL":"Diagonal Tilt", "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.55},
    {"NAME":"redWedge",        "LABEL":"Red Wedge",     "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":1.0},
    {"NAME":"wedgeThrust",     "LABEL":"Wedge Thrust",  "TYPE":"float","MIN":0.0, "MAX":0.4,  "DEFAULT":0.15},
    {"NAME":"whiteCircle",     "LABEL":"White Circle",  "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":1.0},
    {"NAME":"barCount",        "LABEL":"Black Bars",    "TYPE":"float","MIN":0.0, "MAX":6.0,  "DEFAULT":3.0},
    {"NAME":"glyphIntensity",  "LABEL":"Cyrillic Glyphs","TYPE":"float","MIN":0.0,"MAX":1.0,  "DEFAULT":0.45},
    {"NAME":"paperGrain",      "LABEL":"Paper Grain",   "TYPE":"float","MIN":0.0, "MAX":0.10, "DEFAULT":0.025},
    {"NAME":"paletteShift",    "LABEL":"Palette Shift", "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.0},
    {"NAME":"audioReact",      "LABEL":"Audio React",   "TYPE":"float","MIN":0.0, "MAX":2.0,  "DEFAULT":1.0},
    {"NAME":"inputTex",        "LABEL":"Texture",       "TYPE":"image"}
  ]
}*/

// Constructivist palette — disciplined lithograph primaries. Departure
// from these collapses the read.
const vec3 CR_CREAM  = vec3(0.96, 0.92, 0.82);
const vec3 CR_RED    = vec3(0.89, 0.12, 0.14);
const vec3 CR_BLACK  = vec3(0.10, 0.08, 0.10);
const vec3 CR_WHITE  = vec3(0.99, 0.98, 0.94);
const vec3 CR_YELLOW = vec3(0.98, 0.78, 0.12);
const vec3 CR_BLUE   = vec3(0.10, 0.30, 0.78);
const vec3 CR_GREEN  = vec3(0.18, 0.50, 0.28);
const vec3 CR_ORANGE = vec3(0.95, 0.45, 0.10);

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

vec3 crPalette(float idx) {
    int i = int(mod(idx + paletteShift * 8.0, 8.0));
    if (i == 0) return CR_RED;
    if (i == 1) return CR_BLACK;
    if (i == 2) return CR_YELLOW;
    if (i == 3) return CR_BLUE;
    if (i == 4) return CR_WHITE;
    if (i == 5) return CR_ORANGE;
    if (i == 6) return CR_GREEN;
    return CR_RED;
}

// SDF primitives
float sdBox(vec2 p, vec2 b) {
    vec2 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}
float sdCircle(vec2 p, float r) { return length(p) - r; }
float sdRoundedBox(vec2 p, vec2 b, float r) {
    vec2 d = abs(p) - b + r;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - r;
}
float sdTriangle(vec2 p) {
    // Equilateral triangle pointing up, side length 1
    const float k = sqrt(3.0);
    p.x = abs(p.x) - 0.5;
    p.y = p.y + 0.5 / k;
    if (p.x + k * p.y > 0.0) p = vec2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
    p.x -= clamp(p.x, -1.0, 0.0);
    return -length(p) * sign(p.y);
}
// Half-disc (the Kassák / Lissitzky arc shape)
float sdArc(vec2 p, float ra, float rb, float angle) {
    p = mat2(cos(angle), sin(angle), -sin(angle), cos(angle)) * p;
    p.x = abs(p.x);
    return max(length(p) - ra, ra - rb - length(p));
}
// Long thin rectangle (constructivist bar)
float sdBar(vec2 p, float len_, float thick, float angle) {
    p = mat2(cos(angle), sin(angle), -sin(angle), cos(angle)) * p;
    return sdBox(p, vec2(len_, thick));
}
// Wedge — isosceles triangle pointing along direction
float sdWedge(vec2 p, vec2 dir, float len_, float halfWidth) {
    // Rotate into wedge frame
    vec2 axis = normalize(dir);
    vec2 perp = vec2(-axis.y, axis.x);
    float along = dot(p, axis);
    float across = dot(p, perp);
    if (along < 0.0) return length(p);
    if (along > len_) return length(p - axis * len_);
    float t = along / len_;
    float halfB = halfWidth * (1.0 - t);
    return abs(across) - halfB;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 cuv = (uv - 0.5) * vec2(aspect, 1.0);

    // Cream paper with subtle grain
    vec3 col = CR_CREAM;
    col += (vnoise(uv * RENDERSIZE.x * 0.5) - 0.5) * paperGrain;

    float t = TIME * motion;

    // Background: soft diagonal cream-shading reinforces the diagonal
    // composition energy of constructivist posters.
    float diag = (cuv.x + cuv.y) * compositionTilt * 0.3;
    col *= 1.0 - diag * 0.04;

    // ── Hero: red wedge slamming through ──────────────────────────────
    if (redWedge > 0.001) {
        float wedgeAngle = 3.14159 * 0.55 + sin(t * 0.2) * 0.10;
        vec2 wDir = vec2(cos(wedgeAngle), sin(wedgeAngle));
        // Origin oscillates along the diagonal, gives the "thrust" feel
        vec2 wOrigin = vec2(-0.4 + sin(t * 0.5) * 0.05,
                             0.3 + cos(t * 0.7) * 0.05);
        float wedge = sdWedge(cuv - wOrigin, wDir, 1.4 + wedgeThrust, 0.18 + wedgeThrust * 0.5);
        col = mix(col, CR_RED, smoothstep(0.005, -0.005, wedge) * redWedge);
    }

    // ── Hero: white circle (symbol of order being struck) ──────────────
    if (whiteCircle > 0.001) {
        vec2 cPos = vec2(0.10 + sin(t * 0.3) * 0.02, 0.05 + cos(t * 0.4) * 0.02);
        float circle = sdCircle(cuv - cPos, 0.18);
        col = mix(col, CR_WHITE, smoothstep(0.004, -0.004, circle) * whiteCircle);
        // Optional video inside the circle
        if (IMG_SIZE_inputTex.x > 0.0) {
            vec3 src = texture(inputTex, uv).rgb;
            float L = dot(src, vec3(0.299, 0.587, 0.114));
            vec3 hard = vec3(step(0.5, L));
            col = mix(col, hard, smoothstep(0.004, -0.004, circle) * whiteCircle * 0.9);
        }
        // Black ring outline
        col = mix(col, CR_BLACK, smoothstep(0.008, 0.002, abs(circle)) * 0.85);
    }

    // ── N animated geometric shapes scattered across the canvas ───────
    int N = int(clamp(shapeCount, 0.0, 24.0));
    for (int i = 0; i < 24; i++) {
        if (i >= N) break;
        float fi = float(i);
        // Drift trajectory — slow diagonal float with hash-driven phase
        vec2 home = vec2(hash11(fi * 7.13) - 0.5, hash11(fi * 11.7) - 0.5) * 1.6;
        home.x *= aspect / max(aspect, 0.5);
        vec2 wobble = vec2(sin(t * 0.4 + fi * 1.1), cos(t * 0.5 + fi * 1.7)) * 0.04;
        vec2 ctr = home + wobble;

        // Per-shape rotation
        float rot = t * (0.05 + hash11(fi * 13.7) * 0.20) + fi * 0.7;
        float ca = cos(rot), sa = sin(rot);
        vec2 lp = mat2(ca, -sa, sa, ca) * (cuv - ctr);

        // Per-shape kind (0..5)
        int kind = int(hash11(fi * 17.9) * 6.0);
        float scale = 0.05 + hash11(fi * 23.1) * 0.10;
        scale *= 1.0 + audioBass * audioReact * 0.10;
        float dist;
        if      (kind == 0) dist = sdBox(lp, vec2(scale, scale * 0.5));
        else if (kind == 1) dist = sdCircle(lp, scale);
        else if (kind == 2) dist = sdTriangle(lp / scale) * scale;
        else if (kind == 3) dist = sdRoundedBox(lp, vec2(scale, scale * 0.85), scale * 0.35);
        else if (kind == 4) dist = sdArc(lp, scale, scale * 0.45, 0.0);
        else                dist = sdBar(lp, scale, scale * 0.12, 0.0);

        // Color
        float pidx = mod(fi * 2.31 + floor(t * 0.05), 8.0);
        vec3 fillCol = crPalette(pidx);

        // Fill
        col = mix(col, fillCol, smoothstep(0.005, -0.005, dist));
        // Black contour
        col = mix(col, CR_BLACK, smoothstep(0.006, 0.002, abs(dist)) * 0.55);
    }

    // ── Black bars (Lissitzky's diagonal slabs) ───────────────────────
    int B = int(clamp(barCount, 0.0, 6.0));
    for (int i = 0; i < 6; i++) {
        if (i >= B) break;
        float fi = float(i);
        float angle = (fi * 0.62 - 1.0 + sin(t * 0.1 + fi) * 0.12) * compositionTilt;
        vec2 origin = vec2(-0.3 + 0.6 * hash11(fi * 31.7),
                           -0.3 + 0.6 * hash11(fi * 37.1));
        float bar = sdBar(cuv - origin, 0.6 + 0.3 * hash11(fi * 41.3), 0.025, angle);
        col = mix(col, CR_BLACK, smoothstep(0.004, -0.004, bar));
    }

    // ── Cyrillic glyph clusters ───────────────────────────────────────
    if (glyphIntensity > 0.001) {
        // Two clusters of vertical strokes + horizontal bars (stencil look)
        for (int g = 0; g < 2; g++) {
            float fg = float(g);
            vec2 origin = vec2(0.18 + fg * 0.55, 0.10);
            vec2 ld = (uv - origin) * vec2(60.0, 30.0);
            if (ld.x >= 0.0 && ld.x <= 8.0 && ld.y >= 0.0 && ld.y <= 4.0) {
                vec2 ci = floor(ld);
                float h = hash21(ci + floor(t * (0.4 + audioHigh * audioReact * 1.0)));
                float vert = step(h, 0.55) * step(0.30, fract(ld.x)) * step(fract(ld.x), 0.55);
                float bar  = step(0.55, h) * step(h, 0.85) * step(0.40, fract(ld.y)) * step(fract(ld.y), 0.62);
                float ink  = max(vert, bar);
                col = mix(col, CR_BLACK, ink * glyphIntensity);
            }
        }
    }

    // Surprise: every ~22s a single red diagonal beam sweeps the canvas
    {
        float _ph = fract(TIME / 22.0);
        float _f  = smoothstep(0.0, 0.05, _ph) * smoothstep(0.35, 0.20, _ph);
        float _t = (_ph - 0.05) / 0.30;
        vec2 _r = vec2(uv.x * 0.7071 + uv.y * 0.7071,
                      -uv.x * 0.7071 + uv.y * 0.7071);
        float _band = exp(-pow((_r.x - (-0.2 + 1.4 * _t)) * 30.0, 2.0));
        col = mix(col, vec3(0.90, 0.10, 0.08), _f * _band * 0.85);
    }

    // Audio breath
    col *= 0.94 + audioLevel * audioReact * 0.10;

    gl_FragColor = vec4(col, 1.0);
}
