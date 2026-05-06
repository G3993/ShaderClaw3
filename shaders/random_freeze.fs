/*{
  "DESCRIPTION": "Amethyst Geode 3D — camera inside a hollow geode sphere looking at crystal formations",
  "CREDIT": "ShaderClaw auto-improve 2026-05-06",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Speed",         "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 3.0  },
    { "NAME": "crystalCount", "LABEL": "Crystal Count", "TYPE": "float", "DEFAULT": 8.0,  "MIN": 2.0,  "MAX": 12.0 },
    { "NAME": "hdrPeak",      "LABEL": "HDR Peak",      "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0  },
    { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0  }
  ]
}*/

// ── Palette ────────────────────────────────────────────────────────────────
// Void/rock:      vec3(0.01, 0.0, 0.02)   — dark obsidian
// Deep amethyst:  vec3(0.5, 0.0, 1.5)     — HDR purple
// Rose gold:      vec3(2.0, 0.5, 0.2)     — HDR warm
// Pale crystal:   vec3(1.2, 0.8, 2.0)     — HDR lavender
// Specular white: vec3(2.5, 2.2, 2.0)

// ── Hash / utility ─────────────────────────────────────────────────────────
float hash1(float n) { return fract(sin(n) * 43758.5453123); }
float hash1v(vec3 p) { return fract(sin(dot(p, vec3(127.1, 311.7, 74.3))) * 43758.5453); }

// ── Crystal colour from position hash ──────────────────────────────────────
vec3 crystalColor(int idx, float pk) {
    float fi = float(idx);
    float h = hash1(fi * 17.3 + 3.7);
    if (h < 0.34) return vec3(0.5, 0.0, 1.5) * pk;          // deep amethyst
    if (h < 0.67) return vec3(2.0, 0.5, 0.2) * (pk * 0.8);  // rose gold
    return vec3(1.2, 0.8, 2.0) * pk;                         // pale crystal
}

// ── SDF primitives ─────────────────────────────────────────────────────────
// Capsule: ends a, b; radii ra (at a), rb (at b)
float sdCapsuleTapered(vec3 p, vec3 a, vec3 b, float ra, float rb) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    float r = mix(ra, rb, h);
    return length(pa - ba * h) - r;
}

// Tilted box (for small faceted crystals)
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// ── Geode scene map ────────────────────────────────────────────────────────
// Returns: x = SDF distance, y = material id (0=rock/sphere, 1-8=crystal index+1)
vec2 map(vec3 p, float tm, float nCrystals, float audioGlow) {
    // Inner surface of hollow sphere (camera is inside → use negative SDF)
    float sphere = -(length(p) - 4.5);

    float crystals = 100.0;
    float bestIdx = 0.0;

    int N = int(clamp(nCrystals, 2.0, 12.0));

    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);
        // Fibonacci lattice on sphere surface
        float theta = fi * 2.39996323;
        float phi = acos(1.0 - 2.0 * (fi + 0.5) / float(N));

        // Crystal tip on sphere surface (pointing inward)
        vec3 tip = vec3(
            sin(phi) * cos(theta),
            cos(phi),
            sin(phi) * sin(theta)
        ) * 3.8;

        // Crystal base (closer to center, slight sway)
        vec3 base = tip * 0.4 + 0.15 * vec3(
            cos(fi * 1.7 + tm * 0.2),
            sin(fi * 2.3 + tm * 0.15),
            cos(fi * 0.9 + tm * 0.25)
        );

        // Tapered capsule: wide at base, sharp at tip
        float r0 = 0.15 + 0.08 * sin(fi * 2.7) + 0.04 * audioGlow;
        float r1 = 0.02;
        float dcr = sdCapsuleTapered(p, base, tip, r0, r1);

        if (dcr < crystals) {
            crystals = dcr;
            bestIdx = fi + 1.0;
        }
    }

    // Small faceted box crystals scattered around
    float boxes = 100.0;
    float bestBoxIdx = 0.0;
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        float ang = fi * 1.047197551 + tm * 0.08; // slow rotation
        vec3 cpos = vec3(cos(ang) * 2.8, sin(fi * 1.1) * 1.5, sin(ang) * 2.8);
        // Rotate box slightly
        float rot = fi * 0.7 + tm * 0.05;
        float cr = cos(rot), sr = sin(rot);
        vec3 lp = p - cpos;
        lp = vec3(cr * lp.x - sr * lp.z, lp.y, sr * lp.x + cr * lp.z);
        float s = 0.06 + 0.04 * hash1(fi * 5.3);
        float db = sdBox(lp, vec3(s * 0.6, s * 1.8, s * 0.4));
        if (db < boxes) {
            boxes = db;
            bestBoxIdx = fi + 13.0;
        }
    }

    // Combine all
    float d = sphere;
    float matId = 0.0;

    if (crystals < d) { d = crystals; matId = bestIdx; }
    if (boxes < d)    { d = boxes;    matId = bestBoxIdx; }

    return vec2(d, matId);
}

// ── Normal via central differences ─────────────────────────────────────────
vec3 calcNormal(vec3 p, float tm, float nCrystals, float audioGlow) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p + e.xyy, tm, nCrystals, audioGlow).x - map(p - e.xyy, tm, nCrystals, audioGlow).x,
        map(p + e.yxy, tm, nCrystals, audioGlow).x - map(p - e.yxy, tm, nCrystals, audioGlow).x,
        map(p + e.yyx, tm, nCrystals, audioGlow).x - map(p - e.yyx, tm, nCrystals, audioGlow).x
    ));
}

// ── Main ────────────────────────────────────────────────────────────────────
void main() {
    float tm = TIME * speed;
    float audioGlow = audioBass * audioReact * 2.0;

    // Camera inside the geode — slowly rotating to look around
    float camAngle = tm * 0.18;
    float camTilt  = sin(tm * 0.11) * 0.35;

    vec3 ro = vec3(0.0, 0.1, 0.0); // camera at near-center

    // Camera target: orbiting gaze direction
    vec3 target = vec3(cos(camAngle) * 3.5, sin(camTilt) * 1.5, sin(camAngle) * 3.5);

    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up    = cross(right, fwd);

    // Screen-space ray
    vec2 uv = (isf_FragNormCoord - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    vec3 rd = normalize(fwd + uv.x * right + uv.y * up);

    // ── Raymarching (64 steps) ──────────────────────────────────────────────
    float t = 0.001;
    float matId = 0.0;
    bool hit = false;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * t;
        vec2 res = map(p, tm, crystalCount, audioGlow);
        float d = res.x;
        if (d < 0.001) {
            matId = res.y;
            hit = true;
            break;
        }
        t += d * 0.6; // conservative step inside concave geometry
        if (t > 12.0) break;
    }

    // ── Shading ────────────────────────────────────────────────────────────
    vec3 col = vec3(0.01, 0.0, 0.02); // void obsidian

    if (hit) {
        vec3 p = ro + rd * t;
        vec3 nor = calcNormal(p, tm, crystalCount, audioGlow);

        // Lighting: warm key from above, cool fill from below
        vec3 keyDir  = normalize(vec3(0.4, 1.0, 0.3));
        vec3 fillDir = normalize(vec3(-0.3, -1.0, -0.4));

        float keyDiff  = max(dot(nor, keyDir),  0.0);
        float fillDiff = max(dot(nor, fillDir), 0.0);

        vec3 keyColor  = vec3(2.0, 1.2, 0.5);  // warm orange
        vec3 fillColor = vec3(0.2, 0.4, 1.0);  // cool blue

        // Specular
        vec3 halfVec = normalize(keyDir - rd);
        float spec = pow(max(dot(nor, halfVec), 0.0), 48.0);
        vec3 specColor = vec3(2.5, 2.2, 2.0);

        // fwidth AA on SDF iso-edge (approximated from screen-space derivatives)
        vec2 sdfSample = map(p, tm, crystalCount, audioGlow);
        float fw = fwidth(sdfSample.x);
        float edgeMask = smoothstep(0.0, fw * 2.0, abs(sdfSample.x));

        // Material colour
        vec3 baseCol;
        if (matId < 0.5) {
            // Rock / sphere interior wall — dark obsidian with faint purple veins
            baseCol = vec3(0.01, 0.0, 0.02) + nor * vec3(0.04, 0.0, 0.08);
        } else {
            // Crystal (pillar or box) — pick colour by index
            int idx = int(matId - 0.5);
            float pk = hdrPeak * (1.0 + audioGlow * 0.3);
            baseCol = crystalColor(idx, pk);
        }

        // Subsurface-like glow: crystal self-illumination
        float selfGlow = 0.0;
        if (matId > 0.5) {
            // Glow pulsing with audio and time
            selfGlow = 0.3 + 0.15 * sin(tm * 1.7 + matId * 2.3) + audioGlow * 0.25;
        }

        col  = baseCol * (keyDiff * keyColor + fillDiff * fillColor * 0.3);
        col += specColor * spec * (matId > 0.5 ? 1.0 : 0.3);
        col += baseCol * selfGlow;
        col  = mix(baseCol * 0.05, col, edgeMask);
    }

    gl_FragColor = vec4(col, 1.0);
}
