/*{
  "DESCRIPTION": "RGB Prism Vortex — rotating triangular prism array splits white light into pure R/G/B channels. HDR hot-white where channels realign. Black void background.",
  "CATEGORIES": ["Generator", "3D"],
  "CREDIT": "auto-improve 2026-05-05",
  "INPUTS": [
    { "NAME": "prismCount",    "LABEL": "Prism Count",    "TYPE": "float", "DEFAULT": 5.0,  "MIN": 2.0,  "MAX": 12.0 },
    { "NAME": "rotSpeed",      "LABEL": "Rotation Speed", "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "dispersion",    "LABEL": "Dispersion",     "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0,  "MAX": 0.25 },
    { "NAME": "edgeSharp",     "LABEL": "Edge Sharpness", "TYPE": "float", "DEFAULT": 80.0, "MIN": 10.0, "MAX": 200.0 },
    { "NAME": "hdrPeak",       "LABEL": "Brightness",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5,  "MAX": 4.0 },
    { "NAME": "audioReact",    "LABEL": "Audio React",    "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define SURF_DIST 0.003
#define MAX_DIST  10.0
#define TAU       6.28318530718

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }
mat2  rot2(float a)   { float c=cos(a),s=sin(a); return mat2(c,-s,s,c); }

float sdTriangle(vec2 p, float r) {
    const float k = sqrt(3.0);
    p.x = abs(p.x) - r;
    p.y = p.y + r / k;
    if (p.x + k * p.y > 0.0) p = vec2(p.x - k*p.y, -k*p.x - p.y) * 0.5;
    p.x -= clamp(p.x, -2.0*r, 0.0);
    return -length(p) * sign(p.y);
}

float sdTriPrism(vec3 p, float r) {
    vec2 q = abs(p.yz);
    float h = 3.0;
    float d = sdTriangle(p.xy, r);
    d = max(d, abs(p.z) - h);
    return d;
}

float map(vec3 p, float rotOff) {
    float t = TIME * rotSpeed + rotOff;
    p.xy = rot2(t) * p.xy;

    float N    = prismCount;
    float best = MAX_DIST;
    float ang  = TAU / N;

    for (int i = 0; i < 12; i++) {
        if (float(i) >= N) break;
        float a = ang * float(i);
        float radius = 1.4;
        vec3 pp = p - vec3(cos(a)*radius, sin(a)*radius, 0.0);
        float r = 0.18 * (1.0 + audioBass * audioReact * 0.15);
        best = min(best, sdTriPrism(pp, r));
    }
    return best;
}

vec3 calcNormal(vec3 p, float rotOff) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p+e.xyy, rotOff)-map(p-e.xyy, rotOff),
        map(p+e.yxy, rotOff)-map(p-e.yxy, rotOff),
        map(p+e.yyx, rotOff)-map(p-e.yyx, rotOff)));
}

float traceChannel(vec3 ro, vec3 rd, float rotOff) {
    float d = 0.0;
    for (int i = 0; i < MAX_STEPS; i++) {
        float h = map(ro + rd * d, rotOff);
        if (h < SURF_DIST) return d;
        if (d > MAX_DIST)  return -1.0;
        d += h * 0.9;
    }
    return -1.0;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    vec3 ro = vec3(0.0, 0.0, 4.0);
    vec3 rd = normalize(vec3(uv, -2.5));

    float disp = dispersion * (1.0 + audioHigh * audioReact * 0.5);
    float angR =  disp;
    float angG =  0.0;
    float angB = -disp;

    float dR = traceChannel(ro, rd, angR);
    float dG = traceChannel(ro, rd, angG);
    float dB = traceChannel(ro, rd, angB);

    vec3 col = vec3(0.0);

    if (dR > 0.0) {
        vec3 p  = ro + rd * dR;
        vec3 n  = calcNormal(p, angR);
        float L = max(dot(n, normalize(vec3(1.0, 1.5, 2.0))), 0.0);
        float fw = fwidth(dR);
        float aa = smoothstep(SURF_DIST + fw, SURF_DIST, map(p, angR));
        col.r += (L * 1.6 + 0.3) * aa;
    }
    if (dG > 0.0) {
        vec3 p  = ro + rd * dG;
        vec3 n  = calcNormal(p, angG);
        float L = max(dot(n, normalize(vec3(1.0, 1.5, 2.0))), 0.0);
        float fw = fwidth(dG);
        float aa = smoothstep(SURF_DIST + fw, SURF_DIST, map(p, angG));
        col.g += (L * 1.6 + 0.3) * aa;
    }
    if (dB > 0.0) {
        vec3 p  = ro + rd * dB;
        vec3 n  = calcNormal(p, angB);
        float L = max(dot(n, normalize(vec3(1.0, 1.5, 2.0))), 0.0);
        float fw = fwidth(dB);
        float aa = smoothstep(SURF_DIST + fw, SURF_DIST, map(p, angB));
        col.b += (L * 1.6 + 0.3) * aa;
    }

    float alignment = min(col.r, min(col.g, col.b));
    col += vec3(alignment) * 1.5;

    col *= hdrPeak;

    gl_FragColor = vec4(col, 1.0);
}
