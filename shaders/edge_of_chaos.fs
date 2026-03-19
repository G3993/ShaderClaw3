/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Reaction-diffusion patterns at the edge of order and chaos",
  "INPUTS": [
    {
      "NAME": "chaosLevel",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Chaos Level"
    },
    {
      "NAME": "patternScale",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Pattern Scale"
    },
    {
      "NAME": "baseColor",
      "LABEL": "Color",
      "TYPE": "color",
      "DEFAULT": [0.91, 0.25, 0.34, 1.0]
    },
    {
      "NAME": "inputTex",
      "LABEL": "Texture",
      "TYPE": "image"
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
  float t = TIME * chaosLevel;
  float p1Val = chaosLevel;
  float p2Val = patternScale;

  float n1 = fbm(uv * 4.0 * p2Val + t * 0.2); float n2 = fbm(uv * 8.0 * p2Val - t * 0.15 + 10.0); float reaction = sin(n1 * 10.0 + n2 * 5.0 + t) * 0.5 + 0.5; float edge = abs(n1 - n2); float pattern = smoothstep(0.3 - p1Val * 0.2, 0.5, reaction); vec3 col = vec3(0.02, 0.015, 0.01); col += vec3(0.7, 0.4, 0.1) * pattern; col += vec3(1.0, 0.8, 0.4) * smoothstep(0.1, 0.0, edge) * 0.5; col += vec3(0.1, 0.05, 0.02) * audioLevel;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}