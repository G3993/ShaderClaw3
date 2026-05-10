/*{
  "DESCRIPTION": "3D Neon Crystal Cave — conical stalactite crystals hanging from a dark ceiling, lit by three colored point lights from below. Palette: electric teal, deep magenta, gold on void black. LINEAR HDR peaks 2.5+; camera orbits slowly upward. Audio modulates glow pulse.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "crystalCount", "LABEL": "Crystal Count", "TYPE": "float", "DEFAULT": 7.0,  "MIN": 3.0, "MAX": 12.0 },
    { "NAME": "crystalScale", "LABEL": "Crystal Scale", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.1, "MAX": 0.8 },
    { "NAME": "swaySpeed",    "LABEL": "Sway Speed",    "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "glowPeak",     "LABEL": "Glow Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 5.0 },
    { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define MAX_DIST  18.0
#define EPS       0.002
#define PI        3.14159265

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// Tapered cone crystal: capsule with linearly shrinking radius toward tip
float sdCrystal(vec3 p, vec3 top, vec3 tip, float rTop) {
    vec3 ab = tip - top;
    vec3 ap = p - top;
    float h = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
    float r = mix(rTop, 0.0, h);
    return length(ap - h * ab) - r;
}

// Floor plane at y = -3.2
float sdFloor(vec3 p) { return p.y + 3.2; }

struct Hit { float d; int id; };

Hit map(vec3 p, float t) {
    Hit h;
    h.d = MAX_DIST;
    h.id = -1;

    float fl = sdFloor(p);
    if (fl < h.d) { h.d = fl; h.id = 0; }

    float nf = crystalCount;
    for (int i = 0; i < 12; i++) {
        if (float(i) >= nf) break;
        float fi = float(i);
        float s1 = hash11(fi * 7.31 + 1.1);
        float s2 = hash11(fi * 3.17 + 2.3);
        float angle = fi * (2.0 * PI / nf) + s1 * 1.2;
        float ring  = 0.9 + s2 * 1.8;

        float scale = crystalScale / 0.35;
        float swX = sin(t * swaySpeed * 0.7 + fi * 1.3) * 0.08 * scale;
        float swZ = cos(t * swaySpeed * 0.6 + fi * 2.1) * 0.08 * scale;

        vec3 top = vec3(cos(angle) * ring + swX, 4.6, sin(angle) * ring + swZ);
        float len = (1.5 + s1 * 1.8) * scale;
        vec3 tip = vec3(top.x + swX * 0.2, top.y - len, top.z + swZ * 0.2);
        float rTop = 0.13 * scale;

        float cd = sdCrystal(p, top, tip, rTop);
        if (cd < h.d) { h.d = cd; h.id = 1 + (i % 3); }
    }
    return h;
}

vec3 getNormal(vec3 p, float t) {
    vec2 e = vec2(EPS, 0.0);
    return normalize(vec3(
        map(p + e.xyy, t).d - map(p - e.xyy, t).d,
        map(p + e.yxy, t).d - map(p - e.yxy, t).d,
        map(p + e.yyx, t).d - map(p - e.yyx, t).d
    ));
}

void main() {
    vec2 res = RENDERSIZE;
    vec2 uv  = (isf_FragNormCoord.xy - 0.5) * vec2(res.x / res.y, 1.0);
    float t  = TIME;

    // Audio modulates glow — capped K ≤ 1.2 per motion rules §2
    float audio  = clamp(audioLevel * audioReact, 0.0, 1.5);
    float aGlow  = 1.0 + audio * min(audioReact * 0.5, 1.2);

    // Camera: slow orbit below the crystal field, looking up
    float spin = t * 0.07;
    vec3 ro = vec3(sin(spin) * 0.7, -2.0, cos(spin) * 0.7);
    vec3 target = vec3(0.0, 2.2, 0.0);
    vec3 fwd   = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up    = cross(right, fwd);
    vec3 rd    = normalize(fwd + uv.x * right + uv.y * up * 0.82);

    // Raymarch
    float td = 0.0;
    Hit h; h.d = MAX_DIST; h.id = -1;
    for (int i = 0; i < MAX_STEPS; i++) {
        Hit s = map(ro + rd * td, t);
        if (s.d < EPS) { h = s; break; }
        td += s.d;
        if (td > MAX_DIST) break;
    }

    // HDR palette — fully saturated, no white-mixing
    vec3 palTeal    = vec3(0.0,  1.0,  0.85);
    vec3 palMagenta = vec3(1.0,  0.05, 0.90);
    vec3 palGold    = vec3(1.0,  0.72, 0.0);
    vec3 palFloor   = vec3(0.006, 0.003, 0.015);

    // Three point lights positioned below the crystal field
    vec3 lPos[3];
    lPos[0] = vec3( 1.2, -2.6,  0.7);
    lPos[1] = vec3(-1.1, -2.6, -0.6);
    lPos[2] = vec3( 0.1, -2.6,  1.4);
    vec3 lCol[3];
    lCol[0] = palTeal    * 3.0 * glowPeak * aGlow;
    lCol[1] = palMagenta * 2.5 * glowPeak * aGlow;
    lCol[2] = palGold    * 2.2 * glowPeak * aGlow;

    vec3 col = vec3(0.004, 0.001, 0.010); // void bg

    if (h.id >= 0) {
        vec3 pos = ro + rd * td;
        vec3 nor = getNormal(pos, t);

        vec3 baseCol = (h.id == 0) ? palFloor :
                       (h.id == 1) ? palTeal  :
                       (h.id == 2) ? palMagenta : palGold;

        vec3 lit = vec3(0.0);
        for (int li = 0; li < 3; li++) {
            vec3 ld   = lPos[li] - pos;
            float d2  = dot(ld, ld);
            ld        = normalize(ld);
            float diff = max(dot(nor, ld), 0.0);
            float att  = 1.0 / (1.0 + d2 * 0.10);
            lit += lCol[li] * diff * att;

            // Specular: sharp glint on facets
            vec3 hv   = normalize(ld - rd);
            float sp  = pow(max(dot(nor, hv), 0.0), 64.0);
            lit += lCol[li] * sp * 0.7;
        }

        // fwidth edge glow — crystal rim catch-light
        float edgeW   = fwidth(h.d);
        float edgeGlow = exp(-edgeW * 60.0) * glowPeak * aGlow;
        lit += baseCol * edgeGlow;

        col = baseCol * 0.03 + lit * 0.35;
    }

    // Volumetric crystal glow along ray (soft halo around each stalactite axis)
    float nf = crystalCount;
    for (int i = 0; i < 12; i++) {
        if (float(i) >= nf) break;
        float fi  = float(i);
        float s1  = hash11(fi * 7.31 + 1.1);
        float s2  = hash11(fi * 3.17 + 2.3);
        float angle = fi * (2.0 * PI / nf) + s1 * 1.2;
        float ring  = 0.9 + s2 * 1.8;
        float scale = crystalScale / 0.35;
        float swX = sin(t * swaySpeed * 0.7 + fi * 1.3) * 0.08 * scale;
        float swZ = cos(t * swaySpeed * 0.6 + fi * 2.1) * 0.08 * scale;
        vec3 ctr = vec3(cos(angle) * ring + swX, 1.5, sin(angle) * ring + swZ);

        vec3 rToC = ctr - ro;
        float proj = dot(rToC, rd);
        if (proj > 0.0 && proj < MAX_DIST) {
            float closestD = length(rToC - rd * proj);
            float g = exp(-closestD * closestD * 9.0) * 0.10;
            vec3 gc = (i % 3 == 0) ? palTeal :
                      (i % 3 == 1) ? palMagenta : palGold;
            col += gc * g * glowPeak * aGlow * 0.5;
        }
    }

    gl_FragColor = vec4(max(col, vec3(0.0)), 1.0);
}
