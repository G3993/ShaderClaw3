/*{
  "DESCRIPTION": "Neon Spiderweb — concentric neon rings with radial threads, slowly rotating",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw — analytic polar spiderweb",
  "INPUTS": [
    { "NAME": "numRings",   "LABEL": "Ring Count",    "TYPE": "float", "DEFAULT": 8.0,  "MIN": 2.0,  "MAX": 20.0 },
    { "NAME": "numThreads", "LABEL": "Thread Count",  "TYPE": "float", "DEFAULT": 12.0, "MIN": 3.0,  "MAX": 32.0 },
    { "NAME": "rotSpeed",   "LABEL": "Rot Speed",     "TYPE": "float", "DEFAULT": 0.15, "MIN": -1.0, "MAX": 1.0  },
    { "NAME": "wobble",     "LABEL": "Wobble",        "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 1.0  },
    { "NAME": "hdrRing",    "LABEL": "Ring HDR",      "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0  },
    { "NAME": "pulse",      "LABEL": "Audio Pulse",   "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0  },
    { "NAME": "colorDrift", "LABEL": "Color Drift",   "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0,  "MAX": 0.2  }
  ]
}*/

#define TAU 6.28318530718

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv     = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float audio  = 1.0 + audioBass * pulse;

    // Centered, aspect-corrected coordinates
    vec2 p = vec2((uv.x - 0.5) * aspect, uv.y - 0.5);

    // Bass-driven breathing: web expands on beat
    float breathe = 1.0 + audioBass * pulse * 0.07;
    p /= breathe;

    float r     = length(p);
    float theta = atan(p.y, p.x) + TIME * rotSpeed;

    float maxR        = 0.55;
    float ringSpacing = maxR / numRings;
    float threadAngle = TAU / numThreads;

    // Web wobble: rings oscillate with angle, threads with radius
    float rW = r
        + sin(theta * 3.0 + TIME * 0.9) * wobble * ringSpacing * 0.18
        + sin(theta * 7.1 - TIME * 0.5) * wobble * ringSpacing * 0.07;
    float thetaW = theta
        + sin(r * 14.0 + TIME * 0.7) * wobble * 0.06
        + sin(r * 28.3 - TIME * 0.4) * wobble * 0.025;

    // Ring distance (UV space)
    float dRing = mod(rW, ringSpacing);
    dRing = min(dRing, ringSpacing - dRing);

    // Thread distance (screen-space arc length)
    float dThetaRaw = mod(thetaW + threadAngle * 0.5, threadAngle) - threadAngle * 0.5;
    float dThread   = abs(dThetaRaw) * max(r, 0.015);

    // Web extent mask — inner void + outer fade
    float innerMask = smoothstep(0.0, ringSpacing * 0.6, r);
    float outerMask = smoothstep(maxR + 0.03, maxR - 0.02, r);
    float webMask   = innerMask * outerMask;

    // Pixel size for sharp AA
    float px = 0.5 / min(RENDERSIZE.x, RENDERSIZE.y);

    float ringCore  = (1.0 - smoothstep(0.0, px * 1.5, dRing))  * webMask;
    float ringGlow  = (1.0 - smoothstep(0.0, px * 7.0, dRing))  * webMask;
    float threadCore= (1.0 - smoothstep(0.0, px * 0.9, dThread))* webMask;
    float threadGlow= (1.0 - smoothstep(0.0, px * 4.0, dThread))* webMask;
    float nodeFlash = ringCore * threadCore; // intersection white-hot nodes

    // Ring color — 4-hue palette cycling per ring + global time drift
    // Palette: violet → cyan → gold → magenta (all fully saturated)
    float ringIdx    = floor(rW / ringSpacing);
    float globalHue  = fract(TIME * colorDrift);
    float colorPhase = fract(ringIdx / 4.0 + globalHue);

    vec3 VIOLET  = vec3(0.42, 0.0, 1.0);
    vec3 CYAN    = vec3(0.0,  0.88, 1.0);
    vec3 GOLD    = vec3(1.0,  0.75, 0.0);
    vec3 MAGENTA = vec3(1.0,  0.0,  0.85);

    vec3 ringColor;
    if (colorPhase < 0.25)      ringColor = mix(VIOLET,  CYAN,    colorPhase * 4.0);
    else if (colorPhase < 0.5)  ringColor = mix(CYAN,    GOLD,    (colorPhase - 0.25) * 4.0);
    else if (colorPhase < 0.75) ringColor = mix(GOLD,    MAGENTA, (colorPhase - 0.5)  * 4.0);
    else                         ringColor = mix(MAGENTA, VIOLET,  (colorPhase - 0.75) * 4.0);

    // Silver-neon threads (slight cool tint, dimensionally subordinate to rings)
    vec3 threadColor = vec3(0.65, 0.80, 1.0);
    // White-hot for node intersections
    vec3 nodeColor   = vec3(1.0, 0.96, 0.88);

    float b = hdrRing * audio;

    vec3 col = vec3(0.0);
    col += ringColor  * (ringCore  * b + ringGlow  * 0.35 * b);
    col += threadColor* (threadCore* b + threadGlow* 0.20 * b);
    col += nodeColor  * nodeFlash  * b * 2.8;  // white-hot HDR nodes

    gl_FragColor = vec4(col, 1.0);
}
