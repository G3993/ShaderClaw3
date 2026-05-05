/*{
  "DESCRIPTION": "Van Gogh Starry Night — swirling domain-warped night sky with HDR star halos and luminous brushstroke turbulence",
  "CATEGORIES": ["Generator", "Art"],
  "CREDIT": "ShaderClaw auto-improve",
  "INPUTS": [
    { "NAME": "swirlSpeed",  "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "LABEL": "Swirl Speed" },
    { "NAME": "starBright",  "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 4.0, "LABEL": "Star HDR" },
    { "NAME": "audioMod",    "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" },
    { "NAME": "skyColor",    "TYPE": "color", "DEFAULT": [0.02, 0.02, 0.12, 1.0], "LABEL": "Sky Dark" },
    { "NAME": "waveColor",   "TYPE": "color", "DEFAULT": [0.0, 0.35, 0.9, 1.0],   "LABEL": "Wave Blue" }
  ]
}*/

// ---- Hash / Noise ----
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float noise2(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(hash(i),               hash(i + vec2(1.0, 0.0)), u.x),
        mix(hash(i + vec2(0.0,1.0)), hash(i + vec2(1.0, 1.0)), u.x),
        u.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        v += a * noise2(p);
        p = p * 2.03 + vec2(3.7, 1.9);
        a *= 0.5;
    }
    return v;
}

// ---- SDF helpers ----
float sdCircle(vec2 p, float r) {
    return length(p) - r;
}

// Crescent moon: large circle minus smaller offset circle
float sdCrescent(vec2 p, float r, float r2, vec2 offset) {
    float outer = sdCircle(p, r);
    float inner = sdCircle(p - offset, r2);
    return max(outer, -inner);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 st = (uv - 0.5) * vec2(aspect, 1.0);   // aspect-corrected, centered

    // Audio modulator (never a gate)
    float audio = 1.0 + audioMod * 0.5;

    // ---- Domain-warped FBM (Van Gogh swirling brushstrokes) ----
    // Scale UV for swirl domain
    vec2 swirlUV = uv * vec2(aspect, 1.0) * 3.0;
    float t = TIME * swirlSpeed * 0.12;

    vec2 q = vec2(
        fbm(swirlUV),
        fbm(swirlUV + vec2(5.2, 1.3))
    );
    vec2 r = vec2(
        fbm(swirlUV + 3.0 * q + t + vec2(1.7, 9.2)),
        fbm(swirlUV + 3.0 * q + vec2(8.3, 2.8))
    );
    float f = fbm(swirlUV + 3.5 * r);

    // ---- Background: midnight sky with cobalt swirl ----
    vec3 sky  = skyColor.rgb;   // deep midnight navy
    vec3 wave = waveColor.rgb;  // cobalt blue wave
    vec3 col = mix(sky, wave, f * f * 1.4);

    // Add a warmer brushstroke band near swirl peaks
    vec3 goldBand = vec3(0.55, 0.4, 0.05);
    col += goldBand * smoothstep(0.6, 0.85, f) * 0.45;

    // ---- Crescent moon (upper-right) ----
    // Moon in normalised UV space
    vec2 moonCenter = vec2(0.72 * aspect, 0.36) - vec2(0.5 * aspect, 0.5);
    float moonR  = 0.072;
    float moonR2 = 0.058;
    vec2  moonOff = vec2(0.038, 0.0);
    float dMoon = sdCrescent(st - moonCenter, moonR, moonR2, moonOff);
    float fw0 = fwidth(dMoon);
    float moonMask = 1.0 - smoothstep(-fw0, fw0, dMoon);
    // Moon: electric gold HDR
    vec3 moonCol = vec3(1.0, 0.85, 0.0) * 2.5;
    // Black ink edge
    float moonEdge = smoothstep(-fw0 * 3.0, -fw0, dMoon) * (1.0 - smoothstep(-fw0, fw0 * 2.0, dMoon));
    col = mix(col, moonCol, moonMask);
    col = mix(col, vec3(0.0), moonEdge * 0.9);

    // Soft glow around moon
    float moonGlow = exp(-max(0.0, dMoon) * 14.0) * 0.8;
    col += vec3(0.9, 0.75, 0.0) * moonGlow * starBright * 0.35 * audio;

    // ---- 8 Stars (hardcoded positions in UV [0..1] space) ----
    // Each star is a circular HDR point with glow halo
    vec2 starPos[8];
    starPos[0] = vec2(0.15, 0.82);
    starPos[1] = vec2(0.31, 0.91);
    starPos[2] = vec2(0.52, 0.88);
    starPos[3] = vec2(0.64, 0.78);
    starPos[4] = vec2(0.43, 0.70);
    starPos[5] = vec2(0.22, 0.65);
    starPos[6] = vec2(0.08, 0.55);
    starPos[7] = vec2(0.78, 0.85);

    // Star color: white-hot core, electric gold halo
    vec3 starCore = vec3(2.0, 2.0, 2.0);
    vec3 starGold  = vec3(1.0, 0.85, 0.0) * 2.5;

    for (int i = 0; i < 8; i++) {
        // Convert to aspect-correct centered space
        vec2 sp = (starPos[i] - 0.5) * vec2(aspect, 1.0);
        float dist = length(st - sp);

        // Flicker per-star with time
        float flicker = 0.85 + 0.15 * sin(TIME * (2.3 + float(i) * 0.97) + float(i) * 1.7);

        // Core: very tight HDR point
        float core = max(0.0, 1.0 - dist * starBright * 28.0);
        col += starCore * pow(core, 3.0) * starBright * audio * flicker;

        // Gold halo
        float halo = max(0.0, 1.0 - dist * starBright * 11.0);
        col += starGold * pow(halo, 2.5) * audio * flicker * 0.7;

        // Soft outer glow ring
        float glow = exp(-dist * 18.0) * 0.4;
        col += vec3(0.3, 0.5, 1.0) * glow * starBright * 0.3 * audio * flicker;

        // Cross-shaped diffraction spike (horizontal + vertical)
        float spike = max(0.0, 1.0 - abs(st.x - sp.x) * 90.0) *
                      exp(-abs(st.y - sp.y) * 40.0) * 0.4;
        spike      += max(0.0, 1.0 - abs(st.y - sp.y) * 90.0) *
                      exp(-abs(st.x - sp.x) * 40.0) * 0.4;
        col += starGold * spike * audio * flicker;
    }

    // ---- Spiral arm highlight near center ----
    // A broad swirl of electric cobalt running across the middle
    float ang = atan(st.y, st.x);
    float rad = length(st);
    float spiralPhase = ang / (2.0 * 3.14159) + rad * 1.5 - TIME * swirlSpeed * 0.2;
    float spiral = smoothstep(0.0, 0.3, sin(spiralPhase * 2.0 * 3.14159 * 3.0) * 0.5 + 0.5);
    // Only in the lower half of the frame (the village area)
    spiral *= smoothstep(0.1, 0.0, uv.y - 0.55);
    col += vec3(0.0, 0.4, 1.0) * spiral * 0.35 * audio;

    // ---- Dark village silhouette at bottom ----
    // Simple rectangular black band at the bottom 15% of frame
    float villageY = 0.18;
    float villageEdge = smoothstep(villageY, villageY - 0.02, uv.y);
    col = mix(col, vec3(0.0), villageEdge);

    // Cypress-tree-like spire: narrow dark triangle on left
    float cx = abs(uv.x - 0.12) * aspect;
    float cy = (uv.y - villageY) / 0.25;   // height in village band
    float spire = smoothstep(0.03, 0.0, cx - cy * 0.04) * smoothstep(1.0, 0.0, cy);
    spire *= step(villageY, uv.y) * step(uv.y, villageY + 0.30);
    col = mix(col, vec3(0.0), spire);

    gl_FragColor = vec4(col, 1.0);
}
