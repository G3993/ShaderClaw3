/*{
  "CATEGORIES": [
    "Radiant",
    "Geometric",
    "Gold"
  ],
  "DESCRIPTION": "Golden mosaic tiles with moving light sources, wave-flip animation, specular highlights, and micro-facet sparkle. From Radiant by Paul Bakaus (MIT).",
  "INPUTS": [
    {
      "NAME": "animMode",
      "LABEL": "Wave Flip",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 1
    },
    {
      "NAME": "tileScale",
      "LABEL": "Tile Scale",
      "TYPE": "float",
      "MIN": 0.3,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "waveSpeed",
      "LABEL": "Wave Speed",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 6,
      "DEFAULT": 4
    },
    {
      "NAME": "waveDelay",
      "LABEL": "Wave Delay",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 4,
      "DEFAULT": 1.5
    },
    {
      "NAME": "waveDir",
      "LABEL": "Wave Direction",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 3,
      "DEFAULT": 0
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

// Gilt Mosaic - Radiant Shaders Gallery (MIT License)

#define PI 3.14159265359
#define TAU 6.28318530718

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

vec2 hash2(vec2 p) {
    return vec2(
        fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453),
        fract(sin(dot(p, vec2(269.5, 183.3))) * 43758.5453)
    );
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

vec4 tileGrid(vec2 p, float scale) {
    vec2 sp = p * scale;
    vec2 id = floor(sp);
    vec2 f = fract(sp);
    vec2 jitter = hash2(id) * 0.12 - 0.06;
    f -= jitter;
    return vec4(f, id);
}

vec3 tileNormal(vec2 id) {
    float h1 = hash(id * 1.731 + 17.3);
    float h2 = hash(id * 2.419 + 31.7);
    float tiltX = (h1 - 0.5) * 0.35;
    float tiltY = (h2 - 0.5) * 0.35;
    return normalize(vec3(tiltX, tiltY, 1.0));
}

float groutMask(vec2 f, float groutWidth) {
    vec2 edge = smoothstep(vec2(0.0), vec2(groutWidth), f) *
                smoothstep(vec2(0.0), vec2(groutWidth), vec2(1.0) - f);
    return edge.x * edge.y;
}

float tileRoughness(vec2 f, vec2 id) {
    float n1 = noise(f * 8.0 + id * 3.7);
    float n2 = noise(f * 16.0 + id * 7.1 + 50.0);
    return n1 * 0.6 + n2 * 0.4;
}

vec2 waveFlip(vec2 tileCenter, float rawTime, vec2 aspect) {
    float sweepDur = waveSpeed;
    float cycleDur = sweepDur + waveDelay;
    float cycleT = mod(rawTime, cycleDur);
    float waveCount = floor(rawTime / cycleDur);
    float isOddWave = mod(waveCount, 2.0);
    float sweepRaw = clamp(cycleT / sweepDur, 0.0, 1.0);
    float sweep = sweepRaw * sweepRaw * (3.0 - 2.0 * sweepRaw);
    float dir = floor(waveDir + 0.5);
    float axisLen = aspect.x;
    float tilePos = tileCenter.x;
    if (dir == 1.0) { tilePos = aspect.x - tileCenter.x; }
    else if (dir == 2.0) { tilePos = 1.0 - tileCenter.y; axisLen = 1.0; }
    else if (dir == 3.0) { tilePos = tileCenter.y; axisLen = 1.0; }
    float waveX = sweep * (axisLen + 0.6) - 0.3;
    float tileRand = hash(tileCenter * 31.7 + vec2(17.3, 59.1));
    float stagger = tileRand * 0.06;
    float dist = tilePos - waveX + stagger;
    float flipProgress = smoothstep(0.5, -0.3, dist);
    float flipAngle = (isOddWave + flipProgress) * PI;
    float inTransition = smoothstep(0.6, 0.0, abs(dist + 0.1));
    return vec2(flipAngle, inTransition);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE;
    vec2 aspect = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    vec2 p = uv * aspect;
    float t = TIME * (1.0 + audioLevel * 0.3);

    float scale = 18.0 * tileScale;
    vec4 tile = tileGrid(p, scale);
    vec2 f = tile.xy;
    vec2 id = tile.zw;

    float groutW = 0.06;
    float tMask = groutMask(f, groutW);

    float tileHash = hash(id);
    float tileHash2 = hash(id + 200.0);
    float tileHash3 = hash(id + 400.0);
    vec3 N = tileNormal(id);

    float roughness = tileRoughness(f, id);
    N = normalize(N + vec3(
        (roughness - 0.5) * 0.12,
        (noise(f * 12.0 + id * 5.3) - 0.5) * 0.12,
        0.0
    ));

    vec2 tileCenter = (id + 0.5) / scale;
    vec2 flipData = waveFlip(tileCenter, TIME, aspect);
    float flipAngle = flipData.x * animMode;
    float inTransition = flipData.y * animMode;

    float cosFlip = cos(flipAngle);
    float abscos = abs(cosFlip);
    float dir = floor(waveDir + 0.5);
    bool flipVertical = (dir == 2.0 || dir == 3.0);

    if (flipVertical) {
        f.y = (f.y - 0.5) / max(abscos, 0.04) + 0.5;
    } else {
        f.x = (f.x - 0.5) / max(abscos, 0.04) + 0.5;
    }
    float inBounds = step(0.0, f.x) * step(f.x, 1.0) * step(0.0, f.y) * step(f.y, 1.0);
    tMask *= inBounds;

    float isBack = step(cosFlip, 0.0);

    float sinFlip = sin(flipAngle);
    vec3 flippedN;
    if (flipVertical) {
        flippedN = vec3(N.x, N.y * cosFlip + N.z * sinFlip, -N.y * sinFlip + N.z * cosFlip);
    } else {
        flippedN = vec3(N.x * cosFlip + N.z * sinFlip, N.y, -N.x * sinFlip + N.z * cosFlip);
    }
    N = normalize(mix(N, flippedN, animMode));

    if (isBack > 0.5) {
        vec3 backN = tileNormal(id + 500.0);
        float backRough = tileRoughness(f, id + 500.0);
        backN = normalize(backN + vec3(
            (backRough - 0.5) * 0.15,
            (noise(f * 14.0 + id * 3.7) - 0.5) * 0.15,
            0.0
        ));
        N = backN;
        roughness = backRough;
    }

    // Moving light sources - mouse can influence light 1
    vec3 light1Pos = vec3(
        aspect.x * 0.5 + sin(t * 0.7) * aspect.x * 0.4,
        0.5 + cos(t * 0.53) * 0.4,
        0.8 + sin(t * 0.31) * 0.15
    );
    if (mousePos.x > 0.0) {
        vec2 mUV = mousePos / RENDERSIZE * aspect;
        light1Pos.xy = mUV;
    }

    vec3 light2Pos = vec3(
        aspect.x * 0.5 + cos(t * 0.43 + 2.0) * aspect.x * 0.35,
        0.5 + sin(t * 0.37 + 1.5) * 0.35,
        0.7 + cos(t * 0.29) * 0.1
    );
    vec3 light3Pos = vec3(
        aspect.x * 0.5 + sin(t * 0.19 + 4.0) * aspect.x * 0.25,
        0.5 + cos(t * 0.23 + 3.0) * 0.25,
        1.2
    );

    vec3 tileWorldPos = vec3(p, 0.0);
    vec3 viewDir = normalize(vec3(aspect.x * 0.5, 0.5, 1.5) - tileWorldPos);

    vec3 L1 = normalize(light1Pos - tileWorldPos);
    vec3 H1 = normalize(L1 + viewDir);
    float NdotH1 = max(dot(N, H1), 0.0);
    float spec1 = pow(NdotH1, 80.0 + tileHash * 60.0);
    float diff1 = max(dot(N, L1), 0.0);

    vec3 L2 = normalize(light2Pos - tileWorldPos);
    vec3 H2 = normalize(L2 + viewDir);
    float NdotH2 = max(dot(N, H2), 0.0);
    float spec2 = pow(NdotH2, 60.0 + tileHash2 * 80.0);
    float diff2 = max(dot(N, L2), 0.0);

    vec3 L3 = normalize(light3Pos - tileWorldPos);
    vec3 H3 = normalize(L3 + viewDir);
    float NdotH3 = max(dot(N, H3), 0.0);
    float spec3 = pow(NdotH3, 30.0 + tileHash3 * 20.0);
    float diff3 = max(dot(N, L3), 0.0);

    float specTotal = spec1 * 1.2 + spec2 * 0.9 + spec3 * 0.4;
    float diffTotal = diff1 * 0.5 + diff2 * 0.35 + diff3 * 0.25;

    float breathe = 1.0 + sin(t * 0.6) * 0.08 + sin(t * 0.37 + 1.0) * 0.05;
    specTotal *= breathe;
    diffTotal *= breathe;

    float flipFlash = isBack * smoothstep(0.3, 0.7, inTransition);
    float flipGlow = inTransition * inTransition * 0.3;

    float shimmerPhase = tileHash * TAU + t * (0.8 + tileHash2 * 1.5);
    float shimmer = pow(max(sin(shimmerPhase), 0.0), 16.0);
    float shimmer2Phase = tileHash3 * TAU + t * (0.5 + tileHash * 0.7) + 2.0;
    float shimmer2 = pow(max(sin(shimmer2Phase), 0.0), 24.0);
    float shimmerBlend = 1.0 - animMode;
    float shimmerTotal = (shimmer * 0.6 + shimmer2 * 0.4) * shimmerBlend;

    vec3 groutColor = vec3(0.03, 0.02, 0.01);
    vec3 darkGold = vec3(0.12, 0.09, 0.05);
    vec3 medGold = vec3(0.45, 0.32, 0.14);
    vec3 brightGold = vec3(0.78, 0.58, 0.24);
    vec3 flashGold = vec3(1.0, 0.85, 0.55);
    vec3 hotGold = vec3(1.0, 0.95, 0.80);

    float baseVar = tileHash;
    vec3 tileBase = mix(darkGold, medGold, smoothstep(0.0, 0.5, baseVar));
    tileBase = mix(tileBase, brightGold, smoothstep(0.5, 0.85, baseVar));
    tileBase *= 0.9 + tileHash2 * 0.2;

    vec3 tileColor = tileBase;
    tileColor += tileBase * diffTotal * 0.6;

    vec3 specColor = mix(brightGold, flashGold, smoothstep(0.0, 0.5, specTotal));
    specColor = mix(specColor, hotGold, smoothstep(0.5, 1.0, specTotal));
    tileColor += specColor * specTotal * 1.4;

    vec3 shimmerColor = mix(flashGold, hotGold, shimmerTotal);
    tileColor += shimmerColor * shimmerTotal * 0.7;

    float flipShade = mix(1.0, abscos * 0.7 + 0.3, inTransition);
    tileColor *= flipShade;
    tileColor = mix(tileColor, tileColor * vec3(1.2, 1.05, 0.85), isBack * 0.6);
    float edgeOnGlow = pow(1.0 - abscos, 4.0) * inTransition;
    tileColor += medGold * edgeOnGlow * 0.25 * animMode;

    float microSpec = pow(roughness, 4.0) * specTotal * 3.0;
    tileColor += flashGold * microSpec * 0.3;

    float edgeDist = min(min(f.x, 1.0 - f.x), min(f.y, 1.0 - f.y));
    float edgeHighlight = smoothstep(0.15, 0.05, edgeDist);
    tileColor += brightGold * edgeHighlight * (diffTotal + specTotal * 0.5) * 0.15;

    vec3 col = mix(groutColor, tileColor, tMask);

    float groutDepth = 1.0 - tMask;
    col -= vec3(0.01, 0.008, 0.005) * groutDepth * (1.0 - smoothstep(0.0, 0.03, edgeDist));

    float glow1 = smoothstep(0.7, 0.0, length(p - light1Pos.xy));
    float glow2 = smoothstep(0.6, 0.0, length(p - light2Pos.xy));
    col += medGold * glow1 * 0.06;
    col += medGold * glow2 * 0.04;

    vec2 vigUv = uv * 2.0 - 1.0;
    float vig = 1.0 - dot(vigUv, vigUv) * 0.35;
    vig = max(vig, 0.0);
    vig = smoothstep(0.0, 1.0, vig);
    col *= 0.5 + vig * 0.5;

    col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);
    col = pow(col, vec3(0.95, 1.0, 1.1));

    col *= baseColor.rgb;
    vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
    vec4 texSample = texture2D(inputTex, texUV);
    col = mix(col, col * texSample.rgb, texSample.a * 0.5);

    gl_FragColor = vec4(col, 1.0);
}
