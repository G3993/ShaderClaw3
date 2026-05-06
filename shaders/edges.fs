/*{
  "DESCRIPTION": "Armillary Sphere — 3D concentric gyroscope rings, gold on midnight navy, electric cyan rim",
  "CREDIT": "ShaderClaw3 auto-improve v17",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME": "ringCount", "TYPE": "float", "DEFAULT": 5.0, "MIN": 2.0, "MAX": 8.0},
    {"NAME": "spinSpeed", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0},
    {"NAME": "audioMod",  "TYPE": "audio"}
  ]
}*/
precision highp float;
#define PI 3.14159265359
float apeak(){return (audioBass.x+audioBass.y+audioBass.z+audioBass.w)*0.25;}
mat3 rotX(float a){float c=cos(a),s=sin(a);return mat3(1,0,0,0,c,-s,0,s,c);}
mat3 rotY(float a){float c=cos(a),s=sin(a);return mat3(c,0,s,0,1,0,-s,0,c);}
float sdTorus(vec3 p,float R,float r){return length(vec2(length(p.xz)-R,p.y))-r;}
float scene(vec3 p,float ap,float t){
    float d=length(p)-0.16;
    float thick=0.022+ap*0.008;
    float n=min(floor(ringCount),8.0);
    for(float i=0.0;i<8.0;i++){
        if(i>=n)break;
        float ph=i*PI/n;
        vec3 rp=rotX(ph+t*(0.6+i*0.12))*rotY(ph*1.618+t*0.35)*p;
        d=min(d,sdTorus(rp,0.28+i*0.13,thick));
    }
    return d;
}
vec3 nrm(vec3 p,float ap,float t){
    float e=0.001;
    return normalize(vec3(
        scene(p+vec3(e,0,0),ap,t)-scene(p-vec3(e,0,0),ap,t),
        scene(p+vec3(0,e,0),ap,t)-scene(p-vec3(0,e,0),ap,t),
        scene(p+vec3(0,0,e),ap,t)-scene(p-vec3(0,0,e),ap,t)));
}
void main(){
    vec2 uv=(isf_FragNormCoord*2.0-1.0)*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    float ap=apeak();
    float t=TIME*spinSpeed;
    vec3 ro=vec3(sin(t*0.18)*2.1,cos(t*0.07)*0.55,cos(t*0.18)*2.1);
    vec3 fw=normalize(-ro);
    vec3 rt=normalize(cross(vec3(0,1,0),fw));
    vec3 up=cross(fw,rt);
    vec3 rd=normalize(uv.x*rt+uv.y*up+1.8*fw);
    float dist=0.0;bool hit=false;
    for(int i=0;i<64;i++){
        float d=scene(ro+rd*dist,ap,t);
        if(d<0.001){hit=true;break;}
        if(dist>6.0)break;
        dist+=d;
    }
    // Deep navy starfield background
    vec3 col=vec3(0.008,0.015,0.07)+vec3(0.0,0.04,0.15)*pow(max(0.0,rd.y+0.3),2.0);
    if(hit){
        vec3 p=ro+rd*dist;
        vec3 n=nrm(p,ap,t);
        vec3 L=normalize(vec3(1.4,2.0,0.8));
        float diff=max(dot(n,L),0.0);
        float spec=pow(max(dot(reflect(-L,n),-rd),0.0),48.0);
        // Gold rings vs navy inner sphere
        float isSphere=step(length(p),0.18);
        vec3 base=mix(vec3(1.0,0.75,0.10),vec3(0.06,0.12,0.50),isSphere);
        col=base*(0.08+diff*2.4)+vec3(3.0,2.5,1.2)*spec;
        // HDR electric cyan rim glow
        float rim=pow(1.0-max(dot(n,-rd),0.0),3.5);
        col+=vec3(0.1,1.8,3.2)*rim*(1.3+ap);
        // fwidth AA on longitude ring-lines (surface decoration)
        float phi=atan(p.z,p.x);
        float lineVal=fract(phi/(PI*2.0)*10.0+0.5);
        float lineAA=fwidth(phi/(PI*2.0)*10.0);
        float lineEdge=smoothstep(lineAA,0.0,min(lineVal,1.0-lineVal)-0.05);
        col*=1.0-0.35*lineEdge*(1.0-isSphere);
    }
    gl_FragColor=vec4(col,1.0);
}
