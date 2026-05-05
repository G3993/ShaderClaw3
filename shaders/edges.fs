/*{
  "DESCRIPTION": "Bioluminescent Reef — 3D raymarched coral colony in abyssal void; teal/violet/cyan/magenta polyp glow on pitch-black ocean floor",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"coralScale","TYPE":"float","DEFAULT":1.0,"MIN":0.4,"MAX":2.0,"LABEL":"Scale"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"pulseRate","TYPE":"float","DEFAULT":1.0,"MIN":0.2,"MAX":3.0,"LABEL":"Pulse Rate"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"},
    {"NAME":"camSpin","TYPE":"float","DEFAULT":0.04,"MIN":0.0,"MAX":0.2,"LABEL":"Orbit Speed"}
  ]
}*/

float h11(float n){return fract(sin(n*127.1)*43758.5453);}

float sdCap(vec3 p,vec3 a,vec3 b,float r){
    vec3 ab=b-a,ap=p-a;
    return length(ap-ab*clamp(dot(ap,ab)/dot(ab,ab),0.,1.))-r;
}
float sdSph(vec3 p,float r){return length(p)-r;}
float smin(float a,float b,float k){
    float h=clamp(.5+.5*(b-a)/k,0.,1.);
    return mix(b,a,h)-k*h*(1.-h);
}

float sdCoralTree(vec3 p,float s){
    float pulse=.007*sin(TIME*pulseRate+s*1.7);
    vec3 A=vec3(0.);
    vec3 B=vec3((h11(s)-.5)*.12,.58,(h11(s+1.)-.5)*.12);
    float trunk=sdCap(p,A,B,.04);
    vec3 C=B+vec3((h11(s+2.)-.5)*.24,.20,(h11(s+3.)-.5)*.18);
    vec3 D=B+vec3((h11(s+4.)-.5)*.20,.17,(h11(s+5.)-.5)*.22);
    float br=min(sdCap(p,B,C,.024),sdCap(p,B,D,.024));
    float tips=min(sdSph(p-C,.040+pulse),sdSph(p-D,.036+pulse));
    return smin(smin(trunk,br,.05),tips,.025);
}

vec2 scene(vec3 p){
    float gnd=p.y+.92;
    vec2 res=vec2(gnd,0.);
    float cs=coralScale;
    float d;
    d=sdCoralTree((p-vec3( 0.,-.92, 0.))/cs,1.3)*cs; if(d<res.x)res=vec2(d,1.);
    d=sdCoralTree((p-vec3( .75,-.92,-.35))/cs,5.1)*cs; if(d<res.x)res=vec2(d,2.);
    d=sdCoralTree((p-vec3(-.65,-.92, .55))/cs,8.7)*cs; if(d<res.x)res=vec2(d,3.);
    d=sdCoralTree((p-vec3( .30,-.92, .90))/cs,12.4)*cs; if(d<res.x)res=vec2(d,4.);
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

vec3 coralCol(float id){
    if(id<1.5) return vec3(0.,1.,.85);
    if(id<2.5) return vec3(0.,.75,1.);
    if(id<3.5) return vec3(.65,0.,1.);
    return vec3(1.,.0,.65);
}

// Tip world positions for screen-space glow
vec3 tipPos(int i){
    float cs=coralScale;
    if(i==0){ float s=1.3; return vec3(0.,-.92,0.)  +vec3((h11(s)-.5)*.12,.58,(h11(s+1.)-.5)*.12)*cs+vec3((h11(s+2.)-.5)*.24,.20,(h11(s+3.)-.5)*.18)*cs;}
    if(i==1){ float s=5.1; return vec3(.75,-.92,-.35)+vec3((h11(s)-.5)*.12,.58,(h11(s+1.)-.5)*.12)*cs+vec3((h11(s+2.)-.5)*.24,.20,(h11(s+3.)-.5)*.18)*cs;}
    if(i==2){ float s=8.7; return vec3(-.65,-.92,.55)+vec3((h11(s)-.5)*.12,.58,(h11(s+1.)-.5)*.12)*cs+vec3((h11(s+2.)-.5)*.24,.20,(h11(s+3.)-.5)*.18)*cs;}
    float s=12.4; return vec3(.30,-.92,.90)+vec3((h11(s)-.5)*.12,.58,(h11(s+1.)-.5)*.12)*cs+vec3((h11(s+2.)-.5)*.24,.20,(h11(s+3.)-.5)*.18)*cs;
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.6;

    float ang=TIME*camSpin;
    vec3 ro=vec3(sin(ang)*2.2,.45,cos(ang)*2.2);
    vec3 fwd=normalize(vec3(0.,-.35,0.)-ro);
    vec3 right=normalize(cross(fwd,vec3(0.,1.,0.)));
    vec3 up_=cross(right,fwd);
    vec3 rd=normalize(fwd+uv.x*right+uv.y*up_);

    float d=.05; float matId=-1.;
    for(int i=0;i<72;i++){
        vec2 r=scene(ro+rd*d);
        if(r.x<.0015){matId=r.y;break;}
        if(d>12.)break;
        d+=r.x;
    }

    vec3 col=vec3(0.,.0015,.005);

    if(matId>=0.){
        vec3 pos=ro+rd*d;
        vec3 n=calcNor(pos);
        if(matId<.5){
            // ocean floor: almost black with faint teal ambient
            col=vec3(.008,.014,.018)+vec3(0.,.025,.04)*(.5+.5*sin(pos.x*.9+pos.z*.7));
        } else {
            vec3 bc=coralCol(matId);
            float diff=max(0.,dot(n,normalize(vec3(.2,1.,.3))))*.3;
            float emit=.65;
            col=bc*(emit+diff)*glowPeak*audio;
            // fwidth edge darkening — black ink silhouette at coral boundary
            float edge=fwidth(scene(pos).x);
            col*=smoothstep(0.,edge*5.,scene(pos).x+.002);
        }
    }

    // Additive screen-space polyp glow halos
    vec3 gc0=vec3(0.,1.,.85); vec3 gc1=vec3(0.,.75,1.);
    vec3 gc2=vec3(.65,0.,1.);  vec3 gc3=vec3(1.,0.,.65);
    for(int i=0;i<4;i++){
        vec3 tp=tipPos(i);
        vec3 gc=(i==0)?gc0:(i==1)?gc1:(i==2)?gc2:gc3;
        vec3 oc=ro-tp;
        float b=dot(oc,rd); float cl2=max(0.,dot(oc,oc)-b*b);
        col+=gc*exp(-cl2*35.)*glowPeak*.14*audio;
    }

    col*=1.-smoothstep(.55,1.,length(uv));
    gl_FragColor=vec4(col,1.);
}
