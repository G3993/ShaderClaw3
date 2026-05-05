/*{
    "DESCRIPTION": "Orbital Rings — raymarched nested torus-SDF rings forming a Fabergé lattice. Painterly gold-magenta-emerald HDR palette, orbiting camera, black ink silhouette.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Painterly"],
    "INPUTS": [
        { "NAME": "ringCount",  "LABEL": "Ring Count",  "TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0,  "MAX": 7.0 },
        { "NAME": "spinSpeed",  "LABEL": "Spin Speed",  "TYPE": "float", "DEFAULT": 0.32, "MIN": 0.0,  "MAX": 1.5 },
        { "NAME": "tubeGirth",  "LABEL": "Tube Girth",  "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.01, "MAX": 0.25 },
        { "NAME": "hdrBoost",   "LABEL": "HDR Boost",   "TYPE": "float", "DEFAULT": 2.4,  "MIN": 1.0,  "MAX": 4.0 },
        { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

// Rotate around Y axis
vec3 rotY(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(p.x * c + p.z * s, p.y, -p.x * s + p.z * c);
}

// Rotate around X axis
vec3 rotX(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(p.x, p.y * c - p.z * s, p.y * s + p.z * c);
}

// Rotate around Z axis
vec3 rotZ(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(p.x * c - p.y * s, p.x * s + p.y * c, p.z);
}

float scene(vec3 p, float t) {
    float audio = 1.0 + audioLevel * audioReact * 0.25;
    float R = 0.5 * audio;
    float tube = tubeGirth;
    float spin = t * spinSpeed;

    int N = int(clamp(ringCount, 1.0, 7.0));
    float d = 1e8;

    // Ring 0: equatorial (XZ plane)
    d = min(d, sdTorus(rotY(p, spin * 0.7),            R,      tube));

    if (N >= 2)
    d = min(d, sdTorus(rotZ(rotY(p, spin * -0.5), 1.5708), R,      tube));

    if (N >= 3)
    d = min(d, sdTorus(rotX(rotY(p, spin * 0.4), 1.5708), R,      tube));

    if (N >= 4)
    d = min(d, sdTorus(rotZ(rotY(p, spin * 0.9), 0.7854), R * 0.85, tube * 0.85));

    if (N >= 5)
    d = min(d, sdTorus(rotX(rotY(p, spin * -0.8), 0.7854), R * 0.85, tube * 0.85));

    if (N >= 6)
    d = min(d, sdTorus(rotZ(rotY(p, spin * 0.6), 2.3562), R * 0.70, tube * 0.70));

    if (N >= 7)
    d = min(d, sdTorus(rotX(rotY(p, spin * -1.1), 2.3562), R * 0.70, tube * 0.70));

    return d;
}

vec3 calcNormal(vec3 p, float t) {
    const vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, t) - scene(p - e.xyy, t),
        scene(p + e.yxy, t) - scene(p - e.yxy, t),
        scene(p + e.yyx, t) - scene(p - e.yyx, t)
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

    float t = TIME;

    float camA = t * 0.11;
    vec3 ro = vec3(cos(camA) * 2.0, 0.55 + sin(t * 0.07) * 0.35, sin(camA) * 2.0);
    vec3 fwd   = normalize(-ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd * 1.6 + uv.x * right + uv.y * up);

    float dt   = 0.0;
    bool  hit  = false;
    float dSurf = 1.0;
    for (int i = 0; i < 80; i++) {
        vec3 p = ro + rd * dt;
        dSurf = scene(p, t);
        if (dSurf < 0.001) { hit = true; break; }
        if (dt > 8.0) break;
        dt += max(dSurf * 0.85, 0.005);
    }

    // Deep void background
    vec3 col = vec3(0.004, 0.002, 0.008);

    if (hit) {
        vec3 p   = ro + rd * dt;
        vec3 nor = calcNormal(p, t);
        vec3 vd  = -rd;

        // 4-color palette: gold, magenta, emerald, white-hot
        // Choose by position in 3D
        float hue = fract(dot(p, vec3(0.4, 0.7, 0.3)) + t * 0.04);
        vec3 ringCol = hsvRgb(hue, 1.0, 1.0);

        vec3 keyDir = normalize(vec3(1.5, 2.0, 1.0));
        float diff  = max(0.0, dot(nor, keyDir));
        float spec  = pow(max(0.0, dot(reflect(-keyDir, nor), vd)), 24.0);
        float rim   = pow(1.0 - max(0.0, dot(nor, vd)), 3.5);

        col  = ringCol * (diff * 0.7 + 0.08);
        col += vec3(1.0)      * spec * hdrBoost;
        col += ringCol        * rim  * hdrBoost * 0.5;
        col *= hdrBoost * 0.8;

        // Black ink edge
        float ew   = fwidth(dSurf) * 4.0;
        float edge = smoothstep(0.0, ew, abs(dSurf));
        col = mix(vec3(0.0), col, edge);
    }

    gl_FragColor = vec4(col, 1.0);
}
