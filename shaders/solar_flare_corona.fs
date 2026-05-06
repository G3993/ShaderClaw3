/*{
  "CATEGORIES": ["Generator", "Astronomy", "Minimal"],
  "DESCRIPTION": "Minimal sun + eclipse + sunset composition. A single contemplative disc on a graded sky — coral horizon below, deep navy above. A moon-like silhouette drifts across the sun, occasionally producing a total eclipse with a thin gold corona ring. No flares, no spicules, no chaos. Calm, contemplative. Linear HDR.",
  "INPUTS": [
    { "NAME": "mood",             "LABEL": "Mood",              "TYPE": "long",  "DEFAULT": 0,
      "VALUES": [0, 1, 2, 3],
      "LABELS": ["Total Eclipse", "Sunset Disc", "Diamond Ring", "Minimal Disc"] },
    { "NAME": "eclipseAmount",    "LABEL": "Eclipse Amount",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.50 },
    { "NAME": "sunSize",          "LABEL": "Sun Size",          "TYPE": "float", "MIN": 0.10, "MAX": 0.40, "DEFAULT": 0.22 },
    { "NAME": "coronaIntensity",  "LABEL": "Corona Intensity",  "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.00 },
    { "NAME": "audioReact",       "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.50 },
    { "NAME": "skyShift",         "LABEL": "Sky Shift",         "TYPE": "float", "MIN": -0.50, "MAX": 0.50, "DEFAULT": 0.00 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  Minimal Sun + Eclipse + Sunset
//  References: total solar eclipse photographs (Bailly's beads, diamond ring),
//  Hiroshi Sugimoto seascapes, James Turrell skyspaces, Rothko horizons.
//
//  Composition (back to front):
//    1) graded sky   — navy top, coral/peach bottom, subtle horizon line
//    2) atmospheric haze near the horizon
//    3) corona ring  — only visible during eclipse (gold HDR halo)
//    4) sun disc     — bone-white HDR, soft limb
//    5) moon disc    — deep silhouette occluding sun (drift across)
//    6) diamond bead — single bright bead at the eclipse edge in DR mood
// ════════════════════════════════════════════════════════════════════════

// ─── palette (linear HDR) ─────────────────────────────────────────────
const vec3 NAVY        = vec3(0.040, 0.050, 0.120);
const vec3 NAVY_DEEP   = vec3(0.020, 0.025, 0.075);
const vec3 CORAL       = vec3(1.000, 0.400, 0.250);
const vec3 PEACH       = vec3(1.000, 0.700, 0.450);
const vec3 GOLD_CORONA = vec3(2.500, 1.600, 0.800); // HDR
const vec3 SUN_BONE    = vec3(2.800, 2.600, 2.000); // HDR
const vec3 MOON_SHADE  = vec3(0.012, 0.018, 0.045);

// ─── small utilities ──────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

// Smooth disc mask (1 inside, 0 outside, AA edge)
float discMask(vec2 p, vec2 c, float r) {
    float d = length(p - c);
    float aa = fwidth(d) + 1e-4;
    return 1.0 - smoothstep(r - aa, r + aa, d);
}

// ─── main ─────────────────────────────────────────────────────────────
void main() {
    vec2 uv  = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p   = (uv - 0.5) * vec2(aspect, 1.0);

    float t      = TIME;
    float audio  = clamp(audioReact, 0.0, 2.0);
    // synthesize a slow breath if audio is silent
    float breath = (0.5 + 0.5 * sin(t * 0.31)) * audio * 0.35;

    int   moodI    = int(mood + 0.5);
    float sunR     = clamp(sunSize, 0.10, 0.40);
    float moonR    = sunR * 1.02; // a touch larger so totality is clean

    // ── sun position: slightly above horizon, breathing ───────────────
    float sunY = -0.05 + 0.02 * sin(t * 0.13);
    vec2  sunC = vec2(0.05 + 0.04 * sin(t * 0.07), sunY);

    // ── moon drift: slow horizontal sweep modulated by eclipseAmount ──
    // eclipseAmount=0 -> moon parked far off-screen (no eclipse).
    // eclipseAmount=1 -> moon centred on sun (totality).
    float driftPhase = sin(t * 0.05) * 0.5 + 0.5; // 0..1
    float baseOffset = mix(2.0, 0.0, clamp(eclipseAmount, 0.0, 1.0));
    // gentle vertical wobble so it never feels mechanical
    vec2  moonC = sunC + vec2((driftPhase - 0.5) * 0.06 + baseOffset,
                              0.005 * sin(t * 0.09));

    // mood overrides:
    //   0 Total Eclipse   -> moon centered, eclipseAmount snapped to ~1
    //   1 Sunset Disc     -> moon parked far off, no eclipse
    //   2 Diamond Ring    -> moon offset slightly so a bead leaks at the edge
    //   3 Minimal Disc    -> sky flattened to navy, no moon
    float ecl = clamp(eclipseAmount, 0.0, 1.0);
    if (moodI == 0) { moonC = sunC; ecl = 1.0; }
    if (moodI == 1) { moonC = sunC + vec2(3.0, 0.0); ecl = 0.0; }
    if (moodI == 2) { moonC = sunC + vec2(sunR * 0.18, sunR * 0.10); ecl = 0.95; }
    if (moodI == 3) { moonC = sunC + vec2(3.0, 0.0); ecl = 0.0; }

    // ── 1) sky gradient ───────────────────────────────────────────────
    float skyT = clamp(uv.y + skyShift, 0.0, 1.0);
    // top -> NAVY_DEEP, mid -> NAVY, low -> CORAL, very low -> PEACH (warm)
    vec3 sky;
    if (moodI == 3) {
        // Minimal Disc: pure navy field, barely graded
        sky = mix(NAVY_DEEP, NAVY, smoothstep(0.0, 1.0, skyT));
    } else {
        vec3 lower  = mix(PEACH, CORAL, smoothstep(0.05, 0.30, skyT));
        vec3 upper  = mix(NAVY,  NAVY_DEEP, smoothstep(0.55, 1.00, skyT));
        sky         = mix(lower, upper, smoothstep(0.30, 0.55, skyT));
    }

    // ── 2) horizon line + atmospheric haze ────────────────────────────
    float horizonY = 0.18 + skyShift * 0.20;
    float horizonD = abs(uv.y - horizonY);
    float horizonLine = exp(-horizonD * 380.0) * 0.12;
    if (moodI != 3) sky += CORAL * horizonLine;

    // soft haze band hugging the horizon
    float haze = exp(-pow((uv.y - horizonY) * 6.0, 2.0));
    if (moodI != 3) sky = mix(sky, sky + PEACH * 0.12, haze * 0.6);

    vec3 col = sky;

    // ── 3) corona ring (thin gold halo around the sun) ────────────────
    // Always present at low intensity; bright during eclipse.
    float rSun = length(p - sunC);
    {
        // outer falloff
        float outer = exp(-pow(max(rSun - sunR, 0.0) / (sunR * 0.65), 2.0) * 5.0);
        // inner ring spike right at the limb
        float ring  = exp(-pow((rSun - sunR * 1.02) / (sunR * 0.06), 2.0));

        // Gentle radial filaments — very subtle, no chaos
        float ang   = atan(p.y - sunC.y, p.x - sunC.x);
        float ripple = 0.85 + 0.15 * vnoise(vec2(ang * 3.0, t * 0.05));

        float coronaAmt = (0.20 + 0.80 * ecl) * coronaIntensity * (1.0 + breath);
        col += GOLD_CORONA * outer * 0.45 * coronaAmt * ripple;
        col += GOLD_CORONA * ring  * 0.90 * coronaAmt * ripple;
    }

    // ── 4) sun disc ───────────────────────────────────────────────────
    float sunMask = discMask(p, sunC, sunR);
    if (sunMask > 0.0) {
        // soft bone-white interior, slightly warmer toward the limb
        float rN = clamp(rSun / sunR, 0.0, 1.0);
        float limb = 0.85 + 0.15 * sqrt(max(1.0 - rN * rN, 0.0));
        vec3  sunCol = SUN_BONE * limb;
        // slight peach bleed on the lower edge (sunset feel)
        sunCol = mix(sunCol, sunCol * 0.85 + PEACH * 0.55, smoothstep(0.0, 1.0, (sunC.y - p.y) / sunR * 0.5 + 0.5) * 0.25);

        // dim the sun proportionally to eclipse coverage so it feels swallowed
        float dim = 1.0 - 0.55 * ecl;
        col = mix(col, sunCol * dim, sunMask);
    }

    // ── 5) moon disc (silhouette) ─────────────────────────────────────
    float moonMask = discMask(p, moonC, moonR);
    if (moonMask > 0.0) {
        // pure shadow with a barely-lit earthshine tint
        vec3 moonCol = MOON_SHADE;
        col = mix(col, moonCol, moonMask);
    }

    // ── 6) diamond-ring bead (only DR mood, or near-total eclipse) ────
    {
        float beadStrength = 0.0;
        if (moodI == 2) {
            beadStrength = 1.0;
        } else if (ecl > 0.92 && ecl < 0.995 && moodI == 0) {
            beadStrength = smoothstep(0.92, 0.97, ecl) * smoothstep(0.995, 0.97, ecl);
        }
        if (beadStrength > 0.0) {
            // place bead at the angle from moon-centre to sun-centre, on the limb
            vec2 dir = sunC - moonC;
            float dl = length(dir) + 1e-5;
            vec2 beadP = moonC + (dir / dl) * moonR;
            float bd  = length(p - beadP);
            float bead = exp(-pow(bd / (sunR * 0.05), 2.0));
            col += SUN_BONE * bead * beadStrength * 1.20;
            // small flare flicker from the bead
            float flare = exp(-pow(bd / (sunR * 0.18), 2.0)) * 0.35;
            col += GOLD_CORONA * flare * beadStrength;
        }
    }

    // ── final: very subtle dither so deep navy isn't banded ───────────
    col += (hash21(uv * RENDERSIZE.xy + t) - 0.5) * 0.004;

    gl_FragColor = vec4(col, 1.0);
}
