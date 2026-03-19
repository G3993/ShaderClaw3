/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Slow-burning embers with smoky wisps and deep warm glow",
  "INPUTS": [
    {"NAME": "burnRate", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0, "LABEL": "Burn Rate"},
    {"NAME": "smokeAmount", "TYPE": "float", "DEFAULT": 1, "MIN": 0.0, "MAX": 2.0, "LABEL": "Smoke Amount"},
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
  float t = TIME * burnRate;
  float p1Val = burnRate;
  float p2Val = smokeAmount;

  float smoke = fbm(uv * 3.0 + vec2(0.0, -t * 0.3)); float ember = fbm(uv * 5.0 + t * 0.2); float mask = smoothstep(0.2, 0.6, smoke); float hot = smoothstep(0.5, 0.8, ember) * (1.0 - mask); vec3 col = vec3(0.02, 0.01, 0.008); col += vec3(0.6, 0.2, 0.05) * hot * p2Val * 0.8; col += vec3(1.0, 0.5, 0.1) * pow(hot, 3.0) * 0.5; col += vec3(0.03, 0.02, 0.01) * smoke * p2Val; col += vec3(0.1, 0.04, 0.01) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}