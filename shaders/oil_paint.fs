/*{
  "DESCRIPTION": "Jade Ring Sculpture — 5 raymarched nested tori in jade and glacier palette",
  "CREDIT": "auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Abstract"],
  "INPUTS": [
    { "NAME": "ringCount",  "LABEL": "Ring Count", "TYPE": "float", "DEFAULT": 5.0, "MIN": 2.0, "MAX": 8.0 },
    { "NAME": "rotSpeed",   "LABEL": "Speed",      "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "hdrPeak",    "LABEL": "HDR Peak",   "TYPE": "float", "DEFAULT": 2.5, "MIN": 1.0, "MAX": 4.0 },
    { "NAME": "audioReact", "LABEL": "Audio",      "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

// --- Rotation helpers ---
mat2 rot2(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}
vec3 rotY(vec3 p, float a) { p.xz = rot2(a) * p.xz; return p; }
vec3 rotX(vec3 p, float a) { p.yz = rot2(a) * p.yz; return p; }
vec3 rotZ(vec3 p, float a) { p.xy = rot2(a) * p.xy; return p; }

// --- Torus SDF ---
float sdTorus(vec3 p, vec2 t) {
    return length(vec2(length(p.xz) - t.x, p.y)) - t.y;
}

// --- Scene: nested tori at varying angles ---
float scene(vec3 p) {
    float tt  = TIME * rotSpeed;
    float audio = 1.0 + audioLevel * audioReact * 0.35;

    int N = int(clamp(ringCount, 2.0, 8.0));
    float d = 1e10;

    for (int i = 0; i < 8; i++) {
        if (i >= N) break;
        float fi = float(i);

        // Major radius grows, tube radius fixed, with audio pulse
        float majorR = (0.55 + fi * 0.18) * audio;
        float tubeR  = 0.06 + fi * 0.01;

        // Each ring gets a unique rotation axis and phase
        float phase = tt + fi * 0.65;
        vec3 lp = p;

        // Alternate axis: xz-plane, xy-plane, yz-plane with cross tilts
        float tilt = fi * 0.42 + 0.2;
        if      (i == 0) { lp = rotX(lp, phase); }
        else if (i == 1) { lp = rotZ(lp, phase * 0.8); lp = rotX(lp, tilt); }
        else if (i == 2) { lp = rotY(lp, phase * 1.1); lp = rotZ(lp, tilt); }
        else if (i == 3) { lp = rotX(lp, tilt); lp = rotZ(lp, phase * 0.6); }
        else if (i == 4) { lp = rotZ(lp, tilt * 0.5); lp = rotY(lp, phase * 1.3); }
        else if (i == 5) { lp = rotX(lp, phase * 0.9 + tilt); }
        else if (i == 6) { lp = rotY(lp, tilt * 1.2); lp = rotX(lp, phase * 0.7); }
        else             { lp = rotZ(lp, phase * 1.1); lp = rotY(lp, tilt * 0.8); }

        float dt = sdTorus(lp, vec2(majorR, tubeR));
        d = min(d, dt);
    }
    return d;
}

// --- Normal via finite differences ---
vec3 calcNormal(vec3 p) {
    float eps = 0.001;
    return normalize(vec3(
        scene(p + vec3(eps, 0.0, 0.0)) - scene(p - vec3(eps, 0.0, 0.0)),
        scene(p + vec3(0.0, eps, 0.0)) - scene(p - vec3(0.0, eps, 0.0)),
        scene(p + vec3(0.0, 0.0, eps)) - scene(p - vec3(0.0, 0.0, eps))
    ));
}

// --- Jade/glacier palette: fully saturated, no white-mixing ---
// jade green, glacier teal, ice highlight
vec3 ringColor(vec3 p, vec3 n, float tt) {
    // Map normal direction to palette
    float blend = 0.5 + 0.5 * sin(n.y * 3.0 + tt * 0.7 + p.x * 2.0);

    vec3 jade    = vec3(0.1, 0.8, 0.5);   // jade green
    vec3 glacier = vec3(0.0, 0.7, 0.9);   // glacier teal
    vec3 ice     = vec3(0.9, 1.0, 1.0);   // ice white — used only in HDR specular

    // Mix jade and glacier based on surface angle — no grey/white in diffuse
    return mix(jade, glacier, blend);
}

void main() {
    vec2 uv = isf_FragNormCoord.xy * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspect;

    float audio = 1.0 + audioLevel * audioReact * 0.35;
    float tt = TIME * rotSpeed;

    // Orbiting camera — slow, slightly elevated
    float camAngle = tt * 0.35;
    float camElev  = 0.55 + sin(tt * 0.14) * 0.15;
    float camDist  = 3.4;
    vec3 ro = vec3(cos(camAngle) * camDist * cos(camElev),
                   sin(camElev) * camDist,
                   sin(camAngle) * camDist * cos(camElev));
    vec3 forward = normalize(-ro);
    vec3 right   = normalize(cross(vec3(0.0, 1.0, 0.0), forward));
    vec3 up      = cross(forward, right);

    float fov = 1.2;
    vec3 rd = normalize(forward * fov + right * uv.x + up * uv.y);

    // --- 64-step raymarch ---
    float dist = 0.0;
    float hit  = 0.0;
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dist;
        float d = scene(p);
        if (d < 0.001) { hit = 1.0; break; }
        if (dist > 12.0) break;
        dist += d;
    }

    // Black void background
    vec3 col = vec3(0.005, 0.01, 0.008);

    if (hit > 0.5) {
        vec3 p = ro + rd * dist;
        vec3 n = calcNormal(p);

        // Black silhouette edge
        float edge = 1.0 - smoothstep(0.0, 0.2, abs(dot(n, -rd)));

        // Surface color: jade/glacier blend
        vec3 baseCol = ringColor(p, n, tt) * hdrPeak;

        // Key light from upper-right-front
        vec3 keyLight = normalize(vec3(1.5, 2.0, 1.0));
        float diff = max(dot(n, keyLight), 0.0);

        // Fill light from below — cold blue-green tint
        vec3 fillLight = normalize(vec3(-1.0, -1.0, 0.5));
        float fill = max(dot(n, fillLight), 0.0) * 0.25;

        // Specular: ice-white highlight (peak 2.5+)
        vec3 halfVec = normalize(keyLight - rd);
        float spec   = pow(max(dot(n, halfVec), 0.0), 64.0);
        // Ice white highlight — HDR peak value
        vec3 specCol = vec3(0.9, 1.0, 1.0) * spec * hdrPeak * audio;

        // Rim light — teal rim from camera-opposite side
        float rim    = pow(1.0 - max(dot(n, -rd), 0.0), 3.0);
        vec3 rimCol  = vec3(0.0, 0.7, 0.9) * rim * hdrPeak * 0.5;

        // Subtle self-emission from jade core
        vec3 ambient = vec3(0.1, 0.8, 0.5) * 0.12 * hdrPeak;

        col = ambient + baseCol * (diff * 1.2 + fill) + specCol + rimCol;
        // Black edge
        col *= (1.0 - edge * 0.92);
    }

    // No tone mapping — raw HDR output
    gl_FragColor = vec4(col, 1.0);
}
