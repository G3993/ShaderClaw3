/*{
  "DESCRIPTION": "Turbine Core — 3D jet turbine cross-section: spinning blades, titanium casing, orange heat glow from center",
  "CREDIT": "ShaderClaw auto-improve v8",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"bladeCount","LABEL":"Blade Count","TYPE":"float","MIN":4.0,"MAX":24.0,"DEFAULT":12.0},
    {"NAME":"spinSpeed","LABEL":"Spin Speed","TYPE":"float","MIN":0.0,"MAX":4.0,"DEFAULT":1.5},
    {"NAME":"coreColor","LABEL":"Core Glow","TYPE":"color","DEFAULT":[1.0,0.42,0.04,1.0]},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.5},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

float sdBox(vec3 p,vec3 b){vec3 q=abs(p)-b;return length(max(q,0.))+min(max(q.x,max(q.y,q.z)),0.);}
float sdCyl(vec3 p,float h,float r){vec2 d=abs(vec2(length(p.xz),p.y))-vec2(r,h);return min(max(d.x,d.y),0.)+length(max(d,0.));}

float sdBlade(vec3 p,float N,float spin){
    float d=1e9;
    int iN=int(clamp(N,4.0,24.0));
    for(int i=0;i<24;i++){
        if(i>=iN) break;
        float ang=float(i)/N*6.28318+spin;
        float ca=cos(ang),sa=sin(ang);
        vec3 lp=vec3(ca*p.x+sa*p.z, p.y, -sa*p.x+ca*p.z);
        // Blade: long thin box radially aligned
        float blade=sdBox(lp-vec3(0.32,0.0,0.0),vec3(0.18,0.008,0.045));
        d=min(d,blade);
    }
    return d;
}

vec2 scene(vec3 p,float spin){
    float outer=abs(sdCyl(p,0.065,0.56))-0.022; // outer ring (thick walled)
    float blade=sdBlade(p,bladeCount,spin);
    float hub  =sdCyl(p,0.075,0.09);             // center hub
    float core =sdCyl(p,0.065,0.035);            // glowing core

    // Build material id from closest surface
    float s=outer; float mat=1.0;
    if(blade<s){s=blade;mat=2.0;}
    if(hub  <s){s=hub;  mat=3.0;}
    if(core <s){s=core; mat=4.0;}
    return vec2(s,mat);
}

vec3 calcN(vec3 p,float spin){
    vec2 e=vec2(.0008,0);
    return normalize(vec3(
        scene(p+e.xyy,spin).x-scene(p-e.xyy,spin).x,
        scene(p+e.yxy,spin).x-scene(p-e.yxy,spin).x,
        scene(p+e.yyx,spin).x-scene(p-e.yyx,spin).x));
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/min(RENDERSIZE.x,RENDERSIZE.y);
    float audio=1.0+audioBass*audioReact*.5;
    float spin=TIME*spinSpeed*(1.0+audioBass*audioReact*.4);

    // Looking straight down +Z into turbine
    vec3 ro=vec3(0.0,0.0,2.2);
    vec3 rd=normalize(vec3(uv,-0.85));

    float tm=0.05; float mat=-1.0;
    for(int i=0;i<64;i++){
        vec2 h=scene(ro+rd*tm,spin);
        if(h.x<.0005){mat=h.y;break;}
        tm+=h.x*.8;
        if(tm>7.) break;
    }

    // Void black background
    vec3 col=vec3(0.0,0.0,0.0);

    if(mat>0.0){
        vec3 p=ro+rd*tm;
        vec3 n=calcN(p,spin);

        vec3 light=normalize(vec3(0.3,0.4,1.0));
        float diff=max(dot(n,light),0.0);
        float spec=pow(max(dot(reflect(-light,n),-rd),0.0),80.0);

        // 4-color palette: titanium steel, blade steel, hot hub, white-hot core
        vec3 STEEL   =vec3(0.52,0.55,0.62);   // titanium grey
        vec3 HOT     =coreColor.rgb;            // user-set orange glow
        vec3 WHITEHOT=vec3(1.5,1.2,0.8);       // HDR white-hot

        if(mat<1.5){
            // Outer ring casing: cool titanium
            col=STEEL*(diff*.75+.2)+vec3(1.0)*spec*1.8;
        } else if(mat<2.5){
            // Blades: polished steel, warm highlight from core
            float rimHeat=exp(-length(p.xz)*length(p.xz)*2.5)*0.6;
            col=STEEL*(diff*.65+.18)+HOT*rimHeat*hdrPeak+vec3(1.0)*spec*1.2;
        } else if(mat<3.5){
            // Hub: heated, orange-tinted
            col=HOT*(diff*.7+.35)*hdrPeak*audio;
        } else {
            // Core: white-hot HDR
            col=WHITEHOT*hdrPeak*audio;
        }
    }

    // Volumetric core heat bloom in center of view
    float coreR=length(uv);
    col+=coreColor.rgb*exp(-coreR*coreR*10.0)*hdrPeak*.9*audio;
    // Outer ambient orange glow
    col+=coreColor.rgb*exp(-coreR*coreR*2.5)*.1*hdrPeak;

    gl_FragColor=vec4(col,1.0);
}
