/*{
  "DESCRIPTION": "Gravity Streams — orbiting particles with glowing trails, deferred lighting, and texture-mapped streams",
  "CATEGORIES": ["Generator", "Simulation"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "particleSize", "LABEL": "Particle Size", "TYPE": "float", "DEFAULT": 12.0, "MIN": 2.0, "MAX": 64.0 },
    { "NAME": "orbitSpeed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "orbitChaos", "LABEL": "Chaos", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "fadeRate", "LABEL": "Trail Fade", "TYPE": "float", "DEFAULT": 0.002, "MIN": 0.0, "MAX": 0.05 },
    { "NAME": "glossiness", "LABEL": "Glossiness", "TYPE": "float", "DEFAULT": 120.0, "MIN": 4.0, "MAX": 256.0 },
    { "NAME": "specular", "LABEL": "Specular", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "surfaceHeight", "LABEL": "Surface Depth", "TYPE": "float", "DEFAULT": 384.0, "MIN": 0.0, "MAX": 800.0 },
    { "NAME": "glowAmount", "LABEL": "Glow", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "vignette", "LABEL": "Vignette", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "scrollSpeed", "LABEL": "Camera Scroll", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "texScale", "LABEL": "Tex Scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ],
  "PASSES": [
    { "TARGET": "albedoBuf", "PERSISTENT": true },
    { "TARGET": "normalBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Gravity Streams — analytical orbits (no persistent particle buffer needed)
// Particle positions computed from TIME using multi-frequency Lissajous orbits
// with gravitational clustering. Trails + deferred lighting in persistent buffers.

#define N_PARTICLES 12
#define PI 3.14159265

float hash(vec2 co) {
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

// Compute particle position analytically from time
// Uses layered sinusoidal orbits that simulate gravitational interaction
vec2 particlePos(int id, float t) {
    float fi = float(id);
    float seed1 = hash(vec2(fi * 7.13, 1.0));
    float seed2 = hash(vec2(fi * 3.71, 2.0));
    float seed3 = hash(vec2(fi * 11.37, 3.0));
    float seed4 = hash(vec2(fi * 5.91, 4.0));

    // Base orbit — each particle has unique frequency + phase
    float baseFreq = 0.3 + seed1 * 0.4;
    float basePhase = seed2 * PI * 2.0;
    float baseRadius = 0.12 + seed3 * 0.25;

    // Secondary wobble (simulates gravitational perturbation)
    float wobbleFreq = 0.7 + seed4 * 1.3;
    float wobbleAmt = 0.03 + seed1 * 0.08;

    // Gravitational clustering — particles share common attractor points
    // Two attractors orbiting slowly
    float a1x = sin(t * 0.13) * 0.15;
    float a1y = cos(t * 0.17) * 0.12;
    float a2x = sin(t * 0.11 + PI) * 0.12;
    float a2y = cos(t * 0.14 + PI) * 0.15;

    // Each particle orbits around one of the attractors (alternating)
    float attract = mod(fi, 2.0) < 0.5 ? 1.0 : 0.0;
    float ax = mix(a1x, a2x, attract);
    float ay = mix(a1y, a2y, attract);

    // Occasionally swap attractor allegiance (creates stream crossings)
    float swapPhase = sin(t * 0.08 + fi * 0.7);
    if (swapPhase > 0.3) {
        ax = mix(ax, a2x, smoothstep(0.3, 0.8, swapPhase));
        ay = mix(ay, a2y, smoothstep(0.3, 0.8, swapPhase));
    }

    float wobbleScale = wobbleAmt * (1.0 + orbitChaos * 2.0);

    float x = 0.5 + ax
        + cos(t * baseFreq + basePhase) * baseRadius
        + sin(t * wobbleFreq + seed3 * 5.0) * wobbleScale;

    float y = 0.5 + ay
        + sin(t * baseFreq * 0.8 + basePhase + 1.57) * baseRadius * 0.7
        + cos(t * wobbleFreq * 0.9 + seed1 * 5.0) * wobbleScale;

    // Keep in bounds
    x = clamp(x, 0.02, 0.98);
    y = clamp(y, 0.02, 0.98);

    return vec2(x, y) * RENDERSIZE;
}

vec2 texUV(vec2 px) {
    vec2 st = px / RENDERSIZE;
    float ar = RENDERSIZE.x / RENDERSIZE.y;
    float tar = IMG_SIZE_inputTex.x / max(IMG_SIZE_inputTex.y, 1.0);
    vec2 c = st - 0.5;
    float r = ar / max(tar, 0.001);
    if (r > 1.0) c.x *= r; else c.y /= r;
    c /= texScale;
    return fract(c + 0.5);
}

void main() {
    vec2 Res = RENDERSIZE;
    vec2 pos = gl_FragCoord.xy;
    vec2 uv = isf_FragNormCoord;
    float pSize = particleSize;
    float t = TIME * orbitSpeed;

    // ===== PASS 0: Albedo (trails) =====
    // Interpolate between previous and current position to draw continuous trails
    if (PASSINDEX == 0) {
        vec2 scrollUV = (pos + vec2(scrollSpeed, 0.0) + 0.5) / Res;
        vec4 col = texture2D(albedoBuf, scrollUV);
        col.a *= (1.0 - fadeRate);

        bool hasTex = IMG_SIZE_inputTex.x > 0.0;
        // Use real frame delta with slight overshoot to prevent gaps
        float dt = TIMEDELTA * orbitSpeed * 1.2;

        for (int i = 0; i < N_PARTICLES; i++) {
            vec2 ppCur = particlePos(i, t);
            vec2 ppPrev = particlePos(i, t - dt);

            vec3 pc;
            if (hasTex) {
                // Sample texture at midpoint for stable color along trail
                vec2 ppMid = (ppCur + ppPrev) * 0.5;
                pc = texture2D(inputTex, texUV(ppMid)).rgb;
            } else {
                float fi = float(i);
                pc = normalize(vec3(0.1) +
                    vec3(hash(vec2(fi, 0.1) + TIME * 0.01),
                         hash(vec2(fi, 1.7) + TIME * 0.01),
                         hash(vec2(fi, 3.1) + TIME * 0.01)));
            }

            // Capsule SDF: distance from pixel to line segment (prev → cur)
            vec2 seg = ppCur - ppPrev;
            float segLen = length(seg);
            float dist;
            if (segLen < 0.5) {
                dist = distance(pos, ppCur);
            } else {
                vec2 toPos = pos - ppPrev;
                float proj = clamp(dot(toPos, seg) / (segLen * segLen), 0.0, 1.0);
                dist = distance(pos, ppPrev + seg * proj);
            }

            float a = smoothstep(pSize, pSize * 0.4, dist);
            col = mix(col, vec4(pc, 1.0), a);
        }

        gl_FragColor = col;
        return;
    }

    // ===== PASS 1: Normals (trails) =====
    if (PASSINDEX == 1) {
        vec2 scrollUV = (pos + vec2(scrollSpeed, 0.0) + 0.5) / Res;
        vec4 nrm = texture2D(normalBuf, scrollUV);
        float dt = TIMEDELTA * orbitSpeed * 1.2;

        for (int i = 0; i < N_PARTICLES; i++) {
            vec2 ppCur = particlePos(i, t);
            vec2 ppPrev = particlePos(i, t - dt);

            vec2 seg = ppCur - ppPrev;
            float segLen = length(seg);
            vec2 closest;
            if (segLen < 0.5) {
                closest = ppCur;
            } else {
                vec2 toPos = pos - ppPrev;
                float proj = clamp(dot(toPos, seg) / (segLen * segLen), 0.0, 1.0);
                closest = ppPrev + seg * proj;
            }

            vec2 v = pos - closest;
            float l = length(v);
            float a = smoothstep(pSize, pSize * 0.4, l);
            float z = sqrt(abs(pSize * pSize - l * l));
            nrm = mix(nrm, vec4(normalize(vec3(v, z)), 1.0), a);
        }

        gl_FragColor = nrm;
        return;
    }

    // ===== PASS 2: Compositing =====
    vec4 albedo = texture2D(albedoBuf, uv);
    vec3 rawN = texture2D(normalBuf, uv).xyz;
    vec3 normal = length(rawN) > 0.01 ? normalize(rawN) : vec3(0.0, 0.0, 1.0);

    // Simple directional light on the trail surface
    vec3 lDir = normalize(vec3(1.0, 2.0, 1.5));
    float nDot = clamp(dot(normal, lDir), 0.0, 1.0);
    float diff = mix(0.3, 1.0, nDot);
    float spec = pow(clamp(dot(reflect(-lDir, normal), vec3(0,0,1)), 0.0, 1.0), glossiness) * specular;

    // Lit trail color
    vec3 result = albedo.rgb * diff + vec3(spec * 0.3);

    // Mix with background based on trail alpha
    result = mix(bgColor.rgb, result, min(albedo.a * 1.2, 1.0));

    // Particle glow — small colored point light per particle
    for (int i = 0; i < N_PARTICLES; i++) {
        vec2 pp = particlePos(i, t);
        vec3 pc = texture2D(albedoBuf, pp / Res).rgb;
        if (dot(pc, pc) < 0.01) pc = vec3(0.5);
        float dist = distance(pp, pos);
        // Tight glow: bright core fading quickly
        float core = smoothstep(pSize * 0.8, 0.0, dist);
        float halo = smoothstep(pSize * 2.5, pSize * 0.5, dist) * 0.3;
        result += pc * (core + halo) * glowAmount * 0.4;
    }

    // Vignette
    if (vignette > 0.001) {
        vec2 co = (uv - 0.5) * (Res.x / Res.y) * 2.0;
        float rf = length(co) * 0.25 * vignette;
        float rf2 = rf * rf + 1.0;
        result *= pow(1.0 / (rf2 * rf2), 2.24);
    }

    // Gamma
    result = pow(max(result, vec3(0.0)), vec3(1.0 / 2.2));

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.01, 0.1, albedo.a);

    gl_FragColor = vec4(result, alpha);
}
