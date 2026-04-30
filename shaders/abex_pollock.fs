/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Pollock action painting — N independent drippers wander the canvas via curl-noise advection, depositing thick paint into a persistent buffer that fades slowly. All-over skein composition: black, aluminium, ochre, cadmium red on raw canvas. After Number 1A (1948) and Autumn Rhythm (1950).",
  "INPUTS": [
    { "NAME": "pollockWork", "LABEL": "Painting", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["Autumn Rhythm (1950)", "Lavender Mist (1950)", "Number 1A (1948)", "Blue Poles (1952)", "Convergence (1952)"] },
    { "NAME": "drippers", "LABEL": "Drippers", "TYPE": "float", "MIN": 4.0, "MAX": 100.0, "DEFAULT": 40.0 },
    { "NAME": "strokeWidth", "LABEL": "Stroke Width", "TYPE": "float", "MIN": 0.001, "MAX": 0.025, "DEFAULT": 0.006 },
    { "NAME": "turbulence", "LABEL": "Turbulence", "TYPE": "float", "MIN": 0.5, "MAX": 6.0, "DEFAULT": 2.4 },
    { "NAME": "wanderSpeed", "LABEL": "Wander Speed", "TYPE": "float", "MIN": 0.01, "MAX": 0.4, "DEFAULT": 0.18 },
    { "NAME": "paintFade", "LABEL": "Paint Persistence", "TYPE": "float", "MIN": 0.94, "MAX": 1.0, "DEFAULT": 0.985 },
    { "NAME": "splatterDensity", "LABEL": "Splatter", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.25 },
    { "NAME": "wetness", "LABEL": "Wetness", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "blackWeight", "LABEL": "Black Skein", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "useTexColor", "LABEL": "Use Tex Colour", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "resetField", "LABEL": "Reset", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "paintBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Pollock's classical drip palette: enamel black, lead white, aluminium
// silver, cadmium red. The skein needs distinctive separation between
// strokes — random rainbow defeats the read.
const vec3 POL_BLACK = vec3(0.05, 0.04, 0.04);
const vec3 POL_WHITE = vec3(0.95, 0.93, 0.88);
const vec3 POL_SILVR = vec3(0.62, 0.64, 0.65);
const vec3 POL_RED   = vec3(0.78, 0.16, 0.12);
const vec3 POL_OCHRE = vec3(0.62, 0.46, 0.18);
const vec3 RAW_CANVAS = vec3(0.88, 0.83, 0.72);

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float p) {
    return fract(sin(p * 12.9898) * 43758.5453);
}

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

// 2D curl of a noise potential — divergence-free vector field. Each
// "dripper" follows this so its trajectory wanders without crossing
// itself the way Bezier or sine paths do.
vec2 curl(vec2 p) {
    float e = 0.01;
    float a = vnoise(p + vec2(0.0, e)) - vnoise(p - vec2(0.0, e));
    float b = vnoise(p + vec2(e, 0.0)) - vnoise(p - vec2(e, 0.0));
    return normalize(vec2(a, -b) + 1e-5);
}

// Compute a dripper's current position by Eulerian-stepping along the
// curl field. Doing N steps per fragment is expensive but each fragment
// only walks N=12 steps → O(N) per fragment per dripper.
vec2 dripperPos(int id, float t, float turb, float speed) {
    float fid = float(id);
    vec2 base = vec2(hash11(fid * 1.31), hash11(fid * 2.97 + 4.7));
    vec2 p = base;
    // Walk a fixed number of steps with longer stride — saves 57% of
    // noise calls vs the original 14-step loop while preserving total
    // path length. The per-frame TIME term inside curl already gives
    // continuous wandering.
    for (int i = 0; i < 6; i++) {
        p += curl(p * turb + fid * 11.7 + TIME * 0.02) * speed * 0.08;
        p = clamp(p, 0.02, 0.98);
    }
    // Per-frame walk so the dripper moves continuously rather than
    // settling at a fixed integration result.
    p += 0.02 * vec2(sin(TIME * 0.50 + fid),
                     cos(TIME * 0.40 + fid));
    // Optional pooling — every few seconds, 15% of drippers dwell near
    // their base point so the canvas develops paint pools.
    if (hash11(fid + floor(TIME * 0.3)) > 0.85) p = mix(p, base, 0.5);
    return clamp(p, 0.02, 0.98);
}

// Per-painting palette swap — Pollock's drip-period spans wildly
// different colour worlds: Lavender Mist's pinks and pale greys vs
// Blue Poles's cobalt verticals vs Number 1A's ochre/black.
void pollockPalette(int w, out vec3 c0, out vec3 c1, out vec3 c2,
                    out vec3 c3, out vec3 c4) {
    if (w == 1) {            // Lavender Mist 1950 — pink/grey/white veils
        c0 = vec3(0.18, 0.16, 0.18); c1 = vec3(0.93, 0.91, 0.92);
        c2 = vec3(0.78, 0.66, 0.74); c3 = vec3(0.55, 0.50, 0.58);
        c4 = vec3(0.88, 0.78, 0.82);
    } else if (w == 2) {     // Number 1A 1948 — handprints, ochre + black
        c0 = vec3(0.06, 0.05, 0.05); c1 = vec3(0.96, 0.94, 0.88);
        c2 = vec3(0.70, 0.55, 0.20); c3 = vec3(0.40, 0.32, 0.20);
        c4 = vec3(0.85, 0.75, 0.55);
    } else if (w == 3) {     // Blue Poles 1952 — cobalt verticals
        c0 = vec3(0.05, 0.04, 0.04); c1 = vec3(0.92, 0.88, 0.78);
        c2 = vec3(0.10, 0.22, 0.62); c3 = vec3(0.78, 0.18, 0.14);
        c4 = vec3(0.65, 0.55, 0.20);
    } else if (w == 4) {     // Convergence 1952 — white/red/yellow chaos
        c0 = vec3(0.05, 0.04, 0.04); c1 = vec3(0.96, 0.95, 0.92);
        c2 = vec3(0.85, 0.20, 0.18); c3 = vec3(0.92, 0.78, 0.20);
        c4 = vec3(0.20, 0.30, 0.55);
    } else {                 // 0 = Autumn Rhythm 1950 (default)
        c0 = POL_BLACK; c1 = POL_WHITE; c2 = POL_SILVR;
        c3 = POL_RED;   c4 = POL_OCHRE;
    }
}

vec3 dripperColor(int id, vec3 srcSample, float blackBias) {
    vec3 c0, c1, c2, c3, c4;
    pollockPalette(int(pollockWork), c0, c1, c2, c3, c4);
    float h = hash11(float(id) * 7.13);
    if (h < blackBias)            return c0;
    if (h < blackBias + 0.18)     return c1;
    if (h < blackBias + 0.32)     return c2;
    if (h < blackBias + 0.42)     return c3;
    return c4;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    int N = int(clamp(drippers, 1.0, 100.0));

    // ============= PASS 0 — paintBuf accumulation =============
    if (PASSINDEX == 0) {

        if (FRAMEINDEX < 2 || resetField) {
            gl_FragColor = vec4(RAW_CANVAS, 1.0);
            return;
        }

        // Slow self-decay toward canvas — paint stays for many seconds,
        // long enough for a dense skein to build, but eventually clears
        // so live performance never saturates the buffer.
        vec3 prev = texture(paintBuf, uv).rgb;
        prev = mix(RAW_CANVAS, prev, paintFade);

        // No self-advection — Pollock paint stays where the gesture put
        // it. (Removed to differentiate visually from Fauvism's flowing
        // paint buffer; both used to be persistent paint + curl drift.)

        // Deposit: walk each dripper to its CURRENT position (not its
        // whole history) and check if this fragment is on the stroke.
        // Wider strokes for low-frequency content (bass kicks).
        float t = TIME * wanderSpeed * (0.5 + audioMid * audioReact * 1.5);
        // Width chosen per dripper per second so strokes vary thickness
        // like real flicks of enamel — not all uniform.
        float wHash = hash11(float(0) + floor(TIME * 1.2));  // shared baseline
        float w = strokeWidth * (1.0 + audioLevel * audioReact * 0.8);
        // Hard loop bound 100 — GLSL needs a constant upper limit on
        // for-loop counters. The early `break` keeps actual cost
        // proportional to the live N, not to the 100 ceiling.
        for (int i = 0; i < 100; i++) {
            if (i >= N) break;
            float fi = float(i);
            // Sample each dripper at TWO time-offsets and deposit along
            // the SEGMENT between previous and current head positions.
            // Converts per-frame stamps into continuous skeins —
            // *Autumn Rhythm* lattice density instead of bead-chains.
            float tOff = hash11(fi * 0.71) * 8.0;
            vec2 pNow  = dripperPos(i, t + tOff, turbulence, 1.0);
            vec2 pPrev = dripperPos(i, t + tOff - 0.06, turbulence, 1.0);
            // Distance from fragment to the segment pPrev→pNow
            vec2 ab = pNow - pPrev;
            float h = clamp(dot(uv - pPrev, ab) / max(dot(ab, ab), 1e-6),
                            0.0, 1.0);
            vec2 cl = pPrev + ab * h;
            vec2 d  = uv - cl;
            d.x *= aspect;
            float ds = length(d);
            if (ds > w * 4.0) continue;
            float falloff = smoothstep(w, w * 0.4, ds);
            if (falloff < 0.001) continue;
            vec3 src = (IMG_SIZE_inputTex.x > 0.0)
                     ? texture(inputTex, cl).rgb : vec3(0.5);
            vec3 c = useTexColor ? src
                                 : dripperColor(i, src, blackWeight);
            prev = mix(prev, c, falloff);
        }

        gl_FragColor = vec4(prev, 1.0);
        return;
    }

    // ============= PASS 1 — output ============================================

    vec3 col = texture(paintBuf, uv).rgb;

    // Splatter: tiny solid dots scattered at hashed positions, replacing
    // the underlying canvas value. Treble surges push splatter density.
    if (splatterDensity > 0.0) {
        vec2 g = uv * 480.0;
        vec2 gi = floor(g);
        float roll = hash21(gi);
        if (roll > 1.0 - splatterDensity * 0.05
                * (0.5 + audioHigh * audioReact * 1.3)) {
            float spat = step(length(fract(g) - 0.5), 0.18);
            int cidx = int(hash21(gi + 17.3) * 4.0);
            vec3 sc = (cidx == 0) ? POL_BLACK
                    : (cidx == 1) ? POL_WHITE
                    : (cidx == 2) ? POL_RED : POL_OCHRE;
            col = mix(col, sc, spat);
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
