/*{
  "CATEGORIES": ["Generator", "Audio Reactive", "3D"],
  "DESCRIPTION": "Prism Basilica — gothic cathedral interior view. A single pointed-arch window emits an HDR sun core with per-channel dispersion through stained-glass panes. Volumetric god-rays sweep across cobalt, ruby, emerald and gold beams; black ink ribs form the tracery silhouette; refracted color pools wash the stone floor. Audio bass pulses the sun core, mid sways the rays, treble sparkles the dust motes. Returns LINEAR HDR.",
  "INPUTS": [
    { "NAME": "exposure",    "LABEL": "Exposure",       "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.05 },
    { "NAME": "sunIntensity","LABEL": "Sun Intensity",  "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 2.6 },
    { "NAME": "rayLength",   "LABEL": "Ray Length",     "TYPE": "float", "MIN": 0.2, "MAX": 1.6, "DEFAULT": 0.95 },
    { "NAME": "dispersion",  "LABEL": "Dispersion",     "TYPE": "float", "MIN": 0.0, "MAX": 0.18, "DEFAULT": 0.07 },
    { "NAME": "rayCount",    "LABEL": "Ray Count",      "TYPE": "float", "MIN": 4.0, "MAX": 24.0, "DEFAULT": 11.0 },
    { "NAME": "raySway",     "LABEL": "Ray Sway",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.7 },
    { "NAME": "dustDensity", "LABEL": "Dust Density",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.85 },
    { "NAME": "archHeight",  "LABEL": "Arch Height",    "TYPE": "float", "MIN": 0.4, "MAX": 1.2, "DEFAULT": 0.78 },
    { "NAME": "archWidth",   "LABEL": "Arch Width",     "TYPE": "float", "MIN": 0.15, "MAX": 0.55, "DEFAULT": 0.3 },
    { "NAME": "audioReact",  "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "stoneColor",  "LABEL": "Stone Color",    "TYPE": "color", "DEFAULT": [0.015, 0.012, 0.025, 1.0] },
    { "NAME": "sunColor",    "LABEL": "Sun Core",       "TYPE": "color", "DEFAULT": [1.0, 0.92, 0.55, 1.0] },
    { "NAME": "paneCobalt",  "LABEL": "Cobalt Pane",    "TYPE": "color", "DEFAULT": [0.10, 0.32, 1.0, 1.0] },
    { "NAME": "paneRuby",    "LABEL": "Ruby Pane",      "TYPE": "color", "DEFAULT": [1.0, 0.12, 0.28, 1.0] },
    { "NAME": "paneEmerald", "LABEL": "Emerald Pane",   "TYPE": "color", "DEFAULT": [0.18, 0.95, 0.45, 1.0] },
    { "NAME": "paneGold",    "LABEL": "Gold Pane",      "TYPE": "color", "DEFAULT": [1.0, 0.78, 0.15, 1.0] }
  ]
}*/

// ═══════════════════════════════════════════════════════════════════════
//   PRISM BASILICA
//   Pointed-arch cathedral window. Pure 2D analytical SDF, but reads as
//   3D through perspective convergence + volumetric god-ray accumulation.
//   Output: LINEAR HDR (peaks 3.0+).
// ═══════════════════════════════════════════════════════════════════════

const float PI = 3.14159265359;

float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

// Pointed-arch SDF: rectangle bottom + two circle arcs meeting at apex.
// p centered at base of arch (y=0 floor, y=h apex). w = half-width.
float sdGothicArch(vec2 p, float w, float h) {
    // Arc radius set so two arcs cross at the apex (x=0, y=h).
    // Each arc center at (±w, h - sqrt(r^2 - w^2)). We pick r = w * 1.4.
    float r  = w * 1.42;
    float yc = h - sqrt(max(r * r - w * w, 0.0));
    // Bottom rectangle (from y=0 to y=yc).
    vec2 d = vec2(abs(p.x) - w, max(0.0, yc) - p.y);
    float rect = max(d.x, max(-p.y, p.y - yc));
    // Right-arc center at (-w, yc) — mirrored for left.
    float arc = length(vec2(abs(p.x) + w, p.y - yc) * vec2(1.0, 1.0)) - r;
    // Inside the arch above yc: arc value; below: rect value.
    return (p.y > yc) ? arc : rect;
}

// Stained-glass pane assignment based on angular slice from arch center.
// Returns one of four jeweled colors mixed by angle around the sun.
vec3 paneColor(vec2 p, vec3 cob, vec3 rub, vec3 em, vec3 gold) {
    float a = atan(p.y, p.x);                    // -pi..pi
    float k = (a / PI) * 0.5 + 0.5;              // 0..1
    k = fract(k * 4.0);                          // 0..1 within slice
    float slice = floor((a / PI) * 0.5 + 0.5) * 4.0; // 0..3 slice id
    int  s = int(mod(slice, 4.0));
    vec3 c = cob;
    if (s == 1) c = rub;
    if (s == 2) c = em;
    if (s == 3) c = gold;
    return c;
}

// Gothic tracery — radial pattern of ink-black ribs inside the arch.
// Returns 0..1 ink density (1 = fully black rib).
float tracery(vec2 p, float w, float h) {
    // Vertical mullion (single bar down center).
    float mull = 1.0 - smoothstep(0.0035, 0.0085, abs(p.x));
    // Diagonal cross ribs.
    vec2 q = vec2(p.x, p.y - h * 0.5);
    float diag1 = 1.0 - smoothstep(0.005, 0.011, abs(q.x + q.y * 0.7));
    float diag2 = 1.0 - smoothstep(0.005, 0.011, abs(q.x - q.y * 0.7));
    // Quatrefoil rosette near apex.
    float rd = length(p - vec2(0.0, h * 0.78)) - 0.052;
    float rose = 1.0 - smoothstep(0.0, 0.012, abs(rd));
    // Petal cuts.
    for (int i = 0; i < 4; i++) {
        float ang = float(i) * PI * 0.5 + PI * 0.25;
        vec2 c = vec2(cos(ang), sin(ang)) * 0.052;
        float pd = length(p - vec2(0.0, h * 0.78) - c) - 0.034;
        rose = max(rose, 1.0 - smoothstep(0.0, 0.012, abs(pd)));
    }
    // Horizontal transom.
    float tran = 1.0 - smoothstep(0.0035, 0.008, abs(p.y - h * 0.42));
    tran *= step(abs(p.x), w * 0.96);
    return clamp(mull * step(p.y, h * 0.95) + diag1 + diag2 + rose + tran, 0.0, 1.0);
}

// God-ray accumulator — sample N rays radiating from sun position,
// each modulated by audio sway and per-channel hue offset for dispersion.
vec3 godRays(vec2 p, vec2 sun, float audio, float nRays, float swayAmt,
             float disp, vec3 baseTint, float lenK) {
    vec3 acc = vec3(0.0);
    vec2 d   = p - sun;
    float r  = length(d);
    float a  = atan(d.y, d.x);
    for (int i = 0; i < 24; i++) {
        if (float(i) >= nRays) break;
        float fi    = float(i);
        float baseA = (fi / nRays) * 2.0 * PI;
        float sway  = sin(TIME * (0.7 + 0.13 * fi) + fi * 1.7) * swayAmt * 0.18;
        float ang   = baseA + sway + 0.3 * audio * sin(TIME * 0.4 + fi);
        float dA    = abs(mod(a - ang + PI, 2.0 * PI) - PI);
        float beam  = exp(-dA * dA * 140.0);
        float fall  = exp(-r * (2.6 - 1.0 * lenK));
        // Per-ray jeweled tint cycling
        float h = fract(fi / nRays + TIME * 0.04);
        vec3 tint = vec3(
            0.5 + 0.5 * cos(6.28 * (h + 0.0)),
            0.5 + 0.5 * cos(6.28 * (h + 0.33)),
            0.5 + 0.5 * cos(6.28 * (h + 0.66)));
        // Dispersion shifts per-channel angular offset.
        float dR = abs(mod(a - ang - disp + PI, 2.0 * PI) - PI);
        float dB = abs(mod(a - ang + disp + PI, 2.0 * PI) - PI);
        vec3 chrom = vec3(exp(-dR * dR * 140.0), beam, exp(-dB * dB * 140.0));
        acc += tint * chrom * fall * (0.6 + 0.4 * audio);
    }
    return acc * baseTint * 0.45;
}

void main() {
    vec2 res = RENDERSIZE;
    vec2 uv  = isf_FragNormCoord.xy * 2.0 - 1.0;
    uv.x *= res.x / res.y;

    float audio = clamp(audioReact, 0.0, 2.0);
    float bass  = audioBass;
    float mid   = audioMid;
    float high  = audioHigh;
    float pulse = (0.7 + 0.6 * bass * audio);

    // Arch is centered slightly above floor with the camera tilted up.
    vec2  archP = vec2(uv.x, uv.y + 0.55);
    float arch  = sdGothicArch(archP, archWidth, archHeight);

    // Floor plane: y < -0.55 in original uv.
    bool floorRegion = (uv.y < -0.55);
    bool insideArch  = (arch < 0.0);

    // Sun position (in screen-uv space): apex of the arch interior.
    vec2 sunPos = vec2(0.0, archHeight * 0.35 - 0.55);
    vec2 toSun  = uv - sunPos;
    float sunR  = length(toSun);

    // ── 1) Stone wall (outside arch, above floor) ─────────────────────
    vec3 col = stoneColor.rgb;
    // Subtle stone texture: directional fbm-lite via hash bands.
    float stoneNoise = hash21(floor(uv * 28.0));
    col += vec3(0.012, 0.010, 0.014) * (stoneNoise - 0.5);

    // ── 2) Sky / outside view through the arch ────────────────────────
    if (insideArch) {
        // Stained-glass tint based on direction from sun.
        vec3 pane = paneColor(uv - sunPos,
                              paneCobalt.rgb, paneRuby.rgb,
                              paneEmerald.rgb, paneGold.rgb);
        // Sky base — saturated jewel ground darkening with distance.
        float skyFall = exp(-sunR * 1.3);
        col = pane * (0.35 + 1.4 * skyFall);

        // HDR sun core with bass pulse — peaks at 3.0+ linear.
        float coreR   = 0.045 + 0.012 * bass * audio;
        float core    = exp(-pow(sunR / coreR, 2.0));
        float halo    = exp(-pow(sunR / (coreR * 6.0), 2.0));
        col += sunColor.rgb * (core * 3.2 * sunIntensity * pulse
                              + halo * 0.55 * sunIntensity);

        // Per-channel chromatic fringing on the bright halo.
        float disp = dispersion * (0.6 + 0.6 * audio);
        vec3 fringe = vec3(
            exp(-pow((sunR + disp) / (coreR * 4.0), 2.0)),
            exp(-pow((sunR        ) / (coreR * 4.0), 2.0)),
            exp(-pow((sunR - disp) / (coreR * 4.0), 2.0)));
        col += sunColor.rgb * fringe * 0.7 * sunIntensity;
    }

    // ── 3) God rays (volumetric beams) — sample everywhere visible ───
    if (!floorRegion) {
        vec3 rays = godRays(uv, sunPos, audio, rayCount, raySway,
                            dispersion * 6.0, sunColor.rgb, rayLength);
        // Rays only escape through the arch opening; mask by arch interior
        // smoothly so beam edges blend with stone tracery.
        float mask = smoothstep(0.04, -0.02, arch);
        col += rays * mask * sunIntensity * (0.85 + 0.5 * mid * audio);
    }

    // ── 4) Floor — refracted prism caustic puddles + horizon line ────
    if (floorRegion) {
        // Pseudo-ground perspective projection (inverse-y depth).
        float depth = 1.0 / max(-uv.y - 0.55, 0.05);
        vec2  gp    = vec2(uv.x * depth, depth);
        // Stone base.
        col = stoneColor.rgb * 1.4;
        // Caustic puddles — three colored circles below the sun.
        for (int i = 0; i < 3; i++) {
            float fi = float(i);
            vec3 pc = (i == 0) ? paneCobalt.rgb
                    : (i == 1) ? paneRuby.rgb
                               : paneEmerald.rgb;
            vec2 pcen = vec2((fi - 1.0) * 0.55, 1.4 + 0.3 * fi);
            float pd  = length(gp - pcen) - 0.45;
            float pud = smoothstep(0.0, -0.3, pd);
            // Audio mid drives caustic tremor.
            pud *= 0.8 + 0.4 * sin(TIME * 1.2 + fi * 2.1 + mid * audio * 4.0);
            col += pc * pud * 1.6;
        }
        // Sun spill on near floor.
        float spill = exp(-pow(uv.x, 2.0) * 4.0) * smoothstep(-1.0, -0.55, uv.y);
        col += sunColor.rgb * spill * 0.55 * pulse;
    }

    // ── 5) Tracery ribs (black ink silhouette inside arch) ───────────
    if (insideArch) {
        float ink = tracery(uv - vec2(0.0, -0.55), archWidth, archHeight);
        col *= 1.0 - ink * 0.96;
    }

    // ── 6) Arch frame edge — soft AA rim of stone over jewel light ──
    float edge = 1.0 - smoothstep(-0.006, 0.008, arch);
    col = mix(col, stoneColor.rgb * 0.4, clamp(edge, 0.0, 1.0));

    // ── 7) Dust motes — sparkle on treble inside the rays ────────────
    if (insideArch || !floorRegion) {
        vec2  mp   = uv * 8.0 + vec2(TIME * 0.05, -TIME * 0.02);
        vec2  cell = floor(mp);
        vec2  fcl  = fract(mp);
        float h    = hash21(cell);
        float spark= step(0.985 - 0.04 * high * audio * dustDensity, h);
        float dot  = exp(-length(fcl - 0.5) * 16.0);
        col += sunColor.rgb * spark * dot * 1.4 * dustDensity;
    }

    // ── 8) Vignette + exposure ──────────────────────────────────────
    col *= 1.0 - 0.18 * dot(uv, uv) * 0.25;
    col *= exposure;

    gl_FragColor = vec4(col, 1.0);
}
