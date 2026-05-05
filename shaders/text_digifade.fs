/*{
  "DESCRIPTION": "Geodesic Arc Orb — 3D raymarched geodesic sphere of glowing struts; electric cyan/magenta/gold arcs orbit through void-black",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"orbRadius","TYPE":"float","DEFAULT":1.0,"MIN":0.5,"MAX":1.8,"LABEL":"Orb Radius"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.8,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"spinSpeed","TYPE":"float","DEFAULT":0.15,"MIN":0.0,"MAX":0.5,"LABEL":"Spin Speed"},
    {"NAME":"arcThick","TYPE":"float","DEFAULT":0.025,"MIN":0.01,"MAX":0.06,"LABEL":"Arc Thickness"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"}
  ]
}*/

float h11(float n){return fract(sin(n*127.1)*43758.5453);}

float sdCapsule(vec3 p,vec3 a,vec3 b,float r){
    vec3 ab=b-a,ap=p-a;
    return length(ap-ab*clamp(dot(ap,ab)/dot(ab,ab),0.,1.))-r;
}

// Geodesic icosahedron vertices (normalized)
vec3 icoVert(int i){
    float phi=1.6180339887; // golden ratio
    float n=1./sqrt(1.+phi*phi);
    float p=phi*n;
    // 12 vertices of icosahedron
    if(i== 0) return vec3( 0., n, p);
    if(i== 1) return vec3( 0., n,-p);
    if(i== 2) return vec3( 0.,-n, p);
    if(i== 3) return vec3( 0.,-n,-p);
    if(i== 4) return vec3( n, p, 0.);
    if(i== 5) return vec3( n,-p, 0.);
    if(i== 6) return vec3(-n, p, 0.);
    if(i== 7) return vec3(-n,-p, 0.);
    if(i== 8) return vec3( p, 0., n);
    if(i== 9) return vec3( p, 0.,-n);
    if(i==10) return vec3(-p, 0., n);
    return vec3(-p, 0.,-n);
}

// Icosahedron edge list: 30 edges
// We use a compact adjacency: connect if dot(a,b) > 0.45 (neighbor vertices)
float sdGeodesic(vec3 p,float R){
    float r=arcThick;
    float minD=1e9;
    for(int i=0;i<12;i++){
        for(int j=i+1;j<12;j++){
            vec3 a=icoVert(i)*R;
            vec3 b=icoVert(j)*R;
            if(dot(normalize(a),normalize(b))>.45){
                float d=sdCapsule(p,a,b,r);
                minD=min(minD,d);
            }
        }
    }
    return minD;
}

// Edge index for color
float edgeId(vec3 p,float R){
    float r=arcThick+.02;
    float minD=1e9; float id=0.;
    for(int i=0;i<12;i++){
        for(int j=i+1;j<12;j++){
            vec3 a=icoVert(i)*R;
            vec3 b=icoVert(j)*R;
            if(dot(normalize(a),normalize(b))>.45){
                float d=sdCapsule(p,a,b,r);
                if(d<minD){minD=d;id=float(i*12+j);}
            }
        }
    }
    return id;
}

vec2 scene(vec3 p){
    float R=orbRadius;
    float geo=sdGeodesic(p,R);
    return vec2(geo,1.);
}

vec3 calcNor(vec3 p){
    float R=orbRadius;
    vec2 e=vec2(.001,0.);
    return normalize(vec3(
        scene(p+e.xyy).x-scene(p-e.xyy).x,
        scene(p+e.yxy).x-scene(p-e.yxy).x,
        scene(p+e.yyx).x-scene(p-e.yyx).x
    ));
}

vec3 edgeColor(float id,float t){
    float v=fract(id*.0137+t*.08);
    if(v<.33)  return mix(vec3(0.,1.,1.),vec3(1.,0.,.8),v*3.);    // cyan→magenta
    if(v<.67)  return mix(vec3(1.,0.,.8),vec3(1.,.8,0.),v*3.-1.); // magenta→gold
    return mix(vec3(1.,.8,0.),vec3(0.,1.,1.),v*3.-2.);            // gold→cyan
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.7;

    // Rotate the whole scene (spin the orb)
    float ang=t*spinSpeed;
    float ca=cos(ang),sa=sin(ang);
    mat3 rotY=mat3(ca,0.,sa,0.,1.,0.,-sa,0.,ca);
    float ang2=t*spinSpeed*.37;
    float ca2=cos(ang2),sa2=sin(ang2);
    mat3 rotX=mat3(1.,0.,0.,0.,ca2,-sa2,0.,sa2,ca2);
    mat3 rot=rotY*rotX;

    vec3 ro=vec3(0.,0.,3.2);
    vec3 rd_=normalize(vec3(uv,-1.3));
    // Apply inverse rotation to ray (equivalent to rotating scene forward)
    vec3 ro2=transpose(rot)*ro;
    vec3 rd2=transpose(rot)*rd_;

    vec3 col=vec3(.002,.001,.004);

    float d=.1; float matId=-1.;
    for(int i=0;i<72;i++){
        vec2 r=scene(ro2+rd2*d);
        if(r.x<.001){matId=r.y;break;}
        if(d>8.)break;
        d+=r.x;
    }

    if(matId>=0.){
        vec3 pos=ro2+rd2*d;
        vec3 n=calcNor(pos);
        float eid=edgeId(pos,orbRadius);
        vec3 ec=edgeColor(eid,t);
        float diff=max(0.,dot(n,normalize(vec3(.4,1.,.5))))*.2+.75;
        col=ec*diff*glowPeak*audio;
        float edge=fwidth(scene(pos).x);
        col*=smoothstep(0.,edge*5.,scene(pos).x+.001);
    }

    // Screen-space ambient glow sphere halo
    float oHalo=exp(-max(0.,length(uv)-orbRadius*.65)*6.)*glowPeak*.08*audio;
    col+=mix(vec3(0.,1.,1.),vec3(1.,0.,.8),sin(t*.3)*.5+.5)*oHalo;

    // Inner core sparkle
    float coreDist=length(uv);
    col+=vec3(.3,.1,.6)*exp(-coreDist*6.)*glowPeak*.06*audio;

    col*=1.-smoothstep(.58,.95,length(uv)*.85);
    gl_FragColor=vec4(col,1.);
}
