/*{
  "DESCRIPTION": "Ether — volumetric light tendrils with rotating space distortion",
  "CREDIT": "nimitz (Shadertoy), adapted for ShaderClaw",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Speed",       "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "depth",       "LABEL": "Depth",       "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5,  "MAX": 6.0 },
    { "NAME": "colorTint",   "LABEL": "Color",       "TYPE": "color", "DEFAULT": [0.5647, 0.2941, 0.5098, 1.0] },
    { "NAME": "highlightR",  "LABEL": "Highlight R", "TYPE": "float", "DEFAULT": 5.0,  "MIN": 0.0,  "MAX": 12.0 },
    { "NAME": "highlightG",  "LABEL": "Highlight G", "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.0,  "MAX": 12.0 },
    { "NAME": "highlightB",  "LABEL": "Highlight B", "TYPE": "float", "DEFAULT": 3.0,  "MIN": 0.0,  "MAX": 12.0 },
    { "NAME": "brightness",  "LABEL": "Brightness",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "twist",       "LABEL": "Twist",       "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 3.0 },
    { "NAME": "tendrilSize", "LABEL": "Tendril Size","TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3,  "MAX": 3.0 },
    { "NAME": "fov",         "LABEL": "FOV",         "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.4,  "MAX": 2.5 },
    { "NAME": "centerX",     "LABEL": "Center X",    "TYPE": "float", "DEFAULT": 0.9,  "MIN": -1.0, "MAX": 2.0 },
    { "NAME": "centerY",     "LABEL": "Center Y",    "TYPE": "float", "DEFAULT": 0.5,  "MIN": -1.0, "MAX": 2.0 },
    { "NAME": "audioReact",  "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "transparentBg","LABEL": "Transparent","TYPE": "bool",  "DEFAULT": 0.0 }
  ]
}*/

mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

float map(vec3 p, float t) {
    p.xz *= rot(t * 0.4 * twist);
    p.xy *= rot(t * 0.3 * twist);
    vec3 q = p * 2.0 + t;
    return length(p + vec3(sin(t * 0.7))) * log(length(p) + 1.0)
         + sin(q.x + sin(q.z + sin(q.y))) * 0.5 * tendrilSize - 1.0;
}

void main() {
    float audioPulse = 1.0 + audioBass * audioReact * 0.25;
    float t = TIME * speed * audioPulse;
    vec2 p = gl_FragCoord.xy / RENDERSIZE.y - vec2(centerX, centerY);
    p *= fov;
    vec3 cl = vec3(0.0);
    float d = depth;

    for (int i = 0; i <= 5; i++) {
        vec3 pos = vec3(0.0, 0.0, 5.0) + normalize(vec3(p, -1.0)) * d;
        float rz = map(pos, t);
        float f = clamp((rz - map(pos + 0.1, t)) * 0.5, -0.1, 1.0);
        vec3 l = colorTint.rgb + vec3(highlightR, highlightG, highlightB) * f;
        cl = cl * l + smoothstep(2.5, 0.0, rz) * 0.7 * l;
        d += min(rz, 1.0);
    }

    cl *= brightness * (0.85 + audioLevel * audioReact * 0.30);

    float alpha = 1.0;
    if (transparentBg) {
        alpha = clamp(dot(cl, vec3(0.299, 0.587, 0.114)) * 1.5, 0.0, 1.0);
    }

    // Surprise: every ~50s a chromatic aurora curtain ripples once
    // across the field — the ether gets pulled through a dispersive
    // medium for ~2s.
    {
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _ph = fract(TIME / 50.0);
        float _f  = smoothstep(0.0, 0.05, _ph) * smoothstep(0.30, 0.18, _ph);
        float _wave = sin(_suv.y * 8.0 + TIME * 4.0);
        vec3 _shift = vec3(sin(_suv.y * 6.0 + TIME * 2.0),
                           sin(_suv.y * 6.0 + TIME * 2.0 + 2.094),
                           sin(_suv.y * 6.0 + TIME * 2.0 + 4.188)) * 0.5 + 0.5;
        cl += _shift * 0.22 * _f * _wave * _wave;
    }

    gl_FragColor = vec4(cl, alpha);
}
