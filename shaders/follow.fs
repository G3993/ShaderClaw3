/*{
  "DESCRIPTION": "Follow — a self-organizing particle flow. Each cell carries mass + velocity; mass is pushed along its velocity toward the local centre-of-mass gradient while a 1/r^2 gravity clumps the streams into travelling points, so a field of drifting specks continuously chases, merges and scatters. A side force feeds new mass from the left so it never settles. Palette-coloured with an additive glow. Ported from Cole Peterson's multi-buffer Shadertoy; texelFetch feedback -> resolution-independent buffer sampling, mouse/keyboard interaction dropped for autonomous motion.",
  "CREDIT": "Cole Peterson (Plento) — ISF port for Easel",
  "CATEGORIES": ["Simulation", "Generator", "Abstract"],
  "INPUTS": [
    { "NAME": "sideForce", "LABEL": "Feed",      "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "gravity",   "LABEL": "Clump",     "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "glowAmt",   "LABEL": "Glow",      "TYPE": "float", "MIN": 0.0, "MAX": 0.4, "DEFAULT": 0.08 }
  ],
  "PASSES": [
    { "TARGET": "bufA", "PERSISTENT": true },
    {}
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  FOLLOW — Easel ISF port of Cole Peterson's particle-flow Shadertoy.
//    iChannel0 (Buffer A self-feedback) -> bufA
//    texelFetch(ivec2(p))               -> texture(bufA, p/RENDERSIZE)  (res-independent)
//    iMouse / keyboard / Buffer C       -> dropped (autonomous side-force drive)
//    iFrame -> FRAMEINDEX, iResolution -> RENDERSIZE, iTime -> TIME
//  Pass order: bufA (sim, PASSINDEX 0), image (colour+glow, PASSINDEX 1).
// ════════════════════════════════════════════════════════════════════════

#define R RENDERSIZE.xy
#define A(p) texture(bufA, (p) / RENDERSIZE.xy)
#define ss(a, b, t) smoothstep(a, b, t)

const vec2 UP    = vec2(0.,  1.);
const vec2 DOWN  = vec2(0., -1.);
const vec2 LEFT  = vec2(-1., 0.);
const vec2 RIGHT = vec2(1.,  0.);
const vec2 UPL   = vec2(-1.,  1.);
const vec2 DOWNL = vec2(-1., -1.);
const vec2 UPR   = vec2(1.,   1.);
const vec2 DOWNR = vec2(1.,  -1.);

vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}
float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * .1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec3 pal(float t) {
    return 0.5 + 0.44 * cos(vec3(1.4, 1.1, 1.4) * t + vec3(1.1, 6.1, 4.4) + .5);
}

// ── Buffer A helpers (neighbourhood gradients) ─────────────────────────
vec4 Grad(vec2 u) {
    vec4 up = A(u + UP), down = A(u + DOWN), left = A(u + LEFT), right = A(u + RIGHT);
    vec4 upl = A(u + UPL), downl = A(u + DOWNL), upr = A(u + UPR), downr = A(u + DOWNR);
    return (up + down + left + right + upr + downr + upl + downl) / 8.;
}
vec2 massGrad(vec2 u) {
    vec2 cm = vec2(0.);
    cm += A(u + UP).x    * UP;
    cm += A(u + DOWN).x  * DOWN;
    cm += A(u + LEFT).x  * LEFT;
    cm += A(u + RIGHT).x * RIGHT;
    cm += A(u + UPL).x   * UPL;
    cm += A(u + DOWNL).x * DOWNL;
    cm += A(u + UPR).x   * UPR;
    cm += A(u + DOWNR).x * DOWNR;
    return cm;
}

// ── image helpers ───────────────────────────────────────────────────────
vec3 color(vec4 bA) {
    float t = abs(bA.z * 2.) + abs(bA.w * 4.) + 3.6 * length(bA.zw);
    vec3 col = vec3(clamp(bA.x, 0., 1.)) * pal(t * .2 + .3);
    return col * 3.6;
}
float glow(vec2 u) {
    vec2 uv = u / R;
    float blur = 0.;
    const float N = 3.;
    for (float i = 0.; i < N; i++) {
        blur += texture(bufA, uv + vec2(i * .001, 0.)).x;
        blur += texture(bufA, uv - vec2(i * .001, 0.)).x;
        blur += texture(bufA, uv + vec2(0., i * .001)).x;
        blur += texture(bufA, uv - vec2(0., i * .001)).x;
    }
    return blur / N * 4.;
}

void main() {
    vec2 u = gl_FragCoord.xy;

    // ── Buffer A: the mass/velocity simulation ───────────────────────────
    if (PASSINDEX == 0) {
        vec4 bA = A(u);

        vec4 grad = Grad(u);
        vec2 vAvg = grad.zw;
        vec2 acc = clamp(vAvg - bA.zw, -1., 1.);

        bA = mix(bA, grad, .02);

        vec2 mg = massGrad(u);
        float massAvg = grad.x;
        float massDif = massAvg - bA.x;
        float dp = dot(mg, bA.zw);
        bA.x -= dp * massAvg;
        bA.x += .999 * dp * massDif;

        bA.zw += acc;

        // 1/r^2 gravity toward the mass gradient — clumps streams into points.
        float r = max(length(mg), 1.);
        float grav = -(3.5 * gravity * massAvg * bA.x) / (r * r);
        bA.zw -= mg * grav;

        // Side force: keep feeding mass + push from the left so it never settles.
        float sf = sideForce * .002 * ss(R.x, 0., u.x * 0.9);
        bA.zw += sf * vec2(1., 0.);
        bA.x  += .06 * sf;

        // Seed random specks on the first frames so there's motion immediately.
        if (FRAMEINDEX < 8) {
            bA = vec4(0.);
            if (hash12(u * 343. + 232.) < .1) {
                bA.zw = 12. * (2. * hash22(u * 543. + 332.) - 1.);
                bA.x = hash22(u * 543. + 332.).x;
            }
        }

        bA.zw = clamp(bA.zw, -120., 120.);
        bA.x = clamp(bA.x, 0., 1.);
        gl_FragColor = bA;
        return;
    }

    // ── Image: palette colour + additive glow ─────────────────────────────
    vec4 bA = A(u);
    vec3 col = color(bA);
    float g = glow(u);
    col += glowAmt * g * pal(1.5 * length(bA.zw));
    gl_FragColor = vec4(sqrt(clamp(col, 0.0, 1.0)), 1.0);
}
