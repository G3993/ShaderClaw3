/*{
    "DESCRIPTION": "Recursive black hole — fractal tiled glow rings with cosine palette, depth iterations, and audio reactivity",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        { "NAME": "iterations", "LABEL": "Depth", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 8.0 },
        { "NAME": "ringScale", "LABEL": "Ring Scale", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.5, "MAX": 4.0 },
        { "NAME": "ringSpeed", "LABEL": "Ring Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "glowIntensity", "LABEL": "Glow", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.005, "MAX": 0.1 },
        { "NAME": "hueShift", "LABEL": "Hue Shift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "colorSpeed", "LABEL": "Color Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "zoom", "LABEL": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 5.0 },
        { "NAME": "warp", "LABEL": "Warp", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "palette", "LABEL": "Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Neon","Fire","Ice","Acid"] }
    ]
}*/

// Cosine palette — art-grade color from distance + time
// color = a + b * cos(2π * (c * t + d))
vec3 cosPalette(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    return a + b * cos(6.28318 * (c * t + d));
}

vec3 getPalette(float t, int pal) {
    // Neon (default) — cyan/magenta/yellow
    if (pal == 1) {
        // Fire — red/orange/gold
        return cosPalette(t,
            vec3(0.5, 0.3, 0.1),
            vec3(0.5, 0.4, 0.2),
            vec3(1.0, 0.7, 0.4),
            vec3(0.0, 0.15, 0.20));
    }
    if (pal == 2) {
        // Ice — blue/white/cyan
        return cosPalette(t,
            vec3(0.4, 0.5, 0.7),
            vec3(0.3, 0.3, 0.3),
            vec3(1.0, 1.0, 1.0),
            vec3(0.0, 0.1, 0.2));
    }
    if (pal == 3) {
        // Acid — green/yellow/purple
        return cosPalette(t,
            vec3(0.5, 0.5, 0.2),
            vec3(0.5, 0.5, 0.5),
            vec3(1.0, 1.0, 0.5),
            vec3(0.8, 0.9, 0.3));
    }
    // Neon
    return cosPalette(t,
        vec3(0.5, 0.5, 0.5),
        vec3(0.5, 0.5, 0.5),
        vec3(1.0, 1.0, 1.0),
        vec3(0.263, 0.416, 0.557));
}

void main() {
    // Coordinate normalization — centered, aspect-corrected
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 uv0 = uv; // save original for color indexing
    uv *= zoom;

    // Mouse influence — warp UV toward/away from mouse
    vec2 mp = (mousePos - 0.5) * 2.0;
    mp.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME * ringSpeed;
    int iters = int(iterations);
    int pal = int(palette);

    vec3 finalColor = vec3(0.0);

    // ── Recursive iteration — each pass adds layered depth ──
    for (int i = 0; i < 8; i++) {
        if (i >= iters) break;
        float fi = float(i);

        // Fractal tiling — fract creates infinite grid, -0.5 centers each cell
        uv = fract(uv * ringScale) - 0.5;

        // Mouse warp — subtle push per iteration
        uv += mp * warp * 0.05 / (fi + 1.0);

        // Distance from cell center — the circle shape
        float d = length(uv);

        // "Black hole" effect: add time to distance inside sin()
        // Makes rings appear to be sucked into the center
        d = sin(d * 8.0 + t + fi * 0.5) / 8.0;
        d = abs(d);

        // Audio reactivity — bass pulses the ring width, mid shifts phase
        d += audioBass * 0.02;
        float audioPhase = audioMid * 0.3;

        // Cosine palette color — driven by original UV distance + time + iteration
        float colorIndex = length(uv0) + fi * 0.4 + TIME * colorSpeed * 0.2 + hueShift + audioPhase;
        vec3 col = getPalette(colorIndex, pal);

        // Glow effect — small value / distance = bright neon glow
        // Closer to ring = brighter, falls off with distance
        float glow = glowIntensity / d;

        // Audio boost to glow
        glow *= 1.0 + audioLevel * 2.0;

        // Pow for sharper, more defined rings (less muddy)
        glow = pow(glow, 1.2);

        // Accumulate — each iteration adds its own colored glow layer
        finalColor += col * glow;
    }

    // Tone mapping — prevent blowout while keeping the neon punch
    finalColor = finalColor / (1.0 + finalColor);

    // Surprise: every ~7s a full-canvas white flash — the strike.
    // Brief and unmistakable.
    {
        float _ph = fract(TIME / 7.0);
        float _flash = smoothstep(0.0, 0.005, _ph) * smoothstep(0.08, 0.04, _ph);
        finalColor = mix(finalColor, vec3(1.0), _flash * 0.95);
    }

    gl_FragColor = vec4(finalColor, 1.0);
}
