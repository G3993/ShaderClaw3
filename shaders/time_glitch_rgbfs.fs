/*{
  "DESCRIPTION": "Neon Mandala Engine — 2D SDF rotating mandala with petal rings and radiating spokes; saffron/crimson/gold/violet on black ink",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "CREDIT": "auto-improve v3",
  "INPUTS": [
    {"NAME":"petalCount","TYPE":"float","DEFAULT":8.0,"MIN":3.0,"MAX":16.0,"LABEL":"Petals"},
    {"NAME":"ringCount","TYPE":"float","DEFAULT":4.0,"MIN":2.0,"MAX":7.0,"LABEL":"Rings"},
    {"NAME":"glowPeak","TYPE":"float","DEFAULT":2.5,"MIN":1.0,"MAX":4.0,"LABEL":"HDR Glow"},
    {"NAME":"spinSpeed","TYPE":"float","DEFAULT":0.12,"MIN":0.0,"MAX":0.5,"LABEL":"Spin"},
    {"NAME":"audioMod","TYPE":"float","DEFAULT":0.8,"MIN":0.0,"MAX":2.0,"LABEL":"Audio React"}
  ]
}*/

#define PI 3.14159265
#define TAU 6.28318530

float h11(float n){return fract(sin(n*127.1)*43758.5453);}

// Radial SDF: return signed distance to a polygon petal in polar space
float petalSDF(vec2 p,float N,float R,float r,float phase){
    float ang=atan(p.y,p.x)-phase;
    float step_=TAU/N;
    ang=mod(ang,step_)-step_*.5; // fold into one petal
    vec2 pFold=vec2(length(p)*cos(ang),length(p)*sin(ang));
    // Ellipse approximation for petal
    float ellA=R*.4,ellB=R*.22;
    vec2 c=vec2(R*.6,0.); // center of petal
    return length((pFold-c)/vec2(ellA,ellB))-1.;
}

// Ring iso-line (thin circle)
float ringLine(float r,float R,float width){
    return abs(r-R)-width;
}

// Spoke (thin line from center outward)
float spoke(vec2 p,float N,float phase,float width){
    float ang=atan(p.y,p.x)-phase;
    float step_=TAU/N;
    ang=mod(ang,step_);
    ang=min(ang,step_-ang);
    return ang*length(p)-width; // approximate arc→chord width
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/RENDERSIZE.y;
    float t=TIME;
    float audio=1.+audioLevel*audioMod+audioBass*audioMod*.7;
    float r=length(uv);
    float ang=atan(uv.y,uv.x);

    // 4 ring radii
    float R0=.18, R1=.32, R2=.47, R3=.65;

    float spin=t*spinSpeed;

    // ---- RINGS ----
    float ring0=ringLine(r,R0,.007);
    float ring1=ringLine(r,R1,.008);
    float ring2=ringLine(r,R2,.009);
    float ring3=ringLine(r,R3,.010);

    // ---- PETALS on each ring ----
    int N=int(min(petalCount,16.));
    float petal0=petalSDF(uv,petalCount,R0,.06,spin*.7);
    float petal1=petalSDF(uv,petalCount,R1,.10,spin*1.1+PI/petalCount);
    float petal2=petalSDF(uv,petalCount,R2,.14,-spin*.9);
    float petal3=petalSDF(uv,petalCount,R3,.18,spin*1.3+PI/petalCount*2.);

    // ---- SPOKES ----
    float sp0=spoke(uv,petalCount,spin*.5,.003);
    float sp1=spoke(uv,petalCount*2.,-spin*.4+.05,.002);

    // ---- CENTRAL DISC ----
    float core=r-.06;

    // Palette: saffron, crimson, gold, violet — 4 colors, fully saturated
    vec3 saffron=vec3(1.,.55,.0);
    vec3 crimson=vec3(1.,.03,.1);
    vec3 gold=vec3(1.,.8,.0);
    vec3 violet=vec3(.55,.0,1.);

    vec3 col=vec3(0.); // black

    // Accumulate SDF layers with smooth anti-aliased edges
    float aa=fwidth(r)*.5+.0015;

    // Ring 0: violet
    col+=violet *smoothstep(aa,-aa,ring0)*glowPeak*audio;
    // Ring 1: saffron
    col+=saffron*smoothstep(aa,-aa,ring1)*glowPeak*audio;
    // Ring 2: gold
    col+=gold   *smoothstep(aa,-aa,ring2)*glowPeak*audio;
    // Ring 3: crimson
    col+=crimson*smoothstep(aa,-aa,ring3)*glowPeak*audio;

    // Petals — per-ring with alternating colors + audio pulse
    float petalAA=aa;
    col+=saffron*smoothstep(petalAA,-petalAA,petal0)*glowPeak*.9*audio;
    col+=gold   *smoothstep(petalAA,-petalAA,petal1)*glowPeak*.8*audio;
    col+=violet *smoothstep(petalAA,-petalAA,petal2)*glowPeak*.85*audio;
    col+=crimson*smoothstep(petalAA,-petalAA,petal3)*glowPeak*.9*audio;

    // Spokes
    col+=gold   *smoothstep(aa,-aa,sp0)*glowPeak*.7*audio;
    col+=violet *smoothstep(aa,-aa,sp1)*glowPeak*.5*audio;

    // Core disc
    col+=mix(saffron,crimson,.5+.5*sin(t*2.))*smoothstep(aa,-aa,core)*glowPeak*audio;

    // Outer ring glow halo
    int NR=int(min(ringCount,7.));
    float rr=R3;
    for(int i=0;i<7;i++){
        if(i>=NR)break;
        float ri=R0+float(i)*(R3-R0)/max(float(NR-1),1.);
        vec3 hcol=(i==0)?violet:(i==1)?saffron:(i==2)?gold:crimson;
        float halo=exp(-abs(r-ri)*15.)*glowPeak*.08*audio;
        col+=hcol*halo;
    }

    // Vignette + outer fade to black (hard edge)
    col*=smoothstep(R3+.12,R3-.02,r);
    col*=1.-smoothstep(.55,.85,r);

    gl_FragColor=vec4(col,1.);
}
