/*{
  "DESCRIPTION": "Neon Torii Gate — 3D raymarched Shinto gate in night rain; HDR vermillion/gold/cyan neon against void-black",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"rainIntensity","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":1.5,"LABEL":"Rain"},
    {"NAME":"neonBuzz","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0,"LABEL":"Neon Buzz"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"},
    {"NAME":"camDist","TYPE":"float","DEFAULT":3.5,"MIN":1.5,"MAX":6.0,"LABEL":"Cam Distance"}
  ]
}*/

float h11(float n){return fract(sin(n*127.1)*43758.5453);}
float h21(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}

float sdBox(vec3 p,vec3 b){vec3 q=abs(p)-b;return length(max(q,0.))+min(max(q.x,max(q.y,q.z)),0.);}
float sdCylinder(vec3 p,float r,float h){
    vec2 d=abs(vec2(length(p.xz),p.y))-vec2(r,h);
    return length(max(d,0.))+min(max(d.x,d.y),0.);
}

// Torii: two vertical pillars + two horizontal kasagi beams
vec2 scene(vec3 p){
    float t=TIME;
    float buzz=neonBuzz*(.9+.1*sin(t*47.3)); // neon flicker

    // Main gate dimensions
    float pillarR=.08, pillarH=1.4;
    float kasagiW=1.6, kasagiH=.08, kasagiD=.14;
    float shimakiH=.08;

    // Left pillar
    float lp=sdCylinder(p-vec3(-0.8,0.,0.),pillarR,pillarH);
    // Right pillar
    float rp=sdCylinder(p-vec3( 0.8,0.,0.),pillarR,pillarH);
    // Top kasagi (upper beam — curves up at ends)
    float kb=sdBox(p-vec3(0.,pillarH*.9,0.),vec3(kasagiW*.5,kasagiH,kasagiD*.5));
    // Lower nuki (inner crossbeam)
    float nuki=sdBox(p-vec3(0.,pillarH*.55,0.),vec3(kasagiW*.42,shimakiH,kasagiD*.3));
    // Top cap balls on pillars
    float bc1=length(p-vec3(-0.8,pillarH,0.))-.12;
    float bc2=length(p-vec3( 0.8,pillarH,0.))-.12;

    float gate=min(min(min(lp,rp),min(kb,nuki)),min(bc1,bc2));

    // Ground plane
    float gnd=p.y+.5;

    vec2 res=vec2(gnd,0.); // mat 0 = ground
    if(gate<res.x) res=vec2(gate,1.); // mat 1 = gate
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
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.6;

    vec3 ro=vec3(sin(t*.03)*camDist*.3,0.8,camDist);
    vec3 target=vec3(0.,0.4,0.);
    vec3 fwd=normalize(target-ro);
    vec3 right=normalize(cross(fwd,vec3(0.,1.,0.)));
    vec3 up_=cross(right,fwd);
    vec3 rd=normalize(fwd+uv.x*right+uv.y*up_);

    // Void-black night sky
    vec3 col=vec3(.003,.002,.006);

    float d=.1; float matId=-1.;
    for(int i=0;i<80;i++){
        vec2 r=scene(ro+rd*d);
        if(r.x<.001){matId=r.y;break;}
        if(d>12.)break;
        d+=r.x;
    }

    if(matId>=0.){
        vec3 pos=ro+rd*d;
        vec3 n=calcNor(pos);
        if(matId<.5){
            // Wet ground — near-black with neon reflection
            float refl=pow(max(0.,1.-abs(dot(n,rd))),4.);
            // Simple neon color puddle reflection
            float pX=pos.x;
            float gateRefl=exp(-abs(pX)*.7)*exp(-abs(pos.z-camDist*.5)*.3);
            col=vec3(.01,.01,.015)+vec3(.8,.05,.0)*gateRefl*refl*glowPeak*.6
               +vec3(.0,.8,.9)*gateRefl*refl*glowPeak*.3;
        } else {
            // Torii gate: vermillion red with gold trim + cyan neon tubes
            float buzz=.9+.1*sin(t*47.3+pos.y*3.)*neonBuzz;
            vec3 vermillion=vec3(1.,.08,.0);   // HDR vermillion
            vec3 gold=vec3(1.,.8,.0);           // gold accents
            vec3 cyan=vec3(0.,.9,1.);           // neon trim
            float isTop=smoothstep(1.1,1.3,pos.y);
            float isCap=smoothstep(.9,1.0,length(pos.xz-.8)*.5+length(pos.y-1.4)*2.);
            vec3 matCol=mix(vermillion,gold,isTop);
            matCol=mix(matCol,cyan,isCap*.5);
            float diff=max(0.,dot(n,normalize(vec3(.3,1.,.4))))*.3+.6;
            col=matCol*diff*glowPeak*buzz*audio;
            float edge=fwidth(scene(pos).x);
            col*=smoothstep(0.,edge*5.,scene(pos).x+.001);
        }
    }

    // Rain streaks: vertical lines of bright cyan/white
    float ry=fract(uv.y*3.+t*(.8+rainIntensity*1.5));
    float rx=h11(floor(uv.x*60.+t*.2)*13.);
    float rStreak=exp(-abs(fract(uv.x*60.+rx*50.+t*.1)-.5)*30.)*
                  exp(-abs(ry-.5)*8.)*rainIntensity;
    col+=vec3(.5,.8,1.)*rStreak*glowPeak*.15;

    // Neon glow halos on gate beams
    float cx1=exp(-abs(uv.y-.15)*12.)*exp(-abs(uv.x)*.6)*.08*glowPeak*audio;
    col+=vec3(1.,.08,0.)*cx1;
    float cx2=exp(-abs(uv.y+.02)*18.)*exp(-abs(uv.x)*.7)*.05*glowPeak*audio;
    col+=vec3(0.,.85,1.)*cx2;

    col*=1.-smoothstep(.55,.9,length(uv)*.85);
    gl_FragColor=vec4(col,1.);
}
