/*{
    "DESCRIPTION": "Aurora Magnetica — 3D volumetric aurora curtains bending in a magnetic field. Wide upward camera. Palette: electric cyan, violet, gold, deep-space navy. 64-step volume march.",
    "CATEGORIES": ["Generator", "3D", "Volumetric", "Audio Reactive"],
    "CREDIT": "ShaderClaw auto-improve",
    "INPUTS": [
        { "NAME": "curtainDensity", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 8.0,  "LABEL": "Curtain Density" },
        { "NAME": "waveSpeed",      "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 2.0,  "LABEL": "Wave Speed" },
        { "NAME": "hdrPeak",        "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 4.0,  "LABEL": "HDR Peak" },
        { "NAME": "audioMod",       "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0,  "LABEL": "Audio Mod" }
    ]
}*/

float hash21(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p); vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash21(i), hash21(i+vec2(1,0)), u.x),
               mix(hash21(i+vec2(0,1)), hash21(i+vec2(1,1)), u.x), u.y);
}

float fbm2(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int k = 0; k < 4; k++) { v += a * vnoise(p); p *= 2.2; a *= 0.5; }
    return v;
}

float auroraDensity(vec3 p, float t) {
    float xWarp = fbm2(vec2(p.x * 0.4 + t * 0.15, p.z * 0.3 + t * 0.08)) * 0.8;
    float zWarp = fbm2(vec2(p.z * 0.35 - t * 0.12, p.x * 0.25 + t * 0.1)) * 0.8;
    float curtain = sin((p.x + xWarp) * curtainDensity + t * waveSpeed)
                  * cos((p.z + zWarp) * curtainDensity * 0.7 - t * waveSpeed * 0.6);
    curtain = curtain * 0.5 + 0.5;
    float hEnv = smoothstep(-0.2, 2.5, p.y) * smoothstep(6.0, 2.0, p.y);
    return curtain * curtain * hEnv * 1.5;
}

vec3 auroraColor(vec3 p, float t) {
    float phase = p.x * 0.3 + p.z * 0.2 + t * 0.2;
    float fi = fract(phase * 0.25);
    if (fi < 0.33) return mix(vec3(0.0, 0.9, 1.0),  vec3(0.5, 0.1, 1.0),  fi * 3.0);
    if (fi < 0.67) return mix(vec3(0.5, 0.1, 1.0),  vec3(1.0, 0.75, 0.0), (fi-0.33)*3.0);
    return             mix(vec3(1.0, 0.75, 0.0), vec3(0.0, 0.9,  1.0), (fi-0.67)*3.0);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME * 0.5;
    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;

    // Camera pointing up into sky, slow drift
    vec3 ro = vec3(sin(TIME*0.07)*0.5, 0.0, cos(TIME*0.05)*0.5);
    vec3 rd = normalize(vec3(uv.x * 0.85, 1.0, uv.y * 0.5 + 0.6));

    // Deep-space navy background
    float skyH = pow(max(rd.y, 0.0), 0.4);
    vec3 bgCol = mix(vec3(0.0, 0.0, 0.02), vec3(0.0, 0.03, 0.14), skyH);

    // Volumetric march through aurora layer
    vec3 accCol   = vec3(0.0);
    float transmit = 1.0;
    float stepLen  = 0.1;

    for (int i = 0; i < 64; i++) {
        float dist = float(i) * stepLen;
        vec3 p = ro + rd * dist;
        if (p.y < -0.3 || p.y > 6.5) continue;
        float dens = auroraDensity(p, t) * stepLen * 1.2;
        if (dens > 0.001) {
            vec3 aCol = auroraColor(p, t) * hdrPeak * audio;
            accCol   += aCol * dens * transmit;
            transmit *= exp(-dens * 0.8);
            if (transmit < 0.01) break;
        }
    }

    vec3 col = bgCol * transmit + accCol;

    // Faint stars
    float star = step(0.983, hash21(floor(uv * 22.0) + 7.3)) * (0.6 + hash21(floor(uv * 22.0)) * 0.4);
    col += vec3(star) * transmit * 1.8;

    gl_FragColor = vec4(col, 1.0);
}
