/*{
    "DESCRIPTION": "Fauve Expressionism — standalone 2D procedural impasto brush-stroke canvas. Thick ridge strokes, Fauvist palette (cobalt, vermilion, cadmium yellow, viridian). HDR specular on paint ridges.",
    "CATEGORIES": ["Generator", "Painterly", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "strokeScale",  "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 10.0, "LABEL": "Stroke Scale" },
        { "NAME": "warpStrength", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0,  "LABEL": "Warp Strength" },
        { "NAME": "hdrPeak",      "TYPE": "float", "DEFAULT": 2.2, "MIN": 1.0, "MAX": 4.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",     "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p); vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21(i), hash21(i+vec2(1,0)), u.x),
               mix(hash21(i+vec2(0,1)), hash21(i+vec2(1,1)), u.x), u.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int k = 0; k < 5; k++) { v += a * vnoise(p); p *= 2.1; a *= 0.5; }
    return v;
}

// Fauvist palette — 4 fully saturated hues, no white-mixing
vec3 fauveColor(float t) {
    t = fract(t);
    if (t < 0.25) return mix(vec3(0.04, 0.22, 0.9),  vec3(0.0,  0.78, 0.1),  t * 4.0);
    if (t < 0.50) return mix(vec3(0.0,  0.78, 0.1),  vec3(0.95, 0.12, 0.03), (t-0.25)*4.0);
    if (t < 0.75) return mix(vec3(0.95, 0.12, 0.03), vec3(1.0,  0.72, 0.0),  (t-0.50)*4.0);
    return             mix(vec3(1.0,  0.72, 0.0),  vec3(0.04, 0.22, 0.9),  (t-0.75)*4.0);
}

void main() {
    float asp = RENDERSIZE.x / RENDERSIZE.y;
    vec2 st = isf_FragNormCoord * vec2(asp, 1.0) * strokeScale;

    float t = TIME * 0.07;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.4;

    // Animated domain warp drives stroke direction
    vec2 warp = vec2(fbm(st + vec2(t*0.7, t*0.3)),
                     fbm(st + vec2(t*0.4, t*0.9) + 3.7));
    vec2 sw = st + warp * warpStrength * 2.0;

    // Stroke direction angle from FBM
    float angle = fbm(sw * 0.5 + t) * 6.2832;
    vec2 dir    = vec2(cos(angle), sin(angle));
    vec2 perp   = vec2(-dir.y, dir.x);

    float sAlong = dot(st, dir)  * 1.5;
    float sAcross= dot(st, perp) * 2.0 + fbm(sw) * 0.7;

    // Color index from large-scale FBM
    float ci = fbm(sw * 0.28 + t * 0.12) + fbm(sw * 0.15 - t * 0.09) * 0.6;
    vec3 baseCol = fauveColor(ci);

    // Impasto ridge profile
    float h = sin(sAlong * 12.566 + sAcross * 6.28) * 0.5 + 0.5;
    h = h * h;

    // Finite-difference normal from height field
    float eps = 0.004;
    float hL = pow(sin((sAlong-eps)*12.566 + sAcross*6.28)*0.5+0.5, 2.0);
    float hR = pow(sin((sAlong+eps)*12.566 + sAcross*6.28)*0.5+0.5, 2.0);
    float hD = pow(sin(sAlong*12.566 + (sAcross-eps)*6.28)*0.5+0.5, 2.0);
    float hU = pow(sin(sAlong*12.566 + (sAcross+eps)*6.28)*0.5+0.5, 2.0);
    vec3 N = normalize(vec3((hL-hR)/(2.0*eps), (hD-hU)/(2.0*eps), 0.25));

    // Warm directional key light
    vec3 key = normalize(vec3(0.55, 0.75, 0.5));
    float diff = max(dot(N, key), 0.0);
    float spec = pow(max(dot(reflect(-key, N), vec3(0.0,0.0,1.0)), 0.0), 20.0);

    // Black ink in deep stroke troughs
    float ink = smoothstep(0.12, 0.0, h);

    vec3 col = baseCol * (0.12 + diff * 0.88) * hdrPeak * audio;
    col += vec3(1.0) * spec * hdrPeak * 0.8;       // HDR specular ridge
    col  = mix(vec3(0.0, 0.0, 0.01), col, 1.0 - ink * 0.85); // ink troughs

    gl_FragColor = vec4(col, 1.0);
}
