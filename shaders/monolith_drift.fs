/*{
  "DESCRIPTION":"Monolith Cubism — bold 3D monoliths drift through space with cubist fragmentation, gravitational armature lines, vivid color control, full audio reactivity, and complete compositional control.",
  "CREDIT":"ShaderClaw3 × Analytic Cubism Fusion",
  "CATEGORIES":["Generator","3D","Atmospheric","Audio Reactive","Art Movement"],
  "INPUTS":[
    {"NAME":"driftSpeed","LABEL":"Drift Speed","TYPE":"float","DEFAULT":0.22,"MIN":0.0,"MAX":2.0},
    {"NAME":"depthRange","LABEL":"Depth Range","TYPE":"float","DEFAULT":8.0,"MIN":1.0,"MAX":20.0},
    {"NAME":"nearZ","LABEL":"Near Distance","TYPE":"float","DEFAULT":0.3,"MIN":0.05,"MAX":2.0},
    {"NAME":"spread","LABEL":"Spread","TYPE":"float","DEFAULT":0.6,"MIN":0.0,"MAX":2.0},
    {"NAME":"fogDensity","LABEL":"Fog Density","TYPE":"float","DEFAULT":0.3,"MIN":0.0,"MAX":2.0},
    {"NAME":"rimGlow","LABEL":"Rim Glow","TYPE":"float","DEFAULT":1.2,"MIN":0.0,"MAX":4.0},
    {"NAME":"slabCount","LABEL":"Slab Count","TYPE":"float","DEFAULT":18.0,"MIN":4.0,"MAX":32.0},
    {"NAME":"slabAspect","LABEL":"Slab Aspect","TYPE":"float","DEFAULT":0.22,"MIN":0.05,"MAX":1.0},
    {"NAME":"slabHeight","LABEL":"Slab Height","TYPE":"float","DEFAULT":0.38,"MIN":0.05,"MAX":1.2},
    {"NAME":"cornerRadius","LABEL":"Corner Radius","TYPE":"float","DEFAULT":0.015,"MIN":0.0,"MAX":0.1},
    {"NAME":"colorMode","LABEL":"Color Mode","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"accentA","LABEL":"Accent A","TYPE":"color","DEFAULT":[1.0,1.0,1.0,1.0]},
    {"NAME":"accentB","LABEL":"Accent B","TYPE":"color","DEFAULT":[0.0,0.0,0.0,1.0]},
    {"NAME":"hueCycle","LABEL":"Hue Cycle Speed","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":2.0},
    {"NAME":"saturation","LABEL":"Saturation","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0},
    {"NAME":"brightness","LABEL":"Brightness","TYPE":"float","DEFAULT":1.0,"MIN":0.2,"MAX":3.0},
    {"NAME":"contrast","LABEL":"Contrast","TYPE":"float","DEFAULT":1.0,"MIN":0.5,"MAX":3.0},
    {"NAME":"armatureCount","LABEL":"Armature Lines","TYPE":"float","DEFAULT":12.0,"MIN":0.0,"MAX":24.0},
    {"NAME":"armatureWeight","LABEL":"Line Weight","TYPE":"float","DEFAULT":0.0022,"MIN":0.0005,"MAX":0.008},
    {"NAME":"armatureBleed","LABEL":"Chalky Bleed","TYPE":"float","DEFAULT":0.6,"MIN":0.0,"MAX":1.0},
    {"NAME":"armatureBrightness","LABEL":"Line Brightness","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":4.0},
    {"NAME":"cubeCount","LABEL":"Cube Count","TYPE":"float","DEFAULT":6.0,"MIN":0.0,"MAX":12.0},
    {"NAME":"cubeSize","LABEL":"Cube Size","TYPE":"float","DEFAULT":0.045,"MIN":0.005,"MAX":0.15},
    {"NAME":"cubeSpin","LABEL":"Cube Spin","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":4.0},
    {"NAME":"gravityStrength","LABEL":"Gravity Strength","TYPE":"float","DEFAULT":0.85,"MIN":0.0,"MAX":3.0},
    {"NAME":"orbitalSpeed","LABEL":"Orbital Speed","TYPE":"float","DEFAULT":0.35,"MIN":0.0,"MAX":2.0},
    {"NAME":"compositionSeed","LABEL":"Composition Seed","TYPE":"float","DEFAULT":7.0,"MIN":0.0,"MAX":80.0},
    {"NAME":"fragmentMix","LABEL":"Cubist Fragment Mix","TYPE":"float","DEFAULT":0.5,"MIN":0.0,"MAX":1.0},
    {"NAME":"planeCount","LABEL":"Plane Count","TYPE":"float","DEFAULT":7.0,"MIN":0.0,"MAX":12.0},
    {"NAME":"planeAlpha","LABEL":"Plane Alpha","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":1.0},
    {"NAME":"planeSize","LABEL":"Plane Size","TYPE":"float","DEFAULT":0.18,"MIN":0.05,"MAX":0.5},
    {"NAME":"sway","LABEL":"Sway Amount","TYPE":"float","DEFAULT":0.04,"MIN":0.0,"MAX":0.3},
    {"NAME":"vignette","LABEL":"Vignette","TYPE":"float","DEFAULT":0.4,"MIN":0.0,"MAX":1.0},
    {"NAME":"randomness","LABEL":"Randomness","TYPE":"float","DEFAULT":0.7,"MIN":0.0,"MAX":1.0},
    {"NAME":"audioReact","LABEL":"Audio Reactivity","TYPE":"float","DEFAULT":1.0,"MIN":0.0,"MAX":3.0},
    {"NAME":"inputImage","LABEL":"Your Image","TYPE":"image"},
    {"NAME":"texMix","LABEL":"Image Amount","TYPE":"float","DEFAULT":0.0,"MIN":0.0,"MAX":1.0}
  ]
}*/

// ═══════════════════════════════════════════════════════════
//  UTILITY
// ═══════════════════════════════════════════════════════════

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 ip = floor(p), fp = fract(p);
    fp = fp * fp * (3.0 - 2.0 * fp);
    float a = hash21(ip);
    float b = hash21(ip + vec2(1.0, 0.0));
    float c = hash21(ip + vec2(0.0, 1.0));
    float d = hash21(ip + vec2(1.0, 1.0));
    return mix(mix(a, b, fp.x), mix(c, d, fp.x), fp.y);
}

// HSV → RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Signed distance to rounded box
float sdRoundBox(vec2 p, vec2 b, float r) {
    vec2 d = abs(p) - b + r;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - r;
}

// ═══════════════════════════════════════════════════════════
//  COLOR PALETTE
// ═══════════════════════════════════════════════════════════

vec3 vivid(float k, float hShift, float sat) {
    float h = fract(k + hShift);
    // Split: low sat = b&w monolith tones, high sat = vivid hues
    float bwVal = mix(0.05, 0.95, k);
    vec3 bw = vec3(bwVal);
    vec3 vivCol = hsv2rgb(vec3(h, 0.85, 0.9));
    return mix(bw, vivCol, sat);
}

// ═══════════════════════════════════════════════════════════
//  GRAVITY
// ═══════════════════════════════════════════════════════════

vec2 gravityBend(vec2 p, float strength) {
    vec2 r = p - vec2(0.5);
    float d = length(r) + 1e-3;
    float pull = strength * 0.045 / (d * d + 0.04);
    return -normalize(r) * pull;
}

float gravityPotential(vec2 p) {
    float d = length(p - vec2(0.5)) + 1e-3;
    return 1.0 / (d + 0.12);
}

// ═══════════════════════════════════════════════════════════
//  CUBE ORBITS — Kepler-like ellipses
// ═══════════════════════════════════════════════════════════

vec3 cubeOrbit(float fi, float t, float ospeed, float rng) {
    float a   = 0.15 + hash11(fi * 1.7) * (0.15 + rng * 0.2);
    float e   = 0.10 + hash11(fi * 3.1) * (0.3 + rng * 0.4);
    float phi = hash11(fi * 5.9) * 6.2832;
    float tilt= (hash11(fi * 7.3) - 0.5) * 1.8 * (0.5 + rng * 0.5);
    float w   = ospeed * pow(a, -1.5);
    float th  = phi + t * w;
    vec2 op   = vec2(a * cos(th), a * (1.0 - e) * sin(th));
    float ct = cos(tilt), st = sin(tilt);
    vec2 sp;
    sp.x = op.x;
    sp.y = op.y * ct;
    float z = op.y * st;
    return vec3(sp.x, sp.y, z);
}

// ═══════════════════════════════════════════════════════════
//  ARMATURE — gravity-bent charcoal lines
// ═══════════════════════════════════════════════════════════

float armatureField(vec2 uv, float seed, int N, float weight,
                    float bleed, float treble, float gStr, float rng) {
    float ink = 0.0;
    float chalky = 0.0;
    float jitter = 0.0006 + treble * 0.002 + rng * 0.001;
    vec2 bent = uv + gravityBend(uv, gStr) * (0.6 + rng * 0.8);

    for (int i = 0; i < 24; i++) {
        if (i >= N) break;
        float fi  = float(i) + seed * 1.731;
        float cluster = hash11(fi * 3.17);
        float baseAng;
        if      (cluster < 0.34) baseAng = 1.5708;
        else if (cluster < 0.67) baseAng = 1.5708 + 0.95;
        else                     baseAng = 1.5708 - 0.95;
        float ang = baseAng + (hash11(fi * 7.91) - 0.5) * (0.36 + rng * 1.2);
        ang += 0.06 * sin(TIME * 0.05 + fi * 0.7);

        float off = (hash11(fi * 11.13) - 0.5) * (0.8 + rng * 0.6);
        off += jitter * sin(TIME * 0.7 + fi * 11.1);

        vec2 n = vec2(cos(ang), sin(ang));
        float d = abs(dot(bent - 0.5, n) - off);

        vec2 along = vec2(-n.y, n.x);
        float tt  = dot(bent - 0.5, along);
        float t0 = (hash11(fi * 17.3) - 0.5) * 0.9;
        float t1 = t0 + 0.2 + hash11(fi * 19.1) * (0.4 + rng * 0.4);
        float ends = smoothstep(0.06, 0.0, max(t0 - tt, tt - t1));

        float core = smoothstep(weight, 0.0, d) * ends;
        float halo = smoothstep(weight * 7.0, weight * 1.5, d) * ends * 0.5;
        ink    = max(ink, core);
        chalky = max(chalky, halo);
    }
    return ink + chalky * bleed;
}

// ═══════════════════════════════════════════════════════════
//  CUBIST PLANES
// ═══════════════════════════════════════════════════════════

void evalPlane(vec2 uv, float fi, float aspect, float pSize, float bass,
               float rng, float hShift, float sat,
               out float inside, out vec2 local, out vec2 halfSize, out vec3 col) {
    vec2 raw = vec2(hash11(fi * 1.31), hash11(fi * 2.97 + 4.7));
    vec2 ctr = mix(raw, vec2(0.5), 0.4 + rng * 0.3);
    ctr += 0.05 * vec2(sin(TIME * 0.18 + fi), cos(TIME * 0.13 + fi * 1.7));

    float rotC = hash11(fi * 5.7);
    float rot;
    if      (rotC < 0.34) rot =  0.0;
    else if (rotC < 0.67) rot =  0.95;
    else                  rot = -0.95;
    rot += (hash11(fi * 13.9) - 0.5) * (0.38 + rng * 1.5);

    float sH = hash11(fi * 7.13);
    float sA = hash11(fi * 9.71);
    halfSize = vec2(pSize * (0.50 + sH * 0.85), pSize * (0.30 + sA * 0.95));
    halfSize *= 1.0 + bass * 0.1;

    float ca = cos(-rot), sa = sin(-rot);
    vec2 d = uv - ctr; d.x *= aspect;
    local = vec2(ca*d.x - sa*d.y, sa*d.x + ca*d.y);
    vec2 q = abs(local) - halfSize;
    float sd = max(q.x, q.y);
    inside = smoothstep(0.003, -0.001, sd);
    col = vivid(hash11(fi * 23.3), hShift, sat);
}

// ═══════════════════════════════════════════════════════════
//  MAIN
// ═══════════════════════════════════════════════════════════

void main() {
    vec2 uv = isf_FragNormCoord.xy;
    float aspect = RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Audio
    float bass   = clamp(audioBass,  0.0, 1.0) * audioReact;
    float mid    = clamp(audioMid,   0.0, 1.0) * audioReact;
    float treble = clamp(audioHigh,  0.0, 1.0) * audioReact;
    float lvl    = clamp(audioLevel, 0.0, 1.0) * audioReact;

    // Randomness & seeds
    float rng     = randomness;
    float hShift  = fract(hueCycle * TIME * 0.05 + compositionSeed * 0.013);
    float sat     = saturation;

    // Gentle sway
    float swayAmt = sway + bass * 0.02;
    {
        vec2 c = uv - 0.5;
        float a = 0.025 * sin(TIME * 0.08) * (1.0 + swayAmt * 5.0);
        float ca = cos(a), sa = sin(a);
        c = vec2(ca*c.x - sa*c.y, sa*c.x + ca*c.y);
        c.y += sin(TIME * 0.05) * swayAmt;
        uv = c + 0.5;
    }

    // Centred UV for monolith layer
    vec2 uvC = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Gravity
    float gStr = gravityStrength * (1.0 + 0.3 * bass);

    // ── ACCENT COLORS ─────────────────────────────────────
    vec3 aA = accentA.rgb;
    vec3 aB = accentB.rgb;
    // In B&W mode sat=0, colorMode blends between accent pair
    float bwBlend = colorMode; // 0=pure accents, 1=full vivid palette

    // ── 1. BACKGROUND ─────────────────────────────────────
    // Dark near-black with subtle paper grain
    vec3 paperCol = vec3(0.07, 0.07, 0.07);
    float grain = vnoise(uv * vec2(420.0, 380.0));
    paperCol *= 0.92 + 0.08 * grain;
    // Accent-tinted subtle floor glow
    vec2 uvS = (uv - 0.5);
    float floorG = smoothstep(0.0, -0.45, uvS.y);
    paperCol += aA * 0.04 * floorG + aB * 0.02 * floorG;

    vec3 col = paperCol;

    // Vignette
    float vig = smoothstep(0.2, 0.95, length(uv - 0.5));
    col *= 1.0 - vig * vignette;

    // ── 2. CUBIST PLANES ──────────────────────────────────
    int PN = int(planeCount + 0.5);
    for (int i = 0; i < 12; i++) {
        if (i >= PN) break;
        float fi = float(i) + compositionSeed * 3.71;
        float inside; vec2 local; vec2 halfSize; vec3 planeC;
        evalPlane(uv, fi, aspect, planeSize, bass, rng, hShift, sat,
                  inside, local, halfSize, planeC);
        if (inside < 0.001) continue;

        // Shade the plane
        float ldJ  = 2.35 + (hash11(fi * 31.7) - 0.5) * 0.7;
        vec2 lvec  = vec2(cos(ldJ), sin(ldJ));
        float shade = dot(local / max(halfSize, vec2(1e-4)), lvec) * 0.5 + 0.5;
        vec3 planeTint = mix(mix(aA, aB, hash11(fi * 0.37)), planeC, bwBlend);
        planeTint *= (0.45 + 0.55 * shade);

        // Image sampling
        bool hasImg = (texMix > 0.0);
        if (hasImg) {
            float theta = (hash11(fi * 5.7) < 0.34) ? 0.0
                        : (hash11(fi * 5.7) < 0.67) ? 0.95 : -0.95;
            theta += (hash11(fi * 13.9) - 0.5) * 0.38;
            float scl2 = 0.7 + 0.6 * hash11(fi * 41.7);
            vec2 jit2  = vec2(hash11(fi * 53.1) - 0.5,
                              hash11(fi * 67.3) - 0.5) * 0.30;
            vec2 q2 = uv - 0.5;
            float cs2 = cos(theta), sn2 = sin(theta);
            q2 = vec2(cs2*q2.x - sn2*q2.y, sn2*q2.x + cs2*q2.y);
            q2 *= scl2;
            vec2 sampUV = mod(q2 + 0.5 + jit2, vec2(1.0));
            vec3 imgS = IMG_NORM_PIXEL(inputImage, sampUV).rgb;
            planeTint = mix(planeTint, imgS * planeTint * 1.4, texMix);
        }

        float pulse = 1.0 + 0.2 * bass * sin(TIME * 2.7 + fi * 1.3);
        float alpha = clamp(planeAlpha * pulse * inside * fragmentMix, 0.0, 0.95);
        col = mix(col, planeTint, alpha);
    }

    // ── 3. DRIFTING 3D MONOLITHS ──────────────────────────
    float bassPulse = 1.0 + bass * 0.6;
    int SN = int(slabCount + 0.5);

    for (int i = 0; i < 32; i++) {
        if (i >= SN) break;
        float fi = float(i);

        // Recycle depth with audio modulation
        float z = mod(fi / float(SN) - TIME * driftSpeed * (1.0 + bass * 0.6), 1.0);
        z = z * depthRange + nearZ;
        float scale = 1.0 / z;

        // Stable random position per slab
        float rx = hash11(fi + compositionSeed * 0.1) * 2.0 - 1.0;
        float ry = (hash11(fi + compositionSeed * 0.1 + 7.0) * 2.0 - 1.0) * 0.45;
        // Add extra randomness
        rx += (hash11(fi * 33.7 + compositionSeed) - 0.5) * rng * 0.8;
        ry += (hash11(fi * 19.3 + compositionSeed) - 0.5) * rng * 0.4;
        vec2 off = vec2(rx, ry) * spread;

        // Slab-specific tilt angle for 3D feel
        float tiltAng = (hash11(fi * 41.1) - 0.5) * rng * 0.6
                      + sin(TIME * 0.07 + fi * 2.1) * 0.04;
        float ct = cos(tiltAng), st = sin(tiltAng);

        // Parallax
        vec2 p = uvC - off * scale * 0.35;
        // Apply tilt rotation to sample point
        p = vec2(ct * p.x - st * p.y, st * p.x + ct * p.y);

        // Monolith half-extents with user control
        float hw = slabAspect * scale;
        float hh = slabHeight * scale;
        vec2 he = vec2(hw, hh);
        float cr = cornerRadius * scale;
        float d = sdRoundBox(p, he, cr);

        // Fog
        float fog = exp(-z * fogDensity);

        // AA edge
        float aa = 1.5 / min(RENDERSIZE.x, RENDERSIZE.y);
        float body = smoothstep(aa, -aa, d);
        float rimW = 0.006 * scale + aa;
        float rim  = smoothstep(rimW, 0.0, abs(d));

        // Depth shading
        float shade = mix(0.08, 0.65, clamp(scale * 0.55, 0.0, 1.0));

        // Per-slab color using accent pair + vivid palette
        float kk = hash11(fi * 17.7 + compositionSeed);
        vec3 slabAccA = mix(aA, aB, kk);
        vec3 slabVivid = vivid(kk, hShift + fi * 0.037, sat);
        vec3 slabAccent = mix(slabAccA, slabVivid, bwBlend);

        // Image texture on slab face
        if (texMix > 0.0) {
            vec2 fuv = p / max(he, vec2(1e-4));
            fuv = fuv * 0.5 + 0.5;
            vec3 img = IMG_NORM_PIXEL(inputImage, clamp(fuv, 0.0, 1.0)).rgb;
            slabAccent = mix(slabAccent, img * slabAccent * 1.3, texMix);
        }

        vec3 bodyCol = slabAccent * shade * bassPulse;
        bodyCol *= 1.0 + mid * 0.2;

        // Shimmer on rim — more intense with randomness
        float shimmer = 1.0 + treble * (0.8 + rng * 0.8)
                      * (0.5 + 0.5 * sin(TIME * 2.0 + fi * 1.7));
        vec3 rimColor = slabAccent * rimGlow * shimmer;
        // Hot rim punch
        rimColor += slabAccent * rimGlow * 0.5 * smoothstep(0.5, 1.0, shimmer);

        // Extra vivid face highlight stripe — 3D bevel feel
        float bevelU = abs(p.x / max(hw, 1e-4));
        float bevel  = smoothstep(0.7, 0.9, bevelU) * body;
        bodyCol = mix(bodyCol, bodyCol * 2.5 + slabAccent * 0.3, bevel * 0.4);

        // Subtle side-face darkening
        float sideShade = smoothstep(0.85, 1.0, abs(p.y / max(hh, 1e-4)));
        bodyCol = mix(bodyCol, bodyCol * 0.3, sideShade * body);

        vec3 slabCol = bodyCol * body + rimColor * rim;
        float alpha  = clamp(body + rim * 0.9, 0.0, 1.0) * fog;
        col = mix(col, slabCol, alpha);
    }

    // ── 4. ORBITING CUBES ─────────────────────────────────
    int CN = int(cubeCount + 0.5);
    for (int k = 0; k < 12; k++) {
        if (k >= CN) break;
        float fk = float(k) + compositionSeed * 1.117;
        vec3 orb = cubeOrbit(fk, TIME, orbitalSpeed, rng);
        vec2 cuv = orb.xy / vec2(aspect, 1.0) + 0.5;

        float pot = gravityPotential(cuv) * gStr;
        vec2 radial2D = normalize(vec2(0.5) - cuv + 1e-5);

        float baseH = cubeSize * (0.8 + hash11(fk * 17.3) * 0.6);
        baseH *= 1.0 + 0.1 * bass;
        float tide = clamp(pot * 0.18, 0.0, 0.65);

        float spinAng = TIME * cubeSpin * (0.18 + 0.12 * hash11(fk * 21.1))
                      + fk + treble * 0.5;
        float depthFade = clamp(0.5 + orb.z * 0.6, 0.2, 1.0);

        vec2 dq = uv - cuv;
        dq.x *= aspect;
        float pr = dot(dq, radial2D);
        vec2  pn = dq - radial2D * pr;
        dq = radial2D * (pr * (1.0 + tide)) + pn * (1.0 - tide * 0.5);
        float cs2 = cos(-spinAng), sn2 = sin(-spinAng);
        vec2 lp = vec2(cs2*dq.x - sn2*dq.y, sn2*dq.x + cs2*dq.y);

        vec2 qq = abs(lp) - vec2(baseH);
        float sd = max(qq.x, qq.y);
        float inside = smoothstep(0.002, -0.002, sd);
        if (inside < 0.001) continue;

        float faceK = (abs(lp.x) > abs(lp.y)) ? sign(lp.x) * 0.5 + 0.5
                                               : sign(lp.y) * 0.3 + 0.6;
        vec3 cubeBaseC = vivid(hash11(fk * 29.7), hShift + fk * 0.11, sat);
        vec3 cubeAccC  = mix(aA, aB, hash11(fk * 0.53));
        vec3 cubeCol   = mix(cubeAccC, cubeBaseC, bwBlend);
        vec3 base = cubeCol * (0.3 + 0.7 * faceK) * depthFade;

        // Hot rim on cube edges
        float edge = smoothstep(baseH * 0.9, baseH, max(abs(lp.x), abs(lp.y)));
        base += mix(cubeAccC, cubeBaseC, bwBlend) * edge * rimGlow * 0.6;
        base *= 1.0 + 0.2 * bass * sin(TIME * 3.1 + fk * 2.0);

        col = mix(col, base, clamp(inside * 0.95 * fragmentMix, 0.0, 0.95));
    }

    // ── 5. ARMATURE LINES ─────────────────────────────────
    int AN = int(armatureCount + 0.5);
    if (AN > 0) {
        float arm = armatureField(uv, compositionSeed * 0.717,
                                  AN, armatureWeight, armatureBleed,
                                  treble, gStr, rng);
        // Core is white/accent; bleed is dark
        vec3 inkDark = mix(aB, vec3(0.0), 0.7);
        vec3 inkHot  = mix(aA, vivid(hShift, hShift, sat), bwBlend);
        inkHot *= armatureBrightness;
        float armCore = smoothstep(0.6, 1.0, arm);
        col = mix(col, inkDark, clamp(arm * (1.0 - armCore), 0.0, 1.0));
        col = mix(col, inkHot,  armCore * armatureBrightness * 0.7);
        col += inkHot * armCore * 0.6 * armatureBrightness;
    }

    // ── 6. AUDIO BREATH ───────────────────────────────────
    col *= 0.93 + 0.08 * lvl;

    // ── 7. BRIGHTNESS / CONTRAST ──────────────────────────
    col = (col - 0.5) * contrast + 0.5;
    col *= brightness;

    // ── 8. TONEMAP + GAMMA ────────────────────────────────
    col = col / (1.0 + col);
    col = pow(max(col, vec3(0.0)), vec3(0.4545));

    gl_FragColor = vec4(col, 1.0);
}