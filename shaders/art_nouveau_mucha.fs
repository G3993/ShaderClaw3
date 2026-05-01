/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Art Nouveau after Mucha + 1960s San Francisco psychedelic poster art — ornate flowing frame, blooming concentric flowers at the center, sinuous whiplash tendrils, hippy palette (hot pink, lavender, sunflower, peacock teal, aubergine, gold). Single pass; animated breath; the frame opens and closes with bass.",
  "INPUTS": [
    { "NAME": "petalCount",     "LABEL": "Petal Count",     "TYPE":"float","MIN":3.0, "MAX":24.0, "DEFAULT":12.0 },
    { "NAME": "flowerLayers",   "LABEL": "Flower Layers",   "TYPE":"float","MIN":1.0, "MAX":5.0,  "DEFAULT":3.0 },
    { "NAME": "tendrilCount",   "LABEL": "Tendrils",        "TYPE":"float","MIN":0.0, "MAX":12.0, "DEFAULT":6.0 },
    { "NAME": "tendrilWidth",   "LABEL": "Tendril Width",   "TYPE":"float","MIN":0.001,"MAX":0.015,"DEFAULT":0.005 },
    { "NAME": "frameStrength",  "LABEL": "Frame Strength",  "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.85 },
    { "NAME": "frameWidth",     "LABEL": "Frame Width",     "TYPE":"float","MIN":0.02,"MAX":0.18, "DEFAULT":0.07 },
    { "NAME": "rotateSpeed",    "LABEL": "Rotation Speed",  "TYPE":"float","MIN":0.0, "MAX":1.5,  "DEFAULT":0.18 },
    { "NAME": "breathe",        "LABEL": "Breathe",         "TYPE":"float","MIN":0.0, "MAX":0.30, "DEFAULT":0.08 },
    { "NAME": "goldStrength",   "LABEL": "Gold",            "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.55 },
    { "NAME": "paletteShift",   "LABEL": "Palette Shift",   "TYPE":"float","MIN":0.0, "MAX":1.0,  "DEFAULT":0.0 },
    { "NAME": "audioReact",     "LABEL": "Audio React",     "TYPE":"float","MIN":0.0, "MAX":2.0,  "DEFAULT":1.0 },
    { "NAME": "inputTex",       "LABEL": "Texture",         "TYPE":"image" }
  ]
}*/

// Hippy / Mucha palette — strong saturated tones with cream paper +
// bronze gold for contour lines.
const vec3 PALE_CREAM   = vec3(0.96, 0.92, 0.78);
const vec3 HOT_PINK     = vec3(0.96, 0.30, 0.60);
const vec3 LAVENDER     = vec3(0.72, 0.55, 0.92);
const vec3 SUNFLOWER    = vec3(0.99, 0.78, 0.18);
const vec3 PEACOCK      = vec3(0.10, 0.55, 0.65);
const vec3 AUBERGINE    = vec3(0.32, 0.10, 0.40);
const vec3 BURNT_ORANGE = vec3(0.92, 0.45, 0.18);
const vec3 SAGE         = vec3(0.40, 0.65, 0.40);
const vec3 GOLD         = vec3(0.95, 0.78, 0.30);
const vec3 CONTOUR      = vec3(0.18, 0.10, 0.08);

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

vec3 hippyColor(float idx) {
    int i = int(mod(idx + paletteShift * 7.0, 7.0));
    if (i == 0) return HOT_PINK;
    if (i == 1) return LAVENDER;
    if (i == 2) return SUNFLOWER;
    if (i == 3) return PEACOCK;
    if (i == 4) return AUBERGINE;
    if (i == 5) return BURNT_ORANGE;
    return SAGE;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec2 cuv = (uv - 0.5) * vec2(aspect, 1.0);
    float r  = length(cuv);
    float th = atan(cuv.y, cuv.x);
    float t = TIME * rotateSpeed;

    // Background — cream with subtle rose-tint vignette
    vec3 col = mix(PALE_CREAM, mix(PALE_CREAM, HOT_PINK, 0.20), smoothstep(0.0, 0.7, r));

    // ── Concentric flower layers ──────────────────────────────────────
    int L = int(clamp(flowerLayers, 1.0, 5.0));
    for (int li = 0; li < 5; li++) {
        if (li >= L) break;
        float fl = float(li);
        float layerR = 0.10 + fl * 0.10;                // base radius
        float layerRot = t * (0.4 + fl * 0.3) * (li % 2 == 0 ? 1.0 : -1.0);
        // Breath: layers expand/contract with bass
        float breath = 1.0 + sin(TIME * 0.5 + fl) * breathe + audioBass * audioReact * 0.05;
        layerR *= breath;
        // Petal pattern: radius modulated by cos(petalCount * theta)
        float pcount = petalCount * (li % 2 == 0 ? 1.0 : 0.7);
        float petalR = layerR * (1.0 + 0.35 * cos(pcount * (th + layerRot)));
        // Filled petal ring
        float ring = smoothstep(0.008, 0.002, abs(r - petalR));
        // Petal fill — color by layer
        vec3 petalCol = hippyColor(fl + floor(t * 0.25));
        col = mix(col, petalCol, smoothstep(petalR + 0.005, petalR - 0.015, r) * 0.65);
        // Bright contour
        col = mix(col, CONTOUR, ring * 0.7);
    }

    // ── Center stamen — golden disc with hatched lines ────────────────
    {
        float disc = smoothstep(0.045, 0.040, r);
        col = mix(col, GOLD * (1.0 + 0.4 * sin(TIME * 1.0)), disc * 0.85);
        // Stamen radial spikes
        float spikes = step(0.85, abs(sin(th * 18.0))) * step(r, 0.07) * step(0.04, r);
        col = mix(col, CONTOUR, spikes * 0.5);
    }

    // ── Whiplash tendrils — sinuous curves flowing across the canvas ──
    int T = int(clamp(tendrilCount, 0.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= T) break;
        float fi = float(i);
        // Each tendril is a parametric curve. Sample a fixed number of
        // points along it and accumulate to the closest distance.
        float phase = TIME * 0.10 + fi * 1.7;
        float minDist = 1e9;
        for (int k = 0; k < 12; k++) {
            float fk = float(k) / 11.0;
            // Ribbon path: gentle sin curve from edge to edge
            float u = fk;
            vec2 P = vec2(mix(-aspect * 0.5, aspect * 0.5, u),
                          0.40 * sin(u * 4.0 + phase + fi * 0.7) +
                          0.20 * sin(u * 7.5 + phase * 1.3 + fi));
            float d = length(cuv - P);
            minDist = min(minDist, d);
        }
        float ribbon = smoothstep(tendrilWidth, 0.0, minDist);
        vec3 tendrilCol = hippyColor(fi + 1.0);
        col = mix(col, tendrilCol, ribbon * 0.85);
        // Gold edge
        float ribbonEdge = smoothstep(tendrilWidth * 1.6, tendrilWidth, minDist) - ribbon;
        col = mix(col, GOLD, ribbonEdge * goldStrength);
    }

    // ── Ornate frame at the canvas edges ──────────────────────────────
    if (frameStrength > 0.001) {
        // Distance from canvas edge
        vec2 edgeD = min(uv, 1.0 - uv);
        float edge = min(edgeD.x, edgeD.y);
        // Ornate scallops along the edge
        float scallop = sin(uv.x * 28.0 + sin(TIME * 0.3) * 0.5) * 0.5 + 0.5;
        scallop *= sin(uv.y * 28.0) * 0.5 + 0.5;
        float frameW = frameWidth * (1.0 + audioMid * audioReact * 0.10);
        float frameInside = smoothstep(frameW, frameW * 0.7, edge);
        // Gold frame band
        col = mix(col, GOLD, frameInside * frameStrength * (0.7 + 0.3 * scallop));
        // Inner contour line of the frame
        float inner = smoothstep(0.003, 0.0, abs(edge - frameW * 0.6));
        col = mix(col, CONTOUR, inner * frameStrength);
        // Corner rosettes
        for (int c = 0; c < 4; c++) {
            float fc = float(c);
            vec2 corner = vec2(mod(fc, 2.0), floor(fc / 2.0));
            float dC = length(uv - corner);
            float rose = smoothstep(0.05, 0.04, dC) * (0.5 + 0.5 * sin(th * 8.0));
            col = mix(col, GOLD, rose * frameStrength * 0.85);
            float roseRing = smoothstep(0.005, 0.0, abs(dC - 0.05));
            col = mix(col, CONTOUR, roseRing * frameStrength);
        }
    }

    // Optional input texture — bleeds into the central flower area
    if (IMG_SIZE_inputTex.x > 0.0) {
        float diskMask = smoothstep(0.40, 0.20, r);
        vec3 src = texture(inputTex, uv).rgb;
        col = mix(col, src, diskMask * 0.30);
    }

    // Surprise: every ~28s a luminous moth wing silhouette flutters
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 28.0);
        float _f  = smoothstep(0.0, 0.06, _ph) * smoothstep(0.30, 0.18, _ph);
        vec2 _p = (_suv - vec2(0.5, 0.32 + 0.06 * sin(TIME * 0.7)));
        _p.x = abs(_p.x);
        float _wing = smoothstep(0.18, 0.0, length(_p - vec2(0.10, 0.0)) + 0.06 * sin(_p.y * 24.0 + TIME));
        col = mix(col, vec3(1.00, 0.92, 0.62), _f * _wing * 0.55);
    }

    // Audio breath
    col *= 0.92 + audioLevel * audioReact * 0.10;

    gl_FragColor = vec4(col, 1.0);
}
