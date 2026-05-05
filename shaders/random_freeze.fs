/*{
    "DESCRIPTION": "Basalt Columns — raymarched hexagonal prismatic pillars (Giant's Causeway) rising from lava cracks. Black obsidian / molten orange / gold HDR palette. Complete opposite temperature to prior Arctic Shard.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Cinematic"],
    "INPUTS": [
        { "NAME": "columnCount",  "LABEL": "Column Grid",   "TYPE": "float", "DEFAULT": 4.0,  "MIN": 2.0,  "MAX": 8.0 },
        { "NAME": "columnHeight", "LABEL": "Column Height", "TYPE": "float", "DEFAULT": 1.4,  "MIN": 0.4,  "MAX": 3.0 },
        { "NAME": "lavaGlow",     "LABEL": "Lava Glow",     "TYPE": "float", "DEFAULT": 2.6,  "MIN": 1.0,  "MAX": 4.0 },
        { "NAME": "crackWidth",   "LABEL": "Crack Width",   "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.01, "MAX": 0.3 },
        { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 73.1) * 43758.5); }

// 2D hexagonal grid: returns cell id and local coords
vec2 hexGrid(vec2 p, out vec2 id) {
    const vec2 s = vec2(1.0, 1.7320508);
    vec2 a = mod(p, s) - s * 0.5;
    vec2 b = mod(p - s * 0.5, s) - s * 0.5;
    vec2 gv = dot(a, a) < dot(b, b) ? a : b;
    id = p - gv;
    return gv;
}

// Hexagonal prism SDF (elongated along Y)
float sdHexPrism(vec3 p, float r, float h) {
    const vec3 k = vec3(-0.8660254, 0.5, 0.57735);
    p.xz = abs(p.xz);
    p.xz -= 2.0 * min(dot(k.xy, p.xz), 0.0) * k.xy;
    vec2 d = vec2(length(p.xz - vec2(clamp(p.x, -k.z * r, k.z * r), r)) * sign(p.z - r),
                  abs(p.y) - h);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

float lavaFloor(vec2 p, float t) {
    // Lava cracks: bright between columns
    vec2 id;
    hexGrid(p * 2.0 / columnCount, id);
    float h21 = hash21(id);
    float ripple = sin(t * 1.4 * (0.5 + h21 * 0.7) + h21 * 6.28) * 0.5 + 0.5;
    return ripple;
}

float scene(vec3 p, float t, out float lavaT, out vec2 colId) {
    float audio = 1.0 + audioLevel * audioReact * 0.2;

    float gSize = 1.0 / columnCount * 2.2;
    vec2 id;
    vec2 gv = hexGrid(p.xz / gSize, id);
    colId = id;

    // Per-column height variation
    float heightVar = hash21(id) * 0.4 + 0.8;
    float colH = columnHeight * heightVar * audio;
    float colR = gSize * (0.42 - crackWidth * 0.5);

    // Prism SDF in local XZ
    vec3 localP = vec3(gv.x, p.y, gv.y);
    float dCol = sdHexPrism(localP, colR, colH);

    // Floor at y = -colH base
    float dFloor = p.y + colH + 0.01;
    lavaT = lavaFloor(p.xz, t);

    return min(dCol, dFloor);
}

vec3 calcNormal(vec3 p, float t) {
    float lt; vec2 ci;
    const vec2 e = vec2(0.003, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, t, lt, ci) - scene(p - e.xyy, t, lt, ci),
        scene(p + e.yxy, t, lt, ci) - scene(p - e.yxy, t, lt, ci),
        scene(p + e.yyx, t, lt, ci) - scene(p - e.yyx, t, lt, ci)
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;
    float audio = 1.0 + audioLevel * audioReact;

    // Camera: side-elevated view of the column field
    float camA = t * 0.06;
    vec3 ro = vec3(cos(camA) * 3.0, 1.8 + sin(t * 0.05) * 0.4, sin(camA) * 3.0);
    vec3 target = vec3(0.0, 0.2, 0.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd * 1.5 + uv.x * right + uv.y * up);

    float dt   = 0.0;
    bool  hit  = false;
    float dSurf = 1.0;
    float lavaT = 0.0;
    vec2  colId = vec2(0.0);

    for (int i = 0; i < 80; i++) {
        vec3 p = ro + rd * dt;
        float lt; vec2 ci;
        dSurf = scene(p, t, lt, ci);
        if (dSurf < 0.002) {
            hit   = true;
            // Recompute for shading
            scene(p, t, lavaT, colId);
            break;
        }
        if (dt > 12.0) break;
        dt += max(abs(dSurf) * 0.6, 0.008);
    }

    // Sky: smoky charcoal with ember glow at horizon
    float skyH = uv.y * 0.5 + 0.5;
    vec3 col = mix(vec3(0.18, 0.06, 0.01), vec3(0.04, 0.02, 0.05), skyH);

    if (hit) {
        vec3 p   = ro + rd * dt;
        vec3 nor = calcNormal(p, t);

        bool isFloor = (p.y < -columnHeight * 0.6);

        // Palette: black obsidian, lava orange, gold, white-hot
        vec3 obsidian  = vec3(0.03, 0.02, 0.02);
        vec3 lavaOrange = vec3(1.00, 0.28, 0.00);
        vec3 gold      = vec3(1.00, 0.72, 0.10);
        vec3 whiteHot  = vec3(1.00, 0.92, 0.75);

        if (isFloor) {
            // Lava floor: glowing in cracks between columns
            vec3 lavaCol = mix(obsidian, lavaOrange, smoothstep(0.3, 0.8, lavaT));
            lavaCol = mix(lavaCol, gold,     smoothstep(0.7, 0.95, lavaT));
            lavaCol = mix(lavaCol, whiteHot, smoothstep(0.90, 1.0, lavaT));
            col  = lavaCol * lavaGlow;
        } else {
            // Basalt column: mostly black with orange rim from lava below
            vec3 upLight = normalize(vec3(0.0, -1.0, 0.0));  // lava light from below
            float belowLight = max(0.0, dot(nor, -upLight));
            vec3 keyDir = normalize(vec3(0.6, 1.5, 0.8));
            float diff  = max(0.0, dot(nor, keyDir)) * 0.3;
            float spec  = pow(max(0.0, dot(reflect(-keyDir, nor), -rd)), 32.0);

            float hv = hash21(colId);
            float lavaPulse = sin(t * 1.4 * (0.5 + hv * 0.5)) * 0.5 + 0.5;

            col  = obsidian * (diff + 0.05);
            col += lavaOrange * belowLight * lavaGlow * (0.4 + lavaPulse * 0.5) * audio;
            col += gold       * belowLight * belowLight * lavaGlow * 0.3;
            col += whiteHot   * spec * lavaGlow * 0.6;

            // Black ink top edge
            float ew   = fwidth(dSurf) * 3.0;
            float edge = smoothstep(0.0, ew, abs(dSurf));
            col = mix(vec3(0.0), col, edge);
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
