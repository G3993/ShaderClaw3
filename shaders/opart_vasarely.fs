/*{
  "CATEGORIES": ["Generator", "Op Art", "Audio Reactive"],
  "DESCRIPTION": "Op Art after Victor Vasarely & Bridget Riley — four canonical modes producing GENUINE optical vibration. (0) Vega: a square grid radially bulged into an apparent sphere; (1) Tridim: Vasarely's three-axis cube illusion in pure black/white; (2) Riley Wave: vertical bands modulated by a sinusoidal frequency-warp (after 'Cataract 3', 1967); (3) Zebra: serpentine ribbons (after Vasarely's 1937 proto-Op piece). Audio drives bulge depth (bass), wave frequency (mid), and angular shift (treble). Crisp fwidth-based AA — no blurry blobs. Color modes: Mono, Mono+Accent, Holographic (rainbow grid shift), Custom (colorA/colorB). Depth slider adds 3D perspective foreshortening; flowSpeed gives continuous gentle drift even at audio=0. Returns LINEAR HDR.",
  "INPUTS": [
    { "NAME": "mode", "LABEL": "Mood", "TYPE": "long",
      "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Vega","Tridim","Riley Wave","Zebra"] },
    { "NAME": "gridDensity", "LABEL": "Grid Density", "TYPE": "float", "MIN": 8.0, "MAX": 48.0, "DEFAULT": 22.0 },
    { "NAME": "bulgeAmount", "LABEL": "Bulge Depth",  "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.65 },
    { "NAME": "bulgeRadius", "LABEL": "Bulge Radius", "TYPE": "float", "MIN": 0.3, "MAX": 1.4, "DEFAULT": 0.85 },
    { "NAME": "waveFreq",    "LABEL": "Wave Frequency", "TYPE": "float", "MIN": 1.0, "MAX": 12.0, "DEFAULT": 4.5 },
    { "NAME": "waveAmp",     "LABEL": "Wave Amplitude", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "rotate",      "LABEL": "Rotation",       "TYPE": "float", "MIN": -3.14159, "MAX": 3.14159, "DEFAULT": 0.0 },
    { "NAME": "vpColor",     "LABEL": "VP Color (Tridim only)", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "colorMode",   "LABEL": "Color Mode", "TYPE": "long",
      "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Mono","Mono+Accent","Holographic","Custom"] },
    { "NAME": "colorA",      "LABEL": "Color A (Custom)", "TYPE": "color", "DEFAULT": [0.05, 0.18, 0.85, 1.0] },
    { "NAME": "colorB",      "LABEL": "Color B (Custom)", "TYPE": "color", "DEFAULT": [0.98, 0.78, 0.05, 1.0] },
    { "NAME": "depthAmount", "LABEL": "3D Depth", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "flowSpeed",   "LABEL": "Flow Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6 },
    { "NAME": "audioReact",  "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  VASARELY / RILEY  —  Op Art
//
//  Optical illusion REQUIRES geometric precision. We use fwidth() based
//  anti-aliasing on every edge so cells stay crisp at any resolution and
//  the eye actually engages the after-image / motion-illusion response.
//
//  Mode 0  Vega    — square grid warped by a radial bulge (Vega-Nor 1969)
//  Mode 1  Tridim  — three-axis isometric cube grid (Tridim 1968)
//  Mode 2  Riley   — vertical bands at non-uniform frequency (Cataract 3)
//  Mode 3  Zebra   — undulating ribbons (Vasarely 'Zebra' 1937)
// ════════════════════════════════════════════════════════════════════════

// Crisp band — value v in [0,1]; threshold 0.5; AA from screen-space deriv.
float aaStep(float v) {
    float w = fwidth(v) * 0.75 + 1e-5;
    return smoothstep(0.5 - w, 0.5 + w, v);
}

// Crisp box mask: 1 inside the unit-cell core of half-width hw, 0 outside.
float aaBox(vec2 p, float hw) {
    vec2 d = abs(p) - vec2(hw);
    float e = max(d.x, d.y);
    float w = fwidth(e) * 0.75 + 1e-5;
    return 1.0 - smoothstep(-w, w, e);
}

mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// HSV → RGB (linear) for holographic rainbow paths.
vec3 hsv2rgb(vec3 c) {
    vec3 p = abs(fract(c.xxx + vec3(0.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0);
    return c.z * mix(vec3(1.0), clamp(p - 1.0, 0.0, 1.0), c.y);
}

// ─── Mode 0: Vega — square grid radially compressed by a bulge ──────────
//   Returns rgb where r = parity-cell value, g = bulge-strength k (used
//   downstream to drive holographic hue shift), b = horizontal cell index
//   (also used for hue shift).
vec3 modeVega(vec2 uv, float t, float bulge, float radius, float dens, float depth) {
    vec2 p = uv;
    float r = length(p);
    // Spherical-bulge warp: r' = r * (1 - bulge * (1 - r/R)^2) inside R.
    float k = clamp(1.0 - r / max(radius, 1e-3), 0.0, 1.0);
    float warp = 1.0 - bulge * k * k;
    p *= warp;
    // Optional 3D foreshortening: tilt the grid plane in y so distant cells
    // shrink — produces a perspective dome on top of the radial bulge.
    if (depth > 1e-4) {
        float tilt = 0.55 * depth;
        // Slow tilt sweep so the dome breathes
        float sw = sin(t * 0.2) * 0.35 * depth;
        p.y *= 1.0 + tilt * (p.y * 0.5 + 0.5);
        p.x *= 1.0 + sw * p.y;
    }
    // Square grid; checkerboard parity gives the convex/concave reading.
    vec2 g = p * dens;
    vec2 cell = floor(g);
    vec2 f = fract(g) - 0.5;
    float parity = mod(cell.x + cell.y, 2.0);
    // Inside each cell, paint a smaller square that grows toward centre —
    // the size modulation IS the bulge cue (Vasarely 'Vega' compression).
    float sizeMod = mix(0.18, 0.46, k);
    float box = aaBox(f, sizeMod);
    float val = mix(parity, 1.0 - parity, box);
    // Pack helpful info for color path: cell hash for hue.
    float cellHash = fract(sin(cell.x * 12.9898 + cell.y * 78.233) * 43758.5453);
    return vec3(val, k, cellHash);
}

// ─── Mode 1: Tridim — isometric three-rhombus cube illusion ─────────────
//  Each hexagonal cell shows three rhombic faces (top, left, right) at
//  three brightness levels (white, mid-grey, black) — the canonical
//  "stack of cubes" Necker reversal. Pure Vasarely Tridim.
vec3 modeTridim(vec2 uv, float t, float dens, bool vp, float depth) {
    // Optional perspective foreshortening for a 3D-stacked-cubes feel.
    vec2 puv = uv;
    if (depth > 1e-4) {
        float k = 0.45 * depth;
        puv.y *= 1.0 + k * (puv.y * 0.5 + 0.5);
        puv *= 1.0 - 0.15 * depth;
    }
    // Hex grid via skewed axes
    vec2 p = puv * dens;
    vec2 a = vec2(1.0, 0.0);
    vec2 b = vec2(0.5, 0.8660254);
    // Solve p = u*a + v*b for (u,v)
    float v = p.y / b.y;
    float u = p.x - v * b.x;
    // Round to nearest lattice cell with proper hex tie-breaking
    float ru = floor(u + 0.5);
    float rv = floor(v + 0.5);
    vec2 cellPos = ru * a + rv * b;
    vec2 q = p - cellPos;
    // Determine which of three rhombic sectors (top, lower-left, lower-right)
    // by angle. Top = +y region; lower-left and lower-right split below.
    float ang = atan(q.y, q.x);
    // Faces: top  (60°..120°), right (-60°..60°), left (120°..240°)
    int face;
    if (ang > 1.0472 && ang <= 2.0944) face = 0;          // top
    else if (ang > -1.0472 && ang <= 1.0472) face = 1;    // right
    else face = 2;                                         // left
    // Subtle slow Necker reversal — swap face brightness rules over time
    float flip = step(0.0, sin(t * 0.18));
    float topV   = mix(1.0, 0.0, flip);
    float rightV = 0.5;
    float leftV  = mix(0.0, 1.0, flip);
    float val = (face == 0) ? topV : (face == 1) ? rightV : leftV;

    // Crisp rhombus edge — distance from cell centre projected on face axes
    // Use grid-line darkening to outline cubes (the Tridim ink lines).
    float edge = min(abs(q.x), min(abs(q.y), abs(q.x * 0.5 + q.y * 0.866)));
    edge = min(edge, abs(q.x * 0.5 - q.y * 0.866));
    float lw = fwidth(edge) * 1.2 + 0.02;
    float line = 1.0 - smoothstep(0.0, lw, edge - 0.0);
    line = clamp(line, 0.0, 1.0);

    vec3 col = vec3(val);
    if (vp) {
        // VP late-period palette: cobalt, cadmium yellow, black, white.
        // Strict assignment per face; no mixing, no interpolation.
        vec3 cobalt   = vec3(0.05, 0.18, 0.85);
        vec3 cadmium  = vec3(0.98, 0.78, 0.05);
        vec3 black    = vec3(0.0);
        vec3 white    = vec3(1.0);
        col = (face == 0) ? (flip > 0.5 ? black   : white)
            : (face == 1) ? cobalt
            :               (flip > 0.5 ? cadmium : black);
    }
    col *= 1.0 - line * 0.85;
    // Encode face id in alpha-ish channel via tiny offset on .b for hue path.
    // We return RGB straight; downstream colorize() detects vp via uniform.
    return col;
}

// ─── Mode 2: Riley Wave — vertical bands with sinusoidal freq warp ──────
vec3 modeRiley(vec2 uv, float t, float freq, float amp, float depth) {
    // Phase warp: integrate a frequency that varies sinusoidally across x.
    // Bands compress where dphase/dx is large, expand where small —
    // produces the unmistakable Riley 'Cataract' undulation.
    float x = uv.x;
    float y = uv.y;
    // Optional 3D depth: warp y as if viewed receding into the page.
    if (depth > 1e-4) {
        float dy = (y - 0.5);
        y = 0.5 + dy * (1.0 + 0.55 * depth * dy);
        x = 0.5 + (x - 0.5) * (1.0 - 0.25 * depth * (y - 0.5));
    }
    // A travelling sinusoid warps the column position
    float phi = 2.0 * 3.14159265 * freq * x
              + amp * sin(2.0 * 3.14159265 * (1.5 * x + 0.25 * y) + t * 0.7)
              + 0.4 * sin(t * 0.35 + y * 6.0);
    float v = 0.5 + 0.5 * sin(phi);
    // Pack horizontal phase fraction into .g for holographic hue shift.
    return vec3(aaStep(v), fract(phi / 6.2831853), 0.0);
}

// ─── Mode 3: Zebra — undulating ribbons crossing the canvas ─────────────
vec3 modeZebra(vec2 uv, float t, float freq, float amp, float depth) {
    // Ribbons follow a curved center-line; Vasarely's 1937 piece reads as
    // a flat zebra silhouette built from continuous serpentine bands.
    vec2 puv = uv;
    if (depth > 1e-4) {
        // Cylindrical bend: wrap x around an apparent cylinder.
        float dx = puv.x - 0.5;
        puv.x = 0.5 + dx * (1.0 - 0.4 * depth * dx * dx * 4.0);
        puv.y += 0.08 * depth * sin(puv.x * 3.14159);
    }
    float yWarp = puv.y + amp * 0.35 * sin(puv.x * 2.0 + t * 0.4)
                       + amp * 0.18 * sin(puv.x * 5.5 - t * 0.25);
    float xWarp = puv.x + amp * 0.25 * sin(puv.y * 2.7 - t * 0.3);
    // Stripes perpendicular to the local tangent — combine x and y phase
    float phi = (xWarp * freq * 1.3 + yWarp * freq * 0.4) * 3.14159265;
    float v = 0.5 + 0.5 * sin(phi);
    return vec3(aaStep(v), fract(phi / 6.2831853), 0.0);
}

// ─── Color map: convert a mono base + spatial hue cue into final color ──
//  base.r  = mono value (0..1)
//  base.g  = secondary cue (cell-k, phase fraction)
//  base.b  = tertiary cue (cell hash)
//  pos     = uv before centering (for global gradient)
vec3 colorize(vec3 base, vec2 pos, float t, int cmode, vec3 cA, vec3 cB) {
    float mono = base.r;
    if (cmode == 0) {
        // Pure mono — black/white.
        return vec3(mono);
    } else if (cmode == 1) {
        // Mono + Accent: bright cells take cA, dark cells stay near black,
        // mid-grey areas (Tridim 0.5) take a cool accent.
        vec3 hi = cA;
        vec3 lo = vec3(0.0);
        vec3 mid = mix(lo, hi, 0.35);
        // Distance from 0.5 picks mid-band
        float midW = 1.0 - smoothstep(0.0, 0.18, abs(mono - 0.5));
        vec3 c = mix(lo, hi, mono);
        return mix(c, mid, midW * 0.65);
    } else if (cmode == 2) {
        // Holographic: rainbow hue advancing across position + time +
        // secondary cue (cell hash / phase fraction). Mono modulates value.
        float hue = fract(pos.x * 0.55 + pos.y * 0.30
                        + base.b * 0.40 + base.g * 0.25
                        + t * 0.04);
        // Hologram-y: high saturation, slight inversion shimmer.
        float shimmer = 0.06 * sin(t * 1.3 + pos.y * 8.0);
        vec3 rainbow = hsv2rgb(vec3(hue, 0.85, 1.0 + shimmer));
        // Multiply by mono so the Op-Art structure is preserved.
        return rainbow * (0.18 + 0.82 * mono);
    } else {
        // Custom: lerp between user colors A and B by mono value.
        return mix(cA, cB, mono);
    }
}

// ─── main ───────────────────────────────────────────────────────────────
void main() {
    vec2 uv  = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Centred coords with aspect compensation (square cells everywhere).
    vec2 p = uv - 0.5;
    p.x *= aspect;

    // Continuous flow gives motion even at audio=0.
    float flow = clamp(flowSpeed, 0.0, 2.0);
    float t = TIME * (0.6 + 0.7 * flow);  // base 0.6× speed; flow accelerates.
    float audio = clamp(audioReact, 0.0, 2.0);
    float depth = clamp(depthAmount, 0.0, 1.0);

    // Treble nudges rotation; TIME breathes a slow drift so it lives in silence.
    // flowSpeed adds a continuous gentle angular drift.
    float rot = rotate
              + 0.05 * sin(t * 0.13)
              + 0.08 * flow * sin(t * 0.07)
              + 0.04 * audioHigh * audio;
    p = rot2(rot) * p;

    // Slow spatial drift so the pattern wanders even in silence (flow-driven).
    vec2 driftedUV = uv;
    driftedUV.x += 0.015 * flow * sin(t * 0.31);
    driftedUV.y += 0.012 * flow * cos(t * 0.27);
    p += vec2(0.012 * flow * sin(t * 0.23),
              0.010 * flow * cos(t * 0.19));

    // Bass deepens the bulge; mid wobbles wave frequency.
    // flowSpeed adds a continuous gentle breathing on bulge/wave.
    float bulge = bulgeAmount * (0.85 + 0.25 * sin(t * 0.5))
                * (1.0 + 0.18 * flow * sin(t * 0.41))
                * (1.0 + 0.45 * audioBass * audio);
    float wFreq = waveFreq * (1.0 + 0.18 * sin(t * 0.4))
                * (1.0 + 0.12 * flow * sin(t * 0.29))
                * (1.0 + 0.35 * audioMid * audio);
    float wAmp  = waveAmp  * (1.0 + 0.12 * flow * sin(t * 0.33))
                             * (1.0 + 0.25 * audioMid * audio);

    int m = int(mode + 0.5);
    int cmode = int(colorMode + 0.5);
    vec3 base;
    if      (m == 0) base = modeVega  (p, t, bulge, bulgeRadius, gridDensity, depth);
    else if (m == 1) base = modeTridim(p, t, gridDensity * 0.45, vpColor, depth);
    else if (m == 2) base = modeRiley (driftedUV, t, wFreq, wAmp, depth);
    else             base = modeZebra (driftedUV, t, wFreq * 0.6, wAmp, depth);

    vec3 col;
    if (m == 1 && vpColor) {
        // Tridim VP mode keeps its strict palette regardless of colorMode.
        col = base;
    } else {
        col = colorize(base, uv, t, cmode, colorA.rgb, colorB.rgb);
    }

    // Output is pure black/white (or VP palette) by design — no vignette,
    // no haze: the optical illusion only fires on hard contrast edges.
    // A whisper of LINEAR HDR boost at the brightest extreme lets the
    // host's tone-map breathe without softening the edges.
    col = clamp(col, 0.0, 1.2);
    // micro-overshoot on whites only (HDR cue) — keeps blacks pure.
    col += step(0.95, max(max(col.r, col.g), col.b)) * 0.06;

    gl_FragColor = vec4(col, 1.0);
}
