/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Digital signal degradation with glitch artifacts and scan lines",
  "INPUTS": [
    {
      "NAME": "decayRate",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Decay Rate"
    },
    {
      "NAME": "glitchAmount",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Glitch Amount"
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
  float t = TIME * decayRate;
  float p1Val = decayRate;
  float p2Val = glitchAmount;

  float scan = sin(uv.y * 200.0 + t * 5.0) * 0.5 + 0.5; float glitch = step(0.98 - p2Val * 0.1, hash(vec2(floor(uv.y * 20.0), floor(t * 10.0)))); vec2 guv = uv + vec2(glitch * 0.1, 0.0); float signal = fbm(guv * 4.0 + t * 0.3) * (1.0 - p1Val * 0.5); vec3 col = vec3(signal * 0.8, signal * 0.6, signal * 0.4) * scan; col += vec3(0.0, 0.1, 0.0) * glitch * p2Val; col += vec3(0.05, 0.03, 0.01) * audioLevel;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}