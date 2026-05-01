/*{
  "CATEGORIES": ["Effect", "Generator", "Audio Reactive"],
  "DESCRIPTION": "The image surrenders to gravity — multiple Persistence-of-Memory clocks slung over a melted branch in a vermilion-and-ochre desert; pixel-sorting drag pulls bright values downward; ants crawl; the soft skull/face rock looms in the corner. Loud audio widens drips. Without an inputTex, the procedural Dalí scene plays.",
  "INPUTS": [
    {"NAME":"sagAmp",      "LABEL":"Sag Amount",      "TYPE":"float","MIN":0.0, "MAX":0.5,  "DEFAULT":0.16},
    {"NAME":"sagFreq",     "LABEL":"Sag Frequency",   "TYPE":"float","MIN":0.5, "MAX":8.0,  "DEFAULT":2.5},
    {"NAME":"flow",        "LABEL":"Flow Speed",      "TYPE":"float","MIN":0.0, "MAX":2.0,  "DEFAULT":0.30},
    {"NAME":"chroma",      "LABEL":"Chroma Smear",    "TYPE":"float","MIN":0.0, "MAX":0.05, "DEFAULT":0.014},
    {"NAME":"smear",       "LABEL":"Lateral Smear",   "TYPE":"float","MIN":0.0, "MAX":0.05, "DEFAULT":0.012},
    {"NAME":"pixelSort",   "LABEL":"Pixel Sort",      "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.55},
    {"NAME":"sortThresh",  "LABEL":"Sort Threshold",  "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.40},
    {"NAME":"clockCount",  "LABEL":"Clock Count",     "TYPE":"float","MIN":1.0, "MAX":6.0,  "DEFAULT":4.0},
    {"NAME":"branchShow",  "LABEL":"Branch",          "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":1.0},
    {"NAME":"skullShow",   "LABEL":"Face Rock",       "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":1.0},
    {"NAME":"antCount",    "LABEL":"Ant Count",       "TYPE":"float","MIN":0.0, "MAX":12.0, "DEFAULT":5.0},
    {"NAME":"reactive",    "LABEL":"Audio React",     "TYPE":"float","MIN":0.0, "MAX":2.0,  "DEFAULT":1.0},
    {"NAME":"dripBlur",    "LABEL":"Drip Blur",       "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.4},
    {"NAME":"inputTex",    "LABEL":"Texture",         "TYPE":"image"}
  ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    float a = hash(i), b = hash(i + vec2(1, 0));
    float c = hash(i + vec2(0, 1)), d = hash(i + vec2(1, 1));
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

// ──────────────────────────────────────────────────────────────────────
// Dali desert procedural scene (when no inputTex is bound)
// ──────────────────────────────────────────────────────────────────────

// One melting clock at (cx, cy), face radius r, droop direction droop.
float sdMeltingClockN(vec2 q, float r, vec2 droop) {
    // Sag: positive q.x droops in +droop direction, negative in -droop.
    float saggR = smoothstep(-r * 1.3, r * 1.8, q.x) * droop.y;
    float saggL = smoothstep(r * 1.3, -r * 1.8, q.x) * droop.x;
    q.y += saggR + saggL;
    float body  = length(q * vec2(1.4, 1.0)) - r;
    // Drape: flowing tail of the clock face that hangs off
    vec2 tailAnchor = vec2(r * 0.4, -r * 1.2);
    float drape = length(q - tailAnchor) - r * 0.55;
    drape = max(drape, q.x - r * 0.6);
    return min(body, drape);
}

// Long, thin, twisted dead branch over which clocks drape
float sdBranch(vec2 uv) {
    // Branch curve along a sine
    float bY = 0.55 + 0.04 * sin(uv.x * 6.0 - 1.2);
    float bMain = abs(uv.y - bY) - 0.012;
    // Twiggy fork
    float fY  = 0.62 + (uv.x - 0.7) * 0.6;
    float bFork = abs(uv.y - fY) - 0.008;
    bFork = max(bFork, abs(uv.x - 0.78) - 0.10);
    return min(bMain, bFork);
}

// Soft melting face/skull rock silhouette (lower right of the canvas)
float sdSoftSkull(vec2 uv) {
    vec2 p = (uv - vec2(0.65, 0.30));
    p.x *= 1.4;
    // Soft brow
    float brow = length(p - vec2(0.0, 0.04)) - 0.20;
    // Eye socket subtraction
    float eye  = length(p - vec2(-0.04, 0.06)) - 0.04;
    float jaw  = length(p - vec2(0.04, -0.06)) - 0.18;
    float skull = max(min(brow, jaw), -eye);
    return skull;
}

vec3 daliScene(vec2 uv) {
    // Sky gradient — yellow-orange high to lavender-grey low
    vec3 skyHi  = vec3(0.97, 0.78, 0.42);
    vec3 skyLo  = vec3(0.55, 0.42, 0.55);
    vec3 sky    = mix(skyLo, skyHi, smoothstep(0.45, 0.95, uv.y));
    // Distant cliffs (Persistence of Memory horizon)
    float cliff = step(0.43, uv.y) * step(uv.y, 0.50)
                * step(0.60, uv.x) * step(uv.x, 0.95);
    sky = mix(sky, vec3(0.78, 0.65, 0.35),
              cliff * smoothstep(0.6, 0.85, uv.x));
    // Ground — rich umber-ochre
    vec3 grnHi = vec3(0.72, 0.52, 0.28);
    vec3 grnLo = vec3(0.40, 0.27, 0.16);
    vec3 ground = mix(grnLo, grnHi, smoothstep(0.0, 0.45, uv.y));
    // Long shadows raking left-to-right at low height
    float shadow = smoothstep(0.0, 0.20, fract(uv.x * 5.0 + uv.y * 0.5)) - 0.5;
    ground *= 1.0 + shadow * smoothstep(0.4, 0.0, uv.y) * 0.15;

    float horizon = smoothstep(0.42, 0.46, uv.y);
    vec3 col = mix(ground, sky, horizon);

    // Soft skull/face rock
    if (skullShow > 0.001) {
        float skull = sdSoftSkull(uv);
        if (skull < 0.0) {
            // Face value depends on subsurface gradient
            float depth = clamp(-skull * 5.0, 0.0, 1.0);
            vec3 skullCol = mix(vec3(0.65, 0.45, 0.30), vec3(0.40, 0.28, 0.20), 1.0 - depth);
            col = mix(col, skullCol, skullShow * 0.85);
        }
    }

    // Dead branch
    if (branchShow > 0.001) {
        float branch = sdBranch(uv);
        if (branch < 0.0) {
            col = mix(col, vec3(0.30, 0.20, 0.12), branchShow);
        }
    }

    // Multiple melting clocks
    int N = int(clamp(clockCount, 0.0, 6.0));
    for (int i = 0; i < 6; i++) {
        if (i >= N) break;
        float fi = float(i);
        // Hashed position + radius + droop direction
        vec2 cPos = vec2(0.15 + 0.65 * hash11(fi * 7.13),
                         0.50 + 0.20 * hash11(fi * 11.7));
        float r   = 0.05 + 0.05 * hash11(fi * 13.3);
        // Droop direction: wax over an edge; direction varies
        vec2 droop = vec2(0.06 + 0.06 * hash11(fi * 17.9),
                          0.06 + 0.10 * hash11(fi * 19.3));
        // Slow time-driven droop animation — clocks visibly soften.
        droop *= 1.0 + 0.5 * sin(TIME * 0.30 + fi * 1.7);

        float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
        vec2 q = (uv - cPos) * vec2(aspect, 1.0);
        float clock = sdMeltingClockN(q, r, droop);
        if (clock < 0.0) {
            // Ochre face
            vec3 face = vec3(0.85, 0.72, 0.42);
            // Hour marks
            float th = atan(q.y, q.x);
            float rr = length(q);
            float hourMark = step(r * 0.78, rr) * step(rr, r * 0.92)
                           * step(0.85, abs(sin(th * 6.0)));
            face = mix(face, vec3(0.15, 0.10, 0.06), hourMark);
            col = face;
        }
        // Gold rim
        col = mix(col, vec3(0.95, 0.78, 0.30),
                  smoothstep(0.004, 0.0, abs(clock)) * 0.85);
    }

    return col;
}

vec3 sampleSrc(vec2 uv) {
    if (IMG_SIZE_inputTex.x > 0.0) {
        return texture(inputTex, clamp(uv, 0.0, 1.0)).rgb;
    }
    return daliScene(uv);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    // Sag — horizontal sin curve drives vertical drop. Audio mid widens
    // the drip; flow speed scaled by audioLevel so loud moments melt faster.
    float sag = sin(uv.x * sagFreq + TIME * flow * (1.0 + audioLevel * 0.5))
              * sagAmp * (1.0 + audioMid * reactive);

    vec2 base = vec2(uv.x + smear * sin(uv.y * 4.0 + TIME * flow), uv.y - sag);

    // Per-channel chromatic offset
    float ch = chroma * (1.0 + audioHigh * reactive);
    float r = sampleSrc(base + vec2(0.0,  ch)).r;
    float g = sampleSrc(base                 ).g;
    float b = sampleSrc(base + vec2(0.0, -ch)).b;
    vec3 col = vec3(r, g, b);

    // ──────────────────────────────────────────────────────────────────
    // Pixel sort drag — bright pixels drag their color downward in
    // long vertical streaks. Sample upward; if a pixel above is brighter
    // than this pixel and above the threshold, that color "leaks" down.
    // ──────────────────────────────────────────────────────────────────
    if (pixelSort > 0.001) {
        vec3 streak = vec3(0.0);
        float weight = 0.0;
        const int SS = 8;
        for (int i = 1; i <= SS; i++) {
            float fi = float(i);
            float dy = fi * 0.012 * pixelSort;
            vec3 above = sampleSrc(base + vec2(0.0, dy));
            float lAbove = max(max(above.r, above.g), above.b);
            float w = step(sortThresh, lAbove) * (1.0 - fi / float(SS + 1));
            streak += above * w;
            weight += w;
        }
        if (weight > 0.0) {
            streak /= weight;
            float thisL = max(max(col.r, col.g), col.b);
            float gate = smoothstep(sortThresh - 0.05, sortThresh + 0.05, weight / float(SS) + thisL * 0.5);
            col = mix(col, max(col, streak), pixelSort * gate * 0.85);
        }
    }

    // Drip blur — gravity-aligned downward bleed
    if (dripBlur > 0.001) {
        vec3 drip = vec3(0.0);
        const int N = 6;
        for (int i = 1; i <= N; i++) {
            float fi = float(i) / float(N);
            drip += sampleSrc(base - vec2(0.0, fi * 0.05 * dripBlur));
        }
        drip /= float(N);
        float thresh = 0.30 - audioBass * 0.15;
        float dripWeight = smoothstep(thresh, thresh + 0.1, max(max(col.r, col.g), col.b));
        col = mix(col, drip, dripBlur * 0.5 * dripWeight);
    }

    // Multiple ants crawling on different paths
    int A = int(clamp(antCount, 0.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= A) break;
        float fi = float(i);
        float speed = 0.04 * (0.5 + hash11(fi * 7.13));
        float lane  = 0.18 + 0.18 * hash11(fi * 11.7);
        float xPos  = fract(TIME * speed + hash11(fi * 13.7));
        // Slight vertical wiggle
        float yPos  = lane + 0.03 * sin(TIME * 6.0 + fi);
        vec2 ant = vec2(xPos, yPos);
        float d = length((uv - ant) * vec2(1.0, 1.6));
        float body = smoothstep(0.012, 0.0, d);
        col = mix(col, vec3(0.05, 0.04, 0.04), body);
    }

    // Soft S-curve tone map
    col = smoothstep(vec3(-0.05), vec3(1.05), col);

    gl_FragColor = vec4(col, 1.0);
}
