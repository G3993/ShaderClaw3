/*{
  "DESCRIPTION": "Fireflow — a compact buoyant fluid. A single feedback buffer holds velocity (xy), a pressure-like term (z) and buoyancy (w); each step diffuses, advects along its own velocity, removes divergence, and lets buoyancy push upward while an auto-driven swirl injects heat — giving a rising, flame-like flow. The image pass maps the field to warm color. Ported to Easel ISF: feedback buffer persistent, keyboard reset dropped, mouse replaced by an automatic swirl.",
  "CREDIT": "Shadertoy buoyant-flow original — ISF port for Easel.",
  "CATEGORIES": ["VFX", "Fluid", "Simulation", "Generator"],
  "INPUTS": [
    { "NAME": "diffuse",   "LABEL": "Diffuse (3.5-11.5)", "TYPE": "float", "MIN": 1.0, "MAX": 12.0, "DEFAULT": 5.5 },
    { "NAME": "buoyancy",  "LABEL": "Buoyancy",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0 },
    { "NAME": "stirSpeed", "LABEL": "Swirl Speed","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.4 },
    { "NAME": "colorScale","LABEL": "Tint",      "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "exposure",  "LABEL": "Exposure",  "TYPE": "float", "MIN": 0.3, "MAX": 2.5, "DEFAULT": 1.0 },
    { "NAME": "speed",     "LABEL": "Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "audioReact","LABEL": "Audio React","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.35 },
    { "NAME": "inputTex",  "TYPE": "image", "LABEL": "Texture" },
    { "NAME": "texMix",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "LABEL": "Texture Mix" }
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

// ── Audio conditioning (playbook: soft knees + floors, never linear) ─────
float aKnee(float x, float lo, float hi) { return smoothstep(lo, hi, x); }
float aBassP() { return pow(aKnee(audioBass, 0.05, 0.85), 1.6); }  // structural weight (flame heat)
float aHighP() { return pow(aKnee(audioHigh, 0.10, 0.90), 1.2); }  // sparkle (embers)
float aBeatP() { return audioBeatPulse * audioBeatPulse; }         // decaying accent (flare)

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
        float spd = speed;
        vec2 fc  = gl_FragCoord.xy;
        vec2 res = RENDERSIZE.xy;
        float uy = fc.y / res.y;
        float n = fract(sin(fc.x * 0.21 + TIME * spd * 3.0) * 43758.5)
                * fract(sin(fc.x * 0.07 - TIME * spd * 2.1) * 24634.6);
        float baseBand = smoothstep(0.20, 0.0, uy);                 // bottom ~20%
        float flames = baseBand * (0.5 + 0.9 * n)
                     * (0.6 + 0.4 * sin(TIME * spd * stirSpeed * 6.0 + fc.x * 0.05));
        // Audio: bass stokes the flame source (the dominant structure), a
        // beat kicks a brief extra flare. Idle floor: audio 0 -> exactly
        // the authored fire.
        float stoke = 1.0 + audioReact * (3.2 * aBassP() + 2.2 * aBeatP());
        v.w += flames * 0.7 * stoke;                     // heat
        v.y += flames * 1.0 * stoke;                     // upward jet
        v.x += (n - 0.5) * baseBand * 1.3;              // sideways turbulence

        // Whole-plume audio lift: bass/beat add a soft, wide heat glow that
        // fades out toward the top of frame — the whole fire "breathes"
        // with the track (physical, in the persistent field, not a flat
        // post-process tint — playbook law 4/5). Idle floor: audio 0 ->
        // exactly zero, no lift.
        float glowBand = smoothstep(0.9, 0.0, uy);
        v.w += glowBand * audioReact * (1.3 * aBassP() + 1.8 * aBeatP()) * 0.06;

        v.w *= 0.985;                                   // heat cools as it rises

        if (FRAMEINDEX < 4) { gl_FragColor = vec4(0.0); return; }
        gl_FragColor = clamp(v * 0.999, vec4(-1), vec4(1));
        return;
    }

    // ───────── Pass 1 — image (fire ramp from heat + motion) ─────────
    // Bass/beat conditioning for this pass.
    float aB = audioReact * aBassP();
    float aBeat = audioReact * aBeatP();

    // Bass gives the flame a gentle turbulent wobble as it samples the field
    // (the dominant structure shifts, not just brightens) — a beat adds a
    // short extra shove. Idle floor: audio 0 -> exactly uv, no wobble.
    vec2 wobble = 0.075 * aB * vec2(sin(TIME * 3.1 + uv.y * 26.0), cos(TIME * 2.4 + uv.x * 22.0))
                + 0.05 * aBeat * vec2(sin(uv.y * 40.0), cos(uv.x * 34.0));
    vec4 fld = texture(bufA, uv + wobble);
    float heat = clamp(fld.w * 1.4 + length(fld.xy) * 0.4, 0.0, 1.0);
    // black → deep red → orange → yellow → white-hot
    vec3 fire = mix(vec3(0.0),            vec3(0.7, 0.05, 0.0), smoothstep(0.0,  0.25, heat));
    fire      = mix(fire,                 vec3(1.0, 0.35, 0.0), smoothstep(0.20, 0.50, heat));
    fire      = mix(fire,                 vec3(1.5, 1.0,  0.2), smoothstep(0.45, 0.80, heat));
    fire      = mix(fire,                 vec3(1.8, 1.7,  1.3), smoothstep(0.80, 1.0,  heat));

    // Bass/beat deepen the flame toward a rich molten-gold tone — a real
    // hue + value shift (mix, not add), so it can actually move a pixel
    // that's currently clipped white, unlike an additive gain which only
    // ever pushes further past the clip. The weight climbs high enough at
    // strong sustained bass to visibly warm even the white-hot core; at
    // typical listening levels (aB well under 1) it stays a gentle tint.
    // Idle floor: audio 0 -> weight 0 -> exactly the authored fire ramp.
    vec3 molten = vec3(1.05, 0.55, 0.14) * (0.55 + 0.5 * heat);
    float moltenWeight = clamp(2.6 * aB + 2.0 * aBeat, 0.0, 0.85);
    fire = mix(fire, molten, moltenWeight);

    // Highs: sparse ember sparkle scattered through the flame body — silence
    // keeps the clean flame, sound peppers a few bright embers through it.
    float emberN = fract(sin(dot(gl_FragCoord.xy, vec2(12.9898, 78.233)) + TIME * 7.0) * 43758.5453);
    float ember = step(0.965, emberN) * smoothstep(0.12, 0.6, heat);
    fire += vec3(1.6, 1.5, 1.2) * ember * audioReact * 2.4 * aHighP();

    // Bass/beat lift a rising ember haze through the dark surrounds — the
    // other part of the frame with real headroom. Reads as embers/heat-
    // shimmer breathing in the smoke. Idle floor: audio 0 -> the surrounds
    // stay clean black.
    float darkMask = 1.0 - smoothstep(0.0, 0.85, heat);
    fire += vec3(2.6, 0.7, 1.5) * (aB + aBeat) * darkMask;

    if (texMix > 0.001) {
        // The flame burns through the texture: sample it warped by the same
        // velocity field driving the fire's shimmer, then let it show only
        // where heat is low — the image is consumed as heat rises, not
        // pasted flat over the flame.
        vec2 texUV = clamp(uv + fld.xy * 0.06, 0.0, 1.0);
        vec3 texCol = texture2D(inputTex, texUV).rgb;
        vec3 blended = mix(texCol, fire, clamp(heat * 1.3, 0.0, 1.0));
        fire = mix(fire, blended, texMix);
    }

    gl_FragColor = vec4(fire * colorScale.rgb * exposure, 1.0);
}
