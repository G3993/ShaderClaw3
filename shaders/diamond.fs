/*{
  "DESCRIPTION": "Anomalous Dispersion — brilliant-cut diamond with spectral refraction and total internal reflection",
  "CREDIT": "ShaderClaw (dispersion model from Shadertoy, diamond DF by TambakoJaguar)",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "dispersion", "LABEL": "Dispersion", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "iorBase", "LABEL": "IOR", "TYPE": "float", "DEFAULT": 0.414, "MIN": 0.1, "MAX": 0.8 },
    { "NAME": "anomalyScale", "LABEL": "Anomaly Scale", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "anomalySharpness", "LABEL": "Anomaly Sharp", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 16.0 },
    { "NAME": "rotSpeed", "LABEL": "Rotate Speed", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "envBrightness", "LABEL": "Environment", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
  ]
}*/

// performance and raymarching options
#define WAVELENGTHS 7
#define INTERSECTION_PRECISION 0.001
#define MIN_INCREMENT 0.01
#define ITERATIONS 80
#define MAX_BOUNCES 5
#define AA_SAMPLES 1
#define BOUND 6.0
#define DIST_SCALE 1.0
#define DIAMOND_RADIUS 4.2

// optical properties
#define CRIT_ANGLE_SCALE 1.0
#define CRIT_ANGLE_SHARPNESS 2.0
#define BOUNCE_ATTENUATION_SCALE 0.5

#define TWO_PI 6.28318530718
#define PI 3.14159265359

// ---- Procedural environment (replaces cubemap) ----

// Hash for procedural stars
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// Pre-normalized light directions (avoid per-pixel normalize)
const vec3 KEY_DIR  = vec3(0.6963, 0.5571, 0.4178);
const vec3 FILL_DIR = vec3(-0.8321, 0.5199, -0.3120);
const vec3 RIM_DIR  = vec3(0.0, -0.5145, -0.8575);
const vec3 SP1_DIR  = vec3(0.4264, 0.8528, 0.2559);
const vec3 SP2_DIR  = vec3(-0.3352, 0.7821, 0.8938);
const vec3 SP3_DIR  = vec3(0.8321, 0.2080, -0.5199);

// Procedural HDR-like environment (optimized — fewer pow, precomputed dirs)
vec3 envMap(vec3 rd) {
    float sky = 0.5 + 0.5 * rd.y;
    vec3 col = mix(vec3(0.02, 0.02, 0.04), vec3(0.08, 0.06, 0.12), sky);

    // Studio lights — use exp2 trick: pow(x, n) ≈ exp2(n * log2(x))
    float kd = max(0.0, dot(rd, KEY_DIR));
    float k = kd * kd; k *= k; k *= k; k *= k; k *= k; k *= k; // ^64 via squaring
    col += vec3(1.0, 0.95, 0.85) * k * 3.0;

    float fd = max(0.0, dot(rd, FILL_DIR));
    float f = fd * fd; f *= f; f *= f; f *= f; f *= fd; // ^32 approx
    col += vec3(0.6, 0.7, 1.0) * f * 1.5;

    float rd2 = max(0.0, dot(rd, RIM_DIR));
    float r = rd2 * rd2; r *= r; r *= r; r *= r; // ^16
    col += vec3(0.91, 0.25, 0.34) * r * 2.0;

    // Ground band
    col += vec3(0.15, 0.12, 0.1) * exp(-8.0 * rd.y * rd.y) * step(rd.y, 0.0);

    // Sparkle points — high power via exp2
    float s1d = max(0.0, dot(rd, SP1_DIR));
    float s2d = max(0.0, dot(rd, SP2_DIR));
    float s3d = max(0.0, dot(rd, SP3_DIR));
    float s1 = exp2(256.0 * log2(s1d + 0.0001));
    float s2 = exp2(256.0 * log2(s2d + 0.0001));
    float s3 = exp2(256.0 * log2(s3d + 0.0001));
    col += vec3(1.0) * (s1 + s2 + s3) * 5.0;

    return col * envBrightness;
}

// Sample environment and return per-wavelength intensity
float sampleEnv(float i, vec3 rd) {
    vec3 col = envMap(rd);
    vec3 w = vec3((1.0 - i) * (1.0 - i), 2.0 * i * (1.0 - i), i * i);
    return dot(w, col);
}

// ---- Diamond distance field (brilliant cut) ----

float dist(vec3 pos) {
    vec3 posr = pos;
    float d = 0.94;
    float b = 0.5;
    float af2 = 4.0 / PI;
    float s = atan(posr.y, posr.x);
    float sf = floor(s * af2 + b) / af2;
    float sf2 = floor(s * af2) / af2;

    vec3 flatvec  = vec3(cos(sf), sin(sf), 1.444);
    vec3 flatvec2 = vec3(cos(sf), sin(sf), -1.072);
    vec3 flatvec3 = vec3(cos(s), sin(s), 0.0);
    float csf1 = cos(sf + 0.21);
    float csf2 = cos(sf - 0.21);
    float ssf1 = sin(sf + 0.21);
    float ssf2 = sin(sf - 0.21);
    vec3 flatvec4 = vec3(csf1, ssf1, -1.02);
    vec3 flatvec5 = vec3(csf2, ssf2, -1.02);
    vec3 flatvec6 = vec3(csf2, ssf2, 1.03);
    vec3 flatvec7 = vec3(csf1, ssf1, 1.03);
    vec3 flatvec8 = vec3(cos(sf2 + 0.393), sin(sf2 + 0.393), 2.21);

    float d1 = dot(flatvec, posr) - d;
    d1 = max(dot(flatvec2, posr) - d, d1);
    d1 = max(dot(vec3(0.0, 0.0, 1.0), posr) - 0.3, d1);
    d1 = max(dot(vec3(0.0, 0.0, -1.0), posr) - 0.865, d1);
    d1 = max(dot(flatvec3, posr) - 0.911, d1);
    d1 = max(dot(flatvec4, posr) - 0.9193, d1);
    d1 = max(dot(flatvec5, posr) - 0.9193, d1);
    d1 = max(dot(flatvec6, posr) - 0.912, d1);
    d1 = max(dot(flatvec7, posr) - 0.912, d1);
    d1 = max(dot(flatvec8, posr) - 1.131, d1);
    return d1;
}

// ---- Fresnel (Schlick approximation) ----

float fresnel(vec3 ray, vec3 norm, float n2) {
    float n1 = 1.0;
    float angle = clamp(acos(-dot(ray, norm)), -3.14 / 2.15, 3.14 / 2.15);
    float r0 = pow((n1 - n2) / (n1 + n2), 2.0);
    float r = r0 + (1.0 - r0) * pow(1.0 - cos(angle), 5.0);
    return clamp(r, 0.0, 1.0);
}

float doModel(vec3 p) {
    return dist(p / 4.0);
}

vec3 calcNormal(vec3 pos) {
    const float eps = INTERSECTION_PRECISION;
    float d = doModel(pos);
    return normalize(vec3(
        doModel(pos + vec3(eps, 0.0, 0.0)) - d,
        doModel(pos + vec3(0.0, eps, 0.0)) - d,
        doModel(pos + vec3(0.0, 0.0, eps)) - d
    ));
}

// ---- Bounding sphere test (skip empty space) ----
// Returns t of nearest intersection with sphere of radius r centered at origin, or -1.0
float sphereIntersect(vec3 ro, vec3 rd, float r) {
    float b = dot(ro, rd);
    float c = dot(ro, ro) - r * r;
    float h = b * b - c;
    if (h < 0.0) return -1.0;
    return -b - sqrt(h);
}

// ---- Bounce state ----

struct Bounce {
    vec3 position;
    vec3 ray_direction;
    float attenuation;
    float reflectance;
    float ior;
    float bounces;
    float wavelength;
};

float sigmoid(float t, float t0, float k) {
    return 1.0 / (1.0 + exp(-exp(k) * (t - t0)));
}

float filmic_gamma(float x) {
    return (x * (x * 6.2 + 0.5)) / (x * (x * 6.2 + 1.7) + 0.06);
}

vec3 filmic_gamma_v(vec3 x) {
    return (x * (x * 6.2 + 0.5)) / (x * (x * 6.2 + 1.7) + 0.06);
}

float filmic_gamma_inverse(float x) {
    x = clamp(x, 0.0, 0.99);
    return (0.0016129 * (-950.329 + 1567.48 * x + 85.0 * sqrt(125.0 - 106.0 * x + 701.0 * x * x)))
        / (26.8328 - sqrt(125.0 - 106.0 * x + 701.0 * x * x));
}

// ---- Bounce logic ----

float doBounce(inout Bounce b) {
    float td = doModel(b.position);
    float t = DIST_SCALE * abs(td);
    float sig = sign(td);

    vec3 pos = b.position + t * b.ray_direction;

    if (clamp(pos, -BOUND, BOUND) != pos || (sig > 0.0 && b.bounces > 1.0) || int(b.bounces) >= MAX_BOUNCES) {
        return -1.0;
    }

    if (t < INTERSECTION_PRECISION) {
        vec3 normal = calcNormal(pos);
        b.attenuation *= pow(abs(dot(b.ray_direction, normal)), BOUNCE_ATTENUATION_SCALE / (b.bounces + 1.0));

        if (sig == -1.0) {
            // Inside diamond
            float angle = abs(acos(dot(b.ray_direction, normal)));
            float critical_angle = abs(asin(b.ior)) * CRIT_ANGLE_SCALE;

            vec3 refl = reflect(b.ray_direction, normal);
            vec3 refr = refract(b.ray_direction, normal, 1.0 / b.ior);
            float k = sigmoid(angle, critical_angle, CRIT_ANGLE_SHARPNESS);
            b.ray_direction = normalize(mix(refr, refl, vec3(k)));
        } else {
            // Outside — entering
            float f = fresnel(b.ray_direction, normal, 1.0 / b.ior);
            float envSample = sampleEnv(b.wavelength, reflect(b.ray_direction, normal));
            b.reflectance += filmic_gamma_inverse(mix(0.0, envSample, f));
            b.ray_direction = refract(b.ray_direction, normal, b.ior);
        }

        b.position = pos + MIN_INCREMENT * b.ray_direction;
        b.bounces += 1.0;
    } else {
        b.position = pos;
    }

    return 1.0;
}

// ---- IOR curve with anomaly ----

float iorCurve(float x) {
    return x - sin(0.5 * TIME) * sign(x - 0.5) * anomalyScale / pow(1.0 + abs(x - 0.5), anomalySharpness);
}

Bounce initBounce(vec3 ro, vec3 rd, float i, float sphereT) {
    float idx = i / float(WAVELENGTHS - 1);
    float ior = iorBase + iorCurve(1.0 - idx) * sin(TIME * 0.67) * dispersion;
    // Advance to bounding sphere to skip empty space
    vec3 startPos = (sphereT > 0.0) ? ro + rd * (sphereT - 0.01) : ro;
    return Bounce(startPos, rd, 1.0, 0.0, ior, 1.0, idx);
}

// ---- Green weight for spectral downsampling (4PL fit) ----

float greenWeight() {
    float a = 4569547.0;
    float b = 2.899324;
    float c = 0.008024607;
    float d = 0.07336188;
    return d + (a - d) / (1.0 + pow(log(float(WAVELENGTHS)) / c, b)) + 2.0;
}

vec3 sampleWeights(float i) {
    return vec3((1.0 - i) * (1.0 - i), greenWeight() * i * (1.0 - i), i * i);
}

// ---- Downsample to RGB ----

vec3 resampleColor(Bounce b[WAVELENGTHS]) {
    vec3 col = vec3(0.0);
    for (int i = 0; i < WAVELENGTHS; i++) {
        float index = float(i) / float(WAVELENGTHS - 1);
        float envIntensity = filmic_gamma_inverse(
            clamp(b[i].attenuation * sampleEnv(index, b[i].ray_direction), 0.0, 0.99)
        );
        float intensity = envIntensity + b[i].reflectance;
        col += sampleWeights(index) * intensity;
    }
    return 1.4 * filmic_gamma_v(3.0 * col / float(WAVELENGTHS));
}

// ---- Camera ----

void doCamera(out vec3 camPos, out vec3 camTar) {
    float an = 1.5 + sin(-TIME * rotSpeed - 0.38) * 4.0;
    float bn = -2.0 * cos(-TIME * rotSpeed - 0.38);

    // Mouse override
    if (mouseDown > 0.5) {
        an = 10.0 * mousePos.x - 5.0;
        bn = 10.0 * mousePos.y - 5.0;
    }

    camPos = vec3(6.5 * sin(an), bn, 6.5 * cos(an));
    camTar = vec3(0.0);
}

mat3 calcLookAtMatrix(vec3 ro, vec3 ta, float roll) {
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(sin(roll), cos(roll), 0.0)));
    vec3 vv = normalize(cross(uu, ww));
    return mat3(uu, vv, ww);
}

// ---- Main ----

void main() {
    vec2 p = (-RENDERSIZE.xy + 2.0 * gl_FragCoord.xy) / RENDERSIZE.y;

    vec3 ro, ta;
    doCamera(ro, ta);
    mat3 camMat = calcLookAtMatrix(ro, ta, 0.0);

    float dh = 0.5 / RENDERSIZE.y;

    Bounce bounces[WAVELENGTHS];

    vec3 col = vec3(0.0);
    float mask = 0.0;

    for (int samp = 0; samp < AA_SAMPLES; samp++) {
        float sampF = float(samp);
        vec2 dxy = dh * vec2(cos(sampF * TWO_PI / float(AA_SAMPLES)),
                             sin(sampF * TWO_PI / float(AA_SAMPLES)));
        vec3 rd = normalize(camMat * vec3(p.xy + dxy, 1.5));

        // Bounding sphere test — skip rays that miss the diamond entirely
        float sphereT = sphereIntersect(ro, rd, DIAMOND_RADIUS);
        bool hitSphere = sphereT > 0.0 || dot(ro, ro) < DIAMOND_RADIUS * DIAMOND_RADIUS;

        if (hitSphere) {
            for (int i = 0; i < WAVELENGTHS; i++) {
                bounces[i] = initBounce(ro, rd, float(i), sphereT);
            }
            for (int i = 0; i < WAVELENGTHS; i++) {
                for (int j = 0; j < ITERATIONS; j++) {
                    if (doBounce(bounces[i]) == -1.0) break;
                }
            }
            col += resampleColor(bounces);
            // Compute diamond mask
            float diamond = 0.0;
            for (int i = 0; i < WAVELENGTHS; i++) {
                diamond += bounces[i].bounces;
            }
            mask += step(2.0, diamond / float(WAVELENGTHS));
        }
    }

    col /= float(AA_SAMPLES);
    mask /= float(AA_SAMPLES);

    vec3 bg = bgColor.rgb;
    vec3 finalCol = mix(bg, col, mask);

    float alpha = 1.0;
    if (transparentBg) {
        alpha = mask * dot(col, vec3(0.299, 0.587, 0.114));
        alpha = clamp(alpha, 0.0, 1.0);
    }

    gl_FragColor = vec4(finalCol, alpha);
}
