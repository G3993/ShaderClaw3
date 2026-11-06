/*{
  "CATEGORIES": [
    "Generator",
    "Cymatics",
    "3D",
    "Audio Reactive"
  ],
  "DESCRIPTION": "Chladni Figures v2: a real vibrating plate in a white studio. Pass 1 is a sand simulation. Grains are kicked off the vibrating antinodes and collect on the nodal lines. The flow follows the gradient of the time-averaged plate energy Σa²Z², and its divergence comes from the exact Laplacian eigen-identity, so sand piles up instead of being smeared. The music's spectrum decides which of 12 plate eigenmodes resonate: when the song changes, the sand visibly migrates into the new figure. Bass hits make the whole sand bed jump, and the plate flexes in slow motion. Pass 2 raymarches the plate, sand ridges, airborne grains, soft key shadows, studio reflections and the plate's shadow on a white seamless floor.",
  "INPUTS": [
    { "NAME": "mood", "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2],
      "LABELS": ["Black Plate · White Sand","Brass Plate · Fine Sand","White Plate · Iron Filings"] },
    { "NAME": "driveMode", "LABEL": "Drive", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1],
      "LABELS": ["Audio Resonance","Tone Sweep"] },

    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0, "MAX": 2, "DEFAULT": 1, "GROUP": "Audio Reactivity" },
    { "NAME": "bounce", "LABEL": "Bounce / Kick", "TYPE": "float", "MIN": 0, "MAX": 2, "DEFAULT": 1, "GROUP": "Audio Reactivity" },
    { "NAME": "plateFlex", "LABEL": "Plate Flex", "TYPE": "float", "MIN": 0, "MAX": 2, "DEFAULT": 1, "GROUP": "Audio Reactivity" },

    { "NAME": "sandSpeed", "LABEL": "Migration Speed", "TYPE": "float", "MIN": 0.1, "MAX": 3, "DEFAULT": 1, "GROUP": "Physics" },
    { "NAME": "sandAmount", "LABEL": "Sand Amount", "TYPE": "float", "MIN": 0.2, "MAX": 2, "DEFAULT": 1, "GROUP": "Physics" },
    { "NAME": "lineWidth", "LABEL": "Line Width", "TYPE": "float", "MIN": 0.4, "MAX": 3, "DEFAULT": 1.2, "GROUP": "Physics" },
    { "NAME": "sandHeight", "LABEL": "Pile Height", "TYPE": "float", "MIN": 0.2, "MAX": 2.5, "DEFAULT": 1, "GROUP": "Physics" },
    { "NAME": "morphPeriod", "LABEL": "Sweep Period (s)", "TYPE": "float", "MIN": 4, "MAX": 30, "DEFAULT": 11, "GROUP": "Physics" },

    { "NAME": "camTilt", "LABEL": "Camera Tilt", "TYPE": "float", "MIN": 20, "MAX": 90, "DEFAULT": 90, "GROUP": "Camera" },
    { "NAME": "camOrbit", "LABEL": "Camera Orbit", "TYPE": "float", "MIN": 0, "MAX": 6.283, "DEFAULT": 0, "GROUP": "Camera" },
    { "NAME": "orbitSpeed", "LABEL": "Orbit Speed", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0, "GROUP": "Camera" },
    { "NAME": "zoom", "LABEL": "Zoom", "TYPE": "float", "MIN": 0.6, "MAX": 2.2, "DEFAULT": 1, "GROUP": "Camera" },

    { "NAME": "lightAngle", "LABEL": "Light Angle", "TYPE": "float", "MIN": 0, "MAX": 6.283, "DEFAULT": 2.4, "GROUP": "Lighting" },
    { "NAME": "lightElevation", "LABEL": "Light Elevation", "TYPE": "float", "MIN": 0.15, "MAX": 1, "DEFAULT": 0.55, "GROUP": "Lighting" },
    { "NAME": "lightIntensity", "LABEL": "Light Intensity", "TYPE": "float", "MIN": 0, "MAX": 2, "DEFAULT": 1, "GROUP": "Lighting" },
    { "NAME": "lightSoftness", "LABEL": "Shadow Softness", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0.45, "GROUP": "Lighting" },
    { "NAME": "grainSparkle", "LABEL": "Grain Sparkle", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0.5, "GROUP": "Lighting" },

    { "NAME": "sandColor", "LABEL": "Sand Color", "TYPE": "color", "DEFAULT": [0.93, 0.90, 0.84, 1.0], "GROUP": "Color" },
    { "NAME": "plateTint", "LABEL": "Plate Tint", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0], "GROUP": "Color" },
    { "NAME": "studioColor", "LABEL": "Studio / Background", "TYPE": "color", "DEFAULT": [0.975, 0.97, 0.96, 1.0], "GROUP": "Color" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG", "TYPE": "bool", "DEFAULT": false, "GROUP": "Color" },
    { "NAME": "vignetteAmount", "LABEL": "Vignette", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0.06, "GROUP": "Color" },
    { "NAME": "brightness", "LABEL": "Brightness", "TYPE": "float", "MIN": 0, "MAX": 2, "DEFAULT": 1, "GROUP": "Color" },
    { "NAME": "contrast", "LABEL": "Contrast", "TYPE": "float", "MIN": 0, "MAX": 2, "DEFAULT": 1, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "sandBuf", "PERSISTENT": true, "FLOAT": true },
    {}
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  Chladni Figures v2 — sand physics on a resonating plate, rendered 3D
//
//  sandBuf (plate-UV space, persistent):
//    .r  sand density everywhere
//    .g  row 0, x<12  : smoothed drive amplitude of eigenmode i
//        row 0, x=12  : slow bass average     x=13 : kick envelope
//        row 0, x=14  : smoothed level
//
//  Eigenmodes of the square plate:  Z = sin(nπx)sin(mπy) ± sin(mπx)sin(nπy)
//  ∇²Z = −π²k²Z  (k² = n²+m²)  → the sand-flow divergence is analytic.
// ════════════════════════════════════════════════════════════════════════

#define PI 3.14159265359
#define NMODES 12
#define T_PLATE 0.05
#define FLOOR_Y 0.62
#define SLAB_TOP 0.07
#define SLAB_BOT -0.045

float hash21(vec2 p) { p = fract(p * vec2(233.34, 851.73)); p += dot(p, p + 23.45); return fract(p.x * p.y); }
float hash11(float n) { return fract(sin(n * 91.3458) * 47453.5453); }

// (n, m, sign) — sorted by k² so the spectrum maps bass→simple, treble→fine
vec3 modeNM(int i) {
    if (i == 0)  return vec3(1.0, 3.0, -1.0);
    if (i == 1)  return vec3(2.0, 3.0,  1.0);
    if (i == 2)  return vec3(1.0, 4.0,  1.0);
    if (i == 3)  return vec3(2.0, 5.0, -1.0);
    if (i == 4)  return vec3(3.0, 5.0,  1.0);
    if (i == 5)  return vec3(1.0, 6.0, -1.0);
    if (i == 6)  return vec3(4.0, 5.0, -1.0);
    if (i == 7)  return vec3(2.0, 7.0,  1.0);
    if (i == 8)  return vec3(4.0, 7.0, -1.0);
    if (i == 9)  return vec3(5.0, 8.0,  1.0);
    if (i == 10) return vec3(3.0, 10.0, -1.0);
    return              vec3(7.0, 11.0,  1.0);
}

// Z and ∇Z (plate uv 0..1)
vec3 chladni(vec2 p, vec3 nms) {
    float a = nms.x * PI, b = nms.y * PI, s = nms.z;
    float sax = sin(a * p.x), cax = cos(a * p.x);
    float sbx = sin(b * p.x), cbx = cos(b * p.x);
    float say = sin(a * p.y), cay = cos(a * p.y);
    float sby = sin(b * p.y), cby = cos(b * p.y);
    float Z  = sax * sby + s * sbx * say;
    float dx = a * cax * sby + s * b * cbx * say;
    float dy = b * sax * cby + s * a * sbx * cay;
    return vec3(Z, dx, dy);
}

vec2 px() { return 1.0 / RENDERSIZE; }
float stateAt(int i) {
    return texture2D(sandBuf, vec2((float(i) + 0.5) * px().x, 0.5 * px().y)).g;
}

// Pick the two strongest resonating modes from the smoothed drive bank.
// Returns indices in x,y and squared amplitudes (energy) in z,w.
vec4 topModes() {
    float w1 = -1.0, w2 = -1.0, i1 = 0.0, i2 = 1.0;
    for (int i = 0; i < NMODES; i++) {
        float w = stateAt(i);
        if (w > w1) { w2 = w1; i2 = i1; w1 = w; i1 = float(i); }
        else if (w > w2) { w2 = w; i2 = float(i); }
    }
    w1 = max(w1, 0.0); w2 = max(w2, 0.0);
    float s1 = pow(w1, 6.0), s2 = pow(w2, 6.0);
    float f1 = s1 / (s1 + s2 + 1e-6);
    float Etot = clamp(w1 * 1.3, 0.0, 1.4); Etot *= Etot;
    return vec4(i1, i2, Etot * f1, Etot * (1.0 - f1));
}

// Spectrum → drive target for mode i (plate resonance f ∝ k²)
float driveTarget(int i) {
    vec3 nm = modeNM(i);
    float k2 = nm.x * nm.x + nm.y * nm.y;
    float fi = float(i);
    float pos = mod(TIME / max(morphPeriod, 1.0), float(NMODES));
    float d = abs(fi - pos); d = min(d, float(NMODES) - d);
    float sweep = exp(-d * d * 3.0) * 0.8;
    if (int(driveMode + 0.5) == 1) return sweep;

    float ar = clamp(audioReact, 0.0, 2.0);
    float f = 45.0 * pow(k2 / 10.0, 1.25);
    float x = f / 22050.0;
    float fft = max(texture2D(audioFFT, vec2(x * 0.84, 0.5)).r,
                max(texture2D(audioFFT, vec2(x, 0.5)).r,
                    texture2D(audioFFT, vec2(x * 1.19, 0.5)).r));
    float tilt = pow(k2 / 10.0, 0.4);
    float band = (i < 4) ? audioBass : ((i < 8) ? audioMid : audioHigh);
    float a = (fft * tilt * 1.5 + band * 0.45) * ar;
    float lvl = clamp(audioLevel * ar * 2.0, 0.0, 1.0);
    return max(sweep * (0.65 - 0.4 * lvl), a);
}

// ───────────────────────────── PASS 0: sand sim ─────────────────────────────
vec4 simPass() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE;
    vec2 tx = px();
    float dt = clamp(TIMEDELTA, 0.001, 0.05);
    vec4 prev = texture2D(sandBuf, uv);
    int ix = int(floor(gl_FragCoord.x));
    int iy = int(floor(gl_FragCoord.y));
    float ar = clamp(audioReact, 0.0, 2.0);

    if (FRAMEINDEX < 2) {
        float n = hash21(gl_FragCoord.xy) * 0.5 + hash21(floor(gl_FragCoord.xy / 7.0)) * 0.5;
        return vec4((0.35 + 0.45 * n) * sandAmount, 0.0, 0.0, 1.0);
    }

    // ---- state bank (row 0) ----
    float g = prev.g;
    if (iy == 0 && ix < 15) {
        if (ix < NMODES) {
            float tgt = driveTarget(ix);
            float rate = tgt > g ? 3.5 : 0.7;
            g = mix(g, tgt, 1.0 - exp(-dt * rate));
        } else if (ix == 12) {
            g = mix(g, audioBass * ar, 1.0 - exp(-dt * 1.2));
        } else if (ix == 13) {
            float slow = stateAt(12);
            float k = clamp((audioBass * ar - slow - 0.05) * 3.5, 0.0, 1.0);
            g = max(g * exp(-dt * 4.5), k);
        } else {
            g = mix(g, audioLevel * ar, 1.0 - exp(-dt * 6.0));
        }
    }

    // ---- modal field ----
    vec4 tm = topModes();
    vec3 nm1 = modeNM(int(tm.x)), nm2 = modeNM(int(tm.y));
    vec3 z1 = chladni(uv, nm1), z2 = chladni(uv, nm2);
    float pk1 = PI * PI * (nm1.x * nm1.x + nm1.y * nm1.y);
    float pk2 = PI * PI * (nm2.x * nm2.x + nm2.y * nm2.y);
    float kick = stateAt(13);

    // grains drift down the gradient of time-averaged energy Σ a²Z²
    float c = 2.6 * sandSpeed;
    vec2 v = -c * (tm.z * z1.x * z1.yz / pk1 + tm.w * z2.x * z2.yz / pk2);
    float divv = -c * (tm.z * (dot(z1.yz, z1.yz) / pk1 - z1.x * z1.x)
                     + tm.w * (dot(z2.yz, z2.yz) / pk2 - z2.x * z2.x));
    // plus a tiny jitter: grains don't move as a fluid
    vec2 jit = (vec2(hash21(gl_FragCoord.xy + TIME * 61.0), hash21(gl_FragCoord.xy * 1.7 + TIME * 37.0)) - 0.5);
    float E = tm.z * z1.x * z1.x + tm.w * z2.x * z2.x;
    vec2 src = uv - v * dt + jit * tx * (0.6 + 2.0 * E);

    float Dmax = 2.4 * sandAmount;
    vec2 ro = max(vec2(lineWidth * 0.0011), tx);
    float Dc = texture2D(sandBuf, src).r;
    float blur = 0.25 * (texture2D(sandBuf, src + vec2( ro.x,  ro.y)).r
                       + texture2D(sandBuf, src + vec2(-ro.x,  ro.y)).r
                       + texture2D(sandBuf, src + vec2( ro.x, -ro.y)).r
                       + texture2D(sandBuf, src + vec2(-ro.x, -ro.y)).r);

    // agitation: bouncing grains scatter where the plate moves; kicks hit everything
    float agit = 0.35 * E + kick * clamp(bounce, 0.0, 2.0) * (0.25 + 0.6 * E);
    float alpha = clamp(0.09 + agit * 0.9, 0.0, 0.92);
    float D = mix(Dc, blur, alpha);

    float comp = clamp(1.0 - divv * dt, 0.6, 1.6);
    if (comp > 1.0) comp = 1.0 + (comp - 1.0) * max(0.0, 1.0 - D / Dmax);
    D *= comp;

    // steady trickle of fresh sand, and grains slowly lost off the rim
    D += dt * 0.03 * sandAmount * (0.4 + 1.2 * hash21(gl_FragCoord.xy + floor(TIME * 3.0)));
    vec2 edge = min(uv, 1.0 - uv);
    D *= mix(0.985, 1.0, smoothstep(0.0, 0.012, min(edge.x, edge.y)));
    D = clamp(D, 0.0, Dmax);
    return vec4(D, g, 0.0, 1.0);
}

// ───────────────────────────── PASS 1: render ─────────────────────────────
vec4 gTM; vec3 gNM1, gNM2; float gFlexAmp; float gKick; float gSandH;

float density(vec2 puv) { return texture2D(sandBuf, puv).r; }

float plateDisp(vec2 puv) {
    float z1 = chladni(puv, gNM1).x, z2 = chladni(puv, gNM2).x;
    float k1 = sqrt(gNM1.x * gNM1.x + gNM1.y * gNM1.y);
    float k2 = sqrt(gNM2.x * gNM2.x + gNM2.y * gNM2.y);
    return gFlexAmp * (sqrt(gTM.z) * z1 * sin(TIME * (4.0 + 0.35 * k1))
                     + sqrt(gTM.w) * z2 * sin(TIME * (4.0 + 0.35 * k2) + 1.3));
}
float sandH(float D) { return gSandH * (1.0 - exp(-1.3 * max(D - 0.22, 0.0))); }
float topH(vec2 puv) { return plateDisp(puv) + sandH(density(puv)); }

vec2 toPlate(vec3 p) { return p.xz * 0.5 + 0.5; }

float sdSquare(vec2 p, float b) { vec2 d = abs(p) - b; return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0); }

vec3 toLin(vec3 c) { return pow(max(c, 0.0), vec3(2.2)); }

// white seamless studio, used for reflections
vec3 envLight(vec3 r, vec3 L, vec3 bgL, float rough) {
    // white cyc wall on one side, open dark studio behind the camera
    float wall = smoothstep(-0.35, 0.65, dot(normalize(r.xz + 1e-4), normalize(L.xz + 1e-4)) * 0.5 + 0.5 * r.y);
    vec3 e = bgL * (0.08 + 0.92 * wall) * (0.6 + 0.4 * smoothstep(-0.1, 0.9, r.y));
    // straight up is the dark camera rig / ceiling, so an overhead view keeps the plate black
    e *= mix(1.0, 0.12, smoothstep(0.82, 0.98, r.y));
    float box = smoothstep(0.80 + 0.1 * (1.0 - rough), 0.995, dot(r, L));
    e += vec3(1.0, 0.98, 0.95) * box * 3.0 * lightIntensity;
    return e;
}

vec4 renderPass() {
    vec2 frag = gl_FragCoord.xy;
    vec2 res = RENDERSIZE;
    vec2 q = (frag - 0.5 * res) / res.y;

    gTM = topModes();
    gNM1 = modeNM(int(gTM.x)); gNM2 = modeNM(int(gTM.y));
    gKick = stateAt(13);
    float lvl = stateAt(14);
    gFlexAmp = 0.0016 * plateFlex * (0.35 + 1.2 * lvl + 2.2 * gKick);
    gSandH = 0.016 * sandHeight;

    // ---- camera ----
    float el = radians(clamp(camTilt, 5.0, 89.95));
    float az = camOrbit + TIME * orbitSpeed * 0.08;
    // straight overhead (tilt 90) the plate fills the frame edge to edge;
    // tilting down eases out to the full studio view
    float aspect = res.x / res.y;
    float fillDist = 3.8 / max(aspect, 1.0) * 0.97 + SLAB_TOP;
    float topness = smoothstep(70.0, 89.0, camTilt);
    float dist = mix(5.9, fillDist, topness) / zoom;
    vec3 ta = vec3(0.0, -0.12 * cos(el) * (1.0 - topness), 0.0);
    // camera bob on kicks — the table shakes a little
    vec3 ro = ta + dist * vec3(cos(el) * sin(az), sin(el), cos(el) * cos(az));
    vec3 fw = normalize(ta - ro);
    vec3 rt = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    if (abs(fw.y) > 0.999) rt = vec3(cos(az), 0.0, -sin(az));
    vec3 up = cross(rt, fw);
    vec3 rd = normalize(q.x * rt + q.y * up + 1.9 * fw);

    float lel = clamp(lightElevation, 0.1, 1.0) * PI * 0.5;
    vec3 L = normalize(vec3(cos(lightAngle) * cos(lel), sin(lel), sin(lightAngle) * cos(lel)));
    vec3 bgL = toLin(studioColor.rgb);
    vec3 sandL = toLin(sandColor.rgb);
    vec3 tint = toLin(plateTint.rgb);

    int moodI = int(mood + 0.5);
    vec3 plateAlb; float plateF0; float plateRough; vec3 grainAlb = sandL;
    if (moodI == 1) { plateAlb = vec3(0.50, 0.33, 0.12) * tint; plateF0 = 0.55; plateRough = 0.30; grainAlb = sandL * vec3(1.0, 0.96, 0.88); }
    else if (moodI == 2) { plateAlb = vec3(0.80, 0.80, 0.79) * tint; plateF0 = 0.04; plateRough = 0.45; grainAlb = vec3(0.035, 0.034, 0.033); }
    else { plateAlb = vec3(0.018, 0.018, 0.02) * tint; plateF0 = 0.05; plateRough = 0.25; }

    float softK = mix(40.0, 6.0, clamp(lightSoftness, 0.0, 1.0));
    vec3 col = bgL;
    float alphaOut = 1.0;
    float tHit = 1e9;

    // ---- plate slab: top surface (heightfield) or sides ----
    vec3 bmin = vec3(-1.0, -T_PLATE, -1.0), bmax = vec3(1.0, SLAB_TOP, 1.0);
    vec3 inv = 1.0 / rd;
    vec3 t0 = (bmin - ro) * inv, t1 = (bmax - ro) * inv;
    vec3 tmin = min(t0, t1), tmax = max(t0, t1);
    float tN = max(max(tmin.x, tmin.y), tmin.z);
    float tF = min(min(tmax.x, tmax.y), tmax.z);
    bool hitPlate = false; bool sideHit = false;
    vec3 P = vec3(0.0);
    if (tN < tF && tF > 0.0) {
        vec3 Pe = ro + rd * tN;
        if (tmin.y < max(tmin.x, tmin.z) && Pe.y < 0.0) {
            sideHit = true; hitPlate = true; P = Pe; tHit = tN;
        } else {
            // march the heightfield inside the thin slab around y=0
            float ts = tN;
            float te = min(tF, (SLAB_BOT - ro.y) / rd.y);
            float dtM = (te - ts) / 56.0;
            float tp = ts, tc = ts;
            bool found = false;
            for (int i = 0; i < 57; i++) {
                tc = ts + dtM * float(i);
                vec3 pp = ro + rd * tc;
                if (pp.y < topH(toPlate(pp))) { found = true; break; }
                tp = tc;
            }
            if (found) {
                for (int i = 0; i < 6; i++) {
                    float tm = 0.5 * (tp + tc);
                    vec3 pp = ro + rd * tm;
                    if (pp.y < topH(toPlate(pp))) tc = tm; else tp = tm;
                }
                hitPlate = true; tHit = tc; P = ro + rd * tc;
            } else if (te < tF - 1e-4) {
                // reached the bottom of the slab inside the plate footprint
                hitPlate = true; tHit = te; P = ro + rd * te;
            }
        }
    }

    // ---- airborne grains (bouncing off the antinodes / jumping on kicks) ----
    float cellS = 0.012;
    float hopDrive = clamp(bounce, 0.0, 2.0);
    float hopH = 0.055 * hopDrive * (0.25 + 1.3 * gKick + 0.6 * lvl);
    vec3 grainCol = vec3(0.0); float grainA = 0.0;
    if (hopDrive > 0.001) {
        float tMid = (0.5 * hopH - ro.y) / rd.y;
        vec2 cxz = (ro + rd * tMid).xz;
        vec2 cid0 = floor(cxz / cellS);
        for (int j = -2; j <= 2; j++)
        for (int i = -2; i <= 2; i++) {
            vec2 cid = cid0 + vec2(float(i), float(j));
            float h = hash21(cid);
            vec2 gxz = (cid + 0.2 + 0.6 * vec2(h, hash21(cid + 3.1))) * cellS;
            if (abs(gxz.x) > 1.0 || abs(gxz.y) > 1.0) continue;
            vec2 puv = gxz * 0.5 + 0.5;
            float Dg = density(puv);
            float z1 = chladni(puv, gNM1).x, z2 = chladni(puv, gNM2).x;
            float Eg = gTM.z * z1 * z1 + gTM.w * z2 * z2;
            float p = (clamp(Eg * 0.5, 0.0, 0.5) * (0.4 + lvl) + gKick * 0.55) * smoothstep(0.03, 0.5, Dg) * hopDrive;
            if (h > p) continue;
            float ph = fract(TIME * (1.6 + 1.3 * hash11(h * 77.0)) + h * 13.0);
            float gy = hopH * (0.3 + 0.7 * hash11(h * 31.0)) * 4.0 * ph * (1.0 - ph);
            vec3 G = vec3(gxz.x, gy + 0.004, gxz.y);
            float tg = dot(G - ro, rd);
            if (tg > tHit + 0.002) continue;
            float dg = length(ro + rd * tg - G);
            float rg = 0.0032;
            float a = 1.0 - smoothstep(rg * 0.6, rg, dg);
            if (a > grainA) {
                float lit = 0.55 + 0.45 * clamp(dot(normalize(ro + rd * tg - G + vec3(0.0, 0.001, 0.0)), L) * 0.5 + 0.5, 0.0, 1.0);
                grainCol = grainAlb * lit * (0.6 + 0.6 * lightIntensity);
                grainA = a;
            }
        }
    }

    if (hitPlate) {
        vec2 puv = toPlate(P);
        float D = density(puv);
        if (sideHit) {
            // anodized rim
            vec3 n = vec3(0.0);
            if (abs(P.x) > abs(P.z)) n.x = sign(P.x); else n.z = sign(P.z);
            float diff = max(dot(n, L), 0.0);
            vec3 rr = reflect(rd, n);
            float fr = plateF0 + (1.0 - plateF0) * pow(1.0 - max(dot(-rd, n), 0.0), 5.0);
            col = plateAlb * (0.25 * bgL + diff * lightIntensity * 1.1) + fr * envLight(rr, L, bgL, plateRough) * 0.6;
            col *= 0.9 + 0.1 * smoothstep(-T_PLATE, 0.0, P.y);
        } else {
            // normal from the heightfield
            float e = 1.6 / min(res.x, res.y);
            float hx = topH(puv + vec2(e, 0.0)) - topH(puv - vec2(e, 0.0));
            float hz = topH(puv + vec2(0.0, e)) - topH(puv - vec2(0.0, e));
            vec3 N = normalize(vec3(-hx / (4.0 * e), 1.0, -hz / (4.0 * e)));

            // grain-scale micro-normals, only where there is sand
            vec2 gc = puv * 520.0;
            vec2 gi = floor(gc), gf = fract(gc) - 0.5;
            float gh = hash21(gi);
            vec2 gn = (vec2(gh, hash21(gi + 5.3)) - 0.5);
            float cov = (D > 0.45) ? 1.0 : step(gh, pow(max(D - 0.04, 0.0) / 0.41, 2.0));
            float sandMask = clamp(cov * smoothstep(0.0, 0.08, D), 0.0, 1.0);
            vec3 Ns = normalize(N + vec3(gn.x + gf.x * 0.6, 0.0, gn.y + gf.y * 0.6) * 0.55 * sandMask);

            // soft key shadow — march the heightfield toward the light
            float sh = 1.0;
            float ts = 0.004;
            for (int i = 0; i < 18; i++) {
                vec3 sp = P + L * ts;
                if (sp.y > SLAB_TOP) break;
                if (abs(sp.x) > 1.0 || abs(sp.z) > 1.0) break;
                float dh = sp.y - topH(toPlate(sp));
                sh = min(sh, clamp(softK * dh / ts, 0.0, 1.0));
                ts += 0.006 + ts * 0.25;
            }
            sh = sh * sh * (3.0 - 2.0 * sh);

            // shadows of airborne grains
            if (hopDrive > 0.001) {
                vec2 cid0 = floor(P.xz / cellS);
                for (int j = -1; j <= 1; j++)
                for (int i = -1; i <= 1; i++) {
                    vec2 cid = cid0 + vec2(float(i), float(j));
                    float h = hash21(cid);
                    vec2 gxz = (cid + 0.2 + 0.6 * vec2(h, hash21(cid + 3.1))) * cellS;
                    vec2 guv = gxz * 0.5 + 0.5;
                    float Dg = density(guv);
                    float z1 = chladni(guv, gNM1).x, z2 = chladni(guv, gNM2).x;
                    float Eg = gTM.z * z1 * z1 + gTM.w * z2 * z2;
                    float p = (clamp(Eg * 0.5, 0.0, 0.5) * (0.4 + lvl) + gKick * 0.55) * smoothstep(0.03, 0.5, Dg) * hopDrive;
                    if (h > p) continue;
                    float ph = fract(TIME * (1.6 + 1.3 * hash11(h * 77.0)) + h * 13.0);
                    float gy = hopH * (0.3 + 0.7 * hash11(h * 31.0)) * 4.0 * ph * (1.0 - ph);
                    vec2 sxz = gxz - L.xz / L.y * gy;
                    float ds = length(P.xz - sxz);
                    sh *= mix(1.0, 0.35, 1.0 - smoothstep(0.003, 0.003 + gy * 0.15 * (0.3 + lightSoftness), ds));
                }
            }

            // cavity AO from the blurred sand field
            float ao = 1.0;
            {
                float rA = 0.012;
                float Db = 0.25 * (density(puv + vec2(rA, 0.0)) + density(puv - vec2(rA, 0.0))
                                 + density(puv + vec2(0.0, rA)) + density(puv - vec2(0.0, rA)));
                ao = clamp(1.0 - (sandH(Db) - sandH(D)) * 22.0 / sandHeight, 0.45, 1.0);
                ao *= 0.85 + 0.15 * smoothstep(0.0, 1.0, sandH(D) / gSandH);
            }

            float ndl = max(dot(Ns, L), 0.0);
            float hemi = 0.5 + 0.5 * Ns.y;
            float keyI = 1.25 * lightIntensity;

            // sand: rough diffuse with a little wrap (light bleeds through grains)
            float wrap = max((dot(Ns, L) + 0.3) / 1.3, 0.0);
            vec3 sandC = grainAlb * (keyI * mix(ndl, wrap, 0.35) * sh + 0.42 * hemi * ao) ;
            vec3 Hv = normalize(L - rd);
            float tw = 0.5 + 0.5 * sin(TIME * (3.0 + 9.0 * hash11(gh * 9.0)) + gh * 60.0);
            float sparkle = step(1.0 - 0.025 * grainSparkle * (1.0 + 2.0 * clamp(audioHigh * audioReact, 0.0, 1.0)), hash21(gi + 17.0))
                          * pow(max(dot(Ns, Hv), 0.0), 24.0) * sh * tw;
            sandC += vec3(1.0, 0.98, 0.94) * sparkle * 2.5 * lightIntensity * (moodI == 2 ? 0.4 : 1.0);

            // plate: dielectric/metal with studio reflections
            vec3 rr = reflect(rd, N);
            float nv = max(dot(-rd, N), 0.0);
            float fr = plateF0 + (1.0 - plateF0) * pow(1.0 - nv, 5.0);
            float a2 = plateRough * plateRough; a2 *= a2;
            float nh = max(dot(N, Hv), 0.0);
            float dGGX = a2 / (PI * pow(nh * nh * (a2 - 1.0) + 1.0, 2.0));
            vec3 specC = (moodI == 1) ? plateAlb * 1.6 : vec3(1.0);
            vec3 plateC = plateAlb * (1.0 - fr) * (keyI * ndl * sh + 0.42 * hemi * ao)
                        + specC * fr * envLight(rr, L, bgL, plateRough) * mix(0.55, 1.0, sh) * ao
                        + specC * dGGX * 0.12 * ndl * sh * keyI;
            // faint brushed texture on the plate
            plateC *= 0.96 + 0.04 * hash21(vec2(floor(puv.x * 900.0), floor(puv.y * 3.0)));

            col = mix(plateC, sandC, sandMask);

            // contact darkening where sand sits on the plate
            col *= mix(1.0, 0.82, (1.0 - sandMask) * smoothstep(0.02, 0.3, D));
        }
    } else {
        // ---- post + floor ----
        float tFloor = rd.y < 0.0 ? (-FLOOR_Y - ro.y) / rd.y : 1e9;
        // center post (the vibration driver shaft)
        float rp = 0.045;
        float bq = dot(ro.xz, rd.xz), aq = dot(rd.xz, rd.xz), cq = dot(ro.xz, ro.xz) - rp * rp;
        float disc = bq * bq - aq * cq;
        bool postHit = false;
        if (disc > 0.0) {
            float tc = (-bq - sqrt(disc)) / aq;
            vec3 pc = ro + rd * tc;
            if (tc > 0.0 && pc.y < -T_PLATE && pc.y > -FLOOR_Y && tc < tFloor) {
                postHit = true;
                vec3 n = normalize(vec3(pc.x, 0.0, pc.z));
                float diff = max(dot(n, L), 0.0);
                vec3 rr = reflect(rd, n);
                float fr = 0.6 + 0.4 * pow(1.0 - max(dot(-rd, n), 0.0), 5.0);
                float underPlate = smoothstep(-T_PLATE - 0.5, -T_PLATE, pc.y);
                col = vec3(0.03) * (0.3 * bgL + diff) + fr * envLight(rr, L, bgL, 0.35) * 0.35 * (1.0 - 0.6 * underPlate);
            }
        }
        if (!postHit && tFloor < 1e8) {
            vec3 F = ro + rd * tFloor;
            // plate shadow from the key (area light → penumbra grows with height)
            float tp = (0.0 - F.y) / L.y;
            vec2 Q = F.xz + L.xz * tp;
            float w = 0.03 + tp * (0.04 + 0.22 * lightSoftness);
            float shKey = smoothstep(-w, w, sdSquare(Q, 1.0));
            // post shadow
            vec2 dir = -L.xz / L.y;
            float sl = (FLOOR_Y - T_PLATE);
            float s = clamp(dot(F.xz, dir) / dot(dir, dir), 0.0, sl);
            float dps = length(F.xz - dir * s);
            shKey *= mix(1.0, smoothstep(rp * 0.7, rp + 0.02 + 0.05 * s * lightSoftness, dps), step(0.0, dot(F.xz, dir)));
            // sky occlusion under the plate
            float occ = 1.0 - smoothstep(-0.55, 0.65, sdSquare(F.xz, 1.0));
            occ += (1.0 - smoothstep(0.0, 0.18, length(F.xz))) * 0.5;
            float vis = mix(1.0, 0.45, clamp(occ, 0.0, 1.0)) * mix(0.3, 1.0, shKey);
            col = bgL * mix(1.0, vis, 0.85);
            alphaOut = (1.0 - vis) * 0.85;
        } else if (!postHit) {
            col = bgL;
            alphaOut = 0.0;
        }
    }

    col = mix(col, grainCol, grainA);
    alphaOut = max(alphaOut, grainA);

    // mild filmic shoulder on highlights only, keep the white studio white
    col = col / (1.0 + max(col - 1.0, 0.0));
    col = pow(max(col, 0.0), vec3(1.0 / 2.2));

    // tiny lens niceties
    vec2 uvn = frag / res;
    col *= 1.0 - vignetteAmount * dot(uvn - 0.5, uvn - 0.5) * 2.5;
    col += (hash21(frag + fract(TIME) * 100.0) - 0.5) * 0.008;

    col *= brightness;
    col = (col - 0.5) * contrast + 0.5;
    if (transparentBg) {
        // shadow-only floor: black at the shadow's alpha
        if (alphaOut < 0.999) col *= 0.0;
        return vec4(clamp(col, 0.0, 1.0), alphaOut);
    }
    return vec4(clamp(col, 0.0, 1.0), 1.0);
}

void main() {
    if (PASSINDEX == 0) gl_FragColor = simPass();
    else gl_FragColor = renderPass();
}
