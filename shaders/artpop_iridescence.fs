/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Thin-film iridescent bubble surfaces with flowing organic interference patterns",
  "INPUTS": [
    {
      "NAME": "filmThickness",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Film Thickness"
    },
    {
      "NAME": "flowSpeed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Flow Speed"
    },
    {
      "NAME": "baseColor",
      "LABEL": "Color",
      "TYPE": "color",
      "DEFAULT": [0.91, 0.25, 0.34, 1.0]
    },
    {
      "NAME": "inputTexture",
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

float fbm(vec2 p) { float f = 0.0; f += 0.5 * snoise(p); p *= 2.02; f += 0.25 * snoise(p); p *= 2.03; f += 0.125 * snoise(p); p *= 2.01; f += 0.0625 * snoise(p); return f; }
float warpedNoise(vec2 p, float t) {
  vec2 q = vec2(fbm(p + t * 0.12), fbm(p + vec2(5.2, 1.3) + t * 0.09));
  return fbm(p + 4.0 * q + vec2(1.7, 9.2) + t * 0.07);
}

vec3 thinFilm(float thickness, float cosTheta) {
  float phase = thickness * cosTheta;
  vec3 film = 0.5 + 0.5 * cos(phase + vec3(0.0, 2.094, 4.189));
  return film * film;
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  float aspect = RENDERSIZE.x / RENDERSIZE.y;
  vec2 p = (uv - 0.5) * vec2(aspect, 1.0);
  float t = TIME * flowSpeed;
  float ft = filmThickness + audioBass * 0.5;

  float dist1 = length(p * vec2(1.0, 1.1));
  float mask1 = smoothstep(0.82, 0.45, dist1);

  float surface = warpedNoise(p * 2.5, t);
  float eps = 0.004;
  vec3 norm = normalize(vec3(surface - warpedNoise(p * 2.5 + vec2(eps, 0.0), t), surface - warpedNoise(p * 2.5 + vec2(0.0, eps), t), eps));
  float cosTheta = max(abs(dot(norm, vec3(0, 0, 1))), 0.15);
  float thick = 8.0 + surface * 12.0 * ft + p.y * 4.0 + sin(t * 0.25) * 2.0;
  vec3 film = thinFilm(thick, cosTheta);
  float fresnel = pow(1.0 - cosTheta, 4.0);
  film = mix(film, film * 2.0 + vec3(0.15, 0.1, 0.2), fresnel * 0.5);
  film += vec3(1.0, 0.97, 0.92) * pow(max(dot(norm, normalize(vec3(0.4, 0.6, 1.0) + vec3(0, 0, 1.0))), 0.0), 80.0) * 0.5;
  film += vec3(0.1, 0.05, 0.02) * audioLevel;

  vec3 bg = vec3(0.015, 0.015, 0.03);
  vec3 col = mix(bg, film, mask1);

  float vig = 1.0 - smoothstep(0.35, 1.3, length(p * vec2(0.85, 1.0)));
  col *= 0.6 + 0.4 * vig;
  col = clamp(col, 0.0, 1.0);
  col = pow(col, vec3(0.95));
  col = col * col * (3.0 - 2.0 * col);

  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTexture, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}