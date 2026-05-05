/*{
  "DESCRIPTION": "Aurora Volumetrica — volumetric 3D aurora borealis: layered colored curtains of light rippling across a dark polar sky.",
  "CREDIT": "ShaderClaw auto-improve v7",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "auroraSpeed",  "LABEL": "Aurora Speed",  "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "curtainCount", "LABEL": "Curtains",      "TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0, "MAX": 8.0 },
    { "NAME": "brightness",   "LABEL": "Brightness",    "TYPE": "float", "DEFAULT": 2.2,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "starDensity",  "LABEL": "Stars",         "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioMod",     "LABEL": "Audio Mod",     "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define PI   3.14159265
#define TAU  6.28318530

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash12(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float noise2(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(i),           hash12(i + vec2(1.0, 0.0)), u.x),
               mix(hash12(i + vec2(0.0, 1.0)), hash12(i + vec2(1.0, 1.0)), u.x), u.y);
}

// Procedural star field
float stars(vec3 rd, float density) {
    vec3 d = normalize(rd);
    float sx = floor(d.x * 80.0 + 0.5) / 80.0;
    float sy = floor(d.y * 80.0 + 0.5) / 80.0;
    float sz = floor(d.z * 60.0 + 0.5) / 60.0;
    float h = hash12(vec2(sx + sz * 3.7, sy + sz * 5.3));
    float h2 = hash12(vec2(sx + sz * 7.1 + 1.0, sy + sz * 2.9 + 1.0));
    float bright = step(1.0 - density * 0.012, h);
    return bright * (0.3 + h2 * 0.7);
}

// Single aurora curtain: column of volumetric light
// uv.x = east-west, altitude tracked by ray parameter
float auroraCurtain(vec2 xz, float altitude, float seed, float t) {
    float phase = t * auroraSpeed + seed;
    // Horizontal wave position
    float cx = sin(altitude * 0.8 + phase * 0.7) * 0.6
             + sin(altitude * 1.3 - phase * 1.1) * 0.3;
    cx += seed * 1.3;
    // Width ripple
    float width = 0.12 + 0.06 * sin(altitude * 2.0 + phase * 1.5 + seed);
    float dist = abs(xz.x - cx) - width;
    // Soft curtain edge
    return smoothstep(0.18, 0.0, max(dist, 0.0))
         * smoothstep(-0.4, 0.2, xz.y - altitude + 0.5)   // bottom fade
         * smoothstep(3.5, 1.0, altitude)                  // top fade
         * (0.5 + 0.5 * noise2(vec2(altitude * 1.7 + seed, t * 0.5 + seed)));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.4;
    float t = TIME;

    // Camera: looking slightly up from ground level, slowly panning
    float camT = t * 0.035;
    vec3 ro = vec3(sin(camT) * 0.5, 0.1, 0.0);
    vec3 target = vec3(sin(camT * 0.6) * 0.4, 0.55, 1.0);
    vec3 fwd = normalize(target - ro);
    vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 upV = cross(fwd, rgt);
    vec3 rd  = normalize(fwd + uv.x * rgt * 0.85 + uv.y * upV * 0.75);

    // Polar night sky
    float nightGrad = smoothstep(-0.1, 1.0, rd.y);
    vec3 col = mix(vec3(0.0, 0.01, 0.03), vec3(0.0, 0.0, 0.01), nightGrad);

    // Stars
    float starB = stars(rd, starDensity);
    col += vec3(0.85, 0.90, 1.0) * starB * smoothstep(0.05, 0.3, rd.y);

    // Aurora: march along ray through curtain layers
    // Model aurora as thin vertical bands at various altitudes
    if (rd.y > 0.02) {
        // Ray-plane intersection for a set of altitude planes
        vec3 accAurora = vec3(0.0);
        float nb = floor(clamp(curtainCount, 1.0, 8.0));

        // 4-color aurora palette (fully saturated): violet, cyan, gold, green
        vec3 palette[4];
        palette[0] = vec3(0.55, 0.0,  1.0);  // violet
        palette[1] = vec3(0.0,  1.0,  0.8);  // cyan
        palette[2] = vec3(1.0,  0.75, 0.0);  // gold
        palette[3] = vec3(0.1,  1.0,  0.2);  // green

        for (int ci = 0; ci < 8; ci++) {
            if (float(ci) >= nb) break;
            float fi = float(ci);
            float seed = fi * 5.73;

            // March through altitude range 0.5–2.5 in steps
            for (int ai = 0; ai < 12; ai++) {
                float altitude = 0.5 + float(ai) * 0.17;
                // Ray intersects horizontal slab at this altitude
                float rayT = (altitude - ro.y) / max(rd.y, 0.001);
                vec3 p = ro + rd * rayT;
                float density = auroraCurtain(p.xz, altitude, seed, t);
                vec3 curtainCol = palette[ci & 3];
                accAurora += curtainCol * density * 0.08;
            }
        }

        // Blend accumulated aurora over sky
        float auroraLum = max(accAurora.r, max(accAurora.g, accAurora.b));
        col = mix(col, accAurora * brightness * audio, min(auroraLum * 3.0, 1.0));
        col += accAurora * brightness * audio * 0.4; // additive glow
    }

    // Ground: dark snow with slight aurora reflection
    if (rd.y < 0.02) {
        float groundT = (0.0 - ro.y) / min(rd.y, -0.001);
        float snowGlow = exp(-groundT * 0.3) * 0.05;
        col += vec3(0.3, 0.6, 1.0) * snowGlow * brightness * audio;
    }

    FragColor = vec4(col, 1.0);
}
