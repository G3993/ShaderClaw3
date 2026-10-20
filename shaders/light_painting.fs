/*{
  "DESCRIPTION": "Light Painting — long-exposure photography as physics. Glowing LED wands swing on damped-pendulum harmonograph paths through 3D space while the camera's shutter stays open: every frame the light is integrated into a persistent sheet of film, so the wands paint thick luminous ribbons — gel gradients across their width, fine LED-pixel streaks along their length, a white-hot core where the hand holds the light — that fold over themselves and flare brightest wherever a ribbon turns edge-on to the lens. The pendulums lose energy, so each plate spirals inward into a tighter figure before the next plate begins with new ratios and new gels while the old exposure slowly fades. Steel-wool sparks burst from the wand, fly ballistically under drag and gravity and cool from white through amber to red. The film itself responds like film: a Hurter–Driffield curve rolls the highlights into white, red halation glows around the hottest strokes, grain lives in the mid-tones. Bass pushes more light into the exposure, beats throw spark bursts and flare the cores, mids widen the ribbons, highs sparkle the LEDs.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #10",
  "CATEGORIES": ["Generator", "Abstract", "Light", "Simulation", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Pendulum Speed",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "plateLength", "LABEL": "Plate Length (s)", "TYPE": "float", "AUDIO": false, "MIN": 8.0, "MAX": 60.0, "DEFAULT": 26.0, "GROUP": "Speed" },
    { "NAME": "trailTime",   "LABEL": "Exposure Memory (s)", "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 20.0, "DEFAULT": 4.5, "GROUP": "Speed" },
    { "NAME": "complexity",  "LABEL": "Figure Complexity","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.35, "GROUP": "Harmonograph" },
    { "NAME": "damping",     "LABEL": "Pendulum Damping", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Harmonograph" },
    { "NAME": "emitters",    "LABEL": "Light Wands (3rd joins dense plates)", "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 3.0,  "DEFAULT": 2.0,  "GROUP": "Harmonograph" },
    { "NAME": "tubeLength",  "LABEL": "Ribbon Width",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 0.4,  "DEFAULT": 0.11, "GROUP": "Harmonograph" },
    { "NAME": "twist",       "LABEL": "Ribbon Twist",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.1,  "GROUP": "Harmonograph" },
    { "NAME": "depth3D",     "LABEL": "Depth",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Harmonograph" },
    { "NAME": "sway",        "LABEL": "3D Sway",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Harmonograph" },
    { "NAME": "ledStripes",  "LABEL": "LED Pixels",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 60.0, "DEFAULT": 26.0, "GROUP": "Harmonograph" },
    { "NAME": "sparks",      "LABEL": "Steel Wool Sparks","TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.55, "GROUP": "Sparks" },
    { "NAME": "sparkRate",   "LABEL": "Bursts per 10 s",  "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 5.0, "DEFAULT": 2.5, "GROUP": "Sparks" },
    { "NAME": "gravity",     "LABEL": "Spark Gravity",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Sparks" },
    { "NAME": "gel1",        "LABEL": "Gel 1",            "TYPE": "color", "DEFAULT": [1.0, 0.06, 0.45, 1.0], "GROUP": "Color" },
    { "NAME": "gel2",        "LABEL": "Gel 2",            "TYPE": "color", "DEFAULT": [0.0, 0.62, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "gel3",        "LABEL": "Gel 3",            "TYPE": "color", "DEFAULT": [1.0, 0.45, 0.02, 1.0], "GROUP": "Color" },
    { "NAME": "gel4",        "LABEL": "Gel 4",            "TYPE": "color", "DEFAULT": [0.42, 0.08, 1.0, 1.0], "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "exposure",    "LABEL": "Exposure",         "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Film" },
    { "NAME": "filmContrast","LABEL": "Film Contrast",    "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Film" },
    { "NAME": "halation",    "LABEL": "Halation",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.7,  "GROUP": "Film" },
    { "NAME": "bloom",       "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Film" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.2,  "GROUP": "Film" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Film" },
    { "NAME": "filmRes",     "LABEL": "Film Resolution",  "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Film" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG", "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "film",  "PERSISTENT": true, "WIDTH": "floor($WIDTH*$filmRes)", "HEIGHT": "floor($HEIGHT*$filmRes)" },
    { "TARGET": "halfB", "WIDTH": "$WIDTH/2", "HEIGHT": "$HEIGHT/2" },
    { "TARGET": "quarB", "WIDTH": "$WIDTH/4", "HEIGHT": "$HEIGHT/4" },
    { "TARGET": "blurB", "WIDTH": "$WIDTH/4", "HEIGHT": "$HEIGHT/4" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Light Painting — CURVEBALL pack #10
//   pass 0  film  : persistent HDR exposure. Each frame the film decays a
//                   little (exposure memory) and integrates the light that
//                   fell on it during the frame:
//                   · LED wands = line lights swept along 3D harmonograph
//                     paths. Every sub-step deposits an oblique, motion-
//                     stretched gaussian (σ ∝ step length) so consecutive
//                     steps and frames sum seamlessly; the exposure is
//                     energy-conserving (∝ 1 / perpendicular speed), so
//                     ribbons flare where they turn edge-on.
//                   · the wand's hand-held core = a point light.
//                   · steel-wool sparks = ballistic points with drag and
//                     gravity, cooling blackbody colour.
//                   texel (0,0) = state (beat-burst age, seed)
//   pass 1-3       : exact 2×2 box downsamples + spiral blur (bloom/halation)
//   pass 4  final  : Hurter–Driffield film curve, halation, grain, finisher
// ─────────────────────────────────────────────────────────────────────────

#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h21(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }

float bassP() { return smoothstep(0.04, 0.8, audioBass); }
float midP()  { return smoothstep(0.05, 0.8, audioMid); }
float highP() { return smoothstep(0.08, 0.85, audioHigh); }
float levP()  { return clamp(audioLevel, 0.0, 1.0); }
float beatP() { return clamp(audioBeatPulse, 0.0, 1.0); }

vec3 gelPick(float k) {
    k = mod(floor(k + 0.5), 4.0);
    if (k < 0.5) return gel1.rgb;
    if (k < 1.5) return gel2.rgb;
    if (k < 2.5) return gel3.rgb;
    return gel4.rgb;
}

// each plate gets its own gel scheme: warm, cool, complementary, split
void plateGels(float pl, float e, out vec3 cA, out vec3 cB) {
    float sc = floor(h11(pl * 7.31 + 0.9) * 4.0);
    vec3 g0, g1, g2;
    if (sc < 0.5)      { g0 = gel1.rgb; g1 = gel3.rgb; g2 = mix(gel1.rgb, gel3.rgb, 0.5); }   // warm
    else if (sc < 1.5) { g0 = gel2.rgb; g1 = gel4.rgb; g2 = mix(gel2.rgb, vec3(1.0), 0.25); }  // cool
    else if (sc < 2.5) { g0 = gel3.rgb; g1 = gel2.rgb; g2 = gel4.rgb; }                        // complementary
    else               { g0 = gel4.rgb; g1 = gel1.rgb; g2 = gel2.rgb; }                        // split
    if (e < 0.5)      { cA = g0; cB = g1; }
    else if (e < 1.5) { cA = g1; cB = g2; }
    else              { cA = g2; cB = g0; }
}

// ── shared ───────────────────────────────────────────────────────────────
vec2  g_c;      // film centre (px)
float g_S;      // px per world unit (= film height)
float g_L;      // plate length (pendulum time)
float g_asp;

// state of wand e at pendulum time t: position P, tube direction D, envelope
void wand(float t, float e, out vec3 P, out vec3 D, out float env) {
    float pl = floor(t / g_L);
    float tp = t - pl * g_L;
    float s  = pl * 13.17 + 3.1;                 // plate seed, shared → a nested family
    float nmax = 2.0 + floor(complexity * 3.0);
    float n1 = 1.0 + floor(h11(s + 1.0) * nmax);
    float n2 = 1.0 + floor(h11(s + 2.0) * nmax);
    float n3 = 1.0 + floor(h11(s + 3.0) * nmax);
    float n4 = 1.0 + floor(h11(s + 4.0) * nmax);
    float n5 = 1.0 + floor(h11(s + 5.0) * 2.0);
    vec4 dt4 = (vec4(h11(s + 6.0), h11(s + 7.0), h11(s + 8.0), h11(s + 9.0)) - 0.5) * 0.14;
    float w0 = 1.45;
    float dec = exp(-damping * 0.7 * tp / g_L);
    float ep = e * 0.95 + h11(s + 10.0) * TAU;
    float em = 1.0 - e * 0.16;
    float x = 0.60 * sin(w0 * (n1 + dt4.x) * tp + ep)               + 0.36 * sin(w0 * (n2 + dt4.y) * tp + 1.3 * ep + h11(s + 11.0) * TAU);
    float y = 0.56 * sin(w0 * (n3 + dt4.z) * tp + 0.7 * ep + h11(s + 12.0) * TAU) + 0.32 * sin(w0 * (n4 + dt4.w) * tp + 1.9 * ep);
    float z = 0.80 * sin(w0 * (n5 + 0.013) * tp + 0.6 * ep + h11(s + 13.0) * TAU);
    P = vec3(x * 0.5 * g_asp * 1.05, y * 0.58, z * 0.32 * depth3D) * (dec * em);

    float a = twist * (1.5 * tp + 0.9 * sin(w0 * n1 * 0.5 * tp + ep)) + h11(s + 14.0) * TAU + e * 2.1;
    float b = 0.8 * sin(0.47 * tp + e * 1.7 + h11(s + 16.0) * TAU);
    D = vec3(cos(a) * cos(b), sin(a) * cos(b), sin(b)) * sqrt(dec);   // tube length follows the swing

    float yaw = 0.55 * sway * sin(0.21 * tp + h11(s + 15.0) * TAU);
    float cy = cos(yaw), sy = sin(yaw);
    P.xz = vec2(cy * P.x - sy * P.z, sy * P.x + cy * P.z);
    D.xz = vec2(cy * D.x - sy * D.z, sy * D.x + cy * D.z);
    float pit = 0.25 * sway * sin(0.17 * tp + 2.0 + s);
    float cp = cos(pit), sp = sin(pit);
    P.yz = vec2(cp * P.y - sp * P.z, sp * P.y + cp * P.z);
    D.yz = vec2(cp * D.y - sp * D.z, sp * D.y + cp * D.z);

    env = smoothstep(0.0, 0.9, tp) * smoothstep(g_L, g_L - 1.4, tp);
}

// near a pendulum reversal the wand almost stops and sweeps back over itself: shorten the tube there
float revScale(float t, float e, vec3 P) {
    vec3 Pn, Dn; float en;
    wand(t + 0.04, e, Pn, Dn, en);
    float v = length(Pn - P) / 0.04;
    return mix(0.35, 1.0, smoothstep(0.06, 0.32, v));
}

// perspective projection → (px.xy, scale)
vec3 proj(vec3 P) {
    float k = 2.6 / (2.6 + P.z);
    return vec3(g_c + P.xy * k * g_S, k);
}

// one swept sub-step of a line light (tube) from ends (a0,b0) to (a1,b1)
vec3 tubeStep(vec2 p, vec3 a0, vec3 b0, vec3 a1, vec3 b1, float trueLen,
              vec3 cA, vec3 cB, float w, float stripes, float sparkle) {
    vec2 A = 0.5 * (a0.xy + a1.xy), B = 0.5 * (b0.xy + b1.xy);
    vec2 VA = a1.xy - a0.xy, VB = b1.xy - b0.xy;
    vec2 T = B - A;
    float L = length(T);
    if (L < 0.75) return vec3(0.0);
    vec2 N = vec2(-T.y, T.x) / L;
    float minp = 0.6 * w;
    // first solve with the centre velocity, then refine with the local one
    vec2 V = 0.5 * (VA + VB);
    float vp = dot(V, N);
    if (abs(vp) < minp) V += N * ((vp < 0.0 ? -minp : minp) - vp);
    vec2 q = p - A;
    float det = T.x * V.y - T.y * V.x;
    float s = (q.x * V.y - q.y * V.x) / det;
    V = mix(VA, VB, clamp(s, 0.0, 1.0));
    vp = dot(V, N);
    if (abs(vp) < minp) V += N * ((vp < 0.0 ? -minp : minp) - vp);
    det = T.x * V.y - T.y * V.x;
    s = (q.x * V.y - q.y * V.x) / det;
    float tau = (T.x * q.y - T.y * q.x) / det;
    float vl = length(V);
    float sm = max(0.6 * vl, w);
    float dm = tau * vl;
    float g = exp(-0.5 * dm * dm / (sm * sm));
    if (g < 1e-4) return vec3(0.0);
    float sinT = abs(det) / (L * vl);
    float aaS = w / (L * max(sinT, 0.15));
    // sliding along its own axis paints flat CG-looking sheets: collapse the ribbon to its core line
    float narrow = max(smoothstep(0.10, 0.50, sinT), 1.5 * w / L);
    float s0 = 0.5 - 0.5 * narrow, s1 = 0.5 + 0.5 * narrow;
    float edge = smoothstep(s0 - aaS, s0 + aaS, s) * smoothstep(s1 + aaS, s1 - aaS, s);
    if (edge <= 0.0) return vec3(0.0);
    float sc = clamp((s - s0) / max(s1 - s0, 1e-3), 0.0, 1.0);
    float fold = min(1.0 / max(sinT, 0.4), 2.5) * min(1.0 / narrow, 2.0);       // edge-on sweep concentrates the light
    float fore = clamp(trueLen / L, 1.0, 2.5);           // foreshortened tube: same power, fewer px
    float led = 1.0;
    if (stripes > 0.5) {
        float lp = 0.5 + 0.5 * cos(TAU * sc * stripes);
        float lid = floor(sc * stripes + 0.5);
        led = 0.30 + 0.70 * lp * lp * (0.75 + 0.25 * h11(lid * 7.3 + sparkle));
    }
    float rim = 1.0 + 0.6 * (1.0 - smoothstep(0.0, 0.12, min(sc, 1.0 - sc)));  // brighter tube ends = crisp ribbon borders
    vec3 col = mix(cA, cB, smoothstep(0.08, 0.92, sc));
    return col * (led * rim * fold * fore * edge * g / (sm * 2.5066));
}

// point light moved by one step (C0 → C1); normalised 2D gaussian (energy conserving)
float pointStep(vec2 p, vec2 C0, vec2 C1, float w) {
    vec2 C = 0.5 * (C0 + C1);
    vec2 V = C1 - C0;
    float vl = length(V);
    vec2 m = vl > 1e-4 ? V / vl : vec2(1.0, 0.0);
    vec2 q = p - C;
    float dm = dot(q, m), dp = dot(q, vec2(-m.y, m.x));
    float sm = max(0.6 * vl, w);
    return exp(-0.5 * (dm * dm / (sm * sm) + dp * dp / (w * w))) / (6.2832 * sm * w);
}

vec3 blackbody(float h) {   // h 0 cold → 1 white hot
    h = clamp(h, 0.0, 1.0);
    return vec3(1.0, 0.18 + 0.75 * h * h, 0.03 + 0.85 * pow(h, 3.0)) * (0.25 + 1.6 * h * h);
}

// one steel-wool burst: born at real time tb, seed sd; deposits the frame step
vec3 burst(vec2 p, float tb, float sd, float age, float dt, float amt, float tScale, float tOff) {
    if (age < 0.0 || age > 2.0) return vec3(0.0);
    float G = 0.55 * gravity, k = 1.9;
    float bound = ((1.25 / k) * (1.0 - exp(-k * age)) + 0.5 * G * age * age / (1.0 + k * age * 0.3) + 0.04) * g_S * 1.35;
    vec3 P, D; float env;
    wand(tb * tScale + tOff, 0.0, P, D, env);
    vec3 o = proj(P);
    if (length(p - o.xy) > bound + 6.0) return vec3(0.0);
    vec3 acc = vec3(0.0);
    float count = 64.0 * amt;
    vec3 gv = vec3(0.0, -G, 0.0);
    for (int i = 0; i < 64; i++) {
        float fi = float(i);
        if (fi >= count) break;
        float r1 = h11(sd * 91.7 + fi * 1.618), r2 = h11(sd * 37.1 + fi * 2.718);
        float r3 = h11(sd * 13.3 + fi * 3.141), r4 = h11(sd * 7.93 + fi * 0.577);
        float life = 0.35 + 1.25 * r4 * r4;
        if (age > life) continue;
        float ct = 2.0 * r1 - 1.0, ph = TAU * r2, st = sqrt(max(1.0 - ct * ct, 0.0));
        vec3 v0 = vec3(st * cos(ph), ct * 0.85 + 0.15, st * sin(ph)) * (0.18 + 1.05 * pow(r3, 1.6));
        float a0 = max(age - dt, 0.0);
        vec3 x0 = P + (v0 - gv / k) * (1.0 - exp(-k * a0)) / k + gv * a0 / k;
        vec3 x1 = P + (v0 - gv / k) * (1.0 - exp(-k * age)) / k + gv * age / k;
        vec3 p0 = proj(x0), p1 = proj(x1);
        float heat = 1.0 - age / life;
        float flick = 0.75 + 0.5 * h11(fi * 3.7 + floor(age * 40.0) + sd * 11.0);
        acc += blackbody(heat) * flick * pointStep(p, p0.xy, p1.xy, 0.85);
    }
    return acc;
}

// spark exposure → colour: dense fresh streaks white-hot, thin cooling tails amber/red
vec3 sparkCol(float a) {
    return a * mix(vec3(1.0, 0.22, 0.03), vec3(1.0, 0.75, 0.45), smoothstep(0.2, 2.5, a));
}

void main() {
    vec2 fc = gl_FragCoord.xy;
    float ar = audioReact;

    // ═════════════════════════════ FILM ═════════════════════════════════
    if (PASSINDEX == 0) {
        vec2 R = RENDERSIZE.xy;
        float dt = clamp(TIMEDELTA, 0.0, 0.1);
        bool init = FRAMEINDEX < 2;
        vec4 st = texture2D(film, vec2(0.5) / R);
        if (fc.x < 1.0 && fc.y < 1.0) {
            float age = init ? 9.0 : st.r + dt;
            float sd  = init ? 0.37 : st.g;
            if (!init && beatP() > 0.55 && age > 1.4 && ar > 0.01 && sparks > 0.001) { age = 0.0; sd = fract(sd + 0.6180339); }
            gl_FragColor = vec4(min(age, 9.0), sd, 0.0, 1.0);
            return;
        }
        g_c = 0.5 * R; g_S = R.y; g_asp = R.x / R.y;
        g_L = plateLength;

        vec4 F0 = init ? vec4(0.0) : texture2D(film, fc / R);
        vec3 E = F0.rgb;
        float tau = max(trailTime, 0.3);
        float tpNow = mod(TIME * speed + plateLength * 0.55, plateLength) / plateLength;
        E *= exp(-dt / (tau * mix(1.0, 0.6, smoothstep(0.45, 1.0, tpNow))));
        float SP = F0.a * exp(-dt / 1.1);     // sparks: their own short-lived channel

        float tOff = plateLength * 0.55;
        float T = TIME * speed + tOff;
        float spd = max(speed, 0.001);
        // normal frame: this frame's interval in 6 sub-steps; first frame: pre-expose the last few seconds
        float span = init ? min(tau * 1.2, 7.0) : dt;
        float nsub = init ? 150.0 : 6.0;
        float T0 = T - span * speed;
        float dts = span / nsub;                    // real seconds per sub-step
        float I0 = 0.36 * R.y * (1.0 + 0.35 * bassP() * ar);   // tube power per px of tube
        float Ic = 2.6 * R.y * (1.0 + 0.6 * beatP() * ar);     // hand-held core
        float wpx = max(0.85, 0.9 * R.y / 1080.0);
        float hlBase = tubeLength * (1.0 + 0.3 * midP() * ar);
        float sparkleSeed = floor(TIME * 12.0) * highP() * ar;

        for (int ei = 0; ei < 3; ei++) {
            float e = float(ei);
            float plNow = floor(T / g_L);
            bool densePlate = h11(plNow * 5.7 + 2.0) > 0.6;
            float nW = floor(emitters + 0.5);
            if (ei == 2 && nW < 2.5 && !(nW > 1.5 && densePlate)) break;
            if (ei < 2 && e > nW - 0.5) break;
            float hl = hlBase * (ei == 0 ? 1.0 : (ei == 1 ? 0.62 : 0.4));
            float pw = (ei == 0 ? 1.0 : (ei == 1 ? 0.8 : 0.65));

            vec3 P0, D0; float env0;
            wand(T, e, P0, D0, env0);
            vec3 pc = proj(P0);
            float bound = hl * g_S * 1.25 + 2.6 * span * speed * g_S + 12.0;
            if (!init && length(fc - pc.xy) > bound) continue;

            float pl = floor(T0 / g_L);
            float sA = pl * 13.17 + e * 5.0;
            vec3 cA, cB;
            plateGels(pl, e, cA, cB);

            float tPrev = T0;
            wand(tPrev, e, P0, D0, env0);
            float hr0 = hl * revScale(tPrev, e, P0);
            vec3 a0 = proj(P0 - D0 * hr0), b0 = proj(P0 + D0 * hr0), c0 = proj(P0);
            for (int k = 1; k <= 150; k++) {
                float fk = float(k);
                if (fk > nsub) break;
                float tk = T0 + (T - T0) * fk / nsub;
                vec3 P1, D1; float env1;
                wand(tk, e, P1, D1, env1);
                float vPend = length(P1 - P0) * nsub / max(T - T0, 1e-5);   // path speed from the step just taken
                float hr1 = hl * mix(0.35, 1.0, smoothstep(0.06, 0.32, vPend));
                vec3 a1 = proj(P1 - D1 * hr1), b1 = proj(P1 + D1 * hr1), c1 = proj(P1);
                if (floor(tk / g_L) == floor(tPrev / g_L)) {
                    float pl2 = floor(tk / g_L);
                    if (pl2 != pl) {          // plate changed inside the pre-exposure window
                        pl = pl2; sA = pl * 13.17 + e * 5.0;
                        plateGels(pl, e, cA, cB);
                    }
                    float env = 0.5 * (env0 + env1);
                    float ageW = exp(-(T - tk) / (tau * spd));
                    float kz = 0.5 * (a0.z + b1.z);
                    float dof = 1.0 + 1.6 * abs(1.0 - kz) * depth3D;
                    float w = wpx * dof;
                    float trueLen = 2.0 * hr1 * length(D1) * g_S * kz;
                    float atm = 1.0 / (1.0 + 0.5 * max(1.0 - kz, 0.0));
                    vec3 dep = tubeStep(fc, a0, b0, a1, b1, trueLen, cA, cB, w, ledStripes, sparkleSeed + e) * (I0 * pw);
                    vec3 coreC = mix(vec3(1.0), 0.5 * (cA + cB), 0.3);
                    dep += coreC * (Ic * pw * pointStep(fc, c0.xy, c1.xy, w * 0.9));
                    E += dep * (dts * env * ageW * atm);
                }
                a0 = a1; b0 = b1; c0 = c1; env0 = env1; tPrev = tk; P0 = P1;
            }
        }

        // ── steel wool ──
        if (sparks > 0.001) {
            float amt = sparks;
            float sl = 1.8;
            float pB = clamp(sparkRate * 0.18, 0.0, 1.0);
            float jc = floor(TIME / sl);
            for (int j = 0; j < 2; j++) {
                float jj = jc - float(j);
                if (h11(jj * 3.77 + 1.3) < pB) {
                    float tb = (jj + 0.15 + 0.5 * h11(jj * 5.13)) * sl;
                    SP += dot(burst(fc, tb, fract(jj * 0.6180339 + 0.21), TIME - tb, dt, amt, speed, tOff), vec3(0.4, 0.4, 0.2)) * (5.5 * R.y * dt);
                }
            }
            float bAge = init ? 9.0 : st.r;
            if (bAge < 2.0) {
                float tb = TIME - bAge;
                SP += dot(burst(fc, tb, st.g + 0.5, bAge, dt, min(amt * 1.3, 1.0), speed, tOff), vec3(0.4, 0.4, 0.2)) * (5.5 * R.y * dt * (0.6 + 0.6 * ar));
            }
        }

        gl_FragColor = vec4(min(E, vec3(6.0e4)), min(SP, 6.0e4));
        return;
    }

    // ═════════════════════ DOWNSAMPLE / BLUR ═══════════════════════════
    vec2 uv = fc / RENDERSIZE.xy;
    if (PASSINDEX == 1) {
        vec4 f = texture2D(film, uv);
        vec3 c = f.rgb + sparkCol(f.a);
        if (uv.x < 0.004 && uv.y < 0.004) c = vec3(0.0);
        gl_FragColor = vec4(c, 1.0);
        return;
    }
    if (PASSINDEX == 2) {
        gl_FragColor = vec4(texture2D(halfB, uv).rgb, 1.0);
        return;
    }
    if (PASSINDEX == 3) {
        vec2 px = 1.0 / RENDERSIZE.xy;
        vec3 acc = vec3(0.0); float wsum = 0.0;
        float rad = 0.010 * RENDERSIZE.y;
        for (int i = 0; i < 24; i++) {
            float fi = float(i) + 0.5;
            float r = sqrt(fi / 24.0) * rad;
            float a = fi * 2.39996;
            float wt = exp(-2.0 * fi / 24.0);
            acc += texture2D(quarB, uv + vec2(cos(a), sin(a)) * r * px).rgb * wt;
            wsum += wt;
        }
        acc += texture2D(quarB, uv).rgb * 1.5; wsum += 1.5;
        gl_FragColor = vec4(acc / wsum, 1.0);
        return;
    }

    // ═════════════════════════════ FINAL ════════════════════════════════
    vec2 fromC = uv - 0.5;
    float ab = aberration * 0.004 * length(fromC);
    vec4 fC = texture2D(film, uv);
    vec3 E = vec3(texture2D(film, uv + fromC * ab).r, fC.g, texture2D(film, uv - fromC * ab).b) + sparkCol(fC.a);
    if (uv.x < 0.004 && uv.y < 0.004) E = vec3(0.0);
    vec3 bl = texture2D(blurB, uv).rgb;
    vec3 wide = vec3(0.0);
    float asp = RENDERSIZE.x / RENDERSIZE.y;
    for (int i = 0; i < 8; i++) {
        float a = float(i) * 0.785398 + 0.3;
        wide += texture2D(blurB, uv + vec2(cos(a) / asp, sin(a)) * 0.022).rgb;
    }
    wide *= 0.125;

    float lift = 1.0 + (0.45 * bassP() + 0.2 * levP() + 0.15 * beatP()) * ar;
    E = E * exposure * lift;
    E += 0.06 * dot(E, vec3(0.3, 0.5, 0.2));                         // dye-layer crosstalk: hot cores bleach to white
    E += bl * (bloom * 0.35 * exposure * lift);
    float hl = dot(bl * 0.6 + wide, vec3(0.3, 0.5, 0.2));
    E += vec3(1.0, 0.26, 0.08) * hl * halation * (0.35 + 0.25 * levP() * ar) * exposure;   // red halation from the film base

    // Hurter–Driffield: density is a sigmoid in log exposure (toe, straight line, shoulder)
    vec3 x = log2(E * 16.0 + 0.0005);
    float k = 0.85 * filmContrast;
    vec3 col = 1.0 / (1.0 + exp(-k * (x - 2.4)));
    col = max(col - 0.002, 0.0) + vec3(0.003, 0.004, 0.007);          // film base

    // saturation / hue
    float l = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(l), col, saturation);
    if (hueShift > 0.001) {
        float a = hueShift * TAU, s = sin(a), c = cos(a);
        vec3 kk = vec3(0.57735);
        col = col * c + cross(kk, col) * s + kk * dot(kk, col) * (1.0 - c);
    }

    // grain lives in the mid-tones
    float gr = h21(fc + vec2(float(FRAMEINDEX) * 17.31, float(FRAMEINDEX) * 5.13)) - 0.5;
    float lm = dot(col, vec3(0.3, 0.5, 0.2));
    col += gr * grain * 0.07 * (4.0 * lm * (1.0 - lm) + 0.08);

    col *= 1.0 - 0.10 * dot(fromC * vec2(asp, 1.0), fromC * vec2(asp, 1.0));
    col *= 1.0 + (0.22 * bassP() + 0.12 * levP() + 0.10 * beatP()) * ar;   // print-through: the music brightens the whole plate

    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float lu = dot(col, vec3(0.299, 0.587, 0.114));
    col = clamp(mix(vec3(lu), col, 1.22), 0.0, 1.0);

    float alpha = transparentBg ? clamp(max(max(col.r, col.g), col.b) * 1.4, 0.0, 1.0) : 1.0;
    gl_FragColor = vec4(col, alpha);
}
