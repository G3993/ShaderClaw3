/*{
  "DESCRIPTION": "TEE — Trusted Execution Environment. Visualizes data entering a secure enclave as a star, processing as orbiting particles, and exiting.",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "particleCount", "TYPE": "float", "DEFAULT": 150.0, "MIN": 10.0, "MAX": 250.0, "LABEL": "Particles" },
    { "NAME": "starCount", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 6.0, "LABEL": "Stars" },
    { "NAME": "boxSize", "TYPE": "float", "DEFAULT": 1.2, "MIN": 0.5, "MAX": 2.5, "LABEL": "Box Size" },
    { "NAME": "camDist", "TYPE": "float", "DEFAULT": 13.0, "MIN": 5.0, "MAX": 25.0, "LABEL": "Camera Distance" },
    { "NAME": "cycleSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0, "LABEL": "Cycle Speed" },
    { "NAME": "edgeEnergy", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.5, "LABEL": "Edge Energy" },
    { "NAME": "energySpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0, "LABEL": "Energy Speed" },
    { "NAME": "edgeColor", "TYPE": "color", "DEFAULT": [0.91, 0.25, 0.34, 1.0], "LABEL": "Edge Color" },
    { "NAME": "glowColor", "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0], "LABEL": "Glow Color" }
  ]
}*/

#define TAU 6.2831853
#define MAX_STEPS 80
#define SURF 0.001
#define FAR 30.0
#define MAX_PARTICLES 250

mat2 rot(float a){float c=cos(a),s=sin(a);return mat2(c,-s,s,c);}
float hash(float n){return fract(sin(n)*43758.5453);}
vec3 hash3(float n){return vec3(hash(n),hash(n+137.0),hash(n+274.0));}

float sdSeg(vec3 p,vec3 a,vec3 b,float r){
  vec3 ab=b-a,ap=p-a;
  float t=clamp(dot(ap,ab)/dot(ab,ab),0.0,1.0);
  return length(ap-ab*t)-r;
}

vec2 sdSegT(vec3 p,vec3 a,vec3 b,float r){
  vec3 ab=b-a,ap=p-a;
  float t=clamp(dot(ap,ab)/dot(ab,ab),0.0,1.0);
  return vec2(length(ap-ab*t)-r, t);
}

vec3 rB(vec3 p){
  // Mouse controls box rotation: mousePos is 0-1 normalized
  float ax=(mousePos.x-0.5)*TAU; // full rotation range on X
  float ay=(mousePos.y-0.5)*TAU*0.5; // half rotation range on Y
  p.xz*=rot(ax);
  p.yz*=rot(ay);
  return p;
}

float wireFrame(vec3 p){
  p=rB(p);
  float s=boxSize;
  float r=0.012;
  float d=1e10;
  d=min(d,sdSeg(p,vec3(-s,-s,-s),vec3(s,-s,-s),r));
  d=min(d,sdSeg(p,vec3(s,-s,-s),vec3(s,-s,s),r));
  d=min(d,sdSeg(p,vec3(s,-s,s),vec3(-s,-s,s),r));
  d=min(d,sdSeg(p,vec3(-s,-s,s),vec3(-s,-s,-s),r));
  d=min(d,sdSeg(p,vec3(-s,s,-s),vec3(s,s,-s),r));
  d=min(d,sdSeg(p,vec3(s,s,-s),vec3(s,s,s),r));
  d=min(d,sdSeg(p,vec3(s,s,s),vec3(-s,s,s),r));
  d=min(d,sdSeg(p,vec3(-s,s,s),vec3(-s,s,-s),r));
  d=min(d,sdSeg(p,vec3(-s,-s,-s),vec3(-s,s,-s),r));
  d=min(d,sdSeg(p,vec3(s,-s,-s),vec3(s,s,-s),r));
  d=min(d,sdSeg(p,vec3(s,-s,s),vec3(s,s,s),r));
  d=min(d,sdSeg(p,vec3(-s,-s,s),vec3(-s,s,s),r));
  return d;
}

float edgeEnergyField(vec3 p){
  p=rB(p);
  float s=boxSize;
  float r=0.012;
  float energy=0.0;
  float t_=TIME*energySpeed;

  vec2 e0=sdSegT(p,vec3(-s,-s,-s),vec3(s,-s,-s),r);
  energy+=exp(-e0.x*e0.x/0.002)*pow(0.5+0.5*sin(e0.y*TAU*2.0-t_*3.0+0.0),4.0);
  vec2 e1=sdSegT(p,vec3(s,-s,-s),vec3(s,-s,s),r);
  energy+=exp(-e1.x*e1.x/0.002)*pow(0.5+0.5*sin(e1.y*TAU*2.0-t_*3.0+1.57),4.0);
  vec2 e2=sdSegT(p,vec3(s,-s,s),vec3(-s,-s,s),r);
  energy+=exp(-e2.x*e2.x/0.002)*pow(0.5+0.5*sin(e2.y*TAU*2.0-t_*3.0+3.14),4.0);
  vec2 e3=sdSegT(p,vec3(-s,-s,s),vec3(-s,-s,-s),r);
  energy+=exp(-e3.x*e3.x/0.002)*pow(0.5+0.5*sin(e3.y*TAU*2.0-t_*3.0+4.71),4.0);

  vec2 e4=sdSegT(p,vec3(-s,s,-s),vec3(s,s,-s),r);
  energy+=exp(-e4.x*e4.x/0.002)*pow(0.5+0.5*sin(e4.y*TAU*2.0+t_*2.5+0.5),4.0);
  vec2 e5=sdSegT(p,vec3(s,s,-s),vec3(s,s,s),r);
  energy+=exp(-e5.x*e5.x/0.002)*pow(0.5+0.5*sin(e5.y*TAU*2.0+t_*2.5+2.07),4.0);
  vec2 e6=sdSegT(p,vec3(s,s,s),vec3(-s,s,s),r);
  energy+=exp(-e6.x*e6.x/0.002)*pow(0.5+0.5*sin(e6.y*TAU*2.0+t_*2.5+3.64),4.0);
  vec2 e7=sdSegT(p,vec3(-s,s,s),vec3(-s,s,-s),r);
  energy+=exp(-e7.x*e7.x/0.002)*pow(0.5+0.5*sin(e7.y*TAU*2.0+t_*2.5+5.21),4.0);

  vec2 e8=sdSegT(p,vec3(-s,-s,-s),vec3(-s,s,-s),r);
  energy+=exp(-e8.x*e8.x/0.002)*pow(0.5+0.5*sin(e8.y*TAU*1.5-t_*2.0+0.0),4.0);
  vec2 e9=sdSegT(p,vec3(s,-s,-s),vec3(s,s,-s),r);
  energy+=exp(-e9.x*e9.x/0.002)*pow(0.5+0.5*sin(e9.y*TAU*1.5-t_*2.0+1.57),4.0);
  vec2 e10=sdSegT(p,vec3(s,-s,s),vec3(s,s,s),r);
  energy+=exp(-e10.x*e10.x/0.002)*pow(0.5+0.5*sin(e10.y*TAU*1.5-t_*2.0+3.14),4.0);
  vec2 e11=sdSegT(p,vec3(-s,-s,s),vec3(-s,s,s),r);
  energy+=exp(-e11.x*e11.x/0.002)*pow(0.5+0.5*sin(e11.y*TAU*1.5-t_*2.0+4.71),4.0);

  return energy;
}

vec3 getNorm(vec3 p){
  vec2 e=vec2(0.001,0);
  return normalize(vec3(
    wireFrame(p+e.xyy)-wireFrame(p-e.xyy),
    wireFrame(p+e.yxy)-wireFrame(p-e.yxy),
    wireFrame(p+e.yyx)-wireFrame(p-e.yyx)
  ));
}

float march(vec3 ro,vec3 rd){
  float t=0.0;
  for(int i=0;i<MAX_STEPS;i++){
    float d=wireFrame(ro+rd*t);
    if(d<SURF)return t;
    t+=d;
    if(t>FAR)break;
  }
  return -1.0;
}

// 24s cycle (compressed entry for multiple stars)
// 0-3:    stars streak in from below (staggered ~0.4s apart)
// 3:      flash/explode — shockwave ring expands
// 3-7:    particles BLAST outward, slam into box walls, bounce back
// 7-12:   particles orbit chaotically, pressing against walls
// 12-15:  particles converge back to center
// 15:     reform flash
// 15-20:  stars exit downward (staggered)
// 20-24:  pause

#define MAX_STARS 6

float cyc;
float spreadPh;
float convergePh;
float wallHitIntensity;

void phases(){
  spreadPh=smoothstep(3.0,7.0,cyc);
  convergePh=smoothstep(12.0,15.0,cyc);
}

// Bounce function — triangle wave that simulates bouncing off walls
float bounce(float x, float limit){
  float period = limit * 2.0;
  float m = mod(x, period);
  return m < limit ? m : period - m;
}

vec3 partPos(float id, out float wallPress){
  vec3 s=hash3(id*7.31+0.5);
  float limit=boxSize*0.88;

  // Each particle orbits within the box — unique trajectory
  float orbPhase=TIME*(0.15+s.x*0.25);
  float a1=orbPhase+s.y*TAU;
  float a2=orbPhase*0.7+s.x*TAU;
  float a3=orbPhase*0.5+s.z*TAU;

  // Target position — oscillates inside the box, never exceeds limit
  vec3 tgt=vec3(
    sin(a1+s.x*10.0)*limit,
    sin(a2+s.y*10.0)*limit,
    sin(a3+s.z*10.0)*limit
  );

  // Time since explosion
  float del=s.x*0.15+s.z*0.1;
  float raw=smoothstep(del,del+0.15,spreadPh);
  float spd=raw;

  // Convergence
  float cDel=s.z*0.3+s.x*0.2;
  float cRaw=smoothstep(cDel,cDel+0.6,convergePh);
  float conv=cRaw*cRaw;

  float spread=spd*(1.0-conv);

  // Spread from center to target orbit position
  vec3 pos=tgt*spread;

  // Hard clamp — absolute guarantee particles stay inside box
  pos=clamp(pos,vec3(-limit),vec3(limit));

  // Measure how close to walls (for wall glow effect)
  vec3 absP=abs(pos);
  float maxComp=max(absP.x,max(absP.y,absP.z));
  wallPress=smoothstep(limit*0.6,limit*0.95,maxComp)*spread;

  // When converging, pull back to center
  pos=mix(pos,vec3(0.0),conv);

  // Rotate with box
  pos=rB(pos);
  return pos;
}

float partBrt(float id){
  vec3 s=hash3(id*7.31+0.5);
  float pulse=sin(TIME*(1.5+s.y*2.0)+s.x*TAU)*0.12+0.88;
  float del=s.x*0.15+s.z*0.1;
  float fi=smoothstep(del,del+0.05,spreadPh); // fast pop-in
  float cDel=s.z*0.3+s.x*0.2;
  float fo=1.0-smoothstep(cDel+0.3,cDel+0.6,convergePh);
  return pulse*fi*fo;
}

void main(){
  vec2 uv=(gl_FragCoord.xy-0.5*RENDERSIZE)/min(RENDERSIZE.x,RENDERSIZE.y);
  cyc=mod(TIME*cycleSpeed,24.0);
  phases();

  // camera
  float cA=TIME*0.04;
  vec3 ro=vec3(sin(cA)*camDist,2.5,cos(cA)*camDist);
  vec3 ta=vec3(0);
  vec3 ww=normalize(ta-ro);
  vec3 uu=normalize(cross(ww,vec3(0,1,0)));
  vec3 vv=cross(uu,ww);
  vec3 rd=normalize(uv.x*uu+uv.y*vv+2.2*ww);

  vec3 gc=glowColor.rgb;
  vec3 ec=edgeColor.rgb;
  vec3 col=vec3(0);

  // Accumulate wall hit glow from particles
  float totalWallGlow=0.0;

  // PARTICLES: compute first so we know wall pressure for edge glow
  vec3 particleCol=vec3(0);
  float szScale=0.8+0.4*(particleCount/250.0);
  for(int i=0;i<MAX_PARTICLES;i++){
    float fi=float(i);
    // Mask: 1.0 if particle active, 0.0 if beyond count
    float active=step(fi+0.5, particleCount);
    float wallPress;
    vec3 pp=partPos(fi, wallPress);
    float br=partBrt(fi)*active;
    if(br<0.01)continue;
    totalWallGlow+=wallPress*br;
    vec3 toP=pp-ro;
    float tD=dot(toP,rd);
    if(tD<0.0)continue;
    vec3 cl=ro+rd*tD;
    float di=length(cl-pp);
    vec3 sd=hash3(fi*7.31+0.5);
    float szClass=hash(fi*13.7);
    float sz=(szClass<0.3?0.008+sd.y*0.007:szClass<0.7?0.02+sd.y*0.015:0.04+sd.y*0.018)*szScale;
    float core=smoothstep(sz,sz*0.15,di)*br*1.8;
    float halo=exp(-di*di/(sz*sz*0.8))*br*0.2;

    // Particles near walls glow hotter (pressing against the enclave)
    float hotness=1.0+wallPress*2.0;
    vec3 pColor=mix(gc,ec,wallPress*0.6);
    particleCol+=pColor*(core+halo)*hotness*exp(-tD*0.025);
  }

  // Normalize wall glow
  totalWallGlow=clamp(totalWallGlow/max(particleCount*0.1,1.0),0.0,1.0);

  // WIREFRAME: raymarched solid edges with energy + wall pressure glow
  float hit=march(ro,rd);
  if(hit>0.0){
    vec3 p=ro+rd*hit;
    vec3 n=getNorm(p);
    float diff=0.4+0.6*abs(dot(n,normalize(vec3(1,2,3))));
    float fresnel=pow(1.0-abs(dot(n,-rd)),2.0)*0.3;

    vec3 baseEdge=ec*(0.7+fresnel)*diff;
    float eFld=edgeEnergyField(p)*edgeEnergy*spreadPh;

    // Wall pressure makes edges glow brighter — enclave is containing the energy
    float wallBoost=totalWallGlow*2.0;
    vec3 energyCol=mix(ec,gc,(eFld+wallBoost)*0.4)*(1.0+(eFld+wallBoost)*2.0);

    col=mix(baseEdge,energyCol,clamp(eFld+wallBoost,0.0,1.0));
    col+=ec*(eFld+wallBoost*0.5)*0.3*exp(-hit*0.03);
  }

  // Edge volumetric glow (near-miss rays)
  if(hit<0.0 && (edgeEnergy>0.01||totalWallGlow>0.01) && spreadPh>0.0){
    for(int k=0;k<20;k++){
      float ft=float(k)/20.0;
      float sampleT=3.0+ft*(camDist*1.5);
      vec3 sp=ro+rd*sampleT;
      float wd=wireFrame(sp);
      if(wd<0.15){
        float proximity=exp(-wd*wd/0.005);
        float eFld=edgeEnergyField(sp)*edgeEnergy*spreadPh;
        float wallBoost=totalWallGlow*1.5;
        col+=ec*proximity*(eFld+wallBoost)*0.06*exp(-sampleT*0.04);
      }
    }
  }

  // Add particles on top of wireframe
  col+=particleCol;

  // SHOCKWAVE RING at explosion moment (cyc ~3)
  if(cyc>2.5&&cyc<6.0){
    float shockT=cyc-3.0;
    if(shockT>0.0){
      float radius=shockT*boxSize*0.8; // expands outward
      float fade=exp(-shockT*1.5);
      // Ring in 3D — check distance from expanding sphere shell
      vec3 toC=-ro;
      float tC=dot(toC,rd);
      if(tC>0.0){
        vec3 cl=ro+rd*tC;
        float d=length(cl);
        float ring=exp(-pow(d-radius,2.0)/(0.01+shockT*0.02))*fade;
        col+=gc*ring*2.5*exp(-tC*0.02);
        // Inner flash
        float inner=exp(-d*d/(0.02+shockT*0.1))*fade*2.0;
        col+=gc*inner*exp(-tC*0.02);
      }
    }
  }

  // STAR ENTRY: multiple stars streak in from below (0-3s, staggered)
  if(cyc<3.8){
    for(int si=0;si<MAX_STARS;si++){
      float fsi=float(si);
      float active=step(fsi+0.5,starCount);
      if(active<0.5) continue;

      // Each star staggers in ~0.4s apart, takes ~1.5s to arrive
      float delay=fsi*0.4;
      float arriveT=1.5;
      float sT=smoothstep(delay,delay+arriveT,cyc);
      float eased=sT*sT;

      // Slight horizontal offset per star for visual variety
      float offX=sin(fsi*2.1+0.5)*0.8;
      float offZ=cos(fsi*3.7+1.0)*0.8;
      vec3 startP=vec3(offX,-8.0,offZ);
      vec3 sP=mix(startP,vec3(0),eased);

      float sB=smoothstep(delay,delay+0.3,cyc)*(1.0-smoothstep(3.0,3.3,cyc));

      if(sB>0.001){
        vec3 toS=sP-ro;
        float tS=dot(toS,rd);
        if(tS>0.0){
          vec3 cl=ro+rd*tS;
          float di=length(cl-sP);
          float core=smoothstep(0.025,0.0,di)*1.5;
          float sm=exp(-di*di/0.001)*0.4;
          col+=gc*(core+sm)*sB*exp(-tS*0.02);
        }

        // Tail behind each star
        float tailL=1.0*(1.0-eased*0.6);
        vec3 tailDir=normalize(startP-vec3(0));
        for(int j=0;j<15;j++){
          float fj=float(j)/15.0;
          vec3 tp=sP+tailDir*fj*tailL;
          vec3 toT=tp-ro;
          float tT=dot(toT,rd);
          if(tT>0.0){
            vec3 cl=ro+rd*tT;
            float d=length(cl-tp);
            float fade=(1.0-fj);
            float tg=smoothstep(0.02,0.0,d)*fade*fade*0.5;
            col+=gc*0.9*tg*sB*exp(-tT*0.02);
          }
        }
      }
    }
  }

  // ENTRY FLASH at center — brighter, wider
  if(cyc>2.7&&cyc<4.5){
    float flash=exp(-pow(cyc-3.0,2.0)*15.0);
    vec3 toC=-ro;
    float tC=dot(toC,rd);
    if(tC>0.0){
      vec3 cl=ro+rd*tC;
      float d=length(cl);
      col+=gc*exp(-d*d/0.12)*flash*3.0*exp(-tC*0.02);
      // Secondary wider bloom
      col+=gc*0.3*exp(-d*d/0.5)*flash*exp(-tC*0.02);
    }
  }

  // REFORM FLASH at center (before exit)
  if(cyc>14.5&&cyc<16.0){
    float flash=exp(-pow(cyc-15.0,2.0)*25.0);
    vec3 toC=-ro;
    float tC=dot(toC,rd);
    if(tC>0.0){
      vec3 cl=ro+rd*tC;
      float d=length(cl);
      col+=gc*exp(-d*d/0.08)*flash*1.2*exp(-tC*0.02);
    }
  }

  // STAR EXIT: multiple stars descend from center back down (15-20s, staggered)
  if(cyc>14.5&&cyc<20.0){
    for(int si=0;si<MAX_STARS;si++){
      float fsi=float(si);
      float active=step(fsi+0.5,starCount);
      if(active<0.5) continue;

      // Stars leave staggered, reverse order (last in, first out)
      float delay=fsi*0.4;
      float exitStart=15.0+delay;
      float exitEnd=exitStart+2.5;
      float eT=smoothstep(exitStart,exitEnd,cyc);
      float eased=1.0-pow(1.0-eT,2.0);

      // Same offset as entry for visual continuity
      float offX=sin(fsi*2.1+0.5)*0.8;
      float offZ=cos(fsi*3.7+1.0)*0.8;
      vec3 endP=vec3(offX,-8.0,offZ);
      vec3 sP=mix(vec3(0),endP,eased);

      float sB=smoothstep(exitStart,exitStart+0.4,cyc)*(1.0-smoothstep(exitEnd-0.5,exitEnd,cyc));

      if(sB>0.001){
        vec3 toS=sP-ro;
        float tS=dot(toS,rd);
        if(tS>0.0){
          vec3 cl=ro+rd*tS;
          float di=length(cl-sP);
          float core=smoothstep(0.025,0.0,di)*1.5;
          float sm=exp(-di*di/0.001)*0.4;
          col+=gc*(core+sm)*sB*exp(-tS*0.02);
        }

        // Tail behind each exiting star (pointing back up toward center)
        float tailL=0.4+eased*0.8;
        vec3 tailDir=normalize(vec3(0)-endP);
        for(int j=0;j<15;j++){
          float fj=float(j)/15.0;
          vec3 tp=sP+tailDir*fj*tailL;
          vec3 toT=tp-ro;
          float tT=dot(toT,rd);
          if(tT>0.0){
            vec3 cl=ro+rd*tT;
            float d=length(cl-tp);
            float fade=(1.0-fj);
            float tg=smoothstep(0.02,0.0,d)*fade*fade*0.5;
            col+=gc*0.9*tg*sB*exp(-tT*0.02);
          }
        }
      }
    }
  }

  // vignette
  vec2 v=gl_FragCoord.xy/RENDERSIZE;
  col*=1.0-dot((v-0.5)*0.9,(v-0.5)*0.9);
  col=clamp(col,0.0,1.0);
  gl_FragColor=vec4(col,1);
}
