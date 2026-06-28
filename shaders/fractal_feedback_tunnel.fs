/*{
  "DESCRIPTION":"Fractal Feedback Tunnel — a persistent buffer is sampled zoomed + rotated every frame so neon detail spirals inward forever, accumulating an infinite fractal tunnel. Multipass feedback: pass 0 warps last frame inward and injects fresh kaleidoscopic rim detail (HDR, no tonemap); the screen pass tonemaps + vignettes. Alive in silence (constant slow zoom), audio pushes the pull-in, brightens spokes, and ripples the rim.",
  "CREDIT":"ShaderClaw3",
  "CATEGORIES":["Generator","Fractal","Tunnel","Feedback","Audio Reactive"],
  "INPUTS":[
    {"NAME":"zoomRate","LABEL":"Zoom Rate","TYPE":"float","DEFAULT":0.02,"MIN":0.0,"MAX":0.1},
    {"NAME":"rotRate","LABEL":"Swirl","TYPE":"float","DEFAULT":0.01,"MIN":0.0,"MAX":0.1},
    {"NAME":"decay","TYPE":"float","DEFAULT":0.975,"MIN":0.9,"MAX":0.995},
    {"NAME":"folds","TYPE":"float","DEFAULT":6.0,"MIN":3.0,"MAX":12.0},
    {"NAME":"rimRadius","TYPE":"float","DEFAULT":0.45,"MIN":0.2,"MAX":0.7},
    {"NAME":"lineW","TYPE":"float","DEFAULT":0.06,"MIN":0.01,"MAX":0.2},
    {"NAME":"inject","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0},
    {"NAME":"paletteShift","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"inputImage","LABEL":"Your Image","TYPE":"image"},
    {"NAME":"texMix","LABEL":"Image Amount","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"audioReact","LABEL":"Sound Reactivity","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ],
  "PASSES":[
    { "TARGET":"trailBuf", "PERSISTENT": true },
    {}
  ]
}*/

// ShaderClaw3 multipass / WebGL1 GLSL ES 1.00.
// Globals from host: RENDERSIZE, TIME, gl_FragCoord, gl_FragColor, PASSINDEX, FRAMEINDEX.
// trailBuf auto-bound as sampler2D (read with texture2D, uv 0..1).

// House cosine palette.
vec3 pal(float t){ return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0,0.33,0.67))); }

// Cheap value noise for a touch of organic ripple on the injected detail.
float hash(vec2 p){ return fract(sin(dot(p, vec2(127.1,311.7)))*43758.5453123); }
float vnoise(vec2 p){
  vec2 i = floor(p), f = fract(p);
  f = f*f*(3.0-2.0*f);
  float a = hash(i);
  float b = hash(i + vec2(1.0,0.0));
  float c = hash(i + vec2(0.0,1.0));
  float d = hash(i + vec2(1.0,1.0));
  return mix(mix(a,b,f.x), mix(c,d,f.x), f.y);
}

void main() {
  vec2 res = RENDERSIZE;
  vec2 uv  = gl_FragCoord.xy / res;

  // Live audio: runtime auto-provides audioBass/audioMid/audioHigh from the mic/FFT.
  // Scale by the user's Sound Reactivity; both passes read these.
  float bass   = audioBass * audioReact;
  float mid    = audioMid  * audioReact;
  float treble = audioHigh * audioReact;

  if (PASSINDEX == 0) {
    // ---------- FEEDBACK PASS: warp last frame inward, inject new rim detail. HDR, no tonemap. ----------
    vec2 c = uv - 0.5;
    c.x *= res.x / res.y;                                   // aspect-correct working space

    // Zoom > 1 pulls the previous frame toward the camera; bass deepens the pull (K<=1.5 -> stays calm).
    float zoom = 1.0 + zoomRate * (1.0 + bass*0.8);
    // Gentle swirl; mid adds a little extra rotation so the tunnel turns with the music.
    float rot  = rotRate * (1.0 + mid*0.6);
    mat2  R    = mat2(cos(rot), -sin(rot), sin(rot), cos(rot));

    // Sample where this pixel was a frame ago (rotated + scaled inward), then map back to 0..1 uv.
    vec2 prevC  = (R * c) / zoom;
    vec2 prevUV = prevC; prevUV.x *= res.y / res.x; prevUV += 0.5;
    vec3 prev   = texture2D(trailBuf, prevUV).rgb * decay;  // decay<1 fades old detail, prevents blow-up

    // ---- fresh fractal structure at the rim ----
    float r = length(c);
    float a = atan(c.y, c.x);

    // N-fold kaleidoscope on the angle.
    float fold   = abs(fract(a/6.28318 * folds) * 2.0 - 1.0);

    // Bright annulus near the rim, breathing slowly + with treble.
    float rim    = rimRadius + 0.05*sin(TIME*0.3) + 0.04*treble;
    float ring   = smoothstep(0.06, 0.0, abs(r - rim));

    // Radial spokes from the kaleidoscope, sharpened by lineW.
    float spokes = smoothstep(lineW, 0.0, abs(fold - 0.5));

    // Organic ripple so the injected detail never looks mechanical.
    float ripple = 0.6 + 0.4*vnoise(vec2(fold*6.0, r*9.0 - TIME*0.4));

    // Neon color: palette wraps with radius + angle so each ring lands a fresh hue.
    vec3  fresh  = pal(r*2.0 + a*0.2 + paletteShift + TIME*0.03)
                 * (ring + spokes*0.6) * ripple
                 * inject * (0.6 + treble*0.6);

    // ---- USER IMAGE: feed the uploaded picture in as fresh detail so it gets pulled
    //      into the infinite zoom and smears into fractal trails. Unuploaded = black, so guard. ----
    if (texMix > 0.0) {
      float imgAspect = IMG_SIZE_inputImage.x / max(IMG_SIZE_inputImage.y, 1.0);
      vec2  iuv = vec2(c.x / imgAspect, c.y) + 0.5;   // centered, aspect-correct, fit by height
      vec3  img = vec3(0.0);
      if (iuv.x >= 0.0 && iuv.x <= 1.0 && iuv.y >= 0.0 && iuv.y <= 1.0)
        img = texture2D(inputImage, iuv).rgb;
      vec3 imgFresh = img * inject * (0.7 + treble*0.5);
      // Blend the image into the injected detail, then add a touch so it accumulates in the trails.
      fresh = mix(fresh, imgFresh, texMix) + imgFresh * 0.2 * texMix;
    }

    // Accumulate: max keeps trails bright without runaway; small additive term to seed glow.
    vec3 col = max(prev, fresh) + fresh*0.15;

    gl_FragColor = vec4(col, 1.0);

  } else {
    // ---------- SCREEN PASS: read accumulated trail, tonemap + gamma + vignette. ----------
    vec3 col = texture2D(trailBuf, uv).rgb;

    // Subtle chromatic pull toward center sells the "rushing down a tunnel" feel.
    vec2 d = uv - 0.5;
    float ca = 0.004 + bass*0.004;
    float rC = texture2D(trailBuf, uv - d*ca).r;
    float bC = texture2D(trailBuf, uv + d*ca).b;
    col.r = mix(col.r, rC, 0.6);
    col.b = mix(col.b, bC, 0.6);

    float v = smoothstep(1.1, 0.3, length(d));
    col *= v;

    col = col / (1.0 + col);            // Reinhard tonemap
    col = pow(col, vec3(0.4545));       // gamma to sRGB

    gl_FragColor = vec4(col, 1.0);
  }
}
