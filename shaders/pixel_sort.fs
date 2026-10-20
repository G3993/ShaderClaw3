/*{
  "DESCRIPTION": "Pixel Sort — a real sorting algorithm running live on the image. Inside every luminance interval the pixels are compared and swapped by an odd-even transposition sorting network — eight compare-swap steps every frame — so you watch the picture physically sort itself into molten streaks: chrome highlights slide down their columns, reflections stretch into rainbow drips, and whenever the source moves the sort starts over and chases it. A wandering re-sort wave keeps snapping columns back to the live image so the sorting never finishes; datamosh macroblocks drag old pixels along the motion and glitch slices tear the frame on beats. The internal source is an iridescent liquid-chrome sculpture over a black mirror floor lit by neon strip lights; bind any image or camera to Input and it sorts that instead. Bass feeds the datamosh and swells the sculpture, beats fire re-sort waves and RGB slice tears, the level opens the sort window, highs sparkle.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #8",
  "CATEGORIES": ["Generator", "Effect", "Glitch", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "inputTex",    "LABEL": "Input (optional)",  "TYPE": "image" },
    { "NAME": "speed",       "LABEL": "Scene Speed",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6,  "GROUP": "Speed" },
    { "NAME": "sortSpeed",   "LABEL": "Sort Steps / Frame","TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 8.0, "DEFAULT": 8.0, "GROUP": "Speed" },
    { "NAME": "waveRate",    "LABEL": "Re-sort Waves",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.6,  "GROUP": "Speed" },
    { "NAME": "sortAxis",    "LABEL": "Axis (down→right→up→left)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.0, "GROUP": "Sort" },
    { "NAME": "threshLo",    "LABEL": "Window Low",        "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.36, "GROUP": "Sort" },
    { "NAME": "threshHi",    "LABEL": "Window High",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.9, "GROUP": "Sort" },
    { "NAME": "threshVar",   "LABEL": "Window Drift",      "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.75, "GROUP": "Sort" },
    { "NAME": "sortKey",     "LABEL": "Key (luma→hue)",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0,  "GROUP": "Sort" },
    { "NAME": "streakLen",   "LABEL": "Streak Length",     "TYPE": "float", "AUDIO": false, "MIN": 0.05, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Sort" },
    { "NAME": "dripLen",     "LABEL": "Drip Length",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.2, "GROUP": "Sort" },
    { "NAME": "dripMin",     "LABEL": "Drip From Brightness", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Sort" },
    { "NAME": "flipBands",   "LABEL": "Reverse Bands",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.18, "GROUP": "Sort" },
    { "NAME": "melt",        "LABEL": "Melt (refresh)",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Sort" },
    { "NAME": "mosh",        "LABEL": "Datamosh",          "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Glitch" },
    { "NAME": "tear",        "LABEL": "Slice Tear",        "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.4,  "GROUP": "Glitch" },
    { "NAME": "aberration",  "LABEL": "RGB Split",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25, "GROUP": "Glitch" },
    { "NAME": "light1",      "LABEL": "Strip Light 1",     "TYPE": "color", "DEFAULT": [1.0, 0.12, 0.55, 1.0], "GROUP": "Color" },
    { "NAME": "light2",      "LABEL": "Strip Light 2",     "TYPE": "color", "DEFAULT": [0.1, 0.75, 1.0, 1.0],  "GROUP": "Color" },
    { "NAME": "light3",      "LABEL": "Strip Light 3",     "TYPE": "color", "DEFAULT": [1.0, 0.62, 0.12, 1.0], "GROUP": "Color" },
    { "NAME": "backColor",   "LABEL": "Backdrop",          "TYPE": "color", "DEFAULT": [0.02, 0.01, 0.05, 1.0], "GROUP": "Color" },
    { "NAME": "iridescence", "LABEL": "Iridescence",       "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 1.0, "GROUP": "Color" },
    { "NAME": "exposure",    "LABEL": "Exposure",          "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 2.5, "DEFAULT": 0.9, "GROUP": "Light" },
    { "NAME": "bloom",       "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.55, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15, "GROUP": "Finish" },
    { "NAME": "simRes",      "LABEL": "Sort Resolution",   "TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 1.0, "DEFAULT": 1.0, "GROUP": "Finish" },
    { "NAME": "marchRes",    "LABEL": "Scene Resolution",  "TYPE": "float", "AUDIO": false, "MIN": 0.3, "MAX": 1.0, "DEFAULT": 0.75, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "srcBuf", "WIDTH": "min(floor($WIDTH*$marchRes),2304)", "HEIGHT": "min(floor($HEIGHT*$marchRes),1296)" },
    { "TARGET": "dil1", "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "dil2", "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "maskBuf", "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "sortA", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "sortB", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "sortC", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "sortD", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "sortE", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "sortF", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "sortG", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { "TARGET": "sortH", "PERSISTENT": true, "WIDTH": "min(floor($WIDTH*$simRes),2304)", "HEIGHT": "min(floor($HEIGHT*$simRes),1296)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Pixel Sort — FRONTIER pack #8
//   pass 0     srcBuf : the source — liquid-chrome sculpture over a black
//              mirror floor (raymarched, ACES'd so keys live in 0..1),
//              or the bound Input image
//   pass 1     sortA  : inject (melt toward source, datamosh macroblocks,
//              re-sort waves, reset outside the window) + sort step 0
//   pass 2-8   sortB..sortH : odd-even transposition steps 1..7
//              (pixel i pairs with i±1 by parity of i + step; both read the
//              same pair so every swap is a true permutation)
//   pass 9     final : slice tears, RGB split, bloom, grain, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vnoise2(vec2 x) {
    vec2 p = floor(x), f = fract(x); f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash12(p), hash12(p + vec2(1.0, 0.0)), f.x), mix(hash12(p + vec2(0.0, 1.0)), hash12(p + vec2(1.0, 1.0)), f.x), f.y);
}
float lum(vec3 c) { return dot(c, vec3(0.299, 0.587, 0.114)); }
vec3 aces(vec3 x) { const float a = 2.51, b = 0.03, c = 2.43, d = 0.59, e = 0.14; return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0); }
vec3 hueRot(vec3 c, float h) {
    float a = h * TAU, s = sin(a), co = cos(a);
    const vec3 k = vec3(0.57735);
    return c * co + cross(k, c) * s + k * dot(k, c) * (1.0 - co);
}

float g_t, g_ar, g_bass, g_mid, g_high, g_beat, g_lev;

void audioSetup() {
    g_ar   = audioReact;
    g_bass = pow(smoothstep(0.05, 0.85, audioBass), 1.4) * g_ar;
    g_mid  = pow(smoothstep(0.06, 0.85, audioMid),  1.2) * g_ar;
    g_high = pow(smoothstep(0.10, 0.90, audioHigh), 1.2) * g_ar;
    g_beat = clamp(audioBeatPulse, 0.0, 1.0) * g_ar;
    g_lev  = clamp(audioLevel, 0.0, 1.0) * g_ar;
    g_t    = TIME * speed;
}

// ═════════════════════════════════════════════════════════════════════════
//  SOURCE  (pass 0) — liquid chrome
// ═════════════════════════════════════════════════════════════════════════
float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }

vec3 blobPos(float i) {
    float t = g_t * 0.55 + i * 1.7;
    return vec3(sin(t * 0.83 + i) * 0.85, sin(t * 0.61 + i * 2.3) * 0.55 + 0.1, cos(t * 0.71 + i * 1.1) * 0.6);
}

float sdScene(vec3 p) {
    float bnd = length(p - vec3(0.0, 0.1, 0.0)) - 2.0;
    if (bnd > 0.3) return bnd;
    float d = 1e5;
    float sw = 1.0 + 0.12 * g_bass;
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        float r = (0.34 + 0.09 * sin(g_t * 0.9 + fi * 2.1)) * sw;
        d = smin(d, length(p - blobPos(fi)) - r, 0.42);
    }
    // a slow ring threading through the blobs
    vec3 q = p - vec3(0.0, 0.1, 0.0);
    float a = g_t * 0.3;
    q.xy = mat2(cos(a), -sin(a), sin(a), cos(a)) * q.xy;
    q.yz = mat2(cos(a * 0.7), -sin(a * 0.7), sin(a * 0.7), cos(a * 0.7)) * q.yz;
    float tor = length(vec2(length(q.xz) - 1.05, q.y)) - 0.075;
    d = smin(d, tor, 0.25);
    // liquid ripples
    d += 0.018 * sin(p.x * 9.0 + g_t * 2.0) * sin(p.y * 8.0 - g_t * 1.6) * sin(p.z * 10.0 + g_t);
    return d * 0.85;
}

vec3 calcNormal(vec3 p) {
    const vec2 k = vec2(1.0, -1.0);
    float e = 0.0015;
    return normalize(k.xyy * sdScene(p + k.xyy * e) + k.yyx * sdScene(p + k.yyx * e) +
                     k.yxy * sdScene(p + k.yxy * e) + k.xxx * sdScene(p + k.xxx * e));
}

float march(vec3 ro, vec3 rd, float far, int steps) {
    float t = 0.0;
    for (int i = 0; i < 80; i++) {
        if (i >= steps) break;
        float d = sdScene(ro + rd * t);
        if (d < 0.0012 * (1.0 + t)) return t;
        t += d;
        if (t > far) break;
    }
    return -1.0;
}

// studio: three vertical neon strips, a top softbox, a horizon line
vec3 env(vec3 d) {
    float az = atan(d.z, d.x);
    float sp = g_t * 0.12;
    vec3 L1 = hueRot(light1.rgb, hueShift), L2 = hueRot(light2.rgb, hueShift), L3 = hueRot(light3.rgb, hueShift);
    // sunset dome: hot amber horizon -> magenta -> deep indigo zenith
    float y = d.y;
    vec3 c = backColor.rgb;
    c = mix(L3 * 1.1, L1 * 0.55, smoothstep(-0.05, 0.25, y));
    c = mix(c, backColor.rgb * 2.0 + L2 * 0.05, smoothstep(0.2, 0.75, y));
    c = mix(backColor.rgb, c, smoothstep(-0.35, -0.02, y));
    // drifting cloud banks give the reflections texture
    float cl = vnoise2(vec2(az * 3.0 + sp * 2.0, y * 9.0)) * vnoise2(vec2(az * 7.0 - sp, y * 21.0) + 4.0);
    c *= 0.55 + 1.1 * cl * smoothstep(-0.05, 0.4, y);
    float h = smoothstep(-0.25, -0.05, d.y) * smoothstep(0.95, 0.6, d.y);
    float a1 = abs(mod(az - sp + PI, TAU) - PI);
    float a2 = abs(mod(az - sp - 2.1 + PI, TAU) - PI);
    float a3 = abs(mod(az - sp + 2.2 + PI, TAU) - PI);
    c += L1 * 6.0 * exp(-pow(a1 * 9.0, 2.0)) * h;
    c += L2 * 6.0 * exp(-pow(a2 * 11.0, 2.0)) * h;
    c += L3 * 5.0 * exp(-pow(a3 * 14.0, 2.0)) * h;
    // thin hard rims on the strips (crisp edge highlights in the chrome)
    c += (L1 + vec3(0.6)) * 3.0 * exp(-pow(a1 * 40.0, 2.0)) * h;
    c += (L2 + vec3(0.6)) * 3.0 * exp(-pow(a2 * 46.0, 2.0)) * h;
    c += vec3(2.2) * smoothstep(0.78, 0.86, d.y);           // softbox
    c += mix(L2, L1, 0.5 + 0.5 * sin(az * 2.0 + sp)) * 1.6 * exp(-abs(d.y + 0.02) * 60.0); // horizon neon
    return c;
}

vec3 filmTint(float cosT, float thick) {
    // cheap thin-film interference: phase per wavelength
    float opd = thick * cosT;
    return 0.5 + 0.5 * cos(TAU * (opd / vec3(0.65, 0.53, 0.44)));
}

vec3 shadeChrome(vec3 p, vec3 rd, vec3 n) {
    vec3 rf = reflect(rd, n);
    float ct = clamp(dot(-rd, n), 0.0, 1.0);
    float fr = 0.62 + 0.38 * pow(1.0 - ct, 5.0);
    float thick = 0.9 + 1.4 * vnoise2(p.xy * 2.3 + p.z + g_t * 0.2) + 0.4 * sin(p.y * 6.0 + g_t);
    vec3 tint = mix(vec3(1.0), filmTint(ct, thick * 1.4), 0.55 * iridescence);
    // the lower hemisphere sees the black mirror floor: a dimmed mirrored dome
    vec3 er = rf.y < 0.0 ? env(vec3(rf.x, -rf.y, rf.z)) * 0.6 : env(rf);
    vec3 c = er * tint * fr;
    // floor bounce light from below
    c += backColor.rgb * 0.4 * clamp(-n.y, 0.0, 1.0);
    // pearlescent coat: a soft iridescent body colour so the chrome never goes murky
    c += filmTint(ct, thick * 0.9 + 0.6) * mix(hueRot(light1.rgb, hueShift), hueRot(light2.rgb, hueShift), 0.5 + 0.5 * n.y) * 0.22 * iridescence;
    return c;
}

// the stage wall seen directly: near-black, a neon halo ring behind the
// sculpture, a dashed inner ring and a scatter of LED pins — sharp bright
// features on black that melt into long drips when sorted
vec2 g_ringC;
float g_subj;   // 1 = sculpture (direct or mirrored), 0 = stage
float g_yh;     // screen y of the horizon

// the stage: a full-frame neon sunset — gradient sky with aurora curtains,
// a striped synthwave sun behind the sculpture (rimmed by the halo ring), a
// black city skyline with lit windows on a glowing horizon tube. Wide
// luminous gradients everywhere so every column has something to sort;
// blacks only as accents (skyline, sun stripes)
vec3 backdrop(vec2 uv) {
    vec3 L1 = hueRot(light1.rgb, hueShift), L2 = hueRot(light2.rgb, hueShift), L3 = hueRot(light3.rgb, hueShift);
    float h = uv.y - g_yh;                       // height above the horizon
    // sky gradient: hot amber horizon → magenta → violet → deep indigo
    vec3 c = mix(L3 * 1.25 + vec3(0.15, 0.05, 0.0), L1 * 0.95, smoothstep(0.0, 0.2, h));
    c = mix(c, mix(L1, L2, 0.45) * 0.42, smoothstep(0.16, 0.5, h));
    c = mix(c, backColor.rgb * 1.5, smoothstep(0.45, 0.95, h));
    // horizontal cloud bands (vary brightness ACROSS x so sorting rearranges them)
    float cb = vnoise2(vec2(uv.x * 2.2 + g_t * 0.05, h * 16.0)) * vnoise2(vec2(uv.x * 5.0 - g_t * 0.03, h * 40.0) + 3.0);
    c *= 0.6 + 0.9 * cb * smoothstep(0.02, 0.12, h) * smoothstep(0.7, 0.3, h);
    // aurora curtains high in the sky
    float ax = uv.x * 2.6 + 0.6 * vnoise2(vec2(uv.x * 1.3, g_t * 0.08)) + g_t * 0.04;
    float curtain = pow(0.5 + 0.5 * sin(ax * 3.1 + 2.0 * vnoise2(vec2(ax, h * 2.0 + g_t * 0.1))), 3.0);
    float aBand = smoothstep(0.32, 0.55, h) * smoothstep(1.05, 0.6, h);
    float rays = 0.55 + 0.45 * vnoise2(vec2(uv.x * 60.0, h * 3.0 - g_t * 0.3));
    c += mix(L2, vec3(0.2, 1.0, 0.55), 0.35) * 0.9 * curtain * aBand * rays * smoothstep(0.32, 0.42, h + 0.08 * curtain);

    // striped synthwave sun behind the sculpture
    vec2 q = uv - g_ringC;
    float r = length(q), a = atan(q.y, q.x);
    float R0 = 0.44 + 0.012 * g_bass;
    float sunT = clamp(q.y / R0 * 0.5 + 0.5, 0.0, 1.0);
    vec3 sunC = mix(L1 * 1.4, L3 * 1.6 + vec3(0.35, 0.25, 0.0), sunT);
    float disc = smoothstep(R0 + 0.002, R0 - 0.002, r);
    float yy = (q.y + R0) / R0;                  // 0 at the bottom of the sun
    float stripeW = smoothstep(1.1, 0.0, yy) * 0.55;
    float stripe = step(stripeW, fract(yy * 7.0 - g_t * 0.15));
    c = mix(c, sunC, disc * mix(1.0, stripe, step(yy, 1.1)));
    // halo ring on the sun's rim
    float hue = 0.5 + 0.5 * sin(a + g_t * 0.4);
    vec3 rc = mix(mix(L1, L2, hue), L3, 0.5 + 0.5 * sin(a * 2.0 - g_t * 0.3));
    float dr = abs(r - (R0 + 0.02));
    c += rc * (2.6 * exp(-pow(dr / 0.0045, 2.0)) + 0.45 * exp(-dr * 30.0));
    c += vec3(1.4) * exp(-pow(dr / 0.0016, 2.0));

    // black skyline with lit windows
    float bw = 0.035;
    float bi = floor(uv.x / bw);
    float bh = (0.015 + 0.11 * pow(hash11(bi * 1.37 + 4.0), 2.2)) * step(0.18, hash11(bi * 2.1));
    bh += 0.03 * step(0.9, hash11(bi * 5.3)) * step(abs(fract(uv.x / bw) - 0.5), 0.12);  // spires
    if (h > 0.0 && h < bh) {
        vec3 bc = backColor.rgb * 0.35;
        vec2 wg = vec2(fract(uv.x / bw * 5.0), fract(h * 110.0));
        vec2 wi = vec2(floor(uv.x / bw * 5.0), floor(h * 110.0));
        float lit = step(0.62, hash12(wi + bi * 13.0)) * step(abs(wg.x - 0.5), 0.26) * step(abs(wg.y - 0.5), 0.22);
        lit *= step(0.006, bh - h);
        vec3 wc = hash12(wi * 1.7) > 0.5 ? L3 * 1.3 : L2 * 1.1;
        c = bc + wc * lit * (0.75 + 0.25 * sin(g_t * 0.7 + hash12(wi) * 30.0));
    }

    // neon tube along the horizon, full width
    float dy = uv.y - (g_yh + 0.004);
    vec3 hc = mix(L2, L1, smoothstep(-0.9, 0.9, uv.x + 0.25 * sin(g_t * 0.2)));
    c += hc * (2.4 * exp(-pow(dy / 0.0028, 2.0)) + 0.3 * exp(-abs(dy) * 60.0));
    c += vec3(1.1) * exp(-pow(dy / 0.0011, 2.0));
    return c;
}

// neon synthwave grid on the black mirror floor
vec3 floorGrid(vec3 p, float tf) {
    vec3 L1 = hueRot(light1.rgb, hueShift), L2 = hueRot(light2.rgb, hueShift);
    vec2 gp = p.xz * vec2(1.6, 1.6) + vec2(0.0, g_t * 0.6);
    vec2 gd = abs(fract(gp) - 0.5);
    float w = 0.012 + 0.012 * tf;                 // widen with distance so it never aliases
    float line = max(smoothstep(0.5 - w, 0.5, gd.x), smoothstep(0.5 - w, 0.5, gd.y));
    float fade = exp(-0.035 * tf * tf * 0.25);
    vec3 gc = mix(L1, L2, 0.5 + 0.5 * sin(p.x * 0.5 + g_t * 0.3));
    return gc * line * 1.6 * fade;
}

vec3 sourceScene(vec2 fc, vec2 res) {
    vec2 uv = (fc - 0.5 * res) / res.y;
    float ca = 0.35 * sin(g_t * 0.11) + 0.25;
    vec3 ro = vec3(sin(ca) * 4.7, -0.3 + 0.12 * sin(g_t * 0.07), cos(ca) * 4.7);
    vec3 ta = vec3(0.0, 0.32, 0.0);
    vec3 ww = normalize(ta - ro), uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0))), vv = cross(uu, ww);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.9 * ww);
    // halo centred on the sculpture's screen position
    vec3 cdir = normalize(vec3(0.0, 0.15, 0.0) - ro);
    g_ringC = vec2(dot(cdir, uu), dot(cdir, vv)) * 1.9 / dot(cdir, ww);

    g_subj = 0.0;
    g_yh = -1.9 * ww.y / vv.y;
    float floorY = -1.05;
    float t = march(ro, rd, 8.0, 72);
    vec3 col;
    if (t > 0.0) {
        vec3 p = ro + rd * t;
        col = shadeChrome(p, rd, calcNormal(p));
        g_subj = 1.0;
    } else {
        col = backdrop(uv);
        if (rd.y < 0.0) {
            float tf = (floorY - ro.y) / rd.y;
            vec3 p = ro + rd * tf;
            vec3 rr = reflect(rd, vec3(0.0, 1.0, 0.0));
            // glossy black mirror floor: reflect the sculpture
            float tr = march(p + rr * 0.02, rr, 5.0, 40);
            float fr = 0.62 + 0.35 * pow(1.0 - clamp(-rd.y, 0.0, 1.0), 5.0);
            vec3 refl;
            if (tr > 0.0) { vec3 q = p + rr * tr; refl = shadeChrome(q, rr, calcNormal(q)); fr = max(fr, 0.6); g_subj = 1.0; }
            else {
                refl = backdrop(vec2(uv.x, 2.0 * g_yh - uv.y)) * 0.85 + env(rr) * 0.06;
            }
            vec3 base = backColor.rgb * 0.3;
            float near = exp(-0.004 * tf * tf);
            col = mix(base, refl, fr) + floorGrid(p, tf) * near;
            // far floor fades to a pure mirror
            col = mix(refl * 0.85, col, near);
            // contact shadow pool under the sculpture
            col *= mix(0.25, 1.0, smoothstep(0.2, 1.6, length(p.xz)));
        }
    }
    col = aces(col * exposure * (0.92 + 0.22 * g_lev + 0.12 * g_bass));
    return col;
}

vec4 inputOrScene(vec2 fc, vec2 res) {
    if (IMG_SIZE_inputTex.x > 0.5) return vec4(texture2D(inputTex, fc / res).rgb, 1.0);
    vec3 c = sourceScene(fc, res);
    return vec4(c, g_subj);
}

// ═════════════════════════════════════════════════════════════════════════
//  SORT  (passes 1..8)
// ═════════════════════════════════════════════════════════════════════════
vec4 prevSort(vec2 uv) {
    if (PASSINDEX == 4) return texture2D(sortH, uv);
    if (PASSINDEX == 5) return texture2D(sortA, uv);
    if (PASSINDEX == 6) return texture2D(sortB, uv);
    if (PASSINDEX == 7) return texture2D(sortC, uv);
    if (PASSINDEX == 8) return texture2D(sortD, uv);
    if (PASSINDEX == 9) return texture2D(sortE, uv);
    if (PASSINDEX == 10) return texture2D(sortF, uv);
    return texture2D(sortG, uv);
}

float sortKeyOf(vec3 c) {
    float l = lum(c);
    float mx = max(c.r, max(c.g, c.b)), mn = min(c.r, min(c.g, c.b));
    float hue = 0.0;
    if (mx - mn > 1e-4) {
        if (mx == c.r)      hue = mod((c.g - c.b) / (mx - mn), 6.0);
        else if (mx == c.g) hue = (c.b - c.r) / (mx - mn) + 2.0;
        else                hue = (c.r - c.g) / (mx - mn) + 4.0;
        hue /= 6.0;
    }
    return mix(l, hue * 0.85 + l * 0.15, sortKey);
}

// axis frame: along = sort direction (index i), across = column
float g_axis;
vec2 alongDir() {
    if (g_axis < 0.5) return vec2(0.0, -1.0);   // down
    if (g_axis < 1.5) return vec2(1.0, 0.0);    // right
    if (g_axis < 2.5) return vec2(0.0, 1.0);    // up
    return vec2(-1.0, 0.0);                     // left
}

// the window's low edge drifts in slow noise across the frame: some regions
// melt fully, others stay crisp islands
float windowLoAt(vec2 fc) {
    vec2 q = fc / RENDERSIZE.y;
    float n = vnoise2(q * vec2(1.3, 0.9) + vec2(TIME * 0.035, -TIME * 0.02)) * 0.7 + 0.3 * vnoise2(q * 5.0 + 7.0);
    return threshLo - 0.10 * g_lev + (n - 0.5) * 0.9 * threshVar;
}
bool inWindowLo(float k, float lo) { return k >= lo && k <= threshHi; }
bool inWindow(float k, vec2 fc) { return inWindowLo(k, windowLoAt(fc)); }

// pass-2 injection, evaluated per CELL so a pixel can also evaluate its
// sort partner's injected value — both sides of a compare-swap must see the
// same pair or the network duplicates/loses pixels (1-px dashes)
vec4 injected(vec2 cell, float col, vec2 ad, vec2 cr) {
    vec2 fc = cell + 0.5;
    vec4 mk = texture2D(maskBuf, fc / R);
    vec3 src = mk.rgb;
    vec4 st = texture2D(sortH, fc / R);
    float tt = TIME;
    // datamosh macroblocks: drag last frame's pixels along a motion field
    vec2 blk = floor(fc / 24.0);
    float slot = floor(tt * (1.0 + 1.5 * g_bass));
    float sel = hash12(blk * 1.37 + slot * 7.13);
    float moshAmt = mosh * (0.03 + 0.45 * g_bass + 0.25 * g_beat);
    bool moshed = sel < moshAmt * 0.5 && FRAMEINDEX > 4;
    if (moshed) {
        vec2 mv = floor(vec2(vnoise2(blk * 0.21 + slot) - 0.5, vnoise2(blk * 0.21 + slot + 9.1) - 0.5) * 6.0 + 0.5);
        mv += ad * 2.0;
        st = texture2D(sortH, (fc - mv) / R);
    }
    // melt: the sorted state slowly re-absorbs the live source
    float refresh = 0.002 + 0.03 * melt * melt;
    // re-sort waves: bands sweep across the columns snapping them back
    float colN = col / dot(R, cr);
    float wp = fract(tt * 0.07 * waveRate + 0.13);
    float band = smoothstep(0.03, 0.0, abs(colN - wp));
    float wp2 = fract(-tt * 0.045 * waveRate + 0.61);
    band = max(band, smoothstep(0.018, 0.0, abs(colN - wp2)));
    float grp = floor(col / 24.0);
    float bslot = floor(tt * 2.0);
    band = max(band, step(hash11(grp * 3.1 + bslot), 0.18 * g_beat));
    refresh = max(refresh, band * min(waveRate * 2.0, 1.0));
    // stochastic per-pixel replacement (never a colour blend — blending sorted
    // and live colours greys everything out)
    // refresh whole short column segments so it reads as streaks, never salt
    float segI = floor((dot(cell, abs(ad)) + hash11(col) * 40.0) / 40.0);
    if (!moshed && hash12(vec2(col, segI) * 0.913 + fract(tt * 7.31) * 517.0) < refresh) st.rgb = src;
    // outside every run the image is the live source (clean, unsorted)
    if (mk.a < 0.5 && !moshed) st.rgb = src;
    if (FRAMEINDEX < 2) {
        st.rgb = src;
        // seed mid-melt so frame 0 already reads as sorting in progress:
        // each run pixel takes a run pixel from a random distance upstream
        // (a smear the network then finishes sorting)
        if (mk.a > 0.5) {
            float k = (6.0 + 70.0 * vnoise2(vec2(col / 9.0, 2.0))) * hash12(fc * 0.37);
            vec4 u = texture2D(maskBuf, (fc - ad * floor(k)) / R);
            if (u.a > 0.5) st.rgb = u.rgb;
        }
    }
    return st;
}

void main() {
    audioSetup();
    vec2 fc = gl_FragCoord.xy;

    if (PASSINDEX == 0) {
        gl_FragColor = inputOrScene(fc, R);
        return;
    }

    g_axis = floor(sortAxis + 0.5);
    vec2 ad = alongDir();
    vec2 cr = vec2(abs(ad.y), abs(ad.x));          // across axis
    vec2 cell = floor(fc);
    float N   = dot(R, abs(ad));
    float col = dot(cell, cr);

    // ── interval mask: a run is where the source sits inside the window, OR
    //    up to one drip length downstream of a bright window pixel, so highlights
    //    melt out into the dark. Exact dilation along the sort axis in three
    //    8-tap passes (1px → 8px → 64px strides), so thin rims never alias.
    if (PASSINDEX == 1) {
        // r = sculpture drip seeds, g = stage drip seeds (they drip shorter)
        vec2 m = vec2(0.0);
        float wlo = windowLoAt(fc);     // the drift is slow: one lookup covers all 8 taps
        for (int j = 0; j < 8; j++) {
            vec4 s2 = texture2D(srcBuf, (fc - ad * float(j)) / R);
            if (inWindowLo(sortKeyOf(s2.rgb), wlo) && lum(s2.rgb) > dripMin) {
                if (s2.a > 0.5) m.x = 1.0; else m.y = 1.0;
            }
        }
        gl_FragColor = vec4(m, 0.0, 1.0);
        return;
    }
    if (PASSINDEX == 2) {
        vec2 m = vec2(0.0);
        for (int j = 0; j < 8; j++) m = max(m, texture2D(dil1, (fc - ad * float(j) * 8.0) / R).rg);
        gl_FragColor = vec4(m, 0.0, 1.0);
        return;
    }
    if (PASSINDEX == 3) {
        vec2 uv = fc / R;
        vec3 src = texture2D(srcBuf, uv).rgb;
        float run = inWindow(sortKeyOf(src), fc) ? 1.0 : 0.0;
        // drip length varies in smooth ribbons across columns (+ a little per-column grit)
        float drip = mix(8.0, 560.0, dripLen) * mix(0.2, 1.7, vnoise2(vec2(col / 11.0, 3.7)) * 0.85 + 0.15 * hash11(col * 0.377 + 1.0));
        drip *= (1.0 + 0.5 * g_bass) * N / 1080.0;
        // dil2 covers [p-63, p]; step by ≤64 px so the union is gap-free
        float stp  = min(64.0, max(drip - 63.0, 0.0) / 7.0);
        // stage drips: shorter, and far more ragged so a long neon line never
        // melts into a hard-edged band
        float vn = vnoise2(vec2(col / 6.0, 5.3)) * 0.7 + 0.3 * vnoise2(vec2(col / 1.7, 8.1));
        float drip2 = mix(8.0, 560.0, dripLen) * 0.45 * (0.05 + 2.6 * vn * vn * vn) * (1.0 + 0.5 * g_bass) * N / 1080.0;
        float stp2 = min(64.0, max(drip2 - 63.0, 0.0) / 7.0);
        if (drip2 < 63.0) stp2 = 0.0;
        float m = texture2D(dil2, uv).r;
        m = max(m, drip2 >= 63.0 ? texture2D(dil2, uv).g : texture2D(dil1, uv).g);
        for (int j = 1; j < 8; j++) {
            m = max(m, texture2D(dil2, (fc - ad * float(j) * stp) / R).r);
            if (drip2 >= 63.0) m = max(m, texture2D(dil2, (fc - ad * float(j) * stp2) / R).g);
            // short stage drips: 8-px dil1 steps up to drip2
            if (float(j) * 8.0 < drip2) m = max(m, texture2D(dil1, (fc - ad * float(j) * 8.0) / R).g);
        }
        run = max(run, m);
        gl_FragColor = vec4(src, run);
        return;
    }

    if (PASSINDEX >= 4 && PASSINDEX <= 11) {
        // index grows along the sort direction
        float idx  = dot(cell, abs(ad));
        if (dot(ad, vec2(1.0)) < 0.0) idx = N - 1.0 - idx;
        vec2 uv    = fc / R;
        vec4 mk    = texture2D(maskBuf, uv);
        vec3 src   = mk.rgb;

        vec4 st = prevSort(uv);
        float k = float(PASSINDEX - 4);

        if (PASSINDEX == 4) st = injected(cell, col, ad, cr);

        // ── odd-even transposition step
        if (k < floor(sortSpeed + 0.5)) {
            float stepN = float(FRAMEINDEX) * 8.0 + k;
            float par = mod(idx + stepN, 2.0);
            float pIdx = par < 0.5 ? idx + 1.0 : idx - 1.0;
            if (pIdx >= 0.0 && pIdx <= N - 1.0) {
                float lo = min(idx, pIdx);
                vec2 pc = cell + ad * (pIdx - idx);
                vec2 puv = (pc + 0.5) / R;
                vec4 ps = PASSINDEX == 4 ? injected(pc, col, ad, cr) : prevSort(puv);
                float pm = texture2D(maskBuf, puv).a;
                float L = mix(30.0, N * 1.2, streakLen) * mix(0.35, 1.65, vnoise2(vec2(col / 17.0, 9.1)));
                float off = (vnoise2(vec2(col / 23.0, 1.3)) * 2.0 + 0.08 * hash11(col * 1.913 + 7.0)) * L;
                bool brk = floor((lo + off) / L) != floor((lo + 1.0 + off) / L);
                if (mk.a > 0.5 && pm > 0.5 && !brk) {
                    float bandId = floor(col / mix(18.0, 90.0, hash11(floor(col / 60.0) + 0.5)));
                    float flip = step(hash11(bandId * 2.71 + floor(TIME * 0.05 + hash11(bandId))), flipBands);
                    float ka = sortKeyOf(st.rgb), kb = sortKeyOf(ps.rgb);
                    bool selfLow = idx < pIdx;
                    bool wantHigh = selfLow ? (flip > 0.5) : (flip < 0.5);
                    bool selfIsHigh = ka > kb || (ka == kb && selfLow);
                    if (wantHigh != selfIsHigh) st = ps;
                }
            }
        }
        st.a = 1.0;
        gl_FragColor = st;
        return;
    }

    // ═════════════════════════════════════════════════════════════════════
    //  FINAL (pass 9)
    // ═════════════════════════════════════════════════════════════════════
    vec2 uv = fc / R;
    float tt = TIME;
    // slice tears: horizontal slabs jump sideways on beats, eased
    float slab = floor(uv.y * 60.0 + floor(tt * 3.0) * 7.0);
    float tearOn = step(hash11(slab * 1.7 + floor(tt * 6.0)), tear * 0.22 * smoothstep(0.45, 0.9, g_beat));
    float sh = (hash11(slab * 3.3 + floor(tt * 6.0)) - 0.5) * 0.08 * tearOn;
    vec2 suv = vec2(uv.x + sh, uv.y);
    float ab = (0.0012 + 0.006 * aberration * (0.3 + g_beat + tearOn)) * aberration;
    vec3 c;
    // split along the sort axis so it fringes streak heads, not every column;
    // torn slabs split sideways
    vec2 abd = mix(alongDir() * vec2(R.y / R.x, 1.0), vec2(1.0, 0.0), tearOn) * ab;
    // along-axis reconstruction filter. Odd-even transposition leaves 1-px a/b
    // interleave where opposing streams cross and 8-px "trains" (8 steps a
    // frame) where a moving run boundary injects a pixel per frame; a tent
    // along the sort axis only — 9 taps on sorted pixels, 3 on untouched
    // ones — dissolves both while every streak edge across the axis stays
    // pixel-sharp
    vec2 sa = alongDir() / R;
    vec3 srcHere = texture2D(srcBuf, suv).rgb;
    float sorted = smoothstep(0.03, 0.12, length(texture2D(sortH, suv).rgb - srcHere));
    float wl = mix(1.0, 4.0, sorted);
    vec3 acc = vec3(0.0);
    float wsum = 0.0;
    for (int i = -4; i <= 4; i++) {
        float fi = float(i);
        float w = max(0.0, 1.0 - abs(fi) / (wl + 0.5));
        if (w <= 0.0) continue;
        vec2 o = sa * fi;
        acc.r += w * texture2D(sortH, suv + abd + o).r;
        acc.g += w * texture2D(sortH, suv + o).g;
        acc.b += w * texture2D(sortH, suv - abd + o).b;
        wsum += w;
    }
    c = acc / wsum;
    // torn slabs get a hard channel invert sliver
    c = mix(c, c.gbr, tearOn * step(0.85, hash11(slab + 0.37)) * step(abs(uv.x - hash11(slab + 0.9)), 0.18));

    // bloom from the sorted image (rotated taps, thresholded)
    vec3 bl = vec3(0.0);
    float ang = 0.7;
    for (int i = 0; i < 12; i++) {
        float fi = float(i);
        float a = ang + fi * 2.39996;
        float r = (0.004 + 0.022 * fract(fi * 0.618)) ;
        vec3 s = texture2D(sortH, suv + vec2(cos(a), sin(a)) * r * vec2(R.y / R.x, 1.0)).rgb;
        bl += max(s - 0.6, 0.0);
    }
    c += bl / 12.0 * bloom * (1.4 + 0.6 * g_lev);

    // highs: sparse sparkle on the brightest sorted pixels
    float sp = step(0.996 - 0.01 * g_high, hash12(floor(fc / 2.0) + floor(tt * 20.0)));
    c += sp * smoothstep(0.7, 0.95, lum(c)) * 0.6;

    c = clamp(c, 0.0, 1.0);
    float l0 = lum(c);
    c = mix(vec3(l0), c, saturation);
    c += (hash12(fc + fract(tt) * 100.0) - 0.5) * 0.06 * grain;
    // HD finisher
    c = clamp(c, 0.0, 1.0);
    c = mix(c, c * c * (3.0 - 2.0 * c), 0.55);
    float l = dot(c, vec3(0.299, 0.587, 0.114));
    c = clamp(mix(vec3(l), c, 1.22), 0.0, 1.0);
    float alpha = transparentBg ? clamp(l * 2.0, 0.0, 1.0) : 1.0;
    gl_FragColor = vec4(c, alpha);
}
