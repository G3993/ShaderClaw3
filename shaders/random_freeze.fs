/*{
  "DESCRIPTION": "Bioluminescent Coral — 3D raymarched coral reef with glowing branch clusters, anemone polyps, and floating particles in dark water",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "branchCount",  "LABEL": "Branches",    "TYPE": "float", "DEFAULT": 6.0, "MIN": 2.0, "MAX": 10.0 },
    { "NAME": "glowStrength", "LABEL": "Glow",        "TYPE": "float", "DEFAULT": 2.3, "MIN": 0.5, "MAX": 4.0  },
    { "NAME": "audioPulse",   "LABEL": "Audio Pulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0  },
    { "NAME": "sway",         "LABEL": "Sway",        "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0  },
    { "NAME": "particleAmt",  "LABEL": "Particles",   "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0  }
  ]
}*/

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5); }

float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdCap(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p-a, ba = b-a;
    float h = clamp(dot(pa,ba)/dot(ba,ba), 0.0, 1.0);
    return length(pa - ba*h) - r;
}

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5*(b-a)/k, 0.0, 1.0);
    return mix(b, a, h) - k*h*(1.0-h);
}

// One coral branch: main stem + 3 sub-branches + polyp spheres at tips
float sdCoralBranch(vec3 p, float id, float t) {
    float swayAng = sin(t * sway + id * 2.1) * 0.12;
    // Main stem
    vec3 tip0 = vec3(swayAng, 1.2, 0.0);
    float d = sdCap(p, vec3(0.0), tip0, 0.055);
    // 3 sub-branches
    for (int j = 0; j < 3; j++) {
        float fj = float(j);
        float ba = fj * 2.094 + id; // evenly spaced around stem
        float h  = 0.5 + hash1(id*3.1 + fj) * 0.4; // branch height on stem
        vec3 from = tip0 * h;
        vec3 dir  = vec3(sin(ba)*0.55 + swayAng*0.5, 0.5, cos(ba)*0.55);
        vec3 to   = from + dir * (0.35 + hash1(id*7.1 + fj)*0.2);
        d = smin(d, sdCap(p, from, to, 0.035), 0.06);
        // Polyp sphere at tip
        d = smin(d, sdSphere(p - to, 0.065 * (1.0 + audioBass * audioPulse * 0.2)), 0.04);
    }
    // Polyp at main stem tip
    d = smin(d, sdSphere(p - tip0, 0.08 * (1.0 + audioMid * audioPulse * 0.15)), 0.05);
    return d;
}

vec2 scene(vec3 p) {
    float bestD = 1e9; float bestId = -1.0;
    int N = int(clamp(branchCount, 2.0, 10.0));
    for (int i = 0; i < 10; i++) {
        if (i >= N) break;
        float fi = float(i);
        float ang = fi * 2.399;
        float rad = 0.6 + hash1(fi * 5.1) * 1.2;
        float sc  = 0.4 + hash1(fi * 3.7) * 0.5;
        vec3 ctr  = vec3(sin(ang)*rad, -0.05, cos(ang)*rad);
        float di  = sdCoralBranch((p - ctr) / sc, fi, TIME) * sc;
        if (di < bestD) { bestD = di; bestId = fi; }
    }
    // Sea floor
    float gnd = p.y + 0.08;
    if (gnd < bestD) { bestD = gnd; bestId = -0.1; }
    return vec2(bestD, bestId);
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(0.003, 0.0);
    return normalize(vec3(
        scene(p+e.xyy).x - scene(p-e.xyy).x,
        scene(p+e.yxy).x - scene(p-e.yxy).x,
        scene(p+e.yyx).x - scene(p-e.yyx).x
    ));
}

vec3 coralColor(float id) {
    int ci = int(mod(id, 5.0));
    if (ci == 0) return vec3(1.0,  0.05, 0.75);  // hot magenta
    if (ci == 1) return vec3(0.0,  1.0,  0.85);  // electric cyan
    if (ci == 2) return vec3(0.2,  1.0,  0.2);   // vivid green
    if (ci == 3) return vec3(1.0,  0.55, 0.0);   // orange
               return vec3(0.6,  0.0,  1.0);    // violet
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    float camA = TIME * 0.1;
    vec3 ro = vec3(sin(camA)*3.6, 1.0, cos(camA)*3.6);
    vec3 ta = vec3(0.0, 0.6, 0.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x*ri + uv.y*up + 1.6*fw);

    float bgH = dot(rd, vec3(0,1,0))*0.5 + 0.5;
    vec3 bg = mix(vec3(0.0, 0.01, 0.04), vec3(0.0, 0.04, 0.12), bgH*bgH);

    float t = 0.05; float mid = -99.0;
    for (int i = 0; i < 64; i++) {
        vec2 res = scene(ro + rd*t);
        if (res.x < 0.003) { mid = res.y; break; }
        if (t > 12.0) break;
        t += res.x * 0.8;
    }

    vec3 col = bg;
    if (mid >= 0.0) {
        vec3 p = ro + rd*t;
        vec3 n = getNormal(p);
        vec3 L  = normalize(vec3(0.3, 1.5, 0.4));
        vec3 L2 = normalize(vec3(-0.3, 0.4, -0.8));
        vec3 basecol = coralColor(mid);
        float diff = max(dot(n,L),0.0)*0.55 + max(dot(n,L2),0.0)*0.2 + 0.25;
        float spec = pow(max(dot(reflect(-L,n),-rd),0.0), 20.0);
        float rim  = pow(clamp(1.0-dot(n,-rd),0.0,1.0), 3.0);
        float face = smoothstep(0.0, 0.2, dot(n,-rd));
        col  = basecol * diff * glowStrength * face;
        col += basecol * rim  * glowStrength * 1.5;
        col += vec3(1.0)* spec * glowStrength;
        col += basecol * 0.45 * glowStrength; // bioluminescent emission
    } else if (mid > -0.5) {
        // Sandy seafloor
        vec3 p = ro + rd*t;
        float noise = hash1(floor(p.x*5.0)*7.3 + floor(p.z*5.0)*3.1);
        col = vec3(0.0, 0.015, 0.025) + vec3(0.0, 0.01, 0.015)*noise;
    }

    // Bioluminescent particles
    for (int i = 0; i < 20; i++) {
        float fi = float(i);
        vec3 sp = vec3(
            sin(fi*1.41 + TIME*0.15 + hash1(fi*7.3)*6.28) * 2.8,
            0.2 + hash1(fi*2.9)*2.0 + sin(TIME*0.18 + hash1(fi*1.7)*6.28)*0.3,
            cos(fi*1.83 + TIME*0.12 + hash1(fi*4.1)*6.28) * 2.8
        );
        vec3 dv = sp - ro;
        float tcl = dot(rd, dv);
        if (tcl > 0.01 && tcl < t+0.1) {
            float d2 = length(dv - rd*tcl);
            if (d2 < 0.06) col += vec3(0.2, 0.9, 1.0) * (0.06-d2)*22.0 * particleAmt;
        }
    }

    col = mix(col, bg, clamp((t-4.5)/8.0, 0.0, 1.0)*0.5);
    gl_FragColor = vec4(col, 1.0);
}
