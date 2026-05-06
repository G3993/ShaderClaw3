/*{
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "DESCRIPTION": "Stained glass — adaptive Voronoi subdivision with glass refraction, 3D lead cames, and backlit luminance.",
  "INPUTS": [
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4 },
    { "NAME": "cellDensity", "LABEL": "Cell Density", "TYPE": "float", "MIN": 4.0, "MAX": 24.0, "DEFAULT": 10.0 },
    { "NAME": "leadWeight", "LABEL": "Lead Weight", "TYPE": "float", "MIN": 0.005, "MAX": 0.05, "DEFAULT": 0.018 },
    { "NAME": "backlightIntensity", "LABEL": "Backlight", "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.6 },
    { "NAME": "palette", "LABEL": "Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Chartres", "Sainte-Chapelle", "Modern"] }
  ]
}*/

// Real medieval glass reads luminous because:
//   1. Cells are bounded by opaque lead cames that are darker than any
//      pigment, anchoring the eye and making the colour pop by contrast.
//   2. The pigments are deep, saturated, narrow-band (cobalt, manganese,
//      copper-ruby) — not pastels.
//   3. There is real backlight behind the panel — a sun or sky — that
//      blows the glass out to bright HDR peaks at the brightest cells.
// We model all three: Voronoi cell partition for the pieces, dark
// bevelled lines along cell boundaries (lead cames) with a 3D shadow
// to one side, and HDR backlit cell tinting so peak luminances exceed
// 1.0 in linear output.

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

vec2 hash22(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
    return fract(sin(p) * 43758.5453);
}

vec3 hash32(vec2 p) {
    return fract(sin(vec3(
        dot(p, vec2(127.1, 311.7)),
        dot(p, vec2(269.5, 183.3)),
        dot(p, vec2(113.5,  271.9))
    )) * 43758.5453);
}

// Returns:
//   .xy = cell id of the closest seed
//   .z  = distance to closest seed (F1)
//   .w  = distance from the boundary (|F2 - F1| approx) — small near edges
vec4 voronoi(vec2 uv, float drift) {
    vec2 ip = floor(uv);
    vec2 fp = fract(uv);
    float d1 = 1e9;
    float d2 = 1e9;
    vec2  best = vec2(0.0);
    vec2  bestOffset = vec2(0.0);
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            vec2 g = vec2(float(i), float(j));
            vec2 cellId = ip + g;
            vec2 r = hash22(cellId);
            // Slow drift on seed positions for life — each seed orbits
            // a small loop. Phase varies per cell so motion is incoherent.
            vec2 driftPhase = 6.2831853 * r;
            vec2 wob = 0.30 * vec2(sin(drift + driftPhase.x),
                                   cos(drift * 0.83 + driftPhase.y));
            vec2 seed = g + 0.5 + 0.45 * (r - 0.5) + wob;
            vec2 dv = seed - fp;
            float d = dot(dv, dv);
            if (d < d1) {
                d2 = d1;
                d1 = d;
                best = cellId;
                bestOffset = dv;
            } else if (d < d2) {
                d2 = d;
            }
        }
    }
    // Approximate edge distance: half the gap between F1 and F2 in
    // squared-dist space, converted back to linear-ish distance.
    float edgeDist = 0.5 * (sqrt(d2) - sqrt(d1));
    return vec4(best, sqrt(d1), edgeDist);
}

vec3 paletteColor(float idx, int paletteSel) {
    // Chartres — deep cobalt, ruby, emerald, amber, royal purple, black.
    // (Black is for the rare lead-line piece tint.)
    vec3 chartres[6];
    chartres[0] = vec3(0.07, 0.18, 0.95);   // cobalt
    chartres[1] = vec3(0.92, 0.10, 0.18);   // ruby
    chartres[2] = vec3(0.10, 0.55, 0.22);   // emerald
    chartres[3] = vec3(0.98, 0.65, 0.12);   // amber
    chartres[4] = vec3(0.35, 0.08, 0.55);   // royal purple
    chartres[5] = vec3(0.04, 0.04, 0.06);   // lead-line black

    // Sainte-Chapelle — heavier on cobalt and ruby, the celebrated
    // 13th-century "Parisian blue" dominates.
    vec3 sainte[6];
    sainte[0] = vec3(0.04, 0.14, 1.10);     // parisian blue (slightly HDR)
    sainte[1] = vec3(1.00, 0.08, 0.14);     // ruby
    sainte[2] = vec3(0.06, 0.42, 0.90);     // sky blue
    sainte[3] = vec3(0.95, 0.78, 0.20);     // gold
    sainte[4] = vec3(0.55, 0.05, 0.30);     // crimson-purple
    sainte[5] = vec3(0.03, 0.03, 0.05);

    // Modern — cleaner contemporary palette (Chagall / Matisse style).
    vec3 modern[6];
    modern[0] = vec3(0.20, 0.55, 0.95);     // cyan-blue
    modern[1] = vec3(0.95, 0.30, 0.45);     // coral
    modern[2] = vec3(0.30, 0.85, 0.55);     // mint
    modern[3] = vec3(1.00, 0.80, 0.30);     // saffron
    modern[4] = vec3(0.55, 0.45, 0.85);     // lavender
    modern[5] = vec3(0.08, 0.08, 0.10);

    // Sample pseudo-randomly into the palette. Bias away from the
    // black slot (idx -> 5) so most cells are coloured glass, not lead.
    float t = fract(idx * 7.137);
    int pick = int(floor(t * 5.0));         // 0..4 only — never the black
    if (paletteSel == 1) {
        if (pick == 0) return sainte[0];
        if (pick == 1) return sainte[1];
        if (pick == 2) return sainte[2];
        if (pick == 3) return sainte[3];
        return sainte[4];
    } else if (paletteSel == 2) {
        if (pick == 0) return modern[0];
        if (pick == 1) return modern[1];
        if (pick == 2) return modern[2];
        if (pick == 3) return modern[3];
        return modern[4];
    }
    if (pick == 0) return chartres[0];
    if (pick == 1) return chartres[1];
    if (pick == 2) return chartres[2];
    if (pick == 3) return chartres[3];
    return chartres[4];
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 aspect = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

    int paletteSel = int(clamp(palette + 0.5, 0.0, 2.0));

    // Density controls cell count. Audio gently scales drift speed so
    // glass "breathes" with the music without rearranging the panel.
    float density = max(cellDensity, 1.0);
    float driftT  = TIME * (0.06 + 0.18 * audioReact);

    // Voronoi sampling space. We multiply by aspect so cells are square,
    // not stretched, regardless of panel proportions.
    vec2 vuv = uv * aspect * density;

    // --- Glass refraction ---
    // Real glass is uneven and refracts the backlight. We apply a small
    // displacement to the sampling position based on a low-frequency
    // hash field so each cell looks subtly bumpy, with chromatic offset
    // between R and B sampling — that is the "stained glass" caustic feel.
    vec2 refractOff = vec2(
        sin(vuv.x * 0.7 + vuv.y * 1.1 + driftT * 0.5),
        cos(vuv.y * 0.9 - vuv.x * 0.6 + driftT * 0.4)
    ) * 0.04;

    vec4 vR = voronoi(vuv + refractOff * 1.10, driftT);
    vec4 vG = voronoi(vuv,                     driftT);
    vec4 vB = voronoi(vuv - refractOff * 1.10, driftT);

    // Use the green channel's cell as the "real" cell ID. The R and B
    // channels' separate Voronoi sample IDs become the refractive tint
    // shift only when sampled within the same cell — otherwise we clamp.
    float idxG = hash21(vG.xy);
    vec3  cellColor = paletteColor(idxG, paletteSel);

    // Per-cell HDR luminance variation. Some cells sit nearly clear
    // (peak), others deeply pigmented. Backlight sun position warms the
    // upper-centre region.
    float idxL = hash21(vG.xy + vec2(17.3, 9.1));
    vec2 sunDir = vec2(0.5, 0.78) - uv;
    float sunFall = exp(-dot(sunDir, sunDir) * 4.5);

    // Backlight in linear space — peaks 1.8..2.5 at the brightest cells
    // when backlightIntensity is at its higher range.
    float cellBright = mix(0.55, 2.5, idxL);
    float backlight  = backlightIntensity * (0.7 + 1.5 * sunFall) * cellBright;

    // The pigment blocks most of the backlight — saturated pigments
    // transmit narrow bands. We model that by tinting the backlight by
    // the cell colour, so the bright spots are coloured light, not white.
    vec3 transmitted = cellColor * backlight;

    // Tiny chromatic dispersion: slightly different luminance per channel
    // only along the refraction direction. Keep subtle.
    float chromR = 1.0 + 0.04 * (vR.z - vG.z);
    float chromB = 1.0 - 0.04 * (vG.z - vB.z);
    transmitted.r *= chromR;
    transmitted.b *= chromB;

    // Hot core for the brightest cells — let them clip into HDR.
    float hot = smoothstep(0.78, 1.0, idxL) * sunFall;
    transmitted += vec3(0.6, 0.5, 0.35) * hot;

    // Per-cell painterly micro-noise (sand, bubbles, imperfections in
    // hand-blown glass) so cells aren't dead flat.
    float micro = hash21(floor(vuv * 80.0) + vG.xy);
    transmitted *= (0.94 + 0.12 * micro);

    // --- Lead cames (3D black bevelled lines) ---
    // edgeDist == vG.w: small near a cell boundary, large in the
    // interior. We carve the came as a thick black line, then add a
    // bevel highlight on one side and a shadow on the other for 3D feel.
    float lw = leadWeight;
    float leadCore   = 1.0 - smoothstep(lw * 0.55, lw * 1.05, vG.w);
    float leadOuter  = 1.0 - smoothstep(lw * 1.05, lw * 1.85, vG.w);

    // Bevel: brighter on the upper-left of each came, darker on the
    // lower-right. We approximate the came surface normal from the
    // gradient of the edge distance via a screen-space tilt.
    vec2 bevelLight = normalize(vec2(-0.7, 0.7));
    // Cheap normal proxy: vector from current UV toward the closest seed
    // is roughly perpendicular to the came; rotated, it points along it.
    vec2 toSeed = -normalize(vec2(vR.z - vG.z, vB.z - vG.z) + vec2(1e-4));
    float bevel = clamp(dot(toSeed, bevelLight), -1.0, 1.0);

    vec3 leadColor = vec3(0.02, 0.02, 0.025);
    float bevelHi  = 0.18 * max(bevel, 0.0)  * leadCore;
    float bevelLo  = 0.18 * max(-bevel, 0.0) * leadCore;

    // Soft shadow under the came — the outer ring darkens the adjacent
    // glass slightly, selling the 3D "the came sits proud of the glass"
    // read.
    float shadow = (leadOuter - leadCore) * 0.45;
    transmitted *= (1.0 - shadow);

    vec3 col = mix(transmitted, leadColor + vec3(bevelHi) - vec3(bevelLo), leadCore);

    // Tiny global vignette so the panel reads as a window, not a tile.
    float r = length(uv - 0.5);
    col *= (1.0 - 0.18 * r * r);

    // Output linear HDR — peaks remain >1.0 in the bright spots, which
    // the host's tonemapper or bloom pass will resolve.
    gl_FragColor = vec4(col, 1.0);
}
