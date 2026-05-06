/*{
  "CATEGORIES": ["3D", "Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Dada in 3D — Hannah Höch / Marcel Duchamp / Tristan Tzara as a raymarched still life. Floating Dada objects (gears, bicycle wheels, arrows, extruded type, half-discs) drift on Lissajous orbits, slowly rotating each on its own axis, casting hard shadows on a paper-cream floor. Studio 3-point light. Bass kicks SLAM the camera in a sudden translation cut — Dada cut. Treble spins the fastest gear/wheel. Palette: bone, ink, brick, ochre, rare Klein blue. Saturated and PUNCHY, zero desaturation. Output LINEAR HDR with HDR peaks 1.6-2.5 on metal highlights for bloom.",
  "INPUTS": [
    { "NAME": "camDist",       "LABEL": "Camera Distance", "TYPE": "float", "MIN": 2.0, "MAX": 14.0, "DEFAULT": 6.0 },
    { "NAME": "camHeight",     "LABEL": "Camera Height",   "TYPE": "float", "MIN": -2.0, "MAX": 4.0, "DEFAULT": 1.4 },
    { "NAME": "camOrbitSpeed", "LABEL": "Orbit Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.18 },
    { "NAME": "camAzimuth",    "LABEL": "Camera Azimuth",  "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 0.0 },
    { "NAME": "keyAngle",      "LABEL": "Key Light Angle", "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 0.9 },
    { "NAME": "keyElevation",  "LABEL": "Key Elevation",   "TYPE": "float", "MIN": 0.0, "MAX": 1.5708, "DEFAULT": 0.85 },
    { "NAME": "keyColor",      "LABEL": "Key Light",       "TYPE": "color", "DEFAULT": [1.0, 0.95, 0.84, 1.0] },
    { "NAME": "fillColor",     "LABEL": "Fill Light",      "TYPE": "color", "DEFAULT": [0.55, 0.72, 1.0, 1.0] },
    { "NAME": "ambient",       "LABEL": "Ambient",         "TYPE": "float", "MIN": 0.0, "MAX": 0.5,  "DEFAULT": 0.10 },
    { "NAME": "rimStrength",   "LABEL": "Rim Strength",    "TYPE": "float", "MIN": 0.0, "MAX": 1.5,  "DEFAULT": 0.55 },
    { "NAME": "exposure",      "LABEL": "Exposure",        "TYPE": "float", "MIN": 0.3, "MAX": 3.0,  "DEFAULT": 1.05 },
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "driftSpeed",    "LABEL": "Drift Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.7 },
    { "NAME": "paletteShift",  "LABEL": "Palette Shift",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.3 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//   DADA 3D — Höch / Duchamp / Tzara as a raymarched studio still life.
//   Floating gears, wheels, arrows, extruded letters, half-discs.
//   Bass = sudden camera translation cut. Treble = fastest gear/wheel.
//   Output LINEAR HDR.
// ════════════════════════════════════════════════════════════════════════

#define MAX_STEPS 110
#define MAX_DIST  60.0
#define EPS       0.0009

#define NUM_OBJ   8

// ── Palette (saturated, no desaturation) ─────────────────────────────────
const vec3 BONE  = vec3(0.94, 0.90, 0.80);
const vec3 INK   = vec3(0.06, 0.05, 0.05);
const vec3 BRICK = vec3(0.78, 0.16, 0.12);
const vec3 OCHRE = vec3(0.96, 0.70, 0.18);
const vec3 KLEIN = vec3(0.04, 0.18, 0.86);

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
vec2  hash22(vec2 p)  {
    return fract(sin(vec2(dot(p, vec2(127.1, 311.7)),
                          dot(p, vec2(269.5,  183.3)))) * 43758.5453);
}

mat2 rot2(float a) { float c=cos(a),s=sin(a); return mat2(c,-s,s,c); }
mat3 rotY(float a) { float c=cos(a),s=sin(a); return mat3(c,0,-s, 0,1,0, s,0,c); }
mat3 rotX(float a) { float c=cos(a),s=sin(a); return mat3(1,0,0, 0,c,-s, 0,s,c); }
mat3 rotZ(float a) { float c=cos(a),s=sin(a); return mat3(c,-s,0, s,c,0, 0,0,1); }

// ── Audio ────────────────────────────────────────────────────────────────
struct Audio { float bass; float mid; float treb; };
Audio synthAudio(float ar, float t) {
    Audio a;
    float lvl = clamp(ar, 0.0, 2.0);
    a.bass = max(0.0, lvl * (0.55 + 0.45 * sin(t * 1.7)  * sin(t * 0.31)));
    a.mid  = max(0.0, lvl * (0.50 + 0.50 * sin(t * 0.83 + 1.4)));
    a.treb = max(0.0, lvl * (0.50 + 0.50 * sin(t * 4.1  + 0.7)));
    return a;
}

// ── Bass cut — sudden camera translation. Holds, then snaps to a new offset.
vec3 bassCutOffset(float t, float bass) {
    float epoch = floor(t * 0.45 + bass * 1.7);
    vec2 dir2   = hash22(vec2(epoch, 17.3)) * 2.0 - 1.0;
    float dirZ  = hash11(epoch * 5.13) * 2.0 - 1.0;
    float mag   = 0.7 + 1.4 * hash11(epoch * 3.7);
    float phase = fract(t * 0.45 + bass * 1.7);
    // "Cut" = near-instant snap, then hold (smoothstep with tiny window).
    float slide = smoothstep(0.0, 0.05, phase);
    return vec3(dir2.x, dirZ * 0.4, dir2.y) * mag * slide;
}

// ── Primitive SDFs ───────────────────────────────────────────────────────
float sdSphere(vec3 p, float r) { return length(p) - r; }
float sdBox(vec3 p, vec3 b)     { vec3 q = abs(p) - b; return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0); }
float sdRoundBox(vec3 p, vec3 b, float r) {
    vec3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - r;
}
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-5), 0.0, 1.0);
    return length(pa - ba * h) - r;
}
float sdCylinder(vec3 p, float h, float r) {
    vec2 d = vec2(length(p.xz) - r, abs(p.y) - h);
    return min(max(d.x,d.y), 0.0) + length(max(d, 0.0));
}
float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}
// Cone — apex at +y, base at y=0, half-height h, base radius r.
float sdCone(vec3 p, float h, float r) {
    vec2 q = vec2(length(p.xz), p.y);
    vec2 a = vec2(r, 0.0);
    vec2 b = vec2(0.0, h);
    vec2 e = b - a;
    vec2 w = q - a;
    float f = clamp(dot(w, e) / dot(e, e), 0.0, 1.0);
    vec2 proj = w - e * f;
    float d = length(proj);
    float s = (w.x * e.y - w.y * e.x) > 0.0 ? 1.0 : -1.0;
    // Cap distance from base plane y < 0
    float cap = -p.y;
    return max(d * s, cap);
}

// ── Compound 3D Dada objects ─────────────────────────────────────────────

// 3D Gear: cylinder + radial teeth (modulated cylinder).
float sdGear3D(vec3 p, float R, float thick, float toothH, float teeth, float rot) {
    p = rotY(rot) * p;
    float ang = atan(p.z, p.x);
    float tooth = 0.5 + 0.5 * cos(ang * teeth);
    float outerR = R + tooth * toothH;
    float radial = length(p.xz) - outerR;
    float axial  = abs(p.y) - thick;
    float body   = max(radial, axial);
    // Inner hub hole
    float hub = -(length(p.xz) - R * 0.18);
    body = max(body, hub);
    return body;
}

// 3D Bicycle wheel: torus (rim) + spokes (capsules) in the same plane (xz).
float sdWheel3D(vec3 p, float R, float rimR, float spokes, float spokeR, float rot) {
    p = rotY(rot) * p;
    float rim = sdTorus(p, R, rimR);
    // Fold by rotational symmetry: angle modulo 2pi/N.
    float k = 6.2831853 / spokes;
    float ang = atan(p.z, p.x);
    float a = mod(ang + k * 0.5, k) - k * 0.5;
    float rad = length(p.xz);
    vec3 pf = vec3(cos(a) * rad, p.y, sin(a) * rad);
    // Spoke as a capsule along +x from origin to rim.
    float spoke = sdCapsule(pf, vec3(0.0, 0.0, 0.0), vec3(R, 0.0, 0.0), spokeR);
    // Hub
    float hub = sdCylinder(p, rimR * 1.3, R * 0.10);
    return min(min(rim, spoke), hub);
}

// 3D Arrow: capsule shaft + cone tip pointing +x.
float sdArrow3D(vec3 p, float L, float r) {
    float shaft = sdCapsule(p, vec3(-L * 0.6, 0.0, 0.0), vec3(L * 0.45, 0.0, 0.0), r);
    // Cone built around +y; rotate so apex points +x.
    vec3 cp = p - vec3(L * 0.45, 0.0, 0.0);
    cp = rotZ(-1.5707963) * cp;   // +y -> +x
    float tip = sdCone(cp, L * 0.35, r * 2.6);
    return min(shaft, tip);
}

// 3D Number/letter primitive: extruded box with a notch — a stylized "7"-like glyph.
float sdGlyph3D(vec3 p, vec3 b, float r) {
    float bar = sdRoundBox(p - vec3(0.0, b.y * 0.7, 0.0), vec3(b.x, b.y * 0.25, b.z), r);
    // Diagonal stem
    vec3 sp = p;
    sp.xy = rot2(-0.55) * sp.xy;
    float stem = sdRoundBox(sp, vec3(b.x * 0.22, b.y * 1.05, b.z), r);
    return min(bar, stem);
}

// 3D Half-disc: thick cylinder sliced by a half-plane.
float sdHalfDisc3D(vec3 p, float R, float thick, float cut) {
    float c = sdCylinder(p, thick, R);
    // Cut plane: rotate around y by 'cut', keep upper half along the rotated x.
    vec3 q = rotY(cut) * p;
    float plane = -q.x;     // remove material where q.x < 0
    return max(c, plane);
}

// ── Object table ─────────────────────────────────────────────────────────
// Each object: id 0..4 selects shape; seed feeds palette + axis; orbit is Lissajous.
struct Hit { float d; float mat; vec3 albedo; float metal; };

Hit unionHit(Hit a, Hit b) { return (a.d < b.d) ? a : b; }

vec3 objColor(int id, float seed, float pal) {
    float r = hash11(seed * 13.7 + pal * 4.31 + float(id) * 1.91);
    if (r < 0.26) return INK;
    if (r < 0.50) return BRICK;
    if (r < 0.74) return OCHRE;
    if (r < 0.80) return KLEIN;        // rare zing
    return BONE;
}

// Object center position from index — Lissajous orbit.
vec3 objCenter(int i, float t) {
    float fi = float(i);
    float seed = fi * 1.731 + 0.5;
    vec2 base = hash22(vec2(seed, seed * 2.3)) * 2.0 - 1.0;
    vec3 c;
    float spd = 0.45 + hash11(seed * 1.7) * 0.6;
    c.x = base.x * 1.9 + 1.4 * sin(t * spd * 0.27 + fi * 0.91);
    c.y = 1.4 + base.y * 0.9 + 0.8 * sin(t * spd * 0.41 + fi * 1.31);
    c.z = (hash11(seed * 7.7) * 2.0 - 1.0) * 1.7 + 1.2 * cos(t * spd * 0.31 + fi * 1.13);
    return c;
}

// Per-object axis of rotation (constant) and rate.
void objAxis(int i, out vec3 axis, out float rate) {
    float fi = float(i);
    float seed = fi * 1.731 + 0.5;
    vec3 a = vec3(hash11(seed * 2.1), hash11(seed * 3.7), hash11(seed * 5.9)) * 2.0 - 1.0;
    axis = normalize(a + vec3(0.001));
    rate = (hash11(seed * 9.1) > 0.5 ? 1.0 : -1.0) * (0.4 + hash11(seed * 11.1) * 0.7);
}

// Rodrigues axis-angle rotation.
vec3 rotAxis(vec3 p, vec3 axis, float ang) {
    float c = cos(ang), s = sin(ang);
    return p * c + cross(axis, p) * s + axis * dot(axis, p) * (1.0 - c);
}

// Evaluate one object's SDF + material given world point.
Hit objectHit(int i, vec3 p, float t, float treb, float pal) {
    float fi = float(i);
    float seed = fi * 1.731 + 0.5;
    int kind = int(mod(fi, 5.0));   // round-robin: 0 gear, 1 wheel, 2 arrow, 3 glyph, 4 halfdisc

    vec3 ctr = objCenter(i, t);
    vec3 axis; float rate;
    objAxis(i, axis, rate);

    // Trebble spins fastest gear/wheel — pick objects 0 and 1 to be the "fastest".
    float trebBoost = (i == 0 || i == 1) ? (1.0 + treb * 3.5) : 1.0;
    float ang = t * rate * trebBoost;

    vec3 lp = p - ctr;
    lp = rotAxis(lp, axis, ang);

    float d;
    vec3 col;
    float metal = 0.0;
    if (kind == 0) {
        d = sdGear3D(lp, 0.55, 0.10, 0.10, 10.0 + floor(hash11(seed * 5.7) * 6.0), 0.0);
        col = objColor(0, seed, pal);
        metal = 0.85;
    } else if (kind == 1) {
        d = sdWheel3D(lp, 0.62, 0.04, 8.0 + floor(hash11(seed * 9.1) * 4.0), 0.018, 0.0);
        col = objColor(1, seed, pal);
        metal = 0.7;
    } else if (kind == 2) {
        d = sdArrow3D(lp, 1.05, 0.07);
        col = objColor(2, seed, pal);
        metal = 0.2;
    } else if (kind == 3) {
        d = sdGlyph3D(lp, vec3(0.42, 0.42, 0.10), 0.03);
        col = objColor(3, seed, pal);
        metal = 0.05;
    } else {
        d = sdHalfDisc3D(lp, 0.55, 0.08, t * 0.4 + seed * 6.28);
        col = objColor(4, seed, pal);
        metal = 0.15;
    }

    Hit h;
    h.d = d;
    h.mat = 1.0;
    h.albedo = col;
    h.metal = metal;
    return h;
}

// ── Scene ────────────────────────────────────────────────────────────────
Hit scene(vec3 p, float t, float treb, float pal) {
    // Paper-cream floor at y = 0.
    Hit floorH;
    floorH.d = p.y;
    floorH.mat = 0.0;
    floorH.albedo = BONE * 0.96;
    floorH.metal = 0.0;

    Hit h = floorH;
    for (int i = 0; i < NUM_OBJ; i++) {
        h = unionHit(h, objectHit(i, p, t, treb, pal));
    }
    return h;
}

float sceneDist(vec3 p, float t, float treb, float pal) {
    return scene(p, t, treb, pal).d;
}

vec3 sceneNormal(vec3 p, float t, float treb, float pal) {
    vec2 e = vec2(0.0015, 0.0);
    return normalize(vec3(
        sceneDist(p + e.xyy, t, treb, pal) - sceneDist(p - e.xyy, t, treb, pal),
        sceneDist(p + e.yxy, t, treb, pal) - sceneDist(p - e.yxy, t, treb, pal),
        sceneDist(p + e.yyx, t, treb, pal) - sceneDist(p - e.yyx, t, treb, pal)));
}

// ── Raymarch ─────────────────────────────────────────────────────────────
Hit march(vec3 ro, vec3 rd, float t, float treb, float pal, out float tHit, out int steps) {
    float tt = 0.0;
    Hit last;
    last.d = 1e9; last.mat = 0.0; last.albedo = vec3(0.0); last.metal = 0.0;
    for (int i = 0; i < MAX_STEPS; i++) {
        steps = i;
        vec3 p = ro + rd * tt;
        Hit h = scene(p, t, treb, pal);
        if (h.d < EPS) { tHit = tt; return h; }
        tt += max(h.d, EPS);
        if (tt > MAX_DIST) break;
        last = h;
    }
    tHit = MAX_DIST;
    last.mat = -1.0;
    return last;
}

// Hard shadow march.
float shadow(vec3 ro, vec3 rd, float t, float treb, float pal) {
    float tt = 0.02;
    for (int i = 0; i < 48; i++) {
        vec3 p = ro + rd * tt;
        float d = sceneDist(p, t, treb, pal);
        if (d < 0.0008) return 0.0;
        tt += max(d, 0.01);
        if (tt > 18.0) break;
    }
    return 1.0;
}

// ── Lighting ─────────────────────────────────────────────────────────────
vec3 shade(vec3 p, vec3 n, vec3 rd, vec3 albedo, float metal,
           vec3 keyDir, vec3 keyCol, vec3 fillCol, float ambientV, float rim,
           float t, float treb, float pal) {
    // Key
    float kd = max(dot(n, keyDir), 0.0);
    float sh = shadow(p + n * 0.002, keyDir, t, treb, pal);
    vec3 keyL = keyCol * kd * sh;

    // Fill from above-side opposite the key
    vec3 fillDir = normalize(vec3(-keyDir.x * 0.6, 0.4, -keyDir.z * 0.4));
    float fd = max(dot(n, fillDir), 0.0);
    vec3 fillL = fillCol * fd * 0.55;

    // Rim
    float rimT = pow(1.0 - max(dot(n, -rd), 0.0), 2.5);
    vec3 rimL = vec3(1.0, 0.96, 0.92) * rimT * rim;

    // Ambient
    vec3 ambL = mix(BONE * 0.8, vec3(0.55,0.65,0.85), 0.4) * ambientV;

    // Diffuse base
    vec3 diffuse = albedo * (keyL + fillL + ambL) + albedo * rimL * 0.4;

    // Specular (Blinn-Phong) — HDR peaks for metals (1.6-2.5)
    vec3 h = normalize(keyDir - rd);
    float spec = pow(max(dot(n, h), 0.0), mix(28.0, 96.0, metal));
    float specPeak = mix(1.6, 2.5, metal);
    vec3 specCol = mix(vec3(1.0), albedo, metal);
    vec3 specL = specCol * spec * specPeak * sh;

    return diffuse + specL;
}

// ── Background ───────────────────────────────────────────────────────────
vec3 background(vec3 rd) {
    float h = clamp(rd.y * 0.5 + 0.5, 0.0, 1.0);
    vec3 sky = mix(BONE * 0.92, BONE * 1.05, h);
    return sky;
}

// ── Main ─────────────────────────────────────────────────────────────────
void main() {
    vec2 res = RENDERSIZE.xy;
    vec2 uv = (gl_FragCoord.xy - 0.5 * res) / res.y;

    float t = TIME * driftSpeed;
    Audio A = synthAudio(audioReact, TIME);

    // Camera
    float az = camAzimuth + TIME * camOrbitSpeed;
    vec3 target = vec3(0.0, 1.4, 0.0);
    vec3 ro = vec3(sin(az) * camDist, camHeight + 1.4, cos(az) * camDist);

    // BASS CUT: snap the entire camera (and target) by a translation.
    vec3 cut = bassCutOffset(TIME, A.bass);
    ro     += cut;
    target += cut * 0.6;

    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    float fov = 1.25;
    vec3 rd = normalize(fwd + right * uv.x * fov + up * uv.y * fov);

    // Light
    vec3 keyDir = normalize(vec3(cos(keyAngle) * cos(keyElevation),
                                 sin(keyElevation),
                                 sin(keyAngle) * cos(keyElevation)));

    // March
    float tHit; int steps;
    Hit h = march(ro, rd, t, A.treb, paletteShift, tHit, steps);

    vec3 col;
    if (h.mat < 0.0) {
        col = background(rd);
    } else {
        vec3 p = ro + rd * tHit;
        vec3 n = sceneNormal(p, t, A.treb, paletteShift);
        col = shade(p, n, rd, h.albedo, h.metal,
                    keyDir, keyColor.rgb, fillColor.rgb,
                    ambient, rimStrength,
                    t, A.treb, paletteShift);

        // Distance fog into bone
        float fog = 1.0 - exp(-tHit * 0.025);
        col = mix(col, BONE * 1.0, fog * 0.25);
    }

    // Bass kick contrast bump (subtle, no flash)
    float kick = smoothstep(0.85, 1.15, A.bass);
    col = mix(col, col * 1.10, kick * 0.20);

    // Exposure (linear)
    col *= exposure;

    // Output LINEAR HDR
    gl_FragColor = vec4(col, 1.0);
}
