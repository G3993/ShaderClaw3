/*{
  "DESCRIPTION": "Woodblock Bloom — 2D concentric ink-ring bursts, Japanese woodblock print palette: vermillion ground, black ink, gold highlights",
  "CREDIT": "ShaderClaw auto-improve v8",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"ringCount","LABEL":"Ring Count","TYPE":"float","MIN":2.0,"MAX":16.0,"DEFAULT":8.0},
    {"NAME":"expandSpeed","LABEL":"Expand Speed","TYPE":"float","MIN":0.1,"MAX":3.0,"DEFAULT":0.55},
    {"NAME":"inkThick","LABEL":"Ink Thickness","TYPE":"float","MIN":0.003,"MAX":0.06,"DEFAULT":0.016},
    {"NAME":"spokeN","LABEL":"Spokes","TYPE":"float","MIN":4.0,"MAX":24.0,"DEFAULT":12.0},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.5},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

float h11(float p){p=fract(p*.1031);p*=p+33.33;p*=p+p;return fract(p);}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/min(RENDERSIZE.x,RENDERSIZE.y);
    float audio=1.0+audioBass*audioReact*.5;

    // 4-color woodblock palette
    vec3 BG   =vec3(0.72,0.07,0.03);   // deep vermillion ground
    vec3 INK  =vec3(0.00,0.00,0.015);  // near-black sumi ink
    vec3 GOLD =vec3(1.0, 0.82,0.0);    // shining gold
    vec3 BONE =vec3(0.97,0.93,0.82);   // bone white

    vec3 col=BG;
    float r=length(uv);
    float angle=atan(uv.y,uv.x);

    // ── concentric ink rings ──────────────────────────────────────────
    int N=int(clamp(ringCount,2.0,16.0));
    float period=2.8;
    for(int i=0;i<16;i++){
        if(i>=N) break;
        float fi=float(i);
        float phase=h11(fi*7.31)*period;
        float t=mod(TIME*expandSpeed+phase,period);
        float radius=t/period*0.58*audio;
        float d=abs(r-radius);
        float aa=fwidth(d);
        float fade=1.0-t/period;

        // alternating ink / gold rings
        vec3 rc=(mod(fi,2.0)<1.0) ? INK : GOLD;
        float ring=smoothstep(inkThick+aa,inkThick*0.25,d)*fade;
        col=mix(col,rc*hdrPeak,ring);

        // soft bleed halo
        col+=rc*exp(-d*d/(inkThick*inkThick*6.0))*fade*0.25*hdrPeak;
    }

    // ── radial spokes (woodblock carved lines) ────────────────────────
    float sectorAng=3.14159265*2.0/clamp(spokeN,4.0,24.0);
    float localAng=mod(angle+3.14159265,3.14159265*2.0);
    float sectorPos=mod(localAng,sectorAng);
    float spokeD=min(sectorPos,sectorAng-sectorPos)*r;
    float saa=fwidth(spokeD);
    float spkW=inkThick*0.7;
    float spokeMask=smoothstep(spkW+saa,spkW*0.2,spokeD)*(1.0-smoothstep(0.04,0.05,r))*(1.0-smoothstep(0.48,0.52,r));
    // only between hub and outer rim
    spokeMask*=step(0.04,r)*step(r,0.50);
    col=mix(col,INK,spokeMask*0.65);

    // ── central focal hub: gold circle, black ink core ────────────────
    float outerGold=smoothstep(fwidth(r-0.042)+0.042,0.042,r);
    float innerBlack=smoothstep(0.022+fwidth(r-0.022),0.022,r);
    col=mix(col,GOLD*hdrPeak*1.6,outerGold*(1.0-innerBlack));
    col=mix(col,INK,innerBlack);

    // ── outer border ink ring ─────────────────────────────────────────
    float borderD=abs(r-0.52);
    float baa=fwidth(borderD);
    col=mix(col,INK,smoothstep(inkThick*1.5+baa,inkThick*0.3,borderD));

    gl_FragColor=vec4(col,1.0);
}
