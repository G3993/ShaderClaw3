/*{
  "DESCRIPTION": "Snowflake Formation — 2D hexagonal snowflake with branching arms. Ice-blue HDR on void black. v4: 2D hex symmetry vs prior 2D Voronoi frost / 3D crystal shard ring.",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"branches","TYPE":"float","DEFAULT":4.0,"MIN":1.0,"MAX":6.0},
    {"NAME":"rotSpd",  "TYPE":"float","DEFAULT":0.12,"MIN":0.0,"MAX":1.0},
    {"NAME":"hdrBoost","TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
mat2 rot2(float a){float c=cos(a),s=sin(a);return mat2(c,-s,s,c);}
float sdSeg(vec2 p,vec2 a,vec2 b,float r){vec2 pa=p-a,ba=b-a;float h=clamp(dot(pa,ba)/dot(ba,ba),0,1);return length(pa-ba*h)-r;}
float snowflake(vec2 p,float armLen,float brN,float rot){
    float d=1e8;
    for(int arm=0;arm<6;arm++){
        float ang=float(arm)*1.0472+rot;
        vec2 lp=rot2(ang)*p;
        d=min(d,sdSeg(lp,vec2(0),vec2(armLen,0),0.011));
        int nb=int(clamp(brN,1.0,6.0));
        for(int b=1;b<=6;b++){
            if(b>nb)break;
            float fb=float(b); float pos2=armLen*(0.18+fb*0.12); float blen=armLen*0.18*(1.0-fb*0.1);
            vec2 bp=lp-vec2(pos2,0);
            d=min(d,sdSeg(bp,vec2(0),vec2(0,blen),0.007));
            d=min(d,sdSeg(bp,vec2(0),vec2(0,-blen),0.007));
        }
    }
    for(int h=0;h<6;h++){float a=float(h)*1.0472+rot; float a2=a+1.0472; d=min(d,sdSeg(p,0.06*vec2(cos(a),sin(a)),0.06*vec2(cos(a2),sin(a2)),0.009));}
    return d;
}
void main(){
    vec2 uv=isf_FragNormCoord*2.0-1.0; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    float t=TIME*rotSpd; float audio=1.0+(audioLevel+audioBass*0.4)*audioMod*0.22;
    vec3 ICE_BLUE=vec3(0.4,0.85,2.6)*hdrBoost*audio;
    vec3 GLACIER=vec3(0.1,1.6,2.2)*hdrBoost*audio;
    vec3 ICE_WHITE=vec3(2.0,2.2,2.8)*hdrBoost*audio;
    vec3 VOID=vec3(0.005,0.010,0.042);
    vec3 col=VOID;
    for(int fi=0;fi<6;fi++){float ffi=float(fi);float a=ffi*1.0472;vec2 fp=uv-0.75*vec2(cos(a),sin(a));float d2=snowflake(fp*2.5,0.38,2.0,t*0.3+ffi*0.9);col+=GLACIER*0.25*exp(-max(d2,0.0)*18.0);}
    float d=snowflake(uv,0.42,branches,t);
    float fw=fwidth(d);
    col=mix(col,ICE_BLUE,exp(-max(d,0.0)*10.0)*0.5);
    col=mix(col,ICE_WHITE,smoothstep(fw,-fw,d));
    col=mix(col,VOID*2.0,smoothstep(fw*3.0,0.0,abs(d))*0.35);
    gl_FragColor=vec4(col,1.0);
}
