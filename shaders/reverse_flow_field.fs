/*{
  "DESCRIPTION": "Neon Silk Ribbons — iso-contour ribbons of domain-warped FBM field. Cyan/magenta/gold/violet on void black. v4: contour ribbons vs v2 3D volumetric aurora / v1 volcanic flow trace.",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"ribbons",   "TYPE":"float","DEFAULT":14.0,"MIN":4.0,"MAX":24.0},
    {"NAME":"warpSpeed", "TYPE":"float","DEFAULT":0.35,"MIN":0.0,"MAX":2.0},
    {"NAME":"warpAmt",   "TYPE":"float","DEFAULT":1.2,"MIN":0.0,"MAX":3.0},
    {"NAME":"hdrBoost",  "TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod",  "TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
vec2 domainWarp(vec2 p,float t){
    vec2 q=vec2(sin(p.x*1.7+t)*cos(p.y*1.2-t*0.6),cos(p.x*1.3-t*0.4)*sin(p.y*2.1+t*0.7));
    vec2 r=vec2(sin((p.x+q.x)*2.3+t*0.5),cos((p.y+q.y)*1.9-t*0.3));
    return p+q*warpAmt*0.3+r*warpAmt*0.12;
}
float field(vec2 p){return sin(p.x*1.8+p.y*1.1)*cos(p.y*2.7-p.x*0.8)*sin(p.x*0.9+p.y*3.2);}
void main(){
    vec2 uv=isf_FragNormCoord*2.0-1.0; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    float t=TIME*warpSpeed; float audio=1.0+(audioLevel+audioBass*0.5)*audioMod*0.22;
    vec3 CYAN=vec3(0.0,2.5,2.3)*hdrBoost*audio;
    vec3 MAG=vec3(2.5,0.05,1.8)*hdrBoost*audio;
    vec3 GOLD=vec3(2.4,1.7,0.0)*hdrBoost*audio;
    vec3 VIOLET=vec3(1.5,0.0,2.5)*hdrBoost*audio;
    vec3 BG=vec3(0,0,0.012);
    vec2 wp=domainWarp(uv,t); float f=field(wp);
    int N=int(clamp(ribbons,4.0,24.0)); float step_f=2.0/float(N);
    vec3 col=BG;
    for(int i=0;i<24;i++){
        if(i>=N)break; float fi=float(i);
        float iso=-1.0+fi*step_f+step_f*0.5;
        float d=abs(f-iso); float fw=fwidth(f)*0.4;
        float ribbon=smoothstep(fw*2.5,fw*0.1,d);
        int ci=int(mod(fi,4.0));
        vec3 rc=(ci==0)?CYAN:(ci==1)?MAG:(ci==2)?GOLD:VIOLET;
        col=mix(col,rc,ribbon*0.88);
        col=mix(col,BG,smoothstep(fw*5.0,0.0,d-fw*0.5)*ribbon*0.55);
    }
    gl_FragColor=vec4(col,1.0);
}
