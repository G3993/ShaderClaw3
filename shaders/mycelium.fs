/*{
  "DESCRIPTION": "Mycelium — a fungal network growing through dark soil. From a few glowing spores, fine hyphae thread outward, forking again and again into a fractal tree of ever-finer side branches that meander through the grain of the earth; neighbouring threads fuse with little cross-bridges (anastomosis) and whole colonies grow into one another until the soil is laced with a living web. Bright growing tips lead the front, nutrient pulses ride along the threads, and tiny mushroom pins pop up at the junctions while spores drift through the air. It morphs between four forms: Soil Web — crisp white threads on dark brown soil full of grit; Bioluminescent — foxfire-green hyphae and glowing Mycena caps on pure black; Root Network — the wood-wide web, thick bark-brown tree roots reaching down from above while golden fungal links weave between them and trade glowing nutrients in both directions; Spore Print — big radial gill-pattern prints in cream, rust and lilac forming around each spore point with faint mycelium creeping out. Each cycle the old network dims away as new spores germinate. Bass makes it grow faster, beats send bright nutrient pulses down every thread, mids drive the flow, highs make the drifting spores sparkle. At rest only a few sparse colonies grow slowly with soft pulses; during a build the network contracts, the pulses race and the tips shimmer and tremble; on the drop the whole web lights up in a wave from the centre outward, then the peak grows dense with many colonies, fruiting bodies and bright pulses.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Organic", "Nature", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Soil Web", "Bioluminescent", "Root Network", "Spore Print"], "GROUP": "Form" },
    { "NAME": "morphPeriod", "LABEL": "Growth Cycle (s)", "TYPE": "float", "AUDIO": false, "MIN": 8.0, "MAX": 90.0, "DEFAULT": 26.0, "GROUP": "Form" },
    { "NAME": "colonies",    "LABEL": "Colonies",         "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Network" },
    { "NAME": "branching",   "LABEL": "Branching",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Network" },
    { "NAME": "depth",       "LABEL": "Fractal Depth",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Network" },
    { "NAME": "fusion",      "LABEL": "Anastomosis",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Network" },
    { "NAME": "meander",     "LABEL": "Meander",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Network" },
    { "NAME": "fruiting",    "LABEL": "Fruiting Bodies",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Network" },
    { "NAME": "pulseAmt",    "LABEL": "Nutrient Pulses",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "flowSpeed",   "LABEL": "Flow Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "sporeDrift",  "LABEL": "Drifting Spores",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Motion" },
    { "NAME": "lineWidth",   "LABEL": "Thread Width",     "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "glow",        "LABEL": "Glow",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "soilTex",     "LABEL": "Soil Texture",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.02, 0.015, 0.01, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vn2(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}
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

// ── network state ───────────────────────────────────────────────────────
float gPx, gT, gFlow, gPd, gMaxLf, gFuse, gFruit, gNfrac;
float gS0, gMode, gSd, gRf;
vec2 gOrg;
// per-render accumulators
float aLine, aGlow, aTip, aPulse, aRoot, aRootS, aRootP, aCap, aCapS, aCapR, aHalo, aPrint, aCore;
vec3 aPrintCol;

// node of quadtree cell `cell` at level L (level 0 = hubs / spore points)
vec2 nodePos(float L, vec2 cell) {
    float sL = gS0 * exp2(-L);
    vec2 j = h22(cell + vec2(L * 37.1 + gSd, L * 11.3 - gSd * 0.7));
    vec2 pos = (cell + 0.22 + 0.56 * j) * sL;
    if (gMode > 0.5 && L < 0.5) pos = vec2((cell.x + 0.3 + 0.4 * j.x) * sL, gS0 * 1.08);
    return pos + gOrg;
}
// which of a parent's four children never dies (keeps a leading hypha going)
float leaderOf(float L, vec2 pc) { return floor(h12(pc * 1.13 + vec2(L * 3.7, gSd + 1.9)) * 4.0); }
// survival of node (L, cell) and all its ancestors
float aliveN(float L, vec2 cell) {
    float ok = 1.0;
    for (int q = 0; q < 6; q++) {
        if (L < 0.5) break;
        vec2 pc = floor(cell * 0.5);
        vec2 k2 = cell - pc * 2.0;
        float k = k2.x + 2.0 * k2.y;
        if (abs(k - leaderOf(L, pc)) > 0.5) {
            float pdl = gPd * (0.8 + 0.06 * L);
            ok *= smoothstep(pdl - 0.05, pdl + 0.05, h12(cell * 0.773 + vec2(L * 13.1, gSd + 2.7)));
            if (ok < 0.01) break;
        }
        cell = pc;
        L -= 1.0;
    }
    return ok;
}
float hubW(vec2 hc) {
    if (gMode > 0.5) return 1.0 - step(0.5, abs(hc.y));
    return 1.0;
}
float hubDelay(vec2 hc) { return gMode > 0.5 ? -0.25 : (0.06 + 1.1 * (1.0 - gNfrac)) * h12(hc * 2.3 + gSd + 1.0); }
// how far a colony can spread before it fuses with its neighbours
float hubReach(vec2 hc) { return gMode > 0.5 ? 9.0 : (0.2 + 0.12 * gNfrac) * (0.8 + 0.5 * h12(hc * 5.1 + gSd + 3.0)); }
// growth "arrival time" of a point (path distance from its spore)
float gReach;
float arrive(vec2 x, vec2 hub, float dl) {
    float r = length(x - hub) * (1.0 + 0.4 * vn2(x * 7.0 + gSd));
    return r + dl + max(r - gReach, 0.0) * 6.0;
}

// distance to a gently bowed, slightly wiggly hypha a→b; u = position along it
float hypha(vec2 p, vec2 a, vec2 b, float bow, float ph, out float u) {
    vec2 ba = b - a;
    float l2 = max(dot(ba, ba), 1e-8);
    float len = sqrt(l2);
    u = clamp(dot(p - a, ba) / l2, 0.0, 1.0);
    vec2 nrm = vec2(-ba.y, ba.x) / len;
    float off = len * bow * 4.0 * u * (1.0 - u) + min(0.0025, len * 0.06) * sin(u * len * 110.0 + ph) * sin(PI * u);
    return length(p - a - ba * u - nrm * off);
}

// a mushroom pin / glowing cap at a node
void capAt(vec2 v, vec2 q, float hh, float apr, float wgt) {
    float cr = (0.006 + 0.009 * h11(hh * 91.0)) * apr * (1.0 + 0.35 * sin(apr * PI)) * (1.0 + 0.3 * gDrop + 0.12 * bassP() * audioReact);
    float dd = length(v - q);
    float cv = clamp((cr - dd) / gPx + 0.5, 0.0, 1.0) * wgt;
    vec2 nq = (v - q) / max(cr, 1e-4);
    float nz = sqrt(max(1.0 - dot(nq, nq), 0.0));
    float sh = clamp(dot(normalize(vec3(nq, nz + 0.15)), normalize(vec3(-0.5, 0.6, 0.62))), 0.0, 1.0);
    if (cv > aCap) { aCap = cv; aCapS = sh; aCapR = length(nq); }
    aHalo = max(aHalo, wgt * apr * exp(-max(dd - cr, 0.0) / (cr * 1.2 + 0.003)));
}

// radial gill-pattern spore print
void printAt(vec2 v, float Rp, float sd, float wgt) {
    float rr0 = length(v);
    if (rr0 > Rp * 1.12 || Rp < 0.002) return;
    float tt0 = atan(v.y, v.x) / TAU;
    float G = 26.0 + floor(h11(sd * 3.3) * 22.0);
    float x = rr0 / Rp;
    float wob = 0.06 * sin(x * 7.0 + sd) + 0.02 * (vn2(v * 30.0 + sd) - 0.5);
    float g1 = abs(fract(tt0 * G + wob) - 0.5) * 2.0;
    float g2 = abs(fract(tt0 * G * 2.0 + 0.5 + wob * 2.0) - 0.5) * 2.0;
    float g3 = abs(fract(tt0 * G * 4.0 + 0.25 + wob * 4.0) - 0.5) * 2.0;
    float mainG = 1.0 - smoothstep(0.15, 0.6, g1);
    float lam = (1.0 - smoothstep(0.15, 0.6, g2)) * smoothstep(0.42, 0.52, x) * 0.8;
    float lam2 = (1.0 - smoothstep(0.15, 0.6, g3)) * smoothstep(0.72, 0.8, x) * 0.6;
    float dens = max(mainG, max(lam, lam2));
    float spacingPx = TAU * rr0 / (G * 4.0) / gPx;
    dens = mix(0.45, dens, smoothstep(1.2, 3.5, spacingPx * (x > 0.76 ? 1.0 : x > 0.47 ? 2.0 : 4.0)));
    float edge = 1.0 - smoothstep(0.86, 1.04 + 0.1 * (vn2(v * 25.0 + sd) - 0.5), x);
    dens *= smoothstep(0.1, 0.17, x) * edge;
    dens *= 0.7 + 0.3 * smoothstep(0.2, 0.7, x);
    dens *= 0.75 + 0.25 * h12(floor(gl_FragCoord.xy) + sd);
    dens = max(dens, 0.55 * smoothstep(0.03, 0.0, abs(x - 0.13)) * edge);   // stipe ring
    float hc = h11(sd * 5.1);
    vec3 pc = hc < 0.45 ? vec3(0.95, 0.9, 0.8) : hc < 0.75 ? vec3(0.85, 0.42, 0.18) : vec3(0.62, 0.45, 0.75);
    dens *= wgt;
    if (dens > aPrint) { aPrint = dens; aPrintCol = pc; }
}

// accumulate one hypha segment a→c (arrival times ta→tc) at level L
void segment(vec2 pw, vec2 a, vec2 c, float ta, float tc, float L, float hh, float wgt) {
    bool root = gMode > 0.5;
    float u;
    float bow = (hh - 0.5) * (root ? 0.25 : 0.4);
    float d = hypha(pw, a, c, bow, hh * 40.0, u);
    float f = clamp((gRf - ta) / max(tc - ta, 1e-3), 0.0, 1.0);
    if (f <= 0.0) return;
    float len = length(c - a);
    float shown = smoothstep(f + gPx / max(len, 1e-3), f - gPx / max(len, 1e-3), u);
    float age = gRf - mix(ta, tc, u);
    float wd = root ? gPx * lineWidth * max(13.0 - 3.2 * (L + u), 1.8) * (0.7 + 0.3 * smoothstep(0.0, 0.25, age))
                    : gPx * lineWidth * max(1.7 - 0.22 * (L - 1.0 + u), 0.55);
    float cov = clamp((wd - d) / gPx + 0.5, 0.0, 1.0) * shown * wgt;
    float gl = exp(-d / min(wd * 1.5 + 0.004, 0.06 * gS0 * exp2(-L))) * shown * wgt;
    float T = mix(ta, tc, u);
    float pulse = pow(0.5 + 0.5 * sin(TAU * (T * (root ? 2.5 : 5.0) - gFlow * (root ? -0.7 : 1.0) + hh * 3.0)), 24.0) * step(0.35, fract(hh * 7.31));
    if (root) {
        float rs = sqrt(max(1.0 - (d * d) / (wd * wd), 0.0));
        if (cov > aRoot) { aRoot = cov; aRootS = rs; }
        aRootP = max(aRootP, cov * rs * pulse);
    } else {
        aLine = max(aLine, cov * (1.05 - 0.1 * L));
        aGlow = max(aGlow, gl * (1.1 - 0.1 * L));
        aPulse = max(aPulse, max(cov, gl * 0.8) * pulse);
        // growing tip
        if (f < 0.999) {
            vec2 ab = c - a;
            vec2 e = a + ab * f + vec2(-ab.y, ab.x) / max(len, 1e-4) * len * bow * 4.0 * f * (1.0 - f);
            float dt = length(pw - e);
            aTip = max(aTip, wgt * (smoothstep(wd * 1.8 + gPx, wd * 0.5, dt) + 0.5 * exp(-dt / (wd * 3.0 + 0.003))));
        }
    }
}

// the whole network (mode 0 = fungal mycelium, 1 = tree roots)
void network(vec2 p, float form) {
    bool root = gMode > 0.5;
    // meander: warp the soil so straight links wander
    float mw = meander * (root ? 1.2 : 1.0);
    vec2 w1 = vec2(vn2(p * 3.5 + gSd * 1.3), vn2(p * 3.5 + gSd * 1.3 + 17.0)) - 0.5;
    vec2 w2 = vec2(vn2(p * 13.0 - gSd), vn2(p * 13.0 + gSd + 5.0)) - 0.5;
    vec2 pw = p + mw * (0.06 * w1 + 0.018 * w2);

    // hubs: spore points, cords fusing neighbouring colonies, spore prints
    vec2 hb = floor((pw - gOrg) / gS0);
    for (int hj = -1; hj <= 1; hj++)
    for (int hi = -1; hi <= 1; hi++) {
        vec2 hc = hb + vec2(float(hi), float(hj));
        float hw = hubW(hc);
        if (hw < 0.01) continue;
        vec2 H = nodePos(0.0, hc);
        float dl = hubDelay(hc);
        gReach = 9.0;
        if (!root) {
            float rH = length(pw - H);
            aCore = max(aCore, hw * smoothstep(0.012, 0.004, rH) * smoothstep(dl, dl + 0.03, gRf));
            if (form > 2.5) printAt(pw - H, (0.1 + 0.09 * h11(hc.x * 3.1 + hc.y * 7.7 + gSd)) * smoothstep(dl, dl + 0.45, gRf), hc.x * 1.3 + hc.y * 2.9 + gSd, hw);
            // anastomosis between colonies: thick cords that meet in the middle
            for (int n = 0; n < 2; n++) {
                vec2 nc = hc + (n == 0 ? vec2(1.0, 0.0) : vec2(0.0, 1.0));
                float hh = h12(hc * 3.7 + nc * 1.3 + gSd);
                if (hh > gFuse * 1.6) continue;
                float hw2 = hubW(nc);
                if (hw2 < 0.01) continue;
                vec2 H2 = nodePos(0.0, nc);
                vec2 M = mix(H, H2, 0.5) + (h22(hc + nc + gSd) - 0.5) * gS0 * 0.3;
                float dl2 = hubDelay(nc);
                float tm1 = arrive(M, H, dl), tm2 = arrive(M, H2, dl2);
                segment(pw, H, M, dl, tm1 + 0.05, 1.0, hh, min(hw, hw2));
                segment(pw, H2, M, dl2, tm2 + 0.05, 1.0, fract(hh * 7.0), min(hw, hw2));
            }
        }
    }

    // branching levels: every node sprouts from a point just behind its parent node
    float maxL = root ? min(gMaxLf, 4.0) : gMaxLf;
    for (int li = 1; li <= 6; li++) {
        float L = float(li);
        float lw = clamp(maxL - L + 1.0, 0.0, 1.0);
        if (lw <= 0.0) break;
        float sP = gS0 * exp2(1.0 - L);
        vec2 base = floor((pw - gOrg) / sP - 0.5);
        for (int pj = 0; pj < 2; pj++)
        for (int pi = 0; pi < 2; pi++) {
            vec2 pc = base + vec2(float(pi), float(pj));
            vec2 hc = floor(pc * exp2(1.0 - L));
            float hw = hubW(hc);
            if (hw < 0.01) continue;
            float ap = aliveN(L - 1.0, pc);
            if (ap < 0.01) continue;
            vec2 H = nodePos(0.0, hc);
            float dl = hubDelay(hc);
            gReach = hubReach(hc);
            vec2 P = nodePos(L - 1.0, pc);
            vec2 A = P;
            if (L > 1.5) A = mix(P, nodePos(L - 2.0, floor(pc * 0.5)), 0.05 + 0.15 * h12(pc + L * 9.1 + gSd));
            float tA = arrive(A, H, dl);
            if (tA > gRf) continue;
            float lead = leaderOf(L, pc);
            for (int k = 0; k < 4; k++) {
                float fk = float(k);
                vec2 cc = pc * 2.0 + vec2(mod(fk, 2.0), floor(fk * 0.5));
                float al = 1.0;
                if (abs(fk - lead) > 0.5) {
                    float pdl = gPd * (0.8 + 0.06 * L);
                    al = smoothstep(pdl - 0.05, pdl + 0.05, h12(cc * 0.773 + vec2(L * 13.1, gSd + 2.7)));
                    if (al < 0.01) continue;
                }
                vec2 C = nodePos(L, cc);
                float tC = max(arrive(C, H, dl), tA + 0.5 * length(C - A) + 0.005);
                float hh = h12(cc * 1.91 + L * 4.4 + gSd);
                float wgt = hw * ap * al * lw;
                segment(pw, A, C, tA, tC, L, hh, wgt);
                if (root) continue;
                // fruiting bodies at the nodes
                if (L > 1.5 && L < 4.5 && fract(hh * 13.7) < gFruit) {
                    capAt(pw, C, hh, smoothstep(tC + 0.03, tC + 0.15, gRf), wgt);
                }
                // anastomosis: a later bridge fusing this hypha to its neighbour
                if (mod(fk, 2.0) < 0.5 && fract(hh * 5.3) < gFuse) {
                    vec2 cc2 = cc + vec2(1.0, 0.0);
                    vec2 C2 = nodePos(L, cc2);
                    float tB = max(tC, arrive(C2, H, dl)) + 0.04;
                if (tB > gRf) continue;
                    segment(pw, C, C2, tB, tB + 0.6 * length(C2 - C), L + 0.5, fract(hh * 3.1), wgt * 0.85);
                }
            }
        }
    }
}

// soil / ground per form
vec3 ground(vec2 p, float form) {
    float n = vn2(p * 5.0) * 0.5 + vn2(p * 13.0) * 0.3 + vn2(p * 37.0) * 0.2;
    vec2 gc = floor(p * 230.0);
    float gh = h12(gc);
    float speck = step(0.93, gh) * (0.4 + 0.6 * h12(gc + 3.0));
    float pit = step(gh, 0.07);
    // pebbles
    vec2 pc = floor(p * 18.0);
    vec2 po = h22(pc) - 0.5;
    float pr = length(fract(p * 18.0) - 0.5 - po * 0.4);
    float peb = step(0.8, h12(pc + 9.0)) * smoothstep(0.22, 0.17, pr) * (0.5 + 0.5 * smoothstep(0.2, 0.05, length(fract(p * 18.0) - 0.5 - po * 0.4 + vec2(0.04, -0.04))));
    vec3 c;
    if (form < 0.5) {
        c = mix(vec3(0.03, 0.019, 0.012), vec3(0.14, 0.088, 0.05), smoothstep(0.25, 0.85, n));
        c += speck * vec3(0.11, 0.08, 0.055) + peb * vec3(0.07, 0.05, 0.035);
        c *= 1.0 - 0.45 * pit;
    } else if (form < 1.5) {
        c = vec3(0.003, 0.008, 0.006) + vec3(0.0, 0.022, 0.014) * smoothstep(0.45, 0.9, n) + speck * vec3(0.006, 0.014, 0.01);
    } else if (form < 2.5) {
        c = mix(vec3(0.025, 0.014, 0.009), vec3(0.1, 0.058, 0.034), smoothstep(0.25, 0.85, n));
        c += speck * vec3(0.08, 0.055, 0.035) + peb * vec3(0.05, 0.035, 0.025);
        c *= 1.0 - 0.45 * pit;
    } else {
        c = vec3(0.04, 0.036, 0.034) * (0.8 + 0.4 * n) + speck * 0.012;
    }
    return mix(vec3(lum(c) * 0.0 + 0.012), c, soilTex);
}

vec4 render(vec2 p, vec2 uv, float form, float sd, float grow) {
    aLine = 0.0; aGlow = 0.0; aTip = 0.0; aPulse = 0.0; aRoot = 0.0; aRootS = 0.0; aRootP = 0.0;
    aCap = 0.0; aCapS = 0.0; aCapR = 0.0; aHalo = 0.0; aPrint = 0.0; aCore = 0.0; aPrintCol = vec3(1.0);
    bool roots = form > 1.5 && form < 2.5;
    gRf = grow;
    for (int it = 0; it < 2; it++) {
        if (it == 1 && !roots) break;
        gMode = float(it);
        gSd = sd * 7.31 + float(it) * 3.7;
        gS0 = it == 0 ? 0.5 : 1.0;
        gOrg = it == 0 ? (h22(vec2(sd, 4.2)) - 0.5) * gS0 : vec2(h11(sd) * 0.4, -0.52);
        if (it == 1) gRf = 0.9 + 0.7 * grow;
        network(p, form);
    }
    // ── look per form ──
    vec3 thr, glc, plc, tipc, capc, halc;
    if (form < 0.5)      { thr = vec3(0.95, 0.92, 0.85); glc = vec3(0.55, 0.48, 0.38); plc = vec3(1.0, 0.82, 0.5);  tipc = vec3(1.0);              capc = vec3(0.93, 0.84, 0.68); halc = vec3(0.4, 0.33, 0.24); }
    else if (form < 1.5) { thr = vec3(0.4, 0.95, 0.55);  glc = vec3(0.1, 0.85, 0.35);  plc = vec3(0.75, 1.0, 0.9);  tipc = vec3(0.85, 1.0, 0.9);  capc = vec3(0.45, 1.0, 0.55);  halc = vec3(0.12, 0.9, 0.35); }
    else if (form < 2.5) { thr = vec3(1.0, 0.8, 0.42);   glc = vec3(0.9, 0.48, 0.12);  plc = vec3(1.0, 0.95, 0.65); tipc = vec3(1.0, 0.95, 0.8);  capc = vec3(1.0, 0.7, 0.3);    halc = vec3(0.9, 0.45, 0.1); }
    else                 { thr = vec3(0.82, 0.8, 0.76);  glc = vec3(0.45, 0.4, 0.38);  plc = vec3(1.0, 0.9, 0.75);  tipc = vec3(1.0);              capc = vec3(0.9, 0.85, 0.78);  halc = vec3(0.3, 0.26, 0.22); }
    float bio = (form > 0.5 && form < 1.5) ? 1.0 : 0.0;
    float ar = audioReact;
    // drop: the network lights up in a wave from the centre outward
    float dc = length(p);
    float front = (1.0 - gDrop) * 1.6;
    float lit = 1.0 + gDrop * (2.2 * smoothstep(front + 0.05, front - 0.25, dc) + 2.5 * exp(-pow((dc - front) * 7.0, 2.0)));
    vec3 col = backdrop(uv) + ground(p, form);
    // glow halo under the threads
    float glowK = glow * (0.22 + 0.35 * gLift + 0.5 * bio) * lit;
    col += glc * aGlow * aGlow * glowK * (form > 2.5 ? 0.5 : 1.0);
    // tree roots
    if (roots) {
        float bark = 0.75 + 0.25 * vn2(p * vec2(60.0, 9.0));
        vec3 rc = mix(vec3(0.12, 0.07, 0.04), vec3(0.52, 0.36, 0.22), aRootS) * bark;
        rc += vec3(0.85, 0.62, 0.38) * pow(aRootS, 6.0) * 0.25;
        col = mix(col, rc, aRoot);
        col += vec3(0.55, 1.0, 0.45) * aRootP * pulseAmt * (0.25 + 0.9 * audioBeatPulse * ar + 0.4 * gPeak) * lit * 0.6;
    }
    // hyphae
    float shimmer = 0.5 + 0.5 * sin(gT * (6.0 + 14.0 * gBuild) + dc * 30.0);
    float thrB = (0.62 + 0.45 * gLift) * (form > 2.5 ? 0.5 : 1.0) * mix(1.0, 0.85, bio);
    col = mix(col, thr * thrB * min(lit, 2.0), aLine);
    col += thr * aLine * max(lit - 2.0, 0.0) * 0.4;
    col += plc * aPulse * pulseAmt * (form > 2.5 ? 0.4 : 1.0) * (0.55 + 1.6 * audioBeatPulse * ar + 0.6 * gPeak + 0.5 * gBuild) * lit;
    col += tipc * aTip * (0.5 + 0.6 * gLift + 0.6 * gBuild * shimmer);
    // spore prints, dusted over the threads
    col = mix(col, aPrintCol * (0.75 + 0.25 * gLift), aPrint * 0.92);
    // fruiting bodies
    vec3 capShade;
    if (bio > 0.5) capShade = capc * (0.55 + 0.6 * aCapS) * (1.0 + 0.4 * smoothstep(0.6, 1.0, aCapR)) + vec3(0.6, 1.0, 0.7) * pow(aCapS, 8.0) * 0.5;
    else capShade = mix(capc * vec3(0.45, 0.35, 0.25), capc, aCapS) + vec3(1.0) * pow(aCapS, 10.0) * 0.35;
    capShade *= 1.0 - 0.35 * smoothstep(0.0, 0.18, 0.18 - aCapR) * (1.0 - bio);   // dimple at the cap centre
    col += halc * aHalo * aHalo * (0.25 + 0.7 * bio) * glow * (0.6 + 0.5 * gLift + 0.6 * audioBeatPulse * ar) * lit;
    col = mix(col, capShade * min(lit, 1.6), aCap);
    // spore cores
    col += mix(vec3(1.0, 0.95, 0.85), thr, 0.4) * aCore * (0.7 + 0.4 * audioBeatPulse * ar);
    float alpha = clamp(max(max(aLine, aRoot), max(aCap, aPrint)) + aGlow * 0.4 + aPulse, 0.0, 1.0);
    return vec4(col, alpha);
}

vec3 sporeTint(float form) {
    if (form < 0.5) return vec3(1.0, 0.92, 0.75);
    if (form < 1.5) return vec3(0.55, 1.0, 0.65);
    if (form < 2.5) return vec3(1.0, 0.85, 0.5);
    return vec3(1.0, 0.95, 0.88);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    gPx = 1.0 / R.y;
    gT = TIME * 0.5 + (audioMidTime * 0.3 + audioBassTime * 0.2) * ar + 1.5 * gBuild * gBuild;
    gFlow = (TIME * 0.22 + audioMidTime * 0.15 * ar + audioBassTime * 0.12 * ar) * flowSpeed + 1.2 * gBuild * gBuild;
    gPd = clamp(0.95 - 0.55 * branching - 0.2 * gLift + 0.15 * gRest, 0.1, 1.3);
    gMaxLf = 3.2 + 2.8 * depth * (0.75 + 0.35 * gLift);
    gNfrac = clamp(colonies * (0.55 + 0.5 * gLift), 0.0, 1.0);
    gFuse = fusion * (0.4 + 0.45 * gLift);
    gFruit = fruiting * (0.03 + 0.05 * gLift + 0.04 * gPeak);

    vec2 p = (fc - 0.5 * R) / R.y;
    // build: the web contracts and trembles near the top
    p *= 1.0 + 0.16 * gBuild;
    p += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.006 * smoothstep(0.7, 1.0, gBuild);
    p += 0.02 * vec2(sin(TIME * 0.05), cos(TIME * 0.041));

    // growth cycle: bass makes it grow faster; old network dims as new spores germinate
    float ph = (TIME + 0.8 * audioBassTime * ar) / morphPeriod;
    float cyc = floor(ph), fp = fract(ph);
    float fsel = float(formSel);
    float wPrev = 1.0 - smoothstep(0.04, 0.32, fp);
    float rMax = 0.74 + 0.2 * gPeak - 0.08 * gRest;
    vec3 col = vec3(0.0);
    float alpha = 0.0;
    vec3 sc = vec3(0.0);
    for (int k = 0; k < 2; k++) {
        float wk = k == 0 ? 1.0 - wPrev : wPrev;
        if (wk < 0.002) continue;
        float cc = cyc - float(k);
        float form = fsel > 0.5 ? fsel - 1.0 : mod(cc, 4.0);
        float grow = k == 0 ? rMax * pow(smoothstep(0.0, 0.8, fp), 0.9) : rMax;
        grow *= 1.0 + 0.04 * bassP() * ar;
        vec4 rc = render(p, uv, form, cc + 3.0, grow);
        col += rc.rgb * wk;
        alpha += rc.a * wk;
        sc += sporeTint(form) * wk;
    }

    // drifting spores
    float nDrift = sporeDrift * (0.35 + 0.65 * gLift);
    for (int l = 0; l < 2; l++) {
        float fl = float(l);
        float scl = 9.0 + 7.0 * fl;
        vec2 q = p * scl + vec2(gT * (0.12 + 0.06 * fl), gT * (0.2 + 0.1 * fl) + 0.4 * sin(gT * 0.3 + fl));
        vec2 id = floor(q);
        vec2 f = fract(q) - 0.5;
        vec2 hh = h22(id + fl * 17.0);
        if (hh.x > nDrift) continue;
        vec2 o = (hh - 0.5) * 0.5 + 0.12 * vec2(sin(gT * 1.3 + hh.y * 20.0), cos(gT * 1.1 + hh.x * 20.0));
        float d = length(f - o) / scl;
        float tw = 0.5 + 0.5 * sin(gT * (3.0 + 9.0 * hh.y) + hh.x * 40.0 + audioHighTime * 2.0 * ar);
        float spark = 0.25 + 0.35 * tw + 1.2 * highP() * ar * pow(tw, 4.0);
        float sz = gPx * (1.0 + 1.2 * hh.y) * (1.0 - 0.3 * fl);
        float dot1 = smoothstep(sz + gPx, sz - gPx * 0.5, d) + 0.35 * exp(-d / (sz * 1.6)) * smoothstep(0.5, 0.4, max(abs(f.x), abs(f.y)));
        col += sc * dot1 * spark * (0.6 - 0.2 * fl);
        alpha = max(alpha, dot1 * spark);
    }

    col += vec3(1.0) * gDrop * gDrop * 0.08;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    // vignette
    col *= 1.0 - 0.35 * smoothstep(0.6, 1.2, length((uv - 0.5) * vec2(R.x / R.y, 1.0)));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(alpha, 0.0, 1.0) : 1.0);
}
