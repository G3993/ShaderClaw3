/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Animated Islamic geometric art with multi-layered star tessellations",
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
      "DEFAULT": 1,
      "MIN": 0.3,
      "MAX": 2,
      "LABEL": "Complexity"
    },
    {
      "NAME": "pattern",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Pattern Blend"
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
#define SQRT3 1.7320508

mat2 rot(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

vec2 polarFold(vec2 p, float n) {
  float angle = atan(p.y, p.x);
  float sector = TAU / n;
  angle = abs(mod(angle + sector * 0.5, sector) - sector * 0.5);
  return vec2(cos(angle), sin(angle)) * length(p);
}

float sdSegment(vec2 p, vec2 a, vec2 b) {
  vec2 pa = p - a, ba = b - a;
  return length(pa - ba * clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0));
}

vec3 glowLine(float d, vec3 core, vec3 bloom, float lw, float bw) {
  return core * pow(lw / (abs(d) + lw), 2.0) + bloom * pow(bw / (abs(d) + bw), 1.5) * 0.4;
}

float starPattern6(vec2 p, float scale, float time) {
  p *= scale;
  vec2 fp = polarFold(p, 12.0);
  float br = 1.0 + 0.03 * sin(time * 0.7);
  float d = min(sdSegment(fp, vec2(0.5*br, 0.0), vec2(0.35*br, 0.15*br)),
            min(sdSegment(fp, vec2(0.35*br, 0.15*br), vec2(0.22*br, 0.0)),
            min(sdSegment(fp, vec2(0.22*br, 0.0), vec2(0.12*br, 0.07*br)),
                sdSegment(fp, vec2(0.12*br, 0.07*br), vec2(0.0)))));
  d = min(d, abs(length(p) - 0.5*br));
  d = min(d, abs(length(p) - 0.22*br));
  return d / scale;
}

float hexTileStars(vec2 p, float scale, float time, float rotAngle) {
  p = rot(rotAngle) * p; p *= scale;
  vec2 s = vec2(1.0, SQRT3), h = s * 0.5;
  vec2 a = mod(p, s) - h, b = mod(p - h, s) - h;
  return starPattern6(dot(a,a) < dot(b,b) ? a : b, 1.8, time) / scale;
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME;
  float rs = rotationSpeed + audioBass * 0.2;
  float comp = complexity + audioLevel * 0.3;

  vec3 col = vec3(0.02, 0.015, 0.01);
  vec3 dimGold = vec3(0.25, 0.18, 0.08);
  vec3 medGold = vec3(0.55, 0.40, 0.18);
  vec3 brightGold = vec3(0.78, 0.58, 0.24);

  col += glowLine(hexTileStars(uv, 2.5 * comp, t, t * rs * 0.04), medGold * 0.7, dimGold, 0.0015, 0.012);
  col += glowLine(hexTileStars(uv, 3.5 * comp, t, -t * rs * 0.06), brightGold * 0.6, dimGold * 0.8, 0.0012, 0.010);
  col += glowLine(hexTileStars(uv, 3.0 * comp, t, -t * rs * 0.035), medGold * 0.45, dimGold * 0.5, 0.001, 0.008);

  float r = length(uv);
  col += medGold * exp(-r * r * 4.0) * 0.08;
  col *= 0.92 + 0.08 * sin(t * 0.3);
  col *= 0.5 + max(1.0 - r * r * 0.6, 0.0) * 0.5;
  col = col / (1.0 + col * 0.3);
  col = pow(col, vec3(0.95, 0.98, 1.06));

  col *= baseColor.rgb;
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, col * texSample.rgb, texSample.a * 0.5);

  gl_FragColor = vec4(col, 1.0);
}
