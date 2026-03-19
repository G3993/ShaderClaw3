/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Sacred geometry mandala with golden ratio spirals and fractal hexagonal detail",
  "INPUTS": [
    {
      "NAME": "rotationSpeed",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Rotation Speed"
    },
    {
      "NAME": "complexity",
      "TYPE": "float",
      "DEFAULT": 5,
      "MIN": 2,
      "MAX": 8,
      "LABEL": "Complexity"
    },
    {
      "NAME": "baseColor",
      "LABEL": "Color",
      "TYPE": "color",
      "DEFAULT": [
        0.91,
        0.25,
        0.34,
        1
      ]
    },
    {
      "NAME": "inputTex",
      "LABEL": "Texture",
      "TYPE": "image"
    }
  ]
}*/

precision highp float;

#define PI 3.14159265359
#define TAU 6.28318530718
#define PHI 1.6180339887

mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

vec3 gold(float t) {
  return vec3(0.45, 0.32, 0.14) + vec3(0.45, 0.35, 0.2) * cos(TAU * (vec3(1.0, 0.8, 0.5) * t + vec3(0.0, 0.1, 0.25)));
}

float hexDist(vec2 p) { p = abs(p); return max(p.x + p.y * 0.577350269, p.y * 1.154700538); }

float mandalaLayer(vec2 uv, float time, float layer, float total, float rs) {
  float t = layer / total;
  float radius = 0.08 + t * 0.38;
  float dir = mod(layer, 2.0) < 1.0 ? 1.0 : -1.0;
  vec2 p = rot(time * rs * (1.5 - t * 1.2) * dir + layer * PHI) * uv;
  float r = length(p);
  float sym = 6.0 + floor(layer * 1.5);
  float a = mod(atan(p.y, p.x) + PI / sym, TAU / sym) - PI / sym;
  vec2 sp = vec2(cos(a), sin(a)) * r;
  float d = abs(r - radius) - 0.003 * (1.0 + t);
  d = min(d, abs(length(sp - vec2(radius, 0.0)) - radius * 0.35 / PHI) - 0.002);
  d = min(d, abs(length(sp - vec2(radius * 0.65, 0.0)) - radius * 0.25) - 0.0015);
  d = min(d, hexDist(sp - vec2(radius, 0.0)) - (0.012 + t * 0.008));
  return d;
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME;
  float rs = rotationSpeed + audioBass * 0.2;
  float comp = complexity + audioLevel * 2.0;
  float r = length(uv);

  vec3 col = vec3(0.0);
  for (int i = 0; i < 8; i++) {
    if (float(i) >= comp) break;
    float fi = float(i);
    float d = mandalaLayer(uv, t, fi, comp, rs);
    float glow = 0.0025 / (abs(d) + 0.0025) + 0.008 / (abs(d) + 0.008) * 0.3;
    col += gold(fi / comp + t * 0.02) * glow * (1.0 - fi / comp * 0.5) * 0.6;
  }

  col += vec3(1.0, 0.92, 0.7) * 0.01 / (r * r + 0.01) * (0.8 + 0.2 * sin(t * 1.5)) * 0.08;
  col += gold(0.5 + t * 0.015) * (0.004 / (abs(abs(r - 0.47) - 0.003) + 0.004)) * 0.35;
  col *= smoothstep(0.48, 0.336, r);
  col += vec3(0.3, 0.2, 0.08) * exp(-r * r * 6.0) * 0.06;
  col = col / (1.0 + col * 0.4);
  col = pow(col, vec3(0.95, 0.98, 1.05));

  col *= baseColor.rgb;
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, col * texSample.rgb, texSample.a * 0.5);

  gl_FragColor = vec4(col, 1.0);
}
