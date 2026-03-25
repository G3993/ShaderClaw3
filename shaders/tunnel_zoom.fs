/*{
  "DESCRIPTION": "Tunnel Zoom — radial zoom + twist from center, infinite tunnel effect",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "zoomSpeed", "LABEL": "Zoom Speed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "twist", "LABEL": "Twist", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "repeat", "LABEL": "Repeat", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 4.0 },
    { "NAME": "chromatic", "LABEL": "Chromatic", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float bass = smoothstep(0.0, 0.3, audioBass);
    float mid = smoothstep(0.0, 0.3, audioMid);
    float high = smoothstep(0.0, 0.3, audioHigh);
    float bassTime = TIME;
    float t = TIME * zoomSpeed;

    // Center on mouse
    vec2 center = mousePos;
    vec2 p = uv - center;
    p.x *= aspect;

    float r = length(p);
    float a = atan(p.y, p.x);

    // Zoom: log-polar mapping — bassTime syncs zoom to beat
    float logR = log(max(r, 0.001)) - bassTime * 2.0 * zoomSpeed - t * 0.3;
    // Mid drives twist intensity
    a += logR * twist * (1.0 + mid * 2.0);

    // Map back to UV
    vec2 warpUV = vec2(
        fract(a / 6.2832 * repeat + 0.5),
        fract(logR * repeat * 0.5)
    );

    vec3 col;
    if (hasInput) {
        if (chromatic > 0.001) {
            float ca = chromatic * 0.02;
            float rv = texture2D(inputTex, warpUV + vec2(ca, 0.0)).r;
            float gv = texture2D(inputTex, warpUV).g;
            float bv = texture2D(inputTex, warpUV - vec2(ca, 0.0)).b;
            col = vec3(rv, gv, bv);
        } else {
            col = texture2D(inputTex, warpUV).rgb;
        }
    } else {
        col = vec3(
            0.5 + 0.5 * sin(warpUV.x * 20.0),
            0.5 + 0.5 * sin(warpUV.y * 15.0 + 2.0),
            0.5 + 0.5 * sin((warpUV.x - warpUV.y) * 10.0 + 4.0)
        ) * (0.8 + 0.2 * sin(logR * 10.0));
    }

    // High adds shimmer
    col += col * high * 0.3 * sin(a * 20.0 + logR * 15.0);

    // Vignette
    col *= 1.0 - r * 0.3;

    float alpha = 1.0;
    if (transparentBg) alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.3)));
    gl_FragColor = vec4(col, alpha);
}
