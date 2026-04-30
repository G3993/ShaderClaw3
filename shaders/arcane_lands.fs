/*{
  "DESCRIPTION": "Arcane Lands — raymarched landscape flythrough with god rays and lens flare. Optional rock texture for triplanar surfaces.",
  "CREDIT": "Ported from Shadertoy XdcfR7 by Dave Hoskins (CC BY-NC-SA 3.0)",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "rockTex",     "LABEL": "Rock Texture (optional)", "TYPE": "image" },
    { "NAME": "speed",       "LABEL": "Flight Speed",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 4.0 },
    { "NAME": "timeOffset",  "LABEL": "Time Offset",   "TYPE": "float", "DEFAULT": 410.0,"MIN": 0.0,  "MAX": 2000.0 },
    { "NAME": "marchSteps",  "LABEL": "March Steps",   "TYPE": "float", "DEFAULT": 200.0,"MIN": 80.0, "MAX": 500.0 },
    { "NAME": "godrayAmt",   "LABEL": "God Rays",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "lensFlare",   "LABEL": "Lens Flare",    "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "vignetteAmt", "LABEL": "Vignette",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 }
  ],
  "PASSES": [
    { "TARGET": "bufA", "WIDTH": "64", "HEIGHT": "1" },
    { "TARGET": "bufB", "PERSISTENT": false },
    {}
  ]
}*/

// ──────────────────────────────────────────────────────────────────────
// Buffer A index slots (held in a 64×1 data texture)
// ──────────────────────────────────────────────────────────────────────
#define CAMERA_POS    0
#define CAMERA_TAR    1
#define CAMERA_MAT0   2
#define CAMERA_MAT1   3
#define CAMERA_MAT2   4
#define SUN_DIRECTION 5
#define LAST          6

#define FAR        1100.0
#define TAU        6.28318530718
#define SUN_COLOUR vec3(1.0, 0.8, 0.7)
#define FOG_COLOUR vec3(0.4, 0.4, 0.4)

// ──────────────────────────────────────────────────────────────────────
// Globals (set per-pass)
// ──────────────────────────────────────────────────────────────────────
vec3  g_sunLight;
vec3  g_camPos;
mat3  g_camMat;
float g_specular;
float g_zProj;

// ──────────────────────────────────────────────────────────────────────
// Hashes (Hoskins-style uint hashes — GLSL 330 supports uvec/uint)
// ──────────────────────────────────────────────────────────────────────
const uint UI0 = 1597334673u;
const uint UI1 = 3812015801u;
const uvec2 UI2 = uvec2(UI0, UI1);
const float UIF = 1.0 / float(0xffffffffu);

float hash11(float p) {
    uvec2 n = uvec2(uint(int(p))) * UI2;
    uint q = (n.x ^ n.y) * UI0;
    return float(q) * UIF;
}
float hash12(vec2 p) {
    uvec2 q = uvec2(ivec2(p)) * UI2;
    uint n = (q.x ^ q.y) * UI0;
    return float(n) * UIF;
}

// ──────────────────────────────────────────────────────────────────────
// Procedural noise (replaces iChannel3 256×256 lookup)
// ──────────────────────────────────────────────────────────────────────
float n2dHash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float noise2d(vec2 x) {
    vec2 p = floor(x);
    vec2 f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(n2dHash(p),                n2dHash(p + vec2(1, 0)), f.x),
               mix(n2dHash(p + vec2(0, 1)),   n2dHash(p + vec2(1, 1)), f.x), f.y);
}
float noise3d(vec3 p) {
    vec3 f = fract(p);
    p = floor(p);
    f = f * f * (3.0 - 2.0 * f);
    vec2 uv = (p.xy + vec2(37.0, 17.0) * p.z) + f.xy;
    float a = noise2d((uv + 0.5) / 256.0);
    float b = noise2d((uv + vec2(0.0, 1.0) + 0.5) / 256.0);
    return mix(a, b, f.z);
}

// ──────────────────────────────────────────────────────────────────────
// Camera path & matrix (used in Buffer A)
// ──────────────────────────────────────────────────────────────────────
vec3 cameraPath(float z) {
    return vec3(200.0 * sin(z * 0.0045) + 190.0 * cos(z * 0.001),
                43.0 * (cos(z * 0.0047) + sin(z * 0.0013)) + 53.0 * sin(z * 0.003),
                z);
}
mat3 setCamMat(vec3 ro, vec3 ta, float cr) {
    vec3 cw = normalize(ta - ro);
    vec3 cp = vec3(sin(cr), cos(cr), 0.0);
    vec3 cu = normalize(cross(cw, cp));
    vec3 cv = normalize(cross(cu, cw));
    return mat3(cu, cv, cw);
}

float sMax(float a, float b, float s) {
    float h = clamp(0.5 + 0.5 * (a - b) / s, 0.0, 1.0);
    return mix(b, a, h) + h * (1.0 - h) * s;
}
float projectZ(vec2 uv) { return 0.6; }

// ──────────────────────────────────────────────────────────────────────
// Read camera state from Buffer A
// ──────────────────────────────────────────────────────────────────────
vec4 getStore(int num) {
    return texelFetch(bufA, ivec2(num, 0), 0);
}
mat3 getStoreMat33(int num) {
    vec3 m0 = texelFetch(bufA, ivec2(num,     0), 0).xyz;
    vec3 m1 = texelFetch(bufA, ivec2(num + 1, 0), 0).xyz;
    vec3 m2 = texelFetch(bufA, ivec2(num + 2, 0), 0).xyz;
    return mat3(m0, m1, m2);
}

// ──────────────────────────────────────────────────────────────────────
// PASS 0 — Buffer A: store camera pos/target/matrix + sun direction
// ──────────────────────────────────────────────────────────────────────
float grabTime() {
    return (TIME * speed + timeOffset) * 32.0;
}
int storeIndex(ivec2 p) { return p.x + 64 * p.y; }

vec4 passBufA(vec2 fragCoord) {
    ivec2 pos = ivec2(fragCoord);
    vec4 col = vec4(0.0);
    int num = storeIndex(pos);
    if (num >= LAST) return col;

    float gTime = grabTime();
    float r = gTime / 63.0;
    vec3 cP = vec3(0.0), cT = vec3(0.0);
    mat3 cM = mat3(1.0);
    if (num <= CAMERA_MAT2) {
        cP = cameraPath(gTime) + vec3(sin(r * 0.4) * 24.0, cos(r * 0.3) * 24.0, 0.0);
        cT = cameraPath(gTime + 30.0);
        cM = setCamMat(cP, cT, (cT.x - cP.x) * 0.02);
    }
    if      (num == CAMERA_POS)      col.xyz = cP;
    else if (num == CAMERA_TAR)      col.xyz = cT;
    else if (num == CAMERA_MAT0)     col.xyz = cM[0];
    else if (num == CAMERA_MAT1)     col.xyz = cM[1];
    else if (num == CAMERA_MAT2)     col.xyz = cM[2];
    else if (num == SUN_DIRECTION)   col.xyz = normalize(vec3(0.3, 0.75, 0.4));
    return col;
}

// ──────────────────────────────────────────────────────────────────────
// PASS 1 — Buffer B: raymarched landscape + sky/clouds
// ──────────────────────────────────────────────────────────────────────
#define VOR_SCALE 0.01

// Terrain SDF — with mip-aware noise sampling
float mapTerrain(vec3 p, float di) {
    // Detail layer (replaces textureLod(iChannel1, ...) in original)
    float te = noise2d(p.xz * 0.0017 + p.xy * 0.0019 - p.zy * 0.0017) * 80.0;
    // Wibbly cross noise
    float h = dot(sin(p * 0.019), cos(p.zxy * 0.017)) * 100.0;
    // Plateau steps
    float g = p.y * 0.33 + noise2d(p.xz * 0.0003) * 40.0;
    float c = 60.0;
    g /= c;
    float s = fract(g);
    g = floor(g) * c + pow(s, 20.0) * c;
    float d = h + te + g;
    // Carve a tunnel along the camera path
    vec2 o = cameraPath(p.z).xy;
    p.xy -= o;
    float tunnel = 40.0 - length(p.xy);
    return sMax(d, tunnel, 140.0);
}

vec3 getSky(vec3 dir) {
    return mix(FOG_COLOUR, vec3(0.05, 0.14, 0.5), abs(dir.y));
}

vec3 getNormal(vec3 p, float e) {
    return normalize(vec3(
        mapTerrain(p + vec3(e, 0, 0), e) - mapTerrain(p - vec3(e, 0, 0), e),
        mapTerrain(p + vec3(0, e, 0), e) - mapTerrain(p - vec3(0, e, 0), e),
        mapTerrain(p + vec3(0, 0, e), e) - mapTerrain(p - vec3(0, 0, e), e)
    ));
}

float binarySubdivide(vec3 rO, vec3 rD, vec2 t) {
    float halfwayT = 0.0;
    for (int i = 0; i < 8; i++) {
        halfwayT = dot(t, vec2(0.5));
        float d = mapTerrain(rO + halfwayT * rD, halfwayT * 0.002);
        t = mix(vec2(t.x, halfwayT), vec2(halfwayT, t.y), step(0.01, d));
    }
    return halfwayT;
}

float marchScene(vec3 rO, vec3 rD, vec2 co) {
    float t = 5.0 + 10.0 * hash12(co);
    float oldT = 0.0;
    vec2 dist = vec2(1000.0);
    int N = int(marchSteps);
    for (int j = 0; j < 500; j++) {
        if (j >= N) break;
        if (t >= FAR) break;
        vec3 p = rO + t * rD;
        float h = mapTerrain(p, t * 0.002);
        if (h < 0.01) {
            dist = vec2(oldT, t);
            break;
        }
        oldT = t;
        t += h * 0.35 + t * 0.001;
    }
    if (t < FAR) t = binarySubdivide(rO, rD, dist);
    return t;
}

float findClouds2D(vec2 p) {
    float a = 1.0, r = 0.0;
    p *= 0.0015;
    for (int i = 0; i < 5; i++) {
        r += noise2d(p *= 2.2) * a;
        a *= 0.5;
    }
    return max(r - 1.0, 0.0);
}
vec4 getClouds(vec3 pos, vec3 dir) {
    if (dir.y < 0.0) return vec4(0.0);
    float d = (1600.0 / dir.y);
    vec2 p = pos.xz + dir.xz * d;
    float r = findClouds2D(p);
    float t = findClouds2D(p + normalize(g_sunLight.xz) * 15.0);
    t = sqrt(max((r - t) * 20.0, 0.2)) * 0.8;
    return vec4(vec3(t) * SUN_COLOUR, r);
}

vec3 texCubeFallback(vec3 p, vec3 n) {
    // Procedural rock substitute — when no rockTex is bound
    float a = noise3d(p * 8.0);
    float b = noise3d(p * 4.0 + 3.7);
    return mix(vec3(0.55, 0.45, 0.40), vec3(0.45, 0.36, 0.30), a) * (0.7 + 0.5 * b);
}
vec3 texCubeImage(sampler2D tex, vec3 p, vec3 n) {
    vec3 x = textureLod(tex, p.yz, 0.0).xyz;
    vec3 y = textureLod(tex, p.zx, 0.0).xyz;
    vec3 z = textureLod(tex, p.xy, 0.0).xyz;
    return (x * abs(n.x) + y * abs(n.y) + z * abs(n.z)) /
           (1e-20 + abs(n.x) + abs(n.y) + abs(n.z));
}

vec3 albedo(vec3 pos, vec3 nor) {
    g_specular = 0.8;
    vec3 alb;
    if (IMG_SIZE_rockTex.x > 0.0) {
        alb = texCubeImage(rockTex, pos * 0.017, nor).yxz;
    } else {
        alb = texCubeFallback(pos * 0.017, nor);
    }
    float f = noise3d(pos * 0.01);
    alb *= vec3(0.75 + f, 1.0, 0.9);

    float grass = smoothstep(0.1, 0.8, nor.y) * (noise3d(pos * 0.07) + 0.1);
    float v = (noise3d(pos * 0.05) + noise3d(pos * 0.1) * 0.5) * 0.5;
    vec3 grassCol = vec3(0.1 + v, 0.8, 0.1) * 0.7;
    alb = mix(alb, grassCol, grass);
    alb = clamp(alb, 0.0, 1.0);
    g_specular = max(g_specular - grass, 0.0);
    return pow(alb, vec3(1.3));
}

float shadow(vec3 ro, vec3 rd) {
    float res = 1.0;
    float t = 0.1;
    for (int i = 0; i < 10; i++) {
        float h = mapTerrain(ro + rd * t, 1.0);
        res = min(res, 4.0 * h / t);
        t += h + t * 0.01;
        if (res < 0.3) break;
    }
    return clamp(res, 0.3, 1.0);
}
float calcOcc(vec3 pos, vec3 nor) {
    float occ = 0.0, sca = 1.0;
    for (int i = 0; i < 5; i++) {
        float h = 0.1 + float(i);
        float d = mapTerrain(pos + h * nor, 0.0);
        occ += (h - d) * sca;
        sca *= 0.5;
    }
    return clamp(1.0 - occ, 0.0, 1.0);
}

vec3 lighting(vec3 mat_, vec3 pos, vec3 normal, vec3 eyeDir) {
    float sh = shadow(pos + normal * 0.2, g_sunLight);
    vec3 col = mat_ * SUN_COLOUR * max(dot(g_sunLight, normal), 0.0) * sh;
    float occ = calcOcc(pos, normal);
    col += mat_ * SUN_COLOUR * abs(-(normal.y * 0.14)) * occ;
    normal = reflect(eyeDir, normal);
    col += pow(max(dot(g_sunLight, normal), 0.0), 12.0) * SUN_COLOUR * sh * g_specular * occ;
    return min(col, 1.0);
}

vec4 passBufB(vec2 fragCoord) {
    vec2 uv = (-RENDERSIZE + 2.0 * fragCoord) / RENDERSIZE.y;
    g_specular = 0.0;
    g_sunLight = getStore(SUN_DIRECTION).xyz;
    g_camPos   = getStore(CAMERA_POS).xyz;
    g_camMat   = getStoreMat33(CAMERA_MAT0);

    vec3 dir = g_camMat * normalize(vec3(uv, projectZ(uv)));
    vec3 sky = getSky(dir);
    float dhit = marchScene(g_camPos, dir, fragCoord);

    vec3 col;
    if (dhit < FAR) {
        vec3 p = g_camPos + dhit * dir;
        float pixel = RENDERSIZE.y;
        vec3 nor = getNormal(p, dhit / pixel);
        vec3 mat_ = albedo(p, nor);
        col = lighting(mat_, p, nor, dir);
    } else {
        col = sky;
        vec4 cc = getClouds(g_camPos, dir);
        col = mix(col, cc.xyz, cc.w);
        col += pow(max(dot(g_sunLight, dir), 0.0), 200.0) * SUN_COLOUR;
        col = min(col, 1.0);
    }
    col = clamp(col, 0.0, 1.0);
    col = col * 0.6 + col * col * (3.0 - 2.0 * col);
    return vec4(col, dhit);
}

// ──────────────────────────────────────────────────────────────────────
// PASS 2 — Image: god rays + lens flare + vignette + tone curve
// ──────────────────────────────────────────────────────────────────────
float getMist(vec3 dir, vec3 pos) {
    vec3 clou = dir * 1.5 + pos * 0.02;
    float t = noise3d(clou);
    t += noise3d(clou * 2.1) * 0.4;
    t += noise3d(clou * 4.3) * 0.2;
    t += noise3d(clou * 7.9) * 0.1;
    return t;
}

float obscurePartsOfSun(vec2 p) {
    float a = 0.0, z;
    float e = 0.08;
    vec2 asp = vec2(RENDERSIZE.y / RENDERSIZE.x, 1.0);
    vec2 texUV;
    texUV = 0.5 + 0.5 * p * asp;                           z = texture(bufB, texUV).w; if (z >= FAR) a += 0.5;
    texUV = 0.5 + 0.5 * (p + vec2( e,  e)) * asp;          z = texture(bufB, texUV).w; if (z >  FAR) a += 0.125;
    texUV = 0.5 + 0.5 * (p + vec2( e, -e)) * asp;          z = texture(bufB, texUV).w; if (z >  FAR) a += 0.125;
    texUV = 0.5 + 0.5 * (p + vec2(-e, -e)) * asp;          z = texture(bufB, texUV).w; if (z >  FAR) a += 0.125;
    texUV = 0.5 + 0.5 * (p + vec2(-e,  e)) * asp;          z = texture(bufB, texUV).w; if (z >  FAR) a += 0.125;
    return a;
}

float godRays(vec2 uv) {
    float ra = 0.0;
    vec2 sunPos = vec2(dot(g_sunLight, g_camMat[0]), dot(g_sunLight, g_camMat[1])) - vec2(0.05, -0.15);
    vec2 p = uv - sunPos;
    float add = hash12(uv * 4000.0) * 0.02;
    for (float x = 0.1; x < 1.0; x += 0.02) {
        float z = max(textureLod(bufB, (sunPos + p * (x + add) + 1.0) * 0.5, 0.0).w, 300.0) - 300.0;
        ra += z * x;
    }
    return ra * 0.00001;
}

vec3 computeLensFlare(vec2 uv, vec3 dir, mat3 camMat, vec3 sunPos) {
    vec3 col = vec3(0.0);
    mat3 inv = transpose(camMat);
    vec3 cp = inv * (-sunPos);
    if (cp.z < 0.0) {
        vec2 sun2d = g_zProj * cp.xy / cp.z;
        if (sun2d.x < -2.0 || sun2d.x > 2.0 || sun2d.y < -2.0 || sun2d.y > 2.0) return col;
        float z = obscurePartsOfSun(sun2d);
        if (z > 0.0) {
            float bri = max(dot(dir, g_sunLight) * 0.5, 0.0);
            bri = pow(bri, 3.0) * 5.0 * z;
            vec2 uvT = uv - sun2d;
            float glare1 = max(dot(dir, g_sunLight), 0.0);
            uvT = mix(uvT, uv, -2.3);
            float glare2 = max(1.7 - length(uvT + sun2d * 3.0) * 4.0, 0.0);
            float glare3 = max(1.7 - pow(length(uvT + sun2d * 3.5) * 14.0, 200.0), 0.0) * 0.7;
            col += bri * vec3(1.0, 0.0, 0.0)  * pow(glare1, 10.5) * 2.0;
            col += bri * vec3(0.5, 0.05, 0.0) * pow(glare2, 3.0);
            col += bri * vec3(0.1, 0.1, 0.6) * pow(glare3, 3.0) * 3.0;
        }
    }
    return col * 0.8;
}

vec4 passFinal(vec2 fragCoord) {
    vec2 xy = (-RENDERSIZE + 2.0 * fragCoord) / RENDERSIZE;
    vec2 uv = xy * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

    vec4 base = texelFetch(bufB, ivec2(fragCoord), 0);
    vec3 col = base.xyz;
    g_sunLight = getStore(SUN_DIRECTION).xyz;
    g_camPos   = getStore(CAMERA_POS).xyz;
    g_camMat   = getStoreMat33(CAMERA_MAT0);
    g_zProj    = projectZ(uv);

    vec3 dir = g_camMat * normalize(vec3(uv, g_zProj));
    vec3 sunPos = g_sunLight * 20000.0;

    float t = getMist(dir, g_camPos);
    t = mix(1.0, t, exp(-0.00005 * base.w));
    float gr = godRays(xy);
    col += gr * t * SUN_COLOUR * godrayAmt;
    col += computeLensFlare(uv, dir, g_camMat, sunPos) * lensFlare;

    float vig = mix(1.0, smoothstep(4.2, 0.5, dot(uv, uv)), vignetteAmt);
    col *= vig;
    col *= smoothstep(0.0, 4.0, TIME);
    col = min(col * vec3(1.1, 1.0, 0.8), 1.0);
    return vec4(sqrt(col), 1.0);
}

// ──────────────────────────────────────────────────────────────────────
void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    if      (PASSINDEX == 0) FragColor = passBufA(fragCoord);
    else if (PASSINDEX == 1) FragColor = passBufB(fragCoord);
    else                     FragColor = passFinal(fragCoord);
}
