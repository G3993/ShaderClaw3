/*{
  "DESCRIPTION": "Constellation Walker — cellular random walkers tracing star constellations on a deep indigo night sky. Each walker is a glowing star node; trails become constellation connection lines. Stellar palette: warm gold stars, cyan secondary stars, HDR white-hot cores 2.5+. Completely different from prior saturation-fix and bioluminescent-reef angles.",
  "CREDIT": "ShaderClaw — constellation walker redesign",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "gridSize",   "LABEL": "Grid Size",    "TYPE": "float", "DEFAULT": 80.0,  "MIN": 20.0, "MAX": 300.0 },
    { "NAME": "walkers",    "LABEL": "Walkers",      "TYPE": "float", "DEFAULT": 8.0,   "MIN": 1.0,  "MAX": 16.0 },
    { "NAME": "stepRate",   "LABEL": "Step Rate",    "TYPE": "float", "DEFAULT": 25.0,  "MIN": 1.0,  "MAX": 120.0 },
    { "NAME": "hueDrift",   "LABEL": "Hue Drift",    "TYPE": "float", "DEFAULT": 0.008, "MIN": 0.0,  "MAX": 0.1 },
    { "NAME": "fadeRate",   "LABEL": "Trail Fade",   "TYPE": "float", "DEFAULT": 0.002, "MIN": 0.0,  "MAX": 0.08 },
    { "NAME": "starSize",   "LABEL": "Star Size",    "TYPE": "float", "DEFAULT": 2.5,   "MIN": 0.5,  "MAX": 6.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,   "MIN": 0.5,  "MAX": 5.0 },
    { "NAME": "bloom",      "LABEL": "Bloom",        "TYPE": "float", "DEFAULT": 0.8,   "MIN": 0.0,  "MAX": 1.5 },
    { "NAME": "pulse",      "LABEL": "Audio Pulse",  "TYPE": "float", "DEFAULT": 0.5,   "MIN": 0.0,  "MAX": 1.5 },
    { "NAME": "bounceEdges","LABEL": "Bounce Edges", "TYPE": "bool",  "DEFAULT": true },
    { "NAME": "backgroundColor", "LABEL": "BG Color","TYPE": "color", "DEFAULT": [0.01, 0.005, 0.06, 1.0] }
  ],
  "PASSES": [
    { "TARGET": "stateBuf", "PERSISTENT": true, "WIDTH": 16, "HEIGHT": 1 },
    { "TARGET": "canvas", "PERSISTENT": true },
    {}
  ]
}*/

#define MAX_WALKERS 16
#define TAU 6.28318530718

float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec2 neighborDir(int dir) {
    if (dir == 0) return vec2(-1.0, -1.0);
    if (dir == 1) return vec2( 0.0, -1.0);
    if (dir == 2) return vec2( 1.0, -1.0);
    if (dir == 3) return vec2(-1.0,  0.0);
    if (dir == 4) return vec2( 1.0,  0.0);
    if (dir == 5) return vec2(-1.0,  1.0);
    if (dir == 6) return vec2( 0.0,  1.0);
    return vec2( 1.0,  1.0);
}

vec4 readWalker(float id) {
    return texture2D(stateBuf, vec2((id + 0.5) / 16.0, 0.5));
}

void main() {
    vec2 Res = RENDERSIZE;
    vec2 pos = gl_FragCoord.xy;
    float cell = 1.0 / max(gridSize, 1.0);
    float audio = 1.0 + audioLevel * clamp(pulse, 0.0, 1.5);

    // =============================================================
    // PASS 0: advance walker state buffer (16x1)
    // state encoding: vec4(x_norm, y_norm, hue, stepAccumulator)
    // =============================================================
    if (PASSINDEX == 0) {
        float id = floor(pos.x);
        if (id >= walkers) {
            gl_FragColor = vec4(0.0);
            return;
        }

        // Seed the walker near center on first frames
        if (FRAMEINDEX < 2) {
            float jx = (hash11(id * 7.31 + 1.0) - 0.5) * 0.15;
            float jy = (hash11(id * 3.19 + 2.0) - 0.5) * 0.15;
            float h0 = hash11(id * 11.7 + 3.0);
            gl_FragColor = vec4(0.5 + jx, 0.5 + jy, h0, 0.0);
            return;
        }

        vec4 prev = readWalker(id);
        vec2 p = prev.rg;
        float h = prev.b;
        float acc = prev.a + TIMEDELTA * stepRate * audio;

        // Walk up to 6 discrete cell steps this frame
        for (int s = 0; s < 6; s++) {
            if (acc < 1.0) break;
            acc -= 1.0;

            float seed = TIME * 97.13 + id * 13.7 + float(s) * 3.31;
            float r = hash12(vec2(seed, seed * 0.47));
            int dir = int(floor(r * 8.0));
            vec2 stepVec = neighborDir(dir) * cell;
            p += stepVec;

            if (bounceEdges) {
                if (p.x < 0.0) p.x = -p.x;
                if (p.x > 1.0) p.x = 2.0 - p.x;
                if (p.y < 0.0) p.y = -p.y;
                if (p.y > 1.0) p.y = 2.0 - p.y;
            } else {
                p = fract(p);
            }

            float dh = (hash12(vec2(seed + 7.7, id)) - 0.5) * 2.0 * hueDrift;
            h = fract(h + dh + 1.0);
        }

        gl_FragColor = vec4(p, h, acc);
        return;
    }

    // =============================================================
    // PASS 1: update persistent canvas (fade + paint constellation stars)
    // =============================================================
    if (PASSINDEX == 1) {
        vec2 uv = pos / Res;
        vec4 prev = texture2D(canvas, uv);
        vec4 col = prev * (1.0 - fadeRate);

        // Aspect-correct pixel coords
        float aspect = Res.x / Res.y;
        vec2 screenPos = vec2(uv.x * aspect, uv.y);

        // Constellation star palette: warm gold, cyan, white alternating
        // Each walker gets a "star color" based on its hue
        for (int i = 0; i < MAX_WALKERS; i++) {
            if (float(i) >= walkers) break;
            vec4 st = readWalker(float(i));
            vec2 wPos = vec2(st.r * aspect, st.g);

            // Distance from this pixel to walker position (in screen coords)
            float cellSize = 1.0 / gridSize;
            float pixDist = length(screenPos - wPos) / (cellSize * starSize);

            if (pixDist < 3.0) {
                // Star color: gold/cyan/white based on hue bucket
                float h = st.b;
                vec3 starCol;
                if (h < 0.33)      starCol = vec3(1.0, 0.75, 0.0);  // gold
                else if (h < 0.66) starCol = vec3(0.0, 1.0, 0.90);  // cyan
                else               starCol = vec3(0.90, 0.80, 1.0); // lavender-white

                // HDR gaussian star glow — white-hot core fading to color
                float glow = exp(-pixDist * pixDist * 0.8);
                float core = exp(-pixDist * pixDist * 6.0);
                vec3 emission = starCol * glow * hdrPeak + vec3(2.5, 2.4, 2.2) * core * 0.4;
                col = vec4(max(col.rgb, emission * audio), 1.0);
            }

            // Constellation trail: paint a faint line between consecutive walkers
            // (draw line segment from walker i to walker (i+1))
            if (i + 1 < MAX_WALKERS && float(i + 1) < walkers) {
                vec4 st2 = readWalker(float(i + 1));
                vec2 wPos2 = vec2(st2.r * aspect, st2.g);

                // Distance from pixel to line segment wPos-wPos2
                vec2 ab = wPos2 - wPos;
                vec2 ap = screenPos - wPos;
                float abLen2 = dot(ab, ab);
                float t2 = 0.0;
                if (abLen2 > 0.0001) t2 = clamp(dot(ap, ab) / abLen2, 0.0, 1.0);
                float lineDist = length(ap - t2 * ab) / (cellSize * 0.6);

                if (lineDist < 1.0) {
                    // Only draw line if walkers are within ~8 cells of each other
                    float walkerDist = length(wPos2 - wPos) / cellSize;
                    if (walkerDist < 8.0) {
                        float lineGlow = exp(-lineDist * lineDist * 2.0) * 0.3;
                        vec3 lineCol = mix(
                            (st.b < 0.5 ? vec3(1.0, 0.75, 0.0) : vec3(0.0, 1.0, 0.90)),
                            (st2.b < 0.5 ? vec3(1.0, 0.75, 0.0) : vec3(0.0, 1.0, 0.90)),
                            t2
                        );
                        col.rgb += lineCol * lineGlow * hdrPeak * 0.25;
                    }
                }
            }
        }

        gl_FragColor = col;
        return;
    }

    // =============================================================
    // PASS 2: final display (bloom + deep indigo starfield background)
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

    // Background starfield: faint distant stars in deep indigo
    vec3 bg = backgroundColor.rgb;
    // Add faint background star dots
    float bStarHash = hash12(floor(uv * vec2(120.0, 80.0)));
    float bStarOn   = step(0.97, bStarHash); // 3% of bg cells have a faint star
    float bStarLum  = hash12(floor(uv * vec2(120.0, 80.0)) + vec2(7.1, 3.3)) * 0.3 + 0.05;
    bg += vec3(bStarLum * 0.7, bStarLum * 0.85, bStarLum) * bStarOn;

    float lum = max(c.r, max(c.g, c.b));
    float alpha = clamp(lum * 6.0, 0.0, 1.0);
    vec3 outRgb = mix(bg, c, alpha);
    gl_FragColor = vec4(outRgb, 1.0);
}
