/*{
  "DESCRIPTION": "Vishes — Mycelium Network: bioluminescent fungal tendrils growing through 3D space, raymarched with branching tube SDFs, electric cyan / lime / magenta palette, audio-reactive radius and glow.",
  "CREDIT": "ShaderClaw — 3D mycelium network",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "growthSpeed",   "LABEL": "Growth Speed",   "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 3.0  },
    { "NAME": "networkScale",  "LABEL": "Network Scale",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3,  "MAX": 3.0  },
    { "NAME": "glowIntensity", "LABEL": "Glow Intensity", "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.0,  "MAX": 4.0  },
    { "NAME": "audioMod",      "LABEL": "Audio Mod",      "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.0,  "MAX": 3.0  },
    { "NAME": "branchDepth",   "LABEL": "Branch Depth",   "TYPE": "float", "DEFAULT": 2.0,  "MIN": 1.0,  "MAX": 3.0  }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────────
// Constants
// ─────────────────────────────────────────────────────────────────────────────
#define MAX_STEPS  64
#define MAX_DIST   18.0
#define SURF_DIST  0.003
#define PI         3.14159265359
#define TAU        6.28318530718

// ─────────────────────────────────────────────────────────────────────────────
// Hash / noise — no rand(), no texture, deterministic forever
// ─────────────────────────────────────────────────────────────────────────────
float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

vec3 hash31(float p) {
    return vec3(
        hash11(p),
        hash11(p * 1.7319 + 13.7),
        hash11(p * 3.1415 +  7.3)
    );
}

// ─────────────────────────────────────────────────────────────────────────────
// SDF primitives
// ─────────────────────────────────────────────────────────────────────────────

// Capsule — segment a→b, radius r
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b - a;
    vec3 ap = p - a;
    float t = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
    return length(ap - ab * t) - r;
}

// Sphere
float sdSphere(vec3 p, vec3 c, float r) {
    return length(p - c) - r;
}

// ─────────────────────────────────────────────────────────────────────────────
// Palette — 5 slots, fully saturated, HDR peaks ≥ 2.0, no white mixing
//   0 → void black background
//   1 → electric cyan   (primary tendrils)
//   2 → neon lime-green (level-2 branches)
//   3 → hot magenta     (node spheres / tips)
//   4 → HDR white core  (narrow inner track)
// ─────────────────────────────────────────────────────────────────────────────
vec3 matColor(int m) {
    if (m == 1) return vec3(0.10, 2.50, 2.20);   // electric cyan  (HDR)
    if (m == 2) return vec3(0.30, 2.50, 0.10);   // neon lime      (HDR)
    if (m == 3) return vec3(2.50, 0.10, 1.80);   // hot magenta    (HDR)
    if (m == 4) return vec3(2.50, 2.50, 2.50);   // inner core     (HDR white peak)
    return vec3(0.0, 0.0, 0.01);                  // void black
}

// ─────────────────────────────────────────────────────────────────────────────
// Branch direction — unit-sphere sample from a scalar seed
// ─────────────────────────────────────────────────────────────────────────────
vec3 branchDir(float seed) {
    vec3 h     = hash31(seed);
    float theta = h.x * TAU;
    float phi   = acos(2.0 * h.y - 1.0);
    return vec3(sin(phi) * cos(theta),
                cos(phi),
                sin(phi) * sin(theta));
}

// ─────────────────────────────────────────────────────────────────────────────
// Hit record
// ─────────────────────────────────────────────────────────────────────────────
struct HitInfo {
    float dist;
    int   mat;
};

HitInfo minHit(HitInfo a, HitInfo b) {
    return (a.dist < b.dist) ? a : b;
}

// ─────────────────────────────────────────────────────────────────────────────
// Scene SDF — 8 level-1 tendrils + up to 16 level-2 branches + level-3 fringe
//             = up to ~24 tubes total (stays within spec of ~20)
// audioRadius = audioLevel * audioMod (pre-multiplied by caller)
// ─────────────────────────────────────────────────────────────────────────────
HitInfo sceneSDF(vec3 p, float audioRadius) {
    // Work in network-local space
    p /= networkScale;

    float baseR = 0.055 * (1.0 + audioRadius);

    HitInfo result;
    result.dist = MAX_DIST;
    result.mat  = 0;

    // Central hub — magenta anchor sphere
    {
        float hubR = 0.10 * (0.9 + 0.1 * sin(TIME * growthSpeed));
        HitInfo hub;
        hub.dist = sdSphere(p, vec3(0.0), hubR);
        hub.mat  = 3;
        result   = minHit(result, hub);
    }

    // ── Level-1: 8 primary tendrils ──────────────────────────────────────────
    for (int i = 0; i < 8; i++) {
        float fi    = float(i);
        float seed1 = fi * 7.31 + 1.0;

        vec3  dir1  = branchDir(seed1);
        float len1  = 0.55 + hash11(seed1 + 3.7) * 0.45;
        vec3  tipB  = dir1 * len1;          // tip of level-1 tube (origin → tipB)

        // Radius breathes (growth oscillation) and tapers toward tip
        float rA = baseR * (0.9 + 0.1 * sin(TIME * growthSpeed * 0.7 + seed1));
        float rB = rA * 0.55;

        // Interpolate radius along the capsule for the taper look
        vec3  abSeg = tipB;                 // = tipB - vec3(0)
        float tSeg  = clamp(dot(p, abSeg) / dot(abSeg, abSeg), 0.0, 1.0);
        float rInterp = mix(rA, rB, tSeg);

        // Primary capsule (cyan)
        HitInfo hi1;
        hi1.dist = sdCapsule(p, vec3(0.0), tipB, rInterp);
        hi1.mat  = 1;
        result   = minHit(result, hi1);

        // Inner core track — HDR white, very thin
        {
            HitInfo core;
            core.dist = sdCapsule(p, dir1 * 0.05, tipB - dir1 * 0.05, rInterp * 0.18);
            core.mat  = 4;
            result    = minHit(result, core);
        }

        // Node sphere at level-1 tip (magenta)
        {
            HitInfo node;
            node.dist = sdSphere(p, tipB, rA * 1.15);
            node.mat  = 3;
            result    = minHit(result, node);
        }

        // ── Level-2 branches ─────────────────────────────────────────────────
        if (branchDepth >= 2.0) {
            // First 4 tendrils get 2 sub-branches; last 4 get 1 → 4×2 + 4×1 = 12
            int numSub = (i < 4) ? 2 : 1;
            for (int j = 0; j < 2; j++) {
                if (j >= numSub) break;

                float seed2 = seed1 * 3.7 + float(j) * 11.3 + 5.0;
                vec3  dir2  = normalize(dir1 * 0.6 + branchDir(seed2) * 0.8);
                float len2  = 0.28 + hash11(seed2 + 2.1) * 0.28;
                vec3  tipC  = tipB;
                vec3  tipD  = tipB + dir2 * len2;

                float rC = rB;
                float rD = rC * 0.45 * (0.85 + 0.15 * sin(TIME * growthSpeed * 0.7 + seed2 * 1.3));

                // Level-2 capsule (lime-green)
                HitInfo hi2;
                hi2.dist = sdCapsule(p, tipC, tipD, mix(rC, rD, 0.5));
                hi2.mat  = 2;
                result   = minHit(result, hi2);

                // Level-2 tip node (magenta)
                {
                    HitInfo node2;
                    node2.dist = sdSphere(p, tipD, rD * 1.4);
                    node2.mat  = 3;
                    result     = minHit(result, node2);
                }

                // ── Level-3 fringe filaments (j==0 only to stay within budget) ──
                if (branchDepth >= 3.0 && j == 0) {
                    float seed3 = seed2 * 2.3 + 17.0;
                    vec3  dir3  = normalize(dir2 * 0.5 + branchDir(seed3) * 0.9);
                    float len3  = 0.14 + hash11(seed3) * 0.14;
                    float rF    = rD * 0.55;

                    HitInfo hi3;
                    hi3.dist = sdCapsule(p, tipD, tipD + dir3 * len3, rF);
                    hi3.mat  = 2;
                    result   = minHit(result, hi3);
                }
            }
        }
    }

    // Scale distance back out of local space
    result.dist *= networkScale;
    return result;
}

// ─────────────────────────────────────────────────────────────────────────────
// Normal via tetrahedron finite-difference (4 evaluations)
// ─────────────────────────────────────────────────────────────────────────────
vec3 calcNormal(vec3 p, float ar) {
    vec2 e = vec2(0.002, -0.002);
    return normalize(
        e.xyy * sceneSDF(p + e.xyy, ar).dist +
        e.yyx * sceneSDF(p + e.yyx, ar).dist +
        e.yxy * sceneSDF(p + e.yxy, ar).dist +
        e.xxx * sceneSDF(p + e.xxx, ar).dist
    );
}

// ─────────────────────────────────────────────────────────────────────────────
// Main
// ─────────────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    // ── Audio modulators (modulator pattern: 1.0 + level * factor) ────────────
    float audioRadius = audioLevel * audioMod;          // extra radius beyond base
    float aGlow       = 1.0 + audioLevel * audioMod * 0.6;  // glow brightness mod

    // ── Camera: slow orbit, slight downward tilt, looks at origin ─────────────
    float orbit    = TIME * 0.18;
    float camDist  = 3.8;
    float camElev  = 0.35;   // radians above equator

    vec3 ro = vec3(
        cos(orbit) * cos(camElev) * camDist,
        sin(camElev)              * camDist,
        sin(orbit) * cos(camElev) * camDist
    );
    vec3 target = vec3(0.0, 0.10, 0.0);
    vec3 fwd    = normalize(target - ro);
    vec3 right  = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up     = cross(right, fwd);

    // 1.6 focal length gives a natural FOV without wide-angle distortion
    vec3 rd = normalize(fwd * 1.6 + right * uv.x + up * uv.y);

    // ── Raymarching — 64 steps, damped step near surface ─────────────────────
    float totalDist = 0.0;
    bool  hit       = false;
    vec3  hitPos;
    HitInfo hi;
    hi.dist = MAX_DIST;
    hi.mat  = 0;

    for (int s = 0; s < MAX_STEPS; s++) {
        hitPos = ro + rd * totalDist;
        hi     = sceneSDF(hitPos, audioRadius);

        if (hi.dist < SURF_DIST) { hit = true; break; }
        if (totalDist > MAX_DIST) break;
        totalDist += hi.dist * 0.72;   // conservative step so we don't overshoot thin tubes
    }

    // ── Volumetric fringe halo — cheap 16-sample glow pass along the ray ─────
    // Finds the closest approach of the ray to any surface; drives additive glow.
    float nearestD = MAX_DIST;
    {
        float tProbe = 0.0;
        for (int k = 0; k < 16; k++) {
            vec3    pp     = ro + rd * tProbe;
            HitInfo probe  = sceneSDF(pp, audioRadius);
            float   dMin   = probe.dist;
            nearestD = min(nearestD, dMin);
            tProbe  += max(dMin, 0.05);
            if (tProbe > MAX_DIST) break;
        }
    }

    // ── Background void + dual-layer exponential network halo ─────────────────
    vec3 bg = vec3(0.0, 0.0, 0.01);   // void black with the faintest blue tint

    float haloTight = exp(-max(nearestD, 0.0) * 4.0)  * glowIntensity * aGlow;
    float haloSoft  = exp(-max(nearestD, 0.0) * 1.2)  * glowIntensity * 0.35 * aGlow;
    // Tight halo in cyan, soft outer halo in lime — keeps the palette
    bg += vec3(0.05, 1.50, 1.40) * haloTight * 0.50;
    bg += vec3(0.10, 0.90, 0.20) * haloSoft  * 0.25;

    // ── Surface shading ───────────────────────────────────────────────────────
    vec3 col = bg;

    if (hit) {
        vec3 n = calcNormal(hitPos, audioRadius);
        vec3 v = normalize(ro - hitPos);

        // Two-light rig: warm key from upper-right, cool fill from lower-left
        vec3  L1    = normalize(vec3( 2.0,  3.5,  1.5));
        vec3  L2    = normalize(vec3(-1.5, -1.0, -1.0));
        float diff1 = max(dot(n, L1), 0.0);
        float diff2 = max(dot(n, L2), 0.0) * 0.25;
        float spec  = pow(max(dot(reflect(-L1, n), v), 0.0), 64.0);
        float rim   = pow(1.0 - max(dot(n, v), 0.0), 2.5);

        vec3 baseC = matColor(hi.mat);

        // Diffuse + ambient + rim in the material colour (HDR values stay HDR)
        col  = baseC * (diff1 * 0.60 + diff2 + 0.25);
        col += baseC * rim * 1.4;

        // Specular spike — HDR white so bloom catches it hard
        col += vec3(2.5) * spec * 0.5;

        // fwidth() anti-alias on the silhouette edge
        float dEdge = abs(hi.dist);
        float aa    = 1.0 - smoothstep(0.0, fwidth(dEdge) * 2.0, dEdge);
        col  = mix(col, bg, aa * 0.6);

        // Per-surface additive local glow (tight exponential falloff)
        float localGlow = exp(-max(hi.dist, 0.0) * 12.0) * glowIntensity * aGlow;
        col += baseC * localGlow * 0.45;

        // Depth fade so far-away geometry fades into the halo gracefully
        float depthFade = 1.0 - smoothstep(MAX_DIST * 0.5, MAX_DIST, totalDist);
        col *= depthFade;

        // Blend a trace of background halo under the surface for depth
        col += bg * 0.15;
    }

    // Output LINEAR HDR — no ACES, no clamp, no gamma
    gl_FragColor = vec4(col, 1.0);
}
