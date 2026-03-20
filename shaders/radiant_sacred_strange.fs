/*{
  "CATEGORIES": [
    "Radiant",
    "Geometric",
    "Sacred"
  ],
  "DESCRIPTION": "Sacred geometry patterns with golden spirals, polar-folded SDF stars, hex/square tiling, and glowing line rendering. From Radiant by Paul Bakaus (MIT).",
  "INPUTS": [
    {
      "NAME": "rotationSpeed",
      "LABEL": "Rotation Speed",
      "TYPE": "float",
      "MIN": 0.05,
      "MAX": 1,
      "DEFAULT": 0.3
    },
    {
      "NAME": "complexity",
      "LABEL": "Complexity",
      "TYPE": "float",
      "MIN": 0.3,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "pattern",
      "LABEL": "Dimensional Shift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.05
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

// Sacred Strange - Radiant Shaders Gallery (MIT License)

#define PI 3.14159265359
#define TAU 6.28318530718
#define PHI 1.6180339887
#define SQRT3 1.7320508

mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

vec3 gold(float t) {
    vec3 a = vec3(0.45, 0.32, 0.14);
    vec3 b = vec3(0.45, 0.35, 0.2);
    vec3 c = vec3(1.0, 0.8, 0.5);
    vec3 d = vec3(0.0, 0.1, 0.25);
    return a + b * cos(TAU * (c * t + d));
}

vec2 polarFold(vec2 p, float n) {
    float angle = atan(p.y, p.x);
    float sector = TAU / n;
    angle = mod(angle + sector * 0.5, sector) - sector * 0.5;
    angle = abs(angle);
    float r = length(p);
    return vec2(cos(angle), sin(angle)) * r;
}

float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a;
    vec2 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

float sdRing(vec2 p, float r) {
    return abs(length(p) - r);
}

float sdPolygon(vec2 p, float r, float n) {
    float angle = atan(p.y, p.x);
    float sector = TAU / n;
    float a = mod(angle + sector * 0.5, sector) - sector * 0.5;
    float rp = length(p);
    vec2 q = vec2(cos(a), abs(sin(a))) * rp;
    vec2 edge = vec2(cos(sector * 0.5), sin(sector * 0.5)) * r;
    vec2 d = q - edge * clamp(dot(q, edge) / dot(edge, edge), 0.0, 1.0);
    return length(d) * sign(q.x * edge.y - q.y * edge.x);
}

vec3 glowLine(float d, vec3 coreColor, vec3 bloomColor, float lineWidth, float bloomWidth) {
    float core = lineWidth / (abs(d) + lineWidth);
    core = pow(core, 2.0);
    float bloom = bloomWidth / (abs(d) + bloomWidth);
    bloom = pow(bloom, 1.5);
    return coreColor * core + bloomColor * bloom * 0.4;
}

float starMotif(vec2 p, float symmetry, float breathe, float innerRatio, float petalInfluence) {
    float d = 1e9;
    float doubleSym = symmetry * 2.0;
    vec2 fp = polarFold(p, doubleSym);
    vec2 a1 = vec2(0.5 * breathe, 0.0);
    vec2 b1 = vec2(mix(0.35, 0.32, innerRatio) * breathe, mix(0.15, 0.13, innerRatio) * breathe);
    d = min(d, sdSegment(fp, a1, b1));
    float innerR = mix(0.22, 0.18, innerRatio);
    vec2 a2 = b1;
    vec2 b2 = vec2(innerR * breathe, 0.0);
    d = min(d, sdSegment(fp, a2, b2));
    vec2 a3 = b2;
    vec2 b3 = vec2(mix(0.12, 0.10, innerRatio) * breathe, mix(0.07, 0.06, innerRatio) * breathe);
    d = min(d, sdSegment(fp, a3, b3));
    vec2 a4 = b3;
    vec2 b4 = vec2(0.0, 0.0);
    d = min(d, sdSegment(fp, a4, b4));
    d = min(d, sdRing(p, 0.5 * breathe));
    d = min(d, sdRing(p, innerR * breathe));
    if (petalInfluence > 0.01) {
        float petalR = 0.5 * breathe * 0.35 / PHI;
        vec2 petalCenter = vec2(0.5 * breathe, 0.0);
        float petal = abs(length(fp - petalCenter) - petalR) - 0.002;
        d = min(d, mix(1e9, petal, petalInfluence));
        vec2 innerPetalCenter = vec2(0.5 * breathe * 0.65, 0.0);
        float innerPetalR = 0.5 * breathe * 0.25;
        float innerPetal = abs(length(fp - innerPetalCenter) - innerPetalR) - 0.0015;
        d = min(d, mix(1e9, innerPetal, petalInfluence));
    }
    return d;
}

float hexTileMotif(vec2 p, float scale, float symmetry, float breathe, float innerRatio, float petalInfl) {
    p *= scale;
    vec2 s = vec2(1.0, SQRT3);
    vec2 h = s * 0.5;
    vec2 a = mod(p, s) - h;
    vec2 b = mod(p - h, s) - h;
    vec2 gUV = dot(a, a) < dot(b, b) ? a : b;
    float d = starMotif(gUV, symmetry, breathe, innerRatio, petalInfl);
    return d / scale;
}

float centralMotif(vec2 p, float scale, float symmetry, float breathe, float innerRatio, float petalInfl) {
    p *= scale;
    float d = starMotif(p, symmetry, breathe, innerRatio, petalInfl);
    return d / scale;
}

float geometryLayer(vec2 p, float scale, float symmetry, float breathe, float innerRatio, float petalInfl, float tilingMix) {
    float tiledScale = scale;
    float centralScale = scale * 0.35;
    if (tilingMix < 0.01) {
        return hexTileMotif(p, tiledScale, symmetry, breathe, innerRatio, petalInfl);
    } else if (tilingMix > 0.99) {
        return centralMotif(p, centralScale, symmetry, breathe, innerRatio, petalInfl);
    } else {
        float dTiled = hexTileMotif(p, mix(tiledScale, tiledScale * 0.6, tilingMix), symmetry, breathe, innerRatio, petalInfl);
        float dCentral = centralMotif(p, mix(centralScale * 1.5, centralScale, tilingMix), symmetry, breathe, innerRatio, petalInfl);
        return mix(dTiled, dCentral, tilingMix);
    }
}

float goldenSpiral(vec2 uv, float t, float rotSpd) {
    float r = length(uv);
    float a = atan(uv.y, uv.x);
    float spiralPhase = log(max(r, 0.001)) / log(PHI) * PI * 0.5;
    float spiralD = abs(mod(a - spiralPhase + t * rotSpd * 0.2 + PI, TAU) - PI);
    spiralD = min(spiralD, abs(mod(a - spiralPhase + t * rotSpd * 0.2 + PI + PI, TAU) - PI));
    float fade = smoothstep(0.0, 0.05, r) * smoothstep(0.5, 0.35, r);
    return spiralD * fade + (1.0 - fade);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
    float t = TIME;
    float rotSpeed = rotationSpeed * (1.0 + audioLevel * 0.3);
    float pat = clamp(pattern, 0.0, 1.0);
    float r = length(uv);

    float tilingMix = pat;
    float baseSym = mix(6.0, 10.0, pat);
    float spiralInfluence = smoothstep(0.2, 0.8, pat);
    float centralGlowStr = mix(0.03, 0.12, pat);
    float petalInfluence = smoothstep(0.3, 0.9, pat);
    float objectRadius = mix(1.0, 0.48, pat * pat);
    float objectFade = mix(1.0, smoothstep(objectRadius, objectRadius * 0.7, r), pat);
    float breathe = 1.0 + 0.03 * sin(t * 0.6);

    // ShaderClaw accent in bright gold tones
    vec3 dimGold = vec3(0.35, 0.25, 0.12);
    vec3 medGold = vec3(0.7, 0.50, 0.22);
    vec3 brightGold = vec3(0.95, 0.72, 0.32);
    vec3 coreGlow = vec3(1.0, 0.82, 0.55);
    vec3 hotGold = vec3(1.0, 0.92, 0.72);

    vec3 col = vec3(0.02, 0.015, 0.01);

    float numLayers = 2.0 + (complexity - 0.3) * (4.0 / 1.7);
    numLayers = clamp(numLayers, 2.0, 6.0);

    // Layer 1
    {
        float sym1 = floor(baseSym);
        float rot1 = t * rotSpeed * 0.04;
        vec2 p1 = rot(rot1) * uv;
        float scale1 = mix(2.5, 1.8, pat) * complexity;
        float d1 = geometryLayer(p1, scale1, sym1, breathe, 0.0, petalInfluence * 0.3, tilingMix);
        col += glowLine(d1, medGold, dimGold * 1.5, 0.002, 0.016);
    }

    // Layer 2
    if (numLayers > 2.0) {
        float sym2 = floor(baseSym + 2.0);
        float rot2 = -t * rotSpeed * 0.06;
        vec2 p2 = rot(rot2) * uv;
        float scale2 = mix(3.5, 2.2, pat) * complexity;
        float innerR2 = mix(0.3, 0.6, pat);
        float d2 = geometryLayer(p2, scale2, sym2, breathe, innerR2, petalInfluence * 0.5, tilingMix);
        float layer2alpha = min(numLayers - 2.0, 1.0);
        col += glowLine(d2, brightGold * 0.8, dimGold * 1.2, 0.0015, 0.014) * layer2alpha;
    }

    // Layer 3
    if (numLayers > 3.0) {
        float sym3 = floor(mix(5.0, 8.0, pat));
        float rot3 = t * rotSpeed * 0.09;
        vec2 p3 = rot(rot3) * uv;
        float scale3 = mix(4.0, 2.5, pat) * complexity;
        float d3 = geometryLayer(p3, scale3, sym3, breathe * (1.0 + 0.01 * sin(t * 0.5 + 2.0)), 0.5, petalInfluence * 0.7, tilingMix);
        float layer3alpha = min(numLayers - 3.0, 1.0);
        col += glowLine(d3, coreGlow * 0.6, medGold * 0.7, 0.0012, 0.012) * layer3alpha;
    }

    // Layer 4
    if (numLayers > 4.0) {
        float sym4 = floor(mix(8.0, 12.0, pat));
        float rot4 = t * rotSpeed * mix(0.03, 0.05, pat);
        vec2 p4 = rot(rot4) * uv;
        float d4 = centralMotif(p4, mix(2.2, 1.8, pat), sym4, breathe, 0.7, petalInfluence);
        float centralFade = smoothstep(mix(0.5, 0.48, pat), 0.12, r);
        float layer4alpha = min(numLayers - 4.0, 1.0);
        col += glowLine(d4, hotGold * 0.7, coreGlow * 0.5, 0.0025, 0.022) * centralFade * layer4alpha;
    }

    // Layer 5
    if (numLayers > 5.0) {
        float ringBreath = 1.0 + 0.04 * sin(t * 0.4);
        float rScale = mix(0.12, 0.10, pat) * ringBreath;
        float ringD = 1e9;
        float polySides1 = mix(6.0, 8.0, pat);
        float polySides2 = mix(8.0, 10.0, pat);
        float pr0 = abs(sdPolygon(rot(t * rotSpeed * 0.02) * uv, rScale * 1.0, polySides1));
        float pr1 = abs(sdPolygon(rot(-t * rotSpeed * 0.025) * uv, rScale * 2.0, polySides2));
        float pr2 = abs(sdPolygon(rot(t * rotSpeed * 0.015) * uv, rScale * 3.0, 12.0));
        float pr3 = abs(sdPolygon(rot(-t * rotSpeed * 0.018) * uv, rScale * 4.0, polySides1));
        ringD = min(ringD, pr0);
        ringD = min(ringD, pr1);
        ringD = min(ringD, pr2);
        ringD = min(ringD, pr3);
        float layer5alpha = min(numLayers - 5.0, 1.0);
        col += glowLine(ringD, brightGold * 0.5, dimGold * 0.5, 0.0012, 0.01) * layer5alpha;
    }

    // Golden spiral arms
    if (spiralInfluence > 0.01) {
        float spiral = goldenSpiral(uv, t, rotSpeed);
        float spiralGlow = 0.015 / (spiral + 0.015);
        col += gold(0.7 + t * 0.01) * spiralGlow * 0.25 * spiralInfluence;
    }

    // Decorative elements
    if (pat > 0.3) {
        float decoFade = smoothstep(0.3, 0.7, pat);
        float outerRing = abs(r - objectRadius + 0.01) - 0.003;
        float outerGlow = 0.004 / (abs(outerRing) + 0.004);
        col += gold(0.5 + t * 0.015) * outerGlow * 0.35 * decoFade;
        float outerRing2 = abs(r - objectRadius + 0.035) - 0.002;
        float outerGlow2 = 0.003 / (abs(outerRing2) + 0.003);
        col += gold(0.6) * outerGlow2 * 0.2 * decoFade;
        float dotAngle = atan(uv.y, uv.x);
        float dotSymmetry = mix(6.0, 12.0, pat);
        float dotA = mod(dotAngle + PI / dotSymmetry, TAU / dotSymmetry) - PI / dotSymmetry;
        vec2 dotP = vec2(cos(dotA), sin(dotA)) * r;
        vec2 dotCenter = vec2(objectRadius - 0.01, 0.0);
        float dotD = length(dotP - dotCenter) - 0.008;
        float dotGlow = 0.004 / (abs(dotD) + 0.004);
        col += vec3(1.0, 0.9, 0.65) * dotGlow * 0.3 * decoFade;
    }

    // Center pulse
    float centerPulse = 0.8 + 0.2 * sin(t * 1.5);
    float centerGlowD = exp(-r * r * mix(3.0, 6.0, pat)) * centralGlowStr * 2.0 * centerPulse;
    col += hotGold * centerGlowD;

    if (pat > 0.2) {
        float ringPulse = 0.9 + 0.1 * sin(t * 2.3 + 1.0);
        float innerRing = abs(r - 0.03 * ringPulse) - 0.002;
        float innerRingGlow = 0.003 / (abs(innerRing) + 0.003);
        col += vec3(1.0, 0.88, 0.6) * innerRingGlow * 0.3 * smoothstep(0.2, 0.6, pat);
    }

    float ambientGlow = exp(-r * r * mix(2.5, 4.0, pat)) * mix(0.1, 0.14, pat);
    col += vec3(0.5, 0.35, 0.16) * ambientGlow;

    col *= objectFade;
    float pulse = 0.92 + 0.08 * sin(t * 0.3);
    col *= pulse;
    float vig = 1.0 - r * r * mix(0.35, 0.25, pat);
    vig = max(vig, 0.0);
    col *= (0.65 + vig * 0.35);
    col = col / (1.0 + col * 0.3);
    col = pow(col, vec3(0.95, 0.98, 1.06));

    // Texture as source — shader VFX processes the input content
    vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 texSample = texture2D(inputTex, texUV);
    if (texSample.a > 0.01) {
        // Blend: texture is the source, shader effect modulates it
        float effectStrength = max(col.r, max(col.g, col.b));
        col = mix(texSample.rgb, col, 0.5) * (0.5 + effectStrength * 0.5);
        col *= baseColor.rgb;
    } else {
        col *= baseColor.rgb;
    }

    gl_FragColor = vec4(col, 1.0);
}
