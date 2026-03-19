/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Retro synthwave neon grid receding into the horizon",
  "INPUTS": [
    {"NAME": "driveSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Drive Speed"},
    {"NAME": "neonIntensity", "TYPE": "float", "DEFAULT": 1, "MIN": 0.0, "MAX": 2.0, "LABEL": "Neon Intensity"},
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
  float t = TIME * driveSpeed;
  float p1Val = driveSpeed;
  float p2Val = neonIntensity;

  vec2 gp = uv; gp.y = 1.0 / (max(uv.y + 0.3, 0.01)); gp.x *= gp.y * 0.5; gp.y -= t * 2.0; float gridX = smoothstep(0.04, 0.0, abs(fract(gp.x) - 0.5) - 0.47); float gridY = smoothstep(0.04, 0.0, abs(fract(gp.y * 0.5) - 0.5) - 0.47); float grid = max(gridX, gridY) * smoothstep(-0.3, 0.0, uv.y); vec3 col = vec3(0.01, 0.0, 0.03); col += vec3(0.8, 0.2, 0.8) * grid * p2Val * 0.5; float sun = smoothstep(0.15, 0.1, length(uv - vec2(0.0, 0.25))); col += vec3(1.0, 0.4, 0.1) * sun; col += vec3(0.1, 0.02, 0.1) * audioBass;

  float vig = 1.0 - smoothstep(0.4, 1.2, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}