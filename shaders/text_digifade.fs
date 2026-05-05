/*{
  "DESCRIPTION": "Holographic Data Sphere — 3D raymarched sphere with three orbiting torus rings, HDR chromatic shading and audio pulse",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "sphereR",    "LABEL": "Sphere Size",  "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.2,  "MAX": 1.2  },
    { "NAME": "torusR",     "LABEL": "Orbit Radius", "TYPE": "float", "DEFAULT": 1.35, "MIN": 0.8,  "MAX": 2.2  },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0  },
    { "NAME": "ringSpeed",  "LABEL": "Ring Speed",   "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 2.0  },
    { "NAME": "audioPulse", "LABEL": "Audio Pulse",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0  }
  ]
}*/

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5); }

float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

// Torus rotated by angle on XY plane
float sdTorusXY(vec3 p, float R, float r, float angle) {
    float c = cos(angle), s = sin(angle);
    vec3 q = vec3(p.x*c - p.y*s, p.x*s + p.y*c, p.z);
    return sdTorus(q, R, r);
}

vec2 scene(vec3 p) {
    float pulse = 1.0 + audioBass * audioPulse * 0.12;
    float sr = sphereR * pulse;
    float d = sdSphere(p, sr);
    float matId = 0.0;

    // 3 tori: each orbiting at a different phase and tilt
    for (int i = 0; i < 3; i++) {
        float fi = float(i);
        float orbitPhase = fi * 2.094 + TIME * ringSpeed * (0.7 + fi * 0.28);
        float tiltAngle  = fi * 1.047;
        float thickR = 0.04 + fi * 0.008;

        // Orbit: rotate p around Y by orbitPhase, then tilt around Z by tiltAngle
        float cO = cos(orbitPhase), sO = sin(orbitPhase);
        vec3 rp = vec3(p.x*cO + p.z*sO, p.y, -p.x*sO + p.z*cO);
        float cT = cos(tiltAngle), sT = sin(tiltAngle);
        vec3 tp = vec3(rp.x, rp.y*cT - rp.z*sT, rp.y*sT + rp.z*cT);
        float td = sdTorus(tp, torusR, thickR * (1.0 + audioHigh * audioPulse * 0.2));
        if (td < d) { d = td; matId = fi + 1.0; }
    }

    return vec2(d, matId);
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(0.003, 0.0);
    return normalize(vec3(
        scene(p+e.xyy).x - scene(p-e.xyy).x,
        scene(p+e.yxy).x - scene(p-e.yxy).x,
        scene(p+e.yyx).x - scene(p-e.yyx).x
    ));
}

// 5-hue holographic palette
vec3 holoColor(float id, vec3 p) {
    int ci = int(mod(id, 5.0));
    if (ci == 0) {
        // Sphere: iridescent based on normal direction
        float az = atan(p.z, p.x);
        float po = p.y / sphereR;
        float plasma = sin(az*3.0 + TIME*1.1)*0.4 + sin(po*4.0 + TIME*0.8)*0.4
                     + sin(az*5.0 - po*2.0 + TIME*0.5)*0.2;
        float t = plasma*0.5 + 0.5;
        if (t < 0.25)       return mix(vec3(0.0, 0.9, 1.0), vec3(1.0, 0.0, 0.6), t*4.0);
        else if (t < 0.5)   return mix(vec3(1.0, 0.0, 0.6), vec3(1.0, 0.75, 0.0), (t-0.25)*4.0);
        else if (t < 0.75)  return mix(vec3(1.0, 0.75, 0.0), vec3(0.0, 1.0, 0.35), (t-0.5)*4.0);
        else                return mix(vec3(0.0, 1.0, 0.35), vec3(0.7, 0.0, 1.0), (t-0.75)*4.0);
    }
    if (ci == 1) return vec3(0.0,  0.9,  1.0);   // cyan ring
    if (ci == 2) return vec3(1.0,  0.0,  0.55);  // magenta ring
               return vec3(1.0,  0.75, 0.0);   // gold ring
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    float camA = TIME * 0.09;
    vec3 ro = vec3(sin(camA)*3.8, 0.9 + sin(TIME*0.13)*0.4, cos(camA)*3.8);
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x*ri + uv.y*up + 1.6*fw);

    float bgH = dot(rd, vec3(0,1,0))*0.5 + 0.5;
    vec3 bg = mix(vec3(0.0, 0.0, 0.012), vec3(0.0, 0.008, 0.03), bgH*bgH);

    // Grid haze suggesting a holographic projector floor
    float gridX = abs(fract(rd.x * 6.0 + 0.5) - 0.5);
    float gridZ = abs(fract(rd.z * 6.0 + 0.5) - 0.5);
    bg += vec3(0.0, 0.5, 1.0) * max(0.0, 0.03 - min(gridX, gridZ)) * 4.0 * (1.0 - bgH);

    float t = 0.05; float mid = -99.0;
    for (int i = 0; i < 80; i++) {
        vec2 res = scene(ro + rd*t);
        if (res.x < 0.003) { mid = res.y; break; }
        if (t > 14.0) break;
        t += res.x * 0.75;
    }

    vec3 col = bg;
    if (mid >= 0.0) {
        vec3 p  = ro + rd*t;
        vec3 n  = getNormal(p);
        vec3 L  = normalize(vec3(0.4, 1.3, 0.5));
        vec3 L2 = normalize(vec3(-0.5, 0.3, -0.6));
        vec3 basecol = holoColor(mid, p);
        float diff = max(dot(n,L),0.0)*0.5 + max(dot(n,L2),0.0)*0.15 + 0.2;
        float spec = pow(max(dot(reflect(-L,n),-rd),0.0), 48.0);
        float rim  = pow(clamp(1.0-dot(n,-rd),0.0,1.0), 2.5);
        float face = smoothstep(0.0, 0.2, dot(n,-rd));
        col  = basecol * diff * hdrPeak * face;
        col += basecol * rim  * hdrPeak * 1.6;
        col += vec3(1.0) * spec * hdrPeak;
        col += basecol * 0.3 * hdrPeak;  // holographic emission

        // Scanline modulation (holographic effect — subtle)
        float scan = 0.85 + 0.15 * sin(p.y * 18.0 + TIME * 4.0);
        col *= scan;
    }

    col = mix(col, bg, clamp((t-6.0)/8.0, 0.0, 1.0)*0.5);
    gl_FragColor = vec4(col, 1.0);
}
