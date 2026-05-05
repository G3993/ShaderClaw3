/*{
  "DESCRIPTION": "Lissajous Chromatic Web — multiple animated Lissajous parametric curves as thick neon lines. Crimson, electric blue, gold, lime on void black.",
  "CREDIT": "ShaderClaw auto-improve v6",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "lineWidth",   "LABEL": "Line Width",  "TYPE": "float", "DEFAULT": 0.008, "MIN": 0.002, "MAX": 0.04 },
    { "NAME": "phaseSpeed",  "LABEL": "Phase Speed", "TYPE": "float", "DEFAULT": 0.3,   "MIN": 0.0,   "MAX": 1.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.8,   "MIN": 1.0,   "MAX": 4.0 },
    { "NAME": "audioReact",  "LABEL": "Audio",       "TYPE": "float", "DEFAULT": 0.7,   "MIN": 0.0,   "MAX": 2.0 }
  ]
}*/

// Sample 100 points along a Lissajous curve and return minimum distance
// from the current fragment (uv in [-1,1] aspect-corrected space) to the curve.
// a, b: frequency integers; phi: phase offset; scale: half-size in uv space
float lissajousDist(vec2 frag, float a, float b, float phi, float scale) {
    float minDist = 1e6;
    for (int k = 0; k < 100; k++) {
        float s  = (float(k) / 99.0) * 6.28318530718;
        vec2  pt = vec2(sin(a * s + phi), sin(b * s)) * scale;
        float d  = length(frag - pt);
        if (d < minDist) minDist = d;
    }
    return minDist;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    // Audio: scales line width for pulse effect
    float audio = 1.0 + audioLevel * audioReact + audioBass * audioReact * 0.4;
    float lw    = lineWidth * audio;

    // Void black background
    vec3 col = vec3(0.0, 0.0, 0.01);

    // ── Curve 1: 3:2, crimson ─────────────────────────────────────────
    {
        float phi1  = TIME * phaseSpeed * 0.7;
        float d1    = lissajousDist(uv, 3.0, 2.0, phi1, 0.88);
        float line1 = smoothstep(lw, lw * 0.3, d1);
        float halo1 = exp(-d1 * 60.0) * 0.4;
        vec3  c1    = vec3(1.0, 0.02, 0.08);   // crimson
        col += c1 * (line1 + halo1) * hdrPeak;
    }

    // ── Curve 2: 5:4, electric blue ──────────────────────────────────
    {
        float phi2  = TIME * phaseSpeed * 0.5 + 1.1;
        float d2    = lissajousDist(uv, 5.0, 4.0, phi2, 0.88);
        float line2 = smoothstep(lw, lw * 0.3, d2);
        float halo2 = exp(-d2 * 60.0) * 0.4;
        vec3  c2    = vec3(0.05, 0.3, 1.0);    // electric blue
        col += c2 * (line2 + halo2) * hdrPeak;
    }

    // ── Curve 3: 4:3, gold ───────────────────────────────────────────
    {
        float phi3  = TIME * phaseSpeed * 0.9 + 2.3;
        float d3    = lissajousDist(uv, 4.0, 3.0, phi3, 0.88);
        float line3 = smoothstep(lw, lw * 0.3, d3);
        float halo3 = exp(-d3 * 60.0) * 0.4;
        vec3  c3    = vec3(1.0, 0.85, 0.0);    // gold
        col += c3 * (line3 + halo3) * hdrPeak;
    }

    // ── Curve 4: 3:5, lime ───────────────────────────────────────────
    {
        float phi4  = TIME * phaseSpeed * 0.6 + 0.7;
        float d4    = lissajousDist(uv, 3.0, 5.0, phi4, 0.88);
        float line4 = smoothstep(lw, lw * 0.3, d4);
        float halo4 = exp(-d4 * 60.0) * 0.4;
        vec3  c4    = vec3(0.1, 1.0, 0.05);    // lime
        col += c4 * (line4 + halo4) * hdrPeak;
    }

    gl_FragColor = vec4(col, 1.0);
}
