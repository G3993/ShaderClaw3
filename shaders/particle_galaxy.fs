/*{
  "CATEGORIES": ["Generator", "Atmospheric", "Audio Reactive"],
  "DESCRIPTION": "Spiral galaxy disc with N rotating star particles plotted along log-spiral arms. Stars get blue-O type at the disc outskirts and yellow-G type near the bulge. Bass kicks ignite supernova flashes; treble drives nebular gas glow. Bulge is a hot warm-tinted core; halo is a faint diffuse Hubble-deep-field star field",
  "INPUTS": [
    { "NAME": "particleCount",   "LABEL": "Particles",        "TYPE": "float", "MIN": 50.0,  "MAX": 2000.0, "DEFAULT": 600.0 },
    { "NAME": "armCount",        "LABEL": "Arms",             "TYPE": "float", "MIN": 2.0,   "MAX": 4.0,    "DEFAULT": 2.0 },
    { "NAME": "spiralTightness", "LABEL": "Spiral Tightness", "TYPE": "float", "MIN": 0.10,  "MAX": 0.60,   "DEFAULT": 0.28 },
    { "NAME": "rotationSpeed",   "LABEL": "Rotation Speed",   "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,    "DEFAULT": 0.18 },
    { "NAME": "diskRadius",      "LABEL": "Disc Radius",      "TYPE": "float", "MIN": 0.20,  "MAX": 0.80,   "DEFAULT": 0.55 },
    { "NAME": "bulgeRadius",     "LABEL": "Bulge Radius",     "TYPE": "float", "MIN": 0.02,  "MAX": 0.30,   "DEFAULT": 0.10 },
    { "NAME": "bulgeBrightness", "LABEL": "Bulge Brightness", "TYPE": "float", "MIN": 0.0,   "MAX": 3.0,    "DEFAULT": 1.4 },
    { "NAME": "starSize",        "LABEL": "Star Size",        "TYPE": "float", "MIN": 0.5,   "MAX": 4.0,    "DEFAULT": 1.4 },
    { "NAME": "nebulaIntensity", "LABEL": "Nebula",           "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,    "DEFAULT": 0.85 },
    { "NAME": "haloDensity",     "LABEL": "Halo Stars",       "TYPE": "float", "MIN": 0.0,   "MAX": 1.0,    "DEFAULT": 0.55 },
    { "NAME": "supernovaProb",   "LABEL": "Supernova Prob",   "TYPE": "float", "MIN": 0.0,   "MAX": 0.20,   "DEFAULT": 0.04 },
    { "NAME": "hueRotate",       "LABEL": "Hue Rotate",       "TYPE": "float", "MIN": -1.0,  "MAX": 1.0,    "DEFAULT": 0.0 },
    { "NAME": "audioReact",      "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,   "MAX": 2.0,    "DEFAULT": 1.0 },
    { "NAME": "bgColor",         "LABEL": "Background",       "TYPE": "color", "DEFAULT": [0.01, 0.01, 0.025, 1.0] }
  ]
}*/

// ============================================================
// Particle Galaxy
// Logarithmic-spiral disc of N rotating star particles.
// Star colour graded by orbital radius (O-type outer -> G/K inner -> red bulge).
// Bulge: warm gaussian. Nebula: fbm masked by spiral arm density.
// Supernova: rare per-particle bass-driven flash. Halo: Hubble-deep-field stars.
// ============================================================

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
vec2  hash21(float n) { return fract(sin(vec2(n, n + 1.7)) * vec2(43758.5453, 22578.1459)); }
float hash21v(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21v(ip);
    float b = hash21v(ip + vec2(1.0, 0.0));
    float c = hash21v(ip + vec2(0.0, 1.0));
    float d = hash21v(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 r = mat2(0.8, -0.6, 0.6, 0.8);
    for (int i = 0; i < 5; i++) {
        v += a * vnoise(p);
        p = r * p * 2.07;
        a *= 0.5;
    }
    return v;
}

// Hue-rotate a colour by an angle in turns (-1..1 -> -tau..tau).
vec3 hueShift(vec3 c, float turns) {
    if (abs(turns) < 0.001) return c;
    float a = turns * 6.2831853;
    float ca = cos(a), sa = sin(a);
    // YIQ-ish quick rotation matrix.
    mat3 m = mat3(
        0.299 + 0.701 * ca + 0.168 * sa, 0.587 - 0.587 * ca + 0.330 * sa, 0.114 - 0.114 * ca - 0.497 * sa,
        0.299 - 0.299 * ca - 0.328 * sa, 0.587 + 0.413 * ca + 0.035 * sa, 0.114 - 0.114 * ca + 0.292 * sa,
        0.299 - 0.300 * ca + 1.250 * sa, 0.587 - 0.588 * ca - 1.050 * sa, 0.114 + 0.886 * ca - 0.203 * sa
    );
    return clamp(m * c, 0.0, 4.0);
}

// Colour by orbital radius t in [0,1] — outer blue (O type) -> white (F/G) ->
// yellow-orange (G/K) -> red giant bulge.
vec3 starColourByRadius(float t) {
    // Stops:
    //   t=0.00 deep red (bulge core)
    //   t=0.18 orange
    //   t=0.45 warm yellow-white
    //   t=0.70 white
    //   t=1.00 hot blue
    vec3 c;
    if (t < 0.18) {
        c = mix(vec3(1.00, 0.35, 0.20), vec3(1.00, 0.65, 0.30), t / 0.18);
    } else if (t < 0.45) {
        c = mix(vec3(1.00, 0.65, 0.30), vec3(1.00, 0.92, 0.78), (t - 0.18) / 0.27);
    } else if (t < 0.70) {
        c = mix(vec3(1.00, 0.92, 0.78), vec3(0.95, 0.97, 1.00), (t - 0.45) / 0.25);
    } else {
        c = mix(vec3(0.95, 0.97, 1.00), vec3(0.55, 0.75, 1.00), (t - 0.70) / 0.30);
    }
    return c;
}

void main() {
    vec2 res = RENDERSIZE.xy;
    vec2 uv  = (gl_FragCoord.xy - 0.5 * res) / min(res.x, res.y);
    // 'uv' is centred, isotropic, ~[-0.5, 0.5] short edge.

    float r  = length(uv);
    float th = atan(uv.y, uv.x);

    vec3 col = bgColor.rgb;

    // ---- Halo deep-field stars (behind everything) ----
    if (haloDensity > 0.0) {
        for (int s = 0; s < 2; s++) {
            float dens = (s == 0) ? 220.0 : 520.0;
            vec2 g = floor(gl_FragCoord.xy / res.y * dens);
            float h = hash21v(g);
            float thr = mix(0.997, 0.990, haloDensity);
            if (h > thr) {
                vec2 fp = fract(gl_FragCoord.xy / res.y * dens) - 0.5;
                float bright = exp(-dot(fp, fp) * 90.0);
                float twinkle = 0.6 + 0.4 * sin(TIME * (0.7 + h * 3.5) + h * 27.0);
                vec3 tint = mix(vec3(0.85, 0.90, 1.0), vec3(1.0, 0.90, 0.78), hash11(h * 7.1));
                col += tint * bright * twinkle * (s == 0 ? 0.55 : 0.30);
            }
        }
    }

    // ---- Spiral arm density field (used for nebula + reference) ----
    // For a log spiral r = a*exp(b*theta), an arm passes through (r,theta)
    // when theta - log(r)/b == 2*pi*k/armCount.
    float armN = clamp(armCount, 2.0, 4.0);
    float b = max(spiralTightness, 0.08);
    float baseRot = TIME * rotationSpeed * 0.4;
    float armPhase = th - log(max(r, 1e-3)) / b - baseRot;
    float armCos = cos(armPhase * armN);                   // [-1,1]
    float armDensity = 0.5 + 0.5 * armCos;                 // [0,1]
    // Falloff of disc.
    float discMask = smoothstep(diskRadius, bulgeRadius * 1.2, r) * 0.0
                   + smoothstep(diskRadius, diskRadius * 0.55, r);

    // ---- Nebular gas (fbm masked by arm density) ----
    if (nebulaIntensity > 0.0) {
        vec2 nq = uv * 4.5;
        // Co-rotate noise slowly with the disc.
        float ca = cos(baseRot * 0.5), sa = sin(baseRot * 0.5);
        nq = mat2(ca, -sa, sa, ca) * nq;
        float n = fbm(nq + vec2(TIME * 0.03, -TIME * 0.02));
        n = pow(n, 1.4);
        float armBoost = pow(armDensity, 2.5);
        float nebMask = n * armBoost * discMask;
        // Two nebula tints: pinkish HII + cool blue reflection.
        vec3 hii  = vec3(0.95, 0.35, 0.55);
        vec3 refl = vec3(0.30, 0.55, 1.00);
        float mixT = 0.5 + 0.5 * sin(armPhase * 1.7 + TIME * 0.1);
        vec3 nebCol = mix(refl, hii, mixT);
        float trebleBoost = 1.0 + audioHigh * audioReact * 1.2;
        col += nebCol * nebMask * nebulaIntensity * 0.55 * trebleBoost;
    }

    // ---- Star particles ----
    int N = int(clamp(particleCount, 50.0, 2000.0));
    float invMin = 1.0 / min(res.x, res.y);
    float pixSize = starSize * 1.6 * invMin;     // base star radius in uv units
    float pixSize2 = pixSize * pixSize;

    for (int i = 0; i < 2000; i++) {
        if (i >= N) break;
        float fi = float(i);

        // Per-particle hashes.
        vec2  h2 = hash21(fi * 1.31 + 7.7);
        float h3 = hash11(fi * 2.71 + 3.3);
        float h4 = hash11(fi * 5.13 + 11.1);

        // Radial distribution biased toward centre (sqrt -> uniform area;
        // pow > 0.5 -> denser bulge).
        float rNorm = pow(h2.x, 0.85);
        float ri = rNorm * diskRadius;

        // Pick which arm this star belongs to.
        float armIdx = floor(h2.y * armN);
        float armOffset = armIdx * (6.2831853 / armN);

        // Spiral baseline angle: theta = log(r)/b + arm offset.
        float thetaSpiral = log(max(ri, 1e-3)) / b + armOffset;

        // Small per-star scatter across the arm so it's a band, not a wire.
        float scatter = (h3 - 0.5) * 0.55;
        thetaSpiral += scatter;

        // Differential angular velocity — Kepler-ish: omega ~ 1/sqrt(r).
        // Inner stars spin faster. Cap to avoid singularity at r->0.
        float omega = rotationSpeed * 1.4 / sqrt(max(ri, 0.05));
        float thetaI = thetaSpiral + TIME * omega;

        // Tiny orbital wobble for life.
        thetaI += sin(TIME * (0.5 + h4 * 1.5) + fi * 0.7) * 0.015;

        vec2 pos = vec2(cos(thetaI), sin(thetaI)) * ri;
        vec2 d = uv - pos;
        float d2 = dot(d, d);

        // Quick reject — particles far outside this pixel's reach.
        float reach = pixSize2 * 25.0;
        if (d2 > reach) continue;

        // Per-star size jitter.
        float sizeJ = 0.6 + h4 * 0.9;
        float sigma2 = pixSize2 * sizeJ * sizeJ;
        float blob = exp(-d2 / (sigma2 * 1.2));

        // Twinkle.
        float tw = 0.7 + 0.3 * sin(TIME * (1.0 + h3 * 3.0) + fi * 1.3);

        // Colour by orbital radius (normalised within disc).
        float radT = clamp(ri / max(diskRadius, 1e-3), 0.0, 1.0);
        vec3 sc = starColourByRadius(radT);

        // Brightness — inner stars a touch brighter, fades at disc edge.
        float bright = blob * tw
                     * (0.55 + 0.7 * (1.0 - radT))
                     * smoothstep(diskRadius * 1.05, diskRadius * 0.85, ri);

        // Supernova: rare per-star flash on bass.
        // Uses a TIME-windowed hash so different stars trigger at different bursts.
        float bassDrive = audioBass * audioReact;
        float window = floor(TIME * 1.7);
        float flashH = hash11(fi * 9.91 + window * 13.37);
        float threshold = 1.0 - clamp(supernovaProb, 0.0, 0.2) * (0.5 + bassDrive * 1.5);
        if (flashH > threshold) {
            float burst = smoothstep(0.0, 0.25, fract(TIME * 1.7))
                        * smoothstep(1.0,  0.4, fract(TIME * 1.7));
            // Bigger soft halo with cyan-white core.
            float halo = exp(-d2 / (sigma2 * 18.0));
            vec3 snCol = vec3(1.0, 0.95, 0.85);
            bright += halo * burst * 1.3;
            sc = mix(sc, snCol, 0.7);
        }

        col += sc * bright * 1.6;
    }

    // ---- Bulge core glow (warm gaussian) ----
    if (bulgeBrightness > 0.0) {
        float bg = exp(-(r * r) / (bulgeRadius * bulgeRadius * 0.85));
        // Soft outer halo around the bulge.
        float bgOuter = exp(-(r * r) / (bulgeRadius * bulgeRadius * 4.5)) * 0.35;
        vec3 bulgeCol = vec3(1.00, 0.78, 0.55);
        // Audio bass pulse on the core.
        float pulse = 1.0 + audioBass * audioReact * 0.5;
        col += bulgeCol * (bg + bgOuter) * bulgeBrightness * pulse;
    }

    // ---- Disc glow underlay (so arms have some diffuse light) ----
    float discGlow = exp(-r * r * 6.0) * 0.25;
    col += vec3(0.55, 0.45, 0.75) * discGlow * 0.6;

    // ---- Hue rotate global ----
    col = hueShift(col, hueRotate);

    // ---- Atmospheric grain ----
    col += (hash21v(gl_FragCoord.xy + TIME) - 0.5) * 0.010;

    gl_FragColor = vec4(col, 1.0);
}
