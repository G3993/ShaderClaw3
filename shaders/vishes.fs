/*{
    "DESCRIPTION": "Crystal Lattice — raymarched 3D geometric crystal structure with jewel palette. Close-up mineral composition. Standalone HDR generator.",
    "CREDIT": "auto-improve",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D", "Abstract"],
    "INPUTS": [
        {"NAME":"rotSpeed","TYPE":"float","DEFAULT":0.2,"MIN":0.0,"MAX":1.0,"LABEL":"Rotation Speed"},
        {"NAME":"crystalScale","TYPE":"float","DEFAULT":1.0,"MIN":0.2,"MAX":2.0,"LABEL":"Crystal Scale"},
        {"NAME":"specPeak","TYPE":"float","DEFAULT":3.5,"MIN":1.0,"MAX":6.0,"LABEL":"HDR Peak"},
        {"NAME":"hueShift","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0,"LABEL":"Hue Shift"},
        {"NAME":"audioMod","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0,"LABEL":"Audio Mod"}
    ]
}*/

float sdOctahedron(vec3 p, float s) {
    p = abs(p);
    return (p.x + p.y + p.z - s) * 0.57735027;
}

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b - a; vec3 ap = p - a;
    float t = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
    return length(ap - ab * t) - r;
}

float sdBox(vec3 p, vec3 b) {
    vec3 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
}

vec2 map(vec3 p, float scale, float t) {
    float bestD = 1e9;
    float bestID = 0.0;

    // Central crystal (large octahedron)
    float central = sdOctahedron(p, 0.6 * scale);
    if (central < bestD) { bestD = central; bestID = 1.0; }

    // Satellite crystals arranged in two rings
    for (float i = 0.0; i < 6.0; i++) {
        float ang = i / 6.0 * 6.28318 + t * rotSpeed;
        vec3 pos = vec3(cos(ang) * 1.2 * scale, sin(i * 1.1) * 0.3 * scale, sin(ang) * 1.2 * scale);
        float satScale = (0.2 + sin(i * 3.7) * 0.05) * scale;
        float sat = sdOctahedron(p - pos, satScale);
        if (sat < bestD) { bestD = sat; bestID = 2.0 + i; }

        // Bond capsule from center to satellite
        float bond = sdCapsule(p, vec3(0.0), pos, 0.025 * scale);
        if (bond < bestD) { bestD = bond; bestID = -1.0; }
    }

    // Top and bottom apex crystals
    float top = sdOctahedron(p - vec3(0.0, 1.0 * scale, 0.0), 0.15 * scale);
    float bot = sdOctahedron(p - vec3(0.0, -1.0 * scale, 0.0), 0.15 * scale);
    if (top < bestD) { bestD = top; bestID = 8.0; }
    if (bot < bestD) { bestD = bot; bestID = 9.0; }

    return vec2(bestD, bestID);
}

vec3 calcNormal(vec3 p, float scale, float t) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p + e.xyy, scale, t).x - map(p - e.xyy, scale, t).x,
        map(p + e.yxy, scale, t).x - map(p - e.yxy, scale, t).x,
        map(p + e.yyx, scale, t).x - map(p - e.yyx, scale, t).x
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float audio = 1.0 + (audioLevel + audioBass * 0.6) * audioMod;
    float scale = crystalScale * audio;

    float camAng = t * rotSpeed * 0.5;
    vec3 ro = vec3(sin(camAng) * 3.0, 0.8 + sin(t * 0.23) * 0.3, cos(camAng) * 3.0);
    vec3 ta = vec3(0.0, 0.2 * scale, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + uv.x * right + uv.y * up);

    vec3 col = vec3(0.0); // void black
    float rayT = 0.01;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * rayT;
        vec2 res = map(p, scale, t);
        float d = res.x;

        if (d < 0.002) {
            vec3 nor = calcNormal(p, scale, t);
            vec3 lightDir = normalize(vec3(1.0, 2.0, 1.0));
            float diff = max(0.0, dot(nor, lightDir));

            float id = res.y;
            vec3 sapphire = vec3(0.0, 0.3, 2.5 + hueShift);
            vec3 cyan = vec3(0.0, 2.0, 2.5);
            vec3 violet = vec3(0.8 + hueShift, 0.0, 2.5);
            vec3 white = vec3(specPeak, specPeak, specPeak * 1.1);

            vec3 baseCol;
            if (id < 0.0) baseCol = cyan * 0.5; // bonds
            else if (id < 1.5) baseCol = sapphire; // central
            else if (id < 8.0) baseCol = mix(violet, cyan, fract(id * 0.3)); // satellites
            else baseCol = white * 0.8; // apex

            // Specular
            vec3 refl = reflect(-lightDir, nor);
            float spec = pow(max(0.0, dot(refl, -rd)), 32.0);
            col = baseCol * (0.1 + diff * 0.7) + white * spec * specPeak;

            // Edge emission (sharp edge glow from near-grazing angle)
            float edge = 1.0 - abs(dot(nor, -rd));
            edge = pow(edge, 3.0);
            col += mix(cyan, violet, edge) * edge * specPeak * 0.8;

            break;
        }
        rayT += max(d * 0.7, 0.002);
        if (rayT > 15.0) break;
    }

    gl_FragColor = vec4(col, 1.0);
}
