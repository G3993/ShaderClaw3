/*{
  "CATEGORIES": ["Generator", "Audio Reactive", "Geometric", "3D"],
  "DESCRIPTION": "Chladni Figures 3D — sand on a vibrating plate, raymarched with bump-mapped nodal ridges and Blinn-Phong lighting. Audio frequencies sculpt the standing wave modes in real time.",
  "INPUTS": [
    {"NAME":"baseN","LABEL":"Mode N","TYPE":"float","MIN":1.0,"MAX":12.0,"DEFAULT":3.0},
    {"NAME":"baseM","LABEL":"Mode M","TYPE":"float","MIN":1.0,"MAX":12.0,"DEFAULT":5.0},
    {"NAME":"audioModeRange","LABEL":"Audio Mode Range","TYPE":"float","MIN":0.0,"MAX":8.0,"DEFAULT":4.0},
    {"NAME":"lineSharpness","LABEL":"Line Sharpness","TYPE":"float","MIN":0.001,"MAX":0.1,"DEFAULT":0.02},
    {"NAME":"jitter","LABEL":"Sand Grain","TYPE":"float","MIN":0.0,"MAX":0.05,"DEFAULT":0.005},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0},
    {"NAME":"sandColor","LABEL":"Sand","TYPE":"color","DEFAULT":[0.95,0.88,0.7,1.0]},
    {"NAME":"plateColor","LABEL":"Plate","TYPE":"color","DEFAULT":[0.06,0.05,0.05,1.0]},
    {"NAME":"inputTex","LABEL":"Plate Texture","TYPE":"image"}
  ]
}*/

#define PI 3.14159265

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float sdBox(vec3 p, vec3 b) {
    vec3 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
}

float chladniF(vec2 q, float n, float m) {
    return sin(n * PI * q.x) * sin(m * PI * q.y)
         - sin(m * PI * q.x) * sin(n * PI * q.y);
}

float sandH(vec2 q, float n, float m, float sharpness) {
    return smoothstep(sharpness * 2.0, 0.0, abs(chladniF(q, n, m)));
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    float bass = 0.5 + 0.5 * audioBass * audioReact;
    float lvl  = 0.5 + 0.5 * audioLevel * audioReact;

    float n = baseN + audioBass * audioModeRange + sin(TIME * 0.05) * 0.5;
    float m = baseM + audioHigh * audioModeRange + cos(TIME * 0.07) * 0.5;
    if (abs(n - m) < 0.5) m += 1.0;

    // Camera — slow orbit, looking down at the plate
    float camA = TIME * 0.12;
    vec3 ro = vec3(sin(camA) * 2.0, 1.8, cos(camA) * 2.0);
    vec3 fwd = normalize(-ro);
    vec3 right = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(right, fwd);
    vec2 ndc = (uv - 0.5) * vec2(aspect, 1.0);
    vec3 rd = normalize(fwd + ndc.x * right + ndc.y * up);

    vec3 plateHalf = vec3(1.0, 0.025, 1.0);
    vec3 col = vec3(0.005, 0.005, 0.01);

    float t = 0.0;
    bool hit = false;
    vec3 hitP;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * t;
        float d = sdBox(p, plateHalf);
        if (d < 0.001) { hitP = p; hit = true; break; }
        if (t > 9.0) break;
        t += max(d * 0.7, 0.002);
    }

    if (hit) {
        vec2 plateUV = hitP.xz * 0.5 + 0.5;

        // Sand grain jitter (audioMid modulates grain)
        float mids = 0.5 + 0.5 * audioMid * audioReact;
        vec2 jUV = plateUV + (vec2(hash(plateUV * 37.1), hash(plateUV * 61.3 + 1.7)) - 0.5)
                           * jitter * (0.8 + mids * 0.5);

        float sandRaw = sandH(jUV, n, m, lineSharpness);
        float sandFw = max(fwidth(sandRaw), 0.005);
        float sandAA = smoothstep(0.5 - sandFw, 0.5 + sandFw, sandRaw);

        // Surface normal via finite differences on SDF
        vec2 ev = vec2(0.001, 0.0);
        vec3 N = normalize(vec3(
            sdBox(hitP + ev.xyy, plateHalf) - sdBox(hitP - ev.xyy, plateHalf),
            sdBox(hitP + ev.yxy, plateHalf) - sdBox(hitP - ev.yxy, plateHalf),
            sdBox(hitP + ev.yyx, plateHalf) - sdBox(hitP - ev.yyx, plateHalf)
        ));

        // Bump map on top face — sand ridges perturb normal
        if (N.y > 0.5) {
            float be = 0.004;
            float hL = sandH(plateUV - vec2(be, 0.0), n, m, lineSharpness);
            float hR = sandH(plateUV + vec2(be, 0.0), n, m, lineSharpness);
            float hD = sandH(plateUV - vec2(0.0, be), n, m, lineSharpness);
            float hU = sandH(plateUV + vec2(0.0, be), n, m, lineSharpness);
            vec3 bumpN = normalize(vec3((hL - hR) * 0.8, 0.06, (hD - hU) * 0.8));
            N = normalize(mix(N, bumpN, sandAA * 0.85));
        }

        // Blinn-Phong lighting
        vec3 L = normalize(vec3(0.6, 1.0, 0.4));
        vec3 V = normalize(-rd);
        vec3 H = normalize(L + V);
        float diff = max(dot(N, L), 0.0);
        float spec = pow(max(dot(N, H), 0.0), 32.0);

        // Plate base — optional texture
        vec3 plateBase = plateColor.rgb;
        if (IMG_SIZE_inputTex.x > 0.0)
            plateBase = mix(plateColor.rgb, texture(inputTex, plateUV).rgb * 0.5, 0.5);

        vec3 matCol = mix(plateBase, sandColor.rgb, sandAA);
        float matSpec = mix(0.30, 0.12, sandAA);

        // Diffuse + HDR specular
        col = matCol * (0.15 + diff * 0.85);
        col += vec3(1.0, 0.97, 0.90) * spec * matSpec * 2.5 * (0.8 + bass * 1.2);

        // Sand ridge emissive glow — HDR when audio is loud
        col += sandColor.rgb * sandAA * 1.2 * (0.5 + lvl);

        // Phantom resonance — higher harmonic every ~30s
        float ph = fract(TIME / 30.0);
        float flashF = smoothstep(0.0, 0.10, ph) * smoothstep(0.40, 0.25, ph);
        float fineF = sin(hitP.x * 60.0) * sin(hitP.z * 60.0)
                    + sin(hitP.x * 47.0 + 1.7) * sin(hitP.z * 47.0 + 0.4);
        float harmonic = smoothstep(lineSharpness * 6.0, 0.0, abs(fineF));
        col += sandColor.rgb * harmonic * flashF * 0.6 * (0.7 + bass * 0.6);

        // Plate edge accent
        float edgeDist = length(max(abs(hitP.xz) - vec2(0.92), 0.0));
        col += plateBase * 0.4 * exp(-edgeDist * 22.0);
    }

    // Vignette
    col *= 1.0 - 0.55 * dot((uv - 0.5), (uv - 0.5)) * 3.0;

    gl_FragColor = vec4(col, 1.0);
}
