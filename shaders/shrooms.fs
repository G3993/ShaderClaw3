/*{
  "DESCRIPTION": "Shrooms — rows of soft, gradient-dyed mushroom stalks dancing in 3D. Each stalk is a bent, bulbous capsule smooth-blended into a rounded cap, swaying on its own spring with a phase lag along the row so waves of motion roll through the crowd; bass squashes and stretches them like a bounce, beats kick a wiggle that ripples down the line, mids widen the sway, highs shimmer the grain. Colour is a living gradient up each stalk (pale foot → glowing body → dark cap) that slowly flows and drifts. The scene morphs continuously between three looks: Aurora (a tall merged wall of drips in maroon, blue, cream, lavender and red), Ember Rows (three stacked tiers of slim dark-capped stalks fading red → pink → butter) and Cobalt Cluster (a few fat blue clubs fading to white on warm paper). Show dynamics: at Rest only a few stalks remain, short and drowsy, swaying slowly in soft colour with lots of paper around them; the Build fills the rows, stretches every stalk tall and thin, leans the crowd inward, speeds up the bounce and sends a shimmer band racing up the stems while the camera pushes in and the rows tremble; the Drop squashes the whole crowd flat and springs it back up with caps puffing out, the rows bursting outward and a bright flash, settling into a packed, saturated, wildly dancing peak.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "3D", "Nature", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "lookSel",     "LABEL": "Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Auto Cycle", "Aurora", "Ember Rows", "Cobalt Cluster"], "GROUP": "Look" },
    { "NAME": "lookPeriod",  "LABEL": "Look Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 6.0, "MAX": 90.0, "DEFAULT": 22.0, "GROUP": "Look" },
    { "NAME": "morphLen",    "LABEL": "Morph Length",    "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 0.9, "DEFAULT": 0.45, "GROUP": "Look" },
    { "NAME": "dance",       "LABEL": "Dance",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "wiggle",      "LABEL": "Wiggle",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "squash",      "LABEL": "Squash & Stretch","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "speed",       "LABEL": "Speed",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Motion" },
    { "NAME": "gradFlow",    "LABEL": "Gradient Flow",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "softness",    "LABEL": "Blend Softness",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Shape" },
    { "NAME": "thickness",   "LABEL": "Thickness",       "TYPE": "float", "MIN": 0.5, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Shape" },
    { "NAME": "capSize",     "LABEL": "Cap Size",        "TYPE": "float", "MIN": 0.5, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Shape" },
    { "NAME": "zoom",        "LABEL": "Zoom",            "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.8, "DEFAULT": 1.0, "GROUP": "Shape" },
    { "NAME": "useCustom",   "LABEL": "Custom Palette",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" },
    { "NAME": "pFoot",       "LABEL": "Foot",            "TYPE": "color", "DEFAULT": [1.0, 0.95, 0.62, 1.0], "GROUP": "Color" },
    { "NAME": "pLow",        "LABEL": "Lower Body",      "TYPE": "color", "DEFAULT": [1.0, 0.45, 0.88, 1.0], "GROUP": "Color" },
    { "NAME": "pMid",        "LABEL": "Body",            "TYPE": "color", "DEFAULT": [1.0, 0.2, 0.08, 1.0],  "GROUP": "Color" },
    { "NAME": "pHigh",       "LABEL": "Upper Body",      "TYPE": "color", "DEFAULT": [0.45, 0.05, 0.02, 1.0], "GROUP": "Color" },
    { "NAME": "pCap",        "LABEL": "Cap",             "TYPE": "color", "DEFAULT": [0.1, 0.03, 0.02, 1.0], "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",      "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "grain",       "LABEL": "Grain",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Look", "Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.86, 0.83, 0.8, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.78, 0.74, 0.71, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Shrooms — single-pass raymarch. Up to 3 tiers (rows going back and up),
// each a finite row of stalks evaluated by domain repetition (own cell ±1),
// smooth-unioned so neighbours melt into each other. All layout, shape and
// palette numbers are a "look" vector; looks are blended continuously.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

// ── Studio Palette: remap to black → accent → white (red or blue) ─────────
vec3 studioPalette(vec3 c) {
    float m = float(paletteMode);
    if (m < 0.5 || palAmount <= 0.0) return c;
    vec3 acc = m < 1.5 ? palRed.rgb : palBlue.rgb;
    float L = clamp(dot(c, vec3(0.2126, 0.7152, 0.0722)), 0.0, 1.0);
    float mx = max(c.r, max(c.g, c.b)), mn = min(c.r, min(c.g, c.b));
    float sat = mx > 1e-4 ? (mx - mn) / mx : 0.0;
    float t = pow(L, 0.8);
    vec3 ramp = t < 0.5 ? mix(vec3(0.0), acc, t * 2.0) : mix(acc, vec3(1.0), (t - 0.5) * 2.0);
    vec3 mono = vec3(smoothstep(0.0, 1.0, L));
    vec3 o = mix(mono, ramp, smoothstep(0.06, 0.4, sat));
    return mix(c, o, palAmount);
}

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
vec3 hueRot(vec3 c, float h) {
    if (h <= 0.0) return c;
    float a = h * TAU; vec3 k = vec3(0.57735); float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}

// ── Show dynamics: rest → build → peak / drop ───────────────────────────
float gRest, gBuild, gPeak, gDrop, gLift;
void showDynamics() {
    float m = float(showState);
    float e = clamp(audioEnergy, 0.0, 1.0);
    float b = clamp(audioBuildup, 0.0, 1.0);
    float d = clamp(audioDrop, 0.0, 1.0);
    if (m > 0.5) {                                   // manual override for the VJ
        if (m < 1.5)      { e = 0.04; b = 0.0; d = 0.0; }                                      // Rest
        else if (m < 2.5) { float r = fract(TIME / 8.0); e = 0.3 + 0.35 * r; b = r * r; d = 0.0; } // Build: 8 s riser, loops
        else              { e = 1.0; b = 0.0; d = exp(-fract(TIME / 4.0) * 5.0); } // Peak: hits every 4 s + beats
    }
    float k = dynamics;
    gBuild = b * k;                                                  // tension 0..1
    gDrop  = d * k;                                                  // impact impulse 0..1
    gPeak  = smoothstep(0.5, 0.9, e) * k;                            // sustained high section
    gRest  = (1.0 - smoothstep(0.08, 0.4, e)) * (1.0 - b) * k;       // quiet / breakdown
    gLift  = mix(0.5, clamp(0.15 + e * 0.7 + gBuild * 0.25 + d * 0.3, 0.0, 1.0), k);  // 0 rest … 1 peak; 0.5 when dynamics = 0
}

// look = layout + shape + palette
//  L0 = (tiers, stalks per row, spacing, stalk radius)
//  L1 = (stalk height, cap radius, cap flatness, blend k)
//  L2 = (tier depth step, tier rise, sway, camera height)
//  palette c0 foot … c4 cap, bg top / bottom
vec4 L0, L1, L2;
vec3 P0, P1, P2, P3, P4, BG1, BG2;

void lookParams(float i, out vec4 a, out vec4 b, out vec4 c, out vec3 c0, out vec3 c1, out vec3 c2, out vec3 c3, out vec3 c4, out vec3 g1, out vec3 g2) {
    if (i < 0.5) {          // Aurora
        a = vec4(1.0, 7.0, 0.62, 0.34);   b = vec4(3.4, 0.5, 0.8, 0.3);  c = vec4(0.0, 0.0, 0.45, 1.45);
        c0 = vec3(0.03, 0.05, 0.14); c1 = vec3(1.0, 0.24, 0.1); c2 = vec3(0.68, 0.55, 1.0); c3 = vec3(1.0, 0.96, 0.66); c4 = vec3(0.04, 0.36, 0.95);
        g1 = vec3(0.16, 0.02, 0.06); g2 = vec3(0.92, 0.8, 0.86);
    } else if (i < 1.5) {   // Ember Rows
        a = vec4(3.0, 12.0, 0.29, 0.125); b = vec4(1.3, 0.2, 0.95, 0.06); c = vec4(0.7, 1.12, 0.7, 1.65);
        c0 = vec3(1.0, 0.95, 0.62); c1 = vec3(1.0, 0.45, 0.88); c2 = vec3(1.0, 0.2, 0.08); c3 = vec3(0.45, 0.05, 0.02); c4 = vec3(0.1, 0.03, 0.02);
        g1 = vec3(0.86, 0.83, 0.8); g2 = vec3(0.8, 0.76, 0.73);
    } else {                // Cobalt Cluster
        a = vec4(2.0, 4.0, 0.56, 0.24);   b = vec4(2.3, 0.4, 0.75, 0.05); c = vec4(0.6, 0.62, 0.6, 1.4);
        c0 = vec3(0.93, 0.95, 1.0); c1 = vec3(0.8, 0.87, 1.0); c2 = vec3(0.42, 0.63, 1.0); c3 = vec3(0.1, 0.33, 0.82); c4 = vec3(0.05, 0.2, 0.62);
        g1 = vec3(0.88, 0.86, 0.84); g2 = vec3(0.82, 0.8, 0.78);
    }
}

float smin(float a, float b, float k, out float h) {
    h = clamp(0.5 + 0.5 * (b - a) / max(k, 1e-4), 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

float gT;          // animation time
float gKick;       // beat impulse
float gMatY;       // hit stalk height fraction
float gMatS;       // hit stalk seed
float gSp;         // stalk spacing (motion is measured in it)
float gStretch;    // show-state stalk stretch (camera framing follows it)

// one stalk in its local frame (base at origin)
float stalk(vec3 p, float seed, float xIdx, float tier, float w, out float yN) {
    float H = L1.x * (0.82 + 0.3 * h11(seed * 3.1));
    H *= smoothstep(0.12, 0.6, w);                 // edge stalks sprout from the ground instead of thinning into threads
    // squash & stretch bounce (volume preserving)
    float bounce = sin(gT * 2.1 + xIdx * 0.55 + tier * 1.3 + seed * 6.0);
    float st = 1.0 + squash * (0.06 * bounce + 0.16 * bassP() * audioReact * (0.6 + 0.4 * sin(xIdx * 1.7 + seed * 9.0)));
    // build: stalks stretch tall; rest: shorter, drowsy; drop: squash flat then spring back up
    float spring = gDrop * cos((1.0 - gDrop) * 9.0) ;            // damped spring as the impulse decays
    st *= gStretch - 0.32 * spring;
    st = max(st, 0.35);
    H *= st;
    float rs = L0.w * thickness * w / sqrt(st);
    float y = p.y;
    float yf = clamp(y / H, 0.0, 1.0);
    // dance: sway grows with height, waves travel along the row
    float ph = gT * 1.3 - xIdx * 0.45 + tier * 0.9 + seed * 6.0;
    float sway = L2.z * gSp * dance * (1.0 + 0.5 * midP() * audioReact) * (1.0 + 1.1 * (gLift - 0.5));
    vec2 off = sway * yf * yf * vec2(sin(ph), 0.5 * cos(ph * 0.8 + 1.0));
    // wiggle: travelling S-bend up the stalk + beat kick
    off.x += wiggle * 0.22 * gSp * sin(yf * 2.2 * PI - gT * 2.2 + seed * 10.0) * yf;
    off.x += gKick * 0.35 * gSp * sin(yf * 5.0 - xIdx) * yf;
    // build: the crowd leans in toward the centre; drop: bursts outward
    off.x += (-0.22 * gBuild + 0.5 * gDrop * gDrop) * gSp * clamp(xIdx * 0.35, -1.5, 1.5) * yf * yf;
    // build tremor near the top of the riser
    off.x += 0.06 * gSp * smoothstep(0.65, 1.0, gBuild) * sin(TIME * 63.0 + seed * 40.0) * yf;
    vec3 q = p - vec3(off.x, 0.0, off.y);
    // stem: capsule with a soft belly
    float belly = 1.0 + 0.22 * sin(yf * PI * 1.1 + seed * 4.0) + 0.12 * sin(yf * 7.0 + gT + seed * 7.0);
    float r = rs * belly;
    vec3 qs = q; qs.y -= clamp(qs.y, 0.0, H);
    float dStem = length(qs) - r;
    // cap: flattened ellipsoid at the top, a little offset for character
    float cr = L1.y * capSize * w * (0.85 + 0.3 * h11(seed * 7.7)) * (1.0 + 0.08 * gKick) * (1.0 + 0.15 * (gLift - 0.5) + 0.35 * gDrop * gDrop);
    vec3 qc = q - vec3(0.06 * sin(seed * 20.0) * cr, H, 0.0);
    qc.y /= L1.z;
    float dCap = (length(qc) - cr) * L1.z;
    float hh;
    float d = smin(dStem, dCap, 0.12 * cr + 0.02, hh);
    yN = clamp(q.y / (H + cr * L1.z), 0.0, 1.05);
    return d;
}

float map(vec3 p) {
    float d = 1e5;
    float mY = 0.0, mS = 0.0;
    for (int k = 0; k < 3; k++) {
        float tier = float(k);
        float tw = clamp(L0.x - tier, 0.0, 1.0);            // tier visibility (fades during morph)
        if (tw < 0.01) continue;
        vec3 pt = p - vec3(0.0, tier * L2.y, -tier * L2.x);
        float sp = L0.z; gSp = sp;
        float n = L0.y * (1.0 + 0.7 * (gLift - 0.5)) + 1.5 * gBuild;   // rest: sparse … peak: packed
        float shift = mod(tier, 2.0) * 0.5 * sp;
        float xi = floor((pt.x - shift) / sp + 0.5);
        for (int j = -2; j <= 2; j++) {
            float idx = xi + float(j);
            // finite row, soft-edged count
            float w = tw * (1.0 - smoothstep(n * 0.5 - 0.5, n * 0.5 + 0.3, abs(idx + 0.25)));
            if (w < 0.12) continue;
            float seed = h11(idx * 1.37 + tier * 7.1 + 0.3);
            vec3 q = pt - vec3(idx * sp + shift + (seed - 0.5) * sp * 0.35, 0.0, (h11(seed * 9.1) - 0.5) * 0.25);
            float yN;
            float ds = stalk(q, seed, idx, tier, w, yN);
            float hb;
            d = smin(ds, d, L1.w * softness + 0.002, hb);
            mY = mix(mY, yN, hb);
            mS = mix(mS, seed, hb);
        }
    }
    gMatY = mY; gMatS = mS;
    return d;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.005, -0.005);
    return normalize(e.xyy * map(p + e.xyy) + e.yyx * map(p + e.yyx) + e.yxy * map(p + e.yxy) + e.xxx * map(p + e.xxx));
}

vec3 palette(float t) {
    vec3 c0 = useCustom ? pFoot.rgb : P0;
    vec3 c1 = useCustom ? pLow.rgb  : P1;
    vec3 c2 = useCustom ? pMid.rgb  : P2;
    vec3 c3 = useCustom ? pHigh.rgb : P3;
    vec3 c4 = useCustom ? pCap.rgb  : P4;
    t = clamp(t, 0.0, 1.0);
    vec3 c = mix(c0, c1, smoothstep(0.0, 0.25, t));
    c = mix(c, c2, smoothstep(0.22, 0.5, t));
    c = mix(c, c3, smoothstep(0.5, 0.75, t));
    c = mix(c, c4, smoothstep(0.72, 0.95, t));
    return c;
}

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    vec3 a = BG1, b = BG2;
    if (m > 0.5 && m < 1.5) return bgColor.rgb;
    if (m > 0.5) { a = bgColor.rgb; b = bgColor2.rgb; }
    if (m > 2.5 && m < 3.5) {
        vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
        return mix(a, b, smoothstep(0.0, 0.85, length(dd)));
    }
    return mix(b, a, smoothstep(0.0, 1.0, uv.y));
}

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;

    // look selection + continuous morph
    showDynamics();
    float sel = float(lookSel);
    float la, lb, f;
    if (sel < 0.5) {
        float ph = TIME / lookPeriod;
        la = mod(floor(ph), 3.0); lb = mod(la + 1.0, 3.0);
        f = smoothstep(1.0 - morphLen, 1.0, fract(ph));
    } else { la = sel - 1.0; lb = la; f = 0.0; }
    vec4 a0, a1, a2, b0, b1, b2; vec3 p0, p1, p2, p3, p4, g1, g2, q0, q1, q2, q3, q4, k1, k2;
    lookParams(la, a0, a1, a2, p0, p1, p2, p3, p4, g1, g2);
    lookParams(lb, b0, b1, b2, q0, q1, q2, q3, q4, k1, k2);
    L0 = mix(a0, b0, f); L1 = mix(a1, b1, f); L2 = mix(a2, b2, f);
    P0 = mix(p0, q0, f); P1 = mix(p1, q1, f); P2 = mix(p2, q2, f); P3 = mix(p3, q3, f); P4 = mix(p4, q4, f);
    BG1 = mix(g1, k1, f); BG2 = mix(g2, k2, f);

    // continuous clocks: music clocks speed the dance up; build adds a bounded wind-up
    gT = TIME * speed * 0.8 + 0.4 * audioBassTime * ar * speed + 0.3 * audioMidTime * speed + 3.0 * gBuild * gBuild;
    gKick = audioBeatPulse * audioBeatPulse * ar * wiggle;

    // camera: long lens, nearly orthographic, looking at the crowd
    vec2 sp = (fc - 0.5 * R) / R.y;
    gStretch = 1.0 + 0.12 * (gLift - 0.5) + 0.28 * gBuild;
    float sceneH = L1.x * mix(1.0, gStretch, 0.85) + (L0.x - 1.0) * L2.y + L1.y * L1.z * 1.6;
    float camH = 0.5 * sceneH + (L2.w - 1.45) * 0.3;
    vec3 ro = vec3(0.0, camH, 14.0);
    vec3 ta = vec3(0.0, camH - 0.15, 0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = cross(uu, ww);
    float focal = 14.0 / (sceneH * 1.18) * zoom * (1.0 + 0.1 * (gLift - 0.5) + 0.1 * gBuild + 0.05 * gDrop);
    vec3 rd = normalize(sp.x * uu + sp.y * vv + focal * ww);

    vec3 bg = backdrop(uv);
    vec3 col = bg;
    float alpha = 0.0;

    float t = 10.0;
    bool hit = false;
    for (int i = 0; i < 150; i++) {
        vec3 p = ro + rd * t;
        float d = map(p);
        if (d < 0.0008 * t) { hit = true; break; }
        t += d * 0.6;
        if (t > 22.0) break;
    }
    if (hit) {
        vec3 p = ro + rd * t;
        vec3 n = calcNormal(p);
        map(p);
        float yN = gMatY, seed = gMatS;
        // living gradient: drifts up and down each stalk
        float flowT = yN + gradFlow * (0.07 * sin(gT * 0.7 + seed * 12.0 + p.x * 0.8) + 0.04 * sin(gT * 1.3 - p.y * 1.5));
        vec3 base = palette(flowT);
        vec3 L = normalize(vec3(-0.4, 0.6, 0.7));
        float diff = clamp(dot(n, L) * 0.5 + 0.5, 0.0, 1.0);
        float fres = pow(1.0 - clamp(dot(n, -rd), 0.0, 1.0), 2.5);
        // soft airbrushed shading: mostly gradient, gentle form
        col = base * (0.74 + 0.34 * diff);
        // inner glow on the body, darker silhouette edge on caps (like the references)
        col = mix(col, col * 0.55, fres * smoothstep(0.55, 0.9, yN) * 0.8);
        col += mix(base, vec3(1.0), 0.4) * fres * (1.0 - smoothstep(0.4, 0.8, yN)) * 0.22 * (1.0 + highP() * ar);
        col *= 1.0 + 0.15 * audioLevel * ar;
        // build: a shimmer band racing up the stems, its rate climbing
        float shim = pow(0.5 + 0.5 * sin(100.0 * gBuild * gBuild * gBuild - yN * 10.0 + seed * 6.0), 6.0) * gBuild;
        col += mix(base, vec3(1.0), 0.6) * shim * 0.4 * (1.0 - smoothstep(0.7, 0.95, yN));
        // drop: bright flash on the bodies + glowing rims
        col += mix(base, vec3(1.0), 0.5) * (gDrop * gDrop * 0.35 + fres * gDrop * 0.6);
        // rest: soft and a touch pale; peak: saturated
        col = mix(vec3(lum(col)), col, 1.0 + 0.5 * (gLift - 0.5));
        alpha = 1.0;
    }

    col = hueRot(col, hueShift);
    col = mix(vec3(lum(col)), col, saturation);
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * (0.08 + 0.04 * highP() * ar);
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    float outA = transparentBg ? alpha : 1.0;
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), outA);
}
