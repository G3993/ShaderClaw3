/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "German Expressionism after Kirchner Street Berlin (1913), Nolde Last Supper (1909), Heckel Crouching Woman (1913) — high-contrast posterize over carved-wood ridged noise, time-driven horizontal shear that leans the perspective like a Berlin streetcar tilt, persistent paint accumulation for stacked woodcut grooves, acid LUT and black ridge contours from luma edge-detect. No Voronoi, no cells, real movement.",
  "INPUTS": [
    { "NAME": "shearAmount",  "LABEL": "Perspective Shear",  "TYPE": "float", "MIN": 0.0,  "MAX": 0.40, "DEFAULT": 0.18 },
    { "NAME": "shearSpeed",   "LABEL": "Shear Speed",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.5 },
    { "NAME": "carveScale",   "LABEL": "Wood-Carve Scale",   "TYPE": "float", "MIN": 2.0,  "MAX": 16.0, "DEFAULT": 6.5 },
    { "NAME": "ridgeAmp",     "LABEL": "Ridge Amplitude",    "TYPE": "float", "MIN": 0.0,  "MAX": 0.20, "DEFAULT": 0.07 },
    { "NAME": "flow",         "LABEL": "Carve Flow",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.30 },
    { "NAME": "acidTint",     "LABEL": "Acid Tint",          "TYPE": "float", "MIN": 0.0,  "MAX": 0.80, "DEFAULT": 0.42 },
    { "NAME": "contrast",     "LABEL": "Contrast",           "TYPE": "float", "MIN": 1.0,  "MAX": 2.4,  "DEFAULT": 1.55 },
    { "NAME": "posterize",    "LABEL": "Posterize Steps",    "TYPE": "float", "MIN": 2.0,  "MAX": 8.0,  "DEFAULT": 4.0 },
    { "NAME": "inkLines",     "LABEL": "Ink Edge Strength",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "paintFade",    "LABEL": "Paint Persistence",  "TYPE": "float", "MIN": 0.92, "MAX": 1.0,  "DEFAULT": 0.985 },
    { "NAME": "audioReact",   "LABEL": "Audio React",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "resetField",   "LABEL": "Reset",              "TYPE": "bool",  "DEFAULT": false },
    { "NAME": "inputTex",     "LABEL": "Texture",            "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "carveBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Kirchner's woodcut grammar is RIDGED noise (carved-wood ridges), not
// Worley cells. We drop the cellular technique entirely and use:
//   ridge(p) = 1 - |2*vnoise(p) - 1|     — sharp ridge contours
//   ridgedFbm                              — multi-octave woodcut grain
//   horizontal shear keyed to TIME         — Berlin street perspective tilt
//   persistent buffer                      — strokes accumulate like
//                                            successive carving passes
//   acid LUT + ink-edge contours           — Die Brücke palette signifier

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
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

// Ridged noise — sharp triangular peaks where vnoise crosses 0.5. This
// is the carved-wood signifier: each "ridge" reads as a chisel groove.
float ridge(vec2 p) {
    float n = vnoise(p);
    return 1.0 - abs(2.0 * n - 1.0);
}

float ridgedFbm(vec2 p) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 5; i++) {
        s += a * ridge(p);
        p = mat2(1.6, 1.2, -1.2, 1.6) * p;
        a *= 0.5;
    }
    return s;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Berlin-street perspective shear — the canvas tilts left/right
    // continuously as if you're standing on a moving streetcar. This is
    // the Kirchner *Street, Berlin* signifier: figures lean diagonally.
    float shearT = sin(TIME * shearSpeed) * shearAmount
                 * (1.0 + audioMid * audioReact * 0.6);
    vec2 sUV = uv;
    sUV.x += (sUV.y - 0.5) * shearT;

    // ============= PASS 0 — carveBuf accumulation =============
    if (PASSINDEX == 0) {

        if (FRAMEINDEX < 2 || resetField) {
            gl_FragColor = vec4(0.5, 0.4, 0.4, 1.0);
            return;
        }

        // Sample previous frame at unsheared uv displaced by ridge gradient
        // Skip the 4-tap gradient warp — paintFade alone gives the
        // stacked-passes look, and the warp was barely visible. Saves
        // ~80% of pass-0 noise calls (4× ridgedFbm = 20 vnoise/frag gone).
        vec2 advUV = uv;
        vec3 prev = texture(carveBuf, advUV).rgb;

        // Slow fade so the carving accumulates but doesn't saturate.
        prev *= paintFade;

        // Source content (texture or fallback Berlin streetscape).
        vec3 fresh;
        if (IMG_SIZE_inputTex.x > 0.0) {
            fresh = texture(inputTex, sUV).rgb;
        } else {
            // Procedural Berlin street: vertical lamp streaks + horizon
            // gradient + flickering windows.
            float streetHorizon = smoothstep(0.4, 0.55, sUV.y);
            vec3 sky    = mix(vec3(0.18, 0.10, 0.20),
                              vec3(0.32, 0.18, 0.28), sUV.y);
            vec3 street = mix(vec3(0.12, 0.10, 0.14),
                              vec3(0.36, 0.30, 0.28), sUV.y);
            fresh = mix(street, sky, streetHorizon);
            // Lamp streaks — vertical bright vertical bars at hashed x
            float lampX = floor(sUV.x * 4.0);
            float lampPhase = hash21(vec2(lampX, 0.0));
            float lampStreak = smoothstep(0.02, 0.0,
                abs(fract(sUV.x * 4.0) - 0.5))
                * smoothstep(0.4, 0.6, sUV.y);
            fresh += vec3(0.95, 0.78, 0.42)
                  * lampStreak * (0.5 + 0.5 * sin(TIME * 1.3 + lampPhase * 6.28));
            // Lit windows — small bright squares at hashed positions
            vec2 wuv = floor(sUV * vec2(20.0, 14.0));
            float wHash = hash21(wuv);
            if (wHash > 0.85 && sUV.y > 0.45) {
                float pulse = step(hash21(wuv + floor(TIME * 0.7)), 0.6);
                fresh = mix(fresh, vec3(0.95, 0.85, 0.45), pulse * 0.65);
            }
        }

        // Apply carve overlay — the ridged noise modulates source brightness
        // so the ridges read as gouged grooves.
        float carve = ridgedFbm(sUV * carveScale + TIME * flow * 0.6);
        fresh *= 0.65 + carve * 0.7;

        // Composite new on top with low-alpha so the buffer accumulates.
        vec3 outC = mix(prev, fresh, 0.22 + audioBass * audioReact * 0.18);

        gl_FragColor = vec4(outC, 1.0);
        return;
    }

    // ============= PASS 1 — output ============================================

    vec3 col = texture(carveBuf, uv).rgb;

    // Acid LUT — magenta + sulphur yellow + cobalt unevenly mixed.
    // Strength capped per the Kirchner spec (>0.6 looks like a 90s filter).
    vec3 acid = mix(vec3(0.7, 1.4, 0.4), vec3(1.4, 0.5, 1.3), 0.5);
    col = mix(col, col * acid,
              clamp(acidTint, 0.0, 0.7)
              * (0.5 + audioHigh * audioReact * 0.5));

    // Hard contrast curve — flatten mid-tones into stark light/dark.
    col = (col - 0.5) * contrast + 0.5;

    // Posterize — discrete colour steps for the woodblock-print feel.
    float steps = max(2.0, posterize);
    col = floor(col * steps) / (steps - 1.0);

    // Black ink edges from luma gradient.
    float L = dot(col, vec3(0.299, 0.587, 0.114));
    float edge = length(vec2(dFdx(L), dFdy(L)));
    float ink = smoothstep(0.05, 0.18, edge * (0.4 + audioMid * audioReact * 0.8));
    col = mix(col, vec3(0.05, 0.04, 0.06), ink * inkLines);

    // Audio-driven luminance breath
    col *= 0.92 + audioLevel * audioReact * 0.12;

    gl_FragColor = vec4(col, 1.0);
}
