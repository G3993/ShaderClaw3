/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Eclipse — glowing solar eclipse with corona, diamond ring effect, and audio-reactive flares",
  "INPUTS": [
    { "NAME": "coronaSize", "LABEL": "Corona", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.1, "MAX": 1.5 },
    { "NAME": "ringWidth", "LABEL": "Ring", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.005, "MAX": 0.1 },
    { "NAME": "flareCount", "LABEL": "Flares", "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 16.0 },
    { "NAME": "flareLength", "LABEL": "Flare Size", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "pulse", "LABEL": "Pulse", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "baseColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
    { "NAME": "coronaColor", "LABEL": "Corona", "TYPE": "color", "DEFAULT": [1.0, 0.6, 0.2, 1.0] },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": true }
  ]
}*/

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

float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * noise(p);
        p *= 2.1;
        a *= 0.5;
    }
    return v;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);

    float dist = length(p);
    float angle = atan(p.y, p.x);

    // Audio reactivity
    float bassP = audioBass * 0.5;
    float levelP = audioLevel * 0.3;

    // Moon disc — solid black circle
    float moonR = 0.18;
    float moonMask = smoothstep(moonR + 0.003, moonR - 0.003, dist);

    // Diamond ring — bright point on the edge
    float diamondAngle = TIME * 0.2;
    float diamond = exp(-80.0 * pow(angle - diamondAngle, 2.0)) * smoothstep(moonR + 0.03, moonR, dist);
    diamond += exp(-80.0 * pow(angle - diamondAngle + 6.283, 2.0)) * smoothstep(moonR + 0.03, moonR, dist);

    // Inner corona ring — thin bright edge
    float ring = exp(-pow((dist - moonR) / max(ringWidth, 0.001), 2.0));

    // Outer corona — soft glow with noise turbulence
    float coronaDist = dist - moonR;
    float coronaFade = exp(-coronaDist * (3.0 / max(coronaSize, 0.01)));
    float turbulence = fbm(vec2(angle * 3.0 + TIME * 0.3, dist * 8.0 - TIME * 0.5));
    float corona = coronaFade * (0.5 + 0.5 * turbulence);
    corona *= smoothstep(moonR - 0.01, moonR + 0.02, dist); // hide inside moon

    // Radial flares
    float flares = 0.0;
    float fc = floor(flareCount);
    for (float i = 0.0; i < 16.0; i++) {
        if (i >= fc) break;
        float fa = i * 6.2832 / fc + TIME * 0.1 * (1.0 + pulse * sin(TIME + i));
        float angleDiff = abs(mod(angle - fa + 3.14159, 6.2832) - 3.14159);
        float flare = exp(-angleDiff * (20.0 - flareLength * 8.0));
        flare *= exp(-dist * (2.0 / max(coronaSize * (1.0 + flareLength), 0.1)));
        flare *= smoothstep(moonR - 0.01, moonR + 0.05, dist);
        flares += flare;
    }

    // Pulse animation
    float pulseMod = 1.0 + pulse * 0.15 * sin(TIME * 2.0) + bassP;

    // Compose
    vec3 col = vec3(0.0);

    // Corona color
    vec3 innerCol = vec3(1.0, 0.95, 0.9); // white-hot inner
    vec3 outerCol = coronaColor.rgb;
    vec3 coronaCol = mix(innerCol, outerCol, smoothstep(0.0, coronaSize * 0.5, coronaDist));

    col += coronaCol * corona * 1.5 * pulseMod;
    col += innerCol * ring * 2.0;
    col += baseColor.rgb * flares * 0.8 * pulseMod;
    col += vec3(1.0, 0.95, 0.85) * diamond * 3.0;

    // Audio — level adds subtle overall brightness
    col *= 1.0 + levelP;

    // Moon disc (black)
    col *= (1.0 - moonMask);

    // Texture blend
    vec2 texUV = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 texSample = texture2D(inputTex, texUV);
    col = mix(col, col * texSample.rgb, texSample.a * 0.3);

    // Tint with base color
    col = mix(col, col * baseColor.rgb, 0.3);

    col = clamp(col, 0.0, 1.0);

    if (transparentBg) {
        float a = max(col.r, max(col.g, col.b));
        gl_FragColor = vec4(col, a);
    } else {
        gl_FragColor = vec4(col, 1.0);
    }
}
