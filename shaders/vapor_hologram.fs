/*{
  "DESCRIPTION": "Neon Cathedral — raymarched gothic arches with volumetric jewel-toned light shafts. Deep violet/gold/cyan/magenta palette",
  "CREDIT": "ShaderClaw auto-improve v3",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "archScale",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.5, "MAX": 2.5, "LABEL": "Arch Scale" },
    { "NAME": "shaftDens",  "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Light Shaft" },
    { "NAME": "camSway",    "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.5, "LABEL": "Cam Sway" },
    { "NAME": "glowPeak",   "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0, "LABEL": "Glow Peak" },
    { "NAME": "audioMod",   "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" }
  ]
}*/

#define STEPS 80
#define MAXD  18.0
#define EPS   0.004

const vec3 C_VIOLET  = vec3(0.5,  0.0,  1.0);
const vec3 C_GOLD    = vec3(1.0,  0.75, 0.0);
const vec3 C_CYAN    = vec3(0.0,  0.85, 1.0);
const vec3 C_MAGENTA = vec3(1.0,  0.0,  0.8);

float sdCylinder(vec3 p, float r, float h) {
    vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(r, h);
    return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p)-b;
    return length(max(q,0.0))+min(max(q.x,max(q.y,q.z)),0.0);
}
float sdArch(vec3 p, float W, float H, float thick) {
    // Pointed gothic arch: two offset cylinders forming the arch shape
    float r = W * 0.55;
    float c1 = sdCylinder(p - vec3(-W*0.25, 0.0, 0.0), r, H);
    float c2 = sdCylinder(p - vec3( W*0.25, 0.0, 0.0), r, H);
    float arch = max(-c1, -c2); // intersection = arch void
    // Arch frame: box minus arch void
    float frame = sdBox(p, vec3(W*0.5+thick, H, thick*0.5));
    return max(frame, -max(-c1,-c2) + thick);
}

float sdf(vec3 p) {
    float s = archScale;
    // Repeat arches along X axis
    float rep = 2.8 * s;
    vec3 pr = p;
    pr.x = mod(pr.x + rep*0.5, rep) - rep*0.5;
    float arch = sdArch(pr - vec3(0.0, 0.0*s, 0.0), 1.2*s, 2.5*s, 0.15*s);
    // Floor
    float floor_ = p.y + 0.5*s;
    return min(arch, floor_);
}

vec3 calcN(vec3 p) {
    const float e = 0.003;
    return normalize(vec3(
        sdf(p+vec3(e,0,0))-sdf(p-vec3(e,0,0)),
        sdf(p+vec3(0,e,0))-sdf(p-vec3(0,e,0)),
        sdf(p+vec3(0,0,e))-sdf(p-vec3(0,0,e))));
}

// Volumetric light shaft: samples along ray through colored pillars of light
vec3 lightShafts(vec3 ro, vec3 rd, float hitD) {
    vec3 shaftCol = vec3(0.0);
    float stepD = min(hitD, MAXD) / 20.0;
    for (int i = 0; i < 20; i++) {
        float sd = stepD * (float(i) + 0.5);
        vec3 sp = ro + rd * sd;
        // Color per X position band
        float rep = 2.8 * archScale;
        float bx = mod(sp.x + rep*0.5, rep) / rep;
        vec3 shCol;
        if      (bx < 0.25) shCol = C_VIOLET;
        else if (bx < 0.5)  shCol = C_GOLD;
        else if (bx < 0.75) shCol = C_CYAN;
        else                shCol = C_MAGENTA;
        // Shaft fades with height (stronger high up) and distance from axis
        float heightFade = smoothstep(-0.5*archScale, 2.0*archScale, sp.y);
        float radial = exp(-abs(sp.x - rep*0.5) * 1.5 / archScale);
        shaftCol += shCol * heightFade * radial * shaftDens * stepD * 0.15;
    }
    return shaftCol;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float amod = 1.0 + audioLevel * audioMod * 0.2;
    // Camera inside cathedral looking up/forward, gentle sway
    vec3 ro = vec3(sin(t*camSway)*0.3, -0.3*archScale, -t*0.08);
    vec3 ta = ro + vec3(0.0, 1.5, -1.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 1.6*ww);
    float d = 0.0;
    bool hit = false;
    for (int i = 0; i < STEPS; i++) {
        float h = sdf(ro + rd * d);
        if (h < EPS) { hit = true; break; }
        if (d > MAXD) break;
        d += max(h, EPS);
    }
    // Dark void with volumetric shafts
    float hitD = hit ? d : MAXD;
    vec3 col = lightShafts(ro, rd, hitD) * glowPeak * amod;
    if (hit) {
        vec3 p = ro + rd * d;
        vec3 n = calcN(p);
        // Color stone arch by band
        float rep = 2.8 * archScale;
        float bx = mod(p.x + rep*0.5, rep) / rep;
        vec3 bc;
        if      (bx < 0.25) bc = C_VIOLET;
        else if (bx < 0.5)  bc = C_GOLD;
        else if (bx < 0.75) bc = C_CYAN;
        else                bc = C_MAGENTA;
        // Ambient lit stone (no direct sun — stained glass colored ambient)
        float diff = max(dot(n, normalize(vec3(0,1,-0.3))), 0.0) * 0.4 + 0.15;
        float nv   = max(dot(n,-rd),0.0);
        float ink  = 1.0 - smoothstep(0.0, 0.2, nv);
        col += mix(bc*diff*glowPeak, vec3(0.0), ink*0.9);
    }
    gl_FragColor = vec4(col, 1.0);
}
