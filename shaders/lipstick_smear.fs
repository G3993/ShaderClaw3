/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Bold lipstick smear streaks across a dark surface",
  "INPUTS": [
    {
      "NAME": "smearSpeed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Smear Speed"
    },
    {
      "NAME": "intensity",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Intensity"
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
  float t = TIME * smearSpeed;
  float p1Val = smearSpeed;
  float p2Val = intensity;

  vec2 suv = vec2(uv.x, uv.y * 0.5); float smear = fbm(suv * 3.0 + vec2(t * 0.3, 0.0)); float streak = smoothstep(0.3, 0.5, smear) * smoothstep(0.7, 0.5, smear); vec3 lipColor = mix(vec3(0.8, 0.1, 0.15), vec3(0.6, 0.05, 0.1), fbm(uv * 5.0)); vec3 col = vec3(0.02, 0.015, 0.01); col += lipColor * streak * p2Val * 1.5; col += vec3(1.0, 0.3, 0.4) * pow(streak, 4.0) * 0.3; col += vec3(0.1, 0.02, 0.03) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}