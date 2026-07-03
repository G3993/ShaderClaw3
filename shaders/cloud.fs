/*{
  "DESCRIPTION": "Cloud — a single volumetric blob raymarched in real time: a rotating, self-shadowing cloud of procedural fBm noise lit by a sun with coloured shadow scattering and ambient fill. Ported from a multi-buffer Shadertoy (which baked a 3D density volume into a 2D buffer and sampled a 3D noise texture); rebuilt for Easel as one pass with fully procedural 3D noise so it needs no external textures or buffers.",
  "CREDIT": "Shadertoy volumetric cloud — single-pass procedural ISF port for Easel",
  "CATEGORIES": ["3D", "Generator", "Atmospheric"],
  "INPUTS": [
    { "NAME": "density",    "LABEL": "Density",     "TYPE": "float", "MIN": 8.0,  "MAX": 64.0, "DEFAULT": 32.0 },
    { "NAME": "rotSpeed",   "LABEL": "Rotate",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.5 },
    { "NAME": "colorSpeed", "LABEL": "Color Drift", "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0 },
    { "NAME": "bgColor",    "LABEL": "Sky",         "TYPE": "color", "DEFAULT": [0.5, 0.6, 0.8, 1.0] }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  CLOUD — Easel ISF port. The original stored a 3D density field in a 2D
//  buffer and sampled a 3D noise texture; Easel buffers are half-res 2D, so
//  density is computed PROCEDURALLY inline (fBm value noise) — one pass, no
//  buffers, no sampler3D. Also de-HLSL'd: mat3x3 -> mat3, f-suffixes removed.
// ════════════════════════════════════════════════════════════════════════

#define PI 3.1415927
#define STEPS 60
#define camDist 1.25

const vec3  sunDir          = vec3(0.7, -1.0, -0.4);   // normalized below
const float shadowStepSize0 = 0.0025;
const float shadowStepScalar= 1.15;
const vec3  ambientDensity  = vec3(0.14, 0.13, 0.1) * 3.0;
const vec3  boundingBoxRad  = vec3(0.75 * 0.5);

float saturate(float v) { return clamp(v, 0.0, 1.0); }

// ── procedural 3D value-noise fBm (replaces the noise texture) ──────────
float h31(vec3 p) {
    p = fract(p * 0.1031);
    p += dot(p, p.zyx + 31.32);
    return fract((p.x + p.y) * p.z);
}
float vnoise3(vec3 p) {
    vec3 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float n000 = h31(i + vec3(0, 0, 0)), n100 = h31(i + vec3(1, 0, 0));
    float n010 = h31(i + vec3(0, 1, 0)), n110 = h31(i + vec3(1, 1, 0));
    float n001 = h31(i + vec3(0, 0, 1)), n101 = h31(i + vec3(1, 0, 1));
    float n011 = h31(i + vec3(0, 1, 1)), n111 = h31(i + vec3(1, 1, 1));
    return mix(mix(mix(n000, n100, f.x), mix(n010, n110, f.x), f.y),
               mix(mix(n001, n101, f.x), mix(n011, n111, f.x), f.y), f.z);
}
float Noise(vec3 p) {
    vec3 cp = p * 0.5;
    float v = 0.0, weight = 1.0, tw = 0.0;
    for (int i = 0; i < 5; i++) {
        v  += (vnoise3(cp) * 2.0 - 1.0) * weight;
        tw += weight;
        weight *= 0.5;
        cp = mat3(0.00, 1.60, 1.20, -1.60, 0.72, -0.96, -1.20, -0.96, 1.28) * cp;
    }
    return v / tw;
}
float Density(vec3 p) {
    float v = Noise(p);
    float core = 1.0 - saturate(length(p - vec3(0.5)) * 2.0);
    core = pow(core, 2.0);
    v = (v + core) * core;
    v = saturate(v);
    v = 1.0 - pow(1.0 - v, 2.0);
    v = smoothstep(0.0, 0.2, pow(v, 1.5));
    return v;
}
float SampleDensity(vec3 p) { return Density(p + vec3(0.5)); }

// ── ray/box, hash jitter, camera ────────────────────────────────────────
vec2 boxIntersection(vec3 ro, vec3 rd, vec3 boxSize) {
    vec3 m = 1.0 / rd;
    vec3 n = m * ro;
    vec3 k = abs(m) * boxSize;
    vec3 t1 = -n - k, t2 = -n + k;
    float tN = max(max(t1.x, t1.y), t1.z);
    float tF = min(min(t2.x, t2.y), t2.z);
    if (tN > tF || tF < 0.0) return vec2(-1.0);
    return vec2(tN, tF);
}
float hash12(vec2 src) {
    // Float hash (GLSL ES 1.0 friendly replacement for murmur/uint hash)
    return fract(sin(dot(src, vec2(127.1, 311.7))) * 43758.5453123);
}
void RORD(vec2 fc, vec2 iRes, out vec3 ro, out vec3 rd) {
    ro = vec3(0, 0, camDist);
    float fl = 2.0;
    vec3 cf = normalize(vec3(0.0) - ro);
    vec3 cr = normalize(cross(cf, vec3(0, 1, 0)));
    vec3 cu = normalize(cross(cr, cf));
    vec2 uv = (2.0 * fc - iRes) / iRes.y;
    rd = normalize(uv.x * cr + uv.y * cu + fl * cf);
}

// ── animated rotation of the sample point (the "tumble") ────────────────
mat3 GetPointRot(float centerDist, float time) {
    float dal = mix(0.0, 1.5, pow(saturate(centerDist), 0.5));
    float speedX = 4.0 / 3.0, speedY = 5.0 / 3.0, speedZ = 8.0 / 3.0;
    float tX = time * speedX - dal * 1.5;
    float tY = time * speedY - dal * 3.5;
    float tZ = time * speedZ - dal * 2.5;
    float angleX = pow(sin(tX), 2.0) * 1.8763 * rotSpeed;
    float angleY = pow(sin(tY), 2.0) * 3.3154 * rotSpeed;
    float angleZ = smoothstep(0.0, 1.0, pow(sin(tZ), 2.0)) * 2.5123 * rotSpeed;
    float cx = cos(angleX), sx = sin(angleX);
    float cy = cos(angleY), sy = sin(angleY);
    float cz = cos(angleZ), sz = sin(angleZ);
    mat3 rotX = mat3(1, 0, 0, 0, cx, -sx, 0, sx, cx);
    mat3 rotY = mat3(cy, 0, sy, 0, 1, 0, -sy, 0, cy);
    mat3 rotZ = mat3(cz, -sz, 0, sz, cz, 0, 0, 0, 1);
    return rotX * rotY * rotZ;
}

float ShadowStep(vec3 p, float jitter) {
    vec3 sd = normalize(sunDir);
    vec2 box = boxIntersection(p, -sd, boundingBoxRad);
    if (box.y < 0.0) return 0.0;
    float shadowStep = shadowStepSize0;
    float d = jitter * shadowStep * 0.125;
    float alpha = 0.0;
    bool inside = true;
    for (int i = 0; i < 32; i++) {
        if (!inside) break;
        d += shadowStep;
        float overstep = max(0.0, d - box.y);
        float curStep = shadowStep - overstep;
        float v = SampleDensity(p - d * sd) * curStep * density;
        alpha += v;
        inside = overstep == 0.0;
        shadowStep *= shadowStepScalar;
    }
    return alpha;
}

vec4 Render(vec3 ro, vec3 rd, float rand) {
    vec2 box = boxIntersection(ro, rd, boundingBoxRad);
    if (box.y < 0.0) return vec4(0.0);
    float totDist = box.y - box.x;
    float stepSize = sqrt(dot(boundingBoxRad * 2.0, boundingBoxRad * 2.0)) / float(STEPS);
    int steps = int(ceil(totDist / stepSize)) + 1;

    float t = max(0.0, box.x);
    float transmittance = 1.0;
    vec3 col = vec3(0.0);
    rand = rand * 2.0 - 1.0;
    t += rand * stepSize * 0.5;

    for (int i = 0; i < STEPS + 2; i++) {
        if (i >= steps) break;
        t += stepSize;
        float overstep = max(0.0, t - box.y);
        t = min(t, box.y);
        float curStepSize = stepSize - overstep;

        vec3 p = ro + t * rd;
        p = GetPointRot(length(p), TIME) * p;

        float tCol = TIME * colorSpeed;
        vec3 colPhase = p.z * 2.0 - tCol + vec3(0, 2, 4);
        vec3 shadowDensity = 0.5 + 0.5 * cos(colPhase) * 0.75;

        float v = saturate(SampleDensity(p) * stepSize * density);
        if (v > 0.001) {
            float s = ShadowStep(p, rand);
            vec3 st = exp(-s * shadowDensity);
            col += st * v * transmittance * 0.75;

            float ssa = curStepSize;
            float ambient = SampleDensity(p + vec3(0.0, 0.0, ssa));
            ambient += SampleDensity(p + vec3(0.0, 0.0, 2.0 * ssa));
            ambient += SampleDensity(p + vec3(0.0, 0.0, 3.0 * ssa));
            col += exp(-ambient * ambientDensity) * v * transmittance * 0.25;

            transmittance *= 1.0 - v;
        }
        if (transmittance <= 0.01) { transmittance = 0.0; break; }
    }
    return vec4(col, 1.0 - transmittance);
}

void main() {
    vec3 ro, rd;
    RORD(gl_FragCoord.xy, RENDERSIZE.xy, ro, rd);
    float rand = hash12(gl_FragCoord.xy + TIME);
    vec4 result = Render(ro, rd, rand);
    vec3 bg = bgColor.rgb;
    if (result.a > 0.0) result.rgb /= result.a;
    vec3 col = mix(bg, result.rgb, result.a);
    col = pow(col, vec3(1.0 / 2.2));
    gl_FragColor = vec4(col, 1.0);
}
