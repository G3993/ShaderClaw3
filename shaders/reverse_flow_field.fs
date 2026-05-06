/*{
  "DESCRIPTION": "Flowing Tendrils — 3D curving hair strands, deep teal gradient on midnight blue",
  "CREDIT": "ShaderClaw3 auto-improve v16",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME": "strandCount", "TYPE": "float", "DEFAULT": 8.0,  "MIN": 3.0, "MAX": 16.0},
    {"NAME": "flowSpeed",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0},
    {"NAME": "audioMod",    "TYPE": "audio"}
  ]
}*/
precision highp float;
#define TAU 6.28318530718
float apeak(){return (audioBass.x+audioBass.y+audioBass.z+audioBass.w)*0.25;}
float hash1(float n){return fract(sin(n)*43758.5);}
float noise1(float x){float i=floor(x),f=fract(x);f=f*f*(3.0-2.0*f);return mix(hash1(i),hash1(i+1.0),f);}
float sdSeg(vec3 p,vec3 a,vec3 b){vec3 ab=b-a,ap=p-a;float t=clamp(dot(ap,ab)/dot(ab,ab),0.0,1.0);return length(ap-t*ab);}
vec3 bez(vec3 p0,vec3 p1,vec3 p2,float t){return (1.0-t)*(1.0-t)*p0+2.0*(1.0-t)*t*p1+t*t*p2;}
float strandSDF(vec3 pos,float idx,float t){
    float seed=idx*7.3+1.1;
    float ph=hash1(seed)*TAU;
    float r0=0.3+hash1(seed+1.0)*0.35;
    vec3 p0=vec3(cos(ph)*r0,-0.85,sin(ph)*r0);
    float wx=(noise1(t*flowSpeed+idx*2.3+0.5)-0.5)*0.45;
    float wz=(noise1(t*flowSpeed*0.7+idx*3.1+1.5)-0.5)*0.45;
    vec3 p1=p0+vec3(wx,0.85,wz);
    vec3 p2=p0+vec3(wx*2.0,1.7,wz*2.0);
    float minD=1e9;
    vec3 prev=bez(p0,p1,p2,0.0);
    for(int k=1;k<=10;k++){
        float ft=float(k)/10.0;vec3 curr=bez(p0,p1,p2,ft);
        minD=min(minD,sdSeg(pos,prev,curr));prev=curr;}
    return minD-0.011;}
float mapScene(vec3 p,float ap,float t){
    float d=1e9;float n=min(floor(strandCount),16.0);
    for(float i=0.0;i<16.0;i++){if(i>=n)break;d=min(d,strandSDF(p,i,t));}
    return d;}
vec3 calcNorm(vec3 p,float ap,float t){float e=0.001;
    return normalize(vec3(
        mapScene(p+vec3(e,0,0),ap,t)-mapScene(p-vec3(e,0,0),ap,t),
        mapScene(p+vec3(0,e,0),ap,t)-mapScene(p-vec3(0,e,0),ap,t),
        mapScene(p+vec3(0,0,e),ap,t)-mapScene(p-vec3(0,0,e),ap,t)));}
void main(){
    vec2 uv=(isf_FragNormCoord*2.0-1.0)*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    float ap=apeak();float t=TIME;
    vec3 ro=vec3(0.0,0.1,2.2);
    vec3 rd=normalize(vec3(uv*0.8,-1.4));
    float dist=0.0;bool hit=false;
    for(int i=0;i<64;i++){
        float d=mapScene(ro+rd*dist,ap,t);
        if(d<0.001){hit=true;break;}
        if(dist>5.0)break;dist+=d*0.8;}
    // Midnight blue background with subtle upward gradient
    vec3 col=vec3(0.005,0.01,0.04)+vec3(0.0,0.02,0.08)*pow(max(0.0,dot(rd,vec3(0,1,0))),2.0);
    if(hit){
        vec3 p=ro+rd*dist;
        vec3 n=calcNorm(p,ap,t);
        vec3 L=normalize(vec3(0.5,1.5,1.0));
        float diff=max(dot(n,L),0.0);
        float spec=pow(max(dot(reflect(-L,n),-rd),0.0),32.0);
        // Height gradient: deep cobalt root → bright teal mid → HDR cyan tip
        float ht=clamp((p.y+0.85)/1.7,0.0,1.0);
        vec3 base=ht<0.5?mix(vec3(0.0,0.15,0.45),vec3(0.0,0.7,0.8),ht*2.0):
                         mix(vec3(0.0,0.7,0.8),vec3(0.5,2.5,2.8),(ht-0.5)*2.0);
        col=base*(0.15+diff*1.8)+vec3(1.5,3.0,3.5)*spec*(1.0+ap*0.5);
        // Deep violet rim
        float rim=pow(1.0-max(dot(n,-rd),0.0),3.0);
        col+=vec3(1.0,0.0,2.5)*rim*(0.8+ap*0.6);
        // fwidth AA silhouette edge
        float sil=fwidth(dot(n,-rd));
        col*=smoothstep(0.0,sil,dot(n,-rd)+0.05);}
    gl_FragColor=vec4(col,1.0);}
