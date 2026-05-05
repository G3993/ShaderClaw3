/*{
    "DESCRIPTION": "Cathedral Stone — 3D raymarched stone corridor with vaulted arches. Warm torchlight key, cool fill. Sandstone / mortar / deep shadow palette. 64-step SDF march.",
    "CATEGORIES": ["Generator", "3D", "Architecture", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "corridorLen", "TYPE": "float", "DEFAULT": 6.0,  "MIN": 2.0, "MAX": 12.0, "LABEL": "Corridor Depth" },
        { "NAME": "archFreq",    "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.5, "MAX": 5.0,  "LABEL": "Arch Frequency" },
        { "NAME": "hdrPeak",     "TYPE": "float", "DEFAULT": 2.5,  "MIN": 1.0, "MAX": 4.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",    "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

// Box SDF
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

// Cylinder (infinite Y axis)
float sdCylY(vec2 xz, float r) { return length(xz) - r; }

// Stone-block procedural bump texture
float stoneBump(vec3 p) {
    vec2 cell = floor(p.xz * 4.0 + vec2(p.y * 0.5));
    return hash21(cell) * 0.04 - 0.02;
}

// Scene SDF: tunnel + arches + floor
float sceneSDF(vec3 p) {
    // Tunnel walls (box subtract)
    float wall = sdBox(p, vec3(1.4, 2.5, corridorLen));
    float tunnel = -sdBox(p, vec3(1.0, 2.0, corridorLen + 0.5)); // hollow interior
    float room = max(wall, tunnel);

    // Vaulted arch columns at regular intervals
    float pz = mod(p.z + 1.0, archFreq) - archFreq * 0.5;
    float arch = sdBox(vec3(abs(p.x) - 1.1, p.y - 1.2, pz), vec3(0.12, 1.5, 0.12));

    // Floor plane
    float floor_ = p.y + 2.0;

    float scene = min(arch, floor_);
    scene = min(scene, wall);

    // Stone bump on walls
    scene += stoneBump(p) * 0.5;
    return scene;
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneSDF(p+e.xyy)-sceneSDF(p-e.xyy),
        sceneSDF(p+e.yxy)-sceneSDF(p-e.yxy),
        sceneSDF(p+e.yyx)-sceneSDF(p-e.yyx)
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.4;

    // Camera walking forward down corridor
    float camZ = mod(t * 0.5, corridorLen * 2.0) - corridorLen;
    vec3 ro = vec3(0.0, 0.2, camZ - corridorLen * 0.4);
    vec3 fw = vec3(0.0, 0.0, 1.0);
    vec3 rgt = vec3(1.0, 0.0, 0.0);
    vec3 up  = vec3(0.0, 1.0, 0.0);
    vec3 rd  = normalize(fw + uv.x * rgt * 0.7 + uv.y * up * 0.6);

    float dist = 0.0;
    bool  hit  = false;
    for (int i = 0; i < 64; i++) {
        float d = sceneSDF(ro + rd * dist);
        if (d < 0.002) { hit = true; break; }
        dist += d * 0.9;
        if (dist > 20.0) break;
    }

    // Background: deep void black
    vec3 col = vec3(0.0, 0.0, 0.005);

    if (hit) {
        vec3 p = ro + rd * dist;
        vec3 N = calcNormal(p);

        // Sandstone color with procedural variation
        vec2 bCell = floor(p.xz * 3.5 + vec2(p.y * 0.6));
        float var = hash21(bCell) * 0.25;
        vec3 stone = vec3(0.72 + var, 0.52 + var*0.7, 0.3 + var*0.4); // sandstone range

        // Torch light: warm orange, flickering with TIME
        float flicker = 0.9 + 0.1 * sin(t * 7.3 + hash11(floor(p.z)) * 6.28);
        vec3 torchPos = vec3(0.0, 1.5, ro.z + 0.5); // torch near camera
        vec3 toTorch  = normalize(torchPos - p);
        float torchD  = max(dot(N, toTorch), 0.0);
        float torchAtten = 1.0 / (1.0 + length(torchPos - p) * 0.8);
        vec3  torchCol = vec3(1.0, 0.55, 0.1) * torchD * torchAtten * flicker * 3.0;

        // Cool fill from above
        float fill = max(dot(N, normalize(vec3(0.0, 1.0, 0.3))), 0.0) * 0.15;
        vec3 fillCol = vec3(0.3, 0.45, 0.8) * fill;

        // fwidth AA on stone edges
        float edgeAA = fwidth(dist);
        float inkLine = smoothstep(0.0, edgeAA * 3.0, abs(sceneSDF(p)));

        col = stone * (torchCol + fillCol) * hdrPeak * audio;
        col += vec3(1.0, 0.7, 0.3) * pow(torchD * torchAtten, 4.0) * hdrPeak; // specular

        // Dark mortar in block crevices
        float mortarH = hash21(floor(p.xz * 4.0)) * 0.015;
        float mortar = smoothstep(0.12, 0.0, fract(p.x * 4.0) - mortarH)
                     + smoothstep(0.12, 0.0, fract(p.y * 3.5) - mortarH);
        col = mix(col, vec3(0.02, 0.02, 0.02), clamp(mortar, 0.0, 1.0) * 0.8);
    }

    gl_FragColor = vec4(col, 1.0);
}
