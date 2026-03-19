/*{
  "CATEGORIES": [
    "Radiant",
    "Organic",
    "Fabric"
  ],
  "DESCRIPTION": "Multi-layered silk fabric with domain-warped folds, Kajiya-Kay anisotropic specular, translucent layers, and sparkle. From Radiant by Paul Bakaus (MIT).",
  "INPUTS": [
    {
      "NAME": "flowSpeed",
      "LABEL": "Flow Speed",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 1.5,
      "DEFAULT": 0.4
    },
    {
      "NAME": "sheenIntensity",
      "LABEL": "Sheen Intensity",
      "TYPE": "float",
      "MIN": 0.3,
      "MAX": 2,
      "DEFAULT": 1
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

// Silk Cascade - Radiant Shaders Gallery (MIT License)

#define PI 3.14159265359

float hash12(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash12(i);
    float b = hash12(i + vec2(1.0, 0.0));
    float c = hash12(i + vec2(0.0, 1.0));
    float d = hash12(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm3(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 rot = mat2(0.8, -0.6, 0.6, 0.8);
    for (int i = 0; i < 3; i++) {
        v += a * vnoise(p);
        p = rot * p * 2.0;
        a *= 0.5;
    }
    return v;
}

vec2 domainWarp(vec2 p, float t, float scale, float seed) {
    return vec2(
        fbm3(p * scale + vec2(1.7 + seed, 9.2) + t * 0.15),
        fbm3(p * scale + vec2(8.3, 2.8 + seed) - t * 0.12)
    );
}

vec3 fabricFold(vec2 p, float t, float seed, float freq, float flow) {
    float ts = t * flow;
    vec2 warp = domainWarp(p + seed * 3.7, ts, 1.2, seed);
    vec2 wp = p + warp * 0.55;
    float h = 0.0;
    vec2 g = vec2(0.0);

    float f1x = freq * 0.7, f1y = freq * 0.4;
    float ph1 = wp.x * f1x + wp.y * f1y + ts * 0.3 + seed * 2.1;
    h += sin(ph1) * 0.35; g += cos(ph1) * 0.35 * vec2(f1x, f1y);

    float f2x = -freq * 0.3, f2y = freq * 0.9;
    float ph2 = wp.x * f2x + wp.y * f2y + ts * 0.25 + seed * 1.3;
    h += sin(ph2) * 0.25; g += cos(ph2) * 0.25 * vec2(f2x, f2y);

    float f3 = freq * 0.6;
    float ph3 = (wp.x + wp.y) * f3 + ts * 0.2 + seed * 4.5;
    h += sin(ph3) * 0.18; g += cos(ph3) * 0.18 * vec2(f3, f3);

    float f4x = freq * 1.8, f4y = freq * 1.2;
    float ph4 = wp.x * f4x + wp.y * f4y - ts * 0.35 + seed * 0.7;
    h += sin(ph4) * 0.08; g += cos(ph4) * 0.08 * vec2(f4x, f4y);

    float f5x = -freq * 0.8, f5y = freq * 2.0;
    float ph5 = wp.x * f5x + wp.y * f5y + ts * 0.28 + seed * 5.2;
    h += sin(ph5) * 0.06; g += cos(ph5) * 0.06 * vec2(f5x, f5y);

    h += vnoise(wp * freq * 0.9 + seed * 10.0 + ts * 0.04) * 0.12 - 0.06;
    return vec3(h, g);
}

float kajiyaSpec(vec2 grad, vec3 L, vec3 V, float shine) {
    float gl2 = dot(grad, grad);
    if (gl2 < 0.0001) return 0.0;
    vec2 tg = vec2(-grad.y, grad.x) / sqrt(gl2);
    vec3 T = normalize(vec3(tg, 0.0));
    vec3 H = normalize(L + V);
    float TdH = dot(T, H);
    return pow(sqrt(max(1.0 - TdH * TdH, 0.0)), shine);
}

vec4 shadeLayer(
    vec2 p, float t,
    float seed, float freq, float flow,
    vec3 darkCol, vec3 midCol, vec3 brightCol, vec3 specCol,
    float opacity, float shine,
    vec3 L1, vec3 L2, vec3 V,
    float sheenMul
) {
    vec3 fold = fabricFold(p, t, seed, freq, flow);
    float h = fold.x;
    vec2 grad = fold.yz;
    vec3 N = normalize(vec3(-grad * 1.8, 1.0));

    float NdL1 = max(dot(N, L1), 0.0);
    float NdL2 = max(dot(N, L2), 0.0);
    float lit = NdL1 * 0.75 + NdL2 * 0.12;

    float depth = smoothstep(-0.8, 0.4, h);

    float shade = lit * depth;
    float midBlend = smoothstep(0.0, 0.35, shade);
    float brightBlend = smoothstep(0.25, 0.7, shade);
    vec3 fabric = mix(darkCol, midCol, midBlend);
    fabric = mix(fabric, brightCol, brightBlend * 0.5);

    float sp = kajiyaSpec(grad, L1, V, shine) * 0.9;
    sp += kajiyaSpec(grad, L2, V, shine * 0.6) * 0.15;
    sp *= sheenMul;
    float specPow = sp * sp * sp;
    fabric += specCol * specPow * 0.9;

    float trans = smoothstep(0.3, 0.9, depth) * lit * 0.08;
    fabric += vec3(0.45, 0.28, 0.15) * trans;

    float sparkle = hash12(floor(p * 500.0 + t * 0.7));
    sparkle = step(0.9992, sparkle) * specPow * 20.0 * sheenMul;
    fabric += specCol * min(sparkle, 2.0);

    float alpha = opacity * (0.65 + depth * 0.35);
    return vec4(fabric, alpha);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);

    if (mousePos.x > 0.0) {
        vec2 mUV = mousePos / RENDERSIZE;
        vec2 mP = (mUV - 0.5) * vec2(aspect, 1.0);
        p -= mP * 0.5;
    }

    // Audio reactivity
    float t = TIME * flowSpeed * (1.0 + audioBass * 0.4);
    float audioSheen = sheenIntensity * (1.0 + audioLevel * 0.5);

    vec3 L1 = normalize(vec3(
        0.4 + sin(t * 0.07) * 0.3,
        0.9 + cos(t * 0.09) * 0.15,
        0.8
    ));
    vec3 L2 = normalize(vec3(
        -0.7 + cos(t * 0.06) * 0.2,
        -0.3 + sin(t * 0.08) * 0.15,
        0.6
    ));
    vec3 V = vec3(0.0, 0.0, 1.0);

    float bgD = length(p);
    vec3 bg = mix(
        vec3(0.055, 0.03, 0.075),
        vec3(0.012, 0.006, 0.02),
        smoothstep(0.0, 1.0, bgD)
    );
    bg += vec3(0.025, 0.012, 0.035) * exp(-bgD * bgD * 2.0);

    // Layer 1: Gold silk
    vec4 ly1 = shadeLayer(
        p * 0.8 + vec2(0.15, t * 0.015), t,
        0.0, 2.0, 0.5,
        vec3(0.10, 0.06, 0.02),
        vec3(0.50, 0.38, 0.15),
        vec3(0.80, 0.65, 0.32),
        vec3(1.0, 0.92, 0.65),
        0.30, 26.0,
        L1, L2, V,
        audioSheen * 0.7
    );

    // Layer 2: ShaderClaw red silk
    vec4 ly2 = shadeLayer(
        p * 1.0 + vec2(t * 0.012, -0.1), t,
        1.0, 3.2, 0.75,
        vec3(0.08, 0.02, 0.03),
        vec3(0.45, 0.12, 0.17),
        vec3(0.91, 0.25, 0.34),  // ShaderClaw red
        vec3(1.0, 0.82, 0.86),
        0.38, 40.0,
        L1, L2, V,
        audioSheen * 0.9
    );

    // Layer 3: Deep purple silk
    vec4 ly3 = shadeLayer(
        p * 1.2 + vec2(-t * 0.008, t * 0.02), t,
        2.0, 4.5, 1.0,
        vec3(0.06, 0.04, 0.10),
        vec3(0.30, 0.22, 0.45),
        vec3(0.58, 0.48, 0.72),
        vec3(1.0, 0.90, 0.97),
        0.50, 55.0,
        L1, L2, V,
        audioSheen
    );

    vec3 col = bg;
    col = mix(col, ly1.rgb, ly1.a);
    col += vec3(0.35, 0.18, 0.08) * ly1.a * ly2.a * 0.08;
    col = mix(col, ly2.rgb, ly2.a);
    col += vec3(0.30, 0.15, 0.25) * ly2.a * ly3.a * 0.06;
    col += vec3(0.40, 0.25, 0.12) * ly1.a * ly2.a * ly3.a * 0.04;
    col = mix(col, ly3.rgb, ly3.a);

    float cov = (ly1.a + ly2.a + ly3.a) * 0.333;
    col += vec3(0.35, 0.20, 0.12) * cov * 0.04;

    float vig = 1.0 - smoothstep(0.25, 1.15, length(p * vec2(0.85, 1.0)));
    col *= 0.6 + 0.4 * vig;

    float lum = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(vec3(lum), col, 1.35);

    col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);
    col = pow(max(col, 0.0), vec3(0.4545));

    float grain = hash12(gl_FragCoord.xy + fract(TIME * 7.13) * 100.0);
    col += (grain - 0.5) * 0.015;

    col *= baseColor.rgb;
    vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
    vec4 texSample = texture2D(inputTex, texUV);
    col = mix(col, col * texSample.rgb, texSample.a * 0.5);

    gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
