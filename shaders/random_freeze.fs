/*{
    "DESCRIPTION": "Infernal Crown — spinning volcanic torus crown with arching magma-spine capsules, dramatic rim lighting in black void",
    "CREDIT": "ShaderClaw",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        {
            "NAME": "crownRadius",
            "TYPE": "float",
            "MIN": 0.5,
            "MAX": 2.0,
            "DEFAULT": 1.0,
            "LABEL": "Crown Radius"
        },
        {
            "NAME": "spineCount",
            "TYPE": "float",
            "MIN": 3.0,
            "MAX": 8.0,
            "DEFAULT": 5.0,
            "LABEL": "Spine Count"
        },
        {
            "NAME": "spinSpeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 3.0,
            "DEFAULT": 1.2,
            "LABEL": "Spin Speed"
        },
        {
            "NAME": "crownTilt",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.5,
            "DEFAULT": 0.2,
            "LABEL": "Crown Tilt"
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
            "NAME": "hdrPeak",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 4.0,
            "DEFAULT": 2.8,
            "LABEL": "HDR Peak"
        }
    ]
}*/

// ---- Rotation helpers ----
mat3 rotY(float a) {
    float c = cos(a), s = sin(a);
    return mat3( c, 0.0,  s,
                 0.0, 1.0, 0.0,
                -s, 0.0,  c);
}
mat3 rotX(float a) {
    float c = cos(a), s = sin(a);
    return mat3(1.0, 0.0, 0.0,
                0.0,  c,  -s,
                0.0,  s,   c);
}

// ---- SDF primitives ----
float sdTorus(vec3 p, vec2 t) {
    vec2 q = vec2(length(p.xz) - t.x, p.y);
    return length(q) - t.y;
}

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3  ab = b - a;
    vec3  ap = p - a;
    float t  = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
    return length(ap - ab * t) - r;
}

// ---- Scene ----
// MaterialIDs: 0=torus base, 1=spine capsules
struct Hit {
    float dist;
    int   matID;
};

Hit minHit(Hit a, Hit b) {
    return (a.dist < b.dist) ? a : b;
}

Hit mapScene(vec3 p, float yScale) {
    // Spin + tilt the entire crown
    mat3 ry  = rotY(TIME * spinSpeed);
    mat3 rx  = rotX(crownTilt);
    vec3 pR  = rx * ry * p;

    // Audio modulates Y scale — crown pulses taller with bass
    pR.y /= yScale;

    // Torus base ring — R=crownRadius, r=0.12
    Hit hTorus;
    hTorus.dist  = sdTorus(pR, vec2(crownRadius, 0.12));
    hTorus.matID = 0;

    Hit hBest = hTorus;

    float PI2 = 6.28318530718;
    int   n   = int(clamp(spineCount, 3.0, 8.0));

    for (int i = 0; i < 8; i++) {
        if (i >= n) break;
        float fi    = float(i);
        float angle = PI2 * fi / float(n);
        float ca    = cos(angle);
        float sa    = sin(angle);

        // Spine: from torus ring up to a narrowing tip above center
        vec3 a = vec3(ca * crownRadius,        0.0, sa * crownRadius);
        vec3 b = vec3(ca * crownRadius * 0.3,  1.2, sa * crownRadius * 0.3);

        Hit hSpine;
        hSpine.dist  = sdCapsule(pR, a, b, 0.08);
        hSpine.matID = 1;

        hBest = minHit(hBest, hSpine);
    }

    return hBest;
}

vec3 calcNormal(vec3 p, float ys) {
    vec2 e = vec2(0.0005, 0.0);
    return normalize(vec3(
        mapScene(p + e.xyy, ys).dist - mapScene(p - e.xyy, ys).dist,
        mapScene(p + e.yxy, ys).dist - mapScene(p - e.yxy, ys).dist,
        mapScene(p + e.yyx, ys).dist - mapScene(p - e.yyx, ys).dist
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x   *= RENDERSIZE.x / RENDERSIZE.y;

    // Audio drives Y pulse — modulates scale, never gates
    float yScale = 1.0 + audioLevel * audioPulse * 0.35;

    // Orbiting camera at slight elevation — different speed from crown spin
    vec3 ro  = vec3(sin(TIME * 0.22) * 3.8, 2.2, cos(TIME * 0.22) * 3.8);
    vec3 ta  = vec3(0.0, 0.5, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 rgt = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up  = cross(rgt, fwd);
    vec3 rd  = normalize(fwd + uv.x * rgt + uv.y * up);

    // Lighting
    vec3 keyLight = normalize(vec3(0.5, 4.0, 0.5));
    vec3 specCol  = vec3(3.0, 2.5, 1.0);    // white-hot specular HDR 3.0
    vec3 rimCol   = vec3(1.2, 0.0, 2.5);    // violet rim HDR — contrasting cool

    // Raymarch — 96 steps
    float tMarch = 0.0;
    Hit   hit;
    hit.matID = -1;
    hit.dist  = 1e9;
    vec3  hitP = ro;

    for (int s = 0; s < 96; s++) {
        hitP = ro + rd * tMarch;
        hit  = mapScene(hitP, yScale);
        if (hit.dist < 0.001) break;
        if (tMarch > 30.0)   { hit.matID = -1; break; }
        tMarch += max(hit.dist, 0.002);
    }

    // Char black void background
    vec3 col = vec3(0.0);

    if (hit.matID >= 0) {
        vec3 n = calcNormal(hitP, yScale);

        // Magma gradient by world Y: low = deep ember, high = magma core
        // col = mix(ember, magma, yN) * (hdrPeak / 2.8)
        float yN     = clamp(hitP.y / 1.5, 0.0, 1.0);
        vec3  ember  = vec3(2.0, 0.12, 0.0);   // deep ember HDR
        vec3  magma  = vec3(3.0, 0.6,  0.0);   // magma core HDR peak 3.0
        vec3  baseCol = mix(ember, magma, yN) * (hdrPeak / 2.8);

        // Diffuse + ambient
        float diff = max(dot(n, keyLight), 0.0);
        float amb  = 0.05;

        // Blinn-Phong specular — white-hot peak 3.0
        vec3  halfV = normalize(keyLight - rd);
        float spec  = pow(max(dot(n, halfV), 0.0), 32.0) * 3.0;

        // Violet rim light — dramatic contrast against hot orange
        float rimF = pow(1.0 - max(dot(n, -rd), 0.0), 2.0);
        vec3  rim  = rimCol * rimF * 0.8;

        col = baseCol * (diff + amb) + specCol * spec + rim;

        // fwidth() AA on torus/spine iso-edges — smooth dark boundary
        float edgeD = abs(hit.dist);
        float fw    = fwidth(edgeD);
        col = mix(col, vec3(0.0), smoothstep(fw * 2.0, 0.0, edgeD));
    }

    gl_FragColor = vec4(col, 1.0);
}
