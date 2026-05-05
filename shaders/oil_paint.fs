/*{
  "DESCRIPTION": "Fauvist Mediterranean — standalone scene generator: bold flat-color Fauvist landscape with cadmium sun, cerulean sea, and vermillion cliffs. No image input needed.",
  "CATEGORIES": ["Generator", "Art"],
  "CREDIT": "Easel / ShaderClaw v3",
  "INPUTS": [
    { "NAME": "waveSpeed",   "LABEL": "Wave Speed",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.5  },
    { "NAME": "horizonY",    "LABEL": "Horizon",      "TYPE": "float", "MIN": 0.3, "MAX": 0.7,  "DEFAULT": 0.52 },
    { "NAME": "hdrBoost",    "LABEL": "HDR Boost",    "TYPE": "float", "MIN": 1.0, "MAX": 3.0,  "DEFAULT": 2.0  },
    { "NAME": "sunSize",     "LABEL": "Sun Size",     "TYPE": "float", "MIN": 0.04,"MAX": 0.25, "DEFAULT": 0.10 },
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0  }
  ]
}*/

precision highp float;

// ── Palette: 5 Fauvist primaries (no white-mixing) ───────────────────────────
const vec3 CADMIUM_YELLOW = vec3(1.00, 0.80, 0.00);
const vec3 VERMILLION     = vec3(1.00, 0.15, 0.05);
const vec3 CERULEAN       = vec3(0.00, 0.35, 1.00);
const vec3 EMERALD        = vec3(0.00, 0.70, 0.15);
const vec3 DEEP_INDIGO    = vec3(0.08, 0.00, 0.30);

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * (sin(p.x) * cos(p.y));
        p = p * 2.1 + vec2(1.7, 2.3);
        a *= 0.5;
    }
    return v;
}

// AA step (edge sharpness matching Fauvist black outlines)
float edge(float d) { return smoothstep(fwidth(d) * 1.2, 0.0, d); }

void main() {
    vec2 uv = isf_FragNormCoord;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float aud = 1.0 + (audioLevel + audioBass * 0.5) * audioReact * 0.4;

    float y = uv.y;
    float x = uv.x;

    // ── Sky ───────────────────────────────────────────────────────────────────
    vec3 col = mix(DEEP_INDIGO, CERULEAN, smoothstep(horizonY, 1.0, y));
    // Fauvist brushstroke pattern in sky (wavy horizontal bands)
    float skyWave = 0.015 * sin((uv.x * 6.0 + t * 0.3) * 3.14159) * sin(uv.y * 8.0 + t * 0.2);
    col = mix(col, col * 1.4, smoothstep(0.02, 0.0, abs(mod(y - skyWave + 0.05, 0.12) - 0.06) - 0.005));

    // ── Sun ───────────────────────────────────────────────────────────────────
    vec2 sunPos = vec2(0.5, horizonY + 0.18 + 0.04 * sin(t * 0.2));
    float sunDist = length(uv - sunPos) - sunSize;
    float sunGlow = exp(-max(sunDist, 0.0) * 18.0);
    float sunBody = edge(-sunDist);
    // Black outline
    float sunOutline = smoothstep(0.0, fwidth(sunDist), -sunDist - sunSize * 0.08);
    col = mix(col, CADMIUM_YELLOW * hdrBoost * 1.5 * aud, sunBody);
    col = mix(col, vec3(0.0), sunOutline * (1.0 - sunBody) * 0.5);
    col += CADMIUM_YELLOW * sunGlow * 0.6 * hdrBoost * aud;

    // ── Sea (below horizon) ───────────────────────────────────────────────────
    if (y < horizonY) {
        // Fauvist flat sea with wave stripes
        float seaWave = sin(uv.x * 14.0 + t * waveSpeed * 3.0) * 0.012
                      + sin(uv.x * 7.5 - t * waveSpeed * 2.0) * 0.008;
        float waveStripe = abs(mod(y - horizonY - seaWave, 0.05) - 0.025);
        float waveEdge   = smoothstep(0.003, 0.001, waveStripe);
        vec3 seaCol = mix(CERULEAN * 0.6, CERULEAN, smoothstep(0.0, horizonY, y));
        seaCol = mix(seaCol, seaCol * 1.8, waveEdge);
        // Sun reflection stripe
        float refX = abs(uv.x - sunPos.x) * 4.0;
        float refY = (horizonY - y) * 12.0;
        float refGlow = exp(-refX * refX) * exp(-refY * 0.5);
        seaCol += CADMIUM_YELLOW * refGlow * 0.8 * hdrBoost * aud;
        col = seaCol;
    }

    // ── Cliffs (left and right foreground) ───────────────────────────────────
    float cliffL = step(0.0, -(x - 0.0) + (0.22 + 0.04 * fbm(vec2(y * 8.0, 1.0))));
    float cliffR = step(0.0, (x - 1.0)  + (0.22 + 0.04 * fbm(vec2(y * 8.0 + 3.0, 2.0))));
    float cliffY = smoothstep(0.0, horizonY * 0.6, y);
    float cliffMask = max(cliffL, cliffR) * cliffY;
    // Cliff: dark outline edge + vermillion body
    float cliffEdgeL = step(0.0, -(x - 0.0) + (0.22 + 0.04 * fbm(vec2(y * 8.0, 1.0))) - 0.006);
    float cliffEdgeR = step(0.0, (x - 1.0)  + (0.22 + 0.04 * fbm(vec2(y * 8.0 + 3.0, 2.0))) - 0.006);
    float cliffEdge  = max(cliffEdgeL, cliffEdgeR) * cliffY;
    vec3 cliffCol = VERMILLION * hdrBoost * aud;
    col = mix(col, vec3(0.0), cliffMask - cliffEdge);      // black outline
    col = mix(col, cliffCol, cliffEdge);

    // ── Foreground vegetation (bottom band) ───────────────────────────────────
    float vegY = smoothstep(0.1, 0.0, y);
    float vegWave = 0.5 + 0.5 * sin(uv.x * 22.0 + t * 0.8) * sin(uv.x * 13.0 - t * 0.5);
    col = mix(col, EMERALD * hdrBoost * aud * vegWave, vegY * 0.9);
    // Black outline at base
    col = mix(col, vec3(0.0), smoothstep(0.01, 0.0, y) * 0.8);

    // ── Horizon line (black) ──────────────────────────────────────────────────
    float hLine = smoothstep(0.004, 0.0, abs(y - horizonY));
    col = mix(col, vec3(0.0), hLine);

    gl_FragColor = vec4(col, 1.0);
}
