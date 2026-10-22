/*{
  "DESCRIPTION": "Feathers — deep 3D plumage of iridescent feathers. Layer upon layer of feathers, each with a glowing white shaft, fine chevron barbs, ragged split edges and fluffy down at the base, in pearl grey, warm brown, slate blue and white, while holographic thin-film light throws rainbow patches across them like light through a prism. Three layouts morph into each other: a Fan Burst that blooms endlessly outward from the centre, a Spiral Wing that coils, and a Plumage Field of feathers flowing upward like the breast of a bird. The feathers ruffle in waves of wind and the rainbow light drifts over them. At rest the plumage breathes slowly with soft pearly light; during a build it spins and streams faster, ruffles harder and the shimmer races; on the drop every feather flares open in a burst of rainbow light, then the peak glows with full prismatic colour.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "3D", "Nature", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "layoutSel",   "LABEL": "Layout", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Auto Morph", "Fan Burst", "Spiral Wing", "Plumage Field"], "GROUP": "Feathers" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 14.0, "GROUP": "Feathers" },
    { "NAME": "featherCount","LABEL": "Feather Count",    "TYPE": "float", "AUDIO": false, "MIN": 8.0, "MAX": 34.0, "DEFAULT": 22.0, "GROUP": "Feathers" },
    { "NAME": "featherLen",  "LABEL": "Feather Length",   "TYPE": "float", "MIN": 1.2, "MAX": 3.4, "DEFAULT": 2.7, "GROUP": "Feathers" },
    { "NAME": "featherWidth","LABEL": "Feather Width",    "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.05, "GROUP": "Feathers" },
    { "NAME": "fluff",       "LABEL": "Fluff & Splits",   "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.8, "GROUP": "Feathers" },
    { "NAME": "iridescence", "LABEL": "Iridescence",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "prismPatches","LABEL": "Rainbow Patches",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Light" },
    { "NAME": "sheen",       "LABEL": "Sheen",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "baseTone",    "LABEL": "Base Tones", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Natural (pearl, brown, slate)", "Pearl White", "Midnight Blue"], "GROUP": "Light" },
    { "NAME": "flow",        "LABEL": "Flow Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "ruffle",      "LABEL": "Ruffle",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "depthFade",   "LABEL": "Depth Fade",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.03, 0.025, 0.025, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vn2(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}
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

float gT;

// thin-film interference colour (soap / pigeon-neck iridescence)
vec3 thinFilm(float th) {
    float t = fract(th);
    vec3 c = mix(vec3(1.0, 0.78, 0.35), vec3(1.0, 0.45, 0.25), smoothstep(0.0, 0.2, t));   // gold → orange
    c = mix(c, vec3(0.95, 0.4, 0.75), smoothstep(0.2, 0.4, t));                           // pink
    c = mix(c, vec3(0.45, 0.45, 1.0), smoothstep(0.4, 0.58, t));                          // violet-blue
    c = mix(c, vec3(0.3, 0.8, 1.0), smoothstep(0.58, 0.74, t));                           // cyan
    c = mix(c, vec3(0.55, 0.95, 0.6), smoothstep(0.74, 0.84, t) * 0.7);                   // a touch of green
    c = mix(c, vec3(1.0, 0.78, 0.35), smoothstep(0.86, 1.0, t));
    return mix(vec3(0.9), c * 1.15, 0.8);
}
vec3 baseColour(float h, float tone) {
    if (tone > 1.5) return mix(vec3(0.06, 0.09, 0.2), vec3(0.2, 0.3, 0.55), h);       // midnight
    if (tone > 0.5) return mix(vec3(0.72, 0.7, 0.7), vec3(0.95, 0.94, 0.92), h);     // pearl
    if (h < 0.35) return mix(vec3(0.66, 0.64, 0.63), vec3(0.85, 0.83, 0.8), h / 0.35); // pearl grey
    if (h < 0.6)  return mix(vec3(0.42, 0.31, 0.22), vec3(0.6, 0.47, 0.34), (h - 0.35) / 0.25); // warm brown
    if (h < 0.85) return mix(vec3(0.2, 0.27, 0.4), vec3(0.32, 0.42, 0.58), (h - 0.6) / 0.25);   // slate blue
    return vec3(0.92, 0.9, 0.88);                                                      // white
}

// one feather in its cell. pc = point in plumage coords, id = cell, z-order out.
// returns rgba (premultiplied-ish colour, coverage) and depth priority in .w via zOut
vec4 feather(vec2 pc, vec2 id, float layer, float cellPx, vec2 world, out float zOut) {
    zOut = -1.0;
    float h = h12(id + layer * 17.0);
    float h2 = h12(id * 1.7 + layer * 5.0 + 3.0);
    float L = featherLen * (0.8 + 0.35 * h2) * (1.0 + 0.12 * gPeak - 0.08 * gRest);
    float W = featherWidth * (0.85 + 0.3 * h) * (1.0 + 0.35 * gDrop + 0.1 * gPeak);
    vec2 base = vec2(id.x + 0.5 + (h - 0.5) * 0.3, id.y + (h2 - 0.5) * 0.4);
    vec2 d = pc - base;
    float y = d.y / L;                                   // 0 base … 1 tip
    if (y < -0.05 || y > 1.05) return vec4(0.0);
    // ruffle: a wave of wind runs along the plumage, each feather bends and twists
    float wind = sin(base.y * 0.6 - gT * 1.6 + h * 6.0) * (0.5 + 0.5 * sin(base.x * 0.7 + gT * 0.5));
    float bend = (0.18 * (h - 0.5) + 0.14 * ruffle * wind * (0.6 + 0.6 * gLift + 0.6 * gBuild)) * y * y;
    float lean = (h2 - 0.5) * 0.2;
    float x = (d.x - bend * L - lean * d.y) / W;
    // vane outline: rounded, slightly asymmetric, with a curled tip
    float yc = clamp(y, 0.0, 1.0);
    float prof = pow(sin(PI * pow(yc, 0.72)), 0.55) * (1.0 - 0.15 * yc) * smoothstep(1.0, 0.86, yc + 0.02 * (h - 0.5));
    float asym = x > 0.0 ? 1.0 : 0.82 + 0.1 * h;
    float w = 0.5 * prof * asym;
    // ragged edge + splits between barbs
    float barbCoord = (y * 26.0 - abs(x) * 16.0);
    float rag = 1.0 - fluff * 0.08 * vn2(vec2(barbCoord * 0.35, id.x * 3.0 + h * 10.0 + sign(x)));
    w *= rag;
    float ax = abs(x);
    float aa = 1.6 / max(cellPx * W, 1.0);              // edge softness ≈ 1.5 px
    float cover = smoothstep(w + aa, w - aa, ax) * smoothstep(-0.05, 0.03, y);
    // splits: thin gaps along the barbs
    float split = smoothstep(0.02, 0.0, abs(fract(barbCoord * 0.07 + vn2(vec2(id.x, y * 3.0)) * 0.6) - 0.5) - 0.47) * fluff * smoothstep(0.15, 0.6, ax / max(w, 1e-3));
    cover *= 1.0 - 0.45 * split * smoothstep(0.3, 0.7, y);
    // downy fluff around the base: soft wispy halo
    float fl = fluff * smoothstep(0.22, 0.0, y) * smoothstep(w + 0.12, w, ax) * smoothstep(0.35, 0.75, vn2(vec2(ax * 26.0 + id.x, y * 9.0 - gT)));
    float alpha = max(cover, fl * 0.35);
    if (alpha < 0.003) return vec4(0.0);

    // 3D vane: curved cross-section and a twist along the length
    float twist = (h - 0.5) * 0.9 + 0.35 * ruffle * wind;
    vec3 n = normalize(vec3(x * 1.4 + sin(twist) * 0.6, -0.25 + 0.4 * y, 1.0));
    n.xz = rot(twist * 0.5) * n.xz;
    vec3 V = vec3(0.0, 0.0, 1.0);
    float lightA = gT * 0.15 + 0.8;
    vec3 Ld = normalize(vec3(cos(lightA) * 0.7, 0.6 + 0.2 * sin(gT * 0.2), 0.7));
    float dif = 0.35 + 0.65 * clamp(dot(n, Ld), 0.0, 1.0);
    vec3 H = normalize(Ld + V);
    float spec = pow(clamp(dot(n, H), 0.0, 1.0), 26.0);
    // fine barbs: chevron lines, fade when too dense to resolve
    float barbF = 95.0;
    float linesPx = cellPx * L / barbF;
    float barbs = 0.5 + 0.5 * sin((y * barbF - ax * barbF * 0.62 / max(L / W, 0.3)) * TAU);
    barbs = mix(0.5, barbs, smoothstep(3.0, 6.0, linesPx));
    // hair-like streaks along the barbs (resolution-independent)
    // hairline barbs as stretched noise: fine, organic, no moiré
    float bc = y * 70.0 - ax * 44.0;
    float streak = vn2(vec2(bc + id.x * 7.0, ax * 6.0 + h * 5.0));
    float streak2 = vn2(vec2(bc * 2.3 + 3.0, ax * 14.0 + id.y));
    float hair = mix(streak, streak2, 0.45);
    float hairAmt = smoothstep(2.0, 5.0, cellPx * L / 70.0);
    barbs = mix(barbs, mix(0.55, hair, hairAmt), 0.85);
    float tone = float(baseTone);
    vec3 base0 = baseColour(fract(h * 7.31 + h2 * 0.5), tone);
    // darker toward the base and along the outer vane, lighter near the shaft
    base0 *= 0.7 + 0.35 * smoothstep(0.0, 0.6, y) - 0.2 * smoothstep(0.4, 1.0, ax / max(w, 1e-3));
    float edgeAO = smoothstep(0.0, 0.45, (w - ax) / max(w, 1e-3));
    vec3 c = base0 * dif * (0.6 + 0.6 * barbs) * (0.35 + 0.65 * edgeAO);
    // holographic thin-film: thickness drifts over the plumage in patches
    float film = vn2(world * 2.2 + vec2(gT * 0.12, -gT * 0.08)) * 1.4 + dot(n, Ld) * 0.6 + y * 0.35 + h * 0.3;
    float patch = smoothstep(0.62 - 0.45 * prismPatches - 0.25 * gLift, 0.95, vn2(world * 1.3 + vec2(-gT * 0.1, gT * 0.07) + h))
                + 0.6 * gDrop + 0.25 * gBuild * (0.5 + 0.5 * sin(TIME * (6.0 + 20.0 * gBuild) + world.x * 4.0));
    vec3 irid = thinFilm(film + 0.25 * sin(TIME * 0.3));
    float iA = 0.8 * iridescence * clamp(patch, 0.0, 1.0) * (0.3 + 0.7 * smoothstep(0.1, 0.9, spec * 3.0 + 0.4 * dif));
    c = mix(c, irid * (0.5 + 0.8 * dif) * (0.65 + 0.55 * barbs), clamp(iA, 0.0, 1.0));
    c *= 0.45 + 0.55 * edgeAO;
    c += irid * spec * iridescence * clamp(patch, 0.0, 1.0) * 0.6;
    c += vec3(1.0, 0.97, 0.92) * spec * sheen * (0.25 + 0.35 * highP() * audioReact);
    // shaft (rachis): a fine bright line, thinner toward the tip
    float sw = (0.028 - 0.018 * y);
    float shaft = smoothstep(sw + aa, sw * 0.3, ax) * smoothstep(0.02, 0.1, y) * smoothstep(0.97, 0.9, y) * cover;
    c = mix(c, vec3(0.9, 0.86, 0.78) * (0.6 + 0.45 * dif) + spec * 0.4, shaft * 0.7);
    // fluff colour
    c = mix(c, base0 * 0.9 + 0.05, (1.0 - cover) * step(cover, 0.5));
    // layering priority: inner feathers / upper layers lie on top
    zOut = 10.0 - layer * 3.0 - id.y * 0.01 + h * 0.5 + cover;
    return vec4(c, alpha);
}

// plumage coordinates for a layout
vec2 layoutCoords(float k, vec2 sp, float N, out float cellPx, out float valid) {
    valid = 1.0;
    float r = length(sp);
    float a = atan(sp.y, sp.x);
    float pxPerUnit = RENDERSIZE.y;
    if (k < 1.5) {
        // Fan Burst (k=0) / Spiral Wing (k=1): log-polar → conformal, feathers grow outward
        vec2 c = sp - vec2(0.0, -0.15);
        r = max(length(c), 1e-4);
        a = atan(c.x, c.y);                               // 0 = straight up
        float zoomFlow = gT * 0.55;
        float pu = log(r) * N / TAU - zoomFlow;
        float pv = a * N / TAU + gT * 0.06 + (k > 0.5 ? (log(r) * N / TAU) * 0.55 : 0.0);
        cellPx = r * TAU / N * pxPerUnit;
        if (r < 0.015) valid = 0.0;
        return vec2(pv, pu);
    }
    // Plumage Field: rows of feathers flowing up, warped like a bird's breast
    vec2 p = sp * (N / 2.4);
    p.x += 0.6 * sin(p.y * 0.35 + gT * 0.3) + 0.35 * sin(p.y * 0.8 - gT * 0.2);
    p.y -= gT * 0.9;
    p.x *= 1.15;
    cellPx = pxPerUnit * 2.4 / N;
    return p;
}

vec4 plumage(float k, vec2 sp) {
    float N = floor(featherCount);
    float cellPx, valid;
    vec3 acc = vec3(0.0);
    float accA = 0.0;
    float bestZ = -1e9;
    vec4 best = vec4(0.0);
    // three staggered layers, deepest first; keep the top-most opaque feather
    vec3 col = vec3(0.0); float cov = 0.0;
    for (int l = 1; l >= 0; l--) {
        float fl = float(l);
        vec2 pc = layoutCoords(k, sp, N, cellPx, valid);
        if (valid < 0.5) continue;
        pc += vec2(fl * 0.5, fl * 0.37);                // stagger
        vec2 id0 = floor(pc);
        // this layer's top feather at this pixel
        float lz = -1e9; vec4 lc = vec4(0.0);
        for (int j = -1; j <= 4; j++)               // -1: bases jittered above the cell edge
        for (int i = -2; i <= 2; i++) {
            vec2 id = id0 - vec2(float(-i), float(j));
            vec2 idw = id;
            if (k < 1.5) idw.x = mod(idw.x, N);       // wrap around the circle
            float z;
            vec4 f = feather(pc - (id - idw), idw, fl, cellPx, sp, z);
            if (f.a > 0.003 && z > lz) { lz = z; lc = f; }
        }
        float shade = 1.0 - depthFade * fl * 0.6;      // deeper layers sit in shadow
        lc.rgb *= shade;
        col = mix(col, lc.rgb, lc.a);
        cov = max(cov, lc.a);
    }
    return vec4(col, cov);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    gT = TIME * 0.4 * flow + (audioMidTime * 0.3 + audioBassTime * 0.25) * ar * flow + 1.2 * gBuild;
    sp /= 1.0 + 0.12 * gDrop - 0.06 * gBuild;
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.006 * smoothstep(0.65, 1.0, gBuild);

    float ph = TIME / morphPeriod;
    float ls = float(layoutSel);
    float ka = ls > 0.5 ? ls - 1.0 : mod(floor(ph), 3.0);
    float kb = ls > 0.5 ? ka : mod(floor(ph) + 1.0, 3.0);
    float f = ls > 0.5 ? 0.0 : smoothstep(0.75, 1.0, fract(ph));
    vec4 A = plumage(ka, sp);
    if (f > 0.001) A = mix(A, plumage(kb, sp), f);

    vec3 bg = backdrop(uv);
    vec3 col = mix(bg, A.rgb, A.a);
    // soft vignette of light toward the centre
    // rest: dim pearly hush with a heavy vignette; peak: a prismatic light blooms from the centre
    float rr = dot(sp, sp);
    col *= mix(0.45, 1.0, exp(-rr * (2.2 - 1.4 * gLift))) * (0.55 + 0.6 * gLift);
    vec3 prism = 0.5 + 0.5 * cos(TAU * (atan(sp.y, sp.x) / TAU + gT * 0.05 + vec3(0.0, 0.33, 0.67)));
    col += prism * exp(-rr * 3.0) * A.a * (0.35 * gPeak + 0.5 * gBuild * gBuild) * iridescence;
    col *= 1.0 + 0.12 * audioBeatPulse * ar;
    col += vec3(1.0, 0.95, 0.9) * gDrop * gDrop * 0.25 * A.a;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? A.a : 1.0);
}
