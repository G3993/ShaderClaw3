/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Cosmic stardust particles drifting through a warm nebula veil",
  "INPUTS": [
    {"NAME": "driftSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Drift Speed"},
    {"NAME": "starDensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Star Density"},
    {"NAME": "audioLevel", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Level"},
    {"NAME": "audioBass", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Bass"}
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
  float t = TIME * driftSpeed;
  float sd = starDensity + audioLevel * 0.5;

  float nebula = fbm(uv * 2.0 + vec2(t * 0.1, t * 0.07));
  float nebula2 = fbm(uv * 3.0 + vec2(-t * 0.08, t * 0.12) + 50.0);
  vec3 nebulaCol = mix(vec3(0.15, 0.05, 0.1), vec3(0.3, 0.15, 0.05), nebula);
  nebulaCol += vec3(0.05, 0.1, 0.15) * nebula2;

  float stars = 0.0;
  for (int i = 0; i < 3; i++) {
    float fi = float(i);
    vec2 st = fract(uv * (20.0 + fi * 15.0) * sd + t * (0.1 + fi * 0.05)) - 0.5;
    float d = length(st);
    float sparkle = 1.0 + 0.5 * sin(TIME * (3.0 + fi * 2.0) + hash(floor(uv * (20.0 + fi * 15.0) * sd)) * 6.28);
    stars += smoothstep(0.03 / (1.0 + fi * 0.5), 0.0, d) * sparkle;
  }

  vec3 col = nebulaCol * 0.3 + vec3(0.02, 0.01, 0.015);
  col += vec3(1.0, 0.9, 0.7) * stars * 0.5;
  col += vec3(0.1, 0.06, 0.03) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.5 + 0.5 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}