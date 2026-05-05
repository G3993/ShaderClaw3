/*{
  "DESCRIPTION": "Tron Lightgrid — 3D raymarched grid room with neon floor lines and glowing pillars. Electric blue/orange palette. v4: 3D Tron vs v2 3D dissolving cubes / v1 2D CRT / v3 solar plasma.",
  "CATEGORIES": ["Generator","3D"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"gridDensity","TYPE":"float","DEFAULT":8.0,"MIN":4.0,"MAX":20.0},
    {"NAME":"speed",      "TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":2.0},
    {"NAME":"hdrBoost",   "TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod",   "TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
float h21(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}
void main(){
    vec2 uv=isf_FragNormCoord*2.0-1.0; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    float t=TIME*speed; float audio=1.0+(audioLevel+audioBass*0.4)*audioMod*0.2;
    vec3 ELEC_BLUE=vec3(0.0,0.6,2.8)*hdrBoost*audio;
    vec3 NEON_ORG=vec3(2.6,0.7,0.0)*hdrBoost*audio;
    vec3 CYAN_GLOW=vec3(0.0,2.5,2.2)*hdrBoost*audio;
    vec3 WHITE_HOT=vec3(2.8,2.8,3.0)*hdrBoost*audio;
    vec3 BG=vec3(0,0,0.02);
    vec3 ro=vec3(0,0.4,-t*0.8); vec3 rd=normalize(vec3(uv.x,uv.y-0.15,-1.6));
    float horizBlend=exp(-abs(uv.y+0.15)*4.0)*0.5;
    vec3 col=BG+ELEC_BLUE*horizBlend*0.3;
    float tFloor=(-ro.y)/rd.y;
    if(tFloor>0.0&&tFloor<80.0){
        vec3 hp=ro+rd*tFloor; float gd=gridDensity;
        float gx=abs(fract(hp.x*gd)*2.0-1.0); float gz=abs(fract(hp.z*gd)*2.0-1.0);
        float lineW=0.06; float lineX=smoothstep(lineW,0.0,1.0-gx); float lineZ=smoothstep(lineW,0.0,1.0-gz);
        float grid=max(lineX,lineZ); float fog=exp(-tFloor*0.035);
        vec3 floorLine=mix(ELEC_BLUE,NEON_ORG,lineX/(lineX+lineZ+0.001));
        col=mix(BG,floorLine*grid,fog*grid);
        col+=WHITE_HOT*lineX*lineZ*fog*0.4;
        vec2 cellF=fract(vec2(hp.x,hp.z)*gd);
        float nearNode=max(1.0-max(abs(cellF.x-0.5),abs(cellF.y-0.5))*2.0,0.0);
        float pillarH=h21(floor(vec2(hp.x,hp.z)*gd));
        if(pillarH>0.3)col+=CYAN_GLOW*nearNode*nearNode*pillarH*fog*0.5;
    }
    float tCeil=(0.8-ro.y)/rd.y;
    if(tCeil>0.0&&tCeil<80.0){
        vec3 cp=ro+rd*tCeil; float gd=gridDensity;
        float gx=abs(fract(cp.x*gd)*2.0-1.0); float gz=abs(fract(cp.z*gd)*2.0-1.0);
        float grid=max(smoothstep(0.08,0.0,1.0-gx),smoothstep(0.08,0.0,1.0-gz));
        col+=ELEC_BLUE*grid*exp(-tCeil*0.04)*0.3;
    }
    gl_FragColor=vec4(col,1.0);
}
