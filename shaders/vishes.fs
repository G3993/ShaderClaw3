/*{
  "DESCRIPTION": "Coral Reef — 3D raymarched bioluminescent coral forest with audio-reactive polyps",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "audioReact",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "cameraSpeed",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.3 },
    { "NAME": "coralDensity",  "TYPE": "float", "MIN": 1.0, "MAX": 8.0, "DEFAULT": 5.0 },
    { "NAME": "glowIntensity", "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.5 }
  ]
}*/

precision highp float;

// ── Palette ──────────────────────────────────────────────────────────────────
const vec3 NAVY         = vec3(0.00, 0.02, 0.08);
const vec3 CORAL_WARM   = vec3(1.00, 0.20, 0.35);
const vec3 TEAL_GLOW    = vec3(0.00, 1.80, 1.20);
const vec3 MAGENTA_GLOW = vec3(1.50, 0.00, 1.80);
const vec3 IVORY_TIP    = vec3(2.20, 2.00, 1.60);

// ── Hash helpers ─────────────────────────────────────────────────────────────
float hash(float n) { return fract(sin(n) * 43758.5453123); }
float hash2f(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123); }

// ── SDF primitives ────────────────────────────────────────────────────────────
float sdCappedCylinder(vec3 p, float r, float h) {
    vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(r, h);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

// ── Scene SDF ────────────────────────────────────────────────────────────────
// Returns (dist, materialId) where materialId:
//   0 = ground, 1.x = stalk, 2.x = polyp tip
vec2 sceneSDF(vec3 p, float audio) {
    float d     = 1e9;
    float matId = 0.0;

    // Ground plane
    float ground = p.y + 0.2;
    if (ground < d) { d = ground; matId = 0.0; }

    // Coral grid — 7x7 cells
    for (int ix = -3; ix <= 3; ix++) {
        for (int iz = -3; iz <= 3; iz++) {
            vec2  cellId = vec2(float(ix), float(iz));
            float h1     = hash2f(cellId);
            float h2     = hash2f(cellId + vec2(13.7,  5.3));
            float h3     = hash2f(cellId + vec2( 7.1, 23.9));

            vec3  center = vec3(float(ix) * 1.4 + (h1 - 0.5) * 0.8,
                                -0.2,
                                float(iz) * 1.4 + (h2 - 0.5) * 0.8);

            float radius = 0.04 + h3 * 0.05;
            float height = 0.40 + h1 * 0.90;

            // Stalk + base smooth union
            float stalk      = sdCappedCylinder(p - center, radius, height * 0.5);
            float baseSphere = sdSphere(p - center - vec3(0.0, -height * 0.5 + 0.05, 0.0),
                                        radius * 1.6);
            float stalkShape = smin(stalk, baseSphere, 0.08);

            if (stalkShape < d) { d = stalkShape; matId = 1.0 + h1 * 0.01; }

            // Polyp tip — audio-reactive radius
            vec3  tipPos = center + vec3(0.0, height * 0.5 + 0.08, 0.0);
            float tipR   = 0.07 + audio * 0.03 * glowIntensity;
            float tip    = sdSphere(p - tipPos, tipR);

            if (tip < d) { d = tip; matId = 2.0 + h2 * 0.01; }
        }
    }

    return vec2(d, matId);
}

// ── Normals (central differences) ────────────────────────────────────────────
vec3 calcNormal(vec3 p, float audio) {
    const float eps = 0.002;
    return normalize(vec3(
        sceneSDF(p + vec3(eps, 0.0, 0.0), audio).x - sceneSDF(p - vec3(eps, 0.0, 0.0), audio).x,
        sceneSDF(p + vec3(0.0, eps, 0.0), audio).x - sceneSDF(p - vec3(0.0, eps, 0.0), audio).x,
        sceneSDF(p + vec3(0.0, 0.0, eps), audio).x - sceneSDF(p - vec3(0.0, 0.0, eps), audio).x
    ));
}

void main() {
    // Audio modulator
    float audio = 0.5 + 0.5 * audioBass * audioReact;

    // ── Camera setup ──────────────────────────────────────────────────────────
    float camAngle = TIME * cameraSpeed * 0.4;
    vec3  ro       = vec3(cos(camAngle) * 4.0,
                          1.5 + sin(TIME * 0.2) * 0.5,
                          sin(camAngle) * 4.0);
    vec3  target   = vec3(0.0, 0.5, 0.0);
    vec3  forward  = normalize(target - ro);
    vec3  right    = normalize(cross(forward, vec3(0.0, 1.0, 0.0)));
    vec3  up       = cross(right, forward);

    vec2  screenUV = isf_FragNormCoord * 2.0 - 1.0;
    screenUV.x    *= RENDERSIZE.x / RENDERSIZE.y;
    vec3  rd       = normalize(forward + screenUV.x * right + screenUV.y * up);

    // ── Raymarching (64 steps) ────────────────────────────────────────────────
    float t     = 0.01;
    float matId = -1.0;
    bool  hit   = false;

    for (int i = 0; i < 64; i++) {
        vec3  pos  = ro + rd * t;
        vec2  res  = sceneSDF(pos, audio);
        float dist = res.x;
        if (dist < 0.005) {
            hit   = true;
            matId = res.y;
            break;
        }
        t += max(dist, 0.01);
        if (t > 20.0) break;
    }

    // ── Shading ───────────────────────────────────────────────────────────────
    vec3 col;

    if (!hit) {
        // Deep navy void + bioluminescent particle field
        col = NAVY;
        col += NAVY * 0.5 * max(0.0, -rd.y);
        // Deep-sea particle sparkles
        vec2  starUV   = rd.xy / (abs(rd.z) + 0.001);
        float starHash = hash2f(floor(starUV * 120.0));
        float starGlow = smoothstep(0.97, 1.0, starHash);
        col += TEAL_GLOW * starGlow * 0.15;
    } else {
        vec3  pos      = ro + rd * t;
        vec3  nor      = calcNormal(pos, audio);
        vec3  lightPos = vec3(0.0, 5.0, 0.0);
        vec3  lDir     = normalize(lightPos - pos);
        float diff     = max(dot(nor, lDir), 0.0);
        vec3  halfVec  = normalize(lDir - rd);
        float spec     = pow(max(dot(nor, halfVec), 0.0), 32.0);

        if (matId < 0.5) {
            // Ground
            col  = NAVY * 2.0;
            col += TEAL_GLOW * 0.08 * diff;
            float grid = step(0.48, fract(pos.x * 2.0)) * step(0.48, fract(pos.z * 2.0));
            col += TEAL_GLOW * grid * 0.03;
        } else if (matId < 2.0) {
            // Coral stalk
            col  = CORAL_WARM * (0.5 + 0.5 * diff) * (1.0 + (audio - 0.5) * 0.5);
            col += CORAL_WARM * spec * 0.4;
            col += NAVY * 0.3;
        } else {
            // Polyp tips — bioluminescent colors keyed by hash
            float tipHash = fract((matId - 2.0) * 10000.0);
            vec3  tipBase;
            if (tipHash < 0.33) {
                tipBase = TEAL_GLOW;
            } else if (tipHash < 0.66) {
                tipBase = MAGENTA_GLOW;
            } else {
                tipBase = IVORY_TIP;
            }
            float brightness = glowIntensity * (0.8 + audio * 0.8);
            col  = tipBase * brightness;
            col += tipBase * spec * 1.2 * audio;
            // Subtle halo emission
            float halo = exp(-t * 0.02) * 0.1 * audio;
            col += tipBase * halo;
        }

        // Underwater depth fog
        float fogAmt = 1.0 - exp(-t * 0.06);
        col = mix(col, NAVY, fogAmt);
    }

    gl_FragColor = vec4(col, 1.0);
}
