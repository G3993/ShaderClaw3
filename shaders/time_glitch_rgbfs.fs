/*{
  "DESCRIPTION": "Fractured Prism — shattered octahedral polyhedra cluster orbiting in a neon void. Hot magenta, electric lime, and burnt gold palette.",
  "CATEGORIES": ["Generator", "3D", "Glitch"],
  "INPUTS": [
    { "NAME": "shardCount",  "LABEL": "Shard Count",  "TYPE": "float", "MIN": 2.0,  "MAX": 8.0,   "DEFAULT": 6.0 },
    { "NAME": "scatterDist", "LABEL": "Scatter Dist", "TYPE": "float", "MIN": 0.3,  "MAX": 2.5,   "DEFAULT": 1.2 },
    { "NAME": "spinSpeed",   "LABEL": "Spin Speed",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,   "DEFAULT": 0.4 },
    { "NAME": "camDist",     "LABEL": "Camera Dist",  "TYPE": "float", "MIN": 3.0,  "MAX": 8.0,   "DEFAULT": 5.0 },
    { "NAME": "specPow",     "LABEL": "Specular Pow", "TYPE": "float", "MIN": 8.0,  "MAX": 128.0, "DEFAULT": 32.0 },
    { "NAME": "audioPulse",  "LABEL": "Audio Pulse",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,   "DEFAULT": 0.7 }
  ]
}*/

// ── Hashing ──────────────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// ── Rotation matrices ─────────────────────────────────────────────────────
mat3 rotY(float a) {
    float c = cos(a), s = sin(a);
    return mat3(c,0.0,s, 0.0,1.0,0.0, -s,0.0,c);
}
mat3 rotX(float a) {
    float c = cos(a), s = sin(a);
    return mat3(1.0,0.0,0.0, 0.0,c,-s, 0.0,s,c);
}

// ── Octahedron SDF (approximates icosahedron cheaply) ────────────────────
float sdOctahedron(vec3 p, float s) {
    p = abs(p);
    return (p.x + p.y + p.z - s) * 0.57735027;
}

// ── Scene ─────────────────────────────────────────────────────────────────
struct Hit { float dist; int id; };

Hit map(vec3 p) {
    Hit h;
    h.dist = 1e9;
    h.id = -1;
    int N = int(clamp(shardCount, 1.0, 8.0));
    for (int i = 0; i < 8; i++) {
        if (i >= N) break;
        float fi = float(i);
        float angle = fi * 6.28318 / float(N);
        float yOff  = (hash11(fi * 3.7) - 0.5) * 0.9;
        vec3 center = vec3(cos(angle) * scatterDist, yOff, sin(angle) * scatterDist);
        float rot   = TIME * spinSpeed * (0.3 + hash11(fi * 1.3) * 0.7) + fi * 1.4;
        vec3 lp     = rotX(rot * 0.7) * rotY(rot) * (p - center);
        float size  = 0.3 + hash11(fi * 7.1) * 0.2;
        // Audio pulsates shard size
        float pulsedSize = size * (1.0 + audioLevel * audioPulse * 0.15);
        float d = sdOctahedron(lp, pulsedSize);
        if (d < h.dist) { h.dist = d; h.id = i; }
    }
    return h;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(1e-3, 0.0);
    return normalize(vec3(
        map(p + e.xyy).dist - map(p - e.xyy).dist,
        map(p + e.yxy).dist - map(p - e.yxy).dist,
        map(p + e.yyx).dist - map(p - e.yyx).dist
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Camera orbiting the cluster
    vec3 ro = vec3(sin(TIME * 0.15) * camDist,
                   camDist * 0.4,
                   cos(TIME * 0.15) * camDist);
    vec3 target = vec3(0.0, 0.0, 0.0);

    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd + uv.x * right + uv.y * up);

    // ── Palette ──────────────────────────────────────────────────────────
    vec3 colMagenta  = vec3(2.5, 0.0, 2.0);
    vec3 colLime     = vec3(0.5, 2.5, 0.0);
    vec3 colGold     = vec3(2.5, 1.8, 0.0);
    vec3 colSpecular = vec3(3.0, 3.0, 2.5);

    // ── Raymarching ──────────────────────────────────────────────────────
    vec3 col = vec3(0.0); // void black background
    float t  = 0.1;
    bool hit = false;
    int hitId = -1;
    vec3 hitPos;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * t;
        Hit h  = map(p);
        if (h.dist < 0.001) {
            hit    = true;
            hitId  = h.id;
            hitPos = p;
            break;
        }
        t += h.dist;
        if (t > 30.0) break;
    }

    if (hit) {
        vec3 n = calcNormal(hitPos);

        // Material color by shard id
        vec3 matCol;
        int mod3 = int(mod(float(hitId), 3.0));
        if      (mod3 == 0) matCol = colMagenta;
        else if (mod3 == 1) matCol = colLime;
        else                matCol = colGold;

        // Phong lighting
        vec3 keyLight = normalize(vec3(1.5, 3.0, 1.0));
        float diff    = max(dot(n, keyLight), 0.0);
        vec3 hv       = normalize(keyLight - rd);
        float spec    = pow(max(dot(n, hv), 0.0), specPow) * 3.0;

        // Silhouette darkening
        float sv = 1.0 - max(dot(n, -rd), 0.0);
        col = matCol * (0.05 + diff * 0.95);
        col += colSpecular * spec;
        col  = mix(col, vec3(0.0), sv * sv * 0.6);

        // fwidth silhouette AA
        float fw = fwidth(map(hitPos).dist);
        float aa = 1.0 - smoothstep(0.0, fw * 2.0, map(hitPos).dist);
        col *= aa;
    }

    gl_FragColor = vec4(col, 1.0);
}
