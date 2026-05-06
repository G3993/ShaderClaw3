/*{
  "DESCRIPTION": "Coral Mandala — 3D raymarched ring of bioluminescent coral polyp clusters with 8-fold rotational symmetry",
  "CREDIT": "ShaderClaw — coral mandala v2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "orbitSpeed", "LABEL": "Orbit Speed",  "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "symFold",    "LABEL": "Symmetry",     "TYPE": "float", "DEFAULT": 8.0,  "MIN": 3.0, "MAX": 12.0 },
    { "NAME": "ringRadius", "LABEL": "Ring Radius",  "TYPE": "float", "DEFAULT": 1.3,  "MIN": 0.4, "MAX": 2.5 },
    { "NAME": "blobSize",   "LABEL": "Blob Size",    "TYPE": "float", "DEFAULT": 0.28, "MIN": 0.05,"MAX": 0.6 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 4.0 },
    { "NAME": "glowStr",    "LABEL": "Glow",         "TYPE": "float", "DEFAULT": 1.5,  "MIN": 0.0, "MAX": 4.0 },
    { "NAME": "audioReact", "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "bg",         "LABEL": "Background",   "TYPE": "color", "DEFAULT": [0.0, 0.005, 0.02, 1.0] }
  ]
}*/

#define PI 3.14159265359
#define MAX_STEPS 64
#define MAX_DIST  8.0
#define SURF_DIST 0.0015

// ── Smooth union ────────────────────────────────────────────────────────────
float smin(float a, float b, float k) {
    float h = max(k - abs(a - b), 0.0) / k;
    return min(a, b) - h * h * k * 0.25;
}

// ── Trig helpers ────────────────────────────────────────────────────────────
vec3 rotY3(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(c*p.x + s*p.z, p.y, -s*p.x + c*p.z);
}
vec3 rotX3(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(p.x, c*p.y - s*p.z, s*p.y + c*p.z);
}

// ── 8-fold radial symmetry in XZ plane ─────────────────────────────────────
vec3 mandalaFold(vec3 p, float n) {
    float sector = PI / n; // half-sector angle (full sector = 2*PI/n)
    float a = atan(p.z, p.x);
    a = mod(a + sector, 2.0 * sector) - sector;
    float r = length(p.xz);
    return vec3(cos(a) * r, p.y, sin(a) * r);
}

// ── Coral polyp cluster SDF: center sphere + 3 branch spheres ───────────────
float coralPolyp(vec3 p, float r) {
    float bs = blobSize * r;
    float d = length(p) - bs * 0.52;
    d = smin(d, length(p - vec3(bs * 0.72, bs * 0.3, 0.0)) - bs * 0.28, bs * 0.35);
    d = smin(d, length(p - vec3(-bs * 0.45, bs * 0.6, bs * 0.25)) - bs * 0.22, bs * 0.30);
    d = smin(d, length(p - vec3(0.0, bs * 0.8, -bs * 0.4)) - bs * 0.20, bs * 0.28);
    return d;
}

// ── Scene SDF: ring of polyps + secondary ring + central nodule ─────────────
float sceneSDF(vec3 p, float t) {
    // Global slow rotation
    p = rotY3(p, t * orbitSpeed * 0.18);
    p = rotX3(p, sin(t * orbitSpeed * 0.12) * 0.3);

    // Apply mandala fold (N-fold symmetry)
    vec3 ps = mandalaFold(p, symFold);

    // Position one polyp at the ring radius
    vec3 polypPos = ps - vec3(ringRadius, 0.0, 0.0);

    // Primary polyp cluster (1 per sector after fold)
    float r = 1.0; // scale relative unit
    float d = coralPolyp(polypPos, r);

    // Secondary smaller cluster slightly above/below
    vec3 polypPos2 = ps - vec3(ringRadius * 0.82, blobSize * 0.55, 0.0);
    d = smin(d, coralPolyp(polypPos2, 0.6), blobSize * 0.4);

    // Central hub sphere
    float hub = length(p) - blobSize * 0.7;
    d = smin(d, hub, blobSize * 0.5);

    return d;
}

// ── 4-color bioluminescent palette by sector angle ──────────────────────────
vec3 coralColor(vec3 p, float t) {
    // Angle in XZ plane after removing fold — use original p angle
    float a = fract(atan(p.z, p.x) / (2.0 * PI) + 0.5);
    float sector = fract(a * symFold);
    float h = hdrPeak;

    // 4 saturated bioluminescent colors cycling through sectors
    float idx = floor(a * 4.0);
    float frac = fract(a * 4.0);
    vec3 c;
    if (idx < 1.0)      c = mix(vec3(1.0, 0.30, 0.38), vec3(0.0, 0.90, 0.78), frac); // coral→turquoise
    else if (idx < 2.0) c = mix(vec3(0.0, 0.90, 0.78), vec3(0.28, 1.0, 0.18),  frac); // turquoise→lime
    else if (idx < 3.0) c = mix(vec3(0.28, 1.0, 0.18), vec3(1.0, 0.18, 0.62),  frac); // lime→pink
    else                c = mix(vec3(1.0, 0.18, 0.62), vec3(1.0, 0.30, 0.38),   frac); // pink→coral

    // Animate hue slightly with TIME
    float hShift = sin(t * orbitSpeed * 0.4 + length(p) * 2.0) * 0.08;
    c = mix(c, c.yzx, abs(hShift)); // subtle hue rotation

    return c * h;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;
    float audioBoost = 1.0 + audioLevel * audioReact * 0.7
                           + audioBass  * audioReact * 0.3;

    // Camera: orbits around the mandala, slightly elevated
    float camAngle = t * orbitSpeed * 0.22;
    vec3 ro = vec3(sin(camAngle) * 4.5, 1.8 + sin(t * orbitSpeed * 0.15) * 0.4, cos(camAngle) * 4.5);
    vec3 target = vec3(0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd * 2.0 + right * uv.x + up * uv.y);

    // Raymarch
    float dist = 0.05;
    vec3 glowAccum = vec3(0.0);
    bool hit = false;
    vec3 hitP = vec3(0.0);

    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * dist;
        float d = sceneSDF(p, t);

        // Glow accumulation near surface
        float gs = glowStr / max(blobSize, 0.05);
        glowAccum += coralColor(p, t) * exp(-max(d, 0.0) * gs) * 0.04;

        if (d < SURF_DIST) {
            hit = true;
            hitP = p;
            break;
        }
        dist += d;
        if (dist > MAX_DIST) break;
    }

    vec3 col = bg.rgb;

    if (hit) {
        // Numerical normal
        float e = 0.001;
        vec3 n = normalize(vec3(
            sceneSDF(hitP + vec3(e,0,0), t) - sceneSDF(hitP - vec3(e,0,0), t),
            sceneSDF(hitP + vec3(0,e,0), t) - sceneSDF(hitP - vec3(0,e,0), t),
            sceneSDF(hitP + vec3(0,0,e), t) - sceneSDF(hitP - vec3(0,0,e), t)
        ));

        vec3 neonC = coralColor(hitP, t);

        // Fresnel: rim glow on grazing angles
        float fres = 1.0 - abs(dot(n, -rd));
        float diffuse = 0.25 + 0.75 * max(dot(n, normalize(vec3(0.5, 1.0, 0.3))), 0.0);

        // Specular: white-hot highlight
        vec3 h = normalize(normalize(vec3(0.5, 1.0, 0.3)) + (-rd));
        float spec = pow(max(dot(n, h), 0.0), 32.0) * hdrPeak;

        col = neonC * (diffuse + fres * 0.6) * audioBoost + vec3(spec);
    }

    col += glowAccum * audioBoost;

    gl_FragColor = vec4(col, 1.0);
}
