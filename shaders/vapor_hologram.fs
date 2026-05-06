/*{
  "DESCRIPTION": "Neon Cityscape 3D — street-level raymarched cyberpunk corridor with sdBox buildings, neon strip lights, glossy street reflections. William Gibson night city aesthetic.",
  "CATEGORIES": ["Generator", "3D", "Audio Reactive"],
  "CREDIT": "ShaderClaw — full rewrite to 3D cyberpunk raymarcher",
  "ISFVSN": "2",
  "INPUTS": [
    { "NAME": "speed",       "LABEL": "Camera Speed",   "TYPE": "float", "MIN": 0.0,  "MAX": 4.0,  "DEFAULT": 1.0 },
    { "NAME": "cityDensity", "LABEL": "City Density",   "TYPE": "float", "MIN": 0.3,  "MAX": 2.0,  "DEFAULT": 1.0 },
    { "NAME": "hdrPeak",     "LABEL": "HDR Peak",       "TYPE": "float", "MIN": 1.0,  "MAX": 5.0,  "DEFAULT": 2.5 },
    { "NAME": "audioReact",  "LABEL": "Audio React",    "TYPE": "float", "MIN": 0.0,  "MAX": 2.0,  "DEFAULT": 0.5 }
  ]
}*/

// ── Hash utilities ────────────────────────────────────────────────────────────
float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
float hash21(vec2 p)  { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

// Building height: [2,8] from cell id
float buildingHeight(vec2 cell) {
    return 2.0 + hash21(cell + vec2(3.71, 9.13)) * 6.0;
}

// Per-building neon color index → one of 3 HDR colors
vec3 neonColor(float idx, float pk) {
    float t = fract(idx * 0.333 + 0.05);
    if (t < 0.333) return vec3(0.0, 2.5, 2.0) * pk;           // electric cyan
    if (t < 0.667) return vec3(2.0, 0.0, 1.5) * pk;           // neon magenta
    return             vec3(2.5, 0.8, 0.0) * pk;               // warm orange
}

// ── SDF: axis-aligned box centred at origin ───────────────────────────────────
float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

// ── Scene SDF ────────────────────────────────────────────────────────────────
// Returns vec2(dist, material_id)
//   material 0 = nothing, 1 = building, 2 = street
vec2 sceneMap(vec3 p) {
    float streetY = p.y;                          // street at y=0
    vec2  best    = vec2(streetY, 2.0);           // floor plane

    // Buildings repeated along X and Z with cityDensity spacing
    float spacing = 4.0 / cityDensity;
    float halfW   = 1.2 / cityDensity;            // half-width of building block

    // Two rows: left (x < -street_gap) and right (x > +street_gap)
    float streetGap = 1.8;

    for (int row = 0; row < 2; row++) {
        float rowSign = (row == 0) ? -1.0 : 1.0;
        float rowCenterX = rowSign * (streetGap + halfW + 0.2);

        // Which building cell are we near along Z?
        float cellZ = floor((p.z + spacing * 0.5) / spacing);

        // Check 3 consecutive cells for smooth transitions
        for (int ci = -1; ci <= 1; ci++) {
            float cz    = cellZ + float(ci);
            vec2 cellId = vec2(float(row) + 7.3, cz * 1.91 + 3.7);
            float bH    = buildingHeight(cellId);
            float bD    = halfW * 1.6;             // depth along Z
            vec3  bCtr  = vec3(rowCenterX, bH * 0.5, cz * spacing);
            vec3  bHalf = vec3(halfW, bH * 0.5, bD);

            float d = sdBox(p - bCtr, bHalf);
            if (d < best.x) best = vec2(d, 1.0);
        }
    }
    return best;
}

// ── Normal via central differences ───────────────────────────────────────────
vec3 calcNormal(vec3 p) {
    vec2 e = vec2(0.001, 0.0);
    return normalize(vec3(
        sceneMap(p + e.xyy).x - sceneMap(p - e.xyy).x,
        sceneMap(p + e.yxy).x - sceneMap(p - e.yxy).x,
        sceneMap(p + e.yyx).x - sceneMap(p - e.yyx).x
    ));
}

// ── Neon strip colour at a hit point on a building face ───────────────────────
vec3 neonStrips(vec3 hp, vec2 cellId, float bH, float pk, float audioMod) {
    // Multiple horizontal strips at fractions of building height
    float yFrac  = hp.y / max(bH, 0.001);
    float d1     = abs(fract(yFrac * 2.5) - 0.5);          // repeating strips
    float stripD = d1 * bH * 0.5;

    // fwidth AA on strip edge
    float fw    = fwidth(stripD);
    float neon  = smoothstep(fw, 0.0, stripD - 0.05);

    float idx   = hash21(cellId + vec2(1.1, 2.2));
    vec3  nc    = neonColor(idx, pk);
    return nc * neon * audioMod;
}

// ── Sky glow ─────────────────────────────────────────────────────────────────
vec3 skyColor(vec3 rd) {
    float horizon = exp(-abs(rd.y) * 6.0);
    vec3  base    = vec3(0.0, 0.0, 0.01);            // void dark
    vec3  hGlow   = mix(vec3(2.0, 0.0, 1.5), vec3(0.0, 2.5, 2.0),
                        clamp(rd.x * 0.5 + 0.5, 0.0, 1.0)) * 0.18;
    return base + hGlow * horizon;
}

// ── Main ─────────────────────────────────────────────────────────────────────
void main() {
    vec2 uv    = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;
    uv.x      *= RENDERSIZE.x / max(RENDERSIZE.y, 1.0);

    // Audio modulator
    float audio = 1.0 + (audioLevel * 0.4 + audioBass * 0.6) * audioReact;

    // Camera: street-level, advancing along +Z
    float camZ   = TIME * speed;
    float bob    = sin(TIME * 1.3) * 0.06;
    vec3  ro     = vec3(sin(TIME * 0.17) * 0.25, 1.0 + bob, camZ);

    // Look direction: slight downward tilt
    vec3  forward = normalize(vec3(0.0, -0.08, 1.0));
    vec3  right   = normalize(cross(forward, vec3(0.0, 1.0, 0.0)));
    vec3  up      = cross(right, forward);
    vec3  rd      = normalize(forward + uv.x * right * 0.7 + uv.y * up * 0.7);

    // ── Raymarching (64 steps) ──
    float t    = 0.02;
    float tMax = 60.0;
    float matId = 0.0;
    bool  hit  = false;

    for (int i = 0; i < 64; i++) {
        vec3  p   = ro + rd * t;
        vec2  res = sceneMap(p);
        if (res.x < 0.001) {
            matId = res.y;
            hit   = true;
            break;
        }
        t += max(res.x * 0.9, 0.002);
        if (t > tMax) break;
    }

    // ── Shading ──
    vec3 col = skyColor(rd);

    if (hit) {
        vec3  hp = ro + rd * t;
        vec3  n  = calcNormal(hp);
        float pk = hdrPeak;

        if (matId > 1.5) {
            // ── Street ──
            vec3 streetBase = vec3(0.005, 0.005, 0.012);
            // Glossy reflection of sky/neon glow
            vec3 refl = skyColor(reflect(rd, n)) * 0.35;
            // Distance fade for reflectivity
            float rfade = exp(-t * 0.04);
            col = streetBase + refl * rfade;

            // Add faint grid lines for depth cue
            vec2  gridUV  = hp.xz * 0.5;
            vec2  gd      = abs(fract(gridUV) - 0.5);
            float fw      = length(fwidth(gridUV));
            float gridLine = smoothstep(fw, 0.0, min(gd.x, gd.y) - 0.02);
            col += vec3(0.0, 0.5, 0.4) * gridLine * 0.15;

        } else {
            // ── Building ──
            // Building dark base
            col = vec3(0.01, 0.01, 0.02);

            // Determine which cell this building belongs to
            float spacing  = 4.0 / cityDensity;
            float streetGap = 1.8;
            float halfW     = 1.2 / cityDensity;

            // Find nearest building row and cell
            float rowSign   = sign(hp.x);
            float rowF      = rowSign > 0.0 ? 1.0 : 0.0;
            float cellZ     = floor((hp.z + spacing * 0.5) / spacing);
            vec2  cellId    = vec2(rowF + 7.3, cellZ * 1.91 + 3.7);
            float bH        = buildingHeight(cellId);

            // Neon strips
            col += neonStrips(hp, cellId, bH, pk, audio);

            // Window glow: tiny bright spots scattered on facade
            vec2  winUV   = vec2(hp.y * 1.5, abs(hp.x - sign(hp.x) * (streetGap + halfW + 0.2)) * 4.0 + hp.z);
            float wHash   = hash21(floor(winUV * 3.0));
            float winOn   = step(0.7, wHash);
            float winIdx  = hash21(floor(winUV * 3.0) + vec2(5.5, 9.1));
            vec3  winCol  = neonColor(winIdx, pk * 0.3) * winOn;
            float winMask = smoothstep(0.35, 0.0, length(fract(winUV * 3.0) - 0.5));
            col += winCol * winMask * 0.4 * audio;

            // Ambient edge glow from neon environment
            float envAO = exp(-t * 0.03);
            col += vec3(0.0, 0.02, 0.04) * envAO;
        }

        // Fog (distance fade to dark city void)
        float fog  = exp(-t * 0.055);
        col        = mix(vec3(0.0, 0.0, 0.01), col, fog);
    }

    // Very slight vignette for cinematic feel
    float vig = 1.0 - 0.35 * dot(uv * vec2(0.7), uv * vec2(0.7));
    col *= vig;

    gl_FragColor = vec4(col, 1.0);
}
