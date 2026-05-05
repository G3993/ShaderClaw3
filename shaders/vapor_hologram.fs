/*{
  "DESCRIPTION": "Sacred Torus Portal — 3D raymarched torus knot portal ring; gold/violet inscribed geometry pulsing in void-black; completely different from vaporwave v1/v2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"knotP","TYPE":"float","DEFAULT":2.0,"MIN":1.0,"MAX":5.0,"LABEL":"Knot P"},
    {"NAME":"knotQ","TYPE":"float","DEFAULT":3.0,"MIN":1.0,"MAX":5.0,"LABEL":"Knot Q"},
    {"NAME":"tubeRadius","TYPE":"float","DEFAULT":0.08,"MIN":0.03,"MAX":0.18,"LABEL":"Tube Radius"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"}
  ]
}*/

#define TAU 6.28318530718
#define STEPS 80

// Torus knot parametric curve point
vec3 torusKnotPoint(float t,float p,float q){
    float r=cos(q*t)+2.;
    return vec3(r*cos(p*t),r*sin(p*t),sin(q*t)*-1.);
}

// Torus knot SDF: sample N points along curve, find min capsule
float sdTorusKnot(vec3 pos,float p,float q,float r){
    float minD=1e9;
    int N=72;
    vec3 prev=torusKnotPoint(0.,p,q)*.35;
    for(int i=1;i<=N;i++){
        float t=float(i)/float(N)*TAU;
        vec3 cur=torusKnotPoint(t,p,q)*.35;
        // Capsule SDF
        vec3 ab=cur-prev,ap=pos-prev;
        float h=clamp(dot(ap,ab)/dot(ab,ab),0.,1.);
        float d=length(ap-ab*h)-r;
        minD=min(minD,d);
        prev=cur;
    }
    return minD;
}

vec2 scene(vec3 p){
    float d=sdTorusKnot(p,knotP,knotQ,tubeRadius);
    return vec2(d,1.);
}

vec3 calcNor(vec3 p){
    vec2 e=vec2(.002,0.);
    return normalize(vec3(
        scene(p+e.xyy).x-scene(p-e.xyy).x,
        scene(p+e.yxy).x-scene(p-e.yxy).x,
        scene(p+e.yyx).x-scene(p-e.yyx).x
    ));
}

vec3 knotColor(vec3 pos,float t){
    // Color based on angle around knot — gold/violet/crimson cycle
    float ang=atan(pos.z,pos.x)+pos.y*.8+t*.15;
    float v=fract(ang/(TAU));
    vec3 gold=vec3(1.,.75,0.);
    vec3 violet=vec3(.6,0.,1.);
    vec3 crimson=vec3(1.,.08,.1);
    if(v<.33) return mix(gold,violet,v*3.);
    if(v<.67) return mix(violet,crimson,(v-.33)*3.);
    return mix(crimson,gold,(v-.67)*3.);
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.7;

    // Camera orbiting
    float ang=t*.06;
    float elev=.3+.15*sin(t*.11);
    vec3 ro=vec3(sin(ang)*cos(elev)*3.0,sin(elev)*3.0,cos(ang)*cos(elev)*3.0);
    vec3 fwd=normalize(-ro);
    vec3 right=normalize(cross(fwd,vec3(0.,1.,0.)));
    vec3 up_=cross(right,fwd);
    vec3 rd=normalize(fwd+uv.x*right+uv.y*up_);

    // Void black background
    vec3 col=vec3(.002,.001,.004);

    float d=.1; float matId=-1.;
    for(int i=0;i<STEPS;i++){
        vec2 r=scene(ro+rd*d);
        if(r.x<.001){matId=r.y;break;}
        if(d>10.)break;
        d+=r.x*.9;
    }

    if(matId>=0.){
        vec3 pos=ro+rd*d;
        vec3 n=calcNor(pos);
        vec3 kc=knotColor(pos,t);
        float diff=max(0.,dot(n,normalize(vec3(.5,1.,.3))))*.25+.7;
        // Specular shimmer
        float spec=pow(max(0.,dot(reflect(-normalize(vec3(.5,1.,.3)),n),-rd)),24.)*.4;
        col=kc*diff*glowPeak*audio+vec3(1.,1.,1.)*spec*glowPeak*.3*audio;
        float edge=fwidth(scene(pos).x);
        col*=smoothstep(0.,edge*5.,scene(pos).x+.001);
    }

    // Inner portal glow: the space inside the knot loop glows faintly violet
    // Computed as screen-space distance from center
    float portalGlow=exp(-length(uv)*3.)*glowPeak*.05*audio;
    col+=vec3(.4,0.,.8)*portalGlow;

    // Outer glow halo
    float halo=exp(-max(0.,length(uv)-1.1)*4.)*glowPeak*.06*audio;
    col+=mix(vec3(1.,.75,0.),vec3(.6,0.,1.),sin(t*.3)*.5+.5)*halo;

    col*=1.-smoothstep(.6,.95,length(uv)*.85);
    gl_FragColor=vec4(col,1.);
}
