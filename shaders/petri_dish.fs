/*{
  "DESCRIPTION": "Petri Dish — a top-down, time-lapse view into a glass petri dish of agar where living colonies grow, collide and bloom. Glossy domed bacterial colonies swell out with concentric growth rings and wet specular glints in cream, golden yellow, Serratia red, violet, orange and pink; branching fractal colonies push feathery DLA-like fronts out across the gel; a swarming film spreads in pale terraced rings; colonies that meet stop at fine demarcation grooves instead of overlapping. The forms morph each growth cycle: Bacteria Colonies; Mould Bloom, where fuzzy white hyphae radiate from powdery blue-green, olive, black and pink sporulating centres studded with glossy droplets; Antibiotic Assay, a cream bacterial lawn streaked over the agar with white paper disks punching clear inhibition halos that widen as it grows, a few resistant colonies creeping inside; and Bioluminescent, where blue-green colonies glow on black agar with quorum-sensing waves pulsing through them. The glass rim catches a sharp specular highlight and the lid throws a soft window reflection; when the dish is full, a sterilising wipe dissolves across it to fresh agar and growth begins again. Bass swells every colony and speeds the time-lapse, beats seed new colonies at random spots with a bright pop, and the drop fires a shockwave that bursts colonies everywhere at once. At rest the dish is sparse and calm; during a build it tightens, grows faster and starts to tremble; on the drop a burst of new colonies erupts across the agar, and the peak runs dense, bright and fully alive.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Organic", "Science", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Culture", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Bacteria Colonies", "Mould Bloom", "Antibiotic Assay", "Bioluminescent"], "GROUP": "Culture" },
    { "NAME": "morphPeriod", "LABEL": "Growth Cycle (s)", "TYPE": "float", "AUDIO": false, "MIN": 10.0, "MAX": 90.0, "DEFAULT": 28.0, "GROUP": "Culture" },
    { "NAME": "growthRate",  "LABEL": "Growth Speed",     "TYPE": "float", "MIN": 0.4, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Culture" },
    { "NAME": "colonyCount", "LABEL": "Colony Count",     "TYPE": "float", "MIN": 0.2, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Culture" },
    { "NAME": "beatSpawn",   "LABEL": "Beat Spawning",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Culture" },
    { "NAME": "dishSize",    "LABEL": "Dish Size",        "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 0.6, "DEFAULT": 0.42, "GROUP": "Dish" },
    { "NAME": "spinSpeed",   "LABEL": "Dish Rotation",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Dish" },
    { "NAME": "rimShine",    "LABEL": "Glass Shine",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Dish" },
    { "NAME": "gloss",       "LABEL": "Colony Gloss",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Dish" },
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
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.03, 0.04, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
    { "TARGET": "spawnBuf", "PERSISTENT": true, "FLOAT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718
#define NSLOT 16.0

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float h13(vec3 p)  { p = fract(p * 0.1031); p += dot(p, p.zyx + 31.32); return fract((p.x + p.y) * p.z); }
float vn2(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1.0, 0.0)), f.x), mix(h12(i + vec2(0.0, 1.0)), h12(i + vec2(1.0, 1.0)), f.x), f.y);
}
float vn3(vec3 x) {
    vec3 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(h13(i), h13(i + vec3(1.0, 0.0, 0.0)), f.x), mix(h13(i + vec3(0.0, 1.0, 0.0)), h13(i + vec3(1.0, 1.0, 0.0)), f.x), f.y),
               mix(mix(h13(i + vec3(0.0, 0.0, 1.0)), h13(i + vec3(1.0, 0.0, 1.0)), f.x), mix(h13(i + vec3(0.0, 1.0, 1.0)), h13(i + vec3(1.0, 1.0, 1.0)), f.x), f.y), f.z);
}
float ridge(float n) { return 1.0 - abs(2.0 * n - 1.0); }
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

// ── growth clock: one cycle = one dish from fresh agar to full, then a wipe ──
float gP, gCyc, gFp, gA, gForm, gNext, gT;
void clocks() {
    float ar = audioReact;
    gP = morphPeriod;
    float clk = TIME + audioBassTime * 0.35 * ar + audioMidTime * 0.08 * ar + 1.5 * gBuild * gBuild;
    float ph = clk / gP;
    gCyc = floor(ph);
    gFp = fract(ph);
    gA = gFp * gP;
    float fs = float(formSel);
    gForm = fs > 0.5 ? fs - 1.0 : mod(gCyc, 4.0);
    gNext = fs > 0.5 ? fs - 1.0 : mod(gCyc + 1.0, 4.0);
    gT = TIME * 0.8 + audioMidTime * 0.3 * ar + audioHighTime * 0.15 * ar + 2.0 * gBuild * gBuild;
}

// colony types: 0 glossy ring colony, 1 branching front, 2 mould, 3 small pigmented spot, 4 swarming film
float colonyType(float form, float h, float isEvent) {
    if (form < 0.5) return isEvent > 0.5 ? (h < 0.6 ? 3.0 : 0.0) : (h < 0.32 ? 0.0 : h < 0.58 ? 1.0 : 3.0);
    if (form < 1.5) return isEvent > 0.5 ? (h < 0.7 ? 2.0 : 3.0) : (h < 0.66 ? 2.0 : h < 0.82 ? 0.0 : 3.0);
    if (form < 2.5) return 3.0;
    return isEvent > 0.5 ? (h < 0.6 ? 3.0 : 1.0) : (h < 0.45 ? 1.0 : h < 0.78 ? 0.0 : 3.0);
}
float colonyRmax(float typ, float h) {
    if (typ < 0.5) return 0.09 + 0.11 * h;
    if (typ < 1.5) return 0.17 + 0.17 * h;
    if (typ < 2.5) return 0.15 + 0.18 * h;
    if (typ < 3.5) return 0.025 + 0.04 * h;
    return 0.65 + 0.3 * h;
}
float colonyTau(float typ) {
    if (typ < 0.5) return 0.3;
    if (typ < 1.5) return 0.38;
    if (typ < 2.5) return 0.32;
    if (typ < 3.5) return 0.16;
    return 0.5;
}
float colonyR(float typ, float rmax, float age) {
    if (age <= 0.0) return 0.0;
    float r = rmax * (1.0 - exp(-age * growthRate / (0.75 * colonyTau(typ) * gP)));
    r *= 1.0 + (0.07 * bassP() * audioReact + 0.12 * gDrop) * (typ > 3.5 ? 0.4 : 1.0);
    return r;
}
// dendritic (DLA-like) colony: branches split in two at fixed radii, curling and thinning outward
float wrapA(float a) { return a - TAU * floor(a / TAU + 0.5); }
float dendrite(vec2 v, float d, float seed, float px, out float tw) {
    float th = mod(atan(v.y, v.x) + seed * TAU, TAU);
    float lv = d < 0.035 ? 0.0 : d < 0.08 ? 1.0 : d < 0.14 ? 2.0 : d < 0.21 ? 3.0 : 4.0;
    float rlo = lv < 0.5 ? 0.0 : lv < 1.5 ? 0.035 : lv < 2.5 ? 0.08 : lv < 3.5 ? 0.14 : 0.21;
    float rhi = lv < 0.5 ? 0.035 : lv < 1.5 ? 0.08 : lv < 2.5 ? 0.14 : lv < 3.5 ? 0.21 : 0.3;
    float n = 4.0 * pow(2.0, lv);
    float sw = TAU / n;
    float idx = floor(th / sw);
    float pidx = floor(idx * 0.5);
    float oc = (idx + 0.5 + 0.55 * (h12(vec2(idx, lv + seed * 17.0)) - 0.5)) * sw;
    float pc = lv < 0.5 ? oc : (pidx + 0.5 + 0.55 * (h12(vec2(pidx, lv - 1.0 + seed * 17.0)) - 0.5)) * sw * 2.0;
    float tb = mix(pc, oc, smoothstep(rlo, rlo + 0.75 * (rhi - rlo), d));
    tb += (vn2(vec2(d * 30.0, idx * 3.1 + lv * 17.0 + seed * 5.0)) - 0.5) * 0.45 * sw * smoothstep(rlo, rhi, d);
    float arc = d * abs(wrapA(th - tb));
    tw = vn2(vec2(d * 160.0, idx + seed * 9.0));
    float w = 0.0105 * pow(0.76, lv) * (0.75 + 0.5 * tw) + px * 0.6;
    float alive = step(0.1 * lv, h12(vec2(idx * 1.7, lv * 3.3 + seed)) + 0.15) * step(0.1 * max(lv - 1.0, 0.0), h12(vec2(pidx * 1.7, lv * 3.3 - 3.3 + seed)) + 0.15);
    float f = smoothstep(w + px, w - px, arc) * alive;
    // hairy fringe along the branches
    float fz = vn2(vec2(th * 60.0, d * 90.0) + seed * 40.0);
    f = max(f, smoothstep(w * 2.2, w, arc) * smoothstep(0.55, 0.8, fz) * alive);
    f = max(f, smoothstep(0.03, 0.022, d));
    return f;
}
vec3 pigment(float h) {
    if (h < 0.22) return vec3(0.96, 0.9, 0.74);   // cream
    if (h < 0.42) return vec3(1.0, 0.78, 0.12);   // golden yellow
    if (h < 0.58) return vec3(0.84, 0.08, 0.12);  // Serratia red
    if (h < 0.7)  return vec3(0.46, 0.17, 0.66);  // violet
    if (h < 0.82) return vec3(1.0, 0.5, 0.1);     // orange
    if (h < 0.92) return vec3(0.97, 0.96, 0.92);  // white
    return vec3(0.96, 0.52, 0.62);                // pink
}
vec3 agarCol(float f, vec2 q, float L) {
    vec3 a = f < 0.5 ? vec3(0.44, 0.31, 0.14) : f < 1.5 ? vec3(0.5, 0.41, 0.23) : f < 2.5 ? vec3(0.52, 0.38, 0.14) : vec3(0.006, 0.014, 0.028);
    float n = vn2(q * 4.0 + 3.1) * 0.6 + vn2(q * 11.0 + 7.7) * 0.4;
    a *= 0.86 + 0.24 * n;
    a *= 1.1 - 0.3 * L * L;
    a *= 1.0 + 0.35 * smoothstep(0.88, 0.99, L) - 0.5 * smoothstep(0.985, 1.0, L);
    return a;
}

void main() {
    showDynamics();
    clocks();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;

    // ── pass 0: spawn events (beats / drop) in a persistent slot buffer ──
    // layout by normalised coords: bottom half = 16 slot columns (x, y, birth, seed); top half = control
    if (PASSINDEX == 0) {
        vec4 ctrl = texture2D(spawnBuf, vec2(0.5, 0.75));        // counter, armed, cycle id, drop latch
        float slot = floor(clamp(uv.x, 0.0, 0.9999) * NSLOT);
        vec4 me = texture2D(spawnBuf, vec2((slot + 0.5) / NSLOT, 0.25));
        float cycId = mod(gCyc, 512.0);
        bool fresh = FRAMEINDEX < 2 || abs(ctrl.z - cycId) > 0.5;
        if (fresh) { ctrl = vec4(0.0, 1.0, cycId, 0.0); me = vec4(0.0, 0.0, -1.0, 0.0); }
        bool canSpawn = gFp < 0.86 && beatSpawn > 0.01;
        bool beat = canSpawn && ctrl.y > 0.5 && audioBeatPulse * ar > 0.35 && h11(floor(TIME * 3.0) + cycId) < 0.35 + 0.65 * beatSpawn;
        bool burst = canSpawn && gDrop > 0.45 && ctrl.w < 0.5;
        if (burst || (beat && abs(slot - ctrl.x) < 0.5)) {
            vec2 hh = h22(vec2(slot * 7.13 + fract(TIME * 0.731) * 113.0, cycId + TIME * 0.37));
            float an = hh.x * TAU, rr = sqrt(hh.y) * 0.84;
            me = vec4(rr * cos(an), rr * sin(an), gA - (burst ? h11(slot + 3.1) * 0.6 : 0.0), h11(slot * 3.7 + TIME));
        }
        if (beat) ctrl.x = mod(ctrl.x + 1.0, NSLOT);
        if (beat) ctrl.y = 0.0;
        if (audioBeatPulse * ar < 0.2) ctrl.y = 1.0;
        if (burst) ctrl.w = 1.0;
        if (gDrop < 0.15) ctrl.w = 0.0;
        gl_FragColor = uv.y < 0.5 ? me : ctrl;
        return;
    }

    // ── pass 1: the dish ────────────────────────────────────────────────
    vec2 sp = (fc - 0.5 * R) / R.y;
    float trem = smoothstep(0.55, 1.0, gBuild);
    sp += trem * 0.004 * (h22(vec2(floor(TIME * 24.0), 3.0)) - 0.5);
    float DR = dishSize * (1.0 + 0.05 * gPeak - 0.08 * gBuild + 0.04 * gDrop);
    vec2 qs = sp / DR;                       // screen-aligned dish coords (light, rim)
    float L = length(qs);
    float px = 1.0 / (R.y * DR);             // dish units per pixel
    float spin = TIME * 0.012 * spinSpeed + audioMidTime * 0.01 * ar * spinSpeed;
    vec2 q = rot(spin) * qs;                 // culture frame (turns with the dish)
    float form = gForm;
    bool bio = form > 2.5;

    vec3 bg = backdrop(uv);
    // soft drop shadow of the dish on the bench
    float shL = length(qs - vec2(0.035, -0.05));
    bg *= 1.0 - 0.45 * smoothstep(1.25, 0.95, shL);

    vec3 col = bg;
    float alpha = 0.0;
    float inAgar = smoothstep(1.0 + px, 1.0 - px, L);

    if (L < 1.12) {
        vec3 agar = agarCol(form, q, L);
        vec3 cc = agar;
        vec3 light = normalize(vec3(-0.5, 0.6, 0.62));
        vec3 hv = normalize(light + vec3(0.0, 0.0, 1.0));
        float pulse = audioBeatPulse * ar;

        // ── antibiotic assay: lawn + paper disks + inhibition halos ──
        float lawnGrow = smoothstep(0.02 * gP, 0.32 * gP, gA * growthRate);
        if (form > 1.5 && form < 2.5) {
            float zoneMask = 1.0, zoneRim = 0.0, diskM = 0.0, diskShadow = 0.0;
            vec2 diskV = vec2(0.0);
            for (int k = 0; k < 6; k++) {
                float fk = float(k);
                float an = fk * TAU / 5.0 + gCyc * 1.7 + 0.4;
                vec2 dc = fk < 4.5 ? 0.56 * vec2(cos(an), sin(an)) : vec2(0.0);
                float zr = 0.085 + (0.1 + 0.2 * h11(fk * 5.3 + gCyc * 2.1)) * smoothstep(0.04 * gP, 0.5 * gP, gA * growthRate);
                zr *= 1.0 + 0.04 * bassP() * ar + 0.06 * gDrop;
                float dz = length(q - dc);
                zoneMask = min(zoneMask, smoothstep(zr - 0.006, zr + 0.02, dz));
                zoneRim = max(zoneRim, exp(-pow((dz - zr - 0.025) / 0.018, 2.0)));
                float dm = smoothstep(0.085 + px, 0.085 - px, dz);
                if (dm > diskM) { diskM = dm; diskV = q - dc; }
                diskShadow = max(diskShadow, smoothstep(0.11, 0.08, length(q - dc - vec2(0.012, -0.018))));
            }
            // swabbed lawn: streaky, grainy, cream
            vec2 sd = rot(gCyc * 0.9) * q;
            float streak = 0.88 + 0.12 * sin(sd.y * 70.0 + vn2(sd * 6.0) * 4.0);
            float lg = vn2(q * 60.0) * 0.5 + vn2(q * 140.0) * 0.5;
            float lawn = lawnGrow * zoneMask * streak * (0.72 + 0.28 * lg) * (1.0 + 0.25 * zoneRim * lawnGrow);
            vec3 lawnC = vec3(0.92, 0.84, 0.62);
            cc = mix(cc, lawnC * (0.85 + 0.2 * lg), clamp(lawn * 0.82, 0.0, 1.0));
            cc *= 1.0 - 0.35 * diskShadow * (1.0 - diskM);
            // paper disks: fibrous white with a printed code ring
            float fib = vn2(diskV * 260.0) * 0.6 + vn2(diskV * 90.0) * 0.4;
            float dl = length(diskV);
            float code = smoothstep(0.004, 0.0, abs(dl - 0.055)) * step(0.5, fract(atan(diskV.y, diskV.x) * 2.0));
            vec3 paper = vec3(0.95, 0.95, 0.92) * (0.86 + 0.14 * fib) * (1.0 - 0.6 * code);
            paper *= 0.85 + 0.2 * dot(normalize(vec3(diskV * 4.0, 1.0)), light);
            cc = mix(cc, paper, diskM);
        }

        // ── swarming film underneath (type 4, slot 0 only) ──
        float s0 = h11(gCyc * 7.31 + 0.5);
        bool swarm = (form < 0.5 || bio) && h11(s0 * 41.0 + 9.0) < 0.6;
        if (swarm) {
            vec2 c0 = 0.35 * vec2(cos(s0 * TAU), sin(s0 * TAU)) * h11(s0 * 3.3);
            float r0 = colonyR(4.0, colonyRmax(4.0, h11(s0 * 5.7)), gA);
            vec2 v0 = q - c0;
            float d0 = length(v0);
            vec2 dir0 = v0 / max(d0, 1e-4);
            float wob = vn3(vec3(dir0 * 5.0, s0 * 9.0)) - 0.5;
            float u0 = d0 / max(r0 * (1.0 + 0.12 * wob), 1e-4);
            float film = smoothstep(1.0, 0.97, u0);
            float terr = fract(d0 * 8.0 + wob * 0.6 * smoothstep(0.0, 0.25, d0) + s0);
            float tEdge = smoothstep(0.82, 0.97, terr) * smoothstep(1.0, 0.97, terr);
            if (bio) {
                float wave = 0.5 + 0.5 * sin(d0 * 30.0 - gT * 2.0 * (1.0 + 2.0 * gBuild));
                cc += film * (vec3(0.05, 0.35, 0.55) * (0.25 + 0.5 * wave) + vec3(0.1, 0.8, 0.9) * tEdge * 0.4) * (0.5 + 0.6 * gLift);
            } else {
                cc = mix(cc, vec3(0.86, 0.85, 0.78), film * (0.28 + 0.22 * terr + 0.3 * tEdge));
            }
        }

        // ── colonies: owner = nearest front (colonies stop where they meet) ──
        float e1 = 1e3, e2 = 1e3;
        vec2 bc = vec2(0.0);
        float br = 0.0, bt = 0.0, bs = 0.0, ba = 0.0;
        float keepFrac = colonyCount * (1.0 - 0.55 * gRest) * (form > 1.5 && form < 2.5 ? 0.3 : 1.0);
        for (int k = 0; k < 18; k++) {
            float fk = float(k);
            float seed = h11(fk * 13.17 + gCyc * 7.31 + 0.5);
            if (k > 2 && h11(seed * 17.9 + 4.4) > keepFrac) continue;
            float typ = colonyType(form, h11(seed * 29.3 + 1.1), 0.0);
            float an = h11(seed * 91.7 + 1.3) * TAU;
            float rr = sqrt(h11(seed * 53.1 + 2.9)) * 0.8;
            vec2 c = rr * vec2(cos(an), sin(an));
            float birth = (k < 3 ? h11(seed * 7.7) * 0.06 : h11(seed * 7.7) * 0.55) * gP;
            float age = gA - birth;
            float r = colonyR(typ, colonyRmax(typ, h11(seed * 61.3 + 0.7)), age);
            if (r <= 0.0) continue;
            float e = length(q - c) - r * (typ > 1.5 && typ < 2.5 ? 1.15 : 1.0);
            if (e < e1) { e2 = e1; e1 = e; bc = c; br = r; bt = typ; bs = seed; ba = age; }
            else if (e < e2) { e2 = e; }
        }
        for (int k = 0; k < 16; k++) {
            vec4 ev = texture2D(spawnBuf, vec2((float(k) + 0.5) / NSLOT, 0.25));
            if (ev.z < 0.0) continue;
            float age = gA - ev.z;
            float seed = fract(ev.w * 7.13 + float(k) * 0.371);
            float typ = colonyType(form, ev.w, 1.0);
            float r = colonyR(typ, colonyRmax(typ, h11(seed * 61.3)) * (typ < 2.5 ? 0.55 : 1.0), age);
            if (r <= 0.0) continue;
            float e = length(q - ev.xy) - r * (typ > 1.5 && typ < 2.5 ? 1.15 : 1.0);
            if (e < e1) { e2 = e1; e1 = e; bc = ev.xy; br = r; bt = typ; bs = seed; ba = age; }
            else if (e < e2) { e2 = e; }
        }

        float glowE = e1;
        if (br > 0.0 && e1 < 0.09) {
            vec2 v = q - bc;
            float d = length(v);
            vec2 dir = v / max(d, 1e-4);
            vec3 ccol = vec3(0.0);
            float opa = 0.0;
            float spec = 0.0, dif = 1.0;
            float tip = 0.0;
            vec3 pig = pigment(h11(bs * 3.7 + 0.2));
            if (bt < 0.5 || bt > 2.5) {
                // glossy domed colony with growth rings (or a small pigmented spot)
                float rr = br * (1.0 + 0.09 * (vn3(vec3(dir * 4.0, bs * 9.0)) - 0.5) * (bt < 0.5 ? 1.0 : 0.5));
                float u = d / max(rr, 1e-4);
                opa = smoothstep(1.0, 1.0 - 1.5 * px / max(rr, 1e-4), u);
                float sq = sqrt(max(1.0 - u * u, 0.0));
                vec3 n = normalize(vec3(dir * 0.75 * u / max(sq, 0.18), 1.0));
                dif = clamp(dot(n, light), 0.0, 1.0);
                spec = pow(max(dot(n, hv), 0.0), 70.0) * (0.6 + 0.5 * gLift) * gloss;
                float ring = bt < 0.5 ? 0.5 + 0.5 * cos(d * 120.0 + bs * 6.0 + (vn2(v * 30.0) - 0.5) * 2.0) : 0.5;
                float core = smoothstep(0.45, 0.0, u);
                ccol = pig * (0.78 + 0.22 * ring);
                ccol = mix(ccol, pig * pig * 0.75, core * 0.55);
                ccol *= 0.72 + 0.4 * dif;
                opa *= mix(0.55, 1.0, smoothstep(1.0, 0.7, u));
                tip = exp(-pow((u - 1.0) * rr / 0.012, 2.0));
            } else if (bt < 1.5) {
                // branching fractal front: radially stretched ridged noise, dense core, fingering tips
                float u = d / br;
                float tw = 0.0;
                float fill = dendrite(v, d, bs, px, tw);
                float ue = d + 0.05 * (vn2(dir * 6.0 + bs * 19.0) - 0.5) + 0.03 * (tw - 0.5);
                fill *= smoothstep(br, br - 0.012, ue);
                opa = fill * 0.92;
                tip = fill * smoothstep(br - 0.05, br - 0.008, ue);
                float b = 1.0 - smoothstep(0.0, 0.4, u) * 0.3;
                ccol = mix(vec3(0.95, 0.92, 0.82), pig, 0.25) * (0.7 + 0.3 * b);
                ccol += vec3(0.08) * tip;
            } else {
                // mould: fuzzy radiating hyphae, powdery sporulating centre, glossy exudate droplets
                float sh = gT * (0.25 + 0.6 * gBuild);
                float fb = vn3(vec3(dir * 95.0, d * 14.0 + bs * 5.0 - sh));
                float fb2 = vn3(vec3(dir * 36.0, d * 5.0 + bs * 11.0));
                float u = d / br;
                float ue = u - 0.24 * (fb * 0.6 + fb2 * 0.4);
                opa = smoothstep(1.0, 0.72, ue);
                float hs = h11(bs * 8.3 + 0.9);
                vec3 spore = hs < 0.4 ? vec3(0.2, 0.47, 0.42) : hs < 0.62 ? vec3(0.45, 0.5, 0.2) : hs < 0.82 ? vec3(0.07, 0.07, 0.065) : vec3(0.88, 0.42, 0.58);
                float zone = smoothstep(0.78, 0.45, u + 0.1 * (fb2 - 0.5));
                float powder = vn2(v * 140.0) * 0.5 + vn2(v * 55.0) * 0.5;
                float rings = 0.5 + 0.5 * sin(d * 75.0 + fb2 * 3.0);
                vec3 white = vec3(0.93, 0.94, 0.9) * (0.82 + 0.18 * fb);
                ccol = mix(white, spore * (0.75 + 0.45 * powder) * (0.85 + 0.25 * rings), zone);
                // exudate droplets on the sporulating zone
                vec2 g = v * 55.0;
                vec2 gi = floor(g);
                vec2 gpos = h22(gi + bs * 17.0);
                float dd = length(fract(g) - 0.2 - 0.6 * gpos);
                float drop = smoothstep(0.16, 0.08, dd) * step(0.7, h12(gi + 3.0 + bs)) * zone;
                spec = drop * smoothstep(0.1, 0.0, length(fract(g) - 0.2 - 0.6 * gpos - vec2(-0.04, 0.04))) * gloss;
                ccol = mix(ccol, mix(ccol, vec3(1.0, 0.92, 0.55), 0.4), drop * 0.6);
                opa *= 0.97;
            }
            // merge grooves where two colonies meet
            float groove = (1.0 - smoothstep(0.0, 0.014, e2 - e1)) * step(e2, 0.004);
            // newborn pop (beat spawns)
            float born = exp(-ba * 1.6);
            if (bio) {
                vec3 bc1 = mix(vec3(0.1, 1.0, 0.72), vec3(0.12, 0.5, 1.0), h11(bs * 4.1));
                float wave = 0.5 + 0.5 * sin(d * 48.0 - gT * 2.6 * (1.0 + 1.5 * gBuild + 0.6 * gPeak) + bs * 6.0);
                float inten = (0.3 + 0.55 * wave + 0.6 * tip) * (0.55 + 0.7 * gLift) * (1.0 + 0.35 * pulse);
                vec3 em = bc1 * inten * opa;
                em += vec3(0.7, 1.0, 1.0) * spec * 0.25;
                em *= 1.0 - 0.8 * groove;
                cc += em + bc1 * born * 1.2 * opa;
            } else {
                ccol *= 1.0 - 0.55 * groove;
                ccol += vec3(1.0, 0.97, 0.9) * tip * 0.1 * pulse;
                cc = mix(cc, ccol, clamp(opa, 0.0, 1.0));
                cc += vec3(1.0, 0.98, 0.92) * spec * 0.85 * opa;
                cc += vec3(1.0, 0.95, 0.8) * born * 0.6 * opa;
                // faint shadow just outside the colony edge
                cc *= 1.0 - 0.25 * smoothstep(0.03, 0.0, e1) * step(0.0, e1) * (1.0 - opa);
            }
        }
        if (bio) {
            float halo = exp(-max(glowE, 0.0) * 28.0) * step(glowE, 0.5);
            cc += vec3(0.05, 0.45, 0.5) * halo * (0.12 + 0.18 * gLift + 0.2 * pulse);
        }

        // ── drop shockwave across the agar ──
        float sw = (1.0 - gDrop) * 1.25;
        float wave = exp(-pow((length(q) - sw) * 10.0, 2.0)) * gDrop;
        cc += (bio ? vec3(0.2, 1.0, 0.85) : vec3(1.0, 0.95, 0.85)) * wave * 0.55;

        // ── sterilising wipe → fresh agar of the next culture ──
        float wp = smoothstep(0.86, 0.985, gFp);
        if (wp > 0.0) {
            vec2 wd = vec2(cos(gCyc * 2.4 + 0.7), sin(gCyc * 2.4 + 0.7));
            float nf = 0.62 * (dot(qs, wd) * 0.5 + 0.5) + 0.38 * (vn2(qs * 3.0 + gCyc) * 0.65 + vn2(qs * 9.0) * 0.35) + 0.07 * (vn2(qs * 45.0) - 0.5);
            float edge = wp * 1.3 - 0.15;
            float clean = smoothstep(edge + 0.02, edge - 0.02, nf);
            float front = exp(-pow((nf - edge) / 0.03, 2.0)) * smoothstep(0.0, 0.08, wp) * smoothstep(1.0, 0.9, wp);
            cc = mix(cc, agarCol(gNext, q, L), clean);
            cc += mix(vec3(0.85, 0.9, 1.0), vec3(0.3, 1.0, 0.9), gNext > 2.5 ? 1.0 : 0.0) * front * 0.75;
        }
        // fresh-dish fade in
        cc = mix(agarCol(form, q, L), cc, smoothstep(0.0, 0.02, gFp));

        // lid reflections: broad soft band + a window glint
        float band = smoothstep(0.32, 0.0, abs(dot(qs, normalize(vec2(1.0, -1.0))) + 0.25)) * smoothstep(1.0, 0.5, L);
        float win = smoothstep(0.2, 0.0, length((qs - vec2(-0.5, 0.52)) * vec2(1.0, 1.8)));
        cc += vec3(0.9, 0.95, 1.0) * (band * 0.05 + win * 0.07) * rimShine * (bio ? 0.4 : 1.0);

        col = mix(col, cc, inAgar);

        // ── glass rim ──
        float rimM = smoothstep(1.0 - px, 1.0 + px, L) * smoothstep(1.085 + px, 1.085 - px, L);
        float ang = atan(qs.y, qs.x);
        float hiA = pow(max(cos(ang - 2.25), 0.0), 14.0);
        float hiB = pow(max(cos(ang + 0.85), 0.0), 26.0) * 0.45;
        float lines = exp(-pow((L - 1.004) / 0.005, 2.0)) * 0.55 + exp(-pow((L - 1.04) / 0.006, 2.0)) * 0.3 + exp(-pow((L - 1.08) / 0.005, 2.0)) * 0.5;
        vec3 glassTint = bio ? vec3(0.3, 0.85, 0.9) : vec3(0.85, 0.92, 1.0);
        vec3 rim = bg * 0.6 + glassTint * (0.05 + lines * (0.35 + 0.9 * hiA + 0.4 * hiB));
        rim += vec3(1.0) * (hiA * 0.6 + hiB * 0.35) * smoothstep(1.0, 1.03, L) * smoothstep(1.085, 1.05, L);
        rim *= rimShine * (0.8 + 0.25 * gLift) + (1.0 - rimShine) * 0.3;
        col = mix(col, rim, rimM);
        alpha = max(inAgar, rimM);
    }

    // peak: richer colour; rest: softer
    col = mix(vec3(lum(col)), col, 1.0 + 0.25 * gPeak - 0.15 * gRest);
    col += vec3(1.0) * gDrop * gDrop * 0.08;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(alpha + lum(col), 0.0, 1.0) : 1.0);
}
