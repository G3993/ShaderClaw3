/*{
  "DESCRIPTION": "Underwater — looking up from the deep. Volumetric god rays pierce down through dancing caustics, rising bubbles trail upward toward the rippling surface. Slow parallax drift adds depth; soft directional light/shadow planes give the water column a 3D feel. Deep-to-aqua depth gradient, HDR linear output so the sun disc and brightest caustic peaks catch bloom.",
  "CREDIT": "ShaderClaw — original underwater god-ray composition, depth/light extensions",
  "CATEGORIES": [
    "Generator",
    "3D",
    "Audio Reactive"
  ],
  "INPUTS": [
    {
      "NAME": "sunPosX",
      "LABEL": "Sun X",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 1,
      "GROUP": "Camera / Layout"
    },
    {
      "NAME": "sunPosY",
      "LABEL": "Sun Y",
      "TYPE": "float",
      "DEFAULT": 0.92,
      "MIN": 0.5,
      "MAX": 1,
      "GROUP": "Camera / Layout"
    },
    {
      "NAME": "vignette",
      "LABEL": "Vignette",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": 0,
      "MAX": 0.8,
      "GROUP": "Camera / Layout"
    },
    {
      "NAME": "godrayIntensity",
      "LABEL": "God-Ray Intensity",
      "TYPE": "float",
      "DEFAULT": 1.2,
      "MIN": 0,
      "MAX": 3
    },
    {
      "NAME": "godraySamples",
      "LABEL": "Ray Samples",
      "TYPE": "long",
      "DEFAULT": 64,
      "VALUES": [
        16,
        32,
        48,
        64,
        96,
        128
      ],
      "LABELS": [
        "16",
        "32",
        "48",
        "64",
        "96",
        "128"
      ]
    },
    {
      "NAME": "godrayDecay",
      "LABEL": "Ray Decay",
      "TYPE": "float",
      "DEFAULT": 0.965,
      "MIN": 0.85,
      "MAX": 0.995
    },
    {
      "NAME": "causticIntensity",
      "LABEL": "Caustics",
      "TYPE": "float",
      "DEFAULT": 1.2,
      "MIN": 0,
      "MAX": 3
    },
    {
      "NAME": "causticScale",
      "LABEL": "Caustic Scale",
      "TYPE": "float",
      "DEFAULT": 4,
      "MIN": 1,
      "MAX": 10
    },
    {
      "NAME": "bubbleCount",
      "LABEL": "Bubbles",
      "TYPE": "long",
      "DEFAULT": 24,
      "VALUES": [
        0,
        12,
        24,
        48,
        96
      ],
      "LABELS": [
        "0",
        "12",
        "24",
        "48",
        "96"
      ]
    },
    {
      "NAME": "driftAmount",
      "LABEL": "Drift Amount",
      "TYPE": "float",
      "DEFAULT": 0.55,
      "MIN": 0,
      "MAX": 2
    },
    {
      "NAME": "lightAngle",
      "LABEL": "Light Angle",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": -1.57,
      "MAX": 1.57
    },
    {
      "NAME": "shadowSoftness",
      "LABEL": "Shadow Softness",
      "TYPE": "float",
      "DEFAULT": 0.55,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "shadowDepth",
      "LABEL": "Shadow Depth",
      "TYPE": "float",
      "DEFAULT": 0.4,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "causticSpeed",
      "LABEL": "Caustic Speed",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": 0,
      "MAX": 1.5,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "bubbleRise",
      "LABEL": "Bubble Rise",
      "TYPE": "float",
      "DEFAULT": 0.12,
      "MIN": 0,
      "MAX": 0.6,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "driftSpeed",
      "LABEL": "Drift Speed",
      "TYPE": "float",
      "DEFAULT": 0.04,
      "MIN": 0,
      "MAX": 0.2,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "depthColor",
      "LABEL": "Deep Color",
      "TYPE": "color",
      "DEFAULT": [
        0.015,
        0.045,
        0.115,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "surfaceColor",
      "LABEL": "Surface Color",
      "TYPE": "color",
      "DEFAULT": [
        0.2,
        0.65,
        0.85,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "sunColor",
      "LABEL": "Sun Color",
      "TYPE": "color",
      "DEFAULT": [
        1,
        0.95,
        0.8,
        1
      ],
      "GROUP": "Color"
    },
    {
      "NAME": "hueShift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0,
      "LABEL": "Hue Shift",
      "GROUP": "Color"
    },
    {
      "NAME": "colorBoost",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "LABEL": "Color Boost",
      "GROUP": "Color"
    },
    {
      "NAME": "bgColor",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        0,
        0
      ],
      "LABEL": "Background",
      "GROUP": "Background"
    },
    {
      "NAME": "audioReact",
      "LABEL": "Audio React",
      "TYPE": "float",
      "DEFAULT": 0.8,
      "MIN": 0,
      "MAX": 2,
      "GROUP": "Audio Reactivity"
    }
  ]
}*/

// ====================================================================
// Underwater — looking up from the deep.
//
//   • Depth gradient → fake 3D depth
//   • Animated caustics → wavy light refraction patterns
//   • Volumetric god rays → radial brightness march toward the sun
//   • Directional water-column light/shadow planes (new)
//   • Slow parallax drift on multiple depth layers (new)
//   • Ambient occlusion-style depth darkening (new)
//   • Rising bubbles with rim highlights
//   • Drifting motes / particulate
//   • Surface ripple haze
//
// HDR linear output — peaks > 1.0 intentional for bloom.
// ====================================================================

#define PI 3.14159265

// ─── Utility hashes ───────────────────────────────────────────────
float h11(float x)  { return fract(sin(x * 127.1) * 43758.5453); }
float h12(vec2 p)   { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// Value noise
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(h12(i),               h12(i + vec2(1,0)), f.x),
               mix(h12(i + vec2(0,1)),   h12(i + vec2(1,1)), f.x), f.y);
}

// Two-octave fbm for drift/warp
float fbm2(vec2 p) {
    return 0.5 * vnoise(p) + 0.25 * vnoise(p * 2.03 + 5.7);
}

// ─── Caustics ─────────────────────────────────────────────────────
// Sharp caustic field — 4 oriented sin layers with sharpened peaks.
float caustic(vec2 p, float t) {
    vec2 q = p;
    q.x += sin(p.y * 1.5 + t * 0.7) * 0.45;
    q.y += cos(p.x * 1.8 - t * 0.9) * 0.45;
    float c = 0.0;
    for (int i = 0; i < 4; i++) {
        float fi = float(i);
        vec2 d = vec2(sin(fi * 1.7), cos(fi * 2.3));
        float k = 1.6 + fi * 0.45;
        float s = 0.5 + 0.5 * sin(dot(q, d) * k + t * (0.3 + fi * 0.2));
        c += pow(s, 6.0);
    }
    return c * 0.6;
}

// Cheap version for the god-ray loop.
float causticFast(vec2 p, float t) {
    float a = 0.5 + 0.5 * sin(p.x * 12.0 + t * 1.5);
    float b = 0.5 + 0.5 * sin(p.y * 14.0 - t * 1.1);
    return pow(a * b, 3.0);
}

// ─── Directional light / shadow planes ────────────────────────────
// Simulates shafts of shadow cast by unseen surface debris / waves.
// We build several soft "shadow bands" perpendicular to the light
// direction and fade them with depth so they appear to originate near
// the surface and dissolve before they reach the abyss.
float shadowPlanes(vec2 uv, float aspect, float t) {
    // Light direction vector in screen space, derived from lightAngle.
    vec2 ldir = vec2(sin(lightAngle), cos(lightAngle));
    // Project uv onto the perpendicular axis to get band coordinate.
    vec2 uvA = vec2(uv.x * aspect, uv.y);
    float proj = dot(uvA, vec2(-ldir.y, ldir.x));
    // Slowly drifting band offset (very slow — almost imperceptible but
    // adds that sense of the surface moving far above).
    float offset = t * driftSpeed * 0.4;
    // Three overlapping bands at different frequencies / speeds.
    float bands =
        0.50 * (0.5 + 0.5 * sin((proj * 3.1 + offset * 1.0) * 4.0))
      + 0.30 * (0.5 + 0.5 * sin((proj * 5.3 - offset * 0.7) * 6.5))
      + 0.20 * (0.5 + 0.5 * sin((proj * 8.7 + offset * 1.3) * 9.0));
    // Shadow is strongest near the surface, fades toward the deep.
    float depthFade = smoothstep(0.0, 0.75, uv.y);
    // Softness control: low = crisp bands, high = barely visible.
    float sharpness = mix(12.0, 1.5, shadowSoftness);
    float shadow = pow(bands, sharpness);
    return shadow * depthFade * shadowDepth;
}

// ─── Parallax drift layers ─────────────────────────────────────────
// Returns a soft volumetric haze contribution from "depth layer" i,
// with a slow organic drift. Layering creates parallax depth cues.
vec3 driftLayer(vec2 uv, float aspect, float t, float layerIndex,
                vec3 tint, float brightness) {
    float li = layerIndex;
    // Each layer drifts at a slightly different speed and direction —
    // slower layers appear further away (parallax).
    float speed  = driftSpeed * (0.3 + li * 0.25);
    vec2  drift  = vec2(sin(t * speed + li * 2.3),
                        cos(t * speed * 0.7 + li * 1.7)) * driftAmount * 0.06;
    // Scale and warp the noise lookup per layer.
    float sc = 2.8 + li * 1.6;
    vec2  p  = uv * vec2(aspect, 1.0) * sc + drift + li * vec2(3.1, 7.4);
    float n  = fbm2(p + fbm2(p * 0.6) * 0.5);
    // A soft blob shape — pow sharpens so we get distinct cloudy pockets.
    float blob = pow(clamp(n, 0.0, 1.0), 3.5);
    // Fade toward the top (absorbed by surface light) and at the very bottom.
    float depthFade = smoothstep(0.0, 0.2, uv.y) * smoothstep(1.0, 0.55, uv.y);
    return tint * blob * brightness * depthFade;
}

void main() {
    vec2  res    = RENDERSIZE;
    vec2  uv     = gl_FragCoord.xy / res;
    float aspect = res.x / res.y;
    float audio  = clamp(audioReact, 0.0, 2.0);
    float t      = TIME;

    // ─── Slow global drift — gentle horizontal sway of the whole
    // scene, as if we ourselves are drifting with the current.
    float driftX = sin(t * driftSpeed * 0.8)        * driftAmount * 0.012;
    float driftY = sin(t * driftSpeed * 0.53 + 1.2) * driftAmount * 0.006;
    vec2  uvD    = uv + vec2(driftX, driftY);
    // Clamp so sampling stays sane near edges.
    uvD = clamp(uvD, 0.001, 0.999);

    // Aspect-corrected spatial UV for features.
    vec2 puv = vec2(uvD.x * aspect, uvD.y);

    // ─── Depth gradient ───────────────────────────────────────────
    float depth = pow(uvD.y, 1.15);
    vec3 col = mix(depthColor.rgb, surfaceColor.rgb, depth);

    // ─── Ambient-occlusion-style depth darkening ──────────────────
    // Water absorbs light; the deeper we look the darker it gets.
    // This is separate from the gradient — it's a multiplicative term
    // so colours stay physically plausible rather than just shifting hue.
    float aoDepth = mix(0.30, 0.85, pow(uvD.y, 0.7));
    col *= aoDepth;

    // ─── Fine-grain water detail (secondary noise layer) ──────────
    // Adds subtle micro-structure to the water body itself, on top of
    // the smooth depth gradient, so it never reads as a flat wash of
    // colour — this is the richer "density" layer the deep water needs.
    float grain = vnoise(puv * 18.0 + t * 0.03) * 0.5
                + vnoise(puv * 37.0 - t * 0.021 + 4.1) * 0.5;
    col *= 1.0 + (grain - 0.5) * 0.24;

    // ─── Micro-contour lines ────────────────────────────────────────
    // Thin bright threads traced along the steepest edges of the grain
    // field — reads like fine light-refraction lacing on the water
    // body, and gives the frame real edge structure instead of smooth
    // gradients everywhere.
    float gEps = 0.006;
    float gGx = vnoise(puv * 18.0 + t * 0.03 + vec2(gEps, 0.0))
              - vnoise(puv * 18.0 + t * 0.03 - vec2(gEps, 0.0));
    float gGy = vnoise(puv * 18.0 + t * 0.03 + vec2(0.0, gEps))
              - vnoise(puv * 18.0 + t * 0.03 - vec2(0.0, gEps));
    float contour = clamp(length(vec2(gGx, gGy)) * 22.0 - 0.35, 0.0, 1.0);
    col += mix(depthColor.rgb, surfaceColor.rgb, 0.6) * contour * 0.22;

    // ─── Volumetric drift / haze layers ──────────────────────────
    // Three parallax layers of soft particulate/haze tinted to the
    // water column colour.  Brightness is kept subtle.
    vec3 hazeCol = mix(depthColor.rgb, surfaceColor.rgb, 0.5);
    col += driftLayer(uvD, aspect, t, 0.0, hazeCol, 0.06);
    col += driftLayer(uvD, aspect, t, 1.0, hazeCol * 1.1, 0.04);
    col += driftLayer(uvD, aspect, t, 2.0, hazeCol * 0.9, 0.03);

    // ─── Directional shadow planes ────────────────────────────────
    float shad = shadowPlanes(uvD, aspect, t);
    col *= 1.0 - shad * 0.6;  // Darken — never go fully black.

    // ─── Caustics ─────────────────────────────────────────────────
    float surfaceProx = smoothstep(0.0, 0.85, uvD.y);
    float c1 = caustic(puv * causticScale,                              t * causticSpeed);
    float c2 = caustic(puv * causticScale * 1.7 + vec2(5.3, 2.1),       t * causticSpeed * 1.3);
    float c  = max(c1, c2 * 0.7) * surfaceProx;
    c *= 1.0 + audioMid * audio * 0.7;
    // Caustics also respect the shadow planes — bright patches sit
    // between shadow bands, mimicking real underwater light.
    c *= 0.65 + shad * 0.35;
    col += sunColor.rgb * c * causticIntensity * 0.20;

    // ─── God rays ─────────────────────────────────────────────────
    vec2 sunUV = vec2(sunPosX, sunPosY);
    int N = int(godraySamples);
    if (N < 4)   N = 4;
    if (N > 256) N = 256;
    float Nf = float(N);
    vec2 deltaUV   = (uvD - sunUV) / Nf;
    vec2 samplePos = uvD;
    float illum  = 0.0;
    float weight = 1.0;
    for (int i = 0; i < 256; i++) {
        if (i >= N) break;
        samplePos -= deltaUV;
        float maskDist = length((samplePos - sunUV) * vec2(aspect * 0.6, 1.0));
        float sunMask  = exp(-maskDist * 3.2);
        float surfMask = causticFast(samplePos * vec2(aspect, 1.0), t * causticSpeed * 0.8);
        surfMask *= smoothstep(0.55, 1.0, samplePos.y);
        illum  += max(sunMask, surfMask * 0.55) * weight;
        weight *= godrayDecay;
    }
    illum /= Nf;
    illum *= godrayIntensity;
    illum *= 1.0 + audioBass * audio * 1.3;
    col += sunColor.rgb * illum * 0.75;

    // ─── Sun disc + halo ──────────────────────────────────────────
    float sunDist = length((uvD - sunUV) * vec2(aspect, 1.0));
    float sunDisc = smoothstep(0.12, 0.02, sunDist);
    col += sunColor.rgb * sunDisc * 1.5;
    float halo = exp(-sunDist * 2.6);
    col += sunColor.rgb * halo * 0.30;

    // ─── Directional side-light scattering ───────────────────────
    // A soft glow coming from the light direction, giving the water
    // column a sense of being lit from one side (like sunlight entering
    // at an angle).  Very gentle — just enough to notice.
    vec2 ldir2D = normalize(vec2(sin(lightAngle), cos(lightAngle)));
    float sideLight = dot(normalize(vec2(uvD.x * aspect - sunPosX * aspect,
                                         uvD.y - sunPosY)), ldir2D);
    sideLight = clamp(sideLight, 0.0, 1.0);
    sideLight = pow(sideLight, 3.0);
    sideLight *= smoothstep(1.0, 0.3, uvD.y); // only below surface
    col += sunColor.rgb * sideLight * 0.05 * (1.0 - aoDepth + 0.5);

    // ─── Bubbles ──────────────────────────────────────────────────
    int B = int(bubbleCount);
    if (B > 96) B = 96;
    for (int i = 0; i < 96; i++) {
        if (i >= B) break;
        float fi   = float(i);
        vec2 seed  = vec2(h11(fi * 7.1), h11(fi * 13.3));
        float bx   = seed.x * aspect;
        float size = mix(0.003, 0.014, h11(fi * 23.7));
        float life = mix(4.0, 12.0, h11(fi * 31.1));
        float phase = fract((t + seed.y * 100.0) * bubbleRise / life * 8.0);
        // Gentle lateral drift on bubbles too — they follow the current.
        bx += sin(t * 0.5 + fi * 2.1) * 0.018;
        bx += driftX * 0.5;
        vec2 bp = vec2(bx, phase);
        float bd = length(puv - bp);
        float fill = smoothstep(size, size * 0.4, bd);
        float rim  = smoothstep(size * 0.95, size * 0.75, bd) -
                     smoothstep(size * 0.75, size * 0.5,  bd);
        float lifeFade = smoothstep(0.0, 0.10, phase) *
                         smoothstep(1.0, 0.85, phase);
        // Bubbles are slightly brighter on the side facing the light.
        float bubbleLit = 0.7 + 0.3 * sin(lightAngle + atan(bp.y - 0.5, bp.x - sunPosX * aspect));
        col += vec3(0.65, 0.85, 0.95) * fill * lifeFade * 0.22 * bubbleLit;
        col += vec3(1.30, 1.55, 1.70) * rim  * lifeFade * 0.65;
    }

    // ─── Drifting motes / particulate ────────────────────────────
    // UV offset so motes drift slowly with the current. Cell-hashed dots:
    // pixel-sharp cores with a soft additive halo — the old pow(vnoise,9)
    // read as blurry upscaled noise blobs instead of marine snow.
    vec2 moteUV = uv * 90.0 + vec2(driftX * 12.0, t * 0.06 + driftY * 8.0);
    vec2 mCell = floor(moteUV);
    vec2 mF    = fract(moteUV);
    float motes = 0.0;
    if (h12(mCell * 1.13 + 7.7) > 0.80) {              // sparse occupancy
        vec2  mPos = vec2(h12(mCell + 3.1), h12(mCell + 9.4)) * 0.6 + 0.2;
        float mR   = mix(0.06, 0.16, h12(mCell + 5.2));  // radius in cell units
        float mD   = length(mF - mPos);
        float mPx  = 90.0 / RENDERSIZE.y;                // one pixel in cell units
        float mCore = 1.0 - smoothstep(mR - 1.5 * mPx, mR + 1.5 * mPx, mD);
        float mHalo = exp(-mD * mD / (mR * mR * 6.0)) * 0.35;
        motes = (mCore + mHalo) * (0.5 + 0.5 * h12(mCell + 12.9));
    }
    col += vec3(0.45, 0.65, 0.85) * motes * 0.45;

    // ─── Soft highlight compression ────────────────────────────────
    // Many additive light terms (caustics, god rays, sun halo, bubble
    // rims) stack on top of each other; without this they crush the
    // whole frame to flat white. This keeps genuine highlights (sun
    // core, caustic peaks) bright enough to still trigger bloom while
    // preserving the depth gradient / colour structure everywhere else.
    float peak = max(col.r, max(col.g, col.b));
    if (peak > 0.88) {
        float excess = peak - 0.88;
        float comp = 0.88 + (1.0 - exp(-excess * 1.2)) * 0.7;
        col *= comp / peak;
    }

    // ─── Surface ripple band ──────────────────────────────────────
    if (uvD.y > 0.90) {
        float ripple = 0.5 + 0.5 * sin(uvD.x * 80.0 * aspect + t * 3.5);
        ripple *= smoothstep(0.90, 1.0, uvD.y);
        col += sunColor.rgb * ripple * 0.20;
    }

    // ─── Vignette ─────────────────────────────────────────────────
    vec2 vuv = uv * (1.0 - uv.yx);
    float vig = pow(max(vuv.x * vuv.y * 16.0, 0.0), max(vignette, 0.001));
    col *= mix(1.0, vig, clamp(vignette, 0.0, 1.0));

    // ---- universal color block (defaults = no-op) ----
    vec3 uc = col;
    float ucL = dot(uc, vec3(0.299, 0.587, 0.114));
    uc = mix(vec3(ucL), uc, colorBoost);                     // saturation
    if (hueShift > 0.0005) {                                  // cheap hue rotate (YIQ)
        float hA = hueShift * 6.2831853;
        float hC = cos(hA), hS = sin(hA);
        mat3 hM = mat3(0.299,0.587,0.114, 0.299,0.587,0.114, 0.299,0.587,0.114)
                + hC * mat3(0.701,-0.587,-0.114, -0.299,0.413,-0.114, -0.300,-0.588,0.886)
                + hS * mat3(0.168,0.330,-0.497, -0.328,0.035,0.292, 1.250,-1.050,-0.203);
        uc = clamp(hM * uc, 0.0, 1.0);
    }
    // background: tint the darkest end (the deep-water void) toward bgColor
    uc = mix(uc, bgColor.rgb, bgColor.a * (1.0 - smoothstep(0.0, 0.35, ucL)));
    col = uc;

    // HDR linear output — peaks > 1.0 are intentional for bloom.
    gl_FragColor = vec4(col, 1.0);
}