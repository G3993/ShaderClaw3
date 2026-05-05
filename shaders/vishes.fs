/*{
  "DESCRIPTION": "Rhodonea Spirograph — evolving polar rose curves (r=cos(k*theta)) as glowing neon lines. v3: mathematical polar curves vs v2 3D coral / v1 cellular walker.",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw auto-improve v3",
  "INPUTS": [
    {"NAME":"curveCount","TYPE":"float","DEFAULT":5.0,"MIN":1.0,"MAX":7.0},
    {"NAME":"evolution", "TYPE":"float","DEFAULT":0.12,"MIN":0.0,"MAX":1.0},
    {"NAME":"hdrBoost",  "TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod",  "TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
float h11(float n){return fract(sin(n*127.1)*43758.5453);}
void main(){
    vec2 uv=(isf_FragNormCoord-0.5)*2.1; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    float t=TIME*evolution; float audio=1.0+(audioLevel+audioBass*0.5)*audioMod*0.22;
    vec3 CYAN=vec3(0.0,2.5,2.3)*hdrBoost*audio;
    vec3 MAGENTA=vec3(2.5,0.05,1.8)*hdrBoost*audio;
    vec3 GOLD=vec3(2.4,1.7,0.0)*hdrBoost*audio;
    vec3 VIOLET=vec3(1.5,0.0,2.5)*hdrBoost*audio;
    vec3 BG=vec3(0,0,0.012);
    float r=length(uv); float theta=atan(uv.y,uv.x);
    int N=int(clamp(curveCount,1.0,7.0)); vec3 col=BG;
    for(int ci=0;ci<7;ci++){
        if(ci>=N)break; float fi=float(ci);
        float s1=h11(fi*1.37),s2=h11(fi*2.91),s3=h11(fi*4.17);
        float k=floor(s1*4.0)+2.0; float scale=0.55+s2*0.3;
        float phase=t*(0.2+s3*0.8)+fi*0.8;
        float rRose=scale*abs(cos(k*(theta+phase)));
        float d=abs(r-rRose);
        float fw=fwidth(d);
        float glow=exp(-d*16.0)*0.55;
        float line=smoothstep(fw*1.5,-fw*0.5,d-0.014);
        int cii=int(mod(fi,4.0));
        vec3 cc=(cii==0)?CYAN:(cii==1)?MAGENTA:(cii==2)?GOLD:VIOLET;
        col+=cc*glow;
        col=mix(col,cc,line);
        col=mix(col,BG,smoothstep(fw*3.0,0.0,abs(d))*line*0.5);
    }
    gl_FragColor=vec4(col,1.0);
}
