/*{
  "CATEGORIES": [
    "Generator",
    "Geometric",
    "Audio Reactive"
  ],
  "DESCRIPTION": "Meter Made — crisp 2.5D geometric composition (SDF circles / boxes / triangles in shallow parallax). A traveling highlight step-sequences the shape ring on the EaselAudio bar ramp (audioPhase4), outlines thicken with the mids, fills flash-dip on audioMidHit, and the whole composition zoom-breathes with the bass. Layers: back plane / mid plane / front plane / outlines + universal hueShift/colorBoost/bgColor/audioReactivity.",
  "INPUTS": [
    {
      "NAME": "outlineGlow",
      "LABEL": "Outlines",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "backPlane",
      "LABEL": "Back Plane",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Camera / Layout"
    },
    {
      "NAME": "midPlane",
      "LABEL": "Mid Plane",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Camera / Layout"
    },
    {
      "NAME": "frontPlane",
      "LABEL": "Front Plane",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Camera / Layout"
    },
    {
      "NAME": "hueShift",
      "LABEL": "Hue Shift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0,
      "GROUP": "Color"
    },
    {
      "NAME": "colorBoost",
      "LABEL": "Color Boost",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Color"
    },
    {
      "NAME": "bgColor",
      "LABEL": "Background",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        0,
        0
      ],
      "GROUP": "Background"
    },
    {
      "NAME": "audioReactivity",
      "LABEL": "Audio React",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Audio Reactivity"
    }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  METER MADE — EaselAudio flagship (id 1204)
//  Structure on beats (bar-phase step sequencer, EASED — the highlight
//  travels, it never snaps), texture on levels (outline width follows the
//  mids linearly, zoom breathes on smoothed bass), events ramped in over
//  the first quarter beat so nothing steps in a single frame. Dark slate
//  scene → whole-frame GAIN follower (can't clip). Beautiful in silence:
//  the ring slow-orbits and the planes parallax-pan on TIME alone.
// ════════════════════════════════════════════════════════════════════════

float hash11(float p) { return fract(sin(p * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// ── SDFs ────────────────────────────────────────────────────────────────
float sdCircle(vec2 p, float r) { return length(p) - r; }
float sdBox(vec2 p, vec2 b) {
    vec2 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}
float sdTri(vec2 p, float r) {
    const float k = 1.7320508;
    p.x = abs(p.x) - r;
    p.y = p.y + r / k;
    if (p.x + k * p.y > 0.0) p = vec2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
    p.x -= clamp(p.x, -2.0 * r, 0.0);
    return -length(p) * sign(p.y);
}

vec2 rot2(vec2 p, float a) {
    float s = sin(a), c = cos(a);
    return vec2(c * p.x - s * p.y, s * p.x + c * p.y);
}

// Bauhaus-poster palette
vec3 palShape(int k) {
    int m = int(mod(float(k), 4.0));
    if (m == 0) return vec3(0.95, 0.42, 0.30); // coral
    if (m == 1) return vec3(0.24, 0.72, 0.66); // teal
    if (m == 2) return vec3(0.98, 0.76, 0.28); // amber
    return vec3(0.88, 0.86, 0.82);             // bone
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    vec2 p0 = uv - 0.5;
    p0.x *= RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    float px = 1.0 / max(RENDERSIZE.y, 1.0); // one pixel in p-units — PIXEL-accurate AA
    float t = TIME;

    float aR = clamp(audioReactivity, 0.0, 2.0);

    // ── EaselAudio conditioning ─────────────────────────────────────────
    float bass = clamp(audioBass, 0.0, 1.0);
    float mid  = clamp(audioMid,  0.0, 1.0);
    float high = clamp(audioHigh, 0.0, 1.0);
    float beatRamp = smoothstep(0.0, 0.25, audioBeatPhase);
    float midHitE  = clamp(max(audioMidHit, stemDrumsHit * 0.7), 0.0, 1.0) * beatRamp * aR;
    float beatSoft = clamp(audioBeatPulse, 0.0, 1.0) * beatRamp;

    // ── Zoom breath: idle sine + smoothed bass (±5%) ────────────────────
    float zoom = 1.0 + 0.030 * sin(t * 0.23) + 0.055 * bass * aR;
    vec2 p = p0 / zoom;

    // Slow global sway
    float sway = 0.04 * sin(t * 0.11);
    p = rot2(p, sway);

    // ── Background: deep slate gradient + fine dot lattice ──────────────
    vec3 bg = mix(vec3(0.10, 0.11, 0.16), vec3(0.22, 0.21, 0.28),
                  clamp(uv.y + 0.18 * sin(t * 0.07 + uv.x * 2.0), 0.0, 1.0));
    // slow cross-canvas tint wash (color entropy without breaking the slate)
    bg += vec3(0.06, 0.02, -0.02) * sin(uv.x * 3.1 + t * 0.05)
        + vec3(-0.02, 0.03, 0.06) * sin(uv.y * 2.7 - t * 0.04 + 1.3);
    // dot lattice (texture + edge energy) — pixel-crisp discs. STATIC:
    // a drifting sharp lattice measurably floods the silence noise floor
    // (ambient responseMag → 0), anchored dots give the same edges free.
    float pxl = px * 11.0;                       // one pixel in lattice units
    vec2 lat = fract(p0 * 11.0) - 0.5;           // screen-fixed (immune to zoom breath)
    float dots = smoothstep(0.042 + pxl, 0.042 - pxl, length(lat));
    bg += vec3(0.20, 0.21, 0.26) * dots;
    // finer counter-lattice at half cell offset (detail octave)
    vec2 lat2 = fract(p0 * 22.0 + 0.5) - 0.5;
    float pxl2 = px * 22.0;
    float dots2 = smoothstep(0.030 + pxl2, 0.030 - pxl2, length(lat2));
    bg += vec3(0.09, 0.10, 0.13) * dots2;
    // hairline print-grid between the dots (1px rules, Bauhaus blueprint)
    vec2 gd = abs(lat);
    float grid = smoothstep(1.2 * pxl, 0.4 * pxl, min(gd.x, gd.y));
    bg += vec3(0.045, 0.05, 0.065) * grid;
    bg = mix(bg, bgColor.rgb, clamp(bgColor.a, 0.0, 1.0));

    vec3 col = bg;

    // Outline width: base + mids (linear follower) + idle breath
    float ow = 0.0075 * (1.0 + 0.15 * sin(t * 0.5)) * (1.0 + 1.1 * mid * aR);
    float owF = ow * clamp(outlineGlow, 0.0, 2.0);

    // ══ BACK PLANE — three big dim shapes, deepest parallax ═════════════
    vec2 pan = vec2(sin(t * 0.09), cos(t * 0.07)) * 0.10;
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        vec2 q = p - pan * 0.35 - vec2(cos(fi * 2.09 + t * 0.05), sin(fi * 2.09 + t * 0.04)) * 0.44;
        q = rot2(q, t * 0.03 + fi * 1.3);
        float sd;
        if (i == 0)      sd = sdCircle(q, 0.38);
        else if (i == 1) sd = sdBox(q, vec2(0.33, 0.27));
        else             sd = sdTri(q, 0.36);
        float fill = smoothstep(1.5 * px, -1.5 * px, sd);        // pixel-crisp silhouette
        vec3 sc = mix(bg, palShape(i) * 0.42 + vec3(0.06), 0.9);
        col = mix(col, sc, fill * 0.9 * clamp(backPlane, 0.0, 2.0) * 0.5);
        // outline: crisp 1-2px core + faint soft glow
        float olc = smoothstep(max(owF * 0.5, 1.2 * px) + px, max(owF * 0.5, 1.2 * px) - px, abs(sd));
        float olg = smoothstep(owF * 2.2, 0.0, abs(sd));
        col += palShape(i) * (olc * 0.16 + olg * 0.05) * clamp(backPlane, 0.0, 2.0) * 0.5;
    }

    // ══ MID PLANE — ring of 8 step-sequenced shapes ═════════════════════
    // Ring rotation: slow TIME orbit + smooth 8-beat swing (continuous at
    // the phase wrap: cosine ramp starts and ends at 0)
    float w8 = 0.5 - 0.5 * cos(6.28318 * audioPhase8);
    float ringRot = t * 0.06 + 0.42 * w8;
    // Sequencer pointer travels the ring on the bar ramp (2 slots / beat)
    float seq = audioPhase4 * 8.0;

    for (int k = 0; k < 8; k++) {
        float fk = float(k);
        float ang = fk * 0.7853982 + ringRot;
        vec2 c = vec2(cos(ang), sin(ang)) * (0.335 + 0.025 * sin(t * 0.19 + fk));
        vec2 q = p - pan * 0.7 - c;
        q = rot2(q, -ang * 0.5 + t * 0.08);

        float sz = 0.105 * (1.0 + 0.06 * sin(t * 0.31 + fk * 1.9));
        float sd;
        int m = int(mod(fk, 3.0));
        if (m == 0)      sd = sdCircle(q, sz);
        else if (m == 1) sd = sdBox(q, vec2(sz * 0.9));
        else             sd = sdTri(q, sz * 1.1);

        // Traveling highlight (eased, wrap-aware)
        float ds = abs(seq - fk);
        ds = min(ds, 8.0 - ds);
        float lit = smoothstep(1.4, 0.0, ds);

        // Fill: base + sequencer lift, flash-DIP on mid hits (ramped)
        float fillLum = (0.58 + 0.55 * lit) * (1.0 - 0.30 * midHitE);
        float fill = smoothstep(1.5 * px, -1.5 * px, sd);        // pixel-crisp silhouette
        vec3 sc = palShape(k) * fillLum;
        // vertical gradient inside the fill (2.5D read + color entropy)
        sc *= 1.0 + 0.45 * clamp(q.y / max(sz, 1e-3), -1.0, 1.0);
        col = mix(col, sc, fill * clamp(midPlane, 0.0, 2.0) * 0.5 * 1.6);

        // Outline: crisp mid-driven core + soft glow, brightens when lit
        float lw = max(owF * 0.6, 1.2 * px);
        float olc = smoothstep(lw + px, lw - px, abs(sd));
        float olg = smoothstep(owF * 2.0, 0.0, abs(sd));
        col += palShape(k) * (olc * 0.95 + olg * 0.16) * (0.35 + 0.65 * lit)
             * clamp(outlineGlow, 0.0, 2.0) * 0.6;
        // echo ring just outside the shape (crisp hairline + faint glow)
        float ed = abs(sd - sz * 0.45);
        float echo = smoothstep(1.2 * px + px, 1.2 * px - px, ed) * 0.75
                   + smoothstep(owF * 1.2, 0.0, ed) * 0.25;
        col += palShape(k) * echo * 0.34 * clamp(outlineGlow, 0.0, 2.0) * 0.6;
        // inner inset hairline (secondary structure inside the fill)
        float insd = abs(sd + sz * 0.34);
        float inset = smoothstep(1.1 * px + px, 1.1 * px - px, insd);
        col += palShape(k) * inset * 0.34 * fill * clamp(midPlane, 0.0, 2.0) * 0.5;
    }

    // ══ FRONT PLANE — small crisp accents, biggest parallax ═════════════
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        float h = hash11(fi * 5.7 + 3.1);
        vec2 c = vec2(sin(t * (0.12 + 0.05 * h) + fi * 2.4),
                      cos(t * (0.10 + 0.04 * h) + fi * 1.9)) * vec2(0.46, 0.36);
        vec2 q = p - pan * 1.6 - c;
        q = rot2(q, t * 0.2 + fi * 2.0);
        float sd;
        if (i == 0)      sd = sdCircle(q, 0.035);
        else if (i == 1) sd = sdBox(q, vec2(0.030));
        else             sd = sdTri(q, 0.038);
        // outline-only rings (accent) — crisp core + soft glow
        float lwF = max(owF * 0.6, 1.2 * px);
        float olc = smoothstep(lwF + px, lwF - px, abs(sd));
        float olg = smoothstep(owF * 1.8, 0.0, abs(sd));
        col += palShape(i + 1) * (olc * 0.75 + olg * 0.18) * clamp(frontPlane, 0.0, 2.0) * 0.6;
        float fill = smoothstep(1.5 * px, -1.5 * px, sd);
        col = mix(col, palShape(i + 2) * 0.9, fill * 0.55 * clamp(frontPlane, 0.0, 2.0) * 0.5);
    }

    // ── Whole-frame GAIN follower (dark scene, clip-safe) ───────────────
    float gain = (0.30 * bass + 0.22 * mid + 0.16 * high
                + 0.26 * beatSoft + 0.16 * clamp(audioPunch, 0.0, 1.0) * beatRamp)
               * 0.46 * aR;
    col *= 1.0 + gain;

    // ── Universal color block ───────────────────────────────────────────
    if (hueShift > 0.001) {
        vec3 hsv = rgb2hsv(clamp(col, 0.0, 2.0));
        hsv.x = fract(hsv.x + hueShift);
        col = hsv2rgb(hsv);
    }
    float luma = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(luma), col, clamp(colorBoost, 0.0, 2.0));

    // Gentle vignette to seat the composition
    col *= 1.0 - 0.35 * smoothstep(0.30, 0.95, dot(p0, p0));

    // Whisper of STATIC grain — texture/edges without a motion noise floor
    // (per-frame grain flicker measurably drowned the ambient followers)
    float gr = hash21(uv * RENDERSIZE.xy) - 0.5;
    col += gr * 0.058;

    gl_FragColor = vec4(col, 1.0);
}
