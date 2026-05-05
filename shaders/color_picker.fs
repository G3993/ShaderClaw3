/*{
  "DESCRIPTION": "Neon Plasma Globe — 3D analytical sphere with FBM lightning arc ridges. Camera orbits slowly. Standalone generator.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"speed",      "TYPE":"float","DEFAULT":0.5, "MIN":0.0, "MAX":2.0,  "LABEL":"Orbit Speed"},
    {"NAME":"arcDensity", "TYPE":"float","DEFAULT":4.0, "MIN":1.0, "MAX":8.0,  "LABEL":"Arc Density"},
    {"NAME":"hdrPeak",    "TYPE":"float","DEFAULT":2.5, "MIN":1.0, "MAX":4.0,  "LABEL":"HDR Peak"},
    {"NAME":"audioReact", "TYPE":"float","DEFAULT":0.7, "MIN":0.0, "MAX":2.0,  "LABEL":"Audio React"},
    {"NAME":"camDist",    "TYPE":"float","DEFAULT":2.8, "MIN":1.5, "MAX":6.0,  "LABEL":"Camera Dist"},
    {"NAME":"glowWidth",  "TYPE":"float","DEFAULT":0.08,"MIN":0.01,"MAX":0.3,  "LABEL":"Glow Width"}
  ]
}*/

// ---------- hash / value noise / FBM (3-D) ----------

float hash3f(vec3 p) {
    p = fract(p * vec3(127.1, 311.7, 74.7));
    p += dot(p, p.yzx + 19.19);
    return fract((p.x + p.y) * p.z);
}

float vnoise3(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    vec3 u = f * f * (3.0 - 2.0 * f);
    float v000 = hash3f(i + vec3(0.0,0.0,0.0));
    float v100 = hash3f(i + vec3(1.0,0.0,0.0));
    float v010 = hash3f(i + vec3(0.0,1.0,0.0));
    float v110 = hash3f(i + vec3(1.0,1.0,0.0));
    float v001 = hash3f(i + vec3(0.0,0.0,1.0));
    float v101 = hash3f(i + vec3(1.0,0.0,1.0));
    float v011 = hash3f(i + vec3(0.0,1.0,1.0));
    float v111 = hash3f(i + vec3(1.0,1.0,1.0));
    return mix(mix(mix(v000,v100,u.x), mix(v010,v110,u.x), u.y),
               mix(mix(v001,v101,u.x), mix(v011,v111,u.x), u.y), u.z);
}

float fbm3(vec3 p) {
    float v = 0.0;
    float a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * vnoise3(p);
        p *= 2.09;
        a *= 0.48;
    }
    return v;
}

// ---------- 5-hue palette cycling ----------

vec3 arcPalette(float t) {
    vec3 c0 = vec3(1.0, 0.0, 0.8);   // hot magenta
    vec3 c1 = vec3(0.0, 1.0, 1.0);   // electric cyan
    vec3 c2 = vec3(1.0, 0.85,0.0);   // vivid gold
    vec3 c3 = vec3(0.5, 0.0, 1.0);   // deep violet
    vec3 c4 = vec3(1.0, 0.4, 0.0);   // hot orange
    t = fract(t);
    float s = t * 5.0;
    float f = fract(s);
    int idx = int(floor(s));
    if (idx == 0) return mix(c0, c1, f);
    if (idx == 1) return mix(c1, c2, f);
    if (idx == 2) return mix(c2, c3, f);
    if (idx == 3) return mix(c3, c4, f);
    return mix(c4, c0, f);
}

// ---------- main ----------

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // audio modulation
    float aLevel = 1.0 + audioLevel * audioReact;
    float aBass  = 1.0 + audioBass  * audioReact * 1.2;

    // camera orbit
    float camAngle = TIME * speed * 0.23;
    vec3 camPos = vec3(sin(camAngle) * camDist, 0.35, cos(camAngle) * camDist);
    vec3 target  = vec3(0.0);

    // camera basis
    vec3 fwd   = normalize(target - camPos);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up    = cross(right, fwd);

    // ray
    vec3 rd = normalize(fwd + uv.x * right * 0.7 + uv.y * up * 0.7);
    vec3 ro  = camPos;

    // analytical sphere (unit sphere at origin)
    float b    = dot(ro, rd);
    float c    = dot(ro, ro) - 1.0;
    float disc = b * b - c;

    vec3 col = vec3(0.0);

    if (disc > 0.0) {
        // ---------- sphere hit ----------
        float sqrtDisc = sqrt(disc);
        float t0   = -b - sqrtDisc;
        float tHit = (t0 > 0.001) ? t0 : (-b + sqrtDisc);

        vec3 hitPos = ro + rd * tHit;
        vec3 N      = normalize(hitPos);   // unit sphere: normal == position

        // arc ridges via FBM on rotated surface coord
        float density = arcDensity * aBass;
        vec3 sp  = N * density;
        float a1 = fbm3(sp + vec3(TIME * 0.4 * speed));
        float a2 = fbm3(sp * 1.7 + vec3(0.0, TIME * 0.3 * speed, TIME * 0.2 * speed));
        float arcVal = (a1 + a2 * 0.5) / 1.5;

        // primary ridge
        float edgeDist = abs(arcVal - 0.5);
        float fw       = fwidth(arcVal);
        float ridge    = 1.0 - smoothstep(0.0, glowWidth + fw, edgeDist);

        // secondary thin ridges (higher frequency)
        float arcHigh  = fract(arcVal * 3.0);
        float edgeDist2 = abs(arcHigh - 0.5);
        float fw2       = fwidth(arcHigh);
        float ridge2   = 1.0 - smoothstep(0.0, glowWidth * 0.35 + fw2, edgeDist2);

        vec3 arcCol1 = arcPalette(arcVal + TIME * 0.07 * speed);
        vec3 arcCol2 = arcPalette(arcVal * 2.0 + TIME * 0.11 * speed + 0.3);

        // very dark base
        vec3 surfBase = vec3(0.01, 0.0, 0.02);

        // rim glow (violet)
        float rim     = pow(1.0 - abs(dot(N, normalize(camPos))), 2.0);
        vec3  rimGlow = vec3(0.5, 0.0, 1.0) * rim * 1.5;

        col = surfBase
            + arcCol1 * ridge  * hdrPeak * aLevel
            + arcCol2 * ridge2 * hdrPeak * 0.5 * aLevel
            + rimGlow;

    } else {
        // ---------- ray missed — soft halo around sphere ----------
        // distance from ray to sphere center
        float closestDist = sqrt(max(0.0, c - b * b));
        float halo = exp(-max(0.0, closestDist - 1.0) * 4.5);
        vec3 haloColor = vec3(0.5, 0.0, 1.0) * halo * 0.7;

        // faint spark at tangent ring
        float fw      = fwidth(closestDist);
        float sparkD  = abs(closestDist - 1.04);
        float spark   = 1.0 - smoothstep(0.0, 0.018 + fw, sparkD);
        float tClose  = max(0.0, -b);
        spark *= clamp(tClose / camDist, 0.0, 1.0);
        vec3 sparkCol = arcPalette(closestDist * 3.0 + TIME * 0.2 * speed) * spark * hdrPeak;

        col = haloColor + sparkCol;
    }

    gl_FragColor = vec4(col, 1.0);
}
