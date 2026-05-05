/*{
    "DESCRIPTION": "Voronoi Lightning — animated 2D Voronoi cell edges with neon plasma glow. Black void interiors, HDR neon edge halos.",
    "CATEGORIES": ["Generator", "Particles"],
    "CREDIT": "ShaderClaw auto-improve v14",
    "INPUTS": [
        { "NAME": "cellCount",  "LABEL": "Cell Count",  "TYPE": "float", "DEFAULT": 10.0, "MIN": 3.0,  "MAX": 25.0 },
        { "NAME": "edgeGlow",   "LABEL": "Edge Glow",   "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5,  "MAX": 4.0 },
        { "NAME": "speed",      "LABEL": "Speed",       "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 2.0 },
        { "NAME": "hdrBoost",   "LABEL": "HDR Boost",   "TYPE": "float", "DEFAULT": 2.0,  "MIN": 1.0,  "MAX": 4.0 },
        { "NAME": "audioReactivity", "LABEL": "Audio",  "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0 }
    ]
}*/

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

vec2 hash22(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
    return fract(sin(p) * 43758.5453);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME * speed;
    float audio = 1.0 + (audioLevel + audioBass * 0.5) * audioReactivity;

    vec2 p = vec2(uv.x * aspect, uv.y) * cellCount;

    float d1 = 1e10, d2 = 1e10;
    float nearHue = 0.0, nearHue2 = 0.0;

    vec2 pi = floor(p);
    vec2 pf = fract(p);

    // Find 2 nearest Voronoi cell centers
    for (int y = -2; y <= 2; y++) {
        for (int x = -2; x <= 2; x++) {
            vec2 nb = vec2(float(x), float(y));
            vec2 cell = pi + nb;
            vec2 seed = hash22(cell);
            vec2 center = nb + seed + 0.45 * sin(t * (0.4 + seed) * 6.28318);
            float d = length(center - pf);
            float hue = hash11(cell.x * 3.7 + cell.y * 7.3);
            if (d < d1) { d2 = d1; nearHue2 = nearHue; d1 = d; nearHue = hue; }
            else if (d < d2) { d2 = d; nearHue2 = hue; }
        }
    }

    // Edge distance = d2 - d1
    float edgeDist = d2 - d1;
    float fw = fwidth(edgeDist);

    // Glow: exponential falloff from edge
    float glow = exp(-edgeDist * 28.0) * edgeGlow * audio;
    float core = smoothstep(fw * 2.0, 0.0, edgeDist);

    // Per-cell saturated neon color
    vec3 edgeCol   = hsv2rgb(vec3(nearHue,  1.0, 1.0));
    vec3 edgeCol2  = hsv2rgb(vec3(nearHue2, 1.0, 1.0));
    vec3 glowColor = mix(edgeCol, edgeCol2, 0.5);

    // Black void + neon edge accumulation
    vec3 col = vec3(0.005, 0.0, 0.01);
    col += glowColor * glow * 0.5;
    col += edgeCol  * core * edgeGlow * hdrBoost;
    // White-hot core at exact edge
    col += vec3(1.0) * core * core * hdrBoost * 0.8;

    gl_FragColor = vec4(col, 1.0);
}
