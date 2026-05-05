/*{
  "DESCRIPTION": "Stained Glass Cathedral — procedural gothic rose window with deep jewel-tone panels, black lead cames, and HDR light shafts",
  "CATEGORIES": ["Generator", "Art"],
  "CREDIT": "Easel / ShaderClaw v3",
  "INPUTS": [
    { "NAME": "rotateSpeed",  "LABEL": "Rotate Speed",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.12 },
    { "NAME": "panels",       "LABEL": "Ring Panels",   "TYPE": "float", "MIN": 4.0, "MAX": 16.0, "DEFAULT": 8.0  },
    { "NAME": "leadWidth",    "LABEL": "Lead Width",    "TYPE": "float", "MIN": 0.002,"MAX": 0.04, "DEFAULT": 0.012 },
    { "NAME": "hdrBoost",     "LABEL": "HDR Boost",     "TYPE": "float", "MIN": 1.0, "MAX": 4.0,  "DEFAULT": 2.2  },
    { "NAME": "shaftGlow",    "LABEL": "Shaft Glow",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0  },
    { "NAME": "audioReact",   "LABEL": "Audio React",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0  }
  ]
}*/

precision highp float;

// ── Palette: 5 jewel tones ────────────────────────────────────────────────────
const vec3 COBALT   = vec3(0.00, 0.18, 1.00);
const vec3 CRIMSON  = vec3(1.00, 0.04, 0.10);
const vec3 AMBER    = vec3(1.00, 0.55, 0.00);
const vec3 EMERALD  = vec3(0.00, 0.80, 0.20);
const vec3 VIOLET   = vec3(0.50, 0.00, 1.00);

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float came(float d) {
    return smoothstep(0.0, fwidth(d) * 1.5, abs(d) - leadWidth);
}

float shaft(vec2 uv, vec2 dir, float t) {
    float proj = dot(uv, dir);
    float perp = length(uv - dir * proj);
    float beam = exp(-perp * perp * 18.0) * max(0.0, proj);
    return beam * (0.85 + 0.15 * sin(t * 1.3 + proj * 4.0));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float t   = TIME * rotateSpeed;
    float aud = 1.0 + audioLevel * audioReact * 0.5 + audioBass * audioReact * 0.3;

    float r   = length(uv);
    float ang = atan(uv.y, uv.x);
    float angR = ang + t * 0.4;

    float N = panels;
    float sector = floor((angR + 3.14159265) / (6.28318530 / N));
    float sectorAng = (angR + 3.14159265) - sector * (6.28318530 / N);
    float midAng = (6.28318530 / N) * 0.5;

    const float R0 = 0.08;
    const float R1 = 0.25;
    const float R2 = 0.48;
    const float R3 = 0.72;
    const float R4 = 0.88;

    vec3 col = vec3(0.0);
    float glassAlpha = 0.0;

    if (r < R0) {
        float dc = came(r - R0 * 0.7);
        col = AMBER * hdrBoost * 1.4 * aud;
        glassAlpha = dc;
    } else if (r < R1) {
        float spoke = came(abs(sectorAng - midAng) - midAng * 0.4);
        float rimI = came(r - R1);
        float rimO = came(r - R0);
        float clear = min(spoke, min(rimI, rimO));
        col = (mod(sector, 2.0) < 1.0) ? VIOLET : COBALT;
        col *= hdrBoost * aud;
        glassAlpha = clear;
    } else if (r < R2) {
        float rimI = came(r - R2);
        float rimO = came(r - R1);
        float spk = came(abs(mod(angR * N / 6.28318530, 1.0) - 0.5) * 2.0 - (1.0 - leadWidth * 18.0));
        col = CRIMSON * hdrBoost * aud;
        glassAlpha = min(min(rimI, rimO), spk);
    } else if (r < R3) {
        float rimI = came(r - R3);
        float rimO = came(r - R2);
        float spk = came(abs(mod(angR * N / 6.28318530, 1.0) - 0.5) * 2.0 - (1.0 - leadWidth * 16.0));
        float tidx = mod(sector, 3.0);
        if (tidx < 1.0)      col = EMERALD;
        else if (tidx < 2.0) col = AMBER;
        else                  col = COBALT;
        col *= hdrBoost * aud;
        glassAlpha = min(min(rimI, rimO), spk);
    } else if (r < R4) {
        float rimI = came(r - R4);
        float rimO = came(r - R3);
        col = VIOLET * hdrBoost * 0.8 * aud;
        glassAlpha = min(rimI, rimO);
    } else {
        col = vec3(0.01, 0.01, 0.02);
        glassAlpha = 1.0;
    }

    vec3 glass = col * glassAlpha;

    float s1 = shaft(uv, normalize(vec2( 0.7,  1.0)), t);
    float s2 = shaft(uv, normalize(vec2(-0.6,  0.9)), t);
    float shaftCol3 = shaft(uv, normalize(vec2( 0.2,  1.0)), t);
    vec3 shafts = (AMBER * s1 + COBALT * s2 * 0.6 + VIOLET * shaftCol3 * 0.4)
                   * shaftGlow * aud * 0.5 * step(R4, r);

    float transmit = 0.0;
    if (r < R4 && glassAlpha > 0.5) {
        transmit = (0.25 + 0.1 * sin(t * 2.1 + ang * 3.0 + r * 8.0)) * aud;
    }

    gl_FragColor = vec4(glass + glass * transmit + shafts, 1.0);
}
