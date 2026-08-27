/*{
  "DESCRIPTION": "Chrome Cypher — liquid metal seen through shattered glass: molten chrome blobs pour through a cubist mosaic of angular panes, every shard refracting the same liquid from its own stolen angle, seams burning with cyan-magenta circuit light, data-rain glyphs ghosting past in the dark shards. Futurist, cold, precise. Bass swells the chrome, beats re-shatter the panes, mids pour the liquid faster, highs spark the circuit seams.",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "shardScale",   "LABEL": "Shard Scale",   "TYPE": "float", "MIN": 2.0, "MAX": 10.0, "DEFAULT": 5.0,  "GROUP": "Shape / Geometry" },
    { "NAME": "refractAmt",   "LABEL": "Cubist Refract","TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Shape / Geometry" },
    { "NAME": "chromeFat",    "LABEL": "Chrome Amount", "TYPE": "float", "MIN": 0.4, "MAX": 1.6,  "DEFAULT": 1.0,  "GROUP": "Shape / Geometry" },
    { "NAME": "neonAmt",      "LABEL": "Neon Seams",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.7,  "GROUP": "Color" },
    { "NAME": "glyphAmt",     "LABEL": "Data Rain",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.4,  "GROUP": "Color" },
    { "NAME": "hueShift",     "LABEL": "Neon Hue",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "brightness",   "LABEL": "Brightness",    "TYPE": "float", "MIN": 0.3, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "flowSpeed",    "LABEL": "Pour Speed",    "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Motion / Animation" },
    { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Audio Reactivity" },
    { "NAME": "trailAmt",   "LABEL": "Motion Trails", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Depth / Passes" },
    { "NAME": "bloomAmt",   "LABEL": "Bloom Depth",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Depth / Passes" },
    { "NAME": "aberration", "LABEL": "Lens Depth",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Depth / Passes" }
  ],
  "PASSES": [
    { "TARGET": "abTrail", "PERSISTENT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// CHROME CYPHER v2 — deeper cubism, truer metal. TWO shatter scales: a
//   coarse voronoi cuts a few HUGE panes; hash-chosen coarse cells
//   sub-fracture into fine shard clusters (hierarchical break). Every
//   pane is a sheet of glass at its own hashed depth: parallax drift +
//   slight zoom per depth, so the mosaic reads as layered panes, not a
//   flat tile map. The shared liquid-chrome field is shaded against a
//   real environment: horizon gradient (cool sky / dark floor), a hot
//   horizon filament, rotating softbox strips, and neon bounce light
//   from below — plus sharp fresnel edges tinted by the seam circuitry.
//   Seams are live traces: bright pulses RUN along them, triple-point
//   junctions glow as nodes. Dark panes keep the data rain.
// ─────────────────────────────────────────────────────────────────────────

#define R RENDERSIZE.xy
#define TAU 6.2831853

float hash11(float n) { return fract(sin(n) * 43758.5453123); }
float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}
vec3 pal(float h) { return 0.5 + 0.5 * cos(TAU * h + vec3(0.0, 2.094, 4.188)); }

// shared liquid-chrome field: 7 metaballs pouring
float chromeField(vec2 p, float t, float fat) {
    float f = 0.0;
    for (int i = 0; i < 7; i++) {
        float fi = float(i);
        float h1 = hash11(fi * 12.99 + 1.0);
        float h2 = hash11(fi * 4.41 + 7.0);
        vec2 c = vec2(0.55 * sin(t * (0.31 + 0.22 * h1) + h1 * TAU),
                      0.45 * sin(t * (0.24 + 0.28 * h2) + h2 * TAU));
        float r = fat * (0.16 + 0.14 * h2);
        f += exp(-dot(p - c, p - c) / (r * r) * 1.8);
    }
    return f;
}

// voronoi with 1st/2nd/3rd nearest — seam AND junction info.
// Sites orbit smoothly (no discrete reseeds): the shatter is alive, never jumps.
void voro(vec2 gp, float tv, out float d1, out float d2, out float d3, out vec2 bestId) {
    vec2 cellI = floor(gp);
    d1 = 1e3; d2 = 1e3; d3 = 1e3; bestId = vec2(0.0);
    for (int oy = -1; oy <= 1; oy++)
    for (int ox = -1; ox <= 1; ox++) {
        vec2 o = vec2(float(ox), float(oy));
        vec2 id = cellI + o;
        float h1 = hash21(id + 0.31);
        float h2 = hash21(id * 1.7 + 3.0);
        vec2 site = id + 0.5 + 0.40 * vec2(sin(tv * (0.5 + 0.7 * h1) + h1 * TAU),
                                           cos(tv * (0.4 + 0.8 * h2) + h2 * TAU));
        float d = length(gp - site);
        if (d < d1) { d3 = d2; d2 = d1; d1 = d; bestId = id; }
        else if (d < d2) { d3 = d2; d2 = d; }
        else if (d < d3) { d3 = d; }
    }
}

// studio environment for the liquid metal: horizon + softboxes + neon bounce
vec3 envMap(vec3 rv, float t, vec3 neonA, vec3 neonB, vec3 neonC) {
    float y = rv.y;
    vec3 sky = mix(vec3(0.055, 0.075, 0.13), vec3(0.62, 0.76, 0.98), smoothstep(-0.05, 0.75, y));
    vec3 flo = mix(vec3(0.030, 0.022, 0.050), vec3(0.015, 0.015, 0.028), smoothstep(-0.1, -0.9, y));
    vec3 e = mix(flo, sky, smoothstep(-0.05, 0.07, y));
    e += vec3(1.0, 0.82, 0.58) * exp(-abs(y - 0.015) * 26.0) * 0.85;     // hot horizon filament
    float az = atan(rv.x, rv.z + 0.2);
    float strip = smoothstep(0.70, 0.96, cos(az * 3.0 + t * 0.14));      // rotating softboxes
    e += vec3(0.85, 0.92, 1.0) * strip * smoothstep(0.02, 0.45, y) * 1.15;
    float strip2 = smoothstep(0.82, 0.98, cos(az * 5.0 - t * 0.09 + 1.7));
    e += neonC * strip2 * smoothstep(0.0, 0.4, y) * 0.45;
    e += neonA * exp(-abs(y + 0.42) * 5.5) * 0.40;                       // secondary bounce
    e += neonB * exp(-abs(y + 0.78) * 6.5) * 0.34;
    return e;
}

vec3 renderScene() {
    vec2 fc = gl_FragCoord.xy;
    vec2 q = (fc - 0.5 * R) / R.y;

    float t   = TIME * flowSpeed * 0.5;
    float amt = audioReact;
    float bassP  = pow(smoothstep(0.05, 0.85, audioBass), 1.5);
    float midP   = pow(smoothstep(0.06, 0.85, audioMid),  1.2);
    float highP  = pow(smoothstep(0.10, 0.90, audioHigh), 1.2);
    float levelP = clamp(audioLevel, 0.0, 1.0);
    float beatP  = clamp(audioBeatPulse, 0.0, 1.0);

    vec3 neonA = pal(hueShift + 0.50);                            // cyan side
    vec3 neonB = pal(hueShift + 0.92);                            // magenta side
    vec3 neonC = pal(hueShift + 0.12);                            // amber accent

    // ── hierarchical shatter: huge panes + fine shard clusters ──────────
    float tv = TIME * 0.11;                                       // slow living drift
    float sC = shardScale * 0.42;                                 // coarse: a few HUGE panes
    float sF = shardScale * 1.75;                                 // fine: shard clusters
    float d1C, d2C, d3C, d1F, d2F, d3F;
    vec2 idC, idF;
    voro(q * sC, tv, d1C, d2C, d3C, idC);
    voro(q * sF + 31.7, tv * 1.35, d1F, d2F, d3F, idF);
    float subFlag = step(0.40, hash21(idC * 2.9 + 5.0));          // which coarse cells shatter
    float seamC = (d2C - d1C) / sC;
    float seamF = (d2F - d1F) / sF;
    float seam = mix(seamC, min(seamC, seamF), subFlag);          // fine shards nest inside coarse
    float jd   = mix((d3C - d1C) / sC, min((d3C - d1C) / sC, (d3F - d1F) / sF), subFlag);
    vec2 paneId = mix(idC * 7.3, idF, subFlag);
    float hcell  = hash21(paneId * 3.7 + 11.0);
    float hcell2 = hash21(paneId * 1.3 + 29.0);
    float depth  = hcell2;                                        // pane's glass depth 0(front)..1(back)

    // ── cubist refraction + per-depth parallax: layered glass ───────────
    float ang = (hcell - 0.5) * (1.2 + 1.4 * subFlag) * refractAmt;
    float caa = cos(ang), saa = sin(ang);
    vec2 pq = vec2(caa * q.x - saa * q.y, saa * q.x + caa * q.y);
    pq *= 1.0 + (depth - 0.5) * 0.22 * refractAmt;                // deeper panes see wider
    pq += refractAmt * 0.20 * (vec2(hcell, hcell2) - 0.5)
        + (depth - 0.5) * 0.085 * refractAmt
          * vec2(sin(t * 0.23 + hcell * TAU), cos(t * 0.19 + hcell2 * TAU))  // parallax drift
        + amt * beatP * 0.012 * (vec2(hcell, hcell2) - 0.5);

    // ── shared liquid chrome, shaded per-pane ───────────────────────────
    float fat = chromeFat * (1.0 + amt * 0.28 * bassP);
    float pour = 1.0 + amt * 0.8 * midP;
    float f = chromeField(pq, t * pour, fat);
    vec2 e = vec2(0.010, 0.0);
    vec2 grad = vec2(chromeField(pq + e.xy, t * pour, fat) - f,
                     chromeField(pq + e.yx, t * pour, fat) - f) / e.x;
    float th = 0.42;
    float metal = smoothstep(th, th + 0.025, f);                  // sharper metal silhouette
    vec3 n = normalize(vec3(-grad, 0.85));
    vec3 rv = reflect(vec3(0.0, 0.0, -1.0), n);
    vec3 chrome = envMap(rv, t, neonA, neonB, neonC);
    // sharp fresnel edge tinted by the circuitry
    float fr = pow(1.0 - clamp(n.z, 0.0, 1.0), 3.0);
    chrome = chrome * (0.55 + 0.45 * (1.0 - fr));
    chrome += mix(neonA, neonB, hcell) * fr * (0.9 + amt * 0.5 * highP);
    // key-light specular, tight
    vec3 L = normalize(vec3(0.35, 0.55, 0.76));
    chrome += vec3(1.0, 0.97, 0.92) * pow(max(dot(n, L), 0.0), 42.0) * 1.1;
    // caustic filament where the field skims its own threshold
    chrome += mix(neonC, vec3(1.0), 0.5) * exp(-abs(f - th - 0.10) * 34.0) * 0.30;

    // ── dark pane interiors: data rain ──────────────────────────────────
    vec3 dark = mix(vec3(0.012, 0.015, 0.028), vec3(0.045, 0.045, 0.085), hcell2);
    dark *= 0.65 + 0.35 * (1.0 - depth);                          // deeper glass = darker
    float colX = floor(pq.x * 34.0);
    float rain = fract(hash11(colX * 7.7) + t * (0.5 + hash11(colX) * 1.2));
    float gy = fract(pq.y * 17.0 + rain * 6.0);
    float glyph = step(0.55, hash21(vec2(colX, floor(pq.y * 17.0 + rain * 6.0))))
                * smoothstep(0.0, 0.15, gy) * smoothstep(0.6, 0.45, gy);
    dark += mix(neonA, mix(neonB, neonC, step(0.7, hash11(colX * 9.1))), hash11(colX * 3.0))
          * glyph * glyphAmt
          * (0.25 + 0.35 * smoothstep(0.8, 1.0, rain)) * (1.0 + amt * 0.6 * highP);
    // micro-circuit hatching etched in the glass (two scales, crisp)
    vec2 mg = abs(fract(pq * 26.0) - 0.5);
    float micro = smoothstep(0.46, 0.495, max(mg.x, mg.y)) * step(0.6, hash21(floor(pq * 26.0) + 4.2));
    vec2 mg2 = abs(fract(pq * 64.0) - 0.5);
    float micro2 = smoothstep(0.44, 0.49, max(mg2.x, mg2.y)) * step(0.78, hash21(floor(pq * 64.0) + 9.4));
    dark += mix(neonA, neonB, hcell) * (micro * 0.20 + micro2 * 0.12) * glyphAmt;

    vec3 col = mix(dark, chrome, metal);
    col *= 0.72 + 0.28 * (1.0 - depth * 0.85);                    // depth-graded lighting

    // ── living circuit seams: traces, running pulses, junction nodes ────
    float seamLine = smoothstep(0.022, 0.007, seam);
    float seamGlow = exp(-seam * 11.0);
    vec3 seamCol = mix(mix(neonA, neonB, hcell), neonC, step(0.86, hcell2));
    float flicker = 0.80 + 0.20 * sin(t * (3.0 + hcell * 4.0) + hcell * TAU);
    flicker *= 1.0 + amt * (0.9 * beatP + 0.6 * highP);
    // pulses RUNNING along the seams (traveling wave in pane-hash direction)
    float pdir = hcell * TAU;
    float pulsePh = dot(q, vec2(cos(pdir), sin(pdir))) * 10.0
                  - t * (2.2 + 2.4 * hcell) - amt * 2.2 * midP;
    float pulse = pow(0.5 + 0.5 * sin(pulsePh), 14.0);
    col += seamCol * neonAmt * (seamLine * (0.55 + 1.9 * pulse) + seamGlow * 0.26) * flicker;
    // junction nodes at triple points
    float node = smoothstep(0.055, 0.014, jd);
    float nodeBlink = 0.55 + 0.45 * sin(t * (4.0 + hcell2 * 5.0) + hcell2 * TAU);
    col += mix(neonB, neonA, hcell2) * neonAmt * node
         * (0.65 + 0.85 * nodeBlink + amt * 1.1 * beatP);
    col += vec3(1.0) * node * pulse * neonAmt * 0.8;              // pulse arriving at a node flares white

    // ── moody finish ────────────────────────────────────────────────────
    col *= 0.95 + 0.05 * sin(fc.y * 1.7);
    col *= 1.0 - 0.30 * smoothstep(0.42, 1.05, length(q));
    float gr = hash21(fc + fract(t) * vec2(17.0, 29.0)) - 0.5;
    col += 0.022 * gr;
    col *= brightness * mix(1.0, 0.78 + 0.34 * levelP + 0.26 * clamp(audioBass, 0.0, 1.0)
                                 + 0.14 * clamp(audioMid, 0.0, 1.0) + 0.16 * beatP, amt);
    return col;
}

// ─── multipass: trail accumulation → bloom + lens-depth composite ────────
void main() {
    vec2 uv = gl_FragCoord.xy / R;
    if (PASSINDEX == 0) {
        vec3 col = renderScene();
        // luminous motion trails persist in the buffer (8-bit-safe decay floor)
        vec3 prev = texture2D(abTrail, uv).rgb;
        float decay = 0.50 + 0.46 * trailAmt;
        col = max(col, prev * decay - 0.0045);
        // write-dither: keeps soft gradients band-free in 8-bit buffer hosts
        col += (hash21(gl_FragCoord.xy + fract(TIME) * 61.0) - 0.5) * 0.006;
        gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
    } else {
        // chromatic lens depth: R/B pulled apart along the radial axis
        vec2 dir = uv - 0.5;
        float ab = aberration * 0.0045;
        vec3 base;
        base.r = texture2D(abTrail, uv + dir * ab).r;
        base.g = texture2D(abTrail, uv).g;
        base.b = texture2D(abTrail, uv - dir * ab).b;
        // wide two-ring bloom of the bright field — real glow depth
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float a = float(i) * 0.7853982;
            vec2 o = vec2(cos(a), sin(a)) * (3.5 / R.y);
            bl += texture2D(abTrail, uv + o).rgb;
            bl += texture2D(abTrail, uv + o * 2.6).rgb * 0.6;
        }
        bl /= 12.8;
        bl = max(bl - 0.52, 0.0);
        vec3 col = base + bl * bl * bloomAmt * 1.8;
        // HD finish: S-curve contrast + saturation richness
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
        float lumHD = dot(col, vec3(0.299, 0.587, 0.114));
        col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
        gl_FragColor = vec4(col, 1.0);
    }
}
