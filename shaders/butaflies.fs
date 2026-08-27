/*{
  "DESCRIPTION": "Butaflies — a swarm of small, sharp-winged butterflies fluttering through a tintable sky. Two dozen pointed-wing butterflies at different depths — from particle-tiny sparks to delicate foreground fliers — flit and bob with real butterfly flight: erratic darts, flap-synced rise, drifting around a roaming swarm heart. Background and butterfly colors are fully yours. Bass breathes the swarm wide, mids stir the flutter, beats kick the wingbeats of a hashed subset, highs sparkle the sky grain.",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "swarmSize",    "LABEL": "Swarm Spread",   "TYPE": "float", "MIN": 0.4, "MAX": 1.6, "DEFAULT": 1.0,  "GROUP": "Shape / Geometry" },
    { "NAME": "flySize",      "LABEL": "Butterfly Size", "TYPE": "float", "MIN": 0.1, "MAX": 1.8, "DEFAULT": 0.5,  "GROUP": "Shape / Geometry" },
    { "NAME": "glowAmt",      "LABEL": "Glow",           "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.45, "GROUP": "Color" },
    { "NAME": "hazeAmt",      "LABEL": "Sky Haze",       "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5,  "GROUP": "Color" },
    { "NAME": "bgColor",      "LABEL": "Background Color","TYPE": "color", "DEFAULT": [0.55, 0.70, 0.92, 1.0], "GROUP": "Color" },
    { "NAME": "flyColor",     "LABEL": "Butterfly Color","TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0], "GROUP": "Color" },
    { "NAME": "paletteShift", "LABEL": "Palette Shift",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.0,  "GROUP": "Color" },
    { "NAME": "brightness",   "LABEL": "Brightness",     "TYPE": "float", "MIN": 0.3, "MAX": 2.0, "DEFAULT": 1.0,  "GROUP": "Color" },
    { "NAME": "motionSpeed",  "LABEL": "Motion Speed",   "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0,  "GROUP": "Motion / Animation" },
    { "NAME": "audioReact",   "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Audio Reactivity" },
    { "NAME": "trailAmt",   "LABEL": "Motion Trails", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Depth / Passes" },
    { "NAME": "bloomAmt",   "LABEL": "Bloom Depth",   "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "GROUP": "Depth / Passes" },
    { "NAME": "aberration", "LABEL": "Lens Depth",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "GROUP": "Depth / Passes" }
  ],
  "PASSES": [
    { "TARGET": "abTrail", "PERSISTENT": true },
    { }
  ]
}*/

// ─────────────────────────────────────────────────────────────────────────
// BUTAFLIES v2 — reworked per Lu's notes: butterflies are SMALL (particle
//   scale allowed, flySize floor 0.1), wings are POINTED (swept tapered
//   forewing + angled tapered hindwing — real butterfly silhouette, not
//   fat cartoon lobes), edges are CRISP (tight AA, tight glow), and
//   flight is NATURAL: erratic two-frequency flutter jitter, flap-synced
//   vertical bob (they rise on the downstroke), heading that twitches
//   like a real flier — no smooth planetary orbits. Background and
//   butterfly colors are direct color inputs; the sky gradient, haze and
//   fog all derive from bgColor so any palette holds together.
// ─────────────────────────────────────────────────────────────────────────

#define R RENDERSIZE.xy
#define PI 3.14159265
#define TAU 6.2831853

float hash11(float n) { return fract(sin(n) * 43758.5453123); }
float hash21(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21(i), hash21(i + vec2(1.0, 0.0)), f.x),
               mix(hash21(i + vec2(0.0, 1.0)), hash21(i + vec2(1.0, 1.0)), f.x), f.y);
}
float fbm(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 3; i++) {
        v += a * vnoise(p);
        p = p * 2.13 + vec2(7.3, 3.1);
        a *= 0.5;
    }
    return v;
}
vec3 hueRotate(vec3 c, float a) {
    float cs = cos(a), sn = sin(a);
    mat3 m = mat3(
        0.299 + 0.701*cs + 0.168*sn, 0.587 - 0.587*cs + 0.330*sn, 0.114 - 0.114*cs - 0.497*sn,
        0.299 - 0.299*cs - 0.328*sn, 0.587 + 0.413*cs + 0.035*sn, 0.114 - 0.114*cs + 0.292*sn,
        0.299 - 0.300*cs + 1.250*sn, 0.587 - 0.588*cs - 1.050*sn, 0.114 + 0.886*cs - 0.203*sn);
    return clamp(c * m, 0.0, 2.0);
}

// pointed butterfly silhouette; flap folds the wings' x; returns (mask, glow)
vec2 wingField(vec2 p, float flap) {
    p.x = abs(p.x) / max(flap, 0.12);
    // forewing: swept up-and-out, width tapers to a point at the tip
    vec2 fw = p - vec2(0.34, 0.16);
    float ca = cos(-0.52), sa = sin(-0.52);
    fw = vec2(ca * fw.x - sa * fw.y, sa * fw.x + ca * fw.y);
    float tipF = clamp(fw.x / 0.62 + 0.5, 0.0, 1.0);
    float eF = length(fw / vec2(0.60, max(0.24 * (1.0 - 0.62 * tipF), 0.02)));
    // hindwing: smaller, angled down, tapering to a soft tail point
    vec2 hw = p - vec2(0.20, -0.24);
    float cb = cos(0.48), sb = sin(0.48);
    hw = vec2(cb * hw.x - sb * hw.y, sb * hw.x + cb * hw.y);
    float tipH = clamp(hw.x / 0.42 + 0.5, 0.0, 1.0);
    float eH = length(hw / vec2(0.40, max(0.28 * (1.0 - 0.45 * tipH), 0.02)));
    // slim body
    float eB = length(p / vec2(0.05, 0.32));
    float e = min(min(eF, eH), eB);
    float mask = smoothstep(1.0, 0.965, e);
    float glow = exp(-max(e - 1.0, 0.0) * 6.5);
    return vec2(mask, glow);
}

vec3 renderScene() {
    vec2 fc = gl_FragCoord.xy;
    vec2 q = (fc - 0.5 * R) / R.y;

    float t   = TIME * motionSpeed * 0.55;
    float amt = audioReact;
    float bassP  = pow(smoothstep(0.05, 0.85, audioBass), 1.5);
    float midP   = pow(smoothstep(0.06, 0.85, audioMid),  1.2);
    float highP  = pow(smoothstep(0.10, 0.90, audioHigh), 1.2);
    float levelP = clamp(audioLevel, 0.0, 1.0);
    float beatP  = clamp(audioBeatPulse, 0.0, 1.0);

    // ── sky built from bgColor: darker up top, milk-lifted low ──────────
    vec3 bg = bgColor.rgb;
    vec3 skyTop = bg * 0.78;
    vec3 skyLow = mix(bg, vec3(1.0), 0.30);
    vec3 col = mix(skyLow, skyTop, smoothstep(-0.5, 0.55, q.y));
    float cloud = fbm(q * 2.1 + vec2(t * 0.015, 0.0));
    col = mix(col, mix(bg, vec3(1.0), 0.55), hazeAmt * 0.45 * smoothstep(0.45, 0.8, cloud));
    col = mix(col, bg * 0.62, hazeAmt * 0.35 * smoothstep(0.62, 0.9,
              fbm(q * 1.3 + vec2(-t * 0.01, 3.7))) * smoothstep(0.35, 0.85, length(q)));
    // soft key light
    col += bg * 0.18 * exp(-length(q - vec2(0.45, 0.42)) * 2.2);

    // ── the swarm ───────────────────────────────────────────────────────
    vec2 heart = vec2(0.16 * sin(t * 0.11) + 0.06 * sin(t * 0.043 + 1.7),
                      0.05 + 0.10 * sin(t * 0.077 + 0.6));
    float spread = swarmSize * (1.0 + amt * 0.18 * bassP + 0.04 * sin(t * 0.19));
    float flutterAmp = 1.0 + amt * 0.9 * midP;
    vec3 flyC = flyColor.rgb;

    for (int i = 0; i < 24; i++) {
        float fi = float(i);
        float h1 = hash11(fi * 12.9898 + 1.0);
        float h2 = hash11(fi * 4.1414 + 7.0);
        float h3 = hash11(fi * 7.7777 + 3.0);
        float h4 = hash11(fi * 2.3183 + 9.0);
        float h5 = hash11(fi * 9.2426 + 5.0);
        float react = step(0.6, hash11(fi * 5.55 + 2.0));

        float z = mix(0.55, 2.3, h4);
        float dir = (h2 > 0.5) ? 1.0 : -1.0;

        // flap first — flight bobs with the wingbeat
        float ft = t * (6.0 + 5.0 * h3) + h1 * TAU
                 + react * amt * 3.0 * beatP;
        float flap = 0.25 + 0.75 * abs(cos(ft));

        // drift around the heart + REAL flutter: two incommensurate
        // jitters and a flap-synced rise (butterflies climb on the
        // downstroke, sink between beats)
        float oa = h1 * TAU + dir * t * (0.04 + 0.10 * h3);
        float orad = (0.06 + 0.42 * h2) * spread;
        vec2 pos = heart + orad * vec2(cos(oa) * 1.15, sin(oa) * 0.8);
        pos += 0.045 * flutterAmp * vec2(sin(t * (1.2 + 1.1 * h5) + h1 * TAU),
                                         cos(t * (1.5 + 0.9 * h1) + h5 * TAU));
        pos += 0.020 * flutterAmp * vec2(sin(t * (3.4 + 2.2 * h3) + h2 * 40.0),
                                         sin(t * (2.8 + 1.9 * h2) + h3 * 20.0));
        pos.y += 0.012 * cos(ft) / z;                            // flap-synced bob

        float size = 0.055 * flySize * (0.7 + 0.6 * h5) / z;
        vec2 lp = (q - pos) / size;
        if (dot(lp, lp) > 6.5) continue;

        // heading: drift tangent + nervous twitch, pitched slightly up
        float head = oa + dir * PI * 0.5
                   + 0.30 * sin(t * (1.1 + h3) + h3 * TAU)
                   + 0.18 * sin(t * 3.2 + h1 * 9.0);
        float ca = cos(head), sa = sin(head);
        lp = vec2(ca * lp.x - sa * lp.y, sa * lp.x + ca * lp.y);

        vec2 wf = wingField(lp, flap);

        float fog = smoothstep(0.5, 2.3, z);
        float lum = (1.0 - 0.40 * fog)
                  * (1.0 + react * amt * (0.7 * beatP + 0.25 * levelP));
        vec3 bodyCol = mix(flyC, col, fog * 0.5);
        col = mix(col, bodyCol * lum, wf.x * (1.0 - 0.25 * fog));
        col += mix(flyC, vec3(1.0), 0.3) * wf.y * glowAmt * lum
             * (0.22 - 0.13 * fog) * (0.7 + 0.6 * flap);
    }

    // ── grain + finish ──────────────────────────────────────────────────
    float gr = hash21(fc + fract(t) * vec2(17.0, 29.0)) - 0.5;
    col += 0.022 * gr * (1.0 + amt * 0.8 * highP);
    if (paletteShift > 0.001) col = hueRotate(col, paletteShift * TAU);
    return col;
}

// ─── multipass: trail accumulation → bloom + lens-depth composite ────────
void main() {
    vec2 uv = gl_FragCoord.xy / R;
    if (PASSINDEX == 0) {
        vec3 col = renderScene();
        // luminous motion trails persist in the buffer (8-bit-safe decay floor)
        vec3 prev = texture2D(abTrail, uv).rgb;
        float decay = 0.50 + 0.46 * trailAmt;
        col = max(col, prev * decay - 0.0045);
        // write-dither: keeps soft gradients band-free in 8-bit buffer hosts
        col += (hash21(gl_FragCoord.xy + fract(TIME) * 61.0) - 0.5) * 0.006;
        gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
    } else {
        // chromatic lens depth: R/B pulled apart along the radial axis
        vec2 dir = uv - 0.5;
        float ab = aberration * 0.0045;
        vec3 base;
        base.r = texture2D(abTrail, uv + dir * ab).r;
        base.g = texture2D(abTrail, uv).g;
        base.b = texture2D(abTrail, uv - dir * ab).b;
        // wide two-ring bloom of the bright field — real glow depth
        vec3 bl = vec3(0.0);
        for (int i = 0; i < 8; i++) {
            float a = float(i) * 0.7853982;
            vec2 o = vec2(cos(a), sin(a)) * (3.5 / R.y);
            bl += texture2D(abTrail, uv + o).rgb;
            bl += texture2D(abTrail, uv + o * 2.6).rgb * 0.6;
        }
        bl /= 12.8;
        bl = max(bl - 0.52, 0.0);
        vec3 col = base + bl * bl * bloomAmt * 1.8;
        // audio lift applied post-trail so dips stay visible
        float lvl = clamp(audioLevel, 0.0, 1.0);
        col *= brightness * mix(1.0, 0.64 + 0.36 * lvl, audioReact)
             * (1.0 + audioReact * 0.08 * clamp(audioBeatPulse, 0.0, 1.0));
        // HD finish: S-curve contrast + saturation richness
        col = clamp(col, 0.0, 1.0);
        col = mix(col, col * col * (3.0 - 2.0 * col), 0.55);
        float lumHD = dot(col, vec3(0.299, 0.587, 0.114));
        col = clamp(mix(vec3(lumHD), col, 1.22), 0.0, 1.0);
        gl_FragColor = vec4(col, 1.0);
    }
}
