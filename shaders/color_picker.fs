/*{
  "DESCRIPTION": "Ferrofluid Spikes — 3D SDF ferrofluid cone-spike grid; metallic dark steel with HDR neon oil-slick iridescence; completely different from prior prism/cathedral versions",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v4",
  "INPUTS": [
    {"NAME":"spikeCount","TYPE":"float","DEFAULT":5.0,"MIN":3.0,"MAX":9.0,"LABEL":"Grid Size"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"},
    {"NAME":"waveSpeed","TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":1.5,"LABEL":"Wave Speed"},
    {"NAME":"camElev","TYPE":"float","DEFAULT":0.4,"MIN":0.1,"MAX":0.8,"LABEL":"Cam Elevation"}
  ]
}*/

float h11(float n){return fract(sin(n*127.1)*43758.5453);}
float h21(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}

// Ferrofluid spike: cone-shaped SDF
float sdCone(vec3 p,float r,float h){
    vec2 q=vec2(length(p.xz),p.y);
    vec2 tip=vec2(0.,h);
    vec2 base=vec2(r,0.);
    vec2 e=base-tip;
    vec2 d=q-tip;
    float t_=clamp(dot(d,e)/dot(e,e),0.,1.);
    return length(d-e*t_)*sign(e.y*d.x-e.x*d.y);
}
float sdSphere(vec3 p,float r){return length(p)-r;}

// One spike: cone + small sphere base for smoothness
float sdSpike(vec3 p,float h){
    float cone=sdCone(p,h*.25,h);
    float base=sdSphere(p,h*.18);
    return min(cone,base);
}

// Spike height modulated by distance from center + time wave
float spikeH(vec2 gridPos,float t){
    float d=length(gridPos)/spikeCount;
    float wave=sin(d*6.-t*waveSpeed*3.)*(.4+.6*exp(-d*1.5));
    float audio_=1.+audioLevel*audioMod*.5;
    return (.18+wave*.2)*audio_;
}

vec2 scene(vec3 p){
    // Ground plane (ferrofluid surface)
    float gnd=p.y;
    vec2 res=vec2(gnd,0.);

    float spacing=2.2/spikeCount;
    float N=spikeCount;
    float half_=(N-1.)*.5*spacing;
    float t=TIME;

    for(int ix=0;ix<9;ix++){
        for(int iz=0;iz<9;iz++){
            if(float(ix)>=N||float(iz)>=N) break;
            vec2 gp=vec2(float(ix)-.5*(N-1.),float(iz)-.5*(N-1.));
            vec3 center=vec3(gp.x*spacing,0.,gp.y*spacing);
            float h=spikeH(gp,t);
            float d=sdSpike(p-center,h);
            if(d<res.x) res=vec2(d,float(ix*9+iz)+1.);
        }
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

// Oil-slick iridescent color: thin-film interference by view angle
vec3 iridescent(float n,float viewAngle){
    float hue=fract(n*.0137+viewAngle*.5+TIME*.03);
    vec3 teal=vec3(0.,1.,.85);
    vec3 magenta=vec3(1.,0.,.7);
    vec3 gold=vec3(1.,.8,.0);
    vec3 violet=vec3(.5,0.,1.);
    if(hue<.25) return mix(teal,magenta,hue*4.);
    if(hue<.5)  return mix(magenta,gold,(hue-.25)*4.);
    if(hue<.75) return mix(gold,violet,(hue-.5)*4.);
    return mix(violet,teal,(hue-.75)*4.);
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.7;

    float camAng=t*.04;
    float elev=camElev;
    vec3 ro=vec3(sin(camAng)*3.5*cos(elev),sin(elev)*3.5,cos(camAng)*3.5*cos(elev));
    vec3 fwd=normalize(vec3(0.,-.2,0.)-ro);
    vec3 right=normalize(cross(fwd,vec3(0.,1.,0.)));
    vec3 up_=cross(right,fwd);
    vec3 rd=normalize(fwd+uv.x*right+uv.y*up_);

    // Dark metallic background
    vec3 col=vec3(.008,.007,.01);

    float d=.05; float matId=-1.;
    for(int i=0;i<64;i++){
        vec2 r=scene(ro+rd*d);
        if(r.x<.001){matId=r.y;break;}
        if(d>8.)break;
        d+=r.x;
    }

    if(matId>=0.){
        vec3 pos=ro+rd*d;
        vec3 n=calcNor(pos);
        if(matId<.5){
            // Ferrofluid surface: dark metallic mirror
            float refl=pow(max(0.,1.-abs(dot(n,-rd))),3.);
            col=vec3(.015,.013,.02)+vec3(.03,.025,.04)*refl;
        } else {
            // Spike: metallic dark steel with iridescent oil-film coating
            float viewA=abs(dot(n,-rd));
            vec3 iri=iridescent(matId,viewA);
            // Steel base color: very dark
            vec3 steel=vec3(.04,.035,.05);
            // Specular: sharp iridescent highlight
            vec3 ldir=normalize(vec3(.5,1.,.4));
            float spec=pow(max(0.,dot(reflect(-ldir,n),-rd)),32.)*1.2;
            float diff=max(0.,dot(n,ldir))*.15+.25;
            col=mix(steel,iri,.4+.6*spec)*diff*glowPeak*audio
                +iri*spec*glowPeak*.8*audio;
            float edge=fwidth(scene(pos).x);
            col*=smoothstep(0.,edge*4.,scene(pos).x+.001);
        }
    }

    // Backlight glow from below (under ferrofluid surface) — teal
    float bgGlow=exp(-max(0.,d-1.)*1.5)*glowPeak*.04*audio;
    col+=vec3(0.,.6,.8)*bgGlow;

    col*=1.-smoothstep(.55,.9,length(uv)*.85);
    gl_FragColor=vec4(col,1.);
}
