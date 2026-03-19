/*{
  "CATEGORIES": ["Radiant", "Space", "Noise"],
  "DESCRIPTION": "Solar eclipse with animated corona rays, chromosphere, diamond ring effect, and solar wind particles. From Radiant by Paul Bakaus (MIT).",
  "INPUTS": [
    { "NAME": "coronaSize", "LABEL": "Corona Size", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "rayIntensity", "LABEL": "Ray Intensity", "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "mousePos", "LABEL": "Mouse Position", "TYPE": "point2D", "DEFAULT": [0.0, 0.0] },
    { "NAME": "audioLevel", "LABEL": "Audio Level", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 },
    { "NAME": "audioBass", "LABEL": "Audio Bass", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0 }
  ]
}*/

// Eclipse Glow - Radiant Shaders Gallery (MIT License)

#define PI 3.14159265359
#define TAU 6.28318530718

float hash(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    return mix(
        mix(hash(i), hash(i + vec2(1.0, 0.0)), f.x),
        mix(hash(i + vec2(0.0, 1.0)), hash(i + vec2(1.0, 1.0)), f.x),
        f.y
    );
}

float fbm3(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 rot = mat2(0.8, 0.6, -0.6, 0.8);
    for (int i = 0; i < 3; i++) {
        v += a * vnoise(p);
        p = rot * p * 2.1 + vec2(1.7, 9.2);
        a *= 0.5;
    }
    return v;
}

float fbm4(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 rot = mat2(0.866, 0.5, -0.5, 0.866);
    for (int i = 0; i < 4; i++) {
        v += a * vnoise(p);
        p = rot * p * 2.05 + vec2(3.1, 7.4);
        a *= 0.48;
    }
    return v;
}

void main() {
    vec2 eclipseCenter = RENDERSIZE * 0.5;
    if (mousePos.x > 0.0) eclipseCenter = mousePos;
    vec2 uv = (gl_FragCoord.xy - eclipseCenter) / min(RENDERSIZE.x, RENDERSIZE.y);
    float t = TIME;

    // Audio reactivity: corona pulses with bass
    float audioCorona = coronaSize * (1.0 + audioBass * 0.5);
    float audioRay = rayIntensity * (1.0 + audioLevel * 0.4);

    float r = length(uv);
    float a = atan(uv.y, uv.x);

    // Background: deep space
    vec3 col = vec3(0.005, 0.003, 0.008);

    // Star field
    vec2 starGrid = floor(gl_FragCoord.xy / 3.0);
    float starHash = hash(starGrid * 0.73 + vec2(13.7, 29.3));
    float starBright = step(0.997, starHash);
    float twinkle = sin(t * 1.5 + starHash * 100.0) * 0.4 + 0.6;
    starBright *= twinkle * hash(starGrid * 1.31 + vec2(7.1, 3.9));
    starBright *= smoothstep(0.15, 0.45, r);
    col += vec3(0.7, 0.75, 0.9) * starBright * 0.6;

    float discRadius = 0.15;
    float chromoRadius = discRadius + 0.008;
    float rotAngle = a + t * 0.05;

    // Corona rays
    float ray1 = fbm3(vec2(rotAngle * 3.0, r * 4.0 - t * 0.08));
    float ray2 = fbm3(vec2(rotAngle * 7.0 + 5.0, r * 6.0 - t * 0.12));
    float ray3 = fbm4(vec2(rotAngle * 13.0 + 10.0, r * 8.0 - t * 0.18));
    float rays = ray1 * 0.5 + ray2 * 0.3 + ray3 * 0.2;

    float coronaOuter = discRadius + 0.35 * audioCorona;
    float radialFalloff = smoothstep(coronaOuter, discRadius + 0.02, r);
    radialFalloff *= smoothstep(discRadius - 0.01, discRadius + 0.03, r);

    float rayReach = discRadius + 0.6 * audioCorona;
    float rayFalloff = smoothstep(rayReach, discRadius + 0.03, r);
    rayFalloff *= smoothstep(discRadius - 0.01, discRadius + 0.03, r);

    // ShaderClaw color accent in the corona
    float colorMix = smoothstep(discRadius, coronaOuter, r);
    vec3 innerColor = vec3(1.0, 0.75, 0.30);
    vec3 outerColor = vec3(0.91, 0.25, 0.34); // ShaderClaw red
    vec3 coronaColor = mix(innerColor, outerColor, colorMix);

    float coronaGlow = radialFalloff * (0.4 + rays * 0.6);
    col += coronaColor * coronaGlow * 1.2 * audioRay;

    float rayStreak = rayFalloff * pow(rays, 1.5) * 0.8;
    col += mix(coronaColor, outerColor, 0.5) * rayStreak * audioRay;

    // Chromosphere ring
    float chromoDist = abs(r - chromoRadius);
    float chromo = exp(-chromoDist * chromoDist / 0.00008);
    float chromoNoise = fbm3(vec2(rotAngle * 10.0, t * 0.2));
    chromo *= 0.7 + chromoNoise * 0.5;
    vec3 chromoColor = vec3(1.0, 0.85, 0.5);
    col += chromoColor * chromo * 2.5 * audioRay;

    // Hot inner edge
    float innerEdge = exp(-pow((r - discRadius) * 80.0, 2.0));
    innerEdge *= smoothstep(discRadius - 0.02, discRadius + 0.005, r);
    col += vec3(1.0, 0.95, 0.8) * innerEdge * 3.0;

    // Solar wind particles
    float particleAngle = a + t * 0.03;
    float particleR = fract(r * 5.0 - t * 0.15);
    vec2 particleUV = vec2(particleAngle * 8.0, particleR * 12.0);
    float particle = hash(floor(particleUV));
    particle = step(0.96, particle);
    float particleFade = smoothstep(0.0, 0.2, particleR) * smoothstep(1.0, 0.5, particleR);
    particleFade *= smoothstep(discRadius, discRadius + 0.08, r);
    particleFade *= smoothstep(rayReach + 0.15, discRadius + 0.1, r);
    col += vec3(1.0, 0.8, 0.5) * particle * particleFade * 0.15 * audioRay;

    // Second particle layer
    float p2Angle = a - t * 0.02;
    float p2R = fract(r * 3.5 - t * 0.1);
    vec2 p2UV = vec2(p2Angle * 12.0, p2R * 8.0);
    float p2 = hash(floor(p2UV) + vec2(77.0, 33.0));
    p2 = step(0.97, p2);
    float p2Fade = smoothstep(0.0, 0.3, p2R) * smoothstep(1.0, 0.4, p2R);
    p2Fade *= smoothstep(discRadius, discRadius + 0.1, r);
    p2Fade *= smoothstep(rayReach + 0.2, discRadius + 0.12, r);
    col += vec3(0.9, 0.6, 0.3) * p2 * p2Fade * 0.1 * audioRay;

    // Bloom
    float bloomDist = max(r - discRadius, 0.0);
    float bloom = exp(-bloomDist * 2.5);
    bloom *= smoothstep(discRadius - 0.05, discRadius + 0.01, r);
    col += vec3(0.4, 0.25, 0.1) * bloom * 0.25 * audioRay;

    float wideBloom = exp(-r * 1.2) * 0.15;
    col += vec3(0.3, 0.18, 0.06) * wideBloom * audioRay;

    // Horizontal lens streak
    float streak = exp(-abs(uv.y) * 30.0) * exp(-abs(r - discRadius) * 8.0);
    streak *= smoothstep(discRadius - 0.02, discRadius + 0.05, r);
    col += vec3(0.6, 0.4, 0.2) * streak * 0.15 * audioRay;

    // Dark moon disc
    float disc = smoothstep(discRadius + 0.003, discRadius - 0.003, r);
    col *= 1.0 - disc;
    float lunarNoise = vnoise(uv * 40.0) * 0.008;
    col += vec3(lunarNoise * 0.5, lunarNoise * 0.4, lunarNoise * 0.3) * disc;

    // Diamond ring effect
    float diamondAngle = sin(t * 0.02) * PI;
    vec2 diamondDir = vec2(cos(diamondAngle), sin(diamondAngle));
    float diamondDot = dot(normalize(uv), diamondDir);
    float diamond = smoothstep(0.97, 1.0, diamondDot);
    float diamondR = smoothstep(discRadius + 0.03, discRadius, r);
    diamondR *= smoothstep(discRadius - 0.03, discRadius, r);
    float diamondGlow = diamond * diamondR;
    col += vec3(1.0, 0.9, 0.7) * diamondGlow * 1.5 * audioRay;
    float diamondBloom = diamond * exp(-abs(r - discRadius) * 15.0);
    col += vec3(0.5, 0.35, 0.15) * diamondBloom * 0.4 * audioRay;

    // Film grain
    float grain = (hash(gl_FragCoord.xy + fract(t * 43.0) * 1000.0) - 0.5) * 0.015;
    col += grain;

    // Vignette
    float vig = 1.0 - smoothstep(0.5, 1.3, r);
    col *= 0.85 + 0.15 * vig;

    // Tone mapping
    col = max(col, vec3(0.0));
    col = col / (1.0 + col * 0.3);
    float lum = dot(col, vec3(0.299, 0.587, 0.114));
    col = mix(col, col * vec3(1.06, 0.97, 0.90), smoothstep(0.05, 0.0, lum) * 0.2);

    gl_FragColor = vec4(col, 1.0);
}
