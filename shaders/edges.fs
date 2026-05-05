/*{
    "DESCRIPTION": "Neon Helix — raymarched intertwined double-helix tubes with HDR neon glow. Standalone 3D generator.",
    "CATEGORIES": ["Generator", "3D", "Abstract"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "helixSpeed",   "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Helix Speed" },
        { "NAME": "tubeRadius",   "TYPE": "float", "DEFAULT": 0.09, "MIN": 0.02,"MAX": 0.25,"LABEL": "Tube Radius" },
        { "NAME": "helixPitch",   "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.5, "MAX": 3.0, "LABEL": "Pitch" },
        { "NAME": "glowWidth",    "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.05,"MAX": 0.6, "LABEL": "Glow Width" },
        { "NAME": "hdrPeak",      "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0, "MAX": 4.0, "LABEL": "HDR Peak" },
        { "NAME": "camAngle",     "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Orbit" },
        { "NAME": "audioReact",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" }
    ]
}*/

// Distance to a helix strand (capsule swept along sinusoidal spine)
float sdHelixStrand(vec3 p, float phase, float pitch, float radius, float t) {
    // Map Y to angle along helix axis
    float angle = p.y * pitch + phase + t;
    vec2 center = vec2(cos(angle) * 0.35, sin(angle) * 0.35);
    vec2 diff   = p.xz - center;
    return length(diff) - radius;
}

float map(vec3 p, float t) {
    float d0 = sdHelixStrand(p, 0.0,   helixPitch, tubeRadius, t);
    float d1 = sdHelixStrand(p, 3.14159, helixPitch, tubeRadius, t);
    return min(d0, d1);
}

vec3 calcNormal(vec3 p, float t) {
    float e = 0.0004;
    return normalize(vec3(
        map(p+vec3(e,0,0),t)-map(p-vec3(e,0,0),t),
        map(p+vec3(0,e,0),t)-map(p-vec3(0,e,0),t),
        map(p+vec3(0,0,e),t)-map(p-vec3(0,0,e),t)
    ));
}

// Strand ID: which strand was hit (for color assignment)
float strandId(vec3 p, float t) {
    float d0 = sdHelixStrand(p, 0.0,   helixPitch, tubeRadius, t);
    float d1 = sdHelixStrand(p, 3.14159, helixPitch, tubeRadius, t);
    return (d0 < d1) ? 0.0 : 1.0;
}

void main() {
    vec2 uv  = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x    *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel * 0.5 + audioBass * 0.5) * audioReact;
    float t     = TIME * helixSpeed;

    // Orbiting camera
    float camA  = TIME * camAngle * 0.7;
    float camR  = 1.8 / audio;
    vec3  ro    = vec3(sin(camA) * camR, 0.2, cos(camA) * camR);
    vec3  target= vec3(0.0, 0.0, 0.0);
    vec3  fwd   = normalize(target - ro);
    vec3  rgt   = normalize(cross(vec3(0,1,0), fwd));
    vec3  up2   = cross(fwd, rgt);
    vec3  rd    = normalize(uv.x * rgt + uv.y * up2 + 1.6 * fwd);

    // March
    float td = 0.05, hit = -1.0;
    float sid = 0.0;
    for (int i = 0; i < 80; i++) {
        vec3  p = ro + rd * td;
        float d = map(p, t);
        if (d < 0.0005) { hit = td; sid = strandId(p, t); break; }
        if (td > 5.0) break;
        td += max(d * 0.8, 0.002);
    }

    // 4-color palette: electric magenta, cyan, gold, deep violet bg
    vec3 colA = vec3(1.0, 0.05, 0.9);   // electric magenta (strand 0)
    vec3 colB = vec3(0.0, 1.0,  1.0);   // cyan (strand 1)
    vec3 colSpec = vec3(1.0, 1.0, 1.0); // white specular
    vec3 bgCol   = vec3(0.005, 0.0, 0.02);

    // Background: subtle neon radial glow
    float bgGlow = exp(-length(uv) * 1.8) * 0.12;
    vec3 col = bgCol + colA * bgGlow * 0.5 + colB * bgGlow * 0.5;

    // Volumetric tube glow (visible even without hit — captures tubes near ray)
    {
        float minD = 1e6;
        float tg = 0.05;
        for (int i = 0; i < 60; i++) {
            vec3 p = ro + rd * tg;
            float d = map(p, t);
            minD = min(minD, d);
            if (tg > 4.5) break;
            tg += max(d * 0.6, 0.004);
        }
        float glow0 = exp(-max(minD, 0.0) / glowWidth);
        // Split glow by closer strand
        vec3 pGlow = ro + rd * clamp(tg * 0.5, 0.05, 4.5);
        float dA   = sdHelixStrand(pGlow, 0.0,   helixPitch, tubeRadius, t);
        float dB   = sdHelixStrand(pGlow, 3.14159, helixPitch, tubeRadius, t);
        vec3 glowCol = (dA < dB) ? colA : colB;
        col += glowCol * glow0 * hdrPeak * audio * 0.6;
    }

    if (hit > 0.0) {
        vec3 p = ro + rd * hit;
        vec3 n = calcNormal(p, t);
        vec3 sc = (sid < 0.5) ? colA : colB;

        vec3  lk   = normalize(vec3(1.5, 2.0, 0.8));
        float diff = max(dot(n, lk), 0.0);
        float spec = pow(max(dot(reflect(-lk, n), -rd), 0.0), 40.0);
        float fres = pow(1.0 - abs(dot(n, -rd)), 3.0);

        col  = sc * (0.1 + diff * 0.9) * hdrPeak * audio;
        col += colSpec * spec * hdrPeak * 0.9;
        col += sc * fres * hdrPeak * 0.7;

        // Black ink silhouette at grazing edges
        float aa = fwidth(hit * 0.001);
        float edge = smoothstep(0.0, aa + 0.015, fres - 0.7);
        col = mix(col, vec3(0.0), edge);
    }

    gl_FragColor = vec4(col, 1.0);
}
