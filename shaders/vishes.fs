/*{
  "DESCRIPTION": "Volcanic Lava Lake — 3D aerial view of lava lake; black obsidian crust with glowing orange/gold/white-hot crack network; fully different from coral reef v2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"crackDensity","TYPE":"float","DEFAULT":1.0,"MIN":0.3,"MAX":2.5,"LABEL":"Crack Density"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":3.0,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"flowSpeed","TYPE":"float","DEFAULT":0.3,"MIN":0.0,"MAX":1.5,"LABEL":"Flow Speed"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"},
    {"NAME":"camHeight","TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":5.0,"LABEL":"Camera Height"}
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

float fbm(vec2 p){
    return vnoise(p)*.5+vnoise(p*2.1+vec2(5.3,1.7))*.25+vnoise(p*4.3+vec2(2.1,8.9))*.125;
}

// Lava crack: Voronoi-based distance field
vec2 voronoi(vec2 p){
    vec2 i=floor(p),f=fract(p);
    float md=8.;
    vec2 mr=vec2(0.);
    for(int y=-1;y<=1;y++){
        for(int x=-1;x<=1;x++){
            vec2 g=vec2(float(x),float(y));
            vec2 o=vec2(h21(i+g),h21(i+g+vec2(3.7,1.9)));
            o=.5+.5*sin(TIME*flowSpeed*.4+6.28*o);
            vec2 r=g+o-f;
            float d=dot(r,r);
            if(d<md){md=d;mr=r;}
        }
    }
    return vec2(sqrt(md),dot(mr,mr));
}

// Crack network: Voronoi cell edges = obsidian; cell interior = dark rock
float crackPattern(vec2 uv,float t){
    float scale=3.*crackDensity;
    // Domain warp for organic feel
    vec2 warp=vec2(fbm(uv*2.+vec2(1.7,9.2)),fbm(uv*2.+vec2(8.3,2.8)))*0.15;
    vec2 wu=uv+warp+vec2(t*.04,t*.02);
    vec2 v=voronoi(wu*scale);
    // Edge proximity (small value = near crack edge)
    return v.x;
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.7;

    // Top-down orthographic camera tilted slightly
    float tiltX=.25;
    vec3 ro=vec3(uv.x*2.5,camHeight,uv.y*2.5-camHeight*sin(tiltX));
    vec3 rd=normalize(vec3(0.,0.,1.)*cos(tiltX)+vec3(0.,1.,0.)*sin(tiltX)*(-1.));

    // We're doing a flat top-down view — the "3D" is from the perspective tilting + camera height varying crack parallax
    // Cast ray onto a horizontal plane at y=0
    float planeD=-ro.y/rd.y;
    vec3 hitPos=ro+rd*planeD;
    vec2 surfaceUV=hitPos.xz;

    // Crack pattern at surface
    float crack=crackPattern(surfaceUV,t);
    float aa=fwidth(crack);

    // Edge threshold: small crack value = bright lava; large = dark obsidian
    float edgeW=.04+.02*sin(t*1.7+surfaceUV.x*3.); // breathing crack width
    float lavaFraction=1.-smoothstep(edgeW*.5,edgeW*2.,crack);

    // Secondary cracks at higher frequency
    float crack2=crackPattern(surfaceUV*2.3+vec2(5.1,3.7),t*.7);
    float lava2=1.-smoothstep(.025,.08,crack2);

    // Palette: black obsidian → deep orange → gold → white-hot
    vec3 obsidian=vec3(.02,.01,.005);
    vec3 deepOrange=vec3(1.,.2,.0);
    vec3 gold=vec3(1.,.65,.0);
    vec3 whiteHot=vec3(1.,.95,.7);

    // Temperature from crack proximity (innermost cracks = white-hot)
    float lavaT=lavaFraction+lava2*.35;
    vec3 lavaCol;
    if(lavaT<.2)       lavaCol=obsidian;
    else if(lavaT<.5)  lavaCol=mix(obsidian,deepOrange,(lavaT-.2)*3.33);
    else if(lavaT<.8)  lavaCol=mix(deepOrange,gold,(lavaT-.5)*3.33);
    else               lavaCol=mix(gold,whiteHot,(lavaT-.8)*5.);

    // HDR boost on hot zones
    vec3 col=lavaCol*glowPeak*audio*lavaT+obsidian*(1.-lavaT)*(.1+lavaT*.9);

    // Subtle parallax: small height variation based on crack depth
    float heightFactor=lavaFraction*.03;
    vec2 parallaxUV=surfaceUV+rd.xz*heightFactor;
    float crack_p=crackPattern(parallaxUV,t);
    float lava_p=1.-smoothstep(edgeW*.5,edgeW*2.,crack_p);
    col=mix(col,col*1.1,abs(lava_p-lavaFraction)*.2);

    // Camera-based vignette
    col*=1.-smoothstep(.5,.9,length(uv)*.85);

    gl_FragColor=vec4(col,1.);
}
