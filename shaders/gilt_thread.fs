/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Woven golden thread patterns with intricate overlapping filaments",
  "INPUTS": [
    {
      "NAME": "weaveSpeed",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Weave Speed"
    },
    {
      "NAME": "threadDensity",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.5,
      "MAX": 2,
      "LABEL": "Thread Density"
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

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME * weaveSpeed;
  float td = threadDensity + audioBass * 0.3;

  float thread = 0.0;
  for (int i = 0; i < 6; i++) {
    float fi = float(i);
    float angle = fi * 0.524 + t * 0.1 * (mod(fi, 2.0) < 1.0 ? 1.0 : -1.0);
    float ca = cos(angle), sa = sin(angle);
    vec2 ruv = vec2(ca * uv.x + sa * uv.y, -sa * uv.x + ca * uv.y);
    float wave = sin(ruv.x * 20.0 * td + t * (0.5 + fi * 0.2)) * 0.5 + 0.5;
    float line = smoothstep(0.02, 0.0, abs(fract(ruv.y * 15.0 * td + wave * 0.1) - 0.5) - 0.04);
    thread += line * (0.5 + 0.5 * sin(fi * 2.1 + t));
  }
  thread = clamp(thread, 0.0, 1.0);

  vec3 goldThread = vec3(0.85, 0.65, 0.25) * thread;
  vec3 goldHighlight = vec3(1.0, 0.9, 0.6) * pow(thread, 3.0);
  vec3 bg = vec3(0.02, 0.015, 0.01);

  vec3 col = bg + goldThread * 0.5 + goldHighlight * 0.3 + vec3(0.08, 0.05, 0.02) * audioLevel;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(col, vec3(0.95, 1.0, 1.08));

  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTexture, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}