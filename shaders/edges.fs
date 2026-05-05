/*{
    "DESCRIPTION": "Neon Grid City — 3D raymarched night cityscape of SDF boxes with glowing neon edge outlines, wet street reflections, and audio-reactive glow. Camera drifts forward through a street corridor.",
    "CREDIT": "ShaderClaw3 / edges v2",
    "ISFVSN": "2",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        {
            "NAME": "driftSpeed",
            "LABEL": "Drift Speed",
            "TYPE": "float",
            "DEFAULT": 0.35,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "neonBrightness",
            "LABEL": "Neon Brightness",
            "TYPE": "float",
            "DEFAULT": 2.5,
            "MIN": 0.0,
            "MAX": 4.0
        },
        {
            "NAME": "buildingDensity",
            "LABEL": "Building Density",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.3,
            "MAX": 2.5
        },
        {
            "NAME": "cameraHeight",
            "LABEL": "Camera Height",
            "TYPE": "float",
            "DEFAULT": 0.55,
            "MIN": 0.0,
            "MAX": 3.0
        },
        {
            "NAME": "audioMod",
            "LABEL": "Audio Reactivity",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.0,
            "MAX": 2.0
        }
    ]
}*/

// ── Constants ─────────────────────────────────────────────────────────────────
#define MAX_STEPS  64
#define SURF_DIST  0.005
#define MAX_DIST   60.0
#define PI         3.14159265358979

// ── Hash / random utilities ───────────────────────────────────────────────────
float hash11(float n)       { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2  p)       { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash12(float a, float b) { return hash21(vec2(a, b)); }

// ── 5-color neon palette (fully saturated HDR) ────────────────────────────────
// 0=cyan  1=magenta  2=lime  3=gold  4=violet
vec3 neonColor(int idx) {
    if (idx == 0) return vec3(0.10, 2.50, 2.20);   // electric cyan
    if (idx == 1) return vec3(2.50, 0.10, 1.80);   // hot magenta
    if (idx == 2) return vec3(0.30, 2.50, 0.10);   // lime green
    if (idx == 3) return vec3(2.50, 1.80, 0.00);   // gold
                  return vec3(1.50, 0.10, 2.50);   // deep violet
}

// Pick a neon color from grid position hash
vec3 buildingNeon(vec2 gridID) {
    float h = hash21(gridID);
    int idx = int(h * 5.0) % 5;
    return neonColor(idx);
}

// ── City SDF ──────────────────────────────────────────────────────────────────
// Returns (distance, neonColor, materialID)
// materialID: 0=sky, 1=building, 2=street

struct HitInfo {
    float dist;
    vec3  neon;
    int   matID;  // 1=building, 2=street
};

// SDF for a single box with half-extents e, centered at origin
float sdBox(vec3 p, vec3 e) {
    vec3 q = abs(p) - e;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Flat infinite ground plane at y=0
float sdGround(vec3 p) { return p.y; }

// Building grid: buildings in a repeating XZ grid, varying heights by hash
// Returns (sdf, neon) for the city block layer
vec2 cityGrid(vec3 p, float density) {
    // Cell size driven by density parameter
    float cellW = 4.0 / density;
    float gap   = 1.2 / density;   // street corridor half-width

    // Tile XZ
    vec2  cell  = floor(vec2(p.x, p.z) / cellW);
    vec2  local = mod(vec2(p.x, p.z), cellW) - cellW * 0.5;

    float bHalfW = (cellW * 0.5 - gap);  // building footprint half-size

    // Building height from hash (varies per cell)
    float h1 = hash21(cell);
    float h2 = hash21(cell + vec2(7.3, 2.9));
    float bHeight = 3.0 + h1 * 14.0;    // 3..17 units tall
    float bDepth  = bHalfW * (0.5 + h2 * 0.5);  // varies depth

    // Box SDF — building block
    vec3  bCenter = vec3(0.0, bHeight * 0.5, 0.0);
    vec3  bExt    = vec3(bHalfW, bHeight * 0.5, bDepth);
    float bDist   = sdBox(p - vec3(local.x, 0.0, local.y) - bCenter, bExt);

    // Adjacent cell to get the "other side" buildings (corridor)
    vec2  cell2 = floor(vec2(p.x - cellW * 0.5, p.z) / cellW);
    vec2  loc2  = mod(vec2(p.x - cellW * 0.5, p.z), cellW) - cellW * 0.5;
    float h3    = hash21(cell2 + vec2(3.1, 8.7));
    float h4    = hash21(cell2 + vec2(11.2, 5.4));
    float bH2   = 3.0 + h3 * 14.0;
    float bD2   = bHalfW * (0.5 + h4 * 0.5);
    vec3  bC2   = vec3(0.0, bH2 * 0.5, 0.0);
    vec3  bE2   = vec3(bHalfW, bH2 * 0.5, bD2);
    float bD2F  = sdBox(p - vec3(loc2.x, 0.0, loc2.y) - bC2, bE2);

    float best  = min(bDist, bD2F);
    return vec2(best, 0.0);  // neon picked separately per cell
}

// Full scene SDF — returns (dist, matID)
//   matID encoding: 0=miss, 1=building, 2=street
vec2 sceneSDF(vec3 p, float density) {
    // Street / ground
    float gDist = sdGround(p);

    // City buildings
    vec2  city  = cityGrid(p, density);
    float bDist = city.x;

    if (bDist < gDist) return vec2(bDist, 1.0);
    return vec2(gDist, 2.0);
}

// ── Normal ────────────────────────────────────────────────────────────────────
vec3 calcNormal(vec3 p, float density) {
    const vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        sceneSDF(p + e.xyy, density).x - sceneSDF(p - e.xyy, density).x,
        sceneSDF(p + e.yxy, density).x - sceneSDF(p - e.yxy, density).x,
        sceneSDF(p + e.yyx, density).x - sceneSDF(p - e.yyx, density).x
    ));
}

// ── Raymarcher — returns (dist, matID) ───────────────────────────────────────
vec2 march(vec3 ro, vec3 rd, float density) {
    float t = 0.1;
    float mat = 0.0;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec2  r = sceneSDF(ro + rd * t, density);
        float d = r.x;
        mat     = r.y;
        if (d < SURF_DIST) return vec2(t, mat);
        if (t > MAX_DIST)  return vec2(-1.0, 0.0);
        t += d * 0.9;
    }
    return vec2(-1.0, 0.0);
}

// ── Neon edge glow on building surfaces ──────────────────────────────────────
// Uses fwidth on the building SDF to detect edges, then assigns HDR neon color.
vec3 buildingEdgeGlow(vec3 p, vec3 n, float density, float glowStr) {
    // Local grid cell of this surface point
    float cellW = 4.0 / density;
    vec2  cell  = floor(vec2(p.x, p.z) / cellW);
    vec3  neon  = buildingNeon(cell);

    // Edge mask: the SDF gradient magnitude at the surface encodes edge proximity.
    // We compute how curved the surface is (second derivative proxy) by comparing
    // the normal at nearby points — sharp edges have rapidly changing normals.
    vec2 e2 = vec2(0.08, 0.0);
    vec3 n1 = calcNormal(p + e2.xyy, density);
    vec3 n2 = calcNormal(p + e2.yxy, density);
    float edginess = 1.0 - clamp(dot(n, n1) * dot(n, n2), 0.0, 1.0);

    // Building body is pure black; edges get the neon color
    return neon * pow(edginess, 0.6) * glowStr;
}

// ── Star field (sparse dots) ──────────────────────────────────────────────────
float starField(vec3 rd) {
    // Project ray direction to a 2D sky grid
    vec2  sky  = vec2(atan(rd.x, rd.z), asin(clamp(rd.y, -1.0, 1.0)));
    vec2  id   = floor(sky * 28.0);
    float h    = hash21(id);
    float h2   = hash21(id + vec2(5.3, 2.1));
    // Only show stars in the upper hemisphere, sparse
    float star = (h > 0.96 && rd.y > 0.05) ? pow(h2, 8.0) * 1.8 : 0.0;
    return star;
}

void main() {
    // ── Pixel UV (centered, aspect-correct) ──────────────────────────────────
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;

    // ── Audio: modulator pattern ──────────────────────────────────────────────
    float aLvl  = max(0.0, audioLevel);
    float aMult = 1.0 + aLvl * audioMod;

    // ── Neon brightness (audio-modulated) ────────────────────────────────────
    float glowStr = neonBrightness * aMult;

    // ── Camera: street level, drifting forward along Z (TIME-driven) ─────────
    float camZ  = TIME * driftSpeed;            // drift forward forever
    vec3  ro    = vec3(0.0, cameraHeight, camZ);
    // Slow sinusoidal sway left-right for cinematics
    ro.x += sin(TIME * 0.18) * 0.4;

    // Look slightly ahead and very slightly upward
    vec3  target = vec3(sin(TIME * 0.09) * 0.3,
                        cameraHeight * 0.85,
                        camZ + 10.0);
    vec3  fwd    = normalize(target - ro);
    vec3  right  = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3  up     = cross(right, fwd);
    vec3  rd     = normalize(fwd * 1.5 + right * uv.x + up * uv.y);

    // ── Primary ray march ─────────────────────────────────────────────────────
    vec2  hitR   = march(ro, rd, buildingDensity);
    float hitT   = hitR.x;
    int   matID  = int(hitR.y + 0.5);
    bool  hit    = (hitT > 0.0);
    vec3  pHit   = ro + rd * hitT;

    // ── Sky color: pure black with stars ─────────────────────────────────────
    vec3 skyCol = vec3(starField(rd));

    // ── Surface shading ───────────────────────────────────────────────────────
    vec3 surfCol = vec3(0.0);
    if (hit) {
        vec3 n = calcNormal(pHit, buildingDensity);

        if (matID == 1) {
            // Building: pure black body + neon edge glow
            surfCol = buildingEdgeGlow(pHit, n, buildingDensity, glowStr);
        } else {
            // Street: dark wet asphalt
            // Subtle grid lines (pavement)
            vec2  pv  = fract(pHit.xz * 0.5) - 0.5;
            float grid= smoothstep(0.48, 0.45, max(abs(pv.x), abs(pv.y)));
            surfCol = vec3(0.02, 0.02, 0.025) * (0.5 + grid * 0.5);
        }
    }

    // ── Wet street reflection ─────────────────────────────────────────────────
    vec3 reflCol = vec3(0.0);
    if (hit && matID == 2) {
        // Reflect ray off ground (normal = (0,1,0) for flat street)
        vec3 reflDir = reflect(rd, vec3(0.0, 1.0, 0.0));
        vec2 reflR   = march(pHit + vec3(0.0, SURF_DIST * 3.0, 0.0), reflDir, buildingDensity);
        float reflT  = reflR.x;
        int   reflMat = int(reflR.y + 0.5);

        if (reflT > 0.0 && reflMat == 1) {
            vec3 rp  = pHit + reflDir * reflT;
            vec3 rn  = calcNormal(rp, buildingDensity);
            reflCol  = buildingEdgeGlow(rp, rn, buildingDensity, glowStr * 0.45);
        } else {
            // Star reflection on wet ground
            reflCol = vec3(starField(reflDir)) * 0.3;
        }
        // Wet asphalt Fresnel: more reflective at glancing angles
        float viewDot  = abs(dot(rd, vec3(0.0, 1.0, 0.0)));
        float wetFresn = pow(1.0 - viewDot, 2.5) * 0.9 + 0.1;
        surfCol = mix(surfCol, surfCol + reflCol * wetFresn, wetFresn);
    }

    // ── Atmospheric neon glow in the air (volumetric scatter) ────────────────
    // A cheap fog-like scattering: sample a few points along the ray and check
    // proximity to building edges, accumulating colour. Gives the "neon bleed
    // into foggy night air" look without full volumetrics.
    vec3 airGlow = vec3(0.0);
    {
        float maxT  = hit ? min(hitT, 30.0) : 30.0;
        int   NS    = 10;
        for (int si = 0; si < NS; si++) {
            float sT  = maxT * (float(si) + 0.5) / float(NS);
            vec3  sp  = ro + rd * sT;

            // Only compute near buildings (within ~1 unit of any building face)
            vec2  sr  = sceneSDF(sp, buildingDensity);
            float sd  = sr.x;
            if (sd < 0.0 || sd > 1.5) continue;

            // Which cell's neon bleeds here?
            float cellW = 4.0 / buildingDensity;
            vec2  cell  = floor(vec2(sp.x, sp.z) / cellW);
            vec3  neon  = buildingNeon(cell);

            // Glow falls off exponentially with distance from surface
            float falloff = exp(-sd * 3.5) * (1.0 / float(NS));
            airGlow += neon * falloff * glowStr * 0.18;
        }
    }

    // ── Depth fog: deep black beyond ~20 units ────────────────────────────────
    float fogAmt = hit ? clamp((hitT - 8.0) / 40.0, 0.0, 1.0) : 1.0;
    vec3  fogCol = vec3(0.0);  // fog is black (night city darkness)

    // ── Compose ───────────────────────────────────────────────────────────────
    vec3 col = hit ? surfCol : skyCol;
    col      = mix(col, fogCol, fogAmt * 0.75);   // depth fade
    col     += airGlow;                            // additive neon air scatter

    // Output linear HDR — NO tone-map, NO clamp, NO ACES
    gl_FragColor = vec4(col, 1.0);
}
