/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Luxurious draped silk fabric with flowing folds, specular highlights and warm color",
  "INPUTS": [
    {"NAME": "flowSpeed", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0, "LABEL": "Flow Speed"},
    {"NAME": "foldDepth", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Fold Depth"},
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
  vec2 i = floor(v + dot(v, C.yy)), x0 = v - i + dot(i, C.xx);
  vec2 i1 = (x0.x > x0.y) ? vec2(1,0) : vec2(0,1);
  vec4 x12 = x0.xyxy + C.xxzz; x12.xy -= i1; i = mod289v2(i);
  vec3 p = permute(permute(i.y + vec3(0, i1.y, 1)) + i.x + vec3(0, i1.x, 1));
  vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m*m*m;
  vec3 x = 2.0 * fract(p * C.www) - 1.0, h = abs(x) - 0.5, ox = floor(x + 0.5), a0 = x - ox;
  m *= 1.79284291400159 - 0.85373472095314 * (a0*a0 + h*h);
  vec3 g; g.x = a0.x * x0.x + h.x * x0.y; g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return 130.0 * dot(m, g);
}

float silkHeight(vec2 uv, float t) {
  float ca = cos(0.45), sa = sin(0.45);
  vec2 fuv = vec2(ca * uv.x + sa * uv.y, -sa * uv.x + ca * uv.y);
  float folds = sin(fuv.y * 4.0 + t * 0.3) * 0.30 + sin(fuv.y * 6.5 + t * 0.45 + 1.0) * 0.16 + sin(fuv.y * 10.0 + t * 0.55 + 2.5) * 0.07;
  folds *= sin(fuv.x * 0.8 + t * 0.15) * 0.4 + 0.6;
  float billow = sin(uv.x * 0.9 + uv.y * 0.6 + t * 0.2) * 0.15 + sin(uv.x * 0.5 - uv.y * 1.1 + t * 0.18 + 2.0) * 0.12;
  return folds + billow + snoise(uv * 0.3 + t * 0.04) * 0.1;
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  float aspect = RENDERSIZE.x / RENDERSIZE.y;
  vec2 p = vec2((uv.x - 0.5) * aspect, uv.y - 0.5) * 3.2;
  float t = TIME * flowSpeed;
  float fd = foldDepth + audioBass * 0.5;

  vec2 warp = vec2(snoise(p * 0.18 + t * 0.05), snoise(p * 0.18 + t * 0.04 + vec2(8.3, 0.0))) * 0.25;
  vec2 sp = p + warp;
  float h = silkHeight(sp, t) * fd;
  float eps = 0.003;
  vec3 N = normalize(vec3(h - silkHeight(sp + vec2(eps, 0.0), t) * fd, h - silkHeight(sp + vec2(0.0, eps), t) * fd, eps));

  vec3 L1 = normalize(vec3(0.3, 0.5, 0.75)), V = vec3(0, 0, 1), H1 = normalize(L1 + V);
  float d1 = max((dot(N, L1) + 0.3) / 1.3, 0.0);
  float sheen = pow(max(dot(N, H1), 0.0), 3.0) * 0.4;
  float spec = pow(max(dot(N, H1), 0.0), 18.0);
  float spark = pow(max(dot(N, H1), 0.0), 90.0);
  float fresnel = pow(1.0 - max(dot(N, V), 0.0), 3.0);

  float cm1 = smoothstep(-0.3, 0.4, h);
  vec3 baseColor = mix(vec3(0.32, 0.07, 0.09), vec3(0.45, 0.22, 0.08), cm1);
  baseColor = mix(baseColor, vec3(0.70, 0.50, 0.16), (snoise(sp * 0.25 + t * 0.02) * 0.5 + 0.5) * 0.5);

  vec3 col = baseColor * 0.15 + baseColor * d1 * 0.5 * vec3(1.0, 0.87, 0.58);
  col += baseColor * sheen * vec3(1.0, 0.87, 0.58);
  col += vec3(1.0, 0.88, 0.55) * spec * 0.6 + vec3(1.0, 0.94, 0.75) * spark * 0.5;
  col += vec3(1.0, 0.94, 0.75) * fresnel * 0.1;
  col += vec3(0.1, 0.06, 0.02) * audioLevel;

  col *= 1.0 - smoothstep(0.05, -0.25, h) * 0.5;
  col = mix(col, vec3(0.05, 0.02, 0.025), smoothstep(-0.1, -0.45, h) * 0.45);

  col *= smoothstep(0.0, 1.0, 1.0 - dot(uv - 0.5, uv - 0.5) * 1.4);
  col = col / (col + vec3(0.6));
  col = pow(col, vec3(0.9));
  col = max(col, vec3(0.018, 0.015, 0.013));

  gl_FragColor = vec4(col, 1.0);
}