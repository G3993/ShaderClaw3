/*{
  "DESCRIPTION": "Alien Mushroom Garden — 3D raymarched bioluminescent mushroom field with glowing caps, drifting spores, deep-violet atmosphere",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "mushroomCount", "LABEL": "Mushrooms",   "TYPE": "float", "DEFAULT": 7.0,  "MIN": 2.0, "MAX": 12.0 },
    { "NAME": "glowStrength",  "LABEL": "Bio Glow",    "TYPE": "float", "DEFAULT": 2.2,  "MIN": 0.5, "MAX": 4.0  },
    { "NAME": "sporeCount",    "LABEL": "Spores",      "TYPE": "float", "DEFAULT": 18.0, "MIN": 0.0, "MAX": 40.0 },
    { "NAME": "audioPulse",    "LABEL": "Audio Pulse", "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0  },
    { "NAME": "swaySpeed",     "LABEL": "Sway Speed",  "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 2.0  }
  ]
}*/

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5); }

float sdCap(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p-a, ba = b-a;
    float h = clamp(dot(pa,ba)/dot(ba,ba), 0.0, 1.0);
    return length(pa - ba*h) - r;
}

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5*(b-a)/k, 0.0, 1.0);
    return mix(b, a, h) - k*h*(1.0-h);
}

float sdMushroom(vec3 p, float stemR, float capR) {
    float stem = sdCap(p, vec3(0.0), vec3(0.0, 1.0, 0.0), stemR);
    vec3 cp = (p - vec3(0.0, 1.0 + capR*0.38, 0.0)) / vec3(1.0, 0.52, 1.0);
    float cap = length(cp) - capR;
    return smin(stem, cap, 0.11);
}

vec2 scene(vec3 p) {
    float bestD = 1e9; float bestId = -1.0;
    int N = int(clamp(mushroomCount, 2.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= N) break;
        float fi = float(i);
        float ph  = hash1(fi * 3.7) * 6.2832;
        float rad = 0.8 + hash1(fi * 5.1) * 1.5;
        float ang = fi * 2.399;
        float sway = sin(TIME * swaySpeed + ph) * 0.09;
        vec3 ctr = vec3(sin(ang + sway) * rad, -0.05, cos(ang) * rad);
        float sc  = 0.22 + hash1(fi * 7.3) * 0.38;
        float capR = 0.5 + hash1(fi * 2.1) * 0.35;
        float pulse = 1.0 + audioBass * audioPulse * 0.11;
        float di = sdMushroom((p - ctr) / (sc * pulse), 0.19, capR) * sc * pulse;
        if (di < bestD) { bestD = di; bestId = fi; }
    }
    float gnd = p.y + 0.06;
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

vec3 mushroomColor(float id) {
    int ci = int(mod(id, 5.0));
    if (ci == 0) return vec3(0.0,  1.0,  0.85);
    if (ci == 1) return vec3(1.0,  0.05, 0.75);
    if (ci == 2) return vec3(0.35, 1.0,  0.0);
    if (ci == 3) return vec3(0.55, 0.0,  1.0);
               return vec3(1.0,  0.6,  0.0);
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    float camA = TIME * 0.09;
    vec3 ro = vec3(sin(camA)*3.8, 1.1, cos(camA)*3.8);
    vec3 ta = vec3(0.0, 0.5, 0.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x*ri + uv.y*up + 1.65*fw);

    float bgH = dot(rd, vec3(0,1,0))*0.5 + 0.5;
    vec3 bg = mix(vec3(0.0, 0.0, 0.012), vec3(0.04, 0.0, 0.1), bgH);

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
        vec3 L  = normalize(vec3(0.3, 1.5, 0.5));
        vec3 L2 = normalize(vec3(-0.4, 0.3, -0.7));
        vec3 basecol = mushroomColor(mid);
        float diff = max(dot(n,L),0.0)*0.55 + max(dot(n,L2),0.0)*0.2 + 0.25;
        float spec = pow(max(dot(reflect(-L,n),-rd),0.0), 24.0);
        float rim  = pow(clamp(1.0-dot(n,-rd),0.0,1.0), 3.0);
        float face = smoothstep(0.0, 0.22, dot(n,-rd));
        col  = basecol * diff * glowStrength * face;
        col += basecol * rim  * glowStrength * 1.4; // HDR rim
        col += vec3(1.0)* spec * glowStrength;       // HDR spec
        col += basecol * 0.55 * glowStrength;        // bioluminescent emissive
    } else if (mid > -0.5) {
        vec3 p = ro + rd*t;
        float gx = step(0.47, abs(fract(p.x*3.0)-0.5));
        float gz = step(0.47, abs(fract(p.z*3.0)-0.5));
        col = vec3(0.0, 0.01, 0.018) + vec3(0.0, 0.03, 0.04)*min(gx+gz, 1.0);
    }

    // Floating spores
    int NS = int(clamp(sporeCount, 0.0, 40.0));
    for (int i = 0; i < 40; i++) {
        if (i >= NS) break;
        float fi = float(i);
        vec3 sp = vec3(
            sin(fi*1.37 + TIME*0.17 + hash1(fi*7.1)*6.28) * 2.6,
            0.3 + hash1(fi*3.1)*1.8 + sin(TIME*0.2+hash1(fi*2.7)*6.28)*0.22,
            cos(fi*2.09 + TIME*0.13 + hash1(fi*5.3)*6.28) * 2.6
        );
        vec3 dv = sp - ro;
        float tcl = dot(rd, dv);
        if (tcl > 0.01 && tcl < t+0.1) {
            float d2 = length(dv - rd*tcl);
            if (d2 < 0.05) col += vec3(0.35, 1.0, 0.55) * (0.05-d2)*28.0;
        }
    }

    col = mix(col, bg, clamp((t-5.0)/7.0, 0.0, 1.0)*0.45);
    gl_FragColor = vec4(col, 1.0);
}
