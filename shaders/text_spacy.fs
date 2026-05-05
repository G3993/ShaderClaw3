/*{
  "DESCRIPTION": "Gas Giant Storm — 3D close-up gas planet bands; spiraling eye vortex with amber/teal/violet HDR storm palette",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"bandSpeed","TYPE":"float","DEFAULT":0.15,"MIN":0.02,"MAX":0.5,"LABEL":"Band Drift"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.2,"MIN":1.0,"MAX":3.5,"LABEL":"HDR Glow"},
    {"NAME":"stormSize","TYPE":"float","DEFAULT":0.22,"MIN":0.05,"MAX":0.45,"LABEL":"Eye Size"},
    {"NAME":"turbulence","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":2.0,"LABEL":"Turbulence"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"}
  ]
}*/

float h11(float n){return fract(sin(n*127.1)*43758.5453);}
float h21(vec2 p){return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453);}

float vnoise(vec2 p){
    vec2 i=floor(p),f=fract(p);
    f=f*f*(3.-2.*f);
    return mix(mix(h21(i),h21(i+vec2(1.,0.)),f.x),
               mix(h21(i+vec2(0.,1.)),h21(i+vec2(1.,1.)),f.x),f.y);
}
float fbm2(vec2 p){
    return vnoise(p)*.5+vnoise(p*2.)*0.25+vnoise(p*4.)*0.125;
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.6;

    // Sphere surface via SDF: camera at (0,0,2), sphere at origin r=1
    vec3 ro=vec3(0.,0.,2.0);
    vec3 rd=normalize(vec3(uv,-1.2));

    // Ray-sphere intersection
    vec3 oc=ro; float b=dot(oc,rd);
    float det=b*b-dot(oc,oc)+1.0;
    vec3 col=vec3(.002,.002,.005); // space black background

    if(det>0.){
        float dist=-b-sqrt(det);
        vec3 pos=ro+rd*dist;
        vec3 n=normalize(pos);

        // Spherical coords for surface
        float lat=asin(n.y)/3.14159+.5;
        float lon=atan(n.z,n.x)/6.28318;

        // Animated band coordinate
        float bandCoord=lat*8.+t*bandSpeed;
        float warp=fbm2(vec2(lon*3.+t*.05,bandCoord*.5))*turbulence*.25;
        float bandFinal=fract(bandCoord+warp);

        // Eye of the storm (oval region near equator-center)
        float eyeLon=fract(lon-t*.03);
        float eyeLat=abs(lat-.5);
        float eyeDist=length(vec2(eyeLon-.5,eyeLat*.8))*2.5;
        float inEye=1.-smoothstep(stormSize*.4,stormSize*.8,eyeDist);
        float eyeSwirl=atan(lat-.5,eyeLon-.5)*3.+t*1.1;

        // Band color palette: amber / cream / teal / violet
        vec3 amber=vec3(1.,.55,.05);
        vec3 cream=vec3(1.,.9,.6);
        vec3 teal=vec3(0.,.75,.6);
        vec3 violet=vec3(.5,.1,.9);
        vec3 dark=vec3(.04,.03,.06);

        float b4=fract(bandFinal*2.);
        vec3 bandCol;
        if(b4<.25)      bandCol=mix(dark,amber,b4*4.);
        else if(b4<.5)  bandCol=mix(amber,cream,(b4-.25)*4.);
        else if(b4<.75) bandCol=mix(cream,teal,(b4-.5)*4.);
        else            bandCol=mix(teal,violet,(b4-.75)*4.);

        // Eye region: deep dark swirl with bright rim
        float swirl=fbm2(vec2(eyeSwirl*.3,eyeDist*2.+t*.2))*.5;
        vec3 eyeCol=mix(dark,vec3(.6,.3,1.),swirl)*inEye;

        // HDR limb darkening (bright at center, dark at rim)
        float limb=max(0.,dot(n,-rd));
        float limbPow=pow(limb,.5)*.4+.6;

        vec3 surface=mix(bandCol,eyeCol,inEye);
        surface*=limbPow*glowPeak*audio;

        // Specular highlight: one key light
        vec3 ldir=normalize(vec3(.6,.4,1.));
        float spec=pow(max(0.,dot(reflect(-ldir,n),-rd)),12.)*.4;
        surface+=cream*spec*glowPeak*.5*audio;

        // Atmosphere: faint cyan rim
        float rim=pow(1.-limb,4.)*glowPeak*.4;
        surface+=teal*rim*audio;

        col=surface;
    }

    col*=1.-smoothstep(.6,1.,length(uv)*.75);
    gl_FragColor=vec4(col,1.);
}
