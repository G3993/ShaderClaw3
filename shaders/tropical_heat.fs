/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Tropical heat shimmer with domain-warped noise, chromatic aberration and vivid warm palette",
  "INPUTS": [
    {"NAME": "heatIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Heat Intensity"},
    {"NAME": "colorVibrancy", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Color Vibrancy"},
    {"NAME": "audioLevel", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Level"},
    {"NAME": "audioBass", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Bass"}
  ]
}*/

precision highp float;

vec3 mod289(vec3 x) { return x - floor(x * (1.0/289.0)) * 289.0; }
vec2 mod289v2(vec2 x) { return x - floor(x * (1.0/289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x * 34.0) + 1.0) * x); }
float snoise(vec2 v) {
  const vec4 C = vec4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
  vec2 i = floor(v + dot(v, C.yy)); vec2 x0 = v - i + dot(i, C.xx);
  vec2 i1 = (x0.x > x0.y) ? vec2(1,0) : vec2(0,1);
  vec4 x12 = x0.xyxy + C.xxzz; x12.xy -= i1; i = mod289v2(i);
  vec3 p = permute(permute(i.y + vec3(0, i1.y, 1)) + i.x + vec3(0, i1.x, 1));
  vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m; m = m*m;
  vec3 x = 2.0 * fract(p * C.www) - 1.0; vec3 h = abs(x) - 0.5;
  vec3 ox = floor(x + 0.5); vec3 a0 = x - ox;
  m *= 1.79284291400159 - 0.85373472095314 * (a0*a0 + h*h);
  vec3 g; g.x = a0.x * x0.x + h.x * x0.y; g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return 130.0 * dot(m, g);
}

float fbm(vec2 p, float t) {
  float v = 0.0, a = 0.5;
  for (int i = 0; i < 6; i++) { v += a * snoise(p + t * 0.25); p = p * 2.05 + vec2(1.7, 9.2); a *= 0.5; }
  return v;
}

float warpedFbm(vec2 p, float t) {
  vec2 q = vec2(fbm(p, t), fbm(p + vec2(5.2, 1.3), t));
  vec2 r = vec2(fbm(p + 3.0 * q + vec2(1.7, 9.2), t * 1.15), fbm(p + 3.0 * q + vec2(8.3, 2.8), t * 1.15));
  return fbm(p + 2.5 * r, t * 0.9);
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 p = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME;
  float hi = heatIntensity + audioBass * 0.5;
  float cv = colorVibrancy + audioLevel * 0.3;

  vec2 distort = vec2(
    snoise(vec2(uv.x * 4.0 + t * 0.8, uv.y * 7.0 - t * 1.5)) * 0.4 * hi * 0.025,
    snoise(vec2(uv.x * 3.0, uv.y * 6.0 - t * 1.8)) * 0.5 * hi * 0.018
  );

  float ab = hi * 0.012;
  vec2 pR = p + distort * 1.3 + vec2(ab, ab * 0.5);
  vec2 pG = p + distort;
  vec2 pB = p + distort * 0.7 - vec2(ab * 0.8, ab * 0.3);

  vec3 col;
  col.r = (0.55 + 0.45 * cos(6.28 * (warpedFbm(pR * 1.5, t * 0.3) * 0.8 + t * 0.05)));
  col.g = (0.55 + 0.45 * cos(6.28 * (warpedFbm(pG * 1.5, t * 0.3 + 0.7) * 0.8 + t * 0.05 + 0.33)));
  col.b = (0.55 + 0.45 * cos(6.28 * (warpedFbm(pB * 1.5, t * 0.3 + 1.4) * 0.8 + t * 0.05 + 0.66)));

  float lum = dot(col, vec3(0.299, 0.587, 0.114));
  col = mix(vec3(lum), col, 1.0 + cv * 0.6);

  float bloomTime = pow(sin(t * 0.4) * 0.5 + 0.5, 6.0);
  vec2 bc = vec2(snoise(vec2(t * 0.13, 0.0)) * 0.4, snoise(vec2(0.0, t * 0.11 + 3.0)) * 0.4);
  col += vec3(0.95, 0.4, 0.2) * bloomTime * smoothstep(0.5, 0.0, length(p - bc)) * 0.7 * cv;

  col = mix(col, vec3(0.78, 0.58, 0.42) * lum, 0.12);
  col *= pow(clamp(1.0 - dot(p, p) * 0.5, 0.0, 1.0), 0.7);
  col = col / (1.0 + col * 0.25);
  col = pow(col, vec3(0.95)) * vec3(1.05, 0.97, 0.88);

  gl_FragColor = vec4(col, 1.0);
}
