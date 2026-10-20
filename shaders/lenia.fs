/*{
  "DESCRIPTION": "Lenia — microscopic glass animals that are alive. A real Lenia continuous cellular automaton runs every frame in the Orbium regime, where living tissue can only survive as discrete soliton creatures: each one seeded from the actual Orbium pattern, then holding its own shape purely through the kernel-and-growth law as it glides across the water. They swim at random headings, meet, fuse into twin-bodied hybrids, flare and burst apart; long-range crowding keeps collisions from flooding into a labyrinth, and new creatures keep hatching into empty water, so the population turns over forever. Four bioluminescent species (cyan, magenta, lime, amber) each carry their colour inside their own body. Rendered as translucent gel with real thickness: the deep-blue abyss refracts through each body, fresnel membranes and inner lamellae glow (iso-surfaces of the creature itself, so they move with it), a hot nucleus pulses, wet speculars catch a key light; behind them a deeper defocused layer of creatures drifts under faint god rays, with marine snow and bokeh in front. Bass lifts the bioluminescence, beats hatch extra creatures and send light pulses through the bodies, highs sparkle the snow.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #2. Orbium pattern after Bert Chan, Lenia (2019).",
  "CATEGORIES": ["Generator", "Simulation", "Organic", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Swim Speed",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.75, "GROUP": "Speed" },
    { "NAME": "drift",      "LABEL": "Camera Drift",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Speed" },
    { "NAME": "colorCycle", "LABEL": "Color Cycle",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.0, "GROUP": "Speed" },
    { "NAME": "kernelR",    "LABEL": "Creature Size",     "TYPE": "float", "AUDIO": false, "MIN": 12.0, "MAX": 16.0, "DEFAULT": 15.0, "GROUP": "Life" },
    { "NAME": "spawn",      "LABEL": "Hatching",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Life" },
    { "NAME": "zoom",       "LABEL": "Zoom",              "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 2.5, "DEFAULT": 1.35, "GROUP": "Scene" },
    { "NAME": "relief",     "LABEL": "Body Thickness",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Scene" },
    { "NAME": "refraction", "LABEL": "Refraction",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Scene" },
    { "NAME": "depthLayers","LABEL": "Deep Layer",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Scene" },
    { "NAME": "rays",       "LABEL": "God Rays",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Scene" },
    { "NAME": "snow",       "LABEL": "Marine Snow",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Scene" },
    { "NAME": "color1",     "LABEL": "Species 1",         "TYPE": "color", "DEFAULT": [0.15, 0.9, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "color2",     "LABEL": "Species 2",         "TYPE": "color", "DEFAULT": [1.0, 0.16, 0.7, 1.0],  "GROUP": "Color" },
    { "NAME": "color3",     "LABEL": "Species 3",         "TYPE": "color", "DEFAULT": [0.62, 1.0, 0.18, 1.0], "GROUP": "Color" },
    { "NAME": "color4",     "LABEL": "Species 4",         "TYPE": "color", "DEFAULT": [1.0, 0.58, 0.1, 1.0],  "GROUP": "Color" },
    { "NAME": "abyssColor", "LABEL": "Abyss",             "TYPE": "color", "DEFAULT": [0.006, 0.03, 0.075, 1.0], "GROUP": "Color" },
    { "NAME": "rayColor",   "LABEL": "Ray Color",         "TYPE": "color", "DEFAULT": [0.25, 0.55, 0.85, 1.0], "GROUP": "Color" },
    { "NAME": "glow",       "LABEL": "Bioluminescence",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "hueShift",   "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "saturation", "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "specAmt",    "LABEL": "Wet Specular",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "bloom",      "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.9, "GROUP": "Finish" },
    { "NAME": "aberration", "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "grain",      "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "simRes",     "LABEL": "Sim Resolution",    "TYPE": "float", "AUDIO": false, "MIN": 0.15, "MAX": 0.4, "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "audioReact", "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG", "TYPE": "bool", "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "lifeA", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),512)", "HEIGHT": "min(floor($HEIGHT*$simRes),288)" },
    { "TARGET": "sceneBuf" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Lenia — FRONTIER pack #2
//   pass 0  lifeA (persistent, capped size): one Lenia step per frame.
//           A ← clip(A + dt·G(K∗A)), K = smooth ring kernel of radius R,
//           G = Gaussian growth bump (μ 0.15, σ 0.015) — the Orbium regime,
//           where tissue only survives as discrete gliding soliton creatures.
//           The ring convolution is taken over 2×2 texel blocks with bilinear
//           taps (¼ of the fetches). Creatures are seeded from the real Orbium
//           pattern (20×20, packed 4-bit, rotated + rescaled to R).
//           Long-range crowding (mean of U over 2.5R) raises μ where tissue
//           piles up, so collisions end in a burst instead of a labyrinth;
//           new creatures hatch into empty water. Species hue is carried by
//           each body (A-weighted average over the kernel).
//           rgba = (A, U = K∗A, crowding C, species hue)
//   pass 1  sceneBuf: deep-water scene. Abyss gradient, god rays, marine snow,
//           a deeper defocused layer of creatures, then the main layer: gel
//           bodies with thickness (height from A), background refracted
//           through them, fresnel membrane, inner lamellae + nucleus that are
//           iso-surfaces of the body itself (they move with it), death bursts.
//   pass 2  final: bloom, fringe, ACES, HD finisher.
// ─────────────────────────────────────────────────────────────────────────

#define TAU 6.28318530718

float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm(vec2 p) { float a = 0.5, s = 0.0; for (int i = 0; i < 4; i++) { s += a * vnoise(p); p = p * 2.03 + 17.1; a *= 0.5; } return s; }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.06, 0.85, audioMid); }
float highP() { return smoothstep(0.10, 0.90, audioHigh); }
float levP()  { return clamp(audioLevel, 0.0, 1.0); }
float beatP() { return clamp(audioBeatPulse, 0.0, 1.0); }

#define MU    0.15
#define SIGMA 0.015
float growthG(float U, float mu) { return 2.0 * exp(-(U - mu) * (U - mu) / (2.0 * SIGMA * SIGMA)) - 1.0; }
float crowdMu(float C) { return 0.1 * smoothstep(0.05, 0.09, C); }

// ── the Orbium (Chan 2019), 20×20, quantised to 16 levels, 5 cells per float ──
vec4 orbRow(int r) {
    if (r == 0) return vec4(0.0, 8736.0, 0.0, 4.0);
    else if (r == 1) return vec4(0.0, 214081.0, 74274.0, 3.0);
    else if (r == 2) return vec4(0.0, 423762.0, 205347.0, 115.0);
    else if (r == 3) return vec4(65536.0, 428130.0, 1.0, 162.0);
    else if (r == 4) return vec4(204800.0, 288339.0, 2.0, 1584.0);
    else if (r == 5) return vec4(143616.0, 349201.0, 4.0, 3072.0);
    else if (r == 6) return vec4(8708.0, 484352.0, 87.0, 13056.0);
    else if (r == 7) return vec4(784.0, 619776.0, 153.0, 28672.0);
    else if (r == 8) return vec4(912.0, 692992.0, 1979.0, 20480.0);
    else if (r == 9) return vec4(912.0, 827392.0, 11758.0, 77824.0);
    else if (r == 10) return vec4(1792.0, 958464.0, 40959.0, 143360.0);
    else if (r == 11) return vec4(3072.0, 1011712.0, 323583.0, 143360.0);
    else if (r == 12) return vec4(1792.0, 999424.0, 589535.0, 78354.0);
    else if (r == 13) return vec4(24576.0, 790528.0, 646351.0, 13109.0);
    else if (r == 14) return vec4(151552.0, 528384.0, 637901.0, 9286.0);
    else if (r == 15) return vec4(262144.0, 331777.0, 563882.0, 4934.0);
    else if (r == 16) return vec4(131072.0, 270884.0, 424071.0, 309.0);
    else if (r == 17) return vec4(0.0, 275249.0, 284261.0, 19.0);
    else if (r == 18) return vec4(0.0, 209408.0, 144196.0, 1.0);
    else if (r == 19) return vec4(0.0, 65536.0, 4369.0, 0.0);
    return vec4(0.0);
}


// species: 0 cyan (reference), 1 magenta (small, quick), 2 lime (large), 3 amber (mid)
float speciesScale(float h) {
    float x = fract(h) * 4.0;
    float a = 1.0, b = 0.7, c = 1.22, d = 0.86;
    float k = smoothstep(0.25, 0.75, fract(x));
    if (x < 1.0) return mix(a, b, k);
    if (x < 2.0) return mix(b, c, k);
    if (x < 3.0) return mix(c, d, k);
    return mix(d, a, k);
}

float orbCell(float x, float y) {
    if (x < 0.0 || y < 0.0 || x > 19.5 || y > 19.5) return 0.0;
    vec4 row = orbRow(int(y + 0.5));
    float k = floor(x / 5.0 + 0.001);
    float f = k < 0.5 ? row.x : (k < 1.5 ? row.y : (k < 2.5 ? row.z : row.w));
    float d = x - k * 5.0;
    return mod(floor((f + 0.5) / exp2(4.0 * d)), 16.0) / 15.0;
}
// pattern value at a point given in creature-local texels (0 at the centre)
float orbAt(vec2 p, float ang, float Kr) {
    float c = cos(ang), s = sin(ang);
    vec2 q = vec2(c * p.x + s * p.y, -s * p.x + c * p.y) / (Kr / 13.0) + 10.0 - 0.5;
    if (q.x < -1.0 || q.y < -1.0 || q.x > 20.0 || q.y > 20.0) return 0.0;
    vec2 i = floor(q), f = q - i;
    float a = orbCell(i.x, i.y), b = orbCell(i.x + 1.0, i.y);
    float cc = orbCell(i.x, i.y + 1.0), d = orbCell(i.x + 1.0, i.y + 1.0);
    return mix(mix(a, b, f.x), mix(cc, d, f.x), f.y);
}
vec2 torusD(vec2 d, vec2 R) { return d - R * floor(d / R + 0.5); }

// ═════════════════════════════════════════════════════════════════════════
//  LIFE  (pass 0)
// ═════════════════════════════════════════════════════════════════════════
vec4 seedField(vec2 fc, vec2 R, float Kr) {
    // a jittered grid of creatures, most cells occupied, random headings + species
    float cs = Kr * 4.3;
    vec2 cell = floor(fc / cs);
    vec2 h = hash22(cell + 4.7);
    vec2 cc = (cell + 0.5) * cs + (h - 0.5) * cs * 0.4;
    float on = step(0.08, hash12(cell + 1.3));
    float ang = hash12(cell + 9.9) * TAU;
    float hue = floor(hash12(cell + 2.2) * 4.0) / 4.0;
    float v = orbAt(fc - cc, ang, min(Kr * speciesScale(hue), 19.3)) * on;
    return vec4(v, 0.0, 0.0, hue);
}

vec4 lifeStep(vec2 fc, vec2 R) {
    vec2 ts = 1.0 / R;
    vec4 prev = texture2D(lifeA, fc * ts);
    float Kr = min(kernelR * speciesScale(prev.a), 19.3);
    float ar = audioReact;

    // ring kernel over 2×2 blocks; also carry the species hue with the bodies
    float sum = 0.0, wsum = 0.0, hsum = 0.0, hw = 0.0;
    for (int j = -10; j < 10; j++) {
        for (int i = -10; i < 10; i++) {
            vec2 o = vec2(float(i) * 2.0 + 0.5, float(j) * 2.0 + 0.5);
            float r = length(o) / Kr;
            if (r >= 1.0) continue;
            float w = exp(4.0 - 1.0 / max(r * (1.0 - r), 1e-4));
            vec4 s = texture2D(lifeA, fract((fc + o) * ts));
            sum += w * s.r;
            wsum += w;
            hsum += w * s.r * s.a;
            hw += w * s.r;
        }
    }
    float U = sum / max(wsum, 1e-5);
    float hue = hw > 1e-3 * wsum ? hsum / hw : prev.a;
    hue = mix(hue, prev.a, step(hw, 1e-3 * wsum));

    // crowding: mean of last frame's U over a 2.5R disc, sampled every 8 texels
    float cs = 0.0, cn = 0.0;
    for (int j = -5; j <= 5; j++) {
        for (int i = -5; i <= 5; i++) {
            vec2 o = vec2(float(i), float(j)) * 8.0;
            if (length(o) > 2.5 * Kr) continue;
            cs += texture2D(lifeA, fract((fc + o) * ts)).g;
            cn += 1.0;
        }
    }
    float C = cs / max(cn, 1.0);

    float G = growthG(U, MU + crowdMu(C));
    float dt = mix(0.055, 0.1, speed);
    float A = clamp(prev.r + dt * G, 0.0, 1.0);

    // hatching: a new creature appears in empty water every so often
    float rate = 6.0 * spawn * (1.0 + 0.6 * ar * beatP());
    float n = floor(TIME * rate), np = floor(max(TIME - TIMEDELTA, 0.0) * rate);
    if (spawn > 0.0 && n != np) {
        vec2 sp = hash22(vec2(n * 1.37, 5.1)) * R;
        float ang = hash12(vec2(n, 2.9)) * TAU;
        vec4 at = texture2D(lifeA, sp * ts);
        if (at.b < 0.011 && at.g < 0.003) {
            float nh = floor(fract(n * 0.618034 + hash12(vec2(n, 7.7)) * 0.25) * 4.0) / 4.0;
            float v = orbAt(torusD(fc - sp, R), ang, min(kernelR * speciesScale(nh), 19.3));
            // the hatchling's species claims a ring of water around it too, so its kernel radius is right from frame 1
            float claim = step(length(torusD(fc - sp, R)), 1.6 * kernelR);
            if (v > A) A = v;
            hue = mix(hue, nh, claim);
        }
    }
    return vec4(A, U, C, hue);
}

// ═════════════════════════════════════════════════════════════════════════
//  SCENE  (pass 1)
// ═════════════════════════════════════════════════════════════════════════
vec2 g_simR;
float g_t, g_ar;

vec4 sampleLife(vec2 p) {
    // B-spline bicubic via 4 bilinear taps (p in sim texels)
    vec2 st = p - 0.5;
    vec2 i = floor(st), f = st - i;
    vec2 f2 = f * f, f3 = f2 * f;
    vec2 w0 = (1.0 - 3.0 * f + 3.0 * f2 - f3) / 6.0;
    vec2 w1 = (4.0 - 6.0 * f2 + 3.0 * f3) / 6.0;
    vec2 w2 = (1.0 + 3.0 * f + 3.0 * f2 - 3.0 * f3) / 6.0;
    vec2 w3 = f3 / 6.0;
    vec2 s0 = w0 + w1, s1 = w2 + w3;
    vec2 c0 = i - 0.5 + w1 / s0 + 1.0;
    vec2 c1 = i + 1.5 + w3 / s1 + 1.0;
    vec2 ir = 1.0 / g_simR;
    vec4 a = texture2D(lifeA, fract(vec2(c0.x, c0.y) * ir));
    vec4 b = texture2D(lifeA, fract(vec2(c1.x, c0.y) * ir));
    vec4 c = texture2D(lifeA, fract(vec2(c0.x, c1.y) * ir));
    vec4 d = texture2D(lifeA, fract(vec2(c1.x, c1.y) * ir));
    float kx = s1.x / (s0.x + s1.x), ky = s1.y / (s0.y + s1.y);
    return mix(mix(a, b, kx), mix(c, d, kx), ky);
}

vec3 palette(vec3 c) {
    c = hueRot(c, hueShift + colorCycle * 0.02 * g_t);
    float l = lum(c);
    return max(mix(vec3(l), c, saturation), 0.0);
}
vec4 speciesAnat(float h) {
    float x = fract(h) * 4.0;
    vec4 a = vec4(62.0, 1.0, 0.78, 0.0);    // cyan: nested lamellae
    vec4 b = vec4(120.0, 0.35, 0.66, 0.7);  // magenta: fine rings + beads, big nucleus
    vec4 c = vec4(30.0, 1.35, 0.74, 0.0);   // lime: bold bands
    vec4 d = vec4(6.0, 0.6, 0.86, 1.0);     // amber: granular, small hot core
    float k = smoothstep(0.25, 0.75, fract(x));
    if (x < 1.0) return mix(a, b, k);
    if (x < 2.0) return mix(b, c, k);
    if (x < 3.0) return mix(c, d, k);
    return mix(d, a, k);
}
vec3 speciesCol(float h) {
    // 4 species: cyan, magenta, lime, amber (cyclic blend so merged bodies sit between)
    float x = fract(h) * 4.0;
    vec3 c0 = palette(color1.rgb), c1 = palette(color2.rgb), c2 = palette(color3.rgb), c3 = palette(color4.rgb);
    float k = fract(x);
    k = smoothstep(0.25, 0.75, k);
    if (x < 1.0) return mix(c0, c1, k);
    if (x < 2.0) return mix(c1, c2, k);
    if (x < 3.0) return mix(c2, c3, k);
    return mix(c3, c0, k);
}

// abyss behind the main layer: gradient, god rays, deep defocused creatures, marine snow
vec3 abyssAt(vec2 uv, vec2 pan, float simPer) {
    vec3 deep = palette(abyssColor.rgb);
    vec3 col = deep * (0.5 + 1.9 * smoothstep(-0.45, 0.75, uv.y));
    // god rays slanting down from the surface
    float ray = fbm(vec2((uv.x - uv.y * 0.32) * 5.0 + 3.0, g_t * 0.04));
    float ray2 = fbm(vec2((uv.x - uv.y * 0.32) * 11.0 - 7.0, g_t * 0.07));
    ray = (pow(smoothstep(0.40, 0.85, ray), 2.0) + 0.35 * pow(smoothstep(0.5, 0.9, ray2), 3.0)) * smoothstep(-0.6, 0.65, uv.y);
    col += palette(rayColor.rgb) * ray * 0.24 * rays;
    // surface glow at the very top
    col += palette(rayColor.rgb) * 0.06 * rays * smoothstep(0.2, 0.62, uv.y);
    // a deeper layer of creatures: smaller, slower, defocused (U field), dim
    vec2 wd = (pan * 0.5 + vec2(173.0, 91.0)) + uv * simPer * 1.05;
    vec4 sd = sampleLife(wd);
    float Ad = smoothstep(0.015, 0.17, sd.g);
    vec3 dc = mix(speciesCol(sd.a), deep * 6.0, 0.5);
    col += dc * Ad * 0.2 * depthLayers;
    // marine snow, two far layers, crisp tiny flecks drifting down
    for (int k = 0; k < 3; k++) {
        float fk = float(k) * 0.5;
        float sc = mix(110.0, 40.0, fk);
        vec2 q = uv * sc + pan * 0.002 * sc * (0.3 + fk) + vec2(fk * 13.1 + g_t * 0.2, g_t * (0.5 + 0.6 * fk) + fk * 7.7);
        q.x += 0.4 * sin(q.y * 0.3 + fk);
        vec2 id = floor(q), f = fract(q) - 0.5;
        float h = hash12(id + fk * 31.0);
        if (h < 0.14) {
            vec2 o = (hash22(id + 3.1 + fk) - 0.5) * 0.6;
            float d = length(f - o);
            float rad = mix(0.05, 0.08, fk) * (0.6 + 0.8 * hash12(id + 5.0));
            col += vec3(0.55, 0.75, 0.9) * (1.0 - smoothstep(rad * 0.5, rad, d)) * mix(0.25, 0.55, fk) * snow;
        }
    }
    return col;
}

vec4 renderScene(vec2 fc, vec2 R) {
    vec2 uv = (fc - 0.5 * R) / R.y;
    float ar = g_ar;
    float bass = bassP(), mid = midP(), high = highP(), lev = levP(), beat = beatP();

    // view: slow drifting pan over the culture
    float simPer = g_simR.y * 0.78 / zoom;                   // sim texels per screen height
    vec2 pan = vec2(g_t * 3.0, g_t * 1.7) * drift + vec2(sin(g_t * 0.05), cos(g_t * 0.037)) * 25.0 * drift;
    // gentle lens bulge = water depth cue
    vec2 uvl = uv * (1.0 + 0.06 * dot(uv, uv));
    vec2 w = pan + uvl * simPer;
    float foot = simPer / R.y;

    vec4 s = sampleLife(w);
    float A = s.r, U = s.g, hue = s.a;
    float e = 0.75;
    float Ax = sampleLife(w + vec2(e, 0.0)).r, Ay = sampleLife(w + vec2(0.0, e)).r;
    vec2 grad = vec2(Ax - A, Ay - A) / e;
    float gl = length(grad) + 1e-4;
    float G = growthG(U, MU + crowdMu(s.b));

    vec3 sc = speciesCol(hue);
    float thick = smoothstep(0.02, 0.95, A);
    float body = smoothstep(0.035, 0.11, A);

    // gel normal: height = A
    float hs = 9.0 * relief;
    vec3 N = normalize(vec3(-grad * hs, 1.0));
    vec3 L = normalize(vec3(-0.45, 0.6, 1.0));
    vec3 V = vec3(0.0, 0.0, 1.0);
    vec3 Hh = normalize(L + V);

    // background, refracted through the body
    vec2 refr = N.xy * thick * 0.045 * refraction;
    vec3 bg = abyssAt(uv + refr, pan, simPer);

    float lumBoost = glow * (0.9 + ar * (0.35 * bass + 0.25 * lev - 0.15));
    // luminescence wave through each body on beats + a slow idle breath
    float breath = 0.5 + 0.5 * sin(g_t * 1.3 + hue * 17.0 - A * 6.0);
    lumBoost *= 0.85 + 0.3 * breath + 0.6 * ar * beat * smoothstep(0.2, 0.8, A);

    // halo (kernel-smoothed U), tight
    vec3 col = bg;
    float halo = smoothstep(0.015, 0.16, U) * (1.0 - body);
    col += sc * halo * halo * 0.35 * lumBoost;

    // body interior: tinted, absorbing gel over the refracted abyss
    vec3 gel = bg * mix(vec3(1.0), sc * 0.6 + 0.15, thick * 0.85) * (1.0 - 0.45 * thick);
    gel += sc * (0.05 + 0.2 * thick * thick) * lumBoost;
    // inner lamellae = iso-surfaces of the body (they glide with it)
    float px = max(foot, 1e-3);
    float lam = 0.0;
    lam += 1.0 - smoothstep(0.5, 1.5, abs(A - 0.30) / gl / px);
    lam += 0.8 * (1.0 - smoothstep(0.5, 1.4, abs(A - 0.52) / gl / px));
    lam += 0.6 * (1.0 - smoothstep(0.5, 1.3, abs(A - 0.72) / gl / px));
    // fine growth rings between them
    // species anatomy: (ring frequency, lamella weight, nucleus threshold, granules)
    vec4 sp4 = speciesAnat(hue);
    float rings = 0.5 + 0.5 * cos(A * sp4.x);
    rings = pow(rings, 8.0) * smoothstep(0.12, 0.3, A) * (1.0 - smoothstep(0.85, 0.95, A));
    // granules: speckle keyed to the body's own fields, so it rides along with it
    float gran = vnoise(vec2(A * 26.0, U * 160.0) + hue * 40.0);
    gran = smoothstep(0.62, 0.8, gran) * smoothstep(0.15, 0.4, A) * sp4.w;
    gel += sc * (lam * 0.75 * sp4.y + rings * 0.22 + gran * 0.9) * lumBoost;
    // nucleus: white-hot core tinted by the species
    float core = smoothstep(sp4.z, sp4.z + 0.2, A);
    gel += mix(sc, vec3(1.0), 0.3) * core * 1.3 * lumBoost;
    // death bursts: tissue collapsing after a collision flares bright
    float dying = smoothstep(-0.25, -0.85, G) * smoothstep(0.04, 0.25, A);
    gel += mix(sc, vec3(1.0, 0.85, 0.6), 0.5) * dying * 1.4 * lumBoost;
    col = mix(col, gel, body);

    // membrane: crisp outer iso line + fresnel rim
    float mem = 1.0 - smoothstep(0.6, 1.7, abs(A - 0.06) / gl / px);
    float fres = pow(1.0 - clamp(N.z, 0.0, 1.0), 2.0);
    col += sc * mem * 1.15 * lumBoost;
    col += sc * fres * body * 0.8 * lumBoost;

    // wet speculars
    float spec = pow(clamp(dot(N, Hh), 0.0, 1.0), 64.0) * specAmt * smoothstep(0.05, 0.3, A);
    col += vec3(0.9, 0.97, 1.0) * spec * 1.4 * smoothstep(0.01, 0.06, gl * hs);

    // near marine snow / bokeh in front of everything
    vec2 q = uv * 11.0 + vec2(g_t * 0.12, g_t * 0.32);
    vec2 id = floor(q), f = fract(q) - 0.5;
    float h = hash12(id + 77.0);
    vec3 nearC = vec3(0.0);
    if (h < 0.05) {
        vec2 o = (hash22(id + 3.1) - 0.5) * 0.5;
        float d = length(f - o);
        float rad = 0.12 + 0.1 * hash12(id + 5.0);
        float bok = (1.0 - smoothstep(rad * 0.82, rad, d)) * (0.35 + 0.65 * smoothstep(rad * 0.45, rad * 0.95, d));
        nearC = vec3(0.5, 0.75, 0.95) * bok * 0.22 * snow * (1.0 + 1.5 * ar * high);
    }
    col += nearC;

    float alpha = clamp(body + mem + halo * 0.6 + lum(nearC), 0.0, 1.0);
    return vec4(col, alpha);
}

void main() {
    vec2 fc = gl_FragCoord.xy;
    vec2 R = RENDERSIZE;
    g_t = TIME;
    g_ar = audioReact;

    if (PASSINDEX == 0) {
        if (FRAMEINDEX < 2) { gl_FragColor = seedField(fc, R, kernelR); return; }
        gl_FragColor = lifeStep(fc, R);
        return;
    }
    if (PASSINDEX == 1) {
        g_simR = vec2(min(floor(R.x * simRes), 512.0), min(floor(R.y * simRes), 288.0));
        gl_FragColor = renderScene(fc, R);
        return;
    }

    // ── final ────────────────────────────────────────────────────────────
    vec2 st = fc / R;
    vec2 uvc = (fc - 0.5 * R) / R.y;
    vec2 asp = vec2(R.y / R.x, 1.0);
    vec4 c0 = texture2D(sceneBuf, st);
    float ab = aberration * 0.0022 * length(uvc);
    vec2 dir = normalize(uvc + 1e-4);
    vec3 col = vec3(texture2D(sceneBuf, st + dir * ab * asp).r, c0.g, texture2D(sceneBuf, st - dir * ab * asp).b);
    vec3 bl = vec3(0.0);
    for (int i = 0; i < 24; i++) {
        float fi = float(i);
        float a = fi * 2.399963;
        float rr = sqrt((fi + 0.5) / 24.0) * 0.032;
        vec2 of = vec2(cos(a), sin(a)) * rr * asp;
        vec3 s = texture2D(sceneBuf, st + of).rgb;
        bl += max(s - 0.45, 0.0) * (1.0 - rr / 0.032);
    }
    bl /= 24.0;
    col += bl * bloom * 2.0;
    col *= mix(1.0, 0.8 + 0.28 * levP() + 0.22 * bassP(), audioReact);
    col = aces(col * 1.1);
    col += (hash12(fc + fract(TIME) * 61.0) - 0.5) * 0.04 * grain;
    col *= 1.0 - 0.12 * smoothstep(0.5, 1.3, length(uvc));
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float l = lum(col);
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    gl_FragColor = vec4(col, transparentBg ? c0.a : 1.0);
}
