/*{
  "DESCRIPTION": "Bioluminescent Mycelium Network — 3D raymarched fungal thread network: teal-violet-white capsule tubes glowing in void black, slow pulsing bioluminescence",
  "CREDIT": "Easel auto-improve 2026-05-06",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "nodeCount",  "LABEL": "Node Count",  "TYPE": "float", "DEFAULT": 12.0, "MIN": 4.0,  "MAX": 24.0 },
    { "NAME": "netRadius",  "LABEL": "Net Radius",  "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.3,  "MAX": 2.5 },
    { "NAME": "tubeRadius", "LABEL": "Tube Radius", "TYPE": "float", "DEFAULT": 0.025,"MIN": 0.005,"MAX": 0.1 },
    { "NAME": "glowScale",  "LABEL": "Glow Scale",  "TYPE": "float", "DEFAULT": 2.4,  "MIN": 1.0,  "MAX": 5.0 },
    { "NAME": "pulseSpeed", "LABEL": "Pulse Speed", "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

#define MAX_MARCH 64
#define MAX_DIST  10.0
#define SURF_DIST 0.003

float hash11(float n) { return fract(sin(n*12.9898)*43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// Reproducible node positions (Fibonacci-on-sphere, scaled)
vec3 nodePos(int i, float t) {
    float fi = float(i);
    float golden = 2.39996322972865; // golden angle
    float phi = acos(1.0 - 2.0*(fi+0.5)/nodeCount);
    float theta = golden * fi;
    // Slow drift: each node oscillates along its radial direction
    float drift = sin(t * pulseSpeed * (0.3 + hash11(fi)*0.4) + fi*1.7) * 0.08;
    return (netRadius + drift) * vec3(sin(phi)*cos(theta), cos(phi), sin(phi)*sin(theta));
}

// SDF of single capsule (connection between two nodes)
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b-a, ap = p-a;
    float t = clamp(dot(ap,ab)/dot(ab,ab), 0.0, 1.0);
    return length(ap - ab*t) - r;
}

// SDF of full mycelium network (nodes + connections)
float mapMycelium(vec3 p) {
    float d = MAX_DIST;
    int N = int(clamp(nodeCount, 4.0, 24.0));
    float t = TIME * pulseSpeed;

    // Node spheres (junction points)
    for (int i = 0; i < 24; i++) {
        if (i >= N) break;
        vec3 np = nodePos(i, TIME);
        d = min(d, length(p - np) - tubeRadius*2.5); // junction sphere
    }

    // Connections: each node connects to 3 nearest neighbors
    // (simplified: connect node i to nodes i+1, i+2, i+3 mod N)
    for (int i = 0; i < 24; i++) {
        if (i >= N) break;
        vec3 a = nodePos(i, TIME);
        for (int j = 1; j <= 3; j++) {
            int k = int(mod(float(i+j), float(N)));
            vec3 b = nodePos(k, TIME);
            d = min(d, sdCapsule(p, a, b, tubeRadius));
        }
    }
    return d;
}

// Bioluminescent color per tube based on position (teal->violet->white-hot)
vec3 myceliumColor(vec3 p, float nodeAmt) {
    // Height-based color mapping
    float ht = p.y / netRadius;
    vec3 col;
    float t3 = clamp(ht * 1.5 + 0.5, 0.0, 1.0) * 3.0;
    vec3 c0 = vec3(0.0, 0.85, 0.7);  // teal
    vec3 c1 = vec3(0.5, 0.1,  1.0);  // violet
    vec3 c2 = vec3(0.8, 0.4,  1.0);  // bright violet
    vec3 c3 = vec3(1.1, 1.0,  1.05); // white-hot HDR
    if (t3<1.0) col = mix(c0,c1,t3);
    else if (t3<2.0) col = mix(c1,c2,t3-1.0);
    else col = mix(c2,c3,t3-2.0);
    // Pulse brightness based on node proximity
    col *= 0.6 + 0.4*nodeAmt;
    return col;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        mapMycelium(p+e.xyy)-mapMycelium(p-e.xyy),
        mapMycelium(p+e.yxy)-mapMycelium(p-e.yxy),
        mapMycelium(p+e.yyx)-mapMycelium(p-e.yyx)));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float audio = 1.0 + audioLevel*audioReact*0.3 + audioBass*audioReact*0.25;

    // Close-up camera orbiting the mycelium network
    float camT = TIME * pulseSpeed * 0.4;
    float camR = netRadius * 2.8;
    vec3 ro = vec3(sin(camT)*camR, sin(camT*0.37)*netRadius*0.5, cos(camT)*camR);
    vec3 ta = vec3(0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv = cross(uu, ww);
    vec3 rd = normalize(uv.x*uu + uv.y*vv + 1.8*ww);

    float dist = 0.0;
    float hitT = MAX_DIST;
    for (int i = 0; i < MAX_MARCH; i++) {
        vec3 p = ro + rd*dist;
        float d = mapMycelium(p);
        if (d < SURF_DIST) { hitT = dist; break; }
        dist += d * 0.7;
        if (dist > MAX_DIST) break;
    }

    // Deep void background -- absolute black-blue
    vec3 col = vec3(0.003, 0.004, 0.008);

    if (hitT < MAX_DIST) {
        vec3 p   = ro + rd*hitT;
        vec3 n   = calcNormal(p);

        // Find nearest node to determine junction proximity
        float nodeProx = 0.0;
        int N = int(clamp(nodeCount, 4.0, 24.0));
        for (int i = 0; i < 24; i++) {
            if (i >= N) break;
            vec3 np = nodePos(i, TIME);
            float nd = length(p - np);
            nodeProx = max(nodeProx, exp(-nd * 5.0));
        }

        vec3 baseCol = myceliumColor(p, nodeProx);
        vec3 lDir = normalize(vec3(0.5, 1.0, 0.3));
        float diff = 0.2 + 0.8*max(dot(n,lDir),0.0);
        float spec = pow(max(dot(reflect(-lDir,n),-rd),0.0),32.0);
        float rim  = pow(1.0-max(dot(n,-rd),0.0),4.0);

        col = baseCol * diff * glowScale * audio;
        col += vec3(1.0,0.9,1.0) * spec * glowScale * audio;        // white-hot spec
        col += baseCol * rim * glowScale * 0.5 * audio;              // rim glow

        // fwidth AA on tube silhouette
        float tubeSdf = mapMycelium(p);
        col *= 0.02 + smoothstep(0.0, fwidth(tubeSdf)*2.0, abs(tubeSdf)+SURF_DIST);
    }

    // Volumetric bioluminescent glow (fog of tubes glowing in dark)
    float vt = 0.0;
    for (int i = 0; i < 48; i++) {
        vec3 vp = ro + rd*vt;
        float d = mapMycelium(vp);
        vec3 vc = myceliumColor(vp, 0.5);
        float contrib = exp(-max(d,0.0)*18.0) * 0.04;
        col += vc * contrib * glowScale * audio;
        vt += max(d*0.5, 0.05);
        if (vt > MAX_DIST) break;
    }

    // Bioluminescent pulse: slow sinusoidal brightness modulation
    float pulse = 0.85 + 0.15*sin(TIME*pulseSpeed*2.3);
    col *= pulse;

    gl_FragColor = vec4(col, 1.0);
}
