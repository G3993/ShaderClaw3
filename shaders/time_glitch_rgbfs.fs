/*{
  "DESCRIPTION": "Chromatic Void — 2D kaleidoscopic RGB prism fractal with glitch decay. Standalone generator, no input required.",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "zoom",       "LABEL": "Zoom",       "TYPE": "float", "MIN": 0.5, "MAX": 4.0,  "DEFAULT": 1.8 },
    { "NAME": "rotSpeed",   "LABEL": "Rotation",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.25 },
    { "NAME": "folds",      "LABEL": "Folds",      "TYPE": "float", "MIN": 2.0, "MAX": 8.0,  "DEFAULT": 6.0 },
    { "NAME": "glitchAmt",  "LABEL": "Glitch",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.4 },
    { "NAME": "hdrBoost",   "LABEL": "HDR Boost",  "TYPE": "float", "MIN": 1.0, "MAX": 4.0,  "DEFAULT": 2.5 },
    { "NAME": "audioMod",   "LABEL": "Audio React","TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

// Chromatic Void — 2D kaleidoscopic RGB prism (completely different from 3D signal planes)

float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

// Fold space into kaleidoscope wedge
vec2 kFold(vec2 p, float n) {
    float angle = 3.14159265 / n;
    float a = atan(p.y, p.x);
    a = mod(a, 2.0 * angle) - angle;
    return length(p) * vec2(cos(a), sin(a));
}

// SDF for a glitchy rectangular bar at y=0
float sdBar(vec2 p, float w, float h) {
    vec2 d = abs(p) - vec2(w, h);
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

// One channel of the chromatic fractal
float fractalChan(vec2 uv, float t, float chanOffset) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv = vec2(uv.x * aspect, uv.y) * 2.0 - vec2(aspect, 1.0);
    uv *= zoom;

    // Kaleidoscope fold
    float rot = t * rotSpeed + chanOffset * 0.4;
    float cr = cos(rot), sr = sin(rot);
    uv = vec2(cr * uv.x - sr * uv.y, sr * uv.x + cr * uv.y);
    uv = kFold(uv, folds);

    // Iterative inversion + fold (creates fractal self-similarity)
    float d = 1e9;
    float scale = 1.0;
    for (int i = 0; i < 5; i++) {
        uv = abs(uv) - vec2(0.5 + float(i) * 0.05, 0.3);
        float len = length(uv);
        if (len < 0.001) break;
        uv /= len * len;
        scale *= len * len;

        // Glitch: per-iteration horizontal displacement
        float glitchY = floor(uv.y * (4.0 + float(i)));
        float gShift = hash11(glitchY + t * (0.7 + float(i) * 0.3) + float(i) * 7.1);
        uv.x += (gShift - 0.5) * glitchAmt * 0.3;

        float bd = sdBar(uv - vec2(0.0, 0.1 * sin(t * 1.3 + float(i) * 2.1)),
                         0.15 + float(i) * 0.02, 0.04);
        d = min(d, bd / scale);
    }

    return clamp(1.0 - d * 12.0, 0.0, 1.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float t = TIME;
    float audio = 1.0 + audioLevel * audioMod * 0.5 + audioBass * audioMod * 0.3;

    // Per-channel time offsets for chromatic split (the glitch effect)
    float rOff = 0.0;
    float gOff = 0.07 * glitchAmt;
    float bOff = -0.13 * glitchAmt;

    // Per-channel UV offsets (horizontal glitch)
    float gShiftX = hash21(vec2(floor(uv.y * 12.0), floor(t * 6.0))) - 0.5;
    vec2 uvR = uv + vec2(gShiftX * glitchAmt * 0.04, 0.0);
    vec2 uvG = uv;
    vec2 uvB = uv - vec2(gShiftX * glitchAmt * 0.04, 0.0);

    float r = fractalChan(uvR, t + rOff, 0.0);
    float g = fractalChan(uvG, t + gOff, 0.33);
    float b = fractalChan(uvB, t + bOff, 0.67);

    // Palette: signal red, data green, electric blue + white-hot peaks
    vec3 col = vec3(r, g, b) * hdrBoost * audio;

    // Hot-white specular peaks where all channels align
    float hotWhite = min(r, min(g, b));
    col += hotWhite * hotWhite * 2.0 * hdrBoost;

    // Block-dropout glitch noise
    float blockX = floor(uv.x * 8.0);
    float blockY = floor(uv.y * 5.0);
    float blockNoise = hash21(vec2(blockX * 7.3 + floor(t * 8.0), blockY * 3.1));
    float dropout = step(1.0 - glitchAmt * 0.2, blockNoise);
    col *= 1.0 - dropout * 0.7;

    gl_FragColor = vec4(col, 1.0);
}
