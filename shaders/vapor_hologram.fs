/*{
  "DESCRIPTION": "Vaporwave Hologram 3D — twin suns sinking into a Tron grid horizon, hot pink to magenta to indigo sky, with 3-5 raymarched SDF primitives (cube, sphere, pyramid, torus) drifting through the scene on independent orbits. No focal element at the optical center. Scanlines, chromatic aberration, HDR peaks on sun discs, grid lines, and sun reflections off the chrome primitives. Single-pass, LINEAR HDR, no internal tonemap. Alive in silence — TIME-driven, audio amplifies.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "Easel — vaporwave_hologram_3d",
  "INPUTS": [
    { "NAME": "horizonY",      "LABEL": "Horizon",       "TYPE": "float", "MIN": 0.40, "MAX": 0.75, "DEFAULT": 0.55 },
    { "NAME": "skyZenith",     "LABEL": "Sky Zenith",    "TYPE": "color", "DEFAULT": [0.18, 0.05, 0.55, 1.0] },
    { "NAME": "skyMid",        "LABEL": "Sky Mid",       "TYPE": "color", "DEFAULT": [0.85, 0.18, 0.62, 1.0] },
    { "NAME": "skyHorizon",    "LABEL": "Sky Horizon",   "TYPE": "color", "DEFAULT": [1.0, 0.42, 0.71, 1.0] },
    { "NAME": "sunSize",       "LABEL": "Sun Size",      "TYPE": "float", "MIN": 0.05, "MAX": 0.40, "DEFAULT": 0.18 },
    { "NAME": "sunSplit",      "LABEL": "Twin Spread",   "TYPE": "float", "MIN": 0.0,  "MAX": 0.40, "DEFAULT": 0.20 },
    { "NAME": "sunBars",       "LABEL": "Sun Bars",      "TYPE": "float", "MIN": 0.0,  "MAX": 12.0, "DEFAULT": 5.0 },
    { "NAME": "sunHDR",        "LABEL": "Sun HDR Peak",  "TYPE": "float", "MIN": 1.0,  "MAX": 8.0,  "DEFAULT": 3.5 },
    { "NAME": "gridDensity",   "LABEL": "Grid Density",  "TYPE": "float", "MIN": 4.0,  "MAX": 24.0, "DEFAULT": 12.0 },
    { "NAME": "gridPersp",     "LABEL": "Grid Persp.",   "TYPE": "float", "MIN": 0.5,  "MAX": 4.0,  "DEFAULT": 1.8 },
    { "NAME": "gridSpeed",     "LABEL": "Grid Speed",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.25 },
    { "NAME": "gridHDR",       "LABEL": "Grid HDR Peak", "TYPE": "float", "MIN": 1.0,  "MAX": 6.0,  "DEFAULT": 2.4 },
    { "NAME": "objCount",      "LABEL": "3D Object Count","TYPE": "float", "MIN": 3.0, "MAX": 5.0,  "DEFAULT": 4.0 },
    { "NAME": "objSpread",     "LABEL": "3D Spread",     "TYPE": "float", "MIN": 0.5,  "MAX": 3.0,  "DEFAULT": 1.6 },
    { "NAME": "objScale",      "LABEL": "3D Scale",      "TYPE": "float", "MIN": 0.05, "MAX": 0.50, "DEFAULT": 0.20 },
    { "NAME": "chromaticAb",   "LABEL": "Chromatic Ab.", "TYPE": "float", "MIN": 0.0,  "MAX": 0.04, "DEFAULT": 0.010 },
    { "NAME": "scanFreq",      "LABEL": "Scanlines",     "TYPE": "float", "MIN": 0.0,  "MAX": 4.0,  "DEFAULT": 1.6 },
    { "NAME": "scanDepth",     "LABEL": "Scanline Depth","TYPE": "float", "MIN": 0.0,  "MAX": 0.4,  "DEFAULT": 0.12 },
    { "NAME": "audioReact",    "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
// VAPORWAVE HOLOGRAM 3D
// Single-pass. LINEAR HDR out (no tonemap). Twin suns + Tron grid floor +
// 3-5 raymarched chrome primitives drifting on independent orbits, never
// occupying the optical centre. Chromatic aberration + scanlines applied
// last, in linear space.
// ════════════════════════════════════════════════════════════════════════

#define MAX_STEPS 64
#define MAX_DIST  18.0
#define EPS       0.0015
#define PI        3.14159265

// ── hash / utility ──────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
vec2  hash21(float n) { return fract(sin(vec2(n, n + 1.7)) * vec2(43758.5453, 22578.1459)); }
mat2  rot2(float a)   { float c=cos(a), s=sin(a); return mat2(c,-s,s,c); }

// ── SDF library ─────────────────────────────────────────────────────────
float sdSphere(vec3 p, float r) { return length(p) - r; }
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}
float sdTorus(vec3 p, vec2 t) {
    vec2 q = vec2(length(p.xz) - t.x, p.y);
    return length(q) - t.y;
}
// Square-base pyramid (apex up, base centred at origin)
float sdPyramid(vec3 p, float h) {
    float m2 = h * h + 0.25;
    p.xz = abs(p.xz);
    p.xz = (p.z > p.x) ? p.zx : p.xz;
    p.xz -= 0.5;
    vec3 q = vec3(p.z, h * p.y - 0.5 * p.x, h * p.x + 0.5 * p.y);
    float s = max(-q.x, 0.0);
    float t = clamp((q.y - 0.5 * p.z) / (m2 + 0.25), 0.0, 1.0);
    float a = m2 * (q.x + s) * (q.x + s) + q.y * q.y;
    float b = m2 * (q.x + 0.5 * t) * (q.x + 0.5 * t) + (q.y - m2 * t) * (q.y - m2 * t);
    float d2 = min(q.y, -q.x * m2 - q.y * 0.5) > 0.0 ? 0.0 : min(a, b);
    return sqrt((d2 + q.z * q.z) / m2) * sign(max(q.z, -p.y));
}

// ── object placement (orbits, never near center) ───────────────────────
// Each object i in [0..4]: independent orbit radius, angle, height.
// Ring layout pushed off-center so they drift through edges/quadrants.
vec3 objCenter(int i, float bass) {
    float fi = float(i);
    float ang   = TIME * (0.18 + hash11(fi * 1.9) * 0.32) + fi * (PI * 2.0 / 5.0);
    float radius = objSpread * (0.9 + 0.4 * hash11(fi * 7.3));
    // bias each orbit centre off the optical axis so the focal point stays empty
    vec2 bias = vec2(
        cos(fi * 1.31 + 0.7) * 0.55 * objSpread,
        sin(fi * 0.97 + 1.4) * 0.30 * objSpread
    );
    float height = 0.25 + sin(TIME * (0.5 + hash11(fi * 3.1) * 0.7) + fi * 1.7) * 0.45;
    height += bass * 0.15;
    return vec3(bias.x + cos(ang) * radius, height, bias.y + sin(ang) * radius);
}

// returns (dist, matID encoded in fract)
// matID: 0=sphere chrome-pink, 1=cube chrome-cyan, 2=pyramid chrome-magenta, 3=torus chrome-violet
vec2 mapObjects(vec3 p, float bass) {
    int N = int(clamp(objCount, 3.0, 5.0));
    float best = 1e9;
    float matID = 0.0;
    for (int i = 0; i < 5; i++) {
        if (i >= N) break;
        float fi = float(i);
        vec3 c = objCenter(i, bass);
        vec3 q = p - c;
        // per-object spin
        float spinY = TIME * (0.6 + hash11(fi * 5.2) * 1.2) + fi;
        float spinX = TIME * (0.4 + hash11(fi * 8.7) * 0.9);
        q.xz = rot2(spinY) * q.xz;
        q.yz = rot2(spinX) * q.yz;
        float scl = objScale * (0.7 + hash11(fi * 11.3) * 0.7) * (1.0 + bass * 0.18);
        int kind = int(mod(fi, 4.0));
        float d;
        if      (kind == 0) d = sdSphere(q, scl);
        else if (kind == 1) d = sdBox(q, vec3(scl * 0.85));
        else if (kind == 2) d = sdPyramid(q / scl, 1.1) * scl;
        else                d = sdTorus(q, vec2(scl, scl * 0.32));
        if (d < best) { best = d; matID = float(kind); }
    }
    return vec2(best, matID);
}

vec3 calcNormal(vec3 p, float bass) {
    vec2 e = vec2(EPS, 0.0);
    return normalize(vec3(
        mapObjects(p + e.xyy, bass).x - mapObjects(p - e.xyy, bass).x,
        mapObjects(p + e.yxy, bass).x - mapObjects(p - e.yxy, bass).x,
        mapObjects(p + e.yyx, bass).x - mapObjects(p - e.yyx, bass).x
    ));
}

// ── sky / sun / grid (procedural background) ───────────────────────────
vec3 skyColor(vec2 uv) {
    // hot pink horizon → magenta mid → indigo zenith
    float t = clamp((uv.y - horizonY) / max(1.0 - horizonY, 0.01), 0.0, 1.0);
    vec3 a = mix(skyHorizon.rgb, skyMid.rgb,    smoothstep(0.0, 0.55, t));
    vec3 b = mix(a,              skyZenith.rgb, smoothstep(0.45, 1.0, t));
    return b;
}

// returns linear HDR radiance for the twin sun discs
vec3 twinSun(vec2 uv, float aspect, float bass) {
    vec3 acc = vec3(0.0);
    float sr = sunSize * (1.0 + bass * 0.08);
    for (int s = -1; s <= 1; s += 2) {
        vec2 sc = vec2(0.5 + float(s) * sunSplit, horizonY + sr * 0.05);
        vec2 sd = uv - sc; sd.x *= aspect;
        float r = length(sd);
        // disc with soft edge
        float disc = smoothstep(sr, sr * 0.92, r);
        // vertical gradient inside disc (orange bottom -> magenta top)
        float ty = clamp((sd.y / sr + 1.0) * 0.5, 0.0, 1.0);
        vec3 sunC = mix(vec3(1.0, 0.55, 0.18), vec3(1.0, 0.22, 0.62), ty);
        // horizontal bars
        if (sunBars > 0.5) {
            float barY = sd.y / sr;
            float bar = step(0.0, sin(barY * sunBars * PI + 0.4 + TIME * 0.5));
            // bars cut to dark, leaving radial slivers
            sunC *= mix(0.25, 1.0, bar);
        }
        // HDR peak boost — bright disc
        vec3 hdr = sunC * sunHDR;
        // outer halo (soft glow) — also linear additive
        float halo = exp(-r * r * 18.0) * 0.6 + exp(-r * r * 90.0) * 1.2;
        acc += hdr * disc + sunC * halo * 0.6;
    }
    return acc;
}

// Tron-grid floor — returns (color, lineMask) packed; alpha = lineMask
vec4 tronGrid(vec2 uv, float aspect, float bass, float mid) {
    if (uv.y >= horizonY) return vec4(0.0);
    float dh = max(horizonY - uv.y, 0.001);
    float gridU = (uv.x - 0.5) / (dh * gridPersp + 0.05);
    float gridV = 1.0 / dh - TIME * gridSpeed * (1.0 + mid * 0.4);
    float gx = abs(fract(gridU * gridDensity) - 0.5);
    float gy = abs(fract(gridV) - 0.5);
    float lineW = 0.05 * dh + 0.005;
    float line = smoothstep(0.5, 0.5 - lineW, max(gx, gy));
    // floor base — deep indigo to violet near horizon
    vec3 floorBase = mix(vec3(0.04, 0.02, 0.10), vec3(0.30, 0.06, 0.42), uv.y / horizonY);
    // line color — hot cyan-pink, HDR-bright
    vec3 lineC = mix(vec3(1.0, 0.42, 0.85), vec3(0.45, 1.0, 1.0),
                     0.5 + 0.5 * sin(gridV * 0.6));
    lineC *= gridHDR;
    // boost lines that are far (perspective) — they read as horizon glow
    float horizonFade = smoothstep(horizonY - 0.04, horizonY, uv.y);
    vec3 col = mix(floorBase, lineC, line);
    col = mix(col, skyColor(uv), horizonFade);
    return vec4(col, line * (1.0 - horizonFade));
}

// ── camera / scene compose ─────────────────────────────────────────────
void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec2 uv = fragCoord / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    float bass = clamp(audioBass, 0.0, 1.0) * audioReact;
    float mid  = clamp(audioMid,  0.0, 1.0) * audioReact;
    float high = clamp(audioHigh, 0.0, 1.0) * audioReact;

    // Background = sky + twin sun (linear HDR) + grid floor
    vec3 bgCol = skyColor(uv);
    bgCol += twinSun(uv, aspect, bass);
    vec4 grid = tronGrid(uv, aspect, bass, mid);
    bgCol = mix(bgCol, grid.rgb, step(uv.y, horizonY));

    // ── chromatic aberration sample offsets (background) ──
    // We sample sky/sun/grid at three slightly offset uv's.
    float ca = chromaticAb * (1.0 + high * 0.5);
    vec2  uvR = uv + vec2( ca, 0.0);
    vec2  uvB = uv - vec2( ca, 0.0);
    vec3 bgR = skyColor(uvR) + twinSun(uvR, aspect, bass);
    {
        vec4 gR = tronGrid(uvR, aspect, bass, mid);
        bgR = mix(bgR, gR.rgb, step(uvR.y, horizonY));
    }
    vec3 bgB = skyColor(uvB) + twinSun(uvB, aspect, bass);
    {
        vec4 gB = tronGrid(uvB, aspect, bass, mid);
        bgB = mix(bgB, gB.rgb, step(uvB.y, horizonY));
    }
    vec3 bgFinal = vec3(bgR.r, bgCol.g, bgB.b);

    // ── raymarch the 3D drifting primitives ──
    // camera looking slightly down toward horizon
    vec3 ro = vec3(0.0, 0.55, 3.4);
    vec2 ndc = (fragCoord / RENDERSIZE.xy) * 2.0 - 1.0;
    ndc.x *= aspect;
    // pitch down a touch so primitives float against sky and skim above grid
    float pitch = -0.05;
    vec3 fwd = normalize(vec3(0.0, sin(pitch), -cos(pitch)));
    vec3 right = vec3(1.0, 0.0, 0.0);
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd + right * ndc.x * 0.85 + up * ndc.y * 0.85);

    float t = 0.0;
    float matID = 0.0;
    bool hit = false;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * t;
        vec2 m = mapObjects(p, bass);
        if (m.x < EPS) { hit = true; matID = m.y; break; }
        if (t > MAX_DIST) break;
        t += m.x * 0.85;
    }

    vec3 col = bgFinal;

    if (hit) {
        vec3 p = ro + rd * t;
        vec3 n = calcNormal(p, bass);
        // light directions — twin-sun keys (warm + magenta) + cool fill from sky
        vec3 lWarm = normalize(vec3(-0.4, 0.3, 0.6));
        vec3 lCool = normalize(vec3( 0.4, 0.3, 0.6));
        vec3 fill  = normalize(vec3(0.0, 1.0, 0.2));
        float dW = max(dot(n, lWarm), 0.0);
        float dC = max(dot(n, lCool), 0.0);
        float dF = max(dot(n, fill),  0.0);

        // base colour per material — chrome with a tint
        vec3 tint;
        if      (matID < 0.5) tint = vec3(1.0, 0.55, 0.85);   // sphere — pink chrome
        else if (matID < 1.5) tint = vec3(0.45, 0.95, 1.0);   // cube   — cyan chrome
        else if (matID < 2.5) tint = vec3(1.0, 0.30, 0.75);   // pyramid— magenta chrome
        else                  tint = vec3(0.65, 0.50, 1.0);   // torus  — violet chrome

        // diffuse shading
        vec3 lit = tint * (0.18 + 1.1 * dW * vec3(1.0, 0.55, 0.32)
                                + 0.9 * dC * vec3(1.0, 0.32, 0.62)
                                + 0.35 * dF * vec3(0.55, 0.65, 1.0));

        // specular highlights — spec reflects sun positions, HDR
        vec3 v = -rd;
        vec3 hW = normalize(lWarm + v);
        vec3 hC = normalize(lCool + v);
        float spW = pow(max(dot(n, hW), 0.0), 64.0);
        float spC = pow(max(dot(n, hC), 0.0), 64.0);
        lit += spW * vec3(1.0, 0.55, 0.20) * sunHDR * 0.6;
        lit += spC * vec3(1.0, 0.25, 0.65) * sunHDR * 0.6;

        // sun-disc reflection — sample sky in the reflected direction
        vec3 r = reflect(rd, n);
        // map reflected direction to a fake "sky uv" — y from r.y, x from r.x
        vec2 reflUV = vec2(0.5 + r.x * 0.5, 0.5 + r.y * 0.5);
        reflUV = clamp(reflUV, 0.0, 1.0);
        vec3 reflSun = twinSun(reflUV, 1.0, bass);
        vec3 reflSky = skyColor(reflUV);
        lit += (reflSun + reflSky * 0.25) * 0.35;

        // fresnel rim — cyan/magenta edge bloom
        float fres = pow(1.0 - max(dot(n, v), 0.0), 3.0);
        lit += fres * mix(vec3(0.4, 1.0, 1.0), vec3(1.0, 0.4, 0.9), 0.5 + 0.5 * sin(TIME * 0.7 + matID))
             * 1.2;

        col = lit;
    }

    // ── scanlines (linear, applied to whole frame) ──
    float scan = 1.0 - scanDepth * (0.5 + 0.5 * sin(fragCoord.y * scanFreq * PI));
    col *= scan;

    // tiny vignette (very mild — keep HDR)
    vec2 vUV = uv - 0.5;
    float vig = 1.0 - dot(vUV, vUV) * 0.55;
    col *= vig;

    // alive-in-silence pulse on grid lines — slow breathing in luminance
    col += vec3(0.04, 0.02, 0.08) * (0.5 + 0.5 * sin(TIME * 0.6));

    // OUTPUT LINEAR HDR — no tonemap, no pow, no clamp
    gl_FragColor = vec4(col, 1.0);
}
