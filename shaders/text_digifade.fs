/*{
  "DESCRIPTION": "Cathedral Glass — rose window stained glass, analytic circle SDFs, fwidth AA",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed",     "LABEL": "Spin Speed", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0,   "MAX": 1.0  },
    { "NAME": "zoom",      "LABEL": "Zoom",       "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3,   "MAX": 3.0  },
    { "NAME": "hdrPeak",   "LABEL": "Brightness", "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.5,   "MAX": 5.0  },
    { "NAME": "leadWidth", "LABEL": "Lead Lines", "TYPE": "float", "DEFAULT": 0.012,"MIN": 0.002, "MAX": 0.05 }
  ]
}*/

const float PI  = 3.14159265;
const float TAU = 6.28318530;

#define COBALT  vec3(0.08, 0.22, 0.88)
#define CRIMSON vec3(0.78, 0.03, 0.09)
#define GOLD    vec3(1.0,  0.82, 0.0)
#define EMERALD vec3(0.03, 0.54, 0.22)
#define VOID    vec3(0.0,  0.0,  0.012)

// Paint a circle pane with fwidth-AA fill and black lead-came border
void applyPane(inout vec3 col, vec3 paneColor, float d, float lw) {
    float fw   = fwidth(d);
    float lead = smoothstep(lw + fw, lw - fw, abs(d));
    float fill = smoothstep(fw, -fw, d) * (1.0 - lead);
    col = mix(col, paneColor, fill);
    col = mix(col, VOID,      lead);
}

vec4 renderGlass(vec2 uv) {
    float t     = TIME * speed;
    float audio = 1.0 + audioBass * 0.18;
    uv /= (zoom * audio);

    float r  = length(uv);
    float lw = leadWidth;
    vec3 col = VOID;

    // Outer frame ring
    applyPane(col, EMERALD * hdrPeak * 1.5, abs(r - 0.95) - 0.025, lw);

    // Ring 3 — 18 emerald petals (counter-rotate)
    for (int i = 0; i < 18; i++) {
        float ang = TAU * float(i) / 18.0 - t * 0.12;
        vec2  c   = vec2(cos(ang), sin(ang)) * 0.80;
        applyPane(col, EMERALD * hdrPeak * 1.6, length(uv - c) - 0.095, lw);
    }

    // Ring 2 — 12 crimson petals
    for (int i = 0; i < 12; i++) {
        float ang = TAU * float(i) / 12.0 + t * 0.18;
        vec2  c   = vec2(cos(ang), sin(ang)) * 0.57;
        applyPane(col, CRIMSON * hdrPeak * 1.9, length(uv - c) - 0.125, lw);
    }

    // Ring 1 — 6 cobalt petals
    for (int i = 0; i < 6; i++) {
        float ang = TAU * float(i) / 6.0 + t * 0.25;
        vec2  c   = vec2(cos(ang), sin(ang)) * 0.33;
        applyPane(col, COBALT * hdrPeak * 2.1, length(uv - c) - 0.145, lw);
    }

    // Center gold medallion
    applyPane(col, GOLD * hdrPeak * 2.5, r - 0.12, lw);

    // Hot gold core — audio-reactive inner eye
    float corePulse = 1.0 + audioHigh * 0.4;
    applyPane(col, GOLD * hdrPeak * 3.0 * corePulse, r - 0.04, lw * 0.5);

    return vec4(col, 1.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);
    vec4 col = renderGlass(uv);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float t = TIME * 17.0;
        float band       = floor(uv.y * mix(8.0, 40.0, g) + t * 3.0);
        float bandNoise  = fract(sin(band * 91.7 + t) * 43758.5);
        float bandActive = step(1.0 - g * 0.6, bandNoise);
        float shift      = (bandNoise - 0.5) * 0.08 * g * bandActive;
        float chromaAmt  = g * 0.015;
        vec2 uvR = uv + vec2(shift + chromaAmt, 0.0);
        vec2 uvB = uv + vec2(shift - chromaAmt, 0.0);
        vec2 uvG = uv + vec2(shift, chromaAmt * 0.5);
        vec4 cR = renderGlass(uvR);
        vec4 cG = renderGlass(uvG);
        vec4 cB = renderGlass(uvB);
        vec4 glitched = vec4(cR.r, cG.g, cB.b, max(max(cR.a, cG.a), cB.a));
        float scanline   = 0.95 + 0.05 * sin(uv.y * RENDERSIZE.y * 1.5 + t * 40.0);
        float blockX     = floor(uv.x * 6.0);
        float blockY     = floor(uv.y * 4.0);
        float blockNoise = fract(sin((blockX + blockY * 7.0) * 113.1 + floor(t * 8.0)) * 43758.5);
        float dropout    = step(1.0 - g * 0.15, blockNoise);
        glitched.rgb *= scanline;
        glitched.rgb *= 1.0 - dropout;
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = col;
}
