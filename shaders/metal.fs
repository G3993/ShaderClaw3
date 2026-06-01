/*{
  "DESCRIPTION": "Metal — a stable-fluids liquid-metal sim. Semi-Lagrangian velocity advection + an auto-driven stir injects dye and momentum; divergence is removed by a Jacobi pressure projection (cheap one-iteration-per-frame solve that converges temporally — real-time, unlike the original's ~760-tap single-pass convolution). Visualizes the concentration field as a fake-3D shaded metallic surface, with velocity/pressure debug views. Ported to Easel ISF from Schuetze/Vimont stable-fluids.",
  "CREDIT": "Robert Schuetze (trirop) + Ulysse Vimont 2017, CC BY-NC-SA 3.0. Fast-adapted ISF port for Easel.",
  "CATEGORIES": ["VFX", "Fluid", "Simulation", "Generator"],
  "INPUTS": [
    { "NAME": "stirSpeed",   "LABEL": "Stir Speed",   "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.3 },
    { "NAME": "force",       "LABEL": "Stir Force",   "TYPE": "float", "MIN": 0.0, "MAX": 6.0, "DEFAULT": 3.0 },
    { "NAME": "dye",         "LABEL": "Dye Rate",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6 },
    { "NAME": "dissipation", "LABEL": "Dissipation",  "TYPE": "float", "MIN": 0.95, "MAX": 1.0, "DEFAULT": 0.997 },
    { "NAME": "flowSpeed",   "LABEL": "Flow Speed",   "TYPE": "float", "MIN": 0.2, "MAX": 4.0, "DEFAULT": 1.6 },
    { "NAME": "viewMode",    "LABEL": "View (0 shaded,1 conc,2 vel,3 pressure)", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.0 },
    { "NAME": "exposure",    "LABEL": "Exposure",     "TYPE": "float", "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0 }
  ],
  "PASSES": [
    { "TARGET": "velBuf", "PERSISTENT": true },
    { "TARGET": "divBuf" },
    { "TARGET": "prsBuf", "PERSISTENT": true },
    {}
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  METAL — stable fluids (fast-adapted).
//    Pass 0 velBuf : advect velocity (semi-Lagrangian) + project by previous
//                    pressure gradient + auto-stir force/dye + boundaries.
//                    (xy = velocity, z = dye concentration)
//    Pass 1 divBuf : divergence of the velocity field.
//    Pass 2 prsBuf : ONE Jacobi pressure iteration (converges across frames).
//    Pass 3 image  : metallic visualization of the concentration field.
//  Half-float buffers store signed velocity / pressure directly.
// ════════════════════════════════════════════════════════════════════════

vec3 hsv2rgb_smooth(vec3 c) {
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

vec2 stirPos() {                 // normalized 0..1, circling
    float t = TIME * stirSpeed;
    return vec2(0.5) + 0.30 * vec2(cos(t), sin(t * 1.43));
}
vec2 stirVel() {                 // stir velocity direction (pixel-ish units)
    float t = TIME * stirSpeed;
    return vec2(-sin(t), 1.43 * cos(t * 1.43));
}

// Curl (scalar vorticity) of the velocity field at uv — used for vorticity
// confinement, which re-injects the swirly detail that advection smears out.
float curlAt(vec2 uv, vec2 texel) {
    float vL = texture(velBuf, uv - vec2(texel.x, 0.0)).y;
    float vR = texture(velBuf, uv + vec2(texel.x, 0.0)).y;
    float vB = texture(velBuf, uv - vec2(0.0, texel.y)).x;
    float vT = texture(velBuf, uv + vec2(0.0, texel.y)).x;
    return (vR - vL) - (vT - vB);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 texel = 1.0 / RENDERSIZE.xy;
    vec2 aspect = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

    // ───────── Pass 0 — velocity advection + projection + forces ─────────
    if (PASSINDEX == 0) {
        if (FRAMEINDEX < 4) { gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0); return; }

        float dt = flowSpeed;
        vec2 vel = texture(velBuf, uv).xy;

        // Semi-Lagrangian backtrace (advect velocity + dye by the field).
        vec2 src = uv - vel * texel * dt;
        vec4 a = texture(velBuf, src);
        vel = a.xy;
        float conc = a.z;

        // Projection: subtract the gradient of the previous-frame pressure.
        float pL = texture(prsBuf, uv - vec2(texel.x, 0.0)).x;
        float pR = texture(prsBuf, uv + vec2(texel.x, 0.0)).x;
        float pB = texture(prsBuf, uv - vec2(0.0, texel.y)).x;
        float pT = texture(prsBuf, uv + vec2(0.0, texel.y)).x;
        vel -= 0.5 * vec2(pR - pL, pT - pB);

        // Auto-stir: inject a swirling dye vortex at the moving stir point so
        // the field always churns and dye builds up (a single weak push read
        // as black). Tangential component makes it spin; radial dye fills it.
        vec2 m = stirPos();
        vec2 rel = (uv - m) * aspect;
        float fall = exp(-dot(rel, rel) / (0.11 * 0.11));
        vec2 tangent = normalize(vec2(-rel.y, rel.x) + 1e-5);
        vel += (stirVel() + tangent * 1.5) * force * fall;
        conc += dye * fall;

        // Vorticity confinement: push velocity toward swirl centers so the
        // flow forms wispy curling filaments instead of smearing flat.
        float cC = curlAt(uv, texel);
        float cL = abs(curlAt(uv - vec2(texel.x, 0.0), texel));
        float cR = abs(curlAt(uv + vec2(texel.x, 0.0), texel));
        float cB = abs(curlAt(uv - vec2(0.0, texel.y), texel));
        float cT = abs(curlAt(uv + vec2(0.0, texel.y), texel));
        vec2 g = vec2(cR - cL, cT - cB);
        g /= (length(g) + 1e-5);
        vel += vec2(g.y, -g.x) * cC * 1.2;

        conc *= dissipation;
        vel = clamp(vel * 0.9995, vec2(-8.0), vec2(8.0));   // stability clamp

        // No-slip boundaries.
        if (uv.x < texel.x || uv.x > 1.0 - texel.x) vel.x = 0.0;
        if (uv.y < texel.y || uv.y > 1.0 - texel.y) vel.y = 0.0;

        gl_FragColor = vec4(vel, clamp(conc, 0.0, 1.0), 1.0);
        return;
    }

    // ───────── Pass 1 — divergence ─────────
    if (PASSINDEX == 1) {
        float vl = texture(velBuf, uv - vec2(texel.x, 0.0)).x;
        float vr = texture(velBuf, uv + vec2(texel.x, 0.0)).x;
        float vb = texture(velBuf, uv - vec2(0.0, texel.y)).y;
        float vt = texture(velBuf, uv + vec2(0.0, texel.y)).y;
        float div = 0.5 * ((vr - vl) + (vt - vb));
        gl_FragColor = vec4(div, 0.0, 0.0, 1.0);
        return;
    }

    // ───────── Pass 2 — Jacobi pressure (one iteration / frame) ─────────
    if (PASSINDEX == 2) {
        if (FRAMEINDEX < 4) { gl_FragColor = vec4(0.0); return; }
        float pL = texture(prsBuf, uv - vec2(texel.x, 0.0)).x;
        float pR = texture(prsBuf, uv + vec2(texel.x, 0.0)).x;
        float pB = texture(prsBuf, uv - vec2(0.0, texel.y)).x;
        float pT = texture(prsBuf, uv + vec2(0.0, texel.y)).x;
        float div = texture(divBuf, uv).x;
        float p = (pL + pR + pB + pT - div) * 0.25;
        gl_FragColor = vec4(p, 0.0, 0.0, 1.0);
        return;
    }

    // ───────── Pass 3 — metallic visualization ─────────
    vec4 fld = texture(velBuf, uv);
    float concentration = fld.z;
    float pressure = texture(prsBuf, uv).x;
    float amplitude = length(fld.xy);
    float phase = atan(fld.y, fld.x) / 6.2831853;

    int mode = int(viewMode + 0.5);
    vec3 col;
    if (mode == 1) {
        col = vec3(concentration);
    } else if (mode == 2) {
        col = hsv2rgb_smooth(vec3(phase, 0.6, 1.0 - exp(-amplitude)));
    } else if (mode == 3) {
        float pl = texture(prsBuf, uv - vec2(texel.x, 0.0)).x;
        float pr = texture(prsBuf, uv + vec2(texel.x, 0.0)).x;
        float pt = texture(prsBuf, uv - vec2(0.0, texel.y)).x;
        float pb = texture(prsBuf, uv + vec2(0.0, texel.y)).x;
        vec2 grad = vec2(pr - pl, pb - pt);
        col = vec3(0.2 + 0.8 * max(dot(normalize(vec3(0.0, 1.0, 1.0)),
                   normalize(vec3(grad.x, 0.4, grad.y))), 0.0));
    } else {
        // Default: fake-3D shaded metallic surface. A dark chrome base reads
        // even at low dye, brightening to bluish metal where concentration is
        // high; the amplified gradient gives the molten 3D relief.
        float cl = texture(velBuf, uv - vec2(texel.x, 0.0)).z;
        float cr = texture(velBuf, uv + vec2(texel.x, 0.0)).z;
        float ct = texture(velBuf, uv - vec2(0.0, texel.y)).z;
        float cb = texture(velBuf, uv + vec2(0.0, texel.y)).z;
        vec2 grad = vec2(cr - cl, cb - ct);
        float shade = 0.3 + 0.7 * max(dot(normalize(vec3(0.0, 1.0, 1.0)),
                      normalize(vec3(grad.x * 4.0, 0.05, grad.y * 4.0))), 0.0);
        vec3 metal = mix(vec3(0.14, 0.17, 0.22), vec3(0.85, 0.9, 1.0),
                         clamp(concentration, 0.0, 1.0));
        col = metal * shade;
    }

    gl_FragColor = vec4(col * exposure, 1.0);
}
