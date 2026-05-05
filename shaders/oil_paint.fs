/*{
  "DESCRIPTION": "Crystal Geode — 3D raymarched amethyst geode interior; jewel-lit amethyst/sapphire/gold crystal walls in deep void",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"crystalDensity","TYPE":"float","DEFAULT":1.0,"MIN":0.3,"MAX":2.5,"LABEL":"Crystal Density"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"sparkleRate","TYPE":"float","DEFAULT":1.2,"MIN":0.2,"MAX":3.0,"LABEL":"Sparkle Rate"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"},
    {"NAME":"camSpin","TYPE":"float","DEFAULT":0.05,"MIN":0.0,"MAX":0.3,"LABEL":"Cam Orbit"}
  ]
}*/

float h11(float n){return fract(sin(n*127.1)*43758.5453);}
float h21(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}

float smin(float a,float b,float k){float h=clamp(.5+.5*(b-a)/k,0.,1.);return mix(b,a,h)-k*h*(1.-h);}

// Elongated box (crystal prism shape)
float sdBox(vec3 p,vec3 b){vec3 q=abs(p)-b;return length(max(q,0.))+min(max(q.x,max(q.y,q.z)),0.);}

// Single crystal spike: elongated box rotated on x and z axes
float sdCrystal(vec3 p,float seed){
    float a1=(h11(seed)-.5)*1.4;
    float a2=(h11(seed+1.)-.5)*1.4;
    float h=.3+h11(seed+2.)*.5;
    float w=.025+h11(seed+3.)*.04;
    // Rotation
    float ca=cos(a1),sa=sin(a1),cb=cos(a2),sb=sin(a2);
    vec3 q=p;
    q.xz=mat2(ca,-sa,sa,ca)*q.xz;
    q.yz=mat2(cb,-sb,sb,cb)*q.yz;
    return sdBox(q,vec3(w,h,w));
}

// Geode bowl: ring of crystals growing inward from a sphere shell
vec2 scene(vec3 p){
    // Outer void / rock shell — hollow sphere, interior
    float shell=abs(length(p)-1.8)-.12; // hollow sphere shell
    vec2 res=vec2(shell*crystalDensity, 99.); // mat 99 = rock

    // Crystals on the inner surface of the sphere, growing inward
    float cs=crystalDensity;
    for(int i=0;i<16;i++){
        float s=float(i)*7.31;
        // Distribute on sphere surface (golden angle)
        float phi=float(i)*2.399963;
        float ct=cos(float(i)*.7+.3);
        float st=sqrt(1.-ct*ct);
        vec3 base=vec3(st*cos(phi),ct,st*sin(phi))*1.65*cs;
        // Orient crystal along sphere normal (pointing inward)
        vec3 lp=p-base;
        // Rotate to sphere normal frame
        vec3 nrm=normalize(base);
        // Find rotation to align Y with -nrm
        vec3 up=vec3(0.,1.,0.);
        vec3 axis=cross(up,-nrm);
        float cosA=dot(up,-nrm);
        float sinA=length(axis);
        axis=normalize(axis+vec3(.0001));
        // Rodrigues rotation
        lp=lp*cosA+cross(axis,lp)*sinA+axis*dot(axis,lp)*(1.-cosA);
        float d=sdCrystal(lp/cs,s)*cs;
        if(d<res.x) res=vec2(d,float(i));
    }

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

// 4-color jewel palette cycling
vec3 crystalCol(float id,float t){
    // amethyst, sapphire, rose-quartz, citrine
    float hue=fract(h11(id)*.8+t*.07);
    if(hue<.25) return mix(vec3(.65,0.,1.),vec3(0.,.4,1.),hue*4.);
    if(hue<.5)  return mix(vec3(0.,.4,1.),vec3(1.,.15,.5),hue*4.-1.);
    if(hue<.75) return mix(vec3(1.,.15,.5),vec3(1.,.75,0.),hue*4.-2.);
    return mix(vec3(1.,.75,0.),vec3(.65,0.,1.),hue*4.-3.);
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.5;

    float ang=t*camSpin;
    float pitch=.35+.1*sin(t*.11);
    vec3 ro=vec3(sin(ang)*cos(pitch)*2.6,sin(pitch)*2.6,cos(ang)*cos(pitch)*2.6);
    vec3 fwd=normalize(-ro);
    vec3 right=normalize(cross(fwd,vec3(0.,1.,0.)));
    vec3 up_=cross(right,fwd);
    vec3 rd=normalize(fwd+uv.x*right+uv.y*up_);

    float d=.1; float matId=-1.;
    for(int i=0;i<80;i++){
        vec2 r=scene(ro+rd*d);
        if(r.x<.001){matId=r.y;break;}
        if(d>8.)break;
        d+=r.x*.8;
    }

    // Deep void background — dark purple-black
    vec3 col=vec3(.004,.002,.008);

    if(matId>=0.){
        vec3 pos=ro+rd*d;
        vec3 n=calcNor(pos);
        if(matId>90.){
            // Rock shell — dark, barely visible
            col=vec3(.01,.008,.012);
        } else {
            vec3 bc=crystalCol(matId,t);
            // Caustic sparkle: sharp specular lobe
            vec3 lightDir=normalize(vec3(sin(t*.3)*.5,1.,cos(t*.3)*.5));
            float spec=pow(max(0.,dot(reflect(-lightDir,n),-rd)),32.)*.8;
            float sparkle=pow(max(0.,dot(reflect(-lightDir,n),-rd)),128.)*2.5;
            sparkle*=(.5+.5*sin(t*sparkleRate*3.+matId*5.1));
            float diff=max(0.,dot(n,lightDir))*.3;
            // Inner glow from geode center
            float innerGlow=max(0.,dot(n,-normalize(pos)))*.5;
            col=bc*(diff+innerGlow+.5)*glowPeak*audio
                +vec3(1.,1.,1.)*spec*glowPeak*.4
                +bc*sparkle*audio;
            float edge=fwidth(scene(pos).x);
            col*=smoothstep(0.,edge*4.,scene(pos).x+.001);
        }
    }

    col*=1.-smoothstep(.6,1.,length(uv)*.85);
    gl_FragColor=vec4(col,1.);
}
