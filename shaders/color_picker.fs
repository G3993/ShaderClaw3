/*{
  "DESCRIPTION": "Constructivist Tower — 3D raymarched Suprematist architecture in saturated crimson, jet black, and cadmium gold. El Lissitzky — Proun series.",
  "CATEGORIES": ["Generator", "3D"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"rotSpd",  "LABEL":"Rotation",  "TYPE":"float","DEFAULT":0.25,"MIN":0.0,"MAX":2.0},
    {"NAME":"hdrBoost","LABEL":"HDR Peak",  "TYPE":"float","DEFAULT":2.5, "MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod","LABEL":"Audio",     "TYPE":"float","DEFAULT":1.0, "MIN":0.0,"MAX":2.0}
  ]
}*/
float sdBox(vec3 p, vec3 b) { vec3 q=abs(p)-b; return length(max(q,0.0))+min(max(q.x,max(q.y,q.z)),0.0); }
float sdCyl(vec3 p, float r, float h) { vec2 d=abs(vec2(length(p.xz),p.y))-vec2(r,h); return min(max(d.x,d.y),0.0)+length(max(d,0.0)); }
mat2 rot2(float a){float c=cos(a),s=sin(a);return mat2(c,-s,s,c);}
float scene(vec3 p){
    p.xz=rot2(TIME*rotSpd)*p.xz;
    float d=sdBox(p-vec3(0,-2.3,0),vec3(1.1,0.12,1.1));
    d=min(d,sdBox(p-vec3(0,-1.2,0),vec3(0.30,1.0,0.30)));
    d=min(d,sdBox(p-vec3(0,-0.15,0),vec3(0.85,0.09,0.85)));
    vec3 pa=p-vec3(0.5,0.2,0); pa.xy=rot2(0.785)*pa.xy;
    d=min(d,sdBox(pa,vec3(0.07,0.55,0.07)));
    d=min(d,sdBox(p-vec3(0,0.95,0),vec3(0.20,0.80,0.20)));
    d=min(d,sdBox(p-vec3(0,1.72,0),vec3(0.55,0.07,0.55)));
    d=min(d,sdCyl(p-vec3(0,2.25,0),0.22,0.25));
    d=min(d,sdBox(p-vec3(-0.65,-0.4,0),vec3(0.14,0.28,0.14)));
    d=min(d,sdBox(p-vec3(0.62,0.45,0),vec3(0.10,0.18,0.10)));
    return d;
}
vec3 getNormal(vec3 p){const vec2 e=vec2(0.001,-0.001);return normalize(e.xyy*scene(p+e.xyy)+e.yyx*scene(p+e.yyx)+e.yxy*scene(p+e.yxy)+e.xxx*scene(p+e.xxx));}
vec3 palette(vec3 p,float hdrB){
    float ny=(p.y+2.5)/5.0;
    if(ny<0.12)return vec3(2.5,0.02,0.01)*hdrB;
    if(ny<0.30)return vec3(0.01,0.01,0.01);
    if(ny<0.42)return vec3(2.3,1.6,0.02)*hdrB;
    if(ny<0.62)return vec3(2.5,0.02,0.01)*hdrB;
    if(ny<0.78)return vec3(0.01,0.01,0.01);
    if(ny<0.90)return vec3(2.3,1.6,0.02)*hdrB;
    return vec3(2.5,2.3,2.0)*hdrB;
}
void main(){
    vec2 uv=isf_FragNormCoord*2.0-1.0; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    float audio=1.0+(audioLevel+audioBass*0.5)*audioMod*0.2;
    vec3 ro=vec3(0,0.3,5.5); vec3 rd=normalize(vec3(uv.x,uv.y*0.9-0.05,-1.6));
    vec3 ld=normalize(vec3(1.5,2.5,1.0));
    float dist=0.0; bool hit=false; vec3 pos=ro;
    for(int i=0;i<80;i++){ float d=scene(pos); if(d<0.001){hit=true;break;} if(dist>12.0)break; dist+=max(d*0.9,0.003); pos=ro+rd*dist; }
    float stripe=step(0.96,fract((uv.x-uv.y*0.6+TIME*0.02)*3.0));
    vec3 bg=vec3(0.04,0.02,0.02)+vec3(0.18,0,0)*stripe;
    vec3 col=bg;
    if(hit){ vec3 N=getNormal(pos); vec3 bc=palette(pos,hdrBoost*audio); float diff=clamp(dot(N,ld),0,1); float fill=clamp(dot(N,normalize(vec3(-1,1,-0.5))),0,1)*0.22; float spec=pow(clamp(dot(reflect(-ld,N),-rd),0,1),32.0)*0.9; float ink=1.0-smoothstep(0.0,0.32,dot(N,-rd)); col=bc*(diff+fill+0.12)+vec3(3,2.5,1.5)*spec; col*=1.0-ink*0.94; }
    gl_FragColor=vec4(col,1.0);
}
