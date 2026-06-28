/*{
  "DESCRIPTION":"Phyllotaxis Rush — a 3D sunflower-seed spiral of glowing dots streaming toward the viewer along the golden angle. Calm starfield flight, live mic/audio-reactive rush + sparkle. Optionally reconstructs your image out of the flying dots.",
  "CREDIT":"ShaderClaw3",
  "CATEGORIES":["Generator","3D","Particles","Audio Reactive"],
  "INPUTS":[
    {"NAME":"rushSpeed","LABEL":"Rush Speed","TYPE":"float","DEFAULT":0.16,"MIN":0.0,"MAX":1.0},
    {"NAME":"spread","TYPE":"float","DEFAULT":2.2,"MIN":0.5,"MAX":5.0},
    {"NAME":"nearZ","TYPE":"float","DEFAULT":0.25,"MIN":0.05,"MAX":1.0},
    {"NAME":"farZ","TYPE":"float","DEFAULT":4.0,"MIN":1.0,"MAX":10.0},
    {"NAME":"dotSize","TYPE":"float","DEFAULT":0.012,"MIN":0.002,"MAX":0.05},
    {"NAME":"brightness","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":3.0},
    {"NAME":"paletteShift","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"rotSpeed","TYPE":"float","DEFAULT":0.06,"MIN":0.0,"MAX":0.5},
    {"NAME":"audioReact","LABEL":"Sound Reactivity","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0},
    {"NAME":"inputImage","LABEL":"Your Image","TYPE":"image"},
    {"NAME":"texMix","LABEL":"Image Amount","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0}
  ]
}*/

vec3 pal(float t){return 0.5+0.5*cos(6.28318*(t+vec3(0.0,0.33,0.67)));}

const int N = 180;

void main() {
  // live audio globals (audioBass/audioMid/audioHigh) are auto-declared by the runtime
  float bass   = audioBass * audioReact;
  float mid    = audioMid  * audioReact;
  float treble = audioHigh * audioReact;

  vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);

  vec3 col = vec3(0.0);

  // global slow rotation of the whole spiral (mid loosens the drift a touch)
  float baseRot = TIME * rotSpeed * (1.0 + mid*0.4);

  float Nf = float(N);

  for (int i = 0; i < 180; i++) {
    float fi = float(i);

    float ang = fi * 2.39996323 + baseRot;          // golden angle + drift
    float rad = sqrt(fi / Nf) * spread;             // phyllotaxis radius (XY plane)

    // travel toward camera; bass speeds the rush (K = 0.6)
    float z = fract(fi / Nf - TIME * rushSpeed * (1.0 + bass*0.6)); // 0 far .. 1 near
    float depth = mix(farZ, nearZ, z);              // near = small depth

    vec2 proj = vec2(cos(ang), sin(ang)) * rad / depth; // perspective: closer = spread out
    float size = dotSize / depth;                   // closer = bigger

    float d = length(uv - proj);
    float glow = smoothstep(size, 0.0, d);          // crisp core
    glow += 0.4 * smoothstep(size*3.0, 0.0, d);     // soft halo

    // fade in while far, fade out as it sweeps past the camera
    float fade = smoothstep(0.0, 0.15, z) * (1.0 - smoothstep(0.85, 1.0, z));

    // treble sparkles the nearest dots (positional pulse, K <= 0.6)
    float sparkle = 1.0 + treble * 0.6 * smoothstep(0.6, 1.0, z)
                        * (0.5 + 0.5*sin(fi*12.9898 + TIME*9.0));

    vec3 tint = pal(fi*0.012 + paletteShift + TIME*0.02);

    // reconstruct the user's image from the flat phyllotaxis seed layout:
    // sample inputImage at each dot's base position, tint that dot by it.
    // (unuploaded image = black, so guard on texMix; default texMix 0)
    if (texMix > 0.0) {
      vec2 seed = vec2(cos(ang), sin(ang)) * sqrt(fi / Nf) * 0.5 + 0.5;
      vec3 img = texture2D(inputImage, seed).rgb;
      tint = mix(tint, img, texMix);
    }

    col += tint * glow * fade * sparkle * brightness;
  }

  // tonemap + gamma
  col = col / (1.0 + col);
  col = pow(col, vec3(0.4545));

  gl_FragColor = vec4(col, 1.0);
}
