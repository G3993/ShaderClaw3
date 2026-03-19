/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Spinning vinyl record with concentric groove patterns",
  "INPUTS": [
    {"NAME": "spinSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0, "LABEL": "Spin Speed"},
    {"NAME": "wearAmount", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Wear Amount"},
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
  float t = TIME * spinSpeed;
  float p1Val = spinSpeed;
  float p2Val = wearAmount;

  float r = length(uv); float a = atan(uv.y, uv.x); float grooves = sin(r * 200.0) * 0.5 + 0.5; float spin = sin(a * 2.0 + r * 50.0 - t * 3.0) * 0.5 + 0.5; float label = smoothstep(0.12, 0.1, r); float vinyl = smoothstep(0.5, 0.48, r) * (1.0 - label); float wear = vnoise(vec2(a * 3.0, r * 20.0) + t * 0.1) * p2Val; vec3 col = vec3(0.02, 0.02, 0.025) * (1.0 + grooves * 0.3) * vinyl; col += vec3(0.1, 0.08, 0.06) * spin * vinyl * 0.2; col += vec3(0.5, 0.35, 0.15) * label; col += vec3(0.03, 0.02, 0.01) * wear * vinyl; col += vec3(0.02, 0.015, 0.01) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}