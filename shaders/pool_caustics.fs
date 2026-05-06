/*{
  "CATEGORIES": ["Generator", "Atmospheric", "Audio Reactive"],
  "DESCRIPTION": "Underwater caustic light patterns — focused sun rays refracting through wave-rippled water and projecting bright dancing curves onto a tile floor below. Real-pool aesthetic with cyan tint, slow drift, audio-bass driving wave amplitude. Hypnotic, calming, instantly recognisable",
  "INPUTS": [
    { "NAME": "causticScale",    "LABEL": "Caustic Scale",    "TYPE": "float", "MIN": 1.0,  "MAX": 12.0, "DEFAULT": 4.5 },
    { "NAME": "causticContrast", "LABEL": "Caustic Contrast", "TYPE": "float", "MIN": 0.5,  "MAX": 6.0,  "DEFAULT": 2.6 },
    { "NAME": "waveSpeed",       "LABEL": "Wave Speed",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.35 },
    { "NAME": "sunAngle",        "LABEL": "Sun Angle",        "TYPE": "float", "MIN": -3.14159, "MAX": 3.14159, "DEFAULT": -2.2 },
    { "NAME": "tileVisibility",  "LABEL": "Tile Visibility",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "tileFreq",        "LABEL": "Tile Frequency",   "TYPE": "float", "MIN": 2.0,  "MAX": 24.0, "DEFAULT": 8.0 },
    { "NAME": "waterTint",       "LABEL": "Water Tint",       "TYPE": "color", "DEFAULT": [0.05, 0.42, 0.55, 1.0] },
    { "NAME": "sunColor",        "LABEL": "Sun Colour",       "TYPE": "color", "DEFAULT": [1.0, 0.96, 0.78, 1.0] },
    { "NAME": "audioReact",      "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "depthDarken",     "LABEL": "Depth Darken",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.45 }
  ]
}*/

// ============================================================
// Pool Caustics
// Two layers of fbm offset against each other; their squared
// difference produces the bright sharp caustic curves seen on
// the bottom of a sunlit swimming pool. Three octaves of caustic
// scale are summed for natural multi-frequency content.
// Audio: bass amplifies wave amplitude, mid drifts laterally,
// treble adds high-frequency detail.
// ============================================================

float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

// Standard fbm with rotated octaves to avoid axis-aligned bias.
float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    mat2 r = mat2(0.80, -0.60, 0.60, 0.80);
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p = r * p * 2.04;
        a *= 0.5;
    }
    return v;
}

// One caustic octave: two fbm samples drifted in opposite directions.
// Squaring the absolute difference produces the sharp bright curves.
float causticOctave(vec2 uv, float scale, float t, float amp, float treble) {
    vec2 v1 = vec2( 0.13,  0.07) * t;
    vec2 v2 = vec2(-0.09,  0.11) * t;
    // Treble adds a finer ripple component to sharpen edges.
    float fineRipple = 0.08 * treble * sin(uv.x * scale * 3.0 + t * 1.7)
                     * cos(uv.y * scale * 3.2 - t * 1.3);
    float n1 = fbm(uv * scale + v1) + fineRipple;
    float n2 = fbm(uv * scale * 1.3 + v2);
    float d = n1 - n2;
    return pow(abs(d), 2.0) * amp;
}

// Sum three caustic scales for natural multi-frequency look.
float caustic(vec2 uv, float t, float bass, float treble) {
    float baseScale = causticScale;
    float c = 0.0;
    c += causticOctave(uv, baseScale,        t,        1.00, treble);
    c += causticOctave(uv, baseScale * 1.9,  t * 1.3,  0.55, treble) * 0.7;
    c += causticOctave(uv, baseScale * 3.6,  t * 1.7,  0.30, treble) * 0.45;
    // Bass swells overall caustic amplitude.
    c *= 1.0 + 1.4 * bass;
    return c;
}

// Faint pool-floor tile grout lines.
float tileFloor(vec2 uv) {
    vec2 f = fract(uv * tileFreq);
    float gx = smoothstep(0.96, 0.99, f.x) + smoothstep(0.04, 0.01, f.x);
    float gy = smoothstep(0.96, 0.99, f.y) + smoothstep(0.04, 0.01, f.y);
    return clamp(gx + gy, 0.0, 1.0);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    // Aspect-correct so caustics aren't squashed.
    vec2 res = RENDERSIZE;
    vec2 p = (uv - 0.5) * vec2(res.x / res.y, 1.0);

    float bass   = clamp(audioBass   * audioReact, 0.0, 2.0);
    float mid    = clamp(audioMid    * audioReact, 0.0, 2.0);
    float treble = clamp(audioHigh   * audioReact, 0.0, 2.0);

    float t = TIME * waveSpeed;

    // Mid-driven lateral drift.
    vec2 drift = vec2(sin(TIME * 0.07) * 0.15 + mid * 0.18,
                      cos(TIME * 0.05) * 0.12);
    vec2 sampleUV = p + drift;

    // Sun direction (drifts slightly with time so highlight wanders).
    float ang = sunAngle + sin(TIME * 0.04) * 0.08;
    vec2 sunDir = vec2(cos(ang), sin(ang));
    // Project a soft directional offset into the caustic field — the
    // "sun rays" effectively shear the wave displacement.
    vec2 rayUV = sampleUV + sunDir * 0.06 * (1.0 + 0.5 * bass);

    float c = caustic(rayUV, t, bass, treble);
    c = pow(c, max(0.5, 1.0 / max(causticContrast, 0.001)));
    c *= causticContrast;

    // Compose: deep cyan ground + bright sun-coloured curves.
    vec3 ground = waterTint.rgb;

    // Faint tile grout darkens the ground slightly.
    float tile = tileFloor(sampleUV) * tileVisibility;
    ground *= (1.0 - tile * 0.55);

    // Caustic energy = sun colour multiplied by caustic, with subtle
    // additive falloff so highlights don't blow out.
    vec3 lightAdd = sunColor.rgb * c;
    lightAdd = lightAdd / (1.0 + lightAdd * 0.6);

    // Vignette / depth darkening — pool corners feel deeper.
    float r = length(p);
    float depth = 1.0 - depthDarken * smoothstep(0.2, 0.95, r);

    vec3 col = ground + lightAdd * 1.6;
    col *= depth;

    // Subtle blue-cyan global tint pulled toward water colour.
    col = mix(col, col * waterTint.rgb * 1.8, 0.18);

    // Beat-driven sparkle: a tiny secondary highlight where caustic peaks.
    float sparkle = smoothstep(0.55, 0.95, c) * (0.4 + 0.6 * bass);
    col += sunColor.rgb * sparkle * 0.25;

    gl_FragColor = vec4(col, 1.0);
}
