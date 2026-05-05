/*{
  "DESCRIPTION": "Baroque Still Life — raymarched SDF still life with Caravaggio chiaroscuro lighting. Amber/crimson/gold/sienna palette",
  "CREDIT": "ShaderClaw auto-improve v3",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "lightX",   "TYPE": "float", "DEFAULT": 1.4,  "MIN": -2.0,"MAX": 2.0, "LABEL": "Light X" },
    { "NAME": "camSpeed", "TYPE": "float", "DEFAULT": 0.07, "MIN": 0.0, "MAX": 0.5, "LABEL": "Camera Speed" },
    { "NAME": "glowPeak", "TYPE": "float", "DEFAULT": 2.2,  "MIN": 1.0, "MAX": 4.0, "LABEL": "Glow Peak" },
    { "NAME": "audioMod", "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" }
  ]
}*/

#define STEPS 64
#define MAXD  12.0
#define EPS   0.004

const vec3 C_AMBER   = vec3(1.0,  0.55, 0.0);
const vec3 C_CRIMSON = vec3(0.85, 0.04, 0.04);
const vec3 C_GOLD    = vec3(1.0,  0.75, 0.1);
const vec3 C_SIENNA  = vec3(0.42, 0.17, 0.04);

float sdSphere(vec3 p, float r) { return length(p) - r; }
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}
float sdTorus(vec3 p, float R, float r) {
    return length(vec2(length(p.xz)-R, p.y)) - r;
}
float opSm(float a, float b, float k) {
    float h = max(k-abs(a-b),0.0)/k;
    return min(a,b) - h*h*k*0.25;
}

vec2 scene(vec3 p) {
    float tableY = -0.6;
    float table = sdBox(p - vec3(0.0,tableY,0.0), vec3(3.0,0.05,2.0));
    vec2 res = vec2(table, 3.0);
    // Amber apple
    float a1 = opSm(sdSphere(p-vec3(-0.42,tableY+0.28, 0.1),0.27),
                    sdSphere(p-vec3(-0.37,tableY+0.38,0.06),0.11),0.1);
    if (a1 < res.x) res = vec2(a1, 0.0);
    // Crimson apple
    float a2 = opSm(sdSphere(p-vec3(0.32,tableY+0.26,-0.1),0.25),
                    sdSphere(p-vec3(0.29,tableY+0.35,-0.07),0.10),0.1);
    if (a2 < res.x) res = vec2(a2, 1.0);
    // Gold goblet
    float g1 = sdTorus(p-vec3(0.0,tableY+0.08,0.42),0.12,0.04);
    float g2 = sdSphere(p-vec3(0.0,tableY+0.46,0.42),0.14);
    float g3 = sdBox(p-vec3(0.0,tableY+0.19,0.42),vec3(0.03,0.13,0.03));
    float goblet = min(min(g1,g2),g3);
    if (goblet < res.x) res = vec2(goblet, 2.0);
    return res;
}

float softShadow(vec3 ro, vec3 rd, float mint, float maxt) {
    float res = 1.0, t = mint;
    for (int i = 0; i < 16; i++) {
        float h = scene(ro+rd*t).x;
        if (h < 0.001) return 0.0;
        res = min(res, 6.0*h/t);
        t += clamp(h,0.01,0.25);
        if (t>maxt) break;
    }
    return clamp(res,0.0,1.0);
}

vec3 calcN(vec3 p) {
    const float e = 0.002;
    return normalize(vec3(
        scene(p+vec3(e,0,0)).x-scene(p-vec3(e,0,0)).x,
        scene(p+vec3(0,e,0)).x-scene(p-vec3(0,e,0)).x,
        scene(p+vec3(0,0,e)).x-scene(p-vec3(0,0,e)).x));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float amod = 1.0 + audioLevel * audioMod * 0.25;
    float tableY = -0.6;
    float camA = t * camSpeed * amod;
    vec3 ro = vec3(cos(camA)*2.2, tableY+1.15, sin(camA)*2.2);
    vec3 ta = vec3(0.0, tableY+0.25, 0.35);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 1.8*ww);
    float d = 0.0;
    float mat = -1.0;
    for (int i = 0; i < STEPS; i++) {
        vec2 res = scene(ro+rd*d);
        if (res.x < EPS) { mat = res.y; break; }
        if (d > MAXD) break;
        d += res.x;
    }
    // Caravaggio void: very deep warm black
    vec3 col = vec3(0.02, 0.01, 0.004);
    if (mat >= 0.0) {
        vec3 p = ro + rd * d;
        vec3 n = calcN(p);
        vec3 bc;
        if      (mat < 0.5) bc = C_AMBER;
        else if (mat < 1.5) bc = C_CRIMSON;
        else if (mat < 2.5) bc = C_GOLD;
        else                bc = C_SIENNA;
        vec3 kL = normalize(vec3(lightX, 2.0, -0.8));
        float diff = max(dot(n, kL), 0.0);
        float shad = softShadow(p+n*0.01, kL, 0.02, 4.0);
        float spec = pow(max(dot(reflect(-kL,n),-rd),0.0),30.0) * (mat<2.5?0.6:0.08);
        float nv   = max(dot(n,-rd),0.0);
        float ink  = 1.0 - smoothstep(0.0, 0.2, nv);
        col = mix(
            bc*(diff*shad*0.9+0.05)*glowPeak + vec3(1.0,0.9,0.6)*spec*glowPeak*0.3,
            vec3(0.0),
            ink * 0.9
        );
    }
    gl_FragColor = vec4(col, 1.0);
}
