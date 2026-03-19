/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Translucent layered veils drifting and overlapping",
  "INPUTS": [
    {
      "NAME": "veilSpeed",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Veil Speed"
    },
    {
      "NAME": "layerCount",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Layer Count"
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
  float t = TIME * veilSpeed;
  float p1Val = veilSpeed;
  float p2Val = layerCount;

  vec3 col = vec3(0.015, 0.01, 0.02); for (int i = 0; i < 5; i++) { float fi = float(i); float angle = fi * 0.628 + t * 0.1; float ca = cos(angle), sa = sin(angle); vec2 vuv = vec2(ca * uv.x + sa * uv.y, -sa * uv.x + ca * uv.y); float veil = smoothstep(0.4, 0.6, fbm(vuv * (2.0 + fi) * p2Val + t * (0.1 + fi * 0.03))); vec3 veilCol = mix(vec3(0.3, 0.15, 0.2), vec3(0.2, 0.1, 0.3), fi / 5.0); col += veilCol * veil * 0.15; } col += vec3(0.05, 0.03, 0.04) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTexture, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}