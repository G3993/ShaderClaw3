/*{
  "DESCRIPTION": "Event Horizon — a Schwarzschild black hole rendered by integrating every light ray along its curved geodesic: light bends around the photon sphere, the back of the accretion disk is lifted into a halo over the top and under the bottom of the shadow, the starfield folds into an Einstein ring and a razor-thin photon ring hugs the darkness. The disk is a real volume of turbulent plasma in Keplerian rotation — inner rings whip round faster than the outer ones, stretching hot filaments into spiral streaks — coloured by a blackbody temperature profile, Doppler-beamed (the approaching side blazes blue-white, the receding side sinks to deep ember red) and gravitationally redshifted near the horizon. Faint relativistic jets lance out of the poles. Anamorphic bloom, ACES and the HD finish. Bass feeds the accretion rate, beats fire magnetic flares that orbit with the gas, mids churn the turbulence, highs sparkle hot clumps.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #7",
  "CATEGORIES": ["Generator", "3D", "Space", "Science", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Disk Speed",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Speed" },
    { "NAME": "orbitSpeed",  "LABEL": "Camera Orbit",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.4,  "GROUP": "Speed" },
    { "NAME": "drift",       "LABEL": "Camera Drift",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.5,  "GROUP": "Speed" },
    { "NAME": "inclination", "LABEL": "Inclination (deg)",  "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 80.0, "DEFAULT": 9.0,  "GROUP": "Black Hole" },
    { "NAME": "camDist",     "LABEL": "Distance",           "TYPE": "float", "AUDIO": false, "MIN": 10.0, "MAX": 45.0, "DEFAULT": 25.0, "GROUP": "Black Hole" },
    { "NAME": "fov",         "LABEL": "Field of View",      "TYPE": "float", "AUDIO": false, "MIN": 0.3,  "MAX": 1.6,  "DEFAULT": 0.78, "GROUP": "Black Hole" },
    { "NAME": "roll",        "LABEL": "Roll",               "TYPE": "float", "AUDIO": false, "MIN": -0.6, "MAX": 0.6,  "DEFAULT": -0.08, "GROUP": "Black Hole" },
    { "NAME": "lensing",     "LABEL": "Lensing",            "TYPE": "float", "AUDIO": false, "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Black Hole" },
    { "NAME": "diskInner",   "LABEL": "Disk Inner (ISCO)",  "TYPE": "float", "AUDIO": false, "MIN": 1.6,  "MAX": 6.0,  "DEFAULT": 3.0,  "GROUP": "Accretion Disk" },
    { "NAME": "diskOuter",   "LABEL": "Disk Outer",         "TYPE": "float", "AUDIO": false, "MIN": 6.0,  "MAX": 24.0, "DEFAULT": 13.0, "GROUP": "Accretion Disk" },
    { "NAME": "thickness",   "LABEL": "Thickness",          "TYPE": "float", "AUDIO": false, "MIN": 0.2,  "MAX": 3.0,  "DEFAULT": 0.45,  "GROUP": "Accretion Disk" },
    { "NAME": "turbulence",  "LABEL": "Turbulence",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.85, "GROUP": "Accretion Disk" },
    { "NAME": "filaments",   "LABEL": "Filament Detail",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Accretion Disk" },
    { "NAME": "temperature", "LABEL": "Temperature",        "TYPE": "float", "MIN": 0.3,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Accretion Disk" },
    { "NAME": "beaming",     "LABEL": "Doppler Beaming",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.8,  "GROUP": "Accretion Disk" },
    { "NAME": "diskBright",  "LABEL": "Disk Brightness",    "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.8,  "GROUP": "Accretion Disk" },
    { "NAME": "flares",      "LABEL": "Flares",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Accretion Disk" },
    { "NAME": "jetAmt",      "LABEL": "Jets",               "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.45, "GROUP": "Jets & Sky" },
    { "NAME": "photonRing",  "LABEL": "Photon Ring",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.25,  "GROUP": "Jets & Sky" },
    { "NAME": "starAmt",     "LABEL": "Stars",              "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Jets & Sky" },
    { "NAME": "nebulaAmt",   "LABEL": "Nebula",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6,  "GROUP": "Jets & Sky" },
    { "NAME": "paletteMix",  "LABEL": "Blackbody → Custom", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "hotColor",    "LABEL": "Hot Color",          "TYPE": "color", "DEFAULT": [1.0, 0.92, 0.75, 1.0], "GROUP": "Color" },
    { "NAME": "coolColor",   "LABEL": "Cool Color",         "TYPE": "color", "DEFAULT": [0.9, 0.18, 0.04, 1.0], "GROUP": "Color" },
    { "NAME": "jetColor",    "LABEL": "Jet Color",          "TYPE": "color", "DEFAULT": [0.35, 0.6, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "nebulaColor", "LABEL": "Nebula Color",       "TYPE": "color", "DEFAULT": [0.25, 0.12, 0.55, 1.0], "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",          "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.05, "GROUP": "Color" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG",   "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" },
    { "NAME": "exposure",    "LABEL": "Exposure",           "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "bloom",       "LABEL": "Bloom",              "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "streak",      "LABEL": "Anamorphic Streak",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.45,  "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe",   "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.2,  "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",              "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "quality",     "LABEL": "Ray Quality",        "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "marchRes",    "LABEL": "March Resolution",   "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" }
  ],
  "PASSES": [
    { "TARGET": "ehScene", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Event Horizon — FRONTIER pack #7
//   units: Schwarzschild radius rs = 1 (horizon r=1, photon sphere r=1.5,
//   ISCO r=3). Each camera ray is integrated as a null geodesic with the
//   Binet form  a = -1.5 h² x / r⁵  (h = |x × v|, conserved), so bending,
//   the photon sphere, Einstein ring and the lifted back-disk all emerge.
//   pass 0  ehScene : geodesic march + volumetric disk + jets + lensed sky (HDR)
//   pass 1  final   : fringe, bloom (2 radii), anamorphic streak, ACES, HD finish
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359

float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float hash13(vec3 p3) { p3 = fract(p3 * 0.1031); p3 += dot(p3, p3.zyx + 31.32); return fract((p3.x + p3.y) * p3.z); }
vec3  hash33(vec3 p3) { p3 = fract(p3 * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yxz + 33.33); return fract((p3.xxy + p3.yxx) * p3.zyx); }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

// value noise periodic in x with period P (so the disk angle never seams)
float pnoise(vec2 x, float P) {
    vec2 i = floor(x), f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    float i0 = mod(i.x, P), i1 = mod(i.x + 1.0, P);
    float a = hash12(vec2(i0, i.y)),       b = hash12(vec2(i1, i.y));
    float c = hash12(vec2(i0, i.y + 1.0)), d = hash12(vec2(i1, i.y + 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}
float vnoise3(vec3 x) {
    vec3 i = floor(x), f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(hash13(i), hash13(i + vec3(1,0,0)), f.x), mix(hash13(i + vec3(0,1,0)), hash13(i + vec3(1,1,0)), f.x), f.y),
               mix(mix(hash13(i + vec3(0,0,1)), hash13(i + vec3(1,0,1)), f.x), mix(hash13(i + vec3(0,1,1)), hash13(i + vec3(1,1,1)), f.x), f.y), f.z);
}

// blackbody (T in Kelvin) → linear-ish RGB, normalised
vec3 blackbody(float T) {
    float t = T / 100.0;
    vec3 c;
    c.r = t <= 66.0 ? 1.0 : clamp(1.292936 * pow(t - 60.0, -0.1332047), 0.0, 1.0);
    c.g = t <= 66.0 ? clamp(0.3900816 * log(t) - 0.6318414, 0.0, 1.0) : clamp(1.1298909 * pow(t - 60.0, -0.0755148), 0.0, 1.0);
    c.b = t >= 66.0 ? 1.0 : (t <= 19.0 ? 0.0 : clamp(0.5432068 * log(t - 10.0) - 1.1962541, 0.0, 1.0));
    return c * c; // to linear
}

// ── shared state ─────────────────────────────────────────────────────────
float g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev;
float g_scrX;
float g_rin, g_rout, g_flow0, g_flow1, g_flowW, g_flowP;
vec3  g_ro, g_uu, g_vv, g_ww;

void setup() {
    g_ar   = audioReact;
    g_bass = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * g_ar;
    g_mid  = pow(smoothstep(0.06, 0.85, audioMid), 1.2) * g_ar;
    g_high = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * g_ar;
    g_beat = clamp(audioBeatPulse, 0.0, 1.0) * g_ar;
    g_lev  = clamp(audioLevel, 0.0, 1.0) * g_ar;
    g_t    = TIME;
    g_rin  = diskInner;
    g_rout = max(diskOuter, diskInner + 2.0);

    // flow-map clock: two phases half a period apart, crossfaded, so the
    // differential (Keplerian) rotation shears the texture without ever
    // winding it into aliasing hair
    g_flowP = 9.0;
    float tt = g_t * speed;
    g_flow0 = fract(tt / g_flowP) * g_flowP;
    g_flow1 = fract(tt / g_flowP + 0.5) * g_flowP;
    g_flowW = abs(fract(tt / g_flowP) * 2.0 - 1.0); // 1 when phase0 is at its wrap → use phase1

    // camera: slow azimuth orbit + breathing inclination / distance
    float ot  = g_t * orbitSpeed * 0.035;
    float inc = radians(inclination) + drift * 0.05 * sin(g_t * 0.071) ;
    float dist = camDist * (1.0 + drift * 0.06 * sin(g_t * 0.053 + 1.3));
    float az  = ot * TAU + 0.6;
    g_ro = dist * vec3(cos(inc) * cos(az), sin(inc), cos(inc) * sin(az));
    vec3 target = vec3(0.0, 0.0, 0.0);
    g_ww = normalize(target - g_ro);
    float rl = roll + drift * 0.03 * sin(g_t * 0.09);
    vec3 up = vec3(sin(rl), cos(rl), 0.0);
    // roll defined in camera space
    vec3 u0 = normalize(cross(g_ww, vec3(0.0, 1.0, 0.0)));
    vec3 v0 = cross(u0, g_ww);
    g_uu = u0 * cos(rl) + v0 * sin(rl);
    g_vv = v0 * cos(rl) - u0 * sin(rl);
}

// ── accretion disk volume ────────────────────────────────────────────────
float diskHalfH(float r) { return (0.022 + 0.009 * r) * thickness; }

// returns density in .x, temperature (K) in .y, local hot-clump emission boost in .z
vec3 diskSample(vec3 p, float r) {
    float h = diskHalfH(r);
    float y = p.y / h;
    float vert = exp(-y * y * 1.6);
    if (vert < 0.01) return vec3(0.0);

    // radial envelope: soft plunge inside ISCO, long fade outside
    if (r < g_rin * 0.94 || r > g_rout * 1.02) return vec3(0.0);
    float ang = atan(p.z, p.x);               // -PI..PI
    float omega = 1.6 / (r * sqrt(r));        // Keplerian Ω ∝ r^-1.5
    float lr = log(r);

    // two flow phases
    float a0 = ang / TAU + omega * g_flow0 / TAU;
    float a1 = ang / TAU + omega * g_flow1 / TAU;
    float nw = sqrt(g_flowW * g_flowW + (1.0 - g_flowW) * (1.0 - g_flowW));

    // radial envelope: razor ISCO edge, ragged outer silhouette (edge radius
    // wanders with angle and rides the flow, so the rim breaks into tongues)
    float inner = smoothstep(g_rin * 0.95, g_rin * 1.005, r);
    float outer = 1.0;
    if (r > g_rout * 0.6) {
        float eN = mix(pnoise(vec2(a0 * 11.0, 4.1), 11.0), pnoise(vec2(a1 * 11.0, 4.1), 11.0), g_flowW);
        float eN2 = mix(pnoise(vec2(a0 * 37.0, 8.3), 37.0), pnoise(vec2(a1 * 37.0, 8.3), 37.0), g_flowW);
        float rEdge = g_rout * (0.74 + 0.2 * eN + 0.08 * eN2);
        outer = 1.0 - smoothstep(rEdge * 0.9, rEdge, r);
    }
    float env = inner * outer;
    if (env < 0.002) return vec3(0.0);

    float turb = turbulence * (1.0 + 0.35 * g_mid);
    float d0 = 0.0, d1 = 0.0, amp = 0.5, tot = 0.0;
    float P = 14.0, fr = 7.0;
    for (int o = 0; o < 6; o++) {
        float fo = float(o);
        float off = fo * 3.7;
        // elongated: angle frequency P, radial frequency fr  → spiral streaks
        float n0 = pnoise(vec2(a0 * P, lr * fr + off + y * 0.6), P);
        float n1 = pnoise(vec2(a1 * P, lr * fr + off + y * 0.6), P);
        // ridged at the fine octaves → crisp filaments
        float rg = smoothstep(1.0, 3.0, fo) * filaments;
        n0 = mix(n0, 1.0 - abs(2.0 * n0 - 1.0), rg);
        n1 = mix(n1, 1.0 - abs(2.0 * n1 - 1.0), rg);
        d0 += amp * n0; d1 += amp * n1; tot += amp;
        amp *= 0.55; P *= 2.0; fr *= 2.1;
    }
    // variance-preserving crossfade keeps contrast through the blend
    float n = clamp((mix(d0, d1, g_flowW) / tot - 0.5) / nw + 0.5, 0.0, 1.0);
    float nc = smoothstep(0.36, 0.74, n);
    float dens = mix(0.5, nc * nc * 2.2, clamp(turb, 0.0, 1.0));
    dens *= mix(1.0, 0.6 + 0.8 * n, max(turb - 1.0, 0.0) * 2.0);

    // concentric gaps — the ring structure seen in disks
    float rw = lr + 0.012 * sin(ang * 3.0 + lr * 5.0);
    float rings = (0.7 + 0.3 * sin(rw * 41.0 + 1.7 * sin(rw * 9.0))) * (0.75 + 0.25 * sin(rw * 113.0 + 2.0 * sin(rw * 23.0))) * (0.85 + 0.15 * sin(rw * 260.0));
    dens *= mix(1.0, rings, 0.4);

    // hair: thin Keplerian-sheared streaks (long in angle, ~0.06 rs across)
    float hw = r * 17.0 + 0.6 * sin(lr * 7.0 + ang);
    float h0 = pnoise(vec2(a0 * 96.0, hw), 96.0), h1 = pnoise(vec2(a1 * 96.0, hw), 96.0);
    float hair = clamp((mix(h0, h1, g_flowW) - 0.5) / nw + 0.5, 0.0, 1.0);
    hair = 1.0 - abs(2.0 * hair - 1.0);
    dens *= mix(1.0, 0.35 + 1.25 * hair * hair * hair, 0.8 * filaments);

    // dust lanes: dark absorbing bands that break the outer disk
    float laneW = smoothstep(g_rin * 1.6, g_rout * 0.55, r);
    if (laneW > 0.001) {
        float l0 = pnoise(vec2(a0 * 19.0, lr * 13.0 + 9.0), 19.0), l1 = pnoise(vec2(a1 * 19.0, lr * 13.0 + 9.0), 19.0);
        float lanes = smoothstep(0.58, 0.78, clamp((mix(l0, l1, g_flowW) - 0.5) / nw + 0.5, 0.0, 1.0));
        dens *= 1.0 - 0.8 * lanes * laneW;
    }

    // the ISCO rim: hottest, thinnest, brightest filament hugging the shadow
    float rimD = (r - g_rin * 1.015) / (0.035 * g_rin);
    dens += 0.75 * exp(-rimD * rimD) * (0.6 + 0.4 * hair);

    // hot clumps / magnetic flares orbiting with the gas
    float clump = 0.0;
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        float rc = mix(g_rin * 1.1, g_rin * 2.6, hash12(vec2(fi, 3.1)));
        float om = 1.6 / (rc * sqrt(rc));
        float ac = hash12(vec2(fi, 7.7)) * TAU + om * g_t * speed;
        vec2 cp = rc * vec2(cos(ac), sin(ac));
        vec2 dv = p.xz - cp;
        // stretch along the orbit (tangent)
        vec2 tng = vec2(-sin(ac), cos(ac));
        float along = dot(dv, tng), across = dot(dv, vec2(cos(ac), sin(ac)));
        float life = 0.5 + 0.5 * sin(g_t * (0.21 + 0.07 * fi) + fi * 2.3);
        clump += exp(-(along * along * 1.2 + across * across * 14.0)) * life * life;
    }
    clump *= flares * (0.6 + 1.6 * g_beat + 0.8 * g_high);

    // temperature profile (Shakura–Sunyaev-ish): T ∝ r^-3/4 (1 - sqrt(rin/r))^1/4
    // (stylised: no zero-torque dip — the inner edge is the hottest, like Gargantua)
    float tr = pow(g_rin / r, 0.75) * (1.0 + 0.55 * exp(-(r - g_rin) * 2.5));
    float T = 7600.0 * temperature * tr * (0.9 + 0.2 * n);
    T *= 1.0 + 0.25 * clump;

    return vec3(dens * env * vert, T, clump * env * vert);
}

// ── jets ─────────────────────────────────────────────────────────────────
float jetDensity(vec3 p) {
    float ay = abs(p.y);
    if (ay < 2.0 || ay > 30.0) return 0.0;
    float rad = length(p.xz);
    float w = 0.035 + 0.014 * ay;
    if (rad > 4.0 * w) return 0.0;
    float core = exp(-rad * rad / (w * w)) + 0.18 * exp(-rad * rad / (9.0 * w * w));
    // knots travelling outward
    float kn = 0.55 + 0.45 * sin(ay * 1.3 - g_t * 3.0 * (0.5 + speed));
    kn *= 0.7 + 0.3 * vnoise3(vec3(p.xz * 3.0, ay * 0.8 - g_t * 2.0));
    float fade = smoothstep(2.0, 5.0, ay) * exp(-ay * 0.09);
    return core * kn * fade;
}

// ── lensed sky ───────────────────────────────────────────────────────────
vec2 octUV(vec3 d) {
    d /= abs(d.x) + abs(d.y) + abs(d.z);
    vec2 p = d.xz;
    if (d.y < 0.0) p = (1.0 - abs(p.yx)) * vec2(p.x >= 0.0 ? 1.0 : -1.0, p.y >= 0.0 ? 1.0 : -1.0);
    return p;
}
vec3 octDir(vec2 p) {
    vec3 d = vec3(p.x, 1.0 - abs(p.x) - abs(p.y), p.y);
    if (d.y < 0.0) d.xz = (1.0 - abs(d.zx)) * vec2(d.x >= 0.0 ? 1.0 : -1.0, d.z >= 0.0 ? 1.0 : -1.0);
    return normalize(d);
}
// er/et: radial/tangential axes around the lens axis; mu: (radial, tangential)
// magnification to undo far from the shadow so the far field stays round points
vec3 starLayer(vec2 o, vec3 d, vec3 er, vec3 et, vec2 mu, float sc, float prob, float pix) {
    vec2 q = o * sc;
    vec2 c = floor(q);
    vec3 col = vec3(0.0);
    // star kept ≥0.2 cell from the edges and ≪ cell size → own cell only
    {
        vec2 cc = c;
        vec3 h = hash33(vec3(cc, sc));
        if (h.x < prob) {
            vec2 sp = cc + 0.5 + 0.6 * (hash33(vec3(cc, sc + 7.1)).xy - 0.5);
            vec3 dv = d - octDir(sp / sc);
            vec2 dd = vec2(dot(dv, er), dot(dv, et)) * mu;
            float size = pix * (0.42 + 0.5 * h.y * h.y);
            float b = exp(-dot(dd, dd) / (size * size));
            float T = mix(3000.0, 16000.0, h.z * h.z);
            vec3 c3 = mix(blackbody(T), vec3(1.0), 0.45);
            col += c3 * b * (0.035 + 0.45 * pow(h.y, 5.0) + 9.0 * pow(h.y, 30.0));
        }
    }
    return col;
}

vec3 skyColor(vec3 d, vec3 rd0, float pix, float starFade) {
    vec3 col = vec3(0.0);
    // stars: four densities, pinpoint, angular size tied to the pixel footprint.
    // Lensing magnification (tangential θ/β, radial θ/(2θ-β) for a point lens) is
    // undone beyond ~2 shadow radii so only the region near the ring shows arcs.
    vec3 ax = normalize(-g_ro);
    float th = acos(clamp(dot(rd0, ax), -1.0, 1.0));
    float be = acos(clamp(dot(d, ax), -1.0, 1.0));
    float thSh = 2.6 / length(g_ro);
    float comp = smoothstep(thSh * 1.25, thSh * 2.3, th) * step(0.0, th - be + 0.02);
    be = max(be, 1e-3);
    vec2 mu = mix(vec2(1.0), vec2(th / max(2.0 * th - be, 1e-3), th / be), comp);
    vec3 er = d - ax * dot(d, ax);
    er = dot(er, er) > 1e-8 ? normalize(er) : g_uu;
    vec3 et = cross(ax, er);
    vec2 o = octUV(d);
    col += starLayer(o, d, er, et, mu, 22.0,  0.6, pix) * 1.3;
    col += starLayer(o, d, er, et, mu, 55.0,  0.40, pix) * 0.9;
    col += starLayer(o, d, er, et, mu, 130.0, 0.32, pix) * 0.7;
    col += starLayer(o, d, er, et, mu, 300.0, 0.2, pix) * 0.55;
    col *= starAmt * starFade;
    // a faint galactic band + dust lanes
    vec3 bandN = normalize(vec3(0.25, 1.0, 0.35));
    float band = exp(-pow(dot(d, bandN) * 3.2, 2.0));
    float n = vnoise3(d * 5.0) * 0.5 + vnoise3(d * 11.0) * 0.3 + vnoise3(d * 27.0) * 0.2;
    float dust = smoothstep(0.35, 0.7, vnoise3(d * 8.0 + 3.0));
    vec3 neb = nebulaColor.rgb * band * n * n * (1.0 - 0.7 * dust) * 0.22;
    neb += vec3(0.9, 0.5, 0.35) * band * pow(n, 4.0) * 0.08;
    col += neb * nebulaAmt;
    return col;
}

// ── geodesic march ───────────────────────────────────────────────────────
vec4 trace(vec3 ro, vec3 rd, float pix, float jit) {
    vec3 pos = ro;
    vec3 vel = rd;
    vec3 hv = cross(pos, vel);
    float h2 = dot(hv, hv);
    float lensK = 1.5 * lensing;

    vec3 acc = vec3(0.0);
    float trans = 1.0;
    float minR = 1e3;
    bool captured = false;
    // rays below the critical impact parameter are doomed; their SECOND pass through
    // the disk (the plunge image inside the shadow) is a sliver squeezed by extreme
    // lensing — it only aliases into blue sparkle, so it is dimmed
    bool doomed = sqrt(h2) < 2.598 * sqrt(max(lensing, 0.05));
    float slabPasses = 0.0;
    bool wasIn = false;
    float stepQ = 1.0 / quality;
    // linear bass/mid feed (soft music still breathes) + a kneed punch on top
    float accretion = diskBright * (1.0 + 0.4 * clamp(audioBass, 0.0, 1.0) * g_ar + 0.2 * clamp(audioMid, 0.0, 1.0) * g_ar + 0.3 * g_bass);

    // skip the empty approach analytically: jump to the bounding sphere (r = 30)
    float bR = max(g_rout * 1.6, 30.0);
    float bb = dot(pos, vel), cc = dot(pos, pos) - bR * bR;
    float disc = bb * bb - cc;
    if (cc > 0.0) {
        if (disc < 0.0 || bb > 0.0) {
            return vec4(skyColor(rd, rd, pix, 1.0), 0.0);
        }
        pos += vel * (-bb - sqrt(disc));
    }

    for (int i = 0; i < 260; i++) {
        float r = length(pos);
        minR = min(minR, r);
        if (r < 1.0) { captured = true; break; }
        if (r > bR + 0.5 && dot(pos, vel) > 0.0) break;

        // adaptive step: tight near the horizon and inside the disk slab
        float dt = clamp(0.09 * r * (r - 0.85) / r, 0.012, 1.4) * stepQ;
        dt = min(dt, max(0.06 * (r - 0.95), 0.01));
        float hd = diskHalfH(r);
        bool inSlab = abs(pos.y) < 3.2 * hd && r < g_rout * 1.05 && r > g_rin * 0.75;
        float rxz = length(pos.xz);
        if (jetAmt > 0.001 && abs(pos.y) > 1.8) { float jw = 0.035 + 0.014 * abs(pos.y); if (rxz < 5.0 * jw) dt = min(dt, max(rxz * 0.4, jw * 0.8)); }
        // resolve the vertical profile (needs dt·|v̂y| < h/2) and the texture along the ray
        if (inSlab && !wasIn) slabPasses += 1.0;
        wasIn = inSlab;
        if (inSlab) {
            float vy = abs(vel.y) / length(vel);
            float dV = hd * 0.5 / max(vy, 0.02);
            dt = min(dt, max(min(dV, 0.035 + 0.008 * r), 0.012) * stepQ);
        }
        else if (abs(pos.y) < 6.0 * hd && r < g_rout * 1.05) dt = min(dt, max(abs(pos.y) - 2.9 * hd, 0.04));
        // velocity Verlet on the Binet acceleration (2nd order, stable stars)
        vec3 a = -lensK * h2 * pos / (r * r * r * r * r);
        pos += vel * dt + 0.5 * a * dt * dt;
        float r2 = length(pos);
        vec3 a2 = -lensK * h2 * pos / (r2 * r2 * r2 * r2 * r2);
        vel += 0.5 * (a + a2) * dt;

        // disk volume
        if (inSlab) {
            float rr = length(pos);
            vec3 ds = diskSample(pos, rr);
            if (ds.x > 0.0005) {
                // Doppler + gravitational shift
                float beta = sqrt(0.5 / max(rr - 1.0, 0.3));
                beta = min(beta, 0.75);
                vec3 phiHat = normalize(vec3(-pos.z, 0.0, pos.x));
                vec3 toObs = -normalize(vel);
                float gam = 1.0 / sqrt(1.0 - beta * beta);
                float D = 1.0 / (gam * (1.0 - beta * dot(phiHat, toObs)));
                float gGrav = sqrt(max(1.0 - 1.0 / rr, 0.02));
                float g = D * gGrav;
                float gb = mix(1.0, g, beaming);
                float T = ds.y * gb;
                vec3 bbC = blackbody(T);
                float tN = clamp((T - 2500.0) / 13000.0, 0.0, 1.0);
                vec3 cust = mix(coolColor.rgb, hotColor.rgb, tN) * (0.25 + 1.2 * tN);
                vec3 em = mix(bbC * (0.25 + 1.4 * tN * tN + 0.15 * tN), cust, paletteMix);
                float I = pow(max(gb, 0.05), 2.6) * (0.55 + 0.9 * (ds.y / 7000.0));
                em = max(mix(vec3(lum(em)), em, 1.35), 0.0);
                em *= I * accretion * 0.9;
                em += vec3(1.0, 0.85, 0.7) * ds.z * 0.5 * accretion * pow(max(gb, 0.05), 3.0);
                if (doomed && slabPasses > 1.5) em *= 0.08;
                float k = ds.x * 13.0;
                float stepT = exp(-k * dt);
                acc += trans * em * ds.x * dt * 7.0 * (1.0 - stepT) / max(k * dt, 1e-4);
                trans *= stepT;
                if (trans < 0.01) break;
            }
        }
        // jets (only near the axis)
        if (jetAmt > 0.001) {
            float jd = jetDensity(pos);
            if (jd > 0.001) acc += trans * jetColor.rgb * jd * dt * jetAmt * 2.6 * (1.0 + 0.6 * g_bass);
        }
    }

    vec3 col = acc;
    float cover = 1.0 - trans;
    if (!captured) {
        vec3 dir = normalize(vel);
        // just outside the critical curve the whole sky is squeezed into a thin
        // annulus: a pixel covers thousands of stars → fade them to avoid sparkle noise
        float bc0 = 2.598 * sqrt(max(lensing, 0.05));
        float sf = smoothstep(bc0 * 1.03, bc0 * 1.3, sqrt(h2));
        col += trans * skyColor(dir, rd, pix, sf);
    } else {
        cover = 1.0;
    }
    // photon ring: rays skimming the photon sphere stack many lensed images
    // measured on the conserved impact parameter b (critical b = 3√3/2 rs), so its
    // screen width is a steady ~2 px instead of an aliased dotted line
    float bImp = sqrt(h2), bc = 2.598 * sqrt(max(lensing, 0.05));
    float db = (bImp - bc) / (bImp > bc ? 0.05 : 0.018);
    float pr = trans * trans * trans * trans * exp(-db * db) * step(0.01, lensing);
    pr *= mix(1.0, 0.45 + 0.9 * smoothstep(-0.25, 0.25, g_scrX), beaming * 0.6);
    col += photonRing * pr * mix(vec3(1.0, 0.86, 0.66), hotColor.rgb, paletteMix) * 0.9 * diskBright * (1.0 + 0.4 * g_bass);
    return vec4(col, max(cover, pr * 0.8));
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    setup();

    if (PASSINDEX == 0) {
        vec2 uv = (fragCoord - 0.5 * R) / R.y;
        vec3 rd = normalize(uv.x * g_uu + uv.y * g_vv + (1.0 / tan(fov * 0.5)) * 0.5 * g_ww);
        float pix = fov / R.y * 1.1;
        float jit = fract(52.9829189 * fract(0.06711056 * fragCoord.x + 0.00583715 * fragCoord.y));
        g_scrX = uv.x;
        vec4 sc = trace(g_ro, rd, pix, jit);
        vec3 col = sc.rgb * exposure;
        col = hueRot(col, hueShift);
        gl_FragColor = vec4(max(col, 0.0), sc.a);
    } else {
        vec2 st = fragCoord / R;
        vec2 uv = (fragCoord - 0.5 * R) / R.y;
        vec2 asp = vec2(R.y / R.x, 1.0);
        vec4 c0 = texture2D(ehScene, st);
        vec3 col = c0.rgb;
        float ignR = 0.0; // fixed spiral: per-pixel rotation speckled the dark shadow next to the hot rim
        // chromatic fringe (radial)
        float ab = aberration * 0.003;
        vec2 dir = (st - 0.5) * 2.0;
        vec3 fr = vec3(texture2D(ehScene, st + dir * ab).r, c0.g, texture2D(ehScene, st - dir * ab).b);
        col = mix(col, fr, 0.8);
        // bloom: wide + tight
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 20; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + ignR;
            float rr = sqrt((fi + 0.5) / 20.0) * 0.07;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            vec3 s = texture2D(ehScene, st + of).rgb;
            bl += max(s - 1.1, 0.0) * (1.0 - rr / 0.07);
        }
        bl /= 20.0;
        vec3 bl2 = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float fi = float(i);
            float a = fi * 2.399963 + ignR + 1.0;
            float rr = sqrt((fi + 0.5) / 8.0) * 0.012;
            vec2 of = vec2(cos(a), sin(a)) * rr * asp;
            bl2 += max(texture2D(ehScene, st + of).rgb - 0.7, 0.0);
        }
        bl2 /= 8.0;
        col += (bl * 1.3 + bl2 * 0.5) * bloom;
        // anamorphic streak
        if (streak > 0.001) {
            vec3 sk = vec3(0.0);
            for (int i = 0; i < 14; i++) {
                float fi = float(i) - 6.5;
                float w = 1.0 - abs(fi) / 7.5;
                vec2 of = vec2(fi * 0.02 * asp.x, 0.0);
                sk += max(texture2D(ehScene, st + of).rgb - 2.0, 0.0) * w * w;
            }
            sk /= 5.0;
            col += sk * vec3(0.45, 0.7, 1.25) * streak * 0.5;
        }
        col *= 1.0 + 0.22 * clamp(audioLevel, 0.0, 1.0) * g_ar;
        // 4-point diffraction glints on the brightest points (stars, flares)
        {
            vec3 gl = vec3(0.0);
            vec2 px = 1.0 / R;
            for (int i = 1; i <= 7; i++) {
                float fi = float(i);
                float w = exp(-fi * 0.42);
                vec2 oh = vec2(fi * 1.6 * px.x, 0.0), ov = vec2(0.0, fi * 1.6 * px.y);
                gl += w * (max(texture2D(ehScene, st + oh).rgb - 1.6, 0.0) + max(texture2D(ehScene, st - oh).rgb - 1.6, 0.0)
                         + max(texture2D(ehScene, st + ov).rgb - 1.6, 0.0) + max(texture2D(ehScene, st - ov).rgb - 1.6, 0.0));
            }
            col += gl * 0.22 * (0.5 + 0.5 * streak);
        }
        col = aces(col * 1.1);
        float l0 = lum(col);
        col = clamp(mix(vec3(l0), col, saturation), 0.0, 1.0);
        float g1 = (hash12(fragCoord + fract(g_t) * 61.0) - 0.5) * 0.04;
        col += g1 * grain * (0.4 + 0.6 * (1.0 - abs(l0 - 0.5) * 2.0));
        col *= 1.0 - 0.12 * smoothstep(0.5, 1.4, length(uv));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
        float lumHD = lum(col);
        col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
        float alphaOut = transparentBg ? clamp(c0.a + lum(col) * 0.5, 0.0, 1.0) : 1.0;
        gl_FragColor = vec4(col, alphaOut);
    }
}
