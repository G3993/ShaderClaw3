/*{
  "DESCRIPTION": "Twist Vortex — angular twist that spirals toward center, audio-reactive spin",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "twistAmount", "LABEL": "Twist", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 10.0 },
    { "NAME": "twistRadius", "LABEL": "Radius", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 2.0 },
    { "NAME": "spin", "LABEL": "Spin", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "segments", "LABEL": "Segments", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 8.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float bass = audioBass;

    vec2 center = mousePos;
    vec2 p = uv - center;
    p.x *= aspect;

    float r = length(p);
    float a = atan(p.y, p.x);

    float rad = twistRadius;
    float tw = twistAmount * (1.0 + bass * 2.0);

    // Twist: more rotation closer to center
    float falloff = 1.0 - smoothstep(0.0, rad, r);
    float angle = tw * falloff * falloff + spin * TIME;

    float ca = cos(angle), sa = sin(angle);
    vec2 twisted = vec2(ca * p.x - sa * p.y, sa * p.x + ca * p.y);
    twisted.x /= aspect;
    vec2 warpUV = twisted + center;

    // Optional kaleidoscope segments
    if (segments > 0.5) {
        float seg = floor(segments);
        float segAngle = 6.2832 / seg;
        float na = atan(twisted.y, twisted.x * aspect);
        na = mod(na, segAngle);
        if (mod(floor(na / segAngle * seg), 2.0) > 0.5) na = segAngle - na;
        float nr = length(twisted * vec2(aspect, 1.0));
        warpUV = center + vec2(cos(na) / aspect, sin(na)) * nr;
    }

    warpUV = fract(warpUV);

    vec3 col;
    if (hasInput) {
        col = texture2D(inputTex, warpUV).rgb;
    } else {
        col = vec3(0.5 + 0.5 * sin(a * 5.0 + TIME), 0.3, 0.5 + 0.5 * cos(r * 20.0));
    }

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
