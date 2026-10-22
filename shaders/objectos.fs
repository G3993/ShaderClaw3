/*{
  "DESCRIPTION": "Objectos — a wobbly totem of 3 to 6 soft 3D objects (spheres, fat rings, clover crosses, puffy flowers, twin blobs, rounded cubes, pills, pucks) balanced on top of each other and dancing. A real spring–damper simulation runs every frame in a tiny persistent buffer: each object has its own squash-and-stretch spring and two lean springs coupled to the one beneath it, so motion travels up the stack with lag and overshoot like jelly; the whole totem hops on beats, lands, and squashes on impact. Bass compresses the stack, beats make it jump, mids add a rubbery wobble and twist, highs make the glossy highlights flare. Each slot slowly morphs into another shape, and the materials keep trading — glossy black, glowing gradient, dark-rimmed matte, stripes, dots, checks, iridescent — in a palette pulled from the references (signal red, cyan, maroon, pink, lavender, ink). Rest: a small, sparse totem of three far away, swaying lazily in soft desaturated light; Build: the stack grows, coils down into a squash, spins faster and trembles while the camera pushes in and the highlights flicker; Drop: a white flash and shockwave ring as the whole totem is punched up into a big hop and scale burst, settling into a tall, saturated, double-time dancing peak.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "objCount",    "LABEL": "Objects",          "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 6.0, "DEFAULT": 4.0, "GROUP": "Stack" },
    { "NAME": "shapePeriod", "LABEL": "Shape Change (s)", "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 40.0, "DEFAULT": 9.0, "GROUP": "Stack" },
    { "NAME": "matPeriod",   "LABEL": "Material Change (s)", "TYPE": "float", "AUDIO": false, "MIN": 2.0, "MAX": 40.0, "DEFAULT": 7.0, "GROUP": "Stack" },
    { "NAME": "objScale",    "LABEL": "Object Size",      "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Stack" },
    { "NAME": "dance",       "LABEL": "Dance",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "tempo",       "LABEL": "Idle Tempo",       "TYPE": "float", "MIN": 0.2, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "squash",      "LABEL": "Squash & Stretch", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "jelly",       "LABEL": "Jelly (bounciness)","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Physics" },
    { "NAME": "jump",        "LABEL": "Beat Jump",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "deform",      "LABEL": "Deform / Twist",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Physics" },
    { "NAME": "spin",        "LABEL": "Spin",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Physics" },
    { "NAME": "patScale",    "LABEL": "Pattern Scale",    "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Material" },
    { "NAME": "gloss",       "LABEL": "Gloss",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Material" },
    { "NAME": "glowAmt",     "LABEL": "Glow",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Material" },
    { "NAME": "c1",          "LABEL": "Color 1",          "TYPE": "color", "DEFAULT": [1.0, 0.02, 0.02, 1.0], "GROUP": "Palette" },
    { "NAME": "c2",          "LABEL": "Color 2",          "TYPE": "color", "DEFAULT": [0.0, 0.9, 0.95, 1.0], "GROUP": "Palette" },
    { "NAME": "c3",          "LABEL": "Color 3",          "TYPE": "color", "DEFAULT": [0.42, 0.0, 0.12, 1.0], "GROUP": "Palette" },
    { "NAME": "c4",          "LABEL": "Color 4",          "TYPE": "color", "DEFAULT": [1.0, 0.55, 0.92, 1.0], "GROUP": "Palette" },
    { "NAME": "c5",          "LABEL": "Color 5",          "TYPE": "color", "DEFAULT": [0.55, 0.45, 1.0, 1.0], "GROUP": "Palette" },
    { "NAME": "c6",          "LABEL": "Color 6",          "TYPE": "color", "DEFAULT": [0.03, 0.03, 0.04, 1.0], "GROUP": "Palette" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Palette" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Palette" },
    { "NAME": "zoom",        "LABEL": "Zoom",             "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Camera" },
    { "NAME": "camOrbit",    "LABEL": "Camera Orbit",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Camera" },
    { "NAME": "shadow",      "LABEL": "Floor Shadow",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Camera" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.93, 0.94, 0.99, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.02, 0.1, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
    { "TARGET": "phys", "PERSISTENT": true, "WIDTH": "8", "HEIGHT": "2" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Objectos
//   pass 0  phys (8×2, persistent): per-slot spring state
//           row 0 = (squash s, squash vel, lean x, lean x vel)
//           row 1 = (lean z, lean z vel, hop height, hop vel)   hop: slot 0 only
//   pass 1  final: builds the stack from the state (contact-to-contact along
//           the accumulated lean), raymarches it, shades with morphing
//           materials, composites over the user background.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

// ── Studio Palette: remap to black → accent → white (red or blue) ─────────
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
#define MAXN 6

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
vec3 hueRot(vec3 c, float h) {
    if (h <= 0.0) return c;
    float a = h * TAU; vec3 k = vec3(0.57735); float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

// ── Show dynamics: rest → build → peak / drop ───────────────────────────
float gRest, gBuild, gPeak, gDrop, gLift;
void showDynamics() {
    float m = float(showState);
    float e = clamp(audioEnergy, 0.0, 1.0);
    float b = clamp(audioBuildup, 0.0, 1.0);
    float d = clamp(audioDrop, 0.0, 1.0);
    if (m > 0.5) {                                   // manual override for the VJ
        if (m < 1.5)      { e = 0.04; b = 0.0; d = 0.0; }                                      // Rest
        else if (m < 2.5) { float r = fract(TIME / 8.0); e = 0.3 + 0.35 * r; b = r * r; d = 0.0; } // Build: 8 s riser, loops
        else              { e = 1.0; b = 0.0; d = exp(-fract(TIME / 4.0) * 5.0); } // Peak: hits every 4 s + beats
    }
    float k = dynamics;
    gBuild = b * k;                                                  // tension 0..1
    gDrop  = d * k;                                                  // impact impulse 0..1
    gPeak  = smoothstep(0.5, 0.9, e) * k;                            // sustained high section
    gRest  = (1.0 - smoothstep(0.08, 0.4, e)) * (1.0 - b) * k;       // quiet / breakdown
    gLift  = mix(0.5, clamp(0.15 + e * 0.7 + gBuild * 0.25 + d * 0.3, 0.0, 1.0), k);  // 0 rest … 1 peak; 0.5 when dynamics = 0
}

// ═════════════════════════════════════════════════════════════════════════
//  PHYSICS
// ═════════════════════════════════════════════════════════════════════════
vec4 st0(float i) { return texture2D(phys, vec2((i + 0.5) / 8.0, 0.25)); }
vec4 st1(float i) { return texture2D(phys, vec2((i + 0.5) / 8.0, 0.75)); }

vec4 physPass() {
    // slots indexed by normalised position: works whether the host allocates
    // this pass at 8×2 (ShaderClaw web) or at full canvas size (Easel native)
    vec2 su = gl_FragCoord.xy / RENDERSIZE.xy;
    float i = min(floor(su.x * 8.0), 7.0);
    float row = min(floor(su.y * 2.0), 1.0);
    if (FRAMEINDEX < 3) return vec4(0.0);
    float ar = audioReact;
    vec4 a0 = st0(i), a1 = st1(i);
    vec4 b0 = i > 0.5 ? st0(i - 1.0) : vec4(0.0);
    vec4 b1 = i > 0.5 ? st1(i - 1.0) : vec4(0.0);
    vec4 g1 = st1(0.0);

    float dt = clamp(TIMEDELTA, 0.0, 1.0 / 30.0) * 0.5;   // two substeps
    float damp = mix(9.0, 2.2, jelly);
    float kS = 70.0, kT = 34.0;
    float beat = audioBeatPulse * audioBeatPulse * ar;
    float tt = TIME * tempo + audioMidTime * 0.3 * tempo;
    // rest: lazy, small groove … peak: big groove that crossfades into double time
    float dAmt = dance * mix(0.35, 1.45, gLift);
    float dbl = smoothstep(0.55, 0.95, gLift);
    float trem = gBuild * gBuild * gBuild;            // tremor near the top of the build

    float s = a0.x, sv = a0.y, tx = a0.z, txv = a0.w;
    float tz = a1.x, tzv = a1.y, hop = a1.z, hv = a1.w;

    for (int k = 0; k < 2; k++) {
        // squash spring: bass compresses (more at the bottom), beats stretch,
        // a groove keeps it alive in silence, neighbour below drags it along
        float fS = -1.6 * bassP() * ar * (1.0 - 0.1 * i)
                 + 3.0 * beat
                 + 0.9 * dAmt * mix(sin(tt * 4.2 - i * 0.7), sin(tt * 8.4 - i * 0.9), dbl)
                 - 2.2 * gBuild * (1.0 - 0.12 * i)                  // build: the stack coils down
                 + 6.0 * gDrop * gDrop                              // drop: everything springs tall
                 + 18.0 * (b0.x - s) * step(0.5, i);
        float acc = -kS * s - damp * sv + fS * 12.0 * squash;
        sv += acc * dt; s += sv * dt;
        // landing squash on the base
        if (i < 0.5 && g1.z + g1.w * dt * 2.0 <= 0.0 && g1.w < -0.6) sv -= 2.2 * min(-g1.w, 4.0) * squash;

        // lean springs: target follows the object below + a side-to-side dance
        float swayX = dAmt * (0.11 * mix(sin(tt * 2.1 - i * 0.8), sin(tt * 4.2 - i * 0.8), dbl) + 0.05 * sin(tt * 3.3 + i)) * (1.0 + 0.6 * bassP() * ar) * (1.0 - 0.6 * gBuild)
                    + 0.05 * trem * sin(TIME * 47.0 + i * 2.3);
        float swayZ = dAmt * 0.07 * sin(tt * 1.7 - i * 0.6 + 1.3) * (1.0 - 0.6 * gBuild) + 0.035 * trem * sin(TIME * 39.0 + i * 1.7);
        float flip = mod(floor(TIME * 1.7), 2.0) * 2.0 - 1.0;
        float accX = -kT * (tx - (0.55 * b0.z + swayX)) - damp * 0.8 * txv + beat * flip * 5.0 * dance + gDrop * gDrop * flip * 9.0 * (0.5 + 0.2 * i) + 25.0 * (b0.w - txv) * 0.05 * step(0.5, i);
        float accZ = -kT * (tz - (0.55 * b1.x + swayZ)) - damp * 0.8 * tzv;
        txv += accX * dt; tx += txv * dt;
        tzv += accZ * dt; tz += tzv * dt;

        // hop (slot 0 carries the whole stack)
        if (i < 0.5) {
            hv -= 14.0 * dt;
            hop += hv * dt;
            if (hop <= 0.0) { hop = 0.0; hv = hv < -0.6 ? -hv * 0.22 * jelly : 0.0; }
            if (audioBeatPulse > 0.88 && hop < 0.02 && hv <= 0.1) hv = 3.6 * jump * ar * mix(0.5, 1.2, gLift);
            if (gDrop > 0.6 && hop < 0.02 && hv <= 0.1) hv = 5.5 * max(jump, 0.4);   // the drop launches the totem
        }
    }
    s = clamp(s, -0.45, 0.6); tx = clamp(tx, -0.35, 0.35); tz = clamp(tz, -0.3, 0.3);
    sv = clamp(sv, -20.0, 20.0); txv = clamp(txv, -20.0, 20.0); tzv = clamp(tzv, -20.0, 20.0);
    hop = clamp(hop, 0.0, 2.0); hv = clamp(hv, -12.0, 12.0);
    return row < 0.5 ? vec4(s, sv, tx, txv) : vec4(tz, tzv, hop, hv);
}

// ═════════════════════════════════════════════════════════════════════════
//  SHAPES (local frame, resting on nothing: centred, half-height returned)
// ═════════════════════════════════════════════════════════════════════════
float shapeHalfH(float t) {
    if (t < 0.5) return 0.5;      // sphere
    if (t < 1.5) return 0.25;     // fat ring
    if (t < 2.5) return 0.26;     // clover cross
    if (t < 3.5) return 0.42;     // puffy flower
    if (t < 4.5) return 0.38;     // twin blob
    if (t < 5.5) return 0.4;      // rounded cube
    if (t < 6.5) return 0.29;     // pill
    return 0.3;                   // puck
}
float shapeSDF(float t, vec3 p) {
    if (t < 0.5) return length(p) - 0.5;
    if (t < 1.5) { vec2 q = vec2(length(p.xz) - 0.56, p.y); return length(q) - 0.25; }
    if (t < 2.5) {
        vec3 q = p; q.xz = rot(0.785) * q.xz;
        vec3 a = q; a.x -= clamp(a.x, -0.5, 0.5);
        vec3 b = q; b.z -= clamp(b.z, -0.5, 0.5);
        return smin(length(a) - 0.26, length(b) - 0.26, 0.12);
    }
    if (t < 3.5) {
        float d = length(p - vec3(0.0, 0.1, 0.0)) - 0.3;
        for (int k = 0; k < 5; k++) {
            float a = float(k) * TAU / 5.0;
            vec3 c = vec3(cos(a) * 0.28, -0.12, sin(a) * 0.28);
            vec3 q = p - c; q.y *= 0.8;
            d = smin(d, length(q) - 0.21, 0.1);
        }
        return d;
    }
    if (t < 4.5) {
        float d1 = length(p - vec3(-0.22, 0.0, 0.0)) - 0.37;
        float d2 = length(p - vec3(0.22, 0.0, 0.0)) - 0.37;
        return smin(d1, d2, 0.06);
    }
    if (t < 5.5) { vec3 q = abs(p) - vec3(0.4 - 0.14); return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - 0.14; }
    if (t < 6.5) { vec3 q = p; q.x -= clamp(q.x, -0.32, 0.32); return length(q) - 0.29; }
    vec3 q = p / vec3(0.62, 0.3, 0.62);
    return (length(q) - 1.0) * 0.3;
}

// ═════════════════════════════════════════════════════════════════════════
//  STACK
// ═════════════════════════════════════════════════════════════════════════
vec3  oC[MAXN];      // centres
vec2  oLean[MAXN];   // accumulated lean (x, z)
float oS[MAXN];      // squash
float oA[MAXN], oB[MAXN], oF[MAXN];   // shape a → b, morph f
float oSpin[MAXN];
float gN;
float gTop;
float oK[MAXN];      // per-object scale (top object grows/shrinks in with the dynamics)
int   gHitId;
float gHitBlend;

void buildStack() {
    float nEff = clamp(floor(clamp(objCount, 3.0, 6.0) + 0.5) + (gLift - 0.5) * 2.4, 2.6, 6.0);
    gN = ceil(nEff - 0.02);
    float punch = 1.0 + 0.22 * gDrop * gDrop + 0.08 * gPeak - 0.12 * gRest;
    float hop = st1(0.0).z;
    vec3 contact = vec3(0.0, hop, 0.0);
    vec2 lean = vec2(0.0);
    for (int i = 0; i < MAXN; i++) {
        float fi = float(i);
        vec4 s0 = st0(fi), s1 = st1(fi);
        float ph = TIME / shapePeriod + h11(fi * 3.7) * 0.999;
        float k = floor(ph);
        float sa = floor(h11(fi * 1.91 + k * 0.731) * 8.0);
        float sb = floor(h11(fi * 1.91 + (k + 1.0) * 0.731) * 8.0);
        if (i == 0) { sa = mod(sa, 2.0) < 0.5 ? 1.0 : 7.0; sb = mod(sb, 2.0) < 0.5 ? 1.0 : 7.0; }   // a steady base: ring or puck
        float f = smoothstep(0.7, 1.0, fract(ph));
        oA[i] = sa; oB[i] = sb; oF[i] = f;
        float sq = 1.0 + s0.x;
        oS[i] = sq;
        // the last object fades in by size (count follows the show) — no popping
        float grow = smoothstep(0.0, 1.0, clamp(nEff - fi, 0.0, 1.0));
        oK[i] = max(grow, 0.03) * punch;
        float hh = mix(shapeHalfH(sa), shapeHalfH(sb), f) * sq * objScale * oK[i];
        lean += vec2(s0.z, s1.x);
        oLean[i] = lean;
        vec3 ax = normalize(vec3(sin(lean.x), 1.0, sin(lean.y)));
        vec3 c = contact + ax * hh * 0.97;
        oC[i] = c;
        contact = c + ax * hh * 0.97;
        oSpin[i] = (TIME * spin + audioMidTime * 0.4 * spin + 5.0 * gBuild * gBuild * spin) * (0.25 + 0.4 * h11(fi * 5.1)) * (mod(fi, 2.0) * 2.0 - 1.0);
        if (fi > gN - 0.5 || grow < 0.03) oC[i] = vec3(0.0, -100.0, 0.0);
    }
    gTop = contact.y;
}

float objSDF(vec3 c, vec2 lean, float sq, float sa, float sb, float f, float spn, float fi, float ok, vec3 p) {
    vec3 q = p - c;
    // undo the lean (small-angle: rotate about z then x)
    q.xy = rot(lean.x) * q.xy;
    q.zy = rot(lean.y) * q.zy;
    vec3 sc = vec3(1.0 / sqrt(sq), sq, 1.0 / sqrt(sq)) * objScale * ok;
    q /= sc;
    // rubbery deform: twist + wobble
    float tw = deform * (0.6 * sin(TIME * 1.3 + fi * 2.0) * mix(0.5, 1.3, gLift) + 0.8 * midP() * audioReact + 0.9 * gBuild * gBuild) * q.y;
    q.xz = rot(tw + spn) * q.xz;
    q.xz *= 1.0 + deform * 0.06 * sin(q.y * 9.0 + TIME * 3.0 + fi);
    float d = mix(shapeSDF(sa, q), shapeSDF(sb, q), f);
    return d * min(min(sc.x, sc.y), sc.z) * 0.85;
}

float map(vec3 p) {
    float d = 1e5;
    gHitId = 0;
    for (int i = 0; i < MAXN; i++) {
        if (float(i) > gN - 0.5) break;
        float di = objSDF(oC[i], oLean[i], oS[i], oA[i], oB[i], oF[i], oSpin[i], float(i), oK[i], p);
        if (di < d) { gHitId = i; }
        d = smin(d, di, 0.025);
    }
    return d;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.0015, -0.0015);
    return normalize(e.xyy * map(p + e.xyy) + e.yyx * map(p + e.yyx) + e.yxy * map(p + e.yxy) + e.xxx * map(p + e.xxx));
}

// ═════════════════════════════════════════════════════════════════════════
//  MATERIALS
// ═════════════════════════════════════════════════════════════════════════
vec3 palC(float k) {
    k = mod(k, 6.0);
    if (k < 0.5) return c1.rgb;
    if (k < 1.5) return c2.rgb;
    if (k < 2.5) return c3.rgb;
    if (k < 3.5) return c4.rgb;
    if (k < 4.5) return c5.rgb;
    return c6.rgb;
}

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

// one material family evaluated on a hit
vec3 material(float m, float ia, float ib, vec3 lp, vec3 n, vec3 rd, vec3 L, vec2 uv) {
    vec3 A = palC(ia), B = palC(ib);
    float diff = clamp(dot(n, L), 0.0, 1.0);
    float wrap = clamp(dot(n, L) * 0.5 + 0.5, 0.0, 1.0);
    float fres = pow(1.0 - clamp(dot(n, -rd), 0.0, 1.0), 2.0);
    vec3 H = normalize(L - rd);
    float spec = pow(clamp(dot(n, H), 0.0, 1.0), 70.0);
    vec3 env = mix(backdrop(vec2(uv.x, 0.15)), backdrop(vec2(uv.x, 0.9)), n.y * 0.5 + 0.5);
    float ps = patScale;
    if (m < 0.5) {                          // glossy (black sphere in the reference)
        vec3 c = A * (0.15 + 0.75 * diff) + env * 0.18 * fres;
        return c + vec3(1.0) * spec * 0.9 * gloss + vec3(0.6) * pow(clamp(dot(n, H), 0.0, 1.0), 12.0) * 0.12 * gloss;
    }
    if (m < 1.5) {                          // glowing gradient (cyan blob)
        vec3 c = mix(A, B, smoothstep(-0.5, 0.5, lp.y)) * (0.75 + 0.35 * glowAmt);
        c *= 0.75 + 0.25 * wrap;
        return c + mix(B, vec3(1.0), 0.3) * fres * 0.35 * glowAmt;
    }
    if (m < 2.5) {                          // dark-rimmed matte (clover in the reference)
        vec3 c = mix(B, A, pow(clamp(dot(n, -rd), 0.0, 1.0), 1.5));
        c = mix(c, c6.rgb * 0.5 + B * 0.1, smoothstep(0.55, 1.0, fres) * 0.8);
        return c * (0.65 + 0.45 * wrap) + vec3(1.0) * spec * 0.25 * gloss;
    }
    if (m < 3.5) {                          // stripes that slide
        float s = smoothstep(-0.05, 0.05, sin(lp.y * 18.0 * ps + TIME * 2.0));
        vec3 c = mix(A, B, s) * (0.3 + 0.8 * wrap);
        return c + vec3(1.0) * spec * 0.5 * gloss;
    }
    if (m < 4.5) {                          // polka dots (on the sphere of directions)
        vec3 q = normalize(lp) * 4.0 * ps;
        vec3 cell = floor(q + 0.5);
        float dd = length(q - cell);
        float dot1 = smoothstep(0.34, 0.28, dd);
        vec3 c = mix(A, B, dot1) * (0.3 + 0.8 * wrap);
        return c + vec3(1.0) * spec * 0.5 * gloss;
    }
    if (m < 5.5) {                          // checks
        vec3 q = floor(lp * 5.0 * ps + vec3(0.0, TIME * 0.4, 0.0));
        float ch = mod(q.x + q.y + q.z, 2.0);
        vec3 c = mix(A, B, ch) * (0.3 + 0.8 * wrap);
        return c + vec3(1.0) * spec * 0.4 * gloss;
    }
    // iridescent
    float t = dot(n, -rd) * 1.8 + lp.y * 0.8 + TIME * 0.15;
    vec3 ir = 0.5 + 0.5 * cos(TAU * (t + vec3(0.0, 0.33, 0.67)));
    vec3 c = mix(A, ir, 0.65) * (0.35 + 0.75 * wrap);
    return c + vec3(1.0) * spec * 0.8 * gloss;
}

vec4 renderPass() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    buildStack();

    // camera frames the stack
    float H = max(gTop, 1.5);
    float orbit = camOrbit * (0.6 * sin(TIME * 0.11) + 0.2 * sin(TIME * 0.23));
    vec3 ta = vec3(0.0, H * 0.46, 0.0);
    vec3 ro = ta + vec3(sin(orbit) * 9.0, 1.6, cos(orbit) * 9.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    vec2 sp = (fc - 0.5 * R) / R.y;
    // rest: pulled back (lots of negative space) … build: push in … drop: punch
    float focal = 9.0 / (H * 1.1 + 0.6) * zoom * (mix(0.72, 1.0, smoothstep(0.1, 0.7, gLift)) + 0.07 * gPeak + 0.22 * gBuild * gBuild - 0.12 * gDrop);
    sp += 0.004 * gBuild * gBuild * gBuild * vec2(sin(TIME * 53.0), cos(TIME * 61.0));   // tremor
    vec3 rd = normalize(sp.x * uu + sp.y * vv + focal * ww);

    vec3 bg = backdrop(uv);
    // drop: a shockwave ring travels out from the base; peak: a soft halo behind the totem
    {
        vec2 sc = (fc - 0.5 * R) / R.y;
        vec2 bc = vec2(0.0, -0.32);
        float rr = length((sc - bc) * vec2(1.0, 1.6));
        float wave = (1.0 - gDrop) * 1.4;
        float ring = exp(-pow((rr - wave) * 10.0, 2.0)) * gDrop * smoothstep(0.0, 0.5, gDrop) * 1.4;
        vec3 acc = mix(c1.rgb, c4.rgb, 0.5 + 0.5 * sin(TIME * 0.3));
        bg += acc * ring * 0.9;
        float halo = exp(-dot(sc - vec2(0.0, 0.05), sc - vec2(0.0, 0.05)) * 3.0);
        bg = mix(bg, bg + acc * 0.35 * halo, gPeak * 0.7 + gBuild * 0.3);
        bg *= 1.0 - 0.18 * gRest * smoothstep(0.3, 0.9, length(sc));
    }
    vec3 col = bg;
    float alpha = 0.0;

    // floor contact shadow (floor itself invisible)
    float tf = -ro.y / rd.y;
    if (rd.y < 0.0 && shadow > 0.001) {
        vec3 fp = ro + rd * tf;
        float hop = st1(0.0).z;
        float r = length(fp.xz - oC[0].xz);
        float sh = exp(-r * r * 3.0) * shadow * 0.55 / (1.0 + hop * 2.0);
        col = mix(col, col * 0.35, sh);
        alpha = sh;
    }

    float t = 4.0;
    bool hit = false;
    for (int i = 0; i < 120; i++) {
        vec3 p = ro + rd * t;
        float d = map(p);
        if (d < 0.001) { hit = true; break; }
        t += d * 0.85;
        if (t > 20.0) break;
    }
    if (hit) {
        vec3 p = ro + rd * t;
        vec3 n = calcNormal(p);
        map(p);
        int id = gHitId;
        // local position on that object for patterns
        vec3 lp = vec3(0.0);
        float fi = 0.0;
        for (int i = 0; i < MAXN; i++) {
            if (i == id) {
                lp = p - oC[i];
                lp.xz = rot(oSpin[i]) * lp.xz;
                fi = float(i);
            }
        }
        // material + palette pick, morphing on each object's own clock
        float ph = TIME / matPeriod + h11(fi * 7.3) * 0.999;
        float k = floor(ph);
        float f = smoothstep(0.75, 1.0, fract(ph));
        float mA = floor(h11(fi * 2.3 + k * 0.913) * 7.0);
        float mB = floor(h11(fi * 2.3 + (k + 1.0) * 0.913) * 7.0);
        float pA1 = floor(h11(fi * 4.1 + k * 1.37) * 6.0), pA2 = pA1 + 1.0 + floor(h11(fi * 8.3 + k) * 4.0);
        float pB1 = floor(h11(fi * 4.1 + (k + 1.0) * 1.37) * 6.0), pB2 = pB1 + 1.0 + floor(h11(fi * 8.3 + k + 1.0) * 4.0);
        vec3 L = normalize(vec3(0.5, 0.75, 0.6));
        vec3 cA = material(mA, pA1, pA2, lp, n, rd, L, uv);
        vec3 cB = material(mB, pB1, pB2, lp, n, rd, L, uv);
        col = mix(cA, cB, f);
        // contact occlusion between neighbours
        float ao = clamp(map(p + n * 0.06) / 0.06, 0.0, 1.0);
        col *= 0.55 + 0.45 * ao;
        col += vec3(1.0) * highP() * ar * 0.25 * pow(clamp(dot(n, normalize(L - rd)), 0.0, 1.0), 40.0);
        col *= 1.0 + 0.15 * audioLevel * ar;
        // rest: softer light … build: a highlight shimmer whose rate climbs … peak: hotter
        float shim = 0.5 + 0.5 * sin(TIME * 9.0 + gBuild * gBuild * 70.0 + fi * 1.7);
        col += vec3(1.0) * gBuild * gBuild * shim * 0.35 * pow(clamp(dot(n, normalize(L - rd)), 0.0, 1.0), 20.0);
        col *= mix(0.85, 1.12, gLift) + 0.15 * gBuild;
        alpha = 1.0;
    }

    col = hueRot(col, hueShift);
    col = mix(vec3(lum(col)), col, saturation * (1.0 - 0.3 * gRest + 0.25 * gPeak + 0.2 * gBuild));
    // drop flash + chromatic fringe burst
    col = mix(col, vec3(1.0), 0.55 * gDrop * gDrop * gDrop);
    col += vec3(0.25, -0.05, 0.3) * gDrop * gDrop * (1.0 - alpha * 0.5);
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    return vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}

void main() {
    showDynamics();
    if (PASSINDEX == 0) gl_FragColor = physPass();
    else                gl_FragColor = renderPass();
}
