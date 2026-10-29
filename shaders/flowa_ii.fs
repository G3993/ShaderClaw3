/*{
  "DESCRIPTION": "Flowa II — futuristic layered flowers of light, always on a stem. A hero flower sways on a long green stem with leaves in a dense garden of others, while its head morphs through ten species: a Star Glow bloom with spiked petals and a pastel halo, a Fan of triangular blades in white, yellow and orange, Prism Shards of overlapping rectangles with an RGB split, Chrome Petals with flat pink, blue, orange and mint colour blocks, metallic rims and ball-tipped stamens, and a Bell Cluster of half-moon bells in magenta, lime and silver, grainy Tulips, layered Lotus petals, tilted Lily Pads with radial veins, a spotted Mushroom Bloom over a frilly ring, and dotted Seed Pods. Around it a garden of smaller flowers on thin stems with long tapering blade leaves fills the frame, each morphing between species on its own schedule, with riso-print grain over everything. Every head is shaded like a 3D object, with puffy depth, chrome rim light, a shimmer sweep and iridescent edges. Around it a data layer tracks the flower: boxes lock onto blobs and show an X-ray heatmap of the petals inside, joined by dashed lines and number readouts, while translucent squares in screen, multiply and difference blend modes hop over the stem. A datamosh pass smears blocky trails of the flower as it moves. At rest it is a single quiet bloom with few trackers that drift slowly from blob to blob; during a build the head spins and tightens, the trackers multiply and jitter, and the scan speeds up; on the drop the flower bursts open with an RGB split while the mosh freezes into a long smear, then the peak blooms with every layer alive. Tracker Speed and Overlay / Mosh Speed set how fast the boxes glide and the squares and trails move (both calm by default).",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "Nature", "Glitch", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speciesSel",  "LABEL": "Flower", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], "LABELS": ["Auto Morph", "Star Glow", "Fan Blades", "Prism Shards", "Chrome Petals", "Bell Cluster", "Tulip", "Lotus", "Lily Pad", "Mushroom Bloom", "Seed Pod"], "GROUP": "Flower" },
    { "NAME": "gardenCount", "LABEL": "Garden Density",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 10.0, "DEFAULT": 10.0, "GROUP": "Garden" },
    { "NAME": "bladeLeaves", "LABEL": "Blade Leaves",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Garden" },
    { "NAME": "risoGrain",   "LABEL": "Riso Grain",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Garden" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 40.0, "DEFAULT": 9.0, "GROUP": "Flower" },
    { "NAME": "flowerSize",  "LABEL": "Flower Size",      "TYPE": "float", "MIN": 0.5, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Flower" },
    { "NAME": "sway",        "LABEL": "Sway",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 2.0, "GROUP": "Flower" },
    { "NAME": "spin",        "LABEL": "Head Spin",        "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Flower" },
    { "NAME": "shimmer",     "LABEL": "Shimmer",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Flower" },
    { "NAME": "stemColor",   "LABEL": "Stem Colour",      "TYPE": "color", "DEFAULT": [0.05, 0.6, 0.3, 1.0], "GROUP": "Flower" },
    { "NAME": "trackers",    "LABEL": "Tracking Boxes",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Data Layer" },
    { "NAME": "xray",        "LABEL": "X-Ray Heat",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Data Layer" },
    { "NAME": "squares",     "LABEL": "Blend Squares",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Data Layer" },
    { "NAME": "blobSpeed",   "LABEL": "Tracker Speed",    "TYPE": "float", "AUDIO": false, "MIN": 0.02, "MAX": 1.5, "DEFAULT": 0.22, "GROUP": "Data Layer" },
    { "NAME": "overlaySpeed","LABEL": "Overlay / Mosh Speed", "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 2.0, "DEFAULT": 0.5, "GROUP": "Data Layer" },
    { "NAME": "readouts",    "LABEL": "Numbers & Lines",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Data Layer" },
    { "NAME": "moshAmt",     "LABEL": "Datamosh Trails",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Data Layer" },
    { "NAME": "rgbSplit",    "LABEL": "RGB Split",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Data Layer" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [1.0, 0.7333, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.03, 0.05, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
    { "TARGET": "flowerBuf" },
    { "TARGET": "moshBuf", "PERSISTENT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h21(float p) { vec3 p3 = fract(vec3(p) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vn2(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
float sdSeg(vec2 p, vec2 a, vec2 b) { vec2 pa = p - a, ba = b - a; float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0); return length(pa - ba * h); }

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
vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

// ── Show dynamics: rest → build → peak / drop ───────────────────────────
float gRest, gBuild, gPeak, gDrop, gLift;
void showDynamics() {
    float m = float(showState);
    float e = clamp(audioEnergy, 0.0, 1.0);
    float b = clamp(audioBuildup, 0.0, 1.0);
    float d = clamp(audioDrop, 0.0, 1.0);
    if (m > 0.5) {
        if (m < 1.5)      { e = 0.04; b = 0.0; d = 0.0; }
        else if (m < 2.5) { float r = fract(TIME / 8.0); e = 0.3 + 0.35 * r; b = r * r; d = 0.0; }
        else              { e = 1.0; b = 0.0; d = exp(-fract(TIME / 4.0) * 5.0); }
    }
    float k = dynamics;
    gBuild = b * k;
    gDrop  = d * k;
    gPeak  = smoothstep(0.5, 0.9, e) * k;
    gRest  = (1.0 - smoothstep(0.08, 0.4, e)) * (1.0 - b) * k;
    gLift  = mix(0.5, clamp(0.15 + e * 0.7 + gBuild * 0.25 + d * 0.3, 0.0, 1.0), k);
}

float gT, gStep, gBT;
float gHue;      // per-flower colour seed
vec2  gH;        // head centre (screen units)
float gRH;       // head radius
float gSpinA;    // head rotation

// ── the five species: return signed distance (neg inside), colour, glow ──
// ── garden species (refs: tulips, lotus, lily pads, psychedelic mushroom, seed pods) ──
vec3 tulipHue(float h) {
    h = fract(h) * 5.0;
    if (h < 1.0) return vec3(0.92, 0.08, 0.06);
    if (h < 2.0) return vec3(1.0, 0.85, 0.3);
    if (h < 3.0) return vec3(0.85, 0.1, 0.45);
    if (h < 4.0) return vec3(0.75, 0.55, 0.95);
    return vec3(1.0, 0.45, 0.35);
}
float species2(float k, vec2 q, out vec3 col, out vec3 glow) {
    float RH = gRH;
    glow = vec3(0.0);
    col = vec3(1.0);
    float bloom = 0.85 + 0.15 * gLift + 0.25 * gDrop - 0.12 * gBuild;
    if (k < 5.5) {
        // Tulip: three overlapping cupped petals, darker at the base and on the side petals
        vec3 hue = tulipHue(gHue);
        float open = 0.12 * bloom;
        vec2 c0 = q / RH; c0.y -= 0.05;
        float dC = length(c0 / vec2(0.4, 0.68)) - 1.0;
        vec2 l = rot(0.25 + open) * (c0 - vec2(-0.3 - open, -0.02));
        float dL = length(l / vec2(0.36, 0.64)) - 1.0;
        vec2 rr = rot(-0.25 - open) * (c0 - vec2(0.3 + open, -0.02));
        float dR = length(rr / vec2(0.36, 0.64)) - 1.0;
        float d = min(dC, min(dL, dR)) * 0.4 * RH;
        vec3 c = mix(hue * 0.35, hue, smoothstep(-0.7, 0.2, c0.y));
        c = mix(c, hue * 1.15 + 0.1, smoothstep(0.0, 0.6, c0.y) * 0.5);
        c *= dC < 0.0 ? 1.0 : 0.72;
        col = c;
        return d;
    }
    if (k < 6.5) {
        // Lotus: two layers of pointed petals fanning upward, cream → coral → mint
        float d = 1e3;
        vec2 base = vec2(0.0, -0.45) * RH;
        for (int j = 0; j < 9; j++) {
            float fj = float(j);
            bool inner = fj > 4.5;
            float n = inner ? 4.0 : 5.0;
            float jj = inner ? fj - 5.0 : fj;
            float th = (jj / (n - 1.0) - 0.5) * (inner ? 1.4 : 2.4) * bloom;
            float L = RH * (inner ? 0.95 : 1.1) * (1.0 - 0.18 * abs(th));
            float W = RH * (inner ? 0.24 : 0.3);
            vec2 u = rot(th) * (q - base);
            float t = u.y / L;
            float w = W * pow(max(sin(PI * clamp(t, 0.0, 1.0)), 0.0), 0.75) * (1.0 - 0.35 * t);
            float dj = max(abs(u.x) - w, max(-u.y, u.y - L));
            if (dj < 0.0 && (d > 0.0 || inner)) {
                vec3 c = mix(vec3(0.98, 0.95, 0.85), vec3(1.0, 0.45, 0.4), smoothstep(0.35, 1.0, t));
                c = mix(c, vec3(0.3, 0.95, 0.75), smoothstep(0.35, 0.0, t) * 0.6);
                c *= 0.8 + 0.25 * (1.0 - abs(u.x) / max(w, 1e-4));
                col = c * (inner ? 1.05 : 0.85);
            }
            d = min(d, dj);
        }
        return d;
    }
    if (k < 7.5) {
        // Lily Pad: a tilted round leaf with radial veins, a notch and an orange bud
        vec2 u = q / RH; u.y = (u.y + 0.1) * 1.7;
        float rr = length(u);
        float ang = atan(u.y, u.x);
        float d = (rr - 0.95 - 0.04 * sin(ang * 7.0)) * RH / 1.7;
        if (abs(atan(u.x, -u.y)) < 0.18 && rr > 0.15) d = max(d, 0.01 * RH);
        float veins = smoothstep(0.08, 0.0, abs(fract(ang / TAU * 18.0) - 0.5)) * smoothstep(0.1, 0.9, rr);
        vec3 c = mix(vec3(0.85, 0.95, 0.3), vec3(0.05, 0.55, 0.3), smoothstep(0.0, 0.7, rr));
        c = mix(c, vec3(0.0, 0.3, 0.2), smoothstep(0.7, 1.0, rr));
        c = mix(c, vec3(0.75, 1.0, 0.5), veins * 0.45);
        float bud = length((q - vec2(0.05, 0.08) * RH) / (vec2(0.09, 0.07) * RH)) - 1.0;
        if (bud < 0.0) c = vec3(1.0, 0.55, 0.1);
        col = c;
        return min(d, bud * 0.07 * RH);
    }
    if (k < 8.5) {
        // Mushroom Bloom: spotted dome over a frilly orange ring with a dark mouth
        vec2 u = q / RH;
        float dDome = max(length((u - vec2(0.0, 0.05)) / vec2(0.8, 0.7)) - 1.0, -(u.y - 0.05) * 3.0);
        float frill = length(u / vec2(0.95, 0.24)) - 1.0 - 0.06 * sin(u.x * 22.0);
        float d = min(dDome, frill) * 0.3 * RH;
        vec3 c;
        if (frill < 0.0 && u.y < 0.05) {
            float rr = length(u / vec2(0.95, 0.24));
            c = mix(vec3(0.3, 0.05, 0.08), vec3(1.0, 0.95, 0.4), smoothstep(0.2, 0.45, rr));
            c = mix(c, vec3(1.0, 0.45, 0.05), smoothstep(0.55, 1.0, rr));
        } else {
            c = mix(vec3(1.0, 0.1, 0.15), vec3(1.0, 0.35, 0.6), smoothstep(0.7, 0.0, u.y));
            vec2 sp = u * 4.0;
            float spot = smoothstep(0.42, 0.2, length(fract(sp + vec2(0.5 * floor(sp.y), 0.0)) - 0.5));
            c = mix(c, vec3(0.85, 1.0, 0.65), spot * 0.6);
            c += vec3(1.0) * exp(-dot(u - vec2(-0.35, 0.5), u - vec2(-0.35, 0.5)) * 8.0) * 0.5;
        }
        col = c;
        return d;
    }
    // Seed Pod / bud: a teardrop with a gradient and either a dotted skin or a red eye
    vec2 u = q / RH;
    u.x /= 1.0 - 0.35 * smoothstep(-0.2, 0.9, u.y);
    float d = (length(u / vec2(0.55, 0.85)) - 1.0) * 0.5 * RH;
    vec3 c = mix(vec3(0.45, 1.0, 0.35), vec3(1.0, 0.85, 0.3), smoothstep(-0.8, 0.0, u.y));
    c = mix(c, vec3(1.0, 0.4, 0.55), smoothstep(0.0, 0.8, u.y));
    if (fract(gHue * 3.7) > 0.5) {
        vec2 g = u * 9.0;
        float dt = smoothstep(0.3, 0.15, length(fract(g + vec2(0.5 * mod(floor(g.y), 2.0), 0.0)) - 0.5));
        c = mix(c, c * 0.25, dt * 0.85);
    } else {
        c = mix(c, vec3(1.0, 0.2, 0.15), smoothstep(0.08, 0.04, length(u - vec2(0.0, -0.25))));
    }
    col = c;
    return d;
}

float species(float k, vec2 q, out vec3 col, out vec3 glow) {
    if (k > 4.5) return species2(k, q, col, glow);
    float RH = gRH;
    float r = length(q), a = atan(q.y, q.x);
    float t = r / RH;
    glow = vec3(0.0);
    col = vec3(1.0);
    float bloom = 0.85 + 0.15 * gLift + 0.25 * gDrop - 0.12 * gBuild;
    if (k < 0.5) {
        // Star Glow: two layers of spiked petals, pastel halo
        float N = 8.0;
        float s1 = pow(1.0 - abs(sin(N * 0.5 * a)), 2.6);
        float R1 = RH * mix(0.42, 1.0, s1) * bloom;
        float d1 = r - R1;
        float s2 = pow(1.0 - abs(sin(N * 0.5 * (a + PI / N))), 2.4);
        float R2 = RH * 0.55 * mix(0.4, 1.0, s2) * bloom;
        float d2 = r - R2;
        col = mix(vec3(1.0), vec3(0.6, 1.0, 0.9), smoothstep(0.08, 0.3, t));
        col = mix(col, vec3(1.0, 0.65, 0.75), smoothstep(0.3, 0.5, t) * 0.7);
        col = mix(col, vec3(0.1, 0.08, 0.3), smoothstep(0.45, 0.95, t));
        if (d2 < 0.0) col = mix(vec3(0.9, 1.0, 1.0), vec3(0.15, 0.12, 0.4), smoothstep(0.0, 0.5, t) * 0.8);
        col = mix(col, vec3(0.08, 0.06, 0.25), exp(-abs(d2) / (0.006 * RH * 4.0)) * 0.7);
        col += vec3(0.2, 1.0, 0.6) * exp(-r / (0.03 * RH)) * 0.6;
        // halo
        float hal = exp(-max(d1, 0.0) / (0.18 * RH)) * smoothstep(1.4, 0.4, t);
        glow = mix(vec3(0.55, 0.45, 1.0), vec3(1.0, 0.65, 0.75), 0.5 + 0.5 * sin(t * 9.0 - gT)) * hal * 0.35;
        return d1;
    }
    if (k < 1.5) {
        // Fan Blades: triangular blades with straight cut ends, gap at the bottom
        float NB = 12.0;
        float aa = a + PI;
        float i = floor(aa / (TAU / NB));
        float la = fract(aa / (TAU / NB)) - 0.5;
        float hb = h11(i * 3.7 + 1.0);
        float Ri = RH * (0.75 + 0.4 * hb) * bloom;
        float bottom = step(abs(a + PI * 0.5), 0.55);             // leave room for the stem
        float gap = step(0.86, h11(i * 5.1));                      // a missing blade or two
        float w = 0.42;
        float dEnd = r * cos(la * TAU / NB) - Ri;
        float dSide = (abs(la) - w) * r * TAU / NB;
        float d = max(dEnd, dSide);
        if (bottom > 0.5 || gap > 0.5) d = max(d, 0.02 * RH);
        float dc = r - 0.17 * RH * bloom;                         // white heart
        d = min(d, dc);
        col = mix(vec3(1.0), vec3(1.0, 0.85, 0.15), smoothstep(0.2, 0.6, t));
        col = mix(col, vec3(1.0, 0.45, 0.05), smoothstep(0.6, 1.05, t));
        float irid = exp(-pow(la * 5.0 + 1.2 - 0.6 * sin(gT + i), 2.0));
        col = mix(col, mix(vec3(0.75, 0.75, 1.0), vec3(0.6, 1.0, 1.0), 0.5 + 0.5 * sin(t * 8.0)), irid * 0.55 * smoothstep(0.2, 0.5, t));
        if (dc < 0.0) col = mix(vec3(1.0), vec3(1.0, 0.95, 0.3), step(0.8, h12(floor(q / (0.012 * RH)))) * smoothstep(0.1, 0.17, t));
        return d;
    }
    if (k < 2.5) {
        // Prism Shards: overlapping rectangles radiating out, white → pink → red
        float d = 1e3;
        for (int j = 0; j < 12; j++) {
            float fj = float(j);
            float th = fj / 12.0 * TAU * 0.5 + PI * 0.25 + (h11(fj) - 0.5) * 0.4 + (fj > 5.5 ? PI : 0.0) * 0.55;
            float L = RH * (0.55 + 0.6 * h11(fj + 3.0)) * bloom;
            float W = RH * (0.1 + 0.18 * h11(fj + 7.0));
            vec2 u = rot(-th) * q;
            u.y -= W * 0.4 * (h11(fj + 11.0) - 0.5);
            float dj = max(max(u.x - L, -u.x + 0.05 * RH), abs(u.y) - W * smoothstep(0.0, 0.25 * RH, u.x + 0.02));
            if (dj < 0.0) {
                float s = u.x / L;
                col = mix(vec3(1.0), vec3(1.0, 0.55, 0.8), smoothstep(0.1, 0.55, s));
                col = mix(col, vec3(1.0, 0.08, 0.15), smoothstep(0.55, 1.0, s));
                col = mix(col, vec3(0.2, 0.55, 1.0), smoothstep(0.2, 1.0, u.y / W) * 0.6);
            }
            d = min(d, dj);
        }
        // thin needles in the middle
        for (int j = 0; j < 4; j++) {
            float fj = float(j);
            vec2 u = rot(-(PI * 0.42 + fj * 0.09)) * q;
            float dn = max(abs(u.y) - 0.012 * RH * (1.0 - u.x / (0.6 * RH)), max(u.x - 0.6 * RH, -u.x));
            if (dn < 0.0) col = mix(vec3(1.0), vec3(1.0, 0.85, 0.2), u.x / (0.6 * RH));
            d = min(d, dn);
        }
        col += vec3(0.1, 1.0, 0.8) * exp(-r / (0.05 * RH)) * 0.6;
        return d;
    }
    if (k < 3.5) {
        // Chrome Petals: ellipse petals with flat colour blocks, metallic rims, stamens
        float d = 1e3;
        vec3 pc = vec3(0.0);
        for (int j = 0; j < 7; j++) {
            float fj = float(j);
            float th = fj / 7.0 * TAU + 0.3;
            vec2 u = rot(-th) * q;
            u.x -= 0.55 * RH * bloom;
            float dj = length(u / vec2(0.5 * RH * bloom, 0.24 * RH)) - 1.0;
            dj *= 0.24 * RH;
            if (dj < d) {
                d = dj;
                float cut = step(0.0, dot(u, normalize(vec2(h11(fj) - 0.5, 1.0))) + (h11(fj + 2.0) - 0.5) * 0.2 * RH);
                vec3 c1 = h11(fj + 4.0) < 0.5 ? vec3(1.0, 0.45, 0.65) : vec3(0.25, 0.45, 1.0);
                vec3 c2 = h11(fj + 6.0) < 0.5 ? vec3(1.0, 0.65, 0.2) : vec3(0.55, 0.95, 0.85);
                pc = mix(c1, c2, cut);
                // engraved lines
                pc *= 0.9 + 0.1 * step(0.5, fract(u.x / (0.06 * RH) + u.y * 3.0 / RH));
            }
        }
        col = pc;
        // center: dark chrome ball + stamens with ball tips
        float dcen = r - 0.16 * RH;
        if (dcen < 0.0) {
            vec2 n = q / (0.16 * RH);
            col = mix(vec3(0.05, 0.03, 0.12), vec3(0.6, 0.5, 1.0), pow(clamp(1.0 - length(n - vec2(-0.35, 0.35)), 0.0, 1.0), 4.0));
        }
        d = min(d, dcen);
        for (int j = 0; j < 14; j++) {
            float fj = float(j);
            float th = fj / 14.0 * TAU + 0.15;
            vec2 tip = vec2(cos(th), sin(th)) * RH * (0.36 + 0.06 * h11(fj));
            float ds = sdSeg(q, vec2(cos(th), sin(th)) * 0.12 * RH, tip) - 0.006 * RH;
            float db = length(q - tip) - 0.025 * RH;
            if (min(ds, db) < 0.0) col = db < 0.0 ? vec3(0.9, 0.85, 1.0) : vec3(0.55, 0.35, 0.85);
            d = min(d, min(ds, db));
        }
        return d;
    }
    // Bell Cluster: half-moon bells hanging from the arch, magenta → lime → silver
    float d = 1e3;
    for (int j = 0; j < 7; j++) {
        float fj = float(j);
        vec2 b = RH * vec2(0.25 + 0.45 * h11(fj + 1.0) + 0.1 * sin(gT * 0.5 + fj), 0.55 - fj * 0.3);
        b += vec2(0.03, 0.0) * sin(gT * 1.3 + fj * 2.0) * RH;
        float rb = RH * (0.26 + 0.05 * h11(fj + 9.0)) * bloom;
        vec2 u = q - b;
        float dj = max(length(u) - rb, dot(u, normalize(vec2(1.0, -1.0))) - rb * 0.25);
        if (dj < d) {
            d = dj;
            float s = dot(u, normalize(vec2(1.0, -1.0))) / rb;           // diagonal gradient
            vec3 c = mix(vec3(1.0, 0.3, 0.95), vec3(0.75, 1.0, 0.1), smoothstep(-1.0, -0.3, s));
            c = mix(c, vec3(1.0), smoothstep(-0.3, 0.0, s));
            c = mix(c, vec3(0.25), smoothstep(0.0, 0.25, s));
            col = c;
        }
        // pedicel: a thin arc from the arch to each bell
        float dp = abs(length(q - (b + vec2(-0.2, 0.25) * RH)) - 0.22 * RH) - 0.008 * RH;
        dp = max(dp, -dot(q - b, vec2(0.0, 1.0)));
        if (dp < 0.0 && dp < d) col = stemColor.rgb;
        d = min(d, dp);
    }
    return d;
}

// head (two species morphing) + stem + leaves. Returns rgb, alpha; height for shading
vec4 flower(vec2 p, out float dOut) {
    float ph = TIME / morphPeriod;
    float ss = float(speciesSel);
    float ka = ss > 0.5 ? ss - 1.0 : mod(floor(ph) * 3.0, 10.0);
    float kb = ss > 0.5 ? ka : mod(floor(ph) * 3.0 + 3.0, 10.0);
    float f = ss > 0.5 ? 0.0 : smoothstep(0.7, 1.0, fract(ph));
    vec2 q = rot(-gSpinA) * (p - gH);
    q.x /= 0.78 + 0.22 * cos(gT * 0.4);                    // 3D tilt
    vec3 cA, gA, cB, gB;
    float dA = species(ka, q, cA, gA);
    float d = dA; vec3 col = cA; vec3 glow = gA;
    if (f > 0.001) {
        float dB = species(kb, q, cB, gB);
        d = mix(dA, dB, f);
        col = mix(cA, cB, f);
        glow = mix(gA, gB, f);
    }
    dOut = d;
    float aa = 1.5 / RENDERSIZE.y;
    float head = smoothstep(aa, -aa, d);
    // stem: quadratic curve from below the frame to the head
    vec2 base = vec2(0.02 * sin(gT * 0.1), -0.47);
    vec2 ctrl = vec2(-0.18 + 0.05 * sin(gT * 0.3) * sway, -0.1);
    float ds = 1e3;
    vec2 prev = base;
    for (int i = 1; i <= 16; i++) {
        float t = float(i) / 16.0;
        vec2 bp = mix(mix(base, ctrl, t), mix(ctrl, gH, t), t);
        ds = min(ds, sdSeg(p, prev, bp));
        prev = bp;
    }
    float sw = 0.0055 * flowerSize * (1.0 + 0.4 * (1.0 - smoothstep(-0.6, 0.0, p.y)));
    float stem = smoothstep(sw + aa, sw - aa, ds);
    // leaves: two long lens leaves at the base (outlined or filled by species)
    float leaf = 0.0;
    for (int i = 0; i < 2; i++) {
        float fi = float(i);
        vec2 lp = p - (base + vec2(0.0, 0.12));
        lp = rot(-(fi < 0.5 ? 0.55 + 0.05 * sin(gT * 0.4) * sway : 2.45 + 0.05 * sin(gT * 0.35) * sway)) * lp;
        float len = 0.2 * flowerSize * (1.0 - 0.15 * gRest);
        float wl = 0.045 * flowerSize;
        float xx = lp.x / len;
        float halfW = wl * sin(PI * clamp(xx, 0.0, 1.0));
        float dl = abs(lp.y) - halfW;
        dl = max(dl, max(-lp.x, lp.x - len));
        bool outline = mod(floor(ph) + fi, 2.0) < 1.0;
        float lv = outline ? smoothstep(0.004 + aa, 0.004 - aa, abs(dl)) : smoothstep(aa, -aa, dl);
        leaf = max(leaf, lv);
    }
    vec3 c = stemColor.rgb;
    float aStem = max(stem, leaf);
    vec4 res = vec4(c * aStem, aStem);
    // head on top, with 3D shading added later
    res = mix(res, vec4(col, 1.0), head);
    res.rgb += glow * (1.0 - head);
    res.a = max(res.a, clamp(lum(glow), 0.0, 1.0));
    return res;
}

// ── the garden: more flowers on stems with tall blade leaves, behind the hero ──
vec4 garden(vec2 p) {
    float N = floor(gardenCount);
    vec4 acc = vec4(0.0);
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float aa = 1.5 / RENDERSIZE.y;
    float vis = gardenCount * (0.35 + 0.65 * gLift + 0.2 * gDrop);
    float saveRH = gRH;
    float ph = TIME / morphPeriod;
    for (int i = 0; i < 10; i++) {
        float fi = float(i);
        if (fi >= N) break;
        float hi = h11(fi * 7.31 + 2.0);
        float grow = smoothstep(fi, fi + 1.0, vis);
        if (grow < 0.01) continue;
        float depth = (fi + 0.5) / max(N, 1.0);
        float x = (fract(fi * 0.618 + 0.31) - 0.5) * 0.92 * aspect + 0.02 * sin(gT * 0.5 + fi * 1.7) * sway;
        float yH = -0.12 + 0.5 * h11(fi * 3.3 + 1.0);
        vec2 H = vec2(x, yH);
        float RH = (0.06 + 0.09 * depth) * flowerSize * grow * (1.0 + 0.04 * bassP() * audioReact);
        // stem: near-vertical line from the head to the bottom of the frame
        float sx = x + 0.04 * (hi - 0.5) * (H.y - p.y) + 0.01 * sin(p.y * 6.0 + gT * 0.4 + fi);
        float dstem = abs(p.x - sx) - 0.0022 * (0.7 + depth);
        float stem = smoothstep(aa, -aa, dstem) * step(p.y, H.y);
        float stemType = floor(h11(fi * 5.1) * 3.0);
        vec3 sc = stemType < 0.5 ? vec3(0.12, 0.13, 0.32) : stemType < 1.5 ? stemColor.rgb : vec3(1.0, 0.5, 0.3);
        // blade leaves: long tapering curves (ref: tulip poster)
        float blade = 0.0; vec3 bc = vec3(0.0);
        for (int b = 0; b < 2; b++) {
            float fb = float(b);
            float side = fb < 0.5 ? -1.0 : 1.0;
            float top = H.y - 0.05 - 0.3 * h11(fi + fb * 3.0);
            float t = (p.y + 0.5) / max(top + 0.5, 0.05);
            if (t < 0.0 || t > 1.0) continue;
            float cx = sx + side * (0.015 + 0.07 * t * t * (0.5 + h11(fi * 2.0 + fb))) + 0.015 * sin(gT * 0.4 + fi + fb) * t;
            float w = (0.012 + 0.028 * depth) * pow(sin(PI * t), 0.8) * (1.0 - 0.6 * t);
            float m = smoothstep(aa, -aa, abs(p.x - cx) - w) * bladeLeaves;
            if (m > blade) {
                blade = m;
                float shade = 0.5 + 0.5 * sin(t * 4.0 + fi + fb * 2.0);
                bc = mix(vec3(0.02, 0.08, 0.05), mix(vec3(0.28, 0.45, 0.28), vec3(0.62, 0.65, 0.35), h11(fi + fb * 7.0)), shade);
            }
        }
        // the head: its own species, morphing on its own schedule
        float k0 = mod(floor(ph * 0.7 + hi * 10.0) + fi * 3.0, 10.0);
        float k1 = mod(k0 + 3.0, 10.0);
        float fm = smoothstep(0.7, 1.0, fract(ph * 0.7 + hi * 10.0));
        gRH = RH;
        gHue = hi + fi * 0.37;
        vec2 q = rot(0.25 * sin(gT * 0.3 + fi)) * (p - H);
        vec3 cA, gA, cB, gB;
        float d = species(k0, q, cA, gA);
        vec3 col = cA; vec3 glo = gA;
        if (fm > 0.001) { float dB = species(k1, q, cB, gB); d = mix(d, dB, fm); col = mix(cA, cB, fm); glo = mix(gA, gB, fm); }
        float head = smoothstep(aa, -aa, d);
        float puff = smoothstep(0.0, 0.3 * RH, -d);
        col *= 0.6 + 0.5 * puff;
        vec2 hl = q / max(RH, 1e-4) - vec2(-0.3, 0.35);
        col += vec3(1.0) * exp(-dot(hl, hl) * 6.0) * 0.25 * puff;
        float haze = 0.45 + 0.55 * depth;
        vec4 layer = vec4(bc, blade);
        layer = mix(layer, vec4(sc, 1.0), stem);
        layer = mix(layer, vec4(col, 1.0), head);
        layer.rgb *= haze;
        acc.rgb = mix(acc.rgb, layer.rgb, layer.a) + glo * haze * 0.6 * (1.0 - head);
        acc.a = max(acc.a, max(layer.a, lum(glo) * 0.5));
    }
    gRH = saveRH;
    gHue = 0.0;
    return acc;
}

// plasma-like X-ray heat colour (indigo → magenta → red → orange → yellow → lime)
vec3 heat(float v) {
    v = clamp(v, 0.0, 1.0);
    vec3 c = mix(vec3(0.18, 0.15, 0.5), vec3(0.75, 0.1, 0.55), smoothstep(0.0, 0.25, v));
    c = mix(c, vec3(0.95, 0.15, 0.1), smoothstep(0.25, 0.45, v));
    c = mix(c, vec3(1.0, 0.6, 0.05), smoothstep(0.45, 0.65, v));
    c = mix(c, vec3(1.0, 0.95, 0.2), smoothstep(0.65, 0.8, v));
    c = mix(c, vec3(0.5, 1.0, 0.35), smoothstep(0.8, 1.0, v));
    return c;
}
// 3x5 bitmap digits
float digitBits(float d) {
    if (d < 0.5) return 31599.0; if (d < 1.5) return 9362.0; if (d < 2.5) return 29671.0; if (d < 3.5) return 29647.0;
    if (d < 4.5) return 23497.0; if (d < 5.5) return 31183.0; if (d < 6.5) return 31215.0; if (d < 7.5) return 29330.0;
    if (d < 8.5) return 31727.0; return 31695.0;
}
float glyph(vec2 l, float d) {             // l in glyph cells (0..3, 0..5)
    if (l.x < 0.0 || l.y < 0.0 || l.x >= 3.0 || l.y >= 5.0) return 0.0;
    float idx = floor(l.x) + floor(l.y) * 3.0;
    return mod(floor(digitBits(d) / pow(2.0, idx)), 2.0);
}
// a number readout like "0.4286" at origin o (bottom-left), cell size s (pixels)
float readout(vec2 fc, vec2 o, float s, float val) {
    vec2 l = (fc - o) / s;
    if (l.y < 0.0 || l.y >= 5.0 || l.x < 0.0 || l.x >= 24.0) return 0.0;
    float ci = floor(l.x / 4.0);
    vec2 g = vec2(mod(l.x, 4.0), l.y);
    if (ci < 0.5) return glyph(g, floor(val));
    if (ci < 1.5) return step(abs(g.x - 1.0), 0.5) * step(g.y, 1.0);          // decimal point
    float dgt = mod(floor(fract(val) * pow(10.0, ci - 1.0)), 10.0);
    return glyph(g, dgt);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 p = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    gT = TIME * 0.6 + (audioMidTime * 0.35 + audioBassTime * 0.25) * ar + 1.2 * gBuild * gBuild;
    // overlay step clock (squares, mosh refresh): additive, scaled only by the user slider.
    // overlaySpeed = 1 ≈ the original pace at medium energy.
    gStep = floor(overlaySpeed * (TIME * 1.3 + audioMidTime * 1.0 * ar + audioBassTime * 0.4 * ar) + 4.0 * gBuild * gBuild + 2.0 * gLift);
    // tracker clock: continuous, so the boxes can glide. blobSpeed = 1 ≈ the original hop rate.
    gBT = blobSpeed * (TIME * 1.3 + audioMidTime * 1.0 * ar + audioBassTime * 0.4 * ar) + 1.5 * gBuild * gBuild + 0.8 * gLift;
    float swayX = (0.06 * sin(gT * 0.45) + 0.025 * sin(gT * 1.1)) * sway;
    gH = vec2(0.04 + swayX, 0.14 + 0.02 * sin(gT * 0.6));
    gRH = 0.3 * flowerSize * (0.82 + 0.25 * gLift + 0.2 * gDrop - 0.1 * gBuild) * (1.0 + 0.05 * bassP() * ar);
    gSpinA = gT * 0.25 * spin + 0.6 * gBuild * gBuild + 0.15 * sin(gT * 0.3);

    if (PASSINDEX == 0) {
        // ── the flower, lit like a 3D object ──
        float d;
        vec4 Gd = garden(p);
        gHue = floor(TIME / morphPeriod) * 0.37;
        vec4 F = flower(p, d);
        if (d < 0.0) {
            float e = 1.0 / R.y;
            float dx, dy;
            vec4 Fx = flower(p + vec2(e * 2.0, 0.0), dx);
            vec4 Fy = flower(p + vec2(0.0, e * 2.0), dy);
            vec2 g = vec2(dx - d, dy - d) / (e * 2.0);
            float hgt = smoothstep(0.0, 0.08 * gRH, -d);
            vec3 n = normalize(vec3(g * (1.0 - hgt) * 1.6, 1.0));
            vec3 L = normalize(vec3(-0.5, 0.6, 0.7));
            float dif = 0.55 + 0.45 * clamp(dot(n, L), 0.0, 1.0);
            float spec = pow(clamp(dot(reflect(-L, n), vec3(0.0, 0.0, 1.0)), 0.0, 1.0), 24.0);
            vec3 c = F.rgb * dif + spec * 0.5;
            // chrome / iridescent rim
            float rim = exp(d / (0.01 * gRH));
            vec3 irid = 0.5 + 0.5 * cos(TAU * (atan(p.y - gH.y, p.x - gH.x) / TAU + gT * 0.05 + vec3(0.0, 0.33, 0.67)));
            c = mix(c, irid * 1.2, rim * 0.55);
            // shimmer sweep across the head
            vec2 sd = normalize(vec2(1.0, 0.6));
            float band = exp(-pow((dot(p - gH, sd) / gRH) * 3.0 - 2.5 * sin(gT * 0.7 + 3.0 * gBuild), 2.0) * 4.0);
            c += vec3(1.0) * band * 0.35 * shimmer * (0.6 + 0.6 * gLift + highP() * ar);
            F.rgb = c;
        }
        // garden behind the hero
        F.rgb = mix(Gd.rgb, F.rgb, clamp(F.a, 0.0, 1.0));
        F.a = max(F.a, Gd.a);
        // riso grain: speckled print texture on everything painted
        float gr = h12(gl_FragCoord.xy + floor(TIME * 12.0) * 7.0);
        F.rgb *= 1.0 - risoGrain * 0.35 * (gr - 0.5) * 2.0 * step(0.01, F.a);
        gl_FragColor = F;
        return;
    }

    if (PASSINDEX == 1) {
        // ── datamosh trails around the flower ──
        vec4 cur = texture2D(flowerBuf, uv);
        float MB = 14.0;
        vec2 mb = floor(fc / MB);
        vec2 rel = (mb * MB - 0.5 * R) / R.y - gH;
        vec2 swirl = normalize(vec2(-rel.y, rel.x) + 1e-4) * 0.6 + normalize(rel + 1e-4) * 0.8;
        vec2 jit = vec2(vn2(mb * 0.2 + gT * 0.3), vn2(mb * 0.2 + 7.0 - gT * 0.25)) - 0.5;
        vec2 mv = floor((swirl + jit * 2.0) * (1.5 + 3.0 * gLift + 4.0 * gBuild) * min(overlaySpeed, 1.5) + 0.5);
        vec4 prev = texture2D(moshBuf, (fc - mv) / R);
        float m = moshAmt * (0.75 + 0.2 * gLift + 0.25 * gDrop);
        float refresh = step(m, h12(mb + vec2(gStep, 3.0)));
        vec4 o = prev * (0.965 - 0.02 * gRest);
        o = mix(o, cur, max(refresh * 0.5, cur.a * 0.7));
        if (FRAMEINDEX < 2) o = cur;
        gl_FragColor = o;
        return;
    }

    // ── composite: trails + crisp flower + data layer ──
    vec3 col = backdrop(uv);
    vec4 trail = texture2D(moshBuf, uv);
    col += trail.rgb * 0.55 * moshAmt;
    // RGB split on the flower (prism look, big on the drop)
    float split = (rgbSplit * (0.003 + 0.006 * gLift) + 0.02 * gDrop) * R.y;
    vec4 F = texture2D(flowerBuf, uv);
    float Fr = texture2D(flowerBuf, uv + vec2(split, 0.0) / R).r;
    float Fb = texture2D(flowerBuf, uv - vec2(split, 0.0) / R).b;
    vec3 fl = vec3(Fr, F.g, Fb);
    col = mix(col, fl, clamp(F.a, 0.0, 1.0));
    col += fl * (1.0 - clamp(F.a, 0.0, 1.0)) * 0.6;
    float alpha = clamp(F.a + lum(trail.rgb) * 0.5, 0.0, 1.0);

    // translucent blend-mode squares hopping over stem and flower (ref: green / pink / red squares)
    float nSq = floor(3.0 + 11.0 * squares * (0.35 + 0.65 * gLift));
    for (int i = 0; i < 14; i++) {
        float fi = float(i);
        if (fi >= nSq) break;
        float ep = floor(gStep * (0.08 + 0.05 * h11(fi)) + h11(fi + 3.0) * 5.0);
        vec2 hh = h21(fi * 7.3 + ep * 1.7);
        // squares cluster along the stem and around the head
        float along = hh.x;
        vec2 c0 = mix(vec2(-0.05, -0.45), gH, along) + (hh.y - 0.5) * vec2(0.45, 0.25) * (0.6 + along);
        float sz = (0.025 + 0.06 * h11(fi * 3.1 + ep)) * (0.7 + 0.5 * gLift);
        vec2 dd = abs(p - c0);
        if (max(dd.x, dd.y) < sz) {
            float k = floor(h11(fi + ep * 2.3) * 4.0);
            vec3 sc = k < 0.5 ? vec3(0.2, 0.75, 0.4) : k < 1.5 ? vec3(0.95, 0.45, 0.55) : k < 2.5 ? vec3(0.7, 0.12, 0.18) : vec3(0.3, 0.4, 0.55);
            float mode = floor(h11(fi * 1.9 + ep) * 4.0);
            vec3 blended;
            if (mode < 0.5) blended = 1.0 - (1.0 - col) * (1.0 - sc);           // screen
            else if (mode < 1.5) blended = col * sc * 1.6;                     // multiply
            else if (mode < 2.5) blended = abs(col - sc);                      // difference
            else blended = mix(col, sc, 0.75);                                 // normal
            col = mix(col, blended, 0.8);
            alpha = max(alpha, 0.8);
        }
    }

    // blob tracking boxes with X-ray heat inside, dashed links and readouts
    float nB = floor(1.0 + 7.0 * trackers * (0.3 + 0.7 * gLift + 0.4 * gBuild));
    vec2 prevC = vec2(0.0);
    float lineMask = 0.0;
    for (int i = 0; i < 9; i++) {
        float fi = float(i);
        if (fi >= nB) break;
        // each box holds on a blob, then glides (eased) to the next one
        float bph = gBT * (0.12 + 0.08 * h11(fi * 2.0)) + h11(fi) * 3.0;
        float ep = floor(bph);
        float gl = smoothstep(0.35, 1.0, fract(bph));
        gl = gl * gl * (3.0 - 2.0 * gl);                                        // extra-soft ease in/out
        vec2 hA = h21(fi * 3.3 + ep * 0.77), hB = h21(fi * 3.3 + (ep + 1.0) * 0.77);
        // lock onto a blob of the flower: polar spot on the head, or a point on the stem
        vec2 cA = hA.x < 0.75 ? gH + rot(hA.y * TAU) * vec2(gRH * (0.25 + 0.5 * h11(fi + ep)), 0.0)
                             : mix(vec2(0.0, -0.35), gH, hA.y * 0.6);
        vec2 cB = hB.x < 0.75 ? gH + rot(hB.y * TAU) * vec2(gRH * (0.25 + 0.5 * h11(fi + ep + 1.0)), 0.0)
                             : mix(vec2(0.0, -0.35), gH, hB.y * 0.6);
        vec2 c0 = mix(cA, cB, gl);
        // tracking wobble: smooth noise instead of per-step jumps
        c0 += (vec2(vn2(vec2(fi * 7.1, gBT * 1.5)), vn2(vec2(fi * 3.9 + 40.0, gBT * 1.5))) - 0.5) * 0.008 * (1.0 + 3.0 * gBuild);
        vec2 hs = mix(vec2(0.025 + 0.045 * h11(fi * 5.0 + ep), 0.025 + 0.045 * h11(fi * 7.0 + ep)),
                      vec2(0.025 + 0.045 * h11(fi * 5.0 + ep + 1.0), 0.025 + 0.045 * h11(fi * 7.0 + ep + 1.0)), gl);
        vec2 dd = abs(p - c0);
        float inside = step(max(dd.x / hs.x, dd.y / hs.y), 1.0);
        float px1 = 1.0 / R.y;
        float edge = inside * step(1.0 - 1.4 * px1 / min(hs.x, hs.y), max(dd.x / hs.x, dd.y / hs.y));
        if (inside > 0.5) {
            // X-ray: luminance + edges of the flower, mapped to heat
            vec2 e = 1.5 / R;
            float l0 = lum(texture2D(flowerBuf, uv).rgb);
            float lx = lum(texture2D(flowerBuf, uv + vec2(e.x, 0.0)).rgb);
            float ly = lum(texture2D(flowerBuf, uv + vec2(0.0, e.y)).rgb);
            float ed = length(vec2(lx - l0, ly - l0)) * 6.0;
            float v = l0 * 0.75 + ed + 0.15 * sin(p.y * 300.0 + gT * 6.0) * F.a;
            vec3 xr = heat(v * (0.8 + 0.4 * gLift));
            col = mix(col, xr, xray * 0.92);
            alpha = 1.0;
        }
        col = mix(col, vec3(1.0), edge);
        // readout under the box
        float val = mix(h11(fi * 13.0 + ep), h11(fi * 13.0 + ep + 1.0), gl) * 1.9;
        float s = max(floor(R.y / 360.0), 1.0);
        vec2 o = (c0 + vec2(-hs.x * 0.6, -hs.y * 0.25)) * R.y + 0.5 * R;
        float txt = readout(fc, o, s, val) * readouts;
        col = mix(col, vec3(1.0), txt);
        // dashed link to the previous box
        if (fi > 0.5) {
            float dl = sdSeg(p, prevC, c0);
            vec2 ba = c0 - prevC;
            float tt = clamp(dot(p - prevC, ba) / dot(ba, ba), 0.0, 1.0) * length(ba);
            float dash = step(0.5, fract(tt / 0.012 - gBT * 0.6));
            lineMask = max(lineMask, smoothstep(1.2 * px1, 0.3 * px1, dl) * dash * readouts * 0.85);
        }
        prevC = c0;
    }
    col = mix(col, vec3(1.0), lineMask);
    alpha = max(alpha, lineMask);

    // drop flash, beat lift
    col += vec3(1.0) * gDrop * gDrop * 0.2 * alpha;
    col *= 0.8 + 0.3 * gLift + 0.1 * audioBeatPulse * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
