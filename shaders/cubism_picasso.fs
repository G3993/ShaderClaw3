/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Cubism after Picasso's analytic phase — Portrait of Kahnweiler (1910) and Ma Jolie (1912). N translucent rectangular planes, each rotated and sampling the source from its own viewpoint, alpha-stacked with per-plane gradient shading. Near-monochrome ochre/umber palette, optional stencilled letter fragments. Picasso's grid of perspectives rendered as overlapping SDF planes — no tessellation, no facets.",
  "INPUTS": [
    { "NAME": "picassoWork", "LABEL": "Painting", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Kahnweiler (1910)", "Demoiselles d'Avignon (1907)", "Three Musicians (1921)", "Guernica (1937)", "Ma Jolie (1912)"] },
    { "NAME": "planes", "LABEL": "Planes", "TYPE": "float", "MIN": 3.0, "MAX": 24.0, "DEFAULT": 20.0 },
    { "NAME": "planeSize", "LABEL": "Plane Size", "TYPE": "float", "MIN": 0.06, "MAX": 0.45, "DEFAULT": 0.18 },
    { "NAME": "planeAlpha", "LABEL": "Plane Alpha", "TYPE": "float", "MIN": 0.10, "MAX": 0.85, "DEFAULT": 0.55 },
    { "NAME": "edgeFeather", "LABEL": "Edge Feather", "TYPE": "float", "MIN": 0.0, "MAX": 0.6, "DEFAULT": 0.35 },
    { "NAME": "skewAmount",  "LABEL": "Plane Skew",   "TYPE": "float", "MIN": 0.0, "MAX": 0.9,  "DEFAULT": 0.45 },
    { "NAME": "rotateRange", "LABEL": "Rotation Range", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.42 },
    { "NAME": "scaleVar", "LABEL": "Scale Variance", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45 },
    { "NAME": "aspectVar", "LABEL": "Aspect Variance", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "centerBias", "LABEL": "Center Bias", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.78 },
    { "NAME": "viewpointSpread", "LABEL": "Viewpoint Spread", "TYPE": "float", "MIN": 0.0, "MAX": 0.6, "DEFAULT": 0.18 },
    { "NAME": "lightDir", "LABEL": "Light Direction", "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 2.4 },
    { "NAME": "shading", "LABEL": "Plane Shading", "TYPE": "float", "MIN": 0.0, "MAX": 0.8, "DEFAULT": 0.65 },
    { "NAME": "brushwork", "LABEL": "Brushwork", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35 },
    { "NAME": "edgeDarkness", "LABEL": "Edge Darkness", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65 },
    { "NAME": "ochreStrength", "LABEL": "Ochre Strength", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.00 },
    { "NAME": "warmth", "LABEL": "Palette Warmth", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.40 },
    { "NAME": "vignette", "LABEL": "Vignette", "TYPE": "float", "MIN": 0.0, "MAX": 0.8, "DEFAULT": 0.45 },
    { "NAME": "letterFragments", "LABEL": "Letter Fragments", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "drift", "LABEL": "Composition Drift", "TYPE": "float", "MIN": 0.0, "MAX": 0.20, "DEFAULT": 0.05 },
    { "NAME": "recomposeRate", "LABEL": "Recompose Rate", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.18 },
    { "NAME": "depthParallax", "LABEL": "Depth Parallax", "TYPE": "float", "MIN": 0.0, "MAX": 0.30, "DEFAULT": 0.10 },
    { "NAME": "spaceCurve",    "LABEL": "Space Curvature","TYPE": "float", "MIN": 0.0, "MAX": 0.50, "DEFAULT": 0.18 },
    { "NAME": "colorBlend",    "LABEL": "Color Blend",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "specStrength",  "LABEL": "Specular",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.45 },
    { "NAME": "specSharpness", "LABEL": "Specular Sharp", "TYPE": "float", "MIN": 0.0, "MAX": 0.10, "DEFAULT": 0.025 },
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

// 2D value noise — used for painterly brushwork inside facets.
float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

// Brush stroke noise — directional fbm tilted along the lit direction so
// the texture inside each facet reads as paint, not isotropic grain.
float brushNoise(vec2 p, float dir) {
    float c = cos(dir), s = sin(dir);
    vec2 q = vec2(c * p.x + s * p.y, -s * p.x + c * p.y);
    q.y *= 3.0; // anisotropic — strokes elongate along light direction
    float v = 0.0;
    float a = 0.55;
    for (int i = 0; i < 4; i++) {
        v += vnoise(q) * a;
        q *= 2.07;
        a *= 0.5;
    }
    return v;
}

// Soft SDF blob — used to assemble the figure's body parts.
float sdEllipse(vec2 p, vec2 c, vec2 r, float rot) {
    float ca = cos(rot), sa = sin(rot);
    vec2 d = p - c;
    d = vec2(ca * d.x - sa * d.y, sa * d.x + ca * d.y);
    return length(d / r) - 1.0;
}

// Procedural "Girl with a Mandolin" subject mass — used when no inputTex
// is bound. Returns vec2(luminance, depth) where:
//   .x = surface brightness 0..1 (modulated by a top-left light source)
//   .y = depth/density 0..1 (1 = on figure, 0 = empty paper)
// The figure is composed as a pile of overlapping ellipses: head, torso,
// shoulders, hips, mandolin oval, sound-hole, neck. Coordinates are in
// canvas-fitted UV space; aspect already pre-corrected by caller.
vec2 subjectMass(vec2 uv, float t) {
    // Slow drift so the subject "breathes" between facet recompositions.
    float dx = sin(t * 0.07) * 0.012;
    float dy = cos(t * 0.05) * 0.010;
    vec2 p = uv + vec2(dx, dy);

    float density = 0.0;

    // Head — top-center ellipse, slightly tilted (Picasso's head leans).
    density = max(density, smoothstep(0.10, -0.04, sdEllipse(p, vec2(0.50, 0.78), vec2(0.10, 0.13), -0.18)));
    // Neck
    density = max(density, smoothstep(0.06, -0.02, sdEllipse(p, vec2(0.50, 0.66), vec2(0.05, 0.06), 0.00)));
    // Shoulders / upper torso
    density = max(density, smoothstep(0.10, -0.04, sdEllipse(p, vec2(0.50, 0.55), vec2(0.18, 0.10), 0.00)));
    // Mid torso
    density = max(density, smoothstep(0.12, -0.04, sdEllipse(p, vec2(0.50, 0.42), vec2(0.20, 0.12), 0.00)));
    // Hips / sitting mass
    density = max(density, smoothstep(0.14, -0.04, sdEllipse(p, vec2(0.50, 0.26), vec2(0.24, 0.14), 0.00)));
    // Mandolin body — oval lower-right, hugged by the right arm
    density = max(density, smoothstep(0.10, -0.04, sdEllipse(p, vec2(0.62, 0.22), vec2(0.13, 0.16), 0.32)));
    // Mandolin neck running up-left toward the player's hand
    density = max(density, smoothstep(0.05, -0.02, sdEllipse(p, vec2(0.55, 0.36), vec2(0.04, 0.10), 0.55)));
    // Right arm reaching across to the mandolin
    density = max(density, smoothstep(0.08, -0.02, sdEllipse(p, vec2(0.45, 0.36), vec2(0.10, 0.05), -0.35)));

    // Top-left raking light gives the figure form. Light comes from upper
    // left; sin-curve falloff so cheekbones/torso top read as bright,
    // bottom-right reads as shadow.
    vec2 lightVec = normalize(vec2(-0.6, 0.8));
    float lit = dot(p - 0.5, lightVec) * 1.6 + 0.45;
    lit = clamp(lit, 0.05, 1.0);

    // Slight grain across the mass so it doesn't look like a flat cutout.
    float grain = vnoise(p * 18.0) * 0.15;
    float lum = lit * 0.85 + 0.10 + grain;
    lum = clamp(lum, 0.05, 1.05);

    return vec2(lum, density);
}

vec3 ochreLut(vec3 c, float strength, float warm) {
    float L = dot(c, vec3(0.299, 0.587, 0.114));
    vec3 cool = vec3(0.32, 0.30, 0.26) + L * vec3(0.55, 0.50, 0.42);
    vec3 hot  = vec3(0.42, 0.32, 0.18) + L * vec3(0.62, 0.50, 0.30);
    vec3 ochre = mix(cool, hot, clamp(warm, 0.0, 1.0));
    return mix(c, ochre, strength);
}

// Per-painting palette LUT — Picasso's career spans wildly different
// colour worlds: monochrome ochre Kahnweiler vs the pink+ochre+ultramarine
// of Demoiselles, the synthetic-flat saturated colour of Three Musicians,
// and the pure greyscale of Guernica.
vec3 picassoLut(int w, vec3 c, float strength) {
    float L = dot(c, vec3(0.299, 0.587, 0.114));
    vec3 target;
    if (w == 1) {            // Demoiselles 1907 — pink/ochre on ultramarine
        vec3 hot  = vec3(0.85, 0.62, 0.55) * L + vec3(0.18, 0.10, 0.06);
        vec3 cool = vec3(0.18, 0.20, 0.55) * L + vec3(0.05, 0.08, 0.18);
        target = mix(cool, hot, smoothstep(0.30, 0.65, L));
    } else if (w == 2) {     // Three Musicians 1921 — synthetic flat colour
        // Posterize toward a 4-colour synthetic-cubist palette.
        if      (L < 0.25) target = vec3(0.10, 0.08, 0.05);
        else if (L < 0.50) target = vec3(0.55, 0.18, 0.14);   // dark red
        else if (L < 0.75) target = vec3(0.92, 0.78, 0.18);   // mustard
        else               target = vec3(0.18, 0.30, 0.62);   // ultramarine
    } else if (w == 3) {     // Guernica 1937 — pure greyscale, hard contrast
        float Lc = pow(L, 0.85);
        target = vec3(Lc) * vec3(1.02, 1.00, 0.98);
    } else if (w == 4) {     // Ma Jolie 1911-12 — analytic with letter scraps
        // Same warm ochre as Kahnweiler but brown-shifted.
        vec3 cool = vec3(0.30, 0.28, 0.24) + L * vec3(0.55, 0.48, 0.40);
        vec3 hot  = vec3(0.45, 0.34, 0.20) + L * vec3(0.60, 0.46, 0.28);
        target = mix(cool, hot, 0.7);
    } else {                 // 0 = Kahnweiler / Girl with a Mandolin 1910
        // 4-stop lookup keyed on luminance, sampling the actual reference
        // palette: dark sepia → cool grey → warm tan → cream highlight.
        // Picasso's analytic monochrome lives in this narrow band.
        vec3 c0 = vec3(0.18, 0.16, 0.13);   // deep shadow sepia
        vec3 c1 = vec3(0.42, 0.40, 0.36);   // cool slate grey
        vec3 c2 = vec3(0.72, 0.65, 0.52);   // warm tan
        vec3 c3 = vec3(0.92, 0.88, 0.78);   // cream highlight
        if      (L < 0.33) target = mix(c0, c1, L / 0.33);
        else if (L < 0.66) target = mix(c1, c2, (L - 0.33) / 0.33);
        else               target = mix(c2, c3, (L - 0.66) / 0.34);
    }
    return mix(c, target, strength);
}

// Picasso analytic-cubism letter scraps — JOU / MA JOLIE / BAL are the
// canonical stencilled fragments. Drawn as explicit per-letter SDFs so
// the marks actually READ as letters at painting scale, not as noise.
float drawJ(vec2 p) {
    float bar  = step(0.40, p.x) * step(p.x, 0.60) * step(0.20, p.y);
    float hook = step(0.10, p.x) * step(p.x, 0.40)
               * step(0.10, p.y) * step(p.y, 0.30);
    return max(bar, hook);
}
float drawO(vec2 p) {
    vec2 d = p - 0.5;
    float r = length(d * vec2(1.4, 1.0));
    return step(0.30, r) * step(r, 0.45);
}
float drawU(vec2 p) {
    float L = step(0.10, p.x) * step(p.x, 0.30) * step(0.20, p.y);
    float R = step(0.70, p.x) * step(p.x, 0.90) * step(0.20, p.y);
    float B = step(0.10, p.x) * step(p.x, 0.90)
            * step(0.10, p.y) * step(p.y, 0.30);
    return max(max(L, R), B);
}
float drawA(vec2 p) {
    float L = step(0.10, p.x) * step(p.x, 0.28) * step(0.10, p.y);
    float R = step(0.72, p.x) * step(p.x, 0.90) * step(0.10, p.y);
    float T = step(0.10, p.x) * step(p.x, 0.90) * step(0.85, p.y);
    float M = step(0.20, p.x) * step(p.x, 0.80)
            * step(0.45, p.y) * step(p.y, 0.55);
    return max(max(L, R), max(T, M));
}
float letterField(vec2 uv, float seed) {
    // 4 letter cells at hashed-but-drifting positions, scaled to read
    // legibly across the canvas. Rotates through J / O / U / A.
    float total = 0.0;
    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        vec2 origin = vec2(0.18 + hash11(fi + seed) * 0.6,
                           0.18 + hash11(fi * 3.7 + seed) * 0.6);
        origin += 0.01 * vec2(sin(TIME * 0.20 + fi),
                              cos(TIME * 0.17 + fi));
        vec2 lp = (uv - origin) * vec2(18.0, 14.0);
        if (lp.x < 0.0 || lp.x > 1.0
         || lp.y < 0.0 || lp.y > 1.0) continue;
        float ink = (i == 0) ? drawJ(lp)
                  : (i == 1) ? drawO(lp)
                  : (i == 2) ? drawU(lp)
                             : drawA(lp);
        total = max(total, ink);
    }
    return total;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Space curvature — gentle barrel/lens warp before plane evaluation.
    // Pulls the composition into a curved-space cubist read instead of a
    // flat poster. Strength on spaceCurve.
    {
        vec2 c = uv - 0.5;
        float r2 = dot(c, c);
        uv = 0.5 + c * (1.0 + spaceCurve * r2 * 1.6);
    }

    // Background — warm cream paper, vignetted toward edges so the eye
    // is led to the central pile of planes (Picasso's compositions
    // collapse mass toward the centre of the frame).
    vec3 paper = vec3(0.74, 0.68, 0.56);
    float vig = smoothstep(0.30, 0.95, length(uv - 0.5));
    paper *= 1.0 - vig * vignette;

    // Faint paper grain so the background reads as canvas, not flat.
    paper *= 0.92 + 0.08 * vnoise(uv * 280.0);
    vec3 col = paper;

    int N = int(clamp(planes, 1.0, 24.0));

    // Walk planes back-to-front. Each iteration tests if the fragment
    // lies inside the rotated rectangle and, if so, composites a sampled
    // and shaded patch onto the running col via alpha-over.
    for (int i = 0; i < 24; i++) {
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

        // Per-plane DEPTH — distance from camera in [0..1]. Front planes
        // are larger and parallax-shifted more; back planes anchor the
        // composition. This is the "curvature of space and time" layer.
        float depth = hash11(fi * 53.7);
        // Apply parallax based on distance from canvas center
        vec2 cFromCenter = uv - 0.5;
        ctr += cFromCenter * (depth - 0.5) * depthParallax;
        // Front planes are larger and slow-drift more; back planes squat.
        float depthScale = mix(0.7, 1.4, depth);

        // Plane half-extents. AspectVar lets some planes be tall, some
        // squat — Picasso's planes are not uniform rectangles.
        float hSz = hash11(fi * 7.13);
        float hAp = hash11(fi * 9.71);
        vec2 halfSize = vec2(planeSize * (0.55 + hSz * 0.9),
                             planeSize * (0.30 + hAp * 0.9));
        halfSize *= 1.0 + (hSz - 0.5) * 2.0 * scaleVar;
        halfSize.y *= 0.5 + hAp * aspectVar;
        halfSize *= depthScale;

        // Rotation
        float rot = (hash11(fi * 11.7) - 0.5) * 2.0 * rotateRange
                  + audioMid * audioReact * 0.08
                  * sign(hash11(fi * 11.7) - 0.5);
        float ca = cos(-rot), sa = sin(-rot);
        vec2 d  = uv - ctr;
        d.x *= aspect; halfSize.x *= 1.0; // keep size in screen-aspect space
        vec2 local = vec2(ca * d.x - sa * d.y,
                          sa * d.x + ca * d.y);

        // Per-plane parallelogram skew — pushes each plane off-square so
        // the inside test rejects a tilted-rectangle (cubist polygon),
        // not just an axis-aligned one. Two independent skew angles per
        // plane give 4-sided polygons of varying obliqueness.
        float skX = (hash11(fi * 41.3) - 0.5) * 2.0 * skewAmount;
        float skY = (hash11(fi * 47.9) - 0.5) * 2.0 * skewAmount;
        vec2 skewed = vec2(local.x + local.y * skX,
                           local.y + local.x * skY);

        // Feathered SDF — distance from plane edge in normalized units.
        // Negative = inside, positive = outside. We keep planes that are
        // within edgeFeather of their boundary so edges blend painterly.
        float edgeX = halfSize.x - abs(skewed.x);
        float edgeY = halfSize.y - abs(skewed.y);
        float edgeDist = min(edgeX, edgeY) / planeSize;
        float feather = max(edgeFeather, 0.0001);
        if (edgeDist < -feather) continue;
        float edgeMix = clamp(edgeDist / feather + 0.5, 0.0, 1.0);
        edgeMix = smoothstep(0.0, 1.0, edgeMix);

        // Per-plane "viewpoint" — each plane samples the subject from a
        // slightly different region so adjacent planes show the same
        // content shifted/rotated. This is analytic cubism's signature.
        vec2 vp = (vec2(hash11(fi * 23.7), hash11(fi * 29.3)) - 0.5)
                * viewpointSpread;
        vec2 worldSampleUV = ctr + vp + (local * 0.6); // sample in world space
        vec2 sUV = (local / max(halfSize, vec2(1e-4))) * 0.45 + 0.5 + vp;

        vec3 sample_;
        if (IMG_SIZE_inputTex.x > 0.0) {
            sample_ = texture(inputTex, fract(sUV)).rgb;
        } else {
            // No input bound — show the cubist scaffold itself: a soft
            // tonal field driven by world position + slow noise, so each
            // facet reads as a faceted plane of paint without trying to
            // render a particular figure. The faceting IS the subject.
            // Depth-driven flow: front planes flow faster than back ones.
            float flowSpeed = mix(0.02, 0.10, depth);
            float field  = vnoise(worldSampleUV * 4.5 + vec2(TIME * flowSpeed, 0.0));
            float field2 = vnoise(worldSampleUV * 1.7 + fi * 0.31 + TIME * flowSpeed * 0.5);
            float lum = mix(0.30, 0.92, field * 0.7 + field2 * 0.3);
            // Per-depth tint — back layers cooler, front layers warmer
            vec3 depthTint = mix(vec3(0.78, 0.82, 0.95), vec3(1.05, 0.96, 0.78), depth);
            sample_ = vec3(lum) * depthTint;
            // Color blend across layers — weave a secondary hue tied to
            // a slowly-rotating phasor so the painting breathes color.
            vec3 hueA = vec3(0.95, 0.55, 0.25);
            vec3 hueB = vec3(0.30, 0.55, 0.92);
            float hueT = sin(TIME * 0.10 + fi * 0.7 + worldSampleUV.x * 2.0) * 0.5 + 0.5;
            sample_ = mix(sample_, mix(hueA, hueB, hueT) * lum * 1.15, colorBlend * 0.5);
        }

        // Per-plane lighting: gradient along a direction = global lightDir
        // + per-plane jitter so adjacent planes don't share their shading
        // (which would flatten the pile back into one image).
        float ldJit = lightDir + (hash11(fi * 31.7) - 0.5) * 1.6;
        vec2  lvec  = vec2(cos(ldJit), sin(ldJit));
        float shade = dot(local / max(halfSize, vec2(1e-4)), lvec);
        shade = shade * 0.5 + 0.5;
        sample_ *= 1.0 - shading + shading * shade;

        // Painterly brushwork — anisotropic noise tilted along light dir
        // gives each facet a visible stroke texture instead of flat fill.
        if (brushwork > 0.0) {
            float bn = brushNoise(local * 18.0 + fi * 4.7, ldJit);
            sample_ *= 1.0 - brushwork * 0.35 + brushwork * 0.35 * bn * 2.0;
        }

        // Alpha-over composite, modulated by feathered edge and per-plane
        // fade cycle so planes phase in and out without hard boundaries.
        float alpha = clamp(planeAlpha
                          * (0.55 + hash11(fi * 37.3) * 0.7)
                          * pow(fadeT, 1.5)
                          * edgeMix,
                          0.0, 0.95);
        col = mix(col, sample_, alpha);

        // Specular highlight on a chosen plane edge (front-facing toward
        // light direction). Mimics a subtle CG bevel — the cubism revival
        // through computer graphics, not a print.
        if (specStrength > 0.0) {
            float specEdge = smoothstep(specSharpness * 1.2, specSharpness * 0.2,
                                         abs(edgeDist) / planeSize);
            float specSide = max(dot(local / max(halfSize, vec2(1e-4)), lvec), 0.0);
            col += vec3(1.0, 0.95, 0.82) * specEdge * specSide * specStrength
                 * mix(0.4, 1.0, depth) * alpha;
        }

        // Hairline pencil-line scaffolding at the plane's actual edge
        // (not the feathered zone) — Picasso bounded analytic planes
        // with thin graphite contours.
        float lineMask = smoothstep(0.0, 0.012, edgeDist / planeSize);
        col *= mix(1.0 - edgeDarkness * 0.5, 1.0, lineMask);
    }

    // Stencilled letter fragments scattered across the painting.
    if (letterFragments > 0.0) {
        float lf = letterField(uv, compositionSeed);
        col = mix(col, vec3(0.10, 0.08, 0.06), lf * letterFragments);
    }

    // Final palette compression toward Picasso's umber-ochre.
    col = picassoLut(int(picassoWork), col, ochreStrength);

    // Subtle audio luminance breath.
    col *= 0.93 + audioLevel * audioReact * 0.12;

    gl_FragColor = vec4(col, 1.0);
}
