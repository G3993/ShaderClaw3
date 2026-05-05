/*{
  "DESCRIPTION": "Plasma Sphere Grid — 3D raymarched neon lattice sphere: meridians and parallels glow in magenta/cyan/gold. Audio pulses the tube radius. NEW ANGLE: 3D SDF capsule web vs prior 2D bouncing-particle bounce.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "INPUTS": [
    {"NAME":"camDist",   "LABEL":"Camera Dist", "TYPE":"float","MIN":1.5,"MAX":6.0,"DEFAULT":3.2},
    {"NAME":"camSpeed",  "LABEL":"Orbit Speed", "TYPE":"float","MIN":0.0,"MAX":0.8,"DEFAULT":0.14},
    {"NAME":"tubeRad",   "LABEL":"Tube Radius", "TYPE":"float","MIN":0.005,"MAX":0.06,"DEFAULT":0.018},
    {"NAME":"sphereR",   "LABEL":"Sphere Radius","TYPE":"float","MIN":0.5,"MAX":1.5,"DEFAULT":1.0},
    {"NAME":"hdrPeak",   "LABEL":"HDR Peak",    "TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.8},
    {"NAME":"pulseAmt",  "LABEL":"Pulse",       "TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.55},
    {"NAME":"audioReact","LABEL":"Audio",       "TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

// 4-colour neon palette (no white mixing)
vec3 neonPal(float t) {
    t = fract(t);
    if (t < 0.25) return mix(vec3(2.5, 0.0, 2.5), vec3(0.0, 2.5, 2.5), t * 4.0);
    if (t < 0.50) return mix(vec3(0.0, 2.5, 2.5), vec3(2.5, 2.0, 0.0), (t-0.25)*4.0);
    if (t < 0.75) return mix(vec3(2.5, 2.0, 0.0), vec3(0.8, 0.0, 2.5), (t-0.50)*4.0);
    return         mix(vec3(0.8, 0.0, 2.5), vec3(2.5, 0.0, 2.5), (t-0.75)*4.0);
}

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

vec3 sphPt(float lat, float lon, float R) {
    return vec3(R * cos(lat) * cos(lon), R * sin(lat), R * cos(lat) * sin(lon));
}

float sceneSDF(vec3 p, out vec3 outCol) {
    float tAudio = 1.0 + audioBass * audioReact * pulseAmt * 0.6
                       + sin(TIME * 6.28318 * 1.2) * pulseAmt * 0.2;
    float r = tubeRad * tAudio;
    float R = sphereR;

    float minD = 1e9;
    outCol = vec3(0.0);

    int MERS = 8;
    int PARS = 5;

    for (int m = 0; m < 8; m++) {
        if (m >= MERS) break;
        float lon = float(m) / float(MERS) * 6.28318;
        int SEG = 16;
        for (int s = 0; s < 16; s++) {
            if (s >= SEG) break;
            float latA = -1.5707963 + float(s)   / float(SEG) * 3.14159265;
            float latB = -1.5707963 + float(s+1) / float(SEG) * 3.14159265;
            vec3 pa = sphPt(latA, lon, R);
            vec3 pb = sphPt(latB, lon, R);
            float d = sdCapsule(p, pa, pb, r);
            if (d < minD) {
                minD = d;
                float ci = float(m) / float(MERS);
                outCol = neonPal(ci + TIME * 0.04);
            }
        }
    }

    for (int q = 0; q < 5; q++) {
        if (q >= PARS) break;
        float lat = -1.3 + float(q) / float(PARS-1) * 2.6;
        float ringR = R * cos(lat);
        float ringY = R * sin(lat);
        int SEG2 = 24;
        for (int s = 0; s < 24; s++) {
            if (s >= SEG2) break;
            float aA = float(s)   / float(SEG2) * 6.28318;
            float aB = float(s+1) / float(SEG2) * 6.28318;
            vec3 pa = vec3(ringR * cos(aA), ringY, ringR * sin(aA));
            vec3 pb = vec3(ringR * cos(aB), ringY, ringR * sin(aB));
            float d = sdCapsule(p, pa, pb, r * 0.8);
            if (d < minD) {
                minD = d;
                float ci = float(q) / float(PARS) + 0.15;
                outCol = neonPal(ci + TIME * 0.04);
            }
        }
    }

    return minD;
}

vec3 calcNormal(vec3 p) {
    vec3 c; vec2 h = vec2(0.0005, 0.0);
    return normalize(vec3(
        sceneSDF(p+h.xyy,c) - sceneSDF(p-h.xyy,c),
        sceneSDF(p+h.yxy,c) - sceneSDF(p-h.yxy,c),
        sceneSDF(p+h.yyx,c) - sceneSDF(p-h.yyx,c)
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    float ang   = TIME * camSpeed;
    float pitch = 0.30 + 0.18 * sin(TIME * camSpeed * 0.37);
    vec3 ro = vec3(camDist * cos(ang) * cos(pitch),
                   camDist * sin(pitch),
                   camDist * sin(ang) * cos(pitch));
    vec3 ww = normalize(-ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.5 * ww);

    float t = 0.1;
    vec3 hitCol = vec3(0.0);
    bool hit = false;
    for (int i = 0; i < 64; i++) {
        vec3 p  = ro + rd * t;
        float d = sceneSDF(p, hitCol);
        if (d < 0.0008) { hit = true; break; }
        if (t > 15.0)   break;
        t += max(d, 0.003);
    }

    vec3 col = vec3(0.0, 0.0, 0.01);

    if (hit) {
        vec3 p   = ro + rd * t;
        vec3 n   = calcNormal(p);
        vec3 lDir = normalize(vec3(1.8, 2.0, -1.2));
        float diff = max(dot(n, lDir), 0.0);
        float spec = pow(max(dot(reflect(-lDir, n), -rd), 0.0), 40.0);
        float rim  = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);

        float audio = 1.0 + audioLevel * audioReact * 0.5
                          + audioBass  * audioReact * 0.3;

        float edge = fwidth(t);
        float inkMask = 1.0 - smoothstep(0.0, edge * 6.0, edge);

        col = hitCol * (0.35 + 0.65 * diff) * hdrPeak * audio;
        col += hitCol * spec * 1.8 * audio;
        col += hitCol * rim  * 0.9 * hdrPeak;
        col *= (1.0 - inkMask * 0.8);
    }

    gl_FragColor = vec4(col, 1.0);
}
