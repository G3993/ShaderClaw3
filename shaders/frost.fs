/*{
  "DESCRIPTION": "Frost — real ice crystals growing on a window at sunrise. A Kobayashi phase-field solidification runs live (four solver steps every frame): the ice front grows where the glass is undercooled, its interface energy is sixfold anisotropic, latent heat released at the front slows its neighbours, and the instability splits every arm into dendrites with side branches, exactly how frost ferns and snowflake arms grow. Several crystals nucleate at different orientations, collide into grain boundaries and keep branching. The low winter sun shines through from behind: thin arms and growth tips burn gold-white, edges split into prismatic fringes, facets glint, and the clear glass around each crystal (vapour it has stolen) stays bright while the rest of the pane beads with condensation — every bead a tiny lens holding an upside-down sunrise. As the sun climbs the frost melts away from it in a warm front, the pane clears, and new crystals nucleate. Bass undercools the glass so the ice grows faster, beats nucleate new crystals, the level warms the sun, highs make the facets glint.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #2",
  "CATEGORIES": ["Generator", "Nature", "Simulation", "Light", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "growSpeed",   "LABEL": "Growth Speed",      "TYPE": "float", "MIN": 0.1, "MAX": 1.0,   "DEFAULT": 0.3,  "GROUP": "Speed" },
    { "NAME": "cycleLength", "LABEL": "Freeze/Thaw Cycle (s)", "TYPE": "float", "AUDIO": false, "MIN": 30.0, "MAX": 240.0, "DEFAULT": 75.0, "GROUP": "Speed" },
    { "NAME": "drift",       "LABEL": "Camera Drift",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "sunRise",     "LABEL": "Sun Rise",          "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Speed" },
    { "NAME": "anisotropy",  "LABEL": "Sixfold Anisotropy","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Crystal" },
    { "NAME": "latentHeat",  "LABEL": "Latent Heat (ferny→faceted)", "TYPE": "float", "AUDIO": false, "MIN": 1.2, "MAX": 2.4, "DEFAULT": 2.0, "GROUP": "Crystal" },
    { "NAME": "undercool",   "LABEL": "Undercooling",      "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.2, "DEFAULT": 1.0, "GROUP": "Crystal" },
    { "NAME": "branching",   "LABEL": "Side Branching",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5,  "GROUP": "Crystal" },
    { "NAME": "seedCount",   "LABEL": "Crystals",          "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 12.0, "DEFAULT": 5.0, "GROUP": "Crystal" },
    { "NAME": "zoom",        "LABEL": "Zoom",              "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 2.0, "DEFAULT": 1.0,  "GROUP": "Crystal" },
    { "NAME": "simRes",      "LABEL": "Sim Resolution",    "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.75, "GROUP": "Crystal" },
    { "NAME": "condensation","LABEL": "Condensation",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.7,  "GROUP": "Glass" },
    { "NAME": "clearHalo",   "LABEL": "Clear Halo",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.7,  "GROUP": "Glass" },
    { "NAME": "refraction",  "LABEL": "Refraction",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.5,  "GROUP": "Glass" },
    { "NAME": "skyTop",      "LABEL": "Sky Top",           "TYPE": "color", "DEFAULT": [0.03, 0.05, 0.16, 1.0], "GROUP": "Color" },
    { "NAME": "skyMid",      "LABEL": "Sky Glow",          "TYPE": "color", "DEFAULT": [0.62, 0.20, 0.42, 1.0], "GROUP": "Color" },
    { "NAME": "horizonColor","LABEL": "Horizon",           "TYPE": "color", "DEFAULT": [1.0, 0.46, 0.12, 1.0],  "GROUP": "Color" },
    { "NAME": "sunColor",    "LABEL": "Sun",               "TYPE": "color", "DEFAULT": [1.0, 0.78, 0.46, 1.0],  "GROUP": "Color" },
    { "NAME": "iceTint",     "LABEL": "Ice",               "TYPE": "color", "DEFAULT": [0.62, 0.82, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "snowColor",   "LABEL": "Snowfield",         "TYPE": "color", "DEFAULT": [0.36, 0.40, 0.66, 1.0], "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0, "MAX": 1.5,   "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "sunGlow",     "LABEL": "Sun Glow",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0,   "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "edgeLight",   "LABEL": "Edge Light",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0,   "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "prism",       "LABEL": "Prismatic Fringe",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0,   "DEFAULT": 0.9,  "GROUP": "Light" },
    { "NAME": "sparkle",     "LABEL": "Glints",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0,   "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0,   "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.12, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0,   "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",   "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "fA", "PERSISTENT": true, "WIDTH": "floor($WIDTH*min($simRes,1152.0/$WIDTH))", "HEIGHT": "floor($HEIGHT*min($simRes,1152.0/$WIDTH))" },
    { "TARGET": "fB", "PERSISTENT": true, "WIDTH": "floor($WIDTH*min($simRes,1152.0/$WIDTH))", "HEIGHT": "floor($HEIGHT*min($simRes,1152.0/$WIDTH))" },
    { "TARGET": "fC", "PERSISTENT": true, "WIDTH": "floor($WIDTH*min($simRes,1152.0/$WIDTH))", "HEIGHT": "floor($HEIGHT*min($simRes,1152.0/$WIDTH))" },
    { "TARGET": "fD", "PERSISTENT": true, "WIDTH": "floor($WIDTH*min($simRes,1152.0/$WIDTH))", "HEIGHT": "floor($HEIGHT*min($simRes,1152.0/$WIDTH))" },
    { "TARGET": "fBlur", "WIDTH": "floor($WIDTH*min($simRes,1152.0/$WIDTH))", "HEIGHT": "floor($HEIGHT*min($simRes,1152.0/$WIDTH))" },
    { "TARGET": "fScene" },
    { "TARGET": "fBloom", "WIDTH": "$WIDTH/4", "HEIGHT": "$HEIGHT/4" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Frost — CURVEBALL pack #2
//   passes 0-3  fA → … → fD : Kobayashi (1993) anisotropic phase-field
//               solidification, one explicit step per pass (4 per frame).
//               state = (phi solid fraction, T temperature, grain orientation
//               as a fraction of 60°, age in seconds since frozen)
//   pass 4      fBlur : thickness (small blur of phi), vapour halo (large
//               blur), blurred temperature
//   pass 5      fScene : sunrise behind glass, refracting ice, condensation
//               beads, prismatic edges, glints (HDR)
//   pass 6      fBloom : quarter-res bright prefilter
//   pass 7      final : bloom, fringe, ACES, HD finisher, grain
// ─────────────────────────────────────────────────────────────────────────

#define TAU 6.28318530718
#define PI  3.14159265359

// Kobayashi constants (grid units dx = 0.03)
#define KDX    0.03
#define KTAU   0.0003
#define KEPS   0.0075
#define KALPHA 0.9
#define KGAMMA 10.0
#define KJ     6.0

float fh11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float fh12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  fh22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float fvn(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(fh12(p), fh12(p + vec2(1.0, 0.0)), f.x), mix(fh12(p + vec2(0.0, 1.0)), fh12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float ffbm(vec2 p) { float a = 0.5, s = 0.0; for (int i = 0; i < 4; i++) { s += a * fvn(p); p = p * 2.07 + 13.7; a *= 0.5; } return s; }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { return clamp((x * (2.51 * x + 0.03)) / (x * (2.43 * x + 0.59) + 0.14), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float levP()  { return clamp(audioLevel, 0.0, 1.0); }

vec2 simSize() {
    vec2 R = RENDERSIZE.xy;
    float f = min(simRes, 1152.0 / R.x);
    return floor(R * f);
}

// global clock for the sun; the freeze/thaw cycle itself is LOCAL — its phase
// sweeps across the pane (localPhase) so one region thaws while another regrows
float cyclePhase() { return fract(TIME / cycleLength); }
float localPhase(vec2 uv) { return fract(TIME / cycleLength + 0.62 * uv.x + 0.18 * uv.y + 0.08 * sin(uv.y * 5.0 + uv.x * 3.0)); }
float cycleIndex() { return floor(TIME / cycleLength); }

vec2 sunPosUV(float ph, float aspect) {
    float rise = 0.5 - 0.5 * cos(TAU * ph);         // periodic: no jump at the wrap
    return vec2(0.64 + 0.05 * sin(ph * 2.0), 0.335 + sunRise * 0.15 * rise);
}

// ═════════════════════════════════════════════════════════════════════════
//  PHASE FIELD (passes 0..3)
// ═════════════════════════════════════════════════════════════════════════
vec4 P(vec2 uv) {
    uv = clamp(uv, vec2(0.0), vec2(1.0));
    if (PASSINDEX == 0) return texture2D(fD, uv);
    if (PASSINDEX == 1) return texture2D(fA, uv);
    if (PASSINDEX == 2) return texture2D(fB, uv);
    return texture2D(fC, uv);
}

// anisotropic eps(theta) and its derivative for a gradient g
vec2 epsOf(vec2 g, float th0, float delta) {
    if (dot(g, g) < 1e-8) return vec2(KEPS, 0.0);
    float a = KJ * (atan(g.y, g.x) - th0);
    return vec2(KEPS * (1.0 + delta * cos(a)), -KEPS * delta * KJ * sin(a));
}

// analytic grown dendrite used for the very first frame (the phase field
// sharpens it and keeps growing from its tips)
float starShape(vec2 d, float th0, float R) {
    float c = cos(-th0), s = sin(-th0);
    vec2 q = vec2(c * d.x - s * d.y, s * d.x + c * d.y);
    float r = length(q);
    if (r > R + 2.0) return 0.0;
    float ang = atan(q.y, q.x);
    float a = mod(ang + PI / 6.0, PI / 3.0) - PI / 6.0;
    vec2 l = vec2(cos(a), sin(a)) * r;          // x along the arm, y across
    float inside = 0.0;
    float wArm = 1.6 + 2.2 * (1.0 - l.x / R);
    if (l.x < R && abs(l.y) < wArm) inside = 1.0;
    // side branches at 60°, every 9 cells
    float sp = 9.0;
    float k = floor(l.x / sp);
    float bx = k * sp;
    vec2 rel = vec2(l.x - bx, abs(l.y));
    float t = dot(rel, vec2(0.5, 0.8660254));
    float perp = abs(dot(rel, vec2(-0.8660254, 0.5)));
    float bl = (R - bx) * 0.42 * (0.6 + 0.4 * fh11(k + th0 * 17.0));
    if (k > 0.5 && t > 0.0 && t < bl && perp < 0.6 + 1.3 * (1.0 - t / bl)) inside = 1.0;
    if (r < 3.5) inside = 1.0;
    return inside;
}

vec4 simPass() {
    vec2 S = RENDERSIZE.xy;
    vec2 px = 1.0 / S;
    vec2 uv = gl_FragCoord.xy * px;
    vec2 cell = gl_FragCoord.xy;
    float ar = audioReact;

    float ph = cyclePhase();
    float ci = cycleIndex();
    bool first = FRAMEINDEX < 2;
    bool reset = first;

    vec4 c;
    if (PASSINDEX == 0 && reset) {
        // fresh pane: undercooled glass + nuclei (grown stars on the first frame)
        float T0 = 1.0 - undercool;
        c = vec4(0.0, T0, 0.0, 0.0);
        float n = clamp(seedCount, 1.0, 12.0);
        for (int i = 0; i < 12; i++) {
            if (float(i) >= n) break;
            // R2 low-discrepancy placement so crystals spread over the pane
            vec2 h = fract(vec2(0.5) + float(i) * vec2(0.7548776662, 0.5698402910) + ci * vec2(0.317, 0.173));
            h += (fh22(vec2(float(i), ci)) - 0.5) * 0.08;
            vec2 cp = (vec2(0.07, 0.09) + h * vec2(0.86, 0.82)) * S;
            float th0 = fh11(float(i) * 3.7 + ci * 11.3) * TAU / 6.0;
            float R = S.y * (first ? (0.08 + 0.13 * fh11(float(i) * 5.1 + 2.0)) : (0.03 + 0.05 * fh11(float(i) * 5.1 + 2.0 + ci)));
            float inside = starShape(cell - cp, th0, R);
            if (inside > 0.5) { c = vec4(1.0, T0 + 0.3, fract(th0 / (TAU / 6.0)), 12.0); }
        }
        return c;
    }

    c = P(uv);
    vec4 e = P(uv + vec2(px.x, 0.0)), w = P(uv - vec2(px.x, 0.0));
    vec4 n = P(uv + vec2(0.0, px.y)), s = P(uv - vec2(0.0, px.y));
    vec4 ne = P(uv + px), nw = P(uv + vec2(-px.x, px.y));
    vec4 se = P(uv + vec2(px.x, -px.y)), sw = P(uv - px);
    vec4 ee = P(uv + vec2(2.0 * px.x, 0.0)), ww = P(uv - vec2(2.0 * px.x, 0.0));
    vec4 nn = P(uv + vec2(0.0, 2.0 * px.y)), ss = P(uv - vec2(0.0, 2.0 * px.y));

    // grain orientation: liquid adopts the orientation of its most solid neighbour
    float grain = c.b;
    float best = c.r;
    if (e.r > best + 0.02) { best = e.r; grain = e.b; }
    if (w.r > best + 0.02) { best = w.r; grain = w.b; }
    if (n.r > best + 0.02) { best = n.r; grain = n.b; }
    if (s.r > best + 0.02) { best = s.r; grain = s.b; }
    if (c.r > 0.5) grain = c.b;
    float th0 = grain * TAU / 6.0;

    float delta = 0.015 + 0.055 * anisotropy;
    float i2 = 1.0 / (2.0 * KDX);
    vec2 gE = vec2(ee.r - c.r, ne.r - se.r) * i2;
    vec2 gW = vec2(c.r - ww.r, nw.r - sw.r) * i2;
    vec2 gN = vec2(ne.r - nw.r, nn.r - c.r) * i2;
    vec2 gS = vec2(se.r - sw.r, c.r - ss.r) * i2;
    vec2 gC = vec2(e.r - w.r, n.r - s.r) * i2;
    vec2 kE = epsOf(gE, th0, delta), kW = epsOf(gW, th0, delta);
    vec2 kN = epsOf(gN, th0, delta), kS = epsOf(gS, th0, delta);
    vec2 kC = epsOf(gC, th0, delta);

    float t1 = -(kE.x * kE.y * gE.y - kW.x * kW.y * gW.y) * i2;
    float t2 =  (kN.x * kN.y * gN.x - kS.x * kS.y * gS.x) * i2;
    float e2C = kC.x * kC.x;
    float t3 = (0.5 * (kE.x * kE.x + e2C) * (e.r - c.r) + 0.5 * (kW.x * kW.x + e2C) * (w.r - c.r)
              + 0.5 * (kN.x * kN.x + e2C) * (n.r - c.r) + 0.5 * (kS.x * kS.x + e2C) * (s.r - c.r)) / (KDX * KDX);

    // undercooling driving force; melt front radiates from the sun late in the cycle
    float m = KALPHA / PI * atan(KGAMMA * (1.0 - c.g));
    float aspect = S.x / S.y;
    vec2 sp = sunPosUV(ph, aspect);
    float dd = length((uv - sp) * vec2(aspect, 1.0));
    float lph = localPhase(uv);
    // thaw window wraps past the cycle seam so no remnant regrows as a ghost outline
    float melt = max(smoothstep(0.80, 0.90, lph), 1.0 - smoothstep(0.03, 0.07, lph)) * (0.75 + 0.25 * smoothstep(0.9, 0.2, dd));
    m = mix(m, -0.42, melt);

    float aN = 0.02 * branching;
    float chi = fh12(cell + vec2(float(FRAMEINDEX) * 1.37, float(PASSINDEX) * 9.1)) - 0.5;
    float react = c.r * (1.0 - c.r) * (c.r - 0.5 + m + aN * chi * 2.0);

    float dt = 1.75e-4 * growSpeed * (1.0 + 0.18 * bassP() * ar);
    dt = min(dt, 2.0e-4);
    float dp = dt / KTAU * (t1 + t2 + t3 + react);
    float p = clamp(c.r + dp, 0.0, 1.0);
    p = max(p - melt * 0.0012, 0.0);
    p *= 1.0 - 0.04 * smoothstep(0.6, 1.0, melt);   // thaw eats the interior too, not only the edge
    dp = p - c.r;

    float lapT = (e.g + w.g + n.g + s.g - 4.0 * c.g) / (KDX * KDX);
    float T = c.g + dt * lapT + latentHeat * dp;
    // the cold outside pulls heat out through the glass (slow sink toward the
    // undercooled temperature) so growth never stalls but stays sparse
    float T0 = 1.0 - undercool;
    T -= (T - T0) * dt * 0.15;
    T -= (T - T0) * melt * 0.02;   // the thawed pane relaxes back to cold glass
    T = clamp(T, -0.5, 1.6);

    // nucleation: a slow spontaneous rhythm, plus beats, only while freezing
    if (PASSINDEX == 0 && lph < 0.55) {
        float slot = floor(TIME * 2.2);
        vec2 q = (vec2(0.04, 0.05) + fh22(vec2(slot, 4.2)) * vec2(0.92, 0.9)) * S;
        float fresh = 1.0 - smoothstep(0.0, 0.3, lph);         // freshly thawed glass seeds eagerly
        float nucl = step(length(cell - q), 2.5) * step(fh11(slot * 1.91), 0.35 + 0.65 * fresh);
        float bslot = floor(TIME * 3.0);
        vec2 qb = (vec2(0.06, 0.08) + fh22(vec2(bslot * 1.3, 9.7 + ci)) * vec2(0.88, 0.84)) * S;
        nucl = max(nucl, step(length(cell - qb), 2.5) * step(0.55, audioBeatPulse * ar));
        if (nucl > 0.5 && c.r < 0.3 && c.g < 1.0 - undercool + 0.2) {
            p = 1.0; grain = fh11(slot * 2.3 + bslot * 0.71); T = c.g;
        }
    }

    float age = c.a;
    if (PASSINDEX == 3) age = p > 0.5 ? min(c.a + TIMEDELTA, 60.0) : 0.0;
    return vec4(p, T, grain, age);
}

// ═════════════════════════════════════════════════════════════════════════
//  BLUR (pass 4): thickness, vapour halo, temperature
// ═════════════════════════════════════════════════════════════════════════
vec4 blurPass() {
    vec2 S = RENDERSIZE.xy;
    vec2 px = 1.0 / S;
    vec2 uv = gl_FragCoord.xy * px;
    vec4 c = texture2D(fD, uv);
    float a = c.r * 0.2, b = c.r * 0.08, t = c.g * 0.2;
    float wa = 0.2, wb = 0.08;
    for (int i = 0; i < 14; i++) {
        float fi = float(i);
        float ang = fi * 2.39996323;
        float rr = sqrt((fi + 0.5) / 14.0);
        vec2 d = vec2(cos(ang), sin(ang));
        vec4 s1 = texture2D(fD, uv + d * rr * 2.6 * px);
        vec4 s2 = texture2D(fD, uv + d * rr * 13.0 * px);
        float w1 = 1.0 - 0.5 * rr;
        a += s1.r * w1; t += s1.g * w1; wa += w1;
        b += s2.r; wb += 1.0;
    }
    return vec4(a / wa, b / wb, t / wa, c.r);
}

// ═════════════════════════════════════════════════════════════════════════
//  SCENE (pass 5)
// ═════════════════════════════════════════════════════════════════════════
vec4 bspline(sampler2D tex, vec2 uv, vec2 res) {
    vec2 st = uv * res - 0.5;
    vec2 i = floor(st);
    vec2 f = st - i;
    vec2 f2 = f * f, f3 = f2 * f;
    vec2 w0 = (1.0 / 6.0) * (-f3 + 3.0 * f2 - 3.0 * f + 1.0);
    vec2 w1 = (1.0 / 6.0) * (3.0 * f3 - 6.0 * f2 + 4.0);
    vec2 w2 = (1.0 / 6.0) * (-3.0 * f3 + 3.0 * f2 + 3.0 * f + 1.0);
    vec2 w3 = (1.0 / 6.0) * f3;
    vec2 g0 = w0 + w1, g1 = w2 + w3;
    vec2 h0 = (i - 1.0 + w1 / g0 + 0.5) / res;
    vec2 h1 = (i + 1.0 + w3 / g1 + 0.5) / res;
    return g0.y * (g0.x * texture2D(tex, vec2(h0.x, h0.y)) + g1.x * texture2D(tex, vec2(h1.x, h0.y)))
         + g1.y * (g0.x * texture2D(tex, vec2(h0.x, h1.y)) + g1.x * texture2D(tex, vec2(h1.x, h1.y)));
}

float g_aspect, g_ph, g_bl;
vec2  g_sun;
vec3  g_sunCol;

// out-of-focus winter sunrise behind the glass
vec3 background(vec2 u) {
    float hy = 0.335;
    vec3 sunC = g_sunCol;
    vec2 sd = (u - g_sun) * vec2(g_aspect, 1.0);
    float dS = length(sd);
    vec3 col;
    float y = u.y;
    // sky
    float t = clamp((y - hy) / (1.0 - hy), 0.0, 1.0);
    col = mix(horizonColor.rgb * 1.15, skyMid.rgb, smoothstep(0.0, 0.32, t));
    col = mix(col, skyTop.rgb, smoothstep(0.22, 0.95, t));
    col += sunC * (exp(-dS * 5.0) * 0.55 + exp(-dS * 16.0) * 1.1) * sunGlow * (1.0 + (0.4 * bassP() + 0.3 * levP()) * audioReact);
    // high thin cloud streaks lit from below
    float cl = ffbm(vec2(u.x * 3.0 + TIME * 0.004, u.y * 14.0));
    cl = smoothstep(0.55, 0.85, cl) * smoothstep(hy + 0.05, hy + 0.3, y) * (1.0 - smoothstep(0.75, 0.95, y));
    col = mix(col, mix(skyMid.rgb * 0.5, sunC * 1.4, exp(-dS * 2.5)), cl * 0.55);
    // sun disc, out of focus: a bright disc with a crisp rim
    float r = 0.045;
    float disc = smoothstep(r, r - 0.004, dS);
    col += sunC * disc * 5.5 * sunGlow;
    // distant hills + tree line (aerial perspective), then near tree line
    float h1 = hy + 0.025 + 0.03 * ffbm(vec2(u.x * 4.0, 1.3));
    float hill = smoothstep(h1 + 0.006, h1 - 0.006, y);
    col = mix(col, mix(skyMid.rgb * 0.55, horizonColor.rgb * 0.55, 0.4), hill * step(hy - 0.02, y) * 0.85);
    // two rows of firs: irregular spacing / height, tiered branch silhouettes
    float treeM = 0.0;
    float snowT = 0.0;
    for (int k = 0; k < 2; k++) {
        float fk = float(k);
        float den = 22.0 + 17.0 * fk;
        float tx = u.x * den + fk * 0.37;
        float tid = floor(tx);
        float best = 0.0;
        for (int j = -1; j <= 1; j++) {
            float id = tid + float(j);
            float cx = id + 0.5 + 0.35 * (fh11(id * 1.7 + fk) - 0.5);
            float hgt = (0.02 + 0.075 * pow(fh11(id * 3.3 + fk * 5.0), 1.6)) * (1.0 - 0.35 * fk);
            float present = step(0.18, fh11(id * 9.1 + fk));
            float ty = (y - (hy - 0.006)) / max(hgt, 1e-3);
            float dx = abs(tx - cx) / den * g_aspect / max(hgt, 1e-3);
            float tier = fract(ty * (5.0 + 2.0 * fh11(id)));
            float w = (1.0 - ty) * 0.42 * (0.7 + 0.3 * (1.0 - tier));
            float soft = 0.09 + 0.06 * fk;                      // behind the glass: out of focus
            float inside = smoothstep(-0.03, 0.03, ty) * smoothstep(1.04, 0.96, ty) * smoothstep(w + soft, w - soft, dx) * present;
            snowT = max(snowT, inside * smoothstep(0.55, 0.95, tier) * step(fk, 0.5));
            best = max(best, inside);
        }
        treeM = max(treeM, best);
    }
    treeM = max(treeM, smoothstep(hy + 0.002, hy - 0.006, y) * step(hy - 0.03, y));
    vec3 treeCol = vec3(0.015, 0.02, 0.05) + sunC * 0.12 * exp(-abs(sd.x) * 3.0);
    treeCol += (snowColor.rgb * 0.35 + sunC * 0.25 * exp(-abs(sd.x) * 4.0)) * snowT;
    col = mix(col, treeCol, treeM);
    // snowfield below the horizon
    if (y < hy - 0.004) {
        float fy = (hy - y) / hy;
        vec3 snow = snowColor.rgb * (0.55 - 0.45 * fy);
        snow += sunC * 0.9 * exp(-abs(sd.x) * 7.0) * exp(-fy * 5.0);      // sun path on snow
        snow += sunC * 0.25 * exp(-dS * 3.0);
        snow *= 0.35 + 0.65 * smoothstep(1.0, 0.15, fy);                    // dark foreground
        snow += vec3(0.0, 0.0, 0.02);
        col = mix(snow, col, treeM);
    }
    // bokeh from snow sparkle and far lights
    for (int i = 0; i < 9; i++) {
        float fi = float(i);
        vec2 bp = vec2(fh11(fi * 3.1 + 0.2), 0.12 + 0.3 * fh11(fi * 7.7 + 1.0));
        bp.x += 0.01 * sin(TIME * 0.05 + fi);
        float br = 0.012 + 0.022 * fh11(fi * 1.9);
        float bd = length((u - bp) * vec2(g_aspect, 1.0));
        float bk = smoothstep(br, br - 0.0025, bd) * (0.55 + 0.45 * smoothstep(br * 0.55, br, bd));
        vec3 bc = mix(sunC, iceTint.rgb, fh11(fi * 5.3));
        col += bc * bk * (0.12 + 0.25 * fh11(fi * 2.7));
    }
    return col;
}

vec4 scenePass() {
    vec2 R = RENDERSIZE.xy;
    vec2 uvG = gl_FragCoord.xy / R;
    g_aspect = R.x / R.y;
    g_ph = cyclePhase();
    g_sun = sunPosUV(g_ph, g_aspect);
    g_sunCol = sunColor.rgb;
    float ar = audioReact;
    vec2 S = simSize();

    // glass coordinates (slow breathing drift)
    float z = zoom * (1.0 + 0.035 * drift * (0.5 + 0.5 * sin(TIME * 0.043)));
    vec2 uvS = (uvG - 0.5) / z + 0.5 + drift * 0.008 * vec2(sin(TIME * 0.031), cos(TIME * 0.027));
    vec2 bgShift = drift * 0.004 * vec2(sin(TIME * 0.031), cos(TIME * 0.027));

    vec2 sd = (uvG - g_sun) * vec2(g_aspect, 1.0);
    float dS = length(sd);
    vec2 sunDir = -sd / max(dS, 1e-4);            // pointing toward the sun
    float bl = (0.22 + 0.7 * exp(-dS * 2.8) + 0.45 * exp(-dS * 9.0)) * sunGlow * (1.0 + (0.4 * bassP() + 0.3 * levP()) * ar);

    // ice coverage with exact screen-pixel AA
    vec2 dpx = 1.0 / R / z;
    float inSim = step(0.0, uvS.x) * step(uvS.x, 1.0) * step(0.0, uvS.y) * step(uvS.y, 1.0);
    float phiC = bspline(fD, uvS, S).r * inSim;
    float phiX = bspline(fD, uvS + vec2(dpx.x, 0.0), S).r * inSim;
    float phiY = bspline(fD, uvS + vec2(0.0, dpx.y), S).r * inSim;
    vec2 gpx = vec2(phiX - phiC, phiY - phiC);
    float gl = max(length(gpx), 1e-4);
    float sdist = (phiC - 0.5) / gl;              // signed distance to the ice edge in screen px
    float mask = clamp(sdist + 0.5, 0.0, 1.0);

    vec4 bt = texture2D(fBlur, uvS);
    vec2 spx = 1.0 / S;
    float tx1 = texture2D(fBlur, uvS + vec2(spx.x, 0.0)).r - texture2D(fBlur, uvS - vec2(spx.x, 0.0)).r;
    float ty1 = texture2D(fBlur, uvS + vec2(0.0, spx.y)).r - texture2D(fBlur, uvS - vec2(0.0, spx.y)).r;
    vec2 gT = vec2(tx1, ty1);
    float thick = bt.r * inSim;
    float halo = bt.g * inSim;
    float Tb = bt.b * inSim;
    vec3 nrm = normalize(vec3(-gT * 3.5, 1.0));

    // background through the glass (ice refracts it)
    vec2 refr = (gT * 0.9 + nrm.xy * 0.012) * refraction * mask;
    vec3 bg = background(uvG + bgShift + refr);
    vec3 col = bg;

    // ── condensation beads on the bare glass (clear halo around crystals) ──
    float fog = condensation * (1.0 - mask) * (1.0 - clearHalo * smoothstep(0.02, 0.28, halo)) * inSim;
    if (fog > 0.01) {
        float den = 62.0;
        vec2 gp = uvS * vec2(g_aspect, 1.0) * den;
        vec2 gi = floor(gp), gf = fract(gp);
        float bestD = 9.0; vec2 bestO = vec2(0.0); float bestR = 0.3;
        for (int j = -1; j <= 1; j++) {
            for (int i = -1; i <= 1; i++) {
                vec2 o = vec2(float(i), float(j));
                vec2 h = fh22(gi + o);
                vec2 cc = o + 0.15 + 0.7 * h;
                float r = 0.12 + 0.3 * fh12(gi + o + 7.3) * fh12(gi + o + 2.1);
                vec2 d = gf - cc;
                float dist = length(d) - r;
                if (dist < bestD) { bestD = dist; bestO = d; bestR = r; }
            }
        }
        float cellPx = R.y * z / den;
        float bead = clamp(-bestD * cellPx + 0.5, 0.0, 1.0);
        vec2 q = bestO / bestR;
        float ql = length(q);
        vec3 inv = background(uvG + bgShift - q * 0.012);     // each bead: a tiny inverted sunrise
        float rim = smoothstep(0.55, 1.0, ql);
        vec2 hlP = sunDir * 0.42;
        float hl = exp(-length(q - hlP) * 9.0);
        vec3 beadCol = inv * (1.02 - 0.55 * rim) + g_sunCol * bl * (hl * 1.4 + rim * 0.12 * exp(-dS * 3.0));
        // the film between beads scatters a little sunlight (no milk)
        vec3 film = col * 0.9 + g_sunCol * 0.05 * bl;
        col = mix(col, film, fog);
        col = mix(col, beadCol, bead * fog);
    }

    // ── ice: clear, refracting; light scatters in thin feathery edges + facets ──
    float edgeBand = mask * exp(-max(sdist, 0.0) * 0.55);               // light caught just inside each edge
    float thin = mask * (1.0 - smoothstep(0.25, 0.75, thick));          // fine side-branches
    float frosted = mask * smoothstep(0.7, 1.0, thick);                  // only thick cores go milky
    float facet = pow(max(0.0, dot(nrm, normalize(vec3(sunDir * 0.7, 0.75)))), 34.0) * mask;
    float hot = smoothstep(0.18, 0.6, Tb) * edgeBand;                   // latent heat at growing tips

    float blI = min(bl, 1.0);                       // ice lighting saturates so crystals keep detail against the sun
    vec3 ice = iceTint.rgb;
    vec3 lit = mix(ice, g_sunCol, clamp(exp(-dS * 3.2) * 0.8, 0.0, 1.0));
    col *= 1.0 - mask * (0.04 + 0.1 * thick);       // ice absorbs a touch
    col += lit * (edgeBand * 0.55 + thin * 0.18 + frosted * 0.16) * blI * edgeLight;
    col += lit * mask * smoothstep(0.08, 0.3, length(gT)) * 0.25 * blI; // scattering where the ice thickness changes
    col += g_sunCol * facet * 1.6 * bl;
    col += mix(g_sunCol, vec3(1.0, 0.95, 0.85), 0.5) * hot * 1.2 * blI;
    // micro frost texture inside the ice (rime grains)
    float rime = fh12(floor(uvS * S * 2.0)) * fh12(floor(uvS * S * 2.0) + 3.1);
    col += lit * mask * smoothstep(0.6, 1.0, rime) * (0.05 + 0.1 * thick) * blI;
    float rim = edgeBand;

    // prismatic fringe: the edge splits into R / G / B bands on the sunward side
    float pr = 0.6 * prism * (0.4 + 0.6 * clamp(exp(-dS * 1.8) * 1.5, 0.0, 1.0)) * (1.0 + 0.3 * smoothstep(0.1, 0.8, audioMid) * ar);
    float bR = exp(-pow((sdist - 1.6) / 0.9, 2.0));
    float bG = exp(-pow((sdist + 0.0) / 0.9, 2.0));
    float bB = exp(-pow((sdist + 1.6) / 0.9, 2.0));
    col += vec3(bR * 1.0, bG * 0.55, bB * 1.1) * g_sunCol * pr * 0.6 * bl * inSim;

    // glints: one candidate per 6x6 sim cells, twinkling where it sits on an edge
    vec2 gcell = floor(uvS * S / 6.0);
    vec2 gpos = (gcell + 0.2 + 0.6 * fh22(gcell + 5.5)) * 6.0 / S;
    vec2 gd = (uvS - gpos) * z * R;
    float gphase = fh12(gcell + 1.7);
    float tw = pow(0.5 + 0.5 * sin(TIME * (1.2 + 2.0 * gphase) + gphase * 40.0), 6.0);
    float gOn = step(0.7, gphase) * (edgeBand + facet * 4.0);
    float star = exp(-dot(gd, gd) * 0.35) * 2.0 + (exp(-abs(gd.x) * 1.4) * exp(-abs(gd.y) * 0.22) + exp(-abs(gd.y) * 1.4) * exp(-abs(gd.x) * 0.22)) * 0.5;
    col += vec3(1.0, 0.97, 0.9) * star * tw * gOn * sparkle * (0.6 + 0.9 * highP() * ar) * bl * 0.8;

    float alpha = 1.0;
    if (transparentBg) {
        alpha = clamp(mask + rim * 0.8 + fog * 0.3, 0.0, 1.0);
    }
    return vec4(max(col, 0.0), alpha);
}

// ═════════════════════════════════════════════════════════════════════════
//  BLOOM PREFILTER (pass 6) + FINAL (pass 7)
// ═════════════════════════════════════════════════════════════════════════
vec4 bloomPass() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 px = 1.0 / RENDERSIZE.xy;
    vec3 c = vec3(0.0);
    c += texture2D(fScene, uv + px * vec2(-0.25, -0.25)).rgb;
    c += texture2D(fScene, uv + px * vec2(0.25, -0.25)).rgb;
    c += texture2D(fScene, uv + px * vec2(-0.25, 0.25)).rgb;
    c += texture2D(fScene, uv + px * vec2(0.25, 0.25)).rgb;
    c *= 0.25;
    float l = lum(c);
    c *= smoothstep(0.75, 1.6, l);
    return vec4(c, 1.0);
}

vec4 finalPass() {
    vec2 R = RENDERSIZE.xy;
    vec2 uv = gl_FragCoord.xy / R;
    vec4 sc = texture2D(fScene, uv);
    vec3 col = sc.rgb;
    // radial fringe
    vec2 dc = uv - 0.5;
    float ab = aberration * 0.0025 * dot(dc, dc) * 4.0;
    col.r = texture2D(fScene, uv + dc * ab * 4.0).r;
    col.b = texture2D(fScene, uv - dc * ab * 4.0).b;
    // bloom: dense golden-angle spiral on the quarter-res prefilter (wide + tight)
    vec2 bpx = 4.0 / R;
    vec3 b = vec3(0.0);
    float wsum = 0.0;
    for (int i = 0; i < 32; i++) {
        float fi = float(i);
        float ang = fi * 2.39996323;
        float rr = (fi + 0.5) / 32.0;
        vec2 o = vec2(cos(ang), sin(ang)) * rr * rr * 14.0 * bpx;
        float w = exp(-rr * 2.5);
        b += texture2D(fBloom, uv + o).rgb * w;
        wsum += w;
    }
    b /= wsum;
    col += b * bloom * 0.9;

    col *= 1.0 + (0.22 * smoothstep(0.05, 0.85, audioBass) + 0.12 * clamp(audioLevel, 0.0, 1.0)) * audioReact;
    col = hueRot(col, hueShift);
    col = aces(col * 1.05);
    float l = lum(col);
    col = mix(vec3(l), col, saturation);
    // vignette (≤0.12)
    col *= 1.0 - 0.12 * smoothstep(0.35, 0.95, length(dc * vec2(R.x / R.y, 1.0)));
    // HD finisher
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    l = dot(col, vec3(0.299, 0.587, 0.114));
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    // grain
    float gr = fh12(gl_FragCoord.xy + fract(TIME * 13.7) * 91.0) - 0.5;
    col += gr * grain * 0.05;
    // triangular dither, 1.5 LSB, so the smooth sunrise gradient never posterises
    float d1 = fh12(gl_FragCoord.xy * 1.03 + fract(TIME * 7.1) * 57.0);
    float d2 = fh12(gl_FragCoord.xy * 0.97 + fract(TIME * 5.3) * 31.0 + 11.0);
    col += (d1 + d2 - 1.0) * (1.5 / 255.0);
    return vec4(clamp(col, 0.0, 1.0), transparentBg ? sc.a : 1.0);
}

void main() {
    if (PASSINDEX <= 3)      gl_FragColor = simPass();
    else if (PASSINDEX == 4) gl_FragColor = blurPass();
    else if (PASSINDEX == 5) gl_FragColor = scenePass();
    else if (PASSINDEX == 6) gl_FragColor = bloomPass();
    else                     gl_FragColor = finalPass();
}
