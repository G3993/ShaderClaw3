/*{
  "DESCRIPTION": "Obsidian Mirror — 3D shattered obsidian plane, glassy black shards with lava-orange fault cracks and white-hot reflections",
  "CREDIT": "ShaderClaw auto-improve v9",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"shardCount","LABEL":"Shard Count","TYPE":"float","MIN":3.0,"MAX":12.0,"DEFAULT":7.0},
    {"NAME":"faultColor","LABEL":"Fault Glow","TYPE":"color","DEFAULT":[1.0,0.32,0.02,1.0]},
    {"NAME":"spinSpeed","LABEL":"Orbit Speed","TYPE":"float","MIN":0.0,"MAX":0.5,"DEFAULT":0.1},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.5},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

float h11(float p){p=fract(p*.1031);p*=p+33.33;p*=p+p;return fract(p);}

float sdBox(vec3 p,vec3 b){vec3 q=abs(p)-b;return length(max(q,0.))+min(max(q.x,max(q.y,q.z)),0.);}

mat3 rotX(float a){float c=cos(a),s=sin(a);return mat3(1,0,0,0,c,-s,0,s,c);}
mat3 rotY(float a){float c=cos(a),s=sin(a);return mat3(c,0,s,0,1,0,-s,0,c);}

vec2 scene(vec3 p,float t){
    float d=1e9; float mat=0.0;
    int iN=int(clamp(shardCount,3.0,12.0));
    for(int i=0;i<12;i++){
        if(i>=iN) break;
        float fi=float(i);
        float ang=fi/shardCount*6.28318+t*spinSpeed;
        float rr=0.18+h11(fi*7.31)*.45;
        vec3 c=vec3(cos(ang)*rr,(h11(fi*3.7)-.5)*.5,sin(ang)*rr);
        float tx=(h11(fi*3.7)-.5)*2.0;
        float tz=(h11(fi*9.1)-.5)*2.0;
        mat3 R=rotX(tx)*rotY(tz);
        vec3 lp=R*(p-c);
        float sz1=0.07+h11(fi*5.1)*.1;
        float sz2=0.22+h11(fi*11.3)*.2;
        float ds=sdBox(lp,vec3(sz1,sz2,0.005));
        if(ds<d){d=ds;mat=fi+1.0;}
    }
    float gnd=p.y+0.65;
    if(gnd<d){d=gnd;mat=0.0;}
    return vec2(d,mat);
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
    float audio=1.0+audioLevel*audioReact*.4;

    float camAng=t*spinSpeed;
    vec3 ro=vec3(sin(camAng)*1.9,0.55+sin(t*.19)*.15,cos(camAng)*1.9);
    vec3 fw=normalize(vec3(0,-0.22,0)-ro);
    vec3 rt=normalize(cross(fw,vec3(0,1,0)));
    vec3 up=cross(rt,fw);
    vec3 rd=normalize(fw+uv.x*rt+uv.y*up);

    float tm=0.05; float mat=-1.0;
    for(int i=0;i<64;i++){
        vec2 h=scene(ro+rd*tm,t);
        if(h.x<.0006){mat=h.y;break;}
        tm+=h.x;
        if(tm>10.) break;
    }

    vec3 VOID  =vec3(0.0,0.0,0.0);
    vec3 OBSID =vec3(0.025,0.018,0.015);
    vec3 FAULT =faultColor.rgb;
    vec3 WHTHT =vec3(1.6,1.3,0.9);

    vec3 col=VOID;

    if(mat>=0.0){
        vec3 p=ro+rd*tm;
        vec3 n=calcN(p,t);

        vec3 light=normalize(vec3(1.2,2.0,.8));
        float diff=max(dot(n,light),0.0);
        float spec=pow(max(dot(reflect(-light,n),-rd),0.0),150.0);
        float rim=pow(1.0-abs(dot(-rd,n)),4.0);

        if(mat<0.5){
            col=OBSID*(diff*.4+.1)+FAULT*spec*hdrPeak*.5;
        } else {
            col=OBSID*(diff*.3+.08)
               +FAULT*(rim*1.4+spec*.8)*hdrPeak*audio
               +WHTHT*spec*2.5*hdrPeak;
        }
    }

    col+=FAULT*exp(-length(uv)*length(uv)*3.5)*.12*hdrPeak;

    gl_FragColor=vec4(col,1.0);
}
