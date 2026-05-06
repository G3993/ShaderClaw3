/*{
    "DESCRIPTION": "Neon Crystal Cave — raymarched cave interior with glowing gem-crystal formations emitting HDR colored light",
    "CREDIT": "ShaderClaw auto-improve",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "crystalColor1", "LABEL": "Crystal A", "TYPE": "color", "DEFAULT": [1.0, 0.05, 0.3, 1.0] },
        { "NAME": "crystalColor2", "LABEL": "Crystal B", "TYPE": "color", "DEFAULT": [0.0, 0.8, 1.0, 1.0] },
        { "NAME": "crystalColor3", "LABEL": "Crystal C", "TYPE": "color", "DEFAULT": [1.0, 0.7, 0.0, 1.0] },
        { "NAME": "crystalColor4", "LABEL": "Crystal D", "TYPE": "color", "DEFAULT": [0.6, 0.0, 1.0, 1.0] },
        { "NAME": "glowScale",     "LABEL": "Glow",     "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 5.0 },
        { "NAME": "camSpeed",      "LABEL": "Cam Speed","TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "crystalCount",  "LABEL": "Crystals", "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 12.0 },
        { "NAME": "audioMod",      "LABEL": "Audio",    "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 }
    ]
}*/

#define PI  3.14159265
#define TAU 6.28318530
#define MAX_STEPS 72
#define MAX_DIST  18.0
#define SURF_DIST 0.003

float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

// Elongated octahedron — crystal spire shape
float sdCrystal(vec3 p, float h, float r) {
    p.y -= h * 0.5;
    vec3 q = abs(p);
    float e = max(q.x + q.y + q.z - r, q.y - h);
    return e * 0.57735;
}

float sdCave(vec3 p) {
    float fl  = p.y + 1.5;
    float ceil = -(p.y - 3.0);
    float walls = length(vec2(p.x, p.z)) - 4.2;
    return min(min(fl, ceil), walls);
}

struct Hit { float d; int id; };

Hit scene(vec3 p, float audioBoost) {
    int N = int(clamp(crystalCount, 2.0, 12.0));
    float best = sdCave(p);
    int bestId = 0;
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);
        float ang = TAU * fi / float(N) + TIME * 0.04;
        float rad = 1.4 + 0.9 * hash(fi * 3.7);
        float s = 0.3 + 0.6 * hash(fi * 7.13) + 0.08 * audioBoost;
        float baseY = -1.5;
        if (i % 3 == 2) baseY = 3.0 - s * 0.8; // ceiling stalactites
        vec3 base = vec3(cos(ang) * rad, baseY, sin(ang) * rad);
        // flip ceiling crystals
        vec3 q = p - base;
        if (i % 3 == 2) q.y = -q.y;
        float d = sdCrystal(q, s * 1.6, s * 0.30);
        if (d < best) { best = d; bestId = i + 1; }
    }
    return Hit(best, bestId);
}

vec3 calcNormal(vec3 p, float ab) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, ab).d - scene(p - e.xyy, ab).d,
        scene(p + e.yxy, ab).d - scene(p - e.yxy, ab).d,
        scene(p + e.yyx, ab).d - scene(p - e.yyx, ab).d
    ));
}

vec3 crystalHDR(int id, float ab) {
    int idx = (id - 1) % 4;
    vec3 c;
    if      (idx == 0) c = crystalColor1.rgb;
    else if (idx == 1) c = crystalColor2.rgb;
    else if (idx == 2) c = crystalColor3.rgb;
    else               c = crystalColor4.rgb;
    return c * glowScale * (1.0 + ab * audioMod * 0.6);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / RENDERSIZE.y;

    float audioBoost = 0.0;
#ifdef AUDIOBASS
    audioBoost += audioBass * audioMod;
#endif
#ifdef AUDIOHIGH
    audioBoost += audioHigh * audioMod * 0.5;
#endif

    float t = TIME * camSpeed;
    float camAng = t * 0.65;
    vec3 ro = vec3(cos(camAng) * 2.0, 0.2 + 0.5 * sin(t * 0.29), sin(camAng) * 2.0);
    vec3 fwd   = normalize(-ro + vec3(0.0, 0.1, 0.0));
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(uv.x * right + uv.y * up + 1.6 * fwd);

    float dist = 0.0;
    int hitId  = 0;
    bool hit   = false;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * dist;
        Hit h = scene(p, audioBoost);
        if (h.d < SURF_DIST) { hitId = h.id; hit = true; break; }
        if (dist > MAX_DIST) break;
        dist += max(h.d, SURF_DIST);
    }

    vec3 col = vec3(0.0);

    if (hit) {
        vec3 p = ro + rd * dist;
        vec3 n = calcNormal(p, audioBoost);
        int N = int(clamp(crystalCount, 2.0, 12.0));

        if (hitId == 0) {
            // Cave wall lit by crystal point-lights
            vec3 wallCol = vec3(0.015, 0.0, 0.03);
            for (int i = 0; i < 12; i++) {
                if (i >= N) break;
                float fi = float(i);
                float ang = TAU * fi / float(N) + TIME * 0.04;
                float rad = 1.4 + 0.9 * hash(fi * 3.7);
                vec3 lpos = vec3(cos(ang) * rad, 0.0, sin(ang) * rad);
                vec3 lc = crystalHDR(i + 1, audioBoost);
                float falloff = 1.0 / (1.0 + dot(p - lpos, p - lpos) * 0.5);
                float diff = max(0.0, dot(n, normalize(lpos - p)));
                wallCol += lc * diff * falloff * 0.28;
            }
            col = wallCol;
        } else {
            // Crystal emissive core
            vec3 em = crystalHDR(hitId, audioBoost);
            float fresnel = pow(1.0 - abs(dot(n, -rd)), 2.5);
            // Black ink silhouette on grazing angles
            float ink = smoothstep(0.0, 0.25, 1.0 - fresnel);
            col = em * (0.55 + 0.45 * fresnel) * ink;
        }
    } else {
        col = vec3(0.008, 0.0, 0.016);
    }

    // Volumetric glow halos from crystal centers along ray
    int N2 = int(clamp(crystalCount, 2.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= N2) break;
        float fi = float(i);
        float ang = TAU * fi / float(N2) + TIME * 0.04;
        float rad = 1.4 + 0.9 * hash(fi * 3.7);
        vec3 lpos = vec3(cos(ang) * rad, 0.0, sin(ang) * rad);
        vec3 lc = crystalHDR(i + 1, audioBoost);
        float tl = clamp(dot(lpos - ro, rd), 0.0, hit ? dist : MAX_DIST);
        vec3 cp  = ro + rd * tl;
        float r2 = dot(cp - lpos, cp - lpos);
        col += lc * exp(-r2 * 4.0) * 0.10;
    }

    gl_FragColor = vec4(col, 1.0);
}
