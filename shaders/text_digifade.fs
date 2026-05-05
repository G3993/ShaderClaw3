/*{
  "DESCRIPTION": "Data Void — 3D raymarched floating data panels in deep black space. Each panel glows electric blue/violet at its edges and displays a flickering data-cell grid. Magenta HDR spikes. Camera drifts forward.",
  "CREDIT": "ShaderClaw auto-improve",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "INPUTS": [
    {"NAME":"speed",       "LABEL":"Speed",        "TYPE":"float","DEFAULT":0.3, "MIN":0.0,"MAX":1.5},
    {"NAME":"gridDensity", "LABEL":"Grid Density",  "TYPE":"float","DEFAULT":8.0, "MIN":4.0,"MAX":16.0},
    {"NAME":"hdrPeak",    "LABEL":"HDR Peak",     "TYPE":"float","DEFAULT":2.5, "MIN":1.0,"MAX":4.0},
    {"NAME":"audioReact", "LABEL":"Audio React",  "TYPE":"float","DEFAULT":0.6, "MIN":0.0,"MAX":2.0},
    {"NAME":"panelCount", "LABEL":"Panel Count",  "TYPE":"float","DEFAULT":6.0, "MIN":3.0,"MAX":12.0}
  ]
}*/

// ── Palette ──────────────────────────────────────────────────────────────────
const vec3 COL_VOID       = vec3(0.0, 0.0, 0.01);
const vec3 COL_PANEL_FACE = vec3(0.0, 0.02, 0.05);
const vec3 COL_DATA_BLUE  = vec3(0.0, 0.5, 1.0);   // * 2.0
const vec3 COL_EDGE_VIOL  = vec3(0.2, 0.0, 1.0);   // * 2.5
const vec3 COL_SPIKE_MAG  = vec3(1.0, 0.0, 0.8);   // * 3.0

// ── Hash helpers ─────────────────────────────────────────────────────────────
float hash11(float n)  { return fract(sin(n * 127.1) * 43758.5453); }
float hash12(vec2 p)   { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float hash13(vec3 p)   { return fract(sin(dot(p, vec3(127.1, 311.7, 74.7))) * 43758.5453); }

// ── SDF helpers ──────────────────────────────────────────────────────────────
float sdBox(vec3 p, vec3 b) {
    vec3 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, max(d.y, d.z)), 0.0);
}

// ── Rotation matrices ─────────────────────────────────────────────────────────
mat3 rotX(float a) {
    float s = sin(a), c = cos(a);
    return mat3(1.0, 0.0, 0.0,  0.0, c, -s,  0.0, s, c);
}
mat3 rotY(float a) {
    float s = sin(a), c = cos(a);
    return mat3(c, 0.0, s,  0.0, 1.0, 0.0,  -s, 0.0, c);
}
mat3 rotZ(float a) {
    float s = sin(a), c = cos(a);
    return mat3(c, -s, 0.0,  s, c, 0.0,  0.0, 0.0, 1.0);
}

// ── Panel geometry ─────────────────────────────────────────────────────────
// Half-extents for each panel element
const vec3 PANEL_SIZE = vec3(1.2, 0.8, 0.02);
const vec3 GLOW_SIZE  = vec3(1.25, 0.85, 0.07);

// ── Panel scene SDF + hit info ────────────────────────────────────────────────
// Returns (dist, panelIndex * 10 + matID)
// matID: 0 = face, 1 = edge glow region
vec2 panelSDF(vec3 ro_p, int idx) {
    float fi = float(idx);

    // Each panel positioned on a sphere shell, orbit determined by index
    float angle   = fi * 1.0472 + TIME * (0.03 + hash11(fi + 0.5) * 0.04); // spread + slow orbit
    float radius  = 3.5 + hash11(fi * 1.3) * 2.5;
    float elevation = (hash11(fi * 2.7) - 0.5) * 2.4;

    vec3 panelPos = vec3(sin(angle) * radius, elevation, cos(angle) * radius);

    // Panel rotation: varying pitch/yaw per panel
    float pitch = hash11(fi * 3.1) * 1.5 - 0.75 + TIME * 0.012 * (hash11(fi * 4.3) - 0.5);
    float yaw   = hash11(fi * 5.7) * 6.28318 + TIME * 0.018 * (hash11(fi * 6.9) - 0.5);
    float roll  = hash11(fi * 7.3) * 0.8 - 0.4;

    // Transform point into panel local space
    vec3 lp = ro_p - panelPos;
    lp = rotY(-yaw) * lp;
    lp = rotX(-pitch) * lp;
    lp = rotZ(-roll) * lp;

    float dPanel = sdBox(lp, PANEL_SIZE);
    float dGlow  = sdBox(lp, GLOW_SIZE);

    // Edge region: between inner panel and outer glow box
    float dEdge = max(dGlow, -dPanel + 0.01);   // shell around panel face

    if (dPanel < dEdge) return vec2(dPanel, fi * 10.0 + 0.0);
    return vec2(dEdge,  fi * 10.0 + 1.0);
}

// ── Full scene ────────────────────────────────────────────────────────────────
vec2 sceneSDF(vec3 p) {
    vec2 res = vec2(1e9, -1.0);
    int n = int(clamp(panelCount, 3.0, 12.0));
    for (int i = 0; i < 12; i++) {
        if (i >= n) break;
        vec2 pr = panelSDF(p, i);
        if (pr.x < res.x) res = pr;
    }
    return res;
}

// ── Normal via finite differences ─────────────────────────────────────────────
vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneSDF(p + e.xyy).x - sceneSDF(p - e.xyy).x,
        sceneSDF(p + e.yxy).x - sceneSDF(p - e.yxy).x,
        sceneSDF(p + e.yyx).x - sceneSDF(p - e.yyx).x
    ));
}

// ── Data grid pattern on panel face ─────────────────────────────────────────
// p:       world hit point (needs to be in panel local space, supplied as localUV)
// returns: vec3 color emitted by the panel face
vec3 dataFaceColor(vec2 localUV, float panelIdx, float audioBst) {
    // localUV in range [-1, 1] for the panel face
    vec2 normUV = localUV * 0.5 + 0.5;   // [0, 1]

    vec2 gridUV  = fract(normUV * gridDensity);
    vec2 cellID  = floor(normUV * gridDensity);

    // Cell border (fwidth AA)
    float border = 0.10;
    float fwx    = fwidth(gridUV.x);
    float fwy    = fwidth(gridUV.y);
    float lineX  = smoothstep(border - fwx, border + fwx, gridUV.x) *
                   smoothstep(border - fwx, border + fwx, 1.0 - gridUV.x);
    float lineY  = smoothstep(border - fwy, border + fwy, gridUV.y) *
                   smoothstep(border - fwy, border + fwy, 1.0 - gridUV.y);
    float inCell = lineX * lineY;

    // Cell activity: hash by cellID + panel index, flicker at ~3 fps
    float cellHash = hash12(cellID + vec2(panelIdx * 47.3, panelIdx * 31.1));
    float timeSlot = floor(TIME * 3.0);
    float flicker  = hash12(cellID + vec2(panelIdx + timeSlot * 13.7, timeSlot * 7.3));
    float active   = step(0.55, flicker) * step(0.3, cellHash);

    // Hot-spike: every ~5 seconds a single cell blasts magenta
    float spikeSlot  = floor(TIME * 0.2);
    float spikeCellX = floor(hash11(panelIdx + spikeSlot * 53.1) * gridDensity);
    float spikeCellY = floor(hash11(panelIdx + spikeSlot * 79.3) * gridDensity);
    // Use abs distance < 0.5 to test float cell equality (both are floor() results)
    float matchX     = step(abs(cellID.x - spikeCellX), 0.5);
    float matchY     = step(abs(cellID.y - spikeCellY), 0.5);
    float isSpike    = matchX * matchY;
    // Smooth the spike in/out
    float spikeFade  = sin(fract(TIME * 0.2) * 3.14159) * 0.8;

    // Base cell color
    vec3 cellCol = COL_DATA_BLUE * 2.0 * active * audioBst;
    cellCol     += COL_SPIKE_MAG * 3.0 * isSpike * spikeFade * audioBst;

    // Panel face base (dark)
    vec3 face = COL_PANEL_FACE;
    face += cellCol * inCell;

    return face;
}

// ── Main ─────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv = isf_FragNormCoord * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;

    // Camera: drift forward, gentle yaw
    float camZ   = -TIME * speed * 1.5;
    float camYaw = sin(TIME * 0.07) * 0.3;
    vec3 ro = vec3(sin(camYaw) * 0.8, sin(TIME * 0.04) * 0.3, camZ);
    vec3 fwd   = normalize(vec3(sin(camYaw) * 0.1, 0.0, 1.0));
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), fwd));
    vec3 up    = cross(fwd, right);
    vec3 rd    = normalize(uv.x * right + uv.y * up + fwd * 1.7);

    float audioBst = 1.0 + audioLevel * audioReact;

    // ── Raymarch ─────────────────────────────────────────────────────────────
    float t     = 0.1;
    float matID = -1.0;
    vec3  p     = ro;
    bool  hit   = false;

    for (int i = 0; i < 64; i++) {
        p = ro + rd * t;
        vec2 res = sceneSDF(p);
        float d  = res.x;
        if (d < 0.003) { hit = true; matID = res.y; break; }
        if (t > 40.0)  break;
        t += max(d * 0.8, 0.01);
    }

    vec3 col = COL_VOID;

    if (hit) {
        float panelIdx = floor(matID / 10.0);
        float surfID   = mod(matID, 10.0);

        vec3 n = calcNormal(p);

        // ── Panel face ───────────────────────────────────────────────────
        if (surfID < 0.5) {
            // Reconstruct panel local UV from normal and world position
            // Use cross product of n to form local axes (approximate)
            // Since panels are thin, n ≈ panel face normal in world space
            // Project p onto plane perpendicular to n for local UV
            vec3 right2 = normalize(cross(n, vec3(0.0, 1.0, 0.0)));
            vec3 up2    = cross(right2, n);
            // Reconstruct panel center (approx using panel radius + orbit)
            float fi      = panelIdx;
            float angle   = fi * 1.0472 + TIME * (0.03 + hash11(fi + 0.5) * 0.04);
            float radius  = 3.5 + hash11(fi * 1.3) * 2.5;
            float elev    = (hash11(fi * 2.7) - 0.5) * 2.4;
            vec3 pCenter  = vec3(sin(angle) * radius, elev, cos(angle) * radius);
            vec3 dp       = p - pCenter;
            vec2 localUV  = vec2(dot(dp, right2), dot(dp, up2));
            // Normalize to [-1, 1] over panel half-extents
            localUV      /= vec2(1.2, 0.8);

            col = dataFaceColor(localUV, panelIdx, audioBst);

            // Subtle Fresnel rim from edges
            float rim = 1.0 - abs(dot(n, normalize(ro - p)));
            col += COL_EDGE_VIOL * 2.5 * pow(rim, 3.0) * 0.4 * audioBst;
        }
        // ── Edge glow region ─────────────────────────────────────────────
        else {
            // Glow based on distance from panel face SDF
            float fi      = panelIdx;
            float angle   = fi * 1.0472 + TIME * (0.03 + hash11(fi + 0.5) * 0.04);
            float radius  = 3.5 + hash11(fi * 1.3) * 2.5;
            float elev    = (hash11(fi * 2.7) - 0.5) * 2.4;
            vec3 pCenter  = vec3(sin(angle) * radius, elev, cos(angle) * radius);

            // Approx edge distance via SDF of inner box
            float pitch = hash11(fi * 3.1) * 1.5 - 0.75 + TIME * 0.012 * (hash11(fi * 4.3) - 0.5);
            float yaw   = hash11(fi * 5.7) * 6.28318 + TIME * 0.018 * (hash11(fi * 6.9) - 0.5);
            float roll  = hash11(fi * 7.3) * 0.8 - 0.4;
            vec3 lp2    = p - pCenter;
            lp2 = rotY(-yaw) * lp2;
            lp2 = rotX(-pitch) * lp2;
            lp2 = rotZ(-roll) * lp2;
            float innerD = sdBox(lp2, PANEL_SIZE);
            // fwidth AA on glow falloff
            float fw     = fwidth(innerD);
            float glowF  = exp(-innerD * 25.0);
            glowF        = smoothstep(fw, 0.0, innerD - 0.01) * 0.3 + glowF * 0.7;

            col = COL_EDGE_VIOL * 2.5 * glowF * hdrPeak * audioBst;
            // Mix in audio-reactive spike on edges
            col += COL_DATA_BLUE * 2.0 * glowF * audioBass * audioReact * 0.4;
        }

        // Depth fade to void
        float fogT = 1.0 - exp(-t * 0.03);
        col = mix(col, COL_VOID, fogT * 0.85);
    }

    // ── Additive edge glow halos bleed into surrounding space ────────────────
    // Sample a few ray points and accumulate panel proximity glow
    int np = int(clamp(panelCount, 3.0, 12.0));
    for (int k = 0; k < 6; k++) {
        float kt = 0.5 + float(k) * 3.5;
        if (kt > t) break;
        vec3 kp = ro + rd * kt;
        for (int idx = 0; idx < 12; idx++) {
            if (idx >= np) break;
            vec2 pr = panelSDF(kp, idx);
            col += COL_EDGE_VIOL * 2.5 * exp(-pr.x * 12.0) * 0.008 * audioBst;
            col += COL_DATA_BLUE * 2.0 * exp(-pr.x * 20.0) * 0.005 * audioBst;
        }
    }

    gl_FragColor = vec4(col, 1.0);
}
