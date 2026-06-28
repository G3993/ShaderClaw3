/*{
  "DESCRIPTION":"Sierpinski Gate — fly endlessly into a recursive triangle (Sierpinski) lattice; self-similar zoom loops seamlessly, neon folded edges glow on deep-black voids. Bass drives the dive, mid drifts the palette, treble flares the glow.",
  "CREDIT":"ShaderClaw3",
  "CATEGORIES":["Generator","Fractal","Tunnel","Audio Reactive"],
  "INPUTS":[
    {"NAME":"zoomSpeed","LABEL":"Dive Speed","TYPE":"float","DEFAULT":0.10,"MIN":0.0,"MAX":1.0},
    {"NAME":"scaleStep","TYPE":"float","DEFAULT":2.0,"MIN":1.5,"MAX":3.0},
    {"NAME":"iters","TYPE":"float","DEFAULT":8.0,"MIN":4.0,"MAX":10.0},
    {"NAME":"rotSpeed","TYPE":"float","DEFAULT":0.05,"MIN":0.0,"MAX":0.5},
    {"NAME":"edgeGlow","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0},
    {"NAME":"paletteShift","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"inputImage","LABEL":"Your Image","TYPE":"image"},
    {"NAME":"texMix","LABEL":"Image Amount","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"audioReact","LABEL":"Sound Reactivity","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/

vec3 pal(float t){return 0.5+0.5*cos(6.28318*(t+vec3(0.0,0.33,0.67)));}

mat2 rot(float a){float c=cos(a),s=sin(a);return mat2(c,-s,s,c);}

void main() {
  // live audio globals (auto-provided by runtime) -> capped bands, alive at silence
  float bass   = audioBass * audioReact;
  float mid    = audioMid  * audioReact;
  float treble = audioHigh * audioReact;

  vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);

  int it = int(iters);

  // self-similar seamless zoom toward camera (Sierpinski is scale-3-ish; use scaleStep)
  float ls = log(max(scaleStep, 1.5));
  float zoom = exp(fract(TIME * zoomSpeed * (1.0 + bass*0.6)) * ls);
  vec2 p = uv * zoom;

  // slow rotation + gentle depth-driven twist so the gate swirls
  float depth = log(max(zoom, 1e-4)) / ls;
  float ang = TIME * rotSpeed + depth * 0.18;
  p = rot(ang) * p;

  // Sierpinski IFS fold — fold toward nearest of 3 triangle vertices
  vec2 v0 = vec2(0.0, 1.0);
  vec2 v1 = vec2(0.866, -0.5);
  vec2 v2 = vec2(-0.866, -0.5);

  float scale = 1.0;
  float lastDepth = 0.0;
  for (int i = 0; i < 10; i++) {
    if (i >= it) break;
    p *= 2.0; scale *= 2.0;
    float d0 = dot(p - v0, p - v0);
    float d1 = dot(p - v1, p - v1);
    float d2 = dot(p - v2, p - v2);
    vec2 c = v0; float md = d0;
    if (d1 < md) { md = d1; c = v1; }
    if (d2 < md) { md = d2; c = v2; }
    p = p - c;
    lastDepth = float(i);
  }

  // distance estimate to the fractal structure
  float d = length(p) / scale;

  // folded local coordinate mapped to 0..1 -> tiles the image across the
  // self-similar Sierpinski cells (p is the final folded point in ~[-1,1])
  vec2 texUV = fract(p * 0.5 + 0.5);

  // glowing neon edges around the fractal lattice
  float aa = 1.5 / min(RENDERSIZE.x, RENDERSIZE.y) / scale;
  float glow = edgeGlow * (1.0 + treble * 0.6);
  float edge = glow * 0.012 / (d + 0.002);
  edge += smoothstep(aa*3.0, 0.0, d) * (0.6 + treble*0.4);

  // color indexed by iteration depth + palette drift + slow time + a little mid
  float t = lastDepth * 0.11 + paletteShift + TIME * 0.03 + depth * 0.05 + mid * 0.15;
  vec3 tint = pal(t);

  // texture the triangles with the user's image (guard: unuploaded = black)
  if (texMix > 0.0) {
    vec3 img = texture2D(inputImage, texUV).rgb;
    tint = mix(tint, tint * (0.25 + 1.75 * img), texMix);
  }

  vec3 col = tint * edge;

  // deep near-black void with a faint interior breath
  col += pal(t + 0.5) * 0.02 * smoothstep(0.6, 0.0, d);

  // tonemap + gamma (house style)
  col = col / (1.0 + col);
  col = pow(col, vec3(0.4545));

  gl_FragColor = vec4(col, 1.0);
}
