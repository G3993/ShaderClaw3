/*{
  "DESCRIPTION": "Neon Growth — neon coral/lichen colonies growing over a scratchy ink skeleton on tan paper. stemMelody (+ its Presence) feeds continuous colony growth via a domain-warped field advected on the audioMidTime clock; stemDrums spawns spore pops that ride audioPhase2 ramps; treble makes the lavender outline glow breathe. Colonies are lit as puffy 3D matter (gradient lighting + cast shadow). No text.",
  "CREDIT": "ShaderClaw",
  "CATEGORIES": [
    "Generator",
    "Audio"
  ],
  "INPUTS": [
    {
      "NAME": "glowAmount",
      "LABEL": "Outline Glow",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2
    },
    {
      "NAME": "inkAmount",
      "LABEL": "Ink Skeleton",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "GROUP": "Shape / Geometry"
    },
    {
      "NAME": "colonyGrowth",
      "LABEL": "Colony Growth",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "sporeAmount",
      "LABEL": "Spore Pops",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "GROUP": "Motion / Animation"
    },
    {
      "NAME": "hueShift",
      "LABEL": "Hue Shift",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1,
      "GROUP": "Color"
    },
    {
      "NAME": "colorBoost",
      "LABEL": "Color Boost",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "GROUP": "Color"
    },
    {
      "NAME": "bgColor",
      "LABEL": "Background",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        0,
        0
      ],
      "GROUP": "Background"
    },
    {
      "NAME": "audioReactivity",
      "LABEL": "Audio Reactivity",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 2,
      "GROUP": "Audio Reactivity"
    }
  ]
}*/

// ============================================================================
// NEON GROWTH — A-List VFX #2 (ref: growingfungi_.jpg)
//   Four independent layers, back to front:
//     1. ink skeleton  — clustered scratchy cross-hatch strokes on tan paper
//     2. colonies      — coral/orange/lime blobs from a domain-warped fbm
//                        field; the threshold breathes with stemMelody
//                        (LINEAR follower), the field advects on the
//                        integrated audioMidTime clock, and the boundary
//                        crawls on the same clock (never raw->position)
//     3. spore pops    — staggered per-cell pops on audioPhase2 ramps, gated
//                        by a soft-kneed stemDrums envelope (event path)
//     4. outline glow  — pale lavender halo around every colony boundary,
//                        width + brightness breathing with treble
//   3D: colony field is treated as a heightmap — gradient lighting from the
//   upper-left, specular hint, dark contact rim, and a cast shadow onto the
//   paper below. Silence: field drifts on TIME, boundary sways, spores drip.
// ============================================================================

#define TAU 6.2831853

float hash21(vec2 p){
  p = fract(p * vec2(123.34, 456.21));
  p += dot(p, p + 45.32);
  return fract(p.x * p.y);
}

float vnoise(vec2 p){
  vec2 i = floor(p), f = fract(p);
  vec2 u = f * f * (3.0 - 2.0 * f);
  float a = hash21(i);
  float b = hash21(i + vec2(1.0, 0.0));
  float c = hash21(i + vec2(0.0, 1.0));
  float d = hash21(i + vec2(1.0, 1.0));
  return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm3(vec2 p){
  float v = 0.0, a = 0.5;
  for (int i = 0; i < 3; i++){
    v += a * vnoise(p);
    p = p * 2.03 + vec2(17.3, 9.1);
    a *= 0.5;
  }
  return v * 1.1428; // normalize to ~0..1
}

// domain-warped colony field — t1 is an integrated clock (TIME + audioMidTime)
float colonyField(vec2 p, float t1){
  vec2 q = p + 1.0 * vec2(fbm3(p * 1.9 + vec2(0.0, t1)) - 0.5,
                          fbm3(p * 1.9 + vec2(5.2, -t1 * 0.8)) - 0.5);
  return fbm3(q * 2.6 + vec2(t1 * 0.35, 1.7));
}

vec3 hueRotate(vec3 c, float a){
  float an = a * TAU;
  vec3 k = vec3(0.57735);
  float cs = cos(an), sn = sin(an);
  return c * cs + cross(k, c) * sn + k * dot(k, c) * (1.0 - cs);
}

void main(){
  vec2 uv = isf_FragNormCoord;
  vec2 p = uv - 0.5;
  p.x *= RENDERSIZE.x / RENDERSIZE.y;
  float react = audioReactivity;
  float px = 1.0 / RENDERSIZE.y;   // 1 pixel in uv units — all hard edges scale off this
  float pw = px * 0.75;            // vnoise slope ≈ 0.75/cell → field units per pixel per unit freq

  // --- conditioned audio drivers ---------------------------------------------
  // continuous paths: LINEAR followers (playbook keeps knees for event paths)
  float melL  = clamp(0.50 * stemMelody + 0.60 * stemMelodyPresence + 0.30 * audioMid, 0.0, 1.3) * react;
  float highL = clamp(0.55 * audioHigh + 0.55 * stemAir, 0.0, 1.3) * react;
  float bassL = clamp(0.70 * audioBass + 0.50 * stemBass, 0.0, 1.3) * react;
  float lvlL  = clamp(audioLevel, 0.0, 1.0) * react;
  // event path: soft knee on the drum stem envelope
  float drumK = smoothstep(0.04, 0.75, stemDrums) * react;

  // integrated growth clocks — drift in silence, advance with melody energy
  float t1 = TIME * 0.055 + audioMidTime * 0.10;
  float tCrawl = TIME * 0.7 + audioMidTime * 2.4;

  // --- layer 0: tan paper ------------------------------------------------------
  vec3 paper = (bgColor.a > 0.004) ? bgColor.rgb : vec3(0.760, 0.700, 0.585);
  // paper tooth: crisp static stipple (binarized grain, ~1px edges at eval scale)
  paper *= 0.895 + 0.21 * smoothstep(0.5 - 10.0 * px, 0.5 + 10.0 * px, vnoise(p * 90.0));
  paper *= 1.0 - 0.60 * dot(uv - 0.5, uv - 0.5);              // vignette

  // --- layer 1: scratchy ink skeleton -------------------------------------------
  // pixel-width strokes: ~0.5px solid core, crisp by 1.8px — the old fixed
  // field-unit bands went sub-pixel at full screen and aliased into gray mush
  float mask = smoothstep(0.40, 0.60, fbm3(p * 1.25 + vec2(41.3, 17.9)));
  float l1 = 1.0 - smoothstep(26.0 * pw * 0.6, 26.0 * pw * 1.5, abs(vnoise(p * vec2(26.0, 6.0) + 3.1) - 0.5));
  float l2 = 1.0 - smoothstep(28.0 * pw * 0.6, 28.0 * pw * 1.5, abs(vnoise(p * vec2(6.0, 28.0) + 9.7) - 0.5));
  vec2 pr = mat2(0.707, -0.707, 0.707, 0.707) * p;
  float l3 = 1.0 - smoothstep(20.0 * pw * 0.6, 20.0 * pw * 1.5, abs(vnoise(pr * vec2(20.0, 7.0) + 6.3) - 0.5));
  float l4 = 1.0 - smoothstep(44.0 * pw * 0.5, 44.0 * pw * 1.3, abs(vnoise(p * vec2(44.0, 12.0) + 14.9) - 0.5));
  float ink = mask * clamp(l1 + 0.9 * l2 + 0.85 * l3 + 0.7 * l4, 0.0, 1.0);
  ink = clamp(ink * inkAmount, 0.0, 1.0);
  vec3 col = mix(paper, vec3(0.05, 0.04, 0.05), ink * 0.94);

  // pale speckle dots scattered on the paper (reference dots)
  vec2 spCell = floor(p * 70.0);
  float spDot = step(0.945, hash21(spCell + 0.7))
              * smoothstep(0.30, 0.30 - 1.6 * 70.0 * px, length(fract(p * 70.0) - 0.5));
  col = mix(col, vec3(0.92, 0.90, 1.00), spDot * 0.8);

  // faint pencil under-hatch across the open paper — the sketch layer the
  // colonies grow over; crisp pixel-width strokes, light gray so it stays "under"
  float l5 = 1.0 - smoothstep(36.0 * pw * 0.5, 36.0 * pw * 1.3,
                              abs(vnoise(pr * vec2(36.0, 9.0) + 21.7) - 0.5));
  col = mix(col, vec3(0.24, 0.20, 0.22), (1.0 - mask) * l5 * clamp(inkAmount, 0.0, 1.0) * 0.64);

  // --- layer 2: neon colonies ----------------------------------------------------
  float F = colonyField(p, t1);
  // growth: melody expands the boundary (LINEAR); slow aperiodic idle breath;
  // boundary crawl rides the integrated mid clock so its SPEED follows music
  float thr = 0.560 - 0.105 * colonyGrowth * melL - 0.075 * lvlL
            - 0.010 * sin(TIME * 0.31) - 0.008 * sin(TIME * 0.53 + 1.7)
            + 0.014 * sin(p.x * 21.0 + p.y * 17.0 + tCrawl);
  float s = F - thr;
  float body = smoothstep(0.0, 0.010, s);

  // cast shadow onto paper (light from upper-left)
  float Fs = colonyField(p + vec2(-0.035, 0.05), t1);
  float shad = smoothstep(thr, thr + 0.05, Fs) * (1.0 - body) * 0.25;
  col *= 1.0 - shad;

  // dark contact rim hugging the outside of the boundary (inked edge) —
  // pixel-tight: colony field slope ≈ 1.8/uv, so 1.5px rim = 2.7*px field units
  float rim = exp(-abs(s + 0.020) / (2.7 * px)) * (1.0 - body);
  col = mix(col, vec3(0.07, 0.05, 0.08), rim * 0.80);

  // satellite lime-green colonies from a second field read (reference: green
  // patches scattered around the orange growth, breathing with melody too)
  float g = fbm3(p * 2.8 + vec2(13.1, 7.7) + t1 * 0.25);
  float thrG = 0.660 - 0.045 * colonyGrowth * melL;
  // crisp lime boundary (~2px; field slope ≈ 2/uv) with a tight dark contact rim
  float gBody = smoothstep(thrG, thrG + 4.0 * px, g) * (1.0 - body);
  vec3 limeC = vec3(0.62, 0.86, 0.08) * (1.0 + 0.25 * melL);
  col = mix(col, vec3(0.10, 0.08, 0.06), exp(-abs(g - thrG) / (3.0 * px)) * (1.0 - body) * (1.0 - gBody) * 0.55);
  col = mix(col, limeC, gBody * 0.95);
  col = mix(col, vec3(0.83, 0.80, 0.97),
            exp(-abs(g - thrG) / (4.5 * px)) * (1.0 - body) * glowAmount * (0.12 + 0.30 * highL));

  if (s > -0.20){
    float tt = smoothstep(0.0, 0.24, s);
    vec3 cc = mix(vec3(1.00, 0.30, 0.26), vec3(1.00, 0.55, 0.06), tt);
    cc = mix(cc, vec3(1.00, 0.83, 0.15), smoothstep(0.75, 1.0, tt));
    cc = mix(cc, vec3(0.62, 0.86, 0.08), smoothstep(0.60, 0.72, g) * 0.9);
    // pores: dark red-brown holes speckling the colony interior
    float pores = smoothstep(0.54, 0.60, vnoise(p * 34.0 + 3.7));
    cc = mix(cc, vec3(0.42, 0.09, 0.05), pores * (0.45 + 0.55 * tt));
    // puffy 3D lighting from the field gradient
    float e = 0.016;
    float Fx = colonyField(p + vec2(e, 0.0), t1) - F;
    float Fy = colonyField(p + vec2(0.0, e), t1) - F;
    float lit = clamp((Fx * (-0.6) + Fy * 0.8) / e, -1.0, 1.0);
    cc *= 1.0 + 0.28 * lit;
    cc += vec3(1.0, 0.9, 0.7) * pow(max(lit, 0.0), 3.0) * 0.25;
    // colony luminance breathes with melody + bass (LINEAR; exactly 1.0 in silence)
    cc *= 1.0 + 0.18 * melL + 0.14 * bassL;
    col = mix(col, cc, body);
  }

  // --- layer 4: lavender outline glow (treble width + brightness) -----------------
  // neon tube read: soft breathing halo + a crisp ~1px bright core line hugging
  // the colony boundary (colony field slope ≈ 1.8/uv → |s| in px ≈ |s|/(1.8*px))
  float halo = exp(-abs(s) * (38.0 - 16.0 * clamp(highL, 0.0, 1.0)));
  col = mix(col, vec3(0.80, 0.76, 1.00), halo * glowAmount * (0.25 + 0.60 * highL));
  float tubeCore = smoothstep(5.2 * px, 1.6 * px, abs(s));
  col = mix(col, vec3(0.90, 0.87, 1.00), tubeCore * min(glowAmount, 1.0) * (0.85 + 0.15 * highL));
  float tubeCoreG = smoothstep(5.2 * px, 1.6 * px, abs(g - thrG)) * (1.0 - body);
  col = mix(col, vec3(0.86, 0.90, 0.78), tubeCoreG * min(glowAmount, 1.0) * (0.50 + 0.30 * highL));

  // --- layer 3: spore pops (drums) --------------------------------------------------
  vec2 sp = p * 6.5;
  vec2 cell = floor(sp);
  float rnd = hash21(cell);
  if (rnd > 0.45){
    vec2 ctr = cell + 0.5 + (vec2(hash21(cell + 11.1), hash21(cell + 27.7)) - 0.5) * 0.7;
    float r = length(sp - ctr);
    // staggered pops: beat-phase ramp + TIME fallback so silence still drips
    float age = fract(audioPhase2 + TIME * 0.13 + rnd * 7.0);
    float R = mix(0.06, 0.38, age);
    float alpha = pow(1.0 - age, 1.8);
    float pxs = 6.5 * px; // 1 pixel in spore-cell units
    float dsk = smoothstep(R, R - 1.6 * pxs, r);                     // sharp disk core
    float ring = smoothstep(1.4 * pxs, 0.2 * pxs, abs(r - R)) * 0.75 // crisp 1px rim
               + smoothstep(0.16, 0.0, abs(r - R)) * 0.18;           // soft halo kept
    float hue = hash21(cell + 5.5);
    vec3 scol = hue < 0.4 ? vec3(1.00, 0.35, 0.25)
              : (hue < 0.75 ? vec3(0.65, 0.88, 0.10) : vec3(0.95, 0.92, 0.85));
    // ink-drawn outline just outside the spore disk (crisp dark circle)
    float ringD = smoothstep(1.6 * pxs, 0.4 * pxs, abs(r - R - 1.2 * pxs));
    col = mix(col, vec3(0.08, 0.06, 0.08),
              ringD * alpha * (0.62 + 0.30 * drumK) * sporeAmount);
    float sInt = (dsk + ring) * alpha * (0.50 + 0.50 * drumK) * sporeAmount;
    col = mix(col, scol, clamp(sInt, 0.0, 1.0) * 0.85);
  }

  // whole-frame LINEAR follower — darken-dip (scene is bright; dips can't clip
  // and frameDiff-correlate exactly as well as gains). Multiplies to 1 in silence.
  col *= 1.0 - clamp(react * (0.20 * audioBass + 0.10 * audioMid), 0.0, 0.40);

  // universal grading
  col = hueRotate(col, hueShift);
  float l = dot(col, vec3(0.299, 0.587, 0.114));
  col = mix(vec3(l), col, colorBoost);

  gl_FragColor = vec4(col, 1.0);
}
