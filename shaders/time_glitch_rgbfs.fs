/*{
  "DESCRIPTION": "Datamosh Monolith — raymarched SDF obsidian monolith with animated circuit trace glitch projection. Electric blue/violet/magenta palette",
  "CREDIT": "ShaderClaw auto-improve v3",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Glitch"],
  "INPUTS": [
    { "NAME": "monW",      "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.1, "MAX": 1.0, "LABEL": "Monolith Width" },
    { "NAME": "monH",      "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.5, "MAX": 3.5, "LABEL": "Monolith Height" },
    { "NAME": "glitchAmt", "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Glitch Amount" },
    { "NAME": "glowPeak",  "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0, "LABEL": "Glow Peak" },
    { "NAME": "audioMod",  "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" }
  ]
}*/

#define STEPS 64
#define MAXD  12.0
#define EPS   0.003

const vec3 C_BLUE    = vec3(0.0,  0.5,  1.0);
const vec3 C_VIOLET  = vec3(0.55, 0.0,  1.0);
const vec3 C_MAGENTA = vec3(1.0,  0.0,  0.8);

float hashG(float n)  { return fract(sin(n*127.1)*43758.5); }
float hashG2(vec2 p)  { return fract(sin(dot(p, vec2(127.1,311.7)))*43758.5); }

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

float circuitTrace(vec2 uv, float channel, float t) {
    float glitch = glitchAmt * (1.0 + audioLevel * audioMod * 0.3);
    // Glitch displacement rows
    float rowIdx = floor(uv.y * 16.0);
    float rowNoise = hashG2(vec2(rowIdx, floor(t * (2.0 + channel))));
    float disp = (rowNoise - 0.5) * glitch * 0.12 * step(0.7, rowNoise);
    vec2 u = uv + vec2(disp, 0.0);
    // Horizontal + vertical trace grid
    float trH = smoothstep(0.04, 0.0, abs(fract(u.y*10.0+0.5)-0.5)-0.46);
    float trV = smoothstep(0.04, 0.0, abs(fract(u.x*16.0+0.5)-0.5)-0.46);
    // Animated data pulse
    float pulse = step(0.95, sin((u.x*16.0 + channel*3.3) * 3.14159 - t * (6.0 + channel*2.0)));
    return max(trH, trV) + pulse * 0.5;
}

vec3 sdfMap(vec3 p) {
    // Monolith centered at origin
    float box = sdBox(p, vec3(monW, monH, monW*0.6));
    return vec3(box, 0.0, 0.0);
}

vec3 calcN(vec3 p) {
    const float e = 0.003;
    return normalize(vec3(
        sdfMap(p+vec3(e,0,0)).x-sdfMap(p-vec3(e,0,0)).x,
        sdfMap(p+vec3(0,e,0)).x-sdfMap(p-vec3(0,e,0)).x,
        sdfMap(p+vec3(0,0,e)).x-sdfMap(p-vec3(0,0,e)).x));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float amod = 1.0 + audioLevel * audioMod * 0.18;
    // Camera orbits the monolith slowly
    float camA = t * 0.15;
    vec3 ro = vec3(cos(camA)*3.5, 0.5 + sin(t*0.07)*0.3, sin(camA)*3.5);
    vec3 ta = vec3(0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 2.0*ww);
    float d = 0.0;
    bool hit = false;
    for (int i = 0; i < STEPS; i++) {
        float h = sdfMap(ro+rd*d).x;
        if (h < EPS) { hit = true; break; }
        if (d > MAXD) break;
        d += max(h, EPS);
    }
    vec3 col = vec3(0.0, 0.0, 0.01);
    if (hit) {
        vec3 p = ro + rd * d;
        vec3 n = calcN(p);
        // Project UV onto face
        vec2 faceUV;
        vec3 an = abs(n);
        if (an.z > an.x && an.z > an.y) faceUV = vec2(p.x/monW, p.y/monH)*0.5+0.5;
        else if (an.x > an.y)            faceUV = vec2(p.z/(monW*0.6), p.y/monH)*0.5+0.5;
        else                              faceUV = vec2(p.x/monW, p.z/(monW*0.6))*0.5+0.5;
        // Three independent circuit channels
        float cR = circuitTrace(faceUV, 0.0, t);
        float cG = circuitTrace(faceUV + vec2(0.5,0.3), 1.0, t);
        float cB = circuitTrace(faceUV + vec2(0.2,0.7), 2.0, t);
        vec3 circuitCol = C_BLUE*cR + C_VIOLET*cG + C_MAGENTA*cB;
        // Ink silhouette
        float nv  = max(dot(n,-rd),0.0);
        float ink = 1.0 - smoothstep(0.0, 0.2, nv);
        // fwidth AA on circuit traces
        float fw  = fwidth(cR + cG + cB);
        float aa  = smoothstep(0.0, fw*2.0, cR+cG+cB+0.01);
        col = mix(circuitCol * glowPeak * amod * aa, vec3(0.0), ink*0.9);
    }
    gl_FragColor = vec4(col, 1.0);
}
