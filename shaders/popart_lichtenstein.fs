/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Pop Art mass-printing aesthetic — three switchable techniques: Ben-Day dots (Lichtenstein), 4-up silkscreen with mis-registered colour (Warhol), and Halftone Pop (Rosenquist). Posterize, RGB channel offset, comic outlines, optional speech bubble flashing a sound-word on bass.",
  "INPUTS": [
    { "NAME": "techniqueStyle", "LABEL": "Technique", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Ben-Day Dots", "4-Up Silkscreen", "Halftone Pop"] },
    { "NAME": "silkscreenShift", "LABEL": "Silkscreen Mis-Register", "TYPE": "float", "MIN": 0.0, "MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "silkscreenSat", "LABEL": "Silkscreen Saturation", "TYPE": "float", "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.4 },
    { "NAME": "posterizeLevels", "LABEL": "Posterize Levels", "TYPE": "float", "MIN": 2.0, "MAX": 6.0, "DEFAULT": 4.0 },
    { "NAME": "dotDensity", "LABEL": "Ben-Day Density", "TYPE": "float", "MIN": 40.0, "MAX": 280.0, "DEFAULT": 140.0 },
    { "NAME": "dotMaxRadius", "LABEL": "Dot Max Radius", "TYPE": "float", "MIN": 0.10, "MAX": 0.50, "DEFAULT": 0.36 },
    { "NAME": "outlineThreshold", "LABEL": "Outline Threshold", "TYPE": "float", "MIN": 0.02, "MAX": 0.30, "DEFAULT": 0.10 },
    { "NAME": "outlineWidth", "LABEL": "Outline Width", "TYPE": "float", "MIN": 0.5, "MAX": 6.0, "DEFAULT": 1.0 },
    { "NAME": "saturation", "LABEL": "Palette Saturation", "TYPE": "float", "MIN": 0.6, "MAX": 1.6, "DEFAULT": 1.15 },
    { "NAME": "speechBubble", "LABEL": "Speech Bubble", "TYPE": "bool", "DEFAULT": true },
    { "NAME": "speechWord", "LABEL": "Sound Word", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["WHAAM", "POW", "ZOK", "BAM"] },
    { "NAME": "bubbleX", "LABEL": "Bubble X", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.72 },
    { "NAME": "bubbleY", "LABEL": "Bubble Y", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.78 },
    { "NAME": "bubbleSize", "LABEL": "Bubble Size", "TYPE": "float", "MIN": 0.05, "MAX": 0.30, "DEFAULT": 0.14 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

const vec3 LIK_INK     = vec3(0.05, 0.04, 0.04);
const vec3 LIK_WHITE   = vec3(0.97, 0.95, 0.92);
const vec3 LIK_RED     = vec3(0.86, 0.13, 0.16);
const vec3 LIK_YELLOW  = vec3(0.97, 0.85, 0.12);
const vec3 LIK_BLUE    = vec3(0.10, 0.30, 0.85);
const vec3 LIK_FLESH   = vec3(0.96, 0.78, 0.62);

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

vec3 saturateC(vec3 c, float s) {
    float L = dot(c, vec3(0.299, 0.587, 0.114));
    return mix(vec3(L), c, s);
}

// Procedural sound-word — fat block letters drawn as union of rounded
// rectangles. Each word laid out at fixed positions so it reads at scale.
float drawWord(vec2 uv, int word) {
    // Normalise into a 6×2 cell grid that holds the word letters.
    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) return 0.0;
    float cellW = 1.0 / 6.0;
    int idx = int(floor(uv.x / cellW));
    vec2 cuv = vec2(fract(uv.x / cellW), uv.y);
    if (idx > 4) return 0.0;

    // Per-word, per-letter ASCII-ish mask — fat geometric letterforms.
    int letter = -1;
    if (word == 0) {        // WHAAM
        if (idx == 0)      letter = 22; // W
        else if (idx == 1) letter = 7;  // H
        else if (idx == 2) letter = 0;  // A
        else if (idx == 3) letter = 0;  // A
        else if (idx == 4) letter = 12; // M
    } else if (word == 1) { // POW
        if (idx == 0)      letter = 15; // P
        else if (idx == 1) letter = 14; // O
        else if (idx == 2) letter = 22; // W
        else return 0.0;
    } else if (word == 2) { // ZOK
        if (idx == 0)      letter = 25; // Z
        else if (idx == 1) letter = 14; // O
        else if (idx == 2) letter = 10; // K
        else return 0.0;
    } else {                // BAM
        if (idx == 0)      letter = 1;  // B
        else if (idx == 1) letter = 0;  // A
        else if (idx == 2) letter = 12; // M
        else return 0.0;
    }
    if (letter < 0) return 0.0;

    // Inset margin
    if (cuv.x < 0.10 || cuv.x > 0.90 || cuv.y < 0.10 || cuv.y > 0.90) return 0.0;
    cuv = (cuv - 0.10) / 0.80; // 0..1 inside letter cell

    // Crude generic letter shape via vertical/horizontal bar masks per-letter
    float result = 0.0;
    // Verticals
    float left = step(0.0, cuv.x) * step(cuv.x, 0.20);
    float right = step(0.80, cuv.x) * step(cuv.x, 1.0);
    float midX = step(0.40, cuv.x) * step(cuv.x, 0.60);
    // Horizontals
    float top = step(0.0, cuv.y) * step(cuv.y, 0.20);
    float bot = step(0.80, cuv.y) * step(cuv.y, 1.0);
    float midY = step(0.40, cuv.y) * step(cuv.y, 0.60);

    if (letter == 0)         result = left + right + top + midY;       // A (no bottom)
    else if (letter == 1)    result = left + top + bot + midY + right; // B
    else if (letter == 7)    result = left + right + midY;             // H
    else if (letter == 10)   result = left + midY + right * step(0.5, cuv.y) + right * step(cuv.y, 0.5); // K (approx)
    else if (letter == 12)   result = left + right + top;              // M
    else if (letter == 14)   result = left + right + top + bot;        // O
    else if (letter == 15)   result = left + right * step(0.5, 1.0 - cuv.y) + top + midY; // P
    else if (letter == 22)   result = left + right + bot;              // W
    else if (letter == 25)   result = top + bot + (step(cuv.y, cuv.x + 0.1) * step(cuv.x - 0.1, cuv.y)); // Z

    return clamp(result, 0.0, 1.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 px = 1.0 / RENDERSIZE.xy;

    // Source — texture or fallback procedural test pattern (RGB UV grid
    // so the user can read what the shader is doing without an input).
    vec3 raw;
    if (IMG_SIZE_inputTex.x > 0.0) {
        raw = texture(inputTex, uv).rgb;
    } else {
        // Whaam! radial starburst fallback — comic-explosion silhouette
        // in red/yellow on cobalt sky. The Lichtenstein iconic graphic.
        float aspectF = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
        vec2 cuv = (uv - vec2(0.5)) * vec2(aspectF, 1.0);
        float ang = atan(cuv.y, cuv.x);
        float r   = length(cuv);
        float burstR = 0.30 + 0.08 * sin(ang * 8.0 + TIME * 0.30);
        float burst      = step(r, burstR);
        float burstInner = step(r, burstR * 0.60);
        raw = mix(LIK_BLUE * 0.7, LIK_YELLOW, burst);
        raw = mix(raw, LIK_RED, burstInner);
    }

    float lum = dot(raw, vec3(0.299, 0.587, 0.114));
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    int tech = int(techniqueStyle);
    vec3 col;

    if (tech == 1) {
        // 4-UP SILKSCREEN (Warhol Marilyn-style) — 2×2 grid each tinted a
        // different palette, with mis-registered RGB channels.
        vec2 quad   = floor(uv * 2.0);
        float qid   = quad.x + quad.y * 2.0;
        vec2  quv   = fract(uv * 2.0);
        vec2  dirR  = vec2(cos(qid * 1.7),  sin(qid * 1.7))      * silkscreenShift;
        vec2  dirG  = vec2(cos(qid * 2.3 + 1.3), sin(qid * 2.3 + 1.3)) * silkscreenShift;
        vec2  dirB  = vec2(cos(qid * 0.7 - 1.0), sin(qid * 0.7 - 1.0)) * silkscreenShift;
        vec3  src;
        if (IMG_SIZE_inputTex.x > 0.0) {
            float r2 = texture(inputTex, quv + dirR).r;
            float g2 = texture(inputTex, quv + dirG).g;
            float b2 = texture(inputTex, quv + dirB).b;
            src = vec3(r2, g2, b2);
        } else {
            src = raw;
        }
        // High-contrast posterize per channel — silkscreen reduction.
        vec3 mis = vec3(step(0.45, src.r), step(0.45, src.g), step(0.45, src.b));
        // Per-quadrant Warhol palette tints.
        const vec3 TINT[4] = vec3[4](
            vec3(0.96, 0.18, 0.55),  // hot pink
            vec3(0.20, 0.78, 0.90),  // cyan
            vec3(0.97, 0.85, 0.18),  // yellow
            vec3(0.16, 0.28, 0.86)   // royal blue
        );
        int qi = int(qid);
        vec3 tint = TINT[qi];
        col = mix(tint * 0.6, tint + mis * 0.6, 0.65);
        // Saturation boost characteristic of Warhol prints.
        float Lc = dot(col, vec3(0.299, 0.587, 0.114));
        col = mix(vec3(Lc), col, silkscreenSat);
        // Hairline gap between quadrants
        vec2  qf  = fract(uv * 2.0);
        float gap = step(qf.x, 0.004) + step(0.996, qf.x)
                  + step(qf.y, 0.004) + step(0.996, qf.y);
        col *= 1.0 - clamp(gap, 0.0, 1.0) * 0.85;

    } else if (tech == 2) {
        // HALFTONE POP (Rosenquist billboard) — pure halftone over the
        // entire image, dot radius driven by per-pixel luminance.
        vec2  g = vec2(uv.x * aspect, uv.y) * dotDensity;
        vec2  gf = fract(g) - 0.5;
        float r  = (1.0 - lum) * dotMaxRadius
                 * (1.0 + audioMid * audioReact * 0.25);
        float dotD = length(gf);
        float dotFw = max(fwidth(dotD), 0.5 / dotDensity);
        float dot_ = smoothstep(r + dotFw, r - dotFw, dotD);
        // Pick foreground colour from an extended pop palette using the
        // posterized luminance as the index.
        float levels = max(2.0, floor(posterizeLevels));
        float lvl    = floor(lum * levels) / levels;
        vec3 fg = (lvl < 0.25) ? LIK_INK
                : (lvl < 0.50) ? LIK_RED
                : (lvl < 0.75) ? LIK_BLUE
                : LIK_YELLOW;
        col = mix(LIK_WHITE, fg, dot_);

    } else {
        // BEN-DAY DOTS (Lichtenstein default) — posterize to comic
        // primaries; only the shadow band is replaced by Ben-Day dots.
        float levels = max(2.0, floor(posterizeLevels));
        float lvl    = floor(lum * levels) / levels;
        if      (lvl < 0.25) col = LIK_INK;
        else if (lvl < 0.50) col = LIK_RED;
        else if (lvl < 0.75) col = LIK_YELLOW;
        else                 col = LIK_WHITE;

        if (lum > 0.18 && lum < 0.55) {
            vec2  g = vec2(uv.x * aspect, uv.y) * dotDensity;
            vec2  gf2 = fract(g) - 0.5;
            float r2  = (1.0 - lum) * dotMaxRadius
                      * (1.0 + audioMid * audioReact * 0.25);
            float dotD2 = length(gf2);
            float dotFw2 = max(fwidth(dotD2), 0.5 / dotDensity);
            float dot2 = smoothstep(r2 + dotFw2, r2 - dotFw2, dotD2);
            col = mix(LIK_WHITE, LIK_RED, dot2);
        }
    }

    // Edge detect on the COMPUTED luminance — works whether or not an
    // input texture is bound. dFdx/dFdy is per-pixel, so multiply the
    // result up to the scale of luminance gradient.
    float gx = dFdx(lum);
    float gy = dFdy(lum);
    float edge = abs(gx) + abs(gy);
    float outline = smoothstep(outlineThreshold,
                                outlineThreshold + 0.04,
                                edge * outlineWidth * 40.0);
    col = mix(col, LIK_INK, outline);

    col = saturateC(col, saturation * (0.9 + audioLevel * audioReact * 0.15));

    // Speech bubble — always present, sizes with TIME and audio.
    if (speechBubble) {
        vec2 bC = vec2(bubbleX, bubbleY);
        float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
        vec2 bD = (uv - bC); bD.x *= aspect;
        float bSz = bubbleSize
                  * (0.85 + 0.15 * sin(TIME * 1.5))
                  * (1.0 + audioBass * audioReact * 0.6);
        float ellipse = length(bD * vec2(1.0, 1.6)) - bSz;
        if (ellipse < 0.0) {
            // Bubble fill
            col = LIK_WHITE;
            // Letter rendering inside bubble
            vec2 wuv = (bD / bSz) * 0.5 + 0.5;
            float w = drawWord(wuv * vec2(1.4, 1.0) - vec2(0.2, 0.0),
                               int(speechWord));
            col = mix(col, LIK_INK, w);
        }
        // Outline ring around bubble
        col = mix(col, LIK_INK, smoothstep(0.0, 0.003,
                                           abs(ellipse) - 0.003));
        // Bubble tail — small triangle pointing toward bottom-left.
        // (Simplified: a darker dot offset below the bubble.)
        float tail = smoothstep(0.04, 0.0,
                       length(bD - vec2(-bSz * 0.6, -bSz * 0.95)));
        col = mix(col, LIK_INK, tail * 0.6);
    }

    // Surprise: every ~14s a sudden onomatopoeia bubble flashes —
    // a yellow starburst with a bold black outline at a corner. POW!
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 14.0);
        float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.20, 0.10, _ph);
        vec2 _o = vec2(0.78, 0.78);
        vec2 _d = (_suv - _o);
        float _ang = atan(_d.y, _d.x);
        float _r = length(_d);
        float _spike = 0.10 + 0.04 * cos(_ang * 12.0);
        float _star = smoothstep(_spike, _spike * 0.92, _r);
        float _outline = smoothstep(_spike + 0.012, _spike + 0.005, _r) * (1.0 - _star);
        col += vec3(1.0, 0.85, 0.15) * _f * _star * 1.5;
        col = mix(col, vec3(0.05),             _f * _outline);
    }

    gl_FragColor = vec4(col, 1.0);
}
