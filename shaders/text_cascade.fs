/*{
  "DESCRIPTION": "Matrix Code Rain — falling columns of electric green glyphs on void black. v4: cyber aesthetic vs v2 3D bioluminescent cave / v1 2D aurora.",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"density",  "TYPE":"float","DEFAULT":0.5,"MIN":0.1,"MAX":1.0},
    {"NAME":"fallSpeed","TYPE":"float","DEFAULT":0.8,"MIN":0.1,"MAX":3.0},
    {"NAME":"hdrBoost", "TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod", "TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
float h11(float n){return fract(sin(n*127.1)*43758.5453);}
float h21(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}
void main(){
    vec2 uv=isf_FragNormCoord; float t=TIME*fallSpeed;
    float audio=1.0+(audioLevel+audioBass*0.4)*audioMod*0.22;
    vec3 G_HOT=vec3(0.15,2.8,0.4)*hdrBoost*audio;
    vec3 G_MID=vec3(0.03,1.5,0.18)*hdrBoost*audio;
    vec3 G_DIM=vec3(0.01,0.5,0.06)*hdrBoost*audio;
    vec3 W_LEAD=vec3(2.8,3.2,2.8)*hdrBoost*audio;
    vec3 BG=vec3(0);
    float cols=mix(20.0,60.0,density); float colW=1.0/cols;
    float col_i=floor(uv.x/colW);
    float spd=0.4+h11(col_i*3.17)*1.2; float ph=h11(col_i*7.31);
    float active=step(0.35,h11(col_i*13.7+audioBass*audioMod));
    float leaderY=fract(t*spd*0.14+ph);
    float cellH=colW*1.6; float cell_i=floor(uv.y/cellH); float cellY=fract(uv.y/cellH);
    float totalCells=1.0/cellH; float leaderCell=leaderY*totalCells;
    float dist=leaderCell-cell_i; if(dist<0.0)dist+=totalCells;
    float glyphProb=step(h21(vec2(col_i,floor(cell_i))),0.78);
    float mask=step(0.05,fract(uv.x/colW))*step(fract(uv.x/colW),0.85)*step(0.04,cellY)*step(cellY,0.78);
    vec3 gc=BG;
    if(active>0.5&&glyphProb>0.5){
        if(dist<0.5)gc=W_LEAD;
        else if(dist<1.0)gc=G_HOT*(1.0-(dist-0.5)*1.5);
        else if(dist<3.0)gc=G_MID*(1.0-(dist-1.0)*0.4);
        else if(dist<8.0)gc=G_DIM*(1.0-(dist-3.0)*0.14);
    }
    gl_FragColor=vec4(BG+gc*mask,1.0);
}
