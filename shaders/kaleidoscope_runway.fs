/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Fashion runway kaleidoscope with shifting mirror symmetry",
  "INPUTS": [
    {
      "NAME": "rotSpeed",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Rot Speed"
    },
    {
      "NAME": "foldCount",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Fold Count"
    },
    {
      "NAME": "baseColor",
      "LABEL": "Color",
      "TYPE": "color",
      "DEFAULT": [0.91, 0.25, 0.34, 1.0]
    },
    {
      "NAME": "inputTexture",
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
  float t = TIME * rotSpeed;
  float p1Val = rotSpeed;
  float p2Val = foldCount;

  float n = 6.0 + p2Val * 6.0; float a = atan(uv.y, uv.x); float r = length(uv); float sector = 6.28318 / n; a = abs(mod(a + sector * 0.5, sector) - sector * 0.5); vec2 kuv = vec2(cos(a), sin(a)) * r; float pattern = fbm(kuv * 5.0 + t * 0.2); vec3 col = 0.5 + 0.5 * cos(6.28 * (vec3(1.0, 0.8, 0.6) * pattern + vec3(0.0, 0.1, 0.2) + t * 0.05)); col *= 0.8 + 0.2 * sin(r * 20.0 - t * 2.0); col *= 1.0 - r * 0.4; col += vec3(0.08, 0.04, 0.02) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTexture, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}