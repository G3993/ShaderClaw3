/*{
  "DESCRIPTION": "Mitosis — a living colony of cells dividing under the microscope. Each glowing cell has a translucent wobbling membrane, drifting cytoplasm granules and a textured nucleus; one by one, at their own pace, they go through the whole cell cycle: the chromatin condenses into X-shaped chromosomes stained magenta with cyan kinetochores, two centrosome stars split and migrate to opposite poles, a green spindle of fibres grows between them and the chromosomes line up on the metaphase plate, then the sister chromatids are torn apart into V-shapes racing toward the poles, the membrane pinches in a cleavage furrow, a bright midbody bridge snaps and two round daughter cells drift apart — and divide again. Three microscope looks: Fluorescence (neon stains glowing on black), Phase Contrast (grey field, cells ringed with bright optical halos, dark chromosomes) and Stained Slide (H&E-style pink cytoplasm and purple nuclei on a pale glass slide); Auto Morph cross-dissolves between them. Bass pulses the membranes, mids make them wobble, highs make the granules sparkle and the beat drives the cycle forward. At rest the colony is sparse, large and slow; during a build the cells contract, the cycle races and the membranes start to tremble; on the drop every cell splits at once in a bright shockwave, and the peak is a dense, saturated, fast-dividing culture.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Organic", "Science", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Microscope", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Auto Morph", "Fluorescence", "Phase Contrast", "Stained Slide"], "GROUP": "Look" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 18.0, "GROUP": "Look" },
    { "NAME": "glow",        "LABEL": "Glow",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "cellDensity", "LABEL": "Cells Across",     "TYPE": "float", "AUDIO": false, "MIN": 1.5, "MAX": 7.0, "DEFAULT": 3.2, "GROUP": "Colony" },
    { "NAME": "cycleSpeed",  "LABEL": "Division Speed",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Colony" },
    { "NAME": "wobble",      "LABEL": "Membrane Wobble",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Colony" },
    { "NAME": "granules",    "LABEL": "Granules",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Colony" },
    { "NAME": "drift",       "LABEL": "Drift",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Colony" },
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
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.02, 0.03, 0.05, 1.0], "GROUP": "Background" },
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

float smin(float a, float b, float k) {
    if (k <= 1e-4) return min(a, b);
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}
float sdSeg(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
    return length(pa - ba * h);
}

// accumulated microscope fields (max-composited over all cells)
float fBody, fMem, fHalo, fNuc, fNucl, fChrom, fKin, fSpin, fCent, fMid, fMito;
float gT, gPxC, gRb, gWob;

// membrane wobble in world angle so it is continuous across divisions
float wob(float th, float seed) {
    float w = 0.035 * sin(3.0 * th + gT * 0.9 + seed * 6.28)
            + 0.025 * sin(5.0 * th - gT * 1.3 + seed * 3.1)
            + 0.012 * sin(8.0 * th + gT * 2.1 + seed * 9.7);
    w *= gWob;
    w += gBuild * gBuild * gBuild * 0.03 * sin(11.0 * th + TIME * 37.0 + seed * 5.0);   // tremor near the top of a build
    return w;
}

// draw one cell. x: world, C: world centre, ang: division axis, p: cycle phase,
// seedA/seedB: lineage seeds of the two halves, fade: visibility
void cellDraw(vec2 x, vec2 C, float ang, float p, float seedA, float seedB, float fade, float dropK) {
    if (fade < 0.01) return;
    vec2 q = rot(-ang) * (x - C) / gRb;                    // cell units, x along division axis
    float lq = length(q);
    if (lq > 2.4) return;

    float dsp = clamp(gDrop * 1.25, 0.0, 1.0) * dropK;             // drop: every cell splits at once
    float grow = smoothstep(0.0, 0.4, p);
    float pro  = max(smoothstep(0.38, 0.52, p), dsp);
    float pro2 = max(smoothstep(0.40, 0.54, p), dsp);
    float meta = max(smoothstep(0.50, 0.60, p), dsp);
    float ana  = max(smoothstep(0.64, 0.78, p), dsp);
    float telo = max(smoothstep(0.76, 0.92, p), dsp * 0.85);
    float sepd = smoothstep(0.90, 1.0, p);
    float mito = smoothstep(0.38, 0.5, p) * (1.0 - smoothstep(0.9, 1.0, p));
    mito = max(mito, dsp);

    float r = mix(0.76, 1.0, grow) * mix(1.0, 0.86, ana) * mix(1.0, 0.88, telo) * mix(0.75, 1.0, fade);
    r *= 1.0 + 0.07 * mito;                                 // mitotic cells round up
    float s = 0.3 * ana + 0.52 * telo + 0.12 * sepd;
    float k = 0.6 * ana * (1.0 - telo);
    float split = smoothstep(0.55, 0.85, p);

    // the two halves
    vec2 qa = q - vec2(s, 0.0), qb = q + vec2(s, 0.0);
    float tha = atan(qa.y, qa.x) + ang, thb = atan(qb.y, qb.x) + ang;
    float wa = wob(tha, seedA);
    float wb = mix(wob(thb, seedA), wob(thb, seedB), split);
    float da = length(qa) - r * (1.0 + wa);
    float db = length(qb) - r * (1.0 + wb);
    float d = smin(da, db, k);

    float px = gPxC;
    float body = 1.0 - smoothstep(-px * 1.5, px * 1.5, d);
    float mem  = exp(-abs(d) / (0.022 + px)) + 0.35 * exp(-max(-d, 0.0) / 0.12) * body;
    float halo = exp(-max(d, 0.0) / 0.09) * (1.0 - body) * (0.6 + 0.8 * mito - 0.4 * dsp);

    fBody = max(fBody, body * fade);
    fMem  = max(fMem, mem * fade);
    fHalo = max(fHalo, halo * fade);
    fMito = max(fMito, mito * body * fade);

    // nuclei: one in interphase, envelope breaks down in prophase, two re-form in telophase
    float nucVis = max(1.0 - smoothstep(0.46, 0.55, p), smoothstep(0.84, 0.97, p)) * (1.0 - dsp * 0.8);
    float rn = 0.45 * r;
    if (nucVis > 0.01) {
        for (int h = 0; h < 2; h++) {
            float sg = h == 0 ? 1.0 : -1.0;
            float sd = h == 0 ? seedA : mix(seedA, seedB, split);
            vec2 nq = q - vec2(sg * s, 0.0);
            vec2 wq = rot(ang) * nq;                         // world-oriented for texture
            float th = atan(nq.y, nq.x);
            float dn = length(nq) - rn * (1.0 + 0.04 * sin(2.0 * (th + ang) + gT * 0.7 + sd * 4.0) * gWob);
            float inN = 1.0 - smoothstep(-px * 1.5, px * 1.5, dn);
            float tex = vn2(wq * 11.0 + sd * 37.0) * 0.6 + vn2(wq * 26.0 - sd * 11.0) * 0.4;
            tex = mix(tex, smoothstep(0.45, 0.75, tex), pro);   // chromatin clumps as it condenses
            float env = exp(-abs(dn) / (0.02 + px));
            float nv = (inN * (0.45 + 0.75 * tex) + env * 0.5) * nucVis * fade;
            fNuc = max(fNuc, nv);
            vec2 nlo = (h22(vec2(sd * 91.0, 3.0)) - 0.5) * rn * 0.8;
            float nl = 1.0 - smoothstep(0.07 * r, 0.1 * r, length(wq - nlo));
            fNucl = max(fNucl, nl * inN * nucVis * fade);
        }
    }

    // centrosomes: split from beside the nucleus and migrate to the poles
    float centVis = max(smoothstep(0.37, 0.45, p), dsp) * (1.0 - smoothstep(0.9, 0.99, p));
    float xc = mix(0.05, 0.68, pro2) + s * 0.95;
    float yc = mix(0.52, 0.0, pro2);
    if (centVis > 0.01) {
        for (int h = 0; h < 2; h++) {
            float sg = h == 0 ? 1.0 : -1.0;
            vec2 cq = q - vec2(sg * xc, yc);
            float rc = length(cq);
            float a = atan(cq.y, cq.x);
            float core = exp(-rc * rc / 0.0012) + 0.4 * exp(-rc / 0.05);
            float rays = pow(max(cos(a * 9.0 + seedA * 20.0 + sg), 0.0), 10.0) * exp(-rc / (0.12 + 0.12 * pro2));
            fCent = max(fCent, core * centVis * fade);
            fSpin = max(fSpin, rays * 0.8 * pro2 * centVis * fade);
        }
    }

    // spindle: a lens of fibres between the poles
    float spinVis = max(smoothstep(0.47, 0.57, p), dsp) * (1.0 - smoothstep(0.84, 0.95, p));
    if (spinVis > 0.01 && abs(q.x) < xc) {
        float u = q.x / xc;
        float W = 0.42 * r * (1.0 - u * u) + 0.004;
        float v = q.y / W;
        float fib = pow(0.5 + 0.5 * cos(v * PI * 6.0 + 0.5 * sin(u * 3.0 + gT)), 5.0);
        fib *= 1.0 - smoothstep(0.75, 1.05, abs(v));
        fib = fib * 0.85 + 0.25 * exp(-v * v * 2.0);
        fib *= smoothstep(1.0, 0.88, abs(u)) * 0.9 + 0.1;
        fSpin = max(fSpin, fib * spinVis * fade * body);
    }
    // midbody: the last bridge before the daughters separate
    float midV = telo * (1.0 - sepd) * smoothstep(0.82, 0.9, p);
    if (midV > 0.01) {
        float mb = exp(-q.x * q.x / 0.006 - q.y * q.y / 0.0015);
        fMid = max(fMid, mb * midV * fade);
    }

    // chromosomes: condense → align on the metaphase plate → sisters pulled to the poles
    float chromVis = pro * (1.0 - smoothstep(0.86, 0.98, p)) * fade;
    if (chromVis > 0.01 && lq < 2.0) {
        float dmin = 1e3, kmin = 1e3;
        for (int i = 0; i < 7; i++) {
            float fi = float(i);
            vec2 hh = h22(vec2(seedA * 53.0 + fi, fi * 7.1 + 1.3));
            float h3 = h11(seedA * 17.0 + fi * 3.3);
            vec2 pos0 = (hh - 0.5) * 0.62;
            float ang0 = h3 * PI;
            vec2 plate = vec2((hh.y - 0.5) * 0.04, (fi - 3.0) / 3.0 * 0.44 + (hh.x - 0.5) * 0.03);
            vec2 base = mix(pos0, plate, meta);
            base += 0.012 * vec2(sin(gT * 2.3 + fi * 1.7), cos(gT * 1.9 + fi * 2.3)) * (1.0 - ana);
            float a = ang0 * (1.0 - meta);
            mat2 ra = rot(a);
            float L = (0.1 + 0.05 * hh.y) * mix(0.55, 1.0, pro);
            float sp = mix(mix(0.6, 0.85, meta), -0.95, ana);
            for (int h = 0; h < 2; h++) {
                float sg = h == 0 ? 1.0 : -1.0;
                vec2 csM = base + ra * vec2(sg * 0.018, 0.0);
                vec2 csA = vec2(sg * (s + mix(0.018, 0.47 + 0.05 * hh.y, ana) * (1.0 - telo)), base.y * mix(1.0, 0.75, ana));
                vec2 cs = mix(csM, csA, ana);
                vec2 d1 = ra * vec2(sg * sin(sp), cos(sp));
                vec2 d2 = ra * vec2(sg * sin(sp), -cos(sp));
                float dd = min(sdSeg(q, cs, cs + L * d1), sdSeg(q, cs, cs + L * d2));
                dmin = min(dmin, dd);
                kmin = min(kmin, length(q - cs));
            }
        }
        float w = mix(0.05, 0.022, pro) * (1.0 + 0.8 * telo);
        float ch = (1.0 - smoothstep(w - px, w + px * 1.5, dmin)) + 0.35 * exp(-dmin / 0.05);
        fChrom = max(fChrom, ch * chromVis);
        fKin = max(fKin, exp(-kmin * kmin / 0.0009) * chromVis * smoothstep(0.45, 0.6, p));
    }
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    gT = TIME * 0.8 + (audioMidTime * 0.5 + audioBassTime * 0.2) * ar + 1.5 * gBuild * gBuild;
    gWob = wobble * (0.8 + 0.6 * midP() * ar + 0.4 * gPeak);

    // colony framing: sparse and large at rest, contracting in a build
    float dens = cellDensity * (1.0 - 0.22 * gRest + 0.12 * gBuild + 0.1 * gPeak);
    vec2 x = sp * dens;
    // slow flow of the whole culture plus a gentle swirling current
    float dt = (TIME * 0.035 + audioBassTime * 0.02 * ar) * drift;
    x += vec2(dt, dt * 0.37);
    x += 0.06 * drift * vec2(sin(x.y * 0.9 + TIME * 0.21), cos(x.x * 0.8 - TIME * 0.17));
    gPxC = dens / R.y;
    gRb = 0.32 * (1.0 - 0.1 * gBuild) * (1.0 + 0.06 * bassP() * ar);
    gPxC /= gRb;

    fBody = 0.0; fMem = 0.0; fHalo = 0.0; fNuc = 0.0; fNucl = 0.0; fChrom = 0.0;
    fKin = 0.0; fSpin = 0.0; fCent = 0.0; fMid = 0.0; fMito = 0.0;

    // cell cycle clock: beats push the cycle forward, builds race it
    float cyc = TIME * 0.055 * cycleSpeed + (audioBassTime * 0.035 + audioMidTime * 0.01) * ar + 0.3 * gBuild * gBuild;
    float hDiv = 0.94 * gRb;                                // half-separation of daughters at the end of a cycle
    vec2 g = floor(x);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 site = g + vec2(float(i), float(j));
        float hs = h12(site + 17.3);
        float rate = 0.8 + 0.4 * h12(site * 1.7 + 4.1);
        // sparse colony at rest: some sites stay empty
        float alive = smoothstep(hs - 0.08, hs + 0.08, mix(1.12, 0.5, gRest));
        if (alive < 0.01) continue;
        float P = cyc * rate + hs * 7.0;
        float kc = floor(P), p = fract(P);
        vec2 jk  = (h22(site * 3.1 + kc * 1.37) - 0.5) * 0.34;
        vec2 jp  = (h22(site * 3.1 + (kc - 1.0) * 1.37) - 0.5) * 0.34;
        float ak  = h12(site * 5.3 + kc * 2.17) * PI;
        float apv = h12(site * 5.3 + (kc - 1.0) * 2.17) * PI;
        vec2 Pk = site + 0.5 + jk;
        vec2 Pp = site + 0.5 + jp;
        vec2 dirP = vec2(cos(apv), sin(apv));
        vec2 Astart = Pp + hDiv * dirP;
        vec2 C = mix(Astart, Pk, smoothstep(0.0, 0.4, p));
        float seedA = hs;
        float seedB  = h12(site * 7.7 + kc * 3.9);
        float seedBp = h12(site * 7.7 + (kc - 1.0) * 3.9);
        cellDraw(x, C, ak, p, seedA, seedB, alive, 1.0);
        // the other daughter of the previous division drifts off and sinks out of focus
        if (p < 0.38) {
            float gf = 1.0 - smoothstep(0.08, 0.38, p);
            vec2 B = Pp - hDiv * dirP - dirP * 0.1 * gRb * smoothstep(0.0, 0.38, p);
            cellDraw(x, B, ak, 0.0, seedBp, seedBp, gf * alive, 0.0);
        }
    }

    // drifting cytoplasm granules
    vec2 gq = x * 26.0 + vec2(sin(TIME * 0.3 + x.y * 3.0), cos(TIME * 0.27 + x.x * 3.0)) * 1.5;
    vec2 gid = floor(gq);
    vec2 gf2 = fract(gq) - 0.5 - (h22(gid) - 0.5) * 0.5;
    float gh = h12(gid + 5.0);
    float tw = 0.55 + 0.45 * sin(TIME * 1.7 + gh * 30.0 + audioHighTime * 2.0 * ar + 4.0 * gBuild * gBuild);
    tw += highP() * ar * 0.6 * step(0.6, gh);
    float gran = (1.0 - smoothstep(0.06, 0.16, length(gf2))) * step(0.45, gh) * tw * granules;
    gran *= fBody * (1.0 - clamp(fNuc * 1.5, 0.0, 1.0)) * (1.0 - fChrom);

    // drop shockwave
    float shock = gDrop * exp(-pow((length(sp) - (1.0 - gDrop) * 1.4) * 14.0, 2.0));
    float sat = 0.75 + 0.25 * gLift + 0.2 * gPeak - 0.15 * gRest;
    float beat = audioBeatPulse * ar;
    float bassM = 1.0 + 0.7 * bassP() * ar;

    // ── Fluorescence: neon stains on black ──────────────────────────────
    vec3 bg0 = backdrop(uv);
    vec3 e = vec3(0.0);
    e += fBody * vec3(0.02, 0.05, 0.09) * (1.0 + 0.4 * fMito);
    e += fMem * vec3(0.15, 0.45, 0.95) * 0.55 * bassM * glow;
    e += fNuc * vec3(0.12, 0.26, 1.0) * 0.8;
    e += fNucl * vec3(0.3, 0.4, 1.0) * 0.25;
    e += fSpin * vec3(0.2, 1.0, 0.35) * 0.75 * glow;
    e += fChrom * vec3(1.0, 0.12, 0.72) * (1.15 + 0.6 * beat) * glow;
    e += fKin * vec3(0.2, 1.0, 1.0) * 1.1;
    e += fCent * vec3(1.0, 0.95, 0.6) * 1.6 * glow;
    e += fMid * vec3(0.5, 1.0, 0.6) * 1.2;
    e += gran * vec3(0.5, 0.75, 1.0) * 0.45;
    e += shock * vec3(0.6, 0.8, 1.0) * 0.8;
    e *= (0.75 + 0.45 * gLift);
    vec3 colF = bg0 + (1.0 - exp(-e * 1.25));
    colF = mix(vec3(lum(colF)), colF, sat);
    float vigF = 1.0 - smoothstep(0.55, 1.15, length(sp * vec2(0.8, 1.0)));
    colF *= mix(1.0, vigF, 0.55);

    // ── Phase contrast: grey field, bright halos, dark structure ────────
    vec3 bgP = vec3(0.47, 0.5, 0.47) + bg0 * 0.5;
    float ph = 0.0;
    ph -= fBody * 0.1 * (1.0 - 0.5 * fMito);
    ph += fHalo * 0.42 * bassM;
    ph -= fMem * 0.12 * fBody;
    ph -= fNuc * 0.12;
    ph -= fNucl * 0.28;
    ph -= fChrom * (0.42 + 0.2 * beat);
    ph += fSpin * 0.06 * fBody;
    ph += fCent * 0.2;
    ph -= fMid * 0.3;
    ph -= gran * 0.35;
    ph += shock * 0.4;
    vec3 colP = bgP + ph * vec3(1.0, 1.02, 0.98) * (0.85 + 0.3 * gLift);
    colP += vec3(0.06, 0.08, 0.04) * fHalo * fMito;
    float vigP = 1.0 - smoothstep(0.35, 1.2, length(sp));
    colP *= mix(0.55, 1.08, vigP);

    // ── Stained slide: H&E absorption on pale glass ─────────────────────
    vec3 bgS = vec3(0.97, 0.93, 0.94) - bg0 * 0.3;
    vec3 eos = vec3(0.12, 0.85, 0.42), hem = vec3(0.78, 0.95, 0.32);
    vec3 ab = vec3(0.0);
    ab += fBody * eos * (0.55 + 0.25 * fMito);
    ab += fMem * fBody * eos * 0.35 * bassM;
    ab += fNuc * hem * 0.85;
    ab += fNucl * hem * 0.9;
    ab += fChrom * hem * (2.2 + 0.6 * beat);
    ab += fSpin * fBody * eos * 0.4;
    ab += fCent * hem * 0.8;
    ab += fMid * hem * 1.2;
    ab += gran * vec3(0.35, 0.7, 0.3) * 0.9;
    ab *= 0.8 + 0.35 * gLift;
    vec3 colS = bgS * exp(-ab * 1.2);
    colS = mix(vec3(lum(colS)), colS, clamp(sat + 0.1, 0.0, 1.3));
    colS += shock * vec3(0.2, 0.05, 0.12);
    float vigS = 1.0 - smoothstep(0.6, 1.3, length(sp));
    colS *= mix(0.8, 1.0, vigS);

    // ── look selection / auto morph (cross-dissolve) ─────────────────────
    float fsel = float(formSel);
    float wF = 0.0, wP = 0.0, wS = 0.0;
    if (fsel > 0.5) {
        wF = fsel < 1.5 ? 1.0 : 0.0; wP = (fsel > 1.5 && fsel < 2.5) ? 1.0 : 0.0; wS = fsel > 2.5 ? 1.0 : 0.0;
    } else {
        float mp = TIME / morphPeriod;
        float idx = mod(floor(mp), 3.0);
        float bl = smoothstep(0.82, 1.0, fract(mp));
        float nx = mod(idx + 1.0, 3.0);
        wF = (idx < 0.5 ? 1.0 - bl : 0.0) + (nx < 0.5 ? bl : 0.0);
        wP = (abs(idx - 1.0) < 0.5 ? 1.0 - bl : 0.0) + (abs(nx - 1.0) < 0.5 ? bl : 0.0);
        wS = (idx > 1.5 ? 1.0 - bl : 0.0) + (nx > 1.5 ? bl : 0.0);
    }
    vec3 col = colF * wF + colP * wP + colS * wS;
    float alpha = clamp(fBody + lum(colF) * wF, 0.0, 1.0);

    col += vec3(1.0) * gDrop * gDrop * 0.12;
    col *= 1.0 + 0.08 * beat;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(col, transparentBg ? alpha : 1.0);
}
