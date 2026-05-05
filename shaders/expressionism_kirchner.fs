/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "German Expressionism after Kirchner Street Berlin (1913), Nolde, Heckel — angular figures and tilted streets in acid Die Brücke palette, heavy black contour lines, carved-wood ridge texture. Single-pass, no buffer feedback. Anxiety on the boulevard.",
  "INPUTS": [
    { "NAME": "figureCount",   "LABEL": "Figure Count",     "TYPE": "float", "MIN": 2.0,  "MAX": 14.0, "DEFAULT": 7.0 },
    { "NAME": "perspective",   "LABEL": "Street Tilt",      "TYPE": "float", "MIN": 0.0,  "MAX": 0.6,  "DEFAULT": 0.30 },
    { "NAME": "tiltSpeed",     "LABEL": "Tilt Speed",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.6 },
    { "NAME": "walkSpeed",     "LABEL": "Walk Speed",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5 },
    { "NAME": "carveScale",    "LABEL": "Wood-Carve Scale", "TYPE": "float", "MIN": 4.0,  "MAX": 40.0, "DEFAULT": 18.0 },
    { "NAME": "carveStrength", "LABEL": "Carve Strength",   "TYPE": "float", "MIN": 0.0,  "MAX": 0.6,  "DEFAULT": 0.32 },
    { "NAME": "inkWeight",     "LABEL": "Contour Weight",   "TYPE": "float", "MIN": 0.0,  "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "posterize",     "LABEL": "Posterize",        "TYPE": "float", "MIN": 2.0,  "MAX": 8.0,  "DEFAULT": 4.0 },
    { "NAME": "anxiety",       "LABEL": "Anxiety Wobble",   "TYPE": "float", "MIN": 0.0,  "MAX": 0.20, "DEFAULT": 0.06 },
    { "NAME": "acidShift",     "LABEL": "Acid Shift",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0 },
    { "NAME": "audioReact",    "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "inputTex",      "LABEL": "Texture",          "TYPE": "image" }
  ]
}*/

// Die Brücke palette — acid sour greens, ultramarine, hot pink, vermillion,
// raw umber. Kirchner's Berlin street paintings live in this band.
const vec3 BRK_GREEN  = vec3(0.55, 0.78, 0.12);
const vec3 BRK_BLUE   = vec3(0.10, 0.22, 0.78);
const vec3 BRK_PINK   = vec3(0.95, 0.32, 0.55);
const vec3 BRK_VERM   = vec3(0.92, 0.18, 0.10);
const vec3 BRK_OCHRE  = vec3(0.78, 0.58, 0.18);
const vec3 BRK_PURPLE = vec3(0.42, 0.10, 0.55);
const vec3 BRK_BLACK  = vec3(0.08, 0.06, 0.08);
const vec3 BRK_PAPER  = vec3(0.85, 0.78, 0.62);

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}
// Sharp-ridge noise (carved-wood signifier)
float ridge(vec2 p) {
    float n = vnoise(p);
    return 1.0 - abs(2.0 * n - 1.0);
}
float ridgedFbm(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 5; i++) {
        s += a * ridge(p);
        p = mat2(1.6, 1.2, -1.2, 1.6) * p;
        a *= 0.55;
    }
    return s;
}

vec3 brkPalette(float idx) {
    int i = int(mod(idx + acidShift * 7.0, 7.0));
    if (i == 0) return BRK_GREEN;
    if (i == 1) return BRK_BLUE;
    if (i == 2) return BRK_PINK;
    if (i == 3) return BRK_VERM;
    if (i == 4) return BRK_OCHRE;
    if (i == 5) return BRK_PURPLE;
    return BRK_BLACK;
}

// Angular figure SDF — head + tilted body + arms, in normalized local coords.
// Returns a soft signed distance: <0 inside, >0 outside.
float sdFigure(vec2 lp, float seed) {
    // Head: tilted ellipse
    float headR = 0.18 + hash11(seed * 1.7) * 0.08;
    vec2 headP = lp - vec2(0.0, 0.62);
    float headTilt = (hash11(seed * 3.1) - 0.5) * 0.6;
    float ca = cos(headTilt), sa = sin(headTilt);
    headP = vec2(ca * headP.x - sa * headP.y, sa * headP.x + ca * headP.y);
    float head = length(headP / vec2(headR * 0.85, headR * 1.10)) - 1.0;

    // Body: trapezoid (wide at shoulders, narrow at waist)
    vec2 bodyP = lp - vec2(0.0, 0.10);
    float w = mix(0.32, 0.18, clamp((bodyP.y + 0.4) / 0.8, 0.0, 1.0));
    float bx = abs(bodyP.x) - w;
    float by = abs(bodyP.y) - 0.46;
    float body = max(bx, by);

    // Coat brim — flat hat
    vec2 hatP = lp - vec2(0.0, 0.78);
    float hat = max(abs(hatP.x) - 0.30, abs(hatP.y) - 0.04);

    return min(min(head, body), hat);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Anxiety wobble — global low-frequency UV jitter so figures sway.
    vec2 jitter = vec2(sin(TIME * 0.7) * 0.012, cos(TIME * 0.5) * 0.008) * (1.0 + audioBass * audioReact);
    vec2 wuv = uv + jitter * (anxiety / 0.06);

    // Street perspective tilt — horizontal shear keyed to time, like the
    // sloping pavement in Kirchner's "Street, Berlin".
    float shear = sin(TIME * tiltSpeed * 0.5) * perspective;
    wuv.x += (wuv.y - 0.5) * shear;

    // Background — striated street + carved-wood ridges; vertical stripes
    // suggest looming buildings.
    float vstripes = step(0.5, fract(wuv.x * 3.5 + sin(TIME * 0.2) * 0.3));
    vec3 bg = mix(BRK_OCHRE * 0.7, BRK_BLUE * 0.6, vstripes);
    bg = mix(bg, BRK_PAPER * 0.6, smoothstep(0.55, 0.0, wuv.y));

    // Carved-wood ridges over the whole field — gives the woodcut grain.
    float carve = ridgedFbm(wuv * carveScale + vec2(TIME * 0.05, 0.0));
    bg = mix(bg, BRK_BLACK, smoothstep(0.65, 0.85, carve) * carveStrength);

    vec3 col = bg;
    float minDist = 1e9;
    vec3 figureColor = BRK_BLACK;

    // Walk N figures across the canvas. Each figure has its own walking
    // phase so the crowd is staggered — no one steps in unison.
    int N = int(clamp(figureCount, 1.0, 14.0));
    for (int i = 0; i < 14; i++) {
        if (i >= N) break;
        float fi = float(i);
        // Walk position
        float walkPhase = TIME * walkSpeed * (0.6 + hash11(fi * 7.13) * 0.8) + fi * 0.7;
        float fx = fract(walkPhase * 0.05 + hash11(fi * 11.7));
        float fy = 0.20 + hash11(fi * 5.3) * 0.55;
        // Figures lean forward as they walk — couple lean to phase.
        float lean = sin(walkPhase * 2.0) * 0.10;

        vec2 ctr = vec2(fx, fy);
        // Head bob
        ctr.y += sin(walkPhase * 4.0) * 0.012;

        vec2 d = wuv - ctr;
        d.x *= aspect;
        // Apply lean (horizontal shear of the figure's local space)
        d.x += d.y * lean;
        // Scale: figures further "back" smaller, suggesting depth
        float scale = 0.18 + hash11(fi * 17.3) * 0.12;
        vec2 lp = d / scale;

        float dist = sdFigure(lp, fi);
        if (dist < minDist) {
            minDist = dist;
            // Pick a Die Brücke color for this figure's coat
            float pidx = mod(fi * 2.31 + floor(walkPhase * 0.1), 7.0);
            figureColor = brkPalette(pidx);
        }
    }

    // Composite the closest figure. SDF<0 → fill; just outside → black ink.
    float fillMix = smoothstep(0.005, -0.005, minDist);
    col = mix(col, figureColor, fillMix);

    // Heavy black contour line at the figure boundary.
    float contour = (1.0 - smoothstep(inkWeight, inkWeight + 0.005, abs(minDist)));
    col = mix(col, BRK_BLACK, contour);

    // Optional input texture — bleeds into figure fills only (so the
    // background remains painted).
    if (IMG_SIZE_inputTex.x > 0.0) {
        vec3 src = texture(inputTex, uv).rgb;
        // Snap to nearest Die Brücke color
        vec3 best = BRK_GREEN; float bd = 1e9;
        for (int k = 0; k < 7; k++) {
            vec3 cand = brkPalette(float(k));
            float dd = dot(src - cand, src - cand);
            if (dd < bd) { bd = dd; best = cand; }
        }
        col = mix(col, best, fillMix * 0.6);
    }

    // Posterize — collapse tonal subtlety, force the woodcut look.
    float steps = clamp(posterize, 2.0, 8.0);
    col = floor(col * steps) / steps;

    // Acid contrast push
    col = (col - 0.5) * 1.6 + 0.5;
    col = clamp(col, 0.0, 1.0);

    // Surprise: every ~19s a sodium-yellow streetlamp glow swells from
    // the upper edge — a Berlin window cracking open onto an alley.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 19.0);
        float _f  = smoothstep(0.0, 0.08, _ph) * smoothstep(0.32, 0.18, _ph);
        float _x  = 0.20 + 0.60 * fract(sin(floor(TIME / 19.0) * 71.3) * 43758.5453);
        float _r  = length((_suv - vec2(_x, 1.05)) * vec2(1.6, 1.0));
        col = mix(col, vec3(1.00, 0.78, 0.35), _f * smoothstep(0.40, 0.0, _r) * 0.6);
    }

    // Audio breath
    col *= 0.92 + audioLevel * audioReact * 0.12;

    gl_FragColor = vec4(col, 1.0);
}
