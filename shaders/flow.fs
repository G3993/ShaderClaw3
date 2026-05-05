/*{
  "DESCRIPTION": "Gravity Streams 3D — plasma orbs on 3D orbital paths with volumetric neon trails and HDR cinematic lighting.",
  "CATEGORIES": ["Generator", "Simulation", "3D"],
  "INPUTS": [
    { "NAME": "orbitSpeed",  "LABEL": "Speed",       "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "orbitChaos",  "LABEL": "Chaos",       "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glowAmount",  "LABEL": "Glow",        "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "trailLength", "LABEL": "Trail",       "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioDrive",  "LABEL": "Audio Drive", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "bgColor",     "LABEL": "Background",  "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.01, 1.0] },
    { "NAME": "transparentBg","LABEL":"Transparent", "TYPE": "bool",  "DEFAULT": false }
  ]
}*/

// ─── Gravity Streams 3D ──────────────────────────────────────────────────────
// 8 plasma orbs on layered Lissajous orbits in 3D space.
// Volumetric glow trails computed analytically from sampled past positions —
// no persistent buffers. Linear HDR output; host applies soft-knee tonemap.
// ─────────────────────────────────────────────────────────────────────────────

#define N_ORBS        8
#define STEPS         64
#define TRAIL_STEPS   14
#define PI            3.14159265359

float hash(float n)  { return fract(sin(n * 127.1) * 43758.5453); }

// 4-color saturated neon palette — blue, magenta, gold, cyan
vec3 orbColor(int i) {
    int c = int(mod(float(i), 4.0));
    if (c == 0) return vec3(0.0,  0.55, 3.0);   // electric blue
    if (c == 1) return vec3(3.0,  0.0,  1.2);   // hot magenta
    if (c == 2) return vec3(2.8,  1.4,  0.0);   // acid gold
    return           vec3(0.0,  2.8,  0.9);     // cyan-teal
}

// 3D orbital position — Lissajous + attractor perturbation
vec3 orbPos(int id, float t) {
    float fi = float(id);
    float s1 = hash(fi * 7.13), s2 = hash(fi * 3.71);
    float s3 = hash(fi * 11.37), s4 = hash(fi * 5.91);

    float bFreq = 0.22 + s1 * 0.30;
    float bPhase = s2 * PI * 2.0;
    float bR   = 2.2 + s3 * 2.2;
    float wFreq = 0.65 + s4 * 1.2;
    float wAmt  = (0.14 + s1 * 0.22) * (1.0 + orbitChaos * 2.5);

    // Two slow attractors in opposition
    float at = t * 0.12;
    vec3 a1 = vec3(sin(at) * 0.9, cos(at * 1.3) * 0.6, sin(at * 0.8) * 0.7);
    vec3 a2 = -a1;
    vec3 att = mod(fi, 2.0) < 1.0 ? a1 : a2;

    return att + vec3(
        cos(t * bFreq + bPhase)         * bR         + sin(t * wFreq + s3*5.0) * wAmt,
        sin(t * bFreq * 0.83 + bPhase + PI*0.5) * bR * 0.72 + cos(t * wFreq * 0.9 + s1*5.0) * wAmt * 0.55,
        cos(t * bFreq * 1.15 + s4*PI)  * bR * 0.65  + sin(t * wFreq * 0.72 + s2*4.0) * wAmt
    );
}

// SDF scene — all orbs as spheres, audio-pulsed radius
float sceneSDF(vec3 p, float t, out int hitId) {
    float d = 1e10;
    hitId = -1;
    for (int i = 0; i < N_ORBS; i++) {
        vec3 pos = orbPos(i, t);
        float pulse = 1.0 + audioBass * audioDrive * 0.22 * hash(float(i) * 11.3);
        float r = (0.15 + hash(float(i) * 3.7) * 0.07) * pulse;
        float di = length(p - pos) - r;
        if (di < d) { d = di; hitId = i; }
    }
    return d;
}

vec3 calcNormal(vec3 p, float t) {
    int dummy;
    vec2 e = vec2(0.003, 0.0);
    return normalize(vec3(
        sceneSDF(p + e.xyy, t, dummy) - sceneSDF(p - e.xyy, t, dummy),
        sceneSDF(p + e.yxy, t, dummy) - sceneSDF(p - e.yxy, t, dummy),
        sceneSDF(p + e.yyx, t, dummy) - sceneSDF(p - e.yyx, t, dummy)
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Slow-orbiting camera
    float camA = TIME * 0.065;
    float camDist = 10.0;
    vec3 ro = vec3(sin(camA) * camDist, 3.5 + sin(camA * 0.6) * 1.8, cos(camA) * camDist);
    vec3 fwd   = normalize(-ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up    = cross(right, fwd);
    vec3 rd    = normalize(fwd * 1.35 + right * uv.x + up * uv.y);

    float t = TIME * orbitSpeed;

    // ── Raymarch ────────────────────────────────────────────────────────────
    float dist = 0.0;
    bool hit = false;
    int hitOrb = -1;
    vec3 hitP;
    int dummy;

    for (int i = 0; i < STEPS; i++) {
        vec3 p = ro + rd * dist;
        float d = sceneSDF(p, t, dummy);
        if (d < 0.004) { hit = true; hitP = p; break; }
        dist += d;
        if (dist > 28.0) break;
    }

    // Identify which orb was hit
    if (hit) {
        float bestD = 1e10;
        for (int i = 0; i < N_ORBS; i++) {
            vec3 pos = orbPos(i, t);
            float pulse = 1.0 + audioBass * audioDrive * 0.22 * hash(float(i) * 11.3);
            float r = (0.15 + hash(float(i) * 3.7) * 0.07) * pulse;
            float di = abs(length(hitP - pos) - r);
            if (di < bestD) { bestD = di; hitOrb = i; }
        }
    }

    // ── Volumetric trail accumulation ───────────────────────────────────────
    // For each orb, sample past positions and accumulate glow along the ray.
    // This creates luminous tube-like trails without persistent buffers.
    vec3 col = bgColor.rgb;
    float TRAIL_DT = 0.10 * trailLength;

    for (int i = 0; i < N_ORBS; i++) {
        vec3 oc = orbColor(i);

        // Halo from current position
        {
            vec3 pos = orbPos(i, t);
            vec3 toOrb = pos - ro;
            float proj = dot(toOrb, rd);
            if (proj > 0.0) {
                float perp2 = dot(toOrb, toOrb) - proj * proj;
                float halo = exp(-perp2 * 1.2) * glowAmount;
                col += oc * halo * 1.8;
            }
        }

        // Trail: past positions, exponentially fading
        for (int s = 1; s <= TRAIL_STEPS; s++) {
            float tOff  = float(s) * TRAIL_DT;
            float fade  = pow(1.0 - float(s) / float(TRAIL_STEPS + 1), 2.2);
            vec3 pos    = orbPos(i, t - tOff);
            vec3 toOrb  = pos - ro;
            float proj  = dot(toOrb, rd);
            if (proj < 0.0) continue;
            float perp2 = dot(toOrb, toOrb) - proj * proj;
            float glow  = exp(-perp2 * 3.5) * fade * glowAmount;
            col += oc * glow * 0.55;
        }
    }

    // ── Surface shading if hit ───────────────────────────────────────────────
    if (hit && hitOrb >= 0) {
        vec3 n  = calcNormal(hitP, t);
        vec3 v  = normalize(ro - hitP);
        vec3 oc = orbColor(hitOrb);

        // Cinematic two-light setup
        vec3 L1 = normalize(vec3(2.5, 3.5, 1.5));
        vec3 L2 = normalize(vec3(-1.2, -0.5, 2.0));
        float diff1 = max(dot(n, L1), 0.0);
        float diff2 = max(dot(n, L2), 0.0) * 0.35;

        vec3 H1 = normalize(L1 + v);
        float spec = pow(max(dot(n, H1), 0.0), 28.0);
        float fres = pow(1.0 - max(dot(n, v), 0.0), 2.0);

        // HDR surface — specular 3.0+, fresnel rim 2.5+
        vec3 surf = oc * (diff1 + diff2 + 0.15);
        surf += vec3(2.5, 2.5, 3.0) * spec * 3.5;         // white-hot specular peak
        surf += oc * fres * 2.8;                           // HDR rim glow
        surf += oc * audioBass * audioDrive * 0.6;         // audio pulse

        // Orb surface dominates but absorbs some background trail
        col = surf + col * 0.08;
    }

    // ── Surprise: every ~22s one orb "goes nova" for ~0.4s ──────────────────
    {
        float _ph = fract(TIME / 22.0);
        float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.22, 0.12, _ph);
        int   _novaOrb = 3;
        vec3  _novaPos = orbPos(_novaOrb, t);
        vec3  _toNova  = _novaPos - ro;
        float _proj    = dot(_toNova, rd);
        if (_proj > 0.0) {
            float _perp2 = dot(_toNova, _toNova) - _proj * _proj;
            float _burst = exp(-_perp2 * 0.15) * _f * 4.0;
            col += orbColor(_novaOrb) * _burst;
        }
    }

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.0, 0.12, length(col - bgColor.rgb));

    gl_FragColor = vec4(col, alpha);
}
