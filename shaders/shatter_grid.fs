/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Shattering grid of 3D tiles that explode outward on bass, revealing texture through each tile",
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
      "NAME": "gridSize",
      "LABEL": "Grid Size",
      "TYPE": "float",
      "DEFAULT": 6.0,
      "MIN": 3.0,
      "MAX": 12.0
    },
    {
      "NAME": "shatterAmount",
      "LABEL": "Shatter",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "tileThickness",
      "LABEL": "Thickness",
      "TYPE": "float",
      "DEFAULT": 0.08,
      "MIN": 0.02,
      "MAX": 0.2
    },
    {
      "NAME": "bevelSize",
      "LABEL": "Bevel",
      "TYPE": "float",
      "DEFAULT": 0.015,
      "MIN": 0.0,
      "MAX": 0.05
    },
    {
      "NAME": "bobHeight",
      "LABEL": "Bob Height",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "tileColor",
      "LABEL": "Tile Color",
      "TYPE": "color",
      "DEFAULT": [0.15, 0.15, 0.15, 1.0]
    }
  ]
}*/

#define MAX_STEPS 32
#define SURF_DIST 0.003
#define MAX_DIST 10.0
#define PI 3.14159265

mat2 rot2(float a) { float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

float hash(vec2 p) {
  return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float sdRoundBox(vec3 p, vec3 b, float r) {
  vec3 q = abs(p) - b;
  return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - r;
}

// Store tile UV for texture lookup
vec2 currentTileUV;

float scene(vec3 p, vec2 shatterCenter) {
  float d = MAX_DIST;

  float tileSize = 1.0 / gridSize;
  float halfTile = tileSize * 0.5;

  // Audio-driven shatter
  float bassShatter = smoothstep(0.0, 1.0, audioBass) * shatterAmount;
  float midRotation = smoothstep(0.0, 1.0, audioMid) * 0.5;

  // We raymarch a flat grid of tiles in front of the camera
  // Project p onto the grid plane, find nearest tiles
  // Tiles are in the XY plane at z=0

  // Check nearby tiles (we can't loop all tiles, so use the grid cell)
  vec2 gridUV = p.xy;
  vec2 cellIdx = floor((gridUV + 0.5) / tileSize);

  float bestD = MAX_DIST;
  vec2 bestTileUV = vec2(0.5);

  for (int ix = -1; ix <= 1; ix++) {
    for (int iy = -1; iy <= 1; iy++) {
      vec2 ci = cellIdx + vec2(float(ix), float(iy));

      // Tile center in world space
      vec2 tileCenter = (ci + 0.5) * tileSize - 0.5;

      // Skip tiles outside visible range (account for aspect ratio)
      float aspect = RENDERSIZE.x / RENDERSIZE.y;
      if (abs(tileCenter.x) > 0.5 + aspect * 0.3 || abs(tileCenter.y) > 0.8) continue;

      float h = hash(ci);

      // Distance from shatter center affects explosion
      float distFromCenter = length(tileCenter - (shatterCenter - 0.5));
      float shatterFactor = bassShatter * exp(-distFromCenter * 2.0);

      // Vertical bob — scaled by bobHeight parameter (bind to audio for reactivity)
      float bob = bobHeight * 0.5;
      float wave = sin(TIME * 1.2 + ci.x * 1.8 + ci.y * 1.3) * bob * 0.3
                 + sin(TIME * 0.7 + ci.y * 2.1) * bob * 0.2;
      // Audio-reactive height: bass pushes tiles, per-tile variation
      float audioHeight = audioBass * (h * 0.6 + 0.4) * bob
                        + audioMid * hash(ci + 50.0) * bob * 0.4;

      // Tile displacement
      vec3 offset = vec3(0.0, 0.0, 0.0);
      offset.z = wave + audioHeight; // Idle bob + audio push toward camera
      offset.z += shatterFactor * (h * 0.8 + 0.2) * -1.5; // Shatter explosion
      offset.xy += (vec2(h, hash(ci + 100.0)) - 0.5) * shatterFactor * 0.3;

      // Tile rotation from audioMid
      vec3 q = p - vec3(tileCenter, 0.0) - offset;

      float rotAngle = midRotation * (h - 0.5) * PI * 0.5;
      q.xz *= rot2(rotAngle * 0.7);
      q.yz *= rot2(rotAngle * 0.5);

      // Box tile with bevel
      vec3 tileBox = vec3(halfTile * 0.9, halfTile * 0.9, tileThickness);
      float td = sdRoundBox(q, tileBox, bevelSize);

      if (td < bestD) {
        bestD = td;
        // UV of this tile in texture space (0-1 range)
        bestTileUV = tileCenter + 0.5;
      }
    }
  }

  currentTileUV = bestTileUV;
  return bestD;
}

vec3 calcNormal(vec3 p, vec2 sc) {
  vec2 e = vec2(0.003, -0.003);
  return normalize(
    e.xyy * scene(p + e.xyy, sc) +
    e.yyx * scene(p + e.yyx, sc) +
    e.yxy * scene(p + e.yxy, sc) +
    e.xxx * scene(p + e.xxx, sc)
  );
}

void main() {
  float aspect = RENDERSIZE.x / RENDERSIZE.y;
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
  vec2 screenUV = gl_FragCoord.xy / RENDERSIZE.xy;

  // Shatter center follows mouse, defaults to screen center
  vec2 shatterCenter = vec2(0.5);
  if (mousePos.x > 0.0 || mousePos.y > 0.0) {
    shatterCenter = mousePos;
  }

  // Camera looks at the tile grid — pull back to cover full viewport
  vec3 ro = vec3(0.0, 0.0, 1.2);
  vec3 rd = normalize(vec3(uv, -0.8));

  // Raymarch
  float totalDist = 0.0;
  bool hit = false;
  vec3 p;
  for (int i = 0; i < MAX_STEPS; i++) {
    p = ro + rd * totalDist;
    float d = scene(p, shatterCenter);
    if (d < SURF_DIST) { hit = true; break; }
    if (totalDist > MAX_DIST) break;
    totalDist += d;
  }

  bool hasTexture = IMG_SIZE_inputTex.x > 0.0;

  vec3 col = vec3(0.0);
  float alpha = 0.0;

  if (hit) {
    vec3 n = calcNormal(p, shatterCenter);
    vec3 v = normalize(ro - p);
    vec3 L = normalize(vec3(1.0, 2.0, 3.0));
    vec3 H = normalize(L + v);

    float diff = max(dot(n, L), 0.0);
    float spec = pow(max(dot(n, H), 0.0), 96.0);
    float fresnel = 0.3 + 0.7 * pow(1.0 - max(dot(n, v), 0.0), 4.0);

    // Edge highlight — detect bevel edges
    float edgeHighlight = pow(1.0 - abs(dot(n, vec3(0.0, 0.0, 1.0))), 2.0) * 0.5;

    if (hasTexture) {
      // Each tile shows the texture piece corresponding to its UV position
      // Use normal refraction for some distortion
      vec2 tileTexUV = screenUV + n.xy * 0.03;
      vec3 texCol = texture2D(inputTex, tileTexUV).rgb;

      col = texCol * (diff * 0.5 + 0.2);
      col += texCol * spec * fresnel * 0.6;
      col += baseColor.rgb * edgeHighlight * 0.4;
    } else {
      // Tile color with per-tile brightness variation
      vec3 albedo = tileColor.rgb * (0.7 + 0.6 * hash(floor((p.xy + 0.5) * gridSize)));
      col = albedo * (diff * 0.5 + 0.2);
      col += vec3(1.0) * spec * fresnel * 1.0;
      col += baseColor.rgb * edgeHighlight * 0.6;
    }

    col += smoothstep(0.0, 1.0, audioLevel) * baseColor.rgb * 0.04;
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
