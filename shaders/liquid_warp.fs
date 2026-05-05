/*{
  "DESCRIPTION": "Liquid Warp — underwater ripple displacement on input video, audio-reactive",
  "CATEGORIES": ["VFX"],
  "INPUTS": [
    { "NAME": "inputTex", "LABEL": "Input", "TYPE": "image" },
    { "NAME": "amount", "LABEL": "Amount", "TYPE": "float", "DEFAULT": 0.03, "MIN": 0.0, "MAX": 0.15 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0 },
    { "NAME": "scale", "LABEL": "Scale", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 30.0 },
    { "NAME": "chromatic", "LABEL": "Chromatic", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;
    bool hasInput = IMG_SIZE_inputTex.x > 0.0;
    float t = TIME * speed;
    float bass = smoothstep(0.0, 0.3, audioBass);
    float mid = smoothstep(0.0, 0.3, audioMid);
    float high = smoothstep(0.0, 0.3, audioHigh);
    float amt = amount * (1.0 + bass * 3.0 + audioBass * 2.0);

    // Multi-layer sine displacement
    vec2 d = vec2(0.0);
    d.x += sin(uv.y * scale + t * 1.3) * amt;
    d.x += sin(uv.y * scale * 2.1 - t * 0.7) * amt * 0.5;
    d.y += cos(uv.x * scale * 1.4 + t) * amt;
    d.y += cos(uv.x * scale * 2.7 - t * 1.1) * amt * 0.4;

    // High adds fine ripple detail
    d += vec2(sin(uv.y * scale * 6.0 + t * 3.0), cos(uv.x * scale * 6.0 + t * 2.5)) * amt * high * 0.4;

    // Mouse influence — ripple from mouse
    vec2 mp = uv - mousePos;
    float mr = length(mp);
    d += mp * sin(mr * 40.0 - t * 4.0) * amt * 0.5 * exp(-mr * 5.0);

    // Bass hit triggers a splash from center
    vec2 sp = uv - 0.5;
    float sr = length(sp);
    d += sp * audioBass * 0.06 * sin(sr * 30.0) * exp(-sr * 4.0);

    vec3 col;
    if (hasInput) {
        if (chromatic > 0.001 || mid > 0.01) {
            float ca = chromatic * amt * 0.5 + mid * 0.008;
            float r = texture2D(inputTex, uv + d + vec2(ca, 0.0)).r;
            float g = texture2D(inputTex, uv + d).g;
            float b = texture2D(inputTex, uv + d - vec2(ca, 0.0)).b;
            col = vec3(r, g, b);
        } else {
            col = texture2D(inputTex, uv + d).rgb;
        }
    } else {
        vec2 wuv = uv + d;
        col = vec3(
            0.5 + 0.5 * sin(wuv.x * 10.0 + t),
            0.5 + 0.5 * sin(wuv.y * 12.0 + t * 1.3),
            0.5 + 0.5 * sin((wuv.x + wuv.y) * 8.0 - t)
        );
    }

    float alpha = 1.0;
    if (transparentBg) {
        alpha = smoothstep(0.02, 0.15, dot(col, vec3(0.299, 0.587, 0.114)));
    }

    // Surprise: every ~25s the liquid briefly crystallizes — the warp
    // freezes into hexagonal facets for ~1s, like ice forming on a
    // film of water, then thaws.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 25.0);
        float _f  = smoothstep(0.0, 0.05, _ph) * smoothstep(0.22, 0.12, _ph);
        // Hex grid quantize
        vec2 _h = _suv * 18.0;
        vec2 _hi = floor(_h + 0.5);
        col = mix(col, vec3(0.85, 0.92, 1.0) * (0.4 + 0.6 * fract(sin(dot(_hi, vec2(12.9898, 78.233))) * 43758.5453)), _f * 0.65);
    }
    gl_FragColor = vec4(col, alpha);
}
