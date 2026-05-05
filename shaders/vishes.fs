/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Glowing Lattice Pulse - 3D grid of cubes pulsing alive via three-wave interference, orbiting camera",
  "INPUTS": [
    { "NAME": "speed",    "LABEL": "Speed",    "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 0.6  },
    { "NAME": "density",  "LABEL": "Density",  "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5  },
    { "NAME": "boxSize",  "LABEL": "Box Size", "TYPE": "float", "MIN": 0.1, "MAX": 0.45,"DEFAULT": 0.28 },
    { "NAME": "hdrPeak",  "LABEL": "HDR Peak", "TYPE": "float", "MIN": 1.0, "MAX": 4.0, "DEFAULT": 2.8  },
    { "NAME": "col0",     "LABEL": "Color A",  "TYPE": "color", "DEFAULT": [0.4, 0.0, 1.0, 1.0]        },
    { "NAME": "col1",     "LABEL": "Color B",  "TYPE": "color", "DEFAULT": [0.0, 1.0, 0.9, 1.0]        },
    { "NAME": "col2",     "LABEL": "Color C",  "TYPE": "color", "DEFAULT": [1.0, 0.7, 0.0, 1.0]        },
    { "NAME": "col3",     "LABEL": "Color D",  "TYPE": "color", "DEFAULT": [1.0, 0.05,0.7, 1.0]        }
  ]
}*/

const float PI = 3.14159265;

float h3(vec3 p) { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453); }

float sdBox(vec3 p, float he) {
    vec3 d = abs(p) - he;
    return length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
}

// Three-wave interference: cell is alive when product exceeds threshold
float aliveFn(vec3 cid, float t) {
    float gridR = mix(3.5, 7.0, density);
    if (length(cid) > gridR) return 0.0;

    float ph = h3(cid) * 6.28;
    float val = sin(cid.x * 0.88 + t * 0.37 + ph)
              * cos(cid.y * 1.15 - t * 0.49)
              * sin(cid.z * 0.73 + t * 0.28 + ph * 0.6);
    return step(0.12, val);
}

// Cell color from position hash
vec3 cellColor(vec3 cid) {
    int ci = int(h3(cid + vec3(0.5)) * 3.999);
    if (ci == 0) return col0.rgb;
    if (ci == 1) return col1.rgb;
    if (ci == 2) return col2.rgb;
    return col3.rgb;
}

// SDF over 3x3x3 neighborhood of nearest cells
vec2 scene(vec3 p, float t) {
    vec3 base = floor(p + 0.5);
    float md = 1e6;
    vec3 hitCell = vec3(0.0);

    for (int dx = -1; dx <= 1; dx++) {
        for (int dy = -1; dy <= 1; dy++) {
            for (int dz = -1; dz <= 1; dz++) {
                vec3 cid = base + vec3(float(dx), float(dy), float(dz));
                if (aliveFn(cid, t) < 0.5) continue;
                float d = sdBox(p - cid, boxSize);
                if (d < md) { md = d; hitCell = cid; }
            }
        }
    }
    return vec2(md, h3(hitCell + vec3(0.5)) * 3.999);
}

vec3 sceneNormal(vec3 p, float t) {
    float e = 0.001;
    return normalize(vec3(
        scene(p + vec3(e,0,0), t).x - scene(p - vec3(e,0,0), t).x,
        scene(p + vec3(0,e,0), t).x - scene(p - vec3(0,e,0), t).x,
        scene(p + vec3(0,0,e), t).x - scene(p - vec3(0,0,e), t).x
    ));
}

vec4 renderScene(vec2 uv) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME * speed;

    // Orbiting camera
    float camA  = t * 0.18;
    float camEl = sin(t * 0.13) * 0.42;
    float gridR = mix(3.5, 7.0, density);
    float camR  = gridR * 1.8 + 2.0;
    vec3 ro = vec3(sin(camA) * cos(camEl), sin(camEl), cos(camA) * cos(camEl)) * camR;
    vec3 ta = vec3(0.0);

    vec3 fwd = normalize(ta - ro);
    vec3 rgt = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 upV = cross(rgt, fwd);

    vec2 ndc = (uv * 2.0 - 1.0) * vec2(aspect, 1.0);
    vec3 rd  = normalize(fwd + ndc.x * rgt + ndc.y * upV * (RENDERSIZE.y / RENDERSIZE.x < 1.0 ? 1.0 : 1.0));

    // Background: deep void with faint grid-edge glow
    vec3 bg = vec3(0.0, 0.0, 0.008);

    // March
    float dt   = 0.02;
    float hitId = -1.0;
    vec3 hitCell = vec3(0.0);

    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * dt;
        vec2 res = scene(p, t);
        if (res.x < 0.002) { hitId = res.y; break; }
        if (dt > camR * 2.5) break;
        dt += max(res.x * 0.8, 0.01);
    }

    vec3 col = bg;

    if (hitId >= 0.0) {
        vec3 p = ro + rd * dt;
        vec3 n = sceneNormal(p, t);

        // Nearest alive cell determines color
        vec3 base = floor(p + 0.5);
        vec3 cid  = base;
        float best = 1e6;
        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++) {
                for (int dz = -1; dz <= 1; dz++) {
                    vec3 c = base + vec3(float(dx), float(dy), float(dz));
                    if (aliveFn(c, t) < 0.5) continue;
                    float dd = length(p - c);
                    if (dd < best) { best = dd; cid = c; }
                }
            }
        }
        vec3 cCol = cellColor(cid);

        vec3 L1 = normalize(vec3(1.0, 1.5, 0.5));
        vec3 L2 = normalize(vec3(-0.7, 0.4, -0.6));
        float diff = max(dot(n, L1), 0.0) * 0.6 + max(dot(n, L2), 0.0) * 0.25 + 0.15;
        float spec = pow(max(dot(reflect(-L1, n), -rd), 0.0), 40.0)
                   + pow(max(dot(reflect(-L2, n), -rd), 0.0), 24.0) * 0.4;
        float fres = pow(1.0 - max(dot(n, -rd), 0.0), 4.0);

        col  = cCol * diff * hdrPeak;
        col += vec3(1.0) * spec * hdrPeak;
        col += cCol * fres * hdrPeak * 0.7;

        float fog = clamp(dt / (camR * 2.5), 0.0, 1.0);
        col = mix(col, bg, fog * 0.35);
    }

    // Vignette
    vec2 vc = uv - 0.5;
    col *= 1.0 - dot(vc, vc) * 1.1;

    return vec4(col, 1.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 col = renderScene(uv);

    if (_voiceGlitch > 0.01) {
        float g  = _voiceGlitch;
        float vt = TIME * 17.0;
        float band   = floor(uv.y * mix(8.0, 40.0, g) + vt * 3.0);
        float bNoise = fract(sin(band * 91.7 + vt) * 43758.5);
        float bAct   = step(1.0 - g * 0.6, bNoise);
        float shift  = (bNoise - 0.5) * 0.08 * g * bAct;
        float chroma = g * 0.015;
        vec2 uvR = uv + vec2(shift + chroma, 0.0);
        vec2 uvB = uv + vec2(shift - chroma, 0.0);
        vec2 uvG = uv + vec2(shift, chroma * 0.5);
        vec4 cR = renderScene(uvR);
        vec4 cG = renderScene(uvG);
        vec4 cB = renderScene(uvB);
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
