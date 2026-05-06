/*{
  "DESCRIPTION": "Recursive Wireframe Room — infinite nested box corridor, HDR cyan and violet wireframe",
  "CREDIT": "ShaderClaw3 auto-improve v16",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    {"NAME": "flySpeed",  "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0},
    {"NAME": "wireWidth", "TYPE": "float", "DEFAULT": 0.04,"MIN": 0.005,"MAX": 0.12},
    {"NAME": "audioMod",  "TYPE": "audio"}
  ]
}*/
precision highp float;
float apeak(){return (audioBass.x+audioBass.y+audioBass.z+audioBass.w)*0.25;}
void main(){
    vec2 uv=(isf_FragNormCoord*2.0-1.0)*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    float ap=apeak();
    float t=TIME*flySpeed;
    float ww=wireWidth*(1.0+ap*0.4);
    vec3 col=vec3(0.0);
    // Perspective-projected nested box frames along Z corridor
    for(int k=0;k<14;k++){
        float zRoom=float(k)*1.8+mod(t,1.8);
        if(zRoom<=0.02)continue;
        float sc=1.0/zRoom;
        float hs=0.85; // half-size
        // Projected AABB in screen space
        float xL=-hs*sc,xR=hs*sc,yB=-hs*sc,yT=hs*sc;
        // Check if pixel is inside the projected rect
        float inRect=step(xL,uv.x)*step(uv.x,xR)*step(yB,uv.y)*step(uv.y,yT);
        // Distance to each edge
        float dL=abs(uv.x-xL),dR=abs(uv.x-xR);
        float dB=abs(uv.y-yB),dT=abs(uv.y-yT);
        float nearH=min(dL,dR);
        float nearV=min(dB,dT);
        float aaH=fwidth(nearH);
        float aaV=fwidth(nearV);
        float edgeH=inRect*smoothstep(aaH*2.0,0.0,nearH-ww);
        float edgeV=inRect*smoothstep(aaV*2.0,0.0,nearV-ww);
        float wire=max(edgeH,edgeV);
        // Corner cross-bars (horizontal stripes for depth cue)
        float crossH=inRect*smoothstep(aaH*2.0,0.0,abs(uv.y)-ww*0.5);
        float crossV=inRect*smoothstep(aaV*2.0,0.0,abs(uv.x)-ww*0.5);
        wire=max(wire,max(crossH,crossV)*0.3);
        // Depth-based color: bright cyan near → violet far
        float depth=float(k)/14.0;
        float bright=exp(-depth*2.2)*(1.5+ap*0.8);
        vec3 wireCol=mix(vec3(0.0,2.8,3.2),vec3(2.2,0.0,2.8),depth);
        col+=wireCol*wire*bright;
    }
    // Subtle receding grid on floor/ceiling for parallax feel
    float gy=abs(fract(uv.y*3.0+t*0.1)-0.5)*2.0;
    float gx=abs(fract(uv.x*3.0)-0.5)*2.0;
    float gridLine=smoothstep(fwidth(gx)*3.0,0.0,gx-0.9)*0.5+smoothstep(fwidth(gy)*3.0,0.0,gy-0.9)*0.5;
    col+=vec3(0.0,0.15,0.25)*gridLine*0.12*(1.0+ap*0.3);
    gl_FragColor=vec4(col,1.0);
}
