/*{
    "DESCRIPTION": "Spectral Prism 3D — glass triangular prism with chromatic dispersion beams, volumetric glow",
    "CREDIT": "ShaderClaw auto-improve 2026-05-06",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "speed",      "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 3.0,  "LABEL": "Speed" },
        { "NAME": "beamSpread", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0,  "LABEL": "Beam Spread" },
        { "NAME": "hdrPeak",    "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 5.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioReact", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio React" }
    ]
}*/

// ---------- SDF helpers ----------
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float distToSegment(vec3 p, vec3 a, vec3 b) {
    vec3 pa = p - a;
    vec3 ba = b - a;
    float t = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * t);
}

// ---------- Scene SDF ----------
float mapScene(vec3 p) {
    // Glass prism (box approximation)
    float prism = sdBox(p, vec3(0.22, 0.75, 0.20));
    return prism;
}

// ---------- Normal via central differences ----------
vec3 calcNormal(vec3 p) {
    const float eps = 0.002;
    return normalize(vec3(
        mapScene(p + vec3(eps, 0.0, 0.0)) - mapScene(p - vec3(eps, 0.0, 0.0)),
        mapScene(p + vec3(0.0, eps, 0.0)) - mapScene(p - vec3(0.0, eps, 0.0)),
        mapScene(p + vec3(0.0, 0.0, eps)) - mapScene(p - vec3(0.0, 0.0, eps))
    ));
}

// ---------- Beam segments ----------
// Input beam: from left, going right, horizontally centered
// Output beams: emerge from right face, spread at angles

struct Beam {
    vec3 a;
    vec3 b;
    vec3 col;
    float width;
};

Beam getBeam(int idx, float spread, float audio, float hdrPk) {
    Beam bm;
    // Input beam — warm white
    if (idx == 0) {
        bm.a = vec3(-2.8, 0.0, 0.0);
        bm.b = vec3(-0.22, 0.0, 0.0);
        bm.col = vec3(2.0, 1.8, 1.4) * audio * hdrPk * 0.55;
        bm.width = 0.035;
    }
    // Crimson output — goes up-right
    else if (idx == 1) {
        bm.a = vec3(0.22, 0.0, 0.0);
        bm.b = vec3(2.5,  1.4 * spread * 2.0, 0.0);
        bm.col = vec3(2.0, 0.0, 0.05) * audio * hdrPk * 0.70;
        bm.width = 0.040;
    }
    // Electric blue output — goes right-center
    else if (idx == 2) {
        bm.a = vec3(0.22, 0.0, 0.0);
        bm.b = vec3(2.5,  0.0,  spread * 0.3);
        bm.col = vec3(0.0, 0.5, 3.0) * audio * hdrPk * 0.75;
        bm.width = 0.038;
    }
    // Acid yellow output — goes down-right
    else {
        bm.a = vec3(0.22, 0.0, 0.0);
        bm.b = vec3(2.5, -1.4 * spread * 2.0, 0.0);
        bm.col = vec3(2.5, 1.8, 0.0) * audio * hdrPk * 0.68;
        bm.width = 0.040;
    }
    return bm;
}

void main() {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= aspect;

    float audio = 1.0 + (audioLevel * 0.5 + audioBass * 0.5) * audioReact;

    // --- Camera orbit ---
    float camAngle = TIME * speed * 0.12;
    float camR = 5.5;
    vec3 ro = vec3(sin(camAngle) * camR, 0.35 + sin(TIME * speed * 0.07) * 0.4, cos(camAngle) * camR);
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);

    float fov = 1.3;
    vec3 rd = normalize(fwd + uv.x * right * fov * 0.5 + uv.y * up * fov * 0.5);

    // --- Raymarching (64 steps) ---
    float t = 0.0;
    float hitDist = -1.0;
    vec3 hitPos = vec3(0.0);
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * t;
        float d = mapScene(p);
        if (d < 0.001) {
            hitDist = t;
            hitPos = p;
            break;
        }
        t += d;
        if (t > 20.0) break;
    }

    // --- Prism shading ---
    vec3 col = vec3(0.0);
    if (hitDist > 0.0) {
        vec3 n = calcNormal(hitPos);
        // fwidth() AA on SDF iso-edge
        float edge = fwidth(mapScene(hitPos));
        float edgeMask = smoothstep(0.0, edge * 2.0, abs(mapScene(hitPos)));

        // Glass-like: refract + specular
        vec3 lightDir = normalize(vec3(-1.5, 2.0, 1.0));
        float diff = max(dot(n, lightDir), 0.0);
        vec3 viewDir = normalize(ro - hitPos);
        float spec = pow(max(dot(reflect(-lightDir, n), viewDir), 0.0), 64.0);
        // Ice-blue glass tint
        vec3 glassTint = vec3(0.4, 0.7, 1.0) * 0.35 * diff;
        vec3 specColor = vec3(2.2, 2.2, 2.4) * spec;
        col = glassTint + specColor;
        col *= (1.0 - edgeMask * 0.4);
    }

    // --- Volumetric beam accumulation ---
    vec3 volAcc = vec3(0.0);
    float volStep = 0.08;
    int volSteps = 50;
    for (int vi = 0; vi < volSteps; vi++) {
        float vt = float(vi) * volStep + 0.1;
        vec3 vp = ro + rd * vt;

        for (int bi = 0; bi < 4; bi++) {
            Beam bm = getBeam(bi, beamSpread, audio, hdrPeak);
            float dist = distToSegment(vp, bm.a, bm.b);
            // AA on beam edge via fwidth approximation
            float aaW = bm.width * 0.15;
            float glow = exp(-dist / (bm.width + aaW));
            // AA sharpening at iso edge
            float innerEdge = smoothstep(bm.width, bm.width - aaW, dist);
            volAcc += bm.col * (glow * 0.07 + innerEdge * 0.5) * volStep;
        }
    }

    col += volAcc;

    gl_FragColor = vec4(col, 1.0);
}
