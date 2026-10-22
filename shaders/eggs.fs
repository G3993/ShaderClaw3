/*{
  "DESCRIPTION": "Eggs — a luminous particle egg. Three forms morph into each other: Hatch (hundreds of wiggling filaments burst from a bright nucleus out to a glittering shell of dust, each tipped with a glowing bead), Vortex (spiral lanes of streaking particles swirl around the centre like a galaxy, every particle a bright head with a fading tail) and Field Lines (a rotating 3D egg wrapped in dotted field lines with particles travelling pole to pole, its silhouette bulging into lobes). White light on black with a share of particles in the Studio Palette accent. Bass makes the egg breathe and the filaments reach, beats flash the particles and fire a burst outward, mids spin the vortex, highs make the shell dust sparkle. Rest: a small, sparse egg — few short filaments, thin lanes and quiet dust breathing slowly; Build: the egg contracts and coils, filaments retract and wind tighter, the vortex and field lines spin faster and the shell dust flickers ever quicker with a tremor at the top; Drop: the egg hatches — a flash in the nucleus, the shell dust blasts outward in a ring and the filaments shoot past the shell, then it settles into a large, dense, colour-flecked peak egg.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-21 — original.",
  "CATEGORIES": ["Generator", "Particles", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Auto Morph", "Hatch", "Vortex", "Field Lines"], "GROUP": "Egg" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 12.0, "GROUP": "Egg" },
    { "NAME": "eggSize",     "LABEL": "Egg Size",         "TYPE": "float", "MIN": 0.4, "MAX": 1.4, "DEFAULT": 1.0, "GROUP": "Egg" },
    { "NAME": "eggShape",    "LABEL": "Egg Shape",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Egg" },
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
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
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

vec3 gAcc;       // accent colour for tinted particles
float gPx;       // one pixel in egg units
float gT;

vec3 tint(float h) { return h < clamp(accentShare + 0.15 * gPeak, 0.0, 1.0) ? gAcc * 1.3 : vec3(1.0); }

// ── Hatch: filaments from the nucleus to the shell ───────────────────────
vec3 hatch(vec2 q, float rho, float a, float beat) {
    vec3 c = vec3(0.0);
    float N = floor(46.0 * density);
    for (int layer = 0; layer < 2; layer++) {
        float fl = float(layer);
        float off = fl * 0.5;
        float k0 = floor((a + PI) / TAU * N + off);
        for (int j = -1; j <= 1; j++) {
            float k = mod(k0 + float(j), N);
            float hs = h11(k * 1.37 + fl * 31.0);
            float hs2 = h11(k * 2.71 + fl * 17.0);
            if (hs2 < 0.35) continue;                            // not every sector has a filament
            float liv = alive(h11(k * 4.13 + fl * 7.7));
            if (liv < 0.001) continue;
            float ac = (k - off + 0.5) / N * TAU - PI + (hs - 0.5) * TAU / N + gT * 0.03 * (fl * 2.0 - 1.0);
            float len = (0.55 + 0.42 * h11(k * 5.3 + fl)) * (0.85 + 0.15 * sin(gT * 0.7 + k) + 0.12 * bassP() * audioReact + 0.15 * beat);
            // rest: short; build: retract; drop: shoot out past the shell
            len *= mix(0.62, 1.0, gLift) * (1.0 - 0.3 * gBuild) * liv;
            len = min(len, 0.97 + 0.15 * gPeak) * (1.0 + 0.3 * gDrop);
            float wf = (9.0 + 6.0 * hs) * (1.0 + 0.8 * gBuild);      // coil tighter during the build
            float wa = wiggle * (0.05 + 0.05 * hs) * mix(0.6, 1.15, gLift);
            float wig = wa * sin(rho * wf - gT * 2.2 + k) * rho;
            float da = wrapA(a - ac - wig);
            float d = abs(da) * rho;
            float on = step(rho, len) * smoothstep(0.02, 0.08, rho);
            float line = exp(-d / (gPx * 0.8)) * on * (0.35 + 0.65 * rho / len);
            vec2 tip = vec2(cos(ac + wa * sin(len * wf - gT * 2.2 + k) * len),
                            sin(ac + wa * sin(len * wf - gT * 2.2 + k) * len)) * len;
            float br = (0.012 + 0.012 * hs) * dotSize * mix(0.75, 1.15, gLift);
            float dt = length(q - tip);
            float dot1 = smoothstep(br + gPx, br - gPx, dt) + 0.3 * exp(-dt / br) * smoothstep(3.0 * br, br, dt);
            c += tint(h11(k * 9.1 + fl)) * (line * 0.55 * lineBright * mix(0.7, 1.1, gLift) + dot1 * (1.0 + beat + gDrop)) * liv;
        }
    }
    // nucleus: a tight cluster of beads
    vec2 gp = q * 26.0;
    vec2 gi = floor(gp), gf = fract(gp);
    float nuc = 0.0;
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 o = vec2(float(i), float(j));
        vec2 cc = o + h22(gi + o);
        float rr = length((gi + o) / 26.0);
        float keep = step(h12(gi + o + 4.0), smoothstep(0.22, 0.02, rr));
        nuc += keep * smoothstep(0.28, 0.12, length(gf - cc));
    }
    c += vec3(nuc) * (1.0 + beat + 1.5 * gDrop);
    c += vec3(1.0) * exp(-rho * 9.0) * (0.25 + 0.15 * gBuild + 1.2 * gDrop);
    return c;
}

// ── Vortex: spiral lanes of streaking particles ──────────────────────────
vec3 vortex(vec2 q, float rho, float a, float beat) {
    vec3 c = vec3(0.0);
    float lr = log(max(rho, 1e-3));
    for (int fam = 0; fam < 3; fam++) {
        float ff = float(fam);
        float b = 0.32 + 0.08 * ff;                              // winding
        float lanes = floor((10.0 + 4.0 * ff) * density);
        float u = (lr - b * a) * lanes / TAU * 2.0 + gT * 0.12 * (1.0 + 0.5 * midP() * audioReact) + ff * 0.37;
        float ui = floor(u);
        float uf = fract(u) - 0.5;
        float lh = h11(ui * 1.7 + ff * 13.0);
        // lane centre jitter + thickness in pixels
        float dud = lanes / TAU * 2.0 / max(rho, 0.02);          // u per unit distance (approx)
        float dLane = abs(uf - (lh - 0.5) * 0.5) / dud;
        if (dLane > gPx * 4.0 && dLane > 0.03) continue;
        float liv = alive(h11(ui * 2.37 + ff * 5.1));
        if (liv < 0.001) continue;
        // particles running along the lane
        float M = floor(5.0 + 4.0 * lh);
        float v = (a / TAU + 0.5) * M - gT * (0.25 + 0.2 * lh) * (fam == 1 ? -1.0 : 1.0);
        float vi = floor(v);
        float vf = fract(v);
        float ph = h11(vi * 3.1 + ui * 7.3 + ff);
        float head = 0.25 + 0.6 * ph;
        float L = trailLen * (0.4 + 0.6 * h11(vi + ui * 2.0));
        float dir = fam == 1 ? 1.0 : -1.0;
        float s = dir > 0.0 ? (vf - head) : (head - vf);         // distance behind the head along the lane
        float trail = step(0.0, s) * step(s, L) * (1.0 - s / L);
        float env = smoothstep(1.02, 0.75, rho) * smoothstep(0.03, 0.12, rho);
        float line = exp(-dLane / (gPx * 0.7)) * trail * env;
        // the head bead
        float dAlong = abs(vf - head) / M * TAU * rho;
        float br = 0.009 * dotSize;
        float bead = smoothstep(br + gPx, br - gPx, length(vec2(dAlong, dLane))) * env;
        vec3 tc = tint(h11(vi * 5.5 + ui + ff * 3.0));
        c += tc * (line * 0.6 * lineBright + bead * (1.1 + beat + gDrop)) * liv;
    }
    // dense core
    c += vec3(1.0) * exp(-rho * 14.0) * (0.4 + 0.2 * gBuild + gDrop);
    return c;
}

// ── Field Lines: a rotating 3D egg wrapped in dotted lines ───────────────
vec3 fieldSide(vec3 n, float side, float beat) {
    // rotate
    float ry = gT * 0.25, rx = 0.35 + 0.15 * sin(gT * 0.13);
    n.xz = mat2(cos(ry), -sin(ry), sin(ry), cos(ry)) * n.xz;
    n.yz = mat2(cos(rx), -sin(rx), sin(rx), cos(rx)) * n.yz;
    float lat = asin(clamp(n.y, -1.0, 1.0));
    float lon = atan(n.x, n.z);
    float N = floor(34.0 * density);
    float lon2 = lon + wiggle * (0.5 * sin(lat * 2.0 + gT * 0.6) + 0.12 * sin(lat * 7.0 - gT * 1.4));
    float li = floor(lon2 / TAU * N + 0.5);
    float dl = abs(wrapA(lon2 - li / N * TAU)) * cos(lat);
    float lh = h11(li * 1.3);
    float line = exp(-dl / (gPx * 0.9)) * alive(h11(li * 3.91 + 0.3));
    // dotted: particles travel pole to pole along each line
    float M = 16.0 + 10.0 * lh;
    float flow = lat / PI * M - gT * (0.6 + 0.6 * lh);
    float fd = abs(fract(flow) - 0.5);
    float dots = smoothstep(0.32, 0.12, fd);
    float headK = step(0.82, h11(floor(flow) * 2.3 + li * 7.1)) * alive(h11(li * 3.91 + 0.3));
    float beadR = 0.012 * dotSize / max(1.0 / M * PI, 0.01);
    float bead = headK * smoothstep(0.5 * beadR + 0.08, 0.5 * beadR, fd) * exp(-dl / (gPx * 2.5));
    float poleGlow = exp(-(1.0 - abs(n.y)) * 6.0) * 0.4;
    vec3 tc = tint(h11(li * 9.7 + floor(flow)));
    return tc * (line * (0.25 + 0.75 * dots) * 0.55 * lineBright + bead * (1.2 + beat)) * side + vec3(poleGlow) * side * 0.5;
}

vec3 field(vec2 q, float rho, float beat) {
    if (rho >= 1.0) return vec3(0.0);
    float z = sqrt(1.0 - rho * rho);
    vec3 front = fieldSide(vec3(q, z), 1.0, beat);
    vec3 back = fieldSide(vec3(q, -z), 0.28, beat);
    return front + back;
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

    vec3 light = vec3(0.0);
    if (w0 > 0.001) light += hatch(q, rho, a, beat) * w0;
    if (w1 > 0.001) light += vortex(q, rho, a, beat) * w1;
    if (w2 > 0.001) light += field(q, rho, beat) * w2;

    // glittering shell dust (strongest for Hatch)
    float shellW = shellDust * (0.45 + 0.55 * w0 + 0.2 * w2);
    // drop: the shell dust is blasted outward, then drifts back as the impulse decays
    float blast = 1.0 + 1.1 * gDrop * (1.0 - gDrop) + 0.2 * gDrop;
    vec2 qd = q / blast;
    float rhod = length(qd);
    vec2 sp = qd * 70.0;
    vec2 si = floor(sp), sf = fract(sp);
    vec2 sc = h22(si);
    float sd = length(sf - sc);
    float shellProf = exp(-pow((rhod - 0.95) / 0.06, 2.0)) + 0.25 * smoothstep(1.0, 0.3, rhod) * smoothstep(0.2, 0.7, rhod);
    shellProf *= mix(0.55, 1.1, gLift);
    float keep = step(1.0 - shellProf * 0.9, h12(si + 2.0));
    // flicker whose rate climbs with the build (phase ~ b^3, bounded, continuous)
    float tw = 0.5 + 0.5 * sin(TIME * (2.0 + 4.0 * h12(si + 9.0)) + h12(si) * 30.0 + 50.0 * gBuild * gBuild * gBuild * (0.5 + h12(si + 4.0)));
    float sz = 0.18 + 0.25 * h12(si + 5.0) * h12(si + 6.0);
    float dust = keep * smoothstep(sz, sz * 0.4, sd) * (0.4 + 0.6 * tw) * (1.0 + 1.2 * highP() * ar);
    // occasional bright bokeh flares on the shell
    float fl = step(0.985 - 0.02 * gPeak, h12(si + 13.0)) * exp(-pow((rhod - 0.93) / 0.08, 2.0)) * smoothstep(1.2, 0.0, sd) * (0.6 + 0.4 * tw);
    light += (vec3(dust) + tint(h12(si + 3.0)) * fl * 1.5) * shellW * (1.0 + 0.6 * gDrop + 0.5 * gBuild);
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
