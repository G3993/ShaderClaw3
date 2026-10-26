/*{
  "DESCRIPTION": "Neurons — a deep-space forest of nerve cells receding in layers through true black. Each neuron is a luminous glassy soma with a fractal crown of tapering, gently curving dendrites and one long wandering axon ending in a spray of synaptic boutons, all rendered as lit 3D tubes and spheres with a specular glint, a translucent glow and an iridescent sheen that slides from cyan through electric blue and violet to magenta and hot pink. Action potentials run as white-hot beads of light with a neon bloom inward along the dendrites, flash the cell body, race out along the axon and burst at the synapses in a puff of tiny neurotransmitter sparks, so firing cascades from cell to cell across the network. The camera drifts slowly forward: the near and middle layers are crisp, the far forest falls away soft and dim into a faint blue depth haze and huge out-of-focus cells drift past in the foreground. It morphs between four looks — Iridescent, dark glass neurons edged in an oil-slick violet-cyan-pink sheen; Neon Brainbow, every cell its own neon hue; Electric, blue-white neon; and Glia Web, a dense mesh of fine processes studded with glowing star-shaped astrocytes. Beats fire new cascades, bass swells the cell bodies, highs make the synapses sparkle and a drop fires the whole network at once. At rest only a sparse few cells fire slowly in the dark; during a build the forest thickens, the drift and firing accelerate and the light shimmers and trembles; on the drop every neuron flashes together in one outward wave before the peak runs at full density and colour.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Science", "Organic", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Iridescent", "Neon Brainbow", "Electric", "Glia Web"], "GROUP": "Form" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 16.0, "GROUP": "Form" },
    { "NAME": "density",     "LABEL": "Neuron Density",   "TYPE": "float", "MIN": 0.2, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Network" },
    { "NAME": "branching",   "LABEL": "Dendrite Spread",  "TYPE": "float", "MIN": 0.5, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Network" },
    { "NAME": "thickness",   "LABEL": "Branch Thickness", "TYPE": "float", "MIN": 0.4, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Network" },
    { "NAME": "fireRate",    "LABEL": "Firing Rate",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "drift",       "LABEL": "Forward Drift",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "dof",         "LABEL": "Depth of Field",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "glow",        "LABEL": "Glow",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.004, 0.02, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718
#define NLAY 5

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec3  h32(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yxz + 33.33); return fract((p3.xxy + p3.yzz) * p3.zyx); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
vec2 r2(vec2 v, float a) { float c = cos(a), s = sin(a); return vec2(c * v.x - s * v.y, s * v.x + c * v.y); }
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}

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

// form weights: Iridescent, Brainbow, Electric, Glia
float wG, wB, wE, wL;

// ── one neuron: nearest element of soma / dendrite tree / axon ─────────
float nDist, nW, nS, nType, nK, nBout, nSoma;
vec2  nTip;

void consider(float dist, float w, float s, float typ, float k) {
    if (dist - w < nDist - nW) { nDist = dist; nW = w; nS = s; nType = typ; nK = k; }
}

void neuron(vec2 q, vec3 h, float somaR) {
    nDist = 1e3; nW = 0.0; nS = 0.0; nType = 0.0; nK = 0.0; nBout = 1e3; nTip = vec2(1e3);
    float th = thickness;
    // soma: slightly pyramidal teardrop pointing along the main apical dendrite
    float a0 = h.x * TAU;
    float ang = atan(q.y, q.x);
    float rq = length(q);
    float sr = somaR * (1.0 + 0.18 * cos(ang - a0) + 0.05 * cos(2.0 * (ang - a0) + h.y * 6.0));
    consider(rq, sr, 0.0, 2.0, 0.0);
    nSoma = rq - sr;
    float reach = 0.78 * branching;
    // ── dendrites: 5 primaries, evaluate the two nearest sectors, fractal fold
    if (rq < reach + 0.06) {
        float sec = TAU / 5.0;
        float f = (ang - a0) / sec;
        float k0 = floor(f + 0.5);
        float k1 = k0 + (fract(f + 0.5) > 0.5 ? 1.0 : -1.0);
        for (int pi = 0; pi < 2; pi++) {
            float kk = pi == 0 ? k0 : k1;
            float km = mod(kk, 5.0);
            float hk = h11(km * 7.13 + h.z * 91.7);
            float thk = a0 + kk * sec + (hk - 0.5) * sec * 0.45;
            vec2 p = r2(q, PI * 0.5 - thk);
            float L = (km < 0.5 ? 0.24 : 0.17 + 0.07 * hk) * branching;   // k=0 is the long apical dendrite
            float w = (km < 0.5 ? 0.017 : 0.013) * th;
            float bend = (h11(hk * 31.1) - 0.5) * 0.5;
            float acc = 0.0;
            float sd = hk * 17.0 + h.y * 5.0;
            for (int lv = 0; lv < 5; lv++) {
                float yy = clamp(p.y, 0.0, L);
                float off = bend * yy * yy / L;
                float dist = length(vec2(p.x - off, p.y - yy));
                consider(dist, w * (1.0 - 0.4 * yy / L) + 0.0012, acc + yy, 0.0, hk);
                // to the fork
                p -= vec2(bend * L, L);
                acc += L;
                float side = p.x > 0.0 ? 1.0 : 0.0;
                sd = h11(sd * 1.37 + side * 3.71 + float(lv) * 0.53);
                if (lv >= 1 && sd < 0.12) break;                         // some twigs end early
                p.x = abs(p.x);
                p = r2(p, 0.32 + 0.42 * sd);
                L *= 0.68 + 0.18 * h11(sd * 9.7);
                w *= 0.66;
                bend = (h11(sd * 5.3) - 0.5) * 0.7;
                if (dot(p, p) > L * L * 7.0 + 0.003) break;              // outside this subtree
            }
        }
    }
    // ── axon: long wandering fibre out the back, terminal arbor + boutons
    float tha = a0 + PI + (h.y - 0.5) * 0.9;
    vec2 p = r2(q, PI * 0.5 - tha);
    float La = (0.62 + 0.3 * h.z) * branching;
    float yy = clamp(p.y, 0.0, La);
    float wob = 0.045 * sin(yy * 7.0 + h.y * 9.0) * smoothstep(0.0, 0.15, yy) + 0.02 * sin(yy * 17.0 + h.x * 5.0);
    float dax = length(vec2(p.x - wob, p.y - yy));
    consider(dax, 0.0055 * th + 0.0012, yy, 1.0, 0.0);
    // terminal arbor at the end of the axon
    vec2 e = p - vec2(0.045 * sin(La * 7.0 + h.y * 9.0) + 0.02 * sin(La * 17.0 + h.x * 5.0), La);
    if (dot(e, e) < 0.09) {
        float sd = h.z * 13.0;
        float L = 0.085;
        float acc = La;
        for (int lv = 0; lv < 3; lv++) {
            sd = h11(sd * 1.91 + float(lv));
            e.x = abs(e.x);
            e = r2(e, 0.45 + 0.4 * sd);
            float ty = clamp(e.y, 0.0, L);
            consider(length(vec2(e.x, e.y - ty)), 0.0035 * th + 0.0012, acc + ty, 3.0, 0.0);
            e.y -= L; acc += L;
            L *= 0.72;
        }
        float bd = length(e) - 0.011 * th;
        if (bd < nBout) { nBout = bd; nTip = e; }
        consider(length(e), 0.011 * th, acc, 4.0, 0.0);
    }
}

// fine glial mesh (voronoi edges)
float gliaMesh(vec2 p, out float starD, out vec2 starQ, out vec2 starId) {
    vec2 ip = floor(p), fp = fract(p);
    float f1 = 8.0, f2 = 8.0;
    vec2 best = vec2(0.0);
    starId = vec2(0.0);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 o = vec2(float(i), float(j));
        vec2 c = o + h32(ip + o + 7.7).xy * 0.8 + 0.1 - fp;
        float d = dot(c, c);
        if (d < f1) { f2 = f1; f1 = d; best = c; starId = ip + o; } else if (d < f2) { f2 = d; }
    }
    starD = sqrt(f1);
    starQ = -best;
    return sqrt(f2) - sqrt(f1);
}

// cool neon ramp: cyan -> electric blue -> violet -> magenta -> hot pink (ping-pong, never warm)
vec3 coolHue(float h) {
    float t = abs(fract(h) * 2.0 - 1.0);
    vec3 c = mix(vec3(0.0, 0.85, 1.0), vec3(0.08, 0.32, 1.0), smoothstep(0.0, 0.25, t));
    c = mix(c, vec3(0.52, 0.14, 1.0), smoothstep(0.25, 0.5, t));
    c = mix(c, vec3(0.95, 0.08, 0.92), smoothstep(0.5, 0.75, t));
    return mix(c, vec3(1.0, 0.12, 0.55), smoothstep(0.75, 1.0, t));
}

void main() {
    showDynamics();
    vec2 RS = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / RS;
    vec2 sp = (fc - 0.5 * RS) / RS.y;
    float ar = audioReact;
    float beat = audioBeatPulse * ar;

    // ── form morph
    float ph = TIME / morphPeriod;
    float fsel = float(formSel);
    float fa = fsel > 0.5 ? fsel - 1.0 : mod(floor(ph), 4.0);
    float fb = fsel > 0.5 ? fa : mod(fa + 1.0, 4.0);
    float mt = fsel > 0.5 ? 0.0 : smoothstep(0.78, 1.0, fract(ph));
    wG = mix(fa < 0.5 ? 1.0 : 0.0, fb < 0.5 ? 1.0 : 0.0, mt);
    wB = mix(abs(fa - 1.0) < 0.5 ? 1.0 : 0.0, abs(fb - 1.0) < 0.5 ? 1.0 : 0.0, mt);
    wE = mix(abs(fa - 2.0) < 0.5 ? 1.0 : 0.0, abs(fb - 2.0) < 0.5 ? 1.0 : 0.0, mt);
    wL = mix(fa > 2.5 ? 1.0 : 0.0, fb > 2.5 ? 1.0 : 0.0, mt);

    // ── clocks (no time jumps: base + audio clocks + bounded offsets)
    float gT = TIME * 0.6 + (audioMidTime * 0.35 + audioBassTime * 0.2) * ar + 1.2 * gBuild * gBuild;
    float fireT = (TIME * 0.16 + audioMidTime * 0.08 * ar + audioBassTime * 0.05 * ar) * fireRate + 0.9 * gBuild * gBuild;
    float zoomT = (TIME * 0.035 + audioBassTime * 0.012 * ar) * drift + 0.25 * gBuild * gBuild;

    // build tremor near the top of the riser
    float trem = smoothstep(0.6, 1.0, gBuild);
    sp += trem * 0.0035 * vec2(sin(TIME * 61.0), cos(TIME * 53.0));
    vec2 camDrift = vec2(sin(TIME * 0.043) * 0.5 + 0.2 * sin(TIME * 0.11), cos(TIME * 0.031) * 0.35);
    float camRot = 0.12 * sin(TIME * 0.027);

    float dens = density * mix(1.0, 0.4, gRest) * (1.0 + 0.25 * gPeak + 0.2 * gBuild);
    float fireProb = clamp(0.35 + 0.45 * gLift + 0.3 * gPeak - 0.25 * gRest + 0.35 * beat, 0.1, 1.0);
    float pulseGain = (0.8 + 0.7 * gLift + 0.4 * gPeak - 0.3 * gRest) * (1.0 + 1.2 * beat);
    float hiSpark = highP() * ar;
    float somaBoost = bassP() * ar;

    // background: true black + a whisper of deep-blue depth haze
    vec2 bp = sp + camDrift * 0.05;
    float neb = vnoise(bp * 2.2 + 3.0) * 0.6 + vnoise(bp * 5.1 - TIME * 0.01) * 0.4;
    vec3 space = backdrop(uv);
    vec3 nebC = wE * vec3(0.004, 0.012, 0.04) + wB * vec3(0.016, 0.004, 0.03) + wL * vec3(0.002, 0.014, 0.03) + wG * vec3(0.01, 0.006, 0.03);
    space += nebC * smoothstep(0.3, 0.9, neb) * (0.6 + 0.5 * gLift);
    float star = step(0.9965, h12(floor(fc * 0.5))) * (0.5 + 0.5 * sin(TIME * 2.0 + h12(floor(fc * 0.5) + 3.0) * 30.0));
    space += star * 0.22 * vec3(0.75, 0.85, 1.0);

    vec3 colE = space;
    float alpha = 0.0;
    float uZ = fract(zoomT);
    float lyBase = floor(zoomT);
    vec2 dirC = vec2(cos(TIME * 0.05), sin(TIME * 0.05));
    vec3 Ldir = normalize(vec3(-0.45, 0.55, 0.7));

    for (int j = 0; j < NLAY; j++) {                  // back to front
        float z = (float(NLAY - j) - uZ) / float(NLAY);  // 1 far -> 0 near
        float lid = lyBase - float(j);
        float fade = smoothstep(0.0, 0.16, z) * (1.0 - smoothstep(0.8, 1.0, z));
        if (fade < 0.01) continue;
        float sc = 1.25 * exp2(z * 3.2);
        float px = sc / RS.y;
        float bS = dof * 0.010 * pow(abs(z - 0.42) / 0.5, 1.4) * (z < 0.42 ? 2.2 : 1.0);
        float bW = bS * sc;
        float fog = mix(1.0, 0.1, smoothstep(0.2, 0.95, z));
        vec3 haze = vec3(0.0, 0.01, 0.035) * smoothstep(0.45, 1.0, z);
        vec2 lp = r2(sp + camDrift * (1.25 - z) * 0.6, camRot * (1.0 - z)) * sc + vec2(lid * 17.31, lid * 9.17);
        vec2 ip = floor(lp);

        float cov = 0.0;
        vec3 covC = vec3(0.0);
        vec3 emi = vec3(0.0);
        vec3 halo = vec3(0.0);
        for (int oy = -1; oy <= 1; oy++)
        for (int ox = -1; ox <= 1; ox++) {
            vec2 cid = ip + vec2(float(ox), float(oy));
            vec3 h = h32(cid + lid * 3.7);
            if (h12(cid * 1.31 + lid) > dens * 0.6) continue;
            vec2 soma = cid + 0.5 + (h.yz - 0.5) * 0.45;
            vec2 q = lp - soma;
            if (dot(q, q) > 1.3) continue;

            // firing cycle: cascade wave across the sheet + per-cell jitter
            float cyc = fireT - dot(cid, dirC) * 0.28 + h.x * 0.35;
            float fph = fract(cyc);
            float fid = floor(cyc);
            float fires = step(h12(cid + fid * 1.7 + lid), fireProb);
            float somaR = 0.05 * (1.0 + 0.4 * somaBoost + 0.3 * exp(-pow((fph - 0.45) / 0.05, 2.0)) * fires + 0.5 * gDrop);
            neuron(q, h, somaR);

            float edge = nDist - nW;
            float soft = px + bW;
            float cv = 1.0 - smoothstep(-soft * 0.5, soft, edge);
            cv *= clamp((2.0 * nW + 0.4 * px) / (px + bW * 0.8), 0.0, 1.0);
            float hl = exp(-max(edge, 0.0) / (0.03 + bW * 1.5));
            float hl2 = exp(-max(edge, 0.0) / (0.007 + bW * 0.5));

            // action potentials
            float em = 0.0;
            float maxD = 0.6 * branching;
            if (nType < 0.5) {                                     // dendrites: inward beads
                float bgate = step(h11(nK * 13.0 + fid), 0.75) * fires;
                float spos = maxD * (1.0 - smoothstep(0.0, 0.45, fph));
                em = 1.3 * exp(-pow((nS - spos) / 0.022, 2.0)) * bgate * step(fph, 0.47);
                em += 0.05 * pow(0.5 + 0.5 * sin(nS * 45.0 - gT * 4.0 + h.x * 20.0), 8.0) * (0.3 + gLift + 1.5 * trem * (0.5 + 0.5 * sin(TIME * 30.0)));
            } else if (nType < 1.5 || nType > 2.5) {                // axon & terminals: outward bead
                float La = (0.62 + 0.3 * h.z) * branching;
                float spos = (La + 0.22) * smoothstep(0.45, 0.88, fph);
                em = exp(-pow((nS - spos) / 0.05, 2.0)) * fires * step(0.43, fph);
                em += 0.08 * pow(0.5 + 0.5 * sin(nS * 30.0 + gT * 3.0), 10.0) * (0.3 + gLift);
            }
            float somaF = exp(-pow((fph - 0.47) / 0.06, 2.0)) * fires;
            if (nType > 1.5 && nType < 2.5) em = 0.0;
            float somaE = (0.1 + 0.9 * somaF + 0.4 * somaBoost + gDrop) * ((1.0 - smoothstep(-soft, soft, nSoma)) * 0.38 + 0.26 * exp(-max(nSoma, 0.0) / (0.016 + bW * 1.2)));
            // synaptic flash + neurotransmitter sparks
            float syn = exp(-pow((fph - 0.9) / 0.04, 2.0)) * fires;
            float bflash = (syn * 1.1 + hiSpark * 0.8 * step(0.6, h12(cid + floor(TIME * 12.0)))) * exp(-max(nBout, 0.0) / (0.005 + bW * 0.3)) / (1.0 + bW * 30.0);
            float sparks = 0.0;
            if (nBout < 0.07) {
                float st = smoothstep(0.86, 1.0, fph);
                for (int s = 0; s < 4; s++) {
                    float fs = float(s);
                    float a = h11(fs * 3.1 + h.x * 40.0 + fid) * TAU;
                    vec2 sp2 = vec2(cos(a), sin(a)) * (0.012 + 0.05 * st * (0.6 + 0.6 * h11(fs + h.y * 9.0)));
                    sparks += exp(-dot(nTip - sp2, nTip - sp2) / (0.00002 + bW * bW + px * px)) * (1.0 - st) * st * 4.0;
                }
                sparks *= fires * (1.0 + 2.0 * hiSpark);
            }
            // drop: the whole network fires at once, wave outward from every soma
            em += gDrop * 1.5 * exp(-pow((nS - (1.0 - gDrop) * 0.9) / 0.12, 2.0));
            em = (em + somaE) * pulseGain / (1.0 + bW * 45.0);

            // ── colour: cool neon only
            vec3 hue = wE * vec3(0.3, 0.6, 1.0) + wB * coolHue(h.x * 1.7 + lid * 0.13) + wL * mix(vec3(0.1, 0.85, 0.95), vec3(0.45, 0.4, 1.0), h.y);
            hue += wG * coolHue(h.x + nS * 0.9 + 0.02 * gT);
            // ── 3D lit tube / sphere shading (fades to flat when sub-pixel or defocused)
            float tw = clamp(nDist / max(nW, 1e-5), 0.0, 1.0);
            float nzz = sqrt(max(1.0 - tw * tw, 0.0));
            float isSoma = step(1.5, nType) * step(nType, 2.5);
            vec3 nrm = isSoma > 0.5 ? normalize(vec3(q / max(nW, 1e-4) * 0.95, nzz + 0.05)) : vec3(0.0, 0.0, 1.0);
            float dif = isSoma > 0.5 ? max(dot(nrm, Ldir), 0.0) : nzz;
            float spec = isSoma > 0.5 ? pow(max(dot(reflect(-Ldir, nrm), vec3(0.0, 0.0, 1.0)), 0.0), 28.0) : pow(nzz, 26.0);
            float rim = pow(tw, 2.2) * (isSoma > 0.5 ? 1.0 : 0.85);
            float shK = clamp(nW * 2.0 / (px + bW * 1.2) - 0.6, 0.0, 1.0) * (1.0 - smoothstep(0.3, 0.8, z) * 0.6);
            vec3 iri = coolHue(h.y * 3.0 + tw * 0.7 + nS * 1.3 + 0.03 * gT + 0.35 * wG);
            float bodyA = mix(1.0, 0.16, wG);                   // Iridescent: dark glass body
            float lineB = wE * 0.6 + wB * 0.85 + wL * 0.32 + wG * 0.95;
            lineB *= 0.75 + 0.3 * gLift;
            vec3 flatC = hue * lineB * mix(1.0, 0.45, wG);
            vec3 lit = hue * lineB * bodyA * (0.22 + 0.78 * dif)
                     + iri * rim * lineB * (0.55 + 0.9 * wG)
                     + vec3(0.85, 0.92, 1.0) * spec * (0.55 + 0.35 * gLift);
            vec3 shaded = mix(flatC, lit, shK);
            if (cv > cov) { cov = cv; covC = shaded; }
            halo += mix(hue, iri, 0.5) * hl * 0.012 * glow * (0.5 + gLift);
            // firing light: white-hot core, neon bloom around it
            float hlW = exp(-max(edge, 0.0) / (0.06 + bW * 2.0));
            vec3 bloomC = mix(hue, iri, 0.4 + 0.4 * wG);
            vec3 ee = em * (vec3(1.0) * cv * (1.0 + 0.6 * glow) + bloomC * (0.55 * hl2 + 0.12 * hlW) * (0.6 + 0.7 * glow))
                    + mix(bloomC, vec3(1.0), 0.55) * (bflash + sparks) * (1.0 + 0.6 * hiSpark);
            emi += ee;
        }

        // glia: astrocyte stars + fine mesh woven between the neurons
        if (wL > 0.01) {
            float sD; vec2 sQ, sId;
            float me = gliaMesh(lp * 2.3 + 11.0 + 0.15 * sin(lp.yx * 5.0 + gT * 0.3), sD, sQ, sId);
            vec3 sh = h32(sId + lid * 5.1);
            float mw = 0.03 + bW * 2.3 * 2.0;
            float mcov = (1.0 - smoothstep(0.0, mw + px * 2.3, me)) * clamp((0.03 + px) / (mw), 0.0, 1.0);
            vec3 mc = mix(vec3(0.05, 0.5, 0.65), vec3(0.4, 0.25, 0.95), vnoise(lp * 0.7));
            float mpulse = pow(0.5 + 0.5 * sin(dot(lp, vec2(3.0, 2.0)) - gT * 2.0), 6.0);
            colE += mc * mcov * wL * 0.38 * fade * fog * (0.5 + 0.4 * gLift + 0.8 * mpulse * gLift);
            float sa = atan(sQ.y, sQ.x);
            float sr = sD / 2.3;
            float nr = 3.0 + floor(sh.y * 3.0);
            float rays = pow(abs(cos(sa * nr + sh.z * 6.0 + sin(sr * 25.0 + sh.x * 9.0) * 0.35)), 14.0 / (1.0 + bW * 40.0));
            float starI = (rays * exp(-sr / 0.15) * 0.9 + exp(-sr / (0.018 + bW))) * (0.6 + 0.5 * gLift + 0.5 * somaBoost + 0.4 * sin(gT + sh.x * 30.0)) * step(sh.x, 0.3 + 0.15 * gPeak);
            colE += mix(vec3(0.4, 0.85, 1.0), vec3(0.8, 0.5, 1.0), sh.z) * starI * wL * 0.62 * fade * fog;
        }

        // composite back to front: occlude what's behind, then add this layer's light
        colE = colE * (1.0 - cov * fade * 0.85) + (covC * cov + halo + haze * cov) * fade * fog;
        colE += emi * fade * fog;
        alpha = max(alpha, cov * fade);
    }

    vec3 col = colE;
    float vig = 1.0 - smoothstep(0.6, 1.3, length(sp * vec2(0.8, 1.0)));
    col *= mix(1.0, vig, 0.55);
    col += vec3(0.85, 0.9, 1.0) * gDrop * gDrop * 0.1;
    col *= 1.0 + 0.1 * beat;
    col = col / (1.0 + 0.25 * max(col - 0.8, 0.0));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.03 * (0.3 + lum(col));
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = clamp(col, 0.0, 1.0);
    if (float(paletteMode) < 0.5) {
        // Colorful re-hues near-grey light by brightness: keep hot whites ice-blue so they stay cool neon
        float mxc = max(col.r, max(col.g, col.b)), mnc = min(col.r, min(col.g, col.b));
        float stc = mxc > 1e-3 ? (mxc - mnc) / mxc : 1.0;
        col *= mix(vec3(0.76, 0.9, 1.0), vec3(1.0), smoothstep(0.08, 0.26, stc));
    }
    col = studioPalette(clamp(col, 0.0, 1.0));
    alpha = clamp(alpha + lum(col), 0.0, 1.0);
    gl_FragColor = vec4(col, transparentBg ? alpha : 1.0);
}
