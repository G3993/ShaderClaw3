/*{
  "DESCRIPTION": "Laser Sculptor — a raw block of marble sits on a plinth in a dark, hazy room and a ring of up to eight audio-reactive lasers carves a statue out of it over the course of the night. The lasers rough the block out in chipped, faceted passes (top-down, bottom-up, outside-in or in random patches), then go back in for the fine detail until a polished figure stands where the block was. Every hit point burns white-hot, sprays sparks and lights the stone in the laser colour; the band being cut glows molten; the stone itself breaks down into streams of millions of tiny marble grains that pour off every cut, spread as they fall and settle as white powder on the plinth and floor, while the cutting edge crumbles to grit; bigger chunks break off too, glowing hot as they tumble down, bounce off the plinth and pile up as rubble on the floor; a cloud of marble dust hangs in the beams. Nine statues (Listener bust, Torso, Wave, Hand, Skull, Ammonite Shell, Winged Victory, Flamingo, AVA Octagon Totem) and ten marbles (Carrara, Black Marquina, Rosa Pink, Onyx Glow, Calacatta Gold, Verde Alpi, Azul, Rosso Levanto, Holo Opal, Liquid Chrome), with Auto Cycle for both. Lasers: count, five scan patterns (Orbit, Raster, Random Jumps, Lock-On, Spiral), jitter, strobe, decorative club fan beams, emitter ring height/radius/spin, eight palettes. Eight cameras: Slow Orbit, Front, Low Hero, Swoop, Handheld, Beat Cuts (new angle on the hits), Laser Follow and Crane, with shake and drop punch. A master Chaos control turns everything up at once and Drama sets how hard the music hits. The music drives the work: at rest one or two beams scan lazily and the carving creeps; a build wakes more lasers; on the drop every beam flashes, rocks rain down and the camera punches in. Carve Time sets how long one statue takes; Manual mode puts the carving on a slider (or a MIDI fader); Restart starts a fresh block.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-11-02 — original. Made for the AVA Live Miami proposal.",
  "CATEGORIES": ["Generator", "3D", "Sculpture", "Lasers", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "statueSel",   "LABEL": "Statue", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], "LABELS": ["Auto Cycle", "Listener Bust", "Torso", "Wave", "Hand", "Skull", "Ammonite Shell", "Winged Victory", "Flamingo", "AVA Octagon Totem"], "GROUP": "Sculpture" },
    { "NAME": "statueScale", "LABEL": "Statue Size", "TYPE": "float", "AUDIO": false, "MIN": 0.75, "MAX": 1.15, "DEFAULT": 1.0, "GROUP": "Sculpture" },
    { "NAME": "marbleSel",   "LABEL": "Marble", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9], "LABELS": ["Carrara White", "Black Marquina", "Rosa Pink", "Onyx Glow", "Calacatta Gold", "Verde Alpi", "Azul", "Rosso Levanto", "Holo Opal", "Liquid Chrome"], "GROUP": "Sculpture" },
    { "NAME": "marbleCycle", "LABEL": "New Marble Each Block", "TYPE": "bool", "DEFAULT": false, "GROUP": "Sculpture" },
    { "NAME": "veinAmt",     "LABEL": "Veins", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Sculpture" },

    { "NAME": "carveMode",   "LABEL": "Carving", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["Auto (time + music)", "Manual (slider)"], "GROUP": "Carving" },
    { "NAME": "carveTime",   "LABEL": "Carve Time (min)", "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 30.0, "DEFAULT": 4.0, "GROUP": "Carving" },
    { "NAME": "manualProg",  "LABEL": "Manual Progress", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Carving" },
    { "NAME": "carveDir",    "LABEL": "Carve Order", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Top Down", "Bottom Up", "Outside In", "Random Patches"], "GROUP": "Carving" },
    { "NAME": "musicPace",   "LABEL": "Music Speeds Carving", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Carving" },
    { "NAME": "facetSize",   "LABEL": "Chip Size", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Carving" },
    { "NAME": "roughness",   "LABEL": "Rough Cut Chaos", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Carving" },
    { "NAME": "holdTime",    "LABEL": "Hold Finished (s)", "TYPE": "float", "AUDIO": false, "MIN": 2.0, "MAX": 120.0, "DEFAULT": 20.0, "GROUP": "Carving" },
    { "NAME": "loopCarve",   "LABEL": "Loop (new block)", "TYPE": "bool", "DEFAULT": true, "GROUP": "Carving" },
    { "NAME": "restart",     "LABEL": "Restart (hold on)", "TYPE": "bool", "DEFAULT": false, "GROUP": "Carving" },

    { "NAME": "chaos",       "LABEL": "CHAOS", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Chaos" },
    { "NAME": "drama",       "LABEL": "Drama (music hits)", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Chaos" },
    { "NAME": "partAmt",     "LABEL": "Marble Particles", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "partSize",    "LABEL": "Particle Size", "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "partFall",    "LABEL": "Particle Fall Speed", "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "partSpread",  "LABEL": "Stream Spread", "TYPE": "float", "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "crumble",     "LABEL": "Crumbling Edge", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Particles" },
    { "NAME": "powder",      "LABEL": "Powder Build-Up", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Particles" },
    { "NAME": "rockAmt",     "LABEL": "Falling Rocks", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Chaos" },
    { "NAME": "rockSize",    "LABEL": "Rock Size", "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Chaos" },
    { "NAME": "rubbleAmt",   "LABEL": "Rubble Pile", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Chaos" },
    { "NAME": "dustAmt",     "LABEL": "Marble Dust", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.5, "GROUP": "Chaos" },
    { "NAME": "sparkAmt",    "LABEL": "Sparks", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Chaos" },
    { "NAME": "heatGlow",    "LABEL": "Molten Glow", "TYPE": "float", "MIN": 0.0, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Chaos" },

    { "NAME": "laserCount",  "LABEL": "Max Lasers", "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 8.0, "DEFAULT": 5.0, "GROUP": "Lasers" },
    { "NAME": "alwaysOn",    "LABEL": "All Lasers Always On", "TYPE": "bool", "DEFAULT": false, "GROUP": "Lasers" },
    { "NAME": "scanPattern", "LABEL": "Scan Pattern", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Orbit", "Raster", "Random Jumps", "Lock-On", "Spiral", "Mix (changes)"], "GROUP": "Lasers" },
    { "NAME": "laserPal",    "LABEL": "Laser Colours", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7], "LABELS": ["Miami Pink & Cyan", "Ruby", "Emerald", "White Hot", "Rainbow", "Sunset", "Ice", "Custom"], "GROUP": "Lasers" },
    { "NAME": "laserColA",   "LABEL": "Custom Colour A", "TYPE": "color", "DEFAULT": [1.0, 0.1, 0.55, 1.0], "GROUP": "Lasers" },
    { "NAME": "laserColB",   "LABEL": "Custom Colour B", "TYPE": "color", "DEFAULT": [0.1, 0.85, 1.0, 1.0], "GROUP": "Lasers" },
    { "NAME": "laserBright", "LABEL": "Beam Brightness", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Lasers" },
    { "NAME": "beamWidth",   "LABEL": "Beam Width", "TYPE": "float", "MIN": 0.3, "MAX": 5.0, "DEFAULT": 1.0, "GROUP": "Lasers" },
    { "NAME": "burnSize",    "LABEL": "Burn Spot Size", "TYPE": "float", "MIN": 0.3, "MAX": 4.0, "DEFAULT": 1.0, "GROUP": "Lasers" },
    { "NAME": "scanSpeed",   "LABEL": "Scan Speed", "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 4.0, "DEFAULT": 1.0, "GROUP": "Lasers" },
    { "NAME": "jitter",      "LABEL": "Beam Jitter", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Lasers" },
    { "NAME": "strobe",      "LABEL": "Strobe", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Lasers" },
    { "NAME": "fanBeams",    "LABEL": "Club Fan Beams", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Lasers" },
    { "NAME": "emitHeight",  "LABEL": "Emitter Height", "TYPE": "float", "AUDIO": false, "MIN": 1.2, "MAX": 4.0, "DEFAULT": 2.55, "GROUP": "Lasers" },
    { "NAME": "emitRadius",  "LABEL": "Emitter Ring Size", "TYPE": "float", "AUDIO": false, "MIN": 0.9, "MAX": 3.5, "DEFAULT": 1.85, "GROUP": "Lasers" },
    { "NAME": "emitSpin",    "LABEL": "Emitter Spin", "TYPE": "float", "AUDIO": false, "MIN": -2.0, "MAX": 2.0, "DEFAULT": 0.15, "GROUP": "Lasers" },
    { "NAME": "showEmitters","LABEL": "Show Emitter Heads", "TYPE": "bool", "DEFAULT": true, "GROUP": "Lasers" },

    { "NAME": "camMode",     "LABEL": "Camera", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7], "LABELS": ["Slow Orbit", "Front", "Low Hero", "Swoop", "Handheld", "Beat Cuts", "Laser Follow", "Crane"], "GROUP": "Camera" },
    { "NAME": "orbitSpeed",  "LABEL": "Camera Speed", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.5, "GROUP": "Camera" },
    { "NAME": "zoom",        "LABEL": "Zoom", "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.2, "DEFAULT": 1.0, "GROUP": "Camera" },
    { "NAME": "camHeight",   "LABEL": "Camera Height", "TYPE": "float", "AUDIO": false, "MIN": -1.0, "MAX": 1.5, "DEFAULT": 0.0, "GROUP": "Camera" },
    { "NAME": "fov",         "LABEL": "Lens (wide ↔ tele)", "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.8, "DEFAULT": 1.0, "GROUP": "Camera" },
    { "NAME": "camShake",    "LABEL": "Shake", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Camera" },
    { "NAME": "dropPunch",   "LABEL": "Drop Punch-In", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Camera" },
    { "NAME": "cutEvery",    "LABEL": "Beat Cuts: max seconds", "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 20.0, "DEFAULT": 6.0, "GROUP": "Camera" },
    { "NAME": "cutMin",      "LABEL": "Beat Cuts: min seconds", "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 8.0, "DEFAULT": 1.5, "GROUP": "Camera" },

    { "NAME": "roomSel",     "LABEL": "Room", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Gallery Black", "Miami Dusk", "White Studio"], "GROUP": "Room" },
    { "NAME": "hazeAmt",     "LABEL": "Haze", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Room" },
    { "NAME": "grain",       "LABEL": "Grain", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Room" },
    { "NAME": "transparentBg","LABEL": "Transparent BG", "TYPE": "bool", "DEFAULT": false, "GROUP": "Room" },

    { "NAME": "audioReact",  "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" }
  ],
  "PASSES": [
    { "TARGET": "stateBuf", "PERSISTENT": true, "FLOAT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Pass 0 = state row (row 0 only):
//   px0      r fine / g coarse progress ((g+r)/1000), b block index, a seconds held
//   px1-8    laser hit xyz, w = laser on
//   px9-16   hit normal xyz, w = 1 if the beam hit stone
//   px17     r = seconds since this block appeared
//   px18     camera cuts: r cut index, g seconds since cut, b previous onset
//   px20-43  falling rocks: origin xyz, w = age (s); dead when age > life
// Pass 1 = raymarched room, plinth, carved block, rocks, dust, beams, sparks.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718
#define NSTEP 1000.0
#define NROCK 24
#define NRUB 18
#define NSTAT 9.0
#define NMARB 10.0

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float h13(vec3 p)  { p = fract(p * 0.1031); p += dot(p, p.zyx + 31.32); return fract((p.x + p.y) * p.z); }
vec3 h33(float s) { return vec3(h11(s * 1.31 + 0.7), h11(s * 2.17 + 3.1), h11(s * 3.71 + 5.3)); }
float n3d(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(h13(i), h13(i + vec3(1,0,0)), f.x), mix(h13(i + vec3(0,1,0)), h13(i + vec3(1,1,0)), f.x), f.y),
               mix(mix(h13(i + vec3(0,0,1)), h13(i + vec3(1,0,1)), f.x), mix(h13(i + vec3(0,1,1)), h13(i + vec3(1,1,1)), f.x), f.y), f.z);
}
float fbm3(vec3 p) { return 0.55 * n3d(p) + 0.3 * n3d(p * 2.07 + 3.1) + 0.15 * n3d(p * 4.3 + 7.7); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
vec3 hsv(float h, float s, float v) { vec3 k = clamp(abs(fract(h + vec3(0.0, 2.0 / 3.0, 1.0 / 3.0)) * 6.0 - 3.0) - 1.0, 0.0, 1.0); return v * mix(vec3(1.0), k, s); }

// ── SDF kit ──────────────────────────────────────────────────────────────
float sdBox(vec3 p, vec3 b) { vec3 q = abs(p) - b; return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0); }
float sdRBox(vec3 p, vec3 b, float r) { return sdBox(p, b - r) - r; }
float sdEll(vec3 p, vec3 r) { float k0 = length(p / r); float k1 = length(p / (r * r)); return k0 * (k0 - 1.0) / max(k1, 1e-5); }
float sdCap(vec3 p, vec3 a, vec3 b, float r) { vec3 pa = p - a, ba = b - a; float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0); return length(pa - ba * h) - r; }
float sdCyl(vec3 p, float r, float h) { vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(r, h); return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)); }
float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }
float smax(float a, float b, float k) { return -smin(-a, -b, k); }
float sdOct(vec2 p, float r) {
    const vec3 k = vec3(-0.9238795325, 0.3826834323, 0.4142135623);
    p = abs(p);
    p -= 2.0 * min(dot(vec2(k.x, k.y), p), 0.0) * vec2(k.x, k.y);
    p -= 2.0 * min(dot(vec2(-k.x, k.y), p), 0.0) * vec2(-k.x, k.y);
    p -= vec2(clamp(p.x, -k.z * r, k.z * r), r);
    return length(p) * sign(p.y);
}
float extrude(float d2, float dz, float h) { vec2 w = vec2(d2, abs(dz) - h); return min(max(w.x, w.y), 0.0) + length(max(w, 0.0)); }

// ── Scene layout ─────────────────────────────────────────────────────────
const float PLINTH_TOP = 0.38;
const vec3  BLOCK_C = vec3(0.0, 1.0, 0.0);
const vec3  BLOCK_H = vec3(0.40, 0.62, 0.34);
const float STATUE_H = 1.24;
float gChaos;

// ── Statues (local: y = 0 at plinth top) ────────────────────────────────
float socle(vec3 q) {
    float d = sdCyl(q - vec3(0.0, 0.05, 0.0), 0.2, 0.05) - 0.01;
    d = smin(d, sdCyl(q - vec3(0.0, 0.14, 0.0), 0.11, 0.06), 0.05);
    return d;
}

float statueListener(vec3 q) {
    float d = socle(q);
    // chest + shoulders (classical bust cut)
    float chest = sdEll(q - vec3(0.0, 0.42, 0.0), vec3(0.34, 0.25, 0.19));
    chest = smin(chest, sdEll(vec3(abs(q.x), q.yz) - vec3(0.22, 0.55, -0.01), vec3(0.15, 0.1, 0.13)), 0.08);
    chest = smax(chest, 0.2 - q.y + 0.06 * (q.x * q.x * 6.0), 0.05);       // curved bust cut
    float pec = sdEll(vec3(abs(q.x), q.yz) - vec3(0.1, 0.47, 0.13), vec3(0.11, 0.07, 0.06));
    chest = smin(chest, pec, 0.05);
    d = smin(d, chest, 0.07);
    // drapery folds across the chest
    float fold = sin(q.x * 38.0 + q.y * 14.0) * 0.006 * smoothstep(0.5, 0.25, q.y) * step(0.18, q.y);
    d += fold;
    // head tilts down and to the side, listening
    vec3 hp = q - vec3(0.0, 0.62, 0.0);
    hp.yz = rot(-0.22) * hp.yz;
    hp.xy = rot(0.12) * hp.xy;
    hp += vec3(0.0, 0.62, 0.0);
    float neck = sdCap(hp, vec3(0.0, 0.5, -0.01), vec3(0.0, 0.8, 0.02), 0.075);
    d = smin(d, neck, 0.06);
    float head = sdEll(hp - vec3(0.0, 0.93, 0.02), vec3(0.125, 0.165, 0.15));
    head = smin(head, sdEll(hp - vec3(0.0, 0.98, -0.03), vec3(0.14, 0.14, 0.15)), 0.05);
    head = smin(head, sdEll(hp - vec3(0.0, 0.845, 0.07), vec3(0.085, 0.07, 0.085)), 0.05);    // jaw
    vec3 hx = vec3(abs(hp.x), hp.yz);
    head = smax(head, -sdEll(hx - vec3(0.052, 0.955, 0.155), vec3(0.04, 0.026, 0.03)), 0.02);  // eye sockets
    head = smin(head, sdEll(hx - vec3(0.052, 0.948, 0.142), vec3(0.033, 0.014, 0.022)), 0.012); // closed lids
    head = smin(head, sdCap(hx, vec3(0.02, 0.985, 0.158), vec3(0.085, 0.982, 0.13), 0.012), 0.02); // brow
    vec3 np = hp - vec3(0.0, 0.925, 0.165); np.yz = rot(-0.35) * np.yz;
    head = smin(head, sdEll(np, vec3(0.02, 0.048, 0.03)), 0.02);                               // nose
    head = smin(head, sdEll(hp - vec3(0.0, 0.868, 0.158), vec3(0.03, 0.009, 0.014)), 0.008);    // lips
    head = smin(head, sdEll(hp - vec3(0.0, 0.852, 0.152), vec3(0.026, 0.01, 0.014)), 0.008);
    head = smin(head, sdEll(hx - vec3(0.13, 0.93, -0.01), vec3(0.018, 0.048, 0.032)), 0.015);  // ears
    // hair: waved cap + bun
    float hairMask = smoothstep(0.93, 1.0, hp.y + max(-hp.z, 0.0) * 0.8 - max(hp.z - 0.05, 0.0) * 0.6);
    float waves = sin(atan(hp.x, hp.z + 0.02) * 22.0 + hp.y * 12.0) * 0.006;
    head -= hairMask * (0.012 + waves);
    head = smin(head, sdEll(hp - vec3(0.0, 0.97, -0.17), vec3(0.075, 0.07, 0.065)), 0.04);
    d = smin(d, head, 0.03);
    return d;
}

float statueTorso(vec3 q) {
    float d = socle(q);
    vec3 p = q;
    float tw = (p.y - 0.4) * 0.35;                    // contrapposto twist
    p.xz = rot(tw) * p.xz;
    p.xy = rot(0.06 * smoothstep(0.3, 1.0, p.y)) * p.xy;
    vec3 px = vec3(abs(p.x), p.yz);
    float body = sdEll(p - vec3(0.0, 0.36, 0.0), vec3(0.23, 0.16, 0.16));              // pelvis
    body = smin(body, sdCap(px, vec3(0.11, 0.32, 0.01), vec3(0.14, 0.16, 0.04), 0.095), 0.06); // thigh stumps
    body = smin(body, sdEll(p - vec3(0.0, 0.56, 0.0), vec3(0.17, 0.14, 0.13)), 0.08);  // waist
    body = smin(body, sdEll(p - vec3(0.0, 0.78, -0.005), vec3(0.23, 0.2, 0.15)), 0.08); // ribcage
    body = smin(body, sdEll(px - vec3(0.095, 0.82, 0.095), vec3(0.1, 0.075, 0.06)), 0.04); // pecs
    body = smin(body, sdEll(px - vec3(0.25, 0.92, 0.0), vec3(0.11, 0.095, 0.1)), 0.06);  // shoulders
    float arm = sdCap(px, vec3(0.28, 0.9, 0.0), vec3(0.35, 0.72, 0.03), 0.07);         // arm stumps, cut clean
    arm = max(arm, dot(px - vec3(0.33, 0.78, 0.03), normalize(vec3(0.45, -0.85, 0.1))));
    body = smin(body, arm, 0.04);
    body = smin(body, sdCap(p, vec3(0.0, 0.95, -0.01), vec3(0.0, 1.07, 0.01), 0.068), 0.05); // neck
    body = smax(body, p.y - 1.08, 0.01);
    // abdominal + rib relief
    float linea = exp(-p.x * p.x * 900.0) * smoothstep(0.42, 0.55, p.y) * smoothstep(0.72, 0.6, p.y) * step(0.0, p.z);
    float ribs = sin(p.y * 40.0 - abs(p.x) * 6.0) * 0.004 * smoothstep(0.62, 0.7, p.y) * smoothstep(0.86, 0.76, p.y) * step(0.05, abs(p.x));
    body += 0.006 * linea + ribs;
    float navel = sdEll(p - vec3(0.0, 0.5, 0.125), vec3(0.012, 0.016, 0.02));
    body = smax(body, -navel, 0.01);
    d = smin(d, body, 0.06);
    return d;
}

float statueWave(vec3 q) {
    float d = socle(q);
    vec3 p = q - vec3(0.0, 0.66, 0.0);
    float tw = p.y * 1.6;
    p.xz = rot(tw) * p.xz;
    float form = sdEll(p, vec3(0.27, 0.5, 0.17));
    // crest curling over the top
    vec3 c = p - vec3(0.06, 0.32, 0.0);
    c.xy = rot(0.6) * c.xy;
    form = smin(form, sdEll(c, vec3(0.2, 0.09, 0.12)), 0.08);
    // the hole the light passes through
    vec3 hpp = p - vec3(0.02, 0.06, 0.0);
    float hole = length(hpp.xy * vec2(1.0, 0.8)) - 0.1;
    form = smax(form, -hole, 0.06);
    form += sin(p.y * 18.0 + atan(p.x, p.z) * 2.0) * 0.008;
    form = smax(form, -q.y + 0.17, 0.02);
    d = smin(d, form, 0.07);
    return d;
}


float statueHand(vec3 q) {
    float d = socle(q);
    vec3 p = q;
    p.xy = rot(0.08) * p.xy;
    float arm = sdCap(p, vec3(0.0, 0.12, 0.0), vec3(0.0, 0.56, 0.0), 0.085);
    arm = smin(arm, sdEll(p - vec3(0.0, 0.58, 0.0), vec3(0.1, 0.06, 0.06)), 0.04);   // wrist
    float palm = sdRBox(p - vec3(0.0, 0.72, 0.0), vec3(0.125, 0.14, 0.04), 0.035);
    palm = smin(palm, sdEll(p - vec3(-0.07, 0.66, 0.02), vec3(0.06, 0.08, 0.04)), 0.03);
    float h = smin(arm, palm, 0.06);
    // four fingers, slightly spread, knuckles bent forward
    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        float x = -0.09 + fi * 0.06;
        float len = (fi == 1.0 ? 0.3 : (fi == 2.0 ? 0.28 : (fi == 0.0 ? 0.24 : 0.21)));
        float spread = (fi - 1.5) * 0.06;
        vec3 a = vec3(x, 0.84, 0.0);
        vec3 b = a + vec3(spread * len, len * 0.55, 0.012);
        vec3 c = b + vec3(spread * len * 0.8, len * 0.45, 0.04 + 0.02 * fi);
        float f = min(sdCap(p, a, b, 0.03), sdCap(p, b, c, 0.026));
        f = smin(f, length(p - b) - 0.031, 0.01);
        h = smin(h, f, 0.02);
    }
    h = smin(h, sdCap(p, vec3(0.11, 0.66, 0.02), vec3(0.2, 0.78, 0.07), 0.036), 0.03);    // thumb
    h = smin(h, sdCap(p, vec3(0.2, 0.78, 0.07), vec3(0.25, 0.89, 0.1), 0.031), 0.015);
    // lines of the palm
    h += 0.004 * exp(-pow((p.y - 0.7 - p.x * 0.4) * 60.0, 2.0)) * step(0.0, p.z);
    return smin(d, h, 0.05);
}

float statueSkull(vec3 q) {
    float d = socle(q);
    d = smin(d, sdCap(q, vec3(0.0, 0.12, -0.02), vec3(0.0, 0.38, -0.02), 0.08), 0.06);
    vec3 p = q - vec3(0.0, 0.0, -0.02);
    vec3 px = vec3(abs(p.x), p.yz);
    float cran = sdEll(p - vec3(0.0, 0.78, -0.03), vec3(0.28, 0.29, 0.31));
    float face = sdEll(p - vec3(0.0, 0.6, 0.11), vec3(0.21, 0.19, 0.18));
    float s = smin(cran, face, 0.08);
    s = smin(s, sdEll(px - vec3(0.15, 0.57, 0.15), vec3(0.08, 0.05, 0.07)), 0.04);     // cheekbones
    s = smax(s, -sdEll(px - vec3(0.27, 0.66, 0.02), vec3(0.06, 0.08, 0.12)), 0.04);    // temples
    s = smax(s, -sdEll(px - vec3(0.095, 0.66, 0.25), vec3(0.075, 0.07, 0.09)), 0.025); // eye sockets
    vec3 np = p - vec3(0.0, 0.55, 0.28);
    s = smax(s, -sdEll(np, vec3(0.032 + (0.55 - p.y) * 0.25, 0.05, 0.08)), 0.015);       // nose cavity
    // jaw + teeth
    float jaw = sdEll(p - vec3(0.0, 0.43, 0.1), vec3(0.17, 0.09, 0.16));
    jaw = smax(jaw, -(p.y - 0.37), 0.02);
    float teethZone = smoothstep(0.035, 0.0, abs(p.y - 0.465)) * step(0.08, p.z);
    s = smin(s, jaw, 0.04);
    s += teethZone * (0.008 * abs(sin(p.x * 70.0)) + 0.004);
    s = smax(s, -sdEll(p - vec3(0.0, 0.465, 0.24), vec3(0.14, 0.004, 0.06)), 0.004);
    s += 0.003 * sin(p.x * 50.0 + p.y * 40.0) * smoothstep(0.85, 1.0, p.y);              // sutures
    return smin(d, s, 0.05);
}

float statueShell(vec3 q) {
    float d = socle(q);
    vec3 p = q - vec3(0.0, 0.64, 0.0);
    p.xy = rot(0.3) * p.xy;
    float r = length(p.xy);
    float th = atan(p.y, p.x);
    const float A0 = 0.0135, B = 0.1255, TMAX = 24.3;
    float s = log(max(r, 1e-4) / A0) / B;
    float k0 = floor((s - th) / TAU);
    float best = 1e9;
    for (int j = 0; j < 3; j++) {
        float tt = th + TAU * (k0 + float(j) - 0.0);
        tt = clamp(tt, 0.0, TMAX);
        float rc = A0 * exp(B * tt);
        vec2 c = rc * vec2(cos(tt), sin(tt));
        float tube = rc * 0.43;
        float ribs = 0.006 * smoothstep(0.03, 0.2, rc) * sin(tt * 9.0);
        float dd = length(vec3(p.xy - c, p.z * 1.15)) - tube - ribs;
        best = min(best, dd);
    }
    best = smax(best, -sdEll(p - vec3(0.0, 0.0, 0.0), vec3(0.03, 0.03, 0.3)), 0.02);
    d = smin(d, best, 0.06);
    d = smin(d, sdCap(q, vec3(0.0, 0.12, 0.0), vec3(0.0, 0.38, 0.0), 0.06), 0.06);
    return d;
}

float statueWinged(vec3 q) {
    float d = statueTorso(q * vec3(1.05, 1.0, 1.05)) / 1.05;
    vec3 px = vec3(abs(q.x), q.yz);
    vec3 w = px - vec3(0.07, 0.8, -0.2);
    w.xy = rot(-0.55) * w.xy;
    w.yz = rot(0.25) * w.yz;
    float wing = sdEll(w - vec3(0.0, 0.2, 0.0), vec3(0.1, 0.25, 0.03));
    wing = smin(wing, sdEll(w - vec3(0.06, 0.3, -0.01), vec3(0.07, 0.12, 0.025)), 0.04);
    wing = smin(wing, sdCap(px, vec3(0.06, 0.82, -0.12), vec3(0.08, 0.84, -0.19), 0.03), 0.03);
    float fr = fract(w.y * 11.0 - w.x * 3.0);
    float feathers = -0.004 * smoothstep(0.0, 0.15, fr) * smoothstep(1.0, 0.6, fr) * smoothstep(0.02, 0.12, w.y);
    wing += feathers;
    return smin(d, wing, 0.05);
}

float statueFlamingo(vec3 q) {
    float d = socle(q);
    vec3 p = q;
    p.xz = rot(0.35) * p.xz;
    vec3 bp = p - vec3(-0.03, 0.62, 0.0);
    bp.xy = rot(-0.18) * bp.xy;
    float body = sdEll(bp, vec3(0.26, 0.13, 0.13));
    body = smin(body, sdEll(p - vec3(-0.25, 0.7, 0.0), vec3(0.11, 0.05, 0.09)), 0.05);      // tail
    body += sin(bp.x * 40.0 + bp.y * 10.0) * 0.004 * step(0.0, bp.y);                      // wing feathers
    float leg = sdCap(p, vec3(0.0, 0.5, 0.0), vec3(0.0, 0.3, 0.0), 0.024);
    leg = smin(leg, sdCap(p, vec3(0.0, 0.3, 0.0), vec3(0.0, 0.14, 0.0), 0.022), 0.01);
    leg = smin(leg, length(p - vec3(0.0, 0.3, 0.0)) - 0.03, 0.01);
    float leg2 = sdCap(p, vec3(-0.02, 0.52, 0.04), vec3(0.08, 0.38, 0.05), 0.02);
    leg2 = min(leg2, sdCap(p, vec3(0.08, 0.38, 0.05), vec3(-0.06, 0.33, 0.06), 0.018));
    float neck = sdCap(p, vec3(0.18, 0.66, 0.0), vec3(0.27, 0.8, 0.0), 0.042);
    neck = smin(neck, sdCap(p, vec3(0.27, 0.8, 0.0), vec3(0.19, 0.93, 0.0), 0.036), 0.03);
    neck = smin(neck, sdCap(p, vec3(0.19, 0.93, 0.0), vec3(0.2, 1.06, 0.0), 0.034), 0.03);
    float head = sdEll(p - vec3(0.25, 1.1, 0.0), vec3(0.065, 0.05, 0.05));
    float beak = sdCap(p, vec3(0.3, 1.1, 0.0), vec3(0.36, 1.07, 0.0), 0.024);
    beak = smin(beak, sdCap(p, vec3(0.36, 1.07, 0.0), vec3(0.37, 1.0, 0.0), 0.014), 0.015);
    float f = smin(body, neck, 0.05);
    f = smin(f, head, 0.03);
    f = smin(f, beak, 0.015);
    f = smax(f, -sdEll(vec3(p.x, p.y, abs(p.z)) - vec3(0.26, 1.115, 0.045), vec3(0.01, 0.01, 0.01)), 0.005); // eyes
    f = smin(f, leg, 0.03);
    f = smin(f, leg2, 0.02);
    return smin(d, f, 0.04);
}

float statueTotem(vec3 q) {
    float d = extrude(sdOct(q.xz, 0.28), q.y - 0.06, 0.06) - 0.008;
    vec3 p = q;
    vec2 r2 = rot(0.3927) * p.xz;
    d = min(d, extrude(sdOct(r2, 0.22), p.y - 0.18, 0.045) - 0.006);
    d = smin(d, extrude(sdOct(p.xz, 0.09), p.y - 0.4, 0.2) - 0.005, 0.03);
    d = min(d, extrude(sdOct(r2, 0.2), p.y - 0.62, 0.035) - 0.006);
    // the octagon frame
    vec2 c = p.xy - vec2(0.0, 0.93);
    float ring = max(sdOct(c, 0.28), -sdOct(c, 0.19));
    float fr = extrude(ring, p.z, 0.055) - 0.008;
    d = smin(d, fr, 0.02);
    // inner octagon facets + sound grooves
    float grooves = sin(length(c) * 120.0) * 0.003 * step(abs(p.z), 0.06);
    d += grooves * step(0.66, p.y);
    float orb = length(p - vec3(0.0, 0.93, 0.0)) - 0.075;
    d = min(d, orb);
    d = smin(d, sdCap(p, vec3(0.0, 0.64, 0.0), vec3(0.0, 0.86, 0.0), 0.02), 0.02);
    return d;
}

float gStatueF;
float statueSDF(vec3 q) {
    float sc = statueScale;
    q.y /= sc; q.xz /= sc;
    float s = gStatueF;
    float d;
    if (s < 0.5) d = statueListener(q);
    else if (s < 1.5) d = statueTorso(q);
    else if (s < 2.5) d = statueWave(q);
    else if (s < 3.5) d = statueHand(q);
    else if (s < 4.5) d = statueSkull(q);
    else if (s < 5.5) d = statueShell(q);
    else if (s < 6.5) d = statueWinged(q);
    else if (s < 7.5) d = statueFlamingo(q);
    else d = statueTotem(q);
    return d * sc;
}

// ── Carving: stock left on top of the statue ─────────────────────────────
float gProg;
float carveOrder(vec3 p, float yN) {
    float d = float(carveDir);
    if (d < 0.5) return 1.0 - yN;
    if (d < 1.5) return yN;
    if (d < 2.5) return clamp(1.0 - length(p.xz * vec2(1.0, 1.15)) / 0.5, 0.0, 1.0);
    return n3d(p * 2.2 + 3.0);
}
float stockAt(vec3 p, float prog) {
    float yN = clamp((p.y - PLINTH_TOP) / STATUE_H, 0.0, 1.0);
    float c = carveOrder(p, yN);
    float rough = smoothstep(0.0, 1.0, (prog - 0.04 - c * 0.3) / 0.48);
    float m = mix(0.75, 0.06, rough);
    m = mix(m, 0.0, smoothstep(0.52, 0.96, prog));
    float freq = mix(12.0, 4.0, facetSize);
    float fac = abs(fract(dot(p, vec3(0.7, 0.45, 0.55)) * freq) - 0.5) + abs(fract(dot(p, vec3(-0.6, 0.3, 0.74)) * freq * 0.86) - 0.5);
    float rc = roughness * (0.6 + 0.8 * gChaos);
    m *= 0.78 + 0.32 * fac * (0.4 + rc) + 0.45 * rc * (n3d(p * 6.0) - 0.5);
    // crumbly, gritty edge (amplitude capped so the field stays marchable)
    if (m > 1e-3 && crumble > 0.0) m += min(m, 0.05) * crumble * 0.22 * (n3d(p * 85.0) - 0.5);
    return m;
}

float gRawFace;
float materialSDF(vec3 p) {
    vec3 bp = p - BLOCK_C;
    float box = sdRBox(bp, BLOCK_H, 0.012);
    if (box > 0.12) { gRawFace = 1.0; return box; }
    float s = statueSDF(p - vec3(0.0, PLINTH_TOP, 0.0));
    float cut = s - stockAt(p, gProg);
    gRawFace = step(cut, box);
    return max(box, cut);
}
float plinthSDF(vec3 p) {
    float d = sdRBox(p - vec3(0.0, PLINTH_TOP * 0.5, 0.0), vec3(0.56, PLINTH_TOP * 0.5, 0.48), 0.01);
    d = min(d, sdRBox(p - vec3(0.0, PLINTH_TOP - 0.012, 0.0), vec3(0.6, 0.014, 0.52), 0.006));
    return d;
}
vec2 mapScene(vec3 p) {
    float st = materialSDF(p);
    float pl = plinthSDF(p);
    vec2 r = vec2(p.y, 0.0);
    if (pl < r.x) r = vec2(pl, 1.0);
    if (st < r.x) r = vec2(st, 2.0);
    return r;
}
float mapD(vec3 p) { return min(min(materialSDF(p), plinthSDF(p)), p.y); }
vec3 calcNormal(vec3 p) {
    const vec2 k = vec2(1.0, -1.0);
    float e = 0.0012;
    return normalize(k.xyy * mapD(p + k.xyy * e) + k.yyx * mapD(p + k.yyx * e) +
                     k.yxy * mapD(p + k.yxy * e) + k.xxx * mapD(p + k.xxx * e));
}

// ── Show dynamics ────────────────────────────────────────────────────────
float gRest, gBuild, gPeak, gDrop, gLift, gE;
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
    float k = dynamics * audioReact;
    gE = e * audioReact;
    gBuild = b * k;
    gDrop  = d * k * drama;
    gPeak  = smoothstep(0.5, 0.9, e) * k;
    gRest  = (1.0 - smoothstep(0.08, 0.4, e)) * (1.0 - b) * k;
    gLift  = mix(0.35, clamp(0.12 + e * 0.75 + gBuild * 0.25 + d * 0.3, 0.0, 1.0), k);
    gChaos = clamp(chaos + 0.35 * gDrop + 0.15 * gBuild, 0.0, 1.5);
}

// ── Lasers ───────────────────────────────────────────────────────────────
float nLasers() { return floor(clamp(laserCount, 1.0, 8.0) + 0.01); }
vec3 emitterPos(float i) {
    float n = nLasers();
    float a = i * TAU / n + 0.6 + TIME * 0.1 * emitSpin;
    return vec3(emitRadius * cos(a), emitHeight + 0.3 * sin(i * 2.1), emitRadius * sin(a));
}
vec3 laserColor(float i) {
    float pal = float(laserPal);
    bool even = mod(i, 2.0) < 0.5;
    float k3 = mod(i, 3.0);
    if (pal < 0.5) return even ? vec3(1.0, 0.12, 0.55) : vec3(0.1, 0.85, 1.0);
    if (pal < 1.5) return even ? vec3(1.0, 0.06, 0.08) : vec3(1.0, 0.25, 0.12);
    if (pal < 2.5) return even ? vec3(0.15, 1.0, 0.35) : vec3(0.45, 1.0, 0.2);
    if (pal < 3.5) return vec3(1.0, 0.95, 0.88);
    if (pal < 4.5) return hsv(i / nLasers() + TIME * 0.04 + audioBassTime * 0.03 * audioReact, 0.85, 1.0);
    if (pal < 5.5) return k3 < 0.5 ? vec3(1.0, 0.45, 0.08) : (k3 < 1.5 ? vec3(1.0, 0.12, 0.5) : vec3(0.6, 0.2, 1.0));
    if (pal < 6.5) return even ? vec3(0.55, 0.85, 1.0) : vec3(0.2, 0.45, 1.0);
    return even ? laserColA.rgb : laserColB.rgb;
}
float laserOn(float i) {
    float n = nLasers();
    if (alwaysOn) return step(i, n - 0.5);
    float want = mix(1.4, n + 0.3, clamp(gLift * 1.1 + gDrop + gChaos * 0.3, 0.0, 1.0));
    want = min(want, n + 0.3);
    return smoothstep(i, i + 0.6, want);
}
float patternNow() {
    float pt = float(scanPattern);
    if (pt > 4.5) return floor(mod(TIME / 11.0 + floor(gDrop * 2.0), 5.0));
    return pt;
}
vec3 laserTarget(float i, float prog, float clk) {
    float n = nLasers();
    float cd = float(carveDir);
    float front = clamp((prog - 0.04) / 0.62, 0.0, 1.0);
    float band;
    if (cd < 0.5) band = 1.0 - front;
    else if (cd < 1.5) band = front;
    else band = 0.5 + 0.42 * sin(clk * 0.31 + i * 1.3);
    if (prog > 0.6) band = 0.12 + 0.85 * (0.5 + 0.5 * sin(clk * 0.37 + i * 1.7 + sin(clk * 0.21 + i)));
    float pt = patternNow();
    vec3 T;
    if (pt < 0.5) {                                         // orbit
        float yN = clamp(band + 0.12 * sin(clk * 0.9 + i * 2.0) + 0.04, 0.04, 0.98);
        float a = clk * (0.55 + 0.12 * i) + i * 1.9;
        float r = 0.1 + 0.08 * sin(clk * 1.3 + i * 3.0);
        T = vec3(r * cos(a), PLINTH_TOP + yN * STATUE_H, r * sin(a) * 0.8);
    } else if (pt < 1.5) {                                  // raster
        float side = i * TAU / n + 0.6;
        vec3 nrm = vec3(cos(side), 0.0, sin(side));
        vec3 tan = vec3(-nrm.z, 0.0, nrm.x);
        float lat = (abs(fract(clk * 0.6 + i * 0.37) * 2.0 - 1.0) * 2.0 - 1.0) * 0.32;
        float yN = clamp(band + (fract(clk * 0.15 + i * 0.21) - 0.5) * 0.22, 0.04, 0.98);
        T = nrm * 0.12 + tan * lat + vec3(0.0, PLINTH_TOP + yN * STATUE_H, 0.0);
    } else if (pt < 2.5) {                                  // random jumps
        float st = floor(clk * 1.6 * (1.0 + gChaos) + i * 0.37);
        vec3 r3 = h33(st * 7.3 + i * 31.0) - 0.5;
        float yN = clamp(band + r3.y * 0.35, 0.04, 0.98);
        T = vec3(r3.x * 0.6, PLINTH_TOP + yN * STATUE_H, r3.z * 0.5);
    } else if (pt < 3.5) {                                  // lock-on: everything converges
        float a = clk * 0.7;
        float yN = clamp(band + 0.1 * sin(clk * 0.5), 0.04, 0.98);
        T = vec3(0.12 * cos(a), PLINTH_TOP + yN * STATUE_H, 0.1 * sin(a)) + (h33(i * 3.3) - 0.5) * 0.03;
    } else {                                                // spiral
        float a = clk * 2.0 + i * TAU / n;
        float yN = 0.05 + 0.9 * fract(-clk * 0.07 + i / n);
        T = vec3(0.13 * cos(a), PLINTH_TOP + yN * STATUE_H, 0.11 * sin(a));
    }
    float jt = jitter * (0.4 + gChaos + clamp(audioHigh * audioReact, 0.0, 1.0));
    T += (vec3(n3d(vec3(TIME * 7.0, i, 0.0)), n3d(vec3(i, TIME * 7.0, 3.0)), n3d(vec3(5.0, i, TIME * 7.0))) - 0.5) * 0.12 * jt;
    return T;
}

// ── Falling rocks ─────────────────────────────────────────────────────────
float solveFall(float y0, float vy, float yT) {
    float a = -1.8, b = vy, c = y0 - yT;                 // 0.5 * g, g = -3.6
    float disc = b * b - 4.0 * a * c;
    if (disc < 0.0) return 0.0;
    return max((-b - sqrt(disc)) / (2.0 * a), 0.0);
}
float rockLife(float seed) { return 4.0 + 2.0 * h11(seed * 9.1); }
float rockRad(float seed) { return (0.016 + 0.03 * h11(seed * 5.7) * h11(seed * 2.3 + 1.0)) * rockSize * (1.0 + 0.4 * gChaos); }
vec3 rockPos(vec3 o, float age, float seed, out float spinT) {
    vec3 r3 = h33(seed);
    vec3 dir = normalize(vec3(o.x, 0.0, o.z) + (r3 - 0.5) * 0.7 + vec3(1e-3));
    float sh = (0.25 + 0.6 * r3.x) * (1.0 + gChaos * 0.8);
    float vy = 0.2 + 1.2 * r3.y * (0.6 + gChaos);
    vec3 v = vec3(dir.x * sh, vy, dir.z * sh);
    float rr = rockRad(seed);
    float tP = solveFall(o.y, vy, PLINTH_TOP + rr);
    vec2 xzP = o.xz + v.xz * tP;
    float t1, gY;
    if (abs(xzP.x) < 0.56 && abs(xzP.y) < 0.48 && o.y > PLINTH_TOP) { t1 = tP; gY = PLINTH_TOP; }
    else { t1 = solveFall(o.y, vy, rr); gY = 0.0; }
    vec3 p;
    if (age < t1) { p = o + v * age + vec3(0.0, -1.8 * age * age, 0.0); spinT = age; }
    else {
        vec3 p1 = o + v * t1 + vec3(0.0, -1.8 * t1 * t1, 0.0);
        float v1y = vy - 3.6 * t1;
        vec3 v2 = vec3(v.x * 0.4, -v1y * 0.3, v.z * 0.4);
        float t2 = 2.0 * v2.y / 3.6;
        float ta = min(age - t1, t2);
        p = p1 + v2 * ta + vec3(0.0, -1.8 * ta * ta, 0.0);
        p.y = max(p.y, gY + rr * 0.7);
        spinT = t1 + ta;
    }
    return p;
}
mat3 rockRot(float seed, float t) {
    vec3 r3 = h33(seed * 3.7);
    float a = r3.x * TAU + t * (2.0 + 6.0 * r3.y), b = r3.z * TAU + t * (1.5 + 4.0 * r3.x);
    float ca = cos(a), sa = sin(a), cb = cos(b), sb = sin(b);
    return mat3(ca, sa, 0.0, -sa, ca, 0.0, 0.0, 0.0, 1.0) * mat3(1.0, 0.0, 0.0, 0.0, cb, sb, 0.0, -sb, cb);
}
// faceted chip = box ∩ rotated box
vec2 boxHit(vec3 ro, vec3 rd, vec3 rad, out vec3 n) {
    vec3 m = 1.0 / (rd + sign(rd) * 1e-6 + vec3(1e-7));
    vec3 nn = m * ro;
    vec3 k = abs(m) * rad;
    vec3 t1 = -nn - k, t2 = -nn + k;
    float tN = max(max(t1.x, t1.y), t1.z), tF = min(min(t2.x, t2.y), t2.z);
    n = -sign(rd) * step(t1.yzx, t1.xyz) * step(t1.zxy, t1.xyz);
    return vec2(tN, tF);
}
float chipHit(vec3 ro, vec3 rd, vec3 c, float r, float seed, float spin, out vec3 nOut) {
    mat3 R = rockRot(seed, spin);
    vec3 lo = (ro - c) * R, ld = rd * R;                 // into rock space (R orthonormal)
    vec3 sz = r * (vec3(0.8) + 0.5 * h33(seed * 1.9));
    vec3 n1, n2;
    vec2 a = boxHit(lo, ld, sz, n1);
    mat2 q = rot(0.785 + h11(seed) * 0.5);
    vec3 lo2 = vec3(q * lo.xy, lo.z), ld2 = vec3(q * ld.xy, ld.z);
    lo2.yz = rot(0.6) * lo2.yz; ld2.yz = rot(0.6) * ld2.yz;
    vec2 b = boxHit(lo2, ld2, sz * vec3(0.95, 1.0, 0.9), n2);
    float tN = max(a.x, b.x), tF = min(a.y, b.y);
    if (tN > tF || tF < 0.0) return -1.0;
    vec3 nl;
    if (a.x > b.x) nl = n1;
    else {
        n2.yz = rot(-0.6) * n2.yz;
        nl = vec3(rot(-0.785 - h11(seed) * 0.5) * n2.xy, n2.z);
    }
    nOut = R * nl;
    return tN;
}

// ─────────────────────────────────────────────────────────────────────────
vec4 S(float i) { return texture2D(stateBuf, vec2(i + 0.5, 0.5) / RENDERSIZE); }
float blockIdx() { return S(0.0).b; }
float statueFor(float bi) { return float(statueSel) > 0.5 ? float(statueSel) - 1.0 : mod(bi, NSTAT); }
float marbleFor(float bi) { return marbleCycle ? mod(float(marbleSel) + bi, NMARB) : float(marbleSel); }

void statePass() {
    vec2 fcf = floor(gl_FragCoord.xy);
    if (fcf.y > 0.5 || fcf.x > 91.5) { gl_FragColor = vec4(0.0); return; }
    float x = fcf.x;
    vec4 s0 = S(0.0);
    bool init = FRAMEINDEX < 2 || restart;
    float dt = clamp(TIMEDELTA, 0.0, 0.1);
    float progPrev = init ? 0.0 : clamp((s0.g + s0.r) / NSTEP, 0.0, 1.0);
    float bi = init ? 0.0 : s0.b;
    float prog = float(carveMode) > 0.5 ? manualProg : progPrev;

    if (x < 0.5) {
        float fineV = init ? 0.0 : s0.r, coarse = init ? 0.0 : s0.g, held = init ? 0.0 : s0.a;
        if (progPrev < 1.0) {
            float pace = 0.45 + musicPace * (1.1 * gE + 0.6 * clamp(audioBeatPulse, 0.0, 1.0) * audioReact + 0.5 * gDrop);
            fineV += pace / (max(carveTime, 0.05) * 60.0) * dt * NSTEP;
            float carry = floor(fineV);
            coarse += carry; fineV -= carry;
            if (coarse >= NSTEP) { coarse = NSTEP; fineV = 0.0; }
            held = 0.0;
        } else {
            held += dt;
            if (loopCarve && held > holdTime + 2.0) { coarse = 0.0; fineV = 0.0; held = 0.0; bi = mod(bi + 1.0, 90.0); }
        }
        if (restart && FRAMEINDEX >= 2) bi = s0.b;
        gl_FragColor = vec4(fineV, coarse, bi, min(held, 4000.0));
        return;
    }
    if (x > 16.5 && x < 17.5) {
        float age = init ? 0.0 : S(17.0).r;
        age = (s0.g < 0.5 && s0.r < 0.002 && !init) ? 0.0 : age;
        gl_FragColor = vec4(min(age + dt, 600.0), 0.0, 0.0, 1.0);
        return;
    }
    if (x > 17.5 && x < 18.5) {                         // beat-cut camera
        vec4 c = init ? vec4(0.0) : S(18.0);
        float on = clamp(max(audioOnset, audioBassHit) * audioReact * 1.5, 0.0, 1.0);
        float rising = step(0.5, on) * step(c.b, 0.5);
        c.g += dt;
        if ((rising > 0.5 && c.g > cutMin) || c.g > cutEvery || gDrop > 0.6 && c.g > 0.5) { c.r = mod(c.r + 1.0, 97.0); c.g = 0.0; }
        c.b = on;
        gl_FragColor = c;
        return;
    }
    if (x > 18.5 && x < 19.5) { gl_FragColor = vec4(0.0); return; }

    gStatueF = statueFor(bi);
    gProg = prog;
    if (x > 43.5) {                                     // rock pose for the render pass
        float k = x > 67.5 ? x - 68.0 : x - 44.0;
        vec4 r = S(20.0 + k);
        float seed = dot(r.xyz, vec3(17.1, 31.7, 47.3)) + k * 3.1;
        float life = rockLife(seed);
        if (r.w > life || init) { gl_FragColor = vec4(0.0); return; }
        float spinT;
        vec3 c = rockPos(r.xyz, r.w, seed, spinT);
        float rr = rockRad(seed) * (1.0 - smoothstep(life - 0.6, life, r.w));
        gl_FragColor = x > 67.5 ? vec4(spinT, exp(-r.w * 2.5), fract(seed / 97.0) * 97.0, 0.0) : vec4(c, rr);
        return;
    }
    if (x > 19.5) {                                     // rocks
        float k = x - 20.0;
        vec4 r = init ? vec4(0.0, 0.0, 0.0, 99.0) : S(x);
        float seed = dot(r.xyz, vec3(17.1, 31.7, 47.3)) + k * 3.1;
        if (r.w < rockLife(seed)) { r.w += dt; gl_FragColor = r; return; }
        float n = nLasers();
        float li = mod(k + floor(TIME * 7.0), n);
        vec4 hit = S(1.0 + li), hn = S(9.0 + li);
        float chance = rockAmt * (0.008 + 0.05 * gE + 0.35 * gDrop + 0.04 * gChaos + 0.06 * clamp(audioBassHit * audioReact, 0.0, 1.0)) * dt * 60.0;
        if (hit.w > 0.1 && hn.w > 0.5 && prog < 0.97 && h11(k * 13.1 + fract(TIME * 0.37) * 911.0) < chance)
            r = vec4(hit.xyz + hn.xyz * 0.02 + (h33(TIME * 3.3 + k) - 0.5) * 0.04, 0.0);
        else r.w = 99.0;
        gl_FragColor = r;
        return;
    }

    // laser hit points
    float li = mod(x - 1.0, 8.0);
    float clk = TIME * 0.35 * scanSpeed + audioMidTime * 0.2 * audioReact * scanSpeed;
    vec3 E = emitterPos(li);
    vec3 T = laserTarget(li, prog, clk);
    vec3 rd = normalize(T - E);
    float t = 0.0, hitF = 0.0;
    for (int k = 0; k < 80; k++) {
        vec3 p = E + rd * t;
        float d = min(materialSDF(p), plinthSDF(p));
        if (d < 0.002) { hitF = 1.0; break; }
        t += d * 0.8;
        if (t > 8.0) break;
    }
    vec3 H = hitF > 0.5 ? E + rd * t : T;
    float on = laserOn(li) * (1.0 - smoothstep(0.97, 1.0, prog)) * step(li, nLasers() - 0.5);
    if (x < 8.5) gl_FragColor = vec4(H, on);
    else gl_FragColor = vec4(hitF > 0.5 ? calcNormal(H) : -rd, hitF);
}

// ── Render helpers ───────────────────────────────────────────────────────
float rayPointDist(vec3 ro, vec3 rd, vec3 p, out float tAlong) {
    tAlong = dot(p - ro, rd);
    return length(p - ro - rd * max(tAlong, 0.0));
}
vec3 raySegDist(vec3 ro, vec3 rd, vec3 a, vec3 b) {
    vec3 ba = b - a, oa = ro - a;
    float baba = max(dot(ba, ba), 1e-8), bard = dot(ba, rd), baoa = dot(ba, oa), rdoa = dot(rd, oa);
    float den = baba - bard * bard;
    float s = den > 1e-7 ? clamp((baoa - bard * rdoa) / den, 0.0, 1.0) : 0.0;
    float tr = max(dot(a + ba * s - ro, rd), 0.0);
    s = clamp(dot(ro + rd * tr - a, ba) / baba, 0.0, 1.0);
    vec3 q = a + ba * s;
    tr = max(dot(q - ro, rd), 0.0);
    return vec3(length(ro + rd * tr - q), tr, s);
}

float gMarble;
void marbleCols(float sel, out vec3 base, out vec3 vein) {
    if (sel < 0.5)      { base = vec3(0.93, 0.92, 0.9);  vein = vec3(0.45, 0.47, 0.52); }
    else if (sel < 1.5) { base = vec3(0.045, 0.045, 0.05); vein = vec3(0.92, 0.9, 0.86); }
    else if (sel < 2.5) { base = vec3(0.93, 0.74, 0.72); vein = vec3(0.62, 0.36, 0.38); }
    else if (sel < 3.5) { base = vec3(0.98, 0.88, 0.7);  vein = vec3(0.75, 0.48, 0.2); }
    else if (sel < 4.5) { base = vec3(0.95, 0.94, 0.91); vein = vec3(0.8, 0.6, 0.22); }
    else if (sel < 5.5) { base = vec3(0.05, 0.2, 0.12);  vein = vec3(0.75, 0.9, 0.8); }
    else if (sel < 6.5) { base = vec3(0.16, 0.38, 0.75); vein = vec3(0.92, 0.95, 1.0); }
    else if (sel < 7.5) { base = vec3(0.4, 0.07, 0.07);  vein = vec3(0.92, 0.86, 0.8); }
    else if (sel < 8.5) { base = vec3(0.95, 0.95, 0.97); vein = vec3(0.8, 0.82, 0.92); }
    else                { base = vec3(0.72, 0.72, 0.75); vein = vec3(0.5, 0.5, 0.53); }
}
vec3 marbleBase(vec3 p) {
    vec3 base, vein; marbleCols(gMarble, base, vein);
    vec3 w = p * 2.2 + vec3(0.0, 0.0, 4.0 * fbm3(p * 1.3));
    float v = abs(sin(w.x * 1.7 + w.y * 0.9 + 6.0 * fbm3(p * 2.1 + 1.7)));
    float fineV = abs(sin(dot(p, vec3(3.1, 5.3, -2.2)) * 2.0 + 5.0 * fbm3(p * 4.5)));
    float vm = (1.0 - smoothstep(0.0, 0.11, v)) + (1.0 - smoothstep(0.0, 0.05, fineV)) * 0.55 + (1.0 - smoothstep(0.0, 0.3, v)) * 0.18;
    vec3 c = mix(base, vein, clamp(vm * veinAmt, 0.0, 1.0));
    return c * (0.95 + 0.06 * fbm3(p * 9.0));
}
vec3 powderCol() { vec3 b, v; marbleCols(gMarble, b, v); return mix(b, vec3(0.9), 0.35); }

vec3 roomColor(vec3 rd, float sel) {
    float h = rd.y;
    if (sel < 0.5) return mix(vec3(0.012, 0.012, 0.016), vec3(0.03, 0.028, 0.035), smoothstep(-0.2, 0.4, h));
    if (sel < 1.5) {
        vec3 c = mix(vec3(0.22, 0.04, 0.16), vec3(0.03, 0.02, 0.1), smoothstep(-0.05, 0.6, h));
        c = mix(c, vec3(0.5, 0.12, 0.25), exp(-abs(h - 0.02) * 9.0) * 0.6);
        return c * 0.7;
    }
    return mix(vec3(0.82, 0.82, 0.84), vec3(0.93, 0.93, 0.95), smoothstep(-0.3, 0.6, h));
}

// shared laser state for the render pass
vec3 gLH[8]; vec3 gLN[8]; vec3 gLE[8]; vec3 gLC[8]; float gLO[8];
vec3 KEYD, KEYC, AMBC;
float gRoom;

vec3 laserLight(vec3 p, vec3 n) {
    vec3 acc = vec3(0.0);
    vec3 heatCol = vec3(1.0, 0.42, 0.1);
    float bs = burnSize;
    for (int i = 0; i < 8; i++) {
        if (gLO[i] < 0.01) continue;
        vec3 L = gLH[i] - p; float dl = length(L); L /= max(dl, 1e-4);
        float ndl = max(dot(n, L), 0.0) * 0.8 + 0.2;
        acc += (gLC[i] * 0.6 + heatCol * 0.4) * gLO[i] / (1.0 + dl * dl * 55.0) * ndl * 0.9;
        acc += mix(vec3(1.0), gLC[i], 0.25) * exp(-dl * dl * 9000.0 / (bs * bs)) * gLO[i] * 4.0;
        acc += heatCol * exp(-dl * 70.0 / bs) * gLO[i] * 0.8 * heatGlow;
    }
    return acc;
}

vec3 shadeSurf(vec3 p, vec3 n, vec3 rd, float id, float raw, float prog, bool manual, float rockHeat) {
    float sh = 1.0;
    float st = 0.02;
    for (int k = 0; k < 24; k++) {
        float h = mapD(p + n * 0.002 + KEYD * st);
        sh = min(sh, 9.0 * h / st);
        st += clamp(h, 0.015, 0.2);
        if (sh < 0.01 || st > 3.0) break;
    }
    sh = clamp(sh, 0.0, 1.0);
    float ao = 0.0;
    for (int k = 1; k <= 3; k++) { float hk = 0.04 * float(k); ao += (hk - mapD(p + n * hk)) / hk; }
    ao = clamp(1.0 - ao * 0.28, 0.0, 1.0);
    if (id > 2.5) ao = mix(ao, 1.0, 0.5);

    vec3 room = roomColor(rd, gRoom);
    vec3 alb; float rough = 0.6, sss = 0.0;
    bool stone = id > 1.5;
    if (stone) {
        alb = marbleBase(p);
        if (id > 2.5) { rough = 0.7; }
        else if (raw > 0.5) {
            float saw = 0.5 + 0.5 * sin(dot(p, vec3(0.0, 1.0, 0.25)) * 260.0);
            alb *= 0.82 + 0.06 * saw; alb = mix(alb, vec3(dot(alb, vec3(0.33))), 0.25); rough = 0.85;
        } else {
            float scorch = (1.0 - smoothstep(0.55, 0.95, prog)) * 0.18 * (0.5 + 0.5 * n3d(p * 30.0));
            alb *= 1.0 - scorch;
            // fresh cut sugar: tiny glinting crystals while it's still rough
            float grit = step(0.93, h13(floor(p * 260.0))) * (1.0 - smoothstep(0.7, 0.95, prog));
            alb += grit * 0.25;
            rough = mix(0.55, 0.15, smoothstep(0.75, 1.0, prog));
        }
        if (gMarble > 0.5 && gMarble < 1.5) sss = 0.15;
        else if (gMarble > 2.5 && gMarble < 3.5) sss = 1.0;
        else if (gMarble > 4.5 && gMarble < 5.5 || gMarble > 6.5 && gMarble < 7.5) sss = 0.3;
        else if (gMarble > 8.5) sss = 0.0;
        else sss = 0.6;
        if (gMarble > 7.5 && gMarble < 8.5) {
            float fr2 = 1.0 - max(dot(n, -rd), 0.0);
            float ir = fract(fr2 * 0.9 + 0.55 + fbm3(p * 1.5) * 0.25);
            alb = mix(alb, hsv(ir, 0.35, 1.0), 0.15 + 0.6 * fr2 * fr2);
            rough = min(rough, 0.3);
        }
    } else if (id > 0.5) {
        alb = gRoom > 1.5 ? vec3(0.9) : vec3(0.07, 0.07, 0.075);
        rough = 0.4;
    } else {
        alb = gRoom > 1.5 ? vec3(0.85, 0.85, 0.87) : (gRoom > 0.5 ? vec3(0.06, 0.03, 0.06) : vec3(0.025));
        rough = 0.25;
    }
    // marble powder settling on the floor and plinth top
    if (id < 1.5 && powder > 0.0) {
        float r = id < 0.5 ? length(p.xz * vec2(0.9, 1.1)) : 0.0;
        float pile = id < 0.5 ? exp(-max(r - 0.55, 0.0) * 4.0) * step(0.0, r - 0.5) : step(PLINTH_TOP - 0.01, p.y);
        float cov = clamp(prog * 1.3 * powder * pile * (0.55 + 0.6 * fbm3(p * 9.0)) - 0.15, 0.0, 1.0);
        cov = max(cov, cov * step(0.88, h13(floor(p * 300.0))) * 1.5);
        alb = mix(alb, powderCol(), cov);
        rough = mix(rough, 0.9, cov);
    }
    float dif = max(dot(n, KEYD), 0.0);
    float wrap = max(dot(n, KEYD) * 0.6 + 0.4, 0.0);
    vec3 hv = normalize(KEYD - rd);
    float spec = pow(max(dot(n, hv), 0.0), mix(120.0, 12.0, rough)) * (1.0 - rough) * 1.4;
    float fres = pow(1.0 - max(dot(n, -rd), 0.0), 4.0);
    vec3 lit = alb * KEYC * mix(dif, wrap, sss * 0.6) * sh;
    lit += alb * AMBC * (0.55 + 0.45 * n.y) * ao;
    lit += KEYC * spec * sh;
    lit += room * fres * 0.4 * ao;
    vec3 fillDir = normalize(-rd * vec3(1.0, 0.0, 1.0) + vec3(0.0, 0.5, 0.0));
    float reveal = smoothstep(0.85, 1.0, prog);
    lit += alb * KEYC * max(dot(n, fillDir), 0.0) * (0.22 + 0.25 * reveal) * ao;
    lit += alb * vec3(1.0, 0.97, 0.92) * max(n.y * 0.7 + 0.3, 0.0) * exp(-length(p.xz) * 1.5) * reveal * 0.45 * ao;
    lit += KEYC * pow(max(dot(n, normalize(fillDir - rd)), 0.0), mix(120.0, 12.0, rough)) * (1.0 - rough) * 0.5 * reveal;
    if (stone && id < 2.5) {
        float th = clamp(mapD(p - n * 0.06) / -0.06, 0.0, 1.0);
        lit += alb * KEYC * (1.0 - th) * 0.18 * sss * max(dot(-n, KEYD) + 0.6, 0.0);
    }
    // liquid chrome: mirror the room, the emitters and the beams
    if (stone && gMarble > 8.5) {
        vec3 rf = reflect(rd, n);
        vec3 env = roomColor(rf, gRoom) * 1.4 + vec3(0.9) * pow(max(dot(rf, KEYD), 0.0), 60.0) * 2.0;
        for (int i = 0; i < 8; i++) env += gLC[i] * gLO[i] * pow(max(dot(rf, normalize(gLE[i] - p)), 0.0), 300.0) * 6.0;
        env += vec3(0.25) * smoothstep(-0.1, 0.4, rf.y) * (0.5 + 0.5 * fbm3(rf * 3.0));
        // studio strip lights for the chrome to catch
        float az = atan(rf.z, rf.x);
        env += vec3(1.6) * smoothstep(0.93, 0.97, abs(sin(az * 2.0))) * smoothstep(0.05, 0.25, rf.y) * smoothstep(0.85, 0.6, rf.y);
        env += vec3(0.9) * smoothstep(0.9, 0.97, rf.y);
        env += vec3(0.12) * smoothstep(0.0, -0.4, rf.y);
        lit = mix(lit, env * mix(vec3(1.0), alb, 0.3), 0.85);
    }
    lit += laserLight(p, n);
    vec3 heatCol = vec3(1.0, 0.42, 0.1);
    if (stone && id < 2.5 && raw < 0.5) {
        float near = 0.0;
        for (int i = 0; i < 8; i++) near += gLO[i] * exp(-length(gLH[i] - p) * 4.0);
        float actv = 0.0;
        if (!manual) {
            float mNow = stockAt(p, prog), mLag = stockAt(p, max(prog - 0.025, 0.0));
            actv = clamp((mLag - mNow) * 7.0, 0.0, 1.0);
        }
        lit += heatCol * (actv * (0.05 + clamp(near, 0.0, 1.5)) + (manual ? near * 0.35 : 0.0)) * heatGlow * (1.0 - smoothstep(0.92, 1.0, prog)) * (0.6 + 0.6 * clamp(audioBass * audioReact, 0.0, 1.0));
    }
    lit += heatCol * rockHeat * heatGlow * 1.5;
    if (id < 0.5) {
        for (int i = 0; i < 8; i++) {
            float dl = length((gLH[i] - p).xz);
            lit += gLC[i] * gLO[i] * 0.06 / (1.0 + dl * dl * 6.0);
        }
        lit *= mix(0.4, 1.0, exp(-length(p.xz) * 0.35));
    }
    return lit;
}

// ── Marble particles: grain streams pouring off every cut ────────────────
vec3 gSCol; float gSHot;
float streamDensity(vec3 p) {
    float d = 0.0; gSCol = vec3(0.0); gSHot = 0.0;
    for (int i = 0; i < 8; i++) {
        if (gLO[i] < 0.02) continue;
        vec3 H = gLH[i];
        float fall = H.y - p.y;
        if (fall < -0.06) continue;
        float f = max(fall, 0.0);
        vec2 c = H.xz + gLN[i].xz * (0.025 + 0.22 * f * partSpread);
        float spread = (0.018 + 0.09 * f * partSpread) * (1.0 + 0.5 * gChaos);
        vec2 dx = p.xz - c;
        float m = exp(-dot(dx, dx) / (spread * spread));
        m *= smoothstep(-0.06, 0.0, fall) * (1.0 - smoothstep(0.0, 1.6, f) * 0.6);
        vec3 dp = p - H;
        float r2 = dot(dp, dp);
        float w = (m + exp(-r2 * 260.0) * 1.2) * min(gLO[i], 1.5);
        d += w;
        gSCol += gLC[i] * gLO[i] * (m * 0.25 + exp(-r2 * 40.0) * 0.8);
        gSHot += gLO[i] * exp(-sqrt(r2) * 18.0);
    }
    return d;
}
vec4 marbleParticles(vec3 ro, vec3 rd, float tScene, float prog) {
    if (partAmt <= 0.0 && dustAmt <= 0.0) return vec4(0.0);
    // bounds: the work zone around the plinth
    vec3 lo = vec3(1e5), hi = vec3(-1e5);
    for (int i = 0; i < 8; i++) {
        if (gLO[i] < 0.02) continue;
        lo = min(lo, gLH[i]); hi = max(hi, gLH[i]);
    }
    if (hi.x < lo.x) return vec4(0.0);
    float pad = 0.12 + 0.36 * partSpread * (1.0 + 0.5 * gChaos);
    lo = vec3(lo.x - pad, 0.0, lo.z - pad); hi = vec3(hi.x + pad, hi.y + 0.1, hi.z + pad);
    vec3 bc = (lo + hi) * 0.5, bh = (hi - lo) * 0.5;
    vec3 nn;
    vec2 bt = boxHit(ro - bc, rd, bh, nn);
    float t0 = max(bt.x, 0.0), t1 = min(bt.y, tScene);
    if (t1 <= t0) return vec4(0.0);
    const int NS = 24;
    float dtS = (t1 - t0) / float(NS);
    float t = t0 + dtS * h12(gl_FragCoord.xy + fract(TIME) * 37.0);
    vec3 acc = vec3(0.0); float tr = 1.0;
    vec3 pc = powderCol();
    float fallV = 0.55 * partFall * (1.0 + 0.6 * gChaos + 0.4 * gDrop);
    for (int s = 0; s < NS; s++) {
        vec3 p = ro + rd * t;
        float dens = streamDensity(p);
        if (dens > 0.01) {
            // soft dust body
            vec3 q = p * 9.0 + vec3(0.0, TIME * fallV * 9.0, 0.0);
            float dn = fbm3(q) - 0.25;
            float soft = max(dn, 0.0) * dens * dustAmt * 1.4;
            // individual grains: two sizes, falling in jittered columns, each tested against the ray
            float grains = 0.0;
            for (int g = 0; g < 2; g++) {
                float cs = g == 0 ? 0.022 : 0.011;
                vec2 col = floor(p.xz / cs);
                float ch = h12(col + float(g) * 17.0);
                float yOff = TIME * fallV * (0.7 + 0.6 * ch) + ch * 13.0;
                float cy = floor((p.y + yOff) / cs);
                vec3 rnd = h33(dot(vec3(col, cy), vec3(1.0, 57.0, 113.0)) + float(g) * 7.1);
                if (rnd.z < clamp(dens * 0.9 * partAmt, 0.0, 0.95)) {
                    vec3 gp = vec3((col.x + 0.15 + 0.7 * rnd.x) * cs, (cy + 0.5) * cs - yOff, (col.y + 0.15 + 0.7 * rnd.y) * cs);
                    float ta; float dd = rayPointDist(ro, rd, gp, ta);
                    float rad = cs * 0.13 * partSize * (0.6 + 0.8 * rnd.z);
                    grains += exp(-dd * dd / (rad * rad)) * (g == 0 ? 1.0 : 0.8);
                }
            }
            float a = clamp(soft * dtS * 30.0 + grains * 0.85, 0.0, 1.0);
            if (a > 0.001) {
                vec3 L = pc * (KEYC * 0.55 + AMBC * 0.8);
                L += gSCol;
                L += vec3(1.0, 0.45, 0.12) * gSHot * heatGlow * 1.5;
                L += vec3(1.0) * grains * 0.25 * (0.4 + clamp(audioHigh * audioReact, 0.0, 1.0));
                acc += tr * a * L;
                tr *= 1.0 - a;
                if (tr < 0.03) break;
            }
        }
        t += dtS;
    }
    return vec4(acc, 1.0 - tr);
}

void main() {
    showDynamics();
    if (PASSINDEX == 0) { statePass(); return; }

    vec2 fc = gl_FragCoord.xy;
    vec2 uv = (fc - 0.5 * RENDERSIZE) / RENDERSIZE.y;
    float ar = audioReact;

    vec4 s0 = S(0.0);
    float progAuto = clamp((s0.g + s0.r) / NSTEP, 0.0, 1.0);
    bool manual = float(carveMode) > 0.5;
    float prog = manual ? manualProg : progAuto;
    gProg = prog;
    gStatueF = statueFor(s0.b);
    gMarble = marbleFor(s0.b);
    float held = manual ? 0.0 : s0.a;
    float age = manual ? 100.0 : S(17.0).r;
    float roomFade = (loopCarve ? 1.0 - smoothstep(holdTime, holdTime + 1.6, held) : 1.0) * smoothstep(0.0, 1.8, age);
    gRoom = float(roomSel);
    KEYD = normalize(vec3(-0.5, 0.85, 0.45));
    KEYC = gRoom > 1.5 ? vec3(1.0, 0.98, 0.95) * 1.1 : (gRoom > 0.5 ? vec3(1.0, 0.8, 0.85) * 0.75 : vec3(0.95, 0.95, 1.0) * 0.8);
    AMBC = gRoom > 1.5 ? vec3(0.62, 0.62, 0.66) : (gRoom > 0.5 ? vec3(0.2, 0.08, 0.2) : vec3(0.06, 0.065, 0.08));

    // lasers
    float beat = clamp(audioBeatPulse, 0.0, 1.0) * ar;
    float bassHit = clamp(audioBass, 0.0, 1.0) * ar;
    float laserPow = laserBright * (0.75 + 0.6 * gLift + 0.6 * beat * drama + 1.2 * gDrop) * roomFade;
    for (int i = 0; i < 8; i++) {
        float fi = float(i);
        vec4 h = S(1.0 + fi), n = S(9.0 + fi);
        gLH[i] = h.xyz; gLN[i] = normalize(n.xyz + 1e-5);
        gLE[i] = emitterPos(fi); gLC[i] = laserColor(fi);
        float o = h.w * laserPow;
        o *= 0.88 + 0.12 * sin(TIME * 90.0 + fi * 2.0) * clamp(audioHigh * ar * 2.0, 0.0, 1.0);
        float rate = 3.0 + 10.0 * clamp(audioHigh * ar + gDrop, 0.0, 1.0);
        o *= mix(1.0, step(0.45, fract(TIME * rate + fi * 0.37)), strobe);
        gLO[i] = fi < nLasers() ? o : 0.0;
    }

    // ── camera ──
    float cm = float(camMode);
    float spd = orbitSpeed;
    float camR = 3.4 / zoom;
    float camY = 1.35 + camHeight;
    float orbit = TIME * 0.045 * spd + 0.4;
    vec3 look = vec3(0.0, 0.98, 0.0);
    float fl = 1.9 * fov;
    if (cm > 0.5 && cm < 1.5) { orbit = 0.15 * sin(TIME * 0.03 * spd); }
    else if (cm > 1.5 && cm < 2.5) { camY = 0.55 + camHeight; look.y = 1.05; fl = 1.45 * fov; }
    else if (cm > 2.5 && cm < 3.5) {
        float tt = TIME * spd;
        orbit = tt * 0.09 + 0.4;
        camR *= 0.72 + 0.38 * sin(tt * 0.13);
        camY = 0.55 + 1.5 * (0.5 + 0.5 * sin(tt * 0.071)) + camHeight;
        fl *= 0.85 + 0.3 * (0.5 + 0.5 * sin(tt * 0.05));
    } else if (cm > 3.5 && cm < 4.5) {
        orbit = 0.5 * sin(TIME * 0.05 * spd) + 0.25 * sin(TIME * 0.13 * spd);
        camR *= 0.8;
        camY = 1.1 + 0.3 * sin(TIME * 0.09) + camHeight;
    } else if (cm > 4.5 && cm < 5.5) {
        vec4 c = S(18.0);
        vec3 r3 = h33(c.r * 3.17 + 1.0);
        orbit = r3.x * TAU;
        camY = mix(0.35, 2.4, r3.y) + camHeight;
        camR *= mix(0.55, 1.2, r3.z) * (1.0 - 0.1 * min(c.g / 5.0, 1.0));
        fl *= mix(0.8, 1.5, h11(c.r * 5.9));
        look.y = mix(0.75, 1.3, h11(c.r * 2.3));
        orbit += c.g * 0.02 * spd;
    } else if (cm > 5.5 && cm < 6.5) {
        vec3 lh = vec3(0.0); float wsum = 0.0;
        for (int i = 0; i < 8; i++) { lh += gLH[i] * gLO[i]; wsum += gLO[i]; }
        lh = wsum > 0.01 ? lh / wsum : vec3(0.0, 1.0, 0.0);
        look = mix(look, lh, 0.7);
        camR *= 0.55;
        camY = look.y + 0.25 + camHeight * 0.5;
        orbit = TIME * 0.06 * spd + 0.4;
        fl *= 1.15;
    } else if (cm > 6.5) {
        float ph = fract(TIME * 0.02 * spd);
        float e = ph * ph * (3.0 - 2.0 * ph);
        camY = mix(0.25, 3.6, e) + camHeight;
        camR *= mix(0.8, 0.55, e);
        orbit = TIME * 0.03 * spd + 0.4;
        look.y = mix(1.2, 0.9, e);
    }
    vec3 ro = vec3(camR * sin(orbit), camY, camR * cos(orbit));
    // shake: handheld drift + bass + drop + chaos
    float shk = camShake * (0.25 + 0.8 * bassHit + 1.5 * gDrop + 0.6 * gChaos) + (cm > 3.5 && cm < 4.5 ? 0.35 : 0.0);
    vec3 shake = (vec3(n3d(vec3(TIME * 2.3, 0.0, 0.0)), n3d(vec3(0.0, TIME * 2.1, 5.0)), n3d(vec3(7.0, 0.0, TIME * 1.9))) - 0.5) * 0.12;
    shake += (vec3(n3d(vec3(TIME * 23.0, 1.0, 0.0)), n3d(vec3(3.0, TIME * 21.0, 0.0)), 0.0) - 0.5) * 0.03 * (bassHit + gDrop * 2.0);
    ro += shake * shk;
    look += shake * shk * 0.4;
    fl *= 1.0 + dropPunch * (0.35 * gDrop + 0.07 * beat * drama);
    vec3 fw = normalize(look - ro);
    vec3 rt = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(rt, fw);
    float roll = shk * 0.05 * (n3d(vec3(TIME * 1.7, 9.0, 0.0)) - 0.5);
    vec2 uvr = rot(roll) * uv;
    vec3 rd = normalize(uvr.x * rt + uvr.y * up + fl * fw);

    // ── march ──
    float t = 0.0; vec2 hit = vec2(-1.0); float tMax = 16.0;
    for (int k = 0; k < 140; k++) {
        vec3 p = ro + rd * t;
        vec2 r = mapScene(p);
        if (r.x < 0.0008 * t) { hit = vec2(t, r.y); break; }
        t += r.x * 0.82;
        if (t > tMax) break;
    }
    if (hit.x < 0.0) t = tMax;

    // ── rocks: flying chips + the rubble pile ──
    float rockT = 1e9; vec3 rockN = vec3(0.0); float rockHeat = 0.0;
    if (rockAmt > 0.0) {
        for (int k = 0; k < NROCK; k++) {
            vec4 pose = S(44.0 + float(k));
            if (pose.w <= 0.0005) continue;
            float tb; if (rayPointDist(ro, rd, pose.xyz, tb) > pose.w * 1.9) continue;
            vec4 ex = S(68.0 + float(k));
            vec3 nk;
            float tk = chipHit(ro, rd, pose.xyz, pose.w, ex.z, ex.x, nk);
            if (tk > 0.0 && tk < rockT) { rockT = tk; rockN = nk; rockHeat = ex.y; }
        }
    }
    if (rubbleAmt > 0.0) {
        for (int k = 0; k < NRUB; k++) {
            float fk = float(k) + 0.5;
            if (h11(fk * 7.7 + 1.3) > prog * rubbleAmt * 1.1) continue;
            float seed = fk * 19.3 + 4.0;
            float rr = (0.018 + 0.03 * h11(seed * 1.1)) * rockSize;
            vec3 c;
            if (k < 6) {
                float a = h11(seed * 2.2) * TAU;
                vec2 e = vec2(cos(a), sin(a));
                vec2 m = e / max(abs(e.x) / 0.5, abs(e.y) / 0.42);
                c = vec3(m.x, PLINTH_TOP + rr * 0.6, m.y);
            } else {
                float a = h11(seed * 2.2) * TAU;
                float rad = 0.68 + 0.5 * h11(seed * 3.3) * h11(seed * 1.7);
                c = vec3(cos(a) * rad, rr * 0.6, sin(a) * rad * 0.9);
            }
            float tb; if (rayPointDist(ro, rd, c, tb) > rr * 1.9) continue;
            vec3 nk;
            float tk = chipHit(ro, rd, c, rr, seed, 0.0, nk);
            if (tk > 0.0 && tk < rockT) { rockT = tk; rockN = nk; rockHeat = 0.0; }
        }
    }

    vec3 room = roomColor(rd, gRoom);
    vec3 col = room;
    float alpha = 0.0;
    float tSurf = t;
    if (rockT < t) {
        alpha = 1.0;
        tSurf = rockT;
        vec3 p = ro + rd * rockT;
        col = shadeSurf(p, normalize(rockN), rd, 3.0, 0.0, prog, manual, rockHeat);
        col = mix(col, room, 1.0 - exp(-max(rockT - 2.5, 0.0) * (gRoom > 1.5 ? 0.04 : 0.07)));
    } else if (hit.x > 0.0) {
        alpha = 1.0;
        vec3 p = ro + rd * t;
        vec3 n = calcNormal(p);
        materialSDF(p);
        col = shadeSurf(p, n, rd, hit.y, gRawFace, prog, manual, 0.0);
        col = mix(col, room, 1.0 - exp(-max(t - 2.5, 0.0) * (gRoom > 1.5 ? 0.04 : 0.07)));
    }

    // ── marble particles + dust ──
    vec4 part = marbleParticles(ro, rd, tSurf, prog);
    col = col * (1.0 - part.a) + part.rgb;
    alpha = max(alpha, part.a);

    // ── beams ──
    float haze = 0.35 + 0.9 * hazeAmt;
    float bw = 0.0035 * beamWidth * (1.0 + 0.5 * bassHit * drama + 0.6 * gDrop);
    for (int i = 0; i < 8; i++) {
        if (gLO[i] < 0.01) continue;
        vec3 q = raySegDist(ro, rd, gLE[i], gLH[i]);
        float vis = step(q.y, tSurf + 0.01);
        float core = exp(-q.x * q.x / (bw * bw));
        float glowB = bw * 3.0 / (q.x + bw * 3.0);
        vec3 bc = gLC[i];
        col += (mix(bc, vec3(1.0), 0.55) * core * 1.1 + bc * glowB * 0.22 * haze) * gLO[i] * vis;
        float te; float de = rayPointDist(ro, rd, gLE[i], te);
        if (te > 0.0 && showEmitters) col += bc * (exp(-de * de * 1800.0) * 1.5 + 0.0002 / (de * de + 0.004)) * gLO[i];
        float th; float dh = rayPointDist(ro, rd, gLH[i], th);
        col += (vec3(1.0, 0.6, 0.3) * 0.5 + bc * 0.5) * 0.012 / (dh * dh + 0.0015) * gLO[i] * 0.12 * (0.6 + haze) * burnSize * mix(0.35, 1.0, step(th, tSurf + 0.05));
        // club fan beams sweeping the haze
        if (fanBeams > 0.01) {
            vec3 E = gLE[i];
            vec3 dn = normalize(vec3(-E.x, -E.y * 0.8, -E.z));
            vec3 ta = normalize(cross(dn, vec3(0.0, 1.0, 0.0)));
            vec3 tb = cross(dn, ta);
            for (int f = 0; f < 3; f++) {
                float ff = float(f);
                float a = ff / 3.0 * TAU + TIME * (0.4 + 0.3 * float(i)) * scanSpeed + audioBassTime * 0.15 * ar;
                float spread = 0.35 + 0.35 * sin(TIME * 0.21 + float(i));
                vec3 d = normalize(dn + (ta * cos(a) + tb * sin(a)) * spread);
                float len = d.y < -0.05 ? E.y / -d.y : 6.0;
                vec3 qf = raySegDist(ro, rd, E, E + d * len);
                float vf = step(qf.y, tSurf + 0.01);
                float bf = bw * 0.7;
                col += bc * (exp(-qf.x * qf.x / (bf * bf)) * 0.6 + bf * 2.0 / (qf.x + bf * 2.0) * 0.1 * haze) * gLO[i] * fanBeams * vf * (1.0 - qf.z * 0.6);
            }
        }
    }

    // ── sparks ──
    float sparkK = sparkAmt * (0.5 + 0.8 * clamp(audioHigh * ar * 1.6, 0.0, 1.0) + 1.2 * gDrop + 0.5 * beat * drama + 0.5 * gChaos);
    if (sparkK > 0.01) {
        for (int i = 0; i < 8; i++) {
            if (gLO[i] < 0.05) continue;
            float tH; float dH = rayPointDist(ro, rd, gLH[i], tH);
            if (dH > 0.55 * (1.0 + 0.6 * gDrop + 0.5 * gChaos)) continue;
            vec3 N = gLN[i];
            vec3 tA = normalize(cross(N, abs(N.y) < 0.9 ? vec3(0.0, 1.0, 0.0) : vec3(1.0, 0.0, 0.0)));
            vec3 tB = cross(N, tA);
            for (int k = 0; k < 8; k++) {
                float fk = float(k) + float(i) * 17.0;
                float life = 0.55 + 0.4 * h11(fk * 3.1);
                float ph = fract(TIME / life + h11(fk * 7.7));
                float seed = floor(TIME / life + h11(fk * 7.7)) + fk * 13.0;
                float a1 = h11(seed * 1.3) * TAU, sp = (0.6 + 1.6 * h11(seed * 2.9)) * (0.8 + 0.6 * gDrop + 0.5 * gChaos);
                vec3 v = normalize(N * (0.6 + h11(seed * 4.1)) + (tA * cos(a1) + tB * sin(a1)) * 0.9) * sp;
                float tt = ph * life;
                vec3 pos = gLH[i] + v * tt + vec3(0.0, -2.8, 0.0) * tt * tt;
                vec3 vel = v + vec3(0.0, -5.6, 0.0) * tt;
                vec3 q = raySegDist(ro, rd, pos, pos - vel * 0.025);
                if (q.y > tSurf + 0.02) continue;
                float w = 0.0018;
                float g = exp(-q.x * q.x / (w * w)) + 0.15 * w / (q.x + w);
                vec3 sc = mix(vec3(1.0, 0.95, 0.8), mix(vec3(1.0, 0.45, 0.1), gLC[i], 0.35), ph);
                col += sc * g * (1.0 - ph) * gLO[i] * sparkK * 0.9;
            }
        }
    }

    // ── finish ──
    col *= roomFade;
    col += vec3(1.0) * gDrop * gDrop * 0.06 * drama;
    col = col / (1.0 + col * 0.55);
    col = pow(col, vec3(0.92));
    col *= 1.0 - 0.35 * dot(uv * 0.8, uv * 0.8);
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.04;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(max(alpha, dot(col, vec3(0.33)) * 2.0), 0.0, 1.0) : 1.0);
}
