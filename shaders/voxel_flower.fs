/*{
    "DESCRIPTION": "Raymarched voxel flower — recursive branching tree of cubes on a reflective checkerboard floor with animated glow pulses",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "camDist",    "LABEL": "Camera Dist",  "TYPE": "float", "DEFAULT": 4.5,  "MIN": 2.0,  "MAX": 10.0 },
        { "NAME": "camHeight",  "LABEL": "Camera Height", "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.5,  "MAX": 5.0 },
        { "NAME": "rotSpeed",   "LABEL": "Rotate Speed", "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 2.0 },
        { "NAME": "pulseSpeed", "LABEL": "Pulse Speed",  "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 2.0 },
        { "NAME": "branchCount","LABEL": "Branches",     "TYPE": "float", "DEFAULT": 4.0,  "MIN": 2.0,  "MAX": 6.0 },
        { "NAME": "cubeSize",   "LABEL": "Cube Size",    "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.04, "MAX": 0.3 },
        { "NAME": "treeScale",  "LABEL": "Tree Scale",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3,  "MAX": 2.0 },
        { "NAME": "floorRefl",  "LABEL": "Floor Reflect", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "fogDensity", "LABEL": "Fog Density",  "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.0,  "MAX": 0.2 },
        { "NAME": "skyColor",   "LABEL": "Sky Color",    "TYPE": "color", "DEFAULT": [0.25, 0.58, 0.64, 1.0] },
        { "NAME": "glowColor1", "LABEL": "Glow A",       "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
        { "NAME": "glowColor2", "LABEL": "Glow B",       "TYPE": "color", "DEFAULT": [0.2, 0.4, 0.95, 1.0] }
    ]
}*/

float hash(float p) { return fract(sin(p * 127.1) * 43758.5453); }
vec3 hash3(float p) {
    return fract(sin(vec3(p, p + 1.0, p + 2.0) * vec3(127.1, 269.5, 419.2)) * 43758.5453);
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Lightweight distance-only tree — 3 levels, minimal branching
// ~40 sdBox per call (was ~500)
float treeDist(vec3 p) {
    if (length(p) > 5.0 * treeScale) return length(p) - 4.0 * treeScale;

    float d = 1e10;
    float sz = cubeSize * treeScale;
    float baseLen = 1.6 * treeScale;

    for (int b0 = 0; b0 < 4; b0++) {
        if (float(b0) >= branchCount) break;
        float a0 = 6.2831853 * float(b0) / branchCount;
        float s0 = hash(float(b0) * 13.7);
        float len0 = baseLen * (0.7 + s0 * 0.3);
        vec3 dir0 = vec3(cos(a0), 1.0 + s0 * 0.5, sin(a0)) * len0;

        // Bounding cull
        if (length(p - dir0 * 0.5) > len0 * 1.2 + d) continue;

        // 2 cubes along trunk
        for (int i = 0; i < 2; i++) {
            vec3 cp = dir0 * (float(i) * 0.15);
            cp += (hash3(s0 + float(i) * 0.1) - 0.5) * sz * 3.0;
            d = min(d, sdBox(p - cp, vec3(sz * (1.2 + s0 * 0.5))));
        }

        // Level 1: 2 sub-branches × 3 cubes
        for (int b1 = 0; b1 < 2; b1++) {
            float s1 = hash(s0 * 7.0 + float(b1) * 31.3);
            float a1 = a0 + (s1 - 0.5) * 2.5;
            float len1 = len0 * 0.7 * (0.6 + s1 * 0.4);
            vec3 base1 = dir0 * (0.4 + s1 * 0.4);
            vec3 dir1 = vec3(cos(a1) * 0.7, 0.6 + s1 * 0.4, sin(a1) * 0.7) * len1;

            if (length(p - base1 - dir1 * 0.5) > len1 * 1.2 + d) continue;

            for (int i = 0; i < 3; i++) {
                vec3 cp = base1 + dir1 * (float(i) / 3.0);
                cp += (hash3(s1 * 17.0 + float(i) * 0.13) - 0.5) * sz * 2.5;
                float csz = sz * (0.9 + hash(s1 + float(i)) * 0.3);
                d = min(d, sdBox(p - cp, vec3(csz)));
            }

            // Level 2: 1 sub-branch × 2 cubes
            float s2 = hash(s1 * 11.0 + 47.1);
            if (s2 < 0.7) {
                float a2 = a1 + (s2 - 0.35) * 2.0;
                float len2 = len1 * 0.6;
                vec3 base2 = base1 + dir1 * (0.5 + s2 * 0.3);
                vec3 dir2 = vec3(cos(a2) * 0.5, 0.5 + s2 * 0.5, sin(a2) * 0.5) * len2;

                for (int i = 0; i < 2; i++) {
                    vec3 cp = base2 + dir2 * (float(i) * 0.5);
                    cp += (hash3(s2 * 23.0 + float(i) * 0.17) - 0.5) * sz * 1.5;
                    float csz = sz * (0.6 + hash(s2 + float(i) * 3.0) * 0.4);
                    d = min(d, sdBox(p - cp, vec3(csz)));
                }
            }
        }
    }

    return d;
}

// Full tree with glow — only called once at the hit point
vec4 treeDE(vec3 p, float t) {
    if (length(p) > 5.0 * treeScale) return vec4(length(p) - 4.0 * treeScale, 0.0, 0.0, 0.0);

    float d = 1e10;
    float hitDepth = 0.0;
    float glow = 0.0;
    float sz = cubeSize * treeScale;
    float baseLen = 1.6 * treeScale;

    float eff1 = sin(t * pulseSpeed * 3.14159) * 0.5 + 0.5;
    float eff2 = sin(t * pulseSpeed * 3.14159 + 1.5) * 0.5 + 0.5;

    for (int b0 = 0; b0 < 4; b0++) {
        if (float(b0) >= branchCount) break;
        float a0 = 6.2831853 * float(b0) / branchCount;
        float s0 = hash(float(b0) * 13.7);
        float len0 = baseLen * (0.7 + s0 * 0.3);
        vec3 dir0 = vec3(cos(a0), 1.0 + s0 * 0.5, sin(a0)) * len0;

        if (length(p - dir0 * 0.5) > len0 * 1.2 + d) continue;

        for (int i = 0; i < 2; i++) {
            vec3 cp = dir0 * (float(i) * 0.15);
            cp += (hash3(s0 + float(i) * 0.1) - 0.5) * sz * 3.0;
            float dd = sdBox(p - cp, vec3(sz * (1.2 + s0 * 0.5)));
            if (dd < d) { d = dd; hitDepth = 0.15; }
            float dif1 = abs(0.15 - eff1);
            float dif2 = abs(0.15 - eff2);
            if (dif1 < 0.15) glow += (0.15 - dif1) * 5.0 / (1.0 + dd * dd * 100.0);
            if (dif2 < 0.15) glow += (0.15 - dif2) * 5.0 / (1.0 + dd * dd * 100.0);
        }

        for (int b1 = 0; b1 < 2; b1++) {
            float s1 = hash(s0 * 7.0 + float(b1) * 31.3);
            float a1 = a0 + (s1 - 0.5) * 2.5;
            float len1 = len0 * 0.7 * (0.6 + s1 * 0.4);
            vec3 base1 = dir0 * (0.4 + s1 * 0.4);
            vec3 dir1 = vec3(cos(a1) * 0.7, 0.6 + s1 * 0.4, sin(a1) * 0.7) * len1;

            if (length(p - base1 - dir1 * 0.5) > len1 * 1.2 + d) continue;

            for (int i = 0; i < 3; i++) {
                vec3 cp = base1 + dir1 * (float(i) / 3.0);
                cp += (hash3(s1 * 17.0 + float(i) * 0.13) - 0.5) * sz * 2.5;
                float csz = sz * (0.9 + hash(s1 + float(i)) * 0.3);
                float dd = sdBox(p - cp, vec3(csz));
                if (dd < d) { d = dd; hitDepth = 0.4; }
                float dif1 = abs(0.4 - eff1);
                float dif2 = abs(0.4 - eff2);
                if (dif1 < 0.15) glow += (0.15 - dif1) * 4.0 / (1.0 + dd * dd * 80.0);
                if (dif2 < 0.15) glow += (0.15 - dif2) * 4.0 / (1.0 + dd * dd * 80.0);
            }

            float s2 = hash(s1 * 11.0 + 47.1);
            if (s2 < 0.7) {
                float a2 = a1 + (s2 - 0.35) * 2.0;
                float len2 = len1 * 0.6;
                vec3 base2 = base1 + dir1 * (0.5 + s2 * 0.3);
                vec3 dir2 = vec3(cos(a2) * 0.5, 0.5 + s2 * 0.5, sin(a2) * 0.5) * len2;

                for (int i = 0; i < 2; i++) {
                    vec3 cp = base2 + dir2 * (float(i) * 0.5);
                    cp += (hash3(s2 * 23.0 + float(i) * 0.17) - 0.5) * sz * 1.5;
                    float csz = sz * (0.6 + hash(s2 + float(i) * 3.0) * 0.4);
                    float dd = sdBox(p - cp, vec3(csz));
                    if (dd < d) { d = dd; hitDepth = 0.7; }
                    float dif1 = abs(0.7 - eff1);
                    float dif2 = abs(0.7 - eff2);
                    if (dif1 < 0.15) glow += (0.15 - dif1) * 3.0 / (1.0 + dd * dd * 60.0);
                    if (dif2 < 0.15) glow += (0.15 - dif2) * 3.0 / (1.0 + dd * dd * 60.0);
                }
            }
        }
    }

    return vec4(d, hitDepth, glow, 0.0);
}

// Scene distance only
float sceneDist(vec3 p) {
    return min(treeDist(p), p.y);
}

// Full scene (primary raymarch only)
vec4 scene(vec3 p, float t) {
    vec4 tree = treeDE(p, t);
    if (p.y < tree.x) {
        return vec4(p.y, -1.0, tree.z * 0.3, 0.0);
    }
    return tree;
}

// Tetrahedron normal
vec3 calcNormal(vec3 p) {
    vec2 e = vec2(1.0, -1.0) * 0.003;
    return normalize(
        e.xyy * sceneDist(p + e.xyy) +
        e.yyx * sceneDist(p + e.yyx) +
        e.yxy * sceneDist(p + e.yxy) +
        e.xxx * sceneDist(p + e.xxx)
    );
}

float checker(vec2 p) {
    vec2 q = floor(p);
    return mod(q.x + q.y, 2.0);
}

// Shadow — 6 steps only
float softShadow(vec3 ro, vec3 rd, float mint, float maxt, float k) {
    float res = 1.0;
    float tt = mint;
    for (int i = 0; i < 6; i++) {
        if (tt >= maxt) break;
        float h = treeDist(ro + rd * tt);
        if (h < 0.002) return 0.0;
        res = min(res, k * h / tt);
        tt += max(h, 0.1);
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

    // Raymarch — 32 steps
    float totalDist = 0.0;
    vec4 hit = vec4(0.0);
    vec3 pos;
    bool hitSomething = false;

    for (int i = 0; i < 32; i++) {
        pos = ro + rd * totalDist;
        hit = scene(pos, t);
        if (hit.x < 0.003) { hitSomething = true; break; }
        if (totalDist > 15.0) break;
        totalDist += hit.x;
    }

    vec3 col = sky;

    if (hitSomething) {
        vec3 n = calcNormal(pos);
        vec3 lightDir = normalize(vec3(0.7, 0.5, 0.7));
        float diff = max(dot(n, lightDir), 0.0);
        float amb = 0.15;

        float shad = softShadow(pos + n * 0.02, lightDir, 0.05, 6.0, 10.0);

        if (hit.y < 0.0) {
            // Floor
            vec2 floorUV = pos.xz * 7.5;
            float chk = checker(floorUV);
            vec3 floorCol = mix(vec3(0.15, 0.15, 0.18), vec3(0.35, 0.35, 0.38), chk);
            vec3 floorLit = floorCol * (diff * shad * 0.8 + amb);

            // Fake reflection — tint floor toward tree color, no actual raymarching
            vec3 reflDir = reflect(rd, n);
            float fresnel = pow(1.0 - max(dot(-rd, n), 0.0), 3.0);
            vec3 fakeRefl = mix(sky, vec3(0.5, 0.35, 0.45), 0.4) * (0.3 + 0.2 * reflDir.y);
            col = mix(floorLit, fakeRefl, floorRefl * (0.2 + fresnel * 0.6));

            col += vec3(hit.z) * glowColor1.rgb * 0.1;
        } else {
            // Tree surface
            float hue = hit.y * 0.5;
            vec3 treeCol = vec3(
                0.5 + 0.5 * cos(6.2831 * (hue + 0.0)),
                0.5 + 0.5 * cos(6.2831 * (hue + 0.33)),
                0.5 + 0.5 * cos(6.2831 * (hue + 0.67))
            ) * 0.7;

            // Voxel edge darkening
            vec3 absN = abs(n);
            float maxComp = max(absN.x, max(absN.y, absN.z));
            vec3 localUV;
            if (maxComp == absN.x) localUV = vec3(pos.y, pos.z, pos.x);
            else if (maxComp == absN.y) localUV = vec3(pos.x, pos.z, pos.y);
            else localUV = vec3(pos.x, pos.y, pos.z);
            vec2 faceUV = fract(localUV.xy / (cubeSize * 2.0 * treeScale));
            float edge = max(abs(faceUV.x - 0.5), abs(faceUV.y - 0.5));
            treeCol = mix(treeCol, treeCol * 0.3, smoothstep(0.35, 0.5, edge) * 0.6);

            // Lighting
            vec3 viewDir = normalize(ro - pos);
            float spec = pow(max(dot(reflect(-lightDir, n), viewDir), 0.0), 32.0);
            col = treeCol * (diff * shad * 0.8 + amb) + vec3(0.3) * spec * shad;

            // Animated glow
            float effGlow = hit.z;
            col += glowColor1.rgb * effGlow * effGlow * 2.0;
            col += glowColor2.rgb * effGlow * 1.5;
        }

        // Fog
        col = mix(col, skyColor.rgb, 1.0 - exp(-totalDist * fogDensity));
    }

    // Tone map
    col = col / (col + 1.0);
    col = pow(col, vec3(0.85));

    // Vignette
    vec2 vuv = gl_FragCoord.xy / RENDERSIZE.xy;
    float vig = 1.0 - dot((vuv - 0.5) * 1.25, (vuv - 0.5) * 1.25);
    col *= clamp(vig, 0.0, 1.0);

    gl_FragColor = vec4(col, 1.0);
}
