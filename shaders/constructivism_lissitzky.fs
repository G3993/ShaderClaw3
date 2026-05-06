/*{
  "CATEGORIES": ["3D", "Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "El Lissitzky — Proun Room (3D). Raymarched architectural space: pure red triangular prism (the wedge) menacing a white sphere, with a black cube on edge and a thin black slab plane drifting in restrained orbits inside a bright industrial cyc. Hard cool fluorescent overhead light, sharp floor shadows. Bass triggers a periodic SLAM where the wedge advances 30% toward the sphere then retreats. LINEAR HDR.",
  "INPUTS": [
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "slamCadence",   "LABEL": "Slam Cadence",    "TYPE": "float", "MIN": 3.0, "MAX": 14.0, "DEFAULT": 6.5 },
    { "NAME": "driftSpeed",    "LABEL": "Drift Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "camDist",       "LABEL": "Camera Distance", "TYPE": "float", "MIN": 2.0, "MAX": 9.0,  "DEFAULT": 4.6 },
    { "NAME": "camHeight",     "LABEL": "Camera Height",   "TYPE": "float", "MIN": -1.0, "MAX": 3.0, "DEFAULT": 1.1 },
    { "NAME": "camOrbitSpeed", "LABEL": "Orbit Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.06 },
    { "NAME": "camAzimuth",    "LABEL": "Camera Azimuth",  "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 0.55 },
    { "NAME": "keyAngle",      "LABEL": "Key Light Angle", "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 1.05 },
    { "NAME": "keyElevation",  "LABEL": "Key Elevation",   "TYPE": "float", "MIN": 0.2, "MAX": 1.5,  "DEFAULT": 1.15 },
    { "NAME": "exposure",      "LABEL": "Exposure",        "TYPE": "float", "MIN": 0.4, "MAX": 2.4,  "DEFAULT": 1.0 }
  ]
}*/

// El Lissitzky — Proun Room (3D). Red prism, white sphere, black cube on
// edge, thin black slab — drifting in a cool-grey cyc. Bass SLAM advances
// the wedge 30% toward the sphere then retreats. Linear HDR.

#define MAX_STEPS 88
#define MAX_DIST  40.0
#define EPS       0.0009

#define MAT_CYC    0
#define MAT_WEDGE  1
#define MAT_SPHERE 2
#define MAT_CUBE   3
#define MAT_SLAB   4

const float PI = 3.14159265;

// Pure Lissitzky palette — HDR linear.
const vec3 LZ_RED   = vec3(2.00, 0.20, 0.20);
const vec3 LZ_WHITE = vec3(1.00, 1.00, 1.00);
const vec3 LZ_BLACK = vec3(0.04, 0.04, 0.04);
const vec3 LZ_CYC   = vec3(0.92, 0.94, 0.96);

// Charcoal face shading for "black" solids — lit vs shadow side.
const vec3 LZ_BLACK_LIT    = vec3(0.30, 0.30, 0.32);
const vec3 LZ_BLACK_SHADOW = vec3(0.04, 0.04, 0.06);
const vec3 LZ_CYC_LO = vec3(0.86, 0.88, 0.92);
const vec3 LZ_CYC_HI = vec3(0.92, 0.94, 0.96);

// ── SDF library ─────────────────────────────────────────────────────────
float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Triangular prism — isoceles triangle (tip at origin, opening along +x to
// x=h with half-height w), extruded along z by ±d.
float sdTriPrism(vec3 p, float h, float w, float d) {
    vec2 q = p.xy;
    vec2 n1 = normalize(vec2(w, -h));
    vec2 n2 = normalize(vec2(w,  h));
    float tri = max(max(dot(q, n1), dot(q, n2)), q.x - h);
    vec2 w2 = vec2(tri, abs(p.z) - d);
    return min(max(w2.x, w2.y), 0.0) + length(max(w2, 0.0));
}

mat3 rotY(float a) { float c=cos(a), s=sin(a); return mat3(c,0.0,-s, 0.0,1.0,0.0, s,0.0,c); }
mat3 rotX(float a) { float c=cos(a), s=sin(a); return mat3(1.0,0.0,0.0, 0.0,c,-s, 0.0,s,c); }
mat3 rotZ(float a) { float c=cos(a), s=sin(a); return mat3(c,-s,0.0, s,c,0.0, 0.0,0.0,1.0); }

// Cyc — flat floor at y=0 curving up into vertical backdrop via quarter arc.
float sdCyc(vec3 p) {
    float seamZ = -1.6, seamY = 1.8, r = 1.6;
    if (p.z > seamZ && p.y < seamY) return p.y;
    if (p.y > seamY && p.z < seamZ) return -(p.z - (seamZ - r));
    return length(vec2(p.y - seamY, p.z - seamZ)) - r;
}

// ── Slam envelope ───────────────────────────────────────────────────────
// Returns vec3(impact, advance01, phase01)
vec3 slamPhase(float t, float cadence, float audioBoost) {
    float c = max(cadence - audioBoost * 1.6, 3.0);
    float ph = fract(t / c);
    float adv, impact;
    if (ph < 0.07) {
        float k = ph / 0.07; k = k*k*(3.0-2.0*k);
        adv = k; impact = k;
    } else if (ph < 0.22) {
        adv = 1.0; impact = 1.0;
    } else if (ph < 0.50) {
        float k = (ph - 0.22) / 0.28; k = k*k*(3.0-2.0*k);
        adv = 1.0 - k; impact = 1.0 - k;
    } else {
        adv = 0.0; impact = 0.0;
    }
    return vec3(impact, adv, ph);
}

// ── Scene parameters ────────────────────────────────────────────────────
struct Scene {
    vec3 spherePos;  float sphereR;
    vec3 wedgePos;   mat3 wedgeRot;  vec3 wedgeSize;
    vec3 cubePos;    mat3 cubeRot;   float cubeH;
    vec3 slabPos;    mat3 slabRot;   vec3 slabHalf;
    float impact;
};

Scene buildScene() {
    float t = TIME;
    float drift = clamp(driftSpeed, 0.0, 2.0);
    float bass  = clamp(audioBass, 0.0, 1.0) * audioReact;

    vec3 spherePos = vec3(
        0.05 + 0.10 * sin(t * 0.18 * drift),
        0.95 + 0.06 * cos(t * 0.22 * drift),
        0.05 + 0.07 * sin(t * 0.13 * drift + 1.3));
    float sphereR = 0.65;

    // Wedge orbits sphere, tip aimed at sphere, SLAM advances 30%
    vec3 sp = slamPhase(t, slamCadence, bass);
    float dist = 1.95 * (1.0 - sp.y * 0.30);
    float yaw  = t * 0.13 * drift;
    float pit  = -0.18 + 0.10 * sin(t * 0.07 * drift);
    vec3 toBase = vec3(cos(yaw)*cos(pit), sin(pit), sin(yaw)*cos(pit));
    vec3 wedgeBase = spherePos + toBase * dist;
    vec3 tipDir = -toBase;

    vec3 wx = normalize(tipDir);
    vec3 wy = normalize(cross(vec3(0.0, 1.0, 0.0), wx));
    if (length(wy) < 1e-3) wy = vec3(0.0, 0.0, 1.0);
    vec3 wz = normalize(cross(wx, wy));
    mat3 wedgeRot = mat3(wx.x, wy.x, wz.x,
                         wx.y, wy.y, wz.y,
                         wx.z, wy.z, wz.z);
    vec3 wedgeSize = vec3(1.55, 0.55, 0.22);
    vec3 wedgePos  = wedgeBase + tipDir * wedgeSize.x;

    vec3 cubePos = vec3(
        -1.85 + 0.05 * sin(t * 0.11 * drift),
         0.55 + 0.04 * cos(t * 0.15 * drift),
        -0.40 + 0.06 * sin(t * 0.09 * drift + 0.7));
    mat3 cubeRot = rotY(0.18 + t * 0.04 * drift) * rotZ(PI * 0.25) * rotX(0.05);

    vec3 slabPos = vec3(
         1.70 + 0.06 * sin(t * 0.10 * drift + 2.1),
         1.30 + 0.05 * cos(t * 0.13 * drift),
        -0.55 + 0.05 * sin(t * 0.08 * drift));
    mat3 slabRot = rotZ(0.22) * rotY(-0.55 + t * 0.03 * drift) * rotX(0.08);

    Scene s;
    s.spherePos = spherePos; s.sphereR = sphereR;
    s.wedgePos  = wedgePos;  s.wedgeRot = wedgeRot; s.wedgeSize = wedgeSize;
    s.cubePos   = cubePos;   s.cubeRot  = cubeRot;  s.cubeH = 0.42;
    s.slabPos   = slabPos;   s.slabRot  = slabRot;  s.slabHalf = vec3(1.05, 0.62, 0.018);
    s.impact    = sp.x;
    return s;
}

// ── Scene SDF ───────────────────────────────────────────────────────────
struct Hit { float d; int mat; };

Hit map(vec3 p, Scene s) {
    Hit best = Hit(1e9, MAT_CYC);

    float d = sdCyc(p);
    if (d < best.d) { best.d = d; best.mat = MAT_CYC; }

    d = sdSphere(p - s.spherePos, s.sphereR);
    if (d < best.d) { best.d = d; best.mat = MAT_SPHERE; }

    {
        vec3 lp = s.wedgeRot * (p - s.wedgePos);
        d = sdTriPrism(lp, s.wedgeSize.x, s.wedgeSize.y, s.wedgeSize.z);
        if (d < best.d) { best.d = d; best.mat = MAT_WEDGE; }
    }

    {
        vec3 lp = s.cubeRot * (p - s.cubePos);
        d = sdBox(lp, vec3(s.cubeH));
        if (d < best.d) { best.d = d; best.mat = MAT_CUBE; }
    }

    {
        vec3 lp = s.slabRot * (p - s.slabPos);
        d = sdBox(lp, s.slabHalf);
        if (d < best.d) { best.d = d; best.mat = MAT_SLAB; }
    }

    return best;
}

vec3 calcNormal(vec3 p, Scene s) {
    const vec2 e = vec2(EPS, 0.0);
    return normalize(vec3(
        map(p + e.xyy, s).d - map(p - e.xyy, s).d,
        map(p + e.yxy, s).d - map(p - e.yxy, s).d,
        map(p + e.yyx, s).d - map(p - e.yyx, s).d
    ));
}

// Hard architectural shadow — high step count, no banding.
float keyShadow(vec3 ro, vec3 rd, Scene s) {
    float res = 1.0, t = 0.04;
    for (int i = 0; i < 48; i++) {
        if (t > 8.0) break;
        float h = map(ro + rd * t, s).d;
        if (h < 0.0008) return 0.0;
        res = min(res, 18.0 * h / t);
        t  += clamp(h, 0.020, 0.35);
    }
    return clamp(res, 0.0, 1.0);
}

// Analytic floor shadow: project each scene object straight down (along key
// light direction) onto y=0 and accumulate soft elliptical falloffs. This
// replaces the raymarched floor shadow which was producing horizon banding.
float floorShadow(vec3 p, vec3 keyDir, Scene s) {
    // Project a world point onto y=0 along -keyDir to get caster footprint.
    // For each caster, compute distance from the projected center to p.xz.
    float occ = 0.0;
    float invKy = 1.0 / max(keyDir.y, 0.15);

    // Sphere shadow
    {
        vec2 c = (s.spherePos.xz - keyDir.xz * (s.spherePos.y * invKy));
        float d = length(p.xz - c);
        float r = s.sphereR * 1.20;
        occ = max(occ, smoothstep(r, r * 0.45, d) * 0.78);
    }
    // Wedge shadow (use bounding sphere of half-extent)
    {
        vec2 c = (s.wedgePos.xz - keyDir.xz * (s.wedgePos.y * invKy));
        float d = length(p.xz - c);
        float r = max(s.wedgeSize.x, s.wedgeSize.y) * 1.10;
        occ = max(occ, smoothstep(r, r * 0.40, d) * 0.85);
    }
    // Cube shadow
    {
        vec2 c = (s.cubePos.xz - keyDir.xz * (s.cubePos.y * invKy));
        float d = length(p.xz - c);
        float r = s.cubeH * 1.55;
        occ = max(occ, smoothstep(r, r * 0.40, d) * 0.88);
    }
    // Slab shadow
    {
        vec2 c = (s.slabPos.xz - keyDir.xz * (s.slabPos.y * invKy));
        float d = length(p.xz - c);
        float r = max(s.slabHalf.x, s.slabHalf.y) * 1.10;
        occ = max(occ, smoothstep(r, r * 0.35, d) * 0.80);
    }
    return clamp(1.0 - occ, 0.0, 1.0);
}

float ao(vec3 p, vec3 n, Scene s) {
    float occ = 0.0, sca = 1.0;
    for (int i = 0; i < 5; i++) {
        float h = 0.012 + 0.09 * float(i);
        occ += (h - map(p + n * h, s).d) * sca;
        sca *= 0.92;
    }
    return clamp(1.0 - 1.6 * occ, 0.0, 1.0);
}

vec3 sphDir(float a, float e) { return normalize(vec3(cos(a)*cos(e), sin(e), sin(a)*cos(e))); }

// ── Shading ─────────────────────────────────────────────────────────────
vec3 shadeMat(int mat, vec3 p, vec3 n, vec3 v, Scene s) {
    vec3 keyDir  = sphDir(keyAngle, keyElevation);
    vec3 keyCol  = vec3(0.96, 0.99, 1.05);     // cool fluorescent
    vec3 fillCol = vec3(0.90, 0.93, 0.96);     // bounce/fill

    float ndl = max(dot(n, keyDir), 0.0);
    float sh  = keyShadow(p + n * 0.005, keyDir, s);
    float fillTerm = max(dot(n, vec3(0.0, 1.0, 0.0)), 0.0);
    float occ = mix(0.65, 1.0, ao(p, n, s));

    if (mat == MAT_CYC) {
        // Clean cool-grey cyc gradient — no raymarched soft shadow (was banding).
        // Gradient goes lighter near foreground, slightly darker upward into the curve.
        float h = clamp((p.y + 0.4) / 2.4, 0.0, 1.0);
        vec3 base = mix(LZ_CYC_HI, LZ_CYC_LO, h);
        // Analytic projected shadows from objects (kills horizon banding).
        float fs = floorShadow(p, keyDir, s);
        vec3 lit = base * (0.85 + 0.15 * fillTerm) * mix(0.55, 1.0, fs);
        return lit * occ;
    }

    if (mat == MAT_WEDGE) {
        vec3 albedo = LZ_RED * (1.0 + 0.35 * s.impact);
        vec3 H = normalize(keyDir + v);
        float spec = pow(max(dot(n, H), 0.0), 36.0) * sh * 0.35;
        vec3 col = albedo * (0.18 + 0.95 * ndl * sh) + albedo * 0.12 * fillTerm;
        col += keyCol * spec;
        return col * occ;
    }

    if (mat == MAT_SPHERE) {
        vec3 albedo = LZ_WHITE;
        vec3 H = normalize(keyDir + v);
        float spec = pow(max(dot(n, H), 0.0), 64.0) * sh * 0.18;
        vec3 col = albedo * (0.32 + 0.85 * ndl * sh) + albedo * fillCol * 0.22 * fillTerm;
        col += vec3(spec);
        // Subtle red bounce on slam
        col += LZ_RED * 0.04 * s.impact * max(dot(normalize(s.wedgePos - p), n), 0.0);
        return col * occ;
    }

    if (mat == MAT_CUBE) {
        // Charcoal-gray faces: lit side vs shadow side via dot(n, lightDir).
        float litTerm = ndl * sh;
        vec3 face = mix(LZ_BLACK_SHADOW, LZ_BLACK_LIT, litTerm);
        // Subtle fill bounce so silhouette doesn't disappear.
        face += fillCol * LZ_BLACK_LIT * 0.18 * fillTerm;
        vec3 H = normalize(keyDir + v);
        float spec = pow(max(dot(n, H), 0.0), 80.0) * sh * 0.25;
        float rim = pow(1.0 - max(dot(n, v), 0.0), 4.0);
        return (face + vec3(spec) * 0.45 + fillCol * 0.06 * rim) * occ;
    }

    // SLAB — same charcoal face shading, slightly more matte.
    float litTerm = ndl * sh;
    vec3 face = mix(LZ_BLACK_SHADOW, LZ_BLACK_LIT, litTerm);
    face += fillCol * LZ_BLACK_LIT * 0.20 * fillTerm;
    float rim = pow(1.0 - max(dot(n, v), 0.0), 3.0);
    return (face + fillCol * 0.05 * rim) * occ;
}

vec3 background(vec3 rd) {
    float h = clamp(0.5 + 0.5 * rd.y, 0.0, 1.0);
    return mix(LZ_CYC * 0.97, LZ_CYC * 1.04, h);
}

vec4 traceScene(vec3 ro, vec3 rd, Scene s) {
    float dist = 0.0;
    int mat = MAT_CYC;
    bool hit = false;
    for (int i = 0; i < MAX_STEPS; i++) {
        Hit h = map(ro + rd * dist, s);
        if (h.d < EPS) { hit = true; mat = h.mat; break; }
        dist += h.d * 0.92;
        if (dist > MAX_DIST) break;
    }
    if (!hit) return vec4(background(rd), MAX_DIST);
    vec3 p = ro + rd * dist;
    vec3 n = calcNormal(p, s);
    return vec4(shadeMat(mat, p, n, -rd, s), dist);
}

// ── Main ────────────────────────────────────────────────────────────────
void main() {
    vec2 res = RENDERSIZE.xy;
    vec2 fc  = (gl_FragCoord.xy - 0.5 * res) / res.y;

    Scene s = buildScene();

    float mid = clamp(audioMid, 0.0, 1.0) * audioReact;
    float orb = camAzimuth + TIME * camOrbitSpeed * (1.0 + 0.4 * mid);
    vec3 ro = vec3(cos(orb) * camDist, camHeight, sin(orb) * camDist);
    vec3 ta = vec3(0.05, 0.85, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 rgt = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up  = cross(rgt, fwd);

    vec3 rd = normalize(fwd + rgt * fc.x + up * fc.y);

    vec4 sc = traceScene(ro, rd, s);
    vec3 col = sc.rgb;

    // Soft architectural vignette
    vec2 q = (gl_FragCoord.xy / res) - 0.5;
    col *= clamp(1.0 - dot(q, q) * 0.35, 0.0, 1.0);

    col *= exposure;

    gl_FragColor = vec4(col, 1.0);
}
