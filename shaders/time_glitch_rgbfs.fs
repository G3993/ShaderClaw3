/*{
  "DESCRIPTION": "Neon VHS Datamosh — self-contained VHS corruption generator with scrolling bands, horizontal glitch displacement, and scattered static. Warm crimson/orange/gold HDR palette. No input required.",
  "CREDIT": "ShaderClaw3",
  "CATEGORIES": ["Generator", "Glitch"],
  "INPUTS": [
    { "NAME": "glitchAmount",  "LABEL": "Glitch Amount",   "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "scrollSpeed",   "LABEL": "Scroll Speed",    "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "staticDensity", "LABEL": "Static Density",  "TYPE": "float", "DEFAULT": 0.2,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "neonBoost",     "LABEL": "Neon Boost",      "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "audioMod",      "LABEL": "Audio Modulator", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float hash13(vec3 p3) {
    p3 = fract(p3 * 0.1031);
    p3 += dot(p3, p3.zyx + 31.32);
    return fract((p3.x + p3.y) * p3.z);
}

// Warm palette: 4 fully saturated HDR colors
//   0 = black void | 1 = crimson signal | 2 = neon orange | 3 = gold phosphor
vec3 paletteColor(int idx) {
    if (idx == 1) return vec3(2.2, 0.15, 0.1);   // crimson signal
    if (idx == 2) return vec3(2.5, 0.8,  0.0);   // neon orange
    if (idx == 3) return vec3(2.0, 1.8,  0.0);   // gold phosphor
    return vec3(0.0);                              // black void
}

float scanline(float y) {
    float lineIdx = floor(y * RENDERSIZE.y / 2.5);
    return 1.0 - 0.35 * step(0.5, fract(lineIdx * 0.5));
}

float staticDot(vec2 uv, float density, float t) {
    float tCell = floor(t * 12.0);
    float n = hash13(vec3(floor(uv * RENDERSIZE.xy), tCell));
    return step(1.0 - density, n);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

    float audioMult  = 1.0 + audioLevel * audioMod;
    float effGlitch  = clamp(glitchAmount * audioMult, 0.0, 1.0);
    float effStatic  = clamp(staticDensity * audioMult, 0.0, 1.0);

    float scrolledY = fract(uv.y + TIME * scrollSpeed * 0.05);

    // Walk down from y=1 allocating strips
    float cursor   = 1.0;
    float bandSeed = floor(TIME * 0.4);

    vec3  bandColor  = vec3(0.0);
    float glitchX    = 0.0;
    bool  isTear     = false;
    float bandHeight = 0.08;

    for (int i = 0; i < 48; i++) {
        float fi = float(i);
        float h  = 0.01 + 0.12 * hash11(fi * 7.3921 + bandSeed * 3.1);
        float bot = cursor - h;

        if (scrolledY >= bot) {
            // Color index 0-3
            float cRand = hash11(fi * 13.77 + bandSeed * 5.9);
            int cIdx;
            if      (cRand < 0.18) cIdx = 0;
            else if (cRand < 0.52) cIdx = 1;
            else if (cRand < 0.78) cIdx = 2;
            else                   cIdx = 3;
            bandColor = paletteColor(cIdx);

            // Glitch displacement
            float glitchRand = hash11(fi * 2.31 + bandSeed * 0.77);
            float glitchOn   = step(1.0 - effGlitch, glitchRand);
            float glitchDir  = (hash11(fi * 8.11 + bandSeed) - 0.5) * 2.0;
            float glitchMag  = hash11(fi * 4.53 + bandSeed * 2.1) * 0.18;
            glitchX = glitchOn * glitchDir * glitchMag;

            isTear    = (h < 0.008) && (hash11(fi * 9.17 + bandSeed) > 0.7);
            bandHeight = h;
            break;
        }
        cursor = bot;
        if (cursor < 0.0) break;
    }

    // Apply horizontal displacement
    float glitchedX = fract(uv.x + glitchX);
    float jitterSeed = floor(scrolledY * 200.0) + floor(TIME * 30.0);
    float jitterOn   = step(0.85, hash11(jitterSeed * 3.7));
    float microJitter = jitterOn * (hash11(jitterSeed) - 0.5) * 0.012 * effGlitch;
    glitchedX = fract(glitchedX + microJitter);

    vec3 col = isTear ? vec3(3.5, 3.0, 2.5) : bandColor;

    // Intra-band phosphor burn gradient
    float bandFrac = clamp((scrolledY - (cursor - bandHeight)) / max(bandHeight, 0.001), 0.0, 1.0);
    col *= 0.85 + 0.3 * bandFrac;

    // Band edge ink silhouette seam
    float edgeDist = abs(scrolledY - cursor) / max(bandHeight, 0.001);
    float edgeGlow = exp(-edgeDist * 8.0);
    col = mix(col, col * 1.5, edgeGlow * 0.6);

    col *= neonBoost;
    col *= scanline(uv.y);

    // Scattered static sparks
    float spark = staticDot(vec2(glitchedX, uv.y), effStatic, TIME);
    float sparkPal = hash12(gl_FragCoord.xy + vec2(floor(TIME * 12.0)));
    vec3 sparkColor;
    if      (sparkPal < 0.33) sparkColor = vec3(2.2, 0.15, 0.1);
    else if (sparkPal < 0.66) sparkColor = vec3(2.5, 0.8,  0.0);
    else                      sparkColor = vec3(2.0, 1.8,  0.0);
    col = mix(col, sparkColor, spark);

    FragColor = vec4(col, 1.0);
}
