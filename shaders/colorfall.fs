/*{
  "DESCRIPTION": "Colorfall — a waterfall of liquid colour. At the top a churning, glossy pool of paint ripples and swirls, its surface catching wavy white specular glints; from its lip the colour spills over and pours straight down the frame in long, soft, luminous streaks that stretch, waver and taper into pointed, flame-like drips as they accelerate, every column flowing at its own pace, like long-exposure paint with fine film grain. The liquid is lit as a thick, glossy 3D surface — its thickness becomes a height field whose normals catch a key light, wet specular highlights, a fresnel rim and refracted caustic shimmer, while softer, darker curtains fall more slowly far behind for depth. Underneath runs a real liquid simulation: a velocity field with gravity, air drag and viscosity carries the colour, so every streak starts slow at the lip and accelerates as it falls, stretching thinner; columns of the pool open and close so drips detach and fall free, neighbouring streaks merge and spread, and the falling liquid throws up a soft mist where it hits the bottom edge. Four looks morph into one another: Prism Fall (hot pink, violet, electric and sky blue, orange-red and lemon yellow on white highlights), Molten (falling lava, crimson to white-hot gold), Ink Cascade (dense indigo, plum and teal ink sinking through water) and Aurora Veil (thin luminous curtains of green, cyan and violet hanging in the dark). Bass surges the flow and swells the brightness, every kick is an impulse that splashes fresh colour out of the pool and flings it down faster before drag settles it back, mids churn the turbulence wider and highs sparkle as glints running down the glossy rivulets. At rest it is a slow, gentle trickle of a few long streaks over the backdrop; during a build the flow accelerates, the streaks thin and tighten, the pool churns harder and the top trembles; on the drop a flood surge bursts the pool open and crashes down the whole frame behind a white flash, then drains back to a trickle, settling into a full, dense, saturated torrent at the peak.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "Liquid", "Abstract", "Simulation", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "styleSel",    "LABEL": "Style", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Prism Fall", "Molten", "Ink Cascade", "Aurora Veil"], "GROUP": "Style" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 16.0, "GROUP": "Style" },
    { "NAME": "dirSel",      "LABEL": "Direction", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Down", "Up", "Sideways"], "GROUP": "Flow" },
    { "NAME": "fallSpeed",   "LABEL": "Fall Speed",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Flow" },
    { "NAME": "stretch",     "LABEL": "Streak Length",    "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 12.0, "DEFAULT": 5.0, "GROUP": "Flow" },
    { "NAME": "turbulence",  "LABEL": "Turbulence",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Flow" },
    { "NAME": "dripSharp",   "LABEL": "Drip Sharpness",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Flow" },
    { "NAME": "columns",     "LABEL": "Column Count",     "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 4.0, "DEFAULT": 1.1, "GROUP": "Flow" },
    { "NAME": "density",     "LABEL": "Density",          "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Flow" },
    { "NAME": "sourceHeight","LABEL": "Source Pool Height","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.16, "GROUP": "Source Pool" },
    { "NAME": "sourceOn",    "LABEL": "Show Source Pool", "TYPE": "bool",  "DEFAULT": true, "GROUP": "Source Pool" },
    { "NAME": "sheen",       "LABEL": "Sheen",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "saturation",  "LABEL": "Saturation",       "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "depthLayer",  "LABEL": "Depth Layer",      "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.8, "GROUP": "Look" },
    { "NAME": "brightness",  "LABEL": "Brightness",       "TYPE": "float", "MIN": 0.3, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 3, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.03, 0.04, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" },
    { "NAME": "physicsAmt",  "LABEL": "Physics Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "gravity",     "LABEL": "Gravity",          "TYPE": "float", "AUDIO": false, "MIN": 0.1, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "damping",     "LABEL": "Air Drag",         "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.35, "GROUP": "Physics" },
    { "NAME": "viscosity",   "LABEL": "Viscosity",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Physics" },
    { "NAME": "kickForce",   "LABEL": "Kick Splash Force","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "mistAmt",     "LABEL": "Bottom Mist",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Physics" },
    { "NAME": "resetPhysics","LABEL": "Reset Physics",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Physics" }
  ],
  "PASSES": [
    { "TARGET": "velBuf", "PERSISTENT": true, "FLOAT": true },
    { "TARGET": "dyeBuf", "PERSISTENT": true, "FLOAT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718
#define RSIGN 1.0

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
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

// ── noise ───────────────────────────────────────────────────────────────
float vn(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = h12(i), b = h12(i + vec2(1.0, 0.0)), c = h12(i + vec2(0.0, 1.0)), d = h12(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}
float vn1(float x) { float i = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f); return mix(h11(i), h11(i + 1.0), f); }
float fbm3(vec2 p) {
    float s = 0.0, a = 0.5;
    for (int i = 0; i < 3; i++) { s += a * vn(p); p = p * 2.03 + vec2(13.1, 7.7); a *= 0.5; }
    return s / 0.875;
}

// ── colour schemes: 8 cyclic stops each ─────────────────────────────────
vec3 stopCol(float s, float i) {
    if (s < 0.5) {                                            // Prism Fall
        if (i < 0.5) return vec3(1.0, 0.3, 0.72);
        if (i < 1.5) return vec3(0.4, 0.18, 0.95);
        if (i < 2.5) return vec3(0.1, 0.3, 0.95);
        if (i < 3.5) return vec3(0.3, 0.7, 1.0);
        if (i < 4.5) return vec3(0.92, 0.9, 1.0);
        if (i < 5.5) return vec3(1.0, 0.36, 0.78);
        if (i < 6.5) return vec3(0.64, 0.42, 1.0);
        if (i < 7.5) return vec3(1.0, 0.42, 0.86);
        if (i < 8.5) return vec3(1.0, 0.3, 0.08);
        return vec3(1.0, 0.92, 0.22);
    } else if (s < 1.5) {                                     // Molten
        if (i < 0.5) return vec3(0.3, 0.02, 0.0);
        if (i < 1.5) return vec3(0.8, 0.06, 0.02);
        if (i < 2.5) return vec3(1.0, 0.36, 0.03);
        if (i < 3.5) return vec3(1.0, 0.6, 0.1);
        if (i < 4.5) return vec3(1.0, 0.8, 0.42);
        if (i < 5.5) return vec3(1.0, 0.55, 0.06);
        if (i < 6.5) return vec3(0.62, 0.07, 0.02);
        if (i < 7.5) return vec3(0.15, 0.01, 0.0);
        if (i < 8.5) return vec3(1.0, 0.82, 0.42);
        return vec3(0.95, 0.2, 0.05);
    } else if (s < 2.5) {                                     // Ink Cascade
        if (i < 0.5) return vec3(0.02, 0.03, 0.12);
        if (i < 1.5) return vec3(0.06, 0.14, 0.48);
        if (i < 2.5) return vec3(0.28, 0.08, 0.42);
        if (i < 3.5) return vec3(0.03, 0.38, 0.5);
        if (i < 4.5) return vec3(0.1, 0.5, 0.75);
        if (i < 5.5) return vec3(0.12, 0.22, 0.62);
        if (i < 6.5) return vec3(0.5, 0.05, 0.32);
        if (i < 7.5) return vec3(0.01, 0.02, 0.07);
        if (i < 8.5) return vec3(0.55, 0.9, 1.0);
        return vec3(0.75, 0.1, 0.5);
    }
    if (i < 0.5) return vec3(0.0, 0.05, 0.08);                // Aurora Veil
    if (i < 1.5) return vec3(0.05, 0.92, 0.55);
    if (i < 2.5) return vec3(0.1, 0.62, 0.95);
    if (i < 3.5) return vec3(0.55, 0.25, 1.0);
    if (i < 4.5) return vec3(0.85, 1.0, 0.92);
    if (i < 5.5) return vec3(0.15, 0.98, 0.42);
    if (i < 6.5) return vec3(0.3, 0.4, 1.0);
    if (i < 7.5) return vec3(0.02, 0.15, 0.2);
    if (i < 8.5) return vec3(0.95, 0.25, 0.6);
    return vec3(0.8, 1.0, 0.6);
}
// cyclic body colour + warm/hot accent patches (acc 0..1, at = which accent)
vec3 schemeCol(float s, float t, float acc, float at) {
    float f = fract(t) * 8.0;
    float i = floor(f);
    vec3 c = mix(stopCol(s, i), stopCol(s, mod(i + 1.0, 8.0)), smoothstep(0.0, 1.0, fract(f)));
    vec3 a = mix(stopCol(s, 8.0), stopCol(s, 9.0), smoothstep(0.3, 0.7, at));
    return mix(c, a, acc);
}

// globals for the field
float gStrk, gFold, gStri, gPa, gPt, gAcc, gAt, gClock, gTurb, gStr, gCols, gSharp, gThin, gDens, gLip, gChurn;

// the falling colour field at point p (x aspect-scaled, y = 0 bottom .. 1 top)
// returns (colour index, coverage, gloss height)
// sim: (material age in flow units, material x offset, liquid amount) from the state buffer
float gPhys, gAspF;
vec3 simAt(vec2 p) {
    vec4 t = texture2D(dyeBuf, vec2(p.x / gAspF + 0.5, p.y));
    return vec3(t.r * 24.0 / gStr, t.g * gAspF, t.b);
}
vec3 fallField(vec2 p, vec3 sim) {
    float d = max(gLip - p.y, 0.0);                     // distance fallen below the lip
    float cx = (p.x + sim.y * gPhys) * gCols * 3.0;     // material column (carried by the flow)
    // accelerating fall: analytic g(d) is concave → streaks stretch as they drop;
    // with physics the material age comes from the simulated free fall
    float g = mix(pow(d + 0.04, 0.72) * 15.0 / gStr, sim.x, gPhys);
    // per-column speed variation (bounded, no shear build-up)
    float colPh = vn1(cx * 0.45 + 5.0) * TAU;
    float v = gClock - g + 0.35 * sin(gClock * 0.37 + colPh);
    // pointed drip tongues: ridged per-column offset
    float rd = vn1(cx * 1.6 + 1.7);
    float ridge = 1.0 - abs(2.0 * rd - 1.0);
    ridge = pow(ridge, mix(1.0, 3.5, gSharp));
    float ramp = 0.35 + 0.9 * vn1(cx * 0.3 + 11.0);
    v += RSIGN * ridge * ramp * (0.5 + 1.1 * gSharp) * smoothstep(0.0, 0.3, d);
    // wavering domain warp that travels with the flow
    float wx = vn(vec2(cx * 0.35, v * 0.45 + 3.0)) - 0.5;
    float wx2 = vn(vec2(cx * 0.9 + 9.0, v * 0.9)) - 0.5;
    float x = cx + gTurb * (1.2 * wx + 0.45 * wx2) * (0.4 + 0.6 * smoothstep(0.0, 0.5, d));
    vec2 q = vec2(x * 0.55, v);
    float idx = fbm3(q * vec2(1.0, 0.32));
    // streak presence: falling segments of liquid; their lower ends are the pointed drip heads
    float ph1 = v * 0.2 + vn1(x * 0.7 + 4.0) * 3.0;
    float f1 = fract(ph1);
    float dens = mix(gDens, min(gDens + 0.15, 1.0), gPhys);
    float win = mix(0.2, 1.0, dens);
    float seg = smoothstep(0.0, 0.1, f1 - (1.0 - win) * 0.5) * smoothstep(0.0, 0.1, (1.0 + win) * 0.5 - f1);
    seg = max(seg, smoothstep(0.82, 0.97, dens));
    float colMask = smoothstep(0.62 - 0.6 * dens, 0.82 - 0.6 * dens, vn(vec2(x * 2.2 + 21.0, v * 0.06 + 4.0)));
    float cov = seg * colMask;
    // long luminous streaks that always trickle, even when sparse: tapered falling segments
    float sx = x * 2.4;
    float sid = floor(sx);
    float sf = fract(sx) - 0.5 - (h11(sid * 1.7 + 2.0) - 0.5) * 0.3;
    float sw0 = 0.07 + 0.11 * h11(sid + 3.0);
    float prof = exp(-sf * sf / (sw0 * sw0));
    float segN = vn(vec2(sid * 7.3, v * 0.3 + h11(sid) * 10.0));
    float strk = smoothstep(0.62 - 0.3 * prof, 0.7 - 0.3 * prof, segN) * prof * step(0.25, h11(sid + 9.0));
    gStrk = strk;
    cov = max(cov, strk);
    // aurora: thin luminous curtains
    float cur = pow(1.0 - abs(2.0 * vn(vec2(x * 1.1 + 40.0, v * 0.1)) - 1.0), 5.0);
    cov = mix(cov, cov * 0.25 + cur, gThin);
    cov *= mix(1.0, sim.z, gPhys);
    gAcc = smoothstep(0.52, 0.72, vn(vec2(x * 0.22 + 50.0, v * 0.07)));
    gAt = vn(vec2(x * 0.5 + 70.0, v * 0.2));
    gFold = vn(vec2(x * 1.6 + 13.0, v * 0.14 + 6.0));
    gStri = vn(vec2(x * 7.0 + 5.0, v * 0.12)) * 0.6 + 0.4 * vn(vec2(x * 15.0, v * 0.2 + 3.0));
    return vec3(idx, cov, idx + 0.15 * wx2);
}

// churning glossy source pool; returns (colour index, ripple height)
vec3 poolField(vec2 p) {
    float t = gChurn;
    vec2 q = vec2(p.x * 2.6, (p.y - gLip) * 4.5);
    vec2 w = vec2(vn(q * 0.8 + vec2(t * 0.3, -t * 0.2)), vn(q * 0.8 + vec2(5.2 - t * 0.25, 1.3 + t * 0.15))) - 0.5;
    q += w * (1.6 + gTurb * 0.6);
    float idx = fbm3(q * 1.1 + vec2(t * 0.15, 0.0));
    vec2 r = q * vec2(1.3, 5.0) + vec2(2.5, 1.2) * w + vec2(t * 0.35, 0.0);
    float rn = vn(r) + 0.5 * vn(r * 2.1 + vec2(-t * 0.5, 4.0));
    float ph = rn * 9.0 + t * 1.5;
    float rip = sin(ph) * 0.5 + 0.5;
    // thin glossy wave crests (contours of a churning height field), broken into segments
    float crest = 0.75 * pow(1.0 - abs(sin(ph * 0.5)), 22.0) * smoothstep(0.3, 0.65, vn(q * vec2(1.6, 2.5) + vec2(-t * 0.4, 9.0)));
    return vec3(idx, rip * 0.5 + idx * 0.5, crest);
}

// liquid thickness → height
float liqHeight(vec3 f) { float c = smoothstep(0.0, 1.0, f.y); return c * (0.6 + 0.4 * f.z) + 0.04 * gStri * c; }

// glossy wet liquid: wrapped key light, sharp wet specular + broad sheen, fresnel rim
vec3 liquidLight(vec3 base, vec3 n, float gloss) {
    vec3 L = normalize(vec3(-0.45, 0.6, 0.65));
    float dif = 0.6 + 0.4 * dot(n, L);
    vec3 Hh = normalize(L + vec3(0.0, 0.0, 1.0));
    float nh = max(dot(n, Hh), 0.0);
    float spec = pow(nh, 90.0) * 1.1 + pow(nh, 14.0) * 0.12;
    float fres = pow(1.0 - clamp(n.z, 0.0, 1.0), 3.0);
    float rim = fres * (0.35 + 0.65 * max(dot(n.xy, normalize(vec2(0.8, -0.3))), 0.0));
    vec3 c = base * dif;
    c += vec3(1.0, 0.98, 0.95) * spec * gloss;
    c += mix(base, vec3(1.0), 0.55) * rim * 0.75 * gloss;
    return c;
}

// slower, softer, darker curtains far behind the main fall (parallax depth)
vec3 backCurtain(vec2 p, float sA, float sB, float sw) {
    float d = max(gLip - p.y, 0.0);
    float vb = gClock * 0.55 - pow(d + 0.04, 0.72) * 8.0 / gStr;
    float xb = p.x * gCols * 1.4 + 17.0;
    float idx = fbm3(vec2(xb * 0.7, vb * 0.25));
    float cb = pow(1.0 - abs(2.0 * vn(vec2(xb * 1.6 + 9.0, vb * 0.08)) - 1.0), 3.0) * smoothstep(0.3, 0.6, vn(vec2(xb * 0.5 + 3.0, vb * 0.15)));
    float hb = idx * 1.6 + gClock * 0.04 + 0.5;
    vec3 c = mix(schemeCol(sA, hb, 0.0, 0.0), schemeCol(sB, hb, 0.0, 0.0), sw);
    return (c * c * 0.5 + c * 0.08) * cb * (0.6 + 0.4 * smoothstep(0.0, 0.5, d));
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    float asp = R.x / R.y;

    // direction: rotate the frame so "down" is the chosen way
    float dsel = float(dirSel);
    vec2 fu = uv;
    float aspF = asp;
    if (dsel > 0.5 && dsel < 1.5) fu.y = 1.0 - fu.y;
    else if (dsel > 1.5) { fu = vec2(uv.y, uv.x); aspF = 1.0 / asp; }
    if (PASSINDEX < 2) fu = uv;                          // state buffers live in fall space
    gAspF = aspF;
    gPhys = physicsAmt;
    vec2 p = vec2((fu.x - 0.5) * aspF, fu.y);

    // style morph
    float sty = float(styleSel);
    float ph = TIME / morphPeriod;
    float sA = sty > 0.5 ? sty - 1.0 : mod(floor(ph), 4.0);
    float sB = sty > 0.5 ? sA : mod(floor(ph) + 1.0, 4.0);
    // the next scheme pours in from the top: a soft front that sweeps down the frame
    float swT = sty > 0.5 ? 0.0 : smoothstep(0.7, 1.0, fract(ph));
    float swF = 1.25 - swT * 1.6;
    float sw = sty > 0.5 ? 0.0 : smoothstep(swF - 0.12, swF + 0.12, p.y + 0.06 * (vn1(p.x * 6.0) - 0.5));
    // per-style shape: thin curtains, darkness
    float thinA = sA > 2.5 ? 1.0 : 0.0, thinB = sB > 2.5 ? 1.0 : 0.0;
    float darkA = sA > 1.5 && sA < 2.5 ? 1.0 : 0.0, darkB = sB > 1.5 && sB < 2.5 ? 1.0 : 0.0;
    gThin = mix(thinA, thinB, sw);
    float dark = mix(darkA, darkB, sw);

    // clocks (additive, no time jumps)
    gClock = TIME * 0.55 * fallSpeed + audioBassTime * 0.22 * ar + 1.4 * gBuild * gBuild;
    gChurn = TIME * 0.5 + audioMidTime * 0.35 * ar + 2.0 * gBuild * gBuild;
    gTurb = turbulence * (1.0 + 0.6 * midP() * ar + 0.3 * gPeak - 0.3 * gRest);
    gStr = stretch * (1.0 + 0.6 * gRest - 0.35 * gBuild);
    gCols = columns * (1.0 - 0.35 * gRest + 0.5 * gBuild + 0.25 * gPeak);
    gSharp = clamp(dripSharp + 0.35 * gBuild, 0.0, 1.0);
    gDens = clamp(density * (1.0 - 0.5 * gRest) + 0.15 * gPeak + 0.1 * bassP() * ar, 0.0, 1.0);

    // source pool lip
    float srcH = sourceOn ? sourceHeight : 0.0;
    float lip = 1.0 - srcH;
    // build tremor near the top
    float trem = smoothstep(0.55, 1.0, gBuild) * smoothstep(lip - 0.4, 1.0, p.y);
    p.x += (vn1(TIME * 40.0) - 0.5) * 0.02 * trem;
    // the pool's lip itself wobbles
    lip += 0.025 * (vn1(p.x * 4.0 + gChurn * 0.6) - 0.5) * (srcH > 0.0 ? 1.0 : 0.0);
    gLip = srcH > 0.0 ? lip : 1.15;

    // drop: flood front crashing down from the top
    float dn = gDrop / max(dynamics, 0.01);
    float front = gLip - (1.0 - dn) * 1.5 - 0.05;
    float flood = gDrop > 0.01 ? smoothstep(front - 0.12, front + 0.05, p.y) * smoothstep(0.0, 0.25, gDrop) : 0.0;
    gDens = mix(gDens, 1.0, flood);

    // ═══ physics: liquid velocity field + carried material (state buffers, fall space) ═══
    float dt = clamp(TIMEDELTA, 0.0, 1.0 / 30.0);
    float G = 1.8 * gravity * fallSpeed * fallSpeed * (1.0 - 0.45 * gRest + 0.7 * gBuild + 0.25 * gPeak) + 0.02;
    float flowRate = 0.55 * fallSpeed + 0.22 * ar * smoothstep(0.0, 0.8, audioBass) + 1.0 * gBuild;
    float kick = audioBeatPulse * ar * kickForce;
    float dLip = max(gLip - p.y, 0.0);
    // which columns of the pool are pouring (drips detach when a column closes)
    float gx = p.x * (4.0 + 2.0 * gCols);
    float gate = smoothstep(0.78 - 0.75 * gDens, 0.98 - 0.75 * gDens, (0.7 * vn(vec2(gx + 21.0, gClock * 0.12)) + 0.3 * vn(vec2(gx * 2.3 + 4.0, gClock * 0.2))));
    float kcol = step(0.45, h11(floor(gx * 1.7) * 3.1 + floor(audioBassTime * 0.5)));
    gate = clamp(max(gate, kcol * smoothstep(0.1, 0.5, kick)) + flood, 0.0, 1.0);
    if (PASSINDEX == 0) {
        vec4 pv = texture2D(velBuf, uv);
        vec2 v;
        float spl = 0.0;
        bool bad = FRAMEINDEX < 2 || resetPhysics || pv.a < 0.5 || !(abs(pv.x) < 50.0 && abs(pv.y) < 50.0);
        if (bad) {
            v = vec2(0.0, -sqrt(2.0 * G * dLip));
        } else {
            vec2 back = uv - pv.xy * dt;
            v = texture2D(velBuf, back).xy;
            // viscosity: neighbouring streaks drag on each other and merge
            vec2 vnb = 0.5 * (texture2D(velBuf, back + vec2(0.006, 0.0)).xy + texture2D(velBuf, back - vec2(0.006, 0.0)).xy);
            v = mix(v, vnb, 0.35 * viscosity);
            v.y -= G * dt;                                               // gravity
            v.x += (vn(vec2(p.x * 7.0, p.y * 2.5 + gClock * 0.8)) - 0.5) * gTurb * 0.5 * dt;
            // the core of a streak falls faster than its edges (edge drag): heads turn pointed, tails stretch
            float aC = texture2D(dyeBuf, uv).b;
            float aS = 0.5 * (texture2D(dyeBuf, uv + vec2(0.012, 0.0)).b + texture2D(dyeBuf, uv - vec2(0.012, 0.0)).b);
            v.y -= clamp(aC - aS, 0.0, 1.0) * (G * 1.4 + 0.6) * dt;
            v.y += (1.0 - aC) * G * 0.25 * dt;
            v.x *= exp(-(2.0 + 3.0 * viscosity) * dt);
            v.y *= exp(-damping * dt);                                   // air drag → terminal velocity
            // kicks: an impulse that splashes liquid out of the pool and flings it down + sideways
            float nearTop = smoothstep(0.35, 0.0, dLip);
            v.y -= kick * kcol * nearTop * 9.0 * dt;
            v.x += (h11(floor(gx * 1.7) + 5.0) - 0.5) * kick * nearTop * 3.0 * dt;
            // drop: a flood surge pushing down behind the front
            v.y -= gDrop * kickForce * 3.5 * dt * smoothstep(-0.15, 0.1, p.y - front);
            // the pool: slow churning liquid that spills over the lip
            float inP = smoothstep(gLip - 0.005, gLip + 0.02, p.y);
            v = mix(v, vec2((vn(vec2(p.x * 3.0, gChurn * 0.4)) - 0.5) * 0.08, -0.06 - 0.25 * kick * kcol), inP);
            v = vec2(clamp(v.x, -0.6, 0.6), clamp(v.y, -3.0, 0.3));
            // mist: energy from liquid hitting the bottom edge, rising and fading
            spl = texture2D(velBuf, uv - vec2(0.0, 0.3) * dt).b * exp(-1.6 * dt);
            float amtB = texture2D(dyeBuf, uv).b;
            spl = max(spl, step(uv.y, 0.03) * amtB * clamp(-v.y * 0.4, 0.0, 1.5));
        }
        gl_FragColor = vec4(v, spl, 1.0);
        return;
    }
    if (PASSINDEX == 1) {
        vec4 pd = texture2D(dyeBuf, uv);
        vec2 v = texture2D(velBuf, uv).xy;
        vec3 st;
        bool bad = FRAMEINDEX < 2 || resetPhysics || pd.a < 0.5 || !(abs(pd.r) < 100.0);
        if (bad) {
            st = vec3(sqrt(2.0 * dLip / G) * flowRate, 0.0, 1.0);
        } else {
            vec2 back = uv - v * dt;
            vec4 d0 = texture2D(dyeBuf, back);
            float aN = 0.25 * (texture2D(dyeBuf, back + vec2(0.005, 0.0)).b + texture2D(dyeBuf, back - vec2(0.005, 0.0)).b
                             + texture2D(dyeBuf, back + vec2(0.0, 0.01)).b + texture2D(dyeBuf, back - vec2(0.0, 0.01)).b);
            st = vec3(d0.r + dt * flowRate, d0.g + back.x - uv.x, mix(d0.b, aN, 0.15 + 0.3 * viscosity));
            // the pool is the source: fresh material, poured only through open columns
            float inP = smoothstep(gLip - 0.005, gLip + 0.01, p.y);
            if (back.y > 0.999) inP = 1.0;
            st = mix(st, vec3(0.0, 0.0, gate), inP);
            st = vec3(clamp(st.x, 0.0, 8.0), clamp(st.y, -0.4, 0.4), clamp(st.z, 0.0, 1.0));
        }
        gl_FragColor = vec4(st, 1.0);
        return;
    }

    vec3 bg = backdrop(uv);
    float dd = gLip - p.y;
    // ── the source pool (also sampled at the lip so the fall pours out of it) ──
    // the lip hangs down in soft pointed tongues where the pool spills over
    float jr = 1.0 - abs(2.0 * vn1(p.x * gCols * 6.0 + gChurn * 0.15 + 3.0) - 1.0);
    float jag = (0.03 + 0.09 * vn1(p.x * gCols * 1.7 + 8.0)) * pow(jr, 1.0 + 3.0 * gSharp) * (1.0 + 0.6 * gBuild);
    float inPool = srcH > 0.0 ? smoothstep(gLip - jag - 0.03, gLip - jag + 0.015, p.y) : 0.0;
    float nearLip = srcH > 0.0 ? 1.0 - smoothstep(0.0, 0.2, dd) : 0.0;
    vec3 poolCol = vec3(0.0);
    float poolHue = 0.0;
    gPa = 0.0; gPt = 0.0;
    if (inPool > 0.0 || nearLip > 0.0) {
        vec2 pp = vec2(p.x, max(p.y, gLip + 0.01));
        float ep = 1.5 / RENDERSIZE.y + 0.003;
        vec3 P0 = vec3(0.0), Px = vec3(0.0), Py = vec3(0.0);
        for (int i = 0; i < 3; i++) {                    // one call site: +x, +y, centre
            vec2 o = i == 0 ? vec2(ep, 0.0) : (i == 1 ? vec2(0.0, ep) : vec2(0.0));
            vec3 r = poolField(pp + o);
            if (i == 0) Px = r; else if (i == 1) Py = r; else P0 = r;
        }
        float hP0 = P0.y + 0.35 * P0.z, hPx = Px.y + 0.35 * Px.z, hPy = Py.y + 0.35 * Py.z;
        vec3 pn = normalize(vec3(-(hPx - hP0) / ep * 0.02, -(hPy - hP0) / ep * 0.02, 1.0));
        poolHue = P0.x * 1.6 + gClock * 0.04 + p.x * 0.22;
        gPa = smoothstep(0.6, 0.8, vn(pp * vec2(3.0, 6.0) + vec2(gChurn * 0.2, 30.0)));
        gPt = vn(pp * 2.0 + vec2(-gChurn * 0.15, 60.0));
        poolCol = mix(schemeCol(sA, poolHue, gPa, gPt), schemeCol(sB, poolHue, gPa, gPt), sw);
        poolCol = liquidLight(poolCol, pn, sheen * (1.0 - 0.4 * dark) * (0.8 + 0.4 * gLift));
        poolCol += vec3(1.0) * P0.z * 0.35 * (0.6 + 0.6 * highP() * ar) * sheen * smoothstep(gLip - 0.01, gLip + 0.03, p.y) * inPool;
    }

    // ── fall field: the liquid's thickness is a height surface; normals by central differences ──
    float e = 1.5 / RENDERSIZE.y + 0.004;
    vec3 F = vec3(0.0), Fx = vec3(0.0), Fy = vec3(0.0);
    for (int i = 0; i < 3; i++) {                        // one call site: +x, +y, then the centre
        vec2 o = i == 0 ? vec2(e, 0.0) : (i == 1 ? vec2(0.0, e) : vec2(0.0));
        vec3 r = fallField(p + o, simAt(p + o));
        if (i == 0) Fx = r; else if (i == 1) Fy = r; else F = r;
    }
    float h0 = liqHeight(F), hx = liqHeight(Fx), hy = liqHeight(Fy);
    vec3 nrm = normalize(vec3(-(hx - h0) / e * 0.07, -(hy - h0) / e * 0.04, 1.0));

    float hue = F.x * 1.6 + gClock * 0.04 + p.x * 0.22;
    // the colour pours out of the pool: near the lip it carries the pool's hue
    float lipK = nearLip * nearLip;
    hue = mix(hue, poolHue, lipK);
    float acc = mix(gAcc, gPa, lipK), at = mix(gAt, gPt, lipK);
    vec3 cA = schemeCol(sA, hue, acc, at), cB = schemeCol(sB, hue, acc, at);
    vec3 fallCol = mix(cA, cB, sw);
    // thin rivulets with glints travelling down them
    float rx = p.x * gCols * 9.0 + 2.0 * (F.x - 0.5);
    float riv = pow(1.0 - abs(2.0 * vn1(rx) - 1.0), 18.0);
    float dash = smoothstep(0.55, 0.95, vn(vec2(rx * 0.5, dd * 27.0 / gStr + gClock * 6.0)));
    float spark = riv * dash * (0.25 + 1.6 * highP() * ar + 0.4 * gPeak);
    fallCol *= 0.86 + 0.24 * gStri;
    // deep folds between the streaks: richer, darker tones of the same colour
    vec3 deepC = mix(schemeCol(sA, hue + 0.28, 0.0, 0.0), schemeCol(sB, hue + 0.28, 0.0, 0.0), sw);
    fallCol = mix(deepC * deepC * 0.55, fallCol, smoothstep(0.12, 0.55, gFold));
    // caustic shimmer refracted through the moving liquid
    float caus = vn(p * vec2(11.0, 4.5) + nrm.xy * 2.5 + vec2(0.0, gClock * 1.6));
    fallCol *= 0.9 + 0.35 * caus * caus * (0.6 + 0.6 * highP() * ar);
    float gloss = sheen * (1.0 - 0.45 * dark);
    fallCol = liquidLight(fallCol, nrm, gloss);
    fallCol += vec3(1.0) * spark * 0.6 * gloss;
    float cov = F.y;
    cov = mix(cov, smoothstep(0.2, 0.8, cov), 0.5);       // foreground streaks: crisper edges
    fallCol *= 1.0 + 0.3 * gStrk;

    // depth: soft, darker background curtains falling slower behind (parallax), seen refracted at the edges
    vec3 backC = backCurtain(p + nrm.xy * 0.035 * cov, sA, sB, sw) * depthLayer;
    bg = mix(bg, bg * 0.4 + backC, smoothstep(0.0, 0.15, length(backC)));

    // beat: fresh bright drips pushed out of the pool — pointed tongues that shoot down
    float bp = audioBeatPulse * ar;
    float bx = p.x * gCols * 4.0 + 0.3 * (F.x - 0.5);
    float colId = floor(bx);
    float pick = step(0.5, h11(colId * 3.1 + floor(audioBassTime * 0.5)));
    float prof = clamp(1.0 - abs(fract(bx) - 0.5) * 2.0, 0.0, 1.0);
    float dl = (0.12 + 0.5 * h11(colId + 7.0)) * (1.0 - 0.7 * bp) + 0.06;
    float tongue = smoothstep(0.0, 0.08, pow(prof, 1.3) * dl - max(dd, 0.0)) * step(-0.05, dd);
    float drip = pick * bp * tongue * (1.0 - 0.7 * gPhys);
    vec3 dripCol = mix(cA, cB, sw);
    fallCol = mix(fallCol, dripCol * 1.25 + 0.15, drip * 0.8);
    cov = max(cov, drip);
    vec3 col;
    float alpha;

    // brightness / saturation / show states
    float bright = brightness * (0.9 + 0.08 * gLift + 0.12 * bassP() * ar) * mix(1.0, 0.82, dark);
    fallCol *= bright;
    poolCol *= bright;
    float satK = saturation * (1.0 + 0.2 * gPeak);
    fallCol = mix(vec3(lum(fallCol)), fallCol, satK);
    poolCol = mix(vec3(lum(poolCol)), poolCol, satK);
    // the flood glows as it lands
    fallCol += fallCol * flood * 0.45 * gDrop;
    // a frothy bright crest leads the flood down the frame
    float foam = exp(-pow((p.y - front) * 11.0, 2.0)) * smoothstep(0.02, 0.25, gDrop) * (0.7 + 0.6 * gStri);
    vec3 foamCol = sqrt(max(mix(fallCol, cA, 0.5), 0.0)) * 1.15 + 0.12;
    fallCol = mix(fallCol, foamCol, foam * 0.85);
    flood = max(flood, foam);
    cov = max(cov, foam * 0.9);

    // thin/soft edges keep their colour (deeper, richer) instead of greying over the backdrop
    vec3 edgeCol = mix(fallCol * fallCol * 1.35, fallCol, cov);
    col = mix(bg, edgeCol, cov);
    alpha = cov;
    col = mix(col, poolCol, inPool);
    alpha = max(alpha, inPool);

    // mist thrown up where the falling liquid hits the bottom edge
    float mist = texture2D(velBuf, fu).b * mistAmt * gPhys;
    float mistN = vn(p * vec2(14.0, 7.0) + vec2(0.0, -TIME * 0.7)) * 0.6 + 0.4 * vn(p * vec2(31.0, 15.0) + vec2(3.0, -TIME * 1.3));
    col += (sqrt(max(cA, 0.0)) * 0.6 + 0.35) * mist * mistN * smoothstep(0.45, 0.0, p.y) * 0.6;

    // drop flash
    col += vec3(1.0) * gDrop * gDrop * gDrop * 0.45;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    // film grain + dither
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.12;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(alpha, 0.0, 1.0) : 1.0);
}
