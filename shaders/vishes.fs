/*{
  "DESCRIPTION": "Physarum Network — raymarched 3D slime mold tube network with glowing junction nodes. Electric green/cyan/gold palette",
  "CREDIT": "ShaderClaw auto-improve v3",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "netScale",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.3, "MAX": 3.0, "LABEL": "Network Scale" },
    { "NAME": "tubeR",     "TYPE": "float", "DEFAULT": 0.06, "MIN": 0.01,"MAX": 0.2, "LABEL": "Tube Radius" },
    { "NAME": "nodeR",     "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.03,"MAX": 0.3, "LABEL": "Node Radius" },
    { "NAME": "glowPeak",  "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0, "LABEL": "Glow Peak" },
    { "NAME": "audioMod",  "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" }
  ]
}*/

#define STEPS 64
#define MAXD  10.0
#define EPS   0.004

const vec3 C_GREEN  = vec3(0.1,  1.0,  0.2);
const vec3 C_CYAN   = vec3(0.0,  0.9,  1.0);
const vec3 C_GOLD   = vec3(1.0,  0.8,  0.0);
const vec3 C_LIME   = vec3(0.5,  1.0,  0.0);

float hash31(vec3 p) { return fract(sin(dot(p, vec3(127.1,311.7,74.7)))*43758.5); }

// Network of tubes: 9 fixed nodes in a 3x3 grid + connecting tubes
// All positions time-animated slightly for liveness
vec3 nodePos(int i, float t) {
    float fi = float(i);
    float ox = (mod(fi, 3.0) - 1.0) * 1.2;
    float oz = (floor(fi / 3.0) - 1.0) * 1.2;
    // Slight breathing motion
    ox += sin(t * (0.3 + fi*0.1) + fi) * 0.08;
    oz += cos(t * (0.25 + fi*0.13) + fi*1.3) * 0.08;
    float oy = sin(t * 0.2 + fi * 0.7) * 0.1;
    return vec3(ox, oy, oz) * netScale;
}

float sdSphere(vec3 p, float r) { return length(p) - r; }
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa=p-a, ba=b-a;
    float h=clamp(dot(pa,ba)/dot(ba,ba),0.0,1.0);
    return length(pa-ba*h)-r;
}
float opSm(float a, float b, float k) {
    float h=max(k-abs(a-b),0.0)/k;
    return min(a,b)-h*h*k*0.25;
}

// Returns (sdf, material_id)
vec2 network(vec3 p, float t) {
    float amod = 1.0 + audioLevel * audioMod * 0.12;
    float tr = tubeR * netScale * amod;
    float nr = nodeR * netScale * amod;
    float md = 1e5;
    float mat = 0.0;
    // 9 nodes
    for (int i = 0; i < 9; i++) {
        vec3 n = nodePos(i, t);
        float d = sdSphere(p - n, nr);
        if (d < md) { md = d; mat = 1.0; }
    }
    // Connecting tubes between neighbors
    int pairs[12]; // flat pair list
    pairs[0]=0; pairs[1]=1; pairs[2]=1; pairs[3]=2;
    pairs[4]=3; pairs[5]=4; pairs[6]=4; pairs[7]=5;
    pairs[8]=6; pairs[9]=7; pairs[10]=7; pairs[11]=8;
    // + vertical connections
    for (int i = 0; i < 6; i++) {
        int a = i;
        int b = i + 3;
        vec3 na = nodePos(a, t);
        vec3 nb = nodePos(b, t);
        float d = sdCapsule(p, na, nb, tr);
        if (d < md) { md = d; mat = 0.0; }
    }
    // Diagonal cross connections
    for (int i = 0; i < 6; i++) {
        vec3 na = nodePos(i, t);
        vec3 nb = nodePos(i+2 < 9 ? i+2 : 8, t);
        float d = sdCapsule(p, na, nb, tr);
        if (d < md) { md = d; mat = 0.0; }
    }
    return vec2(md, mat);
}

vec3 calcN(vec3 p, float t) {
    const float e = 0.003;
    return normalize(vec3(
        network(p+vec3(e,0,0),t).x-network(p-vec3(e,0,0),t).x,
        network(p+vec3(0,e,0),t).x-network(p-vec3(0,e,0),t).x,
        network(p+vec3(0,0,e),t).x-network(p-vec3(0,0,e),t).x));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    // Aerial angled camera
    float camA = t * 0.12;
    vec3 ro = vec3(cos(camA)*5.0, 3.5, sin(camA)*5.0);
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 2.2*ww);
    float d = 0.0;
    float mat = -1.0;
    for (int i = 0; i < STEPS; i++) {
        vec2 res = network(ro+rd*d, t);
        if (res.x < EPS) { mat = res.y; break; }
        if (d > MAXD) break;
        d += max(res.x, EPS);
    }
    vec3 col = vec3(0.0, 0.0, 0.008);
    if (mat >= 0.0) {
        vec3 p = ro + rd * d;
        vec3 n = calcN(p, t);
        // Color: nodes = gold/hot, tubes = green/cyan
        float isNode = mat;
        vec3 bc = isNode > 0.5 ? C_GOLD : mix(C_GREEN, C_CYAN, fract(dot(p*0.5, vec3(0.37,0.53,0.41))+t*0.05));
        vec3 kL = normalize(vec3(0.5, 1.5, 0.3));
        float diff = max(dot(n,kL),0.0);
        float spec = pow(max(dot(reflect(-kL,n),-rd),0.0),16.0);
        float nv   = max(dot(n,-rd),0.0);
        float ink  = 1.0 - smoothstep(0.0, 0.2, nv);
        float peakScale = isNode > 0.5 ? 1.2 : 1.0; // nodes brighter
        col = mix(
            bc*(diff*0.7+0.2)*glowPeak*peakScale + vec3(1.0)*spec*0.3*glowPeak,
            vec3(0.0),
            ink * 0.9
        );
    }
    gl_FragColor = vec4(col, 1.0);
}
