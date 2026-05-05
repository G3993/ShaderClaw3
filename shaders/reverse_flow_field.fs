/*{
  "DESCRIPTION": "Bioluminescent Abyss — deep underwater scene with glowing bio-luminescent organisms drifting through domain-warped space. Single-pass 3D raymarch.",
  "CREDIT": "ShaderClaw original — 3D rewrite",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "orgCount",     "LABEL": "Organisms",      "TYPE": "float", "DEFAULT": 8.0,  "MIN": 2.0,  "MAX": 12.0 },
    { "NAME": "driftSpeed",   "LABEL": "Drift Speed",    "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 2.0  },
    { "NAME": "glowRadius",   "LABEL": "Glow Radius",    "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.1,  "MAX": 0.5  },
    { "NAME": "glowStrength", "LABEL": "Glow Strength",  "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.0,  "MAX": 3.0  },
    { "NAME": "depthFog",     "LABEL": "Depth Fog",      "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "audioPulse",   "LABEL": "Audio Pulse",    "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0  }
  ]
}*/

// ──────────────────────────────────────────────────────────────────────
// Bioluminescent Abyss — single-pass 3D raymarch, no multi-pass
// ──────────────────────────────────────────────────────────────────────

// Palette (fully saturated HDR)
const vec3 BIO_CYAN    = vec3(0.0,  2.5,  2.5);   // HDR
const vec3 BIO_MAGENTA = vec3(2.5,  0.0,  1.5);   // HDR
const vec3 DEEP_OCEAN  = vec3(0.0,  0.01, 0.08);  // background void
const vec3 JADE        = vec3(0.0,  1.8,  0.5);   // HDR accent
const vec3 SPEC_WHITE  = vec3(2.5,  3.0,  3.0);   // HDR specular

// ---- organism world position for index i ----
vec3 orgPos(int i, float t) {
    float fi = float(i);
    return vec3(
        sin(t * 0.3 * driftSpeed + fi * 2.1) * 1.8,
        sin(t * 0.2 * driftSpeed + fi * 1.7) * 1.2,
        cos(t * 0.25 * driftSpeed + fi * 1.4) * 1.8
    );
}

// ---- organism radius with audio-reactive pulse ----
float orgRadius(int i, float t) {
    float fi = float(i);
    float base = glowRadius * (0.8 + 0.2 * sin(t * 2.0 + fi * 1.3));
    return base * (1.0 + audioLevel * audioPulse * 0.3);
}

// ---- color by index parity: even=cyan, odd=magenta ----
vec3 orgColor(int i) {
    return (mod(float(i), 2.0) < 0.5) ? BIO_CYAN : BIO_MAGENTA;
}

// ---- scene SDF: minimum over all active organisms ----
float sceneSDF(vec3 p, float t) {
    float d = 1e9;
    int N = int(clamp(orgCount, 2.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        vec3 op = orgPos(i, t);
        float r  = orgRadius(i, t);
        d = min(d, length(p - op) - r);
    }
    return d;
}

// ---- central-difference normal ----
vec3 calcNormal(vec3 p, float t) {
    const float eps = 0.002;
    return normalize(vec3(
        sceneSDF(p + vec3(eps, 0.0, 0.0), t) - sceneSDF(p - vec3(eps, 0.0, 0.0), t),
        sceneSDF(p + vec3(0.0, eps, 0.0), t) - sceneSDF(p - vec3(0.0, eps, 0.0), t),
        sceneSDF(p + vec3(0.0, 0.0, eps), t) - sceneSDF(p - vec3(0.0, 0.0, eps), t)
    ));
}

// ---- find closest organism at a point ----
void closestOrg(vec3 p, float t, out int hitIdx, out float hitDist) {
    hitDist = 1e9;
    hitIdx  = 0;
    int N = int(clamp(orgCount, 2.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        vec3 op = orgPos(i, t);
        float r  = orgRadius(i, t);
        float d  = length(p - op) - r;
        if (d < hitDist) { hitDist = d; hitIdx = i; }
    }
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Camera: slowly tilting, orbiting slightly
    vec3 ro = vec3(0.0, 0.0, 5.0);
    ro.y = sin(TIME * 0.1) * 0.8;
    ro.x = sin(TIME * 0.07) * 0.5;

    // Look-at the origin
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up    = cross(right, fwd);
    vec3 rd    = normalize(fwd + uv.x * right + uv.y * up);

    float t   = TIME;
    float tMax = 18.0;

    // 64-step sphere march
    float tt   = 0.0;
    bool  hit  = false;
    int   hitI = 0;
    const int STEPS = 64;

    for (int s = 0; s < STEPS; s++) {
        vec3 p = ro + rd * tt;
        float d = sceneSDF(p, t);
        if (d < 0.002) {
            hit = true;
            int hi; float hd;
            closestOrg(p, t, hi, hd);
            hitI = hi;
            break;
        }
        tt += max(d * 0.7, 0.002);
        if (tt > tMax) break;
    }

    vec3 col = DEEP_OCEAN;

    if (hit) {
        vec3 hp  = ro + rd * tt;
        vec3 nor = calcNormal(hp, t);

        // Deep-sea lighting: cyan key from above + ambient blue-green fill
        vec3 lightDir = normalize(vec3(0.3, 1.0, 0.5));
        float diff    = max(dot(nor, lightDir), 0.0);
        vec3  surf    = orgColor(hitI);
        float spec    = pow(max(dot(reflect(-lightDir, nor), -rd), 0.0), 32.0);

        vec3 amb = surf * 0.12 + vec3(0.0, 0.04, 0.08);
        col = amb + surf * diff * 0.7 + SPEC_WHITE * spec * 0.4;

        // Jade rim glow
        float rim = pow(1.0 - max(dot(nor, -rd), 0.0), 3.0);
        col += JADE * rim * 0.3;

        // Soft glow ring at sphere surface using derivative-based AA
        float dAtSurf = sceneSDF(hp, t);
        float fw = length(vec2(dFdx(dAtSurf), dFdy(dAtSurf)));
        float ring = 1.0 - smoothstep(-fw * 2.0, fw * 2.0, dAtSurf);
        col += surf * ring * glowStrength * 0.5;
    }

    // Analytical volumetric glow halos — no additional march needed
    int N = int(clamp(orgCount, 2.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        vec3  op = orgPos(i, t);
        float r  = orgRadius(i, t);

        // Closest point on ray to organism centre
        float tGlow   = clamp(dot(op - ro, rd), 0.0, tMax);
        vec3  closest = ro + rd * tGlow;
        float dist2org = length(closest - op);

        // Exponential falloff from sphere surface
        float surfDist = max(dist2org - r, 0.0);
        float glowFade = exp(-surfDist * 7.0);

        // Occlusion: fade glow behind a solid hit
        float glowMask;
        if (hit) {
            float occlude = smoothstep(0.0, 0.4, tGlow - tt);
            glowMask = glowFade * (1.0 - clamp(occlude, 0.0, 1.0));
        } else {
            glowMask = glowFade;
        }

        col += orgColor(i) * glowStrength * glowMask * 0.18
             * (1.0 + audioLevel * audioPulse * 0.4);
    }

    // Depth fog toward deep ocean
    float fogAmt = 1.0 - exp(-tt * depthFog * 0.3);
    col = mix(col, DEEP_OCEAN, fogAmt);

    // Background caustic shimmer when no geometry hit
    if (!hit) {
        float caustic = 0.5 + 0.5 * sin(uv.x * 8.0 + TIME * 0.4)
                                   * sin(uv.y * 6.0 - TIME * 0.3);
        col += DEEP_OCEAN * caustic * 0.3;
    }

    gl_FragColor = vec4(col, 1.0);
}
