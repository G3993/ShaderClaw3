/*{
  "DESCRIPTION": "Prism — Newton's experimentum crucis staged as a light installation. A slit lamp throws a sheet of white light through a glass prism that fans it into a true spectrum; a second, inverted prism catches the fan and folds every colour back into a single white beam, which then strikes a slowly turning pentagonal crystal that shatters it again into several rainbows that stand as tall bars of colour on a dark matte screen. Every wavelength from deep violet to far red is traced on its own path through all three bodies — real Cauchy dispersion, Snell refraction, Fresnel losses and total internal reflection — and coloured through the CIE colour-matching functions, so overlapping spectra mix additively and fringes are real. Across the cycle the second prism drifts out of true (the white beam frays into colour), rises clear of the light entirely (the full fan swings across the screen as one huge rainbow), and drops back to snap the light white again, while the crystal's facets sweep its spectra up and down the screen. Dust hangs in every beam, the glass glows where the light runs through it, flares burn at each face, caustic glints spark on the floor beneath the glass, and the camera drifts between the whole installation and close macro views of the light leaving the glass. Bass brightens the lamp, beats shimmer the dust, mids nudge the crystal, highs sparkle the motes.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #4",
  "CATEGORIES": ["Generator", "3D", "Light", "Science", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Cycle Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "crystalSpin",  "LABEL": "Crystal Spin",      "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "orbitSpeed",   "LABEL": "Camera Move",       "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "dustDrift",    "LABEL": "Dust Drift",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Speed" },
    { "NAME": "dispersion",   "LABEL": "Dispersion",        "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "glassIndex",   "LABEL": "Prism Glass Index", "TYPE": "float", "AUDIO": false, "MIN": 1.45, "MAX": 1.8, "DEFAULT": 1.62, "GROUP": "Optics" },
    { "NAME": "crystalIndex", "LABEL": "Crystal Index",     "TYPE": "float", "AUDIO": false, "MIN": 1.35, "MAX": 1.7, "DEFAULT": 1.5, "GROUP": "Optics" },
    { "NAME": "recombine",    "LABEL": "Recombination",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Optics" },
    { "NAME": "beamWidth",    "LABEL": "Beam Width",        "TYPE": "float", "AUDIO": false, "MIN": 0.03, "MAX": 0.2, "DEFAULT": 0.11, "GROUP": "Optics" },
    { "NAME": "beamHeight",   "LABEL": "Sheet Depth",       "TYPE": "float", "AUDIO": false, "MIN": 0.02, "MAX": 0.3, "DEFAULT": 0.12, "GROUP": "Optics" },
    { "NAME": "fanOut",       "LABEL": "Sheet Spread",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 0.15, "DEFAULT": 0.10, "GROUP": "Optics" },
    { "NAME": "beamIntensity","LABEL": "Lamp Intensity",    "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "haze",         "LABEL": "Haze",              "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "dust",         "LABEL": "Dust",              "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Light" },
    { "NAME": "glassGlow",    "LABEL": "Glass Glow",        "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.6, "GROUP": "Light" },
    { "NAME": "flares",       "LABEL": "Face Flares",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "floorGloss",   "LABEL": "Floor Gloss",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Light" },
    { "NAME": "roomLight",    "LABEL": "Room Light",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "beamColor",    "LABEL": "Lamp Colour",       "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0], "GROUP": "Color" },
    { "NAME": "wallColor",    "LABEL": "Screen Colour",     "TYPE": "color", "DEFAULT": [0.78, 0.77, 0.76, 1.0], "GROUP": "Color" },
    { "NAME": "roomColor",    "LABEL": "Room Tint",         "TYPE": "color", "DEFAULT": [0.16, 0.12, 0.32, 1.0], "GROUP": "Color" },
    { "NAME": "saturation",   "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.05, "GROUP": "Color" },
    { "NAME": "exposure",     "LABEL": "Exposure",          "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "bloom",        "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Finish" },
    { "NAME": "streak",       "LABEL": "Anamorphic Streak", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "GROUP": "Finish" },
    { "NAME": "grain",        "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "marchRes",     "LABEL": "Render Resolution", "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 1.0, "DEFAULT": 0.75, "GROUP": "Finish" },
    { "NAME": "audioReact",   "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "rayBuf", "WIDTH": "24", "HEIGHT": "23" },
    { "TARGET": "sceneBuf", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { "TARGET": "bloomA", "WIDTH": "floor($WIDTH/4)", "HEIGHT": "floor($HEIGHT/4)" },
    { "TARGET": "bloomB", "WIDTH": "floor($WIDTH/12)", "HEIGHT": "floor($HEIGHT/12)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Prism — CURVEBALL pack #4: Newton's experimentum crucis as an installation
//
//   All light travels in one vertical sheet (the x/y plane, z = depth). The
//   sheet starts sheetDepth thick at the slit and spreads by fanOut per metre
//   of path, so far spectra land as big rectangles on the screen.
//   Bodies (2D convex polygons extruded along z):
//     0  prism 1   (apex up)     — fans the white beam into a spectrum
//     1  prism 2   (prism 1 rotated 180°) — folds the fan back to white
//     2  crystal   (pentagon, spinning)   — shatters it into several spectra
//
//   pass 0  rayBuf (NL wavelengths × 23 rows): every wavelength × 3 sub-rays
//           (across the beam width) traced through the system — up to three
//           air segments each (after body 1, 2, 3): origin, dir, length,
//           path length, transmittance, angular spread to neighbour λ.
//           + CIE colour row, + in-glass chords, + lamp→glass length.
//   pass 1  sceneBuf: analytic room (floor, screen, back wall, rods, lamp,
//           glass); surfaces lit by every wavelength wedge; camera rays
//           gather in-scatter through the dusty sheet; glass refracts the
//           room behind it; flares at every face crossing.
//   pass 2-3 bloom pyramid       pass 4  finish (ACES, HD finisher)
// ─────────────────────────────────────────────────────────────────────────

#define NL    24
#define NLF   24.0
#define NROW  23.0
#define PI    3.14159265359
#define TAU   6.28318530718
#define XW    3.5            // screen wall plane x (faces −x)
#define ZB    3.6            // back wall plane z
#define HREF  0.05
#define GDP   0.24           // prism half depth (z)
#define GDC   0.42           // crystal half depth (z)
#define CR    0.19           // crystal radius
#define YC    1.05           // height of the light sheet's centre
#define LIFTH 0.95           // how far prism 2 rises when lifted out of the light
#define PL    0.42           // prism side length

float bassP() { return smoothstep(0.04, 0.85, audioBass); }
float midP()  { return smoothstep(0.05, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.90, audioHigh); }
float beatP() { return clamp(audioBeatPulse, 0.0, 1.0); }

float hash12(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float hash13(vec3 p3) { p3 = fract(p3 * 0.1031); p3 += dot(p3, p3.zyx + 31.32); return fract((p3.x + p3.y) * p3.z); }
float vnoise3(vec3 x) {
    vec3 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    float a = hash13(p), b = hash13(p + vec3(1,0,0)), c = hash13(p + vec3(0,1,0)), d = hash13(p + vec3(1,1,0));
    float e = hash13(p + vec3(0,0,1)), g = hash13(p + vec3(1,0,1)), h = hash13(p + vec3(0,1,1)), k = hash13(p + vec3(1,1,1));
    return mix(mix(mix(a,b,f.x), mix(c,d,f.x), f.y), mix(mix(e,g,f.x), mix(h,k,f.x), f.y), f.z);
}
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1,0)), f.x), mix(hash12(p + vec2(0,1)), hash12(p + vec2(1,1)), f.x), f.y);
}
float cross2(vec2 a, vec2 b) { return a.x * b.y - a.y * b.x; }
vec2 rot2(vec2 v, float a) { float c = cos(a), s = sin(a); return vec2(c * v.x - s * v.y, s * v.x + c * v.y); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
vec3 aces(vec3 x) { return clamp((x * (2.51 * x + 0.03)) / (x * (2.43 * x + 0.59) + 0.14), 0.0, 1.0); }
float band(float x, float lo, float hi, float s) { return smoothstep(lo - s, lo + s, x) * (1.0 - smoothstep(hi - s, hi + s, x)); }

// ═════════════════════════════════════════════════════════════════════════
//  OPTICS (shared by every pass — depends only on time + inputs)
// ═════════════════════════════════════════════════════════════════════════
float g_t;
vec2  g_src;                       // slit aperture
vec2  g_c0, g_c1, g_c2;            // body centres
float g_a0, g_a1, g_a2;            // body rotations
vec3  g_f00, g_f01, g_f02, g_f10, g_f11, g_f12, g_f20, g_f21, g_f22, g_f23, g_f24;   // face planes (n.xy, d)
float g_lift;                      // 0 = prism 2 in the fan, 1 = lifted clear
float g_mis;                       // prism 2 misalignment (rad)

float smooth01(float a, float b, float x) { return smoothstep(a, b, x); }

vec3 triFace(int k, vec2 c, float th) {
    float A = radians(40.0);
    float h = PL * cos(0.5 * A), a = 2.0 * h / 3.0, b = h / 3.0;
    vec3 f;
    if (k == 0) f = vec3(-cos(0.5 * A), sin(0.5 * A), a * sin(0.5 * A));
    else if (k == 1) f = vec3(cos(0.5 * A), sin(0.5 * A), a * sin(0.5 * A));
    else f = vec3(0.0, -1.0, b);
    vec2 n = rot2(f.xy, th);
    return vec3(n, f.z + dot(n, c));
}
vec3 pentFace(int k, vec2 c, float th) {
    float a = th + float(k) * TAU / 5.0;
    vec2 n = vec2(cos(a), sin(a));
    return vec3(n, CR * cos(PI / 5.0) + dot(n, c));
}
vec2 triVert(int k, vec2 c, float th) {
    float A = radians(40.0);
    float h = PL * cos(0.5 * A), a = 2.0 * h / 3.0, b = h / 3.0;
    vec2 v = k == 0 ? vec2(0.0, a) : (k == 1 ? vec2(h * tan(0.5 * A), -b) : vec2(-h * tan(0.5 * A), -b));
    return c + rot2(v, th);
}

void setupOptics() {
    float A = radians(40.0);
    float dmin = 2.0 * asin(clamp(glassIndex * sin(0.5 * A), -1.0, 1.0)) - A;
    float PER = 64.0;
    float tc = g_t * speed;
    float u = fract(tc / PER + 0.02);
    // ── the cycle ────────────────────────────────────────────────────────
    //  0.00–0.30  aligned: white beam → crystal → shattered spectra
    //  0.30–0.44  prism 2 drifts out of true: the white beam frays into colour
    //  0.44–0.52  prism 2 lifts clear: the full fan floods the floor
    //  0.52–0.80  prism 1 sweeps the big spectrum
    //  0.80–0.84  prism 2 drops back in — recombination snaps white
    float drift = smooth01(0.30, 0.42, u) * (1.0 - smooth01(0.80, 0.86, u));
    g_mis = drift * radians(7.0) * sin(tc * 0.35 + 0.8) + (1.0 - recombine) * radians(12.0);
    g_lift = smooth01(0.44, 0.52, u) * (1.0 - smooth01(0.80, 0.835, u));
    float sweep = smooth01(0.50, 0.56, u) * (1.0 - smooth01(0.76, 0.80, u));
    float a1 = -0.5 * dmin + sweep * 0.10 * sin(tc * 0.22) + 0.012 * sin(tc * 0.13);

    g_src = vec2(-2.7, 1.25);
    g_c0 = vec2(-1.75, 1.25);  g_a0 = a1;
    // prism 2 sits 0.30 down the fan, as prism 1 rotated by 180° (that exact pose
    // undoes the dispersion for every wavelength); when lifted it rides up its hanger
    vec2 fanDir = rot2(vec2(1.0, 0.0), -dmin);
    g_c1 = vec2(-1.3873, 1.0629);  g_a1 = a1 + PI + g_mis;
    g_c2 = vec2(0.35, 1.0629);   g_a2 = g_t * 0.075 * crystalSpin + 0.06 * audioReact * midP();

    g_f00 = triFace(0, g_c0, g_a0); g_f01 = triFace(1, g_c0, g_a0); g_f02 = triFace(2, g_c0, g_a0);
    g_f10 = triFace(0, g_c1, g_a1); g_f11 = triFace(1, g_c1, g_a1); g_f12 = triFace(2, g_c1, g_a1);
    g_f20 = pentFace(0, g_c2, g_a2); g_f21 = pentFace(1, g_c2, g_a2); g_f22 = pentFace(2, g_c2, g_a2);
    g_f23 = pentFace(3, g_c2, g_a2); g_f24 = pentFace(4, g_c2, g_a2);
}

vec3 facePlane(int e, int k) {
    if (e == 0) { if (k == 0) return g_f00; if (k == 1) return g_f01; return g_f02; }
    if (e == 1) { if (k == 0) return g_f10; if (k == 1) return g_f11; return g_f12; }
    if (k == 0) return g_f20; if (k == 1) return g_f21; if (k == 2) return g_f22; if (k == 3) return g_f23; return g_f24;
}

float cauchyN(int e, float lamNm) {
    float lu = lamNm * 0.001;
    float B = 0.042 * dispersion;
    float x = 1.0 / (lu * lu) - 1.0 / (0.55 * 0.55);
    return e == 2 ? crystalIndex + 2.8 * B * x : glassIndex + B * x;
}

float fresnelD(float cosi, float n1, float n2) {
    cosi = clamp(cosi, 0.0, 1.0);
    float s = n1 / n2 * sqrt(max(0.0, 1.0 - cosi * cosi));
    if (s >= 1.0) return 1.0;
    float cost = sqrt(1.0 - s * s);
    float rs = (n1 * cosi - n2 * cost) / (n1 * cosi + n2 * cost);
    float rp = (n1 * cost - n2 * cosi) / (n1 * cost + n2 * cosi);
    return 0.5 * (rs * rs + rp * rp);
}

// 2D ray entering body e from outside → t, outward normal of the entry face
float bodyEnter(int e, vec2 o, vec2 d, out vec2 nn) {
    float tn = -1e9, tf = 1e9; nn = vec2(0.0);
    for (int k = 0; k < 5; k++) {
        if (e < 2 && k > 2) break;
        vec3 f = facePlane(e, k);
        float dn = dot(d, f.xy), dist = dot(o, f.xy) - f.z;
        if (abs(dn) < 1e-7) { if (dist > 0.0) return 1e9; continue; }
        float t = -dist / dn;
        if (dn < 0.0) { if (t > tn) { tn = t; nn = f.xy; } }
        else tf = min(tf, t);
    }
    if (tn > tf || tn < 1e-5) return 1e9;
    return tn;
}
// 2D ray inside body e → distance to the exit face, its outward normal
float bodyExit(int e, vec2 o, vec2 d, out vec2 nf) {
    float tf = 1e9; nf = vec2(0.0);
    for (int k = 0; k < 5; k++) {
        if (e < 2 && k > 2) break;
        vec3 f = facePlane(e, k);
        float dn = dot(d, f.xy);
        if (dn > 1e-7) { float t = (f.z - dot(o, f.xy)) / dn; if (t < tf) { tf = t; nf = f.xy; } }
    }
    return tf;
}

// trace one sub-ray through the system.
//   A_i = (origin, dir), B_i = (length, path length at origin, transmittance, 0)
//   for the air segment after the i-th body; G_e = (entry, exit) chord in body e
void traceSys(vec2 o, vec2 d, float lam,
              out vec4 A0, out vec4 B0, out vec4 A1, out vec4 B1, out vec4 A2, out vec4 B2,
              out vec4 G0, out vec4 G1, out vec4 G2, out float inLen) {
    A0 = vec4(0.0); B0 = vec4(0.0); A1 = vec4(0.0); B1 = vec4(0.0); A2 = vec4(0.0); B2 = vec4(0.0);
    G0 = vec4(0.0); G1 = vec4(0.0); G2 = vec4(0.0);
    inLen = 1e3;
    vec2 p = o, dr = d;
    float s = 0.0, T = 1.0;
    int passed = 0, lastE = -1;
    for (int step = 0; step < 4; step++) {
        float tb = 1e9; int eb = -1; vec2 nb = vec2(0.0);
        for (int e = 0; e < 3; e++) {
            if (e == lastE) continue;
            if (e == 1 && g_lift > 0.35) continue;      // prism 2 has risen clear of the sheet
            vec2 nn; float t = bodyEnter(e, p, dr, nn);
            if (t < tb) { tb = t; eb = e; nb = nn; }
        }
        float segL = eb < 0 ? 1e3 : tb;
        if (passed == 0) inLen = segL;
        else {
            vec4 a = vec4(p, dr), b = vec4(segL, s, T, 0.0);
            if (passed == 1) { A0 = a; B0 = b; } else if (passed == 2) { A1 = a; B1 = b; } else { A2 = a; B2 = b; }
        }
        if (eb < 0 || passed >= 3) break;
        p += dr * tb; s += tb;
        float n = cauchyN(eb, lam);
        T *= 1.0 - fresnelD(-dot(dr, nb), 1.0, n);
        dr = refract(dr, nb, 1.0 / n);
        vec2 entryP = p;
        bool outOK = false;
        for (int k = 0; k < 4; k++) {
            vec2 nf; float tf = bodyExit(eb, p, dr, nf);
            if (tf > 1e8) break;
            vec2 q = p + dr * tf; s += tf;
            vec2 tr = refract(dr, -nf, n);
            if (dot(tr, tr) < 0.5) { dr = reflect(dr, nf); p = q; continue; }
            T *= 1.0 - fresnelD(dot(dr, nf), n, 1.0);
            dr = normalize(tr); p = q; outOK = true;
            break;
        }
        vec4 g = vec4(entryP, p);
        if (eb == 0) G0 = g; else if (eb == 1) G1 = g; else G2 = g;
        if (!outOK) { T = 0.0; break; }
        lastE = eb; passed++;
    }
}

// ── CIE 1931 (Wyman, Sloan & Shirley multi-lobe fit) ─────────────────────
float glob(float x, float mu, float s1, float s2) { float s = x < mu ? s1 : s2; float u = (x - mu) / s; return exp(-0.5 * u * u); }
vec3 cieXYZ(float l) {
    float X = 1.056 * glob(l, 599.8, 37.9, 31.0) + 0.362 * glob(l, 442.0, 16.0, 26.7) - 0.065 * glob(l, 501.1, 20.4, 26.2);
    float Y = 0.821 * glob(l, 568.8, 46.9, 40.5) + 0.286 * glob(l, 530.9, 16.3, 31.1);
    float Z = 1.217 * glob(l, 437.0, 11.8, 36.0) + 0.681 * glob(l, 459.0, 26.0, 13.8);
    return vec3(X, Y, Z);
}
vec3 xyz2rgb(vec3 c) {
    return vec3( 3.2406 * c.x - 1.5372 * c.y - 0.4986 * c.z,
                -0.9689 * c.x + 1.8758 * c.y + 0.0415 * c.z,
                 0.0557 * c.x - 0.2040 * c.y + 1.0570 * c.z);
}
float lamOf(float i) { return mix(400.0, 700.0, (i + 0.5) / NLF); }
vec3 specRGB(float i) {
    vec3 c = xyz2rgb(cieXYZ(lamOf(i)));
    float y = max(lum(c), 0.0);
    float m = min(min(c.r, c.g), c.b);
    if (m < 0.0) c = mix(vec3(y), c, y / max(y - m, 1e-4));
    return max(c, 0.0);
}

// ═════════════════════════════════════════════════════════════════════════
//  PASS 0 — ray table
//   rows 0..17 : sub-ray j (0..2), segment i (0..2):  row 2(3j+i) = A,  +1 = B (B.w = spread)
//   row 18     : colour (white-balanced CIE)
//   rows 19..21: in-glass chord of the middle sub-ray, body 0..2
//   row 22     : lamp → first glass length (middle sub-ray)
// ═════════════════════════════════════════════════════════════════════════
vec4 pickSeg(int i, bool wantB, vec4 A0, vec4 B0, vec4 A1, vec4 B1, vec4 A2, vec4 B2) {
    if (i == 0) return wantB ? B0 : A0;
    if (i == 1) return wantB ? B1 : A1;
    return wantB ? B2 : A2;
}

vec4 rayPass(vec2 uv) {
    float fi = floor(clamp(uv.x, 0.0, 0.9999) * NLF);
    float r = floor(clamp(uv.y, 0.0, 0.9999) * NROW);
    float lam = lamOf(fi);
    vec4 A0, B0, A1, B1, A2, B2, G0, G1, G2; float inL;
    if (r < 17.5) {
        float q = floor(r * 0.5);
        float jf = floor(q / 3.0);
        int i = int(q - 3.0 * jf + 0.5);
        bool wantB = mod(r, 2.0) > 0.5;
        float off = (jf - 1.0) * beamWidth / 3.0;
        vec2 o = g_src + vec2(0.0, off);
        traceSys(o, vec2(1.0, 0.0), lam, A0, B0, A1, B1, A2, B2, G0, G1, G2, inL);
        vec4 v = pickSeg(i, wantB, A0, B0, A1, B1, A2, B2);
        if (!wantB) return v;
        // angular spread to the neighbouring wavelengths (so adjacent wedges tile the fan)
        vec2 dHere = pickSeg(i, false, A0, B0, A1, B1, A2, B2).zw;
        traceSys(o, vec2(1.0, 0.0), lamOf(min(fi + 1.0, NLF - 1.0)), A0, B0, A1, B1, A2, B2, G0, G1, G2, inL);
        vec4 an = pickSeg(i, false, A0, B0, A1, B1, A2, B2);
        traceSys(o, vec2(1.0, 0.0), lamOf(max(fi - 1.0, 0.0)), A0, B0, A1, B1, A2, B2, G0, G1, G2, inL);
        vec4 ap = pickSeg(i, false, A0, B0, A1, B1, A2, B2);
        float k = 0.0, nk = 0.0;
        if (dot(an.zw, an.zw) > 0.5 && dot(an.zw, dHere) > 0.97) { k += length(an.zw - dHere); nk += 1.0; }
        if (dot(ap.zw, ap.zw) > 0.5 && dot(ap.zw, dHere) > 0.97) { k += length(ap.zw - dHere); nk += 1.0; }
        v.w = nk > 0.0 ? min(0.5 * k / nk, 0.03) : 0.01;
        // a sliver of only a few wavelengths split off on its own path draws a hairline — fade it
        float cnt = nk;
        traceSys(o, vec2(1.0, 0.0), lamOf(min(fi + 2.0, NLF - 1.0)), A0, B0, A1, B1, A2, B2, G0, G1, G2, inL);
        vec4 a2 = pickSeg(i, false, A0, B0, A1, B1, A2, B2);
        traceSys(o, vec2(1.0, 0.0), lamOf(max(fi - 2.0, 0.0)), A0, B0, A1, B1, A2, B2, G0, G1, G2, inL);
        vec4 a3 = pickSeg(i, false, A0, B0, A1, B1, A2, B2);
        if (dot(a2.zw, a2.zw) > 0.5 && dot(a2.zw, dHere) > 0.95) cnt += 1.0;
        if (dot(a3.zw, a3.zw) > 0.5 && dot(a3.zw, dHere) > 0.95) cnt += 1.0;
        v.z *= smoothstep(1.5, 3.5, cnt);
        v.z *= smoothstep(0.05, 0.3, dHere.x);     // light thrown back toward the lamp (TIR leaks) lands off-stage as slivers — skip it
        return v;
    }
    if (r < 18.5) {
        vec3 sum = vec3(0.0);
        for (int k = 0; k < NL; k++) sum += specRGB(float(k));
        return vec4(specRGB(fi) / max(sum, vec3(1e-4)), 0.0);
    }
    traceSys(g_src, vec2(1.0, 0.0), lam, A0, B0, A1, B1, A2, B2, G0, G1, G2, inL);
    if (r < 19.5) return G0;
    if (r < 20.5) return G1;
    if (r < 21.5) return G2;
    return vec4(inL, 0.0, 0.0, 0.0);
}

// ═════════════════════════════════════════════════════════════════════════
//  PASS 1 — scene
// ═════════════════════════════════════════════════════════════════════════
vec3  g_ro, g_cf, g_cr, g_cu;
float g_cheap;
float g_pixAng, g_bass, g_mid, g_high, g_beat, g_lamp, g_tBase, g_inLen, g_hwFull, g_hwSub;
// stage culling envelopes: stage 0 = seg0 (middle), 1 = seg1 (middle), 2..4 = seg2 sub-ray 0..2
vec4 g_e0a, g_e0b, g_e1a, g_e1b, g_e2a, g_e2b, g_e3a, g_e3b, g_e4a, g_e4b;     // (o, d) of λ first / last
vec2 g_m0, g_m1, g_m2, g_m3, g_m4;                                             // (max spread, any energy)
vec3 g_bounce;

vec3 toS(vec3 w)  { return vec3(w.x, w.z, w.y - YC); }      // world → sheet space (plane x/y, depth)
vec3 toSd(vec3 w) { return vec3(w.x, w.z, w.y); }

vec4 ray(float i, float r) { return texture2D(rayBuf, vec2((i + 0.5) / NLF, (r + 0.5) / NROW)); }

float sheetHz(float s) { return 0.5 * beamHeight + fanOut * s; }

int  stageJ(int st) { return st < 2 ? 1 : st - 2; }
int  stageI(int st) { return st < 2 ? st : 2; }
float stageRow(int st) { return float(2 * (3 * stageJ(st) + stageI(st))); }
float stageHw(int st) { return st < 2 ? g_hwFull : g_hwSub; }

void loadEnvelope(int st, out vec4 a, out vec4 b, out vec2 m) {
    float ra = stageRow(st);
    a = ray(0.0, ra); b = ray(NLF - 1.0, ra);
    vec4 ba = ray(0.0, ra + 1.0), bb = ray(NLF - 1.0, ra + 1.0), bm = ray(0.5 * NLF, ra + 1.0);
    m = vec2(max(max(ba.w, bb.w), 0.004), max(max(ba.z, bb.z), bm.z));
}
void getEnv(int st, out vec4 a, out vec4 b, out vec2 m) {
    if (st == 0) { a = g_e0a; b = g_e0b; m = g_m0; }
    else if (st == 1) { a = g_e1a; b = g_e1b; m = g_m1; }
    else if (st == 2) { a = g_e2a; b = g_e2b; m = g_m2; }
    else if (st == 3) { a = g_e3a; b = g_e3b; m = g_m3; }
    else { a = g_e4a; b = g_e4b; m = g_m4; }
}
// signed "inside" values of a point w.r.t. the region swept between the first and last wavelength
vec3 envSide(vec2 q, vec4 a, vec4 b, float marg) {
    vec2 mo = 0.5 * (a.xy + b.xy), md = normalize(a.zw + b.zw);
    vec2 mid = mo + md;                                    // a point well inside the fan
    float sa = sign(cross2(a.zw, mid - a.xy) + 1e-6), sb = sign(cross2(b.zw, mid - b.xy) + 1e-6);
    float al = max(dot(q - mo, md), 0.0);
    float mg = marg * (1.0 + al) + 0.02;
    return vec3(sa * cross2(a.zw, q - a.xy) + mg, sb * cross2(b.zw, q - b.xy) + mg, dot(q - mo, md) + mg);
}

// ─── wedge of one wavelength at a point in the sheet ─────────────────────
//   irradiance factor (energy per unit width per unit depth), AA'd
float wedgeAt(vec2 p, float pz, vec4 A, vec4 B, float hw, float foot) {
    vec2 v = p - A.xy;
    float al = dot(v, A.zw);
    float pe = abs(cross2(A.zw, v));
    float k = B.w;
    float w = hw + k * max(al, 0.0);
    float soft = foot + 0.0012 + 1.3 * k * max(al, 0.0);     // neighbouring wavelengths overlap: a continuous spectrum, no bands
    float sl = smoothstep(w + soft, w - soft, pe) * smoothstep(-foot, foot, al) * (1.0 - smoothstep(B.x - foot, B.x + foot, al));
    float hz = sheetHz(B.y + max(al, 0.0));
    float zz = band(pz, -hz, hz, foot + 0.004);
    return sl * zz * (0.035 / max(w + 0.5 * soft, 0.003)) * (HREF / hz) * B.z;
}

void clipLin(float p, float q, inout float lo, inout float hi) {
    if (abs(q) < 1e-7) { if (p > 0.0) { lo = 1.0; hi = 0.0; } return; }
    float t = -p / q;
    if (q > 0.0) hi = min(hi, t); else lo = max(lo, t);
}
// in-scatter of one wavelength wedge along a view-ray segment
float wedgeAlong(vec3 row, vec3 rdw, float tmax, vec4 A, vec4 B, float hw) {
    vec3 ro = toS(row), rd = toSd(rdw);
    vec2 o = ro.xy - A.xy;
    float lo = 0.0, hi = tmax;
    float k = B.w;
    float c0 = cross2(A.zw, o), c1 = cross2(A.zw, rd.xy);
    float a0 = dot(A.zw, o), a1 = dot(A.zw, rd.xy);
    clipLin( c0 - hw - k * a0,  c1 - k * a1, lo, hi);
    clipLin(-c0 - hw - k * a0, -c1 - k * a1, lo, hi);
    clipLin(-a0, -a1, lo, hi);
    clipLin(a0 - B.x, a1, lo, hi);
    clipLin(-row.y, -rdw.y, lo, hi);
    if (hi <= lo) return 0.0;
    // depth: |z| ≤ hz0 + fanOut·(s0 + along), evaluated at 0.8× and 1.2× so the sheet's edge is soft
    float h0 = 0.5 * beamHeight + fanOut * (B.y + a0), h1 = fanOut * a1;
    float L = 0.0, tm = 0.0;
    for (int q = 0; q < 2; q++) {
        float sc = q == 0 ? 0.8 : 1.2;
        float l2 = lo, h2 = hi;
        clipLin( ro.z - h0 * sc,  rd.z - h1 * sc, l2, h2);
        clipLin(-ro.z - h0 * sc, -rd.z - h1 * sc, l2, h2);
        float d = max(h2 - l2, 0.0);
        L += 0.5 * d; tm += 0.5 * (l2 + h2) * 0.5;
    }
    if (L <= 0.0) return 0.0;
    float am = max(a0 + a1 * tm, 0.0);
    return L * (0.035 / max(hw + k * am, 0.003)) * (HREF / sheetHz(B.y + am)) * B.z;
}

// ─── scene geometry ──────────────────────────────────────────────────────
float bodyY0(int e) { float gd = e == 2 ? GDC : GDP; return YC - gd + (e == 1 ? LIFTH * g_lift : 0.0); }
float bodyY1(int e) { float gd = e == 2 ? GDC : GDP; return YC + gd + (e == 1 ? LIFTH * g_lift : 0.0); }
// glass body: its 2D polygon (in the x/z floor plane) standing upright between bodyY0 and bodyY1
float bodyHit3(int e, vec3 ro, vec3 rd, out vec3 N, out float tfar, out vec3 Nfar) {
    float tn = -1e9, tf = 1e9;
    vec3 nn = vec3(0.0), nf = vec3(0.0);
    for (int k = 0; k < 5; k++) {
        if (e < 2 && k > 2) break;
        vec3 f = facePlane(e, k);
        float dn = dot(rd.xz, f.xy), dist = dot(ro.xz, f.xy) - f.z;
        if (abs(dn) < 1e-7) { if (dist > 0.0) return 1e9; continue; }
        float t = -dist / dn;
        if (dn < 0.0) { if (t > tn) { tn = t; nn = vec3(f.x, 0.0, f.y); } }
        else          { if (t < tf) { tf = t; nf = vec3(f.x, 0.0, f.y); } }
    }
    float y0 = bodyY0(e), y1 = bodyY1(e);
    if (abs(rd.y) > 1e-7) {
        float ta = (y0 - ro.y) / rd.y, tb = (y1 - ro.y) / rd.y;
        if (rd.y > 0.0) { if (ta > tn) { tn = ta; nn = vec3(0, -1, 0); } if (tb < tf) { tf = tb; nf = vec3(0, 1, 0); } }
        else            { if (tb > tn) { tn = tb; nn = vec3(0, 1, 0); }  if (ta < tf) { tf = ta; nf = vec3(0, -1, 0); } }
    } else if (ro.y < y0 || ro.y > y1) return 1e9;
    N = nn; Nfar = nf; tfar = tf;
    if (tn > tf || tf < 1e-4) return 1e9;
    return tn > 1e-4 ? tn : 1e9;
}

float cylY(vec3 ro, vec3 rd, vec2 cxz, float r, float y0, float y1, out vec3 N) {
    vec2 oc = ro.xz - cxz;
    float a = dot(rd.xz, rd.xz), b = dot(oc, rd.xz), c = dot(oc, oc) - r * r;
    float h = b * b - a * c;
    if (h < 0.0 || a < 1e-8) return 1e9;
    float t = (-b - sqrt(h)) / a;
    float y = ro.y + rd.y * t;
    if (t < 1e-4 || y < y0 || y > y1) return 1e9;
    N = normalize(vec3(oc.x + rd.x * t, 0.0, oc.y + rd.z * t));
    return t;
}
float boxHit(vec3 ro, vec3 rd, vec3 bmin, vec3 bmax, out vec3 N) {
    vec3 inv = 1.0 / rd;
    vec3 t0 = (bmin - ro) * inv, t1 = (bmax - ro) * inv;
    vec3 tn = min(t0, t1), tf = max(t0, t1);
    float a = max(max(tn.x, tn.y), tn.z), b = min(min(tf.x, tf.y), tf.z);
    if (a > b || a < 1e-4) return 1e9;
    N = a == tn.x ? vec3(-sign(rd.x), 0, 0) : (a == tn.y ? vec3(0, -sign(rd.y), 0) : vec3(0, 0, -sign(rd.z)));
    return a;
}

// kind: 0 void, 1 floor, 2 screen, 3 back wall, 4 metal (rods, lamp), 10+e glass body e
float sceneHit(vec3 ro, vec3 rd, out int kind, out vec3 N) {
    float t = 1e9; kind = 0; N = vec3(0.0);
    if (rd.y < -1e-5) { float tt = -ro.y / rd.y; vec3 P = ro + rd * tt; if (tt > 1e-4 && P.x < XW && P.z < ZB) { t = tt; kind = 1; N = vec3(0, 1, 0); } }
    if (rd.x > 1e-5)  { float tt = (XW - ro.x) / rd.x; vec3 P = ro + rd * tt; if (tt > 1e-4 && tt < t && P.y > 0.0 && P.z < ZB) { t = tt; kind = 2; N = vec3(-1, 0, 0); } }
    if (rd.z > 1e-5)  { float tt = (ZB - ro.z) / rd.z; vec3 P = ro + rd * tt; if (tt > 1e-4 && tt < t && P.y > 0.0 && P.x < XW) { t = tt; kind = 3; N = vec3(0, 0, -1); } }
    for (int e = 0; e < 3; e++) {
        vec3 Nb, Nf; float tf;
        float tb = bodyHit3(e, ro, rd, Nb, tf, Nf);
        if (tb < t) { t = tb; kind = 10 + e; N = Nb; }
    }
    vec3 Nr; float tr;
    // prisms 1 and the crystal stand on posts; prism 2 hangs from a rod that lifts it out of the light
    tr = cylY(ro, rd, g_c0, 0.013, 0.0, bodyY0(0), Nr);
    if (tr < t) { t = tr; kind = 4; N = Nr; }
    tr = cylY(ro, rd, g_c1, 0.008, bodyY1(1), 6.0, Nr);
    if (tr < t) { t = tr; kind = 4; N = Nr; }
    tr = cylY(ro, rd, g_c2, 0.02, 0.0, bodyY0(2), Nr);
    if (tr < t) { t = tr; kind = 4; N = Nr; }
    // the slit lamp on its stand
    tr = boxHit(ro, rd, vec3(g_src.x - 0.34, YC - 0.13, g_src.y - 0.11), vec3(g_src.x - 0.01, YC + 0.13, g_src.y + 0.11), Nr);
    if (tr < t) { t = tr; kind = 4; N = Nr; }
    tr = cylY(ro, rd, vec2(g_src.x - 0.17, g_src.y), 0.022, 0.0, YC - 0.13, Nr);
    if (tr < t) { t = tr; kind = 4; N = Nr; }
    return t;
}

// dark room environment for reflections: a soft overhead strip and a faint kicker
vec3 roomEnv(vec3 d) {
    vec3 c = roomColor.rgb * (0.06 + 0.08 * max(d.y, 0.0)) * roomLight;
    float sb = band(d.x, -0.5, 0.5, 0.01) * band(d.y, 0.72, 0.80, 0.004);
    float kk = band(d.z, -0.05, 0.05, 0.004) * band(d.y, 0.0, 0.5, 0.01) * step(d.x, -0.6);
    return c + vec3(1.0, 0.97, 0.92) * (sb * 1.6 + kk * 0.8) * roomLight;
}

// ─── light at a surface point from every wavelength of every stage ───────
vec3 stageLight(int st, vec3 P, int kind, float foot) {
    vec4 ea, eb; vec2 em;
    getEnv(st, ea, eb, em);
    vec3 acc = vec3(0.0);
    if (em.y < 1e-3) return acc;
    vec3 sd = envSide(P.xz, ea, eb, em.x);
    if (min(min(sd.x, sd.y), sd.z) < 0.0) return acc;
    float ra = stageRow(st), hw = stageHw(st);
    for (int ii = 0; ii < NL; ii++) {
        float i = float(ii);
        vec4 A = ray(i, ra), B = ray(i, ra + 1.0);
        if (B.z < 1e-4) continue;
        float inc = kind == 2 ? abs(A.z) : 0.15;
        acc += ray(i, 18.0).rgb * wedgeAt(P.xz, P.y - YC, A, B, hw, foot) * inc;
    }
    return acc;
}
vec3 lightAt(vec3 P, int kind, float foot) {
    vec3 acc = vec3(0.0);
    for (int st = 0; st < 5; st++) acc += stageLight(st, P, kind, foot);
    // the white beam from the lamp to the first prism
    vec4 A = vec4(g_src, 1.0, 0.0), B = vec4(g_inLen, 0.0, 1.0, 0.0);
    acc += vec3(1.0) * wedgeAt(P.xz, P.y - YC, A, B, g_hwFull, foot) * (kind == 2 ? 1.0 : 0.15);
    return acc * beamColor.rgb * g_lamp * 14.0;     // a surface catches the whole beam; haze scatters a sliver of it
}

vec3 stageVolume(int st, vec3 ro, vec3 rd, float tmax, out float mask) {
    mask = 0.0;
    vec4 ea, eb; vec2 em;
    getEnv(st, ea, eb, em);
    vec3 acc = vec3(0.0);
    if (em.y < 1e-3) return acc;
    // the part of the view ray that can be inside the sheet (|z| below the widest spread)
    float hzMax = sheetHz(14.0);
    float t0 = 0.0, t1 = tmax;
    if (abs(rd.y) > 1e-6) { float ta = (YC - hzMax - ro.y) / rd.y, tb = (YC + hzMax - ro.y) / rd.y; t0 = max(t0, min(ta, tb)); t1 = min(t1, max(ta, tb)); }
    if (t1 <= t0) return acc;
    vec3 sa = envSide((ro + rd * t0).xz, ea, eb, em.x), sb = envSide((ro + rd * t1).xz, ea, eb, em.x);
    if ((sa.x < 0.0 && sb.x < 0.0) || (sa.y < 0.0 && sb.y < 0.0) || (sa.z < 0.0 && sb.z < 0.0)) return acc;
    float ra = stageRow(st), hw = stageHw(st);
    vec3 cs = vec3(0.0);
    for (int ii = 0; ii < NL; ii++) {
        float i = float(ii);
        vec4 A = ray(i, ra), B = ray(i, ra + 1.0);
        if (B.z < 1e-4) continue;
        float l = wedgeAlong(ro, rd, tmax, A, vec4(B.xyz, B.w * 1.15), hw);
        vec3 cc = ray(i, 18.0).rgb;
        acc += cc * l;
        cs += cc * B.z;
        mask += l;
    }
    // the glass eats more violet than red (TIR, Fresnel): re-balance the stage's haze so the
    // dusty beam stays luminous white-to-rainbow instead of going brown
    vec3 bal = clamp(vec3(lum(cs)) / max(cs, vec3(1e-4)), vec3(0.6), vec3(2.2));
    return acc * bal;
}

// glowing chord of the light inside a glass body (three wavelengths so the fan shows inside prism 2)
vec3 glassChords(vec3 ro, vec3 rd, float tmax) {
    vec3 acc = vec3(0.0);
    for (int e = 0; e < 3; e++) {
        for (int c = 0; c < 3; c++) {
            float i = c == 0 ? 2.0 : (c == 1 ? 11.0 : 21.0);
            vec4 G = ray(i, 19.0 + float(e));
            vec2 seg = G.zw - G.xy; float sl = length(seg);
            if (sl < 1e-3) continue;
            vec4 A = vec4(G.xy, seg / sl), B = vec4(sl, 1.0, 1.0, 0.0);
            vec3 col = c == 0 ? vec3(0.35, 0.45, 1.0) : (c == 1 ? vec3(0.4, 1.0, 0.5) : vec3(1.0, 0.35, 0.25));
            acc += col * wedgeAlong(ro, rd, tmax, A, B, g_hwFull * 0.8);
        }
    }
    return acc * 1.3 * glassGlow;
}

vec3 volumeAlong(vec3 ro, vec3 rd, float tmax, out float mask) {
    vec3 acc = vec3(0.0);
    mask = 0.0;
    for (int st = 0; st < 5; st++) { float m; acc += stageVolume(st, ro, rd, tmax, m); mask += m; }
    vec4 A = vec4(g_src, 1.0, 0.0), B = vec4(g_inLen, 0.0, 1.0, 0.0);
    float li = wedgeAlong(ro, rd, tmax, A, B, g_hwFull);
    acc += vec3(1.0) * li;
    mask += li;
    acc += glassChords(ro, rd, tmax);
    return acc * beamColor.rgb * g_lamp;
}

float dustAt(vec3 ro, vec3 rd) {
    float tm = abs(rd.y) > 1e-4 ? (YC - ro.y) / rd.y : 2.0;
    tm = clamp(tm, 0.0, 9.0);
    vec3 p = ro + rd * tm;
    vec3 q = p * 5.0 + vec3(0.10, 0.06, -0.05) * g_t * dustDrift * 5.0;
    float n = vnoise3(q) * 0.55 + vnoise3(q * 2.3 + 7.1) * 0.3 + vnoise3(q * 5.3 - 3.3) * 0.15;
    float d = mix(1.0, 0.30 + 1.4 * n, clamp(dust, 0.0, 1.0));
    return d * (1.0 + 0.25 * g_beat * audioReact);
}

float motes(vec3 ro, vec3 rd, float tmax) {
    float m = 0.0;
    for (int k = 0; k < 3; k++) {
        float fk = float(k);
        float zp = (fk - 1.0) * 0.12;
        float tm = abs(rd.y) > 1e-4 ? (YC + zp * 2.5 - ro.y) / rd.y : -1.0;
        if (tm <= 0.0 || tm > tmax) continue;
        vec3 p = ro + rd * tm;
        vec2 q = p.xz * 60.0 + vec2(g_t * 0.5 * dustDrift, -g_t * 0.2 * dustDrift) * (1.0 + fk * 0.3) + fk * 17.0;
        vec2 cell = floor(q), f = fract(q) - 0.5;
        float h = hash12(cell + fk * 3.1);
        if (h > 0.955) {
            vec2 off = vec2(hash12(cell + 1.7), hash12(cell + 9.3)) - 0.5;
            float d = length(f - off * 0.6);
            float tw = 0.6 + 0.4 * sin(g_t * (2.0 + 4.0 * h) + h * 40.0);
            m += smoothstep(0.10, 0.025, d) * tw * (1.0 + 1.5 * g_high * audioReact);
        }
    }
    return m;
}

vec3 screenAlbedo(vec3 P) {
    float g = vnoise2(P.yz * 45.0) * 0.5 + vnoise2(P.yz * 160.0) * 0.3 + vnoise2(P.yz * 6.0) * 0.2;
    return wallColor.rgb * (0.88 + 0.18 * g);
}

// opaque surface shading, no reflection
vec3 shadeOpaque(vec3 P, int kind, float t, vec3 N, vec3 rd) {
    float foot = (t + g_tBase) * g_pixAng * 1.2;
    if (kind == 4) {
        float F = 0.04 + 0.96 * pow(1.0 - max(dot(-rd, N), 0.0), 5.0);
        return roomEnv(reflect(rd, N)) * (0.3 + F) + g_bounce * 0.04 * (0.5 + 0.5 * N.x) + roomColor.rgb * 0.03;
    }
    vec3 L = g_cheap > 0.5 && kind != 2 ? vec3(0.0) : lightAt(P, kind, foot);
    vec3 alb = kind == 2 ? screenAlbedo(P) : (kind == 1 ? vec3(0.07, 0.065, 0.075) : vec3(0.22, 0.20, 0.24) * (0.85 + 0.3 * vnoise2(P.xy * 30.0)));
    vec3 lalb = kind == 1 ? vec3(0.42) : alb;                 // the floor's dust film catches the spectra
    // room light: a soft pool on the screen, faint elsewhere; bounce from the lit screen
    vec3 amb = roomColor.rgb * 0.55 * roomLight;
    float nearScreen = exp(-max(XW - P.x, 0.0) * 1.1);
    if (kind == 2) amb *= 0.12;                // the screen stays near-black until light lands on it
    if (kind == 3) amb *= 0.35 + 1.2 * exp(-pow(length((P.xy - vec2(-0.6, 1.4)) * vec2(0.35, 0.7)), 2.0));
    vec3 bounce = g_bounce * (kind == 2 ? 0.0 : 0.22 * nearScreen);
    return lalb * L + alb * (amb + bounce);
}

vec3 glassCheap(vec3 rd, vec3 N, float n) {
    return roomEnv(reflect(rd, N)) * fresnelD(abs(dot(rd, N)), 1.0, n) + roomColor.rgb * 0.02;
}

vec3 traceSecondary(vec3 ro, vec3 rd) {
    int kind; vec3 N;
    float t = sceneHit(ro, rd, kind, N);
    vec3 c;
    if (kind == 0) { c = roomEnv(rd); t = 12.0; }
    else if (kind >= 10) {
        int e = kind - 10;
        vec3 Nf; float tf; vec3 Nn;
        bodyHit3(e, ro, rd, Nn, tf, Nf);
        c = glassCheap(rd, N, 1.55);
        int k2; vec3 N2;
        vec3 q = ro + rd * (tf + 1e-3);
        float t2 = sceneHit(q, rd, k2, N2);
        if (k2 > 0 && k2 < 10) c += shadeOpaque(q + rd * t2, k2, tf + t2, N2, rd) * 0.85;
        t = tf + t2;
    }
    else c = shadeOpaque(ro + rd * t, kind, t, N, rd);
    float msk;
    c += volumeAlong(ro, rd, min(t, 12.0), msk) * 3.2 * haze * dustAt(ro, rd);
    return c;
}

// a hot flare where the beam crosses a glass face
vec3 flareAt(vec3 ro, vec3 rd, float tmax, vec2 p2, vec3 col, float size) {
    vec3 P = vec3(p2.x, YC, p2.y);
    vec3 toP = P - ro;
    float tp = dot(toP, rd);
    if (tp <= 0.0 || tp > tmax + 0.4) return vec3(0.0);
    float d = length(toP - rd * tp) / (tp * g_pixAng);
    float ang = atan(dot(toP - rd * tp, g_cu), dot(toP - rd * tp, g_cr));
    float star = pow(abs(cos(ang * 2.0 + 0.5)), 60.0) * exp(-d * 0.05) * 0.18;
    return col * (exp(-d * 0.45) * 3.0 + exp(-d * 0.08) * 0.35 + star) * size;
}

vec3 renderScene(vec2 uv, out float cover) {
    vec3 rd = normalize(g_cf * 1.5 + g_cr * uv.x + g_cu * uv.y);
    vec3 ro = g_ro;
    int kind; vec3 N;
    float t = sceneHit(ro, rd, kind, N);
    vec3 col;
    cover = kind == 0 ? 0.0 : 1.0;
    float tVol = min(t, 12.0);

    if (kind == 0) {
        col = roomEnv(rd);
    } else if (kind == 1) {
        vec3 P = ro + rd * t;
        col = shadeOpaque(P, 1, t, N, rd);
        // spectral bounce: the lit screen glows colour back onto the floor in front of it
        float nearS = exp(-max(XW - P.x, 0.0) * 1.8);
        if (nearS > 0.02) col += lightAt(vec3(XW - 0.01, 0.35, P.z), 2, 0.06) * vec3(0.07, 0.065, 0.075) * 2.2 * nearS;
        // caustic glints: each glass body focuses a little light onto the floor just past it
        for (int e = 0; e < 3; e++) {
            vec4 G = ray(11.0, 19.0 + float(e));
            vec2 ch = G.zw - G.xy;
            if (dot(ch, ch) < 1e-6) continue;
            vec2 c0 = G.zw + normalize(ch) * 0.10;
            vec2 dd = P.xz - c0;
            float r2 = dot(dd * vec2(1.0, 2.2), dd * vec2(1.0, 2.2));
            float core = exp(-r2 * 2600.0);
            float ring = exp(-r2 * 380.0);
            vec3 hue = 0.5 + 0.5 * cos(TAU * (sqrt(r2) * 9.0 + vec3(0.0, 0.33, 0.67)));
            col += (vec3(1.0, 0.97, 0.9) * core * 2.4 + hue * ring * 0.35) * g_lamp * flares;
        }
        // polished concrete: a blurred-ish mirror (slight normal jitter by distance-filtered noise)
        float F = 0.04 + 0.96 * pow(1.0 - max(dot(-rd, N), 0.0), 5.0);
        g_tBase = t; g_cheap = 1.0;
        col += traceSecondary(P + N * 1e-3, reflect(rd, N)) * mix(0.15, 1.0, F) * floorGloss;
        g_tBase = 0.0; g_cheap = 0.0;
    } else if (kind < 10) {
        col = shadeOpaque(ro + rd * t, kind, t, N, rd);
    } else {
        int e = kind - 10;
        vec3 P = ro + rd * t;
        vec3 Nf0; float tf0; vec3 Nn0;
        bodyHit3(e, ro, rd, Nn0, tf0, Nf0);
        float n = e == 2 ? crystalIndex : glassIndex;
        float F = fresnelD(abs(dot(rd, N)), 1.0, n);
        g_tBase = t;
        g_cheap = 1.0;
        vec3 refl = roomEnv(reflect(rd, N)) + traceSecondary(P + N * 1e-3, reflect(rd, N)) * 0.35;
        g_cheap = 0.0;
        // refraction through the body, two indices (red / blue) → real chromatic fringes on what's behind
        vec3 trans = vec3(0.0);
        for (int c = 0; c < 2; c++) {
            float nc = n + (float(c) - 0.5) * 0.06 * dispersion;
            vec3 di = refract(rd, N, 1.0 / nc);
            vec3 Nb, Nfb; float tfb;
            bodyHit3(e, P - di * 1e-3, di, Nb, tfb, Nfb);
            vec3 Q = P - di * 1e-3 + di * tfb;
            vec3 dout = refract(di, -Nfb, nc);
            if (dot(dout, dout) < 0.5) dout = reflect(di, Nfb);
            vec3 s = traceSecondary(Q + dout * 2e-3, dout) * exp(-tfb * vec3(0.9, 0.16, 0.65));   // BK7-ish green body tint, deeper through thick glass
            if (c == 0) trans += s * vec3(1.0, 0.5, 0.0); else trans += s * vec3(0.0, 0.5, 1.0);
        }
        g_tBase = 0.0;
        col = refl * F + trans * (1.0 - F) * vec3(0.94, 0.97, 1.0);
        // inside the polished body the softbox bounces around: a faint lit interior + face sheen
        float facing = abs(dot(rd, N));
        col += roomEnv(reflect(refract(rd, N, 1.0 / n), Nf0)) * 0.35 * (1.0 - facing) + roomColor.rgb * 0.06 * roomLight;
        // polished edges: the polygon outline of both caps + the long edges
        float foot = t * g_pixAng * 1.5;
        float edgeL = 0.0;
        for (int cp = 0; cp < 2; cp++) {
            float yc = cp == 0 ? bodyY1(e) : bodyY0(e);
            float tc = (yc - ro.y) / rd.y;
            vec2 q = (ro + rd * tc).xz;
            float inside = -1e9, de = 1e9;
            for (int k = 0; k < 5; k++) {
                if (e < 2 && k > 2) break;
                vec3 f = facePlane(e, k);
                float dd = dot(q, f.xy) - f.z;
                inside = max(inside, dd); de = min(de, abs(dd));
            }
            float fc = abs(tc) * g_pixAng * 1.2;
            float o = (1.0 - smoothstep(0.5 * fc, 1.5 * fc + 0.001, de)) * (1.0 - smoothstep(0.0, fc * 2.0 + 0.002, inside));
            edgeL = max(edgeL, o * (cp == 0 ? 1.0 : 0.5));
        }
        {
            float d1 = 1e9, d2 = 1e9;
            for (int k = 0; k < 5; k++) {
                if (e < 2 && k > 2) break;
                vec3 f = facePlane(e, k);
                float dd = abs(dot(P.xz, f.xy) - f.z);
                if (dd < d1) { d2 = d1; d1 = dd; } else if (dd < d2) d2 = dd;
            }
            edgeL = max(edgeL, (1.0 - step(0.5, abs(N.y))) * (1.0 - smoothstep(0.5 * foot, 1.5 * foot + 0.001, d2)) * 0.7);
        }
        // hairline only: edges catch the overhead strip faintly, and blaze where the beam lights them
        float spec = pow(max(dot(reflect(rd, N), vec3(0.0, 1.0, 0.0)), 0.0), 8.0);
        col += edgeL * (vec3(0.9, 0.95, 1.0) * (0.06 + 0.25 * spec) * roomLight + 0.35 * lightAt(vec3(P.x, YC, P.z), 2, 0.02));
    }

    float msk;
    vec3 v = volumeAlong(ro, rd, tVol, msk);
    col += v * 3.2 * haze * dustAt(ro, rd);
    float mo = motes(ro, rd, tVol);
    vec3 mcol = v / max(max(v.r, max(v.g, v.b)), 1e-4);
    col += mo * min(msk * 3.0, 1.0) * mix(vec3(1.0), mcol, 0.75) * 1.1 * dust * g_lamp;
    // the lamp's aperture + face flares where the light enters and leaves glass
    col += flareAt(ro, rd, t, g_src, beamColor.rgb * g_lamp, 0.7);
    for (int e = 0; e < 3; e++) {
        vec4 G = ray(11.0, 19.0 + float(e));
        if (dot(G.zw - G.xy, G.zw - G.xy) < 1e-6) continue;
        col += flareAt(ro, rd, t, G.xy, beamColor.rgb * g_lamp, 0.35 * flares);
        col += flareAt(ro, rd, t, G.zw, beamColor.rgb * g_lamp, 0.45 * flares);
    }
    return col;
}

// ═════════════════════════════════════════════════════════════════════════
//  PASSES 2–4 — bloom + finish
// ═════════════════════════════════════════════════════════════════════════
vec3 sceneTap(vec2 uv) { return texture2D(sceneBuf, clamp(uv, 0.0, 1.0)).rgb; }

vec3 bloomGather(vec2 uv, vec2 texel, float radius, float thr, bool fromScene) {
    vec3 acc = vec3(0.0); float ws = 0.0;
    for (int k = 0; k < 17; k++) {
        float fk = float(k);
        vec2 o = k == 0 ? vec2(0.0) : vec2(cos(fk * 2.39996), sin(fk * 2.39996)) * sqrt(fk / 16.0);
        vec2 u = uv + o * texel * radius;
        vec3 sv = fromScene ? texture2D(sceneBuf, clamp(u, 0.0, 1.0)).rgb : texture2D(bloomA, clamp(u, 0.0, 1.0)).rgb;
        float w = 1.0 - 0.55 * length(o);
        acc += max(sv - thr, 0.0) * w; ws += w;
    }
    return acc / ws;
}

vec3 finishPass(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;
    vec2 px = 1.0 / RENDERSIZE.xy;
    vec3 c = sceneTap(uv);
    vec3 bl = texture2D(bloomA, uv).rgb * 0.9 + texture2D(bloomB, uv).rgb * 0.75;
    vec3 st = vec3(0.0);
    for (int k = -8; k <= 8; k++) {
        float fk = float(k);
        vec3 s = sceneTap(uv + vec2(fk * px.x * 14.0, 0.0));
        st += max(s - 1.6, 0.0) * exp(-abs(fk) * 0.28);
    }
    st *= 0.06 * vec3(0.75, 0.85, 1.15);
    c += bl * bloom + st * streak;
    c *= exposure;
    c = aces(c);
    float l = lum(c);
    c = mix(vec3(l), c, saturation);
    c = clamp(c, 0.0, 1.0);
    c = mix(c, c * c * (3.0 - 2.0 * c), 0.55);
    l = dot(c, vec3(0.299, 0.587, 0.114));
    c = clamp(mix(vec3(l), c, 1.22), 0.0, 1.0);
    vec2 q = uv - 0.5;
    c *= 1.0 - 0.12 * dot(q, q) * 2.0;
    c += (hash12(fragCoord + fract(g_t * 13.1) * 100.0) - 0.5) * 0.035 * grain;
    return clamp(c, 0.0, 1.0);
}

// camera keys: the whole installation, the crystal shattering the beam, the first prism
void setupCamera() {
    // keyed to the optics cycle: macro on the crystal while it shatters white light,
    // back out wide while the big fan floods the floor, macro on the prisms as recombination snaps back
    float tc = g_t * speed;
    float u = fract(tc / 64.0 + 0.02);
    float wC = smoothstep(0.08, 0.16, u) * (1.0 - smoothstep(0.30, 0.40, u));
    float wP = smoothstep(0.72, 0.79, u) * (1.0 - smoothstep(0.90, 0.98, u));
    float wW = 1.0 - wC - wP;
    float ts = g_t * orbitSpeed;
    vec3 roW = vec3(-3.4, 2.3, -1.2) + vec3(0.3 * sin(ts * 0.05), 0.12 * sin(ts * 0.07 + 1.0), 0.25 * sin(ts * 0.04 + 2.0));
    vec3 taW = vec3(1.8, 0.95, 0.9);
    vec3 roC = vec3(-0.6, 1.75, 0.15) + vec3(0.12 * sin(ts * 0.09), 0.05 * sin(ts * 0.11), 0.1 * sin(ts * 0.07));
    vec3 taC = vec3(3.0, 0.95, 0.95);
    vec3 roP = vec3(-3.3, 1.75, 0.35) + vec3(0.1 * sin(ts * 0.1), 0.04 * sin(ts * 0.13), 0.08 * sin(ts * 0.08));
    vec3 taP = vec3(0.2, 1.0, 1.1);
    g_ro = roW * wW + roC * wC + roP * wP;
    vec3 ta = taW * wW + taC * wC + taP * wP;
    g_cf = normalize(ta - g_ro);
    g_cr = normalize(cross(vec3(0, 1, 0), g_cf));
    g_cu = cross(g_cf, g_cr);
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_tBase = 0.0; g_cheap = 0.0;
    setupOptics();

    if (PASSINDEX == 0) {
        // hosts that ignore the pass size render this table at full canvas size: only texels at a
        // cell centre are ever sampled, so everything else exits early
        vec2 uv0 = fragCoord / RENDERSIZE.xy;
        vec2 cc = (floor(uv0 * vec2(NLF, NROW)) + 0.5) / vec2(NLF, NROW) * RENDERSIZE.xy;
        if (any(greaterThan(abs(fragCoord - cc), vec2(1.5)))) { gl_FragColor = vec4(0.0); return; }
        gl_FragColor = rayPass(uv0);
        return;
    }
    if (PASSINDEX == 1) {
        g_hwFull = 0.5 * beamWidth;
        g_hwSub = beamWidth / 4.0;      // sub-rays overlap a little: no hairline slivers
        g_inLen = ray(11.0, 22.0).x;
        loadEnvelope(0, g_e0a, g_e0b, g_m0);
        loadEnvelope(1, g_e1a, g_e1b, g_m1);
        loadEnvelope(2, g_e2a, g_e2b, g_m2);
        loadEnvelope(3, g_e3a, g_e3b, g_m3);
        loadEnvelope(4, g_e4a, g_e4b, g_m4);
        g_lamp = 1.6 * beamIntensity * (1.0 + 0.55 * audioReact * g_bass + 0.2 * audioReact * clamp(audioLevel, 0.0, 1.0));
        // light reaching the room: whatever leaves the last body (spectra average to white)
        float eFinal = max(g_m2.y, 0.0) + max(g_m3.y, 0.0) + max(g_m4.y, 0.0);
        eFinal = mix(eFinal, 3.0 * g_m0.y, g_lift);
        g_bounce = beamColor.rgb * g_lamp * 0.10 * eFinal;
        setupCamera();
        g_pixAng = 1.0 / (1.5 * RENDERSIZE.y);
        vec2 uv = (fragCoord - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
        float cover;
        vec3 c = renderScene(uv, cover);
        gl_FragColor = vec4(c, cover);
        return;
    }
    if (PASSINDEX == 2) { vec2 u = fragCoord / RENDERSIZE.xy; gl_FragColor = vec4(bloomGather(u, 1.0 / RENDERSIZE.xy, 2.2, 0.6, true), 1.0); return; }
    if (PASSINDEX == 3) { vec2 u = fragCoord / RENDERSIZE.xy; gl_FragColor = vec4(bloomGather(u, 1.0 / RENDERSIZE.xy, 2.6, 0.0, false), 1.0); return; }
    vec3 c = finishPass(fragCoord);
    float a = 1.0;
    if (transparentBg) { a = clamp(texture2D(sceneBuf, fragCoord / RENDERSIZE.xy).a + lum(c), 0.0, 1.0); }
    gl_FragColor = vec4(c, a);
}
