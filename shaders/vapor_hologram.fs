/*{
  "DESCRIPTION": "Neon Dreamscape — flat-graphic silhouette: solid-black mountain layers against magenta-to-cyan gradient sky; floating Y2K neon primitives; hologram scanline transmission. v8: 2D flat/graphic vs all prior 3D/scene approaches.",
  "CATEGORIES": ["Generator", "Glitch", "Audio Reactive"],
  "CREDIT": "ShaderClaw auto-improve v8",
  "INPUTS": [
    { "NAME": "scrollSpeed",    "LABEL": "Scroll Speed",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.4  },
    { "NAME": "mountainAmt",    "LABEL": "Mountain Layers","TYPE": "float", "MIN": 1.0, "MAX": 5.0,  "DEFAULT": 4.0  },
    { "NAME": "skyTop",         "LABEL": "Sky Top",        "TYPE": "color", "DEFAULT": [1.0, 0.05, 0.55, 1.0]       },
    { "NAME": "skyHorizon",     "LABEL": "Sky Horizon",    "TYPE": "color", "DEFAULT": [0.0,  0.9,  1.0, 1.0]       },
    { "NAME": "y2kCount",       "LABEL": "Y2K Objects",    "TYPE": "float", "MIN": 0.0, "MAX": 16.0, "DEFAULT": 8.0  },
    { "NAME": "y2kSpeed",       "LABEL": "Y2K Speed",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.5  },
    { "NAME": "hdrPeak",        "LABEL": "HDR Peak",       "TYPE": "float", "MIN": 1.0, "MAX": 4.0,  "DEFAULT": 2.8  },
    { "NAME": "holoGlow",       "LABEL": "Holo Glow",      "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.4  },
    { "NAME": "holoChroma",     "LABEL": "Chroma Shift",   "TYPE": "float", "MIN": 0.0, "MAX": 0.04, "DEFAULT": 0.01 },
    { "NAME": "audioReact",     "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0  }
  ],
  "PASSES": [
    { "TARGET": "scene" },
    {}
  ]
}*/

float hash11(float n) { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Smooth noise: value noise 1D
float vnoise(float x) {
    float i = floor(x);
    float f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    return mix(hash11(i), hash11(i + 1.0), f);
}

// Mountain silhouette at a given layer: ridgeline height at x
float ridge(float x, float layer) {
    float h = 0.0;
    float amp = 0.12, freq = 1.8;
    for (int i = 0; i < 5; i++) {
        h += vnoise(x * freq + layer * 7.31 + float(i) * 3.17) * amp;
        freq *= 2.1; amp *= 0.55;
    }
    return h + 0.08 + layer * 0.14;
}

// Y2K SDF shapes (reused from prior)
float sdStar5(vec2 p, float r) {
    const vec2 k1 = vec2(0.809016994, -0.587785252);
    const vec2 k2 = vec2(-k1.x, k1.y);
    p.x = abs(p.x);
    p -= 2.0 * max(dot(k1, p), 0.0) * k1;
    p -= 2.0 * max(dot(k2, p), 0.0) * k2;
    p.x = abs(p.x);
    p.y -= r;
    vec2 ba = vec2(-0.309016994, 0.951056516) * 0.4;
    float hh = clamp(dot(p, ba) / dot(ba, ba), 0.0, 1.0);
    return length(p - ba * hh) * sign(p.y * ba.x - p.x * ba.y);
}
float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 q = abs(p) - b + r;
    return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - r;
}
float sdSparkle(vec2 p) {
    return min(max(abs(p.x) - 0.07, abs(p.y) - 0.27),
               max(abs(p.y) - 0.07, abs(p.x) - 0.27));
}

// ──────────────────────────────────────────────────────────────────────
// PASS 0 — scene
// ──────────────────────────────────────────────────────────────────────
vec4 passScene(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    float audio = 1.0 + audioLevel * audioReact;

    // Sky gradient: top=magenta, horizon=cyan
    vec3 col = mix(skyHorizon.rgb * hdrPeak, skyTop.rgb * hdrPeak,
                   smoothstep(0.0, 0.85, uv.y));

    // Horizontal neon scan-stripe (pulsing)
    float stripe = abs(sin(uv.y * 40.0 + TIME * scrollSpeed * 3.0)) * 0.06;
    col += skyHorizon.rgb * stripe * audio;

    // Mountain silhouettes — from back to front, each darker
    int N = int(clamp(mountainAmt, 1.0, 5.0));
    float t = TIME * scrollSpeed * 0.15;
    for (int i = 0; i < 5; i++) {
        if (i >= N) break;
        float fi = float(i);
        float layerX = uv.x + t * (0.3 + fi * 0.12); // parallax scroll
        float h = ridge(layerX, fi);
        // Mountain is BLACK below the ridgeline
        if (uv.y < h) {
            // Silhouette: pure black but with neon edge glow
            float edgeDist = h - uv.y;
            float edge = exp(-edgeDist * 60.0);
            // Edge glows in the mountain's accent color
            float hue = fi * 0.17 + 0.8; // violet → magenta range
            vec3 accent = hsv2rgb(vec3(fract(hue), 1.0, 1.0)) * hdrPeak * edge;
            col = vec3(0.0) + accent;
            break; // frontmost mountain wins
        }
    }

    // Moon / gradient circle in sky
    vec2 mc = vec2(0.72, 0.78);
    mc.x += sin(TIME * 0.07) * 0.02;
    float mr = 0.10 * (1.0 + audioBass * audioReact * 0.05);
    float moonD = length((uv - mc) * vec2(aspect, 1.0));
    if (moonD < mr && uv.y > ridge(mc.x, 0.0)) {
        float nm = moonD / mr;
        vec3 moonC = mix(vec3(3.0, 3.0, 3.0), skyTop.rgb * hdrPeak, nm * nm);
        col = moonC;
    }
    // Moon glow halo
    float moonGlow = exp(-moonD * moonD / (mr * mr * 0.5)) * 0.4;
    if (uv.y > ridge(uv.x, 0.0))
        col += skyTop.rgb * moonGlow * hdrPeak;

    // Y2K neon shapes floating in sky
    int Y = int(clamp(y2kCount, 0.0, 16.0));
    for (int i = 0; i < 16; i++) {
        if (i >= Y) break;
        float fi = float(i);
        float bx = fract(hash11(fi * 1.37) + TIME * y2kSpeed * (0.04 + hash11(fi * 2.7) * 0.06));
        float by = 0.45 + hash11(fi * 5.13) * 0.45; // upper half of sky
        vec2 ctr = vec2(bx, by);
        float sz = 0.025 + hash11(fi * 3.1) * 0.035;
        sz *= 1.0 + audioBass * audioReact * 0.3;
        float hue = fract(fi * 0.19 + TIME * 0.04);
        vec3 shapeCol = hsv2rgb(vec3(hue, 1.0, 1.0)) * hdrPeak;
        float rot = TIME * (0.4 + hash11(fi * 7.1) * 1.0);
        float ca = cos(rot), sa = sin(rot);
        vec2 d = uv - ctr; d.x *= aspect;
        vec2 lp = vec2(ca * d.x - sa * d.y, sa * d.x + ca * d.y) / max(sz, 1e-4);
        int kind = int(hash11(fi * 41.7) * 3.0);
        float dist;
        if      (kind == 0) dist = sdStar5(lp, 0.80);
        else if (kind == 1) dist = sdSparkle(lp * 1.1);
        else                dist = sdRoundBox(lp, vec2(0.7, 0.35), 0.15);
        // Only draw if in sky (above frontmost mountain)
        if (uv.y > ridge(uv.x, 0.0)) {
            float vis = smoothstep(0.0, -0.05, dist);
            col = mix(col, shapeCol, vis * 0.9);
            // Neon outline glow
            float outline = exp(-abs(dist) * 25.0);
            col += shapeCol * outline * 0.5;
            // Black ink interior border
            float ink = smoothstep(0.04, 0.0, abs(dist));
            col = mix(col, vec3(0.0), ink * 0.4 * vis);
        }
    }

    return vec4(col, 1.0);
}

// ──────────────────────────────────────────────────────────────────────
// PASS 1 — hologram glitch
// ──────────────────────────────────────────────────────────────────────
vec4 passHologram(vec2 fragCoord) {
    vec2 uv = fragCoord / RENDERSIZE.xy;

    // Vertical tear
    float bandH = 0.03;
    float bandY = floor(uv.y / bandH) * bandH;
    float tearTrig = step(0.93, hash21(vec2(bandY, floor(TIME * 6.0))));
    uv.x += tearTrig * (hash21(vec2(bandY, TIME)) - 0.5) * 0.12;

    // Chromatic shift
    float ch = holoChroma * (1.0 + audioHigh * audioReact);
    float r = texture(scene, clamp(uv + vec2( ch, 0.0), 0.0, 1.0)).r;
    float g = texture(scene, clamp(uv,                  0.0, 1.0)).g;
    float b = texture(scene, clamp(uv - vec2( ch, 0.0), 0.0, 1.0)).b;
    vec3 holo = vec3(r, g, b);

    // Scanlines
    holo *= 0.88 + 0.12 * sin(gl_FragCoord.y * 1.2);

    // Edge glow
    float lum = dot(holo, vec3(0.299, 0.587, 0.114));
    holo += vec3(0.2, 0.8, 1.0) * pow(lum, 1.6) * holoGlow * 0.25;

    // Fix audio blackout: minimum 90% transmission
    holo *= max(0.9, 0.7 + audioLevel * audioReact * 0.4);

    return vec4(holo, 1.0);
}

void main() {
    if (PASSINDEX == 0) FragColor = passScene(gl_FragCoord.xy);
    else                FragColor = passHologram(gl_FragCoord.xy);
}
