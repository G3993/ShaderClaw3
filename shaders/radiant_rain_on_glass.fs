/*{
  "CATEGORIES": [
    "Radiant",
    "Weather",
    "Noise"
  ],
  "DESCRIPTION": "Procedural rain drops running down glass with refraction distortion. Inspired by Radiant by Paul Bakaus (MIT).",
  "INPUTS": [
    {
      "NAME": "rainAmount",
      "LABEL": "Rain Amount",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "refraction",
      "LABEL": "Refraction",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 3,
      "DEFAULT": 1
    },
    {
      "NAME": "dropSpeed",
      "LABEL": "Drop Speed",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 2,
      "DEFAULT": 0.7
    },
    {
      "NAME": "bgColor",
      "LABEL": "Background Color",
      "TYPE": "color",
      "DEFAULT": [
        0.91,
        0.25,
        0.34,
        1
      ]
    }
  ]
}*/

// Rain on Glass - Procedural rain effect
// Inspired by Radiant Shaders Gallery (MIT License)

#define PI 3.14159265359

float hash(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 rot = mat2(0.8, 0.6, -0.6, 0.8);
    for (int i = 0; i < 4; i++) {
        v += a * noise(p);
        p = rot * p * 2.0;
        a *= 0.5;
    }
    return v;
}

// Generate a single raindrop trail
vec3 raindrop(vec2 uv, float t, float seed) {
    float speed = dropSpeed * (0.6 + hash(vec2(seed, 0.0)) * 0.8);
    float xPos = hash(vec2(seed * 1.23, seed * 4.56));
    float startTime = hash(vec2(seed * 7.89, seed * 0.12)) * 8.0;

    // Drop position
    float dropX = xPos;
    float dropY = 1.0 - mod((t + startTime) * speed * 0.15, 1.4);

    // Wobble
    dropX += sin(dropY * 8.0 + seed * 3.0) * 0.008;
    dropX += sin(dropY * 15.0 + seed * 7.0) * 0.003;

    vec2 dropCenter = vec2(dropX, dropY);
    vec2 delta = uv - dropCenter;

    // Drop shape: elongated vertically
    float dx = delta.x * 18.0;
    float dy = delta.y * 8.0;
    float dropDist = dx * dx + dy * dy;

    // Main drop
    float drop = smoothstep(1.0, 0.0, dropDist);

    // Trail behind the drop
    float trail = 0.0;
    if (delta.y > 0.0 && delta.y < 0.3) {
        float trailWidth = 0.012 * smoothstep(0.3, 0.0, delta.y);
        trail = smoothstep(trailWidth, 0.0, abs(delta.x - sin(delta.y * 20.0 + seed * 5.0) * 0.003));
        trail *= smoothstep(0.3, 0.01, delta.y);

        // Trail beads
        float beadPhase = fract(delta.y * 30.0 + t * speed * 0.5 + seed);
        float bead = smoothstep(0.3, 0.5, beadPhase) * smoothstep(0.7, 0.5, beadPhase);
        trail *= 0.3 + bead * 0.7;
    }

    float alpha = drop * 0.8 + trail * 0.4;
    // Refraction offset = gradient of the drop shape
    float refractX = -delta.x * drop * 20.0;
    float refractY = -delta.y * drop * 10.0 - trail * 2.0;

    return vec3(refractX, refractY, alpha);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;

    // Audio reactivity: bass affects rain intensity
    float audioRain = 1.0 + audioBass * 1.5;

    // Number of drops
    int numDrops = int(30.0 * rainAmount * audioRain);

    // Accumulate refraction and alpha from all drops
    vec2 totalRefract = vec2(0.0);
    float totalAlpha = 0.0;

    for (int i = 0; i < 60; i++) {
        if (i >= numDrops) break;
        float seed = float(i) + 0.5;
        vec3 dropData = raindrop(vec2(uv.x / aspect * aspect, uv.y), t, seed);
        totalRefract += dropData.xy * refraction;
        totalAlpha = max(totalAlpha, dropData.z);
    }

    // Small static droplets (condensation)
    vec2 condGrid = floor(uv * 80.0 * rainAmount);
    float condHash = hash(condGrid);
    if (condHash > 0.92) {
        vec2 condF = fract(uv * 80.0 * rainAmount) - 0.5;
        float condDist = length(condF);
        float condDrop = smoothstep(0.25, 0.0, condDist);
        float condShimmer = 0.5 + 0.5 * sin(TIME * 0.5 + condHash * 20.0);
        totalRefract += condF * condDrop * refraction * 0.3;
        totalAlpha = max(totalAlpha, condDrop * 0.3 * condShimmer);
    }

    // Background: blurred colored gradient with refraction
    vec2 bgUV = uv + totalRefract * 0.05;

    // City lights / bokeh background
    vec3 bg = vec3(0.0);

    // Large bokeh circles
    for (int i = 0; i < 12; i++) {
        float fi = float(i);
        vec2 bokehPos = vec2(
            hash(vec2(fi * 1.3, fi * 2.7)),
            hash(vec2(fi * 3.1, fi * 0.9))
        );
        float bokehR = 0.04 + hash(vec2(fi * 5.5, 0.0)) * 0.06;
        float bokehDist = length(bgUV - bokehPos);
        float bokeh = smoothstep(bokehR, bokehR * 0.3, bokehDist);

        // Color varies per bokeh
        vec3 bokehColor = mix(
            bgColor.rgb,
            vec3(1.0, 0.8, 0.4),
            hash(vec2(fi * 7.7, fi * 1.1))
        );
        bokehColor = mix(bokehColor, vec3(0.3, 0.5, 1.0), hash(vec2(fi * 2.2, fi * 8.8)) * 0.5);
        bg += bokehColor * bokeh * (0.3 + 0.7 * hash(vec2(fi * 4.4, 0.0)));
    }

    // Ambient city glow
    bg += bgColor.rgb * 0.1 * smoothstep(0.0, 0.5, bgUV.y);
    bg += vec3(0.05, 0.03, 0.08);

    // Dark gradient for depth
    bg *= 0.5 + 0.5 * smoothstep(1.5, 0.0, length(uv - 0.5));

    // Glass wetness tint
    vec3 col = bg;

    // Water droplet highlight/refraction effect
    float highlight = totalAlpha * 1.5;
    col += vec3(0.15, 0.18, 0.22) * highlight;

    // Specular on drops
    float spec = pow(totalAlpha, 3.0) * 2.0;
    col += vec3(1.0, 0.97, 0.92) * spec;

    // Subtle running water streaks
    float streakNoise = fbm(vec2(uv.x * 20.0, uv.y * 5.0 - TIME * dropSpeed * 0.3));
    float streak = smoothstep(0.5, 0.7, streakNoise) * 0.05;
    col += vec3(0.1, 0.12, 0.15) * streak;

    // Vignette
    float vig = 1.0 - smoothstep(0.4, 1.2, length(uv - 0.5));
    col *= 0.7 + 0.3 * vig;

    // Tone mapping
    col = col / (1.0 + col * 0.3);

    gl_FragColor = vec4(col, 1.0);
}
