/*{
  "DESCRIPTION": "Flower of Life — the sacred-geometry pattern of overlapping circles, pulsing and morphing between flat 2D line art and 3D. It starts as crisp glowing circle outlines on black, exactly like the drawn pattern, with light travelling around the rings and the circles breathing in and out. Then it lifts into 3D: the camera tilts and orbits while every circle becomes a real glass-and-light ring in space, in one of four ways — Petal Bloom, where the rings open outward like the petals of a flower; Ripple, where rings rise and fall on waves spreading from the centre; Dome, where the whole pattern wraps onto a sphere; and Globes, where each circle grows two crossing rings into a wire sphere, like the Fruit of Life. Then it settles back into flat 2D and the next 3D form follows. Show the infinite grid or the classic 19-circle flower with its double outer ring. At rest it stays flat and slowly breathing; during a build it lifts into 3D and the light races around the rings; on the drop every circle pulses outward in a bright shockwave, then the peak orbits fully in 3D.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-22 — original.",
  "CATEGORIES": ["Generator", "3D", "Geometric", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "extentSel",   "LABEL": "Pattern", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1], "LABELS": ["Infinite Grid", "Classic Flower (19 circles)"], "GROUP": "Geometry" },
    { "NAME": "formSel",     "LABEL": "3D Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Petal Bloom", "Ripple", "Dome", "Globes"], "GROUP": "Geometry" },
    { "NAME": "depthSel",    "LABEL": "2D ↔ 3D", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2], "LABELS": ["Auto (flat ↔ 3D)", "Always Flat 2D", "Always 3D"], "GROUP": "Geometry" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 4.0, "MAX": 60.0, "DEFAULT": 14.0, "GROUP": "Geometry" },
    { "NAME": "cellSize",    "LABEL": "Circle Size",      "TYPE": "float", "AUDIO": false, "MIN": 0.2, "MAX": 0.8, "DEFAULT": 0.5, "GROUP": "Geometry" },
    { "NAME": "overlapMorph","LABEL": "Overlap Breathing","TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Geometry" },
    { "NAME": "lineWidth",   "LABEL": "Line Width",       "TYPE": "float", "MIN": 0.3, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Geometry" },
    { "NAME": "pulse",       "LABEL": "Pulse",            "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "orbit",       "LABEL": "3D Orbit",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "travel",      "LABEL": "Travelling Light", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Motion" },
    { "NAME": "colourSel",   "LABEL": "Colour", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3], "LABELS": ["White Light", "Gold", "Rainbow", "Custom"], "GROUP": "Look" },
    { "NAME": "lineColor",   "LABEL": "Custom Colour",    "TYPE": "color", "DEFAULT": [0.6, 0.85, 1.0, 1.0], "GROUP": "Look" },
    { "NAME": "glow",        "LABEL": "Glow",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",            "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "GROUP": "Finish" },
    { "NAME": "paletteMode", "LABEL": "Studio Palette", "TYPE": "long", "DEFAULT": 1, "VALUES": [0, 1, 2], "LABELS": ["Original Colours", "Red · Black · White", "Blue · Black · White"], "GROUP": "Studio Palette" },
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

float gT, gM3, gForm, gR, gTube, gOverlap;
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
        z = m * 0.15 * dc;
    } else if (gForm < 1.5) {                            // Ripple: waves of height
        float w = gT * 1.6 - dc * 4.5;
        z = m * 0.28 * sin(w);
        vec2 grad = (dc > 1e-3 ? c / dc : vec2(0.0)) * (-m * 0.28 * 4.5 * cos(w));
        n = normalize(vec3(-grad * 0.6, 1.0));
    } else if (gForm < 2.5) {                            // Dome: wrapped onto a sphere
        float D = 2.2;
        float zd = sqrt(max(D * D - dc * dc, 0.0)) - D;
        z = m * zd;
        n = normalize(mix(vec3(0.0, 0.0, 1.0), normalize(vec3(c, zd + D)), m));
    }
}

float ringSD(vec3 l, vec3 n, float R) {
    vec3 u = normalize(cross(n, abs(n.x) < 0.9 ? vec3(1.0, 0.0, 0.0) : vec3(0.0, 1.0, 0.0)));
    vec3 v = cross(n, u);
    vec2 pl = vec2(dot(l, u), dot(l, v));
    return length(vec2(length(pl) - R, dot(l, n))) - gTube;
}

float map(vec3 p) {
    float r = gR;
    // nearest hex lattice point (spacing r, the classic flower of life layout)
    vec2 f = vec2(p.x / r - p.y / (r * S3), p.y * 2.0 / (r * S3));
    vec3 cube = vec3(f.x, f.y, -f.x - f.y);
    vec3 rc = floor(cube + 0.5);
    vec3 df = abs(rc - cube);
    if (df.x > df.y && df.x > df.z) rc.x = -rc.y - rc.z;
    else if (df.y > df.z) rc.y = -rc.x - rc.z;
    // safe bound so rays far from the (finite, 3D) disc still step toward it
    float limB = mix(40.0, 1.75, gM3) + r * 1.3;
    float d = max(max(length(p.xy) - limB, abs(p.z) - 3.0), 0.0) + 0.04;
    bool classic = float(extentSel) > 0.5;
    for (int j = -2; j <= 2; j++)
    for (int i = -2; i <= 2; i++) {
        if (abs(float(i + j)) > 2.0) continue;
        vec2 ax = rc.xy + vec2(float(i), float(j));
        if (classic && max(max(abs(ax.x), abs(ax.y)), abs(ax.x + ax.y)) > 2.0) continue;
        vec2 c = vec2(ax.x * r + ax.y * r * 0.5, ax.y * r * S3 * 0.5);
        float dc = length(c);
        // in 3D the infinite grid gathers into a finite disc of rings
        float lim = mix(40.0, 1.75, gM3);
        float keep = 1.0 - smoothstep(lim - 0.4, lim + 0.2, dc);
        if (keep < 0.02) continue;
        float R = keep * gOverlap * r * (1.0 + 0.035 * pulse * sin(gT * 2.0 - dc * 3.0) + 0.05 * bassP() * audioReact * pulse
                    + 0.18 * gDrop * exp(-pow(dc - (1.0 - gDrop) * 3.0, 2.0) * 2.0));
        float z; vec3 n;
        ringFrame(c, z, n);
        vec3 l = p - vec3(c, z);
        float dr = ringSD(l, n, R);
        if (gForm > 2.5 && gM3 > 0.02) {                 // Globes: two crossing rings grow into a wire sphere
            float g = gM3;
            vec3 n2 = normalize(vec3(cos(gT * 0.25), sin(gT * 0.25), 0.0));
            dr = min(dr, ringSD(l, normalize(mix(vec3(0.0, 0.0, 1.0), n2, g)), R * g));
            vec3 n3 = vec3(-n2.y, n2.x, 0.0);
            dr = min(dr, ringSD(l, normalize(mix(vec3(0.0, 0.0, 1.0), n3, g)), R * g * 0.999));
        }
        if (dr < d) { d = dr; gAng = atan(l.y, l.x); gCellD = dc; }
    }
    if (classic) {                                       // double outer ring of the classic flower
        float ro = r * 3.0;
        vec3 l = p - vec3(0.0, 0.0, gForm > 1.5 && gForm < 2.5 ? -gM3 * 0.8 : 0.0);
        float o1 = length(vec2(length(l.xy) - ro, l.z)) - gTube;
        float o2 = length(vec2(length(l.xy) - ro * 1.04, l.z)) - gTube;
        float dd = min(o1, o2);
        if (dd < d) { d = dd; gAng = atan(l.y, l.x); gCellD = 3.0 * r; }
    }
    return d;
}

vec3 lineCol(float ang, float dc) {
    float cs = float(colourSel);
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
    gT = TIME * 0.6 + (audioMidTime * 0.4 + audioBassTime * 0.3) * ar + 1.5 * gBuild * gBuild;
    gR = cellSize;
    // circles breathe between tight hex packing (radius ≈ half spacing) and the full flower overlap
    float ov = 0.5 + 0.5 * sin(TIME * 0.35 + audioBassTime * 0.2 * audioReact);
    gOverlap = mix(1.0, mix(0.56, 1.0, ov), overlapMorph);

    // 2D ↔ 3D: flat, rise, hold in 3D, settle back to flat
    float ph = TIME / morphPeriod;
    float fp = fract(ph);
    float auto3 = smoothstep(0.12, 0.42, fp) * (1.0 - smoothstep(0.7, 0.95, fp));
    float ds = float(depthSel);
    float m3 = ds < 0.5 ? auto3 : ds < 1.5 ? 0.0 : 1.0;
    m3 = clamp(m3 + 0.6 * gBuild * (ds < 1.5 && ds > 0.5 ? 0.0 : 1.0) - 0.5 * gRest * (ds > 1.5 ? 0.0 : 1.0), 0.0, 1.0);
    gM3 = m3;
    float fsel = float(formSel);
    gForm = fsel > 0.5 ? fsel - 1.0 : mod(floor(ph), 4.0);      // the form switches while flat

    // camera: straight-on in 2D, tilting and orbiting in 3D
    float pxW = 1.0 / R.y;
    float camD = 10.0;
    float elev = mix(0.0, 0.9, m3);
    float az = (TIME * 0.12 + audioBassTime * 0.05 * ar) * orbit;
    vec3 ro = vec3(0.0, 0.0, camD);
    ro.yz = rot(elev) * ro.yz;
    ro.xy = rot(az * m3) * ro.xy;
    vec3 ta = vec3(0.0, 0.0, mix(0.0, gForm > 1.5 && gForm < 2.5 ? -0.6 : 0.0, m3));
    vec3 ww = normalize(ta - ro);
    vec3 upv = vec3(rot(az * m3) * vec2(0.0, 1.0), 0.0);
    vec3 uu = normalize(cross(ww, upv));
    vec3 vv = cross(uu, ww);
    float focal = 3.0 + 0.4 * gBuild - 0.15 * gRest;
    vec3 rd = normalize(sp.x * uu + sp.y * vv + focal * ww);
    // tube radius ≈ 1.3 px at the 2D framing
    gTube = lineWidth * 1.3 * pxW * camD / focal;

    vec3 col = vec3(0.0);
    float alpha = 0.0;
    float glowAcc = 0.0;
    float t = 1.5;
    float hitT = -1.0;
    float ang = 0.0, cd = 0.0;
    for (int i = 0; i < 130; i++) {
        vec3 p = ro + rd * t;
        float d = map(p);
        glowAcc += exp(-max(d, 0.0) * (220.0 / max(lineWidth, 0.3))) * 0.06;
        if (d < 0.0004) { hitT = t; ang = gAng; cd = gCellD; break; }
        t += max(d * 0.9, 0.0008 + 0.02 * step(0.3, d));
        if (t > camD + 4.0) break;
    }
    vec3 bg = backdrop(uv);
    if (hitT > 0.0) {
        vec3 p = ro + rd * hitT;
        vec2 e = vec2(0.0008, 0.0);
        vec3 n = normalize(vec3(map(p + e.xyy) - map(p - e.xyy), map(p + e.yxy) - map(p - e.yxy), map(p + e.yyx) - map(p - e.yyx)));
        map(p);
        ang = gAng; cd = gCellD;
        float dif = 0.55 + 0.45 * clamp(dot(n, normalize(vec3(0.3, 0.5, 0.8))), 0.0, 1.0);
        float spec = pow(clamp(dot(reflect(rd, n), normalize(vec3(-0.3, 0.6, 0.7))), 0.0, 1.0), 30.0);
        // travelling light: bright pulses running around every ring
        float run = pow(0.5 + 0.5 * sin(ang * 2.0 - gT * 3.0 * travel - cd * 2.5), 8.0) * travel;
        vec3 lc = lineCol(ang, cd);
        float shade = mix(1.0, dif, m3);
        col = lc * shade * (0.75 + 0.35 * gLift) + lc * run * (0.6 + 0.8 * gLift) + spec * m3 * 0.8;
        // depth fade in 3D
        col *= mix(1.0, exp(-max(hitT - camD + 1.5, 0.0) * 0.35), m3);
        alpha = 1.0;
    }
    vec3 gc = lineCol(ang, cd);
    col += gc * glowAcc * glow * (0.35 + 0.6 * gLift + 1.2 * gDrop);
    // classic vignette for the infinite grid, like the reference's fade at the corners
    float vig = 1.0 - smoothstep(0.75, 1.25, length(sp * vec2(0.85, 1.0)));
    col *= mix(1.0, vig, 0.6);
    col = bg * (1.0 - alpha) + col;
    col += vec3(1.0) * gDrop * gDrop * 0.1;
    col *= 1.0 + 0.12 * audioBeatPulse * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? clamp(alpha + lum(col), 0.0, 1.0) : 1.0);
}
