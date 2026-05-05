/*{
  "DESCRIPTION": "Neon Mandala Kaleidoscope — 2D kaleidoscopic mandala with HDR neon petals, concentric rings, and audio-reactive pulse",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "petalCount",  "LABEL": "Petals",      "TYPE": "float", "DEFAULT": 8.0,  "MIN": 3.0, "MAX": 16.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.4,  "MIN": 1.0, "MAX": 4.0  },
    { "NAME": "rotSpeed",    "LABEL": "Spin Speed",  "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0, "MAX": 2.0  },
    { "NAME": "audioPulse",  "LABEL": "Audio Pulse", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0  },
    { "NAME": "zoom",        "LABEL": "Zoom",        "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3, "MAX": 3.0  }
  ]
}*/

const float PI = 3.14159265;
const float TWO_PI = 6.28318530;

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5); }

// 5-color neon palette
vec3 neonColor(int ci) {
    if (ci == 0) return vec3(1.0,  0.0,  0.55);  // hot pink
    if (ci == 1) return vec3(0.0,  0.9,  1.0);   // electric cyan
    if (ci == 2) return vec3(1.0,  0.8,  0.0);   // gold
    if (ci == 3) return vec3(0.0,  1.0,  0.4);   // vivid green
               return vec3(0.65, 0.0,  1.0);   // violet
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
    uv /= zoom;

    float r = length(uv);
    float theta = atan(uv.y, uv.x) + TIME * rotSpeed;

    // K-fold kaleidoscope mirror
    float K = floor(clamp(petalCount, 3.0, 16.0));
    float sector = TWO_PI / K;
    theta = mod(theta, sector);
    if (theta > sector * 0.5) theta = sector - theta;  // mirror

    // Polar coordinates in reduced sector
    vec2 kp = vec2(r * cos(theta), r * sin(theta));

    vec3 col = vec3(0.0);
    float bassPulse = 1.0 + audioBass * audioPulse * 0.5;
    float midPulse  = 1.0 + audioMid  * audioPulse * 0.35;

    // Layer 1: Concentric rings (5 rings)
    for (int i = 0; i < 5; i++) {
        float fi = float(i + 1);
        float rTarget = (fi * 0.13) * (i == 0 ? bassPulse : 1.0);
        float dr = abs(r - rTarget) - 0.008;
        float ring = exp(-dr*dr * 2500.0);
        vec3 rc = neonColor(i);
        col += rc * ring * hdrPeak * (i == 0 ? bassPulse : 1.0);
    }

    // Layer 2: Radial petal lines
    float petalFreq = K;
    float petalLine = abs(sin(theta * petalFreq)) - 0.92;
    petalLine = abs(petalLine) - 0.012;
    float petals = exp(-petalLine*petalLine * 800.0) * smoothstep(0.65, 0.05, r);
    col += neonColor(1) * petals * hdrPeak * midPulse;

    // Layer 3: Inner star burst (diagonal petal lines at half-angle)
    float starLine = abs(sin(theta * petalFreq * 2.0)) - 0.88;
    starLine = abs(starLine) - 0.008;
    float starburst = exp(-starLine*starLine * 1200.0) * smoothstep(0.35, 0.02, r);
    col += neonColor(0) * starburst * hdrPeak * bassPulse;

    // Layer 4: Diamond lattice in the reduced sector
    float dx = abs(fract(kp.x * 5.5 + 0.5) - 0.5);
    float dy = abs(fract(kp.y * 5.5 + 0.5) - 0.5);
    float lattice = smoothstep(0.45, 0.42, min(dx, dy)) * smoothstep(0.7, 0.1, r);
    col += neonColor(4) * lattice * hdrPeak * 0.5;

    // Layer 5: Outer halo bloom
    float halo = exp(-max(0.0, r - 0.7) * 8.0) * 0.4;
    col += neonColor(3) * halo * hdrPeak * 0.4;

    // Center spark
    col += neonColor(2) * exp(-r*r * 180.0) * hdrPeak * bassPulse;

    // Breathing background tint
    float breathe = 0.5 + 0.5*sin(TIME * 0.8);
    vec3 bgTint = mix(vec3(0.0, 0.0, 0.015), vec3(0.008, 0.0, 0.025), breathe);
    col = bgTint + col;

    gl_FragColor = vec4(col, 1.0);
}
