/*{
  "DESCRIPTION": "Eyeball — a living 3D eye (or two). Each eyeball is a true sphere with a refracting cornea, a fibrous iris and a pupil that is a portal: look through it and you see a whole world inside — Fire (rising flames), Water (caustics and bubbles), Thunder (storm clouds split by lightning), Galaxy (a spiral nebula of stars), Acid (oil-slick swirls) or a Rainbow iris of streaming colour fibres — and that world also flows through the iris fibres. Worlds morph from one to the next. The white of the eye has its own styles: classic red veins, neon holographic, dark sculpted creases or liquid chrome, with wet highlights, window reflections and a fresnel rim. Choose One Eye, Two Eyes or Twin Iris (two irises on one ball). Switch on Eyelids & Skin for almond lids, curled lashes and blinks, and the 1-Bit Dither look turns it all into chunky black-and-white pixel art. The gaze drifts and snaps to new targets, the pupil dilates with the bass, beats make it jump and strike lightning, mids churn the worlds, highs sparkle the highlights. The space around the eye comes alive too: a World Corona where the fire, water or lightning spills out around the ball, a giant Iris Tunnel behind it, or spinning Aura Rays with orbiting sparks. At rest the eye drifts calmly with a soft glow; during a build the pupil tightens to a pinpoint, the surroundings contract and spin faster and the eye starts to tremble; on the drop the pupil blows wide, a shockwave ring blasts outward and the world floods out around it.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "eyeMode",     "LABEL": "Eyes", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["One Eye", "Two Eyes", "Twin Iris (one ball)"], "GROUP": "Eye" },
    { "NAME": "worldSel",    "LABEL": "World Inside", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6], "LABELS": ["Auto Morph", "Fire", "Water", "Thunder", "Galaxy", "Acid", "Rainbow Iris"], "GROUP": "Eye" },
    { "NAME": "scleraSel",   "LABEL": "Eyeball Style", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Auto Morph", "Classic Veins", "Neon Holo", "Dark Sculpt", "Chrome", "Glass Orb (world inside)"], "GROUP": "Eye" },
    { "NAME": "lidsOn",      "LABEL": "Eyelids & Skin", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["Off (bare eyeball)", "On (lids, lashes, blinks)"], "GROUP": "Eye" },
    { "NAME": "lookSel",     "LABEL": "Render Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["Glossy Colour", "1-Bit Dither"], "GROUP": "Eye" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 10.0, "GROUP": "Eye" },
    { "NAME": "lidOpen",     "LABEL": "Lid Opening",      "TYPE": "float", "MIN": 0.3, "MAX": 1.2, "DEFAULT": 0.85, "GROUP": "Eyelids" },
    { "NAME": "blinkRate",   "LABEL": "Blink Rate",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Eyelids" },
    { "NAME": "lashLen",     "LABEL": "Lashes",           "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Eyelids" },
    { "NAME": "skinColor",   "LABEL": "Skin Colour",      "TYPE": "color", "DEFAULT": [0.82, 0.62, 0.52, 1.0], "GROUP": "Eyelids" },
    { "NAME": "ditherScale", "LABEL": "Dither Pixel Size","TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 10.0, "DEFAULT": 3.0, "GROUP": "1-Bit Dither" },
    { "NAME": "ditherContrast","LABEL": "Dither Contrast","TYPE": "float", "MIN": 0.5, "MAX": 2.5, "DEFAULT": 1.3, "GROUP": "1-Bit Dither" },
    { "NAME": "ditherInk",   "LABEL": "Dither Light Ink", "TYPE": "color", "DEFAULT": [0.96, 0.95, 0.97, 1.0], "GROUP": "1-Bit Dither" },
    { "NAME": "eyeSize",     "LABEL": "Eye Size",         "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Eye" },
    { "NAME": "irisSize",    "LABEL": "Iris Size",        "TYPE": "float", "MIN": 0.6, "MAX": 1.4, "DEFAULT": 1.0, "GROUP": "Eye" },
    { "NAME": "pupilSize",   "LABEL": "Pupil Size",       "TYPE": "float", "MIN": 0.1, "MAX": 0.8, "DEFAULT": 0.42, "GROUP": "Eye" },
    { "NAME": "gazeMotion",  "LABEL": "Gaze Motion",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "saccades",    "LABEL": "Saccade Jumps",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Motion" },
    { "NAME": "worldSpeed",  "LABEL": "World Speed",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "worldOnIris", "LABEL": "World Through Iris","TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Look" },
    { "NAME": "surroundSel", "LABEL": "Surroundings", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6], "LABELS": ["Auto Morph", "World Corona", "Iris Tunnel", "Aura Rays", "Plain", "Radial Gradient", "Minimal Black"], "GROUP": "Look" },
    { "NAME": "gradColA",    "LABEL": "Radial Centre", "TYPE": "color", "DEFAULT": [0.95, 0.12, 0.45, 1.0], "GROUP": "Look" },
    { "NAME": "gradColB",    "LABEL": "Radial Mid",    "TYPE": "color", "DEFAULT": [0.35, 0.12, 0.95, 1.0], "GROUP": "Look" },
    { "NAME": "gradColC",    "LABEL": "Radial Edge",   "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.08, 1.0], "GROUP": "Look" },
    { "NAME": "gradDrift",   "LABEL": "Radial Colour Drift", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Look" },
    { "NAME": "surroundAmt", "LABEL": "Surround Amount",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "worldOnBall", "LABEL": "World On Eyeball", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Look" },
    { "NAME": "veins",       "LABEL": "Veins",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "wetness",     "LABEL": "Wet Highlights",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "glowAmt",     "LABEL": "Inner Glow",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState", "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",  "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 2, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.05, 0.04, 0.07, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Eyeball — single pass, analytic spheres. Cornea refraction finds the iris
// plane; inside the pupil a short volume march renders the world behind it.
// ─────────────────────────────────────────────────────────────────────────

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float h13(vec3 p)  { p = fract(p * 0.1031); p += dot(p, p.zyx + 31.32); return fract((p.x + p.y) * p.z); }
float vn3(vec3 x) {
    vec3 i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(mix(h13(i), h13(i + vec3(1, 0, 0)), f.x), mix(h13(i + vec3(0, 1, 0)), h13(i + vec3(1, 1, 0)), f.x), f.y),
               mix(mix(h13(i + vec3(0, 0, 1)), h13(i + vec3(1, 0, 1)), f.x), mix(h13(i + vec3(0, 1, 1)), h13(i + vec3(1, 1, 1)), f.x), f.y), f.z);
}
float fbm3(vec3 p) { return 0.5 * vn3(p) + 0.27 * vn3(p * 2.03 + 1.7) + 0.15 * vn3(p * 4.1 + 3.1) + 0.08 * vn3(p * 8.3 + 5.3); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
float lum(vec3 c) { return dot(c, vec3(0.2126, 0.7152, 0.0722)); }
vec3 rainbow(float t) { return 0.5 + 0.5 * cos(TAU * (t + vec3(0.0, 0.33, 0.67))); }
vec3 hueRot(vec3 c, float h) {
    if (h <= 0.0) return c;
    float a = h * TAU; vec3 k = vec3(0.57735); float ca = cos(a);
    return c * ca + cross(k, c) * sin(a) + k * dot(k, c) * (1.0 - ca);
}
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

float gT, gW;      // world time, world morph weight toward next world

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
float gWa, gWb;    // current and next world index
float gStrike;     // lightning strike envelope

// ── the worlds inside (q: eye-local, unit sphere; -z is deeper) ──────────
vec4 world(float k, vec3 q) {
    float t = gT;
    if (k < 0.5) {                                   // Fire
        vec3 p = q * 3.0 + vec3(0.0, -t * 1.6, 0.0);
        float n = fbm3(p + vec3(0.0, 0.0, t * 0.3)) + 0.25 * midP() * audioReact;
        float h = clamp(0.55 - q.y * 0.6, 0.0, 1.0);
        float fl = smoothstep(0.35, 0.85, n * h * 1.6);
        vec3 c = mix(vec3(0.15, 0.0, 0.0), vec3(1.0, 0.25, 0.02), smoothstep(0.0, 0.4, fl));
        c = mix(c, vec3(1.0, 0.75, 0.15), smoothstep(0.4, 0.75, fl));
        c = mix(c, vec3(1.0, 0.97, 0.8), smoothstep(0.75, 1.0, fl));
        return vec4(c * fl * 1.6, fl);
    }
    if (k < 1.5) {                                   // Water
        vec2 p = q.xz * 4.0 + vec2(t * 0.2, -t * 0.15);
        float cs = 0.0;
        for (int i = 0; i < 3; i++) {
            float fi = float(i);
            p += vec2(sin(p.y * 1.3 + t * 0.7 + fi), cos(p.x * 1.1 - t * 0.6 + fi)) * 0.5;
            cs += 0.15 / (0.05 + abs(sin(p.x + fi) * sin(p.y - fi)));
        }
        cs = clamp(cs * 0.08, 0.0, 1.5);
        vec3 c = mix(vec3(0.0, 0.08, 0.25), vec3(0.0, 0.55, 0.85), 0.5 + 0.5 * q.y);
        c += vec3(0.6, 0.95, 1.0) * cs * 0.6;
        // bubbles rising
        vec3 bp = q * 6.0 + vec3(0.0, -t * 1.2, 0.0);
        vec3 bi = floor(bp), bf = fract(bp) - 0.5;
        float br = 0.12 + 0.15 * h13(bi);
        float bd = length(bf - (vec3(h13(bi + 1.0), 0.0, h13(bi + 2.0)) - 0.5) * 0.5);
        float bub = step(0.6, h13(bi + 4.0)) * smoothstep(0.03, 0.0, abs(bd - br));
        c += vec3(0.8, 1.0, 1.0) * bub;
        return vec4(c * 0.9, 0.5 + 0.3 * cs);
    }
    if (k < 2.5) {                                   // Thunder
        float cl = fbm3(q * 2.5 + vec3(t * 0.15, 0.0, t * 0.1));
        vec3 c = mix(vec3(0.02, 0.0, 0.06), vec3(0.3, 0.15, 0.5), smoothstep(0.35, 0.75, cl));
        // ridged lightning in a vertical sheet, flashing on strikes
        vec3 lp = q * vec3(3.0, 1.5, 3.0) + vec3(0.0, t * 0.1, floor(t * 2.0) * 1.7);
        float r = 1.0 - abs(vn3(lp) * 2.0 - 1.0);
        float r2 = 1.0 - abs(vn3(lp * 2.3 + 4.0) * 2.0 - 1.0);
        float bolt = pow(r, 12.0) + 0.6 * pow(r2, 16.0);
        float flash = 0.4 + 1.6 * gStrike + 0.9 * step(0.7, h11(floor(t * 6.0)));
        // a main forked strike that re-rolls each beat-ish step
        float sid = floor(t * 1.3);
        float x0 = (h11(sid * 5.1) - 0.5) * 0.8;
        float jag = x0 + 0.18 * (vn3(vec3(q.y * 6.0, sid, 0.0)) - 0.5) + 0.07 * (vn3(vec3(q.y * 19.0, sid, 3.0)) - 0.5);
        float fork = x0 + (q.y - 0.1) * (h11(sid * 2.3) - 0.5) * 1.6 + 0.08 * (vn3(vec3(q.y * 14.0, sid, 9.0)) - 0.5);
        float life = exp(-fract(t * 1.3) * 3.0);
        float mainB = exp(-abs(q.x - jag) * 70.0) + 0.5 * exp(-abs(q.x - jag) * 12.0);
        float fk = (exp(-abs(q.x - fork) * 90.0) + 0.3 * exp(-abs(q.x - fork) * 15.0)) * step(q.y, 0.1);
        bolt += (mainB + fk * 0.7) * life * 1.4;
        c += vec3(0.75, 0.7, 1.0) * bolt * flash * 3.0;
        c += vec3(0.4, 0.3, 0.8) * gStrike * 0.4 * cl;
        return vec4(c, 0.5 + bolt);
    }
    if (k < 3.5) {                                   // Galaxy
        float r = length(q.xy);
        float a = atan(q.y, q.x) - t * 0.25;
        float arms = 0.5 + 0.5 * cos(2.0 * (a - log(r + 0.05) * 2.2));
        float dust = fbm3(q * 4.0 + vec3(0.0, 0.0, t * 0.1));
        float dens = arms * smoothstep(0.95, 0.0, r) * (0.4 + dust);
        vec3 c = mix(vec3(0.25, 0.05, 0.4), vec3(1.0, 0.45, 0.8), arms) * dens;
        c += vec3(1.0, 0.85, 0.7) * exp(-r * 7.0) * 1.2;
        vec3 sp = q * 22.0;
        float st = step(0.985, h13(floor(sp))) * smoothstep(0.35, 0.0, length(fract(sp) - 0.5));
        c += vec3(0.9, 0.95, 1.0) * st * (0.6 + 0.6 * sin(t * 4.0 + h13(floor(sp)) * 30.0));
        return vec4(c, dens + st);
    }
    if (k < 4.5) {                                   // Acid
        vec3 p = q * 2.0;
        p += 0.5 * vec3(sin(p.y * 2.0 + t), sin(p.z * 2.3 - t * 0.8), sin(p.x * 1.7 + t * 0.6));
        float th = fbm3(p) * 3.0 + t * 0.2;
        vec3 c = 0.5 + 0.5 * cos(TAU * (th * vec3(1.0, 1.15, 1.3) + vec3(0.0, 0.2, 0.45)));
        c = pow(c, vec3(1.6)) * 1.2;
        return vec4(c, 0.8);
    }
    // Rainbow iris world: streaming colour fibres
    float a = atan(q.y, q.x);
    float f = vn3(vec3(cos(a) * 18.0, sin(a) * 18.0, length(q.xy) * 3.0 - t * 0.6));
    vec3 c = mix(rainbow(a / TAU + f * 0.6 + t * 0.03), vec3(1.0), 0.25) * (0.4 + f);
    return vec4(c, 0.7);
}

vec4 worldMix(vec3 q) {
    vec4 a = world(gWa, q);
    if (gW > 0.001) a = mix(a, world(gWb, q), gW);
    return a;
}

// ── iris texture (ref: fibrous rainbow iris) ─────────────────────────────
vec3 irisColor(vec2 ip, float irisR, float pupR) {
    float r = length(ip) / irisR;                    // 0 centre … 1 limbus
    float a = atan(ip.y, ip.x);
    float fib = vn3(vec3(cos(a) * 26.0, sin(a) * 26.0, r * 2.2 + gT * 0.05));
    float fib2 = vn3(vec3(cos(a) * 60.0, sin(a) * 60.0, r * 5.0));
    float crypt = smoothstep(0.62, 0.8, vn3(vec3(cos(a) * 9.0, sin(a) * 9.0, r * 6.0)));
    vec3 c = mix(rainbow(a / TAU * 1.3 + fib * 0.8 + r * 0.4), vec3(0.95), 0.35);
    c *= 0.45 + 0.75 * fib + 0.35 * fib2;
    c *= 1.0 - 0.45 * crypt;
    // collarette: a bright jagged ring
    float col = smoothstep(0.06, 0.0, abs(r - 0.42 - 0.05 * sin(a * 11.0 + fib * 3.0)));
    c += vec3(1.0, 0.9, 0.75) * col * 0.4;
    // pupil ruff: warm, dark
    float pr = pupR / irisR;
    c = mix(c, vec3(0.5, 0.05, 0.05) * (0.5 + fib), smoothstep(pr + 0.12, pr, r) * 0.8);
    // limbal ring
    c *= 1.0 - 0.75 * smoothstep(0.82, 1.0, r);
    // the world flows through the fibres
    vec3 q = vec3(ip / irisR, -0.2);
    vec4 w = worldMix(q * 0.9);
    c = mix(c, w.rgb * (0.55 + 0.9 * fib) + c * 0.15, worldOnIris);
    return c;
}

// ── sclera styles ────────────────────────────────────────────────────────
float veinField(vec3 n) {
    // branching veins that run from the back of the ball toward the iris:
    // ridged noise in (around-axis angle, polar angle), stretched along the polar angle
    float th = acos(clamp(n.z, -1.0, 1.0));
    vec2 d = normalize(n.xy + 1e-5);
    float w = vn3(vec3(d * 3.0, th * 2.0)) - 0.5;
    vec2 dw = vec2(d.x * cos(w) - d.y * sin(w), d.x * sin(w) + d.y * cos(w));
    float r1 = 1.0 - abs(vn3(vec3(dw * 5.0, th * 1.3)) * 2.0 - 1.0);
    float r2 = 1.0 - abs(vn3(vec3(dw * 12.0 + 7.0, th * 3.5)) * 2.0 - 1.0);
    float r3 = 1.0 - abs(vn3(vec3(dw * 26.0 + 3.0, th * 7.0)) * 2.0 - 1.0);
    float g2 = smoothstep(0.25, 0.5, vn3(vec3(dw * 4.0, th * 3.0) + 11.0));
    float g3 = smoothstep(0.3, 0.55, vn3(vec3(dw * 7.0, th * 5.0) + 23.0));
    float taper = smoothstep(0.35, 1.6, th);
    return pow(r1, mix(40.0, 12.0, taper)) + 0.9 * g2 * pow(r2, mix(40.0, 18.0, taper)) + 0.7 * g3 * pow(r3, 24.0) * taper;
}
vec3 sclera(float k, vec3 ln, vec3 n, vec3 rd, vec3 rdl, float thetaN) {
    float away = smoothstep(0.5, 2.2, thetaN);       // 0 near iris … 1 at the back
    float vv = veinField(ln) * veins * smoothstep(0.45, 0.9, thetaN);
    if (k < 0.5) {                                   // Classic
        vec3 c = mix(vec3(0.95, 0.92, 0.88), vec3(0.95, 0.72, 0.68), away * 0.6);
        c = mix(c, vec3(0.75, 0.05, 0.05), clamp(vv * 1.4, 0.0, 1.0));
        return c;
    }
    if (k < 1.5) {                                   // Neon holo (ref 3)
        float h = dot(n, -rd) * 1.5 + ln.y * 0.5 + gT * 0.05;
        vec3 c = mix(vec3(1.0, 0.75, 0.35), vec3(1.0, 0.25, 0.3), away);
        c = mix(c, vec3(0.3, 1.0, 0.25), smoothstep(0.5, 0.9, ln.y) * 0.8);
        c += rainbow(h) * 0.25;
        c = mix(c, vec3(1.0, 0.1, 0.05), clamp(vv * 1.2, 0.0, 1.0));
        return c;
    }
    if (k < 2.5) {                                   // Dark sculpt (ref 2)
        float cr = 1.0 - abs(vn3(ln * 3.0) * 2.0 - 1.0);
        vec3 c = mix(vec3(0.55, 0.52, 0.5), vec3(0.15, 0.13, 0.12), away * 0.8);
        c *= 0.7 + 0.5 * smoothstep(0.85, 0.98, cr);
        c = mix(c, vec3(0.6, 0.3, 0.25), clamp(vv, 0.0, 1.0) * 0.6);
        return c;
    }
    if (k > 3.5) {                                   // Glass orb: the world fills the ball
        vec3 rl = refract(rdl, ln, 1.0 / 1.33);
        vec3 q = ln;
        vec3 acc = vec3(0.0); float tr = 1.0;
        for (int i = 0; i < 12; i++) {
            q += rl * 0.15;
            if (length(q) > 1.0) break;
            vec4 w = worldMix(q * 0.95);
            acc += w.rgb * tr * 0.13 * glowAmt;
            tr *= 1.0 - clamp(w.a * 0.1, 0.0, 0.4);
        }
        vec3 c = 1.0 - exp(-acc * 1.3) + vec3(0.03, 0.02, 0.05);
        return mix(c, vec3(0.8, 0.1, 0.1), clamp(vv, 0.0, 1.0) * 0.5);
    }
    // Chrome
    vec3 rf = reflect(rd, n);
    vec3 env = mix(vec3(0.05), vec3(1.0), smoothstep(-0.1, 0.4, rf.y)) + vec3(1.2) * smoothstep(0.9, 0.98, dot(rf, normalize(vec3(-0.4, 0.6, 0.6))));
    env *= 0.8 + 0.4 * sin(rf.x * 6.0 + gT);
    return mix(env, vec3(0.9, 0.1, 0.1), clamp(vv, 0.0, 1.0) * 0.5);
}

// sclera + the world creeping across the ball in drifting patches
vec3 scleraW(float k, vec3 ln, vec3 n, vec3 rd, vec3 rdl, float th) {
    vec3 c = sclera(k, ln, n, rd, rdl, th);
    if (worldOnBall > 0.0 && k < 3.5) {
        float m = smoothstep(0.42, 0.72, fbm3(ln * 1.8 + vec3(gT * 0.12, -gT * 0.08, 0.0)) + 0.2 * midP() * audioReact);
        m *= smoothstep(0.35, 0.8, th);
        vec4 w = worldMix(ln * 0.95);
        c = mix(c, w.rgb * 1.3 + c * 0.1, clamp(m * worldOnBall * 1.6, 0.0, 1.0));
    }
    return c;
}

// ordered dither (8x8 Bayer, recursive form)
float bayer2(vec2 a) { a = floor(a); return fract(dot(a, vec2(0.5, a.y * 0.75))); }
float bayer4(vec2 a) { return bayer2(0.5 * a) * 0.25 + bayer2(a); }
float bayer8(vec2 a) { return bayer4(0.5 * a) * 0.25 + bayer2(a); }

// eyelids, lashes and skin around one eye. q = screen offset in eye radii.
// returns skin colour in .rgb and the skin coverage in .a; shadow → lidShadow
float gLidShadow;
vec4 lids(vec2 q, float open, float px) {
    float x = q.x / 1.0;
    float xs = clamp(1.0 - x * x, 0.0, 1.0);
    float top = 0.66 * open * pow(xs, 0.7) + 0.06 * x - 0.05 * (1.0 - open);
    float bot = -0.52 * open * pow(xs, 0.95) + 0.04 * x - 0.08;
    float dTop = top - q.y, dBot = q.y - bot;
    float inside = smoothstep(-px, px, min(dTop, dBot)) * step(abs(x), 1.0);
    gLidShadow = mix(0.35, 1.0, smoothstep(0.0, 0.3, dTop)) * mix(0.7, 1.0, smoothstep(0.0, 0.12, dBot));
    // skin: soft shading, the upper-lid fold, under-eye curve, pores
    vec3 sk = skinColor.rgb;
    float fold = top + 0.32 + 0.12 * xs;                   // crease above the lid
    float shade = 0.75 + 0.25 * smoothstep(-1.5, 1.5, q.y);
    shade *= 1.0 - 0.35 * exp(-pow((q.y - fold) / 0.07, 2.0)) * smoothstep(1.3, 0.3, abs(q.x));
    shade *= 1.0 - 0.3 * exp(-pow((q.y - (bot - 0.22)) / 0.08, 2.0)) * smoothstep(1.4, 0.4, abs(q.x));
    shade *= 0.93 + 0.1 * vn3(vec3(q * 3.0, 1.0)) + 0.08 * vn3(vec3(q * 45.0, 3.0));
    float inEye = smoothstep(1.15, 0.92, abs(x));
    // the lid itself catches light, the lash line is dark
    shade *= 1.0 + 0.25 * smoothstep(0.2, 0.0, -dTop) * step(dTop, 0.0) * inEye;
    shade *= 1.0 - 0.75 * exp(-pow(-dTop / 0.03, 2.0)) * step(dTop, 0.02) * inEye;
    shade *= 1.0 - 0.45 * exp(-pow(-dBot / 0.025, 2.0)) * step(dBot, 0.02) * inEye;
    vec3 c = sk * shade;
    // lashes: curled strokes rising from the lid edges
    float lash = 0.0;
    float sp = 0.055;
    for (int j = -1; j <= 1; j++) {
        // upper
        float d = -dTop;                                   // height above the lid edge
        float id = floor(q.x / sp) + float(j);
        float hj = h11(id * 1.37 + 5.0);
        float L = lashLen * (0.18 + 0.22 * hj) * smoothstep(1.05, 0.35, abs(id * sp));
        float sgn = sign(id * sp + 0.0001);
        float xl = (id + 0.5) * sp + sgn * (d * 0.35 + d * d * 2.2) + (hj - 0.5) * 0.02;
        float th = 0.012 * (1.0 - clamp(d / max(L, 1e-3), 0.0, 1.0));
        lash = max(lash, smoothstep(th + px, th - px * 0.5, abs(q.x - xl)) * step(0.0, d) * step(d, L));
        // lower (shorter, sparser)
        float d2 = -dBot;
        float id2 = floor(q.x / (sp * 1.6)) + float(j);
        float hk = h11(id2 * 2.91 + 1.0);
        float L2 = lashLen * (0.06 + 0.09 * hk) * smoothstep(0.95, 0.3, abs(id2 * sp * 1.6));
        float xl2 = (id2 + 0.5) * sp * 1.6 + sign(id2 + 0.5) * d2 * 0.4;
        float th2 = 0.008 * (1.0 - clamp(d2 / max(L2, 1e-3), 0.0, 1.0));
        lash = max(lash, smoothstep(th2 + px, th2 - px * 0.5, abs(q.x - xl2)) * step(0.0, d2) * step(d2, L2) * step(hk, 0.75));
    }
    c = mix(c, vec3(0.04, 0.03, 0.03), lash);
    float cover = max(1.0 - inside, lash);
    // tear duct: a pink corner on the inner side
    float duct = exp(-dot(q - vec2(-0.97, -0.02), q - vec2(-0.97, -0.02)) / 0.004);
    c = mix(c, vec3(0.85, 0.45, 0.45), duct * 0.7);
    // fade the face out into the background far from the eye
    float fade = 1.0 - smoothstep(2.3, 3.4, length(q * vec2(0.75, 1.0)));
    return vec4(c, cover * fade);
}

// ── the space around the eye(s) ───────────────────────────────────────────
vec3 surroundings(vec2 sp, bool two, float Rr, float zc, float ph) {
    float ss = float(surroundSel);
    if (ss > 3.5) return vec3(0.0);
    float rs = Rr * 1.6 / zc;
    vec2 cc = two ? vec2((sp.x > 0.0 ? 1.15 : -1.15) * eyeSize * 1.6 / zc, 0.0) : vec2(0.0);
    vec2 d = (sp - cc) / rs;                       // in eye radii
    float r = length(d), a = atan(d.y, d.x);
    if (r < 0.98) return vec3(0.0);
    float ar = audioReact;
    float lift = gLift;
    float t = TIME * 0.25 + (audioMidTime * 0.25 + audioBassTime * 0.15) * ar;
    // which style (auto morphs along with the worlds, half the rate)
    float sa = ss > 0.5 ? ss - 1.0 : mod(floor(ph * 0.5), 3.0);
    float sb = ss > 0.5 ? sa : mod(floor(ph * 0.5) + 1.0, 3.0);
    float sf = ss > 0.5 ? 0.0 : smoothstep(0.7, 1.0, fract(ph * 0.5));
    vec3 res = vec3(0.0);
    for (int i = 0; i < 2; i++) {
        float st = i == 0 ? sa : sb;
        float w = i == 0 ? 1.0 - sf : sf;
        if (w < 0.001) continue;
        vec3 c = vec3(0.0);
        if (st < 0.5) {
            // World Corona: the inner world spills out around the ball
            float reach = 0.5 + 1.1 * lift + 0.9 * gDrop;
            float m = exp(-(r - 1.0) / max(reach, 0.05)) * smoothstep(0.98, 1.06, r);
            vec2 dr = mat2(0.799, -0.602, 0.602, 0.799) * d;       // rotate off the noise grid
            vec3 q = vec3(dr * (0.3 - 0.08 * gBuild), 0.15 + 0.1 * sin(t * 0.3));
            vec4 wv = worldMix(q);
            float flick = 0.75 + 0.25 * vn3(vec3(a * 3.0, r * 2.0 - t * 2.0, t));
            c = (1.0 - exp(-wv.rgb * 1.4)) * m * flick * (0.25 + 0.95 * lift) * glowAmt;
        } else if (st < 1.5) {
            // Iris Tunnel: a giant iris behind the eye, fibres streaming inward
            float lr = log(r);
            float fib = vn3(vec3(cos(a) * 14.0, sin(a) * 14.0, lr * 3.0 - t * (1.5 + 2.5 * gBuild)));
            float fib2 = vn3(vec3(cos(a) * 40.0, sin(a) * 40.0, lr * 6.0 - t * 3.0));
            float rings = 0.5 + 0.5 * sin(lr * 10.0 - t * 4.0 - gBuild * 6.0);
            vec3 hue = rainbow(a / TAU + lr * 0.3 + t * 0.05);
            c = mix(hue, vec3(1.0), 0.3) * (0.35 * fib + 0.25 * fib2) * (0.6 + 0.4 * rings);
            c *= smoothstep(0.98, 1.3, r) * exp(-(r - 1.0) * (0.55 - 0.25 * lift)) * (0.45 + 0.9 * lift);
            c += vec3(0.9, 0.3, 0.2) * smoothstep(0.98, 1.05, r) * exp(-(r - 1.0) * 12.0) * 0.5;   // dark ring glow
        } else {
            // Aura Rays: spinning god-rays and orbiting sparks
            float spin = t * (0.6 + 1.6 * gBuild);
            float rays = pow(0.5 + 0.5 * cos(a * 12.0 + spin + 2.0 * vn3(vec3(a * 2.0, t, 0.0))), 6.0 - 3.0 * lift);
            rays += 0.6 * pow(0.5 + 0.5 * cos(a * 31.0 - spin * 1.7), 14.0);
            float len = 0.7 + 1.6 * lift + 1.5 * gDrop;
            c = mix(vec3(0.6, 0.4, 1.0), vec3(1.0, 0.5, 0.25), 0.5 + 0.5 * sin(a * 2.0 + t)) * rays * exp(-(r - 1.0) / len) * 0.8;
            // sparks orbiting in rings
            float ring = floor((r - 1.0) * 6.0);
            float ang = a + t * (0.5 + h11(ring * 3.1)) * (mod(ring, 2.0) * 2.0 - 1.0);
            vec2 cell = vec2(fract(ang / TAU * (14.0 + ring * 6.0)) - 0.5, fract((r - 1.0) * 6.0) - 0.5);
            float on = step(0.6 - 0.35 * lift, h12(vec2(floor(ang / TAU * (14.0 + ring * 6.0)), ring)));
            c += vec3(1.0, 0.95, 0.85) * on * smoothstep(0.18, 0.0, length(cell * vec2(1.0, 1.0))) * exp(-(r - 1.0) * 0.8) * (0.4 + highP() * ar);
        }
        res += c * w;
    }
    // shockwave rings: one big ring on the drop, small ones on beats
    float ringR = 1.0 + (1.0 - gDrop) * 4.0;
    res += vec3(1.0, 0.9, 0.85) * exp(-pow((r - ringR) * 7.0, 2.0)) * gDrop * 1.5;
    res += vec3(0.8, 0.85, 1.0) * exp(-pow((r - 1.0 - (1.0 - audioBeatPulse) * 1.5) * 12.0, 2.0)) * audioBeatPulse * ar * 0.35 * (0.4 + lift);
    // build: the field contracts toward the eye — a bright inner ring tightens
    res += vec3(1.0, 0.6, 0.4) * exp(-pow((r - 1.0 - (1.0 - gBuild) * 1.2) * 10.0, 2.0)) * gBuild * 0.6;
    return res;
}

// big radial gradient centred behind the eye(s); breathes with bass, flares on build/drop
vec3 radialGrad(vec2 sp, float Rr, float zc) {
    float r = length(sp * vec2(0.85, 1.0)) / (Rr * 1.6 / zc);       // in eye radii
    float ar = audioReact;
    float t = TIME * 0.12 + audioMidTime * 0.08 * ar;
    float sc = 1.0 + 0.35 * gLift + 0.25 * bassP() * ar + 0.6 * gDrop - 0.2 * gBuild;
    float x = max(r - 0.9, 0.0) / (2.1 * sc);                       // starts at the eye's rim
    // slow hue drift of the three stops (rotate in a cheap hue space)
    float hd = gradDrift * 0.5 * sin(t);
    mat3 hr = mat3(1.0);
    float ca = cos(hd * 3.0), sa = sin(hd * 3.0);
    vec3 k = vec3(0.57735);
    hr = mat3(ca) + (1.0 - ca) * mat3(k.x*k, k.y*k, k.z*k) + sa * mat3(0.0, k.z, -k.y, -k.z, 0.0, k.x, k.y, -k.x, 0.0);
    vec3 A = clamp(hr * gradColA.rgb, 0.0, 1.0), B = clamp(hr * gradColB.rgb, 0.0, 1.0);
    vec3 c = mix(A, B, smoothstep(0.0, 0.55, x));
    c = mix(c, gradColC.rgb, smoothstep(0.3, 1.0, x));
    // hot glow hugging the eyeball + soft bloom
    c += A * exp(-x * 9.0) * (0.55 + 0.4 * gLift + 0.6 * gDrop);
    c += mix(A, vec3(1.0), 0.4) * exp(-x * 26.0) * 0.35;
    // gentle swirl of light so the field isn't static
    float sw = 0.5 + 0.5 * sin(atan(sp.y, sp.x) * 3.0 + t * 2.0 + x * 6.0);
    c *= 0.9 + 0.18 * sw * (1.0 - smoothstep(0.2, 0.9, x));
    // soft concentric ripple that travels out on beats
    c *= 1.0 + 0.12 * audioBeatPulse * ar * exp(-pow((x - (1.0 - audioBeatPulse) * 0.8) * 6.0, 2.0));
    return c * (0.75 + 0.35 * gLift + 0.5 * gDrop);
}

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

// shade one eyeball hit; gaze dirs g1 (and g2 for twin iris)
vec3 shadeEye(vec3 ro, vec3 rd, vec3 C, float R, vec3 g1, vec3 g2, bool twin, float sk) {
    float b = dot(ro - C, rd);
    float c = dot(ro - C, ro - C) - R * R;
    float t = -b - sqrt(max(b * b - c, 0.0));
    vec3 P = ro + rd * t;
    vec3 n = normalize(P - C);
    // nearest gaze axis (twin iris has two)
    vec3 g = g1;
    if (twin && dot(n, g2) > dot(n, g1)) g = g2;
    vec3 gs = normalize(g1 + g2);                           // sclera frame (no seam on twin)
    vec3 rgS = normalize(cross(vec3(0.0, 1.0, 0.0), gs)), upS = cross(gs, rgS);
    vec3 lnS = vec3(dot(n, rgS), dot(n, upS), dot(n, gs));
    vec3 rlS = vec3(dot(rd, rgS), dot(rd, upS), dot(rd, gs));
    vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), g));
    vec3 up = cross(g, rgt);
    vec3 ln = vec3(dot(n, rgt), dot(n, up), dot(n, g));     // eye-local normal
    float theta = acos(clamp(ln.z, -1.0, 1.0));
    float thI = 0.56 * irisSize * (twin ? 0.75 : 1.0);
    vec3 col;
    float ar = audioReact;
    if (theta < thI + 0.04) {
        // cornea refraction → iris plane (local z = cos(thI))
        vec3 rl = vec3(dot(rd, rgt), dot(rd, up), dot(rd, g));
        vec3 rr = refract(rl, ln, 1.0 / 1.376);
        vec3 pl = ln;                                       // local hit (unit sphere)
        float zi = cos(thI);
        float ti = (zi - pl.z) / min(rr.z, -0.05);
        vec2 ip = pl.xy + rr.xy * ti;
        float irisR = sin(thI);
        float pupR = irisR * clamp(pupilSize * (1.0 + 0.45 * bassP() * ar + 0.25 * sin(TIME * 0.5)) * (1.0 - 0.45 * gBuild + 0.9 * gDrop + 0.15 * gPeak), 0.08, 0.9);
        float rI = length(ip);
        if (rI < pupR) {
            // through the pupil: march the world behind it
            vec3 acc = vec3(0.0);
            float trans = 1.0;
            vec3 q = vec3(ip, zi);
            for (int i = 0; i < 14; i++) {
                q += rr * 0.07;
                if (length(q) > 1.0) break;
                vec4 w = worldMix(vec3((ip + (q.xy - ip) * 0.2) / pupR * 0.8, (q.z - zi) * 2.5 + 0.3));
                acc += w.rgb * trans * 0.12 * glowAmt;
                trans *= 1.0 - clamp(w.a * 0.12, 0.0, 0.5);
            }
            col = (1.0 - exp(-acc * 1.5)) * smoothstep(pupR, pupR * 0.85, rI);
        } else {
            col = irisColor(ip, irisR, pupR);
        }
        // blend into the sclera at the limbus
        vec3 sc = scleraW(sk, lnS, n, rd, rlS, theta);
        col = mix(col, sc, smoothstep(thI - 0.02, thI + 0.04, theta));
        // cornea: glossy dome
        float spec = pow(clamp(dot(reflect(rd, n), normalize(vec3(-0.5, 0.6, 0.65))), 0.0, 1.0), 120.0);
        col += vec3(1.0) * spec * 1.6 * wetness;
    } else {
        col = scleraW(sk, lnS, n, rd, rlS, theta);
        float dif = 0.55 + 0.45 * clamp(dot(n, normalize(vec3(-0.4, 0.6, 0.7))), 0.0, 1.0);
        col *= dif;
    }
    // wet highlights: two window reflections + a soft rim
    vec3 rf = reflect(rd, n);
    vec2 w1 = vec2(rf.x + 0.45, rf.y - 0.5);
    float win = smoothstep(0.08, 0.06, max(abs(w1.x), abs(w1.y) * 0.7)) * step(0.0, rf.z);
    float win2 = smoothstep(0.05, 0.035, max(abs(rf.x - 0.35), abs(rf.y - 0.45) * 0.6)) * step(0.0, rf.z);
    col += vec3(0.95, 0.97, 1.0) * (win * 0.8 + win2 * 0.5) * wetness * (1.0 + highP() * ar);
    float fres = pow(1.0 - clamp(dot(n, -rd), 0.0, 1.0), 3.0);
    col = mix(col, col * 0.25, fres * 0.85);                    // darken toward the silhouette
    col += mix(vec3(0.6, 0.4, 1.0), vec3(1.0, 0.4, 0.2), sk / 4.0) * fres * 0.25;
    return col;
}

void main() {
    vec2 R = RENDERSIZE.xy;
    bool dith = float(lookSel) > 0.5;
    float ds = max(floor(ditherScale), 1.0);
    vec2 fc = dith ? (floor(gl_FragCoord.xy / ds) + 0.5) * ds : gl_FragCoord.xy;   // chunky pixels
    vec2 uv = fc / R;
    float ar = audioReact;
    showDynamics();
    gT = TIME * worldSpeed * 0.7 + (0.4 * audioBassTime + 0.3 * audioMidTime) * ar * worldSpeed + 0.3 * midP() * ar + 0.6 * gBuild;
    gStrike = audioBeatPulse * audioBeatPulse * ar;

    // worlds + sclera style morphing
    float ph = TIME / morphPeriod;
    float k = floor(ph);
    float f = smoothstep(0.65, 1.0, fract(ph));
    float ws = float(worldSel), ss = float(scleraSel);
    gWa = ws > 0.5 ? ws - 1.0 : mod(k, 6.0);
    gWb = ws > 0.5 ? gWa : mod(k + 1.0, 6.0);
    gW = ws > 0.5 ? 0.0 : f;
    float sk = ss > 0.5 ? ss - 1.0 : mod(floor(ph * 0.5 + 0.25), 5.0);

    // gaze: smooth drift + saccade jumps (beats trigger extra jumps)
    float sacT = TIME * (0.6 + 0.6 * saccades);
    float si = floor(sacT);
    float sf = smoothstep(0.0, 0.08, fract(sacT));
    vec2 s0 = (vec2(h11(si * 3.1), h11(si * 7.7)) - 0.5);
    vec2 s1 = (vec2(h11((si + 1.0) * 3.1), h11((si + 1.0) * 7.7)) - 0.5);
    vec2 sac = mix(s0, s1, sf) * saccades * 0.7 + (vec2(h11(floor(TIME * 3.0)), h11(floor(TIME * 3.0) + 9.0)) - 0.5) * 0.25 * audioBeatPulse * ar;
    vec2 drift = vec2(0.35 * sin(TIME * 0.31), 0.22 * sin(TIME * 0.23 + 1.0)) * gazeMotion;
    vec3 target = vec3((drift + sac) * 2.5, 6.0);

    // camera
    float mode = float(eyeMode);
    bool two = mode > 0.5 && mode < 1.5;
    bool twin = mode > 1.5;
    vec3 ro = vec3(0.0, 0.0, two ? 6.8 : 4.6);
    vec2 sp = (fc - 0.5 * R) / R.y;
    vec3 rd = normalize(vec3(sp, -1.6));

    // build: a tremor that grows toward the top of the riser
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 5.0)) - 0.5) * 0.012 * smoothstep(0.6, 1.0, gBuild);
    rd = normalize(vec3(sp, -1.6));

    float sSel = float(surroundSel);
    bool minimal = sSel > 5.5;
    vec3 col = minimal ? vec3(0.0) : backdrop(uv);
    float alpha = 0.0;
    float Rr = (two ? 0.95 : 1.0) * eyeSize * (1.0 + 0.03 * bassP() * ar + 0.06 * gDrop - 0.03 * gRest);
    if (sSel > 4.5 && !minimal) col = radialGrad(sp, Rr, ro.z);
    col += surroundings(sp, two, Rr, ro.z, ph) * surroundAmt;
    for (int e = 0; e < 2; e++) {
        if (!two && e > 0) break;
        vec3 C = two ? vec3(e == 0 ? -1.15 : 1.15, 0.0, 0.0) * eyeSize : vec3(0.0);
        float b = dot(ro - C, rd);
        float c = dot(ro - C, ro - C) - Rr * Rr;
        float h = b * b - c;
        // soft halo around the ball
        float dc = length(cross(C - ro, rd)) - Rr;
        if (!minimal) col += vec3(0.45, 0.3, 0.8) * exp(-max(dc, 0.0) * 6.0) * 0.12 * glowAmt;
        if (h < 0.0) continue;
        vec3 g1 = normalize(target - C);
        vec3 g2 = g1;
        if (twin) {
            // two irises spread across one ball (cross-eyed twin)
            vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), g1));
            g2 = normalize(g1 + rgt * 0.55);
            g1 = normalize(g1 - rgt * 0.55);
        }
        vec3 ec = shadeEye(ro, rd, C, Rr, g1, g2, twin, sk);
        // anti-aliased silhouette
        float edge = clamp(-dc * R.y / 2.6 + 0.5, 0.0, 1.0);
        col = mix(col, ec, edge);
        alpha = max(alpha, edge);
    }

    // eyelids, lashes, skin + blinks
    if (float(lidsOn) > 0.5) {
        float bp = fract(TIME * (0.12 + 0.25 * blinkRate) + 0.37);
        float blink = blinkRate > 0.0 ? 1.0 - exp(-pow((bp - 0.5) / 0.018, 2.0)) : 1.0;
        blink = min(blink, 1.0 - 0.6 * audioBeatPulse * ar * step(0.6, blinkRate));
        float open = lidOpen * (twin ? 1.25 : 1.0) * blink;
        // nearest eye on screen
        float zc = ro.z;
        vec2 c0 = two ? vec2(-1.15 * eyeSize * 1.6 / zc, 0.0) : vec2(0.0);
        vec2 c1 = two ? vec2( 1.15 * eyeSize * 1.6 / zc, 0.0) : vec2(0.0);
        vec2 cc = (two && sp.x > 0.0) ? c1 : c0;
        float rs = Rr * 1.6 / zc;
        vec2 q = (sp - cc) / rs;
        if (two && sp.x > 0.0) q.x = -q.x;                 // mirror so tear ducts face the nose
        vec4 lk = lids(q * vec2(twin ? 0.8 : 1.0, 1.0), open, 1.5 / (R.y * rs));
        col *= gLidShadow;
        col = mix(col, lk.rgb, lk.a);
        alpha = max(alpha, lk.a);
    }

    col = hueRot(col, hueShift);
    col *= 1.0 + 0.15 * audioLevel * ar;
    if (dith) {
        // 1-bit: ordered dither between black and the light ink
        float L = clamp(lum(col), 0.0, 1.0);
        L = clamp((L - 0.5) * ditherContrast + 0.5, 0.0, 1.0);
        float bit = step(bayer8(floor(gl_FragCoord.xy / ds)) + 0.001, L);
        col = mix(vec3(0.015), ditherInk.rgb, bit);
        gl_FragColor = vec4(studioPalette(col), transparentBg ? alpha : 1.0);
        return;
    }
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
