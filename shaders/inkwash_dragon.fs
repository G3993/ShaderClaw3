/*{
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "DESCRIPTION": "Inkwash Dragon — sumi-e mountain landscape with a calligraphic black ink dragon coiling through layered fog. A single HDR sun disk burns through bone-white sky; tapered brush-stroke spine threads between three painted ridges; gold-leaf eye and red vermillion seal punctuate the composition. Audio bass tremors the mountain, mid coils the dragon, treble flares the sun. Returns LINEAR HDR.",
  "INPUTS": [
    { "NAME": "exposure",    "LABEL": "Exposure",      "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.05 },
    { "NAME": "sunIntensity","LABEL": "Sun Intensity", "TYPE": "float", "MIN": 0.5, "MAX": 4.0, "DEFAULT": 2.4 },
    { "NAME": "sunSize",     "LABEL": "Sun Size",      "TYPE": "float", "MIN": 0.06, "MAX": 0.3, "DEFAULT": 0.13 },
    { "NAME": "sunPosX",     "LABEL": "Sun X",         "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "sunPosY",     "LABEL": "Sun Y",         "TYPE": "float", "MIN": -0.2, "MAX": 0.9, "DEFAULT": 0.45 },
    { "NAME": "dragonScale", "LABEL": "Dragon Scale",  "TYPE": "float", "MIN": 0.4, "MAX": 1.6, "DEFAULT": 0.95 },
    { "NAME": "dragonCoil",  "LABEL": "Dragon Coil",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "fogDensity",  "LABEL": "Fog Density",   "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.85 },
    { "NAME": "ridgeAmount", "LABEL": "Ridge Amount",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7 },
    { "NAME": "audioReact",  "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "boneColor",   "LABEL": "Bone Sky",      "TYPE": "color", "DEFAULT": [1.0, 0.97, 0.88, 1.0] },
    { "NAME": "sumiColor",   "LABEL": "Sumi Ink",      "TYPE": "color", "DEFAULT": [0.02, 0.018, 0.025, 1.0] },
    { "NAME": "sunColor",    "LABEL": "Sun Disk",      "TYPE": "color", "DEFAULT": [1.0, 0.85, 0.42, 1.0] },
    { "NAME": "goldColor",   "LABEL": "Gold Leaf",     "TYPE": "color", "DEFAULT": [1.0, 0.78, 0.18, 1.0] },
    { "NAME": "lakeColor",   "LABEL": "Lake Wash",     "TYPE": "color", "DEFAULT": [0.18, 0.32, 0.45, 1.0] },
    { "NAME": "sealColor",   "LABEL": "Seal Red",      "TYPE": "color", "DEFAULT": [0.85, 0.12, 0.10, 1.0] }
  ]
}*/

// ═══════════════════════════════════════════════════════════════════════
//   INKWASH DRAGON
//   Sumi-e composition: three layered ridges (back to front), HDR sun
//   disk in the upper-right, calligraphic dragon spine threading from
//   off-screen-left into the middle distance. Gold eye, vermillion seal.
//   Output: LINEAR HDR.
// ═══════════════════════════════════════════════════════════════════════

const float PI = 3.14159265359;

// ─── hash + 2D value noise ─────────────────────────────────────────────
float hash21(vec2 p) {
    p = fract(p * vec2(91.345, 47.853));
    p += dot(p, p + 23.45);
    return fract(p.x * p.y);
}
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p = p * 2.05 + vec2(13.4, 7.1);
        a *= 0.5;
    }
    return v;
}

// ─── distance to capsule (rounded line segment) ───────────────────────
float sdCapsule(vec2 p, vec2 a, vec2 b, float r) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

// ─── dragon spine: parametric path, thickness varies along t ──────────
//   t=0 tail, t=1 head. Returns ink density 0..1 and writes head/tail
//   positions for eye placement.
float dragonInk(vec2 uv, float scale, float coilAmt, float audioMod,
                out vec2 headPos, out vec2 dirHead) {
    const int N = 18;
    float ink = 0.0;
    vec2 prev = vec2(-1.6, -0.18) * scale;
    headPos = prev;
    dirHead = vec2(1.0, 0.0);
    for (int i = 1; i <= N; i++) {
        float t  = float(i) / float(N);
        // Coiling sinuous path — three controlled humps + audio-driven sway.
        float x  = mix(-1.6, 0.65, t);
        float coil = sin(t * 7.6 + TIME * 0.5 * coilAmt + audioMod * 1.5) * 0.36
                   + sin(t * 3.1 + TIME * 0.31) * 0.22;
        float y  = -0.05 + coil * (0.55 - 0.45 * smoothstep(0.0, 0.85, t));
        // Lift the head slightly above the upper ridge.
        if (t > 0.82) y += (t - 0.82) * 1.6;
        vec2 cur = vec2(x, y) * scale;
        // Body width tapers thick→thin from head end.
        float thick = mix(0.030, 0.018,
                          pow(abs(t - 0.55) * 2.0, 1.5))
                    * (0.85 + 0.25 * sin(t * 12.0 + TIME * 0.4));
        if (t < 0.06)  thick *= t / 0.06;            // tail point
        if (t > 0.95)  thick *= (1.0 - (t - 0.95) / 0.05) * 1.4 + 0.6 * (t - 0.95) / 0.05; // jowl
        float d = sdCapsule(uv, prev, cur, thick);
        // Rough brush edge: ink density falls off softly with grain.
        float grain = 0.6 + 0.4 * vnoise(uv * 24.0 + float(i));
        ink = max(ink, smoothstep(0.005, -0.012, d) * grain);
        // Spine markings: small back-fin spikes at every 3rd segment.
        if (mod(float(i), 3.0) < 0.5 && t > 0.15 && t < 0.92) {
            vec2 mid  = (prev + cur) * 0.5;
            vec2 bd   = normalize(cur - prev);
            vec2 nrm  = vec2(-bd.y, bd.x);
            vec2 tip  = mid + nrm * (thick * 2.4 + 0.012 * sin(TIME * 1.5 + t * 8.0));
            float fd  = sdCapsule(uv, mid, tip, thick * 0.55);
            ink = max(ink, smoothstep(0.004, -0.008, fd) * grain * 0.85);
        }
        if (i == N) {
            headPos = cur;
            dirHead = normalize(cur - prev + vec2(1e-4, 0.0));
        }
        prev = cur;
    }
    return clamp(ink, 0.0, 1.0);
}

// ─── ridge silhouette: thresholded fbm gives a soft mountain edge ─────
float ridge(vec2 uv, float baseY, float amp, float freq, float seed) {
    float h = fbm(vec2(uv.x * freq + seed, seed * 1.7)) * amp;
    return smoothstep(0.0, -0.04, uv.y - (baseY + h));
}

void main() {
    vec2 res = RENDERSIZE;
    vec2 uv  = isf_FragNormCoord.xy * 2.0 - 1.0;
    uv.x *= res.x / res.y;

    float audio = clamp(audioReact, 0.0, 2.0);
    float bass  = audioBass;
    float mid   = audioMid;
    float high  = audioHigh;

    // ── 1) Sky — bone-white wash with subtle warm gradient ──────────
    float skyGrad = smoothstep(-1.0, 1.0, uv.y);
    vec3 col = mix(boneColor.rgb * 0.92, boneColor.rgb * 1.05, skyGrad);

    // Painterly fog band — soft horizontal stripes of warm haze.
    float fog = fbm(uv * vec2(2.5, 0.9) + vec2(TIME * 0.04, 0.0));
    col = mix(col, boneColor.rgb * 1.18, fog * fogDensity * 0.45);

    // ── 2) HDR sun disk — calligraphic single bright circle ─────────
    vec2 sunP = vec2(sunPosX, sunPosY);
    float sd  = length(uv - sunP);
    float core = exp(-pow(sd / sunSize, 2.0));
    float halo = exp(-pow(sd / (sunSize * 3.5), 2.0));
    float trebleBoost = 1.0 + 0.6 * high * audio;
    col += sunColor.rgb * (core * 3.0 * sunIntensity * trebleBoost
                         + halo * 0.55 * sunIntensity);
    // Edge crispness — calligraphic ink ring around sun (very thin).
    float ring = 1.0 - smoothstep(0.002, 0.008, abs(sd - sunSize * 1.05));
    col = mix(col, sumiColor.rgb, ring * 0.55);

    // ── 3) Three layered mountain ridges (back to front) ───────────
    // Bass tremor displaces ridges vertically.
    float tremor = bass * audio * 0.04;
    // Back ridge — far, low contrast, lake-blue tint.
    float r0 = ridge(uv, -0.05 + tremor * 0.6, 0.18 * ridgeAmount, 1.6, 3.7);
    col = mix(col, lakeColor.rgb * 1.1, r0 * 0.55);
    // Middle ridge — medium contrast.
    float r1 = ridge(uv, -0.25 + tremor, 0.22 * ridgeAmount, 2.2, 9.1);
    col = mix(col, mix(lakeColor.rgb * 0.55, sumiColor.rgb, 0.6), r1 * 0.85);
    // Front ridge — near silhouette in deep ink.
    float r2 = ridge(uv, -0.45 + tremor * 1.3, 0.28 * ridgeAmount, 3.1, 14.3);
    col = mix(col, sumiColor.rgb, r2 * 0.96);

    // Wet-edge bleed at ridge tops — bone-white halo immediately above.
    float bleed = smoothstep(0.0, 0.04, uv.y - (-0.45));
    col = mix(col, col * 1.08, (1.0 - bleed) * 0.0); // (kept as no-op marker)

    // Reflected sun in the lake plane (below front ridge).
    if (uv.y < -0.45 + tremor * 1.3 - 0.02) {
        float refl = exp(-pow((uv.x - sunP.x) / (sunSize * 1.6), 2.0))
                   * exp(-pow((uv.y + 0.45 - (-(sunP.y - (-0.45)))) /
                              (sunSize * 0.4), 2.0));
        // Approximate vertical mirror about y = -0.45 horizon.
        float mirrorY = -0.9 - sunP.y;
        refl = exp(-pow((uv.x - sunP.x) / (sunSize * 1.6), 2.0))
             * exp(-pow((uv.y - mirrorY) / (sunSize * 0.5), 2.0));
        col += sunColor.rgb * refl * 1.6 * sunIntensity;
    }

    // ── 4) Dragon — calligraphic ink spine ──────────────────────────
    vec2 head; vec2 hdir;
    float ink = dragonInk(uv, dragonScale, dragonCoil,
                          mid * audio * dragonCoil, head, hdir);
    col = mix(col, sumiColor.rgb, ink * 0.97);

    // Gold-leaf eye on the head — small HDR pinpoint.
    vec2 eyeOff = vec2(-hdir.y, hdir.x) * 0.018 + hdir * 0.012;
    vec2 eyeP   = head + eyeOff;
    float ed   = length(uv - eyeP);
    float eyeCore = exp(-pow(ed / 0.011, 2.0));
    float eyeHalo = exp(-pow(ed / 0.04, 2.0));
    col += goldColor.rgb * (eyeCore * 3.4 + eyeHalo * 0.4);

    // ── 5) Ink dot accents — claw marks and brush spatter ──────────
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        vec2 sp = vec2(0.7 - fi * 0.08, -0.25 - 0.09 * fi);
        sp.x += 0.04 * sin(TIME * 0.3 + fi);
        float sd2 = length(uv - sp) - 0.012;
        col = mix(col, sumiColor.rgb,
                  smoothstep(0.0, -0.006, sd2) * 0.7);
    }

    // ── 6) Vermillion seal — bottom-right red square with white kanji-ish notches ──
    vec2 sealP = vec2(0.78, -0.78);
    vec2 sd3   = abs(uv - sealP) - vec2(0.07, 0.07);
    float sBox = max(sd3.x, sd3.y);
    if (sBox < 0.0) {
        col = sealColor.rgb * 1.4;
        // Fake kanji: 4 small white slashes inside.
        for (int i = 0; i < 4; i++) {
            float fi = float(i);
            vec2 lp = sealP + vec2(-0.04 + (mod(fi, 2.0)) * 0.08,
                                   -0.04 + floor(fi * 0.5) * 0.08);
            float ld = length(uv - lp) - 0.012;
            col = mix(col, boneColor.rgb,
                      smoothstep(0.0, -0.005, ld) * 0.9);
        }
    }
    // Seal rim ink.
    float rim = 1.0 - smoothstep(0.001, 0.0035, abs(sBox + 0.005));
    col = mix(col, sumiColor.rgb, rim * 0.7);

    // ── 7) Brush-stroke noise overlay (very subtle paper texture) ───
    float paper = vnoise(uv * 320.0) * 0.04;
    col -= paper * 0.4;

    // ── 8) Vignette + exposure ─────────────────────────────────────
    float vig = 1.0 - 0.22 * dot(uv, uv) * 0.3;
    col *= vig * exposure;

    gl_FragColor = vec4(col, 1.0);
}
