/*{
  "DESCRIPTION": "Holo Stage Hologram — Pass 0: 3D raymarched floating platform with holographic projector cone, dark industrial aesthetic, teal/cyan/magenta palette. Pass 1: holographic glitch channel — vertical tear, RGB shift, EMI bursts, scanlines.",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "CREDIT": "ShaderClaw — holo stage + hologram glitch",
  "INPUTS": [
    { "NAME": "horizonY",          "LABEL": "Horizon",          "TYPE": "float", "MIN": 0.40, "MAX": 0.75, "DEFAULT": 0.55 },
    { "NAME": "skyTopColor",       "LABEL": "Sky Top",          "TYPE": "color", "DEFAULT": [1.0, 0.42, 0.71, 1.0] },
    { "NAME": "skyHorizonColor",   "LABEL": "Sky Horizon",      "TYPE": "color", "DEFAULT": [0.36, 0.85, 0.76, 1.0] },
    { "NAME": "sunSize",           "LABEL": "Sun Size",         "TYPE": "float", "MIN": 0.05, "MAX": 0.40, "DEFAULT": 0.22 },
    { "NAME": "sunBars",           "LABEL": "Sun Bars",         "TYPE": "float", "MIN": 0.0,  "MAX": 12.0, "DEFAULT": 6.0 },
    { "NAME": "gridDensity",       "LABEL": "Grid Density",     "TYPE": "float", "MIN": 4.0,  "MAX": 24.0, "DEFAULT": 12.0 },
    { "NAME": "gridPersp",         "LABEL": "Grid Perspective", "TYPE": "float", "MIN": 0.5,  "MAX": 4.0,  "DEFAULT": 1.8 },
    { "NAME": "gridSpeed",         "LABEL": "Grid Speed",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25 },
    { "NAME": "y2kCount",          "LABEL": "Y2K Object Count", "TYPE": "float", "MIN": 0.0,  "MAX": 20.0, "DEFAULT": 12.0 },
    { "NAME": "y2kSpeed",          "LABEL": "Y2K Speed",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6 },
    { "NAME": "y2kSize",           "LABEL": "Y2K Size",         "TYPE": "float", "MIN": 0.02, "MAX": 0.20, "DEFAULT": 0.07 },
    { "NAME": "y2kChaos",          "LABEL": "Chaos",            "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.7 },
    { "NAME": "katakanaIntensity", "LABEL": "Katakana",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6 },
    { "NAME": "vaporPosterize",    "LABEL": "Vapor Posterize",  "TYPE": "float", "MIN": 1.0,  "MAX": 32.0, "DEFAULT": 16.0 },
    { "NAME": "holoChroma",        "LABEL": "Holo Chroma",      "TYPE": "float", "MIN": 0.0,  "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "holoScanFreq",      "LABEL": "Holo Scanlines",   "TYPE": "float", "MIN": 1.0,  "MAX": 4.0,  "DEFAULT": 2.0 },
    { "NAME": "holoTear",          "LABEL": "Tear Probability", "TYPE": "float", "MIN": 0.0,  "MAX": 0.3,  "DEFAULT": 0.06 },
    { "NAME": "holoBreak",         "LABEL": "EMI Break",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.3 },
    { "NAME": "holoGlow",          "LABEL": "Holo Glow",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.7 },
    { "NAME": "holoTint",          "LABEL": "Hologram Tint",    "TYPE": "color", "DEFAULT": [0.55, 1.0, 0.95, 1.0] },
    { "NAME": "holoMix",           "LABEL": "Hologram Mix",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.85 },
    { "NAME": "audioReact",        "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "orbitSpeed",        "LABEL": "Orbit Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.15 },
    { "NAME": "inputTex",          "LABEL": "Texture (optional GIF source)", "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "vapor" },
    {}
  ]
}*/

// ══════════════════════════════════════════════════════════════════════════════
// Shared utilities
// ══════════════════════════════════════════════════════════════════════════════
float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

// ══════════════════════════════════════════════════════════════════════════════
// PASS 0 — "Holo Stage": 3D raymarched floating platform with holographic
//           projector cone. Dark industrial, teal/cyan/magenta palette.
// ══════════════════════════════════════════════════════════════════════════════

// ── Palette (fully saturated, HDR peaks ≥ 2.0, no white mixing) ─────────────
//   Dark charcoal body:     vec3(0.08, 0.08, 0.12)
//   Grid lines — e-cyan:    vec3(0.20, 2.50, 2.00)  HDR
//   Pillar tops — magenta:  vec3(2.50, 0.10, 1.80)  HDR
//   Projector cone — teal:  vec3(0.50, 2.00, 2.50)  HDR
//   Background — void black: vec3(0.0)

// ── 3D SDF primitives ────────────────────────────────────────────────────────
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float sdCylinder(vec3 p, float r, float h) {
    vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(r, h);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

// Cone pointing upward from p==0 toward +y, apex at y=h, base radius r at y=0
float sdCone(vec3 p, float r, float h) {
    vec2 q  = vec2(length(p.xz), p.y);
    vec2 k1 = vec2(r, 0.0);
    vec2 k2 = vec2(r - 0.0, h);   // slope direction
    // slope: tip at (0, h), base at (r, 0)
    float t  = clamp((dot(q - k1, k2 - k1)) / dot(k2 - k1, k2 - k1), 0.0, 1.0);
    vec2  nr = q - mix(k1, k2, t);
    float d1 = length(nr) * sign(max(nr.x, -nr.y));
    float d2 = q.y - h;
    float d3 = -q.y;
    return max(d1, max(d3, d2));   // inside cone when all negative
}

// ── Scene material IDs ───────────────────────────────────────────────────────
#define MAT_BG       0
#define MAT_PLATFORM 1    // dark charcoal
#define MAT_GRID     2    // electric cyan HDR
#define MAT_PILLAR   3    // dark charcoal body
#define MAT_PILLAR_T 4    // hot magenta tips
#define MAT_PROJ     5    // projector body (dark)
#define MAT_CONE     6    // teal-white glow HDR

struct Hit3 {
    float d;
    int   mat;
};

Hit3 minHit3(Hit3 a, Hit3 b) { return (a.d < b.d) ? a : b; }

// Audio-modulated cone brightness: 1.0 + audioLevel * audioReact * factor
// Applied at shading time, not inside the SDF.

Hit3 holoStageSDF(vec3 p) {
    Hit3 res;
    res.d   = 1e9;
    res.mat = MAT_BG;

    // Floating platform: 2×0.15×2 box, sits at y = 0.0 (top face)
    float platform = sdBox(p - vec3(0.0, -0.075, 0.0), vec3(1.0, 0.075, 1.0));
    Hit3 hPlatform; hPlatform.d = platform; hPlatform.mat = MAT_PLATFORM;
    res = minHit3(res, hPlatform);

    // Platform grid: overlay thin cyan lines via floor-modulo pattern
    // (handled in shading, not SDF — we flag the platform face as MAT_GRID
    //  when the grid pattern fires)

    // 4 corner pillars: thin cylinders at ±0.8 on xz, height 0.4 above platform top
    vec2 corners[4];
    corners[0] = vec2( 0.80,  0.80);
    corners[1] = vec2(-0.80,  0.80);
    corners[2] = vec2( 0.80, -0.80);
    corners[3] = vec2(-0.80, -0.80);

    for (int ci = 0; ci < 4; ci++) {
        vec3 pc = p - vec3(corners[ci].x, 0.20, corners[ci].y);
        float pillar = sdCylinder(pc, 0.045, 0.20);
        Hit3 hP; hP.d = pillar; hP.mat = MAT_PILLAR;
        res = minHit3(res, hP);

        // Pillar top cap: small cylinder, magenta
        vec3 pct = p - vec3(corners[ci].x, 0.42, corners[ci].y);
        float cap = sdCylinder(pct, 0.065, 0.025);
        Hit3 hCap; hCap.d = cap; hCap.mat = MAT_PILLAR_T;
        res = minHit3(res, hCap);
    }

    // Central projector: cylinder at origin, sits on top of platform
    vec3 pProj = p - vec3(0.0, 0.125, 0.0);
    float proj = sdCylinder(pProj, 0.10, 0.125);
    Hit3 hProj; hProj.d = proj; hProj.mat = MAT_PROJ;
    res = minHit3(res, hProj);

    // Holographic cone above projector: apex at y=1.0, base r=0.55 at y=0.25
    vec3 pCone = p - vec3(0.0, 0.25, 0.0);
    float cone = sdCone(pCone, 0.55, 0.75);
    Hit3 hCone; hCone.d = cone; hCone.mat = MAT_CONE;
    res = minHit3(res, hCone);

    // Floor plane at y = -0.5 (keep scene bounded below)
    Hit3 hFloor; hFloor.d = p.y + 0.50; hFloor.mat = MAT_BG;
    res = minHit3(res, hFloor);

    return res;
}

// ── Normal via tetrahedron FD ─────────────────────────────────────────────────
vec3 holoNormal(vec3 p) {
    vec2 e = vec2(0.002, -0.002);
    return normalize(
        e.xyy * holoStageSDF(p + e.xyy).d +
        e.yyx * holoStageSDF(p + e.yyx).d +
        e.yxy * holoStageSDF(p + e.yxy).d +
        e.xxx * holoStageSDF(p + e.xxx).d
    );
}

// ── Material colour ───────────────────────────────────────────────────────────
vec3 holoMatColor(int mat, vec3 worldPos, float coneMod) {
    if (mat == MAT_PLATFORM) {
        // Grid lines on the top face via fmod — electric cyan HDR lines on charcoal
        vec2  gp   = worldPos.xz * gridDensity * 0.5;
        vec2  gc   = abs(fract(gp) - 0.5);
        float line = 1.0 - smoothstep(0.42, 0.50, max(gc.x, gc.y));
        float gridGlow = 1.0 + audioLevel * audioReact * 1.2;   // audio modulator
        vec3  charcoal = vec3(0.08, 0.08, 0.12);
        vec3  cyan     = vec3(0.20, 2.50, 2.00) * gridGlow;
        return mix(charcoal, cyan, line * 0.85);
    }
    if (mat == MAT_PILLAR)   return vec3(0.10, 0.10, 0.16);           // dark body
    if (mat == MAT_PILLAR_T) return vec3(2.50, 0.10, 1.80);           // hot magenta (HDR)
    if (mat == MAT_PROJ)     return vec3(0.12, 0.12, 0.18);           // projector body
    if (mat == MAT_CONE)     return vec3(0.50, 2.00, 2.50) * coneMod; // teal glow (HDR)
    return vec3(0.0);   // void black background / floor
}

// ── Pass 0 entry ──────────────────────────────────────────────────────────────
vec4 passHoloStage(vec2 fragCoord) {
    vec2 uv = (fragCoord - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Camera orbit: slow rotation around the platform, slight downward angle
    float orb   = TIME * orbitSpeed;
    float camD  = 3.0;
    float camEl = 0.40;   // radians above horizontal

    vec3 ro = vec3(
        cos(orb) * cos(camEl) * camD,
        sin(camEl) * camD,
        sin(orb) * cos(camEl) * camD
    );
    vec3 target = vec3(0.0, 0.20, 0.0);
    vec3 fwd    = normalize(target - ro);
    vec3 right  = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up     = cross(right, fwd);
    vec3 rd     = normalize(fwd * 1.6 + right * uv.x + up * uv.y);

    // Audio modulators (modulator not gate)
    float coneMod = 1.0 + audioLevel * audioReact * 1.5;   // projector brightness

    // Raymarching — 64 steps
    float totalDist = 0.0;
    bool  hit       = false;
    vec3  hitPos;
    Hit3  hi;
    hi.d   = 1e9;
    hi.mat = MAT_BG;

    for (int s = 0; s < 64; s++) {
        hitPos = ro + rd * totalDist;
        hi     = holoStageSDF(hitPos);

        if (hi.d < 0.003) { hit = true; break; }
        if (totalDist > 12.0) break;
        totalDist += hi.d * 0.80;
    }

    // ── Background: void black ────────────────────────────────────────────────
    vec3 col = vec3(0.0);

    // Subtle ambient cone glow in the background (additive volumetric halo)
    // Sample the cone's distance along the ray's closest approach
    {
        float nearCone = 1e9;
        float tP = 0.0;
        for (int k = 0; k < 12; k++) {
            vec3 pp = ro + rd * tP;
            // Only the cone geometry matters for halo
            vec3 pConeP = pp - vec3(0.0, 0.25, 0.0);
            float dc = sdCone(pConeP, 0.55, 0.75);
            nearCone = min(nearCone, dc);
            tP += max(dc, 0.04);
            if (tP > 12.0) break;
        }
        float coneHalo = exp(-max(nearCone, 0.0) * 3.0) * coneMod * 0.40;
        col += vec3(0.15, 0.60, 0.75) * coneHalo;
    }

    // ── Surface shading ───────────────────────────────────────────────────────
    if (hit && hi.mat != MAT_BG) {
        vec3 n = holoNormal(hitPos);
        vec3 v = normalize(ro - hitPos);

        // Key light: overhead-right; fill: from camera side
        vec3  L1    = normalize(vec3(1.5, 3.0, 1.0));
        float diff  = max(dot(n, L1), 0.0);
        float amb   = 0.15;
        float spec  = pow(max(dot(reflect(-L1, n), v), 0.0), 48.0);
        float rim   = pow(1.0 - max(dot(n, v), 0.0), 3.0);

        vec3 baseC = holoMatColor(hi.mat, hitPos, coneMod);

        // Diffuse + ambient
        col = baseC * (diff * 0.65 + amb);

        // Rim light in teal — echoes the cone glow colour, gives depth
        col += vec3(0.20, 1.60, 2.00) * rim * 0.35;

        // Specular — HDR white so bloom catches it
        col += vec3(2.0, 2.0, 2.5) * spec * 0.4;

        // Cone interior: additive inner glow, audio-driven
        if (hi.mat == MAT_CONE) {
            float innerGlow = exp(-max(hi.d, 0.0) * 8.0) * coneMod;
            col += vec3(0.20, 1.80, 2.50) * innerGlow * 0.60;
        }

        // Depth fade
        float depthFade = 1.0 - smoothstep(8.0, 12.0, totalDist);
        col *= depthFade;
    }

    // Posterize (same as the original pass so pass 1 glitch has quantized data)
    if (vaporPosterize > 1.0) col = floor(col * vaporPosterize) / vaporPosterize;

    return vec4(col, 1.0);
}

// ══════════════════════════════════════════════════════════════════════════════
// PASS 1 — Hologram glitch over the holo-stage buffer
//           THIS CODE IS KEPT EXACTLY AS-IS FROM THE ORIGINAL
// ══════════════════════════════════════════════════════════════════════════════
vec4 passHologram(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;

    // Vertical tear — band-shifted bands of vapor.
    float bandH = 0.04;
    float bandY = floor(uv.y / bandH) * bandH;
    float tearTrig = step(1.0 - holoTear * (1.0 + audioBass * audioReact),
                          hash21(vec2(bandY, floor(TIME * 8.0))));
    uv.x += tearTrig * (hash21(vec2(bandY, TIME)) - 0.5) * 0.15;

    // RGB chromatic shift on the vapor buffer
    float ch = holoChroma * (1.0 + audioHigh * audioReact);
    float r = texture(vapor, clamp(uv + vec2( ch, 0.0), 0.0, 1.0)).r;
    float g = texture(vapor, clamp(uv,                  0.0, 1.0)).g;
    float b = texture(vapor, clamp(uv - vec2( ch, 0.0), 0.0, 1.0)).b;
    vec3 holo = vec3(r, g, b) * holoTint.rgb;

    // Scanlines (resolution-aware)
    holo *= 0.85 + 0.15 * sin(gl_FragCoord.y * holoScanFreq * 0.5);

    // EMI break: rare bursts replace fragments with hash noise
    float breakTrig = step(0.9, hash21(vec2(floor(TIME * 4.0), 0.0)));
    holo = mix(holo, vec3(hash21(uv * TIME)),
               holoBreak * audioBass * audioReact * 0.4 * breakTrig);

    // Mid-band flicker
    float flicker = 0.92 + 0.08 * sin(TIME * 60.0
                  + hash21(vec2(floor(TIME * 30.0))) * 6.28);
    holo *= mix(1.0, flicker, audioMid * audioReact * 0.5);

    // Edge bloom — bright pixels glow beyond their position
    float lum = dot(holo, vec3(0.299, 0.587, 0.114));
    holo += holoTint.rgb * pow(lum, 1.4) * holoGlow * 0.3;

    // Transmission strength — low audio dims the hologram (signal weakens)
    holo *= 0.5 + audioLevel * 0.6;

    // Mix: 0 = pure vapor, 1 = full hologram
    vec3 vapor_ = texture(vapor, fragCoord / RENDERSIZE.xy).rgb;
    return vec4(mix(vapor_, holo, holoMix), 1.0);
}

// ══════════════════════════════════════════════════════════════════════════════
void main() {
    if (PASSINDEX == 0) FragColor = passHoloStage(gl_FragCoord.xy);
    else                FragColor = passHologram(gl_FragCoord.xy);
}
