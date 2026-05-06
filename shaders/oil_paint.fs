/*{
    "DESCRIPTION": "Volumetric Paint Cloud 3D — 8 metaball blobs, abstract expressionist, Zorn palette",
    "CREDIT": "ShaderClaw auto-improve 2026-05-06",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "speed",      "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 3.0,  "LABEL": "Speed" },
        { "NAME": "blobScale",  "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 3.0,  "LABEL": "Blob Scale" },
        { "NAME": "hdrPeak",    "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 5.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioReact", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio React" }
    ]
}*/

// Polynomial smooth-min (k = smoothness radius)
float smin(float a, float b, float k) {
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * k * 0.25;
}

// Blob positions via golden angle spacing + TIME drift
vec3 blobPos(int i, float tm) {
    float fi = float(i);
    float phase = fi * 2.39996323; // golden angle in radians
    return vec3(
        sin(phase) * 1.4 + sin(tm * 0.28 + fi * 0.9) * 0.3,
        cos(phase * 0.71) * 0.8 + cos(tm * 0.19 + fi * 1.27) * 0.2,
        sin(phase * 1.33) * 1.1 + sin(tm * 0.23 + fi * 0.71) * 0.2
    );
}

// Blob radii (varied for composition)
float blobRadius(int i) {
    float fi = float(i);
    return 0.38 + 0.12 * fract(sin(fi * 37.91) * 4987.3);
}

// Blob palette — vermillion, cobalt blue, yellow ochre cycling
vec3 blobColor(int i) {
    int m = int(mod(float(i), 3.0));
    if (m == 0) return vec3(2.0, 0.08, 0.0);    // vermillion
    if (m == 1) return vec3(0.0, 0.15, 2.5);    // cobalt blue
    return vec3(2.5, 1.5, 0.0);                  // yellow ochre
}

// Scene SDF: smooth-blended metaballs
float mapScene(vec3 p, float bs, float aScale) {
    float d = 1e9;
    for (int i = 0; i < 8; i++) {
        vec3 bp = blobPos(i, TIME * bs);
        float r = blobRadius(i) * aScale;
        float db = length(p - bp) - r;
        d = smin(d, db, 0.55);
    }
    return d;
}

// Weighted color blend: each blob contributes inversely with distance
vec3 blobColorBlend(vec3 p, float bs, float aScale) {
    vec3 weightedCol = vec3(0.0);
    float totalW = 0.0;
    for (int i = 0; i < 8; i++) {
        vec3 bp = blobPos(i, TIME * bs);
        float r = blobRadius(i) * aScale;
        float db = max(length(p - bp) - r, 0.001);
        float w = 1.0 / (db * db + 0.01);
        weightedCol += blobColor(i) * w;
        totalW += w;
    }
    return weightedCol / max(totalW, 0.001);
}

// Normal via central differences
vec3 calcNormal(vec3 p, float bs, float aScale) {
    const float eps = 0.003;
    return normalize(vec3(
        mapScene(p + vec3(eps, 0.0, 0.0), bs, aScale) - mapScene(p - vec3(eps, 0.0, 0.0), bs, aScale),
        mapScene(p + vec3(0.0, eps, 0.0), bs, aScale) - mapScene(p - vec3(0.0, eps, 0.0), bs, aScale),
        mapScene(p + vec3(0.0, 0.0, eps), bs, aScale) - mapScene(p - vec3(0.0, 0.0, eps), bs, aScale)
    ));
}

void main() {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= aspect;

    float audio = 1.0 + (audioLevel * 0.5 + audioBass * 0.5) * audioReact;
    float aScale = blobScale * (0.85 + audioBass * audioReact * 0.35); // bass pulses blobs

    // Camera orbits with gentle bob
    float camAngle = TIME * speed * 0.18;
    float camR = 5.5;
    vec3 ro = vec3(
        sin(camAngle) * camR,
        sin(TIME * speed * 0.12) * 0.7,
        cos(camAngle) * camR
    );
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);

    float fov = 1.2;
    vec3 rd = normalize(fwd + uv.x * right * fov * 0.5 + uv.y * up * fov * 0.5);

    // Raymarch — 64 steps
    float t = 0.01;
    float hitDist = -1.0;
    vec3 hitPos = vec3(0.0);
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * t;
        float d = mapScene(p, speed, aScale);
        if (d < 0.003) {
            hitDist = t;
            hitPos = p;
            break;
        }
        t += d * 0.85;
        if (t > 25.0) break;
    }

    vec3 col = vec3(0.0); // void black background

    if (hitDist > 0.0) {
        vec3 n = calcNormal(hitPos, speed, aScale);

        // fwidth() ink outline: dark ring around blob boundary
        float sdfVal = mapScene(hitPos, speed, aScale);
        float fw = fwidth(sdfVal);
        float inkRing = 1.0 - smoothstep(0.0, fw * 3.5, abs(sdfVal) + 0.008);

        // Per-blob weighted color blend
        vec3 baseCol = blobColorBlend(hitPos, speed, aScale);

        // Painterly lighting: warm key + cool fill + gloss specular
        vec3 keyDir = normalize(vec3(1.5, 2.0, 1.0));
        vec3 fillDir = normalize(vec3(-1.0, 0.5, -1.0));
        vec3 viewDir = normalize(ro - hitPos);

        float keyDiff  = max(dot(n, keyDir), 0.0);
        float fillDiff = max(dot(n, fillDir), 0.0) * 0.4;
        float spec = pow(max(dot(reflect(-keyDir, n), viewDir), 0.0), 48.0);

        vec3 keyLight  = vec3(1.2, 0.9, 0.7) * keyDiff;  // warm
        vec3 fillLight = vec3(0.4, 0.6, 1.0) * fillDiff; // cool
        vec3 specColor = vec3(1.8, 1.6, 1.5) * spec;     // gloss highlight

        col = baseCol * (keyLight + fillLight) * hdrPeak * audio;
        col += specColor;

        // Ink outline darkens the edge
        col *= (1.0 - inkRing * 0.85);
    }

    gl_FragColor = vec4(col, 1.0);
}
