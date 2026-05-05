/*{
  "DESCRIPTION": "Neon River Delta — 3D aerial view of bioluminescent glowing channels on a void landscape. Electric blue/cyan/violet/gold palette",
  "CREDIT": "ShaderClaw auto-improve v3",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "channelW",  "TYPE": "float", "DEFAULT": 0.42, "MIN": 0.1, "MAX": 0.9, "LABEL": "Channel Width" },
    { "NAME": "flowSpeed", "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Flow Speed" },
    { "NAME": "altitude",  "TYPE": "float", "DEFAULT": 3.5,  "MIN": 1.0, "MAX": 7.0, "LABEL": "Altitude" },
    { "NAME": "glowPeak",  "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0, "LABEL": "Glow Peak" },
    { "NAME": "audioMod",  "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" }
  ]
}*/

const vec3 C_BLUE   = vec3(0.0,  0.3,  1.0);
const vec3 C_CYAN   = vec3(0.0,  0.9,  1.0);
const vec3 C_VIOLET = vec3(0.5,  0.0,  1.0);
const vec3 C_GOLD   = vec3(1.0,  0.8,  0.0);
const vec3 C_LAND   = vec3(0.01, 0.01, 0.02);

float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1,311.7)))*43758.5); }
float noiseS(vec2 p) {
    vec2 i=floor(p), f=fract(p);
    f=f*f*(3.0-2.0*f);
    return mix(mix(hash21(i),hash21(i+vec2(1,0)),f.x),
               mix(hash21(i+vec2(0,1)),hash21(i+vec2(1,1)),f.x),f.y);
}

float channelGlow(vec2 xz) {
    vec2 p = xz*0.75 + vec2(TIME*flowSpeed*0.1, TIME*flowSpeed*0.07);
    float n1 = noiseS(p);
    float n2 = noiseS(p*2.1+vec2(1.3,0.7));
    float n3 = noiseS(p*4.3-vec2(0.5,1.1));
    float combined = n1*0.5 + n2*0.3 + n3*0.2;
    return 1.0 - smoothstep(channelW-0.1, channelW+0.1, combined);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float amod = 1.0 + audioLevel * audioMod * 0.2;
    float driftX = sin(t*0.07)*0.6 + t*flowSpeed*0.09;
    float driftZ = cos(t*0.05)*0.4 + t*flowSpeed*0.05;
    vec3 ro = vec3(driftX, altitude, driftZ);
    vec3 ta = ro + vec3(0.0, -altitude, -0.15);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 2.5*ww);
    vec3 col = vec3(0.0, 0.0, 0.005);
    if (rd.y < 0.0) {
        float gt = -ro.y / rd.y;
        if (gt > 0.0) {
            vec3 p = ro + rd * gt;
            vec2 xz = p.xz;
            float ch = channelGlow(xz);
            float glow = ch * glowPeak * amod;
            float pulseT = t * flowSpeed * 3.5;
            float flow = 0.5 + 0.5*sin(xz.x*5.0 + xz.y*3.5 - pulseT);
            float pulse = ch * flow;
            // fwidth AA on channel edges
            float fw = fwidth(ch);
            float edgeAA = smoothstep(0.0, fw*6.0, ch) * (1.0-smoothstep(1.0-fw*6.0, 1.0, ch));
            vec3 baseCol = mix(C_BLUE, C_CYAN, ch);
            baseCol = mix(baseCol, C_VIOLET, noiseS(xz*1.8+vec2(t*0.02))*ch*0.5);
            col = C_LAND;
            col += baseCol * glow;
            col += C_GOLD * pulse * glow * 0.5;
            col += C_CYAN * edgeAA * glowPeak * 0.6;
        }
    }
    gl_FragColor = vec4(col, 1.0);
}
