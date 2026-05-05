/*{
  "DESCRIPTION": "Neon Orbit Diagram — concentric planetary orbits with glowing dot-planets. Cosmic orrery. v4: vs v3 arctic cave / v2 3D hyperspace / v1 2D starfield.",
  "CATEGORIES": ["Generator"],
  "CREDIT": "ShaderClaw auto-improve v4",
  "INPUTS": [
    {"NAME":"planets",  "TYPE":"float","DEFAULT":5.0,"MIN":2.0,"MAX":8.0},
    {"NAME":"orbitSpd", "TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":2.0},
    {"NAME":"hdrBoost", "TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0},
    {"NAME":"audioMod", "TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":2.0}
  ]
}*/
float h11(float n){return fract(sin(n*127.1)*43758.5453);}
void main(){
    vec2 uv=(isf_FragNormCoord-0.5)*2.2; uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    float t=TIME*orbitSpd; float audio=1.0+(audioLevel+audioBass*0.4)*audioMod*0.22;
    vec3 GOLD=vec3(2.5,1.8,0.0)*hdrBoost*audio;
    vec3 ORANGE=vec3(2.4,0.6,0.0)*hdrBoost*audio;
    vec3 CYAN=vec3(0.0,2.4,2.2)*hdrBoost*audio;
    vec3 VIOLET=vec3(1.4,0.0,2.5)*hdrBoost*audio;
    vec3 BG=vec3(0,0,0.01);
    float r=length(uv); float theta=atan(uv.y,uv.x);
    float starNoise=fract(sin(floor(r*18.0)*127.1+floor(theta*9.0)*311.7)*43758.5);
    vec3 col=BG+vec3(0.8,0.9,1.0)*step(0.97,starNoise)*0.3*hdrBoost;
    float sunD=length(uv)-0.08; float sunFw=fwidth(sunD);
    col+=GOLD*exp(-max(sunD,0.0)*8.0);
    col=mix(col,GOLD,smoothstep(sunFw,-sunFw,sunD));
    int N=int(clamp(planets,2.0,8.0));
    for(int pi=0;pi<8;pi++){
        if(pi>=N)break; float fi=float(pi);
        float s1=h11(fi*1.37),s2=h11(fi*2.91);
        float orbitR=0.18+fi*0.12+s1*0.04;
        float angle=t*(0.4+s2*1.2)+fi*1.25;
        float pSize=0.018+s1*0.02;
        float orbitD=abs(length(uv)-orbitR); float orbitFw=fwidth(orbitD);
        vec2 pPos=orbitR*vec2(cos(angle),sin(angle));
        float pD=length(uv-pPos)-pSize; float pFw=fwidth(pD);
        float warmness=1.0-fi/8.0;
        vec3 pc=mix(CYAN,ORANGE,warmness*warmness);
        if(pi==2||pi==5)pc=VIOLET;
        col+=pc*smoothstep(orbitFw*1.5,0.0,orbitD-0.005)*0.18;
        col+=pc*exp(-max(pD,0.0)*18.0)*0.8;
        col=mix(col,pc,smoothstep(pFw,-pFw,pD));
        float trailAng=angle-0.4;
        for(int tj=0;tj<5;tj++){
            float fa=float(tj); float ta=trailAng-fa*0.08;
            vec2 tp=orbitR*vec2(cos(ta),sin(ta));
            float td=length(uv-tp)-(pSize*0.6);
            col+=pc*exp(-max(td,0.0)*25.0)*(1.0-fa*0.2)*0.25;
        }
    }
    gl_FragColor=vec4(col,1.0);
}
