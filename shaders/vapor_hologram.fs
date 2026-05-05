/*{
  "DESCRIPTION": "Moonlit Desert — cool-toned night desert raymarched in 3D. Moonlit sand dunes, mountain silhouettes, bright moon with corona, scattered cacti. Single-pass.",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "moonSize",    "LABEL": "Moon Size",     "TYPE": "float", "MIN": 0.05, "MAX": 0.30, "DEFAULT": 0.15 },
    { "NAME": "moonHeight",  "LABEL": "Moon Height",   "TYPE": "float", "MIN": 0.40, "MAX": 0.90, "DEFAULT": 0.72 },
    { "NAME": "duneFreq",    "LABEL": "Dune Frequency","TYPE": "float", "MIN": 1.0,  "MAX": 8.0,  "DEFAULT": 3.0  },
    { "NAME": "duneAmp",     "LABEL": "Dune Amplitude","TYPE": "float", "MIN": 0.05, "MAX": 0.40, "DEFAULT": 0.18 },
    { "NAME": "cactusCount", "LABEL": "Cactus Count",  "TYPE": "float", "MIN": 0.0,  "MAX": 8.0,  "DEFAULT": 5.0  },
    { "NAME": "starDensity", "LABEL": "Star Density",  "TYPE": "float", "MIN": 0.5,  "MAX": 3.0,  "DEFAULT": 1.5  },
    { "NAME": "audioReact",  "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5  }
  ]
}*/

// ── Hashing ───────────────────────────────────────────────────────────────
float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

// ── Dune height field ─────────────────────────────────────────────────────
float duneHeight(vec2 xz) {
    return ( sin(xz.x * duneFreq * 0.7) * cos(xz.z * duneFreq * 0.5)
           + sin(xz.x * duneFreq * 1.3 + 0.7) * sin(xz.z * duneFreq * 0.9) )
           * duneAmp;
}

// ── Terrain march — sphere-trace height field ─────────────────────────────
float terrainMarch(vec3 ro, vec3 rd) {
    float t = 0.5;
    for (int i = 0; i < 48; i++) {
        vec3 p = ro + rd * t;
        float h = duneHeight(p.xz) - p.y;  // positive = below ground
        if (h > 0.0) return t;
        t += max(abs(h) * 0.4, 0.02);
        if (t > 20.0) break;
    }
    return -1.0;
}

// ── Terrain normal via finite diff ────────────────────────────────────────
vec3 terrainNormal(vec3 p) {
    float eps = 0.01;
    float h0  = duneHeight(p.xz);
    float hx  = duneHeight(p.xz + vec2(eps, 0.0));
    float hz  = duneHeight(p.xz + vec2(0.0, eps));
    return normalize(vec3(h0 - hx, eps, h0 - hz));
}

// ── Cactus SDF: trunk + two arms ─────────────────────────────────────────
float sdCapsule2D(vec2 p, float halfH, float r) {
    p.y = p.y - clamp(p.y, 0.0, halfH * 2.0);
    return length(p) - r;
}

float sdCactus(vec3 p, float seed) {
    float trunkH = 0.5 + hash11(seed) * 0.4;
    // Restrict to above ground
    if (p.y < -0.02 || p.y > trunkH + 0.25) return 1e9;

    // Trunk: vertical cylinder approximated as capsule
    float trunk = length(p.xz) - 0.04;
    float trunkSDF = max(trunk, max(-p.y, p.y - trunkH));

    // Arm 1: right side
    float arm1y = 0.25 + hash11(seed * 2.1) * 0.15;
    vec3 arm1p = p - vec3(0.0, arm1y, 0.0);
    // arm goes +x then up
    float arm1H = 0.12;
    vec3 armA1 = vec3(0.0, arm1y, 0.0);
    vec3 armA2 = vec3(0.15, arm1y, 0.0);
    vec3 armA3 = vec3(0.15, arm1y + arm1H, 0.0);
    // capsule A1->A2
    vec3 ab = armA2 - armA1;
    float tA = clamp(dot(p - armA1, ab) / dot(ab, ab), 0.0, 1.0);
    float segH = length(p - armA1 - ab * tA) - 0.035;
    // capsule A2->A3
    vec3 ab2 = armA3 - armA2;
    float tB = clamp(dot(p - armA2, ab2) / dot(ab2, ab2), 0.0, 1.0);
    float segV = length(p - armA2 - ab2 * tB) - 0.035;
    float arm1SDF = min(segH, segV);

    // Arm 2: left side (mirrored)
    vec3 brmA1 = vec3(0.0,  arm1y * 0.9, 0.0);
    vec3 brmA2 = vec3(-0.15, arm1y * 0.9, 0.0);
    vec3 brmA3 = vec3(-0.15, arm1y * 0.9 + arm1H * 0.9, 0.0);
    vec3 cb = brmA2 - brmA1;
    float tC = clamp(dot(p - brmA1, cb) / dot(cb, cb), 0.0, 1.0);
    float segH2 = length(p - brmA1 - cb * tC) - 0.035;
    vec3 db = brmA3 - brmA2;
    float tD = clamp(dot(p - brmA2, db) / dot(db, db), 0.0, 1.0);
    float segV2 = length(p - brmA2 - db * tD) - 0.035;
    float arm2SDF = min(segH2, segV2);

    return min(trunkSDF, min(arm1SDF, arm2SDF));
}

// ── Mountain silhouette (far background) ─────────────────────────────────
float mountainProfile(float x) {
    // Low-frequency bumpy silhouette
    return 0.12 * (sin(x * 1.3) * 0.4 + sin(x * 2.7 + 1.0) * 0.3
                 + sin(x * 5.1 + 2.4) * 0.15 + sin(x * 0.4 + 0.5) * 0.4);
}

// ── Star field ────────────────────────────────────────────────────────────
float starField(vec2 rd2, float density) {
    // Tile the sky into cells, place a star in each
    vec2 cell = floor(rd2 * 200.0 * density);
    vec2 frac = fract(rd2 * 200.0 * density);
    float seed = hash21(cell);
    // Place star offset within cell
    vec2 starOff = vec2(hash21(cell + 0.1), hash21(cell + 0.2));
    float dist = length(frac - starOff);
    float size = 0.003 * density * (0.5 + 0.5 * hash21(cell + 0.3));
    float twinkle = 0.7 + 0.3 * sin(TIME * (2.0 + hash21(cell + 0.4) * 4.0) + seed * 6.28);
    return smoothstep(size * 1.5, size * 0.3, dist) * twinkle * step(0.7, seed);
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = fragCoord / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Screen-space coordinate centered
    vec2 sc = (uv * 2.0 - 1.0) * vec2(aspect, 1.0);

    // ── Camera — fixed position looking slightly downward ─────────────
    vec3 ro = vec3(0.0, 0.5, -2.0);
    vec3 target = vec3(0.0, 0.18, 0.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd + sc.x * right * 0.6 + sc.y * up * 0.6);

    // ── Palette ───────────────────────────────────────────────────────
    vec3 colNightSky  = vec3(0.0,   0.01,  0.04);
    vec3 colStarSilv  = vec3(1.8,   2.0,   2.5);
    vec3 colMoonCor   = vec3(2.5,   2.8,   3.0);
    vec3 colMoonLitSd = vec3(0.6,   0.7,   0.9);
    vec3 colDuneShadow= vec3(0.01,  0.02,  0.05);
    vec3 colCactus    = vec3(0.0,   0.0,   0.0);

    // ── Audio-reactive parameters ─────────────────────────────────────
    float effMoonSize = moonSize * (1.0 + audioLevel * audioReact * 0.1);
    float starBoost   = 1.0 + audioBass * audioReact * 0.2;

    // ── Sky base color ────────────────────────────────────────────────
    // Horizon is slightly lighter navy, zenith is deep black-blue
    float skyT = clamp(uv.y * 1.4, 0.0, 1.0);
    vec3 col = mix(vec3(0.0, 0.015, 0.06), colNightSky, skyT);

    // ── Stars ─────────────────────────────────────────────────────────
    // Project ray direction to 2D for star tiling
    vec2 stUV = vec2(atan(rd.x, rd.z) / 6.28318 + 0.5, rd.y * 0.5 + 0.5);
    float starV = starField(stUV, starDensity) * starBoost;
    col += colStarSilv * starV * clamp(rd.y * 2.0, 0.0, 1.0);

    // ── Moon ──────────────────────────────────────────────────────────
    // Moon is placed in 2D screen space for simplicity
    vec2 moonCenter = vec2(0.0, moonHeight * 2.0 - 1.0);
    float moonD = length(sc - moonCenter) - effMoonSize;

    // Anti-aliased moon edge
    float fw = fwidth(moonD);
    float moonMask = 1.0 - smoothstep(-fw, fw, moonD);

    // Corona: soft glow ring outside moon
    float coronaR = smoothstep(effMoonSize * 3.0, 0.0, abs(moonD)) * (1.0 - moonMask);
    col += colMoonCor * coronaR * 0.7;

    // Moon body: subtle horizontal band texture (like faint surface detail)
    if (moonMask > 0.01) {
        vec2 mLocal = (sc - moonCenter) / effMoonSize; // [-1,1] within moon
        float bandTex = 0.92 + 0.08 * sin(mLocal.y * 8.0 + 0.3);
        // Limb darkening: edges slightly dimmer
        float limbD = 1.0 - dot(mLocal, mLocal);
        vec3 moonCol = colMoonCor * bandTex * sqrt(max(limbD, 0.0));
        col = mix(col, moonCol, moonMask);
    }

    // ── Distant mountain silhouettes ──────────────────────────────────
    // Project to screen y vs a mountain height profile at a fixed "distance"
    float mtProfile = mountainProfile(sc.x * 0.7 + 0.3);
    float mtY = -0.55 + mtProfile;  // baseline just below horizon
    // Mountains only visible in lower portion of sky
    if (sc.y < mtY + 0.02 && sc.y > mtY - 0.12) {
        float mtMask = smoothstep(mtY + 0.015, mtY - 0.005, sc.y);
        col = mix(col, vec3(0.005, 0.008, 0.02), mtMask);
    }

    // ── Terrain raymarching ───────────────────────────────────────────
    float tHit = terrainMarch(ro, rd);
    bool onTerrain = tHit > 0.0;
    bool cactusHit = false;

    if (onTerrain) {
        vec3 hitP = ro + rd * tHit;

        // Dune normal for lighting
        vec3 norm = terrainNormal(hitP);

        // Moonlight direction (from upper-left where moon is)
        vec3 moonDir = normalize(vec3(-0.3, 0.9, 0.5));
        float diff = max(dot(norm, moonDir), 0.0);

        // Moonlit sand color
        vec3 sandLit  = colMoonLitSd * (0.15 + diff * 0.85);
        vec3 sandShad = colDuneShadow;
        vec3 sand = mix(sandShad, sandLit, diff * diff);

        // Subtle secondary bounce light from opposite side
        float fill = max(dot(norm, -moonDir * vec3(1,0,1)), 0.0) * 0.08;
        sand += vec3(0.0, 0.02, 0.06) * fill;

        col = sand;

        // ── Cacti on terrain ─────────────────────────────────────────
        int NC = int(clamp(cactusCount, 0.0, 8.0));
        for (int ci = 0; ci < 8; ci++) {
            if (ci >= NC) break;
            float fi = float(ci);
            // Place cacti at fixed random xz positions
            float cx = (hash11(fi * 7.31) - 0.5) * 3.0;
            float cz = hash11(fi * 3.17) * 4.0 + 0.5;
            float cy = duneHeight(vec2(cx, cz));
            vec3 cBase = vec3(cx, cy, cz);
            // March along ray to find cactus
            float tc = 0.2;
            for (int si = 0; si < 32; si++) {
                vec3 cp = ro + rd * tc;
                vec3 lp = cp - cBase;
                float dc = sdCactus(lp, fi);
                if (dc < 0.005) {
                    cactusHit = true;
                    break;
                }
                tc += max(dc * 0.5, 0.01);
                if (tc > 20.0) break;
            }
            if (cactusHit) break;
        }

        if (cactusHit) col = colCactus;
    }

    // ── Below terrain background fallback ─────────────────────────────
    // If ray never hit terrain (pointing too steep), show deep ground color
    if (!onTerrain && rd.y < 0.0) {
        col = colDuneShadow * 0.5;
    }

    gl_FragColor = vec4(col, 1.0);
}
