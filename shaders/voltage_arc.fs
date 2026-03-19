/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Electric arcs crackling between orbiting conductor points with volumetric glow",
  "INPUTS": [
    {
      "NAME": "arcIntensity",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Arc Intensity"
    },
    {
      "NAME": "crackleSpeed",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 3,
      "LABEL": "Crackle Speed"
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
float arcNoise(vec2 p) { return vnoise(p) * 0.6 + vnoise(p * 2.3 + 17.1) * 0.3 + vnoise(p * 5.7 + 43.2) * 0.1; }

vec2 conductor(int idx, float t) {
  float fi = float(idx);
  float angle = fi * 1.571 + t * (0.12 + fi * 0.03);
  return vec2(sin(fi * 2.7 + 0.5) * 0.1 + cos(angle) * (0.25 + fi * 0.05),
              cos(fi * 1.9 + 1.3) * 0.08 + sin(angle * 1.3 + fi * 0.8) * (0.18 + fi * 0.04));
}

float electricArc(vec2 uv, vec2 a, vec2 b, float t, float seed) {
  vec2 ab = b - a; float len = length(ab);
  if (len < 0.001) return 0.0;
  vec2 dir = ab / len, perp = vec2(-dir.y, dir.x);
  float param = clamp(dot(uv - a, dir) / len, 0.0, 1.0);
  float noiseT = t * crackleSpeed * 8.0 + seed * 100.0;
  float disp = (arcNoise(vec2(param * 6.0 + seed * 37.0, noiseT)) - 0.5) * 0.12;
  float taper = min(param * (1.0 - param) * 4.0, 1.0);
  disp *= taper;
  vec2 arcPt = a + dir * (param * len) + perp * disp;
  float d = length(uv - arcPt);
  float pulse = (0.7 + 0.3 * sin(t * 1.5 + seed * 5.0)) * (0.85 + 0.15 * sin(noiseT * 13.0));
  return (min(0.004 / (d * d + 0.00006), 12.0) * 0.5 + min(0.002 / (d + 0.005), 1.5) * 0.15) * pulse * taper;
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME;
  float intensity = arcIntensity + audioLevel * 0.5;

  vec3 bg = vec3(0.031, 0.024, 0.016) + vec3(vnoise(uv * 3.0 + t * 0.05) * 0.006);
  vec2 c0 = conductor(0, t), c1 = conductor(1, t), c2 = conductor(2, t), c3 = conductor(3, t);

  if (mousePos.x > 0.0 || mousePos.y > 0.0) {
    vec2 mp = (mousePos - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    c0 = mix(c0, mp, 0.3); c1 = mix(c1, mp, 0.3); c2 = mix(c2, mp, 0.3); c3 = mix(c3, mp, 0.3);
  }

  float totalArc = electricArc(uv, c0, c1, t, 1.0) + electricArc(uv, c1, c2, t, 2.7) +
                   electricArc(uv, c2, c3, t, 4.3) + electricArc(uv, c3, c0, t, 6.1);
  totalArc += electricArc(uv, c0, c2, t, 8.5) * smoothstep(0.3, 0.7, sin(t * 0.4 + 1.0) * 0.5 + 0.5);
  totalArc += audioBass * 0.5;

  float arcVal = totalArc * intensity;
  vec3 col = bg;
  col += vec3(1.0, 0.6, 0.2) * smoothstep(0.0, 0.3, arcVal) * 0.3;
  col += vec3(1.8, 1.2, 0.55) * smoothstep(0.2, 1.0, arcVal) * 0.5;
  col += vec3(3.0, 2.8, 2.4) * smoothstep(0.8, 2.0, arcVal) * 0.8;
  col += vec3(2.5, 2.3, 2.0) * smoothstep(2.0, 4.0, arcVal) * 0.5;

  for (int i = 0; i < 4; i++) {
    vec2 cp = (i == 0) ? c0 : (i == 1) ? c1 : (i == 2) ? c2 : c3;
    float d = length(uv - cp);
    col += vec3(1.8, 1.2, 0.55) * min(0.0006 / (d * d + 0.00005), 6.0) * 0.15;
  }

  col += (hash(gl_FragCoord.xy + fract(t * 43.0) * 1000.0) - 0.5) * 0.015;
  col *= 0.75 + smoothstep(1.2, 0.5, length(uv * vec2(0.9, 1.0))) * 0.25;
  col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);
  col = pow(max(col, vec3(0.0)), vec3(0.92, 0.97, 1.05));

  gl_FragColor = vec4(col, 1.0);
}