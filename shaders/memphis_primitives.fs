/*{
  "CATEGORIES": ["Generator", "Geometric"],
  "DESCRIPTION": "Sottsass / Memphis Group postcard come alive — checkered grids, polka dots, primary triangles rearranging on the beat. Bauhaus + Memphis 1981 colour discipline, no gradients.",
  "INPUTS": [
    {"NAME":"gridX","TYPE":"float","MIN":2.0,"MAX":12.0,"DEFAULT":6.0},
    {"NAME":"gridY","TYPE":"float","MIN":2.0,"MAX":12.0,"DEFAULT":4.0},
    {"NAME":"layoutShift","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.3},
    {"NAME":"primitiveScale","TYPE":"float","MIN":0.3,"MAX":0.9,"DEFAULT":0.7},
    {"NAME":"rotationSpeed","TYPE":"float","MIN":0.0,"MAX":3.0,"DEFAULT":0.5},
    {"NAME":"useMask","TYPE":"bool","DEFAULT":false},
    {"NAME":"bgColor","TYPE":"color","DEFAULT":[0.96,0.94,0.88,1.0]},
    {"NAME":"inputTex","TYPE":"image"}
  ]
}*/

// Memphis Group palette — Sottsass / du Pasquier 1981. Five colours,
// no gradients between them, just clean flat juxtaposition. Each cell
// in the grid picks one based on a hash, beat-jolted.
vec3 palette(float h) {
    int i = int(floor(h * 5.0));
    if (i == 0) return vec3(2.4, 0.12, 0.18);       // HDR memphis red
    if (i == 1) return vec3(2.8, 2.1, 0.10);        // HDR memphis yellow
    if (i == 2) return vec3(0.08, 0.28, 3.2);       // HDR memphis blue
    if (i == 3) return vec3(0.03, 0.02, 0.05);      // ink black
    return vec3(0.96, 0.94, 0.88);                  // off-white (paper)
}

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

float sdCircle(vec2 p, float r) { return length(p) - r; }
float sdSquare(vec2 p, float r) { vec2 d = abs(p) - r; return max(d.x, d.y); }
// Equilateral triangle with circumradius r
float sdTriangle(vec2 p, float r) {
    const float k = 1.7320508; // sqrt(3)
    p.x = abs(p.x) - r;
    p.y = p.y + r / k;
    if (p.x + k * p.y > 0.0) p = vec2(p.x - k * p.y, -k * p.x - p.y) * 0.5;
    p.x -= clamp(p.x, -2.0 * r, 0.0);
    return -length(p) * sign(p.y);
}

// Squiggle line — sin wave centered in the cell. Returns alpha mask.
float squiggle(vec2 p, float freq, float thick) {
    float y = sin(p.x * freq * 6.2832 + freq) * 0.25;
    return 1.0 - smoothstep(thick * 0.6, thick, abs(p.y - y));
}

// Checkerboard inside cell — n×n.
float checker(vec2 p, float n) {
    vec2 q = floor(p * n + n * 0.5);
    return mod(q.x + q.y, 2.0);
}

// Diagonal stripes
float stripes(vec2 p, float n) {
    float v = sin((p.x + p.y) * n * 6.2832);
    return step(0.0, v);
}

// Polka dots inside cell — 3×3 dot grid.
float polkaDots(vec2 p, float r) {
    vec2 g = fract(p * 3.0 + 1.5) - 0.5;
    float d = length(g) - r;
    return 1.0 - smoothstep(0.0, 0.02, d);
}

// Ink outline compositing helper: sharp fwidth-based AA + black ink border
vec3 inkFill(float d, vec3 c, vec3 bg) {
    float pw = max(fwidth(d), 0.002);
    float fill = 1.0 - smoothstep(-pw, pw, d);
    float ink = smoothstep(pw * 6.0, pw, abs(d));
    vec3 result = mix(bg, c, fill);
    return mix(result, vec3(0.02, 0.01, 0.03), ink);
}

// Choose one of 7 primitives based on a hash. Returns flat colour
// composited over the cell background paper.
vec3 drawPrimitive(int kind, vec2 p, float ang, float scale, vec3 c, vec3 bg) {
    p = rot(ang) * p;
    p /= scale;

    if (kind == 0) {
        return inkFill(sdCircle(p, 0.5), c, bg);
    }
    if (kind == 1) {
        return inkFill(sdSquare(p, 0.5), c, bg);
    }
    if (kind == 2) {
        return inkFill(sdTriangle(p, 0.5), c, bg);
    }
    if (kind == 3) {
        // Checkerboard inside a circle frame with ink border
        float frame = sdCircle(p, 0.5);
        float pw = max(fwidth(frame), 0.002);
        float fill = 1.0 - smoothstep(-pw, pw, frame);
        float ink = smoothstep(pw * 6.0, pw, abs(frame));
        vec3 inner = mix(bg, c, checker(p, 4.0));
        vec3 result = mix(bg, inner, fill);
        return mix(result, vec3(0.02, 0.01, 0.03), ink);
    }
    if (kind == 4) {
        // Diagonal stripes inside a square with ink border
        float d = sdSquare(p, 0.5);
        float pw = max(fwidth(d), 0.002);
        float fill = 1.0 - smoothstep(-pw, pw, d);
        float ink = smoothstep(pw * 6.0, pw, abs(d));
        vec3 inner = mix(bg, c, stripes(p, 4.0));
        vec3 result = mix(bg, inner, fill);
        return mix(result, vec3(0.02, 0.01, 0.03), ink);
    }
    if (kind == 5) {
        // Squiggle line — no bounding box
        return mix(bg, c, squiggle(p, 2.0, 0.06));
    }
    // kind == 6: Polka dots inside a square with ink border
    float frame = sdSquare(p, 0.5);
    float pw = max(fwidth(frame), 0.002);
    float fill = 1.0 - smoothstep(-pw, pw, frame);
    float ink = smoothstep(pw * 6.0, pw, abs(frame));
    vec3 inner = mix(bg, c, polkaDots(p, 0.18));
    vec3 result = mix(bg, inner, fill);
    return mix(result, vec3(0.02, 0.01, 0.03), ink);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 cell = vec2(max(2.0, gridX), max(2.0, gridY));
    vec2 cId  = floor(uv * cell);

    // Beat re-roll: bass jolts the seed, so cells redraw on transients.
    float beatStep = floor(audioBass * layoutShift * 12.0 + TIME * 0.05 * layoutShift);
    float seed     = hash(cId + beatStep);

    int prim = int(seed * 7.0);

    // Per-cell rotation from a second hash so adjacent cells don't
    // rotate in lockstep.
    float ang = TIME * rotationSpeed * (0.5 + audioMid * 1.5)
              * (hash(cId + 17.3) - 0.5) * 2.0;

    // Local cell coords centered at 0.
    vec2 cUV = (fract(uv * cell) - 0.5) * 1.6;

    // Optional inputTex mask: hide cells that fall on dark video pixels.
    vec3 paper = bgColor.rgb;
    if (useMask && IMG_SIZE_inputTex.x > 0.0) {
        vec2 sampleUV = (cId + 0.5) / cell;
        vec3 t = texture(inputTex, sampleUV).rgb;
        if (max(t.r, max(t.g, t.b)) < 0.3) { gl_FragColor = vec4(paper, 1.0); return; }
    }

    vec3 col = drawPrimitive(prim, cUV, ang, primitiveScale, palette(seed), paper);

    // Audio-coupled brightness burst on beat
    float pulse = 1.0 + audioLevel * audioBass * 0.7;
    col *= pulse;

    // Surprise: every ~13s a Sottsass squiggle — three quick zigzag
    // strokes — chases across the canvas in lipstick pink. The pattern
    // language always wanted to be a doodle.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 13.0);
        float _f  = smoothstep(0.0, 0.05, _ph) * smoothstep(0.28, 0.16, _ph);
        float _x  = (_ph - 0.05) / 0.23;
        float _zig = 0.5 + 0.20 * sin(_x * 28.0) * sin(_x * 11.0);
        float _line = exp(-pow((_suv.y - _zig) * 60.0, 2.0));
        float _on = step(_suv.x, _x) * step(_x - 0.30, _suv.x);
        col = mix(col, vec3(3.5, 0.4, 1.8), _f * _line * _on * 0.9);
    }

    gl_FragColor = vec4(col, 1.0);
}
