/*{
  "DESCRIPTION": "Orbs — big glowing circles drifting on slow, curving Lissajous paths, each one sweeping out a slinky tube behind it: hundreds of fine, closely stacked circle outlines that fan open where the orb sweeps fast and bunch into saturated, glowing rims where the tube turns away. Colour flows along every trail — blue into yellow into red into green — while the newest circle at the head stays solid and the tubes weave over and under one another in depth, swelling as they come toward you. Behind them a bold mesh gradient of huge soft colour fields drifts and blends. Four looks morph into each other: Slinky Lines (fine ring hatching on a pale paper backdrop, like a print), Glow Orbs (soft luminous spheres trailing light on a deep gradient), Solid Tubes (smooth shaded, glossy tubes) and Neon Rings (bright line-light on near black). The orbs are real simulated bodies with mass and momentum: each is tethered by a spring to a drifting anchor under adjustable gravity, they collide elastically with each other (passing over and under by depth) and bounce off the frame, and the slinky trails are the recorded history of that motion, so the rings fan open where an orb was flung fast and bunch where it hung slowly. Everything is lit as glossy 3D: soft key and rim light, fresnel, two-lobe specular, reflections of the backdrop, contact shadows and occlusion where tubes cross, and depth haze that softens far orbs. Bass swells the orbs and every beat lands as an impulse kick, the mids stir them and the highs make the hatching shimmer. At rest a few orbs float slowly with short trails; during a build the springs stiffen and pull the orbs into a tight whirling orbit that starts to tremble; the drop is one explosive outward blast with a shockwave and a flash of colour that the orbs visibly recover from, and the peak fills the screen with every orb, long trails and full colour.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Geometric", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "styleSel",    "LABEL": "Style", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Slinky Lines", "Glow Orbs", "Solid Tubes", "Neon Rings"], "GROUP": "Style" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 16.0, "GROUP": "Style" },
    { "NAME": "orbCount",    "LABEL": "Orb Count",       "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 6.0, "DEFAULT": 5.0, "GROUP": "Orbs" },
    { "NAME": "orbSize",     "LABEL": "Size",            "TYPE": "float", "MIN": 0.4, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Orbs" },
    { "NAME": "trailLen",    "LABEL": "Trail Length",    "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Orbs" },
    { "NAME": "ringDensity", "LABEL": "Ring Density",    "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Orbs" },
    { "NAME": "lineThick",   "LABEL": "Line Thickness",  "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Orbs" },
    { "NAME": "speed",       "LABEL": "Speed",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "colourSpeed", "LABEL": "Colour Speed",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "glow",        "LABEL": "Glow",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "sphereShade", "LABEL": "Sphere Shading",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Look" },
    { "NAME": "depthHaze",   "LABEL": "Depth Haze / DOF", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Look" },
    { "NAME": "lightSmear",  "LABEL": "Light Smear (feedback)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Mesh Gradient (animated)", "Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.87, 0.85, 0.83, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.62, 0.6, 0.64, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" },
    { "NAME": "physicsAmt",  "LABEL": "Physics Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "tether",      "LABEL": "Tether Spring",    "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "gravity",     "LABEL": "Gravity",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.35, "GROUP": "Physics" },
    { "NAME": "bounce",      "LABEL": "Bounce",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Physics" },
    { "NAME": "damping",     "LABEL": "Damping",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.8, "GROUP": "Physics" },
    { "NAME": "kickForce",   "LABEL": "Kick Force",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "resetPhysics","LABEL": "Reset Physics",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Physics" }
  ],
  "PASSES": [
    { "TARGET": "physBuf", "PERSISTENT": true, "FLOAT": true },
    { "TARGET": "smearBuf", "PERSISTENT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718
#define NS  48

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

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

// ── colour: blue → yellow → red → green, looping ───────────────────────
vec3 ramp(float u) {
    u = fract(u) * 6.0;
    vec3 c0 = vec3(0.04, 0.22, 0.95);   // ultramarine
    vec3 c1 = vec3(0.0, 0.62, 0.42);    // green
    vec3 c2 = vec3(1.0, 0.82, 0.05);    // yellow
    vec3 c3 = vec3(1.0, 0.38, 0.04);    // orange
    vec3 c4 = vec3(0.93, 0.03, 0.15);   // red
    vec3 c5 = vec3(0.55, 0.08, 0.75);   // violet
    float f = smoothstep(0.0, 1.0, fract(u));
    vec3 c;
    if (u < 1.0) c = mix(c0, c1, f);
    else if (u < 2.0) c = mix(c1, c2, f);
    else if (u < 3.0) c = mix(c2, c3, f);
    else if (u < 4.0) c = mix(c3, c4, f);
    else if (u < 5.0) c = mix(c4, c5, f);
    else c = mix(c5, c0, f);
    // keep the in-between colours vivid instead of mixing to grey
    float l = dot(c, vec3(0.2126, 0.7152, 0.0722));
    return clamp(vec3(l) + (c - vec3(l)) * (1.0 + 2.2 * f * (1.0 - f)), 0.0, 1.0);
}

// style parameters: A = (hatch, tubeShade, fillAlpha, glow)  B = (trailAlpha, paleBg, neon, -)
vec4 styleA(float s) {
    if (s < 0.5) return vec4(1.0, 0.6, 1.0, 0.25);     // Slinky Lines
    if (s < 1.5) return vec4(0.5, 0.7, 0.55, 1.0);     // Glow Orbs
    if (s < 2.5) return vec4(0.0, 1.0, 1.0, 0.4);      // Solid Tubes
    return vec4(1.0, 0.0, 0.0, 1.0);                   // Neon Rings
}
vec4 styleB(float s) {
    if (s < 0.5) return vec4(1.0, 1.0, 0.0, 0.0);
    if (s < 1.5) return vec4(0.6, 0.0, 0.25, 0.0);
    if (s < 2.5) return vec4(1.0, 0.0, 0.0, 0.0);
    return vec4(1.0, 0.0, 1.0, 0.0);
}
float sHatch, sShade, sFill, sGlow, sTrailA, sPale, sNeon;

// ── backdrop: animated mesh gradient + the standard modes ─────────────
vec3 meshGradient(vec2 sp) {
    float t = TIME * 0.05 + audioBassTime * 0.02 * audioReact;
    vec3 acc = vec3(0.0);
    float wsum = 0.0;
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        vec2 c = vec2(0.75 * sin(t * (0.7 + 0.13 * fi) + fi * 2.1), 0.45 * cos(t * (0.55 + 0.11 * fi) + fi * 1.7));
        float w = exp(-dot(sp - c, sp - c) * 2.6);
        w = w * w + 0.0005;
        vec3 k = 0.5 + 0.5 * cos(TAU * (fi * 0.21 + t * 0.12 + vec3(0.0, 0.33, 0.67)));
        k = mix(vec3(lum(k)), k, 1.5);
        acc += k * w; wsum += w;
    }
    vec3 m = acc / wsum;
    m = clamp(mix(vec3(lum(m)), m, 1.35), 0.0, 1.0);
    // luminance swell so the red palette still reads light / dark regions
    float sh = 0.75 + 0.35 * smoothstep(-0.6, 0.6, sp.y + 0.25 * sin(sp.x * 2.0 + t * 3.0));
    // big slow light / shadow fields so the red palette reads dark → red → light
    float lf = 0.5 + 0.5 * sin(sp.x * 1.7 - sp.y * 1.3 + t * 2.3) * cos(sp.y * 1.9 + t * 1.7);
    vec3 deep = m * sh * (0.22 + 0.8 * lf) * (0.8 + 0.25 * gLift);
    vec3 pale = mix(vec3(0.89, 0.87, 0.85), m, 0.16) * (0.96 + 0.05 * sh);
    vec3 dark = m * 0.16 * sh + vec3(0.01, 0.0, 0.03);
    deep *= 1.0 - 0.9 * (1.0 - sTrailA);          // glow look: a darker field so the light reads
    vec3 o = mix(deep, pale, sPale);
    return mix(o, dark, sNeon);
}
vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    vec2 sp = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    if (m < 0.5) return meshGradient(sp);
    if (m < 1.5) return bgColor.rgb;
    if (m < 2.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(sp)));
}

// ── orbs: analytic "anchor" paths that the physical bodies are tethered to ──
float gAX, gAY, gTight, gSize, gCnt, gT, gDelta, gL, gPhi, gRing;
vec4 orbFr(float fk) {
    return vec4(0.33 + 0.2 * h11(fk * 7.13 + 1.1), 0.41 + 0.22 * h11(fk * 3.71 + 2.3),
                0.3 + 0.2 * h11(fk * 5.3 + 3.7), 0.35 + 0.2 * h11(fk * 9.1 + 4.9));
}
vec4 orbPh(float fk) {
    return vec4(h11(fk * 1.7 + 0.3), h11(fk * 2.9 + 0.7), h11(fk * 4.1 + 0.9), h11(fk * 6.7 + 1.3)) * TAU;
}
float orbRad(float fk) { return gSize * (0.8 + 0.4 * h11(fk * 11.3 + 5.0)); }
vec3 orbPath(vec4 fr, vec4 ph, float k, float t) {
    vec2 p = vec2(0.72 * sin(t * fr.x + ph.x) + 0.28 * sin(t * fr.z * 1.9 + ph.z),
                  0.70 * sin(t * fr.y + ph.y) + 0.30 * sin(t * fr.w * 1.6 + ph.w)) * vec2(gAX, gAY);
    float z = sin(t * (fr.x + fr.y) * 0.6 + ph.z + ph.w);
    // build: the anchors tighten into a whirling orbit around the centre
    float a = t * 1.1 + k * TAU / max(gCnt, 1.0);
    vec2 o = vec2(cos(a), sin(a)) * (0.27 + 0.05 * sin(t * 0.7 + k)) * vec2(1.25, 1.0);
    p = mix(p, o, gTight);
    z = mix(z, sin(a), gTight);
    return vec3(p, z);
}

void setupGlobals() {
    vec2 R = RENDERSIZE.xy;
    float ar = audioReact;
    gT = TIME * 0.42 * speed + (audioMidTime * 0.35 + audioBassTime * 0.1) * ar + 1.4 * gBuild * gBuild;
    gAX = R.x / R.y * 0.5 - 0.12;
    gAY = 0.37;
    gTight = 0.75 * smoothstep(0.0, 0.9, gBuild);
    gCnt = clamp(mix(orbCount, min(orbCount, 3.0), smoothstep(0.3, 0.75, gRest)), 1.0, 6.0);
    gCnt = mix(gCnt, 6.0, gPeak);
    gSize = 0.12 * orbSize * (1.0 + 0.16 * bassP() * ar + 0.12 * gDrop - 0.1 * gTight) * (1.0 + 0.1 * gRest);
    // trail = ring buffer of NS knots; one knot is laid every gDelta seconds
    float Ts = 3.6 * trailLen * (1.0 - 0.55 * gRest + 0.8 * gBuild + 0.35 * gPeak);
    gDelta = max(0.085, Ts / 44.0);
    gL = clamp(Ts / gDelta, 3.0, 44.0);
    gRing = max(floor(3.6 * ringDensity + 0.5), 1.0);   // rings laid per knot
}

// ── state buffer: NCOL × NROW cells, resolution independent ────────────
// row k (0..5): col 0 = pos.xyz + colour phase, col 1 = vel.xyz + marker,
//               col 2.. = trail knots 1..NS-1 (shift register, newest first)
// row 6, col 0 = (knot timer phi, -, -, marker)
#define NCOL 50.0
#define NROW 8.0
#define ZS 0.3
vec2 cellUV(float cx, float cy) {
    vec2 w = RENDERSIZE.xy / vec2(NCOL, NROW);
    return (floor((vec2(cx, cy) + 0.5) * w) + 0.5) / RENDERSIZE.xy;
}
// knot i of orb k: 0 = the live body, i ≥ 1 = history
vec4 knot(float k, float i) { return texture2D(physBuf, cellUV(i < 0.5 ? 0.0 : i + 1.0, k)); }
float knotA(float i) { return i < 0.5 ? 0.0 : gPhi + i - 1.0; }   // trail parameter of a knot

// smooth (Catmull-Rom) position along the simulated trail at parameter a
vec4 trailAt(float k, float a, out vec2 tang) {
    float j = a < gPhi ? 0.0 : min(floor(a - gPhi) + 1.0, float(NS) - 2.0);
    float seg = j < 0.5 ? max(gPhi, 1e-3) : 1.0;
    float f = clamp((a - knotA(j)) / seg, 0.0, 1.0);
    vec4 p1 = knot(k, j);
    vec4 p2 = knot(k, j + 1.0);
    vec4 p0 = knot(k, max(j - 1.0, 0.0));
    vec4 p3 = knot(k, min(j + 2.0, float(NS) - 1.0));
    if (j < 0.5) p0 = 2.0 * p1 - p2;
    vec3 m1 = 0.5 * (p2.xyz - p0.xyz), m2 = 0.5 * (p3.xyz - p1.xyz);
    float f2 = f * f, f3 = f2 * f;
    vec3 c = (2.0 * f3 - 3.0 * f2 + 1.0) * p1.xyz + (f3 - 2.0 * f2 + f) * m1 + (-2.0 * f3 + 3.0 * f2) * p2.xyz + (f3 - f2) * m2;
    vec3 dc = (6.0 * f2 - 6.0 * f) * p1.xyz + (3.0 * f2 - 4.0 * f + 1.0) * m1 + (6.0 * f - 6.0 * f2) * p2.xyz + (3.0 * f2 - 2.0 * f) * m2;
    tang = dc.xy / seg;
    float w = p1.w + (fract(p2.w - p1.w + 0.5) - 0.5) * f;
    return vec4(c, w);
}

vec4 physics(vec2 fc) {
    vec2 R = RENDERSIZE.xy;
    vec2 cell = floor(fc / (R / vec2(NCOL, NROW)));
    float dt = clamp(TIMEDELTA, 0.0, 1.0 / 30.0);
    vec4 G = texture2D(physBuf, cellUV(0.0, 6.0));
    bool ok = FRAMEINDEX > 1 && !resetPhysics && G.w > 0.5 && G.w < 1.5 && G.x >= 0.0 && G.x < 1.01;
    float phi = G.x + dt / gDelta;
    bool push = phi >= 1.0;
    if (push) phi = min(phi - 1.0, 0.98);
    if (cell.y > 6.5 || cell.x > float(NS) + 0.5) return vec4(0.0);
    if (cell.y > 5.5) return cell.x < 0.5 ? vec4(ok ? phi : 0.0, 0.0, 0.0, 1.0) : vec4(0.0);

    float fk = cell.y;
    vec4 fr = orbFr(fk), ph = orbPh(fk);
    vec3 tg = orbPath(fr, ph, fk, gT);
    float colPh = fract(gT * 0.09);
    if (cell.x > 1.5) {
        // trail knots: shift one slot back each time a knot is laid
        if (!ok) return vec4(tg, colPh);
        return push ? knot(fk, cell.x - 2.0) : texture2D(physBuf, cellUV(cell.x, fk));
    }

    vec4 P = texture2D(physBuf, cellUV(0.0, fk));
    vec4 V = texture2D(physBuf, cellUV(1.0, fk));
    vec3 p = P.xyz, v = V.xyz;
    bool bad = !ok || V.w < 0.5 || !(abs(p.x) < 4.0 && abs(p.y) < 4.0 && abs(p.z) < 4.0 && abs(v.x) < 40.0 && abs(v.y) < 40.0 && abs(v.z) < 40.0);
    if (bad) { p = tg; v = vec3(0.0); }
    else {
        float ar = audioReact;
        float asp = R.x / R.y;
        // tether spring to the moving anchor (stiffer during the build → tight orbit)
        float kS = mix(2.4, 10.0, gTight) * (1.0 + 0.5 * gRest) * tether;
        vec3 acc = (tg - p) * vec3(kS, kS, 0.6 * kS);
        vec2 rad = p.xy;
        float rl = length(rad) + 1e-4;
        acc.xy += vec2(-rad.y, rad.x) / rl * 1.2 * gTight;          // build swirl
        acc.y -= gravity * (1.0 - 0.6 * gTight);
        // beat / bass kicks = impulses (the pulse envelope is short, so F·dt ≈ an impulse)
        float beat = audioBeatPulse * ar;
        float seed = floor(audioBassTime * 2.0 + TIME * 0.25);
        vec2 jit = vec2(h11(fk * 3.1 + seed), h11(fk * 5.9 + seed + 7.0)) - 0.5;
        vec2 kd = normalize(normalize(v.xy + 1e-4) + 0.5 * rad / rl + 1.2 * jit + 1e-4);
        float kick = kickForce * 4.5 * beat * beat * (1.0 + 0.5 * bassP() * ar) * (1.0 - 0.6 * gRest);
        acc.xy += kd * kick;
        acc.z += kickForce * 5.0 * beat * beat * (0.8 - 0.6 * p.z) * (1.0 - 0.6 * gRest);
        // drop: explosive outward blast
        float bl = 30.0 * kickForce * gDrop * gDrop;
        acc.xy += normalize(rad + 0.05 * vec2(cos(fk * 2.4), sin(fk * 2.4))) * bl;
        acc.z += bl * (h11(fk * 5.7 + 2.0) - 0.4) * 0.6;
        // peak: restless turbulence keeps them colliding
        float tu = 0.05 + 0.55 * gPeak + 0.2 * midP() * ar;
        acc.xy += tu * vec2(sin(TIME * 1.31 + fk * 2.1 + audioMidTime * 0.7), cos(TIME * 1.07 + fk * 3.7 + audioMidTime * 0.6)) * (1.0 - 0.7 * gRest);
        v += acc * dt;
        v *= exp(-damping * dt * (1.0 + 1.2 * gRest - 0.45 * gPeak));

        // elastic collisions between the visible orbs (depth-scaled spheres, so they can also pass over / under)
        float rk0 = orbRad(fk);
        float m = rk0 * rk0;
        float rk = rk0 * (1.0 + 0.3 * p.z);
        if (fk < gCnt - 0.02) {
            for (int j = 0; j < 6; j++) {
                float fj = float(j);
                if (fj > gCnt - 0.02) break;
                if (abs(fj - fk) < 0.5) continue;
                vec4 Pj = texture2D(physBuf, cellUV(0.0, fj));
                vec4 Vj = texture2D(physBuf, cellUV(1.0, fj));
                float rj0 = orbRad(fj);
                float rs = 0.92 * (rk + rj0 * (1.0 + 0.3 * Pj.z));
                vec3 d = vec3(p.xy - Pj.xy, (p.z - Pj.z) * ZS);
                float dl = length(d);
                if (dl < rs && dl > 1e-5) {
                    vec3 n = d / dl;
                    float mj = rj0 * rj0;
                    float w = mj / (m + mj);
                    vec3 vv = vec3(v.xy, v.z * ZS);
                    float rel = dot(vv - vec3(Vj.xy, Vj.z * ZS), n);
                    if (rel < 0.0) vv -= (1.0 + bounce) * w * rel * n;
                    v = vec3(vv.xy, vv.z / ZS);
                    p += vec3(n.xy, n.z / ZS) * (rs - dl) * w * 0.8;
                }
            }
        }
        float vl = length(v.xy);
        if (vl > 2.5) v.xy *= 2.5 / vl;
        v.z = clamp(v.z, -5.0, 5.0);
        p += v * dt;
        // frame walls + depth limits
        float rw = rk0 * (1.0 + 0.3 * clamp(p.z, -1.0, 1.0));
        vec3 lim = vec3(asp * 0.5 - rw, 0.5 - rw, 1.0);
        if (p.x > lim.x)  { p.x = lim.x;  v.x = -abs(v.x) * bounce; }
        if (p.x < -lim.x) { p.x = -lim.x; v.x =  abs(v.x) * bounce; }
        if (p.y > lim.y)  { p.y = lim.y;  v.y = -abs(v.y) * bounce; }
        if (p.y < -lim.y) { p.y = -lim.y; v.y =  abs(v.y) * bounce; }
        if (p.z > lim.z)  { p.z = lim.z;  v.z = -abs(v.z) * bounce; }
        if (p.z < -lim.z) { p.z = -lim.z; v.z =  abs(v.z) * bounce; }
        // Physics Amount 0 → locked to the analytic path (the original motion)
        float lock = (1.0 - physicsAmt) * (1.0 - physicsAmt);
        p = mix(p, tg, lock);
        v *= 1.0 - lock;
    }
    return cell.x < 0.5 ? vec4(p, colPh) : vec4(v, 1.0);
}

// ── physically-flavoured surface shading: soft key + rim light, glossy
//    two-lobe specular, fresnel and an environment reflection of the backdrop ──
vec3 shadeSurf(vec3 N, vec3 base, vec3 rc, vec3 shadowC, vec3 bg, float shK, out float specO) {
    vec3 Lk = normalize(vec3(-0.45, 0.6, 0.65));
    float wrap = clamp(dot(N, Lk) * 0.55 + 0.45, 0.0, 1.0);
    float dif = wrap * wrap;
    vec3 H = normalize(Lk + vec3(0.0, 0.0, 1.0));
    float nh = max(dot(N, H), 0.0);
    specO = pow(nh, 110.0) * 1.2 + pow(nh, 16.0) * 0.16;
    float fres = pow(1.0 - clamp(N.z, 0.0, 1.0), 3.0);
    float ry = 2.0 * N.z * N.y;                              // reflected view ray, y
    vec3 env = mix(bg * 0.45, bg * 1.15 + 0.14, smoothstep(-0.7, 0.8, ry));
    float rim = fres * smoothstep(-0.2, 0.7, dot(normalize(N.xy + 1e-5), vec2(0.82, -0.57)));
    vec3 c = mix(base, mix(shadowC, base * 1.12, dif), shK);
    c = mix(c, env, (0.08 + 0.5 * fres) * shK);
    c += rc * rim * 0.6 * shK + vec3(specO) * shK * 0.85;
    return c;
}

// shrinking-step search for the true swept-disc distance between knots
void refineD(float fk, float rk, float L, vec2 sp, inout float dm, inout float sm, inout float zm) {
    float sc = sm;
    vec2 tg;
    for (int j = 0; j < 6; j++) {
        float st = 0.5 * exp2(-floor(float(j) * 0.5));
        float s = clamp(sc + (mod(float(j), 2.0) < 0.5 ? -st : st), 0.0, L);
        vec4 c = trailAt(fk, s, tg);
        float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, s));
        float d = length(sp - c.xy) - r;
        if (d < dm) { dm = d; zm = c.z; sm = s; }
        if (mod(float(j), 2.0) > 0.5) sc = sm;
    }
}

vec4 scene(vec2 fc) {
    vec2 R = RENDERSIZE.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float px = 1.0 / R.y;
    float ar = audioReact;

    // style morph (parameters only: one render)
    float st = float(styleSel);
    float sa = st - 1.0, sb = st - 1.0, sm = 0.0;
    if (st < 0.5) {
        float ph = TIME / morphPeriod;
        sa = mod(floor(ph), 4.0);
        sb = mod(sa + 1.0, 4.0);
        sm = smoothstep(0.78, 1.0, fract(ph));
    }
    vec4 A = mix(styleA(sa), styleA(sb), sm);
    vec4 B = mix(styleB(sa), styleB(sb), sm);
    sHatch = A.x; sShade = A.y; sFill = A.z; sGlow = A.w;
    sTrailA = B.x; sPale = B.y; sNeon = B.z;

    float cT = TIME * 0.06 * colourSpeed + audioHighTime * 0.02 * ar;

    // build tremor near the top
    float trem = smoothstep(0.65, 1.0, gBuild);
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.008 * trem;

    gPhi = clamp(texture2D(physBuf, cellUV(0.0, 6.0)).x, 0.0, 0.999);
    float L = gL;
    float wpx = lineThick * max(R.y / 600.0, 0.8);   // line width in pixels
    float beat = audioBeatPulse * ar;
    float satK = 0.85 + 0.15 * gLift + 0.1 * gPeak - 0.1 * gRest;

    vec3 bg = backdrop(uv);
    vec3 colF = vec3(0.0); float aF = 0.0; float zF = -1e3;   // front layer
    vec3 colB = vec3(0.0); float aB = 0.0; float zB = -1e3;   // layer behind
    vec3 glowAdd = vec3(0.0);
    vec3 occ[6];

    for (int k = 0; k < 6; k++) {
        occ[k] = vec3(1e3, -2.0, 1.0);
        float fk = float(k);
        if (fk > gCnt - 0.02) break;
        float vis = clamp(gCnt - fk, 0.0, 1.0);
        float rk = orbRad(fk) * mix(0.6, 1.0, vis);
        vis = smoothstep(0.0, 1.0, vis);
        float uk = fk * 0.29 + 0.13;

        // ── coarse search over the simulated trail knots, newest first ──
        float sHit = -1.0;
        float dPrev = 1.0, aPrev = 0.0;
        float dmin = 1e3, sMin = 0.0, zMin = 0.0, dHead = 1e3;
        float gwk = rk * (0.25 + 0.35 * sGlow);
        float gW = 0.0, gS = 0.0, gU = 0.0;
        vec4 cHead = vec4(0.0);
        float iHit = 0.0, rHit = 1.0, dSelf = 1e3, dOld = 1e3, dNew = 1e3, sNew = 0.0;
        float arc = 0.0, arcHit = 0.0;
        vec2 pLast = vec2(0.0);
        for (int i = 0; i < NS; i++) {
            float fi = float(i);
            float s = knotA(fi);
            if (s > L + 1.0) break;
            vec4 c = knot(fk, fi);
            float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, s));
            float d = length(sp - c.xy) - r;
            if (i == 0) { dHead = d; cHead = c; pLast = c.xy; }
            arc += length(c.xy - pLast); pLast = c.xy;              // arc length along the trail
            if (d < dmin) { dmin = d; sMin = s; zMin = c.z; }
            // after the hit keep scanning: true silhouette distance for AA, and
            // whether an older, separate part of the tube lies underneath
            if (sHit >= 0.0) {
                if (arc - arcHit > 2.2 * rHit) dOld = min(dOld, d); else dSelf = min(dSelf, d);
                if (dSelf < -4.0 * px) break;
                continue;
            }
            if (d < 0.0) { sHit = s; iHit = fi; arcHit = arc; rHit = r; dSelf = d; if (d < -4.0 * px) break; continue; }
            float gwt = exp(-d / gwk) * (1.0 - smoothstep(0.6 * L, L, s));
            gW += gwt; gS += gwt * s; gU += gwt * (fract(c.w - cHead.w + 0.5) - 0.5);
            dPrev = d; aPrev = s;
        }
        // newer, separate parts of the tube passing over this one (self-overlap)
        if (sHit >= 0.0) {
            arc = 0.0;
            for (int i = 0; i < NS; i++) {
                float fi = float(i);
                if (fi > iHit - 0.5) break;
                vec4 c = knot(fk, fi);
                float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, knotA(fi)));
                float d = length(sp - c.xy) - r;
                if (i == 0) pLast = c.xy;
                arc += length(c.xy - pLast); pLast = c.xy;
                if (arcHit - arc > 2.2 * rHit && d < dNew) { dNew = d; sNew = knotA(fi); }
            }
        }
        // a newer part may really cover this pixel between its knots: switch to it
        bool switched = false;
        if (dNew < rk * 0.35) {
            float zN = 0.0;
            refineD(fk, rk, L, sp, dNew, sNew, zN);
            if (dNew < 0.0) { switched = true; sHit = sNew; dSelf = dNew; dOld = -1.0; zMin = zN; }
        }
        vec2 tg;
        // close the scallops between knots at the tube silhouette
        if (dmin > -2.0 * px && dmin < rk * 0.35) {
            refineD(fk, rk, L, sp, dmin, sMin, zMin);
        }
        occ[k] = vec3(dmin, zMin, rk);
        float u, z, alpha;
        vec3 col;
        // depth of field: far orbs (z < 0) get softer edges, then haze below
        float far = smoothstep(0.1, -1.0, zMin) * depthHaze;
        float aaw = px * (0.85 + 3.5 * far);
        float cover = smoothstep(aaw, -aaw, dmin);
        if (sHit < -0.5 && cover < 0.001) {
            // only glow out here (smooth sum over the whole trail)
            vec3 gc = ramp(uk + cHead.w + gU / max(gW, 1e-5) + cT + 0.33);
            glowAdd += gc * (1.0 - exp(-gW * 0.22)) * (0.12 + 0.5 * sGlow) * (glow * (0.6 + 0.4 * gLift) + 0.8 * beat);
            continue;
        }
        bool exact = sHit >= 0.0 && !switched;
        if (sHit < 0.0) sHit = sMin;

        if (sHit < 0.02) {
            // ── head: the live body, solid with soft sphere shading ──
            vec4 c = cHead;
            float r = rk * (1.0 + 0.3 * c.z);
            vec2 q = (sp - c.xy) / r;
            float rho = clamp(length(q), 0.0, 1.0);
            vec3 N = vec3(q, sqrt(max(1.0 - rho * rho, 0.0)));
            u = uk + c.w + cT;
            z = c.z;
            vec3 base = ramp(u) * satK;
            float shK = clamp(0.35 + sphereShade * (0.3 + sShade), 0.0, 1.0);
            vec3 shadowC = ramp(u - 0.12) * satK * 0.5;
            vec3 rc = ramp(u + 0.33) * satK;
            float spec;
            col = shadeSurf(N, base, rc, shadowC, bg, shK, spec);
            // glowing sphere core for the light looks
            col = mix(col, base * (0.5 + 1.1 * N.z * N.z) + vec3(1.0) * pow(N.z, 6.0) * 0.45, sGlow * (1.0 - sPale) * (1.0 - sNeon) * 0.8);
            // neon: dark body, bright outline
            col = mix(col, base * (0.12 + 0.9 * pow(rho, 5.0)) + bg * 0.55 + spec * 0.6, sNeon);
            // outline + saturated rim
            float edgePx = (1.0 - rho) * r / px;
            float rimC = smoothstep(wpx * 1.5 + 1.0 + 2.0 * far, wpx * 0.5, edgePx);
            col = mix(col, mix(rc * 0.8, rc * 1.6 + 0.15, sNeon), rimC * (0.6 + 0.4 * sNeon));
            col = mix(col, rc * 1.1, pow(rho, 8.0) * 0.35 * (1.0 - sNeon));
            alpha = max(cover, smoothstep(aaw, -aaw, dHead)) * mix(1.0, 0.8, sNeon);
            // halo around the head
            float gw = r * (0.35 + 0.4 * sGlow);
            glowAdd += base * exp(-max(dHead, 0.0) / gw) * 0.45 * sGlow * (glow + beat);
        } else {
            // ── body: refine the newest covering parameter (bisection + secant) ──
            float lo = aPrev, hi = sHit;
            float flo = dPrev, fhi = -1.0;
            bool moved = false;
            if (!exact) { lo = hi; flo = 1.0; moved = true; fhi = -1.0; }
            else for (int b = 0; b < 5; b++) {
                float sm2 = 0.5 * (lo + hi);
                vec4 c = trailAt(fk, sm2, tg);
                float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, sm2));
                float d = length(sp - c.xy) - r;
                if (d < 0.0) { hi = sm2; fhi = d; moved = true; } else { lo = sm2; flo = d; }
            }
            if (!moved) { // hi never moved: evaluate it
                vec4 c = trailAt(fk, hi, tg);
                float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, hi));
                fhi = length(sp - c.xy) - r;
            }
            float sS = mix(lo, hi, clamp(flo / max(flo - fhi, 1e-6), 0.0, 1.0));
            float dfds = abs(flo - fhi) / max(hi - lo, 1e-6);
            if (!exact) sS = hi;
            vec4 c = trailAt(fk, sS, tg);
            if (!exact) dfds = 0.45 * length(tg);
            float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, sS));
            z = c.z;
            vec2 n = normalize(sp - c.xy + 1e-6);
            vec2 v = normalize(-tg + 1e-6);                  // toward the head
            float ndv = dot(n, v);
            float perp = sqrt(max(1.0 - ndv * ndv, 0.0));   // 1 at the tube's silhouette
            u = uk + c.w + cT;
            vec3 base = ramp(u) * satK;
            vec3 rc = ramp(u + 0.33) * satK;
            // smooth tube shading (sphere swept along the path)
            vec2 n2 = n - ndv * v;
            vec3 N = normalize(vec3(n2, sqrt(max(1.0 - perp * perp, 0.0)) + 0.15));
            vec3 shadowC = ramp(u - 0.12) * satK * 0.5;
            float spec;
            vec3 fillC = shadeSurf(N, base, rc, shadowC, bg, sShade, spec);
            // saturated rims where the tube turns away
            fillC = mix(fillC, rc * (1.0 + 0.3 * beat), pow(perp, 5.0) * (0.55 + 0.25 * beat));
            // glow look: luminous body
            fillC = mix(fillC, base * 1.25 + 0.12, (1.0 - sTrailA) * 2.0);
            // neon: dark body
            fillC = mix(fillC, base * 0.08 + bg * 0.35 + spec * 0.5, sNeon);
            // ── hatching: gRing rings per knot, fixed to the knots, so rings stay where laid
            //    and spread where the body moved fast, bunch where it moved slowly ──
            float q = (gPhi - sS) * gRing;
            float pxPerQ = dfds / gRing / px;              // pixels between neighbouring rings
            float fq = fract(q);
            float dq = min(fq, 1.0 - fq) * pxPerQ;
            float line = 1.0 - smoothstep(wpx * 0.5 - 0.5 - far, wpx * 0.5 + 0.6 + 2.0 * far, dq);
            line = mix(line, clamp(wpx / max(pxPerQ, 1e-3), 0.0, 1.0) * 0.85, smoothstep(3.5, 1.5, pxPerQ));
            // highs shimmer along the rings
            float ang = atan(n.y, n.x);
            line *= 1.0 + highP() * ar * 0.9 * sin(ang * 9.0 + q * 0.7 + TIME * 14.0);
            vec3 lineC = mix(rc * 0.85, rc * 1.7 + 0.12, sNeon);
            lineC = mix(lineC, rc * 1.4 + 0.1, sGlow * (1.0 - sNeon) * (1.0 - sPale) * 0.7);
            col = mix(fillC, lineC, clamp(line, 0.0, 1.0) * sHatch);
            // trail fade
            float tf = 1.0 - smoothstep(0.75 * L, L, sS);
            float trailA = mix(sTrailA, 1.0, smoothstep(0.25 * L, 0.0, sS));
            alpha = cover * tf * trailA * mix(1.0, sFill, 1.0 - line * sHatch);
            alpha = max(alpha, cover * tf * line * sHatch * sNeon);
            // light-trail look: add the rings as light
            glowAdd += lineC * line * sHatch * sGlow * (1.0 - sPale) * 0.25 * tf * (glow + beat);
        }
        // self-overlap: soft contact shadow under the newer part of the tube and an
        // anti-aliased crease line along the overlap edge (no stair-stepping)
        float crease = max(smoothstep(1.7 * px, 0.15 * px, dNew), smoothstep(-1.9 * px, -0.15 * px, dSelf) * step(dOld, -1.5 * px));
        col *= 1.0 - (0.4 - 0.25 * sNeon) * exp(-max(dNew, 0.0) / (0.3 * rk));
        col = mix(col, col * 0.3, crease * 0.75);
        // atmospheric depth: far geometry sinks into the backdrop haze
        col = mix(col, bg * 0.9 + lum(col) * 0.08, 0.3 * far);
        alpha *= vis;
        // composite: keep the two nearest layers by depth (z toward the viewer)
        if (z > zF) { colB = colF; aB = aF; zB = zF; colF = col; aF = alpha; zF = z; }
        else if (z > zB) { colB = col; aB = alpha; zB = z; }
    }

    // contact shadows / ambient occlusion: every orb nearer the viewer darkens
    // the layers behind it around its silhouette (and the backdrop beneath all)
    float oBg = 0.0, oB = 0.0, oF = 0.0;
    for (int j = 0; j < 6; j++) {
        vec3 o = occ[j];
        float sh = exp(-max(o.x, 0.0) / (0.5 * o.z));
        oBg += sh * 0.55;
        oB += sh * smoothstep(0.02, 0.35, o.y - zB);
        oF += sh * smoothstep(0.02, 0.35, o.y - zF);
    }
    float shStr = 0.55 - 0.35 * sNeon;
    vec3 col = bg * (1.0 - shStr * 0.6 * min(oBg, 1.0));
    vec3 cB = colB * (1.0 - shStr * min(oB, 0.9)), cF = colF * (1.0 - shStr * min(oF, 0.9));
    // soft depth order where two tubes pass through each other (no jagged intersection lines)
    float dz = smoothstep(0.0, 0.08, zF - zB);
    col = mix(mix(mix(col, cF, aF), cB, aB), mix(mix(col, cB, aB), cF, aF), 0.5 + 0.5 * dz);
    float alpha = clamp(max(aF, aB), 0.0, 1.0);
    col += glowAdd * mix(0.7, 0.3, sPale) * (1.0 - 0.85 * aF);
    // drop: shockwave ring and colour flash
    float sr = (1.0 - gDrop) * 1.2;
    float shock = exp(-pow((length(sp) - sr) / 0.03, 2.0)) * gDrop;
    vec3 flashC = ramp(cT * 3.0 + 0.6);
    col += flashC * shock * 1.2;
    col = mix(col, flashC * 1.2 + 0.2, gDrop * gDrop * 0.3);
    alpha = clamp(alpha + lum(glowAdd) + shock, 0.0, 1.0);
    return vec4(col, alpha);
}

void main() {
    showDynamics();
    setupGlobals();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    if (PASSINDEX == 0) {
        gl_FragColor = physics(fc);
        return;
    }
    if (PASSINDEX == 1) {
        vec4 cur = scene(fc);
        vec4 prev = texture2D(smearBuf, uv);
        float sm = lightSmear * (FRAMEINDEX < 2 ? 0.0 : 1.0);
        vec4 o = mix(cur, max(cur, prev * 0.975), sm);
        gl_FragColor = o;
        return;
    }
    vec4 s = texture2D(smearBuf, uv);
    vec3 col = s.rgb;
    float alpha = s.a;
    col *= 1.0 + 0.08 * audioBeatPulse * audioReact;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
