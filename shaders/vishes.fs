/*{
    "DESCRIPTION": "Coral Bioluminescence — 3D raymarched coral formation with bioluminescent glow. Deep ocean floor. Palette: void black ocean, bio-cyan, violet, deep teal coral. 64-step SDF march.",
    "CATEGORIES": ["Generator", "3D", "Bioluminescent", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "coralScale",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3, "MAX": 3.0,  "LABEL": "Coral Scale" },
        { "NAME": "glowRadius",  "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.1, "MAX": 1.5,  "LABEL": "Glow Radius" },
        { "NAME": "hdrPeak",     "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0, "MAX": 5.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",    "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash11(float n)  { return fract(sin(n*127.1)*43758.5453); }
float hash21(vec2 p)   { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
vec2  hash22(vec2 p)   { return fract(sin(vec2(dot(p,vec2(127.1,311.7)),dot(p,vec2(269.5,183.3))))*43758.5453); }

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b-a, ap = p-a;
    float h = clamp(dot(ap,ab)/dot(ab,ab), 0.0, 1.0);
    return length(ap - ab*h) - r;
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

// Coral branch SDF: recursive-like branching capsule tree
float coralBranch(vec3 p, vec3 base, float len, float rad, float seed) {
    float ang = hash11(seed * 3.17) * 1.4 - 0.7; // branch lean angle
    float pha = hash11(seed * 5.31) * 6.2832;
    vec3 dir = normalize(vec3(sin(ang)*cos(pha), cos(ang), sin(ang)*sin(pha)));
    vec3 tip  = base + dir * len;
    float d = sdCapsule(p, base, tip, rad);
    // Two sub-branches
    if (len > 0.07 * coralScale) {
        float nextLen = len * 0.65;
        float nextRad = rad * 0.65;
        d = min(d, coralBranch(p, tip, nextLen, nextRad, seed + 0.31));
        d = min(d, coralBranch(p, tip, nextLen, nextRad, seed + 0.73));
    }
    return d;
}

float sceneSDF(vec3 p) {
    float d = 1e6;
    // 5 coral colonies spread on floor
    for (int ci = 0; ci < 5; ci++) {
        float fci = float(ci);
        vec2 pos2d = hash22(vec2(fci, fci * 1.7)) * 3.0 - 1.5;
        pos2d *= coralScale;
        vec3 base = vec3(pos2d.x, -1.0, pos2d.y);
        float h   = 0.5 + hash11(fci * 2.3) * 0.6;
        d = min(d, coralBranch(p, base, h * coralScale, 0.04 * coralScale, fci * 11.7));
    }
    // Ocean floor
    d = min(d, p.y + 1.0);
    return d;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneSDF(p+e.xyy)-sceneSDF(p-e.xyy),
        sceneSDF(p+e.yxy)-sceneSDF(p-e.yxy),
        sceneSDF(p+e.yyx)-sceneSDF(p-e.yyx)
    ));
}

vec3 bioColor(float seed) {
    float h = hash11(seed * 9.13);
    if (h < 0.4) return vec3(0.0,  0.9,  1.0);  // bio-cyan
    if (h < 0.7) return vec3(0.55, 0.0,  1.0);  // violet
    return             vec3(0.0,  0.75, 0.45);   // deep teal
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;

    // Camera: wide medium shot of the reef
    float camAng = t * 0.12;
    vec3 ro = vec3(sin(camAng) * 2.5 * coralScale, 0.3, cos(camAng) * 2.5 * coralScale);
    vec3 fw  = normalize(vec3(0.0, -0.3, 0.0) - ro);
    vec3 rgt = normalize(cross(fw, vec3(0.0,1.0,0.0)));
    vec3 up_ = cross(rgt, fw);
    vec3 rd  = normalize(fw + uv.x * rgt * 0.65 + uv.y * up_ * 0.65);

    float dist = 0.0;
    bool hit = false;
    bool isFloor = false;
    for (int i = 0; i < 64; i++) {
        float d = sceneSDF(ro + rd * dist);
        if (d < 0.003) {
            hit = true;
            isFloor = ((ro + rd * dist).y < -0.95);
            break;
        }
        dist += d * 0.8;
        if (dist > 12.0) break;
    }

    // Deep ocean ambient — void black to deep teal
    float depthFade = clamp(rd.y * 0.5 + 0.5, 0.0, 1.0);
    vec3 col = mix(vec3(0.0,0.0,0.01), vec3(0.0,0.05,0.08), depthFade);

    if (hit) {
        vec3 p = ro + rd * dist;
        vec3 N = calcNormal(p);

        if (isFloor) {
            col = vec3(0.04, 0.07, 0.06); // dark sandy floor
        } else {
            // Determine nearest colony seed for color
            float bestSeed = 0.0; float dmin = 1e6;
            for (int ci = 0; ci < 5; ci++) {
                float fci = float(ci);
                vec2 pos2d = hash22(vec2(fci, fci*1.7)) * 3.0 - 1.5;
                pos2d *= coralScale;
                float dd = length(p.xz - pos2d);
                if (dd < dmin) { dmin = dd; bestSeed = fci; }
            }
            vec3 base = bioColor(bestSeed);

            // Bioluminescent glow: pulsing with TIME
            float pulse = 0.8 + 0.2 * sin(t * 1.8 + bestSeed * 3.7);
            float kD = max(dot(N, normalize(vec3(0.3, 1.0, 0.2))), 0.0);

            col = base * (kD + 0.2) * pulse * hdrPeak * audio;
        }

        // Depth fog
        float fog = exp(-dist * 0.1);
        col *= fog;
    }

    // Bioluminescent glow halos from coral tips (screenspace approximation)
    for (int ci = 0; ci < 5; ci++) {
        float fci = float(ci);
        vec2 pos2d = (hash22(vec2(fci, fci*1.7)) * 3.0 - 1.5) * coralScale;
        vec3 worldPt = vec3(pos2d.x, 0.2, pos2d.y);
        vec3 toLight  = worldPt - ro;
        float proj = dot(toLight, rd);
        if (proj > 0.0) {
            vec3 closest = ro + rd * proj - worldPt;
            float dr = length(closest);
            float bloom = exp(-dr * dr / (glowRadius * glowRadius)) * exp(-proj * 0.08);
            col += bioColor(fci) * bloom * hdrPeak * 0.3 * audio;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
