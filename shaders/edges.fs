/*{
  "DESCRIPTION": "Neon Web — 2D glowing spider-silk web with electric blue threads on void black, radial spokes + concentric rings with fwidth AA",
  "CREDIT": "ShaderClaw auto-improve v9",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"spokes","LABEL":"Spokes","TYPE":"float","MIN":4.0,"MAX":24.0,"DEFAULT":10.0},
    {"NAME":"rings","LABEL":"Rings","TYPE":"float","MIN":2.0,"MAX":12.0,"DEFAULT":6.0},
    {"NAME":"silkWidth","LABEL":"Silk Width","TYPE":"float","MIN":0.001,"MAX":0.016,"DEFAULT":0.0045},
    {"NAME":"silkColor","LABEL":"Silk Color","TYPE":"color","DEFAULT":[0.25,0.65,1.0,1.0]},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.5},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/min(RENDERSIZE.x,RENDERSIZE.y);
    float audio=1.0+audioBass*audioReact*.5;
    float r=length(uv);
    float angle=atan(uv.y,uv.x);

    vec3 VOID  =vec3(0.0,0.0,0.008);
    vec3 SILK  =silkColor.rgb;
    vec3 INDIGO=vec3(0.05,0.02,0.18);

    vec3 col=VOID;

    // radial spoke SDF: arc distance to nearest spoke line
    float N=clamp(spokes,4.0,24.0);
    float sectorAng=6.28318/N;
    float localAng=mod(angle+3.14159,6.28318);
    float sPos=mod(localAng,sectorAng);
    float spokeArcDist=min(sPos,sectorAng-sPos)*r;
    float saa=fwidth(spokeArcDist);
    float sw=silkWidth*(0.5+r*.5);
    float spokeMask=smoothstep(sw+saa,sw*.2,spokeArcDist)*step(0.04,r)*step(r,0.52);

    // concentric ring SDF
    float Rc=clamp(rings,2.0,12.0);
    float ringSpacing=0.46/Rc;
    float rPhase=mod(r-0.04,ringSpacing);
    float ringD=min(rPhase,ringSpacing-rPhase);
    float raa=fwidth(ringD);
    float rw=silkWidth*1.2;
    float ringMask=smoothstep(rw+raa,rw*.2,ringD)*step(0.04,r)*step(r,0.52);

    float web=max(spokeMask,ringMask);
    col+=SILK*web*hdrPeak*audio;

    // phosphorescent halo
    col+=SILK*exp(-spokeArcDist*spokeArcDist/(sw*sw*18.0))*spokeMask*.35*hdrPeak*step(0.04,r)*step(r,0.52);
    col+=SILK*exp(-ringD*ringD/(rw*rw*18.0))*ringMask*.35*hdrPeak*step(0.04,r)*step(r,0.52);

    // central hub: glowing ring + black ink core
    float hubR=0.032;
    float hubD=abs(r-hubR);
    float haa=fwidth(hubD);
    col+=SILK*smoothstep(haa*2.0,0.0,hubD-hubR*.15)*hdrPeak*1.8*audio;
    col=mix(col,VOID,smoothstep(hubR*.4+haa,hubR*.4-haa,r));

    // outer frame ring
    float frameD=abs(r-0.54);
    float faa=fwidth(frameD);
    col+=SILK*smoothstep(silkWidth*2.0+faa,silkWidth*.3,frameD)*hdrPeak*.6;

    // ambient indigo glow
    col+=INDIGO*exp(-r*r*4.0)*.4;

    // breathing pulse driven by audio
    col*=0.9+0.1*sin(TIME*1.1+audioBass*audioReact*2.0);

    gl_FragColor=vec4(col,1.0);
}
