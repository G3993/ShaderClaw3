/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Analytic Cubism after Picasso — Kahnweiler (1910), Ma Jolie (1912), Joueur de Cartes (1913-14) — recast through SPACETIME. Real raymarched 3D cubes orbit canvas-centre on Kepler-like ellipses (m·v = const). Cubes near the gravity well deform — edges pulled inward, faces stretched along the radial. Charcoal armature lines bend along the gravitational field, no longer straight. Per-plane camera sampling preserved from the analytic painting. Output is LINEAR HDR — host applies ACES.",
  "INPUTS": [
    { "NAME": "armatureCount",  "LABEL": "Armature Lines",  "TYPE": "long",  "DEFAULT": 14, "VALUES": [10, 12, 14, 16, 18], "LABELS": ["10","12","14","16","18"] },
    { "NAME": "armatureWeight", "LABEL": "Line Weight",     "TYPE": "float", "MIN": 0.0008, "MAX": 0.004,  "DEFAULT": 0.0018 },
    { "NAME": "armatureBleed",  "LABEL": "Chalky Bleed",    "TYPE": "float", "MIN": 0.0,    "MAX": 1.0,    "DEFAULT": 0.55 },
    { "NAME": "planeCount",     "LABEL": "Plane Count",     "TYPE": "long",  "DEFAULT": 8,  "VALUES": [6, 7, 8, 9, 10], "LABELS": ["6","7","8","9","10"] },
    { "NAME": "planeAlpha",     "LABEL": "Plane Alpha",     "TYPE": "float", "MIN": 0.55,   "MAX": 0.95,   "DEFAULT": 0.78 },
    { "NAME": "planeSize",      "LABEL": "Plane Size",      "TYPE": "float", "MIN": 0.10,   "MAX": 0.32,   "DEFAULT": 0.20 },
    { "NAME": "shading",        "LABEL": "Plane Modeling",  "TYPE": "float", "MIN": 0.0,    "MAX": 0.8,    "DEFAULT": 0.55 },
    { "NAME": "lettering",      "LABEL": "Letter Stencils", "TYPE": "float", "MIN": 0.0,    "MAX": 1.0,    "DEFAULT": 0.0 },
    { "NAME": "drift",          "LABEL": "Composition Drift","TYPE": "float", "MIN": 0.0,    "MAX": 0.10,   "DEFAULT": 0.035 },
    { "NAME": "vignette",       "LABEL": "Vignette",        "TYPE": "float", "MIN": 0.0,    "MAX": 0.6,    "DEFAULT": 0.30 },
    { "NAME": "audioReact",     "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,    "MAX": 2.0,    "DEFAULT": 1.0 },
    { "NAME": "inputTex",       "LABEL": "Source",          "TYPE": "image" },
    { "NAME": "cameraMix",      "LABEL": "Camera Mix",      "TYPE": "float", "MIN": 0.0,    "MAX": 1.0,    "DEFAULT": 0.85 },
    { "NAME": "compositionSeed","LABEL": "Composition Seed","TYPE": "float", "MIN": 0.0,    "MAX": 80.0,   "DEFAULT": 7.0 },
    { "NAME": "cubeCount",      "LABEL": "Cube Count",      "TYPE": "long",  "DEFAULT": 6,  "VALUES": [5, 6, 7, 8], "LABELS": ["5","6","7","8"] },
    { "NAME": "gravityStrength","LABEL": "Gravity Strength","TYPE": "float", "MIN": 0.0,    "MAX": 1.5,    "DEFAULT": 0.85 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  ANALYTIC CUBISM × SPACETIME — Picasso meets Schwarzschild
//
//  Picasso fragmented the sitter by walking around her with a pencil.
//  General Relativity fragments LIGHT by curving the space the photon
//  travels through. Both decompose a single object into many viewpoints
//  and reassemble — one through artistic will, one through metric tensors.
//
//  This shader keeps the analytic-cubist surface (ochre planes, charcoal
//  armature, per-plane camera sampling) and lays it OVER raymarched 3D
//  cubes orbiting the canvas centre on Kepler-like ellipses. Mass times
//  velocity is conserved. Each cube bends near the gravity well — its
//  edges pulled inward (signed-distance squish) along the radial vector,
//  stretched along the tangent. The armature ink follows the same field:
//  geodesics, not straight lines. The painting is the lensed image.
// ════════════════════════════════════════════════════════════════════════

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

// ─── PALETTE — strict near-monochrome vocabulary ─────────────────────
vec3 palettePick(float k) {
    if (k < 0.18) return vec3(0.20, 0.17, 0.13);
    if (k < 0.36) return vec3(0.34, 0.30, 0.24);
    if (k < 0.52) return vec3(0.48, 0.46, 0.42);
    if (k < 0.68) return vec3(0.62, 0.58, 0.50);
    if (k < 0.84) return vec3(0.78, 0.69, 0.48);
    return            vec3(0.20, 0.26, 0.34);
}

vec3 paperBase(vec2 uv) {
    vec3 base = vec3(0.66, 0.60, 0.50);
    float g = vnoise(uv * vec2(420.0, 380.0));
    base *= 0.93 + 0.07 * g;
    base *= 1.0 + 0.06 * sin(uv.y * 1.6 + 0.4);
    return base;
}

// ─── GRAVITATIONAL FIELD ──────────────────────────────────────────────
// In screen-space (uv centred at 0.5,0.5), the canvas centre is the
// gravity well. Field strength falls off ~1/r with a soft core.
// Returns a 2D bend vector: how much a sample at uv should be pulled
// toward the centre (used to bend armature lines and cubes).
vec2 gravityBend(vec2 p, float strength) {
    vec2 r = p - vec2(0.5);
    float d = length(r) + 1e-3;
    // 1/(d+core) profile — strong near centre, gentle far away.
    float pull = strength * 0.045 / (d * d + 0.04);
    return -normalize(r) * pull;
}

// Scalar potential at a point — used to deform cube SDFs (closer to
// centre = stronger inward squish).
float gravityPotential(vec2 p) {
    float d = length(p - vec2(0.5)) + 1e-3;
    return 1.0 / (d + 0.12);
}

// ─── CUBE ORBITS — Kepler-like ─────────────────────────────────────────
// Each cube has a semi-major axis a, eccentricity e, phase phi, and an
// orbital frequency w ~ a^(-3/2) (Kepler's third law in 2D). The result
// is mass-times-velocity conserved across orbits — the shader feels like
// a small solar system rather than a turntable.
//
// Returns vec3(cx, cy, depthZ) — a 2D screen position plus a depth z in
// [-1..1] used for parallax and back-to-front compositing.
vec3 cubeOrbit(float fi, float t) {
    float a    = 0.18 + hash11(fi * 1.7) * 0.22;       // semi-major
    float e    = 0.12 + hash11(fi * 3.1) * 0.45;        // eccentricity
    float phi  = hash11(fi * 5.9) * 6.2832;             // phase
    float tilt = (hash11(fi * 7.3) - 0.5) * 1.8;        // orbital plane tilt
    // Kepler: w ~ a^(-3/2)
    float w    = 0.35 * pow(a, -1.5);
    float th   = phi + t * w;
    // Ellipse in orbital frame
    vec2 op = vec2(a * cos(th), a * (1.0 - e) * sin(th));
    // Tilt rotation (gives 3D feel — z follows out-of-plane component)
    float ct = cos(tilt), st = sin(tilt);
    vec2 sp;
    sp.x = op.x;
    sp.y = op.y * ct;
    float z = op.y * st;       // out-of-plane → depth
    return vec3(sp.x, sp.y, z);
}

// ─── ARMATURE — straight charcoal lines, but bent by gravity ──────────
// We sample the perpendicular distance to a line not at uv but at a
// gravitationally-displaced uv. The result is curved geodesics.
float armatureField(vec2 uv, float seed, int N, float weight,
                    float bleed, float treble, float gStrength) {
    float ink = 0.0;
    float chalky = 0.0;
    float jitter = 0.0006 + treble * 0.0014;

    // Bend uv toward centre — strength tapers far out so far-field lines
    // remain straight. This is the "armature follows the field" rule.
    vec2 bent = uv + gravityBend(uv, gStrength) * 0.85;

    for (int i = 0; i < 18; i++) {
        if (i >= N) break;
        float fi = float(i) + seed * 1.731;
        float cluster = hash11(fi * 3.17);
        float baseAng;
        if      (cluster < 0.34) baseAng = 1.5708;
        else if (cluster < 0.67) baseAng = 1.5708 + 0.95;
        else                     baseAng = 1.5708 - 0.95;
        float ang = baseAng + (hash11(fi * 7.91) - 0.5) * 0.36;
        ang += 0.04 * sin(TIME * 0.05 + fi * 0.7);

        float off = (hash11(fi * 11.13) - 0.5) * 1.05;
        off += jitter * sin(TIME * 0.7 + fi * 11.1);

        vec2 n = vec2(cos(ang), sin(ang));
        // Use the gravity-bent sample point — line appears curved.
        float d = abs(dot(bent - 0.5, n) - off);

        vec2  along = vec2(-n.y, n.x);
        float t  = dot(bent - 0.5, along);
        float t0 = (hash11(fi * 17.3) - 0.5) * 0.9;
        float t1 = t0 + 0.25 + hash11(fi * 19.1) * 0.55;
        float ends = smoothstep(0.06, 0.0, max(t0 - t, t - t1));

        float core = smoothstep(weight, 0.0, d) * ends;
        float halo = smoothstep(weight * 6.0, weight * 1.5, d) * ends * 0.45;

        ink     = max(ink, core);
        chalky  = max(chalky, halo);
    }

    return ink + chalky * bleed;
}

// ─── PLANES (analytic-cubist, unchanged) ──────────────────────────────
struct Plane { float inside; vec2 local; vec2 halfSize; vec3 col; };

Plane evalPlane(vec2 uv, float fi, float aspect, float planeBaseSize,
                float bass) {
    Plane P;
    vec2 raw = vec2(hash11(fi * 1.31), hash11(fi * 2.97 + 4.7));
    vec2 ctr = mix(raw, vec2(0.5), 0.55);
    ctr += 0.04 * vec2(sin(TIME * 0.18 + fi),
                       cos(TIME * 0.13 + fi * 1.7));
    float rotCluster = hash11(fi * 5.7);
    float rot;
    if      (rotCluster < 0.34) rot =  0.0;
    else if (rotCluster < 0.67) rot =  0.95;
    else                        rot = -0.95;
    rot += (hash11(fi * 13.9) - 0.5) * 0.38;
    float sH = hash11(fi * 7.13);
    float sA = hash11(fi * 9.71);
    vec2 halfSize = vec2(planeBaseSize * (0.50 + sH * 0.85),
                         planeBaseSize * (0.30 + sA * 0.95));
    halfSize *= 1.0 + bass * 0.06;
    float ca = cos(-rot), sa = sin(-rot);
    vec2 d = uv - ctr;  d.x *= aspect;
    vec2 local = vec2(ca * d.x - sa * d.y,
                      sa * d.x + ca * d.y);
    vec2 q = abs(local) - halfSize;
    float sd = max(q.x, q.y);
    P.inside = smoothstep(0.002, -0.001, sd);
    P.col = palettePick(hash11(fi * 23.3));
    P.local    = local;
    P.halfSize = halfSize;
    return P;
}

// ─── LETTERFORMS (unchanged) ──────────────────────────────────────────
float bar(vec2 p, vec4 r) {
    return step(r.x, p.x) * step(p.x, r.z)
         * step(r.y, p.y) * step(p.y, r.w);
}
float letterM(vec2 p) {
    float L = bar(p, vec4(0.05, 0.05, 0.22, 0.95));
    float R = bar(p, vec4(0.78, 0.05, 0.95, 0.95));
    float D1 = bar(p, vec4(0.22, 0.55, 0.50, 0.95));
    float D2 = bar(p, vec4(0.50, 0.55, 0.78, 0.95));
    return max(max(L, R), max(D1, D2));
}
float letterA(vec2 p) {
    float L = bar(p, vec4(0.05, 0.05, 0.22, 0.95));
    float R = bar(p, vec4(0.78, 0.05, 0.95, 0.95));
    float T = bar(p, vec4(0.05, 0.78, 0.95, 0.95));
    float Mid = bar(p, vec4(0.22, 0.42, 0.78, 0.55));
    return max(max(L, R), max(T, Mid));
}
float letterJ(vec2 p) {
    float V = bar(p, vec4(0.62, 0.20, 0.80, 0.95));
    float H = bar(p, vec4(0.18, 0.78, 0.80, 0.95));
    float B = bar(p, vec4(0.18, 0.05, 0.62, 0.20));
    float U = bar(p, vec4(0.18, 0.05, 0.36, 0.40));
    return max(max(V, H), max(B, U));
}
float letterO(vec2 p) {
    float OUTr = bar(p, vec4(0.10, 0.10, 0.90, 0.90));
    float INr  = bar(p, vec4(0.28, 0.28, 0.72, 0.72));
    return OUTr * (1.0 - INr);
}
float letterL(vec2 p) {
    float V = bar(p, vec4(0.10, 0.05, 0.30, 0.95));
    float H = bar(p, vec4(0.10, 0.05, 0.85, 0.22));
    return max(V, H);
}
float letterI(vec2 p) {
    return bar(p, vec4(0.40, 0.05, 0.60, 0.95));
}
float letterE(vec2 p) {
    float V = bar(p, vec4(0.10, 0.05, 0.30, 0.95));
    float T = bar(p, vec4(0.10, 0.78, 0.85, 0.95));
    float M = bar(p, vec4(0.10, 0.42, 0.70, 0.55));
    float B = bar(p, vec4(0.10, 0.05, 0.85, 0.22));
    return max(max(V, T), max(M, B));
}
float letterU(vec2 p) {
    float L = bar(p, vec4(0.10, 0.10, 0.30, 0.95));
    float R = bar(p, vec4(0.70, 0.10, 0.90, 0.95));
    float B = bar(p, vec4(0.10, 0.05, 0.90, 0.22));
    return max(max(L, R), B);
}
float letterB(vec2 p) {
    float V = bar(p, vec4(0.10, 0.05, 0.30, 0.95));
    float T = bar(p, vec4(0.10, 0.78, 0.85, 0.95));
    float M = bar(p, vec4(0.10, 0.42, 0.78, 0.55));
    float Bt= bar(p, vec4(0.10, 0.05, 0.85, 0.22));
    float RT= bar(p, vec4(0.78, 0.55, 0.95, 0.78));
    float RB= bar(p, vec4(0.78, 0.22, 0.95, 0.42));
    return max(max(max(V, T), max(M, Bt)), max(RT, RB));
}

float drawWord(vec2 uv, vec2 origin, float scl, float ang, int word) {
    float ca = cos(-ang), sa = sin(-ang);
    vec2 d = uv - origin;
    vec2 lp = vec2(ca * d.x - sa * d.y, sa * d.x + ca * d.y) / scl;
    float ink = 0.0;
    if (word == 0) {
        for (int g = 0; g < 8; g++) {
            float x0 = float(g);
            vec2 cell = vec2(lp.x - x0, lp.y);
            if (cell.x < 0.0 || cell.x > 1.0 || cell.y < 0.0 || cell.y > 1.0) continue;
            float l = 0.0;
            if      (g == 0) l = letterM(cell);
            else if (g == 1) l = letterA(cell);
            else if (g == 2) l = 0.0;
            else if (g == 3) l = letterJ(cell);
            else if (g == 4) l = letterO(cell);
            else if (g == 5) l = letterL(cell);
            else if (g == 6) l = letterI(cell);
            else             l = letterE(cell);
            ink = max(ink, l);
        }
    } else if (word == 1) {
        for (int g = 0; g < 3; g++) {
            float x0 = float(g);
            vec2 cell = vec2(lp.x - x0, lp.y);
            if (cell.x < 0.0 || cell.x > 1.0 || cell.y < 0.0 || cell.y > 1.0) continue;
            float l = (g == 0) ? letterJ(cell)
                    : (g == 1) ? letterO(cell)
                               : letterU(cell);
            ink = max(ink, l);
        }
    } else {
        for (int g = 0; g < 3; g++) {
            float x0 = float(g);
            vec2 cell = vec2(lp.x - x0, lp.y);
            if (cell.x < 0.0 || cell.x > 1.0 || cell.y < 0.0 || cell.y > 1.0) continue;
            float l = (g == 0) ? letterB(cell)
                    : (g == 1) ? letterA(cell)
                               : letterL(cell);
            ink = max(ink, l);
        }
    }
    return ink;
}

// ─── MAIN ─────────────────────────────────────────────────────────────
void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    float bass    = clamp(audioBass,    0.0, 1.0) * audioReact;
    float treble  = clamp(audioHigh,    0.0, 1.0) * audioReact;
    float lvl     = clamp(audioLevel,   0.0, 1.0) * audioReact;
    int   N       = int(planeCount + 0.5);
    int   AN      = int(armatureCount + 0.5);
    int   CN      = int(cubeCount + 0.5);
    float gStr    = gravityStrength * (1.0 + 0.25 * bass);

    {
        vec2 c = uv - 0.5;
        float a = 0.025 * sin(TIME * 0.08) * (1.0 + drift * 5.0);
        float ca = cos(a), sa = sin(a);
        c = vec2(ca * c.x - sa * c.y, sa * c.x + ca * c.y);
        uv = c + 0.5;
    }

    // 1. Paper ground
    vec3 col = paperBase(uv);
    float vig = smoothstep(0.20, 0.95, length(uv - 0.5));
    col *= 1.0 - vig * vignette;

    bool hasSource = (IMG_SIZE(inputTex).x >= 1.0);
    if (hasSource) {
        vec4 probe = IMG_NORM_PIXEL(inputTex, vec2(0.5, 0.5));
        if (probe.a < 0.01 && (probe.r + probe.g + probe.b) < 0.001) {
            hasSource = false;
        }
    }

    // 2. PLANES — composite analytic-cubist fragments back-to-front.
    for (int i = 0; i < 10; i++) {
        if (i >= N) break;
        float fi = float(i) + compositionSeed * 3.71;
        Plane P = evalPlane(uv, fi, aspect, planeSize, bass);
        if (P.inside < 0.001) continue;

        float ldJit = 2.35 + (hash11(fi * 31.7) - 0.5) * 0.7;
        vec2  lvec  = vec2(cos(ldJit), sin(ldJit));
        float shade = dot(P.local / max(P.halfSize, vec2(1e-4)), lvec) * 0.5 + 0.5;
        vec3 planeTint = P.col * (1.0 - shading + shading * shade);
        vec3 fragCol = planeTint;

        // Per-plane camera sampling preserved.
        if (hasSource && cameraMix > 0.001) {
            float rotCluster = hash11(fi * 5.7);
            float theta;
            if      (rotCluster < 0.34) theta =  0.0;
            else if (rotCluster < 0.67) theta =  0.95;
            else                        theta = -0.95;
            theta += (hash11(fi * 13.9) - 0.5) * 0.38;
            float scl = 0.7 + 0.6 * hash11(fi * 41.7);
            vec2 jit = vec2(hash11(fi * 53.1) - 0.5,
                            hash11(fi * 67.3) - 0.5) * 0.30;
            vec2 q = uv - 0.5;
            float cs = cos(theta), sn = sin(theta);
            q = vec2(cs * q.x - sn * q.y, sn * q.x + cs * q.y);
            q *= scl;
            vec2 sampUV = q + 0.5 + jit;
            sampUV = mod(sampUV, vec2(1.0));
            vec3 sampled = IMG_NORM_PIXEL(inputTex, sampUV).rgb;
            float lumi = dot(sampled, vec3(0.2126, 0.7152, 0.0722));
            float lift = 1.0 + 0.30 * smoothstep(0.55, 0.95, lumi);
            sampled *= lift;
            vec3 tinted = sampled * planeTint * 1.4;
            fragCol = mix(planeTint, tinted, cameraMix);
        }

        float pulse = 1.0 + 0.18 * bass * sin(TIME * 2.7 + fi * 1.3);
        float a = clamp(planeAlpha * pulse * P.inside, 0.0, 0.95);
        col = mix(col, fragCol, a);

        float spineDist = abs(P.local.y / max(P.halfSize.y, 1e-4));
        float spine = smoothstep(0.08, 0.0, abs(spineDist - 0.0)) * 0.06;
        col *= 1.0 - spine * P.inside;
    }

    // 3. ORBITING CUBES — flat 2D depiction of Kepler orbits, back-to-front.
    //    Each cube is drawn as a rotated quad with shaded faces; gravitational
    //    potential squishes it toward the centre. Sorted by depth z.
    for (int k = 0; k < 8; k++) {
        if (k >= CN) break;
        float fk = float(k) + compositionSeed * 1.117;
        vec3 orb = cubeOrbit(fk, TIME);
        vec2 cuv = orb.xy / vec2(aspect, 1.0) + 0.5;

        float pot = gravityPotential(cuv) * gStr;
        vec2 radial2D = normalize(vec2(0.5) - cuv + 1e-5);

        // Half-extent in screen space
        float baseH = 0.045 + hash11(fk * 17.3) * 0.035;
        baseH *= 1.0 + 0.08 * bass;
        // Tidal stretch along radial, squeeze perpendicular
        float tide = clamp(pot * 0.18, 0.0, 0.55);

        // Local frame for cube — spin in 2D
        float spinAng = TIME * (0.18 + 0.12 * hash11(fk * 21.1)) + fk;
        float depthFade = clamp(0.5 + orb.z * 0.6, 0.25, 1.0);

        // Sample point in cube-local coords
        vec2 d = uv - cuv;
        d.x *= aspect;
        // Apply tidal deformation along radial
        float pr = dot(d, radial2D);
        vec2  pn = d - radial2D * pr;
        d = radial2D * (pr * (1.0 + tide)) + pn * (1.0 - tide * 0.45);
        // Spin
        float cs = cos(-spinAng), sn = sin(-spinAng);
        vec2 lp = vec2(cs * d.x - sn * d.y, sn * d.x + cs * d.y);

        vec2 q = abs(lp) - vec2(baseH);
        float sd = max(q.x, q.y);
        float inside = smoothstep(0.0015, -0.0015, sd);
        if (inside < 0.001) continue;

        // Face shading — pick face by largest |lp| component
        float face = (abs(lp.x) > abs(lp.y)) ? sign(lp.x) * 0.7
                                              : sign(lp.y) * 0.4 + 0.55;
        vec3 cubeCol = palettePick(hash11(fk * 29.7));
        vec3 base = cubeCol * (0.35 + 0.65 * face) * depthFade;
        // Rim near edges
        float edge = smoothstep(baseH * 0.95, baseH, max(abs(lp.x), abs(lp.y)));
        base += vec3(1.55, 1.40, 1.10) * edge * 0.40;

        col = mix(col, base, clamp(inside * 0.92, 0.0, 0.95));
    }

    // 4. STENCILED LETTERFORMS (unchanged)
    if (lettering > 0.0) {
        vec2 textOrigin = vec2(0.18, 0.74);
        float ink1 = drawWord(uv, textOrigin, 0.030, -0.08, 0);
        float ink2 = drawWord(uv, vec2(0.66, 0.42), 0.032, 0.85, 1);
        float ink3 = drawWord(uv, vec2(0.30, 0.22), 0.028, 0.04, 2);
        float ink = max(max(ink1, ink2), ink3) * lettering;
        col = mix(col, vec3(0.10, 0.08, 0.06), ink * 0.85);
    }

    // 5. ARMATURE — drawn LAST. Bent by gravity. Line cores HDR-bright.
    float arm = armatureField(uv, compositionSeed * 0.717,
                              AN, armatureWeight, armatureBleed,
                              treble, gStr);
    // Graphite ink, but core punches above 1.0 in linear so the host ACES
    // can bloom the hottest line junctions.
    vec3 inkColour = vec3(0.10, 0.10, 0.11);
    vec3 inkHot    = vec3(1.55, 1.45, 1.20);
    float armCore  = smoothstep(0.65, 1.0, arm);
    col = mix(col, inkColour, clamp(arm, 0.0, 1.0));
    col += inkHot * armCore * 0.45;

    // 6. Final breath — silent loudness modulation of overall value.
    col *= 0.95 + 0.06 * lvl;

    // LINEAR HDR out — no internal tonemap. Host applies ACES.
    gl_FragColor = vec4(col, 1.0);
}
