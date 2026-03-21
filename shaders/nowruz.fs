/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Nowruz — Persian new year spring equinox. Saffron & turquoise sacred geometry blooming with fire & water, voice-reactive floral emergence",
  "INPUTS": [
    { "NAME": "bloomIntensity", "LABEL": "Bloom", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "geometryFolds", "LABEL": "Geometry", "TYPE": "float", "DEFAULT": 6.0, "MIN": 3.0, "MAX": 12.0 },
    { "NAME": "fireWater", "LABEL": "Fire & Water", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "goldenHour", "LABEL": "Golden Hour", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "mirrorSymmetry", "LABEL": "Mirror", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "hazeAmount", "LABEL": "Haze", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "baseColor", "LABEL": "Color", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" },
    { "NAME": "transparentBg", "LABEL": "Transparent", "TYPE": "bool", "DEFAULT": false }
  ]
}*/

// --- Noise ---
float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash3(vec3 p) { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453); }

float noise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash(i), hash(i + vec2(1, 0)), f.x),
               mix(hash(i + vec2(0, 1)), hash(i + vec2(1, 1)), f.x), f.y);
}

float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    mat2 rot = mat2(0.8, 0.6, -0.6, 0.8);
    for (int i = 0; i < 5; i++) {
        v += a * noise(p);
        p = rot * p * 2.1;
        a *= 0.5;
    }
    return v;
}

// --- Persian geometry: polar fold + mirror ---
float persianStar(vec2 p, float folds, float mirror) {
    float angle = atan(p.y, p.x);
    float r = length(p);

    // Polar fold into N segments
    float seg = 6.2832 / folds;
    angle = mod(angle + seg * 0.5, seg) - seg * 0.5;

    // Mirror symmetry within each fold
    if (mirror > 0.5) angle = abs(angle);

    // Geometric pattern — nested rings + petal shapes
    vec2 q = vec2(cos(angle), sin(angle)) * r;
    float petal = length(q - vec2(0.15, 0.0)) - 0.08;
    float ring1 = abs(r - 0.2) - 0.005;
    float ring2 = abs(r - 0.35) - 0.003;
    float ring3 = abs(r - 0.5) - 0.002;

    // Lattice lines within the fold
    float lattice = min(abs(q.y) - 0.002, abs(q.x - r * 0.5) - 0.002);

    return min(min(petal, min(ring1, ring2)), min(ring3, lattice));
}

// --- Floral bloom emergence ---
float floralBloom(vec2 p, float t, float audio) {
    float r = length(p);
    float a = atan(p.y, p.x);

    // Petals unfurl over time, audio accelerates
    float bloomT = t * 0.15 + audio * 2.0;
    float petalCount = 8.0;
    float petalAngle = mod(a + bloomT * 0.2, 6.2832 / petalCount) - 3.14159 / petalCount;

    // Petal shape: grows outward with time
    float petalR = 0.1 + 0.25 * smoothstep(0.0, 1.0, sin(bloomT) * 0.5 + 0.5);
    float petal = length(vec2(petalAngle * r * petalCount, r - petalR * 0.6)) - petalR * 0.3;

    // Inner seed circle
    float seed = r - 0.06 - audio * 0.02;

    return min(petal, seed);
}

// --- Fire & water element ---
vec3 fireWaterField(vec2 p, float t, float mix_fw) {
    // Fire: rising warm turbulence
    vec2 fireP = p + vec2(0.0, -t * 0.3);
    float fire = fbm(fireP * 3.0 + t * 0.5);
    fire = smoothstep(0.3, 0.7, fire);
    vec3 fireCol = mix(vec3(0.9, 0.3, 0.05), vec3(1.0, 0.85, 0.2), fire);

    // Water: flowing cool ripples
    float water = sin(p.x * 8.0 + t * 1.5 + fbm(p * 4.0) * 3.0) *
                  sin(p.y * 6.0 - t * 1.2 + fbm(p.yx * 3.5) * 2.0);
    water = smoothstep(-0.2, 0.5, water);
    vec3 waterCol = mix(vec3(0.0, 0.35, 0.45), vec3(0.1, 0.7, 0.75), water);

    return mix(fireCol, waterCol, mix_fw);
}

// --- Saffron / turquoise gradient ---
vec3 saffronTurquoise(float t, float golden) {
    // Saffron: deep orange-gold
    vec3 saffron = vec3(0.95, 0.6, 0.1);
    // Turquoise: persian blue-green
    vec3 turquoise = vec3(0.1, 0.65, 0.7);
    // Gold: warm metallic
    vec3 gold = vec3(1.0, 0.84, 0.3);

    vec3 base = mix(turquoise, saffron, t);
    return mix(base, gold, golden * 0.4);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = (uv - 0.5) * vec2(aspect, 1.0);

    float t = TIME;

    // Audio: gentle response
    float bass = smoothstep(0.0, 0.3, audioBass);
    float mid = smoothstep(0.0, 0.3, audioMid);
    float high = smoothstep(0.0, 0.2, audioHigh);
    float level = smoothstep(0.0, 0.2, audioLevel);

    // Voice bloom: audioLevel drives the floral emergence
    float voiceBloom = level * bloomIntensity;

    // --- Layer 1: Ceremonial haze background ---
    vec2 hazeP = p * 2.0 + vec2(t * 0.05, t * 0.03);
    float haze = fbm(hazeP) * hazeAmount;
    vec3 hazeCol = saffronTurquoise(0.5 + haze * 0.3, goldenHour) * 0.15;

    // --- Layer 2: Fire & water field ---
    vec3 fwField = fireWaterField(p, t, fireWater);
    float fwMask = fbm(p * 3.0 + t * 0.2);
    fwMask = smoothstep(0.3, 0.6, fwMask) * 0.3 * (0.5 + bass * 0.5);

    // --- Layer 3: Persian sacred geometry ---
    // Slowly rotate the geometry, audio pulses it
    float geoRot = t * 0.08 + bass * 0.1;
    mat2 rot = mat2(cos(geoRot), -sin(geoRot), sin(geoRot), cos(geoRot));
    vec2 gp = rot * p;

    float geo = persianStar(gp, geometryFolds, mirrorSymmetry);
    float geoGlow = 0.003 / (abs(geo) + 0.003);
    geoGlow *= smoothstep(0.6, 0.0, length(gp)); // fade at edges

    // Geometry color: saffron to turquoise based on radius
    float geoR = length(gp);
    vec3 geoCol = saffronTurquoise(geoR * 1.5, goldenHour);
    geoCol += vec3(1.0, 0.9, 0.7) * high * 0.3; // high freq sparkle

    // --- Layer 4: Floral bloom (voice-reactive) ---
    float bloom = floralBloom(gp * (1.5 + voiceBloom * 0.5), t, voiceBloom);
    float bloomGlow = 0.004 / (abs(bloom) + 0.004);
    bloomGlow *= smoothstep(0.0, 1.5, voiceBloom + 0.3); // fades in with voice
    bloomGlow *= smoothstep(0.5, 0.0, length(gp)); // centered

    vec3 bloomCol = mix(
        vec3(0.95, 0.4, 0.5),   // rose
        vec3(1.0, 0.85, 0.3),   // gold
        sin(t * 0.5) * 0.5 + 0.5
    );
    bloomCol += baseColor.rgb * 0.3;

    // --- Layer 5: Golden hour light rays ---
    float rays = 0.0;
    float rayAngle = atan(p.y, p.x);
    for (float i = 0.0; i < 8.0; i++) {
        float ra = i * 0.7854 + t * 0.03;
        float diff = abs(mod(rayAngle - ra + 3.14159, 6.2832) - 3.14159);
        rays += exp(-diff * 12.0) * exp(-length(p) * 2.0) * goldenHour;
    }
    vec3 rayCol = vec3(1.0, 0.82, 0.35) * rays * 0.25;

    // --- Compose ---
    vec3 col = hazeCol;
    col += fwField * fwMask;
    col += geoCol * geoGlow * 0.6;
    col += bloomCol * bloomGlow * 0.8;
    col += rayCol;

    // Subtle ember particles
    float ember = hash(floor(p * 30.0 + t * 2.0));
    ember = smoothstep(0.97, 1.0, ember) * smoothstep(0.5, 0.0, length(p)) * bass;
    col += vec3(1.0, 0.6, 0.2) * ember * 2.0;

    // Liquid gold rim on the geometry
    float goldRim = smoothstep(0.008, 0.002, abs(geo)) * smoothstep(0.5, 0.1, geoR);
    col += vec3(1.0, 0.84, 0.3) * goldRim * 0.5 * (0.7 + mid * 0.3);

    // --- Texture masking: geometry reveals the texture ---
    vec4 texSample = texture2D(inputTex, uv);
    if (texSample.a > 0.01) {
        // Texture visible through the geometric pattern + bloom
        float mask = geoGlow * 0.5 + bloomGlow * 0.5 + goldRim;
        mask = smoothstep(0.1, 0.8, mask);
        vec3 texCol = texSample.rgb * saffronTurquoise(geoR, goldenHour * 0.5);
        col = mix(col, texCol, mask * 0.7);
    }

    // Base color tint
    col = mix(col, col * baseColor.rgb, 0.25);

    // Vignette — ceremonial frame
    float vig = 1.0 - dot(uv - 0.5, uv - 0.5) * 1.5;
    col *= smoothstep(0.0, 0.5, vig);

    // Tone mapping
    col = col / (col + 0.8);
    col = pow(col, vec3(0.9));

    col = clamp(col, 0.0, 1.0);

    if (transparentBg) {
        float a = max(col.r, max(col.g, col.b));
        a = smoothstep(0.02, 0.1, a);
        gl_FragColor = vec4(col, a);
    } else {
        gl_FragColor = vec4(col, 1.0);
    }
}
