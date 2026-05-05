/*{
    "DESCRIPTION": "Neon Gyroscopes — 4 nested rotating torus rings in deep void. Studio key light. Fully saturated HDR palette. 64-step raymarch.",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "speed",   "TYPE": "float", "DEFAULT": 0.5,   "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Spin Speed" },
        { "NAME": "tubeR",   "TYPE": "float", "DEFAULT": 0.055, "MIN": 0.02, "MAX": 0.15, "LABEL": "Tube Radius" },
        { "NAME": "hdrPeak", "TYPE": "float", "DEFAULT": 2.5,   "MIN": 1.0,  "MAX": 4.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod","TYPE": "float", "DEFAULT": 0.6,   "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c,-s,s,c); }

float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

float ringDist(vec3 p, int ri) {
    float f    = float(ri) * 0.25;
    float R    = 0.30 + f * 0.55;
    float dir  = (ri < 2) ? 1.0 : -1.0;
    float spin = TIME * speed * (0.3 + hash11(float(ri)) * 0.7) * dir;
    vec3 q = p;
    q.xz = rot2(spin)          * q.xz;
    q.xy = rot2(f * 1.5 + 0.2) * q.xy;
    q.yz = rot2(f * 0.8 + 0.4) * q.yz;
    return sdTorus(q, R, tubeR);
}

float sceneSDF(vec3 p) {
    float d = ringDist(p, 0);
    d = min(d, ringDist(p, 1));
    d = min(d, ringDist(p, 2));
    d = min(d, ringDist(p, 3));
    return d;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.0012, 0.0);
    return normalize(vec3(
        sceneSDF(p + e.xyy) - sceneSDF(p - e.xyy),
        sceneSDF(p + e.yxy) - sceneSDF(p - e.yxy),
        sceneSDF(p + e.yyx) - sceneSDF(p - e.yyx)
    ));
}

int hitRing(vec3 p) {
    float d0 = ringDist(p, 0);
    float d1 = ringDist(p, 1);
    float d2 = ringDist(p, 2);
    float d3 = ringDist(p, 3);
    int best = 0; float dmin = d0;
    if (d1 < dmin) { dmin = d1; best = 1; }
    if (d2 < dmin) { dmin = d2; best = 2; }
    if (d3 < dmin) { dmin = d3; best = 3; }
    return best;
}

vec3 ringColor(int i) {
    if (i == 0) return vec3(1.0, 0.05, 0.8);  // hot magenta
    if (i == 1) return vec3(0.0, 0.9,  1.0);  // electric cyan
    if (i == 2) return vec3(1.0, 0.75, 0.0);  // gold
    return             vec3(0.55, 0.1, 1.0);  // violet
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;

    // Slow camera orbit for liveness
    float camA = TIME * 0.12;
    vec3 ro = vec3(sin(camA) * 0.4, 0.45, 3.0 + cos(camA * 0.7) * 0.2);
    vec3 fw  = normalize(vec3(0.0) - ro);
    vec3 rgt = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up  = cross(rgt, fw);
    vec3 rd  = normalize(fw + uv.x * rgt * 0.7 + uv.y * up * 0.7);

    // 64-step march
    float dist = 0.0;
    bool  hit  = false;
    for (int i = 0; i < 64; i++) {
        float d = sceneSDF(ro + rd * dist);
        if (d < 0.001) { hit = true; break; }
        dist += d;
        if (dist > 8.0) break;
    }

    vec3 col = vec3(0.0); // deep void

    if (hit) {
        vec3 p    = ro + rd * dist;
        vec3 N    = calcNormal(p);
        int  rid  = hitRing(p);
        vec3 base = ringColor(rid);

        // Studio key (warm upper-left) + dim cool fill
        vec3 key  = normalize(vec3(-0.8, 1.2, -0.5));
        vec3 fill = normalize(vec3(1.0,  0.3,  0.6));
        float kD  = max(dot(N, key),  0.0);
        float fD  = max(dot(N, fill), 0.0) * 0.25;
        float sp  = pow(max(dot(reflect(-key, N), -rd), 0.0), 48.0);

        // fwidth silhouette AA — edge fadeout
        float edgeW = fwidth(dist);
        float edgeFade = 1.0 - smoothstep(0.0, edgeW * 5.0, 0.0);

        col  = base * (kD + fD + 0.07) * hdrPeak * audio;
        col += vec3(1.0) * sp * hdrPeak * 0.9; // white-hot specular
    }

    gl_FragColor = vec4(col, 1.0);
}
