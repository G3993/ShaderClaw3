/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Oscars ceremony — dramatic gold spotlight cone with volumetric god rays, floating dust particles, film grain, and audio reactivity",
  "INPUTS": [
    { "NAME": "color1", "LABEL": "Color 1", "TYPE": "color", "DEFAULT": [1.0, 0.85, 0.45, 1.0] },
    { "NAME": "color2", "LABEL": "Color 2", "TYPE": "color", "DEFAULT": [0.9, 0.3, 0.15, 1.0] },
    { "NAME": "color3", "LABEL": "Color 3", "TYPE": "color", "DEFAULT": [0.6, 0.2, 0.8, 1.0] },
    { "NAME": "colorSpeed", "LABEL": "Color Speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.3 },
    { "NAME": "colorAudio", "LABEL": "Color Audio", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.3 },
    { "NAME": "bgColor", "LABEL": "Background", "TYPE": "color", "DEFAULT": [0.02, 0.015, 0.005, 1.0] },
    { "NAME": "intensity", "LABEL": "Intensity", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "coneWidth", "LABEL": "Cone Width", "TYPE": "float", "MIN": 0.1, "MAX": 1.5, "DEFAULT": 0.55 },
    { "NAME": "rayCount", "LABEL": "Ray Count", "TYPE": "float", "MIN": 2.0, "MAX": 20.0, "DEFAULT": 8.0 },
    { "NAME": "raySharpness", "LABEL": "Ray Sharpness", "TYPE": "float", "MIN": 0.1, "MAX": 3.0, "DEFAULT": 1.2 },
    { "NAME": "drift", "LABEL": "Drift", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.43 },
    { "NAME": "speed", "LABEL": "Speed", "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.4 },
    { "NAME": "dustAmount", "LABEL": "Dust Particles", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
    { "NAME": "dustSize", "LABEL": "Dust Size", "TYPE": "float", "MIN": 0.5, "MAX": 3.0, "DEFAULT": 1.2 },
    { "NAME": "noiseAmount", "LABEL": "Film Grain", "TYPE": "float", "MIN": 0.0, "MAX": 0.15, "DEFAULT": 0.04 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.6 },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

// ═══════════════════════════════════════════════════════════════════════
// OSCAR CEREMONY — Gold volumetric spotlight with god rays & dust
// ═══════════════════════════════════════════════════════════════════════

const float PI = 3.14159265;
const float MAX_DUST = 80.0;

// Smooth 3-color cycle: blends between color1 → color2 → color3 → color1
vec3 triColorBlend(vec3 c1, vec3 c2, vec3 c3, float t) {
    t = fract(t); // 0..1 loops
    if (t < 1.0/3.0) return mix(c1, c2, t * 3.0);
    if (t < 2.0/3.0) return mix(c2, c3, (t - 1.0/3.0) * 3.0);
    return mix(c3, c1, (t - 2.0/3.0) * 3.0);
}

// Hash functions
float hash(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

float hash1(float n) {
    return fract(sin(n) * 43758.5453);
}

// Value noise
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

// FBM for organic variation
float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    vec2 shift = vec2(100.0);
    for (int i = 0; i < 4; i++) {
        v += a * noise(p);
        p = p * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}

// Film grain
float grain(vec2 uv, float t) {
    return hash(uv * RENDERSIZE.xy + fract(t * 43.13) * 1000.0) - 0.5;
}

// Dust particle — returns brightness for a single particle at UV
float dustParticle(vec2 uv, float id, float t, float aspect, float audioBoost) {
    // Deterministic position from particle ID
    float seed1 = hash1(id * 127.1);
    float seed2 = hash1(id * 311.7);
    float seed3 = hash1(id * 73.3);
    float seed4 = hash1(id * 419.2);

    // Base position — scattered across frame, biased toward cone area
    float px = 0.2 + 0.6 * seed1; // mostly center-ish
    float py = seed2;

    // Slow floating drift — each particle has its own speed and direction
    float driftSpeed = 0.02 + 0.04 * seed3;
    float driftAngle = seed4 * PI * 2.0;
    px += sin(t * driftSpeed + driftAngle) * 0.08;
    py += cos(t * driftSpeed * 0.7 + seed1 * 10.0) * 0.06;

    // Gentle upward float (dust rises in warm light)
    py += fract(t * (0.005 + 0.015 * seed2) + seed3) * 0.3 - 0.15;

    // Audio: bass pulses particles outward from center
    float audioDisplace = audioBoost * 0.03;
    px += (seed1 - 0.5) * audioDisplace;
    py += (seed2 - 0.5) * audioDisplace;

    // Wrap position
    px = fract(px);
    py = fract(py);

    // Distance from pixel to particle
    vec2 diff = uv - vec2(px, py);
    diff.x *= aspect;
    float d = length(diff);

    // Particle size with subtle audio pulse
    float size = (0.001 + 0.002 * seed3) * dustSize * (1.0 + audioBoost * 0.5);

    // Soft circle
    float brightness = smoothstep(size, size * 0.2, d);

    // Twinkle: slow pulsing brightness per particle
    brightness *= 0.3 + 0.7 * (0.5 + 0.5 * sin(t * (1.0 + seed4 * 3.0) + seed1 * 20.0));

    // Audio makes particles flicker
    brightness *= 1.0 + audioBoost * 0.4 * sin(t * 8.0 + id * 5.0);

    return brightness;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Audio reactivity
    float aLevel = audioLevel * audioReact;
    float aBass = audioBass * audioReact;
    float aHigh = audioHigh * audioReact;
    float aMid = audioMid * audioReact;

    // Light source at top center
    vec2 lightPos = vec2(0.5, 1.05);
    vec2 toLight = uv - lightPos;
    toLight.x *= aspect;
    float dist = length(toLight);
    float angle = atan(toLight.x, -toLight.y);

    // === Volumetric cone ===
    float coneAngle = abs(angle);
    float maxConeAngle = coneWidth * (0.8 + 0.4 * (1.0 - uv.y));
    float coneMask = smoothstep(maxConeAngle, maxConeAngle * 0.3, coneAngle);

    // Vertical falloff
    float vertFade = pow(max(0.0, 1.0 - dist * 0.6), 1.5);

    // Audio pumps the cone
    float coneIntensity = coneMask * vertFade * intensity * (1.0 + aBass * 0.8);

    // === God rays ===
    float t = TIME * speed;
    float driftOffset = sin(t * 0.7) * drift + cos(t * 0.43) * drift * 0.5;
    float rayAngle = angle + driftOffset;

    float rays = 0.0;
    rays += pow(max(0.0, cos(rayAngle * rayCount)), raySharpness * 2.0);
    rays += 0.5 * pow(max(0.0, cos(rayAngle * rayCount * 2.0 + t * 0.3)), raySharpness * 3.0);
    rays += 0.25 * pow(max(0.0, cos(rayAngle * rayCount * 0.5 - t * 0.2)), raySharpness);

    // Organic ray noise
    float rayNoise = fbm(vec2(angle * 3.0 + t * 0.1, dist * 2.0 - t * 0.15));
    rays *= 0.6 + 0.4 * rayNoise;

    // Audio shimmer on rays
    rays *= 1.0 + aHigh * 0.5 * sin(angle * 12.0 + TIME * 5.0);

    float rayIntensity = rays * coneMask * vertFade * 0.6 * intensity;

    // === Hot center glow ===
    float centerGlow = exp(-dist * dist * 8.0) * 2.0 * intensity;
    centerGlow *= 1.0 + aMid * 0.5;

    // === Atmospheric haze ===
    float haze = fbm(uv * 3.0 + vec2(t * 0.05, t * 0.03));
    haze *= coneMask * 0.15 * intensity;

    // === Combine lighting ===
    float totalLight = coneIntensity + rayIntensity + centerGlow + haze;

    // 3-color cycling — audio pushes the blend position
    float colorT = TIME * colorSpeed * 0.1 + aBass * colorAudio * 0.4 + aMid * colorAudio * 0.15;
    vec3 gold = triColorBlend(color1.rgb, color2.rgb, color3.rgb, colorT);
    vec3 warmGold = gold * (0.9 + 0.1 * sin(angle * 2.0 + t * 0.5));
    vec3 hotColor = mix(warmGold, vec3(1.0, 0.97, 0.9), smoothstep(0.3, 0.0, dist));

    vec3 col = bgColor.rgb + hotColor * totalLight;

    // === Floating dust particles ===
    float dustTotal = 0.0;
    float numParticles = MAX_DUST * dustAmount;
    for (int i = 0; i < 80; i++) {
        if (float(i) >= numParticles) break;
        dustTotal += dustParticle(uv, float(i), TIME, aspect, aBass);
    }
    // Dust is gold, brighter inside the cone
    float dustInCone = dustTotal * (0.3 + 0.7 * coneMask);
    col += gold * dustInCone * 0.8 * intensity;

    // === Film grain ===
    float g = grain(uv, TIME) * noiseAmount;
    col += g;

    // === Subtle noise texture layer ===
    float texNoise = fbm(uv * 8.0 + vec2(TIME * 0.02, -TIME * 0.015)) * 0.03 * intensity;
    col += vec3(texNoise) * gold;

    col = clamp(col, 0.0, 1.0);

    if (transparentBg) {
        float lum = dot(col, vec3(0.299, 0.587, 0.114));
        gl_FragColor = vec4(col, lum);
    } else {
        gl_FragColor = vec4(col, 1.0);
    }
}
