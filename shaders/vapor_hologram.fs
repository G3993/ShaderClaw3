/*{
  "DESCRIPTION": "Torii Gate at Dusk — Japanese torii gate silhouette against layered gradient sky, floating paper lanterns, cherry blossom petals drifting",
  "CATEGORIES": ["Generator", "Art", "Audio Reactive"],
  "CREDIT": "Easel / ShaderClaw v3",
  "INPUTS": [
    { "NAME": "skyShift",    "LABEL": "Sky Shift",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3  },
    { "NAME": "petalCount",  "LABEL": "Petal Count",  "TYPE": "float", "MIN": 5.0, "MAX": 30.0,"DEFAULT": 15.0 },
    { "NAME": "lanterns",    "LABEL": "Lanterns",     "TYPE": "float", "MIN": 0.0, "MAX": 8.0, "DEFAULT": 5.0  },
    { "NAME": "hdrBoost",    "LABEL": "HDR Boost",    "TYPE": "float", "MIN": 1.0, "MAX": 3.0, "DEFAULT": 2.0  },
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0  }
  ]
}*/

precision highp float;

// ── Palette: Japanese dusk (4 colors + black silhouette) ─────────────────────
const vec3 VERMILLION  = vec3(0.90, 0.15, 0.05);  // torii lacquer
const vec3 GOLD_SKY    = vec3(1.00, 0.65, 0.10);  // horizon glow
const vec3 DEEP_INDIGO = vec3(0.08, 0.05, 0.35);  // zenith sky
const vec3 SAKURA_PINK = vec3(1.00, 0.50, 0.60);  // cherry blossom
const vec3 INK_BLACK   = vec3(0.00, 0.00, 0.00);  // silhouette

float hash(float n)  { return fract(sin(n * 12.9898) * 43758.5453); }
float hash2(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// ── Sky gradient ──────────────────────────────────────────────────────────────
vec3 skyColor(float y, float t) {
    float skyT = y + skyShift * 0.2 + sin(t * 0.05) * 0.03;
    vec3 c0 = GOLD_SKY;           // horizon
    vec3 c1 = mix(VERMILLION, DEEP_INDIGO, 0.4); // mid
    vec3 c2 = DEEP_INDIGO;        // zenith
    if (skyT < 0.45) return mix(c0, c1, skyT / 0.45);
    return mix(c1, c2, (skyT - 0.45) / 0.55);
}

// ── Torii gate SDF ────────────────────────────────────────────────────────────
float sdBox(vec2 p, vec2 b) {
    vec2 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

float sdTorii(vec2 uv) {
    // Aspect correction
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - vec2(0.5, 0.0)) * vec2(aspect, 1.0);

    float d = 1e9;

    // Left pillar
    d = min(d, sdBox(p - vec2(-0.38, 0.28), vec2(0.04, 0.28)));
    // Right pillar
    d = min(d, sdBox(p - vec2( 0.38, 0.28), vec2(0.04, 0.28)));

    // Top horizontal beam (kasagi) — slightly curved via abs
    float kasagiY = 0.56 + abs(p.x) * 0.07;
    d = min(d, sdBox(p - vec2(0.0, kasagiY), vec2(0.52, 0.035)));

    // Lower horizontal beam (nuki)
    d = min(d, sdBox(p - vec2(0.0, 0.44), vec2(0.42, 0.025)));

    // Top cap overhangs (shimagi) — extends beyond kasagi
    d = min(d, sdBox(p - vec2(0.0, 0.60), vec2(0.56, 0.018)));

    // Komainu pedestals (small blocks at base of pillars)
    d = min(d, sdBox(p - vec2(-0.38, 0.01), vec2(0.065, 0.012)));
    d = min(d, sdBox(p - vec2( 0.38, 0.01), vec2(0.065, 0.012)));

    return d;
}

// ── Paper lantern ─────────────────────────────────────────────────────────────
float sdLantern(vec2 p, vec2 center, float scale) {
    vec2 q = (p - center) / scale;
    // Oval body
    float body = length(q / vec2(0.5, 0.7)) - 1.0;
    // Top + bottom caps
    float capT = sdBox(q - vec2(0, 0.75), vec2(0.18, 0.1));
    float capB = sdBox(q - vec2(0, -0.75), vec2(0.18, 0.1));
    return min(body, min(capT, capB)) * scale;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    float t   = TIME;
    float aud = 1.0 + (audioLevel + audioBass * 0.5) * audioReact * 0.4;

    // Sky
    vec3 col = skyColor(uv.y, t) * hdrBoost * aud;

    // Horizon glow line
    float horizon = smoothstep(0.04, 0.0, abs(uv.y - 0.14));
    col += GOLD_SKY * horizon * 1.5 * hdrBoost * aud;

    // Ground (dark earth / gravel path)
    float groundMask = step(uv.y, 0.14);
    col = mix(col, vec3(0.04, 0.02, 0.01), groundMask);

    // Torii gate silhouette
    float gateD  = sdTorii(uv);
    float gateMask = smoothstep(fwidth(gateD) * 1.5, -fwidth(gateD) * 0.5, gateD);

    // Gate: mostly black silhouette + vermillion lacquer tint where lit
    vec3 gateCol = mix(INK_BLACK, VERMILLION * 0.3, 0.25);
    col = mix(col, gateCol, gateMask);

    // Lantern glow (warm amber behind gate opening)
    for (int i = 0; i < 8; i++) {
        if (float(i) >= lanterns) break;
        float fi = float(i);
        float lx = 0.2 + hash(fi * 3.7) * 0.6;
        float ly = 0.18 + hash(fi * 5.1) * 0.4 + sin(t * 0.3 + fi) * 0.01;
        float ls = 0.015 + hash(fi * 2.9) * 0.012;
        float lDist = sdLantern(uv, vec2(lx, ly), ls);
        float lGlow = exp(-max(lDist, 0.0) * 60.0);
        float lBody = smoothstep(fwidth(lDist) * 1.5, -fwidth(lDist) * 0.5, lDist);
        float pulse = 0.85 + 0.15 * sin(t * 1.8 + fi * 2.3);
        col += GOLD_SKY * lGlow * pulse * 0.8 * hdrBoost * aud;
        col = mix(col, INK_BLACK, lBody * 0.7);
    }

    // Cherry blossom petals (drifting)
    for (int i = 0; i < 30; i++) {
        if (float(i) >= petalCount) break;
        float fi = float(i);
        float px = fract(hash(fi * 1.7) + t * 0.04 * (0.5 + hash(fi * 3.1) * 0.5));
        float py = fract(hash(fi * 2.3 + 0.5) - t * 0.06 * (0.3 + hash(fi * 4.7) * 0.4));
        float pr = 0.006 + hash(fi * 5.9) * 0.006;
        float pd = length(uv - vec2(px, py)) - pr;
        float pMask = smoothstep(fwidth(pd) * 2.0, -fwidth(pd), pd);
        col = mix(col, SAKURA_PINK * hdrBoost * aud * (0.8 + 0.2 * sin(t + fi)), pMask);
    }

    // Stars (faint dots in deep indigo sky)
    if (uv.y > 0.5) {
        vec2 starCell = floor(uv * 80.0);
        float star = step(0.987, hash2(starCell));
        float starFlicker = 0.7 + 0.3 * sin(t * 2.0 + hash2(starCell) * 6.28);
        col += GOLD_SKY * star * starFlicker * 0.6 * hdrBoost;
    }

    gl_FragColor = vec4(col, 1.0);
}
