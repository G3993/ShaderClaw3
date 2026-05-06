/*{
  "DESCRIPTION": "Neon City Night Drive — first-person 3D fly-through of an infinite neon-lit city canyon. SDF box buildings, emissive neon strips, dark reflective pavement, fwidth AA grid.",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Speed",        "TYPE": "float", "MIN": 0.0,  "MAX": 4.0,  "DEFAULT": 1.2  },
    { "NAME": "fovScale",    "LABEL": "FOV",          "TYPE": "float", "MIN": 0.5,  "MAX": 2.0,  "DEFAULT": 1.0  },
    { "NAME": "streetWidth", "LABEL": "Street Width", "TYPE": "float", "MIN": 1.0,  "MAX": 5.0,  "DEFAULT": 2.5  },
    { "NAME": "hdrGlow",     "LABEL": "HDR Glow",     "TYPE": "float", "MIN": 0.5,  "MAX": 4.0,  "DEFAULT": 2.2  },
    { "NAME": "paletteShift","LABEL": "Hue Shift",    "TYPE": "float", "MIN": 0.0,  "MAX": 1.0,  "DEFAULT": 0.0  },
    { "NAME": "audioMod",    "LABEL": "Audio Mod",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0  }
  ]
}*/

const float PI = 3.14159265;

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }

vec3 hue2rgb(float h) {
    vec3 rgb = abs(fract(vec3(h, h + 0.667, h + 0.333)) * 6.0 - 3.0) - 1.0;
    return clamp(rgb, 0.0, 1.0);
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// Returns vec3(dist, mat, neonHue)
// mat: 0=ground, 1=building, 2=neon strip
vec3 sdScene(vec3 p) {
    float sw     = streetWidth;
    float blockZ = 8.0;

    float dGround = p.y;

    // Domain repeat in Z: one building slab per blockZ, centred at multiples of blockZ
    float bIdxF  = floor(p.z / blockZ + 0.5);
    float bz_loc = p.z - bIdxF * blockZ;          // local Z in [-4, 4]

    float bH  = 2.5 + hash11(bIdxF)       * 5.5;  // height 2.5–8
    float bD  = 0.7 + hash11(bIdxF + 0.3) * 0.8;  // half-depth 0.7–1.5
    float bW  = 1.2 + hash11(bIdxF + 0.7) * 0.6;  // half-width in x 1.2–1.8
    float nHue = fract(hash11(bIdxF + 1.1) + paletteShift + TIME * 0.015);

    // Left building: centred at x = -(sw + bW)
    vec3  pL  = vec3(p.x + sw + bW, p.y - bH * 0.5, bz_loc);
    float dBL = sdBox(pL, vec3(bW, bH * 0.5, bD));

    // Right building: centred at x = +(sw + bW)
    vec3  pR  = vec3(p.x - sw - bW, p.y - bH * 0.5, bz_loc);
    float dBR = sdBox(pR, vec3(bW, bH * 0.5, bD));

    float dBuild = min(dBL, dBR);

    // Neon strips: thin slab flush with inner face of building, near top
    float nY  = bH - 0.28;   // centre height of strip
    float nT  = 0.06;        // half-height
    float nXh = 0.07;        // half-thickness in x (protrudes slightly into road)

    vec3  pNL = vec3(p.x + sw - nXh, p.y - nY, bz_loc);
    float dNL = sdBox(pNL, vec3(nXh, nT, bD * 0.88));

    vec3  pNR = vec3(p.x - sw + nXh, p.y - nY, bz_loc);
    float dNR = sdBox(pNR, vec3(nXh, nT, bD * 0.88));

    float dNeon = min(dNL, dNR);

    float d   = dGround;
    float mat = 0.0;
    if (dBuild < d) { d = dBuild; mat = 1.0; }
    if (dNeon  < d) { d = dNeon;  mat = 2.0; }

    return vec3(d, mat, nHue);
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sdScene(p + e.xyy).x - sdScene(p - e.xyy).x,
        sdScene(p + e.yxy).x - sdScene(p - e.yxy).x,
        sdScene(p + e.yyx).x - sdScene(p - e.yyx).x
    ));
}

// Returns vec3(t, mat, neonHue); t < 0 = miss
vec3 march(vec3 ro, vec3 rd) {
    float t = 0.02;
    for (int i = 0; i < 80; i++) {
        vec3 res = sdScene(ro + rd * t);
        if (res.x < 0.0004) return vec3(t, res.y, res.z);
        t += res.x * 0.85;
        if (t > 55.0) break;
    }
    return vec3(-1.0, 0.0, 0.0);
}

void main() {
    vec2  uv    = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float audio = 1.0 + (audioLevel + audioBass * 0.5) * audioMod * 0.3;
    float t     = TIME * speed;

    // Camera: forward motion, subtle head-bob, gentle lateral weave
    float bob = sin(t * 1.6) * 0.05;
    vec3  ro  = vec3(sin(t * 0.12) * 0.25, 0.85 + bob, t);
    vec3  aim = vec3(sin(t * 0.07) * 0.12, 0.65, t + 1.0);
    vec3  fwd  = normalize(aim - ro);
    vec3  rght = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3  up   = cross(rght, fwd);
    vec3  rd   = normalize(fwd + uv.x * rght * fovScale + uv.y * up * fovScale);

    vec3 hit = march(ro, rd);
    vec3 col;

    if (hit.x > 0.0) {
        float dist = hit.x;
        float mat  = hit.y;
        float nHue = hit.z;
        vec3  p    = ro + rd * dist;
        vec3  n    = calcNormal(p);
        float fog  = exp(-dist * 0.038);

        vec3  L    = normalize(vec3(0.2, 1.0, 0.4));
        float diff = max(dot(n, L), 0.0);
        vec3  R    = reflect(-L, n);
        float spec = pow(max(dot(R, -rd), 0.0), 28.0);

        if (mat < 0.5) {
            // Ground — dark pavement with fwidth-AA grid lines and neon shimmer
            vec2  gUV = p.xz;
            vec2  fw  = fwidth(gUV);
            vec2  gFr = fract(gUV) - 0.5;
            float lX  = smoothstep(fw.x * 2.0, 0.0, abs(abs(gFr.x) - 0.49));
            float lZ  = smoothstep(fw.y * 2.0, 0.0, abs(abs(gFr.y) - 0.49));
            float grid = max(lX, lZ) * 0.2;
            vec3 gndCol = vec3(0.015, 0.015, 0.03) + vec3(0.0, 0.05, 0.18) * grid;
            float shimmer = pow(max(0.0, 1.0 - abs(n.y)), 6.0) * 0.4;
            gndCol += hue2rgb(nHue) * shimmer * hdrGlow * audio;
            col = mix(vec3(0.0), gndCol, fog);

        } else if (mat < 1.5) {
            // Building — dark concrete with sparse lit windows
            vec3 bCol = vec3(0.04, 0.04, 0.06);
            vec2 wUV  = p.xy * 1.8 + p.xz * 0.5;
            vec2 wFr  = fract(wUV + 0.5);
            float wMask = smoothstep(0.30, 0.22, max(abs(wFr.x - 0.5), abs(wFr.y - 0.5)));
            float wHash = hash11(floor(wUV.x) + floor(wUV.y) * 19.0);
            bCol += hue2rgb(fract(nHue + 0.55)) * wMask * step(0.55, wHash) * 0.18 * hdrGlow;
            bCol += vec3(1.0) * spec * 0.12;
            col = mix(vec3(0.0), bCol, fog);

        } else {
            // Neon strip — HDR emissive, per-building hue, audio pulse
            float pulse = 0.88 + 0.12 * sin(TIME * 3.1 + nHue * 11.0);
            col = hue2rgb(nHue) * hdrGlow * 2.5 * pulse * audio;
            col = mix(vec3(0.0), col, fog);
        }

    } else {
        // Sky: deep black zenith, warm orange city glow at horizon, neon haze columns
        float skyY   = uv.y + 0.35;
        vec3  skyCol = mix(
            vec3(0.28, 0.04, 0.0) * 0.45,  // orange horizon
            vec3(0.0, 0.0, 0.018),          // near-black zenith
            clamp(skyY * 2.5, 0.0, 1.0)
        );
        float hzX   = floor(rd.x * 6.0);
        float hzHue = fract(hash11(hzX + 7.3) + paletteShift);
        float hz    = exp(-abs(skyY + hash11(hzX) * 0.4) * 9.0) * 0.35;
        skyCol += hue2rgb(hzHue) * hz * hdrGlow * 0.5;
        col = skyCol;
    }

    // Voice glitch
    if (_voiceGlitch > 0.01) {
        float g   = _voiceGlitch;
        float tt  = TIME * 17.0;
        vec2  uvN = gl_FragCoord.xy / RENDERSIZE.xy;
        float band      = floor(uvN.y * mix(8.0, 40.0, g) + tt * 3.0);
        float bandNoise = fract(sin(band * 91.7 + tt) * 43758.5453);
        float scanline  = 0.95 + 0.05 * sin(uvN.y * RENDERSIZE.y * 1.5 + tt * 40.0);
        float blockX    = floor(uvN.x * 6.0);
        float blockY    = floor(uvN.y * 4.0);
        float blockNoise = fract(sin((blockX + blockY * 7.0) * 113.1 + floor(tt * 8.0)) * 43758.5453);
        float dropout   = step(1.0 - g * 0.15, blockNoise);
        vec3  glitched  = col * scanline * (1.0 - dropout);
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = vec4(col, 1.0);
}
