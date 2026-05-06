/*{
  "DESCRIPTION": "Hydrothermal Vent — 3D deep-sea vent plume, cobalt ocean vs crimson heat glow",
  "CREDIT": "ShaderClaw3 auto-improve v17",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME": "plumeSpeed", "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0},
    {"NAME": "heatPeak",   "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 5.0},
    {"NAME": "audioMod",   "TYPE": "audio"}
  ]
}*/
precision highp float;
float apeak(){return (audioBass.x+audioBass.y+audioBass.z+audioBass.w)*0.25;}
float hash2(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5);}
float noise2(vec2 p){vec2 i=floor(p),f=fract(p);f=f*f*(3.0-2.0*f);
    return mix(mix(hash2(i),hash2(i+vec2(1,0)),f.x),mix(hash2(i+vec2(0,1)),hash2(i+vec2(1,1)),f.x),f.y);}
float fbm2(vec2 p){float v=0.0,a=0.5;for(int i=0;i<4;i++){v+=a*noise2(p);p*=2.1;a*=0.5;}return v;}
float sdCylinder(vec3 p,float r,float h){
    vec2 d=abs(vec2(length(p.xz)-r,p.y))-vec2(0.0,h);
    return min(max(d.x,d.y),0.0)+length(max(d,0.0));}
float ventPlume(vec3 p,float t){
    float w=fbm2(vec2(p.y*0.9-t*plumeSpeed,p.z*0.4))*0.22;
    float w2=fbm2(vec2(p.z*0.9+t*plumeSpeed*0.8,p.y*0.4))*0.22;
    vec2 xz=p.xz-vec2(w,w2);
    return length(xz)-(0.05+max(0.0,p.y)*0.10);}
float mapScene(vec3 p,float t){
    return min(min(sdCylinder(p-vec3(0,-1.5,0),0.08,0.5),ventPlume(p-vec3(0,-0.95,0),t)),p.y+1.65);}
vec3 calcNorm(vec3 p,float t){float e=0.002;
    return normalize(vec3(
        mapScene(p+vec3(e,0,0),t)-mapScene(p-vec3(e,0,0),t),
        mapScene(p+vec3(0,e,0),t)-mapScene(p-vec3(0,e,0),t),
        mapScene(p+vec3(0,0,e),t)-mapScene(p-vec3(0,0,e),t)));}
void main(){
    vec2 uv=(isf_FragNormCoord*2.0-1.0)*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    float ap=apeak();float t=TIME;
    vec3 ro=vec3(sin(t*0.09)*1.1,0.2,cos(t*0.09)*1.1+1.1);
    vec3 fw=normalize(vec3(0,-0.5,0)-ro);
    vec3 rt=normalize(cross(vec3(0,1,0),fw));
    vec3 rd=normalize(uv.x*rt+uv.y*cross(fw,rt)+1.6*fw);
    float dist=0.0;bool hit=false;int hitType=0;
    for(int i=0;i<64;i++){
        vec3 p=ro+rd*dist;
        float dV=ventPlume(p-vec3(0,-0.95,0),t);
        float dC=sdCylinder(p-vec3(0,-1.5,0),0.08,0.5);
        float dF=p.y+1.65;
        float d=min(min(dV,dC),dF);
        if(d<0.001){hit=true;hitType=(dV<dC&&dV<dF)?0:(dC<dF?1:2);break;}
        if(dist>5.0)break;dist+=d*0.7;}
    // Deep cobalt ocean bg
    vec3 col=mix(vec3(0.0,0.04,0.18),vec3(0.0,0.08,0.35),smoothstep(-0.5,0.5,uv.y));
    col+=vec3(0.0,0.12,0.4)*pow(max(0.0,noise2(uv*8.0+TIME*0.3)-0.7),2.0)*0.5;
    if(hit){
        vec3 p=ro+rd*dist;
        vec3 n=calcNorm(p,t);
        vec3 L=normalize(vec3(0,1,0.2));
        float diff=max(dot(n,L),0.0);
        float spec=pow(max(dot(reflect(-L,n),-rd),0.0),32.0);
        if(hitType==0){
            float heat=smoothstep(0.0,1.5,p.y+0.95)*(1.0+ap*0.4);
            vec3 pc=heat<0.5?mix(vec3(0.5,0.01,0.0),vec3(1.0,0.3,0.0),heat*2.0):
                             mix(vec3(1.0,0.3,0.0),vec3(2.8,2.2,0.4),(heat-0.5)*2.0);
            col=pc*heatPeak*(0.4+diff*0.6)*(1.0+ap*0.5)+vec3(3.5,2.5,0.5)*spec;
        } else if(hitType==1){
            col=vec3(0.08,0.04,0.02)*(0.2+diff)+vec3(1.5,0.4,0.0)*spec*0.5;
            col+=vec3(0.8,0.15,0.0)*0.3*(1.0-diff);
        } else {
            col=vec3(0.04,0.06,0.12)*fbm2(p.xz*4.0)*3.0+vec3(0.0,0.5,1.2)*spec*0.4;}
        float ea=fwidth(dot(n,-rd));
        col*=0.7+0.3*smoothstep(0.0,ea,dot(n,-rd));}
    col+=vec3(1.5,0.3,0.0)*0.05*(1.0+ap)*max(0.0,1.0-length(uv-vec2(0,-0.12))*2.5);
    gl_FragColor=vec4(col,1.0);}
