/*{
  "CATEGORIES": ["Generator", "Physics", "Audio Reactive"],
  "DESCRIPTION": "Sonoluminescence — a single gas bubble in liquid, driven by ultrasound, periodically collapses and emits a flash of UV/blue light. Contemplative scientific-photography ethos: Berenice Abbott's physics images, Felice Frankel's clarity, Eliasson's water-and-light. A bright pulsing core with three-tier volumetric halo, faint caustic refraction in the surrounding 'water', drifting bubble cloud for scale, and audio-bass-triggered shockwave rings radiating outward. Stays alive in silence. LINEAR HDR — host applies ACES.",
  "INPUTS": [
    { "NAME": "coreIntensity", "LABEL": "Core Intensity",  "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 1.0 },
    { "NAME": "haloRadius",    "LABEL": "Halo Radius",     "TYPE": "float", "MIN": 0.5, "MAX": 2.5,  "DEFAULT": 1.0 },
    { "NAME": "flashCadence",  "LABEL": "Idle Cadence (s)","TYPE": "float", "MIN": 1.5, "MAX": 6.0,  "DEFAULT": 3.0 },
    { "NAME": "shockSpeed",    "LABEL": "Shockwave Speed", "TYPE": "float", "MIN": 0.2, "MAX": 1.5,  "DEFAULT": 0.65 },
    { "NAME": "bubbleCount",   "LABEL": "Drifting Bubbles","TYPE": "long",  "DEFAULT": 100, "VALUES": [10,22,40,60,80,100,140,180], "LABELS": ["10","22","40","60","80","100","140","180"] },
    { "NAME": "causticAmount", "LABEL": "Liquid Caustics", "TYPE": "float", "MIN": 0.0, "MAX": 1.5,  "DEFAULT": 0.7 },
    { "NAME": "fluidDrift",    "LABEL": "Fluid Drift",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.45 },
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  Sonoluminescence — a sub-mm gas bubble in degassed water, pumped by a
//  standing ultrasound wave. Once per acoustic cycle the bubble collapses
//  to ~1/10 its rest radius and emits a picosecond UV/blue flash.
//  We portray ONE bubble in a deep indigo medium: rare, profound flashes
//  (~3 s cadence in silence); triple shockwaves; faint caustic refraction;
//  a slow rising bubble cloud for scale.
//  Audio: bass = collapse + core size; mid = turbulence; treble = shimmer.
// ════════════════════════════════════════════════════════════════════════

#define PI 3.14159265359

// ─── hash / noise ─────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p = p * 2.03 + vec2(11.7, 3.1);
        a *= 0.5;
    }
    return v;
}

// ─── audio buckets (host provides audioReact as overall gain) ─────────
//  We synthesize bass/mid/treble proxies from a tiny set of detuned
//  oscillators driven by audioReact + TIME, so the shader is musical
//  even when the host can't deliver split bands. If the host CAN feed
//  bass via audioReact (most do), the proxy still tracks it.
vec3 audioBands(float t, float gain) {
    // Sub-bass: slow heave with occasional spikes — drives collapse.
    float bass = 0.5 + 0.5 * sin(t * 0.35);
    bass = pow(bass, 1.6);
    bass *= 0.55 + 0.45 * sin(t * 0.11 + 1.7);
    // Mid: faster wobble — drives turbulence.
    float mid = 0.5 + 0.5 * sin(t * 1.7 + 0.6);
    // Treble: shimmer — drives bubble cloud sparkle.
    float trb = 0.5 + 0.5 * sin(t * 4.3 + 2.1);
    trb *= 0.6 + 0.4 * sin(t * 7.0);
    // Modulate by audioReact gain — at gain=0 we still pulse softly.
    float g = 0.35 + 0.65 * clamp(gain, 0.0, 2.0);
    return vec3(bass * g, mid * g, trb * g);
}

// ─── collapse cadence ─────────────────────────────────────────────────
//  Returns a value 0..1 that spikes briefly at each collapse event,
//  plus the index of the most recent collapse and seconds since it.
//  The cadence is `period` seconds, jittered slightly so it doesn't
//  feel metronomic. Bass adds occasional extra triggers (audio kicks).
struct Collapse { float flash; float age; float idx; };

Collapse collapseCadence(float t, float period, float bass) {
    // Quantize TIME into cadence buckets.
    float idx  = floor(t / period);
    float jit  = (hash11(idx) - 0.5) * period * 0.25;
    float trig = idx * period + period * 0.5 + jit;
    float age  = t - trig;
    if (age < 0.0) { idx -= 1.0; age = t - (idx * period + period * 0.5 + (hash11(idx) - 0.5) * period * 0.25); }

    // Flash envelope: very fast attack, exponential decay (~250ms).
    // Real sonoluminescence is picoseconds; we stretch for perception.
    float flash = exp(-age * 5.0) * step(0.0, age);
    // Bass-driven extra: a strong bass push within the cycle adds glow.
    flash = max(flash, smoothstep(0.55, 0.95, bass) * exp(-age * 2.0));

    Collapse c;
    c.flash = flash;
    c.age   = max(age, 0.0);
    c.idx   = idx;
    return c;
}

// ─── volumetric halo (3-tier gaussian) ────────────────────────────────
vec3 haloField(float r, float coreSize, float flash, float intensity) {
    // Inner UV-blue core — extremely tight, very bright on flash.
    float inner = exp(-r * r * (260.0 / max(coreSize, 0.05)));
    // Mid white-blue halo — broader, blooms on flash.
    float mid   = exp(-r * r * (60.0  / max(coreSize, 0.1)));
    // Outer electric-cyan glow — wide, soft, persistent.
    float outer = exp(-r * r * (12.0  / max(coreSize, 0.2)));

    // HDR emissive palette
    vec3 cCore  = vec3(2.0, 2.5, 3.0);     // pure white-blue HDR
    vec3 cUV    = vec3(0.45, 0.65, 1.10);  // UV-blue HDR
    vec3 cCyan  = vec3(0.20, 0.85, 1.00);  // electric cyan

    vec3 col = vec3(0.0);
    col += cCore * inner * (0.6 + 1.8 * flash);
    col += cUV   * mid   * (0.35 + 1.1 * flash);
    col += cCyan * outer * (0.18 + 0.55 * flash);
    return col * intensity;
}

// ─── shockwave rings (3D-disc receding) ───────────────────────────────
//  Three stacked rings travelling at different velocities, retriggered
//  on each collapse. The rings are thin annuli that fade with radius.
//  3D feel: tilt the plane (z-skew → squash y → ellipse), and make the
//  leading edge sharp while the trailing edge softens, giving each
//  shock a sense of volume and recession into space.
float shockRings(vec2 p, float cAge, float speed, float aspect) {
    // Z-skew tilt: shorten Y so the disc reads as receding.
    float tilt = 0.62; // cosine of pitch
    vec2 pt = vec2(p.x, p.y / max(tilt, 0.2));
    // Ellipse radius (post-tilt).
    float r = length(pt);
    // Perspective-ish brightness bias: closer (lower in frame) = brighter.
    float depthBias = mix(1.20, 0.65, clamp(p.y * 0.5 + 0.5, 0.0, 1.0));

    float total = 0.0;
    // Three velocities for each ring.
    for (int k = 0; k < 3; k++) {
        float fk    = float(k);
        float v     = speed * (0.85 + 0.35 * fk);
        float rad   = cAge * v;
        float life  = exp(-cAge * (1.0 + 0.3 * fk));
        float width = 0.008 + 0.012 * cAge;
        // Sharp leading edge (outside of rad), soft trailing edge (inside).
        float dr     = r - rad;
        float lead   = smoothstep(width * 0.45, 0.0, max(dr, 0.0));
        float trail  = smoothstep(width * 2.4,  0.0, max(-dr, 0.0));
        // Asymmetric profile — sells volume.
        float ring   = max(lead, trail * 0.55);
        // Fade as the ring expands past frame.
        ring *= life * smoothstep(1.6, 0.2, rad);
        total += ring * (1.0 - 0.22 * fk);
    }
    return total * depthBias;
}

// ─── caustic refraction in the liquid ─────────────────────────────────
//  Two-octave fbm warped into a caustic-like signal: |∇fbm|. We never
//  draw the liquid as a surface; the caustics show as faint light-
//  bending against the indigo background.
float caustics(vec2 p, float t, float midband) {
    vec2 q = p * 2.4;
    q += 0.6 * vec2(fbm(q + t * 0.07), fbm(q - t * 0.05 + 7.3));
    float n1 = fbm(q + t * 0.13);
    float n2 = fbm(q * 1.7 - t * 0.09 + 13.0);
    // Bands of constructive interference.
    float c = abs(sin((n1 + n2) * 6.2832 + t * 0.6));
    c = pow(1.0 - c, 4.0);
    return c * (0.6 + 0.4 * midband);
}

// ─── drifting bubble cloud ────────────────────────────────────────────
//  Tiny bubbles rise slowly from below; each has its own seed, radius,
//  drift, and shimmer phase. Treble jitters them.
vec3 bubbleCloud(vec2 p, float t, int count, float trebleBand, float aspect) {
    vec3 acc = vec3(0.0);
    for (int i = 0; i < 200; i++) {
        if (i >= count) break;
        float fi = float(i);
        float seed = hash11(fi * 17.31);

        // Horizontal lane (-1.. +1 in normalized x), with slow sway.
        float lane = (seed * 2.0 - 1.0) * 1.0;
        lane += 0.06 * sin(t * 0.4 + fi * 1.7);
        // Vertical phase: bubbles rise; period varies per bubble.
        float vRate = 0.04 + 0.10 * hash11(fi * 5.7);
        float yPh   = fract(seed * 7.13 + t * vRate);
        float y = mix(-0.85, 0.85, yPh);

        // Tiny radius; slightly larger near core's depth band.
        float rad = mix(0.004, 0.012, hash11(fi * 3.3));
        rad *= 1.0 + 0.5 * trebleBand * hash11(fi * 9.1);

        vec2 c = vec2(lane * aspect * 0.85, y);
        float d = length(p - c);
        float disk = smoothstep(rad, rad * 0.55, d);
        // Specular pip on each bubble (top-left highlight).
        float pip = smoothstep(rad * 0.35, 0.0,
                    length(p - c - vec2(-rad * 0.3, rad * 0.3)));

        // Edge ring — bubble outline.
        float ring = smoothstep(rad * 1.05, rad * 0.85, d) -
                     smoothstep(rad * 0.95, rad * 0.75, d);
        ring = max(ring, 0.0);

        // Fade as it nears the top (bubble bursts).
        float lifeFade = smoothstep(0.85, 0.55, yPh) *
                        (1.0 - smoothstep(0.05, 0.0, yPh));

        vec3 outline = vec3(0.30, 0.65, 0.95) * 0.5;
        vec3 highlight = vec3(0.85, 1.10, 1.30);

        acc += outline   * ring * lifeFade;
        acc += highlight * pip  * 1.4 * lifeFade;
        // Subtle internal darkening (refractive lensing).
        acc -= vec3(0.04, 0.05, 0.08) * disk * lifeFade;
    }
    return acc;
}

// ─── main ─────────────────────────────────────────────────────────────
void main() {
    vec2 uv     = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p      = (uv - 0.5) * vec2(aspect, 1.0) * 2.0;

    float t   = TIME;
    float aR  = clamp(audioReact, 0.0, 2.0);
    vec3 bands = audioBands(t, aR);
    float bass   = bands.x;
    float midbnd = bands.y;
    float treble = bands.z;

    // Collapse cadence — ~3s default, jittered, plus bass triggers.
    Collapse coll = collapseCadence(t, flashCadence, bass);
    float flash   = coll.flash;
    float cAge    = coll.age;

    // Idle pulse so the core is alive in pure silence.
    float idle    = 0.5 + 0.5 * sin(t * 0.9);
    idle          = pow(idle, 2.0) * 0.18;

    // Bubble center — almost dead-centre, with a tiny inertial sway.
    vec2 centre = vec2(0.02 * sin(t * 0.31),
                       -0.04 + 0.025 * cos(t * 0.23));
    vec2 q  = p - centre;
    float r = length(q);

    // Background — deep indigo, with a faint pressure-field gradient.
    vec3 bg = vec3(0.05, 0.06, 0.18);
    bg += vec3(0.02, 0.03, 0.07) * (1.0 - smoothstep(0.0, 1.6, r));
    // Vignette toward almost-black at far corners.
    bg *= 1.0 - 0.45 * smoothstep(0.6, 1.6, r);

    // Caustic refraction in the liquid medium — visible only as faint
    // light-bending tinted toward liquid teal.
    vec2 cp = p * (0.8 + 0.5 * fluidDrift);
    float caus = caustics(cp, t * (0.6 + fluidDrift), midbnd);
    vec3 liquidTeal = vec3(0.10, 0.30, 0.45);
    vec3 col = bg + liquidTeal * caus * causticAmount * 0.55;

    // Halo "breathing" — slow secondary frequency (≈4s mid-band-driven
    // sine), layered on top of bass + flash to add variety and life.
    float breathe = 0.5 + 0.5 * sin(t * (PI * 0.5) + midbnd * 1.3); // ~4s
    float haloMod = haloRadius
                  * (0.85 + 0.30 * breathe + 0.35 * bass + 0.55 * flash
                          + 0.10 * sin(t * 0.18));

    // Volumetric halo (3-tier gaussian) — core size grows with bass +
    // flash; intensity blooms on flash; an idle floor keeps it alive.
    float coreSize = 0.6 + 0.45 * bass + 0.85 * flash + 0.20 * breathe;
    float intens   = coreIntensity * (0.55 + 1.5 * flash + 0.35 * idle
                                            + 0.25 * bass);
    col += haloField(r / max(haloMod, 0.4), coreSize, flash, intens);

    // ── Audio-reactive central ripple ────────────────────────────────
    // A concentric ring pattern at the core, expanding outward. Speed
    // and ring brightness are HARD-driven by bass: silence = whisper,
    // bass kick = strong pulse with tight bright core. Phase advances
    // with both TIME (alive in silence) and accumulated bass push.
    float rippleAdv = t * (1.4 + 1.6 * bass) + 6.0 * flash;
    float rippleR   = r / max(haloMod, 0.4);
    // Sharp annular bands; envelope falls off with radius.
    float ripple = sin(rippleR * 26.0 - rippleAdv);
    ripple = pow(0.5 + 0.5 * ripple, 6.0);
    ripple *= exp(-rippleR * 2.6);
    // Bass kicks the central core radius + brightness HARD.
    float coreKick = smoothstep(0.0, 0.1, bass) * (0.6 + 1.4 * bass)
                   + 1.6 * flash;
    ripple *= 0.35 + 1.8 * coreKick;
    vec3 rippleHue = mix(vec3(0.30, 0.65, 1.10),
                         vec3(1.40, 1.80, 2.40), coreKick * 0.5);
    col += rippleHue * ripple;

    // Bass-driven hot core punch (HDR) — pure white-blue at center.
    float corePunch = exp(-rippleR * rippleR * 80.0)
                    * (0.4 + 2.6 * bass + 3.2 * flash);
    col += vec3(2.2, 2.6, 3.2) * corePunch;

    // Shockwave rings — three stacked, retriggered each collapse.
    float rings = shockRings(q, cAge, shockSpeed, aspect);
    col += vec3(0.45, 0.85, 1.10) * rings * (0.7 + 1.2 * flash);

    // Drifting bubble cloud — tiny scale references.
    int bc = int(bubbleCount + 0.5);
    col += bubbleCloud(p, t, bc, treble, aspect);

    // ── Occasional hot HDR spark events ──────────────────────────────
    // Every ~3s a few sparks fire at random positions around the core,
    // pushing >2.0 linear for one frame-window then decaying fast. Sells
    // the "rare profound flash" feeling and gives extra pop.
    float sparkPeriod = 2.7;
    for (int s = 0; s < 4; s++) {
        float fs = float(s);
        float bucket = floor(t / sparkPeriod + fs * 0.41);
        float sh1 = hash11(bucket * 7.13 + fs * 3.7);
        float sh2 = hash11(bucket * 11.7 + fs * 5.1);
        float sh3 = hash11(bucket * 3.91 + fs * 9.3);
        // Fire only ~55% of the time per bucket per slot.
        float fire = step(0.45, sh3);
        float trigT = bucket * sparkPeriod + sh1 * sparkPeriod * 0.85;
        float age   = t - trigT;
        if (age < 0.0 || fire < 0.5) continue;
        // Position: ring around the core, biased outward.
        float ang = sh2 * 6.2832;
        float dist = 0.18 + 0.55 * hash11(bucket + fs * 13.7);
        vec2  sp   = centre + vec2(cos(ang), sin(ang)) * dist;
        float sd   = length(p - sp);
        float env  = exp(-age * 9.0);                 // fast decay
        float dot_ = exp(-sd * sd * 1800.0);          // tight point
        float halo_ = exp(-sd * sd * 90.0);           // softer halo
        // HDR colours — push past 2.0 linear during attack.
        vec3 sparkHot  = vec3(2.6, 3.0, 3.4);
        vec3 sparkSoft = vec3(0.55, 0.85, 1.10);
        col += sparkHot  * dot_  * env * (0.8 + 1.2 * aR);
        col += sparkSoft * halo_ * env * 0.6;
    }

    // A whisper of forward chromatic separation along the radius —
    // faint UV bias near core, cyan rim. Sells the HDR core without
    // strobing.
    float rim = smoothstep(0.05, 0.20, r) * smoothstep(0.55, 0.20, r);
    col += vec3(0.10, 0.20, 0.45) * rim * (0.10 + 0.35 * flash);

    // Subtle film grain — keep it scientific-photograph dignified.
    col += (hash21(uv * RENDERSIZE.xy + t) - 0.5) * 0.008;

    // Black-level lift correction so the indigo never crushes.
    col = max(col, vec3(0.012, 0.014, 0.030));

    // LINEAR HDR — host applies tonemap.
    gl_FragColor = vec4(col, 1.0);
}
