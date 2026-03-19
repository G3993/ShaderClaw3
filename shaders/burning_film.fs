/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Burning celluloid film effect with melting edges and ember glow",
  "INPUTS": [
    {
      "NAME": "burnSpeed",
      "TYPE": "float",
      "DEFAULT": 0.4,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Burn Speed"
    },
    {
      "NAME": "burnIntensity",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Burn Intensity"
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
  float t = TIME * burnSpeed;
  float bi = burnIntensity + audioBass * 0.5;

  float burn = fbm(uv * 3.0 + vec2(0.0, -t * 0.5));
  float burnEdge = fbm(uv * 5.0 + vec2(t * 0.3, -t * 0.8));
  float burnMask = smoothstep(0.3 - bi * 0.2, 0.5, burn + sin(t * 0.5) * 0.1);
  float edge = smoothstep(0.0, 0.15, burnMask) * (1.0 - smoothstep(0.15, 0.35, burnMask));
  float ember = smoothstep(0.0, 0.1, burnMask) * (1.0 - smoothstep(0.1, 0.2, burnMask));

  vec3 filmColor = vec3(0.04, 0.03, 0.025);
  vec3 burnColor = vec3(0.8, 0.3, 0.05);
  vec3 emberColor = vec3(1.0, 0.7, 0.2);
  vec3 ashColor = vec3(0.01, 0.008, 0.005);

  vec3 col = mix(filmColor, ashColor, 1.0 - burnMask);
  col += burnColor * edge * bi * 2.0;
  col += emberColor * ember * bi * 1.5;
  col += vec3(0.15, 0.06, 0.01) * pow(edge, 0.5) * bi;
  col += vec3(0.05, 0.02, 0.01) * audioLevel;

  col += (hash(gl_FragCoord.xy + fract(TIME) * 100.0) - 0.5) * 0.02;
  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));

  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}