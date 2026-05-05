/*{
  "CATEGORIES": ["Generator", "Audio Reactive", "Geometric", "3D"],
  "DESCRIPTION": "Particle Grid 3D — Kraftwerk/Ryoji Ikeda data.matrix raymarched: a grid of spheres, each column driven by one FFT bin. Studio lighting, emissive HDR on loud bins. Linear HDR output.",
  "INPUTS": [
    {"NAME":"gridW","LABEL":"Columns","TYPE":"float","MIN":4.0,"MAX":20.0,"DEFAULT":10.0},
    {"NAME":"gridH","LABEL":"Rows","TYPE":"float","MIN":2.0,"MAX":8.0,"DEFAULT":5.0},
    {"NAME":"sphereScale","LABEL":"Scale","TYPE":"float","MIN":0.05,"MAX":0.45,"DEFAULT":0.28},
    {"NAME":"decay","LABEL":"Decay","TYPE":"float","MIN":0.5,"MAX":0.99,"DEFAULT":0.9},
    {"NAME":"lowColor","LABEL":"Low Color","TYPE":"color","DEFAULT":[1.0,0.2,0.3,1.0]},
    {"NAME":"highColor","LABEL":"High Color","TYPE":"color","DEFAULT":[0.2,0.8,1.0,1.0]},
    {"NAME":"audioReact","LABEL":"Audio React","TYPE":"float","MIN":0.0,"MAX":2.0,"DEFAULT":1.0},
    {"NAME":"camDist","LABEL":"Cam Dist","TYPE":"float","MIN":2.0,"MAX":14.0,"DEFAULT":7.0},
    {"NAME":"camHeight","LABEL":"Cam Height","TYPE":"float","MIN":0.5,"MAX":5.0,"DEFAULT":2.0},
    {"NAME":"camOrbitSpeed","LABEL":"Orbit Speed","TYPE":"float","MIN":0.0,"MAX":0.5,"DEFAULT":0.07},
    {"NAME":"ambient","LABEL":"Ambient","TYPE":"float","MIN":0.0,"MAX":0.5,"DEFAULT":0.05}
  ]
}*/

float hash(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float getAmp(float col, float t) {
    float bin = clamp((col + 0.5) / max(gridW, 1.0) * 0.95, 0.0, 1.0);
    float a = texture(audioFFT, vec2(bin, 0.5)).r * audioReact;
    float wobble = 0.5 + 0.5 * sin(t * 1.4 + col * 0.71 + 3.14);
    // decay: high decay keeps last FFT value; low decay mixes in idle wobble
    return mix(a, max(a, wobble * 0.12), 1.0 - decay);
}

// Grid SDF — domain-repeated sphere per cell, clamped to grid bounds.
// Returns dist; caller recomputes cell from hit point.
float sceneDist(vec3 p, float gw, float gh, float cw, float ch, float t) {
    float halfW = gw * cw * 0.5;
    float halfH = gh * ch * 0.5;

    // Outside grid bounding box — return cheap bbox distance
    vec2 outside = max(abs(p.xz) - vec2(halfW, halfH), vec2(0.0));
    if (length(outside) > cw * 0.5) return length(outside) + abs(p.y) - 0.3;

    float ix = clamp(floor((p.x + halfW) / cw), 0.0, gw - 1.0);
    float iz = clamp(floor((p.z + halfH) / ch), 0.0, gh - 1.0);
    vec3 center = vec3((ix + 0.5) * cw - halfW, 0.0, (iz + 0.5) * ch - halfH);
    float amp = getAmp(ix, t);
    float r = sphereScale * min(cw, ch) * 0.5 * (0.08 + amp * 0.92);
    return length(p - center) - r;
}

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME;
    float audioMod = 0.5 + 0.5 * audioLevel * audioReact;

    float gw = max(4.0, gridW);
    float gh = max(2.0, gridH);
    float cw = 6.0 / gw;
    float ch = 3.0 / gh;

    // Orbiting camera
    float angle = t * camOrbitSpeed;
    vec3 ro = vec3(cos(angle) * camDist, camHeight, sin(angle) * camDist);
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(vec3(0.0, 1.0, 0.0), ww));
    vec3 vv = cross(ww, uu);
    vec3 rd = normalize(uv.x * uu + uv.y * vv + 1.5 * ww);

    // Raymarch
    float dist = 0.1;
    bool hit = false;
    for (int i = 0; i < 64; i++) {
        float d = sceneDist(ro + rd * dist, gw, gh, cw, ch, t);
        if (d < 0.002) { hit = true; break; }
        if (dist > 40.0) break;
        dist += max(d * 0.75, 0.005);
    }

    vec3 col = vec3(0.0);

    if (hit) {
        vec3 p = ro + rd * dist;

        // Normal by finite differences
        vec2 e2 = vec2(0.003, 0.0);
        vec3 n = normalize(vec3(
            sceneDist(p + e2.xyy, gw,gh,cw,ch,t) - sceneDist(p - e2.xyy, gw,gh,cw,ch,t),
            sceneDist(p + e2.yxy, gw,gh,cw,ch,t) - sceneDist(p - e2.yxy, gw,gh,cw,ch,t),
            sceneDist(p + e2.yyx, gw,gh,cw,ch,t) - sceneDist(p - e2.yyx, gw,gh,cw,ch,t)));

        vec3 v = normalize(-rd);

        // Recover cell id
        float halfW = gw * cw * 0.5; float halfH = gh * ch * 0.5;
        float ix = clamp(floor((p.x + halfW) / cw), 0.0, gw - 1.0);
        float iz = clamp(floor((p.z + halfH) / ch), 0.0, gh - 1.0);
        float colorT = ix / max(gw - 1.0, 1.0);
        vec3 base = mix(lowColor.rgb, highColor.rgb, colorT);
        float amp = getAmp(ix, t);

        // Studio key light from upper-right
        vec3 keyDir = normalize(vec3(0.5, 1.2, 0.6));
        float diff = max(dot(n, keyDir), 0.0);
        vec3 hv = normalize(keyDir + v);
        float spec = pow(max(dot(n, hv), 0.0), 80.0);

        col = base * (ambient + diff * 0.85);
        // HDR specular — peaks at ~2.0 on bright spheres
        col += vec3(1.0, 0.95, 0.85) * (spec * 1.8 + pow(spec, 6.0) * 2.2);
        // Emissive glow: loud bins become HDR light sources
        col += base * (amp * 2.0 + pow(amp, 2.0) * 3.5) * audioMod;

        // Bass column gets extra low-freq HDR boost
        if (ix < 1.0) col += lowColor.rgb * audioBass * audioReact * 1.2;
        // Treble column
        if (ix > gw - 2.0) col += highColor.rgb * audioHigh * audioReact * 1.2;

        // Diagonal cascade every 10s
        float _ph = fract(t / 10.0);
        float _w = (ix + iz) / max(gw + gh, 1.0);
        float _f = smoothstep(0.0, 0.04, _ph - _w * 0.3)
                 * smoothstep(_w * 0.3 + 0.18, _w * 0.3 + 0.10, _ph);
        col += vec3(1.0, 0.85, 0.55) * _f * 1.5;

    } else {
        // Dark studio background
        col = vec3(0.004, 0.004, 0.008);

        // Subtle floor grid
        if (rd.y < -0.001) {
            float ft = -camHeight / rd.y;
            if (ft > 0.0 && ft < 60.0) {
                vec3 fp = ro + rd * ft;
                float gx = abs(fract(fp.x * 0.5) - 0.5);
                float gz = abs(fract(fp.z * 0.5) - 0.5);
                float fw = fwidth(fp.x * 0.5);
                float gl2 = smoothstep(fw * 2.0, 0.0, min(gx, gz)) * 0.1 * exp(-ft * 0.04);
                col += mix(lowColor.rgb, highColor.rgb, 0.5) * gl2;
            }
        }
    }

    // Linear HDR output — host applies ACES
    gl_FragColor = vec4(col, 1.0);
}
