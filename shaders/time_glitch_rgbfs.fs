/*{
  "DESCRIPTION": "Prismatic Crystal Ball — 3D raymarched sphere with Voronoi-fractured interior, glowing amber-gold-white-hot specular. Close-up gem portrait.",
  "CREDIT": "Easel auto-improve 2026-05-06",
  "CATEGORIES": ["Generator", "3D"],
  "INPUTS": [
    { "NAME": "camSpeed",    "LABEL": "Cam Speed",    "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "crackDensity","LABEL": "Crack Density","TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0, "MAX": 10.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",     "TYPE": "float", "DEFAULT": 2.8,  "MIN": 1.0, "MAX": 5.0 },
    { "NAME": "glowColor",   "LABEL": "Core Color",   "TYPE": "color", "DEFAULT": [1.0, 0.62, 0.0, 1.0] },
    { "NAME": "audioReact",  "LABEL": "Audio React",  "TYPE": "float", "DEFAULT": 0.8,  "MIN": 0.0, "MAX": 2.0 }
  ]
}*/

#define MAX_STEPS 64
#define MAX_DIST  12.0
#define SURF_DIST 0.002

float hash11(float n) { return fract(sin(n*12.9898)*43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float hash31(vec3 p)  { return fract(sin(dot(p,vec3(127.1,311.7,74.7)))*43758.5453); }

// 3D Voronoi: returns distance to nearest cell center and cell ID
vec2 voronoi3D(vec3 p) {
    vec3 pi = floor(p);
    vec3 pf = fract(p);
    float minDist = 10.0;
    float cellID  = 0.0;
    for (int x = -1; x <= 1; x++) {
        for (int y = -1; y <= 1; y++) {
            for (int z = -1; z <= 1; z++) {
                vec3 nb = vec3(float(x),float(y),float(z));
                vec3 np = pi + nb;
                vec3 rp = vec3(hash31(np), hash31(np+vec3(1,0,0)), hash31(np+vec3(0,1,0)));
                rp = 0.5 + 0.5*sin(TIME*camSpeed*0.5 + 6.28318*rp); // animated cells
                vec3 diff = nb + rp - pf;
                float d = length(diff);
                if (d < minDist) {
                    minDist = d;
                    cellID = hash31(np + vec3(0,0,1));
                }
            }
        }
    }
    return vec2(minDist, cellID);
}

// Crystal sphere SDF
float sdSphere(vec3 p, float r) { return length(p) - r; }

// Map: sphere of radius 0.9
float map(vec3 p) {
    return sdSphere(p, 0.9);
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        map(p+e.xyy)-map(p-e.xyy),
        map(p+e.yxy)-map(p-e.yxy),
        map(p+e.yyx)-map(p-e.yyx)));
}

// Amber-gold spectrum: 0=deep amber, 0.5=gold, 1.0=white-hot
vec3 gemSpectrum(float t) {
    vec3 c0 = vec3(0.6,  0.2,  0.0);   // deep amber
    vec3 c1 = glowColor.rgb;            // user gold (default [1,0.62,0])
    vec3 c2 = vec3(1.0,  0.92, 0.6);   // pale gold
    vec3 c3 = vec3(1.2,  1.15, 1.0);   // white-hot HDR
    float t2 = t*3.0;
    if (t2<1.0) return mix(c0,c1,t2);
    if (t2<2.0) return mix(c1,c2,t2-1.0);
    return mix(c2,c3,t2-2.0);
}

void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);
    uv.x *= aspect;

    float audio = 1.0 + audioLevel*audioReact*0.3 + audioBass*audioReact*0.2;

    // Close-up orbiting camera
    float camT = TIME * camSpeed;
    float camR = 2.5;
    vec3 ro = vec3(sin(camT)*camR, sin(camT*0.41)*0.4, cos(camT)*camR);
    vec3 ta  = vec3(0.0);
    vec3 ww  = normalize(ta - ro);
    vec3 uu  = normalize(cross(ww, vec3(0,1,0)));
    vec3 vv  = cross(uu, ww);
    vec3 rd  = normalize(uv.x*uu + uv.y*vv + 1.8*ww);

    // March to sphere surface
    float dist = 0.0;
    float hitT = MAX_DIST;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd*dist;
        float d = map(p);
        if (d < SURF_DIST) { hitT = dist; break; }
        dist += d;
        if (dist > MAX_DIST) break;
    }

    // Background: deep void with refracted light halos
    vec3 col = vec3(0.005, 0.003, 0.001);
    // Refracted background glow (light coming through crystal)
    float bgGlow = exp(-length(uv)*1.2)*0.3;
    col += glowColor.rgb * bgGlow * hdrPeak * 0.3 * audio;

    if (hitT < MAX_DIST) {
        vec3 p = ro + rd*hitT;
        vec3 n = calcNormal(p);

        // Interior: sample Voronoi crystal structure
        // March a refracted ray inside the sphere
        vec3 refractDir = refract(rd, n, 0.65); // glass IOR ~1.5
        if (length(refractDir) < 0.001) refractDir = rd; // TIR fallback
        vec3 interior = p + refractDir * 0.1;

        // Sample Voronoi at scaled interior position
        vec2 vor = voronoi3D(interior * crackDensity);
        float crackDist = vor.x;
        float cellHue   = vor.y;

        // Crack glow: thin bright edges between Voronoi cells
        float crackMask = exp(-crackDist * 8.0);

        // Cell color based on hue ID
        vec3 cellCol = gemSpectrum(cellHue);

        // Interior emission: cells glow in amber-gold
        vec3 interiorCol = cellCol * (0.3 + crackMask * hdrPeak * audio);

        // Surface: specular highlight + rim
        vec3 lDir = normalize(vec3(0.7, 1.2, 0.5));
        float spec = pow(max(dot(reflect(-lDir, n), -rd), 0.0), 128.0);
        float rim  = pow(1.0 - max(dot(n, -rd), 0.0), 4.0);

        // Fresnel: at grazing angles show surface reflection (gold specular)
        float fresnel = pow(1.0 - max(dot(n, -rd), 0.0), 2.0);

        col = interiorCol;
        col += gemSpectrum(1.0) * spec * hdrPeak * 2.0 * audio; // white-hot specular
        col += gemSpectrum(0.5) * rim * hdrPeak * 0.5 * audio;  // gold rim
        col += gemSpectrum(0.7) * fresnel * hdrPeak * 0.3 * audio; // fresnel

        // fwidth AA on sphere silhouette
        float sdfAA = fwidth(map(p));
        col *= smoothstep(0.0, sdfAA, abs(map(p)) + SURF_DIST*2.0) + 0.01;

        // Secondary internal bounces: march deeper into sphere for inner glow
        float innerT = 0.1;
        vec3 innerGlow = vec3(0.0);
        for (int j = 0; j < 24; j++) {
            vec3 ip = p + refractDir*innerT;
            if (length(ip) > 0.9) break;
            vec2 iv = voronoi3D(ip * crackDensity);
            float ig = exp(-iv.x * 6.0);
            innerGlow += gemSpectrum(iv.y) * ig * 0.04;
            innerT += 0.06;
        }
        col += innerGlow * hdrPeak * audio;
    }

    // Caustic ring around crystal (projected light)
    float caustDist = abs(length(uv) - 1.1);
    col += glowColor.rgb * exp(-caustDist*12.0) * hdrPeak * 0.3 * audio;

    gl_FragColor = vec4(col, 1.0);
}
