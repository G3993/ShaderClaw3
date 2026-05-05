/*{
  "DESCRIPTION": "Solar Magnetic Arcs — 3D raymarched plasma torus-arcs around a glowing stellar core; HDR gold/orange/white against deep space",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"arcCount","TYPE":"float","DEFAULT":6.0,"MIN":2.0,"MAX":10.0,"LABEL":"Arc Count"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"plasmaSpeed","TYPE":"float","DEFAULT":0.4,"MIN":0.1,"MAX":1.5,"LABEL":"Plasma Speed"},
    {"NAME":"starSize","TYPE":"float","DEFAULT":0.45,"MIN":0.2,"MAX":0.8,"LABEL":"Star Size"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"}
  ]
}*/

float h11(float n){return fract(sin(n*127.1)*43758.5453);}

float sdSphere(vec3 p,float r){return length(p)-r;}

// Torus arc: full torus but clipped to arc length
float sdTorus(vec3 p,float R,float r){
    return length(vec2(length(p.xz)-R,p.y))-r;
}

// Arc segment of torus: limit angular range
float sdArc(vec3 p,float R,float r,float arcLen){
    // Angle in xz plane
    float ang=atan(p.z,p.x);
    float d=sdTorus(p,R,r);
    float distEnd=min(abs(ang),abs(ang-arcLen));
    distEnd=min(distEnd,abs(ang+arcLen));
    // Clamp to arc: penalize points outside angular range
    float halfArc=arcLen*.5;
    float outside=max(0.,abs(ang+3.14159-halfArc)-(halfArc+.05));
    return d+outside*0.5;
}

// One plasma arc: a rotated torus segment
float sdPlasmaArc(vec3 p,float seed,float t){
    float r1=(h11(seed)-.5)*3.14159*2.; // orbit tilt x
    float r2=(h11(seed+1.)-.5)*3.14159*2.; // orbit tilt z
    float phase=h11(seed+2.)*6.28+t*plasmaSpeed*(1.+h11(seed+3.)*.5);
    float R=.65+h11(seed+4.)*.35; // orbit radius
    float r=.04+h11(seed+5.)*.03;  // tube radius
    float arcLen=1.5+h11(seed+6.)*2.5; // arc angular span
    // Rotate arc orientation
    vec3 q=p;
    float ca=cos(r1),sa=sin(r1);
    q.xy=mat2(ca,-sa,sa,ca)*q.xy;
    float cb=cos(r2),sb=sin(r2);
    q.yz=mat2(cb,-sb,sb,cb)*q.yz;
    // Rotate arc around star (phase animation)
    float cp=cos(phase),sp=sin(phase);
    q.xz=mat2(cp,-sp,sp,cp)*q.xz;
    return sdTorus(q,R,r);
}

vec2 scene(vec3 p){
    // Stellar core — hot sphere
    float star=sdSphere(p,starSize);
    vec2 res=vec2(star,99.); // mat 99 = star

    // Plasma arcs
    int N=int(min(arcCount,10.));
    for(int i=0;i<10;i++){
        if(i>=N)break;
        float s=float(i)*4.7;
        float d=sdPlasmaArc(p,s,TIME);
        if(d<res.x) res=vec2(d,float(i));
    }
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

vec3 plasmaCol(float id,float t){
    float phase=h11(id)*6.28+t*.2;
    // Cycle: white-hot → gold → orange → crimson
    float v=.5+.5*sin(phase);
    if(v<.33)  return mix(vec3(1.,.3,0.),vec3(1.,.65,0.),v*3.);
    if(v<.67)  return mix(vec3(1.,.65,0.),vec3(1.,.95,.2),v*3.-1.);
    return mix(vec3(1.,.95,.2),vec3(1.,1.,1.),v*3.-2.);
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.7;

    // Slow camera arc
    float ang=t*.04+1.1;
    float elev=.45+.15*sin(t*.09);
    vec3 ro=vec3(sin(ang)*cos(elev)*2.5,sin(elev)*2.5,cos(ang)*cos(elev)*2.5);
    vec3 fwd=normalize(-ro);
    vec3 right=normalize(cross(fwd,vec3(0.,1.,0.)));
    vec3 up_=cross(right,fwd);
    vec3 rd=normalize(fwd+uv.x*right+uv.y*up_);

    // Deep space background
    vec3 col=vec3(.002,.001,.005);
    // Subtle star field
    float sf=h11(floor(uv.x*300.)*317.+floor(uv.y*300.)*139.);
    if(sf>.97) col=vec3(sf*sf*.6);

    float d=.1; float matId=-1.;
    for(int i=0;i<64;i++){
        vec2 r=scene(ro+rd*d);
        if(r.x<.0015){matId=r.y;break;}
        if(d>8.)break;
        d+=r.x;
    }

    if(matId>=0.){
        vec3 pos=ro+rd*d;
        vec3 n=calcNor(pos);
        if(matId>90.){
            // Star surface: white-gold gradient + granulation
            float gran=.5+.5*sin(pos.x*18.+TIME*.8)*sin(pos.y*14.-TIME*.5)*sin(pos.z*16.+TIME*.6);
            vec3 starCol=mix(vec3(1.,.9,.2),vec3(1.,1.,1.),gran*.5);
            float limb=max(0.,dot(n,-fwd));
            starCol*=.4+.6*limb;
            col=starCol*glowPeak*2.0*audio;
        } else {
            vec3 bc=plasmaCol(matId,t);
            // Bright plasma tube with edge-darkening ink outline
            float diff=max(0.,dot(n,normalize(vec3(0.,1.,.5))))*.2+.7;
            col=bc*diff*glowPeak*audio;
            float edge=fwidth(scene(pos).x);
            col*=smoothstep(0.,edge*5.,scene(pos).x+.001);
        }
    }

    // Stellar corona glow (screen-space halo around star)
    vec3 oc=ro; // star at origin
    float b=dot(oc,rd);
    float cl2=max(0.,dot(oc,oc)-b*b);
    float corona=exp(-cl2*3.)*glowPeak*.4*audio;
    col+=mix(vec3(1.,.7,.1),vec3(1.,.3,0.),corona*.3)*corona;

    col*=1.-smoothstep(.6,1.,length(uv)*.85);
    gl_FragColor=vec4(col,1.);
}
