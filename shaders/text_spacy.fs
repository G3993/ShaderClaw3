/*{
  "DESCRIPTION": "Wormhole Portal — 3D raymarched infinite tunnel of torus rings receding into a glowing violet/cyan/gold portal. Camera dives into the wormhole with audio-reactive speed and glow.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"speed",       "LABEL":"Speed",         "TYPE":"float","DEFAULT":0.8, "MIN":0.1,"MAX":3.0},
    {"NAME":"ringSpacing", "LABEL":"Ring Spacing",  "TYPE":"float","DEFAULT":1.2, "MIN":0.5,"MAX":3.0},
    {"NAME":"tubeRadius",  "LABEL":"Tube Radius",   "TYPE":"float","DEFAULT":0.8, "MIN":0.3,"MAX":1.5},
    {"NAME":"tubeThick",   "LABEL":"Tube Thickness","TYPE":"float","DEFAULT":0.06,"MIN":0.01,"MAX":0.2},
    {"NAME":"hdrPeak",     "LABEL":"HDR Peak",      "TYPE":"float","DEFAULT":2.5, "MIN":1.0,"MAX":4.0},
    {"NAME":"audioReact",  "LABEL":"Audio React",   "TYPE":"float","DEFAULT":0.6, "MIN":0.0,"MAX":2.0}
  ]
}*/

// ── constants ──────────────────────────────────────────────────────────────
const float PI        = 3.14159265359;
const int   MARCH_STEPS = 64;
const float MAX_DIST  = 18.0;
const float SURF_EPS  = 0.0015;
const int   N_RINGS   = 12;

// ── SDF: torus centred at origin, main axis = Z ───────────────────────────
// t.x = major radius (ring centre distance from Z axis), t.y = tube radius
float sdTorus(vec3 p, vec2 t) {
    vec2 q = vec2(length(p.xy) - t.x, p.z);
    return length(q) - t.y;
}

// ── scene SDF — returns vec2(dist, ringIndex) ─────────────────────────────
vec2 sceneSDF(vec3 p, float scroll) {
    float best  = 1e9;
    float bestI = -1.0;
    float band  = ringSpacing * float(N_RINGS);

    for (int i = 0; i < N_RINGS; i++) {
        float fi   = float(i);
        float zBase = fi * ringSpacing - scroll;
        // wrap so rings tile infinitely ahead
        float zPos  = zBase - floor(zBase / band) * band;
        if (zPos > MAX_DIST + ringSpacing) continue;

        vec3  rp = p - vec3(0.0, 0.0, zPos);
        float d  = sdTorus(rp, vec2(tubeRadius, tubeThick));
        if (d < best) { best = d; bestI = fi; }
    }
    return vec2(best, bestI);
}

// ── finite-difference normal ───────────────────────────────────────────────
vec3 calcNormal(vec3 p, float scroll) {
    const float e = 0.001;
    return normalize(vec3(
        sceneSDF(p + vec3( e, 0, 0), scroll).x - sceneSDF(p - vec3( e, 0, 0), scroll).x,
        sceneSDF(p + vec3(0,  e, 0), scroll).x - sceneSDF(p - vec3(0,  e, 0), scroll).x,
        sceneSDF(p + vec3(0, 0,  e), scroll).x - sceneSDF(p - vec3(0, 0,  e), scroll).x
    ));
}

// ── ring colour by index — violet / gold / hot-magenta cycle ──────────────
vec3 ringColor(float idx, float peak) {
    int kind = int(mod(idx, 3.0));
    if (kind == 0) return vec3(0.5, 0.0, 1.0) * peak;             // violet
    if (kind == 1) return vec3(1.0, 0.8, 0.0) * (peak * 0.8);    // gold
    return              vec3(1.0, 0.0, 0.7) * (peak * 1.12);     // hot magenta
}

// ── main ───────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    float audio      = 1.0 + audioLevel * audioReact;
    float bassAudio  = 1.0 + audioBass  * audioReact;

    // scroll: infinite travel into the tunnel
    float effectiveSpeed = speed * bassAudio;
    float band           = ringSpacing * float(N_RINGS);
    float scroll         = mod(TIME * effectiveSpeed, band);

    // camera: gentle corkscrewing spiral wobble
    float T   = TIME * 0.2;
    vec3  ro  = vec3(sin(T) * 0.12, cos(T) * 0.12, 0.0);
    // ray direction: slight perspective cone into +Z
    vec3  rd  = normalize(vec3(uv * 0.65, 1.0));

    // ── raymarching ──────────────────────────────────────────────────────
    float t      = 0.05;
    float ringHit = -1.0;
    float tHit    = MAX_DIST;
    float minDist = 1e9;

    for (int i = 0; i < MARCH_STEPS; i++) {
        vec3  p  = ro + rd * t;
        vec2  sd = sceneSDF(p, scroll);
        float d  = sd.x;

        minDist = min(minDist, d);

        if (d < SURF_EPS) {
            ringHit = sd.y;
            tHit    = t;
            break;
        }
        if (t >= MAX_DIST) break;
        t += max(d * 0.55, SURF_EPS * 2.0);
    }

    // ── compositing ──────────────────────────────────────────────────────
    vec3 col = vec3(0.0); // void black background

    // portal glow — cyan HDR bloom from the vanishing point
    float portalDist = length(uv * 0.6);
    vec3  portalCol  = vec3(0.0, 1.0, 0.9) * 3.0
                     * exp(-portalDist * 3.5)
                     * exp(-tHit * 0.22)
                     * audio;
    col += portalCol;

    // inter-ring volumetric scatter (portal light leaking between rings)
    float volGlow = exp(-minDist * 14.0) * 0.35 * audio;
    col += vec3(0.1, 0.5, 1.0) * volGlow * hdrPeak * 0.28;

    if (ringHit >= 0.0) {
        vec3  p       = ro + rd * tHit;
        vec3  N       = calcNormal(p, scroll);
        vec3  toPortal = normalize(vec3(0.0, 0.0, MAX_DIST) - p);
        float diff    = max(dot(N, toPortal), 0.0) * 0.7 + 0.3;

        vec3 rCol = ringColor(ringHit, hdrPeak) * diff;

        // fwidth AA on the torus surface edge
        vec2  sdEdge = sceneSDF(p, scroll);
        float fw     = fwidth(sdEdge.x);
        float edge   = 1.0 - smoothstep(-fw, fw, sdEdge.x + SURF_EPS * 3.0);

        // fresnel-like rim glow in portal cyan
        float rim = pow(1.0 - abs(dot(N, -rd)), 3.0);
        rCol += vec3(0.0, 1.0, 0.9) * rim * 1.6 * hdrPeak * audio;

        // specular highlight from portal direction
        vec3  H    = normalize(toPortal - rd);
        float spec = pow(max(dot(N, H), 0.0), 32.0);
        rCol += vec3(1.0, 0.95, 0.8) * spec * hdrPeak * 0.45;

        col += rCol * edge;
    }

    // faint cylindrical tunnel wall envelope
    float wallR   = tubeRadius * 1.18;
    float rdxyLen = max(length(rd.xy), 0.001);
    float wallD   = abs(length(ro.xy + rd.xy * (wallR / rdxyLen)) - wallR);
    float wallGlow = exp(-wallD * 9.0) * 0.055 * audio;
    col += vec3(0.28, 0.0, 0.58) * wallGlow * hdrPeak;

    gl_FragColor = vec4(col, 1.0);
}
