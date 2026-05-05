/*{
  "DESCRIPTION": "Aztec Calendar — 2D procedural Aztec sun stone: concentric carved rings with glyph-like radial notches, crimson-gold-black palette",
  "CREDIT": "ShaderClaw auto-improve v9",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"ringCount","LABEL":"Ring Count","TYPE":"float","MIN":3.0,"MAX":10.0,"DEFAULT":6.0},
    {"NAME":"glyphCount","LABEL":"Glyph Segments","TYPE":"float","MIN":8.0,"MAX":32.0,"DEFAULT":20.0},
    {"NAME":"spinSpeed","LABEL":"Spin Speed","TYPE":"float","MIN":0.0,"MAX":0.5,"DEFAULT":0.06},
    {"NAME":"stoneColor","LABEL":"Stone Color","TYPE":"color","DEFAULT":[0.62,0.12,0.02,1.0]},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.5},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/min(RENDERSIZE.x,RENDERSIZE.y);
    float r=length(uv);
    float angle=atan(uv.y,uv.x);
    float t=TIME;
    float audio=1.0+audioBass*audioReact*.4;

    // 4-color Aztec palette: void black, carved stone, crimson, gold
    vec3 VOID  =vec3(0.0,0.0,0.0);
    vec3 STONE =stoneColor.rgb;
    vec3 GOLD  =vec3(1.0,0.80,0.0);
    vec3 INK   =vec3(0.0,0.0,0.01);

    vec3 col=VOID;

    float Rc=clamp(ringCount,3.0,10.0);
    float outerR=0.52;
    float innerR=0.05;

    // Stone disk base
    float diskMask=step(r,outerR)*step(innerR,r);
    col=mix(col,STONE*hdrPeak,diskMask);

    // ── concentric carved rings ───────────────────────────────────────
    for(int i=0;i<10;i++){
        if(float(i)>=Rc) break;
        float fi=float(i);
        float ringR=innerR+(outerR-innerR)*(fi+1.0)/(Rc+1.0);
        float d=abs(r-ringR);
        float aa=fwidth(d);
        float w=0.008+fi/Rc*.006;
        // Carved grooves: dark incised lines
        col=mix(col,INK,smoothstep(w+aa,w*.2,d)*diskMask);
    }

    // ── radial glyph notches ──────────────────────────────────────────
    float Ng=clamp(glyphCount,8.0,32.0);
    float glyph_ang=6.28318/Ng;
    float spin=t*spinSpeed*(1.0+audioBass*audioReact*.3);

    // Outer ring: large notches
    float angO=mod(angle+spin,glyph_ang);
    float notchO=min(angO,glyph_ang-angO)*r;
    float noaa=fwidth(notchO);
    float outerBand=step(outerR-0.08,r)*step(r,outerR);
    col=mix(col,INK,smoothstep(0.012+noaa,0.004,notchO)*outerBand);

    // Inner band: smaller triangular notches (half angular period)
    float Ni=Ng*2.0;
    float glyph_ang2=6.28318/Ni;
    float angI=mod(angle-spin*.7,glyph_ang2);
    float notchI=min(angI,glyph_ang2-angI)*r;
    float niaa=fwidth(notchI);
    float innerBand=step(innerR+0.06,r)*step(r,innerR+0.14);
    col=mix(col,INK,smoothstep(0.008+niaa,0.002,notchI)*innerBand);

    // ── sun face: central circle + radial spikes ──────────────────────
    // Sun spikes (ray burst around center disk)
    float rayAng=mod(angle+spin*1.5+3.14159/glyphCount,6.28318/8.0);
    float rayD=min(rayAng,6.28318/8.0-rayAng)*r;
    float raa=fwidth(rayD);
    float rayZone=step(innerR,r)*step(r,innerR+0.045);
    col=mix(col,GOLD*hdrPeak,smoothstep(0.010+raa,0.002,rayD)*rayZone);

    // Center disk: gold face
    float centD=r-innerR;
    float caa=fwidth(centD);
    col=mix(col,GOLD*hdrPeak*audio,smoothstep(caa,0.0,centD));
    // Black center eye
    col=mix(col,INK,smoothstep(0.015+caa,0.015,r));

    // ── outer frame: thick black border ──────────────────────────────
    float frameD=r-outerR;
    float faa=fwidth(frameD);
    col=mix(col,INK,smoothstep(faa,0.0,frameD-0.0));  // mask outside
    float borderD=abs(r-outerR);
    float baa=fwidth(borderD);
    col=mix(col,INK,smoothstep(.012+baa,.003,borderD)*step(r,outerR+.005));

    gl_FragColor=vec4(col,1.0);
}
