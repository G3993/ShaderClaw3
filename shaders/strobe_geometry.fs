/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Sharp geometric shapes flash with glowing afterimage silhouettes that fade cyan to magenta to dark",
  "INPUTS": [
    {
      "NAME": "flashRate",
      "TYPE": "float",
      "DEFAULT": 0.7,
      "MIN": 0.1,
      "MAX": 2,
      "LABEL": "Flash Rate"
    },
    {
      "NAME": "glowIntensity",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Glow Intensity"
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

#define PI 3.141592653589793
#define TAU 6.283185307179586
#define MAX_SHAPES 8

vec3 ACESFilm(vec3 x) {
  float a = 2.51; float b = 0.03; float c = 2.43; float d = 0.59; float e = 0.14;
  return clamp((x * (a * x + b)) / (x * (c * x + d) + e), 0.0, 1.0);
}

float hash(float n) { return fract(sin(n) * 43758.5453123); }
vec2 hash2(float n) { return vec2(hash(n), hash(n + 7.31)); }

float vnoise(vec2 p) {
  vec2 i = floor(p); vec2 f = fract(p);
  f = f * f * (3.0 - 2.0 * f);
  float n = i.x + i.y * 57.0;
  return mix(mix(hash(n), hash(n + 1.0), f.x), mix(hash(n + 57.0), hash(n + 58.0), f.x), f.y);
}

mat2 rot2(float a) { float c = cos(a); float s = sin(a); return mat2(c, -s, s, c); }

float sdTriangle(vec2 p, float r) {
  float k = sqrt(3.0);
  p.x = abs(p.x) - r; p.y = p.y + r / k;
  if (p.x + k * p.y > 0.0) p = vec2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
  p.x -= clamp(p.x, -2.0 * r, 0.0);
  return -length(p) * sign(p.y);
}

float sdBox(vec2 p, vec2 b) {
  vec2 d = abs(p) - b;
  return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

float sdRhombus(vec2 p, vec2 b) {
  vec2 q = abs(p);
  float h = clamp((-2.0 * dot(q, b) + dot(b, b)) / dot(b, b), -1.0, 1.0);
  float d = length(q - 0.5 * b * vec2(1.0 - h, 1.0 + h));
  return d * sign(q.x * b.y + q.y * b.x - b.x * b.y);
}

float sdHexagon(vec2 p, float r) {
  vec2 q = abs(p);
  float d = dot(q, normalize(vec2(1.0, 1.732)));
  return max(d, q.y) - r;
}

float sdTrapezoid(vec2 p, float r1, float r2, float he) {
  vec2 k1 = vec2(r2, he); vec2 k2 = vec2(r2 - r1, 2.0 * he);
  p.x = abs(p.x);
  vec2 ca = vec2(max(0.0, p.x - ((p.y < 0.0) ? r1 : r2)), abs(p.y) - he);
  vec2 cb = p - k1 + k2 * clamp(dot(k1 - p, k2) / dot(k2, k2), 0.0, 1.0);
  float s = (cb.x < 0.0 && ca.y < 0.0) ? -1.0 : 1.0;
  return s * sqrt(min(dot(ca, ca), dot(cb, cb)));
}

float evalShape(vec2 p, int type, float scale) {
  float d = 1e5;
  if (type == 0) d = sdTriangle(p, scale * 0.36);
  else if (type == 1) d = sdBox(p, vec2(scale * 0.38, scale * 0.14));
  else if (type == 2) d = sdRhombus(p, vec2(scale * 0.12, scale * 0.40));
  else if (type == 3) d = sdHexagon(p, scale * 0.25);
  else if (type == 4) d = sdTrapezoid(p, scale * 0.15, scale * 0.35, scale * 0.18);
  return d;
}

vec3 decayColor(float phase) {
  vec3 white = vec3(1.0); vec3 cyan = vec3(0.0, 0.95, 1.0);
  vec3 blue = vec3(0.15, 0.4, 1.0); vec3 magenta = vec3(0.95, 0.1, 0.85);
  vec3 purple = vec3(0.35, 0.05, 0.55); vec3 dark = vec3(0.05, 0.01, 0.1);
  vec3 col = dark;
  if (phase < 0.05) col = mix(white, white, phase / 0.05);
  else if (phase < 0.20) col = mix(white, cyan, (phase - 0.05) / 0.15);
  else if (phase < 0.40) col = mix(cyan, blue, (phase - 0.20) / 0.20);
  else if (phase < 0.65) col = mix(blue, magenta, (phase - 0.40) / 0.25);
  else if (phase < 0.85) col = mix(magenta, purple, (phase - 0.65) / 0.20);
  else col = mix(purple, dark, (phase - 0.85) / 0.15);
  return col;
}

void main() {
  vec2 fragCoord = gl_FragCoord.xy;
  vec2 uv = fragCoord / RENDERSIZE.xy;
  float aspect = RENDERSIZE.x / RENDERSIZE.y;
  vec2 p = (uv - 0.5) * vec2(aspect, 1.0);

  float t = TIME;
  float fr = flashRate * (1.0 + audioBass * 0.5);
  float decayDuration = 4.0;
  float cycleLen = float(MAX_SHAPES) * fr;
  float cycleTime = mod(t, cycleLen);

  vec3 bg = vec3(0.015, 0.012, 0.025);
  float bgNoise = vnoise(p * 3.0 + t * 0.05) * 0.4 + vnoise(p * 7.0 - t * 0.03) * 0.3;
  bg += vec3(0.008, 0.006, 0.015) * bgNoise;

  float currentShapeIdx = floor(cycleTime / fr);
  float timeSinceLastFlash = cycleTime - currentShapeIdx * fr;
  float bgPulse = exp(-timeSinceLastFlash * 6.0) * 0.12;
  bg += vec3(0.08, 0.06, 0.12) * bgPulse;

  vec3 col = bg;
  float gi = glowIntensity + audioLevel * 0.5;

  for (int i = 0; i < MAX_SHAPES; i++) {
    float fi = float(i);
    bool isStrong = (mod(fi, 2.0) < 0.5);
    float cycle = floor(t / cycleLen);
    float seed = fi * 13.37 + cycle * 97.31;
    float jitter = (hash(seed + 10.0) - 0.5) * 0.2 * fr;
    float birthT = fi * fr + jitter;
    float age = cycleTime - birthT;
    if (age < 0.0 || age > decayDuration) continue;
    float phase = age / decayDuration;

    vec2 center = (hash2(seed + 1.0) * 2.0 - 1.0) * vec2(0.65, 0.50);
    float baseAngle = floor(hash(seed + 2.0) * 8.0) * (PI / 4.0);
    float rotation = baseAngle + (hash(seed + 5.0) - 0.5) * (PI / 6.0);
    float baseScale = 0.4 + hash(seed + 3.0) * 1.2;
    float scale = isStrong ? baseScale * 1.3 : baseScale * 0.7;
    int type = int(mod(hash(seed + 4.0) * 5.0, 5.0));

    vec2 localP = p - center;
    localP = rot2(rotation) * localP;
    float d = evalShape(localP, type, scale);

    float flashPhase = clamp(age / 0.2, 0.0, 1.0);
    float isFlashing = 1.0 - flashPhase;
    float beatMul = isStrong ? 1.4 : 0.8;

    float fillAlpha = smoothstep(0.005, -0.005, d);
    float interiorDist = clamp(-d / (scale * 0.25), 0.0, 1.0);
    float edgeBrightness = 1.0 - interiorDist * 0.65;
    float fillFade = exp(-age * 2.5);
    float flashBright = isFlashing * 2.0 * beatMul + fillFade;
    vec3 fillColor = decayColor(phase) * flashBright * edgeBrightness;
    fillColor = mix(fillColor, vec3(2.5) * edgeBrightness, isFlashing * fillAlpha);
    col += fillColor * fillAlpha * (1.0 - phase * phase) * gi * 0.5;

    float edgeWidth = 0.006 + 0.003 * (1.0 - phase);
    float edge = smoothstep(edgeWidth, edgeWidth * 0.3, abs(d));
    float edgeFade = 1.0 - phase * phase * phase;
    col += decayColor(phase * 0.85) * edgeFade * edge * gi * 1.2;

    float bloomWidth = mix(0.015 + 0.015 * (1.0 - phase), isStrong ? 0.08 : 0.05, isFlashing);
    float bloom = exp(-abs(d) / bloomWidth);
    float bloomIntensity = mix(0.3, 1.8 * beatMul, isFlashing) * (1.0 - phase);
    col += decayColor(phase * 0.7) * bloom * bloomIntensity * gi * 0.4;
  }

  float scanline = sin(fragCoord.y * 1.5) * 0.5 + 0.5;
  col *= 0.92 + scanline * 0.08;

  vec2 vc = uv - 0.5;
  float vig = 1.0 - dot(vc, vc) * 1.8;
  col *= pow(clamp(vig, 0.0, 1.0), 0.5);

  float grain = (fract(sin(dot(fragCoord, vec2(12.9898, 78.233)) + fract(t * 0.1) * 100.0) * 43758.5453) - 0.5) * 0.025;
  col += grain;
  col = ACESFilm(col);
  col = pow(max(col, 0.0), vec3(0.95));

  col *= baseColor.rgb;
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, col * texSample.rgb, texSample.a * 0.5);

  gl_FragColor = vec4(col, 1.0);
}
