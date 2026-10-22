/*{
  "DESCRIPTION": "Marble — a cabinet of living glass orbs. Every sphere holds its own slowly churning 3D flow field (iterated domain warp sampled on the rotating sphere), read out through interference and thermal colour maps: black voids, deep blue, cyan, acid yellow, ember orange and red fringes. Six families of orb share the grid — vivid fringe orbs, soft defocused glass, kaleidoscopic mandalas, ghost orbs whose shell disappears so only the coloured blob floats, ochre marbled agate with contour lines, and dark void orbs threaded with cyan filaments — and each orb drifts from one family into the next on its own clock. A travelling light sweeps the grid, every orb bobs and breathes, beats ripple a light ring outward from the centre, bass churns the flow, mids slide the colour bands, highs light up the rims.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "layoutMode",  "LABEL": "Layout", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1], "LABELS": ["Orb Grid", "Single Orb"], "GROUP": "Layout" },
    { "NAME": "gridCols",    "LABEL": "Columns",         "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 12.0, "DEFAULT": 7.0, "GROUP": "Layout" },
    { "NAME": "gridRows",    "LABEL": "Rows",            "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 8.0,  "DEFAULT": 4.0, "GROUP": "Layout" },
    { "NAME": "orbSize",     "LABEL": "Orb Size",        "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 1.0,  "DEFAULT": 0.84, "GROUP": "Layout" },
    { "NAME": "styleSel",    "LABEL": "Style (single / locked)", "TYPE": "long", "DEFAULT": 6, "VALUES": [0, 1, 2, 3, 4, 5, 6],
      "LABELS": ["Vivid Fringe", "Soft Glass", "Kaleido", "Ghost", "Ochre Agate", "Void Filament", "Mixed (all)"], "GROUP": "Layout" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 9.0, "GROUP": "Motion" },
    { "NAME": "flow",        "LABEL": "Flow Speed",      "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.45, "GROUP": "Motion" },
    { "NAME": "spin",        "LABEL": "Spin",            "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Motion" },
    { "NAME": "bob",         "LABEL": "Float / Bob",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Motion" },
    { "NAME": "warp",        "LABEL": "Swirl",           "TYPE": "float", "MIN": 0.2, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Pattern" },
    { "NAME": "bands",       "LABEL": "Fringe Bands",    "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 1.4, "GROUP": "Pattern" },
    { "NAME": "patScale",    "LABEL": "Pattern Scale",   "TYPE": "float", "MIN": 0.5, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Pattern" },
    { "NAME": "cVoid",       "LABEL": "Void",            "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0],     "GROUP": "Color" },
    { "NAME": "cDeep",       "LABEL": "Deep Blue",       "TYPE": "color", "DEFAULT": [0.0, 0.16, 0.42, 1.0],   "GROUP": "Color" },
    { "NAME": "cCyan",       "LABEL": "Cyan",            "TYPE": "color", "DEFAULT": [0.05, 0.72, 0.92, 1.0],  "GROUP": "Color" },
    { "NAME": "cYellow",     "LABEL": "Yellow",          "TYPE": "color", "DEFAULT": [0.93, 0.96, 0.18, 1.0],  "GROUP": "Color" },
    { "NAME": "cEmber",      "LABEL": "Ember",           "TYPE": "color", "DEFAULT": [1.0, 0.5, 0.02, 1.0],    "GROUP": "Color" },
    { "NAME": "cRed",        "LABEL": "Red",             "TYPE": "color", "DEFAULT": [0.88, 0.12, 0.04, 1.0],  "GROUP": "Color" },
    { "NAME": "cOchre",      "LABEL": "Ochre",           "TYPE": "color", "DEFAULT": [0.8, 0.6, 0.24, 1.0],    "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",      "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "lightSweep",  "LABEL": "Light Sweep",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Light" },
    { "NAME": "rimGlow",     "LABEL": "Rim Glow",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Light" },
    { "NAME": "specular",    "LABEL": "Specular",        "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.5, "GROUP": "Light" },
    { "NAME": "grain",       "LABEL": "Grain",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.045, 0.045, 0.045, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Marble — single pass. Each pixel finds its orb, lifts the disc to a unit
// sphere normal, rotates it, and samples a 3D domain-warped flow field on
// the sphere. Six style functions read that field out; each orb blends
// between two styles while it morphs.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }

float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

vec3 hueRot(vec3 c, float h) {
    if (h <= 0.0) return c;
    float a = h * TAU;
    vec3 k = vec3(0.57735);
    float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}

mat3 rotXY(float a, float b) {
    float ca = cos(a), sa = sin(a), cb = cos(b), sb = sin(b);
    mat3 ry = mat3(ca, 0.0, -sa, 0.0, 1.0, 0.0, sa, 0.0, ca);
    mat3 rx = mat3(1.0, 0.0, 0.0, 0.0, cb, sb, 0.0, -sb, cb);
    return ry * rx;
}

// ── the flow: iterated sinusoidal domain warp in 3D ──────────────────────
float field(vec3 p, float t, float seed, float w, float freq) {
    vec3 q = p * freq * patScale + seed * vec3(3.1, 1.7, 5.3);
    float a = w * warp;
    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        q += a * sin(q.zxy * (1.1 + 0.35 * fi) + vec3(t * (0.31 + 0.11 * fi), t * 0.23 + fi * 1.3, -t * 0.27 + fi * 2.1));
        a *= 0.62;
    }
    return sin(q.x + 0.7 * sin(q.y * 1.3)) * cos(q.z * 0.9 - 0.5 * q.y);
}

// cyclic interference / thermal map: void → deep → cyan → yellow → ember → red → void
vec3 spectral(float x) {
    x = fract(x);
    vec3 c = cVoid.rgb;
    c = mix(c, cDeep.rgb,   smoothstep(0.02, 0.14, x));
    c = mix(c, cCyan.rgb,   smoothstep(0.14, 0.3, x));
    c = mix(c, mix(cCyan.rgb, cYellow.rgb, 0.45), smoothstep(0.3, 0.4, x));
    c = mix(c, cYellow.rgb, smoothstep(0.4, 0.52, x));
    c = mix(c, cEmber.rgb,  smoothstep(0.52, 0.66, x));
    c = mix(c, cRed.rgb,    smoothstep(0.66, 0.8, x));
    c = mix(c, cVoid.rgb,   smoothstep(0.8, 0.97, x));
    return c;
}

vec3 ochreMap(float x) {
    vec3 dark = cOchre.rgb * vec3(0.3, 0.17, 0.08);
    vec3 rust = cOchre.rgb * vec3(0.9, 0.5, 0.25);
    vec3 pale = mix(cOchre.rgb, vec3(0.9, 0.88, 0.7), 0.45);
    vec3 olive = cOchre.rgb * vec3(0.62, 0.68, 0.45);
    vec3 c = mix(dark, rust, smoothstep(0.05, 0.35, x));
    c = mix(c, cOchre.rgb, smoothstep(0.35, 0.55, x));
    c = mix(c, pale, smoothstep(0.55, 0.75, x));
    c = mix(c, olive, smoothstep(0.75, 1.0, x));
    return c;
}

// returns rgb + alpha (alpha < 1 only for ghost orbs)
vec4 orbStyle(float s, vec3 p, vec3 n, vec2 d, float r, float t, float seed, float bandOff) {
    vec3 L = normalize(vec3(-0.45, 0.55, 0.7));
    float diff = 0.5 + 0.5 * dot(n, L);
    vec3 col; float a = 1.0;
    if (s < 0.5) {                                      // vivid fringe
        float v = field(p, t, seed, 1.15, 1.35);
        float x = 0.5 + 0.5 * v * bands * 1.1 + bandOff;
        col = spectral(x);
        col *= 0.82 + 0.25 * diff;
    } else if (s < 1.5) {                               // soft glass
        float v = field(p, t * 0.8, seed, 0.75, 0.9);
        float x = 0.5 + 0.42 * v * bands + bandOff;
        col = spectral(x);
        col = mix(col, vec3(lum(col)) * vec3(0.85, 0.85, 0.6), 0.32);
        col = col * 0.72 + vec3(0.06, 0.07, 0.05);
        // inner lens reflection band
        float lens = exp(-pow((n.y - 0.15 + 0.08 * sin(t * 0.4 + seed * 9.0)) * 7.0, 2.0)) * (1.0 - r * r);
        col += vec3(0.2, 0.22, 0.16) * lens;
        col *= 0.6 + 0.5 * diff;
    } else if (s < 2.5) {                               // kaleido
        float N = 6.0 + 2.0 * floor(seed * 3.0);
        float ang = atan(d.y, d.x) + t * 0.1;
        float seg = TAU / N;
        float af = abs(mod(ang + 0.5 * seg, seg) - 0.5 * seg);
        float rr = pow(r, 0.8);
        vec3 pk = vec3(cos(af) * rr, sin(af) * rr, n.z) * (1.0 + 0.6 / (r + 0.35));
        float v = field(pk, t, seed, 1.1, 1.7);
        float x = 0.5 + 0.5 * v * bands * 1.4 + bandOff;
        col = spectral(x) * (0.55 + 0.45 * smoothstep(0.0, 0.4, r)) * (0.75 + 0.35 * diff);
    } else if (s < 3.5) {                               // ghost: the shell vanishes, the blob floats
        float v = field(p, t, seed, 1.0, 1.1);
        float x = 0.5 + 0.5 * v * bands + bandOff;
        col = spectral(x);
        a = smoothstep(0.08, 0.35, lum(col)) * smoothstep(1.0, 0.6, r);
    } else if (s < 4.5) {                               // ochre agate
        float v = field(p, t * 0.7, seed, 1.25, 1.2);
        float x = 0.5 + 0.5 * v;
        col = ochreMap(x);
        float c = abs(fract(x * 10.0 + bandOff * 3.0) - 0.5);
        col *= 1.0 - 0.3 * smoothstep(0.1, 0.0, c);
        col += vec3(0.16, 0.12, 0.05) * smoothstep(0.06, 0.0, abs(c - 0.25));
        // now and then a thin spectral sheen breaks through
        float sheen = smoothstep(0.08, 0.0, abs(x - 0.5 - 0.15 * sin(t * 0.3 + seed * 7.0)));
        col = mix(col, spectral(x * 3.0 + bandOff), sheen * 0.55);
        col *= 0.5 + 0.65 * diff;
    } else {                                            // void filament
        float v = field(p, t, seed, 1.2, 1.4);
        float fil = exp(-abs(v) * 16.0);
        float pocket = smoothstep(0.55, 0.95, v);
        col = cCyan.rgb * fil * 1.1 + mix(cEmber.rgb, cYellow.rgb, smoothstep(0.75, 1.0, v)) * pocket;
        col += cDeep.rgb * 0.25 * smoothstep(-0.2, -0.9, v);
    }
    return vec4(col, a);
}

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    bool single = float(layoutMode) > 0.5;
    float cols = single ? 1.0 : floor(gridCols + 0.5);
    float rows = single ? 1.0 : floor(gridRows + 0.5);
    float cell = min(R.x / cols, R.y / rows);
    vec2 origin = 0.5 * (R - cell * vec2(cols, rows));
    vec2 g = (fc - origin) / cell;

    vec3 bg = backdrop(uv);
    vec3 col = bg;
    float alpha = 0.0;

    // check own tile and immediate neighbours (bobbing orbs + halos overlap borders)
    vec2 ti0 = floor(g);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 ti = ti0 + vec2(float(i), float(j));
        if (ti.x < 0.0 || ti.y < 0.0 || ti.x >= cols || ti.y >= rows) continue;
        float idx = ti.x + (rows - 1.0 - ti.y) * cols;
        float seed = h11(idx * 1.37 + 0.21);
        float seed2 = h11(idx * 2.91 + 5.3);
        vec2 ctr = ti + 0.5;
        // float / bob
        ctr += bob * 0.07 * vec2(sin(TIME * (0.4 + 0.3 * seed) + seed * 20.0), cos(TIME * (0.33 + 0.3 * seed2) + seed2 * 17.0));
        // beat ring travels outward from the grid centre
        vec2 gc = vec2(cols, rows) * 0.5;
        float distC = length(ctr - gc) / max(cols, rows);
        float ring = exp(-pow((distC - (1.0 - audioBeatPulse) * 0.8) * 9.0, 2.0)) * audioBeatPulse * ar;
        float rad = 0.5 * orbSize * (single ? 1.1 : 1.0) * (1.0 + 0.025 * sin(TIME * 0.9 + seed * 30.0) + 0.05 * ring + 0.03 * bassP() * ar);

        vec2 d = (g - ctr) / rad;
        float r = length(d);
        float pxR = 1.5 / (cell * rad);

        // soft outer halo
        if (r > 1.0) {
            float halo = exp(-(r - 1.0) * 18.0) * 0.12 * rimGlow * (1.0 + highP() * ar);
            col += mix(cCyan.rgb, cEmber.rgb, seed) * halo;
            continue;
        }

        // style assignment and morph
        float sl = float(styleSel);
        float ph = TIME / morphPeriod + seed * 3.0;
        float k = floor(ph);
        float f = smoothstep(0.65, 1.0, fract(ph));
        float sa, sb;
        if (sl > 5.5 && single) {
            // one orb walks through every family in turn, melting from one into the next
            sa = mod(k, 6.0); sb = mod(k + 1.0, 6.0);
            f = smoothstep(0.4, 1.0, fract(ph));
            f = f * f * (3.0 - 2.0 * f);
        } else if (sl > 5.5) {
            sa = floor(h11(idx * 0.73 + k * 1.91) * 6.0);
            sb = floor(h11(idx * 0.73 + (k + 1.0) * 1.91) * 6.0);
        } else { sa = sl; sb = sl; f = 0.0; }

        vec3 n = vec3(d, sqrt(max(1.0 - r * r, 0.0)));
        float tFlow = TIME * flow + 0.35 * audioBassTime * ar * flow + seed * 40.0;
        mat3 M = rotXY(TIME * spin * (0.25 + 0.3 * seed) + seed * 6.0, 0.4 * sin(TIME * spin * 0.2 + seed2 * 6.0));
        vec3 p = M * n;
        float bandOff = 0.12 * sin(TIME * 0.05 + seed * 6.0) + 0.18 * midP() * ar;

        vec4 c = orbStyle(sa, p, n, d, r, tFlow, seed, bandOff);
        if (f > 0.001) c = mix(c, orbStyle(sb, p, n, d, r, tFlow, seed, bandOff), f);

        // rim: a thin coloured line just inside the edge
        float rimW = smoothstep(0.86, 0.985, r) * smoothstep(1.0, 0.97, r);
        vec3 rimC = mix(cCyan.rgb, cEmber.rgb, step(0.6, seed2));
        float ghostness = 1.0 - c.a;
        c.rgb += rimC * rimW * 0.5 * rimGlow * (1.0 + 0.8 * highP() * ar) * (1.0 - ghostness);
        // specular glint
        vec3 H = normalize(vec3(-0.45, 0.55, 0.7) + vec3(0.0, 0.0, 1.0));
        c.rgb += vec3(1.0, 0.97, 0.9) * pow(max(dot(n, H), 0.0), 60.0) * specular * 0.6 * (1.0 - ghostness);

        // light sweep across the grid + beat ring
        float sweep = 0.5 + 0.5 * sin(dot(ctr, vec2(0.55, 0.35)) * 1.2 - TIME * 0.6);
        c.rgb *= 1.0 + lightSweep * (0.45 * sweep - 0.2) + 0.5 * ring;
        c.rgb *= 1.0 + 0.2 * audioLevel * ar;

        float edge = smoothstep(1.0, 1.0 - pxR, r);
        float a = c.a * edge;
        col = mix(col, c.rgb, a);
        alpha = max(alpha, a);
    }

    col = hueRot(col, hueShift);
    float l = lum(col);
    col = mix(vec3(l), col, saturation);
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.07;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    float outA = transparentBg ? alpha : 1.0;
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), outA);
}
