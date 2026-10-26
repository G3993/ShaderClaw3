/*{
  "DESCRIPTION": "Perlin — a living study of noise, texture and grain. Animated Perlin gradient noise is layered into fractal fields and folded back into itself (domain warping), so it boils, flows and curls like smoke in slow motion, then rendered through six textures that morph into each other: Smoke (soft warped clouds with deep shadows), Marble (turbulent veins cutting through polished stone), Topographic (the noise drawn as glowing contour lines, like a map of shifting terrain), Risograph (halftone dots sized by the noise, printed in two misregistered inks on paper), Paper Fibre (felted fibres and pulp flecks drifting through the light), and Film Stock (heavy silver-halide grain, scratches, dust, gate weave and flicker). Everything is finished with a tunable photographic grain — fine or coarse, mono or colour, frozen or crawling. Bass swells and darkens the fields, mids speed up the boiling, highs sharpen and sparkle the grain, beats push a ripple through the warp. At rest it drifts slowly with soft, sparse grain; during a build the noise tightens to finer detail, swirls faster and the grain gets hotter; on the drop the warp snaps outward in a bright shockwave and the grain flashes, then the peak boils at full contrast. Every texture is lit as a 3D relief surface (3D Relief control; 0 = flat print): soft volumetric clouds for Smoke, clear-coated polished stone with a studio reflection for Marble, embossed terraces with crisp contour risers for Topographic, raised glossy ink domes casting tiny shadows for Risograph, proud fibres for Paper, and a halation/lens glow for Film — key and rim lights, fresnel specular, height AO and a soft focus falloff toward the frame edge. Underneath runs a real fluid simulation: twelve point vortices in counter-rotating pairs push each other around (Biot–Savart), carry inertia, ride a curl-noise breeze, rise or sink with buoyancy and bounce off the frame, and every texture is carried by the swirl they wind into it — kicks spin up a pair and send it lunging along its own jet, the drop fires a radial blast on a damped spring that shoots out, rebounds and settles, and between hits the twist relaxes home (the grain stays locked to the screen).",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Texture", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "textureSel",  "LABEL": "Texture", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6], "LABELS": ["Auto Morph", "Smoke", "Marble", "Topographic", "Risograph", "Paper Fibre", "Film Stock"], "GROUP": "Noise" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 12.0, "GROUP": "Noise" },
    { "NAME": "noiseScale",  "LABEL": "Scale",            "TYPE": "float", "MIN": 0.4, "MAX": 4.0, "DEFAULT": 1.4, "GROUP": "Noise" },
    { "NAME": "octaves",     "LABEL": "Detail (octaves)", "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 7.0, "DEFAULT": 5.0, "GROUP": "Noise" },
    { "NAME": "warpAmt",     "LABEL": "Domain Warp",      "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.4, "GROUP": "Noise" },
    { "NAME": "flowSpeed",   "LABEL": "Boil Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Noise" },
    { "NAME": "drift",       "LABEL": "Drift",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "GROUP": "Noise" },
    { "NAME": "relief",      "LABEL": "3D Relief",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Look" },
    { "NAME": "contrast",    "LABEL": "Contrast",         "TYPE": "float", "MIN": 0.5, "MAX": 2.5, "DEFAULT": 1.2, "GROUP": "Look" },
    { "NAME": "inkA",        "LABEL": "Ink / Shadow",     "TYPE": "color", "DEFAULT": [0.04, 0.03, 0.05, 1.0], "GROUP": "Look" },
    { "NAME": "inkB",        "LABEL": "Colour",           "TYPE": "color", "DEFAULT": [0.95, 0.22, 0.32, 1.0], "GROUP": "Look" },
    { "NAME": "inkC",        "LABEL": "Highlight / Paper","TYPE": "color", "DEFAULT": [0.96, 0.93, 0.86, 1.0], "GROUP": "Look" },
    { "NAME": "lineCount",   "LABEL": "Contour Lines",    "TYPE": "float", "MIN": 4.0, "MAX": 40.0, "DEFAULT": 16.0, "GROUP": "Look" },
    { "NAME": "dotSize",     "LABEL": "Halftone Size (px)","TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 20.0, "DEFAULT": 7.0, "GROUP": "Look" },
    { "NAME": "grainAmt",    "LABEL": "Grain",            "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.55, "GROUP": "Grain" },
    { "NAME": "grainSize",   "LABEL": "Grain Size",       "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 4.0, "DEFAULT": 1.3, "GROUP": "Grain" },
    { "NAME": "grainColour", "LABEL": "Colour Grain",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Grain" },
    { "NAME": "grainSpeed",  "LABEL": "Grain Crawl",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Grain" },
    { "NAME": "vignette",    "LABEL": "Vignette",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Grain" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.03, 0.04, 1.0], "GROUP": "Background" },
    { "NAME": "bgMix",       "LABEL": "Background Show-Through", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" },
    { "NAME": "physicsAmt",  "LABEL": "Physics Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Physics" },
    { "NAME": "gravity",     "LABEL": "Buoyancy / Gravity","TYPE": "float", "AUDIO": false, "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Physics" },
    { "NAME": "damping",     "LABEL": "Damping",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Physics" },
    { "NAME": "bounceAmt",   "LABEL": "Bounce",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Physics" },
    { "NAME": "springBack",  "LABEL": "Spring Back",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Physics" },
    { "NAME": "swirlAmt",    "LABEL": "Swirl (Vorticity)","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Physics" },
    { "NAME": "kickForce",   "LABEL": "Kick Force",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "resetPhysics","LABEL": "Reset Physics",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Physics" }
  ],
  "PASSES": [
    { "TARGET": "simBuf", "PERSISTENT": true, "FLOAT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec3  h33(vec3 p)  { p = vec3(dot(p, vec3(127.1, 311.7, 74.7)), dot(p, vec3(269.5, 183.3, 246.1)), dot(p, vec3(113.5, 271.9, 124.6))); return fract(sin(p) * 43758.5453123) * 2.0 - 1.0; }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

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

// ── 3D Perlin gradient noise (quintic fade), range ≈ -1..1 ──────────────
float perlin(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    vec3 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    float n000 = dot(h33(i), f);
    float n100 = dot(h33(i + vec3(1, 0, 0)), f - vec3(1, 0, 0));
    float n010 = dot(h33(i + vec3(0, 1, 0)), f - vec3(0, 1, 0));
    float n110 = dot(h33(i + vec3(1, 1, 0)), f - vec3(1, 1, 0));
    float n001 = dot(h33(i + vec3(0, 0, 1)), f - vec3(0, 0, 1));
    float n101 = dot(h33(i + vec3(1, 0, 1)), f - vec3(1, 0, 1));
    float n011 = dot(h33(i + vec3(0, 1, 1)), f - vec3(0, 1, 1));
    float n111 = dot(h33(i + vec3(1, 1, 1)), f - vec3(1, 1, 1));
    return 1.6 * mix(mix(mix(n000, n100, u.x), mix(n010, n110, u.x), u.y),
                     mix(mix(n001, n101, u.x), mix(n011, n111, u.x), u.y), u.z);
}
// fractal sum; ridged = 1 gives turbulence |n|
float gOct, gOctCap, gWarpK;
float fbm(vec3 p, float ridged) {
    float s = 0.0, a = 0.5, norm = 0.0;
    for (int i = 0; i < 7; i++) {
        if (float(i) >= min(gOct, gOctCap)) break;
        float n = perlin(p);
        n = mix(n, 1.0 - 2.0 * abs(n), ridged);
        s += a * n; norm += a;
        p = vec3(rot(0.6) * p.xy, p.z) * 2.03 + vec3(1.7, -2.3, 0.9);
        a *= 0.5;
    }
    return s / max(norm, 1e-3);
}

float gT;
// the warped field: q (first warp) and r (second warp) give flowing structure
float field(vec2 p, float ridged, out vec2 warp) {
    float z = gT;
    vec2 q = vec2(fbm(vec3(p, z), 0.0), fbm(vec3(p + vec2(5.2, 1.3), z * 1.1), 0.0));
    float wk = warpAmt * gWarpK;
    vec2 r = vec2(fbm(vec3(p + 2.0 * q * wk + vec2(1.7, 9.2), z * 0.9), 0.0),
                  fbm(vec3(p + 2.0 * q * wk + vec2(8.3, 2.8), z * 1.2), 0.0));
    warp = r;
    return fbm(vec3(p + 2.0 * r * wk, z * 0.7), ridged);
}

vec3 ramp3(float t) {
    t = clamp(t, 0.0, 1.0);
    return t < 0.5 ? mix(inkA.rgb, inkB.rgb, smoothstep(0.0, 0.5, t)) : mix(inkB.rgb, inkC.rgb, smoothstep(0.5, 1.0, t));
}

// one texture at point p (noise space) and pixel fc.
// The warped field is evaluated from ONE call site (in a loop) to keep the shader small.
// p0 = undisplaced noise coords; dsp = fluid displacement (noise units) at this pixel.
// Relief: the loop also evaluates the field at +x / +y screen neighbours (same call site) → a true
// finite-difference gradient of the full warped field → normals for one shared lighting block.
float gDK, gRel, gEs, gFocus;
vec2 gDC0, gDC1, gDX, gDY, gSp;
vec3 textureLook(float k, vec2 p0, vec2 fc, float px, vec2 dsp) {
    vec2 p = p0 - dsp;                                              // material coordinates, carried by the flow
    // per-texture sampling setup
    float scl = k < 0.5 ? 1.0 : k < 1.5 ? 0.45 : k < 2.5 ? 0.38 : k < 3.5 ? 1.0 : k < 4.5 ? 0.6 : 0.9;
    float ridged = 0.0;
    gWarpK  = k < 0.5 ? 1.0 : k < 1.5 ? 0.45 : k < 2.5 ? 0.35 : k < 3.5 ? 0.8 : k < 4.5 ? 0.6 : 0.9;
    gOctCap = k < 0.5 ? 7.0 : k < 1.5 ? 4.0 : k < 2.5 ? 3.0 : k < 3.5 ? 4.0 : 7.0;
    bool riso = k > 2.5 && k < 3.5;
    bool topo = k > 1.5 && k < 2.5;
    bool fd = topo || (gRel > 0.001 && k < 1.5);                    // smoke / marble / topo get field normals
    float cell = max(dotSize, 2.0);
    vec2 P0 = p * scl, P1 = P0, P2 = P0;
    vec2 gA0 = vec2(0.0), gA1 = vec2(0.0);
    float eN = gEs * px * RENDERSIZE.y;                             // FD step in noise units (gEs is in screen-height units)
    if (fd) {
        P1 = (p0 + vec2(eN, 0.0) - gDX) * scl;
        P2 = (p0 + vec2(0.0, eN) - gDY) * scl;
    }
    if (riso) {
        for (int i = 0; i < 2; i++) {
            float fi = float(i);
            vec2 off = fi < 0.5 ? vec2(0.0) : vec2(2.5, -1.5) + 1.5 * vec2(sin(gT * 7.0), cos(gT * 5.0));   // misregistration
            float ang = fi < 0.5 ? 0.26 : 1.31;
            vec2 g = rot(ang) * (fc + off) / cell;
            vec2 id = floor(g) + 0.5;
            vec2 cpix = rot(-ang) * id * cell - off;
            vec2 cp = (cpix - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y * noiseScale + (p0 - (fc - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y * noiseScale);
            cp -= i == 0 ? gDC0 : gDC1;                                     // each dot cell rides the flow at its centre (dots stay round)
            if (i == 0) { P0 = cp; gA0 = g; } else { P1 = cp + 0.37; gA1 = g; }
        }
    }
    // Smoke shades from a softer, lower-octave copy of the same field (volumetric billows, not crunch):
    // j = 0 full-detail colour sample, j = 1..3 low-octave centre / +x / +y.  One field() call site.
    bool soft = fd && k < 0.5;
    float octFull = gOct;
    float F0 = 0.0, F1 = 0.0, F2 = 0.0, F3 = 0.0;
    vec2 w = vec2(0.0), wtmp;
    for (int j = 0; j < 4; j++) {
        if (j == 1 && !riso && !fd) break;
        if (j == 2 && !fd) break;
        if (j == 3 && !soft) break;
        gOct = soft && j > 0 ? max(octFull - 3.0, 2.0) : octFull;
        vec2 Pj = soft ? (j == 0 ? P0 : j == 1 ? P0 : j == 2 ? P1 : P2) : (j == 0 ? P0 : j == 1 ? P1 : P2);
        float fv = field(Pj, ridged, wtmp);
        if (j == 0) { F0 = fv; w = wtmp; } else if (j == 1) F1 = fv; else if (j == 2) F2 = fv; else F3 = fv;
    }
    gOct = octFull;
    float f = F0;
    vec2 G = !fd ? vec2(0.0) : soft ? vec2(F2 - F1, F3 - F1) / max(gEs, 1e-6) : vec2(F1 - F0, F2 - F0) / max(gEs, 1e-6);    // d field / d screen-height unit

    // material outputs for the shared lighting block
    vec3 c;
    vec2 gh = vec2(0.0);                // height gradient (screen units → normal = (-gh, 1))
    float ao = 1.0, shin = 16.0, specK = 0.1, F0r = 0.04, rimK = 0.25, wrap = 0.3, envK = 0.0, sss = 0.0;
    if (k < 0.5) {                                                  // Smoke — volumetric soft-lit cloud
        float v = smoothstep(0.05, 0.95, 0.5 + 0.5 * f * contrast);
        c = ramp3(v * 0.85 + 0.1 * length(w));
        c *= 0.75 + 0.45 * smoothstep(0.2, 0.9, v);
        gh = G * 0.5 * contrast * 0.09;
        ao = mix(0.45, 1.08, smoothstep(-0.45, 0.45, F1 * contrast));   // valleys of the soft field sit in shadow              // thin gaps between billows sit in shadow
        shin = 6.0; specK = 0.05; rimK = 0.55; wrap = 0.9; sss = (1.0 - v) * 0.6;
    } else if (k < 1.5) {                                           // Marble — polished stone, clear coat
        float ph1 = (p.x + p.y * 0.4) * 3.5 + f * 14.0 * contrast + w.x * 5.0;
        float sv = sin(ph1);
        float v = pow(1.0 - abs(sv), 7.0);
        float v2 = pow(1.0 - abs(sin((p.y - p.x * 0.3) * 2.5 + f * 18.0 + w.y * 4.0)), 26.0);
        vec3 stone = mix(inkC.rgb * 0.82, inkC.rgb, smoothstep(-0.5, 0.6, f));
        stone *= 0.94 + 0.06 * h12(floor(p * 300.0));                // fine crystal sparkle
        c = mix(stone, inkB.rgb, v * 0.85);
        c = mix(c, inkA.rgb, v2 * 0.9);
        // veins are very slightly etched into the polish
        float dv = -7.0 * pow(1.0 - abs(sv), 6.0) * sign(sv) * cos(ph1);
        gh = G * 0.03 - dv * (G * 14.0 * contrast) * 0.0005;
        ao = 1.0 - 0.18 * v2;
        shin = 90.0; specK = 1.4; F0r = 0.06; rimK = 0.2; wrap = 0.15; envK = 1.0;
    } else if (topo) {                                              // Topographic — embossed terraces
        float gainH = 1.8 * contrast * 0.8;
        float h = f * gainH + 0.5;                                  // stretch the field so many contours appear (unclamped: no flat plateaus)
        float n = floor(lineCount);
        float lv = h * n;
        float slope = max(length(G) * gainH * n / RENDERSIZE.y, 1e-4);   // contour units per pixel
        float fl = fract(lv);
        float dpx = (0.5 - abs(fl - 0.5)) / slope;                  // distance to the nearest contour, in pixels
        float lw = 0.7 + 0.6 * gLift;
        float line = smoothstep(lw + 0.8, lw - 0.2, dpx);
        float major = mod(floor(lv + 0.5), 5.0) < 0.5 ? 1.0 : 0.0;
        float tier = floor(lv + 0.5) / n;
        vec3 base = inkA.rgb + 0.06 * clamp(h, 0.0, 1.0);
        base = mix(base, base + inkB.rgb * 0.16 * clamp(tier, 0.0, 1.2), gRel);  // higher terraces catch more light
        c = mix(base, mix(inkB.rgb, inkC.rgb, major), line * (0.55 + 0.45 * major));
        // step faces: the crisp line itself is the riser of each terrace
        vec2 gd = G / max(length(G), 1e-5);
        float riser = smoothstep(lw + 2.5 * max(1.0, RENDERSIZE.y / 540.0), 0.0, dpx);   // bevel scales with resolution
        gh = gd * riser * (0.9 + 0.6 * major) + G * 0.004;
        // ambient occlusion at the foot of the next terrace up
        float dUp = (1.0 - fl) / slope * 1080.0 / RENDERSIZE.y;
        ao = 1.0 - 0.7 * exp(-dUp / 9.0) * (1.0 - line);
        shin = 48.0; specK = 0.45; rimK = 0.35; wrap = 0.2;
    } else if (riso) {                                              // Risograph — raised ink dots, two inks multiplied
        c = inkC.rgb;
        vec2 Ls = normalize(vec2(-0.55, 0.6));
        for (int i = 0; i < 2; i++) {
            float fi = float(i);
            float fv = i == 0 ? F0 : F1;
            vec2 g = i == 0 ? gA0 : gA1;
            float ang = fi < 0.5 ? 0.26 : 1.31;
            float tone = clamp(0.5 + 0.55 * fv * contrast + (fi - 0.5) * 0.15, 0.0, 1.0);
            float r = sqrt(tone) * 0.62;
            vec2 lc = fract(g) - 0.5;
            float dd = length(lc);
            float aa = 0.06 + 0.5 / cell;
            float dotm = smoothstep(r + aa, r - aa, dd);
            // drop shadow of the raised dot onto the paper (away from the light)
            float ds = length(fract(g + rot(ang) * Ls * 0.17 * gRel) - 0.5);
            float shd = smoothstep(r + 0.12, r - 0.08, ds) * (1.0 - dotm);
            c *= 1.0 - 0.38 * shd * gRel;
            vec3 ink = fi < 0.5 ? inkB.rgb : inkA.rgb + vec3(0.05, 0.1, 0.25);
            c = mix(c, c * ink * 1.05, dotm * 0.92);
            // dome normal (rotated back to screen space)
            float z = sqrt(max(r * r - dd * dd, 0.0));
            vec2 dg = rot(-ang) * (lc / max(z, 0.22 * r + 0.02)) * 0.9;
            gh = mix(gh, dg, dotm);
        }
        shin = 28.0; specK = 0.3; rimK = 0.3; wrap = 0.25;
    } else if (k < 4.5) {                                           // Paper fibre — felted relief
        c = mix(inkC.rgb * 0.92, inkC.rgb, 0.5 + 0.5 * f);
        float fib = 0.0;
        vec2 gf = vec2(0.0);
        for (int i = 0; i < 3; i++) {
            float fi = float(i);
            float fs = 6.0 + fi * 5.0;
            vec2 q = p * fs + w * 3.0 + fi * 13.1;
            vec2 id = floor(q), fq = fract(q) - 0.5;
            float a = h12(id) * TAU + w.x * 2.0;
            vec2 dir = vec2(cos(a), sin(a));
            float along = dot(fq, dir), across = dot(fq, vec2(-dir.y, dir.x));
            float xa = across + 0.15 * along * along;
            float sres = smoothstep(0.05, 0.0, abs(xa)) * smoothstep(0.5, 0.2, abs(along)) * step(0.55, h12(id + 7.0));
            float st = sres * (0.5 + 0.5 * h12(id + 3.0));
            if (st > fib) { fib = st; gf = vec2(-dir.y, dir.x) * clamp(xa / 0.05, -1.0, 1.0) * st; }
        }
        c = mix(c, mix(inkB.rgb, inkA.rgb, 0.3), fib * 0.55);
        vec2 fp = p * 40.0 + w * 6.0;
        vec2 fid = floor(fp);
        vec2 fo = fract(fp) - 0.5 - (vec2(h12(fid + 1.0), h12(fid + 2.0)) - 0.5) * 0.4;
        float fl = step(0.97, h12(fid)) * smoothstep(0.3, 0.12, length(fo));
        c = mix(c, inkA.rgb * 1.2, fl * 0.55);
        c *= 0.92 + 0.12 * smoothstep(-0.4, 0.6, f);
        // fibres stand proud of the sheet (rounded cross-section), flecks are little pits
        gh = gf * 2.4 - fo * fl * 3.0 + G * 0.0;
        ao = 1.0 - 0.12 * fl;
        shin = 10.0; specK = 0.06; rimK = 0.2; wrap = 0.5;
    } else {                                                        // Film stock: silver-halide clumps, scratches, dust, flicker
        float v = smoothstep(-0.6, 0.7, f * contrast);
        c = mix(inkA.rgb, inkC.rgb, v);
        float fr = floor(TIME * 24.0);
        float clump = h12(floor(fc / (2.0 * grainSize)) + fr * 3.1);
        c *= 0.86 + 0.28 * clump;
        float sx = h11(fr * 1.7);
        float scr = smoothstep(1.5, 0.0, abs(fc.x - sx * RENDERSIZE.x)) * step(0.55, h11(fr * 2.3));
        c = mix(c, inkC.rgb, scr * 0.6);
        float dust = step(0.9993, h12(floor(fc / 3.0) + fr));
        c = mix(c, inkA.rgb, dust);
        c *= 0.92 + 0.08 * h11(fr);
        // lens / emulsion depth glow: halation bleeding red around highlights + a soft projector bloom
        vec3 hal = vec3(1.0, 0.35, 0.18) * mix(vec3(1.0), inkB.rgb * 1.6, 0.5);
        c += hal * smoothstep(0.45, 1.0, v) * 0.22 * gRel;
        c = mix(c, c * 1.12 + 0.03 * inkC.rgb, gRel * exp(-dot(gSp, gSp) * 2.2));
        return c;
    }
    if (gRel < 0.001) return c;

    // ── shared 3D lighting: key + rim, glossy spec with fresnel, studio reflection, AO / translucency ──
    vec3 nrm = normalize(vec3(-gh * gRel, 1.0));
    vec3 V = normalize(vec3(-gSp * 0.45, 1.0));                      // gentle perspective → spec drifts across the frame
    vec3 L = normalize(vec3(-0.55, 0.6, 0.62));
    vec3 Lr = normalize(vec3(0.7, -0.45, 0.25));
    float ndl = dot(nrm, L);
    float dif = clamp((ndl + wrap) / (1.0 + wrap), 0.0, 1.0);
    float ndv = max(dot(nrm, V), 0.0);
    float fres = F0r + (1.0 - F0r) * pow(1.0 - ndv, 5.0);
    vec3 Hh = normalize(L + V);
    float spec = pow(max(dot(nrm, Hh), 0.0), shin) * specK * (0.35 + 2.5 * fres) * step(0.0, ndl);
    float rim = pow(1.0 - nrm.z, 1.2) * max(dot(nrm.xy, Lr.xy), 0.0) / max(length(nrm.xy), 1e-4) * rimK;
    rim += pow(1.0 - ndv, 3.0) * rimK * 0.4;
    vec3 Rf = reflect(-V, nrm);
    vec2 rp = Rf.xy / max(Rf.z, 0.2);                               // studio softbox seen in the reflection
    vec2 wb = abs(rot(0.5) * (rp - vec2(-0.17, 0.13))) - vec2(0.17, 0.06);
    float env = smoothstep(0.045, 0.0, length(max(wb, 0.0)) + min(max(wb.x, wb.y), 0.0)) * 0.85
              + 0.25 * smoothstep(-0.35, 0.35, rp.y - rp.x);
    vec3 keyC = vec3(1.0, 0.96, 0.9), rimC = mix(vec3(0.75, 0.85, 1.0), inkC.rgb, 0.5);
    float amb = k > 0.5 && k < 1.5 ? 0.3 : 0.42;
    vec3 lit = c * (amb * ao + 0.78 * dif * mix(1.0, ao, 0.6));
    lit += c * sss * pow(max(dot(-nrm.xy, L.xy), 0.0), 1.5) * 0.6;   // light through thin smoke
    lit += keyC * spec + rimC * rim * mix(vec3(1.0), c, 0.5) + vec3(env * envK * (0.18 + 1.6 * fres));
    lit = mix(lit, lit * 0.82, gFocus * 0.5);                       // out-of-focus edges recede
    return mix(c, lit, gRel);
}

// ── Physics: Lagrangian vortex-particle fluid ───────────────────────────
// 12 point vortices (6 counter-rotating pairs) live in a persistent state buffer laid out
// as a 4×8 grid of cells (resolution independent; read at cell centres):
//   rows 0-2 : vortex i = row*4+col → (pos.xy, vel.xy)   [screen-height units]
//   rows 3-5 : vortex aux           → (circulation Γ [rad/s at the core], material twist θ, -, 1)
//   row 6    : col 0 globals A      → (prev beat pulse, kick counter, blast e, blast ė)
//              col 1 globals B      → (valid marker = 1, -, -, -)
// Vortices induce velocity on each other (Biot–Savart, Lamb–Oseen cores), carry inertia, drift
// on a curl-noise breeze, feel buoyancy, collide softly and bounce off the frame. The image pass
// rebuilds the displacement analytically from the 12 vortices, so it stays perfectly smooth.
#define NV 12
vec2 cellUV(float col, float row) { return (vec2(col, row) + 0.5) / vec2(4.0, 8.0); }
vec4 vPos(float i) { return texture2D(simBuf, cellUV(mod(i, 4.0), floor(i / 4.0))); }
vec4 vAux(float i) { return texture2D(simBuf, cellUV(mod(i, 4.0), 3.0 + floor(i / 4.0))); }
float vSign(float i) { return mod(i, 2.0) < 0.5 ? 1.0 : -1.0; }
float vCore(float i) { return 0.09 + 0.08 * h11(i * 3.71 + 1.3); }
float gBlastE;

vec4 simStep(vec2 fc, vec2 R, float dt, float ar) {
    vec2 cc = fc / R * vec2(4.0, 8.0);
    vec2 cid = floor(cc);
    vec2 lp = (fract(cc) - 0.5) * R / vec2(4.0, 8.0);                // px from the cell centre
    vec4 self = texture2D(simBuf, fc / R);
    if (max(abs(lp.x), abs(lp.y)) > 3.0 || cid.y > 6.5) return self;  // only cell centres compute
    float asp = R.x / R.y;
    vec4 GA = texture2D(simBuf, cellUV(0.0, 6.0));
    vec4 GB = texture2D(simBuf, cellUV(1.0, 6.0));
    bool init = FRAMEINDEX < 2 || resetPhysics || abs(GB.x - 1.0) > 0.01 || !(abs(GA.x) + abs(GA.y) + abs(GA.z) + abs(GA.w) < 1e3);
    // beat onset (rising edge of the beat pulse) → kick one vortex pair
    float pulse = clamp(audioBeatPulse, 0.0, 1.0);
    bool onset = pulse > 0.3 && GA.x <= 0.3;
    float slot = mod(GA.y, 6.0);
    float kickAmt = kickForce * ar * (0.5 + 0.8 * bassP()) * (0.6 + 0.3 * gLift + 0.6 * gPeak + 0.3 * gBuild - 0.35 * gRest);
    float blast = smoothstep(0.55, 1.0, gDrop) * kickForce * (0.4 + 0.6 * ar);
    float spin0 = (0.25 + 0.25 * gLift + 0.9 * gBuild + 0.35 * gPeak - 0.15 * gRest) * (0.3 + 1.4 * swirlAmt);
    if (cid.y > 5.5) {
        if (cid.x > 0.5) return vec4(1.0, 0.0, 0.0, 1.0);
        if (init) return vec4(pulse, 0.0, 0.0, 0.0);
        // blast: damped spring (shock out, rebound, settle); a build pulls it in (tension)
        float w = TAU * 0.55, z = 0.32;
        float acc = -w * w * GA.z - 2.0 * z * w * GA.w + blast * 22.0 - 0.6 * gBuild * gBuild;
        float ed = GA.w + acc * dt;
        float e = clamp(GA.z + ed * dt, -0.25, 0.45);
        return vec4(pulse, mod(GA.y + (onset ? 1.0 : 0.0), 60.0), e, clamp(ed, -4.0, 4.0));
    }
    bool aux = cid.y > 2.5;
    float i = cid.x + 4.0 * (aux ? cid.y - 3.0 : cid.y);
    float sg = vSign(i);
    float pr = floor(i / 2.0);
    if (init) {
        vec2 pc = (vec2(h11(pr * 7.13 + 0.7), h11(pr * 3.37 + 9.1)) - 0.5) * vec2(asp * 0.75, 0.7);
        float a = h11(pr * 5.9 + 2.2) * TAU;
        vec2 pos = pc + sg * vec2(-sin(a), cos(a)) * 0.1;
        return aux ? vec4(sg * spin0, 0.0, 0.0, 1.0) : vec4(pos, 0.0, 0.0);
    }
    vec4 P = vPos(i), A = vAux(i);
    bool kicked = onset && abs(pr - slot) < 0.5;
    if (aux) {
        // circulation relaxes to a gentle baseline spin (viscous decay); kicks and the blast spin it up
        float G = A.x;
        G += (sg * spin0 - G) * (0.45 + 1.3 * damping) * (1.0 + gRest) * dt;
        if (kicked) G += sg * kickAmt * (0.8 + 1.6 * swirlAmt);
        G += sg * blast * (2.0 + 4.0 * swirlAmt) * dt * 4.0;
        G = clamp(G, -8.0, 8.0);
        // material twist: winds up with the spin, springs back home
        float kth = 0.3 + 1.6 * springBack * springBack;
        float th = clamp(A.y + (G - kth * A.y) * dt, -7.0, 7.0);
        return vec4(G, th, 0.0, 1.0);
    }
    // ── vortex body dynamics ──
    vec2 c = P.xy, u = P.zw;
    vec2 uind = vec2(0.0), rep = vec2(0.0), upair = vec2(0.0);
    for (int j = 0; j < NV; j++) {
        float fj = float(j);
        if (abs(fj - i) < 0.5) continue;
        vec2 cj = vPos(fj).xy;
        float Gj = vAux(fj).x;
        float rc2 = vCore(fj) * vCore(fj);
        vec2 d = c - cj;
        float r2 = dot(d, d);
        vec2 ui = Gj * rc2 * vec2(-d.y, d.x) / (r2 + rc2);          // Lamb–Oseen-softened Biot–Savart
        uind += ui;
        if (abs(fj - (i + sg)) < 0.5) upair = ui;
        rep += d / (r2 + 0.002) * exp(-r2 / 0.012);                  // soft body collision
    }
    // curl-noise breeze (divergence-free stream function)
    float tb = TIME * 0.07 + audioMidTime * 0.04 * ar;
    // psi = sin(1.7x + tb) sin(1.3y - 1.7tb) + 0.38 sin(2.9y + 1.1x + 1.3tb);  u = (dpsi/dy, -dpsi/dx)
    float w2 = 0.38 * cos(2.9 * c.y + 1.1 * c.x + 1.3 * tb);
    vec2 ub = vec2(1.3 * sin(1.7 * c.x + tb) * cos(1.3 * c.y - 1.7 * tb) + 2.9 * w2,
                  -1.7 * cos(1.7 * c.x + tb) * sin(1.3 * c.y - 1.7 * tb) - 1.1 * w2);
    ub *= 0.025 + 0.03 * gLift + 0.08 * gBuild + 0.04 * gPeak;
    float drag = 1.2 + 2.5 * damping + 1.5 * gRest;
    vec2 acc = (uind + ub - u) * drag;                                // inertia: the body lags the local flow
    acc.y += gravity * (h11(i * 5.17 + 0.3) * 2.0 - 0.6) * 0.18;      // buoyancy: warm cores rise, cold sink
    acc += rep * 0.004;
    acc -= c * 0.35 * gBuild * gBuild;                                // tension: drawn inward on the build
    float rr = length(c);
    acc += c / (rr + 0.05) * blast * 9.0;                             // drop: blasted outward
    u += acc * dt;
    if (kicked) u += normalize(upair + vec2(1e-4, 0.0)) * kickAmt * 0.35;   // the pair lunges along its own jet
    float su = length(u);
    if (su > 1.2) u *= 1.2 / su;
    c += u * dt;
    // bounce off the frame
    vec2 B = vec2(asp * 0.5 - 0.05, 0.45);
    float rest = 0.3 + 0.65 * bounceAmt;
    if (c.x >  B.x) { c.x =  B.x; u.x = -abs(u.x) * rest; }
    if (c.x < -B.x) { c.x = -B.x; u.x =  abs(u.x) * rest; }
    if (c.y >  B.y) { c.y =  B.y; u.y = -abs(u.y) * rest; }
    if (c.y < -B.y) { c.y = -B.y; u.y =  abs(u.y) * rest; }
    return vec4(c, u);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    if (PASSINDEX == 0) {
        gl_FragColor = simStep(fc, R, clamp(TIMEDELTA, 0.0, 1.0 / 30.0), ar);
        return;
    }
    gT = TIME * 0.12 * flowSpeed + (audioMidTime * 0.12 + audioBassTime * 0.06) * ar * flowSpeed + 0.6 * gBuild * gBuild;
    gOct = clamp(floor(octaves + 0.6 * gBuild + 0.8 * gPeak - 1.2 * gRest + 0.5), 1.0, 7.0);

    vec2 sp = (fc - 0.5 * R) / R.y;
    // film-gate weave + build tremor
    sp += vec2(sin(TIME * 7.3), cos(TIME * 5.9)) * 0.0015 * (0.3 + gBuild);
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.006 * smoothstep(0.7, 1.0, gBuild);
    // scale: finer detail as the build climbs, a bass swell, drop shockwave
    float sc = noiseScale * (1.0 + 0.45 * gBuild - 0.15 * gRest) * (1.0 - 0.06 * bassP() * ar);
    float rr = length(sp);
    float shock = exp(-pow((rr - (1.0 - gDrop) * 1.2) * 9.0, 2.0)) * gDrop;
    vec2 p = sp * sc + vec2(TIME * 0.03, -TIME * 0.02) * drift * 4.0;
    float K = clamp(physicsAmt, 0.0, 1.0);
    p += (normalize(sp + 1e-4) * shock * 0.25 + sp * 0.12 * audioBeatPulse * ar * exp(-rr * 2.0)) * (1.0 - K);   // analytic fallback
    float px = sc / R.y;

    // relief / focus: FD step for normals widens toward the frame edge (soft depth-of-field on the lighting)
    gRel = clamp(relief, 0.0, 1.0);
    gSp = sp;
    gFocus = smoothstep(0.35, 1.05, length(sp * vec2(0.75, 1.0))) * gRel;
    gEs = max(1.75 / R.y, 0.0028) * (1.0 + 2.5 * gFocus);
    // ── material displacement from the simulated vortices (5 points: pixel, +x, +y neighbours, 2 riso cells) ──
    vec2 q0 = sp, q1 = sp + vec2(gEs, 0.0), q2 = sp + vec2(0.0, gEs), q3 = sp, q4 = sp;
    {
        float cell = max(dotSize, 2.0);
        for (int i = 0; i < 2; i++) {
            float fi = float(i);
            vec2 off = fi < 0.5 ? vec2(0.0) : vec2(2.5, -1.5) + 1.5 * vec2(sin(gT * 7.0), cos(gT * 5.0));
            float ang = fi < 0.5 ? 0.26 : 1.31;
            vec2 cpix = rot(-ang) * (floor(rot(ang) * (fc + off) / cell) + 0.5) * cell - off;
            vec2 cs = (cpix - 0.5 * R) / R.y;
            if (i == 0) q3 = cs; else q4 = cs;
        }
    }
    vec2 s1 = q1, s2 = q2, s3 = q3, s4 = q4;
    float e = texture2D(simBuf, cellUV(0.0, 6.0)).z;
    if (K > 0.001) {
        // blast: radial expansion around the centre (e > 0 pushes material outward)
        q0 *= 1.0 - e * exp(-length(q0) * 1.3); q1 *= 1.0 - e * exp(-length(q1) * 1.3);
        q2 *= 1.0 - e * exp(-length(q2) * 1.3); q3 *= 1.0 - e * exp(-length(q3) * 1.3);
        q4 *= 1.0 - e * exp(-length(q4) * 1.3);
        for (int j = 0; j < NV; j++) {
            float fj = float(j);
            vec2 c = vPos(fj).xy;
            float th = vAux(fj).y;
            float irc2 = 1.0 / (vCore(fj) * vCore(fj));
            // Lamb–Oseen angular profile: rigid core, 1/r² skirt → spiral arms
            vec4 x = vec4(dot(q0 - c, q0 - c), dot(q1 - c, q1 - c), dot(q2 - c, q2 - c), dot(q3 - c, q3 - c)) * irc2;
            vec4 a = -th * (1.0 - exp(-x)) / max(x, vec4(1e-4));
            vec4 cs = cos(a), sn = sin(a);
            vec2 d;
            d = q0 - c; q0 = c + vec2(cs.x * d.x - sn.x * d.y, sn.x * d.x + cs.x * d.y);
            d = q1 - c; q1 = c + vec2(cs.y * d.x - sn.y * d.y, sn.y * d.x + cs.y * d.y);
            d = q2 - c; q2 = c + vec2(cs.z * d.x - sn.z * d.y, sn.z * d.x + cs.z * d.y);
            d = q3 - c; q3 = c + vec2(cs.w * d.x - sn.w * d.y, sn.w * d.x + cs.w * d.y);
            d = q4 - c;
            float x4 = dot(d, d) * irc2, a4 = -th * (1.0 - exp(-x4)) / max(x4, 1e-4);
            q4 = c + vec2(cos(a4) * d.x - sin(a4) * d.y, sin(a4) * d.x + cos(a4) * d.y);
        }
    }
    gDK = sc * K;
    vec2 dsp = (sp - q0) * gDK;
    gDX = (s1 - q1) * gDK; gDY = (s2 - q2) * gDK;
    gDC0 = (s3 - q3) * gDK; gDC1 = (s4 - q4) * gDK;

    // texture morph
    float ph = TIME / morphPeriod;
    float ts = float(textureSel);
    float ka = ts > 0.5 ? ts - 1.0 : mod(floor(ph), 6.0);
    float kb = ts > 0.5 ? ka : mod(floor(ph) + 1.0, 6.0);
    float mf = ts > 0.5 ? 0.0 : smoothstep(0.75, 1.0, fract(ph));
    // grainy dissolve: the next texture eats through in noise-shaped patches (one textureLook call site)
    float nd = perlin(vec3(sp * 3.0, TIME * 0.3)) * 0.5 + 0.5;
    float m = mf > 0.001 ? smoothstep(nd - 0.12, nd + 0.12, mf * 1.24 - 0.12) : 0.0;
    vec3 col = vec3(0.0);
    for (int i = 0; i < 2; i++) {
        float wgt = i == 0 ? 1.0 - m : m;
        if (wgt < 0.001) continue;
        col += textureLook(i == 0 ? ka : kb, p, fc, px, dsp) * wgt;
    }

    // shape & energy
    col = mix(col, backdrop(uv), bgMix * (1.0 - lum(col)));
    col *= 0.78 + 0.32 * gLift + 0.1 * audioBeatPulse * ar;
    col += inkC.rgb * shock * 0.6;
    col = mix(col, vec3(lum(col)), 0.35 * gRest);
    col *= 1.0 - vignette * smoothstep(0.45, 1.25, length(sp * vec2(0.85, 1.0)));

    // photographic grain (resolution-independent size, crawling per frame)
    float gs = max(grainSize, 1.0);
    float fr = floor(TIME * 24.0 * grainSpeed);
    vec2 gp = fc / gs;
    float g1 = perlin(vec3(gp * 0.9, fr * 1.7)) ;
    float g2 = h12(floor(gp) + fr * 13.0) * 2.0 - 1.0;
    float gm = mix(g2, g1, 0.45);
    vec3 gc = vec3(gm);
    if (grainColour > 0.0) {
        vec3 gcol = vec3(h12(floor(gp) + fr * 7.0 + 1.3), h12(floor(gp) + fr * 7.0 + 5.1), h12(floor(gp) + fr * 7.0 + 9.7)) * 2.0 - 1.0;
        gc = mix(gc, gcol, grainColour);
    }
    float gA = grainAmt * (0.6 + 0.5 * gLift + 0.6 * gBuild + 0.8 * gDrop + 0.4 * highP() * ar) * (0.35 + 0.65 * (1.0 - abs(lum(col) - 0.5) * 1.4));
    col += gc * 0.09 * gA;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(lum(col) * 1.4, 0.0, 1.0) : 1.0);
}
