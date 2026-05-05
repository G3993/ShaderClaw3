/*{
    "DESCRIPTION": "Neon Rain City — 3D raymarched cyberpunk city grid at night with falling rain. Neon signs, wet reflective ground. Colors: hot magenta, neon cyan, amber, deep black. 64-step SDF march.",
    "CATEGORIES": ["Generator", "3D", "Cyberpunk", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "cityDensity", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 6.0,  "LABEL": "City Density" },
        { "NAME": "rainSpeed",   "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.5, "MAX": 8.0,  "LABEL": "Rain Speed" },
        { "NAME": "hdrPeak",     "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 5.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",    "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash11(float n) { return fract(sin(n*127.1)*43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

vec3 neonColor(vec2 bid) {
    float h = hash21(bid);
    if (h < 0.33) return vec3(1.0, 0.05, 0.8);
    if (h < 0.66) return vec3(0.0, 0.9,  1.0);
    return             vec3(1.0, 0.55, 0.1);
}

float cityScene(vec3 p) {
    float cellSize = 1.0 / cityDensity;
    vec2 cell = floor(p.xz / cellSize);
    vec2 fp   = fract(p.xz / cellSize) - 0.5;
    float seed = hash21(cell);
    float bH  = 0.4 + seed * 1.6;
    float bW  = 0.1 + seed * 0.12;
    float build  = sdBox(vec3(fp.x, p.y - bH*0.5, fp.y), vec3(bW, bH*0.5, bW));
    float ground = p.y + 0.001;
    return min(build, ground);
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        cityScene(p+e.xyy)-cityScene(p-e.xyy),
        cityScene(p+e.yxy)-cityScene(p-e.yxy),
        cityScene(p+e.yyx)-cityScene(p-e.yyx)
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.6;

    vec3 ro = vec3(0.0, 0.35, -2.5 + t * 0.4);
    vec3 fw = normalize(vec3(0.0, -0.05, 1.0));
    vec3 rd = normalize(fw + uv.x * vec3(1,0,0) * 0.75 + uv.y * vec3(0,1,0) * 0.75);

    float dist = 0.0;
    bool hit = false;
    bool isGround = false;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dist;
        float d = cityScene(p);
        if (d < 0.002) {
            hit = true;
            isGround = (p.y < 0.01);
            break;
        }
        dist += d;
        if (dist > 18.0) break;
    }

    vec3 col = vec3(0.0, 0.0, 0.02);

    if (hit) {
        vec3 p = ro + rd * dist;
        vec3 N = calcNormal(p);
        float cellSize = 1.0 / cityDensity;
        vec2 cell = floor(p.xz / cellSize);
        vec3 neon = neonColor(cell);

        if (isGround) {
            float ripple = sin(length(p.xz) * 18.0 - t * 3.0) * 0.5 + 0.5;
            col = vec3(0.01, 0.01, 0.03);
            col += neon * 0.6 * (0.4 + ripple * 0.6) * hdrPeak * audio;
            col += neonColor(cell + vec2(0.5,0.7)) * 0.3 * hdrPeak;
        } else {
            float kD = max(dot(N, normalize(vec3(-0.3,1.0,-0.5))), 0.0);
            float signY  = fract(p.y * 3.0);
            float isSign = step(0.6, hash21(cell + floor(p.y*3.0)*vec2(0.1,0.1)))
                         * step(0.1, signY) * step(signY, 0.35);
            float pulse  = 0.7 + 0.3 * sin(t * 4.0 + hash21(cell) * 6.28);
            col  = vec3(0.06, 0.06, 0.08) * (kD + 0.08);
            col += neon * isSign * pulse * hdrPeak * audio;
        }
    }

    float fog = exp(-dist * 0.06);
    col = col * fog + vec3(0.0, 0.0, 0.04) * (1.0 - fog);

    // Rain streaks
    float rainSeed  = floor(uv.x * 55.0 + 0.5);
    float rainPhase = fract(uv.y * 0.6 + t * rainSpeed * 0.2 + hash11(rainSeed) * 0.8);
    float rainW     = abs(fract(uv.x * 55.0) - 0.5);
    float rainLine  = 1.0 - smoothstep(0.0, fwidth(rainW), rainW - 0.47);
    float rainFade  = rainPhase * (1.0 - rainPhase) * 4.0;
    col += vec3(0.5, 0.75, 0.9) * rainLine * rainFade * 0.35 * audio;

    gl_FragColor = vec4(col, 1.0);
}
