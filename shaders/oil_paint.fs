/*{
  "DESCRIPTION": "Oil Paint Impasto — 3D smooth-union metaballs as paint globs. Umber/cadmium red/cobalt blue/cadmium yellow. v4: 3D spheroid blobs vs prior 2D FBM brush / 3D lava plane.",
  "CATEGORIES": ["Generator","3D"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"blobs",   "TYPE":"float","DEFAULT":8.0,"MIN":3.0,"MAX":12.0},
    {"NAME":"speed",   "TYPE":"float","DEFAULT":0.3,"MIN":0.0,"MAX":2.0},
    {"NAME":"hdrBoost","TYPE":"float","DEFAULT":2.3,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
float h11(float n){return fract(sin(n*127.1)*43758.5453);}
float smin(float a,float b,float k){float h=max(k-abs(a-b),0.0)/k;return min(a,b)-h*h*k*0.25;}
float scene(vec3 p){
    float t=TIME*speed; float audio=1.0+(audioLevel+audioBass*0.4)*audioMod*0.2;
    int N=int(clamp(blobs,3.0,12.0)); float d=1e8;
    for(int i=0;i<12;i++){
        if(i>=N)break; float fi=float(i);
        float s1=h11(fi*1.37),s2=h11(fi*2.91),s3=h11(fi*4.17);
        vec3 c=vec3(sin(t*(0.2+s1*0.5)+s1*6.28)*0.60,cos(t*(0.15+s2*0.4)+s2*6.28)*0.50,sin(t*(0.18+s3*0.35)+s3*6.28)*0.40);
        float r=(0.12+s1*0.18)*audio;
        d=smin(d,length(p-c)-r,0.25);
    }
    return d;
}
vec3 getNormal(vec3 p){const vec2 e=vec2(0.001,-0.001);return normalize(e.xyy*scene(p+e.xyy)+e.yyx*scene(p+e.yyx)+e.yxy*scene(p+e.yxy)+e.xxx*scene(p+e.xxx));}
vec3 paintCol(vec3 p,float hdrB){
    float ph=fract(sin(p.x*3.1+p.y*5.7+p.z*2.3)*43758.5);
    if(ph<0.28)return vec3(1.9,0.55,0.10)*hdrB;
    if(ph<0.55)return vec3(2.4,0.08,0.04)*hdrB;
    if(ph<0.78)return vec3(0.05,0.25,2.5)*hdrB;
    return vec3(2.3,1.8,0.04)*hdrB;
}
void main(){
    vec2 uv=isf_FragNormCoord*2.0-1.0; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    vec3 ro=vec3(0,0,2.8); vec3 rd=normalize(vec3(uv.x,uv.y,-1.8));
    vec3 ld=normalize(vec3(1.2,2.0,1.0)); vec3 rl=normalize(vec3(-1,0.3,-1));
    float dist=0.0; bool hit=false; vec3 pos=ro;
    for(int i=0;i<64;i++){float d=scene(pos);if(d<0.002){hit=true;break;}if(dist>8.0)break;dist+=max(d*0.85,0.003);pos=ro+rd*dist;}
    vec3 bg=vec3(0.18,0.12,0.08)*0.5; vec3 col=bg;
    if(hit){
        vec3 N=getNormal(pos); vec3 bc=paintCol(pos,hdrBoost);
        float diff=clamp(dot(N,ld),0,1); float rim=clamp(dot(N,rl),0,1)*0.3;
        float spec=pow(clamp(dot(reflect(-ld,N),-rd),0,1),20.0)*0.7;
        float ndv=dot(N,-rd);
        col=bc*(diff*0.85+rim+0.18)+vec3(2.8,2.2,1.6)*spec;
        col*=mix(1.0,0.55,1.0-smoothstep(0.0,0.30,ndv));
    }
    gl_FragColor=vec4(col,1.0);
}
