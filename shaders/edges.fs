/*{
    "DESCRIPTION": "Neon Grid Network — 3D raymarched SDF lattice of glowing neon tubes connecting pulsing node spheres. Orbiting camera. Audio pulses node size and tube brightness.",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "CREDIT": "ShaderClaw",
    "INPUTS": [
        { "NAME": "tubeRadius",  "LABEL": "Tube Radius",  "TYPE": "float", "DEFAULT": 0.040, "MIN": 0.01,  "MAX": 0.10  },
        { "NAME": "nodeRadius",  "LABEL": "Node Size",    "TYPE": "float", "DEFAULT": 0.090, "MIN": 0.02,  "MAX": 0.20  },
        { "NAME": "camSpeed",    "LABEL": "Orbit Speed",  "TYPE": "float", "DEFAULT": 0.25,  "MIN": 0.0,   "MAX": 1.0   },
        { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.8,   "MIN": 1.0,   "MAX": 5.0   },
        { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 0.8,   "MIN": 0.0,   "MAX": 2.0   }
    ]
}*/

// 4-color neon: cyan / magenta / gold / violet
vec3 neonPal(float t) {
    t = fract(t);
    if (t < 0.25) return mix(vec3(0.0,1.0,1.0),  vec3(1.0,0.0,1.0),  t*4.0);
    if (t < 0.50) return mix(vec3(1.0,0.0,1.0),  vec3(1.0,0.85,0.0), (t-0.25)*4.0);
    if (t < 0.75) return mix(vec3(1.0,0.85,0.0), vec3(0.5,0.0,1.0),  (t-0.50)*4.0);
    return mix(vec3(0.5,0.0,1.0), vec3(0.0,1.0,1.0), (t-0.75)*4.0);
}

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }
float hash13(vec3 p)  { return fract(sin(dot(p,vec3(127.1,311.7,74.7)))*43758.5453); }

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 ab = b-a, ap = p-a;
    float t = clamp(dot(ap,ab)/dot(ab,ab), 0.0, 1.0);
    return length(ap - ab*t) - r;
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

float map(vec3 p, float t) {
    float audio = 1.0 + audioLevel * audioReact * 0.4 + audioBass * audioReact * 0.5;
    float tr = tubeRadius;
    float nr = nodeRadius * audio;

    // Tiled lattice — 1-unit spacing
    vec3 cell = floor(p + 0.5);
    vec3 local = p - cell;

    // Three axis tubes
    float dx = sdCapsule(local, vec3(-0.5,0,0), vec3(0.5,0,0), tr);
    float dy = sdCapsule(local, vec3(0,-0.5,0), vec3(0,0.5,0), tr);
    float dz = sdCapsule(local, vec3(0,0,-0.5), vec3(0,0,0.5), tr);
    float tubes = min(dx, min(dy, dz));

    // Pulsing node at lattice vertex
    float pulse = 0.2 * sin(t * 3.1 + hash13(cell) * 6.28);
    float node = sdSphere(local, nr * (1.0 + pulse));

    return min(tubes, node);
}

vec3 calcNormal(vec3 p, float t) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p+e.xyy,t)-map(p-e.xyy,t),
        map(p+e.yxy,t)-map(p-e.yxy,t),
        map(p+e.yyx,t)-map(p-e.yyx,t)));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;

    // Orbiting camera
    float angle = t * camSpeed;
    vec3 ro = vec3(cos(angle)*3.2, 1.5 + sin(t*0.27)*0.5, sin(angle)*3.2);
    vec3 fw = normalize(vec3(0) - ro);
    vec3 rg = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(rg, fw);
    vec3 rd = normalize(fw + uv.x*rg + uv.y*up);

    vec3 col = vec3(0.0, 0.0, 0.012); // void black background
    float dm = 0.01;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dm;
        float d = map(p, t);
        if (d < 0.002) {
            vec3 N = calcNormal(p, t);
            vec3 cell = floor(p + 0.5);

            // Color by lattice cell hash + slow time drift
            float hue = hash13(cell) + t * 0.04;
            vec3 neon = neonPal(hue) * hdrPeak;

            vec3 light = normalize(vec3(1.0, 2.0, 1.5));
            float diff = max(dot(N, light), 0.0);
            float spec = pow(max(dot(reflect(-light,N), -rd), 0.0), 18.0);

            // fwidth black ink edge
            float dotNV = dot(N, -rd);
            float edgeW = fwidth(dotNV);
            float edge = 1.0 - smoothstep(-edgeW, 0.13+edgeW, dotNV);

            col = neon * (0.15 + diff * 0.85) + vec3(1.0)*spec*2.5;
            col *= 1.0 - edge * 0.9;
            break;
        }
        if (dm > 10.0) break;
        dm += max(d * 0.85, 0.005);
    }

    gl_FragColor = vec4(col, 1.0);
}
