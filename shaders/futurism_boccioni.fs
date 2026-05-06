/*{
  "CATEGORIES": ["3D", "Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Futurism after Boccioni — Dynamism of a Cyclist (1913) and Unique Forms of Continuity in Space (1913). A stylized humanoid SDF stutter-cloned along a velocity vector produces true forward-sweep motion phases (the Futurist 'lines of force'). Force rays burst from a wandering origin; divisionist colour dabs streak the wake; warm sienna/ochre palette; lateral camera pan reinforces forward motion. Single-pass 3D — no frame feedback. Returns LINEAR HDR; host applies ACES.",
  "INPUTS": [
    { "NAME": "camDist",       "LABEL": "Camera Distance", "TYPE": "float", "MIN": 1.5, "MAX": 12.0, "DEFAULT": 4.5 },
    { "NAME": "camHeight",     "LABEL": "Camera Height",   "TYPE": "float", "MIN": -3.0, "MAX": 4.0, "DEFAULT": 1.2 },
    { "NAME": "camOrbitSpeed", "LABEL": "Orbit Speed",     "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.18 },
    { "NAME": "camAzimuth",    "LABEL": "Camera Azimuth",  "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 0.0 },
    { "NAME": "keyAngle",      "LABEL": "Key Light Angle", "TYPE": "float", "MIN": 0.0, "MAX": 6.2832, "DEFAULT": 0.785 },
    { "NAME": "keyElevation",  "LABEL": "Key Elevation",   "TYPE": "float", "MIN": 0.0, "MAX": 1.5708, "DEFAULT": 0.7 },
    { "NAME": "keyColor",      "LABEL": "Key Light",       "TYPE": "color", "DEFAULT": [1.0, 0.94, 0.82, 1.0] },
    { "NAME": "fillColor",     "LABEL": "Fill Light",      "TYPE": "color", "DEFAULT": [0.55, 0.70, 1.0, 1.0] },
    { "NAME": "ambient",       "LABEL": "Ambient",         "TYPE": "float", "MIN": 0.0, "MAX": 0.5,  "DEFAULT": 0.08 },
    { "NAME": "rimStrength",   "LABEL": "Rim Strength",    "TYPE": "float", "MIN": 0.0, "MAX": 1.5,  "DEFAULT": 0.5 },
    { "NAME": "exposure",      "LABEL": "Exposure",        "TYPE": "float", "MIN": 0.3, "MAX": 3.0,  "DEFAULT": 1.0 },
    { "NAME": "phantomCount",     "LABEL": "Phantom Copies",   "TYPE": "long",  "DEFAULT": 7, "VALUES": [3,5,7,9,11,13], "LABELS": ["3","5","7","9","11","13"] },
    { "NAME": "phantomSpread",    "LABEL": "Phantom Spread",   "TYPE": "float", "MIN": 0.05, "MAX": 0.6,  "DEFAULT": 0.22 },
    { "NAME": "velocityMag",      "LABEL": "Sweep Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.5,  "DEFAULT": 0.55 },
    { "NAME": "fragment",         "LABEL": "Fragmentation",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "forceRays",        "LABEL": "Force Rays",       "TYPE": "long",  "DEFAULT": 16, "VALUES": [0,8,12,16,24,32], "LABELS": ["Off","8","12","16","24","32"] },
    { "NAME": "rayBrightness",    "LABEL": "Ray Brightness",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.85 },
    { "NAME": "forceRayBias",     "LABEL": "Ray Velocity Bias","TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "divisionistDots",  "LABEL": "Divisionist Dabs", "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "warmth",           "LABEL": "Warmth",           "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.65 },
    { "NAME": "paletteWarmth",    "LABEL": "Palette Warmth",   "TYPE": "float", "MIN": -1.0, "MAX": 1.0,  "DEFAULT": 0.0 },
    { "NAME": "paperGrain",       "LABEL": "Paper Grain",      "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.18 },
    { "NAME": "speedHueShift",    "LABEL": "Speed Hue Shift",  "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "audioReact",       "LABEL": "Audio React",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0 }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  Futurism — Boccioni
//  The cyclist is built as an SDF (head + torso + two arms + two legs in
//  a forward hunch) and replicated N times along a velocity vector. At
//  each raymarch sample we take the MIN over all clones; each clone
//  fades in shading weight by index.
// ════════════════════════════════════════════════════════════════════════

#define MAX_STEPS  72
#define MAX_DIST   24.0
#define EPS        0.0012

// ─── prim helpers ─────────────────────────────────────────────────────
float sdSphere (vec3 p, float r)  { return length(p) - r; }
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3  pa = p - a, ba = b - a;
    float h  = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}
float opSmoothUnion(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}
float opSmoothSub(float a, float b, float k) {
    float h = clamp(0.5 - 0.5 * (b + a) / k, 0.0, 1.0);
    return mix(a, -b, h) + k * h * (1.0 - h);
}

// ─── hash / noise ─────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec3  hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// ─── the cyclist ──────────────────────────────────────────────────────
float sdCyclist(vec3 p, float t, float frag) {
    vec3  hPos = vec3(0.42, 0.78, 0.0);
    float head = sdSphere(p - hPos, 0.18);
    float torso = sdCapsule(p, vec3(0.0, 0.0, 0.0), vec3(0.34, 0.62, 0.0), 0.16);
    float armL = sdCapsule(p, vec3(0.30, 0.55, -0.13), vec3(0.78, 0.40, -0.18), 0.06);
    float armR = sdCapsule(p, vec3(0.30, 0.55,  0.13), vec3(0.78, 0.40,  0.18), 0.06);
    float ph    = t * 6.0;
    float pyL   = sin(ph)         * 0.18;
    float pxL   = cos(ph)         * 0.18 + 0.05;
    float pyR   = sin(ph + 3.1416) * 0.18;
    float pxR   = cos(ph + 3.1416) * 0.18 + 0.05;
    float legL  = sdCapsule(p, vec3(-0.04, -0.05, -0.10), vec3(pxL, -0.55 + pyL, -0.10), 0.075);
    float legR  = sdCapsule(p, vec3(-0.04, -0.05,  0.10), vec3(pxR, -0.55 + pyR,  0.10), 0.075);
    float body = head;
    body = opSmoothUnion(body, torso, 0.10);
    body = opSmoothUnion(body, armL,  0.07);
    body = opSmoothUnion(body, armR,  0.07);
    body = opSmoothUnion(body, legL,  0.08);
    body = opSmoothUnion(body, legR,  0.08);
    if (frag > 0.001) {
        float slab1 = abs(dot(p, normalize(vec3( 0.7,  0.6,  0.4))) - 0.05) - 0.02 * frag;
        float slab2 = abs(dot(p, normalize(vec3(-0.5,  0.7, -0.5))) - 0.18) - 0.02 * frag;
        float slab3 = abs(dot(p, normalize(vec3( 0.2, -0.6,  0.7))) + 0.10) - 0.02 * frag;
        body = opSmoothSub(body, slab1, 0.06 * frag);
        body = opSmoothSub(body, slab2, 0.06 * frag);
        body = opSmoothSub(body, slab3, 0.06 * frag);
    }
    return body;
}

float sdScene(vec3 p, float t, float frag, int copies, float spread,
              float velMag, float audio, out float bestK) {
    bestK = 0.0;
    float d = 1e9;
    for (int k = 0; k < 13; k++) {
        if (k >= copies) break;
        float kf  = float(k);
        float ofs = -kf * spread * (1.0 + 0.4 * audio);
        vec3 q = p - vec3(velMag * sin(t * 0.6 + ofs * 1.2) + ofs, 0.0, 0.0);
        float a = -ofs * 0.08;
        float c = cos(a), s = sin(a);
        q.xz = mat2(c, -s, s, c) * q.xz;
        float dk = sdCyclist(q, t + ofs * 0.5, frag);
        if (dk < d) { d = dk; bestK = kf; }
    }
    return d;
}

// ─── normal via tetrahedron ───────────────────────────────────────────
vec3 sceneNormal(vec3 p, float t, float frag, int copies, float spread,
                 float velMag, float audio) {
    const vec2 e = vec2(0.0012, -0.0012);
    float dummy;
    return normalize(
        e.xyy * sdScene(p + e.xyy, t, frag, copies, spread, velMag, audio, dummy) +
        e.yyx * sdScene(p + e.yyx, t, frag, copies, spread, velMag, audio, dummy) +
        e.yxy * sdScene(p + e.yxy, t, frag, copies, spread, velMag, audio, dummy) +
        e.xxx * sdScene(p + e.xxx, t, frag, copies, spread, velMag, audio, dummy));
}

// ─── force rays ───────────────────────────────────────────────────────
float forceField(vec2 uv, float t, int rays, float audio, float bias) {
    if (rays <= 0) return 0.0;
    vec2  org = vec2(0.05 + 0.25 * sin(t * 0.41),
                     0.10 + 0.18 * cos(t * 0.33));
    vec2  d   = uv - org;
    float ang = atan(d.y, d.x);
    float r   = length(d);
    float n   = float(rays);
    float spokes = pow(0.5 + 0.5 * cos(ang * n + t * 0.6), 28.0);
    // bias toward velocity direction (+x): brighter when ang near 0
    float aim = 0.5 + 0.5 * cos(ang);
    spokes *= mix(1.0, aim * 1.8, bias);
    spokes *= exp(-r * (1.4 - 0.5 * audio));
    return spokes;
}

// ─── divisionist colour dabs ──────────────────────────────────────────
float divDabs(vec2 uv, float t, float strength, float velMag) {
    if (strength < 0.001) return 0.0;
    float acc = 0.0;
    for (int i = 0; i < 14; i++) {
        float fi   = float(i);
        float seed = hash11(fi * 7.13);
        float xph  = fract(seed + t * (0.10 + velMag * 0.4) + fi * 0.07);
        vec2  c    = vec2(xph, 0.30 + 0.5 * hash11(fi * 1.7) + 0.05 * sin(t + fi));
        float r    = mix(0.006, 0.018, hash11(fi * 5.7));
        float d    = length((uv - c) * vec2(1.0, 1.4));
        acc += smoothstep(r, 0.0, d) * (0.5 + 0.5 * hash11(fi * 9.3));
    }
    return acc * strength;
}

// ─── main ─────────────────────────────────────────────────────────────
void main() {
    vec2 uv  = isf_FragNormCoord.xy;
    vec2 ndc = (uv * 2.0 - 1.0) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

    float t      = TIME;
    float audio  = clamp(audioReact, 0.0, 2.0);
    float frag   = clamp(fragment, 0.0, 1.0) * (1.0 + 0.25 * audio);
    int   copies = int(phantomCount + 0.5);
    int   rays   = int(forceRays + 0.5);

    // ── Universal 3D camera ─────────────────────────────────────────
    float az  = camAzimuth + camOrbitSpeed * t;
    vec3  ro  = vec3(sin(az) * camDist, camHeight, cos(az) * camDist);
    // lateral-pan flavor: small per-frame x bias driven by orbit speed
    float panX = -0.4 * sin(t * max(camOrbitSpeed, 0.0001) * 1.0);
    ro.x += panX;
    vec3  ta  = vec3(panX + velocityMag * 0.6, camHeight * 0.15, 0.0);
    vec3  fw  = normalize(ta - ro);
    vec3  ri  = normalize(cross(vec3(0.0, 1.0, 0.0), fw));
    vec3  up  = cross(fw, ri);
    vec3  rd  = normalize(fw + ndc.x * ri + ndc.y * up);

    // Boccioni palette + global palette warmth shift
    vec3 sienna = vec3(0.85, 0.45, 0.20);
    vec3 umber  = vec3(0.18, 0.12, 0.10);
    vec3 ochre  = vec3(0.95, 0.78, 0.42);
    vec3 ground = vec3(0.35, 0.22, 0.14);
    vec3 warmTint = vec3(1.0 + 0.18 * paletteWarmth,
                         1.0,
                         1.0 - 0.25 * paletteWarmth);

    float skyT  = clamp(rd.y, 0.0, 1.0);
    vec3  bgSky = mix(mix(ground, ochre, 0.5), umber, skyT);
    bgSky       = mix(bgSky, sienna * 0.7, (1.0 - skyT) * 0.5);
    vec3  col   = bgSky * warmTint;

    // Raymarch
    float d = 0.0;
    float hitK = -1.0;
    bool  hit  = false;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3  p = ro + rd * d;
        float tmpK;
        float ds = sdScene(p, t, frag, copies, phantomSpread, velocityMag, audio, tmpK);
        if (ds < EPS) { hit = true; hitK = tmpK; break; }
        if (d > MAX_DIST) break;
        d += ds * 0.92;
    }

    if (hit) {
        vec3 p = ro + rd * d;
        vec3 n = sceneNormal(p, t, frag, copies, phantomSpread, velocityMag, audio);

        float phase = hitK / max(1.0, float(copies - 1));
        float wgt   = exp(-phase * 1.6);
        float hue   = mix(0.06, 0.62, phase * speedHueShift)
                    + 0.04 * sin(t * 0.4 + phase * 6.0);
        vec3  base  = mix(ochre, hsv2rgb(vec3(hue, 0.85, 1.0)), 0.55);
        base        = mix(base, sienna, 1.0 - warmth);
        base       *= warmTint;

        // ── Universal lighting ─────────────────────────────────────
        float ce = cos(keyElevation), se = sin(keyElevation);
        vec3  keyDir = normalize(vec3(cos(keyAngle) * ce, se, sin(keyAngle) * ce));
        float kd   = max(dot(n, keyDir), 0.0);
        float fres = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);
        vec3  lit  = base * (ambient + 1.20 * kd) * keyColor.rgb * wgt
                   + fillColor.rgb * fres * rimStrength * wgt;
        lit       *= exposure;
        col        = mix(col, lit, clamp(wgt + 0.25, 0.0, 1.0));
    }

    // Force rays — composite over the whole frame (additive)
    float ff = forceField(uv, t, rays, audio, forceRayBias);
    col += vec3(1.00, 0.86, 0.55) * warmTint * ff * rayBrightness * exposure;

    // Divisionist colour dabs
    float dabs = divDabs(uv, t, divisionistDots, velocityMag);
    col += vec3(0.95, 0.45, 0.32) * warmTint * dabs * 0.55;
    col += vec3(0.45, 0.65, 0.95) * dabs * 0.25;

    // Vignette + paper grain
    col *= 1.0 - 0.22 * dot(ndc * 0.5, ndc * 0.5);
    float grain = (hash21(uv * RENDERSIZE.xy) - 0.5);
    col += grain * (0.012 + 0.06 * paperGrain);
    // paper-tooth: low-frequency multiplicative striations
    float tooth = hash21(floor(uv * RENDERSIZE.xy * 0.15)) - 0.5;
    col *= 1.0 - paperGrain * 0.12 * abs(tooth);

    gl_FragColor = vec4(col, 1.0);
}
