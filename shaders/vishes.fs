/*{
  "DESCRIPTION": "Turing Morphogenesis — 2D reaction-diffusion Turing patterns, vivid biomorphic HDR palette",
  "CREDIT": "ShaderClaw3 auto-improve v16",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME": "patScale",   "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 8.0},
    {"NAME": "driftSpeed", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0},
    {"NAME": "hdrPeak",    "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 5.0},
    {"NAME": "audioMod",   "TYPE": "audio"}
  ]
}*/
precision highp float;
#define TAU 6.28318530718
float apeak(){return (audioBass.x+audioBass.y+audioBass.z+audioBass.w)*0.25;}
float hash2(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5);}
float noise2(vec2 p){vec2 i=floor(p),f=fract(p);f=f*f*(3.0-2.0*f);
    return mix(mix(hash2(i),hash2(i+vec2(1,0)),f.x),mix(hash2(i+vec2(0,1)),hash2(i+vec2(1,1)),f.x),f.y);}
// Activator: fine-scale FBM
float activator(vec2 p){float v=0.0,a=0.5;for(int i=0;i<5;i++){v+=a*noise2(p);p*=2.1;a*=0.5;}return v;}
// Inhibitor: coarse-scale FBM (different frequency ratio)
float inhibitor(vec2 p){float v=0.0,a=0.5;for(int i=0;i<5;i++){v+=a*noise2(p);p*=2.35+float(i)*0.08;a*=0.48;}return v;}
// Palette: void black → deep violet → hot magenta → acid lime → electric cyan
vec3 morphPal(float t){
    t=fract(t+1.0);
    if(t<0.2) return mix(vec3(0.0),       vec3(0.3,0.0,0.6),  t/0.2);
    if(t<0.4) return mix(vec3(0.3,0.0,0.6),vec3(2.5,0.0,1.8),(t-0.2)/0.2);
    if(t<0.6) return mix(vec3(2.5,0.0,1.8),vec3(0.2,2.8,0.0),(t-0.4)/0.2);
    if(t<0.8) return mix(vec3(0.2,2.8,0.0),vec3(0.0,2.5,2.5),(t-0.6)/0.2);
    return mix(vec3(0.0,2.5,2.5),vec3(0.0),                  (t-0.8)/0.2);
}
void main(){
    float ap=apeak();
    vec2 uv=isf_FragNormCoord*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0)*patScale;
    float t=TIME*driftSpeed;
    // Domain warp for organic flow
    vec2 warp=vec2(noise2(uv*0.7+t*0.25)-0.5,noise2(uv*0.7+t*0.25+vec2(43.1,17.3))-0.5)*0.4*(1.0+ap*0.3);
    vec2 p=uv+warp;
    float act=activator(p*1.5+t*0.18);
    float inh=inhibitor(p*0.55+t*0.14+vec2(7.3,13.7));
    float turing=act-inh;
    // Smooth threshold: positive = spot/stripe region
    float pattern=smoothstep(-0.04,0.04,turing);
    // fwidth AA on Turing boundary contour
    float fw=fwidth(turing);
    float boundary=smoothstep(fw*2.5,0.0,abs(turing)-fw*0.4);
    // Color: hue driven by activator intensity + drift
    float hue=pattern*0.55+act*0.18+t*0.04+ap*0.08;
    vec3 col=morphPal(hue)*hdrPeak*(0.7+pattern*0.5)*(1.0+ap*0.3);
    // Black ink edge at Turing contour boundaries
    col*=1.0-boundary*0.85;
    // HDR hot-white flash at high-activator peaks
    col+=vec3(2.8,2.8,2.8)*max(0.0,act-0.72)*pattern*(1.0+ap*0.45);
    gl_FragColor=vec4(col,1.0);
}
