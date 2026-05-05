/*{
    "DESCRIPTION": "Holographic Shatter — array of raymarched thin iridescent shard-planes floating in void, view-angle-dependent holo coloring. Magenta/cyan/gold palette: complete contrast to prior CRT phosphor green.",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator", "3D", "Holographic"],
    "INPUTS": [
        { "NAME": "shardCount",  "LABEL": "Shard Count",  "TYPE": "float", "DEFAULT": 8.0,  "MIN": 3.0,  "MAX": 14.0 },
        { "NAME": "shardSize",   "LABEL": "Shard Size",   "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.05, "MAX": 0.8 },
        { "NAME": "driftSpeed",  "LABEL": "Drift Speed",  "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 1.5 },
        { "NAME": "hdrBoost",    "LABEL": "HDR Boost",    "TYPE": "float", "DEFAULT": 2.4,  "MIN": 1.0,  "MAX": 4.0 },
        { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Rotate around arbitrary axis
vec3 rotAxis(vec3 p, vec3 ax, float a) {
    float c = cos(a), s = sin(a);
    return p * c + cross(ax, p) * s + ax * dot(ax, p) * (1.0 - c);
}

// Thin shard: flat box with arbitrary orientation
float shard(vec3 p, float fi, float t) {
    float h11   = hash11(fi * 3.17);
    float h21   = hash11(fi * 5.37 + 0.5);
    float h31   = hash11(fi * 7.53 + 1.0);
    float h41   = hash11(fi * 11.7 + 2.0);

    // Position
    float theta = fi / shardCount * 6.28318;
    float radius = 0.4 + h11 * 0.5;
    float dz    = (h21 - 0.5) * 1.2;
    vec3 center = vec3(cos(theta) * radius, sin(theta) * radius * 0.6 + dz, dz * 0.4);

    // Drift
    center += vec3(sin(t * driftSpeed * (0.4 + h31 * 0.6) + h21 * 6.28) * 0.12,
                   cos(t * driftSpeed * (0.3 + h41 * 0.5) + h31 * 6.28) * 0.12,
                   sin(t * driftSpeed * 0.25 + h11 * 6.28) * 0.08);

    // Orientation
    vec3 ax1 = normalize(vec3(h11, h21, h31) * 2.0 - 1.0);
    float rot = t * driftSpeed * (h41 - 0.5) * 0.4 + h11 * 6.28;

    vec3 lp = rotAxis(p - center, ax1, rot);
    // Thin slab: large in XY, thin in Z
    return sdBox(lp, vec3(shardSize * (0.6 + h21 * 0.7), shardSize * (0.4 + h31 * 0.6), 0.012));
}

float scene(vec3 p, float t, out float shardId) {
    float audio = 1.0 + audioLevel * audioReact * 0.15;
    float best  = 1e8;
    shardId     = 0.0;

    int N = int(clamp(shardCount, 3.0, 14.0));
    for (int i = 0; i < 14; i++) {
        if (i >= N) break;
        float fi = float(i);
        float d  = shard(p, fi, t) / audio;
        if (d < best) { best = d; shardId = fi; }
    }
    return best;
}

vec3 calcNormal(vec3 p, float t) {
    float si;
    const vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        scene(p + e.xyy, t, si) - scene(p - e.xyy, t, si),
        scene(p + e.yxy, t, si) - scene(p - e.yxy, t, si),
        scene(p + e.yyx, t, si) - scene(p - e.yyx, t, si)
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

    // Slowly orbiting camera
    float camA = t * 0.08;
    vec3 ro = vec3(cos(camA) * 2.2, sin(t * 0.05) * 0.5, sin(camA) * 2.2);
    vec3 fwd   = normalize(-ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(fwd * 1.5 + uv.x * right + uv.y * up);

    float dt   = 0.0;
    bool  hit  = false;
    float dSurf = 1.0;
    float shardId = 0.0;

    for (int i = 0; i < 80; i++) {
        vec3 p = ro + rd * dt;
        float si;
        dSurf = scene(p, t, si);
        if (dSurf < 0.001) {
            hit     = true;
            shardId = si;
            break;
        }
        if (dt > 8.0) break;
        dt += max(dSurf * 0.85, 0.005);
    }

    // Void black background
    vec3 col = vec3(0.003, 0.001, 0.006);

    if (hit) {
        vec3 p   = ro + rd * dt;
        vec3 nor = calcNormal(p, t);
        vec3 vd  = -rd;

        // Iridescent: hue from view angle (like oil-film interference)
        float iridHue = fract(dot(nor, vd) * 1.8 + shardId * 0.13 + t * 0.03);
        vec3 iridCol  = hsvRgb(iridHue, 1.0, 1.0);

        // Palette: hot magenta, electric blue, cyan, white-hot
        // Controlled by shard index
        float palIdx = fract(shardId * 0.37);
        vec3 magenta = vec3(1.00, 0.00, 0.75);
        vec3 elecBlue = vec3(0.10, 0.35, 1.00);
        vec3 elecCyan = vec3(0.00, 0.90, 1.00);
        vec3 baseCol  = mix(mix(magenta, elecBlue, palIdx), elecCyan, palIdx * palIdx);

        vec3 keyDir = normalize(vec3(1.0, 2.0, 1.0));
        float diff  = max(0.0, dot(nor, keyDir));
        float spec  = pow(max(0.0, dot(reflect(-keyDir, nor), vd)), 64.0);
        float fres  = pow(1.0 - max(0.0, dot(nor, vd)), 3.0);

        col  = baseCol * (diff * 0.4 + 0.05);
        col += iridCol * fres   * hdrBoost * 0.8;
        col += vec3(1.0) * spec * hdrBoost;
        col *= hdrBoost * 0.85;

        // Black ink edge (thin slabs have sharp silhouettes)
        float ew   = fwidth(dSurf) * 4.0;
        float edge = smoothstep(0.0, ew, abs(dSurf));
        col = mix(vec3(0.0), col, edge);
    }

    gl_FragColor = vec4(col, 1.0);
}
