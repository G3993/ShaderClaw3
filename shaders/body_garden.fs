/*{
  "DESCRIPTION": "Body Garden — a still glass human figure, seen three-quarter from head to hips, with a living garden inside. The glass shell glows at its rim and catches highlights while the interior is raymarched as a volume: lungs breathe and sparkle with thousands of multicoloured points of light, a glowing pink nerve tree branches from the throat into the chest and pulses on the beat, vines spiral up through the torso and neck, flowers open and close in the head, shoulder and chest, a dense bed of blooms and leaves fills the belly, a cluster of sparks lives in the brain, and abstract 3D elements (rings, cubes and orbs) drift through the body. The figure never moves; everything inside it does. Bass makes the lungs breathe deeper, beats send a pulse up the nerves and flash the sparks, mids speed the drifting elements, highs twinkle the lights.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "3D", "Nature", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "facing",      "LABEL": "Figure Turn",      "TYPE": "float", "AUDIO": false, "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Figure" },
    { "NAME": "figScale",    "LABEL": "Figure Size",      "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Figure" },
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
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.02, 0.06, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
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
    if (m < 0.5 || palAmount <= 0.0) return c;
    vec3 acc = m < 1.5 ? palRed.rgb : palBlue.rgb;
    float L = clamp(dot(c, vec3(0.2126, 0.7152, 0.0722)), 0.0, 1.0);
    float mx = max(c.r, max(c.g, c.b)), mn = min(c.r, min(c.g, c.b));
    float sat = mx > 1e-4 ? (mx - mn) / mx : 0.0;
    float t = pow(L, 0.8);
    vec3 ramp = t < 0.5 ? mix(vec3(0.0), acc, t * 2.0) : mix(acc, vec3(1.0), (t - 0.5) * 2.0);
    vec3 mono = vec3(smoothstep(0.0, 1.0, L));
    vec3 o = mix(mono, ramp, smoothstep(0.06, 0.4, sat));
    return mix(c, o, palAmount);
}

float gT, gBreath, gBeat;

// ── the figure (body space: y up, face toward +z) ────────────────────────
float sdEll(vec3 p, vec3 r) { float k0 = length(p / r); float k1 = length(p / (r * r)); return k0 * (k0 - 1.0) / max(k1, 1e-5); }
float sdCap(vec3 p, vec3 a, vec3 b, float r) { vec3 pa = p - a, ba = b - a; float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0); return length(pa - ba * h) - r; }

float bodySDF(vec3 p) {
    // head
    float d = sdEll(p - vec3(0.0, 1.62, -0.04), vec3(0.3, 0.36, 0.36));
    d = smin(d, sdEll(p - vec3(0.0, 1.47, 0.12), vec3(0.24, 0.27, 0.27)), 0.12);      // face
    d = smin(d, sdEll(p - vec3(0.0, 1.47, 0.37), vec3(0.045, 0.085, 0.07)), 0.05);   // nose
    d = smin(d, sdEll(p - vec3(0.0, 1.3, 0.24), vec3(0.14, 0.09, 0.12)), 0.08);      // chin
    // neck
    d = smin(d, sdCap(p, vec3(0.0, 0.98, -0.02), vec3(0.0, 1.3, 0.02), 0.14), 0.1);
    // torso
    float t = sdEll(p - vec3(0.0, 0.45, 0.0), vec3(0.52, 0.62, 0.3));
    t = smin(t, sdEll(p - vec3(0.0, 0.62, 0.06), vec3(0.47, 0.36, 0.3)), 0.15);
    t = smin(t, sdCap(p, vec3(-0.52, 0.86, -0.02), vec3(0.52, 0.86, -0.02), 0.15), 0.18);   // shoulders
    t = smin(t, sdEll(p - vec3(0.0, -0.12, 0.01), vec3(0.42, 0.5, 0.27)), 0.2);          // abdomen
    t = smin(t, sdEll(p - vec3(0.0, -0.62, 0.0), vec3(0.46, 0.32, 0.28)), 0.2);          // hips
    d = smin(d, t, 0.14);
    // arms
    vec3 q = vec3(abs(p.x), p.yz);
    d = smin(d, sdCap(q, vec3(0.62, 0.84, -0.02), vec3(0.7, 0.18, 0.02), 0.13), 0.08);
    d = smin(d, sdCap(q, vec3(0.7, 0.18, 0.02), vec3(0.66, -0.62, 0.16), 0.105), 0.06);
    d = smin(d, sdEll(q - vec3(0.66, -0.78, 0.18), vec3(0.07, 0.13, 0.05)), 0.05);       // hands
    // cropped at the upper thigh
    d = max(d, -(p.y + 0.95));
    return d;
}

vec3 bodyNormal(vec3 p) {
    vec2 e = vec2(0.003, -0.003);
    return normalize(e.xyy * bodySDF(p + e.xyy) + e.yyx * bodySDF(p + e.yyx) + e.yxy * bodySDF(p + e.yxy) + e.xxx * bodySDF(p + e.xxx));
}

// ── the garden inside: emission at a point ───────────────────────────────
float lungSDF(vec3 p) {
    vec3 q = vec3(abs(p.x), p.yz);
    float s = gBreath;
    float d = sdEll((q - vec3(0.235, 0.46, 0.03)) / s, vec3(0.135, 0.31, 0.16)) * s;
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
    return col * exp(-d * 18.0) * (0.4 + 0.8 * tw) * (1.0 + 1.5 * highP() * audioReact + 1.5 * gBeat);
}

// nerve tree: a trunk down the throat that forks into each lung
float nerveSDF(vec3 p, out float along) {
    float w = 0.012;
    vec3 a = vec3(0.0, 1.32, 0.12), b = vec3(0.0, 0.95, 0.06), c = vec3(0.0, 0.72, 0.05);
    float wig = 0.02 * sin(p.y * 14.0 + gT);
    vec3 pw = p + vec3(wig, 0.0, 0.0);
    float d = sdCap(pw, a, b, w);
    d = min(d, sdCap(pw, b, c, w * 1.2));
    along = clamp((1.32 - p.y) / 1.0, 0.0, 1.0);
    vec3 q = vec3(abs(pw.x), pw.yz);
    // bronchi and their branches
    d = min(d, sdCap(q, c, vec3(0.18, 0.6, 0.05), w));
    d = min(d, sdCap(q, vec3(0.18, 0.6, 0.05), vec3(0.24, 0.35, 0.06), w * 0.8));
    d = min(d, sdCap(q, vec3(0.18, 0.6, 0.05), vec3(0.3, 0.68, 0.0), w * 0.7));
    d = min(d, sdCap(q, vec3(0.24, 0.35, 0.06), vec3(0.2, 0.22, 0.1), w * 0.6));
    d = min(d, sdCap(q, vec3(0.24, 0.35, 0.06), vec3(0.32, 0.28, -0.02), w * 0.6));
    // up into the head: a branching glow behind the face
    d = min(d, sdCap(pw, a, vec3(0.0, 1.55, 0.05), w * 0.8));
    d = min(d, sdCap(q, vec3(0.0, 1.55, 0.05), vec3(0.15, 1.72, -0.05), w * 0.6));
    d = min(d, sdCap(q, vec3(0.0, 1.55, 0.05), vec3(0.08, 1.8, 0.12), w * 0.6));
    return d;
}

// vines: helices climbing the body
vec3 vineEmit(vec3 p) {
    vec3 e = vec3(0.0);
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        float R = 0.18 + 0.06 * fi;
        float th = p.y * (5.0 + fi) + fi * 2.1 + gT * 0.25 * (mod(fi, 2.0) * 2.0 - 1.0);
        vec2 c = vec2(cos(th), sin(th)) * R * (p.y > 1.0 ? 0.55 : 1.0) + vec2(0.0, 0.02);
        float d = length(p.xz - c);
        float grow = smoothstep(-0.4, 0.2, sin(gT * 0.15 + fi * 2.0) * 1.2 + 1.0 - (p.y + 0.4) * 0.6);
        float stem = exp(-d * 90.0) * grow;
        // leaves: little blobs along the vine
        float lf = fract(p.y * 9.0 + fi * 0.37);
        float leaf = exp(-length(vec2(d - 0.02, (lf - 0.5) * 0.11)) * 70.0) * step(0.5, h11(floor(p.y * 9.0 + fi * 0.37) + fi * 9.0)) * grow;
        e += mix(cGold.rgb, cLeaf.rgb, 0.5 + 0.5 * sin(fi * 3.0 + p.y * 2.0)) * (stem * 0.8 + leaf * 0.6);
    }
    return e;
}

// flowers: glowing blooms with petals, opening and closing
vec3 flowerEmit(vec3 p) {
    vec3 e = vec3(0.0);
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        vec3 c = i == 0 ? vec3(0.05, 1.75, 0.08) : i == 1 ? vec3(-0.45, 0.8, 0.1) : i == 2 ? vec3(0.1, 0.2, 0.2)
               : i == 3 ? vec3(-0.12, 1.52, -0.12) : i == 4 ? vec3(0.42, 0.62, 0.12) : vec3(-0.2, 1.05, 0.05);
        vec3 q = p - c;
        q.xy = rot(fi * 1.3 + gT * 0.1) * q.xy;
        q.yz = rot(0.6 + fi) * q.yz;
        float r = length(q.xy);
        float a = atan(q.y, q.x);
        float N = 5.0 + mod(fi, 3.0);
        float open = 0.7 + 0.3 * sin(gT * 0.5 + fi * 1.7);
        float petal = 0.09 * open * (0.55 + 0.45 * abs(cos(a * N * 0.5)));
        float thick = abs(q.z + 0.15 * r * r / max(open, 0.3)) - 0.004;
        float dP = max(r - petal, thick);
        float core = length(q) - 0.02;
        vec3 col = i == 0 ? cMagenta.rgb : i == 1 ? cMagenta.rgb : i == 2 ? cViolet.rgb : i == 3 ? cGold.rgb : i == 4 ? cCyan.rgb : cViolet.rgb;
        e += col * exp(-max(dP, 0.0) * 60.0) * 0.7 + mix(col, vec3(1.0), 0.6) * exp(-max(core, 0.0) * 80.0) * 1.2;
    }
    return e;
}

// abstract elements drifting through the body
vec3 elementEmit(vec3 p) {
    vec3 e = vec3(0.0);
    float sp = gT * (0.6 + 0.6 * midP() * audioReact);
    for (int i = 0; i < 7; i++) {
        float fi = float(i);
        float hy = mix(-0.2, 1.7, h11(fi * 3.3));
        float rr = hy > 1.0 ? 0.13 : 0.28;
        vec3 c = vec3(rr * cos(sp * (0.3 + 0.1 * fi) + fi * 2.0), hy + 0.08 * sin(sp * 0.5 + fi), rr * 0.6 * sin(sp * (0.3 + 0.1 * fi) + fi * 2.0));
        vec3 q = p - c;
        q.xz = rot(sp * 0.7 + fi) * q.xz;
        q.xy = rot(sp * 0.5 + fi * 2.0) * q.xy;
        float k = mod(fi, 3.0);
        float d;
        if (k < 0.5) { vec2 tq = vec2(length(q.xz) - 0.06, q.y); d = length(tq) - 0.008; }          // ring
        else if (k < 1.5) { vec3 bq = abs(q) - vec3(0.035); d = abs(length(max(bq, 0.0)) + min(max(bq.x, max(bq.y, bq.z)), 0.0)) - 0.003; } // wire cube
        else d = abs(length(q) - 0.04) - 0.004;                                                    // bubble shell
        vec3 col = k < 0.5 ? cCyan.rgb : (k < 1.5 ? cGold.rgb : cViolet.rgb);
        e += col * exp(-max(d, 0.0) * 120.0) * 0.9;
    }
    return e;
}

// a packed bed of blooms and leaves in the abdomen
vec3 bellyEmit(vec3 p) {
    float reg = smoothstep(0.25, -0.05, p.y) * smoothstep(-0.95, -0.7, p.y);
    if (reg < 0.01) return vec3(0.0);
    vec3 gp = p * 9.0 + vec3(0.0, gT * 0.05, 0.0);
    vec3 gi = floor(gp), gf = fract(gp);
    vec3 h = h33(gi + 11.0);
    vec3 c = 0.25 + 0.5 * h33(gi + 2.0);
    vec3 d3 = gf - c;
    float r = length(d3);
    float k = h.x * 6.0;
    vec3 col = k < 1.5 ? cMagenta.rgb : (k < 2.5 ? cViolet.rgb : (k < 3.5 ? cGold.rgb : (k < 4.5 ? vec3(1.0, 0.55, 0.6) : cLeaf.rgb)));
    float open = 0.75 + 0.25 * sin(gT * 0.6 + h.y * 20.0);
    // petals: a rosette modulated around the bloom centre
    float a = atan(d3.y, d3.x) + h.z * 6.0;
    float petals = 0.2 * open * (0.6 + 0.4 * abs(cos(a * (2.5 + floor(h.y * 3.0)))));
    float bloomD = r - petals;
    vec3 e = col * exp(-max(bloomD, 0.0) * 22.0) * 0.55 * step(0.25, h.y);
    e += mix(col, vec3(1.0), 0.5) * exp(-r * 30.0) * 0.6 * step(0.25, h.y);
    // leaves fill the gaps
    e += cLeaf.rgb * 0.18 * exp(-abs(r - 0.32) * 25.0) * step(h.y, 0.45);
    return e * reg * 1.7 * (1.0 + 0.5 * gBeat);
}

// bronchial tree inside each lung: a folded branching fractal, lit peach-pink
float lungTree(vec3 p) {
    vec2 q = vec2(abs(p.x) - 0.235, p.y - 0.74);
    q.x *= sign(p.x) < 0.0 ? 1.0 : -1.0;
    float d = 1e5, L = 0.13, w = 0.009;
    q.y = -q.y;                                   // grow downward
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        d = min(d, length(vec2(q.x, q.y - clamp(q.y, 0.0, L))) - w);
        q.y -= L;
        q.x = abs(q.x);
        q = rot(0.42 + 0.08 * sin(gT * 0.6 + fi)) * q;
        L *= 0.78; w *= 0.8;
    }
    return max(d, abs(p.z - 0.04) - 0.06);
}

vec3 interiorEmit(vec3 p) {
    vec3 e = vec3(0.0);
    // lungs: soft magenta glow full of sparkles
    float dl = lungSDF(p);
    float inL = smoothstep(0.02, -0.03, dl);
    if (inL > 0.001) {
        e += mix(cMagenta.rgb, cViolet.rgb, 0.5 + 0.5 * sin(p.y * 6.0 + gT)) * inL * 0.22 * lungGlow * (1.0 + 0.6 * bassP() * audioReact);
        e += sparkles(p + vec3(0.0, gT * 0.02, 0.0), 26.0, 0.55) * inL * sparkle;
        float tr = lungTree(p);
        float tp = 0.7 + 0.3 * sin(gT * 2.0 - p.y * 12.0) + gBeat;
        e += mix(vec3(1.0, 0.62, 0.45), cMagenta.rgb, 0.35) * exp(-max(tr, 0.0) * 95.0) * inL * 2.2 * tp * nerves;
    }
    e += mix(cMagenta.rgb, vec3(1.0), 0.3) * exp(-abs(dl) * 70.0) * 0.25 * lungGlow;              // lung membrane
    // brain sparks
    float hb = length((p - vec3(0.0, 1.66, -0.05)) / vec3(0.25, 0.25, 0.28));
    if (hb < 1.0) e += sparkles(p, 30.0, 0.18) * smoothstep(1.0, 0.6, hb) * sparkle;
    // body-wide faint dust
    e += sparkles(p + 3.1, 16.0, 0.06) * 0.6 * sparkle;
    // nerves, pulsing up/down on beats
    float along;
    float dn = nerveSDF(p, along);
    float pulse = exp(-pow((along - fract(TIME * 0.4)) * 8.0, 2.0)) + gBeat * 1.5;
    e += mix(cMagenta.rgb, vec3(1.0, 0.8, 0.95), 0.35) * exp(-max(dn, 0.0) * 160.0) * (0.8 + pulse) * nerves;
    e += cMagenta.rgb * exp(-max(dn, 0.0) * 35.0) * 0.12 * nerves;
    if (vines > 0.001) e += vineEmit(p) * vines;
    if (flowers > 0.001) e += flowerEmit(p) * flowers;
    if (elements > 0.001) e += elementEmit(p) * elements;
    if (bellyGarden > 0.001) e += bellyEmit(p) * bellyGarden;
    return e;
}

vec4 scenePass() {
    vec2 R = RENDERSIZE.xy;
    vec2 sp = (gl_FragCoord.xy - 0.5 * R) / R.y;
    // still camera, figure turned toward profile
    vec3 ro = vec3(0.0, 0.55, 5.6 / figScale);
    vec3 ta = vec3(0.0, 0.45, 0.0);
    vec3 ww = normalize(ta - ro), uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0))), vv = cross(uu, ww);
    vec3 rd = normalize(sp.x * uu + sp.y * vv + 1.9 * ww);
    float yaw = facing * 1.4;
    mat2 Ry = rot(yaw);
    vec3 roB = ro; roB.xz = Ry * roB.xz;
    vec3 rdB = rd; rdB.xz = Ry * rdB.xz;

    // sphere-trace the glass skin
    float t = 2.0;
    bool hit = false;
    for (int i = 0; i < 90; i++) {
        float d = bodySDF(roB + rdB * t);
        if (d < 0.001) { hit = true; break; }
        t += d * 0.9;
        if (t > 10.0) break;
    }
    if (!hit) return vec4(0.0);
    vec3 P = roB + rdB * t;
    vec3 n = bodyNormal(P);
    float fres = pow(1.0 - clamp(dot(n, -rdB), 0.0, 1.0), 2.5);

    // march the interior volume (emission, light absorption)
    vec3 acc = vec3(0.0);
    float trans = 1.0;
    float dt = 0.022;
    float s = 0.004;
    for (int i = 0; i < 80; i++) {
        vec3 q = P + rdB * s;
        float bd = bodySDF(q);
        if (bd > 0.002) break;                               // left the body
        vec3 em = interiorEmit(q);
        acc += em * trans * dt * 9.0;
        trans *= 0.985;
        s += dt;
    }
    // glass skin
    vec3 L1 = normalize(vec3(0.6, 0.7, 0.5));
    float spec = pow(clamp(dot(reflect(rdB, n), L1), 0.0, 1.0), 60.0);
    vec3 rimC = mix(shellTint.rgb, cCyan.rgb, 0.5 + 0.5 * n.y);
    vec3 col = acc + rimC * fres * 0.9 * shellGlow + vec3(1.0) * spec * 0.6 * shellGlow;
    col += shellTint.rgb * 0.03;                            // faint glass body
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
    b = b / ws * bloom;
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
    gT = TIME * speed + 0.3 * audioBassTime * audioReact * speed;
    gBeat = audioBeatPulse * audioReact;
    gBreath = 1.0 + 0.05 * sin(TIME * 1.2) + 0.08 * bassP() * audioReact;
    if (PASSINDEX == 0)      gl_FragColor = scenePass();
    else if (PASSINDEX == 1) gl_FragColor = bloomPass();
    else                     gl_FragColor = finalPass();
}
