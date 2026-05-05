/*{
  "DESCRIPTION": "Frost Crystal Lattice — 3D raymarched Voronoi crystal formation growing through deep midnight, cold blue specular facets and HDR white-hot edges",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "Easel / ShaderClaw v3",
  "INPUTS": [
    { "NAME": "growthSpeed",  "LABEL": "Growth Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3  },
    { "NAME": "crystalCount", "LABEL": "Crystal Count", "TYPE": "float", "MIN": 3.0, "MAX": 9.0, "DEFAULT": 6.0  },
    { "NAME": "cameraSpeed",  "LABEL": "Camera Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15 },
    { "NAME": "edgeGlow",     "LABEL": "Edge Glow",     "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.8  },
    { "NAME": "hdrBoost",     "LABEL": "HDR Boost",     "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.2  },
    { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0  }
  ]
}*/

precision highp float;

// ── Palette: cold crystal ─────────────────────────────────────────────────────
const vec3 MIDNIGHT  = vec3(0.00, 0.00, 0.04);
const vec3 ICE_BLUE  = vec3(0.10, 0.50, 1.00);
const vec3 GLACIER   = vec3(0.00, 0.80, 1.00);
const vec3 ARCTIC    = vec3(0.50, 0.85, 1.00);
const vec3 WHITE_HOT = vec3(2.20, 2.50, 3.00);

float hash(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
vec3  hash3(float n) { return vec3(hash(n), hash(n+1.7), hash(n+3.4)); }

// Voronoi-like crystal nucleus positions
vec3 crystalNucleus(float i, float N, float t) {
    float g = growthSpeed;
    vec3 seed = hash3(i) * 2.0 - 1.0;
    seed *= (0.6 + 0.4 * hash(i + 7.3));
    // Slow drift for growth animation
    seed += vec3(sin(t * g * 0.7 + i), cos(t * g * 0.5 + i * 1.3), sin(t * g * 0.3 + i * 0.9)) * 0.12;
    return seed * 1.8;
}

// SDF: distance to Voronoi-faceted crystal (stretched octahedron)
float sdCrystal(vec3 p, vec3 center, float scale) {
    vec3 q = (p - center) / scale;
    // Elongate along random axis for icicle/shard feel
    q.y *= 1.6;
    float oct = (abs(q.x) + abs(q.y) + abs(q.z)) - 1.0;
    return oct * scale;
}

// Scene: cluster of crystals
vec2 sceneSDF(vec3 p, float N, float t) {
    float d = 1e9;
    float id = 0.0;
    for (int i = 0; i < 9; i++) {
        if (float(i) >= N) break;
        vec3 nuc = crystalNucleus(float(i), N, t);
        float scale = 0.25 + hash(float(i) * 3.7) * 0.35;
        float di = sdCrystal(p, nuc, scale);
        if (di < d) { d = di; id = float(i); }
    }
    return vec2(d, id);
}

vec3 sceneNormal(vec3 p, float N, float t) {
    const float e = 0.001;
    return normalize(vec3(
        sceneSDF(p + vec3(e,0,0), N, t).x - sceneSDF(p - vec3(e,0,0), N, t).x,
        sceneSDF(p + vec3(0,e,0), N, t).x - sceneSDF(p - vec3(0,e,0), N, t).x,
        sceneSDF(p + vec3(0,0,e), N, t).x - sceneSDF(p - vec3(0,0,e), N, t).x
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t   = TIME;
    float aud = 1.0 + (audioLevel + audioBass * 0.8) * audioReact * 0.5;
    float N   = crystalCount;

    // Orbiting camera — slow, close up on cluster
    float camAng  = t * cameraSpeed;
    float camElev = 0.4 * sin(t * cameraSpeed * 0.5);
    vec3 camPos = vec3(cos(camAng) * 3.5, camElev, sin(camAng) * 3.5);
    vec3 fwd    = normalize(-camPos);
    vec3 right  = normalize(cross(fwd, vec3(0, 1, 0)));
    vec3 up     = cross(right, fwd);
    vec3 rd     = normalize(fwd + uv.x * right * 0.7 + uv.y * up * 0.7);

    // Raymarch
    float dist = 0.0;
    float hitID = -1.0;
    for (int i = 0; i < 64; i++) {
        vec3 p  = camPos + rd * dist;
        vec2 res = sceneSDF(p, N, t);
        if (res.x < 0.002) { hitID = res.y; break; }
        if (dist > 12.0) break;
        dist += res.x * 0.8;
    }

    vec3 col = MIDNIGHT;

    if (hitID >= 0.0) {
        vec3 p   = camPos + rd * dist;
        vec3 nor = sceneNormal(p, N, t);

        vec3 lightA = normalize(vec3(1.5, 2.0, 0.5));
        vec3 lightB = normalize(vec3(-1.0, 0.5, -1.5));

        float diffA = max(dot(nor, lightA), 0.0);
        float diffB = max(dot(nor, lightB), 0.0) * 0.4;
        float specA = pow(max(dot(reflect(-lightA, nor), -rd), 0.0), 64.0);
        float specB = pow(max(dot(reflect(-lightB, nor), -rd), 0.0), 24.0);
        float rim   = pow(1.0 - abs(dot(nor, -rd)), 3.0);

        vec3 baseCol = mix(ICE_BLUE, GLACIER, hash(hitID + 5.0));
        col = baseCol * (diffA + diffB + 0.1)
            + ARCTIC  * specA * 1.5
            + ICE_BLUE * specB
            + GLACIER  * rim * 1.2;
        col *= hdrBoost * aud;

        // White-hot facet edges via fwidth on distance
        float distToEdge = abs(sceneSDF(p, N, t).x);
        float edgeAA = smoothstep(fwidth(distToEdge) * 3.0, 0.0, distToEdge - 0.004);
        col += WHITE_HOT * edgeAA * edgeGlow * aud;
    }

    // Ambient crystal glow halos
    for (int i = 0; i < 9; i++) {
        if (float(i) >= N) break;
        vec3 nuc = crystalNucleus(float(i), N, t);
        float tP = dot(nuc - camPos, rd);
        if (tP > 0.0 && tP < dist - 0.05) {
            vec3 cl = camPos + rd * tP;
            float d2 = length(cl - nuc);
            float g  = exp(-d2 * d2 * 8.0) * 0.3 * (0.7 + 0.3 * sin(t * 1.5 + float(i)));
            col += GLACIER * g * hdrBoost * aud;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
