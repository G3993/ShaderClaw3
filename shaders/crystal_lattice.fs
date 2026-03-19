/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Growing crystalline lattice structure with refractive edges",
  "INPUTS": [
    {
      "NAME": "growthSpeed",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Growth Speed"
    },
    {
      "NAME": "facetCount",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Facet Count"
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
  float t = TIME * growthSpeed;
  float p1Val = growthSpeed;
  float p2Val = facetCount;

  float grid = 0.0; for (int i = 0; i < 3; i++) { float fi = float(i); float angle = fi * 1.047 + t * 0.05; float ca = cos(angle), sa = sin(angle); vec2 guv = vec2(ca * uv.x + sa * uv.y, -sa * uv.x + ca * uv.y) * (10.0 + fi * 5.0) * p2Val; vec2 gf = fract(guv) - 0.5; grid += smoothstep(0.02, 0.0, min(abs(gf.x), abs(gf.y))); } grid = clamp(grid, 0.0, 1.0); vec3 col = vec3(0.01, 0.015, 0.025); col += vec3(0.4, 0.6, 0.8) * grid * 0.3; col += vec3(0.8, 0.9, 1.0) * pow(grid, 4.0) * 0.5; col += vec3(0.05, 0.08, 0.1) * audioLevel;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTexture, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}