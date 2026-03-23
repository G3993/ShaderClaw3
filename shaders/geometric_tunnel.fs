/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Geometric tunnel flythrough with neon edges and texture-mapped walls",
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
      "NAME": "tunnelSides",
      "LABEL": "Sides",
      "TYPE": "float",
      "DEFAULT": 6.0,
      "MIN": 4.0,
      "MAX": 10.0
    },
    {
      "NAME": "tunnelRadius",
      "LABEL": "Radius",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.8,
      "MAX": 3.0
    },
    {
      "NAME": "neonGlow",
      "LABEL": "Neon Glow",
      "TYPE": "float",
      "DEFAULT": 0.6,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "flightSpeed",
      "LABEL": "Speed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    }
  ]
}*/

#define MAX_STEPS 48
#define SURF_DIST 0.003
#define MAX_DIST 30.0
#define PI 3.14159265

// Polygon distance in 2D (hexagonal/octagonal cross-section)
float sdPolygon(vec2 p, float r, float sides) {
  float an = PI / sides;
  float bn = atan(p.x, p.y);
  float sector = floor(bn / (2.0 * an) + 0.5);
  bn = bn - sector * 2.0 * an;
  vec2 cs = vec2(cos(an), sin(an));
  p = length(p) * vec2(cos(bn), abs(sin(bn)));
  p -= r * cs;
  p.x = max(p.x, 0.0);
  return length(p) * sign(p.y);
}

float scene(vec3 p, float z_offset) {
  // Tunnel along Z axis
  vec3 q = p;
  q.z += z_offset;

  // Bass makes walls pulse
  float bassPulse = smoothstep(0.0, 1.0, audioBass) * 0.25;
  float radius = tunnelRadius + bassPulse + sin(q.z * 0.3 + TIME * 0.5) * 0.15;

  // Negative polygon — inside is the empty space
  float sides = floor(tunnelSides);
  float d = -sdPolygon(q.xy, radius, sides);

  // Add ring segments for edge detail
  float ringSpacing = 2.0;
  float ringZ = mod(q.z + ringSpacing * 0.5, ringSpacing) - ringSpacing * 0.5;
  float ring = max(abs(ringZ) - 0.06, -sdPolygon(q.xy, radius - 0.08, sides));
  d = min(d, ring);

  return d;
}

vec3 calcNormal(vec3 p, float z_offset) {
  vec2 e = vec2(0.003, -0.003);
  return normalize(
    e.xyy * scene(p + e.xyy, z_offset) +
    e.yyx * scene(p + e.yyx, z_offset) +
    e.yxy * scene(p + e.yxy, z_offset) +
    e.xxx * scene(p + e.xxx, z_offset)
  );
}

float edgeGlow(vec3 p, float z_offset) {
  // Detect polygon edges by how close we are to them
  float sides = floor(tunnelSides);
  float an = PI / sides;
  vec2 q = p.xy;
  float bn = atan(q.x, q.y);
  float sector = floor(bn / (2.0 * an) + 0.5);
  float edgeAngle = abs(bn - sector * 2.0 * an);
  // Closer to edge = stronger glow
  float edgeDist = edgeAngle * length(q);
  float glow = exp(-edgeDist * 12.0);

  // Ring glow
  float ringSpacing = 2.0;
  float ringZ = mod(p.z + z_offset + ringSpacing * 0.5, ringSpacing) - ringSpacing * 0.5;
  float ringGlow = exp(-abs(ringZ) * 8.0);

  return (glow + ringGlow * 0.5) * neonGlow;
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

  // Camera flies through tunnel
  float audioSpeed = smoothstep(0.0, 1.0, audioLevel) * flightSpeed;
  float z_offset = TIME * (flightSpeed * 2.0 + audioSpeed * 3.0);

  vec3 ro = vec3(0.0, 0.0, 0.0);

  // Mouse controls camera tilt
  vec2 tilt = vec2(0.0);
  if (mousePos.x > 0.0 || mousePos.y > 0.0) {
    tilt = (mousePos - 0.5) * 0.8;
  }
  vec3 rd = normalize(vec3(uv.x + tilt.x, uv.y + tilt.y, 1.5));

  // Raymarch
  float totalDist = 0.0;
  bool hit = false;
  vec3 p;
  for (int i = 0; i < MAX_STEPS; i++) {
    p = ro + rd * totalDist;
    float d = scene(p, z_offset);
    if (d < SURF_DIST) { hit = true; break; }
    if (totalDist > MAX_DIST) break;
    totalDist += d * 0.8; // Slow down near walls for accuracy
  }

  vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
  vec4 texSample = texture2D(inputTex, texUV);
  bool hasTexture = texSample.a > 0.01;

  vec3 col = vec3(0.0);
  float alpha = 0.0;

  if (hit) {
    vec3 n = calcNormal(p, z_offset);
    vec3 v = normalize(-rd);
    vec3 L = normalize(vec3(0.5, 1.0, -0.5));
    vec3 H = normalize(L + v);

    float diff = max(dot(n, L), 0.0);
    float spec = pow(max(dot(n, H), 0.0), 48.0);
    float fresnel = 0.3 + 0.7 * pow(1.0 - max(dot(n, v), 0.0), 3.0);

    // Neon edge glow
    float glow = edgeGlow(p, z_offset);
    vec3 neonCol = baseColor.rgb * 2.0;

    // Depth fade
    float depthFade = 1.0 - smoothstep(5.0, MAX_DIST, totalDist);

    if (hasTexture) {
      // Texture on tunnel walls
      vec2 refractUV = texUV + n.xy * 0.06;
      // Scroll texture with tunnel flight
      refractUV.y += fract(z_offset * 0.05);
      vec3 texCol = texture2D(inputTex, fract(refractUV)).rgb;

      col = texCol * (diff * 0.5 + 0.15);
      col += texCol * spec * fresnel * 0.6;
      col += neonCol * glow * 0.8;
    } else {
      // Procedural wall color
      vec3 wallCol = vec3(0.08, 0.06, 0.12);
      col = wallCol * (diff * 0.5 + 0.1);
      col += vec3(0.9) * spec * fresnel * 0.5;
      col += neonCol * glow * 1.2;
    }

    col *= depthFade;
    col += smoothstep(0.0, 1.0, audioLevel) * neonCol * 0.05;
    alpha = depthFade;
  }

  // Apply base color tinting
  col *= mix(vec3(1.0), baseColor.rgb, 0.5);

  // Tone mapping
  col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);

  if (!hit && transparentBg) {
    alpha = 0.0;
  }

  gl_FragColor = vec4(col, alpha);
}
