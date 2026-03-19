/*{
  "CATEGORIES": [
    "Radiant"
  ],
  "DESCRIPTION": "Neon-glowing metaball drips rising through darkness with trailing tendrils",
  "INPUTS": [
    {
      "NAME": "dripSpeed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2,
      "LABEL": "Drip Speed"
    },
    {
      "NAME": "blobCount",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 1,
      "LABEL": "Blob Count"
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
#define MAX_BLOBS 12

float hash(vec2 p) { vec3 p3 = fract(vec3(p.xyx) * 0.1031); p3 += dot(p3, p3.yzx + 33.33); return fract((p3.x + p3.y) * p3.z); }
float vnoise(vec2 p) {
  vec2 i = floor(p), f = fract(p); f = f * f * (3.0 - 2.0 * f);
  return mix(mix(hash(i), hash(i+vec2(1,0)), f.x), mix(hash(i+vec2(0,1)), hash(i+vec2(1,1)), f.x), f.y);
}

void main() {
  vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy * 0.5) / RENDERSIZE.y;
  float t = TIME, speed = dripSpeed + audioBass * 0.5;
  vec3 col = vec3(0.025, 0.018, 0.015);

  float energy = 0.0;
  float numBlobs = 4.0 + blobCount * 8.0;
  for (int i = 0; i < MAX_BLOBS; i++) {
    if (float(i) >= numBlobs) break;
    float fi = float(i), phase = fi * 1.618 + fi * fi * 0.13;
    float riseCycle = mod(t * (0.3 + 0.4 * fract(fi * 0.618)) * speed + phase * 0.7, 3.5) - 1.0;
    vec2 bpos = vec2(sin(phase * 2.4) * 0.45 + sin(t * speed * 0.8 + phase * 3.1) * 0.12, -0.7 + riseCycle * 0.9);
    float radius = (0.04 + 0.03 * fract(phase * 0.317)) * (1.0 + 0.15 * sin(t * speed * 1.5 + phase * 4.7));
    float d = length(uv - bpos);
    energy += (radius * radius) / (d * d + 0.0001);
  }

  if (mousePos.x > 0.0 || mousePos.y > 0.0) {
    vec2 mUV = (mousePos - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    energy += 0.0036 / (dot(uv - mUV, uv - mUV) + 0.0001);
  }

  float tendrils = vnoise(vec2(uv.x * 6.0, uv.y * 2.0 - t * speed * 0.6) + 10.0) * 0.5 +
                   vnoise(vec2(uv.x * 12.0, uv.y * 4.0 - t * speed * 0.8) + 20.0) * 0.3;
  tendrils = smoothstep(0.35, 0.65, tendrils) * smoothstep(0.6, -0.3, uv.y);
  float field = energy + tendrils * 0.6;

  float surface = smoothstep(0.4, 0.7, field);
  float inner = smoothstep(0.8, 1.8, field);
  float core = smoothstep(2.0, 4.0, field);

  col += vec3(1.2, 0.55, 0.10) * smoothstep(0.15, 0.5, field) + vec3(0.1, 0.05, 0.02) * audioLevel;
  col = mix(col, vec3(2.5, 1.3, 0.40), surface * 0.95);
  col = mix(col, vec3(3.5, 2.0, 0.70), inner * 0.95);
  col = mix(col, vec3(5.0, 4.0, 2.5), core);
  col += vec3(1.8, 1.0, 0.3) * surface * (1.0 - inner) * 0.8;

  col += (hash(gl_FragCoord.xy + fract(t * 43.758) * 1000.0) - 0.5) * 0.025;
  col *= smoothstep(1.3, 0.4, length(uv * vec2(0.9, 1.0)));
  col = max(col, vec3(0.0));
  col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);
  col = pow(col, vec3(0.90));

  col *= baseColor.rgb;
  vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
  vec4 texSample = texture2D(inputTex, texUV);
  col = mix(col, col * texSample.rgb, texSample.a * 0.5);

  gl_FragColor = vec4(col, 1.0);
}
