/*{
    "DESCRIPTION": "Volcanic Caldera — raymarched 3D erupting lava geysers inside a volcanic crater. Standalone HDR generator.",
    "CREDIT": "auto-improve",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        {"NAME":"geyserCount","TYPE":"float","DEFAULT":6.0,"MIN":2.0,"MAX":10.0,"LABEL":"Geyser Count"},
        {"NAME":"eruptSpeed","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":3.0,"LABEL":"Erupt Speed"},
        {"NAME":"geyserRadius","TYPE":"float","DEFAULT":0.12,"MIN":0.02,"MAX":0.4,"LABEL":"Geyser Radius"},
        {"NAME":"hdrPeak","TYPE":"float","DEFAULT":4.0,"MIN":1.0,"MAX":6.0,"LABEL":"HDR Peak"},
        {"NAME":"audioMod","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":1.0,"LABEL":"Audio Mod"},
        {"NAME":"camHeight","TYPE":"float","DEFAULT":1.5,"MIN":0.5,"MAX":4.0,"LABEL":"Cam Height"}
    ]
}*/

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5453); }

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b - a; vec3 ap = p - a;
    float t = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
    return length(ap - ab * t) - r;
}

float sdPlane(vec3 p) { return p.y; }

vec2 map(vec3 p, float t, float audio) {
    float floorD = sdPlane(p);
    float bestD = floorD;
    float bestID = 0.0;

    float N = max(2.0, geyserCount);
    float r = geyserRadius;

    for (float i = 0.0; i < 10.0; i++) {
        if (i >= N) break;
        float angle = i / N * 6.28318;
        float ring = 1.4;
        vec3 base = vec3(cos(angle) * ring, 0.0, sin(angle) * ring);
        float phase = hash1(i * 3.7) * 6.28;
        float height = (1.5 + sin(t * eruptSpeed + phase) * 0.8) * audio;
        vec3 tip = base + vec3(0.0, height, 0.0);
        float d = sdCapsule(p, base, tip, r);
        if (d < bestD) { bestD = d; bestID = i + 1.0; }
    }

    // Central big geyser
    float cHeight = (2.5 + sin(t * eruptSpeed * 0.7) * 1.0) * audio;
    float cd = sdCapsule(p, vec3(0.0, 0.0, 0.0), vec3(0.0, cHeight, 0.0), r * 1.6);
    if (cd < bestD) { bestD = cd; bestID = -1.0; }

    return vec2(bestD, bestID);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float audio = 1.0 + (audioLevel + audioBass * 0.8) * audioMod;

    float camAng = t * 0.08;
    vec3 ro = vec3(sin(camAng) * 3.5, camHeight, cos(camAng) * 3.5);
    vec3 ta = vec3(0.0, 1.0, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + uv.x * right + uv.y * up);

    vec3 col = vec3(0.01, 0.0, 0.0);
    float rayT = 0.01;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * rayT;
        vec2 res = map(p, t, audio);
        float d = res.x;
        if (d < 0.002) {
            float id = res.y;
            float height = p.y;
            float tipFrac = clamp(height / (3.0 * audio), 0.0, 1.0);
            vec3 crimson = vec3(1.6, 0.0, 0.0);
            vec3 orange = vec3(2.5, 0.8, 0.0);
            vec3 gold = vec3(3.0, 2.0, 0.0);
            vec3 whiteHot = vec3(hdrPeak, hdrPeak * 0.8, hdrPeak * 0.3);
            vec3 geyserCol = mix(crimson, mix(orange, mix(gold, whiteHot, tipFrac * tipFrac), tipFrac), tipFrac);
            if (id < 0.5) {
                // floor
                float crack = sin(p.x * 5.0 + t * 0.3) * sin(p.z * 5.0 + t * 0.2);
                float lava = clamp(crack * 2.0, 0.0, 1.0);
                col = mix(vec3(0.02, 0.0, 0.0), vec3(2.0, 0.6, 0.0), lava * lava);
            } else {
                col = geyserCol;
            }
            break;
        }
        rayT += max(d * 0.8, 0.002);
        if (rayT > 20.0) break;
    }

    gl_FragColor = vec4(col, 1.0);
}
