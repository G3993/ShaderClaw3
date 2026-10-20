/*{
  "DESCRIPTION": "Rain Glass — a night-bus window in the rain, run by a real droplet simulation on the glass. Rain lands as drops of every size, neighbours coalesce, and once a drop is heavy enough it breaks loose and runs, meandering, swallowing the beads in its path and leaving a wet trail that the surface tension pulls apart into strings of tiny beads that slowly dry. Running water wipes clear tracks through the condensation, which fogs back over in its own time. Every drop is a tiny lens: inside it the city is tack-sharp and upside down, ringed by a dark rim with a bright caustic crescent and a glint, while beyond the glass the city melts into bokeh — neon signs, lit windows, sodium street lamps, headlights and tail lights streaking across wet asphalt. The bus pulls away from the lights and stops again, and while it drives the drops get dragged back across the pane. Rain swells from drizzle to downpour with lightning in the heaviest squalls. Bass and beats throw fresh rain at the glass, level brightens the city, mids pulse the neon, highs sparkle the glints.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #5",
  "CATEGORIES": ["Generator", "Nature", "Simulation", "Light", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Speed",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "busMotion",  "LABEL": "Bus Motion",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "traffic",    "LABEL": "Traffic",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "rain",       "LABEL": "Rain",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Rain" },
    { "NAME": "dropSize",   "LABEL": "Drop Size",        "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Rain" },
    { "NAME": "slideEase",  "LABEL": "Slide Ease",       "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Rain" },
    { "NAME": "trails",     "LABEL": "Trails",           "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Rain" },
    { "NAME": "fog",        "LABEL": "Condensation",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55, "GROUP": "Rain" },
    { "NAME": "simRes",     "LABEL": "Sim Resolution",   "TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Rain" },
    { "NAME": "lens",       "LABEL": "Drop Lens",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Glass" },
    { "NAME": "rimDark",    "LABEL": "Dark Rims",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Glass" },
    { "NAME": "glint",      "LABEL": "Glints",           "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Glass" },
    { "NAME": "bokehSize",  "LABEL": "Bokeh Size",       "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "City" },
    { "NAME": "cityGlow",   "LABEL": "City Brightness",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "City" },
    { "NAME": "windows",    "LABEL": "Lit Windows",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.45, "GROUP": "City" },
    { "NAME": "neon1",      "LABEL": "Neon A",           "TYPE": "color", "DEFAULT": [1.0, 0.1, 0.5, 1.0],   "GROUP": "Color" },
    { "NAME": "neon2",      "LABEL": "Neon B",           "TYPE": "color", "DEFAULT": [0.08, 0.75, 1.0, 1.0], "GROUP": "Color" },
    { "NAME": "neon3",      "LABEL": "Neon C",           "TYPE": "color", "DEFAULT": [1.0, 0.5, 0.08, 1.0],  "GROUP": "Color" },
    { "NAME": "lampColor",  "LABEL": "Street Lamps",     "TYPE": "color", "DEFAULT": [1.0, 0.6, 0.26, 1.0],  "GROUP": "Color" },
    { "NAME": "skyColor",   "LABEL": "Night Sky",        "TYPE": "color", "DEFAULT": [0.018, 0.025, 0.07, 1.0], "GROUP": "Color" },
    { "NAME": "lightning",  "LABEL": "Lightning",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Light" },
    { "NAME": "exposure",   "LABEL": "Exposure",         "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "bloom",      "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "aberration", "LABEL": "Chromatic Fringe", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "grain",      "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "audioReact", "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG", "TYPE": "bool", "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "parts", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "dropB", "PERSISTENT": true, "WIDTH": "floor($WIDTH*$simRes)", "HEIGHT": "floor($HEIGHT*$simRes)" },
    { "TARGET": "comp" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Rain Glass — CURVEBALL pack #5
//   The glass is a water heightfield (normalised: a drop of radius r·Lu has
//   peak height r). Two solver steps per frame, each = decide + update:
//     decA/decB   for every cell: should its water slide this step, and
//                 which way (down, down-left, down-right)? Decided from the
//                 blurred local mass so a whole drop moves as one.
//     dropA/dropB gather update (exactly mass-conserving): water arrives from
//                 the three cells above that chose this cell; sliding cells
//                 leave a residue (the trail); still, thin water climbs into
//                 its highest neighbour (beading = surface tension); big
//                 water spreads a little (rounding); rain lands as domes;
//                 condensation (g) regrows and is wiped by water.
//   comp   the city behind the glass (procedural, blur radius as a parameter
//          so the same function gives bokeh outside and a sharp image inside
//          the drops), drops as lenses (inverted refraction, dark rim,
//          caustic crescent, glints), condensation scatter.
//   final  bloom, fringe, grain, ACES, HD finisher.
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }

// ── shared clocks ────────────────────────────────────────────────────────
float g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev;
float g_pan, g_drive, g_weather, g_storm, g_flash;

void setupClocks() {
    g_ar = audioReact;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_t = TIME * speed;
    // the bus: raised-cosine stop → go → stop, period 50 s; pan = ∫ velocity
    float P = 50.0;
    float ph = TAU * g_t / P + 1.9;
    float bm = clamp(busMotion, 0.0, 2.0);
    g_drive = (0.5 - 0.5 * cos(ph)) * min(bm, 1.0);
    g_pan = 0.085 * bm * (0.5 * g_t - P / (2.0 * TAU) * (sin(ph) - sin(1.9)));
    // weather: drizzle ↔ downpour, ~125 s
    g_weather = 0.5 + 0.5 * sin(g_t * 0.05 + 1.0);
    g_storm = smoothstep(0.62, 0.95, g_weather);
    // lightning: rare double-flicker strikes in the squalls
    float bucket = floor(g_t / 7.0);
    float t0 = bucket * 7.0 + 1.0 + 4.5 * hash11(bucket * 3.7);
    float dt = g_t - t0;
    float strike = step(hash11(bucket * 1.93 + 0.4), 0.55 * g_storm * lightning);
    g_flash = strike * step(0.0, dt) * (exp(-dt * 14.0) + 0.6 * exp(-max(dt - 0.12, 0.0) * 9.0) * step(0.12, dt));
}

// ═════════════════════════════════════════════════════════════════════════
//  GLASS SIMULATION  (passes 0..1, sim resolution)
//   parts  runners: rows 0..3 of the first NMAX columns
//          row0 position (int x, frac x, int y, frac y) in sim texels
//          row1 (radius r in Lu, vx, vy, active)
//          row2 position of a runner that stopped this frame (packed)
//          row3 (its radius, stopped flag, 0, 0) → the field takes its water
//   dropB  static water height (r), condensation (g)
// ═════════════════════════════════════════════════════════════════════════
#define NMAX 40
#define NHERO 12
vec2 g_S;      // sim buffer size
float g_Lu;    // length unit in sim texels (a drop of radius 1·Lu is near critical)

vec4 partAt(float i, float row) { return texture2D(parts, (vec2(i, row) + 0.5) / g_S); }
vec2 unpackPos(vec4 t) { return vec2(t.r + t.g, t.b + t.a); }
vec4 packPos(vec2 p) { vec2 f = floor(p); return vec4(f.x, p.x - f.x, f.y, p.y - f.y); }
float fieldH(vec2 c) { return texture2D(dropB, (c + 0.5) / g_S).r; }
float hAt(vec2 c) {
    if (c.x < 0.0 || c.y < 0.0 || c.x > g_S.x - 1.0 || c.y > g_S.y - 1.0) return 0.0;
    return fieldH(c);
}

float rainRate() {
    float r = rain * mix(0.35, 1.6, g_weather);
    r *= 1.0 + 0.8 * g_bass * g_ar;
    return r;
}
float critR() { return 0.95 / slideEase; }
float activeCount() { return floor(float(NMAX) * clamp(0.2 + 0.5 * rainRate(), 0.15, 1.0)); }
float volOf(float r) { return 1.5708 * r * r * r * g_Lu * g_Lu; }
float radOf(float V) { return pow(max(V, 0.0) / (1.5708 * g_Lu * g_Lu), 1.0 / 3.0); }

// a runner breaks loose from the biggest static drop among a few candidates
void spawnRunner(float i, float salt, out vec2 p, out float rr) {
    float F = float(FRAMEINDEX);
    float best = -1.0;
    p = vec2(0.5) * g_S;
    for (int k = 0; k < 8; k++) {
        vec2 cand = hash22(vec2(i * 7.31 + float(k) * 1.37, F * 0.173 + salt)) * vec2(g_S.x, g_S.y * 0.8) + vec2(0.0, g_S.y * 0.18);
        float hc = fieldH(floor(cand));
        if (hc > best) { best = hc; p = cand; }
    }
    float r0 = critR() * mix(1.0, 1.35, hash11(i * 3.7 + F * 0.11 + salt)) * dropSize;
    rr = radOf(volOf(r0) + volOf(max(best, 0.0)) * 0.6);
}

// hero drops: rows 4..5 of the first NHERO columns
//   row4 position (packed), row5 (radius, vy, active, release radius)
//   they sit still and swell with the rain (and with every runner that hits
//   them), and once heavy enough they break loose and slide off the pane.
vec4 heroPass(float i, float row) {
    vec4 r4 = partAt(i, 4.0), r5 = partAt(i, 5.0);
    vec2 p = unpackPos(r4);
    float rr = r5.r, vy = r5.g, rel = r5.a;
    bool act = r5.b > 0.5;
    float F = float(FRAMEINDEX);
    if (FRAMEINDEX < 2 || !act) {
        float salt = FRAMEINDEX < 2 ? 0.0 : F * 0.013;
        p = (vec2(0.06, 0.2) + vec2(0.88, 0.75) * hash22(vec2(i * 3.17 + salt, 9.3 + salt))) * g_S;
        rr = FRAMEINDEX < 2 ? mix(1.0, 2.7, hash11(i * 1.7)) * dropSize : 0.35 * dropSize;
        rel = mix(2.7, 3.6, hash11(i * 4.9 + salt)) * dropSize;
        vy = 0.0;
        act = true;
    } else {
        float sk = clamp(speed, 0.0, 2.0) * (g_S.y / 540.0);
        if (vy == 0.0) {
            // swell: fast pop-in after a fresh coalescence, then slow growth with the rain
            rr += rr < 1.0 * dropSize ? 0.012 : 0.00028 * (0.4 + rainRate());
            for (int j = 0; j < NMAX; j++) {
                vec4 q1 = partAt(float(j), 1.0);
                if (q1.a < 0.5) continue;
                vec2 pj = unpackPos(partAt(float(j), 0.0));
                if (length(pj - p) < (rr + q1.r) * g_Lu * 0.8) rr = radOf(volOf(rr) + volOf(q1.r));
            }
            if (rr > rel) vy = -0.04;
        } else {
            vy = max(vy * 1.03 - 0.015, -2.6);
            float wob = vnoise2(p / (g_S.y * 0.12) + vec2(i * 2.3, g_t * 0.07)) * 2.0 - 1.0;
            p += vec2((wob * 0.35 - g_drive * 0.6) * abs(vy), vy) * sk;
            rr = max(rr - 0.0015, 1.4 * dropSize);
            if (p.y < -rr * g_Lu * 2.0) act = false;
        }
    }
    if (row < 4.5) return packPos(p);
    return vec4(rr, vy, act ? 1.0 : 0.0, rel);
}

vec4 runnerPass(vec2 c) {
    float i = c.x, row = c.y;
    if (row > 3.5 && row < 5.5 && i < float(NHERO) - 0.5) return heroPass(i, row);
    if (i > float(NMAX) - 0.5 || row > 3.5) return vec4(0.0);
    vec4 r0 = partAt(i, 0.0), r1 = partAt(i, 1.0);
    vec2 p = unpackPos(r0);
    float rr = r1.r;
    vec2 v = r1.gb;
    bool act = r1.a > 0.5;
    bool want = i < activeCount();
    bool stopped = false;
    vec2 stopP = p;
    float stopR = rr;

    if (FRAMEINDEX < 2) {
        act = i < activeCount();
        p = hash22(vec2(i * 1.91, 4.2)) * vec2(g_S.x, g_S.y * 0.75) + vec2(0.0, g_S.y * 0.12);
        rr = critR() * mix(1.0, 1.5, hash11(i * 5.3)) * dropSize;
        v = vec2(0.0, -0.5);
    } else if (act) {
        // coalescence with other runners: the bigger one (or lower index) swallows
        bool eaten = false;
        float gain = 0.0;
        for (int j = 0; j < NMAX; j++) {
            float fj = float(j);
            if (abs(fj - i) < 0.5) continue;
            vec4 q1 = partAt(fj, 1.0);
            if (q1.a < 0.5) continue;
            vec2 pj = unpackPos(partAt(fj, 0.0));
            if (length(pj - p) < (rr + q1.r) * g_Lu * 0.8) {
                if (q1.r > rr || (abs(q1.r - rr) < 1e-4 && fj < i)) eaten = true;
                else gain += volOf(q1.r);
            }
        }
        for (int j = 0; j < NHERO; j++) {
            vec4 h5 = partAt(float(j), 5.0);
            if (h5.b < 0.5) continue;
            vec2 ph = unpackPos(partAt(float(j), 4.0));
            if (length(ph - p) < (rr + h5.r) * g_Lu * 0.8) eaten = true;
        }
        if (eaten) {
            act = false;
        } else {
            float V = volOf(rr) + gain;
            rr = radOf(V);
            float over = rr - critR() * 0.82;
            if (over <= 0.0) {
                stopped = true; stopP = p; stopR = rr; act = false;
            } else {
                float sk = clamp(speed, 0.0, 2.0) * (g_S.y / 540.0);
                // stick-slip on the glass
                float pin = 0.12 + 0.88 * smoothstep(0.22, 0.6, vnoise2(p / (g_Lu * 2.6) + vec2(i * 3.1, g_t * 0.3)));
                float vy = -(0.22 + 1.5 * over) * pin;
                float wob = vnoise2(p / (g_S.y * 0.1) + vec2(i * 1.7, g_t * 0.08)) * 2.0 - 1.0;
                float vx = (wob * 0.45 - g_drive * 0.8) * abs(vy) - g_drive * 0.1;
                vec2 dp = vec2(vx, vy) * sk;
                float L = length(dp);
                if (L > 3.0) dp *= 3.0 / L;
                L = length(dp);
                // sweep: swallow static water ahead, leave a residue trail behind
                vec2 dir = L > 1e-4 ? dp / L : vec2(0.0, -1.0);
                vec2 perp = vec2(-dir.y, dir.x);
                float Rt = rr * g_Lu;
                float hAhead = (hAt(floor(p + dir * Rt * 0.95)) + hAt(floor(p + dir * Rt * 0.8 + perp * Rt * 0.55)) + hAt(floor(p + dir * Rt * 0.8 - perp * Rt * 0.55))) / 3.0;
                float res = 0.06 * trails;
                V += (hAhead * 0.9 - res * 0.6) * 2.0 * Rt * L * 0.5;
                rr = radOf(V);
                v = dp;
                p += dp;
                if (p.y < -Rt * 1.5 || p.x < -Rt * 2.0 || p.x > g_S.x + Rt * 2.0) act = false;
            }
        }
    }
    if (!act && want && FRAMEINDEX >= 2) {
        // rain keeps loading the glass: a new runner breaks loose (staggered)
        if (hash11(i * 2.17 + float(FRAMEINDEX) * 0.0131) < 0.1 * rainRate() + 0.02) {
            spawnRunner(i, 0.37, p, rr);
            v = vec2(0.0, -0.3);
            act = true;
        }
    }
    if (row < 0.5) return packPos(p);
    if (row < 1.5) return vec4(act ? rr : 0.0, v, act ? 1.0 : 0.0);
    if (row < 2.5) return packPos(stopP);
    return vec4(stopR, stopped ? 1.0 : 0.0, 0.0, 0.0);
}

// argmax over the 4-neighbourhood of p (fixed order +x, -x, +y, -y)
float argmaxDir(vec2 p, out float hm) {
    float a = hAt(p + vec2(1.0, 0.0)), b = hAt(p - vec2(1.0, 0.0)), cc = hAt(p + vec2(0.0, 1.0)), d = hAt(p - vec2(0.0, 1.0));
    float idx = 0.0; hm = a;
    if (b > hm) { hm = b; idx = 1.0; }
    if (cc > hm) { hm = cc; idx = 2.0; }
    if (d > hm) { hm = d; idx = 3.0; }
    return idx;
}
vec2 dirVec(float i) {
    if (i < 0.5) return vec2(1.0, 0.0);
    if (i < 1.5) return vec2(-1.0, 0.0);
    if (i < 2.5) return vec2(0.0, 1.0);
    return vec2(0.0, -1.0);
}

float dome(vec2 c, vec2 ctr, float rr) {
    float Rt = rr * g_Lu;
    vec2 d = c + 0.5 - ctr;
    return rr * max(0.0, 1.0 - dot(d, d) / (Rt * Rt));
}

// seed: a pane already mid-storm — static drops of every size, mist, and a
// beaded trail above every runner
vec4 seedField(vec2 c) {
    float h = 0.0;
    float clearF = 0.0;
    float G = g_Lu * 2.3;
    vec2 cell = floor(c / G);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 id = cell + vec2(float(i), float(j));
        if (hash12(id + 3.1) < 0.5) {
            vec2 ctr = (id + 0.15 + 0.7 * hash22(id)) * G;
            float rr = mix(0.2, 0.85, pow(hash12(id + 7.7), 2.0)) * dropSize;
            h += dome(c, ctr, rr);
        }
    }
    float G2 = g_Lu * 0.75;
    vec2 c2 = floor(c / G2);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 id = c2 + vec2(float(i), float(j));
        if (hash12(id + 51.3) < 0.0) {
            vec2 ctr = (id + 0.2 + 0.6 * hash22(id + 9.1)) * G2;
            h += dome(c, ctr, mix(0.1, 0.24, hash12(id + 2.2)));
        }
    }
    float sp = g_Lu * 1.1;
    for (int j = 0; j < NMAX; j++) {
        vec4 q1 = partAt(float(j), 1.0);
        if (q1.a < 0.5) continue;
        vec2 pj = unpackPos(partAt(float(j), 0.0));
        float len = g_S.y * (0.12 + 0.5 * hash11(float(j) * 4.1));
        float dy = c.y - pj.y;
        if (dy < q1.r * g_Lu * 0.6 || dy > len) continue;
        float tx = pj.x + sin(dy * 0.05 + float(j)) * g_Lu * 0.5;
        if (abs(c.x - tx) > g_Lu * 2.5) continue;
        clearF = max(clearF, 1.0 - smoothstep(g_Lu * 0.5, g_Lu * 1.6, abs(c.x - tx)));
        float k0 = floor(dy / sp);
        for (int q = -1; q <= 1; q++) {
            float bk = k0 + float(q);
            float by = pj.y + (bk + 0.5) * sp + (hash11(bk * 7.1 + float(j)) - 0.5) * 0.6 * sp;
            float bdy = by - pj.y;
            if (bdy < q1.r * g_Lu * 0.7 || bdy > len) continue;
            float bx = pj.x + sin(bdy * 0.05 + float(j)) * g_Lu * 0.5 + (hash11(bk + float(j) * 3.0) - 0.5) * g_Lu * 0.35;
            float br = mix(0.14, 0.34, hash11(bk * 3.3 + float(j))) * (1.0 - 0.5 * bdy / len);
            h += dome(c, vec2(bx, by), br);
        }
    }
    float fogV = fog * (1.0 - clearF) * (1.0 - smoothstep(0.03, 0.3, h));
    return vec4(h, fogV, 0.0, 1.0);
}

vec4 fieldPass(vec2 c) {
    if (FRAMEINDEX < 2) return seedField(c);
    vec4 me = texture2D(dropB, (c + 0.5) / g_S);
    float h0 = me.r;
    float hn = h0;
    // surface tension: thin water climbs into its highest neighbour (beading);
    // big water spreads a little (rounding)
    const float thin = 0.34;
    const float cb = 0.14;
    float hmSelf;
    argmaxDir(c, hmSelf);
    if (h0 < thin && h0 > 0.0005 && hmSelf > h0) hn -= cb * h0;
    for (int k = 0; k < 4; k++) {
        vec2 e = dirVec(float(k));
        vec2 n = c + e;
        if (n.x < 0.0 || n.y < 0.0 || n.x > g_S.x - 1.0 || n.y > g_S.y - 1.0) continue;
        float hN = fieldH(n);
        float D = 0.05 * smoothstep(0.3, 1.1, max(h0, hN));
        hn += D * (hN - h0);
        if (hN < thin && hN > 0.0005) {
            float hmN;
            float dN = argmaxDir(n, hmN);
            vec2 back = dirVec(dN);
            if (abs(back.x + e.x) + abs(back.y + e.y) < 0.5 && h0 > hN) hn += cb * hN;
        }
    }
    hn = max(hn, 0.0);
    if (hn < 0.3) hn = max(hn - 0.00006 * (1.0 + 2.0 * step(hn, 0.12)), 0.0);
    // oversized puddles shed into the runner system (they are what breaks loose)
    hn -= max(hn - 0.9, 0.0) * 0.02;
    hn *= 0.99965;

    // rain
    float G = g_Lu * 1.8;
    vec2 cell = floor(c / G);
    float F = float(FRAMEINDEX);
    float p = rainRate() * 0.00045 * (1.0 + 5.0 * g_beat * g_ar);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 id = cell + vec2(float(i), float(j));
        vec2 key = id + vec2(F * 0.1173, F * 0.0619);
        if (hash12(key) < p) {
            vec2 ctr = (id + hash22(key + 4.3)) * G;
            float u = hash12(key + 9.7);
            float rr = mix(0.22, 0.85, pow(u, 1.7)) * dropSize;
            hn += dome(c, ctr, rr);
        }
    }

    // runners: swallow what they cover (a narrow residue stripe stays behind),
    // and a runner that stopped hands its water back to the field
    float res = 0.06 * trails;
    float wipe = 0.0;
    for (int j = 0; j < NMAX; j++) {
        float fj = float(j);
        vec4 q1 = partAt(fj, 1.0);
        if (q1.a > 0.5) {
            vec2 pj = unpackPos(partAt(fj, 0.0));
            float Rt = q1.r * g_Lu;
            vec2 d = c + 0.5 - pj;
            if (dot(d, d) < Rt * Rt * 0.85) {
                float stripe = 1.0 - smoothstep(Rt * 0.18, Rt * 0.32, abs(d.x));
                hn = min(hn, res * stripe);
                wipe = 1.0;
            }
        }
        vec4 q3 = partAt(fj, 3.0);
        if (q3.g > 0.5) {
            vec2 ps = unpackPos(partAt(fj, 2.0));
            hn += dome(c, ps, q3.r);
        }
    }

    // hero drops own the glass under them; sliding ones leave a trail
    for (int j = 0; j < NHERO; j++) {
        vec4 h5 = partAt(float(j), 5.0);
        if (h5.b < 0.5) continue;
        vec2 ph = unpackPos(partAt(float(j), 4.0));
        float Rt = h5.r * g_Lu;
        vec2 d = c + 0.5 - ph;
        if (dot(d, d) < Rt * Rt * 1.1) {
            float stripe = h5.g < 0.0 ? 1.0 - smoothstep(Rt * 0.15, Rt * 0.3, abs(d.x)) : 0.0;
            hn = min(hn, res * stripe);
            wipe = 1.0;
        }
    }
    float fg = me.g;
    fg += (fog - fg) * 0.00028;
    fg = min(fg, 1.0 - smoothstep(0.03, 0.3, hn));
    fg *= 1.0 - wipe;
    return vec4(hn, clamp(fg, 0.0, 1.0), 0.0, 1.0);
}

// ═════════════════════════════════════════════════════════════════════════
//  THE CITY BEHIND THE GLASS  (comp pass; r = blur radius in screen heights)
// ═════════════════════════════════════════════════════════════════════════
float g_px;    // one pixel in screen heights
float g_cityK;
vec2  g_pc;    // current sample position (screen heights from centre) for the cat's-eye

// out-of-focus light: a disc through a 6-blade aperture, cat's-eyed toward
// the frame edges, crisp edge, brighter rim, faint onion rings + dust
float bokeh(vec2 d, float rad, float size) {
    float ang = 0.26;
    vec2 dr = vec2(cos(ang) * d.x - sin(ang) * d.y, sin(ang) * d.x + cos(ang) * d.y);
    vec2 a = abs(dr);
    float hx = max(a.x * 0.866 + a.y * 0.5, a.y);
    float L = mix(length(d), hx, 0.45 * smoothstep(0.006, 0.02, rad));
    vec2 sh = g_pc * rad * 1.1;
    float L2 = length(d + sh);
    float aa = g_px * 1.2 + rad * 0.07;
    float m = smoothstep(rad + aa, rad - aa, L) * smoothstep(rad * 1.04 + aa, rad * 1.04 - aa, L2);
    float rim = 0.78 + 0.75 * smoothstep(rad * 0.5, rad * 0.97, L) * smoothstep(rad * 1.05, rad * 0.9, L);
    float tex = 1.0 + 0.05 * sin(L / max(rad, 1e-4) * 17.0);
    float k = pow(size / max(rad, size), 0.62);
    return m * rim * tex * k;
}

vec3 hazeCol(float y) {
    return (lampColor.rgb * 0.05 + neon1.rgb * 0.016 + neon2.rgb * 0.012) * (0.4 + 1.2 * smoothstep(0.7, 0.3, y)) * g_cityK;
}

// a neon sign seen through the defocus: rounded panel of tubes; the blur turns
// it into a soft luminous block, sharp it shows its glyph tubes
vec3 neonBlock(vec2 q, vec2 c, vec2 hs, vec3 nc, float id, float r) {
    vec2 d = q - c;
    vec2 bd = abs(d) - hs;
    float box = length(max(bd, 0.0)) + min(max(bd.x, bd.y), 0.0) - 0.006;
    float bw = r * 0.9 + g_px;
    float cover = smoothstep(bw, -bw, box);
    // glyph tubes, visible only when sharp
    float gx = (d.x + hs.x) / 0.03;
    float gi = floor(gx);
    float gf = fract(gx);
    float glyph = step(0.25, gf) * step(gf, 0.75) * step(0.15 + 0.35 * hash11(gi + id), (d.y + hs.y) / (2.0 * hs.y)) * step((d.y + hs.y) / (2.0 * hs.y), 0.85);
    glyph = max(glyph, step(abs(abs(d.y) - hs.y * 0.82), 0.004) * step(abs(d.x), hs.x));
    float sharp = 1.0 - smoothstep(0.004, 0.012, r);
    float body = mix(0.5, 0.42 + 0.35 * glyph, sharp * 0.4);
    float flick = 1.0;
    if (hash11(id * 9.1) < 0.3) flick = 0.4 + 0.6 * step(0.15, fract(sin(floor(g_t * 7.0 + id * 13.0) * 12.9898) * 43758.5));
    flick *= 1.0 + 0.35 * g_mid * g_ar;
    float halo = exp(-max(box, 0.0) / (0.035 + r * 1.5)) * 0.16;
    return nc * (cover * body * 1.35 + halo) * flick * g_cityK;
}

vec3 city(vec2 st, float r) {
    float asp = R.x / R.y;
    vec2 p = vec2((st.x - 0.5) * asp, st.y - 0.5);
    g_pc = p;
    // night: dark sky, sodium glow low down, wet black street
    vec3 col = skyColor.rgb * (0.55 + 1.7 * smoothstep(0.5, -0.1, p.y));
    col += (neon1.rgb * 0.01 + neon2.rgb * 0.008) * smoothstep(0.45, 0.0, p.y) * g_cityK;
    col += lampColor.rgb * 0.035 * exp(-max(p.y + 0.05, 0.0) * 6.0) * g_cityK;
    col += g_flash * vec3(0.6, 0.55, 0.9) * (0.35 + 0.65 * smoothstep(-0.2, 0.45, p.y));
    float roadY = -0.2;
    // far window lights: sparse, small, warm
    vec2 q = vec2(p.x + g_pan * 0.3, p.y);
    vec2 gs = vec2(0.085, 0.075);
    vec2 gc = floor(q / gs);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 id = gc + vec2(float(i), float(j));
        vec2 lp = (id + 0.15 + 0.7 * hash22(id + 1.3)) * gs;
        float zone = smoothstep(-0.16, -0.1, lp.y) * smoothstep(0.36, 0.2, lp.y) * (0.45 + 0.55 * step(0.0, sin(id.x * 0.9 + 1.7)));
        float ep = floor(g_t / (22.0 + 25.0 * hash12(id)));
        if (hash12(id * 1.13 + ep * 0.37) < windows * 1.0 * zone) {
            float hc = hash12(id + 5.7);
            vec3 lc = hc > 0.84 ? neon2.rgb : (hc > 0.74 ? neon1.rgb : (hc > 0.66 ? vec3(0.95, 0.97, 1.0) : vec3(1.0, 0.5, 0.1)));
            lc *= mix(0.55, 1.0, hash12(id * 2.9));
            col += lc * bokeh(q - lp, max(r * mix(0.8, 1.15, hash12(id + 9.0)), 0.0016), 0.0016) * 1.6 * g_cityK;
        }
    }
    // big neon signs (a few, spaced along the street)
    vec2 qs = vec2(p.x + g_pan * 0.55, p.y);
    float sw = 0.62;
    float si = floor(qs.x / sw);
    for (int k = -1; k <= 1; k++) {
        float id = si + float(k);
        if (hash11(id * 3.7) < 0.82) {
            float pick = hash11(id * 5.3);
            vec3 nc = pick < 0.3 ? neon1.rgb : (pick < 0.58 ? neon2.rgb : (pick < 0.8 ? neon3.rgb : vec3(1.0, 0.08, 0.06)));
            bool vert = hash11(id * 2.1) < 0.45;
            vec2 hs = vert ? vec2(0.022, 0.07 + 0.04 * hash11(id)) : vec2(0.07 + 0.06 * hash11(id), 0.026);
            vec2 c = vec2((id + 0.2 + 0.6 * hash11(id + 0.4)) * sw, 0.08 + 0.17 * hash11(id * 7.7));
            col += neonBlock(qs, c, hs, nc, id, r * 1.1);
            // its reflection smeared down the wet street
            if (p.y < roadY) {
                float w = hs.x + r * 1.2 + 0.004;
                float fx = smoothstep(w, w * 0.4, abs(qs.x - c.x));
                float sh = 0.55 + 0.45 * vnoise2(vec2(qs.x * 90.0, p.y * 50.0 - g_t * 1.3));
                col += nc * fx * sh * exp(-(roadY - p.y) * 5.0) * smoothstep(0.0, 0.03, roadY - p.y) * 0.12 * g_cityK;
            }
        }
    }
    // street lamps
    vec2 ql = vec2(p.x + g_pan, p.y);
    float lsp = 0.55;
    float li = floor(ql.x / lsp);
    for (int k = -1; k <= 1; k++) {
        float id = li + float(k);
        vec2 c = vec2((id + 0.5) * lsp, 0.15 + 0.02 * hash11(id));
        col += lampColor.rgb * bokeh(ql - c, max(r * 1.25, 0.002), 0.002) * 3.2 * g_cityK;
        if (p.y < roadY) {
            float w = r * 0.9 + 0.006;
            float sh = 0.55 + 0.45 * vnoise2(vec2(ql.x * 120.0, p.y * 60.0 - g_t * 1.5));
            col += lampColor.rgb * exp(-abs(ql.x - c.x) / w) * exp(-(roadY - p.y) * 3.0) * smoothstep(0.0, 0.03, roadY - p.y) * sh * 0.3 * g_cityK;
        }
    }
    // traffic lights changing colour
    float tsp = 1.3;
    float ti = floor(ql.x / tsp);
    for (int k = -1; k <= 1; k++) {
        float id = ti + float(k);
        float tx = (id + 0.35) * tsp;
        float cyc = fract(g_t / 16.0 + hash11(id * 2.2));
        float st3 = cyc < 0.45 ? 0.0 : (cyc < 0.55 ? 1.0 : 2.0);
        for (int s = 0; s < 3; s++) {
            float fs = float(s);
            vec3 tc = fs < 0.5 ? vec3(1.0, 0.07, 0.03) : (fs < 1.5 ? vec3(1.0, 0.5, 0.04) : vec3(0.05, 1.0, 0.45));
            float on = abs(fs - (2.0 - st3)) < 0.5 ? 1.0 : 0.03;
            vec2 c = vec2(tx, 0.02 - fs * 0.022);
            col += tc * on * bokeh(ql - c, max(r * 1.1, 0.0018), 0.0018) * 3.0 * g_cityK;
            if (p.y < roadY) col += tc * on * exp(-abs(ql.x - c.x) / (r + 0.006)) * exp(-(roadY - p.y) * 4.0) * smoothstep(0.0, 0.03, roadY - p.y) * 0.2 * g_cityK;
        }
    }
    // cars crossing slowly: headlights one way, tail lights the other
    for (int lane = 0; lane < 2; lane++) {
        float fl = float(lane);
        float vel = fl < 0.5 ? -0.09 : 0.06;
        float y = fl < 0.5 ? -0.165 : -0.19;
        vec3 lc = fl < 0.5 ? vec3(1.0, 0.93, 0.8) : vec3(1.0, 0.06, 0.04);
        float spc = fl < 0.5 ? 0.7 : 0.9;
        float x = p.x + g_pan + g_t * vel * traffic;
        float ci = floor(x / spc);
        for (int k = -1; k <= 1; k++) {
            float id = ci + float(k);
            if (hash11(id * 1.7 + fl * 9.0) < 0.75) {
                float cx = (id + 0.5 + 0.3 * (hash11(id + fl) - 0.5)) * spc;
                for (int s = 0; s < 2; s++) {
                    float xl = cx + (float(s) - 0.5) * 0.06;
                    col += lc * bokeh(vec2(x - xl, p.y - y), max(r * 1.35, 0.0022), 0.0022) * (fl < 0.5 ? 3.2 : 2.6) * g_cityK;
                    if (p.y < y) col += lc * exp(-abs(x - xl) / (r * 0.8 + 0.005)) * exp(-(y - p.y) * 6.0) * 0.18 * g_cityK;
                }
            }
        }
    }
    return col;
}

// ═════════════════════════════════════════════════════════════════════════
//  DROPS  (comp pass)
// ═════════════════════════════════════════════════════════════════════════
vec2 g_Ss;

// cubic B-spline sample of the static water height (4 bilinear taps)
float sampH(vec2 uv) {
    vec2 p = uv * g_Ss - 0.5;
    vec2 i = floor(p), f = p - i;
    vec2 f2 = f * f, f3 = f2 * f;
    vec2 w0 = (1.0 - 3.0 * f + 3.0 * f2 - f3) / 6.0;
    vec2 w1 = (4.0 - 6.0 * f2 + 3.0 * f3) / 6.0;
    vec2 w2 = (1.0 + 3.0 * f + 3.0 * f2 - 3.0 * f3) / 6.0;
    vec2 w3 = f3 / 6.0;
    vec2 g0 = w0 + w1, g1 = w2 + w3;
    vec2 h0 = (i - 0.5 + w1 / g0) / g_Ss + 0.5 / g_Ss;
    vec2 h1 = (i + 1.5 + w3 / g1) / g_Ss + 0.5 / g_Ss;
    float a = texture2D(dropB, vec2(h0.x, h0.y)).r;
    float b = texture2D(dropB, vec2(h1.x, h0.y)).r;
    float c = texture2D(dropB, vec2(h0.x, h1.y)).r;
    float d = texture2D(dropB, vec2(h1.x, h1.y)).r;
    return g0.y * (g0.x * a + g1.x * b) + g1.y * (g0.x * c + g1.x * d);
}

vec4 compose(vec2 fragCoord) {
    vec2 st = fragCoord / R;
    g_px = 1.0 / R.y;
    g_cityK = cityGlow * (1.0 + (0.35 * g_lev + 0.25 * g_bass) * g_ar);
    g_Ss = floor(R * simRes);
    g_S = g_Ss;
    g_Lu = g_Ss.y * 0.019;

    // static water + gradient (per sim texel)
    float h = sampH(st);
    vec2 e = 1.0 / g_Ss;
    float gx = (sampH(st + vec2(e.x, 0.0)) - sampH(st - vec2(e.x, 0.0))) * 0.5;
    float gy = (sampH(st + vec2(0.0, e.y)) - sampH(st - vec2(0.0, e.y))) * 0.5;
    vec2 grad = vec2(gx, gy);
    // analytic drops (heroes + runners) at full resolution — the tallest wins
    vec2 cs = st * g_Ss;
    float hA = 0.0; vec2 gA = vec2(0.0); vec2 cA = vec2(0.0); float rA = 1.0; float caus = 0.0;
    for (int j = 0; j < NMAX + NHERO; j++) {
        bool hero = j >= NMAX;
        float fj = hero ? float(j - NMAX) : float(j);
        vec4 q1 = partAt(fj, hero ? 5.0 : 1.0);
        float on = hero ? q1.b : q1.a;
        if (on < 0.5) continue;
        vec2 pj = unpackPos(partAt(fj, hero ? 4.0 : 0.0));
        float Rt = q1.r * g_Lu;
        vec2 d = cs - pj;
        if (abs(d.x) > Rt * 1.3 || d.y > Rt * 2.0 || d.y < -Rt * 1.8) continue;
        float spd = hero ? abs(q1.g) : length(q1.gb);
        float st2 = 1.0 + 0.3 * min(spd, 2.0);
        // sessile drop on a vertical pane: the belly sags low, the top is a little flatter
        float taper = 1.0 + 0.4 * smoothstep(0.0, Rt * st2, d.y) * min(spd * 2.0 + (hero ? 0.25 : 0.0), 1.0);
        vec2 a = vec2(taper / Rt, 1.0 / (Rt * st2));
        vec2 qd = d * a;
        float q2 = dot(qd, qd);
        if (q2 < 1.0) {
            float hh = q1.r * (1.0 - q2);
            if (hh > hA) { hA = hh; gA = -q1.r * 2.0 * qd * a; cA = pj; rA = Rt; }
        } else if (d.y < 0.0) {
            // the drop focuses light onto the glass just under its lower edge
            float ed = sqrt(q2) - 1.0;
            caus += exp(-ed * 9.0) * smoothstep(0.0, -Rt * 0.6, d.y) * exp(-abs(d.x) / Rt * 1.5) * min(q1.r, 3.0);
        }
    }
    bool analytic = hA > h;
    if (analytic) { h = hA; grad = gA; }
    vec2 slope = grad * g_Lu * 0.75;
    vec3 n = normalize(vec3(-slope, 1.0));

    const float th = 0.07;
    float gPix = length(grad) * (g_Ss.y / R.y);
    float m = clamp((h - th) / (gPix * 1.1 + 0.004) + 0.5, 0.0, 1.0);

    float fogV = texture2D(dropB, st).g;
    vec2 aspK = vec2(R.y / R.x, 1.0);

    // through the glass: a painterly bokeh street
    float film = smoothstep(0.0, th, h);
    vec2 filmOff = -n.xy * 0.01 * film * aspK;
    float rB = 0.034 * bokehSize;
    vec3 bg = city(st + filmOff, rB);
    vec3 haze = hazeCol(st.y);
    // condensation: a faint milky veil and a crust of micro-beads; clear glass where wiped
    vec3 veil = bg * 0.66 + haze * 1.2 + vec3(0.016, 0.016, 0.022);
    vec3 glass = mix(bg * 0.94, veil, smoothstep(0.05, 0.6, fogV) * 0.6);
    vec2 bc = fragCoord / 6.0;
    vec2 bi = floor(bc);
    vec2 bo = hash22(bi + 7.7);
    float br = mix(0.12, 0.42, hash12(bi + 1.1));
    float bd = length(fract(bc) - (0.2 + 0.6 * bo));
    float beadOn = step(hash12(bi + 3.3), fogV * 0.85);
    float bead = smoothstep(br, br - 0.12, bd) * beadOn;
    float beadRim = smoothstep(br - 0.16, br, bd) * bead;
    glass = mix(glass, bg * 1.25 + haze * 0.9, bead * 0.55);
    glass *= 1.0 - beadRim * 0.35;
    glass += (haze * 3.0 + lampColor.rgb * 0.04) * caus * 0.35;
    vec3 col = glass;

    if (m > 0.001) {
        float t = length(n.xy);
        vec3 inner;
        if (analytic) {
            // a real lens: the street inverted and wide-angle inside the drop
            vec2 cuv = (cA + 0.5) / g_Ss;
            vec2 q = (st - cuv) / aspK * R.y / (rA * (R.y / g_Ss.y));
            float qq = dot(q, q);
            vec2 src = cuv - q * (rA * (R.y / g_Ss.y) / R.y) * 6.0 * lens * (1.0 + 0.6 * qq) * aspK;
            src.y = clamp(src.y, 0.02, 0.98);
            inner = city(src, 0.0055) * 1.8 + bg * 0.28;
        } else {
            inner = city(st - n.xy * 0.12 * lens * aspK, 1.4 * g_px);
        }
        // the drop also gathers the defocused glow around it
        inner = analytic ? inner : inner * 1.15 + bg * 0.3;
        float rimD = smoothstep(0.78, 0.97, t) * rimDark + smoothstep(0.93, 1.05, t) * (1.0 - rimDark) * 0.85;
        vec3 dc = inner * (1.0 - rimD);
        vec2 L2 = normalize(vec2(-0.45, 0.9));
        float cres = smoothstep(0.3, 0.62, t) * (1.0 - smoothstep(0.78, 0.93, t)) * pow(max(dot(-n.xy / max(t, 1e-3), L2), 0.0), 2.0);
        dc += cres * (haze * 5.0 + lampColor.rgb * 0.1) * rimDark;
        float gk = glint * (1.0 + 0.8 * g_high * g_ar);
        vec3 L = normalize(vec3(-0.45, 0.75, 0.6));
        vec3 hv = normalize(L + vec3(0.0, 0.0, 1.0));
        dc += vec3(1.0, 0.95, 0.88) * pow(max(dot(n, hv), 0.0), 240.0) * 3.2 * gk;
        vec3 L3 = normalize(vec3(0.65, -0.25, 0.7));
        vec3 hv3 = normalize(L3 + vec3(0.0, 0.0, 1.0));
        dc += mix(neon2.rgb, neon1.rgb, step(0.5, hash12(floor(st * 9.0)))) * pow(max(dot(n, hv3), 0.0), 160.0) * 0.55 * gk;
        
        col = mix(glass, dc, m);
    }
    float alpha = transparentBg ? max(m, fogV * 0.3) : 1.0;
    return vec4(col, alpha);
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    setupClocks();

    if (PASSINDEX <= 1) {
        g_S = R;
        g_Lu = g_S.y * 0.019;
        vec2 c = floor(fragCoord);
        if (PASSINDEX == 0) { gl_FragColor = runnerPass(c); return; }
        gl_FragColor = fieldPass(c);
        return;
    }

    if (PASSINDEX == 2) {
        gl_FragColor = compose(fragCoord);
        return;
    }

    // ── final ────────────────────────────────────────────────────────────
    vec2 st = fragCoord / R;
    vec2 uvc = (fragCoord - 0.5 * R) / R.y;
    vec2 asp = vec2(R.y / R.x, 1.0);
    vec4 c0 = texture2D(comp, st);
    float ab = aberration * 0.0025 * length(uvc);
    vec2 dir = normalize(uvc + 1e-4);
    vec3 col = vec3(texture2D(comp, st + dir * ab * asp).r, c0.g, texture2D(comp, st - dir * ab * asp).b);
    vec3 bl = vec3(0.0);
    float wsum = 0.0;
    for (int i = 0; i < 16; i++) {
        float fi = float(i);
        float a = fi * 2.399963;
        float rr = sqrt((fi + 0.5) / 16.0) * 0.026;
        vec2 of = vec2(cos(a), sin(a)) * rr * asp;
        vec3 s = texture2D(comp, st + of).rgb;
        float w = 1.0 - rr / 0.03;
        bl += max(s - 0.75, 0.0) * w;
        wsum += w;
    }
    col += bl / wsum * bloom * 1.6;
    col *= exposure * 1.15;
    col = aces(col);
    float vig = 1.0 - 0.12 * dot(uvc, uvc);
    col *= vig;
    col += (hash12(fragCoord + fract(TIME) * 91.7) - 0.5) * 0.035 * grain;
    // HD finisher
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float l = dot(col, vec3(0.299, 0.587, 0.114));
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    gl_FragColor = vec4(col, transparentBg ? c0.a : 1.0);
}
