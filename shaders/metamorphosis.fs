/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Morphing metallic blobs via ray-marched metaballs with studio lighting",
  "INPUTS": [
    {
      "NAME": "morphSpeed",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Morph Speed"
    },
    {
      "NAME": "blobCount",
      "TYPE": "float",
      "DEFAULT": 4,
      "MIN": 2,
      "MAX": 6,
      "LABEL": "Blob Count"
    },
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
    }
  ]
}*/

precision highp float;
#define TAU 6.28318530718
#define MAX_STEPS 48
#define SURF_DIST 0.002
#define BLOB_MAX 6

float smin(float a, float b, float k) { float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0); return mix(b, a, h) - k * h * (1.0 - h); }
float sdEllipsoid(vec3 p, vec3 r) { float k0 = length(p / r); return k0 * (k0 - 1.0) / length(p / (r * r)); }

float scene(vec3 p, float t, int cnt) {
  float d = 20.0;
  float speed = t * morphSpeed;
  for (int i = 0; i < BLOB_MAX; i++) {
    if (i >= cnt) break;
    float fi = float(i), phase = fi * TAU / 6.0;
    vec3 pos = vec3(sin(speed * 0.7 + phase) * 0.8, cos(speed * 0.5 + phase * 1.4) * 0.6, sin(speed * 0.6 + phase * 1.8) * 0.5);
    float r = 0.45 + fi * 0.03 + sin(speed * 1.2 + fi * 1.7) * 0.08;
    d = smin(d, sdEllipsoid(p - pos, vec3(r) * vec3(1.0 + sin(speed * 0.9 + fi * 2.3) * 0.25, 1.0 + cos(speed * 0.7 + fi * 1.9) * 0.2, 1.0 + sin(speed * 1.1 + fi * 2.7) * 0.2)), 0.6);
  }
  return d;
}

vec3 calcNormal(vec3 p, float t, int cnt) {
  vec2 e = vec2(0.002, -0.002);
  return normalize(e.xyy * scene(p + e.xyy, t, cnt) + e.yyx * scene(p + e.yyx, t, cnt) + e.yxy * scene(p + e.yxy, t, cnt) + e.xxx * scene(p + e.xxx, t, cnt));
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
  float t = TIME;
  int cnt = int(blobCount + audioBass * 2.0);

  vec3 ro = vec3(0.0, 0.3, 3.8);
  vec3 target = vec3(0.0);
  if (mousePos.x > 0.0 || mousePos.y > 0.0) { target = vec3((mousePos - 0.5) * 1.5, 0.0) * 0.3; }
  vec3 fwd = normalize(target - ro), right = normalize(cross(fwd, vec3(0,1,0))), up = cross(right, fwd);
  vec3 rd = normalize(fwd * 1.5 + right * uv.x + up * uv.y);

  float totalDist = 0.0; bool hit = false; vec3 p;
  for (int i = 0; i < MAX_STEPS; i++) {
    p = ro + rd * totalDist;
    float d = scene(p, t, cnt);
    if (d < SURF_DIST) { hit = true; break; }
    if (totalDist > 20.0) break;
    totalDist += d;
  }

  vec3 col = vec3(0.012, 0.01, 0.008);
  if (hit) {
    vec3 n = calcNormal(p, t, cnt), v = normalize(ro - p);
    vec3 albedo = mix(vec3(0.85, 0.65, 0.3), vec3(0.75, 0.45, 0.25), sin(p.x * 3.0 + p.z * 2.0 + t * morphSpeed * 0.4) * 0.5 + 0.5);
    vec3 L = normalize(vec3(2, 3, 1.5)), H = normalize(L + v);
    float diff = max(dot(n, L), 0.0);
    float spec = pow(max(dot(n, H), 0.0), 256.0);
    float fres = 0.8 + 0.2 * pow(1.0 - max(dot(n, v), 0.0), 5.0);
    col = albedo * diff * vec3(1.0, 0.9, 0.75) * 0.4 + vec3(1.0, 0.9, 0.75) * spec * fres * 2.0;
    col += vec3(0.9, 0.6, 0.35) * pow(1.0 - max(dot(n, v), 0.0), 4.0) * 0.8;
    col += audioLevel * vec3(0.1, 0.06, 0.03);
  }

  col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);
  col = pow(col, vec3(0.95, 0.98, 1.04));
  col *= 1.0 - dot(uv, uv) * 0.25;

  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, texSample.rgb, texSample.a * 0.3);
  col = mix(col, col * baseColor.rgb, 0.5);

  gl_FragColor = vec4(col, 1.0);
}