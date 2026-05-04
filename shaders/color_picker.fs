/*{
  "DESCRIPTION": "3D Itten Color Wheel — 12 hue spheres after Johannes Itten's Bauhaus color circle (1921). Studio 3-point lighting. Pick a hue with the color input to highlight it in the ring.",
  "CREDIT": "ShaderClaw",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Color"],
  "INPUTS": [
    {
      "NAME": "color",
      "LABEL": "Selected Hue",
      "TYPE": "color",
      "DEFAULT": [1.0, 0.0, 0.0, 1.0]
    },
    {
      "NAME": "intensity",
      "LABEL": "Palette Intensity",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "mixAmount",
      "LABEL": "Highlight Strength",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "audioReact",
      "LABEL": "Audio Drive",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 1.0
    }
  ]
}*/

// ── color helpers ─────────────────────────────────────────────────────
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + 1e-10)), d / (q.x + 1e-10), q.x);
}

// ── SDF primitives ────────────────────────────────────────────────────
float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float sdTorus(vec3 p, vec2 t) {
    return length(vec2(length(p.xz) - t.x, p.y)) - t.y;
}

float sdCylinder(vec3 p, float r, float h) {
    vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(r, h);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

// ── scene SDF — returns (dist, objectID) ─────────────────────────────
// IDs: 0-11 hue spheres, 12 central ivory sphere, 13 chrome torus, 14 platform
vec2 map(vec3 p, float ringAngle, float selectedHue, float audio) {
    vec2 res = vec2(1e9, -1.0);

    // 12 hue spheres (Itten's color circle: primary / secondary / tertiary)
    for (int i = 0; i < 12; i++) {
        float fi      = float(i);
        float hue     = fi / 12.0;
        float angle   = fi / 12.0 * 6.28318 + ringAngle;
        float bob     = sin(TIME * 0.45 + fi * 0.7854) * 0.08;
        vec3  center  = vec3(cos(angle) * 1.8, bob, sin(angle) * 1.8);

        float hueDiff = abs(hue - selectedHue);
        hueDiff = min(hueDiff, 1.0 - hueDiff);           // wrap hue distance
        float hl  = exp(-hueDiff * hueDiff * 50.0) * mixAmount;
        float audioPulse = (0.5 + 0.5 * audio);          // audio as modulator, not gate
        float r   = (0.25 + hl * 0.13) * audioPulse;

        float d = sdSphere(p - center, r);
        if (d < res.x) res = vec2(d, fi);
    }

    // Central ivory sphere (Itten's neutral hub)
    float cBob = sin(TIME * 0.28) * 0.04;
    float dc = sdSphere(p - vec3(0.0, cBob, 0.0), 0.32);
    if (dc < res.x) res = vec2(dc, 12.0);

    // Chrome torus ring (framing element)
    float dt = sdTorus(p - vec3(0.0, -0.55, 0.0), vec2(2.0, 0.045));
    if (dt < res.x) res = vec2(dt, 13.0);

    // Platform slab
    float db = sdBox(p - vec3(0.0, -0.73, 0.0), vec3(2.6, 0.095, 2.6));
    if (db < res.x) res = vec2(db, 14.0);

    return res;
}

vec3 calcNormal(vec3 p, float ra, float sh, float au) {
    const vec2 E = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p + E.xyy, ra, sh, au).x - map(p - E.xyy, ra, sh, au).x,
        map(p + E.yxy, ra, sh, au).x - map(p - E.yxy, ra, sh, au).x,
        map(p + E.yyx, ra, sh, au).x - map(p - E.yyx, ra, sh, au).x
    ));
}

// Soft shadow — 16 steps keeps it from exploding on large scenes
float softShadow(vec3 ro, vec3 rd, float mint, float maxt, float k,
                 float ra, float sh, float au) {
    float res = 1.0, t = mint;
    for (int i = 0; i < 16; i++) {
        float h = map(ro + rd * t, ra, sh, au).x;
        res = min(res, k * h / t);
        t  += clamp(h, 0.02, 0.3);
        if (t > maxt) break;
    }
    return clamp(res, 0.0, 1.0);
}

void main() {
    vec2 uv = (isf_FragNormCoord.xy * 2.0 - 1.0)
             * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

    float audio       = audioReact;
    float selectedHue = rgb2hsv(color.rgb).x;
    float ringAngle   = TIME * 0.22;

    // Camera orbits slowly + gentle vertical drift
    float camAngle = TIME * 0.07;
    float camY     = 1.55 + sin(TIME * 0.09) * 0.3;
    vec3  ro = vec3(cos(camAngle) * 5.4, camY, sin(camAngle) * 5.4);
    vec3  ta = vec3(0.0, 0.0, 0.0);
    vec3  fw = normalize(ta - ro);
    vec3  ri = normalize(cross(vec3(0.0, 1.0, 0.0), fw));
    vec3  up = cross(fw, ri);
    vec3  rd = normalize(fw + uv.x * ri + uv.y * up);

    // Raymarch — 64 steps, early exit
    float dist  = 0.002;
    float hitID = -1.0;
    const float EPS = 0.001;
    const float FAR = 26.0;
    for (int i = 0; i < 64; i++) {
        vec2 res = map(ro + rd * dist, ringAngle, selectedHue, audio);
        if (res.x < EPS) { hitID = res.y; break; }
        dist += res.x;
        if (dist > FAR) break;
    }

    vec3 bgCol = vec3(0.04, 0.04, 0.07);
    vec3 col   = bgCol;

    if (hitID >= 0.0) {
        vec3 p = ro + rd * dist;
        vec3 n = calcNormal(p, ringAngle, selectedHue, audio);

        // Studio 3-point lighting directions
        vec3 keyDir  = normalize(vec3(-1.5,  2.5, -1.2));
        vec3 fillDir = normalize(vec3( 2.0,  0.8,  0.5));
        vec3 rimDir  = normalize(vec3( 0.3,  1.2,  2.5));
        vec3 viewDir = normalize(ro - p);

        float shadow = softShadow(p + n * 0.01, keyDir, 0.02, 8.0, 10.0,
                                  ringAngle, selectedHue, audio);
        float kd  = max(dot(n, keyDir), 0.0) * shadow;
        float fd  = max(dot(n, fillDir), 0.0);
        float rim = pow(max(dot(n, rimDir), 0.0), 5.0);

        vec3  halfV   = normalize(keyDir + viewDir);
        float NdotH   = max(dot(n, halfV), 0.0);

        vec3  albedo    = vec3(0.5);
        float roughness = 0.35;
        float specStr   = 0.3;

        if (hitID < 12.0) {
            // Itten hue sphere — fully saturated palette color
            float hue     = hitID / 12.0;
            float hueDiff = abs(hue - selectedHue);
            hueDiff = min(hueDiff, 1.0 - hueDiff);
            float hl  = exp(-hueDiff * hueDiff * 50.0) * mixAmount;
            albedo    = hsv2rgb(vec3(hue, 0.86 + hl * 0.14, 0.70 + hl * 0.30)) * intensity;
            roughness = 0.25 - hl * 0.15;
            specStr   = 0.35 + hl * 0.45;

        } else if (hitID == 12.0) {
            // Central ivory sphere — Itten's neutral hub
            albedo    = vec3(0.97, 0.95, 0.90) * intensity;
            roughness = 0.15;
            specStr   = 0.7;

        } else if (hitID == 13.0) {
            // Brushed chrome torus
            albedo    = vec3(0.68, 0.68, 0.72);
            roughness = 0.08;
            specStr   = 0.95;

        } else {
            // Platform — dark marble with fwidth-AA checkerboard
            vec2 fuv = p.xz * 2.8;
            vec2 dv  = fwidth(fuv);                      // fwidth AA on SDF iso-surface
            vec2 q   = smoothstep(vec2(0.0), dv, fract(fuv))
                     - smoothstep(vec2(1.0) - dv, vec2(1.0), fract(fuv));
            float chk = q.x * q.y + (1.0 - q.x) * (1.0 - q.y);
            albedo    = mix(vec3(0.07, 0.07, 0.09), vec3(0.15, 0.15, 0.18), chk);
            roughness = 0.82;
            specStr   = 0.04;
        }

        float spec = pow(NdotH, mix(18.0, 320.0, 1.0 - roughness)) * specStr;

        // 3-point studio light sums
        vec3 light =
            albedo * vec3(1.0, 0.94, 0.82) * 2.2 * kd   +   // warm key
            albedo * vec3(0.58, 0.72, 1.0) * 0.55 * fd  +   // cool fill
            albedo * vec3(1.0, 1.0, 1.0)  * 0.65 * rim  +   // neutral rim
            albedo * vec3(0.04, 0.04, 0.07);                 // ambient

        light += vec3(1.0, 0.97, 0.90) * spec;

        // Depth fog — mix toward bg over distance
        float fogAmt = 1.0 - exp(-dist * 0.024);
        col = mix(light, bgCol, fogAmt);
    }

    // Output LINEAR HDR — ACES applied host-side
    gl_FragColor = vec4(col, 1.0);
}
