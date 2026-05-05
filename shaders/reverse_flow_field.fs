/*{
  "DESCRIPTION": "Aurora Borealis — layered sinusoidal light curtains in polar night sky with star field and treeline silhouette. Cool cinematic palette.",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "curtainCount", "LABEL": "Curtains",    "TYPE": "float", "DEFAULT": 6.0,  "MIN": 2.0,  "MAX": 10.0 },
    { "NAME": "waveSpeed",    "LABEL": "Wave Speed",  "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0,  "MAX": 2.0  },
    { "NAME": "bandWidth",    "LABEL": "Band Width",  "TYPE": "float", "DEFAULT": 0.14, "MIN": 0.02, "MAX": 0.5  },
    { "NAME": "hdrPeak",      "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.3,  "MIN": 0.5,  "MAX": 4.0  },
    { "NAME": "audioPulse",   "LABEL": "Audio Pulse", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0  }
  ]
}*/

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5); }
float hash2(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5); }

// 5-hue aurora palette (fully saturated, cool tones)
vec3 auroraColor(int ci) {
    if (ci == 0) return vec3(0.0,  1.0,  0.4);   // vivid green
    if (ci == 1) return vec3(0.0,  0.8,  1.0);   // electric cyan
    if (ci == 2) return vec3(0.6,  0.0,  1.0);   // deep violet
    if (ci == 3) return vec3(1.0,  0.0,  0.65);  // magenta
               return vec3(0.1,  1.0,  0.8);    // teal-mint
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float t  = TIME;

    // Night sky: very dark blue gradient
    vec3 sky = mix(vec3(0.0, 0.0, 0.008), vec3(0.003, 0.005, 0.025), uv.y);

    // Stars: 60 point lights
    float starGlow = 0.0;
    for (int i = 0; i < 60; i++) {
        float fi = float(i);
        vec2 sc = vec2(hash1(fi * 1.37), hash1(fi * 2.71) * 0.65 + 0.28);
        float twinkle = 0.65 + 0.35 * sin(t * (1.8 + hash1(fi * 0.91)) + fi * 3.14);
        float d = length(uv - sc);
        starGlow += smoothstep(0.006, 0.0, d) * twinkle;
    }
    vec3 col = sky + vec3(0.88, 0.92, 1.0) * starGlow;

    // Aurora curtains layered
    int N = int(clamp(curtainCount, 2.0, 10.0));
    for (int i = 0; i < 10; i++) {
        if (i >= N) break;
        float fi = float(i);
        float ph   = hash1(fi * 3.7) * 6.2832;
        float freq = 1.4 + hash1(fi * 2.1) * 2.2;
        float spd  = (0.35 + hash1(fi * 5.3) * 0.55) * waveSpeed;
        float yBase = 0.38 + hash1(fi * 7.1) * 0.38;

        // Horizontal oscillation defines the curtain shape
        float wavey = yBase
                    + sin(uv.x * freq * 3.1416 + t * spd + ph)  * 0.075
                    + sin(uv.x * freq * 1.7    + t * spd * 0.65 + ph*1.3) * 0.035;

        float dy   = abs(uv.y - wavey);
        float bw   = bandWidth * (0.7 + 0.3 * sin(t * 0.38 + ph));
        float band = exp(-dy * dy / (bw * bw)) * 1.6;
        band *= 1.0 + audioBass * audioPulse * 0.55;

        vec3 ac = auroraColor(int(mod(fi, 5.0)));
        col += ac * band * hdrPeak;
    }

    // Treeline silhouette (bottom 18%)
    float treeH = 0.13
                + 0.028 * sin(uv.x * 11.0)
                + 0.018 * sin(uv.x * 7.3 + 1.7)
                + 0.012 * sin(uv.x * 19.0 + 0.5);
    float ground = smoothstep(treeH + 0.012, treeH - 0.004, uv.y);
    col = mix(col, vec3(0.0, 0.0, 0.004), ground);

    // Faint moon
    vec2 moonPos = vec2(0.82, 0.78);
    float moonD  = length(uv - moonPos);
    col += vec3(0.85, 0.9, 1.0) * smoothstep(0.04, 0.0, moonD) * 2.0;
    col += vec3(0.5, 0.6, 0.8) * smoothstep(0.12, 0.04, moonD) * 0.2;

    gl_FragColor = vec4(col, 1.0);
}
