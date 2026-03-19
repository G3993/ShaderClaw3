/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Moonlit water surface with interfering ripple waves and specular reflections",
  "INPUTS": [
    {
      "NAME": "rippleSpeed",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Ripple Speed"
    },
    {
      "NAME": "moonGlow",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Moon Glow"
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
#define NUM_SOURCES 4

vec2 ripplePos(int i, float t) {
  float fi = float(i);
  float angle = fi * 1.571 + t * 0.06 * (0.7 + fi * 0.08);
  float r = 0.15 + fi * 0.12;
  return vec2(cos(angle) * r, sin(angle) * r * 0.5 - 0.05);
}

void main() {
  float aspect = RENDERSIZE.x / RENDERSIZE.y;
  vec2 p = (gl_FragCoord.xy / RENDERSIZE.xy - 0.5) * vec2(aspect, 1.0);
  float t = TIME * rippleSpeed;

  float h = 0.0;
  for (int i = 0; i < NUM_SOURCES; i++) {
    vec2 src = ripplePos(i, TIME);
    float d = length(p - src);
    float freq = 40.0 + float(i) * 8.0;
    h += sin(d * freq - t * 5.0 - float(i) * 2.1) / (1.0 + d * 4.0);
  }
  h /= float(NUM_SOURCES) * 0.6;
  h += audioBass * sin(length(p) * 30.0 - t * 6.0) * 0.3;

  if (mousePos.x > 0.0 || mousePos.y > 0.0) {
    vec2 mp = (mousePos - 0.5) * vec2(aspect, 1.0);
    h += sin(length(p - mp) * 60.0 - t * 8.0) * exp(-length(p - mp) * 6.0) * 0.8;
  }

  float eps = 0.002;
  float hR = 0.0, hU = 0.0;
  for (int i = 0; i < NUM_SOURCES; i++) {
    vec2 src = ripplePos(i, TIME);
    float freq = 40.0 + float(i) * 8.0;
    hR += sin(length(p + vec2(eps, 0.0) - src) * freq - t * 5.0 - float(i) * 2.1) / (1.0 + length(p + vec2(eps, 0.0) - src) * 4.0);
    hU += sin(length(p + vec2(0.0, eps) - src) * freq - t * 5.0 - float(i) * 2.1) / (1.0 + length(p + vec2(0.0, eps) - src) * 4.0);
  }
  hR /= float(NUM_SOURCES) * 0.6; hU /= float(NUM_SOURCES) * 0.6;
  vec3 N = normalize(vec3(-(hR - h) / eps * 0.15, -(hU - h) / eps * 0.15, 1.0));

  vec2 moonPos = vec2(0.05, 0.30);
  vec3 halfDir = normalize(normalize(vec3(moonPos - p, 0.8)) + vec3(0, 0, 1));
  float spec = max(dot(N, halfDir), 0.0);

  vec3 waterCol = mix(vec3(0.03, 0.05, 0.12), vec3(0.01, 0.02, 0.06), smoothstep(-0.3, 0.4, p.y) * 0.6);
  vec3 col = waterCol * (0.6 + (0.5 + 0.5 * h) * 0.8);

  vec3 moonColor = vec3(1.0, 0.90, 0.70);
  float mg = moonGlow + audioLevel * 0.3;
  col += vec3(0.95, 0.75, 0.40) * pow(spec, 5.0) * 0.08 * mg;
  col += moonColor * pow(spec, 20.0) * 0.25 * mg;
  col += vec3(1.0, 0.95, 0.85) * pow(spec, 80.0) * 0.8 * mg;

  float moonDist = length(p - moonPos);
  col += moonColor * smoothstep(0.06, 0.04, moonDist) * 0.8 * mg;
  col += moonColor * exp(-moonDist * moonDist * 12.0) * 0.1 * mg;

  float vig = 1.0 - smoothstep(0.45, 1.3, length(p * vec2(0.8, 1.0)));
  col *= 0.55 + 0.45 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.92));

  col *= baseColor.rgb;
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, col * texSample.rgb, texSample.a * 0.5);

  gl_FragColor = vec4(col, 1.0);
}