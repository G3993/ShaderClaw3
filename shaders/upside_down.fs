/*{
  "DESCRIPTION": "Upside Down — a pool of SPH-style liquid particles settles under gravity, clumping into droplets rendered as velocity-hued glow; on every bass hit gravity flips and the whole pool falls upward before raining back down. Mids loosen the liquid, highs shimmer the hues.",
  "CREDIT": "Inspired by michael0884-style SPH particle buffers (Shadertoy), re-authored for ShaderClaw",
  "CATEGORIES": [
    "Generator",
    "Particles"
  ],
  "INPUTS": [
    {
      "NAME": "speed",
      "LABEL": "Speed",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.1,
      "MAX": 3.0
    },
    {
      "NAME": "audioReact",
      "LABEL": "Audio React",
      "TYPE": "float",
      "GROUP": "Audio Reactivity",
      "DEFAULT": 0.6,
      "MIN": 0.0,
      "MAX": 1.0
    }
  ],
  "PASSES": [
    {
      "TARGET": "agentBuf",
      "PERSISTENT": true
    },
    {
      "TARGET": "trailBuf",
      "PERSISTENT": true
    },
    {}
  ]
}*/

#define N_PARTICLES 48
#define TAU 6.283185307179586

float knee(float x, float lo, float hi) { return smoothstep(lo, hi, x); }

// 3 texels per particle: [pos 16-bit x2] [velx 16-bit] [vely 16-bit]
#define VEL_R 0.1
vec4 enc16pair(vec2 p) {
    vec2 e = clamp(p, 0.0, 1.0) * 255.0;
    return vec4(floor(e.x) / 255.0, fract(e.x), floor(e.y) / 255.0, fract(e.y));
}
vec2 dec16pair(vec4 t) { return vec2(t.r + t.g / 255.0, t.b + t.a / 255.0); }

vec4 fetchT(int texel) {
    return texture2D(agentBuf, vec2((float(texel) + 0.5) / RENDERSIZE.x, 0.5 / RENDERSIZE.y));
}
vec2 pPos(int i) { return dec16pair(fetchT(3 * i)); }
vec2 pVel(int i) {
    vec2 vx = dec16pair(fetchT(3 * i + 1));
    vec2 vy = dec16pair(fetchT(3 * i + 2));
    return (vec2(vx.x, vy.x) - 0.5) * 2.0 * VEL_R;
}

vec2 hash2(float n) {
    return fract(sin(vec2(n, n + 17.7)) * vec2(43758.5453, 22578.1459));
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// pass 0 — SPH-lite update
vec4 passAgents() {
    int texel = int(gl_FragCoord.x);
    int i = texel / 3;
    int part = texel - 3 * i;
    if (i >= N_PARTICLES) return vec4(0.0);

    float ar = audioReact;
    float bassP = pow(knee(audioBass, 0.05, 0.85), 1.6);
    float midP  = pow(knee(audioMid,  0.08, 0.90), 1.3);
    float dt = clamp(TIMEDELTA, 0.005, 0.04) * 60.0 * speed;

    vec2 pos = pPos(i);
    vec2 vel = pVel(i);

    // gravity flips upward on bass hits — the pool falls up
    float g = 0.00035 * (1.0 - 2.4 * ar * bassP);
    vec2 F = vec2(0.0, -g);

    // pairwise cohesion/repulsion + viscosity
    float visc = 0.003 * mix(1.0, 0.4 + 0.6 * (1.0 - midP), ar);
    for (int k = 0; k < N_PARTICLES; k++) {
        if (k == i) continue;
        vec2 dx = pPos(k) - pos;
        float d = length(dx) + 1e-4;
        if (d > 0.25) continue;
        vec2 ndir = dx / d;
        F += ndir * (0.00004 * exp(-d / 0.09) - 0.0016 * exp(-d / 0.06));
        F += (pVel(k) - vel) * visc * exp(-d * d / 0.004);
    }

    vel += F * dt;
    vel = clamp(vel, -VEL_R * 0.9, VEL_R * 0.9);
    pos += vel * dt;

    // walls
    if (pos.x < 0.03) { pos.x = 0.03; vel.x = abs(vel.x) * 0.6; }
    if (pos.x > 0.97) { pos.x = 0.97; vel.x = -abs(vel.x) * 0.6; }
    if (pos.y < 0.04) { pos.y = 0.04; vel.y = abs(vel.y) * 0.5; }
    if (pos.y > 0.96) { pos.y = 0.96; vel.y = -abs(vel.y) * 0.5; }

    if (FRAMEINDEX < 2) {
        vec2 h = hash2(float(i) * 3.17);
        pos = vec2(0.08 + 0.84 * h.x, 0.08 + 0.84 * h.y);
        vel = vec2(0.0);
    }

    if (part == 0) return enc16pair(pos);
    if (part == 1) return enc16pair(vec2(vel.x / (2.0 * VEL_R) + 0.5, 0.0));
    return enc16pair(vec2(vel.y / (2.0 * VEL_R) + 0.5, 0.0));
}

// pass 1 — velocity-hued droplet glow with trails
vec4 passTrail() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float ar = audioReact;
    float highP  = pow(knee(audioHigh, 0.10, 0.90), 1.2);
    float levelP = knee(audioLevel, 0.05, 0.90);

    vec3 prev = texture2D(trailBuf, uv).rgb * 0.86;
    vec3 acc = vec3(0.0);
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    for (int k = 0; k < N_PARTICLES; k++) {
        vec2 d2 = (uv - pPos(k)) * vec2(aspect, 1.0);
        float d = dot(d2, d2);
        vec2 v = pVel(k);
        float sp = length(v) / VEL_R;
        float hue = fract(atan(v.y, v.x) / TAU + 0.6 + ar * 0.12 * highP);
        vec3 c = hsv2rgb(vec3(hue, 0.8, 0.22 + 1.1 * sp));
        acc += c * exp(-d * 7000.0);
        acc += c * exp(-d * 400.0) * 0.08;
    }
    acc = acc / (1.0 + 0.85 * acc); // soft-knee so clusters never blow out
    vec3 outC = max(prev, acc);
    if (FRAMEINDEX < 2) outC = vec3(0.0);
    return vec4(clamp(outC, 0.0, 1.0), 1.0);
}

vec4 passImage() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float ar = audioReact;
    vec3 c = texture2D(trailBuf, uv).rgb;
    vec3 bg = mix(vec3(0.02, 0.02, 0.05), vec3(0.05, 0.03, 0.08), uv.y);
    // sustained lift lives here (outside the feedback loop)
    float lift = 1.0 + ar * (0.45 * knee(audioLevel, 0.05, 0.9)
                           + 0.35 * pow(knee(audioBass, 0.05, 0.85), 1.6));
    return vec4(bg + c * lift, 1.0);
}

void main() {
    if      (PASSINDEX == 0) gl_FragColor = passAgents();
    else if (PASSINDEX == 1) gl_FragColor = passTrail();
    else                     gl_FragColor = passImage();
}
