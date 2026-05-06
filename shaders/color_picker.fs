/*{
  "DESCRIPTION": "Prism Array — raymarched glass prisms with HDR rainbow dispersion",
  "CREDIT": "auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Abstract"],
  "INPUTS": [
    { "NAME": "prismCount", "LABEL": "Prisms", "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 10.0 },
    { "NAME": "rotSpeed",   "LABEL": "Speed",  "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR",    "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioReact", "LABEL": "Audio",  "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// --- Rotation helpers ---
mat2 rot2(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}
vec3 rotY(vec3 p, float a) { p.xz = rot2(a) * p.xz; return p; }
vec3 rotX(vec3 p, float a) { p.yz = rot2(a) * p.yz; return p; }

// --- SDF primitives ---
float sdPrism(vec3 p, vec2 h) {
    // Equilateral triangle prism along Y axis
    vec3 q = abs(p);
    float d = max(q.z - h.x,
                  max(q.x * 0.866025 + p.z * 0.5, -p.z) - h.x * 0.5);
    return max(d, q.y - h.y);
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// --- Scene SDF ---
float scene(vec3 p) {
    float t = TIME * rotSpeed;
    float audio = 1.0 + audioLevel * audioReact * 0.35;

    // Central cube (slowly tumbling)
    vec3 cp = rotX(rotY(p, t * 0.7), t * 0.5);
    float d = sdBox(cp, vec3(0.18 * audio));

    // Orbiting triangular prisms
    int N = int(clamp(prismCount, 2.0, 10.0));
    float ringR = 0.90;
    for (int i = 0; i < 10; i++) {
        if (i >= N) break;
        float fi = float(i);
        float angle = fi / float(N) * 6.28318 + t;
        float bob = sin(t * 1.3 + fi * 1.1) * 0.15 * audio;
        vec3 center = vec3(cos(angle) * ringR, bob, sin(angle) * ringR);
        vec3 lp = p - center;
        // Tilt each prism to face tangent to orbit
        lp = rotY(lp, -angle - 1.5708);
        // Spin the prism on its own axis
        lp = rotX(lp, t * 1.2 + fi * 0.7);
        float pr = sdPrism(lp, vec2(0.14 * audio, 0.28));
        d = min(d, pr);
    }
    return d;
}

// --- Normal via finite differences ---
vec3 calcNormal(vec3 p) {
    float eps = 0.001;
    return normalize(vec3(
        scene(p + vec3(eps, 0.0, 0.0)) - scene(p - vec3(eps, 0.0, 0.0)),
        scene(p + vec3(0.0, eps, 0.0)) - scene(p - vec3(0.0, eps, 0.0)),
        scene(p + vec3(0.0, 0.0, eps)) - scene(p - vec3(0.0, 0.0, eps))
    ));
}

// --- Environment: fully-saturated rainbow via sin, no white mixing ---
vec3 envRainbow(vec3 rd) {
    float ang = atan(rd.z, rd.x) + rd.y * 1.2;
    vec3 col = 0.5 + 0.5 * cos(ang * 3.0 + vec3(0.0, 2.094, 4.189));
    return col * hdrPeak;
}

// --- Dispersion: refract R/G/B at different IOR ---
vec3 disperse(vec3 rd, vec3 n) {
    vec3 rR = refract(rd, n, 1.0 / 1.47);
    vec3 rG = refract(rd, n, 1.0 / 1.50);
    vec3 rB = refract(rd, n, 1.0 / 1.53);
    float r = envRainbow(rR).r;
    float g = envRainbow(rG).g;
    float b = envRainbow(rB).b;
    return vec3(r, g, b);
}

// --- Highlight colors: magenta/cyan/gold/orange cycling ---
vec3 prismHighlight(float angle) {
    float t4 = mod(angle / 6.28318 * 4.0, 4.0);
    int idx  = int(t4);
    float fr = fract(t4);
    vec3 a, b;
    // Fully saturated HDR colors — no white mixing
    if      (idx == 0) { a = vec3(1.0, 0.0, 1.0); b = vec3(0.0, 1.0, 1.0); }
    else if (idx == 1) { a = vec3(0.0, 1.0, 1.0); b = vec3(1.0, 0.8, 0.0); }
    else if (idx == 2) { a = vec3(1.0, 0.8, 0.0); b = vec3(1.0, 0.4, 0.0); }
    else               { a = vec3(1.0, 0.4, 0.0); b = vec3(1.0, 0.0, 1.0); }
    return mix(a, b, fr);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float audio = 1.0 + audioLevel * audioReact * 0.35;
    float t = TIME * rotSpeed;

    // Orbiting camera
    float camAngle = t * 0.4;
    float camElev  = 0.4 + sin(t * 0.18) * 0.2;
    float camDist  = 3.2;
    vec3 ro = vec3(cos(camAngle) * camDist * cos(camElev),
                   sin(camElev) * camDist,
                   sin(camAngle) * camDist * cos(camElev));
    vec3 forward = normalize(-ro);
    vec3 right   = normalize(cross(vec3(0.0, 1.0, 0.0), forward));
    vec3 up      = cross(forward, right);

    float fov = 1.3;
    vec3 rd = normalize(forward * fov + right * uv.x + up * uv.y);

    // --- 64-step raymarch ---
    float dist = 0.0;
    float hit  = 0.0;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dist;
        float d = scene(p);
        if (d < 0.001) { hit = 1.0; break; }
        if (dist > 12.0) break;
        dist += d;
    }

    // Black void background
    vec3 col = vec3(0.01, 0.005, 0.02);

    if (hit > 0.5) {
        vec3 p = ro + rd * dist;
        vec3 n = calcNormal(p);

        // Black silhouette edge
        float edge = 1.0 - smoothstep(0.0, 0.2, abs(dot(n, -rd)));

        // Rainbow dispersion (per-channel IOR refraction)
        vec3 dispCol = disperse(rd, n);

        // Highlight color based on position angle
        float ang = atan(p.z, p.x);
        vec3 highlight = prismHighlight(ang) * hdrPeak * audio;

        // Fresnel for specular vs refraction blend
        float fresnel = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);

        // Specular highlight — white-hot spike
        vec3 lightDir = normalize(vec3(1.2, 2.0, 0.8));
        float spec = pow(max(dot(reflect(rd, n), lightDir), 0.0), 32.0);
        vec3 specCol = vec3(3.0, 2.5, 2.0) * spec * hdrPeak;

        // Composite: refracted env + saturated highlight + specular
        col = mix(dispCol * hdrPeak, highlight, 0.35);
        col = mix(col, col + specCol, fresnel);
        // Black edge
        col *= (1.0 - edge * 0.9);
    }

    // No tone mapping — raw HDR output
    gl_FragColor = vec4(col, 1.0);
}
