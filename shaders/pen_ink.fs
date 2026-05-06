/*{
  "CATEGORIES": ["Effect", "Stylized", "Audio Reactive"],
  "DESCRIPTION": "Pen & ink stylizer — converts an input image (or generates a procedural tone) into pen-and-ink illustration. Four modes: Cross-Hatch, Stippling, Contour, Sumi-e Brush. Input texture luminance drives line density. Bass thickens strokes, treble adds spatter. Inspired by Shadertoy XlKBzc.",
  "INPUTS": [
    { "NAME": "inputTex",      "LABEL": "Texture",         "TYPE": "image" },
    { "NAME": "inkMode",       "LABEL": "Mode",            "TYPE": "long",  "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Cross-Hatch", "Stippling", "Contour", "Sumi-e Brush"] },
    { "NAME": "lineDensity",   "LABEL": "Line Density",    "TYPE": "float", "MIN": 4.0,  "MAX": 80.0, "DEFAULT": 28.0 },
    { "NAME": "lineThickness", "LABEL": "Line Thickness",  "TYPE": "float", "MIN": 0.05, "MAX": 1.0,  "DEFAULT": 0.32 },
    { "NAME": "drift",         "LABEL": "Drift Speed",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.30 },
    { "NAME": "contrast",      "LABEL": "Tonal Contrast",  "TYPE": "float", "MIN": 0.4,  "MAX": 3.0,  "DEFAULT": 1.45 },
    { "NAME": "spatterAmt",    "LABEL": "Spatter",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25 },
    { "NAME": "paperGrain",    "LABEL": "Paper Grain",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.20 },
    { "NAME": "vignetteAmt",   "LABEL": "Vignette",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.30 },
    { "NAME": "audioReact",    "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

float h21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = h21(i);
    float b = h21(i + vec2(1.0, 0.0));
    float c = h21(i + vec2(0.0, 1.0));
    float d = h21(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    for (int k = 0; k < 5; k++) {
        v += a * vnoise(p);
        p = p * 2.03 + vec2(17.3, 7.1);
        a *= 0.5;
    }
    return v;
}

vec2 curl2(vec2 p, float t) {
    float e = 0.05;
    float a = fbm(p + vec2(0.0, e) + vec2(t, 0.0)) - fbm(p - vec2(0.0, e) + vec2(t, 0.0));
    float b = fbm(p + vec2(e, 0.0) - vec2(t, 0.0)) - fbm(p - vec2(e, 0.0) - vec2(t, 0.0));
    return vec2(a, -b);
}

float hatchBand(vec2 uv, float angle, float density, float phase, float thickness) {
    vec2 r = vec2(cos(angle), sin(angle));
    float u = dot(uv, r) * density + phase;
    float band = 0.5 + 0.5 * sin(u * 6.28318);
    float w = clamp(thickness, 0.04, 0.96);
    return smoothstep(1.0 - w, 1.0 - w * 0.3, band);
}

float stippleField(vec2 uv, float density, float tone, float phase) {
    vec2 g = uv * density;
    vec2 ip = floor(g + vec2(0.0, phase));
    vec2 fp = fract(g + vec2(0.0, phase));
    float seed = h21(ip);
    if (seed > tone + 0.05) return 0.0;
    vec2 c = fp - vec2(0.5, 0.5);
    float r = 0.18 + 0.32 * (1.0 - tone);
    return smoothstep(r, r * 0.4, length(c));
}

float contourField(float tone, float density, float thickness) {
    float s = tone * density;
    float nearest = abs(fract(s) - 0.5) * 2.0;
    float w = clamp(thickness, 0.05, 0.95);
    return smoothstep(w, w * 0.3, 1.0 - nearest);
}

float sumiBrush(vec2 uv, float t, float density, float thickness) {
    float ink = 0.0;
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        float ang = 0.6 + fi * 1.05 + 0.15 * sin(t * 0.27 + fi);
        vec2 r = vec2(cos(ang), sin(ang));
        float u = dot(uv + curl2(uv * 1.3, t * 0.2) * 0.15, r);
        float along = 0.55 + 0.45 * sin(u * density * 0.5 + t * 0.4 + fi * 1.7);
        float perp = abs(dot(uv, vec2(-r.y, r.x)) + sin(u * 1.3) * 0.1);
        float w = thickness * (0.25 + along * 0.25);
        ink = max(ink, smoothstep(w, w * 0.2, perp) * along);
    }
    return ink;
}

// Procedural fallback tonal field — used when no texture is bound to
// the inputTex slot. Keeps the shader visually alive standalone.
float proceduralTone(vec2 uv, float t, float aBass, float aMid) {
    vec2 q = uv * 1.6 + curl2(uv * 0.9, t * 0.15) * 0.25;
    float n = fbm(q + vec2(t * 0.07, -t * 0.05));
    float r = length(uv) * (0.95 - aBass * 0.10);
    float lobe = 1.0 - smoothstep(0.25, 1.05, r);
    return clamp(n * 0.6 + lobe * 0.55 + aMid * 0.10, 0.0, 1.0);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 cc = (uv - 0.5) * vec2(aspect, 1.0);
    float t = TIME;

    float aBass = clamp(audioBass * audioReact, 0.0, 1.5);
    float aMid  = clamp(audioMid  * audioReact, 0.0, 1.5);
    float aHigh = clamp(audioHigh * audioReact, 0.0, 1.5);

    // Sample input texture and use its luminance as the tonal source.
    // When no texture is bound, IMG_SIZE_inputTex is (0,0) and we fall
    // back to a procedural tone so the shader stays alive standalone.
    bool hasTex = IMG_SIZE_inputTex.x > 0.5 && IMG_SIZE_inputTex.y > 0.5;
    float tone;
    if (hasTex) {
        // Slight curl-noise drift on the sample coords so the strokes
        // animate against the underlying picture without warping it.
        vec2 sampleUV = uv + curl2(cc * 1.7, t * (drift + aMid * 0.4))
                              * (0.005 + aMid * 0.005);
        sampleUV = clamp(sampleUV, 0.0, 1.0);
        vec3 c = texture(inputTex, sampleUV).rgb;
        // Standard luma — bright = light tone (less ink).
        float lum = dot(c, vec3(0.299, 0.587, 0.114));
        // Invert: dark image regions → high "tone" → more ink.
        tone = clamp(1.0 - lum, 0.0, 1.0);
    } else {
        tone = proceduralTone(cc, t, aBass, aMid);
    }

    vec2 driftUV = cc + curl2(cc * 1.7, t * (drift + aMid * 0.4))
                         * (0.10 + aMid * 0.06);

    float thick = clamp(lineThickness + aBass * 0.20, 0.04, 0.96);
    float dens  = max(4.0, lineDensity * (1.0 + aBass * 0.15));
    float phase = t * (0.10 + drift * 0.4);

    float ink = 0.0;
    if (inkMode == 0) {
        float h1 = hatchBand(driftUV, 0.0,       dens, phase,       thick);
        float h2 = hatchBand(driftUV, 0.7853981, dens, phase * 1.2, thick);
        float h3 = hatchBand(driftUV, 1.5707963, dens, phase * 0.8, thick);
        float h4 = hatchBand(driftUV, 2.3561944, dens, phase * 1.4, thick);
        float k1 = smoothstep(0.15, 0.30, tone);
        float k2 = smoothstep(0.30, 0.50, tone);
        float k3 = smoothstep(0.50, 0.70, tone);
        float k4 = smoothstep(0.70, 0.90, tone);
        ink = max(max(h1 * k1, h2 * k2), max(h3 * k3, h4 * k4));
    } else if (inkMode == 1) {
        ink = stippleField(driftUV, dens * 1.2, tone, phase);
    } else if (inkMode == 2) {
        ink = contourField(tone, dens * 0.9, thick) * (0.2 + tone * 0.8);
    } else {
        float wash = smoothstep(0.45, 0.95, tone) * 0.55;
        ink = max(wash, sumiBrush(driftUV, t, dens, thick));
    }

    if (spatterAmt > 0.001) {
        float spLow  = step(0.985 - aBass * 0.05,
                            h21(floor(driftUV * 60.0)  + vec2(floor(t * 1.8))));
        float spHigh = step(0.997 - aHigh * 0.01,
                            h21(floor(driftUV * 200.0) + vec2(floor(t * 14.0))));
        ink = max(ink, (spLow + spHigh) * spatterAmt);
    }

    ink = pow(clamp(ink, 0.0, 1.0), 1.0 / max(0.01, contrast));

    // Hard-coded warm-paper / dark-ink palette. (Color uniforms removed
    // because a default-zero hazard was breaking the render.)
    vec3 paper = vec3(0.96, 0.94, 0.88);
    vec3 inkC  = vec3(0.06, 0.05, 0.07);
    vec3 col   = mix(paper, inkC, ink);

    float grain = (h21(uv * RENDERSIZE + vec2(floor(t * 25.0))) - 0.5)
                * paperGrain * 0.30;
    col += grain;

    float vig = smoothstep(1.05, 0.45, length(uv - 0.5) * 1.4);
    col *= mix(1.0, 0.65 + 0.35 * vig, vignetteAmt);

    gl_FragColor = vec4(col, 1.0);
}
