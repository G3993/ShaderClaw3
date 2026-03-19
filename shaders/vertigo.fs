/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Hitchcock-inspired spiral vertigo effect with hypnotic rotation",
  "INPUTS": [
    {"NAME": "spiralSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Spiral Speed"},
    {"NAME": "depth", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Depth"},
    {"NAME": "audioLevel", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Level"},
    {"NAME": "audioBass", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Bass"}
  ]
}*/

precision highp float;
#define PI 3.14159265359
#define TAU 6.28318530718

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME * spiralSpeed;
  float r = length(uv), a = atan(uv.y, uv.x);
  float dp = depth + audioBass * 0.5;

  float spiral = sin(a * 3.0 + log(r + 0.01) * 8.0 * dp - t * 3.0);
  float spiral2 = sin(a * 5.0 - log(r + 0.01) * 12.0 * dp + t * 2.0);
  float rings = sin(r * 30.0 * dp - t * 4.0) * 0.5 + 0.5;

  float pattern = spiral * 0.5 + spiral2 * 0.3 + rings * 0.2;
  pattern = pattern * 0.5 + 0.5;

  vec3 c1 = vec3(0.8, 0.2, 0.3);
  vec3 c2 = vec3(0.1, 0.05, 0.15);
  vec3 c3 = vec3(0.9, 0.7, 0.3);

  vec3 col = mix(c2, c1, smoothstep(0.3, 0.7, pattern));
  col = mix(col, c3, smoothstep(0.7, 0.95, pattern) * 0.5);
  col *= 1.0 - r * 0.5;
  col += vec3(0.08, 0.04, 0.02) * audioLevel;

  col = pow(col, vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}