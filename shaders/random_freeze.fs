/*{
  "DESCRIPTION": "Plasma Nova Freeze — a nova explosion frozen at peak: N plasma spheres radiating from a dense core, hot crimson-orange-gold-white palette",
  "CREDIT": "Easel auto-improve 2026-05-06",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "novaCount", "LABEL": "Plasma Balls", "TYPE": "float", "DEFAULT": 12.0, "MIN": 4.0, "MAX": 24.0 },
    { "NAME": "burstRadius","LABEL": "Burst Radius","TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.3, "MAX": 2.5 },
    { "NAME": "hdrPeak",   "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "camSpeed",  "LABEL": "Cam Speed",   "TYPE": "float", "DEFAULT": 0.10, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "audioReact","LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.9,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define MAX_DIST  12.0
#define SURF_DIST 0.003

float hash11(float n) { return fract(sin(n*12.9898)*43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// FBM noise for plasma surface turbulence
float noise3(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    f = f*f*(3.0-2.0*f);
    float a = hash21(i.xy+vec2(0,0));
    float b = hash21(i.xy+vec2(1,0));
    float c = hash21(i.xy+vec2(0,1));
    float d = hash21(i.xy+vec2(1,1));
    float ab = mix(a,b,f.x);
    float cd = mix(c,d,f.x);
    return mix(ab, cd, f.y) + hash11(i.z)*0.1;
}

// Plasma sphere SDF: displaced by noise for organic plasma surface
float sdPlasmaSphere(vec3 p, vec3 center, float r) {
    vec3 q = p - center;
    float base = length(q) - r;
    // Plasma turbulence displacement
    float disp = noise3(q * 3.5 + TIME * 1.3) * 0.12
               + noise3(q * 7.0 - TIME * 0.9) * 0.06;
    return base - disp * r;
}

float map(vec3 p) {
    float d = MAX_DIST;
    int N = int(clamp(novaCount, 4.0, 24.0));

    // Central hyperdense core
    float coreR = 0.18 * (1.0 + sin(TIME*3.7)*0.1);
    d = min(d, length(p) - coreR);

    // Radiating plasma balls
    for (int i = 0; i < 24; i++) {
        if (i >= N) break;
        float fi = float(i);
        // Fibonacci sphere distribution for plasma ball positions
        float golden = 2.399963; // golden angle in radians
        float phi = acos(1.0 - 2.0*(fi+0.5)/float(N));
        float theta = golden * fi + TIME * camSpeed * (0.5 + hash11(fi)*0.5);
        vec3 pos = vec3(sin(phi)*cos(theta), cos(phi), sin(phi)*sin(theta));
        pos *= burstRadius * (0.7 + hash11(fi*7.31)*0.3);
        // Add radial drift outward (frozen burst motion)
        pos *= 1.0 + hash11(fi*3.17)*0.15 + sin(TIME*camSpeed*0.7 + fi*1.3)*0.05;

        float r = 0.12 + hash11(fi*2.13)*0.10;
        d = min(d, sdPlasmaSphere(p, pos, r));
    }

    // Ejection filaments: thin capsule tubes connecting core to balls
    for (int i = 0; i < 8; i++) {
        float fi = float(i);
        float theta2 = fi * 0.785398 + TIME * camSpeed * 0.3;
        float phi2   = fi * 0.523599;
        vec3 dir = vec3(sin(phi2)*cos(theta2), cos(phi2), sin(phi2)*sin(theta2));
        vec3 tip = dir * (burstRadius * 0.9);
        // Capsule from core to tip
        vec3 ab = tip; // from origin (core) to tip
        vec3 ap = p;
        float t = clamp(dot(ap,ab)/dot(ab,ab), 0.0, 1.0);
        d = min(d, length(ap - ab*t) - 0.025);
    }

    return d;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p+e.xyy)-map(p-e.xyy),
        map(p+e.yxy)-map(p-e.yxy),
        map(p+e.yyx)-map(p-e.yyx)));
}

// Hot spectrum: 0=deep crimson, 0.33=orange, 0.67=gold, 1.0=white-hot
vec3 hotSpectrum(float t) {
    vec3 c0 = vec3(0.7, 0.0, 0.0);   // deep crimson
    vec3 c1 = vec3(1.0, 0.28, 0.0);  // orange
    vec3 c2 = vec3(1.0, 0.82, 0.0);  // gold
    vec3 c3 = vec3(1.2, 1.1, 1.0);   // white-hot (HDR)
    float t3 = t * 3.0;
    if (t3 < 1.0) return mix(c0, c1, t3);
    if (t3 < 2.0) return mix(c1, c2, t3-1.0);
    return mix(c2, c3, t3-2.0);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float audio = 1.0 + audioLevel*audioReact*0.35 + audioBass*audioReact*0.2;

    // Camera slow orbit
    float camT = TIME * camSpeed;
    vec3 ro = vec3(sin(camT)*4.5, sin(camT*0.41)*0.7, cos(camT)*4.5);
    vec3 ta  = vec3(0.0);
    vec3 ww  = normalize(ta - ro);
    vec3 uu  = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv  = cross(uu, ww);
    vec3 rd  = normalize(uv.x*uu + uv.y*vv + 1.8*ww);

    float dist = 0.0;
    float hitT = MAX_DIST;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd*dist;
        float d = map(p);
        if (d < SURF_DIST) { hitT = dist; break; }
        dist += d * 0.7;
        if (dist > MAX_DIST) break;
    }

    // Deep space background — void with scattered ember sparks
    vec3 col = vec3(0.005, 0.003, 0.002);
    float spark = step(0.9975, hash21(floor(rd.xy*120.0)));
    col += vec3(1.0,0.5,0.1)*spark*0.8;

    if (hitT < MAX_DIST) {
        vec3 p = ro + rd*hitT;
        vec3 n = calcNormal(p);

        // Hot spectrum based on distance from core (closer = hotter = white)
        float coreDist = length(p);
        float hotT = 1.0 - clamp(coreDist / (burstRadius * 1.1), 0.0, 1.0);
        vec3 baseCol = hotSpectrum(hotT);

        vec3 lDir = normalize(vec3(0.5, 1.0, 0.3));
        float diff = 0.3 + 0.7*max(dot(n,lDir), 0.0);
        float spec = pow(max(dot(reflect(-lDir,n),-rd),0.0), 24.0);
        float rim  = pow(1.0 - max(dot(n,-rd),0.0), 3.0);

        col = baseCol * diff * hdrPeak * audio;
        col += hotSpectrum(1.0) * spec * hdrPeak * 1.2 * audio;
        col += hotSpectrum(hotT*0.5+0.5) * rim * hdrPeak * 0.6 * audio;

        // AA on surface edge
        float edgeD = map(p);
        float edgeAA = fwidth(edgeD);
        col *= 0.85 + 0.15 * smoothstep(0.0, edgeAA, edgeD);
    }

    // Volumetric plasma glow (emission)
    float vt = 0.0;
    for (int i = 0; i < 40; i++) {
        vec3 vp = ro + rd*vt;
        float d = map(vp);
        float coreDist = length(vp);
        float hotT = 1.0 - clamp(coreDist/(burstRadius*1.1), 0.0, 1.0);
        col += hotSpectrum(hotT) * exp(-max(d,0.0)*20.0) * 0.06 * hdrPeak * audio;
        vt += max(d*0.45, 0.08);
        if (vt > MAX_DIST) break;
    }

    gl_FragColor = vec4(col, 1.0);
}
