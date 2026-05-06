/*{
  "CATEGORIES": ["Generator", "Cymatics", "Audio Reactive"],
  "DESCRIPTION": "Chladni Figures — sand on a vibrating plate (Ernst Chladni, 1787). The classical nodal-line equation Z = sin(nπx)sin(mπy) − sin(mπx)sin(nπy) is solved per-pixel; sand collects along the zero-crossings. Twelve curated (n,m) mode pairs morph every 6–10 s, with bass triggering an immediate jump. Mid drives sand brightness (louder bow = more sand thrown), treble scatters individual grains. Three moods: charcoal plate, water cymatics, iron filings + dipole. Output linear HDR.",
  "INPUTS": [
    { "NAME": "mood",          "LABEL": "Mood",            "TYPE": "long",  "DEFAULT": 0,
      "VALUES": [0, 1, 2], "LABELS": ["Sand on Plate", "Water Cymatics", "Iron Filings + Magnet"] },
    { "NAME": "morphPeriod",   "LABEL": "Mode Period (s)", "TYPE": "float", "MIN": 4.0,  "MAX": 14.0, "DEFAULT": 8.0 },
    { "NAME": "lineWidth",     "LABEL": "Sand Line Width", "TYPE": "float", "MIN": 0.4,  "MAX": 3.5,  "DEFAULT": 1.4 },
    { "NAME": "settle",        "LABEL": "Settle vs Agitate","TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5 },
    { "NAME": "grainDensity",  "LABEL": "Grain Density",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "warmth",        "LABEL": "Warmth",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.7 },
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  Chladni Figures — after Ernst Chladni's 1787 plate experiments.
//  A bow drawn across a sand-covered metal plate induces standing waves;
//  the sand migrates off the antinodes and crystallizes along the nodal
//  curves. We compute the canonical square-plate solution
//      Z(x,y) = sin(n π x) sin(m π y) − sin(m π x) sin(n π y)
//  and render |Z| ≈ 0 as anti-aliased sand. Twelve curated (n,m) pairs
//  morph over time; bass forces an immediate mode change. Three moods.
// ════════════════════════════════════════════════════════════════════════

#define PI 3.14159265359

// ─── hashes / noise ───────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash31(vec3 p)  { return fract(sin(dot(p, vec3(13.7, 71.3, 41.9))) * 51731.7); }

// 12 curated (n,m) pairs — selected for visual punch. No (1,1) trivials.
// Mix of low/high orders, near-square and elongated, and a couple of
// asymmetric high-contrast favorites that always read well.
vec2 modePair(int i) {
    if (i ==  0) return vec2( 3.0,  5.0);
    if (i ==  1) return vec2( 2.0,  7.0);
    if (i ==  2) return vec2( 4.0,  6.0);
    if (i ==  3) return vec2( 5.0,  7.0);
    if (i ==  4) return vec2( 3.0,  8.0);
    if (i ==  5) return vec2( 6.0,  9.0);
    if (i ==  6) return vec2( 4.0, 11.0);
    if (i ==  7) return vec2( 7.0, 10.0);
    if (i ==  8) return vec2( 5.0, 12.0);
    if (i ==  9) return vec2( 8.0, 11.0);
    if (i == 10) return vec2( 2.0,  9.0);
    return            vec2( 6.0, 13.0);
}

// Chladni nodal field for a square plate.
float chladni(vec2 p, vec2 nm) {
    float a = sin(nm.x * PI * p.x) * sin(nm.y * PI * p.y);
    float b = sin(nm.y * PI * p.x) * sin(nm.x * PI * p.y);
    return a - b;
}

// Smooth per-pixel pseudo-derivative magnitude (analytic-ish).
// fwidth(Z) gives us the on-screen scale of the field; dividing |Z|
// by it produces a width-correct line in screen space.
float nodalLine(vec2 p, vec2 nm, float widthPx) {
    float Z   = chladni(p, nm);
    float aaw = max(fwidth(Z), 1e-5);
    float d   = abs(Z) / aaw;            // distance to zero in pixels
    return 1.0 - smoothstep(widthPx * 0.5, widthPx * 0.5 + 1.2, d);
}

// Iron-filings dipole field (mood 2). Two opposite poles produce a
// vector field; we visualize streamlines orthogonal to ∇ψ where ψ is
// the magnetic scalar potential of two point charges.
float dipoleStream(vec2 p, float t) {
    vec2 a = vec2(0.30, 0.50) + 0.04 * vec2(sin(t * 0.21), cos(t * 0.17));
    vec2 b = vec2(0.70, 0.50) + 0.04 * vec2(cos(t * 0.19), sin(t * 0.23));
    vec2 da = p - a, db = p - b;
    // Tangent angle of the field-line through p (perpendicular to ∇ψ).
    vec2 Bv = da / (dot(da, da) + 1e-3) - db / (dot(db, db) + 1e-3);
    float ang = atan(Bv.y, Bv.x);
    // Streamline density via cosine ridge along the field direction.
    return 0.5 + 0.5 * cos(ang * 7.0 + 12.0 * (p.x + p.y));
}

void main() {
    vec2 uv  = isf_FragNormCoord.xy;
    vec2 ndc = uv * 2.0 - 1.0;

    float t      = TIME;
    float audio  = clamp(audioReact, 0.0, 2.0);
    float bass   = clamp(audioBass  * audio, 0.0, 1.5);
    float mid    = clamp(audioMid   * audio, 0.0, 1.5);
    float treb   = clamp(audioHigh  * audio, 0.0, 1.5);
    float level  = clamp(audioLevel * audio, 0.0, 1.5);

    // ── mode-pair morph ───────────────────────────────────────────────
    // Steady drift through the 12 curated pairs every morphPeriod
    // seconds; a bass transient (>0.55) advances immediately.
    float period = max(4.0, morphPeriod);
    float baseIdx = t / period;
    // Latch a bass-triggered counter via a continuous proxy: we add
    // an offset that grows whenever bass exceeds a threshold,
    // smoothed so the morph isn't jittery.
    float bassKick = smoothstep(0.55, 0.95, bass);
    float idxF     = baseIdx + 1.5 * bassKick + 0.05 * sin(t * 0.07);

    int   ia = int(mod(floor(idxF),       12.0));
    int   ib = int(mod(floor(idxF) + 1.0, 12.0));
    float u  = fract(idxF);
    // Smoothstep blend so the cross-fade feels mechanical-but-soft.
    u = smoothstep(0.0, 1.0, u);

    vec2 nmA = modePair(ia);
    vec2 nmB = modePair(ib);
    vec2 nm  = mix(nmA, nmB, u);

    // ── plate coordinates ─────────────────────────────────────────────
    // Slight square-aspect correction: the classic Chladni plate is
    // square. We center the visible square in the longer dimension.
    float ar = RENDERSIZE.x / RENDERSIZE.y;
    vec2  p  = uv;
    if (ar > 1.0) p.x = (uv.x - 0.5) * ar + 0.5;
    else          p.y = (uv.y - 0.5) / ar + 0.5;

    // Agitation: when louder, sand jitters off the nodal lines. When
    // quieter, it settles. Settle slider biases the equilibrium.
    float agitate = clamp((level - settle * 0.6) * 1.3, 0.0, 1.0);
    vec2  jit     = (vec2(hash21(p * RENDERSIZE.xy),
                          hash21(p * RENDERSIZE.xy + 17.3)) - 0.5);
    p += jit * (0.0015 + 0.010 * agitate);

    // ── render the dominant mood ──────────────────────────────────────
    int   moodI = int(mood + 0.5);
    vec3  col;

    // Compute primary nodal line (used by all moods).
    float widthPx = lineWidth * (0.85 + 0.6 * mid);
    float line    = nodalLine(p, nm, widthPx);

    if (moodI == 1) {
        // ─ Water cymatics: shimmering surface, nodal lines as bright
        //   caustic crests. Plate becomes water; lines become highlights.
        vec3 water1 = vec3(0.04, 0.10, 0.16);
        vec3 water2 = vec3(0.02, 0.05, 0.10);
        // Subtle interference noise over the surface.
        float ripple = 0.5 + 0.5 * sin(40.0 * (p.x + p.y) + t * 1.4);
        vec3 base    = mix(water2, water1, 0.5 + 0.4 * ripple);
        // Caustic crests: sharp-bright on the nodal zero, with a soft
        // halo from a wider band that picks up displaced water.
        float halo = nodalLine(p, nm, widthPx * 4.0) - line;
        vec3  cau  = vec3(0.85, 0.95, 1.05);
        col  = base;
        col += cau * line * (0.9 + 1.2 * mid);
        col += cau * 0.35 * halo;
        // Sun-glint highlight following the bass pulse.
        col += vec3(1.0, 0.9, 0.7) * line * bassKick * 0.6;
    } else if (moodI == 2) {
        // ─ Iron filings around a dipole + Chladni mode overlay.
        //   The dipole gives directional streamlines; Chladni adds a
        //   secondary nodal pattern. Filings clump where both line up.
        vec3 plate = vec3(0.10, 0.09, 0.08);
        float dip  = dipoleStream(p, t);
        float dipL = smoothstep(0.55, 0.92, dip);
        // Modulate Chladni line by dipole strength so the iron clumps
        // visibly along the magnetic field direction.
        float combined = max(line * 0.85, dipL * (0.55 + 0.35 * line));
        vec3 iron = vec3(0.55, 0.50, 0.46);
        col = mix(plate, iron, combined * (0.7 + 0.5 * mid));
        // Specular pop on filing edges with bass.
        col += vec3(0.9, 0.85, 0.78) * combined * bassKick * 0.4;
    } else {
        // ─ Sand on plate (default, the canonical Chladni image).
        vec3 plate = vec3(0.18, 0.16, 0.14);
        // Warm cream sand; warmth slider biases to cooler bone if low.
        vec3 sandWarm = vec3(0.94, 0.88, 0.72);
        vec3 sandCool = vec3(0.86, 0.86, 0.84);
        vec3 sand     = mix(sandCool, sandWarm, clamp(warmth, 0.0, 1.0));
        // Plate has a faint brushed metal grain so the dark area lives.
        float brushed = 0.5 + 0.5 * sin(p.y * 540.0 + hash21(p) * 6.28);
        plate += vec3(0.018) * (brushed - 0.5);

        // Sand brightness: louder mids = more sand thrown, brighter line.
        float brightness = 0.85 + 1.45 * mid + 0.25 * level;
        // A wider settling halo around each line — sand piled into ridges.
        float halo = nodalLine(p, nm, widthPx * 3.5) - line;
        col  = plate;
        col  = mix(col, sand * brightness, line);
        col += sand * halo * (0.18 + 0.35 * mid);
    }

    // ── treble grains: tiny scattered sand particles on the lines ─────
    // Each grain is a stable hashed dot near a nodal line whose
    // appearance is gated by treble. Density slider scales count.
    {
        vec2 gp = floor(uv * RENDERSIZE.xy / 1.6);
        float h1 = hash21(gp);
        float h2 = hash21(gp + 7.7);
        float h3 = hash21(gp + 13.3);
        // Only spawn near a nodal line.
        float gateLine = nodalLine(p, nm, widthPx * 2.2);
        float gateRand = step(1.0 - 0.55 * grainDensity * (0.4 + treb), h1);
        float twinkle  = step(0.55, fract(h2 + t * (0.7 + 1.2 * treb) + h3));
        float grain    = gateLine * gateRand * twinkle;
        vec3  grainCol = vec3(1.0, 0.95, 0.82);
        col += grainCol * grain * (0.6 + 1.4 * treb);
    }

    // ── slow standing-wave shimmer keeps it alive in silence ──────────
    float shimmer = 0.5 + 0.5 * sin(t * 0.6 + (nm.x + nm.y) * 0.3);
    col *= 0.96 + 0.06 * shimmer;

    // ── soft vignette + paint-tooth grain ────────────────────────────
    col *= 1.0 - 0.20 * dot(ndc * 0.5, ndc * 0.5);
    col += (hash21(uv * RENDERSIZE.xy + t) - 0.5) * 0.008;

    // Mild HDR boost on the brightest sand for linear-output glow.
    float lum = dot(col, vec3(0.2126, 0.7152, 0.0722));
    col += col * smoothstep(0.85, 1.6, lum) * 0.6;

    gl_FragColor = vec4(col, 1.0);
}
