/*{
  "DESCRIPTION": "Bismuth Crystal Cavity — 3D SDF raymarched iridescent bismuth crystal steps inside a dark cave. Rainbow metallic surfaces, deep shadow silhouettes.",
  "CREDIT": "ShaderClaw auto-improve v4",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "crystalCount", "LABEL": "Crystal Count", "TYPE": "float", "DEFAULT": 5.0, "MIN": 2.0, "MAX": 10.0 },
    { "NAME": "rotSpeed",     "LABEL": "Orbit Speed",   "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "hdrPeak",      "LABEL": "HDR Peak",      "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioReact",   "LABEL": "Audio",         "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// ─────────────────────────────────────────────────────────
// Utilities
// ─────────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// ─────────────────────────────────────────────────────────
// SDF primitives
// ─────────────────────────────────────────────────────────
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float sdSphere(vec3 p, float r) {
    return length(p) - r;
}

// One bismuth crystal: 5 stair-stepped boxes, each narrower and higher
float sdCrystal(vec3 p, float sz) {
    float d = 1e9;
    float stepH = 0.20;
    for (int j = 0; j < 5; j++) {
        float fj = float(j);
        float scale = 1.0 - fj * 0.15;
        vec3 boxSize = vec3(sz * scale, stepH * 0.5, sz * scale);
        vec3 center  = vec3(0.0, fj * stepH, 0.0);
        d = min(d, sdBox(p - center, boxSize));
    }
    return d;
}

// ─────────────────────────────────────────────────────────
// Scene — returns vec2(dist, materialID)   0=cave  1=crystal
// ─────────────────────────────────────────────────────────
vec2 sceneMap(vec3 pos, float t, float numCrystals, float audioScale) {
    // Cave interior: large sphere negated (inside surface)
    float cave = -sdSphere(pos, 4.5);

    float crystalDist = 1e9;
    float PI = 3.14159265;
    for (int i = 0; i < 10; i++) {
        if (float(i) >= numCrystals) break;
        float fi    = float(i);
        // Spread crystals in a ring on the cave floor
        float angle  = fi / numCrystals * 2.0 * PI + t * 0.05;
        float radius = 1.5 + hash11(fi * 3.71) * 0.9;
        vec3 center  = vec3(sin(angle) * radius,
                            -0.8 + hash11(fi * 7.13) * 0.5,
                            cos(angle) * radius);
        float sz = (0.20 + hash11(fi * 5.31) * 0.12) * audioScale;
        // Per-crystal Y-axis rotation for variety
        float cr   = fi * 1.1 + t * 0.07;
        float ccos = cos(cr), csin = sin(cr);
        vec3 lp    = pos - center;
        lp.xz = vec2(ccos * lp.x - csin * lp.z, csin * lp.x + ccos * lp.z);
        crystalDist = min(crystalDist, sdCrystal(lp, sz));
    }

    if (crystalDist < cave) {
        return vec2(crystalDist, 1.0);
    }
    return vec2(cave, 0.0);
}

vec3 calcNormal(vec3 p, float t, float numCrystals, float audioScale) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneMap(p + e.xyy, t, numCrystals, audioScale).x - sceneMap(p - e.xyy, t, numCrystals, audioScale).x,
        sceneMap(p + e.yxy, t, numCrystals, audioScale).x - sceneMap(p - e.yxy, t, numCrystals, audioScale).x,
        sceneMap(p + e.yyx, t, numCrystals, audioScale).x - sceneMap(p - e.yyx, t, numCrystals, audioScale).x
    ));
}

// ─────────────────────────────────────────────────────────
void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    float t          = TIME;
    float audioScale = 1.0 + (audioBass + audioMid) * audioReact * 0.25;
    float numCrystals = clamp(crystalCount, 2.0, 10.0);

    // Orbiting camera looking toward origin
    float orbitT = TIME * rotSpeed * (1.0 + audioMid * audioReact * 0.2);
    vec3 ro      = vec3(sin(orbitT) * 2.5, 0.5 + sin(orbitT * 0.37) * 0.3, cos(orbitT) * 2.5);
    vec3 target  = vec3(0.0, -0.3, 0.0);
    vec3 forward = normalize(target - ro);
    vec3 right   = normalize(cross(forward, vec3(0.0, 1.0, 0.0)));
    vec3 upV     = cross(right, forward);
    vec3 rd      = normalize(forward + uv.x * right + uv.y * upV);

    // 64-step raymarch
    float dist  = 0.01;
    float matID = -1.0;
    vec3 hitPos = ro;
    bool hit    = false;

    for (int i = 0; i < 64; i++) {
        vec3 p    = ro + rd * dist;
        vec2 res  = sceneMap(p, t, numCrystals, audioScale);
        float stp = res.x;
        if (stp < 0.001) {
            matID = res.y;
            hitPos = p;
            hit   = true;
            break;
        }
        dist += stp * 0.75;
        if (dist > 8.0) break;
    }

    vec3 col = vec3(0.0); // void black default

    if (hit) {
        vec3 N3    = calcNormal(hitPos, t, numCrystals, audioScale);
        vec3 V     = -rd;
        // Overhead point light
        vec3 lightPos = vec3(0.0, 3.5, 0.0);
        vec3 L     = normalize(lightPos - hitPos);
        float diff = max(dot(N3, L), 0.0);

        if (matID > 0.5) {
            // ── Bismuth crystal: iridescent coloring ──
            float hue   = dot(N3, vec3(1.0, 2.0, 3.0)) * 0.3
                        + fract(hitPos.y * 2.0)
                        + t * 0.1;
            vec3 iriCol = hsv2rgb(vec3(fract(hue), 0.9, 1.0)) * hdrPeak;

            // Diffuse shading on top of iridescence
            col = iriCol * (0.25 + diff * 0.75);

            // HDR specular peak 3.0
            vec3  R    = reflect(-L, N3);
            float spec = pow(max(dot(R, V), 0.0), 80.0);
            col += vec3(1.0) * spec * 3.0;

            // Coloured rim light to pop silhouette edges
            float rim = pow(1.0 - max(dot(N3, V), 0.0), 3.0);
            col += hsv2rgb(vec3(fract(hue + 0.5), 0.7, 1.0)) * rim * 0.9;

            // Crush near-black steps to true black (ink contrast)
            float shadow = clamp(diff * 2.5, 0.0, 1.0);
            col *= mix(0.0, 1.0, shadow);
        } else {
            // ── Cave wall: near-black deep-purple ambient only ──
            col = vec3(0.05, 0.03, 0.08) * 0.05 * (0.2 + diff * 0.8);
        }

        // Distance fog to pure black — enhances silhouette
        float fog = exp(-dist * 0.20);
        col *= fog;
    }

    gl_FragColor = vec4(col, 1.0);
}
