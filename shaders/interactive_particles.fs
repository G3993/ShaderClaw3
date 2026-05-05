/*{
  "DESCRIPTION": "Interactive Particles — 1024 particles attracted to a center point and the cursor, glowing with velocity-based color. Click and drag to pull them around.",
  "CREDIT": "Ported from Shadertoy McXXzH by berelium",
  "CATEGORIES": ["Generator", "Particles", "Interactive"],
  "INPUTS": [
    { "NAME": "attraction",     "LABEL": "Attraction",     "TYPE": "float", "DEFAULT": 8.0,    "MIN": 0.0,    "MAX": 30.0 },
    { "NAME": "maxSpeed",       "LABEL": "Max Speed",      "TYPE": "float", "DEFAULT": 600.0,  "MIN": 50.0,   "MAX": 2000.0 },
    { "NAME": "particleSize",   "LABEL": "Particle Size",  "TYPE": "float", "DEFAULT": 0.0005, "MIN": 0.0001, "MAX": 0.003 },
    { "NAME": "intensity",      "LABEL": "Intensity",      "TYPE": "float", "DEFAULT": 1.9,    "MIN": 0.5,    "MAX": 3.0 },
    { "NAME": "contrast",       "LABEL": "Contrast",       "TYPE": "float", "DEFAULT": 1.04,   "MIN": 0.5,    "MAX": 2.0 },
    { "NAME": "gamma",          "LABEL": "Exposure",       "TYPE": "float", "DEFAULT": 1.6,    "MIN": 1.0,    "MAX": 3.0 },
    { "NAME": "audioReact",     "LABEL": "Audio React",    "TYPE": "float", "DEFAULT": 1.0,    "MIN": 0.0,    "MAX": 2.0 },
    { "NAME": "centerAttractor","LABEL": "Center Attract", "TYPE": "bool",  "DEFAULT": true },
    { "NAME": "wrapEdges",      "LABEL": "Wrap Edges",     "TYPE": "bool",  "DEFAULT": true },
    { "NAME": "bounce",         "LABEL": "Bounce",         "TYPE": "float", "DEFAULT": 0.4,    "MIN": 0.0,    "MAX": 1.5 },
    { "NAME": "atanColor",      "LABEL": "Direction Hue",  "TYPE": "bool",  "DEFAULT": true }
  ],
  "PASSES": [
    { "TARGET": "bufA", "PERSISTENT": true },
    {}
  ]
}*/

#define NUM_PARTICLES 1024
#define P_ITERATOR    32
#define PI            3.14159265359
#define ASPECT        (RENDERSIZE.x / RENDERSIZE.y)

// Hash without sine (Dave Hoskins, https://www.shadertoy.com/view/4djSRW)
#define HASHSCALE1 443.8975
#define HASHSCALE3 vec3(443.897, 441.423, 437.195)

float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}
vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * HASHSCALE3);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}

vec3 hsl2rgb(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    return c.z + c.y * (rgb - 0.5) * (1.0 - abs(2.0 * c.z - 1.0));
}

// ──────────────────────────────────────────────────────────────────────
// Pass 0 — Buffer A: simulate one particle per pixel (slots 32×32 = 1024)
// ──────────────────────────────────────────────────────────────────────
vec2 initialVelocity(vec2 pos) {
    vec2 dir = normalize(vec2(0.5) - pos);
    float angle = atan(dir.y, dir.x) * PI;
    float speed = 0.5;
    return speed * maxSpeed * vec2(cos(angle), sin(angle));
}

vec4 passSim(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE;

    // Init: first 5 frames seed random positions
    if (FRAMEINDEX < 5) {
        vec2 pos = hash22(uv.yx);
        vec2 vel = initialVelocity(pos);
        return vec4(pos, vel);
    }

    vec4 prev = texelFetch(bufA, ivec2(fragCoord), 0);
    vec2 position = prev.xy;
    vec2 velocity = prev.zw;

    // Mouse attraction (when held — Easel: mouseDown > 0.5)
    if (mouseDown > 0.5) {
        vec2 mV = mousePos - position;
        velocity += attraction * 5.0 / PI * normalize(mV);
    }

    // Center attractor — inverse-square pull toward (0.5, 0.5)
    if (centerAttractor) {
        vec2 v = vec2(0.5) - position;
        float d = length(v);
        velocity += (v / d) / pow(max(d, 2.0) / attraction, 2.0);
    }

    // Edge handling
    if (wrapEdges) {
        position = fract(position);
    } else {
        if (position.x - particleSize < 0.0)  { position.x = particleSize;       velocity.x =  abs(velocity.x) * bounce; }
        if (position.y + particleSize > 1.0)  { position.y = 1.0 - particleSize; velocity.y = -abs(velocity.y) * bounce; }
        if (position.x + particleSize > 1.0)  { position.x = 1.0 - particleSize; velocity.x = -abs(velocity.x) * bounce; }
        if (position.y - particleSize < 0.0)  { position.y = particleSize;       velocity.y =  abs(velocity.y) * bounce; }
    }

    // Step
    float dt = TIMEDELTA * 0.001;
    position += velocity * dt;

    // Speed cap
    if (length(velocity) > maxSpeed) velocity = normalize(velocity) * maxSpeed;

    return vec4(position, velocity);
}

// ──────────────────────────────────────────────────────────────────────
// Pass 1 — Image: render particles by iterating Buffer A's grid
// ──────────────────────────────────────────────────────────────────────
vec3 renderParticles(vec2 uv) {
    vec3 color = vec3(0.01);
    float bassBoost = 0.5 + 0.5 * audioBass * audioReact;
    float drawSize = particleSize * ASPECT * bassBoost;

    for (int y = 0; y < P_ITERATOR; y++) {
        for (int x = 0; x < P_ITERATOR; x++) {
            vec4 p = texelFetch(bufA, ivec2(x, y), 0);
            vec2 d = uv - p.xy;
            d.x *= ASPECT;

            float mag = (p.z * p.z + p.w * p.w) * 0.000000625;
            vec3 col;
            if (atanColor) {
                float r = atan(p.w, p.z);
                r += PI * 1.5;
                col = hsl2rgb(vec3(((r / PI) / 2.0), mag + 0.45, mag + 0.11));
            } else {
                col = hsl2rgb(vec3(((mag / maxSpeed * maxSpeed) * 2.5), 0.6, max(0.25, mag)));
            }
            float c = 1.0 / length(d);
            c *= drawSize;
            c = pow(c, intensity);
            color += c * col;
        }
    }

    if (centerAttractor) {
        vec2 a = uv - vec2(0.5);
        a.x *= ASPECT;
        float c = 1.0 / length(a);
        c *= drawSize * 10.0;
        c = pow(c, intensity);
        color += c * vec3(0.5);
    }
    return color;
}

vec4 passFinal(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE;
    vec3 col = renderParticles(uv);

    // Contrast then linear HDR exposure — host applies ACES+gamma
    col = (col - 0.5) * contrast + 0.5;
    col *= gamma;
    return vec4(max(col, 0.0), 1.0);
}

// ──────────────────────────────────────────────────────────────────────
void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    if (PASSINDEX == 0) FragColor = passSim(fragCoord);
    else                FragColor = passFinal(fragCoord);
}
