/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Cubism after Picasso's analytic phase — Portrait of Kahnweiler (1910) and Ma Jolie (1912). N translucent rectangular planes, each rotated and sampling the source from its own viewpoint, alpha-stacked with per-plane gradient shading. Near-monochrome ochre/umber palette, optional stencilled letter fragments. Picasso's grid of perspectives rendered as overlapping SDF planes — no tessellation, no facets.",
  "INPUTS": [
    { "NAME": "planes", "LABEL": "Planes", "TYPE": "float", "MIN": 3.0, "MAX": 18.0, "DEFAULT": 11.0 },
    { "NAME": "planeSize", "LABEL": "Plane Size", "TYPE": "float", "MIN": 0.06, "MAX": 0.45, "DEFAULT": 0.22 },
    { "NAME": "planeAlpha", "LABEL": "Plane Alpha", "TYPE": "float", "MIN": 0.10, "MAX": 0.85, "DEFAULT": 0.42 },
    { "NAME": "rotateRange", "LABEL": "Rotation Range", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.55 },
    { "NAME": "scaleVar", "LABEL": "Scale Variance", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "aspectVar", "LABEL": "Aspect Variance", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6 },
    { "NAME": "centerBias", "LABEL": "Center Bias", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65 },
    { "NAME": "viewpointSpread", "LABEL": "Viewpoint Spread", "TYPE": "float", "MIN": 0.0, "MAX": 0.6, "DEFAULT": 0.22 },
    { "NAME": "lightDir", "LABEL": "Light Direction", "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 1.2 },
    { "NAME": "shading", "LABEL": "Plane Shading", "TYPE": "float", "MIN": 0.0, "MAX": 0.8, "DEFAULT": 0.42 },
    { "NAME": "edgeDarkness", "LABEL": "Edge Darkness", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "ochreStrength", "LABEL": "Ochre Strength", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.82 },
    { "NAME": "warmth", "LABEL": "Palette Warmth", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "vignette", "LABEL": "Vignette", "TYPE": "float", "MIN": 0.0, "MAX": 0.8, "DEFAULT": 0.35 },
    { "NAME": "letterFragments", "LABEL": "Letter Fragments", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35 },
    { "NAME": "drift", "LABEL": "Composition Drift", "TYPE": "float", "MIN": 0.0, "MAX": 0.20, "DEFAULT": 0.05 },
    { "NAME": "recomposeRate", "LABEL": "Recompose Rate", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "compositionSeed", "LABEL": "Seed", "TYPE": "float", "MIN": 0.0, "MAX": 80.0, "DEFAULT": 0.0 },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

// Picasso's analytic cubism is *not* a Voronoi tile and *not* slit-scan
// columns. It is OVERLAPPING TRANSLUCENT PLANES, each one a flat
// rectangle shaded as if lit, sampling the subject through its own
// rotation and translation. The viewer perceives the same form from
// many angles simultaneously because the planes BLEED INTO each other.
//
// Implementation: walk N planes back-to-front, alpha-over composite.
// Each plane has independent rotation, size, aspect, sample-offset,
// and per-plane gradient shading driven by a shared lightDir + per-plane
// jitter. Final pass desaturates toward Picasso's umber-ochre LUT and
// optionally stamps a few stencilled letter fragments (the JOU / MA
// JOLIE / BAL signature flourishes of the analytic period).

float hash11(float n) {
    return fract(sin(n * 12.9898) * 43758.5453);
}
float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

vec3 ochreLut(vec3 c, float strength, float warm) {
    float L = dot(c, vec3(0.299, 0.587, 0.114));
    vec3 cool = vec3(0.32, 0.30, 0.26) + L * vec3(0.55, 0.50, 0.42);
    vec3 hot  = vec3(0.42, 0.32, 0.18) + L * vec3(0.62, 0.50, 0.30);
    vec3 ochre = mix(cool, hot, clamp(warm, 0.0, 1.0));
    return mix(c, ochre, strength);
}

// Procedural stencilled letter fragments — sparse mono-spaced glyph rows.
// Real text rendering would need a font atlas; the canonical Picasso
// letter scraps (BAL, JOU, MA JOLIE) are stylised enough that hash-driven
// vertical/horizontal strokes read correctly at painting scale.
float letterField(vec2 uv, float seed) {
    float total = 0.0;
    for (int g = 0; g < 4; g++) {
        float fg = float(g);
        vec2 origin = vec2(hash11(fg * 13.7 + seed),
                           hash11(fg * 17.1 + seed + 5.3));
        vec2 ld = (uv - origin) * vec2(72.0, 28.0);
        if (ld.x < 0.0 || ld.y < 0.0 ||
            ld.x > 12.0 + hash11(fg * 3.7) * 6.0 || ld.y > 3.0) continue;
        vec2 ci = floor(ld);
        float h = hash11(ci.x + ci.y * 13.0 + fg * 7.7 + seed);
        // Vertical stroke if h<0.55, horizontal crossbar if h<0.85.
        float vert = step(h, 0.55) * step(0.18, fract(ld.x))
                   * step(fract(ld.x), 0.55)
                   * step(0.10, fract(ld.y))
                   * step(fract(ld.y), 0.92);
        float bar  = step(0.55, h) * step(h, 0.85)
                   * step(0.40, fract(ld.y))
                   * step(fract(ld.y), 0.62);
        total = max(total, max(vert, bar));
    }
    return total;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Background — warm cream paper, vignetted toward edges so the eye
    // is led to the central pile of planes (Picasso's compositions
    // collapse mass toward the centre of the frame).
    vec3 paper = vec3(0.82, 0.76, 0.62);
    float vig = smoothstep(0.30, 0.95, length(uv - 0.5));
    paper *= 1.0 - vig * vignette;
    vec3 col = paper;

    int N = int(clamp(planes, 1.0, 18.0));

    // Walk planes back-to-front. Each iteration tests if the fragment
    // lies inside the rotated rectangle and, if so, composites a sampled
    // and shaded patch onto the running col via alpha-over.
    for (int i = 0; i < 18; i++) {
        if (i >= N) break;

        float fi = float(i) + compositionSeed * 3.71;

        // Plane centre — biased toward canvas centre per centerBias.
        // Centres time-cycle continuously so the composition recomposes
        // even with audio silent.
        vec2 rawCtr = vec2(hash11(fi * 1.31) + 0.06 * sin(TIME * 0.4 + fi),
                           hash11(fi * 2.97 + 4.7) + 0.06 * cos(TIME * 0.31 + fi * 1.3));
        vec2 ctr = mix(rawCtr, vec2(0.5), centerBias);

        // Bass nudges centres outward briefly on impact.
        vec2 dr = vec2(sin(TIME * 0.27 + fi),
                       cos(TIME * 0.31 + fi * 1.3));
        ctr += dr * drift * (1.0 + audioBass * audioReact);

        // Per-plane fade cycle — planes phase between visible and ghost
        // so the painting is constantly recomposing.
        float fadeT = sin(TIME * recomposeRate + fi * 2.7) * 0.5 + 0.5;

        // Plane half-extents. AspectVar lets some planes be tall, some
        // squat — Picasso's planes are not uniform rectangles.
        float hSz = hash11(fi * 7.13);
        float hAp = hash11(fi * 9.71);
        vec2 halfSize = vec2(planeSize * (0.55 + hSz * 0.9),
                             planeSize * (0.30 + hAp * 0.9));
        halfSize *= 1.0 + (hSz - 0.5) * 2.0 * scaleVar;
        halfSize.y *= 0.5 + hAp * aspectVar;

        // Rotation
        float rot = (hash11(fi * 11.7) - 0.5) * 2.0 * rotateRange
                  + audioMid * audioReact * 0.08
                  * sign(hash11(fi * 11.7) - 0.5);
        float ca = cos(-rot), sa = sin(-rot);
        vec2 d  = uv - ctr;
        d.x *= aspect; halfSize.x *= 1.0; // keep size in screen-aspect space
        vec2 local = vec2(ca * d.x - sa * d.y,
                          sa * d.x + ca * d.y);

        // SDF-style inside test for an oriented rectangle.
        if (abs(local.x) > halfSize.x || abs(local.y) > halfSize.y) continue;

        // Per-plane "viewpoint" — each plane samples the input from a
        // slightly different region so adjacent planes show the same
        // content rotated/translated. This is the analytic-cubism
        // multi-perspective signature.
        vec2 vp = (vec2(hash11(fi * 23.7), hash11(fi * 29.3)) - 0.5)
                * viewpointSpread;
        vec2 sUV = (local / max(halfSize, vec2(1e-4))) * 0.45 + 0.5 + vp;

        vec3 sample;
        if (IMG_SIZE_inputTex.x > 0.0) {
            sample = texture(inputTex, fract(sUV)).rgb;
        } else {
            // Procedural fallback: warm horizontal striation, with a
            // TIME term per-plane so each plane's content shifts.
            float stripe = sin(sUV.y * 11.0 + sUV.x * 2.0
                              + TIME * 0.5 + fi * 1.7) * 0.5 + 0.5;
            sample = mix(vec3(0.55, 0.43, 0.28),
                         vec3(0.30, 0.24, 0.18), stripe);
            sample *= 0.85 + 0.15 * sin(sUV.x * 23.0 + TIME * 0.3);
        }

        // Per-plane lighting: gradient along a direction = global lightDir
        // + per-plane jitter so adjacent planes don't share their shading
        // (which would flatten the pile back into one image).
        float ldJit = lightDir + (hash11(fi * 31.7) - 0.5) * 1.6;
        vec2  lvec  = vec2(cos(ldJit), sin(ldJit));
        float shade = dot(local / max(halfSize, vec2(1e-4)), lvec);
        shade = shade * 0.5 + 0.5;
        sample *= 1.0 - shading + shading * shade;

        // Alpha-over composite. Per-plane alpha jitter, multiplied by
        // the time-driven fade so planes phase in and out.
        float alpha = clamp(planeAlpha
                          * (0.55 + hash11(fi * 37.3) * 0.7)
                          * pow(fadeT, 1.5),
                          0.0, 0.95);
        col = mix(col, sample, alpha);

        // Hairline edge darkening — Picasso's planes are bounded by
        // pencil-line scaffolds inside the analytic phase.
        float edgeX = halfSize.x - abs(local.x);
        float edgeY = halfSize.y - abs(local.y);
        float edgeD = min(edgeX, edgeY) / planeSize;
        float edgeMask = smoothstep(0.0, 0.022, edgeD);
        col *= mix(1.0 - edgeDarkness * 0.6, 1.0, edgeMask);
    }

    // Stencilled letter fragments scattered across the painting.
    if (letterFragments > 0.0) {
        float lf = letterField(uv, compositionSeed);
        col = mix(col, vec3(0.10, 0.08, 0.06), lf * letterFragments);
    }

    // Final palette compression toward Picasso's umber-ochre.
    col = ochreLut(col, ochreStrength, warmth + audioHigh * audioReact * 0.15);

    // Subtle audio luminance breath.
    col *= 0.93 + audioLevel * audioReact * 0.12;

    gl_FragColor = vec4(col, 1.0);
}
