/*{
  "DESCRIPTION": "Neon Neural Network — 3D raymarched graph of glowing nodes connected by pulsing edge tubes, deep black void with HDR activation bursts",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "Easel / ShaderClaw v3",
  "INPUTS": [
    { "NAME": "nodeCount",   "LABEL": "Nodes",        "TYPE": "float", "MIN": 3.0, "MAX": 12.0, "DEFAULT": 7.0  },
    { "NAME": "cameraSpeed", "LABEL": "Camera Speed", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.2  },
    { "NAME": "pulseRate",   "LABEL": "Pulse Rate",   "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 1.2  },
    { "NAME": "tubeRadius",  "LABEL": "Tube Radius",  "TYPE": "float", "MIN": 0.005,"MAX": 0.05, "DEFAULT": 0.018 },
    { "NAME": "hdrBoost",    "LABEL": "HDR Boost",    "TYPE": "float", "MIN": 1.0, "MAX": 4.0,  "DEFAULT": 2.5  },
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0  }
  ]
}*/

precision highp float;

// ── Palette: 4 neon colors ────────────────────────────────────────────────────
const vec3 CYAN_NODE  = vec3(0.00, 1.00, 1.00);
const vec3 MAGENTA_EDGE = vec3(1.00, 0.00, 0.80);
const vec3 GOLD_BURST = vec3(1.00, 0.80, 0.00);
const vec3 VIOLET_RIM = vec3(0.40, 0.00, 1.00);
const vec3 VOID       = vec3(0.00, 0.00, 0.01);

float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }
vec3 nodePos(float i, float N) {
    float ang  = i / N * 6.28318530 + hash(i) * 1.2;
    float elev = (hash(i + 0.5) - 0.5) * 2.2;
    float rad  = 1.0 + hash(i + 0.3) * 0.8;
    return vec3(cos(ang) * rad, elev, sin(ang) * rad);
}

// SDF: sphere
float sdSphere(vec3 p, vec3 c, float r) { return length(p - c) - r; }

// SDF: capsule (edge tube between two nodes)
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b - a, ap = p - a;
    float t = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
    return length(ap - ab * t) - r;
}

// Scene SDF — nodes + edges
float sceneSDF(vec3 p, float N, float t) {
    float d = 1e9;
    float nodeR = 0.09 + 0.02 * sin(t * 2.1);
    for (int i = 0; i < 12; i++) {
        if (float(i) >= N) break;
        vec3 pA = nodePos(float(i), N);
        d = min(d, sdSphere(p, pA, nodeR));
        // Edge to next node (ring topology + skip-one connections)
        vec3 pB = nodePos(mod(float(i) + 1.0, N), N);
        d = min(d, sdCapsule(p, pA, pB, tubeRadius));
        if (float(i) < N - 2.0) {
            vec3 pC = nodePos(mod(float(i) + 2.0, N), N);
            d = min(d, sdCapsule(p, pA, pC, tubeRadius * 0.6));
        }
    }
    return d;
}

// Estimate normal via central differences
vec3 sceneNormal(vec3 p, float N, float t) {
    const float e = 0.001;
    return normalize(vec3(
        sceneSDF(p + vec3(e,0,0), N, t) - sceneSDF(p - vec3(e,0,0), N, t),
        sceneSDF(p + vec3(0,e,0), N, t) - sceneSDF(p - vec3(0,e,0), N, t),
        sceneSDF(p + vec3(0,0,e), N, t) - sceneSDF(p - vec3(0,0,e), N, t)
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t   = TIME;
    float aud = 1.0 + (audioLevel + audioBass * 0.7) * audioReact * 0.6;
    float N   = nodeCount;

    // Orbiting camera
    float camAng = t * cameraSpeed;
    float camElev = sin(t * cameraSpeed * 0.4) * 0.6;
    vec3 camPos = vec3(cos(camAng) * 4.5, camElev + 0.5, sin(camAng) * 4.5);
    vec3 target = vec3(0.0);
    vec3 fwd = normalize(target - camPos);
    vec3 right = normalize(cross(fwd, vec3(0,1,0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + uv.x * right * 0.8 + uv.y * up * 0.8);

    // Raymarch
    float dist = 0.0;
    float hit  = 0.0;
    for (int i = 0; i < 64; i++) {
        vec3 p = camPos + rd * dist;
        float d = sceneSDF(p, N, t);
        if (d < 0.001) { hit = 1.0; break; }
        if (dist > 20.0) break;
        dist += d;
    }

    vec3 col = VOID;

    if (hit > 0.5) {
        vec3 p   = camPos + rd * dist;
        vec3 nor = sceneNormal(p, N, t);

        // Which element did we hit?
        float nodeR = 0.09 + 0.02 * sin(t * 2.1);
        bool isNode = false;
        for (int i = 0; i < 12; i++) {
            if (float(i) >= N) break;
            if (sdSphere(p, nodePos(float(i), N), nodeR) < 0.003) { isNode = true; break; }
        }

        vec3 lightDir = normalize(vec3(1.2, 2.0, 0.8));
        float diff = max(dot(nor, lightDir), 0.0);
        float spec = pow(max(dot(reflect(-lightDir, nor), -rd), 0.0), 32.0);

        // Pulse wave along edges
        float pulse = 0.5 + 0.5 * sin(t * pulseRate * 3.0 - dot(p, vec3(1.0, 0.3, 0.7)) * 4.0);

        if (isNode) {
            // Cyan node with gold activation burst
            float activation = 0.5 + 0.5 * sin(t * pulseRate * 2.0 + dot(p, vec3(0.5)));
            col = (CYAN_NODE * (0.5 + diff * 0.5) + GOLD_BURST * activation * 0.8
                   + vec3(1.0) * spec * 2.0) * hdrBoost * aud;
        } else {
            // Magenta tube with violet rim
            col = (MAGENTA_EDGE * (0.3 + diff * 0.4 + pulse * 0.3)
                   + VIOLET_RIM * spec * 1.5) * hdrBoost * aud;
        }
    }

    // Additive node glow (unoccluded)
    for (int i = 0; i < 12; i++) {
        if (float(i) >= N) break;
        vec3 pA = nodePos(float(i), N);
        // Project node onto ray for glow halo
        float tProj = dot(pA - camPos, rd);
        if (tProj > 0.0 && tProj < dist - 0.1) {
            vec3 closest = camPos + rd * tProj;
            float dist2  = length(closest - pA);
            float glow   = exp(-dist2 * dist2 * 14.0) * 0.4;
            float pulse  = 0.7 + 0.3 * sin(t * pulseRate * 1.8 + float(i) * 1.3);
            col += CYAN_NODE * glow * pulse * hdrBoost * aud;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
