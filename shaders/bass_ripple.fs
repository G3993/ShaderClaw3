/*{
  "CATEGORIES": ["Radiant"],
  "DESCRIPTION": "Beat-synced metallic speaker grille with hexagonal mesh and wave displacement",
  "INPUTS": [
    {"NAME": "bassFreq", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 2.0, "LABEL": "Bass Frequency"},
    {"NAME": "bassIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Bass Intensity"},
    {"NAME": "mousePos", "TYPE": "point2D", "DEFAULT": [0, 0], "MIN": [0, 0], "MAX": [1, 1], "LABEL": "Mouse Position"},
    {"NAME": "audioLevel", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Level"},
    {"NAME": "audioBass", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Audio Bass"}
  ]
}*/

precision highp float;

#define PI 3.14159265359
#define TAU 6.28318530718

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float noise(vec2 p) {
  vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);
  return mix(mix(hash(i), hash(i + vec2(1,0)), f.x), mix(hash(i + vec2(0,1)), hash(i + vec2(1,1)), f.x), f.y);
}
mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

float displacement(vec2 p, float t, float bf, float bi) {
  float period = 1.0 / max(bf, 0.01);
  float beatFrac = fract(t / period);
  float envelope = exp(-beatFrac * 3.5);
  float prevEnv = exp(-(beatFrac + 1.0) * 3.5);
  float dist = length(p);
  float angle = atan(p.y, p.x);

  float wave1 = sin(dist * 14.0 - beatFrac * 22.0) * envelope;
  float wave2 = sin(dist * 9.0 - (beatFrac + 1.0) * 16.0) * prevEnv * 0.6;
  float standing = (sin(p.x * 18.0) * sin(p.y * 18.0)) * envelope * 0.3;
  float radialMode = sin(dist * 22.0) * cos(angle * 3.0) * envelope * 0.2;

  vec2 offCenter = vec2(0.3 * sin(t * 0.2), 0.25 * cos(t * 0.25));
  float wave6 = sin(length(p - offCenter) * 12.0 - beatFrac * 18.0) * envelope * 0.35;

  float mouseWave = 0.0;
  if (mousePos.x > 0.0 || mousePos.y > 0.0) {
    vec2 mn = (mousePos - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    float md = length(p - mn);
    mouseWave = sin(md * 16.0 - beatFrac * 20.0) * envelope * 0.7 / (1.0 + md * 3.0);
  }

  float dome = (1.0 - smoothstep(0.0, 1.0, dist)) * envelope * 0.8;
  float h = (wave1 + wave2 + standing + radialMode + wave6 + mouseWave + dome) * bi;
  float idle = sin(dist * 8.0 + t * 3.0) * 0.03 * (1.0 - envelope);
  return h + idle * bi;
}

vec3 calcNormal(vec2 p, float t, float bf, float bi, float hc) {
  float eps = 0.002;
  float hx = displacement(p + vec2(eps, 0.0), t, bf, bi);
  float hy = displacement(p + vec2(0.0, eps), t, bf, bi);
  return normalize(vec3(-(hx - hc) / eps * 0.35, -(hy - hc) / eps * 0.35, 1.0));
}

vec3 hexGrid(vec2 p, float scale) {
  p *= scale;
  vec2 r = vec2(1.0, 1.732); vec2 h = r * 0.5;
  vec2 a = mod(p, r) - h; vec2 b = mod(p - h, r) - h;
  vec2 g = dot(a,a) < dot(b,b) ? a : b;
  float edgeDist = 0.5 - max(abs(g.x) + abs(g.y) * 0.577, abs(g.y) * 1.155);
  return vec3(edgeDist, p - g);
}

float fresnel(float cosTheta, float f0) { return f0 + (1.0 - f0) * pow(1.0 - cosTheta, 5.0); }

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  float aspect = RENDERSIZE.x / RENDERSIZE.y;
  float t = TIME;
  vec2 cuv = vec2((uv.x - 0.5) * aspect, uv.y - 0.5);

  float bi = bassIntensity + audioBass * 1.5;
  float bf = bassFreq;

  vec2 meshUV = cuv + vec2(sin(t * 0.15) * 0.03, 0.08);
  meshUV.y = meshUV.y / 0.7;
  meshUV = rot2(0.06) * meshUV;

  float h = displacement(meshUV, t, bf, bi);
  vec3 N = calcNormal(meshUV, t, bf, bi, h);

  vec3 hex = hexGrid(meshUV + N.xy * 0.003, 45.0);
  float wire = 1.0 - smoothstep(0.0, 0.06, hex.x);
  float hole = smoothstep(0.06, 0.08, hex.x);

  vec3 V = normalize(vec3(-cuv.x * 0.3, -cuv.y * 0.3 + 0.2, 1.0));
  float period = 1.0 / max(bf, 0.01);
  float bEnv = exp(-fract(t / period) * 3.5);
  float NdV = max(dot(N, V), 0.0);

  vec3 L1 = normalize(vec3(0.4 + sin(t * 0.25) * 0.4, 0.6, 1.0));
  vec3 H1 = normalize(L1 + V);
  float spec1 = pow(max(dot(N, H1), 0.0), 180.0);
  vec3 lightCol1 = vec3(1.0, 0.82, 0.55);

  vec3 L2 = normalize(vec3(-0.8, 0.4, 0.8));
  vec3 H2 = normalize(L2 + V);
  float spec2 = pow(max(dot(N, H2), 0.0), 120.0);
  vec3 lightCol2 = vec3(0.85, 0.55, 0.3);

  vec3 baseColor = vec3(0.38, 0.32, 0.25) + vec3(0.035, 0.025, 0.015) * noise(meshUV * 30.0);
  float fres = fresnel(NdV, 0.75);

  vec3 diffuse = baseColor * (max(dot(N, L1), 0.0) * lightCol1 + max(dot(N, L2), 0.0) * lightCol2 * 0.5 + 0.18);
  vec3 specular = (spec1 * lightCol1 * 3.5 + spec2 * lightCol2 * 2.5) * fres;
  specular *= 1.0 + bEnv * bi;

  float rim = pow(1.0 - NdV, 4.0);
  vec3 wireCol = diffuse + specular + rim * vec3(0.9, 0.55, 0.2) * 0.5;

  vec3 holeCol = vec3(0.015, 0.01, 0.006);
  holeCol += vec3(0.2, 0.12, 0.04) * bEnv * 0.35 * bi;

  vec3 col = mix(holeCol, wireCol, wire);

  vec3 beatColor = mix(vec3(0.5, 0.3, 0.1), vec3(0.4, 0.2, 0.08), sin(t * 0.4) * 0.5 + 0.5);
  col += beatColor * bEnv * 0.03 * bi;

  vec2 vc = uv - 0.5;
  col *= smoothstep(0.0, 1.0, 1.0 - dot(vc, vc) * 2.0);
  col += (hash(gl_FragCoord.xy + fract(t * 7.3) * 100.0) - 0.5) * 0.018;
  col = col / (col + vec3(1.5));
  col = pow(max(col, vec3(0.0)), vec3(0.88));

  gl_FragColor = vec4(col, 1.0);
}
