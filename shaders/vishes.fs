/*{
  "DESCRIPTION": "Vishes — Cell Colony. Raymarched 3D SDF organism: nucleus + orbiting cells pulsing with neon bioluminescence. Standalone generator.",
  "CREDIT": "ShaderClaw — full 3D rewrite replacing 2D grid walkers",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "cellCount",  "LABEL": "Cell Count",  "TYPE": "float", "MIN": 2.0,  "MAX": 16.0, "DEFAULT": 10.0 },
    { "NAME": "orbitSpeed", "LABEL": "Orbit Speed", "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.4 },
    { "NAME": "pulseRate",  "LABEL": "Pulse Rate",  "TYPE": "float", "MIN": 0.0,  "MAX": 4.0,  "DEFAULT": 1.2 },
    { "NAME": "glowPeak",   "LABEL": "HDR Glow",   "TYPE": "float", "MIN": 1.0,  "MAX": 4.0,  "DEFAULT": 2.5 },
    { "NAME": "audioMod",   "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

// Cell Colony — 3D SDF raymarch. Completely replaces the 2D grid-walker system.

float sdSphere(vec3 p, float r) { return length(p) - r; }

float smin(float a, float b, float k) {
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * k * 0.25;
}

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

// 4-color neon palette: magenta, cyan, gold, lime
vec3 cellColor(float id) {
    int i = int(mod(id, 4.0));
    if (i == 0) return vec3(1.0, 0.0, 0.7);   // magenta
    if (i == 1) return vec3(0.0, 0.9, 1.0);   // cyan
    if (i == 2) return vec3(1.0, 0.75, 0.0);  // gold
    return vec3(0.4, 1.0, 0.0);               // lime
}

// Signed distance to the colony SDF
float colonySDF(vec3 p, float t, int N) {
    // Central nucleus — pulsing
    float pulse = sin(t * pulseRate) * 0.05;
    float d = sdSphere(p, 0.28 + pulse);

    // Orbiting cells
    for (int i = 0; i < 16; i++) {
        if (i >= N) break;
        float fi = float(i);
        float phase = fi * 6.2832 / float(max(N, 1));
        float speed = orbitSpeed * (0.7 + hash11(fi * 3.7) * 0.6);
        float orbitR = 0.55 + hash11(fi * 7.1) * 0.35;
        float elevAngle = (hash11(fi * 11.3) - 0.5) * 1.2;
        // 3D orbit position
        float a = phase + t * speed;
        vec3 cpos = vec3(
            orbitR * cos(a) * cos(elevAngle),
            orbitR * sin(elevAngle) + sin(t * pulseRate * 0.7 + fi) * 0.05,
            orbitR * sin(a) * cos(elevAngle)
        );
        float cellR = 0.12 + hash11(fi * 5.3) * 0.08
                    + sin(t * pulseRate + fi * 2.1) * 0.025;
        d = smin(d, sdSphere(p - cpos, cellR), 0.08);
    }
    return d;
}

// Compute normal via central differences
vec3 colonyNormal(vec3 p, float t, int N) {
    float e = 0.002;
    float d0 = colonySDF(p, t, N);
    return normalize(vec3(
        colonySDF(p + vec3(e, 0, 0), t, N) - d0,
        colonySDF(p + vec3(0, e, 0), t, N) - d0,
        colonySDF(p + vec3(0, 0, e), t, N) - d0
    ));
}

// Closest cell index for coloring
int closestCell(vec3 p, float t, int N) {
    float best = 1e9;
    int bestI = 0;
    for (int i = 0; i < 16; i++) {
        if (i >= N) break;
        float fi = float(i);
        float phase = fi * 6.2832 / float(max(N, 1));
        float speed = orbitSpeed * (0.7 + hash11(fi * 3.7) * 0.6);
        float orbitR = 0.55 + hash11(fi * 7.1) * 0.35;
        float elevAngle = (hash11(fi * 11.3) - 0.5) * 1.2;
        float a = phase + t * speed;
        vec3 cpos = vec3(
            orbitR * cos(a) * cos(elevAngle),
            orbitR * sin(elevAngle),
            orbitR * sin(a) * cos(elevAngle)
        );
        float d = length(p - cpos);
        if (d < best) { best = d; bestI = i; }
    }
    return bestI;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float audio = 1.0 + audioLevel * audioMod * 0.4 + audioBass * audioMod * 0.2;
    int N = int(clamp(cellCount, 2.0, 16.0));

    // Camera: slow orbit
    float camA = t * 0.12;
    float camDist = 2.2 / audio;
    vec3 ro = vec3(sin(camA) * camDist, 0.25 + sin(t * 0.17) * 0.2, cos(camA) * camDist);
    vec3 target = vec3(0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0, 1, 0)));
    vec3 up = cross(right, fwd);

    vec2 screen = (uv - 0.5) * vec2(aspect, 1.0);
    vec3 rd = normalize(fwd + screen.x * right + screen.y * up);

    // 64-step raymarch
    float dist = 0.0;
    float hit = 0.0;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dist;
        float d = colonySDF(p, t, N);
        if (d < 0.002) { hit = 1.0; break; }
        if (dist > 6.0) break;
        dist += d * 0.75;
    }

    vec3 col = vec3(0.0);  // black void background

    if (hit > 0.5) {
        vec3 p = ro + rd * dist;
        vec3 n = colonyNormal(p, t, N);

        int cellIdx = closestCell(p, t, N);
        vec3 baseCol = (length(p) < 0.35) ? vec3(0.9, 0.9, 1.0) : cellColor(float(cellIdx));

        // Lighting: key + rim + specular
        vec3 lightDir = normalize(vec3(1.2, 1.5, 0.8));
        float diff = max(dot(n, lightDir), 0.0);
        vec3 viewDir = normalize(ro - p);
        vec3 halfV = normalize(lightDir + viewDir);
        float spec = pow(max(dot(n, halfV), 0.0), 64.0);
        float rim  = pow(1.0 - max(dot(n, viewDir), 0.0), 2.5);

        col = baseCol * (0.05 + diff * 0.65) * glowPeak;
        col += vec3(1.0) * spec * 2.0 * glowPeak;     // HDR specular
        col += baseCol * rim * 1.2 * glowPeak;         // HDR rim glow

        // Edge ink (black silhouette at grazing angles)
        float edge = 1.0 - smoothstep(0.0, 0.25, dot(n, viewDir));
        col *= 1.0 - edge * 0.85;

        col *= audio;
    }

    gl_FragColor = vec4(col, 1.0);
}
