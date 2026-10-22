/*{
  "DESCRIPTION": "Chandelier — a living 3D glass chandelier hanging in the dark, morphing between four forms: Crystal Tiers, cascading rings of hanging prisms around a central column that breathe and counter-rotate; Swan Glass, sweeping arms of tinted glass that curl out and up with glowing bulbs at their tips; Prism Burst, crystal rods radiating from a glass sphere at every length and angle; and Glass Loops, interlinked iridescent rings that roll through each other. The glass is truly see-through: every pixel looks through several layers of crystal, with rainbow dispersion, fresnel reflections of a studio softbox, warm inner light and star glints that twinkle across it. The whole fixture sways gently on its chain. At rest it turns slowly with the bulbs dimmed; during a build the tiers compress and spin faster while the rods draw in; on the drop the crystals burst outward in a flash of light, then the peak blazes with every bulb lit.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Crystal Tiers", "Swan Glass", "Prism Burst", "Glass Loops"], "GROUP": "Chandelier" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 12.0, "GROUP": "Chandelier" },
    { "NAME": "chSize",      "LABEL": "Size",             "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Chandelier" },
    { "NAME": "spin",        "LABEL": "Spin",             "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Chandelier" },
    { "NAME": "swing",       "LABEL": "Sway",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Chandelier" },
    { "NAME": "organic",     "LABEL": "Organic Warp",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Chandelier" },
    { "NAME": "tintSel",     "LABEL": "Glass Tint", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Follow Form", "Clear Crystal", "Amethyst", "Sapphire", "Custom"], "GROUP": "Glass" },
    { "NAME": "glassTint",   "LABEL": "Custom Tint",      "TYPE": "color", "DEFAULT": [1.0, 0.75, 0.9, 1.0], "GROUP": "Glass" },
    { "NAME": "dispersion",  "LABEL": "Rainbow Dispersion","TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Glass" },
    { "NAME": "layers",      "LABEL": "Glass Depth",      "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 4.0, "DEFAULT": 3.0, "GROUP": "Glass" },
    { "NAME": "bulbGlow",    "LABEL": "Bulb Light",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "bulbColor",   "LABEL": "Bulb Colour",      "TYPE": "color", "DEFAULT": [1.0, 0.82, 0.55, 1.0], "GROUP": "Light" },
    { "NAME": "glints",      "LABEL": "Star Glints",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.12, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
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

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }

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
float gFa, gFb, gMix;

float sdBox(vec3 p, vec3 b) { vec3 q = abs(p) - b; return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0); }
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) { vec3 pa = p - a, ba = b - a; float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0); return length(pa - ba * h) - r; }
float sdTorus(vec3 p, float R, float r) { return length(vec2(length(p.xz) - R, p.y)) - r; }
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

// ── Form 1: Crystal Tiers ────────────────────────────────────────────────
float fTiers(vec3 p) {
    float d = sdCapsule(p, vec3(0.0, 2.6, 0.0), vec3(0.0, -1.25, 0.0), 0.035);       // chain + column
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
        // pointed tip
        prism = min(prism, length(q - vec3(0.0, -L - 0.03, 0.0)) - 0.035);
        float ring = length(vec2(length((p - vec3(0.0, ty, 0.0)).xz) - rr, p.y - ty)) - 0.022;
        d = min(d, min(prism, ring));
        // garland arcs between tiers (only on the wider tiers)
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
// ── Form 2: Swan Glass ───────────────────────────────────────────────────
float fSwan(vec3 p) {
    p.y += 0.75;
    p /= 1.2;
    float d = sdCapsule(p, vec3(0.0, 2.6, 0.0), vec3(0.0, 0.55, 0.0), 0.05);
    // the cup at the top
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
        // S-shaped arm: dips down and curls back up at the tip
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
// ── Form 3: Prism Burst ──────────────────────────────────────────────────
float fBurst(vec3 p) {
    p *= 1.25;
    float d = length(p) - 0.32;
    d = min(d, sdCapsule(p, vec3(0.0, 2.6, 0.0), vec3(0.0, 0.3, 0.0), 0.02));
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
// ── Form 4: Glass Loops ──────────────────────────────────────────────────
float fLoops(vec3 p) {
    float d = sdCapsule(p, vec3(0.0, 2.6, 0.0), vec3(0.0, 0.95, 0.0), 0.02);
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
    d = min(d, length(p - vec3(0.0, -0.05, 0.0)) - 0.16);
    return d;
}

float form(float k, vec3 p) {
    if (k < 0.5) return fTiers(p);
    if (k < 1.5) return fSwan(p);
    if (k < 2.5) return fBurst(p);
    return fLoops(p);
}
float map(vec3 p) {
    p /= chSize;
    float d = form(gFa, p);
    if (gMix > 0.001) d = mix(d, form(gFb, p), gMix);
    return d * chSize;
}
vec3 calcN(vec3 p) {
    vec2 e = vec2(0.0015, -0.0015);
    return normalize(e.xyy * map(p + e.xyy) + e.yyx * map(p + e.yyx) + e.yxy * map(p + e.yxy) + e.xxx * map(p + e.xxx));
}

// studio environment the glass reflects and refracts
vec3 env(vec3 d) {
    vec3 c = vec3(0.01);
    c += vec3(1.0) * 2.2 * smoothstep(0.25, 0.4, d.y) * smoothstep(0.75, 0.2, abs(d.x + 0.35)) * step(0.0, -d.z + 0.6);   // softbox
    c += vec3(0.9, 0.95, 1.0) * 1.4 * exp(-pow((d.x - 0.75) * 7.0, 2.0)) * smoothstep(-0.6, 0.3, d.y);                   // strip light
    c += bulbColor.rgb * 0.6 * exp(-pow(d.y + 0.2, 2.0) * 6.0) * (0.4 + 0.8 * gLift);                                    // warm room glow
    // thin rainbow band
    c += (0.5 + 0.5 * cos(TAU * (d.y * 1.5 + d.x * 0.5 + vec3(0.0, 0.33, 0.67)))) * 0.35 * smoothstep(0.5, 0.0, abs(d.y - 0.05));
    return c;
}
vec3 glassTintC(float k) {
    float t = float(tintSel);
    if (t > 3.5) return glassTint.rgb;
    if (t > 2.5) return vec3(0.5, 0.65, 1.0);
    if (t > 1.5) return vec3(0.9, 0.6, 1.0);
    if (t > 0.5) return vec3(1.0);
    if (k > 0.5 && k < 1.5) return vec3(0.9, 0.6, 1.0);           // swan: amethyst
    return vec3(1.0);
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

    float ph = TIME / morphPeriod;
    float fs = float(formSel);
    gFa = fs > 0.5 ? fs - 1.0 : mod(floor(ph), 4.0);
    gFb = fs > 0.5 ? gFa : mod(floor(ph) + 1.0, 4.0);
    gMix = fs > 0.5 ? 0.0 : smoothstep(0.65, 1.0, fract(ph));
    vec3 tint = mix(glassTintC(gFa), glassTintC(gFb), gMix);

    // camera: slightly below, looking up at the fixture
    vec3 ro = vec3(0.0, -0.1, 6.4 - 0.5 * gBuild + 0.4 * gRest);
    vec3 rd = normalize(vec3(sp, -1.75));
    rd.yz = rot(0.05) * rd.yz;
    // pendulum sway: rotate the world around the chain anchor
    float sw = swing * (0.05 * sin(TIME * 0.6) + 0.03 * sin(TIME * 1.1)) + 0.02 * gDrop * sin(TIME * 9.0);
    float yaw = 0.25 * sin(TIME * 0.21) * swing;
    vec3 anchor = vec3(0.0, 2.6, 0.0);
    ro -= anchor; ro.xy = rot(sw) * ro.xy; ro.xz = rot(yaw) * ro.xz; ro += anchor;
    rd.xy = rot(sw) * rd.xy; rd.xz = rot(yaw) * rd.xz;
    // build tremor
    ro.x += 0.01 * smoothstep(0.7, 1.0, gBuild) * sin(TIME * 53.0);

    vec3 bg = backdrop(uv);
    vec3 col = vec3(0.0);
    float trans = 1.0;
    float alpha = 0.0;
    float t = 0.5;
    float glowAcc = 0.0;
    float NL = floor(layers);
    float hits = 0.0;
    for (int i = 0; i < 110; i++) {
        vec3 p = ro + rd * t;
        float d = map(p);
        glowAcc += exp(-max(d, 0.0) * 18.0) * 0.012;                 // light caught in the crystals
        if (d < 0.0015) {
            vec3 n = calcN(p);
            float fres = pow(1.0 - clamp(dot(n, -rd), 0.0, 1.0), 3.0);
            vec3 refl = env(reflect(rd, n));
            // dispersion: each channel refracts a little differently
            float disp = 0.03 * dispersion;
            vec3 refr = vec3(env(refract(rd, n, 1.0 / (1.45 - disp))).r,
                             env(refract(rd, n, 1.0 / 1.45)).g,
                             env(refract(rd, n, 1.0 / (1.45 + disp))).b);
            // rainbow film on edges
            vec3 film = 0.5 + 0.5 * cos(TAU * (dot(n, rd) * 2.0 + gT * 0.05 + vec3(0.0, 0.33, 0.67)));
            vec3 c = refr * tint * 0.28 + refl * (0.08 + 0.9 * fres) + film * fres * 0.7 * dispersion;
            // inner warm light (the chandelier's own glow passing through the glass)
            c += bulbColor.rgb * tint * (0.04 + 0.16 * gLift) * bulbGlow * (1.0 - fres) * smoothstep(1.8, 0.0, length(p.xz));
            float spec = pow(clamp(dot(reflect(rd, n), normalize(vec3(-0.4, 0.8, 0.6))), 0.0, 1.0), 80.0);
            c += vec3(1.0) * spec * 2.0 * (1.0 + highP() * ar);
            col += c * trans;
            alpha = max(alpha, 0.85);
            trans *= 0.62 + 0.2 * (1.0 - fres);
            hits += 1.0;
            if (hits >= NL) break;
            // step through to the far side of this piece of glass
            float tin = t + 0.004;
            for (int j = 0; j < 12; j++) {
                float di = map(ro + rd * tin);
                if (di > 0.0) break;
                tin += max(-di, 0.01);
            }
            t = tin + 0.006;
            continue;
        }
        t += max(d * 0.85, 0.002);
        if (t > 12.0) break;
    }
    // bulbs: glowing points around the fixture
    float bulbs = 0.0;
    float nb = 10.0;
    for (int i = 0; i < 10; i++) {
        float fi = float(i);
        float a = fi / nb * TAU + gT * 0.25 * spin;
        float br = gFa < 0.5 ? 1.1 : gFa < 1.5 ? 1.45 : 0.5;
        vec3 bp = vec3(cos(a) * br, (gFa < 0.5 ? 0.2 : gFa < 1.5 ? -0.6 : 0.0) + 0.6 * h11(fi) * step(1.5, gFa) - 0.3 * step(1.5, gFa), sin(a) * br) * chSize;
        bp -= anchor; bp.xy = rot(sw) * bp.xy; bp.xz = rot(yaw) * bp.xz; bp += anchor;
        bp = ro + (bp - ro);
        vec3 w = bp - ro;
        float along = dot(w, rd);
        float dist = length(w - rd * along);
        float on = smoothstep(0.0, 1.0, gLift * 1.3 - h11(fi * 3.7) * 0.6) + gDrop;
        bulbs += exp(-dist * dist * 900.0) * 1.2 * on + exp(-dist * 9.0) * 0.06 * on;
    }
    col += bulbColor.rgb * bulbs * bulbGlow;
    // light caught in the crystal mass (soft bloom around the fixture)
    col += mix(bulbColor.rgb, vec3(0.8, 0.85, 1.0), 0.5) * glowAcc * (0.4 + 0.9 * gLift + 1.5 * gDrop) * bulbGlow;
    // star glints: 4-point stars that twinkle on the crystal
    for (int i = 0; i < 9; i++) {
        float fi = float(i);
        float cyc = floor(TIME * (0.4 + 0.3 * h11(fi)) + h11(fi * 2.0) * 5.0);
        vec2 gp = (vec2(h11(fi * 7.1 + cyc), h11(fi * 3.9 + cyc * 1.3)) - 0.5) * vec2(0.6, 0.7) * chSize;
        float life = sin(PI * fract(TIME * (0.4 + 0.3 * h11(fi)) + h11(fi * 2.0) * 5.0));
        vec2 dd = abs(sp - gp);
        float star = exp(-dd.x * 900.0) * exp(-dd.y * 45.0) + exp(-dd.y * 900.0) * exp(-dd.x * 45.0);
        star += exp(-dot(dd, dd) * 4000.0) * 2.0;
        vec3 sc = h11(fi * 5.5) < 0.5 ? vec3(1.0, 0.85, 0.55) : vec3(0.8, 0.9, 1.0);
        col += sc * star * life * 0.6 * glints * (0.5 + 0.8 * gLift + highP() * ar);
    }
    col = bg * trans + col;
    col += vec3(1.0) * gDrop * gDrop * 0.15 * alpha;
    col *= 0.85 + 0.25 * gLift + 0.1 * audioBeatPulse * ar;
    col = 1.0 - exp(-col * 1.3);
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(alpha + lum(col) * 0.5, 0.0, 1.0) : 1.0);
}
