/*{
  "DESCRIPTION": "Liquid Particles — colourful 3D particles moving freely through liquid. Three forms morph into each other: a Bead Slab of thousands of tiny glossy 3D beads tilting in space while colour flows through them like poured paint (red, orange, pink, lavender and sky blue rivers); Particle Silk, a volumetric cloud of glowing points that gathers into flowing sheets of liquid fabric that fold and drift; and Liquid Ribbons, glossy twisting rainbow ribbons of liquid with clouds of glitter dust streaming beside them. Everything is driven by one liquid flow field, so particles swirl, pour and curl like ink in water. At rest it is sparse, slow and floating; during a build the particles contract, the flow accelerates and the colours saturate; on the drop everything bursts outward in a flash and then pours at full density through the peak.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "3D", "Particles", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Auto Morph", "Bead Slab", "Particle Silk", "Liquid Ribbons"], "GROUP": "Form" },
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
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
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
// paint value 0..1 at a point: smooth rivers of colour
float paint(vec3 p) {
    vec3 q = liquid(p);
    return 0.5 + 0.5 * sin(q.x * 1.1 + q.y * 0.7 - q.z * 0.5 + 0.3 * sin(q.z * 2.0));
}
vec3 scheme(float s, float k) {
    s = fract(s);
    if (k < 0.5) {   // Sunset Paint (ref: bead slab) black-blue → red → orange → pink → lavender → sky
        vec3 c = mix(vec3(0.05, 0.08, 0.16), vec3(0.85, 0.05, 0.08), smoothstep(0.0, 0.2, s));
        c = mix(c, vec3(1.0, 0.55, 0.1), smoothstep(0.18, 0.32, s));
        c = mix(c, vec3(1.0, 0.35, 0.45), smoothstep(0.32, 0.48, s));
        c = mix(c, vec3(0.62, 0.55, 0.95), smoothstep(0.5, 0.68, s));
        c = mix(c, vec3(0.2, 0.62, 1.0), smoothstep(0.7, 0.86, s));
        c = mix(c, vec3(0.9, 0.2, 0.3), smoothstep(0.9, 1.0, s));
        return c;
    }
    if (k < 1.5) {   // Aurora Smoke (ref: particle smoke) teal → green → orange → pink
        vec3 c = mix(vec3(0.02, 0.25, 0.22), vec3(0.15, 0.75, 0.4), smoothstep(0.0, 0.3, s));
        c = mix(c, vec3(1.0, 0.55, 0.15), smoothstep(0.35, 0.55, s));
        c = mix(c, vec3(1.0, 0.55, 0.85), smoothstep(0.6, 0.8, s));
        c = mix(c, vec3(0.3, 0.6, 1.0), smoothstep(0.85, 1.0, s));
        return c;
    }
    return 0.5 + 0.5 * cos(TAU * (s + vec3(0.0, 0.33, 0.67)));   // Rainbow
}
vec3 saturate3(vec3 c, float s) { return mix(vec3(lum(c)), c, s); }

// ── Form 1: Bead Slab ────────────────────────────────────────────────────
float sdRoundRect(vec2 p, vec2 b, float r) { vec2 q = abs(p) - b + r; return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r; }
vec4 beadSlab(vec2 sp) {
    vec3 ro = vec3(0.0, 0.0, 3.3 - 0.35 * gBuild + 0.25 * gRest);
    vec3 rd = normalize(vec3(sp, -1.75));
    // the slab tilts and turns in 3D
    float ay = (0.45 * sin(gT * 0.11) + 0.15) * spin;
    float ax = (0.25 * sin(gT * 0.08 + 1.0) - 0.12) * spin;
    vec3 ex = vec3(cos(ay), 0.0, -sin(ay));
    vec3 ey = vec3(0.0, 1.0, 0.0);
    ey.yz = rot(ax) * ey.yz; ex.yz = rot(ax) * ex.yz;
    vec3 n = normalize(cross(ex, ey));
    float tp = -dot(ro, n) / dot(rd, n);
    if (tp < 0.0) return vec4(0.0);
    vec3 P = ro + rd * tp;
    vec2 uv = vec2(dot(P, ex), dot(P, ey));
    // drop: punch outward
    uv /= 1.0 + 0.18 * gDrop;
    // slab outline: wide block that breathes into a blob, with crumbling edges
    vec2 halfSz = vec2(0.95, 0.62) * (1.0 - 0.15 * gBuild + 0.05 * gPeak);
    float N = floor(52.0 * pDensity);
    vec2 g = uv * N;
    vec2 id0 = floor(g);
    float bestZ = -1.0;
    vec3 bestN = vec3(0.0); vec2 bestC = vec2(0.0); float bestH = 0.0;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 id = id0 + vec2(float(i), float(j));
        vec2 hh = h22(id);
        float h3 = h12(id + 17.3);
        // free motion: each bead jiggles in its pocket; more energy, more jiggle
        vec2 c = id + 0.5 + (hh - 0.5) * 0.45
               + (0.12 + 0.2 * gLift) * vec2(sin(gT * 2.1 + hh.x * TAU), cos(gT * 1.7 + hh.y * TAU));
        vec2 cu = c / N;
        float sd = sdRoundRect(cu, halfSz, 0.18) + 0.05 * sin(cu.y * 7.0 + gT) * (0.5 + gLift);
        // crumble: edge beads drop out and drift off; drop blasts holes through
        float er = sd + 0.06 * (h3 - 0.5) + 0.1 * gDrop * h12(id + 3.0);
        if (er > 0.0) {
            // stragglers flying off the edge
            if (h3 > 0.9 - 0.1 * gLift && er < 0.25 + 0.4 * gDrop) {
                c += normalize(cu + 1e-3) * (er * N * 0.0);
            } else continue;
        }
        // sparse dark rivers like the reference
        float pv = paint(vec3(cu * 1.2, 0.3));
        if (pv < 0.08 && h3 < 0.4) continue;
        float rb = 0.42 * (0.85 + 0.3 * h3) * pSize * (1.0 + 0.12 * bassP() * audioReact);
        float d = length(g - c);
        if (d > rb) continue;
        float z = sqrt(rb * rb - d * d) + h3 * 0.35;
        if (z > bestZ) {
            bestZ = z;
            vec2 o = (g - c) / rb;
            bestN = vec3(o, sqrt(max(1.0 - dot(o, o), 0.0)));
            bestC = cu; bestH = h3;
        }
    }
    if (bestZ < 0.0) return vec4(0.0);
    vec3 nw = normalize(ex * bestN.x + ey * bestN.y + n * bestN.z);
    vec3 L = normalize(vec3(-0.5, 0.7, 0.8));
    float dif = clamp(dot(nw, L), 0.0, 1.0);
    float spec = pow(clamp(dot(reflect(rd, nw), L), 0.0, 1.0), 40.0) * gloss;
    float ao = 0.45 + 0.55 * bestN.z;
    float pv = paint(vec3(bestC * 1.2, 0.3));
    vec3 base = scheme(pv + (bestH - 0.5) * 0.06, gScheme);
    base *= 0.8 + 0.4 * bestH;
    // dark cores in the low rivers
    base *= 0.4 + 0.6 * smoothstep(0.05, 0.25, pv);
    vec3 c = base * (0.25 + 0.95 * dif) * ao + vec3(1.0) * spec * 0.6;
    c += base * pow(1.0 - bestN.z, 3.0) * 0.25;
    return vec4(c, 1.0);
}

// ── Form 2: Particle Silk (volumetric points on flowing liquid sheets) ───
vec4 particleSilk(vec2 sp) {
    vec3 ro = vec3(0.0, 0.0, 3.0 - 0.4 * gBuild);
    vec3 rd = normalize(vec3(sp, -1.6));
    float ay = gT * 0.08 * spin + 0.4;
    float ax = 0.25 * sin(gT * 0.07) * spin;
    ro.xz = rot(ay) * ro.xz; rd.xz = rot(ay) * rd.xz;
    ro.yz = rot(ax) * ro.yz; rd.yz = rot(ax) * rd.yz;
    vec3 acc = vec3(0.0);
    float cov = 0.0;
    float cs = 1.0 / (80.0 * pDensity);
    float spread = 1.0 - 0.3 * gBuild + 0.45 * gDrop;
    const float NL = 18.0;
    for (int i = 0; i < 18; i++) {
        float fi = float(i);
        // planes perpendicular to the dominant view axis keep the sampling even
        float z = 1.3 - fi * (2.6 / NL);
        float tz = (z - ro.z) / rd.z;
        if (tz < 0.0) continue;
        vec3 P = ro + rd * tz;
        vec2 id = floor(P.xy / cs);
        vec2 hh = h22(id + fi * 31.7);
        vec2 pp = (id + 0.5 + (hh - 0.5) * 0.7) * cs;
        vec3 p3 = vec3(pp, z) / spread;
        // the liquid sheets
        vec3 q = liquid(p3 * 1.25);
        float f = sin(q.x * 1.6 + q.y * 0.9) * cos(q.z * 1.3 - q.y * 0.6);
        float sheet = exp(-f * f / 0.03) + 0.25 * exp(-pow((f - 0.45) / 0.25, 2.0));
        float env = exp(-dot(p3, p3) * (0.55 + 0.35 * gRest));
        float dens = sheet * env * (0.45 + 0.55 * gLift + 0.4 * gDrop);
        float on = step(h12(id * 1.37 + fi), dens * 1.4);
        float d = length(P.xy - pp);
        float r = cs * 0.24 * pSize * (0.7 + 0.6 * h12(id + 5.0));
        float dotv = smoothstep(r, r * 0.3, d) * on;
        float depth = 1.0 - fi / NL;
        float pv = 0.5 + 0.5 * sin(q.x * 0.6 - q.z * 0.8 + 0.6 * gT * 0.2);
        vec3 c = scheme(pv + hh.x * 0.05, gScheme);
        float tw = 1.0 + highP() * audioReact * 1.5 * step(0.92, hh.y) * (0.5 + 0.5 * sin(TIME * 14.0 + hh.x * 40.0));
        acc += c * dotv * (0.45 + 0.75 * depth) * 1.8 * tw;
        // luminous liquid haze along the sheets
        acc += c * sheet * env * 0.03 * (0.5 + gLift);
        cov = max(cov, dotv);
    }
    return vec4(acc, clamp(cov + lum(acc), 0.0, 1.0));
}

// ── Form 3: Liquid Ribbons + glitter dust ────────────────────────────────
vec4 liquidRibbons(vec2 sp, vec2 fc) {
    vec2 p = rot(-0.55 + 0.1 * sin(gT * 0.07) * spin) * sp;
    p /= 1.0 + 0.15 * gDrop;
    vec3 acc = vec3(0.0);
    float cov = 0.0;
    for (int layer = 0; layer < 2; layer++) {
        float fl = float(layer);
        float sc = fl < 0.5 ? 1.0 : 1.7;
        vec2 q = p * sc + vec2(gT * 0.15 * (1.0 + fl * 0.5), fl * 0.37);
        float spc = 0.62;
        float warp = 0.12 * sin(q.x * 2.3 + gT * 0.8) + 0.06 * sin(q.x * 5.1 - gT * 1.3) + 0.1 * liquidAmt * sin(q.x * 1.1 + gT * 0.4);
        float y = q.y + warp;
        float id = floor(y / spc + 0.5);
        float ly = y - id * spc;
        warp += 0.04 * sin(q.x * 3.0 + id * 2.0 + gT);
        ly = q.y + warp - id * spc;
        // twisting ribbon: width shrinks where it turns edge-on
        float tw = q.x * 1.6 + gT * 0.9 + id * 2.1;
        float wid = (0.055 + 0.03 * liquidAmt) * (0.25 + 0.75 * abs(cos(tw))) * (1.0 + 0.3 * gPeak - 0.2 * gRest);
        float a = ly / wid;
        float layerFade = fl < 0.5 ? 1.0 : 0.45;
        if (abs(a) < 1.25) {
            float inside = smoothstep(1.05, 0.9, abs(a));
            float side = sign(cos(tw));
            vec3 hue = scheme((a * side) * 0.38 + 0.5 + id * 0.11 + gT * 0.02, 2.0);
            if (gScheme < 1.5) hue = mix(hue, scheme(a * side * 0.3 + 0.5 + id * 0.1, gScheme), 0.6);
            float nz = sqrt(max(1.0 - a * a, 0.0));
            float spec = pow(nz, 18.0) * gloss * (0.6 + 0.4 * sin(q.x * 4.0 + gT));
            float fibres = 0.85 + 0.15 * sin(a * 38.0 + q.x * 2.0);
            vec3 c = hue * (0.35 + 0.8 * nz) * fibres + vec3(1.0) * spec * 0.7;
            c += hue * smoothstep(1.25, 1.0, abs(a)) * 0.35;                // soft glow edge
            acc = mix(acc, c * layerFade, inside * (fl < 0.5 ? 1.0 : (1.0 - cov)));
            cov = max(cov, inside * layerFade);
        }
        // glitter dust bands beside each ribbon
        float dy = ly - spc * 0.26;
        float dw = (0.09 + 0.08 * gLift + 0.2 * gDrop);
        float dens = exp(-dy * dy / (dw * dw)) * (0.5 + 0.5 * sin(q.x * 1.3 + id + gT * 0.3));
        vec2 gc = floor((fc + vec2(-gT * 60.0 * (1.0 + fl), 0.0)) / (1.6 * pSize));
        float hh = h12(gc + fl * 91.0 + id * 13.0);
        float spark = step(1.0 - dens * 0.55 * glitter * (0.6 + 0.6 * gLift), hh);
        vec3 dc = scheme(0.35 + dy * 2.0 + hh * 0.2 + id * 0.15, gScheme < 1.5 ? gScheme : 1.0);
        float twk = 0.6 + 0.4 * sin(TIME * 9.0 + hh * 60.0) + highP() * audioReact * 0.6;
        acc += dc * spark * twk * (1.0 - cov * 0.8) * layerFade;
        // faint liquid sheen under the dust
        acc += dc * dens * 0.06 * layerFade;
    }
    return vec4(acc, clamp(cov + lum(acc), 0.0, 1.0));
}

vec4 form(float k, vec2 sp, vec2 fc) {
    if (k < 0.5) return beadSlab(sp);
    if (k < 1.5) return particleSilk(sp);
    return liquidRibbons(sp, fc);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    gT = TIME * 0.35 * flowSpeed + (audioMidTime * 0.35 + audioBassTime * 0.2) * ar * flowSpeed + 0.8 * gBuild;
    // build tremor near the top
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.008 * smoothstep(0.65, 1.0, gBuild);

    float ph = TIME / morphPeriod;
    float fs = float(formSel);
    float ka = fs > 0.5 ? fs - 1.0 : mod(floor(ph), 3.0);
    float kb = fs > 0.5 ? ka : mod(floor(ph) + 1.0, 3.0);
    float f = fs > 0.5 ? 0.0 : smoothstep(0.75, 1.0, fract(ph));
    float cselRaw = float(colourSel);

    gScheme = cselRaw > 0.5 ? cselRaw - 1.0 : ka;
    vec4 A = form(ka, sp, fc);
    if (f > 0.001) {
        gScheme = cselRaw > 0.5 ? cselRaw - 1.0 : kb;
        vec4 B = form(kb, sp, fc);
        A = mix(A, B, f);
    }
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
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(A.a, 0.0, 1.0) : 1.0);
}
