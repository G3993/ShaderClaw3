/*{
  "DESCRIPTION": "Komorebi — sunlight falling through leaves into a quiet room, rendered from the real optics: every gap in the canopy is a pinhole camera, so each dancing spot of light on the plaster wall and oak floor is an image of the sun itself — round no matter what shape the gap is, larger the higher the leaf, stretched into ellipses where the low golden-hour beam rakes across the wall. A window frames the beam into panes, a sprig just outside throws soft leaf shadows, and the whole pattern swims and twinkles as wind gusts roll through the tree. Then a slow eclipse arrives: the moon bites into the sun and every one of the hundreds of spots turns into a crescent at once, narrows to a ring of fire at the annular peak, and swings back to round — the hidden physics of dappled light made visible. Dust drifts through window-shaped god rays that carry the same crescents, motes glint when they cross a beam, the plaster's trowel texture rises under the grazing light and the varnished floor holds a soft reflection of the wall. Bass gusts the wind through the canopy and lifts the sun, beats breathe the god rays, highs sparkle the motes.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. CURVEBALL pack #1",
  "CATEGORIES": ["Generator", "3D", "Nature", "Light", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",        "LABEL": "Wind Speed",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "eclipseSpeed", "LABEL": "Eclipse Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "eclipse",      "LABEL": "Eclipse",           "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Eclipse" },
    { "NAME": "eclipseCycle", "LABEL": "Eclipse Cycle (s)", "TYPE": "float", "AUDIO": false, "MIN": 30.0, "MAX": 240.0, "DEFAULT": 90.0, "GROUP": "Eclipse" },
    { "NAME": "moonSize",     "LABEL": "Moon Size (ring→total)", "TYPE": "float", "AUDIO": false, "MIN": 0.8, "MAX": 1.05, "DEFAULT": 0.9, "GROUP": "Eclipse" },
    { "NAME": "gapDensity",   "LABEL": "Canopy Gaps",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.65, "GROUP": "Canopy" },
    { "NAME": "spotSize",     "LABEL": "Sun Spot Size",     "TYPE": "float", "AUDIO": false, "MIN": 0.5, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Canopy" },
    { "NAME": "wind",         "LABEL": "Wind",              "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.7, "GROUP": "Canopy" },
    { "NAME": "twinkle",      "LABEL": "Leaf Twinkle",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "GROUP": "Canopy" },
    { "NAME": "nearLeaves",   "LABEL": "Near Sprig",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Canopy" },
    { "NAME": "windowFrame",  "LABEL": "Window Frame",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Room" },
    { "NAME": "sunAz",        "LABEL": "Sun Direction",     "TYPE": "float", "AUDIO": false, "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Room" },
    { "NAME": "sunEl",        "LABEL": "Sun Height",        "TYPE": "float", "AUDIO": false, "MIN": -1.0, "MAX": 1.0, "DEFAULT": 0.0, "GROUP": "Room" },
    { "NAME": "wallColor",    "LABEL": "Wall Color",        "TYPE": "color", "DEFAULT": [0.80, 0.73, 0.64, 1.0], "GROUP": "Color" },
    { "NAME": "floorColor",   "LABEL": "Floor Color",       "TYPE": "color", "DEFAULT": [0.40, 0.23, 0.11, 1.0], "GROUP": "Color" },
    { "NAME": "sunColor",     "LABEL": "Sun Color",         "TYPE": "color", "DEFAULT": [1.0, 0.66, 0.32, 1.0], "GROUP": "Color" },
    { "NAME": "shadeColor",   "LABEL": "Shade Color",       "TYPE": "color", "DEFAULT": [0.46, 0.46, 0.54, 1.0], "GROUP": "Color" },
    { "NAME": "skyColor",     "LABEL": "Skylight Color",    "TYPE": "color", "DEFAULT": [0.42, 0.58, 0.95, 1.0], "GROUP": "Color" },
    { "NAME": "leafColor",    "LABEL": "Leaf Glow Color",   "TYPE": "color", "DEFAULT": [0.45, 0.85, 0.20, 1.0], "GROUP": "Color" },
    { "NAME": "leafGlow",     "LABEL": "Leaf Glow",         "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "saturation",   "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "sunIntensity", "LABEL": "Sun Intensity",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "ambient",      "LABEL": "Shade Fill",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Light" },
    { "NAME": "dust",         "LABEL": "God Rays",          "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.65, "GROUP": "Light" },
    { "NAME": "motes",        "LABEL": "Dust Motes",        "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.8, "GROUP": "Light" },
    { "NAME": "exposure",     "LABEL": "Exposure",          "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "bloom",        "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6, "GROUP": "Finish" },
    { "NAME": "grain",        "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "volRes",       "LABEL": "God Ray Resolution","TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Finish" },
    { "NAME": "lightRes",     "LABEL": "Sun Spot Resolution","TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Finish" },
    { "NAME": "audioReact",   "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6, "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg","LABEL": "Transparent BG",    "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "volBuf", "WIDTH": "floor($WIDTH*$volRes)", "HEIGHT": "floor($HEIGHT*$volRes)" },
    { "TARGET": "lightBuf", "WIDTH": "floor($WIDTH*$lightRes)", "HEIGHT": "floor($HEIGHT*$lightRes)" },
    { "TARGET": "sceneBuf" },
    { "TARGET": "bloomH", "WIDTH": "floor($WIDTH*0.5)", "HEIGHT": "floor($HEIGHT*0.5)" },
    { "TARGET": "bloomV", "WIDTH": "floor($WIDTH*0.5)", "HEIGHT": "floor($HEIGHT*0.5)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Komorebi — CURVEBALL pack #1
//   The optics: a gap in the canopy at distance t (along the sun ray) from a
//   surface point acts as a pinhole, projecting an inverted image of the sun
//   disk of radius t·θ. Working in the plane perpendicular to the sun
//   direction ("sun uv"), the projection of a point onto that plane does not
//   depend on the layer distance, so every canopy layer is a jittered grid of
//   gaps in sun-uv and each spot is an analytic sun image: disk with limb
//   darkening minus the moon disk → crescents / annular rings, exact & crisp.
//   Spots land elliptical on the wall automatically (the beam is oblique).
//   pass 0 volBuf   : in-scattered sunlight along the view ray (god rays)
//   pass 1 sceneBuf : wall / floor shading, varnish reflection, motes (HDR)
//   pass 2/3        : half-res separable gaussian bloom
//   pass 4 final    : ACES, HD finisher, grain
// ─────────────────────────────────────────────────────────────────────────

#define TAU 6.28318530718

float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec3  hash32(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yxz + 33.33); return fract((p3.xxy + p3.yzz) * p3.zyx); }
float vnoise(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm3o(vec2 p) { float s = 0.0, a = 0.5; for (int i = 0; i < 3; i++) { s += a * vnoise(p); p = p * 2.07 + 13.1; a *= 0.5; } return s; }
float fbm5o(vec2 p) { float s = 0.0, a = 0.5; for (int i = 0; i < 5; i++) { s += a * vnoise(p); p = p * 2.03 + 7.7; a *= 0.5; } return s; }
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { return clamp((x * (2.51 * x + 0.03)) / (x * (2.43 * x + 0.59) + 0.14), 0.0, 1.0); }

// ── shared state ─────────────────────────────────────────────────────────
float g_T, g_ar, g_bass, g_high, g_beat, g_lev, g_theta, g_rm, g_vis, g_windA;
vec2  g_moon, g_winC;
vec3  g_s, g_A, g_B, g_ro, g_uu, g_vv, g_ww, g_sunC, g_shadeC;

float circleOverlap(float d, float r1, float r2) {
    // area of the intersection of two circles (radii r1, r2, centres d apart)
    if (d >= r1 + r2) return 0.0;
    if (d <= abs(r1 - r2)) { float r = min(r1, r2); return 3.14159265 * r * r; }
    float a = r1 * r1 * acos(clamp((d * d + r1 * r1 - r2 * r2) / (2.0 * d * r1), -1.0, 1.0));
    float b = r2 * r2 * acos(clamp((d * d + r2 * r2 - r1 * r1) / (2.0 * d * r2), -1.0, 1.0));
    float c = 0.5 * sqrt(max((-d + r1 + r2) * (d + r1 - r2) * (d - r1 + r2) * (d + r1 + r2), 0.0));
    return a + b - c;
}

void setup() {
    g_ar   = audioReact;
    g_bass = clamp(audioBass, 0.0, 1.0);
    g_high = clamp(audioHigh, 0.0, 1.0);
    g_beat = clamp(audioBeatPulse, 0.0, 1.0);
    g_lev  = clamp(audioLevel, 0.0, 1.0);
    // wind clock: steady breeze + a phase push from the integrated bass clock
    g_T = TIME * speed * 0.8 + audioBassTime * 0.3 * g_ar;
    g_windA = wind * (1.0 + 0.45 * g_ar * g_bass);

    // sun from the front-left, low golden hour
    float az = -0.98 + 0.45 * sunAz;
    float el = 0.50 + 0.22 * sunEl;
    g_s = normalize(vec3(sin(az) * cos(el), sin(el), cos(az) * cos(el)));
    g_A = normalize(cross(g_s, vec3(0.0, 1.0, 0.0)));
    g_B = cross(g_A, g_s);
    g_theta = 0.0105 * spotSize;      // apparent sun radius (≈2.2× real, readable crescents)

    // eclipse: the moon crosses the sun disk once per cycle (offset so frame 0 already shows a bite)
    float cyc = fract((TIME * eclipseSpeed + 6.4) / eclipseCycle);
    float mx = -2.35 + 4.7 * cyc / 0.6;
    g_moon = (eclipse > 0.001 && cyc < 0.6) ? vec2(mx, 0.05 + 0.08 * (cyc - 0.3)) : vec2(9.0);
    g_rm = moonSize;
    float d = length(g_moon);
    g_vis = 1.0 - circleOverlap(d, 1.0, g_rm) / 3.14159265 * eclipse;   // visible fraction of the disk

    vec3 sc = sunColor.rgb;
    // the light cools a touch and the shade deepens as the sun is eaten
    sc = mix(sc, sc * vec3(0.85, 0.95, 1.12), (1.0 - g_vis) * 0.6);
    g_sunC = sc * 2.35 * sunIntensity * (1.0 + 0.32 * g_ar * g_bass + 0.12 * g_ar * g_lev);
    g_shadeC = shadeColor.rgb * 0.40 * ambient * mix(0.55, 1.0, g_vis);

    // camera: looking into the room corner, with a barely-there handheld drift
    float ct = TIME * 0.05;
    g_ro = vec3(0.30 + 0.10 * sin(ct * 1.3), 1.02 + 0.04 * sin(ct * 0.9), 2.75);
    vec3 ta = vec3(0.42 + 0.05 * sin(ct), 0.80, 0.0);
    g_ww = normalize(ta - g_ro);
    g_uu = normalize(cross(vec3(0.0, 1.0, 0.0), g_ww));
    g_vv = cross(g_ww, g_uu);

    // the window patch is centred on this wall point (in sun-uv)
    vec3 Pc = vec3(0.42, 0.95, 0.0);
    g_winC = vec2(dot(Pc, g_A), dot(Pc, g_B));
}

vec3 camRay(vec2 uv) { return normalize(g_uu * uv.x + g_vv * uv.y + g_ww * 1.32); }

// ── the room: wall z=0, floor y=0, baseboard ─────────────────────────────
// returns t, and id: 0 wall, 1 floor, 2 baseboard, -1 none
float traceRoom(vec3 ro, vec3 rd, out float id) {
    float t = 1e9; id = -1.0;
    if (rd.z < 0.0) {
        float tw = -ro.z / rd.z; vec3 p = ro + rd * tw;
        if (p.y >= 0.0 && tw > 0.0) { t = tw; id = 0.0; }
        float tb = (0.03 - ro.z) / rd.z; vec3 q = ro + rd * tb;
        if (tb > 0.0 && q.y >= 0.0 && q.y < 0.13 && tb < t) { t = tb; id = 2.0; }
    }
    if (rd.y < 0.0) {
        float tf = -ro.y / rd.y; vec3 p = ro + rd * tf;
        if (tf > 0.0 && p.z >= 0.0 && tf < t) { t = tf; id = 1.0; }
    }
    return t;
}

// ── sun image: limb-darkened disk minus the moon, AA width e (in sun radii)
float sunImage(vec2 u, float e) {
    float r = length(u);
    float disk = smoothstep(1.0 + e, 1.0 - e, r);
    float limb = 1.0 - 0.5 * (1.0 - sqrt(max(1.0 - r * r, 0.0)));
    float moon = smoothstep(g_rm - e, g_rm + e, length(u - g_moon));
    return disk * limb * mix(1.0, moon, eclipse);
}

// window panes (sun-uv), with penumbra pw
float windowMask(vec2 uv, float pw) {
    vec2 q = uv - g_winC;
    vec2 hs = vec2(0.82, 0.92);
    vec2 dq = abs(q) - hs;
    float dBox = length(max(dq, 0.0)) + min(max(dq.x, dq.y), 0.0);
    float bw = 0.045;
    float dv = abs(q.x) - bw;
    float dh = abs(abs(q.y) - hs.y / 3.0) - bw;
    float open = smoothstep(pw, -pw, dBox) * smoothstep(-pw, pw, dv) * smoothstep(-pw, pw, dh);
    return mix(1.0, open, windowFrame);
}

// near sprig just outside the window: soft leaf silhouettes
float leafSDF(vec2 q, float len, float wid) {
    // pointed leaf: intersection of two offset circles (vesica)
    q = abs(q);
    float r = (len * len + wid * wid) / (2.0 * wid);
    return length(vec2(q.x, q.y + r - wid)) - r;
}
float sprigShadow(vec2 uv, float pw, float sway) {
    float occ = 0.0;
    for (int j = 0; j < 2; j++) {
        float fj = float(j);
        vec2 base = g_winC + (j == 0 ? vec2(-0.74, 0.62) : vec2(0.78, -0.70));
        float ang = (j == 0 ? -0.55 : 2.75) + sway * (0.6 + 0.4 * fj);
        vec2 dir = vec2(cos(ang), sin(ang)), per = vec2(-dir.y, dir.x);
        vec2 d0 = uv - base;
        float along = dot(d0, dir);
        if (along > -0.25 && along < 1.25) {
            // stem with gentle droop
            float bend = 0.22 + 0.04 * fj;
            float across = dot(d0, per) - bend * along * along;
            float stem = abs(across) - 0.010 * (1.2 - along * 0.6);
            occ = max(occ, smoothstep(pw, -pw, stem) * step(0.0, along) * step(along, 1.15));
            for (int i = 0; i < 8; i++) {
                float fi = float(i);
                float sa = 0.14 + 0.13 * fi;
                float side = mod(fi, 2.0) < 0.5 ? 1.0 : -1.0;
                vec2 sp = base + dir * sa + per * (bend * sa * sa);
                float la = ang + side * (0.75 + 0.1 * sin(fi * 2.3)) + sway * (1.2 + 0.3 * sin(fi)) ;
                vec2 ld = vec2(cos(la), sin(la));
                float L = 0.115 + 0.02 * sin(fi * 1.7 + fj);
                vec2 lc = sp + ld * (L + 0.01);
                vec2 lq = uv - lc;
                vec2 lq2 = vec2(dot(lq, ld), dot(lq, vec2(-ld.y, ld.x)));
                float dl = leafSDF(lq2, L, L * 0.38);
                occ = max(occ, smoothstep(pw, -pw, dl));
            }
        }
    }
    return 1.0 - occ * nearLeaves;
}

// foliage density in a layer → probability that a cell holds a gap
float gapProb(vec2 c, float k) {
    float n = 0.72 * vnoise(c * 0.55 + vec2(k * 3.7, -k * 1.9) + 0.03 * g_T) + 0.28 * vnoise(c * 1.9 + k * 5.1);
    return clamp(mix(-0.2, 1.6, n) * (0.3 + 1.0 * gapDensity), 0.0, 0.97);
}

// direct sun visibility at P: window × sprig × Σ canopy pinhole images
float sunVisL(vec3 P, float blur, bool lite) {
    float pa = dot(P, g_s);
    vec2 uv = vec2(dot(P, g_A), dot(P, g_B));

    float tw = 2.6 - pa;
    float win = windowMask(uv, tw * g_theta * 1.6 + blur + 0.004);
    if (win < 0.002) return 0.0;

    float gust = 0.5 + 0.5 * sin(dot(uv, vec2(0.9, 0.4)) * 1.6 - g_T * 1.1);
    gust = gust * gust;

    float tn = 0.85;
    float sway = g_windA * (0.05 * sin(g_T * 1.3) + 0.03 * sin(g_T * 2.9 + 1.0)) * (0.5 + gust);
    float spr = lite ? 1.0 : sprigShadow(uv, tn * g_theta * 1.6 + blur + 0.003, sway);
    if (spr < 0.002) return 0.0;

    float sum = 0.0;
    for (int k = 0; k < 4; k++) {
        if (lite && k == 0) continue;
        float fk = float(k);
        float c = 3.6 + 2.6 * fk;
        float t = c - pa;
        float R = t * g_theta;                       // sun image radius at this layer distance
        float cs = 2.3 * g_theta * (c + 2.5);        // cell size: one gap per cell, 3×3 neighbourhood is enough
        vec2 g = uv / cs;
        vec2 id = floor(g);
        for (int j = -1; j <= 1; j++) {
            for (int i = -1; i <= 1; i++) {
                vec2 cid = id + vec2(float(i), float(j));
                vec3 h = hash32(cid + fk * 31.7);
                if (h.z < gapProb(cid * cs, fk)) {
                    vec3 h2 = hash32(cid * 1.31 + fk * 7.3 + 5.1);
                    // leaves swing in the gusts → each pinhole wanders and blinks
                    float gl = 0.5 + 0.5 * sin(dot(cid * cs, vec2(0.9, 0.4)) * 1.6 - g_T * 1.1);
                    float amp = g_windA * (0.25 + 0.75 * gl * gl) * R * 0.55;
                    vec2 off = vec2(sin(g_T * (1.6 + h2.x) + h2.y * TAU), cos(g_T * (1.3 + h2.y) + h2.x * TAU)) * amp;
                    vec2 G = (cid + 0.18 + 0.64 * h.xy) * cs + off;
                    float gr = mix(0.12, 0.45, h2.z);                 // gap size / image size → edge softness
                    float tw2 = 0.5 + 0.5 * sin(g_T * (1.1 + 2.2 * h2.x) + h.x * 40.0);
                    float open = mix(1.0, 0.3 + 0.7 * smoothstep(0.05, 0.55, tw2), twinkle * min(g_windA, 1.0) * 0.8);
                    vec2 u = -(uv - G) / R;                           // inverted pinhole image
                    sum += open * mix(0.55, 1.0, h2.z) * sunImage(u, gr * 0.5 + blur / R + 0.012);
                }
            }
        }
    }
    float can = 1.0 - exp(-1.35 * sum * (lite ? 1.25 : 1.0));
    return win * spr * can;
}
float sunVis(vec3 P, float blur) { return sunVisL(P, blur, false); }

vec3 sunLit(float v) { return mix(vec3(lum(g_sunC)), g_sunC, 0.35 + 0.65 * smoothstep(0.0, 0.55, v)) * v; }

// ── materials ────────────────────────────────────────────────────────────
float wallHeight(vec2 p) {
    // limewash plaster: broad trowel sweeps + fine grain
    float sweep = fbm3o(vec2(p.x * 1.4 + p.y * 0.6, p.y * 3.2 - p.x * 0.4) * 1.3);
    float grainH = fbm3o(p * 38.0);
    return sweep * 0.6 + grainH * 0.4;
}
vec3 wallNormal(vec2 p) {
    float e = 0.004;
    float h0 = wallHeight(p);
    float hx = wallHeight(p + vec2(e, 0.0)), hy = wallHeight(p + vec2(0.0, e));
    return normalize(vec3(-(hx - h0) / e * 0.010, -(hy - h0) / e * 0.010, 1.0));
}
vec3 floorAlbedo(vec2 p, out float seam, out float rough) {
    // oak planks running toward the camera, staggered joints
    float pw = 0.19;
    float row = floor(p.x / pw);
    float fx = fract(p.x / pw);
    float lenP = 1.6 + 0.8 * hash12(vec2(row, 3.0));
    float zz = p.y + hash12(vec2(row, 1.0)) * lenP;
    float plank = floor(zz / lenP);
    float fz = fract(zz / lenP);
    seam = min(smoothstep(0.0, 0.025, fx) * smoothstep(1.0, 0.975, fx), smoothstep(0.0, 0.006, fz) * smoothstep(1.0, 0.994, fz));
    vec2 gp = vec2(p.x * 9.0 + row * 3.1, zz * 0.9 + plank * 5.0);
    float gr = fbm5o(vec2(gp.x * 3.0 + 0.6 * sin(gp.y * 1.7), gp.y * 0.35));
    float rings = 0.5 + 0.5 * sin(gr * 18.0 + gp.x * 2.0);
    float tone = 0.78 + 0.32 * hash12(vec2(row, plank));
    vec3 a = floorColor.rgb * tone * (0.82 + 0.28 * rings * gr);
    rough = 0.25 + 0.4 * gr;
    return a * mix(0.35, 1.0, seam);
}

vec3 shadeWall(vec3 P, vec3 rd, bool cheap, float vis) {
    vec2 p = P.xy;
    vec3 n = cheap ? vec3(0.0, 0.0, 1.0) : wallNormal(p);
    float ndl = max(dot(n, g_s), 0.0) / max(g_s.z, 0.2);
    vec3 alb = wallColor.rgb * (0.93 + 0.07 * fbm3o(p * 2.3));
    // bounce: the beam's patch glows softly around itself; the floor kicks warm light up the wall
    float patch = windowMask(vec2(dot(P, g_A), dot(P, g_B)), 0.45);
    float corner = exp(-P.y * 3.5);
    float side = 0.55 + 0.75 * smoothstep(2.6, -1.6, P.x) * (0.75 + 0.25 * smoothstep(2.4, 0.4, P.y));
    vec3 sky = skyColor.rgb * 0.30 * ambient * patch;
    vec3 leafT = leafColor.rgb * 0.10 * patch * g_vis * leafGlow * (0.6 + 0.4 * fbm3o(vec2(dot(P, g_A), dot(P, g_B)) * 3.0 + 0.05 * g_T));
    vec3 amb = g_shadeC * side * (0.75 + 0.25 * n.y) * (1.0 - 0.45 * patch) + sky + leafT + mix(g_sunC, vec3(lum(g_sunC)), 0.5) * (0.012 * patch + 0.02 * corner) * g_vis;
    float ao = 1.0 - 0.55 * exp(-P.y * 9.0);
    return alb * (sunLit(vis) * ndl + amb) * ao;
}

float lightSample(vec2 fc, vec2 res) {
    vec2 lr = max(floor(res * lightRes), vec2(1.0));
    vec2 p = fc / res * lr - 0.5;
    vec2 i = floor(p), f = fract(p);
    // Catmull-Rom weights → crisp spot edges after upsampling
    vec2 w0 = f * (-0.5 + f * (1.0 - 0.5 * f));
    vec2 w1 = 1.0 + f * f * (-2.5 + 1.5 * f);
    vec2 w2 = f * (0.5 + f * (2.0 - 1.5 * f));
    vec2 w3 = f * f * (-0.5 + 0.5 * f);
    vec2 w12 = w1 + w2;
    vec2 o12 = w2 / w12;
    vec2 t0 = (i - 0.5) / lr, t3 = (i + 2.5) / lr, t12 = (i + 0.5 + o12) / lr;
    float r = 0.0;
    r += texture2D(lightBuf, vec2(t12.x, t0.y)).r * w12.x * w0.y;
    r += texture2D(lightBuf, vec2(t0.x, t12.y)).r * w0.x * w12.y;
    r += texture2D(lightBuf, t12).r * w12.x * w12.y;
    r += texture2D(lightBuf, vec2(t3.x, t12.y)).r * w3.x * w12.y;
    r += texture2D(lightBuf, vec2(t12.x, t3.y)).r * w12.x * w3.y;
    float wsum = w12.x * w0.y + w0.x * w12.y + w12.x * w12.y + w3.x * w12.y + w12.x * w3.y;
    return clamp(r / wsum, 0.0, 1.5);
}

// ── main ─────────────────────────────────────────────────────────────────
void main() {
    setup();
    vec2 res = RENDERSIZE.xy;
    vec2 fc = gl_FragCoord.xy;
    vec2 uvS = (fc - 0.5 * res) / res.y;

    if (PASSINDEX == 0) {
        // god rays: in-scattered sun along the view ray, IGN-dithered
        vec3 rd = camRay(uvS);
        float id; float tHit = traceRoom(g_ro, rd, id);
        tHit = min(tHit, 9.0);
        float ign = fract(hash12(fc) + 0.61803 * float(FRAMEINDEX - (FRAMEINDEX / 64) * 64));
        const int N = 12;
        float dt = tHit / float(N);
        float acc = 0.0;
        for (int i = 0; i < N; i++) {
            float tt = (float(i) + ign) * dt;
            vec3 P = g_ro + rd * tt;
            float dens = 0.65 + 0.7 * fbm3o(P.xy * 1.3 + vec2(P.z * 0.7, 0.0) + vec2(0.05, 0.02) * TIME);
            acc += sunVisL(P, 0.02, true) * dens * dt;
        }
        // Henyey–Greenstein, mild forward lobe + isotropic so beams read from inside the room
        float cosT = dot(rd, -g_s);
        float g = 0.45;
        float hg = (1.0 - g * g) / pow(1.0 + g * g - 2.0 * g * cosT, 1.5) * 0.08 + 0.06;
        gl_FragColor = vec4(vec3(acc * hg), 1.0);
        return;
    }

    if (PASSINDEX == 1) {
        // sun visibility at the primary hit (half-ish res; spots are AA'd analytic shapes)
        vec3 rd = camRay(uvS);
        float id; float t = traceRoom(g_ro, rd, id);
        float v = 0.0;
        if (id >= 0.0) v = sunVis(g_ro + rd * t, 0.6 / (res.y * 0.25));
        gl_FragColor = vec4(v, 0.0, 0.0, 1.0);
        return;
    }

    if (PASSINDEX == 2) {
        vec3 rd = camRay(uvS);
        float id; float t = traceRoom(g_ro, rd, id);
        vec3 col = vec3(0.0);
        float cover = 0.0;
        if (id >= 0.0) {
            vec3 P = g_ro + rd * t;
            cover = 1.0;
            float LV = lightSample(fc, res);
            if (id < 0.5) {
                col = shadeWall(P, rd, false, LV);
            } else if (id < 1.5) {
                float seam, rough;
                vec3 alb = floorAlbedo(P.xz, seam, rough);
                vec3 n = vec3(0.0, 1.0, 0.0);
                float vis = LV;
                float ndl = max(g_s.y, 0.0) / 0.5;
                float corner = exp(-P.z * 3.0);
                float side = 0.55 + 0.75 * smoothstep(2.6, -1.6, P.x);
                float patchF = windowMask(vec2(dot(P, g_A), dot(P, g_B)), 0.5);
                vec3 amb = g_shadeC * side * 0.9 * (1.0 - 0.4 * patchF) + skyColor.rgb * 0.22 * ambient * patchF + g_sunC * (0.03 * corner + 0.02 * patchF) * g_vis;
                float ao = 1.0 - 0.5 * exp(-P.z * 8.0);
                col = alb * (sunLit(vis) * ndl * seam + amb) * ao;
                // varnish: glossy reflection of the wall, blurred with distance & roughness
                vec3 rr = reflect(rd, n);
                float fres = 0.04 + 0.96 * pow(1.0 - max(dot(-rd, n), 0.0), 5.0);
                if (rr.z < 0.0) {
                    float tw = -P.z / rr.z;
                    vec3 Pw = P + rr * tw;
                    if (Pw.y > 0.0) {
                        // reuse the visible wall's sun field: project the reflected point to the screen
                        vec3 dv = Pw - g_ro;
                        float zc = dot(dv, g_ww);
                        vec2 suv = vec2(dot(dv, g_uu), dot(dv, g_vv)) / zc * 1.32;
                        vec2 sfc = suv * res.y + 0.5 * res;
                        float rv = texture2D(lightBuf, clamp(sfc / res, 0.0, 1.0)).r;
                        vec3 wr = shadeWall(Pw, rr, true, rv);
                        col += wr * fres * 0.55 * seam * (1.0 - rough * 0.6);
                    }
                }
                // sun glints on the varnish where the sun spot sits
                vec3 hv = normalize(g_s - rd);
                col += g_sunC * vis * pow(max(dot(n, hv), 0.0), 60.0) * 0.25 * seam;
            } else {
                // baseboard: painted wood, top edge catches light
                vec3 alb = wallColor.rgb * 0.92;
                float vis = LV;
                float ndl = max(g_s.z, 0.0) / max(g_s.z, 0.2);
                col = alb * (sunLit(vis) * ndl * 0.9 + g_shadeC * 0.7);
                col *= 0.8 + 0.2 * smoothstep(0.11, 0.128, P.y);
            }
        }

        // god rays (half-res, smooth)
        // smooth the dithered half-res march: 9-tap tent at volume-texel spacing
        vec2 vt = 1.0 / max(floor(res * volRes), vec2(1.0));
        vec2 vuv = fc / res;
        float vol = texture2D(volBuf, vuv).r * 0.25;
        vol += (texture2D(volBuf, vuv + vec2(vt.x, 0.0)).r + texture2D(volBuf, vuv - vec2(vt.x, 0.0)).r
              + texture2D(volBuf, vuv + vec2(0.0, vt.y)).r + texture2D(volBuf, vuv - vec2(0.0, vt.y)).r) * 0.125;
        vol += (texture2D(volBuf, vuv + vt).r + texture2D(volBuf, vuv - vt).r
              + texture2D(volBuf, vuv + vec2(vt.x, -vt.y)).r + texture2D(volBuf, vuv - vec2(vt.x, -vt.y)).r) * 0.0625;
        col += mix(g_sunC, vec3(lum(g_sunC)), 0.3) * vol * dust * (1.0 + 0.35 * g_ar * g_beat * smoothstep(0.0, 0.3, audioBeatPhase + 0.3));

        // dust motes: three depth layers; they glint only inside the beams
        if (motes > 0.001) {
            for (int k = 0; k < 3; k++) {
                float fk = float(k);
                float depth = 1.1 + 0.85 * fk;
                if (depth < t) {
                    float cell = 0.055 + 0.025 * fk;
                    vec2 sp = uvS / cell + vec2(fk * 17.0, fk * 5.0);
                    vec2 cid = floor(sp);
                    vec3 h = hash32(cid + fk * 9.0);
                    if (h.z < 0.35) {
                        float tt = TIME * (0.12 + 0.1 * h.x);
                        vec2 mp = cid + 0.5 + 0.3 * vec2(sin(tt + h.x * TAU), cos(tt * 0.8 + h.y * TAU));
                        vec2 suv = mp * cell - vec2(fk * 17.0, fk * 5.0) * cell;
                        float pr = (2.4 - 0.6 * fk) / res.y;                     // radius in uv units
                        float dpx = length(uvS - suv);
                        if (dpx < pr) {
                            vec3 wp = g_ro + camRay(suv) * depth;
                            float lit = sunVisL(wp, 0.01, true);
                            float soft = (k == 0) ? 0.6 : 0.25;
                            float m = smoothstep(pr, pr * soft, dpx);
                            float tw = 0.6 + 0.4 * sin(TIME * (3.0 + 5.0 * h.y) + h.x * 30.0);
                            col += g_sunC * lit * m * tw * motes * (0.5 + 0.35 * fk) * (1.0 + 0.8 * g_ar * g_high);
                        }
                    }
                }
            }
        }
        gl_FragColor = vec4(col, cover);
        return;
    }

    // ── bloom: separable gaussian at half res (no tap ghosts) ────────────
    if (PASSINDEX == 3 || PASSINDEX == 4) {
        vec2 uvb = fc / res;
        vec2 dir = (PASSINDEX == 3) ? vec2(1.0 / res.x, 0.0) : vec2(0.0, 1.0 / res.y);
        vec3 acc = vec3(0.0); float ws = 0.0;
        for (int i = -7; i <= 7; i++) {
            float fi = float(i);
            float w = exp(-fi * fi / 18.0);
            vec3 c;
            if (PASSINDEX == 3) {
                c = texture2D(sceneBuf, uvb + dir * fi * 1.5).rgb;
                float l0 = max(lum(c), 1e-4);
                c *= max(l0 - 0.9, 0.0) / l0;           // luminance threshold, hue kept
            } else {
                c = texture2D(bloomH, uvb + dir * fi * 1.5).rgb;
            }
            acc += c * w; ws += w;
        }
        gl_FragColor = vec4(acc / ws, 1.0);
        return;
    }

    // ── final: bloom, ACES, HD finisher ──────────────────────────────────
    vec2 uv = fc / res;
    vec4 sc = texture2D(sceneBuf, uv);
    vec3 col = sc.rgb;
    vec3 bl = texture2D(bloomV, uv).rgb;
    col += mix(bl, vec3(lum(bl)), 0.35) * bloom * 1.4;

    float ex = exposure * mix(1.0, 1.0 / max(g_vis, 0.2), 0.3);
    col = aces(col * ex * 0.95);
    float l = lum(col);
    col = mix(vec3(l), col, saturation);

    vec2 vq = uv - 0.5;
    col *= 1.0 - 0.12 * dot(vq, vq) * 2.0;

    col = clamp(col, 0.0, 1.0);
    col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
    l = dot(col, vec3(0.299, 0.587, 0.114));
    col = clamp(mix(vec3(l), col, 1.22), 0.0, 1.0);

    float gn = hash12(fc + fract(TIME * 7.13) * 91.0) - 0.5;
    col += gn * grain * 0.05;

    float alpha = transparentBg ? clamp(sc.a, 0.0, 1.0) : 1.0;
    gl_FragColor = vec4(clamp(col, 0.0, 1.0), alpha);
}
