/*{
  "DESCRIPTION": "Kaleidoscope Mirror Fractal — recursive triangle reflections, jewel-tone HDR palette",
  "CREDIT": "ShaderClaw3 auto-improve v18",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME": "segments",  "TYPE": "float", "DEFAULT": 6.0,  "MIN": 3.0, "MAX": 12.0},
    {"NAME": "zoomSpeed", "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0},
    {"NAME": "glowPeak",  "TYPE": "float", "DEFAULT": 2.8,  "MIN": 0.5, "MAX": 6.0},
    {"NAME": "audioMod",  "TYPE": "audio"}
  ]
}*/
precision highp float;
#define TAU 6.28318530718
float apeak(){return (audioBass.x+audioBass.y+audioBass.z+audioBass.w)*0.25;}
// Cosine palette: deep magenta → electric cyan → gold → violet
vec3 jewel(float t){
    return vec3(0.5)+vec3(0.5)*cos(TAU*(vec3(1.0)*t+vec3(0.0,0.333,0.667)));
}
// Mirror-fold UV into one kaleidoscope sector
vec2 kfold(vec2 p, float n){
    float s=TAU/n;
    float raw=atan(p.y,p.x);
    float a=mod(raw+62.83,s);
    float m=mod(floor((raw+62.83)/s),2.0);
    if(m<1.0) a=s-a;
    return length(p)*vec2(cos(a),sin(a));
}
void main(){
    vec2 uv=(isf_FragNormCoord*2.0-1.0)*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    float ap=apeak();
    float t=TIME*zoomSpeed;
    float n=max(3.0,floor(segments));
    // Animated zoom tunnel
    float phase=fract(t);
    uv/=(0.3+phase*0.7)*(0.7+ap*0.2);
    vec2 p=kfold(uv,n);
    float r=length(p);
    // Recursive mirror folds → fractal glow accumulation
    float lum=0.0;
    for(int i=0;i<6;i++){
        p=kfold(p,n);
        p=abs(p)-0.30*pow(0.8,float(i));
        float d2=dot(p,p);
        lum+=0.09/(0.002+d2*5.0)*pow(0.6,float(i));
    }
    float hue=lum*0.45+atan(p.y,p.x)/TAU+t*0.07+ap*0.12;
    vec3 col=jewel(hue)*lum*glowPeak*(1.0+ap*0.6);
    // HDR gold core burst
    col+=vec3(3.0,2.2,0.5)*max(0.0,1.0-r*3.2)*(1.0+ap);
    // Electric magenta pulse ring
    float ringR=0.22+ap*0.04;
    col+=vec3(2.5,0.1,2.0)*smoothstep(0.03,0.0,abs(r-ringR));
    // Deep violet outer vignette glow
    col+=vec3(0.6,0.0,1.8)*smoothstep(0.4,1.2,r);
    gl_FragColor=vec4(col,1.0);
}
