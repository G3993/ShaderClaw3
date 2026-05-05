/*{
  "DESCRIPTION": "Vaporwave Portrait — classical bust silhouette on magenta/cyan gradient with Greek columns. v3: figurative composition vs v2 3D city / v1 flat vaporwave.",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw auto-improve v3",
  "INPUTS": [
    {"NAME":"scanFreq","TYPE":"float","DEFAULT":2.0,"MIN":0.5,"MAX":4.0},
    {"NAME":"hdrBoost","TYPE":"float","DEFAULT":2.4,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
float sdCircle(vec2 p,float r){return length(p)-r;}
float sdRect(vec2 p,vec2 b){vec2 d=abs(p)-b;return length(max(d,0.0))+min(max(d.x,d.y),0.0);}
float bustSDF(vec2 p){
    float head=sdCircle(p-vec2(0,0.15),0.18);
    float neck=sdRect(p-vec2(0,-0.07),vec2(0.055,0.08));
    float sholdr=sdRect(p-vec2(0,-0.25),vec2(0.40,0.14));
    return min(min(head,neck),sholdr);
}
void main(){
    vec2 uv=(isf_FragNormCoord-0.5)*2.0; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    float t=TIME*0.15; float audio=1.0+(audioLevel+audioBass*0.4)*audioMod*0.22;
    vec3 HOT_PINK=vec3(2.4,0.06,1.2)*hdrBoost*audio;
    vec3 ELEC_CYAN=vec3(0.05,2.4,2.2)*hdrBoost*audio;
    vec3 DEEP_MAG=vec3(2.2,0.0,0.9)*hdrBoost*audio;
    vec3 VIOLET=vec3(1.2,0.0,2.5)*hdrBoost*audio;
    vec3 INK_BLK=vec3(0);
    float gy=uv.y*0.5+0.5;
    vec3 bg=mix(ELEC_CYAN*0.5,HOT_PINK*0.5,gy);
    bg*=0.80+0.20*sin(gl_FragCoord.y*scanFreq);
    bg+=VIOLET*0.08*sin(uv.y*4.0+t);
    vec3 col=bg;
    for(int ci=-2;ci<=2;ci++){
        float fx=float(ci)*0.45;
        float d=sdRect(uv-vec2(fx,-0.2),vec2(0.035,0.7));
        float fw=fwidth(d);
        col=mix(col,DEEP_MAG*0.35,smoothstep(fw,-fw,d));
        float cap=sdRect(uv-vec2(fx,0.5),vec2(0.065,0.025));
        col=mix(col,DEEP_MAG*0.4,smoothstep(fwidth(cap),-fwidth(cap),cap));
    }
    if(uv.y<-0.45){
        float fx=floor(uv.x*4.0+t*0.3);
        float fy=floor(uv.y*4.0);
        float checker=mod(fx+fy,2.0);
        vec3 chk=mix(INK_BLK,HOT_PINK*0.4,checker);
        col=mix(col,chk,smoothstep(-0.45,-0.55,uv.y));
    }
    float bd=bustSDF(uv); float fw=fwidth(bd);
    col=mix(col,INK_BLK,smoothstep(fw,-fw,bd));
    col+=HOT_PINK*exp(-max(bd,0.0)*25.0)*0.9*(1.0-smoothstep(fw,-fw,bd));
    col+=ELEC_CYAN*0.04*(sin(uv.y*18.0+t*2.0)*0.5+0.5)*(1.0-smoothstep(fw,-fw,bd));
    gl_FragColor=vec4(col,1.0);
}
