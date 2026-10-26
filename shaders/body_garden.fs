/*{
  "DESCRIPTION": "Body Garden — two looks. Pixel Mosaic (default): one flat 2D front-view human figure, or two standing side by side, with real anatomy built from a mosaic of coloured pixel tiles — red, yellow, green, blue, white and black squares, split cells, diagonal hatching and little outlined squares that keep flipping and re-arranging digitally — while contour lines made of broken pixel dashes ripple outward from the body across the whole frame like an aura or a fingerprint. At rest the rings drift slowly and the tiles barely change; during a build the rings speed up and tighten and the tiles flicker faster; on the drop a bright shock ring bursts out of the body and the tiles scatter. Glass Garden: a still glass human figure with real anatomy (a sculpted human body baked into a 3D distance field), seen three-quarter from head to mid-thigh, with a living garden inside. The glass shell glows at its rim and catches highlights while the interior is raymarched as a volume: lungs breathe and sparkle with thousands of multicoloured points of light, a glowing pink nerve tree branches from the throat into the chest and pulses on the beat, vines spiral up through the torso and neck, flowers open and close in the head, shoulder and chest, a dense bed of blooms and leaves fills the belly, a cluster of sparks lives in the brain, and abstract 3D elements (rings, cubes and orbs) drift through the body. The figure never moves; everything inside it does. Bass makes the lungs breathe deeper, beats send a pulse up the nerves and flash the sparks, mids speed the drifting elements, highs twinkle the lights. Show Dynamics: at Rest the garden sleeps (buds closed, vines low, a few slow sparks and drifting shapes, soft glass); during a Build the vines climb, flowers unfurl, the sparks thicken and shimmer faster while the abstract shapes converge on the heart and tremble; at the Drop a shockwave of light bursts out from the chest through the whole body, the nerves and glass flash, and every bloom snaps fully open.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original. Figure: MakeHuman base mesh (CC0), baked to a signed distance field (body_sdf.png).",
  "CATEGORIES": ["Generator", "3D", "Nature", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "styleSel",    "LABEL": "Style", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["Pixel Mosaic (flat 2D)", "Glass Garden (3D)"], "GROUP": "Figure" },
    { "NAME": "peopleSel",   "LABEL": "People", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["One Figure", "Two Figures Side by Side"], "GROUP": "Pixel Mosaic" },
    { "NAME": "pairGap",     "LABEL": "Pair Spacing",     "TYPE": "float", "MIN": 0.6, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Pixel Mosaic" },
    { "NAME": "tileCount",   "LABEL": "Mosaic Tiles (tall)", "TYPE": "float", "AUDIO": false, "MIN": 16.0, "MAX": 90.0, "DEFAULT": 36.0, "GROUP": "Pixel Mosaic" },
    { "NAME": "ringSpacing", "LABEL": "Contour Spacing",  "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 0.75, "GROUP": "Pixel Mosaic" },
    { "NAME": "ringSpeed",   "LABEL": "Outward Flow",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Pixel Mosaic" },
    { "NAME": "ringDash",    "LABEL": "Contour Density",  "TYPE": "float", "MIN": 0.1, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Pixel Mosaic" },
    { "NAME": "tileFlip",    "LABEL": "Tile Flicker",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Pixel Mosaic" },
    { "NAME": "facing",      "LABEL": "Figure Turn",      "TYPE": "float", "AUDIO": false, "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Figure" },
    { "NAME": "figScale",    "LABEL": "Figure Size",      "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.8, "DEFAULT": 1.0, "GROUP": "Figure" },
    { "NAME": "shellGlow",   "LABEL": "Glass Rim",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Figure" },
    { "NAME": "shellTint",   "LABEL": "Glass Colour",     "TYPE": "color", "DEFAULT": [0.35, 0.55, 1.0, 1.0], "GROUP": "Figure" },
    { "NAME": "lungGlow",    "LABEL": "Lung Light",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Inside" },
    { "NAME": "sparkle",     "LABEL": "Sparkles",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Inside" },
    { "NAME": "nerves",      "LABEL": "Nerve Tree",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Inside" },
    { "NAME": "vines",       "LABEL": "Vines",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Inside" },
    { "NAME": "flowers",     "LABEL": "Flowers",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Inside" },
    { "NAME": "bellyGarden", "LABEL": "Belly Garden",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Inside" },
    { "NAME": "elements",    "LABEL": "Abstract Elements","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Inside" },
    { "NAME": "speed",       "LABEL": "Inner Motion",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Inside" },
    { "NAME": "cMagenta",    "LABEL": "Magenta",          "TYPE": "color", "DEFAULT": [1.0, 0.2, 0.75, 1.0], "GROUP": "Colours" },
    { "NAME": "cViolet",     "LABEL": "Violet",           "TYPE": "color", "DEFAULT": [0.55, 0.35, 1.0, 1.0], "GROUP": "Colours" },
    { "NAME": "cCyan",       "LABEL": "Cyan",             "TYPE": "color", "DEFAULT": [0.3, 0.8, 1.0, 1.0],  "GROUP": "Colours" },
    { "NAME": "cGold",       "LABEL": "Gold",             "TYPE": "color", "DEFAULT": [1.0, 0.6, 0.15, 1.0], "GROUP": "Colours" },
    { "NAME": "cLeaf",       "LABEL": "Leaf",             "TYPE": "color", "DEFAULT": [0.35, 0.9, 0.35, 1.0], "GROUP": "Colours" },
    { "NAME": "bloom",       "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.9, "GROUP": "Finish" },
    { "NAME": "renderScale", "LABEL": "Render Scale",     "TYPE": "float", "AUDIO": false, "MIN": 0.35, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.02, 0.06, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "IMPORTED": {
    "bodyTex": { "PATH": "body_sdf.png" },
    "frontTex": { "PATH": "body_front.png" }
  },
  "PASSES": [
    { "TARGET": "fScene", "WIDTH": "floor($WIDTH*$renderScale)", "HEIGHT": "floor($HEIGHT*$renderScale)" },
    { "TARGET": "fBloom", "WIDTH": "$WIDTH/4", "HEIGHT": "$HEIGHT/4" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Body Garden
//   pass 0  fScene: sphere-trace the glass body, then march through its
//           interior accumulating emission from lungs, sparkles, nerves,
//           vines, flowers and abstract elements (alpha = coverage)
//   pass 1  fBloom: quarter-res bright prefilter
//   pass 2  final: bloom, background, Studio Palette, grain
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

float gT, gBreath, gBeat;

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

// ── the figure: a real human body (MakeHuman CC0 base mesh) baked to a
//    3D signed distance field, packed as 16-bit slices in body_sdf.png.
//    Body space is in decimetres: y up, face toward +z, hips near y = 0.7.
const vec3  SDF_LO = vec3(-3.005379, -3.000000, -1.221600);
const vec3  SDF_N  = vec3(102.0, 198.0, 55.0);
const float SDF_VOX = 0.06;
const float SDF_D = 0.8;
const float A_TW = 104.0, A_TH = 57.0, A_COLS = 19.0, A_W = 1976.0, A_H = 628.0;
float gFlip;      // 1.0 when the PNG's top row sits at v = 0

float sdfSlice(vec2 xz, float j) {
    float tx = mod(j, A_COLS), ty = floor(j / A_COLS);
    float px = tx * A_TW + 1.0 + xz.x + 0.5;
    float py = 1.0 + ty * A_TH + 1.0 + xz.y + 0.5;
    float v = gFlip > 0.5 ? py / A_H : 1.0 - py / A_H;
    vec4 t = texture2D(bodyTex, vec2(px / A_W, v));
    return (t.r * 255.0 * 256.0 + t.g * 255.0) / 65535.0;
}

float bodySDF(vec3 p) {
    vec3 g = (p - SDF_LO) / SDF_VOX;
    vec3 gc = clamp(g, vec3(0.0), SDF_N - 1.0);
    float outside = length(g - gc) * SDF_VOX;
    float j0 = floor(gc.y);
    float j1 = min(j0 + 1.0, SDF_N.y - 1.0);
    float a = sdfSlice(gc.xz, j0), b = sdfSlice(gc.xz, j1);
    float d = (mix(a, b, gc.y - j0) * 2.0 - 1.0) * SDF_D;
    d = max(d + outside, -(p.y + 2.7));            // crop at mid-thigh
    return d;
}
float sdEll(vec3 p, vec3 r) { float k0 = length(p / r); float k1 = length(p / (r * r)); return k0 * (k0 - 1.0) / max(k1, 1e-5); }
float sdCap(vec3 p, vec3 a, vec3 b, float r) { vec3 pa = p - a, ba = b - a; float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0); return length(pa - ba * h) - r; }

vec3 bodyNormal(vec3 p) {
    vec2 e = vec2(0.045, -0.045);
    return normalize(e.xyy * bodySDF(p + e.xyy) + e.yyx * bodySDF(p + e.yyx) + e.yxy * bodySDF(p + e.yxy) + e.xxx * bodySDF(p + e.xxx));
}

// ── the garden inside (positions from the mesh skeleton, decimetres) ─────
//    larynx ~ (0, 6.2, 0.8)   carina ~ (0, 4.7, 0.3)   chest centre ~ (0, 4.2, 0.3)
//    brain ~ (0, 7.7, 0.05)   navel ~ (0, 1.6, 0.9)    pelvis ~ (0, 0.7, 0.1)
float lungSDF(vec3 p) {
    vec3 q = vec3(abs(p.x), p.yz);
    float s = gBreath;
    vec3 c = vec3(0.78, 4.1, 0.32);
    float d = sdEll((q - c) / s, vec3(0.6, 1.1, 0.72)) * s;
    // flatten the inner face against the heart
    d = max(d, 0.18 - q.x);
    return d;
}

vec3 sparkles(vec3 p, float scale, float density) {
    vec3 gp = p * scale;
    vec3 gi = floor(gp), gf = fract(gp);
    vec3 h = h33(gi);
    if (h.x > density) return vec3(0.0);
    vec3 c = 0.2 + 0.6 * h33(gi + 7.0);
    float d = length(gf - c);
    float tw = 0.5 + 0.5 * sin(TIME * (2.0 + 5.0 * h.y) + h.z * 40.0);
    float k = h.y * 5.0;
    vec3 col = k < 1.0 ? cMagenta.rgb : (k < 2.0 ? cGold.rgb : (k < 3.0 ? cCyan.rgb : (k < 4.0 ? cLeaf.rgb : vec3(1.0, 0.9, 0.95))));
    float shim = 1.0 + gBuild * 1.6 * (0.5 + 0.5 * sin(70.0 * gBuild * gBuild + h.z * 40.0 + TIME * 3.0));
    return col * exp(-d * 18.0) * (0.4 + 0.8 * tw) * (1.0 + 1.5 * highP() * audioReact + 1.5 * gBeat + 2.0 * gDrop) * shim;
}

// airway + nerve tree: trachea down the throat, bronchi into each lung, and a
// branching glow up into the head
float nerveSDF(vec3 p, out float along) {
    float w = 0.045;
    float wig = 0.05 * sin(p.y * 4.0 + gT);
    vec3 pw = p + vec3(wig, 0.0, 0.0);
    vec3 lar = vec3(0.0, 6.3, 0.75), car = vec3(0.0, 4.75, 0.35);
    float d = sdCap(pw, lar, car, w);
    along = clamp((7.6 - p.y) / 4.5, 0.0, 1.0);
    vec3 q = vec3(abs(pw.x), pw.yz);
    vec3 b1 = vec3(0.5, 4.45, 0.35);
    d = min(d, sdCap(q, car, b1, w * 0.9));
    d = min(d, sdCap(q, b1, vec3(0.75, 3.7, 0.4), w * 0.7));
    d = min(d, sdCap(q, b1, vec3(0.95, 4.6, 0.25), w * 0.6));
    d = min(d, sdCap(q, vec3(0.75, 3.7, 0.4), vec3(0.6, 3.2, 0.55), w * 0.5));
    d = min(d, sdCap(q, vec3(0.75, 3.7, 0.4), vec3(1.05, 3.4, 0.2), w * 0.5));
    // into the head: brain stem and branches
    vec3 st = vec3(0.0, 7.2, 0.15);
    d = min(d, sdCap(pw, lar, st, w * 0.8));
    d = min(d, sdCap(q, st, vec3(0.45, 7.9, -0.1), w * 0.6));
    d = min(d, sdCap(q, st, vec3(0.25, 8.1, 0.55), w * 0.55));
    d = min(d, sdCap(q, vec3(0.45, 7.9, -0.1), vec3(0.55, 8.15, -0.5), w * 0.45));
    // down the spine toward the belly
    d = min(d, sdCap(pw, car, vec3(0.05, 1.3, 0.2), w * 0.6));
    return d;
}

// vines: helices climbing through the torso, neck and head
vec3 vineEmit(vec3 p) {
    vec3 e = vec3(0.0);
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        float R = (p.y > 5.6 ? 0.32 : 0.62 + 0.12 * fi) * (p.y > 6.6 ? 1.5 : 1.0);
        float th = p.y * (1.6 + 0.3 * fi) + fi * 2.1 + gT * 0.25 * (mod(fi, 2.0) * 2.0 - 1.0);
        vec2 c = vec2(cos(th), sin(th)) * R + vec2(0.0, p.y > 6.6 ? 0.1 : 0.25);
        float d = length(p.xz - c);
        float grow = smoothstep(-0.4, 0.2, sin(gT * 0.15 + fi * 2.0) * 1.2 + 1.0 - (p.y + 2.5) * 0.12 + (gLift - 0.5) * 2.2 + gBuild * 0.6);
        float stem = exp(-d * 30.0) * grow;
        float lf = fract(p.y * 2.6 + fi * 0.37);
        float leaf = exp(-length(vec2(d - 0.07, (lf - 0.5) * 0.38)) * 22.0) * step(0.45, h11(floor(p.y * 2.6 + fi * 0.37) + fi * 9.0)) * grow;
        e += mix(cGold.rgb, cLeaf.rgb, 0.5 + 0.5 * sin(fi * 3.0 + p.y * 0.6)) * (stem * 0.8 + leaf * 0.6);
    }
    return e;
}

// flowers: glowing blooms opening and closing
vec3 flowerEmit(vec3 p) {
    vec3 e = vec3(0.0);
    for (int i = 0; i < 7; i++) {
        float fi = float(i);
        vec3 c = i == 0 ? vec3(0.3, 7.75, 0.35) : i == 1 ? vec3(-0.35, 7.35, 0.55) : i == 2 ? vec3(1.45, 4.95, 0.15)
               : i == 3 ? vec3(-0.65, 3.0, 0.65) : i == 4 ? vec3(0.45, 1.8, 0.7) : i == 5 ? vec3(-1.4, 4.8, 0.1) : vec3(0.0, 5.6, 0.45);
        vec3 q = p - c;
        q.xy = rot(fi * 1.3 + gT * 0.1) * q.xy;
        q.yz = rot(0.6 + fi) * q.yz;
        float r = length(q.xy);
        float a = atan(q.y, q.x);
        float N = 5.0 + mod(fi, 3.0);
        float open = (0.7 + 0.3 * sin(gT * 0.5 + fi * 1.7)) * mix(0.45, 1.15, gLift) + 0.35 * gDrop;
        float petal = 0.32 * open * (0.55 + 0.45 * abs(cos(a * N * 0.5)));
        float thick = abs(q.z + 0.5 * r * r / max(open, 0.3)) - 0.015;
        float dP = max(r - petal, thick);
        float core = length(q) - 0.06;
        vec3 col = i == 0 ? cMagenta.rgb : i == 1 ? cViolet.rgb : i == 2 ? cMagenta.rgb : i == 3 ? cGold.rgb : i == 4 ? cCyan.rgb : i == 5 ? cViolet.rgb : cMagenta.rgb;
        e += col * exp(-max(dP, 0.0) * 18.0) * 0.7 * (0.55 + 0.6 * gLift) + mix(col, vec3(1.0), 0.6) * exp(-max(core, 0.0) * 25.0) * 1.2;
    }
    return e;
}

// abstract elements drifting through the body
vec3 elementEmit(vec3 p) {
    vec3 e = vec3(0.0);
    float sp = gT * 0.6 + 0.6 * audioMidTime * audioReact * speed + 2.5 * gBuild * gBuild;
    float nEl = mix(2.5, 8.0, smoothstep(0.1, 0.75, gLift));
    float conv = 0.55 * gBuild;                                  // converge on the heart
    float trem = smoothstep(0.65, 1.0, gBuild) * 0.06;
    for (int i = 0; i < 8; i++) {
        float fi = float(i);
        float on = clamp(nEl - fi, 0.0, 1.0);
        if (on <= 0.0) break;
        float hy = mix(-1.5, 7.8, h11(fi * 3.3));
        float rr = hy > 6.0 ? 0.35 : (hy > 5.4 ? 0.2 : 0.75);
        vec3 c = vec3(rr * cos(sp * (0.3 + 0.1 * fi) + fi * 2.0), hy + 0.25 * sin(sp * 0.5 + fi), 0.2 + rr * 0.5 * sin(sp * (0.3 + 0.1 * fi) + fi * 2.0));
        c = mix(c, vec3(c.x * 0.4, mix(c.y, 4.3, 0.6), 0.3), conv);
        c += trem * vec3(sin(TIME * 41.0 + fi * 3.0), sin(TIME * 37.0 + fi * 5.0), sin(TIME * 45.0 + fi));
        vec3 q = p - c;
        q.xz = rot(sp * 0.7 + fi) * q.xz;
        q.xy = rot(sp * 0.5 + fi * 2.0) * q.xy;
        float k = mod(fi, 3.0);
        float d;
        if (k < 0.5) { vec2 tq = vec2(length(q.xz) - 0.2, q.y); d = length(tq) - 0.025; }
        else if (k < 1.5) { vec3 bq = abs(q) - vec3(0.12); d = abs(length(max(bq, 0.0)) + min(max(bq.x, max(bq.y, bq.z)), 0.0)) - 0.01; }
        else d = abs(length(q) - 0.14) - 0.012;
        vec3 col = k < 0.5 ? cCyan.rgb : (k < 1.5 ? cGold.rgb : cViolet.rgb);
        e += col * exp(-max(d, 0.0) * 40.0) * 0.9 * on * (0.7 + 0.5 * gLift + 0.6 * gBuild);
    }
    return e;
}

// a packed bed of blooms and leaves in the abdomen
vec3 bellyEmit(vec3 p) {
    float reg = smoothstep(2.9, 2.3, p.y) * smoothstep(-1.6, -0.8, p.y);
    if (reg < 0.01) return vec3(0.0);
    vec3 gp = p * 3.0 + vec3(0.0, gT * 0.05, 0.0);
    vec3 gi = floor(gp), gf = fract(gp);
    vec3 h = h33(gi + 11.0);
    vec3 c = 0.25 + 0.5 * h33(gi + 2.0);
    vec3 d3 = gf - c;
    float r = length(d3);
    float k = h.x * 6.0;
    vec3 col = k < 1.5 ? cMagenta.rgb : (k < 2.5 ? cViolet.rgb : (k < 3.5 ? cGold.rgb : (k < 4.5 ? vec3(1.0, 0.55, 0.6) : cLeaf.rgb)));
    float open = 0.75 + 0.25 * sin(gT * 0.6 + h.y * 20.0);
    float a = atan(d3.y, d3.x) + h.z * 6.0;
    float petals = 0.2 * open * (0.6 + 0.4 * abs(cos(a * (2.5 + floor(h.y * 3.0)))));
    float bloomD = r - petals;
    float thr = mix(0.72, 0.2, gLift);                           // fewer blooms at rest
    vec3 e = col * exp(-max(bloomD, 0.0) * 22.0) * 0.55 * step(thr, h.y);
    e += mix(col, vec3(1.0), 0.5) * exp(-r * 30.0) * 0.6 * step(thr, h.y);
    e += cLeaf.rgb * 0.18 * exp(-abs(r - 0.32) * 25.0) * step(h.y, 0.45);
    return e * reg * 1.7 * (1.0 + 0.5 * gBeat);
}

// bronchial tree inside each lung: a folded branching fractal
float lungTree(vec3 p) {
    vec2 q = vec2(abs(p.x) - 0.55, p.y - 4.55);
    q.x = -q.x;
    q.y = -q.y;
    float d = 1e5, L = 0.42, w = 0.03;
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        d = min(d, length(vec2(q.x, q.y - clamp(q.y, 0.0, L))) - w);
        q.y -= L;
        q.x = abs(q.x);
        q = rot(0.42 + 0.08 * sin(gT * 0.6 + fi)) * q;
        L *= 0.78; w *= 0.8;
    }
    return max(d, abs(p.z - 0.32) - 0.3);
}

vec3 interiorEmit(vec3 p) {
    vec3 e = vec3(0.0);
    float dl = lungSDF(p);
    float inL = smoothstep(0.06, -0.1, dl);
    if (inL > 0.001) {
        e += mix(cMagenta.rgb, cViolet.rgb, 0.5 + 0.5 * sin(p.y * 2.0 + gT)) * inL * 0.22 * lungGlow * (1.0 + 0.6 * bassP() * audioReact) * (0.55 + 0.7 * gLift);
        e += sparkles(p + vec3(0.0, gT * 0.06, 0.0), 8.0, mix(0.18, 0.8, gLift)) * inL * sparkle;
        float tr = lungTree(p);
        float tp = 0.7 + 0.3 * sin(gT * 2.0 - p.y * 3.5) + gBeat;
        e += mix(vec3(1.0, 0.62, 0.45), cMagenta.rgb, 0.35) * exp(-max(tr, 0.0) * 32.0) * inL * 2.2 * tp * nerves * (0.35 + 0.8 * gLift);
    }
    e += mix(cMagenta.rgb, vec3(1.0), 0.3) * exp(-abs(dl) * 22.0) * 0.25 * lungGlow;
    float hb = length((p - vec3(0.0, 7.7, 0.05)) / vec3(0.72, 0.68, 0.85));
    if (hb < 1.0) e += sparkles(p, 9.0, 0.2) * smoothstep(1.0, 0.6, hb) * sparkle;
    e += sparkles(p + 3.1, 5.0, mix(0.02, 0.16, gLift)) * 0.6 * sparkle;
    float along;
    float dn = nerveSDF(p, along);
    float pulse = exp(-pow((along - fract(TIME * 0.4)) * 8.0, 2.0)) + gBeat * 1.5
                + gBuild * exp(-pow((along - fract(TIME * 0.4 + 3.0 * gBuild * gBuild)) * 8.0, 2.0)) * 1.5
                + gDrop * 3.0;
    e += mix(cMagenta.rgb, vec3(1.0, 0.8, 0.95), 0.35) * exp(-max(dn, 0.0) * 50.0) * (0.8 + pulse) * nerves;
    e += cMagenta.rgb * exp(-max(dn, 0.0) * 11.0) * 0.12 * nerves;
    if (vines > 0.001) e += vineEmit(p) * vines;
    if (flowers > 0.001) e += flowerEmit(p) * flowers;
    if (elements > 0.001) e += elementEmit(p) * elements;
    if (bellyGarden > 0.001) e += bellyEmit(p) * bellyGarden * (0.45 + 0.75 * gLift);
    // drop: a shockwave of light expanding out from the heart
    if (gDrop > 0.01) {
        float rw = (1.0 - gDrop) * 7.0;
        float sw = exp(-abs(length((p - vec3(0.0, 4.2, 0.3)) * vec3(1.0, 0.6, 1.0)) - rw) * 12.0);
        e += mix(vec3(1.0, 0.85, 0.95), cCyan.rgb, 0.35) * sw * gDrop * 0.55;
    }
    return e;
}

vec4 scenePass() {
    vec2 R = RENDERSIZE.xy;
    vec2 sp = (gl_FragCoord.xy - 0.5 * R) / R.y;
    gFlip = texture2D(bodyTex, vec2(0.5 / A_W, 0.5 / A_H)).b > 0.5 ? 1.0 : 0.0;
    // still camera on the figure, turned toward profile
    vec3 ta = vec3(0.0, 3.2, 0.0);
    vec3 ro = ta + vec3(0.0, 0.8, 22.5 / figScale);
    vec3 ww = normalize(ta - ro), uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0))), vv = cross(uu, ww);
    vec3 rd = normalize(sp.x * uu + sp.y * vv + 2.0 * ww);
    mat2 Ry = rot(facing * 1.4);
    vec3 roB = ro; roB.xz = Ry * roB.xz;
    vec3 rdB = rd; rdB.xz = Ry * rdB.xz;

    // trace the glass skin (start at the body's bounding box)
    vec3 bmin = SDF_LO, bmax = SDF_LO + (SDF_N - 1.0) * SDF_VOX;
    vec3 inv = 1.0 / rdB;
    vec3 t0 = (bmin - roB) * inv, t1 = (bmax - roB) * inv;
    vec3 tsm = min(t0, t1), tbg = max(t0, t1);
    float tn = max(max(tsm.x, tsm.y), tsm.z), tf = min(min(tbg.x, tbg.y), tbg.z);
    if (tf < max(tn, 0.0)) return vec4(0.0);
    float t = max(tn, 0.0);
    bool hit = false;
    for (int i = 0; i < 110; i++) {
        float d = bodySDF(roB + rdB * t);
        if (d < 0.004) { hit = true; break; }
        t += max(d * 0.9, 0.01);
        if (t > tf) break;
    }
    if (!hit) return vec4(0.0);
    vec3 P = roB + rdB * t;
    vec3 n = bodyNormal(P);
    float fres = pow(1.0 - clamp(dot(n, -rdB), 0.0, 1.0), 2.5);

    // march the interior volume
    vec3 acc = vec3(0.0);
    float trans = 1.0;
    float dt = 0.06;
    float s = 0.02;
    for (int i = 0; i < 90; i++) {
        vec3 q = P + rdB * s;
        if (bodySDF(q) > 0.01) break;
        acc += interiorEmit(q) * trans * dt * 3.2;
        trans *= 0.988;
        s += dt;
    }
    vec3 L1 = normalize(vec3(0.6, 0.7, 0.5));
    vec3 rf = reflect(rdB, n);
    float spec = pow(clamp(dot(rf, L1), 0.0, 1.0), 70.0) + 0.5 * pow(clamp(dot(rf, normalize(vec3(-0.7, 0.3, 0.6))), 0.0, 1.0), 40.0);
    vec3 rimC = mix(shellTint.rgb, cCyan.rgb, 0.5 + 0.5 * n.y);
    acc *= 0.6 + 0.5 * gLift;
    rimC *= 0.75 + 0.35 * gLift + 1.2 * gDrop;
    vec3 col = acc + rimC * fres * 0.9 * shellGlow + vec3(0.9, 0.95, 1.0) * spec * 0.7 * shellGlow;
    col += shellTint.rgb * (0.07 + 0.18 * pow(fres, 0.7));                 // luminous glass body
    col += shellTint.rgb * 0.12 * smoothstep(0.0, 1.0, n.y * 0.5 + 0.5) * shellGlow;
    float a = clamp(0.35 + fres + lum(acc) * 2.0, 0.0, 1.0);
    return vec4(col, a);
}

vec4 bloomPass() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 px = 1.0 / RENDERSIZE.xy;
    vec3 c = vec3(0.0);
    c += texture2D(fScene, uv + px * vec2(-0.25, -0.25)).rgb;
    c += texture2D(fScene, uv + px * vec2(0.25, -0.25)).rgb;
    c += texture2D(fScene, uv + px * vec2(-0.25, 0.25)).rgb;
    c += texture2D(fScene, uv + px * vec2(0.25, 0.25)).rgb;
    c *= 0.25;
    c *= smoothstep(0.35, 1.0, lum(c));
    return vec4(c, 1.0);
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
// one or two figures standing side by side, centred; the second is mirrored
float bodyD(vec2 p) {
    if (float(peopleSel) < 0.5) return frontSD(p);
    float k = 0.8;                                      // a touch smaller so both fit
    float off = 2.55 * pairGap;
    float a = frontSD((p - vec2(-off, 0.0)) / k) * k;
    float b = frontSD(vec2(-(p.x - off), p.y) / k) * k;
    return min(a, b);
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
    float flowT = TIME * 0.35 * ringSpeed + (audioBassTime * 0.35 + audioMidTime * 0.2) * ar * ringSpeed + 1.6 * gBuild * gBuild;
    float stepT = floor((TIME * 0.8 + audioMidTime * 1.2 * ar) * (1.0 + 2.0 * gLift) * tileFlip + 10.0 * gBuild * gBuild);

    vec3 col = backdrop(uv);
    float alpha = 0.0;

    // ── the figure as a tile mosaic ──
    float cs = 16.7 / floor(tileCount);                // tile size in dm
    vec2 cell = floor(p / cs);
    vec2 l = p / cs - cell;                            // 0..1 inside the tile
    float dC = bodyD((cell + 0.5) * cs);
    // drop: tiles near the edge scatter outward for a moment
    float scatter = gDrop * step(dC, 0.0) * step(-0.9, dC) * step(0.6, h12(cell + 3.0));
    if (dC < 0.08 * cs && scatter < 0.5) {
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
        float gp = min(max(pxDm * 2.0, 0.05), ringSpacing * 0.16);
        vec2 ps = (floor(p / gp) + 0.5) * gp;
        float ds = bodyD(ps);
        float sp2 = ringSpacing * (1.0 - 0.3 * gBuild);
        float rp = ds / sp2 - flowT;
        float ri = floor(rp);
        float fr = fract(rp);
        float lw = clamp(1.1 * gp / sp2, 0.02, 0.22);
        float line = step(fr, lw);
        // break each ring into dashes: segments of the pixel grid, varying per ring
        vec2 seg = floor(ps / (gp * (4.0 + 6.0 * h11(ri * 1.3))));
        float on = step(1.0 - ringDash * (0.7 + 0.4 * gLift), h12(seg + ri * 7.13));
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

vec4 finalPass() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec4 sc = texture2D(fScene, uv);
    vec3 b = vec3(0.0);
    float ws = 0.0;
    for (int i = 0; i < 24; i++) {
        float fi = float(i);
        float ang = fi * 2.39996323;
        float rr = (fi + 0.5) / 24.0;
        vec2 o = vec2(cos(ang), sin(ang)) * rr * rr * 14.0 * 4.0 / R;
        float w = exp(-rr * 2.5);
        b += texture2D(fBloom, uv + o).rgb * w;
        ws += w;
    }
    b = b / ws * bloom * (0.65 + 0.6 * gLift + 0.8 * gDrop);
    vec3 light = sc.rgb + b;
    light *= 1.0 + 0.12 * audioLevel * audioReact;
    vec3 col = 1.0 - exp(-light * 1.4);
    float cov = clamp(sc.a + lum(b), 0.0, 1.0);
    col = backdrop(uv) * (1.0 - clamp(sc.a, 0.0, 1.0)) + col;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    return vec4(clamp(col, 0.0, 1.0), transparentBg ? cov : 1.0);
}

void main() {
    showDynamics();
    gT = TIME * speed + 0.3 * audioBassTime * audioReact * speed;
    gBeat = audioBeatPulse * audioReact;
    gBreath = 1.0 + 0.05 * sin(TIME * 1.2) + 0.08 * bassP() * audioReact + 0.05 * (gLift - 0.5) + 0.07 * gDrop;
    if (float(styleSel) < 0.5) {                       // Pixel Mosaic: flat 2D, no raymarch
        gl_FragColor = PASSINDEX == 2 ? mosaicPass() : vec4(0.0);
        return;
    }
    if (PASSINDEX == 0)      gl_FragColor = scenePass();
    else if (PASSINDEX == 1) gl_FragColor = bloomPass();
    else                     gl_FragColor = finalPass();
}
