/*{
  "CATEGORIES": ["Generator", "Light", "Audio Reactive"],
  "DESCRIPTION": "Turrell Chroma — pure colored light as material. Slowly evolving Ganzfeld fields cycle through curated triads (Roden Crater Dawn, Skyspace Twilight, Aten Reign Ascension, Ganzfeld Mint, Wedgework Bruise, Notion Motion Aqua, Tate Modern Sunrise, plus the originals). The wedge axis slowly drifts ±30° across the cycle for almost-imperceptible motion. cycleDuration controls pace (20–120s). Output LINEAR HDR.",
  "INPUTS": [
    { "NAME": "mood",           "LABEL": "Mood",            "TYPE": "long",  "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Aten Reign","Wedgework","Ganzfeld","Skyspace"] },
    { "NAME": "cycleDuration",  "LABEL": "Cycle (s)",       "TYPE": "float", "MIN": 20.0, "MAX": 120.0, "DEFAULT": 50.0 },
    { "NAME": "wedgeAngle",     "LABEL": "Wedge Angle",     "TYPE": "float", "MIN": 0.0,  "MAX": 6.2832, "DEFAULT": 1.05 },
    { "NAME": "wedgeStrength",  "LABEL": "Wedge Strength",  "TYPE": "float", "MIN": 0.0,  "MAX": 0.35, "DEFAULT": 0.16 },
    { "NAME": "vignette",       "LABEL": "Edge Falloff",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.42 },
    { "NAME": "luminance",      "LABEL": "Luminance",       "TYPE": "float", "MIN": 0.4,  "MAX": 1.6,  "DEFAULT": 1.0 },
    { "NAME": "audioReact",     "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  Turrell Chroma — Light & Space
//  No objects, no patterns. The CANVAS IS THE LIGHT. A primary hue field
//  drifts between curated triads over a long cycle (default ~50s); a
//  Wedgework gradient gives the architectural-wall illusion; corners fall
//  into the dark of the surrounding room. Audio is a whisper, not a voice.
//  Turrell pieces are about staring at a single color for minutes — the
//  calmness is the point. Transitions sit at the edge of perception.
// ════════════════════════════════════════════════════════════════════════

// Eight curated triads — each a color story we ease through (A → B → C → A …).
// Stored as 8 × 3 = 24 vec3s.
//   0  Aten Reign Ascension      — red → orange → gold
//   1  Wedgework cool            — cobalt → cyan → mint
//   2  Skyspace Twilight         — indigo → magenta → ivory
//   3  Roden Crater Dawn         — cobalt → coral → bone
//   4  Ganzfeld Mint             — sage → teal → cream
//   5  Wedgework Bruise          — purple → violet → rose
//   6  Notion Motion Aqua        — turquoise → cyan → pearl
//   7  Tate Modern Sunrise       — warm gold → amber → ivory
vec3 paletteA(int idx) {
    // 0 Aten Reign Ascension — red → orange → gold
    if (idx == 0) return vec3(0.86, 0.18, 0.16);
    if (idx == 1) return vec3(0.96, 0.46, 0.18);
    if (idx == 2) return vec3(0.98, 0.80, 0.30);
    // 3 Wedgework cool — cobalt → cyan → mint
    if (idx == 3) return vec3(0.10, 0.28, 0.78);
    if (idx == 4) return vec3(0.18, 0.62, 0.86);
    if (idx == 5) return vec3(0.46, 0.86, 0.74);
    // 6 Skyspace Twilight — indigo → magenta → ivory
    if (idx == 6) return vec3(0.16, 0.14, 0.46);
    if (idx == 7) return vec3(0.74, 0.30, 0.62);
    if (idx == 8) return vec3(0.96, 0.92, 0.86);
    // 9 Roden Crater Dawn — cobalt → coral → bone
    if (idx == 9)  return vec3(0.16, 0.30, 0.62);
    if (idx == 10) return vec3(0.94, 0.50, 0.42);
    if (idx == 11) return vec3(0.96, 0.92, 0.82);
    // 12 Ganzfeld Mint — sage → teal → cream
    if (idx == 12) return vec3(0.58, 0.78, 0.62);
    if (idx == 13) return vec3(0.22, 0.66, 0.62);
    if (idx == 14) return vec3(0.96, 0.94, 0.82);
    // 15 Wedgework Bruise — purple → violet → rose
    if (idx == 15) return vec3(0.34, 0.16, 0.52);
    if (idx == 16) return vec3(0.58, 0.28, 0.78);
    if (idx == 17) return vec3(0.92, 0.52, 0.66);
    // 18 Notion Motion Aqua — turquoise → cyan → pearl
    if (idx == 18) return vec3(0.20, 0.74, 0.70);
    if (idx == 19) return vec3(0.30, 0.82, 0.92);
    if (idx == 20) return vec3(0.94, 0.96, 0.94);
    // 21 Tate Modern Sunrise — warm gold → amber → ivory
    if (idx == 21) return vec3(0.94, 0.72, 0.30);
    if (idx == 22) return vec3(0.96, 0.56, 0.22);
    return vec3(0.98, 0.94, 0.84);
}

// Mood routing — which triads does each mood draw from, and in what order?
// Each mood has a 6-slot ring (two triads chained). Ganzfeld is a curated
// random walk through all 8 triads (one color per triad) for max variety.
int moodRing(int mood, int slot) {
    // Aten Reign — Aten Reign Ascension + Tate Modern Sunrise (warm)
    if (mood == 0) {
        if (slot == 0) return 0;  if (slot == 1) return 1;  if (slot == 2) return 2;
        if (slot == 3) return 21; if (slot == 4) return 22; return 23;
    }
    // Wedgework — cool wedgework + bruise wedgework
    if (mood == 1) {
        if (slot == 0) return 3;  if (slot == 1) return 4;  if (slot == 2) return 5;
        if (slot == 3) return 15; if (slot == 4) return 16; return 17;
    }
    // Ganzfeld — random walk: one peak from each of 6 different triads
    if (mood == 2) {
        if (slot == 0) return 1;  if (slot == 1) return 4;  if (slot == 2) return 7;
        if (slot == 3) return 10; if (slot == 4) return 13; return 19;
    }
    // Skyspace — Skyspace Twilight + Roden Crater Dawn (dwell longer)
    if (slot == 0) return 6;  if (slot == 1) return 7;  if (slot == 2) return 8;
    if (slot == 3) return 9;  if (slot == 4) return 10; return 11;
}

// Smooth ease: smootherstep — Ken Perlin's quintic. Slower at endpoints
// than smoothstep, which matters because Turrell transitions live in
// the endpoints. Combined with a long cycle this becomes invisible.
float smoother(float x) {
    x = clamp(x, 0.0, 1.0);
    return x * x * x * (x * (x * 6.0 - 15.0) + 10.0);
}

// RGB → HSV → RGB so we can do a saturation lift on bass.
vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)),
                d / (q.x + e), q.x);
}
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// 1D hash for the treble shimmer — keep it minimal; corners only.
float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

void main() {
    vec2 uv  = isf_FragNormCoord.xy;
    vec2 ndc = uv * 2.0 - 1.0;
    float t  = TIME;

    int mood = int(clamp(float(mood), 0.0, 3.0) + 0.5);

    // ── Audio split ────────────────────────────────────────────────────
    // Split the single audioReact scalar into bass/mid/treble bands using
    // three different time-shaped envelopes so the shader behaves musically
    // even from a single input. Each band stays whisper-quiet — Turrell
    // would never tolerate a kick-pumping wall. (Slowed slightly to match
    // the calmer cycle.)
    float a   = clamp(audioReact, 0.0, 2.0);
    float bass = a * (0.55 + 0.45 * sin(t * 0.7));      // slow swell
    float mid  = a * (0.50 + 0.50 * sin(t * 0.36 + 1.3)); // medium
    float treb = a * (0.50 + 0.50 * sin(t * 2.1 + 2.1));  // gentle flicker
    bass = max(bass, 0.0);
    mid  = max(mid,  0.0);
    treb = max(treb, 0.0);

    // ── Cycle position ─────────────────────────────────────────────────
    // cycleDuration is the full ring period (20–120s). Skyspace dwells
    // 1.6× longer per color than the others. Mid subtly modulates the
    // rate (within ±5%, slowed from ±12% — keep it serene).
    float transitionScale = (mood == 3) ? 1.6 : 1.0;
    float period = max(cycleDuration, 20.0) * transitionScale;
    float rate   = 1.0 / period * (1.0 + 0.05 * (mid - 0.5));
    float phase  = t * rate;                      // continuous
    float ringF  = phase * 6.0;                   // 6 slots per full ring
    float slotF  = floor(ringF);
    float frac   = smoother(ringF - slotF);       // ease this slot
    int   sA     = int(mod(slotF,       6.0));
    int   sB     = int(mod(slotF + 1.0, 6.0));

    int   idxA   = moodRing(mood, sA);
    int   idxB   = moodRing(mood, sB);
    vec3  cA     = paletteA(idxA);
    vec3  cB     = paletteA(idxB);

    // Primary Ganzfeld field — eased mix between the two curated hues.
    vec3 field = mix(cA, cB, frac);

    // ── Wedgework gradient wash ────────────────────────────────────────
    // A direction across the canvas (wedgeAngle radians from +x), with a
    // slow drift of ±30° (≈0.524 rad) over the full cycle so the gradient
    // direction never sits still — but the motion is below the threshold
    // of conscious perception, like a sundial.
    float drift     = 0.5236 * sin(phase * 6.2832); // ±30° over one cycle
    float angleNow  = wedgeAngle + drift;
    vec2  wDir = vec2(cos(angleNow), sin(angleNow));
    float wT   = dot(ndc, wDir);                       // [-~1.4, +~1.4]
    wT         = clamp(0.5 + 0.5 * wT, 0.0, 1.0);      // [0,1]
    // Asymmetric: brighter on one edge, dimmer on the other. Gentle.
    float wedge = mix(1.0 - wedgeStrength, 1.0 + wedgeStrength, wT);
    field *= wedge;

    // Faint chromatic crossfade across the wedge — the bright side leans
    // toward the NEXT color in the ring; the dim side toward the previous.
    // This is the Turrell trick: edges of the field already read as the
    // hue you're transitioning into, so the eye can't fix the moment.
    int   sPrev = int(mod(slotF + 5.0, 6.0));
    vec3  cPrev = paletteA(moodRing(mood, sPrev));
    field = mix(field, mix(cPrev, cB, wT), 0.10);

    // ── Audio: bass saturation lift, treble corner shimmer ─────────────
    // Bass: 5–10% saturation lift (HSV). Barely perceptible.
    vec3 hsv = rgb2hsv(field);
    hsv.y    = clamp(hsv.y + 0.05 + 0.05 * bass, 0.0, 1.0);
    // tiny hue nudge from bass — keeps it alive without "moving"
    hsv.x    = fract(hsv.x + 0.004 * bass);
    field    = hsv2rgb(hsv);

    // Treble: chromatic shimmer ONLY at corners — mist effect. The center
    // stays silent. Slowed (was 6.0 — now 2.4) to match the new pace.
    float r2  = dot(ndc, ndc);                    // 0 center, ~2 corner
    float corner = smoothstep(0.6, 1.6, r2);
    float shimmerN = hash21(floor(uv * RENDERSIZE.xy * 0.5) + floor(t * 2.4));
    vec3  shimmer  = vec3(shimmerN, hash21(uv + 13.7), hash21(uv + 91.1)) - 0.5;
    field += shimmer * corner * treb * 0.018;

    // ── Skyspace sun-arc — only when mood == 3, very subtle ────────────
    // A barely-there warm tint that travels across the wedge axis on a
    // very slow cycle, like the sun moving over a Skyspace oculus.
    if (mood == 3) {
        float arc = 0.5 + 0.5 * sin(t * 0.0084);  // ~12 minute cycle
        float arcMask = exp(-pow((wT - arc) * 2.6, 2.0));
        field += vec3(0.06, 0.04, 0.01) * arcMask;
    }

    // ── Vignette — corners fall into the surrounding room ──────────────
    // Soft, smooth, never crushing. vignette param sets the depth.
    float falloff = 1.0 - vignette * smoothstep(0.2, 1.8, r2);
    field *= falloff;

    // Overall luminance — Light & Space rooms run bright but never harsh.
    field *= luminance;

    // The output is LINEAR HDR; host applies tone mapping.
    gl_FragColor = vec4(field, 1.0);
}
