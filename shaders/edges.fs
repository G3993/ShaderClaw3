/*{
  "DESCRIPTION": "DNA Double Helix — 3D raymarched twin-strand helix of neon spheres with gold connector rungs. Studio lighting.",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "helixR",     "LABEL": "Helix Radius", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.2,  "MAX": 1.2  },
    { "NAME": "pitch",      "LABEL": "Pitch",        "TYPE": "float", "DEFAULT": 0.9,  "MIN": 0.3,  "MAX": 2.5  },
    { "NAME": "sphereR",    "LABEL": "Sphere Size",  "TYPE": "float", "DEFAULT": 0.11, "MIN": 0.04, "MAX": 0.26 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0  },
    { "NAME": "audioPulse", "LABEL": "Audio Pulse",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 2.0  }
  ]
}*/

float sdCap(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p-a, ba = b-a;
    float h = clamp(dot(pa,ba)/dot(ba,ba), 0.0, 1.0);
    return length(pa - ba*h) - r;
}

// Nearest sphere on one helix strand — samples 7 turns around p.y
float helixStrand(vec3 p, float phaseOff) {
    // Helix parametric: pos(t) = (helixR*cos(t+phase), t/pitch, helixR*sin(t+phase))
    // Approximation: t ~ p.y * pitch, then sweep ±3 turns
    float t0  = p.y * pitch;
    float sr  = sphereR * (1.0 + audioBass * audioPulse * 0.12);
    float minD = 1e9;
    for (int k = -3; k <= 3; k++) {
        float t  = t0 + float(k) * 6.2832;
        vec3  sc = vec3(helixR * cos(t + phaseOff),
                        t / pitch,
                        helixR * sin(t + phaseOff));
        minD = min(minD, length(p - sc) - sr);
    }
    return minD;
}

// Rungs every half-turn connecting the two strands
float helixRungs(vec3 p) {
    float t0   = p.y * pitch;
    float minD = 1e9;
    for (int k = -4; k <= 4; k++) {
        float t  = (floor(t0 / 3.1416) + float(k)) * 3.1416;
        float yk = t / pitch;
        if (abs(p.y - yk) > 0.55) continue;
        // Rung from strand 1 to strand 2
        vec3 a = vec3(helixR * cos(t),           yk, helixR * sin(t));
        vec3 b = vec3(helixR * cos(t + 3.1416),  yk, helixR * sin(t + 3.1416));
        minD = min(minD, sdCap(p, a, b, 0.033));
    }
    return minD;
}

// Returns (dist, matID): 0=strand1 cyan, 1=strand2 magenta, 0.5=rung gold
vec2 scene(vec3 wp) {
    // Spin the entire helix slowly
    float ang = TIME * 0.35;
    float ca = cos(ang); float sa = sin(ang);
    vec3 p = vec3(ca*wp.x + sa*wp.z, wp.y, -sa*wp.x + ca*wp.z);

    float s1 = helixStrand(p, 0.0);
    float s2 = helixStrand(p, 3.1416);
    float ru = helixRungs(p);

    float d = min(s1, min(s2, ru));
    float matId = 0.0;
    if (s2 < s1 && s2 < ru) matId = 1.0;
    if (ru < s1 && ru < s2) matId = 0.5;
    return vec2(d, matId);
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        scene(p+e.xyy).x - scene(p-e.xyy).x,
        scene(p+e.yxy).x - scene(p-e.yxy).x,
        scene(p+e.yyx).x - scene(p-e.yyx).x
    ));
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Orbiting camera with vertical oscillation
    float camA = TIME * 0.14;
    vec3 ro = vec3(sin(camA)*3.3, sin(TIME*0.12)*1.6, cos(camA)*3.3);
    vec3 ta = vec3(0.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x*ri + uv.y*up + 1.7*fw);

    vec3 bg = vec3(0.0, 0.005, 0.02);

    float t = 0.05; float mid = -1.0;
    for (int i = 0; i < 64; i++) {
        vec2 res = scene(ro + rd*t);
        if (res.x < 0.002) { mid = res.y; break; }
        if (t > 10.0) break;
        t += res.x * 0.7;
    }

    vec3 col = bg;
    if (mid >= 0.0) {
        vec3 p  = ro + rd*t;
        vec3 n  = getNormal(p);
        vec3 L  = normalize(vec3(0.6, 1.0, 0.4));
        vec3 L2 = normalize(vec3(-0.3, 0.5, -0.7));

        // Strand 1: electric cyan, Strand 2: hot magenta, Rungs: gold
        vec3 basecol;
        if      (mid < 0.3) basecol = vec3(0.0,  1.0,  0.9);  // cyan
        else if (mid < 0.7) basecol = vec3(1.0,  0.75, 0.0);  // gold
        else                basecol = vec3(1.0,  0.0,  0.7);  // magenta

        float diff = max(dot(n,L),0.0)*0.6 + max(dot(n,L2),0.0)*0.2 + 0.2;
        float spec = pow(max(dot(reflect(-L,n),-rd),0.0), 32.0);
        float rim  = pow(clamp(1.0-dot(n,-rd),0.0,1.0), 3.0);

        col  = basecol * diff * hdrPeak;
        col += vec3(1.0) * spec * hdrPeak;      // white HDR spec
        col += basecol   * rim  * hdrPeak;      // colored HDR rim
    }

    gl_FragColor = vec4(col, 1.0);
}
