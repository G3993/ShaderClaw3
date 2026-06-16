/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "Futurism after Boccioni Dynamism of a Cyclist (1913) and Balla Dynamism of a Dog on a Leash (1912) — persistent frame-feedback motion-blur trails along an oscillating velocity vector, radiating force lines from a wandering origin, divisionist colour dabs streaking the trail. Speed as visual concept, all of it actually moving.",
  "INPUTS": [
    { "NAME": "trailPersistence", "LABEL": "Trail Persistence", "TYPE": "float", "MIN": 0.85, "MAX": 0.998, "DEFAULT": 0.88 },
    { "NAME": "velocityMag", "LABEL": "Velocity Magnitude", "TYPE": "float", "MIN": 0.0, "MAX": 0.10, "DEFAULT": 0.040 },
    { "NAME": "velocityRotSpeed", "LABEL": "Velocity Rotation", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.15 },
    { "NAME": "phantomCount", "LABEL": "Phantom Copies", "TYPE": "float", "MIN": 0.0, "MAX": 10.0, "DEFAULT": 5.0 },
    { "NAME": "phantomSpread", "LABEL": "Phantom Spread", "TYPE": "float", "MIN": 0.0, "MAX": 0.20, "DEFAULT": 0.07 },
    { "NAME": "forceRays", "LABEL": "Force Rays", "TYPE": "float", "MIN": 0.0, "MAX": 32.0, "DEFAULT": 16.0 },
    { "NAME": "rayBrightness", "LABEL": "Ray Brightness", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 0.85 },
    { "NAME": "rayOriginDrift", "LABEL": "Ray Origin Drift", "TYPE": "float", "MIN": 0.0, "MAX": 0.6, "DEFAULT": 0.25 },
    { "NAME": "divisionistDots", "LABEL": "Divisionist Dots", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45 },
    { "NAME": "speedHueShift", "LABEL": "Speed Hue Shift", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35 },
    { "NAME": "warmth", "LABEL": "Warmth", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "resetField", "LABEL": "Reset", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ],
  "PASSES": [
    { "TARGET": "trailBuf", "PERSISTENT": true },
    {}
  ]
}*/

// Real motion-blur via persistent frame feedback: each frame, sample the
// previous trailBuf at uv shifted opposite to velocity, fade slightly,
// then composite the new input on top. Result is a true moving streak —
// not a one-shot N-tap convolution. Force lines are computed in the
// final pass so they don't get smeared into the trail.

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}
float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

// HSV utility for speed-based colour shifts.
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 fallbackBody(vec2 uv, vec2 vel) {
    // Boccioni Dynamism of a Cyclist (1913) — discrete faceted wedge
    // shards radiating from the moving centre, NOT a smooth blob.
    // Cobalt + orange palette per the painting.
    vec2 c = vec2(0.5) + vel * sin(TIME * 0.7) * 6.0;
    vec2 d = uv - c;
    float ang = atan(d.y, d.x);
    float r   = length(d);
    // 7-bladed faceted wedge silhouette — fractured cyclist forms.
    float blade  = pow(abs(sin(ang * 3.5 + TIME * 0.4)), 0.6);
    float bladeR = 0.18 + 0.10 * blade;
    float wedge  = smoothstep(bladeR + 0.02, bladeR - 0.02, r);
    vec3 sky    = vec3(0.10, 0.14, 0.30);
    vec3 cobalt = vec3(0.08, 0.30, 0.78);
    vec3 orange = vec3(0.98, 0.45, 0.10);
    vec3 body   = mix(cobalt, orange, blade);
    return mix(sky, body, wedge);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Velocity vector — snakes through the canvas via two sine terms,
    // with magnitude breathing on its own (no audio gate). Bass + level
    // amplify on top.
    float vAng = TIME * velocityRotSpeed
               + sin(TIME * 0.13) * 0.6
               + audioBass * audioReact * 0.4;
    float vMag = velocityMag
               * (0.5 + 0.5 * sin(TIME * 0.30))
               * (1.0 + audioLevel * audioReact * 1.5);
    vec2 vel = vec2(cos(vAng), sin(vAng)) * vMag;

    // ============= PASS 0 — trail accumulation =============
    if (PASSINDEX == 0) {

        if (FRAMEINDEX < 2 || resetField) {
            // Init with the source so first-frame isn't black.
            vec3 init = (IMG_SIZE_inputTex.x > 0.0)
                      ? texture(inputTex, uv).rgb
                      : fallbackBody(uv, vel);
            gl_FragColor = vec4(init, 1.0);
            return;
        }

        // Sample previous trail at uv shifted backward along velocity —
        // the streak grows because each frame the previous content is
        // pulled along the velocity vector. Fade slightly so the trail
        // dies off after enough frames.
        vec3 prev = texture(trailBuf, uv - vel).rgb;
        prev *= trailPersistence;

        // New frame content. Phantom copies offset perpendicular to vel
        // — Balla's dog with 20 leg positions.
        vec2 vn = length(vel) > 1e-5 ? normalize(vel) : vec2(1.0, 0.0);
        vec2 vp = vec2(-vn.y, vn.x);
        // Max-blend phantoms — Boccioni Cyclist's discrete fractured
        // copies, not a soft additive average. Each phantom remains a
        // distinct overlapping plane, NOT a smoothed motion-blur.
        vec3 newC = vec3(0.0);
        int PC = int(clamp(phantomCount, 1.0, 10.0));
        for (int i = 0; i < 10; i++) {
            if (i >= PC) break;
            float fi = float(i);
            vec2 off = vp * (fi - float(PC) * 0.5)
                     * phantomSpread * 0.18;
            vec2 sUV = uv - off;
            vec3 c = (IMG_SIZE_inputTex.x > 0.0)
                   ? texture(inputTex, sUV).rgb
                   : fallbackBody(sUV, vel);
            // Per-phantom darkening so closer-to-head copies are brighter.
            float w = 1.0 - fi / float(PC) * 0.6;
            newC = max(newC, c * w);
        }

        // Composite new on top — but with low alpha so the trail dominates.
        // Effective: prev decays, new gets stamped on each frame.
        vec3 outC = max(prev, newC * (1.0 - trailPersistence) * 4.0);
        outC = mix(prev, outC, 0.6);
        gl_FragColor = vec4(outC, 1.0);
        return;
    }

    // ============= PASS 1 — output ============================================

    vec3 col = texture(trailBuf, uv).rgb;

    // Speed-driven hue shift — fast trail bleeds toward Balla's electric
    // blues; slow trail stays warm Boccioni red.
    if (speedHueShift > 0.0) {
        float L = dot(col, vec3(0.299, 0.587, 0.114));
        float speed = length(vel) * 40.0
                    + audioLevel * audioReact * 0.6;
        vec3 hot   = vec3(1.10, 0.90, 0.65) * L;
        vec3 cool  = vec3(0.70, 0.85, 1.20) * L;
        vec3 hue   = mix(hot, cool, clamp(speed, 0.0, 1.0));
        col = mix(col, hue, speedHueShift * 0.5);
    }

    // Force lines — radiating rays from a moving origin so the rays feel
    // like vectors tearing through space.
    if (forceRays > 0.0 && rayBrightness > 0.0) {
        vec2 origin = vec2(0.5, 0.5)
                    + vec2(sin(TIME * 0.83), cos(TIME * 0.59))
                      * rayOriginDrift
                    + vel * 0.5;
        vec2 d = uv - origin; d.x *= aspect;
        float th = atan(d.y, d.x);
        float rays = pow(abs(sin(th * forceRays * 0.5)), 14.0);
        rays *= smoothstep(0.7, 0.0, length(d));
        col += rays * vec3(1.0, 0.4, 0.2)
             * rayBrightness * (0.4 + audioMid * audioReact * 1.2);
    }

    // Divisionist dots streaking the trail — small bright dabs of
    // Severini-style colour particles, perpendicular to velocity vector.
    if (divisionistDots > 0.0) {
        vec2 vn = length(vel) > 1e-5 ? normalize(vel) : vec2(1.0, 0.0);
        vec2 vp = vec2(-vn.y, vn.x);
        vec2 dotG = vec2(dot(uv, vn) * 110.0,
                         dot(uv, vp) * 60.0);
        vec2 di = floor(dotG);
        float dh = hash21(di + floor(TIME * 4.0));
        if (dh > 0.92) {
            float ds = step(length(fract(dotG) - 0.5), 0.20);
            // Bright complementary palette for dabs
            float hueRoll = hash21(di * 1.3);
            vec3 dab = hsv2rgb(vec3(hueRoll, 0.85, 0.95));
            col = mix(col, dab,
                      ds * divisionistDots
                       * (0.5 + audioHigh * audioReact * 0.8));
        }
    }

    // Warm earth grade
    col = mix(col, col * vec3(1.10, 0.95, 0.80), warmth);

    gl_FragColor = vec4(col, 1.0);
}
