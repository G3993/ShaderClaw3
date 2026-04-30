/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Art Nouveau after Mucha (Gismonda 1894), Klimt (gold-mosaic period 1907), and Beardsley (Peacock Skirt 1893) — persistent paint buffer continuously inscribed by N parametric whiplash strokes flowing across the canvas, depositing gold and ink along sinuous S-curves. Frame-feedback gives the strokes lasting trails so the painting LIVES; nothing static, nothing stacked. Pure flowing line, in motion.",
  "INPUTS": [
    { "NAME": "artistStyle",     "LABEL": "Style",             "TYPE": "long",  "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Whiplash Curves", "Mosaic Gold Ground", "Pure Linework"] },
    { "NAME": "tendrilCount",    "LABEL": "Tendril Count",     "TYPE": "float", "MIN": 2.0,  "MAX": 24.0, "DEFAULT": 10.0 },
    { "NAME": "tendrilSpeed",    "LABEL": "Tendril Speed",     "TYPE": "float", "MIN": 0.02, "MAX": 1.0,  "DEFAULT": 0.20 },
    { "NAME": "strokeWidth",     "LABEL": "Stroke Width",      "TYPE": "float", "MIN": 0.001,"MAX": 0.015,"DEFAULT": 0.0035 },
    { "NAME": "whiplashAmp",     "LABEL": "Whiplash Amp",      "TYPE": "float", "MIN": 0.05, "MAX": 0.50, "DEFAULT": 0.20 },
    { "NAME": "harmonicMix",     "LABEL": "Harmonic Mix",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.50 },
    { "NAME": "paintFade",       "LABEL": "Paint Persistence", "TYPE": "float", "MIN": 0.92, "MAX": 1.0,  "DEFAULT": 0.985 },
    { "NAME": "goldRatio",       "LABEL": "Gold Stroke Ratio", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.45 },
    { "NAME": "fieldWarmth",     "LABEL": "Field Warmth",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6 },
    { "NAME": "petalBloom",      "LABEL": "Petal Bloom",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.45 },
    { "NAME": "audioReact",      "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "resetField",      "LABEL": "Reset",             "TYPE": "bool",  "DEFAULT": false },
    { "NAME": "inputTex",        "LABEL": "Texture",           "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "paintBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Mucha's whiplash line is the SUBJECT, not a frame ornament around one.
// We simulate the strokes as moving pen-tips: each tendril is a
// parametric S-curve whose head walks across the canvas over time. At
// each fragment, we compute distance-to-nearest-tendril-head; if close,
// we deposit colour. The persistent paintBuf makes those deposits last
// — the strokes accumulate as continuous traces, decaying slowly so the
// painting evolves rather than freezes.
//
// Three artist styles share the technique but differ in palette:
//   0 — Mucha:    pastel cream/rose field, gold + ink strokes, soft halo bloom
//   1 — Klimt:    deep gold field with mosaic micro-pattern, ink strokes
//   2 — Beardsley: pure black-on-white, no gold, dense lacework

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

vec3 fieldColor(int style, vec2 uv, float warm, float audioLvl) {
    float aspectF = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    if (style == 0) {
        // Mucha pastel field with Byzantine halo arc behind the upper
        // half — the iconic Mucha signature device (Gismonda, Job,
        // Zodiac all share it). Without it the shader reads as
        // generic art-nouveau wallpaper.
        vec3 cream = vec3(0.97, 0.93, 0.84);
        vec3 rose  = vec3(0.94, 0.80, 0.78);
        vec3 sage  = vec3(0.78, 0.86, 0.74);
        vec3 mid   = mix(sage, rose, warm);
        vec3 base  = mix(cream, mid, smoothstep(0.0, 1.0, uv.y));
        // Aspect-correct halo so it renders as a circle in screen
        // space, not a vertical ellipse on widescreen displays.
        vec2 halo  = (uv - vec2(0.5, 0.62)) * vec2(aspectF, 1.0);
        float r    = length(halo);
        // Gilded ring at radius ~0.30
        float arc  = smoothstep(0.36, 0.30, r) - smoothstep(0.30, 0.24, r);
        base = mix(base, vec3(0.92, 0.78, 0.36), arc * 0.55);
        // Soft mid-tone disc behind the figure
        base = mix(base, mid * 0.92, smoothstep(0.36, 0.0, r) * 0.18);
        return base;
    } else if (style == 1) {
        // Klimt mosaic gold ground — deep gold with subtle pattern
        vec3 gold     = mix(vec3(0.62, 0.45, 0.18), vec3(0.92, 0.78, 0.36), warm);
        // Mosaic tiles — drift slowly so the gold ground micro-shimmers
        // instead of reading as a frozen Voronoi-like tile field.
        vec2 g = floor((uv + 0.005 * vec2(sin(TIME * 0.05),
                                          cos(TIME * 0.04))) * 80.0);
        float h = hash21(g + floor(TIME * 0.3)) * 0.18;
        return gold * (0.85 + h);
    } else {
        // Beardsley: pure white field
        return vec3(0.98, 0.97, 0.94);
    }
}

vec3 strokeColor(int style, float seed, float gold) {
    vec3 GOLD = vec3(0.94, 0.82, 0.36);
    vec3 INK  = vec3(0.10, 0.07, 0.05);
    if (style == 2) {
        // Beardsley — only ink
        return INK;
    }
    if (style == 1) {
        // Klimt — ink lines on gold ground
        return INK;
    }
    return (seed < gold) ? GOLD : INK;
}

// Parametric whiplash position: an S-curve crossing the canvas over time
// `t`, with tendril `id` having its own start/end and amplitude.
vec2 tendrilPos(float t, int id, float amp, float harmonics) {
    float fid = float(id);
    // Travel direction — diagonal vector chosen per tendril
    float startAng = hash11(fid * 1.31) * 6.2832;
    vec2 startPt = vec2(0.5) + 0.6 * vec2(cos(startAng), sin(startAng));
    float endAng = startAng + 3.14159 + (hash11(fid * 2.97) - 0.5) * 1.5;
    vec2 endPt = vec2(0.5) + 0.6 * vec2(cos(endAng), sin(endAng));

    // Ping-pong along the path so the head reverses smoothly instead of
    // teleporting back to start. smoothstep gives an S-curve acceleration
    // — actual Mucha whiplash, not a linear gradient ride.
    float tt = abs(fract(t * 0.5 + hash11(fid * 5.7)) * 2.0 - 1.0);
    vec2 base = mix(startPt, endPt, smoothstep(0.0, 1.0, tt));

    vec2 dir = normalize(endPt - startPt + 1e-5);
    vec2 perp = vec2(-dir.y, dir.x);
    // Low-frequency arc bend — the whole stroke arcs across its length.
    base += perp * sin(tt * 3.14159) * amp * 0.5;

    float w1 = sin(tt * 6.2832 + fid * 1.7) * amp;
    float w2 = sin(tt * 12.566 + fid * 2.3 + 1.3)
            * amp * 0.4 * harmonics;
    return base + perp * (w1 + w2);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    int style = int(artistStyle);

    // ============= PASS 0 — paintBuf accumulation =============
    if (PASSINDEX == 0) {

        if (FRAMEINDEX < 2 || resetField) {
            gl_FragColor = vec4(fieldColor(style, uv,
                                            fieldWarmth, audioLevel), 1.0);
            return;
        }

        vec3 prev = texture(paintBuf, uv).rgb;
        // Slow decay back to field — strokes dissolve over many seconds.
        vec3 field = fieldColor(style, uv, fieldWarmth, audioLevel);
        prev = mix(field, prev, paintFade);

        // Walk each tendril head at this frame's TIME and check if our
        // fragment is on the head's stroke.
        int N = int(clamp(tendrilCount, 1.0, 24.0));
        float t  = TIME * tendrilSpeed
                 * (1.0 + audioMid * audioReact * 0.6);
        vec3 col = prev;
        for (int i = 0; i < 24; i++) {
            if (i >= N) break;
            float fi = float(i);
            vec2 head = tendrilPos(t + hash11(fi * 9.7) * 6.0, i,
                                   whiplashAmp, harmonicMix);
            // Distance to head + a short tail shadow sampled along path
            // (head only — the tail is generated by the persistent buffer
            // accumulating successive heads frame by frame).
            vec2 d = uv - head; d.x *= aspect;
            float dh = length(d);
            float w = strokeWidth * (1.0 + audioBass * audioReact * 0.4)
                    * (0.7 + hash11(fi * 11.7) * 0.6);
            if (dh < w * 4.0) {
                float falloff = smoothstep(w, 0.0, dh);
                if (falloff > 0.001) {
                    vec3 sc = strokeColor(style,
                                          hash11(fi * 13.3),
                                          goldRatio);
                    col = mix(col, sc, falloff);
                }
            }
        }
        gl_FragColor = vec4(col, 1.0);
        return;
    }

    // ============= PASS 1 — output ============================================

    vec3 col = texture(paintBuf, uv).rgb;

    // Petal bloom — Mucha's tendrils sometimes terminate in lily-bud or
    // peacock-eye flourishes. We add a faint radial highlight at any
    // pixel that is bright (gold-laden) — gives the metallic edge bloom
    // characteristic of Klimt's gold-leaf and Mucha's gilt accents.
    if (style != 2 && petalBloom > 0.0) {
        float L = dot(col, vec3(0.299, 0.587, 0.114));
        float goldness = clamp(L - 0.55, 0.0, 1.0);
        col += vec3(0.95, 0.78, 0.30) * goldness * petalBloom * 0.45;
    }

    // Audio breath
    col *= 0.92 + audioLevel * audioReact * 0.10;

    gl_FragColor = vec4(col, 1.0);
}
