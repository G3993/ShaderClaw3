/*{
  "DESCRIPTION": "Fluid Body — organic fluid simulation with chromatic aberration, color rotation, and audio-reactive momentum. Inspired by Synesthesia's Fluid Body.",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator", "Simulation"],
  "INPUTS": [
    { "NAME": "scaleFactor",   "LABEL": "Scale Factor",   "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.1,  "MAX": 2.0 },
    { "NAME": "pulse",         "LABEL": "Pulse",          "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "momentum",      "LABEL": "Momentum",       "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "flow",          "LABEL": "Flow",           "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "driftX",        "LABEL": "Drift X",        "TYPE": "float", "DEFAULT": 0.0,  "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "driftY",        "LABEL": "Drift Y",        "TYPE": "float", "DEFAULT": 0.0,  "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "chromatic",     "LABEL": "Chromatic",      "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "coruscate",     "LABEL": "Coruscate",      "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "colorRotate",   "LABEL": "Color Rotate",   "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "colorSpeed",    "LABEL": "Color Speed",    "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "zap",           "LABEL": "Zap",            "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "lens",          "LABEL": "Lens Distort",   "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "bars",          "LABEL": "Bars",           "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "reactivity",    "LABEL": "Reactivity",     "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "audioSpeed",    "LABEL": "Audio Speed",    "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "lowColor",      "LABEL": "Low Color",      "TYPE": "color", "DEFAULT": [0.05, 0.0, 0.1, 1.0] },
    { "NAME": "highColor",     "LABEL": "High Color",     "TYPE": "color", "DEFAULT": [1.0, 0.6, 0.1, 1.0] },
    { "NAME": "limitColors",   "LABEL": "Limit Colors",   "TYPE": "bool",  "DEFAULT": false },
    { "NAME": "transparentBg", "LABEL": "Transparent",    "TYPE": "bool",  "DEFAULT": false }
  ],
  "PASSES": [
    { "TARGET": "fluidBuf", "PERSISTENT": true },
    {}
  ]
}*/

#define PI  3.14159265
#define PI2 6.28318530

// ---- Noise ----
float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}

float simplex2D(vec2 p) {
    const float K1 = 0.366025404; // (sqrt(3)-1)/2
    const float K2 = 0.211324865; // (3-sqrt(3))/6
    vec2 i = floor(p + (p.x + p.y) * K1);
    vec2 a = p - i + (i.x + i.y) * K2;
    float m = step(a.y, a.x);
    vec2 o = vec2(m, 1.0 - m);
    vec2 b = a - o + K2;
    vec2 c = a - 1.0 + 2.0 * K2;
    vec3 h = max(0.5 - vec3(dot(a,a), dot(b,b), dot(c,c)), 0.0);
    h = h * h * h * h;
    vec3 n = h * vec3(dot(a, hash22(i) - 0.5),
                      dot(b, hash22(i + o) - 0.5),
                      dot(c, hash22(i + 1.0) - 0.5));
    return dot(n, vec3(70.0));
}

// ---- HSV ----
vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// ---- Rotation ----
vec2 rot2(vec2 v, float a) {
    float c = cos(a), s = sin(a);
    return vec2(c*v.x - s*v.y, s*v.x + c*v.y);
}

// ---- Fluid curl measurement ----
#define ROT_NUM 5
float _ang = PI2 / float(ROT_NUM);

float getCurl(vec2 pos, vec2 b, vec2 Res) {
    vec2 p = b;
    float curl = 0.0;
    for (int i = 0; i < ROT_NUM; i++) {
        vec2 samp = texture2D(fluidBuf, fract((pos + p) / Res)).xy - 0.5;
        curl += dot(samp, p.yx * vec2(1.0, -1.0));
        p = rot2(p, _ang);
    }
    return curl / float(ROT_NUM) / dot(b, b);
}

// ---- Lens distortion ----
vec2 lensWarp(vec2 uv, float amount) {
    vec2 c = uv - 0.5;
    float r2 = dot(c, c);
    float distort = 1.0 + amount * r2 * 4.0;
    return c * distort + 0.5;
}

void main() {
    vec2 Res = RENDERSIZE;
    vec2 pos = gl_FragCoord.xy;
    vec2 uv = isf_FragNormCoord;
    float aspect = Res.x / Res.y;

    // Audio modulation
    float audioMod = audioBass * reactivity;
    float audioTimeMod = 1.0 + audioMod * audioSpeed * 2.0;

    // ===== PASS 0: Fluid Simulation =====
    if (PASSINDEX == 0) {
        float t = TIME * audioTimeMod;

        // Stochastic evaluation offset
        vec2 b = cos(float(FRAMEINDEX) * 0.3 - vec2(0.0, 1.57));

        // Compute velocity from multi-scale curl
        vec2 v = vec2(0.0);
        float bbMax = 0.5 * Res.y * scaleFactor;
        bbMax *= bbMax;

        for (int l = 0; l < 20; l++) {
            if (dot(b, b) > bbMax) break;
            vec2 p = b;
            for (int i = 0; i < ROT_NUM; i++) {
                v += p.yx * getCurl(pos + p, -rot2(b, _ang * 0.5), Res);
                p = rot2(p, _ang);
            }
            b *= 2.0;
        }

        // Self-advection with momentum
        float speedScale = (3.0 + momentum * 12.0) * sqrt(Res.x / 600.0);
        vec2 drift = vec2(driftX, driftY) * 0.5;
        vec2 advUV = fract((pos - v * vec2(-1.0, 1.0) * speedScale - drift) / Res);
        vec4 col = texture2D(fluidBuf, advUV);

        // Viscosity/flow: how much computed velocity feeds back
        float visc = 0.01 + flow * 0.08;
        col.xy = mix(col.xy, v * vec2(-1.0, 1.0) * sqrt(0.125) * 0.9, visc);

        // ---- Pulse: periodic radial injection ----
        if (pulse > 0.01) {
            float pulsePhase = sin(t * (0.5 + pulse * 2.0));
            vec2 center = (pos / Res) - 0.5;
            float dist = length(center);
            float pulseMask = smoothstep(0.3, 0.0, dist);
            vec2 radial = normalize(center + 0.001);
            col.xy += radial * pulsePhase * pulse * 0.004 * pulseMask;
        }

        // ---- Multiple swirl points for organic motion ----
        for (int s = 0; s < 3; s++) {
            float fs = float(s);
            float phase = t * (0.3 + fs * 0.15) + fs * 2.094;
            vec2 swirlPos = vec2(
                0.5 + 0.3 * sin(phase * 0.7 + fs),
                0.5 + 0.3 * cos(phase * 0.5 + fs * 1.5)
            ) * Res;
            vec2 d = fract((pos - swirlPos) / Res.x + 0.5) - 0.5;
            float falloff = 1.0 / (dot(d, d) / 0.04 + 0.08);
            vec2 swirlVel = vec2(cos(phase + fs * 0.5), sin(phase * 1.3 + fs));
            col.xy += swirlVel * flow * 0.002 * falloff;
        }

        // ---- Mouse interaction ----
        float interacting = max(mouseDown, pinchHold);
        if (interacting > 0.3) {
            vec2 scr = fract((pos - mousePos * Res) / Res.x + 0.5) - 0.5;
            float falloff = 1.0 / (dot(scr, scr) / 0.05 + 0.05);
            col.xy += 0.0003 * mouseDelta * Res * interacting * falloff;
        }

        // ---- Zap: sudden energy burst ----
        if (zap > 0.01) {
            float zapPhase = hash21(vec2(floor(t * 4.0), 0.0)) * PI2;
            vec2 zapPos = vec2(0.5 + 0.3 * cos(zapPhase), 0.5 + 0.3 * sin(zapPhase)) * Res;
            vec2 d = fract((pos - zapPos) / Res.x + 0.5) - 0.5;
            float falloff = 1.0 / (dot(d, d) / 0.02 + 0.03);
            vec2 zapDir = vec2(cos(zapPhase * 3.0), sin(zapPhase * 2.7));
            col.xy += zapDir * zap * 0.01 * falloff;
        }

        // ---- Audio-reactive push ----
        if (audioMod > 0.05) {
            vec2 center = (pos / Res) - 0.5;
            float pushAngle = hash21(vec2(float(FRAMEINDEX) * 0.1, 1.0)) * PI2;
            float dist = length(center);
            float radialPush = smoothstep(0.4, 0.0, dist);
            col.xy += vec2(cos(pushAngle), sin(pushAngle)) * audioMod * 0.005 * radialPush;

            // Mid and high frequencies add finer turbulence
            float turbulence = audioMid * reactivity * 0.003;
            col.xy += (hash22(pos * 0.1 + TIME) - 0.5) * turbulence;
        }

        // ---- Color injection into z/w channels (for rendering pass) ----
        float colorPhase = t * colorSpeed * 0.3 + colorRotate * PI2;
        float noiseVal = simplex2D(pos * 0.003 + t * 0.1);
        col.z = mix(col.z, sin(colorPhase + noiseVal * PI2) * 0.5 + 0.5, 0.02 + audioMod * 0.02);
        col.w = mix(col.w, cos(colorPhase * 1.3 + noiseVal * PI2 * 0.7) * 0.5 + 0.5, 0.02 + audioMod * 0.02);

        // Coruscate: shimmer overlay
        if (coruscate > 0.01) {
            float shimmer = simplex2D(pos * 0.01 + t * 0.5) * coruscate;
            col.z += shimmer * 0.05;
        }

        // Initialization
        if (FRAMEINDEX < 4) {
            float n1 = simplex2D(pos * 0.005);
            float n2 = simplex2D(pos * 0.005 + 100.0);
            col = vec4(n1 * 0.3, n2 * 0.3, 0.5, 0.5);
        }

        gl_FragColor = col;
        return;
    }

    // ===== PASS 1: Rendering =====

    // Lens distortion
    vec2 renderUV = uv;
    if (lens > 0.01) {
        renderUV = lensWarp(uv, lens * 0.5);
    }

    vec4 sim = texture2D(fluidBuf, renderUV);
    vec2 vel = sim.xy;
    float speed = length(vel);

    // ---- Chromatic aberration ----
    float chromAmt = chromatic * 0.02 * (1.0 + audioMod);
    vec2 chromDir = vel * chromAmt;

    float r = texture2D(fluidBuf, fract(renderUV + chromDir)).z;
    float g = texture2D(fluidBuf, fract(renderUV)).z;
    float b_ch = texture2D(fluidBuf, fract(renderUV - chromDir)).z;

    // Additional sampling from w channel for color depth
    float r2 = texture2D(fluidBuf, fract(renderUV + chromDir * 1.5)).w;
    float g2 = texture2D(fluidBuf, fract(renderUV + chromDir * 0.3)).w;
    float b2 = texture2D(fluidBuf, fract(renderUV - chromDir * 1.5)).w;

    // Build base color from fluid field
    vec3 fluidRGB = vec3(
        mix(r, r2, 0.5),
        mix(g, g2, 0.4),
        mix(b_ch, b2, 0.5)
    );

    // ---- Surface normal from velocity gradient ----
    float delta = 1.5 / Res.x;
    float vC = length(texture2D(fluidBuf, renderUV).xy);
    float vL = length(texture2D(fluidBuf, renderUV + vec2(-delta, 0.0)).xy);
    float vR = length(texture2D(fluidBuf, renderUV + vec2(delta, 0.0)).xy);
    float vU = length(texture2D(fluidBuf, renderUV + vec2(0.0, delta)).xy);
    float vD = length(texture2D(fluidBuf, renderUV + vec2(0.0, -delta)).xy);

    vec3 normal = normalize(vec3(
        -(vR - vL) * 0.1 * Res.x,
        -(vU - vD) * 0.1 * Res.y,
        1.0
    ));

    // ---- Color palette generation ----
    // Rich palette from fluid state: deep purples, magentas, cyans, golds
    float colorPhase = TIME * colorSpeed * 0.2 + colorRotate;
    float hue1 = fract(colorPhase + fluidRGB.r * 0.4 + speed * 2.0);
    float hue2 = fract(hue1 + 0.33);
    float hue3 = fract(hue1 + 0.67);

    vec3 palette1 = hsv2rgb(vec3(hue1, 0.7 + fluidRGB.g * 0.3, 0.6 + speed * 2.0));
    vec3 palette2 = hsv2rgb(vec3(hue2, 0.8, 0.5 + fluidRGB.b * 0.5));
    vec3 palette3 = hsv2rgb(vec3(hue3, 0.6, 0.8));

    // Blend palettes based on fluid channels
    vec3 col = mix(palette1, palette2, fluidRGB.g);
    col = mix(col, palette3, fluidRGB.b * 0.5);

    // ---- Surface lighting ----
    vec3 lightDir = normalize(vec3(0.4, 0.6, 1.0));
    float diffuse = max(dot(normal, lightDir), 0.0);
    float specular = pow(max(dot(reflect(-lightDir, normal), vec3(0.0, 0.0, 1.0)), 0.0), 32.0);

    col *= 0.6 + diffuse * 0.5;
    col += specular * 0.15 * (1.0 + audioMod);

    // ---- Coruscate: sparkle/shimmer ----
    if (coruscate > 0.01) {
        float sparkle = pow(max(simplex2D(renderUV * Res * 0.02 + TIME * 2.0), 0.0), 8.0);
        col += sparkle * coruscate * vec3(1.0, 0.9, 0.8) * 0.5;
    }

    // ---- Bars overlay ----
    if (bars > 0.01) {
        float barFreq = 10.0 + bars * 40.0;
        float barPattern = abs(sin(renderUV.x * barFreq * PI));
        barPattern = smoothstep(0.3, 0.7, barPattern);
        col = mix(col, col * (0.3 + barPattern * 0.7), bars);
    }

    // ---- Color rotation (hue shift) ----
    if (colorRotate > 0.01 || colorSpeed > 0.01) {
        vec3 hsv = rgb2hsv(col);
        hsv.x = fract(hsv.x + colorRotate + TIME * colorSpeed * 0.1);
        col = hsv2rgb(hsv);
    }

    // ---- Limit colors ----
    if (limitColors) {
        float lum = dot(col, vec3(0.299, 0.587, 0.114));
        col = mix(lowColor.rgb, highColor.rgb, smoothstep(0.0, 1.0, lum));
    }

    // ---- Audio brightness boost ----
    col += col * audioMod * 0.3;

    // Clamp
    col = clamp(col, 0.0, 1.0);

    // Transparent background
    float alpha = 1.0;
    if (transparentBg) {
        float lum = dot(col, vec3(0.299, 0.587, 0.114));
        alpha = smoothstep(0.02, 0.15, lum);
    }

    gl_FragColor = vec4(col, alpha);
}
