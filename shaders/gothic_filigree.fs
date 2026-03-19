/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Ornate gothic filigree scrollwork patterns",
  "INPUTS": [
    {
      "NAME": "scrollSpeed",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Scroll Speed"
    },
    {
      "NAME": "detail",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Detail"
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
  float t = TIME * scrollSpeed;
  float p1Val = scrollSpeed;
  float p2Val = detail;

  float pattern = 0.0; for (int i = 0; i < 4; i++) { float fi = float(i); float scale = 3.0 + fi * 3.0; vec2 puv = uv * scale * p2Val; float curl = sin(puv.x + sin(puv.y * 2.0 + t * 0.2)) * cos(puv.y + cos(puv.x * 1.5 + t * 0.15)); pattern += smoothstep(0.4, 0.5, curl) * (1.0 / (1.0 + fi)); } vec3 col = vec3(0.02, 0.015, 0.01); col += vec3(0.6, 0.45, 0.2) * pattern * 0.4; col += vec3(0.9, 0.75, 0.4) * pow(pattern, 3.0) * 0.3; col += vec3(0.05, 0.03, 0.01) * audioLevel;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTexture, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}