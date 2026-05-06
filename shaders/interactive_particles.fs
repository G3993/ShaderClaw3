/*{
  "DESCRIPTION": "Interactive Particles — infinite procedural particle field. Multiple streams of particles loop forever by drifting across the canvas via fract(time*speed+seed); no accumulating state means no decay. mouseX/mouseY (-1..1) steer the field — bind from MIDI/OSC/another source to let collaborators edit the flow live. Audio modulates density and glow (never gates). Outputs LINEAR HDR; host applies tonemap.",
  "CREDIT": "Reimagined as a procedural infinite field by Easel/ShaderClaw3",
  "CATEGORIES": ["Generator", "Particles", "Interactive", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "streamCount",    "LABEL": "Streams",         "TYPE": "long",  "DEFAULT": 3, "VALUES": [1,2,3,4,5], "LABELS": ["1","2","3","4","5"] },
    { "NAME": "particlesPerStream","LABEL": "Density",      "TYPE": "long",  "DEFAULT": 2, "VALUES": [0,1,2,3],   "LABELS": ["Sparse","Med","Dense","Storm"] },
    { "NAME": "flowSpeed",      "LABEL": "Flow Speed",      "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0,  "MAX": 1.5 },
    { "NAME": "flowAngle",      "LABEL": "Flow Angle",      "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 6.2832 },
    { "NAME": "spread",         "LABEL": "Stream Spread",   "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "swirl",          "LABEL": "Swirl",           "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "particleSize",   "LABEL": "Particle Size",   "TYPE": "float", "DEFAULT": 0.0014, "MIN": 0.0003, "MAX": 0.005 },
    { "NAME": "intensity",      "LABEL": "Intensity",       "TYPE": "float", "DEFAULT": 1.7,  "MIN": 0.5,  "MAX": 3.0 },
    { "NAME": "exposure",       "LABEL": "Exposure",        "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.2,  "MAX": 3.0 },
    { "NAME": "hueShift",       "LABEL": "Hue",             "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "hueSpread",      "LABEL": "Hue Spread",      "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "mouseX",         "LABEL": "Mouse X (-1..1)", "TYPE": "float", "DEFAULT": 0.0,  "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "mouseY",         "LABEL": "Mouse Y (-1..1)", "TYPE": "float", "DEFAULT": 0.0,  "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "mouseInfluence", "LABEL": "Steer Strength",  "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "mouseRadius",    "LABEL": "Steer Radius",    "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.05, "MAX": 1.5 },
    { "NAME": "audioReact",     "LABEL": "Audio React",     "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

// INTERACTIVE PARTICLES — fully procedural; no buffers, no state, no decay.
// Each particle's position is f(streamIndex, particleIndex, TIME) so the field
// is INFINITE BY DESIGN — every frame is computed fresh from TIME. Collaborators
// can bind mouseX / mouseY (and any of the float inputs) from MIDI/OSC/another
// source to "edit" the flow live without ever respawning anything.
// LINEAR HDR out; host applies tonemap.

#define PI 3.14159265359
#define TWO_PI 6.28318530718
#define MAX_PARTICLES 96

float h11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
vec2  h21(float n) { return vec2(h11(n), h11(n + 17.31)); }

// HSV → RGB (for HDR-friendly emissive colors).
vec3 hsv2rgb(vec3 c) {
    vec3 p = abs(fract(c.xxx + vec3(0.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0);
    return c.z * mix(vec3(1.0), clamp(p - 1.0, 0.0, 1.0), c.y);
}

int particleTier(int t) {
    return t == 0 ? 24 : t == 1 ? 48 : t == 2 ? 72 : MAX_PARTICLES;
}

// Compute a particle's procedural position in [0,1]^2.
// (streamIdx, partIdx) seed; TIME drives wrap; mouseX/Y steers when bound.
vec2 particlePos(int streamIdx, int partIdx, float t, float audio,
                 vec2 steerTarget, float steerW) {
    float sf = float(streamIdx);
    float pf = float(partIdx);
    float seed = pf * 0.137 + sf * 7.91;

    // Per-stream direction & speed (small variance per particle).
    float a   = flowAngle + sf * 1.21 + (h11(seed + 3.7) - 0.5) * 0.6;
    float spd = flowSpeed * (0.55 + 0.9 * h11(seed + 9.1)) * (1.0 + 0.25 * sf);
    vec2  dir = vec2(cos(a), sin(a));

    // Phase wraps forever via fract — INFINITE LOOP BY DESIGN.
    float phase = fract(t * spd + h11(seed));

    // Lateral offset across the stream's perpendicular axis.
    vec2  perp = vec2(-dir.y, dir.x);
    float lat  = (h11(seed + 1.3) - 0.5) * 2.0 * spread;

    // Base linear flow centered on (0.5, 0.5).
    vec2 base = vec2(0.5) + dir * (phase - 0.5) * 1.6 + perp * lat * 0.5;

    // Swirl: a slow rotational wobble around screen center, per-particle phase.
    float swA = t * (0.15 + 0.35 * h11(seed + 4.2)) + seed * 1.7;
    vec2  sw  = vec2(cos(swA), sin(swA)) * (0.04 + 0.06 * h11(seed + 5.5)) * swirl;
    base += sw;

    // Steer toward an external target (mouseX/Y or held mouse). Falloff with
    // distance — particles outside steerRadius are barely affected; this keeps
    // the global flow intact while letting the user "edit" a region.
    vec2 toT = steerTarget - base;
    float d  = length(toT);
    float w  = exp(-(d * d) / max(mouseRadius * mouseRadius, 1e-4)) * steerW;
    base += toT * clamp(w, 0.0, 0.85);

    // Wrap back into [0,1]^2 so streams seamlessly replace themselves.
    base = fract(base);

    // Audio-modulated micro-jitter (modulator, NEVER a gate — works at audio=0).
    float jA = t * 2.3 + seed * 11.0;
    base += (vec2(sin(jA), cos(jA * 1.3))) * 0.004 * (0.4 + audio);

    return base;
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = fragCoord / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Audio: pure modulator. audio=0 means a slightly less dense / less bright
    // field — never an empty one. Bass dominates density, mid/high add shimmer.
    float audio = clamp(audioBass * 1.0 + audioMid * 0.5 + audioHigh * 0.3, 0.0, 2.0);
    audio *= clamp(audioReact, 0.0, 2.0);

    // External steer target: prefer host mousePos when held, else mouseX/Y.
    // mouseX/Y are -1..1 so a MIDI knob centered at 0 means "no steering bias".
    vec2 mxy01 = vec2(mouseX, mouseY) * 0.5 + 0.5;     // -1..1 → 0..1
    vec2 steer = mix(mxy01, mousePos, step(0.5, mouseDown));
    float steerW = mouseInfluence * (0.35 + 0.65 * step(0.5, mouseDown)
                                    + 0.65 * step(0.001, length(vec2(mouseX, mouseY))));

    int sCount = clamp(int(streamCount), 1, 5);
    int pCount = particleTier(int(particlesPerStream));
    // Audio adds up to +50% density without ever subtracting.
    int activeCount = clamp(pCount + int(float(pCount) * 0.5 * audio), 1, MAX_PARTICLES);

    float drawSize = particleSize * aspect;
    vec3 color = vec3(0.0);

    for (int s = 0; s < 5; s++) {
        if (s >= sCount) break;
        float sf = float(s);
        for (int i = 0; i < MAX_PARTICLES; i++) {
            if (i >= activeCount) break;

            vec2 p = particlePos(s, i, TIME, audio, steer, steerW);

            vec2 d = uv - p;
            d.x *= aspect;
            float r = length(d);

            // Soft glow kernel — smooth 1/r falloff, never produces NaN.
            float c = drawSize / max(r, 1e-4);
            c = pow(c, intensity);

            // Per-particle hue: stream + index + a slow time drift for variety.
            float hue = fract(hueShift
                              + sf * 0.13
                              + h11(float(i) * 0.07 + sf * 3.1) * hueSpread
                              + TIME * 0.015);
            float sat = 0.7 + 0.25 * h11(float(i) * 0.31 + sf);
            float val = 0.6 + 0.6 * audio;
            vec3  col = hsv2rgb(vec3(hue, sat, val));

            color += c * col;
        }
    }

    // Subtle ambient floor so audio=0 still has presence (modulator, not gate).
    color += vec3(0.012, 0.014, 0.020);

    // LINEAR HDR out — host applies tonemap.
    color *= exposure;
    gl_FragColor = vec4(color, 1.0);
}
