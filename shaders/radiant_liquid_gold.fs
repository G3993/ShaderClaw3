/*{
  "CATEGORIES": ["Radiant", "Organic", "Noise"],
  "DESCRIPTION": "Viscous liquid gold with domain-warped FBM, metaballs, Fresnel reflections, and specular highlights. From Radiant by Paul Bakaus (MIT).",
  "INPUTS": [
    { "NAME": "flowSpeed", "LABEL": "Flow Speed", "TYPE": "float", "MIN": 0.1, "MAX": 1.5, "DEFAULT": 0.4 },
    { "NAME": "viscosity", "LABEL": "Viscosity", "TYPE": "float", "MIN": 0.2, "MAX": 1.0, "DEFAULT": 0.6 },
    { "NAME": "mousePos", "LABEL": "Mouse Position", "TYPE": "point2D", "DEFAULT": [0.0, 0.0] },
    { "NAME": "audioLevel", "LABEL": "Audio Level", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "audioBass", "LABEL": "Audio Bass", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 }
  ]
}*/

// Liquid Gold - Radiant Shaders Gallery (MIT License)

#define PI 3.14159265359

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
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

float fbm(vec2 p, float t, float visc) {
    float val = 0.0;
    float amp = 0.5;
    float freq = 1.0;
    float decay = 0.45 + visc * 0.2;
    for (int i = 0; i < 6; i++) {
        val += amp * noise(p * freq + t);
        freq *= 2.0 + visc * 0.3;
        amp *= decay;
        p += vec2(1.7, 9.2);
    }
    return val;
}

float warpedField(vec2 p, float t, float visc) {
    vec2 q = vec2(
        fbm(p + vec2(0.0, 0.0), t * 0.5, visc),
        fbm(p + vec2(5.2, 1.3), t * 0.5, visc)
    );
    vec2 r = vec2(
        fbm(p + 3.0 * q + vec2(1.7, 9.2), t * 0.7, visc),
        fbm(p + 3.0 * q + vec2(8.3, 2.8), t * 0.7, visc)
    );
    float f = fbm(p + 2.5 * r, t * 0.4, visc);
    return f + length(q) * 0.4 + length(r) * 0.3;
}

float metaballs(vec2 p, float t) {
    float val = 0.0;
    for (int i = 0; i < 7; i++) {
        float fi = float(i);
        vec2 center = vec2(
            sin(t * 0.3 + fi * 2.1) * 0.6 + cos(t * 0.2 + fi * 1.3) * 0.3,
            cos(t * 0.25 + fi * 1.7) * 0.6 + sin(t * 0.15 + fi * 2.5) * 0.3
        );
        float radius = 0.15 + 0.1 * sin(t * 0.4 + fi * 3.0);
        float d = length(p - center);
        val += radius / (d + 0.05);
    }
    return val;
}

vec3 getNormal(vec2 p, float t, float visc, float warpCenter) {
    float eps = 0.005;
    float hC = warpCenter + metaballs(p, t) * 0.08;
    float hR = warpedField(p + vec2(eps, 0.0), t, visc) + metaballs(p + vec2(eps, 0.0), t) * 0.08;
    float hU = warpedField(p + vec2(0.0, eps), t, visc) + metaballs(p + vec2(0.0, eps), t) * 0.08;
    vec3 n = normalize(vec3(
        (hC - hR) / eps,
        (hC - hU) / eps,
        1.0
    ));
    return n;
}

float fresnel(float cosTheta, float f0) {
    return f0 + (1.0 - f0) * pow(1.0 - cosTheta, 5.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE * 0.5) / min(RENDERSIZE.x, RENDERSIZE.y);
    vec2 screenUv = gl_FragCoord.xy / RENDERSIZE;

    // Audio reactivity: flow speed modulated by bass
    float t = TIME * flowSpeed * (1.0 + audioBass * 0.6);
    float visc = viscosity;

    // Mouse influence on light direction
    vec3 lightDir1 = normalize(vec3(0.4, 0.5, 0.9));
    if (mousePos.x > 0.0) {
        vec2 mUV = mousePos / RENDERSIZE;
        lightDir1 = normalize(vec3(mUV.x - 0.5, mUV.y - 0.5, 0.9));
    }

    float field = warpedField(uv * 2.0, t, visc);
    float meta = metaballs(uv, t);
    float height = field + meta * 0.08;
    vec3 normal = getNormal(uv * 2.0, t, visc, field);

    vec3 viewDir = normalize(vec3(0.0, 0.0, 1.0));
    vec3 lightDir2 = normalize(vec3(-0.6, -0.3, 0.7));
    vec3 lightDir3 = normalize(vec3(0.0, 0.8, 0.5));

    // ShaderClaw accent: subtle red-gold tint
    vec3 goldBase = vec3(0.83, 0.61, 0.22);
    vec3 goldBright = vec3(1.0, 0.84, 0.45);
    vec3 goldDeep = vec3(0.55, 0.35, 0.08);
    vec3 goldShadow = vec3(0.18, 0.10, 0.02);
    vec3 whiteHot = vec3(1.0, 0.97, 0.88);

    float f0 = 0.8;

    float NdotL1 = max(dot(normal, lightDir1), 0.0);
    float NdotL2 = max(dot(normal, lightDir2), 0.0);
    float NdotL3 = max(dot(normal, lightDir3), 0.0);

    vec3 halfVec1 = normalize(lightDir1 + viewDir);
    vec3 halfVec2 = normalize(lightDir2 + viewDir);
    vec3 halfVec3 = normalize(lightDir3 + viewDir);

    float spec1 = pow(max(dot(normal, halfVec1), 0.0), 120.0);
    float spec2 = pow(max(dot(normal, halfVec2), 0.0), 80.0);
    float spec3 = pow(max(dot(normal, halfVec3), 0.0), 200.0);

    float NdotV = max(dot(normal, viewDir), 0.0);
    float fres = fresnel(NdotV, f0);

    float fieldNorm = smoothstep(0.3, 1.8, field);
    vec3 baseColor = mix(goldShadow, goldDeep, smoothstep(0.0, 0.3, fieldNorm));
    baseColor = mix(baseColor, goldBase, smoothstep(0.3, 0.6, fieldNorm));
    baseColor = mix(baseColor, goldBright, smoothstep(0.6, 0.9, fieldNorm));

    // Audio reactivity: specular highlights pulse with audioLevel
    float audioSpec = 1.0 + audioLevel * 1.0;

    vec3 diffuse = baseColor * (NdotL1 * 0.5 + NdotL2 * 0.3 + NdotL3 * 0.2);

    vec3 specColor1 = mix(goldBright, whiteHot, spec1);
    vec3 specColor2 = mix(goldBright, whiteHot, spec2 * 0.5);
    vec3 specColor3 = mix(goldBright, whiteHot, spec3);

    vec3 specular = specColor1 * spec1 * 1.2 * audioSpec
                  + specColor2 * spec2 * 0.6
                  + specColor3 * spec3 * 1.5 * audioSpec;

    vec2 reflUv = normal.xy * 0.5 + 0.5;
    vec3 envRefl = mix(
        vec3(0.12, 0.07, 0.02),
        vec3(0.45, 0.30, 0.12),
        reflUv.y
    );
    envRefl = mix(envRefl, vec3(0.7, 0.55, 0.25), smoothstep(0.6, 1.0, reflUv.y));

    vec3 col = diffuse * 0.4 + specular * fres + envRefl * fres * 0.5;
    col += baseColor * 0.12;

    float metaGrad = abs(meta - 3.5);
    float tensionLine = smoothstep(0.5, 0.0, metaGrad) * 0.3;
    col += goldBright * tensionLine;

    float ripple = noise(uv * 15.0 + t * 2.0);
    ripple = ripple * ripple;
    float rippleHighlight = smoothstep(0.6, 0.9, ripple) * 0.08;
    col += whiteHot * rippleHighlight * fres;

    float dist = length(uv);
    float vignette = 1.0 - smoothstep(0.3, 1.2, dist);
    col *= 0.35 + vignette * 0.65;

    float poolGlow = smoothstep(0.8, 0.0, dist) * 0.15;
    col += goldBright * poolGlow;

    // ACES tone mapping
    col = col * (2.51 * col + 0.03) / (col * (2.43 * col + 0.59) + 0.14);
    col = pow(col, vec3(0.95, 1.0, 1.08));

    gl_FragColor = vec4(col, 1.0);
}
