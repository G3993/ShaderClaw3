/*{
    "DESCRIPTION": "Prismatic Gem — raymarched rotating diamond SDF with spectral facet HDR reflections. Cinematic studio lighting, black ink edges, linear HDR output.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Cinematic"],
    "INPUTS": [
        { "NAME": "spinSpeed",  "LABEL": "Spin Speed",  "TYPE": "float", "DEFAULT": 0.22,  "MIN": 0.0,  "MAX": 1.0 },
        { "NAME": "facetSharp", "LABEL": "Facet Edge",  "TYPE": "float", "DEFAULT": 32.0,  "MIN": 4.0,  "MAX": 128.0 },
        { "NAME": "gemSize",    "LABEL": "Gem Size",    "TYPE": "float", "DEFAULT": 0.65,  "MIN": 0.1,  "MAX": 1.5 },
        { "NAME": "hdrBoost",   "LABEL": "HDR Boost",   "TYPE": "float", "DEFAULT": 2.5,   "MIN": 1.0,  "MAX": 4.0 },
        { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.5,   "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

float sdOctahedron(vec3 p, float s) {
    p = abs(p);
    return (p.x + p.y + p.z - s) * 0.57735027;
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Truncated octahedron = gem shape
float sdGem(vec3 p, float r) {
    float oct  = sdOctahedron(p, r * 1.35);
    float clip = sdBox(p, vec3(r * 0.88, r * 0.62, r * 0.88));
    return max(oct, clip);
}

vec3 calcNormal(vec3 p, float r) {
    const vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sdGem(p + e.xyy, r) - sdGem(p - e.xyy, r),
        sdGem(p + e.yxy, r) - sdGem(p - e.yxy, r),
        sdGem(p + e.yyx, r) - sdGem(p - e.yyx, r)
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

    float t    = TIME;
    float audio = 1.0 + audioLevel * audioReact;
    float r    = gemSize * audio;

    // Drifting camera orbit
    vec3 ro = vec3(sin(t * 0.18) * 0.4, 0.28 + sin(t * 0.09) * 0.18, 2.9);
    vec3 target = vec3(0.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd * 1.7 + uv.x * right + uv.y * up);

    // Gem spin angle
    float ang = t * spinSpeed;
    float ca = cos(ang), sa = sin(ang);

    // Raymarch
    float dt   = 0.0;
    bool  hit  = false;
    float dSurf = 1.0;
    for (int i = 0; i < 80; i++) {
        vec3 p = ro + rd * dt;
        // Rotate in XZ
        vec3 pr = vec3(p.x * ca + p.z * sa, p.y, -p.x * sa + p.z * ca);
        dSurf = sdGem(pr, r);
        if (dSurf < 0.001) { hit = true; break; }
        if (dt > 10.0) break;
        dt += max(dSurf * 0.85, 0.005);
    }

    // Black void background + sparse star field
    vec3 col = vec3(0.003, 0.001, 0.010);
    float sv = hash11(floor(uv.x * 110.0) + floor(uv.y * 110.0) * 137.0);
    col += vec3(0.7, 0.85, 1.0) * step(0.996, sv) * 0.5;

    if (hit) {
        vec3 p  = ro + rd * dt;
        vec3 pr = vec3(p.x * ca + p.z * sa, p.y, -p.x * sa + p.z * ca);
        vec3 nor = calcNormal(pr, r);
        vec3 vd  = -rd;

        // Three studio lights
        vec3 keyDir  = normalize(vec3( 1.8,  2.5,  1.2));
        vec3 fillDir = normalize(vec3(-1.4,  0.9,  0.7));
        vec3 rimDir  = normalize(vec3( 0.0, -1.1, -1.6));

        float kD = max(0.0, dot(nor, keyDir));
        float fD = max(0.0, dot(nor, fillDir));
        float rD = max(0.0, dot(nor, rimDir));
        float kS = pow(max(0.0, dot(reflect(-keyDir,  nor), vd)), facetSharp);
        float fS = pow(max(0.0, dot(reflect(-fillDir, nor), vd)), facetSharp * 0.4);

        // Spectral hue from normal orientation
        float hue  = fract(dot(nor, vec3(0.42, 0.31, 0.27)) * 2.1 + 0.07);
        vec3 facet = hsvRgb(hue, 1.0, 1.0);

        col  = facet * (kD * 0.65 + fD * 0.18 + rD * 0.14 + 0.04);
        col += vec3(1.0) * kS * hdrBoost;                   // white-hot key spec
        col += vec3(0.45, 0.75, 1.0) * fS * hdrBoost * 0.4; // cyan fill spec
        col *= hdrBoost * 0.75;

        // Black ink edge via fwidth AA
        float ew   = fwidth(dSurf) * 3.5;
        float edge = smoothstep(0.0, ew, abs(dSurf));
        col = mix(vec3(0.0), col, edge);
    }

    gl_FragColor = vec4(col, 1.0);
}
