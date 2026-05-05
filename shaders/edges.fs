/*{
  "DESCRIPTION": "Crystal Ribbon Storm — 3D raymarched dark sphere core surrounded by 12 orbiting crystalline ribbons at varied inclinations. Cinematic three-point lighting.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"speed",       "TYPE":"float","DEFAULT":0.5, "MIN":0.0,"MAX":2.0,  "LABEL":"Orbit Speed"},
    {"NAME":"ribbonScale", "TYPE":"float","DEFAULT":1.0, "MIN":0.3,"MAX":2.0,  "LABEL":"Ribbon Scale"},
    {"NAME":"hdrPeak",     "TYPE":"float","DEFAULT":2.5, "MIN":1.0,"MAX":4.0,  "LABEL":"HDR Peak"},
    {"NAME":"audioReact",  "TYPE":"float","DEFAULT":0.6, "MIN":0.0,"MAX":2.0,  "LABEL":"Audio React"}
  ]
}*/

// ---------- hash ----------

float hashF(float n) {
    return fract(sin(n) * 43758.5453123);
}

// ---------- rotation matrices ----------

mat3 rotY(float a) {
    float c = cos(a), s = sin(a);
    return mat3(c, 0.0, s,  0.0, 1.0, 0.0,  -s, 0.0, c);
}

mat3 rotX(float a) {
    float c = cos(a), s = sin(a);
    return mat3(1.0, 0.0, 0.0,  0.0, c, -s,  0.0, s, c);
}

mat3 rotZ(float a) {
    float c = cos(a), s = sin(a);
    return mat3(c, -s, 0.0,  s, c, 0.0,  0.0, 0.0, 1.0);
}

// ---------- SDFs ----------

float sdSphere(vec3 p, float r) {
    return length(p) - r;
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float opUnion(float a, float b) {
    return min(a, b);
}

// ---------- scene SDF + material id ----------
// Returns vec2: x=dist, y=material (0=sphere, 1=ribbon)

vec2 sceneSDF(vec3 p, float t, float spd, float rScale) {
    float sphereDist = sdSphere(p, 0.5);

    float bestRibbon = 1e10;

    float orbitR = 1.3;
    // ribbon half-extents
    vec3 ribbonSize = vec3(0.8, 0.03, 0.03) * rScale;

    for (int i = 0; i < 12; i++) {
        float fi = float(i);
        float h1 = hashF(fi * 1.37 + 0.5);
        float h2 = hashF(fi * 3.71 + 1.2);
        float h3 = hashF(fi * 7.13 + 2.9);

        float orbitSpeed = spd * (0.3 + h1 * 0.4);
        float angle = 6.28318 * fi / 12.0 + t * orbitSpeed;
        float inclination = 3.14159 * h2 * 0.8;
        float rollAngle   = 6.28318 * h3;

        // ribbon center on orbit
        vec3 ribbonCenter = vec3(cos(angle) * orbitR, 0.0, sin(angle) * orbitR);

        // apply inclination rotation (tilt orbit plane)
        ribbonCenter = rotX(inclination) * ribbonCenter;

        // transform point into ribbon local space
        vec3 lp = p - ribbonCenter;

        // orient ribbon tangent to orbit: tangent = derivative of orbit w.r.t angle
        // tangent in base orbit: (-sin(angle), 0, cos(angle))
        vec3 tangent = rotX(inclination) * vec3(-sin(angle), 0.0, cos(angle));
        // ribbon axis aligned along tangent
        // build local frame
        vec3 axisX = normalize(tangent);
        vec3 axisY = vec3(0.0, 1.0, 0.0);
        // if axisX too close to Y, pick Z
        if (abs(axisX.y) > 0.9) axisY = vec3(0.0, 0.0, 1.0);
        vec3 axisZ = normalize(cross(axisX, axisY));
        axisY = cross(axisZ, axisX);

        // apply extra roll
        float cosR = cos(rollAngle), sinR = sin(rollAngle);
        vec3 axisY2 = cosR * axisY + sinR * axisZ;
        vec3 axisZ2 = -sinR * axisY + cosR * axisZ;

        // project into local frame
        vec3 lLocal = vec3(dot(lp, axisX), dot(lp, axisY2), dot(lp, axisZ2));
        float d = sdBox(lLocal, ribbonSize);
        bestRibbon = opUnion(bestRibbon, d);
    }

    if (sphereDist < bestRibbon) {
        return vec2(sphereDist, 0.0);
    }
    return vec2(bestRibbon, 1.0);
}

// ---------- finite-difference normal ----------

vec3 sceneNormal(vec3 p, float t, float spd, float rScale) {
    float eps = 0.001;
    vec2 e = vec2(eps, 0.0);
    float dx = sceneSDF(p + e.xyy, t, spd, rScale).x - sceneSDF(p - e.xyy, t, spd, rScale).x;
    float dy = sceneSDF(p + e.yxy, t, spd, rScale).x - sceneSDF(p - e.yxy, t, spd, rScale).x;
    float dz = sceneSDF(p + e.yyx, t, spd, rScale).x - sceneSDF(p - e.yyx, t, spd, rScale).x;
    return normalize(vec3(dx, dy, dz));
}

// ---------- main ----------

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // audio
    float aLevel = 1.0 + audioLevel * audioReact;
    float aBass  = 1.0 + audioBass  * audioReact * 1.3;

    // cinematic camera — slight orbit
    float camA = TIME * speed * 0.15;
    vec3 camPos = vec3(sin(camA) * 3.5, 0.8 + sin(TIME * speed * 0.07) * 0.3, cos(camA) * 3.5);
    vec3 target  = vec3(0.0);
    vec3 fwd     = normalize(target - camPos);
    vec3 right   = normalize(cross(fwd, vec3(0.0,1.0,0.0)));
    vec3 upV     = cross(right, fwd);
    vec3 rd      = normalize(fwd + uv.x * right * 0.65 + uv.y * upV * 0.65);
    vec3 ro      = camPos;

    // ---------- raymarch ----------
    float totalDist = 0.0;
    float matID = -1.0;
    bool hit = false;

    for (int step = 0; step < 64; step++) {
        vec3 p = ro + rd * totalDist;
        vec2 res = sceneSDF(p, TIME, speed, ribbonScale * aBass);
        float d = res.x;
        if (d < 0.001) {
            matID = res.y;
            hit = true;
            break;
        }
        totalDist += d * 0.9;
        if (totalDist > 20.0) break;
    }

    vec3 col = vec3(0.0, 0.0, 0.01); // void black background

    if (hit) {
        vec3 p = ro + rd * totalDist;
        vec3 N = sceneNormal(p, TIME, speed, ribbonScale * aBass);

        // three-point lights
        vec3 keyDir  = normalize(vec3( 1.0, 0.8, 0.6));   // front-right
        vec3 fillDir = normalize(vec3(-1.0, 0.3, 0.5));   // left
        vec3 rimDir  = normalize(vec3( 0.0,-0.2,-1.0));   // behind viewer

        float keyDiff  = max(0.0, dot(N, keyDir));
        float fillDiff = max(0.0, dot(N, fillDir));
        float rimDiff  = max(0.0, dot(N, rimDir));

        // specular (Phong ^32)
        vec3 viewDir = normalize(-rd);
        vec3 keySpec  = vec3(pow(max(0.0, dot(reflect(-keyDir,  N), viewDir)), 32.0));
        vec3 fillSpec = vec3(pow(max(0.0, dot(reflect(-fillDir, N), viewDir)), 32.0));
        vec3 rimSpec  = vec3(pow(max(0.0, dot(reflect(-rimDir,  N), viewDir)), 32.0));

        // HDR fwidth AA on SDF edge
        float edgeFw = fwidth(totalDist);
        float edgeSoft = 1.0 - smoothstep(0.0, edgeFw * 2.0, 0.001);

        if (matID < 0.5) {
            // ---------- sphere core ----------
            vec3 sphereBase = vec3(0.02, 0.01, 0.03);
            // dark with cool rim
            vec3 lightCyan = vec3(0.0, 1.0, 1.0);
            col = sphereBase
                + lightCyan * fillDiff * 0.2
                + vec3(0.3, 0.0, 1.0) * rimDiff * 0.5 * aLevel;

        } else {
            // ---------- crystal ribbon ----------
            // base ribbon tint: cycles per ribbon by position
            float ribbonHue = hashF(floor(totalDist * 3.0 + 7.3));
            vec3 ribBase;
            if (ribbonHue < 0.33) {
                ribBase = vec3(0.0, 1.0, 1.0);    // electric cyan
            } else if (ribbonHue < 0.66) {
                ribBase = vec3(1.0, 0.0, 0.7);    // hot magenta
            } else {
                ribBase = vec3(0.3, 0.0, 1.0);    // deep violet
            }

            // diamond white specular (HDR)
            vec3 diamondSpec = vec3(1.0) * 2.5;

            vec3 keyLight  = diamondSpec * keySpec  * hdrPeak * aLevel;
            vec3 fillLight = vec3(0.0,1.0,1.0) * fillDiff * 0.6 * hdrPeak * 0.5;
            vec3 rimLight  = vec3(1.0,0.0,0.7) * rimDiff  * hdrPeak * aLevel;

            col = ribBase * 0.05
                + ribBase  * keyDiff  * hdrPeak * 0.4 * aLevel
                + fillLight
                + rimLight
                + keyLight;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
