/*{
    "DESCRIPTION": "Torus Portal — raymarched glowing torus SDF hovering above a holographic perspective grid. Close-up 3D portrait composition: contrasts prior wide flat 2D vaporwave scene. Hot pink / electric cyan / violet HDR palette.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Holographic"],
    "INPUTS": [
        { "NAME": "torusR",     "LABEL": "Torus Major R", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.1,  "MAX": 1.2 },
        { "NAME": "torusr",     "LABEL": "Torus Minor r", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.02, "MAX": 0.5 },
        { "NAME": "spinSpeed",  "LABEL": "Spin Speed",    "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 2.0 },
        { "NAME": "hdrBoost",   "LABEL": "HDR Boost",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0 },
        { "NAME": "audioReact", "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Grid floor: box with repeating grid lines cutout
float sdGridFloor(vec3 p) {
    float floor_ = p.y + 0.55;
    return floor_;
}

float scene(vec3 p, float t, out int matType) {
    float audio = 1.0 + audioLevel * audioReact * 0.2;

    // Torus tilted and spinning
    float spinA = t * spinSpeed;
    float tiltA = 0.4 + sin(t * 0.2) * 0.15;
    // Rotate around Y (spin)
    float cy = cos(spinA), sy = sin(spinA);
    vec3 pt = vec3(p.x * cy + p.z * sy, p.y, -p.x * sy + p.z * cy);
    // Tilt around X
    float cx = cos(tiltA), sx = sin(tiltA);
    pt = vec3(pt.x, pt.y * cx - pt.z * sx, pt.y * sx + pt.z * cx);
    // Hover: oscillate Y
    pt.y -= sin(t * 0.7) * 0.08;

    float dTorus = sdTorus(pt, torusR * audio, torusr * audio);

    float dFloor = sdGridFloor(p);

    if (dTorus < dFloor) { matType = 0; return dTorus; }
    matType = 1;
    return dFloor;
}

vec3 calcNormal(vec3 p, float t) {
    int mt;
    const vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, t, mt) - scene(p - e.xyy, t, mt),
        scene(p + e.yxy, t, mt) - scene(p - e.yxy, t, mt),
        scene(p + e.yyx, t, mt) - scene(p - e.yyx, t, mt)
    ));
}

vec3 hsvRgb(float h, float s, float v) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(h + K.xyz) * 6.0 - K.www);
    return v * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), s);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t     = TIME;
    float audio = 1.0 + audioLevel * audioReact;

    // Camera: eye-level looking slightly up at the torus
    vec3 ro = vec3(sin(t * 0.08) * 0.3, 0.15, 2.4);
    vec3 target = vec3(0.0, 0.15, 0.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd * 1.6 + uv.x * right + uv.y * up);

    float dt   = 0.0;
    bool  hit  = false;
    float dSurf = 1.0;
    int matType = 0;

    for (int i = 0; i < 80; i++) {
        vec3 p = ro + rd * dt;
        int mt;
        dSurf = scene(p, t, mt);
        if (dSurf < 0.002) { hit = true; matType = mt; break; }
        if (dt > 8.0) break;
        dt += max(dSurf * 0.85, 0.006);
    }

    // Dark void / deep purple bg
    vec3 col = vec3(0.004, 0.001, 0.012);

    if (hit) {
        vec3 p   = ro + rd * dt;
        vec3 nor = calcNormal(p, t);
        vec3 vd  = -rd;

        // Palette: hot pink, electric cyan, violet, white-hot
        vec3 hotPink  = vec3(1.00, 0.07, 0.65);
        vec3 elecCyan = vec3(0.00, 0.90, 1.00);
        vec3 violet   = vec3(0.50, 0.00, 1.00);
        vec3 whiteHot = vec3(1.00, 0.88, 1.00);

        if (matType == 0) {
            // Torus: iridescent pink-cyan based on angle
            float iridHue = fract(dot(nor, vd) * 1.5 + t * 0.04);
            vec3 iridCol  = mix(hotPink, elecCyan, iridHue);

            vec3 keyDir = normalize(vec3(0.8, 1.8, 1.0));
            float diff  = max(0.0, dot(nor, keyDir));
            float spec  = pow(max(0.0, dot(reflect(-keyDir, nor), vd)), 48.0);
            float fres  = pow(1.0 - max(0.0, dot(nor, vd)), 2.5);

            col  = iridCol * (diff * 0.6 + 0.08);
            col += violet    * fres   * hdrBoost * 0.6;
            col += whiteHot  * spec   * hdrBoost;
            col *= hdrBoost * 0.85;

            float ew   = fwidth(dSurf) * 4.0;
            col = mix(vec3(0.0), col, smoothstep(0.0, ew, abs(dSurf)));
        } else {
            // Grid floor: perspective grid of cyan lines
            vec3 fp = p;
            float gx = abs(fract(fp.x * 4.0) - 0.5);
            float gz = abs(fract(fp.z * 4.0) - 0.5);
            float grid = 1.0 - smoothstep(0.0, 0.04, min(gx, gz));

            // Horizon fade
            float fogFloor = exp(-dt * 0.4);
            vec3 gridCol = elecCyan * grid * hdrBoost * 0.6 * fogFloor;

            // Hot pink grid glow
            gridCol += hotPink * grid * grid * hdrBoost * 0.3 * fogFloor;

            col = gridCol;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
