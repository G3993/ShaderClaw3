/*{
  "DESCRIPTION": "Falling Crystal Prisms — 3D raymarched octahedral crystals drifting through void with HDR edge glow",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "prismCount",  "LABEL": "Prisms",      "TYPE": "float", "DEFAULT": 8.0, "MIN": 2.0, "MAX": 14.0 },
    { "NAME": "glowStrength","LABEL": "Glow",         "TYPE": "float", "DEFAULT": 2.2, "MIN": 0.5, "MAX": 4.0  },
    { "NAME": "fallSpeed",   "LABEL": "Fall Speed",   "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 2.0  },
    { "NAME": "audioPulse",  "LABEL": "Audio Pulse",  "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0  },
    { "NAME": "spinSpeed",   "LABEL": "Spin Speed",   "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0  }
  ]
}*/

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5); }

// Approximate elongated octahedron SDF (elongated along Y)
float sdOctPrism(vec3 p, float r, float elongate) {
    p.y /= elongate;
    return (abs(p.x) + abs(p.y) + abs(p.z) - r) * 0.57735 * min(1.0, elongate);
}

vec2 scene(vec3 p) {
    float bestD = 1e9; float bestId = -1.0;
    int N = int(clamp(prismCount, 2.0, 14.0));
    for (int i = 0; i < 14; i++) {
        if (i >= N) break;
        float fi = float(i);
        float ph   = hash1(fi * 3.7) * 6.2832;
        float rad  = 0.9 + hash1(fi * 5.1) * 2.1;
        float ang  = fi * 2.399;
        float yOff = mod(hash1(fi * 2.3) * 9.0 - TIME * fallSpeed * (0.35 + hash1(fi*4.1)*0.55), 9.0) - 4.5;
        float spinA = TIME * spinSpeed * (0.4 + hash1(fi*1.7)*0.9) + ph;

        vec3 ctr = vec3(sin(ang)*rad, yOff, cos(ang)*rad);
        vec3 q = p - ctr;

        // Spin XZ
        float cA = cos(spinA), sA = sin(spinA);
        float qx = q.x*cA - q.z*sA;
        float qz = q.x*sA + q.z*cA;
        q.x = qx; q.z = qz;

        // Tilt XY
        float cB = cos(spinA*0.6), sB = sin(spinA*0.6);
        float qx2 = q.x*cB - q.y*sB;
        float qy2 = q.x*sB + q.y*cB;
        q.x = qx2; q.y = qy2;

        float sc      = 0.2 + hash1(fi * 6.3) * 0.25;
        float elongate = 1.5 + hash1(fi * 8.1) * 1.3;
        float pulse   = 1.0 + audioBass * audioPulse * 0.12;
        float di = sdOctPrism(q / (sc * pulse), 1.0, elongate) * sc * pulse;
        if (di < bestD) { bestD = di; bestId = fi; }
    }
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

vec3 prismColor(float id) {
    int ci = int(mod(id, 5.0));
    if (ci == 0) return vec3(0.0,  0.9,  1.0);   // icy cyan
    if (ci == 1) return vec3(0.65, 0.0,  1.0);   // violet
    if (ci == 2) return vec3(1.0,  0.75, 0.0);   // gold
    if (ci == 3) return vec3(1.0,  0.0,  0.55);  // magenta
               return vec3(0.0,  1.0,  0.35);   // crystal green
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    float camA = TIME * 0.06;
    vec3 ro = vec3(sin(camA)*5.5, 1.5 + sin(TIME*0.09)*1.2, cos(camA)*5.5);
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x*ri + uv.y*up + 1.6*fw);

    float bgH = dot(rd, vec3(0,1,0))*0.5 + 0.5;
    vec3 bg = mix(vec3(0.0, 0.0, 0.01), vec3(0.0, 0.005, 0.025), bgH*bgH);

    // Star field
    for (int i = 0; i < 40; i++) {
        float fi = float(i);
        vec3 sd = normalize(vec3(
            fract(sin(fi*1.37)*43758.5)*2.0-1.0,
            fract(sin(fi*2.71)*43758.5)*2.0-1.0,
            fract(sin(fi*4.13)*43758.5)*2.0-1.0
        ));
        float d = length(rd - sd);
        float tw = 0.5 + 0.5*sin(TIME*(0.7+fi*0.08)+fi*3.14);
        bg += vec3(0.8, 0.9, 1.0) * smoothstep(0.008, 0.0, d) * tw * 0.3;
    }

    float t = 0.05; float mid = -99.0;
    for (int i = 0; i < 80; i++) {
        vec2 res = scene(ro + rd*t);
        if (res.x < 0.003) { mid = res.y; break; }
        if (t > 18.0) break;
        t += res.x * 0.75;
    }

    vec3 col = bg;
    if (mid >= 0.0) {
        vec3 p = ro + rd*t;
        vec3 n = getNormal(p);
        vec3 L  = normalize(vec3(0.5, 1.2, 0.4));
        vec3 basecol = prismColor(mid);
        float diff = max(dot(n,L),0.0)*0.5 + 0.18;
        float spec = pow(max(dot(reflect(-L,n),-rd),0.0), 52.0);
        float rim  = pow(clamp(1.0-dot(n,-rd),0.0,1.0), 2.5);
        float face = smoothstep(0.0, 0.18, dot(n,-rd));
        col  = basecol * diff * glowStrength * face;
        col += basecol * rim  * glowStrength * 1.7;
        col += vec3(1.0) * spec * glowStrength * 1.3;
        col += basecol * 0.25 * glowStrength;
    }

    col = mix(col, bg, clamp((t-10.0)/8.0, 0.0, 1.0)*0.6);
    gl_FragColor = vec4(col, 1.0);
}
