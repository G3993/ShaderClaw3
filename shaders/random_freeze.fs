/*{
  "DESCRIPTION": "Desert Dune Heatwave — raymarched sand dunes with heat shimmer. Warm ochre/gold/sienna/white-hot HDR palette",
  "CREDIT": "ShaderClaw auto-improve v3",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "duneScale",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3, "MAX": 3.0, "LABEL": "Dune Scale" },
    { "NAME": "heatShimmer", "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.5, "LABEL": "Heat Shimmer" },
    { "NAME": "sunHeight",   "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.1, "MAX": 1.0, "LABEL": "Sun Height" },
    { "NAME": "glowPeak",    "TYPE": "float", "DEFAULT": 2.3,  "MIN": 1.0, "MAX": 4.0, "LABEL": "Glow Peak" },
    { "NAME": "audioMod",    "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" }
  ]
}*/

#define STEPS 64
#define MAXD  20.0
#define EPS   0.005

const vec3 C_SHADOW = vec3(0.12, 0.06, 0.01);
const vec3 C_OCHRE  = vec3(0.82, 0.52, 0.08);
const vec3 C_GOLD   = vec3(1.0,  0.78, 0.10);
const vec3 C_HOT    = vec3(2.5,  1.8,  0.5);   // HDR white-hot

float duneH(vec2 p) {
    float s = duneScale * 0.5;
    return sin(p.x*1.1*s + TIME*0.05)*0.18
         + sin(p.x*0.7*s - p.y*0.4*s + TIME*0.03)*0.12
         + sin(p.x*2.1*s + p.y*1.4*s)*0.07
         + sin(p.x*0.4*s + p.y*2.3*s + TIME*0.02)*0.05;
}
float sdf(vec3 p) { return p.y - duneH(p.xz); }
vec3 calcN(vec3 p) {
    const float e = 0.05;
    return normalize(vec3(
        sdf(p+vec3(e,0,0))-sdf(p-vec3(e,0,0)),
        sdf(p+vec3(0,e,0))-sdf(p-vec3(0,e,0)),
        sdf(p+vec3(0,0,e))-sdf(p-vec3(0,0,e))));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float amod = 1.0 + audioLevel * audioMod * 0.2;
    vec3 ro = vec3(t*0.28, 0.6, t*0.18);
    vec3 ta = ro + vec3(0.0, -0.25, -1.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    // Heat shimmer: perturb ray direction
    float shimA = heatShimmer * amod * 0.012;
    float shimX = sin(uv.x*8.3 + t*5.1) * shimA;
    float shimY = sin(uv.y*6.7 + t*4.3) * shimA * 0.6;
    vec3 rd = normalize((uv.x+shimX)*uu + (uv.y+shimY)*vv + 2.2*ww);
    float d = 0.0;
    bool hit = false;
    for (int i = 0; i < STEPS; i++) {
        float h = sdf(ro + rd*d);
        if (h < EPS) { hit = true; break; }
        if (d > MAXD) break;
        d += max(h*0.75, EPS);
    }
    // Sky: hot desert gold gradient (NO blue — zero cool tones)
    float skyT = clamp(uv.y*0.5+0.6, 0.0, 1.0);
    vec3 col = mix(vec3(1.2,0.8,0.3)*glowPeak*0.5, vec3(2.0,1.4,0.4)*glowPeak*0.5, skyT);
    if (hit) {
        vec3 p = ro + rd * d;
        vec3 n = calcN(p);
        vec3 kL = normalize(vec3(-0.4, sunHeight, -0.3));
        float diff = max(dot(n, kL), 0.0);
        float slope = 1.0 - n.y;
        vec3 bc = mix(C_OCHRE, C_SHADOW, slope);
        bc = mix(bc, C_GOLD, smoothstep(0.5, 1.0, diff));
        float crest = pow(diff, 7.0);
        float nv = max(dot(n,-rd),0.0);
        float ink = 1.0 - smoothstep(0.0, 0.15, nv);
        col = mix(bc*diff*glowPeak + C_HOT*crest*0.8, C_SHADOW*0.3, ink*0.65);
    }
    gl_FragColor = vec4(col, 1.0);
}
