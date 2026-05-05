/*{
    "DESCRIPTION": "Chromatic Spheres — 5 neon HDR spheres in a ring, orbiting camera, studio Phong lighting",
    "CREDIT": "ShaderClaw",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        {
            "NAME": "sphereCount",
            "TYPE": "float",
            "MIN": 1.0,
            "MAX": 5.0,
            "DEFAULT": 5.0,
            "LABEL": "Sphere Count"
        },
        {
            "NAME": "orbitRadius",
            "TYPE": "float",
            "MIN": 0.5,
            "MAX": 2.5,
            "DEFAULT": 1.3,
            "LABEL": "Orbit Radius"
        },
        {
            "NAME": "camDist",
            "TYPE": "float",
            "MIN": 3.0,
            "MAX": 10.0,
            "DEFAULT": 5.5,
            "LABEL": "Camera Distance"
        },
        {
            "NAME": "specPow",
            "TYPE": "float",
            "MIN": 8.0,
            "MAX": 128.0,
            "DEFAULT": 48.0,
            "LABEL": "Specular Power"
        },
        {
            "NAME": "audioPulse",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.6,
            "LABEL": "Audio Pulse"
        }
    ]
}*/

// Fully saturated neon palette — HDR peaks 2.5
vec3 sphereColor(int idx) {
    if (idx == 0) return vec3(2.5, 0.05, 0.05);   // neon red
    if (idx == 1) return vec3(0.05, 2.5, 0.05);   // neon green
    if (idx == 2) return vec3(0.05, 0.05, 2.5);   // neon blue
    if (idx == 3) return vec3(2.5, 2.0, 0.0);     // neon yellow
    return             vec3(0.0, 2.0, 2.5);        // neon cyan
}

// ---- SDF ----
struct Hit {
    float dist;
    int   id;
};

float sdSphere(vec3 p, vec3 c, float r) {
    return length(p - c) - r;
}

Hit mapScene(vec3 p, float sphR) {
    Hit h;
    h.dist = 1e9;
    h.id   = -1;

    float PI2 = 6.28318530718;
    int   n   = int(clamp(sphereCount, 1.0, 5.0));

    for (int i = 0; i < 5; i++) {
        if (i >= n) break;
        float fi    = float(i);
        float angle = PI2 * fi / float(n);
        float bob   = sin(TIME * 0.5 + fi * 1.3) * 0.4;
        vec3  ctr   = vec3(cos(angle) * orbitRadius, bob, sin(angle) * orbitRadius);
        float d     = sdSphere(p, ctr, sphR);
        if (d < h.dist) {
            h.dist = d;
            h.id   = i;
        }
    }
    return h;
}

vec3 calcNormal(vec3 p, float sphR) {
    vec2 e = vec2(0.0005, 0.0);
    return normalize(vec3(
        mapScene(p + e.xyy, sphR).dist - mapScene(p - e.xyy, sphR).dist,
        mapScene(p + e.yxy, sphR).dist - mapScene(p - e.yxy, sphR).dist,
        mapScene(p + e.yyx, sphR).dist - mapScene(p - e.yyx, sphR).dist
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x   *= RENDERSIZE.x / RENDERSIZE.y;

    // Audio-modulated sphere radius
    float sphR = 0.35 * (1.0 + audioLevel * audioPulse * 0.2);

    // Orbiting camera
    vec3 ro  = vec3(sin(TIME * 0.18) * camDist, 1.8, cos(TIME * 0.18) * camDist);
    vec3 ta  = vec3(0.0, 0.0, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 rgt = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up  = cross(rgt, fwd);
    vec3 rd  = normalize(fwd + uv.x * rgt + uv.y * up);

    // Studio lighting directions
    vec3 keyLight  = normalize(vec3(2.0, 3.0, 1.0));
    vec3 fillLight = normalize(vec3(-1.0, 0.5, -1.5));

    // Raymarch — 64 steps minimum
    float tMarch = 0.0;
    Hit   hit;
    hit.id   = -1;
    hit.dist = 1e9;
    vec3  hitP = ro;

    for (int s = 0; s < 96; s++) {
        hitP = ro + rd * tMarch;
        hit  = mapScene(hitP, sphR);
        if (hit.dist < 0.001) break;
        if (tMarch > 40.0)   { hit.id = -1; break; }
        tMarch += hit.dist;
    }

    vec3 col = vec3(0.0); // BLACK void background

    if (hit.id >= 0) {
        vec3 n       = calcNormal(hitP, sphR);
        vec3 baseCol = sphereColor(hit.id);

        // Phong lighting
        float diff    = max(dot(n, keyLight), 0.0);
        float fill    = max(dot(n, fillLight), 0.0) * 0.3;
        float ambient = 0.06;

        // Specular — peak 3.0 HDR
        float spec = pow(max(dot(reflect(-keyLight, n), -rd), 0.0), specPow) * 3.0;

        col = baseCol * (diff + fill + ambient) + vec3(spec);

        // Rim darkening at silhouette edges
        float rim = 1.0 - max(dot(n, -rd), 0.0);
        col *= (1.0 - rim * rim * 0.5);

        // fwidth() AA: darken at SDF zero-crossing for ink-outline feel
        float outerD = abs(hit.dist);
        float fw     = fwidth(outerD);
        col = mix(col, vec3(0.0), smoothstep(fw * 2.0, 0.0, outerD));
    }

    gl_FragColor = vec4(col, 1.0);
}
