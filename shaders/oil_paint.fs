/*{
  "DESCRIPTION": "Painted Canyon Sunset — wide raymarched canyon at golden hour. Warm/cool two-zone lighting, procedural FBM brush-stroke texture on canyon walls, sun disc.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"sunAngle",   "TYPE":"float","DEFAULT":0.4, "MIN":0.0,  "MAX":1.5708,"LABEL":"Sun Angle"},
    {"NAME":"paintDetail","TYPE":"float","DEFAULT":0.4, "MIN":0.0,  "MAX":1.0,   "LABEL":"Paint Detail"},
    {"NAME":"hdrPeak",    "TYPE":"float","DEFAULT":2.0, "MIN":1.0,  "MAX":4.0,   "LABEL":"HDR Peak"},
    {"NAME":"audioReact", "TYPE":"float","DEFAULT":0.5, "MIN":0.0,  "MAX":2.0,   "LABEL":"Audio React"},
    {"NAME":"cameraRoll", "TYPE":"float","DEFAULT":0.0, "MIN":-0.5, "MAX":0.5,   "LABEL":"Camera Roll"}
  ]
}*/

// ---------- noise / FBM (2-D, used on 3-D wall surfaces via xz) ----------

float hashV(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float vnoise2(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    float a = hashV(i + vec2(0,0));
    float b = hashV(i + vec2(1,0));
    float c = hashV(i + vec2(0,1));
    float d = hashV(i + vec2(1,1));
    return mix(mix(a,b,u.x), mix(c,d,u.x), u.y);
}

// 3-D FBM using xz and y slices
float fbmWall(vec3 p, float toffset) {
    vec2 q = p.xz + vec2(p.y * 0.5, 0.0) + vec2(toffset);
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * vnoise2(q);
        q *= 2.13;
        a *= 0.48;
    }
    return v;
}

float fbmSky(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * vnoise2(p);
        p *= 2.0;
        a *= 0.5;
    }
    return v;
}

// ---------- SDFs ----------

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// ---------- scene SDF ----------
// Returns vec2: x=dist, y=material (0=floor, 1=left wall, 2=right wall)

vec2 sceneSDF(vec3 p, float tOff, float pDetail) {
    // Canyon floor: infinite plane at y=-1
    float floorDist = p.y + 1.0;

    // Canyon walls: SDF box with FBM displacement
    // Left wall: center at (-2.5, 1.5, 0), half-size (0.5, 2.5, 30)
    vec3 leftCenter  = vec3(-2.5, 1.5, 0.0);
    vec3 rightCenter = vec3( 2.5, 1.5, 0.0);
    vec3 wallHalf    = vec3(0.5, 2.5, 30.0);

    float leftFBM  = fbmWall(p * 2.0, tOff);
    float rightFBM = fbmWall(p * 2.0 + vec3(17.3, 5.1, 9.7), tOff);

    float leftDist  = sdBox(p - leftCenter,  wallHalf) + (leftFBM  - 0.5) * 0.3 * pDetail;
    float rightDist = sdBox(p - rightCenter, wallHalf) + (rightFBM - 0.5) * 0.3 * pDetail;

    // find closest
    float bestDist = floorDist;
    float bestMat  = 0.0;
    if (leftDist < bestDist)  { bestDist = leftDist;  bestMat = 1.0; }
    if (rightDist < bestDist) { bestDist = rightDist; bestMat = 2.0; }

    return vec2(bestDist, bestMat);
}

// ---------- finite-difference normal ----------

vec3 sceneNormal(vec3 p, float tOff, float pDetail) {
    float eps = 0.002;
    vec2 e = vec2(eps, 0.0);
    float dx = sceneSDF(p + e.xyy, tOff, pDetail).x - sceneSDF(p - e.xyy, tOff, pDetail).x;
    float dy = sceneSDF(p + e.yxy, tOff, pDetail).x - sceneSDF(p - e.yxy, tOff, pDetail).x;
    float dz = sceneSDF(p + e.yyx, tOff, pDetail).x - sceneSDF(p - e.yyx, tOff, pDetail).x;
    return normalize(vec3(dx, dy, dz));
}

// ---------- main ----------

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // camera roll
    float cr = cameraRoll;
    vec2 uvR = vec2(uv.x * cos(cr) - uv.y * sin(cr),
                    uv.x * sin(cr) + uv.y * cos(cr));

    // audio
    float aLevel = 1.0 + audioLevel * audioReact;
    float aBass  = 1.0 + audioBass  * audioReact * 0.8;

    // sun direction from angle
    vec3 sunDir = normalize(vec3(cos(sunAngle) * 0.6, sin(sunAngle), -cos(sunAngle) * 0.4));

    // camera: looking down the canyon (along -Z), slightly elevated
    vec3 camPos = vec3(0.0, 0.8, 6.0);
    vec3 fwd    = normalize(vec3(0.0, -0.15, -1.0));
    vec3 right  = normalize(cross(fwd, vec3(0.0,1.0,0.0)));
    vec3 upV    = cross(right, fwd);

    vec3 rd = normalize(fwd + uvR.x * right * 0.7 + uvR.y * upV * 0.7);
    vec3 ro  = camPos;

    float tOff = TIME * 0.05;

    // ---------- raymarch ----------
    float totalDist = 0.0;
    float matID = -1.0;
    bool hit = false;
    for (int step = 0; step < 64; step++) {
        vec3 p = ro + rd * totalDist;
        vec2 res = sceneSDF(p, tOff, paintDetail);
        float d = res.x;
        if (d < 0.005) {
            matID = res.y;
            hit = true;
            break;
        }
        totalDist += d * 0.85;
        if (totalDist > 30.0) break;
    }

    // ---- sky / background ----
    // sunset gradient: dark blue-black at top → orange/gold at horizon
    vec3 skyBase   = mix(vec3(0.0, 0.02, 0.3), vec3(1.0, 0.35, 0.0) * hdrPeak * 0.6,
                         pow(max(0.0, 1.0 - rd.y * 2.5), 0.5));
    // warm horizon band
    float horizBand = exp(-abs(rd.y) * 6.0);
    skyBase += vec3(1.0, 0.7, 0.0) * horizBand * 0.4 * hdrPeak;

    // cloud streaks in sky
    vec2 skyUV = rd.xz / (abs(rd.y) + 0.01) * 0.15;
    float clouds = fbmSky(skyUV + vec2(tOff * 2.0));
    skyBase += vec3(1.0, 0.55, 0.1) * clouds * 0.15 * max(0.0, -rd.y * 2.0 + 1.0);

    // sun disc
    float sunDot = dot(rd, sunDir);
    float sunDisc = step(0.998, sunDot);
    float sunGlow = exp(-(1.0 - sunDot) * 120.0) * (1.0 - sunDisc);
    skyBase += vec3(2.0, 1.2, 0.3) * sunDisc * hdrPeak * aLevel;
    skyBase += vec3(1.0, 0.55, 0.1) * sunGlow * 0.8 * hdrPeak;

    vec3 col = skyBase;

    if (hit) {
        vec3 p = ro + rd * totalDist;
        vec3 N = sceneNormal(p, tOff, paintDetail);

        // fwidth-based SDF edge AA
        float fw = fwidth(totalDist);

        // paint-stroke texture: modulate surface by FBM
        float stroke = sin(fbmWall(p * 5.0, 0.0) * 3.14159 * 4.0) * 0.5 + 0.5;
        stroke = mix(0.5, stroke, paintDetail);

        // warm sun light
        float NdotSun  = max(0.0, dot(N, sunDir));
        vec3  lightWarm = NdotSun * vec3(1.0, 0.7, 0.2) * 2.0 * hdrPeak * aLevel;

        // cool sky fill (from above)
        float NdotSky  = max(0.0, dot(N, vec3(0.0,1.0,0.0)));
        vec3  lightCool = NdotSky * vec3(0.1, 0.2, 0.6) * 0.8;

        // shadow self-occlusion: bottom of walls darker
        float vertShadow = clamp((p.y + 1.0) / 3.0, 0.0, 1.0);

        if (matID < 0.5) {
            // ---------- canyon floor ----------
            vec3 groundBase = vec3(0.6, 0.2, 0.0); // rust
            col = groundBase * stroke * (lightWarm * 0.6 + lightCool + 0.05);

        } else {
            // ---------- canyon wall (left or right) ----------
            // Warm face vs cool shadow split
            vec3 rockWarm = vec3(0.9, 0.45, 0.1);
            vec3 rockCool = vec3(0.15, 0.05, 0.4);
            float warmCoolT = clamp(NdotSun * 1.5, 0.0, 1.0);
            vec3 rockBase = mix(rockCool, rockWarm, warmCoolT);
            rockBase = mix(rockBase, rockBase * stroke, 0.4);

            col = rockBase * (lightWarm + lightCool + 0.04) * vertShadow;

            // add rim where walls face viewer
            float rimFace = max(0.0, dot(N, normalize(-rd)));
            col += rockWarm * pow(rimFace, 3.0) * 0.3 * hdrPeak;
        }

        // distance fog toward sky color
        float fogT = clamp(totalDist / 28.0, 0.0, 1.0);
        fogT = fogT * fogT;
        col = mix(col, skyBase * 0.3, fogT);
    }

    gl_FragColor = vec4(col, 1.0);
}
