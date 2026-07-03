/*{
  "DESCRIPTION": "Beat Flow — a Milkdrop-style feedback field: audio injects energy into a flowing simulation (bass drives the zoom of the velocity field, mids swirl it, every beat re-rolls the flow direction and burns in a shockwave that the field then carries away).",
  "CATEGORIES": ["Generator", "Fluid", "Audio Reactive"],
  "CREDIT": "Etherea",
  "INPUTS": [
    { "NAME": "audioReact", "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "speed",      "LABEL": "Speed",        "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "trailAmt",   "LABEL": "Trails",       "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "injectAmt",  "LABEL": "Injection",    "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "inputTex",   "LABEL": "Texture",      "TYPE": "image" },
    { "NAME": "texMix",     "LABEL": "Texture Mix",  "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0, "MAX": 1.0 }
  ],
  "PASSES": [
    { "TARGET": "fbBuf", "PERSISTENT": true },
    {}
  ]
}*/

// ── Beat Flow ────────────────────────────────────────────────
// Playbook technique #5: audio injects ENERGY into a feedback
// system with its own dynamics — impulse in, physics out. Bass
// modulates the velocity field (zoom), mids rotate it, beats gate
// structural randomness (flow direction re-rolls) and inject
// shockwave rings that the field advects. Louder music = longer
// trails; silence resolves to a clean, slowly-drifting nebula.

float knee(float x, float lo, float hi) { return smoothstep(lo, hi, x); }
float hash11(float p) { return fract(sin(p * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float fftLog(float t) {
    return texture2D(audioFFT, vec2(pow(clamp(t, 0.0, 1.0), 2.2) * 0.5, 0.5)).r;
}

void main() {
    float amt = clamp(audioReact, 0.0, 2.0);
    vec2 res = RENDERSIZE.xy;
    vec2 uv = gl_FragCoord.xy / res;

    float bassP = pow(knee(audioBass, 0.05, 0.85), 1.6) * amt;
    float midP  = pow(knee(audioMid,  0.08, 0.85), 1.3) * amt;
    float highP = pow(knee(audioHigh, 0.10, 0.90), 1.2) * amt;
    float beatP = audioBeatPulse * audioBeatPulse * amt;
    float drive = 0.25 + 0.75 * knee(audioEnergy, 0.05, 0.9) * min(amt, 1.0);
    float t = TIME * speed;

    if (PASSINDEX == 0) {
        vec2 p = uv - 0.5;
        p.x *= res.x / res.y;

        // ---- velocity field: bass -> zoom, mid -> swirl ---------------------
        float zoom = 1.0 - (0.004 + 0.012 * bassP);
        // beat re-rolls the rotation direction (quantized structural change)
        float beatSeed = hash11(floor(audioBassTime * 3.0 + 0.5) + 13.7);
        float dir = (beatSeed > 0.5) ? 1.0 : -1.0;
        float angV = dir * (0.002 + 0.010 * midP) + 0.0015 * sin(t * 0.21);
        float ca = cos(angV), sa = sin(angV);
        vec2 warped = mat2(ca, -sa, sa, ca) * p * zoom;
        warped.x /= res.x / res.y;
        warped += 0.5;

        // trail decay: louder = longer smears, quiet = clean resolve
        float decay = mix(0.90, 0.975, knee(audioEnergy, 0.08, 0.8)) * clamp(trailAmt, 0.0, 1.0)
                    + (1.0 - clamp(trailAmt, 0.0, 1.0)) * 0.85;
        vec3 prev = texture2D(fbBuf, warped).rgb * decay;

        // ---- injections ------------------------------------------------------
        float d = length(p);
        vec3 inj = vec3(0.0);

        // bass mass breathing at the core
        inj += mix(audioPalShadow, audioPalMid, knee(audioBrightness, 0.2, 0.8))
             * exp(-d * d * 22.0) * (0.10 * drive + 0.65 * bassP) * injectAmt;

        // beat shockwave ring — burned in, then advected by the field
        float age = 1.0 - audioBeatPulse;
        float ring = exp(-pow((d - mix(0.05, 0.9, age)) / mix(0.02, 0.10, age), 2.0));
        inj += mix(audioPalHigh, audioPalAccent, 0.6) * ring * beatP * 0.9 * injectAmt;

        // treble sparks: sparse, short-lived, carried away by the flow
        vec2 cell = floor(uv * vec2(96.0, 54.0));
        float sparkSeed = hash21(cell + floor(t * 10.0));
        float spark = step(0.9965 - highP * 0.0035, sparkSeed) * highP;
        inj += audioPalAccent * spark * 0.8 * injectAmt;

        // idle seed: faint drifting noise so the field never dies
        float idle = hash21(floor(uv * 40.0) + floor(t * 1.5)) < 0.0015 ? 1.0 : 0.0;
        inj += audioPalMid * idle * 0.25 * drive;

        // optional texture feed into the flow
        if (texMix > 0.001) {
            vec3 tex = texture2D(inputTex, uv).rgb;
            inj += tex * texMix * 0.12 * (0.3 + 0.7 * bassP);
        }

        gl_FragColor = vec4(max(prev, inj), 1.0);
        return;
    }

    // ---- final pass: present the field ---------------------------------------
    vec3 c = texture2D(fbBuf, uv).rgb;

    // arousal shapes global contrast; valence warms the lift
    float gammaV = mix(1.22, 0.85, clamp(audioArousal, 0.0, 1.0) * min(amt, 1.0));
    c = pow(max(c, 0.0), vec3(gammaV));
    c += audioPalShadow * 0.05 * drive;   // lifted black floor, never dead

    // gentle vignette
    vec2 vp = uv - 0.5;
    c *= 1.0 - dot(vp, vp) * 0.55;

    gl_FragColor = vec4(c, 1.0);
}
