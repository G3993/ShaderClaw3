/*{
    "DESCRIPTION": "Electric Cage 3D — infinite repeating wireframe box lattice, camera flies through",
    "CREDIT": "ShaderClaw auto-improve 2026-05-06",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "speed",      "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 4.0,  "LABEL": "Speed" },
        { "NAME": "cageSize",   "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 6.0,  "LABEL": "Cage Size" },
        { "NAME": "edgeWidth",  "TYPE": "float", "DEFAULT": 0.05,"MIN": 0.01,"MAX": 0.25, "LABEL": "Edge Width" },
        { "NAME": "hdrPeak",    "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 5.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioReact", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio React" }
    ]
}*/

// Wireframe box SDF
float sdBoxFrame(vec3 p, vec3 b, float e) {
    p = abs(p) - b;
    vec3 q = abs(p + e) - e;
    return min(min(
        length(max(vec3(p.x, q.y, q.z), 0.0)) + min(max(p.x, max(q.y, q.z)), 0.0),
        length(max(vec3(q.x, p.y, q.z), 0.0)) + min(max(q.x, max(p.y, q.z)), 0.0)),
        length(max(vec3(q.x, q.y, p.z), 0.0)) + min(max(q.x, max(q.y, p.z)), 0.0));
}

// Per-cell color: cycles magenta/cyan/yellow via hash of cell ID
vec3 cellColor(vec3 cellId, float tm) {
    float h = fract(sin(dot(cellId, vec3(127.1, 311.7, 74.7))) * 43758.5453 + tm * 0.04);
    vec3 A = vec3(2.0, 0.0, 1.5); // magenta
    vec3 B = vec3(0.0, 2.0, 2.0); // cyan
    vec3 C = vec3(2.5, 2.0, 0.0); // yellow
    if (h < 0.333) return mix(A, B, h * 3.0);
    if (h < 0.666) return mix(B, C, (h - 0.333) * 3.0);
    return mix(C, A, (h - 0.666) * 3.0);
}

// Scene map: returns (distance, cellId)
vec4 mapScene(vec3 p, float cs, float ew) {
    // Infinite repetition via pMod
    vec3 cellId = floor(p / cs + 0.5);
    vec3 lp = p - cellId * cs; // local cell space
    float d = sdBoxFrame(lp, vec3(cs * 0.48), ew);
    return vec4(d, cellId);
}

void main() {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= aspect;

    float audio = 1.0 + (audioLevel * 0.5 + audioBass * 0.5) * audioReact;
    float cs = cageSize;
    float ew = edgeWidth * (0.8 + audioBass * audioReact * 0.5);

    // Camera flies diagonally through lattice — z advances with TIME
    float t = TIME * speed;
    float camZ = t * cs * 0.38;
    float camYaw = sin(t * 0.15) * 0.25;    // gentle yaw oscillation
    float camPitch = sin(t * 0.11) * 0.12;   // gentle pitch

    vec3 ro = vec3(
        sin(t * 0.09) * cs * 0.35,
        sin(t * 0.07) * cs * 0.25,
        camZ
    );

    // Build camera matrix
    vec3 fwd = normalize(vec3(sin(camYaw), sin(camPitch), cos(camYaw)));
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);

    float fov = 1.1;
    vec3 rd = normalize(fwd + uv.x * right * fov * 0.5 + uv.y * up * fov * 0.5);

    // Raymarch — 64 steps
    float tMarch = 0.001;
    float hitDist = -1.0;
    vec3 hitCellId = vec3(0.0);
    vec3 hitPos = vec3(0.0);

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * tMarch;
        vec4 res = mapScene(p, cs, ew);
        float d = res.x;
        if (d < 0.002) {
            hitDist = tMarch;
            hitPos = p;
            hitCellId = res.yzw;
            break;
        }
        // Clamp step to avoid over-stepping near thin edges
        tMarch += max(d * 0.85, 0.001);
        if (tMarch > 60.0) break;
    }

    vec3 col = vec3(0.0); // void black background

    if (hitDist > 0.0) {
        // Get base color for this cell
        vec3 baseCol = cellColor(hitCellId, TIME * speed) * hdrPeak * audio;

        // fwidth() AA on cage edge
        vec4 res = mapScene(hitPos, cs, ew);
        float edgeDist = res.x;
        float fw = fwidth(edgeDist);
        float edgeMask = 1.0 - smoothstep(0.0, fw * 2.0, edgeDist);

        // Edge brightness: bright core fading out
        float edgeFactor = exp(-max(edgeDist, 0.0) / (ew * 0.5 + 0.001));
        col = baseCol * edgeFactor * edgeMask;

        // Distance fog (slight falloff with depth)
        float fog = exp(-hitDist * 0.04);
        col *= fog;
    }

    gl_FragColor = vec4(col, 1.0);
}
