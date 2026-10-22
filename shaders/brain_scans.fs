/*{
  "DESCRIPTION": "Brain Scans — a living MRI built from a real brain scan (the MNI ICBM152 average human brain): folded cortex, white matter, ventricles, corpus callosum, cerebellum and brainstem, wrapped in a skull and scalp. A slice plane sweeps through it continuously and slowly rotates, so the view flows seamlessly from a sagittal profile through oblique cuts to an axial top-down slice and a coronal slice like a live scan, then melts into a contact-sheet series of slices and a rotating 3D cutaway of the whole brain, and the colour treatment morphs through thermal CRT rainbow, orange/green duotone, posterized contour, jet heatmap, clinical film and fluorescent fibre tracts. fMRI activation blooms with the music: bass lights the motor cortex, mids the auditory cortex, highs the visual cortex, and beats fire new hotspots and a scan flash. Show Dynamics: at Rest the scan pulls back and dims to a quiet, low-contrast resting state with only the default-mode network glowing faintly and breathing; during a Build hotspots multiply and spread across the cortex, the scan bar races and flickers faster, contrast and colour climb and the view pushes in with a slight tremor; at the Drop a white scan flash tears the image sideways and a shockwave of activation rips outward through the whole brain before it settles into full-brain peak activity.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original. Brain data: ICBM 152 Nonlinear 2009 template — Copyright (C) 1993-2004 Louis Collins, McConnell Brain Imaging Centre, Montreal Neurological Institute, McGill University (see brain_mri_LICENSE.txt).",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "viewSel",     "LABEL": "View", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5], "LABELS": ["Auto Morph", "Sagittal (profile)", "Axial (top)", "Coronal (front)", "Slice Series", "3D Cutaway"], "GROUP": "Scan" },
    { "NAME": "lookSel",     "LABEL": "Colour Look", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4, 5, 6], "LABELS": ["Auto Morph", "Thermal CRT", "Duotone", "Posterized", "Jet Heatmap", "Clinical Film", "Fluoro Tracts"], "GROUP": "Scan" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 10.0, "GROUP": "Scan" },
    { "NAME": "sweep",       "LABEL": "Slice Sweep",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Scan" },
    { "NAME": "sliceManual", "LABEL": "Slice Position",   "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Scan" },
    { "NAME": "seqMix",      "LABEL": "T1 ↔ T2 Contrast", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Scan" },
    { "NAME": "skullOn",     "LABEL": "Skull & Scalp",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Scan" },
    { "NAME": "gridCols",    "LABEL": "Series Columns",   "TYPE": "float", "AUDIO": false, "MIN": 2.0, "MAX": 6.0, "DEFAULT": 4.0, "GROUP": "Scan" },
    { "NAME": "gridRows",    "LABEL": "Series Rows",      "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.0, "GROUP": "Scan" },
    { "NAME": "rotSpeed",    "LABEL": "3D Rotation",      "TYPE": "float", "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.3, "GROUP": "Scan" },
    { "NAME": "zoom",        "LABEL": "Zoom",             "TYPE": "float", "AUDIO": false, "MIN": 0.6, "MAX": 1.6, "DEFAULT": 1.0, "GROUP": "Scan" },
    { "NAME": "activation",  "LABEL": "fMRI Activation",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Activity" },
    { "NAME": "contrast",    "LABEL": "Contrast",         "TYPE": "float", "MIN": 0.5, "MAX": 2.5, "DEFAULT": 1.45, "GROUP": "Finish" },
    { "NAME": "crt",         "LABEL": "CRT Pixels",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4, "GROUP": "Finish" },
    { "NAME": "scanline",    "LABEL": "Scan Bar",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Finish" },
    { "NAME": "glow",        "LABEL": "Edge Glow",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.02, 0.03, 0.05, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ],
  "IMPORTED": {
    "mriTex": { "PATH": "brain_mri.png" }
  }
}*/

// ─────────────────────────────────────────────────────────────────────────
// Brain Scans — single pass. The MRI volume (MNI152, 1.47 mm voxels, RAS mm:
// x right, y front, z up) is packed as axial slices in brain_mri.png:
//   R = T1 intensity, G = distance outside the brain (0..30 mm).
// A procedural skull + scalp is grown from that distance.
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
float bassP() { return smoothstep(0.05, 0.85, audioBass); }
float midP()  { return smoothstep(0.08, 0.85, audioMid); }
float highP() { return smoothstep(0.08, 0.9, audioHigh); }
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

// ── MRI atlas ────────────────────────────────────────────────────────────
const vec3  M_LO  = vec3(-75.393804, -110.393804, -71.393804);
const vec3  M_N   = vec3(103.0, 128.0, 107.0);
const float M_VOX = 1.474926;
const float M_TW = 105.0, M_TH = 130.0, M_COLS = 19.0, M_W = 1995.0, M_H = 781.0;
float gFlip;

vec2 mriSlice(vec2 xy, float k) {
    float tx = mod(k, M_COLS), ty = floor(k / M_COLS);
    float px = tx * M_TW + 1.0 + xy.x + 0.5;
    float py = 1.0 + ty * M_TH + 1.0 + xy.y + 0.5;
    float v = gFlip > 0.5 ? py / M_H : 1.0 - py / M_H;
    return texture2D(mriTex, vec2(px / M_W, v)).rg;
}
// (T1 intensity, mm outside the brain)
vec2 mri(vec3 p) {
    vec3 g = (p - M_LO) / M_VOX;
    vec3 gc = clamp(g, vec3(0.0), M_N - 1.0);
    float outside = length(g - gc) * M_VOX;
    float k0 = floor(gc.z);
    float k1 = min(k0 + 1.0, M_N.z - 1.0);
    vec2 a = mriSlice(gc.xy, k0), b = mriSlice(gc.xy, k1);
    vec2 r = mix(a, b, gc.z - k0);
    return vec2(r.x * step(outside, 0.5), r.y * 30.0 + outside);
}

float gT, gSeq, gZoom, gContrast;

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

// ── tissue: real brain + grown skull/scalp ───────────────────────────────
// returns intensity (T1↔T2), -1 outside the head; brain membership, activation
float tissue(vec3 p, out float brainW, out float actW) {
    brainW = 0.0; actW = 0.0;
    vec2 m = mri(p);
    float I = m.x, d = m.y;
    float t1, t2;
    if (d < 0.6) {
        brainW = 1.0;
        t1 = I;
        // T2-like: CSF bright, gray mid, white dark
        t2 = I < 0.04 ? 0.0 : mix(1.0, 0.2, smoothstep(0.12, 0.85, I));
    } else {
        float sk = skullOn;
        if (d > 15.0 || sk <= 0.0) return -1.0;
        float wob = 0.8 * sin(p.x * 0.11 + p.y * 0.07) + 0.6 * sin(p.y * 0.13 - p.z * 0.09);
        float dd = d + wob;
        // CSF rim → skull (dark cortical bone, bright diploë) → scalp (fat bright)
        float csf = smoothstep(0.6, 1.2, dd) * (1.0 - smoothstep(2.4, 3.0, dd));
        float bone = smoothstep(2.6, 3.2, dd) * (1.0 - smoothstep(8.2, 8.8, dd));
        float dip = smoothstep(4.2, 4.8, dd) * (1.0 - smoothstep(6.4, 7.0, dd));
        float scalp = smoothstep(8.6, 9.4, dd) * (1.0 - smoothstep(13.5, 14.6, dd));
        float tex = 0.85 + 0.3 * vn3(p * 0.35);
        t1 = csf * 0.1 + bone * 0.05 + dip * 0.62 * tex + scalp * mix(0.55, 0.9, smoothstep(9.0, 12.0, dd)) * tex;
        t2 = csf * 0.95 + bone * 0.04 + dip * 0.5 * tex + scalp * 0.6 * tex;
        if (d > 14.6 - wob * 0.0 && scalp + bone + dip + csf < 0.01) return -1.0;
        t1 *= sk; t2 *= sk;
        if (sk < 1.0 && t1 + t2 < 0.02) return -1.0;
    }
    // fMRI activation (MNI coordinates, mm)
    if (brainW > 0.5) {
        vec3 q = vec3(abs(p.x), p.y, p.z);
        float a1 = exp(-dot(q - vec3(36.0, -22.0, 58.0), q - vec3(36.0, -22.0, 58.0)) / 260.0) * bassP();     // motor
        float a2 = exp(-dot(q - vec3(54.0, -20.0, 6.0), q - vec3(54.0, -20.0, 6.0)) / 220.0) * midP();       // auditory
        float a3 = exp(-dot(q - vec3(12.0, -88.0, 4.0), q - vec3(12.0, -88.0, 4.0)) / 240.0) * highP();      // visual
        float k = floor(TIME * 0.5);
        vec3 hp = vec3((h11(k) - 0.5) * 110.0, (h11(k + 1.0) - 0.5) * 140.0 - 15.0, (h11(k + 2.0) - 0.3) * 80.0);
        float a4 = exp(-dot(p - hp, p - hp) / 300.0) * (0.3 + audioBeatPulse);
        // only light up gray matter, like real activation maps
        float gm = smoothstep(0.15, 0.35, I) * (1.0 - smoothstep(0.7, 0.85, I) * 0.6);
        actW = (a1 + a2 + a3 + a4) * audioReact * mix(0.35, 1.0, gLift);
        // show dynamics (independent of audioReact so the VJ states read in silence)
        // rest: default-mode network (medial prefrontal + posterior cingulate/precuneus), slowly breathing
        float dmn = exp(-dot(q - vec3(4.0, 52.0, 8.0), q - vec3(4.0, 52.0, 8.0)) / 380.0)
                  + exp(-dot(q - vec3(4.0, -55.0, 30.0), q - vec3(4.0, -55.0, 30.0)) / 420.0);
        float st = dmn * (0.35 + 0.2 * sin(TIME * 0.7)) * gRest;
        // build: extra hotspots popping faster and spreading; peak: whole-cortex activity
        float kb = floor(TIME * 2.0);
        for (int j = 0; j < 3; j++) {
            float fj = float(j);
            vec3 hb = vec3((h11(kb + fj * 7.0) - 0.5) * 120.0, (h11(kb + fj * 7.0 + 1.0) - 0.5) * 150.0 - 15.0, (h11(kb + fj * 7.0 + 2.0) - 0.3) * 80.0);
            st += exp(-dot(p - hb, p - hb) / mix(200.0, 600.0, gBuild)) * (gBuild * 1.6 + gPeak * 0.5);
        }
        float nz = vn3(p * 0.045 + vec3(0.0, 0.0, TIME * 0.4));
        st += smoothstep(0.45, 0.8, nz) * gPeak * 0.8;
        st += smoothstep(0.85 - 0.4 * gBuild, 0.95 - 0.3 * gBuild, nz) * gBuild * 1.1;   // activation spreads as the build rises
        // drop: shockwave of activation from the brain centre
        float rw = (1.0 - gDrop) * 110.0;
        st += exp(-pow((length(p - vec3(0.0, -18.0, 15.0)) - rw) / 9.0, 2.0)) * gDrop * 1.6;
        actW = (actW + st) * activation * gm;
    }
    return mix(t1, t2, gSeq);
}

// ── colour looks ─────────────────────────────────────────────────────────
vec3 rainbowMap(float t) {   // blue → cyan → green → yellow → red → magenta
    t = clamp(t, 0.0, 1.0);
    vec3 c = mix(vec3(0.05, 0.1, 0.7), vec3(0.1, 0.8, 1.0), smoothstep(0.0, 0.25, t));
    c = mix(c, vec3(0.2, 0.9, 0.4), smoothstep(0.22, 0.4, t));
    c = mix(c, vec3(1.0, 0.95, 0.1), smoothstep(0.4, 0.58, t));
    c = mix(c, vec3(1.0, 0.15, 0.08), smoothstep(0.58, 0.78, t));
    c = mix(c, vec3(1.0, 0.1, 0.65), smoothstep(0.82, 1.0, t));
    return c;
}
vec3 jet(float t) {
    t = clamp(t, 0.0, 1.0);
    return clamp(vec3(1.5 - abs(4.0 * t - 3.0), 1.5 - abs(4.0 * t - 2.0), 1.5 - abs(4.0 * t - 1.0)), 0.0, 1.0);
}

vec3 look(float k, float I, float brainW, float actW, vec2 fibre, vec2 sp, float edge) {
    if (I < 0.0) return vec3(0.0);
    float c = clamp((I - 0.5) * gContrast + 0.5, 0.0, 1.0);
    vec3 col;
    if (k < 0.5) {                                   // Thermal CRT (ref: rainbow MRI on a monitor)
        float q = floor(c * 9.0) / 9.0;
        col = rainbowMap(q * 0.95 + 0.02);
        col = mix(col, vec3(1.0, 0.95, 0.3), smoothstep(0.08, 0.0, abs(fract(c * 9.0) - 0.0)) * 0.6);
        col += vec3(1.0, 0.9, 0.2) * edge * 0.8 * glow;
    } else if (k < 1.5) {                            // Duotone: brain cool, body warm
        vec3 warm = mix(vec3(0.25, 0.03, 0.0), vec3(1.0, 0.45, 0.05), smoothstep(0.1, 0.6, c));
        warm = mix(warm, vec3(1.0, 0.85, 0.35), smoothstep(0.6, 0.95, c));
        vec3 cool = mix(vec3(0.05, 0.25, 0.15), vec3(0.3, 0.95, 0.65), smoothstep(0.2, 0.6, c));
        cool = mix(cool, vec3(0.6, 0.35, 1.0), smoothstep(0.62, 0.9, c));
        col = mix(warm, cool, brainW);
        col += mix(vec3(1.0, 0.5, 0.2), vec3(0.4, 0.8, 1.0), brainW) * edge * 0.7 * glow;
    } else if (k < 2.5) {                            // Posterized contour (green brain, blue body)
        float q = floor(c * 7.0) / 7.0;
        vec3 body = mix(vec3(0.02, 0.05, 0.35), vec3(0.2, 0.6, 1.0), q);
        vec3 brain = mix(vec3(0.0, 0.3, 0.05), vec3(0.4, 0.95, 0.2), q);
        brain = mix(brain, vec3(0.9, 0.1, 0.15), smoothstep(0.85, 0.95, q) * 0.7);
        col = mix(body, brain, brainW);
        float iso = smoothstep(0.1, 0.0, abs(fract(c * 7.0) - 0.5) - 0.42);
        col = mix(col, col * 0.3, iso * 0.6);
        col += vec3(0.2, 0.5, 1.0) * edge * 0.6 * glow;
    } else if (k < 3.5) {                            // Jet heatmap
        col = jet(c * 0.92);
        col *= 0.35 + 0.65 * smoothstep(0.02, 0.15, c);
        col += vec3(0.6, 0.9, 1.0) * edge * 0.5 * glow;
    } else if (k < 4.5) {                            // Clinical film
        col = vec3(0.62, 0.86, 0.95) * pow(c, 1.15);
        col += vec3(0.7, 0.9, 1.0) * edge * 0.35 * glow;
    } else {                                         // Fluoro tracts: fibres + speckle on black
        float fdir = atan(fibre.y, fibre.x);
        vec3 tract = 0.5 + 0.5 * cos(TAU * (fdir / TAU + vec3(0.0, 0.33, 0.67)));
        float lines = smoothstep(0.55, 1.0, sin((sp.x * fibre.y - sp.y * fibre.x) * 240.0 + h12(floor(sp * 40.0)) * 6.0));
        float speck = step(0.985, h12(floor(sp * 420.0))) * (0.5 + 0.5 * sin(TIME * 3.0 + h12(floor(sp * 420.0)) * 30.0));
        col = tract * (lines * 0.7 + 0.15) * smoothstep(0.3, 0.8, c) * brainW;
        col += mix(vec3(0.2, 0.9, 1.0), vec3(1.0, 0.85, 0.2), brainW) * speck * 1.2;
        col += mix(vec3(0.1, 0.8, 0.9), vec3(0.9, 0.9, 0.2), sp.y * 2.0 + 0.5) * edge * 1.2 * glow;
        col += vec3(0.15, 0.2, 0.3) * c * (1.0 - brainW) * 0.5;
    }
    // fMRI overlay: hot spots in red → yellow (heatmap looks get white-hot)
    vec3 hot = mix(vec3(1.0, 0.1, 0.05), vec3(1.0, 0.95, 0.3), smoothstep(0.3, 1.0, actW));
    col = mix(col, hot, clamp(actW * 1.4, 0.0, 0.9));
    return col;
}

vec3 backdrop(vec2 uv) {
    float m = float(bgMode);
    if (m < 0.5) return bgColor.rgb;
    if (m < 1.5) return mix(bgColor2.rgb, bgColor.rgb, smoothstep(0.0, 1.0, uv.y));
    vec2 dd = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    return mix(bgColor.rgb, bgColor2.rgb, smoothstep(0.0, 0.85, length(dd)));
}

// point in the head for a slice view (mm); s in [-1, 1] is the slice position
// The slice plane rotates continuously: view 0 = sagittal, 1 = axial, 2 = coronal,
// and every value in between is an oblique cut — so views morph like a live scan.
vec3 slicePoint(float view, vec2 q, float s) {
    float S = 235.0 / gZoom;
    float v = clamp(view, 0.0, 2.0);
    float a = smoothstep(0.0, 1.0, clamp(v, 0.0, 1.0));
    float b = smoothstep(0.0, 1.0, clamp(v - 1.0, 0.0, 1.0));
    // basis per view: U (screen x), V (screen y), N (slice axis); origin; slice range
    vec3 U = normalize(mix(mix(vec3(0.0, -1.0, 0.0), vec3(1.0, 0.0, 0.0), a), vec3(1.0, 0.0, 0.0), b));
    vec3 V = mix(mix(vec3(0.0, 0.0, 1.0), vec3(0.0, 1.0, 0.0), a), vec3(0.0, 0.0, 1.0), b);
    vec3 N = mix(mix(vec3(1.0, 0.0, 0.0), vec3(0.0, 0.0, 1.0), a), vec3(0.0, 1.0, 0.0), b);
    // live-scan drift: the plane slowly tilts a little off-axis
    float w1 = 0.12 * sin(gT * 0.23), w2 = 0.1 * sin(gT * 0.17 + 1.3);
    N = normalize(N + U * w1 + V * w2);
    V = normalize(V - N * dot(V, N));
    U = normalize(cross(V, N)) * sign(dot(cross(V, N), U) + 1e-4);
    vec3 O = mix(mix(vec3(0.0, -18.0, 6.0), vec3(0.0, -18.0, 12.0), a), vec3(0.0, -18.0, 4.0), b);
    float rng = mix(mix(62.0, 58.0, a), 82.0, b);
    return O + U * q.x * S + V * q.y * S + N * s * rng;
}

// renders one 2D view at screen point; returns rgb + coverage
vec4 renderSlice(float view, float lk, vec2 fc, vec2 R, float sBase) {
    vec2 q; float s = sBase; float tileEdge = 0.0;
    float v = view;
    float px = 1.0 / R.y;
    if (view > 2.5) {
        // series grid: a film sheet of axial slices with a sagittal row
        float cols = floor(gridCols + 0.5), rows = floor(gridRows + 0.5);
        float cell = min(R.x / cols, R.y / rows);
        vec2 org = 0.5 * (R - cell * vec2(cols, rows));
        vec2 g = (fc - org) / cell;
        if (g.x < 0.0 || g.y < 0.0 || g.x >= cols || g.y >= rows) return vec4(0.0);
        vec2 ti = floor(g);
        float idx = ti.x + (rows - 1.0 - ti.y) * cols;
        float n = cols * rows;
        q = (fract(g) - 0.5) * 1.05;
        v = mod(floor(gT * 0.08) + (idx < n * 0.5 ? 0.0 : 1.0), 3.0) < 1.5 ? 1.0 : 0.0;
        s = mix(0.95, -0.85, (idx + 0.5) / n) + 0.06 * sin(gT * 0.4 + idx);
        if (v < 0.5) s = mix(-0.85, 0.85, fract((idx + 0.5) / max(n * 0.5, 1.0)));
        vec2 fe = abs(fract(g) - 0.5);
        tileEdge = smoothstep(0.497, 0.4995, max(fe.x, fe.y));
        px = 1.05 / cell;
    } else {
        q = (fc - 0.5 * R) / R.y;
    }
    vec3 p = slicePoint(v, q, s);
    float bW, aW;
    float I = tissue(p, bW, aW);
    float b2, a2, b3, a3;
    float Ix = tissue(slicePoint(v, q + vec2(px * 1.5, 0.0), s), b2, a2);
    float Iy = tissue(slicePoint(v, q + vec2(0.0, px * 1.5), s), b3, a3);
    if (I < -0.5 && Ix < -0.5 && Iy < -0.5) return vec4(vec3(0.55, 0.7, 0.8) * tileEdge * 0.6, tileEdge * 0.6);
    vec2 gr = vec2(max(Ix, 0.0) - max(I, 0.0), max(Iy, 0.0) - max(I, 0.0));
    float edge = (I < -0.5 || Ix < -0.5 || Iy < -0.5) ? 1.0 : smoothstep(0.05, 0.3, length(gr));
    vec2 fib = normalize(vec2(-gr.y, gr.x) + 1e-5);
    vec3 col = I < -0.5 ? look(lk, 0.0, max(b2, b3), 0.0, fib, q, 1.0) * 0.8 : look(lk, I, bW, aW, fib, q, edge);
    col += vec3(0.55, 0.7, 0.8) * tileEdge * 0.6;
    return vec4(col, 1.0);
}

// 3D cutaway: raymarch the brain volume, cut by a plane that sweeps with s
vec4 render3D(float lk, vec2 fc, vec2 R, float s) {
    vec2 q = (fc - 0.5 * R) / R.y;
    float a = TIME * rotSpeed * 0.5 + 0.6;
    vec3 c = vec3(0.0, -18.0, 8.0);
    float dist = 470.0 / gZoom;
    vec3 ro = c + vec3(sin(a) * dist, cos(a) * dist, 70.0 + 50.0 * sin(TIME * 0.13));
    vec3 f = normalize(c - ro);
    vec3 r = normalize(cross(f, vec3(0.0, 0.0, 1.0)));
    vec3 u = cross(r, f);
    vec3 rd = normalize(f * 1.9 + r * q.x + u * q.y);
    // the cut plane: faces the camera side, sweeping through the brain
    vec3 cn = normalize(vec3(sin(a + 0.9), cos(a + 0.9), 0.35));
    float cd = s * 55.0;
    // box
    vec3 lo = M_LO - 2.0, hi = M_LO + M_N * M_VOX + 2.0;
    vec3 inv = 1.0 / rd;
    vec3 t0 = (lo - ro) * inv, t1 = (hi - ro) * inv;
    vec3 tn3 = min(t0, t1), tf3 = max(t0, t1);
    float tn = max(max(tn3.x, tn3.y), tn3.z), tf = min(min(tf3.x, tf3.y), tf3.z);
    if (tf < max(tn, 0.0)) return vec4(0.0);
    float t = max(tn, 0.0);
    float glowAcc = 0.0;
    bool prevCut = true;
    for (int i = 0; i < 150; i++) {
        if (t > tf) break;
        vec3 p = ro + rd * t;
        bool cut = dot(p - c, cn) > cd;
        vec2 m = mri(p);
        if (!cut) {
            // ghostly scalp shell
            glowAcc += exp(-pow((m.y - 11.0) / 2.5, 2.0)) * 0.035 * skullOn;
            if (m.y < 0.6 && m.x > 0.22) {
                // refine
                float ta = t - 2.2, tb = t;
                for (int j = 0; j < 4; j++) {
                    float tm = 0.5 * (ta + tb);
                    vec3 pm = ro + rd * tm;
                    vec2 mm = mri(pm);
                    bool inside = dot(pm - c, cn) <= cd && mm.y < 0.6 && mm.x > 0.22;
                    if (inside) tb = tm; else ta = tm;
                }
                t = tb; p = ro + rd * t; m = mri(p);
                float bW, aW;
                float I = tissue(p, bW, aW);
                vec3 col;
                if (prevCut && abs(dot(p - c, cn) - cd) < 3.0) {
                    // on the cut face: the slice, flat
                    col = look(lk, I, 1.0, aW, vec2(1.0, 0.0), q, 0.0);
                    col *= 0.9 + 0.1 * dot(cn, -rd);
                } else {
                    vec3 e = vec3(1.5, 0.0, 0.0);
                    vec3 n = -normalize(vec3(mri(p + e.xyy).x - mri(p - e.xyy).x, mri(p + e.yxy).x - mri(p - e.yxy).x, mri(p + e.yyx).x - mri(p - e.yyx).x) + 1e-5);
                    vec3 L = normalize(-rd + u * 0.6 + r * 0.3);
                    float dif = clamp(dot(n, L), 0.0, 1.0);
                    float fres = pow(1.0 - clamp(dot(n, -rd), 0.0, 1.0), 2.5);
                    float sh = 0.25 + 0.75 * dif;
                    vec3 base = look(lk, clamp(0.2 + 0.75 * sh, 0.0, 1.0), 1.0, aW, n.xy, q, 0.0);
                    col = base * (0.35 + 0.65 * sh) + base * fres * 0.6 * glow;
                }
                col += vec3(0.6, 0.8, 1.0) * glowAcc;
                return vec4(col, 1.0);
            }
            prevCut = false;
        } else prevCut = true;
        float stp = m.y > 6.0 ? max(m.y * 0.6, 2.0) : 2.0;
        t += cut ? 2.0 : stp;
    }
    return vec4(vec3(0.6, 0.8, 1.0) * glowAcc, clamp(glowAcc, 0.0, 1.0));
}

vec4 renderView(float view, float lk, vec2 fc, vec2 R, float s) {
    if (view > 3.5) return render3D(lk, fc, R, s);
    return renderSlice(view, lk, fc, R, s);
}

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    float ar = audioReact;
    showDynamics();
    gFlip = texture2D(mriTex, vec2(0.5 / M_W, 0.5 / M_H)).b > 0.5 ? 1.0 : 0.0;
    gT = TIME + 0.4 * audioMidTime * ar;
    gZoom = zoom * (mix(0.86, 1.06, gLift) + 0.18 * gBuild + 0.06 * gDrop);          // pulls back at rest, pushes in on the build
    gContrast = contrast * (mix(0.72, 1.15, gLift) + 0.15 * gBuild);
    // tremor near the top of the build, and a sideways scan tear on the drop
    float trem = smoothstep(0.7, 1.0, gBuild);
    fc += trem * 1.6 * vec2(sin(TIME * 47.0), sin(TIME * 59.0 + 1.3)) * R.y / 512.0;
    float band = floor(fc.y / R.y * 18.0);
    fc.x += (h11(band + floor(TIME * 20.0)) - 0.5) * 0.12 * R.x * gDrop * step(0.55, h11(band * 1.7 + floor(TIME * 20.0)));
    gSeq = clamp(seqMix + 0.2 * sin(TIME * 0.11), 0.0, 1.0);

    float ph = TIME / morphPeriod;
    // continuous slice orientation: sagittal → axial → coronal → axial → … (eased, never cuts)
    float orient = 1.0 - cos(TIME * PI / (2.0 * morphPeriod) + 0.4 * gBuild * gBuild);
    // auto: two periods of live rotating slices, then the slice series, then the 3D cutaway,
    // each melting into the next with a long dissolve
    float seg = mod(floor(ph), 4.0);
    float vsel = float(viewSel);
    float kind = seg < 1.5 ? 0.0 : seg < 2.5 ? 3.0 : 4.0;
    float nseg = mod(seg + 1.0, 4.0);
    float kindB = nseg < 1.5 ? 0.0 : nseg < 2.5 ? 3.0 : 4.0;
    float va = kind < 0.5 ? orient : kind;
    float vb = kindB < 0.5 ? orient : kindB;
    float f = kind == kindB ? 0.0 : smoothstep(0.45, 1.0, fract(ph));
    if (vsel > 0.5) {
        va = vsel < 3.5 ? vsel - 1.0 : vsel - 1.0;
        vb = va; f = 0.0;
    }
    float la = float(lookSel) > 0.5 ? float(lookSel) - 1.0 : mod(floor(ph * 0.75 + 0.3), 6.0);
    float lb = float(lookSel) > 0.5 ? la : mod(floor(ph * 0.75 + 0.3) + 1.0, 6.0);
    float lf = float(lookSel) > 0.5 ? 0.0 : smoothstep(0.45, 1.0, fract(ph * 0.75 + 0.3));

    float s = clamp(sliceManual + 0.75 * sin(TIME * 0.35 * sweep) * step(0.001, sweep), -1.0, 1.0);

    vec4 A = renderView(va, la, fc, R, s);
    if (lf > 0.001) A.rgb = mix(A.rgb, renderView(va, lb, fc, R, s).rgb, lf);
    vec4 C = A;
    if (f > 0.001) {
        vec4 B = renderView(vb, la, fc, R, s);
        // soft dissolve: a smooth crossfade with a gentle noise breakup, like one scan bleeding into the next
        float n = vn3(vec3(fc / R.y * 6.0, TIME * 0.3));
        float m = smoothstep(0.0, 1.0, clamp(f * 1.3 - 0.15 + (n - 0.5) * 0.3, 0.0, 1.0));
        C = mix(A, B, m);
    }

    vec3 col = backdrop(uv) * (1.0 - C.a) + C.rgb;
    // scan bar races during the build (phase ~ gBuild^2 so the rate climbs without jumps)
    float bar = exp(-pow((uv.y - fract(TIME * 0.15 + 2.5 * gBuild * gBuild)) * 60.0, 2.0)) * scanline;
    col += vec3(0.5, 0.8, 1.0) * bar * (0.35 + 0.5 * gBuild) * C.a;
    float flick = 1.0 + gBuild * 0.25 * step(0.5, fract(30.0 * gBuild * gBuild));
    col *= flick * (0.7 + 0.4 * gLift);
    col *= 1.0 + 0.35 * audioBeatPulse * ar * C.a;
    col = mix(col, vec3(0.9, 0.97, 1.0), gDrop * gDrop * 0.5 * (0.08 + 0.92 * C.a));   // white scan flash
    vec2 cp = fract(fc / 3.0);
    float mask = 0.75 + 0.25 * smoothstep(0.2, 0.5, min(cp.x, cp.y));
    col *= mix(1.0, mask, crt);
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.07;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? C.a : 1.0);
}
