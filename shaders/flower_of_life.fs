/*{
  "DESCRIPTION": "Flower of Life — the sacred-geometry pattern of overlapping circles as an endless field of crisp glowing line art that always fills the whole screen, pulsing, spinning and morphing between flat 2D and 3D. It starts as fine luminous circle outlines on black, exactly like the drawn pattern, with light running around every ring and the circles breathing between tight packing and full overlap while the whole field slowly turns. Then it lifts into 3D: the camera tilts and orbits over an infinite plain of glass-and-light rings, in one of four ways — Petal Bloom, where the rings open outward like petals; Ripple, where rings rise and fall on waves spreading from the centre; Dome, where the middle of the field swells up into a great dome; and Globes, where each circle grows two crossing rings into a wire sphere, like the Fruit of Life — then settles back to flat and the next form follows. Choose the infinite grid or a tiling of classic 19-circle flowers, each with its double outer ring. The motion has tempo: it rushes and slows, drops into half-time stretches and surges into double-time bursts, and bass and energy push every clock faster. At rest it stays flat, slow and calmly breathing; during a build it lifts into 3D and accelerates, the light racing around the rings; on the drop every circle pulses outward in a bright shockwave as the view whips round, and the peak orbits fully in 3D at full speed with sharp tempo changes.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "3D", "Geometric", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "extentSel",   "LABEL": "Pattern", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["Infinite Grid", "Classic Flower Tiles (19 circles)"], "GROUP": "Geometry" },
    { "NAME": "formSel",     "LABEL": "3D Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Petal Bloom", "Ripple", "Dome", "Globes"], "GROUP": "Geometry" },
    { "NAME": "depthSel",    "LABEL": "2D ↔ 3D", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Auto (flat ↔ 3D)", "Always Flat 2D", "Always 3D"], "GROUP": "Geometry" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 3.0, "MAX": 60.0, "DEFAULT": 14.0, "GROUP": "Geometry" },
    { "NAME": "cellSize",    "LABEL": "Circle Size",      "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 0.8, "DEFAULT": 0.5, "GROUP": "Geometry" },
    { "NAME": "overlapMorph","LABEL": "Overlap Breathing","TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Geometry" },
    { "NAME": "lineWidth",   "LABEL": "Line Width",       "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Geometry" },
    { "NAME": "speed",       "LABEL": "Master Speed",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "tempoVar",    "LABEL": "Tempo Changes",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "pulse",       "LABEL": "Pulse",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "pulseSpeed",  "LABEL": "Pulse Speed",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "orbit",       "LABEL": "Rotate / Orbit Speed", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "morphSpeed",  "LABEL": "Form Motion Speed", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "travel",      "LABEL": "Travelling Light", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "colourSel",   "LABEL": "Colour", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2, 3], "LABELS": ["White Light", "Gold", "Rainbow", "Custom"], "GROUP": "Look" },
    { "NAME": "lineColor",   "LABEL": "Custom Colour",    "TYPE": "color", "DEFAULT": [0.6, 0.85, 1.0, 1.0], "GROUP": "Look" },
    { "NAME": "glow",        "LABEL": "Glow",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2, 3], "LABELS": ["Colorful", "Red · Black · White", "Blue · Black · White", "Black & White"], "GROUP": "Studio Palette" },
    { "NAME": "palAmount",   "LABEL": "Palette Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Studio Palette" },
    { "NAME": "palRed",      "LABEL": "Red Accent",       "TYPE": "color", "DEFAULT": [0.92, 0.04, 0.07, 1.0], "GROUP": "Studio Palette" },
    { "NAME": "palBlue",     "LABEL": "Blue Accent",      "TYPE": "color", "DEFAULT": [0.07, 0.32, 1.0, 1.0],  "GROUP": "Studio Palette" },
    { "NAME": "audioReact",  "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Audio Reactivity" },
    { "NAME": "showState",   "LABEL": "Show State", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["Follow Music", "Rest", "Build", "Peak"], "GROUP": "Show Dynamics" },
    { "NAME": "dynamics",    "LABEL": "Build & Drop Depth", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85, "GROUP": "Show Dynamics" },
    { "NAME": "bgMode",      "LABEL": "Background", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Solid", "Vertical Gradient", "Radial Gradient"], "GROUP": "Background" },
    { "NAME": "bgColor",     "LABEL": "BG Color / Top / Center", "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0], "GROUP": "Background" },
    { "NAME": "bgColor2",    "LABEL": "BG Bottom / Edge", "TYPE": "color", "DEFAULT": [0.03, 0.03, 0.04, 1.0], "GROUP": "Background" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" }
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718
#define S3  1.7320508

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

// ── Tempo: bounded clock warps (additive only, never TIME * changing value) ──
// derivative = K/P * (smoothstep'(u) - 1): a rush inside [a,b] of each period, a slower stretch elsewhere;
// negative K gives a half-time stretch inside the window instead.
float rush(float t, float P, float a, float b, float K) { float u = fract(t / P); return K * (smoothstep(a, b, u) - u); }
float tempoW(float t) {
    return rush(t, 9.0, 0.55, 0.72, 3.2)            // a short double-time surge every ~9 s
         + rush(t + 3.1, 14.3, 0.15, 0.27, 2.9)     // a second, offset surge
         + rush(t + 7.7, 23.0, 0.40, 0.62, -1.5);   // an occasional half-time stretch
}

float gT, gPT, gM3, gForm, gR, gTubeK, gOverlap, gDropR;
vec3 gRo;
float gAng;      // angle around the nearest ring (for travelling light)
float gCellD;    // distance of that ring's centre from the middle

// ring placement for lattice cell centre c (in the plane), returns z and normal
void ringFrame(vec2 c, out float z, out vec3 n) {
    float dc = length(c);
    float m = gM3;
    z = 0.0; n = vec3(0.0, 0.0, 1.0);
    if (gForm < 0.5) {                                   // Petal Bloom: rings open outward
        vec2 rd = dc > 1e-3 ? c / dc : vec2(1.0, 0.0);
        float th = m * (0.35 + 0.55 * (0.5 + 0.5 * sin(gT * 1.2 - dc * 3.0)));
        n = normalize(vec3(rd * sin(th), cos(th)));
        z = m * 0.15 * min(dc, 8.0);
    } else if (gForm < 1.5) {                            // Ripple: waves of height
        float w = gT * 1.6 - dc * 4.5;
        z = m * 0.28 * sin(w);
        vec2 grad = (dc > 1e-3 ? c / dc : vec2(0.0)) * (m * 0.28 * 4.5 * cos(w));
        n = normalize(vec3(grad * 0.6, 1.0));
    } else if (gForm < 2.5) {                            // Dome: the middle swells up, the field runs on flat forever
        float A = 1.5 + 0.15 * sin(gT * 0.7);
        float e = exp(-dc * dc / 6.0);
        z = m * A * e;
        vec2 grad = c * (-2.0 / 6.0) * A * e * m;
        n = normalize(vec3(-grad, 1.0));
    }
}

// distance to a ring's centre line (lines get their width in screen space)
float ringSD(vec3 l, vec3 n, float R) {
    float h = dot(l, n);
    return length(vec2(length(l - n * h) - R, h));
}

// nearest hex lattice point (axial coords) for spacing s
vec2 hexRound(vec2 p, float s) {
    vec2 f = vec2(p.x / s - p.y / (s * S3), p.y * 2.0 / (s * S3));
    vec3 cube = vec3(f.x, f.y, -f.x - f.y);
    vec3 rc = floor(cube + 0.5);
    vec3 df = abs(rc - cube);
    if (df.x > df.y && df.x > df.z) rc.x = -rc.y - rc.z;
    else if (df.y > df.z) rc.y = -rc.x - rc.z;
    return rc.xy;
}
vec2 axPos(vec2 ax, float s) { return vec2(ax.x * s + ax.y * s * 0.5, ax.y * s * S3 * 0.5); }

float map(vec3 p) {
    float r = gR;
    bool classic = float(extentSel) > 0.5;
    vec2 rc = hexRound(p.xy, r);
    // never step more than the gap to rings outside the search window (they are ≥ ~0.4 r away sideways)
    float zs; vec3 ns;
    ringFrame(p.xy, zs, ns);
    float d = max(0.4 * r, 0.7 * (abs(p.z - zs) - 1.3 * r - 0.1));
    for (int j = -2; j <= 2; j++)
    for (int i = -2; i <= 2; i++) {
        if (abs(float(i + j)) > 2.0) continue;
        vec2 ax = rc + vec2(float(i), float(j));
        vec2 c = axPos(ax, r);
        float sz = gOverlap;
        if (classic) {
            // classic mode: an endless tiling of 19-circle flowers on a super-lattice 6 cells apart;
            // the one cell left in each three-way gap gets a small circle so no background patch opens up
            vec2 lx = ax - 6.0 * hexRound(c, 6.0 * r);
            float hd = max(max(abs(lx.x), abs(lx.y)), abs(lx.x + lx.y));
            if (hd > 3.5) sz *= 0.45;
            else if (hd > 2.5) continue;
        }
        float dc = length(c);
        float R = sz * r * (1.0 + 0.035 * pulse * sin(gPT * 2.0 - dc * 3.0) + 0.05 * bassP() * audioReact * pulse
                    + 0.18 * gDrop * exp(-pow(dc - (1.0 - gDropR) * 7.0, 2.0) * 1.5));
        float z; vec3 n;
        ringFrame(c, z, n);
        vec3 l = p - vec3(c, z);
        float dr = ringSD(l, n, R);
        if (gForm > 2.5 && gM3 > 0.02) {                 // Globes: two crossing rings grow into a wire sphere
            float g = gM3;
            float ga = gT * 0.25 + dc * 0.9 + 1.7 * h12(ax);
            vec3 n2 = vec3(cos(ga), sin(ga), 0.0);
            dr = min(dr, ringSD(l, normalize(mix(vec3(0.0, 0.0, 1.0), n2, g)), R * g * 0.92));
            vec3 n3 = vec3(-n2.y, n2.x, 0.0);
            dr = min(dr, ringSD(l, normalize(mix(vec3(0.0, 0.0, 1.0), n3, g)), R * g * 0.9));
        }
        if (dr < d) { d = dr; gAng = atan(l.y, l.x); gCellD = dc; }
    }
    if (classic) {                                       // double outer ring of each classic flower, lying on the surface
        vec2 tc = axPos(hexRound(p.xy, 6.0 * r), 6.0 * r);
        vec2 lxy = p.xy - tc;
        float lz = p.z - zs;
        float ro = r * 3.0;
        float dd = min(length(vec2(length(lxy) - ro, lz)), length(vec2(length(lxy) - ro * 1.04, lz)));
        if (dd < d) { d = dd; gAng = atan(lxy.y, lxy.x); gCellD = length(tc) + 3.0 * r; }
    }
    return d;
}

vec3 lineCol(float ang, float dc) {
    float cs = float(colourSel);
    // Colorful studio palette: white / gold lines become saturated neon, a hue per ring
    if (float(paletteMode) < 0.5 && cs < 1.5)
        return clamp(abs(fract(ang / TAU + dc * 0.18 + gT * 0.04 + vec3(0.0, 0.6667, 0.3333)) * 6.0 - 3.0) - 1.0, 0.0, 1.0) * 0.9 + 0.1;
    if (cs < 0.5) return vec3(0.95, 0.97, 1.0);
    if (cs < 1.5) return vec3(1.0, 0.78, 0.4);
    if (cs < 2.5) return 0.55 + 0.45 * cos(TAU * (ang / TAU + dc * 0.3 + gT * 0.05 + vec3(0.0, 0.33, 0.67)));
    return lineColor.rgb;
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;

    // ── clocks: master speed × per-motion speed, with rushes / half-time stretches, audio pushes them on ──
    float sm = float(showState);
    float stR = sm > 2.5 ? 1.6 : sm > 1.5 ? 1.15 : sm > 0.5 ? 0.6 : 1.0;   // pinned Rest is slow, pinned Peak fast
    float base = speed * stR;
    float amp = clamp(0.25 + 0.75 * gPeak + 0.6 * gBuild - 0.25 * gRest, 0.04, 1.0) * tempoVar;
    float dR = dynamics > 1e-3 ? gDrop / dynamics : 0.0;           // raw drop envelope, 1 → 0
    gDropR = dR;
    float whip = 4.0 * dR * (1.0 - dR);                             // 0 at the hit, fast out, eased back
    float bassT = audioBassTime * ar, midT = audioMidTime * ar;
    gT  = morphSpeed * base * 0.5 * (TIME + amp * tempoW(TIME)) + (midT * 0.4 + bassT * 0.35) * morphSpeed
        + 2.0 * gBuild * gBuild + 1.2 * whip;
    gPT = pulseSpeed * base * 0.9 * (TIME + amp * tempoW(TIME + 5.3)) + (bassT * 0.6 + midT * 0.2) * pulseSpeed
        + 2.5 * gBuild * gBuild;
    float az = orbit * base * 0.09 * (TIME + 1.6 * amp * tempoW(TIME + 11.0)) + bassT * 0.05 * orbit
        + 0.5 * gBuild * gBuild + 1.1 * whip * orbit;
    float mT = (TIME + 0.8 * amp * tempoW(TIME + 17.0)) * (0.5 + 0.5 * speed);

    gR = cellSize;
    // circles breathe between tight hex packing (radius ≈ half spacing) and the full flower overlap
    float ov = 0.5 + 0.5 * sin(gPT * 0.4 + 0.3 * gBuild * gBuild);
    gOverlap = mix(1.0, mix(0.56, 1.0, ov), overlapMorph);

    // 2D ↔ 3D: flat, rise, hold in 3D, settle back to flat
    float ph = mT / morphPeriod;
    float fp = fract(ph);
    float auto3 = smoothstep(0.12, 0.42, fp) * (1.0 - smoothstep(0.7, 0.95, fp));
    float ds = float(depthSel);
    float m3 = ds < 0.5 ? auto3 : ds < 1.5 ? 0.0 : 1.0;
    m3 = clamp(m3 + 0.6 * gBuild * (ds < 1.5 && ds > 0.5 ? 0.0 : 1.0) - 0.5 * gRest * (ds > 1.5 ? 0.0 : 1.0), 0.0, 1.0);
    gM3 = m3;
    float fsel = float(formSel);
    gForm = fsel > 0.5 ? fsel - 1.0 : mod(floor(ph), 4.0);      // the form switches while flat
    if (gForm > 2.5) gOverlap = mix(gOverlap, 0.58, m3 * 0.85);  // globes pack loosely so each sphere reads

    // build tremor
    sp += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.004 * smoothstep(0.7, 1.0, gBuild);

    // camera: straight-on (slowly spinning) in 2D, tilting and orbiting in 3D.
    // Tilt stays ≤ 0.8 rad so even the frame corners hit the endless plane well before any horizon.
    float camD = 10.0;
    float elev = 0.8 * m3;
    vec3 ro = vec3(0.0, 0.0, camD);
    ro.yz = rot(elev) * ro.yz;
    ro.xy = rot(az) * ro.xy;
    vec3 ta = vec3(0.0, 0.0, gForm > 1.5 && gForm < 2.5 ? 0.6 * m3 : 0.0);
    vec3 ww = normalize(ta - ro);
    vec3 upv = vec3(rot(az) * vec2(0.0, 1.0), 0.0);
    vec3 uu = normalize(cross(ww, upv));
    vec3 vv = cross(uu, ww);
    float focal = 3.0 + 0.4 * gBuild - 0.15 * gRest + 0.1 * sin(gT * 0.3);
    vec3 rd = normalize(sp.x * uu + sp.y * vv + focal * ww);
    gRo = ro;
    float pxK = 1.0 / (R.y * focal);              // world size of one pixel per unit of ray length
    float hw = 0.65 * lineWidth;                  // line half-width in pixels
    float zBot = -(0.25 + gM3 * 0.9 + gR);

    // ── cone-traced line march: every near pass of a ring is an anti-aliased layer, front to back ──
    float T = 1.0;
    vec3 acc = vec3(0.0);
    float pc = 0.0, pA = 0.0, pD = 0.0, pT = 0.0;
    float glowAcc = 0.0;
    float t = 0.5;
    for (int i = 0; i < 180; i++) {
        vec3 p = ro + rd * t;
        float d = map(p);
        float wpx = t * pxK;
        float q = d / wpx;
        float c = 1.0 - smoothstep(hw - 0.75, hw + 0.75, q);
        if (c > pc) { pc = c; pA = gAng; pD = gCellD; pT = t; }
        glowAcc += T * exp(-q / 3.5) * 0.045 * min(d * 0.9 / wpx, 2.0);
        if (pc > 0.001 && q > hw + 1.2) {        // left this ring: composite the pass
            float run = pow(0.5 + 0.5 * sin(pA * 2.0 - gT * 3.0 * travel - pD * 2.5), 8.0) * travel;
            vec3 lc = lineCol(pA, pD);
            float glint = pow(0.5 + 0.5 * cos(pA - az * 2.0 - 0.8), 18.0);
            vec3 lcol = lc * (0.75 + 0.35 * gLift) + lc * run * (0.6 + 0.8 * gLift) + glint * m3 * 0.7;
            lcol *= mix(1.0, 0.5 + 0.5 * exp(-max(pT - camD + 1.0, 0.0) * 0.15), m3);
            acc += T * pc * lcol;
            T *= 1.0 - pc;
            pc = 0.0;
            if (T < 0.02) break;
        }
        t += max(d * 0.9, wpx * 0.3);
        if (t > 40.0 || (p.z < zBot && rd.z < 0.0)) break;
    }
    if (pc > 0.001) {
        vec3 lc = lineCol(pA, pD);
        acc += T * pc * lc * (0.75 + 0.35 * gLift);
        T *= 1.0 - pc;
    }
    vec3 bg = backdrop(uv);
    vec3 gc = lineCol(pA, pD);
    vec3 col = acc + gc * (1.0 - exp(-glowAcc * glow * (0.35 + 0.6 * gLift + 1.2 * gDrop) * 1.6)) * 0.55;
    float alpha = clamp(1.0 - T + lum(col) * 0.5, 0.0, 1.0);
    col = bg * T + col;

    col += vec3(1.0) * gDrop * gDrop * 0.1;
    col *= 1.0 + 0.12 * audioBeatPulse * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
