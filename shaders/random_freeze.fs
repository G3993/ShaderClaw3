/*{
  "DESCRIPTION": "Plasma Torus Array — neon plasma rings floating in a dark void, orbiting a central axis. Fully saturated neon palette, HDR glow cores.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "torusCount", "LABEL": "Torus Count", "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0, "MAX": 8.0 },
    { "NAME": "rotSpeed",   "LABEL": "Orbit Speed", "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "plasmaFreq", "LABEL": "Plasma Freq", "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0, "MAX": 12.0 },
    { "NAME": "tubeRadius", "LABEL": "Tube Radius", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.04, "MAX": 0.3 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioMod",   "LABEL": "Audio Mod",   "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define SURF_DIST 0.002
#define MAX_DIST  10.0
#define PI 3.14159265
#define TAU 6.28318530

float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Torus SDF
float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

// Rotate around Y axis
mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

float scene(vec3 p) {
    float d = MAX_DIST;
    float nb = floor(clamp(torusCount, 1.0, 8.0));
    float t = TIME * rotSpeed;

    for (int i = 0; i < 8; i++) {
        if (float(i) >= nb) break;
        float fi = float(i);
        float seed = fi * 7.31;

        // Each torus has its own orbit: radius, tilt, and angular phase
        float orbitR = 0.5 + fi * 0.38;
        float tilt   = hash(seed + 1.0) * PI * 0.6 - PI * 0.3;
        float phase  = fi * (TAU / 5.0);

        // Position on orbit
        float angle = t + phase;
        vec3 center = vec3(cos(angle) * orbitR * 0.0, 0.0, 0.0);
        // Tilt torus plane
        vec3 lp = p - center;
        float ct = cos(tilt), st = sin(tilt);
        lp = vec3(lp.x, ct * lp.y - st * lp.z, st * lp.y + ct * lp.z);
        // Spin torus around Y
        float spin = t * (0.4 + hash(seed + 2.0) * 0.8) + phase;
        lp.xz = rot2(spin) * lp.xz;

        // Plasma modulation: tube radius oscillates with torus angle
        float torusAngle = atan(lp.z, lp.x - (length(vec2(lp.x, lp.z)) - orbitR));
        float plasmaRad = tubeRadius * (0.7 + 0.3 * sin(torusAngle * plasmaFreq + TIME * 3.0 + fi));

        d = min(d, sdTorus(lp, orbitR, plasmaRad));
    }
    return d;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        scene(p + e.xyy) - scene(p - e.xyy),
        scene(p + e.yxy) - scene(p - e.yxy),
        scene(p + e.yyx) - scene(p - e.yyx)
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;
    float t = TIME;

    // Slowly orbiting camera
    float camT = t * 0.07;
    vec3 ro = vec3(sin(camT) * 3.8, 0.9 + sin(camT * 0.4) * 0.5, cos(camT) * 3.8);
    vec3 target = vec3(0.0);
    vec3 fwd = normalize(target - ro);
    vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 upV = cross(fwd, rgt);
    vec3 rd  = normalize(fwd + uv.x * rgt + uv.y * upV);

    // Void background: near-black with subtle radial plasma nebula
    float bgGlow = exp(-length(uv) * 1.5);
    vec3 col = vec3(0.0, 0.0, 0.01) + vec3(0.04, 0.0, 0.08) * bgGlow;
    // Subtle background plasma streaks
    float streak = sin(uv.x * 8.0 + t * 0.3) * sin(uv.y * 5.0 + t * 0.2);
    col += vec3(0.01, 0.0, 0.03) * smoothstep(0.4, 0.8, streak);

    // Raymarch
    float dist = 0.05;
    float tHit = -1.0;
    int hitIdx = -1;
    float nb = floor(clamp(torusCount, 1.0, 8.0));

    for (int i = 0; i < MAX_STEPS; i++) {
        float d = scene(ro + rd * dist);
        if (d < SURF_DIST) { tHit = dist; break; }
        if (dist > MAX_DIST) break;
        dist += max(d * 0.7, 0.004);
    }

    if (tHit > 0.0) {
        vec3 p = ro + rd * tHit;
        vec3 n = calcNormal(p);

        // Identify which torus was hit by closest distance
        float minD = MAX_DIST;
        float hitI = 0.0;
        for (int i = 0; i < 8; i++) {
            if (float(i) >= nb) break;
            // Re-evaluate per-torus to find hue
            float fi = float(i);
            float seed = fi * 7.31;
            float orbitR = 0.5 + fi * 0.38;
            float tilt = hash(seed + 1.0) * PI * 0.6 - PI * 0.3;
            float phase = fi * (TAU / 5.0);
            float angle = t * rotSpeed + phase;
            vec3 lp = p;
            float ct = cos(tilt), st = sin(tilt);
            lp = vec3(lp.x, ct * lp.y - st * lp.z, st * lp.y + ct * lp.z);
            float spin = t * rotSpeed * (0.4 + hash(seed + 2.0) * 0.8) + phase;
            lp.xz = rot2(spin) * lp.xz;
            float torusAngle = atan(lp.z, lp.x - (length(vec2(lp.x, lp.z)) - orbitR));
            float plasmaRad = tubeRadius * (0.7 + 0.3 * sin(torusAngle * plasmaFreq + TIME * 3.0 + fi));
            float dd = sdTorus(lp, orbitR, plasmaRad);
            if (dd < minD) { minD = dd; hitI = fi; }
        }

        // Hue cycles through: magenta→cyan→gold→green→violet
        float hue = fract(hitI / max(nb, 1.0) + t * 0.04);
        vec3 plasmaCol = hsv2rgb(vec3(hue, 1.0, 1.0));

        // Core: white-hot center, plasma color towards edge
        float core = pow(max(0.0, dot(-rd, n)), 1.5);
        vec3 surf = plasmaCol * hdrPeak * audio;
        surf = mix(surf, vec3(hdrPeak * audio), core * 0.6); // white-hot center

        // Plasma pulse: brightness oscillates with TIME
        float pulse = 0.85 + 0.15 * sin(TIME * plasmaFreq * 0.8 + hitI);
        surf *= pulse;

        // Black ink edge at silhouette
        float ink = smoothstep(0.0, 0.3, core);
        surf *= ink;

        // fwidth AA
        float fw = fwidth(scene(p));
        float aa = smoothstep(fw * 2.0, 0.0, abs(scene(p)));
        col = mix(col, surf, aa);
    }

    // Glow halos around torus orbits bleeding into BG
    for (int i = 0; i < 8; i++) {
        if (float(i) >= nb) break;
        float fi = float(i);
        float seed = fi * 7.31;
        float hue = fract(fi / nb + t * 0.04);
        vec3 haloCol = hsv2rgb(vec3(hue, 1.0, 1.0));
        float orbitR = 0.5 + fi * 0.38;
        // Project orbit center onto screen approximately
        float screenR = orbitR / 3.8;
        float phase = fi * (TAU / 5.0);
        float screenX = cos(t * rotSpeed * 0.07 + phase) * screenR * 0.5;
        float glowD = length(uv - vec2(screenX, 0.0)) - screenR * 0.3;
        float halo = exp(-max(0.0, glowD) * 8.0) * 0.3;
        col += haloCol * halo * hdrPeak * 0.3 * audio;
    }

    FragColor = vec4(col, 1.0);
}
