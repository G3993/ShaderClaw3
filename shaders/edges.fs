/*{
    "DESCRIPTION": "Neon Billiards — 6 HDR neon spheres bouncing inside a 3D box with capsule motion-blur trails",
    "CREDIT": "ShaderClaw",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "Particles", "3D"],
    "INPUTS": [
        {
            "NAME": "ballCount",
            "TYPE": "float",
            "MIN": 2.0,
            "MAX": 6.0,
            "DEFAULT": 6.0,
            "LABEL": "Ball Count"
        },
        {
            "NAME": "boxSize",
            "TYPE": "float",
            "MIN": 0.8,
            "MAX": 2.0,
            "DEFAULT": 1.4,
            "LABEL": "Box Size"
        },
        {
            "NAME": "ballRadius",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 0.5,
            "DEFAULT": 0.22,
            "LABEL": "Ball Radius"
        },
        {
            "NAME": "trailLen",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.6,
            "LABEL": "Trail Length"
        },
        {
            "NAME": "glowAmt",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 3.0,
            "DEFAULT": 1.8,
            "LABEL": "Glow Amount"
        },
        {
            "NAME": "audioPulse",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.8,
            "LABEL": "Audio Pulse"
        },
        {
            "NAME": "motionSpeed",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 2.0,
            "DEFAULT": 0.6,
            "LABEL": "Motion Speed"
        }
    ]
}*/

// ---- helpers ----
float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// Triangle-wave bounce: any float -> [0,1] bouncing
float bounce01(float x) { return abs(fract(x * 0.5) * 2.0 - 1.0); }

// Fully saturated neon palette — HDR 2.0–2.5
vec3 ballColor(int idx) {
    if (idx == 0) return vec3(2.5, 0.0, 2.0);   // magenta
    if (idx == 1) return vec3(0.0, 2.5, 2.5);   // cyan
    if (idx == 2) return vec3(2.5, 2.0, 0.0);   // gold
    if (idx == 3) return vec3(2.5, 0.4, 0.0);   // orange
    if (idx == 4) return vec3(0.4, 2.5, 0.0);   // lime
    return             vec3(1.2, 0.0, 2.5);      // violet
}

// Deterministic ball position at scaled-time t inside box of half-size bs
vec3 ballPos(int i, float t, float bs) {
    float fi = float(i);
    float sx = 0.3 + hash11(fi * 3.7 + 0.1) * 0.7;
    float sy = 0.3 + hash11(fi * 3.7 + 0.2) * 0.7;
    float sz = 0.3 + hash11(fi * 3.7 + 0.3) * 0.7;
    float px = hash11(fi * 5.1 + 0.4);
    float py = hash11(fi * 5.1 + 0.5);
    float pz = hash11(fi * 5.1 + 0.6);
    return vec3(
        (bounce01(t * sx + px) * 2.0 - 1.0) * bs,
        (bounce01(t * sy + py) * 2.0 - 1.0) * bs,
        (bounce01(t * sz + pz) * 2.0 - 1.0) * bs
    );
}

// ---- SDF primitives ----
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3  ab = b - a;
    vec3  ap = p - a;
    float t  = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
    return length(ap - ab * t) - r;
}

// ---- Scene ----
struct Hit {
    float dist;
    int   id;   // ball index 0-5, 10 = floor, -1 = miss
};

Hit mapScene(vec3 p, float t, float scaledR) {
    Hit h;
    h.dist = 1e9;
    h.id   = -1;

    float trailDt = 0.015 * trailLen;
    int   n       = int(clamp(ballCount, 2.0, 6.0));

    for (int i = 0; i < 6; i++) {
        if (i >= n) break;
        vec3  curr = ballPos(i, t, boxSize);
        vec3  prev = ballPos(i, t - trailDt, boxSize);
        float d    = sdCapsule(p, prev, curr, scaledR);
        if (d < h.dist) {
            h.dist = d;
            h.id   = i;
        }
    }

    // Subtle dark floor at y = -(boxSize * 0.97)
    float floorD = p.y + boxSize * 0.97;
    if (floorD < h.dist) {
        h.dist = floorD;
        h.id   = 10;
    }

    return h;
}

vec3 calcNormal(vec3 p, float t, float scaledR) {
    vec2 e = vec2(0.0005, 0.0);
    return normalize(vec3(
        mapScene(p + e.xyy, t, scaledR).dist - mapScene(p - e.xyy, t, scaledR).dist,
        mapScene(p + e.yxy, t, scaledR).dist - mapScene(p - e.yxy, t, scaledR).dist,
        mapScene(p + e.yyx, t, scaledR).dist - mapScene(p - e.yyx, t, scaledR).dist
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x   *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME * motionSpeed;

    // Audio-modulated ball scale — modulates, never gates
    float scaledR = ballRadius * (1.0 + audioLevel * audioPulse * 0.2);

    // Orbiting camera
    vec3 ro  = vec3(3.5 * sin(TIME * 0.12), 2.0, 3.5 * cos(TIME * 0.12));
    vec3 ta  = vec3(0.0, 0.0, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 rgt = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up  = cross(rgt, fwd);
    vec3 rd  = normalize(fwd + uv.x * rgt + uv.y * up);

    // Lighting
    vec3 keyLight = normalize(vec3(1.5, 3.0, 1.0));
    vec3 rimLight = normalize(vec3(-1.5, -0.5, -1.0));

    // Raymarch — 96 steps
    float tMarch  = 0.0;
    Hit   hit;
    hit.id       = -1;
    hit.dist     = 1e9;
    vec3  hitP   = ro;
    float minD   = 1e9; // track nearest approach for glow

    for (int s = 0; s < 96; s++) {
        hitP     = ro + rd * tMarch;
        hit      = mapScene(hitP, t, scaledR);
        minD     = min(minD, hit.dist);
        if (hit.dist < 0.001) break;
        if (tMarch > 30.0)   { hit.id = -1; break; }
        tMarch  += max(hit.dist, 0.002);
    }

    vec3 col = vec3(0.0); // BLACK void

    if (hit.id >= 0 && hit.id < 10) {
        vec3  n       = calcNormal(hitP, t, scaledR);
        vec3  baseCol = ballColor(hit.id);

        // Blinn-Phong
        float diff    = max(dot(n, keyLight), 0.0);
        float ambient = 0.08;
        float rimF    = pow(max(dot(n, rimLight), 0.0), 2.0) * 0.5;

        vec3  halfV   = normalize(keyLight - rd);
        float spec    = pow(max(dot(n, halfV), 0.0), 32.0) * 2.5;

        vec3  litColor = baseCol * (diff + ambient + rimF) + vec3(3.0, 3.0, 3.0) * spec;

        // fwidth() AA on capsule iso-edge
        float fw = fwidth(hit.dist);
        float aa = smoothstep(fw, -fw, hit.dist);
        col = mix(vec3(0.0), litColor, aa);

        // Glow halo accumulated OUTSIDE march, using nearest approach distance
        col += glowAmt * baseCol * exp(-minD * 12.0);

    } else if (hit.id == 10) {
        // Dark floor with faint diffuse
        vec3  n    = vec3(0.0, 1.0, 0.0);
        float diff = max(dot(n, keyLight), 0.0);
        col = vec3(0.03, 0.02, 0.04) * (diff * 0.4 + 0.1);

        // Colored glow bleed onto floor from balls
        col += glowAmt * 0.3 * exp(-minD * 5.0) * vec3(0.4, 0.1, 0.6);

    } else {
        // Void — bleed of glow halos into empty space
        col = glowAmt * 0.25 * exp(-minD * 6.0) * vec3(0.3, 0.1, 0.5);
    }

    gl_FragColor = vec4(col, 1.0);
}
