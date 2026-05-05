/*{
  "DESCRIPTION": "Shatter Burst — a frozen mid-explosion moment. Diverging shard fragments fly outward from a central burst point, frozen in time. Amber-crimson fire-frozen palette with HDR white-hot core.",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "shardCount",  "LABEL": "Shard Count",   "TYPE": "float", "DEFAULT": 14.0, "MIN": 2.0,  "MAX": 20.0 },
    { "NAME": "burstRadius", "LABEL": "Burst Radius",  "TYPE": "float", "DEFAULT": 1.4,  "MIN": 0.4,  "MAX": 3.5  },
    { "NAME": "shardScale",  "LABEL": "Shard Scale",   "TYPE": "float", "DEFAULT": 0.28, "MIN": 0.05, "MAX": 0.8  },
    { "NAME": "orbitSpeed",  "LABEL": "Orbit Speed",   "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "audioMod",    "LABEL": "Audio Mod",     "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0  }
  ]
}*/

// ---- constants -------------------------------------------------------
#define MAX_STEPS 64
#define MAX_DIST  18.0
#define SURF_DIST 0.003
#define PI        3.14159265359

// ---- palette ---------------------------------------------------------
const vec3 COL_BURST   = vec3(3.0, 2.5, 1.0);   // white-hot central burst
const vec3 COL_OUTER   = vec3(2.0, 0.8, 0.0);   // deep amber (outer face)
const vec3 COL_INNER   = vec3(2.5, 0.2, 0.0);   // hot crimson (inner face)
const vec3 COL_GLINT   = vec3(3.0, 1.5, 0.1);   // HDR orange-white edge glint
const vec3 COL_BLACK   = vec3(0.0, 0.0, 0.0);   // ink black background

// ---- hash functions --------------------------------------------------
float h11(float n) { return fract(sin(n) * 43758.5453); }
vec3  h31(float n) {
    return vec3(h11(n), h11(n + 127.1), h11(n + 269.5));
}

// Per-shard data: position center and orientation
// Returns outward direction unit vector in a sphere-like distribution
vec3 shardDir(float id) {
    // Fibonacci sphere distribution for even angular spread
    float phi   = PI * (3.0 - sqrt(5.0));         // golden angle in radians
    float y     = 1.0 - (id / max(shardCount - 1.0, 1.0)) * 2.0;
    float r     = sqrt(max(1.0 - y * y, 0.0));
    float theta = phi * id;
    return normalize(vec3(cos(theta) * r, y, sin(theta) * r));
}

// Per-shard random tilt axes (for brick-like rotation)
vec3 shardRight(float id) {
    vec3 d = shardDir(id);
    vec3 up = (abs(d.y) < 0.9) ? vec3(0.0, 1.0, 0.0) : vec3(1.0, 0.0, 0.0);
    return normalize(cross(d, up));
}
vec3 shardUp(float id) {
    return normalize(cross(shardDir(id), shardRight(id)));
}

// Signed-box SDF (brick)
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Central burst sphere SDF
float sdSphere(vec3 p, float r) { return length(p) - r; }

// ---- scene SDF -------------------------------------------------------
// Returns vec2(dist, objectID):  id=0 → burst sphere, id=1..N → shards
vec2 mapScene(vec3 p, float audioScale) {
    // "Breathing" frozen-frame oscillation — shard distance breathes in time
    float breathe = sin(TIME * 0.3) * 0.18;

    vec2 res = vec2(1e10, -1.0);

    // Central burst sphere
    float bSize = 0.12 * (1.0 + audioScale * 0.4);
    float dBurst = sdSphere(p, bSize);
    if (dBurst < res.x) res = vec2(dBurst, 0.0);

    float count = floor(shardCount);
    for (float i = 0.0; i < 20.0; i++) {
        if (i >= count) break;

        // Random per-shard variation
        float rVar = 0.8 + h11(i * 5.17 + 1.0) * 0.4;  // 0.8..1.2 radial variation
        float dist = burstRadius * rVar * (1.0 + breathe);

        // Slight individual phase offset in breathing
        float phaseOff = h11(i * 3.31) * PI * 2.0;
        dist += sin(TIME * 0.3 + phaseOff) * 0.06 * burstRadius;

        vec3 dir    = shardDir(i);
        vec3 center = dir * dist;

        // Transform to shard-local space
        vec3 lp = p - center;

        // Brick dimensions: elongated along outward direction, random aspect
        float lenX = shardScale * (1.0 + h11(i * 7.13) * 0.8) * audioScale;
        float lenY = shardScale * (0.4 + h11(i * 11.7) * 0.4) * audioScale;
        float lenZ = shardScale * (0.3 + h11(i * 13.3) * 0.5) * audioScale;

        // Rotate local point into shard frame (outward = local X axis)
        vec3 ax = shardDir(i);
        vec3 ay = shardRight(i);
        vec3 az = shardUp(i);
        vec3 pl = vec3(dot(lp, ax), dot(lp, ay), dot(lp, az));

        float d = sdBox(pl, vec3(lenX, lenY, lenZ));
        if (d < res.x) res = vec2(d, i + 1.0);
    }

    return res;
}

// ---- normal via central differences ----------------------------------
vec3 calcNormal(vec3 p, float audioScale) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        mapScene(p + e.xyy, audioScale).x - mapScene(p - e.xyy, audioScale).x,
        mapScene(p + e.yxy, audioScale).x - mapScene(p - e.yxy, audioScale).x,
        mapScene(p + e.yyx, audioScale).x - mapScene(p - e.yyx, audioScale).x
    ));
}

// ---- raymarch --------------------------------------------------------
vec2 march(vec3 ro, vec3 rd, float audioScale) {
    float t = 0.02;
    vec2 res = vec2(-1.0);
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * t;
        vec2 d = mapScene(p, audioScale);
        if (d.x < SURF_DIST) { res = vec2(t, d.y); break; }
        if (t > MAX_DIST) break;
        t += max(d.x * 0.55, SURF_DIST * 2.0);
    }
    return res;
}

// ---- radial heat-haze glow (additive, no transparency) ---------------
vec3 glowField(vec3 ro, vec3 rd, float audioScale) {
    // Soft volumetric glow by marching and accumulating proximity to origin
    float glow = 0.0;
    float t    = 0.05;
    float burstGlowRadius = 0.6 * (1.0 + audioScale * 0.5);
    for (int i = 0; i < 24; i++) {
        vec3 p   = ro + rd * t;
        float d  = length(p);
        glow    += exp(-max(d - burstGlowRadius, 0.0) * 5.0) * 0.05;
        t       += 0.15;
        if (t > 5.0) break;
    }
    // Warm core glow color: orange-crimson
    return vec3(1.2, 0.35, 0.0) * glow;
}

// ---- shading ---------------------------------------------------------
vec3 shadeShard(vec3 p, vec3 n, float id, vec3 ro) {
    // Direction from shard center back to burst origin
    float fid   = id - 1.0;
    vec3 shardCenter = shardDir(fid) * burstRadius;
    vec3 toBurst = normalize(-shardCenter);   // points inward toward origin

    // How much this fragment faces the burst (inner = hot crimson)
    float inner = clamp(dot(n, toBurst), 0.0, 1.0);
    // How much faces the camera (outer/ambient = amber)
    vec3 vDir   = normalize(ro - p);
    float outer = clamp(dot(n, vDir), 0.0, 1.0);

    // Palette blend: inner face → crimson, outer face → amber
    vec3 baseCol = mix(COL_OUTER, COL_INNER, inner * inner);

    // Edge glint using fwidth-based rim
    float edgeRim = 1.0 - smoothstep(0.0, 0.35, outer);
    baseCol = mix(baseCol, COL_GLINT, edgeRim * 0.7);

    // Hard shadow-like: faces directly away from burst are darkened (ink black)
    float shadowFac = smoothstep(-0.3, 0.2, dot(n, normalize(vec3(1.0, 1.0, 0.5))));
    baseCol = mix(COL_BLACK, baseCol, 0.15 + shadowFac * 0.85);

    // Specular highlight from an imaginary key light upper-right
    vec3 keyLight = normalize(vec3(1.2, 1.5, -0.8));
    vec3 halfV    = normalize(keyLight + vDir);
    float spec    = pow(max(dot(n, halfV), 0.0), 64.0);
    baseCol      += COL_GLINT * spec * 1.8;

    return baseCol;
}

// ---- main ------------------------------------------------------------
void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    // Audio modulator — always 1.0 baseline, never a gate
    float audioLevel = audioBass;
    float audio      = 1.0 + audioMod * audioLevel * 1.5;
    // audioScale modulates shard scale and burst glow
    float audioScale = audio;

    // Camera: slow orbit around the shatter burst
    float camAngle = TIME * orbitSpeed;
    float camH     = 0.35 + sin(TIME * orbitSpeed * 0.4) * 0.2;
    float camDist  = 4.5;
    vec3 ro = vec3(cos(camAngle) * camDist, camH * camDist, sin(camAngle) * camDist);
    vec3 target  = vec3(0.0, 0.0, 0.0);
    vec3 forward = normalize(target - ro);
    vec3 right   = normalize(cross(forward, vec3(0.0, 1.0, 0.0)));
    vec3 upV     = cross(right, forward);
    vec3 rd      = normalize(uv.x * right + uv.y * upV + 1.8 * forward);

    // March
    vec2 hit = march(ro, rd, audioScale);

    vec3 col = COL_BLACK;

    if (hit.x > 0.0) {
        vec3  p  = ro + rd * hit.x;
        float id = hit.y;

        if (id < 0.5) {
            // Central burst sphere — white-hot HDR
            vec3 n = normalize(p);
            vec3 vDir = normalize(ro - p);
            float fresnel = pow(1.0 - max(dot(n, vDir), 0.0), 3.0);
            col = COL_BURST * (1.5 + fresnel * 2.0 * audioScale);
        } else {
            // Shard fragment
            vec3 n = calcNormal(p, audioScale);
            col    = shadeShard(p, n, id, ro);

            // fwidth AA: soften silhouette edges
            float fw = length(vec2(dFdx(hit.x), dFdy(hit.x)));
            float edgeAA = smoothstep(0.0, fw * 2.0, SURF_DIST + 0.001);
            col *= edgeAA;
        }

        // Distance-based darkening toward black for depth
        float depthFog = 1.0 - clamp((hit.x - 2.0) / 8.0, 0.0, 0.65);
        col *= depthFog;
    }

    // Additive heat-haze glow around burst (no transparency, just accumulate)
    col += glowField(ro, rd, audioScale);

    // Ink-black background — output linear HDR, no ACES, no gamma, no clamp
    gl_FragColor = vec4(col, 1.0);
}
