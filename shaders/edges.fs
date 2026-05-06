/*{
    "DESCRIPTION": "3D Neon Ring Gyroscope — three interlocked counter-rotating tori raymarched in HDR neon, with volumetric glow.",
    "CATEGORIES": ["Generator", "3D"],
    "CREDIT": "ShaderClaw — gyroscope v2",
    "INPUTS": [
        { "NAME": "gyroSpeed",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0,  "LABEL": "Rotation Speed" },
        { "NAME": "tubeRadius",  "TYPE": "float", "DEFAULT": 0.048,"MIN": 0.01,"MAX": 0.15, "LABEL": "Tube Thickness" },
        { "NAME": "ringScale",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.4, "MAX": 2.0,  "LABEL": "Ring Scale" },
        { "NAME": "glowRadius",  "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.02,"MAX": 0.8,  "LABEL": "Glow Radius" },
        { "NAME": "hdrPeak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 4.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioReact",  "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio React" },
        { "NAME": "bg",          "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.01, 1.0], "LABEL": "Background" }
    ]
}*/

#define PI 3.14159265359
#define MAX_STEPS 64
#define MAX_DIST  7.0
#define SURF_DIST 0.0012

// ── Rotation helpers ──────────────────────────────────────────────────────────
vec3 rotX(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(p.x, c*p.y - s*p.z, s*p.y + c*p.z);
}
vec3 rotY(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(c*p.x + s*p.z, p.y, -s*p.x + c*p.z);
}
vec3 rotZ(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(c*p.x - s*p.y, s*p.x + c*p.y, p.z);
}

// ── Torus SDF (natural axis: Y, ring in XZ plane) ─────────────────────────────
float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

// ── Per-ring positions with independent spins + global tumble ─────────────────
// Returns SDFs for all 3 rings given world point p and time t.
void ringSDFs(vec3 p, float t, float R, float tr,
              out float d1, out float d2, out float d3) {
    float gs = gyroSpeed;

    // Global slow tumble — makes the whole gyroscope precess
    p = rotY(p, t * gs * 0.13);
    p = rotX(p, t * gs * 0.07);

    // Ring 1: XZ plane, spins around Y
    vec3 p1 = rotY(p, t * gs * 0.80);
    d1 = sdTorus(p1, R, tr);

    // Ring 2: XY plane (rotX by -90°), spins around Z
    vec3 p2 = rotZ(p, t * gs * 1.10);
    p2 = rotX(p2, -PI * 0.5);
    d2 = sdTorus(p2, R, tr);

    // Ring 3: YZ plane (rotZ by -90°), spins around X
    vec3 p3 = rotX(p, t * gs * 0.65);
    p3 = rotZ(p3, -PI * 0.5);
    d3 = sdTorus(p3, R, tr);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;
    float audioBoost = 1.0 + audioLevel * audioReact * 0.8
                           + audioBass  * audioReact * 0.4;
    float R  = ringScale;
    float tr = tubeRadius;

    // Neon colors — fully saturated, no white dilution
    vec3 CYAN    = vec3(0.0, 1.0, 1.0)  * hdrPeak;
    vec3 MAGENTA = vec3(1.0, 0.0, 0.9)  * hdrPeak;
    vec3 GOLD    = vec3(1.0, 0.75, 0.0) * hdrPeak;

    // Camera setup
    vec3 ro = vec3(0.0, 0.0, 3.4);
    vec3 rd = normalize(vec3(uv, -2.1));

    // Raymarch
    float dist = 0.05;
    vec3 glowAccum = vec3(0.0);
    int hitRing = -1;
    float gStr = 10.0 / max(glowRadius, 0.02);

    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * dist;
        float d1, d2, d3;
        ringSDFs(p, t, R, tr, d1, d2, d3);

        // Volumetric glow: exp decay from ring SURFACE (d - tr = distance outside tube)
        float s1 = max(d1 - tr, 0.0);
        float s2 = max(d2 - tr, 0.0);
        float s3 = max(d3 - tr, 0.0);
        glowAccum += CYAN    * exp(-s1 * gStr) * 0.055;
        glowAccum += MAGENTA * exp(-s2 * gStr) * 0.055;
        glowAccum += GOLD    * exp(-s3 * gStr) * 0.055;

        float minD = min(min(d1, d2), d3);

        if (minD < SURF_DIST) {
            if (d1 < d2 && d1 < d3) hitRing = 0;
            else if (d2 < d3)        hitRing = 1;
            else                     hitRing = 2;
            break;
        }
        dist += minD;
        if (dist > MAX_DIST) break;
    }

    // Surface shading: flat neon with Fresnel-edge brightening
    vec3 col = bg.rgb;
    if (hitRing >= 0) {
        vec3 p = ro + rd * dist;
        float d1, d2, d3;
        ringSDFs(p, t, R, tr, d1, d2, d3);

        // Numerical normal
        float eps = 0.001;
        vec3 nd; float da, db, dc;
        ringSDFs(p + vec3(eps,0,0), t, R, tr, da, db, dc);
        float dxa = (hitRing==0) ? da : (hitRing==1) ? db : dc;
        ringSDFs(p - vec3(eps,0,0), t, R, tr, da, db, dc);
        float dxb = (hitRing==0) ? da : (hitRing==1) ? db : dc;
        ringSDFs(p + vec3(0,eps,0), t, R, tr, da, db, dc);
        float dya = (hitRing==0) ? da : (hitRing==1) ? db : dc;
        ringSDFs(p - vec3(0,eps,0), t, R, tr, da, db, dc);
        float dyb = (hitRing==0) ? da : (hitRing==1) ? db : dc;
        ringSDFs(p + vec3(0,0,eps), t, R, tr, da, db, dc);
        float dza = (hitRing==0) ? da : (hitRing==1) ? db : dc;
        ringSDFs(p - vec3(0,0,eps), t, R, tr, da, db, dc);
        float dzb = (hitRing==0) ? da : (hitRing==1) ? db : dc;
        nd = normalize(vec3(dxa - dxb, dya - dyb, dza - dzb));

        // Fresnel: bright at grazing, dark face-on
        float fres = 1.0 - abs(dot(nd, -rd));
        fres = 0.3 + 0.7 * fres * fres;

        vec3 neonCol = (hitRing == 0) ? CYAN : (hitRing == 1) ? MAGENTA : GOLD;
        col = neonCol * fres * audioBoost;
    }

    col += glowAccum * audioBoost;

    gl_FragColor = vec4(col, 1.0);
}
