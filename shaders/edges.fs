/*{
  "DESCRIPTION": "Neon Solar System — raymarched HDR neon planets orbiting a white-hot star",
  "CREDIT": "auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Abstract"],
  "INPUTS": [
    { "NAME": "planetCount", "LABEL": "Planets",     "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 12.0 },
    { "NAME": "orbitSpeed",  "LABEL": "Orbit Speed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioReact",  "LABEL": "Audio",       "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// --- Rotation helpers ---
mat2 rot2(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}
vec3 rotY(vec3 p, float a) { p.xz = rot2(a) * p.xz; return p; }
vec3 rotX(vec3 p, float a) { p.yz = rot2(a) * p.yz; return p; }

// --- SDFs ---
float sdSphere(vec3 p, float r) {
    return length(p) - r;
}

// --- Planet color palette (fully saturated HDR, no white mixing) ---
vec3 planetColor(int idx) {
    if (idx == 0)  return vec3(1.0, 0.0, 1.0);   // magenta
    if (idx == 1)  return vec3(0.0, 1.0, 1.0);   // cyan
    if (idx == 2)  return vec3(1.0, 0.8, 0.0);   // gold
    if (idx == 3)  return vec3(1.0, 0.4, 0.0);   // orange
    if (idx == 4)  return vec3(0.4, 0.0, 1.0);   // violet
    if (idx == 5)  return vec3(0.0, 1.0, 0.4);   // lime
    if (idx == 6)  return vec3(1.0, 0.0, 0.4);   // hot pink
    if (idx == 7)  return vec3(0.0, 0.6, 1.0);   // sky blue
    if (idx == 8)  return vec3(1.0, 1.0, 0.0);   // yellow
    if (idx == 9)  return vec3(0.6, 0.0, 1.0);   // purple
    if (idx == 10) return vec3(0.0, 1.0, 0.6);   // seafoam
    return vec3(1.0, 0.3, 0.8);                  // pink
}

// Planet world position at time t
vec3 planetPos(int i, float t) {
    float fi = float(i);
    // Orbit radius grows with index
    float radius = 0.6 + fi * 0.22;
    // Inner planets orbit faster
    float speed = orbitSpeed * (1.0 + fi * 0.15);
    // Varied 3D orbit inclinations
    float inclination = fi * 0.52 + 0.3;
    float ascNode     = fi * 1.1;
    float angle = t * speed + fi * 0.9;
    // Flat orbit, then tilt
    vec3 pos = vec3(cos(angle) * radius, 0.0, sin(angle) * radius);
    pos = rotX(pos, inclination);
    pos = rotY(pos, ascNode);
    return pos;
}

// Returns (dist, matID)  matID: 0 = star, i+1 = planet i
vec2 scene(vec3 p) {
    float t = TIME;

    // Star at origin — white-hot sphere
    float dStar = sdSphere(p, 0.35);
    vec2 res = vec2(dStar, 0.0);

    int N = int(clamp(planetCount, 2.0, 12.0));
    float audio = 1.0 + audioLevel * audioReact * 0.35;
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float size = (0.09 + float(i) * 0.005) * audio;
        vec3 pp = planetPos(i, t);
        float d = sdSphere(p - pp, size);
        if (d < res.x) {
            res = vec2(d, float(i + 1));
        }
    }
    return res;
}

vec3 calcNormal(vec3 p) {
    float eps = 0.001;
    return normalize(vec3(
        scene(p + vec3(eps, 0.0, 0.0)).x - scene(p - vec3(eps, 0.0, 0.0)).x,
        scene(p + vec3(0.0, eps, 0.0)).x - scene(p - vec3(0.0, eps, 0.0)).x,
        scene(p + vec3(0.0, 0.0, eps)).x - scene(p - vec3(0.0, 0.0, eps)).x
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord.xy * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float audio = 1.0 + audioLevel * audioReact * 0.35;
    float t = TIME;

    // Orbiting camera — slowly circles the system
    float camAngle = t * orbitSpeed * 0.25;
    float camElev  = 0.5 + sin(t * 0.11) * 0.25;
    float camDist  = 4.5;
    vec3 ro = vec3(cos(camAngle) * camDist * cos(camElev),
                   sin(camElev) * camDist,
                   sin(camAngle) * camDist * cos(camElev));
    vec3 forward = normalize(-ro);
    vec3 right   = normalize(cross(vec3(0.0, 1.0, 0.0), forward));
    vec3 up      = cross(forward, right);

    float fov = 1.2;
    vec3 rd = normalize(forward * fov + right * uv.x + up * uv.y);

    // Star glow: volumetric approx — closest approach of ray to origin
    float starGlowT = -dot(ro, rd);
    vec3  closestP  = ro + rd * max(starGlowT, 0.0);
    float starDist2 = dot(closestP, closestP);
    vec3  starGlow  = vec3(3.0, 2.5, 2.0) * hdrPeak * 0.08 / (starDist2 + 0.05);

    // --- 64-step raymarch ---
    float dist  = 0.0;
    float matID = -1.0;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dist;
        vec2 res = scene(p);
        if (res.x < 0.001) {
            matID = res.y;
            break;
        }
        if (dist > 14.0) break;
        dist += res.x;
    }

    // Black void background + star glow
    vec3 col = vec3(0.01, 0.005, 0.02) + starGlow;

    if (matID >= 0.0) {
        vec3 p = ro + rd * dist;
        vec3 n = calcNormal(p);

        // Black silhouette edge
        float edge = 1.0 - smoothstep(0.0, 0.25, abs(dot(n, -rd)));

        if (matID < 0.5) {
            // --- Star surface: white-hot with shimmer ---
            float shimmer = 0.85 + 0.15 * sin(dot(n, vec3(4.0, 7.0, 3.0)) * 8.0 + t * 3.0);
            col = vec3(3.0, 2.5, 2.0) * hdrPeak * shimmer * audio;
            col *= (1.0 - edge * 0.85);
        } else {
            // --- Planet surface: cinematic lighting from star ---
            int planetIdx = int(matID) - 1;
            vec3 baseCol  = planetColor(planetIdx) * hdrPeak;

            // Diffuse: light from star at origin
            vec3 lightDir = normalize(-p);
            float diff = max(dot(n, lightDir), 0.0);

            // Specular (Blinn-Phong from star)
            vec3 halfVec = normalize(lightDir - rd);
            float spec   = pow(max(dot(n, halfVec), 0.0), 48.0);
            vec3 specCol = vec3(3.0, 2.5, 2.0) * spec * hdrPeak * 0.8;

            // Rim: star-side glow
            float rim    = pow(1.0 - max(dot(n, -rd), 0.0), 2.5);
            vec3 rimCol  = baseCol * rim * 0.6;

            // Dark-space ambient
            vec3 ambient = baseCol * 0.08;

            col = ambient + baseCol * diff * 1.4 + specCol + rimCol;
            col *= (1.0 - edge * 0.95);
        }
    }

    // No tone mapping — raw HDR output
    gl_FragColor = vec4(col, 1.0);
}
