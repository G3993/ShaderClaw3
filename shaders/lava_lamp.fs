/*{
    "DESCRIPTION": "Lava lamp — organic metaball wax blobs rising and merging in viscous fluid",
    "CREDIT": "ShaderClaw",
    "CATEGORIES": ["Generator"],
    "INPUTS": [
        { "NAME": "blobCount", "LABEL": "Blobs", "TYPE": "float", "DEFAULT": 7.0, "MIN": 3.0, "MAX": 12.0 },
        { "NAME": "blobSize", "LABEL": "Blob Size", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.1, "MAX": 0.8 },
        { "NAME": "flowSpeed", "LABEL": "Flow Speed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0 },
        { "NAME": "glossiness", "LABEL": "Gloss", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "waxColor", "LABEL": "Wax Color", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
        { "NAME": "liquidColor", "LABEL": "Liquid Color", "TYPE": "color", "DEFAULT": [0.02, 0.01, 0.06, 1.0] },
        { "NAME": "glowColor", "LABEL": "Glow Color", "TYPE": "color", "DEFAULT": [1.0, 0.5, 0.2, 1.0] }
    ]
}*/

// =============================================
// Lava Lamp — metaball potential field approach
// Organic stretchy blobs that merge and split
// =============================================

float hash(float n) { return fract(sin(n) * 43758.5453); }
vec2 hash2(float n) { return vec2(hash(n), hash(n + 7.13)); }

// Blob position — lava lamp physics
vec2 blobPos(float id, float t) {
    vec2 seed = hash2(id * 31.7);
    float period = 8.0 + seed.x * 6.0;
    float phase = seed.y * 6.28 + id * 1.7;
    float cycle = mod(t / period + seed.y, 1.0);

    // Y: slow rise with pause at top and bottom
    float y = smoothstep(0.0, 0.5, cycle) * (1.0 - smoothstep(0.5, 1.0, cycle));
    y = mix(0.1, 0.9, y);
    y += sin(t * 0.5 + phase) * 0.05;

    // X: sinusoidal drift, wider near top
    float spread = 0.15 + 0.1 * y;
    float x = 0.5 + sin(t * 0.3 + phase) * spread;
    x += cos(t * 0.17 + id * 2.3) * 0.08;

    return vec2(x, y);
}

// Blob velocity (for stretching along motion)
vec2 blobVel(float id, float t) {
    float dt = 0.02;
    return (blobPos(id, t + dt) - blobPos(id, t - dt)) / (2.0 * dt);
}

// Metaball potential for a single blob with organic deformation
// Returns potential contribution — higher means closer to blob surface
float blobPotential(vec2 p, vec2 center, float radius, float id, float t) {
    vec2 delta = p - center;

    // Stretch along velocity direction for viscous look
    vec2 vel = blobVel(id, t);
    float speed = length(vel);
    if (speed > 0.001) {
        vec2 dir = vel / speed;
        // Elongate 20-40% along movement
        float stretch = 1.0 + speed * 3.0;
        float along = dot(delta, dir);
        float perp = dot(delta, vec2(-dir.y, dir.x));
        delta = dir * along / stretch + vec2(-dir.y, dir.x) * perp * sqrt(stretch);
    }

    float d2 = dot(delta, delta);

    // Sinusoidal deformation — organic wobble like the Blob shader
    float angle = atan(delta.y, delta.x);
    float deform = 1.0
        + 0.12 * sin(angle * 3.0 + t * 1.1 + id * 2.0)
        + 0.08 * sin(angle * 5.0 - t * 0.7 + id * 4.5)
        + 0.05 * sin(angle * 7.0 + t * 1.9 + id * 1.3);

    float r2 = radius * radius * deform * deform;

    // Classic metaball: r² / d²
    // Falls off smoothly, creates organic merge zones
    return r2 / (d2 + 0.0001);
}

// Blob outer radius helper (shared by field, color, and inner core)
float blobRadius(float id, float baseSize, float t) {
    float seed = hash(id * 17.3);
    float radius = baseSize * 0.15 * (0.6 + seed * 0.8);
    radius *= 1.0 + audioBass * 0.4 + audioLevel * 0.2;
    radius *= 0.95 + 0.05 * sin(t * 3.0 + id * 3.0);
    return radius;
}

// Total metaball field — sum of all potentials
float metaField(vec2 p, float t, int N, float baseSize) {
    float field = 0.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);
        vec2 bpos = blobPos(fi, t);
        bpos.x *= aspect;

        float radius = blobRadius(fi, baseSize, t);
        field += blobPotential(p, bpos, radius, fi, t);
    }

    // Mouse blob
    if (mouseDown > 0.5) {
        vec2 mpos = vec2(mousePos.x * aspect, mousePos.y);
        float r = baseSize * 0.12;
        float d2 = dot(p - mpos, p - mpos);
        field += (r * r) / (d2 + 0.0001);
    }

    return field;
}

// Inner core field — smaller spheres growing inside each blob
float innerCoreField(vec2 p, float t, int N, float baseSize) {
    float field = 0.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);
        vec2 bpos = blobPos(fi, t);
        bpos.x *= aspect;

        // Inner core grows and shrinks on its own cycle
        float seed = hash(fi * 41.3);
        float corePhase = t * (0.4 + seed * 0.3) + fi * 2.7;
        // Smooth grow from 0 to ~40% of parent, then shrink back
        float grow = smoothstep(0.0, 0.6, sin(corePhase) * 0.5 + 0.5);
        float coreRadius = blobRadius(fi, baseSize, t) * 0.45 * grow;

        if (coreRadius < 0.001) continue;

        // Flower shape — petal lobes radiating from center
        vec2 delta = p - bpos;
        float angle = atan(delta.y, delta.x);
        float petals = 6.0 + floor(seed * 4.0); // 6-9 petals per blob
        float petalPhase = fi * 1.3 + t * 0.2;  // slow rotation
        float flower = 1.0 + 0.35 * cos(angle * petals + petalPhase);
        // Second harmonic for rounder petal tips
        flower += 0.12 * cos(angle * petals * 2.0 + petalPhase * 1.7);

        float d2 = dot(delta, delta);
        float r2 = coreRadius * coreRadius * flower * flower;
        field += r2 / (d2 + 0.0001);
    }

    return field;
}

// Gradient of metaball field (for normals/shading)
vec2 metaGradient(vec2 p, float t, int N, float baseSize) {
    float e = 0.002;
    float fx = metaField(p + vec2(e, 0.0), t, N, baseSize)
             - metaField(p - vec2(e, 0.0), t, N, baseSize);
    float fy = metaField(p + vec2(0.0, e), t, N, baseSize)
             - metaField(p - vec2(0.0, e), t, N, baseSize);
    return vec2(fx, fy) / (2.0 * e);
}

// Color ID — which blob is dominant at this point
vec3 blobColorId(vec2 p, float t, int N, float baseSize) {
    float maxPot = 0.0;
    float bestId = 0.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);
        vec2 bpos = blobPos(fi, t);
        bpos.x *= aspect;

        float radius = blobRadius(fi, baseSize, t);

        float pot = blobPotential(p, bpos, radius, fi, t);
        if (pot > maxPot) {
            maxPot = pot;
            bestId = fi;
        }
    }
    return vec3(hash(bestId * 5.1), hash(bestId * 11.3), hash(bestId * 23.7));
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 p = vec2(uv.x * aspect, uv.y);
    float t = TIME * flowSpeed;
    int N = int(blobCount);

    // Metaball field
    float field = metaField(p, t, N, blobSize);

    // Threshold — the magic number that defines the blob surface
    // Higher = smaller blobs, lower = more merged
    float threshold = 1.0;

    // Smooth wax edge
    float isWax = smoothstep(threshold - 0.15, threshold + 0.05, field);

    // Glow around wax — soft falloff below threshold
    float glow = smoothstep(threshold * 0.3, threshold, field) * (1.0 - isWax);

    // Normals from field gradient for 3D-style shading
    vec2 grad = metaGradient(p, t, N, blobSize);
    vec2 normal2d = normalize(grad + 0.0001);

    // Fake 3D shading
    vec2 lightDir = normalize(vec2(0.3, 0.7));
    float diff = max(dot(normal2d, lightDir), 0.0);
    diff = 0.5 + 0.5 * diff; // lift shadows

    // Specular highlight
    float spec = pow(max(dot(normal2d, lightDir), 0.0), 8.0 + glossiness * 60.0);
    float highlight = spec * glossiness * isWax;

    // Moving highlight for glossy liquid feel
    float movingSpec = pow(max(0.0, sin(p.x * 12.0 + p.y * 8.0 + TIME * 0.3) * 0.5 + 0.5), 25.0);
    highlight += movingSpec * glossiness * 0.25 * isWax;

    // Subsurface scattering fake — light bleeds through thin wax regions
    float sss = smoothstep(threshold - 0.1, threshold + 0.5, field) * (1.0 - smoothstep(threshold + 0.5, threshold + 2.0, field));
    sss *= 0.3;

    // Color per blob
    vec3 blobId = blobColorId(p, t, N, blobSize);

    // Wax color with per-blob variation
    vec3 wax = waxColor.rgb;
    wax = mix(wax, wax * (0.8 + 0.4 * blobId), 0.25);
    wax += vec3(audioBass * 0.08, -audioMid * 0.04, audioHigh * 0.06);
    wax *= diff;

    // Liquid background with depth gradient
    vec3 liquid = liquidColor.rgb;
    liquid += vec3(0.01, 0.005, 0.02) * uv.y;

    // Inner core — smaller bright sphere growing inside each blob
    float coreField = innerCoreField(p, t, N, blobSize);
    float coreThreshold = 1.0;
    float isCore = smoothstep(coreThreshold - 0.2, coreThreshold + 0.1, coreField);
    // Only show core inside wax
    isCore *= isWax;
    // Core is brighter, slightly lighter version of wax
    vec3 coreCol = wax * 1.4 + glowColor.rgb * 0.15;

    // Combine
    vec3 col = mix(liquid, wax, isWax);
    // Layer the inner core on top — brighter sphere within the blob
    col = mix(col, coreCol, isCore * 0.7);

    // Glossy highlights
    col += highlight * vec3(1.0, 0.95, 0.9);
    // Extra highlight on inner core surface
    col += isCore * glossiness * 0.15 * vec3(1.0, 0.95, 0.9);

    // Subsurface scattering tint
    col += sss * glowColor.rgb * 0.5 * isWax;

    // Glow around edges
    col += glow * glowColor.rgb * (0.25 + audioBass * 0.3);

    // Audio brightness pulse
    col *= 1.0 + audioLevel * 0.12;

    // Vignette — lava lamp glass tube
    float vig = 1.0 - pow(abs(uv.x - 0.5) * 1.6, 4.0);
    vig *= 1.0 - pow(abs(uv.y - 0.5) * 1.3, 6.0);
    col *= 0.7 + 0.3 * vig;

    gl_FragColor = vec4(col, 1.0);
}
