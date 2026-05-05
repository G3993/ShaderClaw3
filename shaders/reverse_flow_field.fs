/*{
  "DESCRIPTION": "Deep Ocean Current — 3D volumetric water column with bioluminescent streamlines tracing magnetic-like current paths through midnight blue abyss",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "Easel / ShaderClaw v3",
  "INPUTS": [
    { "NAME": "currentSpeed",  "LABEL": "Current Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6  },
    { "NAME": "streamCount",   "LABEL": "Stream Count",  "TYPE": "float", "MIN": 4.0, "MAX": 16.0,"DEFAULT": 10.0 },
    { "NAME": "cameraSpeed",   "LABEL": "Camera Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15 },
    { "NAME": "glowRadius",    "LABEL": "Glow Radius",   "TYPE": "float", "MIN": 0.01,"MAX": 0.2,  "DEFAULT": 0.06 },
    { "NAME": "hdrBoost",      "LABEL": "HDR Boost",     "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.3  },
    { "NAME": "audioReact",    "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0  }
  ]
}*/

precision highp float;

// ── Palette: deep ocean bioluminescence ──────────────────────────────────────
const vec3 ABYSS      = vec3(0.00, 0.00, 0.05);
const vec3 DEEP_TEAL  = vec3(0.00, 0.45, 0.70);
const vec3 BIO_CYAN   = vec3(0.00, 1.00, 0.85);
const vec3 BIO_VIOLET = vec3(0.45, 0.00, 1.00);
const vec3 WHITE_GLOW = vec3(2.00, 2.50, 3.00);

float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }

// Domain-warped flow field giving organic current paths
vec3 flowDir(vec3 p, float t) {
    float s = currentSpeed;
    float a = sin(p.x * 1.2 + p.z * 0.9 + t * s)
            + cos(p.y * 0.8 + p.z * 1.4 - t * s * 0.7);
    float b = cos(p.x * 0.9 - p.y * 1.3 + t * s * 0.5)
            + sin(p.y * 1.1 + p.x * 0.7 + t * s * 0.6);
    float c = sin(p.z * 1.4 - p.x * 0.8 + t * s * 0.8)
            + cos(p.z * 0.6 + p.y * 1.2 - t * s * 0.4);
    return normalize(vec3(a, b, c));
}

// Minimum distance from a point to a streamline (traced N steps forward)
float streamDist(vec3 p, float seedID, float N, float t) {
    float seed1 = hash(seedID);
    float seed2 = hash(seedID + 7.3);
    float seed3 = hash(seedID + 13.7);
    vec3 q = vec3(seed1 * 3.0 - 1.5, seed2 * 3.0 - 1.5, seed3 * 3.0 - 1.5);
    float minD = 1e9;
    const int STEPS = 20;
    for (int i = 0; i < STEPS; i++) {
        vec3 dir = flowDir(q, t);
        q += dir * 0.08;
        minD = min(minD, length(p - q));
    }
    return minD;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t   = TIME;
    float aud = 1.0 + (audioLevel + audioBass * 0.8) * audioReact * 0.5;
    float N   = streamCount;

    // Slow-drifting camera looking into the abyss (downward angle)
    float camAng = t * cameraSpeed;
    vec3 camPos  = vec3(cos(camAng) * 2.5, 1.5 + sin(camAng * 0.4) * 0.5, sin(camAng) * 2.5);
    vec3 target  = vec3(0.0, -0.5, 0.0);
    vec3 fwd     = normalize(target - camPos);
    vec3 right   = normalize(cross(fwd, vec3(0, 1, 0)));
    vec3 up      = cross(right, fwd);
    vec3 rd      = normalize(fwd + uv.x * right * 0.75 + uv.y * up * 0.75);

    // Volume-ray integration — march and accumulate glow from streams
    vec3 col  = ABYSS;
    vec3 accum = vec3(0.0);
    float density = 0.0;
    float stepSize = 0.15;

    for (int step = 0; step < 40; step++) {
        float d = stepSize * float(step) + 0.2;
        if (d > 7.0) break;
        vec3 p = camPos + rd * d;

        // Sample each streamline's glow contribution
        for (int si = 0; si < 16; si++) {
            if (float(si) >= N) break;
            float seedOffset = float(si) * 1.37 + floor(t * currentSpeed * 0.2);
            float sd = streamDist(p, seedOffset, N, t);
            if (sd < glowRadius * 1.5) {
                float glow = exp(-sd * sd / (glowRadius * glowRadius));
                float pulse = 0.6 + 0.4 * sin(t * 2.5 + float(si) * 1.9 + d * 2.0);
                // Alternate colors by stream index
                vec3 streamCol = (mod(float(si), 3.0) < 1.0) ? BIO_CYAN
                               : (mod(float(si), 3.0) < 2.0) ? BIO_VIOLET
                               : DEEP_TEAL;
                accum += streamCol * glow * pulse * stepSize * 0.6 * hdrBoost * aud;
                // White-hot core
                float core = exp(-sd * sd / (glowRadius * glowRadius * 0.08));
                accum += WHITE_GLOW * core * 0.08 * hdrBoost * aud;
            }
        }
    }

    // Depth fog fades to abyss
    col = ABYSS + accum;

    // Subtle bioluminescent particle sparkles (plankton)
    vec2 puvI = floor(isf_FragNormCoord * 180.0);
    float sparkle = step(0.994, hash(puvI.x + puvI.y * 317.1 + floor(t * 8.0) * 41.3));
    col += BIO_CYAN * sparkle * 0.8 * hdrBoost;

    gl_FragColor = vec4(col, 1.0);
}
