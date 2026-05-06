/*{
  "DESCRIPTION": "Rembrandt Sphere — 3D portrait with single-source Rembrandt lighting, FBM impasto texture",
  "CREDIT": "ShaderClaw3 auto-improve v16",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME": "lightAngle", "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 6.28},
    {"NAME": "impasto",    "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.0, "MAX": 0.4},
    {"NAME": "audioMod",   "TYPE": "audio"}
  ]
}*/
precision highp float;
float apeak(){return (audioBass.x+audioBass.y+audioBass.z+audioBass.w)*0.25;}
float hash3(vec3 p){return fract(sin(dot(p,vec3(127.1,311.7,74.7)))*43758.5);}
float noise3(vec3 p){
    vec3 i=floor(p),f=fract(p);f=f*f*(3.0-2.0*f);
    return mix(mix(mix(hash3(i),hash3(i+vec3(1,0,0)),f.x),mix(hash3(i+vec3(0,1,0)),hash3(i+vec3(1,1,0)),f.x),f.y),
               mix(mix(hash3(i+vec3(0,0,1)),hash3(i+vec3(1,0,1)),f.x),mix(hash3(i+vec3(0,1,1)),hash3(i+vec3(1,1,1)),f.x),f.y),f.z);
}
float fbm(vec3 p){float v=0.0,a=0.5;for(int i=0;i<4;i++){v+=a*noise3(p);p*=2.1;a*=0.5;}return v;}
float scene(vec3 p,float ap){
    float n=fbm(p*4.0+TIME*0.06)*impasto*(1.0+ap*0.4);
    return length(p)-0.72-n;
}
vec3 nrm(vec3 p,float ap){
    float e=0.002;
    return normalize(vec3(
        scene(p+vec3(e,0,0),ap)-scene(p-vec3(e,0,0),ap),
        scene(p+vec3(0,e,0),ap)-scene(p-vec3(0,e,0),ap),
        scene(p+vec3(0,0,e),ap)-scene(p-vec3(0,0,e),ap)));
}
void main(){
    vec2 uv=(isf_FragNormCoord*2.0-1.0)*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    float ap=apeak();
    vec3 ro=vec3(0.0,0.0,2.2);
    vec3 rd=normalize(vec3(uv,-1.4));
    float dist=0.0;bool hit=false;
    for(int i=0;i<64;i++){
        float d=scene(ro+rd*dist,ap);
        if(d<0.001){hit=true;break;}
        if(dist>5.0)break;
        dist+=d*0.6;
    }
    // Dark umber canvas — near-black oil paint ground
    vec3 col=vec3(0.02,0.01,0.005);
    if(hit){
        vec3 p=ro+rd*dist;
        vec3 n=nrm(p,ap);
        // Rembrandt light — upper left, slowly rotating
        vec3 L=normalize(vec3(cos(lightAngle+TIME*0.04)*1.0,1.3,0.6));
        float diff=max(dot(n,L),0.0);
        float spec=pow(max(dot(reflect(-L,n),-rd),0.0),24.0);
        float sss=max(dot(n,-L),0.0)*0.25;
        // Warm amber lit side, deep crimson shadow side
        vec3 litColor=vec3(1.0,0.52,0.08);
        vec3 shadowColor=vec3(0.22,0.02,0.03);
        col=mix(shadowColor,litColor,diff*(1.0+ap*0.25));
        col+=vec3(0.14,0.04,0.02)*sss;
        // HDR gold specular peak
        col+=vec3(3.5,2.8,1.0)*spec*(1.0+ap*0.5);
        // fwidth AA on FBM impasto brushstroke edges
        float tex=fbm(p*9.0+TIME*0.08);
        float texAA=fwidth(tex);
        float stroke=smoothstep(texAA,0.0,abs(tex-0.5)-0.09);
        col+=vec3(0.12,0.04,0.0)*stroke*0.35;
        // Fresnel darkens portrait edges
        float fresnel=pow(1.0-max(dot(n,-rd),0.0),2.5);
        col*=1.0-fresnel*0.65;
    }
    gl_FragColor=vec4(col,1.0);
}
