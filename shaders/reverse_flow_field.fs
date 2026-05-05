/*{
  "DESCRIPTION": "Sumi-e Storm — 2D sumi-e ink wash calligraphy: thick black ink strokes on vivid crimson-gold ground, bold brushwork",
  "CREDIT": "ShaderClaw auto-improve v9",
  "CATEGORIES": ["Generator", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"strokeCount","LABEL":"Stroke Count","TYPE":"float","MIN":3.0,"MAX":12.0,"DEFAULT":7.0},
    {"NAME":"inkDark","LABEL":"Ink Darkness","TYPE":"float","MIN":0.5,"MAX":2.0,"DEFAULT":1.2},
    {"NAME":"groundColor","LABEL":"Ground Color","TYPE":"color","DEFAULT":[0.75,0.08,0.02,1.0]},
    {"NAME":"hdrPeak","LABEL":"HDR Peak","TYPE":"float","MIN":1.0,"MAX":4.0,"DEFAULT":2.2},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0}
  ]
}*/

float h11(float p){p=fract(p*.1031);p*=p+33.33;p*=p+p;return fract(p);}

float strokeSDF(vec2 p,vec2 a,vec2 b,float w){
    vec2 pa=p-a,ba=b-a;
    float h=clamp(dot(pa,ba)/dot(ba,ba),0.,1.);
    return length(pa-ba*h)-w;
}

void main(){
    vec2 uv=(gl_FragCoord.xy-RENDERSIZE*.5)/min(RENDERSIZE.x,RENDERSIZE.y);
    float aspect=RENDERSIZE.x/max(RENDERSIZE.y,1.0);
    vec2 uvA=vec2(uv.x*aspect,uv.y);

    float audio=1.0+audioBass*audioReact*.4;
    float t=TIME;

    vec3 GROUND=groundColor.rgb;
    vec3 GOLD  =vec3(1.0,0.78,0.0);
    vec3 INK   =vec3(0.0,0.0,0.01);

    // Ground: crimson bottom → gold top
    float yBias=uv.y*.5+.5;
    vec3 col=mix(GROUND,GOLD*.9,yBias*.45)*hdrPeak;

    // Bokashi gold band
    col+=GOLD*exp(-abs(uv.y-.1)*12.0)*.5*hdrPeak*.4;

    // ── thick sumi-e brushstrokes ─────────────────────────────────────
    int N=int(clamp(strokeCount,3.0,12.0));
    for(int i=0;i<12;i++){
        if(i>=N) break;
        float fi=float(i);
        float ox=(h11(fi*7.31)-.5)*aspect*1.6;
        float oy=(h11(fi*3.17)-.5)*1.2;
        float ang=h11(fi*11.3)*3.14159-1.5708;
        float len=0.2+h11(fi*5.7)*.45;
        float sway=sin(t*(.15+h11(fi*9.1)*.2)+fi*2.3)*.04;
        vec2 ca=vec2(ox+cos(ang)*len*.5+sway,oy+sin(ang)*len*.5);
        vec2 cb=vec2(ox-cos(ang)*len*.5-sway,oy-sin(ang)*len*.5);
        float w=0.012+h11(fi*13.7)*.025;
        float d=strokeSDF(uvA,ca,cb,w);
        float aa=fwidth(d);
        // Core ink edge
        col=mix(col,INK*inkDark,smoothstep(aa,0.0,d));
        // Dry-brush boundary
        col=mix(col,INK*.5,smoothstep(w*4.0+aa,w*1.5,d)*smoothstep(aa,0.0,-d)*.4);
    }

    // Red wax seal (focal point)
    float sR=length(uv-vec2(0.08,-0.12));
    float saa=fwidth(sR-0.04);
    col=mix(col,GROUND*hdrPeak*1.8,smoothstep(saa,0.0,sR-0.04));
    col=mix(col,INK,smoothstep(.013+saa,.013,sR));

    // Gold accent line (ideograph-inspired horizontal stroke)
    float hLine=abs(uv.y-(-.05+sin(t*.12)*.02));
    float haa=fwidth(hLine);
    float inRange=smoothstep(-aspect*.35,-aspect*.15,uv.x)*smoothstep(aspect*.45,aspect*.25,uv.x);
    col+=GOLD*smoothstep(.003+haa,.001,hLine)*inRange*hdrPeak*.6;

    gl_FragColor=vec4(col,1.0);
}
