/*{
  "CATEGORIES": ["Generator", "Light", "Audio Reactive"],
  "DESCRIPTION": "Single-source laser fan — 4 to 12 thin HDR beams emanate from a central point and rotate slowly through volumetric haze. Each beam carries its own curated color (red/green/blue/cyan/magenta/yellow/white). FBM-noise modulates brightness along each beam so the volumetric smoke shows through invisible patches. A BPM-style synchronized pulse kicks every beat and ripples outward. Output is linear HDR — core peaks 2.0-3.0 for downstream bloom.",
  "INPUTS": [
    { "NAME": "beamCount",     "LABEL": "Beam Count",     "TYPE": "long",  "DEFAULT": 6, "VALUES": [4,6,8,12], "LABELS": ["4","6","8","12"] },
    { "NAME": "rotationSpeed", "LABEL": "Rotation",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.45 },
    { "NAME": "hazeIntensity", "LABEL": "Haze",           "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.85 },
    { "NAME": "colorMode",     "LABEL": "Color Mode",     "TYPE": "long",  "DEFAULT": 0, "VALUES": [0,1,2], "LABELS": ["Mono","Rainbow","Custom"] },
    { "NAME": "audioReact",    "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }
  ]
}*/

// LASER FAN — one source, N beams, slow rotation, BPM-synced pulse.

#define PI 3.14159265359
#define TAU 6.28318530718

float h21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = h21(i), b = h21(i + vec2(1.0, 0.0));
    float c = h21(i + vec2(0.0, 1.0)), d = h21(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) { v += a * vnoise(p); p = p * 2.03 + 17.0; a *= 0.5; }
    return v;
}

// Curated laser palette — 7 saturated HDR colors
vec3 paletteColor(int idx) {
    if (idx == 0) return vec3(2.40, 0.18, 0.22);   // red
    if (idx == 1) return vec3(0.22, 2.60, 0.42);   // green
    if (idx == 2) return vec3(0.32, 0.55, 2.70);   // blue
    if (idx == 3) return vec3(0.30, 2.40, 2.50);   // cyan
    if (idx == 4) return vec3(2.50, 0.30, 2.40);   // magenta
    if (idx == 5) return vec3(2.50, 2.30, 0.30);   // yellow
    return            vec3(2.40, 2.40, 2.55);      // white
}

// HSV→RGB for rainbow mode (kept HDR by scaling)
vec3 hsv(float h, float s, float v) {
    vec3 k = mod(vec3(5.0, 3.0, 1.0) + h * 6.0, 6.0);
    k = clamp(min(k, 4.0 - k), 0.0, 1.0);
    return v * mix(vec3(1.0), k, s);
}

vec3 beamColorFor(int i, int N, int mode, float t) {
    if (mode == 0) {
        // MONO — single color, slowly cycles through palette
        int idx = int(mod(floor(t * 0.20), 7.0));
        return paletteColor(idx);
    }
    if (mode == 1) {
        // RAINBOW — distribute hues evenly around the wheel, drift over time
        float h = fract(float(i) / float(N) + t * 0.05);
        return hsv(h, 1.0, 2.45);
    }
    // CUSTOM — each beam picks a different palette slot, deterministic
    int idx = int(mod(float(i) * 2.0 + 1.0, 7.0));
    return paletteColor(idx);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Stage coords: x in [-aspect, aspect], y in [-1, 1]
    vec2 p = uv * 2.0 - 1.0;
    p.x *= aspect;

    float t  = TIME;
    float aR = clamp(audioReact, 0.0, 2.0);

    // ─── BPM-style pulse — synthetic 120 BPM (2 Hz) beat envelope.
    // Each beat rises sharply and decays exponentially. Audio host can
    // amplify via audioReact.
    float bpm    = 120.0;
    float beatHz = bpm / 60.0;
    float beatPhase = fract(t * beatHz);
    // Sharp attack + exponential decay
    float beat = exp(-beatPhase * 6.0) * (0.5 + 0.5 * aR);

    // Vector from center source to current pixel
    vec2 src = vec2(0.0, 0.0);
    vec2 q   = p - src;
    float r  = length(q);
    float ang = atan(q.y, q.x);

    // Slow global rotation
    float rot = t * rotationSpeed;

    // Beam count from enum
    int N = (beamCount <= 4) ? 4 : (beamCount <= 6) ? 6 : (beamCount <= 8) ? 8 : 12;
    float Nf = float(N);

    // Folded angle into single beam-sector → distance to nearest beam axis
    // Each beam sits at angle (k * TAU/N + rot). The signed angular distance
    // to the nearest beam is computed via fold.
    float sector = TAU / Nf;
    float relAng = ang - rot;
    // Index of the nearest beam (used for per-beam color + breath seed)
    float nearestK = floor(relAng / sector + 0.5);
    int   ki       = int(mod(nearestK, Nf));
    float beamAng  = nearestK * sector + rot;
    // Perpendicular distance from the beam axis (radius * sin of angle delta)
    float dAng = ang - beamAng;
    // wrap into [-PI, PI]
    dAng = mod(dAng + PI, TAU) - PI;
    float perp = r * dAng;            // approximate perpendicular distance
    float along = r;                  // distance along the beam from source

    // ─── HAZE FIELD — drifting fbm
    vec2  hazeUV   = vec2(p.x * 1.1, p.y * 1.1 + t * 0.05);
    float hazeBase = fbm(hazeUV * 1.5 + vec2(0.0, t * 0.04));
    float hazeFine = fbm(hazeUV * 3.4 + vec2(t * 0.08, 0.0));
    float haze     = hazeBase * 0.65 + hazeFine * 0.35;
    haze = clamp(haze * hazeIntensity, 0.0, 1.4);

    // ─── BEAM BREATHING — fbm along the beam axis creates invisible
    // patches in the smoke so beams visibly pulse as they cut through.
    float seed     = float(ki) * 1.91;
    vec2  axisCoord = vec2(along * 4.0 + seed, t * 0.55 + seed * 7.13);
    float breathe   = fbm(axisCoord);
    breathe = smoothstep(0.18, 0.78, breathe);

    // BPM pulse ripples outward from the source as a radial wave.
    float pulseRing = exp(-pow((along - beatPhase * 2.2) * 4.0, 2.0)) * beat;

    // ─── BEAM SHAPE — sharp gaussian core + soft volumetric halo
    float core = exp(-perp * perp * 18000.0);
    core      += exp(-perp * perp * 3500.0) * 0.55;

    float haloW = 0.045;
    float halo  = exp(-perp * perp / (haloW * haloW));
    halo *= (0.20 + 1.10 * haze) * breathe;

    // Source glow — small bright bloom right at the origin
    float srcGlow = exp(-r * r * 80.0) * 1.4;

    // Falloff with distance from source — beams thin as they reach the edge
    float reach   = smoothstep(2.4, 0.0, r);
    float endFade = smoothstep(0.0, 0.05, r);   // hide singularity at center
    float beamLine = (core * 0.95 + halo * 0.55) * reach * endFade;

    // BPM pulse boosts the beam intensity globally on each beat
    float beatBoost = 1.0 + 0.85 * beat;
    beamLine *= beatBoost;
    // Pulse ring adds a traveling brighter band along each beam
    beamLine += halo * pulseRing * 0.9 * reach;

    // Color this beam
    vec3 col3 = beamColorFor(ki, N, int(colorMode), t);
    vec3 col = col3 * beamLine;

    // Source glow inherits the average mood color (blend a few neighbors)
    vec3 srcCol = beamColorFor(ki, N, int(colorMode), t);
    col += srcCol * srcGlow * (0.7 + 0.4 * beat);

    // ─── ROOM AMBIENCE — deep cool black + faint smoke pickup
    vec3 ambient = vec3(0.012, 0.014, 0.022);
    float smokePickup = clamp(dot(col, vec3(0.299, 0.587, 0.114)) * 0.04, 0.0, 0.20);
    col += ambient + vec3(0.18, 0.20, 0.30) * haze * (0.05 + smokePickup);

    // Vignette
    vec2  vg  = uv - 0.5;
    float vig = clamp(1.0 - dot(vg, vg) * 0.85, 0.0, 1.0);
    col *= vig;

    // Tiny dither against banding in the haze
    col += (h21(gl_FragCoord.xy + fract(t) * 53.0) - 0.5) * 0.004;

    // Output LINEAR HDR (host applies tone-map / bloom)
    gl_FragColor = vec4(col, 1.0);
}
