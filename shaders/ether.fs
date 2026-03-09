/*{
  "DESCRIPTION": "Ether — volumetric light tendrils with rotating space distortion",
  "CREDIT": "nimitz (Shadertoy), adapted for ShaderClaw",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "depth", "LABEL": "Depth", "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 6.0 },
    { "NAME": "colorTint", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.1, 0.3, 0.4, 1.0] },
    { "NAME": "brightness", "LABEL": "Brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
  ]
}*/

mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

float map(vec3 p, float t) {
    p.xz *= rot(t * 0.4);
    p.xy *= rot(t * 0.3);
    vec3 q = p * 2.0 + t;
    return length(p + vec3(sin(t * 0.7))) * log(length(p) + 1.0)
         + sin(q.x + sin(q.z + sin(q.y))) * 0.5 - 1.0;
}

void main() {
    float t = TIME * speed;
    vec2 p = gl_FragCoord.xy / RENDERSIZE.y - vec2(0.9, 0.5);
    vec3 cl = vec3(0.0);
    float d = depth;

    for (int i = 0; i <= 5; i++) {
        vec3 pos = vec3(0.0, 0.0, 5.0) + normalize(vec3(p, -1.0)) * d;
        float rz = map(pos, t);
        float f = clamp((rz - map(pos + 0.1, t)) * 0.5, -0.1, 1.0);
        vec3 l = colorTint.rgb + vec3(5.0, 2.5, 3.0) * f;
        cl = cl * l + smoothstep(2.5, 0.0, rz) * 0.7 * l;
        d += min(rz, 1.0);
    }

    cl *= brightness;

    float alpha = 1.0;
    if (transparentBg) {
        alpha = clamp(dot(cl, vec3(0.299, 0.587, 0.114)) * 1.5, 0.0, 1.0);
    }

    gl_FragColor = vec4(cl, alpha);
}
