/*{
  "DESCRIPTION": "Neon Aerial City — raymarched infinite city grid viewed from above, neon cyan windows on void black towers",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "camHeight",    "LABEL": "Cam Height",    "TYPE": "float", "MIN": 2.0,  "MAX": 12.0, "DEFAULT": 6.0 },
    { "NAME": "camAngle",     "LABEL": "Cam Tilt",      "TYPE": "float", "MIN": 0.1,  "MAX": 0.9,  "DEFAULT": 0.45 },
    { "NAME": "rotSpeed",     "LABEL": "Rotation",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.18 },
    { "NAME": "towerScale",   "LABEL": "Block Size",    "TYPE": "float", "MIN": 0.5,  "MAX": 2.5,  "DEFAULT": 1.2 },
    { "NAME": "maxHeight",    "LABEL": "Max Tower H",   "TYPE": "float", "MIN": 0.5,  "MAX": 4.0,  "DEFAULT": 2.2 },
    { "NAME": "windowScale",  "LABEL": "Window Grid",   "TYPE": "float", "MIN": 2.0,  "MAX": 12.0, "DEFAULT": 5.0 },
    { "NAME": "winBrightness","LABEL": "Window Glow",   "TYPE": "float", "MIN": 0.5,  "MAX": 3.0,  "DEFAULT": 2.0 },
    { "NAME": "fogDensity",   "LABEL": "Fog",           "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "audioMod",     "LABEL": "Audio Mod",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8 }
  ]
}*/

const float PI  = 3.14159265;
const float INF = 1e20;

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// Tower height from city grid cell id
float towerH(vec2 id) {
    float h = hash21(id);
    // occasional ground-floor gap (streets) — ~25% chance
    return h < 0.25 ? 0.0 : h * maxHeight * (1.0 + audioBass * audioMod * 0.4);
}

// SDF: box centered at origin, half-extents b
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// City SDF — infinite tiled towers
// Returns (dist, material): mat 0=ground, 1=tower wall, 2=tower top
vec2 cityMap(vec3 pos) {
    float grid = towerScale;
    float streetW = 0.22 * grid;  // fraction of block that's street gap
    float blockW  = grid - streetW;

    // Snap to nearest grid cell
    vec2 id = floor(pos.xz / grid);
    vec2 off = (pos.xz / grid - id - 0.5) * grid; // offset within cell [-grid/2, grid/2]

    float h = towerH(id);
    float hw = blockW * 0.5;

    // Ground plane
    float dGnd = pos.y;

    if (h < 0.001) {
        // street / no tower
        return vec2(dGnd, 0.0);
    }

    // Tower box: half-extents (hw, h/2, hw), centered at (0, h/2, 0) in cell space
    vec3 tp = vec3(off.x, pos.y - h * 0.5, off.y);
    float dBox = sdBox(tp, vec3(hw, h * 0.5, hw));

    float dMin = min(dGnd, dBox);
    float mat  = dMin == dGnd ? 0.0 : 1.0;

    // Slightly prefer ground at extreme distance to avoid precision issues
    return vec2(dMin, mat);
}

// Window mask on a tower face — returns emission brightness
float windowMask(vec3 p, vec3 normal) {
    // Project position onto face plane
    vec2 faceUV;
    if (abs(normal.x) > 0.5) faceUV = vec2(p.z, p.y);
    else                      faceUV = vec2(p.x, p.y);

    vec2 wCell = floor(faceUV * windowScale);
    float on = step(0.65, hash21(wCell));   // ~35% windows lit
    // Blink slowly
    float blink = step(0.85, sin(TIME * (0.5 + hash21(wCell + 7.3)) + hash21(wCell * 3.1) * 6.28) * 0.5 + 0.5);
    return on * (1.0 - blink * 0.6);
}

// Normal via central differences
vec3 cityNormal(vec3 p) {
    const float eps = 0.002;
    vec2 e = vec2(1.0, -1.0) * eps;
    return normalize(
        e.xyy * cityMap(p + e.xyy).x +
        e.yyx * cityMap(p + e.yyx).x +
        e.yxy * cityMap(p + e.yxy).x +
        e.xxx * cityMap(p + e.xxx).x
    );
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;

    // Camera: orbiting overhead, tilted down
    float angle = t * rotSpeed * 0.7;
    float tilt   = camAngle * PI * 0.5;  // 0=overhead, 0.5=horizon
    float ch = camHeight * (1.0 + audioBass * audioMod * 0.08);

    vec3 ro = vec3(cos(angle) * 0.5, ch, sin(angle) * 0.5)
            + vec3(sin(t * 0.07) * 2.0, 0.0, cos(t * 0.11) * 2.0); // slow drift

    // Look-at target slightly ahead of camera on floor
    vec3 target = vec3(ro.x + cos(angle + 0.3) * 2.0, 0.0, ro.z + sin(angle + 0.3) * 2.0);
    vec3 fwd = normalize(target - ro);
    vec3 rgt = normalize(cross(vec3(0,1,0), fwd));
    vec3 up  = cross(fwd, rgt);

    // Ray direction
    float fov = 0.8;
    vec3 rd = normalize(fwd + uv.x * rgt * fov + uv.y * up * fov);

    // Raymarching — 64 steps
    float dist = 0.0;
    float mat  = -1.0;
    for (int i = 0; i < 64; i++) {
        vec3 p  = ro + rd * dist;
        vec2 res = cityMap(p);
        if (res.x < 0.002) { mat = res.y; break; }
        if (dist > 80.0)    { mat = -1.0; break; }
        dist += res.x * 0.85;
    }

    // Palette
    // window cyan HDR:        [0, 1, 1]  * winBrightness (2.0)
    // edge orange:            [1, 0.4, 0] * 1.2
    // roof top:               [0, 0.5, 0.4] * 0.6
    // ground:                 [0, 0.04, 0.06]
    // sky/miss:               [0, 0.02, 0.04]
    // fog: deep night cyan    [0, 0.15, 0.2]

    vec3 fogColor = vec3(0.0, 0.12, 0.18);
    vec3 col;

    if (mat < 0.0) {
        // Sky — near black with faint cyan zenith
        float zenith = max(0.0, dot(rd, vec3(0,1,0)));
        col = mix(fogColor * 0.3, vec3(0.0, 0.04, 0.08), zenith);
    } else {
        vec3 hitPos = ro + rd * dist;
        vec3 nor    = cityNormal(hitPos);

        if (mat < 0.5) {
            // Ground
            vec2 gCell = floor(hitPos.xz * 3.0);
            float gLine = min(fract(hitPos.x * 3.0), fract(hitPos.z * 3.0));
            float grid  = smoothstep(0.04, 0.0, gLine) * 0.4;
            col = vec3(0.0, 0.04, 0.06) + vec3(0.0, 0.2, 0.35) * grid;
        } else {
            // Tower wall — check if this face has windows
            bool isTop = nor.y > 0.5;
            if (isTop) {
                // Roof: muted orange-red
                col = vec3(0.8, 0.20, 0.0) * 0.35;
            } else {
                float win = windowMask(hitPos, nor);
                // Base wall: very dark
                vec3 wallCol = vec3(0.0, 0.02, 0.03);
                // Window: cyan HDR
                vec3 winCol  = vec3(0.0, 1.0, 1.0) * win * winBrightness
                             * (1.0 + audioHigh * audioMod * 0.5);
                // Corner edge glow: orange
                float bevel = 1.0 - smoothstep(0.0, 0.015, abs(sdBox(hitPos - vec3(floor(hitPos.x/towerScale)*towerScale + towerScale*0.5, hitPos.y, floor(hitPos.z/towerScale)*towerScale + towerScale*0.5), vec3(towerScale*0.5 - 0.22*towerScale*0.5, 999.0, towerScale*0.5 - 0.22*towerScale*0.5))));
                vec3 edgeCol = vec3(1.0, 0.4, 0.0) * bevel * 1.2;
                col = wallCol + winCol + edgeCol;
            }
        }

        // Distance fog — blends to deep night cyan
        float fogAmt = 1.0 - exp(-dist * fogDensity * 0.06);
        col = mix(col, fogColor, fogAmt);
    }

    // Audio: overall luminance pulse on bass
    col *= 1.0 + audioBass * audioMod * 0.15;

    gl_FragColor = vec4(col, 1.0);
}
