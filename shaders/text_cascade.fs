/*{
  "DESCRIPTION": "Pyroclastic Column — 3D volcanic eruption: rising ember fragments, ash cloud, orange-white heat plume, black silhouette",
  "CREDIT": "ShaderClaw auto-improve v9",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"emberCount","LABEL":"Ember Count","TYPE":"float","MIN":10.0,"MAX":60.0,"DEFAULT":30.0},
    {"NAME":"plumeDensity","LABEL":"Plume Density","TYPE":"float","MIN":0.5,"MAX":3.0,"DEFAULT":1.5},
    {"NAME":"lavaColor","LABEL":"Lava Color","TYPE":"color","DEFAULT":[1.0,0.38,0.02,1.0]},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.5},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

float h11(float p){p=fract(p*.1031);p*=p+33.33;p*=p+p;return fract(p);}

float sdSphere(vec3 p,float r){return length(p)-r;}

vec2 scene(vec3 p,float t){
    // Lava vent mound at base
    float cone=sdSphere(p-vec3(0,-0.85,0),0.38);
    // Central column (cylinder-like chain of spheres)
    float col_=1e9;
    for(int i=0;i<5;i++){
        float fi=float(i);
        float y=-0.55+fi*.25;
        float rad=0.14-fi*.018;
        float s=sdSphere(p-vec3(sin(t*.4+fi*.8)*.03,y,cos(t*.3+fi*.7)*.03),max(rad,.04));
        col_=min(col_,s);
    }
    float base=min(cone,col_);
    float mat=base<col_?1.0:2.0; // cone or column
    // Ground plane
    float gnd=p.y+0.95;
    if(gnd<base){base=gnd;mat=3.0;}
    return vec2(base,mat);
}

vec3 calcN(vec3 p,float t){
    vec2 e=vec2(.001,0);
    return normalize(vec3(
        scene(p+e.xyy,t).x-scene(p-e.xyy,t).x,
        scene(p+e.yxy,t).x-scene(p-e.yxy,t).x,
        scene(p+e.yyx,t).x-scene(p-e.yyx,t).x));
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/min(RENDERSIZE.x,RENDERSIZE.y);
    float t=TIME;
    float audio=1.0+audioBass*audioReact*.5;

    // Camera: slightly angled from front-low
    vec3 ro=vec3(sin(t*.06)*.4,0.1,2.2);
    vec3 fw=normalize(vec3(0,-0.15,0)-ro);
    vec3 rt=normalize(cross(fw,vec3(0,1,0)));
    vec3 up=cross(rt,fw);
    vec3 rd=normalize(fw+uv.x*rt+uv.y*up);

    float tm=0.05; float mat=-1.0;
    for(int i=0;i<64;i++){
        vec2 h=scene(ro+rd*tm,t);
        if(h.x<.001){mat=h.y;break;}
        tm+=h.x;
        if(tm>8.) break;
    }

    vec3 VOID  =vec3(0.0,0.0,0.0);
    vec3 LAVA  =lavaColor.rgb;
    vec3 ASH   =vec3(0.28,0.22,0.18);
    vec3 WHTHT =vec3(1.5,1.1,0.7);

    vec3 col=VOID;

    if(mat>=0.0){
        vec3 p=ro+rd*tm;
        vec3 n=calcN(p,t);
        vec3 light=normalize(vec3(0.3,1.5,.8));
        float diff=max(dot(n,light),0.0);
        float spec=pow(max(dot(reflect(-light,n),-rd),0.0),40.0);

        if(mat<1.5){
            // Lava cone base: white-hot near top, darkening toward base
            float heat=smoothstep(-0.85,-0.5,p.y);
            col=mix(LAVA*.8,WHTHT*hdrPeak,heat*heat)*audio*(diff*.7+.3);
        } else if(mat<2.5){
            // Central column: vivid orange, glowing core
            col=LAVA*(diff*.5+.6)*hdrPeak*audio;
            col+=WHTHT*spec*2.0*hdrPeak;
        } else {
            // Ground: dark ash with red-orange ambient
            col=ASH*(diff*.3+.1)+LAVA*exp(-length(p.xz)*length(p.xz)*.8)*.2*hdrPeak;
        }
    }

    // Pyroclastic plume: additive rising sphere billboards
    int NE=int(clamp(emberCount,10.0,60.0));
    for(int i=0;i<60;i++){
        if(i>=NE) break;
        float fi=float(i);
        float phase=h11(fi*7.31);
        float speed=0.35+h11(fi*3.1)*.5;
        float age=fract(t*speed+phase);
        // Rising from vent
        float spiralX=sin(age*3.14159*2.0+fi*2.3)*(0.15+h11(fi*9.1)*.2)*age*.8;
        float spiralZ=cos(age*3.14159*1.7+fi*3.1)*(0.12+h11(fi*5.7)*.2)*age*.8;
        vec2 screenPos=vec2(spiralX, age*1.0-0.6);
        float d=length(uv-screenPos);
        float sz=(0.02+h11(fi*17.3)*.04)*(1.0+age*.3)*audio;
        // Ember color: white-hot when young, orange when older, ash when old
        vec3 emberCol=mix(WHTHT,mix(LAVA,ASH*0.5,age*.6),age*.5);
        col+=emberCol*exp(-d*d/(sz*sz))*hdrPeak*(1.0-age*.7)*plumeDensity;
    }

    // Volumetric heat shimmer at column base
    col+=LAVA*exp(-length(uv-vec2(0.0,-0.15))*length(uv-vec2(0.0,-0.15))*4.0)*.2*hdrPeak*audio;

    gl_FragColor=vec4(col,1.0);
}
