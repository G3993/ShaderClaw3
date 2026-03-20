/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Liquid morphing orb with noise displacement and iridescent rim lighting",
  "INPUTS": [
    {
      "NAME": "baseColor",
      "LABEL": "Color",
      "TYPE": "color",
      "DEFAULT": [0.91, 0.25, 0.34, 1.0]
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
      "NAME": "displacementAmt",
      "LABEL": "Displacement",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "orbSize",
      "LABEL": "Size",
      "TYPE": "float",
      "DEFAULT": 1.2,
      "MIN": 0.5,
      "MAX": 2.0
    },
    {
      "NAME": "iridescence",
      "LABEL": "Iridescence",
      "TYPE": "float",
      "DEFAULT": 0.6,
      "MIN": 0.0,
      "MAX": 1.0
    }
  ]
}*/

#define MAX_STEPS 48
#define SURF_DIST 0.002
#define MAX_DIST 15.0
#define PI 3.14159265

// Simplex-like noise
vec3 mod289(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec4 mod289(vec4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec4 permute(vec4 x) { return mod289(((x * 34.0) + 1.0) * x); }
vec4 taylorInvSqrt(vec4 r) { return 1.79284291400159 - 0.85373472095314 * r; }

float snoise(vec3 v) {
  const vec2 C = vec2(1.0 / 6.0, 1.0 / 3.0);
  vec3 i = floor(v + dot(v, C.yyy));
  vec3 x0 = v - i + dot(i, C.xxx);
  vec3 g = step(x0.yzx, x0.xyz);
  vec3 l = 1.0 - g;
  vec3 i1 = min(g.xyz, l.zxy);
  vec3 i2 = max(g.xyz, l.zxy);
  vec3 x1 = x0 - i1 + C.xxx;
  vec3 x2 = x0 - i2 + C.yyy;
  vec3 x3 = x0 - 0.5;
  i = mod289(i);
  vec4 p = permute(permute(permute(
    i.z + vec4(0.0, i1.z, i2.z, 1.0))
    + i.y + vec4(0.0, i1.y, i2.y, 1.0))
    + i.x + vec4(0.0, i1.x, i2.x, 1.0));
  vec4 j = p - 49.0 * floor(p * (1.0 / 49.0));
  vec4 x_ = floor(j * (1.0 / 7.0));
  vec4 y_ = floor(j - 7.0 * x_);
  vec4 x2_ = (x_ * 2.0 + 0.5) / 7.0 - 1.0;
  vec4 y2_ = (y_ * 2.0 + 0.5) / 7.0 - 1.0;
  vec4 h = 1.0 - abs(x2_) - abs(y2_);
  vec4 b0 = vec4(x2_.xy, y2_.xy);
  vec4 b1 = vec4(x2_.zw, y2_.zw);
  vec4 s0 = floor(b0) * 2.0 + 1.0;
  vec4 s1 = floor(b1) * 2.0 + 1.0;
  vec4 sh = -step(h, vec4(0.0));
  vec4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
  vec4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
  vec3 p0 = vec3(a0.xy, h.x);
  vec3 p1 = vec3(a0.zw, h.y);
  vec3 p2 = vec3(a1.xy, h.z);
  vec3 p3 = vec3(a1.zw, h.w);
  vec4 norm = taylorInvSqrt(vec4(dot(p0,p0), dot(p1,p1), dot(p2,p2), dot(p3,p3)));
  p0 *= norm.x; p1 *= norm.y; p2 *= norm.z; p3 *= norm.w;
  vec4 m = max(0.6 - vec4(dot(x0,x0), dot(x1,x1), dot(x2,x2), dot(x3,x3)), 0.0);
  m = m * m;
  return 42.0 * dot(m * m, vec4(dot(p0,x0), dot(p1,x1), dot(p2,x2), dot(p3,x3)));
}

float displacement(vec3 p) {
  float bassD = smoothstep(0.0, 1.0, audioBass) * displacementAmt;
  float highD = smoothstep(0.0, 1.0, audioHigh) * displacementAmt * 0.3;

  float n = snoise(p * 2.0 + TIME * 0.3) * bassD * 0.4;
  n += snoise(p * 5.0 + TIME * 0.7) * highD * 0.2;
  n += snoise(p * 1.5 - TIME * 0.2) * 0.08 * displacementAmt;
  return n;
}

float scene(vec3 p) {
  float r = orbSize;
  float d = length(p) - r;
  d += displacement(normalize(p) * r);
  return d;
}

vec3 calcNormal(vec3 p) {
  vec2 e = vec2(0.003, -0.003);
  return normalize(
    e.xyy * scene(p + e.xyy) +
    e.yyx * scene(p + e.yyx) +
    e.yxy * scene(p + e.yxy) +
    e.xxx * scene(p + e.xxx)
  );
}

vec3 iridescentColor(float angle) {
  // Rainbow shift based on view angle
  return vec3(
    0.5 + 0.5 * sin(angle * 6.0 + 0.0),
    0.5 + 0.5 * sin(angle * 6.0 + 2.1),
    0.5 + 0.5 * sin(angle * 6.0 + 4.2)
  );
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

  vec3 ro = vec3(0.0, 0.0, 3.5);
  vec3 target = vec3(0.0);

  // Light position from mouse
  vec3 lightPos = vec3(2.0, 3.0, 2.0);
  if (mousePos.x > 0.0 || mousePos.y > 0.0) {
    lightPos = vec3((mousePos.x - 0.5) * 6.0, (mousePos.y - 0.5) * 6.0, 2.5);
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
    float d = scene(p);
    if (d < SURF_DIST) { hit = true; break; }
    if (totalDist > MAX_DIST) break;
    totalDist += d;
  }

  // Texture: sample at screen UV
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTex, texUV);
  bool hasTexture = texSample.a > 0.01;

  vec3 col = vec3(0.0);
  float alpha = 0.0;

  if (hit) {
    vec3 n = calcNormal(p);
    vec3 v = normalize(ro - p);
    vec3 L = normalize(lightPos - p);
    vec3 H = normalize(L + v);

    float diff = max(dot(n, L), 0.0);
    float spec = pow(max(dot(n, H), 0.0), 96.0);
    float NdotV = max(dot(n, v), 0.0);
    float fresnel = 0.4 + 0.6 * pow(1.0 - NdotV, 4.0);

    // Iridescent rim
    vec3 iriCol = iridescentColor(NdotV + TIME * 0.1) * iridescence;
    float rim = pow(1.0 - NdotV, 3.0);

    if (hasTexture) {
      // Spherical UV mapping: wrap texture around the orb
      vec3 pn = normalize(p);
      vec2 sphereUV = vec2(
        0.5 + atan(pn.z, pn.x) / (2.0 * PI),
        0.5 - asin(clamp(pn.y, -1.0, 1.0)) / PI
      );
      // Add normal-based distortion for liquid feel
      sphereUV += n.xy * 0.04;
      vec3 texCol = texture2D(inputTex, sphereUV).rgb;

      col = texCol * diff * 0.6;
      col += texCol * spec * fresnel * 0.7;
      col += iriCol * rim * 0.5 * texCol;
      col += texCol * 0.1; // ambient
    } else {
      // Procedural liquid look
      vec3 albedo = mix(vec3(0.2, 0.3, 0.6), vec3(0.5, 0.1, 0.4),
        snoise(p * 3.0 + TIME * 0.2) * 0.5 + 0.5);
      col = albedo * diff * 0.5;
      col += vec3(1.0) * spec * fresnel * 1.5;
      col += iriCol * rim * 0.7;
      col += albedo * 0.08;
    }

    // Audio glow
    col += smoothstep(0.0, 1.0, audioLevel) * vec3(0.04, 0.02, 0.03);
    alpha = 1.0;
  }

  col *= baseColor.rgb;

  // Tone mapping
  col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);

  if (!hit && transparentBg) {
    alpha = 0.0;
  }

  gl_FragColor = vec4(col, alpha);
}
