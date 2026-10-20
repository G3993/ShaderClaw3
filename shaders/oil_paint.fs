/*{
  "DESCRIPTION": "Oil Paint — a living oil painting laid down stroke by stroke. A structure-tensor flow field follows every contour of the scene, an anisotropic Kuwahara filter flattens it into clean areas of colour, and three coats of real brush strokes are splatted along the flow: big body strokes, a second coat and fine detail strokes on the edges, each one with its own pigment load (broken colour, a dirty-brush second tone, bristle streaks), a blunt loaded start that tapers dry, ragged bristle edges and a curve that bends with the flow. The paint has real thickness: ridges pushed to the stroke edges, bristle grooves and blobs where the brush landed are lit by a raking gallery light with wet sheen, groove occlusion and canvas weave in the thin passages. Three scenes cycle and repaint themselves as they move — a swirling starry nocturne with a flame cypress over wheat, a burning sunset over a field of sunflowers, and a gestural abstraction on a black ground — or bind any image and it is painted instead. Bass rakes the light lower so the impasto leaps out and pushes the swirl, beats catch the wet sheen, mids thicken the paint.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #4",
  "CATEGORIES": ["Generator", "Painterly", "Filter", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "inputImage",   "TYPE": "image" },
    { "NAME": "speed",        "LABEL": "Scene Speed",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Speed" },
    { "NAME": "swirl",        "LABEL": "Sky Swirl",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "lightMove",    "LABEL": "Light Drift",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.4, "GROUP": "Speed" },
    { "NAME": "scene",        "LABEL": "Scene (nocturne→sunflowers→abstract)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.0, "GROUP": "Scene" },
    { "NAME": "autoCycle",    "LABEL": "Auto Cycle (s, 0 = off)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 120.0, "DEFAULT": 40.0, "GROUP": "Scene" },
    { "NAME": "strokeLength", "LABEL": "Stroke Length",    "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Brush" },
    { "NAME": "brushSize",    "LABEL": "Brush Size",       "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Brush" },
    { "NAME": "sharpness",    "LABEL": "Edge Sharpness",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Brush" },
    { "NAME": "bristles",     "LABEL": "Bristle Detail",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Brush" },
    { "NAME": "impasto",      "LABEL": "Impasto Thickness","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Brush" },
    { "NAME": "brokenColor",  "LABEL": "Broken Colour",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Brush" },
    { "NAME": "skyColor",     "LABEL": "Sky Colour",       "TYPE": "color", "DEFAULT": [0.04, 0.12, 0.62, 1.0], "GROUP": "Color" },
    { "NAME": "sunColor",     "LABEL": "Sun Colour",       "TYPE": "color", "DEFAULT": [1.0, 0.78, 0.08, 1.0],  "GROUP": "Color" },
    { "NAME": "landColor",    "LABEL": "Land Colour",      "TYPE": "color", "DEFAULT": [0.02, 0.32, 0.22, 1.0], "GROUP": "Color" },
    { "NAME": "accentColor",  "LABEL": "Accent Colour",    "TYPE": "color", "DEFAULT": [0.95, 0.12, 0.06, 1.0], "GROUP": "Color" },
    { "NAME": "hueShift",     "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "saturation",   "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "lightAmt",     "LABEL": "Gallery Light",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "lightAz",      "LABEL": "Light Direction",  "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 360.0, "DEFAULT": 135.0, "GROUP": "Light" },
    { "NAME": "lightEl",      "LABEL": "Light Rake (low = grazing)", "TYPE": "float", "AUDIO": false, "MIN": 8.0, "MAX": 80.0, "DEFAULT": 30.0, "GROUP": "Light" },
    { "NAME": "sheen",        "LABEL": "Wet Sheen",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Light" },
    { "NAME": "canvasWeave",  "LABEL": "Canvas Weave",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Light" },
    { "NAME": "bloom",        "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.5, "GROUP": "Finish" },
    { "NAME": "grain",        "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "vignette",     "LABEL": "Vignette",         "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 0.15, "DEFAULT": 0.1, "GROUP": "Finish" },
    { "NAME": "simRes",       "LABEL": "Filter Resolution","TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Finish" },
    { "NAME": "audioReact",   "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",   "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "srcBuf",  "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "tenBuf",  "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "flowBuf", "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "kuwBuf",  "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "paintBuf" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Oil Paint — FRONTIER pack #4
//   pass 0 srcBuf   : the living source (internal landscape, or inputImage)
//   pass 1 tenBuf   : structure tensor (per-channel Sobel → E, F, G)
//   pass 2 flowBuf  : smoothed tensor → stroke direction (eigenvector) +
//                     anisotropy + edge strength
//   pass 3 kuwBuf   : anisotropic (generalised) Kuwahara — 8 sectors on an
//                     ellipse stretched along the flow, variance-weighted
//   pass 4 paintBuf : brush pass — line-integral convolution along the flow
//                     of the Kuwahara colour + stroke seeds + bristle noise.
//                     rgb = paint, a = paint height
//   pass 5 final    : impasto — normals from the height, raking gallery light,
//                     wet sheen, groove occlusion, canvas weave, bloom, ACES,
//                     HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R   RENDERSIZE.xy
#define TAU 6.28318530718
#define PI  3.14159265359

float hash12(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 5; i++) { s += a * vnoise(p); p = mat2(1.6, 1.2, -1.2, 1.6) * p + 7.3; a *= 0.5; }
    return s;
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { return clamp((x * (2.51 * x + 0.03)) / (x * (2.43 * x + 0.59) + 0.14), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

float g_ar() { return clamp(audioReact, 0.0, 1.0); }
float bassP() { return pow(smoothstep(0.05, 0.85, audioBass), 1.4) * g_ar(); }
float midP()  { return pow(smoothstep(0.06, 0.85, audioMid), 1.2) * g_ar(); }
float highP() { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * g_ar(); }
float beatP() { return clamp(audioBeatPulse, 0.0, 1.0) * g_ar(); }

bool hasImage() { return IMG_SIZE_inputImage.x > 0.5; }

// ═════════════════════════════════════════════════════════════════════════
//  THE LIVING SOURCE  (pass 0)
// ═════════════════════════════════════════════════════════════════════════
float sceneClock() { return TIME * speed + 0.6 * bassP(); }

// swirling sky warp: two big vortices + curl-ish fbm warp
vec2 skyWarp(vec2 p, float t) {
    float sw = swirl * (1.0 + 0.25 * bassP());
    vec2 c1 = vec2(-0.25 + 0.08 * sin(t * 0.11), 0.22 + 0.04 * cos(t * 0.13));
    vec2 c2 = vec2(0.42 + 0.06 * cos(t * 0.09), 0.30);
    vec2 d1 = p - c1, d2 = p - c2;
    float a1 = sw * 2.6 * exp(-dot(d1, d1) * 9.0) * (1.0 + 0.3 * sin(t * 0.3));
    float a2 = -sw * 1.8 * exp(-dot(d2, d2) * 14.0);
    d1 = mat2(cos(a1), -sin(a1), sin(a1), cos(a1)) * d1;
    p = c1 + d1;
    d2 = p - c2;
    d2 = mat2(cos(a2), -sin(a2), sin(a2), cos(a2)) * d2;
    p = c2 + d2;
    vec2 q = vec2(fbm(p * 2.2 + vec2(t * 0.05, 0.0)), fbm(p * 2.2 + vec2(5.2, 1.3 - t * 0.04)));
    return p + sw * 0.18 * (q - 0.5) + vec2(t * 0.025 * sw, 0.0);
}

// halo rings around a light (sun / stars)
vec3 halo(vec2 p, vec2 c, float r, vec3 col, float t, float rings) {
    vec2 d = p - c;
    float l = length(d);
    float ang = atan(d.y, d.x);
    float disc = smoothstep(r, r * 0.92, l);
    float ring = 0.5 + 0.5 * cos((l / r) * rings * PI - t * 0.8 + 0.6 * sin(ang * 3.0 + t * 0.2));
    float env = exp(-max(l - r, 0.0) / (r * 1.8));
    vec3 c2 = mix(col, col * vec3(1.0, 0.75, 0.4) + vec3(0.0, 0.05, 0.0), ring);
    return disc * mix(col * 1.15, vec3(1.0, 0.97, 0.75), smoothstep(r * 0.7, 0.0, l)) + (1.0 - disc) * c2 * env * (0.45 + 0.55 * ring);
}

vec3 sceneNocturne(vec2 p, float t) {
    float horizon = -0.12 + 0.05 * sin(p.x * 3.1 + 0.7) + 0.03 * sin(p.x * 7.3 + t * 0.05);
    vec3 col;
    // ── sky
    vec2 q = skyWarp(p, t);
    float f = fbm(q * 3.0);
    float bands = 0.5 + 0.5 * sin(f * 14.0 + q.y * 6.0 - t * 0.15);
    vec3 deep = skyColor.rgb * 0.24 + vec3(0.0, 0.0, 0.03);
    vec3 mid  = skyColor.rgb;
    vec3 pale = mix(skyColor.rgb, vec3(0.3, 0.7, 1.0), 0.55);
    vec3 sky = mix(deep, mid, smoothstep(0.28, 0.62, f));
    sky = mix(sky, pale, smoothstep(0.6, 0.95, bands) * smoothstep(0.42, 0.72, f) * 0.85);
    sky = mix(sky, mix(sunColor.rgb, vec3(1.0), 0.25), smoothstep(0.86, 1.0, bands) * smoothstep(0.5, 0.75, f) * 0.6);
    sky = mix(sky, landColor.rgb * 0.8 + vec3(0.0, 0.15, 0.2), smoothstep(0.25, -0.05, p.y - horizon) * 0.5);
    // moon + stars
    vec2 mc = vec2(0.58 + 0.05 * sin(t * 0.07), 0.32 + 0.02 * cos(t * 0.05));
    sky += halo(p, mc, 0.075 * (1.0 + 0.06 * beatP()), sunColor.rgb, t, 7.0) * 0.8;
    for (int i = 0; i < 9; i++) {
        float fi = float(i);
        vec2 sc = vec2(-0.85 + fi * 0.21 + 0.05 * sin(fi * 7.1), 0.12 + 0.3 * fract(sin(fi * 12.9) * 43.7));
        sc += 0.01 * vec2(sin(t * 0.3 + fi), cos(t * 0.27 + fi * 2.0));
        float sr = 0.018 + 0.014 * fract(fi * 0.618);
        if (abs(sc.x - mc.x) > 0.12 || abs(sc.y - mc.y) > 0.1)
            sky += halo(p, sc, sr * (1.0 + 0.25 * highP()), mix(sunColor.rgb, vec3(0.75, 0.95, 1.0), 0.55), t + fi, 4.0) * 0.9;
    }
    col = sky;
    // ── hills (three layers)
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        float hh = horizon - 0.05 - fi * 0.12 + 0.08 * fbm(vec2(p.x * (1.2 + fi * 0.6) + fi * 3.0 + t * 0.01 * (fi + 1.0), fi));
        float m = smoothstep(0.004, -0.004, p.y - hh);
        float stri = fbm(vec2(p.x * 3.0, (p.y - hh) * 18.0) + fi * 5.0);
        vec3 hc = mix(mix(skyColor.rgb * 0.5 + vec3(0.12, 0.05, 0.25), landColor.rgb, fi * 0.5), mix(sunColor.rgb * 0.7, accentColor.rgb, 0.3), smoothstep(0.55, 0.8, stri) * (0.25 + 0.3 * fi));
        hc *= 0.65 + 0.5 * stri - 0.15 * fi;
        hc *= 0.35 + 0.65 * smoothstep(0.0, 0.025, hh - p.y);   // dark contour along the ridge
        hc *= 1.0 - 0.5 * smoothstep(0.82, 0.96, sin((hh - p.y) * 70.0 + fbm(p * 4.0 + fi) * 6.0)) * smoothstep(0.5, 0.7, fbm(p * 3.0 - fi));   // dark furrow strokes
        col = mix(col, hc, m);
    }
    // ── village under the hills: dark structure strokes against the bands
    float vb = horizon - 0.215 + 0.02 * sin(p.x * 3.0 + 1.0);
    if (p.x > -0.36 && p.x < 0.95 && p.y < vb + 0.09 && p.y > vb - 0.03) {
        float cw = 0.052;
        float hid = floor(p.x / cw);
        float hx = (p.x / cw - hid) - 0.5;
        float hh1 = hash12(vec2(hid, 4.0));
        float hgt = 0.022 + 0.022 * hh1;
        float hw = 0.32 + 0.12 * hash12(vec2(hid, 5.0));
        float base = vb + 0.01 * (hash12(vec2(hid, 6.0)) - 0.5);
        float yy = p.y - base;
        float wall = step(abs(hx), hw) * step(0.0, yy) * step(yy, hgt);
        float roof = step(0.0, yy - hgt) * step(yy - hgt, (hw + 0.06 - abs(hx)) * cw * 0.85);
        float spire = 0.0;
        if (abs(hid - 7.0) < 0.5) spire = step(abs(hx), 0.07 + 0.18 * max(0.0, 1.0 - (yy - hgt) / 0.07)) * step(0.0, yy) * step(yy, hgt + 0.075);
        float house = max(max(wall, roof), spire) * step(0.3, hash12(vec2(hid, 9.0)) + spire);
        vec3 wallC = mix(skyColor.rgb * 0.22 + vec3(0.03, 0.02, 0.05), landColor.rgb * 0.3, hh1);
        vec3 roofC = mix(accentColor.rgb * 0.3, skyColor.rgb * 0.3, hash12(vec2(hid, 7.0)));
        vec3 hc2 = mix(wallC, roofC, roof);
        // lit windows
        vec2 wg = vec2(hx * 6.0, yy / hgt * 2.0);
        vec2 wf = fract(wg) - 0.5;
        float win = wall * step(0.55, hash12(floor(wg) + hid * 3.1)) * step(max(abs(wf.x), abs(wf.y)), 0.22) * step(0.25, yy / hgt) * step(yy / hgt, 0.85);
        hc2 = mix(hc2, sunColor.rgb * 1.15, win);
        col = mix(col, hc2, house);
    }
    // ── foreground wheat: rolling ochre / green rows
    float fgH = -0.36 + 0.06 * sin(p.x * 2.4 + 0.5);
    if (p.y < fgH + 0.01) {
        float m = smoothstep(0.005, -0.005, p.y - fgH);
        float rows = fbm(vec2(p.x * 2.5 + sin(p.y * 6.0) * 0.4, (p.y - fgH) * 14.0) + vec2(t * 0.02, 0.0));
        vec3 wc = mix(landColor.rgb * 0.45, mix(sunColor.rgb, accentColor.rgb, 0.25) * 0.85, smoothstep(0.45, 0.68, rows));
        wc = mix(wc, landColor.rgb * 1.2, smoothstep(0.7, 0.8, rows) * 0.6);
        wc *= 0.45 + 0.55 * smoothstep(0.0, 0.03, fgH - p.y);
        col = mix(col, wc, m);
    }
    // ── cypress: a dark twisting flame of vertical writhing strokes
    float sway = 0.03 * sin(t * 0.4 + p.y * 2.5) * (1.0 + 0.6 * bassP());
    float cx = -0.52 + sway * (p.y + 0.5) + 0.014 * sin(p.y * 7.0 - t * 0.45);
    float tall = clamp((p.y + 0.62) / 1.0, 0.0, 1.0);
    float lobes = 0.5 + 0.5 * sin(p.y * 13.0 - t * 0.55 + 2.2 * fbm(vec2(p.y * 3.0, t * 0.05)));
    float w = 0.11 * pow(1.0 - tall, 0.65) * (0.5 + 0.7 * lobes);
    cx += 0.018 * sin(p.y * 13.0 - t * 0.55 + 1.0) * (1.0 - tall * 0.5);   // flame tongues lick sideways
    cx += 0.02 * sin(p.y * 4.0 + 1.3) * tall;                      // the tip curls
    float xr = (p.x - cx) / max(w, 1e-3);                          // -1..1 across the trunk
    float tree = smoothstep(0.006, -0.004, abs(p.x - cx) - w) * step(p.y, 0.38);
    float strand  = 0.5 + 0.5 * sin(xr * 2.6 + 1.9 * sin(p.y * 6.5 - t * 0.4 + xr * 1.5) + p.y * 3.0);
    float strand2 = fbm(vec2(xr * 1.6, p.y * 4.0 - t * 0.2));
    vec3 tc = landColor.rgb * 0.16 + vec3(0.0, 0.015, 0.03);
    tc = mix(tc, landColor.rgb * 0.8 + vec3(0.0, 0.04, 0.05), smoothstep(0.12, 0.5, strand));
    tc = mix(tc, vec3(0.08, 0.28, 0.5) * (0.7 + 0.6 * skyColor.b), smoothstep(0.8, 0.97, strand) * 0.85);
    tc = mix(tc, vec3(0.3, 0.34, 0.06) + landColor.rgb * 0.15, smoothstep(0.6, 0.78, strand2) * smoothstep(0.4, 0.9, strand) * 0.8);
    tc *= 0.7 + 0.55 * smoothstep(-1.0, 0.8, xr);                  // moonlit side
    col = mix(col, tc, tree);
    return col;
}

vec3 sceneSunflowers(vec2 p, float t) {
    // dusk sky burning down to the horizon over a field of heavy sunflower heads
    float horizon = 0.06 + 0.035 * sin(p.x * 2.1 + 1.0);
    vec2 q = skyWarp(p * 0.8, t * 0.7);
    float f = fbm(q * 2.5);
    float bands = 0.5 + 0.5 * sin(f * 11.0 + q.y * 5.0);
    float hy = p.y - horizon;
    vec3 sky = mix(sunColor.rgb * vec3(1.0, 0.85, 0.6), accentColor.rgb, smoothstep(0.02, 0.2, hy));
    sky = mix(sky, mix(accentColor.rgb, vec3(0.45, 0.04, 0.32), 0.65), smoothstep(0.16, 0.32, hy));
    sky = mix(sky, skyColor.rgb * 0.55, smoothstep(0.28, 0.48, hy));
    sky *= 0.78 + 0.4 * smoothstep(0.25, 0.75, f);
    sky = mix(sky, mix(sunColor.rgb, vec3(1.0, 0.9, 0.6), 0.3), smoothstep(0.7, 0.98, bands) * 0.45 * smoothstep(0.45, 0.1, hy));
    sky = mix(sky, skyColor.rgb * 0.15, smoothstep(0.62, 0.7, fbm(q * 1.6 + 3.0)) * smoothstep(0.1, 0.3, hy) * 0.7);
    vec2 sc = vec2(0.18 + 0.05 * sin(t * 0.05), horizon + 0.07 + 0.02 * sin(t * 0.08));
    sky += halo(p, sc, 0.075 * (1.0 + 0.06 * beatP()), sunColor.rgb, t, 6.0) * 0.55;
    vec3 col = sky;
    // dark far hills, then the leafy field
    float ff = smoothstep(0.004, -0.004, hy);
    float leaf = fbm(vec2(p.x * 7.0, p.y * 9.0) + vec2(0.0, t * 0.02));
    vec3 field = mix(landColor.rgb * 0.22, landColor.rgb * 0.85, smoothstep(0.35, 0.75, leaf));
    field = mix(field, skyColor.rgb * 0.35, smoothstep(-0.12, 0.0, hy) * 0.7);
    col = mix(col, field, ff);
    // sunflower rows, far → near, painter's order
    for (int r = 0; r < 5; r++) {
        float fr = float(r);
        float near = fr / 4.0;                              // 0 far, 1 near
        float rowY = horizon - 0.05 - pow(near, 1.4) * 0.6;
        float sz0 = mix(0.028, 0.115, near * near);
        float sz = sz0;
        float sp = sz0 * 2.6;
        float xx = p.x / sp + fr * 0.37;
        for (int k = 0; k < 2; k++) {
            sz = sz0;
            float id = floor(xx) + float(k) - (fract(xx) < 0.5 ? 1.0 : 0.0);
            float hr = hash12(vec2(id, fr));
            if (hash12(vec2(id * 1.3, fr + 7.0)) < 0.18) continue;      // gaps in the rows
            sz *= 0.82 + 0.36 * hash12(vec2(id, fr + 3.0));
            float bob = 0.012 * sin(t * 0.8 + id * 1.7 + fr) * (1.0 + bassP()) * (0.4 + near);
            vec2 c = vec2((id + 0.5 + 0.35 * (hr - 0.5)) * sp - fr * 0.37 * sp, rowY + bob + sz * 0.5 * (hr - 0.5));
            vec2 d = p - c;
            // stem + a leaf
            float stem = smoothstep(sz * 0.1, sz * 0.05, abs(d.x + 0.12 * d.y)) * step(d.y, 0.0);
            vec2 ld = d - vec2(sz * 0.5 * (hr > 0.5 ? 1.0 : -1.0), -sz * 1.3);
            float lf = smoothstep(0.0, -sz * 0.04, length(ld * vec2(0.7, 1.6)) - sz * 0.55);
            col = mix(col, landColor.rgb * (0.45 + 0.5 * hr), max(stem, lf * 0.9));
            // head: varied size, facing (rotation + foreshortening), some seen from behind
            float hf = hash12(vec2(id, fr + 11.0));
            sz *= 0.75 + 0.6 * hash12(vec2(id, fr + 13.0));
            float rot = (hash12(vec2(id, fr + 17.0)) - 0.5) * 1.6 + 0.15 * sin(t * 0.3 + id);
            float squash = mix(mix(0.78, 0.42, near), 1.0, hash12(vec2(id, fr + 19.0)));
            float cr = cos(rot), sr = sin(rot);
            vec2 hd = vec2(cr * d.x + sr * d.y, -sr * d.x + cr * d.y);
            hd.y /= squash;
            float l = length(hd);
            float ang = atan(hd.y, hd.x);
            bool back = hf < 0.16 && near > 0.3;
            // petals: two rings of individual fat petals
            for (int ring = 0; ring < 2; ring++) {
                float rg = float(ring);
                float N = 14.0;
                float sec = TAU / N;
                float aa = ang + rg * sec * 0.5 + hr * 3.0;
                float k = floor(aa / sec);
                float la = aa - (k + 0.5) * sec;
                float ph = hash12(vec2(k + rg * 31.0, id + fr * 7.0));
                float rIn = sz * 0.36, rOut = sz * (0.88 + 0.22 * ph) * (back ? 0.9 : 1.0);
                float along = l * cos(la), across = l * sin(la);
                float rc = 0.5 * (rIn + rOut), hl = 0.5 * (rOut - rIn);
                float hwid = sz * (0.13 + 0.04 * ph) * (1.0 - 0.5 * smoothstep(0.0, 1.0, (along - rc) / hl));
                float pe = length(vec2((along - rc) / hl, across / max(hwid, 1e-4)));
                float petal = smoothstep(1.0, 0.9, pe);
                vec3 pc = sunColor.rgb * (0.7 + 0.45 * ph) * (ring == 0 ? 0.72 : 1.0);
                pc = mix(pc, mix(sunColor.rgb, accentColor.rgb, 0.6), smoothstep(0.2, 1.0, (along - rc) / hl) * 0.6);
                pc *= 0.8 + 0.3 * smoothstep(1.0, 0.0, abs(across) / max(hwid, 1e-4));   // petal midrib catches light
                col = mix(col, pc, petal);
            }
            if (back) {
                // back of the head: green calyx with pointed sepals
                float sep = sz * (0.52 + 0.2 * pow(abs(cos(ang * 6.0 + hr * 5.0)), 3.0));
                float calyx = smoothstep(sep, sep - 0.003, l);
                vec3 gc = landColor.rgb * (0.5 + 0.5 * smoothstep(sz * 0.55, 0.0, l)) * (0.8 + 0.3 * vnoise(hd / sz * 6.0));
                col = mix(col, gc, calyx);
            } else {
                // seed disc: phyllotaxis spirals, near-black with ochre seed tops, floret ring
                float rr = sqrt(l / sz);
                float s1 = fract(ang / TAU * 21.0 + rr * 9.0);
                float s2 = fract(-ang / TAU * 34.0 + rr * 9.0);
                float seed = smoothstep(0.35, 0.0, abs(s1 - 0.5) * abs(s2 - 0.5) * 8.0);
                float core = smoothstep(sz * 0.42, sz * 0.38, l);
                vec3 cc = mix(vec3(0.03, 0.015, 0.008), mix(accentColor.rgb, sunColor.rgb, 0.3) * 0.32, seed);
                cc = mix(cc, mix(landColor.rgb, sunColor.rgb, 0.5) * 0.55, smoothstep(sz * 0.3, sz * 0.4, l));   // floret ring
                cc *= 0.75 + 0.5 * smoothstep(sz * 0.42, 0.0, l + 0.2 * sz * hd.y / max(l, 1e-4));
                col = mix(col, cc, core);
            }
        }
    }
    return col;
}

vec3 sceneAbstract(vec2 p, float t) {
    // gestural abstraction: colour masses thrown onto a near-black ground
    vec2 q = skyWarp(p * 0.9, t * 1.2);
    vec2 r2 = vec2(fbm(q * 1.7 + t * 0.03), fbm(q * 1.7 + vec2(3.1, -t * 0.025)));
    float a = fbm(q * 1.4 + 2.0 * r2);
    float b = fbm(q * 3.1 - r2 + 9.0);
    float c = fbm(q * 2.2 + 4.0 + r2.yx);
    vec3 col = vec3(0.015, 0.015, 0.03);
    col = mix(col, skyColor.rgb * 1.2,   smoothstep(0.50, 0.56, a) * (1.0 - smoothstep(0.62, 0.68, a)));
    col = mix(col, accentColor.rgb,      smoothstep(0.60, 0.66, b) * (1.0 - smoothstep(0.72, 0.78, b)));
    col = mix(col, sunColor.rgb,         smoothstep(0.64, 0.70, a));
    col = mix(col, landColor.rgb * 1.6,  smoothstep(0.58, 0.63, c) * (1.0 - smoothstep(0.66, 0.70, c)));
    col = mix(col, vec3(0.95, 0.92, 0.86), smoothstep(0.72, 0.76, b) * 0.9);
    col = mix(col, mix(skyColor.rgb, accentColor.rgb, 0.5) * 0.9, smoothstep(0.66, 0.7, c) * 0.8);
    return col;
}
vec3 sourceColor(vec2 uv) {
    if (hasImage()) return texture2D(inputImage, uv).rgb;
    vec2 p = (uv - 0.5) * vec2(R.x / R.y, 1.0);
    float t = sceneClock();
    float s = scene;
    if (autoCycle > 0.5) {
        float k = TIME / autoCycle;
        float seg = floor(k), fr = fract(k);
        s = mod(seg + smoothstep(0.82, 1.0, fr), 3.0);
    }
    vec3 c0 = vec3(0.0), c1 = vec3(0.0), c2 = vec3(0.0);
    float w0 = max(0.0, 1.0 - abs(s - 0.0)) + max(0.0, s - 2.0);
    float w1 = max(0.0, 1.0 - abs(s - 1.0));
    float w2 = max(0.0, 1.0 - abs(s - 2.0)) * step(s, 2.0);
    vec3 col = vec3(0.0);
    if (w0 > 0.001) col += w0 * sceneNocturne(p, t);
    if (w1 > 0.001) col += w1 * sceneSunflowers(p, t);
    if (w2 > 0.001) col += w2 * sceneAbstract(p, t);
    return col;
}

// ═════════════════════════════════════════════════════════════════════════
//  KUWAHARA (pass 3)
// ═════════════════════════════════════════════════════════════════════════
vec3 kuwahara(vec2 uv) {
    vec4 fl = texture2D(flowBuf, uv);
    vec2 dir = normalize(fl.xy + vec2(1e-5, 0.0));
    float A = fl.z;
    float radius = 5.0 * brushSize;            // in filter-buffer pixels
    float alpha = 1.0;
    float a = radius * clamp((alpha + A) / alpha, 0.1, 2.0);
    float b = radius * clamp(alpha / (alpha + A), 0.1, 2.0);
    vec2 px = 1.0 / R;
    // 8 sector directions
    vec3 m0 = vec3(0.0), m1 = vec3(0.0), m2 = vec3(0.0), m3 = vec3(0.0), m4 = vec3(0.0), m5 = vec3(0.0), m6 = vec3(0.0), m7 = vec3(0.0);
    vec3 s0 = vec3(0.0), s1 = vec3(0.0), s2 = vec3(0.0), s3 = vec3(0.0), s4 = vec3(0.0), s5 = vec3(0.0), s6 = vec3(0.0), s7 = vec3(0.0);
    float w0 = 0.0, w1 = 0.0, w2 = 0.0, w3 = 0.0, w4 = 0.0, w5 = 0.0, w6 = 0.0, w7 = 0.0;
    const float D = 0.70710678;
    for (int j = -4; j <= 4; j++) {
        for (int i = -4; i <= 4; i++) {
            vec2 v = vec2(float(i), float(j)) / 4.0;
            float r2 = dot(v, v);
            if (r2 > 1.0) continue;
            vec2 off = dir * (v.x * a) + vec2(-dir.y, dir.x) * (v.y * b);
            vec3 c = texture2D(srcBuf, uv + off * px).rgb;
            vec3 c2 = c * c;
            float g = exp(-r2 * 2.5);
            vec2 n = (r2 > 1e-4) ? v * inversesqrt(r2) : vec2(0.0);
            float k;
            // sector weights: max(0,cos)^8 , centre sample counts everywhere
            #define SECT(W, M, S, DX, DY) k = (r2 > 1e-4) ? max(0.0, dot(n, vec2(DX, DY))) : 0.38; k *= k; k *= k; k *= k; k *= g; W += k; M += k * c; S += k * c2;
            SECT(w0, m0, s0, 1.0, 0.0)
            SECT(w1, m1, s1, D, D)
            SECT(w2, m2, s2, 0.0, 1.0)
            SECT(w3, m3, s3, -D, D)
            SECT(w4, m4, s4, -1.0, 0.0)
            SECT(w5, m5, s5, -D, -D)
            SECT(w6, m6, s6, 0.0, -1.0)
            SECT(w7, m7, s7, D, -D)
        }
    }
    float q = mix(3.0, 12.0, sharpness);
    vec3 outc = vec3(0.0); float wsum = 0.0;
    #define RESOLVE(W, M, S) { vec3 mm = M / W; vec3 vv = abs(S / W - mm * mm); float sg = sqrt(vv.r + vv.g + vv.b) * 12.0; float wt = 1.0 / (1.0 + pow(sg, q)); outc += mm * wt; wsum += wt; }
    RESOLVE(w0, m0, s0) RESOLVE(w1, m1, s1) RESOLVE(w2, m2, s2) RESOLVE(w3, m3, s3)
    RESOLVE(w4, m4, s4) RESOLVE(w5, m5, s5) RESOLVE(w6, m6, s6) RESOLVE(w7, m7, s7)
    return outc / max(wsum, 1e-6);
}

// ═════════════════════════════════════════════════════════════════════════
//  BRUSH (pass 4) — stroke splatting by gather
//   each coat is a jittered grid of strokes; a pixel looks at the strokes of
//   nearby cells, builds each one in its own frame (u along the flow sampled
//   at the stroke centre, v across, bent by the flow at its tip) and keeps the
//   two highest-priority strokes that cover it → composited with AA.
// ═════════════════════════════════════════════════════════════════════════
vec4  g_top;    // rgb + height of the winning stroke
float g_topCov, g_topPri;
vec4  g_sec;
float g_secCov, g_secPri;

//   gate > 0 : coat only where the image has structure (edges, focal points)
//   gate < 0 : coat only in calm areas (big loaded strokes)
//   kind 1   : palette-knife slab (flat plateau, sharp scraped lip) in highlights
void considerStroke(vec2 pix, vec2 id, float cell, float lenMul, float widMul, float seedOff, float gate, float kind, float thick, float pxs) {
    float pri = hash12(id * 1.37 + seedOff);
    vec2  jit = hash22(id + seedOff * 3.1) - 0.5;
    vec2  c   = (id + 0.5 + jit * 0.9) * cell;
    vec2  uvc = c / R;
    vec4  fl  = texture2D(flowBuf, uvc);
    float detail = fl.w;
    float gh = hash12(id + seedOff + 5.0);
    if (gate > 1.5)      { if (gh > smoothstep(0.35, 0.8, detail)) return; }
    else if (gate > 0.5) { if (gh > smoothstep(0.12, 0.5, detail)) return; }
    else if (gate < -0.5){ if (gh > 1.0 - smoothstep(0.1, 0.42, detail)) return; }
    vec3 base = texture2D(kuwBuf, uvc).rgb;
    if (kind > 0.5) { if (gh > smoothstep(0.72, 0.92, lum(base)) * (1.0 - smoothstep(0.2, 0.45, detail)) * 0.45) return; }
    vec2  dir = fl.xy / max(length(fl.xy), 1e-4);
    float ra  = pri * TAU;                      // per-stroke reference keeps the eigenvector's sign stable
    if (dot(dir, vec2(cos(ra), sin(ra))) < 0.0) dir = -dir;
    vec2  per = vec2(-dir.y, dir.x);
    float h2  = hash12(id + seedOff + 9.7);
    float L   = cell * lenMul * (0.75 + 0.6 * h2) * mix(1.0, 0.65, detail) * strokeLength;
    float W   = cell * widMul * (0.75 + 0.5 * pri);
    vec2  d   = pix - c;
    float u   = dot(d, dir);
    float v0  = dot(d, per);
    if (abs(u) > 1.25 * L || abs(v0) > 1.4 * W + 0.8 * L) return;     // cheap reject before the tip fetch
    float bend = 0.0;
    {                                           // bend: flow at the tip vs the centre
        vec2 dt = texture2D(flowBuf, (c + dir * L) / R).xy;
        dt /= max(length(dt), 1e-4);
        if (dot(dt, dir) < 0.0) dt = -dt;
        bend = clamp(dir.x * dt.y - dir.y * dt.x, -0.8, 0.8);
    }
    float v   = v0 - bend * u * u / max(L, 1.0);
    float uu  = u / L;                          // -1 start .. 1 end
    float vv  = v / W;
    if (abs(uu) > 1.25 || abs(vv) > 1.4) return;
    float trackW = max(W * 0.12, 1.7 * max(pxs, 0.6));          // bristle tracks scale with the brush
    float bristle = vnoise(vec2(v / trackW + pri * 50.0, uu * 2.5));
    float e, prof;
    if (kind > 0.5) {
        // knife slab: squarish plateau with a slightly torn trailing edge
        float sq = pow(pow(abs(uu), 2.6) + pow(abs(vv), 2.6), 1.0 / 2.6);
        prof = 1.0;
        e = (sq - 1.0 - 0.06 * (vnoise(vec2(vv * 6.0, pri * 9.0)) - 0.5) * step(0.0, uu)) * min(L, W);
    } else {
        // brush: loaded blunt start, tapering dry end, ragged bristle edge
        prof = sqrt(max(0.0, 1.0 - uu * uu)) * mix(1.0, 0.7, smoothstep(-0.2, 1.0, uu));
        prof = mix(prof, 1.0, smoothstep(-1.0, -0.7, uu) * smoothstep(0.2, -0.5, uu) * 0.5);
        float rag = vnoise(vec2(uu * 4.0 + pri * 30.0, vv * 3.0)) - 0.5;
        e = (abs(vv) - prof * (1.0 + 0.22 * rag) + 0.2 * (1.0 - smoothstep(-0.2, 1.0, uu)) * (bristle - 0.5) * step(0.4, uu)) * W;
    }
    float cov = smoothstep(0.9 * pxs, -0.9 * pxs, e);
    if (cov <= 0.0) return;
    float inside = max(-e, 0.0);                // px from the stroke boundary
    // pigment: stroke colour sampled at its centre, broken colour per stroke
    vec3 col = mix(base, texture2D(kuwBuf, pix / R).rgb, kind > 0.5 ? 0.15 : 0.4);
    float jv = (hash12(id + seedOff + 2.2) - 0.5);
    float jh = (hash12(id + seedOff + 4.4) - 0.5);
    float h;
    if (kind > 0.5) {
        col = mix(col * 1.06, vec3(1.0, 0.97, 0.9), 0.08) * (1.0 + jv * 0.12);   // a little lead white in the pigment
        float scrape = vnoise(vec2(v / (1.3 * max(pxs, 0.6)) + pri * 20.0, uu * 0.6));
        col *= 0.94 + 0.12 * scrape;
        h = thick * (0.95 + 0.08 * (scrape - 0.5) - 0.12 * uu);              // plateau, thins as the knife drags
        h += thick * 0.35 * exp(-inside / (0.1 * min(L, W) + pxs));           // sharp lip the knife leaves
    } else {
        vec3 alt = texture2D(kuwBuf, (c + per * W * 1.4) / R).rgb;          // dirty brush second pigment
        float twoTone = step(0.62, h2) * smoothstep(0.1, 0.6, vv) * smoothstep(0.35, 0.7, bristle);
        col = mix(col, alt, twoTone * 0.8 * brokenColor);
        col = mix(col, hueRot(col, jh * 0.14) * (1.0 + jv * 0.45), brokenColor);
        col *= 1.0 + (bristle - 0.5) * 0.22 * bristles;            // streaks of pigment
        col *= mix(1.0, 0.86, smoothstep(0.3, 1.0, uu));            // brush runs dry
        float load = 0.6 + 0.8 * lum(col);                          // light, loaded paint stands proud
        h = 0.5 * mix(1.0, 0.45, smoothstep(-0.8, 1.0, uu));
        h += 0.36 * exp(-inside / (0.16 * W + pxs)) * (1.0 - smoothstep(0.5, 1.0, uu) * 0.5);  // ridges pushed to the edges
        h += 0.26 * (bristle - 0.5) * bristles;                                              // ridged bristle tracks
        h += 0.30 * smoothstep(-0.75, -0.95, uu) * smoothstep(1.0, 0.5, abs(vv) / max(prof, 0.05));  // blob where the brush landed
        h += 0.42 * smoothstep(0.5, 0.82, uu) * exp(-inside / (0.22 * W + pxs));         // raised lip where the brush lifted
        h *= thick * load;
    }
    h *= cov;
    if (pri > g_topPri) {
        g_sec = g_top; g_secCov = g_topCov; g_secPri = g_topPri;
        g_top = vec4(col, h); g_topCov = cov; g_topPri = pri;
    } else if (pri > g_secPri) {
        g_sec = vec4(col, h); g_secCov = cov; g_secPri = pri;
    }
}

// composite one coat over (col, h)
void coat(vec2 pix, float cell, float lenMul, float widMul, float seedOff, float gate, float kind, float thick, float pxs, inout vec3 col, inout float h) {
    g_top = vec4(0.0); g_sec = vec4(0.0);
    g_topCov = 0.0; g_secCov = 0.0; g_topPri = -1.0; g_secPri = -1.0;
    vec2 id0 = floor(pix / cell);
    for (int j = -2; j <= 2; j++) {
        for (int i = -2; i <= 2; i++) {
            considerStroke(pix, id0 + vec2(float(i), float(j)), cell, lenMul, widMul, seedOff, gate, kind, thick, pxs);
        }
    }
    // second stroke under the first, both over the coat below
    vec3 c2 = mix(col, g_sec.rgb, g_secCov);
    float h2 = mix(h, h * 0.45 + g_sec.a, g_secCov);
    col = mix(c2, g_top.rgb, g_topCov);
    h = mix(h2, h2 * 0.45 + g_top.a, g_topCov);
}

void main() {
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;

    // ── pass 0 : source
    if (PASSINDEX == 0) {
        gl_FragColor = vec4(sourceColor(uv), 1.0);
        return;
    }

    // ── pass 1 : structure tensor
    if (PASSINDEX == 1) {
        vec2 px = 1.0 / R;
        vec3 tl = texture2D(srcBuf, uv + px * vec2(-1.0,  1.0)).rgb;
        vec3 tc = texture2D(srcBuf, uv + px * vec2( 0.0,  1.0)).rgb;
        vec3 tr = texture2D(srcBuf, uv + px * vec2( 1.0,  1.0)).rgb;
        vec3 ml = texture2D(srcBuf, uv + px * vec2(-1.0,  0.0)).rgb;
        vec3 mr = texture2D(srcBuf, uv + px * vec2( 1.0,  0.0)).rgb;
        vec3 bl = texture2D(srcBuf, uv + px * vec2(-1.0, -1.0)).rgb;
        vec3 bc = texture2D(srcBuf, uv + px * vec2( 0.0, -1.0)).rgb;
        vec3 br = texture2D(srcBuf, uv + px * vec2( 1.0, -1.0)).rgb;
        vec3 gx = (tr + 2.0 * mr + br - tl - 2.0 * ml - bl) * 0.25;
        vec3 gy = (tl + 2.0 * tc + tr - bl - 2.0 * bc - br) * 0.25;
        gl_FragColor = vec4(dot(gx, gx), dot(gy, gy), dot(gx, gy), 1.0) * 10.0;
        return;
    }

    // ── pass 2 : smooth tensor → flow
    if (PASSINDEX == 2) {
        vec2 px = 1.0 / R;
        vec3 acc = vec3(0.0); float ws = 0.0;
        for (int j = -3; j <= 3; j++) {
            for (int i = -3; i <= 3; i++) {
                vec2 o = vec2(float(i), float(j));
                float w = exp(-dot(o, o) / 8.0);
                acc += w * texture2D(tenBuf, uv + o * px * 2.4).rgb;
                ws += w;
            }
        }
        acc /= ws;
        float E = acc.x, G = acc.y, F = acc.z;
        // gentle swirling prior so flat areas still get directional strokes
        float t = TIME * 0.02;
        float pa = (fbm(uv * 3.0 + t) - 0.5) * TAU * 1.5 + 1.2;
        vec2 pd = vec2(cos(pa), sin(pa));       // prior gradient direction
        float pr = 0.003;
        E += pr * pd.x * pd.x; G += pr * pd.y * pd.y; F += pr * pd.x * pd.y;
        float disc = sqrt(max((E - G) * (E - G) + 4.0 * F * F, 0.0));
        float l1 = 0.5 * (E + G + disc);
        float l2 = 0.5 * (E + G - disc);
        vec2 tv = vec2(l1 - E, -F);           // tangent (along edges)
        if (dot(tv, tv) < 1e-12) tv = vec2(-pd.y, pd.x);
        tv = normalize(tv);
        float A = (l1 + l2 > 1e-9) ? (l1 - l2) / (l1 + l2) : 0.0;
        float edge = clamp(sqrt(l1) * 3.0, 0.0, 1.0);
        gl_FragColor = vec4(tv, A, edge);
        return;
    }

    // ── pass 3 : anisotropic Kuwahara
    if (PASSINDEX == 3) {
        gl_FragColor = vec4(kuwahara(uv), 1.0);
        return;
    }

    // ── pass 4 : brush strokes (three coats over a thin underpainting)
    if (PASSINDEX == 4) {
        float pxs = R.y / 1080.0;
        vec2 pix = fc;
        float bs = brushSize * pxs;
        // underpainting: thin, darker, scrubbed
        vec3 col = texture2D(kuwBuf, uv).rgb * 0.62;
        float h = 0.04 + 0.06 * vnoise(pix / (3.0 * max(pxs, 0.6)));
        coat(pix, 26.0 * bs, 1.5, 0.36,  0.0,  0.0, 0.0, 1.0,  pxs, col, h);   // body coat everywhere
        coat(pix, 58.0 * bs, 1.7, 0.34, 23.0, -1.0, 0.0, 1.45, pxs, col, h);   // big loaded strokes in calm fields
        coat(pix, 13.0 * bs, 1.4, 0.34, 41.0,  1.0, 0.0, 0.85, pxs, col, h);   // smaller strokes on structure
        coat(pix,  7.0 * bs, 1.3, 0.32, 67.0,  2.0, 0.0, 0.7,  pxs, col, h);   // fine strokes at focal detail
        coat(pix, 30.0 * bs, 1.3, 0.38, 89.0,  0.0, 1.0, 1.7,  pxs, col, h);   // palette-knife slabs in highlights
        h *= impasto * (1.0 + 0.15 * midP());
        gl_FragColor = vec4(max(col, 0.0), h);
        return;
    }
    // ── pass 5 : impasto lighting + finish
    vec2 px = 1.0 / R;
    float pxs = R.y / 1080.0;
    vec4 c0 = texture2D(paintBuf, uv);
    float hL = texture2D(paintBuf, uv - vec2(px.x, 0.0)).a;
    float hR = texture2D(paintBuf, uv + vec2(px.x, 0.0)).a;
    float hD = texture2D(paintBuf, uv - vec2(0.0, px.y)).a;
    float hU = texture2D(paintBuf, uv + vec2(0.0, px.y)).a;
    float arL = clamp(audioReact, 0.0, 1.0);
    float bassL = clamp(audioBass, 0.0, 1.0) * arL, midL = clamp(audioMid, 0.0, 1.0) * arL;
    float bump = 1.5 / max(pxs, 0.5) * (1.0 + 0.6 * bassL);    // bass rakes the light: texture leaps out
    vec2 gr = vec2(hL - hR, hD - hU) * bump;
    gr *= min(1.0, 0.7 / max(length(gr), 1e-5));        // stroke edges catch light, never blow out
    vec3 n = normalize(vec3(gr, 1.0));
    // groove occlusion: height vs local mean
    float hm = 0.0;
    for (int i = 0; i < 8; i++) {
        float a = float(i) * TAU / 8.0;
        hm += texture2D(paintBuf, uv + vec2(cos(a), sin(a)) * px * 4.0 * pxs).a;
    }
    hm /= 8.0;
    float occ = clamp(1.0 + (c0.a - hm) * 1.6, 0.55, 1.15);

    float az = radians(lightAz) + lightMove * 0.35 * sin(TIME * 0.07);
    float el = radians(lightEl * (1.0 - 0.3 * bassL));
    vec3 Ld = normalize(vec3(cos(az) * cos(el), sin(az) * cos(el), sin(el)));
    vec3 V = vec3(0.0, 0.0, 1.0);
    float dif = max(dot(n, Ld), 0.0);
    float flatDif = max(Ld.z, 0.05);
    float shade = mix(1.0, dif / flatDif, 0.85);
    vec3 paint = c0.rgb;
    // canvas weave shows through thin paint
    vec2 wv = fc / max(pxs, 0.5) / 3.0;
    float weave = 0.5 + 0.5 * sin(wv.x * PI) * sin(wv.y * PI);
    float thin = smoothstep(0.35, 0.0, c0.a);
    paint *= 1.0 - canvasWeave * thin * 0.25 * weave;

    float gain = lightAmt * (0.86 + 0.32 * bassL + 0.16 * midL + 0.12 * beatP());
    vec3 col = paint * (0.26 + 0.74 * gain * shade) * occ;
    // wet sheen: tight specular on the ridges
    vec3 H = normalize(Ld + V);
    float spec = pow(max(dot(n, H), 0.0), 36.0);
    float gloss = smoothstep(0.3, 0.9, c0.a) * 0.55;
    col += sheen * gain * spec * gloss * (0.55 + 0.6 * beatP() + 0.4 * highP()) * vec3(1.0, 0.97, 0.92);

    // bloom (cheap, light-only)
    if (bloom > 0.001) {
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 12; i++) {
            float a = float(i) * 2.39996;
            float r = (6.0 + float(i) * 3.0) * pxs;
            vec3 s = texture2D(paintBuf, uv + vec2(cos(a), sin(a)) * r * px).rgb;
            bl += max(s - 0.62, 0.0);
        }
        col += bloom * bl / 12.0 * 1.4;
    }

    // grade
    col = hueRot(col, hueShift);
    float l = lum(col);
    col = mix(vec3(l), col, saturation);
    col = aces(col * 1.08);
    vec2 vq = uv - 0.5;
    col *= 1.0 - vignette * dot(vq, vq) * 2.4;
    col += (hash12(fc + fract(TIME) * 91.0) - 0.5) * grain * 0.06;
    // HD finisher
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    l = dot(col, vec3(0.299, 0.587, 0.114));
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    float alpha = transparentBg ? clamp(max(col.r, max(col.g, col.b)) * 1.5, 0.0, 1.0) : 1.0;
    gl_FragColor = vec4(col, alpha);
}
