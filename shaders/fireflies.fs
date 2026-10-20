/*{
  "DESCRIPTION": "Fireflies — a meadow at the edge of a dark forest at dusk, filled with hundreds of fireflies that slowly learn to flash together. Every firefly is a coupled oscillator (the Kuramoto model): it keeps its own rhythm but nudges its phase toward the neighbours it can see, so the meadow drifts from scattered random twinkling into rolling waves of light that sweep across the grass and finally into whole-forest unison — like the synchronous fireflies of the Great Smoky Mountains — before the swarm scatters and starts learning again. Each flash is a real firefly pulse (a fast rise and a slower fade, with a little climb as it lights) in yellow-green with a few amber strays, scattering into the evening mist as physically integrated glow halos, lighting the grass tips, the blades around it and the nearest tree trunks. Layered forest silhouettes recede into blue haze under a deepening dusk sky with the first stars. Beats jolt the swarm and scatter its phases, bass swells the flashes, the level lifts the mist glow, highs twinkle the stars.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #3",
  "CATEGORIES": ["Generator", "Nature", "Light", "Simulation", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Time Speed",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "flashRate",   "LABEL": "Flash Rate (Hz)",   "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 1.5, "DEFAULT": 0.83, "GROUP": "Speed" },
    { "NAME": "drift",       "LABEL": "Firefly Drift",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "camDrift",    "LABEL": "Camera Drift",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "syncCycle",   "LABEL": "Sync Cycle (s)",    "TYPE": "float", "AUDIO": false, "MIN": 20.0, "MAX": 240.0, "DEFAULT": 80.0, "GROUP": "Synchrony" },
    { "NAME": "coupling",    "LABEL": "Coupling Strength", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Synchrony" },
    { "NAME": "syncHold",    "LABEL": "Manual Sync (0=auto cycle)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Synchrony" },
    { "NAME": "range",       "LABEL": "Sight Range",       "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Synchrony" },
    { "NAME": "spread",      "LABEL": "Rhythm Spread",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Synchrony" },
    { "NAME": "population",  "LABEL": "Fireflies",         "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Synchrony" },
    { "NAME": "glowFloor",   "LABEL": "Lantern Glow",      "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.35, "GROUP": "Synchrony" },
    { "NAME": "pacemaker",   "LABEL": "Wave Pacemaker",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Synchrony" },
    { "NAME": "waveSpeed",   "LABEL": "Wave Speed",        "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 20.0, "DEFAULT": 5.0, "GROUP": "Synchrony" },
    { "NAME": "trailLen",    "LABEL": "Long-Exposure Trails", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Synchrony" },
    { "NAME": "jStroke",     "LABEL": "J-Stroke Flight",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Synchrony" },
    { "NAME": "flyColor",    "LABEL": "Firefly Color",     "TYPE": "color", "DEFAULT": [0.72, 1.0, 0.18, 1.0], "GROUP": "Color" },
    { "NAME": "strayColor",  "LABEL": "Stray Color",       "TYPE": "color", "DEFAULT": [1.0, 0.56, 0.12, 1.0], "GROUP": "Color" },
    { "NAME": "strays",      "LABEL": "Amber Strays",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.14, "GROUP": "Color" },
    { "NAME": "skyColor",    "LABEL": "Dusk Horizon",      "TYPE": "color", "DEFAULT": [0.035, 0.07, 0.17, 1.0], "GROUP": "Color" },
    { "NAME": "zenithColor", "LABEL": "Dusk Zenith",       "TYPE": "color", "DEFAULT": [0.003, 0.006, 0.026, 1.0], "GROUP": "Color" },
    { "NAME": "glowColor",   "LABEL": "Afterglow",         "TYPE": "color", "DEFAULT": [0.30, 0.12, 0.10, 1.0], "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "flashBright", "LABEL": "Flash Brightness",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "mist",        "LABEL": "Mist",              "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "haze",        "LABEL": "Forest Haze",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "dusk",        "LABEL": "Dusk Light",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "moonAmt",     "LABEL": "Moon",              "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "stars",       "LABEL": "Stars",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "grassLight",  "LABEL": "Grass Lighting",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.18, "GROUP": "Finish" },
    { "NAME": "exposure",    "LABEL": "Exposure",          "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "sceneRes",    "LABEL": "Scene Resolution",  "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "ffState", "PERSISTENT": true, "WIDTH": "16", "HEIGHT": "16" },
    { "TARGET": "ffPos", "WIDTH": "64", "HEIGHT": "16" },
    { "TARGET": "ffBin", "WIDTH": "960", "HEIGHT": "18" },
    { "TARGET": "ffScene", "WIDTH": "min(floor($WIDTH*$sceneRes),2560)", "HEIGHT": "min(floor($HEIGHT*$sceneRes),1440)" },
    { "TARGET": "ffTrail", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$sceneRes),2560)", "HEIGHT": "min(floor($HEIGHT*$sceneRes),1440)" },
    { "TARGET": "ffFar", "WIDTH": "floor($WIDTH/8)", "HEIGHT": "floor($HEIGHT/8)" },
    { "TARGET": "ffBloom", "WIDTH": "floor($WIDTH/4)", "HEIGHT": "floor($HEIGHT/4)" },
    { "TARGET": "ffBloom2", "WIDTH": "floor($WIDTH/16)", "HEIGHT": "floor($HEIGHT/16)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Fireflies — CURVEBALL pack #3
//   pass 0  ffState : Kuramoto oscillators, 16×16 cells, firefly i in cell
//           (i mod 16, i / 16).
//           r,g = phase (hi/lo packed, survives 8-bit hosts)  b = flash
//   pass 1  ffPos   : per-firefly world position + flash, hi/lo packed
//   pass 2  ffBin   : 40×18 screen tiles × 96 ids of the lit fireflies that can
//           light each tile (per-pixel loops only visit their tile)
//   pass 3  ffScene : moonlit meadow — sky, layered forest silhouettes (edge
//           AA), grass blade layers; per firefly: analytic in-scattering of a
//           point light through mist (∫ 1/(h²+s²) ds), inverse-square light on
//           the surface the pixel sees, bokeh discs for the nearest ones.
//           alpha = depth for the trail pass
//   pass 4  ffTrail : long exposure — crisp occluded cores, max(prev·decay, cur)
//           so every J-stroke flight leaves a streak
//   pass 5  ffFar   : 1/8-res long-range light + halo tails of all fireflies
//   pass 6  ffBloom : quarter-res bright prefilter
//   pass 7  ffBloom2: 1/16-res wide glow
//   pass 8  final   : bloom, fringe, ACES, HD finisher, grain
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define NFLY 256

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vnoise(float x) { float i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f); return mix(hash11(i), hash11(i + 1.0), f); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm1(float x) { float s = 0.0, a = 0.5; for (int i = 0; i < 5; i++) { s += a * vnoise(x); x = x * 2.13 + 7.7; a *= 0.5; } return s; }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { return clamp((x * (2.51 * x + 0.03)) / (x * (2.43 * x + 0.59) + 0.14), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

float bassP() { return smoothstep(0.04, 0.85, audioBass); }
float highP() { return smoothstep(0.08, 0.90, audioHigh); }
float levP()  { return clamp(audioLevel, 0.0, 1.0); }
float beatP() { return clamp(audioBeatPulse, 0.0, 1.0); }

float T()     { return TIME * speed; }

// ── firefly identity + motion (analytic, identical in every pass) ────────
// state / position buffers are addressed by CELL (resolution independent):
// Easel sizes them 16×16 and 32×16; hosts that ignore pass sizes run them at
// canvas size, where only the pixels around each cell centre are computed.
vec2 flyTexel(float id) { return (vec2(mod(id, 16.0), floor(id / 16.0)) + 0.5) / 16.0; }
vec2 cellOf(vec2 fc, vec2 G, out float isAct) {
    vec2 c = floor(fc / R * G);
    vec2 ctr = (c + 0.5) * R / G;
    vec2 d = abs(fc - ctr);
    isAct = step(max(d.x, d.y), 1.5);
    return c;
}

vec3 flyBase(float id) {
    float h1 = hash11(id * 1.37 + 0.11), h2 = hash11(id * 2.71 + 3.3), h3 = hash11(id * 5.13 + 9.1);
    // depth: more fireflies in the middle meadow, a scatter at the forest edge
    float z = (h2 < 0.82) ? 2.2 + 12.5 * pow(h2 / 0.82, 1.15) : 14.7 + 9.0 * (h2 - 0.82) / 0.18;
    float x = (h1 - 0.5) * (3.0 + z * 1.6);
    float y = 0.18 + 1.6 * h3 * h3 + 0.6 * step(0.85, hash11(id * 7.7)) * hash11(id * 3.9);
    // far layer: a scatter of tiny trails drifting above the treeline
    if (h2 > 0.82) y += 1.2 + 1.6 * hash11(id * 4.4);
    // never park a near (bokeh-sized) firefly high in the sky
    y = min(y, 0.35 + 0.16 * z);
    return vec3(x, y, z);
}

vec3 flyPos(float id, float t, float ph) {
    vec3 b = flyBase(id);
    float a = hash11(id * 9.17) * TAU, c = hash11(id * 4.41) * TAU;
    float w = 0.55 * drift;
    vec3 off = vec3(sin(t * 0.13 * (0.7 + hash11(id)) + a) + 0.5 * sin(t * 0.31 + c),
                    0.35 * sin(t * 0.21 * (0.8 + hash11(id + 3.0)) + c),
                    sin(t * 0.11 * (0.7 + hash11(id + 7.0)) + c) + 0.4 * sin(t * 0.27 + a)) * w * (0.6 + 0.04 * b.z);
    // the "J-stroke": while the lantern is lit the firefly swoops forward,
    // dips and climbs sharply (Photinus' courtship flight), then drifts back
    // in the dark. Long-exposure trails turn this into calligraphy.
    float p = fract(ph);
    float sj = clamp(p / 0.30, 0.0, 1.0);
    float back = 1.0 - smoothstep(0.30, 1.0, p);
    float hd = hash11(id * 6.13) * TAU;
    vec2 hdir = vec2(cos(hd), sin(hd) * 0.35);
    float reach = 0.42 * jStroke * (0.7 + 0.6 * hash11(id * 2.2));
    vec3 J = vec3(hdir.x * sj, 0.0, hdir.y * sj) * reach;
    J.y = (0.62 * sj * sj - 0.16 * sj) * reach;
    return b + off + J * back;
}

float flyAlive(float id) { return step(id / float(NFLY), population); }

// real firefly pulse: ~0.1 s rise, ~0.15 s fade, then dark
float pulse(float ph) {
    float p = fract(ph);
    float per = 1.0 / max(flashRate, 0.05);
    float tr = 0.09 / per, td = 0.13 / per;
    float up = smoothstep(0.0, tr, p);
    float dn = exp(-max(p - tr, 0.0) / td);
    // a dim lantern afterglow so the swarm never vanishes between flashes
    float tail = 0.05 * exp(-max(p - tr, 0.0) / (0.6 / per));
    return up * (dn * 0.9 + tail) + 0.035 * glowFloor;
}

// coupling envelope over the sync cycle: random → waves → unison → scatter
// phases of the cycle:  0.00-0.10 random  ·  0.10-0.58 pacemaker WAVES
// (local coupling + a faster pacemaker group → phase fronts sweep outward)
// · 0.60-0.78 global UNISON  ·  0.78-0.86 scatter  ·  0.86-1.0 random
float cyc(float t) { return syncHold > 0.001 ? 0.7 : fract(t / syncCycle); }
float waveW(float t)   { float c = cyc(t); return smoothstep(0.08, 0.16, c) * (1.0 - smoothstep(0.56, 0.62, c)); }
float unisonW(float t) { float c = cyc(t); return smoothstep(0.58, 0.64, c) * (1.0 - smoothstep(0.78, 0.81, c)); }
float couplingK(float t) {
    if (syncHold > 0.001) return syncHold * 3.2 * coupling;
    return (0.9 * waveW(t) + 2.9 * unisonW(t)) * coupling;
}
vec3 pacePos(float t) {
    float ci = floor(t / syncCycle);
    float m = hash11(ci * 3.7 + 1.3);
    if (m < 0.4) return vec3(-9.0, 0.8, 7.0);          // sweeps left → right
    if (m < 0.7) return vec3(9.0, 0.8, 7.5);           // right → left
    return vec3(0.0, 0.8, 8.5 + 3.0 * hash11(ci));     // rings from the centre
}
float scatterKick(float t) {
    if (syncHold > 0.001) return 0.0;
    float c = fract(t / syncCycle);
    return smoothstep(0.78, 0.80, c) * (1.0 - smoothstep(0.82, 0.86, c));
}

vec4 packPhase(float ph, float fl) {
    ph = fract(ph);
    float hi = floor(ph * 255.0) / 255.0;
    float lo = (ph - hi) * 255.0;
    return vec4(hi, lo, fl, 1.0);
}
float unpackPhase(vec4 s) { return s.r + s.g / 255.0; }
float readPhase(float id) { return unpackPhase(texture2D(ffState, flyTexel(id))); }

// ═════════════════════════════════════════════════════════════════════════
//  PASS 0 — Kuramoto
// ═════════════════════════════════════════════════════════════════════════
vec4 stateStep(vec2 fc) {
    float act;
    vec2 cell = cellOf(fc, vec2(16.0), act);
    if (act < 0.5) return vec4(0.0);
    float id = cell.y * 16.0 + cell.x;
    float t = T();
    if (FRAMEINDEX < 2) {
        float ph = hash11(id * 3.17 + 0.5);
        return packPhase(ph, 0.0);
    }
    float th = readPhase(id);
    vec3 pi = flyPos(id, t, th);
    float L = mix(1.7 * range, 40.0, unisonW(t));
    float sumS = 0.0, sumW = 0.0;
    for (int j = 0; j < NFLY; j++) {
        float fj = float(j);
        if (fj == id) continue;
        float tj = readPhase(fj);
        vec3 pj = flyPos(fj, t, tj);
        // a firefly only "sees" the lit neighbours: coupling weighted by
        // distance and by how visible the other one's flash is
        float d = length(pj - pi);
        float w = exp(-d / L) * flyAlive(fj);
        sumS += w * sin(TAU * (tj - th));
        sumW += w;
    }
    float dt = min(TIMEDELTA, 0.05) * speed;
    float f0 = flashRate * (1.0 + 0.085 * spread * (hash11(id * 6.61 + 1.7) * 2.0 - 1.0));
    // wave phase: each firefly is entrained by the signal arriving from the
    // pacemaker group — the pacemaker's phase delayed by distance / wave
    // speed — so flashes sweep outward as fronts (left→right or rings)
    float dpm = length((pi - pacePos(t)).xz);
    float psi = flashRate * (t - dpm / max(waveSpeed, 0.5));
    float Kf = 3.4 * pacemaker * waveW(t) * smoothstep(0.08, 0.30, cyc(t));
    float K = couplingK(t);
    float dth = f0 + K / TAU * sumS / max(sumW, 1e-3) + Kf / TAU * sin(TAU * (psi - th));
    // desync kicks: the cycle's scatter phase + beats jolting the swarm
    float kick = scatterKick(t) * 0.9 + audioReact * beatP() * 0.10;
    float rnd = hash12(vec2(id, floor(TIME * 30.0))) - 0.5;
    float inc = dth * dt + kick * rnd * dt * 6.0;
    th += inc;
    // b = this frame's phase increment, so the next pass can rebuild where
    // the firefly was one frame ago (continuous long-exposure streaks)
    return packPhase(th, clamp(inc * 10.0, 0.0, 1.0));
}

// ═════════════════════════════════════════════════════════════════════════
//  PASS 1 — per-firefly position + flash, packed hi/lo (computed once)
// ═════════════════════════════════════════════════════════════════════════
vec2 pk(float v) { v = clamp(v, 0.0, 0.99999); float hi = floor(v * 255.0) / 255.0; return vec2(hi, (v - hi) * 255.0); }
float upk(vec2 a) { return a.x + a.y / 255.0; }
// ffPos layout (rgb only, alpha always 1): block 0 = (x hi, x lo, y hi),
// block 1 = (y lo, z hi, z lo), block 2 = (flash, -, -), block 3 = velocity
vec2 posUV(float id, float blk) { return (vec2(mod(id, 16.0) + 16.0 * blk, floor(id / 16.0)) + 0.5) / vec2(64.0, 16.0); }
vec3 readVel(float id) { return (texture2D(ffPos, posUV(id, 3.0)).rgb - 0.5) / 2.5; }
vec4 readFly(float id, out vec3 F) {
    vec3 A = texture2D(ffPos, posUV(id, 0.0)).rgb;
    vec3 B = texture2D(ffPos, posUV(id, 1.0)).rgb;
    vec3 C = texture2D(ffPos, posUV(id, 2.0)).rgb;
    F = vec3(upk(A.xy), upk(vec2(A.z, B.x)), upk(B.yz)) * vec3(80.0, 10.0, 50.0) - vec3(40.0, 2.0, 1.0);
    return vec4(0.0, 0.0, C.r, 1.0);
}

vec4 posStep(vec2 fc) {
    float act;
    vec2 cell = cellOf(fc, vec2(64.0, 16.0), act);
    if (act < 0.5) return vec4(0.0);
    float blk = floor(cell.x / 16.0);
    float id = cell.y * 16.0 + cell.x - 16.0 * blk;
    vec4 st = texture2D(ffState, flyTexel(id));
    float ph = unpackPhase(st);
    vec3 F = flyPos(id, T(), ph);
    if (blk > 2.5) {
        // motion over the last frame (for continuous long-exposure streaks):
        // this buffer is persistent, so blocks A/B still hold last frame's position
        float dt = min(TIMEDELTA, 0.05) * speed;
        vec3 Fp = flyPos(id, T() - dt, ph - st.b * 0.1);
        vec3 d = F - Fp;
        if (FRAMEINDEX < 3 || dot(d, d) > 0.09) d = vec3(0.0);
        return vec4(clamp(d * 2.5 + 0.5, 0.0, 1.0), 1.0);
    }
    vec3 n = (F + vec3(40.0, 2.0, 1.0)) / vec3(80.0, 10.0, 50.0);
    vec2 px_ = pk(n.x), py_ = pk(n.y), pz_ = pk(n.z);
    if (blk < 0.5) return vec4(px_, py_.x, 1.0);
    if (blk < 1.5) return vec4(py_.y, pz_, 1.0);
    return vec4(pulse(ph) * flyAlive(id), 0.0, 0.0, 1.0);
}
// ═════════════════════════════════════════════════════════════════════════
//  PASS 2 — the meadow
// ═════════════════════════════════════════════════════════════════════════
vec3 g_ro, g_rd;

vec3 skyCol(vec3 d) {
    float y = d.y;
    vec3 c = mix(skyColor.rgb * dusk * 0.8, zenithColor.rgb, smoothstep(-0.02, 0.36, y));
    // afterglow band low on the horizon, off to one side
    float az = atan(d.x, d.z);
    float glow = exp(-max(y, 0.0) * 9.0) * (0.35 + 0.65 * exp(-pow((az + 0.55) * 1.6, 2.0)));
    c += glowColor.rgb * glow * 0.55 * dusk;
    // crescent moon low over the trees
    vec3 md = normalize(vec3(-0.55, 0.23, 1.0));
    float mdot = dot(d, md);
    float mr = acos(clamp(mdot, -1.0, 1.0));
    float disk = smoothstep(0.0158, 0.0146, mr);
    vec3 sd = normalize(md + vec3(0.0085, 0.0045, 0.0));
    float shadow = smoothstep(0.0158, 0.0146, acos(clamp(dot(d, sd), -1.0, 1.0)));
    c += vec3(1.0, 0.93, 0.78) * disk * (1.0 - shadow * 0.97) * 0.85 * moonAmt;
    c += vec3(0.55, 0.62, 0.8) * exp(-mr * 9.0) * 0.07 * moonAmt + vec3(0.6, 0.65, 0.8) * exp(-mr * 45.0) * 0.06 * moonAmt;
    // stars
    if (y > 0.02) {
        vec2 sp = vec2(az * 260.0, y * 260.0);
        vec2 ci = floor(sp), cf = fract(sp) - 0.5;
        float h = hash12(ci);
        if (h > 0.965) {
            vec2 o = (vec2(hash12(ci + 3.1), hash12(ci + 7.3)) - 0.5) * 0.6;
            float r = length(cf - o);
            float tw = 0.65 + 0.35 * sin(TIME * (2.0 + 5.0 * hash12(ci + 1.1)) + h * 50.0) * (0.5 + 0.8 * highP() * audioReact);
            float b = pow(hash12(ci + 9.9), 6.0) * 2.2 + 0.25;
            c += vec3(0.85, 0.9, 1.0) * b * tw * smoothstep(0.16, 0.02, r) * smoothstep(0.02, 0.25, y) * stars;
        }
    }
    return c;
}

// forest layer: 1 if the point (x, y) on the layer plane is foliage, trunk or understory.
// .y = edge proximity (0 deep inside the mass, 1 at the silhouette) for rim light
float g_leafFine = 1.0;
float g_fsd = -1.0, g_fcell = 3.0;
float leafN(vec2 p) {
    float f = g_leafFine;
    return (vnoise2(p) * 0.55 + vnoise2(p * 2.7 + 5.1) * 0.3 + (vnoise2(p * 7.9 + 1.7) - 0.5) * 0.15 * f + 0.075);
}
float fbm3(float x) { float s = 0.0, a = 0.5; for (int i = 0; i < 3; i++) { s += a * vnoise(x); x = x * 2.13 + 7.7; a *= 0.5; } return s + 0.06; }
vec2 forestHit(float x, float y, float z, float seed) {
    float cell = 2.4 + 0.08 * z;
    if (y < 0.0 || y > (0.75 + 0.075 * z) * 1.3 + cell * 1.5 + 0.3) return vec2(0.0);
    float cx = x / cell;
    float field = -1e3;
    for (int k = -1; k <= 1; k++) {
        float ci = floor(cx) + float(k);
        float h = hash11(ci * 1.73 + seed);
        float c0 = (ci + 0.3 + 0.4 * hash11(ci * 3.1 + seed)) * cell;
        float rad = cell * (0.55 + 0.5 * h);
        float top = (0.75 + 0.075 * z) * (0.7 + 0.6 * hash11(ci * 5.9 + seed));
        // crown = ellipse sitting on its trunk, tall for some (poplar), wide for others
        float tall = mix(0.8, 1.9, step(0.6, hash11(ci * 8.3 + seed)));
        vec2 q = vec2((x - c0) / rad, (y - top) / (rad * 0.75 * tall));
        field = max(field, 1.0 - length(q));
    }
    vec2 lp = vec2(x, y) * (2.2 / (1.0 + 0.02 * z)) + seed * 7.0;
    g_leafFine = 1.0 - smoothstep(6.0, 22.0, z);
    float f = field;
    if (abs(field) < 0.2) f = field + (leafN(lp) - 0.5) * 0.34;
    float canopy = step(0.0, f);
    // understory shrubs with a leafy 2D edge
    float shrubTop = 0.55 + 0.025 * z + 0.7 * fbm3(x * 0.7 + seed * 2.0);
    float fs = (shrubTop - y) / 0.5;
    if (abs(fs) < 0.45) fs += (leafN(lp * 1.3 + 3.0) - 0.5) * 0.8;
    float shrub = step(0.0, fs);
    // trunks
    float tcw = 1.2 + 0.03 * z;
    float ti = floor(x / tcw);
    float trunk = 0.0;
    if (hash11(ti * 2.3 + seed) > 0.3) {
        float tx = (ti + 0.5 + 0.35 * (hash11(ti * 7.1 + seed) - 0.5)) * tcw;
        float w = (0.06 + 0.12 * hash11(ti * 9.7 + seed)) * (1.0 - 0.15 * y / 5.0);
        trunk = step(abs(x - tx - 0.06 * sin(y * 1.3 + ti)), w) * step(y, 0.8 + 0.09 * z);
    }
    float hit = max(max(canopy, shrub), trunk) * step(0.0, y);
    float e = 1.0 - clamp(max(f, fs * 0.3) / 0.25, 0.0, 1.0);
    g_fsd = max(max(f, fs * 0.5), trunk > 0.5 ? 1.0 : -1.0);   // signed field for edge AA
    g_fcell = cell;
    return vec2(hit, e * hit);
}

// near framing tree on the left + overhanging leaves at the top
float nearTreeHit(float x, float y) {
    if (x < -3.3 || x > 1.6 || y < 0.0) return 0.0;
    float tx = -2.7 + 0.12 * sin(y * 0.9);
    if (y < 1.6 && abs(x - tx) > 0.45) return 0.0;
    float w = 0.36 - 0.025 * y + 0.025 * (vnoise(y * 9.0) - 0.5);
    float trunk = step(abs(x - tx), w) * step(y, 6.0);
    float by = 2.25 + 0.3 * sin(x * 0.7 + 0.4) + 0.16 * (x - tx);
    float branch = step(abs(y - by), 0.05 * (1.7 - 0.45 * (x - tx))) * step(tx, x) * step(x, tx + 3.2);
    vec2 q = vec2(x - tx - 2.2, y - by - 0.55);
    float n = vnoise2(vec2(x, y) * 4.0) * 0.5 + vnoise2(vec2(x, y) * 13.0) * 0.3 + vnoise2(vec2(x, y) * 37.0) * 0.2;
    float r = length(q * vec2(0.45, 1.0));
    float leafy = step(r, 0.55 + 0.7 * n) * step(1.7, y);
    vec2 q2 = vec2(x - tx - 0.6, y - 3.4);
    float leafy2 = step(length(q2 * vec2(0.5, 0.9)), 0.6 + 0.8 * n) * step(2.6, y);
    return max(max(trunk, branch), max(leafy, leafy2));
}

// grass layer at depth z: returns blade coverage + tipness (for rim light)
vec2 grassHit(float x, float y, float z, float seed) {
    float s = 0.055 + 0.004 * z;
    float cx = x / s;
    vec2 best = vec2(0.0);
    for (int k = -1; k <= 1; k++) {
        float ci = floor(cx) + float(k);
        float h = hash11(ci * 1.91 + seed);
        float tall = step(0.86, h);                       // seed-head stems
        float hgt = (0.16 + 0.42 * h * h) * (0.75 + 0.5 * vnoise(ci * 0.07 + seed)) * (1.0 + 0.45 * tall);
        float yy = y / hgt;
        if (yy < -0.05 || yy > 1.08) continue;
        float sway = 0.22 * sin(TIME * (0.9 + 0.5 * hash11(ci + seed)) * speed + ci * 0.37) + 0.12 * (vnoise(ci * 0.05 + TIME * 0.3 * speed) - 0.5);
        float lean = (hash11(ci * 4.7 + seed) - 0.5) * 0.9 + sway;
        float bend = pow(max(yy, 0.0), 1.7);
        float c0 = (ci + 0.5 + 0.4 * (hash11(ci * 3.3 + seed) - 0.5)) * s + lean * hgt * bend * (1.0 + 0.6 * tall);
        float wv = 0.10 + 0.32 * hash11(ci * 7.9 + seed);
        float w = s * mix(wv * (1.0 - min(yy, 1.0) * 0.9), 0.05, tall);
        float d = abs(x - c0) - w;
        if (d < 0.0 && yy <= 1.0) best = max(best, vec2(1.0, yy));
        // oat-like seed head hanging from the tall stems
        if (tall > 0.5 && yy > 0.72) {
            vec2 sh = vec2(x - c0 - lean * hgt * 0.06, y - hgt * 0.95);
            float e = length(sh / vec2(s * 0.16, hgt * 0.13));
            if (e < 1.0) best = max(best, vec2(1.0, 1.0));
        }
    }
    return best;
}

float g_pxAng, g_yaw;
void camPose() {
    float t = T();
    float cd = camDrift;
    g_ro = vec3(1.4 * sin(t * 0.021) * cd, 0.52 + 0.04 * sin(t * 0.05) * cd, 0.6 * sin(t * 0.017) * cd);
    g_yaw = 0.06 * sin(t * 0.013 + 1.0) * cd;
}
void setupCam(vec2 fc) {
    vec2 uv = (fc - 0.5 * R) / R.y;
    camPose();
    vec3 rd = normalize(vec3(uv.x, uv.y + 0.07, 1.25));
    rd.xz = mat2(cos(g_yaw), -sin(g_yaw), sin(g_yaw), cos(g_yaw)) * rd.xz;
    g_rd = rd;
    g_pxAng = 1.0 / R.y / 1.25;
}
// world → screen uv (y up, height-normalised, same space as setupCam's uv); .z = view depth
vec3 projectW(vec3 w) {
    vec3 q = w - g_ro;
    float c = cos(g_yaw), s_ = sin(g_yaw);
    vec2 xz = mat2(c, s_, -s_, c) * q.xz;
    float z = max(xz.y, 1e-3);
    return vec3(1.25 * xz.x / z, 1.25 * q.y / z - 0.07, xz.y);
}

// ── screen-space binning ─────────────────────────────────────────────────
// 40×18 tiles cover uv.x ∈ [-1.2, 1.2] (any aspect up to 2.4:1), uv.y ∈ [-0.5, 0.5].
// Each tile owns 24 texels × 4 channels = up to 96 firefly ids (nearest first) (stored id+1,
// 0 = empty) of the lit fireflies whose light can reach it. Per-pixel loops
// then visit only their tile's list instead of all 256.
#define TILES_X 40.0
#define TILES_Y 18.0
#define TILE_SLOTS 24.0
#define LIGHT_R 1.15
// light reach shrinks with flash brightness (dim lanterns only need a small bin footprint)
float reachR(float fl) { return LIGHT_R * clamp(sqrt(fl * 1.6), 0.18, 1.0); }
vec2 tileOf(vec2 uv) {
    return clamp(floor(vec2((uv.x + 1.2) / 2.4 * TILES_X, (uv.y + 0.5) * TILES_Y)), vec2(0.0), vec2(TILES_X - 1.0, TILES_Y - 1.0));
}
vec4 binStep(vec2 fc) {
    float act;
    vec2 cell = cellOf(fc, vec2(TILES_X * TILE_SLOTS, TILES_Y), act);
    if (act < 0.5) return vec4(0.0);
    float tx = floor(cell.x / TILE_SLOTS);
    float slot = cell.x - tx * TILE_SLOTS;
    vec2 lo = vec2(-1.2 + tx * 2.4 / TILES_X, -0.5 + cell.y / TILES_Y);
    vec2 hi = lo + vec2(2.4 / TILES_X, 1.0 / TILES_Y);
    camPose();
    float first = slot * 4.0;
    float cnt = 0.0;
    vec4 outv = vec4(0.0);
    // two sweeps: near fireflies (they light the most pixels) claim slots first
    for (int i = 0; i < NFLY * 2; i++) {
        float id = mod(float(i), float(NFLY));
        float sweep = step(float(NFLY), float(i));
        vec3 F;
        vec4 B = readFly(id, F);
        if (B.z < 0.004) continue;
        vec3 ph = projectW(F);
        if (ph.z < 0.3) continue;
        if ((ph.z < 9.0) == (sweep > 0.5)) continue;
        vec3 pt = projectW(F - readVel(id));
        float rad = max(reachR(B.z) * 1.25 / ph.z, 0.012) + 0.004;
        vec2 bmin = min(ph.xy, pt.xy) - rad, bmax = max(ph.xy, pt.xy) + rad;
        if (bmax.x < lo.x || bmin.x > hi.x || bmax.y < lo.y || bmin.y > hi.y) continue;
        // disc-vs-rect refinement for the halo/light radius around the head
        vec2 dq = max(max(lo - ph.xy, ph.xy - hi), 0.0);
        vec2 dt_ = max(max(lo - pt.xy, pt.xy - hi), 0.0);
        if (min(dot(dq, dq), dot(dt_, dt_)) > rad * rad) continue;
        float k = cnt - first;
        if (k >= 0.0 && k < 4.0) {
            if (k < 0.5) outv.x = id + 1.0;
            else if (k < 1.5) outv.y = id + 1.0;
            else if (k < 2.5) outv.z = id + 1.0;
            else outv.w = id + 1.0;
        }
        cnt += 1.0;
        if (cnt >= first + 4.0) break;
    }
    return outv;
}
vec4 binTexel(vec2 tile, float s) {
    return texture2D(ffBin, vec2((tile.x * TILE_SLOTS + s + 0.5) / (TILES_X * TILE_SLOTS), (tile.y + 0.5) / TILES_Y));
}

vec3 fogColor() { return mix(skyColor.rgb * dusk * 0.75, glowColor.rgb * 0.08 * dusk, 0.12); }
vec3 surfaceCol(int kind, float depth, float tip) {
    vec3 moonC = vec3(0.24, 0.34, 0.62) * moonAmt;
    vec3 base;
    if (kind == 1) base = vec3(0.003, 0.006, 0.012) + moonC * 0.16 * tip * tip;
    else if (kind == 2) base = mix(vec3(0.003, 0.006, 0.013), vec3(0.010, 0.020, 0.040), tip) + moonC * 0.22 * pow(tip, 2.5);
    else if (kind == 3) base = vec3(0.003, 0.006, 0.013);
    else base = vec3(0.0015, 0.0025, 0.005) + moonC * 0.015;
    float fogAmt = 1.0 - exp(-depth * 0.042 * haze);
    return mix(base, fogColor(), fogAmt);
}

// PASS 2 — the meadow without the firefly cores (cores live in the trail
// buffer). alpha = scene depth / 64 for the trail pass's occlusion test
vec4 renderScene(vec2 fc) {
    setupCam(fc);
    vec3 ro = g_ro, rd = g_rd;
    float t = T();

    float depth = 64.0;
    int kind = 0;               // 0 sky, 1 forest, 2 grass, 3 ground, 4 near tree
    vec3 P = ro + rd * depth;
    float tip = 0.0;
    float rz = max(rd.z, 1e-3);
    float edgeCov = 1.0;        // partial coverage of a forest silhouette pixel
    float frontDepth = 0.0, frontTip = 0.0;
    bool partial = false;

    {
        float zt = 3.2 - ro.z;
        vec3 q = ro + rd * (zt / rz);
        if (nearTreeHit(q.x, q.y) > 0.5) { depth = zt / rz; P = q; kind = 4; }
    }
    if (kind == 0 && rd.y < 0.08) {
        for (int k = 0; k < 7; k++) {
            float zk = 1.05 * pow(1.55, float(k));
            float tl = (zk - ro.z) / rz;
            vec3 q = ro + rd * tl;
            if (q.y < -0.03) continue;
            if (q.y > 0.95) continue;
            vec2 gh = grassHit(q.x, q.y, zk, float(k) * 17.3);
            if (gh.x > 0.5) { depth = tl; P = q; kind = 2; tip = gh.y; break; }
        }
    }
    if (kind == 0) {
        for (int k = 0; k < 4; k++) {
            float zk = 13.0 + 7.0 * float(k) + 4.0 * float(k * k);
            float tl = (zk - ro.z) / rz;
            vec3 q = ro + rd * tl;
            g_fsd = -1.0;
            vec2 fh = forestHit(q.x, q.y, zk, 11.0 + float(k) * 5.7);
            // analytic edge coverage over the pixel footprint
            float fw = 1.3 * tl * g_pxAng / max(g_fcell * 0.6, 0.3);
            float cov = smoothstep(-fw, fw, g_fsd);
            if (fh.x > 0.5 && !partial && cov > 0.98) { depth = tl; P = q; kind = 1; tip = fh.y; break; }
            if (!partial && cov > 0.02) {
                partial = true; edgeCov = cov; frontDepth = tl; frontTip = max(fh.y, 0.7);
                continue;
            }
            if (partial && fh.x > 0.5) { depth = tl; P = q; kind = 1; tip = fh.y; break; }
        }
    }
    if (kind == 0 && rd.y < 0.0) {
        float tg = -ro.y / rd.y;
        if (tg < depth) { depth = tg; P = ro + rd * tg; kind = 3; }
    }

    vec3 col = (kind == 0) ? skyCol(rd) : surfaceCol(kind, depth, tip);
    if (partial) {
        col = mix(col, surfaceCol(1, frontDepth, frontTip), edgeCov);
        if (edgeCov > 0.5) { depth = frontDepth; P = ro + rd * depth; kind = 1; tip = frontTip; }
    }
    float fogAmt = 1.0 - exp(-depth * 0.042 * haze);

    // ── firefly light: mist halos, light on surfaces, near bokeh discs ─────
    float sigma = 0.035 * mist * (1.0 + 0.35 * audioReact * levP());
    float bright = flashBright * (1.0 + 0.45 * audioReact * bassP());
    vec3 glowAcc = vec3(0.0), lightAcc = vec3(0.0), bokAcc = vec3(0.0);
    vec2 tile = tileOf((fc - 0.5 * R) / R.y);
    vec4 bt = vec4(0.0);
    for (int i = 0; i < 96; i++) {
        float k = mod(float(i), 4.0);
        if (k < 0.5) bt = binTexel(tile, floor(float(i) / 4.0));
        float idp = k < 0.5 ? bt.x : (k < 1.5 ? bt.y : (k < 2.5 ? bt.z : bt.w));
        if (idp < 0.5) break;
        float id = idp - 1.0;
        vec3 F;
        vec4 B = readFly(id, F);
        float fl = B.z;
        if (fl < 0.004) continue;
        vec3 q = F - ro;
        float b = dot(q, rd);
        vec3 perp = q - rd * b;
        float h2 = dot(perp, perp);
        vec3 Ls = F - P;
        float ds2 = dot(Ls, Ls);
        bool nearB = b < 2.6 && b > 0.25;
        float rr = reachR(fl);
        if (h2 > rr * rr && (kind == 0 || ds2 > rr * rr) && !nearB) continue;
        float dist = length(q);
        vec3 fc0 = mix(flyColor.rgb, strayColor.rgb, step(1.0 - strays, hash11(id * 8.31 + 2.0)));
        float I = fl * bright * (0.8 + 0.4 * hash11(id * 1.9));
        float h = sqrt(h2) + 0.006;
        float a0 = atan(-b / h), a1 = atan((depth - b) / h);
        float sc = sigma * (a1 - a0) / h * exp(-h * 3.4) * exp(-dist * 0.02 * haze) * (1.0 - smoothstep(0.6 * min(rr, 0.9), min(rr, 0.9), h));
        glowAcc += fc0 * I * sc * 0.10;
        if (nearB && b < depth) {
            // out-of-focus foreground firefly: a soft-edged bokeh disc with a rim
            float ra = 0.0035 + 0.016 * (2.6 - b);
            float ang = h / b;
            float disc = smoothstep(ra, ra * 0.86, ang);
            float rim = smoothstep(ra * 0.7, ra * 0.95, ang) * disc;
            bokAcc += (fc0 * 0.85 + 0.15) * I * (disc * 0.7 + rim * 0.5) * min(0.0009 / (ra * ra), 1.2) * 0.9;
        }
        if (kind != 0) {
            float facing = (kind == 2) ? (0.55 + 0.45 * tip) : ((kind == 3) ? 0.6 : 1.0);
            float lfade = 1.0 - smoothstep(0.6 * rr * rr, rr * rr, ds2);
            lightAcc += fc0 * I * facing / (ds2 * 14.0 + 0.03) * exp(-sqrt(ds2) * 0.5) * lfade;
        }
    }
    lightAcc *= 0.22 * grassLight * (1.0 - fogAmt * 0.7);
    lightAcc /= 1.0 + lum(lightAcc) * 1.2;
    glowAcc /= 1.0 + lum(glowAcc) * 0.8;
    col += lightAcc + glowAcc + bokAcc;
    col *= exposure;
    return vec4(col, clamp(depth / 64.0, 0.0, 1.0));
}

// PASS 3 — long exposure: crisp cores of every lit firefly, occluded by the
// scene depth, accumulated with decay so flight paths become streaks
vec4 trailStep(vec2 fc) {
    setupCam(fc);
    vec3 ro = g_ro, rd = g_rd;
    vec2 st = fc / R;
    float depth = texture2D(ffScene, st).a * 64.0;
    vec3 prev = texture2D(ffTrail, st).rgb;
    if (FRAMEINDEX < 2) prev = vec3(0.0);
    float tau = 0.62 * trailLen;
    float decay = tau > 0.001 ? exp(-min(TIMEDELTA, 0.05) * speed / tau) : 0.0;
    float bright = flashBright * (1.0 + 0.45 * audioReact * bassP());
    vec3 cur = vec3(0.0);
    vec2 tile = tileOf((fc - 0.5 * R) / R.y);
    vec4 bt = vec4(0.0);
    for (int i = 0; i < 96; i++) {
        float k = mod(float(i), 4.0);
        if (k < 0.5) bt = binTexel(tile, floor(float(i) / 4.0));
        float idp = k < 0.5 ? bt.x : (k < 1.5 ? bt.y : (k < 2.5 ? bt.z : bt.w));
        if (idp < 0.5) break;
        float id = idp - 1.0;
        vec3 F;
        vec4 B = readFly(id, F);
        float fl = B.z;
        if (fl < 0.004) continue;
        vec3 q = F - ro;
        float b = dot(q, rd);
        if (b < 2.6 || b > depth) continue;          // near ones are bokeh; occluded ones hidden
        float r = max(0.0085, b * g_pxAng * 0.75);
        // closest approach of the ray to the segment travelled this frame
        vec3 dv = readVel(id);
        vec3 qa = q - dv;
        vec3 pa = qa - rd * dot(qa, rd);
        vec3 pb = q - rd * b;
        vec3 ab = pb - pa;
        float tt = clamp(-dot(pa, ab) / max(dot(ab, ab), 1e-8), 0.0, 1.0);
        vec3 perp = pa + ab * tt;
        float h2 = dot(perp, perp);
        if (h2 > r * r * 30.0) continue;
        vec3 fc0 = mix(flyColor.rgb, strayColor.rgb, step(1.0 - strays, hash11(id * 8.31 + 2.0)));
        float I = fl * bright * (0.8 + 0.4 * hash11(id * 1.9));
        float sz = min((0.0085 * 0.0085) / (r * r), 1.0);
        float core = exp(-h2 / (r * r)) * sz;
        float halo = exp(-sqrt(h2) / (r * 2.2)) * 0.18 * sz;
        cur += (fc0 * 0.72 + 0.28 * vec3(1.0, 1.0, 0.85)) * I * (core * 2.4 + halo) * exp(-b * 0.022 * haze);
    }
    // long exposure: keep the brighter of the fading streak and the new light
    vec3 tr = max(prev * decay, cur);
    return vec4(tr, 1.0);
}

// PASS 5 — far field at 1/8 res: the long-range tails of every firefly's light
// and mist halo (beyond the binned reach). Smooth by nature, so cheap here;
// this is what makes the whole meadow glow when the swarm fires together.
vec4 farStep(vec2 fc) {
    setupCam(fc);
    vec3 ro = g_ro, rd = g_rd;
    float depth = texture2D(ffScene, fc / R).a * 64.0;
    vec3 P = ro + rd * depth;
    bool sky = depth > 63.0;
    float sigma = 0.035 * mist * (1.0 + 0.35 * audioReact * levP());
    float bright = flashBright * (1.0 + 0.45 * audioReact * bassP());
    vec3 glowAcc = vec3(0.0), lightAcc = vec3(0.0);
    for (int i = 0; i < NFLY; i++) {
        float id = float(i);
        vec3 F;
        vec4 B = readFly(id, F);
        float fl = B.z;
        if (fl < 0.004) continue;
        float rr = reachR(fl);
        vec3 q = F - ro;
        float b = dot(q, rd);
        vec3 perp = q - rd * b;
        float h2 = dot(perp, perp);
        vec3 Ls = F - P;
        float ds2 = dot(Ls, Ls);
        vec3 fc0 = mix(flyColor.rgb, strayColor.rgb, step(1.0 - strays, hash11(id * 8.31 + 2.0)));
        float I = fl * bright * (0.8 + 0.4 * hash11(id * 1.9));
        float h = sqrt(h2) + 0.006;
        float hw = smoothstep(0.6 * min(rr, 0.9), min(rr, 0.9), h) * (1.0 - smoothstep(1.0, 1.4, h));
        if (hw > 0.0) {
            float a0 = atan(-b / h), a1 = atan((depth - b) / h);
            glowAcc += fc0 * I * sigma * (a1 - a0) / h * exp(-h * 3.4) * exp(-length(q) * 0.02 * haze) * hw * 0.10;
        }
        if (!sky && ds2 < 9.0) {
            float lw = smoothstep(0.6 * rr * rr, rr * rr, ds2);
            lightAcc += fc0 * I * 0.75 / (ds2 * 14.0 + 0.03) * exp(-sqrt(ds2) * 0.5) * lw * (1.0 - smoothstep(4.0, 9.0, ds2));
        }
    }
    float fogAmt = 1.0 - exp(-depth * 0.042 * haze);
    lightAcc *= 0.22 * grassLight * (1.0 - fogAmt * 0.7);
    vec3 c = (lightAcc + glowAcc) * exposure;
    return vec4(c / (1.0 + lum(c) * 1.2), 1.0);
}

// ═════════════════════════════════════════════════════════════════════════
void main() {
    vec2 fc = gl_FragCoord.xy;
    if (PASSINDEX == 0) { gl_FragColor = stateStep(fc); return; }
    if (PASSINDEX == 1) { gl_FragColor = posStep(fc); return; }
    if (PASSINDEX == 2) { gl_FragColor = binStep(fc); return; }
    if (PASSINDEX == 3) { gl_FragColor = renderScene(fc); return; }
    if (PASSINDEX == 4) { gl_FragColor = trailStep(fc); return; }
    if (PASSINDEX == 5) { gl_FragColor = farStep(fc); return; }
    if (PASSINDEX == 7) {
        // 1/16-res wide glow from the quarter-res prefilter (gaussian-ish 4×4)
        vec2 st = fc / R;
        vec2 px = 1.0 / R;
        vec3 s = vec3(0.0); float wsum = 0.0;
        for (int y = -2; y <= 2; y++) for (int x = -2; x <= 2; x++) {
            float w = exp(-float(x * x + y * y) * 0.35);
            s += texture2D(ffBloom, st + vec2(float(x), float(y)) * px * 0.55).rgb * w; wsum += w;
        }
        gl_FragColor = vec4(s / wsum, 1.0);
        return;
    }
    if (PASSINDEX == 6) {
        vec2 st = fc / R;
        vec2 px = 1.0 / R;
        vec3 s = vec3(0.0);
        for (int y = -1; y <= 1; y++) for (int x = -1; x <= 1; x++) {
            vec2 o = st + vec2(float(x), float(y)) * px * 0.7;
            vec3 c = texture2D(ffScene, o).rgb + texture2D(ffTrail, o).rgb + texture2D(ffFar, o).rgb;
            s += max(c - 0.25, 0.0);
        }
        gl_FragColor = vec4(s / 9.0, 1.0);
        return;
    }
    // ── final ────────────────────────────────────────────────────────────
    vec2 st = fc / R;
    vec2 uvc = (fc - 0.5 * R) / R.y;
    vec2 asp = vec2(R.y / R.x, 1.0);
    vec4 c0 = texture2D(ffScene, st);
    float ab = aberration * 0.0025 * length(uvc);
    vec2 dir = normalize(uvc + 1e-4);
    vec3 col = vec3(texture2D(ffScene, st + dir * ab * asp).r, c0.g, texture2D(ffScene, st - dir * ab * asp).b);
    vec3 trl = texture2D(ffTrail, st).rgb;
    col += trl + texture2D(ffFar, st).rgb;
    // bloom: fixed spiral taps over the quarter-res prefilter, two radii
    vec3 bl = vec3(0.0);
    vec2 qpx = 4.0 / R;
    float wsum = 0.0;
    for (int y = -2; y <= 2; y++) for (int x = -2; x <= 2; x++) {
        float w = exp(-float(x * x + y * y) * 0.45);
        bl += texture2D(ffBloom, st + vec2(float(x), float(y)) * qpx * 0.8).rgb * w; wsum += w;
    }
    bl /= wsum;
    vec3 bw = vec3(0.0);
    for (int i = 0; i < 9; i++) {
        vec2 o = vec2(mod(float(i), 3.0) - 1.0, floor(float(i) / 3.0) - 1.0);
        bw += texture2D(ffBloom2, st + o * 16.0 / R * 0.9).rgb;
    }
    bw /= 9.0;
    col += (bl * 0.55 + bw * 0.45) * bloom * 1.2;
    col *= 1.0 + audioReact * (0.26 * bassP() + 0.14 * levP());
    col = hueRot(col, hueShift);
    col = aces(col * 1.15);
    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    float l = lum(col);
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);
    float vig = 1.0 - 0.12 * dot(uvc, uvc);
    col *= vig;
    col += (hash12(fc + fract(TIME * 7.13) * 91.0) - 0.5) * 0.035 * grain;
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(lum(trl + bl) * 3.0, 0.0, 1.0) : 1.0);
}
