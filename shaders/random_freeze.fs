/*{
    "DESCRIPTION": "Copper Lattice — 3D raymarched hexagonal prism grid in warm copper/rust/gold palette. Audio modulates prism height. Camera tracks slowly over the hex field.",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "CREDIT": "ShaderClaw",
    "INPUTS": [
        { "NAME": "hexSize",    "LABEL": "Hex Size",    "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.1,  "MAX": 1.5  },
        { "NAME": "prismHeight","LABEL": "Height",      "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.1,  "MAX": 2.0  },
        { "NAME": "hdrPeak",    "LABEL": "HDR Peak",    "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0,  "MAX": 4.0  },
        { "NAME": "camTilt",    "LABEL": "Cam Tilt",    "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.1,  "MAX": 1.0  },
        { "NAME": "audioReact", "LABEL": "Audio React", "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0  }
    ]
}*/

// 4-color copper palette: deep rust, copper orange, warm gold, hot brass
vec3 copperPal(float t) {
    t = clamp(t, 0.0, 1.0);
    if (t < 0.33) return mix(vec3(0.25,0.04,0.01), vec3(0.80,0.32,0.08), t*3.0);
    if (t < 0.66) return mix(vec3(0.80,0.32,0.08), vec3(1.00,0.75,0.15), (t-0.33)*3.0);
    return mix(vec3(1.00,0.75,0.15), vec3(1.0,0.95,0.55), (t-0.66)*3.0);
}

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// Hexagonal SDF (2D, regular hexagon)
float sdHex(vec2 p, float r) {
    vec2 q = abs(p);
    return max(q.x*0.866025 + q.y*0.5, q.y) - r;
}

// Hexagonal tiling coordinates
vec2 hexTile(vec2 p, float s) {
    vec2 a = mod(p, vec2(s, s*1.732)) - vec2(s, s*0.866);
    vec2 b = mod(p + vec2(s*0.5, s*0.866), vec2(s, s*1.732)) - vec2(s, s*0.866);
    return dot(a,a) < dot(b,b) ? a : b;
}

float sdPrism(vec3 p, float hexR, float h) {
    float d2d = sdHex(p.xz, hexR);
    return max(d2d, abs(p.y) - h);
}

float map(vec3 p, float t) {
    float audio = 1.0 + audioLevel * audioReact * 0.4 + audioBass * audioReact * 0.5;
    float spacing = hexSize * 2.0;

    // Tile in X and Z with hex pattern
    vec2 tileCoord = vec2(p.x, p.z);
    vec2 local2D = hexTile(tileCoord, spacing);

    // Per-cell height variation driven by hash + time
    vec2 cellId = (tileCoord - local2D) / spacing;
    float cellH = hash21(cellId);
    float h = prismHeight * (0.4 + cellH * 0.6) * audio
             * (1.0 + 0.2 * sin(t * 2.0 + cellH * 6.28));

    float prismR = hexSize * 0.92;
    float dPrism = sdPrism(vec3(local2D.x, p.y, local2D.y), prismR, h);
    float dFloor = p.y + h + 0.02; // floor plane below prisms
    return min(dPrism, dFloor);
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
    float audio = 1.0 + audioLevel * audioReact * 0.3;

    // Camera above the lattice, tracking slowly
    float camX = t * 0.3;
    vec3 ro = vec3(camX, 2.5 * camTilt + sin(t*0.17)*0.3, t * 0.15);
    vec3 target = ro + vec3(0.0, -1.0, 0.5);
    vec3 fw = normalize(target - ro);
    vec3 rg = normalize(cross(fw, vec3(0,1,0)));
    vec3 up = cross(rg, fw);
    vec3 rd = normalize(fw + uv.x*rg + uv.y*up);

    vec3 col = vec3(0.03, 0.01, 0.005); // dark forge-room background
    float dm = 0.01;

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dm;
        float d = map(p, t);
        if (d < 0.002) {
            vec3 N = calcNormal(p, t);

            // Color by height and cell hash
            float cellHeight = p.y;
            float colT = clamp((cellHeight + prismHeight) / (prismHeight * 2.0), 0.0, 1.0);
            vec3 base = copperPal(colT) * hdrPeak;

            // Forge/foundry lighting: hot overhead + cool ambient
            vec3 keyLight = normalize(vec3(0.3, 1.0, 0.5));
            float diff = max(dot(N, keyLight), 0.05);
            float spec = pow(max(dot(reflect(-keyLight, N), -rd), 0.0), 30.0);

            // Emissive glow from top faces (hot metal)
            float topFace = max(N.y, 0.0);
            vec3 emissive = copperPal(0.95) * topFace * 1.5;

            // Black ink edge
            float dotNV = dot(N, -rd);
            float edgeW = fwidth(dotNV);
            float edge = 1.0 - smoothstep(-edgeW, 0.12+edgeW, dotNV);

            col = base * (0.1 + diff * 0.9) + vec3(1.0,0.9,0.7)*spec*2.5 + emissive;
            col *= 1.0 - edge * 0.9;
            break;
        }
        if (dm > 12.0) break;
        dm += d * 0.85;
    }

    gl_FragColor = vec4(col, 1.0);
}
