/*{
  "DESCRIPTION": "Physarum — a real slime mould. Hundreds of thousands of agents live in the image, one per pixel at most: each sniffs the chemical trail ahead, left and right, turns toward the strongest scent, steps forward and lays more trail, so a transport network self-organises with a true hierarchy — thick pulsing trunks linking the food patches by near-shortest paths, medium veins, a fine dendritic mesh at the foraging front, and coarse and fine regions that drift across the dish. Food patches appear, feed and deplete, then reappear elsewhere, so trunks reroute and abandoned routes fade into ghost grooves etched in wet black slate. Seen from a tilted camera with depth of field: molten gold veins with thin-film shoulders and white-hot cores, cytoplasm shuttle-streaming along the trunks and reversing every ~40 s, a teal capillary web at their feet, glossy amber food droplets, raking key light with soft shadows and studio reflections. Bass heats the cores and swells the food, the music breathes the glow, beats spawn bursts of new agents at the food, mids brighten the capillaries and highs make the agents sparkle — the network itself is left to the mould.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #1",
  "CATEGORIES": ["Generator", "3D", "Simulation", "Organic", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Agent Speed",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.9,  "GROUP": "Speed" },
    { "NAME": "drift",       "LABEL": "Camera Drift",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "foodSpeed",   "LABEL": "Food Wander",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Speed" },
    { "NAME": "morph",       "LABEL": "Morphology Drift",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "colorCycle",  "LABEL": "Color Cycle",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.0,  "GROUP": "Speed" },
    { "NAME": "sensorAngle", "LABEL": "Sensor Angle",      "TYPE": "float", "AUDIO": false, "MIN": 8.0,  "MAX": 90.0, "DEFAULT": 40.0, "GROUP": "Physarum" },
    { "NAME": "sensorDist",  "LABEL": "Sensor Distance",   "TYPE": "float", "AUDIO": false, "MIN": 2.0,  "MAX": 30.0, "DEFAULT": 10.0, "GROUP": "Physarum" },
    { "NAME": "turnAngle",   "LABEL": "Turn Angle",        "TYPE": "float", "AUDIO": false, "MIN": 5.0,  "MAX": 90.0, "DEFAULT": 45.0, "GROUP": "Physarum" },
    { "NAME": "density",     "LABEL": "Agent Density",     "TYPE": "float", "AUDIO": false, "MIN": 0.05, "MAX": 0.7,  "DEFAULT": 0.06, "GROUP": "Physarum" },
    { "NAME": "decay",       "LABEL": "Trail Persistence", "TYPE": "float", "AUDIO": false, "MIN": 0.7,  "MAX": 0.99, "DEFAULT": 0.85, "GROUP": "Physarum" },
    { "NAME": "diffusion",   "LABEL": "Trail Diffusion",   "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55, "GROUP": "Physarum" },
    { "NAME": "foodCount",   "LABEL": "Food Sources",      "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 6.0,  "DEFAULT": 5.0,  "GROUP": "Physarum" },
    { "NAME": "foodPull",    "LABEL": "Food Pull",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Physarum" },
    { "NAME": "simRes",      "LABEL": "Sim Resolution",    "TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Physarum" },
    { "NAME": "zoom",        "LABEL": "Zoom",              "TYPE": "float", "AUDIO": false, "MIN": 1.0,  "MAX": 3.0,  "DEFAULT": 1.45, "GROUP": "Relief" },
    { "NAME": "tilt",        "LABEL": "Camera Tilt",       "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 40.0, "DEFAULT": 30.0, "GROUP": "Relief" },
    { "NAME": "coarseMix",   "LABEL": "Coarse / Fine Regions", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Physarum" },
    { "NAME": "streaming",   "LABEL": "Shuttle Streaming", "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Relief" },
    { "NAME": "height",      "LABEL": "Relief Height",     "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Relief" },
    { "NAME": "ghost",       "LABEL": "Ghost Routes",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Relief" },
    { "NAME": "sparkAmt",    "LABEL": "Agent Sparks",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Relief" },
    { "NAME": "veinColor",   "LABEL": "Vein Metal",        "TYPE": "color", "DEFAULT": [1.0, 0.68, 0.26, 1.0], "GROUP": "Color" },
    { "NAME": "coreColor",   "LABEL": "Hot Core",          "TYPE": "color", "DEFAULT": [1.0, 0.42, 0.08, 1.0], "GROUP": "Color" },
    { "NAME": "capColor",    "LABEL": "Capillary Neon",    "TYPE": "color", "DEFAULT": [0.05, 0.85, 0.8, 1.0], "GROUP": "Color" },
    { "NAME": "backColor",   "LABEL": "Slate",             "TYPE": "color", "DEFAULT": [0.012, 0.014, 0.02, 1.0], "GROUP": "Color" },
    { "NAME": "glowAmt",     "LABEL": "Core Glow",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Color" },
    { "NAME": "capAmt",      "LABEL": "Capillary Glow",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "iridescence", "LABEL": "Iridescence",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7,  "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",   "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" },
    { "NAME": "lightAz",     "LABEL": "Light Direction",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 360.0, "DEFAULT": 225.0, "GROUP": "Light" },
    { "NAME": "lightEl",     "LABEL": "Light Height",      "TYPE": "float", "AUDIO": false, "MIN": 8.0, "MAX": 80.0,  "DEFAULT": 30.0,  "GROUP": "Light" },
    { "NAME": "keyLight",    "LABEL": "Key Light",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "shadowAmt",   "LABEL": "Shadows",           "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.8,  "GROUP": "Light" },
    { "NAME": "specAmt",     "LABEL": "Specular",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "envAmt",      "LABEL": "Reflections",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.2,  "GROUP": "Finish" },
    { "NAME": "dof",         "LABEL": "Depth of Field",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" }
  ],
  "PASSES": [
    { "TARGET": "inA", "WIDTH": "min(floor($WIDTH*$simRes),1600)", "HEIGHT": "min(floor($HEIGHT*$simRes),900)" },
    { "TARGET": "agA", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1600)", "HEIGHT": "min(floor($HEIGHT*$simRes),900)" },
    { "TARGET": "trA", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1600)", "HEIGHT": "min(floor($HEIGHT*$simRes),900)" },
    { "TARGET": "inB", "WIDTH": "min(floor($WIDTH*$simRes),1600)", "HEIGHT": "min(floor($HEIGHT*$simRes),900)" },
    { "TARGET": "agB", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1600)", "HEIGHT": "min(floor($HEIGHT*$simRes),900)" },
    { "TARGET": "trB", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1600)", "HEIGHT": "min(floor($HEIGHT*$simRes),900)" },
    { "TARGET": "scene" },
    { "TARGET": "bloomBuf", "WIDTH": "floor($WIDTH/2)", "HEIGHT": "floor($HEIGHT/2)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Physarum — FRONTIER pack #1
//   The slime mould is a per-pixel AGENT CELLULAR AUTOMATON: each sim texel
//   holds at most one agent (rg = position inside the cell, b = heading,
//   a = alive). A fragment shader cannot scatter, so movement is a GATHER:
//     · an agent that wants to leave its cell may only enter a cell that was
//       empty last frame; when several agents target the same empty cell the
//       one with the highest per-frame hash priority wins, losers stay put
//       and pick a new heading (Jones' rule) — agents are conserved;
//     · every fragment re-evaluates its 3x3 (or its target's 3x3) neighbours
//       with the same deterministic sense/turn/step function, so the leaving
//       cell and the receiving cell always agree.
//   passes 0-5  inA → agA → trA → inB → agB → trB : two intent/move/trail substeps
//               trail: r = scent, g = food attractant, b = 1 + slow memory (ghost routes; 0 = uninitialised), a = smooth body
//   pass 6      scene : trail → 3D relief, gold/neon materials, shadows, sparks
//   pass 7      bloomBuf : half-res bright gather
//   pass 8      final : bloom, fringe, ACES, grain, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define TAU 6.28318530718
#define PI  3.14159265359

float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

// ── shared state ─────────────────────────────────────────────────────────
float g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev;
vec2  g_simR;
float g_SA, g_SO, g_RA, g_step, g_seed;

void setupAudio() {
    g_ar   = audioReact;
    g_bass = smoothstep(0.04, 0.85, audioBass);
    g_mid  = smoothstep(0.05, 0.85, audioMid);
    g_high = smoothstep(0.08, 0.90, audioHigh);
    g_beat = clamp(audioBeatPulse, 0.0, 1.0);
    g_lev  = clamp(audioLevel, 0.0, 1.0);
}

// ── food sources: 6 nutrient patches in sim-uv space (torus). Each lives a
//    cycle: appears, feeds, depletes, then reappears somewhere new — trunks
//    reroute and abandoned veins fade into ghost grooves.
void foodInfo(float i, out vec2 pos, out float str) {
    float P = 44.0 + 9.0 * i;
    float ph = (TIME * (0.35 + 0.65 * foodSpeed) + i * 0.41 * P + 6.0) / P;
    float cyc = floor(ph), f = fract(ph);
    str = smoothstep(0.0, 0.10, f) * (1.0 - smoothstep(0.70, 0.92, f));
    str *= clamp(foodCount - i, 0.0, 1.0);
    vec2 base = hash22(vec2(i * 7.13 + 1.0, cyc * 3.71 + 2.0));
    float ft = TIME * 0.02 * foodSpeed + i * 5.0;
    pos = base + 0.035 * vec2(sin(ft * 1.3 + i), cos(ft * 0.9 + i * 2.0));
}
// wrapped distance in aspect-correct units (1 = sim height)
float foodDistP(vec2 uv, vec2 fp) {
    vec2 d = fract(uv - fp + 0.5) - 0.5;
    d.x *= g_simR.x / g_simR.y;
    return length(d);
}
// near = the patch itself, wide = long-range chemoattractant gradient
void foodFields(vec2 uv, out float near, out float wide) {
    near = 0.0; wide = 0.0;
    for (int k = 0; k < 6; k++) {
        vec2 fp; float st;
        foodInfo(float(k), fp, st);
        float d = foodDistP(uv, fp);
        near += st * exp(-d * d * 70.0);
        wide += st * exp(-d * d * 14.0);
    }
}
float foodField(vec2 uv) { float n, w; foodFields(uv, n, w); return n; }
// coarse/fine regions: a slow periodic field scales sensor distance per place
float soField(vec2 uv) {
    float t = TIME * 0.006;
    float n = 0.5 + 0.25 * (sin(TAU * (uv.x * 2.0 + t) + 1.3 * sin(TAU * uv.y * 2.0))
                         + sin(TAU * (uv.y * 1.0 - t * 0.7) + 1.1 * sin(TAU * (uv.x * 1.0 + uv.y))));
    return mix(1.0, mix(0.55, 2.1, smoothstep(0.2, 0.8, n)), coarseMix);
}

// ── tileable seed network: warped periodic Voronoi edges, two scales ─────
float vorEdge(vec2 uv, vec2 per) {
    vec2 p = uv * per;
    vec2 ip = floor(p), fp = fract(p);
    float d1 = 9.0, d2 = 9.0;
    vec2 m1 = vec2(0.0);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 g = vec2(float(i), float(j));
        vec2 c = mod(ip + g, per);
        vec2 o = g + 0.15 + 0.7 * hash22(c * 1.37 + per.x) - fp;
        float d = dot(o, o);
        if (d < d1) { d2 = d1; d1 = d; m1 = o; } else if (d < d2) { d2 = d; }
    }
    // second pass: true distance to the nearest bisector
    float e = 9.0;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 g = vec2(float(i), float(j));
        vec2 c = mod(ip + g, per);
        vec2 o = g + 0.15 + 0.7 * hash22(c * 1.37 + per.x) - fp;
        vec2 dd = o - m1;
        if (dot(dd, dd) > 1e-5) e = min(e, dot(0.5 * (m1 + o), normalize(dd)));
    }
    return e / per.y;   // in sim-height units
}
float seedNet(vec2 uv) {
    vec2 w = uv + 0.012 * vec2(sin(TAU * (uv.y * 3.0 + uv.x)), sin(TAU * (uv.x * 2.0 - uv.y * 2.0)));
    float big = vorEdge(w, vec2(9.0, 5.0));
    float sml = vorEdge(w + 0.13, vec2(23.0, 13.0));
    float px = 1.0 / g_simR.y;
    return exp(-big * big / (px * px * 7.0)) + 0.5 * exp(-sml * sml / (px * px * 2.0));
}

// ═════════════════════════════════════════════════════════════════════════
//  AGENT CA — two phases per substep so every decision is made ONCE:
//    INTENT (passes 0, 3): each agent senses, turns and writes the integer
//      direction code of the cell it wants (w = 1 + k, k = (dx+1)+3(dy+1),
//      4 = stay; w = 0 empty). xy = position inside its cell, z = heading.
//    MOVE   (passes 1, 4): pure integer bookkeeping on the stored codes —
//      a move succeeds only into a cell that is empty in the intent buffer;
//      among agents aiming at the same cell the winner is the one whose
//      direction index has the highest frame-rotated rank (a bijection, so
//      never a tie). Leaving and receiving cells read identical integers, so
//      agents are exactly conserved (re-evaluating floats at two call sites
//      let GPU fma contraction disagree at cell borders and agents multiplied).
// ═════════════════════════════════════════════════════════════════════════
float g_rank;   // frame-rotated rank offset (integer 0..8)

vec4 agPrev(vec2 cell) {            // agents at the start of this substep
    vec2 uv = (mod(cell, g_simR) + 0.5) / g_simR;
    if (PASSINDEX == 0) return texture2D(agB, uv);
    return texture2D(agA, uv);
}
vec4 intentAt(vec2 cell) {
    vec2 uv = (mod(cell, g_simR) + 0.5) / g_simR;
    if (PASSINDEX == 1) return texture2D(inA, uv);
    return texture2D(inB, uv);
}
vec4 trailSrc(vec2 uv) {
    if (PASSINDEX == 0) return texture2D(trB, uv);
    return texture2D(trA, uv);
}
float sense(vec2 pos) {
    vec2 uv = fract(pos / g_simR);
    vec4 t = trailSrc(uv);
    return t.r + t.g;
}
vec2 dirOf(float a) { return vec2(cos(a), sin(a)); }
vec2 codeDelta(float w) {           // w = 1 + k
    float k = floor(w - 1.0 + 0.5);
    float dx = mod(k, 3.0) - 1.0;
    float dy = floor(k / 3.0 + 0.001) - 1.0;
    return vec2(dx, dy);
}
float rankOf(vec2 rel) {            // rel = candidate cell - target cell, never (0,0)
    float k = (rel.x + 1.0) + 3.0 * (rel.y + 1.0);
    return mod(k + g_rank, 9.0);
}

vec4 intentPass(vec2 p) {
    vec2 uv = (p + 0.5) / g_simR;
    bool uninit = (PASSINDEX == 0) && (texture2D(trB, uv).b < 0.5 || FRAMEINDEX < 1);
    if (uninit) {
        float sN = seedNet(uv);
        float pr = density * 0.3 + 0.75 * clamp(sN, 0.0, 1.0);
        if (hash12(p * 0.71 + 3.3) < pr) return vec4(hash22(p * 1.13 + 7.7), hash12(p + 91.0), 5.0);
        return vec4(0.0);
    }
    vec4 a = agPrev(p);
    // population turnover is decided HERE (before the move) so leaving and receiving cells agree
    float life = hash12(p * 0.618 + vec2(g_seed * 0.7548, g_seed * 0.5698) + 4.0);
    if (a.w > 0.5 && life < 0.002) return vec4(0.0);
    if (a.w < 0.5) {
        // birth: keeps the population near `density`, favoured at the food, beats burst
        // agents are born at the food and stream out along the trunks; far fields thin out
        float food = foodField(uv);
        float fb = food * food; fb *= fb;          // tight: agents emerge right at the patch
        float birth = 0.002 * density / max(1.0 - density, 0.05) * (0.12 + 24.0 * fb) / (0.12 + 24.0 * 0.004 * foodCount);
        birth += g_ar * g_beat * food * 0.01;
        if (life < birth)
            return vec4(hash22(p * 1.91 + g_seed), hash12(p * 0.37 + g_seed + 2.0), 5.0);
        return vec4(0.0);
    }
    vec2 pos = p + a.xy;
    float ang = a.z * TAU;
    float so = g_SO * soField(uv);
    float c = sense(pos + so * dirOf(ang));
    float l = sense(pos + so * dirOf(ang + g_SA));
    float r = sense(pos + so * dirOf(ang - g_SA));
    float h = hash12(p * 1.731 + g_seed);
    if (c >= l && c >= r) {
    } else if (c < l && c < r) {
        ang += (h < 0.5 ? 1.0 : -1.0) * g_RA;
    } else if (l > r) {
        ang += g_RA;
    } else {
        ang -= g_RA;
    }
    ang += (hash12(p * 2.913 + g_seed + 5.1) - 0.5) * 0.08;
    vec2 np = a.xy + dirOf(ang) * g_step;          // relative to own cell
    vec2 d = clamp(floor(np), -1.0, 1.0);
    float k = (d.x + 1.0) + 3.0 * (d.y + 1.0);
    return vec4(a.xy, fract(ang / TAU), 1.0 + k);
}

vec4 movePass(vec2 p) {
    vec4 me = intentAt(p);
    float rnd = hash12(p * 3.17 + g_seed + 23.0);
    if (me.w > 0.5) {
        vec2 d = codeDelta(me.w);
        float ang = me.z * TAU;
        if (d.x == 0.0 && d.y == 0.0) {
            vec2 off = clamp(me.xy + dirOf(ang) * g_step, 0.0, 0.999);
            return vec4(off, me.z, 1.0);
        }
        vec2 t = mod(p + d, g_simR);
        bool win = false;
        if (intentAt(t).w < 0.5) {
            float myRank = rankOf(-d);
            win = true;
            for (int j = -1; j <= 1; j++)
            for (int i = -1; i <= 1; i++) {
                if (i == 0 && j == 0) continue;
                vec2 rel = vec2(float(i), float(j));
                if (rel.x == -d.x && rel.y == -d.y) continue;   // that's me
                vec4 o = intentAt(t + rel);
                if (o.w < 0.5) continue;
                vec2 od = codeDelta(o.w);
                if (od.x == -rel.x && od.y == -rel.y && rankOf(rel) > myRank) win = false;
            }
        }
        if (win) return vec4(0.0);                 // moved away
        // blocked: stay, pick a new heading
        return vec4(me.xy, fract(me.z + 0.25 + 0.5 * hash12(p + g_seed * 3.1)), 1.0);
    }
    // empty: adopt the highest-ranked agent aiming here
    float best = -1.0; vec4 got = vec4(0.0);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        if (i == 0 && j == 0) continue;
        vec2 rel = vec2(float(i), float(j));
        vec4 o = intentAt(p + rel);
        if (o.w < 0.5) continue;
        vec2 od = codeDelta(o.w);
        if (od.x != -rel.x || od.y != -rel.y) continue;
        float rk = rankOf(rel);
        if (rk > best) {
            best = rk;
            float ang = o.z * TAU;
            vec2 off = clamp(o.xy + dirOf(ang) * g_step + rel, 0.0, 0.999);
            got = vec4(off, o.z, 1.0);
        }
    }
    return got;
}

// ═════════════════════════════════════════════════════════════════════════
//  TRAIL  (passes 2, 5)
// ═════════════════════════════════════════════════════════════════════════
vec4 trailPrev(vec2 uv) {
    if (PASSINDEX == 2) return texture2D(trB, uv);
    return texture2D(trA, uv);
}
float agentNow(vec2 uv) {
    if (PASSINDEX == 2) return texture2D(agA, uv).w;
    return texture2D(agB, uv).w;
}
vec4 trailPass(vec2 p) {
    vec2 uv = (p + 0.5) / g_simR;
    vec4 prev = trailPrev(uv);
    if (prev.b < 0.5 || FRAMEINDEX < 1) {
        float sN = 14.0 * seedNet(uv);
        return vec4(sN, 0.0, 1.0 + sN * 0.6, sN);
    }
    vec2 px = 1.0 / g_simR;
    float sum = 0.0, sumA = 0.0;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        float w = (i == 0 && j == 0) ? 4.0 : ((i == 0 || j == 0) ? 2.0 : 1.0);
        vec4 tp = trailPrev(fract(uv + vec2(float(i), float(j)) * px));
        sum += w * tp.r; sumA += w * tp.a;
    }
    float blur = sum / 16.0;
    float blurA = sumA / 16.0;
    float fn, fw; foodFields(uv, fn, fw);
    // stronger deposit + slower decay near food → thick trunks between patches
    float dep = step(0.5, agentNow(uv)) * (1.0 + 0.5 * fw);
    float dec = mix(decay, min(decay + 0.05, 0.97), clamp(fw, 0.0, 1.0));
    float tr = mix(prev.r, blur, diffusion) * dec + dep + fn * fn * 0.3;
    tr = min(tr, 60.0);
    float mem = mix(prev.b - 1.0, tr, 0.0025);
    // smooth body: the agent-granular scent relaxed into a tube profile (render only)
    float body = mix(blurA, tr, 0.08);
    // g = food chemoattractant, evaluated once per texel here so agents only read it
    float attract = foodPull * (2.5 * fn + 1.4 * fw);
    return vec4(tr, attract, 1.0 + mem, body);
}

// ═════════════════════════════════════════════════════════════════════════
//  SCENE  (pass 4)
// ═════════════════════════════════════════════════════════════════════════
vec2 g_cen; float g_zoom, g_hs;
vec3 g_L;

// cubic B-spline sample of the final trail buffer (4 bilinear taps), wrapped
vec4 trailSmooth(vec2 uv) {
    vec2 st = uv * g_simR - 0.5;
    vec2 i = floor(st), f = st - i;
    vec2 f2 = f * f, f3 = f2 * f;
    vec2 w0 = (1.0 - 3.0 * f + 3.0 * f2 - f3) / 6.0;
    vec2 w1 = (4.0 - 6.0 * f2 + 3.0 * f3) / 6.0;
    vec2 w2 = (1.0 + 3.0 * f + 3.0 * f2 - 3.0 * f3) / 6.0;
    vec2 w3 = f3 / 6.0;
    vec2 s0 = w0 + w1, s1 = w2 + w3;
    vec2 h0 = (i - 0.5 + w1 / s0) / g_simR;
    vec2 h1 = (i + 1.5 + w3 / s1) / g_simR;
    vec4 a = texture2D(trB, fract(vec2(h0.x, h0.y)));
    vec4 b = texture2D(trB, fract(vec2(h1.x, h0.y)));
    vec4 c = texture2D(trB, fract(vec2(h0.x, h1.y)));
    vec4 d = texture2D(trB, fract(vec2(h1.x, h1.y)));
    return mix(mix(a, b, s1.x / (s0.x + s1.x)), mix(c, d, s1.x / (s0.x + s1.x)), s1.y / (s0.y + s1.y));
}

float shapeH(float tr) { return 1.0 - exp(-max(tr - 1.2, 0.0) * 0.2); }

float foodDome(vec2 uv) {
    float h = 0.0;
    for (int k = 0; k < 6; k++) {
        float fk = float(k);
        vec2 fp; float st;
        foodInfo(fk, fp, st);
        float rad = 0.024 * sqrt(st) * (1.0 + 0.12 * g_ar * g_bass) * (0.8 + 0.4 * hash12(vec2(fk, 3.0)));
        float d = foodDistP(uv, fp) / max(rad, 1e-4);
        h = max(h, step(0.02, st) * sqrt(max(1.0 - d * d, 0.0)));
    }
    return h;
}
float nearestFood(vec2 uv) {
    float dm = 9.0;
    for (int k = 0; k < 6; k++) {
        vec2 fp; float st;
        foodInfo(float(k), fp, st);
        dm = min(dm, foodDistP(uv, fp) + (1.0 - step(0.05, st)) * 9.0);
    }
    return dm;
}

// height used for normals/shadows: scent relief + faint ghost + food domes
float heightAt(vec2 uv, out vec4 tr) {
    tr = trailSmooth(uv);
    float h = shapeH(tr.a) + 0.10 * ghost * shapeH((tr.b - 1.0) * 1.4);
    // thick veins keep rising toward their spine (log, never saturates) → rounded tubes
    h += 0.22 * log(1.0 + 0.12 * max(tr.a - 8.0, 0.0));
    return max(h, foodDome(uv) * 1.25);
}
float heightCheap(vec2 uv) {
    vec4 t = texture2D(trB, fract(uv));
    return shapeH(t.a) + 0.10 * ghost * shapeH((t.b - 1.0) * 1.4);
}

vec3 envMap(vec3 r) {
    // dark studio: two long softbox strips + magenta / cyan lamps
    vec3 c = vec3(0.004, 0.005, 0.008) * (0.5 + 0.5 * r.z);
    float s1 = smoothstep(0.10, 0.02, abs(dot(r.xy, vec2(0.80, 0.60)) - 0.45));
    float s2 = smoothstep(0.08, 0.015, abs(dot(r.xy, vec2(-0.55, 0.83)) + 0.30));
    c += vec3(1.0, 0.95, 0.88) * (s1 * 2.2 + s2 * 1.2) * smoothstep(0.0, 0.25, r.z);
    c += vec3(1.0, 0.12, 0.55) * pow(max(dot(r, normalize(vec3(-0.7, 0.45, 0.55))), 0.0), 24.0) * 0.9;
    c += vec3(0.10, 0.65, 1.0) * pow(max(dot(r, normalize(vec3(0.65, -0.6, 0.45))), 0.0), 20.0) * 2.0;
    return c;
}
vec3 thinFilm(float th, float ct) {
    float ph = th * (2.0 - ct) * 6.0;
    return 0.5 + 0.5 * cos(TAU * (ph + vec3(0.0, 0.33, 0.67)));
}

// tilted camera over the dish: screen pixel → sim uv on the z=0 plane
vec2 projUV(vec2 fc, vec2 R, out vec3 rd) {
    float f = 0.5;
    vec2 p = (fc - 0.5 * R) / R.y * 2.0 * f;
    float t = radians(tilt);
    vec3 d = vec3(p.x, p.y * cos(t) + sin(t), p.y * sin(t) - cos(t));
    float Hc = cos(t) / (2.0 * f * g_zoom);
    vec3 cam = vec3(0.0, -Hc * tan(t), Hc);
    float s = cam.z / max(-d.z, 1e-3);
    vec2 w = cam.xy + s * d.xy;
    rd = normalize(d);
    return g_cen + vec2(w.x * g_simR.y / g_simR.x, w.y);
}

vec4 scenePass(vec2 fragCoord, vec2 R) {
    vec3 rd, rdx;
    vec2 uv = projUV(fragCoord, R, rd);
    vec2 ux = projUV(fragCoord + vec2(1.0, 0.0), R, rdx) - uv;
    vec2 uy = projUV(fragCoord + vec2(0.0, 1.0), R, rdx) - uv;
    vec2 asp2 = vec2(g_simR.x / g_simR.y, 1.0);
    float upp = sqrt(length(ux * asp2) * length(uy * asp2));   // sim-height units per screen px
    vec2 e = vec2(0.85 / g_simR.y) * vec2(g_simR.y / g_simR.x, 1.0);  // ~0.85 sim texel

    vec4 tr;
    float h  = heightAt(uv, tr);
    vec4 dum;
    float hx1 = heightAt(uv + vec2(e.x, 0.0), dum), hx0 = heightAt(uv - vec2(e.x, 0.0), dum);
    float hy1 = heightAt(uv + vec2(0.0, e.y), dum), hy0 = heightAt(uv - vec2(0.0, e.y), dum);
    // relief scale: height units vs. pixels
    float k = g_hs * 4.0;
    vec3 n = normalize(vec3(-(hx1 - hx0) * k, -(hy1 - hy0) * k, 1.0));

    // micro grain on the slate (crisp high-frequency normal)
    vec2 gp = uv * g_simR * 1.8;
    float g0 = hash12(floor(gp)), gx = hash12(floor(gp) + vec2(1.0, 0.0)), gy = hash12(floor(gp) + vec2(0.0, 1.0));
    // crisp AA edges: level sets measured in screen pixels via the local gradient
    float ePx = (0.85 / g_simR.y) / upp;
    float gpx = max(length(vec2(hx1 - hx0, hy1 - hy0)) / (2.0 * ePx), 1e-4);
    float vein = clamp((h - 0.16) / (gpx * 1.2) + 0.5, 0.0, 1.0);
    vec3 ng = normalize(n + vec3(g0 - gx, g0 - gy, 0.0) * 0.12 * (1.0 - vein));

    vec3 V = -rd;
    vec3 L = g_L;
    vec3 H = normalize(L + V);
    float ndl = max(dot(ng, L), 0.0);

    // soft shadow: march toward the light across the height map
    float sh = 1.0;
    {
        vec2 ld = normalize(L.xy) * vec2(g_simR.y / g_simR.x, 1.0);
        float tanE = L.z / max(length(L.xy), 1e-3);
        float tt = 1.2 / g_simR.y;
        for (int s = 0; s < 10; s++) {
            float hs = heightCheap(uv + ld * tt);
            float rayH = h + tt * tanE * g_simR.y / (k * 1.0);
            sh = min(sh, clamp(1.0 - (hs - rayH) * 5.0, 0.0, 1.0));
            tt *= 1.5;
        }
        sh = mix(1.0, sh, shadowAmt);
    }
    // AO from local height deficit
    float hb = 0.25 * (heightCheap(uv + vec2(6.0 * e.x, 0.0)) + heightCheap(uv - vec2(6.0 * e.x, 0.0)) +
                       heightCheap(uv + vec2(0.0, 6.0 * e.y)) + heightCheap(uv - vec2(0.0, 6.0 * e.y)));
    float ao = clamp(1.0 - max(hb - h, 0.0) * 2.2, 0.35, 1.0);

    float cyc = colorCycle * TIME * 0.02 + hueShift;
    vec3 gold = hueRot(veinColor.rgb, cyc);
    vec3 hot  = hueRot(coreColor.rgb, cyc);
    vec3 cap  = hueRot(capColor.rgb, cyc);

    float ct = max(dot(n, V), 0.0);
    float slope = 1.0 - ct;
    float fres = pow(1.0 - ct, 5.0);
    vec3 R3 = reflect(-V, ng);

    // ── slate (background) ──
    vec3 slate = backColor.rgb * (0.4 + 0.6 * ndl) * sh;
    slate += envMap(R3) * 0.035 * envAmt * (0.4 + fres * 3.0);
    slate += vec3(1.0) * pow(max(dot(ng, H), 0.0), 120.0) * 0.25 * specAmt * keyLight * sh;
    // ghost grooves: abandoned routes as dark bronze channels with a faint teal edge
    float gh = shapeH((tr.b - 1.0) * 1.4);
    float ghostLine = smoothstep(0.05, 0.25, gh) * (1.0 - vein);
    slate = mix(slate, gold * 0.05 * (0.4 + ndl) * sh, ghostLine * 0.8 * clamp(ghost, 0.0, 1.0));
    slate += cap * 0.05 * smoothstep(0.03, 0.1, gh) * (1.0 - smoothstep(0.1, 0.3, gh)) * ghost * (1.0 - vein);

    // ── gold vein metal ──
    vec3 film = thinFilm(0.35 + h * 1.6 + (tr.b - 1.0) * 0.01, ct);
    vec3 F0 = mix(gold, gold * film * 1.4, clamp(iridescence * smoothstep(0.1, 0.6, slope), 0.0, 1.0));
    vec3 F = F0 + (1.0 - F0) * fres;
    vec3 metal = F * envMap(R3) * envAmt * 1.1;
    metal += F * vec3(1.0, 0.95, 0.85) * pow(max(dot(ng, H), 0.0), 70.0) * 3.0 * specAmt * keyLight * sh;
    metal += gold * 0.22 * ndl * keyLight * sh;
    metal += F * pow(max(dot(ng, H), 0.0), 10.0) * 0.45 * keyLight * sh;
    metal *= ao;

    vec3 col = mix(slate, metal, vein);

    // ── emission ──
    float bassHeat = 1.0 + 0.55 * g_ar * g_bass;
    // white-hot only along vein SPINES (above their surroundings), so hubs stay molten gold
    float spine = smoothstep(0.04, 0.28, h - hb);
    float coreM = smoothstep(0.5, 0.9, h) * (1.0 - smoothstep(1.0, 1.2, h)) * spine;
    float flick = 0.75 + 0.25 * sin(tr.r * 0.9 - TIME * 3.0 + uv.x * 40.0);
    col += hot * coreM * coreM * 2.4 * glowAmt * bassHeat * flick;
    col += mix(hot, vec3(1.0, 0.92, 0.75), 0.6) * pow(smoothstep(0.80, 0.98, h), 3.0) * spine * 1.6 * glowAmt * bassHeat * (1.0 - smoothstep(1.0, 1.2, h));
    // shuttle streaming: cytoplasm surges out from / back to the food along the
    // thick veins, reversing direction every ~40 s (Physarum's ~1-2 min rhythm)
    {
        float dF = nearestFood(uv);
        float flow = 14.0 * sin(TIME * TAU / 80.0) + TIME * 0.4;
        float wave = 0.5 + 0.5 * sin(dF * 60.0 - flow);
        float pulse = pow(wave, 5.0) * (0.6 + 0.4 * sin(dF * 23.0 - flow * 0.37));
        float trunkM = smoothstep(0.45, 0.85, h) * vein;
        col += mix(hot, vec3(1.0, 0.9, 0.7), 0.35) * trunkM * pulse * 1.6 * streaming * glowAmt * bassHeat;
        col *= 1.0 + trunkM * (wave - 0.5) * 0.25 * streaming;
    }
    // teal capillaries: weak scent and the feet of every vein
    float capLine = 1.0 - clamp(abs(h - 0.07) / (gpx * 0.9), 0.0, 1.0);
    float capFill = smoothstep(0.015, 0.07, h) * (1.0 - vein);
    float capM = max(capLine * (1.0 - vein), capFill * 0.12);
    col += cap * (capLine * (1.0 - vein) * 1.6 + capFill * 0.10) * capAmt * (0.8 + 0.4 * g_ar * g_mid);

    // food droplets: glossy amber with inner glow
    float dome = foodDome(uv);
    if (dome > 0.0) {
        float dm = smoothstep(0.0, 0.08, dome);
        vec3 drop = hot * pow(dome, 2.0) * 3.0 * bassHeat + F * envMap(R3) * 0.35 + F * fres * 1.2;
        drop += vec3(1.0) * pow(max(dot(ng, H), 0.0), 160.0) * 4.0 * specAmt;
        col = mix(col, drop, dm);
    }

    // agent sparks: agents streaming down the veins (sub-cell positions)
    float sp = 0.0;
    {
        vec2 sc = uv * g_simR - 0.5;
        vec2 base = floor(sc);
        for (int j = 0; j <= 1; j++)
        for (int i = 0; i <= 1; i++) {
            vec2 c = base + vec2(float(i), float(j));
            vec4 a = texture2D(agB, (mod(c, g_simR) + 0.5) / g_simR);
            if (a.a < 0.5) continue;
            vec2 d = (sc + 0.5 - (c + a.xy)) / (upp * g_simR.y);   // screen px
            float tw = hash12(c + floor(TIME * 8.0));
            sp += exp(-dot(d, d) * 1.4) * (0.3 + 0.7 * tw);
        }
    }
    col += mix(vec3(1.0, 0.85, 0.6), cap, 0.25) * sp * sparkAmt * (0.35 + 0.65 * smoothstep(0.08, 0.5, h)) * (0.5 + 0.8 * g_ar * g_high);

    col = mix(vec3(lum(col)), col, saturation);
    float cover = clamp(vein + capM + dome + ghostLine * 0.5, 0.0, 1.0);
    return vec4(max(col, 0.0), cover);
}

// ═════════════════════════════════════════════════════════════════════════
void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 R = RENDERSIZE.xy;
    setupAudio();
    g_t = TIME;

    if (PASSINDEX <= 5) {
        g_simR = R;
        vec2 p = floor(fragCoord);
        float sub = (PASSINDEX >= 3) ? 1.0 : 0.0;
        g_rank = mod(mod(float(FRAMEINDEX), 9.0) * 2.0 + sub * 4.0, 9.0);
        g_seed = mod(float(FRAMEINDEX), 977.0) * 2.0 + sub + 1.0;
        float mt = TIME * 0.045 * morph;
        float scl = g_simR.y / 540.0;                 // feature size follows resolution
        g_SA = radians(sensorAngle + 9.0 * sin(mt));
        g_SO = sensorDist * scl * (1.2 + 0.35 * sin(mt * 0.61 + 1.0));
        g_RA = radians(turnAngle + 6.0 * sin(mt * 0.83 + 2.0));
        g_step = clamp(speed, 0.0, 0.98);
        if (PASSINDEX == 0 || PASSINDEX == 3) gl_FragColor = intentPass(p);
        else if (PASSINDEX == 1 || PASSINDEX == 4) gl_FragColor = movePass(p);
        else gl_FragColor = trailPass(p);
        return;
    }

    if (PASSINDEX == 6) {
        g_simR = min(floor(R * simRes), vec2(1600.0, 900.0));
        g_zoom = zoom * (1.0 + 0.04 * sin(TIME * 0.05));
        g_cen = vec2(0.5) + vec2(TIME * 0.0021 * drift, TIME * 0.0013 * drift)
              + 0.03 * drift * vec2(sin(TIME * 0.031), cos(TIME * 0.023));
        g_hs = height * (1.0 + 0.15 * g_ar * g_bass);
        float az = radians(lightAz), el = radians(lightEl);
        g_L = normalize(vec3(cos(az) * cos(el), sin(az) * cos(el), sin(el)));
        gl_FragColor = scenePass(fragCoord, R);
        return;
    }

    if (PASSINDEX == 7) {
        vec2 st = fragCoord / R;
        vec2 px = 1.0 / R;
        vec3 acc = vec3(0.0); float wsum = 0.0;
        for (int j = -2; j <= 2; j++)
        for (int i = -2; i <= 2; i++) {
            vec2 o = vec2(float(i), float(j));
            float w = exp(-dot(o, o) * 0.35);
            vec3 s = texture2D(scene, st + o * px * 1.5).rgb;
            acc += max(s - 0.75, 0.0) * w; wsum += w;
        }
        gl_FragColor = vec4(acc / wsum, 1.0);
        return;
    }

    // ── final ────────────────────────────────────────────────────────────
    vec2 st = fragCoord / R;
    vec2 uvc = (fragCoord - 0.5 * R) / R.y;
    vec2 asp = vec2(R.y / R.x, 1.0);
    vec4 c0 = texture2D(scene, st);
    float ab = aberration * 0.0018 * length(uvc);
    vec2 dir = normalize(uvc + 1e-4);
    vec3 col = vec3(texture2D(scene, st + dir * ab * asp).r, c0.g, texture2D(scene, st - dir * ab * asp).b);

    // depth of field: the far (top) side of the tilted dish falls out of focus
    float dofR = dof * smoothstep(0.42, 1.0, st.y) * clamp(tilt / 30.0, 0.0, 1.3) * 7.0;
    if (dofR > 0.35) {
        vec3 acc = col; float wsum = 1.0;
        for (int i = 0; i < 12; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + hash12(fragCoord) * 6.2832;
            float rr = sqrt((fi + 0.5) / 12.0) * dofR;
            acc += texture2D(scene, st + vec2(cos(a), sin(a)) * rr / R).rgb; wsum += 1.0;
        }
        col = acc / wsum;
    }
    vec3 bl = texture2D(bloomBuf, st).rgb * 0.5;
    for (int i = 0; i < 12; i++) {
        float fi = float(i);
        float a = fi * 2.399963 + hash12(fragCoord) * 0.6;
        float rr = (0.004 + 0.022 * sqrt((fi + 0.5) / 12.0));
        vec2 of = vec2(cos(a), sin(a)) * rr * asp;
        bl += texture2D(bloomBuf, st + of).rgb * (1.0 - rr / 0.03);
    }
    bl /= 6.0;
    col += bl * bloom * 1.4;
    col = aces(col * 1.05);
    // music breathes the whole piece: linear post-tonemap lift that dips below 1 in quiet
    // passages (correlates on every style, never chops)
    col *= mix(1.0, 0.74 + 0.30 * g_lev + 0.26 * g_bass + 0.08 * g_beat, g_ar);
    float gr = (hash12(fragCoord + fract(TIME) * 61.0) - 0.5) * 0.045;
    col += gr * grain;
    col *= 1.0 - 0.10 * smoothstep(0.5, 1.4, length(uvc));
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float l = lum(col);
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    float alphaOut = transparentBg ? clamp(c0.a + lum(bl) * 2.0, 0.0, 1.0) : 1.0;
    gl_FragColor = vec4(col, alphaOut);
}
