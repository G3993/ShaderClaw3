/*{
  "DESCRIPTION": "Eggs — a luminous particle egg. Three forms morph into each other: Hatch (hundreds of wiggling filaments burst from a bright nucleus out to a glittering shell of dust, each tipped with a glowing bead), Vortex (spiral lanes of streaking particles swirl around the centre like a galaxy, every particle a bright head with a fading tail) and Field Lines (a rotating 3D egg wrapped in dotted field lines with particles travelling pole to pole, its silhouette bulging into lobes). Everything drifts like roe and plankton in fluid: a slow curl-noise current carries the whole field in soft eddies, each particle has its own tempo, sway and size, beads stretch along their motion, tumble and breathe, lanes and filaments meander and swim, particles dart and pause instead of marching in lockstep, and near layers drift further and faster than far ones. White light on black with a share of particles in the Studio Palette accent. Bass swells a current through the field that carries the particles and lets them drift back, beats flash the particles and give a gentle scatter that eases out, mids stir the turbulence and spin the vortex, highs make the shell dust sparkle. Rest: a small, sparse egg drifting lazily — few short filaments, thin lanes and quiet dust breathing slowly; Build: the currents strengthen and gather the particles into a swirl as the egg contracts and coils, filaments retract and wind tighter, the vortex and field lines spin faster and the shell dust flickers ever quicker with a tremor at the top; Drop: the egg hatches — a flash in the nucleus, everything bursts outward with the shell dust in a ring and the filaments shooting past the shell, then the particles drift back on turbulent currents into a large, dense, colour-flecked peak egg in lively swirling flow.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "Particles", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Auto Morph", "Hatch", "Vortex", "Field Lines"], "GROUP": "Egg" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 4.0, "GROUP": "Egg" },
    { "NAME": "eggSize",     "LABEL": "Egg Size",         "TYPE": "float", "MIN": 0.4, "MAX": 1.4, "DEFAULT": 1.0, "GROUP": "Egg" },
    { "NAME": "eggShape",    "LABEL": "Egg Shape",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Egg" },
    { "NAME": "density",     "LABEL": "Particle Density", "TYPE": "float", "MIN": 0.4, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "speed",       "LABEL": "Speed",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Particles" },
    { "NAME": "wiggle",      "LABEL": "Wiggle",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "trailLen",    "LABEL": "Trail Length",     "TYPE": "float", "MIN": 0.05, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Particles" },
    { "NAME": "dotSize",     "LABEL": "Bead Size",        "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Particles" },
    { "NAME": "lineBright",  "LABEL": "Line Brightness",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.9, "GROUP": "Particles" },
    { "NAME": "shellDust",   "LABEL": "Shell Dust",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Glow" },
    { "NAME": "glow",        "LABEL": "Glow",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Glow" },
    { "NAME": "accentShare", "LABEL": "Accent Particles", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Glow" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 2, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.07, 0.07, 0.075, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Eggs — single pass. Every particle family lives in a repeated coordinate
// cell (angular sectors, spiral lanes, sphere longitudes), so hundreds of
// particles and their trails cost a handful of evaluations per pixel.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  h22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
float wrapA(float a) { return mod(a + PI, TAU) - PI; }

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
// population cull: a particle family member with hash h survives when h is under the live fraction
float gPop;
float alive(float h) { return clamp((gPop - h) * 8.0, 0.0, 1.0); }

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

vec3 gAcc;       // accent colour for tinted particles
float gPx;       // one pixel in egg units
float gT;        // particle clock (additive: TIME + music clocks + bounded build offset)
float gFT, gFT2; // slow current clock / fast turbulence clock (additive)

vec3 tint(float h) { return h < clamp(accentShare + 0.15 * gPeak, 0.0, 1.0) ? gAcc * 1.3 : vec3(1.0); }

// ── Organic motion toolkit ───────────────────────────────────────────────
// smooth 1D wander noise in -1..1 (eased value noise: no sudden turns)
float vn(float x) { float i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f); return mix(h11(i), h11(i + 1.0), f) * 2.0 - 1.0; }
// 2D value noise with analytic derivatives (x = value, yz = gradient)
vec3 noised(vec2 x) {
    vec2 i = floor(x), f = fract(x);
    vec2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    vec2 du = 30.0 * f * f * (f * (f - 2.0) + 1.0);
    float a = h12(i), b = h12(i + vec2(1.0, 0.0)), c = h12(i + vec2(0.0, 1.0)), d = h12(i + vec2(1.0, 1.0));
    float k = a - b - c + d;
    return vec3(a + (b - a) * u.x + (c - a) * u.y + k * u.x * u.y, du * (vec2(b - a, c - a) + k * u.yx));
}
// Curl-noise current: displacement of the particle field (divergence-free swirl).
// Two drifting large-eddy layers (shared slow current = soft flocking) + a
// domain-warped turbulence layer on its own clock. amp1/amp2 bounded inputs.
float gN1;
vec2 flowD(vec2 q, float amp1, float amp2) {
    vec3 n1 = noised(q * 0.9 + vec2(gFT * 0.29, -gFT * 0.17));
    vec2 p2 = vec2(0.8 * q.x - 0.6 * q.y, 0.6 * q.x + 0.8 * q.y) * 0.8 + vec2(-gFT * 0.21, gFT * 0.26) + 17.0;
    vec3 n2 = noised(p2);
    vec2 g = n1.yz * 0.9 + vec2(0.8 * n2.y + 0.6 * n2.z, -0.6 * n2.y + 0.8 * n2.z) * 0.8;
    vec3 n3 = noised(q * 2.3 + (n1.x + n2.x) * 0.9 + vec2(-gFT2 * 0.37, gFT2 * 0.29) + 41.0);
    gN1 = n1.x + n2.x - 1.0;
    return vec2(g.y, -g.x) * amp1 + vec2(n3.z, -n3.y) * amp2;
}
// a lit 3D bead: squashed/stretched along its motion (dir), a slightly
// ovoid body that tumbles (tum), sphere shading + specular + soft halo
float bead(vec2 dv, vec2 dir, float r, float st, float tum) {
    vec2 m = vec2(dot(dv, dir), dot(dv, vec2(-dir.y, dir.x)));
    m *= vec2(1.0 / (1.0 + st), 1.0 + 0.35 * st);
    float cs = cos(tum), sn = sin(tum);
    m = vec2(cs * m.x - sn * m.y, sn * m.x + cs * m.y);
    m.x *= 0.88 + 0.1 * clamp(m.y / r, -1.0, 1.0);            // egg-shaped body
    float d = length(m);
    float disc = smoothstep(r + gPx, r - gPx, d);
    vec2 n2 = dv / max(length(dv), 1e-5) * min(d / r, 1.0);
    vec3 nn = vec3(n2, sqrt(max(1.0 - dot(n2, n2), 0.0)));
    float lit = 0.5 + 0.45 * max(dot(nn, vec3(-0.42, 0.5, 0.76)), 0.0);
    float spec = pow(max(dot(nn, vec3(-0.25, 0.32, 0.91)), 0.0), 22.0);
    return disc * (lit + 0.5 * spec) + 0.3 * exp(-d / r) * smoothstep(3.0 * r, r, d);
}

// ── Hatch: filaments from the nucleus to the shell ───────────────────────
vec3 hatch(vec2 q0, vec2 D, float beat) {
    vec3 c = vec3(0.0);
    float N = floor(46.0 * density);
    for (int layer = 0; layer < 2; layer++) {
        float fl = float(layer);
        float dep = fl;                                          // layer 1 is nearer: carried further, bigger beads
        vec2 q = q0 - D * (0.6 + 0.6 * dep);
        float rho = length(q), a = atan(q.y, q.x);
        float off = fl * 0.5;
        float k0 = floor((a + PI) / TAU * N + off);
        for (int j = -1; j <= 1; j++) {
            float k = mod(k0 + float(j), N);
            float hs = h11(k * 1.37 + fl * 31.0);
            float hs2 = h11(k * 2.71 + fl * 17.0);
            if (hs2 < 0.35) continue;                            // not every sector has a filament
            float liv = alive(h11(k * 4.13 + fl * 7.7));
            if (liv < 0.001) continue;
            // personality: each filament has its own tempo, sway and reach
            float rate = 0.6 + 0.8 * h11(k * 6.7 + fl * 3.3);
            float sway = (TAU / N) * 0.42 * vn(gT * 0.16 * rate + k * 3.1 + fl * 11.0);
            float ac = (k - off + 0.5) / N * TAU - PI + (hs - 0.5) * 0.9 * TAU / N + sway + gT * 0.03 * (fl * 2.0 - 1.0);
            float len = (0.55 + 0.42 * h11(k * 5.3 + fl)) * (0.86 + 0.16 * vn(gT * 0.32 * rate + k * 1.9) + 0.12 * bassP() * audioReact + 0.15 * beat);
            // rest: short; build: retract; drop: shoot out past the shell
            len *= mix(0.62, 1.0, gLift) * (1.0 - 0.3 * gBuild) * liv;
            len = min(len, 0.97 + 0.15 * gPeak) * (1.0 + 0.3 * gDrop);
            float wf = (8.0 + 6.0 * hs) * (1.0 + 0.8 * gBuild);      // coil tighter during the build
            float wa = wiggle * (0.045 + 0.05 * hs) * mix(0.6, 1.15, gLift) * (0.8 + 0.35 * vn(gT * 0.22 * rate + k * 5.7));
            // swimming flagellum: two travelling waves at personal speeds, growing toward the tip
            float r1 = (1.2 + 1.1 * hs2) * rate, r2 = (1.9 + 1.0 * hs) * rate;
            float p1 = gT * r1 + k, p2 = gT * r2 + k * 1.7;
            float wig = wa * rho * (0.72 * sin(rho * wf - p1) + 0.38 * sin(rho * wf * 1.73 - p2));
            float da = wrapA(a - ac - wig);
            float d = abs(da) * rho;
            float on = step(rho, len) * smoothstep(0.02, 0.08, rho);
            float line = exp(-d / (gPx * 0.8)) * on * (0.35 + 0.65 * rho / len);
            float tA = ac + wa * len * (0.72 * sin(len * wf - p1) + 0.38 * sin(len * wf * 1.73 - p2));
            vec2 rd = vec2(cos(tA), sin(tA));
            vec2 tip = rd * len;
            // tip velocity across the filament → squash & stretch along it
            float vt = -wa * len * len * (0.72 * r1 * cos(len * wf - p1) + 0.38 * r2 * cos(len * wf * 1.73 - p2));
            float br = (0.012 + 0.012 * hs) * dotSize * mix(0.75, 1.15, gLift) * (0.85 + 0.25 * dep)
                     * (1.0 + 0.16 * sin(TIME * (0.7 + 0.6 * hs2) + k * 4.0));   // breathing
            float dot1 = bead(q - tip, vec2(-rd.y, rd.x), br, clamp(abs(vt) * 5.0, 0.0, 0.45), gT * (0.3 + 0.5 * hs) + k);
            c += tint(h11(k * 9.1 + fl)) * (line * 0.55 * lineBright * mix(0.7, 1.1, gLift) + dot1 * (1.0 + beat + gDrop)) * liv * (0.8 + 0.25 * dep);
        }
    }
    // nucleus: a tight cluster of roe-like beads jostling in their cells
    vec2 qn = q0 - D * 0.4;
    float rho = length(qn);
    vec2 gp = qn * 26.0;
    vec2 gi = floor(gp), gf = fract(gp);
    float nuc = 0.0;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 o = vec2(float(i), float(j));
        vec2 hh = h22(gi + o);
        float rt = 0.3 + 0.5 * hh.x;
        vec2 cc = o + 0.5 + 0.3 * vec2(vn(gT * rt + hh.y * 40.0), vn(gT * rt * 0.8 + hh.x * 70.0 + 5.0));
        float rr = length((gi + o) / 26.0);
        float keep = step(h12(gi + o + 4.0), smoothstep(0.22, 0.02, rr));
        float sz = 0.2 * (1.0 + 0.2 * sin(TIME * (0.8 + hh.x) + hh.y * TAU));
        nuc += keep * smoothstep(sz, sz * 0.45, length(gf - cc));
    }
    c += vec3(nuc) * (1.0 + beat + 1.5 * gDrop);
    c += vec3(1.0) * exp(-rho * 9.0) * (0.25 + 0.15 * gBuild + 1.2 * gDrop);
    return c;
}

// ── Vortex: spiral lanes of streaking particles ──────────────────────────
vec3 vortex(vec2 q0, vec2 D, float beat) {
    vec3 c = vec3(0.0);
    for (int fam = 0; fam < 3; fam++) {
        float ff = float(fam);
        float dep = ff * 0.5;                                    // depth 0 far … 1 near (parallax)
        vec2 q = q0 - D * (0.55 + 0.6 * dep);
        float rho = length(q), a = atan(q.y, q.x);
        float lr = log(max(rho, 1e-3));
        float b = 0.32 + 0.08 * ff;                              // winding
        float lanes = floor((10.0 + 4.0 * ff) * density);
        float u = (lr - b * a) * lanes / TAU * 2.0 + gT * 0.12 + ff * 0.37;
        float ui = floor(u);
        float uf = fract(u) - 0.5;
        float lh = h11(ui * 1.7 + ff * 13.0);
        // meandering lane: the path snakes (integer angular freqs keep it seamless), trails follow it
        float wav = 0.11 * sin(a * 3.0 + gT * (0.22 + 0.2 * lh) + ui * 1.3) + 0.05 * sin(a * 7.0 - gT * (0.35 + 0.3 * lh) + ui * 2.1);
        float dud = lanes / TAU * 2.0 / max(rho, 0.02);          // u per unit distance (approx)
        float dLs = (uf - (lh - 0.5) * 0.4 - wav) / dud;
        float dLane = abs(dLs);
        if (dLane > gPx * 5.0 && dLane > 0.035) continue;
        float liv = alive(h11(ui * 2.37 + ff * 5.1));
        if (liv < 0.001) continue;
        // particles drifting along the lane — each lane darts and pauses on its own rhythm
        float M = floor(5.0 + 4.0 * lh);
        float sp = (0.25 + 0.2 * lh) * (0.7 + 0.5 * dep);        // near drift faster
        float w = 0.35 + 0.5 * h11(ui * 4.3 + ff * 2.0);
        float clk = gT * sp + 0.8 * sp / w * sin(gT * w + ui * 2.9);
        float spd = 1.0 + 0.8 * cos(gT * w + ui * 2.9);          // 0.2 … 1.8 × (eased)
        float sgn = fam == 1 ? -1.0 : 1.0;
        float v = (a / TAU + 0.5) * M - clk * sgn;
        float vi = floor(v);
        float vf = fract(v);
        float ph = h11(vi * 3.1 + ui * 7.3 + ff);
        // personal wander inside its cell: overtakes, lags, drifts
        float head = 0.5 + 0.24 * vn(gT * (0.25 + 0.4 * ph) + vi * 7.3 + ui * 1.1);
        float L = trailLen * (0.4 + 0.6 * h11(vi + ui * 2.0)) * (0.35 + 0.4 * spd);
        float s = -sgn * (vf - head);                            // distance behind the head along the lane
        float trail = step(0.0, s) * step(s, L) * (1.0 - s / L);
        float env = smoothstep(1.02, 0.75, rho) * smoothstep(0.03, 0.12, rho);
        float line = exp(-dLane / (gPx * 0.7)) * trail * env;
        // the head bead: stretched with speed, tumbling, breathing
        float dAl = (vf - head) / M * TAU * rho;
        float br = 0.009 * dotSize * (0.85 + 0.3 * dep) * (1.0 + 0.12 * sin(TIME * (0.9 + 0.8 * ph) + vi * 3.7));
        float bd = bead(vec2(dAl, dLs), vec2(1.0, 0.0), br, clamp(spd * sp * 1.1, 0.0, 0.45), gT * (0.4 + 0.6 * ph) + vi) * env;
        vec3 tc = tint(h11(vi * 5.5 + ui + ff * 3.0));
        c += tc * (line * 0.6 * lineBright + bd * (1.1 + beat + gDrop)) * liv * (0.75 + 0.35 * dep);
    }
    // dense core
    float rho = length(q0 - D * 0.5);
    c += vec3(1.0) * exp(-rho * 14.0) * (0.4 + 0.2 * gBuild + gDrop);
    return c;
}

// ── Field Lines: a rotating 3D egg wrapped in dotted lines ───────────────
vec3 fieldSide(vec3 n, float side, float beat) {
    // rotate
    // a lazy, slightly wandering spin (eased noise on top of the steady turn)
    float ry = gT * 0.22 + 0.45 * vn(gT * 0.09 + 3.0), rx = 0.35 + 0.18 * vn(gT * 0.12 + 7.0);
    n.xz = mat2(cos(ry), -sin(ry), sin(ry), cos(ry)) * n.xz;
    n.yz = mat2(cos(rx), -sin(rx), sin(rx), cos(rx)) * n.yz;
    float lat = asin(clamp(n.y, -1.0, 1.0));
    float lon = atan(n.x, n.z);
    float N = floor(34.0 * density);
    float lon2 = lon + wiggle * (0.45 * sin(lat * 2.0 + gT * 0.5 + 0.8 * vn(gT * 0.15)) + 0.12 * sin(lat * 7.0 - gT * 1.1) + 0.06 * sin(lat * 11.0 + lon * 2.0 - gT * 0.8));
    float li = floor(lon2 / TAU * N + 0.5);
    float dl = abs(wrapA(lon2 - li / N * TAU)) * cos(lat);
    float lh = h11(li * 1.3);
    float line = exp(-dl / (gPx * 0.9)) * alive(h11(li * 3.91 + 0.3));
    // dotted: particles travel pole to pole, each line drifting, darting and pausing on its own rhythm
    float M = 16.0 + 10.0 * lh;
    float sp = 0.45 + 0.55 * lh, w = 0.3 + 0.5 * h11(li * 5.3 + 1.0);
    float flow = lat / PI * M - (gT * sp + 0.75 * sp / w * sin(gT * w + li * 1.7));
    float fd = abs(fract(flow) - 0.5);
    float dots = smoothstep(0.32, 0.12, fd);
    float headK = step(0.82, h11(floor(flow) * 2.3 + li * 7.1)) * alive(h11(li * 3.91 + 0.3));
    float beadR = 0.012 * dotSize / max(1.0 / M * PI, 0.01);
    float bead = headK * smoothstep(0.5 * beadR + 0.08, 0.5 * beadR, fd) * exp(-dl / (gPx * 2.5));
    float poleGlow = exp(-(1.0 - abs(n.y)) * 6.0) * 0.4;
    vec3 tc = tint(h11(li * 9.7 + floor(flow)));
    return tc * (line * (0.25 + 0.75 * dots) * 0.55 * lineBright + bead * (1.2 + beat)) * side + vec3(poleGlow) * side * 0.5;
}

vec3 field(vec2 q0, vec2 D, float beat) {
    // the near (front) hemisphere is carried further by the current than the far one
    vec3 c = vec3(0.0);
    for (int s = 0; s < 2; s++) {
        float fs = float(s);
        vec2 q = q0 - D * mix(0.9, 0.4, fs);
        float rho = length(q);
        if (rho >= 1.0) continue;
        float z = sqrt(1.0 - rho * rho) * (1.0 - 2.0 * fs);
        c += fieldSide(vec3(q, z), mix(1.0, 0.28, fs), beat);
    }
    return c;
}

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    float beat = audioBeatPulse * ar;
    // additive clocks only (music clocks + bounded build offset) — no phase jumps
    gT = TIME * speed + 0.4 * audioBassTime * ar * speed + 0.3 * audioMidTime * ar * speed + 3.0 * gBuild * gBuild * speed;
    gFT  = TIME * 0.22 * speed + 0.12 * audioBassTime * ar * speed + 0.06 * audioMidTime * ar * speed + 1.2 * gBuild * gBuild * speed;
    gFT2 = TIME * 0.6 * speed + 0.35 * audioMidTime * ar * speed + 0.2 * audioHighTime * ar * speed + 2.0 * gBuild * gBuild * speed;
    gPop = mix(0.42, 1.08, smoothstep(0.15, 0.85, gLift));
    gAcc = float(paletteMode) > 1.5 ? palBlue.rgb : (float(paletteMode) > 0.5 ? palRed.rgb : vec3(1.0, 0.55, 0.35));

    // form weights
    float sel = float(formSel);
    float w0, w1, w2;
    if (sel < 0.5) {
        float ph = TIME / morphPeriod;
        float k = mod(floor(ph), 3.0), k2 = mod(k + 1.0, 3.0);
        float f = smoothstep(0.6, 1.0, fract(ph));
        w0 = (k < 0.5 ? 1.0 - f : 0.0) + (k2 < 0.5 ? f : 0.0);
        w1 = (abs(k - 1.0) < 0.5 ? 1.0 - f : 0.0) + (abs(k2 - 1.0) < 0.5 ? f : 0.0);
        w2 = (k > 1.5 ? 1.0 - f : 0.0) + (k2 > 1.5 ? f : 0.0);
    } else {
        w0 = abs(sel - 1.0) < 0.5 ? 1.0 : 0.0;
        w1 = abs(sel - 2.0) < 0.5 ? 1.0 : 0.0;
        w2 = abs(sel - 3.0) < 0.5 ? 1.0 : 0.0;
    }

    // egg space
    float Rg = 0.4 * eggSize * (1.0 + 0.05 * bassP() * ar + 0.02 * sin(TIME * 0.8))
             * mix(0.78, 1.0, gLift) * (1.0 - 0.14 * gBuild) * (1.0 + 0.1 * gDrop);
    vec2 p = (fc - 0.5 * R) / R.y;
    // build tremor near the top of the riser
    p += (h22(vec2(floor(TIME * 24.0), 1.3)) - 0.5) * 0.008 * smoothstep(0.65, 1.0, gBuild);
    vec2 q = p / Rg;
    q.y /= 1.0 + 0.14 * eggShape;
    q.x /= 1.0 - 0.09 * eggShape * clamp(q.y, -1.0, 1.0);     // egg: fuller below, narrower above
    float a = atan(q.y, q.x);
    // lobes (field-lines form bulges like the reference)
    q /= 1.0 + w2 * 0.06 * sin(a * 3.0 + gT * 0.5) * wiggle;
    float rho = length(q);
    gPx = 1.0 / (R.y * Rg);

    // ── the fluid the particles drift in ──
    // drop: a burst that disperses outward, then the field drifts back (eased impulse)
    float disp = 1.1 * gDrop * (1.0 - gDrop) + 0.25 * gDrop;
    float bs = bassP() * ar, md = midP() * ar;
    // big eddies: lazy at rest, stronger in the build, lively at the peak, swelled by bass
    float amp1 = 0.018 + 0.016 * gLift + 0.03 * gBuild + 0.025 * bs + 0.05 * disp;
    // turbulence: mids stir it up; the return from a drop is turbulent, not rigid
    float amp2 = (0.003 + 0.007 * gLift + 0.01 * md) * (1.0 + 2.5 * disp);
    vec2 D = flowD(q, amp1, amp2);
    // bass swell: a soft current that carries the field one way, then lets it drift back
    float ca = gFT * 0.35 + 1.3 * vn(gFT * 0.2);
    D += vec2(cos(ca), sin(ca)) * 0.05 * bs * (0.7 + 0.5 * gN1) * smoothstep(1.3, 0.2, rho);
    // build: currents gather the particles into a swirl (bounded twist, eases in and out)
    float tw0 = (0.85 * gBuild + 0.22 * gLift * vn(gFT * 0.45 + 2.0)) * exp(-rho * rho * 1.2);
    D += q - vec2(cos(tw0) * q.x - sin(tw0) * q.y, sin(tw0) * q.x + cos(tw0) * q.y);
    // beat: a gentle, uneven scatter that eases out with the pulse; drop: outward burst
    D += q * (0.06 * beat * (0.7 + 0.6 * gN1) + disp / (1.0 + disp));

    vec3 light = vec3(0.0);
    if (w0 > 0.001) light += hatch(q, D, beat) * w0;
    if (w1 > 0.001) light += vortex(q, D, beat) * w1;
    if (w2 > 0.001) light += field(q, D, beat) * w2;

    // glittering shell dust (strongest for Hatch): a far fine layer and a near coarse
    // layer, each mote wandering in its cell and riding the current (near drifts further)
    float shellW = shellDust * (0.45 + 0.55 * w0 + 0.2 * w2);
    // drop: the shell dust is blasted outward, then drifts back as the impulse decays
    float blast = 1.0 + 1.1 * gDrop * (1.0 - gDrop) + 0.2 * gDrop;
    for (int ly = 0; ly < 2; ly++) {
        float dep = float(ly);
        vec2 qd = (q - D * (0.4 + 0.5 * dep)) / blast;
        float rhod = length(qd);
        float ro = gFT * (0.05 + 0.04 * dep) * (dep * 2.0 - 1.0);   // the layers slide past each other
        qd = vec2(cos(ro) * qd.x - sin(ro) * qd.y, sin(ro) * qd.x + cos(ro) * qd.y);
        vec2 sp = qd * mix(100.0, 64.0, dep);
        vec2 si = floor(sp) + dep * 57.0, sf = fract(sp);
        vec2 hh = h22(si);
        float rt = (0.25 + 0.5 * hh.x) * (0.7 + 0.5 * dep);
        vec2 sc = 0.5 + 0.3 * vec2(vn(gT * rt + hh.y * 40.0), vn(gT * rt * 0.83 + hh.x * 70.0 + 5.0));
        float sd = length(sf - sc);
        float shellProf = exp(-pow((rhod - 0.95) / 0.06, 2.0)) + 0.25 * smoothstep(1.0, 0.3, rhod) * smoothstep(0.2, 0.7, rhod);
        shellProf *= mix(0.55, 1.1, gLift) * mix(0.75, 1.0, dep);
        float keep = step(1.0 - shellProf * 0.9, h12(si + 2.0));
        // flicker whose rate climbs with the build (phase ~ b^3, bounded, continuous)
        float tw = 0.5 + 0.5 * sin(TIME * (2.0 + 4.0 * h12(si + 9.0)) + h12(si) * 30.0 + 50.0 * gBuild * gBuild * gBuild * (0.5 + h12(si + 4.0)));
        float sz = (0.16 + 0.22 * h12(si + 5.0) * h12(si + 6.0)) * (1.0 + 0.2 * sin(TIME * (0.6 + hh.y) + hh.x * TAU));
        float dust = keep * smoothstep(sz, sz * 0.4, sd) * (0.4 + 0.6 * tw) * (1.0 + 1.2 * highP() * ar) * mix(0.55, 1.0, dep);
        // occasional bright bokeh flares on the shell
        float fl = step(0.985 - 0.02 * gPeak, h12(si + 13.0)) * exp(-pow((rhod - 0.93) / 0.08, 2.0)) * smoothstep(1.2, 0.0, sd) * (0.6 + 0.4 * tw) * dep;
        light += (vec3(dust) + tint(h12(si + 3.0)) * fl * 1.5) * shellW * (1.0 + 0.6 * gDrop + 0.5 * gBuild);
    }
    // drop shock ring racing out from the shell
    float ringR = 0.95 + 1.1 * (1.0 - gDrop);
    light += mix(vec3(1.0), gAcc * 1.4, 0.35) * gDrop * exp(-pow((rho - ringR) / (0.03 + 0.08 * (1.0 - gDrop)), 2.0)) * 0.9;

    // egg glow and halo
    float halo = exp(-max(rho - 0.9, 0.0) * 5.0) * smoothstep(0.0, 1.0, rho) * 0.12;
    float breathe = 0.5 + 0.5 * sin(TIME * 0.7);
    light += vec3(halo) * glow * (1.0 + 0.6 * beat + 0.4 * gDrop) * mix(0.7 + 0.3 * breathe, 1.05, gLift);
    light += vec3(0.08) * glow * smoothstep(1.05, 0.0, rho) * (mix(0.6, 0.9, gLift) + 0.5 * gBuild);

    vec3 bg = backdrop(uv);
    vec3 col = bg + light * (1.0 + 0.15 * audioLevel * ar);
    col = 1.0 - exp(-col * 1.3);
    float alpha = clamp(lum(light) * 2.0, 0.0, 1.0);
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
