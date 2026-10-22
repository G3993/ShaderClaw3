/*{
  "DESCRIPTION": "Flowing — a spinning 3D string made of dozens of fine strands of glowing dots. The strands wind around each other like a twisted rope, swelling, pinching, looping out and back in, while the whole string sweeps across the frame in a slow S-curve that sways and re-shapes itself. Dots stream along the strands so the rope seems to pour. Strands in front are brighter and larger, strands behind fade into the dark for real depth. Bass swells the rope and spins it harder, beats flash the dots, mids speed the flow, highs make the dots twinkle. Show Dynamics: at Rest only a few fine strands of soft, sparse dots drift along a slim, gently swaying thread; during a Build the rope coils tighter and thinner, twists harder and spins up, the dots densify, brighten and shimmer faster and the thread trembles; at the Drop the rope bursts open, its strands flung wide in big flashing dots before they settle into a full, fat, colourful peak.",
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
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
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

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 p = (fc - 0.5 * R) / R.y;
    float px = 1.0 / R.y;
    float ar = audioReact;
    showDynamics();
    gT = TIME + 0.5 * audioMidTime * ar;

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
            acc += c * v * bright * onK * brightK * (1.0 + 1.2 * gDrop);
            cov = max(cov, v * bright * onK);
        }
    }
    acc *= endFade;
    cov *= endFade;
    col += acc;
    alpha = clamp(cov, 0.0, 1.0);

    col *= 1.0 + 0.12 * audioLevel * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.06;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
