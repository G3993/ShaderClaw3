/*{
  "DESCRIPTION": "Klein Ocean — IKB ultramarine ocean waves with molten gold foam crests. Wide 3D environmental seascape, raymarched FBM water surface.",
  "CREDIT": "ShaderClaw auto-improve v7 — inspired by Yves Klein's IKB monochromes",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "waveHeight",  "LABEL": "Wave Height",  "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "waveSpeed",   "LABEL": "Wave Speed",   "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.0,  "MAX": 2.0 },
    { "NAME": "windDir",     "LABEL": "Wind Angle",   "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 6.28 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0,  "MAX": 4.0 },
    { "NAME": "foamThresh",  "LABEL": "Foam Thresh",  "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "audioMod",    "LABEL": "Audio Mod",    "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 80
#define SURF_DIST  0.005
#define MAX_DIST   20.0

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(hash(i),            hash(i + vec2(1.0, 0.0)), u.x),
               mix(hash(i + vec2(0.0, 1.0)), hash(i + vec2(1.0, 1.0)), u.x), u.y);
}

// FBM ocean surface height
float fbmOcean(vec2 p, float t) {
    vec2 w = vec2(cos(windDir), sin(windDir));
    float h = 0.0;
    float amp = 1.0, freq = 1.0, tot = 0.0;
    for (int i = 0; i < 6; i++) {
        vec2 q = p * freq + w * t * waveSpeed * (0.6 + float(i) * 0.2);
        q += vec2(sin(q.y * 1.3 + t * 0.4), cos(q.x * 1.7 - t * 0.3)) * 0.25;
        h += noise(q) * amp;
        tot += amp;
        amp *= 0.52;
        freq *= 2.07;
    }
    return (h / tot) * waveHeight;
}

float scene(vec3 p) {
    return p.y - fbmOcean(p.xz, TIME);
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.01, 0.0);
    float hC = fbmOcean(p.xz, TIME);
    float hR = fbmOcean(p.xz + e.xy, TIME);
    float hF = fbmOcean(p.xz + e.yx, TIME);
    return normalize(vec3(hC - hR, e.x, hC - hF));
}

void main() {
    vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio = 1.0 + audioLevel * audioMod + audioBass * audioMod * 0.5;
    float t = TIME;

    // Wide environmental camera: slightly above and angled down to horizon
    float camT = t * 0.04;
    vec3 ro = vec3(sin(camT) * 0.8, 1.1, -4.0 + cos(camT * 0.7) * 0.5);
    vec3 target = vec3(sin(camT * 0.5) * 0.4, 0.0, 2.0);
    vec3 fwd = normalize(target - ro);
    vec3 rgt = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 upV = cross(fwd, rgt);
    vec3 rd  = normalize(fwd + uv.x * rgt * 0.9 + uv.y * upV * 0.9);

    // Sky: IKB gradient — midnight navy → deep azure
    float sky = smoothstep(-0.3, 1.0, dot(rd, vec3(0.0, 1.0, 0.0)));
    vec3 col = mix(vec3(0.04, 0.04, 0.22), vec3(0.01, 0.02, 0.38), sky);
    // Horizon sun: gold blaze (Yves Klein complement)
    float sunAngle = max(0.0, dot(rd, normalize(vec3(0.7, 0.05, 0.7))));
    col += vec3(1.0, 0.72, 0.0) * pow(sunAngle, 24.0) * hdrPeak * 0.6;

    // Raymarch toward ocean surface
    float dist = 0.1;
    float tHit = -1.0;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * dist;
        float d = scene(p);
        if (abs(d) < SURF_DIST) { tHit = dist; break; }
        if (dist > MAX_DIST) break;
        dist += max(abs(d) * 0.55, 0.008);
    }

    if (tHit > 0.0) {
        vec3 p = ro + rd * tHit;
        vec3 n = calcNormal(p);

        // IKB ultramarine depth — darkest in deep troughs, brighter on crests
        float depth = clamp((p.y + waveHeight * 0.5) / waveHeight, 0.0, 1.0);
        vec3 deepBlue   = vec3(0.01, 0.02, 0.55);
        vec3 crestBlue  = vec3(0.08, 0.12, 0.88);
        vec3 oceanBase  = mix(deepBlue, crestBlue, depth);

        // Sun specular: gold white-hot HDR
        vec3 sunDir = normalize(vec3(0.7, 0.5, 0.7));
        vec3 halfV  = normalize(sunDir - rd);
        float spec  = pow(max(0.0, dot(n, halfV)), 96.0);

        // Foam: where wave height is near crest threshold → gold highlight
        float foamMask = smoothstep(foamThresh * waveHeight, waveHeight, p.y);
        vec3 foamCol = vec3(1.0, 0.82, 0.15) * hdrPeak * audio; // gold foam HDR

        // Scatter: shallow angle shows subsurface IKB glow
        float scatter = pow(max(0.0, 1.0 - abs(dot(rd, n))), 4.0) * 0.7;
        vec3 scatterCol = crestBlue * scatter * 1.8;

        vec3 surf = oceanBase + foamCol * foamMask
                  + vec3(1.0, 0.95, 0.75) * spec * hdrPeak * audio
                  + scatterCol;

        // Reflection of sky
        vec3 reflDir = reflect(rd, n);
        float skyRefl = smoothstep(-0.2, 0.6, dot(reflDir, vec3(0.0, 1.0, 0.0)));
        vec3 reflCol = mix(vec3(0.02, 0.02, 0.18), vec3(0.04, 0.06, 0.4), skyRefl);
        float fresnel = pow(1.0 - max(0.0, dot(-rd, n)), 4.0);
        surf = mix(surf, reflCol, fresnel * 0.4);

        // fwidth AA on wave crest edge
        float fw = fwidth(scene(p));
        float aa = smoothstep(fw * 2.0, 0.0, abs(scene(p)));
        col = mix(col, surf, aa);
    }

    FragColor = vec4(col, 1.0);
}
