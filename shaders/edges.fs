/*{
  "DESCRIPTION": "Electric Storm — forking lightning bolt SDF columns in 3D void. Painterly dramatic lighting, HDR plasma cores.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "boltCount",  "LABEL": "Bolt Count",  "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0,  "MAX": 8.0 },
    { "NAME": "branchAmt",  "LABEL": "Branches",    "TYPE": "float", "DEFAULT": 3.0,  "MIN": 0.0,  "MAX": 6.0 },
    { "NAME": "stormSpeed", "LABEL": "Storm Speed",  "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0 },
    { "NAME": "coreColor",  "LABEL": "Core Color",   "TYPE": "color", "DEFAULT": [1.0, 0.95, 0.5, 1.0] },
    { "NAME": "arcColor",   "LABEL": "Arc Color",    "TYPE": "color", "DEFAULT": [0.2, 0.5, 1.0, 1.0] },
    { "NAME": "audioMod",   "LABEL": "Audio Mod",    "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 72
#define SURF_DIST 0.003
#define MAX_DIST  12.0
#define PI 3.14159265

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// Capsule (bolt segment) SDF
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b - a;
    float t = clamp(dot(p - a, ab) / dot(ab, ab), 0.0, 1.0);
    return length(p - a - ab * t) - r;
}

// Jagged lightning bolt: 8 segments with random jags
float boltSDF(vec3 p, float seed, float t, float radius) {
    vec3 a = vec3((hash11(seed) - 0.5) * 1.6, 2.2, (hash11(seed + 1.0) - 0.5) * 1.0);
    float d = MAX_DIST;
    for (int i = 0; i < 8; i++) {
        float fi = float(i);
        float step = 4.4 / 8.0;
        float jx = (hash11(seed + fi * 3.7 + t * 0.3) - 0.5) * 0.55;
        float jz = (hash11(seed + fi * 5.1 + t * 0.3) - 0.5) * 0.35;
        vec3 b = a + vec3(jx, -step, jz);
        d = min(d, sdCapsule(p, a, b, radius));
        a = b;
    }
    return d;
}

// Side branches off main bolt
float branchSDF(vec3 p, float seed, float t, float radius) {
    float d = MAX_DIST;
    float nb = floor(clamp(branchAmt, 0.0, 6.0));
    for (int b = 0; b < 6; b++) {
        if (float(b) >= nb) break;
        float fb = float(b);
        float brSeed = seed + fb * 17.3;
        float sy = 1.8 - fb * 0.55;
        vec3 ba = vec3((hash11(brSeed) - 0.5) * 1.0, sy, (hash11(brSeed + 1.0) - 0.5) * 0.6);
        for (int i = 0; i < 4; i++) {
            float fi = float(i);
            float jx = (hash11(brSeed + fi * 4.3 + t * 0.25) - 0.5) * 0.5;
            float jy = -0.3 - hash11(brSeed + fi * 2.1) * 0.2;
            float jz = (hash11(brSeed + fi * 6.7 + t * 0.25) - 0.5) * 0.4;
            vec3 bb = ba + vec3(jx, jy, jz);
            d = min(d, sdCapsule(p, ba, bb, radius * 0.55));
            ba = bb;
        }
    }
    return d;
}

float scene(vec3 p, float t) {
    float d = MAX_DIST;
    float nb = floor(clamp(boltCount, 1.0, 8.0));
    for (int i = 0; i < 8; i++) {
        if (float(i) >= nb) break;
        float seed = float(i) * 23.7 + floor(t * stormSpeed * 0.5) * 3.1;
        d = min(d, boltSDF(p, seed, t, 0.025));
        d = min(d, branchSDF(p, seed, t, 0.025));
    }
    return d;
}

vec3 calcNormal(vec3 p, float t) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, t) - scene(p - e.xyy, t),
        scene(p + e.yxy, t) - scene(p - e.yxy, t),
        scene(p + e.yyx, t) - scene(p - e.yyx, t)
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.6;
    float t = TIME;

    // Camera looks up into the storm from below-front
    float camSwing = sin(t * stormSpeed * 0.2) * 0.3;
    vec3 ro = vec3(camSwing, -1.8, 4.5);
    vec3 target = vec3(0.0, 0.5, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 upV = cross(fwd, rgt);
    vec3 rd  = normalize(fwd + uv.x * rgt * 0.85 + uv.y * upV * 0.85);

    // Dark storm sky background with subtle cloud texture
    float skyGrad = smoothstep(-0.3, 1.0, uv.y * 0.5 + 0.5);
    vec3 col = mix(vec3(0.01, 0.01, 0.03), vec3(0.03, 0.02, 0.08), skyGrad);
    float cx = sin(uv.x * 3.1 + t * 0.07) * sin(uv.y * 2.3 - t * 0.05);
    col += vec3(0.02, 0.015, 0.04) * smoothstep(-0.2, 0.5, cx);

    // Raymarch
    float tHit = -1.0;
    float dist = 0.05;
    for (int i = 0; i < MAX_STEPS; i++) {
        float d = scene(ro + rd * dist, t);
        if (d < SURF_DIST) { tHit = dist; break; }
        if (dist > MAX_DIST) break;
        dist += max(d * 0.6, 0.005);
    }

    if (tHit > 0.0) {
        vec3 p = ro + rd * tHit;
        vec3 n = calcNormal(p, t);

        // Plasma core: volt yellow → white hot
        float core = pow(max(0.0, dot(-rd, n)), 1.2);
        vec3 coreC = coreColor.rgb * hdrPeak * audio;

        // Arc glow: electric blue corona
        float arcGlow = pow(max(0.0, 1.0 - max(0.0, dot(-rd, n))), 3.0);
        vec3 arcC = arcColor.rgb * arcGlow * hdrPeak * 0.7 * audio;

        // Black ink silhouette at thin angles
        float ink = smoothstep(0.0, 0.25, core);

        col = (coreC * core + arcC) * ink;

        float fw = fwidth(scene(p, t));
        float aa = smoothstep(fw * 2.0, 0.0, abs(scene(p, t)));
        col *= aa;
    }

    // Ambient glow from bolt positions bleeding into bg
    float nb = floor(clamp(boltCount, 1.0, 8.0));
    for (int i = 0; i < 8; i++) {
        if (float(i) >= nb) break;
        float seed = float(i) * 23.7 + floor(t * stormSpeed * 0.5) * 3.1;
        float bx = (hash11(seed) - 0.5) * 1.6 / 4.5;
        float glowDist = abs(uv.x - bx) * 3.0 + abs(uv.y - 0.2) * 2.0;
        float ambGlow = exp(-glowDist * glowDist * 4.0);
        col += arcColor.rgb * ambGlow * hdrPeak * 0.35 * audio;
    }

    FragColor = vec4(col, 1.0);
}
