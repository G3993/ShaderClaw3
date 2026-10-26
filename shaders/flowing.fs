/*{
  "DESCRIPTION": "Flowing — a spinning 3D string made of dozens of fine strands of glowing dots, winding around each other like a twisted rope that swells, pinches, loops out and back in while the whole string sweeps across the frame in a slow, re-shaping S-curve. Strands in front are brighter and larger, strands behind fade into the dark for real depth. Through the lines run a few thin comets of light: sparse white-hot heads with long fading tails, each riding its own strand at its own speed, so the rope stays calm and minimal but alive. Every so often the rope is struck by lightning: a jagged, forked electric discharge with a white-violet core and a coloured bloom jumps along and across the strands, the whole scene flashes as if lit from inside a storm cloud, the strands blaze, then everything decays fast with a stuttering afterglow, and the biggest strikes shake the frame with a low rumble. Bass swells the rope and spins it harder, beats flash the dots and throw small sparks of lightning, mids speed the flow and the comets, highs make the dots twinkle. Show Dynamics: at Rest only a few fine strands drift along a slim thread with one or two lazy comets and rare, far-off silent flashes; during a Build the rope coils tighter, twists and spins up, the dots shimmer faster, more comets race along it and distant flickers come quicker and quicker; at the Drop a huge branching strike tears down the whole rope with a blinding flash and shake as the strands burst open, before settling into a full, fat, colourful, storm-lit peak.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "3D", "Particles", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "strands",     "LABEL": "Strands",          "TYPE": "float", "AUDIO": false, "MIN": 8.0, "MAX": 72.0, "DEFAULT": 36.0, "GROUP": "String" },
    { "NAME": "ropeWidth",   "LABEL": "Thickness",        "TYPE": "float", "MIN": 0.03, "MAX": 0.35, "DEFAULT": 0.11, "GROUP": "String" },
    { "NAME": "bulge",       "LABEL": "Swell & Pinch",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "String" },
    { "NAME": "twist",       "LABEL": "Twist",            "TYPE": "float", "MIN": 0.0, "MAX": 4.0, "DEFAULT": 1.4, "GROUP": "String" },
    { "NAME": "looseness",   "LABEL": "Loose Loops",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "String" },
    { "NAME": "dotSpacing",  "LABEL": "Dot Spacing",      "TYPE": "float", "MIN": 0.004, "MAX": 0.04, "DEFAULT": 0.012, "GROUP": "Dots" },
    { "NAME": "dotSize",     "LABEL": "Dot Size",         "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Dots" },
    { "NAME": "lineMix",     "LABEL": "Solid Lines",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Dots" },
    { "NAME": "spin",        "LABEL": "Spin",             "TYPE": "float", "MIN": -2.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Motion" },
    { "NAME": "flowSpeed",   "LABEL": "Flow Speed",       "TYPE": "float", "MIN": -2.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Motion" },
    { "NAME": "sway",        "LABEL": "Sway",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "pathSel",     "LABEL": "Path", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Auto Morph", "Sweep", "Wave", "Spiral Drop"], "GROUP": "Motion" },
    { "NAME": "morphPeriod", "LABEL": "Path Morph (s)",   "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 40.0, "DEFAULT": 12.0, "GROUP": "Motion" },
    { "NAME": "dotColor",    "LABEL": "Dot Colour",       "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0], "GROUP": "Look" },
    { "NAME": "accentAmt",   "LABEL": "Colour Strands",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Look" },
    { "NAME": "depthFade",   "LABEL": "Depth Fade",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7, "GROUP": "Look" },
    { "NAME": "glow",        "LABEL": "Glow",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Look" },
    { "NAME": "streakAmt",   "LABEL": "Light Streaks",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light & Thunder" },
    { "NAME": "streakSpeed", "LABEL": "Streak Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Light & Thunder" },
    { "NAME": "streakCount", "LABEL": "Streak Count",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 16.0, "DEFAULT": 5.0, "GROUP": "Light & Thunder" },
    { "NAME": "thunderAmt",  "LABEL": "Thunder",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light & Thunder" },
    { "NAME": "thunderRate", "LABEL": "Strike Rate",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Light & Thunder" },
    { "NAME": "flashAmt",    "LABEL": "Sky Flash",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light & Thunder" },
    { "NAME": "boltColor",   "LABEL": "Bolt Colour",      "TYPE": "color", "DEFAULT": [0.55, 0.45, 1.0, 1.0], "GROUP": "Light & Thunder" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 3, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.05, 0.05, 0.06, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

// A rope of strands = projected helices around a 2D spine curve. Each pixel
// finds its spine parameter s and its across-coordinate n ∈ [-1,1]; every strand
// k sits at n = r_k·cos(θ_k + twist·s + spin), depth = sin(...). Dots repeat
// along the spine in arc length.

#define PI  3.14159265359
#define TAU 6.28318530718

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }

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

// ── spine: a cubic Bézier whose control points morph between path shapes ──
vec2 gP0, gP1, gP2, gP3;
float gT;
void pathPts(float k, out vec2 a, out vec2 b, out vec2 c, out vec2 d) {
    if (k < 0.5) {          // Sweep (ref): enters top-left, arcs over and drops bottom-right
        a = vec2(-1.25, 0.32); b = vec2(-0.1, 0.62); c = vec2(0.55, 0.15); d = vec2(0.32, -0.85);
    } else if (k < 1.5) {   // Wave: crosses the frame in an S
        a = vec2(-1.25, -0.15); b = vec2(-0.35, 0.75); c = vec2(0.35, -0.75); d = vec2(1.25, 0.15);
    } else {                // Spiral drop: falls from the top and curls
        a = vec2(-0.2, 0.85); b = vec2(0.75, 0.35); c = vec2(-0.75, -0.2); d = vec2(0.15, -0.9);
    }
}
vec2 spine(float s) {
    float u = 1.0 - s;
    vec2 p = u * u * u * gP0 + 3.0 * u * u * s * gP1 + 3.0 * u * s * s * gP2 + s * s * s * gP3;
    // slow organic sway on top of the path
    p += sway * vec2(0.05 * sin(s * 5.0 + gT * 0.7), 0.06 * sin(s * 4.0 - gT * 0.55 + 1.0)) * mix(0.6, 1.2, gLift);
    // tremor near the top of a build
    p += smoothstep(0.7, 1.0, gBuild) * 0.008 * vec2(sin(s * 31.0 + TIME * 43.0), sin(s * 27.0 - TIME * 51.0));
    return p;
}
vec2 spineD(float s) { return (spine(s + 0.002) - spine(s - 0.002)) / 0.004; }

// ── lightning ────────────────────────────────────────────────────────────
// piecewise-linear value noise → jagged polyline; 4 octaves = fractal zig-zag
float vn1(float x) { float i = floor(x); return mix(h11(i), h11(i + 1.0), fract(x)) - 0.5; }
float jag(float s, float sd) {
    return 1.10 * vn1(s * 7.0 + sd * 13.1) + 0.55 * vn1(s * 17.0 + sd * 7.7)
         + 0.26 * vn1(s * 41.0 + sd * 3.3) + 0.12 * vn1(s * 97.0 + sd * 5.9);
}
// across-rope offset (in rope widths) of a bolt path: weaves between the strands
float boltN(float s, float sd, float j) {
    return 0.7 * sin(s * (3.0 + 2.0 * h11(sd + j)) + sd * 4.0 + j * 2.0) + 1.6 * jag(s, sd + j * 3.17);
}
// envelope of a strike t seconds old: snap on, fast decay, stuttering re-strikes + afterglow
float strikeEnv(float t, float sd) {
    if (t < 0.0 || t > 1.6) return 0.0;
    float e = smoothstep(0.0, 0.012, t) * exp(-t * 9.0);
    e += 0.55 * exp(-abs(t - 0.09 - 0.05 * h11(sd)) * 60.0);                       // re-strike
    e += 0.30 * step(0.35, h11(sd + 2.0)) * exp(-abs(t - 0.24 - 0.1 * h11(sd + 1.0)) * 45.0);
    e *= 0.75 + 0.25 * step(0.4, h11(floor(t * 38.0) + sd * 3.1));                 // flicker
    return e + 0.12 * exp(-t * 2.2);                                                 // afterglow
}

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 p = (fc - 0.5 * R) / R.y;
    float px = 1.0 / R.y;
    float ar = audioReact;
    showDynamics();
    gT = TIME + 0.5 * audioMidTime * ar;

    // ── thunder events ──────────────────────────────────────────────────
    // random timer (always on when silent, rarer and far-off at rest)
    float silent = 1.0 - smoothstep(0.02, 0.12, audioLevel * ar);
    float restK = float(showState) > 0.5 ? gRest : gRest * (1.0 - silent);   // silence alone isn't "rest"
    float Pd = mix(14.0, 3.0, thunderRate) * (1.0 + 1.2 * restK);
    float cId = floor(TIME / Pd);
    float tmBolt = 0.0, tmSeed = 0.0;
    for (int i = 0; i < 2; i++) {
        float c = cId - float(i);
        float t0 = (c + 0.1 + 0.6 * h11(c * 3.7 + 1.0)) * Pd;
        float e = strikeEnv(TIME - t0, c) * step(h11(c * 1.9 + 7.0), 0.85) * (0.55 + 0.45 * h11(c * 5.3));
        if (e > tmBolt) { tmBolt = e; tmSeed = c; }
    }
    tmBolt *= mix(0.45, 1.0, silent);
    // beats / bass hits → small sparks; the drop → the huge one
    float beatB = smoothstep(0.35, 1.0, audioBeatPulse) * bassP() * ar * (0.35 + 0.65 * thunderRate) * (1.0 - 0.8 * restK);
    float dropT = -log(max(gDrop / max(dynamics, 1e-3), 1e-4)) / 5.0 * 4.0;   // seconds since the drop (approx)
    float dropB = gDrop > 0.002 ? strikeEnv(dropT * 0.9, 77.0) * smoothstep(0.0, 0.4, gDrop) * 1.6 : 0.0;
    // build → distant flickers that come faster and faster
    float fl = gBuild * step(1.0 - (0.04 + 0.4 * gBuild * gBuild), h11(floor(TIME * 13.0) + 0.5)) * (0.4 + 0.6 * h11(floor(TIME * 13.0) * 1.3));
    float restB = 1.0 - 0.85 * smoothstep(0.3, 0.9, restK);                      // rest: silent, far-off
    float boltE = max(max(tmBolt * restB, beatB * 0.7), dropB) * thunderAmt;
    float flashE = (max(max(tmBolt * mix(1.0, 0.45, restK), beatB * 0.35), dropB) + 0.45 * fl) * flashAmt * thunderAmt;
    float boltSz = clamp(max(max(tmBolt * 0.7, beatB * 0.35), dropB * 0.75 + 0.35 * step(0.01, dropB)), 0.0, 1.0); // reach & forks
    float bSeed = dropB > max(tmBolt, beatB) ? 77.0 + floor(TIME / 4.0) * 1.7 : (beatB > tmBolt ? floor(TIME * 2.0) * 0.37 + 300.0 : tmSeed + 11.0);
    // rumble shake on the strongest strikes
    float shk = smoothstep(0.7, 1.4, boltE) * 0.006;
    p += shk * vec2(vn1(TIME * 47.0), vn1(TIME * 41.0 + 9.0));

    // path morph
    float ps = float(pathSel);
    vec2 a0, b0, c0, d0, a1, b1, c1, d1;
    float f = 0.0;
    if (ps > 0.5) {
        pathPts(ps - 1.0, a0, b0, c0, d0); a1 = a0; b1 = b0; c1 = c0; d1 = d0;
    } else {
        float ph = TIME / morphPeriod;
        float k = mod(floor(ph), 3.0);
        f = smoothstep(0.55, 1.0, fract(ph));
        pathPts(k, a0, b0, c0, d0);
        pathPts(mod(k + 1.0, 3.0), a1, b1, c1, d1);
    }
    gP0 = mix(a0, a1, f); gP1 = mix(b0, b1, f); gP2 = mix(c0, c1, f); gP3 = mix(d0, d1, f);
    // control points breathe
    gP1 += sway * 0.12 * vec2(sin(TIME * 0.23), cos(TIME * 0.31));
    gP2 += sway * 0.12 * vec2(cos(TIME * 0.19 + 1.3), sin(TIME * 0.27 + 0.4));

    vec3 col = backdrop(uv);
    float alpha = 0.0;

    // nearest spine parameter: coarse search + Newton-ish refine
    float bs = 0.0, bd = 1e9;
    for (int i = 0; i <= 48; i++) {
        float s = -0.12 + float(i) * (1.24 / 48.0);
        vec2 q = spine(s) - p;
        float d = dot(q, q);
        if (d < bd) { bd = d; bs = s; }
    }
    for (int i = 0; i < 3; i++) {
        vec2 dp = spineD(bs);
        bs += dot(p - spine(bs), dp) / max(dot(dp, dp), 1e-5);
        bs = clamp(bs, -0.15, 1.15);
    }
    float s = bs;
    vec2 P = spine(s), D = spineD(s);
    float speed = length(D);
    vec2 T = D / max(speed, 1e-5);
    vec2 N = vec2(-T.y, T.x);
    vec2 rel = p - P;
    float along = dot(rel, T);

    // rope width along its length: swell & pinch, bass breathes it
    float w = ropeWidth * (1.0 + bulge * 0.6 * sin(s * 7.0 - TIME * 0.4) + bulge * 0.35 * sin(s * 13.0 + TIME * 0.3))
            * (1.0 + 0.35 * bassP() * ar)
            * (mix(0.55, 1.2, gLift) * (1.0 - 0.45 * gBuild) + 1.3 * gDrop);       // slim at rest, coils tight on the build, bursts on the drop
    w = max(w, 0.01);
    float n = dot(rel, N) / w;

    // ── the bolt: jagged trunk along/between the lines + forks ─────────────
    float sA = s + along / max(speed, 1e-3);
    vec3 boltL = vec3(0.0), hot = vec3(1.0);
    float boltA = 0.0;
    float boltProx = 0.0;
    if (boltE > 0.003) {
        float L  = mix(0.28, 1.25, boltSz);
        float s0 = mix(-0.1, 1.05 - L, h11(bSeed * 2.3));
        float s1 = s0 + L;
        float wB = clamp(w, 0.05, 0.14) * (1.0 + 1.2 * smoothstep(0.0, 0.5, dropB));                 // bolt lateral scale (screen units)
        float wS = wB / max(speed, 1e-3);
        float nS = n * w;
        float gl = 0.0, core = 0.0, inner = 0.0;
        for (int j = 0; j < 4; j++) {
            float fj = float(j);
            float on = j == 0 ? 1.0 : step(fj, 0.6 + 3.0 * boltSz);
            float a = s0, b = s1, nOff = 0.0, ww = 1.0;
            float u = 1.0;
            float dirB = h11(bSeed + fj * 4.1) < 0.5 ? -1.0 : 1.0;
            if (j > 0) {                                     // fork: leaves the trunk and veers off
                a = s0 + L * (0.12 + 0.25 * fj * h11(bSeed + fj));
                b = a + L * (0.18 + 0.2 * h11(bSeed + fj * 1.7));
                u = clamp((sA - a) / max(b - a, 1e-3), 0.0, 1.0);
                nOff = boltN(a, bSeed, 0.0) - boltN(a, bSeed, fj) + dirB * u * (0.8 + 0.8 * h11(bSeed + fj * 2.9));
                ww = 0.55 * (1.0 - 0.6 * u);
            }
            if (on < 0.5 || sA < a - 0.03 || sA > b + 0.03) continue;
            float nb  = boltN(sA, bSeed, fj) + nOff;
            float nb2 = boltN(sA + 0.003, bSeed, fj) + nOff;
            float sl = (nb2 - nb) / 0.003 * wS;              // screen slope of the bolt
            float d = abs(nS - nb * wB) / sqrt(1.0 + sl * sl);
            float tip = smoothstep(a - 0.004, a + 0.01, sA) * smoothstep(b + 0.004, b - (j == 0 ? 0.03 : 0.08), sA);
            float cw = max(0.0018 * ww * (0.7 + 1.2 * boltSz), px * 0.7);
            core = max(core, smoothstep(cw + px, max(cw - px, 0.0), d) * tip * ww);
            inner = max(inner, exp(-d * d / (0.0028 * 0.0028)) * tip * ww);
            gl += (exp(-d * d / (0.007 * 0.007)) * 0.7 + exp(-d / 0.045) * 0.35) * tip * ww;
        }
        hot = mix(vec3(1.0), mix(boltColor.rgb, vec3(1.0), 0.6), 0.35);   // white-violet core
        float fe = boltE * (0.85 + 0.15 * h11(floor(TIME * 60.0)));
        boltL = (hot * inner * 1.2 + boltColor.rgb * gl * 1.1) * fe;
        boltA = clamp(core * fe * 2.0, 0.0, 1.0);        // the core sits in front of the strands
        boltProx = clamp(gl * fe, 0.0, 2.0);
    }

    float spinPh = TIME * spin * 1.2 + 0.6 * audioBassTime * ar * sign(spin + 1e-4) + 5.0 * gBuild * gBuild * sign(spin + 1e-4);
    // additive clocks only (no TIME * changing-speed → no phase jumps)
    float flow = (TIME * flowSpeed + 0.8 * audioMidTime * ar * flowSpeed) * 0.12 + 0.05 * gBuild * gBuild * sign(flowSpeed + 1e-4);
    float twistK = twist + 1.6 * gBuild;
    float looseK = clamp(looseness * mix(0.4, 1.3, gLift) * (1.0 - 0.6 * gBuild) + 0.8 * gDrop, 0.0, 1.5);
    float brightK = 0.6 + 0.55 * gLift + 0.3 * gBuild;
    float endFade = smoothstep(-0.12, 0.02, s) * smoothstep(1.12, 0.98, s);

    vec3 acc = vec3(0.0);
    float cov = 0.0;
    if (abs(n) < 1.0 + 0.6 * looseK + 0.4 && endFade > 0.0) {
        float NSf = strands * mix(0.35, 1.0, smoothstep(0.1, 0.8, gLift));   // fewer strands at rest
        float NS = floor(strands);
        float arcPos = s * speed;                     // local arc-length coordinate
        for (int k = 0; k < 72; k++) {
            float fk = float(k);
            if (fk >= NS || fk >= ceil(NSf)) break;
            float onK = clamp(NSf - fk, 0.0, 1.0);
            float hk = h11(fk * 7.13 + 1.0);
            float th0 = fk / NS * TAU + hk * 0.5;
            // loose loops: some strands wander out from the rope and come back
            float loose = looseK * (0.25 * sin(s * (5.0 + hk * 6.0) + TIME * (0.3 + hk * 0.4) + fk) + 0.5 * step(0.8, hk) * smoothstep(0.2, 0.9, sin(s * 3.0 + fk * 2.1 + TIME * 0.2)));
            float rk = (0.75 + 0.3 * h11(fk * 3.7 + 2.0)) + loose;
            float ang = th0 + twistK * TAU * s + spinPh * (0.8 + 0.4 * hk);
            float sn = sin(ang), cs = cos(ang);
            float nk = rk * cs;
            float depth = sn * 0.5 + 0.5;                       // 1 = front
            // perpendicular distance in screen units, slope-corrected
            float slope = -rk * sn * twistK * TAU * w / max(speed, 1e-3);
            float dPerp = abs(n - nk) * w / sqrt(1.0 + slope * slope);
            if (dPerp > 0.02) continue;
            float bright = mix(1.0 - depthFade, 1.0, depth);
            float isLine = step(1.0 - lineMix, h11(fk * 5.31 + 9.0));
            // dots: spaced in arc length, streaming along each strand
            float spc = dotSpacing * (0.85 + 0.3 * h11(fk * 9.1)) * mix(1.5, 0.9, gLift);
            float ap = (arcPos + along) / spc - flow / spc * (0.7 + 0.6 * hk) + hk * 10.0;
            float da = (fract(ap) - 0.5) * spc;
            float rad = max(spc * 0.16 * (0.6 + 0.8 * depth) * dotSize * (1.0 + 0.6 * audioBeatPulse * ar + 0.9 * gDrop) * mix(0.8, 1.15, gLift), px * 0.6);
            float dd = length(vec2(da, dPerp));
            float dotv = smoothstep(rad + px, rad - px * 0.5, dd);
            float halo = exp(-dd * dd / (rad * rad * 9.0)) * 0.25 * glow;
            float lw = max(0.0011 * (0.6 + 0.6 * depth), px * 0.5);
            float line = smoothstep(lw + px, max(lw - px, 0.0), dPerp) * 0.55;
            float v = mix(dotv + halo, line + dotv * 0.4, isLine);
            // twinkle on highs
            v *= 1.0 + gBuild * 1.4 * step(0.5, fract(hk * 13.0 + floor(ap) * 0.37 + 22.0 * gBuild * gBuild));   // shimmer, rate climbs
            v *= 1.0 + highP() * ar * 1.5 * step(0.85, h12(vec2(fk, floor(ap))) ) * (0.5 + 0.5 * sin(TIME * 20.0 + fk));
            vec3 c = dotColor.rgb;
            float tint = step(1.0 - clamp(accentAmt * mix(0.4, 1.8, gLift) + 0.25 * gDrop, 0.0, 1.0), h11(fk * 2.71 + 4.0));
            c = mix(c, 0.5 + 0.5 * cos(TAU * (hk + TIME * 0.02 + vec3(0.0, 0.33, 0.67))), tint * 0.85);
            c = mix(c, vec3(1.0), 0.5 * gDrop);
            // storm light: the flash and a nearby bolt make the strands blaze, tinted toward the bolt
            float lit = (0.8 * flashE + 1.2 * boltProx) * (1.0 - 0.65 * smoothstep(0.0, 0.6, gDrop));
            c = mix(c, mix(boltColor.rgb, vec3(1.0), 0.5), clamp(lit * 0.25, 0.0, 0.3));
            vec3 strandL = c * v * bright * onK * brightK * (1.0 + 1.2 * gDrop) * (1.0 + lit);
            // light streak: a sparse comet riding this strand at its own speed
            float hs = h11(fk * 4.37 + 13.0);
            float nStk = streakCount * mix(0.5, 1.4, gLift) + 3.0 * gBuild;
            if (hs * NSf < nStk && streakAmt > 0.0) {
                float vel = (0.10 + 0.16 * h11(fk * 8.11 + 3.0)) * streakSpeed * (1.0 + 0.8 * gBuild);
                float ph = TIME * vel + 0.03 * audioMidTime * ar * streakSpeed + hs * 17.0 + h11(fk * 1.3) * 3.0;
                float cyc = 1.4 + 0.9 * h11(fk * 6.2);                          // gap between passes
                float dirS = flowSpeed < 0.0 ? -1.0 : 1.0;                      // comets travel with the flow
                float sH = 0.5 + dirS * (-0.75 + mod(ph, cyc) * (1.5 / 1.3));
                float dh = (sH - sA) * speed * dirS;                            // >0 behind the head
                float tl = 0.10 + 0.14 * h11(fk * 2.2 + 5.0);
                float tail = smoothstep(-0.004, 0.0015, dh) * exp(-max(dh, 0.0) / tl);
                float sw = max(0.0008 * (0.6 + 0.6 * depth), px * 0.55);
                float lineS = smoothstep(sw + px, max(sw - px, 0.0), dPerp);
                float headR = 0.0035 * (0.7 + 0.5 * depth);
                float hd = dh * dh + dPerp * dPerp;
                float head = exp(-hd / (headR * headR)) * 2.0 + 0.6 * exp(-sqrt(hd) / 0.018);   // hot head + bloom
                float stv = (lineS * tail * 1.6 + exp(-dPerp * dPerp / (0.004 * 0.004)) * tail * 0.4 + head)
                          * streakAmt * bright * onK * (1.0 + 0.6 * audioBeatPulse * ar * highP() + 0.8 * gDrop);
                vec3 sc = mix(mix(c, vec3(1.0), 0.6), vec3(1.0), clamp(head, 0.0, 1.0));   // white-hot head, tinted tail
                strandL += sc * stv;
                v = max(v, stv);
            }
            acc += strandL;
            cov = max(cov, v * bright * onK);
        }
    }
    acc *= endFade;
    cov *= endFade;
    col += acc;
    alpha = clamp(cov, 0.0, 1.0);

    // bolt + storm-cloud flash (soft, low-frequency so it reads as light, not a white frame)
    col += boltL * endFade;
    col = mix(col, hot * 1.3, boltA * endFade);
    float cloud = 0.55 + 0.45 * (vn1(p.x * 2.1 + bSeed * 3.0) + vn1(p.y * 2.7 - bSeed * 1.3 + 4.0));
    float near = exp(-abs(n) * w / 0.28) * endFade;
    vec3 flashC = mix(boltColor.rgb, vec3(0.85, 0.88, 1.0), 0.45);
    col += flashC * flashE * (0.06 * cloud + 0.26 * near);
    alpha = max(alpha, clamp(flashE * 0.15 + length(boltL) * 0.5 + boltA, 0.0, 1.0));
    // highlight shoulder: keeps white-hot cores from clipping into flat blobs
    col = mix(col, 0.8 + 0.2 * (1.0 - exp(-(col - 0.8) / 0.2)), step(0.8, col));

    col *= 1.0 + 0.12 * audioLevel * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
