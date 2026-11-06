/*{
  "DESCRIPTION": "Body Garden — two looks, with presets. Presets: Live (Size Automation, default) — the Pixel Mosaic figure breathes in size from nothing to full (0 → 1.10) over a slow eased ~24 s cycle, nudged by bass and the build, with the contour rings flowing outward at full speed; Static — no automation, the figure held at full size and every input used as set; Mosaic Automation — tile count, contour spacing, tile flicker and contour density each drift slowly on their own eased clocks so the mosaic re-quantises and the aura tightens and loosens by itself; Two Figures — the side-by-side pair, both breathing with the size automation; Custom — every input exactly as set (Figure Size Automation, its min/max/cycle, and People = Auto, where a second figure grows out of the first and slides aside, then merges back). Pixel Mosaic (default): one flat 2D front-view human figure, or two standing side by side, with real anatomy built from a mosaic of coloured pixel tiles — red, yellow, green, blue, white and black squares, split cells, diagonal hatching and little outlined squares that keep flipping and re-arranging digitally — while contour lines made of broken pixel dashes ripple outward from the body across the whole frame like an aura or a fingerprint. At rest the rings drift slowly and the tiles barely change; during a build the rings speed up and tighten and the tiles flicker faster; on the drop a bright shock ring bursts out of the body and the tiles scatter.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original. Figure: MakeHuman base mesh (CC0), baked to a signed distance field (body_sdf.png).",
  "CATEGORIES": [
    "Generator",
    "Nature",
    "Audio Reactive"
  ],
  "INPUTS": [
    {
      "NAME": "presetSel",
      "LABEL": "Preset",
      "TYPE": "long",
      "DEFAULT": 0,
      "VALUES": [
        0,
        1,
        2,
        3,
        4
      ],
      "LABELS": [
        "Live",
        "Static",
        "Mosaic Automation",
        "Two Figures",
        "Custom (use inputs as set)"
      ],
      "GROUP": "Preset"
    },
    {
      "NAME": "peopleSel",
      "LABEL": "People",
      "TYPE": "long",
      "DEFAULT": 0,
      "VALUES": [
        0,
        1,
        2
      ],
      "LABELS": [
        "One Figure",
        "Two Figures Side by Side",
        "Auto (one ↔ two)"
      ],
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "pairPeriod",
      "LABEL": "One ↔ Two Cycle (s)",
      "TYPE": "float",
      "AUDIO": false,
      "MIN": 10.0,
      "MAX": 120.0,
      "DEFAULT": 36.0,
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "pairGap",
      "LABEL": "Pair Spacing",
      "TYPE": "float",
      "MIN": 0.6,
      "MAX": 1.6,
      "DEFAULT": 1.0,
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "merge",
      "LABEL": "Merge (two figures)",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.0,
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "mergeCycle",
      "LABEL": "Merge Cycle (s, 0 = hand)",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 60.0,
      "DEFAULT": 0.0,
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "tileCount",
      "LABEL": "Mosaic Tiles (tall)",
      "TYPE": "float",
      "AUDIO": false,
      "MIN": 16.0,
      "MAX": 90.0,
      "DEFAULT": 36.0,
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "ringSpacing",
      "LABEL": "Contour Spacing",
      "TYPE": "float",
      "MIN": 0.3,
      "MAX": 2.0,
      "DEFAULT": 0.75,
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "ringSpeed",
      "LABEL": "Outward Flow",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 2.0,
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "ringDash",
      "LABEL": "Contour Density",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 1.0,
      "DEFAULT": 0.8,
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "tileFlip",
      "LABEL": "Tile Flicker",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0,
      "GROUP": "Pixel Mosaic"
    },
    {
      "NAME": "figScale",
      "LABEL": "Figure Size",
      "TYPE": "float",
      "AUDIO": false,
      "MIN": 0.6,
      "MAX": 1.8,
      "DEFAULT": 1.0,
      "GROUP": "Figure"
    },
    {
      "NAME": "grain",
      "LABEL": "Grain",
      "TYPE": "float",
      "AUDIO": false,
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.2,
      "GROUP": "Finish"
    },
    {
      "NAME": "paletteMode",
      "LABEL": "Studio Palette",
      "TYPE": "long",
      "DEFAULT": 0,
      "VALUES": [
        0,
        1,
        2,
        3
      ],
      "LABELS": [
        "Colorful",
        "Red · Black · White",
        "Blue · Black · White",
        "Black & White"
      ],
      "GROUP": "Studio Palette"
    },
    {
      "NAME": "palAmount",
      "LABEL": "Palette Amount",
      "TYPE": "float",
      "AUDIO": false,
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 1.0,
      "GROUP": "Studio Palette"
    },
    {
      "NAME": "palRed",
      "LABEL": "Red Accent",
      "TYPE": "color",
      "DEFAULT": [
        0.92,
        0.04,
        0.07,
        1.0
      ],
      "GROUP": "Studio Palette"
    },
    {
      "NAME": "palBlue",
      "LABEL": "Blue Accent",
      "TYPE": "color",
      "DEFAULT": [
        0.07,
        0.32,
        1.0,
        1.0
      ],
      "GROUP": "Studio Palette"
    },
    {
      "NAME": "audioReact",
      "LABEL": "Audio React",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.6,
      "GROUP": "Audio Reactivity"
    },
    {
      "NAME": "showState",
      "LABEL": "Show State",
      "TYPE": "long",
      "DEFAULT": 0,
      "VALUES": [
        0,
        1,
        2,
        3
      ],
      "LABELS": [
        "Follow Music",
        "Rest",
        "Build",
        "Peak"
      ],
      "GROUP": "Show Dynamics"
    },
    {
      "NAME": "dynamics",
      "LABEL": "Build & Drop Depth",
      "TYPE": "float",
      "AUDIO": false,
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.85,
      "GROUP": "Show Dynamics"
    },
    {
      "NAME": "bgMode",
      "LABEL": "Background",
      "TYPE": "long",
      "DEFAULT": 0,
      "VALUES": [
        0,
        1,
        2
      ],
      "LABELS": [
        "Solid",
        "Vertical Gradient",
        "Radial Gradient"
      ],
      "GROUP": "Background"
    },
    {
      "NAME": "bgColor",
      "LABEL": "BG Color / Top / Center",
      "TYPE": "color",
      "DEFAULT": [
        0.0,
        0.0,
        0.0,
        1.0
      ],
      "GROUP": "Background"
    },
    {
      "NAME": "bgColor2",
      "LABEL": "BG Bottom / Edge",
      "TYPE": "color",
      "DEFAULT": [
        0.03,
        0.02,
        0.06,
        1.0
      ],
      "GROUP": "Background"
    },
    {
      "NAME": "transparentBg",
      "LABEL": "Transparent BG",
      "TYPE": "bool",
      "DEFAULT": false,
      "GROUP": "Background"
    }
  ],
  "IMPORTED": {
    "frontTex": {
      "PATH": "body_front.png"
    }
  }
}*/

// ─────────────────────────────────────────────────────────────────────────
// Body Garden — flat 2D pixel-mosaic figure(s) + outward contour rings,
// with presets + automation. Single pass. (3D Glass Garden removed 2026-10-30.)
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec3  h33(vec3 p)  { p = fract(p * vec3(0.1031, 0.1030, 0.0973)); p += dot(p, p.yxz + 33.33); return fract((p.xxy + p.yxx) * p.zyx); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }

vec3 studioPalette(vec3 c) {
    float m = float(paletteMode);
    if (palAmount <= 0.0) return c;
    float L = clamp(dot(c, vec3(0.2126, 0.7152, 0.0722)), 0.0, 1.0);
    float mx = max(c.r, max(c.g, c.b)), mn = min(c.r, min(c.g, c.b));
    float sat = mx > 1e-4 ? (mx - mn) / mx : 0.0;
    vec3 o;
    if (m < 0.5) {
        // Colorful: saturation pushed hard toward neon / primary / poppy colour.
        vec3 hueC = pow((c - mn) / max(mx - mn, 1e-4), vec3(1.4));   // pure hue, nudged toward primaries
        float s2 = 1.0 - pow(1.0 - sat, 4.0);                         // strong but curved saturation boost
        float v = clamp(pow(mx, 0.85) * 1.12, 0.0, 1.0);
        o = mix(vec3(1.0), hueC, s2) * v;
        // flat mid-greys pick up a drifting neon spectrum keyed to brightness
        float hx = L * 1.7 + 0.04 * TIME;
        vec3 neon = pow(clamp(abs(fract(hx + vec3(0.0, 0.6667, 0.3333)) * 6.0 - 3.0) - 1.0, 0.0, 1.0), vec3(1.4));
        float wn = (1.0 - smoothstep(0.02, 0.1, sat)) * smoothstep(0.06, 0.25, L) * (1.0 - smoothstep(0.55, 0.85, L));
        o = mix(o, neon * v, wn);
        o = mix(o, vec3(1.0), smoothstep(0.85, 1.0, L) * 0.5);       // white-hot cores
    } else if (m > 2.5) {
        o = vec3(smoothstep(0.02, 0.95, pow(L, 0.9)));             // Black & White
    } else {
        vec3 acc = m < 1.5 ? palRed.rgb : palBlue.rgb;
        float t = pow(L, 0.8);
        vec3 ramp = t < 0.5 ? mix(vec3(0.0), acc, t * 2.0) : mix(acc, vec3(1.0), (t - 0.5) * 2.0);
        vec3 mono = vec3(smoothstep(0.0, 1.0, L));
        o = mix(mono, ramp, smoothstep(0.06, 0.4, sat));
    }
    return mix(c, o, palAmount);
}


// ── Show dynamics: rest → build → peak / drop ───────────────────────────
float gRest, gBuild, gPeak, gDrop, gLift;
void showDynamics() {
    float m = float(showState);
    float e = clamp(audioEnergy, 0.0, 1.0);
    float b = clamp(audioBuildup, 0.0, 1.0);
    float d = clamp(audioDrop, 0.0, 1.0);
    if (m > 0.5) {                                   // manual override for the VJ
        if (m < 1.5)      { e = 0.04; b = 0.0; d = 0.0; }                                      // Rest
        else if (m < 2.5) { float r = fract(TIME / 8.0); e = 0.3 + 0.35 * r; b = r * r; d = 0.0; } // Build: 8 s riser, loops
        else              { e = 1.0; b = 0.0; d = exp(-fract(TIME / 4.0) * 5.0); } // Peak: hits every 4 s + beats
    }
    float k = dynamics;
    gBuild = b * k;                                                  // tension 0..1
    gDrop  = d * k;                                                  // impact impulse 0..1
    gPeak  = smoothstep(0.5, 0.9, e) * k;                            // sustained high section
    gRest  = (1.0 - smoothstep(0.08, 0.4, e)) * (1.0 - b) * k;       // quiet / breakdown
    gLift  = mix(0.5, clamp(0.15 + e * 0.7 + gBuild * 0.25 + d * 0.3, 0.0, 1.0), k);  // 0 rest … 1 peak; 0.5 when dynamics = 0
}

// ── Presets + automation (all clocks are TIME/constant or integrated, no jumps) ──
float gStyle, gFigA, gPair, gRSpeed, gTiles, gRSpace, gRDash, gFlipClk;
float ease01(float x) { return x * x * (3.0 - 2.0 * x); }
float autoWave(float per, float ph) { return ease01(0.5 - 0.5 * cos(TAU * (TIME / per + ph))); }   // eased 0..1..0
void presetSetup() {
    float pr = float(presetSel);
    float ppl = float(peopleSel);
    float ar = audioReact;
    gStyle = 0.0;                                     // 2D only (Glass Garden 3D removed)
    gRSpeed = ringSpeed; gTiles = tileCount; gRSpace = ringSpacing; gRDash = ringDash;
    gFlipClk = (TIME * 0.8 + audioMidTime * 1.2 * ar) * tileFlip;
    // People (one / two / auto) is always the user's: presets never override it.
    if (pr < 0.5)      { gStyle = 0.0; gRSpeed = 2.0; }                                      // Live
    else if (pr < 1.5) { }                                                                   // Static
    else if (pr < 2.5) {                                                                     // Mosaic Automation
        gStyle = 0.0;
        gTiles  = mix(22.0, 64.0, autoWave(47.0, 0.0));
        gRSpace = mix(0.45, 1.05, autoWave(31.0, 0.37));
        gRDash  = mix(0.65, 1.0, autoWave(23.0, 0.71));
        // flicker rate r(t) = 1.1 + 0.7 sin(w t + 1.9), integrated analytically -> smooth clock
        float w = TAU / 37.0;
        gFlipClk = 0.8 * (1.1 * TIME - 0.7 / w * cos(w * TIME + 1.9)) + audioMidTime * 1.2 * ar * 1.1;
    }
    else if (pr < 3.5) { gStyle = 0.0; if (ppl < 0.5) ppl = 1.0; gRSpeed = 2.0; }   // Two Figures (unless People says auto)
    gFigA = 1.0;                                      // figure size is the Figure Size knob only (no auto scaling)
    // one / two / auto: the second figure grows out of the first and slides aside
    float ph = fract(TIME / max(pairPeriod, 1.0));
    float wAuto = smoothstep(0.38, 0.5, ph) * (1.0 - smoothstep(0.88, 1.0, ph));
    gPair = ppl < 0.5 ? 0.0 : (ppl < 1.5 ? 1.0 : wAuto);
}

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

// ── Pixel Mosaic style: flat 2D front-view figure + outward contour rings ──
// body_front.png: 2D signed distance to the front silhouette (dm), 0.04 dm grid
const vec2  F_LO = vec2(-9.0, -10.5);
const vec2  F_N  = vec2(450.0, 538.0);
const float F_VOX = 0.04;
float frontSD(vec2 p) {
    float flip = texture2D(frontTex, vec2(0.5 / F_N.x, 0.5 / (F_N.y + 1.0))).b > 0.5 ? 1.0 : 0.0;
    vec2 g = (p - F_LO) / F_VOX;
    vec2 gc = clamp(g, vec2(0.0), F_N - 1.0);
    float outside = length(g - gc) * F_VOX;
    float py = 1.0 + (F_N.y - 1.0 - gc.y) + 0.5;
    float v = flip > 0.5 ? py / (F_N.y + 1.0) : 1.0 - py / (F_N.y + 1.0);
    vec4 t = texture2D(frontTex, vec2((gc.x + 0.5) / F_N.x, v));
    return (t.r * 255.0 * 256.0 + t.g * 255.0) / 65535.0 * 15.0 - 1.5 + outside;
}
// one or two figures standing side by side, centred; the second is mirrored.
// gFigA scales the figure(s) about the centre (0 = vanished), gPair 0..1 blends
// one -> two: the second figure grows out of the first and both slide apart.
// Merge (two figures): the pair slides together and melts (smooth union)
// into one hybrid body; Merge Cycle automates it. gOff = each figure's
// distance from the centre.
float gOff = 0.0;
float mergeAmt() {
    float m = merge;
    if (mergeCycle > 0.5) m = merge * autoWave(mergeCycle, 0.0);
    return clamp(m, 0.0, 1.0) * gPair;
}
float bodyD(vec2 p) {
    float fa = max(gFigA, 1e-3);
    vec2 q = p / fa;
    float w = gPair;
    float d;
    if (w < 0.001) { d = frontSD(q); gOff = 0.0; }
    else {
        float mg = mergeAmt();
        float k = mix(1.0, 0.8, w * (1.0 - mg));        // a touch smaller so both fit; full size when merged
        float off = 2.55 * pairGap * w * (1.0 - mg);
        gOff = off * fa;
        float dA = frontSD((q - vec2(-off, 0.0)) / k) * k;
        float kb = k * smoothstep(0.0, 0.7, w);
        d = dA;
        if (kb > 0.01) {
            float dB = frontSD(vec2(-(q.x - off), q.y) / kb) * kb;
            d = smin(dA, dB, 0.05 + 1.6 * mg);          // melt into each other as they merge
        }
    }
    return d * fa;
}
vec3 mosaicColour(float k) {
    k = mod(floor(k), 6.0);
    if (k < 0.5) return vec3(0.86, 0.05, 0.06);   // red
    if (k < 1.5) return vec3(0.98, 0.74, 0.1);    // yellow
    if (k < 2.5) return vec3(0.04, 0.5, 0.25);    // green
    if (k < 3.5) return vec3(0.13, 0.3, 0.66);    // blue
    if (k < 4.5) return vec3(0.97, 0.97, 0.95);   // white
    return vec3(0.0);                             // black
}
vec4 mosaicPass() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    float S = 19.4 / figScale;                         // body dm per screen unit
    float punch = 1.0 + 0.05 * gDrop;
    vec2 p = sp * S / punch + vec2(0.0, 0.16);
    float pxDm = S / R.y;                              // dm per screen pixel
    // clocks: continuous flow + digital steps
    float flowT = TIME * 0.35 * gRSpeed + (audioBassTime * 0.35 + audioMidTime * 0.2) * ar * gRSpeed + 1.6 * gBuild * gBuild;
    float stepT = floor(gFlipClk * (1.0 + 2.0 * gLift) + 10.0 * gBuild * gBuild);

    vec3 col = backdrop(uv);
    float alpha = 0.0;

    // ── the figure as a tile mosaic ──
    float cs = 16.7 / floor(gTiles);                // tile size in dm
    vec2 cell = floor(p / cs);
    vec2 l = p / cs - cell;                            // 0..1 inside the tile
    float dC = bodyD((cell + 0.5) * cs);
    // drop: tiles near the edge scatter outward for a moment
    float scatter = gDrop * step(dC, 0.0) * step(-0.9, dC) * step(0.6, h12(cell + 3.0));
    if (dC < 0.08 * cs && scatter < 0.5 && gFigA > 0.02) {
        float hc = h12(cell);
        float epoch = floor(stepT * (0.15 + 0.35 * hc) + hc * 7.0);    // each tile flips on its own clock
        float k1 = h12(cell + epoch * 1.37);
        float k2 = h12(cell * 1.7 + epoch * 0.71 + 4.0);
        float typ = floor(h12(cell * 0.73 + epoch) * 7.0);
        vec3 a = mosaicColour(k1 * 5.0);                     // avoid black as the main colour
        vec3 b = mosaicColour(k2 * 6.0 + 1.0);
        vec3 c = a;
        float ip = pxDm / cs;                                // one pixel in tile units
        if (typ < 1.5) {
            c = a;                                                         // solid
        } else if (typ < 2.5) {                                            // 2x2 split
            vec2 q = floor(l * 2.0);
            c = mosaicColour(h12(cell + q * 3.1 + epoch) * 6.0);
        } else if (typ < 3.5) {                                            // diagonal hatch
            float hl = fract((l.x + l.y) * 3.0);
            c = mix(a, b.r + b.g + b.b > 2.5 ? vec3(0.86, 0.05, 0.06) : vec3(0.0), step(hl, 0.16));
        } else if (typ < 4.5) {                                            // black tile with an outlined square
            vec2 q = abs(l - 0.5);
            float sq = step(max(q.x, q.y), 0.28) * step(0.28 - 1.6 * ip, max(q.x, q.y));
            c = mix(vec3(0.0), a, sq);
        } else if (typ < 5.5) {                                            // colour tile with an inner square
            vec2 q = abs(l - 0.5);
            c = mix(a, b, step(max(q.x, q.y), 0.22));
        } else {                                                           // diagonal split
            c = l.x > l.y ? a : b;
        }
        // tile seams: thin black gutters
        vec2 e = min(l, 1.0 - l);
        c *= step(min(1.2 * ip, 0.08), min(e.x, e.y));
        // build: a shimmer sweeps up through the body
        c *= 1.0 + 0.5 * gBuild * step(0.85, fract(p.y * 0.15 - TIME * (1.0 + 3.0 * gBuild)));
        col = c;
        alpha = 1.0;
        // a few 'missing' tiles let the black show through
        if (h12(cell + epoch * 0.37 + 9.0) > 0.93) { col = backdrop(uv); alpha = 0.0; }
    } else {
        // ── contour rings flowing outward ──
        float d = bodyD(p);
        // snap to a coarse pixel grid so lines step like pixel strokes
        float gp = min(max(pxDm * 2.0, 0.05), gRSpace * 0.16);
        vec2 ps = (floor(p / gp) + 0.5) * gp;
        float ds = bodyD(ps);
        float sp2 = gRSpace * (1.0 - 0.3 * gBuild);
        float rp = ds / sp2 - flowT;
        float ri = floor(rp);
        float fr = fract(rp);
        float lw = clamp(1.1 * gp / sp2, 0.02, 0.22);
        float line = step(fr, lw);
        // break each ring into dashes: segments of the pixel grid, varying per ring
        vec2 seg = floor(ps / (gp * (4.0 + 6.0 * h11(ri * 1.3))));
        float on = step(1.0 - gRDash * (0.7 + 0.4 * gLift), h12(seg + ri * 7.13));
        // sparse dots and glyph ticks between rings
        float tick = step(0.985 - 0.01 * gLift, h12(floor(p / gp) + floor(flowT * 2.0)));
        float hseg = h12(seg * 1.7 + ri);
        vec3 lc = hseg < 0.72 ? vec3(1.0) : hseg < 0.82 ? vec3(0.25, 0.45, 1.0) : hseg < 0.92 ? vec3(1.0, 0.8, 0.15) : vec3(0.1, 0.7, 0.35);
        float fade = 1.0 - 0.3 * smoothstep(3.0, 12.0, ds);
        float v = max(line * on, tick * 0.8) * fade * (0.75 + 0.35 * gLift + 0.25 * audioBeatPulse * ar);
        // drop: a bright shock ring bursts out of the body
        float shock = exp(-pow((ds - (1.0 - gDrop) * 9.0) / 0.25, 2.0)) * gDrop;
        col = mix(col, lc, clamp(v, 0.0, 1.0));
        col += vec3(1.0) * shock * 1.2;
        alpha = clamp(v + shock, 0.0, 1.0);
        // scattered tiles fly outward on the drop
        if (scatter > 0.5) { col = mosaicColour(h12(cell) * 5.0); alpha = 1.0; }
    }
    col *= 1.0 + 0.12 * audioLevel * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.03;
    col = studioPalette(clamp(col, 0.0, 1.0));
    return vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}

void main() {
    showDynamics();
    presetSetup();
    gl_FragColor = mosaicPass();
}
