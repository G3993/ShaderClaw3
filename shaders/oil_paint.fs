/*{
    "DESCRIPTION": "Lacquerware Totem — three Japanese lacquerware objects (torus, sphere, tower) stacked on a dark platform, warm Blinn-Phong lighting",
    "CREDIT": "ShaderClaw",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        {
            "NAME": "orbitSpeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.2,
            "LABEL": "Orbit Speed"
        },
        {
            "NAME": "spinSpeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.5,
            "LABEL": "Spin Speed"
        },
        {
            "NAME": "specPow",
            "TYPE": "float",
            "MIN": 8.0,
            "MAX": 256.0,
            "DEFAULT": 64.0,
            "LABEL": "Specular Power"
        },
        {
            "NAME": "lacquerColor",
            "TYPE": "color",
            "DEFAULT": [0.9, 0.02, 0.01, 1.0],
            "LABEL": "Lacquer Color"
        },
        {
            "NAME": "audioPulse",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.5,
            "LABEL": "Audio Pulse"
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
    // t.x = major radius, t.y = tube radius
    vec2 q = vec2(length(p.xz) - t.x, p.y);
    return length(q) - t.y;
}

float sdSphere(vec3 p, float r) {
    return length(p) - r;
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// ---- Scene ----
// MaterialIDs: 0=torus(lacquer), 1=sphere(jade), 2=tower(lacquer), 3=floor(platform)
struct Hit {
    float dist;
    int   matID;
};

Hit minHit(Hit a, Hit b) {
    return (a.dist < b.dist) ? a : b;
}

Hit mapScene(vec3 p) {
    // Whole structure rotates (spin) and tilts (slow wobble)
    mat3 ry   = rotY(TIME * spinSpeed);
    mat3 rx   = rotX(sin(TIME * 0.3) * 0.12);
    vec3 pRot = rx * ry * p;

    // Floor plane at y = -0.4 (in world space, not rotated)
    Hit hFloor;
    hFloor.dist  = p.y + 0.4;
    hFloor.matID = 3;

    // Torus at base — y=0 in rotated space, R=0.9, r=0.25
    Hit hTorus;
    hTorus.dist  = sdTorus(pRot - vec3(0.0, 0.0, 0.0), vec2(0.9, 0.25));
    hTorus.matID = 0;

    // Sphere — y=0.85 in rotated space, r=0.45
    Hit hSphere;
    hSphere.dist  = sdSphere(pRot - vec3(0.0, 0.85, 0.0), 0.45);
    hSphere.matID = 1;

    // Thin tower box — y=1.65 in rotated space, b=(0.15, 0.4, 0.15)
    Hit hTower;
    hTower.dist  = sdBox(pRot - vec3(0.0, 1.65, 0.0), vec3(0.15, 0.4, 0.15));
    hTower.matID = 2;

    return minHit(hFloor, minHit(hTorus, minHit(hSphere, hTower)));
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.0005, 0.0);
    return normalize(vec3(
        mapScene(p + e.xyy).dist - mapScene(p - e.xyy).dist,
        mapScene(p + e.yxy).dist - mapScene(p - e.yxy).dist,
        mapScene(p + e.yyx).dist - mapScene(p - e.yyx).dist
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x   *= RENDERSIZE.x / RENDERSIZE.y;

    // Orbiting camera — cinematic framing
    vec3 ro  = vec3(sin(TIME * orbitSpeed) * 3.5, 1.5, cos(TIME * orbitSpeed) * 3.5);
    vec3 ta  = vec3(0.0, 0.6, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 rgt = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up  = cross(rgt, fwd);
    vec3 rd  = normalize(fwd + uv.x * rgt + uv.y * up);

    // Warm directional key light from above-right
    vec3 keyLight  = normalize(vec3(2.0, 4.0, 1.0));
    vec3 fillLight = normalize(vec3(-1.0, 0.5, -1.5));

    // Palette — fully saturated HDR
    // lacquerColor input scaled to HDR (user picks hue, we amplify)
    vec3 lacquer   = lacquerColor.rgb * 2.5;   // HDR peak ~2.5
    vec3 jade      = vec3(0.0, 2.0, 0.4);      // jade green HDR
    vec3 goldSpec  = vec3(2.5, 1.8, 0.0);      // gold leaf HDR
    vec3 specWhite = vec3(3.0, 2.8, 2.2);      // white-hot specular, HDR 3.0

    // Audio modulates specular brightness — scales, never gates
    float specBoost = 1.0 + audioLevel * audioPulse * 0.8;

    // Raymarch — 96 steps
    float tMarch = 0.0;
    Hit   hit;
    hit.matID = -1;
    hit.dist  = 1e9;
    vec3  hitP = ro;

    for (int s = 0; s < 96; s++) {
        hitP = ro + rd * tMarch;
        hit  = mapScene(hitP);
        if (hit.dist < 0.001) break;
        if (tMarch > 30.0)   { hit.matID = -1; break; }
        tMarch += hit.dist;
    }

    // Void black background
    vec3 col = vec3(0.0, 0.0, 0.01);

    if (hit.matID >= 0) {
        vec3 n = calcNormal(hitP);

        float diff    = max(dot(n, keyLight), 0.0);
        float fill    = max(dot(n, fillLight), 0.0) * 0.25;
        float ambient = 0.08;

        // Blinn-Phong half-vector specular
        vec3  halfV = normalize(keyLight - rd);
        float spec  = pow(max(dot(n, halfV), 0.0), specPow) * 2.5 * specBoost;

        vec3 baseCol;
        vec3 specContrib;

        if (hit.matID == 0) {
            // Torus — lacquer red with gold specular
            baseCol     = lacquer;
            specContrib = goldSpec * spec;
        } else if (hit.matID == 1) {
            // Sphere — jade green with white-hot specular
            baseCol     = jade;
            specContrib = specWhite * spec;
        } else if (hit.matID == 2) {
            // Tower — lacquer red with gold specular
            baseCol     = lacquer;
            specContrib = goldSpec * spec;
        } else {
            // Floor platform — near-black, no specular
            baseCol     = vec3(0.02, 0.01, 0.02);
            specContrib = vec3(0.0);
        }

        col = baseCol * (diff + fill + ambient) + specContrib;

        // fwidth() AA on iso-edge boundaries — smooth dark outline
        float edgeD = abs(hit.dist);
        float fw    = fwidth(edgeD);
        col = mix(col, vec3(0.0, 0.0, 0.01), smoothstep(fw * 2.0, 0.0, edgeD));
    }

    gl_FragColor = vec4(col, 1.0);
}
