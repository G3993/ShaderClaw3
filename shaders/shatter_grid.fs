/*{
  "CATEGORIES": ["Generator", "3D"],
  "DESCRIPTION": "Mirror-tile grid that explodes on bass — each tile reveals a piece of the texture. Doug Aitken mirror installations × Daniel Buren mirror compositions × Olafur Eliasson 'Your Dark Room' (2002) × David Hockney iPhone mosaics × Matrix Reloaded pixelated burst. Single-pass screen-space mosaic with audio-reactive shatter, slight perspective foreshortening, sharp grout, edge glints, and four mood tints (Mirror / Stained Glass / Pixel / Hockney). Procedural rainbow fallback when no input texture is bound. Returns LINEAR HDR; host applies ACES.",
  "INPUTS": [
    { "NAME": "mood",          "LABEL": "Mood",            "TYPE": "long",  "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Mirror Mosaic","Stained Glass","Pixel Mosaic","Hockney"] },
    { "NAME": "gridDensity",   "LABEL": "Grid Density",    "TYPE": "long",  "DEFAULT": 1, "VALUES": [0,1,2,3], "LABELS": ["Sparse","Med","Dense","Tight"] },
    { "NAME": "explodeStrength","LABEL":"Explode Strength","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "explodeDecay",  "LABEL": "Explode Decay",   "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.2 },
    { "NAME": "groutWidth",    "LABEL": "Grout Width",     "TYPE": "float", "MIN": 0.0, "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "perspective",   "LABEL": "Perspective",     "TYPE": "float", "MIN": 0.0, "MAX": 0.4, "DEFAULT": 0.10 },
    { "NAME": "tileTilt",      "LABEL": "Tile Tilt (Hockney only)", "TYPE": "float", "MIN": 0.0, "MAX": 0.30, "DEFAULT": 0.10 },
    { "NAME": "groutColor",    "LABEL": "Grout Color",     "TYPE": "color", "DEFAULT": [0.04, 0.04, 0.05, 1.0] },
    { "NAME": "ambient",       "LABEL": "Ambient",         "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.08 },
    { "NAME": "exposure",      "LABEL": "Exposure",        "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "inputTex",      "LABEL": "Texture",         "TYPE": "image" }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  Mirror-tile shatter grid
//  Screen-space mosaic. The canvas is split into a fixed tile grid (12×8
//  to 24×16); each tile samples a different region of inputTex (chosen
//  by per-tile hash). On a bass kick the tiles fly outward from canvas
//  center, rotating + fading, then settle ~1.2s later. Four moods
//  restyle the surface (mirror / stained glass / pixel / Hockney). No
//  raymarcher — the "3D" reading comes from perspective foreshortening
//  + per-tile rotation + edge highlights.
// ════════════════════════════════════════════════════════════════════════

// ─── hash / colour helpers ─────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2  hash22(vec2 p)  {
    return fract(sin(vec2(dot(p, vec2(127.1, 311.7)),
                          dot(p, vec2(269.5, 183.3)))) * 43758.5453);
}
vec3  hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// ─── procedural fallback when inputTex is unbound / black ──────────────
vec3 rainbowFallback(vec2 uv, float t) {
    float a = atan(uv.y - 0.5, uv.x - 0.5);
    float r = length(uv - 0.5);
    float h = fract(a / 6.2831853 + t * 0.05 + r * 0.6);
    return hsv2rgb(vec3(h, 0.78, 0.95));
}

// ─── stained-glass curated palette (6 deep saturated tints) ────────────
vec3 stainedPick(float seed) {
    int i = int(mod(floor(seed * 6.0), 6.0));
    if (i == 0) return vec3(0.78, 0.10, 0.18); // ruby
    if (i == 1) return vec3(0.10, 0.28, 0.62); // chartres cobalt
    if (i == 2) return vec3(0.95, 0.78, 0.20); // gold
    if (i == 3) return vec3(0.18, 0.55, 0.32); // emerald
    if (i == 4) return vec3(0.55, 0.20, 0.62); // violet
    return            vec3(0.92, 0.45, 0.18);  // amber
}

// ─── grid resolution from gridDensity enum ─────────────────────────────
vec2 gridDimsFromIndex(int idx) {
    if (idx <= 0) return vec2(12.0,  8.0);
    if (idx == 1) return vec2(16.0, 10.0);
    if (idx == 2) return vec2(20.0, 14.0);
    return            vec2(24.0, 16.0);
}

// ─── safe input-tex sample with rainbow fallback on near-black ─────────
vec3 sampleSource(vec2 st, float t) {
    vec2 cl = clamp(st, vec2(0.001), vec2(0.999));
    vec3 tex = IMG_NORM_PIXEL(inputTex, cl).rgb;
    float lum = dot(tex, vec3(0.299, 0.587, 0.114));
    float useFb = 1.0 - smoothstep(0.005, 0.03, lum);
    return mix(tex, rainbowFallback(cl, t), useFb);
}

void main() {
    vec2 uv  = isf_FragNormCoord.xy;
    vec2 ndc = (uv * 2.0 - 1.0) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

    float t      = TIME;
    float audio  = clamp(audioReact, 0.0, 2.0);
    int   m      = int(clamp(float(mood), 0.0, 3.0) + 0.5);
    int   gIdx   = int(clamp(float(gridDensity), 0.0, 3.0) + 0.5);
    vec2  grid   = gridDimsFromIndex(gIdx);

    // ── Synthetic bass-kick envelope ─────────────────────────────────
    // Pulse train shaped to feel like a four-on-the-floor kick. Decay
    // is driven by explodeDecay (≈ 1.2 → ~1.2s settle). Heavier accent
    // every 4th beat. audioReact scales overall amplitude.
    float beatHz   = 1.6;                                // ~96 bpm feel
    float phase    = fract(t * beatHz);
    float kick     = exp(-phase * (1.5 + explodeDecay));
    float bar      = floor(t * beatHz);
    float accent   = (mod(bar, 4.0) < 0.5) ? 1.25 : 1.0;
    float burst    = clamp(kick * accent * audio, 0.0, 2.0);
    float ease     = burst * explodeStrength;

    // ── Continuous slow drift (keeps grid alive in silence) ──────────
    float driftAmp = 0.004;
    vec2  drift    = vec2(sin(t * 0.31), cos(t * 0.27)) * driftAmp;

    // ── Mild perspective foreshortening (back / top tiles smaller) ──
    vec2  centered = uv - 0.5;
    float depth    = (1.0 - uv.y);                       // 0 bottom → 1 top
    float pScale   = 1.0 + perspective * (depth - 0.5) * 0.9;
    vec2  pUv      = 0.5 + centered / pScale + drift;

    // ── Tile coordinates ─────────────────────────────────────────────
    vec2  tileF    = pUv * grid;
    vec2  cell     = floor(tileF);
    vec2  inCell   = fract(tileF);                       // 0..1 inside tile

    // ── Per-tile random seeds (stable across frames) ────────────────
    vec2  rnd2     = hash22(cell + 17.3);
    float rndA     = hash21(cell * 1.731 + 4.21);
    float rndB     = hash21(cell * 0.913 + 9.77);

    // ── Explosion offset: each tile flies outward from canvas center.
    // Velocity ∝ (tileCenter - canvasCenter). Per-tile jitter so the
    // explosion isn't a sterile radial fan. Tiles farther out fly farther.
    vec2  tileCenter = (cell + 0.5) / grid;
    vec2  outward    = tileCenter - vec2(0.5);
    float oLen       = max(length(outward), 0.0001);
    vec2  oDir       = outward / oLen;
    vec2  jitter     = (rnd2 - 0.5) * 0.6;
    vec2  vel        = oDir * (0.35 + 0.55 * oLen) + jitter * 0.25;
    float explodeAmt = ease * 0.18;
    vec2  explodeOfs = vel * explodeAmt;

    // ── Per-tile rotation: Hockney always-on tilt + burst spin ──────
    float tiltBase = (m == 3) ? (rnd2.x - 0.5) * 2.0 * tileTilt : 0.0;
    float spinAmt  = ease * (0.6 + rnd2.y * 1.4);
    float rotA     = tiltBase + spinAmt * sign(rnd2.x - 0.5);

    // ── Re-sample inside the tile with the explode transform applied.
    // Rotate inCell about tile-center, then translate by -explodeOfs in
    // tile-space (so tile content visually flies outward post-mapping).
    vec2  local    = inCell - 0.5;
    float c_       = cos(rotA), s_ = sin(rotA);
    local          = mat2(c_, -s_, s_, c_) * local;
    local         -= explodeOfs * grid;                  // tile-space shift
    vec2  tileUv   = local + 0.5;

    // ── Inside-tile mask (else grout). Cracks widen during burst. ───
    float gw       = groutWidth + ease * 0.004;
    vec2  edgeD    = min(tileUv, 1.0 - tileUv);
    float edgeMin  = min(edgeD.x, edgeD.y);
    float aaWidth  = 0.0015;
    float tileMask = smoothstep(0.0, gw + aaWidth, edgeMin);

    // ── Edge glint: thin bright band just inside the seam ───────────
    float glintBand = smoothstep(gw * 1.2, gw * 0.4, edgeMin)
                    - smoothstep(gw * 2.6, gw * 1.4, edgeMin);
    glintBand = max(glintBand, 0.0);

    // ── Each tile reveals a different sub-rect of inputTex ──────────
    float winSize = mix(0.18, 0.42, rndA);
    vec2  winOfs  = rnd2 * (1.0 - winSize);
    vec2  texUv   = winOfs + tileUv * winSize;

    // Pixel-mosaic: collapse to a single sample per tile (center)
    if (m == 2) texUv = winOfs + vec2(0.5) * winSize;

    vec3 src = sampleSource(texUv, t);

    // ── Mood routing ────────────────────────────────────────────────
    vec3 tileCol = src;
    if (m == 0) {
        // Mirror Mosaic — slight desaturate + cool chrome tint + sheen
        float lum  = dot(src, vec3(0.299, 0.587, 0.114));
        tileCol    = mix(src, vec3(lum), 0.35);
        tileCol   *= vec3(0.96, 0.99, 1.05);
        float sheen = 0.5 + 0.5 * sin((rndA + rndB) * 9.0 + t * 0.3);
        tileCol   += vec3(0.12, 0.14, 0.18) * sheen * 0.25;
    } else if (m == 1) {
        // Stained Glass — multiply by curated palette tint per tile +
        // darken near interior edge for a leaded-glass feel
        vec3 tint  = stainedPick(rndB);
        tileCol    = mix(src, src * tint * 1.6, 0.75);
        float lead = smoothstep(0.5, 0.0, edgeMin * 8.0);
        tileCol   *= mix(1.0, 0.55, lead * 0.4);
    } else if (m == 2) {
        // Pixel Mosaic — flat colour per tile, slight saturation push,
        // posterize so it reads arcade-like
        tileCol    = floor(tileCol * 5.0 + 0.5) / 5.0;
        float lum  = dot(tileCol, vec3(0.299, 0.587, 0.114));
        tileCol    = mix(vec3(lum), tileCol, 1.18);
    } else {
        // Hockney — per-tile exposure + warmth jitter for chaos
        tileCol   *= mix(0.85, 1.20, rndA);
        tileCol    = mix(tileCol, tileCol * vec3(1.06, 1.00, 0.92), 0.5);
    }

    // ── Per-tile fade during burst (tiles dim as they fly out) ──────
    float tileFade = 1.0 - 0.55 * ease * (0.4 + 0.6 * rnd2.x);
    tileCol *= clamp(tileFade, 0.0, 1.5);

    // ── Compose: tile interior vs grout vs glint ───────────────────
    vec3 grout = groutColor.rgb;
    if (m == 0) grout = mix(grout, vec3(0.18, 0.20, 0.24), 0.35); // silver
    if (m == 1) grout = mix(grout, vec3(0.02, 0.02, 0.03), 0.6);  // dark lead
    if (m == 3) grout = mix(grout, vec3(0.08, 0.06, 0.05), 0.4);  // warm

    vec3 col = mix(grout, tileCol, tileMask);

    // Edge glints (mirror sells most; subtle elsewhere)
    float glintStrength = (m == 0) ? 0.55 : 0.18;
    vec3  glintHue      = (m == 1) ? vec3(0.95, 0.92, 0.78)
                                   : vec3(0.95, 0.97, 1.00);
    col += glintHue * glintBand * glintStrength * tileMask;

    // ── Subtle perspective shading: top-of-frame tiles dim slightly ─
    float depthShade = 1.0 - perspective * (depth - 0.5) * 0.8;
    col *= clamp(depthShade, 0.4, 1.2);

    // ── Burst flash — tiny global brightening at the moment of kick ─
    col += vec3(0.20, 0.22, 0.26) * ease * 0.15;

    // ── Ambient floor (prevents pitch-black grout on dim inputs) ────
    col += vec3(ambient) * (0.3 + 0.7 * tileMask);

    // ── Vignette + fine grain ───────────────────────────────────────
    col *= 1.0 - 0.18 * dot(ndc * 0.5, ndc * 0.5);
    col += (hash21(uv * RENDERSIZE.xy + t) - 0.5) * 0.008;

    // ── Linear HDR out (host applies ACES) ──────────────────────────
    col *= exposure;
    gl_FragColor = vec4(col, 1.0);
}
