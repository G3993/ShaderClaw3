/*{
  "DESCRIPTION": "Soap Bubble Cluster — 3D raymarched iridescent soap bubbles, thin-film rainbow diffraction, deep-space void background",
  "CREDIT": "ShaderClaw auto-improve v8",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"bubbleCount","LABEL":"Bubble Count","TYPE":"float","MIN":3.0,"MAX":12.0,"DEFAULT":7.0},
    {"NAME":"filmThick","LABEL":"Film Thickness","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.5},
    {"NAME":"rotSpeed","LABEL":"Rotation Speed","TYPE":"float","MIN":0.0,"MAX":1.0,"DEFAULT":0.22},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.5},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

float h11(float p){p=fract(p*.1031);p*=p+33.33;p*=p+p;return fract(p);}

float sdSphere(vec3 p,float r){return length(p)-r;}

vec2 scene(vec3 p,float t,float N){
    float d=1e9; float mat=0.0;
    int iN=int(clamp(N,3.0,12.0));
    for(int i=0;i<12;i++){
        if(i>=iN) break;
        float fi=float(i);
        float ang=fi/N*6.28318+t*(0.18+h11(fi*.73)*.25);
        float orbit=0.28+h11(fi*5.1)*.32;
        float yr=(h11(fi*3.7)-.5)*.55;
        float sz=0.13+h11(fi*9.3)*.18;
        vec3 bc=vec3(cos(ang)*orbit,yr,sin(ang)*orbit);
        float ds=sdSphere(p-bc,sz);
        if(ds<d){d=ds;mat=fi+1.0;}
    }
    return vec2(d,mat);
}

vec3 calcNormal(vec3 p,float t,float N){
    vec2 e=vec2(.001,0);
    return normalize(vec3(
        scene(p+e.xyy,t,N).x-scene(p-e.xyy,t,N).x,
        scene(p+e.yxy,t,N).x-scene(p-e.yxy,t,N).x,
        scene(p+e.yyx,t,N).x-scene(p-e.yyx,t,N).x));
}

vec3 thinFilm(float cosTheta,float thickness){
    // Three-wavelength thin-film interference: R/G/B at 700/550/440nm
    float phR=thickness*cosTheta*6.28318*1.0;
    float phG=thickness*cosTheta*6.28318*1.27;
    float phB=thickness*cosTheta*6.28318*1.59;
    return vec3(.5+.5*cos(phR),.5+.5*cos(phG),.5+.5*cos(phB));
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/min(RENDERSIZE.x,RENDERSIZE.y);
    float t=TIME*rotSpeed;
    float audio=1.0+audioBass*audioReact*.45;
    float N=clamp(bubbleCount,3.0,12.0);

    // Orbiting camera
    float camT=TIME*rotSpeed*.55;
    vec3 ro=vec3(sin(camT)*1.75,0.28+sin(camT*.39)*.25,cos(camT)*1.75);
    vec3 fw=normalize(-ro);
    vec3 rt=normalize(cross(fw,vec3(0,1,0)));
    vec3 up=cross(rt,fw);
    vec3 rd=normalize(fw+uv.x*rt+uv.y*up);

    float tm=0.05; float mat=-1.0;
    for(int i=0;i<64;i++){
        vec2 h=scene(ro+rd*tm,t,N);
        if(h.x<.0008){mat=h.y;break;}
        tm+=h.x;
        if(tm>8.) break;
    }

    // Deep void black bg
    vec3 col=vec3(0.0,0.0,0.008);

    if(mat>0.0){
        vec3 p=ro+rd*tm;
        vec3 n=calcNormal(p,t,N);

        float cosV=abs(dot(-rd,n));
        // Thin-film color varies with film thickness parameter + viewing angle
        float depth=filmThick*4.0+0.5+audioMid*audioReact*.5;
        vec3 film=thinFilm(cosV,depth);

        // Fresnel: rim is bright, face-on is dark (soap bubble transparency)
        float fresnel=pow(1.0-cosV,2.5);

        // Specular highlight
        vec3 light=normalize(vec3(2.0,3.0,1.5));
        float spec=pow(max(dot(reflect(-light,n),-rd),0.0),120.0);

        // Interior: very dark (bubble is hollow)
        vec3 bubSurface=film*(fresnel*2.2+cosV*.15)+vec3(1.0)*spec*3.5;
        col=bubSurface*hdrPeak*audio;
    }

    // Faint ambient glow from bubble cluster center
    col+=vec3(0.4,0.6,1.0)*exp(-length(uv)*length(uv)*5.0)*.08*hdrPeak;

    gl_FragColor=vec4(col,1.0);
}
