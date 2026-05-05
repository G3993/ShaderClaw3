/*{
  "DESCRIPTION": "Abstract Expressionist Studio — raymarched cluster of organic SDFs with toon/painterly stepped lighting, black ink outlines. Palette: cadmium red, ultramarine, ochre, ivory black.",
  "CATEGORIES": ["Generator", "3D"],
  "CREDIT": "auto-improve 2026-05-05",
  "INPUTS": [
    { "NAME": "rotSpeed",    "LABEL": "Rotation",     "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "inkWidth",    "LABEL": "Ink Lines",    "TYPE": "float", "DEFAULT": 1.5,  "MIN": 0.0,  "MAX": 4.0 },
    { "NAME": "toonSteps",   "LABEL": "Toon Steps",   "TYPE": "float", "DEFAULT": 4.0,  "MIN": 2.0,  "MAX": 8.0 },
    { "NAME": "hdrPeak",     "LABEL": "Brightness",   "TYPE": "float", "DEFAULT": 2.2,  "MIN": 0.5,  "MAX": 4.0 },
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define SURF_DIST 0.003
#define MAX_DIST  10.0
#define TAU       6.28318530718

mat2 rot2(float a) { float c=cos(a),s=sin(a); return mat2(c,-s,s,c); }

vec3 PAL_RED   = vec3(0.87, 0.12, 0.05);
vec3 PAL_BLUE  = vec3(0.08, 0.18, 0.78);
vec3 PAL_OCHRE = vec3(0.92, 0.68, 0.12);
vec3 PAL_WHITE = vec3(0.96, 0.94, 0.88);

float smin(float a, float b, float k) {
    float h = max(k - abs(a-b), 0.0) / k;
    return min(a,b) - h*h*k*0.25;
}

struct Hit { float d; int id; };

Hit map(vec3 p) {
    float t  = TIME * rotSpeed;
    float pulse = 1.0 + audioBass * audioReact * 0.1;

    p.xz = rot2(t * 0.7) * p.xz;
    p.xy = rot2(t * 0.4) * p.xy;

    vec2 q1 = vec2(length(p.xz) - 0.7, p.y);
    float torus = length(q1) - 0.28 * pulse;

    float sphere1 = length(p - vec3(0.0, 0.0, 0.0)) - 0.5 * pulse;
    float sphere2 = length(p - vec3(0.6, 0.4, 0.1)) - 0.22 * pulse;
    float sphere3 = length(p - vec3(-0.5, -0.35, 0.2)) - 0.18 * pulse;

    float base = smin(torus, sphere1, 0.4);
    float full = smin(smin(base, sphere2, 0.3), sphere3, 0.25);

    int id = 0;
    if (abs(torus - full) < 0.05) id = 0;
    else if (abs(sphere1 - full) < 0.05) id = 1;
    else if (abs(sphere2 - full) < 0.05) id = 2;
    else id = 3;

    return Hit(full, id);
}

float mapD(vec3 p) { return map(p).d; }

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        mapD(p+e.xyy)-mapD(p-e.xyy),
        mapD(p+e.yxy)-mapD(p-e.yxy),
        mapD(p+e.yyx)-mapD(p-e.yyx)));
}

float toon(float x) {
    float s = toonSteps;
    return floor(x * s + 0.5) / s;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    vec3 ro = vec3(0.0, 0.0, 3.0);
    vec3 rd = normalize(vec3(uv, -2.2));

    float d = 0.0;
    int   hitId = -1;
    for (int i = 0; i < MAX_STEPS; i++) {
        Hit h = map(ro + rd * d);
        if (h.d < SURF_DIST) { hitId = h.id; break; }
        if (d > MAX_DIST) break;
        d += h.d;
    }

    vec3 canvas = vec3(0.95, 0.92, 0.84);
    vec3 col    = canvas;

    if (hitId >= 0) {
        vec3 p  = ro + rd * d;
        vec3 n  = calcNormal(p);

        vec3 L1 = normalize(vec3(0.5, 1.8, 1.5));
        vec3 L2 = normalize(vec3(-1.5,-0.5, 1.0));

        float diff1 = max(dot(n, L1), 0.0);
        float diff2 = max(dot(n, L2), 0.0) * 0.35;
        float diffT = toon(diff1) + diff2;

        vec3  refl  = reflect(rd, n);
        float spec  = pow(max(dot(refl, L1), 0.0), 12.0) * 0.4;
        float specT = toon(spec);

        vec3 matCol;
        if      (hitId == 0) matCol = PAL_RED;
        else if (hitId == 1) matCol = PAL_BLUE;
        else if (hitId == 2) matCol = PAL_OCHRE;
        else                 matCol = PAL_WHITE;

        col = matCol * diffT + PAL_WHITE * specT * 1.5;

        float fw  = fwidth(d) * inkWidth;
        float edge = 1.0 - smoothstep(SURF_DIST, SURF_DIST + fw * 0.04, abs(dot(n, -rd)));
        col = mix(col, vec3(0.04, 0.02, 0.01), edge);

        col *= hdrPeak;
    }

    float vign = 1.0 - dot(uv * 0.55, uv * 0.55);
    col = mix(col, canvas, 1.0 - clamp(vign * 1.2, 0.0, 1.0));

    gl_FragColor = vec4(col, 1.0);
}
