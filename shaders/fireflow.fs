/*{
  "DESCRIPTION": "Fireflow — a compact buoyant fluid. A single feedback buffer holds velocity (xy), a pressure-like term (z) and buoyancy (w); each step diffuses, advects along its own velocity, removes divergence, and lets buoyancy push upward while an auto-driven swirl injects heat — giving a rising, flame-like flow. The image pass maps the field to warm color. Ported to Easel ISF: feedback buffer persistent, keyboard reset dropped, mouse replaced by an automatic swirl.",
  "CREDIT": "Shadertoy buoyant-flow original — ISF port for Easel.",
  "CATEGORIES": ["VFX", "Fluid", "Simulation", "Generator"],
  "INPUTS": [
    { "NAME": "diffuse",   "LABEL": "Diffuse (3.5-11.5)", "TYPE": "float", "MIN": 1.0, "MAX": 12.0, "DEFAULT": 5.5 },
    { "NAME": "buoyancy",  "LABEL": "Buoyancy",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "stirSpeed", "LABEL": "Swirl Speed","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.4 },
    { "NAME": "colorScale","LABEL": "Tint",      "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "exposure",  "LABEL": "Exposure",  "TYPE": "float", "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0 }
  ],
  "PASSES": [
    { "TARGET": "bufA", "PERSISTENT": true },
    {}
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  FIREFLOW — buoyant feedback fluid (ISF port).
//    bufA channels: xy = velocity, z = pressure-ish, w = buoyancy/heat.
//    iChannel0 -> bufA;  iChannel1 keyboard reset -> FRAMEINDEX init;
//    iMouse -> automatic circling swirl that injects heat.
//  PASSINDEX 0 = simulation (bufA), 1 = image.
// ════════════════════════════════════════════════════════════════════════

const float accel       = 0.1;
const float max_speed   = 0.3;
const float dissipate   = 0.001;
const float springiness = 0.01;

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // ───────── Pass 0 — simulation ─────────
    if (PASSINDEX == 0) {
        vec2 delta = vec2(diffuse) / RENDERSIZE.xy;

        vec4 a_ = texture(bufA, uv - delta);
        vec4 b_ = texture(bufA, uv + vec2(delta.x, -delta.y));
        vec4 c_ = texture(bufA, uv + vec2(-delta.x, delta.y));
        vec4 d_ = texture(bufA, uv + delta);

        vec4 v = 0.25 * (a_ + b_ + c_ + d_);
        uv -= delta * clamp(v.xy, vec2(-max_speed), vec2(max_speed));

        // propagate
        v = texture(bufA, uv);

        vec4 a = texture(bufA, uv - delta);
        vec4 b = texture(bufA, uv + vec2(delta.x, -delta.y));
        vec4 c = texture(bufA, uv + vec2(-delta.x, delta.y));
        vec4 d = texture(bufA, uv + delta);
        vec4 avg = 0.25 * (a + b + c + d);
        v = mix(v, avg, dissipate);

        vec4 ddx = (b + d) - (a + c);
        vec4 ddy = (c + d) - (a + b);

        float divergence = ddx.x + ddy.y;
        v.xy -= vec2(ddx.z, ddy.z) * accel;
        v.z  -= divergence * springiness;
        // Buoyancy: heat (w) pushes the fluid UP — strong so the plume fills.
        v.xy += (v.w) * vec2(0.0, 1.0) * buoyancy * 0.20;

        // ── Heat SOURCE: a wide turbulent flame band along the bottom edge,
        //    seeded every frame so fire rises and fills the frame (a single
        //    dim dot read as black). Noise gives the licking-flame texture. ──
        vec2 fc  = gl_FragCoord.xy;
        vec2 res = RENDERSIZE.xy;
        float uy = fc.y / res.y;
        float n = fract(sin(fc.x * 0.21 + TIME * 3.0) * 43758.5)
                * fract(sin(fc.x * 0.07 - TIME * 2.1) * 24634.6);
        float baseBand = smoothstep(0.20, 0.0, uy);                 // bottom ~20%
        float flames = baseBand * (0.5 + 0.9 * n)
                     * (0.6 + 0.4 * sin(TIME * stirSpeed * 6.0 + fc.x * 0.05));
        v.w += flames * 0.7;                            // heat
        v.y += flames * 1.0;                            // upward jet
        v.x += (n - 0.5) * baseBand * 1.3;              // sideways turbulence

        v.w *= 0.985;                                   // heat cools as it rises

        if (FRAMEINDEX < 4) { gl_FragColor = vec4(0.0); return; }
        gl_FragColor = clamp(v * 0.999, vec4(-1), vec4(1));
        return;
    }

    // ───────── Pass 1 — image (fire ramp from heat + motion) ─────────
    vec4 fld = texture(bufA, uv);
    float heat = clamp(fld.w * 1.4 + length(fld.xy) * 0.4, 0.0, 1.0);
    // black → deep red → orange → yellow → white-hot
    vec3 fire = mix(vec3(0.0),            vec3(0.7, 0.05, 0.0), smoothstep(0.0,  0.25, heat));
    fire      = mix(fire,                 vec3(1.0, 0.35, 0.0), smoothstep(0.20, 0.50, heat));
    fire      = mix(fire,                 vec3(1.5, 1.0,  0.2), smoothstep(0.45, 0.80, heat));
    fire      = mix(fire,                 vec3(1.8, 1.7,  1.3), smoothstep(0.80, 1.0,  heat));
    gl_FragColor = vec4(fire * colorScale.rgb * exposure, 1.0);
}
