/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Deep magma core with convection currents and glowing fissures",
  "INPUTS": [
    {
      "NAME": "convectionSpeed",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Convection Speed"
    },
    {
      "NAME": "heatLevel",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Heat Level"
    }
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
  float t = TIME * convectionSpeed;
  float p1Val = convectionSpeed;
  float p2Val = heatLevel;

  float lava = fbm(uv * 2.0 + vec2(t * 0.15, -t * 0.1)); float fissure = fbm(uv * 6.0 + t * 0.2); float crack = smoothstep(0.45, 0.5, fissure); float glow = smoothstep(0.3, 0.7, lava); vec3 col = vec3(0.05, 0.01, 0.0); col += vec3(0.6, 0.1, 0.0) * glow * p2Val; col += vec3(1.0, 0.5, 0.05) * crack * p2Val * 1.5; col += vec3(1.0, 0.8, 0.3) * pow(crack, 3.0) * 0.5; col += vec3(0.15, 0.05, 0.01) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}