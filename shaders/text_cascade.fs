/*{
  "DESCRIPTION": "Cathedral Light — 3D raymarched gothic interior; crimson/cobalt/gold light beams through lancet windows in deep stone shadow",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"dustFloat","TYPE":"float","DEFAULT":0.6,"MIN":0.0,"MAX":1.5,"LABEL":"Dust Motes"},
    {"NAME":"archCount","TYPE":"float","DEFAULT":3.0,"MIN":1.0,"MAX":5.0,"LABEL":"Arch Count"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"},
    {"NAME":"timeScale","TYPE":"float","DEFAULT":0.2,"MIN":0.05,"MAX":1.0,"LABEL":"Time Scale"}
  ]
}*/

float h11(float n){return fract(sin(n*127.1)*43758.5453);}
float h21(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}

float sdBox(vec3 p,vec3 b){vec3 q=abs(p)-b;return length(max(q,0.))+min(max(q.x,max(q.y,q.z)),0.);}

// Rounded lancet arch: box + sphere cap
float sdArch(vec2 p, float w, float h, float topR){
    // Rectangular body
    float body=sdBox(vec3(p,.0),vec3(w,h*.7,.01));
    // Circular cap
    float cap=length(p-vec2(0.,h*.7))-topR;
    return min(body,cap);
}

// Scene: stone walls + gothic arches as windows (light sources)
vec2 scene(vec3 p){
    // Floor
    float floor_=p.y+1.5;
    // Ceiling vault
    float ceil_=-(p.y-3.5);
    // Side walls
    float wallL=p.x+2.5;
    float wallR=-(p.x-2.5);
    float wallBack=p.z+4.0;
    float wallFront=-(p.z-0.5);

    float walls=min(min(floor_,ceil_),min(min(wallL,wallR),min(wallBack,wallFront)));
    vec2 res=vec2(walls,0.); // mat 0 = stone

    // Window arch openings — subtract from wall (negative SDF region)
    // We mark the arch region with a separate material
    int N=int(min(archCount,5.));
    for(int i=0;i<5;i++){
        if(i>=N)break;
        float fi=float(i);
        float zp=-1.5+fi*-.8;
        float yw=1.0;
        // Left window
        vec2 lwp=vec2(abs(p.x)-2.5,p.y-yw);
        float lwdist=max(abs(p.z-zp)-.07,-(sdArch(lwp,.25,.8,.28)));
        float lwWindow=sdArch(lwp,.25,.8,.28)-max(0.,abs(p.z-zp)-.06);
        if(-lwWindow>0.&&abs(p.z-zp)<.12){
            float wd=abs(p.z-zp)-.06;
            if(wd<res.x) res=vec2(wd,fi+1.);
        }
    }
    return res;
}

vec3 calcNor(vec3 p){
    vec2 e=vec2(.002,0.);
    return normalize(vec3(
        scene(p+e.xyy).x-scene(p-e.xyy).x,
        scene(p+e.yxy).x-scene(p-e.yxy).x,
        scene(p+e.yyx).x-scene(p-e.yyx).x
    ));
}

vec3 windowColor(float id){
    float v=fract(id*.37);
    if(v<.33) return vec3(.9,.05,.05);  // crimson
    if(v<.67) return vec3(.05,.15,1.0); // cobalt
    return vec3(1.,.75,.0);             // gold
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME*timeScale;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.5;

    // Camera at nave, looking toward altar
    vec3 ro=vec3(0.,0.,0.4+sin(t*.15)*.1);
    vec3 rd=normalize(vec3(uv.x*.8,uv.y*.6,-1.));

    float d=.1; float matId=-1.;
    for(int i=0;i<80;i++){
        vec2 r=scene(ro+rd*d);
        if(r.x<.002){matId=r.y;break;}
        if(d>12.)break;
        d+=r.x;
    }

    // Deep stone shadow background
    vec3 col=vec3(.008,.006,.012);

    if(matId>=0.){
        vec3 pos=ro+rd*d;
        vec3 n=calcNor(pos);
        if(matId<.5){
            // Stone: very dark, almost black with subtle cool tint
            float stone=(.3+.7*h21(pos.xz*.8+pos.y*.3))*.04;
            col=vec3(stone*1.0,stone*.9,stone*1.1);
            // Colored light staining from windows
            for(int i=0;i<5;i++){
                float fi=float(i);
                float zw=-1.5+fi*-.8;
                // Check if we're in the window light cone
                float zDist=abs(pos.z-zw);
                vec3 wc=windowColor(fi+1.);
                float lightIn=max(0.,1.-zDist*2.)*max(0.,1.-abs(pos.x)*2.5)*glowPeak*.12*audio;
                col+=wc*lightIn;
            }
        } else {
            // Window pane: stained glass light emission
            vec3 wc=windowColor(matId);
            col=wc*glowPeak*audio;
        }
    }

    // Volumetric god-rays: march along 3 colored beams
    vec3 beamCols[3];
    beamCols[0]=vec3(.9,.05,.05);
    beamCols[1]=vec3(.05,.15,1.);
    beamCols[2]=vec3(1.,.75,0.);
    float beamZ[3];
    beamZ[0]=-1.5; beamZ[1]=-2.3; beamZ[2]=-3.1;
    for(int b=0;b<3;b++){
        // Beam direction from window at left wall
        vec3 wPos=vec3(-2.3,1.0,beamZ[b]);
        vec3 beamDir=normalize(vec3(1.,-0.3,0.));
        // Point on beam closest to current ray
        float beamD=0.;
        for(int s=0;s<16;s++){
            vec3 bp=wPos+beamDir*float(s)*.25;
            vec3 oc2=ro-bp;
            float cl2=max(0.,dot(oc2,oc2)-dot(oc2,rd)*dot(oc2,rd));
            float glow=exp(-cl2*8.)*exp(-float(s)*.12)*dustFloat*.005*glowPeak*audio;
            col+=beamCols[b]*glow;
        }
    }

    col*=1.-smoothstep(.55,.9,length(uv)*.85);
    gl_FragColor=vec4(col,1.);
}
