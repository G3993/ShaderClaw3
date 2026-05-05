/*{
  "DESCRIPTION": "Crystal Lattice — infinite 3D grid of neon diamond crystals, traversed by a slow fly-through camera. Cinematic HDR lighting.",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "crystalSize",  "LABEL": "Crystal Size", "TYPE": "float", "DEFAULT": 0.36, "MIN": 0.1,  "MAX": 0.48 },
    { "NAME": "latticeScale", "LABEL": "Cell Scale",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.4,  "MAX": 2.5  },
    { "NAME": "hdrPeak",      "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0  },
    { "NAME": "audioPulse",   "LABEL": "Audio Pulse",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0  },
    { "NAME": "driftSpeed",   "LABEL": "Fly Speed",    "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 1.5  }
  ]
}*/

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5); }
float hash3(vec3 p)  { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5); }

// Octahedron SDF: (|x|+|y|+|z|-r) / sqrt(3)
float sdOct(vec3 p, float r) {
    return (abs(p.x) + abs(p.y) + abs(p.z) - r) * 0.57735;
}

// Thin connector rods between lattice nodes
float sdRods(vec3 p, float cr) {
    return min(min(length(p.yz), length(p.xz)), length(p.xy)) - cr;
}

vec2 scene(vec3 wp) {
    // Drift camera forward and weave
    vec3 p = wp;
    p.z   += TIME * driftSpeed * 0.5;
    p.x   += sin(TIME * driftSpeed * 0.28) * 0.18;
    p.y   += cos(TIME * driftSpeed * 0.19) * 0.1;

    float scale = latticeScale;
    vec3 cellP  = p / scale;
    vec3 cellId = floor(cellP + 0.5);
    vec3 q      = (cellP - cellId) * scale;

    float cs  = crystalSize * (1.0 + audioBass * audioPulse * 0.09);
    float cry = sdOct(q, cs);
    float rod = sdRods(q, cs * 0.07);
    float d   = min(cry, rod);

    float id  = hash3(cellId);
    return vec2(d, id);
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        scene(p+e.xyy).x - scene(p-e.xyy).x,
        scene(p+e.yxy).x - scene(p-e.yxy).x,
        scene(p+e.yyx).x - scene(p-e.yyx).x
    ));
}

// 5-hue fully saturated palette
vec3 crystalColor(float id) {
    int ci = int(id * 4.999);
    if (ci == 0) return vec3(0.45, 0.0,  1.0);  // violet
    if (ci == 1) return vec3(0.0,  1.0,  0.85); // cyan
    if (ci == 2) return vec3(1.0,  0.05, 0.6);  // magenta
    if (ci == 3) return vec3(1.0,  0.75, 0.0);  // gold
               return vec3(0.15, 1.0,  0.2);   // green
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Slowly rotating orbit camera
    float camA  = TIME * 0.09;
    float camEl = sin(TIME * 0.13) * 0.35;
    vec3 ro = vec3(sin(camA)*cos(camEl), sin(camEl), cos(camA)*cos(camEl)) * 2.2;
    vec3 ta = ro + vec3(sin(camA+0.25), 0.0, cos(camA+0.25)) * 2.5;
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x*ri + uv.y*up + 1.5*fw);

    // Dark void background
    vec3 bg = vec3(0.0, 0.0, 0.007);

    float t = 0.02; float hitId = -1.0;
    for (int i = 0; i < 64; i++) {
        vec2 res = scene(ro + rd*t);
        if (res.x < 0.002) { hitId = res.y; break; }
        if (t > 12.0) break;
        t += res.x * 0.75;
    }

    vec3 col = bg;
    if (hitId >= 0.0) {
        vec3 p  = ro + rd*t;
        vec3 n  = getNormal(p);
        vec3 L1 = normalize(vec3(1.0, 1.5, 0.3));
        vec3 L2 = normalize(vec3(-0.5, 0.4, -0.8));

        vec3 basecol = crystalColor(hitId);

        float diff = max(dot(n, L1), 0.0)*0.55 + max(dot(n, L2), 0.0)*0.25 + 0.2;
        float spec = pow(max(dot(reflect(-L1, n), -rd), 0.0), 48.0)
                   + pow(max(dot(reflect(-L2, n), -rd), 0.0), 32.0) * 0.5;
        float fres = pow(1.0 - max(dot(n, -rd), 0.0), 4.0);

        col  = basecol * diff * hdrPeak;
        col += vec3(1.0) * spec * hdrPeak;       // white HDR spec
        col += basecol   * fres * hdrPeak * 0.9; // colored HDR fresnel rim

        // Depth fog towards void
        col = mix(col, bg, clamp(t / 12.0, 0.0, 1.0) * 0.4);
    }

    gl_FragColor = vec4(col, 1.0);
}
