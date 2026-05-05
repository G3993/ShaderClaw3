/*{
    "DESCRIPTION": "Bio Organism — 3D raymarched smooth metaball creature with pulsing bioluminescent lobes. Deep ocean bioluminescent palette: void black / electric blue / cyan-teal / phosphor green. Audio drives lobe pulsing. Camera orbits slowly.",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "CREDIT": "ShaderClaw",
    "INPUTS": [
        { "NAME": "lobeCount",  "LABEL": "Lobe Count",  "TYPE": "float", "DEFAULT": 6.0,  "MIN": 3.0,  "MAX": 10.0 },
        { "NAME": "lobeSize",   "LABEL": "Lobe Size",   "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.1,  "MAX": 0.8  },
        { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0,  "MAX": 5.0  },
        { "NAME": "pulseRate",  "LABEL": "Pulse Rate",  "TYPE": "float", "DEFAULT": 1.2,  "MIN": 0.2,  "MAX": 4.0  },
        { "NAME": "camOrbit",   "LABEL": "Orbit Speed", "TYPE": "float", "DEFAULT": 0.2,  "MIN": 0.0,  "MAX": 1.0  },
        { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.9,  "MIN": 0.0,  "MAX": 2.0  }
    ]
}*/

// 4-color bioluminescent palette: void black / electric blue / cyan-teal / phosphor green
vec3 bioPal(float t) {
    t = fract(t);
    if (t < 0.25) return mix(vec3(0.0,0.2,1.0),  vec3(0.0,0.9,1.0),  t*4.0);
    if (t < 0.50) return mix(vec3(0.0,0.9,1.0),  vec3(0.0,1.0,0.4),  (t-0.25)*4.0);
    if (t < 0.75) return mix(vec3(0.0,1.0,0.4),  vec3(0.3,0.0,1.0),  (t-0.50)*4.0);
    return mix(vec3(0.3,0.0,1.0), vec3(0.0,0.2,1.0), (t-0.75)*4.0);
}

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }

float smin(float a, float b, float k) {
    float h = clamp(0.5+0.5*(b-a)/k, 0.0, 1.0);
    return mix(b, a, h) - k*h*(1.0-h);
}

// Core body = central sphere
float coreBody(vec3 p, float t) {
    float audio = 1.0 + audioBass * audioReact * 0.2;
    float breathe = 0.05 * sin(t * pulseRate * 0.5);
    return length(p) - (0.55 + breathe) * audio;
}

// Lobe position
vec3 lobeCenter(int idx, float t) {
    float fi = float(idx);
    float N = lobeCount;
    float azimuth = fi / N * 6.28318;
    float elevation = (hash11(fi * 3.71) - 0.5) * 1.2;
    float r = 0.65;
    // Pulse orbit radius
    float pulsed = r + 0.1 * sin(t * pulseRate + fi * 1.7);
    return vec3(cos(azimuth)*pulsed*cos(elevation),
                sin(elevation)*pulsed,
                sin(azimuth)*pulsed*cos(elevation));
}

float map(vec3 p, float t) {
    float audio = 1.0 + audioLevel * audioReact * 0.3;
    float d = coreBody(p, t);
    int N = int(clamp(lobeCount, 3.0, 10.0));
    for (int i = 0; i < 10; i++) {
        if (i >= N) break;
        float fi = float(i);
        float ls = lobeSize * (0.6 + hash11(fi*2.3)*0.5) * audio;
        float pulse = 0.12 * sin(t * pulseRate * (0.8 + hash11(fi*4.1)*0.4) + fi * 2.3);
        vec3 center = lobeCenter(i, t);
        float dl = length(p - center) - (ls + pulse);
        d = smin(d, dl, 0.35);
    }
    return d;
}

vec3 calcNormal(vec3 p, float t) {
    vec2 e = vec2(0.003, 0.0);
    return normalize(vec3(
        map(p+e.xyy,t)-map(p-e.xyy,t),
        map(p+e.yxy,t)-map(p-e.yxy,t),
        map(p+e.yyx,t)-map(p-e.yyx,t)));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;

    // Orbit camera
    float angle = t * camOrbit;
    vec3 ro = vec3(cos(angle)*3.2, 0.8 + sin(t*0.3)*0.5, sin(angle)*3.2);
    vec3 fw = normalize(-ro);
    vec3 rg = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(rg, fw);
    vec3 rd = normalize(fw + uv.x*rg + uv.y*up);

    // Deep ocean void background
    vec3 col = vec3(0.0, 0.005, 0.012);
    // Faint particle suspension (random blue-green specks)
    float pSeed = fract(sin(dot(uv*50.0, vec2(127.1,311.7)))*43758.5);
    col += step(0.992, pSeed) * vec3(0.0, 0.3, 0.6) * 0.5;

    float dm = 0.01;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dm;
        float d = map(p, t);
        if (d < 0.004) {
            vec3 N = calcNormal(p, t);

            // Color by surface position (height + azimuth)
            float angle2 = atan(p.x, p.z) / 6.28318 + 0.5;
            float elevation = p.y * 0.5 + 0.5;
            float hue = fract(angle2 * 0.7 + elevation * 0.3 + t * 0.02);
            vec3 bio = bioPal(hue) * hdrPeak;

            // Subsurface scattering approximation (glow from inside)
            float sss = max(0.0, 1.0 - length(p) / 2.0) * 0.5;

            vec3 light = normalize(vec3(0.5, 1.0, 0.5));
            float diff = max(dot(N, light), 0.0);
            float spec = pow(max(dot(reflect(-light,N),-rd),0.0), 20.0);

            // fwidth edge
            float dotNV = dot(N, -rd);
            float edgeW = fwidth(dotNV);
            float edge = 1.0 - smoothstep(-edgeW, 0.12+edgeW, dotNV);

            col = bio * (0.1 + diff*0.7 + sss) + vec3(0.5,1.0,1.0)*spec*3.0;
            col *= 1.0 - edge * 0.9;
            break;
        }
        if (dm > 10.0) break;
        dm += max(d * 0.85, 0.005);
    }

    gl_FragColor = vec4(col, 1.0);
}
