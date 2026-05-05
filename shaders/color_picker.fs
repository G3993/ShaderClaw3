/*{
  "DESCRIPTION": "Prismatic Refractions — a faceted crystal sphere splitting studio light into saturated spectral beams. Standalone 3D raymarched generator.",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "rotSpeed",  "LABEL": "Rotation Speed", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "facets",    "LABEL": "Facets",          "TYPE": "float", "DEFAULT": 6.0,  "MIN": 2.0, "MAX": 12.0 },
    { "NAME": "dispersion","LABEL": "Dispersion",      "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 0.3 },
    { "NAME": "hdrPeak",   "LABEL": "HDR Peak",        "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioMod",  "LABEL": "Audio Mod",       "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define SURF_DIST 0.002
#define MAX_DIST  8.0
#define PI 3.14159265

float hash(float n) { return fract(sin(n * 127.1) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Gyroid-modulated sphere SDF for faceted crystal look
float sdCrystal(vec3 p, float r, float fac) {
    float sphere = length(p) - r;
    float k = fac;
    float gyroid = dot(sin(p * k), cos(p.yzx * k)) / k;
    return sphere - gyroid * 0.04;
}

float map(vec3 p) {
    return sdCrystal(p, 0.72, facets);
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p + e.xyy) - map(p - e.xyy),
        map(p + e.yxy) - map(p - e.yxy),
        map(p + e.yyx) - map(p - e.yyx)
    ));
}

float march(vec3 ro, vec3 rd) {
    float t = 0.1;
    for (int i = 0; i < MAX_STEPS; i++) {
        float d = map(ro + rd * t);
        if (d < SURF_DIST) return t;
        if (t > MAX_DIST) return -1.0;
        t += d * 0.7;
    }
    return -1.0;
}

// Spectral beam: chromatic dispersion refraction
vec3 spectralRefract(vec3 rd, vec3 n, float ior, float wave) {
    float iorW = ior + dispersion * (wave - 0.5);
    return refract(rd, n, 1.0 / iorW);
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.4;
    float t = TIME * rotSpeed;

    // Orbiting camera
    vec3 ro = vec3(sin(t) * 2.2, 0.35 + sin(t * 0.37) * 0.2, cos(t) * 2.2);
    vec3 target = vec3(0.0);
    vec3 fwd = normalize(target - ro);
    vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up  = cross(fwd, rgt);
    vec3 rd  = normalize(fwd + uv.x * rgt + uv.y * up);

    // Studio key light (warm) + fill (cool)
    vec3 keyLight = normalize(vec3(1.2, 1.6, 0.8));
    vec3 fillLight = normalize(vec3(-0.8, 0.4, -1.0));

    // Background: dark studio void with subtle radial glow
    float bgGlow = 1.0 - smoothstep(0.0, 1.8, length(uv));
    vec3 col = vec3(0.01, 0.005, 0.02) + vec3(0.04, 0.02, 0.08) * bgGlow;

    float tHit = march(ro, rd);

    if (tHit > 0.0) {
        vec3 p = ro + rd * tHit;
        vec3 n = calcNormal(p);

        // Chromatic dispersion: march separate R/G/B refracted rays
        vec3 spectral = vec3(0.0);
        for (int ci = 0; ci < 5; ci++) {
            float wave = float(ci) / 4.0;
            vec3 refRd = spectralRefract(rd, n, 1.45 + 0.1 * sin(t * 0.7 + float(ci)), wave);
            float exitT = march(p + refRd * 0.05, refRd);
            vec3 exitP = p + refRd * (exitT > 0.0 ? exitT : 0.5);
            vec3 exitN = calcNormal(exitP);
            vec3 exitRd = refract(refRd, -exitN, 1.45);
            if (length(exitRd) < 0.001) exitRd = reflect(refRd, -exitN);

            // Spectral hue: violet→cyan→gold→ruby
            float hue = 0.75 - wave * 0.6;
            vec3 beamCol = hsv2rgb(vec3(hue, 1.0, 1.0));

            // Light entering via exit beam direction dotted with key light
            float beam = max(0.0, dot(exitRd, keyLight));
            beam = pow(beam, 3.0) * 2.0;
            spectral += beamCol * beam;
        }
        spectral /= 5.0;

        // Surface fresnel reflection
        float fresnel = pow(1.0 - max(0.0, dot(-rd, n)), 3.5);
        vec3 refDir = reflect(rd, n);
        float keySpec = pow(max(0.0, dot(refDir, keyLight)), 32.0);
        float fillSpec = pow(max(0.0, dot(refDir, fillLight)), 8.0);

        // Palette: ruby red, gold, electric cyan, deep violet, HDR white
        vec3 surface = spectral * 1.6
                     + vec3(1.0, 0.9, 0.8) * keySpec * hdrPeak * audio
                     + vec3(0.3, 0.6, 1.0) * fillSpec * 0.8;

        // Fresnel overlay (HDR white specular peak)
        surface += vec3(1.0) * fresnel * hdrPeak * 0.9 * audio;

        // Edge ink (dark silhouette at glancing angles)
        float inkEdge = smoothstep(0.35, 0.55, 1.0 - fresnel);
        surface *= inkEdge;

        // fwidth AA on crystal iso-edge
        float fw = fwidth(map(p));
        float aa = smoothstep(fw * 2.0, 0.0, abs(map(p)));
        col = mix(col, surface, aa);

    } else {
        // Background radial light shafts (light escaping crystal)
        float angle = atan(uv.y, uv.x);
        float shaft = pow(max(0.0, sin(angle * facets + t * 0.5)), 6.0);
        float shaftR = length(uv);
        shaft *= smoothstep(0.9, 0.4, shaftR) * smoothstep(0.0, 0.6, shaftR);
        float hue = fract(angle / (2.0 * PI) + t * 0.07);
        col += hsv2rgb(vec3(hue, 1.0, 1.0)) * shaft * hdrPeak * 0.5 * audio;
    }

    FragColor = vec4(col, 1.0);
}
