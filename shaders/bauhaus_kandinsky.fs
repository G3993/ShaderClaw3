/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Bauhaus after Kandinsky's Composition VIII (1923) and Several Circles (1926) — geometric SDF primitives floating on white, each shape strictly paired to its primary (yellow triangle, red square, blue circle). Lissajous orbits, weighted-circle gradient halos, thin black supporting lines.",
  "INPUTS": [
    { "NAME": "kandinskyWork", "LABEL": "Painting", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Composition VIII (1923)", "Several Circles (1926)", "Yellow Red Blue (1925)", "Composition X (1939)", "On White II (1923)"] },
    { "NAME": "shapeCount", "LABEL": "Shape Count", "TYPE": "float", "MIN": 4.0, "MAX": 22.0, "DEFAULT": 13.0 },
    { "NAME": "shapeSize", "LABEL": "Shape Size", "TYPE": "float", "MIN": 0.04, "MAX": 0.20, "DEFAULT": 0.09 },
    { "NAME": "orbitSpeed", "LABEL": "Orbit Speed", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25 },
    { "NAME": "orbitRange", "LABEL": "Orbit Range", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.22 },
    { "NAME": "haloStrength", "LABEL": "Halo Strength", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "haloRadius", "LABEL": "Halo Radius", "TYPE": "float", "MIN": 1.2, "MAX": 4.0, "DEFAULT": 2.2 },
    { "NAME": "lineCount", "LABEL": "Support Lines", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 4.0 },
    { "NAME": "lineWidth", "LABEL": "Line Width", "TYPE": "float", "MIN": 0.0008, "MAX": 0.005, "DEFAULT": 0.0018 },
    { "NAME": "springReact", "LABEL": "Bass Spring", "TYPE": "float", "MIN": 0.0, "MAX": 0.4, "DEFAULT": 0.12 },
    { "NAME": "strictPairing", "LABEL": "Strict Pairing", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "useTexPalette", "LABEL": "Sample Tex for Palette", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "compositionSeed", "LABEL": "Seed", "TYPE": "float", "MIN": 0.0, "MAX": 50.0, "DEFAULT": 0.0 },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

// Kandinsky's Bauhaus pedagogy: yellow=triangle, red=square, blue=circle.
// We instantiate N shape SDFs with Lissajous-driven centres and union
// them via min(distance) / closer-wins compositing. The "weighted
// circle" gradient is the Several Circles halo.

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

float sdCircle(vec2 p, float r) { return length(p) - r; }

// Centred axis-aligned square SDF
float sdBox(vec2 p, float r) {
    vec2 d = abs(p) - r;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

// Equilateral triangle pointing up
float sdTriangle(vec2 p, float r) {
    const float k = 1.7320508;  // sqrt(3)
    p.x = abs(p.x) - r;
    p.y = p.y + r / k;
    if (p.x + k * p.y > 0.0) p = vec2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
    p.x -= clamp(p.x, -2.0 * r, 0.0);
    return -length(p) * sign(p.y);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 P = vec2(uv.x * aspect, uv.y);

    // Per-painting background — Composition VIII / Several Circles
    // (black) / Yellow-Red-Blue (warm white) / Composition X (black) /
    // On White II (pure white).
    int kw = int(kandinskyWork);
    vec3 col;
    if      (kw == 1) col = vec3(0.06, 0.06, 0.08);  // Several Circles black
    else if (kw == 3) col = vec3(0.04, 0.04, 0.05);  // Composition X black
    else if (kw == 4) col = vec3(0.97, 0.97, 0.97);  // On White II
    else if (kw == 2) col = vec3(0.95, 0.92, 0.84);  // Yellow Red Blue warm
    else              col = vec3(0.97, 0.96, 0.92);  // Composition VIII default

    // Palette — strict pairing or input-derived.
    vec3 yellowC = vec3(0.98, 0.85, 0.10);
    vec3 redC    = vec3(0.89, 0.12, 0.14);
    vec3 blueC   = vec3(0.10, 0.18, 0.65);
    if (useTexPalette && IMG_SIZE_inputTex.x > 0.0) {
        yellowC = texture(inputTex, vec2(0.2, 0.5)).rgb;
        redC    = texture(inputTex, vec2(0.5, 0.5)).rgb;
        blueC   = texture(inputTex, vec2(0.8, 0.5)).rgb;
    }

    int N = int(clamp(shapeCount, 1.0, 22.0));

    // Pass 1 — halo / weighted-circle gradient field. Each shape casts a
    // soft glow whose radius scales with the shape's size, and where
    // halos overlap they additively brighten — Several Circles vibe.
    float haloField = 0.0;
    vec3  haloCol   = vec3(0.0);
    float haloWt    = 0.0;
    for (int i = 0; i < 22; i++) {
        if (i >= N) break;
        float fi = float(i) + compositionSeed * 1.71;
        // Lissajous orbit
        // Orbital home itself walks slowly so the canvas is never the
        // same N positions rotating in fixed circles.
        vec2 home = vec2(0.5 + (hash11(fi * 1.3) - 0.5) * 1.4
                             + 0.10 * sin(TIME * 0.07 + fi),
                         0.5 + (hash11(fi * 2.7) - 0.5) * 1.4
                             + 0.10 * cos(TIME * 0.05 + fi * 1.3));
        float a = TIME * orbitSpeed
                * (0.4 + hash11(fi * 3.1) * 1.4);
        vec2  orbit = vec2(sin(a + fi),
                           cos(a * 0.7 + fi * 1.7))
                    * orbitRange;
        // Bass spring — push outward briefly
        vec2 fromCtr = home + orbit - vec2(0.5);
        if (length(fromCtr) > 1e-4) fromCtr = normalize(fromCtr);
        vec2 ctr = home + orbit + fromCtr * springReact * audioBass * audioReact;
        ctr.x *= aspect;

        float sz = shapeSize * (0.7 + hash11(fi * 5.3) * 0.6)
                 * (1.0 + audioLevel * audioReact * 0.08);
        float r  = length(P - ctr);
        float halo = exp(-pow(r / (sz * haloRadius), 2.0));
        haloField += halo;

        int t = int(mod(fi, 3.0));
        vec3 c = (t == 0) ? yellowC : (t == 1) ? redC : blueC;
        haloCol += c * halo;
        haloWt += halo;
    }
    if (haloWt > 1e-4) {
        haloCol /= haloWt;
        col = mix(col, haloCol, clamp(haloField * haloStrength * 0.35, 0.0, 1.0));
    }

    // Pass 2 — solid shape compositing. Closest-shape-wins via min over
    // signed distances; each shape uses its strict-paired colour.
    float bestSD = 1e9;
    vec3  bestCol = col;
    for (int i = 0; i < 22; i++) {
        if (i >= N) break;
        float fi = float(i) + compositionSeed * 1.71;
        // Orbital home itself walks slowly so the canvas is never the
        // same N positions rotating in fixed circles.
        vec2 home = vec2(0.5 + (hash11(fi * 1.3) - 0.5) * 1.4
                             + 0.10 * sin(TIME * 0.07 + fi),
                         0.5 + (hash11(fi * 2.7) - 0.5) * 1.4
                             + 0.10 * cos(TIME * 0.05 + fi * 1.3));
        float a = TIME * orbitSpeed
                * (0.4 + hash11(fi * 3.1) * 1.4);
        vec2  orbit = vec2(sin(a + fi),
                           cos(a * 0.7 + fi * 1.7))
                    * orbitRange;
        vec2 fromCtr = home + orbit - vec2(0.5);
        if (length(fromCtr) > 1e-4) fromCtr = normalize(fromCtr);
        vec2 ctr = home + orbit + fromCtr * springReact * audioBass * audioReact;
        ctr.x *= aspect;

        float sz = shapeSize * (0.7 + hash11(fi * 5.3) * 0.6)
                 * (1.0 + audioLevel * audioReact * 0.08);

        // Per-shape rotation
        float rot = TIME * audioMid * audioReact * 0.4
                  + hash11(fi * 7.7) * 6.28;
        float ca = cos(-rot), sa = sin(-rot);
        vec2 lp = vec2(ca * (P.x - ctr.x) - sa * (P.y - ctr.y),
                       sa * (P.x - ctr.x) + ca * (P.y - ctr.y));

        int t = int(mod(fi, 3.0));
        if (!strictPairing) t = int(mod(fi * 3.7, 3.0));

        float sd = (t == 0) ? sdTriangle(lp, sz)
                : (t == 1) ? sdBox(lp, sz)
                : sdCircle(lp, sz);

        if (sd < bestSD) {
            bestSD = sd;
            // Every 4th shape gets a checkerboard interior — Composition
            // VIII signature pattern (chess-grid fills inside primitives).
            vec2  chk    = floor(lp / max(sz * 0.32, 1e-4));
            bool  chkOn  = (mod(chk.x + chk.y, 2.0) < 1.0);
            bool  useChk = (mod(fi, 4.0) >= 3.0);
            vec3  baseC  = (t == 0) ? yellowC : (t == 1) ? redC : blueC;
            vec3  altC   = vec3(0.05, 0.05, 0.06);
            bestCol = useChk ? (chkOn ? baseC : altC) : baseC;
        }
    }
    float fillMask = 1.0 - smoothstep(0.0, 0.0025, bestSD);
    col = mix(col, bestCol, fillMask);

    // Pass 3 — thin black support lines — diagonal scaffolds across
    // the canvas, characteristic of Composition VIII.
    int NL = int(clamp(lineCount, 0.0, 10.0));
    for (int k = 0; k < 10; k++) {
        if (k >= NL) break;
        float fk = float(k) + compositionSeed * 0.71;
        float angle = hash11(fk * 1.7) * 6.2832;
        vec2 dir = vec2(cos(angle), sin(angle));
        vec2 pt  = vec2(hash11(fk * 3.3), hash11(fk * 5.1));
        // Distance from point to infinite line at pt with direction dir.
        vec2 d = uv - pt;
        float perp = abs(d.x * (-dir.y) + d.y * dir.x);
        float lm = smoothstep(lineWidth, 0.0, perp);
        col = mix(col, vec3(0.05, 0.05, 0.06),
                  lm * (0.6 + audioHigh * audioReact * 0.4));
    }

    gl_FragColor = vec4(col, 1.0);
}
