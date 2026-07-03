/*{
  "DESCRIPTION": "GeoParticles — Circle-circle intersection path-tracer fused with 3D billboard particle cloud. Organic drift, graph-paper grid, perspective particles, cinematic post. Strongly audio-reactive. Ultra-slow, hypnotic motion.",
  "CREDIT": "Original geo: Yusef28 (Shadertoy 7l2XDm). Original particles: ShaderClaw. Fusion: ShaderClaw.",
  "CATEGORIES": ["Generator", "Audio Reactive", "3D", "Particles"],
  "INPUTS": [
    { "NAME": "speed",           "LABEL": "Speed",             "TYPE": "float", "DEFAULT": 0.08,  "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "intensity",       "LABEL": "Intensity",         "TYPE": "float", "DEFAULT": 1.0,   "MIN": 0.0,  "MAX": 3.0  },
    { "NAME": "scaleParam",      "LABEL": "Scene Scale",       "TYPE": "float", "DEFAULT": 1.0,   "MIN": 0.2,  "MAX": 3.0  },
    { "NAME": "audioReact",      "LABEL": "Audio Reactivity",  "TYPE": "float", "DEFAULT": 1.0,   "MIN": 0.0,  "MAX": 3.0  },
    { "NAME": "radius1",         "LABEL": "Circle A Radius",   "TYPE": "float", "DEFAULT": 2.0,   "MIN": 0.5,  "MAX": 5.0  },
    { "NAME": "radius2",         "LABEL": "Circle B Radius",   "TYPE": "float", "DEFAULT": 3.0,   "MIN": 0.5,  "MAX": 6.0  },
    { "NAME": "samples",         "LABEL": "Path Samples",      "TYPE": "long",  "DEFAULT": 48,    "VALUES": [16,32,48,68,96], "LABELS": ["16","32","48","68","96"] },
    { "NAME": "exposure",        "LABEL": "Exposure",          "TYPE": "float", "DEFAULT": 2.2,   "MIN": 0.5,  "MAX": 5.0  },
    { "NAME": "gamma",           "LABEL": "Gamma",             "TYPE": "float", "DEFAULT": 0.75,  "MIN": 0.4,  "MAX": 1.4  },
    { "NAME": "organicAmp",      "LABEL": "Organic Drift",     "TYPE": "float", "DEFAULT": 0.7,   "MIN": 0.0,  "MAX": 2.0  },
    { "NAME": "vignetteAmount",  "LABEL": "Vignette",          "TYPE": "float", "DEFAULT": 0.55,  "MIN": 0.0,  "MAX": 1.5  },
    { "NAME": "grainAmount",     "LABEL": "Film Grain",        "TYPE": "float", "DEFAULT": 0.04,  "MIN": 0.0,  "MAX": 0.2  },
    { "NAME": "particleCount",   "LABEL": "Particles",         "TYPE": "float", "DEFAULT": 200.0, "MIN": 0.0,  "MAX": 400.0 },
    { "NAME": "particleSize",    "LABEL": "Particle Size",     "TYPE": "float", "DEFAULT": 0.035, "MIN": 0.002,"MAX": 0.12  },
    { "NAME": "spread",          "LABEL": "Particle Spread",   "TYPE": "float", "DEFAULT": 1.2,   "MIN": 0.2,  "MAX": 3.0  },
    { "NAME": "rotateSpeed",     "LABEL": "Camera Spin",       "TYPE": "float", "DEFAULT": 0.04,  "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "fovParam",        "LABEL": "FOV",               "TYPE": "float", "DEFAULT": 2.0,   "MIN": 0.5,  "MAX": 5.0  },
    { "NAME": "pulseAmount",     "LABEL": "Particle Pulse",    "TYPE": "float", "DEFAULT": 0.6,   "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "particleMix",     "LABEL": "Particle Layer Mix","TYPE": "float", "DEFAULT": 0.55,  "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "hueShift",        "LABEL": "Hue Shift",         "TYPE": "float", "DEFAULT": 0.0,   "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "driftSpeed",      "LABEL": "Drift Speed",       "TYPE": "float", "DEFAULT": 0.04,  "MIN": 0.0,  "MAX": 0.5  },
    { "NAME": "colorA",          "LABEL": "Circle A Tint",     "TYPE": "color", "DEFAULT": [0.8, 0.3, 0.7, 1.0] },
    { "NAME": "colorB",          "LABEL": "Circle B Tint",     "TYPE": "color", "DEFAULT": [0.2, 0.5, 0.9, 1.0] },
    { "NAME": "colorIntersect",  "LABEL": "Intersection",      "TYPE": "color", "DEFAULT": [0.6, 0.9, 1.0, 1.0] },
    { "NAME": "bgColor",         "LABEL": "Background",        "TYPE": "color", "DEFAULT": [0.03, 0.02, 0.06, 1.0] }
  ]
}*/

// ─────────────────────────────────────────────
//  UTILITY
// ─────────────────────────────────────────────

float rnd(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453123);
}

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5453); }

vec3 hash3(float n) {
    return fract(sin(vec3(n, n + 1.0, n + 2.0)) * vec3(43758.5453, 22578.1459, 19642.349));
}

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = rnd(i);
    float b = rnd(i + vec2(1.0, 0.0));
    float c = rnd(i + vec2(0.0, 1.0));
    float d = rnd(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float smoothNoise(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int o = 0; o < 4; o++) {
        v += a * vnoise(p);
        p  = p * 2.13 + vec2(1.7, 9.3);
        a *= 0.5;
    }
    return v;
}

vec2 driftAnchor(int L) {
    float f = float(L);
    return vec2(rnd(vec2(f, 1.7)), rnd(vec2(f, 9.3))) - 0.5;
}

vec2 organicOffset(vec2 p, float t, float amp) {
    vec2 offset = vec2(0.0);
    float totalW = 0.0;
    for (int L = 0; L < 4; L++) {
        float depth  = float(L) / 4.0;
        float weight = 1.0 - depth * 0.5;
        vec2  anchor = driftAnchor(L);
        float lspeed = 0.04 + float(L) * 0.02;
        vec2 wobble = vec2(
            smoothNoise(p * (0.4 + depth * 0.3) + anchor + vec2(t * lspeed, float(L) * 3.7)),
            smoothNoise(p * (0.4 + depth * 0.3) + anchor + vec2(float(L) * 5.1, t * lspeed * 1.3))
        );
        wobble = wobble * 2.0 - 1.0;
        offset += wobble * weight;
        totalW += weight;
    }
    return (offset / max(totalW, 0.001)) * amp;
}

// ─────────────────────────────────────────────
//  COLOR
// ─────────────────────────────────────────────

vec3 hue2rgb(float h) {
    h = fract(h);
    float r = abs(h * 6.0 - 3.0) - 1.0;
    float g = 2.0 - abs(h * 6.0 - 2.0);
    float b = 2.0 - abs(h * 6.0 - 4.0);
    return clamp(vec3(r, g, b), 0.0, 1.0);
}

vec3 hsl2rgb(float h, float s, float l) {
    vec3 rgb = hue2rgb(h);
    float c = (1.0 - abs(2.0 * l - 1.0)) * s;
    return (rgb - 0.5) * c + l;
}

// ─────────────────────────────────────────────
//  3-D ROTATION
// ─────────────────────────────────────────────

vec3 rotateY(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(c*p.x + s*p.z, p.y, -s*p.x + c*p.z);
}
vec3 rotateX(vec3 p, float a) {
    float c = cos(a), s = sin(a);
    return vec3(p.x, c*p.y - s*p.z, s*p.y + c*p.z);
}

// ─────────────────────────────────────────────
//  GEO SCENE
// ─────────────────────────────────────────────

void addObj(float dist, vec3 color, inout float endDist, inout vec3 endColor) {
    if (dist < endDist) { endDist = dist; endColor = color; }
}

void mapScene(vec2 uv, float t, inout float d, inout vec3 color) {
    float audioB  = clamp(audioBass  * audioReact, 0.0, 3.0);
    float audioM  = clamp(audioMid   * audioReact, 0.0, 3.0);
    float audioH  = clamp(audioHigh  * audioReact, 0.0, 3.0);

    float orbitR1 = (4.0 + audioB * 0.8) * scaleParam;
    float orbitR2 = (3.0 + audioM * 0.5) * scaleParam;

    // Much slower orbit — use separate slow time
    vec2 center1 = vec2(-orbitR1 * cos(t * 0.12), -orbitR1 * 0.75 * sin(t * 0.22));
    vec2 center2 = vec2( 0.8 * sin(t * 0.09 + audioH * 0.6), 0.5 * cos(t * 0.07 + audioB * 0.4));

    float c1 = radius1 * (1.0 + audioB * 0.15);
    float c2 = radius2 * (1.0 + audioM * 0.10);

    float h1 = center1.x, k1 = center1.y;
    float h2 = center2.x, k2 = center2.y;
    float k3 = k1 - k2, h3 = h1 - h2;
    float c3 = k1*k1 - k2*k2 + h1*h1 - h2*h2 - c1*c1 + c2*c2;
    float w  = -(k3 / h3);
    float u  = -c3 / (2.0 * h3);
    float a  = w*w + 1.0;
    float b  = 2.0 * (w*h1 + w*u + k1);
    float cc = u*u + 2.0*u*h1 + k1*k1 - c1*c1 + h1*h1;
    float disc = b*b - 4.0*a*cc;
    float y1 = (-b + sqrt(max(disc, 0.0))) / (2.0*a);
    float y2 = (-b - sqrt(max(disc, 0.0))) / (2.0*a);
    float x1 = y1*w + u, x2 = y2*w + u;
    vec2 ip1 = -vec2(x1, y1), ip2 = -vec2(x2, y2);

    d = 1e9; color = vec3(0.0);
    float f;

    f = abs(length(uv - center1) - c1);
    addObj(f, colorA.rgb * intensity * 0.25, d, color);

    if (length(uv - center1) > c1) {
        f = abs(length(uv - center2) - c2);
        addObj(f, colorB.rgb * intensity / 3.0, d, color);
    }

    if (disc >= 0.0) {
        float iDot = 0.10 + audioH * audioReact * 0.06;
        f = abs(length(uv - ip1) - iDot);
        addObj(f, colorIntersect.rgb * intensity * 2.2, d, color);
        f = abs(length(uv - ip2) - iDot);
        addObj(f, colorIntersect.rgb * intensity * 2.2, d, color);
    }

    // Slow secondary orbs
    f = abs(length(uv - vec2( 7.0 * scaleParam,  4.0 * sin(t * 0.08))) - 0.5 * scaleParam);
    addObj(f, vec3(1.0, 1.0, 0.7) * intensity, d, color);
    f = abs(length(uv + vec2( 7.0 * scaleParam, -4.0 * cos(t * 0.08))) - 0.5 * scaleParam);
    addObj(f, vec3(1.4, 0.9, 0.5) * intensity, d, color);
}

float traceRay(vec2 ro, vec2 rd, float t, inout vec3 color, vec3 grid) {
    float tt = 0.0;
    for (int i = 0; i < 30; i++) {
        float d;
        mapScene(ro + rd * tt, t, d, color);
        if (d < 0.0001 || tt > 12.0) break;
        tt += d;
    }
    if (tt > 12.0) color = grid;
    return tt;
}

// ─────────────────────────────────────────────
//  MAIN
// ─────────────────────────────────────────────
void main() {
    vec2 res  = RENDERSIZE;
    float asp = res.x / res.y;

    float audioB = clamp(audioBass  * audioReact, 0.0, 3.0);
    float audioM = clamp(audioMid   * audioReact, 0.0, 3.0);
    float audioH = clamp(audioHigh  * audioReact, 0.0, 3.0);

    // ── Ultra-slow time ──
    // 'speed' now tops out at 1.0 (mapped to a very gentle pace)
    float breathe = 1.0 + audioM * 0.05 * sin(TIME * 0.8);
    float t = TIME * speed * breathe;

    // ── Screen coords ──
    vec2 fragNorm = gl_FragCoord.xy / res;
    vec2 centred  = (gl_FragCoord.xy - res * 0.5) / res.y;
    vec2 screen   = vec2(centred.x * asp, centred.y);

    // ── Organic drift — much slower ──
    float driftScale = organicAmp * 0.018;
    vec2 drift = organicOffset(centred, TIME * driftSpeed, driftScale);
    drift += vec2(sin(TIME * 0.18 + audioM * 0.8), cos(TIME * 0.14 + audioH * 0.6))
             * audioB * 0.005;

    // ─────────────────────────────────────────
    //  GRAPH PAPER BACKGROUND
    // ─────────────────────────────────────────
    vec2 st = centred + drift * 0.4;
    vec2 uvGrid = st * 8.0;
    vec3 bgBase = bgColor.rgb;

    vec3 col = bgBase;
    col = mix(col, bgBase + vec3(0.06), 1.0 - length(uvGrid / 8.0));
    float tex = vnoise(st * 80.0 + vec2(13.0, 7.0));
    col = mix(col, bgBase + vec3(0.12), pow(tex, 2.0));

    vec2 lines = fract(uvGrid * 5.0);
    lines = smoothstep(vec2(0.45), vec2(0.52), abs(lines - 0.5));
    col = mix(col, bgBase + vec3(0.15), lines.x);
    col = mix(col, bgBase + vec3(0.15), lines.y);
    lines = fract(uvGrid);
    lines = smoothstep(vec2(0.47), vec2(0.52), abs(lines - 0.5));
    col = mix(col, bgBase + vec3(0.28), lines.x);
    col = mix(col, bgBase + vec3(0.28), lines.y);
    lines = smoothstep(vec2(0.0), vec2(0.02), abs(uvGrid));
    col = mix(col, bgBase + vec3(0.38), 1.0 - lines.x);
    col = mix(col, bgBase + vec3(0.38), 1.0 - lines.y);

    vec3 grid = col * 0.5;

    // ─────────────────────────────────────────
    //  GEO PATH TRACE
    // ─────────────────────────────────────────
    vec2 uv = centred * 10.0 * scaleParam;
    uv += drift * 10.0 * scaleParam;

    int N = int(samples + 0.5);
    if (N < 4)   N = 4;
    if (N > 128) N = 128;
    float Nf = float(N);

    vec3 marchColor = vec3(0.0);
    vec3 tmpColor;

    for (int i = 0; i < 128; i++) {
        if (i >= N) break;
        float fi    = float(i);
        float angle = (fi + rnd(centred + fi)) / Nf * 6.28318;
        vec2  rd    = vec2(cos(angle), sin(angle));
        traceRay(uv, rd, t, tmpColor, grid);
        marchColor += tmpColor;
    }
    marchColor /= Nf;
    col = marchColor * exposure * intensity;

    // ─────────────────────────────────────────
    //  3-D PARTICLE CLOUD  (very slow drift)
    // ─────────────────────────────────────────
    // Camera rotation glacially slow
    float camY = TIME * rotateSpeed * 0.07;
    float camX = sin(TIME * rotateSpeed * 0.031) * 0.3;

    float dynSpread = spread * (1.0 + audioB * 0.25);

    vec3 particleLayer = vec3(0.0);
    float particleAlpha = 0.0;

    int NP = int(particleCount);
    if (NP > 256) NP = 256;   // mobile-safe loop cap (default 200 unaffected)

    for (int i = 0; i < 256; i++) {
        if (i >= NP) break;
        float fi = float(i);

        vec3 pos = (hash3(fi * 3.7) * 2.0 - 1.0) * dynSpread;

        // Extremely slow sine breathing
        vec3 trTime = pos + TIME * speed * 0.06;
        float scaleS = sin(trTime.x * 2.1) + sin(trTime.y * 3.2) + sin(trTime.z * 4.3);

        // Gentle bass kick
        float kickPhase = fract(TIME * 0.4 + fi * 0.013);
        float kick = audioB * smoothstep(0.0, 0.05, kickPhase) * smoothstep(0.25, 0.05, kickPhase);
        pos += normalize(pos + vec3(0.001)) * kick * 0.2;

        float sizeScale = mix(1.0, scaleS * 0.5 + 1.0, pulseAmount);
        sizeScale *= (1.0 + audioH * audioReact * 0.3);

        pos = rotateY(pos, camY);
        pos = rotateX(pos, camX);

        // Very slow z drift
        pos.z += audioH * 0.08 * sin(TIME * 0.9 + fi * 0.17);

        float z = pos.z + fovParam;
        if (z < 0.1) continue;

        vec2 projected = pos.xy / z;
        float sz = particleSize * sizeScale / z;
        float dist = length(screen - projected);
        float d = smoothstep(sz, sz * 0.25, dist);

        if (d > 0.001) {
            float hue = fract((scaleS / 5.0) + hueShift + audioM * 0.08);
            float lval = 0.45 + audioH * 0.10;
            vec3 pcol  = hsl2rgb(hue, 1.0, clamp(lval, 0.2, 0.8));

            float depthFade = smoothstep(fovParam + dynSpread * 2.0, 0.5, z);
            float contrib = d * depthFade * (1.0 + audioB * 0.4);

            particleLayer = mix(particleLayer, pcol, contrib);
            particleAlpha = max(particleAlpha, contrib);
        }
    }

    float pMix = particleMix * (1.0 + audioM * audioReact * 0.2);
    pMix = clamp(pMix, 0.0, 1.0);
    col = col + particleLayer * pMix * particleAlpha;

    // ─────────────────────────────────────────
    //  SWARM CONVERGENCE PULSE  (~every 45 s)
    // ─────────────────────────────────────────
    {
        float ph  = fract(TIME / 45.0);
        float f   = smoothstep(0.0, 0.02, ph) * smoothstep(0.12, 0.06, ph);
        float r   = length(fragNorm - 0.5);
        float pulse = exp(-r * 8.0) * exp(-pow(ph * 5.0 - 0.5, 2.0));
        col += vec3(0.65, 0.85, 1.0) * pulse * f * 1.4 * intensity;
    }

    // ─────────────────────────────────────────
    //  GAMMA
    // ─────────────────────────────────────────
    col = pow(max(col, 0.0), vec3(gamma));

    // ─────────────────────────────────────────
    //  CINEMATIC VIGNETTE
    // ─────────────────────────────────────────
    vec2 vUV = fragNorm - 0.5;
    vUV.x *= asp;
    float vDist = length(vUV);
    float vig  = 1.0 - pow(clamp(vDist * 1.5, 0.0, 1.0), 2.2) * vignetteAmount;
    float vig2 = 1.0 - pow(clamp(vDist * 0.9, 0.0, 1.0), 3.5) * vignetteAmount * 0.4;
    col *= vig * vig2;

    // ─────────────────────────────────────────
    //  FILM GRAIN
    // ─────────────────────────────────────────
    float grainT = float(FRAMEINDEX) * 0.137 + TIME * 13.7;
    float grain  = rnd(gl_FragCoord.xy * 0.5 + vec2(grainT * 17.3, grainT * 31.1));
    grain = (grain - 0.5) * 2.0;
    float lum = dot(col, vec3(0.2126, 0.7152, 0.0722));
    float grainMask = 4.0 * lum * (1.0 - lum);
    col += grain * grainAmount * grainMask;

    // ─────────────────────────────────────────
    //  CHROMATIC ABERRATION
    // ─────────────────────────────────────────
    float caAmt = length(drift) * 0.2 + audioB * 0.003;
    vec2 caOff  = normalize(centred + vec2(0.001)) * caAmt;
    col.r += dot(caOff, vec2( 0.5,  0.3)) * col.r * 0.3;
    col.b -= dot(caOff, vec2( 0.3,  0.5)) * col.b * 0.3;

    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}