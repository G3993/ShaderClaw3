/*{
    "DESCRIPTION": "Neon Lattice — raymarched 3D infinite wireframe grid with volumetric edge glow. Standalone HDR generator.",
    "CREDIT": "auto-improve",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D", "Abstract"],
    "INPUTS": [
        {"NAME":"camSpeed","TYPE":"float","DEFAULT":0.25,"MIN":0.0,"MAX":2.0,"LABEL":"Camera Speed"},
        {"NAME":"cellSize","TYPE":"float","DEFAULT":1.0,"MIN":0.2,"MAX":3.0,"LABEL":"Cell Size"},
        {"NAME":"edgeWidth","TYPE":"float","DEFAULT":0.06,"MIN":0.01,"MAX":0.3,"LABEL":"Edge Width"},
        {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":5.0,"LABEL":"HDR Peak"},
        {"NAME":"hueShift","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0,"LABEL":"Hue Shift"},
        {"NAME":"audioMod","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0,"LABEL":"Audio Mod"}
    ]
}*/

float hash1(float n) { return fract(sin(n * 127.1 + 311.7) * 43758.5453); }
float hash3(vec3 v) { return hash1(v.x * 1.0 + v.y * 57.0 + v.z * 113.0); }

vec3 hsv2rgb(float h, float s, float v) {
    vec3 k = mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0);
    return v * mix(vec3(1.0), clamp(min(k, 4.0 - k), 0.0, 1.0), s);
}

float wireframeDist(vec3 p, float S) {
    vec3 q = fract(p / S) * S;
    vec3 d = min(q, S - q);
    return min(min(max(d.x, d.y), max(d.y, d.z)), max(d.x, d.z));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float audio = 1.0 + (audioLevel + audioBass * 0.8) * audioMod;

    float ang = t * camSpeed * 0.4;
    vec3 ro = vec3(sin(ang) * 3.5, 1.2 + sin(t * 0.19) * 0.6, cos(ang) * 3.5);
    vec3 ta = vec3(0.0, 0.5, 0.0);
    vec3 fwd = normalize(ta - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + uv.x * right + uv.y * up);

    float S = max(0.2, cellSize);
    float ew = edgeWidth * audio;
    float maxDist = 20.0;
    float stepSize = maxDist / 64.0;

    vec3 col = vec3(0.0);

    for (int i = 0; i < 64; i++) {
        float rayT = float(i) * stepSize;
        vec3 p = ro + rd * rayT;
        float wfd = wireframeDist(p, S);
        if (wfd < ew) {
            float t01 = 1.0 - wfd / ew;
            float glow = t01 * t01 * glowPeak * audio;
            vec3 cell = floor(p / S);
            float h = hash3(cell);
            float hue = hueShift + h * 0.5 + float(i) * 0.002;
            col += hsv2rgb(hue, 1.0, 1.0) * glow * stepSize;
        }
    }

    col = min(col, vec3(6.0));
    gl_FragColor = vec4(col, 1.0);
}
