/*{
  "DESCRIPTION": "Torus Knot Neon — raymarched (2,3) torus knot sculpture. Electric cyan/magenta/gold/violet on void black, ink silhouettes",
  "CREDIT": "ShaderClaw auto-improve v3",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "knotR",    "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.3, "MAX": 1.5, "LABEL": "Knot Radius" },
    { "NAME": "tubeR",    "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.04,"MAX": 0.4, "LABEL": "Tube Radius" },
    { "NAME": "rotSpeed", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.5, "LABEL": "Rotate Speed" },
    { "NAME": "glowPeak", "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0, "LABEL": "Glow Peak" },
    { "NAME": "audioMod", "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" }
  ]
}*/

#define STEPS 64
#define MAXD   8.0
#define EPS    0.004

const vec3 C_CYAN    = vec3(0.0,  1.0,  1.0);
const vec3 C_MAGENTA = vec3(1.0,  0.0,  0.9);
const vec3 C_GOLD    = vec3(1.0,  0.8,  0.0);
const vec3 C_VIOLET  = vec3(0.5,  0.0,  1.0);

float sdKnot(vec3 p, float R, float r, float tr) {
    float md = 1e5;
    const int N = 32;
    for (int i = 0; i < N; i++) {
        float t0 = float(i)   / float(N) * 6.28318;
        float t1 = float(i+1) / float(N) * 6.28318;
        vec3 k0 = vec3((R+r*cos(3.0*t0))*cos(2.0*t0), r*sin(3.0*t0), (R+r*cos(3.0*t0))*sin(2.0*t0));
        vec3 k1 = vec3((R+r*cos(3.0*t1))*cos(2.0*t1), r*sin(3.0*t1), (R+r*cos(3.0*t1))*sin(2.0*t1));
        vec3 pa = p-k0, ba = k1-k0;
        float h = clamp(dot(pa,ba)/max(dot(ba,ba),1e-6), 0.0, 1.0);
        md = min(md, length(pa-ba*h));
    }
    return md - tr;
}
float sdf(vec3 p) {
    return sdKnot(p, knotR, knotR*0.38, tubeR*(1.0+audioLevel*audioMod*0.15));
}
vec3 calcN(vec3 p) {
    const float e = 0.003;
    return normalize(vec3(
        sdf(p+vec3(e,0,0))-sdf(p-vec3(e,0,0)),
        sdf(p+vec3(0,e,0))-sdf(p-vec3(0,e,0)),
        sdf(p+vec3(0,0,e))-sdf(p-vec3(0,0,e))));
}
vec3 pal(float t) {
    t = fract(t);
    if (t < 0.33) return mix(C_CYAN,    C_MAGENTA, t*3.0);
    if (t < 0.66) return mix(C_MAGENTA, C_GOLD,    (t-0.33)*3.0);
    return             mix(C_GOLD,    C_VIOLET,  (t-0.66)*3.0);
}
void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float camT = t * rotSpeed;
    vec3 ro = vec3(cos(camT)*3.2, sin(camT*0.5)*1.0, sin(camT)*3.2);
    vec3 ta = vec3(0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 2.0*ww);
    float d = 0.0;
    bool hit = false;
    for (int i = 0; i < STEPS; i++) {
        float h = sdf(ro + rd * d);
        if (h < EPS) { hit = true; break; }
        if (d > MAXD) break;
        d += max(h, EPS);
    }
    vec3 col = vec3(0.0, 0.0, 0.008);
    if (hit) {
        vec3 p = ro + rd * d;
        vec3 n = calcN(p);
        float hue = fract(atan(p.z, p.x) / 6.28318 + t * 0.06);
        vec3 bc = pal(hue);
        vec3 kL = normalize(vec3(2.0, 2.5, -1.0));
        float diff = max(dot(n, kL), 0.0);
        float spec = pow(max(dot(reflect(-kL,n),-rd),0.0), 24.0);
        float nv   = max(dot(n,-rd), 0.0);
        float ink  = 1.0 - smoothstep(0.0, 0.2, nv);
        col = mix(
            bc*(diff*0.7+0.2)*glowPeak + C_GOLD*pow(1.0-nv,2.5)*glowPeak*0.5 + vec3(1.0)*spec*0.4*glowPeak,
            vec3(0.0),
            ink * 0.92
        );
    }
    gl_FragColor = vec4(col, 1.0);
}
