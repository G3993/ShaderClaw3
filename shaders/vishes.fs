/*{
  "DESCRIPTION": "Mycelium Network — 3D raymarched bioluminescent fungal-web of glowing tube-branches in deep void. NEW ANGLE: 3D SDF organic branching network vs prior 2D grid-cell random walkers.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "INPUTS": [
    {"NAME":"branchCount","LABEL":"Strands",      "TYPE":"float","MIN":4.0,"MAX":16.0,"DEFAULT":9.0},
    {"NAME":"tubeRad",    "LABEL":"Tube Radius",  "TYPE":"float","MIN":0.005,"MAX":0.04,"DEFAULT":0.016},
    {"NAME":"glowSpread", "LABEL":"Glow Spread",  "TYPE":"float","MIN":0.0,"MAX":1.0, "DEFAULT":0.55},
    {"NAME":"hdrPeak",    "LABEL":"HDR Peak",     "TYPE":"float","MIN":1.0,"MAX":4.0, "DEFAULT":2.8},
    {"NAME":"swaySpeed",  "LABEL":"Sway Speed",   "TYPE":"float","MIN":0.0,"MAX":1.0, "DEFAULT":0.22},
    {"NAME":"audioReact", "LABEL":"Audio",        "TYPE":"float","MIN":0.0,"MAX":2.0, "DEFAULT":1.0}
  ]
}*/

vec3 mycelPal(float t) {
    t = fract(t);
    vec3 purple = vec3(0.8,  0.0, 2.2);
    vec3 teal   = vec3(0.0,  2.2, 1.8);
    vec3 white  = vec3(2.8,  2.8, 2.8);
    if (t < 0.45) return mix(purple, teal,  t / 0.45);
    if (t < 0.80) return mix(teal,   white, (t-0.45)/0.35);
    return mix(white, purple, (t-0.80)/0.20);
}

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7))) * 43758.5453); }
vec3  hash31(float n) {
    return fract(sin(vec3(n*127.1, n*311.7, n*74.7)) * 43758.5453);
}

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p-a, ba = b-a;
    float h = clamp(dot(pa,ba)/max(dot(ba,ba),1e-6), 0.0, 1.0);
    return length(pa - ba*h) - r;
}
float sdSphere(vec3 p, float r) { return length(p) - r; }

vec3 branchTip(float idx, float level) {
    vec3 seed = hash31(idx + level * 17.3);
    float t = TIME * swaySpeed;
    vec3 dir = normalize(seed - 0.5 + vec3(0.0, 0.3, 0.0));
    float sway = 0.06 * sin(t * (0.7 + seed.x * 0.8) + idx * 2.3);
    dir.x += sway;
    dir.z += sway * 0.7;
    return dir;
}

float sceneSDF(vec3 p, out vec3 outCol) {
    float audio = 1.0 + audioBass * audioReact * 0.25
                      + sin(TIME * 4.0) * 0.05;
    int N = int(clamp(branchCount, 4.0, 16.0));
    float r = tubeRad * audio;
    float minD = 1e9;
    outCol = vec3(0.0);

    float hubD = sdSphere(p, r * 3.5);
    if (hubD < minD) { minD = hubD; outCol = mycelPal(0.5 + TIME * 0.05); }

    for (int i = 0; i < 16; i++) {
        if (i >= N) break;
        float fi = float(i);
        vec3 d1 = branchTip(fi, 0.0);
        float len1 = 0.5 + hash11(fi * 3.7) * 0.4;
        vec3 a1 = vec3(0.0);
        vec3 b1 = d1 * len1;

        float d = sdCapsule(p, a1, b1, r);
        if (d < minD) {
            minD = d;
            float ci = fi / 16.0 + TIME * 0.04;
            outCol = mycelPal(ci);
        }

        for (int j = 0; j < 3; j++) {
            float fj = float(j);
            vec3 d2 = branchTip(fi + fj * 100.0, 1.0);
            float len2 = len1 * (0.4 + hash11(fi * 7.1 + fj * 3.3) * 0.3);
            vec3 a2 = b1;
            vec3 b2 = b1 + d2 * len2;

            float d2v = sdCapsule(p, a2, b2, r * 0.65);
            if (d2v < minD) {
                minD = d2v;
                float ci = (fi + fj) / 20.0 + 0.15 + TIME * 0.04;
                outCol = mycelPal(ci);
            }

            vec3 d3 = branchTip(fi + fj * 200.0, 2.0);
            float len3 = len2 * (0.35 + hash11(fi * 11.1 + fj * 5.7) * 0.25);
            vec3 a3 = b2;
            vec3 b3 = b2 + d3 * len3;
            float d3v = sdCapsule(p, a3, b3, r * 0.40);
            if (d3v < minD) {
                minD = d3v;
                float ci = 0.75 + sin(TIME * 2.5 + fi * 1.3) * 0.04;
                outCol = mycelPal(ci) * glowSpread;
            }

            float nodeD = sdSphere(p - b2, r * 2.2);
            if (nodeD < minD) {
                minD = nodeD;
                outCol = mycelPal(0.8 + fi/16.0 + TIME * 0.06);
            }
        }
    }
    return minD;
}

vec3 calcNormal(vec3 p) {
    vec3 c; vec2 h = vec2(0.0006, 0.0);
    return normalize(vec3(
        sceneSDF(p+h.xyy,c) - sceneSDF(p-h.xyy,c),
        sceneSDF(p+h.yxy,c) - sceneSDF(p-h.yxy,c),
        sceneSDF(p+h.yyx,c) - sceneSDF(p-h.yyx,c)
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    float ang   = TIME * 0.13;
    float pitch = 0.18 + 0.10 * sin(TIME * 0.07);
    float camD  = 2.8 + sin(TIME * 0.05) * 0.3;
    vec3 ro = vec3(camD * cos(ang) * cos(pitch), camD * sin(pitch), camD * sin(ang) * cos(pitch));
    vec3 ww = normalize(-ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.5 * ww);

    float audio = 1.0 + audioLevel * audioReact * 0.45
                      + audioBass  * audioReact * 0.30;

    float t = 0.05;
    vec3 hitCol = vec3(0.0);
    bool hit = false;
    for (int i = 0; i < 64; i++) {
        vec3 pos = ro + rd * t;
        float d  = sceneSDF(pos, hitCol);
        if (d < 0.0008) { hit = true; break; }
        if (t > 10.0)   break;
        t += max(d, 0.003);
    }

    vec3 col = vec3(0.0, 0.0, 0.01);

    if (hit) {
        vec3 p   = ro + rd * t;
        vec3 n   = calcNormal(p);
        vec3 lDir = normalize(vec3(0.5, 1.2, -0.8));
        float diff = max(dot(n, lDir), 0.0);
        float spec = pow(max(dot(reflect(-lDir, n), -rd), 0.0), 30.0);
        float rim  = pow(1.0 - max(dot(n, -rd), 0.0), 4.0);
        float edge = fwidth(t);

        col = hitCol * (0.3 + 0.7 * diff) * hdrPeak * audio;
        col += hitCol * spec * 1.5;
        col += hitCol * rim  * 0.8 * hdrPeak;
        col *= 1.0 - smoothstep(0.0, edge * 4.0, edge) * 0.7;

        float haze = exp(-max(t - 0.5, 0.0) * 0.6) * glowSpread * 0.12;
        col += hitCol * haze;
    }

    float nebula = hash21(floor(uv * 60.0) + TIME * 0.005);
    col += vec3(0.0, 0.02, 0.025) * (nebula * 0.5 + 0.2) * (1.0 - float(hit));

    gl_FragColor = vec4(col, 1.0);
}
