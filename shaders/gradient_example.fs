/*{
	"DESCRIPTION": "Retro gradient foil — volumetric 3D fbm noise field drives a gradient, fused with diffraction-grating holographic foil, sparkle, paper grain, crystal/glitter/stripe/radial patterns, scan-bar, and audio reactivity. Single-pass, no feedback buffer.",
	"CREDIT": "merged: volumetric gradient + holographic foil",
	"ISFVSN": "2.0",
	"CATEGORIES": ["Generator", "Audio Reactive", "Atmospheric"],
	"INPUTS": [
		{ "NAME": "colorA",          "LABEL": "Gradient Top",      "TYPE": "color",  "DEFAULT": [0.05, 0.08, 0.22, 1.0] },
		{ "NAME": "colorB",          "LABEL": "Gradient Bottom",   "TYPE": "color",  "DEFAULT": [0.95, 0.42, 0.18, 1.0] },
		{ "NAME": "noiseScale",      "LABEL": "Noise Scale",       "TYPE": "float",  "MIN": 0.4,  "MAX": 6.0,    "DEFAULT": 1.8 },
		{ "NAME": "flowSpeed",       "LABEL": "Flow Speed",        "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,    "DEFAULT": 0.35 },
		{ "NAME": "depthIntensity",  "LABEL": "Depth Intensity",   "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,    "DEFAULT": 1.0 },
		{ "NAME": "foilPattern",     "LABEL": "Foil Pattern",      "TYPE": "long",   "VALUES": [0,1,2,3], "LABELS": ["Linear","Radial","Crystal","Glitter"], "DEFAULT": 2 },
		{ "NAME": "stripeFreq",      "LABEL": "Pattern Frequency", "TYPE": "float",  "MIN": 1.0,  "MAX": 60.0,   "DEFAULT": 16.0 },
		{ "NAME": "tiltSpeed",       "LABEL": "Tilt Speed",        "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,    "DEFAULT": 0.35 },
		{ "NAME": "tiltAmount",      "LABEL": "Tilt Amount",       "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,    "DEFAULT": 1.0 },
		{ "NAME": "foilBlend",       "LABEL": "Foil Blend",        "TYPE": "float",  "MIN": 0.0,  "MAX": 1.0,    "DEFAULT": 0.55 },
		{ "NAME": "sparkleDensity",  "LABEL": "Sparkle Density",   "TYPE": "float",  "MIN": 0.0,  "MAX": 1.0,    "DEFAULT": 0.55 },
		{ "NAME": "sparkleSize",     "LABEL": "Sparkle Size",      "TYPE": "float",  "MIN": 0.2,  "MAX": 3.0,    "DEFAULT": 1.0 },
		{ "NAME": "paperGrainAmt",   "LABEL": "Paper Grain",       "TYPE": "float",  "MIN": 0.0,  "MAX": 0.6,    "DEFAULT": 0.12 },
		{ "NAME": "hueRotateSpeed",  "LABEL": "Hue Rotate Speed",  "TYPE": "float",  "MIN": -1.0, "MAX": 1.0,    "DEFAULT": 0.08 },
		{ "NAME": "saturation",      "LABEL": "Saturation",        "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,    "DEFAULT": 1.35 },
		{ "NAME": "scanBarSpeed",    "LABEL": "Scan Bar Speed",    "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,    "DEFAULT": 0.5 },
		{ "NAME": "scanFreq",        "LABEL": "Scan Line Freq",    "TYPE": "float",  "MIN": 1.0,  "MAX": 4.0,    "DEFAULT": 2.0 },
		{ "NAME": "scanBarAmount",   "LABEL": "Scan Bar Amount",   "TYPE": "float",  "MIN": 0.0,  "MAX": 1.0,    "DEFAULT": 0.35 },
		{ "NAME": "glow",            "LABEL": "Bloom",             "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,    "DEFAULT": 0.7 },
		{ "NAME": "hologramTint",    "LABEL": "Scan Tint",         "TYPE": "color",  "DEFAULT": [0.4, 1.0, 0.95, 1.0] },
		{ "NAME": "audioReact",      "LABEL": "Audio React",       "TYPE": "float",  "MIN": 0.0,  "MAX": 2.0,    "DEFAULT": 1.0 }
	]
}*/

// ════════════════════════════════════════════════════════
//  UTILITY
// ════════════════════════════════════════════════════════

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash13(vec3 p)  { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453); }

vec2 hash22(vec2 p) {
    return vec2(hash21(p), hash21(p + 17.13));
}

vec3 hsv2rgb(vec3 c) {
    vec3 p = abs(fract(c.xxx + vec3(0.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0);
    return c.z * mix(vec3(1.0), clamp(p - 1.0, 0.0, 1.0), c.y);
}

vec3 satAdjust(vec3 c, float s) {
    float l = dot(c, vec3(0.299, 0.587, 0.114));
    return mix(vec3(l), c, s);
}

// ════════════════════════════════════════════════════════
//  3-D VALUE NOISE + FBM  (volumetric gradient engine)
// ════════════════════════════════════════════════════════

float vnoise3(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float n000 = hash13(i + vec3(0.0, 0.0, 0.0));
    float n100 = hash13(i + vec3(1.0, 0.0, 0.0));
    float n010 = hash13(i + vec3(0.0, 1.0, 0.0));
    float n110 = hash13(i + vec3(1.0, 1.0, 0.0));
    float n001 = hash13(i + vec3(0.0, 0.0, 1.0));
    float n101 = hash13(i + vec3(1.0, 0.0, 1.0));
    float n011 = hash13(i + vec3(0.0, 1.0, 1.0));
    float n111 = hash13(i + vec3(1.0, 1.0, 1.0));
    return mix(
        mix(mix(n000, n100, f.x), mix(n010, n110, f.x), f.y),
        mix(mix(n001, n101, f.x), mix(n011, n111, f.x), f.y),
        f.z);
}

float fbm3(vec3 p) {
    float v = 0.0;
    float a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * vnoise3(p);
        p  = p * 2.03 + vec3(11.7, 5.3, 17.1);
        a *= 0.5;
    }
    return v;
}

// ════════════════════════════════════════════════════════
//  2-D VALUE NOISE  (grain, sparkle)
// ════════════════════════════════════════════════════════

float vnoise2(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

// ════════════════════════════════════════════════════════
//  FOIL PATTERNS
// ════════════════════════════════════════════════════════

vec3 patternCrystal(vec2 uv, float freq, float tilt, float hueOff, float sat) {
    vec2 g    = uv * freq;
    vec2 cell = floor(g);
    vec2 fr   = fract(g);
    vec3 acc  = vec3(0.0);
    float wsum = 0.0;
    for (int jj = -1; jj <= 1; jj++) {
        for (int ii = -1; ii <= 1; ii++) {
            vec2 c  = cell + vec2(float(ii), float(jj));
            vec2 n2 = hash22(c) * 2.0 - 1.0;
            vec3 N  = normalize(vec3(n2, 0.6));
            vec3 V  = normalize(vec3(cos(tilt * 6.2831), sin(tilt * 6.2831), 0.85));
            float d  = dot(N, V) * 0.5 + 0.5;
            float hue = fract(d + hueOff);
            vec3 col  = hsv2rgb(vec3(hue, sat, 1.0));
            col += vec3(pow(d, 14.0));
            float dist = length(fr - (vec2(float(ii), float(jj)) + 0.5));
            float w = exp(-dist * 1.6);
            acc  += col * w;
            wsum += w;
        }
    }
    return acc / max(wsum, 1e-4);
}

vec3 patternGlitter(vec2 uv, float freq, float tilt, float hueOff, float sat, float size) {
    vec2 g    = uv * freq;
    vec2 cell = floor(g);
    vec2 fr   = fract(g) - 0.5;
    vec2 jitter = hash22(cell) - 0.5;
    float dist  = length(fr - jitter * 0.7);
    float pref  = hash21(cell + 7.31);
    float align = cos((tilt - pref) * 6.2831) * 0.5 + 0.5;
    float dot_  = smoothstep(0.18 * size, 0.0, dist) * pow(align, 6.0);
    float hue   = fract(pref + hueOff + tilt * 0.3);
    vec3 base   = hsv2rgb(vec3(hue, sat * 0.7, 0.55));
    return base + vec3(dot_) * (0.9 + align * 1.3);
}

float sparkleLayer(vec2 uv, float density, float size, float tilt) {
    float N    = 8.0 + density * 6.0;
    vec2 g     = uv * N;
    vec2 cell  = floor(g);
    vec2 fr    = fract(g) - 0.5;
    vec2 j     = hash22(cell + 91.7) - 0.5;
    float d    = length(fr - j * 0.8);
    float r    = mix(0.012, 0.04, size * 0.4);
    float phase = hash21(cell + 3.7);
    float prob  = hash21(cell + 19.4);
    if (prob > density) return 0.0;
    float align = pow(cos((tilt - phase) * 6.2831) * 0.5 + 0.5, 16.0);
    return smoothstep(r, 0.0, d) * align;
}

float paperGrain(vec2 uv) {
    float n = vnoise2(uv * 220.0) * 0.5 + vnoise2(uv * 90.0) * 0.5;
    return n - 0.5;
}

// ════════════════════════════════════════════════════════
//  MAIN
// ════════════════════════════════════════════════════════

void main() {

    vec2 uv = isf_FragNormCoord;
    vec2 p  = uv * 2.0 - 1.0;
    p.x    *= RENDERSIZE.x / RENDERSIZE.y;

    // ── Audio drivers ───────────────────────────────────
    float bass = audioBass  * audioReact;
    float mid  = audioMid   * audioReact;
    float treb = audioHigh  * audioReact;
    float lvl  = audioLevel * audioReact;

    // ── Volumetric camera ───────────────────────────────
    float t    = TIME * flowSpeed;
    float zPos = t * (1.0 + 0.6 * bass);
    vec3 camOff = vec3(
        sin(t * 0.31) * 0.6,
        cos(t * 0.27) * 0.4,
        zPos);
    vec3 rd = normalize(vec3(p, 1.4));

    // ── Raymarch noise field (14 steps) ─────────────────
    float density  = 0.0;
    float weightSum = 0.0;
    float maxCrest  = 0.0;
    float tNear = 0.4;
    float tFar  = 3.6;

    for (int i = 0; i < 14; i++) {
        float fi  = float(i) / 13.0;
        float zr  = mix(tNear, tFar, fi);
        vec3 wp   = camOff + rd * zr;
        float n   = fbm3(wp * noiseScale);
        float depthFade = 1.0 - smoothstep(0.6, 1.0, fi);
        density  += n * depthFade;
        weightSum += depthFade;
        maxCrest  = max(maxCrest, n * depthFade);
    }
    density /= max(weightSum, 0.001);

    float field = clamp(density * 1.4 - 0.15, 0.0, 1.2);

    // ── Gradient ────────────────────────────────────────
    float base     = uv.y;
    float modulator = (field - 0.5) * depthIntensity;
    float k         = clamp(base + modulator, 0.0, 1.0);
    k = smoothstep(0.0, 1.0, k);
    vec3 gradCol    = mix(colorB.rgb, colorA.rgb, k);

    // HDR crests
    float crest      = smoothstep(0.55, 0.95, maxCrest);
    float crestBoost = crest * (0.55 + 0.45 * treb) * (0.6 + depthIntensity * 0.4);
    gradCol += gradCol * crestBoost * 0.9;
    gradCol += vec3(0.7, 0.85, 1.0) * crest * 0.25 * (0.5 + treb);
    gradCol *= 1.0 + 0.18 * mid + 0.08 * lvl;

    // Depth vignette
    float vig = 1.0 - 0.35 * dot(p * 0.55, p * 0.55);
    gradCol  *= clamp(vig, 0.55, 1.0);

    // ── Foil diffraction tilt ───────────────────────────
    float tilt  = TIME * tiltSpeed
                + mid  * 0.7
                + p.x * 0.4 + p.y * 0.18;
    tilt       *= tiltAmount;
    // Extra tilt nudge from the noise field — gives foil a 3-D parallax feel
    tilt       += (field - 0.5) * 0.6 * depthIntensity;

    float hueOff = TIME * hueRotateSpeed + bass * 0.15;

    // ── Foil pattern ─────────────────────────────────────
    vec3 foilCol = vec3(0.0);
    int foilPatternI = int(foilPattern + 0.5);
    if (foilPatternI == 0) {
        float h = uv.x * stripeFreq + tilt * 4.0;
        foilCol = hsv2rgb(vec3(fract(h * 0.06 + hueOff), saturation, 1.0));
        float stripe = 0.5 + 0.5 * cos(h * 6.2831 / 12.0);
        foilCol *= 0.7 + 0.3 * stripe;
    } else if (foilPatternI == 1) {
        float h = length(uv - 0.5) * stripeFreq * 0.6 - tilt * 4.0;
        foilCol = hsv2rgb(vec3(fract(h * 0.08 + hueOff), saturation, 1.0));
        float ring = 0.5 + 0.5 * cos(h * 6.2831 / 8.0);
        foilCol *= 0.65 + 0.35 * ring;
    } else if (foilPatternI == 2) {
        foilCol = patternCrystal(uv, stripeFreq * 0.4 + 4.0, tilt, hueOff, saturation);
    } else {
        foilCol = patternGlitter(uv, stripeFreq * 0.8 + 6.0, tilt, hueOff, saturation, sparkleSize);
    }

    // Sparkles — high frequency reacts to treble
    float spkD = clamp(sparkleDensity * (0.7 + treb * 0.9), 0.0, 1.0);
    float spk  = sparkleLayer(uv, spkD, sparkleSize, tilt);
    foilCol   += vec3(spk) * vec3(1.0, 0.98, 0.92) * 1.6;

    // Paper grain
    foilCol += vec3(paperGrain(uv) * paperGrainAmt);

    // ── Blend gradient + foil ────────────────────────────
    // foilBlend 0 = pure gradient, 1 = pure foil.
    // We multiply foil by gradient luminance so the 3-D structure
    // is preserved — the foil lives inside the gradient's shadows/crests.
    float gradLum = dot(gradCol, vec3(0.299, 0.587, 0.114));
    vec3 foilTinted = foilCol * (0.55 + 0.9 * gradLum);
    vec3 col = mix(gradCol, foilTinted, foilBlend);

    // ── Retro scan-bar / interlace ───────────────────────
    float barPos = fract(TIME * scanBarSpeed * (1.0 + mid * 0.5));
    float bar    = exp(-pow((uv.y - barPos) * 9.0, 2.0));
    col += hologramTint.rgb * bar * 0.3 * scanBarAmount;

    // Scan-line grid — pinned to pixel rows
    col *= 0.88 + 0.12 * sin(gl_FragCoord.y * scanFreq * 0.5);

    // Edge bloom from foil highlights
    float lum = dot(col, vec3(0.299, 0.587, 0.114));
    col += hologramTint.rgb * pow(lum, 2.0) * glow * 0.18;

    // ── Global saturation ────────────────────────────────
    col = satAdjust(col, saturation);

    // ── Vignette (screen-space) ──────────────────────────
    float vig2 = smoothstep(1.15, 0.25, length(p * 0.75));
    col *= 0.82 + 0.18 * vig2;

    // ── Soft Reinhard — keeps HDR crests from clipping ───
    col = col / (1.0 + col * 0.22);

    // ── Audio level master gain ──────────────────────────
    col *= 0.72 + lvl * 0.38;

    gl_FragColor = vec4(col, 1.0);
}