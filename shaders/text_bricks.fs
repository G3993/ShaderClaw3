/*{
  "DESCRIPTION": "Neon Corridor — 3D raymarched architectural corridor with brick walls, floor, and ceiling. Neon strip lights run along wall joints. Camera dollies forward cinematically. Hot-pink and cyan HDR neon glow against dark brick.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"speed",       "LABEL":"Speed",        "TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":2.0},
    {"NAME":"brickDensity","LABEL":"Brick Density", "TYPE":"float","DEFAULT":4.0,"MIN":1.0,"MAX":10.0},
    {"NAME":"hdrPeak",    "LABEL":"HDR Peak",     "TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioReact", "LABEL":"Audio React",  "TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":2.0},
    {"NAME":"fov",        "LABEL":"FOV",          "TYPE":"float","DEFAULT":1.8,"MIN":0.5,"MAX":3.0}
  ]
}*/

// ── Palette ──────────────────────────────────────────────────────────────────
const vec3 COL_NEON_PINK  = vec3(1.0, 0.0, 0.6);
const vec3 COL_NEON_CYAN  = vec3(0.0, 1.0, 0.9);
const vec3 COL_BRICK      = vec3(0.35, 0.1, 0.02);
const vec3 COL_MORTAR     = vec3(0.05, 0.03, 0.05);

// ── SDF helpers ─────────────────────────────────────────────────────────────
float sdPlane(vec3 p, vec3 n, float h) {
    return dot(p, n) + h;
}

// Thin 2D slab strip (infinite in one axis): distance in Y and Z only
float sdStrip(vec3 p, float cy, float cz, float ry, float rz) {
    vec2 d = abs(vec2(p.y - cy, p.z - cz)) - vec2(ry, rz);
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

// ── Corridor SDF ─────────────────────────────────────────────────────────────
// Returns vec2(dist, matID):
// matID 0 = brick wall/floor/ceiling
// matID 1 = neon strip pink  (floor joints)
// matID 2 = neon strip cyan  (ceiling joints)
vec2 sceneSDF(vec3 p) {
    // Corridor bounding planes
    float dLeft   =  p.x + 2.0;      // left wall  at x = -2
    float dRight  = -(p.x - 2.0);    // right wall at x = +2
    float dFloor  =  p.y + 1.5;      // floor      at y = -1.5
    float dCeil   = -(p.y - 1.5);    // ceiling    at y = +1.5

    float dWalls = min(min(dLeft, dRight), min(dFloor, dCeil));

    // Pink neon: floor-left joint (x=-2, y=-1.5) and floor-right joint (x=+2, y=-1.5)
    float dPinkL = length(vec2(p.x + 2.0, p.y + 1.5)) - 0.045;
    float dPinkR = length(vec2(p.x - 2.0, p.y + 1.5)) - 0.045;
    float stPink = min(dPinkL, dPinkR);

    // Cyan neon: ceil-left joint (x=-2, y=+1.5) and ceil-right joint (x=+2, y=+1.5)
    float dCyanL = length(vec2(p.x + 2.0, p.y - 1.5)) - 0.045;
    float dCyanR = length(vec2(p.x - 2.0, p.y - 1.5)) - 0.045;
    float stCyan = min(dCyanL, dCyanR);

    // Select closest
    if (stPink < dWalls && stPink < stCyan) return vec2(stPink, 1.0);
    if (stCyan < dWalls)                    return vec2(stCyan, 2.0);
    return vec2(dWalls, 0.0);
}

// ── Normal via finite differences ───────────────────────────────────────────
vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneSDF(p + e.xyy).x - sceneSDF(p - e.xyy).x,
        sceneSDF(p + e.yxy).x - sceneSDF(p - e.yxy).x,
        sceneSDF(p + e.yyx).x - sceneSDF(p - e.yyx).x
    ));
}

// ── Brick pattern ────────────────────────────────────────────────────────────
// Returns 1.0 on brick face, 0.0 in mortar joint
float brickPattern(vec2 localUV, float density) {
    float row   = floor(localUV.y * density);
    float xOff  = mod(row, 2.0) * 0.5;
    vec2 bUV    = vec2(mod(localUV.x * density * 1.6 + xOff, 1.0),
                       mod(localUV.y * density, 1.0));
    float mortarW = 0.08;
    float mx = min(bUV.x, 1.0 - bUV.x);
    float my = min(bUV.y, 1.0 - bUV.y);
    // AA with fwidth
    float fwx = fwidth(bUV.x);
    float fwy = fwidth(bUV.y);
    float brickX = smoothstep(mortarW - fwx, mortarW + fwx, mx);
    float brickY = smoothstep(mortarW - fwy, mortarW + fwy, my);
    return brickX * brickY;
}

// ── Neon glow contribution at a surface point ─────────────────────────────
vec3 neonGlow(vec3 p, float audioBst) {
    float dFL = length(vec2(p.y + 1.5, p.x + 2.0));
    float dFR = length(vec2(p.y + 1.5, p.x - 2.0));
    float dCL = length(vec2(p.y - 1.5, p.x + 2.0));
    float dCR = length(vec2(p.y - 1.5, p.x - 2.0));

    vec3 glow = vec3(0.0);
    glow += COL_NEON_PINK * exp(-dFL * 18.0) * audioBst;
    glow += COL_NEON_PINK * exp(-dFR * 18.0) * audioBst;
    glow += COL_NEON_CYAN * exp(-dCL * 18.0) * audioBst;
    glow += COL_NEON_CYAN * exp(-dCR * 18.0) * audioBst;
    return glow;
}

// ── Main ─────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Camera: dolly forward along +Z, slight horizontal sway
    float camZ  = -TIME * speed * 2.0;
    float sway  = sin(TIME * 0.15) * 0.06;
    vec3 ro     = vec3(sway, 0.0, camZ);
    vec3 rd     = normalize(vec3(uv, fov));

    float audioBst = 1.0 + audioLevel * audioReact;

    // ── Raymarch ─────────────────────────────────────────────────────────────
    float t     = 0.05;
    float matID = -1.0;
    vec3  p     = ro;
    bool  hit   = false;

    for (int i = 0; i < 64; i++) {
        p = ro + rd * t;
        vec2 res = sceneSDF(p);
        float d  = res.x;
        if (d < 0.002) { hit = true; matID = res.y; break; }
        if (t > 50.0)  break;
        t += max(d * 0.85, 0.01);
    }

    vec3 col = vec3(0.0);

    if (hit) {
        vec3 n       = calcNormal(p);
        vec3 viewDir = normalize(ro - p);

        // ── Brick walls / floor / ceiling ────────────────────────────────
        if (matID < 0.5) {
            vec2 localUV;
            if (abs(n.x) > 0.5) {
                // Side wall: depth + height
                localUV = vec2(p.z * 0.05, p.y);
            } else if (abs(n.y) > 0.5) {
                // Floor or ceiling: width + depth
                localUV = vec2(p.x, p.z * 0.05);
            } else {
                localUV = vec2(p.x, p.y);
            }

            float bk     = brickPattern(localUV, brickDensity);
            vec3 surfCol = mix(COL_MORTAR, COL_BRICK, bk);

            // Dim ambient + weak diffuse
            float diff = max(dot(n, normalize(vec3(0.2, 1.0, -0.5))), 0.0);
            col = surfCol * (0.05 + diff * 0.12);

            // Neon bounce light on surface
            col += neonGlow(p, audioBst) * 0.25 * (0.4 + 0.6 * bk);

            // Neon specular highlight on bricks
            vec3 halfV = normalize(normalize(vec3(0.0, -1.0, 0.3)) + viewDir);
            float spec = pow(max(dot(n, halfV), 0.0), 32.0);
            col += COL_NEON_PINK * spec * 0.3 * audioBst;
        }
        // ── Neon pink strip surfaces ─────────────────────────────────────
        else if (matID < 1.5) {
            col = COL_NEON_PINK * hdrPeak * audioBst;
        }
        // ── Neon cyan strip surfaces ─────────────────────────────────────
        else {
            col = COL_NEON_CYAN * hdrPeak * audioBst;
        }

        // Depth fog into the void
        float fogT = 1.0 - exp(-t * 0.045);
        col = mix(col, vec3(0.0), fogT * 0.75);
    }

    // ── Volumetric neon glow bleed along ray (additive) ──────────────────
    for (int k = 0; k < 8; k++) {
        float kt = 1.0 + float(k) * 4.0;
        if (kt > t) break;
        vec3 kp = ro + rd * kt;
        if (abs(kp.x) < 2.0 && abs(kp.y) < 1.5) {
            col += neonGlow(kp, audioBst) * 0.012;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
