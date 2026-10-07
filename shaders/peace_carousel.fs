/*{
  "CATEGORIES": [
    "3D",
    "Generator",
    "Audio Reactive"
  ],
  "DESCRIPTION": "Peace Carousel — a rotating ring of raymarched SDF peace-sign hands (index+middle V, folded ring/pinky, thumb across, sleeve cuff). Configurable count, ring radius, carousel spin, per-hand tumble, finger spread with animated wag, 6 materials (Skin/Chrome/Gold/Hologram/Toon/Candy), 4 backgrounds (Void/Gradient/Disco Rays/Peace Glyph), glow, orbit camera, full audio reactivity (bass=pulse, mid=spin phase, high=shimmer).",
  "INPUTS": [
    {
      "NAME": "handCount",
      "LABEL": "Hand Count",
      "TYPE": "float",
      "MIN": 1,
      "MAX": 12,
      "DEFAULT": 6
    },
    {
      "NAME": "ringRadius",
      "LABEL": "Ring Radius",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 5,
      "DEFAULT": 2.3
    },
    {
      "NAME": "spinSpeed",
      "LABEL": "Carousel Spin",
      "TYPE": "float",
      "MIN": -2,
      "MAX": 2,
      "DEFAULT": 0.35
    },
    {
      "NAME": "tumbleSpeed",
      "LABEL": "Hand Tumble",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 4,
      "DEFAULT": 0.7
    },
    {
      "NAME": "handScale",
      "LABEL": "Hand Scale",
      "TYPE": "float",
      "MIN": 0.3,
      "MAX": 2.2,
      "DEFAULT": 0.8
    },
    {
      "NAME": "fingerSpread",
      "LABEL": "Finger Spread",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.7,
      "DEFAULT": 0.32
    },
    {
      "NAME": "fingerWag",
      "LABEL": "Finger Wag",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.35
    },
    {
      "NAME": "bobAmount",
      "LABEL": "Bob",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.25
    },
    {
      "NAME": "material",
      "LABEL": "Material",
      "TYPE": "long",
      "VALUES": [0, 1, 2, 3, 4, 5],
      "LABELS": ["Skin", "Chrome", "Gold", "Hologram", "Toon", "Candy"],
      "DEFAULT": 1
    },
    {
      "NAME": "baseColor",
      "LABEL": "Base Color",
      "TYPE": "color",
      "DEFAULT": [0.87, 0.62, 0.47, 1.0]
    },
    {
      "NAME": "sleeveColor",
      "LABEL": "Sleeve Color",
      "TYPE": "color",
      "DEFAULT": [0.30, 0.12, 0.62, 1.0]
    },
    {
      "NAME": "accentColor",
      "LABEL": "Accent / Glow",
      "TYPE": "color",
      "DEFAULT": [1.0, 0.35, 0.9, 1.0]
    },
    {
      "NAME": "glowStrength",
      "LABEL": "Glow",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0.5
    },
    {
      "NAME": "bgStyle",
      "LABEL": "Background",
      "TYPE": "long",
      "VALUES": [0, 1, 2, 3],
      "LABELS": ["Void", "Gradient", "Disco Rays", "Peace Glyph"],
      "DEFAULT": 3
    },
    {
      "NAME": "bgColorA",
      "LABEL": "BG Color A",
      "TYPE": "color",
      "DEFAULT": [0.015, 0.01, 0.045, 1.0]
    },
    {
      "NAME": "bgColorB",
      "LABEL": "BG Color B",
      "TYPE": "color",
      "DEFAULT": [0.13, 0.03, 0.20, 1.0]
    },
    {
      "NAME": "audioReact",
      "LABEL": "Audio React",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "exposure",
      "LABEL": "Exposure",
      "TYPE": "float",
      "MIN": 0.3,
      "MAX": 3,
      "DEFAULT": 1
    },
    {
      "NAME": "camDist",
      "LABEL": "Camera Distance",
      "TYPE": "float",
      "MIN": 2,
      "MAX": 12,
      "DEFAULT": 6.8
    },
    {
      "NAME": "camHeight",
      "LABEL": "Camera Height",
      "TYPE": "float",
      "MIN": -3,
      "MAX": 4,
      "DEFAULT": 0.9
    },
    {
      "NAME": "camOrbitSpeed",
      "LABEL": "Camera Orbit",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0.12
    },
    {
      "NAME": "camAzimuth",
      "LABEL": "Camera Azimuth",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 6.2832,
      "DEFAULT": 0
    }
  ]
}*/

// Peace Carousel — rotating ring of raymarched ✌️ hands.
// Hand is a hand-built SDF: palm + wrist + sleeve cuff, index/middle
// extended in a V (animated wag), ring/pinky folded, thumb across.
// Polar domain repetition (2 nearest sectors) keeps cost flat vs count.

#define TAU 6.28318530718

mat2 rot2(float a){ float c=cos(a), s=sin(a); return mat2(c,-s,s,c); }

float smin(float a, float b, float k){
    float h=clamp(0.5+0.5*(b-a)/k,0.0,1.0);
    return mix(b,a,h)-k*h*(1.0-h);
}

float sdCapT(vec3 p, vec3 a, vec3 b, float r1, float r2){
    vec3 pa=p-a, ba=b-a;
    float h=clamp(dot(pa,ba)/dot(ba,ba),0.0,1.0);
    return length(pa-ba*h)-mix(r1,r2,h);
}

float sdRoundBox(vec3 p, vec3 b, float r){
    vec3 q=abs(p)-b;
    return length(max(q,0.0))+min(max(q.x,max(q.y,q.z)),0.0)-r;
}

// ---- globals set once in main, read by map() ----
float gCnt, gSeg, gRad, gSpin, gTumble, gBob, gSpread, gBass, gMid, gHigh;
float gHitId; // instance id of the last (closest) map eval — used for shading

// One ✌️ hand in local space: y up, palm facing +z, origin mid-palm.
// Returns (distance, materialId): 1 = hand, 2 = sleeve.
vec2 sdHand(vec3 p, float spread){
    p.x += 0.02; // recenter (V sits slightly right of palm center)

    // palm + wrist
    float d = sdRoundBox(p-vec3(0.0,-0.04,0.0), vec3(0.31,0.28,0.085), 0.17);
    d = smin(d, sdCapT(p, vec3(0.0,-0.85,0.0), vec3(0.0,-0.38,0.0), 0.22, 0.25), 0.12);

    // extended fingers: middle (x=+0.11) leans left, index (x=+0.32) leans right
    {   // middle finger
        float ang = -(spread*0.45+0.02);
        vec3 k = vec3(0.11, 0.44, 0.0);
        vec3 dir = normalize(vec3(sin(ang), cos(ang), -0.10));
        vec3 j = k + dir*0.42;
        vec3 tip = j + normalize(dir+vec3(0.0,0.0,0.14))*0.36;
        float f = sdCapT(p, k, j, 0.098, 0.090);
        f = smin(f, sdCapT(p, j, tip, 0.090, 0.076), 0.03);
        d = smin(d, f, 0.07);
    }
    {   // index finger
        float ang = (spread*0.55+0.05);
        vec3 k = vec3(0.32, 0.40, 0.0);
        vec3 dir = normalize(vec3(sin(ang), cos(ang), -0.10));
        vec3 j = k + dir*0.38;
        vec3 tip = j + normalize(dir+vec3(0.0,0.0,0.14))*0.33;
        float f = sdCapT(p, k, j, 0.094, 0.086);
        f = smin(f, sdCapT(p, j, tip, 0.086, 0.073), 0.03);
        d = smin(d, f, 0.07);
    }
    // folded fingers curl down over the palm front: ring (x=-0.11), pinky (x=-0.32)
    {
        vec3 k = vec3(-0.11, 0.38, 0.02);
        vec3 j1 = k+vec3(0.0, 0.06, 0.19);
        vec3 j2 = k+vec3(0.0,-0.17, 0.27);
        float f = sdCapT(p, k, j1, 0.095, 0.090);
        f = smin(f, sdCapT(p, j1, j2, 0.090, 0.078), 0.03);
        f = smin(f, length(p-(k+vec3(0.0,-0.26,0.25)))-0.075, 0.03);
        d = smin(d, f, 0.055);
    }
    {
        vec3 k = vec3(-0.31, 0.30, 0.02);
        vec3 j1 = k+vec3(-0.01, 0.05, 0.16);
        vec3 j2 = k+vec3(-0.01,-0.15, 0.24);
        float f = sdCapT(p, k, j1, 0.084, 0.079);
        f = smin(f, sdCapT(p, j1, j2, 0.079, 0.068), 0.03);
        f = smin(f, length(p-(k+vec3(-0.01,-0.22,0.22)))-0.065, 0.03);
        d = smin(d, f, 0.055);
    }
    {   // thumb folded low across the palm front, tip meeting the folded fingers
        vec3 tb = vec3(0.44,-0.32, 0.04);
        vec3 tm_ = vec3(0.30,-0.08, 0.20);
        vec3 tt = vec3(0.00,-0.02, 0.26);
        float th = sdCapT(p, tb, tm_, 0.13, 0.11);
        th = smin(th, sdCapT(p, tm_, tt, 0.11, 0.09), 0.04);
        d = smin(d, th, 0.06);
    }

    // sleeve cuff
    float cuff = sdCapT(p, vec3(0.0,-0.98,0.0), vec3(0.0,-0.68,0.0), 0.33, 0.35);
    if(cuff < d) return vec2(cuff, 2.0);
    return vec2(d, 1.0);
}

vec2 map(vec3 p){
    // carousel spin
    p.xz = rot2(gSpin) * p.xz;

    float ang = atan(p.x, p.z);
    float fi = ang/gSeg;
    float i0 = floor(fi+0.5);
    float i1 = i0 + sign(fi-i0); // second-nearest sector (hands can straddle edges)

    vec2 res = vec2(1e5, 0.0);
    for(int k=0; k<2; k++){
        float id = (k==0) ? i0 : i1;
        float a = id*gSeg;
        vec3 q = p;
        q.xz = rot2(-a) * q.xz;   // sector → local, hand sits on +z axis
        q.z -= gRad;
        float idw = mod(id, gCnt);
        q.y -= sin(TIME*1.4 + idw*2.1) * gBob;          // bob wave around the ring
        q.xz = rot2(gTumble + idw*2.39996) * q.xz;      // per-hand tumble (golden-angle phase)
        float s = max(0.05, handScale * (1.0 + 0.05*sin(TIME*1.7+idw*1.9) + 0.16*gBass));
        float spr = gSpread * (1.0 + fingerWag*0.55*sin(TIME*2.2 + idw*1.7)) + 0.10*gBass;
        vec2 h = sdHand(q/s, spr);
        h.x *= s;
        if(h.x < res.x){ res = h; gHitId = idw; }
    }
    return res;
}

vec3 calcNormal(vec3 p){
    const vec2 e = vec2(0.0025,-0.0025);
    return normalize( e.xyy*map(p+e.xyy).x + e.yyx*map(p+e.yyx).x
                    + e.yxy*map(p+e.yxy).x + e.xxx*map(p+e.xxx).x );
}

float calcAO(vec3 p, vec3 n){
    float occ=0.0, sca=1.0;
    for(int i=0;i<4;i++){
        float h=0.02+0.11*float(i);
        occ += (h - map(p+n*h).x)*sca;
        sca *= 0.72;
    }
    return clamp(1.0-1.8*occ, 0.0, 1.0);
}

// procedural environment for reflective materials
vec3 envColor(vec3 d){
    // dark ground / bright sky for real chrome contrast
    vec3 col = mix(bgColorA.rgb*0.6, bgColorB.rgb*2.2 + 0.35, smoothstep(-0.45,0.55,d.y));
    col += accentColor.rgb * pow(max(0.0, sin(d.y*3.0+1.2)), 6.0)*0.8;
    col += vec3(1.0) * pow(max(0.0, dot(d, normalize(vec3(0.5,0.8,0.3)))), 24.0)*2.2;
    return col;
}

vec3 palette(float t){
    return 0.5 + 0.5*cos(TAU*(t + vec3(0.0,0.33,0.67)));
}

float sdSeg2(vec2 p, vec2 a, vec2 b){
    vec2 pa=p-a, ba=b-a;
    float h=clamp(dot(pa,ba)/dot(ba,ba),0.0,1.0);
    return length(pa-ba*h);
}

// 2D peace-symbol SDF (ring + vertical bar + two 45° legs)
float peaceGlyph(vec2 u, float R){
    float w = R*0.09;
    float d = abs(length(u)-R) - w;
    d = min(d, sdSeg2(u, vec2(0.0,R), vec2(0.0,-R)) - w);
    vec2 leg = vec2(0.7071,-0.7071)*R;
    d = min(d, sdSeg2(u, vec2(0.0), leg) - w);
    d = min(d, sdSeg2(u, vec2(0.0), vec2(-leg.x,leg.y)) - w);
    return d;
}

vec3 background(vec3 rd, vec2 uv){
    int bs = int(bgStyle);
    float vig = 1.0 - 0.45*dot(uv,uv);
    if(bs == 0){
        return bgColorA.rgb * vig;
    }
    if(bs == 1){
        vec3 c = mix(bgColorA.rgb, bgColorB.rgb, smoothstep(-0.7,0.8,uv.y+rd.y*0.5));
        return c * vig;
    }
    if(bs == 2){
        float a = atan(uv.y, uv.x);
        float rays = pow(0.5+0.5*sin(a*9.0 + TIME*0.7), 3.0);
        vec3 c = mix(bgColorA.rgb, bgColorB.rgb, rays);
        c += accentColor.rgb * rays * (0.15 + 0.35*gBass);
        return c * vig;
    }
    // Peace Glyph
    vec2 u = uv;
    u = rot2(sin(TIME*0.11)*0.22) * u;
    float d = peaceGlyph(u, 0.38 + 0.025*gBass);
    vec3 c = mix(bgColorA.rgb, bgColorB.rgb, smoothstep(-0.7,0.8,uv.y));
    float glow = exp(-max(d,0.0)*9.0);
    float core = smoothstep(0.010, -0.010, d);
    c += accentColor.rgb * (glow*0.20*(1.0+gBass*0.8) + core*0.40);
    return c * vig;
}

void main(){
    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;

    // audio (clamped raw bands, scaled by audioReact)
    gBass = clamp(audioBass,0.0,1.0)*audioReact;
    gMid  = clamp(audioMid ,0.0,1.0)*audioReact;
    gHigh = clamp(audioHigh,0.0,1.0)*audioReact;

    gCnt = floor(handCount+0.5);
    gSeg = TAU/gCnt;
    gRad = ringRadius * (1.0 + 0.05*gBass);
    // constant rate + bounded audio phase offset (never rate*audio — camera teleports)
    gSpin   = TIME*spinSpeed + 0.35*gMid;
    gTumble = TIME*tumbleSpeed + 0.30*gMid;
    gBob = bobAmount*(0.6+0.8*gBass);
    gSpread = fingerSpread;
    gHitId = 0.0;

    // orbit camera
    float orb = camAzimuth + TIME*camOrbitSpeed;
    vec3 ro = vec3(sin(orb)*camDist, camHeight, cos(orb)*camDist);
    vec3 ta = vec3(0.0, 0.10, 0.0);
    vec3 ww = normalize(ta-ro);
    vec3 uu = normalize(cross(ww, vec3(0.0,1.0,0.0)));
    vec3 vv = cross(uu,ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 1.5*ww);

    // march
    float t = 0.0;
    float matId = -1.0;
    float glowAcc = 0.0;
    for(int i=0;i<90;i++){
        vec3 pos = ro + rd*t;
        vec2 h = map(pos);
        glowAcc += exp(-abs(h.x)*6.0);
        if(h.x < 0.0015*t + 0.0008){ matId = h.y; break; }
        t += h.x*0.8;   // relaxed step: per-instance rotation bends the field slightly
        if(t > 28.0) break;
    }

    vec3 col = background(rd, uv);
    vec3 bgCol = col;

    if(matId > 0.0){
        vec3 pos = ro + rd*t;
        vec3 n = calcNormal(pos);
        float occ = calcAO(pos, n);
        float idw = gHitId;

        vec3 L  = normalize(vec3(0.55, 0.75, 0.35));
        vec3 Lf = normalize(vec3(-0.6, 0.15,-0.4));
        float ndl = dot(n,L);
        float fres = pow(clamp(1.0+dot(n,rd),0.0,1.0), 3.0);
        vec3 base = (matId > 1.5) ? sleeveColor.rgb : baseColor.rgb;
        int m = int(material);

        if(m == 0){ // skin — wrap diffuse + faint sss rim
            float dif = clamp((ndl+0.45)/1.45, 0.0, 1.0);
            float fil = clamp(dot(n,Lf)*0.5+0.5, 0.0, 1.0)*0.25;
            col = base*(dif*1.15 + fil + 0.10)*occ;
            col += vec3(0.9,0.25,0.15)*fres*0.25*occ; // blood-warm rim
            vec3 h2 = normalize(L-rd);
            col += vec3(1.0)*pow(max(0.0,dot(n,h2)),24.0)*0.18;
        } else if(m == 1 || m == 2){ // chrome / gold
            vec3 tint = (m==2) ? vec3(1.0,0.72,0.30) : vec3(0.92,0.95,1.0);
            if(matId > 1.5) tint *= sleeveColor.rgb*1.6 + 0.2;
            vec3 R = reflect(rd,n);
            R = normalize(R + vec3(sin(TIME*6.3),cos(TIME*5.1),sin(TIME*4.7))*0.01*gHigh);
            col = envColor(R)*tint*(0.55+0.45*fres)*occ;
            vec3 h2 = normalize(L-rd);
            col += tint*pow(max(0.0,dot(n,h2)),60.0)*2.0;
        } else if(m == 3){ // hologram
            float scan = 0.75 + 0.25*sin(gl_FragCoord.y*2.4 + TIME*30.0);
            float flick = 0.85 + 0.15*sin(TIME*47.0 + idw*9.0) + 0.5*gHigh;
            col = bgCol*0.35;
            col += accentColor.rgb * (0.22 + 1.5*fres) * scan * flick * occ;
            col += base * 0.12 * scan;
        } else if(m == 4){ // toon
            float dif = clamp(ndl,0.0,1.0);
            float band = floor(dif*3.0+0.5)/3.0;
            col = base*(0.25+0.85*band)*occ;
            float edge = smoothstep(0.32, 0.18, dot(n,-rd));
            col = mix(col, vec3(0.01), edge);
            col += accentColor.rgb*step(0.985, pow(max(0.0,dot(normalize(L-rd),n)),8.0))*0.8;
        } else { // candy — per-hand cycling palette, glossy
            vec3 cc = palette(idw/max(gCnt,1.0) + pos.y*0.08 + TIME*0.04);
            if(matId > 1.5) cc = sleeveColor.rgb;
            float dif = clamp((ndl+0.35)/1.35,0.0,1.0);
            col = cc*(dif*1.1+0.12)*occ;
            col += envColor(reflect(rd,n))*fres*0.5*occ;
            vec3 h2 = normalize(L-rd);
            col += vec3(1.0)*pow(max(0.0,dot(n,h2)),48.0)*1.2;
        }

        // accent rim shimmer on treble (all materials)
        col += accentColor.rgb * fres * (0.10 + 0.45*gHigh) * occ;

        // depth fade into background
        col = mix(col, bgCol, smoothstep(9.0, 24.0, t)*0.6);
    }

    // near-miss glow
    col += accentColor.rgb * glowAcc * 0.010 * glowStrength * (0.7 + 0.6*gBass);

    // tonemap
    col *= exposure;
    col = col/(1.0+col)*1.25;
    col = pow(max(col,0.0), vec3(0.4545));

    gl_FragColor = vec4(col, 1.0);
}
