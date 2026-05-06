/*{
  "CATEGORIES": ["3D", "Generator", "Audio Reactive"],
  "DESCRIPTION": "Cymatic Oracle — first-person view inside an octagonal marble temple. Eight ink-veined columns ring a central water bowl whose surface ripples with Bessel cymatic modes driven by audio bass/mid/treble. A single HDR oculus beam strikes the water and throws moving caustics across column bases. Camera orbits slowly. Returns LINEAR HDR with peaks 3.0+.",
  "INPUTS": [
    { "NAME": "exposure",    "LABEL": "Exposure",       "TYPE": "float", "MIN": 0.3, "MAX": 3.0,  "DEFAULT": 1.05 },
    { "NAME": "camDist",     "LABEL": "Camera Distance","TYPE": "float", "MIN": 1.8, "MAX": 6.0,  "DEFAULT": 3.4 },
    { "NAME": "camHeight",   "LABEL": "Camera Height",  "TYPE": "float", "MIN": 0.4, "MAX": 2.5,  "DEFAULT": 1.2 },
    { "NAME": "orbitSpeed",  "LABEL": "Orbit Speed",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.10 },
    { "NAME": "beamIntensity","LABEL": "Beam Intensity","TYPE": "float", "MIN": 0.5, "MAX": 4.0,  "DEFAULT": 2.6 },
    { "NAME": "rippleAmp",   "LABEL": "Ripple Amplitude","TYPE":"float", "MIN": 0.0, "MAX": 0.18, "DEFAULT": 0.06 },
    { "NAME": "modeScale",   "LABEL": "Mode Frequency", "TYPE": "float", "MIN": 1.0, "MAX": 14.0, "DEFAULT": 6.0 },
    { "NAME": "specularPower","LABEL": "Specular Power","TYPE": "float", "MIN": 16.0,"MAX": 256.0,"DEFAULT": 96.0 },
    { "NAME": "audioReact",  "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "marbleColor", "LABEL": "Marble",         "TYPE": "color", "DEFAULT": [0.92, 0.90, 0.88, 1.0] },
    { "NAME": "veinColor",   "LABEL": "Vein Ink",       "TYPE": "color", "DEFAULT": [0.04, 0.03, 0.05, 1.0] },
    { "NAME": "waterColor",  "LABEL": "Water Tint",     "TYPE": "color", "DEFAULT": [0.05, 0.45, 0.55, 1.0] },
    { "NAME": "beamColor",   "LABEL": "Beam Color",     "TYPE": "color", "DEFAULT": [1.0, 0.82, 0.42, 1.0] },
    { "NAME": "goldColor",   "LABEL": "Gold Trim",      "TYPE": "color", "DEFAULT": [1.0, 0.78, 0.18, 1.0] },
    { "NAME": "voidColor",   "LABEL": "Void Shadow",    "TYPE": "color", "DEFAULT": [0.01, 0.005, 0.02, 1.0] }
  ]
}*/

// ═══════════════════════════════════════════════════════════════════════
//   CYMATIC ORACLE
//   Temple raymarch: 8 columns + bowl + analytical water plane.
//   Water displacement = sum of three Bessel-like radial modes driven
//   by audioBass / audioMid / audioHigh. Oculus sun beam → caustics.
//   Output: LINEAR HDR (peaks 3.0+).
// ═══════════════════════════════════════════════════════════════════════

#define MAX_STEPS 64
#define MAX_DIST  18.0
#define EPS       0.0014

const float PI = 3.14159265359;

// ─── hashing & marble veining ──────────────────────────────────────────
float hash21(vec2 p) {
    p = fract(p * vec2(91.345, 47.853));
    p += dot(p, p + 23.45);
    return fract(p.x * p.y);
}
float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}
float fbm2(vec2 p) {
    float v = 0.0, a = 0.5;
    for (int i = 0; i < 4; i++) {
        v += a * vnoise(p);
        p = p * 2.07 + vec2(11.3, 5.7);
        a *= 0.5;
    }
    return v;
}

// ─── SDFs ──────────────────────────────────────────────────────────────
float sdCylinderY(vec3 p, float r, float h) {
    vec2 d = vec2(length(p.xz) - r, abs(p.y) - h);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}
float sdRing(vec3 p, float ringR, float thickness, float halfH) {
    float ringD = abs(length(p.xz) - ringR) - thickness;
    return max(ringD, abs(p.y) - halfH);
}
float sdCappedCone(vec3 p, float r1, float r2, float h) {
    vec2 q = vec2(length(p.xz), p.y);
    vec2 k1 = vec2(r2, h);
    vec2 k2 = vec2(r2 - r1, 2.0 * h);
    vec2 ca = vec2(q.x - min(q.x, (q.y < 0.0) ? r1 : r2), abs(q.y) - h);
    vec2 cb = q - k1 + k2 * clamp(dot(k1 - q, k2) / dot(k2, k2), 0.0, 1.0);
    float s = (cb.x < 0.0 && ca.y < 0.0) ? -1.0 : 1.0;
    return s * sqrt(min(dot(ca, ca), dot(cb, cb)));
}

// Eight columns at radius R, plus floor ring + cone bowl base.
// Returns scene SDF and writes a material id (0=floor, 1=column, 2=bowl).
float sceneSDF(vec3 p, out int matId) {
    matId = 0;
    // Floor disc — large flat plane with circular cutout for water.
    float floorY = p.y + 0.0;                     // floor at y=0
    float waterCutout = length(p.xz) - 0.95;      // cylinder cutout for bowl
    float floorD = max(floorY, -waterCutout - 0.02);

    // 8 columns at radius 1.6.
    float colD = 1e6;
    float aSlice = 2.0 * PI / 8.0;
    float a = atan(p.z, p.x);
    float k = floor(a / aSlice + 0.5);
    float ang = k * aSlice;
    vec3 cp = p - vec3(cos(ang) * 1.6, 1.5, sin(ang) * 1.6);
    colD = sdCylinderY(cp, 0.18, 1.5);
    // Capital ring at top of column.
    float capD = sdRing(p - vec3(0.0, 3.0, 0.0), 1.6, 0.06, 0.05);
    colD = min(colD, capD);
    // Base ring at bottom of column.
    float baseD = sdRing(p - vec3(0.0, 0.06, 0.0), 1.6, 0.05, 0.06);
    colD = min(colD, baseD);

    // Bowl rim — thin gold ring around water at radius 0.95.
    float bowlD = sdRing(p - vec3(0.0, 0.04, 0.0), 0.95, 0.04, 0.05);

    float d = min(min(floorD, colD), bowlD);
    if (d == colD)  matId = 1;
    if (d == bowlD) matId = 2;
    return d;
}

// ─── water height — sum of three Bessel-like radial modes ─────────────
//   Each mode: cos(k_i * r - omega_i * t) * envelope
float waterHeight(vec2 xz, float audioMod, float modeFreq) {
    float r = length(xz);
    float a = atan(xz.y, xz.x);
    float bass = audioBass;
    float mid  = audioMid;
    float hi   = audioHigh;
    // Three concentric standing waves at increasing k.
    float k1 = modeFreq * 1.0;
    float k2 = modeFreq * 1.7;
    float k3 = modeFreq * 2.4;
    float w1 = 1.4, w2 = 2.1, w3 = 3.3;
    float env = exp(-r * 0.6);
    // Angular sectoring — m=4 mode (square symmetry of cymatic plate).
    float ang4 = cos(4.0 * a);
    float ang6 = cos(6.0 * a + TIME * 0.4);
    float v = 0.0;
    v += cos(k1 * r - w1 * TIME) * (0.4 + 0.6 * bass) * 0.8 * audioMod;
    v += cos(k2 * r - w2 * TIME) * ang4 * (0.3 + 0.7 * mid) * 0.55 * audioMod;
    v += cos(k3 * r - w3 * TIME) * ang6 * (0.2 + 0.8 * hi) * 0.40 * audioMod;
    return v * env;
}

// ─── water height field gradient (for normal) ─────────────────────────
vec3 waterNormal(vec2 xz, float audioMod, float modeFreq, float amp) {
    float e = 0.0035;
    float h0 = waterHeight(xz, audioMod, modeFreq) * amp;
    float hx = waterHeight(xz + vec2(e, 0.0), audioMod, modeFreq) * amp;
    float hz = waterHeight(xz + vec2(0.0, e), audioMod, modeFreq) * amp;
    vec3 n = normalize(vec3(h0 - hx, e, h0 - hz));
    return n;
}

// ─── march scene SDF ──────────────────────────────────────────────────
float marchScene(vec3 ro, vec3 rd, out int matId) {
    matId = 0;
    float d = 0.0;
    int   m = 0;
    for (int i = 0; i < MAX_STEPS; i++) {
        vec3 p = ro + rd * d;
        float s = sceneSDF(p, m);
        if (s < EPS) { matId = m; return d; }
        if (d > MAX_DIST) break;
        d += s * 0.92;
    }
    return -1.0;
}

vec3 calcNormalScene(vec3 p) {
    const vec2 e = vec2(0.0014, -0.0014);
    int dummy;
    return normalize(
        e.xyy * sceneSDF(p + e.xyy, dummy) +
        e.yyx * sceneSDF(p + e.yyx, dummy) +
        e.yxy * sceneSDF(p + e.yxy, dummy) +
        e.xxx * sceneSDF(p + e.xxx, dummy));
}

// ─── analytical ray–water-plane intersect (water plane at y = 0.10) ──
float intersectWater(vec3 ro, vec3 rd, float py) {
    if (abs(rd.y) < 1e-4) return -1.0;
    float t = (py - ro.y) / rd.y;
    return t > 0.0 ? t : -1.0;
}

void main() {
    vec2 res = RENDERSIZE;
    vec2 uv  = (isf_FragNormCoord.xy * 2.0 - 1.0)
             * vec2(res.x / res.y, 1.0);

    float audio = clamp(audioReact, 0.0, 2.0);
    float audioMod = 0.4 + 0.6 * audio;

    // Camera orbit.
    float ang = TIME * orbitSpeed;
    vec3  ro  = vec3(cos(ang) * camDist, camHeight, sin(ang) * camDist);
    vec3  ta  = vec3(0.0, 0.4, 0.0);
    vec3  fw  = normalize(ta - ro);
    vec3  ri  = normalize(cross(vec3(0.0, 1.0, 0.0), fw));
    vec3  up  = cross(fw, ri);
    vec3  rd  = normalize(fw + uv.x * ri + uv.y * up);

    // Oculus light: directly above origin, infinite-distance directional + cone beam.
    vec3 sunDir = normalize(vec3(0.0, 1.0, 0.0));
    vec3 col = voidColor.rgb;

    // March scene.
    int  matId;
    float td = marchScene(ro, rd, matId);

    // Also test water plane.
    float waterPlaneY = 0.10;
    float wd = intersectWater(ro, rd, waterPlaneY);
    bool insideBowl = false;
    vec3 waterPos;
    if (wd > 0.0) {
        waterPos = ro + rd * wd;
        if (length(waterPos.xz) < 0.93) insideBowl = true;
        else wd = -1.0;
    }

    // If water hit comes before scene hit → render water.
    bool waterHit = insideBowl && (td < 0.0 || wd < td);

    if (waterHit) {
        vec2 xz = waterPos.xz;
        vec3 wn = waterNormal(xz, audioMod, modeScale, rippleAmp * 6.0);
        // Phong specular toward oculus + Fresnel rim.
        float ndl = max(dot(wn, sunDir), 0.0);
        vec3 r   = reflect(-sunDir, wn);
        float spec = pow(max(dot(r, -rd), 0.0), specularPower);

        // Fresnel.
        float cosTh = clamp(-dot(rd, wn), 0.0, 1.0);
        float fr    = pow(1.0 - cosTh, 5.0);
        // Reflected sky/oculus = warm beam.
        vec3 reflEnv = beamColor.rgb * 1.6;
        // Refracted view = water tint deepening with travel.
        float depthFake = 0.6 + 0.4 * vnoise(xz * 4.0 + TIME * 0.1);
        vec3 refrCol = mix(waterColor.rgb * 1.4, voidColor.rgb, depthFake);

        col = mix(refrCol, reflEnv, fr * 0.85);

        // HDR specular peaks on wave crests — peaks 3.0+ linear.
        col += beamColor.rgb * spec * beamIntensity * 3.0;

        // Cymatic ridge ink — bright where the height-field gradient is high.
        float h0 = waterHeight(xz, audioMod, modeScale);
        float ridge = abs(h0);
        col += goldColor.rgb * smoothstep(0.55, 0.85, ridge) * 1.4 * audioMod;

        // Diffuse oculus contribution.
        col += beamColor.rgb * ndl * 0.25 * beamIntensity;

    } else if (td > 0.0) {
        vec3 hp = ro + rd * td;
        vec3 n  = calcNormalScene(hp);

        // Marble base color with veining.
        vec3 base = marbleColor.rgb;
        if (matId == 1) {
            // Column — vertical vein noise.
            float vein = fbm2(vec2(atan(hp.z, hp.x) * 4.0, hp.y * 1.4));
            float ink  = smoothstep(0.55, 0.78, vein);
            base = mix(marbleColor.rgb, veinColor.rgb, ink * 0.55);
        } else if (matId == 2) {
            // Bowl rim — solid gold.
            base = goldColor.rgb * 1.4;
        } else {
            // Floor — large marble tiles with mortar lines.
            vec2 g = floor(hp.xz * 2.5);
            float tileNoise = hash21(g);
            float vein = fbm2(hp.xz * 1.2 + tileNoise * 7.0);
            base = mix(marbleColor.rgb, veinColor.rgb,
                       smoothstep(0.6, 0.78, vein) * 0.4);
            // Mortar lines.
            vec2 fr2 = abs(fract(hp.xz * 2.5) - 0.5);
            float mortar = 1.0 - smoothstep(0.46, 0.49, max(fr2.x, fr2.y));
            base = mix(base, voidColor.rgb, mortar * 0.85);
        }

        // Lighting — directional oculus + ambient void.
        float ndl = max(dot(n, sunDir), 0.0);
        // Fake AO from height (lower = darker).
        float ao = clamp(hp.y * 0.35 + 0.4, 0.25, 1.0);
        col = base * (ao * 0.25 + ndl * 0.85 * beamIntensity * 0.6);

        // Caustics on column bases & floor — projected cymatic pattern
        // computed at the ray's xz from the bowl, modulated by visibility.
        if (matId == 0 || (matId == 1 && hp.y < 0.6)) {
            float caustic = waterHeight(hp.xz * 0.9, audioMod, modeScale * 0.8);
            float c = exp(-pow(caustic - 0.7, 2.0) * 28.0);
            // Beam-shaped projection — softens with distance from bowl.
            float fall = exp(-length(hp.xz) * 0.55);
            col += beamColor.rgb * c * fall * 1.3 * beamIntensity;
        }

        // Specular sheen on capital rings (matId=1 near top).
        if (matId == 1 && abs(hp.y - 3.0) < 0.1) {
            vec3 r = reflect(-sunDir, n);
            float sp = pow(max(dot(r, -rd), 0.0), specularPower * 0.5);
            col += goldColor.rgb * sp * 1.1;
        }
    } else {
        // Sky / void — far above.
        if (rd.y > 0.0) {
            // Oculus opening — bright HDR core when looking up.
            float oc = pow(max(rd.y, 0.0), 6.0);
            col = mix(voidColor.rgb, beamColor.rgb * 2.4 * beamIntensity, oc);
            // Hard disc oculus.
            float disc = smoothstep(0.92, 0.98, rd.y);
            col += beamColor.rgb * disc * 2.4 * beamIntensity;
        } else {
            col = voidColor.rgb;
        }
    }

    // ── Volumetric beam: slight air glow along sun direction ────────
    float beamAir = pow(max(dot(rd, sunDir), 0.0), 12.0);
    col += beamColor.rgb * beamAir * 0.18 * beamIntensity;

    // Vignette + exposure.
    col *= 1.0 - 0.18 * dot(uv, uv) * 0.2;
    col *= exposure;

    gl_FragColor = vec4(col, 1.0);
}
