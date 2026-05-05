/*{
  "DESCRIPTION": "Wireframe Cyberspace — classic 90s green wireframe terrain, first-person flying. v4: nostalgic cyberspace vs v3 VHS horror / v1+v2 3D signal planes.",
  "CATEGORIES": ["Generator","3D"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"flySpeed","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":3.0},
    {"NAME":"waveAmt", "TYPE":"float","DEFAULT":0.3,"MIN":0.0,"MAX":1.5},
    {"NAME":"hdrBoost","TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
float terrainH(float px, float pz, float t){
    return waveAmt*(sin(px*1.3+t)*0.4+sin(pz*2.1-t*0.7)*0.3+sin(px*0.7+pz*1.1+t*0.5)*0.3);
}
void main(){
    vec2 uv=isf_FragNormCoord*2.0-1.0; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    float t=TIME*flySpeed; float audio=1.0+(audioLevel+audioBass*0.4)*audioMod*0.22;
    vec3 G_HOT=vec3(0.1,2.8,0.4)*hdrBoost*audio;
    vec3 G_MID=vec3(0.05,1.6,0.2)*hdrBoost*audio;
    vec3 G_DIM=vec3(0.02,0.6,0.08)*hdrBoost*audio;
    vec3 W_NODE=vec3(0.5,3.0,0.6)*hdrBoost*audio;
    vec3 BG=vec3(0);
    vec3 ro=vec3(0,0.3,-t); vec3 rd=normalize(vec3(uv.x*0.9,uv.y*0.5-0.2,1.0));
    float horizGlow=exp(-abs(uv.y+0.18)*3.5)*0.4;
    vec3 col=BG+G_DIM*horizGlow;
    float dist=0.0; bool hit=false;
    for(int i=0;i<64;i++){
        vec3 pos=ro+rd*dist;
        float hy=terrainH(pos.x,pos.z,t);
        if(pos.y<hy){hit=true;break;}
        if(dist>25.0)break;
        dist+=max((pos.y-hy)*0.5,0.05);
    }
    if(hit){
        vec3 hp=ro+rd*dist; float cellSz=0.5;
        float gx=abs(fract(hp.x/cellSz)*2.0-1.0); float gz=abs(fract(hp.z/cellSz)*2.0-1.0);
        float lineX=smoothstep(0.08,0.0,1.0-gx); float lineZ=smoothstep(0.08,0.0,1.0-gz);
        float grid=max(lineX,lineZ); float fog=exp(-dist*0.12);
        float hy=terrainH(hp.x,hp.z,t);
        float normH=(hy/waveAmt+1.0)*0.5;
        col+=mix(G_DIM,G_HOT,normH)*grid*fog;
        col+=W_NODE*lineX*lineZ*fog*0.6;
    }
    gl_FragColor=vec4(col,1.0);
}
