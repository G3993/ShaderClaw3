/*{
  "DESCRIPTION": "Neon DNA Helix — 3D raymarched double helix; hot cyan/magenta strands with gold rungs rotating in void-black; completely different from aurora/ocean/magma prior versions",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v5",
  "INPUTS": [
    {"NAME":"helixPitch","TYPE":"float","DEFAULT":0.8,"MIN":0.3,"MAX":2.0,"LABEL":"Helix Pitch"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"spinSpeed","TYPE":"float","DEFAULT":0.2,"MIN":0.0,"MAX":0.8,"LABEL":"Spin Speed"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"},
    {"NAME":"rungCount","TYPE":"float","DEFAULT":12.0,"MIN":4.0,"MAX":20.0,"LABEL":"Rungs"}
  ]
}*/

#define TAU 6.28318530718

float h11(float n){return fract(sin(n*127.1)*43758.5453);}

float sdCapsule(vec3 p,vec3 a,vec3 b,float r){
    vec3 ab=b-a,ap=p-a;
    return length(ap-ab*clamp(dot(ap,ab)/dot(ab,ab),0.,1.))-r;
}
float sdSphere(vec3 p,float r){return length(p)-r;}
float smin(float a,float b,float k){float h=clamp(.5+.5*(b-a)/k,0.,1.);return mix(b,a,h)-k*h*(1.-h);}

// Helix strand SDF: one full-turn sampled at N points
float sdStrand(vec3 p,float phase,float r,float t){
    float minD=1e9;
    float pitch=helixPitch*.8;
    int NS=24;
    vec3 prev=vec3(0.);
    for(int i=0;i<=NS;i++){
        float f=float(i)/float(NS);
        float ang=f*TAU*2.+phase+t*spinSpeed;
        float y=(f-.5)*pitch*2.;
        float strandR=.35;
        vec3 cur=vec3(cos(ang)*strandR,y,sin(ang)*strandR);
        if(i>0){
            float d=sdCapsule(p,prev,cur,r);
            minD=min(minD,d);
        }
        prev=cur;
    }
    return minD;
}

// Rung between two strand points
float sdRung(vec3 p,float rungIdx,float t){
    float pitch=helixPitch*.8;
    float totalLen=2.*pitch;
    float f=(rungIdx+.5)/rungCount;
    float ang1=f*TAU*2.+t*spinSpeed;
    float ang2=ang1+3.14159;
    float y=(f-.5)*totalLen;
    float sr=.35;
    vec3 a=vec3(cos(ang1)*sr,y,sin(ang1)*sr);
    vec3 b=vec3(cos(ang2)*sr,y,sin(ang2)*sr);
    return sdCapsule(p,a,b,.018);
}

vec2 scene(vec3 p){
    float t=TIME;
    // Strand 1: cyan
    float s1=sdStrand(p,0.,.028,t);
    // Strand 2: magenta (180° offset)
    float s2=sdStrand(p,3.14159,.028,t);
    // Rungs: gold
    float rMin=1e9;
    int NR=int(min(rungCount,20.));
    for(int i=0;i<20;i++){
        if(i>=NR)break;
        float d=sdRung(p,float(i),t);
        rMin=min(rMin,d);
    }
    float strands=min(s1,s2);
    vec2 res=vec2(min(strands,rMin),0.);
    if(s1<=s2&&s1<=rMin)   res.y=1.; // cyan
    else if(s2<s1&&s2<rMin) res.y=2.; // magenta
    else                     res.y=3.; // gold rung
    return res;
}

vec3 calcNor(vec3 p){
    vec2 e=vec2(.001,0.);
    return normalize(vec3(
        scene(p+e.xyy).x-scene(p-e.xyy).x,
        scene(p+e.yxy).x-scene(p-e.yxy).x,
        scene(p+e.yyx).x-scene(p-e.yyx).x
    ));
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.7;

    // Camera: look along helix axis from slight angle
    float camAng=t*.05+1.2;
    float elev=.4+.1*sin(t*.13);
    vec3 ro=vec3(sin(camAng)*cos(elev)*2.5,sin(elev)*2.5,cos(camAng)*cos(elev)*2.5);
    vec3 fwd=normalize(-ro*.5);
    vec3 right=normalize(cross(fwd,vec3(0.,1.,0.)));
    vec3 up_=cross(right,fwd);
    vec3 rd=normalize(fwd+uv.x*right+uv.y*up_);

    // Void black background
    vec3 col=vec3(.002,.001,.004);

    float d=.05; float matId=-1.;
    for(int i=0;i<72;i++){
        vec2 r=scene(ro+rd*d);
        if(r.x<.001){matId=r.y;break;}
        if(d>8.)break;
        d+=r.x;
    }

    if(matId>=0.){
        vec3 pos=ro+rd*d;
        vec3 n=calcNor(pos);
        vec3 matCol;
        if(matId<1.5)       matCol=vec3(0.,1.,.9);   // cyan
        else if(matId<2.5)  matCol=vec3(1.,0.,.75);  // magenta
        else                matCol=vec3(1.,.8,0.);   // gold

        float diff=max(0.,dot(n,normalize(vec3(.3,1.,.5))))*.2+.75;
        col=matCol*diff*glowPeak*audio;
        float edge=fwidth(scene(pos).x);
        col*=smoothstep(0.,edge*5.,scene(pos).x+.001);
    }

    // Screen-space strand glow halos (soft volumetric feel)
    // Cyan strand glow: compute screen-space distance to helix axis at y=0
    float helixGlow=0.;
    float pitch=helixPitch*.8;
    for(int i=0;i<6;i++){
        float f=float(i)/6.;
        float ang=f*TAU*2.+TIME*spinSpeed;
        float y=(f-.5)*pitch*2.;
        float sr=.35;
        vec3 sp1=vec3(cos(ang)*sr,y,sin(ang)*sr);
        vec3 sp2=vec3(cos(ang+3.14159)*sr,y,sin(ang+3.14159)*sr);
        vec3 oc1=ro-sp1; float b1=dot(oc1,rd); float cl1=max(0.,dot(oc1,oc1)-b1*b1);
        vec3 oc2=ro-sp2; float b2=dot(oc2,rd); float cl2=max(0.,dot(oc2,oc2)-b2*b2);
        col+=vec3(0.,1.,.9)*exp(-cl1*20.)*glowPeak*.06*audio;
        col+=vec3(1.,0.,.75)*exp(-cl2*20.)*glowPeak*.06*audio;
    }

    col*=1.-smoothstep(.58,.92,length(uv)*.85);
    gl_FragColor=vec4(col,1.);
}
