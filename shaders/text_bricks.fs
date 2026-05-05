/*{
  "DESCRIPTION": "Brutalist Concrete — 2D concrete panels with angular shadows and amber construction light. v4: flat abstract Brutalism vs v2 3D gothic corridor / v1 neon bg.",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"panelScale","TYPE":"float","DEFAULT":1.0,"MIN":0.3,"MAX":3.0},
    {"NAME":"lightAng",  "TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":6.28},
    {"NAME":"hdrBoost",  "TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod",  "TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
float h11(float n){return fract(sin(n*127.1)*43758.5453);}
mat2 rot2(float a){float c=cos(a),s=sin(a);return mat2(c,-s,s,c);}
float sdRect(vec2 p,vec2 b){vec2 d=abs(p)-b;return length(max(d,0.0))+min(max(d.x,d.y),0.0);}
void main(){
    vec2 uv=(isf_FragNormCoord-0.5)*2.0; uv.x*=RENDERSIZE.x/RENDERSIZE.y; uv/=panelScale;
    float t=TIME*0.08; float audio=1.0+(audioLevel+audioBass*0.4)*audioMod*0.2;
    vec3 CONCRETE=vec3(0.65,0.60,0.55);
    vec3 SHADOW=vec3(0.01,0.01,0.01);
    vec3 AMBER=vec3(2.5,1.2,0.05)*hdrBoost*audio;
    vec3 CREAM=vec3(2.2,2.0,1.6)*hdrBoost*audio;
    vec3 RUST=vec3(2.0,0.4,0.05)*hdrBoost*audio;
    float la=lightAng+sin(t)*0.15; vec2 ld=normalize(vec2(cos(la),sin(la)));
    vec3 col=SHADOW; float inkAcc=0.0;
    for(int i=0;i<8;i++){
        float fi=float(i);
        float s1=h11(fi*1.37),s2=h11(fi*2.91),s3=h11(fi*4.17),s4=h11(fi*7.53);
        vec2 ctr=vec2(s1*2.8-1.4,s2*2.4-1.2); vec2 sz=vec2(0.25+s3*0.55,0.15+s4*0.45);
        float ang=(s3-0.5)*1.0+t*(s4-0.5)*0.05;
        vec2 lp=rot2(ang)*(uv-ctr); float d=sdRect(lp,sz);
        float fw=fwidth(d);
        float nx=cos(ang),ny=sin(ang); vec2 faceN=vec2(nx,ny);
        float lit=clamp(dot(faceN,ld),0.0,1.0);
        vec3 faceCol;
        if(h11(fi*31.7)<0.5){ faceCol=mix(CONCRETE,CREAM,lit*0.7+0.15); }
        else { faceCol=mix(SHADOW,RUST,lit*0.5+0.1); }
        faceCol+=AMBER*lit*lit*0.4;
        col=mix(col,faceCol,smoothstep(fw,-fw,d));
        inkAcc=max(inkAcc,smoothstep(fw*2.0,0.0,abs(d)));
    }
    col=mix(col,SHADOW,clamp(inkAcc*0.8,0.0,0.95));
    gl_FragColor=vec4(col,1.0);
}
