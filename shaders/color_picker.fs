/*{
  "DESCRIPTION": "Spectral Light Cone — pure HDR spectral fan from a central prism point. Standalone 3D-feeling generator replaces the inputImage colour tinter. NEW ANGLE: spectral prism dispersion vs colour-multiply pass.",
  "CATEGORIES": ["Generator", "3D", "Color"],
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "INPUTS": [
    {"NAME":"openAngle","LABEL":"Spread","TYPE":"float","MIN":0.3,"MAX":3.14159,"DEFAULT":1.6},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.5},
    {"NAME":"beamBlur","LABEL":"Ray Blur","TYPE":"float","MIN":0.002,"MAX":0.08,"DEFAULT":0.014},
    {"NAME":"rotSpeed","LABEL":"Rotation","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.06},
    {"NAME":"prismSize","LABEL":"Prism Glow","TYPE":"float","MIN":0.01,"MAX":0.2,"DEFAULT":0.055},
    {"NAME":"bgDark","LABEL":"BG Darkness","TYPE":"float","MIN":0.0,"MAX":0.08,"DEFAULT":0.008},
    {"NAME":"audioReact","LABEL":"Audio","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

// 5-stop pure spectral palette, all HDR (no white mixing)
vec3 spectrum(float t) {
    t = clamp(t, 0.0, 1.0);
    if (t < 0.25) return mix(vec3(1.6, 0.0, 2.8),  vec3(0.0, 0.2, 2.8),  t * 4.0);
    if (t < 0.50) return mix(vec3(0.0, 0.2, 2.8),  vec3(0.0, 2.8, 1.8),  (t - 0.25) * 4.0);
    if (t < 0.75) return mix(vec3(0.0, 2.8, 1.8),  vec3(2.8, 2.4, 0.0),  (t - 0.50) * 4.0);
    return         mix(vec3(2.8, 2.4, 0.0),         vec3(2.8, 0.0, 0.0),  (t - 0.75) * 4.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

    // Slow rotation
    float rot = TIME * rotSpeed;
    float ca = cos(rot), sa = sin(rot);
    vec2 ruv = vec2(ca * uv.x - sa * uv.y, sa * uv.x + ca * uv.y);

    float audio = 1.0 + audioLevel * audioReact * 0.35
                      + audioBass  * audioReact * 0.25;

    float r     = length(ruv);
    float theta = atan(ruv.y, ruv.x); // -PI..PI

    // Spectral cone: theta in [-halfA, +halfA] mapped to wavelength [0,1]
    float halfA  = openAngle * 0.5;
    float inCone = smoothstep(halfA + beamBlur * 3.0, halfA, abs(theta));
    float tSpec  = clamp((theta + halfA) / openAngle, 0.0, 1.0);

    // Ray brightness: inverse-square radial falloff, sharpened at cone boundary
    float falloff   = 1.0 / (r * r * 12.0 + 0.04);
    float coneFade  = inCone * falloff * 0.8;

    // Narrow bright streak along each spectral band boundary (8 bands)
    float bandPos  = fract(tSpec * 8.0);
    float streak   = smoothstep(0.04, 0.0, abs(bandPos - 0.5)) * inCone
                   * exp(-r * 2.0) * 0.6;

    vec3 rayCol = spectrum(tSpec);
    vec3 col    = rayCol * (coneFade + streak) * hdrPeak * audio;

    // Central prism node — white-hot HDR burst
    float prismR  = prismSize * (1.0 + sin(TIME * 2.7) * 0.08)
                             * (1.0 + audioBass * audioReact * 0.15);
    float prismD  = r - prismR;
    float prismG  = exp(-max(prismD, 0.0) * 60.0);
    // Ink-black silhouette at prism edge
    float inkEdge = smoothstep(fwidth(prismD) * 2.0, 0.0, abs(prismD) - 0.001);
    col += vec3(3.0, 2.8, 2.2) * prismG * audio;
    col *= 1.0 - inkEdge * 0.85;

    // Near-black background tint (not pure void — subtle warmth)
    col += vec3(bgDark * 0.7, bgDark * 0.2, bgDark * 0.5);

    gl_FragColor = vec4(col, 1.0);
}
