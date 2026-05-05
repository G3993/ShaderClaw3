/*{
    "DESCRIPTION": "Holographic Torus — 3D raymarched holographic torus in void space with interference fringe patterns and scanline holography. Cool blue/cyan/teal palette. Audio modulates torus thickness and interference frequency.",
    "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
    "CREDIT": "ShaderClaw",
    "INPUTS": [
        { "NAME": "torusR",      "LABEL": "Torus Radius",    "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.3,  "MAX": 1.5  },
        { "NAME": "torusTube",   "LABEL": "Tube Thickness",  "TYPE": "float", "DEFAULT": 0.28, "MIN": 0.05, "MAX": 0.5  },
        { "NAME": "hdrPeak",     "LABEL": "HDR Peak",        "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0,  "MAX": 5.0  },
        { "NAME": "fringeFreq",  "LABEL": "Fringe Freq",     "TYPE": "float", "DEFAULT": 8.0,  "MIN": 2.0,  "MAX": 20.0 },
        { "NAME": "rotSpeed",    "LABEL": "Rotation Speed",  "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 1.5  },
        { "NAME": "audioReact",  "LABEL": "Audio React",     "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0,  "MAX": 2.0  }
    ]
}*/

// Holographic palette: 4 colors — void black / teal / electric cyan / icy white-blue
// NO vaporwave colors — this is cold holographic
vec3 holoPal(float t) {
    t = fract(t);
    if (t < 0.25) return mix(vec3(0.0,0.6,0.7),  vec3(0.0,1.0,0.9),  t*4.0);
    if (t < 0.50) return mix(vec3(0.0,1.0,0.9),  vec3(0.3,0.7,1.0),  (t-0.25)*4.0);
    if (t < 0.75) return mix(vec3(0.3,0.7,1.0),  vec3(0.5,0.0,1.0),  (t-0.50)*4.0);
    return mix(vec3(0.5,0.0,1.0), vec3(0.0,0.6,0.7), (t-0.75)*4.0);
}

float sdTorus(vec3 p, float R, float r) {
    vec2 q = vec2(length(p.xz) - R, p.y);
    return length(q) - r;
}

vec3 calcNormal(vec3 p, float t, float R, float r) {
    vec2 e = vec2(0.001, 0.0);
    float d0 = sdTorus(p, R, r);
    return normalize(vec3(
        sdTorus(p+e.xyy,R,r)-d0,
        sdTorus(p+e.yxy,R,r)-d0,
        sdTorus(p+e.yyx,R,r)-d0));
}

mat3 rotY(float a) { float c=cos(a),s=sin(a); return mat3(c,0,s,0,1,0,-s,0,c); }
mat3 rotX(float a) { float c=cos(a),s=sin(a); return mat3(1,0,0,0,c,-s,0,s,c); }

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = TIME;
    float audio = 1.0 + audioLevel * audioReact * 0.4 + audioBass * audioReact * 0.3;

    // Stable camera, torus rotates
    vec3 ro = vec3(0.0, 0.3, 3.5);
    vec3 rd = normalize(vec3(uv, -2.2));

    float R = torusR;
    float r = torusTube * audio;

    // Torus rotation
    mat3 rot = rotY(t * rotSpeed) * rotX(t * rotSpeed * 0.37);

    // Deep space + holographic grid background
    vec3 col = vec3(0.0, 0.0, 0.015);
    // Background holographic grid lines
    vec2 gridUV = uv * 8.0;
    float gx = abs(fract(gridUV.x) - 0.5);
    float gy = abs(fract(gridUV.y) - 0.5);
    float grid = smoothstep(0.45, 0.48, max(gx, gy));
    col += vec3(0.0, 0.3, 0.4) * grid * 0.08;

    float dm = 0.01;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dm;
        vec3 pr = rot * p;
        float d = sdTorus(pr, R, r);
        if (d < 0.002) {
            vec3 N = rot * calcNormal(pr, t, R, r);
            // Invert rotation for world-space normal
            N = transpose(rot) * calcNormal(pr, t, R, r);

            // Holographic fringe interference on surface
            // Fringes based on world-space Y position + audio modulation
            float audio2 = 1.0 + audioHigh * audioReact * 0.5;
            float fringe = sin(p.y * fringeFreq * audio2 + t * 3.0) * 0.5 + 0.5;
            float fringe2 = sin(length(p.xz) * fringeFreq * 0.7 + t * 2.1) * 0.5 + 0.5;
            float interf = fringe * fringe2;

            // Holographic scanlines (thin horizontal bands)
            float scan = 0.75 + 0.25 * sin(p.y * 40.0 + t * 10.0);

            // Color from holo palette driven by interference
            vec3 holoCol = holoPal(interf + t * 0.05) * hdrPeak;

            // Lighting
            vec3 light = normalize(vec3(0.5, 0.8, 1.0));
            float diff = max(dot(N, light), 0.05);
            float spec = pow(max(dot(reflect(-light, N), -rd), 0.0), 20.0);

            // Rim (bright glow at silhouette edge — hologram boundary glow)
            float rim = pow(1.0 - max(dot(N, -rd), 0.0), 3.0);

            // fwidth black ink edge
            float dotNV = dot(N, -rd);
            float edgeW = fwidth(dotNV);
            float edge = 1.0 - smoothstep(-edgeW, 0.12+edgeW, dotNV);

            col = holoCol * diff * scan
                + vec3(0.3, 1.0, 1.0) * spec * 3.0
                + vec3(0.0, 0.8, 1.0) * rim * 2.5;
            col *= 1.0 - edge * 0.88;
            break;
        }
        if (dm > 8.0) break;
        dm += max(d * 0.9, 0.005);
    }

    gl_FragColor = vec4(col, 1.0);
}
