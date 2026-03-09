/*{
    "DESCRIPTION": "Data-driven particle flock — particle count, spread, speed, and color driven by external data signals. Connect CSV time series to sculpt the swarm.",
    "CATEGORIES": ["Generator", "Data"],
    "INPUTS": [
        { "NAME": "population",  "LABEL": "Population",   "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "energy",      "LABEL": "Energy",       "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "spread",      "LABEL": "Spread",       "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "warmth",      "LABEL": "Warmth",       "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "progress",    "LABEL": "Progress",     "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "maxBirds",    "LABEL": "Max Birds",    "TYPE": "float", "DEFAULT": 200.0, "MIN": 20.0, "MAX": 500.0 },
        { "NAME": "trailLen",    "LABEL": "Trail Length",  "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0 },
        { "NAME": "camHeight",   "LABEL": "Camera Height", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.0, "MAX": 5.0 },
        { "NAME": "camDist",     "LABEL": "Camera Dist",   "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 15.0 },
        { "NAME": "texture",     "LABEL": "Texture",       "TYPE": "image" },
        { "NAME": "coldColor",   "LABEL": "Cold Color",    "TYPE": "color", "DEFAULT": [0.15, 0.3, 0.7, 1.0] },
        { "NAME": "hotColor",    "LABEL": "Hot Color",     "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0] },
        { "NAME": "bgColor",     "LABEL": "Background",    "TYPE": "color", "DEFAULT": [0.01, 0.01, 0.03, 1.0] }
    ]
}*/

// Hash
float hash(float p) { return fract(sin(p * 127.1) * 43758.5453); }
vec3 hash3(float p) { return fract(sin(vec3(p, p+1.0, p+2.0) * vec3(127.1, 269.5, 419.2)) * 43758.5453); }
float hash2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// Smooth noise
float noise(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float n = dot(i, vec3(1, 57, 113));
    return mix(mix(mix(hash(n), hash(n+1.0), f.x),
                   mix(hash(n+57.0), hash(n+58.0), f.x), f.y),
               mix(mix(hash(n+113.0), hash(n+114.0), f.x),
                   mix(hash(n+170.0), hash(n+171.0), f.x), f.y), f.z);
}

// Bird position: flocking with noise-driven turbulence
vec3 birdPos(float id, float t, float pop, float eng, float spr) {
    vec3 seed = hash3(id * 17.31);

    // Base orbit (flocking center attraction)
    float phase = seed.x * 6.2831 + t * (0.2 + eng * 0.8) * (0.5 + seed.y);
    float radius = spr * 3.0 * (0.3 + seed.z * 0.7);
    float height = (seed.y - 0.5) * spr * 2.0;

    vec3 pos = vec3(
        cos(phase) * radius,
        height + sin(t * 0.5 + id * 0.1) * spr * 0.5,
        sin(phase) * radius
    );

    // Turbulence — more energy = more chaotic
    float turb = eng * 1.5;
    pos.x += noise(vec3(id * 0.1, t * 0.3, 0.0)) * turb;
    pos.y += noise(vec3(0.0, id * 0.1, t * 0.3)) * turb;
    pos.z += noise(vec3(t * 0.3, 0.0, id * 0.1)) * turb;

    // Cohesion: pull toward center proportional to distance
    pos *= 0.85 + 0.15 * (1.0 - eng);

    return pos;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 ndc = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME;

    // Camera
    float ca = t * 0.1;
    vec3 ro = vec3(cos(ca) * camDist, camHeight, sin(ca) * camDist);
    vec3 target = vec3(0.0, 0.5, 0.0);
    vec3 fwd = normalize(target - ro);
    vec3 right = normalize(cross(fwd, vec3(0, 1, 0)));
    vec3 up = cross(right, fwd);
    vec3 rd = normalize(fwd * 2.0 + right * ndc.x + up * ndc.y);

    // Data-driven parameters
    int numBirds = int(population * maxBirds);
    float eng = energy;
    float spr = 0.5 + spread * 3.0;

    // Check if texture is bound (sample center — if all zero, no texture)
    bool hasTex = (IMG_SIZE_texture.x > 0.0);

    vec3 col = bgColor.rgb;

    // Subtle grid floor
    float floorT = -ro.y / rd.y;
    if (floorT > 0.0 && rd.y < 0.0) {
        vec3 floorPos = ro + rd * floorT;
        vec2 grid = abs(fract(floorPos.xz * 0.5) - 0.5);
        float gridLine = smoothstep(0.02, 0.0, min(grid.x, grid.y));
        float fade = exp(-floorT * 0.15);
        col += vec3(0.03) * gridLine * fade;
    }

    // Render birds as point sprites
    for (int i = 0; i < 500; i++) {
        if (i >= numBirds) break;
        float fi = float(i);

        // Current and trailing positions
        vec3 bpos = birdPos(fi, t, population, eng, spr);

        // Project to screen
        vec3 toB = bpos - ro;
        float depth = dot(toB, fwd);
        if (depth < 0.1) continue;

        vec3 projected = ro + fwd * depth;
        float screenX = dot(toB, right) / depth * 2.0;
        float screenY = dot(toB, up) / depth * 2.0;

        vec2 screenPos = vec2(screenX, screenY);
        float dist = length(ndc - screenPos);

        // Point size varies with depth (closer = bigger)
        float pointSize = 0.015 / depth;
        pointSize *= (0.5 + population * 0.5); // more population = slightly bigger

        if (dist < pointSize * 3.0) {
            // Bird shape: bright core with soft glow
            float core = smoothstep(pointSize, pointSize * 0.3, dist);
            float glow = pointSize * 0.8 / (dist + pointSize * 0.2);

            // Color: warm/cold mix based on data + per-bird variation
            float birdWarmth = warmth + (hash(fi * 31.7) - 0.5) * 0.3;
            birdWarmth = clamp(birdWarmth, 0.0, 1.0);
            vec3 birdCol = mix(coldColor.rgb, hotColor.rgb, birdWarmth);

            // Texture: sample at particle screen position if bound
            if (hasTex) {
                vec2 texUV = vec2(screenX, screenY) * 0.25 + 0.5;
                texUV = clamp(texUV, 0.0, 1.0);
                texUV.y = 1.0 - texUV.y;
                vec3 texCol = IMG_NORM_PIXEL(texture, texUV).rgb;
                birdCol = mix(birdCol, texCol, 0.8);
            }

            // Altitude-based brightness variation
            float altFactor = 0.7 + 0.3 * (bpos.y / (spr + 0.01) * 0.5 + 0.5);
            birdCol *= altFactor;

            // Audio reactivity
            birdCol += hotColor.rgb * audioBass * 0.3;

            col += birdCol * (core * 1.5 + glow * 0.2);
        }

        // Motion trail
        if (trailLen > 0.01) {
            vec3 prevPos = birdPos(fi, t - 0.15 * trailLen, population, eng, spr);
            // Line from prevPos to bpos — closest point on segment
            vec3 seg = bpos - prevPos;
            float segLen = length(seg);
            if (segLen > 0.001) {
                vec3 segDir = seg / segLen;
                for (int s = 1; s <= 4; s++) {
                    float st = float(s) / 5.0;
                    vec3 tp = mix(prevPos, bpos, st);
                    vec3 toT = tp - ro;
                    float td = dot(toT, fwd);
                    if (td < 0.1) continue;
                    float tsx = dot(toT, right) / td * 2.0;
                    float tsy = dot(toT, up) / td * 2.0;
                    float tdist = length(ndc - vec2(tsx, tsy));
                    float tsize = 0.008 / td;
                    float tglow = tsize * 0.3 / (tdist + tsize * 0.5);
                    float trailFade = (1.0 - st) * trailLen;
                    vec3 trailCol = mix(coldColor.rgb, hotColor.rgb, warmth) * 0.3;
                    if (hasTex) {
                        vec2 ttUV = vec2(tsx, tsy) * 0.25 + 0.5;
                        ttUV = clamp(ttUV, 0.0, 1.0);
                        ttUV.y = 1.0 - ttUV.y;
                        trailCol = IMG_NORM_PIXEL(texture, ttUV).rgb * 0.3;
                    }
                    col += trailCol * tglow * trailFade;
                }
            }
        }
    }

    // Progress indicator — subtle line at bottom
    if (progress > 0.001) {
        float barY = uv.y;
        float barH = 0.003;
        if (barY < barH) {
            float filled = step(uv.x, progress);
            col += mix(coldColor.rgb, hotColor.rgb, uv.x) * filled * 0.5;
        }
    }

    // Vignette
    float vig = 1.0 - dot((uv - 0.5) * 1.2, (uv - 0.5) * 1.2);
    col *= clamp(vig, 0.0, 1.0);

    // Tone map
    col = col / (col + 0.8);

    gl_FragColor = vec4(col, 1.0);
}
