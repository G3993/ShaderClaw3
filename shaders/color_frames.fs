/*{
  "DESCRIPTION": "Color Frames — a bento poster of rounded panels on warm paper, each filled with a domain-warped iridescent liquid marble (candy pink/cyan/blue/green/yellow/orange). Solid accent panels carry black cut-out glyphs; a blue crosshair plate and a grainy dot round out the grid. Soft drop shadows, hairline ink outlines. Audio swells the flow and shifts the palette.",
  "CREDIT": "ShaderClaw — original liquid-bento composition",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Flow Speed",     "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "warpAmount",   "LABEL": "Marble Warp",    "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "paletteShift", "LABEL": "Palette Shift",  "TYPE": "float", "DEFAULT": 0.00, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "ink",          "LABEL": "Ink Pooling",    "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "outlineAmt",   "LABEL": "Outline",        "TYPE": "float", "DEFAULT": 0.90, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "shadowAmt",    "LABEL": "Drop Shadow",    "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "grain",        "LABEL": "Paper Grain",    "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "audioReact",   "LABEL": "Audio React",    "TYPE": "float", "DEFAULT": 0.60, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "bgColor",      "LABEL": "Paper Color",    "TYPE": "color", "DEFAULT": [0.905, 0.902, 0.892, 1.0] },
    { "NAME": "accentColor",  "LABEL": "Accent Orange",  "TYPE": "color", "DEFAULT": [0.97,  0.42,  0.13,  1.0] }
  ]
}*/

// ====================================================================
// Color Frames — liquid-marble bento poster.
//
// A virtual portrait canvas holds 13 hand-placed rounded panels:
//   • style 0 — iridescent liquid marble (domain-warped fbm → palette)
//   • style 1 — solid accent-orange plate with black cut-out shapes
//   • style 2 — flat blue plate with a crosshair + orbiting dot
//   • style 3 — grainy blue disc
//   • style 4 — tiny solid accent dot
//
// Panels cast soft down-right shadows on warm paper and carry a
// hairline ink outline. Audio swells the marble flow and nudges hue.
// ====================================================================

#define PI 3.14159265
#define NPANEL 13

float h12(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = h12(i);
    float b = h12(i + vec2(1.0, 0.0));
    float c = h12(i + vec2(0.0, 1.0));
    float d = h12(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * vnoise(p);
        p = p * 2.02 + vec2(11.3, 7.7);
        a *= 0.5;
    }
    return v;
}

// Signed distance to a rounded box centred at origin.
float sdRB(vec2 p, vec2 b, float r) {
    vec2 d = abs(p) - b + r;
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - r;
}

// Six-stop cyclic candy palette — the iridescent marble lives here.
vec3 PAL(float t) {
    t = fract(t) * 6.0;
    int i = int(t);
    float f = t - float(i);
    f = f * f * (3.0 - 2.0 * f);
    vec3 S0 = vec3(0.96, 0.55, 0.78);   // pink
    vec3 S1 = vec3(0.42, 0.82, 0.88);   // cyan
    vec3 S2 = vec3(0.20, 0.45, 0.86);   // blue
    vec3 S3 = vec3(0.42, 0.80, 0.52);   // green
    vec3 S4 = vec3(0.97, 0.88, 0.36);   // yellow
    vec3 S5 = vec3(0.98, 0.52, 0.20);   // orange
    if (i == 0) return mix(S0, S1, f);
    if (i == 1) return mix(S1, S2, f);
    if (i == 2) return mix(S2, S3, f);
    if (i == 3) return mix(S3, S4, f);
    if (i == 4) return mix(S4, S5, f);
    return mix(S5, S0, f);
}

// Per-panel layout. luv-space x runs 0..0.80, y runs 0..1 (y down).
void panelData(int idx, out vec2 c, out vec2 b, out float r,
                out int style, out float seed) {
    if (idx == 0)  { c = vec2(0.165, 0.190); b = vec2(0.135, 0.140); r = 0.055; style = 0; seed = 1.0; }
    else if (idx == 1)  { c = vec2(0.420, 0.220); b = vec2(0.100, 0.180); r = 0.030; style = 2; seed = 2.0; }
    else if (idx == 2)  { c = vec2(0.665, 0.230); b = vec2(0.115, 0.170); r = 0.055; style = 0; seed = 3.0; }
    else if (idx == 3)  { c = vec2(0.155, 0.470); b = vec2(0.065, 0.135); r = 0.028; style = 1; seed = 10.0; }
    else if (idx == 4)  { c = vec2(0.278, 0.460); b = vec2(0.055, 0.135); r = 0.028; style = 1; seed = 11.0; }
    else if (idx == 5)  { c = vec2(0.430, 0.500); b = vec2(0.110, 0.170); r = 0.060; style = 0; seed = 4.0; }
    else if (idx == 6)  { c = vec2(0.675, 0.530); b = vec2(0.105, 0.135); r = 0.050; style = 0; seed = 5.0; }
    else if (idx == 7)  { c = vec2(0.205, 0.610); b = vec2(0.125, 0.055); r = 0.055; style = 0; seed = 6.0; }
    else if (idx == 8)  { c = vec2(0.175, 0.825); b = vec2(0.125, 0.125); r = 0.075; style = 0; seed = 7.0; }
    else if (idx == 9)  { c = vec2(0.420, 0.730); b = vec2(0.070, 0.070); r = 0.070; style = 3; seed = 8.0; }
    else if (idx == 10) { c = vec2(0.360, 0.865); b = vec2(0.018, 0.018); r = 0.018; style = 4; seed = 12.0; }
    else if (idx == 11) { c = vec2(0.480, 0.890); b = vec2(0.080, 0.045); r = 0.045; style = 0; seed = 9.0; }
    else                { c = vec2(0.665, 0.820); b = vec2(0.115, 0.140); r = 0.030; style = 1; seed = 13.0; }
}

// Iridescent liquid marble inside one panel. luv ∈ [0,1]², y down.
vec3 liquid(vec2 luv, float seed, float t, float warp, float pshift, float inkAmt) {
    vec2 p = luv * vec2(2.4, 3.0) + seed * 7.13;
    for (int k = 0; k < 2; k++) {
        p += warp * vec2(fbm(p * 1.3 + t * 0.20 + seed),
                         fbm(p * 1.3 - t * 0.16 + seed + 4.0));
    }
    float n = fbm(p + t * 0.05);
    float m = fbm(p * 1.9 - n * 1.5 + seed);

    float h = n * 1.25 + m * 0.55 + pshift + luv.y * 0.22 + seed * 0.07;
    vec3 col = PAL(h);

    // Milky sheen where the secondary field crests.
    col = mix(col, vec3(0.98, 0.98, 0.97), smoothstep(0.62, 0.95, m) * 0.65);
    // Inky pooling where it sinks — the deep black bleed in the source art.
    col = mix(col, vec3(0.03, 0.035, 0.05), smoothstep(0.26, 0.04, m) * inkAmt);
    return col;
}

// Solid accent plate carrying black cut-out shapes. luv y down.
vec3 glyphPlate(vec2 luv, float seed, vec3 accent) {
    vec3 col = accent;
    vec3 K = vec3(0.04, 0.045, 0.06);
    if (seed > 12.5) {
        // "Device" — big black inset window with little detail dashes.
        float win = sdRB(luv - vec2(0.50, 0.56), vec2(0.34, 0.34), 0.05);
        col = mix(col, K, smoothstep(0.012, 0.0, win));
        for (int i = 0; i < 3; i++) {
            float fy = 0.34 + float(i) * 0.07;
            float ln = abs(luv.x - 0.34) + abs(luv.y - fy) * 6.0;
            col = mix(col, accent, smoothstep(0.05, 0.0, ln - 0.02) * step(luv.y, 0.52) * step(0.30, luv.y));
        }
        float knob = length((luv - vec2(0.50, 0.84)) * vec2(1.0, 1.4));
        col = mix(col, accent, smoothstep(0.05, 0.04, knob));
    } else if (seed > 10.5) {
        // Numeral-ish black mark.
        float stem = sdRB(luv - vec2(0.56, 0.52), vec2(0.10, 0.34), 0.04);
        float foot = sdRB(luv - vec2(0.50, 0.84), vec2(0.26, 0.07), 0.03);
        float serif = sdRB((luv - vec2(0.40, 0.22)) * mat2(0.92, -0.39, 0.39, 0.92),
                           vec2(0.05, 0.16), 0.03);
        float g = min(min(stem, foot), serif);
        col = mix(col, K, smoothstep(0.012, 0.0, g));
    } else {
        // Two vertical slot bars near the top.
        float s1 = sdRB(luv - vec2(0.38, 0.26), vec2(0.07, 0.16), 0.025);
        float s2 = sdRB(luv - vec2(0.62, 0.26), vec2(0.07, 0.16), 0.025);
        float hook = sdRB((luv - vec2(0.50, 0.74)) * mat2(0.87, -0.5, 0.5, 0.87),
                          vec2(0.30, 0.10), 0.06);
        float g = min(min(s1, s2), hook);
        col = mix(col, K, smoothstep(0.012, 0.0, g));
    }
    return col;
}

void main() {
    vec2 R = RENDERSIZE;
    float audio = clamp(audioReact, 0.0, 2.0);
    float t = TIME * (speed + audioBass * audio * 0.30) + audioLevel * audio * 0.4;
    float warp = warpAmount + audioMid * audio * 0.40;
    float pshift = paletteShift + audioHigh * audio * 0.15;

    // Centred, y-down coords. Virtual canvas: x∈[0,0.80], y∈[0,1].
    vec2 q = (gl_FragCoord.xy - 0.5 * R) / R.y;
    q.y = -q.y;
    vec2 v = q / 1.02 + vec2(0.40, 0.5);

    // ── Warm paper with faint grain ──
    vec3 col = bgColor.rgb;
    float g = vnoise(gl_FragCoord.xy * 1.7) - 0.5;
    col += g * grain * 0.05;

    // ── Soft drop shadows (one pass, behind every panel) ──
    vec2 shOff = vec2(0.007, 0.012);
    float shadow = 0.0;
    for (int i = 0; i < NPANEL; i++) {
        vec2 c, b; float r, seed; int style;
        panelData(i, c, b, r, style, seed);
        float sd = sdRB((v - shOff) - c, b, r);
        shadow = max(shadow, smoothstep(0.028, 0.0, sd));
    }
    col = mix(col, col * 0.72, shadow * shadowAmt);

    // ── Panels (front-most wins; the layout barely overlaps) ──
    float aa = max(fwidth(v.x + v.y), 1e-4) * 1.4;
    for (int i = 0; i < NPANEL; i++) {
        vec2 c, b; float r, seed; int style;
        panelData(i, c, b, r, style, seed);
        vec2 lp = v - c;
        float sd = sdRB(lp, b, r);
        float cov = smoothstep(aa, -aa, sd);
        if (cov <= 0.0) continue;

        vec2 luv = (lp + b) / (2.0 * b);   // 0..1 within panel, y down
        vec3 pc;
        if (style == 0) {
            pc = liquid(luv, seed, t, warp, pshift, ink);
        } else if (style == 1) {
            pc = glyphPlate(luv, seed, accentColor.rgb);
        } else if (style == 2) {
            pc = vec3(0.15, 0.42, 0.92);
            // crosshair
            float cx = smoothstep(0.010, 0.004, abs(luv.x - 0.5));
            float cy = smoothstep(0.010, 0.004, abs(luv.y - 0.5));
            float ring = abs(length(luv - 0.5) - 0.30);
            pc = mix(pc, vec3(0.06, 0.16, 0.45), max(cx, cy) * smoothstep(0.42, 0.0, length(luv - 0.5)));
            pc = mix(pc, vec3(0.06, 0.16, 0.45), smoothstep(0.012, 0.0, ring) * 0.4);
            // orbiting accent dot + a tilted paddle
            vec2 dotP = vec2(0.5 + 0.30 * cos(t * 0.6), 0.5 + 0.22 * sin(t * 0.6));
            pc = mix(pc, accentColor.rgb, smoothstep(0.045, 0.030, length((luv - dotP) * vec2(1.0, 1.0))));
            float pad = sdRB((luv - vec2(0.5)) * mat2(0.7, -0.7, 0.7, 0.7), vec2(0.035, 0.20), 0.035);
            pc = mix(pc, vec3(0.92, 0.94, 0.98), smoothstep(0.012, 0.0, pad) * 0.85);
        } else if (style == 3) {
            float gg = vnoise(luv * 90.0);
            pc = vec3(0.18, 0.44, 0.88) + (gg - 0.5) * 0.22;
            pc = mix(pc, vec3(0.10, 0.28, 0.62), smoothstep(0.30, 0.5, length(luv - 0.5)));
        } else {
            pc = accentColor.rgb;
        }

        // Hairline ink outline.
        float ol = smoothstep(aa, -aa, abs(sd) - 0.0024);
        pc = mix(pc, vec3(0.05, 0.055, 0.07), ol * outlineAmt);

        col = mix(col, pc, cov);
    }

    col = clamp(col, 0.0, 1.0);
    gl_FragColor = vec4(col, 1.0);
}
