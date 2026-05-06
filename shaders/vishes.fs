/*{
  "DESCRIPTION": "Bioluminescent Tendrils — 6 organic capsule-chain tendrils radiating from a central node, glowing cyan/green/gold in a deep ocean void. Volumetric glow aura, animated wave tips, audio-reactive pulse.",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Speed",       "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5  },
    { "NAME": "zoom",       "LABEL": "Zoom",        "TYPE": "float", "MIN": 0.3,  "MAX": 3.0,  "DEFAULT": 1.0  },
    { "NAME": "tendrilLen", "LABEL": "Length",      "TYPE": "float", "MIN": 0.3,  "MAX": 2.0,  "DEFAULT": 1.0  },
    { "NAME": "tendrilR",   "LABEL": "Thickness",   "TYPE": "float", "MIN": 0.02, "MAX": 0.15, "DEFAULT": 0.065},
    { "NAME": "waveAmt",    "LABEL": "Wave",        "TYPE": "float", "MIN": 0.0,  "MAX": 0.5,  "DEFAULT": 0.18 },
    { "NAME": "hdrGlow",    "LABEL": "HDR Glow",    "TYPE": "float", "MIN": 0.5,  "MAX": 4.0,  "DEFAULT": 2.0  },
    { "NAME": "palette",    "LABEL": "Palette",     "TYPE": "long",  "VALUES": [0,1,2], "LABELS": ["Cyan/Gold","Violet/Cyan","Green/White"], "DEFAULT": 0 },
    { "NAME": "audioMod",   "LABEL": "Audio Mod",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0  }
  ]
}*/

const float PI  = 3.14159265;
const float TAU = 6.28318530;

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

// One tendril: 3-segment capsule chain from origin toward base_dir.
// perp_dir: secondary oscillation axis.
// phase: time-phase offset for this tendril's wave animation.
float tendril(vec3 p, vec3 base_dir, vec3 perp_dir, float phase, float r) {
    float L  = tendrilLen;
    float wa = waveAmt;

    vec3 a = vec3(0.0);

    float breathe = 0.92 + 0.08 * sin(TIME * speed * 0.9 + phase);

    // Segment 1: root → 40% of length, mild upward curve
    vec3 b1 = base_dir * L * 0.40 + vec3(0.0, L * 0.08, 0.0);

    // Segment 2: 40% → 75%, animated perpendicular wobble
    float w1 = sin(TIME * speed * 1.3 + phase) * wa * L * 0.22;
    vec3 b2 = b1 + base_dir * L * 0.35 + perp_dir * w1;

    // Segment 3: tip — full wave amplitude
    float w2x = sin(TIME * speed * 1.8 + phase + 0.7)  * wa * L * 0.35;
    float w2y = sin(TIME * speed * 1.1 + phase + 1.4)  * wa * L * 0.28;
    vec3 b3 = b2 + base_dir * L * 0.25 + perp_dir * w2x + vec3(0.0, w2y, 0.0);

    float d = sdCapsule(p, a, b1, r * breathe);
    d = min(d, sdCapsule(p, b1, b2, r * 0.73 * breathe));
    d = min(d, sdCapsule(p, b2, b3, r * 0.46 * breathe));
    return d;
}

// Scene: 5 equatorial + 1 upward tendril, smooth-union'd
float scene(vec3 p) {
    float r = tendrilR;
    float k = 0.10;

    float d0 = tendril(p, normalize(vec3(1.0,  0.15, 0.0)),  vec3(0.0,1.0,0.0), 0.00, r);
    float d1 = tendril(p, normalize(vec3(cos(TAU*0.2),  0.0, sin(TAU*0.2))), vec3(0.0,1.0,0.0), 1.26, r);
    float d2 = tendril(p, normalize(vec3(cos(TAU*0.4), -0.2, sin(TAU*0.4))), vec3(0.0,1.0,0.0), 2.51, r);
    float d3 = tendril(p, normalize(vec3(cos(TAU*0.6),  0.2, sin(TAU*0.6))), vec3(0.0,1.0,0.0), 3.77, r);
    float d4 = tendril(p, normalize(vec3(cos(TAU*0.8), -0.1, sin(TAU*0.8))), vec3(0.0,1.0,0.0), 5.03, r);
    float d5 = tendril(p, vec3(0.0, 1.0, 0.05),                              vec3(1.0,0.0,0.0), 0.63, r * 0.88);

    float d = smin(d0, d1, k);
    d = smin(d, d2, k);
    d = smin(d, d3, k);
    d = smin(d, d4, k);
    d = smin(d, d5, k);

    // Central node sphere at origin
    d = smin(d, length(p) - tendrilR * 1.6, k);

    return d;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        scene(p + e.xyy) - scene(p - e.xyy),
        scene(p + e.yxy) - scene(p - e.yxy),
        scene(p + e.yyx) - scene(p - e.yyx)
    ));
}

// March + accumulate volumetric glow; returns vec2(t, glowAcc). t<0 = miss.
vec2 march(vec3 ro, vec3 rd) {
    float t    = 0.02;
    float glow = 0.0;
    for (int i = 0; i < 64; i++) {
        float d = scene(ro + rd * t);
        glow += exp(-max(d, 0.0) * 5.5) * 0.022;
        if (d < 0.0003) return vec2(t, glow);
        t += d * 0.72;
        if (t > 18.0) break;
    }
    return vec2(-1.0, clamp(glow, 0.0, 1.0));
}

float calcAO(vec3 p, vec3 n) {
    float occ = 0.0, sc = 1.0;
    for (int i = 0; i < 5; i++) {
        float h = 0.04 + 0.10 * float(i);
        float d = scene(p + h * n);
        occ += (h - d) * sc;
        sc  *= 0.78;
    }
    return clamp(1.0 - 2.8 * occ, 0.0, 1.0);
}

void main() {
    vec2  uv    = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float audio = 1.0 + (audioLevel + audioBass * 0.6) * audioMod * 0.35;
    int   pal   = int(palette);

    // Palette: core, mid-glow, tip, bg
    vec3 cCore, cMid, cTip, cBg;
    if (pal == 0) {
        cCore = vec3(0.0, 1.0, 0.75);    // cyan
        cMid  = vec3(0.1, 0.8, 0.4);     // green
        cTip  = vec3(1.0, 0.88, 0.1);    // gold
        cBg   = vec3(0.0, 0.005, 0.015);
    } else if (pal == 1) {
        cCore = vec3(0.6, 0.2, 1.0);     // violet
        cMid  = vec3(0.1, 0.8, 1.0);     // cyan
        cTip  = vec3(0.9, 0.5, 1.0);     // lavender
        cBg   = vec3(0.005, 0.0, 0.015);
    } else {
        cCore = vec3(0.2, 1.0, 0.35);    // bright green
        cMid  = vec3(0.7, 1.0, 0.8);     // pale green-white
        cTip  = vec3(1.0, 1.0, 1.0);     // white
        cBg   = vec3(0.0, 0.008, 0.002);
    }

    // Camera: slow orbit, gentle elevation oscillation
    float ang  = TIME * speed * 0.38;
    float elev = sin(TIME * speed * 0.22) * 0.55;
    float dist = (2.2 + sin(TIME * speed * 0.17) * 0.2) * zoom;
    vec3  ro   = vec3(cos(ang) * cos(elev), sin(elev), sin(ang) * cos(elev)) * dist;
    vec3  fwd  = normalize(vec3(0.0, 0.15, 0.0) - ro);
    vec3  rght = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3  up   = cross(rght, fwd);
    vec3  rd   = normalize(fwd + uv.x * rght + uv.y * up);

    vec2 res = march(ro, rd);
    float hit  = res.x;
    float glow = res.y;

    vec3 col;

    if (hit > 0.0) {
        vec3  p   = ro + rd * hit;
        vec3  n   = calcNormal(p);
        float occ = calcAO(p, n);

        // Position along tendril: use world-height + radius as proxy for depth
        float posT = clamp(length(p) / (tendrilLen * 1.2 + tendrilR), 0.0, 1.0);
        vec3 matCol = mix(cCore, mix(cMid, cTip, posT), posT);

        // fwidth-based neon iso-rings (concentric on tendril cross-section)
        float localR  = scene(p) + tendrilR;  // approx distance from tendril axis
        float ringP   = fract(localR / tendrilR * 2.0);
        float fw      = fwidth(ringP);
        float ring    = smoothstep(fw * 2.0, 0.0, abs(ringP - 0.5) - 0.14);

        vec3  L    = normalize(vec3(0.8, 1.5, 0.5));
        float diff = max(dot(n, L), 0.0) * 0.4;
        vec3  R    = reflect(-L, n);
        float spec = pow(max(dot(R, -rd), 0.0), 32.0) * 0.3;
        float fres = pow(1.0 - abs(dot(n, -rd)), 4.0);

        col  = matCol * diff * occ;
        col += matCol * 0.7 * hdrGlow * audio;           // strong emissive body
        col += cTip * spec * hdrGlow * 1.5;              // HDR specular peak
        col += matCol * fres * 0.5 * hdrGlow;            // fresnel edge glow
        col += cCore * ring * 0.35 * hdrGlow;            // iso-ring halo

        // Depth fog into void
        col = mix(cBg, col, exp(-hit * 0.08));

    } else {
        // Background void with volumetric glow aura around tendrils
        col = cBg + cCore * glow * hdrGlow * 1.2 * audio;
        // Floating bioluminescent spores
        vec2  uvN = gl_FragCoord.xy / RENDERSIZE.xy;
        float spoN = fract(sin(dot(floor(uvN * 160.0), vec2(127.1, 311.7))) * 43758.5453);
        float spo  = step(0.992, spoN) * exp(-fract(TIME * 0.4 + spoN) * 8.0) * 0.4;
        col += cTip * spo * hdrGlow * 0.5;
    }

    // Voice glitch
    if (_voiceGlitch > 0.01) {
        float g   = _voiceGlitch;
        float tt  = TIME * 17.0;
        vec2  uvN = gl_FragCoord.xy / RENDERSIZE.xy;
        float band      = floor(uvN.y * mix(8.0, 40.0, g) + tt * 3.0);
        float bandNoise = fract(sin(band * 91.7 + tt) * 43758.5453);
        float scanline  = 0.95 + 0.05 * sin(uvN.y * RENDERSIZE.y * 1.5 + tt * 40.0);
        float blockX    = floor(uvN.x * 6.0);
        float blockY    = floor(uvN.y * 4.0);
        float blockNoise = fract(sin((blockX + blockY * 7.0) * 113.1 + floor(tt * 8.0)) * 43758.5453);
        float dropout   = step(1.0 - g * 0.15, blockNoise);
        vec3  glitched  = col * scanline * (1.0 - dropout);
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = vec4(col, 1.0);
}
