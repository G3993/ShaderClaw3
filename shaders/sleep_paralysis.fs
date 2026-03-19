/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Layered shadow figures and fog evoking the sensation of sleep paralysis",
  "INPUTS": [
    {"NAME": "driftSpeed", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "LABEL": "Drift Speed"},
    {"NAME": "shadowDensity", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.5, "LABEL": "Shadow Density"},
    {"NAME": "audioLevel", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Level"},
    {"NAME": "audioBass", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Bass"}
  ]
}*/

precision highp float;

float hash(vec2 p) {
  return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float vnoise(vec2 p) {
  vec2 i = floor(p); vec2 f = fract(p);
  f = f * f * (3.0 - 2.0 * f);
  return mix(mix(hash(i), hash(i+vec2(1,0)), f.x), mix(hash(i+vec2(0,1)), hash(i+vec2(1,1)), f.x), f.y);
}

float fbm(vec2 p) {
  float v = 0.0; float a = 0.5;
  mat2 rot = mat2(0.8, 0.6, -0.6, 0.8);
  for (int i = 0; i < 5; i++) {
    v += a * vnoise(p); p = rot * p * 2.1; a *= 0.5;
  }
  return v;
}

float warpedFbm(vec2 p, float t) {
  vec2 q = vec2(fbm(p + t * 0.05), fbm(p + vec2(5.2, 1.3) + t * 0.04));
  vec2 r = vec2(fbm(p + 3.0 * q + vec2(1.7, 9.2)), fbm(p + 3.0 * q + vec2(8.3, 2.8)));
  return fbm(p + 2.5 * r);
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 p = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME * driftSpeed;
  float sd = shadowDensity + audioBass * 0.3;

  // Breathing pulsation
  float breath = 0.5 + 0.5 * sin(t * 0.4);

  // Multiple shadow layers at different depths
  float shadow = 0.0;
  for (int i = 0; i < 5; i++) {
    float fi = float(i);
    float depth = fi * 0.2;
    float speed = 0.3 - depth * 0.2;
    vec2 offset = vec2(sin(t * speed + fi * 1.3), cos(t * speed * 0.7 + fi * 2.1)) * 0.3;
    float s = warpedFbm(p * (1.0 + fi * 0.3) + offset, t * (1.0 - depth * 0.5));
    s = smoothstep(0.3 - sd * 0.1, 0.6, s);
    shadow += s * (0.5 - depth * 0.08) * breath;
  }
  shadow = clamp(shadow, 0.0, 1.0);

  // Eye-like glints
  float eyes = 0.0;
  for (int i = 0; i < 3; i++) {
    float fi = float(i);
    vec2 eyePos = vec2(sin(t * 0.15 + fi * 2.1) * 0.3, cos(t * 0.12 + fi * 1.7) * 0.2 + 0.1);
    float eyeGlow = exp(-length(p - eyePos) * 20.0) * 0.5;
    eyeGlow *= smoothstep(0.3, 0.7, sin(t * 0.5 + fi * 3.1));
    eyes += eyeGlow;
  }

  // Color: warm desaturated browns and deep blacks
  vec3 fogColor = vec3(0.06, 0.04, 0.03) * (1.0 + audioLevel * 0.3);
  vec3 shadowColor = vec3(0.01, 0.008, 0.005);
  vec3 eyeColor = vec3(0.4, 0.25, 0.1);

  vec3 col = mix(fogColor, shadowColor, shadow);
  col += eyeColor * eyes;

  // Grain
  col += (hash(gl_FragCoord.xy + fract(TIME) * 100.0) - 0.5) * 0.015;

  // Vignette
  float vig = 1.0 - smoothstep(0.3, 1.0, length(p));
  col *= 0.4 + 0.6 * vig;

  col = pow(max(col, 0.0), vec3(0.95));
  gl_FragColor = vec4(col, 1.0);
}
