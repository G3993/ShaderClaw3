/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Thunderous storm clouds with lightning flashes and rain",
  "INPUTS": [
    {
      "NAME": "stormSpeed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Storm Speed"
    },
    {
      "NAME": "lightningFreq",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Lightning Freq"
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
  float t = TIME * stormSpeed;
  float p1Val = stormSpeed;
  float p2Val = lightningFreq;

  float clouds = fbm(uv * 2.0 + vec2(t * 0.15, 0.0)); float clouds2 = fbm(uv * 4.0 + vec2(-t * 0.1, t * 0.05) + 30.0); float storm = clouds * 0.6 + clouds2 * 0.4; float lightning = step(0.99 - p2Val * 0.05, hash(vec2(floor(TIME * 8.0), 0.0))) * exp(-length(uv - vec2(hash(vec2(floor(TIME * 8.0), 1.0)) - 0.5, 0.3)) * 3.0); vec3 col = mix(vec3(0.02, 0.02, 0.04), vec3(0.1, 0.08, 0.12), storm); col += vec3(0.8, 0.85, 1.0) * lightning * 2.0; col += vec3(0.05, 0.04, 0.06) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTexture, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}