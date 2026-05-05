/*{
  "DESCRIPTION": "Bioluminescent Jellyfish — 3D raymarched jellyfish with dome bell and undulating tentacles in deep ocean",
  "CATEGORIES": ["Generator", "3D"],
  "CREDIT": "ShaderClaw auto-improve",
  "INPUTS": [
    { "NAME": "jellyfishCount", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 5.0, "LABEL": "Jellyfish" },
    { "NAME": "glowIntensity", "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 4.0, "LABEL": "Bioluminescence" },
    { "NAME": "tentacleLength", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.2, "MAX": 2.0, "LABEL": "Tentacle Length" },
    { "NAME": "audioReact", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio" }
  ]
}*/

// -----------------------------------------------------------------------
// Bioluminescent Jellyfish — 3D raymarched ocean scene
// Single-pass ISF. HDR linear output.
// -----------------------------------------------------------------------

#define PI  3.14159265358979
#define TAU 6.28318530718

// ---- hash / noise -------------------------------------------------------

float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

// ---- SDF primitives -----------------------------------------------------

// Upside-down hemisphere: dome open at bottom
float sdBell(vec3 p, float r) {
    float sphere = length(p) - r;
    float cutY   = p.y + r * 0.3; // keep top dome only
    return max(sphere, -cutY);
}

// Rounded capsule between two points
float sdCapsule(vec3 p, vec3 a, vec3 b, float radius) {
    vec3 ab = b - a;
    float t  = clamp(dot(p - a, ab) / dot(ab, ab), 0.0, 1.0);
    return length(p - (a + t * ab)) - radius;
}

// Smooth union
float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

// ---- Per-jellyfish SDF + material ---------------------------------------

// Returns vec2(distance, material-id)
// material IDs: 1.0 = bell core, 2.0 = bell rim, 3.0 = tentacle, 4.0 = inner glow
vec2 evalJellyfish(vec3 p, vec3 origin, float seed, float audioBoost) {
    float t = TIME;

    // Bell pulse: radius oscillates, audio makes it faster + larger
    float pulseSpeed  = 0.8 + audioBoost * 0.6;
    float pulseAmp    = 0.04 + audioBoost * 0.03;
    float bellR       = 0.28 + sin(t * pulseSpeed + seed * TAU) * pulseAmp;

    // Gentle drift: jellyfish floats upward slowly, bobs laterally
    vec3 drift = vec3(
        sin(t * 0.13 + seed * 5.7) * 0.12,
        mod(t * 0.05 + seed * 3.3, 4.0) - 2.0,
        sin(t * 0.09 + seed * 2.1) * 0.08
    );

    vec3 lp = p - origin - drift;

    // Bell dome (upside-down hemisphere, open at bottom)
    float dBell = sdBell(lp, bellR);

    // Inner glow sphere (smaller, white-hot core)
    float dCore = length(lp) - bellR * 0.45;

    // Tentacles: 7 capsules hanging from rim
    float dTentacles = 1e6;
    for (int ti = 0; ti < 7; ti++) {
        float fi    = float(ti);
        float angle = fi / 7.0 * TAU + seed * 1.3;

        float rimR = bellR * 0.85;
        vec3 rimPt = vec3(cos(angle) * rimR, -bellR * 0.3, sin(angle) * rimR);

        // Sinusoidal undulation per tentacle
        float swayT    = t * (0.5 + fi * 0.07) + fi * 0.8;
        float swayAmp  = 0.08 + audioBoost * 0.04;
        vec2 sway      = vec2(sin(swayT) * swayAmp, sin(swayT * 0.7 + fi) * swayAmp * 0.6);

        float tLen = tentacleLength * (0.6 + hash11(fi + seed * 7.0) * 0.4);
        vec3 tipPt = rimPt + vec3(sway.x, -tLen, sway.y);

        float radius = max(0.018 - fi * 0.001, 0.008);
        float dc = sdCapsule(lp, rimPt, tipPt, radius);
        dTentacles = min(dTentacles, dc);
    }

    // Smooth-union bell + tentacles
    float dAll = smin(dBell, dTentacles, 0.04);

    // Material selection
    float mat = 3.0; // default: tentacle
    if (dBell < dTentacles + 0.02) {
        mat = 2.0; // bell rim
        if (dCore < 0.0)               mat = 4.0; // inside core = inner glow
        else if (length(lp) < bellR * 0.55) mat = 1.0; // bell core
    }
    if (dCore < dAll) {
        dAll = smin(dAll, dCore, 0.02);
    }

    return vec2(dAll, mat);
}

// ---- Full scene SDF ------------------------------------------------------

vec2 sceneMap(vec3 p, float audioBoost) {
    vec2 result = vec2(1e6, 0.0);

    vec3 pos0 = vec3(-0.45, 0.15, 2.2);
    vec3 pos1 = vec3( 0.30, -0.25, 2.8);
    vec3 pos2 = vec3( 0.10,  0.40, 1.7);
    vec3 pos3 = vec3(-0.60, -0.50, 3.4);
    vec3 pos4 = vec3( 0.55,  0.05, 3.0);

    vec3 positions[5];
    positions[0] = pos0;
    positions[1] = pos1;
    positions[2] = pos2;
    positions[3] = pos3;
    positions[4] = pos4;

    int nJelly = int(clamp(jellyfishCount, 1.0, 5.0));
    for (int j = 0; j < 5; j++) {
        if (j >= nJelly) break;
        float seed = float(j) * 1.618;
        vec2 hit = evalJellyfish(p, positions[j], seed, audioBoost);
        if (hit.x < result.x) result = hit;
    }

    return result;
}

// ---- Material color (HDR) ------------------------------------------------

vec3 materialColor(float matId, float audioBoost) {
    float pulse = 1.0 + sin(TIME * 1.6) * 0.15 + audioBoost * 0.3;

    if (matId > 3.5) {
        // white-hot inner glow — HDR 3.0
        return vec3(1.0, 1.0, 1.0) * 3.0 * pulse * glowIntensity;
    } else if (matId > 2.5) {
        // violet tentacles — HDR 1.5
        return vec3(0.5, 0.0, 1.0) * 1.5 * pulse * glowIntensity;
    } else if (matId > 1.5) {
        // magenta bell rim — HDR 2.0
        return vec3(1.0, 0.0, 0.8) * 2.0 * pulse * glowIntensity;
    } else {
        // electric teal bell core — HDR 2.5
        return vec3(0.0, 1.0, 0.8) * 2.5 * pulse * glowIntensity;
    }
}

// ---- Ambient volumetric glow halo ----------------------------------------

vec3 jellyfishHalo(vec3 ro, vec3 rd, float audioBoost) {
    vec3 halo = vec3(0.0);
    int nJelly = int(clamp(jellyfishCount, 1.0, 5.0));

    vec3 positions[5];
    positions[0] = vec3(-0.45, 0.15, 2.2);
    positions[1] = vec3( 0.30, -0.25, 2.8);
    positions[2] = vec3( 0.10,  0.40, 1.7);
    positions[3] = vec3(-0.60, -0.50, 3.4);
    positions[4] = vec3( 0.55,  0.05, 3.0);

    float pulse = 1.0 + sin(TIME * 1.6) * 0.15 + audioBoost * 0.3;

    for (int j = 0; j < 5; j++) {
        if (j >= nJelly) break;
        float seed = float(j) * 1.618;

        // Match drift from evalJellyfish
        vec3 drift = vec3(
            sin(TIME * 0.13 + seed * 5.7) * 0.12,
            mod(TIME * 0.05 + seed * 3.3, 4.0) - 2.0,
            sin(TIME * 0.09 + seed * 2.1) * 0.08
        );
        vec3 center = positions[j] + drift;

        // Closest approach of ray to jellyfish center
        vec3 oc = center - ro;
        float tc = dot(oc, rd);
        if (tc < 0.0) continue;
        vec3 closest = ro + rd * tc;
        float d2 = length(closest - center);

        // Large wide halo — exp falloff
        float haloRadius = 0.7;
        float falloff = exp(-d2 * d2 / (haloRadius * haloRadius));

        // Teal-dominant halo with magenta tinge
        vec3 haloColor = mix(vec3(0.0, 1.0, 0.8), vec3(1.0, 0.0, 0.8), 0.25);
        halo += haloColor * falloff * 0.35 * glowIntensity * pulse;
    }
    return halo;
}

// ---- Central-difference normal -------------------------------------------

vec3 calcNormal(vec3 p, float audioBoost) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneMap(p + e.xyy, audioBoost).x - sceneMap(p - e.xyy, audioBoost).x,
        sceneMap(p + e.yxy, audioBoost).x - sceneMap(p - e.yxy, audioBoost).x,
        sceneMap(p + e.yyx, audioBoost).x - sceneMap(p - e.yyx, audioBoost).x
    ));
}

// ---- Main ----------------------------------------------------------------

void main() {
    vec2 res = RENDERSIZE;
    vec2 uv  = (gl_FragCoord.xy - res * 0.5) / min(res.x, res.y);

    // Audio reactivity
    float audioBoost = (audioLevel + audioBass * 0.7) * audioReact;

    // Camera: looking across ocean at jellyfish from slight distance
    vec3 ro = vec3(0.0, 0.1, 0.0);
    vec3 rd = normalize(vec3(uv.x, uv.y - 0.05, 1.0));

    // Deep ocean background: near-black deep navy
    vec3 col = vec3(0.0, 0.005, 0.02);

    // Raymarch — 96 iterations for tentacle detail
    float tRay   = 0.05;
    float tHit   = -1.0;
    float matHit = 0.0;
    bool  didHit = false;

    for (int i = 0; i < 96; i++) {
        vec3  p    = ro + rd * tRay;
        vec2  res2 = sceneMap(p, audioBoost);
        float d    = res2.x;

        if (d < 0.002) {
            tHit   = tRay;
            matHit = res2.y;
            didHit = true;
            break;
        }
        tRay += max(d * 0.7, 0.005);
        if (tRay > 10.0) break;
    }

    if (didHit) {
        vec3 hitPos = ro + rd * tHit;
        vec3 nor    = calcNormal(hitPos, audioBoost);

        // fwidth() AA on SDF surface edge
        float surfD = sceneMap(hitPos, audioBoost).x;
        float ddist = fwidth(tHit);
        float edge  = smoothstep(ddist, 0.0, surfD);

        // HDR material color
        vec3 matCol = materialColor(matHit, audioBoost);

        // Fresnel: jellyfish glows brighter at grazing angles (translucent)
        float fresnel = pow(1.0 - abs(dot(nor, -rd)), 2.5);
        matCol += matCol * fresnel * 0.8;

        // Subsurface scatter: teal glow from above
        vec3 lightDir = normalize(vec3(0.2, 1.0, 0.3));
        float sss = max(0.0, dot(-nor, lightDir)) * 0.4 + 0.1;
        matCol += vec3(0.0, 1.0, 0.8) * sss * glowIntensity * 0.3;

        col = mix(col, matCol, edge);
    }

    // Volumetric glow halo (exp falloff around each jellyfish)
    col += jellyfishHalo(ro, rd, audioBoost);

    // Underwater bioluminescent sparkle particles
    float sparkle = hash21(gl_FragCoord.xy + vec2(TIME * 37.1, TIME * 19.3));
    sparkle = pow(max(sparkle - 0.985, 0.0) / 0.015, 2.0);
    col += vec3(0.4, 0.8, 1.0) * sparkle * 0.5;

    // Corner vignette for ocean depth feel
    vec2 vUV = gl_FragCoord.xy / res;
    float vig = 1.0 - smoothstep(0.5, 1.2, length((vUV - 0.5) * vec2(res.x / res.y, 1.0)));
    col *= mix(0.6, 1.0, vig);

    // Linear HDR output — no tonemapping (host handles it)
    gl_FragColor = vec4(col, 1.0);
}
