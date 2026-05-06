/*{
  "DESCRIPTION": "Tropical Vaporwave — coral sunset, teal ocean, palm silhouettes, perspective grid",
  "CREDIT": "ShaderClaw3 auto-improve v16",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME": "sunSpeed",  "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0},
    {"NAME": "waveAmp",   "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0},
    {"NAME": "hdrBoost",  "TYPE": "float", "DEFAULT": 2.2, "MIN": 0.5, "MAX": 4.0},
    {"NAME": "audioMod",  "TYPE": "audio"}
  ]
}*/
precision highp float;
#define PI 3.14159265359
float apeak(){return (audioBass.x+audioBass.y+audioBass.z+audioBass.w)*0.25;}
float hash(float n){return fract(sin(n)*43758.5);}
float hash2(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5);}
float noise2(vec2 p){vec2 i=floor(p),f=fract(p);f=f*f*(3.0-2.0*f);
    return mix(mix(hash2(i),hash2(i+vec2(1,0)),f.x),mix(hash2(i+vec2(0,1)),hash2(i+vec2(1,1)),f.x),f.y);}

// Crude palm silhouette at (px, py) relative to uv
float palmMask(vec2 uv, float px, float sc){
    vec2 p=(uv-vec2(px,0.0))/sc;
    // Trunk: tapered from -1.0 to 0.0 y
    float trunk=max(abs(p.x)-0.015*(1.0-p.y*0.3),max(-p.y-1.0,p.y));
    trunk=max(trunk,max(p.x-0.06,-p.x-0.06));
    // 5 fronds radiating from top (p.y = 0)
    float frond=1e9;
    for(int i=0;i<5;i++){
        float a=float(i)/5.0*PI*1.6-PI*0.8;
        vec2 dir=vec2(sin(a),cos(a));
        vec2 fp=p;
        float t=clamp(dot(fp,dir),0.0,0.4);
        float d=length(fp-dir*t)-0.015*(1.0-t/0.4)*0.5;
        frond=min(frond,d);
    }
    return step(min(max(trunk,0.0),1.0),0.001)+step(max(frond,0.0),0.001);
}

void main(){
    vec2 uv=(isf_FragNormCoord*2.0-1.0)*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    float ap=apeak();
    float t=TIME*sunSpeed;
    float horizon=-0.05+sin(t*0.15)*0.03;
    vec3 col;

    if(uv.y>horizon){
        // Sky: deep violet top → coral mid → warm orange horizon
        float sky=(uv.y-horizon)/(1.0-horizon);
        vec3 topSky=vec3(0.12,0.02,0.32);
        vec3 midSky=vec3(1.0,0.28,0.12);
        vec3 horizSky=vec3(1.3,0.55,0.05);
        col=sky<0.45?mix(horizSky,midSky,sky/0.45):mix(midSky,topSky,(sky-0.45)/0.55);
        col*=hdrBoost*0.75;
        // Large hot coral sun
        float sunY=horizon+0.18+sin(t*0.2)*0.03;
        float sunDist=length(uv-vec2(0.0,sunY));
        float sunR=0.20+ap*0.015;
        float sunMask=smoothstep(sunR+0.005,sunR-0.005,sunDist);
        // Vaporwave horizontal stripe cutouts through sun
        float stripe=step(0.5,fract((uv.y-sunY)/sunR*5.0+0.25))*smoothstep(sunR,sunR*0.15,sunDist);
        col+=vec3(3.2,1.6,0.3)*(sunMask-stripe*sunMask);
        // Sun glow halo
        col+=vec3(2.0,0.7,0.1)*0.25*max(0.0,1.0-sunDist/0.55)*(1.0+ap*0.3);
    } else {
        // Ocean: deep teal with animated wave shimmer
        float wy=uv.y-horizon;
        float wave=(sin(uv.x*9.0+t*2.0)*0.018+sin(uv.x*5.3-t*1.4)*0.012)*waveAmp*(1.0+ap*0.25);
        float depth=clamp(-wy*2.5,0.0,1.0);
        col=mix(vec3(0.0,0.55,0.65),vec3(0.0,0.15,0.35),depth)*hdrBoost*0.6;
        // Specular sun reflection on water
        float reflX=uv.x*0.5;
        float reflY=(uv.y-horizon+wave)*3.5;
        float reflDist=length(vec2(reflX,reflY));
        col+=vec3(3.0,1.8,0.4)*max(0.0,1.0-reflDist*5.0)*(1.0+ap*0.5)*waveAmp;
    }

    // Vaporwave perspective floor below ocean
    if(uv.y<horizon-0.4){
        float gy=(uv.y-(horizon-0.4))/0.6;
        float perspX=uv.x/(max(abs(gy),0.01));
        float gx=abs(fract(perspX*1.5)-0.5);
        float gyF=abs(fract(gy*4.0)-0.5);
        float gxAA=fwidth(perspX*1.5);float gyAA=fwidth(gy*4.0);
        float grid=max(smoothstep(gxAA,0.0,gx-0.02),smoothstep(gyAA,0.0,gyF-0.02));
        col=mix(vec3(0.0,0.0,0.06),vec3(0.0,2.2,2.8)*hdrBoost,grid*(1.0-abs(gy)));
    }

    // Black palm silhouettes at base
    float palmY=horizon-0.15;
    vec2 puv=vec2(uv.x,(uv.y-palmY)/0.7);
    float p1=palmMask(puv,-0.68,0.8);
    float p2=palmMask(puv, 0.70,0.72);
    col=mix(col,vec3(0.0),max(p1,p2));

    gl_FragColor=vec4(col,1.0);
}
