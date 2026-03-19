/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Kintsugi-inspired golden cracks on dark ceramic with multi-scale Voronoi",
  "INPUTS": [
    {
      "NAME": "crackSpeed",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Crack Speed"
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

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
vec2 hash2(vec2 p) { return vec2(fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453), fract(sin(dot(p, vec2(269.5, 183.3))) * 43758.5453)); }

float noise(vec2 p) {
  vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);
  return mix(mix(hash(i), hash(i+vec2(1,0)), f.x), mix(hash(i+vec2(0,1)), hash(i+vec2(1,1)), f.x), f.y);
}

float fbm(vec2 p) {
  float v = 0.0; float a = 0.5;
  for (int i = 0; i < 5; i++) { v += a * noise(p); p = p * 2.03 + vec2(1.7, 9.2); a *= 0.49; }
  return v;
}

vec3 kintsugiVoronoi(vec2 p, float t, float warpAmt) {
  vec2 warp = vec2(fbm(p * 0.8 + t * 0.05), fbm(p * 0.8 + vec2(5.2, 1.3) + t * 0.04));
  p += warp * warpAmt;
  vec2 i = floor(p); vec2 f = fract(p);
  float minDist = 10.0; float secondDist = 10.0; vec2 nearestCell = vec2(0.0);
  for (float y = -2.0; y <= 2.0; y += 1.0) {
    for (float x = -2.0; x <= 2.0; x += 1.0) {
      vec2 nb = vec2(x, y); vec2 cid = i + nb;
      vec2 h = hash2(cid); vec2 pt = nb + h * 0.85 + 0.075;
      pt.x += sin(t * 0.07 + h.x * 20.0) * 0.06;
      pt.y += cos(t * 0.06 + h.y * 20.0) * 0.06;
      float d = dot(pt - f, pt - f);
      if (d < minDist) { secondDist = minDist; minDist = d; nearestCell = cid; }
      else if (d < secondDist) { secondDist = d; }
    }
  }
  return vec3(sqrt(minDist), sqrt(secondDist) - sqrt(minDist), hash(nearestCell));
}

void main() {
  vec2 p = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME * crackSpeed;
  float gi = glowIntensity + audioLevel * 0.5;

  vec3 coarse = kintsugiVoronoi(p * 2.5, t, 0.6);
  vec3 medium = kintsugiVoronoi(p * 5.5, t, 0.4);

  float coarseReveal = smoothstep(0.15, 0.65, sin(t * 0.12 + coarse.z * 6.28) * 0.5 + 0.5);
  float mediumReveal = smoothstep(0.15, 0.65, sin(t * 0.156 + medium.z * 6.28 + 1.0) * 0.5 + 0.5);

  float coarseCrack = smoothstep(0.08, 0.0, coarse.y) * coarseReveal;
  float coarseGlow = smoothstep(0.22, 0.0, coarse.y) * coarseReveal;
  float medCrack = smoothstep(0.06, 0.0, medium.y) * mediumReveal * 0.7;

  float crackCore = max(coarseCrack, medCrack);

  float grain = fbm(p * 8.0 + coarse.z * 40.0);
  vec3 surface = mix(vec3(0.03, 0.025, 0.02), vec3(0.06, 0.045, 0.03), 0.3 + grain * 0.4);

  vec3 goldCore = vec3(1.0, 0.82, 0.55);
  vec3 goldEdge = vec3(0.78, 0.58, 0.24);
  vec3 goldHot = vec3(1.0, 0.92, 0.72);

  float flowTurb = fbm(p * 6.0 + t * vec2(0.08, -0.06));
  float goldTemp = crackCore * (0.6 + flowTurb * 0.4);
  vec3 goldColor = mix(goldEdge, goldCore, smoothstep(0.2, 0.6, goldTemp));
  goldColor = mix(goldColor, goldHot, smoothstep(0.6, 1.0, goldTemp));

  surface += goldEdge * coarseGlow * 0.4 * gi;
  vec3 col = mix(surface, goldColor, smoothstep(0.0, 0.12, crackCore));
  col += goldEdge * coarseGlow * 0.3 * gi;
  col += vec3(0.1, 0.06, 0.02) * audioBass * coarseGlow;

  col *= sin(TIME * 0.15) * 0.08 + 0.92;
  float vig = 1.0 - smoothstep(0.4, 1.3, length(p * vec2(1.0, 1.1)));
  col *= 0.5 + vig * 0.5;
  col = col / (1.0 + col * 0.15);
  col = pow(col, vec3(0.95, 1.0, 1.1));

  col *= baseColor.rgb;
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
  vec4 texSample = IMG_NORM_PIXEL(texture, texUV);
  col = mix(col, col * texSample.rgb, texSample.a * 0.5);

  gl_FragColor = vec4(col, 1.0);
}
