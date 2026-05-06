/*{
  "DESCRIPTION": "Hex Crystal Grid — raymarched RGB hexagonal data prisms. Replaces multi-pass frame buffer.",
  "CREDIT": "auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Abstract"],
  "INPUTS": [
    { "NAME": "gridDensity", "LABEL": "Grid Density", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 4.0 },
    { "NAME": "pulseSpeed",  "LABEL": "Pulse Speed",  "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioReact",  "LABEL": "Audio",        "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// ── Hash utilities ──────────────────────────────────────────────────────
float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}

// ── Hex grid cell repetition ─────────────────────────────────────────────
// Returns the offset of p within the nearest hex cell center
vec2 hexRepeat(vec2 p, float s) {
    vec2 r = vec2(s, s * 1.732051);
    vec2 h = r * 0.5;
    vec2 a = mod(p, r) - h;
    vec2 b = mod(p + h, r) - h;
    return (dot(a, a) < dot(b, b)) ? a : b;
}

// Returns the cell ID for a given world xz position
vec2 hexCellID(vec2 p, float s) {
    vec2 r = vec2(s, s * 1.732051);
    vec2 h = r * 0.5;
    vec2 a = mod(p, r) - h;
    vec2 b = mod(p + h, r) - h;
    vec2 local = (dot(a, a) < dot(b, b)) ? a : b;
    // ID = position of cell center
    return p - local;
}

// ── SDFs ─────────────────────────────────────────────────────────────────
// Hexagonal prism SDF
// p = local position, h.x = hex radius, h.y = half height
float sdHexPrism(vec3 p, vec2 h) {
    vec3 q = abs(p);
    return max(q.z - h.y, max(q.x * 0.866025 + q.y * 0.5, q.y) - h.x);
}

// Floor plane
float sdFloor(vec3 p) {
    return p.y + 0.02;
}

// ── Scene ────────────────────────────────────────────────────────────────
// Returns vec3(dist, colorID, towerT)
// colorID: 0=floor, 1=R tower, 2=G tower, 3=B tower
// towerT: 0..1 normalized height within tower (for top glow)
vec3 sceneSDF(vec3 p) {
    float audioMod = 1.0 + audioBass * audioReact * 0.35;

    // Hex grid cell size driven by gridDensity
    float cellSize = 1.0 / gridDensity;

    // Domain repetition in xz
    vec2 cellOff = hexRepeat(p.xz, cellSize);
    vec2 cellID  = hexCellID(p.xz, cellSize);

    // Per-cell hash for height variety and color
    float h     = hash21(cellID * 0.137 + 3.7);
    float h2    = hash21(cellID * 0.251 + 1.3);

    // Tower height oscillates per-cell
    float towerH = (0.3 + h * 0.7) * audioMod;
    float pulse  = sin(TIME * pulseSpeed * 3.14159 + h2 * 6.28318) * 0.3;
    towerH = towerH + pulse * audioMod;
    towerH = max(towerH, 0.05);

    // Hex prism inner radius (slightly smaller than cell for gap)
    float hexR = cellSize * 0.42;

    // Local position within cell
    vec3 lp = vec3(cellOff.x, p.y - towerH, cellOff.y);

    // SDF of this tower (prism from y=0..towerH*2, centered at y=towerH)
    float halfH = towerH;
    float dist  = sdHexPrism(lp, vec2(hexR, halfH));

    // Color assignment by grid cell index
    // Use quantized cell ID for stable color per cell
    vec2 qID = round(cellID / cellSize);
    float colorMod = mod(qID.x + qID.y * 2.0, 3.0);
    float colorID;
    if (colorMod < 0.5) {
        colorID = 1.0; // Red
    } else if (colorMod < 1.5) {
        colorID = 2.0; // Green
    } else {
        colorID = 3.0; // Blue
    }

    // Height fraction within tower for top glow (0=bottom, 1=top)
    float towerT = clamp((p.y) / (towerH * 2.0), 0.0, 1.0);

    // Floor
    float fd = sdFloor(p);
    if (fd < dist) {
        return vec3(fd, 0.0, 0.0);
    }

    return vec3(dist, colorID, towerT);
}

// ── Normal via finite differences ─────────────────────────────────────────
vec3 calcNormal(vec3 p) {
    const float eps = 0.002;
    vec2 e = vec2(eps, 0.0);
    return normalize(vec3(
        sceneSDF(p + e.xyy).x - sceneSDF(p - e.xyy).x,
        sceneSDF(p + e.yxy).x - sceneSDF(p - e.yxy).x,
        sceneSDF(p + e.yyx).x - sceneSDF(p - e.yyx).x
    ));
}

// ── Raymarcher ────────────────────────────────────────────────────────────
vec3 raymarch(vec3 ro, vec3 rd) {
    float t = 0.001;
    for (int i = 0; i < 80; i++) {
        vec3 p = ro + rd * t;
        vec3 res = sceneSDF(p);
        float d = res.x;
        if (d < 0.001) {
            return vec3(t, res.y, res.z);
        }
        if (t > 20.0) break;
        t += d * 0.85;
    }
    return vec3(-1.0, 0.0, 0.0);
}

// ── Main ──────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Camera: top-down 3/4 view with slow rotation
    float camAngle = TIME * 0.08;
    float camDist  = 4.0;
    float camH     = 3.5;
    vec3 ro = vec3(sin(camAngle) * camDist, camH, cos(camAngle) * camDist);
    vec3 target = vec3(0.0, 0.0, 0.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up    = cross(right, fwd);

    float fov = 0.7;
    vec3 rd = normalize(fwd + uv.x * right * fov + uv.y * up * fov);

    vec3 res = raymarch(ro, rd);
    float t       = res.x;
    float colorID = res.y;
    float towerT  = res.z;

    vec3 col = vec3(0.0); // black void background

    if (t > 0.0) {
        vec3 p = ro + rd * t;
        vec3 n = calcNormal(p);

        vec3 lightDir = normalize(vec3(0.3, 1.0, 0.5));
        float diff  = max(dot(n, lightDir), 0.0);
        float rim   = pow(max(1.0 - dot(-rd, n), 0.0), 4.0);

        // Top-glow factor: emission peaks at the top of each tower
        float topGlow = pow(towerT, 3.0);
        float audioMod = 1.0 + audioLevel * audioReact * 0.35;

        if (colorID < 0.5) {
            // Floor: dark with faint hex grid reflection
            float gridPat = hash21(floor(p.xz * 8.0));
            col = vec3(0.02, 0.02, 0.04) * (0.5 + 0.5 * gridPat);

        } else if (colorID < 1.5) {
            // R tower: dark body, bright red top
            vec3 bodyCol = vec3(0.3, 0.0, 0.0) * diff;
            vec3 topCol  = vec3(2.5, 0.0, 0.0) * hdrPeak / 2.5 * topGlow * audioMod;
            vec3 rimCol  = vec3(2.5, 0.2, 0.2) * rim * 0.4 * hdrPeak / 2.5;
            col = bodyCol + topCol + rimCol;

        } else if (colorID < 2.5) {
            // G tower: dark body, bright green top
            vec3 bodyCol = vec3(0.0, 0.3, 0.0) * diff;
            vec3 topCol  = vec3(0.0, 2.5, 0.0) * hdrPeak / 2.5 * topGlow * audioMod;
            vec3 rimCol  = vec3(0.2, 2.5, 0.2) * rim * 0.4 * hdrPeak / 2.5;
            col = bodyCol + topCol + rimCol;

        } else {
            // B tower: dark body, bright blue top
            vec3 bodyCol = vec3(0.0, 0.05, 0.3) * diff;
            vec3 topCol  = vec3(0.0, 0.5, 2.5) * hdrPeak / 2.5 * topGlow * audioMod;
            vec3 rimCol  = vec3(0.2, 0.5, 2.5) * rim * 0.4 * hdrPeak / 2.5;
            col = bodyCol + topCol + rimCol;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
