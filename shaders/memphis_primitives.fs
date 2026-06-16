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
    if (i == 0) return vec3(0.902, 0.224, 0.275);   // memphis red
    if (i == 1) return vec3(0.957, 0.827, 0.369);   // memphis yellow
    if (i == 2) return vec3(0.227, 0.525, 1.000);   // memphis blue
    if (i == 3) return vec3(0.078, 0.078, 0.094);   // black
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

// Choose one of 7 primitives based on a hash. Returns flat colour
// composited over the cell background paper.
vec3 drawPrimitive(int kind, vec2 p, float ang, float scale, vec3 c, vec3 bg) {
    p = rot(ang) * p;
    p /= scale;

    if (kind == 0) {
        // Circle disc
        float d = sdCircle(p, 0.5);
        return mix(c, bg, smoothstep(0.0, 0.02, d));
    }
    if (kind == 1) {
        // Square
        float d = sdSquare(p, 0.5);
        return mix(c, bg, smoothstep(0.0, 0.02, d));
    }
    if (kind == 2) {
        // Triangle
        float d = sdTriangle(p, 0.5);
        return mix(c, bg, smoothstep(0.0, 0.02, d));
    }
    if (kind == 3) {
        // Checkerboard inside a circle frame
        float frame = sdCircle(p, 0.5);
        if (frame > 0.0) return bg;
        return mix(bg, c, checker(p, 4.0));
    }
    if (kind == 4) {
        // Diagonal stripes inside a square
        float d = sdSquare(p, 0.5);
        if (d > 0.0) return bg;
        return mix(bg, c, stripes(p, 4.0));
    }
    if (kind == 5) {
        // Squiggle line bounded by cell
        return mix(bg, c, squiggle(p, 2.0, 0.06));
    }
    // kind == 6: Polka dots
    float frame = sdSquare(p, 0.5);
    if (frame > 0.0) return bg;
    return mix(bg, c, polkaDots(p, 0.18));
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

    // Audio-coupled scale jitter on the cell — gentle pulse with level.
    float pulse = 1.0 + audioLevel * 0.05;
    col = mix(paper, col, clamp(pulse, 0.5, 1.05));

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
        col = mix(col, vec3(1.0, 0.40, 0.65), _f * _line * _on * 0.85);
    }

    gl_FragColor = vec4(col, 1.0);
}
