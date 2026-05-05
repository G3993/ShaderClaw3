/*{
  "DESCRIPTION": "Prism Dispersion — raymarched triangular prism scattering HDR spectrum light. Standalone generator.",
  "CATEGORIES": ["Generator", "3D", "Abstract"],
  "CREDIT": "ShaderClaw auto-improve",
  "INPUTS": [
    { "NAME": "prismRot",   "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0,  "MAX": 1.0, "LABEL": "Rotation Speed" },
    { "NAME": "dispersion", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.1,  "MAX": 1.0, "LABEL": "Dispersion" },
    { "NAME": "hdrPeak",    "TYPE": "float", "DEFAULT": 2.6,  "MIN": 1.0,  "MAX": 4.0, "LABEL": "HDR Peak" },
    { "NAME": "camTilt",    "TYPE": "float", "DEFAULT": 0.18, "MIN": -0.4, "MAX": 0.4, "LABEL": "Camera Tilt" },
    { "NAME": "audioReact", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0, "LABEL": "Audio React" }
  ]
}*/

// Equilateral-triangle prism SDF (triangle in XY, prism depth along Z)
float sdTriPrism(vec3 p, vec2 h) {
    vec3 q = abs(p);
    return max(q.z - h.y, max(q.x * 0.866025 + p.y * 0.5, -p.y) - h.x * 0.5);
}

float map(vec3 p, float rot) {
    float ca = cos(rot), sa = sin(rot);
    vec3 q = vec3(ca*p.x + sa*p.z, p.y, -sa*p.x + ca*p.z);
    return sdTriPrism(q, vec2(0.75, 0.52));
}

vec3 calcNormal(vec3 p, float rot) {
    float e = 0.0005;
    return normalize(vec3(
        map(p+vec3(e,0,0),rot) - map(p-vec3(e,0,0),rot),
        map(p+vec3(0,e,0),rot) - map(p-vec3(0,e,0),rot),
        map(p+vec3(0,0,e),rot) - map(p-vec3(0,0,e),rot)
    ));
}

// 6-stop fully-saturated spectrum: violet→blue→cyan→green→gold→orange
vec3 spectrum(float t) {
    t = clamp(t, 0.0, 1.0) * 5.0;
    int i = int(t); float f = fract(t);
    const vec3 c0 = vec3(0.55, 0.0,  1.0);  // violet
    const vec3 c1 = vec3(0.0,  0.1,  1.0);  // blue
    const vec3 c2 = vec3(0.0,  1.0,  1.0);  // cyan
    const vec3 c3 = vec3(0.0,  1.0,  0.05); // green
    const vec3 c4 = vec3(1.0,  0.9,  0.0);  // gold
    const vec3 c5 = vec3(1.0,  0.15, 0.0);  // orange-red
    if (i == 0) return mix(c0, c1, f);
    if (i == 1) return mix(c1, c2, f);
    if (i == 2) return mix(c2, c3, f);
    if (i == 3) return mix(c3, c4, f);
    return mix(c4, c5, f);
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + (audioLevel * 0.6 + audioBass * 0.4) * audioReact;
    float rot = TIME * prismRot;

    vec3 ro = vec3(0.0, camTilt * 0.9, 2.4 / audio);
    vec3 fwd = normalize(vec3(0.0, camTilt * -0.25, 0.0) - ro);
    vec3 rgt = normalize(cross(vec3(0,1,0), fwd));
    vec3 up2 = cross(fwd, rgt);
    vec3 rd  = normalize(uv.x * rgt + uv.y * up2 + 1.7 * fwd);

    // Raymarch (64 steps)
    float td = 0.05, hit = -1.0;
    for (int i = 0; i < 64; i++) {
        float d = map(ro + rd * td, rot);
        if (d < 0.0006) { hit = td; break; }
        if (td > 7.0) break;
        td += max(d, 0.003);
    }

    vec3 bg = vec3(0.005, 0.0, 0.025); // deep violet-black

    // Background: faint dispersion halo
    float beamAngle = atan(uv.y, uv.x) / 6.2832 + 0.5;
    vec3 bgBeam = spectrum(beamAngle * dispersion + TIME * 0.04);
    bg += bgBeam * exp(-length(uv) * 2.8) * 0.07 * audio;

    if (hit < 0.0) { gl_FragColor = vec4(bg, 1.0); return; }

    vec3 p  = ro + rd * hit;
    vec3 n  = calcNormal(p, rot);

    // Spectrum color from vertical position in prism
    float specT = (p.y + 0.5) / 1.0 * dispersion + (1.0 - dispersion) * 0.5;
    vec3  specCol = spectrum(specT);

    // Lighting: strong key light + backlight rim
    vec3 lk  = normalize(vec3(1.8, 2.2, 1.0));
    float diff = max(dot(n, lk), 0.0);
    float spec = pow(max(dot(reflect(-lk, n), -rd), 0.0), 56.0);

    // Fresnel: dark edges (black ink silhouette)
    float fres = pow(1.0 - abs(dot(n, -rd)), 3.5);

    vec3 col  = specCol * (0.15 + diff * 0.85) * hdrPeak * audio;
    col      += vec3(1.0) * spec * hdrPeak * 0.8;       // white-hot specular
    col      += specCol   * fres * hdrPeak * 0.6;        // rim HDR

    // Black ink at silhouette edges using fwidth AA
    float dNear = map(p, rot);
    float aa    = fwidth(dNear + td * 0.0002);
    float edge  = 1.0 - smoothstep(0.0, aa + 0.002, abs(dNear) + fres * 0.04);
    col = mix(col, vec3(0.0), edge * 0.85);

    gl_FragColor = vec4(col + bg * (1.0 - smoothstep(0.0, 0.5, hit * 0.3)), 1.0);
}
