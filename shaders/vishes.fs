/*{
  "DESCRIPTION": "Neon Helix — a rotating DNA-like double-helix built from SDF capsule segments. Traveling beads on magenta and cyan strands connected by gold rungs. Single-pass 3D raymarcher.",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "helixRadius", "LABEL": "Helix Radius", "TYPE": "float", "MIN": 0.2,  "MAX": 1.0,  "DEFAULT": 0.5  },
    { "NAME": "helixPitch",  "LABEL": "Helix Pitch",  "TYPE": "float", "MIN": 0.3,  "MAX": 1.5,  "DEFAULT": 0.7  },
    { "NAME": "helixTurns",  "LABEL": "Helix Turns",  "TYPE": "float", "MIN": 1.0,  "MAX": 5.0,  "DEFAULT": 3.0  },
    { "NAME": "beadCount",   "LABEL": "Bead Count",   "TYPE": "float", "MIN": 4.0,  "MAX": 16.0, "DEFAULT": 10.0 },
    { "NAME": "beadSpeed",   "LABEL": "Bead Speed",   "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 0.8  },
    { "NAME": "beadSize",    "LABEL": "Bead Size",    "TYPE": "float", "MIN": 0.03, "MAX": 0.15, "DEFAULT": 0.07 },
    { "NAME": "rungOpacity", "LABEL": "Rung Opacity", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6  },
    { "NAME": "audioPulse",  "LABEL": "Audio Pulse",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7  }
  ]
}*/

// ── Hashing ───────────────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// ── Rotation helpers ──────────────────────────────────────────────────────
mat3 rotateY(float a) {
    float c = cos(a), s = sin(a);
    return mat3(c,0.0,s, 0.0,1.0,0.0, -s,0.0,c);
}

// ── Capsule SDF ───────────────────────────────────────────────────────────
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b - a;
    float t = clamp(dot(p - a, ab) / dot(ab, ab), 0.0, 1.0);
    return length(p - a - ab * t) - r;
}

// ── Scene: double helix + beads ───────────────────────────────────────────
// mat: 0=strandA(magenta), 1=strandB(cyan), 2=rung(gold), 3+ reserved
struct Hit { float dist; int mat; };

Hit map(vec3 p) {
    Hit h;
    h.dist = 1e9;
    h.mat  = -1;

    // Whole helix rotates slowly
    p = rotateY(TIME * 0.15) * p;

    float totalHeight = helixPitch * helixTurns;
    int SEGS = 48;

    for (int i = 0; i < 48; i++) {
        float t0 = float(i)     / float(SEGS);
        float t1 = float(i + 1) / float(SEGS);

        float ang0 = t0 * helixTurns * 6.28318;
        float ang1 = t1 * helixTurns * 6.28318;
        float y0   = (t0 - 0.5) * totalHeight;
        float y1   = (t1 - 0.5) * totalHeight;

        // Strand A
        vec3 aA0 = vec3(cos(ang0) * helixRadius, y0, sin(ang0) * helixRadius);
        vec3 aA1 = vec3(cos(ang1) * helixRadius, y1, sin(ang1) * helixRadius);
        // Strand B (180 degrees offset)
        vec3 aB0 = vec3(cos(ang0 + 3.14159) * helixRadius, y0, sin(ang0 + 3.14159) * helixRadius);
        vec3 aB1 = vec3(cos(ang1 + 3.14159) * helixRadius, y1, sin(ang1 + 3.14159) * helixRadius);

        float dA = sdCapsule(p, aA0, aA1, 0.04);
        float dB = sdCapsule(p, aB0, aB1, 0.04);

        // Rungs every 6 segments
        if (mod(float(i), 6.0) < 0.5) {
            float dR = sdCapsule(p, aA0, aB0, 0.025);
            if (dR < h.dist) { h.dist = dR; h.mat = 2; }
        }

        if (dA < h.dist) { h.dist = dA; h.mat = 0; }
        if (dB < h.dist) { h.dist = dB; h.mat = 1; }
    }

    // Traveling beads along both strands
    int NB = int(clamp(beadCount, 1.0, 16.0));
    for (int i = 0; i < 16; i++) {
        if (i >= NB) break;
        float fi = float(i);
        float tBead = fract(fi / float(NB) + TIME * beadSpeed * (0.5 + hash11(fi) * 0.5));
        float ang = tBead * helixTurns * 6.28318;
        float y   = (tBead - 0.5) * totalHeight;
        int strand = int(mod(fi, 2.0));
        float offset = (strand == 0) ? 0.0 : 3.14159;
        vec3 beadPos = vec3(cos(ang + offset) * helixRadius, y, sin(ang + offset) * helixRadius);
        float effBeadSize = beadSize * (1.0 + audioLevel * audioPulse * 0.2);
        float dBead = length(p - beadPos) - effBeadSize;
        if (dBead < h.dist) { h.dist = dBead; h.mat = strand; }
    }

    return h;
}

vec3 calcNormal(vec3 p) {
    // Note: normal must account for helix rotation — compute in rotated space
    vec3 pr = rotateY(TIME * 0.15) * p;
    vec2 e = vec2(1e-3, 0.0);
    // Use map directly (it rotates internally)
    return normalize(vec3(
        map(p + e.xyy).dist - map(p - e.xyy).dist,
        map(p + e.yxy).dist - map(p - e.yxy).dist,
        map(p + e.yyx).dist - map(p - e.yyx).dist
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // ── Camera orbiting helix ─────────────────────────────────────────────
    vec3 ro = vec3(sin(TIME * 0.2) * 3.0,
                   sin(TIME * 0.07) * 1.0,
                   cos(TIME * 0.2) * 3.0);
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fwd    = normalize(target - ro);
    vec3 right  = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up     = cross(fwd, right);
    vec3 rd     = normalize(fwd + uv.x * right + uv.y * up);

    // ── Palette ───────────────────────────────────────────────────────────
    vec3 colMagenta  = vec3(2.5, 0.0, 2.0);
    vec3 colCyan     = vec3(0.0, 2.2, 2.5);
    vec3 colGold     = vec3(2.5, 1.8, 0.0);
    vec3 colSpecular = vec3(3.0, 3.0, 2.8);
    vec3 colBg       = vec3(0.0, 0.0, 0.0);

    // ── Raymarching ───────────────────────────────────────────────────────
    vec3 col = colBg;
    float t  = 0.1;
    bool hitSomething = false;
    int  hitMat = -1;
    vec3 hitPos;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * t;
        Hit h  = map(p);
        if (h.dist < 0.002) {
            hitSomething = true;
            hitMat = h.mat;
            hitPos = p;
            break;
        }
        t += max(h.dist, 0.005);
        if (t > 20.0) break;
    }

    if (hitSomething) {
        vec3 n = calcNormal(hitPos);

        // Material color
        vec3 matCol;
        if      (hitMat == 0) matCol = colMagenta;
        else if (hitMat == 1) matCol = colCyan;
        else                  matCol = colGold * rungOpacity;

        // Phong lighting
        vec3 keyLight = normalize(vec3(2.0, 3.0, 1.0));
        float diff    = max(dot(n, keyLight), 0.0);
        vec3 hv       = normalize(keyLight - rd);
        float spec    = pow(max(dot(n, hv), 0.0), 32.0) * 3.0;

        col = matCol * (0.05 + diff * 0.95);
        col += colSpecular * spec;

        // Rim lighting: subtle edge bloom
        float rim = 1.0 - max(dot(n, -rd), 0.0);
        col += matCol * pow(rim, 3.0) * 0.5;

        // fwidth AA on silhouette edges
        float fw = fwidth(map(hitPos).dist);
        float aa = 1.0 - smoothstep(-fw * 0.5, fw * 2.0, map(hitPos).dist);
        col = mix(colBg, col, aa);
    }

    gl_FragColor = vec4(col, 1.0);
}
