/*{
  "DESCRIPTION": "Reverse Flow Field — colored seeds streaked backward through an animated cellular flow field. Looks like wind-blown grass tips.",
  "CREDIT": "Ported from Shadertoy X3BBD1 by webwarrior (Material Maker output)",
  "CATEGORIES": ["Generator", "Flow"],
  "INPUTS": [
    { "NAME": "iterations",  "LABEL": "Trace Steps",     "TYPE": "float", "DEFAULT": 64.0, "MIN": 8.0,  "MAX": 128.0 },
    { "NAME": "stepExp",     "LABEL": "Step Size (2^-x)","TYPE": "float", "DEFAULT": 10.0, "MIN": 6.0,  "MAX": 14.0 },
    { "NAME": "flowScale",   "LABEL": "Flow Scale",      "TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0,  "MAX": 12.0 },
    { "NAME": "flowSpeed",   "LABEL": "Flow Speed",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 4.0 },
    { "NAME": "octaves",     "LABEL": "Octaves",         "TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0,  "MAX": 6.0 },
    { "NAME": "persistence", "LABEL": "Persistence",     "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.1,  "MAX": 0.9 },
    { "NAME": "dotDensity",  "LABEL": "Seed Density",    "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.01, "MAX": 0.5 },
    { "NAME": "intensity",   "LABEL": "Brightness",      "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.2,  "MAX": 3.0 },
    { "NAME": "audioMod",    "LABEL": "Audio Flow Mod",  "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0 }
  ],
  "PASSES": [
    { "TARGET": "directions" },
    { "TARGET": "positions"  },
    {}
  ]
}*/

// ──────────────────────────────────────────────────────────────────────
// Shared hashes (used in both Buffer A and Buffer B in the original)
// ──────────────────────────────────────────────────────────────────────
float rand1(vec2 x) {
    return fract(cos(mod(dot(x, vec2(13.9898, 8.141)), 3.14)) * 43758.5453);
}
vec2 rand2(vec2 x) {
    return fract(cos(mod(vec2(dot(x, vec2(13.9898, 8.141)),
                              dot(x, vec2(3.4562, 17.398))), vec2(3.14))) * 43758.5453);
}
vec3 rand3(vec2 x) {
    return fract(cos(mod(vec3(dot(x, vec2(13.9898, 8.141)),
                              dot(x, vec2(3.4562, 17.398)),
                              dot(x, vec2(13.254, 5.867))), vec3(3.14))) * 43758.5453);
}

// ──────────────────────────────────────────────────────────────────────
// Buffer A — animated cellular FBM, encoded as a direction vector
// ──────────────────────────────────────────────────────────────────────
float cellular6_noise_2d(vec2 coord, vec2 size, float offset, float seed) {
    vec2 o = floor(coord) + rand2(vec2(seed, 1.0 - seed)) + size;
    vec2 f = fract(coord);
    float min_dist1 = 2.0;
    float min_dist2 = 2.0;
    for (float x = -1.0; x <= 1.0; x++) {
        for (float y = -1.0; y <= 1.0; y++) {
            vec2 neighbor = vec2(x, y);
            vec2 node = rand2(mod(o + vec2(x, y), size)) + vec2(x, y);
            node = 0.5 + 0.25 * sin(offset * 6.28318530718 + 6.28318530718 * node);
            vec2 diff = neighbor + node - f;
            float dist = max(abs(diff.x), abs(diff.y));
            if (min_dist1 > dist) {
                min_dist2 = min_dist1;
                min_dist1 = dist;
            } else if (min_dist2 > dist) {
                min_dist2 = dist;
            }
        }
    }
    return min_dist2 - min_dist1;
}

float fbm_2d_cellular6(vec2 coord, vec2 size, int folds, int octaves_, float persistence_, float offset, float seed) {
    float normalize_factor = 0.0;
    float value = 0.0;
    float scale = 1.0;
    for (int i = 0; i < 8; i++) {
        if (i >= octaves_) break;
        float noise = cellular6_noise_2d(coord * size, size, offset, seed);
        for (int f = 0; f < 4; ++f) {
            if (f >= folds) break;
            noise = abs(2.0 * noise - 1.0);
        }
        value += noise * scale;
        normalize_factor += scale;
        size *= 2.0;
        scale *= persistence_;
    }
    return value / normalize_factor;
}

vec4 passDirections(vec2 fragCoord) {
    float minSize = min(RENDERSIZE.x, RENDERSIZE.y);
    vec2 UV = vec2(0.0, 1.0) + vec2(1.0, -1.0) * (fragCoord - 0.5 * (RENDERSIZE - vec2(minSize))) / minSize;
    UV /= 4.0;
    UV += TIME * (flowSpeed + audioBass * audioMod * 0.5) / 24.0;
    float field = fbm_2d_cellular6(UV, vec2(flowScale, flowScale), 0, int(octaves), persistence, 0.0, 0.0);
    float theta = field * 6.28318530718;
    return vec4(cos(theta) * 0.5 + 0.5, sin(theta) * 0.5 + 0.5, 0.0, 1.0);
}

// ──────────────────────────────────────────────────────────────────────
// Buffer B — colored grass tip seeds (static gradient + dot mask)
// ──────────────────────────────────────────────────────────────────────
vec3 color_dots(vec2 uv, float size, float seed) {
    vec2 seed2 = rand2(vec2(seed, 1.0 - seed));
    uv /= size;
    vec2 point_pos = floor(uv) + vec2(0.5);
    return rand3(seed2 + point_pos);
}

float dots(vec2 uv, float size, float density, float seed) {
    vec2 seed2 = rand2(vec2(seed, 1.0 - seed));
    uv /= size;
    vec2 point_pos = floor(uv) + vec2(0.5);
    return step(rand1(seed2 + point_pos), density);
}

vec3 blend_darken(vec3 c1, vec3 c2, float opacity) {
    return opacity * min(c1, c2) + (1.0 - opacity) * c2;
}

vec4 auroraGradient(float x) {
    const float p0 = 0.363636, p1 = 0.592727, p2 = 0.804218, p3 = 0.907897;
    const vec4 c0 = vec4(0.0,  0.0,  0.0,  1.0); // black
    const vec4 c1 = vec4(0.3,  0.0,  0.8,  1.0); // deep violet
    const vec4 c2 = vec4(0.0,  0.9,  1.0,  1.0); // electric cyan
    const vec4 c3 = vec4(0.0,  1.0,  0.5,  1.0); // bright emerald
    if (x < p0) return c0;
    if (x < p1) return mix(c0, c1, 0.5 - 0.5 * cos(3.14159265359 * (x - p0) / (p1 - p0)));
    if (x < p2) return mix(c1, c2, 0.5 - 0.5 * cos(3.14159265359 * (x - p1) / (p2 - p1)));
    if (x < p3) return mix(c2, c3, 0.5 - 0.5 * cos(3.14159265359 * (x - p2) / (p3 - p2)));
    return c3;
}

vec4 passPositions(vec2 fragCoord) {
    float minSize = min(RENDERSIZE.x, RENDERSIZE.y);
    vec2 UV = vec2(0.0, 1.0) + vec2(1.0, -1.0) * (fragCoord - 0.5 * (RENDERSIZE - vec2(minSize))) / minSize;
    vec3 dotColor = color_dots(UV, 1.0 / 1024.0, 0.0);
    vec4 grad     = auroraGradient(dot(dotColor, vec3(1.0)) / 3.0);
    float dotMask = dots(UV, 1.0 / 1024.0, dotDensity, 0.334808);
    vec3  blended = blend_darken(grad.rgb, vec3(dotMask), 1.0 * grad.a);
    float a = min(1.0, dotMask + grad.a);
    return vec4(blended, a);
}

// ──────────────────────────────────────────────────────────────────────
// Image — backward trace through the flow field, weighted by a bezier curve
// ──────────────────────────────────────────────────────────────────────
float weightCurve(float x) {
    // Bezier curve: (0,0) → (0.707,0.293) → (1,1), tangents per the Material Maker spec
    const float p0x = 0.0,        p0y = 0.0,        p0rs = 0.0;
    const float p1x = 0.707107,   p1y = 0.292893,   p1ls = 1.0, p1rs = 1.0;
    const float p2x = 1.0,        p2y = 1.0,        p2ls = 4.0;
    if (x <= p1x) {
        float dx = x - p0x;
        float d  = p1x - p0x;
        float t  = dx / d;
        float omt = 1.0 - t;
        d /= 3.0;
        float yac = p0y + d * p0rs;
        float ybc = p1y - d * p1ls;
        return p0y * omt*omt*omt + yac * omt*omt * t * 3.0 + ybc * omt * t*t * 3.0 + p1y * t*t*t;
    }
    float dx = x - p1x;
    float d  = p2x - p1x;
    float t  = dx / d;
    float omt = 1.0 - t;
    d /= 3.0;
    float yac = p1y + d * p1rs;
    float ybc = p2y - d * p2ls;
    return p1y * omt*omt*omt + yac * omt*omt * t * 3.0 + ybc * omt * t*t * 3.0 + p2y * t*t*t;
}

vec3 traceIntensity(vec2 pos) {
    float stepLen = pow(2.0, -stepExp);
    vec3 color    = vec3(0.0);
    float alpha   = 0.0;
    vec2 p        = pos;
    int N         = int(iterations);
    for (int i = 0; i < 128; i++) {
        if (i >= N) break;
        vec4 sample_ = texture(positions, p);
        float w = weightCurve((float(N - i)) / float(N));
        alpha += sample_.a * w;
        color += sample_.rgb * sample_.a * w;
        vec3 dir = texture(directions, p).rgb;
        p = p - (dir.xy - 0.5) * 2.0 * stepLen;
    }
    return color / (alpha + 1.0) * float(N) / 4.0;
}

vec4 passImage(vec2 fragCoord) {
    float maxSize = max(RENDERSIZE.x, RENDERSIZE.y);
    vec2 UV = vec2(0.0, 1.0) + vec2(1.0, -1.0) * (fragCoord - 0.5 * (RENDERSIZE - vec2(maxSize))) / maxSize;
    vec3 col = traceIntensity(UV) * intensity * 2.0;
    return vec4(col, 1.0);
}

// ──────────────────────────────────────────────────────────────────────
void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    if      (PASSINDEX == 0) FragColor = passDirections(fragCoord);
    else if (PASSINDEX == 1) FragColor = passPositions(fragCoord);
    else                     FragColor = passImage(fragCoord);
}
