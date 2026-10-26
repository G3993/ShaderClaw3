/*{
  "DESCRIPTION": "Bloodstream — a flight through a living blood vessel. In Artery you glide down a winding crimson tunnel whose glossy, wet walls are tiled with the faint cobblestone of endothelial cells and wrinkle as they pass; thousands of red blood cells stream toward you and around you, true biconcave discs that tumble as they fly, each with its sunken pale centre, a glistening rim highlight and a translucent red glow where it is thin, while occasional white blood cells roll past as big lumpy pearl-white spheres and tiny amber platelets glint in the flow. In Capillary the vessel narrows to a tight tube and the cells come single file, squeezed into cupped parachute shapes that barely fit. Microscope Smear drops you onto a glass slide under a brightfield lens: a circular field of pink discs with pale centres overlapping like coins, lilac white cells with dark purple lobed nuclei and specks of platelets, drifting and breathing in and out of focus. Thermal shows the artery through a false-colour heat camera, cells glowing yellow-hot against cooler purple walls. Auto Morph travels Artery → Capillary → Thermal → Smear. The heartbeat is the music: bass and beats drive a lub-dub surge that pushes the flow forward, dilates the vessel and sends a pressure wave rolling down the walls (with a gentle resting heartbeat in silence); mids speed the stream; highs make the wet highlights sparkle. At rest the vessel is wide and calm with sparse cells drifting in a slow pulse; during a build it constricts, the stream rushes faster, the walls shimmer and the view trembles; on the drop one massive pulse floods the tunnel with light, blows the vessel open and scatters the cells, and the peak runs at full density with rolling camera and hot colour. Real physics drives the stream: every cell is a simulated rigid body carried by pulsatile Poiseuille flow (fastest in the centre), so each heartbeat and kick is a pressure surge that accelerates the plasma and the cells lag, jostle, collide and bounce off the compliant, springy vessel wall, tumbling in Jeffery orbits spun by the shear and by collisions, while lift keeps a cell-free layer at the wall and heavy white cells marginate and roll along it; a drop is one huge pressure wave that rushes everything through and balloons the wall, which then rings and relaxes back.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-26 — original.",
  "CATEGORIES": ["Generator", "3D", "Science", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "formSel",     "LABEL": "Form", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Auto Morph", "Artery", "Capillary", "Microscope Smear", "Thermal"], "GROUP": "Form" },
    { "NAME": "morphPeriod", "LABEL": "Morph Period (s)", "TYPE": "float", "AUDIO": false, "MIN": 6.0, "MAX": 60.0, "DEFAULT": 16.0, "GROUP": "Form" },
    { "NAME": "flowSpeed",   "LABEL": "Flow Speed",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Flow" },
    { "NAME": "cellDensity", "LABEL": "Cell Density",    "TYPE": "float", "MIN": 0.2, "MAX": 1.0, "DEFAULT": 0.8, "GROUP": "Flow" },
    { "NAME": "wbcAmt",      "LABEL": "White Cells",     "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Flow" },
    { "NAME": "tumble",      "LABEL": "Tumble",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Flow" },
    { "NAME": "heartRate",   "LABEL": "Resting Heart (BPM)", "TYPE": "float", "AUDIO": false, "MIN": 40.0, "MAX": 140.0, "DEFAULT": 66.0, "GROUP": "Heartbeat" },
    { "NAME": "pulseAmt",    "LABEL": "Pulse Strength",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Heartbeat" },
    { "NAME": "wetness",     "LABEL": "Wet Highlights",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "camSway",     "LABEL": "Camera Sway",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Look" },
    { "NAME": "grain",       "LABEL": "Grain",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.1, "GROUP": "Finish" },
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
    { "NAME": "transparentBg","LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Background" },
    { "NAME": "physicsAmt",  "LABEL": "Physics Amount",   "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "gravity",     "LABEL": "Sedimentation (Gravity)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.25, "GROUP": "Physics" },
    { "NAME": "bounce",      "LABEL": "Bounce",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Physics" },
    { "NAME": "kickForce",   "LABEL": "Kick Force",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Physics" },
    { "NAME": "resetPhysics","LABEL": "Reset Physics",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Physics" }
  ],
  "PASSES": [
    { "TARGET": "bodyBuf", "PERSISTENT": true, "FLOAT": true },
    {}
  ]
}*/

#define PI  3.14159265359
#define TAU 6.28318530718
#define NB 40          // simulated bodies (cells)
#define NBF 40.0
#define BL 7.2         // length of the simulated stream ahead of the camera

float h11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float h12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2 h22(vec2 p)   { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
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

// ── Heartbeat: lub-dub envelope ─────────────────────────────────────────
float env(float x) { return x > 0.0 ? (1.0 - exp(-x * 70.0)) * exp(-x * 10.0) : 0.0; }
float lubdub(float beats) { float f = fract(beats); return env(f) + 0.62 * env(f - 0.3); }
float gHB;       // heartbeat clock (beats)
float gHAudio;   // audio part of the heart pulse
float gH;        // total pulse now
float gCap, gThermal, gSmear;
float gCamZ, gRbase, gDil, gDens, gU;
vec4 gG1, gG2;

// ── physics state buffer: NB columns × 4 rows of cells ──────────────────
//   row 0: (q.x, q.y, zHi, zLo)   q = cross-section position / vessel radius, z = depth ahead of camera
//   row 1: (vq.x, vq.y, vz, seed) vz = speed toward the camera (world units/s)
//   row 2: (theta, phi, wTheta, wPhi)  disc-normal orientation + angular velocity
//   row 3: col0 (U centreline flow, D dilation, Dv, marker) · col1 (camHi, camLo, blast, blastV) · col2 (smear shove xy, vel xy)
vec4 stTex(float i, float row) { return texture2D(bodyBuf, vec2((i + 0.5) / NBF, (row + 0.5) * 0.25)); }

// x = radius (world), y = isWhite, z = isPlatelet, w = visibility
vec4 bodyType(float seed) {
    float hE = h11(seed * 917.3 + 3.1);
    float hT = h11(seed * 613.7 + 9.7);
    float sz = clamp((gDens - hE) / 0.08, 0.0, 1.0);
    float isW = step(hT, 0.03 * (0.4 + 1.2 * wbcAmt) * (1.0 + gPeak));
    float isP = step(0.86, hT) * (1.0 - gCap);
    float R = isW > 0.5 ? 0.2 * gDil : (isP > 0.5 ? 0.04 : mix(0.105, 0.16, gCap) * gDil);
    return vec4(R, isW, isP, sz);
}

vec2 pathC(float z) {
    float A = mix(1.0, 0.3, gCap);
    return A * vec2(0.9 * sin(z * 0.21) + 0.35 * sin(z * 0.57 + 1.3), 0.6 * sin(z * 0.16 + 2.0) + 0.3 * sin(z * 0.43));
}
// vessel radius at z (with the pressure wave rolling away from the camera)
float vesR(float z) {
    float dz = z - gCamZ;
    float wave = lubdub(gHB - dz * 0.22) * (0.6 + 0.4 * (1.0 - gRest));
    float pr = (0.08 * wave + 0.06 * gHAudio) * pulseAmt;
    return gRbase * gDil * (1.0 + 0.07 * sin(z * 0.9 + 1.0) + 0.04 * sin(z * 2.3) + pr);
}

// ONE scene function for the wall — called from the march loop and one normal loop only
float mapWall(vec3 p) {
    vec2 q = p.xy - pathC(p.z);
    float r = length(q);
    float a = atan(q.y, q.x);
    float bump = 0.028 * sin(a * 9.0 + p.z * 1.7) * sin(p.z * 3.3 - a * 4.0) + 0.014 * sin(p.z * 7.1 + a * 13.0);
    return (vesR(p.z) * (1.0 + bump) - r) * 0.72;
}

// Evans–Fung biconcave thickness profile (units of cell radius), rho in [0,1]
float bicon(float r) {
    float r2 = r * r;
    return sqrt(max(1.0 - r2, 0.0)) * (0.207 + 2.003 * r2 - 1.123 * r2 * r2);
}

// lumpy white-cell surface
float wbcLump(vec3 l, float s) {
    return length(l) - (0.9 + 0.1 * sin(l.x * 5.0 + s) * sin(l.y * 4.5 - s * 0.7) * sin(l.z * 4.0 + s * 1.3)
                           + 0.035 * sin(l.y * 9.0 + l.z * 7.0 + s));
}

vec3 ironbow(float x) {
    x = clamp(x, 0.0, 1.0);
    vec3 c = mix(vec3(0.0, 0.0, 0.02), vec3(0.28, 0.0, 0.45), smoothstep(0.0, 0.25, x));
    c = mix(c, vec3(0.85, 0.08, 0.2), smoothstep(0.2, 0.5, x));
    c = mix(c, vec3(1.0, 0.55, 0.02), smoothstep(0.45, 0.75, x));
    c = mix(c, vec3(1.0, 1.0, 0.8), smoothstep(0.72, 1.0, x));
    return c;
}

// endothelial cobblestone on the wall: returns (border, nucleus)
vec2 endo(vec2 g) {
    vec2 id = floor(g), f = fract(g);
    float d1 = 9.0, d2 = 9.0; vec2 cc = vec2(0.0);
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 o = vec2(float(i), float(j));
        vec2 pp = o + h22(id + o) * 0.8 + 0.1 - f;
        float d = dot(pp, pp);
        if (d < d1) { d2 = d1; d1 = d; cc = pp; } else if (d < d2) d2 = d;
    }
    float border = 1.0 - smoothstep(0.0, 0.18, sqrt(d2) - sqrt(d1));
    float nuc = 1.0 - smoothstep(0.08, 0.2, length(cc * vec2(1.0, 1.6)));
    return vec2(border, nuc);
}

// ── 3D vessel ───────────────────────────────────────────────────────────
vec4 vesselImage(vec2 uv, vec2 sp, out float heat) {
    float ar = audioReact;
    // camera flies down the vessel
    vec3 ro = vec3(pathC(gCamZ), gCamZ);
    vec3 ta = vec3(pathC(gCamZ + 1.4), gCamZ + 1.4);
    float trem = smoothstep(0.7, 1.0, gBuild);
    ro.xy += (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.03 * trem * gRbase;
    ro.xy += vec2(sin(TIME * 0.31), cos(TIME * 0.23)) * 0.12 * camSway * gRbase * (1.0 - gCap * 0.7);
    vec3 ww = normalize(ta - ro);
    float roll = 0.18 * camSway * sin(TIME * 0.11) + 0.35 * gPeak * sin(TIME * 0.19 + audioBassTime * 0.05 * ar);
    vec3 upv = vec3(sin(roll), cos(roll), 0.0);
    vec3 uu = normalize(cross(ww, upv));
    vec3 vv = cross(uu, ww);
    float focal = 1.05 + 0.25 * gBuild;
    vec3 rd = normalize(sp.x * uu + sp.y * vv + focal * ww);

    // march the wall
    float t = 0.01, tw = 30.0;
    for (int i = 0; i < 80; i++) {
        float d = mapWall(ro + rd * t);
        if (d < 0.0006 * t + 0.0004) { tw = t; break; }
        t += d;
        if (t > 24.0) break;
    }
    if (tw > 29.0) tw = t;

    // ── cells: simulated rigid bodies read from the physics buffer, analytic per-body intersection ──
    float bestT = tw;
    vec3 cN = vec3(0.0);
    float cType = -1.0, cThick = 0.0, cRho = 0.0, cSeed = 0.0, cFade = 1.0;
    float cup = gCap;
    float vr = gRbase * gDil;
    for (int i = 0; i < NB; i++) {
        float fi = float(i);
        vec4 sA = stTex(fi, 0.0);
        vec4 sB = stTex(fi, 1.0);
        vec4 sC = stTex(fi, 2.0);
        float dzc = sA.z + sA.w / 128.0;
        float zc = gCamZ + dzc;
        float k = sB.w * 997.0;
        vec4 T = bodyType(sB.w);
        float hT = h11(sB.w * 613.7 + 9.7);
        float hA = h11(sB.w * 271.3 + 1.9);
        float isW = T.y, isP = T.z;
        float fade = smoothstep(-0.05, 0.25, dzc) * (1.0 - smoothstep(BL * 0.7, BL - 0.18, dzc));
        float R = T.x * T.w * fade;
        if (R < 0.004) continue;
        // simulated position in the cross-section (+ a render-only nudge that clears the lens)
        vec2 qw = sA.xy * vr;
        float ql = length(sA.xy);
        vec2 qd = ql > 1e-3 ? sA.xy / ql : vec2(cos(hA * TAU), sin(hA * TAU));
        float near = 1.0 - smoothstep(0.05, mix(0.7, 1.9, gCap), dzc);
        qw += qd * pow(near, mix(2.0, 1.2, gCap)) * vr * mix(0.55, 1.5, gCap);
        vec3 C = vec3(pathC(zc) + qw, zc);
        float Rb = R * (1.08 + 0.55 * cup);
        vec3 o = ro - C;
        float b = dot(o, rd);
        float c = dot(o, o) - Rb * Rb;
        float disc = b * b - c;
        if (disc < 0.0) continue;
        float sq = sqrt(disc);
        float tn = -b - sq, tf = -b + sq;
        if (tf < 0.0 || tn > bestT) continue;
        if (isW > 0.5) {
            // white cell: lumpy sphere, short march inside its bound
            float s = k * 1.7;
            mat2 roll = rot(sC.x);          // the white cell rolls with its simulated spin
            float tt = max(tn, 0.0);
            bool hit = false;
            float d = 1.0;
            for (int j = 0; j < 22; j++) {
                vec3 lq = (ro + rd * tt - C) / R;
                lq.yz = roll * lq.yz;
                d = wbcLump(lq, s) * R * 0.7;
                if (d < 0.004 * R) { hit = true; break; }
                tt += d;
                if (tt > tf) break;
            }
            if (!hit && d < 0.03 * R && tt < tf) hit = true;
            if (hit && tt < bestT) {
                vec3 l = (ro + rd * tt - C) / R;
                l.yz = roll * l.yz;
                vec3 n = vec3(0.0);
                for (int j = 0; j < 4; j++) {
                    vec3 e = 0.5773 * (2.0 * vec3(float(((j + 3) / 2) - 2 * ((j + 3) / 4)), float((j / 2) - 2 * (j / 4)), float(j - 2 * (j / 2))) - 1.0);
                    n += e * wbcLump(l + e * 0.01, s);
                }
                n.yz = n.yz * roll;          // back to world (transpose rotation)
                bestT = tt; cN = normalize(n); cType = 1.0; cSeed = hA; cFade = fade; cRho = 0.0;
                cThick = length(l);
            }
        } else {
            // red cell / platelet: biconcave disc, tumbling (aligned with the flow in capillaries)
            float a1 = sC.x;                 // simulated tumble (Jeffery orbit + collision spin)
            float a2 = sC.y;
            vec3 Nn = normalize(vec3(sin(a1) * cos(a2), sin(a1) * sin(a2), cos(a1)));
            Nn = normalize(mix(Nn, vec3(0.0, 0.0, -1.0) + 0.25 * vec3(sin(a2), cos(a2), 0.0), cup));
            vec3 U = normalize(cross(Nn, abs(Nn.y) < 0.9 ? vec3(0.0, 1.0, 0.0) : vec3(1.0, 0.0, 0.0)));
            vec3 V = cross(Nn, U);
            vec3 lo = vec3(dot(o, U), dot(o, V), dot(o, Nn)) / R;
            vec3 ld = vec3(dot(rd, U), dot(rd, V), dot(rd, Nn)) / R;
            float thk = isP > 0.5 ? 0.6 : 1.0;
            float cupK = 0.45 * cup;
            float hmax = 0.34 * thk;
            // slab ∩ bounding sphere interval
            float z0 = -hmax, z1 = cupK + hmax;
            float ta0 = max(tn, 0.0), ta1 = tf;
            if (abs(ld.z) > 1e-5) {
                float s0 = (z0 - lo.z) / ld.z, s1 = (z1 - lo.z) / ld.z;
                ta0 = max(ta0, min(s0, s1)); ta1 = min(ta1, max(s0, s1));
            } else if (lo.z < z0 || lo.z > z1) continue;
            if (ta1 <= ta0) continue;
            float dt = (ta1 - ta0) / 10.0;
            float tp = ta0, fp = 1.0;
            float th = -1.0;
            for (int j = 0; j <= 10; j++) {
                float tt = ta0 + dt * float(j);
                vec3 lp = lo + ld * tt;
                float rho = length(lp.xy);
                float f = rho >= 1.0 ? rho - 0.99 : abs(lp.z - cupK * rho * rho) - 0.5 * thk * bicon(rho);
                if (f < 0.0) {
                    // refine between tp and tt
                    float lo_t = tp, hi_t = tt;
                    for (int q = 0; q < 4; q++) {
                        float mt = 0.5 * (lo_t + hi_t);
                        vec3 mp = lo + ld * mt;
                        float mr = length(mp.xy);
                        float mf = mr >= 1.0 ? mr - 0.99 : abs(mp.z - cupK * mr * mr) - 0.5 * thk * bicon(mr);
                        if (mf < 0.0) hi_t = mt; else lo_t = mt;
                    }
                    th = hi_t; break;
                }
                tp = tt; fp = f;
            }
            if (th > 0.0 && th < bestT) {
                vec3 lp = lo + ld * th;
                float rho = min(length(lp.xy), 0.985);
                vec2 rh = length(lp.xy) > 1e-4 ? lp.xy / length(lp.xy) : vec2(1.0, 0.0);
                float hp = 0.5 * thk * (bicon(rho + 0.01) - bicon(rho - 0.01)) / 0.02;
                float zcd = 2.0 * cupK * rho;
                float side = lp.z - cupK * rho * rho > 0.0 ? 1.0 : -1.0;
                vec3 nl = side > 0.0 ? vec3(-(zcd + hp) * rh, 1.0) : vec3((hp - zcd) * rh, -1.0);
                nl = normalize(nl);
                bestT = th; cN = normalize(U * nl.x + V * nl.y + Nn * nl.z);
                cType = isP > 0.5 ? 2.0 : 0.0; cRho = rho; cThick = bicon(rho) * thk; cSeed = hA; cFade = fade;
            }
        }
    }

    // ── shading ──
    float H = gH;
    vec3 fogC = vec3(0.3, 0.022, 0.028) * (0.85 + 0.5 * H + 0.3 * gLift) + vec3(0.9, 0.25, 0.2) * gDrop * 0.9;
    float fogK = mix(0.15, 0.5, gCap);
    vec3 col;
    vec3 p = ro + rd * bestT;
    vec3 keyL = normalize(vv * 0.8 + uu * 0.3 - ww * 0.2);
    float sparkle = 1.0 + 0.6 * highP() * ar + 0.5 * gBuild * (0.5 + 0.5 * sin(TIME * 37.0 + p.z * 11.0));
    float att = 1.0 / (1.0 + 0.35 * bestT * bestT / (gRbase * gRbase + 0.05));
    float hotness = 0.0;
    if (cType < -0.5) {
        // wet vessel wall
        vec3 n = vec3(0.0);
        for (int j = 0; j < 4; j++) {
            vec3 e = 0.5773 * (2.0 * vec3(float(((j + 3) / 2) - 2 * ((j + 3) / 4)), float((j / 2) - 2 * (j / 4)), float(j - 2 * (j / 2))) - 1.0);
            n += e * mapWall(p + e * 0.004 * gRbase);
        }
        n = normalize(n);
        vec2 q = p.xy - pathC(p.z);
        float a = atan(q.y, q.x);
        vec2 en = endo(vec2(a * 3.2, p.z * 4.5 / max(gRbase, 0.3)));
        float vein = smoothstep(0.92, 1.0, sin(a * 3.0 + sin(p.z * 0.8) * 2.0 + p.z * 0.3)) * 0.6;
        vec3 base = mix(vec3(0.42, 0.035, 0.05), vec3(0.75, 0.16, 0.16), en.y * 0.5);
        base *= 1.0 - 0.45 * en.x - 0.5 * vein;
        // pressure wave: a flush of colour rolling down the walls
        float wave = lubdub(gHB - (p.z - gCamZ) * 0.22) * pulseAmt;
        base *= 1.0 + 0.6 * wave + 0.4 * gHAudio;
        float dif = clamp(dot(n, -rd), 0.0, 1.0);
        float kd = clamp(dot(n, keyL), 0.0, 1.0);
        vec3 h1 = normalize(keyL - rd);
        float spec = pow(clamp(dot(n, h1), 0.0, 1.0), 70.0) + 0.5 * pow(clamp(dot(reflect(rd, n), -rd), 0.0, 1.0), 25.0);
        float fres = pow(1.0 - dif, 4.0);
        col = base * (0.25 + 0.9 * dif + 0.5 * kd) * att;
        col += vec3(1.0, 0.85, 0.8) * spec * 1.6 * wetness * sparkle * (0.4 + 0.6 * att) * (1.0 - 0.6 * en.x);
        col += fogC * 1.6 * fres * wetness;
        // drop shockwave sweeping toward the camera
        float sw = exp(-pow((p.z - gCamZ) - 7.0 * gDrop, 2.0) * 1.2) * gDrop;
        col += vec3(1.0, 0.45, 0.35) * sw * 1.4;
        hotness = 0.35 + 0.25 * wave;
    } else if (cType < 0.5) {
        // red blood cell: dimple shading, pale thin centre, glossy rim
        vec3 n = cN;
        float dif = clamp(dot(n, -rd), 0.0, 1.0);
        float kd = clamp(dot(n, keyL), 0.0, 1.0);
        float pale = clamp(1.0 - cThick / 0.66, 0.0, 1.0);
        vec3 base = mix(vec3(0.42, 0.015, 0.03), vec3(1.0, 0.36, 0.3), pow(pale, 1.5));
        float spec = pow(clamp(dot(n, normalize(keyL - rd)), 0.0, 1.0), 60.0) + 0.6 * pow(dif, 30.0);
        float fres = pow(1.0 - dif, 3.0);
        col = base * (0.12 + 0.7 * dif + 0.75 * kd) * att * (1.0 + 0.35 * H);
        col += vec3(0.7, 0.1, 0.07) * pow(pale, 2.0) * 0.35 * (0.5 + att);   // light glowing through the thin centre
        col += vec3(1.0, 0.4, 0.35) * fres * 0.35 * (0.6 + att);           // translucent rim glow
        col += vec3(1.0, 0.9, 0.85) * spec * wetness * sparkle * 0.8 * (0.4 + 0.6 * att);
        hotness = 0.72 + 0.15 * (1.0 - pale) + 0.2 * H;
    } else if (cType < 1.5) {
        // white cell: pearly, lumpy, granular
        vec3 n = cN;
        float dif = clamp(dot(n, -rd), 0.0, 1.0);
        float kd = clamp(dot(n, keyL), 0.0, 1.0);
        float gran = 0.5 + 0.5 * sin(n.x * 7.0 + cSeed * 20.0) * sin(n.y * 6.0 - n.z * 5.0);
        vec3 base = mix(vec3(0.9, 0.8, 0.86), vec3(1.0, 0.95, 0.92), gran);
        float spec = pow(clamp(dot(n, normalize(keyL - rd)), 0.0, 1.0), 40.0);
        float fres = pow(1.0 - dif, 2.5);
        col = base * (0.3 + 0.75 * dif + 0.55 * kd) * (0.3 + 0.7 * att);
        col += vec3(1.0, 0.55, 0.6) * fres * 0.4 * (0.5 + att);
        col += spec * wetness * sparkle * 0.7 * att;
        hotness = 0.95;
    } else {
        // platelet: small amber shard
        vec3 n = cN;
        float dif = clamp(dot(n, -rd), 0.0, 1.0);
        col = vec3(0.95, 0.65, 0.35) * (0.35 + 0.9 * dif) * att + pow(dif, 20.0) * wetness * sparkle * 0.6;
        hotness = 0.85;
    }
    // plasma fog: glowing red depth
    float fg = 1.0 - exp(-bestT * fogK / max(gDil, 0.5));
    col = mix(col, fogC, fg);
    col += backdrop(uv) * fg * 0.35;
    // the light at the end of the vessel
    float ahead = pow(clamp(dot(rd, ww), 0.0, 1.0), 40.0) * (1.0 - gCap * 0.5);
    col += vec3(1.0, 0.35, 0.25) * ahead * (0.12 + 0.25 * H + 0.8 * gDrop) * fg;
    col *= 0.95 + 0.2 * gLift + 0.1 * gPeak + 0.45 * gRest;
    heat = mix(hotness, 0.1, fg * 0.8) * (0.8 + 0.25 * H) + 0.4 * gDrop;
    return vec4(col, 1.0);
}

// ── Microscope smear: brightfield slide, Beer–Lambert absorption ───────
vec4 smearImage(vec2 uv, vec2 sp) {
    float ar = audioReact;
    float H = gH;
    float zoom = 1.0 - 0.18 * gBuild - 0.035 * H + 0.1 * gRest;
    vec2 trem = (vec2(h11(floor(TIME * 30.0)), h11(floor(TIME * 30.0) + 7.0)) - 0.5) * 0.006 * smoothstep(0.7, 1.0, gBuild);
    vec2 p = rot(0.04 * sin(TIME * 0.07)) * (sp + trem) * zoom;
    p += vec2(TIME * 0.018 + audioMidTime * 0.012 * ar, 0.012 * sin(TIME * 0.05) + TIME * 0.004);
    p += gG2.xy;                         // kicks shove the slide; a damped spring brings it home
    float g = 0.11;
    vec2 cell = p / g;
    vec2 id0 = floor(cell);
    float focus = 0.06 + 0.07 * (0.5 + 0.5 * sin(TIME * 0.37)) + 0.08 * H * pulseAmt + 0.1 * gRest - 0.04 * gPeak;
    float dens = mix(0.45, 0.97, clamp(cellDensity * (1.0 - 0.45 * gRest) * (1.0 + 0.25 * gPeak), 0.0, 1.0));
    vec3 absorb = vec3(0.0);
    vec3 kRBC = vec3(0.32, 2.3, 1.5) * 1.45;
    vec3 kNuc = vec3(1.7, 2.3, 0.75);
    vec3 kCyt = vec3(0.22, 0.32, 0.1);
    vec3 kPlt = vec3(1.3, 1.9, 0.5);
    vec2 ctr = p - sp * zoom;            // slide point at screen centre
    for (int j = -1; j <= 1; j++)
    for (int i = -1; i <= 1; i++) {
        vec2 id = id0 + vec2(float(i), float(j));
        float h = h12(id);
        vec2 jit = h22(id + 5.3);
        vec2 c = id + 0.5 + (jit - 0.5) * 0.6 + 0.04 * vec2(sin(TIME * 0.6 + h * 20.0), cos(TIME * 0.5 + h * 13.0));
        // drop: cells blown outward from the centre of view
        vec2 away = c * g - ctr;
        c += normalize(away + 1e-4) * gG1.z * 0.45 * exp(-dot(away, away) * 6.0);
        vec2 q = cell - c;
        float hT = h12(id + 17.0);
        if (h < dens) {
            if (hT < 0.035 * (0.5 + wbcAmt) * (1.0 + gPeak)) {
                // white cell: lilac cytoplasm, lobed purple nucleus, granules
                float r = length(q) / 0.66;
                float cyt = smoothstep(1.0, 1.0 - focus * 1.5, r);
                float nuc = 0.0;
                float ra = h * TAU + TIME * 0.05;
                for (int l = 0; l < 3; l++) {
                    float a = ra + float(l) * 2.2;
                    vec2 lc = 0.19 * vec2(cos(a), sin(a));
                    vec2 lc2 = 0.19 * vec2(cos(a + 2.2), sin(a + 2.2));
                    vec2 pa = q - lc, ba = lc2 - lc;
                    float hseg = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
                    float lob = min(length(q - lc) - 0.2, length(pa - ba * hseg) - 0.07 - 0.02 * float(l));
                    nuc = max(nuc, smoothstep(0.0, -focus * 0.8 - 0.01, lob) * (l < 2 ? 1.0 : 0.0) + smoothstep(0.0, -focus * 0.8 - 0.01, length(q - lc) - 0.2));
                }
                float gr = smoothstep(0.55, 0.9, sin(q.x * 70.0 + h * 9.0) * sin(q.y * 63.0)) * cyt;
                absorb += kCyt * cyt + kNuc * nuc * 0.9 + kCyt * gr * 1.5;
            } else {
                // red cell: biconcave disc seen in transmitted light → pale centre
                vec2 rq = rot(h * TAU) * q;
                rq.x *= 1.0 + 0.14 * jit.x;
                float rad = 0.42 * (0.9 + 0.2 * jit.y) * (1.0 + 0.04 * H);
                float r = length(rq) / rad;
                float T = pow(bicon(min(r, 0.999)) / 0.66, 1.4) * 0.72 * smoothstep(1.0, 1.0 - focus * 2.0, r);
                float rim = exp(-pow((r - 0.98) / (0.05 + focus), 2.0)) * 0.22;
                absorb += kRBC * T + vec3(0.6, 0.9, 0.8) * rim;
            }
        }
        // platelets: tiny purple specks
        if (h12(id + 31.0) < 0.22) {
            vec2 pc = c + (h22(id + 41.0) - 0.5) * 0.9;
            float pr = 0.07 + 0.05 * h12(id + 43.0);
            float pt = smoothstep(pr, pr * (1.0 - focus * 4.0) - 0.005, length(cell - pc));
            absorb += kPlt * pt * 0.6;
        }
    }
    // brightfield lamp and field stop
    float d = length(sp);
    float fieldR = 0.56;
    vec3 lamp = vec3(0.95, 0.92, 0.94) * (0.88 + 0.12 * gLift + 0.06 * H + 0.7 * gDrop);
    lamp *= 1.0 - 0.4 * pow(clamp(d / fieldR, 0.0, 1.0), 3.0);
    vec3 col = lamp * exp(-absorb);
    float inside = smoothstep(fieldR + 0.006, fieldR - 0.006, d);
    col += vec3(0.0, 0.05, 0.12) * exp(-pow((d - fieldR) / 0.012, 2.0)) * 0.6;   // slight chromatic fringe
    col = mix(backdrop(uv), col, inside);
    return vec4(col, inside);
}

// ── Pass 0: the simulation ──────────────────────────────────────────────
// Pulsatile Poiseuille flow u(r) = U·(1 − r²) whose centreline speed U is driven by
// heart/kick/drop pressure surges (Windkessel relaxation); cells are rigid bodies
// dragged toward the local plasma velocity, with soft-sphere DEM collisions
// (spring–dashpot, restitution = Bounce), wall reflection, inertial lift toward the
// core (cell-free layer), white-cell margination, sedimentation, and Jeffery-orbit
// tumbling driven by the local shear rate plus collision torque.
vec4 physicsPass(vec2 fc, bool bad) {
    vec2 cs = RENDERSIZE / vec2(NBF, 4.0);
    vec2 cell = floor(fc / cs);
    vec2 ctr = (cell + 0.5) * cs;
    if (abs(fc.x - ctr.x) > 2.0 || abs(fc.y - ctr.y) > 2.0) return vec4(0.0);
    bool init = FRAMEINDEX < 2 || resetPhysics || bad;
    float dt = clamp(TIMEDELTA, 0.0, 1.0 / 30.0);
    float ph = physicsAmt;
    float ar = audioReact;
    float kick = kickForce * clamp(0.8 * audioBeatPulse + 0.5 * bassP(), 0.0, 1.5) * ar;
    float heart = lubdub(gHB) * (0.55 + 0.45 * (1.0 - gRest)) * pulseAmt;
    float U = gU;

    if (cell.y > 2.5) {
        if (cell.x < 0.5) {
            // vessel: centreline flow with inertia + compliant wall (damped spring)
            vec4 G0 = stTex(0.0, 3.0);
            float D = G0.y, Dv = G0.z;
            float Ub = flowSpeed * (0.4 + 0.35 * (1.0 - gRest) + 0.55 * midP() * ar + 1.3 * gBuild + 0.45 * gPeak);
            float surge = (0.3 + 0.7 * ph) * flowSpeed * (4.0 * heart + 7.0 * kick + 30.0 * gDrop);
            U = clamp(U + ((Ub - U) / 0.7 + surge) * dt, 0.0, 7.0);
            float D0 = 1.0 + 0.14 * gRest - 0.24 * gBuild + 0.05 * gPeak;
            float Pw = 0.05 * heart + 0.07 * kick + 0.6 * gDrop;
            Dv += (70.0 * (D0 + Pw - D) - 4.5 * Dv) * dt;
            D = clamp(D + Dv * dt, 0.6, 1.9);
            Dv = clamp(Dv, -6.0, 6.0);
            if (init) { U = Ub; D = D0; Dv = 0.0; }
            return vec4(U, D, Dv, 1.0);
        }
        if (cell.x < 1.5) {
            // camera tracker (split hi/lo for fp16) + smear blast spring
            float cz = mod(gCamZ, 256.0);
            float B = gG1.z, Bv = gG1.w;
            Bv += (-40.0 * B - 5.0 * Bv + 60.0 * gDrop * (0.4 + 0.6 * ph)) * dt;
            B = clamp(B + Bv * dt, -1.0, 2.0);
            if (init) { B = 0.0; Bv = 0.0; }
            return vec4(floor(cz * 8.0) / 8.0, fract(cz * 8.0), B, Bv);
        }
        if (cell.x < 2.5) {
            // smear slide shove spring: kicks shove it, it rings and settles
            vec2 s = gG2.xy, sv = gG2.zw;
            float a = audioBassTime * 1.3 + TIME * 0.2;
            vec2 F = vec2(cos(a), sin(a)) * (kick * 2.5 + gDrop * 4.0) * ph;
            sv += (-45.0 * s - 4.0 * sv + F) * dt;
            s += sv * dt;
            s = clamp(s, -0.15, 0.15);
            if (init) { s = vec2(0.0); sv = vec2(0.0); }
            return vec4(s, sv);
        }
        return vec4(0.0);
    }

    float i = cell.x;
    vec4 A = stTex(i, 0.0), Bs = stTex(i, 1.0), Cs = stTex(i, 2.0);
    vec2 q = A.xy;
    float z = A.z + A.w / 128.0;
    vec2 vq = Bs.xy;
    float vz = Bs.z, seed = Bs.w;
    float th = Cs.x, phi = Cs.y, wth = Cs.z, wph = Cs.w;
    bool spawn = init || !(abs(z) < 20.0) || !(length(q) < 2.0) || !(abs(vz) < 20.0);
    if (spawn) {
        seed = h11(i * 7.31 + 0.5);
        z = (i + h11(i * 1.9 + 4.0)) / NBF * BL;
    } else {
        float vr = gRbase * gDil;
        vec4 T = bodyType(seed);
        float Ri = T.x * T.w;
        float mi = T.y > 0.5 ? 4.0 : (T.z > 0.5 ? 0.3 : 1.0);
        float r = length(q);
        vec2 qn = r > 1e-3 ? q / r : vec2(cos(seed * 43.0), sin(seed * 43.0));
        // Poiseuille plasma velocity at this radius (white cells roll slower near the wall, plug capillaries)
        float uz = U * (1.0 - r * r);
        if (T.y > 0.5) uz *= mix(1.0, 0.55, smoothstep(0.5, 0.9, r)) * mix(1.0, 0.6, gCap);
        vec2 aq = -vq * 3.0;                         // transverse viscous drag
        float az = (uz - vz) * 6.0;                  // axial drag toward the plasma (tau ≈ 0.17 s)
        float lift = ph * (0.5 + 0.5 * U) * smoothstep(0.35, 1.0, r);
        aq += T.y > 0.5 ? qn * lift * 0.6 : -qn * lift * 2.2;          // margination vs. cell-free layer
        aq.y -= ph * gravity * 0.35 / max(vr, 0.2);                     // sedimentation
        vec2 jit = h22(vec2(i * 3.1, floor(gHB * 2.0))) - 0.5;
        aq += ph * kick * 9.0 * jit;                                     // kicks jostle the stream
        aq += ph * gDrop * 9.0 * qn;                                    // drop pressure wave scatters outward

        // soft-sphere collisions (DEM spring–dashpot) + short-range hydrodynamic repulsion
        vec3 Pi = vec3(q * vr, z);
        vec3 Vi = vec3(vq * vr, -vz);
        vec3 F = vec3(0.0);
        float torque = 0.0;
        float zeta = (1.0 - bounce) * 0.7 + 0.03;
        float kC = 500.0, cC = 2.0 * zeta * sqrt(kC);
        if (Ri > 0.004) {
            for (int j = 0; j < NB; j++) {
                float fj = float(j);
                if (abs(fj - i) < 0.5) continue;
                vec4 Aj = stTex(fj, 0.0);
                vec3 d = vec3(Aj.xy * vr, Aj.z + Aj.w / 128.0) - Pi;
                if (abs(d.z) > 0.9) continue;
                vec4 Bj = stTex(fj, 1.0);
                vec4 Tj = bodyType(Bj.w);
                float Rs = Ri + Tj.x * Tj.w;
                float dist = length(d);
                if (dist > 2.0 * Rs || dist < 1e-4 || Tj.w < 0.01) continue;
                vec3 n = d / dist;
                vec3 vrel = vec3(Bj.xy * vr, -Bj.z) - Vi;
                float vn = dot(vrel, n);
                float ov = Rs - dist;
                float hyd = 1.0 - dist / (2.0 * Rs);
                float f = -3.0 * hyd * hyd;
                if (ov > 0.0) f += -kC * ov + cC * vn;
                F += f * n;
                vec3 vt = vrel - vn * n;
                torque += (ov > 0.0 ? 10.0 : 2.0) * hyd * (vt.z * (n.x + n.y) - (vt.x + vt.y) * n.z);
            }
        }
        aq += ph * F.xy / (mi * vr);
        az += ph * (-F.z) / mi;

        vq = clamp(vq + aq * dt, -4.0, 4.0);
        vz = clamp(vz + az * dt, -1.0, 8.0);
        q += vq * dt;
        // vessel wall: reflect with restitution, wall contact makes the cell roll
        float rmax = max(1.0 - T.x * 0.85 / vr, 0.0);
        r = length(q);
        if (r > rmax) {
            vec2 wn = r > 1e-4 ? q / r : qn;
            q = wn * rmax;
            float vrad = dot(vq, wn);
            if (vrad > 0.0) { vq -= (1.0 + bounce) * vrad * wn; wth += vrad * 4.0 * ph; }
            wth += vz * 0.6 * ph * dt * 10.0;
        }
        // tumbling: Jeffery orbit of an oblate disc (aspect ≈ 0.35) in the local shear + collision torque
        r = length(q);
        float shear = 2.0 * U * r / max(vr, 0.2);
        float ct = cos(th), st = sin(th);
        float jeff = tumble * (0.35 + 0.9 * shear * (0.12 * ct * ct + st * st)) * (1.0 - 0.8 * gCap);
        wth += ((jeff - wth) * 3.0 + torque * ph) * dt;
        wth = clamp(wth, -25.0, 25.0);
        th = mod(th + wth * dt, TAU);
        float tgt = atan(q.y, q.x + 1e-5);
        float dph = mod(tgt - phi + PI, TAU) - PI;
        wph += (dph * 2.0 - wph * 2.0 + torque * 0.3 * ph) * dt;
        wph = clamp(wph, -15.0, 15.0);
        phi = mod(phi + wph * dt, TAU);

        // advance toward (and past) the camera; recycle at the far end
        z -= vz * dt;
        float prevC = gG1.x + gG1.y / 8.0;
        float dC = mod(gCamZ, 256.0) - prevC;
        if (dC < -128.0) dC += 256.0;
        if (dC > 128.0) dC -= 256.0;
        z -= clamp(dC, -0.05, 0.3);
        if (z < -0.35) { z += BL; seed = fract(seed + 0.618034 + 0.0137 * i); spawn = true; }
        else if (z > BL - 0.2) z -= BL;
    }
    if (spawn) {
        float r0 = 0.82 * sqrt(h11(seed * 311.7 + i));
        float a0 = TAU * h11(seed * 173.1 + 2.0 * i);
        q = r0 * vec2(cos(a0), sin(a0));
        vq = vec2(0.0);
        vz = U * (1.0 - r0 * r0);
        th = TAU * h11(seed * 59.3 + 1.0);
        phi = a0; wth = 0.0; wph = 0.0;
    }
    float zh = floor(z * 128.0) / 128.0;
    float zl = fract(z * 128.0);
    if (cell.y < 0.5) return vec4(q, zh, zl);
    if (cell.y < 1.5) return vec4(vq, vz, seed);
    return vec4(th, phi, wth, wph);
}

void main() {
    showDynamics();
    vec2 R = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uv = fc / R;
    vec2 sp = (fc - 0.5 * R) / R.y;
    float ar = audioReact;

    // form weights: 0 Artery, 1 Capillary, 2 Thermal, 3 Smear (auto order)
    float fsel = float(formSel);
    float cur, nxt, bl;
    if (fsel < 0.5) {
        float ph = TIME / morphPeriod;
        cur = mod(floor(ph), 4.0);
        nxt = mod(cur + 1.0, 4.0);
        bl = smoothstep(0.8, 1.0, fract(ph));
    } else {
        cur = fsel < 1.5 ? 0.0 : fsel < 2.5 ? 1.0 : fsel < 3.5 ? 3.0 : 2.0;
        nxt = cur; bl = 0.0;
    }
    vec4 wc = vec4(step(abs(cur - 0.0), 0.1), step(abs(cur - 1.0), 0.1), step(abs(cur - 2.0), 0.1), step(abs(cur - 3.0), 0.1));
    vec4 wn = vec4(step(abs(nxt - 0.0), 0.1), step(abs(nxt - 1.0), 0.1), step(abs(nxt - 2.0), 0.1), step(abs(nxt - 3.0), 0.1));
    vec4 W = mix(wc, wn, bl);
    gCap = smoothstep(0.0, 1.0, W.y);
    gThermal = W.z;
    gSmear = smoothstep(0.0, 1.0, W.w);

    // heartbeat
    gHB = TIME * heartRate / 60.0;
    gHAudio = clamp((0.7 * bassP() + 0.8 * audioBeatPulse) * ar, 0.0, 1.5);
    float autoH = lubdub(gHB) * (0.55 + 0.45 * (1.0 - gRest)) * (1.0 - 0.4 * clamp(gHAudio, 0.0, 1.0));
    gH = clamp(autoH + gHAudio, 0.0, 1.6) * pulseAmt;
    // physics state (flow and dilation are integrated in pass 0)
    vec4 G0 = stTex(0.0, 3.0);
    gG1 = stTex(1.0, 3.0);
    gG2 = stTex(2.0, 3.0);
    bool okG = abs(G0.w - 1.0) < 0.01 && abs(G0.x) < 50.0 && abs(G0.y) < 5.0;
    gU = okG ? G0.x : 1.0;
    gDil = okG ? G0.y : 1.0;
    if (!(abs(gG1.z) < 5.0) || !(length(gG2.xy) < 1.0)) { gG1 = vec4(0.0); gG2 = vec4(0.0); }
    gCamZ = TIME * 0.3 + audioBassTime * 0.08 * ar + 0.6 * gBuild * gBuild;
    gRbase = mix(1.0, 0.25, gCap);
    gDens = clamp(cellDensity * (1.0 - 0.6 * gRest) * (0.85 + 0.25 * gPeak + 0.2 * gDrop), 0.0, 1.0) * mix(1.0, 0.5, gCap);

    if (PASSINDEX == 0) {
        gl_FragColor = physicsPass(fc, !okG);
        return;
    }

    vec3 col = vec3(0.0);
    float alpha = 1.0;
    if (gSmear < 0.999) {
        float heat;
        vec4 A = vesselImage(uv, sp, heat);
        vec3 c3 = A.rgb;
        if (gThermal > 0.001) {
            float hh = heat + 0.04 * sin(fc.y * 1.3 + TIME * 20.0) * 0.2;
            c3 = mix(c3, ironbow(hh), gThermal);
        }
        col = c3;
        alpha = clamp(lum(col) * 4.0, 0.0, 1.0);
    }
    if (gSmear > 0.001) {
        vec4 B = smearImage(uv, sp);
        col = mix(col, B.rgb, gSmear);
        alpha = mix(alpha, B.a, gSmear);
    }
    // transition bloom between the vessel and the slide
    col += vec3(1.0, 0.85, 0.85) * sin(PI * gSmear) * 0.35;
    col += vec3(1.0, 0.8, 0.75) * gDrop * gDrop * 0.12;
    col *= 1.0 + 0.1 * audioBeatPulse * ar;
    col += (h12(fc + fract(TIME * 13.7) * 91.0) - 0.5) * grain * 0.05;
    col += (h12(fc * 1.03 + fract(TIME * 7.1) * 57.0) + h12(fc * 0.97 + 11.0) - 1.0) * (1.5 / 255.0);
    col = studioPalette(clamp(col, 0.0, 1.0));
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), transparentBg ? alpha : 1.0);
}
