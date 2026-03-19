/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Double domain-warped ink dissolution with organic branching tendrils",
  "INPUTS": [
    {
      "NAME": "spreadSpeed",
      "TYPE": "float",
      "DEFAULT": 0.4,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Spread Speed"
    },
    {
      "NAME": "tendrilDetail",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Tendril Detail"
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

vec3 mod289(vec3 x) { return x - floor(x * (1.0/289.0)) * 289.0; }
vec2 mod289v2(vec2 x) { return x - floor(x * (1.0/289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x * 34.0) + 1.0) * x); }

float snoise(vec2 v) {
  const vec4 C = vec4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
  vec2 i = floor(v + dot(v, C.yy));
  vec2 x0 = v - i + dot(i, C.xx);
  vec2 i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
  vec4 x12 = x0.xyxy + C.xxzz; x12.xy -= i1;
  i = mod289v2(i);
  vec3 p = permute(permute(i.y + vec3(0.0, i1.y, 1.0)) + i.x + vec3(0.0, i1.x, 1.0));
  vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m; m = m*m;
  vec3 x = 2.0 * fract(p * C.www) - 1.0;
  vec3 h = abs(x) - 0.5;
  vec3 ox = floor(x + 0.5);
  vec3 a0 = x - ox;
  m *= 1.79284291400159 - 0.85373472095314 * (a0*a0 + h*h);
  vec3 g;
  g.x = a0.x * x0.x + h.x * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return 130.0 * dot(m, g);
}

float fbm4(vec2 p, float dm) {
  float v = 0.0; float a = 0.55;
  mat2 rot = mat2(0.8, 0.6, -0.6, 0.8);
  v += a * snoise(p); a *= 0.45; p = rot * p * 2.02;
  v += a * snoise(p); a *= 0.45; p = rot * p * 2.03;
  v += a * snoise(p) * dm; a *= 0.4; p = rot * p * 2.01;
  v += a * snoise(p) * dm * 0.6;
  return v;
}

float fbm3(vec2 p) {
  float v = 0.0;
  mat2 rot = mat2(0.8, 0.6, -0.6, 0.8);
  v += 0.5 * snoise(p); p = rot * p * 2.02;
  v += 0.25 * snoise(p); p = rot * p * 2.03;
  v += 0.125 * snoise(p);
  return v;
}

float inkField(vec2 p, float t, float dm) {
  vec2 q = vec2(fbm4(p + vec2(0.0) + t * 0.04, dm), fbm4(p + vec2(5.2, 1.3) + t * 0.03, dm));
  vec2 r = vec2(fbm4(p + 2.5 * q + vec2(1.7, 9.2) + t * 0.022, dm), fbm4(p + 2.5 * q + vec2(8.3, 2.8) + t * 0.032, dm));
  return fbm4(p + 2.2 * r + t * 0.015, dm);
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME * spreadSpeed;
  float dm = tendrilDetail + audioBass * 0.5;

  float field = inkField(uv * 0.8, t, dm);

  vec2 center = vec2(0.0);
  if (mousePos.x > 0.0 || mousePos.y > 0.0) {
    center = (mousePos - 0.5) * 2.0;
  }
  vec2 uvShifted = uv - center * 0.6;
  float envelope = 0.0;
  float a1 = t * 0.05;
  envelope += smoothstep(0.95, 0.0, length(uvShifted - vec2(cos(a1)*0.15, sin(a1*0.7)*0.12)));
  float a2 = t * 0.04 + 2.2;
  envelope += smoothstep(0.85, 0.0, length(uvShifted - vec2(cos(a2)*0.25, sin(a2*0.6)*0.2)));
  envelope += smoothstep(0.65, 0.0, length(uvShifted)) * 0.6;
  envelope = clamp(envelope, 0.0, 1.0);

  float inkRaw = smoothstep(-0.2, 0.1, field);
  float ink = inkRaw * envelope;

  float fineField = fbm3(uv * 2.5 + vec2(t * 0.02, -t * 0.015));
  float fineTendril = smoothstep(-0.1, 0.12, fineField) * envelope;
  float combinedInk = max(ink, fineTendril * 0.35);

  float edgeRaw = combinedInk * (1.0 - combinedInk) * 4.0;
  float edgeSoft = smoothstep(0.05, 0.5, edgeRaw);
  float edgeHot = smoothstep(0.6, 1.0, edgeRaw);

  vec3 inkDark = vec3(0.02, 0.015, 0.01);
  vec3 amberDim = vec3(0.06, 0.035, 0.014);
  vec3 amberDeep = vec3(0.18, 0.10, 0.035);
  vec3 amberGold = vec3(0.78, 0.58, 0.42);
  vec3 amberBright = vec3(1.0, 0.82, 0.52);
  vec3 amberHot = vec3(1.0, 0.92, 0.72);

  float liqVar = 0.5 + 0.5 * fbm3(uv * 2.0 + t * 0.03);
  vec3 liquid = mix(amberDim, amberDeep, liqVar * 0.7);
  float c1 = 0.5 + 0.5 * snoise(uv * 6.0 + vec2(t * 0.05, -t * 0.035));
  float c2 = 0.5 + 0.5 * snoise(uv * 10.0 + vec2(-t * 0.03, t * 0.04));
  liquid += vec3(0.42, 0.28, 0.14) * c1 * c2 * 0.05 * (1.0 - combinedInk);

  vec3 col = mix(liquid, inkDark, combinedInk);
  col += amberDeep * edgeSoft * 0.7;
  col += amberGold * smoothstep(0.25, 0.8, edgeRaw) * 0.4;
  col += amberBright * edgeHot * 0.45;
  col += amberHot * edgeHot * edgeHot * 0.25;
  col += vec3(0.1, 0.06, 0.02) * audioLevel;

  float vig = 1.0 - smoothstep(0.35, 1.2, length(uv));
  col *= 0.5 + 0.5 * vig;
  col = pow(max(col, 0.0), vec3(0.93, 0.97, 1.04));

  col *= baseColor.rgb;
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, col * texSample.rgb, texSample.a * 0.5);

  gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
