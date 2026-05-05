/*{
    "DESCRIPTION": "Cyber City — 3D raymarched SDF city skyline with neon-lit building grid. Procedural buildings with glowing window arrays and animated neon signs. Camera pans street-level. Audio pulses building heights.",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "CREDIT": "ShaderClaw",
    "INPUTS": [
        { "NAME": "buildingDensity","LABEL": "Density",    "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.2,  "MAX": 2.0  },
        { "NAME": "neonIntensity",  "LABEL": "Neon HDR",   "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0,  "MAX": 5.0  },
        { "NAME": "camSpeed",       "LABEL": "Cam Speed",  "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 1.0  },
        { "NAME": "audioReact",     "LABEL": "Audio React","TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0  }
    ]
}*/

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// 4-color neon palette: hot pink / electric cyan / toxic yellow-green / violet
vec3 neonSign(float t) {
    t = fract(t);
    if (t < 0.25) return vec3(1.0, 0.05, 0.6);   // hot pink
    if (t < 0.50) return vec3(0.0, 1.0, 0.9);    // electric cyan
    if (t < 0.75) return vec3(0.6, 1.0, 0.0);    // toxic yellow-green
    return vec3(0.5, 0.1, 1.0);                   // violet
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

// Building SDF
struct Building {
    float dist;
    vec2 cellId;
    float height;
};

Building buildingMap(vec3 p, float t) {
    float audio = 1.0 + audioBass * audioReact * 0.3;
    float spacing = buildingDensity;

    vec2 cellF = floor(p.xz / spacing);
    vec2 localXZ = fract(p.xz / spacing) - 0.5;

    float h11 = hash21(cellF);
    float h12 = hash21(cellF + vec2(1,0));
    float bWidth = 0.25 + h11 * 0.2;
    float bDepth = 0.25 + h12 * 0.2;
    float bHeight = (0.5 + h11 * 2.5) * audio;

    // Building box
    float d = sdBox(vec3(localXZ.x, p.y - bHeight, localXZ.y),
                    vec3(bWidth, bHeight, bDepth));

    Building b;
    b.dist = d;
    b.cellId = cellF;
    b.height = bHeight;
    return b;
}

float mapDist(vec3 p, float t) {
    Building b = buildingMap(p, t);
    float dFloor = p.y; // ground plane at y=0
    return min(b.dist, dFloor);
}

vec3 calcNormal(vec3 p, float t) {
    vec2 e = vec2(0.002, 0.0);
    return normalize(vec3(
        mapDist(p+e.xyy,t)-mapDist(p-e.xyy,t),
        mapDist(p+e.yxy,t)-mapDist(p-e.yxy,t),
        mapDist(p+e.yyx,t)-mapDist(p-e.yyx,t)));
}

// Window glow: returns neon color if pixel is on a window
vec3 windowGlow(vec3 p, vec2 cellId, float bHeight, float t) {
    float spacing = buildingDensity;
    vec2 localXZ = fract(p.xz / spacing) - 0.5;

    float winX = fract(localXZ.x * 6.0);
    float winY = fract(p.y * 2.5);
    bool isWin = (winX > 0.15 && winX < 0.85) && (winY > 0.15 && winY < 0.75);

    if (!isWin) return vec3(0.0);

    // Per-window hue
    vec2 winId = vec2(floor(localXZ.x * 6.0), floor(p.y * 2.5));
    float h = hash21(cellId * 17.3 + winId);
    float flickerSeed = hash21(cellId + winId + vec2(floor(t * 3.0 + h * 10.0)));
    float lit = step(0.3, flickerSeed); // 70% windows lit

    return neonSign(h) * lit * neonIntensity;
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;

    // Street-level camera moving forward and slightly panning
    float camZ = t * camSpeed;
    float camX = sin(t * 0.13) * 0.5;
    vec3 ro = vec3(camX, 0.8, camZ);
    vec3 lookTarget = ro + vec3(sin(t * 0.07) * 0.3, 0.2, 1.0);
    vec3 fw = normalize(lookTarget - ro);
    vec3 rg = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(rg, fw);
    vec3 rd = normalize(fw + uv.x*rg + uv.y*up);

    // Night sky gradient (deep indigo → black)
    float skyT = clamp(uv.y + 0.5, 0.0, 1.0);
    vec3 col = mix(vec3(0.0), vec3(0.02, 0.02, 0.06), skyT);

    float dm = 0.01;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dm;
        float d = mapDist(p, t);
        if (d < 0.005) {
            vec3 N = calcNormal(p, t);

            Building b = buildingMap(p, t);
            bool isFloor = (p.y < 0.05);

            vec3 buildingColor;
            if (isFloor) {
                // Wet asphalt — dark, slight neon reflection
                buildingColor = vec3(0.04, 0.04, 0.06);
                // Reflective neon puddles
                float reflH = hash21(p.xz * 3.7);
                buildingColor += neonSign(reflH + t * 0.05) * 0.15;
            } else {
                // Building face — near-black concrete
                buildingColor = vec3(0.06, 0.06, 0.08);
                // Windows
                vec3 wins = windowGlow(p, b.cellId, b.height, t);
                buildingColor += wins;
            }

            // Key light (overhead cool-white streetlight)
            vec3 keyLight = normalize(vec3(0.2, 1.0, 0.3));
            float diff = max(dot(N, keyLight), 0.0) * 0.3;

            // fwidth edge
            float dotNV = dot(N, -rd);
            float edgeW = fwidth(dotNV);
            float edge = 1.0 - smoothstep(-edgeW, 0.10+edgeW, dotNV);

            col = buildingColor * (0.1 + diff) * (1.0 - edge * 0.8);
            break;
        }
        if (dm > 20.0) break;
        dm += d * 0.85;
    }

    gl_FragColor = vec4(col, 1.0);
}
