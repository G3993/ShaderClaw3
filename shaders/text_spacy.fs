/*{
  "DESCRIPTION": "Infinite Crystal Forest — 3D raymarched infinite grid of glowing crystal columns, fly-through camera with bioluminescent light",
  "CREDIT": "ShaderClaw auto-improve",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    { "NAME": "glowStrength", "LABEL": "Glow",        "TYPE": "float", "DEFAULT": 2.2,  "MIN": 0.5, "MAX": 4.0  },
    { "NAME": "flySpeed",     "LABEL": "Fly Speed",    "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.0, "MAX": 3.0  },
    { "NAME": "crystalDens",  "LABEL": "Density",      "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 5.0  },
    { "NAME": "audioPulse",   "LABEL": "Audio Pulse",  "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0, "MAX": 2.0  },
    { "NAME": "crystalHeight","LABEL": "Height",       "TYPE": "float", "DEFAULT": 2.5,  "MIN": 0.5, "MAX": 5.0  }
  ]
}*/

float hash1(float n) { return fract(sin(n * 127.1) * 43758.5); }
float hash2(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5); }

float sdCap(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p-a, ba = b-a;
    float h = clamp(dot(pa,ba)/dot(ba,ba), 0.0, 1.0);
    return length(pa - ba*h) - r;
}

float sdOct(vec3 p, float r) {
    return (abs(p.x) + abs(p.y) + abs(p.z) - r) * 0.57735;
}

// Returns (dist, cellHash) for the nearest crystal in the XZ grid
vec2 crystalCell(vec3 p) {
    float cellSz = crystalDens;
    vec2 cellId = floor(p.xz / cellSz + 0.5);

    float bestD = 1e9; float bestHash = 0.0;
    // Check 3×3 neighborhood for correct nearest
    for (int cx = -1; cx <= 1; cx++) {
        for (int cz = -1; cz <= 1; cz++) {
            vec2 cid = cellId + vec2(cx, cz);
            float ch = hash2(cid);
            float ch2 = hash2(cid + vec2(17.3, 33.1));

            // Jitter within cell
            vec2 jitter = (vec2(ch, ch2) - 0.5) * 0.6;
            vec3 ctr = vec3((cid.x + jitter.x) * cellSz, 0.0, (cid.y + jitter.y) * cellSz);

            float h = crystalHeight * (0.6 + ch * 0.8);
            float pulse = 1.0 + audioBass * audioPulse * 0.1;
            float r = 0.08 + ch2 * 0.05;

            // Stem
            float stem = sdCap(p - ctr, vec3(0.0), vec3(0.0, h, 0.0), r * pulse);
            // Crystal tip (octahedron)
            float tipSz = 0.22 + ch * 0.18;
            float tip = sdOct(p - (ctr + vec3(0.0, h + tipSz*0.8, 0.0)), tipSz * pulse);

            float di = min(stem, tip);
            if (di < bestD) { bestD = di; bestHash = ch; }
        }
    }
    return vec2(bestD, bestHash);
}

vec2 scene(vec3 p) {
    float floor_ = p.y + 0.05;
    vec2 cr = crystalCell(p);
    float d = min(cr.x, floor_);
    float matId = (d == floor_) ? -0.1 : cr.y;
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

vec3 crystalColor(float h) {
    // Map hash [0,1] to 5-color palette
    float t = h * 5.0;
    int ci = int(floor(t));
    if (ci == 0) return vec3(1.0,  0.0,  0.6);   // magenta
    if (ci == 1) return vec3(1.0,  0.7,  0.0);   // gold
    if (ci == 2) return vec3(0.0,  0.9,  1.0);   // cyan
    if (ci == 3) return vec3(0.0,  1.0,  0.35);  // green
               return vec3(0.6,  0.0,  1.0);   // violet
}

void main() {
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE*0.5) / min(RENDERSIZE.x, RENDERSIZE.y);

    float zOff = TIME * flySpeed;
    float weave = sin(TIME * 0.27) * crystalDens * 0.3;
    vec3 ro = vec3(weave, 0.9, zOff);
    vec3 ta = vec3(weave + sin(TIME*0.11)*0.5, 1.6, zOff + 6.0);
    vec3 fw = normalize(ta - ro);
    vec3 ri = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(ri, fw);
    vec3 rd = normalize(uv.x*ri + uv.y*up + 1.5*fw);

    vec3 bg = mix(vec3(0.0, 0.0, 0.01), vec3(0.0, 0.005, 0.02), uv.y*0.5+0.5);

    float t = 0.05; float mid = -99.0;
    for (int i = 0; i < 72; i++) {
        vec2 res = scene(ro + rd*t);
        if (res.x < 0.003) { mid = res.y; break; }
        if (t > 20.0) break;
        t += res.x * 0.75;
    }

    vec3 col = bg;

    if (mid >= 0.0) {
        vec3 p = ro + rd*t;
        vec3 n = getNormal(p);
        vec3 L  = normalize(vec3(0.3, 1.2, 0.4));
        vec3 basecol = crystalColor(mid);
        float diff = max(dot(n,L),0.0)*0.5 + 0.2;
        float spec = pow(max(dot(reflect(-L,n),-rd),0.0), 36.0);
        float rim  = pow(clamp(1.0-dot(n,-rd),0.0,1.0), 2.8);
        float face = smoothstep(0.0, 0.2, dot(n,-rd));
        col  = basecol * diff * glowStrength * face;
        col += basecol * rim  * glowStrength * 1.6;
        col += vec3(1.0) * spec * glowStrength * 1.2;
        col += basecol * 0.35 * glowStrength;
    } else if (mid > -0.5) {
        // Dark mossy floor
        vec3 p = ro + rd*t;
        float gx = abs(fract(p.x/crystalDens + 0.5) - 0.5);
        float gz = abs(fract(p.z/crystalDens + 0.5) - 0.5);
        float vein = smoothstep(0.48, 0.42, min(gx, gz));
        col = vec3(0.0, 0.012, 0.02) + vec3(0.0, 0.04, 0.06)*vein;
    }

    col = mix(col, bg, clamp((t-12.0)/8.0, 0.0, 1.0)*0.65);
    gl_FragColor = vec4(col, 1.0);
}
