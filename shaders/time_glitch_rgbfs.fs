/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Standalone 3D raymarched temporal echo wormhole — concentric torus rings representing time-delayed copies of space collapsing inward through a black-hole-like distortion field. Cinematic HDR neon lighting.",
  "CREDIT": "ShaderClaw auto-improve 2026-05-05",
  "CATEGORIES": [
    "Generator",
    "Glitch",
    "3D"
  ],
  "INPUTS": [
    {
      "NAME": "audioReact",
      "LABEL": "Audio React",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 1.0,
      "DEFAULT": 0.5
    },
    {
      "NAME": "camDist",
      "LABEL": "Camera Distance",
      "TYPE": "float",
      "MIN": 1.5,
      "MAX": 8.0,
      "DEFAULT": 3.5
    },
    {
      "NAME": "ringSpeed",
      "LABEL": "Ring Speed",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 3.0,
      "DEFAULT": 0.6
    },
    {
      "NAME": "ringCount",
      "LABEL": "Ring Count",
      "TYPE": "float",
      "MIN": 2.0,
      "MAX": 12.0,
      "DEFAULT": 6.0
    },
    {
      "NAME": "keyColor",
      "LABEL": "Key Light Color",
      "TYPE": "color",
      "DEFAULT": [0.15, 0.85, 1.0, 1.0]
    },
    {
      "NAME": "fillColor",
      "LABEL": "Fill Light Color",
      "TYPE": "color",
      "DEFAULT": [1.0, 0.05, 0.75, 1.0]
    },
    {
      "NAME": "exposure",
      "LABEL": "Exposure",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 4.0,
      "DEFAULT": 1.4
    }
  ]
}*/

// ─── Constants ────────────────────────────────────────────────────────────────
#define MAX_STEPS 64
#define SURF_DIST  0.0015
#define MAX_DIST   18.0
#define PI         3.14159265359
#define TAU        6.28318530718

// ─── Utilities ────────────────────────────────────────────────────────────────
mat2 rot2(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

// Classic hash for noise
float hash1(float n) { return fract(sin(n) * 43758.5453123); }
float hash1v(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123); }

// ─── SDF: Torus (major radius R, minor radius r) ──────────────────────────────
float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

// ─── Audio modulator — always alive at silence ─────────────────────────────
float audioMod() {
    // audioBass is ISF built-in; guard with uniform presence fallback = 0.0
    float bass = clamp(audioBass, 0.0, 1.0);
    return 0.5 + 0.5 * bass * audioReact;
}

// ─── Scene SDF ────────────────────────────────────────────────────────────────
//  Domain-repeat torus rings along Z, with per-ring twist and radial pulse.
//  Each ring is a slightly rotated torus. The time-warp effect comes from
//  each ring pulsing inward with a phase offset proportional to its index.
float sceneSDF(vec3 p) {
    float aMod   = audioMod();
    float t      = TIME * ringSpeed;

    // Slow whole-scene spiral twist around Z
    float twist  = p.z * 0.18;
    p.xy *= rot2(twist + t * 0.07);

    float d = MAX_DIST;
    int   n = int(clamp(ringCount, 2.0, 12.0));

    for (int i = 0; i < 12; i++) {
        if (i >= n) break;

        float fi    = float(i);
        float phase = fi * (TAU / float(n));

        // Domain repeat along Z: space rings every 2.6 units
        float zPeriod = 2.6;
        vec3  rp      = p;
        // Bring Z into [-zPeriod/2, zPeriod/2] centered at ring's Z slot
        float zSlot   = fi * zPeriod - 0.5 * float(n - 1) * zPeriod;
        rp.z         -= zSlot;
        // Fold for infinite repetition beyond the explicit ring set
        rp.z          = mod(rp.z + zPeriod * 0.5, zPeriod) - zPeriod * 0.5;

        // Per-ring tilt (XZ and YZ planes), creates the "nested but rotated" look
        rp.xz *= rot2(fi * 0.35 + t * (0.13 + fi * 0.04));
        rp.yz *= rot2(fi * 0.22 + t * (0.09 - fi * 0.03));

        // Major radius: large outer rings shrink toward a hot inner core
        float R = 0.65 + fi * 0.28;

        // Pulse amplitude driven by audio; TIME-driven baseline so it moves at silence
        float pulse = 0.10 * sin(t * 1.3 + phase) * (0.6 + 0.4 * aMod)
                    + 0.05 * sin(t * 2.7 - phase * 1.5);
        R          += pulse;

        // Minor radius: thin rings with a slight per-ring taper
        float r = 0.045 + fi * 0.006 + 0.02 * aMod * abs(sin(t * 0.9 + phase));

        d = min(d, sdTorus(rp, R, r));
    }
    return d;
}

// ─── Analytic normal via tetrahedron sampling ──────────────────────────────
vec3 calcNormal(vec3 p) {
    const vec2 e = vec2(0.0012, -0.0012);
    return normalize(
        e.xyy * sceneSDF(p + e.xyy) +
        e.yyx * sceneSDF(p + e.yyx) +
        e.yxy * sceneSDF(p + e.yxy) +
        e.xxx * sceneSDF(p + e.xxx)
    );
}

// ─── Raymarch ─────────────────────────────────────────────────────────────────
float raymarch(vec3 ro, vec3 rd) {
    float t = 0.0;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3  p  = ro + rd * t;
        float d  = sceneSDF(p);
        if (d < SURF_DIST) return t;
        if (t > MAX_DIST)  return -1.0;
        // Adaptive step: tighten near surfaces for sharpness
        t += d * 0.82;
    }
    return -1.0;
}

// ─── Ambient occlusion (soft) ──────────────────────────────────────────────
float calcAO(vec3 p, vec3 n) {
    float ao  = 0.0;
    float sca = 1.0;
    for (int i = 0; i < 5; i++) {
        float h  = 0.01 + 0.15 * float(i) / 4.0;
        float d  = sceneSDF(p + h * n);
        ao      += (h - d) * sca;
        sca     *= 0.7;
    }
    return clamp(1.0 - 3.0 * ao, 0.0, 1.0);
}

// ─── HDR colour palette ────────────────────────────────────────────────────
//  palette(t) maps [0,1] → electric cyan → white-hot core → magenta echo
vec3 ringPalette(float t, float ringIdx, float aMod) {
    // Base cyan-to-white at core; magenta at ring idx offset
    vec3 cyan    = vec3(0.05, 0.9,  1.0);
    vec3 white   = vec3(1.2,  1.3,  1.5);   // intentional HDR > 1
    vec3 magenta = vec3(1.1,  0.05, 0.95);

    // Blend between cyan and magenta based on ring index
    float mixFac = fract(ringIdx * 0.37 + aMod * 0.3);
    vec3  base   = mix(cyan, magenta, mixFac);

    // Hot core: lerp toward white as t→0 (surface proximity)
    vec3  col    = mix(white, base, smoothstep(0.0, 0.25, t));
    return col;
}

// ─── Background: deep void with faint star-like noise ─────────────────────
vec3 voidBackground(vec3 rd) {
    // Slight purple-to-black gradient
    float up  = clamp(rd.y * 0.5 + 0.5, 0.0, 1.0);
    vec3  bg  = mix(vec3(0.0), vec3(0.04, 0.0, 0.08), up);
    // Sparse hot pixel stars
    vec2  sp  = vec2(atan(rd.z, rd.x), asin(rd.y)) * (1.0 / PI);
    float star = hash1v(floor(sp * 180.0));
    bg        += vec3(1.4, 1.2, 1.6) * smoothstep(0.985, 1.0, star) * 0.6;
    return bg;
}

// ─── Main ─────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = (isf_FragNormCoord * 2.0 - 1.0);
    uv.x    *= RENDERSIZE.x / RENDERSIZE.y;   // correct aspect

    // ── Camera ────────────────────────────────────────────────────────────
    float slowOrbit = TIME * 0.08;
    vec3  ro = vec3(
        sin(slowOrbit) * camDist,
        cos(slowOrbit * 0.7) * 0.9,
        cos(slowOrbit) * camDist
    );
    vec3  target = vec3(0.0, 0.0, -1.5);      // aim slightly into tunnel
    vec3  fwd    = normalize(target - ro);
    vec3  rgt    = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3  up     = cross(fwd, rgt);

    float fov    = 0.75;                       // ~42° — cinematic telephoto-ish
    vec3  rd     = normalize(fwd + uv.x * rgt * fov + uv.y * up * fov);

    // ── Raymarch ──────────────────────────────────────────────────────────
    float hit = raymarch(ro, rd);

    vec3  col;
    float aMod = audioMod();

    if (hit < 0.0) {
        col = voidBackground(rd);
    } else {
        vec3  p  = ro + rd * hit;
        vec3  n  = calcNormal(p);
        float ao = calcAO(p, n);

        // Identify closest ring for palette selection
        // (use p.z position as a proxy for ring index)
        float ringIdx = mod(p.z / 2.6 + 32.0, 12.0);

        // Surface-to-camera distance fraction (0=close, 1=far)
        float tFrac = hit / MAX_DIST;

        // Base palette colour
        vec3  matCol = ringPalette(tFrac, ringIdx, aMod);

        // ── Cinematic lighting ─────────────────────────────────────────
        // Key: cool electric light from camera-right
        vec3  keyDir  = normalize(rgt + vec3(0.0, 0.3, 0.2));
        float keyDiff = max(dot(n, keyDir), 0.0);
        float keySpec = pow(max(dot(reflect(-keyDir, n), -rd), 0.0), 28.0);
        vec3  keyCol  = keyColor.rgb * (keyDiff * 1.4 + keySpec * 2.2);

        // Fill: neon magenta from camera-left and below
        vec3  fillDir  = normalize(-rgt + vec3(0.0, -0.4, 0.3));
        float fillDiff = max(dot(n, fillDir), 0.0) * 0.55;
        vec3  fillCol  = fillColor.rgb * fillDiff * 0.9;

        // Rim: subtle back-lighting to separate rings from void
        vec3  rimDir   = normalize(-fwd + vec3(0.0, 0.6, 0.0));
        float rimDiff  = pow(max(dot(n, rimDir), 0.0), 2.5) * 0.45;
        vec3  rimCol   = vec3(0.4, 0.9, 1.0) * rimDiff;

        // SSS-like glow: inner torus surfaces glow magenta/cyan through the tube
        float sss = exp(-hit * 0.28) * (0.5 + 0.5 * aMod);
        vec3  sssCol = mix(vec3(0.05, 0.9, 1.0), vec3(1.0, 0.05, 0.8), fract(ringIdx * 0.43)) * sss * 0.65;

        // Combine lighting — HDR intentional (no clamp before output)
        col  = matCol * (keyCol + fillCol + rimCol) + sssCol;
        col *= ao;

        // fwidth-based AA on SDF edge — smooth the hard surface boundary
        float sdfEdge   = sceneSDF(p);
        float edgeSmooth = 1.0 - smoothstep(0.0, fwidth(sdfEdge) * 3.0, abs(sdfEdge));
        // Boost edge glow for glitch/neon feel
        col += mix(vec3(0.0), vec3(0.8, 1.4, 2.0) * aMod, edgeSmooth * 0.35);

        // HDR peaks: core areas at 1.8–2.5, edges at 1.2–1.5
        float coreMask = exp(-sdfEdge * 60.0);
        col += vec3(1.8, 2.0, 2.5) * coreMask * (0.55 + 0.45 * aMod);

        // Colour temperature shift with audio: warm the fill at high bass
        float bassWarm = clamp(audioBass * audioReact, 0.0, 1.0);
        col = mix(col, col * vec3(1.0, 0.85, 0.65) * 1.3, bassWarm * 0.4);
    }

    // ── Exposure — linear HDR out, host applies ACES ───────────────────
    col *= exposure;

    // Alpha = 1 (opaque generator)
    gl_FragColor = vec4(col, 1.0);
}
