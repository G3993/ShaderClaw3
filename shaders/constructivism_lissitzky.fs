/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Russian Constructivism after Lissitzky — red wedge slamming a white circle on cream paper, with diagonal black bars and Cyrillic-stencil glyph clusters. Beat the Whites with the Red Wedge (1919).",
  "INPUTS": [
    {"NAME":"triangleThrust","TYPE":"float","MIN":0.0,"MAX":0.3,"DEFAULT":0.1},
    {"NAME":"triangleAngle","TYPE":"float","MIN":0.0,"MAX":1.5,"DEFAULT":0.55},
    {"NAME":"circleSize","TYPE":"float","MIN":0.15,"MAX":0.4,"DEFAULT":0.25},
    {"NAME":"barCount","TYPE":"float","MIN":0.0,"MAX":4.0,"DEFAULT":2.0},
    {"NAME":"glyphIntensity","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.6},
    {"NAME":"compositionJitter","TYPE":"float","MIN":0.0,"MAX":0.02,"DEFAULT":0.005},
    {"NAME":"useTex","TYPE":"bool","DEFAULT":false},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

// Fixed lithograph palette — drift from these and the Constructivist read collapses.
const vec3 CREAM   = vec3(0.96, 0.92, 0.82);
const vec3 RED     = vec3(0.89, 0.12, 0.14);
const vec3 BLACK   = vec3(0.10, 0.10, 0.10);
const vec3 WHITE   = vec3(1.00, 1.00, 1.00);

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

// Iso-triangle SDF: half-plane cuts that compose a wedge with apex t0 pointing
// along axis a. width = breadth at the base.
float wedge(vec2 uv, vec2 t0, vec2 axis, float length_, float width) {
    vec2 perp = vec2(-axis.y, axis.x);
    vec2 d = uv - t0;
    float along = dot(d, axis);
    float across = dot(d, perp);
    // Inside if 0 < along < length_ AND |across| < width * (along/length_).
    float t = clamp(along / length_, 0.0, 1.0);
    float halfBase = width * t;
    float inWedge = step(0.0, along) * step(along, length_)
                  * step(abs(across), halfBase);
    return inWedge;
}

// Hard-contrast filter for input video inside the white circle.
vec3 hardContrast(vec3 c) {
    float L = dot(c, vec3(0.299, 0.587, 0.114));
    return vec3(step(0.5, L));
}

// Single black bar at hashed angle/position. Index disambiguates.
float barShape(vec2 uv, int idx, float audioReact) {
    float seed = float(idx) * 1.7;
    float angle = mix(-0.8, 0.8, hash21(vec2(seed, 0.13)))
                + audioReact * 0.05
                + TIME * 0.10 * sign(hash21(vec2(seed, 0.5)) - 0.5);
    vec2 c = vec2(mix(0.18, 0.82, hash21(vec2(seed, 0.31))),
                  mix(0.18, 0.82, hash21(vec2(seed, 0.71))));
    // Searchlight drift — bars sweep slowly across the canvas.
    c += 0.03 * vec2(sin(TIME * 0.4 + seed),
                     cos(TIME * 0.5 + seed));
    vec2 d = uv - c;
    mat2 R = mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
    d = R * d;
    float len = mix(0.18, 0.45, hash21(vec2(seed, 0.97)));
    float thk = 0.012;
    return step(abs(d.x), len * 0.5) * step(abs(d.y), thk);
}

// Sparse procedural Cyrillic-stencil glyph clusters at three positions.
// Not real letters — vertical/horizontal bars at hashed offsets, just enough
// to read as poster type from a distance.
float glyphField(vec2 uv) {
    float total = 0.0;
    for (int g = 0; g < 3; g++) {
        vec2 origin = vec2(0.08 + float(g) * 0.05,
                           0.10 + float(g) * 0.03);
        if (g == 1) origin = vec2(0.78, 0.85);
        if (g == 2) origin = vec2(0.10, 0.78);
        // Constructivist typography slipping past the viewer.
        origin.x += sin(TIME * 0.20 + float(g)) * 0.02;
        origin.y += cos(TIME * 0.18 + float(g) * 1.7) * 0.015;
        vec2 local = (uv - origin) * vec2(28.0, 14.0);
        if (any(lessThan(local, vec2(0.0))) ||
            local.x > 18.0 || local.y > 4.0) continue;
        vec2 ci = floor(local);
        vec2 cf = fract(local);
        float h = hash21(ci);
        // Fat vertical strokes + occasional crossbar — suggests stencilled type.
        float vert = step(h, 0.5) * step(0.15, cf.x) * step(cf.x, 0.45);
        float bar  = step(0.85, h) * step(0.4, cf.y) * step(cf.y, 0.6);
        total = max(total, max(vert, bar));
    }
    // Banner-scale typography across top of poster — Rodchenko grammar.
    // The defining Constructivist mark: poster-banner type running edge-to-edge.
    {
        vec2 origin = vec2(0.10, 0.05);
        origin.x += sin(TIME * 0.15) * 0.01;
        vec2 local = (uv - origin) * vec2(7.0, 3.5);
        if (!(any(lessThan(local, vec2(0.0))) ||
              local.x > 5.0 || local.y > 1.0)) {
            vec2 ci = floor(local);
            float h  = hash21(ci);
            float vert = step(h, 0.55) * step(0.18, fract(local.x))
                       * step(fract(local.x), 0.55);
            float bar  = step(0.55, h) * step(h, 0.85)
                       * step(0.40, fract(local.y))
                       * step(fract(local.y), 0.62);
            total = max(total, max(vert, bar));
        }
    }
    return total;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    // Printing-press shake — re-roll only every 1/8s so it doesn't strobe.
    uv += (vec2(hash21(vec2(floor(TIME * 8.0), 0.3)),
                hash21(vec2(floor(TIME * 8.0), 0.7))) - 0.5)
        * compositionJitter * (0.4 + audioLevel * audioReact * 0.6);

    vec3 col = CREAM;

    // White circle (centre-right, slightly above midline). Bass *contracts*
    // it — the wedge is winning.
    vec2 circCtr = vec2(0.55, 0.45);
    float r = length(uv - circCtr);
    // Circle pulses opposite-phase to the wedge — they breathe against
    // each other so the poster is always in motion.
    float circT = (0.85 + 0.15 * sin(TIME * 1.7 + 3.14159));
    float circ = step(r, circleSize * circT * (1.0 - audioBass * 0.05));
    vec3 circCol = WHITE;
    if (useTex && IMG_SIZE_inputTex.x > 0.0) {
        circCol = hardContrast(texture(inputTex, uv).rgb);
    }
    col = mix(col, circCol, circ);

    // Red wedge — continuous ramming animation independent of audio.
    // Apex thrusts toward the circle; bass adds an extra punch.
    vec2 axis = vec2(cos(triangleAngle), sin(triangleAngle));
    float thrustT = (0.5 + 0.5 * sin(TIME * 1.7));
    vec2 apex = circCtr
              - axis * 0.05
              - axis * triangleThrust * thrustT
              - axis * triangleThrust * audioBass * 0.4;
    float tri = wedge(uv, apex, -axis, 0.55, 0.32);
    col = mix(col, RED, tri);

    // 0-4 black bars at varying diagonals, gently rotating with audioMid.
    int bars = int(clamp(barCount, 0.0, 4.0));
    for (int i = 0; i < 4; i++) {
        if (i >= bars) break;
        col = mix(col, BLACK, barShape(uv, i, audioMid));
    }

    // Cyrillic-style glyph clusters — flicker on treble.
    float gf = glyphField(uv) * glyphIntensity * (0.7 + audioHigh * 0.5);
    col = mix(col, BLACK, gf);

    gl_FragColor = vec4(col, 1.0);
}
