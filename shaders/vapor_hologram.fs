/*{
    "DESCRIPTION": "Japanese Onsen Night — steaming hot spring, torii gate, sakura, mountain silhouettes at dusk",
    "CREDIT": "ShaderClaw auto-improve 2026-05-06",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        {"NAME":"speed",      "LABEL":"Animation","TYPE":"float","DEFAULT":0.3, "MIN":0.0,"MAX":2.0},
        {"NAME":"hdrPeak",    "LABEL":"HDR Peak", "TYPE":"float","DEFAULT":2.5, "MIN":0.5,"MAX":4.0},
        {"NAME":"audioReact", "LABEL":"Audio React","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0},
        {"NAME":"steamDens",  "LABEL":"Steam",    "TYPE":"float","DEFAULT":0.7, "MIN":0.0,"MAX":2.0}
    ]
}*/

float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

float noise21(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21(i), hash21(i + vec2(1,0)), f.x),
               mix(hash21(i + vec2(0,1)), hash21(i + vec2(1,1)), f.x), f.y);
}

// Mountain silhouette: returns 1 above ridge, 0 below
float mountain(vec2 uv, float baseY, float amp, float freq, float seed) {
    float ridge = baseY + amp * (
        noise21(vec2(uv.x * freq + seed, 0.0)) * 0.6 +
        noise21(vec2(uv.x * freq * 2.3 + seed + 1.7, 0.0)) * 0.25 +
        noise21(vec2(uv.x * freq * 5.1 + seed + 3.1, 0.0)) * 0.12
    );
    return smoothstep(ridge, ridge + 0.005, uv.y);
}

// Steam wisp: vertical column with noise drift
float steam(vec2 uv, float cx, float t, float dens) {
    float dx = uv.x - cx;
    float phase = uv.y * 4.0 - t * 1.2;
    float drift = noise21(vec2(phase * 0.7, t * 0.4)) * 0.06 - 0.03;
    float dist = abs(dx - drift);
    float height = smoothstep(0.38, 0.22, uv.y); // only above water line
    float flicker = 0.6 + 0.4 * noise21(vec2(cx * 7.0, t * 1.8));
    return exp(-dist * dist * 800.0) * height * flicker * dens;
}

// Cherry blossom petal: small bloom at position
float blossom(vec2 uv, vec2 center, float t, float seed) {
    center.x += sin(t * 0.3 + seed * 6.28) * 0.04;
    center.y -= fract(t * (0.03 + seed * 0.02) + seed) * 0.8 + 0.1;
    if (center.y < 0.0) return 0.0;
    float d = length(uv - center);
    return exp(-d * d * 4000.0);
}

void main() {
    vec2  uv  = isf_FragNormCoord;
    float t   = TIME * speed;
    float aud = 1.0 + (audioLevel * 0.5 + audioBass * 0.7) * audioReact;

    // Sky gradient (deep indigo to dark cobalt)
    vec3 skyTop  = vec3(0.04, 0.03, 0.18);
    vec3 skyHor  = vec3(0.12, 0.06, 0.28);
    vec3 col     = mix(skyTop, skyHor, uv.y * 0.7);

    // Stars
    for (int si = 0; si < 40; si++) {
        float f   = float(si);
        float sx  = hash11(f * 1.37);
        float sy  = 0.35 + hash11(f * 2.91) * 0.6;
        float twinkle = 0.5 + 0.5 * sin(t * (1.5 + hash11(f * 4.1)) + f);
        float d   = length(uv - vec2(sx, sy));
        col += vec3(0.9, 0.85, 1.0) * exp(-d * d * 80000.0) * twinkle * hdrPeak * 0.6;
    }

    // Far mountain range (deep indigo)
    float farMtn = mountain(uv, 0.55, 0.15, 2.8, 3.14);
    col = mix(vec3(0.06, 0.04, 0.14), col, farMtn);

    // Near mountain range (black silhouette)
    float nearMtn = mountain(uv, 0.42, 0.22, 1.9, 1.61);
    col = mix(vec3(0.02, 0.01, 0.04), col, nearMtn);

    // Torii gate (2D SDF)
    vec2 uvc = uv - vec2(0.5, 0.0);
    float pillarL = max(abs(uvc.x + 0.09) - 0.008, -(uv.y - 0.52));
    float pillarR = max(abs(uvc.x - 0.09) - 0.008, -(uv.y - 0.52));
    float pillarB = max(uv.y - 0.38, 0.0);
    float pillar  = smoothstep(0.002, 0.0, min(pillarL, pillarR) - pillarB);
    float beam1   = smoothstep(0.003, 0.0, max(abs(uv.y - 0.395) - 0.012, abs(uvc.x) - 0.12));
    float beam2   = smoothstep(0.003, 0.0, max(abs(uv.y - 0.375) - 0.006, abs(uvc.x) - 0.105));
    float torii   = max(pillar, max(beam1, beam2));
    float toriiGlow = exp(-abs(uvc.x) * 8.0) * exp(-pow(uv.y - 0.39, 2.0) * 50.0) * 0.4;
    col += vec3(1.0, 0.55, 0.1) * toriiGlow * hdrPeak * aud;
    col = mix(col, vec3(0.3, 0.06, 0.01), torii * 0.9);

    // Water surface
    float waterY = 0.32;
    if (uv.y < waterY) {
        float shimmer = sin(uv.x * 22.0 + t * 1.4) * sin(uv.x * 7.0 - t * 0.9) * 0.03;
        vec3  reflect2 = mix(skyHor, skyTop, (waterY - uv.y) / waterY);
        float depthFade = (waterY - uv.y) / waterY;
        col = mix(vec3(0.03, 0.04, 0.14) + shimmer, reflect2 * 0.35, depthFade * 0.6);
        float reflX = abs(uv.x - 0.5);
        col += vec3(1.0, 0.5, 0.1) * exp(-reflX * reflX * 20.0)
             * exp(-pow(uv.y - waterY + 0.04, 2.0) * 200.0) * hdrPeak * aud * 0.5;
    }

    // Lantern glow (amber HDR)
    float lanternD = length(uv - vec2(0.5, 0.36));
    col += vec3(1.0, 0.58, 0.1) * exp(-lanternD * lanternD * 40.0) * hdrPeak * aud * 0.7;

    // Steam wisps
    float s = 0.0;
    s += steam(uv, 0.38, t, steamDens);
    s += steam(uv, 0.50, t * 0.9, steamDens);
    s += steam(uv, 0.62, t * 1.1, steamDens);
    col += vec3(0.75, 0.80, 0.90) * s * 0.6;

    // Cherry blossoms
    for (int bi = 0; bi < 10; bi++) {
        float f  = float(bi);
        float bx = hash11(f * 1.13 + 0.5);
        float by = 0.3 + hash11(f * 3.7 + 0.2) * 0.4;
        float b  = blossom(uv, vec2(bx, by), t, hash11(f * 2.3));
        col += vec3(1.0, 0.25, 0.50) * b * hdrPeak * aud;
    }

    // Moon
    float moonD = length(uv - vec2(0.78, 0.72));
    col += vec3(0.9, 0.88, 0.75) * smoothstep(0.04, 0.035, moonD) * hdrPeak * 0.8;
    col += vec3(0.7, 0.65, 0.55) * exp(-moonD * moonD * 80.0) * 0.4;

    gl_FragColor = vec4(col, 1.0);
}
