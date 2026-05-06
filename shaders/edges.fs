/*{
  "DESCRIPTION": "Neon Octahedron Web — 3D raymarched double octahedron lattice with 24 glowing neon edge tubes",
  "CREDIT": "Easel auto-improve 2026-05-06",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "camSpeed",  "LABEL": "Cam Speed",  "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0, "MAX": 0.6 },
    { "NAME": "tubeRadius","LABEL": "Tube Radius","TYPE": "float", "DEFAULT": 0.04, "MIN": 0.01,"MAX": 0.15 },
    { "NAME": "glowAmt",   "LABEL": "Glow",       "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "audioReact","LABEL": "Audio React","TYPE": "float", "DEFAULT": 0.9,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define MAX_DIST  15.0
#define SURF_DIST 0.003

// 6 vertices of an octahedron (unit)
vec3 oct[6];
void initOct(float scale) {
    oct[0] = vec3( scale,0,0);
    oct[1] = vec3(-scale,0,0);
    oct[2] = vec3(0, scale,0);
    oct[3] = vec3(0,-scale,0);
    oct[4] = vec3(0,0, scale);
    oct[5] = vec3(0,0,-scale);
}

// SDF capsule between two points
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b - a, ap = p - a;
    float t = clamp(dot(ap,ab)/dot(ab,ab), 0.0, 1.0);
    return length(ap - ab*t) - r;
}

// 12 edges of an octahedron: connect every vertex to every non-opposite vertex
// Opposite pairs: (0,1),(2,3),(4,5)
float octEdges(vec3 p, float scale, float r) {
    initOct(scale);
    float d = MAX_DIST;
    for (int i = 0; i < 6; i++) {
        for (int j = i+1; j < 6; j++) {
            // Skip opposite pairs
            if ((i==0&&j==1)||(i==2&&j==3)||(i==4&&j==5)) continue;
            d = min(d, sdCapsule(p, oct[i], oct[j], r));
        }
    }
    return d;
}

// Color index for edge between vertices i and j
vec3 edgeColor(int i, int j, float t) {
    vec3 colors[4];
    colors[0] = vec3(1.0, 0.0, 0.6);  // hot magenta
    colors[1] = vec3(0.0, 0.9, 1.0);  // electric cyan
    colors[2] = vec3(1.0, 0.85, 0.0); // gold
    colors[3] = vec3(0.2, 1.0, 0.1);  // lime
    int idx = int(mod(float(i + j*3), 4.0));
    return colors[idx];
}

float map(vec3 p) {
    float r = tubeRadius;
    float d = MAX_DIST;
    // Outer octahedron (scale 1.0)
    d = min(d, octEdges(p, 1.0, r));
    // Inner octahedron (scale 0.5, rotated 45deg around Y)
    float ca = cos(0.7854), sa = sin(0.7854);
    vec3 q = vec3(ca*p.x+sa*p.z, p.y, -sa*p.x+ca*p.z);
    d = min(d, octEdges(q, 0.5, r*0.8));
    // Connecting spokes between inner and outer poles
    initOct(1.0);
    vec3 inn[6];
    inn[0] = vec3(0.5,0,0); inn[1] = vec3(-0.5,0,0);
    inn[2] = vec3(0,0.5,0); inn[3] = vec3(0,-0.5,0);
    inn[4] = vec3(0,0,0.5); inn[5] = vec3(0,0,-0.5);
    for (int k = 0; k < 6; k++) {
        d = min(d, sdCapsule(p, oct[k], inn[k], r*0.6));
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

// Closest edge color at point p
vec3 getEdgeColor(vec3 p) {
    float r = tubeRadius;
    float best = MAX_DIST;
    vec3 col = vec3(1.0, 0.0, 0.6);
    initOct(1.0);
    // Outer edges
    vec3 colors[4];
    colors[0] = vec3(1.0, 0.0, 0.6);
    colors[1] = vec3(0.0, 0.9, 1.0);
    colors[2] = vec3(1.0, 0.85, 0.0);
    colors[3] = vec3(0.2, 1.0, 0.1);
    int ci = 0;
    for (int i = 0; i < 6; i++) {
        for (int j = i+1; j < 6; j++) {
            if ((i==0&&j==1)||(i==2&&j==3)||(i==4&&j==5)) continue;
            float d = sdCapsule(p, oct[i], oct[j], r);
            if (d < best) { best = d; col = colors[int(mod(float(i+j*3),4.0))]; }
        }
    }
    return col;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float audio = 1.0 + audioLevel*audioReact*0.3 + audioBass*audioReact*0.2;

    // Orbiting camera
    float camT = TIME * camSpeed;
    vec3 ro = vec3(sin(camT)*3.5, sin(camT*0.37)*0.8, cos(camT)*3.5);
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
        dist += d;
        if (dist > MAX_DIST) break;
    }

    // Background: deep void with faint star noise
    vec2 sv = normalize(rd.xy + 0.001);
    float star = step(0.998, fract(sin(dot(floor(rd*80.0), vec3(127.1,311.7,74.7)))*43758.5453));
    vec3 col = vec3(star)*0.4 + vec3(0.003,0.004,0.01);

    if (hitT < MAX_DIST) {
        vec3 p   = ro + rd*hitT;
        vec3 n   = calcNormal(p);
        vec3 ec  = getEdgeColor(p);
        vec3 lDir = normalize(vec3(1,2,1));
        float diff = 0.3 + 0.7*max(dot(n,lDir),0.0);
        float spec = pow(max(dot(reflect(-lDir,n),-rd),0.0), 64.0);
        float rim  = 1.0 - max(dot(n,-rd),0.0);

        // Black ink core with colored rim glow
        float coreDark = smoothstep(0.0, tubeRadius*0.4, map(p));
        col = ec * diff * glowAmt * audio;
        col += ec * rim*rim * glowAmt * 0.8 * audio;
        col += vec3(1.0) * spec * glowAmt * audio;
        col *= 0.05 + coreDark;  // black core
    }

    // Volumetric neon glow around tubes (marching accumulation)
    float glow = 0.0;
    vec3 glowCol = vec3(0.0);
    float gt = 0.0;
    for (int i = 0; i < 48; i++) {
        vec3 gp = ro + rd*gt;
        float d = map(gp);
        float contrib = exp(-max(d,0.0)*30.0) * 0.04;
        glow += contrib;
        gt += max(d*0.4, 0.05);
        if (gt > MAX_DIST || glow > 1.0) break;
    }
    // Dominant glow color: use closest tube color at camera midpoint
    vec3 midP = ro + rd*(hitT < MAX_DIST ? hitT*0.5 : 2.0);
    vec3 gc = getEdgeColor(midP);
    col += gc * glow * glowAmt * 0.6 * audio;

    gl_FragColor = vec4(col, 1.0);
}
