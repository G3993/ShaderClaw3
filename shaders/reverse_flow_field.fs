/*{
  "DESCRIPTION": "Plasma Flow Tubes 3D — 6 animated capsule plasma tubes in a flowing 3D network with volumetric emission glow",
  "CREDIT": "ShaderClaw auto-improve 2026-05-06",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Speed",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 4.0  },
    { "NAME": "tubeWidth",  "LABEL": "Tube Width", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.01, "MAX": 0.3  },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",   "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0  },
    { "NAME": "audioReact", "LABEL": "Audio React","TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0  }
  ]
}*/

// ── Palette ────────────────────────────────────────────────────────────────
// Void:           vec3(0.0, 0.0, 0.01)  — near-black deep space
// Electric cyan:  vec3(0.0, 2.5, 2.0)  — HDR cyan plasma
// Violet:         vec3(0.8, 0.0, 2.5)  — HDR violet plasma
// Lime green:     vec3(0.5, 2.5, 0.0)  — HDR lime plasma
// Hot white core: vec3(2.5, 2.5, 2.0)  — HDR specular core

const float PI = 3.14159265359;

// ── Tube color by index (mod 3) ────────────────────────────────────────────
vec3 tubeColor(int i) {
    int ci = int(mod(float(i), 3.0));
    if (ci == 0) return vec3(0.0, 2.5, 2.0);   // electric cyan
    if (ci == 1) return vec3(0.8, 0.0, 2.5);   // violet
    return vec3(0.5, 2.5, 0.0);                 // lime green
}

// ── Animated tube endpoints ────────────────────────────────────────────────
vec3 tubeA(int i, float tm) {
    float fi = float(i);
    float ph = fi * 1.0472; // 60-degree spacing
    return vec3(
        cos(ph) * 1.5 + sin(tm * 0.3 + fi) * 0.4,
        sin(fi * 0.7) * 1.0 + cos(tm * 0.2 + fi * 1.3) * 0.3,
        sin(ph) * 1.5 + cos(tm * 0.25 + fi * 0.8) * 0.4
    );
}

vec3 tubeB(int i, float tm) {
    float fi = float(i);
    float ph = fi * 1.0472 + PI;
    return vec3(
        cos(ph) * 1.8 + sin(tm * 0.28 + fi * 1.1) * 0.35,
        sin(fi * 1.2) * 0.8 + cos(tm * 0.22 + fi * 0.9) * 0.25,
        sin(ph) * 1.8 + cos(tm * 0.27 + fi * 1.4) * 0.35
    );
}

// ── Capsule SDF ────────────────────────────────────────────────────────────
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a;
    vec3 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

// ── Scene SDF: min over all 6 tubes, also returns tube index ───────────────
float mapTubes(vec3 p, float tm, float twidth, out int hitIdx) {
    float d = 1e10;
    hitIdx = 0;
    for (int i = 0; i < 6; i++) {
        vec3 a = tubeA(i, tm);
        vec3 b = tubeB(i, tm);
        float di = sdCapsule(p, a, b, twidth);
        if (di < d) {
            d = di;
            hitIdx = i;
        }
    }
    return d;
}

float mapScene(vec3 p, float tm, float twidth) {
    int dummy;
    return mapTubes(p, tm, twidth, dummy);
}

// ── Normal via central differences ────────────────────────────────────────
vec3 calcNormal(vec3 p, float tm, float twidth) {
    float eps = 0.001;
    vec3 e = vec3(eps, 0.0, 0.0);
    return normalize(vec3(
        mapScene(p + e.xyy, tm, twidth) - mapScene(p - e.xyy, tm, twidth),
        mapScene(p + e.yxy, tm, twidth) - mapScene(p - e.yxy, tm, twidth),
        mapScene(p + e.yyx, tm, twidth) - mapScene(p - e.yyx, tm, twidth)
    ));
}

// ── Hash for fwidth AA ────────────────────────────────────────────────────
float hash11(float n) {
    return fract(sin(n * 91.7) * 43758.5453);
}

void main() {
    float tm = TIME * speed;

    // Audio modulation
    float audioMod = 1.0 + audioReact * (audioBass * 0.4 + audioLevel * 0.2);
    float twidth = tubeWidth * audioMod;
    float glowScale = hdrPeak * audioMod;

    // ── Camera orbit around tube cluster ──────────────────────────────────
    float camAngle = tm * 0.18;
    float camTilt  = sin(tm * 0.11) * 0.35;
    vec3 ro = vec3(
        cos(camAngle) * 5.5,
        sin(camTilt) * 2.0,
        sin(camAngle) * 5.5
    );
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);

    // ── Ray direction ─────────────────────────────────────────────────────
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    vec3 rd = normalize(fwd + uv.x * right + uv.y * up);

    // ── Raymarching (64 steps) ────────────────────────────────────────────
    float t = 0.0;
    float tmax = 12.0;
    bool hit = false;
    int hitTubeIdx = 0;
    vec3 hitP = vec3(0.0);

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * t;
        int idx;
        float d = mapTubes(p, tm, twidth, idx);
        if (d < 0.002) {
            hit = true;
            hitTubeIdx = idx;
            hitP = p;
            break;
        }
        t += d * 0.8;
        if (t > tmax) break;
    }

    // ── Volumetric glow pass ─────────────────────────────────────────────
    // March along ray accumulating exp(-dist/width) glow from each tube
    vec3 glowAccum = vec3(0.0);
    float glowT = 0.0;
    float glowStep = tmax / 48.0;
    for (int gi = 0; gi < 48; gi++) {
        vec3 gp = ro + rd * glowT;
        for (int ti = 0; ti < 6; ti++) {
            vec3 a = tubeA(ti, tm);
            vec3 b = tubeB(ti, tm);
            float gd = sdCapsule(gp, a, b, twidth);
            float falloff = exp(-max(gd, 0.0) / (twidth * 3.5));
            glowAccum += tubeColor(ti) * falloff * glowStep * 0.7;
        }
        glowT += glowStep;
    }

    // ── Surface shading (emission only — plasma glow from inside) ─────────
    vec3 col = vec3(0.0, 0.0, 0.01); // void background

    if (hit) {
        vec3 n = calcNormal(hitP, tm, twidth);
        vec3 baseColor = tubeColor(hitTubeIdx);

        // Edge factor: bright core, dimmer at grazing
        float edgeFade = abs(dot(n, -rd));
        // fwidth AA on tube SDF edge
        float sdfEdge = mapScene(hitP, tm, twidth);
        float aaWidth = fwidth(sdfEdge);
        float edgeSmoother = smoothstep(0.0, aaWidth * 2.0, edgeFade);

        // Core emission: hot white at center, colored at rim
        vec3 coreColor = mix(baseColor, vec3(2.5, 2.5, 2.0), pow(edgeFade, 3.0));
        col = coreColor * glowScale * edgeSmoother;
    }

    // Add volumetric glow halo (visible even where no surface hit)
    col += glowAccum * glowScale * 0.5;

    // HDR output — no clamp, no tonemapping, no ACES
    gl_FragColor = vec4(col, 1.0);
}
