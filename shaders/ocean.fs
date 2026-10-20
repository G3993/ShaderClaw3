/*{
  "DESCRIPTION": "Ocean — a real spectral sea living through a slow weather arc. Dozens of Gerstner waves drawn from a wind-sea spectrum (long swells down to ripples, each at its true deep-water speed) plus capillary ripple octaves pile into sharp-crested water that a low camera rides between the swells, pitching and rolling with the sea. Every few minutes the light turns: a golden sunset with a crisp limb-darkened sun and a glitter path of thousands of micro-facet sparkles, then deep blue hour with an indigo sky, first stars and glowing teal crests, then a storm of bruised slate clouds, lightning and building whitecaps, then gold again. Thin crests glow with backlit subsurface light, the surface mirrors volumetric-looking clouds with lit undersides and silver linings, distance filters small waves into physically-based roughness so the horizon stays clean and the foreground razor sharp, and lacy foam with bubbles trails behind each breaking crest while spray lifts into the wind. Bass builds the sea state, beats flash lightning, level brightens glitter and foam, highs sparkle the ripples.",
  "CREDIT": "Easel / ShaderClaw house shader, 2026-10-17 — original. FRONTIER pack #10",
  "CATEGORIES": ["Generator", "3D", "Nature", "Water", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Wave Speed",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "travel",      "LABEL": "Camera Travel",     "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "cloudSpeed",  "LABEL": "Cloud Drift",       "TYPE": "float", "MIN": 0.0,  "MAX": 3.0,  "DEFAULT": 1.0,  "GROUP": "Speed" },
    { "NAME": "weatherCycle","LABEL": "Weather Cycle",     "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 4.0, "DEFAULT": 1.0, "GROUP": "Speed" },
    { "NAME": "seaState",    "LABEL": "Sea State",         "TYPE": "float", "MIN": 0.2,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Sea" },
    { "NAME": "choppy",      "LABEL": "Choppiness",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Sea" },
    { "NAME": "waveScale",   "LABEL": "Wave Scale",        "TYPE": "float", "AUDIO": false, "MIN": 0.4, "MAX": 2.5, "DEFAULT": 1.0, "GROUP": "Sea" },
    { "NAME": "windDir",     "LABEL": "Wind Direction",    "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 360.0, "DEFAULT": 70.0, "GROUP": "Sea" },
    { "NAME": "spread",      "LABEL": "Wave Spread",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0, "GROUP": "Sea" },
    { "NAME": "foamAmt",     "LABEL": "Whitecaps",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Sea" },
    { "NAME": "camHeight",   "LABEL": "Camera Height",     "TYPE": "float", "AUDIO": false, "MIN": 1.0, "MAX": 20.0, "DEFAULT": 3.0, "GROUP": "Camera" },
    { "NAME": "camPitch",    "LABEL": "Camera Pitch",      "TYPE": "float", "AUDIO": false, "MIN": -0.4, "MAX": 0.3, "DEFAULT": -0.02, "GROUP": "Camera" },
    { "NAME": "camYaw",      "LABEL": "Camera Heading",    "TYPE": "float", "AUDIO": false, "MIN": -180.0, "MAX": 180.0, "DEFAULT": 0.0, "GROUP": "Camera" },
    { "NAME": "sunHeight",   "LABEL": "Sun Height",        "TYPE": "float", "AUDIO": false, "MIN": -2.0, "MAX": 40.0, "DEFAULT": 4.0, "GROUP": "Sky" },
    { "NAME": "sunAz",       "LABEL": "Sun Direction",     "TYPE": "float", "AUDIO": false, "MIN": -90.0, "MAX": 90.0, "DEFAULT": 12.0, "GROUP": "Sky" },
    { "NAME": "cloudCover",  "LABEL": "Cloud Cover",       "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.62, "GROUP": "Sky" },
    { "NAME": "weather",     "LABEL": "Weather (sunset→blue hour→storm)", "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.0, "GROUP": "Sky" },
    { "NAME": "stars",       "LABEL": "Stars",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Sky" },
    { "NAME": "moonAmt",     "LABEL": "Moon",              "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Sky" },
    { "NAME": "godRays",     "LABEL": "God Rays",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Sky" },
    { "NAME": "lightning",   "LABEL": "Lightning",         "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Sky" },
    { "NAME": "deepColor",   "LABEL": "Deep Water",        "TYPE": "color", "DEFAULT": [0.004, 0.03, 0.055, 1.0], "GROUP": "Color" },
    { "NAME": "sssColor",    "LABEL": "Crest Glow",        "TYPE": "color", "DEFAULT": [0.02, 0.62, 0.52, 1.0],  "GROUP": "Color" },
    { "NAME": "skyTop",      "LABEL": "Sky Zenith",        "TYPE": "color", "DEFAULT": [0.018, 0.035, 0.1, 1.0], "GROUP": "Color" },
    { "NAME": "skyHorizon",  "LABEL": "Sky Horizon",       "TYPE": "color", "DEFAULT": [0.62, 0.2, 0.13, 1.0],  "GROUP": "Color" },
    { "NAME": "sunColor",    "LABEL": "Sun Color",         "TYPE": "color", "DEFAULT": [1.0, 0.62, 0.32, 1.0],   "GROUP": "Color" },
    { "NAME": "blueTop",     "LABEL": "Blue Hour Zenith",  "TYPE": "color", "DEFAULT": [0.012, 0.026, 0.085, 1.0], "GROUP": "Color" },
    { "NAME": "blueHorizon", "LABEL": "Blue Hour Horizon", "TYPE": "color", "DEFAULT": [0.2, 0.22, 0.4, 1.0],  "GROUP": "Color" },
    { "NAME": "stormTop",    "LABEL": "Storm Zenith",      "TYPE": "color", "DEFAULT": [0.018, 0.04, 0.045, 1.0], "GROUP": "Color" },
    { "NAME": "stormHorizon","LABEL": "Storm Horizon",     "TYPE": "color", "DEFAULT": [0.1, 0.17, 0.16, 1.0],  "GROUP": "Color" },
    { "NAME": "flashColor",  "LABEL": "Lightning Color",   "TYPE": "color", "DEFAULT": [0.82, 0.74, 1.0, 1.0],    "GROUP": "Color" },
    { "NAME": "hueShift",    "LABEL": "Hue Shift",         "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "saturation",  "LABEL": "Saturation",        "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "sunIntensity","LABEL": "Sun Intensity",     "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "sssAmt",      "LABEL": "Subsurface",        "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "glitter",     "LABEL": "Sun Glitter",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "fog",         "LABEL": "Sea Haze",          "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Light" },
    { "NAME": "spray",       "LABEL": "Spray",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0,  "GROUP": "Finish" },
    { "NAME": "bloom",       "LABEL": "Bloom",             "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.8,  "GROUP": "Finish" },
    { "NAME": "aberration",  "LABEL": "Chromatic Fringe",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.04, "GROUP": "Finish" },
    { "NAME": "grain",       "LABEL": "Grain",             "TYPE": "float", "AUDIO": false, "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.18, "GROUP": "Finish" },
    { "NAME": "marchRes",    "LABEL": "March Resolution",  "TYPE": "float", "AUDIO": false, "MIN": 0.25, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Finish" },
    { "NAME": "audioReact",  "LABEL": "Audio React",       "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.6,  "GROUP": "Audio Reactivity" },
    { "NAME": "transparentBg", "LABEL": "Transparent BG",  "TYPE": "bool",  "DEFAULT": false, "GROUP": "Color" }
  ],
  "PASSES": [
    { "TARGET": "ocMarch", "WIDTH": "floor($WIDTH*$marchRes)", "HEIGHT": "floor($HEIGHT*$marchRes)" },
    { "TARGET": "ocEnv", "WIDTH": "$WIDTH/6", "HEIGHT": "$HEIGHT/6" },
    { "TARGET": "ocSky", "WIDTH": "$WIDTH/2", "HEIGHT": "$HEIGHT/2" },
    { "TARGET": "ocScene" },
    { "TARGET": "ocBloom", "WIDTH": "floor($WIDTH/4.0)", "HEIGHT": "floor($HEIGHT/4.0)" },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// Ocean — FRONTIER pack #10
//   pass 0  ocScene : spectral Gerstner sea (16 waves for geometry, 40 for
//                     shading with per-wave footprint filtering), heightfield
//                     march + bisection, analytic normals + Jacobian (foam),
//                     sky with lit storm clouds + lightning, fresnel / GGX sun /
//                     subsurface crests / foam memory lace / aerial haze.
//                     a = 0 sky, 0.5..1 water (+ crest foam → spray source)
//   pass 1  final   : spray mist lifted off the crests, bloom, fringe,
//                     ACES, grain, HD finisher
// ─────────────────────────────────────────────────────────────────────────

#define R    RENDERSIZE.xy
#define TAU  6.28318530718
#define PI   3.14159265359
#define NGEO 16
#define NDET 30
#define WL0  86.0

float bassP()  { return smoothstep(0.04, 0.85, audioBass); }
float highP()  { return pow(smoothstep(0.10, 0.90, audioHigh), 1.2); }
float beatP()  { return clamp(audioBeatPulse, 0.0, 1.0); }
float levP()   { return clamp(audioLevel, 0.0, 1.0); }

float hash11(float p) { p = fract(p * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
vec2  hash22(vec2 p)  { vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973)); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.xx + p3.yz) * p3.zy); }
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

// ── shared state ─────────────────────────────────────────────────────────
float g_t, g_ar, g_bass, g_high, g_beat, g_lev, g_sea, g_chop, g_wind, g_flash, g_cloudT;
vec3  g_ro, g_uu, g_vv, g_ww, g_L, g_sunC, g_hor, g_top, g_deep, g_sss, g_flashC;
vec2  g_flashDir;
float g_lastDens;
float g_wG, g_wB, g_wS, g_cover, g_sunI, g_stars, g_foamMul, g_sssMul, g_moonI;
vec3  g_M, g_moonC;

// ═════════════════════════════════════════════════════════════════════════
//  SPECTRUM — wave i: wavelength falls geometrically from WL0 (swell) to
//  ~0.1 m ripples; deep-water dispersion ω = √(g k); steepness kA decays so
//  the sum stays below breaking except where crests stack (→ foam).
// ═════════════════════════════════════════════════════════════════════════
float g_k0, g_w0;
vec2  g_cw;
void waveParam(float fi, out vec2 D, out float k, out float A, out float w, out float ph) {
    float e = exp(fi * 0.168);
    k = g_k0 * e;
    w = g_w0 * sqrt(e);                                   // ω = √(g k)
    float a = (hash11(fi * 7.13 + 1.7) - 0.5) * (0.75 + fi * 0.035) * spread;
    if (fi < 0.5) a = 0.55 * spread;                      // a crossing swell
    D = g_cw * cos(a) + vec2(-g_cw.y, g_cw.x) * sin(a);
    float kA = 0.1 * g_sea * exp(-0.0284 * fi) * (0.8 + 0.2 * clamp(fi * 0.2, 0.0, 1.0));
    A = kA / k;
    ph = hash11(fi * 3.71 + 0.3) * TAU;
}

// geometry waves: displacement (dx, y, dz) of the material point x
vec3 dispGeo(vec2 x, float t) {
    vec3 s = vec3(0.0);
    for (int i = 0; i < NGEO; i++) {
        vec2 D; float k, A, w, ph;
        waveParam(float(i), D, k, A, w, ph);
        float th = k * dot(D, x) - w * t + ph;
        s.xz += g_chop * A * D * cos(th);
        s.y  += A * sin(th);
    }
    return s;
}
// invert the horizontal displacement: material point that lands at world xz
vec2 solveX0(vec2 xz, float t, int iters) {
    vec2 x0 = xz;
    for (int j = 0; j < 3; j++) {
        if (j >= iters) break;
        x0 = mix(x0, xz - dispGeo(x0, t).xz, 0.8);
    }
    return x0;
}
// far field: horizontal drift is sub-pixel out there and the short swells are
// filtered anyway — 9 waves, no displacement inversion
float heightFar(vec2 x, float t) {
    float y = 0.0;
    for (int i = 0; i < 9; i++) {
        vec2 D; float k, A, w, ph;
        waveParam(float(i), D, k, A, w, ph);
        y += A * sin(k * dot(D, x) - w * t + ph);
    }
    return y;
}
// coarse march height: 12 waves, no displacement inversion (the full-res pass
// secant-refines against the exact surface, so the march only needs a bracket)
vec3 disp12(vec2 x, float t) {
    vec3 s = vec3(0.0);
    for (int i = 0; i < 12; i++) {
        vec2 D; float k, A, w, ph;
        waveParam(float(i), D, k, A, w, ph);
        float th = k * dot(D, x) - w * t + ph;
        s.xz += g_chop * A * D * cos(th);
        s.y  += A * sin(th);
    }
    return s;
}
float heightMid(vec2 xz, float t) {
    // keeps the choppy horizontal shift so crests sit where the real surface has them
    vec2 x0 = xz - disp12(xz, t).xz;
    return disp12(x0, t).y;
}
float heightAt(vec2 xz, float t) {
    vec2 x0 = solveX0(xz, t, 1);
    return dispGeo(x0, t).y;
}
// Jacobian of the geometry waves only (foam memory)
float jacGeo(vec2 x, float t) {
    float jxx = 0.0, jzz = 0.0, jxz = 0.0;
    for (int i = 0; i < 5; i++) {
        vec2 D; float k, A, w, ph;
        waveParam(float(i), D, k, A, w, ph);
        float s = sin(k * dot(D, x) - w * t + ph) * g_chop * A * k;
        jxx += s * D.x * D.x; jzz += s * D.y * D.y; jxz += s * D.x * D.y;
    }
    return (1.0 - jxx) * (1.0 - jzz) - jxz * jxz;
}
// full shading eval with per-wave footprint filtering
void detail(vec2 x, float t, float fp0, vec3 rd, out vec3 n, out float J, out float h, out float hm, out float var, out float fpOut) {
    float ym = 0.0, am = 0.0;
    var = 0.0;
    float y = 0.0, dx = 0.0, dz = 0.0, jxx = 0.0, jzz = 0.0, jxz = 0.0;
    float fp = fp0 / pow(max(-rd.y, 0.02), 0.75);
    for (int i = 0; i < NDET; i++) {
        vec2 D; float k, A, w, ph;
        waveParam(float(i), D, k, A, w, ph);
        // once the swell is summed, re-measure the footprint against the LOCAL
        // slope: a wave face turned toward the camera keeps its ripples
        if (i == 10) {
            vec3 nc = normalize(vec3(-dx, 1.0, -dz));
            fp = fp0 / pow(max(dot(nc, -rd), 0.02), 0.75);
        }
        if (k * fp > 1.0 && i > 10) {
            // LOD: every shorter wave is filtered away for this pixel → only its
            // slope variance remains (geometric tail of the spectrum, closed form)
            var += 0.5 * (k * A) * (k * A) * 6.0;
            break;
        }
        float keep = 1.0 - smoothstep(0.28, 1.0, k * fp);
        var += 0.5 * (k * A) * (k * A) * (1.0 - keep);      // slope variance the pixel can no longer resolve
        A *= keep;
        float th = k * dot(D, x) - w * t + ph;
        float s = sin(th), c = cos(th);
        y += A * s;
        if (i >= 3) { ym += A * s; am += A; }
        dx += A * k * D.x * c; dz += A * k * D.y * c;
        float q = g_chop * A * k * s;
        jxx += q * D.x * D.x; jzz += q * D.y * D.y; jxz += q * D.x * D.y;
    }
    vec3 Tx = vec3(1.0 - jxx, dx, -jxz);
    vec3 Tz = vec3(-jxz, dz, 1.0 - jzz);
    n = normalize(cross(Tz, Tx));
    J = Tx.x * Tz.z - jxz * jxz;
    // where the surface would fold over itself the normal is meaningless: it is a
    // breaking lip, so lay it down (the foam covers it)
    n = normalize(mix(n, vec3(0.0, 1.0, 0.0), smoothstep(0.2, -0.05, J)));
    if (n.y < 0.05) n = normalize(vec3(n.x, 0.05, n.z));
    h = y;
    hm = ym / max(am, 1e-4);
    fpOut = fp;
}

// ═════════════════════════════════════════════════════════════════════════
//  SKY — gradient + sun + lit storm cloud deck + lightning inside it
// ═════════════════════════════════════════════════════════════════════════
float cloudFbm(vec2 p, int oct) {
    float a = 0.5, s = 0.0;
    for (int i = 0; i < 6; i++) {
        if (i >= oct) break;
        s += a * vnoise2(p);
        if (i == 1) p += 0.35 * vec2(vnoise2(p * 0.5), vnoise2(p * 0.5 + 9.0));
        p = mat2(1.6, 1.2, -1.2, 1.6) * p + vec2(3.1, 1.7);
        a *= 0.5;
    }
    return s;
}
vec3 skyBase(vec3 rd, float blur) {
    float y = max(rd.y, 0.0);
    float sd = max(dot(rd, g_L), 0.0);
    vec3 col = mix(g_hor * (0.35 + 0.65 * pow(sd, 2.0)), g_top, pow(smoothstep(0.0, 0.32, y), 0.6));
    col += g_hor * 0.7 * exp(-y * 40.0 / (1.0 + blur * 8.0)) * (0.2 + 0.8 * pow(sd, 3.0)) / (1.0 + blur * 3.0);
    col += g_sunC * (pow(sd, 12.0) * 0.35 + pow(sd, 180.0) * 1.4) * (0.35 + 0.65 * g_sunI) * (1.0 - 0.6 * g_wS);
    return col;
}
// blur: 0 = crisp sky, ~0.5 = what a rough sea reflects (soft cloud edges, no stars)
// crisp point sources, evaluated at full resolution on top of the half-res sky:
// stars, the limb-darkened sun disk, the moon disk
vec3 skyPoints(vec3 rd) {
    vec3 col = vec3(0.0);
    if (g_stars > 0.001 && rd.y > 0.03) {
        vec2 sp = rd.xz / (rd.y + 0.25) * 140.0;
        vec2 sc = floor(sp), sf = fract(sp);
        vec2 so = hash22(sc + 31.7) * 0.7 + 0.15;
        float sh = hash12(sc + 7.1);
        float st = step(0.965, sh) * exp(-dot(sf - so, sf - so) * 260.0);
        st *= (0.5 + 0.5 * sin(TIME * (2.0 + sh * 5.0) + sh * 40.0)) * 0.6 + 0.4;
        col += vec3(0.85, 0.9, 1.0) * st * (1.2 + 3.0 * pow(hash12(sc + 3.3), 6.0)) * g_stars * smoothstep(0.03, 0.2, rd.y);
    }
    float sd = max(dot(rd, g_L), 0.0);
    float ang = sqrt(max(2.0 * (1.0 - sd), 0.0));
    float rho = ang / 0.021;
    float disk = 1.0 - smoothstep(0.9, 1.0, rho);
    disk *= 1.0 - 0.55 * (1.0 - sqrt(max(1.0 - rho * rho, 0.0)));
    col += g_sunC * disk * 24.0 * g_sunI * smoothstep(-0.03, 0.01, g_L.y);
    if (g_moonI > 0.001) {
        float md = max(dot(rd, g_M), 0.0);
        float mang = sqrt(max(2.0 * (1.0 - md), 0.0));
        float mr = mang / 0.013;
        if (mr < 1.2) {
            vec3 mt = normalize(cross(g_M, vec3(0.0, 1.0, 0.0)));
            vec2 mq = vec2(dot(rd - g_M, mt), dot(rd - g_M, cross(mt, g_M))) / 0.013;
            float maria = 0.78 + 0.22 * smoothstep(0.35, 0.7, vnoise2(mq * 2.3 + 4.0) * 0.6 + vnoise2(mq * 5.7) * 0.4);
            float mdisk = (1.0 - smoothstep(0.9, 1.0, mr)) * maria * (1.0 - 0.3 * (1.0 - sqrt(max(1.0 - mr * mr, 0.0))));
            col += g_moonC * mdisk * 3.2 * g_moonI;
        }
    }
    return col;
}
vec3 skyCol(vec3 rd, int oct, float blur) {
    float y = max(rd.y, 0.0);
    float sd = max(dot(rd, g_L), 0.0);
    vec3 col = skyBase(rd, blur);
    // soft sun / moon halos (resolution-independent)
    float ang = sqrt(max(2.0 * (1.0 - sd), 0.0));
    float sunUp = smoothstep(-0.03, 0.01, g_L.y);
    col += g_sunC * (exp(-ang * 90.0) * 1.2 + exp(-ang * 22.0) * 0.25) * (blur > 0.001 ? 0.3 : 1.0) * g_sunI * sunUp;
    if (g_moonI > 0.001) {
        float mang = sqrt(max(2.0 * (1.0 - max(dot(rd, g_M), 0.0)), 0.0));
        col += g_moonC * (exp(-mang * 40.0) * 0.35 + exp(-mang * 9.0) * 0.06) * g_moonI;
    }
    g_lastDens = 0.0;
    if (rd.y > 0.004) {
        float tc = 1.0 / (rd.y + 0.035);
        vec2 cw = vec2(cos(g_wind), sin(g_wind));
        vec2 cp0 = rd.xz * tc * 1.7;
        vec2 cp = vec2(dot(cp0, cw) * 0.55, dot(cp0, vec2(-cw.y, cw.x))) + vec2(0.05, 0.015) * g_cloudT;
        cp += 0.45 * vec2(vnoise2(cp * 0.6 + 0.03 * g_cloudT), vnoise2(cp * 0.6 + 5.2)) ;
        float d = cloudFbm(cp, oct);
        // fine erosion only on the crisp primary view: wispy torn edges
        if (oct >= 5) d += (vnoise2(cp * 13.0 + 0.2 * g_cloudT) - 0.5) * 0.07 + (vnoise2(cp * 29.0) - 0.5) * 0.03;
        float cov = mix(0.74, 0.3, g_cover);
        // the storm deck tears open around the low sun: a ragged gap of warm light
        float gapA = exp(-(ang * ang) / 0.012);
        cov += g_wS * 0.42 * smoothstep(0.15, 0.85, gapA * (0.65 + 0.7 * vnoise2(cp * 1.7 + 2.0)));
        float thick = clamp((d - cov) / 0.5, 0.0, 1.0);
        float dens = smoothstep(0.0, 0.3 + blur * 0.4, thick);
        thick = sqrt(thick);
        float d2 = cloudFbm(cp + g_L.xz * 0.12, oct > 3 ? 3 : oct);
        float lit = clamp(0.5 + (d - d2) * 4.0, 0.0, 1.0);
        float nearH = exp(-rd.y * 6.0);
        float edgeT = (1.0 - thick) * (1.0 - thick);
        // volumetric read: dark self-shadowed cores, undersides warmed by the low
        // sun, lit flanks toward it, a silver lining burning on the thin edges
        vec3 core = (mix(vec3(lum(g_top)), g_top, 0.55 + 0.4 * g_wS) * 1.35 + g_hor * 0.12 + vec3(0.008)) * mix(1.0, 0.55, thick) * (1.0 - 0.35 * g_wS);
        vec3 warm = g_hor * (1.0 - 0.45 * g_wS) * (0.3 + 1.0 * nearH) * (0.5 + 0.7 * pow(sd, 2.0)) * (0.35 + 0.65 * lit);
        float w = clamp((1.0 - thick) * 0.3 + lit * 0.55, 0.0, 1.0) * (0.3 + 0.7 * nearH);
        vec3 cc = mix(core, warm, w);
        cc += g_sunC * (pow(sd, 25.0 + 35.0 * g_wS) * 4.0 * (1.0 - blur) + pow(sd, 6.0 + 20.0 * g_wS) * 0.6) * edgeT * g_sunI * sunUp;
        // lightning lives inside the deck
        float fd = length(rd.xz / max(rd.y, 0.02) * 0.3 - g_flashDir);
        float fl = g_flash * exp(-fd * 3.2);
        cc += g_flashC * fl * (0.4 + 1.6 * dens) * (0.5 + d);
        float fade = smoothstep(0.004, 0.09, rd.y);
        // high cirrus streaks above the deck, catching the last light
        if (oct >= 4) {
            float tc2 = 1.0 / (rd.y + 0.09);
            vec2 q = rd.xz * tc2 * 0.8;
            q = vec2(dot(q, cw) * 0.16, dot(q, vec2(-cw.y, cw.x)) * 1.1) + vec2(0.02, 0.0) * g_cloudT;
            float ci = smoothstep(0.52, 0.86, cloudFbm(q, 3)) * fade * (1.0 - g_wS * 0.7);
            vec3 cic = g_top * 0.8 + g_hor * 0.7 * nearH + g_sunC * pow(sd, 4.0) * 0.9 * g_sunI * sunUp;
            col = mix(col, cic, ci * 0.45);
        }
        col = mix(col, cc, dens * fade);
        g_lastDens = dens * fade;
        col += g_flashC * fl * 0.12 * fade;
    }
    return col;
}

// ═════════════════════════════════════════════════════════════════════════
//  MARCH
// ═════════════════════════════════════════════════════════════════════════
float marchSea(vec3 ro, vec3 rd, float tmax) {
    float maxH = 1.6 * g_sea * waveScale + 0.2;
    if (rd.y >= 0.0 && ro.y > maxH) return -1.0;
    float t = 0.0;
    if (ro.y > maxH) t = (ro.y - maxH) / max(-rd.y, 1e-4);
    if (t > tmax) return -1.0;
    float tPrev = t, dPrev = 1e9;
    for (int i = 0; i < 72; i++) {
        vec3 p = ro + rd * t;
        float d = p.y - (t < 50.0 ? heightMid(p.xz, g_t) : heightFar(p.xz, g_t));
        if (d < 0.0) {
            // bisection between the last point above and this one
            float a = tPrev, b = t;
            for (int j = 0; j < 6; j++) {
                float m = 0.5 * (a + b);
                vec3 q = ro + rd * m;
                if (q.y - (m < 50.0 ? heightMid(q.xz, g_t) : heightFar(q.xz, g_t)) < 0.0) b = m; else a = m;
            }
            return 0.5 * (a + b);
        }
        tPrev = t; dPrev = d;
        t += max(d * (t < 50.0 ? 0.6 : 0.8), 0.015 + t * 0.008);
        if (t > tmax) break;
    }
    return -1.0;
}

// value noise with analytic gradient (xy = d/dx, d/dy)
vec3 vnoiseD(vec2 x) {
    vec2 p = floor(x), f = fract(x);
    vec2 u = f * f * (3.0 - 2.0 * f), du = 6.0 * f * (1.0 - f);
    float a = hash12(p), b = hash12(p + vec2(1.0, 0.0)), c = hash12(p + vec2(0.0, 1.0)), d = hash12(p + vec2(1.0, 1.0));
    float k1 = b - a, k2 = c - a, k4 = a - b - c + d;
    return vec3(a + k1 * u.x + k2 * u.y + k4 * u.x * u.y, du * vec2(k1 + k4 * u.y, k2 + k4 * u.x));
}

// sky panorama: u = azimuth relative to the camera heading (seam behind the
// camera), v = sqrt(elevation) so the horizon gets most of the texels
vec2 envUV(vec3 d) {
    float az = atan(dot(d.xz, vec2(g_ww.z, -g_ww.x)), dot(d.xz, normalize(g_ww.xz)));
    return vec2(az / TAU + 0.5, sqrt(clamp(d.y, 0.0, 1.0)));
}
vec3 envDir(vec2 uv) {
    float az = (uv.x - 0.5) * TAU, el = uv.y * uv.y;
    vec2 f = normalize(g_ww.xz), r = vec2(f.y, -f.x);
    vec2 h = (f * cos(az) + r * sin(az)) * sqrt(max(1.0 - el * el, 0.0));
    return vec3(h.x, el, h.y);
}
vec3 envLookup(vec3 d) { return texture2D(ocEnv, envUV(d)).rgb; }

float marchFrom(vec3 ro, vec3 rd, float t, float tmax) {
    // back off if the start point is already under the water
    for (int k = 0; k < 6; k++) {
        vec3 p0 = ro + rd * t;
        if (p0.y - (t < 70.0 ? heightMid(p0.xz, g_t) : heightFar(p0.xz, g_t)) > 0.0) break;
        t *= 0.7;
    }
    float tPrev = t;
    for (int i = 0; i < 40; i++) {
        vec3 p = ro + rd * t;
        float d = p.y - (t < 70.0 ? heightMid(p.xz, g_t) : heightFar(p.xz, g_t));
        if (d < 0.0) {
            float a = tPrev, b = t;
            for (int j = 0; j < 5; j++) {
                float m = 0.5 * (a + b);
                vec3 q = ro + rd * m;
                if (q.y - (m < 70.0 ? heightMid(q.xz, g_t) : heightFar(q.xz, g_t)) < 0.0) b = m; else a = m;
            }
            return 0.5 * (a + b);
        }
        tPrev = t;
        t += max(d * (t < 50.0 ? 0.7 : 0.85), 0.012 + t * (t < 50.0 ? 0.008 : 0.018));
        if (t > tmax) break;
    }
    return -1.0;
}

vec3 seaShade(vec3 p, vec3 rd, float t, float pixAng, out float foamOut) {
    float fp0 = t * pixAng;
    vec2 x0 = p.xz - disp12(p.xz, g_t).xz;   // same inversion as the hit surface
    vec3 n; float J, h, hm, var, fp;
    detail(x0, g_t, fp0, rd, n, J, h, hm, var, fp);

    // capillary / ripple octaves on top of the Gerstner normal, each faded by the
    // pixel footprint; whatever the pixel can't resolve becomes roughness instead
    vec2 cw = vec2(cos(g_wind), sin(g_wind));
    // capillary ripple TRAINS: 12 short directional waves (0.35 m → 4 cm) around the
    // wind, each with its own dispersion speed — long crests across the wind, so the
    // grazing reflections stretch into thin horizontal slivers, never round blobs
    vec2 gsum = vec2(0.0);
    if (t > 40.0) var += 0.02 * (0.6 + 0.6 * g_sea);   // all ripples unresolved out there
    else
    for (int r = 0; r < 6; r++) {
        float fr = float(r);
        float lam = 0.35 * exp(-fr * 0.38);
        float k = TAU / lam;
        if (k * fp > 1.1) {
            var += 0.5 * (0.11 * pow(0.93, fr)) * (0.11 * pow(0.93, fr)) * (12.0 - fr) * (0.6 + 0.6 * g_sea);
            break;
        }
        float keep = 1.0 - smoothstep(0.3, 1.1, k * fp);
        float ang = g_wind + (hash11(fr * 5.37 + 9.1) - 0.5) * 1.1 * spread;
        vec2 D = vec2(cos(ang), sin(ang));
        float w = sqrt(9.81 * k + 0.074 / 1000.0 * k * k * k);
        float sl = 0.13 * pow(0.88, fr) * (0.6 + 0.6 * g_sea);   // slope amplitude kA
        float ph = hash11(fr * 2.91 + 4.3) * TAU;
        // a slow envelope breaks each train into wave groups
        float env = 0.55 + 0.45 * sin(dot(x0, D.yx * vec2(1.0, -1.0)) * k * 0.07 + ph * 1.7 + g_t * 0.3);
        float c = cos(k * dot(D, x0) - w * g_t + ph);
        gsum += D * sl * c * env * keep;
        var += 0.5 * sl * sl * (1.0 - keep);
    }
    n = normalize(n + vec3(-gsum.x, 0.0, -gsum.y));
    float roughA = clamp(sqrt(2.0 * var + 0.0009), 0.03, 0.55);

    float ndv = max(dot(n, -rd), 0.001);
    float F = 0.02 + 0.98 * pow(1.0 - ndv, 5.0);
    // microfacet shadowing-masking: grazing back-slopes can't mirror the full sky
    float a2r = roughA * roughA;
    F *= 2.0 * ndv / (ndv + sqrt(a2r + (1.0 - a2r) * ndv * ndv));
    vec3 rf = reflect(rd, n);
    // a ray reflected downward would hit the next wave, not the sky: fade it to
    // the dark water it really sees instead of mirroring the bright horizon band
    float rDown = smoothstep(-0.02, -0.3, rf.y);
    rf.y = max(rf.y, 0.0) + 0.004;
    // reflections read a precomputed low-res sky panorama (soft clouds); rough
    // water fades toward the cloud-free gradient
    vec3 refl = mix(envLookup(rf), skyBase(rf, 0.5), smoothstep(0.12, 0.4, roughA));
    refl = mix(refl, g_deep * 2.0 + g_top * 0.2, rDown * 0.85);

    // GGX sun with footprint-aware roughness
    vec3 H = normalize(g_L - rd);
    float nh = max(dot(n, H), 0.0);
    float ndl = max(dot(n, g_L), 0.0);
    float a2 = roughA * roughA;
    float dd = nh * nh * (a2 - 1.0) + 1.0;
    float Dg = a2 / (PI * dd * dd);
    float Fs = 0.02 + 0.98 * pow(1.0 - max(dot(H, -rd), 0.0), 5.0);
    float Gv = 2.0 * ndv / (ndv + sqrt(a2 + (1.0 - a2) * ndv * ndv));
    float sunVis = smoothstep(-0.03, 0.04, g_L.y) * g_sunI;
    vec3 sunL = g_sunC * sunVis * glitter * (1.0 + 0.6 * g_ar * g_lev);
    float audLift = 1.0 + 0.35 * g_ar * g_bass;
    vec3 spec = sunL * min(Dg * Fs * Gv * 0.25 / max(ndv, 0.05), 10.0) * 0.2;
    // the moon gets its own GGX lobe + glitter path
    vec3 Hm = normalize(g_M - rd + vec3(0.0, 1e-4, 0.0));
    float nhm = max(dot(n, Hm), 0.0);
    float ddm = nhm * nhm * (a2 - 1.0) + 1.0;
    float Fm = 0.02 + 0.98 * pow(1.0 - max(dot(Hm, -rd), 0.0), 5.0);
    vec3 moonL = g_moonC * g_moonI * glitter * smoothstep(-0.02, 0.03, g_M.y);
    spec += moonL * min(a2 / (PI * ddm * ddm) * Fm * Gv * 0.25 / max(ndv, 0.05), 10.0) * 0.12;
    // stochastic glints: a few micro-facets per footprint cell drawn from the
    // unresolved slope distribution; each facet wobbles continuously (no popping)
    // and flashes when it aims at the sun or the moon
    float cs = max(fp * 0.9, 0.003);
    vec2 cid = floor(x0 / cs);
    float sig = sqrt(var + 0.003) * 1.6;
    float glint = 0.0, glintM = 0.0;
    for (int j = 0; j < 2; j++) {
        vec4 hj = vec4(hash22(cid * 1.31 + float(j) * 17.3), hash22(cid * 0.77 + float(j) * 5.9));
        float wob = hj.z * 6.2832 + g_t * (1.5 + 3.0 * hj.w);
        vec2 sl = ((hj.xy - 0.5) * 1.6 + 0.45 * vec2(cos(wob), sin(wob))) * sig;
        vec3 nf = normalize(n + vec3(sl.x, 0.0, sl.y));
        glint  += smoothstep(0.99972, 0.99994, dot(nf, H));
        glintM += smoothstep(0.99975, 0.99995, dot(nf, Hm));
    }
    float gfar = 1.0 - smoothstep(150.0, 400.0, t);
    spec += min(sunL * min(glint, 1.5) * (0.6 + 6.0 * Fs) * 5.0 * (1.0 + 0.8 * g_ar * g_high) * gfar * audLift, vec3(1.15));
    spec += min(moonL * min(glintM, 1.5) * (0.6 + 6.0 * Fm) * 2.6 * gfar * audLift, vec3(1.0));

    // body + subsurface through thin backlit crests (the hero colour)
    float hrel = clamp(h / (0.9 * g_sea * waveScale) * 0.5 + 0.5, 0.0, 1.0);
    vec2 hd = normalize(rd.xz), ld = normalize(g_L.xz + 1e-4);
    float toSun = pow(clamp(dot(hd, ld) * 0.5 + 0.5, 0.0, 1.0), 3.0);
    float facing = clamp(1.0 - n.y, 0.0, 1.0);
    vec3 body = g_deep * 1.1 + g_top * 0.14 + g_hor * 0.04;
    float crest = smoothstep(0.1, 0.7, hm) * smoothstep(0.3, 0.92, hrel);
    float sssLight = (0.35 * sunVis * (0.3 + 2.4 * toSun) + 0.25 + 0.6 * g_wB * (0.4 + 1.6 * toSun)) * audLift;
    vec3 sss = g_sss * sssAmt * g_sssMul * crest * sssLight * (0.35 + 1.5 * facing);
    sss *= (1.0 - smoothstep(30.0, 180.0, t)) * 0.75;
    body += g_sss * 0.05 * (0.4 + hrel) * (1.0 - smoothstep(20.0, 200.0, t));
    vec3 col = mix(body + sss, refl, F) + spec;

    // whitecaps: fold (Jacobian) now, plus where crests folded moments ago
    float thr = 0.5 + 0.2 * g_wS + 0.06 * g_ar * g_lev;
    float fNow = smoothstep(thr + 0.18, thr - 0.25, J);
    float fM1 = 0.0, fM2 = 0.0;
    if (t < 90.0) {
        // memory: a spatially smooth random lag, so the residue smears continuously behind each crest
        float lag1 = 0.15 + 1.6 * vnoise2(x0 * 0.35 + 3.1);
        float lag2 = 0.6 + 2.6 * vnoise2(x0 * 0.21 - 7.4);
        fM1  = smoothstep(thr + 0.12, thr - 0.25, jacGeo(x0, g_t - lag1)) * (1.0 - lag1 / 2.2);
        fM2  = smoothstep(thr + 0.08, thr - 0.25, jacGeo(x0, g_t - lag2)) * (1.0 - lag2 / 3.6);
    }
    float mem = max(fNow, max(fM1, fM2 * 0.8));
    // lace: bubble cells (voronoi edges) eaten by two noise octaves → organic holes
    float nz = vnoise2(x0 * 0.9) * 0.55 + vnoise2(x0 * 3.7) * 0.3 + vnoise2(x0 * 9.3) * 0.15;
    vec2 lw = x0 + 0.6 * vec2(vnoise2(x0 * 1.3 + 4.0), vnoise2(x0 * 1.3 - 2.0));
    // octaves fade with the footprint so far foam turns into clean streaks, never noise
    float k2 = 1.0 - smoothstep(0.15, 0.6, 5.1 * fp), k3 = 1.0 - smoothstep(0.15, 0.6, 11.7 * fp);
    float ln = vnoise2(lw * 2.2) * 0.5 + mix(0.5, vnoise2(lw * 5.1), k2) * 0.3 + mix(0.5, vnoise2(lw * 11.7), k3) * 0.2;
    float lace = 0.2 + 0.7 * smoothstep(0.28, 0.72, ln);
    float lacedet = 1.0 - smoothstep(10.0, 60.0, t);
    float lw2 = 0.035 + fp * 0.8;
    float fz = mem * foamAmt * g_foamMul * (1.0 + 0.4 * g_ar * g_lev) * (1.0 - 0.55 * smoothstep(15.0, 140.0, t));
    float foam = smoothstep(lace - lw2, lace + lw2, 0.08 + 0.62 * min(fz, 1.0)) * smoothstep(0.04, 0.22, fz);
    foam = clamp(foam, 0.0, 1.0);
    float dif = ndl * sunVis;
    vec3 foamC = vec3(0.9, 0.93, 0.97) * (0.1 + 0.4 * lum(g_hor) + 0.3 * lum(g_top) + 0.9 * dif * g_sunC) + g_sss * 0.14 * hrel;
    if (t < 35.0 && foam > 0.001) {
        // bubble walls: crisp fine cells inside the foam sheet
        vec2 bp = x0 * 6.5;
        vec2 bc = floor(bp), bf = fract(bp);
        float b1 = 9.0, b2 = 9.0;
        for (int j = -1; j <= 1; j++)
        for (int i = -1; i <= 1; i++) {
            vec2 g = vec2(float(i), float(j));
            float d = length(g + hash22(bc + g + 17.0) - bf);
            if (d < b1) { b2 = b1; b1 = d; } else if (d < b2) b2 = d;
        }
        // bubble walls catch the light, the thin film inside each cell lets the water show
        // round bubble holes of varied size punched through the sheet
        float bsz = 0.12 + 0.22 * hash12(bc + 3.7);
        float hole = (1.0 - smoothstep(bsz * 0.75, bsz, b1)) * step(0.45, hash12(bc + 9.1));
        foamC *= 0.9 + 0.22 * smoothstep(0.3, 0.7, nz) + 0.15 * smoothstep(bsz, bsz * 1.25, b1) * hole;
        foam *= 1.0 - 0.8 * hole * lacedet;
    }
    foamC += g_flashC * g_flash * 0.25;
    col = mix(col, foamC, foam * 0.92);
    // old foam combed into long streaks along the wind
    vec2 cwf = vec2(cos(g_wind), sin(g_wind));
    vec2 sxy = vec2(dot(x0, cwf) * 0.22, dot(x0, vec2(-cwf.y, cwf.x)) * 2.6);
    float sk3 = 1.0 - smoothstep(0.15, 0.6, 2.6 * fp);
    float strk = vnoise2(sxy + vec2(g_t * 0.05, 0.0)) * 0.7 + mix(0.5, vnoise2(sxy * 2.7 + 7.0), sk3) * 0.3;
    float streakF = smoothstep(0.62, 0.66 + fp * 0.5, strk) * smoothstep(0.05, 0.4, max(fM1, fM2)) * g_foamMul * foamAmt;
    col = mix(col, foamC * 0.8, clamp(streakF, 0.0, 1.0) * 0.3 * (1.0 - foam));
    foamOut = foam * hrel * (1.0 - smoothstep(30.0, 120.0, t));

    // aerial perspective toward the horizon haze
    float ff = 1.0 - exp(-t * 0.0032 * fog);
    vec3 haze = skyBase(normalize(vec3(rd.x, 0.012, rd.z)), 0.0);
    col = mix(col, haze, ff);
    return col;
}

void setupCam() {
    float yaw = radians(camYaw) + 0.3 * sin(TIME * 0.017) + 0.07 * sin(TIME * 0.071 + 1.0);
    vec2 fwd = vec2(sin(yaw), cos(yaw));
    vec2 rgt = vec2(fwd.y, -fwd.x);
    float z = TIME * travel * 3.2;
    vec2 cxz = fwd * z;
    // ride low between the swells like a small boat: rise and sink with the sea,
    // pitch and roll with its slope, never dip under a crest
    float hc  = heightAt(cxz, g_t);
    float hf  = heightAt(cxz + fwd * 5.0, g_t);
    float hb  = heightAt(cxz - fwd * 5.0, g_t);
    float hl  = heightAt(cxz - rgt * 4.0, g_t);
    float hr  = heightAt(cxz + rgt * 4.0, g_t);
    g_ro = vec3(cxz.x, max(camHeight * (0.6 + 0.4 * g_sea) + 0.7 * hc, hc + 1.1 * g_sea), cxz.y);
    float pitch = camPitch + 0.35 * atan((hf - hb) / 10.0);
    g_ww = normalize(vec3(fwd.x, pitch, fwd.y));
    float roll = 0.015 * sin(g_t * 0.23) + 0.45 * atan((hr - hl) / 8.0);
    vec3 up = vec3(sin(roll) * rgt.x, cos(roll), sin(roll) * rgt.y);
    g_uu = normalize(cross(up, g_ww));
    g_vv = cross(g_ww, g_uu);
}

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = (fragCoord - 0.5 * R) / R.y;
    g_ar = audioReact;
    g_bass = bassP(); g_high = highP(); g_beat = beatP(); g_lev = levP();
    g_t = TIME * speed;
    // the sea state breathes over minutes: calm stretches, then a building set
    // weather arc: golden sunset → blue hour → storm → back (~3.5 min at cycle 1)
    float cyc = weather + TIME * weatherCycle * (3.0 / 210.0);
    float seg = mod(floor(cyc), 3.0);
    float fb = smoothstep(0.5, 1.0, fract(cyc));
    vec3 wA = seg < 0.5 ? vec3(1.0, 0.0, 0.0) : (seg < 1.5 ? vec3(0.0, 1.0, 0.0) : vec3(0.0, 0.0, 1.0));
    vec3 wN = seg < 0.5 ? vec3(0.0, 1.0, 0.0) : (seg < 1.5 ? vec3(0.0, 0.0, 1.0) : vec3(1.0, 0.0, 0.0));
    vec3 W = mix(wA, wN, fb);
    g_wG = W.x; g_wB = W.y; g_wS = W.z;
    g_cover = dot(W, vec3(cloudCover, 0.42, 0.95));
    g_sunI = sunIntensity * dot(W, vec3(1.0, 0.0, 0.55));
    g_stars = stars * g_wB;
    g_foamMul = dot(W, vec3(1.0, 0.7, 1.35));
    g_sssMul = dot(W, vec3(1.0, 1.8, 0.8));
    // the sea state breathes over minutes and builds toward the storm
    g_sea = seaState * dot(W, vec3(1.0, 0.85, 1.12)) * (0.88 + 0.24 * smoothstep(-0.9, 0.9, sin(TIME * 0.021 + 0.6))) * (1.0 + 0.22 * g_ar * g_bass);
    g_chop = choppy * 0.74 * dot(W, vec3(1.0, 0.9, 1.1));
    g_wind = radians(windDir);
    g_cw = vec2(cos(g_wind), sin(g_wind));
    g_k0 = TAU / (WL0 * waveScale);
    g_w0 = sqrt(9.81 * g_k0);
    g_cloudT = TIME * cloudSpeed;
    g_top = hueRot(W.x * skyTop.rgb + W.y * blueTop.rgb + W.z * stormTop.rgb, hueShift);
    g_hor = hueRot(W.x * skyHorizon.rgb + W.y * blueHorizon.rgb + W.z * stormHorizon.rgb, hueShift);
    vec3 sc0 = sunColor.rgb;
    g_sunC = hueRot(W.x * sc0 + W.y * sc0 * vec3(1.0, 0.55, 0.75) + W.z * sc0 * vec3(1.0, 0.85, 0.7), hueShift);
    g_deep = hueRot(deepColor.rgb, hueShift);
    g_sss = hueRot(sssColor.rgb, hueShift);
    g_flashC = flashColor.rgb;
    // lightning: a slow random strike cell + beats (soft envelope, capped)
    float cellT = floor(TIME * 0.23);
    float ph = fract(TIME * 0.23);
    float idle = step(0.55, hash11(cellT * 1.37)) * exp(-ph * 14.0) * (0.6 + 0.4 * sin(ph * 90.0)) * smoothstep(0.0, 0.02, ph);
    g_flash = lightning * dot(W, vec3(0.12, 0.0, 1.0)) * clamp(idle * 0.8 + 0.45 * g_ar * g_beat * smoothstep(0.25, 0.8, g_bass), 0.0, 1.2);
    g_flashDir = (hash22(vec2(cellT, 3.7)) - 0.5) * vec2(2.6, 1.0) + vec2(0.0, 1.6);
    float yaw0 = radians(camYaw);
    g_moonI = moonAmt * g_wB;
    float ma = yaw0 + radians(sunAz - 7.0), me = radians(2.5 + 6.0 * g_wB);
    g_M = normalize(vec3(sin(ma) * cos(me), sin(me), cos(ma) * cos(me)));
    g_moonC = vec3(0.86, 0.9, 1.0);
    float sa = yaw0 + radians(sunAz), se = radians(dot(W, vec3(sunHeight, -4.5, 7.0)));
    g_L = normalize(vec3(sin(sa) * cos(se), sin(se), cos(sa) * cos(se)));
    setupCam();

    if (PASSINDEX == 0) {
        // low-res march: only the hit DISTANCE is kept (0 = sky)
        vec3 rd = normalize(g_uu * uv.x + g_vv * uv.y + g_ww * 1.35);
        float t = marchSea(g_ro, rd, 420.0);
        if (t < 0.0 && rd.y < 0.0) t = g_ro.y / max(-rd.y, 1e-4);   // far sea as a plane
        gl_FragColor = vec4(max(t, 0.0), 0.0, 0.0, 1.0);
    } else if (PASSINDEX == 1) {
        // sky panorama for the reflections
        vec2 st = fragCoord / R;
        gl_FragColor = vec4(skyCol(envDir(st), 3, 0.15), 1.0);
    } else if (PASSINDEX == 2) {
        // primary sky + clouds at half res (clouds are soft); a = cloud density
        vec3 rd = normalize(g_uu * uv.x + g_vv * uv.y + g_ww * 1.35);
        vec3 c = skyCol(rd, 5, 0.0);
        gl_FragColor = vec4(c, g_lastDens);
    } else if (PASSINDEX == 3) {
        // full-res shading: rebuild this pixel's own hit from the low-res depths
        // (pick the neighbour whose depth best fits THIS ray, then secant-refine
        // against the analytic sea), so every crest edge and ripple is pixel-sharp
        vec3 rd = normalize(g_uu * uv.x + g_vv * uv.y + g_ww * 1.35);
        float pixAng = 1.0 / (1.35 * R.y);
        vec2 Rm = floor(R * marchRes);
        vec2 st = fragCoord / R;
        vec2 b0 = (floor(st * Rm - 0.5) + 0.5) / Rm;
        float best = -1.0, bestE = 1e9, nSky = 0.0, tmn = 1e9, tmx = 0.0;
        // 3x3 low-res neighbourhood: a crest thinner than one march texel still
        // shows up as a depth break and triggers the exact per-pixel re-march
        for (int k = 0; k < 9; k++) {
            vec2 o = (vec2(mod(float(k), 3.0), floor(float(k) / 3.0)) - 1.0) / Rm;
            float tc = texture2D(ocMarch, (floor(st * Rm) + 0.5) / Rm + o).r;   // exact texel centres, never blended depths
            if (tc <= 0.0) { nSky += 1.0; continue; }
            tmn = min(tmn, tc); tmx = max(tmx, tc);
            vec3 q = g_ro + rd * tc;
            float e = abs(q.y - heightFar(q.xz, g_t)) / (0.3 + tc * 0.02);
            if (e < bestE) { bestE = e; best = tc; }
        }
        float t = -1.0;
        bool edge = (nSky > 0.5 && nSky < 8.5 || (tmx > tmn * 1.12 + 0.3)) && tmn < 110.0;
        if (edge) {
            // silhouette / depth break: re-march this exact ray locally at full res
            // start from a per-pixel continuous point (never a texel-constant one,
            // which made the step phase — and so thin crest tips — stair-step)
            float ts = max(g_ro.y - 1.6 * g_sea * waveScale - 0.2, 0.0) / max(-rd.y, 1e-3);
            if (tmn < 1e8) ts = max(ts, tmn * 0.85);    // nothing in this 3x3 is nearer than its nearest hit
            ts *= 0.97 + 0.03 * fract(52.9829189 * fract(0.06711056 * fragCoord.x + 0.00583715 * fragCoord.y));
            t = marchFrom(g_ro, rd, max(ts, 0.0), nSky > 0.5 ? 420.0 : tmx * 1.15 + 1.0);
            if (t < 0.0 && rd.y < -0.002 && nSky < 0.5) t = tmx;
        } else if (best > 0.0) {
            // secant refinement along this pixel's ray
            float t0 = best * 0.95 - 0.05, t1 = best * 1.05 + 0.05;
            vec3 q0 = g_ro + rd * t0, q1 = g_ro + rd * t1;
            float f0 = q0.y - (t0 < 70.0 ? heightMid(q0.xz, g_t) : heightFar(q0.xz, g_t));
            float f1 = q1.y - (t1 < 70.0 ? heightMid(q1.xz, g_t) : heightFar(q1.xz, g_t));
            bool bracket = f0 > 0.0 && f1 < 0.0;
            for (int i = 0; i < 2; i++) {
                float den = f1 - f0;
                if (abs(den) < 1e-6) break;
                float t2 = clamp(t1 - f1 * (t1 - t0) / den, best * 0.6, best * 1.6 + 1.0);
                t0 = t1; f0 = f1; t1 = t2;
                vec3 q2 = g_ro + rd * t1;
                f1 = q2.y - (t1 < 70.0 ? heightMid(q2.xz, g_t) : heightFar(q2.xz, g_t));
            }
            t = t1;
            // didn't converge (this pixel slips past a crest its neighbours hit):
            // re-march the exact ray
            if ((!bracket || abs(f1) > 0.004 + 0.0015 * t1) && best < 60.0) t = marchFrom(g_ro, rd, max(g_ro.y - 1.6 * g_sea * waveScale - 0.2, 0.0) / max(-rd.y, 1e-3), 420.0);
            if (t < 0.0 && rd.y < -0.002) t = g_ro.y / max(-rd.y, 1e-4);
        }
        vec3 col; float a;
        if (t > 0.0) {
            vec3 p = g_ro + rd * t;
            float foam;
            col = seaShade(p, rd, t, pixAng, foam);
            a = 0.5 + 0.5 * clamp(foam, 0.0, 1.0);
        } else {
            vec4 sk = texture2D(ocSky, fragCoord / R);
            col = sk.rgb + skyPoints(rd) * (1.0 - sk.a);
            a = 0.0;
        }
        gl_FragColor = vec4(max(col, 0.0), a);
    } else if (PASSINDEX == 4) {
        // bloom prefilter at quarter res: 5x5 taps over the footprint, soft threshold
        vec2 st = fragCoord / R;
        vec2 px = 1.0 / R;
        vec3 acc = vec3(0.0);
        for (int j = -2; j <= 2; j++)
        for (int i = -2; i <= 2; i++) {
            vec2 o = vec2(float(i), float(j)) * px * 0.8;
            vec3 c = texture2D(ocScene, st + o).rgb;
            acc += min(max(c - 1.2, 0.0), vec3(4.0)) * exp(-0.3 * float(i * i + j * j));
        }
        float rays = 0.0;
        float sw = dot(g_L, g_ww);
        if (sw > 0.05 && godRays * (0.25 * g_wG + 1.2 * g_wS) * g_sunI > 0.01) {
            vec2 suv = 1.35 * vec2(dot(g_L, g_uu), dot(g_L, g_vv)) / sw;
            vec2 sst = vec2(suv.x * R.y / R.x, suv.y) + 0.5;
            vec2 dlt = (sst - st) / 24.0;
            float dith = fract(52.9829189 * fract(0.06711056 * fragCoord.x + 0.00583715 * fragCoord.y));
            vec2 sp2 = st + dlt * 0.5;
            float ra = 0.0, wsum = 0.0, wgt = 1.0;
            for (int i = 0; i < 24; i++) {
                vec4 s4 = texture2D(ocScene, sp2);
                ra += step(s4.a, 0.25) * clamp(lum(s4.rgb) - 0.35, 0.0, 2.5) * wgt;
                wsum += wgt; wgt *= 0.95;
                sp2 += dlt;
            }
            rays = ra / wsum * smoothstep(0.05, 0.4, sw) * smoothstep(-0.03, 0.03, g_L.y);
        }
        gl_FragColor = vec4(acc / 6.4, rays);
    } else {
        vec2 st = fragCoord / R;
        vec2 asp = vec2(R.y / R.x, 1.0);
        vec4 c0 = texture2D(ocScene, st);
        vec3 col = c0.rgb;
        // chromatic fringe toward the frame edge
        vec2 dir = (st - 0.5) * 2.0;
        float ab = aberration * 0.003 * smoothstep(0.35, 1.1, length(dir));
        vec3 fr = vec3(texture2D(ocScene, st + dir * ab).r, c0.g, texture2D(ocScene, st - dir * ab).b);
        col = mix(col, fr, 0.8);
        // spray: mist lifted off the foaming crests and blown downwind
        float sp = 0.0;
        float wx = 0.012 * sin(g_wind);
        vec2 sd2 = st * vec2(R.x / R.y, 1.0);
        float puff = vnoise2(sd2 * 26.0 + vec2(TIME * 0.9, -TIME * 0.5)) * vnoise2(sd2 * 95.0 + vec2(TIME * 2.4, -TIME * 1.3));
        float jit = vnoise2(sd2 * vec2(60.0, 12.0) + vec2(TIME * 1.7, 0.0)) - 0.5;
        for (int j = 1; j <= 6; j++) {
            float fj = float(j) * 2.0;
            vec2 o = vec2(-wx * fj * 0.3 + jit * 0.004 * fj / 12.0, -fj * 0.0022);
            vec4 s = texture2D(ocScene, st + o);
            float fs = max(s.a - 0.5, 0.0) * 2.0;
            sp += fs * (1.0 - fj / 13.0) * 1.2;
        }
        // mist reads only in the air / beyond the crest, never smeared over foam or a near face
        sp *= (1.0 - smoothstep(0.5, 0.7, c0.a)) * (0.5 + 0.5 * step(c0.a, 0.25));
        sp *= (0.15 + 2.2 * puff) * spray * (1.0 + 1.6 * g_wS) * (1.0 + 0.8 * g_ar * (g_bass * 0.5 + g_beat));
        col += mix(vec3(0.7, 0.8, 0.88) * (0.25 + 0.6 * lum(g_hor)), g_sunC, 0.35) * sp * 0.16;
        // god rays (computed at quarter res in the bloom pass, alpha channel)
        col += g_sunC * texture2D(ocBloom, st).a * godRays * (0.25 * g_wG + 1.2 * g_wS) * g_sunI;
        // bloom: two tent rings over the quarter-res prefilter (bilinear → no tap dots)
        vec2 bpx = 4.0 / R;
        vec3 bl = texture2D(ocBloom, st).rgb * 0.25;
        vec3 bw = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float ang = float(i) * 0.785398 + 0.39;
            vec2 dv = vec2(cos(ang), sin(ang));
            bl += texture2D(ocBloom, st + dv * bpx * 1.0).rgb * 0.094;
            bw += texture2D(ocBloom, st + dv * bpx * 3.5).rgb * 0.08 + texture2D(ocBloom, st + dv * bpx * 8.0).rgb * 0.045;
        }
        // anamorphic streak from the sun and glitter
        vec3 an = vec3(0.0);
        for (int i = -8; i <= 8; i++) {
            float fi = float(i);
            an += texture2D(ocBloom, st + vec2(fi * bpx.x * 3.0, 0.0)).rgb * exp(-abs(fi) * 0.28);
        }
        col += (bl * 1.4 + bw * 1.1) * bloom + an * vec3(0.55, 0.7, 1.0) * 0.045 * bloom;
        // music warms the light: the sunset breathes brighter with bass and level
        col *= 1.0 + g_ar * (0.42 * g_bass + 0.16 * g_lev + 0.2 * clamp(audioMid, 0.0, 1.0));
        col = aces(col * 1.1);
        float l0 = lum(col);
        col = clamp(mix(vec3(l0), col, saturation), 0.0, 1.0);
        float g1 = (hash12(fragCoord + fract(TIME) * 61.0) - 0.5) * 0.05;
        col += g1 * grain * (0.5 + 0.5 * (1.0 - abs(l0 - 0.5) * 2.0));
        col *= 1.0 - 0.13 * smoothstep(0.5, 1.4, length(uv));
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
        float lumHD = lum(col);
        col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
        float alphaOut = transparentBg ? smoothstep(0.2, 0.5, c0.a) : 1.0;
        gl_FragColor = vec4(col, alphaOut);
    }
}
