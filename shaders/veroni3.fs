/*{
    "DESCRIPTION": "Glass Voronoi — shiny 3D glass panels with neon edge glow, reflections, and morphing cells",
    "CATEGORIES": ["Generator", "3D"],
    "INPUTS": [
        { "NAME": "cells", "LABEL": "Cells", "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 20.0 },
        { "NAME": "edgeGlow", "LABEL": "Edge Glow", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "neonStr", "LABEL": "Neon", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.0, "MAX": 3.0 },
        { "NAME": "edgeWidth", "LABEL": "Edge Width", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.01, "MAX": 0.2 },
        { "NAME": "roughness", "LABEL": "Roughness", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.01, "MAX": 1.0 },
        { "NAME": "reflStr", "LABEL": "Reflection", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.5 },
        { "NAME": "refractionStr", "LABEL": "Refraction", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "glassDepth", "LABEL": "Glass Depth", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "morphSpeed", "LABEL": "Morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "hueShift", "LABEL": "Hue Shift", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "brightness", "LABEL": "Brightness", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.2, "MAX": 1.5 },
        { "NAME": "zoom", "LABEL": "Zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.3, "MAX": 4.0 },
        { "NAME": "palette", "LABEL": "Palette", "TYPE": "long", "DEFAULT": 0, "VALUES": [0,1,2,3], "LABELS": ["Crystal","Ember","Arctic","Toxic"] }
    ]
}*/

// ── Hash ──
vec2 hash2(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
    return fract(sin(p) * 43758.5453);
}
float hash1(float n) {
    return fract(sin(n * 113.1) * 43758.5453);
}

// ── Voronoi ──
vec4 voronoi(vec2 p, float t) {
    vec2 ip = floor(p);
    vec2 fp = fract(p);

    float d1 = 100.0;
    float d2 = 100.0;
    float cId = 0.0;
    vec2 nearest = vec2(0.0);
    vec2 second = vec2(0.0);

    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            vec2 g = vec2(float(i), float(j));
            vec2 o = hash2(ip + g);
            o = 0.5 + 0.5 * sin(t * (0.4 + o * 0.6) + o * 6.2831);
            vec2 diff = g + o - fp;
            float d = dot(diff, diff);
            if (d < d1) {
                d2 = d1; second = nearest;
                d1 = d; nearest = diff;
                cId = dot(ip + g, vec2(7.0, 113.0));
            } else if (d < d2) {
                d2 = d; second = diff;
            }
        }
    }

    float edge = dot(0.5 * (nearest + second), normalize(second - nearest));
    return vec4(sqrt(d1), edge, cId, sqrt(d2));
}

// ── HSV to RGB ──
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// ── Palette ──
vec3 cellColor(float id, int pal, float t) {
    float hue = fract(id * 0.618033988749 + hueShift + t * 0.02);
    if (pal == 0) return hsv2rgb(vec3(hue * 0.4 + 0.45, 0.5, brightness));
    if (pal == 1) return hsv2rgb(vec3(hue * 0.15 + 0.0, 0.7, brightness));
    if (pal == 2) return hsv2rgb(vec3(hue * 0.15 + 0.52, 0.4, brightness));
    return hsv2rgb(vec3(hue * 0.2 + 0.22, 0.65, brightness));
}

// ── Fake environment reflection (procedural cubemap) ──
vec3 envReflection(vec3 reflDir, float rough) {
    // Procedural gradient sky — gives believable reflections without a texture
    float y = reflDir.y * 0.5 + 0.5;
    vec3 sky = mix(vec3(0.02, 0.03, 0.06), vec3(0.15, 0.2, 0.35), y);
    // Bright spot (sun/light source reflection)
    vec3 sunDir = normalize(vec3(0.4, 0.8, 0.5));
    float sunDot = max(dot(reflDir, sunDir), 0.0);
    // Roughness blurs the reflection: tight highlight when smooth, broad when rough
    float sunPow = mix(256.0, 4.0, rough * rough);
    float sun = pow(sunDot, sunPow);
    sky += vec3(1.0, 0.95, 0.85) * sun * mix(3.0, 0.3, rough);
    // Secondary fill light
    vec3 fillDir = normalize(vec3(-0.6, 0.3, -0.4));
    float fillDot = max(dot(reflDir, fillDir), 0.0);
    sky += vec3(0.1, 0.12, 0.2) * pow(fillDot, mix(64.0, 2.0, rough * rough));
    return sky;
}

// ── GGX specular (Cook-Torrance) ──
float ggxDistribution(float NdotH, float rough) {
    float a = rough * rough;
    float a2 = a * a;
    float d = NdotH * NdotH * (a2 - 1.0) + 1.0;
    return a2 / (3.14159 * d * d + 0.0001);
}

float schlickFresnel(float cosTheta) {
    float f0 = 0.04; // glass IOR ~1.5
    return f0 + (1.0 - f0) * pow(1.0 - cosTheta, 5.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 rawUv = uv;
    uv *= zoom;

    // Mouse push
    vec2 mp = (mousePos - 0.5) * 2.0;
    mp.x *= RENDERSIZE.x / RENDERSIZE.y;
    vec2 toMouse = uv - mp;
    float md = dot(toMouse, toMouse) + 0.5;
    uv += toMouse * 0.15 / md;

    float t = TIME * morphSpeed;
    int pal = int(palette);

    // ── Voronoi ──
    vec2 vp = uv * cells;
    vec4 vor = voronoi(vp, t);
    float d1 = vor.x;
    float edge = vor.y;
    float cId = vor.z;
    float d2 = vor.w;

    // ── Edge gradient for normal computation ──
    float eps = 0.015;
    float edgeR = voronoi(vp + vec2(eps, 0.0), t).y;
    float edgeL = voronoi(vp - vec2(eps, 0.0), t).y;
    float edgeU = voronoi(vp + vec2(0.0, eps), t).y;
    float edgeD = voronoi(vp - vec2(0.0, eps), t).y;
    vec2 edgeGrad = vec2(edgeR - edgeL, edgeU - edgeD) / (2.0 * eps);

    // ── Refraction: distort UV through glass ──
    vec2 refractUV = vp + normalize(edgeGrad + 0.001) * refractionStr * (1.0 - smoothstep(0.0, 0.2, edge)) * 0.25;
    vec4 vorR = voronoi(refractUV, t);
    float rId = vorR.z;

    // ── Glass panel color ──
    vec3 glass = cellColor(rId, pal, TIME);

    // Beer-Lambert absorption
    float thickness = smoothstep(0.0, 0.4, d1);
    vec3 absorbed = glass * (1.0 - exp(-glassDepth * (1.0 - thickness * 0.5) * 3.0));

    // Internal gradient per panel
    float gradAngle = hash1(cId * 2.71828) * 6.2831;
    vec2 gradDir = vec2(cos(gradAngle), sin(gradAngle));
    float panelGrad = dot(normalize(uv - floor(uv * cells) / cells + 0.001), gradDir) * 0.5 + 0.5;
    absorbed *= 0.65 + 0.35 * panelGrad;

    // ── 3D Surface Normal ──
    // Per-cell tilt + edge-driven curvature (glass bulges slightly)
    float tiltX = sin(cId * 3.7 + t * 0.3) * 0.2;
    float tiltY = cos(cId * 5.1 + t * 0.2) * 0.2;
    // Edge curvature — glass surface curves down near edges
    float curvature = (1.0 - smoothstep(0.0, 0.25, edge)) * glassDepth * 0.6;
    vec3 N = normalize(vec3(
        tiltX + edgeGrad.x * curvature,
        tiltY + edgeGrad.y * curvature,
        1.0
    ));

    // ── PBR-ish Lighting ──
    vec3 lightDir = normalize(vec3(0.3, 0.7, 0.9));
    vec3 V = vec3(0.0, 0.0, 1.0);
    vec3 H = normalize(lightDir + V);
    vec3 R = reflect(-V, N);

    float NdotL = max(dot(N, lightDir), 0.0);
    float NdotH = max(dot(N, H), 0.0);
    float NdotV = max(dot(N, V), 0.0);
    float VdotH = max(dot(V, H), 0.0);

    // GGX specular
    float D = ggxDistribution(NdotH, roughness);
    float F = schlickFresnel(VdotH);

    // Specular: sharp highlight on smooth glass, broad on rough
    float specular = D * F / (4.0 * NdotV * NdotL + 0.001);
    specular = clamp(specular, 0.0, 10.0);

    // Fresnel for reflections — stronger at glancing angles
    float fresnelRefl = schlickFresnel(NdotV);
    // Roughness dims fresnel reflections
    fresnelRefl = mix(fresnelRefl, fresnelRefl * 0.3, roughness);

    // Environment reflection
    vec3 envColor = envReflection(R, roughness);

    // Second light for fill (opposite side)
    vec3 light2Dir = normalize(vec3(-0.5, 0.4, 0.7));
    vec3 H2 = normalize(light2Dir + V);
    float NdotH2 = max(dot(N, H2), 0.0);
    float spec2 = ggxDistribution(NdotH2, roughness) * schlickFresnel(max(dot(V, H2), 0.0));
    spec2 = clamp(spec2, 0.0, 5.0) * 0.3;

    // ── Compose glass ──
    // Diffuse transmission (light through glass)
    vec3 litGlass = absorbed * (0.2 + 0.6 * NdotL);
    // Add specular highlights (key + fill)
    litGlass += vec3(1.0, 0.98, 0.94) * specular * 0.8;
    litGlass += vec3(0.9, 0.92, 1.0) * spec2;
    // Environment reflection blended by fresnel
    litGlass = mix(litGlass, envColor * reflStr, fresnelRefl * reflStr);
    // Rim/fresnel adds glass color at edges
    litGlass += glass * fresnelRefl * 0.2;

    // Backlight
    float backlight = 0.75 + 0.25 * (1.0 - length(rawUv) * 0.5);
    litGlass *= backlight;

    // ── Neon Edge Glow ──
    float edgeLine = 1.0 - smoothstep(0.0, edgeWidth, edge);

    // Soft glow bleeding out from edges (inverse distance falloff)
    float glowSoft = edgeGlow * 0.015 / (edge + 0.003);
    glowSoft = min(glowSoft, 5.0);
    // Wider bloom halo
    float glowWide = edgeGlow * 0.004 / (edge * edge + 0.002);
    glowWide = min(glowWide, 2.0);

    // Neon color — blend neighboring cell colors, push bright
    vec3 neonCol = mix(
        cellColor(cId, pal, TIME),
        cellColor(cId + 1.0, pal, TIME),
        0.5
    );
    neonCol = mix(neonCol, vec3(1.0), 0.4); // whiten for neon
    neonCol *= 1.0 + neonStr * 0.5;

    // Audio pulse on glow
    float audioPulse = 1.0 + audioBass * 0.7 + audioLevel * 0.3;
    glowSoft *= audioPulse;
    glowWide *= audioPulse;

    // Hard bright core on the edge line
    vec3 neonCore = neonCol * edgeLine * neonStr * 2.0;
    // Soft glow around edge
    vec3 neonSoft = neonCol * glowSoft * neonStr * 0.4;
    // Wide bloom halo
    vec3 neonBloom = neonCol * glowWide * neonStr * 0.2;

    // Dark came base under the neon
    vec3 cameBase = vec3(0.02, 0.025, 0.03) + neonCol * 0.05;

    // Shadow near edges on glass
    float cameShadow = smoothstep(0.0, edgeWidth * 3.0, edge);
    litGlass *= 0.45 + 0.55 * cameShadow;

    // ── Composite ──
    // Glass panels with edge darkening
    vec3 col = litGlass;

    // Layer neon on edges: hard core replaces, soft glow adds
    col = mix(col, cameBase + neonCore, edgeLine);
    col += neonSoft;
    col += neonBloom;

    // Vignette
    float vig = 1.0 - length(rawUv) * 0.2;
    col *= clamp(vig, 0.0, 1.0);

    // Audio brightness
    col *= 1.0 + audioBass * 0.12;

    // Tone mapping — prevent blowout while keeping neon punch
    col = col / (1.0 + col * 0.3);

    gl_FragColor = vec4(col, 1.0);
}
