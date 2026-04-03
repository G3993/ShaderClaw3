/*{
  "DESCRIPTION": "Sonoluminescence — glowing bubble in water with concentric ripples, glass vessel, and audio-reactive pulse",
  "CREDIT": "ShaderClaw (inspired by TheNewPhysics sonoluminescence visualization)",
  "CATEGORIES": ["Generator", "Nature"],
  "INPUTS": [
    { "NAME": "rippleCount", "LABEL": "Ripples", "TYPE": "float", "DEFAULT": 8.0, "MIN": 3.0, "MAX": 20.0 },
    { "NAME": "rippleSpeed", "LABEL": "Ripple Speed", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "glowIntensity", "LABEL": "Glow", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "waterLevel", "LABEL": "Water Level", "TYPE": "float", "DEFAULT": 0.42, "MIN": 0.2, "MAX": 0.7 },
    { "NAME": "glassWidth", "LABEL": "Glass Width", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.15, "MAX": 0.5 },
    { "NAME": "glassHeight", "LABEL": "Glass Height", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.3, "MAX": 0.9 },
    { "NAME": "pulseRate", "LABEL": "Pulse Rate", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "colorTemp", "LABEL": "Color Temp", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "transparency", "LABEL": "Glass Clarity", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.4 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": 0.0 }
  ]
}*/

#define PI 3.14159265

// Smooth noise
float hash(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float vnoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash(i), hash(i + vec2(1, 0)), f.x),
               mix(hash(i + vec2(0, 1)), hash(i + vec2(1, 1)), f.x), f.y);
}

// Signed distance to a rounded rectangle (glass cross-section)
float sdRoundedRect(vec2 p, vec2 b, float r) {
    vec2 d = abs(p) - b + r;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - r;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);

    float t = TIME * rippleSpeed;

    // Audio reactivity
    float bass = audioBass;
    float pulse = 0.5 + 0.5 * sin(TIME * pulseRate * PI * 2.0);
    float audioPulse = mix(pulse, bass, step(0.01, bass));

    // Color palette: deep blue to cyan based on colorTemp
    vec3 waterDeep = mix(vec3(0.0, 0.02, 0.08), vec3(0.0, 0.04, 0.12), colorTemp);
    vec3 waterBright = mix(vec3(0.05, 0.2, 0.6), vec3(0.1, 0.4, 0.8), colorTemp);
    vec3 glowColor = mix(vec3(0.3, 0.6, 1.0), vec3(0.5, 0.8, 1.0), colorTemp);
    vec3 bgColor = vec3(0.005, 0.008, 0.015);

    // ========== GLASS VESSEL ==========
    vec2 glassCenter = vec2(0.0, 0.05);
    float gw = glassWidth;
    float gh = glassHeight * 0.5;
    float wallThick = 0.012;
    float cornerR = 0.02;

    // Outer glass shell
    float dOuter = sdRoundedRect(p - glassCenter, vec2(gw, gh), cornerR);
    // Inner glass (slightly smaller)
    float dInner = sdRoundedRect(p - glassCenter, vec2(gw - wallThick, gh - wallThick * 0.5), cornerR * 0.5);

    // Glass rim at top
    float rimY = glassCenter.y + gh;
    float rimDist = abs(p.y - rimY);
    float rimMask = smoothstep(wallThick * 2.0, 0.0, rimDist) * smoothstep(gw + wallThick, gw - wallThick, abs(p.x - glassCenter.x));

    // Glass wall mask
    float glassMask = smoothstep(0.003, 0.0, dOuter) - smoothstep(0.003, 0.0, dInner);
    // Glass edge highlight (thin bright line)
    float edgeGlow = smoothstep(0.008, 0.0, abs(dOuter)) * 0.5;
    edgeGlow += smoothstep(0.005, 0.0, abs(dOuter + wallThick * 0.5)) * 0.2;

    // Inside glass mask
    float insideMask = smoothstep(0.002, 0.0, dInner);

    // ========== WATER SURFACE ==========
    float waterY = glassCenter.y - gh + gh * 2.0 * waterLevel;
    // Gentle surface wave
    float surfaceWave = 0.003 * sin(p.x * 30.0 + t * 2.0) + 0.002 * sin(p.x * 50.0 - t * 3.0);
    float waterSurf = waterY + surfaceWave;
    float inWater = insideMask * smoothstep(waterSurf + 0.005, waterSurf - 0.005, p.y);

    // Water surface highlight (meniscus)
    float surfHighlight = smoothstep(0.015, 0.0, abs(p.y - waterSurf)) * insideMask;
    // Meniscus curves up at glass walls
    float wallProx = smoothstep(gw - wallThick, gw - wallThick - 0.04, abs(p.x - glassCenter.x));
    surfHighlight *= (0.3 + 0.7 * wallProx);

    // ========== BUBBLE / GLOW SOURCE ==========
    vec2 bubblePos = vec2(glassCenter.x, waterY - 0.08);
    // Subtle oscillation
    bubblePos.y += 0.005 * sin(TIME * 1.5);
    float distBubble = length(p - bubblePos);

    // Core glow
    float coreGlow = exp(-distBubble * 80.0) * glowIntensity * (0.7 + 0.3 * audioPulse);
    // Soft halo
    float haloGlow = exp(-distBubble * 20.0) * glowIntensity * 0.4 * (0.6 + 0.4 * audioPulse);
    // Wide atmospheric glow
    float atmosGlow = exp(-distBubble * 6.0) * glowIntensity * 0.08;

    // ========== CONCENTRIC RIPPLES ==========
    float ripples = 0.0;
    // Ripples on the water surface, expanding from bubble position projected to surface
    vec2 rippleCenter = vec2(bubblePos.x, waterSurf);
    float distRipple = length(p - rippleCenter);

    for (float i = 0.0; i < 20.0; i += 1.0) {
        if (i >= rippleCount) break;
        // Each ripple ring expands outward over time
        float phase = fract(t * 0.3 + i / rippleCount);
        float radius = phase * gw * 1.2;
        float ringWidth = 0.003 + 0.002 * phase; // wider as they expand
        float ring = smoothstep(ringWidth, 0.0, abs(distRipple - radius));
        float fade = (1.0 - phase) * (1.0 - phase); // fade as they expand
        // Only show ripples near water surface
        float surfProx = exp(-abs(p.y - waterSurf) * 60.0);
        ripples += ring * fade * surfProx * (0.6 + 0.4 * audioPulse);
    }

    // ========== UNDERWATER CAUSTICS ==========
    float caustics = 0.0;
    if (inWater > 0.01) {
        float cx = vnoise(p * 15.0 + t * vec2(0.3, 0.2)) * 0.5
                 + vnoise(p * 30.0 - t * vec2(0.2, 0.4)) * 0.25;
        caustics = pow(cx, 3.0) * 0.3 * inWater;
    }

    // ========== COMPOSE ==========
    vec3 col = bgColor;

    // Water body
    float waterDepth = smoothstep(waterSurf, waterSurf - gh, p.y);
    vec3 waterCol = mix(waterDeep, waterBright * 0.3, waterDepth * 0.3);
    waterCol += caustics * waterBright;
    col = mix(col, waterCol, inWater);

    // Ripples
    col += ripples * glowColor * 0.8;

    // Water surface highlight
    col += surfHighlight * vec3(0.15, 0.3, 0.5) * 0.5;

    // Bubble glow (rendered in water)
    col += coreGlow * vec3(0.9, 0.95, 1.0) * inWater;
    col += haloGlow * glowColor * inWater;
    col += atmosGlow * waterBright;

    // Glass walls
    vec3 glassCol = vec3(0.08, 0.12, 0.18) * (0.5 + edgeGlow * 2.0);
    glassCol += vec3(0.15, 0.2, 0.3) * edgeGlow;
    col = mix(col, glassCol, glassMask * (0.3 + transparency));

    // Glass edge highlight
    col += edgeGlow * vec3(0.2, 0.25, 0.35);

    // Glass rim
    col += rimMask * vec3(0.15, 0.2, 0.3) * 0.5;

    // Subtle reflection on glass (vertical gradient)
    float reflStripe = smoothstep(0.008, 0.0, abs(abs(p.x - glassCenter.x) - gw + 0.005));
    reflStripe *= smoothstep(glassCenter.y - gh * 0.5, glassCenter.y + gh * 0.8, p.y);
    col += reflStripe * vec3(0.06, 0.08, 0.12) * 0.5;

    // Vignette
    float vign = 1.0 - 0.6 * dot(uv - 0.5, uv - 0.5) * 2.0;
    col *= vign;

    gl_FragColor = vec4(col, 1.0);
}
