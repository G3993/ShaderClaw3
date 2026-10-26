/*{
  "DESCRIPTION": "Liquid Particles — HD 3D liquid in two forms that slowly morph into each other. Particle Silk (the main form): a deep volume of tiny glossy micro-spheres that gather into flowing sheets of liquid fabric — the sheets fold, drift and pour like ink in water, each slice of particles riding its own current, with soft silken sheen between them, iridescent rims and real depth of field so near and far particles melt into bokeh. Liquid Ribbons: endless glossy 3D strips of rainbow liquid that twist, turn edge-on and roll over to show their other face, reflecting studio light with a soap-film shimmer; every strip is a continuous curve that runs off the frame in every direction with no seams, joins or ends, layered over a softer, deeper second layer and trailed by streams of glitter dust. Bass swells the particles and widens the strips, mids push the flow, highs make the glitter and sparks twinkle, beats flash the highlights. At rest it is sparse, slow and floating; during a build the particles contract, the flow accelerates and the colours shimmer faster with a tremor near the top; on the drop a flash and shockwave ring burst outward and everything pours at full density through the peak.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "3D", "Particles", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Auto Morph", "Particle Silk", "Liquid Ribbons"], "GROUP": "Form" },
    { "NAME": "colourSel",   "LABEL": "Colours", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Form", "Sunset Paint", "Aurora Smoke", "Rainbow Liquid"], "GROUP": "Form" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 14.0, "GROUP": "Form" },
    { "NAME": "pDensity",    "LABEL": "Particle Density", "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "pSize",       "LABEL": "Particle Size",    "TYPE": "float", "MIN": 0.4, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "glitter",     "LABEL": "Glitter",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "liquidAmt",   "LABEL": "Liquid Swirl",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Liquid" },
    { "NAME": "flowSpeed",   "LABEL": "Flow Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Liquid" },
    { "NAME": "spin",        "LABEL": "Spin / Tilt",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Liquid" },
    { "NAME": "gloss",       "LABEL": "Gloss",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 3, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.04, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
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
        // Colorful: every hue pushed to full saturation — neon, primary, poppy.
        vec3 hueC = (c - mn) / max(mx - mn, 1e-4);
        // near-grey areas get a drifting neon spectrum keyed to brightness
        float hx = L * 1.7 + 0.04 * TIME;
        vec3 neon = clamp(abs(fract(hx + vec3(0.0, 0.6667, 0.3333)) * 6.0 - 3.0) - 1.0, 0.0, 1.0);
        float wc = smoothstep(0.04, 0.22, sat);
        vec3 hue = pow(mix(neon, hueC, wc), vec3(1.5));
        o = hue * clamp(pow(mx, 0.85) * 1.15, 0.0, 1.0);
        o = mix(o, vec3(1.0), smoothstep(0.8, 1.0, L) * 0.5);      // white-hot cores
        o = mix(o, vec3(smoothstep(0.0, 1.0, L)), smoothstep(0.6, 0.92, L) * (1.0 - wc) * 0.85);   // greys stay crisp white
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


float gT;          // flow clock
float gScheme;     // colour scheme 0 sunset, 1 aurora, 2 rainbow
float gPx;         // one pixel in screen units (1 / height)

// ── the liquid: layered sine domain warp (cheap, smooth, swirly) ─────────
vec3 liquid(vec3 p) {
    float amp = liquidAmt * (0.55 + 0.6 * gLift + 0.5 * gBuild);
    float t = gT;
    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        p += amp * 0.42 / (1.0 + fi * 0.8) * vec3(sin(p.y * 1.7 + t * 0.6 + fi * 1.3),
                                                 sin(p.z * 1.9 - t * 0.5 + fi * 2.1),
                                                 sin(p.x * 1.5 + t * 0.4 + fi * 0.7));
        p = p.yzx * 1.03;
    }
    return p;
}
vec3 scheme(float s, float k) {
    s = fract(s);
    if (k < 0.5) {   // Sunset Paint: black-blue → red → orange → pink → lavender → sky
        vec3 c = mix(vec3(0.05, 0.08, 0.16), vec3(0.85, 0.05, 0.08), smoothstep(0.0, 0.2, s));
        c = mix(c, vec3(1.0, 0.55, 0.1), smoothstep(0.18, 0.32, s));
        c = mix(c, vec3(1.0, 0.35, 0.45), smoothstep(0.32, 0.48, s));
        c = mix(c, vec3(0.62, 0.55, 0.95), smoothstep(0.5, 0.68, s));
        c = mix(c, vec3(0.2, 0.62, 1.0), smoothstep(0.7, 0.86, s));
        c = mix(c, vec3(0.9, 0.2, 0.3), smoothstep(0.9, 1.0, s));
        return c;
    }
    if (k < 1.5) {   // Aurora Smoke: teal → green → orange → pink → blue
        vec3 c = mix(vec3(0.02, 0.25, 0.22), vec3(0.15, 0.75, 0.4), smoothstep(0.0, 0.3, s));
        c = mix(c, vec3(1.0, 0.55, 0.15), smoothstep(0.35, 0.55, s));
        c = mix(c, vec3(1.0, 0.55, 0.85), smoothstep(0.6, 0.8, s));
        c = mix(c, vec3(0.3, 0.6, 1.0), smoothstep(0.85, 1.0, s));
        return c;
    }
    return 0.5 + 0.5 * cos(TAU * (s + vec3(0.0, 0.33, 0.67)));   // Rainbow
}
vec3 saturate3(vec3 c, float s) { return mix(vec3(lum(c)), c, s); }
// thin-film interference tint (soap-bubble iridescence) from a view cosine
vec3 thinFilm(float c) { return 0.5 + 0.5 * cos(TAU * (c * 1.7 + vec3(0.0, 0.33, 0.67))); }
// studio reflection: two soft boxes + a dim gradient sky
vec3 studioEnv(vec3 r) {
    float a = smoothstep(0.80, 0.985, dot(r, vec3(-0.40, 0.62, 0.67)));
    float b = smoothstep(0.75, 0.99, dot(r, vec3(0.74, -0.22, 0.63)));
    return vec3(0.05 + 0.08 * clamp(r.y + 0.5, 0.0, 1.0)) + vec3(1.0, 0.97, 0.94) * a * 0.85 + vec3(0.8, 0.9, 1.0) * b * 0.55;
}

// ── Form 1: Particle Silk ────────────────────────────────────────────────
// a volume of glossy micro-spheres gathering on flowing liquid sheets.
// View planes stay facing the camera (bounded orbit); every particle is drawn
// whole (2×2 nearest-cell search), sized in true pixels and depth-of-field blurred.
vec4 particleSilk(vec2 sp) {
    vec3 ro = vec3(0.0, 0.0, 3.0 - 0.4 * gBuild + 0.2 * gRest);
    vec3 rd = normalize(vec3(sp, -1.6));
    float ay = (0.5 * sin(gT * 0.06) + 0.3) * spin;
    float ax = 0.22 * sin(gT * 0.07 + 0.6) * spin;
    ro.xz = rot(ay) * ro.xz; rd.xz = rot(ay) * rd.xz;
    ro.yz = rot(ax) * ro.yz; rd.yz = rot(ax) * rd.yz;
    vec3 Ld = normalize(vec3(-0.45, 0.65, 0.62));
    vec3 acc = vec3(0.0);
    float cov = 0.0;
    float cs = 1.0 / (44.0 * pDensity);
    float spread = 1.0 - 0.3 * gBuild + 0.45 * gDrop;
    float bass = bassP() * audioReact;
    float focusT = ro.z * 0.98;
    const float NL = 22.0;
    for (int i = 0; i < 22; i++) {
        float fi = float(i);
        float z = 1.35 - (fi + 0.5) * (2.7 / NL);
        float tz = (z - ro.z) / rd.z;
        if (tz < 0.05) continue;
        vec3 P = ro + rd * tz;
        // the field, sampled once per slice at this pixel
        vec3 p3 = vec3(P.xy, z) / spread;
        vec3 q = liquid(p3 * 1.25);
        float f = sin(q.x * 1.6 + q.y * 0.9) * cos(q.z * 1.3 - q.y * 0.6);
        float sheet = exp(-f * f / 0.016) + 0.08 * exp(-pow((f - 0.45) / 0.2, 2.0));
        float env = exp(-dot(p3, p3) * (0.5 + 0.45 * gRest));
        float dens = sheet * env * (0.12 + 0.72 * gLift + 0.4 * gDrop);
        float pv = 0.5 + 0.5 * sin(q.x * 0.6 - q.z * 0.8 + 0.12 * gT);
        vec3 base = scheme(pv, gScheme);
        // silky translucent sheen of the sheets themselves
        float sh = exp(-f * f / 0.09) * env;
        acc += base * sh * (0.012 + 0.014 * gLift) * (1.0 + 0.8 * thinFilm(f * 2.0 + pv).g * gloss);
        // particles: each slice drifts with its own current (continuous offsets)
        float da = fi * 2.399;
        vec2 drift = vec2(cos(da), sin(da)) * gT * (0.04 + 0.02 * h11(fi)) + vec2(gT * 0.03, 0.0);
        vec2 g = (P.xy + drift) / cs;
        vec2 cell0 = floor(g - 0.5);
        float pxp = tz * gPx / 1.6;                       // one screen pixel on this slice
        float coc = abs(tz - focusT) * 0.007;             // depth-of-field blur radius
        float depth = 1.0 - fi / NL;
        for (int jj = 0; jj < 4; jj++) {
            vec2 id = cell0 + vec2(mod(float(jj), 2.0), floor(float(jj) / 2.0));
            vec2 hh = h22(id + fi * 31.7);
            float hp = h12(id * 1.37 + fi);
            float on = smoothstep(hp - 0.06, hp + 0.06, dens * 0.95);
            if (on <= 0.0) continue;
            float hs = h12(id + 5.0);
            vec2 c = id + 0.5 + (hh - 0.5) * 0.55 + 0.1 * vec2(sin(gT * 1.3 + hh.x * TAU), cos(gT * 1.1 + hh.y * TAU));
            vec2 dv = (g - c) * cs;
            float d = length(dv);
            float r = cs * 0.13 * pSize * (0.65 + 0.7 * hs) * (1.0 + 0.35 * bass);
            float re = min(sqrt(r * r + coc * coc + 0.5 * pxp * pxp), 0.5 * cs);
            if (d > re + pxp) continue;
            float edge = smoothstep(re + pxp, re - pxp, d);
            float energy = min(r * r / (re * re), 1.0);
            // glossy micro-sphere shading
            vec2 o = dv / max(r, 1e-5);
            float oo = min(dot(o, o), 1.0);
            vec3 n = normalize(vec3(o, sqrt(1.0 - oo) + 0.05));
            float dif = clamp(dot(n, Ld), 0.0, 1.0);
            float spec = pow(clamp(dot(reflect(vec3(0.0, 0.0, -1.0), n), Ld), 0.0, 1.0), 36.0) * gloss;
            vec3 pc = scheme(pv + (hh.x - 0.5) * 0.08, gScheme);
            vec3 sharpC = pc * (0.28 + 0.95 * dif) + vec3(1.0) * spec * 1.1 + thinFilm(n.z + hs) * pow(1.0 - n.z, 2.0) * 0.5 * gloss;
            vec3 bokehC = pc * (0.75 + 0.45 * smoothstep(0.55, 1.0, d / re));
            vec3 cc = mix(sharpC, bokehC, smoothstep(0.0, 1.2, coc / max(r, 1e-5)));
            float tw = 1.0 + highP() * audioReact * 1.6 * step(0.9, hh.y) * (0.5 + 0.5 * sin(TIME * 13.0 + hh.x * 40.0));
            float w = edge * on * (0.5 + 0.7 * depth) * tw;
            acc += cc * w * (0.3 + 0.7 * energy) * 0.95;
            cov = max(cov, edge * on * energy);
        }
    }
    return vec4(acc, clamp(cov + lum(acc), 0.0, 1.0));
}

// ── Form 2: Liquid Ribbons ───────────────────────────────────────────────
// endless twisting 3D strips. Each strip is an analytic curve that runs
// forever along x; the strips repeat forever along y and every pixel tests
// its 5 nearest strips, so a strip is never cut by a repetition cell.
vec4 liquidRibbons(vec2 sp) {
    vec2 p = rot(-0.55 + 0.1 * sin(gT * 0.07) * spin) * sp;
    p /= 1.0 + 0.15 * gDrop;
    vec3 Ld = normalize(vec3(-0.45, 0.6, 0.66));
    vec3 acc = vec3(0.0);
    float cov = 0.0;
    float bass = bassP() * audioReact;
    float spc = 0.62;
    float wid = (0.085 + 0.035 * liquidAmt) * (1.0 + 0.3 * gPeak - 0.25 * gRest + 0.2 * bass);
    float wA = 0.11 + 0.03 * gLift, wB = 0.05, wC = 0.09 * liquidAmt;
    for (int layer = 0; layer < 2; layer++) {
        float back = layer == 0 ? 1.0 : 0.0;         // draw the far layer first
        float sc = mix(1.0, 1.75, back);
        vec2 q = p * sc + vec2(gT * 0.15 * (1.0 + 0.5 * back), 0.37 * back);
        float px = gPx * sc * mix(1.0, 3.5, back);  // far layer is softer (depth of field)
        float fade = mix(1.0, 0.42 + 0.2 * gPeak - 0.15 * gRest, back);
        float id0 = floor(q.y / spc + 0.5);
        vec3 lay = vec3(0.0);
        float lcov = 0.0, zb = -1e3;
        float dust = 0.0; vec3 dcol = vec3(0.0);
        for (int j = -2; j <= 2; j++) {
            float id = id0 + float(j);
            float ph = id * 1.7;
            // centre line: y = id*spc - W(x), continuous and infinite in x
            float a1 = q.x * 2.3 + gT * 0.8 + ph * 0.5;
            float a2 = q.x * 5.1 - gT * 1.3 + ph;
            float a3 = q.x * 1.1 + gT * 0.4;
            float W  = wA * sin(a1) + wB * sin(a2) + wC * sin(a3);
            float dW = wA * 2.3 * cos(a1) + wB * 5.1 * cos(a2) + wC * 1.1 * cos(a3);
            float ly = q.y + W - id * spc;
            float cosS = inversesqrt(1.0 + dW * dW);
            float dperp = ly * cosS;
            // twist along the strip
            float tw = q.x * 1.6 + gT * 0.9 + id * 2.1 + 0.6 * sin(q.x * 0.7 + id);
            float ct = cos(tw), st = sin(tw);
            float hw = wid * abs(ct);
            float e = max(hw, 0.6 * px);
            float cover = smoothstep(e + px, e - px, abs(dperp)) * mix(0.55, 1.0, smoothstep(0.0, 2.5 * px, hw));
            // glitter dust drifts in a band beside every strip
            float dy = dperp - spc * 0.27;
            float dw = 0.09 + 0.06 * gLift + 0.08 * gDrop;     // bounded so the ±2 strip search always covers it
            float dd = exp(-dy * dy / (dw * dw)) * (0.5 + 0.5 * sin(q.x * 1.3 + id + gT * 0.3));
            dust += dd;
            dcol += scheme(0.35 + dy * 2.0 + id * 0.15, gScheme < 1.5 ? gScheme : 1.0) * dd;
            if (cover <= 0.0) continue;
            float s = clamp(dperp / max(hw, 1e-4), -1.0, 1.0) * sign(ct);   // across-strip coord
            // 3D frame: tangent T, in-plane normal N2, screen z
            vec2 T = vec2(1.0, -dW) * cosS;
            vec3 N2 = vec3(-T.y, T.x, 0.0);
            vec3 across = N2 * ct + vec3(0.0, 0.0, st);
            vec3 n = normalize(-N2 * st + vec3(0.0, 0.0, ct) - across * s * 0.55);   // slightly cupped
            float face = 1.0;
            if (n.z < 0.0) { n = -n; face = -1.0; }
            float z = s * wid * st;
            float nv = n.z;
            vec3 hue = scheme(s * face * 0.38 + 0.5 + id * 0.11 + gT * 0.02 + (face < 0.0 ? 0.5 : 0.0), 2.0);
            if (gScheme < 1.5) hue = mix(hue, scheme(s * 0.3 + 0.5 + id * 0.1, gScheme), 0.6);
            float fib = mix(1.0, 0.88 + 0.12 * sin(s * 34.0 + q.x * 2.0), smoothstep(6.0 * px, 20.0 * px, hw));
            float dif = clamp(dot(n, Ld), 0.0, 1.0);
            vec3 hv = normalize(Ld + vec3(0.0, 0.0, 1.0));
            float spec = pow(clamp(dot(n, hv), 0.0, 1.0), 120.0) * 1.3 + pow(clamp(dot(n, hv), 0.0, 1.0), 16.0) * 0.12;
            float fres = 0.04 + 0.96 * pow(1.0 - nv, 5.0);
            vec3 refl = studioEnv(reflect(vec3(0.0, 0.0, -1.0), n));
            vec3 irid = thinFilm(nv + s * 0.25 + id * 0.13);
            vec3 c = hue * fib * (0.16 + 0.95 * dif);
            c += refl * mix(vec3(1.0), irid, 0.55) * (0.15 + 0.85 * fres) * gloss;
            c += vec3(1.0) * spec * gloss * (0.8 + 0.6 * audioBeatPulse * audioReact);
            c += irid * fres * 0.35 * gloss;
            c *= 0.55 + 0.45 * smoothstep(1.0, 0.55, abs(s)) + 0.0;   // darker rolled edges
            if (z > zb) { lay = mix(lay, c, cover); zb = mix(zb, z, step(0.5, cover)); }
            else        { lay += c * cover * (1.0 - lcov); }
            lcov = max(lcov, cover);
        }
        // glitter: round, anti-aliased sparks on a grid that flows with the liquid
        dcol /= max(dust, 1e-4);
        float gs = 0.012 * pSize;
        vec2 gg = (q + vec2(-gT * 0.12, 0.0)) / gs;
        vec2 gc0 = floor(gg - 0.5);
        float spk = 0.0;
        for (int k = 0; k < 4; k++) {
            vec2 gid = gc0 + vec2(mod(float(k), 2.0), floor(float(k) / 2.0));
            float hh = h12(gid + back * 91.0);
            vec2 gj = h22(gid * 1.31 + back * 7.0);
            float thr = 1.0 - dust * 0.42 * glitter * (0.5 + 0.6 * gLift);
            float on = smoothstep(thr - 0.05, thr + 0.05, hh);
            vec2 gcen = gid + 0.5 + (gj - 0.5) * 0.6;
            float d = length(gg - gcen) * gs;
            float r = gs * (0.12 + 0.16 * gj.x);
            float re = max(r, 0.7 * px);
            float twk = 0.55 + 0.45 * sin(TIME * 9.0 + hh * 60.0) + highP() * audioReact * 0.7;
            spk += on * smoothstep(re + px, re - px, d) * twk * min(1.0, r * r / (re * re) + 0.3);
        }
        vec3 under = dcol * (spk * 1.1 + dust * 0.035);
        lay += under * (1.0 - lcov * 0.85);
        acc = mix(acc, lay * fade, lcov) + under * fade * (1.0 - lcov) * (1.0 - cov * 0.5);
        cov = max(cov, lcov * fade);
    }
    return vec4(acc, clamp(cov + lum(acc), 0.0, 1.0));
}

vec4 form(float k, vec2 sp) {
    if (k < 0.5) return particleSilk(sp);
    return liquidRibbons(sp);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    gPx = 1.0 / R.y;
    float ar = audioReact;
    gT = TIME * 0.35 * flowSpeed + (audioMidTime * 0.35 + audioBassTime * 0.2) * ar * flowSpeed + 0.8 * gBuild;
    // build tremor near the top
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.008 * smoothstep(0.65, 1.0, gBuild);

    // 0 Auto Morph (Silk ↔ Ribbons), 1 Particle Silk, 2 Liquid Ribbons
    float ph = TIME / morphPeriod;
    float fs = float(formSel);
    float ka = fs > 0.5 ? fs - 1.0 : mod(floor(ph), 2.0);
    float kb = fs > 0.5 ? ka : 1.0 - ka;
    float f = fs > 0.5 ? 0.0 : smoothstep(0.7, 1.0, fract(ph));
    float cselRaw = float(colourSel);

    gScheme = cselRaw > 0.5 ? cselRaw - 1.0 : ka + 1.0;
    vec4 A = form(ka, sp);
    if (f > 0.001) {
        gScheme = cselRaw > 0.5 ? cselRaw - 1.0 : kb + 1.0;
        vec4 B = form(kb, sp);
        A = mix(A, B, f);
    }
    // drop shockwave ring
    float ring = exp(-pow((length(sp) - (1.0 - gDrop) * 1.3) / 0.07, 2.0)) * gDrop;
    A.rgb *= 1.0 + 0.8 * ring;
    vec3 bg = backdrop(uv);
    vec3 col = bg * (1.0 - clamp(A.a, 0.0, 1.0)) + A.rgb;
    col = saturate3(col, saturation * (0.8 + 0.25 * gLift + 0.2 * gBuild));
    if (hueShift > 0.0) {
        float a = hueShift * TAU; vec3 kk = vec3(0.57735); float ca = cos(a);
        col = col * ca + cross(kk, col) * sin(a) + kk * dot(kk, col) * (1.0 - ca);
    }
    // drop flash + build shimmer
    col += vec3(1.0, 0.95, 0.9) * gDrop * gDrop * 0.35 * smoothstep(0.0, 0.3, A.a + 0.2);
    col *= 1.0 + 0.25 * gBuild * (0.5 + 0.5 * sin(TIME * (8.0 + 30.0 * gBuild)));
    col *= 0.75 + 0.35 * gLift + 0.15 * audioBeatPulse * ar;
    // soft filmic shoulder so glossy highlights roll off instead of clipping
    col = col / (1.0 + 0.18 * max(col - 0.8, 0.0));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(A.a, 0.0, 1.0) : 1.0);
}
