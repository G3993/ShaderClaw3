/*{
  "DESCRIPTION": "Geo — 2D path-traced circle-circle intersection. Two orbiting circles compute their algebraic intersection points in real time; a radial path tracer accumulates global illumination across the canvas, producing painterly glow against a graph-paper grid. Enhanced with organic multi-layer movement, cinematic vignette, and film grain. Ported from Yusef28's Shadertoy (7l2XDm).",
  "CREDIT": "Original: Yusef28 (Shadertoy 7l2XDm). Port & Enhancement: ShaderClaw — adapted to ISF with procedural noise, organic drift layers, cinematic post.",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",          "LABEL": "Orbit Speed",      "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "radius1",        "LABEL": "Circle A Radius",  "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.5,  "MAX": 5.0 },
    { "NAME": "radius2",        "LABEL": "Circle B Radius",  "TYPE": "float", "DEFAULT": 3.0,  "MIN": 0.5,  "MAX": 6.0 },
    { "NAME": "samples",        "LABEL": "Path Samples",     "TYPE": "long",  "DEFAULT": 68,   "VALUES": [16,32,48,68,96,128], "LABELS": ["16","32","48","68","96","128"] },
    { "NAME": "exposure",       "LABEL": "Exposure",         "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.5,  "MAX": 4.0 },
    { "NAME": "gamma",          "LABEL": "Gamma",            "TYPE": "float", "DEFAULT": 0.75, "MIN": 0.4,  "MAX": 1.4 },
    { "NAME": "vignetteAmount", "LABEL": "Vignette Strength","TYPE": "float", "DEFAULT": 0.45, "MIN": 0.0,  "MAX": 1.2 },
    { "NAME": "vignetteShape",  "LABEL": "Vignette Shape",   "TYPE": "float", "DEFAULT": 1.5,  "MIN": 0.5,  "MAX": 4.0 },
    { "NAME": "grainAmount",    "LABEL": "Film Grain",       "TYPE": "float", "DEFAULT": 0.045,"MIN": 0.0,  "MAX": 0.2 },
    { "NAME": "organicAmp",     "LABEL": "Organic Drift",    "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "driftLayers",    "LABEL": "Drift Layers",     "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0,  "MAX": 6.0 },
    { "NAME": "audioReact",     "LABEL": "Audio React",      "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "colorA",         "LABEL": "Circle A Tint",    "TYPE": "color", "DEFAULT": [0.8, 0.3, 0.7, 1.0] },
    { "NAME": "colorB",         "LABEL": "Circle B Tint",    "TYPE": "color", "DEFAULT": [0.2, 0.5, 0.9, 1.0] },
    { "NAME": "colorIntersect", "LABEL": "Intersection",     "TYPE": "color", "DEFAULT": [0.6, 0.7, 1.0, 1.0] }
  ]
}*/

// ====================================================================
// Geo — Circle-Circle Intersection with 2D Path Tracing
// Source: https://www.shadertoy.com/view/7l2XDm by Yusef28
// Enhanced with organic multi-layer movement, layered drift from
// liquid_ripples_3d concept, cinematic vignette + film grain.
// ====================================================================

float rnd(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453123);
}

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = rnd(i);
    float b = rnd(i + vec2(1.0, 0.0));
    float c = rnd(i + vec2(0.0, 1.0));
    float d = rnd(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// Multi-octave smooth noise for organic drift
float smoothNoise(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    for (int o = 0; o < 4; o++) {
        v += a * vnoise(p);
        p  = p * 2.13 + vec2(1.7, 9.3);
        a *= 0.5;
    }
    return v;
}

// Per-layer hashed drift anchor (inspired by liquid_ripples_3d)
vec2 driftAnchor(int L) {
    float f = float(L);
    return vec2(rnd(vec2(f, 1.7)), rnd(vec2(f, 9.3))) - 0.5;
}

// Compute cumulative organic displacement from N drift layers
vec2 organicOffset(vec2 p, float t, float amp, float nLayers) {
    vec2 offset = vec2(0.0);
    float totalW = 0.0;
    for (int L = 0; L < 6; L++) {
        if (float(L) >= nLayers) break;
        float depth = float(L) / max(nLayers, 1.0);
        float weight = 1.0 - depth * 0.5;  // front layers contribute more
        vec2 anchor = driftAnchor(L);
        float lspeed = 0.23 + float(L) * 0.11;
        vec2 wobble = vec2(
            smoothNoise(p * (0.4 + depth * 0.3) + anchor + vec2(t * lspeed, float(L) * 3.7)),
            smoothNoise(p * (0.4 + depth * 0.3) + anchor + vec2(float(L) * 5.1, t * lspeed * 1.3))
        );
        // Map [0,1] → [-1,1]
        wobble = wobble * 2.0 - 1.0;
        offset += wobble * weight;
        totalW += weight;
    }
    return (offset / max(totalW, 0.001)) * amp;
}

void addObj(float dist, vec3 color, inout float endDist, inout vec3 endColor) {
    if (dist < endDist) {
        endDist = dist;
        endColor = color;
    }
}

void mapScene(vec2 uv, float t, inout float d, inout vec3 color) {
    vec2 center1 = vec2(-4.0 * cos(t * 1.0), -3.0 * sin(t * 2.0));
    vec2 center2 = vec2( 0.8 * sin(t),         0.5 * cos(t));

    float c1 = radius1;
    float c2 = radius2;

    float h1 = center1.x, k1 = center1.y;
    float h2 = center2.x, k2 = center2.y;

    float k3 = k1 - k2;
    float h3 = h1 - h2;
    float c3 = k1*k1 - (k2*k2) + h1*h1 - (h2*h2) - (c1*c1) + (c2*c2);
    float w = -(k3 / h3);
    float u = -c3 / (2.0 * h3);

    float a  = w*w + 1.0;
    float b  = 2.0 * (w * h1 + w * u + k1);
    float cc = (u*u) + 2.0 * u * h1 + k1*k1 - (c1*c1) + h1*h1;

    float disc = b*b - 4.0 * a * cc;
    float y1 = (-b + sqrt(max(disc, 0.0))) / (2.0 * a);
    float y2 = (-b - sqrt(max(disc, 0.0))) / (2.0 * a);
    float x1 = y1 * w + u;
    float x2 = y2 * w + u;
    vec2 ip1 = -vec2(x1, y1);
    vec2 ip2 = -vec2(x2, y2);

    d = 1e9;
    color = vec3(0.0);
    float f;

    f = abs(length(uv - center1) - c1);
    addObj(f, colorA.rgb / 4.0, d, color);

    if (length(uv - center1) > c1) {
        f = abs(length(uv - center2) - c2);
        addObj(f, colorB.rgb / 3.0, d, color);
    }

    if (disc >= 0.0) {
        f = abs(length(uv - ip1) - 0.12);
        addObj(f, colorIntersect.rgb * 2.0, d, color);
        f = abs(length(uv - ip2) - 0.12);
        addObj(f, colorIntersect.rgb * 2.0, d, color);
    }

    f = abs(length(uv - vec2( 7.0, 4.0 * sin(t))) - 0.5);
    addObj(f, vec3(1.0, 1.0, 0.7), d, color);
    f = abs(length(uv + vec2( 7.0, 4.0 * cos(t))) - 0.5);
    addObj(f, vec3(1.4, 0.9, 0.5), d, color);
}

float trace(vec2 ro, vec2 rd, float t, inout vec3 color, vec3 grid) {
    float tt = 0.0;
    for (int i = 0; i < 30; i++) {
        float d;
        mapScene(ro + rd * tt, t, d, color);
        if (d < 0.0001 || tt > 10.0) break;
        tt += d;
    }
    if (tt > 10.0) color = grid;
    return tt;
}

void main() {
    vec2 res = RENDERSIZE;
    float audio = clamp(audioReact, 0.0, 2.0);

    // Time with organic micro-breathing from audio mid
    float breathe = 1.0 + audioMid * audio * 0.12 * sin(TIME * 3.7);
    float t = TIME * (speed + audioBass * audio * 0.4) * breathe;

    // Normalised screen coord
    vec2 fragNorm = gl_FragCoord.xy / res;
    vec2 centred  = (gl_FragCoord.xy - res * 0.5) / res.y;

    // ── Organic position drift applied to rendering UV ──
    // Scale organic offset so it's subtle in world-space units
    float driftScale = organicAmp * 0.018;
    vec2 drift = organicOffset(centred, TIME * 0.55, driftScale, driftLayers);
    // Additional bass-driven pulse
    drift += vec2(
        sin(TIME * 1.3 + audioMid * 4.0),
        cos(TIME * 1.1 + audioHigh * 3.0)
    ) * audioBass * audio * 0.008;

    // ── Graph paper background ──
    vec2 st = centred + drift * 0.4;   // drift background slightly less than scene
    vec2 uvGrid = st;
    uvGrid *= 8.0;
    vec3 col = vec3(0.0);
    col = mix(col, vec3(0.16), 1.0 - length(uvGrid / 8.0));
    float tex = vnoise(st * 80.0 + vec2(13.0, 7.0));
    col = mix(col, vec3(0.25), pow(tex, 2.0));
    vec2 lines = fract(uvGrid * 5.0);
    lines = smoothstep(vec2(0.45), vec2(0.52), abs(lines - 0.5));
    col = mix(col, vec3(0.3), lines.x);
    col = mix(col, vec3(0.3), lines.y);
    lines = fract(uvGrid);
    lines = smoothstep(vec2(0.47), vec2(0.52), abs(lines - 0.5));
    col = mix(col, vec3(0.5), lines.x);
    col = mix(col, vec3(0.5), lines.y);
    lines = smoothstep(vec2(0.0), vec2(0.02), abs(uvGrid));
    col = mix(col, vec3(0.6), 1.0 - lines.x);
    col = mix(col, vec3(0.6), 1.0 - lines.y);

    vec3 grid = col / 2.0;

    // ── Path-trace pass (world space UV with organic drift) ──
    vec2 uv = (gl_FragCoord.xy - res * 0.5) / res.y * 10.0;
    // Apply organic drift in world-space units (scale up to match *10 zoom)
    uv += drift * 10.0;

    vec2 ro = uv;
    vec2 rd;
    vec3 tmpColor;
    vec3 marchColor = vec3(0.0);

    int N = samples;
    if (N < 4)   N = 4;
    if (N > 256) N = 256;
    float Nf = float(N);

    for (int i = 0; i < 256; i++) {
        if (i >= N) break;
        float fi = float(i);
        float angle = (fi + rnd(uv + fi)) / Nf * 3.1415 * 2.0;
        rd = vec2(cos(angle), sin(angle));
        trace(ro, rd, t, tmpColor, grid);
        marchColor += tmpColor;
    }
    marchColor /= Nf;
    col = marchColor * exposure;

    // ── Gamma ──
    col = pow(max(col, 0.0), vec3(gamma));

    // ── Cinematic vignette — smooth oval darkening ──
    // Two-term: a broad soft falloff + a tighter inner term
    vec2 vUV = fragNorm - 0.5;
    // Aspect-correct ellipse
    vUV.x *= res.x / res.y;
    float vDist = length(vUV);
    float vig = 1.0 - pow(clamp(vDist * vignetteShape, 0.0, 1.0), 2.2) * vignetteAmount;
    // Second softer pass for cinematic depth
    float vig2 = 1.0 - pow(clamp(vDist * vignetteShape * 0.6, 0.0, 1.0), 3.5) * vignetteAmount * 0.4;
    vig = vig * vig2;
    col *= vig;

    // ── Film grain ──
    // Temporal grain: changes every frame using FRAMEINDEX + spatial hash
    float grainT = float(FRAMEINDEX) * 0.137 + TIME * 13.7;
    float grain = rnd(gl_FragCoord.xy * 0.5 + vec2(grainT * 17.3, grainT * 31.1));
    // Remap to [-1, 1] and bias toward highlights (cinematic look)
    grain = (grain - 0.5) * 2.0;
    // Grain is more visible in midtones, quieter in shadows/highlights
    float lum = dot(col, vec3(0.2126, 0.7152, 0.0722));
    float grainMask = 4.0 * lum * (1.0 - lum);   // bell curve peaking at 0.5
    col += grain * grainAmount * grainMask;

    // ── Subtle chromatic micro-shift from organic drift (cinematic) ──
    // Very mild — just enough to feel like a film lens breathing
    float caAmt = length(drift) * 0.3 + audioBass * audio * 0.004;
    vec2 caOff = normalize(centred + vec2(0.001)) * caAmt;
    // We can't resample ourselves, so approximate with a colour channel split
    // by modulating R and B slightly with luminance and drift direction
    col.r = col.r + dot(caOff, vec2(0.5, 0.3)) * col.r * 0.4;
    col.b = col.b - dot(caOff, vec2(0.3, 0.5)) * col.b * 0.4;

    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}