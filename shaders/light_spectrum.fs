/*{
  "DESCRIPTION": "Light Spectrum — an abstract painting made of light passing through glass. Overlapping translucent panes and shards of dichroic, frosted and reeded glass float at different depths over a luminous white light table; their edges are brushed and painterly, the far panes melt into soft defocus while the near ones stay crisp, and every edge splits into a fine prismatic rainbow fringe. Colour only appears where light comes through the glass — pale lavender, aqua, peach, rose, soft lemon, mint and icy blue, deepening where panes overlap — while rippling caustics, a slow prism wash and long brushstroke sweeps of refracted white light are painted across the scene and leave soft long-exposure trails, all wrapped in a gentle bloom. It flows through four looks: Dichroic Panes (crisp overlapping colour-shifting shards), Light Painting (sweeping brushstrokes of light behind faint glass), Prism Wash (big frosted defocused panes in a drifting spectral wash and strong caustics) and Reeded Glass (the whole scene seen through ribbed glass that slices and repeats the light). Bass swells the light and the glowing light cores, every beat sweeps a bright stroke of light across the glass, mids slide the panes, and highs throw sparkling glints along the glass edges. Rest: a few pale panes and one slow faint stroke on a calm white field; Build: the strokes speed up, multiply and brighten while the colour climbs; Drop: a burst of white light floods through all the glass, edges flaring — settling into full colour, many panes and fast flowing strokes at the Peak.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original. Restyled 2026-10-26 (glass & light).",
  "CATEGORIES": ["Generator", "Light", "Abstract", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Glass Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Dichroic Panes", "Light Painting", "Prism Wash", "Reeded Glass"], "GROUP": "Glass" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 14.0, "GROUP": "Glass" },
    { "NAME": "paneCount",   "LABEL": "Glass Panes",      "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 7.0, "DEFAULT": 6.0, "GROUP": "Glass" },
    { "NAME": "paneSize",    "LABEL": "Pane Size",        "TYPE": "float", "MIN": 0.4, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Glass" },
    { "NAME": "glassColour", "LABEL": "Glass Colour",     "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Glass" },
    { "NAME": "painterly",   "LABEL": "Painterly Edges",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Glass" },
    { "NAME": "frost",       "LABEL": "Frosted Glass",    "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.6, "GROUP": "Glass" },
    { "NAME": "reeds",       "LABEL": "Reeded Glass",     "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Glass" },
    { "NAME": "depthBlur",   "LABEL": "Depth Defocus",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Glass" },
    { "NAME": "speed",       "LABEL": "Speed",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Glass" },
    { "NAME": "strokes",     "LABEL": "Light Strokes",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "strokeWidth", "LABEL": "Stroke Width",     "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "dispersion",  "LABEL": "Prism Dispersion", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "caustics",    "LABEL": "Caustics",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "edgeGlow",    "LABEL": "Edge Light",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "exposure",    "LABEL": "Exposure",         "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "trails",      "LABEL": "Light Trails",     "TYPE": "float", "MIN": 0.0, "MAX": 0.97, "DEFAULT": 0.85, "GROUP": "Motion" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Atmosphere" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Atmosphere" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 3, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Light Table", "TYPE": "long", "DEFAULT": 2, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [1.0, 0.985, 0.97, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.86, 0.87, 0.95, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "PASSES": [
    { "TARGET": "light", "PERSISTENT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Light Spectrum — painting with glass and light
//   pass 0  light (persistent, HDR): light table (backdrop) seen through a
//           stack of glass panes, front-to-back. Each pane multiplies a
//           pastel transmission tint, bends the view behind it (refraction /
//           reeds), and emits prism-split edge light. Behind the glass:
//           brushstroke light sweeps, caustics, prism wash, light cores.
//           Only HDR light (> the white table) leaves long-exposure trails.
//   pass 1  final: soft bloom, highlight shoulder, palette, grain, dither.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

// ── Studio Palette: remap to black → accent → white (red or blue) ─────────
vec3 studioPalette(vec3 c) {
    float m = float(paletteMode);
    if (palAmount <= 0.0) return c;
    float L = clamp(dot(c, vec3(0.2126, 0.7152, 0.0722)), 0.0, 1.0);
    float mx = max(c.r, max(c.g, c.b)), mn = min(c.r, min(c.g, c.b));
    float sat = mx > 1e-4 ? (mx - mn) / mx : 0.0;
    vec3 o;
    if (m < 0.5) {
        // Colorful: saturation pushed hard toward neon / primary / poppy colour.
        vec3 hueC = pow((c - mn) / max(mx - mn, 1e-4), vec3(1.4));   // pure hue, nudged toward primaries
        float s2 = 1.0 - pow(1.0 - sat, 4.0);                         // strong but curved saturation boost
        float v = clamp(pow(mx, 0.85) * 1.12, 0.0, 1.0);
        o = mix(vec3(1.0), hueC, s2) * v;
        // flat mid-greys pick up a drifting neon spectrum keyed to brightness
        float hx = L * 1.7 + 0.04 * TIME;
        vec3 neon = pow(clamp(abs(fract(hx + vec3(0.0, 0.6667, 0.3333)) * 6.0 - 3.0) - 1.0, 0.0, 1.0), vec3(1.4));
        float wn = (1.0 - smoothstep(0.02, 0.1, sat)) * smoothstep(0.06, 0.25, L) * (1.0 - smoothstep(0.55, 0.85, L));
        o = mix(o, neon * v, wn);
        o = mix(o, vec3(1.0), smoothstep(0.85, 1.0, L) * 0.5);       // white-hot cores
    } else if (m > 2.5) {
        o = vec3(smoothstep(0.02, 0.95, pow(L, 0.9)));             // Black & White
    } else {
        vec3 acc = m < 1.5 ? palRed.rgb : palBlue.rgb;
        float t = pow(L, 0.8);
        vec3 ramp = t < 0.5 ? mix(vec3(0.0), acc, t * 2.0) : mix(acc, vec3(1.0), (t - 0.5) * 2.0);
        vec3 mono = vec3(smoothstep(0.0, 1.0, L));
        o = mix(mono, ramp, smoothstep(0.06, 0.4, sat));
    }
    return mix(c, o, palAmount);
}

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vn(vec2 x) {
    vec2 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i), h12(i + vec2(1, 0)), f.x), mix(h12(i + vec2(0, 1)), h12(i + vec2(1, 1)), f.x), f.y);
}
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
vec3 hueRot(vec3 c, float h) {
    if (h <= 0.0) return c;
    float a = h * TAU; vec3 k = vec3(0.57735); float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
// airy pastel spectrum: lavender, icy blue, aqua, mint, lemon, peach, rose
vec3 rainbow(float h) { return 0.5 + 0.5 * cos(TAU * (h + vec3(0.0, 0.33, 0.67))); }
vec3 pastel(float h, float s) { return mix(vec3(1.0), rainbow(h), s); }

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

float gM, gS;                       // pane clock (mids), stroke clock (build-accelerated)
float gW0, gW1, gW2, gW3;           // look weights: dichroic, painting, prism wash, reeded

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

// water-style caustic web (single call site)
float causticWeb(vec2 p, float t, float sharp) {
    vec2 i = p;
    float c = 1.0;
    for (int n = 0; n < 4; n++) {
        float tt = t * (1.0 - 3.5 / float(n + 1));
        i = p + vec2(cos(tt - i.x) + sin(tt + i.y), sin(tt - i.y) + cos(tt + i.x));
        c += 1.0 / length(vec2(p.x / (sin(i.x + tt) / 0.005), p.y / (cos(i.y + tt) / 0.005)));
    }
    c /= 4.0;
    c = 1.17 - pow(c, 1.4);
    return pow(clamp(c, 0.0, 1.0), sharp);
}

vec4 lightPass() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    float aspect = R.x / R.y;
    float px = 1.5 / R.y;
    float bass = bassP() * ar, mid = midP() * ar, high = highP() * ar;
    float beat = audioBeatPulse * ar;

    vec2 p = (fc - 0.5 * R) / R.y;
    // build: slow push-in; drop: a soft scale punch
    p *= (1.0 - 0.1 * gBuild) * (1.0 - 0.06 * gDrop);

    // per-look recipe (weights blend while morphing)
    float lift   = gLift;
    float paneOp = (0.95 * gW0 + 0.4 * gW1 + 0.7 * gW2 + 0.75 * gW3);
    float crisp  = clamp(1.0 * gW0 + 0.55 * gW1 + 0.1 * gW2 + 0.8 * gW3, 0.0, 1.0);
    float dichro = 0.15 + 0.35 * gW0 + 0.1 * gW2;
    float frostA = frost * (0.25 + 0.75 * gW2 + 0.2 * gW1);
    float reedG  = reeds * gW3;                                   // whole-frame ribs
    float sat    = glassColour * (0.36 + 0.14 * gW0 + 0.08 * gW2) * mix(0.45, 1.0, lift) * (1.0 + 0.3 * gDrop);
    float strokeAmt = strokes * (0.45 * gW0 + 1.3 * gW1 + 0.4 * gW2 + 0.5 * gW3);
    float nS = clamp((0.6 + 1.4 * gW1 + 0.3 * gW0 + 0.3 * gW3) * mix(0.6, 1.7, lift) + 1.5 * gBuild, 1.0, 4.0);
    float nP = max(floor(clamp(paneCount, 1.0, 7.0) * mix(0.5, 1.0, smoothstep(0.1, 0.8, lift)) + 0.5), 1.0);

    // ── whole-frame reeded glass: every rib is a little cylinder lens ──
    vec2 pp = p;
    float ribShade = 0.0, ribGlint = 0.0;
    if (reedG > 0.001) {
        float ra = 0.12 + 0.08 * sin(gM * 0.11);
        vec2 rq = rot(ra) * p;
        float N = 13.0;
        float ru = fract(rq.x * N) - 0.5;
        float rx = rq.x + ru * 1.1 * reedG / N;                  // magnify/slice the scene in every rib
        pp = rot(-ra) * vec2(rx, rq.y);
        ribShade = reedG * (0.05 + 0.1 * ru * ru * 4.0);
        float edgeAA = smoothstep(0.5, 0.5 - px * N * 1.5, abs(ru));
        ribShade += reedG * 0.08 * (1.0 - edgeAA);
        ribGlint = reedG * exp(-pow((ru + 0.22) / 0.07, 2.0));
    }

    // ── the glass stack, front (near) to back (far) ──
    vec3 T = vec3(1.0);              // transmission of the light table through all glass
    vec3 E = vec3(0.0);              // emitted / refracted light
    vec2 off = vec2(0.0);            // refraction offset accumulated by nearer panes
    float frostAcc = 0.0;
    float cover = 0.0;
    float hueBase = 0.02 * gM + 0.6;
    float edgeK = edgeGlow * (0.55 + 0.45 * crisp) * mix(0.6, 1.1, lift) * (1.0 + 2.5 * gDrop * gDrop + 0.4 * gDrop + 0.5 * gBuild);
    for (int i = 0; i < 7; i++) {
        float fi = float(i);
        if (fi > nP - 0.5) break;
        float h = h11(fi * 7.13 + 2.0), h2 = h11(fi * 3.71 + 9.0), h3 = h11(fi * 1.93 + 4.0);
        float z = fi / 6.0;                                     // 0 near … 1 far
        vec2 q = (pp + off * (0.6 + 0.4 * z)) * mix(1.12, 0.82, z);
        // drifting placement; mids slide the panes
        vec2 c = (vec2(fract(h + fi * 0.618), h2) - 0.5) * vec2(aspect * 0.85, 0.75);
        c += mix(0.16, 0.07, z) * vec2(sin(gM * (0.35 + 0.3 * h) + fi * 2.1), cos(gM * (0.3 + 0.35 * h2) + fi * 1.3));
        c += 0.05 * mid * vec2(sin(fi * 2.7 + gM), cos(fi * 1.9 - gM));
        float ang = (h3 - 0.5) * 2.6 + 0.18 * sin(gM * 0.22 + fi * 1.7);
        vec2 lq = rot(ang) * (q - c);
        vec2 hs = vec2(0.17 + 0.26 * h2, 0.09 + 0.17 * h) * paneSize * (1.0 + 0.35 * gW2) * (1.0 + 0.06 * bass);
        vec2 dq = abs(lq) - hs;
        float sd = length(max(dq, 0.0)) + min(max(dq.x, dq.y), 0.0) - 0.012;
        vec2 nrm = dq.x > dq.y ? vec2(sign(lq.x), 0.0) : vec2(0.0, sign(lq.y));
        float ca = h * TAU;
        vec2 cn = vec2(cos(ca), sin(ca));
        float cutD = dot(lq, cn) - hs.x * (0.35 + 0.4 * h3);              // shard cut
        if (cutD > sd) { sd = cutD; nrm = cn; }
        // painterly, brushed edge: a slow organic warp plus fine bristle drag
        float bn = vn(lq * vec2(1.8, 11.0) + vec2(fi * 7.0 + gS * 0.04, fi));
        float bn2 = vn(lq * 3.2 + fi * 3.1 + gM * 0.05);
        sd += painterly * (0.034 * (bn2 - 0.5) + 0.007 * (bn - 0.5)) * (1.0 - 0.35 * crisp);
        float bw = px + 0.005 * (1.0 - crisp) + 0.03 * (1.0 - crisp) * gW2 + depthBlur * 0.045 * z * z;
        float m = smoothstep(bw, -bw, sd);
        // fill: brushed opacity + dichroic hue drift across the pane
        float op = paneOp * (0.75 + 0.35 * bn) * (0.75 + 0.25 * h3);
        float hue = hueBase + h * 0.85 + dichro * (0.5 * lq.x + 0.35 * lq.y + 0.05 * sin(gM * 0.4 + fi));
        vec3 tint = pastel(hue, clamp(op * sat, 0.0, 0.42));
        // frosted surface + optional reeds inside this pane
        float reedP = reeds * step(0.72, h2) * 0.6 * (1.0 - gW3);
        float ru = fract(lq.x * 18.0) - 0.5;
        float tex = frostA * 0.06 * (vn(q * 70.0 + fi) - 0.5) + reedP * 0.06 * ru * ru * 4.0;
        vec3 Tin = T;
        float absorb = 0.05 + 0.1 * frostA + 0.04 * (1.0 - z);         // glass body + frost scatter (keeps B&W structure)
        T *= mix(vec3(1.0), tint * (1.0 - tex - absorb), m);
        // thick glass bevel: lit side catches light, far side falls darker (3D slab)
        float sl = dot(nrm, rot(ang) * vec2(-0.55, 0.83));
        float bev = smoothstep(-0.05 - 0.02 * h, -0.006, sd) * m * mix(1.0, 0.5, z);
        E += Tin * bev * max(sl, 0.0) * 0.32 * edgeK * pastel(hue + 0.45, 0.35);
        T *= 1.0 - bev * max(-sl, 0.0) * (0.16 + 0.1 * crisp);
        // prism-split edge light (dispersion fans the rim into a rainbow)
        float rw = bw * 0.7 + 0.0022 + 0.003 * (1.0 - crisp);
        float dd = dispersion * (0.0035 + 0.003 * z) * (1.0 + 0.6 * gDrop);
        vec3 sdc = vec3(sd) + vec3(-dd, 0.0, dd);
        vec3 rim = exp(-(sdc * sdc) / (rw * rw));
        float lit = (0.3 + 0.7 * max(sl, 0.0)) * (0.6 + 0.4 * sin(dot(lq, vec2(9.0, 5.0)) + gM * 0.6 + fi * 1.3));
        vec3 eCol = pastel(hue + 0.5, 0.5);
        E += Tin * rim * eCol * edgeK * lit * mix(1.0, 0.45, z) * 1.1;
        // a thin darker refraction line just inside the edge (reads in B&W)
        float inner = exp(-pow((sd + rw * 1.8) / rw, 2.0));
        T *= 1.0 - (0.22 * crisp + 0.06) * inner * mix(1.0, 0.4, z);
        // highs: sparkling glints riding the glass edges
        float gl = pow(vn(vec2(dot(lq, vec2(-nrm.y, nrm.x)) * 60.0 + gS * 2.0, fi * 5.0)), 7.0);   // streaks along the edge
        E += Tin * rim.g * gl * (0.12 + 5.0 * high) * mix(1.0, 0.3, z) * crisp * crisp;
        // reed highlights in reeded panes
        E += Tin * m * reedP * 0.1 * exp(-pow((ru + 0.2) / 0.12, 2.0)) * (1.0 - z * 0.5);
        // refraction: the view behind is bent (pane tilt + reed lensing)
        off += m * (rot(-ang) * vec2(0.05 * (h - 0.5) + reedP * ru * 0.03, 0.04 * (h3 - 0.5)));
        frostAcc += m * frostA * (0.5 + 0.5 * z);
        cover += m * op;
    }
    T *= 1.0 - ribShade;

    // ── behind the glass: light painted on the back wall ──
    vec2 pb = pp + off;
    float Tl = lum(T);
    // light table, swelling with bass
    vec3 table = backdrop(uv) * (0.8 - 0.07 * gW1 + 0.08 * bass + 0.03 * lift);
    // light cores: big soft sources of white light behind the glass
    vec2 lc = vec2(0.35 * sin(gM * 0.13), 0.12 * cos(gM * 0.17));
    float lr = 0.14 + 0.07 * bass + 0.04 * lift;
    vec3 cores = vec3(1.0, 0.98, 0.95) * (exp(-dot(pb - lc, pb - lc) / (lr * lr)) * (0.12 + 0.35 * bass + 0.1 * lift)
               + exp(-dot(pb + lc * 1.6, pb + lc * 1.6) / (lr * lr * 0.5)) * 0.08 * lift);

    // caustics (one call), sharper on clear glass, softened by frost
    float cs = causticWeb(pb * 4.0 - vec2(250.0, 247.0), gS * 0.5 + 23.0, mix(7.0, 3.0, clamp(frostAcc, 0.0, 1.0)));
    cs = smoothstep(0.12, 0.9, cs);
    float cAmt = caustics * (0.5 + 0.4 * gW0 + 0.4 * gW2 + 0.3 * gW3) * mix(0.45, 1.1, lift) * (0.3 + 1.1 * (1.0 - Tl)) * (1.0 + 0.7 * bass);
    vec3 causC = pastel(hueBase + 0.3 * pb.x - 0.2 * pb.y, 0.55) * cs * cAmt;

    // prism wash: a soft beam of spectral light projected across the glass
    float wa = 0.5 + 0.25 * sin(gM * 0.07);
    vec2 wd = vec2(cos(wa), sin(wa));
    float wb = dot(pb, vec2(-wd.y, wd.x)) - 0.12 * sin(gM * 0.1);
    float beam = exp(-wb * wb / (0.08 + 0.05 * lift));
    vec3 wash = rainbow(dot(pb, wd) * 0.9 - gS * 0.03) * beam * (0.1 + 0.55 * gW2 + 0.08 * gW0) * mix(0.5, 1.0, lift);
    wash += (pastel(dot(pb, wd) * 0.9 - gS * 0.03 + 0.2, 0.6) - 0.5) * beam * dispersion * 0.08;

    // brushstroke sweeps of refracted light (4 painted strokes + 1 beat stroke)
    vec3 S = vec3(0.0);
    vec3 ST = vec3(1.0);             // coloured light the strokes throw onto the table
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        float on = i == 4 ? clamp(beat * 1.6 + 1.2 * gDrop, 0.0, 2.0) : clamp(nS - fi, 0.0, 1.0) * strokeAmt;
        if (on < 0.002) continue;
        float h = h11(fi * 5.31 + 1.7), h2 = h11(fi * 2.17 + 8.3);
        float ang = (h - 0.5) * 1.3 + 0.25 * sin(gS * 0.05 + fi * 2.0) + (i == 4 ? 0.3 * sin(audioBassTime * 0.3) : 0.0);
        vec2 sq = rot(ang) * (pb - vec2(0.0, (h2 - 0.5) * 0.65));
        float x = sq.x;
        float yc = 0.16 * sin(x * (1.4 + h) + gS * (0.3 + 0.2 * h) + fi * 2.0) + 0.06 * sin(x * 3.3 - gS * 0.45 + h2 * 6.0);
        float d = sq.y - yc;
        float len = 0.9 + 0.9 * h;
        float hd = i == 4 ? mix(-1.1, 1.3, 1.0 - audioBeatPulse) : mix(-1.2, 1.4 + len, fract(gS * 0.05 * (0.6 + 0.6 * h) + h2));
        float along = clamp((x - (hd - len)) / len, 0.0, 1.0);     // 0 tail … 1 head
        float env = smoothstep(0.0, 0.35, along) * smoothstep(hd + 0.02, hd - 0.04, x) * (0.35 + 0.65 * along);
        if (env < 0.001) continue;
        float tip = smoothstep(hd + 0.01, hd - 0.16, x);                    // brush tip tapers to a point
        float w = strokeWidth * (0.008 + 0.012 * h) * (0.3 + 0.7 * along) * (0.25 + 0.75 * tip) * (1.0 + frostAcc * 1.2) * (1.0 + 0.5 * bass) * (i == 4 ? 1.6 : 1.0);
        float bristle = 0.35 + 0.65 * vn(vec2(x * 4.0 + fi * 13.0, d / w * 3.0 + fi * 7.0));   // dry-brush streaks
        float core = exp(-(d * d) / (w * w)) * (0.55 + 0.45 * bristle);
        vec3 dv = vec3(d) + vec3(-1.0, 0.0, 1.0) * w * 0.9 * (0.4 + dispersion);
        vec3 sh = pastel(hueBase + h + x * 0.25, 0.75);
        vec3 halo = exp(-(dv * dv) / (w * w * 3.0)) * sh * (0.4 + 0.6 * bristle);
        ST *= mix(vec3(1.0), sh, 0.55 * exp(-(d * d) / (w * w * 30.0)) * env * min(on, 1.0) * mix(0.6, 1.0, lift));
        float soft = 1.0 / (1.0 + frostAcc * 1.5);
        S += (vec3(1.0) * core * 1.4 * soft + halo * 0.5) * env * on * mix(0.55, 1.0, lift) * (1.0 + 0.9 * gBuild);
    }

    // drop: a burst of white light through all the glass
    float r = length(p);
    float rays = pow(vn(vec2(atan(p.y, p.x) * 9.0, r * 2.0 - TIME)), 2.0);
    vec3 burst = vec3(1.0) * gDrop * gDrop * (0.8 + 1.4 * (1.0 - Tl) + 1.0 * rays * exp(-r * 1.2));

    E += T * (cores + causC + wash + S * (0.75 + 0.25 * Tl) + burst);
    E += vec3(1.0, 0.99, 0.97) * ribGlint * 0.22;
    vec3 col = table * T * ST + E;
    col *= exposure;

    // long-exposure trails: only light brighter than the white table persists
    vec4 prev = texture2D(light, uv);
    if (FRAMEINDEX < 2) prev = vec4(col, 1.0);
    float decay = pow(min(trails, 0.97) * 0.92, clamp(TIMEDELTA * 60.0, 0.5, 30.0));   // frame-rate independent
    vec3 outc = max(col, prev.rgb * decay);
    float cov = clamp(1.0 - Tl + lum(E) * 0.6, 0.0, 1.0);
    return vec4(min(outc, vec3(8.0)), cov);
}

vec4 finalPass() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    vec4 B = texture2D(light, uv);
    vec3 L = B.rgb;
    // soft bloom: two rings of taps, highlights only
    vec3 g = vec3(0.0);
    float sc = R.y / 1080.0;
    for (int i = 0; i < 8; i++) {
        float a = float(i) * TAU / 8.0 + 0.3;
        vec2 o = vec2(cos(a), sin(a)) * sc;
        g += max(texture2D(light, uv + o * 9.0 / R).rgb - 0.92, 0.0)
           + max(texture2D(light, uv + o * 30.0 / R).rgb - 0.92, 0.0) * 0.7;
    }
    g /= 13.6;
    vec3 light2 = L + g * bloom * (0.9 + 0.5 * bassP() * ar + 0.8 * gDrop * gDrop);
    light2 *= (1.0 + 0.06 * audioLevel * ar);
    // highlight shoulder: linear to 0.8, then rolls smoothly into white
    vec3 col = mix(light2, 0.8 + 0.2 * (1.0 - exp(-(light2 - 0.8) / 0.2)), step(0.8, light2));
    col = hueRot(col, hueShift);
    col = mix(vec3(lum(col)), col, saturation);
    col *= 1.0 - 0.06 * smoothstep(0.55, 1.1, length((uv - 0.5) * vec2(R.x / R.y, 1.0)));
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    return vec4(clamp(col, 0.0, 1.0), transparentBg ? B.a : 1.0);
}

void main() {
    showDynamics();
    float ar = audioReact;
    // clocks: base time + audio accumulators + bounded build offsets (no time jumps)
    gM = TIME * 0.5 * speed + 0.6 * audioMidTime * ar * speed + 0.8 * gBuild * gBuild * speed;
    gS = TIME * speed + 0.3 * audioBassTime * ar * speed + 0.2 * audioHighTime * ar * speed
       + 4.0 * gBuild * gBuild * speed;
    float sel = float(formSel);
    gW0 = 0.0; gW1 = 0.0; gW2 = 0.0; gW3 = 0.0;
    float fa, fb, f;
    if (sel < 0.5) {
        float ph = TIME / morphPeriod;
        fa = mod(floor(ph), 4.0); fb = mod(fa + 1.0, 4.0);
        f = smoothstep(0.45, 1.0, fract(ph));
    } else { fa = sel - 1.0; fb = fa; f = 0.0; }
    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        float w = (abs(fa - fi) < 0.5 ? 1.0 - f : 0.0) + (abs(fb - fi) < 0.5 ? f : 0.0);
        if (i == 0) gW0 = w; else if (i == 1) gW1 = w; else if (i == 2) gW2 = w; else gW3 = w;
    }

    if (PASSINDEX == 0) gl_FragColor = lightPass();
    else                gl_FragColor = finalPass();
}
