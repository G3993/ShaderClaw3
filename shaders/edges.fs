/*{
  "DESCRIPTION": "Expressionist Brushstorm — 120 diagonal capsule strokes accumulate into a Kirchner-like hot palette. v4: 2D brushwork vs prior 3D torus rings / 2D bounce particles.",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"density",  "TYPE":"float","DEFAULT":90.0,"MIN":20.0,"MAX":120.0},
    {"NAME":"agitation","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":3.0},
    {"NAME":"hdrBoost", "TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod", "TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
float h11(float n){return fract(sin(n*127.1)*43758.5453);}
float stroke(vec2 p,vec2 a,vec2 b,float r){vec2 pa=p-a,ba=b-a;float h=clamp(dot(pa,ba)/dot(ba,ba),0,1);return length(pa-ba*h)-r;}
void main(){
    vec2 uv=isf_FragNormCoord*2.0-1.0; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    float t=TIME*agitation; float audio=1.0+(audioLevel+audioBass*0.5)*audioMod*0.22;
    vec3 YELL=vec3(2.6,1.8,0)*hdrBoost*audio;
    vec3 CRIM=vec3(2.5,0.04,0.02)*hdrBoost*audio;
    vec3 COBALT=vec3(0.05,0.2,2.6)*hdrBoost*audio;
    vec3 BLK=vec3(0);
    vec3 col=BLK; float inkAcc=0.0; int N=int(clamp(density,20.0,120.0));
    for(int i=0;i<120;i++){
        if(i>=N)break; float fi=float(i);
        float s1=h11(fi*1.37),s2=h11(fi*2.91),s3=h11(fi*4.17),s4=h11(fi*7.53);
        vec2 ctr=vec2(s1*2.2-1.1+sin(t*(0.07+s2*0.3)+s3*6.28)*0.12, s2*2.0-1.0+cos(t*(0.06+s3*0.25)+s1*6.28)*0.12);
        float ang=s3*3.14159+sin(t*0.25+fi*0.61)*0.25;
        float len=0.06+s4*0.16; float rad=0.006+s1*0.010;
        vec2 dir=vec2(cos(ang),sin(ang));
        float d=stroke(uv,ctr-dir*len,ctr+dir*len,rad);
        float fw=fwidth(d); float f=smoothstep(fw,-fw,d);
        float ci=h11(fi*31.7);
        vec3 sc=(ci<0.33)?YELL:(ci<0.66)?CRIM:COBALT;
        col=mix(col,sc,f); inkAcc=max(inkAcc,smoothstep(fw*2.0,0.0,abs(d)));
    }
    col=mix(col,BLK,clamp(inkAcc*0.65,0.0,0.88));
    gl_FragColor=vec4(col,1.0);
}
