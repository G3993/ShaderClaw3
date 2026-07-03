/*{
  "DESCRIPTION": "Holographic Flow Field — gradient aurora streams merged with wave-interference ripples, holographic color pops, smooth abstract lines and iridescent shimmer. Single-pass, audio-reactive.",
  "CATEGORIES": ["Generator", "Flow", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "flowScale",      "LABEL": "Flow Scale",       "TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0,  "MAX": 12.0 },
    { "NAME": "flowSpeed",      "LABEL": "Flow Speed",       "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 4.0 },
    { "NAME": "octaves",        "LABEL": "Octaves",          "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0,  "MAX": 6.0 },
    { "NAME": "persistence",    "LABEL": "Persistence",      "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.1,  "MAX": 0.9 },
    { "NAME": "waveFrequency",  "LABEL": "Wave Frequency",   "TYPE": "float", "DEFAULT": 22.0, "MIN": 4.0,  "MAX": 80.0 },
    { "NAME": "waveSpeed",      "LABEL": "Wave Speed",       "TYPE": "float", "DEFAULT": 2.2,  "MIN": 0.1,  "MAX": 8.0 },
    { "NAME": "sourceCount",    "LABEL": "Wave Sources",     "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0,  "MAX": 12.0 },
    { "NAME": "drift",          "LABEL": "Source Drift",     "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "falloff",        "LABEL": "Wave Falloff",     "TYPE": "float", "DEFAULT": 0.9,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "lineSharpness",  "LABEL": "Line Sharpness",   "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "colorShift",     "LABEL": "Color Shift",      "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "gradientBlend",  "LABEL": "Gradient Blend",   "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "holoIntensity",  "LABEL": "Holo Pop",         "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "intensity",      "LABEL": "Brightness",       "TYPE": "float", "DEFAULT": 1.3,  "MIN": 0.2,  "MAX": 3.0 },
    { "NAME": "audioBoost",     "LABEL": "Audio Boost",      "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

// ─────────────────────────────────────────────────
// Hashes
// ─────────────────────────────────────────────────
float hash11(float p) {
    return fract(sin(p * 127.1 + 311.7) * 43758.5453);
}
vec2 hash22(float p) {
    return vec2(hash11(p), hash11(p + 17.31));
}
float rand1(vec2 x) {
    return fract(cos(mod(dot(x, vec2(13.9898, 8.141)), 3.14159)) * 43758.5453);
}
vec2 rand2v(vec2 x) {
    return fract(cos(mod(vec2(dot(x, vec2(13.9898, 8.141)),
                              dot(x, vec2(3.4562, 17.398))), vec2(3.14159))) * 43758.5453);
}

// ─────────────────────────────────────────────────
// Cellular FBM (flow field basis)
// ─────────────────────────────────────────────────
float cellular_noise(vec2 coord, vec2 size, float offset, float seed) {
    vec2 o = floor(coord) + rand2v(vec2(seed, 1.0 - seed)) + size;
    vec2 f = fract(coord);
    float d1 = 2.0, d2 = 2.0;
    for (float x = -1.0; x <= 1.0; x += 1.0) {
        for (float y = -1.0; y <= 1.0; y += 1.0) {
            vec2 nb = vec2(x, y);
            vec2 node = rand2v(mod(o + nb, size)) + nb;
            node = 0.5 + 0.25 * sin(offset * 6.28318 + 6.28318 * node);
            vec2 diff = nb + node - f;
            float dist = max(abs(diff.x), abs(diff.y));
            if (d1 > dist) { d2 = d1; d1 = dist; }
            else if (d2 > dist) { d2 = dist; }
        }
    }
    return d2 - d1;
}

float fbm_cellular(vec2 coord, vec2 size, int oct, float pers, float offset, float seed) {
    float val = 0.0, norm = 0.0, scale = 1.0;
    for (int i = 0; i < 8; i++) {
        if (i >= oct) break;
        val   += cellular_noise(coord * size, size, offset, seed) * scale;
        norm  += scale;
        size  *= 2.0;
        scale *= pers;
    }
    return val / norm;
}

// ─────────────────────────────────────────────────
// Smooth value noise for gradient fields
// ─────────────────────────────────────────────────
float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    float a = rand1(i);
    float b = rand1(i + vec2(1.0, 0.0));
    float c = rand1(i + vec2(0.0, 1.0));
    float d = rand1(i + vec2(1.0, 1.0));
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm_value(vec2 p, int oct) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 6; i++) {
        if (i >= oct) break;
        v += a * vnoise(p);
        p *= 2.1;
        a *= 0.5;
    }
    return v;
}

// ─────────────────────────────────────────────────
// Holographic / iridescent palette
// Aurora → violet → teal → lime → magenta → gold
// ─────────────────────────────────────────────────
vec3 holoSpectrum(float t) {
    // Six-stop vibrant gradient — wrap-around for seamless looping
    t = fract(t);
    vec3 stops[6];
    stops[0] = vec3(0.0,  0.85, 1.0 );  // cyan
    stops[1] = vec3(0.55, 0.05, 1.0 );  // violet
    stops[2] = vec3(0.0,  1.0,  0.55);  // electric teal/lime
    stops[3] = vec3(1.0,  0.15, 0.7 );  // magenta
    stops[4] = vec3(1.0,  0.82, 0.0 );  // gold
    stops[5] = vec3(0.0,  0.85, 1.0 );  // back to cyan
    float s = t * 5.0;
    int   ia = int(s);
    float fr = fract(s);
    // smooth cubic blend
    float sm = fr * fr * (3.0 - 2.0 * fr);
    // GLSL ES 1.0: no dynamic array indexing — select via constant loop.
    vec3 ca = stops[0];
    vec3 cb = stops[0];
    for (int k = 0; k < 6; k++) {
        if (k == ia)     ca = stops[k];
        if (k == ia + 1) cb = stops[k];
    }
    return mix(ca, cb, sm);
}

// Secondary deep-background gradient (dark purples / navys)
vec3 darkGradient(float t) {
    t = fract(t);
    if (t < 0.5) return mix(vec3(0.0, 0.02, 0.15), vec3(0.08, 0.0, 0.22), t * 2.0);
    return mix(vec3(0.08, 0.0, 0.22), vec3(0.0, 0.12, 0.18), (t - 0.5) * 2.0);
}

// ─────────────────────────────────────────────────
// Wave interference
// ─────────────────────────────────────────────────
const int MAX_SRC = 12;

vec2 sourcePos(int idx, float t, float driftAmt) {
    float fi = float(idx);
    vec2 base = 0.15 + 0.70 * hash22(fi * 7.13 + 1.7);
    float phx = hash11(fi * 3.91) * 6.28318;
    float phy = hash11(fi * 5.77) * 6.28318;
    float sx = sin(t * 0.11 + phx) + 0.5 * sin(t * 0.27 + phx * 1.3);
    float sy = cos(t * 0.09 + phy) + 0.5 * cos(t * 0.31 + phy * 1.7);
    return base + driftAmt * 0.18 * vec2(sx, sy);
}

float srcPhase(int idx) { return hash11(float(idx) * 11.71) * 6.28318; }

float waveField(vec2 p, float baseN, float freq, float speed, float bass) {
    float amp = 0.0, energy = 0.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 pa = vec2(p.x * aspect, p.y);
    for (int i = 0; i < MAX_SRC; i++) {
        float fi = float(i);
        float w = (fi < baseN) ? 1.0 :
            smoothstep(0.35 + 0.5 * hash11(fi * 2.13),
                       0.35 + 0.5 * hash11(fi * 2.13) + 0.15, bass)
            * (0.6 + 0.4 * sin(TIME * (1.7 + hash11(fi * 4.4)) + fi));
        if (w <= 0.0001) continue;
        vec2 sp = sourcePos(i, TIME, drift);
        sp.x *= aspect;
        float r  = max(distance(pa, sp), 0.004);
        float ph = srcPhase(i);
        float atten = 1.0 / pow(r, 0.5 * falloff);
        amp    += w * sin(r * freq - TIME * speed - ph) * atten;
        energy += w * atten;
    }
    if (energy > 1e-4) amp /= max(energy * 0.6, 0.5);
    return amp;
}

// ─────────────────────────────────────────────────
// Abstract line overlay — sharp iridescent bands
// ─────────────────────────────────────────────────
float lineBand(float v, float freq2, float sharp) {
    float s = sin(v * freq2 * 6.28318);
    // sharpen sine into thin lines
    float k = 4.0 + sharp * 28.0;
    return pow(max(s, 0.0), k);
}

// ─────────────────────────────────────────────────
// Main
// ─────────────────────────────────────────────────
void main() {
    vec2 uv  = isf_FragNormCoord;
    float t  = TIME;

    // Audio
    float bass = audioBass * audioBoost;
    float treb = audioHigh * audioBoost;
    float mids = audioMid  * audioBoost;
    float aLevel = max(audioLevel, 0.0) * audioBoost;

    // ── Flow field ──
    vec2 flowUV = uv * flowScale + t * flowSpeed * 0.06;
    float flowN = fbm_cellular(flowUV,
                               vec2(flowScale, flowScale),
                               int(octaves), persistence,
                               t * flowSpeed * 0.05, 0.0);
    float theta = flowN * 6.28318;
    vec2 flowDir = vec2(cos(theta), sin(theta));

    // Warped UV along flow for gradient layers
    vec2 warpUV  = uv + flowDir * 0.18;
    vec2 warpUV2 = uv + flowDir * 0.32 + 0.07 * vec2(sin(t * 0.3), cos(t * 0.25));

    // ── Value-noise gradient layers ──
    float g1 = fbm_value(warpUV  * 3.5 + t * flowSpeed * 0.04, int(octaves));
    float g2 = fbm_value(warpUV2 * 2.2 - t * flowSpeed * 0.03, int(octaves));
    float gBlend = mix(g1, g2, 0.5 + 0.5 * sin(t * 0.17));

    // ── Holographic color from flow ──
    float hue1  = fract(flowN * 1.7 + colorShift + t * 0.04);
    float hue2  = fract(gBlend * 2.1 + colorShift + t * 0.06 + 0.33);
    vec3 holoA  = holoSpectrum(hue1);
    vec3 holoB  = holoSpectrum(hue2);
    vec3 darkBg = darkGradient(fract(gBlend * 0.9 + t * 0.02));

    // Smooth gradient blend between holo colors and dark background
    float gradMix = smoothstep(0.25, 0.75, gBlend);
    vec3 gradColor = mix(darkBg, mix(holoA, holoB, gradMix), gradientBlend);

    // ── Wave interference ──
    float freqMod = waveFrequency * (1.0 + 0.8 * treb);
    float spdMod  = waveSpeed    * (1.0 + 0.3 * mids);
    float wamp    = waveField(uv, clamp(sourceCount, 1.0, float(MAX_SRC)),
                              freqMod, spdMod, bass);

    // Map wave amplitude to holographic palette
    float wt = clamp(0.5 + 0.5 * wamp, 0.0, 1.0);
    float waveHue = fract(wt * 0.8 + colorShift + t * 0.05 + 0.6);
    vec3 waveColor = holoSpectrum(waveHue);

    // ── Abstract line overlays ──
    // Lines driven by flow direction angle and value noise
    float lineV1 = lineBand(flowN + gBlend * 0.5, 6.0 + lineSharpness * 4.0, lineSharpness);
    float lineV2 = lineBand(gBlend * 1.3 + flowN * 0.4 + t * 0.03, 9.0, lineSharpness * 0.8);
    // Diagonal holographic line streaks
    float lineV3 = lineBand(uv.x * 1.5 + uv.y * 0.8 + flowN * 0.6 + t * flowSpeed * 0.07,
                             12.0 + treb * 5.0, lineSharpness);

    float lineHue1 = fract(hue1 + 0.25);
    float lineHue2 = fract(hue2 + 0.5);
    float lineHue3 = fract(colorShift + t * 0.08 + 0.72);
    vec3 lineColor  = holoSpectrum(lineHue1) * lineV1
                    + holoSpectrum(lineHue2) * lineV2 * 0.7
                    + holoSpectrum(lineHue3) * lineV3 * 0.9;

    // ── Iridescent shimmer layer ──
    // Thin iridescent bands from wave crests
    float shimmer = smoothstep(0.6, 1.0, abs(wamp));
    float shimHue  = fract(wt * 1.4 + hue1 * 0.5 + t * 0.09);
    vec3 shimColor = holoSpectrum(shimHue) * shimmer * 1.5;

    // ── Compose all layers ──
    // 1. base gradient
    vec3 col = gradColor;
    // 2. blend in wave interference
    float waveBlend = 0.35 + 0.3 * gradMix + 0.1 * aLevel;
    col = mix(col, waveColor, clamp(waveBlend, 0.0, 0.75));
    // 3. add abstract lines as additive pops
    col += lineColor * holoIntensity;
    // 4. shimmer on wave crests
    col += shimColor * holoIntensity * 0.5;

    // ── Holographic pop highlight: sharp bright crests ──
    float crest = smoothstep(0.65, 1.0, wt);
    vec3 crColor = mix(vec3(1.0), holoSpectrum(fract(wt + colorShift)), 0.4);
    col += crColor * crest * holoIntensity * 0.6 * (1.0 + bass * 0.5);

    // ── Vignette ──
    vec2 vc = uv - 0.5;
    float vig = 1.0 - 0.4 * dot(vc, vc) * 2.5;
    col *= clamp(vig, 0.0, 1.0);

    // ── Audio reactive lift ──
    float audioLift = 1.0 + audioBoost * (bass * 0.5 + aLevel * 0.3);
    col *= intensity * audioLift;

    gl_FragColor = vec4(col, 1.0);
}