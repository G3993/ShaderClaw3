/*{
    "DESCRIPTION": "Raymarched voxel flower — recursive branching tree of cubes on a reflective checkerboard floor with animated glow pulses",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "camDist",    "LABEL": "Camera Dist",  "TYPE": "float", "DEFAULT": 4.5,  "MIN": 2.0,  "MAX": 10.0 },
        { "NAME": "camHeight",  "LABEL": "Camera Height", "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.5,  "MAX": 5.0 },
        { "NAME": "rotSpeed",   "LABEL": "Rotate Speed", "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 2.0 },
        { "NAME": "pulseSpeed", "LABEL": "Pulse Speed",  "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 2.0 },
        { "NAME": "branchCount","LABEL": "Branches",     "TYPE": "float", "DEFAULT": 4.0,  "MIN": 2.0,  "MAX": 6.0 },
        { "NAME": "cubeSize",   "LABEL": "Cube Size",    "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.04, "MAX": 0.3 },
        { "NAME": "treeScale",  "LABEL": "Tree Scale",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3,  "MAX": 2.0 },
        { "NAME": "floorRefl",  "LABEL": "Floor Reflect", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "fogDensity", "LABEL": "Fog Density",  "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.0,  "MAX": 0.2 },
        { "NAME": "skyColor",   "LABEL": "Sky Color",    "TYPE": "color", "DEFAULT": [0.25, 0.58, 0.64, 1.0] },
        { "NAME": "glowColor1", "LABEL": "Glow A",       "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
        { "NAME": "glowColor2", "LABEL": "Glow B",       "TYPE": "color", "DEFAULT": [0.2, 0.4, 0.95, 1.0] }
    ]
}*/

// --- Utility ---
float hash(float p) { return fract(sin(p * 127.1) * 43758.5453); }
float hash2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec3 hash3(float p) {
    return fract(sin(vec3(p, p + 1.0, p + 2.0) * vec3(127.1, 269.5, 419.2)) * 43758.5453);
}

// --- SDF primitives ---
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float sdPlane(vec3 p) { return p.y; }

// --- Rotation ---
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// --- Tree structure ---
// We store up to 128 cube positions/sizes via a procedural approach
// evaluated at raymarch time using deterministic branching

struct Branch {
    vec3 pos;
    float angle;
    float length;
    int depth;
    float seed;
};

// Evaluate minimum distance to all cubes in the tree
// Uses iterative branching simulation (deterministic per-frame)
// Returns: x = distance, y = depth (0-1), z = glow intensity
vec4 treeDE(vec3 p, float t) {
    float d = 1e10;
    float hitDepth = 0.0;
    vec3 hitColor = vec3(0.0);
    float glow = 0.0;

    int maxDepth = 4;
    float lenMult = 0.8;
    float sz = cubeSize * treeScale;

    // Effector wave positions (two pulses chasing each other)
    float eff1 = sin(t * pulseSpeed * 3.14159) * 0.5 + 0.5;
    float eff2 = sin(t * pulseSpeed * 3.14159 + 1.5) * 0.5 + 0.5;

    // Iterative BFS-style tree: we simulate branches in a stack-like loop
    // Level 0: trunk
    float baseLen = 1.6 * treeScale;

    // We iterate through branches using deterministic seeds
    for (int b0 = 0; b0 < 6; b0++) {
        if (float(b0) >= branchCount) break;
        float a0 = 6.2831853 * float(b0) / branchCount;
        float s0 = hash(float(b0) * 13.7);
        float len0 = baseLen * (0.7 + s0 * 0.3);
        vec3 dir0 = vec3(cos(a0), 1.0 + s0 * 0.5, sin(a0)) * len0;

        // Cubes along this branch
        for (int i = 0; i < 8; i++) {
            float ft = float(i) / 8.0;
            vec3 cp = dir0 * ft;
            cp += (hash3(s0 + float(i) * 0.1) - 0.5) * sz * 4.0;
            float dd = sdBox(p - cp, vec3(sz * (1.0 + s0 * 0.5)));
            float depth01 = 0.15;
            if (dd < d) { d = dd; hitDepth = depth01; }

            float dif1 = abs(depth01 - eff1);
            float dif2 = abs(depth01 - eff2);
            if (dif1 < 0.15) glow += (0.15 - dif1) * 5.0 / (1.0 + dd * dd * 100.0);
            if (dif2 < 0.15) glow += (0.15 - dif2) * 5.0 / (1.0 + dd * dd * 100.0);
        }

        // Level 1 sub-branches
        for (int b1 = 0; b1 < 5; b1++) {
            float s1 = hash(s0 * 7.0 + float(b1) * 31.3);
            if (s1 > 0.7) continue;
            float a1 = a0 + (s1 - 0.35) * 3.0;
            float len1 = len0 * lenMult * (0.6 + s1 * 0.4);
            vec3 base1 = dir0 * (0.5 + s1 * 0.5);
            vec3 dir1 = vec3(cos(a1) * 0.7, 0.6 + s1 * 0.4, sin(a1) * 0.7) * len1;

            for (int i = 0; i < 6; i++) {
                float ft = float(i) / 6.0;
                vec3 cp = base1 + dir1 * ft;
                cp += (hash3(s1 * 17.0 + float(i) * 0.13) - 0.5) * sz * 3.0;
                float csz = sz * (0.8 + hash(s1 + float(i)) * 0.4);
                float dd = sdBox(p - cp, vec3(csz));
                float depth01 = 0.35;
                if (dd < d) { d = dd; hitDepth = depth01; }

                float dif1 = abs(depth01 - eff1);
                float dif2 = abs(depth01 - eff2);
                if (dif1 < 0.15) glow += (0.15 - dif1) * 4.0 / (1.0 + dd * dd * 80.0);
                if (dif2 < 0.15) glow += (0.15 - dif2) * 4.0 / (1.0 + dd * dd * 80.0);
            }

            // Level 2 sub-branches
            for (int b2 = 0; b2 < 4; b2++) {
                float s2 = hash(s1 * 11.0 + float(b2) * 47.1);
                if (s2 > 0.65) continue;
                float a2 = a1 + (s2 - 0.325) * 2.5;
                float len2 = len1 * lenMult * (0.5 + s2 * 0.5);
                vec3 base2 = base1 + dir1 * (0.4 + s2 * 0.5);
                vec3 dir2 = vec3(cos(a2) * 0.5, 0.4 + s2 * 0.6, sin(a2) * 0.5) * len2;

                for (int i = 0; i < 5; i++) {
                    float ft = float(i) / 5.0;
                    vec3 cp = base2 + dir2 * ft;
                    cp += (hash3(s2 * 23.0 + float(i) * 0.17) - 0.5) * sz * 2.0;
                    float csz = sz * (0.6 + hash(s2 + float(i) * 3.0) * 0.5);
                    float dd = sdBox(p - cp, vec3(csz));
                    float depth01 = 0.55;
                    if (dd < d) { d = dd; hitDepth = depth01; }

                    float dif1 = abs(depth01 - eff1);
                    float dif2 = abs(depth01 - eff2);
                    if (dif1 < 0.15) glow += (0.15 - dif1) * 3.0 / (1.0 + dd * dd * 60.0);
                    if (dif2 < 0.15) glow += (0.15 - dif2) * 3.0 / (1.0 + dd * dd * 60.0);
                }

                // Level 3 — leaf tips
                for (int b3 = 0; b3 < 3; b3++) {
                    float s3 = hash(s2 * 19.0 + float(b3) * 67.3);
                    if (s3 > 0.6) continue;
                    vec3 base3 = base2 + dir2 * (0.5 + s3 * 0.4);
                    vec3 dir3 = normalize(vec3(cos(a2 + s3 * 4.0), 0.3 + s3, sin(a2 + s3 * 4.0))) * len2 * 0.5;

                    for (int i = 0; i < 4; i++) {
                        float ft = float(i) / 4.0;
                        vec3 cp = base3 + dir3 * ft;
                        cp += (hash3(s3 * 31.0 + float(i) * 0.2) - 0.5) * sz * 1.5;
                        float csz = sz * (0.4 + hash(s3 + float(i) * 5.0) * 0.3);
                        float dd = sdBox(p - cp, vec3(csz));
                        float depth01 = 0.8;
                        if (dd < d) { d = dd; hitDepth = depth01; }

                        float dif1 = abs(depth01 - eff1);
                        float dif2 = abs(depth01 - eff2);
                        if (dif1 < 0.15) glow += (0.15 - dif1) * 2.0 / (1.0 + dd * dd * 40.0);
                        if (dif2 < 0.15) glow += (0.15 - dif2) * 2.0 / (1.0 + dd * dd * 40.0);
                    }
                }
            }
        }
    }

    return vec4(d, hitDepth, glow, 0.0);
}

// Scene SDF
vec4 scene(vec3 p, float t) {
    vec4 tree = treeDE(p, t);
    float floor = sdPlane(p);
    if (floor < tree.x) {
        return vec4(floor, -1.0, tree.z * 0.3, 0.0); // -1 marks floor
    }
    return tree;
}

// Normal via central differences
vec3 calcNormal(vec3 p, float t) {
    vec2 e = vec2(0.002, 0.0);
    float d = scene(p, t).x;
    return normalize(vec3(
        scene(p + e.xyy, t).x - d,
        scene(p + e.yxy, t).x - d,
        scene(p + e.yyx, t).x - d
    ));
}

// Checkerboard
float checker(vec2 p) {
    vec2 q = floor(p);
    return mod(q.x + q.y, 2.0);
}

// Soft shadow
float softShadow(vec3 ro, vec3 rd, float mint, float maxt, float k, float t) {
    float res = 1.0;
    float ph = 1e10;
    float tt = mint;
    for (int i = 0; i < 24; i++) {
        if (tt >= maxt) break;
        float h = treeDE(ro + rd * tt, t).x;
        if (h < 0.001) return 0.0;
        float y = h * h / (2.0 * ph);
        float d = sqrt(h * h - y * y);
        res = min(res, k * d / max(0.0, tt - y));
        ph = h;
        tt += h * 0.5;
    }
    return clamp(res, 0.0, 1.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME;

    // Orbiting camera
    float ca = t * rotSpeed;
    vec3 ro = vec3(cos(ca) * camDist, camHeight, sin(ca) * camDist);
    vec3 target = vec3(0.0, 1.2, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd * 1.8 + right * uv.x + up * uv.y);

    // Sky gradient
    vec3 sky = mix(skyColor.rgb * 1.2, vec3(0.0, 0.4, 1.0), max(rd.y, 0.0));
    sky = mix(skyColor.rgb * 0.5, sky, smoothstep(-0.1, 0.3, rd.y));

    // Raymarch
    float totalDist = 0.0;
    vec4 hit = vec4(0.0);
    vec3 pos;
    bool hitSomething = false;

    for (int i = 0; i < 80; i++) {
        pos = ro + rd * totalDist;
        hit = scene(pos, t);
        if (hit.x < 0.002) { hitSomething = true; break; }
        if (totalDist > 20.0) break;
        totalDist += hit.x * 0.8;
    }

    vec3 col = sky;

    if (hitSomething) {
        vec3 n = calcNormal(pos, t);
        vec3 lightDir = normalize(vec3(0.7, 0.5, 0.7));
        float diff = max(dot(n, lightDir), 0.0);
        float amb = 0.15;

        // Shadow
        float shad = softShadow(pos + n * 0.01, lightDir, 0.02, 8.0, 12.0, t);

        if (hit.y < 0.0) {
            // Floor
            vec2 floorUV = pos.xz * 7.5;
            float chk = checker(floorUV);
            vec3 floorCol = mix(vec3(0.15, 0.15, 0.18), vec3(0.35, 0.35, 0.38), chk);

            // Floor lighting
            vec3 floorLit = floorCol * (diff * shad * 0.8 + amb);

            // Reflection
            vec3 reflDir = reflect(rd, n);
            float reflDist = 0.0;
            vec4 reflHit;
            vec3 reflPos;
            bool reflHitSomething = false;
            for (int i = 0; i < 32; i++) {
                reflPos = pos + reflDir * reflDist + n * 0.02;
                reflHit = treeDE(reflPos, t);
                if (reflHit.x < 0.004) { reflHitSomething = true; break; }
                if (reflDist > 10.0) break;
                reflDist += reflHit.x;
            }

            vec3 reflCol = sky;
            if (reflHitSomething) {
                vec3 rn = calcNormal(reflPos, t);
                float rd2 = max(dot(rn, lightDir), 0.0);

                // Color based on tree depth
                float hue = reflHit.y * 0.5;
                vec3 treeCol = vec3(
                    0.5 + 0.5 * cos(6.2831 * (hue + 0.0)),
                    0.5 + 0.5 * cos(6.2831 * (hue + 0.33)),
                    0.5 + 0.5 * cos(6.2831 * (hue + 0.67))
                ) * 0.7;
                reflCol = treeCol * (rd2 * 0.7 + 0.2);

                // Glow in reflection
                float effGlow = reflHit.z;
                reflCol += glowColor1.rgb * effGlow * 0.5;
                reflCol += glowColor2.rgb * effGlow * 0.3;
            }

            // Fresnel
            float fresnel = pow(1.0 - max(dot(-rd, n), 0.0), 3.0);
            col = mix(floorLit, reflCol, floorRefl * (0.3 + fresnel * 0.7));

            // Floor glow from nearby tree
            col += vec3(hit.z) * glowColor1.rgb * 0.1;
        } else {
            // Tree surface
            float hue = hit.y * 0.5;
            vec3 treeCol = vec3(
                0.5 + 0.5 * cos(6.2831 * (hue + 0.0)),
                0.5 + 0.5 * cos(6.2831 * (hue + 0.33)),
                0.5 + 0.5 * cos(6.2831 * (hue + 0.67))
            ) * 0.7;

            // Square edge darkening (voxel look)
            vec3 absN = abs(n);
            float maxComp = max(absN.x, max(absN.y, absN.z));
            vec3 localUV;
            if (maxComp == absN.x) localUV = vec3(pos.y, pos.z, pos.x);
            else if (maxComp == absN.y) localUV = vec3(pos.x, pos.z, pos.y);
            else localUV = vec3(pos.x, pos.y, pos.z);
            vec2 faceUV = fract(localUV.xy / (cubeSize * 2.0 * treeScale));
            float edge = max(abs(faceUV.x - 0.5), abs(faceUV.y - 0.5));
            float edgeFactor = smoothstep(0.35, 0.5, edge);
            treeCol = mix(treeCol, treeCol * 0.3, edgeFactor * 0.6);

            // Lighting
            vec3 viewDir = normalize(ro - pos);
            float spec = pow(max(dot(reflect(-lightDir, n), viewDir), 0.0), 32.0);
            col = treeCol * (diff * shad * 0.8 + amb) + vec3(0.3) * spec * shad;

            // Backlight (blue)
            float backDiff = max(dot(n, normalize(vec3(-0.7, -0.5, -0.7))), 0.0);
            col += vec3(0.02, 0.03, 0.08) * backDiff;

            // Animated glow
            float effGlow = hit.z;
            col += glowColor1.rgb * effGlow * effGlow * 2.0;
            col += glowColor2.rgb * effGlow * 1.5;
        }

        // Fog
        float fogAmount = 1.0 - exp(-totalDist * fogDensity);
        col = mix(col, skyColor.rgb, fogAmount);
    }

    // Tone mapping (ACES-like)
    col = col / (col + vec3(1.0));
    col = pow(col, vec3(0.85));

    // Vignette
    vec2 vuv = gl_FragCoord.xy / RENDERSIZE.xy;
    float vig = 1.0 - dot((vuv - 0.5) * 1.25, (vuv - 0.5) * 1.25);
    col *= clamp(vig, 0.0, 1.0);

    gl_FragColor = vec4(col, 1.0);
}
