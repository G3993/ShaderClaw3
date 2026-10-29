/*{
  "DESCRIPTION": "Chandelier — a living, iridescent 3D glass chandelier that never stops morphing. In Auto Morph the glass flows continuously through ten forms, always mid-way between two of them with a gentle swirl as one melts into the next: Crystal Tiers, cascading rings of hanging prisms around a central column; Swan Glass, sweeping S-curved arms of amethyst glass; Prism Burst, crystal rods radiating from a glass sphere; Glass Loops, interlinked rings rolling through each other; Knot Ribbon, a twisting trefoil ribbon of glass around a glowing bead; Jellyfish, a pulsing glass bell trailing crystal-beaded tendrils; Helix Cascade, twin spiral ribbons dripping with prism drops; Orbiting Shards, a faceted gem core circled by tumbling crystal shards; Lotus Bloom, three rings of cupped glass petals opening and closing; and Gyroid Shell, a breathing lattice orb. The glass is truly see-through — every pixel looks through several layers of crystal with coloured absorption, rainbow dispersion, studio-softbox reflections and an oil-slick / soap-bubble thin-film sheen whose rainbow bands slide with the view angle and flow with the morph, plus warm inner light, caustic shimmer, glowing bulbs and twinkling star glints. Hang it on its chain and it sways like a pendulum; switch the cable off and it floats free, bobbing and drifting above a soft pool of its own light. Bass pushes the morph along and swells the glass, mids drive the spin and highs sparkle the glints. At rest it turns slowly with the bulbs dimmed; during a build the forms compress, spin and morph faster; on the drop the crystals burst outward in a flash of light, and the peak blazes with every bulb lit.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10], "LABELS": ["Auto Morph", "Crystal Tiers", "Swan Glass", "Prism Burst", "Glass Loops", "Knot Ribbon", "Jellyfish", "Helix Cascade", "Orbiting Shards", "Lotus Bloom", "Gyroid Shell"], "GROUP": "Chandelier" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 1.5, "MAX": 40.0, "DEFAULT": 6.0, "GROUP": "Chandelier" },
    { "NAME": "morphSpeed",  "LABEL": "Morph Speed",      "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 4.0, "DEFAULT": 1.0, "GROUP": "Chandelier" },
    { "NAME": "showCable",   "LABEL": "Show Cable (off = Floating)", "TYPE": "bool", "DEFAULT": true, "GROUP": "Chandelier" },
    { "NAME": "chSize",      "LABEL": "Size",             "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.5, "DEFAULT": 1.3, "GROUP": "Chandelier" },
    { "NAME": "spin",        "LABEL": "Spin",             "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Chandelier" },
    { "NAME": "swing",       "LABEL": "Sway / Drift",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Chandelier" },
    { "NAME": "organic",     "LABEL": "Organic Warp",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Chandelier" },
    { "NAME": "iridescence", "LABEL": "Iridescence",      "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Glass" },
    { "NAME": "tintSel",     "LABEL": "Glass Tint", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Follow Form", "Clear Crystal", "Amethyst", "Sapphire", "Custom"], "GROUP": "Glass" },
    { "NAME": "glassTint",   "LABEL": "Custom Tint",      "TYPE": "color", "DEFAULT": [1.0, 0.75, 0.9, 1.0], "GROUP": "Glass" },
    { "NAME": "dispersion",  "LABEL": "Rainbow Dispersion","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Glass" },
    { "NAME": "layers",      "LABEL": "Glass Depth",      "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 4.0, "DEFAULT": 3.0, "GROUP": "Glass" },
    { "NAME": "bulbGlow",    "LABEL": "Bulb Light",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "bulbColor",   "LABEL": "Bulb Colour",      "TYPE": "color", "DEFAULT": [1.0, 0.82, 0.55, 1.0], "GROUP": "Light" },
    { "NAME": "glints",      "LABEL": "Star Glints",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 2, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.04, 0.035, 0.05, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718
#define NFORMS 10.0

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

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
vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

// ── Show dynamics: rest → build → peak / drop ───────────────────────────
float gRest, gBuild, gPeak, gDrop, gLift;
void showDynamics() {
    float m = float(showState);
    float e = clamp(audioEnergy, 0.0, 1.0);
    float b = clamp(audioBuildup, 0.0, 1.0);
    float d = clamp(audioDrop, 0.0, 1.0);
    if (m > 0.5) {
        if (m < 1.5)      { e = 0.04; b = 0.0; d = 0.0; }
        else if (m < 2.5) { float r = fract(TIME / 8.0); e = 0.3 + 0.35 * r; b = r * r; d = 0.0; }
        else              { e = 1.0; b = 0.0; d = exp(-fract(TIME / 4.0) * 5.0); }
    }
    float k = dynamics;
    gBuild = b * k;
    gDrop  = d * k;
    gPeak  = smoothstep(0.5, 0.9, e) * k;
    gRest  = (1.0 - smoothstep(0.08, 0.4, e)) * (1.0 - b) * k;
    gLift  = mix(0.5, clamp(0.15 + e * 0.7 + gBuild * 0.25 + d * 0.3, 0.0, 1.0), k);
}

float gT, gBurst, gComp;
float gFa, gFb, gMix, gMorph, gSwirl, gFront, gCable, gTop;

float sdBox(vec3 p, vec3 b) { vec3 q = abs(p) - b; return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0); }
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) { vec3 pa = p - a, ba = b - a; float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0); return length(pa - ba * h) - r; }
float sdTorus(vec3 p, float R, float r) { return length(vec2(length(p.xz) - R, p.y)) - r; }
float sdOct(vec3 p, float s) { p = abs(p); return (p.x + p.y + p.z - s) * 0.57735; }
float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }

// polar fold: returns (radial, y, lateral) in the sector frame and the sector index
vec3 polar(vec3 p, float N, float off, out float idx) {
    float a = atan(p.z, p.x) + off;
    float sec = TAU / N;
    idx = floor((a + sec * 0.5) / sec);
    a = a - idx * sec;
    float r = length(p.xz);
    return vec3(r * cos(a), p.y, r * sin(a));
}

// ── 1 Crystal Tiers ──────────────────────────────────────────────────────
float fTiers(vec3 p) {
    float d = sdCapsule(p, vec3(0.0, 1.1, 0.0), vec3(0.0, -1.25, 0.0), 0.035);       // column
    d = min(d, length(p - vec3(0.0, -1.3, 0.0)) - 0.09);                             // finial drop
    float ang = atan(p.z, p.x);
    for (int i = 0; i < 6; i++) {
        float fi = float(i);
        float ty = (1.05 - fi * 0.4) * gComp;
        float rr = (0.32 + 0.82 * sin(PI * (fi + 0.7) / 6.4)) * (1.0 + 0.25 * gBurst);
        rr += 0.06 * organic * sin(ang * 3.0 + gT * 0.8 + fi * 1.3);
        float N = 14.0 + fi * 4.0;
        float idx;
        vec3 q = polar(p - vec3(0.0, ty, 0.0), N, gT * 0.25 * spin * (mod(fi, 2.0) < 0.5 ? 1.0 : -1.0) + fi * 0.2, idx);
        float L = (0.42 - 0.03 * fi) * (0.8 + 0.4 * h11(idx + fi * 13.0)) * (1.0 - 0.3 * gBuild);
        q.x -= rr;
        q.xz = rot(PI * 0.25) * q.xz;
        float prism = sdBox(q - vec3(0.0, -L * 0.5, 0.0), vec3(0.028, L * 0.5, 0.028)) - 0.004;
        prism = min(prism, length(q - vec3(0.0, -L - 0.03, 0.0)) - 0.035);
        float ring = length(vec2(length((p - vec3(0.0, ty, 0.0)).xz) - rr, p.y - ty)) - 0.022;
        d = min(d, min(prism, ring));
        if (fi > 0.5 && fi < 4.5) {
            float idx2;
            vec3 g = polar(p, 8.0, fi * 0.4, idx2);
            vec3 c = vec3(rr * 0.82, ty + 0.18, 0.0);
            float arc = abs(length(vec2(g.x - c.x, g.y - c.y)) - 0.24) - 0.012;
            arc = max(arc, max(abs(g.z) - 0.02, g.y - ty - 0.12));
            d = min(d, arc);
        }
    }
    return d;
}
// ── 2 Swan Glass ─────────────────────────────────────────────────────────
float fSwan(vec3 p) {
    p.y += 0.75;
    p /= 1.2;
    float d = length(p - vec3(0.0, 0.42, 0.0)) - 0.1;
    float cup = abs(length(p - vec3(0.0, 0.75, 0.0)) - 0.42) - 0.02;
    cup = max(cup, p.y - 0.75);
    d = min(d, cup);
    for (int k = 0; k < 2; k++) {
        float fk = float(k);
        float idx;
        float N = 8.0;
        vec3 q = polar(p, N, gT * 0.18 * spin + fk * PI / N, idx);
        float r = q.x * (1.0 - 0.2 * gBurst);
        float sway = 0.08 * organic * sin(gT * 0.9 + idx * 1.7 + fk);
        float reach = (fk < 0.5 ? 1.25 : 0.95) * (1.0 + 0.2 * gBurst);
        float t = clamp(r / reach, 0.0, 1.0);
        float g = 0.55 - 1.15 * t + 0.95 * t * t * t + sway * t - 0.15 * fk;
        float dg = (-1.15 + 2.85 * t * t + sway) / reach;
        float arm = abs(q.y - g) / sqrt(1.0 + dg * dg);
        float thick = 0.06 + 0.06 * sin(PI * t) * (fk < 0.5 ? 1.0 : 0.5);
        arm = max(length(vec2(arm, q.z * (fk < 0.5 ? 0.55 : 1.0))) - thick, max(-r, r - reach));
        d = smin(d, arm, 0.06);
    }
    return d * 1.2;
}
// ── 3 Prism Burst ────────────────────────────────────────────────────────
float fBurst(vec3 p) {
    p *= 1.25;
    float d = length(p) - 0.32;
    for (int j = 0; j < 4; j++) {
        float fj = float(j);
        float lat = (fj - 1.5) * 0.48 + 0.08 * organic * sin(gT * 0.5 + fj);
        float N = fj < 0.5 || fj > 2.5 ? 9.0 : 13.0;
        float idx;
        vec3 q = polar(p, N, gT * 0.2 * spin * (fj < 1.5 ? 1.0 : -0.7) + fj * 0.7, idx);
        q.xy = rot(-lat) * q.xy;
        float hh = h11(idx * 3.1 + fj * 17.0);
        float r0 = 0.4 + 0.5 * hh * (1.0 - 0.6 * gBuild);
        float L = (0.35 + 0.65 * h11(idx + fj * 5.0)) * (1.0 + 0.6 * gBurst) * (1.0 - 0.35 * gBuild);
        float w = 0.035 + 0.04 * h11(idx * 7.0 + fj);
        float rod = sdBox(q - vec3(r0 + L * 0.5 + 0.5 * gBurst, 0.0, 0.0), vec3(L * 0.5, w, w * (0.6 + 0.8 * hh))) - 0.006;
        d = min(d, rod);
    }
    return d / 1.25;
}
// ── 4 Glass Loops ────────────────────────────────────────────────────────
float fLoops(vec3 p) {
    float d = length(p - vec3(0.0, -0.05, 0.0)) - 0.16;
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        vec3 q = p;
        q.xz = rot(gT * 0.3 * spin + fi * TAU / 3.0) * q.xz;
        q.yz = rot(0.9 + 0.3 * sin(gT * 0.4 + fi) * organic) * q.yz;
        q.xy = rot(0.4 * fi) * q.xy;
        float R = (0.75 + 0.1 * fi) * (1.0 + 0.25 * gBurst);
        float tube = 0.1 + 0.05 * sin(atan(q.z, q.x) * 2.0 + gT + fi);
        d = smin(d, sdTorus(q, R, tube), 0.08);
    }
    return d;
}
// ── 5 Knot Ribbon: trefoil glass ribbon around a bead ───────────────────
float fKnot(vec3 p) {
    p.xz = rot(gT * 0.22 * spin) * p.xz;
    p.yz = rot(0.35 + 0.1 * sin(gT * 0.3) * organic) * p.yz;
    float R = 0.8 * (1.0 + 0.22 * gBurst) * (1.0 - 0.15 * gBuild);
    float a = atan(p.z, p.x);
    vec2 q = vec2(length(p.xz) - R, p.y);
    float d = length(p) - 0.17;
    for (int k = 0; k < 2; k++) {
        float ang = (a + TAU * float(k)) * 1.5 + gT * 0.3;
        float rr = 0.36 + 0.06 * organic * sin(a * 3.0 + gT);
        vec2 w = q - rr * vec2(cos(ang), sin(ang));
        w = rot(ang * 0.5 + gT * 0.2) * w;
        float rib = (length(w / vec2(0.15, 0.045)) - 1.0) * 0.045;
        d = smin(d, rib, 0.04);
    }
    return d * 0.75;
}
// ── 6 Jellyfish: glass bell with crystal-beaded tendrils ────────────────
float fJelly(vec3 p) {
    p.y -= 0.35;
    float pulse = sin(gT * 1.6);
    float Rb = 0.82 * (1.0 - 0.05 * pulse * organic) * (1.0 + 0.2 * gBurst) * (1.0 - 0.1 * gBuild);
    float ang = atan(p.z, p.x);
    float shell = (abs(length(p * vec3(1.0, 1.35 + 0.08 * pulse * organic, 1.0)) - Rb) - 0.035) * 0.72;
    float rim = -0.05 + 0.05 * sin(ang * 8.0 + gT);
    shell = max(shell, rim - p.y);
    float d = min(shell, (length(p * vec3(1.0, 1.7, 1.0) - vec3(0.0, 0.2, 0.0)) - 0.32) * 0.58);
    // oral arms: three twisting strands under the bell
    vec3 o = p; o.xz = rot(p.y * 4.0 + gT * 0.7) * o.xz;
    float oi;
    vec3 oq = polar(o, 3.0, 0.0, oi);
    float arm = length(vec2(oq.x - 0.1 - 0.05 * sin(p.y * 5.0 + gT), oq.z)) - 0.035;
    d = min(d, max(arm * 0.6, max(p.y, -1.0 - p.y)));
    // tendrils
    float idx;
    vec3 q = polar(p, 12.0, gT * 0.1 * spin, idx);
    float L = (0.75 + 0.75 * h11(idx * 1.7 + 3.0)) * (1.0 - 0.3 * gBuild) * (1.0 + 0.25 * gBurst);
    float y0 = -0.02;
    float s = y0 - p.y;
    float xo = Rb * 0.9 - 0.1 * s + 0.09 * organic * sin(s * 3.5 - gT * 2.0 + idx);
    float zo = 0.06 * organic * cos(s * 3.0 - gT * 1.7 + idx * 2.0);
    vec2 tq = vec2(q.x - xo, q.z - zo);
    float ten = length(tq) - 0.016;
    float sy = mod(s, 0.2) - 0.1;
    ten = min(ten, sdOct(vec3(tq.x, sy * 0.6, tq.y), 0.055));
    ten = max(ten * 0.7, max(-s, s - L));
    return min(d, ten);
}
// ── 7 Helix Cascade: twin spiral ribbons with prism drops ───────────────
float helixR(float y) {
    float t01 = clamp((1.0 - y) / 2.3, 0.0, 1.0);
    return (0.12 + 1.0 * sin(PI * t01) * (1.0 - 0.3 * t01)) * (1.0 + 0.25 * gBurst) * (1.0 - 0.2 * gBuild) + 0.05 * organic * sin(y * 4.0 + gT);
}
float fHelix(vec3 p) {
    p.xz = rot(gT * 0.3 * spin) * p.xz;
    float pitch = 0.6 * gComp;
    float R = helixR(p.y);
    float a = atan(p.z, p.x);
    float d = sdCapsule(p, vec3(0.0, 1.0, 0.0), vec3(0.0, -1.3, 0.0), 0.03);
    d = min(d, length(p - vec3(0.0, -1.33, 0.0)) - 0.07);
    float slope = TAU * max(R, 0.1) / sqrt(TAU * TAU * R * R + pitch * pitch);
    float r = length(p.xz) - R;
    for (int k = 0; k < 2; k++) {
        float u = p.y / pitch + a / TAU + float(k) * 0.5 + gT * 0.15;
        float dy = (fract(u) - 0.5) * pitch * slope;
        vec2 w = abs(vec2(r, dy)) - vec2(0.075, 0.012);
        d = min(d, length(max(w, 0.0)) + min(max(w.x, w.y), 0.0) - 0.008);
    }
    d = max(d, max(p.y - 1.02, -1.42 - p.y));
    // prism drops hanging under one ribbon
    float idx;
    float off = 0.0;
    vec3 q = polar(p, 16.0, off, idx);
    float ac = idx * TAU / 16.0 - off;
    float m = floor((p.y + 0.12) / pitch + ac / TAU + gT * 0.15);
    float yb = pitch * (m + 0.5 - ac / TAU - gT * 0.15);
    float drop = sdOct((q - vec3(helixR(yb), yb - 0.14, 0.0)) * vec3(1.0, 0.45, 1.0), 0.06);
    drop = max(drop, max(yb - 1.0, -1.25 - yb));
    return min(d, drop);
}
// ── 8 Orbiting Shards: gem core with tumbling crystal shards ────────────
float fShards(vec3 p) {
    vec3 c = p;
    c.xz = rot(gT * 0.4 * spin) * c.xz; c.xy = rot(0.35) * c.xy;
    vec3 ac = abs(c);
    float rc = 0.3 * (1.0 + 0.3 * gBurst);
    float dod = max(max(dot(ac, vec3(0.851, 0.526, 0.0)), dot(ac, vec3(0.0, 0.851, 0.526))), dot(ac, vec3(0.526, 0.0, 0.851))) - rc;
    float ico = max(max(dot(ac, vec3(0.577)), dot(ac, vec3(0.0, 0.357, 0.934))), max(dot(ac, vec3(0.934, 0.0, 0.357)), dot(ac, vec3(0.357, 0.934, 0.0)))) - rc * 1.03;
    float d = max(dod, ico);
    for (int k = 0; k < 3; k++) {
        float fk = float(k);
        vec3 q = p;
        q.yz = rot(0.5 + fk * 0.9 + 0.15 * sin(gT * 0.3 + fk) * organic) * q.yz;
        q.xy = rot(fk * 1.1) * q.xy;
        float R = (0.68 + 0.3 * fk) * (1.0 + 0.35 * gBurst) * (1.0 - 0.25 * gBuild);
        float N = 7.0 + fk * 3.0;
        float idx;
        vec3 s = polar(q, N, gT * (0.5 - 0.12 * fk) * (spin + 0.3) * (mod(fk, 2.0) < 0.5 ? 1.0 : -1.0), idx);
        s.x -= R;
        s.y += 0.08 * sin(gT + idx * 1.9);
        s.xy = rot(gT * 0.7 + idx * 2.0) * s.xy;
        s.xz = rot(idx * 1.3 + fk) * s.xz;
        float sz = 0.055 + 0.06 * h11(idx * 2.3 + fk * 11.0);
        d = min(d, sdOct(s * vec3(1.0, 0.4, 1.0), sz));
    }
    return d;
}
// ── 9 Lotus Bloom: three rings of cupped glass petals ───────────────────
float fLotus(vec3 p) {
    p.y += 0.2;
    p.yz = rot(0.5) * p.yz;
    float bloom = clamp(0.6 + 0.22 * sin(gT * 0.6) * organic + 0.35 * gBurst - 0.3 * gBuild, 0.1, 1.2);
    float d = length(p * vec3(1.0, 1.3, 1.0) - vec3(0.0, 0.05, 0.0)) - 0.16;
    d = min(d, sdCapsule(p, vec3(0.0, -0.05, 0.0), vec3(0.0, -0.9, 0.0), 0.03));
    d = min(d, sdOct((p - vec3(0.0, -1.0, 0.0)) * vec3(1.0, 0.5, 1.0), 0.08));
    for (int k = 0; k < 3; k++) {
        float fk = float(k);
        float N = 6.0 + fk * 2.0;
        float idx;
        vec3 q = polar(p, N, fk * 0.45 + gT * 0.08 * spin * (fk == 1.0 ? -1.0 : 1.0), idx);
        float L = 0.48 + fk * 0.24;
        float e = PI * 0.5 - (0.12 + fk * 0.36) * bloom - 0.08 * sin(gT + idx + fk);
        q.x -= 0.08 + 0.05 * fk;
        q.xy = rot(e) * q.xy;
        float xn = clamp(q.x / L, 0.0, 1.0);
        q.y -= 1.6 * q.z * q.z - 0.3 * xn * xn;
        float wdt = (0.11 + 0.05 * fk) * (0.35 + 0.9 * sin(PI * min(xn * 1.1, 1.0)));
        vec2 pe2 = vec2(q.x - L * 0.5, q.z);
        float e2 = (length(pe2 / vec2(L * 0.5, max(wdt, 0.05))) - 1.0) * max(wdt, 0.05);
        vec2 pw = vec2(e2, abs(q.y) - 0.024);
        float pet = length(max(pw, 0.0)) + min(max(pw.x, pw.y), 0.0) - 0.006;
        d = min(d, pet * 0.6);
    }
    return d;
}
// ── 10 Gyroid Shell: breathing lattice orb ──────────────────────────────
float fGyroid(vec3 p) {
    p.xz = rot(gT * 0.2 * spin) * p.xz;
    float R = (1.0 + 0.25 * gBurst) * (1.0 - 0.15 * gBuild) * (1.0 + 0.04 * sin(gT * 1.2) * organic);
    float shell = abs(length(p * vec3(1.0, 0.88, 1.0)) - R) - 0.11;
    vec3 g = p * 4.2 + vec3(0.0, gT * 0.4, gT * 0.15);
    float gy = dot(sin(g), cos(g.yzx));
    float lat = (abs(gy) - 0.22) / 7.0;
    float d = max(shell, lat);
    float core = abs(length(p) - 0.36) - 0.025;
    float g2 = (abs(dot(sin(g * 1.8), cos(g.zxy * 1.8))) - 0.35) / 12.0;
    d = min(d, max(core, g2));
    return d;
}

float formSDF(vec3 p, float k) {
    if (k < 0.5) return fTiers(p);
    if (k < 1.5) return fSwan(p);
    if (k < 2.5) return fBurst(p);
    if (k < 3.5) return fLoops(p);
    if (k < 4.5) return fKnot(p);
    if (k < 5.5) return fJelly(p);
    if (k < 6.5) return fHelix(p);
    if (k < 7.5) return fShards(p);
    if (k < 8.5) return fLotus(p);
    return fGyroid(p);
}
// where the cable meets each form (object space, y)
float cableTop(float k) {
    if (k < 0.5) return 1.1;
    if (k < 1.5) return -0.25;
    if (k < 2.5) return 0.22;
    if (k < 3.5) return 0.08;
    if (k < 4.5) return 0.14;
    if (k < 5.5) return 0.93;
    if (k < 6.5) return 1.0;
    if (k < 7.5) return 0.28;
    if (k < 8.5) return 0.0;
    return 1.0;
}
// bulb ring per form: (radius, height, vertical scatter)
vec3 bulbRing(float k) {
    if (k < 0.5) return vec3(1.1, 0.2, 0.0);
    if (k < 1.5) return vec3(1.45, -0.4, 0.0);
    if (k < 3.5) return vec3(0.5, 0.0, 0.6);
    if (k < 4.5) return vec3(0.8, 0.0, 0.5);
    if (k < 5.5) return vec3(0.6, 0.45, 0.2);
    if (k < 6.5) return vec3(0.55, -0.1, 1.6);
    if (k < 7.5) return vec3(0.95, 0.0, 0.9);
    if (k < 8.5) return vec3(0.4, 0.05, 0.3);
    return vec3(0.5, 0.0, 0.9);
}
vec3 glassTintC(float k) {
    float t = float(tintSel);
    if (t > 3.5) return glassTint.rgb;
    if (t > 2.5) return vec3(0.5, 0.65, 1.0);
    if (t > 1.5) return vec3(0.9, 0.6, 1.0);
    if (t > 0.5) return vec3(1.0);
    if (k > 0.5 && k < 1.5) return vec3(0.9, 0.6, 1.0);     // swan: amethyst
    if (k > 3.5 && k < 4.5) return vec3(0.7, 0.95, 1.0);    // knot: aqua
    if (k > 4.5 && k < 5.5) return vec3(0.75, 0.75, 1.0);   // jelly: pale violet
    if (k > 5.5 && k < 6.5) return vec3(1.0, 0.9, 0.75);    // helix: champagne
    if (k > 7.5 && k < 8.5) return vec3(1.0, 0.75, 0.85);   // lotus: rose
    if (k > 8.5) return vec3(0.75, 1.0, 0.9);               // gyroid: seafoam
    return vec3(1.0);
}

// chain links + stem above the fixture
float cableSDF(vec3 p) {
    float y0 = gTop;
    float d = sdCapsule(p, vec3(0.0, y0 - 0.02, 0.0), vec3(0.0, y0 + 0.3, 0.0), 0.026);
    d = min(d, length(p - vec3(0.0, y0 + 0.3, 0.0)) - 0.055);
    float sp = 0.09;
    float yy = p.y - y0 - 0.34;
    for (int i = 0; i < 2; i++) {
        float id = floor(yy / sp) + float(i);
        vec3 q = vec3(p.x, yy - (id + 0.5) * sp, p.z);
        if (mod(id, 2.0) > 0.5) q.xz = q.zx;
        float l2 = length(vec2(q.x, max(abs(q.y) - 0.022, 0.0))) - 0.03;
        float link = length(vec2(l2, q.z)) - 0.009;
        d = min(d, max(link, -yy));
    }
    return d;
}

// morph wave: the next form pours in from the top through a rippling front
float waveW(vec3 p) {
    float yy = p.y + 0.2 * sin(atan(p.z, p.x) * 3.0 + p.y * 2.0 + gT * 0.8);
    return smoothstep(gFront - 0.32, gFront + 0.32, yy);
}
float map(vec3 p) {
    p /= chSize;
    float bnd = length(p - vec3(0.0, -0.15, 0.0)) - 2.15;
    float d = bnd;
    if (bnd < 0.3) {
        vec3 q = p;
        float lw = waveW(p);
        float band = 4.0 * lw * (1.0 - lw);
        q.xz = rot(band * gSwirl) * q.xz;
        d = 0.0;
        for (int i = 0; i < 2; i++) {
            float w = i == 0 ? 1.0 - lw : lw;
            if (w < 0.001) continue;
            d += w * formSDF(q, i == 0 ? gFa : gFb);
        }
        d *= 1.0 - 0.35 * band;
    }
    if (gCable > 0.5) d = min(d, cableSDF(p));
    return d * chSize;
}
vec3 calcN(vec3 p) {
    vec3 n = vec3(0.0);
    for (int k = 0; k < 4; k++) {
        float fk = float(k);
        vec3 e = vec3(fk == 0.0 || fk == 3.0 ? 1.0 : -1.0, fk > 1.5 ? 1.0 : -1.0, fk == 1.0 || fk == 3.0 ? 1.0 : -1.0);
        n += e * map(p + e * 0.0012);
    }
    return normalize(n);
}

// studio environment the glass reflects and refracts
vec3 env(vec3 d) {
    vec3 c = vec3(0.012, 0.01, 0.018) * (1.0 + d.y);
    c += vec3(1.0) * 2.4 * smoothstep(0.25, 0.42, d.y) * smoothstep(0.75, 0.2, abs(d.x + 0.35)) * step(0.0, -d.z + 0.6);   // softbox
    c += vec3(0.9, 0.95, 1.0) * 1.5 * exp(-pow((d.x - 0.75) * 7.0, 2.0)) * smoothstep(-0.6, 0.3, d.y);                   // strip light
    c += vec3(0.8, 0.9, 1.0) * 0.9 * exp(-pow((d.x + 0.9) * 9.0, 2.0)) * smoothstep(-0.3, 0.4, d.y) * step(0.0, d.z);   // back rim strip
    c += bulbColor.rgb * 0.6 * exp(-pow(d.y + 0.2, 2.0) * 6.0) * (0.4 + 0.8 * gLift);                                    // warm room glow
    c += (0.5 + 0.5 * cos(TAU * (d.y * 1.5 + d.x * 0.5 + vec3(0.0, 0.33, 0.67)))) * 0.3 * smoothstep(0.5, 0.0, abs(d.y - 0.05));
    return c;
}
// thin-film interference (soap bubble / oil slick): thickness in microns
vec3 thinFilm(float cosI, float thick) {
    float ct = sqrt(1.0 - (1.0 - cosI * cosI) / 1.77);
    float opd = 2.0 * 1.33 * thick * ct;
    vec3 f = 0.5 + 0.5 * cos(TAU * opd / vec3(0.65, 0.54, 0.45));
    f *= f;
    return max(mix(vec3(dot(f, vec3(0.333))), f, 1.7), 0.0) * 1.5;
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;
    gT = TIME * 0.5 + (audioMidTime * 0.4 + audioBassTime * 0.3) * ar + 2.0 * gBuild * gBuild;
    gBurst = gDrop * 0.55 + 0.08 * bassP() * ar;
    gComp = 1.0 - 0.22 * gBuild + 0.05 * gDrop;
    gCable = showCable ? 1.0 : 0.0;

    // constant morph: the blend weight never rests; bass, build and drop push it along
    float ph = (TIME / morphPeriod + audioBassTime * 0.12 * ar) * morphSpeed + 0.9 * gBuild * gBuild + 0.3 * gDrop + 3.0;
    float fw = fract(ph);
    float me = fw - 0.55 * sin(TAU * fw) / TAU;          // eased but always moving
    float fs = float(formSel);
    gFa = fs > 0.5 ? fs - 1.0 : mod(floor(ph), NFORMS);
    gFb = fs > 0.5 ? gFa : mod(floor(ph) + 1.0, NFORMS);
    gMix = fs > 0.5 ? 0.0 : me;
    gMorph = fs > 0.5 ? gT * 0.1 : ph;
    gSwirl = 0.45 * min(organic, 1.0);
    gFront = fs > 0.5 ? 9.0 : mix(1.3, -1.55, me);
    float ta = cableTop(gFa), tb = cableTop(gFb);
    gTop = mix(mix(ta, tb, gMix), min(ta, tb), smoothstep(0.0, 0.12, gMix) * smoothstep(1.0, 0.88, gMix));
    vec3 tint = mix(glassTintC(gFa), glassTintC(gFb), gMix);
    vec3 br = mix(bulbRing(gFa), bulbRing(gFb), gMix);

    float floating = 1.0 - gCable;
    // camera: slightly below, looking up at the fixture (more level when floating)
    vec3 ro = vec3(0.0, -0.1, 6.4 - 0.5 * gBuild + 0.4 * gRest);
    vec3 rd = normalize(vec3(sp, -1.75));
    rd.yz = rot(0.05 - 0.07 * floating) * rd.yz;
    vec3 ro0 = ro, rd0 = rd;
    // hanging: pendulum about the chain anchor; floating: bob, drift and slow tilt about the centre
    float sw, yaw, tilt;
    vec3 pivot;
    if (gCable > 0.5) {
        sw = swing * (0.05 * sin(TIME * 0.6) + 0.03 * sin(TIME * 1.1)) + 0.02 * gDrop * sin(TIME * 9.0);
        yaw = 0.25 * sin(TIME * 0.21) * swing;
        tilt = 0.0;
        pivot = vec3(0.0, 2.6, 0.0);
    } else {
        sw = swing * 0.06 * sin(TIME * 0.43 + 1.0) + 0.02 * gDrop * sin(TIME * 9.0);
        yaw = 0.3 * sin(TIME * 0.17) * swing;
        tilt = swing * 0.07 * sin(TIME * 0.31);
        pivot = vec3(0.0, -0.15, 0.0);
        ro -= vec3(0.12 * swing * sin(TIME * 0.23), 0.32 + 0.1 * swing * sin(TIME * 0.7) + 0.05 * swing * sin(TIME * 1.3), 0.0);
    }
    ro -= pivot; ro.xy = rot(sw) * ro.xy; ro.yz = rot(tilt) * ro.yz; ro.xz = rot(yaw) * ro.xz; ro += pivot;
    rd.xy = rot(sw) * rd.xy; rd.yz = rot(tilt) * rd.yz; rd.xz = rot(yaw) * rd.xz;
    ro.x += 0.01 * smoothstep(0.7, 1.0, gBuild) * sin(TIME * 53.0);

    vec3 bg = backdrop(uv);
    // floating: a soft pool of light on the floor below, so it reads as levitating
    if (floating > 0.5 && rd0.y < 0.0) {
        float tp = (-1.6 * chSize - ro0.y) / rd0.y;
        vec3 fp = ro0 + rd0 * tp;
        vec2 fd = fp.xz - vec2(0.12 * swing * sin(TIME * 0.23), 0.0);
        float rr = length(fd) / chSize;
        float bob = 0.1 * swing * sin(TIME * 0.7);
        float pool = exp(-rr * rr * (1.3 + 2.0 * bob)) * (0.3 + 0.55 * gLift + 0.8 * gDrop);
        vec3 pc = mix(bulbColor.rgb, thinFilm(0.8, 0.4 + 0.25 * rr + 0.05 * sin(gMorph)), 0.35 * iridescence);
        float ripple = 0.5 + 0.5 * sin(rr * 9.0 - gT * 1.5 + sin(atan(fd.y, fd.x) * 5.0 + gT) * 0.6);
        bg += pc * pool * (0.22 + 0.1 * ripple * ripple) * bulbGlow * smoothstep(12.0, 5.0, tp);
    }

    vec3 col = vec3(0.0);
    vec3 trans = vec3(1.0);
    float alpha = 0.0;
    float t = 0.5;
    float glowAcc = 0.0;
    float NL = floor(layers);
    float hits = 0.0;
    float inside = 0.0;
    float tIn = 0.0;
    float pixA = 1.0 / (1.75 * R.y);
    float ir = iridescence;
    vec3 Ld = normalize(vec3(-0.4, 0.8, 0.6));
    for (int i = 0; i < 150; i++) {
        vec3 p = ro + rd * t;
        float d = map(p);
        float ad = inside > 0.5 ? -d : d;
        float pe = max(0.0006, t * pixA * 0.7);
        if (ad < pe) {
            vec3 n = calcN(p);
            if (inside > 0.5) n = -n;
            float cosI = clamp(dot(n, -rd), 0.0, 1.0);
            float F = 0.04 + 0.96 * pow(1.0 - cosI, 5.0);
            vec3 rdir = reflect(rd, n);
            vec3 refl = env(rdir);
            float disp = 0.035 * dispersion;
            vec3 refr = vec3(env(refract(rd, n, 1.0 / (1.45 - disp))).r,
                             env(refract(rd, n, 1.0 / 1.45)).g,
                             env(refract(rd, n, 1.0 / (1.45 + disp))).b);
            vec3 op = p / chSize;
            float th = 0.28 + 0.55 * (0.5 + 0.5 * sin(dot(op, vec3(1.7, 2.3, 1.1)) * 2.0 + gMorph * 2.2 + gT * 0.25))
                     + 0.18 * sin(op.y * 6.0 - gMorph * 3.0 + n.x * 2.0) + 0.12 * n.y;
            vec3 film = thinFilm(cosI, th);
            float ew = inside > 0.5 ? 0.5 : 1.0;
            float lwS = waveW(op);
            float seam = 4.0 * lwS * (1.0 - lwS);
            vec3 c = refr * tint * 0.26 * (1.0 - F);
            c += refl * mix(vec3(1.0), film, ir * 0.9) * (0.06 + 1.5 * F);
            c += film * ir * (0.05 + 0.55 * pow(1.0 - cosI, 2.0) + 0.6 * seam) * (0.55 + 0.5 * gLift + 0.3 * midP() * ar);
            // inner warm light and caustic shimmer passing through the glass
            float caus = pow(abs(sin(op.x * 9.0 + gT) * sin(op.y * 11.0 - gT * 0.7) * sin(op.z * 8.0 + gT * 0.5)), 0.5);
            c += bulbColor.rgb * tint * (0.04 + 0.18 * gLift) * bulbGlow * (1.0 - F) * (0.6 + 0.9 * caus * caus) * smoothstep(1.8, 0.0, length(op.xz));
            float spec = pow(clamp(dot(rdir, Ld), 0.0, 1.0), 120.0) * 2.4 + pow(clamp(dot(rdir, normalize(vec3(0.7, 0.3, 0.6))), 0.0, 1.0), 60.0) * 0.8;
            c += mix(vec3(1.0), film, 0.4 * ir) * spec * (1.0 + highP() * ar);
            col += c * trans * ew;
            alpha = max(alpha, 0.85);
            if (inside < 0.5) {
                trans *= 0.7 + 0.22 * (1.0 - F);
                hits += 1.0;
                tIn = t;
                inside = 1.0;
            } else {
                // coloured absorption through the thickness of the glass
                float seg = (t - tIn) / chSize;
                trans *= exp(-seg * (vec3(1.0) - tint * 0.85) * 3.0 - seg * 0.35);
                inside = 0.0;
                if (hits >= NL) break;
            }
            t += max(0.01, pe * 3.0);
            continue;
        }
        float stp = inside > 0.5 ? max(ad * 0.9, 0.006) : max(ad * 0.8, 0.0015);
        if (inside < 0.5) glowAcc += exp(-max(d, 0.0) * 26.0) * min(stp, 0.08) * 0.3;
        t += stp;
        if (t > 12.0) break;
    }
    // bulbs: glowing points around the fixture (object space)
    float bulbs = 0.0;
    for (int i = 0; i < 10; i++) {
        float fi = float(i);
        float a = fi / 10.0 * TAU + gT * 0.25 * spin;
        vec3 bp = vec3(cos(a) * br.x, br.y + br.z * (h11(fi) - 0.5), sin(a) * br.x) * chSize;
        vec3 w = bp - ro;
        float along = dot(w, rd);
        float dist = length(w - rd * along) / chSize;
        float on = smoothstep(0.0, 1.0, gLift * 1.3 - h11(fi * 3.7) * 0.6) + gDrop;
        bulbs += exp(-dist * dist * 900.0) * 1.2 * on + exp(-dist * 9.0) * 0.06 * on;
    }
    col += bulbColor.rgb * bulbs * bulbGlow;
    col += mix(bulbColor.rgb, vec3(0.8, 0.85, 1.0), 0.5) * glowAcc * (0.4 + 0.9 * gLift + 1.5 * gDrop) * bulbGlow;
    // star glints: 4-point stars that twinkle on the crystal
    for (int i = 0; i < 9; i++) {
        float fi = float(i);
        float rate = 0.4 + 0.3 * h11(fi);
        float cyc = floor(TIME * rate + h11(fi * 2.0) * 5.0);
        vec2 gp = (vec2(h11(fi * 7.1 + cyc), h11(fi * 3.9 + cyc * 1.3)) - 0.5) * vec2(0.6, 0.7) * chSize;
        float life = sin(PI * fract(TIME * rate + h11(fi * 2.0) * 5.0));
        vec2 dd = abs(sp - gp);
        float star = exp(-dd.x * 900.0) * exp(-dd.y * 45.0) + exp(-dd.y * 900.0) * exp(-dd.x * 45.0);
        star += exp(-dot(dd, dd) * 4000.0) * 2.0;
        vec3 sc = h11(fi * 5.5) < 0.5 ? vec3(1.0, 0.85, 0.55) : vec3(0.8, 0.9, 1.0);
        col += sc * star * life * 0.6 * glints * (0.35 + 0.8 * gLift + highP() * ar) * (0.4 + 0.6 * alpha);
    }
    col = bg * trans + col;
    col += vec3(1.0) * gDrop * gDrop * 0.15 * alpha;
    col *= 0.5 + 0.26 * gLift + 0.1 * audioBeatPulse * ar;
    col = (col * (2.51 * col + 0.03)) / (col * (2.43 * col + 0.59) + 0.14);   // filmic
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(alpha + lum(col) * 0.5, 0.0, 1.0) : 1.0);
}
