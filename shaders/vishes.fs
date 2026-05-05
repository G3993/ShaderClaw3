/*{
  "DESCRIPTION": "Lissajous Ghosts — smooth Lissajous-curve walkers painting luminous trails on a persistent canvas",
  "CREDIT": "ShaderClaw — Lissajous rework of vishes cell-walker",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "walkers",    "LABEL": "Walkers",    "TYPE": "float", "DEFAULT": 5.0,   "MIN": 1.0,  "MAX": 10.0 },
    { "NAME": "baseFreq",   "LABEL": "Frequency",  "TYPE": "float", "DEFAULT": 0.28,  "MIN": 0.05, "MAX": 1.0 },
    { "NAME": "paintRad",   "LABEL": "Brush Size", "TYPE": "float", "DEFAULT": 0.018, "MIN": 0.003,"MAX": 0.08 },
    { "NAME": "fadeRate",   "LABEL": "Trail Fade", "TYPE": "float", "DEFAULT": 0.008, "MIN": 0.0,  "MAX": 0.05 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",   "TYPE": "float", "DEFAULT": 2.5,   "MIN": 0.5,  "MAX": 4.0 },
    { "NAME": "bloom",      "LABEL": "Bloom",      "TYPE": "float", "DEFAULT": 0.7,   "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "hueDrift",   "LABEL": "Hue Drift",  "TYPE": "float", "DEFAULT": 0.03,  "MIN": 0.0,  "MAX": 0.2 },
    { "NAME": "pulse",      "LABEL": "Audio Pulse","TYPE": "float", "DEFAULT": 0.7,   "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "bgColor",    "LABEL": "BG Color",   "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.01, 1.0] }
  ],
  "PASSES": [
    { "TARGET": "canvas", "PERSISTENT": true },
    {}
  ]
}*/

#define MAX_WALKERS 10
#define TAU 6.28318530718

float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Chosen 5-color palette (fully saturated)
// magenta, cyan, gold, lime, violet
vec3 walkerColor(int i) {
    if (i == 0) return vec3(1.0, 0.0, 1.0);    // magenta
    if (i == 1) return vec3(0.0, 1.0, 1.0);    // cyan
    if (i == 2) return vec3(1.0, 0.85, 0.0);   // gold
    if (i == 3) return vec3(0.2, 1.0, 0.0);    // lime
    return           vec3(0.6, 0.0, 1.0);       // violet
}

// Lissajous walker position at time t
// a:b ratios: 1:2, 2:3, 3:4, 4:5, 5:6 — progressively complex patterns
vec2 lissajousPos(int i, float t) {
    float fi = float(i);
    float a  = float(i + 1);
    float b  = float(i + 2);
    float phx = hash11(fi * 19.1) * TAU;
    float phy = hash11(fi * 31.7) * TAU;
    float freq = baseFreq * (0.7 + hash11(fi * 5.3) * 0.6);
    float rx = 0.38 + hash11(fi * 7.1) * 0.08;  // radius variation
    float ry = 0.38 + hash11(fi * 3.9) * 0.08;
    // Center offset — each walker explores a slightly different quadrant
    float cx = 0.5 + (hash11(fi * 41.3) - 0.5) * 0.12;
    float cy = 0.5 + (hash11(fi * 53.7) - 0.5) * 0.12;
    return vec2(cx + rx * sin(a * freq * t + phx),
                cy + ry * sin(b * freq * t + phy));
}

void main() {
    vec2 Res = RENDERSIZE;
    vec2 pos = gl_FragCoord.xy;
    float aspect = Res.x / Res.y;

    int N = int(clamp(walkers, 1.0, float(MAX_WALKERS)));

    // =============================================================
    // PASS 0: update persistent canvas — fade + paint Lissajous curves
    // =============================================================
    if (PASSINDEX == 0) {
        vec2 uv = pos / Res;
        vec4 prev = texture2D(canvas, uv);
        // Fade towards black
        vec4 col = prev * (1.0 - fadeRate);

        float audioBoost = 1.0 + audioLevel * pulse + audioBass * pulse * 0.5;

        // Paint each walker: smooth soft circle at current + recent positions
        // Sample 4 time sub-steps to fill gaps in fast-moving curves
        for (int i = 0; i < MAX_WALKERS; i++) {
            if (i >= N) break;

            vec3 baseCol = walkerColor(i % 5);
            // Hue drift over time
            float hue0 = fract(float(i) / 5.0 + TIME * hueDrift);
            vec3 walkerCol = hsv2rgb(vec3(hue0, 1.0, 1.0)) * hdrPeak * audioBoost;

            // Sub-step integration to avoid gaps in high-freq curves
            for (int s = 0; s < 4; s++) {
                float dt = TIMEDELTA * float(s) / 4.0;
                vec2 wPos = lissajousPos(i, TIME - dt);

                // Aspect-correct distance
                vec2 delta = vec2((uv.x - wPos.x) * aspect, uv.y - wPos.y);
                float d = length(delta);

                float softR = paintRad * (1.0 + audioBass * pulse * 0.4);
                float paint = smoothstep(softR, softR * 0.2, d);

                if (paint > 0.001) {
                    col.rgb = max(col.rgb, walkerCol * paint);
                    col.a = 1.0;
                }
            }
        }

        // Black ink at low-luminance edges (contrast enhancement)
        float lum = dot(col.rgb, vec3(0.299, 0.587, 0.114));
        col.rgb *= smoothstep(0.02, 0.08, lum);

        gl_FragColor = col;
        return;
    }

    // =============================================================
    // PASS 1: final display — bloom + background blend
    // =============================================================
    vec2 uv = pos / Res;
    vec3 c = texture2D(canvas, uv).rgb;

    if (bloom > 0.001) {
        vec3 sum = vec3(0.0);
        float r = 3.0 / min(Res.x, Res.y);
        for (int x = -2; x <= 2; x++) {
            for (int y = -2; y <= 2; y++) {
                vec2 off = vec2(float(x), float(y)) * r;
                sum += texture2D(canvas, uv + off).rgb;
            }
        }
        sum /= 25.0;
        c += sum * bloom;
    }

    float lum = max(c.r, max(c.g, c.b));
    float alpha = clamp(lum * 5.0, 0.0, 1.0);
    vec3 outRgb = mix(bgColor.rgb, c, alpha);
    gl_FragColor = vec4(outRgb, 1.0);
}
