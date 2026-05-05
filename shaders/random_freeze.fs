/*{
  "DESCRIPTION": "Forge Anvil — 3D blacksmith forge interior: glowing heated metal bar on anvil, ember sparks rising in forge darkness",
  "CREDIT": "ShaderClaw auto-improve v9",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"metalHeat","LABEL":"Metal Heat","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.72},
    {"NAME":"sparkCount","LABEL":"Spark Count","TYPE":"float","MIN":5.0,"MAX":40.0,"DEFAULT":18.0},
    {"NAME":"forgeColor","LABEL":"Forge Color","TYPE":"color","DEFAULT":[1.0,0.38,0.03,1.0]},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.5},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

float h11(float p){p=fract(p*.1031);p*=p+33.33;p*=p+p;return fract(p);}

float sdBox(vec3 p,vec3 b){vec3 q=abs(p)-b;return length(max(q,0.))+min(max(q.x,max(q.y,q.z)),0.);}

vec2 scene(vec3 p){
    float base=sdBox(p-vec3(0,-0.28,0),vec3(0.48,0.13,0.28));
    float horn=sdBox(p-vec3(0.55,-0.24,0),vec3(0.14,0.07,0.09));
    float anvil=min(base,horn);
    float bar  =sdBox(p-vec3(-0.04,-0.09,0),vec3(0.24,0.058,0.07));
    float gnd  =p.y+0.46;
    float s=anvil; float mat=1.0;
    if(bar<s){s=bar;mat=2.0;}
    if(gnd<s){s=gnd;mat=3.0;}
    return vec2(s,mat);
}

vec3 calcN(vec3 p){
    vec2 e=vec2(.001,0);
    return normalize(vec3(
        scene(p+e.xyy).x-scene(p-e.xyy).x,
        scene(p+e.yxy).x-scene(p-e.yxy).x,
        scene(p+e.yyx).x-scene(p-e.yyx).x));
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/min(RENDERSIZE.x,RENDERSIZE.y);
    float t=TIME;
    float audio=1.0+audioBass*audioReact*.5;

    float camAng=t*.08;
    vec3 ro=vec3(sin(camAng)*1.6,0.62,cos(camAng)*1.4+0.3);
    vec3 fw=normalize(vec3(0,-0.18,0)-ro);
    vec3 rt=normalize(cross(fw,vec3(0,1,0)));
    vec3 up=cross(rt,fw);
    vec3 rd=normalize(fw+uv.x*rt+uv.y*up);

    float tm=0.08; float mat=-1.0;
    for(int i=0;i<64;i++){
        vec2 h=scene(ro+rd*tm);
        if(h.x<.001){mat=h.y;break;}
        tm+=h.x;
        if(tm>8.) break;
    }

    vec3 FORGBG=vec3(0.02,0.01,0.0);
    vec3 IRON  =vec3(0.11,0.11,0.14);
    vec3 HOT   =forgeColor.rgb;
    vec3 WHTHT =vec3(1.5,1.2,0.8);

    vec3 col=FORGBG;

    if(mat>=0.0){
        vec3 p=ro+rd*tm;
        vec3 n=calcN(p);
        vec3 light=normalize(vec3(0.4,1.5,.6));
        float diff=max(dot(n,light),0.0);
        float spec=pow(max(dot(reflect(-light,n),-rd),0.0),50.0);

        if(mat<1.5){
            float proxHeat=exp(-length(p-vec3(-0.04,-0.09,0))*2.5)*.5;
            col=IRON*(diff*.7+.2)+HOT*proxHeat*.4*hdrPeak+vec3(.8)*spec*.5;
        } else if(mat<2.5){
            float heat=metalHeat*(1.0+audioBass*audioReact*.3);
            col=mix(HOT*(diff*.6+.5),WHTHT*(diff*.5+.6),heat*heat)*hdrPeak*audio;
        } else {
            col=vec3(0.04,0.03,0.02)*(diff*.3+.1);
        }
    }

    // Ember sparks rising from hot bar (2D additive)
    int NS=int(clamp(sparkCount,5.0,40.0));
    for(int i=0;i<40;i++){
        if(i>=NS) break;
        float fi=float(i);
        float phase=h11(fi*7.31);
        float speed=0.28+h11(fi*3.1)*.35;
        float age=fract(t*speed+phase);
        vec2 origin=vec2(-0.08+h11(fi*11.7)*.18-0.09,-0.18);
        vec2 drift=vec2((h11(fi*19.3)-.5)*.18,age*.95);
        drift.x+=sin(age*3.14159*(h11(fi*23.1)*2.0-1.0))*.06;
        vec2 sUV=origin+drift;
        float d=length(uv-sUV);
        float sz=0.008*(1.0-age*.7)*audio;
        col+=HOT*exp(-d*d/(sz*sz))*hdrPeak*(1.0-age)*1.4;
    }

    col+=HOT*exp(-length(uv-vec2(-0.05,0.0))*length(uv-vec2(-0.05,0.0))*5.0)*.15*hdrPeak*audio;

    gl_FragColor=vec4(col,1.0);
}
