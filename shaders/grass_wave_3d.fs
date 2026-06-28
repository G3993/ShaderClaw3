/*{
  "DESCRIPTION":"Genuinely-3D procedural grass field with user-controllable wave-warp wind. Perspective ground recedes to a horizon, blades draw back-to-front (painter's algorithm), wind sway is a directional wave you steer (amount/frequency/speed/angle/cross), audio bass adds gusts and treble shimmers the tips. Lush by default with no image; optional image as sky backdrop or whole-field tint.",
  "CREDIT":"ShaderClaw3",
  "CATEGORIES":["Generator","3D","Nature","Audio Reactive"],
  "INPUTS":[
    { "NAME":"inputImage", "TYPE":"image" },
    { "NAME":"audioReact", "LABEL":"Sound Reactivity", "TYPE":"float", "DEFAULT":1.0, "MIN":0.0, "MAX":2.0 },

    { "NAME":"waveAmp",     "LABEL":"Wave Amount",    "TYPE":"float", "DEFAULT":0.5,  "MIN":0.0, "MAX":2.0 },
    { "NAME":"waveFreq",    "LABEL":"Wave Frequency", "TYPE":"float", "DEFAULT":0.9,  "MIN":0.1, "MAX":4.0 },
    { "NAME":"waveSpeed",   "LABEL":"Wave Speed",     "TYPE":"float", "DEFAULT":0.3,  "MIN":0.0, "MAX":2.0 },
    { "NAME":"waveAngle",   "LABEL":"Wave Direction", "TYPE":"float", "DEFAULT":0.12, "MIN":0.0, "MAX":1.0 },
    { "NAME":"crossAmt",    "LABEL":"Cross Wave",     "TYPE":"float", "DEFAULT":0.35, "MIN":0.0, "MAX":1.0 },

    { "NAME":"density",     "LABEL":"Density",        "TYPE":"float", "DEFAULT":0.75, "MIN":0.2, "MAX":1.0 },
    { "NAME":"grassHeight", "LABEL":"Blade Height",   "TYPE":"float", "DEFAULT":1.0,  "MIN":0.4, "MAX":2.0 },
    { "NAME":"sunHeight",   "LABEL":"Sun Height",     "TYPE":"float", "DEFAULT":0.55, "MIN":0.0, "MAX":1.0 },

    { "NAME":"baseColor", "LABEL":"Root Color", "TYPE":"color", "DEFAULT":[0.04,0.18,0.05,1.0] },
    { "NAME":"tipColor",  "LABEL":"Tip Color",  "TYPE":"color", "DEFAULT":[0.55,0.80,0.18,1.0] },
    { "NAME":"skyColor",  "LABEL":"Sky Color",  "TYPE":"color", "DEFAULT":[0.18,0.42,0.72,1.0] },

    { "NAME":"texMix", "LABEL":"Image Mix", "TYPE":"float", "DEFAULT":0.0, "MIN":0.0, "MAX":1.0 }
  ]
}*/

// ----- helpers -----
float hash(vec2 p){ return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
mat2 rot(float a){ float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

const float TAU = 6.28318530718;

// distance from point p to quadratic bezier (a -> control b -> c), screen space
// approximate by sampling segments (constant-capped loop)
float bezDist(vec2 p, vec2 a, vec2 b, vec2 c, out float tOut){
  float best = 1e9;
  float bt = 0.0;
  const int N = 10;
  vec2 prev = a;
  for(int i = 1; i <= N; i++){
    float t = float(i) / float(N);
    float u = 1.0 - t;
    vec2 cur = u*u*a + 2.0*u*t*b + t*t*c;
    // distance to segment prev->cur
    vec2 pa = p - prev;
    vec2 ba = cur - prev;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
    float d = length(pa - ba*h);
    if(d < best){ best = d; bt = (float(i-1) + h) / float(N); }
    prev = cur;
  }
  tOut = bt;
  return best;
}

void main() {
  float bass   = audioBass * audioReact;
  float mid    = audioMid  * audioReact;
  float treble = audioHigh * audioReact;

  vec2 uv = gl_FragCoord.xy / RENDERSIZE;
  float aspect = RENDERSIZE.x / RENDERSIZE.y;

  float horizonY = 0.55;
  vec2 sunDir = normalize(vec2(0.35, sunHeight * 0.9 + 0.15));

  // ---------- SKY ----------
  vec3 skyTop = skyColor.rgb * 0.85;
  vec3 skyHaze = mix(skyColor.rgb, vec3(0.92, 0.94, 0.88), 0.7);
  float skyT = clamp((uv.y - horizonY) / (1.0 - horizonY), 0.0, 1.0);
  vec3 sky = mix(skyHaze, skyTop, pow(skyT, 0.8));
  // soft sun
  vec2 sunPos = vec2(0.5 + sunDir.x * 0.35, horizonY + sunDir.y * 0.4);
  vec2 sd = (uv - sunPos);
  sd.x *= aspect;
  float sunGlow = exp(-dot(sd, sd) * 18.0);
  sky += vec3(1.0, 0.92, 0.7) * sunGlow * 0.9;
  sky += vec3(1.0, 0.85, 0.6) * exp(-length(sd) * 4.0) * 0.25;

  // ---------- GROUND base + perspective ----------
  float eps = 0.012;
  float camH = 0.16;
  // depth: large near bottom, -> infinity near horizon
  float depth = camH / max(horizonY - uv.y, eps);
  // world coords for ground texture
  float worldX = (uv.x - 0.5) * aspect * depth * 6.0;
  float worldZ = depth;

  // earthy green ground with subtle depth noise
  float gnoise = hash(floor(vec2(worldX * 1.3, worldZ * 1.3)));
  vec3 ground = mix(vec3(0.06, 0.13, 0.04), vec3(0.10, 0.20, 0.07), gnoise);
  ground = mix(ground, baseColor.rgb, 0.4);
  // depth fog toward horizon haze
  float fog = clamp(depth / 8.0, 0.0, 1.0);
  ground = mix(ground, skyHaze, fog * 0.85);

  bool isSky = uv.y > horizonY;
  vec3 col = isSky ? sky : ground;

  // ---------- GRASS BLADES (back-to-front) ----------
  // wind direction
  float ang = waveAngle * TAU;
  mat2 windRot = rot(ang);
  vec2 windAxis = vec2(cos(ang), sin(ang));

  // gust from bass (K<=0.6 on amplitude)
  float gust = 1.0 + bass * 0.6;
  float ampG = waveAmp * gust;

  float px = gl_FragCoord.x;
  float fragPxSize = 1.0 / RENDERSIZE.y;

  // accumulate nearest covering blade via depth (smaller screenDepth = nearer)
  vec3 bladeCol = vec3(0.0);
  float bladeAlpha = 0.0;
  float bestNear = 1e9; // smaller = nearer (use worldDepth)

  // iterate rows far -> near. Far rows have small depth-step; we march depth.
  const int ROWS = 46;
  int bladesPerRow = int(mix(2.0, 6.0, density) + 0.5);

  for(int r = 0; r < ROWS; r++){
    // far -> near : row 0 = far (near horizon), last = near (bottom)
    float rf = float(r) / float(ROWS - 1);
    // distribute depth non-linearly so far rows pack near horizon
    float d = mix(7.5, camH / (horizonY - eps), 0.0); // placeholder
    // depth from near(large) to far(small): map rf so r=0 -> far(small d)
    float dDepth = mix(6.5, 0.45, rf); // r=0 far (6.5), last near (0.45)

    for(int b = 0; b < 6; b++){
      if(b >= bladesPerRow) break;

      // jittered grid cell hash
      float cellId = float(r) * 13.0 + float(b) * 7.0;
      float h1 = hash(vec2(cellId, 1.0));
      float h2 = hash(vec2(cellId, 2.0));
      float h3 = hash(vec2(cellId, 3.0));
      float h4 = hash(vec2(cellId, 4.0));
      float h5 = hash(vec2(cellId, 5.0));

      // base world position
      float bz = dDepth * (0.92 + h1 * 0.16);
      float bx = (h2 - 0.5) * aspect * bz * 6.0 * (0.8 + 0.4*h3);

      // project base to screen
      // invert: depth = camH/(horizonY - sy)  => sy = horizonY - camH/bz
      float bsy = horizonY - camH / bz;
      if(bsy >= horizonY) continue; // above horizon, skip
      float bsx = 0.5 + (bx / (bz * 6.0 * aspect)) ;

      if(bsx < -0.1 || bsx > 1.1) continue;

      // nearness scale (near = big). reference scale by 1/bz
      float nearScale = clamp(0.18 / bz, 0.0, 1.5);

      // per-blade attributes
      float phase = h4 * TAU;
      float bh = (0.55 + h5 * 0.6) * grassHeight; // blade height factor
      float bw = (0.6 + h1 * 0.7);                // blade width factor
      float hue = (h3 - 0.5);                     // hue jitter
      float lean = (h2 - 0.5) * 0.5;

      // ---- WAVE WARP (the controllable wind) ----
      // rotate world coords into wind frame
      vec2 wpos = windRot * vec2(bx * 0.02, bz);
      float sway =
          sin(wpos.y * waveFreq + TIME * waveSpeed + phase + wpos.x * 0.2)
        + crossAmt * sin(wpos.x * waveFreq * 0.7 - TIME * waveSpeed * 0.6 + phase * 0.5);
      float tipOffWorld = ampG * sway + lean;

      // screen-space sizes
      float bladeHeightScreen = bh * nearScale * 0.9;
      float tipOffScreen = (tipOffWorld * windAxis.x) * nearScale * 0.25;
      float widthScreen = bw * nearScale * 0.020 + 0.0008;

      // bezier in screen space (y up = +): base bsy, tip higher
      vec2 A = vec2(bsx, bsy);
      vec2 Tp = vec2(bsx + tipOffScreen, bsy + bladeHeightScreen);
      vec2 Ctrl = vec2(bsx + tipOffScreen * 0.45, bsy + bladeHeightScreen * 0.55);

      // distance from current pixel (uv corrected for aspect on x)
      vec2 p = vec2((uv.x) , uv.y);
      // adjust x distance by aspect so width is uniform
      vec2 Aa = A, Cc = Ctrl, TT = Tp;
      float tHit;
      // compute distances in aspect-corrected space
      vec2 pa = vec2(p.x * aspect, p.y);
      vec2 A2 = vec2(Aa.x * aspect, Aa.y);
      vec2 C2 = vec2(Cc.x * aspect, Cc.y);
      vec2 T2 = vec2(TT.x * aspect, TT.y);
      float dist = bezDist(pa, A2, C2, T2, tHit);

      // width tapers root->tip
      float wTaper = mix(1.0, 0.12, tHit);
      float halfW = widthScreen * wTaper * aspect;

      float edge = fragPxSize * 1.5 + halfW * 0.001;
      float cover = smoothstep(halfW + edge, halfW - edge, dist);
      if(cover <= 0.001) continue;

      // depth test: nearer (smaller bz) wins
      if(bz < bestNear || bladeAlpha < 0.001){
        // ---- shading ----
        // fake normal: side of spine
        float side = sign((pa.x) - mix(A2.x, T2.x, tHit));
        float lamb = clamp(0.45 + 0.55 * side * windAxis.x * 0.0 + 0.5 + 0.4 * dot(normalize(vec2(side, 1.5)), sunDir), 0.2, 1.2);

        vec3 root = baseColor.rgb;
        vec3 tip = tipColor.rgb;
        // hue jitter shift
        tip += vec3(hue * 0.12, hue * 0.08, -hue * 0.1);
        vec3 bcol = mix(root, tip, pow(tHit, 0.85));

        // AO darker near root, brighter at tips
        float ao = mix(0.55, 1.15, tHit);
        bcol *= ao * lamb;

        // treble tip shimmer (K<=0.6)
        float shimmer = treble * 0.6 * pow(tHit, 3.0) * (0.5 + 0.5 * sin(TIME * 6.0 + phase + bsx * 20.0));
        bcol += vec3(0.9, 1.0, 0.6) * shimmer * 0.25;

        // depth fog on far blades
        float bfog = clamp(bz / 7.0, 0.0, 1.0);
        bcol = mix(bcol, skyHaze, bfog * 0.7);

        // accumulate (front-most replaces)
        float a = cover;
        bladeCol = mix(bladeCol, bcol, a * (bz <= bestNear ? 1.0 : 0.0) + a * (bladeAlpha < 0.001 ? 1.0 : 0.0));
        bladeCol = mix(bladeCol, bcol, a);
        bladeAlpha = max(bladeAlpha, a);
        bestNear = min(bestNear, bz);
      }
    }
  }

  // composite blades over ground/sky
  col = mix(col, bladeCol, bladeAlpha);

  // ---------- optional image (sky backdrop OR field tint) ----------
  if (texMix > 0.0){
    // aspect-correct sample of inputImage by screen uv
    vec2 iuv = uv;
    float imgAspect = IMG_SIZE_inputImage.x / max(IMG_SIZE_inputImage.y, 1.0);
    float scrAspect = aspect;
    // cover-fit
    vec2 iu = iuv - 0.5;
    if(imgAspect > scrAspect){ iu.x *= scrAspect / imgAspect; }
    else { iu.y *= imgAspect / scrAspect; }
    iu += 0.5;
    vec3 img = texture2D(inputImage, iu).rgb;

    // sky region: use image as backdrop; ground/blade region: tint
    if(isSky && bladeAlpha < 0.001){
      col = mix(col, img, texMix);
    } else {
      // tint whole field by image color (multiply-ish)
      vec3 tinted = col * mix(vec3(1.0), img * 1.6, 0.6);
      col = mix(col, tinted, texMix);
    }
  }

  // ---------- atmosphere + vignette ----------
  // horizon haze band
  float hazeBand = exp(-abs(uv.y - horizonY) * 12.0);
  col = mix(col, skyHaze, hazeBand * 0.18);

  vec2 vg = uv - 0.5;
  float vignette = smoothstep(1.1, 0.35, length(vg * vec2(aspect, 1.0)));
  col *= mix(0.78, 1.0, vignette);

  // gentle global mid-band lift so it breathes with audio (non-strobe)
  col *= 1.0 + mid * 0.06;

  // tonemap + gamma
  col = col / (1.0 + col);
  col = pow(max(col, 0.0), vec3(1.0 / 2.2));

  gl_FragColor = vec4(col, 1.0);
}
