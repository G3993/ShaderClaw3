/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Circuit Board City - 3D PCB traces and pads with animated data pulse flow",
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Speed",   "TYPE": "float", "MIN": 0.0, "MAX": 3.0,  "DEFAULT": 0.8  },
    { "NAME": "intensity",  "LABEL": "Glow",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.6  },
    { "NAME": "density",    "LABEL": "Density", "TYPE": "float", "MIN": 0.1, "MAX": 1.0,  "DEFAULT": 0.5  },
    { "NAME": "tilt",       "LABEL": "Tilt",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.35 },
    { "NAME": "traceColor", "LABEL": "Trace",   "TYPE": "color", "DEFAULT": [0.0, 0.5, 1.0, 1.0] },
    { "NAME": "padColor",   "LABEL": "Pad",     "TYPE": "color", "DEFAULT": [1.0, 0.7, 0.0, 1.0] },
    { "NAME": "bgColor",    "LABEL": "BG",      "TYPE": "color", "DEFAULT": [0.0, 0.04, 0.02, 1.0] }
  ]
}*/

const float PI = 3.14159265;

float h2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float sdBox(vec3 p, vec3 b) {
    vec3 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
}

float sdSeg(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    return length(pa - ba * clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0));
}

// Returns vec3(traceDist, padDist, pulseGlow)
vec3 evalPCB(vec2 xz, float t) {
    float tw = 0.04;
    float vr = 0.09;
    float thresh = 1.0 - density * 0.85;
    vec2 base = floor(xz);
    float minTr = 1e6, minPad = 1e6, pulse = 0.0;

    for (int cx = -2; cx <= 2; cx++) {
        for (int cz = -2; cz <= 2; cz++) {
            vec2 c = base + vec2(float(cx), float(cz));
            vec2 o = c;

            // Horizontal trace
            if (h2(c * 3.71 + vec2(0.13)) > thresh) {
                float d = sdSeg(xz, o, o + vec2(1.0, 0.0)) - tw;
                if (d < minTr) {
                    minTr = d;
                    float sid = h2(c * 5.1 + vec2(0.91));
                    float pp = fract(t * (0.15 + sid * 0.5) + sid * 6.28);
                    float along = clamp(xz.x - o.x, 0.0, 1.0);
                    float pd = abs(pp - along);
                    pd = min(pd, 1.0 - pd);
                    pulse = max(pulse, exp(-pd * pd * 1200.0));
                }
            }

            // Vertical trace
            if (h2(c * 7.13 + vec2(0.47)) > thresh) {
                float d = sdSeg(xz, o, o + vec2(0.0, 1.0)) - tw;
                if (d < minTr) {
                    minTr = d;
                    float sid = h2(c * 11.3 + vec2(0.22));
                    float pp = fract(t * (0.15 + sid * 0.5) + sid * 6.28);
                    float along = clamp(xz.y - o.y, 0.0, 1.0);
                    float pd = abs(pp - along);
                    pd = min(pd, 1.0 - pd);
                    pulse = max(pulse, exp(-pd * pd * 1200.0));
                }
            }

            // Via pad at cell corner
            if (h2(c * 2.37 + vec2(1.11)) > 0.3) {
                minPad = min(minPad, length(xz - o) - vr);
            }
        }
    }
    return vec3(minTr, minPad, pulse);
}

// IC package SDF — sparse 3x3 cell chips
float icSDF(vec3 p) {
    vec2 gc = floor(p.xz / 3.0);
    float md = 1e6;
    for (int cx = -1; cx <= 1; cx++) {
        for (int cz = -1; cz <= 1; cz++) {
            vec2 c = gc + vec2(float(cx), float(cz));
            if (h2(c * 0.71 + vec2(3.1)) > 0.8) {
                vec3 ctr = vec3((c.x + 0.5) * 3.0, 0.12, (c.y + 0.5) * 3.0);
                float sx = 0.8 + h2(c) * 0.5;
                float sz = 0.8 + h2(c + vec2(0.3)) * 0.5;
                md = min(md, sdBox(p - ctr, vec3(sx, 0.12, sz)));
            }
        }
    }
    return md;
}

vec4 renderScene(vec2 uv, float t) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float scrollZ = t * 0.5;
    float orbit = t * 0.025;
    float camH = mix(3.5, 6.5, 1.0 - tilt);
    float camBack = mix(0.5, 2.5, 1.0 - tilt);

    vec3 ro = vec3(sin(orbit) * 1.5, camH, -camBack - scrollZ);
    vec3 target = vec3(sin(orbit) * 0.5, 0.0, 1.5 - scrollZ);

    vec3 fwd = normalize(target - ro);
    vec3 rgt = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 up = cross(rgt, fwd);

    vec2 ndc = (uv * 2.0 - 1.0) * vec2(aspect, 1.0) * 0.65;
    vec3 rd = normalize(fwd + ndc.x * rgt + ndc.y * up);

    vec3 col = bgColor.rgb;

    // Raymarch for IC packages
    float dt = 0.01;
    bool hitIC = false;
    vec3 icP = vec3(0.0);
    vec3 icN = vec3(0.0, 1.0, 0.0);

    for (int i = 0; i < 60; i++) {
        vec3 p = ro + rd * dt;
        float gd = p.y;
        float id = icSDF(p);
        float d = min(gd, id);
        if (d < 0.003) {
            if (id < gd && id < 0.006) {
                hitIC = true;
                icP = p;
                float e = 0.002;
                icN = normalize(vec3(
                    icSDF(p + vec3(e,0,0)) - icSDF(p - vec3(e,0,0)),
                    icSDF(p + vec3(0,e,0)) - icSDF(p - vec3(0,e,0)),
                    icSDF(p + vec3(0,0,e)) - icSDF(p - vec3(0,0,e))
                ));
            }
            break;
        }
        dt += max(d, 0.005);
        if (dt > 20.0) break;
    }

    if (hitIC) {
        vec3 base = vec3(0.02, 0.15, 0.06);
        float diff = max(dot(icN, normalize(vec3(0.5, 1.0, 0.3))), 0.0);
        col = base * (0.4 + diff * 0.8);
        // Gold pin accents on side faces
        if (abs(icN.y) < 0.2) {
            float pin = smoothstep(0.35, 0.45, fract(icP.x * 2.5))
                      * smoothstep(0.35, 0.45, fract(icP.z * 2.5));
            col += padColor.rgb * pin * 0.8;
        }
    } else if (rd.y < -0.001) {
        float tGnd = -ro.y / rd.y;
        vec3 hp = ro + rd * tGnd;
        vec2 xz = hp.xz;
        float fog = exp(-tGnd * 0.04);

        col = bgColor.rgb * 1.3;

        vec3 pcb = evalPCB(xz, t);
        float trD = pcb.x, paD = pcb.y, pul = pcb.z;

        // Trace fill and soft glow
        float trFill = smoothstep(0.007, -0.004, trD);
        float trGlow = exp(-max(trD, 0.0) * 45.0) * intensity;
        vec3 tC = traceColor.rgb;
        col = mix(col, tC * 2.2, trFill);
        col += tC * trGlow * 0.6;

        // Data pulse: bright white-cyan flash along filled trace
        col += vec3(2.2, 2.6, 3.0) * pul * trFill * 1.5;
        col += tC * pul * trGlow * 3.0;

        // Solder pad fill and glow
        float paFill = smoothstep(0.01, -0.005, paD);
        float paGlow = exp(-max(paD, 0.0) * 35.0) * intensity;
        col = mix(col, padColor.rgb * 2.8, paFill);
        col += padColor.rgb * paGlow * 0.5;

        col = mix(bgColor.rgb, col, fog);
    }

    // Vignette
    vec2 vc = uv - 0.5;
    col *= 1.0 - dot(vc, vc) * 1.4;

    return vec4(col, 1.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float t = TIME * speed;

    vec4 col = renderScene(uv, t);

    if (_voiceGlitch > 0.01) {
        float g = _voiceGlitch;
        float vt = TIME * 17.0;
        float band = floor(uv.y * mix(8.0, 40.0, g) + vt * 3.0);
        float bNoise = fract(sin(band * 91.7 + vt) * 43758.5);
        float bActive = step(1.0 - g * 0.6, bNoise);
        float shift = (bNoise - 0.5) * 0.08 * g * bActive;
        float chroma = g * 0.015;
        vec2 uvR = uv + vec2(shift + chroma, 0.0);
        vec2 uvB = uv + vec2(shift - chroma, 0.0);
        vec2 uvG = uv + vec2(shift, chroma * 0.5);
        vec4 cR = renderScene(uvR, t);
        vec4 cG = renderScene(uvG, t);
        vec4 cB = renderScene(uvB, t);
        vec4 glitched = vec4(cR.r, cG.g, cB.b, 1.0);
        float scan = 0.95 + 0.05 * sin(uv.y * RENDERSIZE.y * 1.5 + vt * 40.0);
        float bx = floor(uv.x * 6.0), by = floor(uv.y * 4.0);
        float bn = fract(sin((bx + by * 7.0) * 113.1 + floor(vt * 8.0)) * 43758.5);
        float drop = step(1.0 - g * 0.15, bn);
        glitched.rgb *= scan * (1.0 - drop);
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = col;
}
