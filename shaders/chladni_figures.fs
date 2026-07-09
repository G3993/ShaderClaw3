/*{
  "CATEGORIES": [
    "Generator",
    "Cymatics",
    "Audio Reactive"
  ],
  "DESCRIPTION": "Chladni Figures with depth lighting, soft shadows, and slow drift. Sand on a vibrating plate (Ernst Chladni, 1787). Nodal-line equation Z = sin(nπx)sin(mπy) − sin(mπx)sin(nπy) solved per-pixel; sand collects along zero-crossings. Deferred-style normal map gives the sand ridges 3-D depth via a directional light with diffuse + specular. A slow Lissajous drift gently rocks the plate. Bass triggers mode jumps; mid/treble drive brightness and grain.",
  "INPUTS": [
    {
      "NAME": "mood",
      "LABEL": "Mood",
      "TYPE": "long",
      "DEFAULT": 0,
      "VALUES": [
        0,
        1,
        2
      ],
      "LABELS": [
        "Sand on Plate",
        "Water Cymatics",
        "Iron Filings + Magnet"
      ]
    },
    {
      "NAME": "lightAngle",
      "LABEL": "Light Angle",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 6.283,
      "DEFAULT": 0.785
    },
    {
      "NAME": "lightElevation",
      "LABEL": "Light Elevation",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 1,
      "DEFAULT": 0.55
    },
    {
      "NAME": "lightIntensity",
      "LABEL": "Light Intensity",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "glossiness",
      "LABEL": "Glossiness",
      "TYPE": "float",
      "MIN": 4,
      "MAX": 180,
      "DEFAULT": 48
    },
    {
      "NAME": "lineWidth",
      "LABEL": "Sand Line Width",
      "TYPE": "float",
      "MIN": 0.4,
      "MAX": 3.5,
      "DEFAULT": 1.4,
      "GROUP": "Shape / Geometry"
    },
    {
      "NAME": "grainDensity",
      "LABEL": "Grain Density",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.55,
      "GROUP": "Shape / Geometry"
    },
    {
      "NAME": "morphPeriod",
      "LABEL": "Mode Period (s)",
      "TYPE": "float",
      "MIN": 4,
      "MAX": 14,
      "DEFAULT": 8,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "settle",
      "LABEL": "Settle vs Agitate",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.5,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "driftAmount",
      "LABEL": "Drift Amount",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.4,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "driftSpeed",
      "LABEL": "Drift Speed",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.3,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "warmth",
      "LABEL": "Warmth",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.7,
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
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1,
      "GROUP": "Audio Reactivity"
    }
  ]
}*/

// ════════════════════════════════════════════════════════════════════════
//  Chladni Figures — after Ernst Chladni 1787.
//  New in this version:
//    • Deferred-style normal map: the sand ridge height field is
//      differentiated analytically to produce a per-pixel normal,
//      then lit with diffuse + Blinn-Phong specular + ambient occlusion.
//    • A slow Lissajous drift gently rocks the virtual camera so the
//      plate never feels frozen (inspired by the Gravity Streams attractor
//      drift technique from flow.fs).
//    • Soft self-shadow: a second field sample offset along the light
//      direction cheaply occludes the underside of ridges.
// ════════════════════════════════════════════════════════════════════════

#define PI 3.14159265359

// ─── hashes ──────────────────────────────────────────────────────────
float hash11(float n)  { return fract(sin(n * 12.9898) * 43758.5453); }
float hash21(vec2 p)   { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// ─── 12 curated (n,m) mode pairs ─────────────────────────────────────
vec2 modePair(int i) {
    if (i ==  0) return vec2( 3.0,  5.0);
    if (i ==  1) return vec2( 2.0,  7.0);
    if (i ==  2) return vec2( 4.0,  6.0);
    if (i ==  3) return vec2( 5.0,  7.0);
    if (i ==  4) return vec2( 3.0,  8.0);
    if (i ==  5) return vec2( 6.0,  9.0);
    if (i ==  6) return vec2( 4.0, 11.0);
    if (i ==  7) return vec2( 7.0, 10.0);
    if (i ==  8) return vec2( 5.0, 12.0);
    if (i ==  9) return vec2( 8.0, 11.0);
    if (i == 10) return vec2( 2.0,  9.0);
    return            vec2( 6.0, 13.0);
}

// ─── Chladni scalar field ─────────────────────────────────────────────
float chladni(vec2 p, vec2 nm) {
    float a = sin(nm.x * PI * p.x) * sin(nm.y * PI * p.y);
    float b = sin(nm.y * PI * p.x) * sin(nm.x * PI * p.y);
    return a - b;
}

// ─── Analytic gradient of the Chladni field ───────────────────────────
//  dZ/dx and dZ/dy, used to build a surface normal for the sand ridge.
vec2 chladniGrad(vec2 p, vec2 nm) {
    float nx = nm.x * PI;
    float ny = nm.y * PI;
    // dZ/dx
    float dzdx = nx * cos(nx * p.x) * sin(ny * p.y)
               - ny * cos(ny * p.x) * sin(nx * p.y);
    // dZ/dy
    float dzdy = ny * sin(nx * p.x) * cos(ny * p.y)
               - nx * sin(ny * p.x) * cos(nx * p.y);
    return vec2(dzdx, dzdy);
}

// ─── Nodal-line coverage (AA'd) ───────────────────────────────────────
float nodalLine(vec2 p, vec2 nm, float widthPx) {
    float Z   = chladni(p, nm);
    float aaw = max(fwidth(Z), 1e-5);
    float d   = abs(Z) / aaw;
    return 1.0 - smoothstep(widthPx * 0.5, widthPx * 0.5 + 1.2, d);
}

// ─── Height field for the sand ridge ─────────────────────────────────
//  The ridge has maximum height at the nodal zero; it is a smooth bump
//  that falls off with |Z|.  We use a Gaussian envelope so the normal
//  is well-defined even far from the line.
float ridgeHeight(float Z, float aaw, float widthPx, float ridgeAmp) {
    float sigma = widthPx * 0.5 * aaw;  // screen-space half-width in Z-units
    sigma = max(sigma, 1e-4);
    float h = exp(-0.5 * (Z / sigma) * (Z / sigma));
    return h * ridgeAmp;
}

// ─── Iron-filings dipole field (mood 2) ──────────────────────────────
float dipoleStream(vec2 p, float t) {
    vec2 a = vec2(0.30, 0.50) + 0.04 * vec2(sin(t * 0.21), cos(t * 0.17));
    vec2 b = vec2(0.70, 0.50) + 0.04 * vec2(cos(t * 0.19), sin(t * 0.23));
    vec2 da = p - a, db = p - b;
    vec2 Bv = da / (dot(da, da) + 1e-3) - db / (dot(db, db) + 1e-3);
    float ang = atan(Bv.y, Bv.x);
    return 0.5 + 0.5 * cos(ang * 7.0 + 12.0 * (p.x + p.y));
}

// ─── Slow Lissajous plate drift ───────────────────────────────────────
//  Two incommensurate frequencies produce a never-repeating gentle pan.
//  Amplitude is scaled by driftAmount so the user can kill it entirely.
vec2 plateDrift(float t, float amount, float speed) {
    float s = speed * 0.08;  // keep it genuinely slow
    float dx = sin(t * s * 1.000 + 0.0)   * 0.018
             + sin(t * s * 0.618 + 1.1)   * 0.011;
    float dy = sin(t * s * 0.857 + 2.4)   * 0.015
             + sin(t * s * 1.272 + 0.7)   * 0.009;
    return vec2(dx, dy) * amount;
}

void main() {
    vec2 uv  = isf_FragNormCoord.xy;
    vec2 ndc = uv * 2.0 - 1.0;

    float t      = TIME;
    float audio  = clamp(audioReact, 0.0, 2.0);
    float bass   = clamp(audioBass   * audio, 0.0, 1.5);
    float mid    = clamp(audioMid    * audio, 0.0, 1.5);
    float treb   = clamp(audioHigh   * audio, 0.0, 1.5);
    float level  = clamp(audioLevel  * audio, 0.0, 1.5);

    // ── mode-pair morph ───────────────────────────────────────────────
    float period   = max(4.0, morphPeriod);
    float baseIdx  = t / period;
    float bassKick = smoothstep(0.55, 0.95, bass);
    float idxF     = baseIdx + 1.5 * bassKick + 0.05 * sin(t * 0.07);

    int   ia = int(mod(floor(idxF),       12.0));
    int   ib = int(mod(floor(idxF) + 1.0, 12.0));
    float u  = smoothstep(0.0, 1.0, fract(idxF));

    vec2 nmA = modePair(ia);
    vec2 nmB = modePair(ib);
    vec2 nm  = mix(nmA, nmB, u);

    // ── aspect-corrected plate coords ────────────────────────────────
    float ar = RENDERSIZE.x / RENDERSIZE.y;
    vec2  p  = uv;
    if (ar > 1.0) p.x = (uv.x - 0.5) * ar + 0.5;
    else          p.y = (uv.y - 0.5) / ar + 0.5;

    // ── slow plate drift ─────────────────────────────────────────────
    p += plateDrift(t, driftAmount * 0.06, driftSpeed);

    // ── agitation jitter ─────────────────────────────────────────────
    float agitate = clamp((level - settle * 0.6) * 1.3, 0.0, 1.0);
    vec2 jit = (vec2(hash21(p * RENDERSIZE.xy),
                     hash21(p * RENDERSIZE.xy + 17.3)) - 0.5);
    p += jit * (0.0015 + 0.010 * agitate);

    // ── field evaluation ─────────────────────────────────────────────
    float widthPx = lineWidth * (0.85 + 0.6 * mid);
    float Z       = chladni(p, nm);
    float aaw     = max(fwidth(Z), 1e-5);
    float d       = abs(Z) / aaw;
    float line    = 1.0 - smoothstep(widthPx * 0.5, widthPx * 0.5 + 1.2, d);
    float halo    = (1.0 - smoothstep(widthPx * 0.5 * 3.5,
                                      widthPx * 0.5 * 3.5 + 1.2, d)) - line;

    // ── surface normal from ridge height field ────────────────────────
    //  Height = Gaussian bump over |Z|=0, amplitude ~0.35 px equivalent.
    float ridgeAmp = 0.35;
    float h0 = ridgeHeight(Z, aaw, widthPx, ridgeAmp);

    // Analytic normal: N = normalize( -dH/dx, -dH/dy, 1 )
    // dH/dZ = -Z/sigma^2 * H  (Gaussian derivative)
    // dH/dp = dH/dZ * dZ/dp
    float sigma = widthPx * 0.5 * aaw;
    sigma = max(sigma, 1e-4);
    float dHdZ  = -(Z / (sigma * sigma)) * h0;
    vec2  gradZ = chladniGrad(p, nm);
    // Scale gradient from normalised [0,1] space to "pixel" scale so the
    // normal magnitude is meaningful relative to ridgeAmp.
    float pixScale = 1.0 / max(RENDERSIZE.x, RENDERSIZE.y);
    vec2  dHdp  = dHdZ * gradZ * pixScale;
    // Tangent-space normal (pointing into viewer is +Z)
    vec3  N     = normalize(vec3(-dHdp.x, -dHdp.y, 1.0));

    // ── directional light ─────────────────────────────────────────────
    float el = clamp(lightElevation, 0.05, 1.0);
    vec3 Ldir = normalize(vec3(cos(lightAngle) * cos(el * PI * 0.5),
                               sin(lightAngle) * cos(el * PI * 0.5),
                               sin(el * PI * 0.5)));
    float diff = clamp(dot(N, Ldir), 0.0, 1.0);

    // Blinn-Phong specular
    vec3  V    = vec3(0.0, 0.0, 1.0);           // orthographic viewer
    vec3  H    = normalize(Ldir + V);
    float spec = pow(clamp(dot(N, H), 0.0, 1.0), glossiness);

    // Soft self-shadow: sample the field slightly offset along –Ldir
    // in texture space; if that neighbour is lower on the ridge, we're
    // in shadow.
    float shadowStep = 0.003 + 0.004 * (1.0 - el);
    vec2  pShadow    = p - vec2(Ldir.x, Ldir.y) * shadowStep;
    float Zshadow    = chladni(pShadow, nm);
    float hShadow    = ridgeHeight(Zshadow, aaw, widthPx, ridgeAmp);
    // If the shadow sample is higher than current height we are occluded.
    float shadow     = 1.0 - clamp((hShadow - h0) * 6.0, 0.0, 0.65);

    float diffLit  = (0.25 + 0.75 * diff * shadow) * lightIntensity;
    float specLit  = spec * shadow * lightIntensity;

    // ── per-mood base colour ──────────────────────────────────────────
    int moodI = int(mood + 0.5);
    vec3 col;

    if (moodI == 1) {
        // ─ Water cymatics ─────────────────────────────────────────────
        vec3 water1 = vec3(0.04, 0.10, 0.16);
        vec3 water2 = vec3(0.02, 0.05, 0.10);
        float ripple = 0.5 + 0.5 * sin(40.0 * (p.x + p.y) + t * 1.4);
        vec3 base    = mix(water2, water1, 0.5 + 0.4 * ripple);
        base = mix(base, bgColor.rgb, bgColor.a);   // user background = the water bed
        vec3 cau     = vec3(0.85, 0.95, 1.05);

        col  = base;
        col += cau * line  * (0.9 + 1.2 * mid) * diffLit;
        col += cau * 0.35  * halo * diffLit;
        col += vec3(1.0, 0.9, 0.7) * line * bassKick * 0.6;

        // Specular glint on water caustics (cool tint)
        col += vec3(0.7, 0.9, 1.1) * specLit * line * 0.9;

    } else if (moodI == 2) {
        // ─ Iron filings + dipole ──────────────────────────────────────
        vec3 plate = vec3(0.10, 0.09, 0.08);
        plate = mix(plate, bgColor.rgb, bgColor.a); // user background = the plate
        float dip  = dipoleStream(p, t);
        float dipL = smoothstep(0.55, 0.92, dip);
        float combined = max(line * 0.85, dipL * (0.55 + 0.35 * line));
        vec3 iron  = vec3(0.55, 0.50, 0.46);

        // Apply diffuse lighting so ridges catch side-light
        col = mix(plate, iron * diffLit, combined * (0.7 + 0.5 * mid));
        // Self-shadow darkens the base plate under ridges
        col *= mix(1.0, 0.72, (1.0 - shadow) * combined);
        col += vec3(0.9, 0.85, 0.78) * combined * bassKick * 0.4;
        // Metal filing specular highlight
        col += vec3(1.0, 0.95, 0.88) * specLit * combined * 0.7;

    } else {
        // ─ Sand on plate (default) ────────────────────────────────────
        vec3 plate = vec3(0.18, 0.16, 0.14);
        plate = mix(plate, bgColor.rgb, bgColor.a); // user background = the plate
        vec3 sandWarm = vec3(0.94, 0.88, 0.72);
        vec3 sandCool = vec3(0.86, 0.86, 0.84);
        vec3 sand     = mix(sandCool, sandWarm, clamp(warmth, 0.0, 1.0));

        // Brushed plate grain
        float brushed = 0.5 + 0.5 * sin(p.y * 540.0 + hash21(p) * 6.28);
        plate += vec3(0.018) * (brushed - 0.5);

        // Self-shadow on the plate under sand ridges
        float plateShad = mix(1.0, 0.60, (1.0 - shadow) * line);
        plate *= plateShad;

        float brightness = 0.85 + 1.45 * mid + 0.25 * level;

        // Diffuse-lit sand
        vec3 sandLit = sand * brightness * diffLit;

        col  = plate;
        col  = mix(col, sandLit, line);
        col += sand * halo * (0.18 + 0.35 * mid) * diffLit;

        // Specular: warm highlight on sand peaks (slightly golden)
        vec3 specColor = mix(vec3(1.0, 0.97, 0.88),
                             vec3(1.0, 0.90, 0.70),
                             clamp(warmth, 0.0, 1.0));
        col += specColor * specLit * line * 0.55;
    }

    // ── treble grains ─────────────────────────────────────────────────
    {
        vec2  gp      = floor(uv * RENDERSIZE.xy / 1.6);
        float h1      = hash21(gp);
        float h2      = hash21(gp + 7.7);
        float h3      = hash21(gp + 13.3);
        float gateLine = nodalLine(p, nm, widthPx * 2.2);
        float gateRand = step(1.0 - 0.55 * grainDensity * (0.4 + treb), h1);
        float twinkle  = step(0.55, fract(h2 + t * (0.7 + 1.2 * treb) + h3));
        float grain    = gateLine * gateRand * twinkle;
        vec3 grainCol  = vec3(1.0, 0.95, 0.82);
        // Grain inherits the light direction: grains on the lit side pop
        col += grainCol * grain * (0.6 + 1.4 * treb) * (0.6 + 0.4 * diff);
    }

    // ── standing-wave shimmer ──────────────────────────────────────────
    float shimmer = 0.5 + 0.5 * sin(t * 0.6 + (nm.x + nm.y) * 0.3);
    col *= 0.96 + 0.06 * shimmer;

    // ── soft vignette + film grain ────────────────────────────────────
    col *= 1.0 - 0.22 * dot(ndc * 0.5, ndc * 0.5);
    col += (hash21(uv * RENDERSIZE.xy + t) - 0.5) * 0.008;

    // ── mild HDR boost on bright sand ─────────────────────────────────
    float lum = dot(col, vec3(0.2126, 0.7152, 0.0722));
    col += col * smoothstep(0.85, 1.6, lum) * 0.6;

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
    col = uc;

    gl_FragColor = vec4(col, 1.0);
}