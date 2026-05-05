/*{
    "DESCRIPTION": "Stone Hall — raymarched Gothic stone corridor with arching vault, warm amber god rays, and deep shadow. Environmental wide-angle composition; contrasts prior flat neon brick background.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Cinematic"],
    "INPUTS": [
        { "NAME": "hallWidth",  "LABEL": "Hall Width",   "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.4, "MAX": 2.5 },
        { "NAME": "archFreq",   "LABEL": "Arch Freq",    "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0, "MAX": 8.0 },
        { "NAME": "rayIntens",  "LABEL": "God Ray",      "TYPE": "float", "DEFAULT": 2.4,  "MIN": 0.5, "MAX": 4.0 },
        { "NAME": "walkSpeed",  "LABEL": "Walk Speed",   "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "audioReact", "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0 }
    ]
}*/

float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash11(float n) { return fract(sin(n * 73.1) * 43758.5); }

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Arch cross section: rounded rectangle minus circular cutout
float sdArch(vec2 p, float w, float h, float archR) {
    // Floor-to-ceiling rectangle
    float rect = sdBox(vec3(p, 0.0), vec3(w, h, 0.0));
    // Subtract arch (circle at top center)
    float circle = length(p - vec2(0.0, h - archR)) - archR;
    return max(rect, -circle);
}

float scene(vec3 p, float t, out vec3 matId) {
    float zOff = t * walkSpeed;
    vec3 q = p;
    q.z -= zOff;

    // Hall: infinite corridor along Z, arched cross-section
    // Repeating arch ribs every archFreq units
    float segZ = mod(q.z, archFreq) - archFreq * 0.5;

    // Walls and ceiling (box)
    float dWall  = -sdBox(q, vec3(hallWidth, hallWidth * 0.9, 1000.0));
    float dFloor = q.y + hallWidth * 0.9;  // floor plane

    // Arch ribs (thin box along Z)
    float dRib = sdBox(vec3(q.x, q.y, segZ), vec3(hallWidth + 0.05, hallWidth * 0.95, 0.04));

    float d = min(max(dWall, -dFloor), dRib);

    // Stone block texture via abs() ridges on walls
    float brickZ = abs(fract(q.z * 1.2) - 0.5);
    float brickY = abs(fract(q.y * 1.4) - 0.5);
    float mortar = min(brickZ, brickY);

    matId = vec3(mortar, float(dRib < dWall), 0.0); // mortar factor, isRib
    return d;
}

vec3 calcNormal(vec3 p, float t) {
    vec3 mi;
    const vec2 e = vec2(0.003, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, t, mi) - scene(p - e.xyy, t, mi),
        scene(p + e.yxy, t, mi) - scene(p - e.yxy, t, mi),
        scene(p + e.yyx, t, mi) - scene(p - e.yyx, t, mi)
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t     = TIME;
    float audio = 1.0 + audioLevel * audioReact * 0.25;

    // Eye-level camera, drifting side to side slightly
    vec3 ro = vec3(sin(t * 0.09) * 0.15, 0.1, 0.0);
    vec3 rd = normalize(vec3(uv.x, uv.y, -1.6));

    float dt   = 0.05;
    bool  hit  = false;
    float dSurf = 1.0;
    vec3  matId = vec3(0.0);

    for (int i = 0; i < 80; i++) {
        vec3 p = ro + rd * dt;
        dSurf = scene(p, t, matId);
        if (abs(dSurf) < 0.003) { hit = true; break; }
        if (dt > 14.0) break;
        dt += max(abs(dSurf) * 0.7, 0.01);
    }

    // Atmospheric corridor darkness
    vec3 col = vec3(0.04, 0.025, 0.01);

    if (hit) {
        vec3 p   = ro + rd * dt;
        vec3 nor = calcNormal(p, t);
        vec3 vd  = -rd;

        // Palette: warm stone tan, deep shadow, golden sunbeam, white-hot
        vec3 stone   = vec3(0.55, 0.38, 0.22);
        vec3 shadow  = vec3(0.05, 0.03, 0.01);
        vec3 sunbeam = vec3(1.00, 0.80, 0.40);
        vec3 whiteHot = vec3(1.00, 0.95, 0.80);

        float mortar = matId.x;
        bool isRib   = matId.y > 0.5;

        // Sunlight from high arch opening above
        vec3 sunDir = normalize(vec3(0.3, 1.0, -0.5));
        float diff  = max(0.0, dot(nor, sunDir));
        float spec  = pow(max(0.0, dot(reflect(-sunDir, nor), vd)), 24.0);

        // Mortar lines are darker
        vec3 baseCol = stone * (isRib ? 1.2 : 1.0);
        baseCol = mix(baseCol * 0.3, baseCol, smoothstep(0.0, 0.1, mortar));

        col  = baseCol * (diff * 0.6 + 0.08);
        col += sunbeam * diff * diff * rayIntens * 0.5 * audio;
        col += whiteHot * spec * rayIntens * 0.3;

        // God ray volumetric shaft: vertical beam through hall
        float beamX = abs(p.x / hallWidth);
        float ray   = exp(-beamX * beamX * 6.0) * exp(-dt * 0.1);
        col += sunbeam * ray * rayIntens * audio;

        // Depth fog
        float fog = exp(-dt * 0.05);
        col = mix(shadow * 0.3, col, fog);

        // Black ink stone edges via fwidth
        float ew   = fwidth(dSurf) * 3.0;
        float edge = smoothstep(0.0, ew, abs(dSurf));
        col = mix(vec3(0.0), col, edge);
    }

    gl_FragColor = vec4(col, 1.0);
}
