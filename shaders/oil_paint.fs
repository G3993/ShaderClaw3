/*{
  "DESCRIPTION": "Moonlit Japanese Lacquerware — Rimpa-school inspired radial brushstroke arcs: gold and vermilion on black lacquer, with a silver moon disc focal element",
  "CREDIT": "Easel auto-improve 2026-05-06",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "arcSpeed",  "LABEL": "Arc Speed",  "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 0.8 },
    { "NAME": "arcCount",  "LABEL": "Arc Layers", "TYPE": "float", "DEFAULT": 6.0,  "MIN": 2.0, "MAX": 12.0 },
    { "NAME": "hdrPeak",   "LABEL": "HDR Peak",   "TYPE": "float", "DEFAULT": 2.4,  "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "moonSize",  "LABEL": "Moon Size",  "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.05,"MAX": 0.5 },
    { "NAME": "audioReact","LABEL": "Audio React","TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define PI 3.14159265359
#define TAU 6.28318530718

float hash21(vec2 p) { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float hash11(float n) { return fract(sin(n*12.9898)*43758.5453); }

// Smooth FBM for lacquer texture variation
float noise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f*f*(3.0-2.0*f);
    return mix(mix(hash21(i),hash21(i+vec2(1,0)),f.x),
               mix(hash21(i+vec2(0,1)),hash21(i+vec2(1,1)),f.x),f.y);
}
float fbm(vec2 p) {
    float v=0.0,a=0.5;
    for(int i=0;i<4;i++){v+=noise(p)*a;p*=2.0;a*=0.5;}
    return v;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float audio = 1.0 + audioLevel*audioReact*0.3 + audioBass*audioReact*0.15;

    // Lacquer black background with subtle texture
    float bgNoise = fbm(uv*3.0 + TIME*0.02);
    vec3 col = vec3(0.018, 0.012, 0.008) + bgNoise*0.012;

    // Silver moon disc — focal element, top-right quadrant
    vec2 moonPos = vec2(aspect*0.35, 0.38);
    float moonDist = length(uv - moonPos) - moonSize*(1.0+audioBass*audioReact*0.04);
    float moonAA   = fwidth(moonDist);
    float moonMask = 1.0 - smoothstep(-moonAA, moonAA, moonDist);
    // Moon surface: cool silver with subtle craters
    float craterN = fbm(uv*18.0)*0.5 + fbm(uv*42.0)*0.25;
    vec3 moonCol  = vec3(0.72, 0.78, 0.85) * (0.75 + craterN*0.25);
    moonCol      += vec3(0.3, 0.35, 0.45) * pow(max(1.0-length(uv-moonPos)/moonSize,0.0),3.0); // limb brightening
    col = mix(col, moonCol * hdrPeak * 0.8 * audio, moonMask);

    // Moon halo glow
    float moonGlow = exp(-max(moonDist,0.0)*4.0);
    col += vec3(0.4, 0.45, 0.6) * moonGlow * 0.6 * hdrPeak * audio;

    // Rimpa brushstroke arcs — concentric radial arcs around the moon
    int N = int(clamp(arcCount, 2.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);

        // Each arc: a ring at radius r, spanning an arc from angle a0 to a1
        float r0  = 0.28 + fi * 0.12 + sin(TIME * arcSpeed * (0.5 + fi*0.11) + fi*1.7) * 0.04;
        float phaseOff = fi * (PI / float(N)) + TIME * arcSpeed * (0.1 + fi * 0.07);
        float arcSpan  = PI * 0.45 + sin(TIME * arcSpeed * 0.3 + fi) * 0.1;

        // Distance to the arc (ring segment in 2D)
        // Compute polar coords relative to moon
        vec2 d = uv - moonPos;
        float r = length(d);
        float ang = atan(d.y, d.x);

        // Signed distance to arc
        float rDist = abs(r - r0);
        // Angle distance (angular segment)
        float a0 = phaseOff;
        float a1 = phaseOff + arcSpan;
        // Wrap angle into arc range
        float angW = ang - a0;
        angW = mod(angW + TAU, TAU);
        float arcLen = a1 - a0;
        float angDist = min(angW, max(arcLen - angW, 0.0));
        float arcDist = max(rDist, max(-angW, angW - arcLen) * r0) - 0.006;

        // Stroke width modulated by FBM for brushstroke feel
        float bw   = 0.018 + fbm(uv*5.0 + fi)*0.010;
        float aa   = fwidth(arcDist);
        float mask = 1.0 - smoothstep(-aa, aa, arcDist - bw);

        // Alternate colors: gold, vermilion, crimson-gold
        vec3 strokeCol;
        int ci = int(mod(fi, 3.0));
        if (ci == 0) strokeCol = vec3(1.0, 0.80, 0.0);    // gold
        else if (ci == 1) strokeCol = vec3(0.95, 0.22, 0.05); // vermilion
        else strokeCol = vec3(1.0, 0.55, 0.0);             // orange-gold

        // Inner edge darkening (ink-like thick/thin)
        float edgeDark = smoothstep(bw*0.3, bw*0.8, abs(arcDist));
        strokeCol *= 0.4 + edgeDark * 0.6;

        col = mix(col, strokeCol * hdrPeak * audio, mask);

        // Glow halo
        float haloD = max(arcDist - bw, 0.0);
        col += strokeCol * exp(-haloD*60.0) * 0.5 * hdrPeak * audio;
    }

    // Fine gold dust scattered over the lacquer (kintsugi effect)
    for (int j = 0; j < 40; j++) {
        float fj = float(j);
        vec2 dustPos = vec2(hash11(fj*1.31)*2.0-1.0, hash11(fj*2.71)*2.0-1.0);
        dustPos.x *= aspect;
        float dustR = 0.003 + hash11(fj*3.13)*0.008;
        float dd = length(uv - dustPos) - dustR;
        float daa = fwidth(dd);
        float dmask = 1.0 - smoothstep(-daa, daa, dd);
        float pulse = 0.6 + 0.4*sin(TIME*arcSpeed*2.0 + fj*1.7);
        col += vec3(1.0, 0.88, 0.3) * dmask * pulse * hdrPeak * 0.7 * audio;
    }

    gl_FragColor = vec4(col, 1.0);
}
