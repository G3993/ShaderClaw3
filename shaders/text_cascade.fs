/*{
    "DESCRIPTION": "Copper Cascade — raymarched columns of falling molten copper spheres with metallic HDR reflections. Warm copper-gold-orange palette: contrasts prior cool aurora background.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Cinematic"],
    "INPUTS": [
        { "NAME": "columnCount", "LABEL": "Columns",     "TYPE": "float", "DEFAULT": 5.0,  "MIN": 2.0,  "MAX": 10.0 },
        { "NAME": "dropSpeed",   "LABEL": "Drop Speed",  "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 2.0 },
        { "NAME": "dropSize",    "LABEL": "Drop Size",   "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.03, "MAX": 0.4 },
        { "NAME": "hdrBoost",    "LABEL": "HDR Boost",   "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0 },
        { "NAME": "audioReact",  "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float scene(vec3 p, float t, out float dropId) {
    float audio = 1.0 + audioLevel * audioReact * 0.2;
    float dRadius = dropSize * audio;
    float N = columnCount;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    float best = 1e8;
    dropId = 0.0;

    for (int ci = 0; ci < 10; ci++) {
        if (float(ci) >= N) break;
        float fi = float(ci);
        // Column X position
        float cx = (fi / (N - 1.0) - 0.5) * 2.0 * aspect * 0.65;
        float colPhase = hash11(fi * 3.17) * 6.28;

        // Multiple drops per column
        for (int di = 0; di < 6; di++) {
            float fdi = float(di);
            float dropPhase = hash21(vec2(fi, fdi)) * 2.0;
            float dropSpeed2 = dropSpeed * (0.6 + hash11(fi * 7.3 + fdi * 2.1) * 0.8);
            float cy = mod(-(t * dropSpeed2 + dropPhase + fdi * 0.5), 2.8) - 1.4;
            // Small X drift per drop
            float dx = sin(t * 0.3 + colPhase + fdi * 1.1) * 0.04;

            float d = length(p - vec3(cx + dx, cy, 0.0)) - dRadius;
            if (d < best) {
                best   = d;
                dropId = fi + fdi * 0.1;
            }
        }
    }
    return best;
}

vec3 calcNormal(vec3 p, float t) {
    float di;
    const vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, t, di) - scene(p - e.xyy, t, di),
        scene(p + e.yxy, t, di) - scene(p - e.yxy, t, di),
        scene(p + e.yyx, t, di) - scene(p - e.yyx, t, di)
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t     = TIME;
    float audio = 1.0 + audioLevel * audioReact;

    // Fixed frontal camera
    vec3 ro = vec3(0.0, 0.0, 2.5);
    vec3 rd = normalize(vec3(uv, -1.5));

    float dt   = 0.0;
    bool  hit  = false;
    float dSurf = 1.0;
    float dropId = 0.0;

    for (int i = 0; i < 80; i++) {
        vec3 p = ro + rd * dt;
        float di;
        dSurf = scene(p, t, di);
        if (dSurf < 0.001) {
            hit    = true;
            dropId = di;
            break;
        }
        if (dt > 6.0) break;
        dt += max(dSurf * 0.85, 0.005);
    }

    // Black void background
    vec3 col = vec3(0.006, 0.003, 0.001);

    if (hit) {
        vec3 p   = ro + rd * dt;
        vec3 nor = calcNormal(p, t);
        vec3 vd  = -rd;

        // 4-color metallic palette: copper, molten orange, gold, white-hot
        vec3 copper   = vec3(0.72, 0.30, 0.07);
        vec3 moltenOr = vec3(1.00, 0.38, 0.00);
        vec3 gold     = vec3(1.00, 0.72, 0.10);
        vec3 whiteHot = vec3(1.00, 0.95, 0.75);

        // Blend by vertical position + drop identity
        float heatFrac = fract(dropId * 0.31);
        vec3 baseCol = mix(copper, moltenOr, heatFrac);
        baseCol = mix(baseCol, gold, heatFrac * heatFrac);

        // Key light: warm from upper-left
        vec3 keyDir = normalize(vec3(-1.0, 2.0, 1.5));
        float diff  = max(0.0, dot(nor, keyDir));
        float spec  = pow(max(0.0, dot(reflect(-keyDir, nor), vd)), 48.0);

        // Environment reflection (metallic IBL sim)
        float envUp = max(0.0, nor.y);
        float envDn = max(0.0, -nor.y);

        col  = baseCol * (diff * 0.65 + envUp * 0.2 + 0.05);
        col += gold     * envUp  * hdrBoost * 0.5;
        col += moltenOr * envDn  * hdrBoost * 0.35;
        col += whiteHot * spec   * hdrBoost;
        col *= hdrBoost * 0.8;

        // Black ink edge
        float ew   = fwidth(dSurf) * 3.5;
        float edge = smoothstep(0.0, ew, abs(dSurf));
        col = mix(vec3(0.0), col, edge);
    }

    gl_FragColor = vec4(col, 1.0);
}
