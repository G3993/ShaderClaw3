/*{
  "DESCRIPTION": "Neon City Night — 3D raymarched cyberpunk streetscape. Building box SDFs, HDR neon signs, wet ground reflections, volumetric fog, and rain streaks. Audio-reactive neon brightness.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"speed",       "LABEL":"Camera Speed","TYPE":"float","DEFAULT":0.4, "MIN":0.0,"MAX":2.0},
    {"NAME":"fogDensity",  "LABEL":"Fog Density", "TYPE":"float","DEFAULT":0.03,"MIN":0.0,"MAX":0.15},
    {"NAME":"hdrPeak",     "LABEL":"HDR Peak",    "TYPE":"float","DEFAULT":2.5, "MIN":1.0,"MAX":4.0},
    {"NAME":"audioReact",  "LABEL":"Audio React", "TYPE":"float","DEFAULT":0.6, "MIN":0.0,"MAX":2.0},
    {"NAME":"rainDensity", "LABEL":"Rain Density","TYPE":"float","DEFAULT":0.3, "MIN":0.0,"MAX":1.0}
  ]
}*/

// ── constants ──────────────────────────────────────────────────────────────
const float PI       = 3.14159265359;
const int   STEPS    = 64;
const float MAX_D    = 60.0;
const float SURF_EPS = 0.01;
const float GROUND_Y = -1.5;

// ── hash helpers ───────────────────────────────────────────────────────────
float hash1(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash2(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// ── SDF primitives ─────────────────────────────────────────────────────────
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// ── Building layout — 8 buildings on alternating sides of a central street ─
// Returns (dist, buildingID)
vec2 buildingsSDF(vec3 p) {
    float best   = 1e9;
    float bestID = -1.0;

    for (int i = 0; i < 8; i++) {
        float fi     = float(i);
        // deterministic building parameters from index
        float bWidth  = 1.0 + hash1(fi * 3.71) * 1.2;
        float bHeight = 2.0 + hash1(fi * 7.13) * 6.0;
        float bDepth  = 2.0 + hash1(fi * 11.3) * 2.5;
        float bZ      = fi * 7.0;         // spread along Z
        float side    = (mod(fi, 2.0) < 0.5) ? -1.0 : 1.0;
        float bX      = side * (2.2 + hash1(fi * 5.19) * 0.8);

        vec3  bCentre = vec3(bX, GROUND_Y + bHeight * 0.5, bZ);
        vec3  bSize   = vec3(bWidth * 0.5, bHeight * 0.5, bDepth * 0.5);
        float d       = sdBox(p - bCentre, bSize);
        if (d < best) { best = d; bestID = fi; }
    }
    return vec2(best, bestID);
}

// ── Neon sign SDF — thin horizontal slab on a building facade ─────────────
// Returns (dist, signID) where signID encodes colour
vec2 neonSignsSDF(vec3 p) {
    float best   = 1e9;
    float bestID = -1.0;

    for (int i = 0; i < 8; i++) {
        float fi     = float(i);
        float bWidth  = 1.0 + hash1(fi * 3.71) * 1.2;
        float bHeight = 2.0 + hash1(fi * 7.13) * 6.0;
        float bDepth  = 2.0 + hash1(fi * 11.3) * 2.5;
        float bZ      = fi * 7.0;
        float side    = (mod(fi, 2.0) < 0.5) ? -1.0 : 1.0;
        float bX      = side * (2.2 + hash1(fi * 5.19) * 0.8);

        // sign height: somewhere in upper half of building
        float signH   = GROUND_Y + bHeight * (0.55 + hash1(fi * 17.3) * 0.35);
        // sign is on the inward face of the building (facing street)
        float faceZ   = bZ - bDepth * 0.5 - 0.02;
        vec3  sCentre = vec3(bX, signH, faceZ);
        vec3  sSize   = vec3(bWidth * 0.45, 0.06, 0.01);

        float d = sdBox(p - sCentre, sSize);
        if (d < best) { best = d; bestID = fi; }
    }
    return vec2(best, bestID);
}

// ── Neon sign colour from ID ───────────────────────────────────────────────
vec3 neonColor(float id, float peak) {
    int kind = int(mod(id, 3.0));
    if (kind == 0) return vec3(1.0, 0.0, 0.5) * peak;        // pink
    if (kind == 1) return vec3(0.0, 1.0, 0.8) * (peak * 0.9); // cyan
    return              vec3(1.0, 0.7, 0.0) * (peak * 0.8);  // gold
}

// ── Ground plane distance ──────────────────────────────────────────────────
float groundSDF(vec3 p) {
    return p.y - GROUND_Y;
}

// ── Full scene SDF — returns (dist, typeID) ────────────────────────────────
// typeID: 0=buildings, 1=neon signs, 2=ground
vec2 sceneSDF(vec3 p) {
    vec2  bld  = buildingsSDF(p);
    vec2  sgn  = neonSignsSDF(p);
    float gnd  = groundSDF(p);

    if (bld.x < sgn.x && bld.x < gnd) return vec2(bld.x, 0.0 + bld.y * 0.001);
    if (sgn.x < gnd)                   return vec2(sgn.x, 10.0 + sgn.y * 0.001);
    return vec2(gnd, 2.0);
}

// ── finite-difference normal ───────────────────────────────────────────────
vec3 calcNormal(vec3 p) {
    const float e = 0.005;
    return normalize(vec3(
        sceneSDF(p + vec3( e,0,0)).x - sceneSDF(p - vec3( e,0,0)).x,
        sceneSDF(p + vec3(0, e,0)).x - sceneSDF(p - vec3(0, e,0)).x,
        sceneSDF(p + vec3(0,0, e)).x - sceneSDF(p - vec3(0,0, e)).x
    ));
}

// ── main ───────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio     = 1.0 + audioLevel * audioReact;
    float bassAudio = 1.0 + audioBass  * audioReact;
    float highAudio = 1.0 + audioHigh  * audioReact;

    // camera travels along +Z, street-level, slight upward tilt
    float camZ = TIME * speed;
    vec3  ro   = vec3(0.0, 0.3, -camZ);
    // look slightly upward toward buildings
    vec3  lookAt = ro + vec3(0.0, 0.4, 1.0);
    vec3  fwd    = normalize(lookAt - ro);
    vec3  right  = normalize(cross(fwd, vec3(0.0, 1.0, 0.0)));
    vec3  up     = cross(right, fwd);
    vec3  rd     = normalize(fwd + uv.x * right * 0.65 + uv.y * up * 0.65);

    // ── raymarching ──────────────────────────────────────────────────────
    float t      = 0.1;
    float typeID = -1.0;
    float tHit   = MAX_D;
    float fog    = 0.0;

    for (int i = 0; i < STEPS; i++) {
        vec3  p  = ro + rd * t;
        vec2  sd = sceneSDF(p);
        float d  = sd.x;

        // accumulate exponential fog
        fog += fogDensity * 1.5;

        if (d < SURF_EPS) {
            typeID = sd.y;
            tHit   = t;
            break;
        }
        if (t >= MAX_D) break;
        t += max(d * 0.7, SURF_EPS * 2.0);
    }

    fog = 1.0 - exp(-fog * fogDensity * t);

    // night sky colour
    vec3 skyCol = vec3(0.0, 0.02, 0.06);

    // accumulated neon glow along ray (volumetric sign light)
    vec3 neonGlowAccum = vec3(0.0);
    {
        float ts = 0.1;
        for (int i = 0; i < 24; i++) {
            if (ts >= min(tHit, MAX_D)) break;
            vec3  ps  = ro + rd * ts;
            vec2  sgn = neonSignsSDF(ps);
            if (sgn.x < 0.8) {
                float gIntensity = exp(-sgn.x * 18.0) * 0.06 * audio;
                neonGlowAccum += neonColor(sgn.y, hdrPeak) * gIntensity;
            }
            ts += max(sgn.x * 0.5, 0.3);
        }
    }

    // ── surface shading ──────────────────────────────────────────────────
    vec3 col = vec3(0.0);

    if (typeID >= 0.0 && tHit < MAX_D) {
        vec3  p = ro + rd * tHit;
        vec3  N = calcNormal(p);

        if (typeID >= 10.0) {
            // NEON SIGN — HDR emissive
            float signID = floor((typeID - 10.0) * 1000.0 + 0.5);
            vec3  nCol   = neonColor(signID, hdrPeak) * audio;

            // fwidth AA on sign edge
            vec2  sdSgn  = neonSignsSDF(p);
            float fw     = fwidth(sdSgn.x);
            float edge   = 1.0 - smoothstep(-fw, fw, sdSgn.x + SURF_EPS * 2.0);

            col = nCol * edge;

            // hot core bloom
            col += nCol * 0.4 * edge;

        } else if (typeID >= 2.0) {
            // GROUND — wet reflective pavement
            // base: very dark blue-grey
            col = vec3(0.0, 0.01, 0.03);

            // reflection: re-march the reflected ray
            vec3 reflRd   = reflect(rd, N);
            float reflT   = 0.1;
            vec3  reflCol = skyCol;
            for (int r = 0; r < 32; r++) {
                vec3  rp  = p + reflRd * reflT;
                vec2  rsd = sceneSDF(rp);
                if (rsd.x < SURF_EPS) {
                    if (rsd.y >= 10.0) {
                        float sID = floor((rsd.y - 10.0) * 1000.0 + 0.5);
                        reflCol   = neonColor(sID, hdrPeak * 0.7) * audio;
                    } else {
                        reflCol   = vec3(0.01, 0.02, 0.04);
                    }
                    break;
                }
                if (reflT >= 30.0) break;
                reflT += max(rsd.x * 0.7, SURF_EPS * 2.0);
            }

            // wet ground: strong neon reflections
            float fresnel   = pow(1.0 - max(dot(N, -rd), 0.0), 3.0);
            float reflAmt   = 0.65 + fresnel * 0.25;
            col = mix(col, reflCol, reflAmt);

            // puddle shimmer (hash-based)
            float shimmer = hash2(floor(p.xz * 4.0 + TIME * 0.5)) * 0.08;
            col += shimmer;

            // fwidth AA on ground edge (slight)
            float gd = groundSDF(p);
            float fw  = fwidth(gd);
            col *= smoothstep(-fw, fw, gd + SURF_EPS * 2.0) + 0.6;

        } else {
            // BUILDING — dark facade with faint ambient neon bounce
            col = vec3(0.0, 0.0, 0.02);

            // soft neon colour ambient on building faces
            float bldID = mod(typeID * 1000.0, 8.0);
            vec3  nAmb  = neonColor(bldID, 0.12) * audio;
            float normUp = max(dot(N, vec3(0.0, 1.0, 0.0)), 0.0);
            col += nAmb * (1.0 - normUp);

            // fwidth AA on building edges
            vec2  sdBld = buildingsSDF(p);
            float fw    = fwidth(sdBld.x);
            col *= smoothstep(-fw, fw, sdBld.x + SURF_EPS * 2.0) + 0.85;
        }
    } else {
        // sky / miss
        col = skyCol;
    }

    // add volumetric neon glow
    col += neonGlowAccum;

    // ── fog blend ────────────────────────────────────────────────────────
    vec3 fogCol = skyCol + vec3(0.0, 0.01, 0.04); // slightly lighter than sky
    col = mix(col, fogCol, fog);

    // ── rain streaks ─────────────────────────────────────────────────────
    vec2  rainUV   = isf_FragNormCoord * RENDERSIZE / 4.0;
    float rainHash = hash2(floor(rainUV) + floor(TIME * 20.0));
    float rainMask = step(1.0 - rainDensity * 0.005 * highAudio, rainHash);
    // streak: thin vertical alpha
    float streak   = rainMask * (1.0 - abs(fract(rainUV.y) - 0.5) * 2.0);
    col += vec3(0.5, 0.7, 0.9) * streak * 0.8 * rainDensity;

    gl_FragColor = vec4(col, 1.0);
}
