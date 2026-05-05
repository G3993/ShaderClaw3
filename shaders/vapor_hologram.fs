/*{
  "DESCRIPTION": "Digital Shrine — 3D raymarched Japanese torii gate standing in volumetric fog. Cool teal/cyan palette vs prior warm vaporwave pink. NEW ANGLE: 3D SDF architecture vs 2D layered vaporwave scene.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "INPUTS": [
    {"NAME":"fogDensity", "LABEL":"Fog Density",  "TYPE":"float","MIN":0.0,"MAX":2.0, "DEFAULT":0.55},
    {"NAME":"glowAmt",    "LABEL":"Gate Glow",    "TYPE":"float","MIN":0.0,"MAX":3.0, "DEFAULT":2.2},
    {"NAME":"camOrbit",   "LABEL":"Orbit Speed",  "TYPE":"float","MIN":0.0,"MAX":0.5, "DEFAULT":0.06},
    {"NAME":"camHeight",  "LABEL":"Camera Height","TYPE":"float","MIN":0.5,"MAX":2.0, "DEFAULT":1.0},
    {"NAME":"hdrPeak",    "LABEL":"HDR Peak",     "TYPE":"float","MIN":1.0,"MAX":4.0, "DEFAULT":2.5},
    {"NAME":"audioReact", "LABEL":"Audio",        "TYPE":"float","MIN":0.0,"MAX":2.0, "DEFAULT":1.0}
  ]
}*/

vec3 shinePal(float t) {
    t = clamp(t, 0.0, 1.0);
    vec3 teal  = vec3(0.0, 0.9,  0.7);
    vec3 cyan  = vec3(0.0, 2.5,  2.5);
    vec3 white = vec3(2.8, 2.8,  2.8);
    if (t < 0.5) return mix(teal, cyan, t * 2.0);
    return         mix(cyan, white, (t - 0.5) * 2.0);
}

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}
float sdCylinder(vec3 p, float r, float h) {
    vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(r, h);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

float sdTorii(vec3 p) {
    float pillarL = sdCylinder(p - vec3(-0.55, 0.85, 0.0), 0.06, 0.85);
    float pillarR = sdCylinder(p - vec3( 0.55, 0.85, 0.0), 0.06, 0.85);
    float nuki    = sdBox(p - vec3(0.0, 1.25, 0.0), vec3(0.62, 0.055, 0.055));
    float kasagi  = sdBox(p - vec3(0.0, 1.60, 0.0), vec3(0.75, 0.065, 0.065));
    float capL    = sdBox(p - vec3(-0.55, 1.72, 0.0), vec3(0.08, 0.07, 0.07));
    float capR    = sdBox(p - vec3( 0.55, 1.72, 0.0), vec3(0.08, 0.07, 0.07));
    return min(min(min(min(min(pillarL, pillarR), nuki), kasagi), capL), capR);
}

float sceneSDF(vec3 p, out vec3 col) {
    float audio = 1.0 + audioBass * audioReact * 0.15;
    float pulse = 1.0 + sin(TIME * 3.14159 * 1.8) * 0.06 * audio;

    float gate  = sdTorii(p);
    float ground = p.y + 0.02;
    float step1 = sdBox(p - vec3(0.0, -0.02, 0.5),  vec3(0.45, 0.03, 0.18));
    float step2 = sdBox(p - vec3(0.0, -0.08, 0.7),  vec3(0.38, 0.06, 0.16));
    float step3 = sdBox(p - vec3(0.0, -0.18, 0.9),  vec3(0.30, 0.10, 0.14));
    float scene = min(gate, min(ground, min(step1, min(step2, step3))));

    if (gate < ground && gate < step1 && gate < step2 && gate < step3) {
        col = vec3(0.0, 2.5, 2.5) * glowAmt * pulse;
    } else if (ground <= step1 && ground <= step2 && ground <= step3 && ground < gate) {
        col = vec3(0.0, 0.12, 0.12);
    } else {
        col = vec3(0.0, 0.18, 0.18);
    }
    return scene;
}

vec3 calcNormal(vec3 p) {
    vec3 c; vec2 h = vec2(0.0006, 0.0);
    return normalize(vec3(
        sceneSDF(p+h.xyy,c) - sceneSDF(p-h.xyy,c),
        sceneSDF(p+h.yxy,c) - sceneSDF(p-h.yxy,c),
        sceneSDF(p+h.yyx,c) - sceneSDF(p-h.yyx,c)
    ));
}

float fogDens(vec3 p, float t) {
    float base = exp(-p.y * 2.5);
    float drift = 0.5 + 0.5 * sin(p.x * 1.3 + t * 0.4) * cos(p.z * 1.1 + t * 0.3);
    return base * drift * fogDensity * 0.025;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    float ang = TIME * camOrbit;
    float camD = 3.5;
    vec3 ro = vec3(camD * sin(ang), camHeight, camD * cos(ang));
    vec3 ta = vec3(0.0, 0.9, 0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.5 * ww);

    float audio = 1.0 + audioLevel * audioReact * 0.4
                      + audioBass  * audioReact * 0.3;

    float t = 0.05;
    vec3 hitCol = vec3(0.0);
    bool hit = false;
    for (int i = 0; i < 80; i++) {
        vec3 p = ro + rd * t;
        float d = sceneSDF(p, hitCol);
        if (d < 0.0008) { hit = true; break; }
        if (t > 12.0)   break;
        t += max(d * 0.9, 0.004);
    }

    vec3 col = vec3(0.01, 0.02, 0.05);
    vec2 moonDir = normalize(vec2(-0.7, 0.55));
    float moonDist = length(uv - moonDir * 0.35);
    col += vec3(0.3, 0.6, 1.0) * exp(-moonDist * 8.0) * 0.4;

    if (hit) {
        vec3 p  = ro + rd * t;
        vec3 n  = calcNormal(p);
        vec3 lDir = normalize(vec3(-0.5, 1.8, -1.0));
        float diff = max(dot(n, lDir), 0.0);
        float spec = pow(max(dot(reflect(-lDir, n), -rd), 0.0), 50.0);
        float rim  = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);
        float edge = fwidth(t);

        col = hitCol * (0.3 + 0.7 * diff) * hdrPeak * audio;
        col += vec3(0.3, 1.0, 1.0) * spec * 1.5 * audio;
        col += hitCol * rim * 0.7;
        col *= 1.0 - smoothstep(0.0, edge * 5.0, edge) * 0.65;
    }

    if (!hit || t > 2.0) {
        float fogAcc = 0.0;
        vec3  fogCol = vec3(0.0);
        float tFog   = 0.1;
        for (int i = 0; i < 12; i++) {
            vec3 p = ro + rd * tFog;
            float d = fogDens(p, TIME);
            vec3 fc  = shinePal(clamp(p.y * 0.8, 0.0, 1.0)) * 0.6;
            fogCol  += fc * d * (1.0 - fogAcc);
            fogAcc  += d;
            if (fogAcc > 0.9) break;
            tFog    += 0.25;
        }
        col = mix(col, fogCol * hdrPeak, min(fogAcc, 0.7));
    }

    gl_FragColor = vec4(col, 1.0);
}
