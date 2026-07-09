/*{
  "CATEGORIES": [
    "Generator",
    "Atmospheric",
    "Audio Reactive"
  ],
  "DESCRIPTION": "High-fidelity branching forked lightning bolts with continuous polyline SDF, hot core + cyan plasma sheath, stochastic Brownian forks, parallax stormcloud layers, diagonal rain streaks, screen flash with chromatic aberration, ground silhouette, bloom, grain, and vignette. Bass triggers strikes, mids amplify the storm, highs shimmer the clouds.",
  "INPUTS": [
    {
      "NAME": "boltProbability",
      "LABEL": "Bolt Chance",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.55
    },
    {
      "NAME": "flashIntensity",
      "LABEL": "Flash Intensity",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0.95
    },
    {
      "NAME": "rainAngle",
      "LABEL": "Rain Angle",
      "TYPE": "float",
      "MIN": -0.6,
      "MAX": 0.6,
      "DEFAULT": 0.26
    },
    {
      "NAME": "stormDensity",
      "LABEL": "Storm Density",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1.5,
      "DEFAULT": 0.92,
      "GROUP": "Shape / Geometry"
    },
    {
      "NAME": "boltBranchDepth",
      "LABEL": "Branch Depth",
      "TYPE": "float",
      "MIN": 1,
      "MAX": 4,
      "DEFAULT": 3,
      "GROUP": "Shape / Geometry"
    },
    {
      "NAME": "boltCoreWidth",
      "LABEL": "Core Width",
      "TYPE": "float",
      "MIN": 0.0005,
      "MAX": 0.012,
      "DEFAULT": 0.0025,
      "GROUP": "Shape / Geometry"
    },
    {
      "NAME": "boltSheathWidth",
      "LABEL": "Sheath Width",
      "TYPE": "float",
      "MIN": 0.005,
      "MAX": 0.05,
      "DEFAULT": 0.018,
      "GROUP": "Shape / Geometry"
    },
    {
      "NAME": "rainDensity",
      "LABEL": "Rain Density",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.55,
      "GROUP": "Shape / Geometry"
    },
    {
      "NAME": "cloudDrift",
      "LABEL": "Cloud Drift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.2,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "boltLifetime",
      "LABEL": "Bolt Lifetime",
      "TYPE": "float",
      "MIN": 0.15,
      "MAX": 2.5,
      "DEFAULT": 0.95,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "boltJitter",
      "LABEL": "Bolt Jitter",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.3,
      "DEFAULT": 0.085,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "rainSpeed",
      "LABEL": "Rain Speed",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 3,
      "DEFAULT": 1.5,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "skyTopColor",
      "LABEL": "Sky Top",
      "TYPE": "color",
      "DEFAULT": [
        0.018,
        0.022,
        0.052,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "skyBotColor",
      "LABEL": "Sky Bottom",
      "TYPE": "color",
      "DEFAULT": [
        0.1,
        0.12,
        0.18,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "boltCoreColor",
      "LABEL": "Bolt Core",
      "TYPE": "color",
      "DEFAULT": [
        1,
        1,
        1,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "boltSheathColor",
      "LABEL": "Bolt Sheath",
      "TYPE": "color",
      "DEFAULT": [
        0.55,
        0.78,
        1,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "hueShift",
      "LABEL": "Hue Shift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0,
      "GROUP": "Color"
    },
    {
      "NAME": "colorBoost",
      "LABEL": "Color Boost",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Color"
    },
    {
      "NAME": "groundLine",
      "LABEL": "Ground Silhouette",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.3,
      "DEFAULT": 0.075,
      "GROUP": "Camera / Layout"
    },
    {
      "NAME": "audioReact",
      "LABEL": "Audio React",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Audio Reactivity"
    }
  ]
}*/

// ---------- hashing & noise ----------
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash12(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2  hash21(float n) { return fract(sin(vec2(n * 12.9898, n * 78.233)) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash12(ip);
    float b = hash12(ip + vec2(1.0, 0.0));
    float c = hash12(ip + vec2(0.0, 1.0));
    float d = hash12(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 r = mat2(0.8, -0.6, 0.6, 0.8);
    for (int i = 0; i < 6; i++) {
        v += a * vnoise(p);
        p = r * p * 2.04;
        a *= 0.5;
    }
    return v;
}

// ---------- continuous polyline distance ----------
// True point-to-segment distance — pixel-exact, no per-segment "dot connecting".
float sdSeg(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-8), 0.0, 1.0);
    return length(pa - ba * h);
}

// Distance from p to a tapered segment: width interpolates from wA at a to wB at b.
// Returns SIGNED distance (negative inside the tapered tube). Use abs for SDF.
float sdTaperedSeg(vec2 p, vec2 a, vec2 b, float wA, float wB) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-8), 0.0, 1.0);
    float w = mix(wA, wB, h);
    return length(pa - ba * h) - w;
}

// ---------- bolt construction ----------
// We compute, per pixel, the MINIMUM signed distance to the entire bolt polyline
// (trunk + branches + sub-branches), each segment tapered. The result is a single
// continuous SDF from which core + sheath + glow are derived.
//
// Parameters returned packed:
//   .x = min unsigned distance to core line (for core SDF)
//   .y = min unsigned distance to sheath envelope (for sheath SDF)
//   .z = signed core SDF (negative inside)
vec3 boltSDF(vec2 uv, float seed, float depth, float jitter, float life01) {
    // Bolt anchors.
    float topX = mix(0.18, 0.82, hash11(seed * 7.13 + 1.0));
    float botX = topX + (hash11(seed * 3.7 + 11.0) - 0.5) * 0.55;
    vec2 a = vec2(topX, 1.08);
    vec2 b = vec2(botX, -0.06);

    // Coverage envelope: at life01=0 head emerges, by ~0.15 it has fully struck.
    float strike = smoothstep(0.0, 0.18, life01);

    // Trunk direction (normalized) and perpendicular for proper random-walk offset.
    vec2 trunkDir = normalize(b - a);
    vec2 perp = vec2(-trunkDir.y, trunkDir.x);

    const int SEGS = 22;
    float minDC = 1e9;     // min distance to core skeleton
    float minDS = 1e9;     // min distance to sheath skeleton (signed via taper)

    // Brownian-style accumulator: each segment adds a hashed perpendicular kick.
    vec2 prev = a;
    float walk = 0.0;
    // Endpoint reachable so far (for spawning branches).
    vec2 segPivots[6];
    float segPivotT[6];
    int pivotCount = 0;

    for (int i = 1; i <= SEGS; i++) {
        float t = float(i) / float(SEGS);
        // Reveal segments progressively as the bolt strikes.
        if (t > strike + 0.05) break;

        // Brownian walk: hashed delta added to running perpendicular offset.
        float h = hash11(seed * 19.7 + float(i) * 3.1) - 0.5;
        // Stronger kicks in the middle, anchor to endpoints.
        float taperJ = sin(t * 3.14159);
        walk += h * jitter * 0.42;
        walk *= 0.86; // damping toward zero so trunk doesn't drift far
        vec2 base = mix(a, b, t);
        vec2 cur = base + perp * walk * taperJ;

        // Trunk width tapers from full at top to ~0.4 near the tip.
        float wA = mix(0.0030, 0.0014, (t - 1.0/float(SEGS)));
        float wB = mix(0.0030, 0.0014, t);

        float dc = sdSeg(uv, prev, cur);
        float ds = sdTaperedSeg(uv, prev, cur, wA, wB);
        minDC = min(minDC, dc);
        minDS = min(minDS, ds);

        // Stash a few pivots for branch spawning.
        // (GLSL ES 1.0: array writes need const/loop indices — store via const loop.)
        if (pivotCount < 6 && hash11(seed * 5.0 + float(i) * 2.13) > 0.55) {
            for (int s = 0; s < 6; s++) {
                if (s == pivotCount) { segPivots[s] = cur; segPivotT[s] = t; break; }
            }
            pivotCount++;
        }

        prev = cur;
    }

    // ---------- branches ----------
    int branches = int(clamp(depth, 1.0, 4.0));
    for (int k = 0; k < 4; k++) {
        if (k >= branches || k >= pivotCount) break;
        if (segPivotT[k] > strike + 0.02) continue;

        vec2 bp = segPivots[k];
        float bs = seed * 41.0 + float(k) * 17.31;

        // Branch direction = trunk direction rotated by 30-60°.
        float ang = mix(0.52, 1.05, hash11(bs + 1.7));
        ang *= (hash11(bs + 5.5) > 0.5) ? 1.0 : -1.0;
        float ca = cos(ang), sa = sin(ang);
        vec2 brDir = mat2(ca, -sa, sa, ca) * trunkDir;
        vec2 brPerp = vec2(-brDir.y, brDir.x);
        float reach = mix(0.18, 0.42, hash11(bs + 9.1));
        vec2 brEnd = bp + brDir * reach;

        const int BSEGS = 11;
        vec2 bprev = bp;
        float bwalk = 0.0;
        // Branch sub-pivots for sub-branches.
        vec2 subPivots[3];
        int subCount = 0;

        for (int j = 1; j <= BSEGS; j++) {
            float tt = float(j) / float(BSEGS);
            float bh = hash11(bs + float(j) * 2.31) - 0.5;
            bwalk += bh * jitter * 0.35;
            bwalk *= 0.82;
            vec2 bbase = mix(bp, brEnd, tt);
            vec2 bcur = bbase + brPerp * bwalk * sin(tt * 3.14159);

            // Branches at 60% trunk width, tapering toward tip.
            float bwA = mix(0.0019, 0.0007, (tt - 1.0/float(BSEGS)));
            float bwB = mix(0.0019, 0.0007, tt);

            float dc = sdSeg(uv, bprev, bcur);
            float ds = sdTaperedSeg(uv, bprev, bcur, bwA, bwB);
            minDC = min(minDC, dc);
            minDS = min(minDS, ds);

            if (subCount < 3 && hash11(bs + float(j) * 7.7) > 0.72) {
                for (int s = 0; s < 3; s++) {
                    if (s == subCount) { subPivots[s] = bcur; break; }
                }
                subCount++;
            }
            bprev = bcur;
        }

        // ---------- sub-branches (1-2 recursive levels) ----------
        for (int m = 0; m < 3; m++) {
            if (m >= subCount) break;
            if (depth < 2.5) break;
            vec2 sp = subPivots[m];
            float ss = bs * 13.7 + float(m) * 5.3;

            float sang = mix(0.5, 1.0, hash11(ss + 1.1));
            sang *= (hash11(ss + 2.2) > 0.5) ? 1.0 : -1.0;
            float sca = cos(sang), ssa = sin(sang);
            vec2 sDir = mat2(sca, -ssa, ssa, sca) * brDir;
            vec2 sPerp = vec2(-sDir.y, sDir.x);
            float sReach = mix(0.06, 0.18, hash11(ss + 3.3));
            vec2 sEnd = sp + sDir * sReach;

            const int SSEGS = 6;
            vec2 sprev = sp;
            float swalk = 0.0;
            for (int q = 1; q <= SSEGS; q++) {
                float qt = float(q) / float(SSEGS);
                float qh = hash11(ss + float(q) * 1.93) - 0.5;
                swalk += qh * jitter * 0.28;
                swalk *= 0.78;
                vec2 sbase = mix(sp, sEnd, qt);
                vec2 scur = sbase + sPerp * swalk * sin(qt * 3.14159);

                // Sub-branches at 40% width.
                float swA = mix(0.0012, 0.0004, (qt - 1.0/float(SSEGS)));
                float swB = mix(0.0012, 0.0004, qt);

                float dc = sdSeg(uv, sprev, scur);
                float ds = sdTaperedSeg(uv, sprev, scur, swA, swB);
                minDC = min(minDC, dc);
                minDS = min(minDS, ds);
                sprev = scur;
            }
        }
    }

    return vec3(minDC, minDS, minDS);
}

// Lifetime envelope: sharp white onset, exp decay, fast flicker tail.
float boltEnv(float life01, float seed) {
    float onset = smoothstep(0.0, 0.04, life01);
    float decay = exp(-life01 * 4.2);
    float flick = 0.65 + 0.35 * sin(life01 * 110.0 + seed * 7.0)
                       * sin(life01 * 47.0 + seed * 3.0);
    flick = mix(1.0, flick, smoothstep(0.18, 0.55, life01));
    return onset * decay * flick;
}

// ---------- main ----------
void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 uva = vec2(uv.x * aspect, uv.y);

    // Audio: bass triggers strikes, mid amplifies storm, high adds cloud shimmer.
    float bass = clamp(audioBass * audioReact, 0.0, 2.0);
    float mid  = clamp(audioMid  * audioReact, 0.0, 2.0);
    float high = clamp(audioHigh * audioReact, 0.0, 2.0);
    // LINEAR band conditioning (round 2): the round-1 pow(smoothstep(...))
    // knees crushed jazz's soft 0.4-0.5 swung accents and hiphop's sparse
    // sub-heavy kicks to near-zero. The bands are already smoothed upstream —
    // use them linearly; reserve shaping for event terms only.
    float aR     = min(audioReact, 1.5);
    float bassP  = clamp(audioBass, 0.0, 1.0) * aR;
    float subP   = clamp(audioSub,  0.0, 1.0) * aR;
    float midP   = clamp(audioMid,  0.0, 1.0) * aR;
    float punchP = clamp(audioPunch, 0.0, 1.0) * aR;
    // Smooth kick weight — floor lowered to 0.03 so sparse soft kicks count.
    float kick = smoothstep(0.03, 0.50, max(audioBass, audioSub) * audioReact);
    // R3: music-present bolt gate. The free-running slot timer fires huge
    // flashes with no relation to the music (hiphop scored 0.0 with p95 0.25).
    // When audio is present, bolt brightness follows the decaying kick trace
    // instead; between kicks stray bolts dim to 10%. Silence → presence 0 →
    // gate is exactly 1.0 and the original self-timed storm is untouched.
    float presence  = smoothstep(0.06, 0.30, audioLevel * aR);
    float kickTrace = clamp(1.35 * audioBeatPulse + 0.9 * audioPunch + 0.5 * subP, 0.0, 1.0);
    float boltGate  = mix(1.0, 0.10 + 0.90 * kickTrace, presence);
    float stormAmp = stormDensity * (1.0 + 0.45 * mid);

    // ===== STORMCLOUDS — multi-layer parallax =====
    // Atmospheric darkness gradient (top darker than bottom).
    vec3 sky = mix(skyBotColor.rgb, skyTopColor.rgb, smoothstep(-0.05, 1.05, uv.y));

    // Layer 1: far high cumulus — large scale, slow drift.
    vec2 farUv = uv * vec2(1.6, 0.95);
    farUv.x *= aspect;
    float farDrift = TIME * cloudDrift * 0.35;
    float farClouds = fbm(farUv + vec2(farDrift, farDrift * 0.18));
    farClouds = pow(farClouds, mix(1.7, 0.9, stormAmp));

    // Layer 2: near low scud — smaller scale, faster drift.
    vec2 nearUv = uv * vec2(3.6, 1.8);
    nearUv.x *= aspect;
    float nearDrift = TIME * cloudDrift * 1.25;
    float nearClouds = fbm(nearUv + vec2(nearDrift, -nearDrift * 0.4));
    nearClouds = pow(nearClouds, mix(1.4, 0.7, stormAmp));

    // High-frequency shimmer driven by treble.
    float shimmer = vnoise(uv * 22.0 + TIME * (0.6 + high * 1.4));
    float shimmerMix = high * 0.18;

    // Compose clouds: dark grey-blue mass, denser/lower = darker.
    vec3 col = sky;
    vec3 farTone  = sky * 0.6  + vec3(0.018, 0.022, 0.030);
    vec3 nearTone = sky * 0.32 + vec3(0.008, 0.010, 0.018);
    col = mix(col, farTone, clamp(farClouds * stormAmp * 1.05, 0.0, 1.0));
    col = mix(col, nearTone, clamp(nearClouds * stormAmp * 0.85, 0.0, 1.0));
    col += vec3(0.04, 0.05, 0.08) * shimmer * shimmerMix * stormAmp;

    // Top-of-canvas darkness gradient for atmospheric depth.
    col *= mix(0.55, 1.0, smoothstep(0.95, 0.10, uv.y));

    // Continuous band-follow: cloud luminance breathes with bass/mids so the
    // storm tracks beatless material (ambient) too — smooth, no gates.
    // Sub joins so hiphop's sub-heavy kicks move the sky too.
    col *= 1.0 + 0.22 * bassP + 0.12 * midP + 0.12 * subP;

    // ===== BOLT SCHEDULING =====
    // Slot-based time buckets; each slot may fire with hashed probability.
    float slot = max(boltLifetime * 0.85, 0.28);
    float boltCore = 0.0;
    float boltSheath = 0.0;
    float boltOuter = 0.0;
    float flashEnv = 0.0;
    float aliveAmt = 0.0;

    for (int s = -1; s <= 1; s++) {
        float slotIdx = floor(TIME / slot) + float(s);
        float slotStart = slotIdx * slot;
        float life = TIME - slotStart;
        float life01 = clamp(life / boltLifetime, 0.0, 1.0);
        if (life < 0.0 || life > boltLifetime) continue;

        float seed = slotIdx * 13.37 + 1.0;
        float chance = hash11(seed * 0.917);
        // Bass kicks lower the threshold; mid contributes baseline storm activity.
        // Cap below 1.0 so constant loud input (edm) can't saturate the
        // schedule into a fixed audio-blind pulse train.
        float thresh = 1.0 - clamp(boltProbability + 0.35 * kick + 0.12 * mid, 0.0, 0.95);
        if (chance < thresh) continue;

        // Up to 2 bolts on heavy bass.
        float boltCount = 1.0 + step(0.55, bass) * step(0.5, hash11(seed + 31.7));
        for (float bi = 0.0; bi < 2.0; bi += 1.0) {
            if (bi >= boltCount) break;
            float bseed = seed + bi * 91.7;

            vec3 sd = boltSDF(uva, bseed, boltBranchDepth, boltJitter, life01);
            float d = sd.x;          // unsigned distance to skeleton
            float env = boltEnv(life01, bseed);
            // Bolt luminance follows the music: the slot schedule alone can't
            // carry correlation, so brightness tracks bass (sub couples the
            // sparse hiphop kicks, punch accents soft jazz transients).
            // Base restored to 1.0 so silence = exact original bolt look.
            env *= 1.0 + 0.40 * bassP + 0.35 * subP + 0.30 * punchP;
            env *= boltGate;

            // HOT CORE: very tight Gaussian falloff at near-white intensity.
            float coreR = boltCoreWidth;
            float core = exp(-(d * d) / (coreR * coreR)) * env;

            // COOL SHEATH: wider Gaussian at cyan-blue, lower intensity.
            float sheathR = boltSheathWidth;
            float sheath = exp(-(d * d) / (sheathR * sheathR)) * env * 0.65;

            // OUTER GLOW: very wide faint plasma halo.
            float outerR = boltSheathWidth * 4.0;
            float outer = exp(-(d * d) / (outerR * outerR)) * env * 0.22;

            boltCore   += core;
            boltSheath += sheath;
            boltOuter  += outer;

            // Flash envelope: peaks at strike onset, decays in ~200ms.
            float fenv = exp(-life * 9.5) * step(life, 0.30);
            flashEnv = max(flashEnv, fenv * env);
            aliveAmt = max(aliveAmt, env);
        }
    }

    // ===== APPLY BOLT WITH HOT CORE + COOL SHEATH =====
    vec3 coreCol = boltCoreColor.rgb;
    vec3 sheathCol = boltSheathColor.rgb;
    // Outer halo same hue as sheath but desaturated toward grey-blue.
    vec3 outerCol = mix(sheathCol, vec3(0.35, 0.45, 0.65), 0.4);

    col += outerCol  * clamp(boltOuter,  0.0, 4.0) * 0.85;
    col += sheathCol * clamp(boltSheath, 0.0, 4.0) * 1.10;
    col += coreCol   * clamp(boltCore,   0.0, 6.0) * 1.25;

    // ===== WHOLE-CANVAS FLASH WASH =====
    // Brief bright tint multiplied across everything when bolt is alive.
    vec3 flashTint = vec3(0.95, 0.97, 1.0);
    float flashAmt = flashEnv * flashIntensity;
    // Multiply existing scene up (illumination) and add white wash.
    col *= 1.0 + flashAmt * 1.4;
    col += flashTint * flashAmt * 0.55;

    // ===== DIAGONAL RAIN — long thin streak capsules at ~15° =====
    if (rainDensity > 0.0) {
        // Two layers of rain for depth.
        for (int rl = 0; rl < 2; rl++) {
            float layer = float(rl);
            float scale = mix(1.0, 0.55, layer);
            float alpha = mix(0.22, 0.10, layer);
            vec2 ruv = uv;
            // Skew to rainAngle (default ~15°).
            ruv.x += ruv.y * rainAngle;
            ruv *= vec2(180.0 * scale, 22.0 * scale);
            ruv.y += TIME * rainSpeed * (16.0 + 8.0 * layer);
            // Slight horizontal jitter per row for organic feel.
            ruv.x += hash11(floor(ruv.y) * 1.7 + layer * 13.0) * 0.7;
            vec2 rip = floor(ruv);
            vec2 rfp = fract(ruv);
            float rh = hash12(rip + vec2(layer * 31.0, 0.0));
            // Streaks are sparse: only some cells light up.
            float on = step(1.0 - rainDensity * 0.45, rh);
            // Thin vertical capsule: narrow x extent, long y extent within cell.
            float xProf = smoothstep(0.50, 0.46, abs(rfp.x - 0.5));
            float yProf = smoothstep(0.0, 0.15, rfp.y) * (1.0 - smoothstep(0.55, 1.0, rfp.y));
            float streak = xProf * yProf * on;
            // Length variation.
            streak *= 0.5 + 0.5 * hash11(rh * 7.0 + layer * 3.0);
            // Dim during flash so flash doesn't blow out rain.
            streak *= (1.0 - flashEnv * 0.55);
            col += vec3(0.55, 0.65, 0.82) * streak * alpha;
        }
    }

    // ===== GROUND SILHOUETTE — jagged horizon =====
    if (groundLine > 0.0) {
        float horizon = groundLine
            + 0.022 * fbm(vec2(uv.x * 7.0, 0.0))
            + 0.012 * vnoise(vec2(uv.x * 22.0, 0.0));
        float ground = smoothstep(horizon + 0.004, horizon - 0.004, uv.y);
        col = mix(col, vec3(0.005, 0.008, 0.014), ground);
        // Bolt + flash illuminates ground rim.
        float rimGlow = aliveAmt * 0.35 + flashAmt * 0.55;
        col += vec3(0.45, 0.55, 0.72) * ground * rimGlow;
    }

    // ===== POST: chromatic aberration during flash =====
    if (flashAmt > 0.02) {
        vec2 ca = (uv - 0.5) * flashAmt * 0.012;
        // Subtle channel separation by sampling computed color through faux offsets:
        // since we can't resample the buffer, we apply a hue-shift approximation.
        col.r *= 1.0 + length(ca) * 6.0;
        col.b *= 1.0 + length(ca) * 6.0 * 0.7;
    }

    // ===== WHOLE-FRAME LINEAR FOLLOWER + DECAYING BEAT TRACE =====
    // Sub carries hiphop's sparse kicks; audioBeatPulse (decays ~300ms+)
    // leaves a visible tail between hits; linear bass/mid carry jazz's soft
    // accents and ambient swells. Silence multiplies by exactly 1.0.
    col *= 1.0 + 0.12 * bassP + 0.15 * subP + 0.08 * midP
               + 0.22 * audioBeatPulse * aR;

    // ===== POST: bloom approximation on bright pixels =====
    float bright = max(max(col.r, col.g), col.b);
    float bloom = smoothstep(0.85, 1.6, bright);
    col += col * bloom * 0.35;

    // ===== POST: film grain =====
    float grain = hash12(gl_FragCoord.xy + TIME * 60.0) - 0.5;
    col += vec3(grain) * 0.022;

    // ===== POST: vignette =====
    vec2 vc = uv - 0.5;
    float vig = 1.0 - dot(vc, vc) * 0.85;
    col *= vig;

    // ---- universal color block (defaults = no-op) ----
    float ucL = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(ucL), col, colorBoost);
    if (hueShift > 0.0005) {
        float hA = hueShift * 6.2831853;
        float hC = cos(hA), hS = sin(hA);
        mat3 hM = mat3(0.299,0.587,0.114, 0.299,0.587,0.114, 0.299,0.587,0.114)
                + hC * mat3(0.701,-0.587,-0.114, -0.299,0.413,-0.114, -0.300,-0.588,0.886)
                + hS * mat3(0.168,0.330,-0.497, -0.328,0.035,0.292, 1.250,-1.050,-0.203);
        col = clamp(hM * col, 0.0, 1.0);
    }
    col = clamp(col, 0.0, 1.8);
    gl_FragColor = vec4(col, 1.0);
}
