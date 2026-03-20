/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Deep sea bioluminescent organisms pulsing in darkness",
  "INPUTS": [
    {
      "NAME": "pulseSpeed",
      "TYPE": "float",
      "DEFAULT": 0.4,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Pulse Speed"
    },
    {
      "NAME": "glowIntensity",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Glow Intensity"
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
  float t = TIME * pulseSpeed;
  float p1Val = pulseSpeed;
  float p2Val = glowIntensity;

  vec3 col = vec3(0.005, 0.01, 0.02); for (int i = 0; i < 8; i++) { float fi = float(i); float phase = fi * 1.618 * 2.0; vec2 center = vec2(sin(t * 0.2 + phase) * 0.5, cos(t * 0.15 + phase * 0.7) * 0.4); float d = length(uv - center); float pulse = sin(t * (1.0 + fi * 0.3) + phase) * 0.5 + 0.5; float glow = exp(-d * d * 20.0) * pulse * p2Val; vec3 bioCol = mix(vec3(0.0, 0.3, 0.5), vec3(0.0, 0.8, 0.4), fi / 8.0); col += bioCol * glow * 0.3; } col += vec3(0.0, 0.05, 0.03) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  // Texture as source — shader VFX processes the input content
    vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 texSample = texture2D(inputTex, texUV);
    if (texSample.a > 0.01) {
        // Blend: texture is the source, shader effect modulates it
        float effectStrength = max(col.r, max(col.g, col.b));
        col = mix(texSample.rgb, col, 0.5) * (0.5 + effectStrength * 0.5);
        col *= baseColor.rgb;
    } else {
        col *= baseColor.rgb;
    }
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}