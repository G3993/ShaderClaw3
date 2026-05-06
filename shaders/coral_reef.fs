/*{
  "CATEGORIES": ["Generator", "Nature", "Audio Reactive"],
  "DESCRIPTION": "Underwater coral reef — branching coral skeletons in coral pink/yellow/purple, schooling fish particles weaving between them, sunbeam caustics from above, gentle current sway. Bass triggers coordinated fish-school direction reversal; treble shimmers caustics.",
  "INPUTS": [
    { "NAME": "coralCount",       "LABEL": "Coral Count",      "TYPE": "float", "MIN": 2.0,  "MAX": 6.0,  "DEFAULT": 4.0 },
    { "NAME": "coralBranchDepth", "LABEL": "Branch Depth",     "TYPE": "float", "MIN": 1.0,  "MAX": 3.0,  "DEFAULT": 3.0 },
    { "NAME": "fishCount",        "LABEL": "Fish Count",       "TYPE": "float", "MIN": 10.0, "MAX": 60.0, "DEFAULT": 36.0 },
    { "NAME": "schoolCount",      "LABEL": "School Count",     "TYPE": "float", "MIN": 1.0,  "MAX": 6.0,  "DEFAULT": 3.0 },
    { "NAME": "currentSway",      "LABEL": "Current Sway",     "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "causticIntensity", "LABEL": "Caustic Intensity","TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.9 },
    { "NAME": "audioReact",       "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "coralPink",        "LABEL": "Coral Pink",       "TYPE": "color", "DEFAULT": [1.0, 0.45, 0.55, 1.0] },
    { "NAME": "coralYellow",      "LABEL": "Coral Yellow",     "TYPE": "color", "DEFAULT": [1.0, 0.85, 0.4, 1.0] },
    { "NAME": "coralPurple",      "LABEL": "Coral Purple",     "TYPE": "color", "DEFAULT": [0.7, 0.45, 0.95, 1.0] }
  ]
}*/

// -------- Audio uniforms auto-injected by Easel ShaderSource::setAudioState --------

// -------- Hash helpers --------
float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2  hash22(vec2 p)  {
    return fract(sin(vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)))) * 43758.5453);
}

// -------- Rotation --------
vec2 rot2(vec2 p, float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c) * p; }

// -------- Capsule SDF --------
float sdCapsule(vec2 p, vec2 a, vec2 b, float r) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

// -------- Ellipse SDF (approx) for fish bodies --------
float sdEllipse(vec2 p, vec2 ab) {
    float k0 = length(p / ab);
    float k1 = length(p / (ab * ab));
    return k0 * (k0 - 1.0) / max(k1, 1e-6);
}

// -------- Triangle (fish tail) SDF --------
float sdTriangle(vec2 p, float r) {
    const float k = 1.7320508;
    p.x = abs(p.x) - r;
    p.y = p.y + r / k;
    if (p.x + k * p.y > 0.0) p = vec2(p.x - k * p.y, -k * p.x - p.y) * 0.5;
    p.x -= clamp(p.x, -2.0 * r, 0.0);
    return -length(p) * sign(p.y);
}

// -------- Value noise + fbm --------
float vNoise2(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    float a = hash21(i), b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0)), d = hash21(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}
float fbm(vec2 p) {
    float v = 0.0, amp = 0.5;
    for (int i = 0; i < 5; i++) { v += amp * vNoise2(p); p *= 2.03; amp *= 0.5; }
    return v;
}

// -------- Branching coral (capsule tree with bulbous tips + noise texture) --------
float coralBranching(vec2 p, vec2 base, float height, float width, float seed,
                     float sway, int depth, out float tipMask) {
    float sd = 1e9;
    tipMask = 0.0;
    vec2 dir0 = normalize(vec2(sway * 0.4, 1.0));
    vec2 a0 = base;
    vec2 b0 = a0 + dir0 * height;
    // Trunk with subtle width variation along length
    float trunkSD = sdCapsule(p, a0, b0, width * (1.0 + 0.15 * vNoise2(p * 30.0 + seed)));
    sd = min(sd, trunkSD);
    // Bulbous tip on trunk
    float tipTrunk = length(p - b0) - width * 1.6;
    if (tipTrunk < sd) tipMask = 1.0;
    sd = min(sd, tipTrunk);

    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        float along = (fi + 1.0) / 5.0;
        vec2 spA = a0 + dir0 * (height * along);
        float sgn = (mod(fi, 2.0) < 1.0) ? 1.0 : -1.0;
        float ang1 = 0.55 + 0.35 * hash11(seed + fi * 3.7);
        vec2 dir1 = rot2(dir0, sgn * ang1);
        float L1 = height * (0.45 - 0.05 * fi);
        vec2 spB = spA + dir1 * L1;
        sd = min(sd, sdCapsule(p, spA, spB, width * 0.7));
        float tipL1 = length(p - spB) - width * 1.2;
        if (tipL1 < sd) tipMask = 1.0;
        sd = min(sd, tipL1);

        if (depth >= 2) {
            for (int k = 0; k < 3; k++) {
                float fk = float(k);
                float along2 = (fk + 1.0) / 4.0;
                vec2 twA = spA + dir1 * (L1 * along2);
                float sgn2 = (mod(fk, 2.0) < 1.0) ? 1.0 : -1.0;
                float ang2 = 0.5 + 0.3 * hash11(seed + fi * 7.1 + fk * 11.3);
                vec2 dir2 = rot2(dir1, sgn2 * ang2);
                float L2 = L1 * 0.55;
                vec2 twB = twA + dir2 * L2;
                sd = min(sd, sdCapsule(p, twA, twB, width * 0.45));
                float tipL2 = length(p - twB) - width * 0.9;
                if (tipL2 < sd) tipMask = 1.0;
                sd = min(sd, tipL2);

                if (depth >= 3) {
                    vec2 hA = twA + dir2 * (L2 * 0.5);
                    vec2 dir3 = rot2(dir2, sgn2 * 0.55);
                    vec2 hB = hA + dir3 * (L2 * 0.5);
                    sd = min(sd, sdCapsule(p, hA, hB, width * 0.28));
                    float tipL3 = length(p - hB) - width * 0.7;
                    if (tipL3 < sd) tipMask = 1.0;
                    sd = min(sd, tipL3);
                }
            }
        }
    }
    // Surface noise jitters edge
    sd += 0.0015 * (vNoise2(p * 80.0 + seed) - 0.5);
    return sd;
}

// -------- Brain coral (rounded mound with squiggly ridge pattern) --------
float coralBrain(vec2 p, vec2 base, float height, float width, float seed,
                 float sway, out float ridgeMask) {
    vec2 c = base + vec2(sway * 0.3, height * 0.45);
    vec2 d = (p - c) / vec2(height * 0.55, height * 0.45);
    float r = length(d);
    float mound = (r - 1.0) * height * 0.5;
    // Squiggly ridges via cos pattern in polar
    float ang = atan(d.y, d.x);
    float ridges = cos(ang * 7.0 + r * 14.0 + seed * 3.0)
                 + 0.6 * cos(ang * 11.0 - r * 9.0 + seed * 1.7);
    ridgeMask = smoothstep(0.0, 1.2, ridges) * smoothstep(1.05, 0.8, r);
    // Carve ridges into surface slightly
    mound += 0.004 * cos(ang * 9.0 + r * 18.0 + seed);
    return mound;
}

// -------- Fan coral (vertical flat fan with hashed ridges) --------
float coralFan(vec2 p, vec2 base, float height, float width, float seed,
               float sway, out float fanRidgeMask) {
    // Fan grows up from base, slightly leaning with sway
    vec2 q = p - base;
    q = rot2(q, -sway * 0.5);
    // Half-disc shape: large radius, but only upper half, tapered horizontally
    float r = length(q / vec2(height * 0.55, height * 0.95));
    float disc = (r - 1.0) * height * 0.5;
    // Cut off below base
    float floorCut = -q.y;
    float sd = max(disc, floorCut);
    // Hashed radial ridges
    float ang = atan(q.y, max(q.x * 0.6, 0.001) + 0.0001);
    float ridge = abs(fract(ang * 8.0 + hash11(seed) * 6.28) - 0.5);
    fanRidgeMask = smoothstep(0.45, 0.15, ridge) * smoothstep(1.0, 0.7, r);
    // Slight thickness wobble
    sd += 0.002 * (vNoise2(q * 60.0 + seed) - 0.5);
    return sd;
}

// -------- Caustics: thin layered fbm difference, thresholded --------
float caustics(vec2 p, float t, float shimmer) {
    vec2 q1 = p * 3.5 + vec2(0.0, t * 0.15);
    vec2 q2 = p * 4.2 + vec2(t * 0.22, -t * 0.18);
    float a = fbm(q1 + vec2(t * 0.07, 0.0));
    float b = fbm(q2 - vec2(0.0, t * 0.09));
    float c = pow(max(a - b, 0.0), 2.0) * 6.0;
    return c * (0.7 + 0.6 * shimmer);
}

// -------- Reef palette pick (5 curated colors) --------
vec3 reefPalette(float h) {
    if (h < 0.20) return vec3(0.95, 0.55, 0.55); // coral pink
    if (h < 0.40) return vec3(0.95, 0.92, 0.65); // pale yellow
    if (h < 0.60) return vec3(0.55, 0.30, 0.78); // purple
    if (h < 0.80) return vec3(0.62, 0.38, 0.22); // warm brown
    return                vec3(0.45, 0.78, 0.55); // sage green
}

void main() {
    vec2 res = RENDERSIZE;
    vec2 uv = (gl_FragCoord.xy - vec2(0.5 * res.x, 0.0)) / res.y;

    float t = TIME;
    float bass   = clamp(audioBass * audioReact, 0.0, 4.0);
    float mid    = clamp(audioMid  * audioReact, 0.0, 4.0);
    float treble = clamp(audioHigh * audioReact, 0.0, 4.0);
    float lvl    = clamp(audioLevel * audioReact, 0.0, 4.0);

    float aspect = res.x / res.y;
    float aa = 1.5 / res.y;

    // Water gradient: warm sandy floor → deep cyan-blue up.
    vec3 sand    = vec3(0.92, 0.82, 0.55);
    vec3 shallow = vec3(0.20, 0.65, 0.78);
    vec3 deep    = vec3(0.02, 0.12, 0.28);
    float floorH = 0.18;
    float seaT   = smoothstep(floorH, 0.55, uv.y);
    vec3 col     = mix(sand * (0.85 + 0.15 * (uv.y / floorH)), shallow, seaT);
    col          = mix(col, deep, smoothstep(0.55, 1.1, uv.y));

    // Sandy floor noise texture.
    float sandN = fbm(uv * 8.0 + vec2(0.0, t * 0.05));
    if (uv.y < floorH) col = mix(col, sand * (0.7 + 0.4 * sandN), 0.6);

    // Caustics, layered with stronger emphasis on sand floor
    float cau = caustics(uv, t, treble);
    float floorEmphasis = (1.0 - smoothstep(0.0, 0.3, uv.y - 0.0));
    float beamMask = smoothstep(floorH, 1.0, uv.y);
    col += vec3(0.95, 0.98, 1.0) * cau * causticIntensity * (0.35 * beamMask + 0.85 * floorEmphasis);

    // -------- Sunbeams from upper edge (3-5 angled capsules with exp falloff) --------
    {
        const int NB = 4;
        for (int b = 0; b < NB; b++) {
            float fb = float(b);
            float bs = fb * 19.7 + 5.3;
            float drift = sin(t * (0.13 + 0.07 * hash11(bs)) + bs) * 0.35;
            float xTop = mix(-aspect * 0.9, aspect * 0.9, (fb + 0.5) / float(NB)) + drift;
            float ang  = 0.18 + 0.22 * hash11(bs * 1.7);
            vec2 dir   = vec2(sin(ang), -cos(ang));
            vec2 a     = vec2(xTop, 1.2);
            vec2 b2    = a + dir * 1.8;
            float bd   = sdCapsule(uv, a, b2, 0.04 + 0.02 * hash11(bs * 2.3));
            float glow = exp(-max(bd, 0.0) * 22.0);
            col += vec3(1.0, 0.96, 0.78) * glow * (0.18 + 0.12 * treble) * beamMask;
        }
    }

    // Vertical sunbeam shimmer streaks (kept).
    float beams = pow(0.5 + 0.5 * sin(uv.x * 3.0 + fbm(vec2(uv.x * 2.0, t * 0.2)) * 4.0), 8.0);
    col += vec3(1.0, 0.95, 0.7) * beams * (0.06 + 0.04 * treble) * beamMask;

    // -------- Coral towers (3 types: branching / brain / fan) --------
    int   nCoral = int(clamp(coralCount, 2.0, 6.0));
    int   depth  = int(clamp(coralBranchDepth, 1.0, 3.0));
    float swayG  = currentSway * (0.06 + 0.05 * sin(t * 0.6));

    float coralSD = 1e9;
    vec3  coralCol = vec3(0.0);
    float coralAccent = 0.0; // ridge / tip highlight

    for (int i = 0; i < 6; i++) {
        if (i >= nCoral) break;
        float fi    = float(i);
        float seed  = fi * 17.31 + 3.7;
        float xPos  = mix(-aspect * 0.85, aspect * 0.85,
                          (fi + 0.5) / float(nCoral) + (hash11(seed) - 0.5) * 0.15);
        vec2  base  = vec2(xPos, floorH * 0.85);
        float h     = 0.45 + 0.35 * hash11(seed * 1.7);
        float w     = 0.012 + 0.008 * hash11(seed * 2.3);
        float localSway = swayG * sin(t * 0.7 + seed) * (0.5 + 0.5 * mid);

        float typePick = hash11(seed * 9.13);
        float sd;
        float accent = 0.0;
        if (typePick < 0.34) {
            float tipMask;
            sd = coralBranching(uv, base, h, w, seed, localSway, depth, tipMask);
            accent = tipMask;
        } else if (typePick < 0.67) {
            float ridgeMask;
            sd = coralBrain(uv, base, h * 0.55, w, seed, localSway, ridgeMask);
            accent = ridgeMask;
        } else {
            float fanRidgeMask;
            sd = coralFan(uv, base, h * 0.85, w, seed, localSway, fanRidgeMask);
            accent = fanRidgeMask;
        }

        // Hashed reef palette pick
        vec3 cc = reefPalette(hash11(seed * 5.1));
        // Slight per-tower brightness/saturation variation
        cc *= 0.9 + 0.2 * hash11(seed * 6.3);

        if (sd < coralSD) {
            coralSD = sd;
            coralCol = cc;
            coralAccent = accent;
        }
    }

    float coralFill = 1.0 - smoothstep(-aa, aa, coralSD);
    float coralGlow = exp(-max(coralSD, 0.0) * 28.0) * 0.6;
    // Accent: brighten ridges/tips
    vec3 coralShade = coralCol * (0.85 + 0.35 * coralAccent);
    col = mix(col, coralShade, coralFill);
    col += coralCol * coralGlow * (0.4 + 0.5 * lvl);

    // -------- Schooling fish (body ellipse + tail triangle + wiggle) --------
    int   nFish    = int(clamp(fishCount, 10.0, 60.0));
    int   nSchools = int(clamp(schoolCount, 1.0, 6.0));
    float flipPhase = floor(t * (0.4 + 1.5 * bass));
    for (int i = 0; i < 60; i++) {
        if (i >= nFish) break;
        float fi    = float(i);
        float fseed = fi * 9.71 + 1.3;
        float schoolIdx = floor(hash11(fseed) * float(nSchools));
        float sseed = schoolIdx * 41.7 + 7.1;
        float baseDir = hash11(sseed) * 6.2831;
        float flipBit = mod(floor(flipPhase + hash11(sseed * 1.3) * 2.0), 2.0);
        float dir     = baseDir + flipBit * 3.14159;
        vec2  vel     = vec2(cos(dir), sin(dir)) * (0.07 + 0.04 * hash11(sseed * 2.1));

        vec2 origin = vec2(
            mix(-aspect, aspect, hash11(fseed * 1.7)),
            mix(floorH + 0.05, 1.05, hash11(fseed * 2.9))
        );
        vec2 fp = origin + vel * t;
        fp.x += currentSway * 0.05 * sin(t * 0.9 + fseed);
        fp.y += 0.02 * sin(t * 1.7 + fseed * 0.7);
        fp.x = mod(fp.x + aspect * 1.2, aspect * 2.4) - aspect * 1.2;
        fp.y = mod(fp.y - floorH, 1.05 - floorH) + floorH;

        vec2 lp = rot2(uv - fp, -dir);
        float fishScale = 0.85 + 0.5 * hash11(fseed * 3.3); // varied per fish
        float fishSize  = (0.012 + 0.005 * hash11(fseed * 5.1)) * fishScale;

        // Body: oval ellipse along x
        float body = sdEllipse(lp - vec2(fishSize * 0.2, 0.0),
                               vec2(fishSize * 1.6, fishSize * 0.7));

        // Tail wiggle: tail pivots based on time + per-fish phase
        float wiggle = sin(t * 8.0 + fseed * 2.7) * 0.5;
        vec2 tailP = lp + vec2(fishSize * 1.3, 0.0);
        tailP = rot2(tailP, wiggle);
        // Tail triangle pointing back (-x), sized smaller than body
        float tail = sdTriangle(rot2(tailP, -1.5708), fishSize * 0.7);

        float fishSD = min(body, tail);
        float fishMask = 1.0 - smoothstep(-aa, aa, fishSD);

        // Subtle eye dot on body
        float eye = length(lp - vec2(fishSize * 0.9, fishSize * 0.15)) - fishSize * 0.12;
        float eyeMask = 1.0 - smoothstep(-aa, aa, eye);

        vec3 fishCol = mix(vec3(0.9, 0.92, 0.95),
                           mix(vec3(1.0, 0.6, 0.3), vec3(0.5, 0.7, 1.0), hash11(sseed * 4.7)),
                           0.6);
        col = mix(col, fishCol, fishMask * 0.85);
        col = mix(col, vec3(0.05, 0.05, 0.08), eyeMask * fishMask * 0.9);
    }

    // -------- Particulates: 150 plankton/sediment dots drifting upward --------
    for (int i = 0; i < 150; i++) {
        float fi = float(i);
        float ps = fi * 13.7 + 2.3;
        float speed = 0.015 + 0.06 * hash11(ps * 2.1);
        vec2 pp = vec2(
            mix(-aspect, aspect, hash11(ps)) + 0.05 * sin(t * 0.4 + fi * 0.7),
            mod(hash11(ps * 1.7) - t * speed, 1.15)
        );
        float d = length(uv - pp);
        float bright = 0.25 + 0.35 * hash11(ps * 3.3);
        // Smaller / dimmer for distant feel
        float radius = 280.0 + 220.0 * hash11(ps * 4.1);
        col += vec3(0.92, 0.96, 1.0) * exp(-d * radius) * bright;
    }

    // -------- Volumetric haze: low-freq fbm dimming distant pixels --------
    {
        float distFromCenter = length((gl_FragCoord.xy - 0.5 * res) / res.y);
        float hazeFbm = fbm(uv * 1.4 + vec2(t * 0.03, t * 0.02));
        // Distance-driven depth + haze-noise modulation
        float depthAmount = smoothstep(0.3, 1.2, distFromCenter) * 0.55;
        depthAmount *= 0.7 + 0.6 * hazeFbm;
        depthAmount = clamp(depthAmount, 0.0, 0.75);
        vec3 hazeColor = mix(vec3(0.10, 0.32, 0.48), vec3(0.04, 0.16, 0.30), uv.y);
        col = mix(col, hazeColor, depthAmount);
    }

    // Subtle bass flash and depth darkening.
    col += vec3(0.1, 0.2, 0.35) * 0.07 * bass;
    float vig = 1.0 - smoothstep(0.6, 1.4, length((gl_FragCoord.xy - 0.5 * res) / res.y));
    col *= mix(0.7, 1.0, vig);

    gl_FragColor = vec4(col, 1.0);
}
