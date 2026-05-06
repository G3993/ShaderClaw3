/*{
  "DESCRIPTION": "Mycenoid — bioluminescent HDR mushroom grove. Raymarched 3D.",
  "CREDIT": "auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Abstract"],
  "INPUTS": [
    { "NAME": "mushroomCount", "LABEL": "Mushrooms",  "TYPE": "float", "DEFAULT": 7.0, "MIN": 3.0, "MAX": 12.0 },
    { "NAME": "glowSpeed",     "LABEL": "Pulse Speed","TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "hdrPeak",       "LABEL": "HDR Peak",   "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioReact",    "LABEL": "Audio",      "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// ──────────────────────────────────────────────────────────────────────
// Hashing helpers
// ──────────────────────────────────────────────────────────────────────
float hash11(float n) {
    return fract(sin(n) * 43758.5453123);
}
float hash12(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

// ──────────────────────────────────────────────────────────────────────
// Smooth minimum (organic blending)
// ──────────────────────────────────────────────────────────────────────
float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

// ──────────────────────────────────────────────────────────────────────
// SDFs
// ──────────────────────────────────────────────────────────────────────
float sdCylinder(vec3 p, float h, float r) {
    vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(r, h);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

float sdSphere(vec3 p, float r) {
    return length(p) - r;
}

// mushroom: cylinder stem + sphere cap sitting on top
// stemH: half-height of stem, stemR: radius, capR: radius of cap sphere
float sdMushroom(vec3 p, float stemH, float stemR, float capR) {
    float stem = sdCylinder(p - vec3(0.0, stemH, 0.0), stemH, stemR);
    // cap sphere slightly overlaps top of stem
    float cap  = sdSphere(p - vec3(0.0, stemH * 2.0 - capR * 0.3, 0.0), capR);
    return smin(stem, cap, 0.08);
}

// ──────────────────────────────────────────────────────────────────────
// Scene: 7 ring mushrooms + 1 central tall one
// Returns vec2(dist, materialID)  materialID: 0=floor,1=stem,2=cap,3=spore
// ──────────────────────────────────────────────────────────────────────
vec2 sceneSDF(vec3 p) {
    float dist = 1e10;
    float matID = 0.0;

    // Floor plane glow (thin emissive slab at y=0)
    float floorDist = p.y + 0.02;
    if (floorDist < dist) { dist = floorDist; matID = 0.0; }

    // Audio-reactive pulse scale
    float pulse = 1.0 + audioBass * audioReact * 0.35;

    // Central mushroom
    {
        vec3 lp = p;
        float stemH  = 0.55 * pulse;
        float stemR  = 0.07;
        float capR   = 0.28 * pulse;
        float d = sdMushroom(lp, stemH, stemR, capR);
        float capCenterY = stemH * 2.0 - capR * 0.3;
        bool inCap = (lp.y > capCenterY - capR * 0.5);
        float m = inCap ? 2.0 : 1.0;
        if (d < dist) { dist = d; matID = m; }
    }

    // Ring mushrooms
    int N = int(clamp(mushroomCount, 3.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);
        float angle = fi / float(N) * 6.28318530718 + TIME * 0.03;
        float ringR = 0.85;
        vec2 offset2d = vec2(cos(angle), sin(angle)) * ringR;
        vec3 center = vec3(offset2d.x, 0.0, offset2d.y);
        vec3 lp = p - center;

        float h11 = hash11(fi + 1.0);
        float h12v = hash11(fi + 37.3);
        float stemH = (0.25 + h11 * 0.25) * pulse;
        float stemR = 0.04 + h12v * 0.03;
        float capR  = (0.12 + h11 * 0.12) * pulse;

        float d = sdMushroom(lp, stemH, stemR, capR);
        float capCenterY = stemH * 2.0 - capR * 0.3;
        bool inCap = (lp.y > capCenterY - capR * 0.5);

        // Spore spots: hash-based sparkle
        float sporeHash = hash12(vec2(fi, floor(lp.y * 8.0 + 0.5)));
        bool isSpore = inCap && (sporeHash > 0.82);
        float m = isSpore ? 3.0 : (inCap ? 2.0 : 1.0);

        if (d < dist) { dist = d; matID = m; }
    }

    return vec2(dist, matID);
}

// ──────────────────────────────────────────────────────────────────────
// Normal via finite differences
// ──────────────────────────────────────────────────────────────────────
vec3 calcNormal(vec3 p) {
    const float eps = 0.002;
    vec2 e = vec2(eps, 0.0);
    return normalize(vec3(
        sceneSDF(p + e.xyy).x - sceneSDF(p - e.xyy).x,
        sceneSDF(p + e.yxy).x - sceneSDF(p - e.yxy).x,
        sceneSDF(p + e.yyx).x - sceneSDF(p - e.yyx).x
    ));
}

// ──────────────────────────────────────────────────────────────────────
// Raymarcher
// ──────────────────────────────────────────────────────────────────────
vec2 raymarch(vec3 ro, vec3 rd) {
    float t = 0.001;
    float matID = -1.0;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * t;
        vec2 res = sceneSDF(p);
        if (res.x < 0.001) { matID = res.y; break; }
        if (t > 12.0) break;
        t += res.x * 0.8;
    }
    return vec2(t, matID);
}

// ──────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Camera: low angle looking up at grove, slow orbit
    float camAngle = TIME * 0.12;
    float camDist  = 2.2;
    float camHeight = 0.45;
    vec3 ro = vec3(sin(camAngle) * camDist, camHeight, cos(camAngle) * camDist);
    vec3 target = vec3(0.0, 0.5, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);

    float fov = 0.75;
    vec3 rd = normalize(fwd + uv.x * right * fov + uv.y * up * fov);

    vec2 res = raymarch(ro, rd);
    float t  = res.x;
    float matIDv = res.y;

    vec3 col = vec3(0.0); // void black background

    if (matIDv >= 0.0 && t < 12.0) {
        vec3 p = ro + rd * t;
        vec3 n = calcNormal(p);

        // Key light (bioluminescent blue-white from above)
        vec3 lightDir = normalize(vec3(0.5, 1.5, 0.3));
        float diff = max(dot(n, lightDir), 0.0);
        float rim  = pow(max(1.0 - dot(-rd, n), 0.0), 3.0);

        // Pulse over time
        float pulse = 0.75 + 0.25 * sin(TIME * glowSpeed * 3.14159);
        float audioPulse = 1.0 + audioLevel * audioReact * 0.35;

        vec3 emit = vec3(0.0);

        if (matIDv < 0.5) {
            // Floor: emissive gold glow near y=0
            float floorGlow = exp(-p.y * 12.0);
            emit = vec3(2.0, 1.5, 0.0) * floorGlow * pulse * audioPulse;

        } else if (matIDv < 1.5) {
            // Stem: cyan HDR
            float stemShade = 0.3 + 0.7 * diff;
            emit = vec3(0.0, 2.0, 2.5) * (hdrPeak / 2.5) * stemShade * pulse * audioPulse;

        } else if (matIDv < 2.5) {
            // Cap: magenta HDR
            float capShade = 0.4 + 0.6 * diff + rim * 0.5;
            emit = vec3(2.5, 0.0, 2.5) * (hdrPeak / 2.5) * capShade * pulse * audioPulse;

        } else {
            // Spore spots: bright gold specular
            float spec = pow(max(dot(reflect(-lightDir, n), -rd), 0.0), 16.0);
            emit = vec3(3.0, 2.5, 0.5) * (0.6 + spec * 1.5) * pulse * audioPulse;
        }

        col = emit;
    }

    gl_FragColor = vec4(col, 1.0);
}
