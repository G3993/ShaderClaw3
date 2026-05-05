/*{
  "DESCRIPTION": "Aurora Flow — 3D volumetric atmospheric scene. Raymarched sinusoidal ribbon layers represent aurora borealis bands in a wide night-sky composition. Cool blue/violet/teal HDR palette with a star field background.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"speed",      "LABEL":"Speed",       "TYPE":"float","DEFAULT":0.3, "MIN":0.0,  "MAX":1.5},
    {"NAME":"amplitude",  "LABEL":"Amplitude",   "TYPE":"float","DEFAULT":0.4, "MIN":0.1,  "MAX":1.0},
    {"NAME":"thickness",  "LABEL":"Thickness",   "TYPE":"float","DEFAULT":0.15,"MIN":0.05, "MAX":0.5},
    {"NAME":"hdrPeak",   "LABEL":"HDR Peak",    "TYPE":"float","DEFAULT":2.5, "MIN":1.0,  "MAX":4.0},
    {"NAME":"audioReact","LABEL":"Audio React", "TYPE":"float","DEFAULT":0.6, "MIN":0.0,  "MAX":2.0}
  ]
}*/

// ── Hash / utility ──────────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// ── Camera helper ───────────────────────────────────────────────────────────
mat3 lookAt(vec3 ro, vec3 ta, float roll) {
    vec3 fwd = normalize(ta - ro);
    vec3 right = normalize(cross(vec3(sin(roll), cos(roll), 0.0), fwd));
    vec3 up = cross(fwd, right);
    return mat3(right, up, fwd);
}

// ── Aurora layer colors (cool palette) ─────────────────────────────────────
vec3 auroraColor(int layer) {
    if (layer == 0) return vec3(0.4, 0.0, 1.0) * 2.5;   // deep violet
    if (layer == 1) return vec3(0.0, 1.0, 0.7) * 2.2;   // teal
    if (layer == 2) return vec3(0.0, 0.4, 1.0) * 2.0;   // electric blue
    return             vec3(0.9, 0.0, 0.8) * 2.8;        // cold magenta
}

// ── Aurora layer parameters: height, freq, phase, wave-speed ───────────────
void layerParams(int i, out float height, out float freq, out float phase, out float wspeed) {
    if (i == 0)      { height = 0.3;  freq = 1.1; phase = 0.0;  wspeed = 0.9; }
    else if (i == 1) { height = 0.9;  freq = 0.7; phase = 2.1;  wspeed = 0.6; }
    else if (i == 2) { height = 1.5;  freq = 1.4; phase = 4.3;  wspeed = 1.2; }
    else             { height = 2.1;  freq = 0.5; phase = 1.0;  wspeed = 0.4; }
}

// ── Star field ──────────────────────────────────────────────────────────────
float stars(vec3 rd) {
    // Project ray direction to 2D grid for star hashing
    vec2 gp = rd.xz / (abs(rd.y) + 0.01) * 100.0;
    vec2 cell = floor(gp);
    float h = hash21(cell);
    // Only render stars for upward-facing rays
    float upMask = smoothstep(0.05, 0.25, rd.y);
    return (h > 0.995) ? 1.5 * upMask : 0.0;
}

// ── Volumetric aurora accumulation ─────────────────────────────────────────
vec3 marchAurora(vec3 ro, vec3 rd) {
    float audioBoost = 1.0 + audioLevel * audioReact;
    float thk = thickness;

    vec3 col = vec3(0.0);
    float stepLen = 0.12;
    float t = 0.1;

    for (int s = 0; s < 64; s++) {
        vec3 p = ro + rd * t;

        // Accumulate each aurora layer
        for (int i = 0; i < 4; i++) {
            float h, freq, phase, wspeed;
            layerParams(i, h, freq, phase, wspeed);

            // Sinusoidal ribbon surface
            float ribbon = h + amplitude * sin(p.x * freq + TIME * speed * wspeed + phase);
            float dist = abs(p.y - ribbon);

            // Gaussian density with fwidth AA
            float fw = fwidth(dist);
            float density = exp(-(dist * dist) / (thk * thk));
            // AA: smooth the density boundary
            density *= smoothstep(fw, 0.0, dist - thk * 2.5);

            vec3 layCol = auroraColor(i) * hdrPeak;
            col += layCol * density * stepLen * audioBoost * 0.18;
        }

        // March step
        t += stepLen;
        if (t > 30.0) break;
    }

    return col;
}

// ── Main ────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Camera: slow horizontal sweep, looking up into the sky
    float camAngle = TIME * 0.05;
    vec3 ro = vec3(sin(camAngle) * 2.0, 0.5, cos(camAngle) * 2.0);
    // Target slightly above and ahead — always looking up
    vec3 ta = vec3(sin(camAngle + 0.4) * 4.0, 2.5, cos(camAngle + 0.4) * 4.0);

    mat3 cam = lookAt(ro, ta, 0.0);
    vec3 rd = cam * normalize(vec3(uv, 1.8));

    // Background: night sky + stars
    vec3 nightSky = vec3(0.0, 0.01, 0.04);
    float starVal = stars(normalize(rd));
    vec3 col = nightSky + vec3(1.0) * starVal * 1.5;

    // Volumetric aurora layers
    col += marchAurora(ro, rd);

    // Horizon gradient — keep deep black at bottom edge
    float horizonFade = smoothstep(-0.2, 0.3, rd.y);
    col *= horizonFade;
    col += nightSky * (1.0 - horizonFade);

    // Subtle audio pulse on the whole scene
    col *= 0.85 + audioBass * audioReact * 0.35;

    gl_FragColor = vec4(col, 1.0);
}
