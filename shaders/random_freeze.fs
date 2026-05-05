/*{
  "DESCRIPTION": "Fractal Ice Mandala — 6-fold kaleidoscopic domain-folding with saturated polar palette. Standalone generator.",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "Abstract", "Fractal"],
  "INPUTS": [
    { "NAME": "rotSpeed",   "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0,  "MAX": 1.0,  "LABEL": "Rotation Speed" },
    { "NAME": "zoomPulse",  "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0,  "MAX": 1.0,  "LABEL": "Zoom Pulse"     },
    { "NAME": "foldIter",   "TYPE": "float", "DEFAULT": 5.0,  "MIN": 2.0,  "MAX": 8.0,  "LABEL": "Fold Iterations"},
    { "NAME": "hdrPeak",    "TYPE": "float", "DEFAULT": 2.7,  "MIN": 1.0,  "MAX": 4.0,  "LABEL": "HDR Peak"       },
    { "NAME": "colorCycle", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0,  "MAX": 0.5,  "LABEL": "Color Cycle"    },
    { "NAME": "audioReact", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0,  "LABEL": "Audio React"    }
  ]
}*/

// 4-color saturated cold palette: midnight navy / glacier blue / ice cyan / aurora violet
vec3 icePalette(float t) {
    t = fract(t);
    const vec3 c0 = vec3(0.02, 0.04, 0.25);  // midnight navy
    const vec3 c1 = vec3(0.0,  0.45, 1.0);   // glacier blue
    const vec3 c2 = vec3(0.0,  1.0,  0.95);  // ice cyan
    const vec3 c3 = vec3(0.65, 0.0,  1.0);   // aurora violet
    float s = t * 4.0;
    int i = int(s); float f = fract(s);
    if (i == 0) return mix(c0, c1, f);
    if (i == 1) return mix(c1, c2, f);
    if (i == 2) return mix(c2, c3, f);
    return mix(c3, c0, f);
}

// Fold a 2D point into a fundamental sector of an N-fold symmetric pattern
vec2 fold6(vec2 p) {
    const float PI = 3.14159265;
    // 6-fold: reflect into [0, PI/3] angular wedge
    float angle = atan(p.y, p.x);
    // Map angle to [0, 2π], snap to nearest sixth
    angle = mod(angle, PI / 3.0);
    if (angle > PI / 6.0) angle = PI / 3.0 - angle;
    float r = length(p);
    return vec2(cos(angle) * r, sin(angle) * r);
}

// 2D value noise
float hash2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5); }
float noise2(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f*f*(3.0-2.0*f);
    return mix(mix(hash2(i), hash2(i+vec2(1,0)), f.x),
               mix(hash2(i+vec2(0,1)), hash2(i+vec2(1,1)), f.x), f.y);
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x   *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel * 0.6 + audioBass * 0.4) * audioReact;
    float t = TIME;

    // Slow global rotation
    float ca = cos(t * rotSpeed), sa = sin(t * rotSpeed);
    uv = vec2(ca*uv.x - sa*uv.y, sa*uv.x + ca*uv.y);

    // Zoom pulse (audio-reactive)
    float zoom = 1.0 + sin(t * zoomPulse * 2.0) * 0.12 * audio;
    uv *= zoom;

    // 6-fold domain fold
    vec2 p = fold6(uv);

    // Iterative domain folding (like Mandelbox / kaleidoscope)
    float orbit = 0.0;
    int N = int(clamp(foldIter, 2.0, 8.0));
    float scale = 1.8;
    for (int i = 0; i < 8; i++) {
        if (i >= N) break;
        // Translate and scale (folded coordinates)
        p = abs(p) - vec2(0.35, 0.22);
        p = fold6(p);
        orbit += length(p) * pow(0.55, float(i));
        p *= scale;
        p = fold6(p);
    }

    // Color from orbit value + time cycle
    float colorT = orbit * 0.35 + t * colorCycle;
    vec3  col    = icePalette(colorT);

    // HDR: inner orbit values (small orbit) = brightest (like ice core)
    float brightness = exp(-orbit * 0.9) * 3.0 + smoothstep(1.5, 0.0, orbit);
    col *= brightness * hdrPeak * audio;

    // Black ink: rapid orbit drop → sharp edge (crystal facet boundary)
    float edgeOrbit = orbit;
    float aa   = fwidth(edgeOrbit);
    float edge = 1.0 - smoothstep(aa, aa * 4.0, mod(edgeOrbit, 0.18));
    col = mix(col, vec3(0.0), edge * 0.85);

    // White-hot crystal center (orbit near zero)
    float coreGlow = exp(-orbit * 3.5) * hdrPeak * 1.2 * audio;
    col += vec3(0.85, 0.95, 1.0) * coreGlow; // cool white HDR core

    gl_FragColor = vec4(col, 1.0);
}
