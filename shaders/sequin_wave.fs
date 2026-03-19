/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Thousands of tiny reflective discs catching cascading light waves",
  "INPUTS": [
    {
      "NAME": "waveSpeed",
      "TYPE": "float",
      "DEFAULT": 0.8,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Wave Speed"
    },
    {
      "NAME": "sparkle",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Sparkle Intensity"
    },
    {
      "NAME": "baseColor",
      "LABEL": "Color",
      "TYPE": "color",
      "DEFAULT": [0.91, 0.25, 0.34, 1.0]
    },
    {
      "NAME": "texture",
      "LABEL": "Texture",
      "TYPE": "image"
    }
  ]
}*/

precision highp float;

#define PI 3.14159265359
#define TAU 6.28318530718
#define SQRT3 1.7320508

float hash21(vec2 p) {
  p = fract(p * vec2(233.34, 851.73));
  p += dot(p, p + 23.45);
  return fract(p.x * p.y);
}
vec2 hash22(vec2 p) {
  float n = hash21(p);
  return vec2(n, hash21(p + n * 47.0));
}

vec4 hexTile(vec2 p, float scale) {
  p *= scale;
  vec2 s = vec2(1.0, SQRT3); vec2 halfS = s * 0.5;
  vec2 aBase = floor(p / s); vec2 aLocal = mod(p, s) - halfS;
  vec2 pOff = p - halfS;
  vec2 bBase = floor(pOff / s); vec2 bLocal = mod(pOff, s) - halfS;
  float pick = step(dot(aLocal, aLocal), dot(bLocal, bLocal));
  return vec4(mix(bLocal, aLocal, pick), mix(bBase + vec2(0.5), aBase, pick));
}

float waveField(vec2 cp, float t) {
  float w = sin(dot(cp, vec2(0.7, 0.5)) * 3.5 - t * 2.8) * 0.35;
  w += sin(cp.x * 4.2 + t * 1.9) * 0.25;
  float r1 = length(cp - vec2(-0.3, 0.2));
  w += sin(r1 * 6.0 - t * 3.2) * 0.2 * smoothstep(1.2, 0.0, r1);
  w += sin(dot(cp, vec2(-0.4, 0.8)) * 2.8 - t * 1.5) * 0.2;
  return w;
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME * waveSpeed;
  float sequinScale = 38.0;
  vec4 hex = hexTile(uv, sequinScale);
  vec2 localPos = hex.xy; vec2 cellId = hex.zw;

  vec2 rnd = hash22(cellId);
  float sizeVar = 0.85 + rnd.x * 0.3;
  float baseTilt = (rnd.y - 0.5) * 0.15;
  float reflVar = 0.7 + rnd.x * 0.3;
  float phaseOff = rnd.y * TAU;

  float discRadius = 0.42 * sizeVar;
  float dist = length(localPos);
  float disc = smoothstep(discRadius, discRadius - 0.06, dist);
  float bevel = smoothstep(discRadius, discRadius - 0.04, dist) - smoothstep(discRadius - 0.04, discRadius - 0.08, dist);

  vec2 worldPos = cellId / sequinScale;
  float wave = waveField(worldPos, t);
  float shimmer = sin(t * 3.0 + phaseOff) * 0.04;
  float tiltAngle = wave * 0.85 + baseTilt + shimmer + audioBass * 0.3;

  float waveH = waveField(worldPos + vec2(0.01, 0.0), t);
  float waveV = waveField(worldPos + vec2(0.0, 0.01), t);
  float tiltDir = atan(waveV - wave, waveH - wave);

  float ct = cos(tiltAngle); float st = sin(tiltAngle);
  float cd = cos(tiltDir); float sd = sin(tiltDir);
  vec3 N = vec3(st * cd, st * sd, ct);
  vec3 L = normalize(vec3(0.4, 0.6, 0.9));
  vec3 V = vec3(0.0, 0.0, 1.0);
  vec3 R = reflect(-L, N);
  float spec = pow(max(dot(R, V), 0.0), 48.0) + pow(max(dot(R, V), 0.0), 8.0) * 0.15;
  spec *= reflVar * (sparkle + audioLevel * 0.5);

  vec3 darkSequin = vec3(0.02, 0.015, 0.01);
  vec3 copperMid = vec3(0.78, 0.58, 0.42);
  vec3 amberFlash = vec3(1.0, 0.82, 0.55);
  vec3 hotGold = vec3(1.0, 0.92, 0.72);

  float facing = cos(tiltAngle) * 0.5 + 0.5;
  vec3 ambient = mix(darkSequin, vec3(0.05, 0.035, 0.02), facing * 0.6);
  vec3 sequinColor = ambient;
  sequinColor += copperMid * pow(max(facing, 0.0), 3.0) * 0.2 * reflVar;
  sequinColor += copperMid * smoothstep(0.0, 0.3, spec) * 0.5;
  sequinColor += amberFlash * smoothstep(0.3, 0.8, spec) * 0.8;
  sequinColor += hotGold * smoothstep(0.7, 1.0, spec) * 1.2;
  sequinColor += copperMid * bevel * facing * 0.3;

  vec3 col = mix(vec3(0.012, 0.008, 0.005), sequinColor, disc);
  col *= 0.85 + 0.15 * dot(normalize(uv + vec2(0.0001)), vec2(0.4, 0.6));

  float vig = 1.0 - smoothstep(0.4, 1.3, length(uv));
  col *= 0.6 + 0.4 * vig;
  col = pow(max(col, vec3(0.0)), vec3(0.93, 0.97, 1.04));

  col *= baseColor.rgb;
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
  vec4 texSample = IMG_NORM_PIXEL(texture, texUV);
  col = mix(col, col * texSample.rgb, texSample.a * 0.5);

  gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
