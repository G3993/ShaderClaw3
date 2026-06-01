/*{
  "DESCRIPTION": "Linear — a barebones boids flocking sim. Buffer A tracks 280 boids (position + velocity) in the first texel row, steering each toward the average heading and position of neighbors within sight while separating from those too close. Buffer B renders the boids as glowing dots with a ghosting trail; the image pass tone-maps it. Ported to Easel ISF: the Shadertoy 'common' tab folded inline, iChannels mapped to named persistent buffers, mouse/keyboard dropped, iDate seed replaced by TIME.",
  "CREDIT": "Cole Peterson — ISF port for Easel.",
  "CATEGORIES": ["Generator", "Simulation", "Particles"],
  "INPUTS": [
    { "NAME": "speed",    "LABEL": "Speed",      "TYPE": "float", "MIN": 0.5, "MAX": 6.0,  "DEFAULT": 2.2 },
    { "NAME": "dt",       "LABEL": "Sim Speed",  "TYPE": "float", "MIN": 0.1, "MAX": 2.0,  "DEFAULT": 0.7 },
    { "NAME": "sight",    "LABEL": "Sight Radius","TYPE": "float", "MIN": 10.0, "MAX": 120.0, "DEFAULT": 33.0 },
    { "NAME": "trail",    "LABEL": "Trail",      "TYPE": "float", "MIN": 0.0, "MAX": 0.995, "DEFAULT": 0.97 },
    { "NAME": "exposure", "LABEL": "Exposure",   "TYPE": "float", "MIN": 0.3, "MAX": 2.5,  "DEFAULT": 1.0 }
  ],
  "PASSES": [
    { "TARGET": "bufA", "PERSISTENT": true },
    { "TARGET": "bufB", "PERSISTENT": true },
    {}
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  LINEAR — boids flocking (ISF port).
//    bufA: boid state, texel (i,0) = (pos.xy, vel.zw), i in [0,nParticles).
//    bufB: rendered dots + ghost trail (persistent).
//    A(p) -> texelFetch(bufA);  iChannel0/1 -> bufA / bufB by pass.
//  PASSINDEX 0 = bufA sim, 1 = bufB render, 2 = image.
// ════════════════════════════════════════════════════════════════════════

#define R   RENDERSIZE.xy
#define ss(a, b, t) smoothstep(a, b, t)
#define A(p) texelFetch(bufA, ivec2(p), 0)

const int   nParticles = 280;
const float minSep     = 33.0;
const float rad        = 0.0075;
const float obRad      = 45.0;

vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}

void main() {
    // ───────── Pass 0 — boid simulation ─────────
    if (PASSINDEX == 0) {
        ivec2 ip = ivec2(gl_FragCoord.xy);
        if (ip.x < nParticles && ip.y == 0) {
            vec4 bA = A(ip);
            bA.xy += bA.zw * dt;

            vec2 avgDir = vec2(0);
            vec2 avgPos = vec2(0);
            float nb = 0.0;

            for (int i = 0; i < nParticles; i++) {
                vec4 p = A(ivec2(i, 0));
                float d = length(bA.xy - p.xy);
                if (d <= sight) { avgDir += p.zw; avgPos += p.xy; nb++; }
                if (d <= minSep) {
                    vec2 dir = normalize(p.xy - bA.xy);
                    bA.xy -= dir * minSep * .008;
                }
            }

            if (nb > 0.0) {
                avgPos /= nb;
                bA.zw = normalize(avgDir) * speed;
                vec2 dir = normalize(avgPos - bA.xy);
                bA.zw += dir * 0.1;
            }

            bA.z += .22 * cos(TIME + gl_FragCoord.x * 555.0);
            bA.w += .22 * sin(TIME * 1.3 + gl_FragCoord.x * 355.0);

            bA.xy = mod(bA.xy, R);

            if (FRAMEINDEX < 4) {
                bA.xy = hash22(gl_FragCoord.xy * 999.0 + 522.2 + TIME) * R * 0.7 + R * 0.15;
                bA.zw = (2.0 * hash22(gl_FragCoord.xy * 999.0 + 322.2) - 1.0) * 0.4;
            }
            gl_FragColor = bA;
        } else {
            gl_FragColor = vec4(0.0);
        }
        return;
    }

    // ───────── Pass 1 — render boids + trail ─────────
    if (PASSINDEX == 1) {
        vec4 bB = texture(bufB, gl_FragCoord.xy / R);
        for (int i = 0; i < nParticles; i++) {
            vec2 p = A(ivec2(i, 0)).xy;
            float d = length(p - gl_FragCoord.xy);
            vec3 c = 0.5 + 0.5 * cos(vec3(4., 1., 2.) * float(i) * 53.);
            bB.xyz = mix(bB.xyz, c, ss(rad * R.y, rad * R.y - 1.0, d));
            float sd2 = ss(.4 * rad * R.y, .4 * rad * R.y - 1.0, d);
            bB.w = mix(bB.w, 1.0, sd2);
        }
        bB.xyz *= 0.8;
        bB.w   *= trail;
        gl_FragColor = bB;
        return;
    }

    // ───────── Pass 2 — image ─────────
    vec4 bB = texture(bufB, gl_FragCoord.xy / R);
    vec4 f = vec4(0.44 * vec3(bB.w), 1.0);
    f.xyz += 1.0 - exp(-bB.xyz);
    gl_FragColor = vec4(f.rgb * exposure, 1.0);
}
