/*{
  "DESCRIPTION":"Menger Descent — minimal monochrome fly-through of an infinite 3D Menger sponge. Cool-white architecture on near-black, thin glowing edges, depth fog. Bass drives descent speed, treble adds faint edge shimmer, mid slowly rotates the whole structure.",
  "CREDIT":"ShaderClaw3",
  "CATEGORIES":["Generator","Fractal","3D","Audio Reactive"],
  "INPUTS":[
    {"NAME":"camSpeed","LABEL":"Descent Speed","TYPE":"float","DEFAULT":0.09,"MIN":0.0,"MAX":1.0},
    {"NAME":"edgeGlow","LABEL":"Edge Glow","TYPE":"float","DEFAULT":0.6,"MIN":0.0,"MAX":2.0},
    {"NAME":"fogDensity","LABEL":"Fog Density","TYPE":"float","DEFAULT":0.12,"MIN":0.0,"MAX":0.5},
    {"NAME":"accentColor","LABEL":"Accent Color","TYPE":"color","DEFAULT":[0.7,0.85,1.0,1.0]},
    {"NAME":"iterations","LABEL":"Sponge Detail","TYPE":"float","DEFAULT":5.0,"MIN":2.0,"MAX":6.0},
    {"NAME":"inputImage","LABEL":"Your Image","TYPE":"image"},
    {"NAME":"texMix","LABEL":"Image Amount","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"audioReact","LABEL":"Sound Reactivity","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/

// ---- SDFs ------------------------------------------------------------------
float sdBox(vec3 p, vec3 b){ vec3 d=abs(p)-b; return length(max(d,0.0))+min(max(d.x,max(d.y,d.z)),0.0); }

float mengerDE(vec3 p){
  float d = sdBox(p, vec3(1.0));
  float s = 1.0;
  for(int m=0;m<5;m++){
    if(float(m) >= iterations) break;
    vec3 a = mod(p*s, 2.0) - 1.0;
    s *= 3.0;
    vec3 r = abs(1.0 - 3.0*abs(a));
    float da=max(r.x,r.y), db=max(r.y,r.z), dc=max(r.z,r.x);
    float c=(min(da,min(db,dc))-1.0)/s;
    d=max(d,c);
  }
  return d;
}

// ---- helpers ---------------------------------------------------------------
mat2 rot(float a){ float c=cos(a), s=sin(a); return mat2(c,-s,s,c); }

// scene wrapper: slowly rotate the whole sponge with mid
float map(vec3 p){
  float ang = TIME*0.04 + mid*0.6;        // calm rotation, mid eases it
  p.xz = rot(ang) * p.xz;
  p.xy = rot(ang*0.5) * p.xy;
  return mengerDE(p);
}

vec3 calcNormal(vec3 p){
  vec2 e = vec2(0.0015, 0.0);
  return normalize(vec3(
    map(p+e.xyy)-map(p-e.xyy),
    map(p+e.yxy)-map(p-e.yxy),
    map(p+e.yyx)-map(p-e.yyx)
  ));
}

void main(){
  // live audio: runtime auto-provides audioBass/audioMid/audioHigh from the mic FFT
  float bass   = audioBass * audioReact;
  float mid    = audioMid  * audioReact;
  float treble = audioHigh * audioReact;

  vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);

  // ---- camera: descend THROUGH the sponge -----------------------------------
  float spd = camSpeed * (1.0 + bass*0.7);          // bass quickens descent, K<=0.7
  float t0  = TIME * spd;
  vec3 ro = vec3(0.18*sin(t0*0.6), t0, 0.18*cos(t0*0.4)); // drift down the y axis

  vec3 rd = normalize(vec3(uv, 1.5));
  // slow yaw / pitch ~0.05
  float yaw   = sin(TIME*0.05) * 0.05;
  float pitch = cos(TIME*0.04) * 0.05;
  rd.xz = rot(yaw)   * rd.xz;
  rd.yz = rot(pitch) * rd.yz;

  // ---- raymarch -------------------------------------------------------------
  float t = 0.0;
  float steps = 0.0;
  bool hit = false;
  vec3 p = ro;
  for(int i=0;i<100;i++){
    p = ro + rd*t;
    float d = map(p);
    if(d < 0.001){ hit = true; break; }
    if(t > 30.0) break;
    t += d;
    steps += 1.0;
  }

  vec3 accent = accentColor.rgb;
  vec3 col = vec3(0.0);

  if(hit){
    vec3 n = calcNormal(p);

    // soft normal-based shading (key light from above-front)
    vec3 lig = normalize(vec3(0.4, 0.8, -0.5));
    float diff = clamp(dot(n, lig), 0.0, 1.0);
    float fill = clamp(0.5 + 0.5*dot(n, vec3(-0.3,0.2,0.6)), 0.0, 1.0);
    float shade = 0.12 + 0.55*diff + 0.18*fill;

    // ambient occlusion from step count (more steps = deeper crevice = darker)
    float ao = clamp(1.0 - steps/85.0, 0.0, 1.0);
    ao = ao*ao*0.7 + 0.3;

    // thin bright edges: where two faces meet, the normal is far from any axis
    vec3 an = abs(n);
    float maxc = max(an.x, max(an.y, an.z));
    float edge = smoothstep(0.78, 0.62, maxc);   // 1 near corners/edges, 0 on flats
    float shimmer = 1.0 + treble*0.6*sin(TIME*6.0 + p.y*8.0); // K<=0.6
    float edgeAmt = edge * edgeGlow * shimmer;

    // assemble: cool-grey structure + cool-white glowing edges
    vec3 baseAlbedo = vec3(0.78, 0.82, 0.9);

    // optional user image: triplanar-map onto the sponge faces as albedo (subtle)
    if(texMix > 0.0){
      vec3 tn = abs(n);
      tn /= (tn.x + tn.y + tn.z + 1e-4);            // normalize blend weights
      vec3 tex = texture2D(inputImage, p.yz*0.2+0.5).rgb * tn.x
               + texture2D(inputImage, p.xz*0.2+0.5).rgb * tn.y
               + texture2D(inputImage, p.xy*0.2+0.5).rgb * tn.z;
      baseAlbedo = mix(baseAlbedo, tex, texMix);     // user's dial keeps the monochrome look intact
    }

    vec3 structure = baseAlbedo * shade * ao;
    col = structure + accent * edgeAmt * (0.8 + 0.6*ao);

    // depth fog to black
    float fog = exp(-t * fogDensity);
    col *= fog;
  } else {
    // background: near-black, faint vertical glow so it stays alive
    float bg = 0.015 + 0.01*smoothstep(0.7, -0.2, abs(uv.y));
    col = accent * bg;
  }

  // ---- output ---------------------------------------------------------------
  col = col/(1.0+col);
  col = pow(col, vec3(0.4545));
  gl_FragColor = vec4(col, 1.0);
}
