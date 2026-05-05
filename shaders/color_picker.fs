/*{
  "DESCRIPTION": "Gyroid Neon — raymarched gyroid minimal surface. Cinematic violet/cyan/magenta/gold palette with ink-black silhouettes",
  "CREDIT": "ShaderClaw auto-improve v3",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "gScale",    "TYPE": "float", "DEFAULT": 2.2,  "MIN": 0.5, "MAX": 5.0, "LABEL": "Gyroid Scale" },
    { "NAME": "thickness", "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.02,"MAX": 0.3, "LABEL": "Wall Thickness" },
    { "NAME": "rotSpeed",  "TYPE": "float", "DEFAULT": 0.2,  "MIN": 0.0, "MAX": 1.5, "LABEL": "Rotate Speed" },
    { "NAME": "glowPeak",  "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0, "LABEL": "Glow Peak" },
    { "NAME": "audioMod",  "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" }
  ]
}*/

#define STEPS 72
#define MAXD  10.0
#define EPS   0.003

const vec3 C_VIOLET  = vec3(0.55, 0.0,  1.0);
const vec3 C_CYAN    = vec3(0.0,  0.9,  1.0);
const vec3 C_MAGENTA = vec3(1.0,  0.0,  0.75);
const vec3 C_GOLD    = vec3(1.0,  0.78, 0.0);

float gyroid(vec3 p) {
    return cos(p.x)*sin(p.y) + cos(p.y)*sin(p.z) + cos(p.z)*sin(p.x);
}
float sdf(vec3 p) {
    float s = gScale * (1.0 + audioLevel * audioMod * 0.12);
    vec3 q = p * s + vec3(TIME*0.29, TIME*0.19, TIME*0.23);
    return (abs(gyroid(q)) - thickness * s * 1.732) / (s * 1.732);
}
vec3 calcN(vec3 p) {
    const float e = 0.002;
    return normalize(vec3(
        sdf(p+vec3(e,0,0))-sdf(p-vec3(e,0,0)),
        sdf(p+vec3(0,e,0))-sdf(p-vec3(0,e,0)),
        sdf(p+vec3(0,0,e))-sdf(p-vec3(0,0,e))));
}
vec3 pal(float t) {
    t = fract(t);
    if (t < 0.25) return mix(C_VIOLET,  C_CYAN,    t*4.0);
    if (t < 0.50) return mix(C_CYAN,    C_MAGENTA, (t-0.25)*4.0);
    if (t < 0.75) return mix(C_MAGENTA, C_GOLD,    (t-0.50)*4.0);
    return             mix(C_GOLD,    C_VIOLET,  (t-0.75)*4.0);
}
void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float r = 3.6;
    vec3 ro = vec3(sin(t*rotSpeed)*r, cos(t*rotSpeed*0.61)*1.3, cos(t*rotSpeed)*r);
    vec3 ww = normalize(-ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 2.0*ww);
    float d = 0.0;
    bool hit = false;
    for (int i = 0; i < STEPS; i++) {
        float h = sdf(ro + rd * d);
        if (h < EPS) { hit = true; break; }
        if (d > MAXD) break;
        d += max(h * 0.55, EPS);
    }
    vec3 col = vec3(0.0, 0.0, 0.01);
    if (hit) {
        vec3 p = ro + rd * d;
        vec3 n = calcN(p);
        float hue = fract(dot(p*0.35, vec3(0.31,0.47,0.23)) + t*0.08);
        vec3 bc = pal(hue);
        vec3 kL = normalize(vec3(1.8, 2.2, -0.8));
        vec3 fL = normalize(vec3(-1.0, 0.5, 1.0));
        float diff = max(dot(n, kL), 0.0);
        float fill = max(dot(n, fL), 0.0) * 0.3;
        float spec = pow(max(dot(reflect(-kL,n),-rd),0.0), 20.0);
        float nv   = max(dot(n,-rd), 0.0);
        float ink  = 1.0 - smoothstep(0.0, 0.18, nv);
        float fw   = fwidth(sdf(p));
        float aa   = 1.0 - smoothstep(0.0, fw*3.0, abs(sdf(p)));
        col = mix(
            bc*(diff*0.8+fill)*glowPeak + C_GOLD*pow(1.0-nv,3.0)*glowPeak*0.4 + vec3(1.0)*spec*0.35*glowPeak,
            vec3(0.0),
            ink * 0.92
        ) * aa;
    }
    gl_FragColor = vec4(col, 1.0);
}
