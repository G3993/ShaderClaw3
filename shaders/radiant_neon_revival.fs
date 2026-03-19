/*{
  "CATEGORIES": [
    "Radiant",
    "Neon",
    "SDF"
  ],
  "DESCRIPTION": "Neon sign shapes (crown, starburst, heart, lightning) with glow, flicker, reflections, and brick wall background. From Radiant by Paul Bakaus (MIT).",
  "INPUTS": [
    {
      "NAME": "shape",
      "LABEL": "Shape",
      "TYPE": "float",
      "MIN": 1,
      "MAX": 4,
      "DEFAULT": 1
    },
    {
      "NAME": "flickerRate",
      "LABEL": "Flicker Rate",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 1,
      "DEFAULT": 0.5
    },
    {
      "NAME": "glowIntensity",
      "LABEL": "Glow Spread",
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

// Neon Revival - Radiant Shaders Gallery (MIT License)

#define PI 3.14159265359
#define TAU 6.28318530718

float hash(float n) {
    return fract(sin(n) * 43758.5453123);
}

float hash2(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash2(i);
    float b = hash2(i + vec2(1.0, 0.0));
    float c = hash2(i + vec2(0.0, 1.0));
    float d = hash2(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

float sdCircle(vec2 p, vec2 center, float radius) {
    return abs(length(p - center) - radius);
}

float sdArc(vec2 p, vec2 center, float radius, float startAngle, float sweep) {
    vec2 d = p - center;
    float angle = atan(d.y, d.x);
    float a = mod(angle - startAngle + PI, TAU) - PI;
    if (a >= 0.0 && a <= sweep) {
        return abs(length(d) - radius);
    }
    vec2 e1 = center + radius * vec2(cos(startAngle), sin(startAngle));
    vec2 e2 = center + radius * vec2(cos(startAngle + sweep), sin(startAngle + sweep));
    return min(length(p - e1), length(p - e2));
}

float sdTriangle(vec2 p, vec2 a, vec2 b, vec2 c) {
    return min(min(sdSegment(p, a, b), sdSegment(p, b, c)), sdSegment(p, c, a));
}

float sdHeart(vec2 p, vec2 center, float size) {
    vec2 q = (p - center) / size;
    q.y -= 0.2;
    float a1 = sdArc(q, vec2(-0.28, 0.18), 0.36, 0.3, PI);
    float a2 = sdArc(q, vec2(0.28, 0.18), 0.36, -0.15, PI);
    float v1 = sdSegment(q, vec2(-0.56, -0.02), vec2(0.0, -0.65));
    float v2 = sdSegment(q, vec2(0.56, -0.02), vec2(0.0, -0.65));
    return min(min(a1, a2), min(v1, v2)) * size;
}

float sdStar(vec2 p, vec2 center, float outerR, float innerR) {
    float d = 1e10;
    for (int i = 0; i < 5; i++) {
        float fi = float(i);
        float a1 = fi * TAU / 5.0 - PI / 2.0;
        float a2 = (fi + 0.5) * TAU / 5.0 - PI / 2.0;
        float a3 = (fi + 1.0) * TAU / 5.0 - PI / 2.0;
        vec2 outer1 = center + outerR * vec2(cos(a1), sin(a1));
        vec2 inner1 = center + innerR * vec2(cos(a2), sin(a2));
        vec2 outer2 = center + outerR * vec2(cos(a3), sin(a3));
        d = min(d, sdSegment(p, outer1, inner1));
        d = min(d, sdSegment(p, inner1, outer2));
    }
    return d;
}

vec3 neonGlow(float dist, vec3 color, float tubeWidth, float brightness) {
    float core = exp(-dist * dist / (tubeWidth * tubeWidth * 0.3));
    float inner = exp(-dist * dist / (tubeWidth * tubeWidth * 2.5));
    float mid = exp(-dist * dist / (tubeWidth * tubeWidth * 12.0));
    float bloom = exp(-dist * dist / (tubeWidth * tubeWidth * 50.0));
    float scatter = exp(-dist * dist / (tubeWidth * tubeWidth * 200.0));
    vec3 white = vec3(1.0, 0.97, 0.95);
    vec3 result = white * core * 1.5;
    result += mix(white, color, 0.5) * inner * 1.0;
    result += color * mid * 0.6;
    result += color * bloom * 0.25;
    result += color * scatter * 0.08;
    return result * brightness;
}

float flicker(float t, float id, float rate) {
    float phase = hash(id * 13.37) * TAU;
    float speed = 3.0 + hash(id * 7.91) * 8.0;
    float cutout = step(0.92 - rate * 0.15, sin(t * speed * 2.3 + phase * 1.7));
    float dim = 1.0 - cutout * 0.7;
    float buzz = 0.97 + 0.03 * sin(t * 376.99);
    float pulse = 0.92 + 0.08 * sin(t * 0.5 + phase);
    return pulse * buzz * dim;
}

float fadeIn(float t, float delay, float duration) {
    return smoothstep(delay, delay + duration, t);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE * 0.5) / (min(RENDERSIZE.x, RENDERSIZE.y));
    float t = TIME;
    float tw = 0.012;

    // Audio reactivity: glow pulses with bass
    float audioGlow = glowIntensity * (1.0 + audioBass * 0.6);

    vec3 col = vec3(0.02, 0.018, 0.025);
    float brickNoise = noise(gl_FragCoord.xy * 0.15) * 0.015;
    float brickY = step(0.95, fract(uv.y * 12.0)) * 0.008;
    float brickX = step(0.95, fract((uv.x + step(0.5, fract(uv.y * 6.0)) * 0.04) * 8.0)) * 0.006;
    col += brickNoise - brickY - brickX;

    // ShaderClaw accent colors
    vec3 hotPink = vec3(0.91, 0.25, 0.34); // ShaderClaw red
    vec3 electricBlue = vec3(0.1, 0.5, 1.0);
    vec3 warmAmber = vec3(1.0, 0.65, 0.15);
    vec3 softPink = vec3(1.0, 0.35, 0.55);

    vec3 neon = vec3(0.0);
    float dist;
    float fl;
    float fi;

    // SHAPE 1: Crown
    if (shape < 1.5) {
        fi = 41.0;
        dist = sdSegment(uv, vec2(-0.32, -0.05), vec2(0.32, -0.05));
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.2, 1.5);
        neon += neonGlow(dist, warmAmber, tw, fl * audioGlow);

        fi = 42.0;
        dist = sdTriangle(uv, vec2(-0.06, -0.05), vec2(0.0, 0.32), vec2(0.06, -0.05));
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.4, 1.4);
        neon += neonGlow(dist, hotPink, tw, fl * audioGlow * 1.05);

        fi = 43.0;
        dist = sdTriangle(uv, vec2(-0.18, -0.05), vec2(-0.12, 0.18), vec2(-0.06, -0.05));
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.55, 1.4);
        neon += neonGlow(dist, hotPink, tw, fl * audioGlow);

        fi = 44.0;
        dist = sdTriangle(uv, vec2(0.06, -0.05), vec2(0.12, 0.18), vec2(0.18, -0.05));
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.6, 1.4);
        neon += neonGlow(dist, hotPink, tw, fl * audioGlow);

        fi = 45.0;
        dist = sdTriangle(uv, vec2(-0.32, -0.05), vec2(-0.25, 0.12), vec2(-0.18, -0.05));
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.7, 1.4);
        neon += neonGlow(dist, softPink, tw, fl * audioGlow * 0.9);

        fi = 46.0;
        dist = sdTriangle(uv, vec2(0.18, -0.05), vec2(0.25, 0.12), vec2(0.32, -0.05));
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.75, 1.4);
        neon += neonGlow(dist, softPink, tw, fl * audioGlow * 0.9);

        fi = 47.0;
        float jp = 0.8 + 0.2 * sin(t * 1.9);
        float dj1 = sdCircle(uv, vec2(0.0, 0.32), 0.018);
        float dj2 = sdCircle(uv, vec2(-0.12, 0.18), 0.018);
        float dj3 = sdCircle(uv, vec2(0.12, 0.18), 0.018);
        float dj4 = sdCircle(uv, vec2(-0.25, 0.12), 0.018);
        float dj5 = sdCircle(uv, vec2(0.25, 0.12), 0.018);
        dist = min(min(min(dj1, dj2), min(dj3, dj4)), dj5);
        fl = flicker(t, fi, flickerRate * 0.5) * fadeIn(t, 1.2, 1.0) * jp;
        neon += neonGlow(dist, warmAmber, tw * 0.65, fl * audioGlow * 1.15);

        fi = 48.0;
        float sp = 0.85 + 0.15 * sin(t * 1.7);
        dist = sdStar(uv, vec2(-0.50, 0.05), 0.06, 0.025);
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 1.0, 1.2) * sp;
        neon += neonGlow(dist, electricBlue, tw * 0.85, fl * audioGlow);

        fi = 49.0;
        float sp2 = 0.85 + 0.15 * sin(t * 1.7 + 1.2);
        dist = sdStar(uv, vec2(0.50, 0.05), 0.06, 0.025);
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 1.1, 1.2) * sp2;
        neon += neonGlow(dist, electricBlue, tw * 0.85, fl * audioGlow);
    }

    // SHAPE 2: Starburst
    else if (shape < 2.5) {
        fi = 21.0;
        float dotPulse = 0.85 + 0.15 * sin(t * 2.2);
        dist = sdCircle(uv, vec2(0.0, 0.0), 0.04);
        fl = flicker(t, fi, flickerRate * 0.4) * fadeIn(t, 0.2, 1.0) * dotPulse;
        neon += neonGlow(dist, hotPink, tw * 0.6, fl * audioGlow * 1.3);

        fi = 22.0;
        dist = sdCircle(uv, vec2(0.0, 0.0), 0.38);
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.3, 1.5);
        neon += neonGlow(dist, warmAmber, tw * 0.9, fl * audioGlow * 0.9);

        // 8 rays
        float rayLen = 0.32;
        for (int i = 0; i < 8; i++) {
            float angle = float(i) * TAU / 8.0;
            vec2 inner = vec2(cos(angle), sin(angle)) * 0.06;
            vec2 outer = vec2(cos(angle), sin(angle)) * rayLen;
            vec3 rayColor = mod(float(i), 2.0) < 0.5 ? hotPink : electricBlue;
            fi = 23.0 + float(i);
            dist = sdSegment(uv, inner, outer);
            fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.5 + float(i) * 0.1, 1.2);
            neon += neonGlow(dist, rayColor, tw * 0.75, fl * audioGlow * 0.9);
        }
    }

    // SHAPE 3: Heart
    else if (shape < 3.5) {
        fi = 1.0;
        float heartPulse = 0.9 + 0.1 * sin(t * 1.4);
        dist = sdHeart(uv, vec2(0.0, 0.10), 0.28);
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.2, 1.5) * heartPulse;
        neon += neonGlow(dist, hotPink, tw, fl * audioGlow * 1.1);

        fi = 2.0;
        dist = sdSegment(uv, vec2(-0.48, 0.10), vec2(0.48, 0.10));
        fl = flicker(t, fi, flickerRate * 0.7) * fadeIn(t, 0.5, 1.2);
        neon += neonGlow(dist, softPink, tw * 0.55, fl * audioGlow * 0.75);

        fi = 3.0;
        float sp3 = 0.85 + 0.15 * sin(t * 1.6);
        dist = sdStar(uv, vec2(-0.48, 0.10), 0.065, 0.028);
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.8, 1.2) * sp3;
        neon += neonGlow(dist, warmAmber, tw * 0.85, fl * audioGlow);

        fi = 4.0;
        float sp4 = 0.85 + 0.15 * sin(t * 1.6 + 1.0);
        dist = sdStar(uv, vec2(0.48, 0.10), 0.065, 0.028);
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.9, 1.2) * sp4;
        neon += neonGlow(dist, warmAmber, tw * 0.85, fl * audioGlow);

        fi = 5.0;
        float jp2 = 0.8 + 0.2 * sin(t * 2.1);
        dist = sdCircle(uv, vec2(0.0, -0.14), 0.02);
        fl = flicker(t, fi, flickerRate * 0.5) * fadeIn(t, 1.3, 1.0) * jp2;
        neon += neonGlow(dist, electricBlue, tw * 0.6, fl * audioGlow * 1.2);
    }

    // SHAPE 4: Lightning Bolt
    else {
        fi = 61.0;
        dist = sdSegment(uv, vec2(0.0, 0.35), vec2(0.14, 0.12));
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.2, 1.5);
        neon += neonGlow(dist, hotPink, tw, fl * audioGlow * 1.1);

        fi = 62.0;
        dist = sdSegment(uv, vec2(0.14, 0.12), vec2(-0.04, 0.08));
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.4, 1.4);
        neon += neonGlow(dist, hotPink, tw, fl * audioGlow * 1.1);

        fi = 63.0;
        dist = sdSegment(uv, vec2(-0.04, 0.08), vec2(0.10, -0.12));
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 0.6, 1.4);
        neon += neonGlow(dist, hotPink, tw, fl * audioGlow * 1.1);

        fi = 66.0;
        float ep = 0.8 + 0.2 * sin(t * 2.0);
        dist = sdCircle(uv, vec2(0.0, 0.35), 0.025);
        fl = flicker(t, fi, flickerRate * 0.5) * fadeIn(t, 1.2, 1.0) * ep;
        neon += neonGlow(dist, warmAmber, tw * 0.7, fl * audioGlow * 1.2);

        fi = 67.0;
        float ep2 = 0.8 + 0.2 * sin(t * 2.0 + 1.5);
        dist = sdCircle(uv, vec2(0.10, -0.12), 0.025);
        fl = flicker(t, fi, flickerRate * 0.5) * fadeIn(t, 1.3, 1.0) * ep2;
        neon += neonGlow(dist, warmAmber, tw * 0.7, fl * audioGlow * 1.2);

        fi = 68.0;
        float spp = 0.85 + 0.15 * sin(t * 1.8);
        dist = sdStar(uv, vec2(-0.30, 0.12), 0.055, 0.023);
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 1.0, 1.2) * spp;
        neon += neonGlow(dist, electricBlue, tw * 0.8, fl * audioGlow);

        fi = 69.0;
        float spp2 = 0.85 + 0.15 * sin(t * 1.8 + 1.3);
        dist = sdStar(uv, vec2(0.30, 0.12), 0.055, 0.023);
        fl = flicker(t, fi, flickerRate) * fadeIn(t, 1.1, 1.2) * spp2;
        neon += neonGlow(dist, electricBlue, tw * 0.8, fl * audioGlow);

        fi = 70.0;
        float rp = 0.7 + 0.3 * sin(t * 0.9);
        dist = sdCircle(uv, vec2(0.05, 0.115), 0.24);
        fl = flicker(t, fi, flickerRate * 0.6) * fadeIn(t, 0.9, 1.5) * rp;
        neon += neonGlow(dist, softPink, tw * 0.6, fl * audioGlow * 0.7);
    }

    // Full sign flicker
    float fullFlick = 1.0;
    float flickerCycle = mod(t, 12.0);
    if (flickerCycle > 8.0 && flickerCycle < 8.15) fullFlick = 0.1;
    if (flickerCycle > 8.18 && flickerCycle < 8.22) fullFlick = 0.05;
    if (flickerCycle > 8.28 && flickerCycle < 8.32) fullFlick = 0.15;
    neon *= fullFlick;

    // Reflection
    float reflY = -0.20;
    if (shape < 1.5) reflY = -0.14;
    else if (shape < 2.5) reflY = -0.44;
    if (uv.y < reflY) {
        vec2 rUV = vec2(uv.x, 2.0 * reflY - uv.y);
        float reflDist = reflY - uv.y;
        float reflAtten = exp(-reflDist * 4.0) * 0.2;
        rUV.x += sin(uv.y * 25.0 + t * 2.0) * 0.005;
        float reflLum = dot(neon, vec3(0.3, 0.5, 0.2));
        col += neon * reflAtten * 0.3;
    }

    col += neon;

    float vDist = length(uv);
    float vignette = 1.0 - smoothstep(0.5, 1.4, vDist);
    col *= 0.65 + vignette * 0.35;

    col = col / (1.0 + col * 0.3);
    col = pow(col, vec3(0.95, 1.0, 1.05));

    col *= baseColor.rgb;
    vec2 texUV = gl_FragCoord.xy / RENDERSIZE;
    vec4 texSample = texture2D(inputTex, texUV);
    col = mix(col, col * texSample.rgb, texSample.a * 0.5);

    gl_FragColor = vec4(col, 1.0);
}
