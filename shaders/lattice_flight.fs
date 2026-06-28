/*{
  "DESCRIPTION":"Lattice Flight — neon wireframe lattice rushing toward the camera. A screen-space vector tunnel of square grid slices flying past with converging perspective spokes toward an off-center vanishing point. Pure black between glowing thin neon lines. Live mic audio drives flight speed, rotation and line brightness; an optional user image rushes toward the camera tiled across every grid cell.",
  "CREDIT":"ShaderClaw3",
  "CATEGORIES":["Generator","3D","Tunnel","Audio Reactive"],
  "INPUTS":[
    {"NAME":"flightSpeed","LABEL":"Flight Speed","TYPE":"float","DEFAULT":0.12,"MIN":0.0,"MAX":1.0},
    {"NAME":"gridFreq","LABEL":"Grid Frequency","TYPE":"float","DEFAULT":4.0,"MIN":1.0,"MAX":16.0},
    {"NAME":"lineWidth","LABEL":"Line Width","TYPE":"float","DEFAULT":0.045,"MIN":0.005,"MAX":0.2},
    {"NAME":"brightness","LABEL":"Brightness","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":3.0},
    {"NAME":"paletteShift","LABEL":"Palette Shift","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"rotSpeed","LABEL":"Rotation Speed","TYPE":"float","DEFAULT":0.05,"MIN":0.0,"MAX":0.5},
    {"NAME":"spokes","LABEL":"Spokes","TYPE":"float","DEFAULT":16.0,"MIN":4.0,"MAX":48.0},
    {"NAME":"audioReact","LABEL":"Sound Reactivity","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0},
    {"NAME":"inputImage","LABEL":"Your Image","TYPE":"image"},
    {"NAME":"texMix","LABEL":"Image Amount","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0}
  ]
}*/

vec3 pal(float t){
  return 0.5 + 0.5*cos(6.28318*(t + vec3(0.0, 0.33, 0.67)));
}

mat2 rot(float a){
  float c = cos(a), s = sin(a);
  return mat2(c, -s, s, c);
}

void main() {
  // live mic FFT globals (audioBass/audioMid/audioHigh are auto-provided & auto-declared)
  float bass   = audioBass * audioReact;   // K caps below keep base*(1+band*K), K<=1.5
  float mid    = audioMid  * audioReact;
  float treble = audioHigh * audioReact;

  vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);

  // mid slowly rotates the whole lattice (calm)
  float ang = TIME * rotSpeed * (1.0 + mid*0.5);
  uv = rot(ang) * uv;

  // treble sharpens & brightens the lines (K <= 0.6)
  float lw = lineWidth * (1.0 - treble*0.5);          // thinner = sharper
  lw = max(lw, 0.004);
  float bright = brightness * (1.0 + treble*0.6);

  // bass pushes the flight speed forward (K <= 0.7)
  float speed = flightSpeed * (1.0 + bass*0.7);

  const float range = 6.0;   // depth span of recycled slices
  const float near  = 0.25;  // nearest depth

  vec3 col = vec3(0.0);

  // ---- flying grid slices ----
  const int K = 10;
  for (int k = 0; k < K; k++) {
    float fk = float(k) / float(K);
    float z = fract(fk - TIME*speed);          // 0..1 recycling depth coord
    float depth = z*range + near;              // grows as slice nears camera

    vec2 g = uv * depth;                       // perspective expansion
    vec2 cell = fract(g*gridFreq);             // 0..1 coordinate within each grid cell

    vec2 gf = abs(cell - 0.5);
    float line = min(gf.x, gf.y);

    float glow = smoothstep(lw, 0.0, line);            // thin bright core
    float halo = smoothstep(lw*5.0, 0.0, line) * 0.35; // soft bloom halo

    float fade = (1.0 - z);                    // far slices dim
    fade = fade*fade;

    vec3 tint = pal(depth*0.1 + paletteShift);

    // optional user image: each grid cell shows the WHOLE image, tiled, rushing toward camera.
    // unuploaded image samples BLACK, so guard on texMix (default 0 => pure-black gaps).
    if (texMix > 0.0) {
      vec3 img = texture2D(inputImage, cell).rgb;
      tint = mix(tint, tint*img + img, texMix);   // tint the neon lines toward the image
      col += img * (texMix * fade * 0.5);         // emissive image fill of the cell, depth-faded
    }

    col += tint * (glow + halo) * fade * bright;
  }

  // ---- converging radial perspective spokes (vanishing point off-center) ----
  vec2 vp = uv - vec2(0.12, -0.07);            // slightly off-center
  float a = atan(vp.y, vp.x);
  float r = length(vp);

  // angular line distance toward vanishing point
  float sp = max(spokes, 4.0);
  float spk = abs(fract(a/6.28318 * sp + TIME*speed*0.5) - 0.5);
  float spokeLine = smoothstep(lw*1.5, 0.0, spk);
  float spokeHalo = smoothstep(lw*6.0, 0.0, spk) * 0.3;

  // spokes fade in toward the vanishing point and out at the edges
  float spokeFade = smoothstep(0.0, 0.35, r) * (1.0 - smoothstep(0.7, 1.4, r));

  vec3 spokeTint = pal(r*0.6 + paletteShift + 0.5);
  col += spokeTint * (spokeLine + spokeHalo) * spokeFade * bright * 0.7;

  // keep alive at audio=0: gentle central glow at the vanishing point
  col += pal(TIME*0.03 + paletteShift) * 0.04 / (r*4.0 + 0.3);

  // ---- neon finish ----
  col = col / (1.0 + col);
  col = pow(col, vec3(0.4545));

  gl_FragColor = vec4(col, 1.0);
}
