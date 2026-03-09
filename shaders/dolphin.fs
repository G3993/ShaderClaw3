/*{
  "DESCRIPTION": "Raymarched dolphin leaping through ocean waves with caustics, foam, and splash effects",
  "CREDIT": "Inigo Quilez (Shadertoy), adapted for ShaderClaw",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "swimSpeed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "camDist", "LABEL": "Camera Dist", "TYPE": "float", "DEFAULT": 4.0, "MIN": 2.0, "MAX": 8.0 },
    { "NAME": "camHeight", "LABEL": "Camera Height", "TYPE": "float", "DEFAULT": 3.1, "MIN": 0.5, "MAX": 6.0 },
    { "NAME": "waveScale", "LABEL": "Wave Scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "sunColor", "LABEL": "Sun Color", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
    { "NAME": "waterTint", "LABEL": "Water Tint", "TYPE": "color", "DEFAULT": [0.0, 0.28, 0.5, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
  ]
}*/

// ---- Procedural noise (replaces texture channels) ----

float hash2D(vec2 p) {
    p = 50.0 * fract(p * 0.3183099);
    return fract(p.x * p.y * (p.x + p.y));
}

float noise2D(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return -1.0 + 2.0 * mix(
        mix(hash2D(i + vec2(0.0, 0.0)), hash2D(i + vec2(1.0, 0.0)), u.x),
        mix(hash2D(i + vec2(0.0, 1.0)), hash2D(i + vec2(1.0, 1.0)), u.x), u.y);
}

// Smooth multi-octave noise simulating a noise texture lookup
float texNoise(vec2 uv) {
    uv *= 8.0;
    float f = 0.0;
    f += 0.5000 * (0.5 + 0.5 * noise2D(uv)); uv *= 2.01;
    f += 0.2500 * (0.5 + 0.5 * noise2D(uv)); uv *= 2.02;
    f += 0.1250 * (0.5 + 0.5 * noise2D(uv)); uv *= 2.03;
    f += 0.0625 * (0.5 + 0.5 * noise2D(uv));
    return f / 0.9375;
}

vec3 texNoise3(vec2 uv) {
    return vec3(
        texNoise(uv),
        texNoise(uv + vec2(7.13, 3.71)),
        texNoise(uv + vec2(13.37, 17.53))
    );
}

// ---- Distance functions ----

vec2 sd2Segment(vec3 a, vec3 b, vec3 p) {
    vec3 pa = p - a;
    vec3 ba = b - a;
    float t = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    vec3 v = pa - ba * t;
    return vec2(dot(v, v), t);
}

float udRoundBox(vec3 p, vec3 b, float r) {
    return length(max(abs(p) - b, 0.0)) - r;
}

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

float almostIdentity(float x, float m, float n) {
    if (x > m) return x;
    float a = 2.0 * n - m;
    float b = 2.0 * m - 3.0 * n;
    float t = x / m;
    return (a * t + b) * t * t + n;
}

float almostAbs(float x) {
    return 1.0 - x * x;
}

// ---- Dolphin animation state ----

#define NUMI 11
#define NUMF 11.0

vec3 fishPos;
float fishTime;
float isJump;
float isJump2;

vec2 anima(float ih, float t) {
    float an1 = 0.9 * (0.5 + 0.2 * ih) * cos(5.0 * ih - 3.0 * t + 6.2831 / 4.0);
    float an2 = 1.0 * cos(3.5 * ih - 1.0 * t + 6.2831 / 4.0);
    float an = mix(an1, an2, isJump);
    float ro = 0.4 * cos(4.0 * ih - 1.0 * t) * (1.0 - 0.5 * isJump);
    return vec2(an, ro);
}

vec3 anima2(void) {
    vec3 a1 = vec3(0.0, sin(3.0 * fishTime + 6.2831 / 4.0), 0.0);
    vec3 a2 = vec3(0.0, 1.5 + 2.5 * cos(1.0 * fishTime), 0.0);
    vec3 a = mix(a1, a2, isJump);
    a.y *= 0.5;
    a.x += 0.1 * sin(0.1 - 1.0 * fishTime) * (1.0 - isJump);
    return a;
}

// Simple dolphin for collision detection
float sdDolphinCheap(vec3 p) {
    p -= fishPos;
    vec3 a = anima2();
    float res = 100000.0;
    for (int i = 0; i < NUMI; i++) {
        float ih = float(i) / NUMF;
        vec2 anim = anima(ih, fishTime);
        float ll = 0.48;
        if (i == 0) ll = 0.655;
        vec3 b = a + ll * normalize(vec3(sin(anim.y), sin(anim.x), cos(anim.x)));
        vec2 dis = sd2Segment(a, b, p);
        float h = ih + dis.y / NUMF;
        float ra = 0.04 + h * (1.0 - h) * (1.0 - h) * 2.7;
        res = min(res, sqrt(dis.x) - ra);
        a = b;
    }
    return 0.75 * res;
}

vec3 ccd, ccp;

vec2 sdDolphin(vec3 p) {
    vec2 res = vec2(1000.0, 0.0);
    p -= fishPos;
    vec3 a = anima2();

    vec3 p1 = a, d1 = vec3(0.0);
    vec3 p2 = a, d2 = vec3(0.0);
    vec3 p3 = a, d3 = vec3(0.0);
    vec3 mp = a;

    for (int i = 0; i < NUMI; i++) {
        float ih = float(i) / NUMF;
        vec2 anim = anima(ih, fishTime);
        float ll = 0.48;
        if (i == 0) ll = 0.655;
        vec3 b = a + ll * normalize(vec3(sin(anim.y), sin(anim.x), cos(anim.x)));
        vec2 dis = sd2Segment(a, b, p);
        if (dis.x < res.x) {
            res = vec2(dis.x, ih + dis.y / NUMF);
            mp = a + (b - a) * dis.y;
            ccd = b - a;
        }
        if (i == 3) { p1 = a; d1 = b - a; }
        if (i == 4) { p3 = a; d3 = b - a; }
        if (i == (NUMI - 1)) { p2 = b; d2 = b - a; }
        a = b;
    }
    ccp = mp;

    float h = res.y;
    float ra = 0.05 + h * (1.0 - h) * (1.0 - h) * 2.7;
    ra += 7.0 * max(0.0, h - 0.04) * exp(-30.0 * max(0.0, h - 0.04)) * smoothstep(-0.1, 0.1, p.y - mp.y);
    ra -= 0.03 * smoothstep(0.0, 0.1, abs(p.y - mp.y)) * (1.0 - smoothstep(0.0, 0.1, h));
    ra += 0.05 * clamp(1.0 - 3.0 * h, 0.0, 1.0);
    ra += 0.035 * (1.0 - smoothstep(0.0, 0.025, abs(h - 0.1))) * (1.0 - smoothstep(0.0, 0.1, abs(p.y - mp.y)));

    // body
    res.x = 0.75 * (distance(p, mp) - ra);

    // dorsal fin
    d3 = normalize(d3);
    float k = sqrt(1.0 - d3.y * d3.y);
    mat3 ms = mat3(d3.z / k, -d3.x * d3.y / k, d3.x,
                   0.0, k, d3.y,
                   -d3.x / k, -d3.y * d3.z / k, d3.z);
    vec3 ps = ms * (p - p3);
    ps.z -= 0.1;
    float d5 = length(ps.yz) - 0.9;
    d5 = max(d5, -(length(ps.yz - vec2(0.6, 0.0)) - 0.35));
    d5 = max(d5, udRoundBox(ps + vec3(0.0, -0.5, 0.5), vec3(0.0, 0.5, 0.5), 0.02));
    res.x = smin(res.x, d5, 0.1);

    // pectoral fins
    d1 = normalize(d1);
    k = sqrt(1.0 - d1.y * d1.y);
    ms = mat3(d1.z / k, -d1.x * d1.y / k, d1.x,
              0.0, k, d1.y,
              -d1.x / k, -d1.y * d1.z / k, d1.z);
    ps = ms * (p - p1);
    ps.x = abs(ps.x);
    float l = ps.x;
    l = clamp((l - 0.4) / 0.5, 0.0, 1.0);
    l = 4.0 * l * (1.0 - l);
    l *= 1.0 - clamp(5.0 * abs(ps.z + 0.2), 0.0, 1.0);
    ps.xyz += vec3(-0.2, 0.36, -0.2);
    d5 = length(ps.xz) - 0.8;
    d5 = max(d5, -(length(ps.xz - vec2(0.2, 0.4)) - 0.8));
    d5 = max(d5, udRoundBox(ps, vec3(1.0, 0.0, 1.0), 0.015 + 0.05 * l));
    res.x = smin(res.x, d5, 0.12);

    // tail flukes
    d2 = normalize(d2);
    mat2 mf = mat2(d2.z, d2.y, -d2.y, d2.z);
    vec3 pf = p - p2 - d2 * 0.25;
    pf.yz = mf * pf.yz;
    float d4 = length(pf.xz) - 0.6;
    d4 = max(d4, -(length(pf.xz - vec2(0.0, 0.8)) - 0.9));
    d4 = max(d4, udRoundBox(pf, vec3(1.0, 0.005, 1.0), 0.005));
    res.x = smin(res.x, d4, 0.1);

    return res;
}

// ---- Water ----

const mat2 m2 = mat2(0.80, -0.60, 0.60, 0.80);

vec2 sdWaterCheap(vec3 p) {
    vec2 q = 0.1 * p.xz;
    float t = TIME * swimSpeed;
    float f = 0.0;
    f += 0.50000 * almostAbs(noise2D(q)); q = m2 * q * 2.02; q -= 0.1 * t;
    f += 0.25000 * almostAbs(noise2D(q)); q = m2 * q * 2.03; q += 0.2 * t;
    f += 0.12500 * almostAbs(noise2D(q)); q = m2 * q * 2.01; q -= 0.4 * t;
    f += 0.06250 * almostAbs(noise2D(q)); q = m2 * q * 2.02; q += 1.0 * t;
    f += 0.03125 * almostAbs(noise2D(q));
    f *= waveScale;
    return vec2(1.8 - 2.0 * f, f);
}

vec3 sdWater(vec3 p) {
    vec3 w;
    w.xy = sdWaterCheap(p);

    // splash from dolphin collision
    float sss = abs(sdDolphinCheap(p));
    float spla = exp(-4.0 * sss);
    spla += 0.5 * exp(-14.0 * sss);
    spla *= mix(1.0, texNoise(0.2 * p.xz), spla * spla);
    spla *= -0.85;
    spla *= isJump;
    spla *= mix(1.0, smoothstep(0.0, 0.5, p.z - fishPos.z - 1.5), isJump2);

    w.z = sss;
    w.x = p.y - w.x + spla;
    return w;
}

// ---- Raymarching ----

vec2 intersectDolphin(vec3 ro, vec3 rd) {
    const float maxd = 10.0;
    const float precis = 0.001;
    float t = 0.0;
    float l = 0.0;
    for (int i = 0; i < 128; i++) {
        vec2 res = sdDolphin(ro + rd * t);
        float h = res.x;
        l = res.y;
        if (h < precis || t > maxd) break;
        t += h;
    }
    if (t > maxd) t = -1.0;
    return vec2(t, l);
}

vec3 intersectWater(vec3 ro, vec3 rd) {
    const float precis = 0.001;
    float l = 0.0;
    float s = 0.0;
    float t = (2.5 - ro.y) / rd.y;
    if (t < 0.0) return vec3(-1.0);
    for (int i = 0; i < 12; i++) {
        vec3 res = sdWater(ro + rd * t);
        l = res.y;
        s = res.z;
        if (abs(res.x) < precis) break;
        t += res.x;
    }
    return vec3(t, l, s);
}

// ---- Normals ----

vec3 calcNormalFish(vec3 pos) {
    const float eps = 0.08;
    const vec3 e0 = vec3( 0.5773,  0.5773,  0.5773);
    const vec3 e1 = vec3(-0.5773, -0.5773,  0.5773);
    const vec3 e2 = vec3(-0.5773,  0.5773, -0.5773);
    const vec3 e3 = vec3( 0.5773, -0.5773, -0.5773);
    return normalize(
        e0 * sdDolphin(pos + eps * e0).x +
        e1 * sdDolphin(pos + eps * e1).x +
        e2 * sdDolphin(pos + eps * e2).x +
        e3 * sdDolphin(pos + eps * e3).x
    );
}

vec3 calcNormalWater(vec3 pos) {
    const vec3 eps = vec3(0.025, 0.0, 0.0);
    float v = sdWater(pos).x;
    return normalize(vec3(sdWater(pos + eps.xyy).x - v, eps.x, sdWater(pos + eps.yyx).x - v));
}

// ---- Shadows ----

float softshadow(vec3 ro, vec3 rd, float mint, float k) {
    float res = 1.0;
    float t = mint;
    for (int i = 0; i < 25; i++) {
        float h = sdDolphinCheap(ro + rd * t);
        res = min(res, k * h / t);
        t += clamp(h, 0.05, 0.5);
        if (h < 0.0001) break;
    }
    return clamp(res, 0.0, 1.0);
}

// ---- Lighting ----

const vec3 lig = vec3(0.86, 0.15, 0.48);

vec3 doLighting(vec3 pos, vec3 nor, vec3 rd, float glossy, float glossy2, float shadows, vec3 col, float occ) {
    vec3 hal = normalize(lig - rd);
    vec3 ref = reflect(rd, nor);

    float sky = clamp(nor.y, 0.0, 1.0);
    float bou = clamp(-nor.y, 0.0, 1.0);
    float dif = max(dot(nor, lig), 0.0);
    float bac = max(0.3 + 0.7 * dot(nor, -vec3(lig.x, 0.0, lig.z)), 0.0);
    float sha = 1.0 - shadows;
    if ((shadows * dif) > 0.001) sha = softshadow(pos + 0.01 * nor, lig, 0.0005, 32.0);
    float fre = pow(clamp(1.0 + dot(nor, rd), 0.0, 1.0), 5.0);
    float spe = max(0.0, pow(clamp(dot(hal, nor), 0.0, 1.0), 0.01 + glossy));
    float sss = pow(clamp(1.0 + dot(nor, rd), 0.0, 1.0), 2.0);

    float shr = 1.0;
    if (shadows > 0.0) shr = softshadow(pos + 0.01 * nor, normalize(ref + vec3(0.0, 1.0, 0.0)), 0.0005, 8.0);

    vec3 brdf = vec3(0.0);
    brdf += 20.0 * dif * vec3(4.00, 2.20, 1.40) * vec3(sha, sha * 0.5 + 0.5 * sha * sha, sha * sha);
    brdf += 11.0 * sky * vec3(0.20, 0.40, 0.55) * (0.5 + 0.5 * occ);
    brdf += 1.0 * bac * vec3(0.40, 0.60, 0.70);
    brdf += 11.0 * bou * vec3(0.05, 0.30, 0.50);
    brdf += 5.0 * sss * vec3(0.40, 0.40, 0.40) * (0.3 + 0.7 * dif * sha) * glossy * occ;
    brdf += 0.8 * spe * vec3(1.30, 1.00, 0.90) * sha * dif * (0.1 + 0.9 * fre) * glossy * glossy;
    brdf += shr * 40.0 * glossy * vec3(1.0) * occ * smoothstep(-0.3 + 0.3 * glossy2, 0.2, ref.y)
        * (0.5 + 0.5 * smoothstep(-0.2 + 0.2 * glossy2, 1.0, ref.y)) * (0.04 + 0.96 * fre);
    col = col * brdf;
    col += shr * (0.1 + 1.6 * fre) * occ * glossy2 * glossy2 * 40.0 * vec3(1.0, 0.9, 0.8)
        * smoothstep(0.0, 0.2, ref.y) * (0.5 + 0.5 * smoothstep(0.0, 1.0, ref.y));
    col += 1.2 * glossy * pow(spe, 4.0) * vec3(1.4, 1.1, 0.9) * sha * dif * (0.04 + 0.96 * fre) * occ;

    return col;
}

vec3 normalMap(vec2 pos) {
    float v = texNoise(0.03 * pos);
    return normalize(vec3(
        v - texNoise(0.03 * pos + vec2(0.001, 0.0)),
        1.0 / 16.0,
        v - texNoise(0.03 * pos + vec2(0.0, 0.001))
    ));
}

// ---- Main ----

void main() {
    vec2 q = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = -1.0 + 2.0 * q;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t_speed = TIME * swimSpeed;

    // Animate
    fishTime = 0.6 + 2.0 * t_speed - 20.0;
    fishPos = vec3(0.0, -0.2, -1.1 * fishTime);
    isJump = 0.5 + 0.5 * cos(-0.4 + 0.5 * fishTime);
    isJump2 = 0.5 + 0.5 * cos(0.6 + 0.5 * fishTime);

    // Camera
    vec2 m = vec2(0.5);
    if (mouseDown > 0.5) m = mousePos;
    float an = 1.2 + 0.1 * t_speed - 12.0 * (m.x - 0.5);

    vec3 ta = vec3(fishPos.x, 0.8, fishPos.z) - vec3(0.0, 0.0, -2.0);
    vec3 ro = ta + vec3(camDist * sin(an), camHeight, camDist * cos(an));

    // Camera shake
    ro += 0.05 * sin(4.0 * t_speed * vec3(1.1, 1.2, 1.3) + vec3(3.0, 0.0, 1.0));
    ta += 0.05 * sin(4.0 * t_speed * vec3(1.7, 1.5, 1.6) + vec3(1.0, 2.0, 1.0));

    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(vec3(-ww.z, 0.0, ww.x));
    vec3 vv = normalize(cross(uu, ww));
    vec3 rd = normalize(p.x * uu + p.y * vv + 2.0 * ww * (1.0 + 0.7 * smoothstep(-0.4, 0.4, sin(0.34 * t_speed))));

    // ---- Render ----
    float t = 1000.0;
    vec3 col = vec3(0.0);
    vec3 bgcol = vec3(0.6, 0.7, 0.8) - 0.2 * clamp(rd.y, 0.0, 1.0);

    // Acceleration bounding plane
    float pt = (3.2 - ro.y) / rd.y;
    if (rd.y < 0.0 && pt > 0.0) ro = ro + rd * pt;

    // Raymarch dolphin
    vec2 tmat1 = intersectDolphin(ro, rd);
    vec3 posy = vec3(-100000.0);

    if (tmat1.x > 0.0) {
        t = tmat1.x;
        vec3 pos = ro + tmat1.x * rd;
        vec3 nor = calcNormalFish(pos);
        vec3 fpos = pos - fishPos;

        vec3 auu = normalize(vec3(-ccd.z, 0.0, ccd.x));
        vec3 avv = normalize(cross(ccd, auu));
        vec3 ppp = vec3(dot(fpos - ccp, auu), dot(fpos - ccp, avv), tmat1.y);
        vec2 uv = vec2(1.0 * atan(ppp.x, ppp.y) / 3.1416, 4.0 * ppp.z);

        // Procedural bump
        vec3 bnor = -1.0 + 2.0 * texNoise3(uv);
        nor += 0.01 * bnor;

        vec3 te = texNoise3(uv);
        vec4 mate;
        mate.w = 10.0;
        mate.xyz = mix(vec3(0.3, 0.38, 0.46) * 0.6, vec3(0.8, 0.9, 1.0),
                       smoothstep(-0.05, 0.05, ppp.y - tmat1.y * 0.5 + 0.1));
        mate.xyz *= 1.0 + 0.3 * te;
        mate.xyz *= smoothstep(0.0, 0.06,
                    distance(vec3(abs(ppp.x), ppp.yz) * vec3(1.0, 1.0, 4.0), vec3(0.35, 0.0, 0.4)));
        mate.xyz *= 1.0 - 0.75 * (1.0 - smoothstep(0.0, 0.02, abs(ppp.y))) * (1.0 - smoothstep(0.07, 0.11, tmat1.y));
        mate.xyz *= 0.1 * 0.23 * 0.6;
        mate.w *= (0.7 + 0.3 * te.x) * smoothstep(0.0, 0.01, pos.y - sdWaterCheap(pos).x);

        col = doLighting(pos, nor, rd, mate.w, 0.0, 0.0, mate.xyz, 1.0);
        posy = pos;
    }

    // Raymarch water
    vec3 tmat2 = intersectWater(ro, rd);
    if (tmat2.x > 0.0 && (tmat1.x < 0.0 || tmat2.x < tmat1.x)) {
        t = tmat2.x;
        vec3 pos = ro + tmat2.x * rd;
        vec3 nor = calcNormalWater(pos);
        vec3 ref = reflect(rd, nor);
        nor = normalize(nor + 0.15 * normalMap(pos.xz));
        float fre = pow(clamp(1.0 + dot(rd, nor), 0.0, 1.0), 2.0);

        // Water material
        vec4 mate;
        mate.xyz = 0.05 * mix(waterTint.rgb * 0.8, waterTint.rgb,
                   (1.0 - smoothstep(0.2, 0.8, tmat2.y)) * (0.5 + 0.5 * fre));
        mate.w = fre;

        // Foam
        float foam = 1.0 - smoothstep(0.4, 0.6, tmat2.y);
        foam *= abs(nor.z) * 2.0;
        foam *= clamp(1.0 - 2.0 * texNoise(vec2(1.0, 0.75) * 0.31 * pos.xz), 0.0, 1.0);
        mate = mix(mate, vec4(0.1 * 0.2, 0.11 * 0.2, 0.13 * 0.2, 0.5), foam);
        float al = clamp(0.5 + 0.2 * (pos.y - posy.y), 0.0, 1.0);

        // Splash foam
        float sfoam = exp(-3.0 * abs(tmat2.z));
        sfoam *= texNoise(pos.zx);
        sfoam = clamp(sfoam * 3.0, 0.0, 1.0);
        sfoam *= isJump;
        sfoam *= mix(1.0, smoothstep(0.0, 0.5, pos.z - fishPos.z - 1.5), isJump2);
        mate.xyz = mix(mate.xyz, vec3(0.9, 0.95, 1.0) * 0.05, sfoam * sfoam);

        col = mix(col, vec3(0.9, 0.95, 1.0) * 1.2, sfoam);
        al *= 1.0 - sfoam;

        float occ = clamp(3.5 * sdDolphinCheap(pos + vec3(0.0, 0.4, 0.0))
                        * sdDolphinCheap(pos + vec3(0.0, 1.0, 0.0)), 0.0, 1.0);
        occ = mix(1.0, occ, isJump);
        occ = 0.35 + 0.65 * occ;
        mate.xyz *= occ;
        col *= occ;

        mate.xyz = doLighting(pos, nor, rd, mate.w * 10.0, mate.w * 0.5, 1.0, mate.xyz, occ);

        // Caustics on dolphin
        float cc = 0.65 * texNoise(2.5 * 0.02 * posy.xz + 0.007 * t_speed * vec2(1.0, 0.0));
        cc += 0.35 * texNoise(1.8 * 0.04 * posy.xz + 0.011 * t_speed * vec2(0.0, 1.0));
        cc = 0.6 * (1.0 - smoothstep(0.0, 0.05, abs(cc - 0.5)))
           + 0.4 * (1.0 - smoothstep(0.0, 0.20, abs(cc - 0.5)));
        col *= 1.0 + 0.8 * cc;

        col = mix(col, mate.xyz, al);
    }

    // Sun
    float sun = pow(max(0.0, dot(lig, rd)), 8.0);
    col += sunColor.rgb * sun * 0.3;

    // Gamma
    col = pow(clamp(col, 0.0, 1.0), vec3(0.45));

    // Color grading
    col = col * 0.5 + 0.5 * col * col * (3.0 - 2.0 * col);

    // Vignette
    col *= 0.5 + 0.5 * pow(16.0 * q.x * q.y * (1.0 - q.x) * (1.0 - q.y), 0.1);

    // Fade in
    col *= smoothstep(0.0, 1.0, TIME);

    float alpha = 1.0;
    if (transparentBg) {
        alpha = clamp(dot(col, vec3(0.299, 0.587, 0.114)) * 2.0, 0.0, 1.0);
    }

    gl_FragColor = vec4(col, alpha);
}
