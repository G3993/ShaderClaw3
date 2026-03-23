/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Exploding particle sphere — raymarched cloud of small spheres that expand on bass",
  "INPUTS": [
    {
      "NAME": "baseColor",
      "LABEL": "Color",
      "TYPE": "color",
      "DEFAULT": [1.0, 1.0, 1.0, 1.0]
    },
    {
      "NAME": "inputTex",
      "LABEL": "Texture",
      "TYPE": "image"
    },
    {
      "NAME": "transparentBg",
      "LABEL": "Transparent",
      "TYPE": "bool",
      "DEFAULT": true
    },
    {
      "NAME": "particleSize",
      "LABEL": "Particle Size",
      "TYPE": "float",
      "DEFAULT": 0.12,
      "MIN": 0.04,
      "MAX": 0.25
    },
    {
      "NAME": "expansion",
      "LABEL": "Expansion",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "glowIntensity",
      "LABEL": "Glow",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    }
  ]
}*/

#define MAX_STEPS 48
#define SURF_DIST 0.003
#define MAX_DIST 20.0
#define PARTICLE_COUNT 20

// Hash for pseudo-random particle placement
vec3 hash3(float n) {
  return fract(sin(vec3(n, n + 1.0, n + 2.0) * vec3(43.5453, 84.1412, 67.3159)) * 5678.5453);
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

float scene(vec3 p, float expand) {
  float d = MAX_DIST;

  for (int i = 0; i < PARTICLE_COUNT; i++) {
    float fi = float(i);
    vec3 h = hash3(fi * 7.31) * 2.0 - 1.0;
    vec3 dir = normalize(h);

    // Base sphere surface position
    float baseR = 1.0;
    vec3 pos = dir * baseR;

    // Orbiting animation
    float phase = fi * 0.314;
    pos.xz += vec2(sin(TIME * 0.3 + phase), cos(TIME * 0.3 + phase)) * 0.1;
    pos.y += sin(TIME * 0.4 + fi * 0.5) * 0.08;

    // Expansion: particles move outward from sphere surface
    float audioExpand = smoothstep(0.0, 1.0, audioBass) * expansion;
    float levelExpand = smoothstep(0.0, 1.0, audioLevel) * expansion * 0.5;
    pos += dir * (audioExpand * 0.8 + levelExpand * 0.4);

    // Particle size with gentle pulse
    float sz = particleSize + sin(TIME * 1.5 + fi * 2.0) * 0.015;
    sz += smoothstep(0.2, 0.8, audioBass) * 0.03;

    d = min(d, sdSphere(p - pos, sz));
  }
  return d;
}

vec3 calcNormal(vec3 p, float expand) {
  vec2 e = vec2(0.003, -0.003);
  return normalize(
    e.xyy * scene(p + e.xyy, expand) +
    e.yyx * scene(p + e.yyx, expand) +
    e.yxy * scene(p + e.yxy, expand) +
    e.xxx * scene(p + e.xxx, expand)
  );
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float expand = smoothstep(0.0, 1.0, audioBass) * expansion;

  // Camera with mouse orbit
  vec3 ro = vec3(0.0, 0.3, 4.0);
  vec3 target = vec3(0.0);
  if (mousePos.x > 0.0 || mousePos.y > 0.0) {
    float mx = (mousePos.x - 0.5) * 3.14159;
    float my = (mousePos.y - 0.5) * 1.0;
    ro = vec3(sin(mx) * 4.0, my * 2.0 + 0.3, cos(mx) * 4.0);
  }
  vec3 fwd = normalize(target - ro);
  vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
  vec3 up = cross(right, fwd);
  vec3 rd = normalize(fwd * 1.6 + right * uv.x + up * uv.y);

  // Raymarch
  float totalDist = 0.0;
  bool hit = false;
  vec3 p;
  for (int i = 0; i < MAX_STEPS; i++) {
    p = ro + rd * totalDist;
    float d = scene(p, expand);
    if (d < SURF_DIST) { hit = true; break; }
    if (totalDist > MAX_DIST) break;
    totalDist += d;
  }

  // Glow accumulation (soft halo around particles)
  float glow = 0.0;
  {
    float td = 0.0;
    for (int i = 0; i < 32; i++) {
      vec3 gp = ro + rd * td;
      float d = scene(gp, expand);
      glow += glowIntensity * 0.015 / (0.05 + d * d);
      td += max(d, 0.08);
      if (td > 8.0) break;
    }
  }

  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTex, texUV);
  bool hasTexture = texSample.a > 0.01;

  vec3 col = vec3(0.0);
  float alpha = 0.0;

  if (hit) {
    vec3 n = calcNormal(p, expand);
    vec3 v = normalize(ro - p);
    vec3 L = normalize(vec3(1.5, 2.5, 2.0));
    vec3 H = normalize(L + v);

    float diff = max(dot(n, L), 0.0);
    float spec = pow(max(dot(n, H), 0.0), 64.0);
    float fresnel = 0.5 + 0.5 * pow(1.0 - max(dot(n, v), 0.0), 3.0);

    if (hasTexture) {
      vec2 refractUV = texUV + n.xy * 0.06;
      vec3 texCol = texture2D(inputTex, refractUV).rgb;
      col = texCol * diff * 0.6;
      col += texCol * spec * fresnel * 0.8;
      col += texCol * pow(1.0 - max(dot(n, v), 0.0), 3.0) * 0.3;
    } else {
      vec3 albedo = vec3(0.9, 0.85, 0.95);
      col = albedo * diff * 0.5;
      col += vec3(1.0) * spec * fresnel * 1.2;
      col += vec3(0.7, 0.5, 0.9) * pow(1.0 - max(dot(n, v), 0.0), 4.0) * 0.5;
    }
    alpha = 1.0;
  }

  // Add glow
  vec3 glowCol = baseColor.rgb * glow * 0.3;
  col += glowCol;
  if (!hit && glow > 0.05) {
    alpha = clamp(glow * 0.4, 0.0, 1.0);
  }

  col *= baseColor.rgb;

  // Tone mapping
  col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);

  if (!hit && transparentBg && glow < 0.05) {
    alpha = 0.0;
  }

  gl_FragColor = vec4(col, alpha);
}
