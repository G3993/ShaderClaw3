/*{
  "CATEGORIES": ["Generator"],
  "DESCRIPTION": "Synthwave Drive - first-person 3D retrowave fly-through with perspective grid and floating gems",
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Speed",   "TYPE": "float", "MIN": 0.0, "MAX": 3.0, "DEFAULT": 1.0  },
    { "NAME": "intensity",   "LABEL": "Glow",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.7  },
    { "NAME": "gridDensity", "LABEL": "Grid",    "TYPE": "float", "MIN": 1.0, "MAX": 8.0, "DEFAULT": 3.0  },
    { "NAME": "gemCount",    "LABEL": "Gems",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5  },
    { "NAME": "skyTop",      "LABEL": "Sky Top", "TYPE": "color", "DEFAULT": [0.10, 0.0, 0.35, 1.0]      },
    { "NAME": "skyHorizon",  "LABEL": "Horizon", "TYPE": "color", "DEFAULT": [1.0,  0.1, 0.60, 1.0]      },
    { "NAME": "traceCol",    "LABEL": "Grid",    "TYPE": "color", "DEFAULT": [0.0,  1.0, 0.90, 1.0]      },
    { "NAME": "gemCol",      "LABEL": "Gems",    "TYPE": "color", "DEFAULT": [1.0,  0.1, 0.80, 1.0]      }
  ]
}*/

const float PI = 3.14159265;

float h2(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float sdOct(vec3 p, float r) {
    return (abs(p.x) + abs(p.y) + abs(p.z) - r) * 0.5774;
}

// Gem field: spinning octahedra in repeating XZ cells, hovering
float gemField(vec3 p, out float gid) {
    float cx = 5.0, cz = 4.2;
    vec2 cid = floor(vec2(p.x / cx + 0.5, p.z / cz + 0.5));
    vec3 q   = vec3(p.x - cid.x * cx, p.y, p.z - cid.y * cz);

    gid = h2(cid);
    if (step(gid, gemCount) < 0.5) return 1e6;

    float xj  = (h2(cid + vec2(0.11)) - 0.5) * cx * 0.6;
    float zj  = (h2(cid + vec2(0.22)) - 0.5) * cz * 0.5;
    float yp  = 1.5 + sin(TIME * 0.55 + h2(cid + vec2(0.33)) * 6.28) * 0.28;
    float spn = TIME * 0.55 + h2(cid + vec2(0.44)) * 6.28;
    float cs  = cos(spn), sn = sin(spn);

    vec3 lp = q - vec3(xj, yp, zj);
    lp.xz = vec2(cs * lp.x - sn * lp.z, sn * lp.x + cs * lp.z);

    float r = 0.26 + h2(cid + vec2(0.55)) * 0.14;
    return sdOct(lp, r);
}

vec3 gemNormal(vec3 p) {
    float gid; float e = 0.001;
    return normalize(vec3(
        gemField(p + vec3(e,0,0), gid) - gemField(p - vec3(e,0,0), gid),
        gemField(p + vec3(0,e,0), gid) - gemField(p - vec3(0,e,0), gid),
        gemField(p + vec3(0,0,e), gid) - gemField(p - vec3(0,0,e), gid)
    ));
}

vec3 gemColor(float id) {
    int c = int(id * 3.999);
    if (c == 0) return gemCol.rgb;
    if (c == 1) return mix(gemCol.rgb, traceCol.rgb, 0.6);
    if (c == 2) return vec3(1.0, 0.82, 0.05);
    return traceCol.rgb;
}

vec4 renderSynthwave(vec2 uv) {
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    float t = TIME * speed;

    float camZ    = -t * 2.0;
    float camWave = sin(t * 0.28) * 0.18;
    vec3 ro  = vec3(camWave, 0.85, camZ);
    vec3 fwd = normalize(vec3(sin(t * 0.11) * 0.05, -0.06, 1.0));
    vec3 rgt = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3 upV = cross(rgt, fwd);

    vec2 ndc = (uv * 2.0 - 1.0) * vec2(aspect, 1.0) * 0.70;
    vec3 rd  = normalize(fwd + ndc.x * rgt + ndc.y * upV);

    // Sky gradient
    float sf = max(rd.y, 0.0);
    vec3 skyCol = mix(skyHorizon.rgb, skyTop.rgb, pow(sf, 0.45)) * 0.55;

    // Retrowave sun with horizontal bars cut from lower half
    vec3 sunD  = normalize(vec3(0.0, 0.05, 1.0));
    float sAng = acos(clamp(dot(rd, sunD), -1.0, 1.0));
    float sR   = 0.18;
    float sMask = smoothstep(sR, sR - 0.012, sAng);
    float sLY   = dot(rd - sunD * dot(rd, sunD), vec3(0.0, 1.0, 0.0)) / sR;
    float sBar  = step(0.5, fract(sLY * 5.5 + 0.5));
    float sBot  = step(0.0, -sLY);
    sMask *= 1.0 - sBot * (1.0 - sBar);

    vec3 col = skyCol;
    col += skyHorizon.rgb * exp(-sAng * 5.5) * 0.9 * step(rd.y, 0.35);
    col  = mix(col, vec3(3.2, 1.1, 0.05), sMask);
    col += skyHorizon.rgb * exp(-abs(rd.y) * 10.0) * 0.5;

    // Floor t (ray-plane at y=0)
    float tFloor = (rd.y < -0.001) ? (ro.y / (-rd.y)) : 1e9;

    // March gems up to floor or max dist
    float maxT  = min(tFloor, 40.0);
    float dt    = 0.05;
    bool  hitGem = false;
    float gemId  = 0.0;

    for (int i = 0; i < 80; i++) {
        if (dt >= maxT) break;
        vec3 p = ro + rd * dt;
        float gid;
        float gd = gemField(p, gid);
        if (gd < 0.003) { hitGem = true; gemId = gid; break; }
        dt += max(gd, 0.01);
    }

    if (hitGem) {
        vec3 p  = ro + rd * dt;
        vec3 n  = gemNormal(p);
        vec3 gC = gemColor(gemId);
        vec3 L  = normalize(vec3(0.3, 1.0, -0.4));
        float diff = max(dot(n, L), 0.0);
        float spec = pow(max(dot(reflect(-L, n), -rd), 0.0), 36.0);
        float fres = pow(1.0 - abs(dot(n, -rd)), 3.0);
        float fog  = exp(-dt * 0.04);
        col = (gC * diff * 2.5 + vec3(1.0) * spec * 2.8 + gC * fres * 2.0) * fog;
        col += skyHorizon.rgb * (1.0 - fog) * 0.3;
    } else if (tFloor < 40.0) {
        vec3 hp  = ro + rd * tFloor;
        vec2 xz  = hp.xz;
        float fog = exp(-tFloor * 0.032);

        float csz = gridDensity;
        vec2 gf   = abs(fract(xz / csz) - 0.5) * csz;
        float glw   = csz * 0.026;
        float gAmt  = smoothstep(glw * 1.4, glw * 0.1, min(gf.x, gf.y));
        float gGlow = exp(-min(gf.x, gf.y) / csz * 22.0) * intensity;

        vec3 floorBase = skyTop.rgb * 0.07;
        vec3 floorCol  = floorBase;
        floorCol = mix(floorCol, traceCol.rgb * 2.5, gAmt);
        floorCol += traceCol.rgb * gGlow * 0.45;

        col = mix(skyHorizon.rgb * 0.25, floorCol, fog);
    }

    // Vignette
    vec2 vc = uv - 0.5;
    col *= 1.0 - dot(vc, vc) * 1.25;

    return vec4(col, 1.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 col = renderSynthwave(uv);

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
        vec4 cR = renderSynthwave(uvR);
        vec4 cG = renderSynthwave(uvG);
        vec4 cB = renderSynthwave(uvB);
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
