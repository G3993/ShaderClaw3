/*{
  "CATEGORIES": ["Generator", "Symmetry", "Audio Reactive"],
  "DESCRIPTION": "Mirror Fold 3D — N-fold kaleidoscope with curated palettes (Chartres, Kusama, Eliasson, Bauhaus-Klee, Pollock Drip). Fake-3D radial perspective foreshortens the wedge toward a vanishing pole, with a slow camera-orbit parallax. TRUE mirror at the wedge boundary (not rotation). Linear HDR; bloom-friendly peaks 1.4-2.0.",
  "INPUTS": [
    { "NAME": "palette",      "LABEL": "Palette",      "TYPE": "long",  "DEFAULT": 0,
      "VALUES": [0,1,2,3,4], "LABELS": ["Chartres Cathedral","Kusama Polka","Eliasson Sun","Bauhaus Klee","Pollock Drip B&G"] },
    { "NAME": "foldIndex",    "LABEL": "Folds (N)",    "TYPE": "long",  "DEFAULT": 2,
      "VALUES": [0,1,2,3,4], "LABELS": ["3","5","6","8","12"] },
    { "NAME": "rotateBase",   "LABEL": "Rotate Speed", "TYPE": "float", "MIN": 0.0,  "MAX": 0.30, "DEFAULT": 0.07 },
    { "NAME": "centerRadius", "LABEL": "Center Pulse", "TYPE": "float", "MIN": 0.0,  "MAX": 0.6,  "DEFAULT": 0.22 },
    { "NAME": "seamGlint",    "LABEL": "Seam Glint",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.85 },
    { "NAME": "leadWeight",   "LABEL": "Lead / Edge",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "depth3D",      "LABEL": "3D Depth",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.85 },
    { "NAME": "orbitSpeed",   "LABEL": "Camera Orbit", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "exposure",     "LABEL": "Exposure",     "TYPE": "float", "MIN": 0.4,  "MAX": 2.0,  "DEFAULT": 1.05 },
    { "NAME": "audioReact",   "LABEL": "Audio React",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  MIRROR FOLD 3D
//  Curated palette enum + fake-3D perspective foreshortening inside the
//  wedge. The wedge content is squeezed toward a vanishing pole with a
//  1/length(uv) inverse-radial mapping; a slow camera-orbit adds parallax
//  to the angular axis so the kaleidoscope feels like a tunnel of glass
//  rather than a flat roulette wheel.
//  TRUE N-fold mirror (paper-fold reflection) preserved.
// ════════════════════════════════════════════════════════════════════════

// ─── hash / utility ───────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float softBand(float x, float w) {
    return smoothstep(w, 0.0, abs(x));
}

// ─── kaleidoscope: TRUE N-fold mirror reflection ──────────────────────
vec2 kaleido(vec2 p, float N) {
    float r = length(p);
    if (r < 1e-6) return vec2(r, 0.0);
    float a = atan(p.y, p.x);
    float wedge = 3.14159265 / N;
    float two   = 2.0 * wedge;
    float ap = a - two * floor((a + wedge) / two);
    ap = abs(ap);
    return vec2(r, ap);
}

// ─── PALETTES ─────────────────────────────────────────────────────────
// CHARTRES CATHEDRAL — cobalt, ruby, emerald, amber, parchment
vec3 cathedralPane(float k) {
    int  i = int(mod(k, 5.0));
    if (i == 0) return vec3(0.05, 0.18, 0.62);
    if (i == 1) return vec3(0.62, 0.06, 0.12);
    if (i == 2) return vec3(0.06, 0.42, 0.22);
    if (i == 3) return vec3(0.92, 0.62, 0.10);
    return                vec3(0.78, 0.74, 0.62);
}

// BAUHAUS / KLEE — vermilion, ultramarine, cadmium yellow, bone, ink
vec3 bauhausPane(float k) {
    int i = int(mod(k, 5.0));
    if (i == 0) return vec3(0.92, 0.18, 0.12);
    if (i == 1) return vec3(0.10, 0.22, 0.70);
    if (i == 2) return vec3(0.96, 0.82, 0.18);
    if (i == 3) return vec3(0.94, 0.92, 0.86);
    return                vec3(0.06, 0.06, 0.08);
}

// POLLOCK DRIP — black, bone, deep gold, copper, ivory
vec3 pollockPane(float k) {
    int i = int(mod(k, 5.0));
    if (i == 0) return vec3(0.04, 0.04, 0.05);   // ink black
    if (i == 1) return vec3(0.92, 0.88, 0.78);   // bone
    if (i == 2) return vec3(0.78, 0.55, 0.10);   // deep gold
    if (i == 3) return vec3(0.55, 0.32, 0.08);   // copper umber
    return                vec3(0.98, 0.96, 0.92);   // ivory
}

// ─── INTERIOR COMPOSITIONS ────────────────────────────────────────────
//  All take wedge coords (r, a) — already perspective-warped before call.

vec3 paletteCathedral(vec2 ra, float t, float lead, float bass) {
    float r = ra.x;
    float a = ra.y;
    float ringW = 0.18 + 0.04 * bass;
    float ringIdx = floor(r / ringW);
    float angIdx = floor(a / (1.5708 / 4.0));
    float key = ringIdx * 2.0 + angIdx + floor(t * 0.05);
    vec3  pane = cathedralPane(key);
    float ringEdge = softBand(fract(r / ringW) - 0.0, 0.012)
                   + softBand(fract(r / ringW) - 1.0, 0.012);
    float angEdge  = softBand(fract(a / (1.5708 / 4.0)) - 0.0, 0.012)
                   + softBand(fract(a / (1.5708 / 4.0)) - 1.0, 0.012);
    float leadMask = clamp(ringEdge + angEdge, 0.0, 1.0);
    float lumWobble = 0.85 + 0.25 * sin(r * 8.0 - t * 0.6 + key);
    pane *= lumWobble;
    // HDR pop on amber panes — peaks ~1.6 for bloom
    if (int(mod(key, 5.0)) == 3) pane *= 1.65;
    vec3 leadCol = vec3(0.012, 0.010, 0.008);
    return mix(pane, leadCol, leadMask * lead);
}

vec3 paletteKusama(vec2 ra, float t, float bass) {
    vec2 q = vec2(ra.x * cos(ra.y), ra.x * sin(ra.y));
    float pitch = 0.16 + 0.025 * bass;
    vec2  cell  = floor(q / pitch);
    vec2  f     = fract(q / pitch) - 0.5;
    float rseed = hash21(cell + 31.7);
    float dotR  = (0.30 + 0.10 * rseed) * pitch;
    float d     = length(f * pitch);
    float dotMask = smoothstep(dotR, dotR - 0.012, d);
    vec3 ground = vec3(0.97, 0.94, 0.88);
    vec3 dotc   = vec3(1.55, 0.14, 0.18);   // HDR red — bloom peak ~1.55
    vec2  cell2 = floor(q / (pitch * 0.5));
    vec2  f2    = fract(q / (pitch * 0.5)) - 0.5;
    float keep2 = step(0.7, hash21(cell2 + 17.3));
    float d2    = length(f2 * pitch * 0.5);
    float dot2  = smoothstep(0.10 * pitch, 0.08 * pitch, d2) * keep2;
    vec3 col = mix(ground, dotc, dotMask);
    col = mix(col, dotc, dot2 * 0.85);
    return col;
}

vec3 paletteEliasson(vec2 ra, float t, float bass) {
    float r = ra.x;
    float sunR = 0.18 + 0.05 * bass + 0.015 * sin(t * 0.3);
    float disc = smoothstep(sunR + 0.02, sunR - 0.02, r);
    float halo = exp(-r * 1.7);
    vec3 core   = vec3(2.00, 1.55, 0.55);   // HDR core peak ~2.0
    vec3 amber  = vec3(0.95, 0.55, 0.12);
    vec3 deep   = vec3(0.18, 0.06, 0.02);
    vec3 col    = mix(deep, amber, halo);
    col         = mix(col,  core,  disc);
    float dust  = 0.04 * sin(ra.y * 9.0 + t * 0.4) * exp(-r * 0.8);
    col += vec3(0.05, 0.03, 0.01) * dust;
    return col;
}

vec3 paletteBauhaus(vec2 ra, float t, float lead, float bass) {
    vec2  q  = vec2(ra.x * cos(ra.y), ra.x * sin(ra.y));
    float bx = max(abs(q.x), abs(q.y));
    float ringW = 0.10 + 0.02 * bass;
    float idx   = floor(bx / ringW + floor(t * 0.04));
    vec3  pane  = bauhausPane(idx);
    // Klee-ish luminous lift on cadmium yellow tile
    if (int(mod(idx, 5.0)) == 2) pane *= 1.55;
    float seam  = softBand(fract(bx / ringW) - 0.0, 0.02);
    return mix(pane, vec3(0.02), seam * lead * 0.7);
}

//  POLLOCK DRIP — "black & gold" period (Number 8, 1949 / Autumn Rhythm).
//  Layered drips: long whippy strands + spatter dots. We compose using
//  two stretched fbm-ish sin lattices over the wedge planar form so the
//  drips reflect cleanly across the seam.
vec3 palettePollock(vec2 ra, float t, float bass) {
    vec2  q  = vec2(ra.x * cos(ra.y), ra.x * sin(ra.y));
    // Drip-strand 1 — long horizontal-ish whips of black ink
    float s1 = sin(q.x * 14.0 + sin(q.y * 9.0 + t * 0.2) * 1.6) ;
    float strand1 = smoothstep(0.86, 0.96, abs(s1));
    // Drip-strand 2 — diagonal gold whips
    vec2 qr = mat2(0.866, -0.5, 0.5, 0.866) * q;
    float s2 = sin(qr.x * 18.0 + sin(qr.y * 11.0 - t * 0.15) * 2.0);
    float strand2 = smoothstep(0.90, 0.98, abs(s2));
    // Spatter dots — irregular
    vec2 cell = floor(q * 22.0);
    vec2 f    = fract(q * 22.0) - 0.5;
    float h   = hash21(cell);
    float keep = step(0.86, h);
    float spatR = 0.18 + 0.22 * hash21(cell + 7.1);
    float spat = smoothstep(spatR, spatR - 0.04, length(f)) * keep;

    vec3 ivory  = vec3(0.96, 0.93, 0.86);          // canvas
    vec3 gold   = vec3(1.40, 0.95, 0.30);          // HDR gold peak ~1.4
    vec3 ink    = vec3(0.020, 0.018, 0.024);
    vec3 copper = vec3(0.62, 0.32, 0.08);

    // Subtle warm wash — bass swells the canvas tone
    vec3 wash = mix(ivory, ivory * vec3(1.04, 1.00, 0.92), 0.5 + 0.5 * bass);
    vec3 col  = wash;
    col = mix(col, ink,    strand1);
    col = mix(col, gold,   strand2);
    col = mix(col, copper, spat * 0.75);
    // Bright gold spatter highlights
    col = mix(col, gold,   spat * step(0.94, h));
    return col;
}

// ─── 3D-FEEL PERSPECTIVE WARP ─────────────────────────────────────────
//  Squeezes the wedge content toward a vanishing pole at r=0 so the
//  composition reads as a tunnel/dome rather than a flat disc.
//  - Radial inverse: rWarped = r / (1 + k/r) ≈ moves content nearer
//    the centre as the wedge gets wider, exactly as perspective does.
//  - Angular parallax: a slow camera-orbit shifts the angular axis,
//    different by radius (closer rings shift more) — fake parallax.
vec2 perspective3D(vec2 ra, float depth, float orbitT) {
    float r = ra.x;
    float a = ra.y;
    // Inverse-radial foreshorten: pulls content toward centre, then
    // rescaled so r=1 stays near r=1 (so frame fill is preserved).
    float k = 0.35 * depth;
    float rW = r / (1.0 + k / max(r, 0.04));
    rW = mix(r, rW, clamp(depth, 0.0, 1.0));
    // Parallax: angular shift varies inversely with r — closer rings
    // (small r) shift more than far rings, like a dolly orbit.
    float par = 0.18 * depth * sin(orbitT) / (0.25 + r * 1.4);
    float aW  = a + par;
    // Keep aW non-negative (wedge space) by abs-fold if it crosses 0
    aW = abs(aW);
    return vec2(rW, aW);
}

// ─── MAIN ─────────────────────────────────────────────────────────────
void main() {
    vec2  uv  = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    vec2 p = uv - 0.5;
    p.x *= aspect;

    float ar   = clamp(audioReact, 0.0, 2.0);
    float bass = smoothstep(0.0, 0.45, audioBass) * ar;
    float mid  = smoothstep(0.0, 0.45, audioMid)  * ar;
    float high = smoothstep(0.0, 0.45, audioHigh) * ar;

    float t = TIME;

    // N-fold count
    float Nset[5];
    Nset[0] = 3.0; Nset[1] = 5.0; Nset[2] = 6.0; Nset[3] = 8.0; Nset[4] = 12.0;
    int   ni = int(clamp(float(foldIndex), 0.0, 4.0));
    float N  = Nset[ni];

    // Slow rotation
    float rot = (rotateBase + 0.05 * mid) * t * 0.6;
    float cs  = cos(rot), sn = sin(rot);
    p = mat2(cs, -sn, sn, cs) * p;

    // Bass-driven gentle zoom + idle breathing
    float idle  = 0.04 * sin(t * 0.18);
    float pulse = centerRadius * (0.10 * bass + idle);
    float zoom  = 1.0 - pulse;
    p *= zoom;

    // Subtle parallax shift on the planar coords too — camera-dolly feel
    float orbitT = t * orbitSpeed;
    p += depth3D * 0.025 * vec2(sin(orbitT), cos(orbitT * 0.83));

    // Project into wedge coordinates (TRUE mirror at boundary)
    vec2 ra = kaleido(p, N);

    // Apply 3D-feel perspective foreshortening to the wedge content
    ra = perspective3D(ra, depth3D, orbitT);

    // ── Palette dispatch ──────────────────────────────────────────
    int   pal = int(clamp(float(palette), 0.0, 4.0));
    vec3  col;
    if      (pal == 0) col = paletteCathedral(ra, t, leadWeight, bass);
    else if (pal == 1) col = paletteKusama   (ra, t, bass);
    else if (pal == 2) col = paletteEliasson (ra, t, bass);
    else if (pal == 3) col = paletteBauhaus  (ra, t, leadWeight, bass);
    else               col = palettePollock  (ra, t, bass);

    // ── Depth shading — far rings darker, near rings brighter ────
    //  Reads as a lit tunnel rather than a flat wheel.
    {
        float r = ra.x;
        float depthShade = mix(1.0,
                               mix(0.55, 1.25, smoothstep(0.9, 0.0, r)),
                               clamp(depth3D, 0.0, 1.0));
        col *= depthShade;
        // Rim-light at the vanishing pole — a soft HDR highlight
        float rim = exp(-r * 9.0) * (0.4 + 0.6 * bass);
        col += vec3(1.20, 1.05, 0.78) * rim * 0.30 * depth3D;
    }

    // ── SEAM GLINT — light catching the mirror edges ───────────────
    {
        float wedge = 3.14159265 / N;
        float a = atan(p.y, p.x);
        float two = 2.0 * wedge;
        float ap  = a - two * floor((a + wedge) / two);
        float dEdge = wedge - abs(ap);
        float r = length(p);
        float seamLine = smoothstep(0.012, 0.0, dEdge) * smoothstep(0.55, 0.05, r);
        float seed = hash11(floor(r * 28.0) * 7.13 + floor(a * 9.0));
        float spark = step(0.78, seed) * smoothstep(0.018, 0.0, dEdge);
        float glint = (seamLine * (0.15 + 0.85 * high)
                     + spark    * (0.10 + 0.90 * high));
        col += vec3(1.20, 1.10, 0.85) * glint * seamGlint;
    }

    // ── Centre highlight — focal anchor at the mirror pole ─────────
    {
        float r = length(p);
        float core = exp(-r * 14.0) * (0.6 + 0.4 * bass);
        col += vec3(1.10, 0.95, 0.70) * core * 0.35;
    }

    // ── Vignette ──────────────────────────────────────────────────
    vec2 ndc = (uv * 2.0 - 1.0);
    col *= 1.0 - 0.18 * dot(ndc * 0.7, ndc * 0.7);

    // Exposure (linear HDR — host applies ACES)
    col *= exposure;

    gl_FragColor = vec4(col, 1.0);
}
