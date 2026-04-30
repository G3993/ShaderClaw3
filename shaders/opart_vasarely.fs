/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Op Art after Vasarely's Vega series (1968-69) — concentric ribbed rings warped by a spherical bulge so the flat surface acquires impossible volume, with two-tone hue rotation across the canvas. No checkerboard, no cells: pure radial-phase modulation as TouchDesigner-style optical illusion.",
  "INPUTS": [
    { "NAME": "ringFrequency", "LABEL": "Ring Frequency", "TYPE": "float", "MIN": 6.0, "MAX": 60.0, "DEFAULT": 28.0 },
    { "NAME": "bulgeAmount", "LABEL": "Bulge Amount", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.55 },
    { "NAME": "bulgeRadius", "LABEL": "Bulge Radius", "TYPE": "float", "MIN": 0.2, "MAX": 1.2, "DEFAULT": 0.55 },
    { "NAME": "bulgeCx", "LABEL": "Bulge Center X", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "bulgeCy", "LABEL": "Bulge Center Y", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "ribSharpness", "LABEL": "Rib Sharpness", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35 },
    { "NAME": "hueA", "LABEL": "Hue A", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.85 },
    { "NAME": "hueB", "LABEL": "Hue B", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.50 },
    { "NAME": "hueRotateSpeed", "LABEL": "Hue Rotate Speed", "TYPE": "float", "MIN": 0.0, "MAX": 0.4, "DEFAULT": 0.05 },
    { "NAME": "saturation", "LABEL": "Saturation", "TYPE": "float", "MIN": 0.4, "MAX": 1.4, "DEFAULT": 0.95 },
    { "NAME": "twist", "LABEL": "Twist", "TYPE": "float", "MIN": 0.0, "MAX": 6.28, "DEFAULT": 0.0 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "useTex", "LABEL": "Sample Tex for Hues", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

// Vasarely's Vega series creates apparent 3D volume on a flat surface by
// warping a periodic pattern (rings, here) through a SPHERICAL BULGE
// transform: r' = r * (1 + bulge * smoothstep(R, 0, r)). Where the bulge
// is strongest, ring spacing compresses; where weakest, it expands. The
// eye reads compressed = closer = convex.

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Bulge centre — always wanders, audio adds extra jitter on top.
    vec2 bc = vec2(bulgeCx, bulgeCy);
    bc += vec2(sin(TIME * 0.5), cos(TIME * 0.4))
       * 0.05 * (1.0 + audioLevel * audioReact);

    vec2 d = uv - bc;
    d.x *= aspect;

    // Polar coords
    float r  = length(d);
    float th = atan(d.y, d.x);

    // Spherical bulge: time-breathing magnitude so the dome inflates and
    // deflates continuously even with audio off; bass adds extra punch.
    float t = clamp(1.0 - r / max(bulgeRadius, 1e-3), 0.0, 1.0);
    float bulgeMag = bulgeAmount
                   * (0.6 + 0.4 * sin(TIME * 0.7))
                   * (1.0 + audioBass * audioReact * 0.35);
    float warp = 1.0 - bulgeMag * t * t;
    float rW = r * warp;

    // Auto-rotating twist — rings spiral continuously.
    th += (twist + TIME * 0.20) * smoothstep(0.0, bulgeRadius, r);

    // Phase along radius — frequency itself breathes so rings expand
    // and contract.
    float freqNow = ringFrequency * (1.0 + 0.18 * sin(TIME * 0.5));
    float phase = rW * freqNow;
    float band = sin(phase) * 0.5 + 0.5;
    float k = ribSharpness;
    band = smoothstep(0.5 - k * 0.5, 0.5 + k * 0.5, band);

    // Two-tone palette with hue always rotating across canvas; audio
    // accelerates the rotation but never gates it.
    float hueShift = (uv.x + uv.y * 0.4) * 0.5
                   + TIME * hueRotateSpeed
                   + TIME * 0.05 * audioMid * audioReact;
    // Polychrome ring assignment — canonical Vasarely Vega-Nor cycles
    // through ~4 hues per ring index, not just two-tone alternation.
    int ringIdx = int(floor(phase / 3.14159));
    vec3 vegaP[4] = vec3[4](
        hsv2rgb(vec3(fract(0.55 + hueShift), saturation, 0.95)),  // teal
        hsv2rgb(vec3(fract(0.10 + hueShift), saturation, 0.95)),  // orange
        hsv2rgb(vec3(fract(0.85 + hueShift), saturation, 0.95)),  // magenta
        hsv2rgb(vec3(fract(0.30 + hueShift), saturation, 0.95))   // yellow-green
    );
    if (useTex && IMG_SIZE_inputTex.x > 0.0) {
        // When a texture is bound, sample 4 fixed points for the palette
        vegaP[0] = texture(inputTex, vec2(0.15, 0.5)).rgb;
        vegaP[1] = texture(inputTex, vec2(0.45, 0.5)).rgb;
        vegaP[2] = texture(inputTex, vec2(0.75, 0.5)).rgb;
        vegaP[3] = texture(inputTex, vec2(0.95, 0.5)).rgb;
    }
    vec3 colA = vegaP[ringIdx & 3];
    vec3 colB = vegaP[(ringIdx + 1) & 3];
    vec3 col  = mix(colA, colB, band);

    // Centre highlight — bulge focal point glows brighter, sells volume.
    float spec = exp(-pow(r * 4.0 / max(bulgeRadius, 1e-3), 2.0))
               * 0.30 * (1.0 + audioHigh * audioReact * 0.5);
    col += spec * vec3(0.95);

    // Slight radial shadow at the rim of the bulge
    float rim = smoothstep(bulgeRadius * 0.85, bulgeRadius, r)
              * smoothstep(bulgeRadius * 1.15, bulgeRadius, r);
    col *= 1.0 - rim * 0.18;

    gl_FragColor = vec4(col, 1.0);
}
