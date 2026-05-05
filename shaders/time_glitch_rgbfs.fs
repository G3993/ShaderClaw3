/*{
  "DESCRIPTION": "Crystal Lattice — BCC diamond-cubic lattice raymarched from inside. Camera spirals through glowing atoms; white-hot HDR specular facets, neon iso-rings, fwidth AA.",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed",      "LABEL": "Speed",      "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.4  },
    { "NAME": "zoom",       "LABEL": "Zoom",        "TYPE": "float", "MIN": 0.3,  "MAX": 3.0,  "DEFAULT": 1.0  },
    { "NAME": "cellSize",   "LABEL": "Cell Size",   "TYPE": "float", "MIN": 0.6,  "MAX": 3.0,  "DEFAULT": 1.5  },
    { "NAME": "atomRadius", "LABEL": "Atom Size",   "TYPE": "float", "MIN": 0.05, "MAX": 0.45, "DEFAULT": 0.22 },
    { "NAME": "bondRadius", "LABEL": "Bond Width",  "TYPE": "float", "MIN": 0.0,  "MAX": 0.10, "DEFAULT": 0.03 },
    { "NAME": "hdrGlow",    "LABEL": "HDR Glow",    "TYPE": "float", "MIN": 0.5,  "MAX": 4.0,  "DEFAULT": 2.0  },
    { "NAME": "palette",    "LABEL": "Palette",     "TYPE": "long",  "VALUES": [0,1,2], "LABELS": ["Blue Crystal","Amethyst","Ice White"], "DEFAULT": 0 },
    { "NAME": "audioMod",   "LABEL": "Audio Mod",   "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 1.0  }
  ]
}*/

const float PI = 3.14159265;

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

// BCC lattice: A-sites at cubic corners, B-sites at body centres.
// Returns vec2(distance, type): 0=A-atom, 1=B-atom, 2=bond
vec2 sdLattice(vec3 p) {
    float cs = cellSize;
    float ar = atomRadius;
    float br = bondRadius;

    vec3 qa = mod(p + 0.5 * cs, cs) - 0.5 * cs;
    float dA = length(qa) - ar;

    vec3 qb = mod(p, cs) - 0.5 * cs;
    float dB = length(qb) - ar * 0.82;

    float d  = dA;
    float tp = 0.0;
    if (dB < d) { d = dB; tp = 1.0; }

    if (br > 0.0005) {
        float h = cs * 0.5;
        float bd = min(
            min(min(sdCapsule(qa, vec3(0.0), vec3( h,  h,  h), br),
                    sdCapsule(qa, vec3(0.0), vec3( h,  h, -h), br)),
                min(sdCapsule(qa, vec3(0.0), vec3( h, -h,  h), br),
                    sdCapsule(qa, vec3(0.0), vec3( h, -h, -h), br))),
            min(min(sdCapsule(qa, vec3(0.0), vec3(-h,  h,  h), br),
                    sdCapsule(qa, vec3(0.0), vec3(-h,  h, -h), br)),
                min(sdCapsule(qa, vec3(0.0), vec3(-h, -h,  h), br),
                    sdCapsule(qa, vec3(0.0), vec3(-h, -h, -h), br))));
        if (bd < d) { d = bd; tp = 2.0; }
    }

    return vec2(d, tp);
}

vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sdLattice(p + e.xyy).x - sdLattice(p - e.xyy).x,
        sdLattice(p + e.yxy).x - sdLattice(p - e.yxy).x,
        sdLattice(p + e.yyx).x - sdLattice(p - e.yyx).x
    ));
}

float march(vec3 ro, vec3 rd) {
    float t = 0.02;
    for (int i = 0; i < 72; i++) {
        vec2 res = sdLattice(ro + rd * t);
        if (res.x < 0.0003) return t;
        t += res.x * 0.75;
        if (t > 24.0) break;
    }
    return -1.0;
}

float calcAO(vec3 p, vec3 n) {
    float occ = 0.0, sc = 1.0;
    for (int i = 0; i < 5; i++) {
        float h = 0.05 + 0.12 * float(i);
        float d = sdLattice(p + h * n).x;
        occ += (h - d) * sc;
        sc *= 0.8;
    }
    return clamp(1.0 - 3.0 * occ, 0.0, 1.0);
}

void main() {
    vec2 uv   = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float audio = 1.0 + (audioLevel + audioBass * 0.5) * audioMod * 0.3;
    float t = TIME * speed;
    int   pal = int(palette);

    // Camera: slow spiral orbit inside lattice
    float ang  = t * 0.6;
    float ht   = sin(t * 0.25) * cellSize * 0.55;
    float rad  = cellSize * 0.85 * zoom;
    vec3  ro   = vec3(cos(ang) * rad, ht, sin(ang) * rad);
    vec3  tgt  = ro + vec3(cos(ang + 0.5), 0.08, sin(ang + 0.5));
    vec3  fwd  = normalize(tgt - ro);
    vec3  rght = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3  up   = cross(rght, fwd);
    vec3  rd   = normalize(fwd + uv.x * rght + uv.y * up);

    // Palette
    vec3 colA, colB, colBond, bgCol;
    if (pal == 0) {
        colA    = vec3(0.15, 0.45, 1.0);
        colB    = vec3(0.20, 0.75, 1.0);
        colBond = vec3(0.50, 0.85, 1.0);
        bgCol   = vec3(0.0,  0.008, 0.04);
    } else if (pal == 1) {
        colA    = vec3(0.45, 0.05, 1.0);
        colB    = vec3(0.85, 0.35, 1.0);
        colBond = vec3(0.65, 0.45, 1.0);
        bgCol   = vec3(0.015, 0.0, 0.05);
    } else {
        colA    = vec3(0.55, 0.80, 1.0);
        colB    = vec3(0.85, 0.93, 1.0);
        colBond = vec3(0.90, 0.95, 1.0);
        bgCol   = vec3(0.0,  0.015, 0.03);
    }

    float hit = march(ro, rd);
    vec3 col;

    if (hit > 0.0) {
        vec3 p   = ro + rd * hit;
        vec3 n   = calcNormal(p);
        vec2 res = sdLattice(p);
        float occ = calcAO(p, n);

        vec3 matCol = res.y < 0.5 ? colA : (res.y < 1.5 ? colB : colBond);

        // Core glow (proximity to nearest A-site)
        vec3  qa      = mod(p + 0.5 * cellSize, cellSize) - 0.5 * cellSize;
        float coreGlow = 1.0 - clamp(length(qa) / atomRadius, 0.0, 1.0);

        // fwidth-based neon iso-rings on atom surface
        float ringParam = fract(length(qa) / atomRadius * 2.5);
        float fw        = fwidth(ringParam);
        float ring      = smoothstep(fw * 2.0, 0.0, abs(ringParam - 0.5) - 0.12);

        vec3  L    = normalize(vec3(1.4,  2.0,  0.7));
        vec3  L2   = normalize(vec3(-0.9, -0.8,  1.1));
        float diff  = max(dot(n, L),  0.0);
        float diff2 = max(dot(n, L2), 0.0) * 0.22;
        vec3  R    = reflect(-L, n);
        float spec = pow(max(dot(R, -rd), 0.0), 48.0);
        float fres = pow(1.0 - abs(dot(n, -rd)), 5.0);

        col  = matCol * (diff + diff2) * occ;
        col += vec3(1.0, 0.98, 0.95) * spec * hdrGlow * 2.2;  // HDR white specular
        col += matCol * fres * 0.6 * hdrGlow;                  // fresnel rim
        col += matCol * coreGlow * 0.55 * hdrGlow * audio;     // emissive core
        col += matCol * ring * 0.45 * hdrGlow;                 // neon iso-rings

        // Distance fog
        col = mix(bgCol, col, exp(-hit * 0.055));

    } else {
        float skyGrad = 0.4 + 0.6 * max(0.0, dot(rd, vec3(0.0, 1.0, 0.0)));
        col = bgCol * skyGrad;
    }

    // Voice glitch
    if (_voiceGlitch > 0.01) {
        float g  = _voiceGlitch;
        float tt = TIME * 17.0;
        vec2  uvN = gl_FragCoord.xy / RENDERSIZE.xy;
        float band      = floor(uvN.y * mix(8.0, 40.0, g) + tt * 3.0);
        float bandNoise = fract(sin(band * 91.7 + tt) * 43758.5453);
        float scanline  = 0.95 + 0.05 * sin(uvN.y * RENDERSIZE.y * 1.5 + tt * 40.0);
        float blockX    = floor(uvN.x * 6.0);
        float blockY    = floor(uvN.y * 4.0);
        float blockNoise = fract(sin((blockX + blockY * 7.0) * 113.1 + floor(tt * 8.0)) * 43758.5453);
        float dropout   = step(1.0 - g * 0.15, blockNoise);
        vec3  glitched  = col * scanline * (1.0 - dropout);
        col = mix(col, glitched, smoothstep(0.0, 0.3, g));
    }

    gl_FragColor = vec4(col, 1.0);
}
