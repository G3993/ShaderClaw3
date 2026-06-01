/*{
  "DESCRIPTION": "Metamorphic Fluid — raymarched liquid-metal metaballs fused with SPH-style fluid dynamics. Studio lighting, soft shadows, Fresnel rim, audio-reactive blobs, evolving color palettes, and full live controls.",
  "CREDIT": "Easel · metamorphic_fluid (fuses metamorphosis.fs + liquid_3d.fs)",
  "CATEGORIES": ["Generator", "3D", "Fluid", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Speed",        "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 0.6 },
    { "NAME": "intensity",   "LABEL": "Intensity",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "scaleParam",  "LABEL": "Scale",        "TYPE": "float", "MIN": 0.3, "MAX": 2.5,  "DEFAULT": 1.0 },
    { "NAME": "blobCount",   "LABEL": "Blob Count",   "TYPE": "float", "MIN": 2.0, "MAX": 16.0, "DEFAULT": 8.0 },
    { "NAME": "viscosity",   "LABEL": "Viscosity",    "TYPE": "float", "MIN": 0.1, "MAX": 2.5,  "DEFAULT": 0.9 },
    { "NAME": "metalness",   "LABEL": "Metalness",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.75 },
    { "NAME": "roughness",   "LABEL": "Roughness",    "TYPE": "float", "MIN": 0.01,"MAX": 1.0,  "DEFAULT": 0.18 },
    { "NAME": "reflAmt",     "LABEL": "Reflect",      "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.55 },
    { "NAME": "spin",        "LABEL": "Auto Spin",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.3 },
    { "NAME": "reactivity",  "LABEL": "Reactivity",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "colDeep",     "LABEL": "Deep Color",   "TYPE": "color", "DEFAULT": [0.22, 0.35, 1.0,  1.0] },
    { "NAME": "colGlow",     "LABEL": "Glow Color",   "TYPE": "color", "DEFAULT": [1.0,  0.55, 0.15, 1.0] },
    { "NAME": "colAccent",   "LABEL": "Accent Color", "TYPE": "color", "DEFAULT": [0.85, 0.65, 0.3,  1.0] },
    { "NAME": "bgDark",      "LABEL": "BG Dark",      "TYPE": "bool",  "DEFAULT": true },
    { "NAME": "inputTex",    "LABEL": "Texture",      "TYPE": "image" }
  ]
}*/

#define TAU  6.28318530718
#define PI   3.14159265359
#define MAX_STEPS 110
#define MAX_DIST  18.0
#define SURF_DIST 0.0018
#define BLOB_MAX  16
#define FOV_SCALE 1.6

// ── Utilities ────────────────────────────────────────────────────────────────

float hash1(float n){ return fract(sin(n) * 43758.5453123); }

float smin(float a, float b, float k){
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

float sdSphere(vec3 p, float r){ return length(p) - r; }

float sdEllipsoid(vec3 p, vec3 r){
    float k0 = length(p / r);
    float k1 = length(p / (r * r));
    return k0 * (k0 - 1.0) / k1;
}

mat2 rot2(float a){ float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// ── Camera ───────────────────────────────────────────────────────────────────

mat3 getCamera(vec2 angles){
    float cx = cos(angles.x), sx = sin(angles.x);
    float cy = cos(angles.y), sy = sin(angles.y);
    mat3 Rx = mat3(1.0, 0.0, 0.0,  0.0, cy, -sy,  0.0, sy, cy);
    mat3 Ry = mat3(cx, sx, 0.0,  -sx, cx, 0.0,   0.0, 0.0, 1.0);
    return Rx * Ry;
}

// ── Scene SDF ────────────────────────────────────────────────────────────────

// Global scene params set once in main, read in scene/normal helpers
float g_viscK;
float g_t;
int   g_n;
float g_scale;
float g_bassR;
float g_midR;
float g_highR;

float sceneMap(vec3 p){
    float d = 1e9;
    float k = 0.32 * g_viscK;
    for(int i = 0; i < BLOB_MAX; i++){
        if(i >= g_n) break;
        float fi = float(i);

        // Two-layer orbit: slow global drift + fast local wobble
        float phase = fi * TAU / float(BLOB_MAX);
        float sl = g_t * (0.45 + 0.28 * hash1(fi));
        float sm = g_t * (0.38 + 0.33 * hash1(fi + 9.0));
        float sf = g_t * (0.42 + 0.22 * hash1(fi + 3.0));

        vec3 orbit = vec3(
            sin(sl + phase)          * (1.05 + 0.30 * hash1(fi + 5.0)),
            sin(sm + phase * 1.4)    * (0.90 + 0.25 * hash1(fi + 11.0)),
            cos(sf + phase * 1.8)    * (0.85 + 0.20 * hash1(fi + 2.0))
        );

        // Bass pushes blobs outward, mid adds vertical bounce, high shimmers
        orbit += vec3(
            sin(g_t * 2.1 + phase * 1.3) * g_bassR * 0.55,
            sin(g_t * 3.0 + phase * 0.9) * g_midR  * 0.45,
            cos(g_t * 4.2 + phase * 2.1) * g_highR * 0.30
        );

        orbit *= g_scale;

        float r = g_scale * (0.52 + 0.38 * hash1(fi + 7.0));
        // Pulsing radii on bass
        r *= 1.0 + g_bassR * 0.35 * sin(g_t * 1.7 + fi * 2.3);
        // Ellipsoidal stretch driven by mid
        vec3 radii = vec3(r) * vec3(
            1.0 + g_midR  * 0.22 * sin(g_t * 0.9 + fi * 2.1),
            1.0 + g_highR * 0.18 * cos(g_t * 1.3 + fi * 1.7),
            1.0 + g_bassR * 0.15 * sin(g_t * 1.1 + fi * 3.3)
        );

        vec3 q = p - orbit;
        // Per-blob tumble rotation (XY + YZ)
        q.xy = rot2(g_t * 0.25 + fi * 1.1) * q.xy;
        q.yz = rot2(g_t * 0.18 + fi * 0.9) * q.yz;

        d = smin(d, sdEllipsoid(q, radii), k);
    }
    return d;
}

vec3 calcNormal(vec3 p){
    vec2 e = vec2(0.003, -0.003);
    return normalize(
        e.xyy * sceneMap(p + e.xyy) +
        e.yyx * sceneMap(p + e.yyx) +
        e.yxy * sceneMap(p + e.yxy) +
        e.xxx * sceneMap(p + e.xxx)
    );
}

// ── Lighting helpers ─────────────────────────────────────────────────────────

float softShadow(vec3 ro, vec3 rd, float mint, float maxt, float k){
    float res = 1.0, ph = 1e10, t = mint;
    for(int i = 0; i < 14; i++){
        float h = sceneMap(ro + rd * t);
        if(h < 0.001) return 0.0;
        float y = h * h / (2.0 * ph);
        float dd = sqrt(max(h * h - y * y, 0.0));
        res = min(res, k * dd / max(0.0, t - y));
        ph = h; t += h;
        if(t > maxt) break;
    }
    return clamp(res, 0.0, 1.0);
}

float calcAO(vec3 p, vec3 n){
    float occ = 0.0, sca = 1.0;
    for(int i = 0; i < 4; i++){
        float h = 0.01 + 0.14 * float(i) / 3.0;
        occ += (h - sceneMap(p + h * n)) * sca;
        sca *= 0.92;
    }
    return clamp(1.0 - 2.5 * occ, 0.0, 1.0);
}

float fresnel(float cosT, float f0){
    return f0 + (1.0 - f0) * pow(clamp(1.0 - cosT, 0.0, 1.0), 5.0);
}

// ── Environment map ──────────────────────────────────────────────────────────

vec3 envMap(vec3 rd, float bassR, float highR, vec3 deepC, vec3 glowC){
    float up = rd.y * 0.5 + 0.5;
    // Dynamic sky blended from user palette
    vec3 sky = mix(deepC * 0.18, deepC * 0.5 + vec3(0.03, 0.04, 0.12), up);
    // Key light hotspot
    vec3 Ldir = normalize(vec3(0.6, 0.9, 0.4));
    float ks = pow(max(dot(rd, Ldir), 0.0), 24.0 + highR * 18.0);
    sky += glowC * ks * (1.2 + bassR * 0.8);
    // Rim fill
    float rim = pow(max(dot(rd, normalize(vec3(-0.4, 0.2, -1.0))), 0.0), 10.0);
    sky += deepC * rim * 0.35;
    // Subtle nebula-like bands driven by angle
    float band = sin(rd.y * 6.0 + rd.x * 4.0 + g_t * 0.15) * 0.5 + 0.5;
    sky += mix(deepC, glowC, band) * 0.04;
    return sky;
}

// ── Cosine palette ───────────────────────────────────────────────────────────

vec3 cosinePalette(float t2, vec3 a, vec3 b, vec3 c2, vec3 d2){
    return a + b * cos(TAU * (c2 * t2 + d2));
}

// ── Main ─────────────────────────────────────────────────────────────────────

void main(){
    vec2 R  = RENDERSIZE;
    vec2 uv = (gl_FragCoord.xy - 0.5 * R) / max(R.x, R.y);
    vec2 screenUV = gl_FragCoord.xy / R;

    // Audio with reactivity scalar
    float bassR_raw  = clamp(audioBass  * reactivity, 0.0, 1.0);
    float midR_raw   = clamp(audioMid   * reactivity, 0.0, 1.0);
    float highR_raw  = clamp(audioHigh  * reactivity, 0.0, 1.0);
    float levelR     = clamp(audioLevel * reactivity, 0.0, 1.0);

    // Set scene globals
    g_t      = TIME * speed;
    g_n      = int(clamp(blobCount, 2.0, float(BLOB_MAX)));
    g_viscK  = viscosity;
    g_scale  = scaleParam * (1.0 + bassR_raw * 0.25 * intensity);
    g_bassR  = bassR_raw  * intensity;
    g_midR   = midR_raw   * intensity;
    g_highR  = highR_raw  * intensity;

    vec3 deepC  = colDeep.rgb;
    vec3 glowC  = colGlow.rgb;
    vec3 accentC = colAccent.rgb;

    // ── Camera ──
    // Auto-spin + slight vertical sway
    float camYaw   = TIME * spin * 0.5 + levelR * 0.2;
    float camPitch = -0.42 + sin(TIME * 0.11) * 0.12 + midR_raw * 0.1;
    vec2  angles   = vec2(camYaw, camPitch);

    mat3 cam = getCamera(angles);
    vec3 rd  = normalize(transpose(cam) * vec3(FOV_SCALE * uv.x, 1.0, FOV_SCALE * uv.y));
    vec3 cforward = normalize(transpose(cam) * vec3(0.0, 1.0, 0.0));
    float camDist = 4.2 + sin(TIME * 0.13) * 0.4 - g_bassR * 0.3;
    vec3 ro = -cforward * camDist;

    // ── Env / bg ──
    vec3 bg = envMap(rd, g_bassR, g_highR, deepC, glowC);
    if(!bgDark){
        // Vivid bg: hue sweep
        float phi = atan(uv.y, uv.x) / TAU;
        vec3 vivid = cosinePalette(phi + TIME * 0.07,
            vec3(0.5), vec3(0.5),
            vec3(1.0, 0.9, 0.8),
            deepC * 0.5 + vec3(0.0, 0.33, 0.67));
        bg = mix(bg, vivid * 0.7, 0.55);
    }

    // ── Bounding-sphere early-out ──
    float bsB    = dot(ro, rd);
    float bsR    = g_scale * 3.0 + 1.0;
    float bsC    = dot(ro, ro) - bsR * bsR;
    float bsDisc = bsB * bsB - bsC;

    float totalDist = 0.0;
    float minDist   = MAX_DIST;
    vec3  p         = ro;
    bool  hit       = false;

    if(bsDisc > 0.0){
        float sqrtD   = sqrt(bsDisc);
        float tEntry  = -bsB - sqrtD;
        float tExit   = -bsB + sqrtD;
        if(tExit > 0.0){
            totalDist = max(tEntry, 0.0);
            float marchLim = min(tExit, MAX_DIST);
            for(int i = 0; i < MAX_STEPS; i++){
                p = ro + rd * totalDist;
                float dd = sceneMap(p);
                minDist = min(minDist, dd);
                if(dd < SURF_DIST){ hit = true; break; }
                if(totalDist > marchLim) break;
                totalDist += clamp(dd, 0.0005, 0.8);
            }
        }
    }

    vec3 col = bg;

    if(hit){
        vec3 n   = calcNormal(p);
        vec3 v   = normalize(ro - p);
        float NdotV = max(dot(n, v), 0.0);

        // ── Texture path ──
        vec4 texSamp = texture2D(inputTex, screenUV);
        if(texSamp.a > 0.01){
            vec2 refUV = screenUV + n.xy * 0.07 * (1.0 + g_highR * 0.5);
            vec3 texC  = texture2D(inputTex, refUV).rgb;
            vec3 L1    = normalize(vec3(0.6, 0.9, 0.4));
            float diff  = max(dot(n, L1), 0.0);
            float spec  = pow(max(dot(n, normalize(L1 + v)), 0.0), mix(192.0, 12.0, roughness));
            float fres  = fresnel(NdotV, 0.04);
            float sha   = softShadow(p + n * 0.01, L1, 0.02, 5.0, 16.0);
            float ao    = calcAO(p, n);
            col  = texC * diff * sha * 0.8;
            col += texC * spec * fres * 1.8;
            col += texC * pow(1.0 - NdotV, 4.0) * 0.3;
            col *= ao;
        } else {
            // ── Full metallic-fluid shading ──

            // Procedural surface color — driven by position + audio
            float cm1 = sin(p.x * 2.8 + p.z * 1.9 + g_t * 0.35 + g_bassR * 2.0) * 0.5 + 0.5;
            float cm2 = sin(p.y * 3.5 - p.x * 2.2 + g_t * 0.28 + g_midR  * 1.5) * 0.5 + 0.5;
            float cm3 = sin(length(p) * 4.0 - g_t * 0.5 + g_highR * 3.0) * 0.5 + 0.5;

            // Blend deep → glow → accent using cosine palette
            vec3 palA = cosinePalette(cm1,
                (deepC + glowC) * 0.5,
                (glowC - deepC) * 0.5 + vec3(0.1),
                vec3(1.0, 0.95, 0.9),
                vec3(0.0, 0.15, 0.33));
            vec3 albedo = mix(palA, accentC, cm2 * 0.45);
            albedo = mix(albedo, deepC * 0.6 + glowC * 0.4, cm3 * 0.3);
            // Normalise energy
            albedo = normalize(albedo + 0.001) * mix(0.55, 1.0, cm1);

            float metallic = metalness;
            vec3  specColor = mix(vec3(0.04), albedo, metallic);

            // Three-point lighting
            vec3 L1  = normalize(vec3( 0.6,  0.9,  0.4));
            vec3 L1c = mix(glowC, vec3(1.0, 0.95, 0.85), 0.6) * (1.4 + g_bassR * 0.8);
            vec3 L2  = normalize(vec3(-1.5,  0.5, -0.8));
            vec3 L2c = mix(deepC, vec3(0.5, 0.35, 0.8), 0.5) * 0.55;
            vec3 L3  = normalize(vec3( 0.1, -0.4, -1.5));
            vec3 L3c = accentC * 0.65;

            float d1 = max(dot(n, L1), 0.0);
            float d2 = max(dot(n, L2), 0.0);
            float d3 = max(dot(n, L3), 0.0);
            // Half-Lambert on fill lights for softer fluid look
            float d2h = d2 * 0.5 + 0.5 * d2;
            float d3h = d3 * 0.5 + 0.5 * d3;

            vec3 h1  = normalize(L1 + v);
            vec3 h2  = normalize(L2 + v);
            vec3 h3  = normalize(L3 + v);
            float gloss = mix(512.0, 8.0, roughness);
            float s1 = pow(max(dot(n, h1), 0.0), gloss);
            float s2 = pow(max(dot(n, h2), 0.0), gloss * 0.5);
            float s3 = pow(max(dot(n, h3), 0.0), gloss * 0.3);

            float fres = fresnel(NdotV, 0.04 + metallic * 0.76);
            float sha  = softShadow(p + n * 0.012, L1, 0.02, 6.0, 18.0);
            float ao   = calcAO(p, n);

            vec3 diffuse = albedo * (1.0 - metallic) *
                (L1c * d1 * sha + L2c * d2h + L3c * d3h);

            vec3 specHL = specColor *
                (L1c * s1 * sha * 1.6 + L2c * s2 * 0.9 + L3c * s3 * 0.7);

            // Environment reflection
            vec3 reflDir = reflect(-v, n);
            vec3 envC    = envMap(reflDir, g_bassR, g_highR, deepC, glowC);
            vec3 envRefl = envC * mix(vec3(0.04), albedo, metallic) * fres;
            envRefl     *= reflAmt;

            // Fresnel rim glow — color shifts with high frequency audio
            float rimF  = pow(1.0 - NdotV, 3.5 + g_highR * 1.5);
            vec3  rimC  = mix(glowC, accentC, g_highR) * rimF *
                          (0.7 + g_bassR * 0.8) * intensity;

            // Subsurface scatter approximation (non-metal path)
            float sss = pow(max(dot(-v, L1), 0.0), 4.0) * (1.0 - metallic) * 0.18;
            vec3  sssC = mix(deepC, glowC, 0.5) * sss * (1.0 + g_bassR * 0.5);

            // Sharp specular needle
            col  = diffuse + specHL + envRefl + rimC + sssC;
            col *= ao;
            col += mix(vec3(1.0), glowC + vec3(0.3), 0.4) *
                   pow(max(dot(n, h1), 0.0), 1024.0) * sha * 2.5;
        }

        // Depth fog
        float fog = 1.0 - exp(-totalDist * 0.06);
        col = mix(col, bg * 0.5, fog * 0.4);

    } else {
        // Near-miss glow halos (two-band: warm gold + cool deep)
        float glow1 = exp(-minDist * 48.0);
        float glow2 = exp(-minDist * 7.0);
        col += glowC  * glow1 * 2.2 * intensity;
        col += deepC  * glow2 * 0.35 * intensity;

        // Bass pulse on bg
        col += deepC * g_bassR * 0.12 * exp(-dot(uv * 1.5, uv * 1.5));
    }

    // ── Post-processing ──────────────────────────────────────────────────────

    // ACES filmic tone map
    col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);

    // Subtle gamma correction toward warm
    col = pow(max(col, 0.0), vec3(0.93, 0.97, 1.05));

    // Vignette (stronger on bass hits)
    float vigStr = 0.28 + g_bassR * 0.18;
    float vig = 1.0 - dot(uv, uv) * vigStr;
    col *= clamp(vig, 0.0, 1.0);

    // Chromatic aberration on high-frequency peaks
    float abr = g_highR * 0.006 * intensity;
    if(abr > 0.0001){
        vec2 uvR = screenUV + vec2( abr,  abr * 0.5);
        vec2 uvB = screenUV + vec2(-abr, -abr * 0.5);
        // Sample env color at offset directions for subtle fringing
        float rDrift = sin(uv.x * 12.0 + TIME) * abr;
        float bDrift = cos(uv.y * 10.0 + TIME) * abr;
        col.r = mix(col.r, col.r + rDrift * 0.3, 0.5);
        col.b = mix(col.b, col.b + bDrift * 0.3, 0.5);
    }

    // Rhythmic brightness pulse on beat (bass)
    col *= 1.0 + g_bassR * 0.18 * intensity * sin(TIME * 12.0) * 0.4;

    // Chrysalis snap every ~48s — posterize briefly
    {
        float ph = fract(TIME / 48.0);
        float fl = smoothstep(0.0, 0.05, ph) * smoothstep(0.22, 0.10, ph);
        col = mix(col, floor(col * 5.0 + 0.5) / 5.0, fl * 0.65);
    }

    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}