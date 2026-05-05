/*{
  "DESCRIPTION": "Spiral Galaxy Core — 2D analytic spiral galaxy with differential rotation, dust lanes, and HDR core bloom. v9: 2D mathematical galaxy vs prior 3D coral reef, spirograph, graffiti walkers, lava lake, jellyfish, cube lattice.",
  "CREDIT": "ShaderClaw auto-improve v9",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "rotSpeed",   "LABEL": "Rotation Speed", "TYPE": "float", "DEFAULT": 0.18,  "MIN": 0.0, "MAX": 1.0  },
    { "NAME": "armCount",   "LABEL": "Spiral Arms",    "TYPE": "float", "DEFAULT": 2.0,   "MIN": 1.0, "MAX": 4.0  },
    { "NAME": "armWrap",    "LABEL": "Arm Tightness",  "TYPE": "float", "DEFAULT": 2.8,   "MIN": 0.5, "MAX": 6.0  },
    { "NAME": "starDensity","LABEL": "Star Density",   "TYPE": "float", "DEFAULT": 0.85,  "MIN": 0.1, "MAX": 2.0  },
    { "NAME": "coreColor",  "LABEL": "Core Color",     "TYPE": "color", "DEFAULT": [1.0, 0.75, 0.2, 1.0]         },
    { "NAME": "armColor",   "LABEL": "Arm Color",      "TYPE": "color", "DEFAULT": [0.0,  0.7,  1.0, 1.0]        },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",       "TYPE": "float", "DEFAULT": 2.8,   "MIN": 1.0, "MAX": 4.0  },
    { "NAME": "audioReact", "LABEL": "Audio React",    "TYPE": "float", "DEFAULT": 0.7,   "MIN": 0.0, "MAX": 2.0  }
  ]
}*/

#define PI 3.14159265359
#define TAU 6.28318530718

float hash11(float n) { float p = fract(n * 0.1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash12(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }

// Value noise 1D
float vnoise(float x) {
    float i = floor(x); float f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    return mix(hash11(i), hash11(i + 1.0), f);
}

// Star hash: fractured Voronoi-like — checks 9 neighboring cells
float starField(vec2 p, float density) {
    vec2 ip = floor(p * 60.0);
    vec2 fp = fract(p * 60.0);
    float bri = 0.0;
    for (int x = -1; x <= 1; x++) {
        for (int y = -1; y <= 1; y++) {
            vec2 nb = ip + vec2(float(x), float(y));
            float h = hash12(nb);
            if (h > (1.0 - density * 0.04)) {
                vec2 starPos = vec2(hash12(nb + 7.31), hash12(nb + 13.7));
                float d = length(fp - (vec2(float(x), float(y)) + starPos));
                bri += exp(-d * d * 300.0) * (0.3 + hash12(nb + 2.1) * 0.7);
            }
        }
    }
    return bri;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float audio = 1.0 + audioLevel * audioReact + audioBass * audioReact * 0.5;
    float t = TIME * rotSpeed;

    float r = length(uv);
    float theta = atan(uv.y, uv.x);

    // ── Galactic background: deep space dark navy ──
    vec3 col = vec3(0.0, 0.0, 0.02);

    // Faint star background haze (random field)
    float bgStars = starField(uv * 0.25, 1.5);
    col += vec3(0.6, 0.7, 1.0) * bgStars * 0.15;

    // ── Spiral arms (analytic density) ──
    int ARMS = int(clamp(armCount, 1.0, 4.0));
    float armDensity = 0.0;
    float armHue = 0.0;

    for (int arm = 0; arm < 4; arm++) {
        if (arm >= ARMS) break;
        float fa = float(arm);
        float armPhase = fa * TAU / float(ARMS);

        // Logarithmic spiral: theta_spiral = armWrap * ln(r) + armPhase
        // Differential rotation: angular velocity ∝ 1/sqrt(r) (flat rotation curve)
        float angVel = 1.0 / max(sqrt(r), 0.08);
        float spiralTheta = armWrap * log(max(r, 0.01) * 4.0) + armPhase - t * angVel * 0.6;

        // Angular distance from arm center
        float dTheta = theta - spiralTheta;
        // Wrap to [-PI, PI]
        dTheta = dTheta - TAU * floor((dTheta + PI) / TAU);

        // Arm width increases with radius (arms flare outward)
        float width = (0.25 + r * 0.5) / (1.0 + r * 2.0);
        float contrib = exp(-dTheta * dTheta / (width * width + 0.01));

        // Radial falloff: arms fade near center and at large radius
        float radFade = smoothstep(0.05, 0.15, r) * exp(-r * 1.4);

        armDensity += contrib * radFade;
        armHue += contrib * radFade * (fa * 0.18);
    }

    // ── Arm color: mix between coreColor (inner) and armColor (outer) ──
    float armMix = smoothstep(0.08, 0.45, r);
    vec3 armCol = mix(coreColor.rgb, armColor.rgb, armMix) * hdrPeak;

    // Arm glow with audio modulation
    float density = armDensity * starDensity * audio;
    col += armCol * density;

    // ── Dust lanes: dark absorption between arms (cinematic depth) ──
    float dustAng = theta - t * 0.3;
    float dustWave = vnoise(dustAng * 2.0 + armWrap * log(max(r, 0.01) * 4.0)) * 0.5;
    float dustMask = smoothstep(0.05, 0.3, r) * dustWave * 0.6;
    col = mix(col, vec3(0.0), dustMask);

    // ── Galactic core: white-hot center with gold/orange corona ──
    float coreR = 0.06 * (1.0 + audioBass * audioReact * 0.3);
    float coreDist = r / max(coreR, 0.001);

    // Inner core: white-hot
    float coreIntensity = exp(-coreDist * coreDist * 0.8) * 3.5 * audio;
    col += vec3(3.5, 3.2, 2.8) * coreIntensity; // HDR 3.5 white-hot

    // Corona: gold-to-crimson
    float corona = exp(-coreDist * coreDist * 0.05) * 1.5;
    col += mix(coreColor.rgb, vec3(0.9, 0.1, 0.0), coreDist * 0.3) * corona * hdrPeak;

    // ── Individual bright stars in arms: scattered sparkles ──
    float armStars = 0.0;
    for (int i = 0; i < 60; i++) {
        float fi = float(i);
        // Place star along a spiral arm
        float rStar = 0.1 + hash11(fi * 1.37) * 0.65;
        float angVelStar = 1.0 / max(sqrt(rStar), 0.08);
        float armIdx = floor(hash11(fi * 3.71) * float(ARMS));
        float thetaStar = armWrap * log(max(rStar, 0.01) * 4.0)
                        + armIdx * TAU / float(ARMS)
                        - t * angVelStar * 0.6
                        + (hash11(fi * 7.53) - 0.5) * 0.8; // scatter off arm
        vec2 starPos = vec2(cos(thetaStar), sin(thetaStar)) * rStar;
        float d = length(uv - starPos);
        float brightness = (0.4 + hash11(fi * 11.3) * 0.6) * exp(-d * d * 2000.0);
        float hue = 0.55 + hash11(fi * 5.17) * 0.25; // blue-white stars
        armStars += brightness;
    }
    col += mix(armColor.rgb, vec3(2.5, 2.5, 2.5), 0.7) * armStars * hdrPeak;

    // ── Edge vignette ──
    col *= 1.0 - smoothstep(0.85, 1.3, r);

    gl_FragColor = vec4(col, 1.0);
}
