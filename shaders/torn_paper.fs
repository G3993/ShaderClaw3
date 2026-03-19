/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Torn paper edges revealing warm light beneath",
  "INPUTS": [
    {"NAME": "tearSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "LABEL": "Tear Speed"},
    {"NAME": "tearWidth", "TYPE": "float", "DEFAULT": 1, "MIN": 0.0, "MAX": 2.0, "LABEL": "Tear Width"},
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
  float t = TIME * tearSpeed;
  float p1Val = tearSpeed;
  float p2Val = tearWidth;

  float tear = fbm(vec2(uv.x * 3.0, uv.y + t * 0.2) + 5.0); float tearLine = smoothstep(0.48 - p2Val * 0.05, 0.5, tear) * (1.0 - smoothstep(0.5, 0.52 + p2Val * 0.05, tear)); float paper = smoothstep(0.5, 0.52, tear); vec3 paperCol = vec3(0.85, 0.82, 0.78); vec3 tearEdge = vec3(1.0, 0.9, 0.7); vec3 beneath = vec3(0.9, 0.5, 0.15) * (0.5 + 0.5 * fbm(uv * 4.0 + t * 0.1)); vec3 col = mix(beneath, paperCol, paper); col += tearEdge * tearLine * 0.5; col += vec3(0.05, 0.03, 0.01) * audioLevel;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}