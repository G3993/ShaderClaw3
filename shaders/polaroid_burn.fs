/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Overexposed Polaroid with chemical burn edges and warm fade",
  "INPUTS": [
    {"NAME": "burnSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "LABEL": "Burn Speed"},
    {"NAME": "exposure", "TYPE": "float", "DEFAULT": 1, "MIN": 0.0, "MAX": 2.0, "LABEL": "Exposure"},
    {"NAME": "audioLevel", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Level"},
    {"NAME": "audioBass", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Bass"}
  ]
}*/

precision highp float;

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float vnoise(vec2 p) {
  vec2 i = floor(p), f = fract(p); f = f * f * (3.0 - 2.0 * f);
  return mix(mix(hash(i), hash(i+vec2(1,0)), f.x), mix(hash(i+vec2(0,1)), hash(i+vec2(1,1)), f.x), f.y);
}
float fbm(vec2 p) {
  float v = 0.0, a = 0.5; mat2 rot = mat2(0.8, 0.6, -0.6, 0.8);
  for (int i = 0; i < 5; i++) { v += a * vnoise(p); p = rot * p * 2.1; a *= 0.5; }
  return v;
}
void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME * burnSpeed;
  float p1Val = burnSpeed;
  float p2Val = exposure;

  float burn = fbm(uv * 2.5 + t * 0.1); float chemical = fbm(uv * 5.0 + vec2(t * 0.05, -t * 0.08) + 20.0); float overexpose = smoothstep(0.3, 0.7, burn) * p2Val; float chemBurn = smoothstep(0.55, 0.7, chemical) * smoothstep(0.35, 0.5, length(uv)); vec3 col = vec3(0.95, 0.9, 0.82) * (1.0 - overexpose * 0.3); col = mix(col, vec3(1.0, 0.85, 0.5), overexpose * 0.4); col = mix(col, vec3(0.6, 0.35, 0.15), chemBurn * 0.5); col *= smoothstep(0.55, 0.4, max(abs(uv.x), abs(uv.y * 0.8))); col += vec3(0.05, 0.03, 0.01) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}