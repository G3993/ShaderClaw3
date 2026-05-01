/*{
  "CATEGORIES": ["Generator", "Art Movement", "Audio Reactive"],
  "DESCRIPTION": "De Stijl after Mondrian's Broadway Boogie Woogie (1942-43) — orthogonal lanes carrying marching coloured pulses across a white field, with bright primary squares pulsing at intersections. Manhattan grid as syncopated rhythm of yellow/red/blue/grey. No subdivision tree, no Voronoi.",
  "INPUTS": [
    { "NAME": "lanesH", "LABEL": "Horizontal Lanes", "TYPE": "float", "MIN": 2.0, "MAX": 14.0, "DEFAULT": 6.0 },
    { "NAME": "lanesV", "LABEL": "Vertical Lanes", "TYPE": "float", "MIN": 2.0, "MAX": 14.0, "DEFAULT": 6.0 },
    { "NAME": "laneWidth", "LABEL": "Lane Width", "TYPE": "float", "MIN": 0.002, "MAX": 0.040, "DEFAULT": 0.018 },
    { "NAME": "rectMotion", "LABEL": "Rectangle Motion", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45 },
    { "NAME": "rectCount", "LABEL": "Rectangles", "TYPE": "float", "MIN": 0.0, "MAX": 16.0, "DEFAULT": 8.0 },
    { "NAME": "pulseDensity", "LABEL": "Pulse Density", "TYPE": "float", "MIN": 0.5, "MAX": 12.0, "DEFAULT": 4.0 },
    { "NAME": "pulseSize", "LABEL": "Pulse Size", "TYPE": "float", "MIN": 0.005, "MAX": 0.04, "DEFAULT": 0.014 },
    { "NAME": "marchSpeed", "LABEL": "March Speed", "TYPE": "float", "MIN": 0.0, "MAX": 1.5, "DEFAULT": 0.35 },
    { "NAME": "intersectionGlow", "LABEL": "Intersection Glow", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7 },
    { "NAME": "redArea", "LABEL": "Red Probability", "TYPE": "float", "MIN": 0.0, "MAX": 0.6, "DEFAULT": 0.22 },
    { "NAME": "blueArea", "LABEL": "Blue Probability", "TYPE": "float", "MIN": 0.0, "MAX": 0.6, "DEFAULT": 0.18 },
    { "NAME": "yellowArea", "LABEL": "Yellow Probability", "TYPE": "float", "MIN": 0.0, "MAX": 0.6, "DEFAULT": 0.30 },
    { "NAME": "greyMix", "LABEL": "Grey Mix", "TYPE": "float", "MIN": 0.0, "MAX": 0.5, "DEFAULT": 0.18 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 },
    { "NAME": "compositionSeed", "LABEL": "Seed", "TYPE": "float", "MIN": 0.0, "MAX": 50.0, "DEFAULT": 0.0 },
    { "NAME": "inputTex", "LABEL": "Texture", "TYPE": "image" }
  ]
}*/

const vec3 BBW_RED    = vec3(0.89, 0.12, 0.14);
const vec3 BBW_BLUE   = vec3(0.10, 0.18, 0.65);
const vec3 BBW_YELLOW = vec3(0.97, 0.85, 0.10);
const vec3 BBW_GREY   = vec3(0.62, 0.62, 0.60);
const vec3 BBW_PAPER  = vec3(0.96, 0.94, 0.90);

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }

vec3 pickColor(float seed, float r, float b, float y, float g) {
    float h = fract(seed);
    if (h < r)            return BBW_RED;
    if (h < r + b)        return BBW_BLUE;
    if (h < r + b + y)    return BBW_YELLOW;
    if (h < r + b + y + g) return BBW_GREY;
    return vec3(0.05);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    vec3 col = BBW_PAPER;

    int NH = int(clamp(lanesH, 1.0, 14.0));
    int NV = int(clamp(lanesV, 1.0, 14.0));

    // ---- Horizontal lanes ----
    // Each lane has a hashed Y position. Marching pulses move along
    // +x with varying per-lane speed and color.
    for (int i = 0; i < 14; i++) {
        if (i >= NH) break;
        float fi = float(i) + compositionSeed * 1.31;
        float laneY = (float(i) + 0.5) / float(NH)
                    + (hash11(fi * 7.13) - 0.5) * (1.0 / float(NH)) * 0.7
                    + sin(TIME * 0.15 + fi * 2.7) * 0.012;
        float dy = abs(uv.y - laneY);
        if (dy > laneWidth + pulseSize) continue;

        // Lane line — thin grey baseline.
        float lane = smoothstep(laneWidth, 0.0, dy);
        col = mix(col, BBW_GREY * 0.6, lane * 0.35);

        // Pulses marching along this lane.
        float speed = marchSpeed * (0.5 + hash11(fi * 11.1) * 1.5)
                    * (1.0 + audioMid * audioReact * 0.6);
        float dir = (hash11(fi * 17.7) < 0.5) ? -1.0 : 1.0;
        float t = TIME * speed * dir;
        // Multiple pulses per lane.
        for (int k = 0; k < 12; k++) {
            if (float(k) >= pulseDensity) break;
            float fk = float(k);
            float spawn = hash11(fi * 23.3 + fk);
            float xpos = fract(t + spawn);
            float dx = abs(uv.x - xpos);
            // Wrap distance for cleaner edges
            dx = min(dx, 1.0 - dx);
            float pulse = step(dx, pulseSize) * step(dy, pulseSize);
            if (pulse > 0.0) {
                vec3 pc = pickColor(hash11(fi * 31.1 + fk * 5.7),
                                    redArea, blueArea, yellowArea, greyMix);
                col = pc;
            }
        }
    }

    // ---- Vertical lanes ----
    for (int j = 0; j < 14; j++) {
        if (j >= NV) break;
        float fj = float(j) + compositionSeed * 2.17;
        float laneX = (float(j) + 0.5) / float(NV)
                    + (hash11(fj * 5.71) - 0.5) * (1.0 / float(NV)) * 0.7
                    + cos(TIME * 0.13 + fj * 3.1) * 0.012;
        float dx = abs(uv.x - laneX);
        if (dx > laneWidth + pulseSize) continue;

        float lane = smoothstep(laneWidth, 0.0, dx);
        col = mix(col, BBW_GREY * 0.6, lane * 0.35);

        float speed = marchSpeed * (0.5 + hash11(fj * 13.7) * 1.5)
                    * (1.0 + audioHigh * audioReact * 0.6);
        float dir = (hash11(fj * 19.3) < 0.5) ? -1.0 : 1.0;
        float t = TIME * speed * dir;
        for (int k = 0; k < 12; k++) {
            if (float(k) >= pulseDensity) break;
            float fk = float(k);
            float spawn = hash11(fj * 29.7 + fk);
            float ypos = fract(t + spawn);
            float dy2 = abs(uv.y - ypos);
            dy2 = min(dy2, 1.0 - dy2);
            float pulse = step(dy2, pulseSize) * step(dx, pulseSize);
            if (pulse > 0.0) {
                vec3 pc = pickColor(hash11(fj * 37.3 + fk * 7.1),
                                    redArea, blueArea, yellowArea, greyMix);
                col = pc;
            }
        }
    }

    // ---- Static rectangular cell fills — the canonical Mondrian
    // ---- *Composition with Red, Blue and Yellow* device. Without
    // ---- these, shader is Boogie Woogie only; with them, it can read
    // ---- as either the late jazz grids OR the classical primary
    // ---- compositions. Cells positioned and sized at hashed offsets,
    // ---- drift slowly so they breathe without strobing.
    {
        const int CELLS = 4;
        const vec3 CELL_COLS[4] = vec3[4](
            BBW_RED, BBW_BLUE, BBW_YELLOW, BBW_GREY);
        for (int k = 0; k < CELLS; k++) {
            float fk = float(k) + compositionSeed * 5.71;
            vec2 cMin = vec2(hash11(fk * 1.7) * 0.55,
                             hash11(fk * 2.3) * 0.55);
            vec2 cSize = vec2(0.12 + hash11(fk * 3.1) * 0.18,
                              0.10 + hash11(fk * 4.7) * 0.16);
            // Slow drift so cells don't read as dead-static geometry.
            cMin += 0.008 * vec2(sin(TIME * 0.11 + fk),
                                 cos(TIME * 0.09 + fk * 1.7));
            vec2 cMax = cMin + cSize;
            if (uv.x > cMin.x && uv.x < cMax.x
             && uv.y > cMin.y && uv.y < cMax.y) {
                col = CELL_COLS[k];
            }
        }
    }

    // ---- Intersection glow ----
    // Where a horizontal and vertical lane cross, paint a small primary
    // square that pulses with the bass — Boogie Woogie's brightest beats.
    if (intersectionGlow > 0.0) {
        for (int i = 0; i < 14; i++) {
            if (i >= NH) break;
            float fi = float(i) + compositionSeed * 1.31;
            // Lane Y must include the same breath as the lane loop above
            // or the glow squares will drift off the actual lane crossings.
            float laneY = (float(i) + 0.5) / float(NH)
                        + (hash11(fi * 7.13) - 0.5) * (1.0 / float(NH)) * 0.7
                        + sin(TIME * 0.15 + fi * 2.7) * 0.012;
            for (int j = 0; j < 14; j++) {
                if (j >= NV) break;
                float fj = float(j) + compositionSeed * 2.17;
                float laneX = (float(j) + 0.5) / float(NV)
                            + (hash11(fj * 5.71) - 0.5) * (1.0 / float(NV)) * 0.7
                            + cos(TIME * 0.13 + fj * 3.1) * 0.012;
                vec2 d = uv - vec2(laneX, laneY);
                float r = length(d);
                float boxSz = pulseSize * 1.6
                            * (1.0 + audioBass * audioReact * 0.6);
                if (abs(d.x) < boxSz && abs(d.y) < boxSz) {
                    vec3 pc = pickColor(hash11(fi * 41.3 + fj * 43.1),
                                        redArea, blueArea, yellowArea, greyMix);
                    float lit = 1.0 - smoothstep(boxSz * 0.6, boxSz, max(abs(d.x), abs(d.y)));
                    col = mix(col, pc, lit * intersectionGlow);
                }
            }
        }
    }

    // Optional input bleed — quantized to primaries through a 3-colour
    // posterize, lets video drive the composition.
    if (IMG_SIZE_inputTex.x > 0.0) {
        vec3 src = texture(inputTex, uv).rgb;
        float L = dot(src, vec3(0.299, 0.587, 0.114));
        vec3 q = (L > 0.6) ? BBW_YELLOW
              : (L > 0.4) ? BBW_RED
              : (L > 0.2) ? BBW_BLUE : vec3(0.05);
        col = mix(col, q, 0.10);
    }

    // ── Animated colored rectangles drifting across the grid ──────────
    // Adds the "movement" the user wants — colored rectangles that
    // glide between grid intersections, all bordered with thick black.
    int RC = int(clamp(rectCount, 0.0, 16.0));
    for (int ri = 0; ri < 16; ri++) {
        if (ri >= RC) break;
        float fri = float(ri);
        // Each rect drifts on a unique low-frequency path
        vec2 home = vec2(hash11(fri * 7.13), hash11(fri * 11.7));
        vec2 wobble = vec2(sin(TIME * rectMotion * 0.5 + fri * 1.3),
                           cos(TIME * rectMotion * 0.4 + fri * 1.7)) * 0.04;
        vec2 ctr = home + wobble;
        // Snap centers to lane intersections so rects always sit "in" the grid
        float lH = clamp(lanesH, 2.0, 14.0);
        float lV = clamp(lanesV, 2.0, 14.0);
        ctr.x = floor(ctr.x * lV + 0.5) / lV;
        ctr.y = floor(ctr.y * lH + 0.5) / lH;
        // Size — varies, snaps to grid cells
        vec2 hs = vec2(0.06 + 0.08 * hash11(fri * 13.3),
                       0.05 + 0.07 * hash11(fri * 17.9));
        vec2 d = abs(uv - ctr);
        if (d.x < hs.x && d.y < hs.y) {
            float ridx = mod(fri, 4.0);
            vec3 rc = (ridx < 0.5) ? BBW_RED
                    : (ridx < 1.5) ? BBW_YELLOW
                    : (ridx < 2.5) ? BBW_BLUE
                                   : BBW_PAPER;
            col = rc;
        }
        // Thick black outline (Mondrian's signature heavy black borders)
        float outerX = abs(uv.x - ctr.x) - hs.x;
        float outerY = abs(uv.y - ctr.y) - hs.y;
        float edge = max(outerX, outerY);
        // Outline thickness scales with laneWidth (now defaults thicker)
        col = mix(col, vec3(0.05),
                  smoothstep(laneWidth + 0.002, laneWidth - 0.002, abs(edge)) * 0.95);
    }

    // Surprise: every ~31s a single rectangle quietly turns Mondrian-
    // forbidden green for ~0.5s.
    {
        float _ph = fract(TIME / 31.0);
        float _f  = smoothstep(0.0, 0.04, _ph) * smoothstep(0.20, 0.10, _ph);
        vec2 _suv = gl_FragCoord.xy / RENDERSIZE;
        float _h = fract(sin(floor(TIME / 31.0) * 91.7) * 43758.5453);
        vec2 _o = vec2(0.05 + _h * 0.55, 0.05 + fract(_h * 13.7) * 0.55);
        vec2 _s = vec2(0.18 + fract(_h * 7.3) * 0.20, 0.18 + fract(_h * 11.1) * 0.20);
        vec2 _q = (_suv - _o) / _s;
        float _in = step(0.0, _q.x) * step(_q.x, 1.0) * step(0.0, _q.y) * step(_q.y, 1.0);
        col = mix(col, vec3(0.10, 0.55, 0.20), _f * _in * 0.75);
    }

    gl_FragColor = vec4(col, 1.0);
}
