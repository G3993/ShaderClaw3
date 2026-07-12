/*{
  "DESCRIPTION": "Angel Hair — hundreds of particles ride a divergence-free noise flow, engraving fine sinuous ink strands onto a paper-white canvas; a slow tonal wave inverts bands of the image as it drifts. Bass presses the ink darker, mids quicken the strands, highs shift the strand color; the drawing resets on a slow loop.",
  "CREDIT": "Sinuous by nimitz (stormoid, shadertoy 4sGSDw) CC BY-NC-SA 3.0, ShaderClaw audio port",
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
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "tintColor",
      "LABEL": "Tint",
      "TYPE": "color",
      "GROUP": "Color",
      "DEFAULT": [1.0, 1.0, 1.0, 1.0]
    },
    {
      "NAME": "brightness",
      "LABEL": "Brightness",
      "TYPE": "float",
      "GROUP": "Color",
      "DEFAULT": 1.0,
      "MIN": 0.2,
      "MAX": 3.0
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

#define N_PARTICLES 300
#define TAU 6.283185307179586
#define LOOP_T 26.7

float knee(float x, float lo, float hi) { return smoothstep(lo, hi, x); }

// packing: pos in [-1.1,1.1]^2 -> 16-bit/axis (texel 2i); angle 16-bit + speed (texel 2i+1)
vec4 encPos(vec2 p) {
    p = clamp((p + 1.1) / 2.2, 0.0, 1.0) * 255.0;
    return vec4(floor(p.x) / 255.0, fract(p.x), floor(p.y) / 255.0, fract(p.y));
}
vec2 decPos(vec4 t) { return vec2(t.r + t.g / 255.0, t.b + t.a / 255.0) * 2.2 - 1.1; }
vec4 encState(float ang, float spd) {
    float e = fract(ang / TAU) * 255.0;
    return vec4(floor(e) / 255.0, fract(e), clamp(spd / 2.0, 0.0, 1.0), 1.0);
}

vec4 fetchAgent(int texel) {
    return texture2D(agentBuf, vec2((float(texel) + 0.5) / RENDERSIZE.x, 0.5 / RENDERSIZE.y));
}
vec2 agentPos(int i) { return decPos(fetchAgent(2 * i)); }
vec2 agentVel(int i) {
    vec4 s = fetchAgent(2 * i + 1);
    float ang = (s.r + s.g / 255.0) * TAU;
    return vec2(cos(ang), sin(ang)) * s.b * 2.0;
}

// Dave Hoskins hash + iq gradient noise
vec2 hash2(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(443.897, 441.423, 437.195));
    p3 += dot(p3.zxy, p3.yxz + 19.19);
    return fract(vec2(p3.x * p3.y, p3.z * p3.x)) * 2.0 - 1.0;
}
float gnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(dot(hash2(i), f), dot(hash2(i + vec2(1, 0)), f - vec2(1, 0)), u.x),
               mix(dot(hash2(i + vec2(0, 1)), f - vec2(0, 1)),
                   dot(hash2(i + vec2(1, 1)), f - vec2(1, 1)), u.x), u.y);
}
float fbm2(vec2 p, float tm) {
    p *= 2.0;
    p -= tm;
    float z = 2.0, rz = 0.0;
    p += TIME * 0.001 + 0.1;
    const mat2 m2 = mat2(0.80, -0.60, 0.60, 0.80);
    for (int i = 1; i < 7; i++) {
        rz += abs((gnoise(p) - 0.5) * 2.0) / z;
        z *= 1.93;
        p = m2 * p * 2.0;
    }
    return rz;
}

bool resetPulse() {
    return FRAMEINDEX < 12 || mod(TIME * speed, LOOP_T) < max(TIMEDELTA * speed, 0.034);
}

// pass 0 — particle flow (divergence-free field from noise angle)
vec4 passAgents() {
    int texel = int(gl_FragCoord.x);
    int i = texel / 2;
    bool isPos = (texel - 2 * i) == 0;
    if (i >= N_PARTICLES) return vec4(0.0);

    float ar = audioReact;
    float midP = pow(knee(audioMid, 0.08, 0.90), 1.3);

    vec2 pos = agentPos(i);
    vec2 vel = agentVel(i);

    float n1a = fbm2(pos, 0.0);
    float n1b = fbm2(pos.yx, 0.0);
    float nn = fbm2(vec2(n1a, n1b), 0.0) * 5.8 + 0.5;
    vec2 dir = vec2(cos(nn), sin(nn));
    vel = mix(vel, dir * 1.5, 0.05);

    // mids quicken the strands (velocity scale, not a clock)
    pos += vel * 0.004 * speed * mix(1.0, 0.7 + 1.1 * midP, ar);

    if (pos.x > 1.05 || abs(pos.y) > 1.05) {
        vec2 h = hash2(vec2(float(i) * 0.713, 4.7));
        pos = vec2(-0.99, h.x * 0.45);
        vel = vec2(1.5, 0.0) * 0.1;
    }
    if (resetPulse()) {
        // scatter across the canvas so strands appear immediately
        vec2 h = hash2(vec2(float(i) * 0.713, 9.1));
        pos = vec2(h.y * 0.99, h.x * 0.5);
        vel = vec2(1.5, 0.0) * 0.1;
    }

    if (isPos) return encPos(pos);
    return encState(atan(vel.y, vel.x), length(vel));
}

// pass 1 — ink accumulation (bright strands on black, inverted at display)
vec4 passTrail() {
    vec2 p = (gl_FragCoord.xy / RENDERSIZE.xy - 0.5);
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
    p *= 1.1;

    float ar = audioReact;
    float bassP  = pow(knee(audioBass, 0.05, 0.85), 1.6);
    float levelP = knee(audioLevel, 0.05, 0.90);
    float highP  = pow(knee(audioHigh, 0.10, 0.90), 1.2);

    vec3 ink = vec3(0.0);
    float T = TIME * speed;
    for (int i = 0; i < N_PARTICLES; i++) {
        vec2 pos = agentPos(i);
        float d = dot(p - pos, p - pos) * 500.0;
        d = 0.01 / (d + 0.001);
        ink += d * abs(sin(vec3(2.0, 3.4, 1.2) * (T * 0.07 + float(i) * 0.0017 + 2.5
                                                  + ar * 0.3 * highP)
                           + vec3(0.8, 0.0, 1.2)) * 0.7 + 0.3) * 0.04;
    }
    ink *= 0.5 * mix(1.0, 0.4 + 1.6 * bassP + 0.6 * levelP, ar);

    vec3 prev = texture2D(trailBuf, gl_FragCoord.xy / RENDERSIZE.xy).rgb;
    vec3 stored = clamp(max(prev * 0.995, ink), 0.0, 1.0);
    if (FRAMEINDEX < 2) stored = vec3(0.0);
    return vec4(stored, 1.0);
}

// pass 2 — invert to paper + tonal inversion wave
vec4 passImage() {
    vec2 q = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = (q - 0.5);
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
    float m = dot(p * 1.1 + vec2(-0.20, -0.3), p * 1.1 + vec2(-0.20, -0.3));
    vec3 paper = vec3(1.0, 0.98, 0.9) * (1.0 - m * 0.1);
    vec3 c = clamp(paper - texture2D(trailBuf, q).rgb, 0.0, 1.0);
    vec3 col = mix(c, 1.0 - c, smoothstep(-0.3, 0.3, sin(q.y + TIME * 0.0717 + 3.4)));
    return vec4(col * tintColor.rgb * brightness, 1.0);
}

void main() {
    if      (PASSINDEX == 0) gl_FragColor = passAgents();
    else if (PASSINDEX == 1) gl_FragColor = passTrail();
    else                     gl_FragColor = passImage();
}
