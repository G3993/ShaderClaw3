/*{
    "DESCRIPTION": "Prism Light — 3D raymarched glass prism splitting a white beam into HDR spectral fan rays. Black background with cinematic key/fill lighting and audio-reactive rotation.",
    "CREDIT": "ShaderClaw3",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        {
            "NAME": "rotSpeed",
            "LABEL": "Rotation Speed",
            "TYPE": "float",
            "DEFAULT": 0.18,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "fanSpread",
            "LABEL": "Fan Spread",
            "TYPE": "float",
            "DEFAULT": 0.60,
            "MIN": 0.1,
            "MAX": 1.5
        },
        {
            "NAME": "beamWidth",
            "LABEL": "Beam Width",
            "TYPE": "float",
            "DEFAULT": 0.09,
            "MIN": 0.01,
            "MAX": 0.35
        },
        {
            "NAME": "spectralGlow",
            "LABEL": "Spectral Glow",
            "TYPE": "float",
            "DEFAULT": 2.0,
            "MIN": 0.0,
            "MAX": 3.0
        },
        {
            "NAME": "audioMod",
            "LABEL": "Audio Reactivity",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MIN": 0.0,
            "MAX": 2.0
        }
    ]
}*/

// ── Constants ─────────────────────────────────────────────────────────────────
#define MAX_STEPS  64
#define SURF_DIST  0.0012
#define MAX_DIST   16.0
#define PI         3.14159265358979
#define TAU        6.28318530717959

// ── Rotation matrix ───────────────────────────────────────────────────────────
mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// ── Equilateral-triangle prism SDF ────────────────────────────────────────────
// Triangle extruded along Z. The inradius of a regular triangle with side L
// is r = L / (2*sqrt(3)).  We check each of the 3 half-planes and the two
// Z-caps, taking the max (intersection = solid).
float sdPrism(vec3 p, float halfH, float side) {
    // Three edge normals of an equilateral triangle, pointing outward:
    //   n0 = (0, 1, 0)
    //   n1 = (sqrt3/2, -0.5, 0)
    //   n2 = (-sqrt3/2, -0.5, 0)
    float inR = side * 0.28867513;  // side / (2*sqrt3)

    // Signed distance to each face
    float d0 =  p.y                             - inR;
    float d1 =  0.8660254 * p.x - 0.5 * p.y   - inR;
    float d2 = -0.8660254 * p.x - 0.5 * p.y   - inR;

    float dTri = max(max(d0, d1), d2);          // inside triangle
    float dCap = abs(p.z) - halfH;              // Z caps
    return max(dTri, dCap);
}

// ── Global prism angle (set per-frame before march) ──────────────────────────
float gPrismAngle;

float sceneSDF(vec3 p) {
    vec3 q = p;
    // Rotate on Y axis (tilt the prism toward camera)
    q.xz *= rot2(gPrismAngle);
    // Slight tilt on X for a more cinematic angle
    q.yz *= rot2(0.22);
    return sdPrism(q, 0.55, 0.95);
}

// ── Analytic normal via finite differences ────────────────────────────────────
vec3 calcNormal(vec3 p) {
    const vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneSDF(p + e.xyy) - sceneSDF(p - e.xyy),
        sceneSDF(p + e.yxy) - sceneSDF(p - e.yxy),
        sceneSDF(p + e.yyx) - sceneSDF(p - e.yyx)
    ));
}

// ── Raymarcher ────────────────────────────────────────────────────────────────
float march(vec3 ro, vec3 rd) {
    float t = 0.01;
    for (int i = 0; i < MAX_STEPS; i++) {
        float d = sceneSDF(ro + rd * t);
        if (d < SURF_DIST) return t;
        if (t > MAX_DIST)  return -1.0;
        t += d * 0.9;  // conservative step for accuracy
    }
    return -1.0;
}

// ── 5-band spectral palette — fully saturated HDR ────────────────────────────
// Red · Yellow · Green · Cyan-Blue · Violet
vec3 spectralColor(int idx) {
    if (idx == 0) return vec3(2.90, 0.08, 0.05);  // HDR red
    if (idx == 1) return vec3(2.50, 1.20, 0.02);  // HDR yellow-orange
    if (idx == 2) return vec3(0.08, 2.70, 0.08);  // HDR green
    if (idx == 3) return vec3(0.05, 0.60, 2.90);  // HDR blue
                  return vec3(1.40, 0.05, 2.90);  // HDR violet
}

// ── Volumetric cone ray contribution ─────────────────────────────────────────
// Integrate spectral tube along the view ray by sampling points along it.
// The tube is defined by an origin, direction, and half-angle. We use a
// Gaussian kernel on the perpendicular distance to the tube axis.
float tubeSample(vec3 sp, vec3 origin, vec3 dir, float tubeRadius) {
    vec3  v    = sp - origin;
    float proj = dot(v, dir);
    if (proj < 0.0) return 0.0;
    float perp = length(v - dir * proj);
    // Gaussian falloff — softer than hard step, gives volumetric glow
    float r2 = tubeRadius * tubeRadius + 1e-6;
    return exp(-perp * perp / r2);
}

void main() {
    // ── Pixel coordinate (centered, aspect-correct) ──────────────────────────
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;

    // ── Audio: modulator pattern (1 + level × factor) ────────────────────────
    float aLvl  = max(0.0, audioLevel);
    float aMult = 1.0 + aLvl * audioMod;

    // ── Time-driven prism rotation (always animated) ─────────────────────────
    gPrismAngle = TIME * rotSpeed * aMult;

    // ── Camera: slightly above-right, looking at origin ──────────────────────
    vec3 ro     = vec3(2.6, 1.0, 4.2);
    vec3 target = vec3(0.0, -0.05, 0.0);
    vec3 fwd    = normalize(target - ro);
    vec3 right  = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up     = cross(right, fwd);
    vec3 rd     = normalize(fwd * 1.7 + right * uv.x + up * uv.y);

    // ── Raymarch ─────────────────────────────────────────────────────────────
    float hitT  = march(ro, rd);
    bool  hit   = (hitT > 0.0);
    vec3  pHit  = ro + rd * hitT;

    // ── Spectral fan rays ─────────────────────────────────────────────────────
    // The fan originates from the prism's right exit apex (analytic fixed point).
    // Rays spread in the XY plane in the -X hemisphere (exit side).
    vec3 exitPt = vec3(0.52, -0.08, 0.0);

    float spread    = fanSpread * aMult;
    float tubeRad   = beamWidth * 0.055;  // world-space tube radius
    float glowScale = spectralGlow * aMult;

    vec3 rayAccum = vec3(0.0);

    for (int ri = 0; ri < 5; ri++) {
        float fi    = float(ri) / 4.0;                  // 0 → 1
        float angle = PI + (fi - 0.5) * spread;         // fan into -X half
        vec3  dir   = normalize(vec3(cos(angle), sin(angle) * 0.55, 0.0));

        vec3  col   = spectralColor(ri);
        float glow  = 0.0;

        // 32 volumetric samples along the view ray
        for (int si = 0; si < 32; si++) {
            float sT  = mix(0.02, MAX_DIST * 0.65, float(si) / 31.0);
            vec3  sp  = ro + rd * sT;

            // Skip if inside the prism solid (beam not yet split there)
            float pD  = sceneSDF(sp);
            if (pD < 0.0) continue;

            // Skip geometry that is behind the prism from camera
            if (hit && sT < hitT - 0.05) continue;

            float contrib = tubeSample(sp, exitPt, dir, tubeRad * (1.0 + fi * 0.3));
            glow += contrib * (1.0 / 32.0);
        }

        // Per-band tubeRad varies slightly so bands separate visibly
        rayAccum += col * glow * glowScale;
    }

    // ── Incoming white beam (enters from left along +X) ───────────────────────
    // Analytic: a thin tube centered on Y=0, Z=0, travelling from x=-3 to prism.
    float beamGlow = 0.0;
    {
        vec3 bOrig = vec3(-3.5, 0.0, 0.0);
        vec3 bDir  = vec3(1.0,  0.0, 0.0);
        float bRad = beamWidth * 0.040;

        for (int si = 0; si < 24; si++) {
            float sT = mix(0.02, MAX_DIST * 0.55, float(si) / 23.0);
            vec3  sp = ro + rd * sT;

            // Only draw beam on the entry side of the prism
            if (sp.x > 0.3) continue;

            float pD = sceneSDF(sp);
            if (pD < 0.0) continue;  // skip inside glass

            float contrib = tubeSample(sp, bOrig, bDir, bRad);
            beamGlow += contrib * (1.0 / 24.0);
        }
    }
    vec3 beamCol = vec3(2.4, 2.4, 2.5) * beamGlow * spectralGlow;

    // ── Glass prism surface shading ───────────────────────────────────────────
    vec3 glassCol = vec3(0.0);
    if (hit) {
        vec3 n = calcNormal(pHit);
        vec3 v = normalize(ro - pHit);

        // Hard white key light (upper-right-front — cinematic)
        vec3 L1   = normalize(vec3(3.5, 5.0, 3.0));
        float diff1 = max(dot(n, L1), 0.0);
        vec3 H1     = normalize(L1 + v);
        float spec1 = pow(max(dot(n, H1), 0.0), 220.0);  // tight Blinn-Phong

        // Soft blue fill from the left
        vec3 L2     = normalize(vec3(-2.5, 0.5, 1.5));
        float diff2 = max(dot(n, L2), 0.0) * 0.14;

        // Fresnel — increases at glancing angles (glass rim highlight)
        float NdotV = max(dot(n, v), 0.0);
        float fresnel = pow(1.0 - NdotV, 4.0);

        // fwidth-based edge detection for ink-black silhouette
        float sdfAtHit = sceneSDF(pHit);
        float edgeW    = fwidth(sdfAtHit) * 8.0;
        float edgeMask = smoothstep(0.0, edgeW, sdfAtHit + SURF_DIST * 3.0);

        // Glass body: very dark, illuminated only by specular + fresnel
        vec3 glassBase = vec3(0.03, 0.04, 0.08) * (diff1 * 0.35 + diff2);

        // HDR specular highlights
        vec3 specHDR    = vec3(2.6, 2.6, 2.9) * spec1;              // white key spec
        vec3 fresnelHDR = vec3(0.12, 0.22, 2.5) * fresnel * 0.8;   // blue rim

        // Internal spectral caustic: slight rainbow tint on glass body
        float causticA  = atan(n.y, n.x) + TIME * 0.25;
        vec3  caustic   = 0.5 + 0.5 * cos(vec3(0.0, 2.094, 4.188) + causticA * 3.5);
        vec3  causticCol = caustic * 0.30 * diff1 * spectralGlow * 0.35;

        glassCol = glassBase + specHDR + fresnelHDR + causticCol;

        // Ink-black edge: mask dims everything near the silhouette
        glassCol *= edgeMask;
    }

    // ── Compose: additive HDR, no tone-map ───────────────────────────────────
    vec3 col = vec3(0.0);              // Black background
    col     += rayAccum;               // Spectral fan (additive HDR)
    col     += beamCol;                // Input white beam
    if (hit) {
        // Glass prism composites on top of rays (mostly opaque body)
        col = mix(col, glassCol, 0.86);
    }

    // Output linear HDR — NO ACES, NO clamp, NO gamma
    gl_FragColor = vec4(col, 1.0);
}
