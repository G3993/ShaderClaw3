/*{
  "DESCRIPTION": "Plasma Storm — 3D raymarched plasma orb with crackling surface turbulence and volumetric lightning arcs",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "ShaderClaw auto-improve",
  "INPUTS": [
    { "NAME": "plasmaSize",  "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.2, "MAX": 1.0, "LABEL": "Plasma Radius" },
    { "NAME": "turbulence",  "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Turbulence" },
    { "NAME": "hdrPeak",     "TYPE": "float", "DEFAULT": 2.5, "MIN": 0.5, "MAX": 4.0, "LABEL": "HDR Peak" },
    { "NAME": "rotSpeed",    "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "LABEL": "Rotation" },
    { "NAME": "audioMod",    "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "LABEL": "Audio React" },
    { "NAME": "coreColor",   "TYPE": "color", "DEFAULT": [0.3, 0.0, 1.0, 1.0], "LABEL": "Core (Violet)" },
    { "NAME": "arcColor",    "TYPE": "color", "DEFAULT": [0.0, 0.8, 1.0, 1.0], "LABEL": "Arc (Cyan)" }
  ]
}*/

// ---- Noise / FBM ----
float hash(vec3 p) {
    p = fract(p * vec3(127.1, 311.7, 74.7));
    p += dot(p, p + 19.19);
    return fract(p.x * p.y * p.z);
}

float noise3(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    vec3 u = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(mix(hash(i),             hash(i+vec3(1,0,0)), u.x),
            mix(hash(i+vec3(0,1,0)), hash(i+vec3(1,1,0)), u.x), u.y),
        mix(mix(hash(i+vec3(0,0,1)), hash(i+vec3(1,0,1)), u.x),
            mix(hash(i+vec3(0,1,1)), hash(i+vec3(1,1,1)), u.x), u.y), u.z);
}

float fbm(vec3 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * noise3(p);
        p = p * 2.1 + vec3(3.7, 1.9, 2.3);
        a *= 0.5;
    }
    return v;
}

// ---- Rotation ----
mat3 rotY(float a) {
    float c = cos(a), s = sin(a);
    return mat3(c, 0, s, 0, 1, 0, -s, 0, c);
}

// ---- Plasma sphere SDF (surface turbulence) ----
float sdPlasma(vec3 p, float t) {
    float turbNoise = fbm(p * 3.0 + t * 1.3);
    return length(p) - plasmaSize * (1.0 + turbulence * 0.2 * turbNoise);
}

// ---- Lightning bolt SDF (capsule + turbulence noise) ----
float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h) - r;
}

float sdBolt(vec3 p, vec3 a, vec3 b, float t) {
    // Add jagged turbulence along the bolt
    float turbNoise = fbm(p * 12.0 + t * 3.0) * 0.04;
    return sdCapsule(p, a, b, 0.015) - turbNoise * turbulence;
}

// ---- Normal ----
vec3 calcNormal(vec3 p, float t) {
    float e = 0.002;
    return normalize(vec3(
        sdPlasma(p + vec3(e,0,0), t) - sdPlasma(p - vec3(e,0,0), t),
        sdPlasma(p + vec3(0,e,0), t) - sdPlasma(p - vec3(0,e,0), t),
        sdPlasma(p + vec3(0,0,e), t) - sdPlasma(p - vec3(0,0,e), t)
    ));
}

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 ndc = (uv - 0.5) * vec2(aspect, 1.0) * 2.0;

    // Camera
    vec3 ro = vec3(0.0, 0.0, 2.5);
    vec3 rd = normalize(vec3(ndc, -1.8));
    rd = rotY(TIME * rotSpeed * 0.15) * rd;

    // Audio modulator
    float audio = 1.0 + audioMod * 0.4;

    vec3 col = vec3(0.0);
    float tDist = 0.0;
    bool hit = false;
    vec3 hitP;

    // ---- Raymarching: 64 steps ----
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + rd * tDist;
        float d = sdPlasma(p, TIME);
        if (d < 0.001) {
            hit = true;
            hitP = p;
            break;
        }
        tDist += d * 0.7;
        if (tDist > 6.0) break;
    }

    if (hit) {
        vec3 n = calcNormal(hitP, TIME);
        float diff = clamp(dot(n, -rd), 0.0, 1.0);

        // Surface colour: FBM-modulated blend of core and arc
        float surfFBM = fbm(hitP * 5.0 + TIME * 2.0);
        vec3 surfCol = mix(coreColor.rgb, arcColor.rgb, surfFBM);
        col = surfCol * (diff * 0.6 + 0.4) * hdrPeak * audio;

        // White-hot core center
        float centerDot = clamp(1.0 - length(hitP) / plasmaSize, 0.0, 1.0);
        col += vec3(2.0, 2.0, 2.0) * pow(centerDot, 4.0) * hdrPeak * 0.5 * audio;

        // fwidth AA on plasma edge
        float dSurf = sdPlasma(hitP, TIME);
        float fw = fwidth(dSurf);
        float edgeMask = smoothstep(fw * 2.0, 0.0, abs(dSurf));
        col = mix(col, vec3(0.0), edgeMask * 0.4);
    } else {
        // Background: deep black storm sky
        col = vec3(0.01, 0.005, 0.02);
    }

    // ---- Volumetric corona glow (background receives plasma light) ----
    // March closest approach to sphere center for glow
    vec3 closestP = ro + rd * clamp(dot(-ro, rd), 0.0, 8.0);
    float coronaDist = sdPlasma(closestP, TIME);
    float corona = exp(-max(0.0, coronaDist) * 6.0) * hdrPeak * 0.5 * audio;
    col += coreColor.rgb * corona * (hit ? 0.0 : 1.0);

    // ---- Lightning bolts: 4 arcs from sphere surface to frame edges ----
    // Hardcoded arc endpoints (start near sphere surface, end at frame corners)
    vec3 boltStarts[4];
    vec3 boltEnds[4];
    boltStarts[0] = vec3( plasmaSize * 0.7,  plasmaSize * 0.7,  0.0);
    boltStarts[1] = vec3(-plasmaSize * 0.7,  plasmaSize * 0.5,  0.1);
    boltStarts[2] = vec3( plasmaSize * 0.5, -plasmaSize * 0.8, -0.1);
    boltStarts[3] = vec3(-plasmaSize * 0.4, -plasmaSize * 0.6,  0.2);
    boltEnds[0] = vec3( 1.4,  1.2, 0.3);
    boltEnds[1] = vec3(-1.5,  1.3, 0.2);
    boltEnds[2] = vec3( 1.3, -1.4, 0.1);
    boltEnds[3] = vec3(-1.2, -1.5, 0.3);

    // Rotate bolts with scene
    mat3 rY = rotY(TIME * rotSpeed * 0.2);

    for (int i = 0; i < 4; i++) {
        vec3 bStart = rY * boltStarts[i];
        vec3 bEnd   = rY * boltEnds[i];

        // March bolt: sample distance at multiple points along ray
        float boltGlow = 0.0;
        for (int j = 0; j < 32; j++) {
            float bt = float(j) / 31.0 * 5.0;
            vec3 bp = ro + rd * bt;
            float bd = sdBolt(bp, bStart, bEnd, TIME);
            boltGlow += exp(-max(0.0, bd) * 80.0) * 0.1;
        }

        // Arc brightness flicker
        float flicker = 0.7 + 0.3 * sin(TIME * 7.3 + float(i) * 1.7);
        col += arcColor.rgb * boltGlow * hdrPeak * 1.5 * audio * flicker;
    }

    gl_FragColor = vec4(col, 1.0);
}
