/*{
  "DESCRIPTION": "Neon Jellyfish — 5 raymarched SDF jellyfish drifting in deep ocean, fully saturated bioluminescent palette, HDR glow",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "jellyfishCount", "LABEL": "Count",       "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0, "MAX": 8.0 },
    { "NAME": "driftSpeed",     "LABEL": "Drift Speed", "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "tentacleLen",    "LABEL": "Tentacle Len","TYPE": "float", "DEFAULT": 1.4,  "MIN": 0.3, "MAX": 3.0 },
    { "NAME": "glowAmt",        "LABEL": "Glow",        "TYPE": "float", "DEFAULT": 2.2,  "MIN": 0.5, "MAX": 4.0 },
    { "NAME": "audioPulse",     "LABEL": "Audio Pulse", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5); }
float hash2(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5); }

// Bell cap: squashed sphere carved from below (open underside)
float sdBell(vec3 p, float r) {
    vec3 q = p / vec3(1.0, 0.65, 1.0);
    return max(length(q) - r, -p.y - r * 0.25);
}

// Rim torus around bell edge
float sdRim(vec3 p, float r) {
    vec2 q = vec2(length(p.xz) - r * 0.88, p.y + r * 0.22);
    return length(q) - r * 0.09;
}

// Capsule for tentacles
float sdCap(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

// Single jellyfish SDF, returns (dist, matID)
vec2 sdJelly(vec3 p, float id) {
    float r = 0.27 + hash1(id * 7.3) * 0.13;
    float d = min(sdBell(p, r), sdRim(p, r));
    float best = d; float mid = 0.0;

    // 8 tentacles waving over time
    for (int i = 0; i < 8; i++) {
        float fi = float(i);
        float ang = fi * 0.7854; // 2PI/8
        float phase = hash1(id * 3.1 + fi);
        float wag = sin(TIME * 1.4 + fi * 0.9 + phase * 6.28) * 0.18;
        vec3 ta = vec3(sin(ang) * r * 0.72, -r * 0.08, cos(ang) * r * 0.72);
        vec3 tb = ta + vec3(wag, -tentacleLen * r * 3.5, wag * 0.7);
        float td = sdCap(p, ta, tb, 0.017);
        if (td < best) { best = td; mid = 0.5; }
    }
    return vec2(best, mid);
}

// Full scene: N jellyfish, returns (dist, colorID)
vec2 scene(vec3 p) {
    float bestD = 1e9; float bestId = -1.0;
    int N = int(clamp(jellyfishCount, 1.0, 8.0));
    for (int i = 0; i < 8; i++) {
        if (i >= N) break;
        float fi = float(i);
        float ph  = hash1(fi * 3.7) * 6.283;
        float rad = 0.65 + hash1(fi * 5.1) * 0.9;
        float spd = (0.22 + hash1(fi * 2.3) * 0.35) * driftSpeed;
        vec3 ctr = vec3(
            sin(fi * 2.399 + TIME * spd * 0.6 + ph) * rad,
            sin(TIME * spd * 0.9 + ph + 1.1) * 0.6,
            cos(fi * 2.399 + TIME * spd * 0.5 + ph) * rad
        );
        float pulse = 1.0 + audioBass * audioPulse * 0.14;
        vec3 lp = (p - ctr) / pulse;
        vec2 res = sdJelly(lp, fi);
        float di = res.x * pulse;
        if (di < bestD) {
            bestD = di;
            bestId = fi + res.y * 0.01; // encode bell vs tentacle in fractional part
        }
    }
    return vec2(bestD, bestId);
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        scene(p + e.xyy).x - scene(p - e.xyy).x,
        scene(p + e.yxy).x - scene(p - e.yxy).x,
        scene(p + e.yyx).x - scene(p - e.yyx).x
    ));
}

// Saturated 5-color bioluminescent palette
vec3 jellyColor(float id) {
    int ci = int(mod(id, 5.0));
    if (ci == 0) return vec3(0.0,  1.0,  0.9);   // electric cyan
    if (ci == 1) return vec3(1.0,  0.05, 0.75);  // hot magenta
    if (ci == 2) return vec3(0.35, 1.0,  0.0);   // chartreuse
    if (ci == 3) return vec3(0.55, 0.0,  1.0);   // violet
               return vec3(1.0,  0.55, 0.0);     // orange
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Orbiting camera
    float camA = TIME * 0.14;
    vec3 ro = vec3(sin(camA) * 3.8, sin(TIME * 0.19) * 0.65, cos(camA) * 3.8);
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x * ri + uv.y * up + 1.7 * fw);

    // Deep ocean background gradient
    float bgH = dot(rd, vec3(0.0, 1.0, 0.0)) * 0.5 + 0.5;
    vec3 bg = mix(vec3(0.0, 0.003, 0.015), vec3(0.0, 0.02, 0.08), bgH * bgH);

    // Raymarch
    float t = 0.1; float bestId = -1.0;
    for (int i = 0; i < 64; i++) {
        vec2 res = scene(ro + rd * t);
        if (res.x < 0.001) { bestId = res.y; break; }
        if (t > 14.0) break;
        t += res.x * 0.85;
    }

    vec3 col = bg;

    if (bestId >= 0.0) {
        vec3 p = ro + rd * t;
        vec3 n = getNormal(p);
        vec3 L  = normalize(vec3(0.4, 1.0, 0.25));
        vec3 L2 = normalize(vec3(-0.3, 0.3, -0.8));

        float jellId = floor(bestId);
        vec3 jcol = jellyColor(jellId);

        float diff  = max(dot(n, L), 0.0) * 0.6 + max(dot(n, L2), 0.0) * 0.2 + 0.2;
        float spec  = pow(max(dot(reflect(-L, n), -rd), 0.0), 24.0);
        float rim   = pow(clamp(1.0 - dot(n, -rd), 0.0, 1.0), 3.0);
        float pulse = 1.0 + audioMid * audioPulse * 0.4;

        // Ink-dark silhouette: faces pointing away from viewer are darker
        float face = clamp(dot(n, -rd), 0.0, 1.0);
        float ink  = smoothstep(0.0, 0.3, face);

        col  = jcol * diff * glowAmt * pulse * ink;
        col += jcol * rim  * glowAmt * 1.3;       // HDR rim
        col += vec3(1.0)   * spec * glowAmt;       // HDR specular
        // Subsurface ocean bleed
        col  = mix(col, bg + jcol * 0.3, 0.12);
    }

    // Volumetric bioluminescent micro-organisms
    float moteAcc = 0.0;
    for (int i = 0; i < 18; i++) {
        float fi = float(i);
        vec3 pc = vec3(
            sin(fi * 1.31 + TIME * 0.17) * 2.8,
            cos(fi * 2.71 + TIME * 0.13) * 1.8,
            sin(fi * 3.17 + TIME * 0.11) * 2.8
        );
        vec3 dv = pc - ro;
        float tcl = dot(rd, dv);
        if (tcl > 0.001 && tcl < t + 0.1) {
            float d2 = length(dv - rd * tcl);
            if (d2 < 0.06) moteAcc += (0.06 - d2) * 25.0;
        }
    }
    col += vec3(0.15, 0.85, 1.0) * moteAcc * 0.35;

    gl_FragColor = vec4(col, 1.0);
}
