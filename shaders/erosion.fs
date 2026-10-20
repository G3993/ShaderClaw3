/*{
  "DESCRIPTION": "Erosion — a volcanic island at dusk carved live by a real hydraulic erosion simulation: rain falls from drifting storm cells, water flows through a virtual-pipe shallow-water solver, picks up sediment where it runs fast and steep and drops it where it slows, landslides slump the over-steep walls and slow isostatic rebound keeps the mountains rising so the land never wears flat. Rivers branch into a dendritic network that keeps reorganising: dark glossy threads that mirror the dusk sky, a hairline of cyan light burning in the thalweg of every big trunk (molten amber where it runs heavy with silt), white-water cascades down the cliffs and braided shallow sheets across the deltas; still water settles into dark mirror lakes; deltas build out into a black sea. Cutting reveals basalt strata with iron and ash seams. A low camera circles the island under a magenta afterglow with long sun shadows, storm shadows and sun glitter on the sea. Bass pours harder rain and surges the rivers, beats crack lightning inside the storms, highs sparkle the water.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #9",
  "CATEGORIES": ["Generator", "3D", "Simulation", "Landscape", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Erosion Speed",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "orbitSpeed",  "LABEL": "Camera Orbit",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "stormDrift",  "LABEL": "Storm Drift",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "rain",        "LABEL": "Rain",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Erosion" },
    { "NAME": "storms",      "LABEL": "Storm Cells",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Erosion" },
    { "NAME": "erosionRate", "LABEL": "Carving",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Erosion" },
    { "NAME": "uplift",      "LABEL": "Tectonic Uplift",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Erosion" },
    { "NAME": "landslide",   "LABEL": "Landslides",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Erosion" },
    { "NAME": "simRes",      "LABEL": "Sim Resolution",   "TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Erosion" },
    { "NAME": "relief",      "LABEL": "Mountain Height",  "TYPE": "float", "AUDIO": false, "MIN": 0.4,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Terrain" },
    { "NAME": "seaLevel",    "LABEL": "Sea Level",        "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 0.4,  "DEFAULT": 0.13, "GROUP": "Terrain" },
    { "NAME": "strata",      "LABEL": "Strata Bands",     "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Terrain" },
    { "NAME": "flyHeight",   "LABEL": "Fly Height",       "TYPE": "float", "AUDIO": false, "MIN": 0.2,  "MAX": 2.5,  "DEFAULT": 0.8,  "GROUP": "Terrain" },
    { "NAME": "flyDistance", "LABEL": "Fly Distance",     "TYPE": "float", "AUDIO": false, "MIN": 0.4,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Terrain" },
    { "NAME": "rockA",       "LABEL": "Basalt",           "TYPE": "color", "DEFAULT": [0.035, 0.032, 0.04, 1.0], "GROUP": "Color" },
    { "NAME": "rockB",       "LABEL": "Iron Strata",      "TYPE": "color", "DEFAULT": [0.42, 0.11, 0.05, 1.0],   "GROUP": "Color" },
    { "NAME": "rockC",       "LABEL": "Sandstone",        "TYPE": "color", "DEFAULT": [0.74, 0.5, 0.29, 1.0],    "GROUP": "Color" },
    { "NAME": "mossColor",   "LABEL": "Valley Moss",      "TYPE": "color", "DEFAULT": [0.06, 0.16, 0.09, 1.0],   "GROUP": "Color" },
    { "NAME": "riverColor",  "LABEL": "River Glow",       "TYPE": "color", "DEFAULT": [0.1, 0.85, 1.0, 1.0],     "GROUP": "Color" },
    { "NAME": "siltColor",   "LABEL": "Silt Glow",        "TYPE": "color", "DEFAULT": [1.0, 0.45, 0.12, 1.0],    "GROUP": "Color" },
    { "NAME": "seaColor",    "LABEL": "Sea",              "TYPE": "color", "DEFAULT": [0.01, 0.05, 0.08, 1.0],   "GROUP": "Color" },
    { "NAME": "skyColor",    "LABEL": "Sky",              "TYPE": "color", "DEFAULT": [0.02, 0.03, 0.09, 1.0],   "GROUP": "Color" },
    { "NAME": "sunColor",    "LABEL": "Sun",              "TYPE": "color", "DEFAULT": [1.0, 0.55, 0.28, 1.0],    "GROUP": "Color" },
    { "NAME": "riverGlow",   "LABEL": "River Glow Amount","TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.2,  "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "keyLight",    "LABEL": "Sun Strength",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "sunAz",       "LABEL": "Sun Direction",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 360.0, "DEFAULT": 200.0, "GROUP": "Light" },
    { "NAME": "sunEl",       "LABEL": "Sun Height",       "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 70.0,  "DEFAULT": 13.0,  "GROUP": "Light" },
    { "NAME": "shadowAmt",   "LABEL": "Shadows",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.9,  "GROUP": "Light" },
    { "NAME": "cloudShadow", "LABEL": "Storm Shadows",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Light" },
    { "NAME": "lightning",   "LABEL": "Lightning",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "haze",        "LABEL": "Aerial Haze",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.18, "GROUP": "Finish" },
    { "NAME": "marchRes",    "LABEL": "March Resolution", "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.75, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG", "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "fl1", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1024)", "HEIGHT": "min(floor($HEIGHT*$simRes),576)" },
    { "TARGET": "st1", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1024)", "HEIGHT": "min(floor($HEIGHT*$simRes),576)" },
    { "TARGET": "fl2", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1024)", "HEIGHT": "min(floor($HEIGHT*$simRes),576)" },
    { "TARGET": "st2", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1024)", "HEIGHT": "min(floor($HEIGHT*$simRes),576)" },
    { "TARGET": "fl3", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1024)", "HEIGHT": "min(floor($HEIGHT*$simRes),576)" },
    { "TARGET": "st3", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1024)", "HEIGHT": "min(floor($HEIGHT*$simRes),576)" },
    { "TARGET": "net", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),1024)", "HEIGHT": "min(floor($HEIGHT*$simRes),576)" },
    { "TARGET": "scene", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Erosion — FRONTIER pack #9
//   passes 0-5  fl1 st1 fl2 st2 fl3 st3 : three hydraulic-erosion steps per
//               frame (virtual-pipe shallow water). flux pass = outflow to the
//               4 neighbours (L,R,B,T); state pass = water transport, velocity,
//               sediment capacity → erode / deposit, semi-Lagrangian sediment
//               advection, thermal landslides, rain, evaporation, uplift.
//               state = (height hi, height lo, water depth, sediment)
//   pass 6      scene : the eroded height is raymarched as a 3D island in a
//               dusk sea; strata by absolute altitude (so cutting reveals the
//               layers), glowing rivers tinted by their silt load, storms.
//   pass 7      final : bloom, fringe, grain, ACES, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define WS   2.0          // half-height of the island tile in world units
#define HY   0.62         // world height of h = 1

float bassP()  { return pow(smoothstep(0.05, 0.85, audioBass), 1.4); }
float midP()   { return pow(smoothstep(0.06, 0.85, audioMid),  1.2); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float vnoise(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 5; i++) { s += a * vnoise(p); p = mat2(1.6, 1.2, -1.2, 1.6) * p + 17.1; a *= 0.5; }
    return s;
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}
vec3 grade(vec3 c) {
    c = hueRot(c, hueShift);
    float l = lum(c);
    return max(mix(vec3(l), c, saturation), 0.0);
}

float g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev;

// ═════════════════════════════════════════════════════════════════════════
//  SHARED: storms (the sim rains under them, the render shades under them)
// ═════════════════════════════════════════════════════════════════════════
// q = island coords: x ∈ [-A, A], y ∈ [-1, 1]
vec2 stormPos(float i, float A) {
    float tt = g_t * 0.045 * stormDrift + i * 2.1;
    return vec2(A * 0.72 * sin(tt * (0.7 + 0.13 * i) + i * 1.7), 0.62 * sin(tt * (0.53 + 0.11 * i) + i * 4.1 + 0.6));
}
float stormField(vec2 q, float A) {
    float s = 0.0;
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        vec2 c = stormPos(fi, A);
        float r = 0.36 + 0.1 * sin(g_t * 0.11 + fi * 3.0);
        vec2 dq = q - c;
        // ragged cell edge
        float rag = 0.75 + 0.5 * vnoise(q * 3.1 + fi * 7.0 + g_t * 0.05);
        s += exp(-dot(dq, dq) / (r * r * rag));
    }
    return s;
}

// ═════════════════════════════════════════════════════════════════════════
//  SIMULATION  (passes 0..5)
// ═════════════════════════════════════════════════════════════════════════
vec4 stIn(vec2 uv) {
    if (PASSINDEX <= 1) return texture2D(st3, uv);
    if (PASSINDEX <= 3) return texture2D(st1, uv);
    return texture2D(st2, uv);
}
vec4 flPrev(vec2 uv) {
    if (PASSINDEX == 0) return texture2D(fl3, uv);
    if (PASSINDEX == 2) return texture2D(fl1, uv);
    return texture2D(fl2, uv);
}
vec4 flNew(vec2 uv) {
    if (PASSINDEX == 1) return texture2D(fl1, uv);
    if (PASSINDEX == 3) return texture2D(fl2, uv);
    return texture2D(fl3, uv);
}
// height is stored hi/lo: hi = floor(h·256)/256 is exact in f16, lo = fract(h·256)
float hOf(vec4 s) { return s.r + s.g / 256.0; }
vec4 packSt(float h, float d, float sd) {
    float hh = floor(h * 256.0);
    return vec4(hh / 256.0, h * 256.0 - hh, d, sd);
}

float g_HS;   // sim texels per unit of h (keeps sim slopes == rendered slopes)

float islandMask(vec2 q, float A) {
    float r = length(q / vec2(A * 0.84, 0.86));
    r += 0.28 * (fbm(q * 1.6 + 3.0) - 0.5) + 0.06 * (vnoise(q * 7.0) - 0.5);
    return smoothstep(1.0, 0.45, r);
}
float ridged(vec2 p) {
    float s = 0.0, a = 0.55, w = 1.0;
    for (int i = 0; i < 6; i++) {
        float n = 1.0 - abs(2.0 * vnoise(p) - 1.0);
        n = n * n;
        s += a * n * w;
        w = clamp(n * 1.6, 0.0, 1.0);
        p = mat2(1.7, 1.1, -1.1, 1.7) * p + 9.3;
        a *= 0.5;
    }
    return s;
}
// the island the tectonics want: coast + volcanic cone + ridged ranges.
// seeds frame 0 and is the isostatic rebound target, drifting very slowly
float targetH(vec2 q, float A, float drift, out float rg) {
    float r = length(q / vec2(A * 0.84, 0.86));
    float rn = r + 0.28 * (fbm(q * 1.6 + 3.0) - 0.5) + 0.06 * (vnoise(q * 7.0) - 0.5);
    float m = smoothstep(1.0, 0.45, rn);
    float cone = exp(-r * r * 2.6);
    vec2 w = vec2(fbm(q * 1.3 + 1.0 + drift), fbm(q * 1.3 + 8.0 - drift)) - 0.5;
    rg = ridged(q * 1.25 + w * 0.9 + 4.0 + vec2(drift, -0.5 * drift));
    float h = m * 0.11 + m * cone * 0.42 + 0.34 * rg * m * (0.3 + 0.7 * cone);
    return max(h - 0.02, 0.0);
}
// radial drainage seed: iso-lines of a noise sampled on a circle are radial lines;
// a radius-dependent warp makes them meander. Trunks widen downstream, tributaries
// (a second, denser family) only start part-way down. out q = proxy discharge
float seedChannels(vec2 q, float A, out float r, out float dq) {
    vec2 qe = q / vec2(A * 0.84, 0.86);
    r = length(qe);
    float th = atan(qe.y, qe.x);
    vec2 cs = vec2(cos(th), sin(th));
    vec2 w = vec2(fbm(qe * 2.1 + 4.0), fbm(qe * 2.1 + 9.0)) - 0.5;
    float n1 = vnoise(cs * 4.6 + w * 0.9 * r + 13.0);
    float n2 = vnoise(cs * 26.0 + w * 2.4 * r + 41.0);
    float l1 = abs(2.0 * n1 - 1.0), l2 = abs(2.0 * n2 - 1.0);
    float c1 = smoothstep(0.035 + 0.05 * r, 0.0, l1) * smoothstep(0.06, 0.25, r);
    float c2 = smoothstep(0.06 + 0.04 * r, 0.0, l2) * smoothstep(0.2, 0.38, r);
    float land = smoothstep(1.0, 0.82, r);
    c1 *= land; c2 *= land;
    dq = c1 * (0.1 + 0.7 * r * r) + c2 * (0.035 + 0.09 * r);
    return max(c1, c2 * 0.8);
}

vec4 seedState(vec2 uv) {
    float A = R.x / R.y;
    vec2 q = (uv - 0.5) * 2.0 * vec2(A, 1.0);
    float rg;
    float h = targetH(q, A, 0.0, rg);
    // valleys start wet so rivers glow from the first frame
    // pre-carved drainage: radial trunks + finer tributaries cut down the cone,
    // so the first frame already shows a channel network (the sim then takes over)
    float r, dq;
    float ch = seedChannels(q, A, r, dq);
    h = max(h - 0.028 * ch * smoothstep(0.1, 0.18, h), 0.0);
    return packSt(h, ch * 0.12, 0.0);
}

float totH(vec2 uv) { vec4 s = stIn(uv); return hOf(s) * g_HS + s.b; }

vec4 fluxStep(vec2 fc) {
    vec2 ts = 1.0 / R;
    vec2 uv = fc * ts;
    vec4 s = stIn(uv);
    float d = s.b;
    float Ht = hOf(s) * g_HS + d;
    vec4 hn = vec4(totH(uv - vec2(ts.x, 0.0)), totH(uv + vec2(ts.x, 0.0)),
                   totH(uv - vec2(0.0, ts.y)), totH(uv + vec2(0.0, ts.y)));
    // border: open outflow
    if (fc.x < 1.0) hn.x = 0.0;
    if (fc.x > R.x - 1.0) hn.y = 0.0;
    if (fc.y < 1.0) hn.z = 0.0;
    if (fc.y > R.y - 1.0) hn.w = 0.0;
    vec4 f = max(flPrev(uv) * 0.95 + 0.22 * (Ht - hn), 0.0);
    float sum = f.x + f.y + f.z + f.w;
    f *= min(1.0, d / max(sum, 1e-6));
    return f;
}

vec4 stateStep(vec2 fc) {
    vec2 ts = 1.0 / R;
    vec2 uv = fc * ts;
    float A = R.x / R.y;
    vec2 q = (uv - 0.5) * 2.0 * vec2(A, 1.0);
    float spd = speed;
    vec4 s = stIn(uv);
    float h = hOf(s);
    float Ht = h * g_HS;
    float d = s.b;
    vec4 f  = flNew(uv);
    vec4 fl = flNew(uv - vec2(ts.x, 0.0)), fr = flNew(uv + vec2(ts.x, 0.0));
    vec4 fb = flNew(uv - vec2(0.0, ts.y)), ft = flNew(uv + vec2(0.0, ts.y));
    float inflow = fl.y + fr.x + fb.w + ft.z;
    float outflow = f.x + f.y + f.z + f.w;
    float d1 = max(d + inflow - outflow, 0.0);
    float dAvg = max(0.5 * (d + d1), 0.03);
    vec2 v = vec2(0.5 * (fl.y - f.x + f.y - fr.x), 0.5 * (fb.w - f.z + f.w - ft.z)) / dAvg;
    float vl = length(v);
    if (vl > 1.0) { v /= vl; vl = 1.0; }

    // terrain neighbours
    float hL = hOf(stIn(uv - vec2(ts.x, 0.0))) * g_HS, hR = hOf(stIn(uv + vec2(ts.x, 0.0))) * g_HS;
    float hB = hOf(stIn(uv - vec2(0.0, ts.y))) * g_HS, hT = hOf(stIn(uv + vec2(0.0, ts.y))) * g_HS;
    vec2 grad = 0.5 * vec2(hR - hL, hT - hB);
    float sl = length(grad);
    float sinA = sl / sqrt(1.0 + sl * sl);

    // sediment rides the current (semi-Lagrangian)
    float sed = stIn(uv - v * ts).a;

    // capacity → erode or deposit
    float er = erosionRate * spd * (1.0 + 0.6 * g_ar * g_bass);
    float cap = 1.6 * max(sinA, 0.05) * vl * clamp(d1 / 0.08, 0.0, 1.0);
    float dh = 0.0;
    if (cap > sed) {
        float e = min(0.08 * er * (cap - sed), 0.08);
        dh -= e; sed += e;
    } else {
        float dep = 0.016 * (0.5 + 0.5 * er) * (sed - cap);
        dh += dep; sed -= dep;
    }

    // thermal landslides: material above the talus angle slumps (mass-conserving)
    float talus = 0.9;
    float kT = 0.06 * landslide * min(spd, 1.0);
    vec4 dd = vec4(hL, hR, hB, hT) - Ht;
    vec4 over = max(abs(dd) - talus, 0.0) * sign(dd);
    dh += kT * (over.x + over.y + over.z + over.w);

    // soil creep: slow diffusion that heals single-texel pits/spikes
    dh += 0.003 * ((hL + hR + hB + hT) * 0.25 - Ht);
    float Hn = Ht + dh;

    // rain: drizzle + storm cells; bass pours harder, beats burst inside storms
    float storm = stormField(q, A) * storms;
    float rr = rain * spd * (0.25 + 0.75 * smoothstep(0.0, 14.0, g_t)) * (0.0006 + 0.0019 * storm * (1.0 + 0.6 * g_ar * g_bass + 0.5 * g_ar * g_beat));
    d1 += rr;
    d1 *= 1.0 - (0.008 + 0.012 * smoothstep(1.0, 4.0, d1)) * (0.5 + 0.5 * spd);

    // sea: rivers vanish into it and drop their silt → deltas build out
    float m = islandMask(q, A);
    float seaH = seaLevel * g_HS;
    if (Hn < seaH) {
        Hn += sed * 0.35;
        sed *= 0.65;
        d1 *= 0.4;
    }

    // isostatic rebound toward the (slowly drifting) tectonic island keeps it
    // alive forever: erosion carves faster than it heals, so canyons persist
    float hn = Hn / g_HS;
    // (evaluated once per frame, on the last solver step: the field is costly)
    if (PASSINDEX == 5 && m > 0.001) {
        float rgT;
        float tgt = targetH(q, A, g_t * 0.004, rgT);
        hn += uplift * spd * 0.00042 * max(tgt - hn, 0.0);
    }
    // ocean floor outside the island relaxes back down
    hn = mix(hn, 0.0, (1.0 - m) * 0.002);
    hn = clamp(hn, 0.0, 1.2);

    return packSt(hn, clamp(d1, 0.0, 6.0), clamp(sed, 0.0, 4.0));
}

// ═════════════════════════════════════════════════════════════════════════
//  RENDER  (pass 6)
// ═════════════════════════════════════════════════════════════════════════
float g_A;            // island tile aspect
vec2  g_simR;
vec3  g_ro, g_uu, g_vv, g_ww, g_L, g_sunC;
float g_flash;
vec2  g_flashPos;

vec2 worldToUV(vec2 xz) { return xz / vec2(g_A * WS, WS) * 0.5 + 0.5; }
vec4 stS(vec2 uv) { return texture2D(st3, uv); }
float inTile(vec2 uv) { return step(0.0, uv.x) * step(uv.x, 1.0) * step(0.0, uv.y) * step(uv.y, 1.0); }

float terrH(vec2 xz) {             // world height of rock
    vec2 uv = worldToUV(xz);
    if (inTile(uv) < 0.5) return 0.0;
    return hOf(stS(uv)) * HY * relief;
}
vec4 cubic4(sampler2D tex, vec2 uv) {
    // B-spline bicubic from 4 bilinear taps (smooth when magnified)
    vec2 ts = 1.0 / g_simR;
    vec2 st = uv * g_simR - 0.5;
    vec2 i = floor(st), f = fract(st);
    vec2 w0 = (1.0 / 6.0) * (1.0 - f) * (1.0 - f) * (1.0 - f);
    vec2 w1 = (1.0 / 6.0) * (4.0 - 3.0 * f * f * (2.0 - f));
    vec2 w2 = (1.0 / 6.0) * (4.0 - 3.0 * (1.0 - f) * (1.0 - f) * (1.0 + f));
    vec2 w3 = (1.0 / 6.0) * f * f * f;
    vec2 g0 = w0 + w1, g1 = w2 + w3;
    vec2 h0 = (w1 / g0) - 1.0 + i;
    vec2 h1 = (w3 / g1) + 1.0 + i;
    vec2 p00 = (h0 + 0.5) * ts, p11 = (h1 + 0.5) * ts;
    vec4 a = texture2D(tex, vec2(p00.x, p00.y)), b = texture2D(tex, vec2(p11.x, p00.y));
    vec4 c = texture2D(tex, vec2(p00.x, p11.y)), d = texture2D(tex, vec2(p11.x, p11.y));
    return g0.y * (g0.x * a + g1.x * b) + g1.y * (g0.x * c + g1.x * d);
}
float hCubic(vec2 uv) { return hOf(cubic4(st3, uv)); }
float terrHs(vec2 xz) {            // smooth (bicubic) rock height for shading
    vec2 uv = worldToUV(xz);
    if (inTile(uv) < 0.5) return 0.0;
    return hCubic(uv) * HY * relief;
}
// sim water depth → world units (rivers are thin: exaggerate a little)
float waterW(float d) { return d / g_simR.y * 2.0 * WS * 1.4; }

float marchTerrain(vec3 ro, vec3 rd, float tMax) {
    float maxH = HY * relief * 1.25;
    float t = 0.0;
    if (ro.y > maxH) {
        if (rd.y >= 0.0) return -1.0;
        t = (maxH - ro.y) / rd.y;
    }
    float tPrev = t;
    bool hit = false;
    for (int i = 0; i < 150; i++) {
        vec3 p = ro + rd * t;
        float dy = p.y - terrH(p.xz);
        if (dy < 0.0004 * (1.0 + t)) { hit = true; break; }
        if (p.y > maxH && rd.y > 0.0) break;
        tPrev = t;
        t += max(dy * 0.42, 0.003 + t * 0.0018);
        if (t > tMax) break;
    }
    if (!hit) return -1.0;
    float a = tPrev, b = t;
    for (int i = 0; i < 6; i++) {
        float mm = 0.5 * (a + b);
        vec3 p = ro + rd * mm;
        if (p.y - terrH(p.xz) < 0.0) b = mm; else a = mm;
    }
    return 0.5 * (a + b);
}

// reflections only need the silhouette: few, long steps, no refine
float marchTerrainCheap(vec3 ro, vec3 rd, float tMax) {
    float maxH = HY * relief * 1.25;
    float t = 0.01;
    for (int i = 0; i < 40; i++) {
        vec3 p = ro + rd * t;
        if (p.y > maxH) return -1.0;
        float dy = p.y - terrH(p.xz);
        if (dy < 0.002) return t;
        t += max(dy * 0.6, 0.02 + t * 0.01);
        if (t > tMax) break;
    }
    return -1.0;
}

float softShadow(vec3 p, vec3 L) {
    float res = 1.0;
    float t = 0.012;
    float maxH = HY * relief * 1.25;
    for (int i = 0; i < 24; i++) {
        vec3 q = p + L * t;
        if (q.y > maxH) break;
        float dy = q.y - terrH(q.xz);
        res = min(res, 10.0 * dy / t);
        if (res < 0.005) break;
        t += clamp(dy * 0.7, 0.014, 0.2);
    }
    return smoothstep(0.0, 1.0, clamp(res, 0.0, 1.0));
}

// ── dusk sky ────────────────────────────────────────────────────────────
// horizon colour for a view azimuth: shared by sky, sea fog and aerial haze so
// the sea melts into the sky with no seam
vec3 horizonCol(vec3 rd) {
    vec2 h = normalize(rd.xz + 1e-5), l = normalize(g_L.xz + 1e-5);
    float sd = max(dot(h, l), 0.0);
    vec3 cool = grade(skyColor.rgb) * 1.7 + vec3(0.045, 0.014, 0.045);
    vec3 warm = g_sunC * (0.22 + 0.8 * pow(sd, 6.0));
    return mix(cool, warm, 0.1 + 0.7 * pow(sd, 3.0));
}
vec3 skyGrad(vec3 rd) {
    float y = max(rd.y, 0.0);
    float sd = max(dot(rd, g_L), 0.0);
    vec3 hor = horizonCol(rd);
    vec3 mid = vec3(0.17, 0.04, 0.15) * (0.5 + 0.5 * sd) + grade(skyColor.rgb) * 0.9;
    vec3 zen = grade(skyColor.rgb) * 0.45;
    vec3 col = mix(hor, mid, smoothstep(0.0, 0.16, y));
    col = mix(col, zen, smoothstep(0.12, 0.65, y));
    // antisolar side: the Belt of Venus — a pink band riding above Earth's blue shadow
    float anti = pow(max(-dot(normalize(rd.xz + 1e-5), normalize(g_L.xz + 1e-5)), 0.0), 1.5);
    col += vec3(0.26, 0.085, 0.16) * anti * exp(-pow((y - 0.11) / 0.07, 2.0));
    col = mix(col, grade(skyColor.rgb) * 1.3, anti * smoothstep(0.06, 0.0, y) * 0.6);
    return col;
}
float cloudD(vec2 q) {
    vec2 w = vec2(fbm(q * 0.7 + 3.1), fbm(q * 0.7 + 9.4)) - 0.5;
    float d = fbm(q + w * 1.4);
    d += 0.35 * (vnoise(q * 5.3 + w * 3.0) - 0.5);
    return d;
}
vec3 skyCol(vec3 rd) {
    float y = rd.y;
    float sd = max(dot(rd, g_L), 0.0);
    vec3 col = skyGrad(rd);
    // sun disc + halo
    col += g_sunC * (pow(sd, 1400.0) * 20.0 + pow(sd, 60.0) * 0.45 + pow(sd, 8.0) * 0.12) * keyLight;
    // stars in the deep zenith
    if (y > 0.2) {
        vec2 sp = rd.xz / (y + 0.4) * 190.0;
        float st = step(0.9968, hash12(floor(sp))) * smoothstep(0.2, 0.5, y);
        col += vec3(0.8, 0.85, 1.0) * st * 0.55 * smoothstep(0.3, 0.0, length(fract(sp) - 0.5));
    }
    // lit altocumulus layer: density on a plane, lit by its slope toward the sun
    if (y > 0.0) {
        vec2 cq = rd.xz / (y + 0.1) * 0.4 + vec2(g_t * 0.006, g_t * 0.002);
        float d0 = cloudD(cq);
        float cov = smoothstep(0.43, 0.7, d0);
        if (cov > 0.001) {
            vec2 ls = normalize(g_L.xz + 1e-5) * 0.07;
            float d1 = cloudD(cq + ls);
            float lit = clamp((d0 - d1) * 5.0 + 0.45, 0.0, 1.0);
            vec3 base = vec3(0.03, 0.018, 0.035) + grade(skyColor.rgb) * 0.5;
            vec3 litC = g_sunC * (0.35 + 1.5 * pow(sd, 3.0)) + vec3(0.18, 0.05, 0.14);
            vec3 cc = mix(base, litC, lit * lit);
            // thin bright rim where the cloud thins out toward the sun
            cc += g_sunC * smoothstep(0.62, 0.47, d0) * (0.2 + 0.8 * pow(sd, 4.0)) * 0.6;
            float fade = smoothstep(0.01, 0.07, y) * smoothstep(0.85, 0.3, y);
            col = mix(col, cc, cov * fade * 0.9);
        }
    }
    // haze band hugging the horizon
    col = mix(col, horizonCol(rd), exp(-max(y, 0.0) * 22.0) * 0.85);
    return col;
}

// ── strata: many thin layers of varied thickness and tone ───────────────
// formations (coarse) pick a rock family; each holds a varying number of
// laminae with dark partings; rare white ash beds. AA'd by the pixel footprint
vec3 strataCol(float y, vec2 xz, float fpF, out float seam) {
    float hs = y / (HY * relief);
    float f = hs * 38.0 * strata + 0.7 * (fbm(xz * 2.2) - 0.5) + 0.12 * (vnoise(xz * 13.0) - 0.5);
    float fi = floor(f);
    float fr = fract(f);
    float k = hash11(fi * 1.37 + 3.0);
    vec3 basalt = grade(rockA.rgb), rust = mix(grade(rockB.rgb), basalt, 0.25) * 0.5, ochre = grade(rockC.rgb) * 0.42;
    vec3 ashC = vec3(0.5, 0.48, 0.46);
    vec3 col = k < 0.5 ? basalt : (k < 0.72 ? rust : (k < 0.9 ? ochre : mix(basalt, ashC, 0.55)));
    // laminae: 3..8 per formation
    float nL = 3.0 + floor(hash11(fi * 5.3) * 6.0);
    float g = fr * nL;
    float li = floor(g);
    float lf = fract(g);
    col *= 0.72 + 0.5 * hash11(fi * 11.0 + li * 1.7);
    float aaL = clamp(fpF * nL * 2.5, 0.0, 1.0);             // laminae vanish into tone when sub-pixel
    float part = smoothstep(0.12, 0.0, min(lf, 1.0 - lf));
    col *= 1.0 - 0.6 * part * (1.0 - aaL);
    // rare thin ash bed at a formation boundary
    float ashBed = step(0.78, hash11(fi * 3.91 + 1.0)) * smoothstep(0.07, 0.025, fr);
    float aaF = clamp(fpF * 12.0, 0.0, 1.0);
    col = mix(col, ashC, ashBed * (1.0 - aaF) * 0.8);
    seam = ashBed * (1.0 - aaF);
    return col;
}

// render-only sub-texel relief: down-slope ridged gullies, scree, grain
float detailH(vec2 xz, vec2 dn, float slope, float flowAmt, float fp) {
    vec2 pd = vec2(-dn.y, dn.x);
    float u = dot(xz, pd), v = dot(xz, dn);
    float steep = smoothstep(0.06, 0.4, slope);
    float o1 = 1.0 - abs(2.0 * vnoise(vec2(u * 48.0, v * 7.0)) - 1.0);
    float o2 = 1.0 - abs(2.0 * vnoise(vec2(u * 110.0, v * 15.0) + 7.0) - 1.0);
    float k2 = smoothstep(3.0, 6.0, (1.0 / 110.0) / fp);
    float gully = (o1 * o1 + 0.45 * o2 * o2 * k2) * steep * (1.0 + 0.8 * flowAmt);
    float sc = smoothstep(0.3, 0.6, slope);
    float k3 = smoothstep(2.5, 5.0, (1.0 / 170.0) / fp);
    float scree = (vnoise(xz * 170.0) + 0.5 * vnoise(xz * 330.0 + 3.0) * smoothstep(2.5, 5.0, (1.0 / 330.0) / fp)) * sc * k3;
    float grain = vnoise(xz * 90.0 + 11.0) * (1.0 - steep * 0.5);
    return gully * 0.0042 + scree * 0.0016 + grain * 0.0012;
}

float netLAt(vec2 uv) { return log2(1.0 + 200.0 * cubic4(net, uv).b); }
float netLAtLin(vec2 uv) { return log2(1.0 + 200.0 * texture2D(net, uv).b); }

float seaH(vec2 w) {
    float h = 0.006 * sin(dot(w, vec2(0.8, 0.6)) * 9.0 - g_t * 1.1);
    h += 0.004 * sin(dot(w, vec2(-0.5, 0.86)) * 13.0 - g_t * 1.4);
    h += 0.0035 * vnoise(w * 24.0 + vec2(g_t * 0.4, g_t * 0.25));
    h += 0.0012 * vnoise(w * 70.0 - vec2(g_t * 0.7, 0.0));
    h += 0.0004 * vnoise(w * 180.0 + g_t * 0.9);
    return h;
}

vec4 renderScene(vec2 sc) {
    vec3 rd = normalize(g_uu * sc.x + g_vv * sc.y + g_ww * 1.85);
    float seaY = seaLevel * HY * relief;
    float tSea = rd.y < 0.0 ? (seaY - g_ro.y) / rd.y : 1e5;
    if (g_ro.y < seaY) tSea = 1e5;
    float tMax = min(tSea, 40.0);
    float t = marchTerrain(g_ro, rd, tMax + 0.01);
    vec3 sky = skyCol(rd);
    float hazeK = 0.055 * haze;
    vec3 hazeC = horizonCol(rd);
    float fpK = 1.0 / (R.y * 1.85);                     // world size of a pixel per unit distance

    vec3 col;
    float cov = 1.0;
    vec3 p;
    if (t > 0.0 && t < tSea) {
        p = g_ro + rd * t;
        float fp = t * fpK;
        vec2 uv = worldToUV(p.xz);
        vec4 s = cubic4(st3, uv);
        float dW = s.b;
        float e = 2.0 * WS / g_simR.y;
        float en = max(e, fp * 1.5);
        float hc = terrHs(p.xz);
        float hx0 = terrHs(p.xz - vec2(en, 0.0)), hx1 = terrHs(p.xz + vec2(en, 0.0));
        float hz0 = terrHs(p.xz - vec2(0.0, en)), hz1 = terrHs(p.xz + vec2(0.0, en));
        vec2 gH = vec2(hx1 - hx0, hz1 - hz0) / (2.0 * en);
        vec3 n = normalize(vec3(-gH.x, 1.0, -gH.y));
        float slope = 1.0 - n.y;
        vec2 dn = normalize(-gH + 1e-6);                   // down-slope direction

        // ── flow field (for rivers and rills)
        float netL = netLAt(uv);
        float ue = en / (2.0 * g_A * WS);
        float ve = en / (2.0 * WS);
        vec2 gN = vec2(netLAt(uv + vec2(ue, 0.0)) - netL, netLAt(uv + vec2(0.0, ve)) - netL) / en;
        float flowAmt = smoothstep(0.4, 1.7, netL);

        // ── sub-texel relief as a bump on the smooth sim surface
        float dd = max(fp * 1.2, 0.0008);
        float D0 = detailH(p.xz, dn, slope, flowAmt, fp);
        float Dx = detailH(p.xz + vec2(dd, 0.0), dn, slope, flowAmt, fp);
        float Dz = detailH(p.xz + vec2(0.0, dd), dn, slope, flowAmt, fp);
        float near = smoothstep(12.0, 2.0, t);
        vec2 gD = vec2(Dx - D0, Dz - D0) / dd * near;
        vec3 ng = normalize(vec3(-gH.x - gD.x, 1.0, -gH.y - gD.y));

        float fpF = fp * 38.0 * strata / (HY * relief) * 1.6;
        float seam;
        vec3 alb = strataCol(hc + D0 * 0.6, p.xz, fpF, seam);
        float cut = smoothstep(0.08, 0.36, slope);
        vec3 soil = mix(grade(rockA.rgb) * 1.25, grade(rockB.rgb) * 0.4, 0.3 + 0.35 * vnoise(p.xz * 18.0));
        soil *= 0.8 + 0.4 * vnoise(p.xz * 61.0);
        alb = mix(soil, alb, cut);
        seam *= cut;
        float flat_ = smoothstep(0.25, 0.06, slope);
        float lowland = smoothstep(0.42, 0.12, p.y / (HY * relief));
        float moss = flat_ * lowland * (0.55 + 0.45 * vnoise(p.xz * 26.0));
        alb = mix(alb, grade(mossColor.rgb) * (0.7 + 0.6 * vnoise(p.xz * 70.0)), moss * 0.85);
        float ashT = flat_ * smoothstep(0.55, 0.8, p.y / (HY * relief));
        alb = mix(alb, vec3(0.5, 0.48, 0.5), ashT * 0.55);
        // gully floors darker (fines + damp), crests catch light
        alb *= 0.8 + 0.4 * clamp(D0 / 0.004, 0.0, 1.0);

        float sh = mix(1.0, softShadow(p + n * 0.004, g_L), shadowAmt);
        vec2 q = (uv - 0.5) * 2.0 * vec2(g_A, 1.0);
        float stF = stormField(q, g_A) * storms;
        float cloud = clamp(stF * 0.8, 0.0, 1.0) * cloudShadow * (0.6 + 0.4 * vnoise(q * 4.0 + g_t * 0.08));
        sh *= 1.0 - 0.85 * cloud;

        float ea = e * 3.0;
        float ao = terrHs(p.xz + vec2(ea, 0.0)) + terrHs(p.xz - vec2(ea, 0.0)) + terrHs(p.xz + vec2(0.0, ea)) + terrHs(p.xz - vec2(0.0, ea));
        ao = clamp(1.0 - (ao * 0.25 - hc) * 28.0, 0.25, 1.0);
        ao *= 0.7 + 0.3 * clamp(D0 / 0.004, 0.0, 1.0);

        vec3 V = -rd;
        float ndl = max(dot(ng, g_L), 0.0);
        vec3 Hh = normalize(g_L + V);
        vec3 key = g_sunC * 2.2 * keyLight;
        vec3 skyFill = mix(grade(skyColor.rgb) * 5.0, horizonCol(vec3(ng.x, 0.0, ng.z)) * 0.5, 0.35) * (0.5 + 0.5 * ng.y) + vec3(0.015, 0.01, 0.018);
        vec3 bounce = g_sunC * 0.05 * max(0.5 - 0.5 * ng.y - ndl, 0.0);
        col = alb * (key * ndl * sh + skyFill * ao + bounce * ao);
        float spec = pow(max(dot(ng, Hh), 0.0), 36.0) * 0.07;
        col += key * spec * sh;
        col += alb * seam * key * ndl * sh * 0.6;

        // ── water: dark glossy threads; a hairline emissive core only in big trunks;
        //    broad flow (deltas, fans) is a shallow sheet with braided threads
        float steepW = smoothstep(0.2, 0.5, slope);
        float thr = 2.05 + 2.3 * steepW;
        float wN = max(length(gN) * fp * 1.3, 0.025);
        vec2 to = 4.0 / g_simR;
        float avgL = 0.25 * (netLAtLin(uv + vec2(to.x, 0.0)) + netLAtLin(uv - vec2(to.x, 0.0)) + netLAtLin(uv + vec2(0.0, to.y)) + netLAtLin(uv - vec2(0.0, to.y)));
        float exc = netL - avgL;
        // width follows discharge: small streams show only their crest line
        float need = mix(0.75, -0.35, smoothstep(2.8, 6.0, netL));
        float bank = smoothstep(thr - wN, thr + wN, netL) * smoothstep(need - wN, need + wN, exc);
        float lake = smoothstep(0.3 - 0.04, 0.3 + 0.04, dW);
        lake *= smoothstep(0.2, 0.08, slope);               // ponds only sit on flats
        lake *= smoothstep(2.0, 5.0, g_t);                  // the opening flush is not a lake
        float wet = smoothstep(thr - 0.9, thr, netL) * (1.0 - bank);
        col *= 1.0 - 0.45 * wet;                            // damp banks
        float wmask = max(bank, lake);
        if (wmask > 0.001) {
            vec4 f = texture2D(fl3, uv);
            vec2 ts = 1.0 / g_simR;
            vec4 flx = texture2D(fl3, uv - vec2(ts.x, 0.0)), frx = texture2D(fl3, uv + vec2(ts.x, 0.0));
            vec4 fbx = texture2D(fl3, uv - vec2(0.0, ts.y)), ftx = texture2D(fl3, uv + vec2(0.0, ts.y));
            vec2 vel = vec2(flx.y - f.x + f.y - frx.x, fbx.w - f.z + f.w - ftx.z) / max(dW, 0.05);
            float vs = clamp(length(vel), 0.0, 1.5);
            vec2 vdir = vs > 1e-4 ? vel / length(vel) : dn;
            vec2 vperp = vec2(-vdir.y, vdir.x);
            // two-phase flow-map ripples on the skin
            float ph = g_t * 0.9 * (0.4 + speed);
            float p0 = fract(ph), p1 = fract(ph + 0.5);
            vec2 base = p.xz * 70.0;
            float n0 = vnoise(base - vdir * p0 * 4.0 * (0.3 + vs));
            float n1 = vnoise(base - vdir * p1 * 4.0 * (0.3 + vs) + 5.3);
            float ripple = mix(n0, n1, 1.0 - abs(1.0 - 2.0 * p0));
            vec3 wn = normalize(vec3((ripple - 0.5) * 0.16 * (1.0 - lake * 0.7), 1.0, (0.5 - ripple) * 0.16 * (1.0 - lake * 0.7)));
            vec3 rf = reflect(rd, wn);
            float fr = 0.06 + 0.94 * pow(1.0 - max(dot(V, wn), 0.0), 5.0);
            vec3 skyR = skyCol(rf);
            float siltK = smoothstep(0.07, 0.35, cubic4(net, uv).g) * smoothstep(0.5, 5.0, g_t);
            vec3 rc = mix(grade(riverColor.rgb), grade(siltColor.rgb), siltK);

            // channel water: dark body, the sky in its skin
            vec3 body = grade(seaColor.rgb) * 0.45 + grade(siltColor.rgb) * 0.03 * siltK;
            vec3 water = body * (1.0 - fr) + skyR * fr;
            water += g_sunC * pow(max(dot(rf, g_L), 0.0), 260.0) * 7.0 * sh * (0.6 + 0.8 * g_ar * g_high);
            // sky-lit meniscus along the banks: threads read even when thin
            water += horizonCol(rf) * 0.35 * bank * (1.0 - bank) * 4.0;

            // broad flow → shallow sheet: bed and sediment seen through, braided threads
            float broad = smoothstep(0.25, -0.35, exc) * smoothstep(0.2, 0.07, slope) * (1.0 - lake);
            if (broad > 0.001) {
                float br = vnoise(vec2(dot(p.xz, vperp) * 85.0, dot(p.xz, vdir) * 7.0 - g_t * 0.4 * (0.4 + speed)));
                br += 0.5 * vnoise(vec2(dot(p.xz, vperp) * 190.0 + 3.0, dot(p.xz, vdir) * 14.0));
                float thread = smoothstep(0.86, 0.98, br) * (1.0 - smoothstep(1.02, 1.12, br));
                vec3 bed = col * 0.6 + grade(siltColor.rgb) * 0.1 * siltK + grade(rockC.rgb) * 0.03;
                vec3 sheetC = bed * (1.0 - fr * 0.7) + skyR * fr * 0.7;
                sheetC += (skyR * 0.5 + rc * 0.22 * riverGlow) * thread;
                water = mix(water, sheetC, broad);
            }

            // hairline emissive core: distance to the discharge ridge (across the flow)
            vec2 pu = vperp * vec2(1.0 / (2.0 * g_A * WS), 1.0 / (2.0 * WS));
            float hh = e;
            float nP = netLAt(uv + pu * hh), nM = netLAt(uv - pu * hh);
            float gl = (nP - nM) / (2.0 * hh);
            float kap = (nP + nM - 2.0 * netL) / (hh * hh);
            float rdist = abs(gl) / max(-kap, 1e-3);
            float ridge = step(kap, 0.0);
            float hot = smoothstep(thr + 3.2, thr + 5.5, netL);
            float trunk = smoothstep(thr + 1.8, thr + 3.2, netL) * (1.0 - 0.6 * broad);
            // trunks: 4-6 px core + tight halo; mid tributaries: ~2 px faint threads
            float core = (1.0 - smoothstep(2.0 * fp, 3.0 * fp, rdist)) * ridge * trunk;
            float halo = exp(-rdist / (3.5 * fp)) * ridge * trunk * 0.3;
            float trib = (1.0 - smoothstep(0.7 * fp, 1.1 * fp, rdist)) * ridge * smoothstep(thr + 0.45, thr + 1.1, netL) * (1.0 - trunk) * 0.4;
            float amod = (1.0 + 0.45 * g_ar * g_bass) * (0.85 + 0.3 * ripple) * (0.85 + 0.3 * g_ar * g_lev);
            float glow = riverGlow * ((core * (0.9 + 1.2 * hot) + halo) + trib) * amod;
            water += rc * glow * 1.6;

            // cascades on steep walls: white streaks down the fall line, no fill
            float streak = vnoise(vec2(dot(p.xz, vec2(-dn.y, dn.x)) * 260.0, dot(p.xz, dn) * 12.0 - g_t * 3.0 * (0.4 + speed)));
            water += vec3(0.5, 0.56, 0.6) * smoothstep(0.5, 0.85, streak) * 0.45 * steepW * (1.0 - lake);
            col = mix(col, water, wmask);
        }

        float fd = length(q - g_flashPos);
        col += vec3(0.55, 0.65, 1.0) * g_flash * exp(-fd * fd * 6.0) * (0.25 + 0.75 * max(ng.y, 0.0)) * alb * 6.0;

        // aerial perspective: distant land lifts and cools toward the horizon colour
        float fz = 1.0 - exp(-t * hazeK);
        col = mix(col, hazeC, fz);
        col = mix(col, hazeC * 0.7, clamp(stF * 0.25 * storms, 0.0, 0.35) * smoothstep(1.0, 4.0, t));
    } else if (tSea < 1e4) {
        p = g_ro + rd * tSea;
        vec2 wq = p.xz;
        // waves: long swell + wind chop, finite-difference normal, fading with range
        float fall = smoothstep(16.0, 0.8, tSea);
        float ws = 0.012;
        float hw0 = seaH(wq), hwx = seaH(wq + vec2(ws, 0.0)), hwz = seaH(wq + vec2(0.0, ws));
        vec3 n = normalize(vec3(-(hwx - hw0) / ws * fall, 1.0, -(hwz - hw0) / ws * fall));
        vec3 rf = reflect(rd, n);
        float fr = 0.02 + 0.98 * pow(1.0 - max(dot(-rd, n), 0.0), 5.0);
        float depth = seaY - terrH(p.xz);
        float shelf = exp(-max(depth, 0.0) * 40.0);
        vec3 deep = grade(seaColor.rgb);
        vec3 shallow = mix(grade(riverColor.rgb) * 0.2, grade(rockC.rgb) * 0.3, 0.4);
        col = mix(deep, shallow, shelf * 0.8);
        float surf = smoothstep(0.012, 0.0, depth) * (0.6 + 0.4 * vnoise(vec2(p.x * 40.0 + g_t, p.z * 40.0)));
        col += vec3(0.75, 0.85, 0.9) * surf * 0.3;
        // (cloud-free sky in the sea: reflected clouds alias into sparkle at range)
        vec3 refl = skyGrad(rf) + g_sunC * pow(max(dot(rf, g_L), 0.0), 60.0) * 0.45 * keyLight;
        float tr = marchTerrainCheap(p + vec3(0.0, 0.002, 0.0), rf, 6.0);
        if (tr > 0.0) refl = mix(grade(rockA.rgb) * 0.4 + g_sunC * 0.05, hazeC, 1.0 - exp(-tr * hazeK * 2.0));
        col = col * (1.0 - fr) + refl * fr;
        col += g_sunC * pow(max(dot(rf, g_L), 0.0), 350.0) * 10.0 * keyLight * (0.6 + 0.8 * g_ar * g_high);
        vec2 uv = worldToUV(p.xz);
        if (inTile(uv) > 0.5) {
            vec4 s = stS(uv);
            float spill = smoothstep(0.0, 0.3, s.a + s.b * 0.2) * shelf;
            col += mix(grade(riverColor.rgb), grade(siltColor.rgb), 0.5) * spill * 0.2 * riverGlow;
        }
        // sea melts into the horizon haze
        float fz = 1.0 - exp(-tSea * hazeK * 1.15);
        fz = max(fz, smoothstep(-0.025, 0.0, rd.y));
        col = mix(col, hazeC, fz);
    } else {
        col = sky;
        cov = 0.0;
    }

    if (g_flash > 0.001) {
        col += vec3(0.5, 0.55, 0.9) * g_flash * 0.25 * exp(-abs(rd.y) * 4.0);
    }
    return vec4(max(col, 0.0), cov);
}

void setupCam() {
    float ang = g_t * 0.045 * orbitSpeed + 2.3;
    float rad = (2.5 + 0.25 * sin(g_t * 0.029 + 1.2)) * flyDistance;
    vec3 ta = vec3(0.3 * sin(g_t * 0.023), 0.1 * relief, 0.2 * cos(g_t * 0.019));
    g_ro = vec3(cos(ang) * rad * 1.15, (0.66 + 0.12 * sin(g_t * 0.029 + 1.2) + 0.04 * sin(g_t * 0.07)) * flyHeight, sin(ang) * rad * 0.95);
    // keep well above the land
    g_ro.y = max(g_ro.y, terrH(g_ro.xz) + 0.38);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);
    float az = radians(sunAz), el = radians(sunEl);
    g_L = normalize(vec3(cos(el) * cos(az), sin(el), cos(el) * sin(az)));
    g_sunC = grade(sunColor.rgb);
}

void main() {
    vec2 fc = gl_FragCoord.xy;
    g_ar = audioReact;
    g_t = TIME;
    g_bass = bassP(); g_mid = midP(); g_high = highP(); g_beat = beatP(); g_lev = levP();

    if (PASSINDEX <= 5) {
        g_HS = HY * relief * R.y / (2.0 * WS);
        bool isFlux = (PASSINDEX == 0 || PASSINDEX == 2 || PASSINDEX == 4);
        if (FRAMEINDEX < 2) {
            gl_FragColor = isFlux ? vec4(0.0) : seedState(fc / R);
            return;
        }
        gl_FragColor = isFlux ? fluxStep(fc) : stateStep(fc);
        return;
    }

    if (PASSINDEX == 6) {
        // river network memory: slow average of discharge so the dendritic
        // network keeps glowing between storms (storms make it surge)
        vec2 uv = fc / R;
        vec2 ts = 1.0 / R;
        vec4 f = texture2D(fl3, uv);
        vec4 s = texture2D(st3, uv);
        vec4 fl = texture2D(fl3, uv - vec2(ts.x, 0.0)), fr = texture2D(fl3, uv + vec2(ts.x, 0.0));
        vec4 fb = texture2D(fl3, uv - vec2(0.0, ts.y)), ft = texture2D(fl3, uv + vec2(0.0, ts.y));
        // NET discharge (sloshing back and forth cancels out)
        float dis = length(vec2(fl.y - f.x + f.y - fr.x, fb.w - f.z + f.w - ft.z)) * 0.5;
        float sr, sdq = 0.0;
        if (FRAMEINDEX < 2) { float sch = seedChannels((uv - 0.5) * 2.0 * vec2(R.x / R.y, 1.0), R.x / R.y, sr, sdq); }
        float prev = FRAMEINDEX < 2 ? sdq : texture2D(net, uv).r;
        float sedP = FRAMEINDEX < 2 ? 0.0 : texture2D(net, uv).g;
        float now = dis;
        // b = spatially smoothed memory: the pipe solver routes water through 4
        // neighbours, so diagonal channels zig-zag at texel scale; the render uses this
        vec2 o = 0.8 * ts;
        float bl = 0.25 * (texture2D(net, uv + vec2(o.x, o.y)).r + texture2D(net, uv + vec2(-o.x, o.y)).r
                         + texture2D(net, uv + vec2(o.x, -o.y)).r + texture2D(net, uv + vec2(-o.x, -o.y)).r);
        bl = mix(bl, 0.25 * (texture2D(net, uv + vec2(1.8 * o.x, 0.0)).b + texture2D(net, uv - vec2(1.8 * o.x, 0.0)).b
                           + texture2D(net, uv + vec2(0.0, 1.8 * o.y)).b + texture2D(net, uv - vec2(0.0, 1.8 * o.y)).b), 0.35);
        gl_FragColor = vec4(mix(prev, now, now > prev ? 0.06 : mix(0.003, 0.012, smoothstep(1.0, 4.0, g_t))), mix(sedP, s.a, 0.03), FRAMEINDEX < 2 ? sdq : bl, 1.0);
        return;
    }

    if (PASSINDEX == 7) {
        g_simR = floor(min(R / marchRes * simRes, vec2(1024.0, 576.0)));
        g_A = g_simR.x / g_simR.y;
        setupCam();
        // lightning: soft envelope on the beat pulse, centred in a storm cell
        float bn = floor(g_t * 0.5);
        g_flashPos = stormPos(mod(bn, 3.0), g_A) + (hash22(vec2(bn, 4.0)) - 0.5) * 0.3;
        g_flash = lightning * g_ar * g_beat * g_beat * 0.5 * storms;
        vec2 sc = (fc - 0.5 * R) / R.y;
        gl_FragColor = renderScene(sc);
        return;
    }

    // ── final ────────────────────────────────────────────────────────────
    vec2 st = fc / R;
    vec2 uvc = (fc - 0.5 * R) / R.y;
    vec2 asp = vec2(R.y / R.x, 1.0);
    vec4 c0 = texture2D(scene, st);
    float ab = aberration * 0.002 * length(uvc);
    vec2 dir = normalize(uvc + 1e-4);
    vec3 col = vec3(texture2D(scene, st + dir * ab * asp).r, c0.g, texture2D(scene, st - dir * ab * asp).b);
    float thr = 1.0;
    vec3 bl = vec3(0.0);
    float rot = hash12(floor(fc / 2.0)) * 6.28;
    for (int i = 0; i < 16; i++) {
        float fi = float(i);
        float a = fi * 2.399963 + rot;
        float rr = sqrt((fi + 0.5) / 16.0) * 0.024;
        vec2 of = vec2(cos(a), sin(a)) * rr * asp;
        vec3 s = texture2D(scene, st + of).rgb;
        bl += max(s - thr, 0.0) * (1.0 - rr / 0.024);
    }
    bl /= 16.0;
    col += bl * bloom * 2.0;
    col *= mix(1.0, 0.9 + 0.22 * g_lev + 0.12 * g_bass, g_ar);
    col = aces(col * 1.05);
    float g1 = (hash12(fc + fract(g_t) * 61.0) - 0.5) * 0.05;
    col += g1 * grain;
    col *= 1.0 - 0.1 * smoothstep(0.5, 1.4, length(uvc));
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float l = lum(col);
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    // interleaved-gradient-noise dither: kills banding in the dusk gradient
    float ign = fract(52.9829189 * fract(dot(fc + float(FRAMEINDEX) * vec2(5.588, 5.588), vec2(0.06711056, 0.00583715))));
    col = clamp(col + (ign - 0.5) * (1.6 / 255.0), 0.0, 1.0);
    gl_FragColor = vec4(col, transparentBg ? c0.a : 1.0);
}
