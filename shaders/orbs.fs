/*{
  "DESCRIPTION": "Orbs — big glowing circles drifting on slow, curving Lissajous paths, each one sweeping out a slinky tube behind it: hundreds of fine, closely stacked circle outlines that fan open where the orb sweeps fast and bunch into saturated, glowing rims where the tube turns away. Colour flows along every trail — blue into yellow into red into green — while the newest circle at the head stays solid and the tubes weave over and under one another in depth, swelling as they come toward you. Behind them a bold mesh gradient of huge soft colour fields drifts and blends. Four looks morph into each other: Slinky Lines (fine ring hatching on a pale paper backdrop, like a print), Glow Orbs (soft luminous spheres trailing light on a deep gradient), Solid Tubes (smooth shaded, glossy tubes) and Neon Rings (bright line-light on near black). Bass swells the orbs, every beat flares the glow and the rims, the mids push the orbs faster along their paths and the highs make the hatching shimmer. At rest a few orbs float slowly with short trails; during a build the trails stretch, the orbs tighten into a whirling orbit and start to tremble; the drop throws them outward in a shockwave with a flash of colour, and the peak fills the screen with every orb, long trails and full colour.",
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
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
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
    if (s < 0.5) return vec4(1.0, 0.35, 1.0, 0.25);    // Slinky Lines
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

// ── orb paths ───────────────────────────────────────────────────────────
float gAX, gAY, gTight, gSize, gCnt;
vec3 orbPath(vec4 fr, vec4 ph, float k, float t) {
    vec2 p = vec2(0.72 * sin(t * fr.x + ph.x) + 0.28 * sin(t * fr.z * 1.9 + ph.z),
                  0.70 * sin(t * fr.y + ph.y) + 0.30 * sin(t * fr.w * 1.6 + ph.w)) * vec2(gAX, gAY);
    float z = sin(t * (fr.x + fr.y) * 0.6 + ph.z + ph.w);
    // build: tighten into a whirling orbit around the centre
    float a = t * 1.1 + k * TAU / max(gCnt, 1.0);
    vec2 o = vec2(cos(a), sin(a)) * (0.27 + 0.05 * sin(t * 0.7 + k)) * vec2(1.25, 1.0);
    p = mix(p, o, gTight);
    z = mix(z, sin(a), gTight);
    // drop: burst outward
    p *= 1.0 + 0.7 * gDrop;
    return vec3(p, z);
}

vec4 scene(vec2 fc) {
    vec2 R = RENDERSIZE.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float px = 1.0 / R.y;
    float ar = audioReact;
    float asp = R.x / R.y;

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

    // clocks (additive only)
    float T = TIME * 0.42 * speed + (audioMidTime * 0.35 + audioBassTime * 0.1) * ar + 1.4 * gBuild * gBuild;
    float cT = TIME * 0.06 * colourSpeed + audioHighTime * 0.02 * ar;

    // build tremor near the top
    float trem = smoothstep(0.65, 1.0, gBuild);
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.008 * trem;

    gAX = asp * 0.5 - 0.12;
    gAY = 0.37;
    gTight = 0.75 * smoothstep(0.0, 0.9, gBuild);
    gCnt = clamp(mix(orbCount, min(orbCount, 3.0), smoothstep(0.3, 0.75, gRest)), 1.0, 6.0);
    gCnt = mix(gCnt, 6.0, gPeak);
    gSize = 0.12 * orbSize * (1.0 + 0.16 * bassP() * ar + 0.12 * gDrop - 0.1 * gTight) * (1.0 + 0.1 * gRest);
    float L = 4.2 * trailLen * (1.0 - 0.55 * gRest + 0.8 * gBuild + 0.35 * gPeak);
    float ds = L / float(NS - 1);
    float dens = 26.0 * ringDensity;                 // rings per clock unit
    float wpx = lineThick * max(R.y / 600.0, 0.8);   // line width in pixels
    float beat = audioBeatPulse * ar;
    float satK = 0.85 + 0.15 * gLift + 0.1 * gPeak - 0.1 * gRest;

    vec3 bg = backdrop(uv);
    vec3 colF = vec3(0.0); float aF = 0.0; float zF = -1e3;   // front layer
    vec3 colB = vec3(0.0); float aB = 0.0; float zB = -1e3;   // layer behind
    vec3 glowAdd = vec3(0.0);
    vec2 Lgt = normalize(vec2(-0.5, 0.7));

    for (int k = 0; k < 6; k++) {
        float fk = float(k);
        if (fk > gCnt - 0.02) break;
        float vis = clamp(gCnt - fk, 0.0, 1.0);
        vec4 fr = vec4(0.33 + 0.2 * h11(fk * 7.13 + 1.1), 0.41 + 0.22 * h11(fk * 3.71 + 2.3),
                       0.3 + 0.2 * h11(fk * 5.3 + 3.7), 0.35 + 0.2 * h11(fk * 9.1 + 4.9));
        vec4 ph = vec4(h11(fk * 1.7 + 0.3), h11(fk * 2.9 + 0.7), h11(fk * 4.1 + 0.9), h11(fk * 6.7 + 1.3)) * TAU;
        float rk = gSize * (0.8 + 0.4 * h11(fk * 11.3 + 5.0)) * mix(0.6, 1.0, vis);
        vis = smoothstep(0.0, 1.0, vis);
        float uk = fk * 0.29 + 0.13;

        // ── coarse search, newest first: first disc that covers the pixel ──
        float sHit = -1.0;
        float dPrev = 1.0;
        float dmin = 1e3, sMin = 0.0, zMin = 0.0, dHead = 1e3;
        float gwk = rk * (0.25 + 0.35 * sGlow);
        float gW = 0.0, gS = 0.0;
        vec3 cHead = vec3(0.0);
        for (int i = 0; i < NS; i++) {
            float s = float(i) * ds;
            vec3 c = orbPath(fr, ph, fk, T - s);
            float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, s));
            float d = length(sp - c.xy) - r;
            if (i == 0) { dHead = d; cHead = c; }
            if (d < dmin) { dmin = d; sMin = s; zMin = c.z; }
            if (d < 0.0) { sHit = s; break; }
            float gwt = exp(-d / gwk) * (1.0 - smoothstep(0.6 * L, L, s));
            gW += gwt; gS += gwt * s;
            dPrev = d;
        }
        // close the scallops between coarse samples at the tube silhouette
        if (sHit < 0.0 && dmin < rk * 0.35) {
            for (int j = 0; j < 2; j++) {
                float s = clamp(sMin + (float(j) - 0.5) * ds, 0.0, L);
                vec3 c = orbPath(fr, ph, fk, T - s);
                float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, s));
                float d = length(sp - c.xy) - r;
                if (d < dmin) { dmin = d; zMin = c.z; }
            }
        }
        float u, z, alpha;
        vec3 col;
        float cover = sHit >= 0.0 ? 1.0 : smoothstep(px, -px, dmin);
        if (sHit < -0.5 && cover < 0.001) {
            // only glow out here (smooth sum over the whole trail)
            vec3 gc = ramp(uk + (T - gS / max(gW, 1e-5)) * 0.09 + cT + 0.33);
            glowAdd += gc * (1.0 - exp(-gW * 0.22)) * (0.12 + 0.5 * sGlow) * (glow * (0.6 + 0.4 * gLift) + 0.8 * beat);
            continue;
        }
        bool exact = sHit >= 0.0;
        if (!exact) sHit = sMin;

        if (sHit < 0.5 * ds) {
            // ── head: the newest circle, solid with soft sphere shading ──
            vec3 c = cHead;
            float r = rk * (1.0 + 0.3 * c.z);
            vec2 q = (sp - c.xy) / r;
            float rho = clamp(length(q), 0.0, 1.0);
            vec3 N = vec3(q, sqrt(max(1.0 - rho * rho, 0.0)));
            u = uk + T * 0.09 + cT;
            z = c.z;
            vec3 base = ramp(u) * satK;
            float dif = clamp(dot(N, normalize(vec3(-0.45, 0.6, 0.65))), 0.0, 1.0);
            float spec = pow(clamp(dot(reflect(vec3(0.0, 0.0, -1.0), N), normalize(vec3(-0.35, 0.5, 0.8))), 0.0, 1.0), 24.0);
            float shK = clamp(sphereShade * (0.4 + sShade), 0.0, 1.0);
            vec3 shadowC = ramp(u - 0.12) * satK * 0.55;
            col = mix(base, mix(shadowC, base * 1.08, smoothstep(0.0, 0.9, dif)), shK) + spec * shK * 0.45;
            // glowing sphere core for the light looks
            col = mix(col, base * (0.5 + 1.1 * N.z * N.z) + vec3(1.0) * pow(N.z, 6.0) * 0.45, sGlow * (1.0 - sPale) * (1.0 - sNeon) * 0.8);
            // neon: dark body, bright outline
            col = mix(col, base * (0.12 + 0.9 * pow(rho, 5.0)) + bg * 0.55, sNeon);
            // outline + saturated rim
            float edgePx = (1.0 - rho) * r / px;
            float rimC = smoothstep(wpx * 1.5 + 1.0, wpx * 0.5, edgePx);
            vec3 rc = ramp(u + 0.33) * satK;
            col = mix(col, mix(rc * 0.8, rc * 1.6 + 0.15, sNeon), rimC * (0.6 + 0.4 * sNeon));
            col = mix(col, rc * 1.1, pow(rho, 8.0) * 0.35 * (1.0 - sNeon));
            alpha = mix(cover, 1.0, step(0.0, -dHead)) * mix(1.0, 0.8, sNeon);
            // halo around the head
            float gw = r * (0.35 + 0.4 * sGlow);
            glowAdd += base * exp(-max(dHead, 0.0) / gw) * 0.45 * sGlow * (glow + beat);
        } else {
            // ── body: refine the newest covering parameter (bisection + secant) ──
            float lo = max(sHit - ds, 0.0), hi = sHit;
            float flo = dPrev, fhi = -1.0;
            bool moved = false;
            if (!exact) { lo = hi; flo = 1.0; moved = true; fhi = -1.0; }
            else for (int b = 0; b < 5; b++) {
                float sm2 = 0.5 * (lo + hi);
                vec3 c = orbPath(fr, ph, fk, T - sm2);
                float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, sm2));
                float d = length(sp - c.xy) - r;
                if (d < 0.0) { hi = sm2; fhi = d; moved = true; } else { lo = sm2; flo = d; }
            }
            if (!moved) { // hi never moved: evaluate it
                vec3 c = orbPath(fr, ph, fk, T - hi);
                float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, hi));
                fhi = length(sp - c.xy) - r;
            }
            float sS = mix(lo, hi, clamp(flo / max(flo - fhi, 1e-6), 0.0, 1.0));
            float dfds = abs(flo - fhi) / max(hi - lo, 1e-6);
            if (!exact) { sS = hi; dfds = 0.3; }
            vec3 c = orbPath(fr, ph, fk, T - sS);
            vec3 c2 = orbPath(fr, ph, fk, T - sS + 0.02);
            float r = rk * (1.0 + 0.3 * c.z) * (1.0 - 0.4 * smoothstep(0.35 * L, L, sS));
            z = c.z;
            vec2 n = normalize(sp - c.xy + 1e-6);
            vec2 v = normalize(c2.xy - c.xy + 1e-6);         // toward the head
            float ndv = dot(n, v);
            float perp = sqrt(max(1.0 - ndv * ndv, 0.0));   // 1 at the tube's silhouette
            u = uk + (T - sS) * 0.09 + cT;
            vec3 base = ramp(u) * satK;
            vec3 rc = ramp(u + 0.33) * satK;
            // smooth tube shading (sphere swept along the path)
            vec2 n2 = n - ndv * v;
            vec3 N = normalize(vec3(n2, sqrt(max(1.0 - perp * perp, 0.0)) + 0.15));
            float dif = clamp(dot(N, normalize(vec3(-0.45, 0.6, 0.65))), 0.0, 1.0);
            float spec = pow(clamp(dot(reflect(vec3(0.0, 0.0, -1.0), N), normalize(vec3(-0.35, 0.5, 0.8))), 0.0, 1.0), 20.0);
            vec3 shadowC = ramp(u - 0.12) * satK * 0.55;
            vec3 fillC = mix(base, mix(shadowC, base * 1.08, smoothstep(0.0, 0.9, dif)), sShade) + spec * sShade * 0.4;
            // saturated rims where the tube turns away
            fillC = mix(fillC, rc * (1.0 + 0.3 * beat), pow(perp, 5.0) * (0.55 + 0.25 * beat));
            // glow look: luminous body
            fillC = mix(fillC, base * 1.25 + 0.12, (1.0 - sTrailA) * 2.0);
            // neon: dark body
            fillC = mix(fillC, base * 0.08 + bg * 0.35, sNeon);
            // ── hatching: one ring per 1/dens of the absolute clock, so rings stay where laid ──
            float q = (T - sS) * dens;
            float pxPerQ = dfds / dens / px;               // pixels between neighbouring rings
            float fq = fract(q);
            float dq = min(fq, 1.0 - fq) * pxPerQ;
            float line = 1.0 - smoothstep(wpx * 0.5 - 0.5, wpx * 0.5 + 0.6, dq);
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
        alpha *= vis;
        // composite: keep the two nearest layers by depth (z toward the viewer)
        if (z > zF) { colB = colF; aB = aF; zB = zF; colF = col; aF = alpha; zF = z; }
        else if (z > zB) { colB = col; aB = alpha; zB = z; }
    }

    vec3 col = bg;
    col = mix(col, colB, aB);
    col = mix(col, colF, aF);
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
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    if (PASSINDEX == 0) {
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
